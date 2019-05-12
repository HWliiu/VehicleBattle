using GameClient.Common;
using GameClient.Controller;
using GameClient.Model;
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
                    if (notification.Body is Tuple<bool, string> createRoomTuple)
                    {
                        HandleCreateRoomResult(createRoomTuple.Item1, createRoomTuple.Item2);
                    }
                    break;
                case NotifyConsts.LobbyNotification.SearchRoomResult:
                    if (notification.Body is Tuple<bool, string> searchRoomTuple)
                    {
                        HandleSearchResult(searchRoomTuple.Item1, searchRoomTuple.Item2);
                    }
                    break;
                case NotifyConsts.LobbyNotification.JoinRoomResult:
                    if (notification.Body is Tuple<bool, string> joinRoomTuple)
                    {
                        HandleJoinRoomResult(joinRoomTuple.Item1, joinRoomTuple.Item2);
                    }
                    break;
                case NotifyConsts.LobbyNotification.RefreshRoomListResult:
                    if (notification.Body is Tuple<bool, string, List<RoomVO>> refreshRoomListTuple)
                    {
                        HandleRefreshRoomListResult(refreshRoomListTuple.Item1, refreshRoomListTuple.Item2, refreshRoomListTuple.Item3);
                    }
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
                NotifyConsts.LobbyNotification.JoinRoomResult,
                NotifyConsts.LobbyNotification.RefreshRoomListResult,
                NotifyConsts.CommonNotification.UpdateConnState
            };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            AppFacade.Instance.RegisterCommand(NotifyConsts.LobbyNotification.RequestCreateRoom, () => new CreateRoomCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.LobbyNotification.RequestSearchRoom, () => new SearchRoomCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.LobbyNotification.RequestRefreshRoomList, () => new RefreshRoomListCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.LobbyNotification.RequestJoinRoom, () => new JoinRoomCommand());
            AppFacade.Instance.RegisterProxy(new LobbyProxy(nameof(LobbyProxy), null));

            _viewComponent = (ViewComponent as LobbyView) ?? throw new InvalidCastException(nameof(ViewComponent));
            //RoomListPanel
            _viewComponent.RL_RefreshRoomListBtn.onClick.AddListener(OnRefreshRoomListBtn);
            _viewComponent.OnJoinRoom += OnJoinRoomBtn;
            //CreateRoomPanel
            _viewComponent.CR_ConfirmCreateBtn.onClick.AddListener(OnConfirmCreateBtn);
            //SearchRoomPanel
            _viewComponent.SR_ConfirmSearchBtn.onClick.AddListener(OnConfirmSearchBtn);

            SendNotification(NotifyConsts.CommonNotification.UpdateConnState, NetworkService.Instance.ConnectState, nameof(ConnectState));
            OnRefreshRoomListBtn();
        }

        public override void OnRemove()
        {
            AppFacade.Instance.RemoveCommand(nameof(NotifyConsts.LobbyNotification.RequestCreateRoom));
            AppFacade.Instance.RemoveCommand(nameof(NotifyConsts.LobbyNotification.RequestSearchRoom));
            AppFacade.Instance.RemoveCommand(nameof(NotifyConsts.LobbyNotification.RequestRefreshRoomList));
            AppFacade.Instance.RemoveCommand(nameof(NotifyConsts.LobbyNotification.RequestJoinRoom));
            AppFacade.Instance.RemoveProxy(nameof(LobbyProxy));
            base.OnRemove();
        }
        #region RoomListPanel
        private void OnRefreshRoomListBtn() => SendNotification(NotifyConsts.LobbyNotification.RequestRefreshRoomList, null, null);
        private void OnJoinRoomBtn(string roomId) => SendNotification(NotifyConsts.LobbyNotification.RequestJoinRoom, roomId, nameof(String));
        private void HandleRefreshRoomListResult(bool result, string info, List<RoomVO> roomList)
        {
            _viewComponent.RL_LobbyTipsText.text = info;
            if (result)
            {
                _viewComponent.UpdateRoomItems(roomList);
            }
            Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(t => _viewComponent.RL_LobbyTipsText.text = "", TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void HandleJoinRoomResult(bool result, string info)
        {
            _viewComponent.RL_LobbyTipsText.text = info;
            if (result)
            {
                async Task subsequentHandle()
                {
                    await Task.Delay(1000);
                    _viewComponent.RL_LobbyTipsText.text = "正在进入房间";
                    await Task.Delay(1000);
                    _viewComponent.OpenRoomPanel();
                }
                _ = subsequentHandle();
            }
        }
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
            if (_viewComponent.SelectRoomMode != RoomMode.SingleMode)
            {
                _viewComponent.CR_CreateRoomTipsText.text = "组队模式未开放";
                return;
            }

            SendNotification(NotifyConsts.LobbyNotification.RequestCreateRoom, Tuple.Create(roomName, roomMode, roomMap), nameof(Tuple<String, String, String>));
        }
        private void HandleCreateRoomResult(bool result, string info)
        {
            _viewComponent.CR_CreateRoomTipsText.text = info;
            if (result)
            {
                async Task subsequentHandle()
                {
                    await Task.Delay(1000);
                    _viewComponent.CR_CreateRoomTipsText.text = "正在进入房间";
                    await Task.Delay(1000);
                    _viewComponent.OpenRoomPanel();
                }
                _ = subsequentHandle();
            }
        }
        #endregion
        #region SearchRoomPanel
        private void OnConfirmSearchBtn()
        {
            string roomId = _viewComponent.SR_SearchRoomInput.text;
            if (roomId.Length != 4)
            {
                _viewComponent.SR_SearchRoomTipsText.text = "房间号格式错误";
                return;
            }
            _viewComponent.SR_SearchRoomTipsText.text = "";
            SendNotification(NotifyConsts.LobbyNotification.RequestSearchRoom, roomId, nameof(String));
        }
        private void HandleSearchResult(bool result, string info)
        {
            _viewComponent.SR_SearchRoomTipsText.text = info;
            if (result)
            {
                async Task subsequentHandle()
                {
                    await Task.Delay(500);
                    _viewComponent.OnCloseSearchRoomPanel();
                }
                _ = subsequentHandle();
            }
        }
        #endregion
    }
}
