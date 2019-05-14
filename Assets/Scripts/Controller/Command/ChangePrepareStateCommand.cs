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
    class ChangePrepareStateCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            RoomProxy roomProxy = Facade.RetrieveProxy(nameof(RoomProxy)) as RoomProxy;
            var prepareState = (bool)notification.Body;
            roomProxy.RequestChangePrepareState(prepareState);
        }
    }
}
