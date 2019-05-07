using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.View
{
    class RoomMediator : Mediator
    {
        private RoomView _viewComponent;

        public RoomMediator(string mediatorName, object viewComponent = null) : base(mediatorName, viewComponent)
        {
        }

        public override void HandleNotification(INotification notification)
        {
            base.HandleNotification(notification);
        }

        public override string[] ListNotificationInterests()
        {
            return base.ListNotificationInterests();
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _viewComponent = (ViewComponent as RoomView) ?? throw new InvalidCastException(nameof(ViewComponent));
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }
    }
}
