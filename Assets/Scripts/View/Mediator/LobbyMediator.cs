using GameClient.Common;
using GameClient.Service;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.View
{
    class LobbyMediator : Mediator
    {
        private LobbyView _viewComponent;
        public LobbyMediator(string mediatorName, object viewComponent = null) : base(mediatorName, viewComponent)
        {
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case NotifyConsts.LobbyNotification.CreateRoomResult:
                    break;
                case NotifyConsts.LobbyNotification.SearchRoomResult:
                    break;
                case NotifyConsts.LobbyNotification.RefreshRoomListResult:
                    break;
                case NotifyConsts.CommonNotification.UpdateConnState:
                    if (notification.Body is ConnectState state)
                    {
                        UnityUtil.UpdateConnStateDisplay(state, _viewComponent.ConnStateText);
                    }
                    break;
                default:
                    break;
            }
        }

        public override string[] ListNotificationInterests()
        {
            return new string[] {
                NotifyConsts.LobbyNotification.CreateRoomResult,
                NotifyConsts.LobbyNotification.SearchRoomResult,
                NotifyConsts.LobbyNotification.RefreshRoomListResult,
                NotifyConsts.CommonNotification.UpdateConnState
            };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _viewComponent = (ViewComponent as LobbyView) ?? throw new InvalidCastException(nameof(ViewComponent));
            //RoomListPanel
            _viewComponent.RL_RefreshRoomListBtn.onClick.AddListener(OnRefreshRoomListBtn);
            //CreateRoomPanel
            _viewComponent.CR_ConfirmCreateBtn.onClick.AddListener(OnConfirmCreateBtn);
            //SearchRoomPanel

            SendNotification(NotifyConsts.CommonNotification.UpdateConnState, NetworkService.Instance.ConnectState, nameof(ConnectState));
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }
        #region RoomListPanel
        private void OnRefreshRoomListBtn() => SendNotification(NotifyConsts.LobbyNotification.RequestRefreshRoomList, null, null);
        #endregion
        #region CreateRoomPanel
        private void OnConfirmCreateBtn()
        {
            string roomName = _viewComponent.CR_RoomNameInput.text;
            string roomMode = _viewComponent.SelectRoomMode.ToString();
            string roomMap = _viewComponent.SelectRoomMap.ToString();
            if (roomName.Length < 3)
            {
                _viewComponent.CR_CreateRoomTipsText.text = "房间名称不能小于3个字符";
                return;
            }
            if (roomName.Length > 10)
            {
                _viewComponent.CR_CreateRoomTipsText.text = "房间名称不能大于10个字符";
                return;
            }
            _viewComponent.CR_CreateRoomTipsText.text = "";
            SendNotification(NotifyConsts.LobbyNotification.RequestCreateRoom, Tuple.Create(roomName, roomMode, roomMap), nameof(Tuple<String, String, String>));
        }
        #endregion
        #region SearchRoomPanel

        #endregion
    }
}
