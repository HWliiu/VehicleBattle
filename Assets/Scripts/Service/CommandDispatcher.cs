using GameClient.Model;
using GameClient.Service;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameClient.Service
{
    class CommandDispatcher
    {
        private RecvBufQueue _recvBufQueue;
        private AppFacade _appFacade;

        public CommandDispatcher()
        {
            _recvBufQueue = NetworkService.Instance.RecvBufQueue;
            _appFacade = AppFacade.Instance;
        }

        public void StartDispatch(object state)
        {
            if (state is SynchronizationContext context)
            {
                while (true)
                {
                    _recvBufQueue.MessageComeEvent.Wait();
                    // 分发命令
                    var msg = _recvBufQueue.Dequeue();
                    if (msg != null)
                    {
                        JObject o = JObject.Parse(msg);
                        string command = (string)o.SelectToken("Command");
                        o.Property("Command").Remove();
                        // TODO: 有空再来改这一块(可以改成使用反射来找到函数)
                        switch (command)
                        {
                            //封送回主线程执行
                            case NotifyConsts.LoginNotification.LoginResult:
                                context.Post((obj) => (_appFacade.RetrieveProxy(nameof(LoginProxy)) as LoginProxy).LoginResult(obj as JObject), o);
                                break;
                            case NotifyConsts.LoginNotification.LogoutResult:
                                context.Post((obj) => (_appFacade.RetrieveProxy(nameof(LoginProxy)) as LoginProxy).LogoutResult(obj as JObject), o);
                                break;
                            case NotifyConsts.LoginNotification.RegisterResult:
                                context.Post((obj) => (_appFacade.RetrieveProxy(nameof(LoginProxy)) as LoginProxy).RegisterResult(obj as JObject), o);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
