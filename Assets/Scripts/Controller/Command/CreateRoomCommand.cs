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
    class CreateRoomCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            LobbyProxy lobbyProxy = Facade.RetrieveProxy(nameof(LobbyProxy)) as LobbyProxy;
            var tuple = notification.Body as Tuple<string, string, string>;
            lobbyProxy.RequestCreateRoom(tuple.Item1, tuple.Item2, tuple.Item3);
        }
    }
}
