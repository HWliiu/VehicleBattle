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
    class ChangeVehicleCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            GarageProxy garageProxy = Facade.RetrieveProxy(nameof(GarageProxy)) as GarageProxy;
            garageProxy.RequestChangeVehicle(notification.Body as string);
        }
    }
}
