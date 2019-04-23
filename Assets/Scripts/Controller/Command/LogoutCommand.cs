using GameClient.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Controller
{
    class LogoutCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            LoginProxy loginProxy = Facade.RetrieveProxy(nameof(LoginProxy)) as LoginProxy;
            loginProxy.RequestLogout();
        }
    }
}
