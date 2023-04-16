using GameClient.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameClient
{
    public class Main : MonoBehaviour
    {
        private Coroutine _dispatchCoroutine;
        private void Awake()
        {
            //启动整个PureMVC框架
            AppFacade.Instance.StartUp();
            //开启网络服务
            StartCoroutine(StartNetworkService());
        }

        private IEnumerator StartNetworkService()
        {
            yield return null;  //这里要等所有MonoBehaviour的Start执行完
            var context = SynchronizationContext.Current ?? throw new ArgumentNullException(nameof(SynchronizationContext));
            //启动网络服务
            NetworkService.Instance.OnConnectStateChange += (connectState) => context.Post((state) => AppFacade.Instance.SendNotification(NotifyConsts.CommonNotification.UpdateConnState, connectState, nameof(ConnectState)), null);     //确保SendNotification的调用在主线程
            NetworkService.Instance.Start();
            yield return null;
            _dispatchCoroutine = StartCoroutine(CommandDispatcher.Instance.StartDispatch());
        }

        private void OnDestroy()
        {
            StopCoroutine(_dispatchCoroutine);
            //关闭网络服务
            NetworkService.Instance.Close();
        }
    }

}
