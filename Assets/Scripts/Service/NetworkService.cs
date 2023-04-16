using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameClient.Service
{
    class NetworkService
    {
        private Socket _clientSocket;
        private readonly string _serverIp;
        private readonly int _serverPort;
        private readonly int _bufferSize;
        private readonly int _poolSize;
        private BufferManager _bufferManager;  // represents a large reusable set of buffers for all socket operations
        private SocketAsyncEventArgs _readWriteEventArg;
        private SocketAsyncEventArgsPool _readWritePool;
        private Semaphore _poolSemaphore;
        private static readonly Lazy<NetworkService> _instance = new Lazy<NetworkService>(() => new NetworkService());
        public static NetworkService Instance => _instance.Value;

        public RecvBufQueue RecvBufQueue { get => _recvBufQueue; }

        public event Action<ConnectState> OnConnectStateChange; //这个事件到类外注册
        public ConnectState ConnectState
        {
            get => _connectState;
            private set
            {
                if (_connectState != value)
                {
                    OnConnectStateChange?.Invoke(value);    //触发事件
                }
                _connectState = value;
            }
        }

        private RecvBufQueue _recvBufQueue;

        private ConnectState _connectState;

        private NetworkService()
        {
            _serverIp = "127.0.0.1";    // TODO: 从配置文件中读入
            _serverPort = 8080;
            _bufferSize = 1024;
            _poolSize = 128;

            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _bufferManager = new BufferManager(_bufferSize * _poolSize, _bufferSize); //按bufferSize将BufferManager中的m_buffer切分
            _bufferManager.InitBuffer();
            _readWritePool = new SocketAsyncEventArgsPool(_poolSize);
            _recvBufQueue = new RecvBufQueue(_bufferSize * 10);

            //设置readWriteEventArg属性
            for (int i = 0; i < _poolSize; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs
                _readWriteEventArg = new SocketAsyncEventArgs();
                _readWriteEventArg.UserToken = _clientSocket;
                _readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                _bufferManager.SetBuffer(_readWriteEventArg);
                // add SocketAsyncEventArg to the pool
                _readWritePool.Push(_readWriteEventArg);
            }
            //初始化信号量
            _poolSemaphore = new Semaphore(_poolSize, _poolSize);
        }

        public void Start()
        {
            //异步请求连接
            var connectTask = ConnectServerAsync();
            ConnectState = ConnectState.Connecting;
            _ = connectTask.ContinueWith((t) => ConnectState = t.IsFaulted ? ConnectState.Disconnect : ConnectState.Connected/*, TaskScheduler.FromCurrentSynchronizationContext()*/);
            //注意:Unity主线程拥有上下文任务调度器，可以让延续任务在主线程中执行
        }

        public void Close()
        {
            _poolSemaphore.Dispose();
            _clientSocket.Close();
        }

        private async Task ConnectServerAsync()
        {
            await _clientSocket.ConnectAsync(_serverIp, _serverPort).ConfigureAwait(false); //注意:Unity主线程拥有同步上下文,默认情况下await后的代码会切换到主线程中执行,配置ConfigureAwait(false)可以减少上下文切换
            //投递消息接收
            _poolSemaphore.WaitOne();   //申请信号量
            SocketAsyncEventArgs readEventArgs = _readWritePool.Pop();

            if (_clientSocket.ReceiveAsync(readEventArgs) == false)
            {
                OnReceiveCompleted(this, readEventArgs);
            }

            //等价写法1
            //_clientSocket.ConnectAsync(_serverIp, _serverPort).GetAwaiter().OnCompleted(() =>/*await后的代码*/);  //OnCompleted中的代码在当前线程中执行
            //等价写法2
            //_clientSocket.ConnectAsync(_serverIp, _serverPort).ContinueWith((t) =>/*await后的代码*/);
        }

        public async Task SendCommandAsync(string jsonData)
        {
            if (!_clientSocket.Connected)
            {
                //避免重复请求连接
                if (ConnectState == ConnectState.Connecting)
                {
                    return;
                }
                ConnectState = ConnectState.Connecting;
                try
                {
                    await ConnectServerAsync();
                    ConnectState = ConnectState.Connected;
                }
                catch (Exception)
                {
                    ConnectState = ConnectState.Disconnect;
                    return;
                }
            }
            //发送数据
            var sendMsgBuffer = Encoding.Default.GetBytes(jsonData);
            if (sendMsgBuffer.Length > _bufferSize - 4)
            {
                throw new Exception("单个命令数据包太大!");
            }

            _poolSemaphore.WaitOne();
            SocketAsyncEventArgs writeEventArgs = _readWritePool.Pop();
            Array.Copy(BitConverter.GetBytes(sendMsgBuffer.Length), 0, writeEventArgs.Buffer, writeEventArgs.Offset, 4);    //用前4个字节存储包的大小
            Array.Copy(sendMsgBuffer, 0, writeEventArgs.Buffer, writeEventArgs.Offset + 4, sendMsgBuffer.Length);   //将信息拷到readWriteEventArgs的buffer里
            writeEventArgs.SetBuffer(writeEventArgs.Offset, sendMsgBuffer.Length + 4);  //重新设置发送Buffer的大小
            if (_clientSocket.SendAsync(writeEventArgs) == false)
            {
                OnSendCompleted(this, writeEventArgs);
            }
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                //读取数据  
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                _recvBufQueue.Enqueue(data);
            }
            else if (e.SocketError == SocketError.ConnectionReset && e.BytesTransferred == 0)
            {
                ConnectState = ConnectState.Disconnect;
                //Socket socket = e.UserToken as Socket;
                // close the socket associated with the client
                //socket.Close();
                // Free the SocketAsyncEventArg so they can be reused by another client
                _readWritePool.Push(e);
                _poolSemaphore.Release();
                return;
            }
            else
            {
                // TODO: 错误处理
                return;
            }
            //继续接收
            if (_clientSocket.ReceiveAsync(e) == false)
            {
                OnReceiveCompleted(sender, e);
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //Console.WriteLine("成功发送了" + e.BytesTransferred + "个字节！");
            }
            else
            {
                // TODO: log4net
                //Console.WriteLine(Encoding.Default.GetString(e.Buffer) + " 发送错误：" + e.SocketError.ToString());
            }
            // Free the SocketAsyncEventArg so they can be reused by another client
            e.SetBuffer(e.Offset, _bufferSize); //重置bufferSize的大小(SendCommand是会把大小设为发送数据的长度)
            _readWritePool.Push(e);
            _poolSemaphore.Release();   //释放信号量
        }
        // This method is called whenever a receive or send operation is completed on a socket 
        //
        // <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    OnReceiveCompleted(sender, e);
                    break;
                case SocketAsyncOperation.Send:
                    OnSendCompleted(sender, e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }
    }

    class RecvBufQueue
    {
        private List<byte> _dataList;
        //public ManualResetEventSlim MessageComeEvent { get; private set; }
        public bool MessageFlag { get; set; }
        public RecvBufQueue(int size)
        {
            _dataList = new List<byte>(size);
            //MessageComeEvent = new ManualResetEventSlim(false);
        }
        public void Enqueue(byte[] data)
        {
            lock (_dataList)
            {
                _dataList.AddRange(data);
                //MessageComeEvent.Set();
                MessageFlag = true;
            }
        }
        public string Dequeue()
        {
            if (_dataList.Count > 4)
            {
                //获取包的长度,前面4个字节.
                byte[] packHead = _dataList.GetRange(0, 4).ToArray();
                int packLen = BitConverter.ToInt32(packHead, 0);
                if (_dataList.Count < packLen + 4)  //残包
                {
                    //MessageComeEvent.Reset();
                    MessageFlag = false;
                    return null;
                }
                else
                {
                    byte[] packMsg = _dataList.GetRange(4, packLen).ToArray();
                    //从数据池中移除这组数据
                    lock (_dataList)
                    {
                        _dataList.RemoveRange(0, packLen + 4);
                    }
                    return Encoding.Default.GetString(packMsg, 0, packMsg.Length);
                }
            }
            else
            {
                //MessageComeEvent.Reset();
                MessageFlag = false;
                return null;
            }
        }
    }

    // This class creates a single large buffer which can be divided up 
    // and assigned to SocketAsyncEventArgs objects for use with each 
    // socket I/O operation.  
    // This enables bufffers to be easily reused and guards against 
    // fragmenting heap memory.
    // 
    // The operations exposed on the BufferManager class are not thread safe.
    class BufferManager
    {
        int m_numBytes;                 // the total number of bytes controlled by the buffer pool
        byte[] m_buffer;                // the underlying byte array maintained by the Buffer Manager
        Stack<int> m_freeIndexPool;     // 
        int m_currentIndex;
        int m_bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();     //注：这个参数是用来记录空闲Buffer的开始位置的
        }

        // Allocates buffer space used by the buffer pool
        public void InitBuffer()
        {
            // create one big large buffer and divide that 
            // out to each SocketAsyncEventArg object
            m_buffer = new byte[m_numBytes];
        }

        // Assigns a buffer from the buffer pool to the 
        // specified SocketAsyncEventArgs object
        //
        // <returns>true if the buffer was successfully set, else false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {

            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize); //注：从m_buffer大buffer里取一块作为args的Buffer
                m_currentIndex += m_bufferSize;
            }
            return true;
        }

        // Removes the buffer from a SocketAsyncEventArg object.  
        // This frees the buffer back to the buffer pool
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }
    // Represents a collection of reusable SocketAsyncEventArgs objects.  
    class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> m_pool;

        // Initializes the object pool to the specified size
        //
        // The "capacity" parameter is the maximum number of 
        // SocketAsyncEventArgs objects the pool can hold
        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        // Add a SocketAsyncEventArg instance to the pool
        //
        //The "item" parameter is the SocketAsyncEventArgs instance 
        // to add to the pool
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }

        // Removes a SocketAsyncEventArgs instance from the pool
        // and returns the object removed from the pool
        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        // The number of SocketAsyncEventArgs instances in the pool
        public int Count
        {
            get { return m_pool.Count; }
        }

    }
    public enum ConnectState
    {
        Disconnect,
        Connecting,
        Connected
    }
}
