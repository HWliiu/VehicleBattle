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
    class SendMessageCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            RoomProxy roomProxy = Facade.RetrieveProxy(nameof(RoomProxy)) as RoomProxy;
            var message = notification.Body as string;
            roomProxy.RequestSendMessage(message);
        }
    }
}
