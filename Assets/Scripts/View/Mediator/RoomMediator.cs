using GameClient.Model;
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
            switch (notification.Name)
            {
                case NotifyConsts.RoomNotification.InitRoomInfo:
                    if (notification.Body is Tuple<RoomVO, List<PlayerVO>> tuple)
                    {
                        _viewComponent.InitRoomInfo(tuple.Item1, tuple.Item2);
                    }
                    break;
                case NotifyConsts.RoomNotification.NetPlayerJoinRoom:
                    if (notification.Body is NetPlayerVO netPlayer)
                    {
                        _viewComponent.PL_RoomTipsText.text = netPlayer.UserName + "加入房间";
                        _viewComponent.AddPlayerItem(netPlayer);
                    }
                    break;
                default:
                    break;
            }
        }

        public override string[] ListNotificationInterests()
        {
            return new string[] {
                NotifyConsts.RoomNotification.InitRoomInfo,
                NotifyConsts.RoomNotification.NetPlayerJoinRoom
            };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _viewComponent = (ViewComponent as RoomView) ?? throw new InvalidCastException(nameof(ViewComponent));

            AppFacade.Instance.RegisterProxy(new RoomProxy(nameof(RoomProxy), null));
        }

        public override void OnRemove()
        {
            base.OnRemove();
            AppFacade.Instance.RemoveProxy(nameof(RoomProxy));
        }
    }
}
