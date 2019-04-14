using GameClient.Service;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameClient.Service
{
    public class CommandDispatcher
    {
        private RecvBufQueue _recvBufQueue;

        public CommandDispatcher()
        {
            _recvBufQueue = NetworkService.Instance.RecvBufQueue;
        }

        public void StartDispatch(object state)
        {
            if (state is SynchronizationContext context)
            {
                while (true)
                {
                    _recvBufQueue.MessageComeEvent.Wait();
                    // TODO: 分发命令
                    var msg = _recvBufQueue.Dequeue();
                    Debug.Log(msg??"null");
                }
            }
        }
    }
}
