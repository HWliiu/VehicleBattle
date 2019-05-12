using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameClient.Common
{
    sealed class RequestInterceptor
    {
        public event Action<string> OnRequestStateChange;
        public RequestState RequestState
        {
            get => _requestState;
            private set
            {
                if (_requestState != value && OnRequestStateChange != null)
                {
                    string info = null;
                    switch (value)
                    {
                        case RequestState.None:
                            info = "";
                            break;
                        case RequestState.Requesting:
                            info = "";
                            break;
                        case RequestState.RepeatRequest:
                            info = "正在处理 请勿重复请求";
                            break;
                        case RequestState.Frequency:
                            info = "操作过快 请稍后再试";
                            break;
                        case RequestState.TimeOut:
                            info = "请求超时";
                            break;
                        default:
                            break;
                    }
                    OnRequestStateChange(info);
                }
                _requestState = value;
            }
        }
        private readonly float _requestTimeOut;
        private readonly float _requestInterval;
        private CancellationTokenSource _cts;
        private RequestState _requestState;
        private bool _frequentRequest;

        public RequestInterceptor(float requestInterval = 0.0f, float requestTimeOut = 3.0f)
        {
            requestInterval = requestInterval < 0f ? 0f : requestInterval;
            requestTimeOut = requestTimeOut < 0f ? 0f : requestTimeOut;

            RequestState = RequestState.None;
            _requestTimeOut = requestTimeOut;
            _requestInterval = requestInterval;
            _cts = new CancellationTokenSource();
            //_cts.Token.Register(() => RequestState = RequestState.None);  //这个回调不在主线程
        }
        public bool AllowRequest()
        {
            if (RequestState == RequestState.Requesting)
            {
                RequestState = RequestState.RepeatRequest;
                return false;
            }
            else if (RequestState == RequestState.RepeatRequest)
            {
                return false;
            }
            else if (_frequentRequest)
            {
                RequestState = RequestState.Frequency;
                return false;
            }
            return true;
        }
        public async Task BeginWaitResponseAsync()
        {
            BlockFrequentRequest();
            RequestState = RequestState.Requesting;
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_requestTimeOut), _cts.Token);
            }
            catch (TaskCanceledException)
            {
                RequestState = RequestState.None;
                return;
            }

            RequestState = RequestState.TimeOut;
        }
        public void EndWaitResponse() => _cts.Cancel();
        private void BlockFrequentRequest()
        {
            _frequentRequest = true;
            Task.Delay(TimeSpan.FromSeconds(_requestInterval)).ContinueWith((t) => { _frequentRequest = false; if (RequestState == RequestState.Frequency) RequestState = RequestState.None; }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
    enum RequestState
    {
        None,
        Requesting,
        RepeatRequest,
        Frequency,
        TimeOut
    }
}
