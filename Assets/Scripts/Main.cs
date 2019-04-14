﻿using GameClient.Service;
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
        private CancellationTokenSource _cts;
        private void Awake()
        {
            //启动整个PureMVC框架
            AppFacade.Instance.StartUp();
            DontDestroyOnLoad(gameObject);  //确保只载入一次
            //开启网络服务
            StartCoroutine(StartNetworkService());
            _cts = new CancellationTokenSource();
        }

        private IEnumerator StartNetworkService()
        {
            yield return null;  //这里要等所有MonoBehaviour的Start执行完
            var context = SynchronizationContext.Current ?? throw new Exception("SynchronizationContext is null");
            //启动网络服务
            NetworkService.Instance.OnConnectStateChange += (connectState) => context.Post((state) => AppFacade.Instance.SendNotification(NotifyConsts.CommonNotification.UpdateConnState, connectState, nameof(String)), null);     //确保SendNotification的调用在主线程,因为要修改UI组件(SendNotification的第二个参数装箱了)
            NetworkService.Instance.Start();
            yield return null;
            //单独开一个线程分发命令
            Task.Factory.StartNew(new CommandDispatcher().StartDispatch, context, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);   //LongRunning会创建线程池外的一个独立线程
        }

        private void OnDestroy()
        {
            //取消命令分发任务
            _cts.Cancel();
            // TODO: 关闭网络服务

        }
    }

}