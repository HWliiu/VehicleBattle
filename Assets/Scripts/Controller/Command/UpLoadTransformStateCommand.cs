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
    class UpLoadTransformStateCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            BattleProxy battleProxy = Facade.RetrieveProxy(nameof(BattleProxy)) as BattleProxy;
            var vehicle = notification.Body as VehicleVO;
            battleProxy.UpLoadTransformState(vehicle.VehiclePosition, vehicle.VehicleRotation, vehicle.TurretRotation, vehicle.MoveInputValue, vehicle.TurnInputValue);
        }
    }
}
