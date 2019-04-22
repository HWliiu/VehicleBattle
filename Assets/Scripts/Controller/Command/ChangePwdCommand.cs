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
    class ChangePwdCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            MainMenuProxy mainMenuProxy = Facade.RetrieveProxy(nameof(MainMenuProxy)) as MainMenuProxy;
            var tuple = notification.Body as Tuple<string, string>;
            mainMenuProxy.RequestChangePassword(tuple.Item1, tuple.Item2);
        }
    }
}
