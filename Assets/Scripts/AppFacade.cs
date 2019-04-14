using GameClient.Controller;
using PureMVC.Patterns.Facade;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class AppFacade : Facade
    {
        private static readonly Lazy<AppFacade> _instance = new Lazy<AppFacade>(() => new AppFacade());
        public static AppFacade Instance => _instance.Value;

        private AppFacade()
        {
        }

        public void StartUp() => SendNotification(NotifyConsts.CommonNotification.StartUp);

        protected override void InitializeController()
        {
            base.InitializeController();
            RegisterCommand(NotifyConsts.CommonNotification.StartUp, () => new StartUpCommand());
        }
    }
}
