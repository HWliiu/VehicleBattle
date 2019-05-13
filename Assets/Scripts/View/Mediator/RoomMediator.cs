using GameClient.Controller;
using GameClient.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

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
                        HandleInitRoom(tuple.Item1, tuple.Item2);
                    }
                    break;
                case NotifyConsts.RoomNotification.NetPlayerJoinRoom:
                    if (notification.Body is NetPlayerVO netPlayer)
                    {
                        HandleNetPlayerJoin(netPlayer);
                    }
                    break;
                case NotifyConsts.RoomNotification.ExitRoomResult:
                    if (notification.Body is Tuple<bool, string, string, string> exitRoomTuple)
                    {
                        HandleExitRoom(exitRoomTuple.Item1, exitRoomTuple.Item2, exitRoomTuple.Item3, exitRoomTuple.Item4);
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
                NotifyConsts.RoomNotification.NetPlayerJoinRoom,
                NotifyConsts.RoomNotification.ExitRoomResult,
                NotifyConsts.RoomNotification.KickPlayerResult,
                NotifyConsts.RoomNotification.ChangePrepareStateResult,
                NotifyConsts.RoomNotification.SendMessageResult,
                NotifyConsts.RoomNotification.StartGameResult
            };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _viewComponent = (ViewComponent as RoomView) ?? throw new InvalidCastException(nameof(ViewComponent));

            AppFacade.Instance.RegisterProxy(new RoomProxy(nameof(RoomProxy), null));
            AppFacade.Instance.RegisterCommand(NotifyConsts.RoomNotification.RequestExitRoom, () => new ExitRoomCommand());

            //RoomPanel
            _viewComponent.RP_ExitRoomBtn.onClick.AddListener(OnExitRoomBtn);
            _viewComponent.RP_PrepareBtn.onClick.AddListener(OnPrepareBtn);
            _viewComponent.RP_StartGameBtn.onClick.AddListener(OnStartGameBtn);
            //PlayerListPanel

            //ChatPanel
        }

        public override void OnRemove()
        {
            base.OnRemove();
            AppFacade.Instance.RemoveProxy(nameof(RoomProxy));
            AppFacade.Instance.RemoveCommand(NotifyConsts.RoomNotification.RequestExitRoom);
        }

        private void OnExitRoomBtn()
        {
            _viewComponent.DL_DialogTitleText.text = "是否确认退出房间?";
            _viewComponent.DL_DialogTipsText.text = "";
            _viewComponent.DL_DialogConfirmBtn.onClick.AddListener(OnConfirmExitRoomBtn);
            _viewComponent.DialogPanel.gameObject.SetActive(true);
        }
        private void OnConfirmExitRoomBtn() => SendNotification(NotifyConsts.RoomNotification.RequestExitRoom, null, null);
        private void OnPrepareBtn() { }
        private void OnStartGameBtn() { }

        private void HandleInitRoom(RoomVO room, List<PlayerVO> playerList) => _viewComponent.InitRoomInfo(room, playerList);
        private void HandleNetPlayerJoin(PlayerVO netPlayer)
        {
            _viewComponent.PL_RoomTipsText.text = $"{netPlayer.UserName} 加入房间";
            _viewComponent.AddPlayerItem(netPlayer);
        }
        private void HandleExitRoom(bool result, string info, string exitPlayerId, string newOwnerId)
        {
            if (result)
            {
                if (exitPlayerId == PlayerManager.Instance.LocalPlayer.UserID)
                {
                    _viewComponent.DL_DialogTipsText.text = info;
                    async Task subsequentHandle()
                    {
                        await Task.Delay(1000);
                        _viewComponent.DL_DialogTipsText.text = "正在返回大厅";
                        await Task.Delay(1000);
                        _viewComponent.OnDialogCancelBtn();
                        _viewComponent.OpenLobbyPanel();
                    }
                    _ = subsequentHandle();
                }
                else
                {
                    if (newOwnerId != null)
                    {
                        _viewComponent.RemovePlayerItem(exitPlayerId);
                        var owner = newOwnerId == PlayerManager.Instance.LocalPlayer.UserID ? (PlayerVO)PlayerManager.Instance.LocalPlayer : PlayerManager.Instance.GetNetPlayer(newOwnerId);
                        _viewComponent.PL_RoomTipsText.text = $"{PlayerManager.Instance.GetNetPlayer(exitPlayerId).UserName} 退出房间,{owner.UserName} 成为房主";
                        //创建系统messageBar
                        _viewComponent.FindPlayerItem(newOwnerId).transform.Find("RoomOwnerText").GetComponent<Text>().gameObject.SetActive(true);
                        if (newOwnerId == PlayerManager.Instance.LocalPlayer.UserID)
                        {
                            _viewComponent.UpdateKickBtnDisplay();
                        }
                    }
                    else
                    {
                        _viewComponent.RemovePlayerItem(exitPlayerId);
                        _viewComponent.PL_RoomTipsText.text = $"{PlayerManager.Instance.GetNetPlayer(exitPlayerId).UserName} 退出房间";
                    }
                }
            }
            else
            {
                _viewComponent.DL_DialogTipsText.text = info;
            }
        }
    }
}
