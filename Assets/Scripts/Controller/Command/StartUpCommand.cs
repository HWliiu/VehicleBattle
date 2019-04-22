using GameClient.Model;
using GameClient.Service;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameClient.Controller
{
    class StartUpCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            //全局的Command,Proxy统一到这里注册
            Facade.RegisterProxy(new LoginProxy(nameof(LoginProxy), null));

            Facade.RegisterCommand(NotifyConsts.LoginNotification.RequestLogout, () => new LogoutCommand());
        }
    }
}
