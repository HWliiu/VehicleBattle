using GameClient.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.Controller
{
    public class LoginCommand : SimpleCommand
    {
        //Command业务逻辑协调Model与View的逻辑
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            LoginProxy loginProxy = Facade.RetrieveProxy(nameof(LoginProxy)) as LoginProxy;
            var tuple = notification.Body as Tuple<string, string>;
            loginProxy.RequestLogin(tuple.Item1, tuple.Item2);
        }
    }
}
