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
    class UpLoadHealthStateCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            BattleProxy battleProxy = Facade.RetrieveProxy(nameof(BattleProxy)) as BattleProxy;
            var health = (int)notification.Body;
            battleProxy.UpLoadHealthState(health);
        }
    }
}
