using GameClient.Common;
using GameClient.Controller;
using GameClient.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.View
{
    class RoomMediator : Mediator
    {
        private RoomView _viewComponent;
        private string _kickPlayerId;
        public RoomMediator(string mediatorName, object viewComponent = null) : base(mediatorName, viewComponent)
        {
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case NotifyConsts.RoomNotification.InitRoomInfo:
                    if (notification.Body is RoomVO room)
                    {
                        HandleInitRoom(room);
                    }
                    break;
                case NotifyConsts.RoomNotification.NewPlayerJoinRoom:
                    if (notification.Body is Tuple<PlayerVO, List<PlayerVO>> newPlayerJoinTuple)
                    {
                        HandleNewPlayerJoin(newPlayerJoinTuple.Item1, newPlayerJoinTuple.Item2);
                    }
                    break;
                case NotifyConsts.RoomNotification.ExitRoomResult:
                    if (notification.Body is Tuple<bool, string, string, List<PlayerVO>> exitRoomTuple)
                    {
                        HandleExitRoom(exitRoomTuple.Item1, exitRoomTuple.Item2, exitRoomTuple.Item3, exitRoomTuple.Item4);
                    }
                    break;
                case NotifyConsts.RoomNotification.KickPlayerResult:
                    if (notification.Body is Tuple<bool, string, string, List<PlayerVO>> kickPlayerTuple)
                    {
                        HandleKickPlayer(kickPlayerTuple.Item1, kickPlayerTuple.Item2, kickPlayerTuple.Item3, kickPlayerTuple.Item4);
                    }
                    break;
                case NotifyConsts.RoomNotification.ChangePrepareStateResult:
                    if (notification.Body is Tuple<bool, string, string, bool> changePrepareStateTuple)
                    {
                        HandleChangePrepareState(changePrepareStateTuple.Item1, changePrepareStateTuple.Item2, changePrepareStateTuple.Item3, changePrepareStateTuple.Item4);
                    }
                    break;
                case NotifyConsts.RoomNotification.SendMessageResult:
                    if (notification.Body is Tuple<string, string> messageTuple)
                    {
                        HandleSendMessage(messageTuple.Item1, messageTuple.Item2);
                    }
                    break;
                case NotifyConsts.RoomNotification.StartGameResult:
                    if (notification.Body is Tuple<bool, string> startGameTuple)
                    {
                        HandleStartGame(startGameTuple.Item1, startGameTuple.Item2);
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
                NotifyConsts.RoomNotification.NewPlayerJoinRoom,
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
            AppFacade.Instance.RegisterCommand(NotifyConsts.RoomNotification.RequestChangePrepareState, () => new ChangePrepareStateCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.RoomNotification.RequestKickPlayer, () => new KickPlayerCommamd());
            AppFacade.Instance.RegisterCommand(NotifyConsts.RoomNotification.RequestSendMessage, () => new SendMessageCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.RoomNotification.RequestStartGame, () => new StartGameCommand());

            //RoomPanel
            _viewComponent.RP_ExitRoomBtn.onClick.AddListener(OnExitRoomBtn);
            _viewComponent.RP_PrepareBtn.onClick.AddListener(OnPrepareBtn);
            _viewComponent.RP_StartGameBtn.onClick.AddListener(OnStartGameBtn);
            //PlayerListPanel
            _viewComponent.OnKick += OnKickPlayerBtn;
            //ChatPanel
            _viewComponent.CP_SendMessageBtn.onClick.AddListener(OnSendMessageBtn);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            AppFacade.Instance.RemoveProxy(nameof(RoomProxy));
            AppFacade.Instance.RemoveCommand(NotifyConsts.RoomNotification.RequestExitRoom);
            AppFacade.Instance.RemoveCommand(NotifyConsts.RoomNotification.RequestChangePrepareState);
            AppFacade.Instance.RemoveCommand(NotifyConsts.RoomNotification.RequestKickPlayer);
            AppFacade.Instance.RemoveCommand(NotifyConsts.RoomNotification.RequestSendMessage);
            AppFacade.Instance.RemoveCommand(NotifyConsts.RoomNotification.RequestStartGame);
        }

        private void OnExitRoomBtn()
        {
            _viewComponent.DL_DialogTitleText.text = "是否确认退出房间?";
            _viewComponent.DL_DialogTipsText.text = "";
            _viewComponent.DL_DialogConfirmBtn.onClick.AddListener(OnConfirmExitRoomBtn);
            _viewComponent.DialogPanel.gameObject.SetActive(true);
        }
        private void OnConfirmExitRoomBtn() => SendNotification(NotifyConsts.RoomNotification.RequestExitRoom, null, null);
        private void OnPrepareBtn() => SendNotification(NotifyConsts.RoomNotification.RequestChangePrepareState, !PlayerManager.Instance.LocalPlayer.PrepareState, nameof(Boolean));
        private void OnKickPlayerBtn(string playerId)
        {
            _kickPlayerId = playerId;
            _viewComponent.DL_DialogTitleText.text = "是否确认踢除该玩家?";
            _viewComponent.DL_DialogTipsText.text = "";
            _viewComponent.DL_DialogConfirmBtn.onClick.AddListener(OnConfirmKickPlayerBtn);
            _viewComponent.DialogPanel.gameObject.SetActive(true);
        }
        private void OnConfirmKickPlayerBtn() => SendNotification(NotifyConsts.RoomNotification.RequestKickPlayer, _kickPlayerId, nameof(String));
        private void OnStartGameBtn() => SendNotification(NotifyConsts.RoomNotification.RequestStartGame, null, null);
        private void OnSendMessageBtn()
        {
            var message = _viewComponent.CP_MessageInput.text;
            if (message.Length < 0) return;
            _viewComponent.CP_MessageInput.text = null;
            SendNotification(NotifyConsts.RoomNotification.RequestSendMessage, message, nameof(String));
        }

        private void HandleInitRoom(RoomVO room)
        {
            _viewComponent.InitRoomInfo(room);
            _viewComponent.AddMessageItem("系统", $"欢迎{PlayerManager.Instance.LocalPlayer.UserName}加入房间!");
        }

        private void HandleNewPlayerJoin(PlayerVO joinPlayer, List<PlayerVO> playerList)
        {
            _viewComponent.UpdatePlayerList(playerList);
            _viewComponent.PL_RoomTipsText.text = $"{joinPlayer.UserName} 加入房间";
            _viewComponent.AddMessageItem("系统", $"欢迎{joinPlayer.UserName}加入房间!");
        }
        private void HandleExitRoom(bool result, string info, string exitPlayerId, List<PlayerVO> playList)
        {
            if (result)
            {
                if (exitPlayerId == PlayerManager.Instance.LocalPlayer.UserID)
                {
                    _viewComponent.DL_DialogTipsText.text = "退出成功";
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
                    _viewComponent.UpdatePlayerList(playList);
                    _viewComponent.PL_RoomTipsText.text = info;
                    _viewComponent.AddMessageItem("系统", info);
                }
            }
            else
            {
                _viewComponent.DL_DialogTipsText.text = info;
            }
        }
        private void HandleKickPlayer(bool result, string info, string kickPlayerId, List<PlayerVO> playList)
        {
            if (result)
            {
                if (kickPlayerId == PlayerManager.Instance.LocalPlayer.UserID)
                {
                    _viewComponent.DL_DialogTitleText.text = "";
                    _viewComponent.DL_DialogTipsText.text = "您已被房主踢出房间";
                    _viewComponent.DL_DialogConfirmBtn.onClick.AddListener(backToLobby);
                    _viewComponent.DL_DialogCancelBtn.onClick.AddListener(backToLobby);
                    _viewComponent.DialogPanel.gameObject.SetActive(true);

                    void backToLobby()
                    {
                        _viewComponent.DL_DialogCancelBtn.onClick.AddListener(_viewComponent.OnDialogCancelBtn);
                        _ = subsequentHandle();
                    }
                    async Task subsequentHandle()
                    {
                        _viewComponent.DL_DialogTipsText.text = "正在返回大厅";
                        await Task.Delay(1000);
                        _viewComponent.OnDialogCancelBtn();
                        _viewComponent.OpenLobbyPanel();
                    }
                }
                else
                {
                    if (PlayerManager.Instance.RoomOwner.UserID == PlayerManager.Instance.LocalPlayer.UserID)
                    {
                        async Task subsequentHandle()
                        {
                            _viewComponent.DL_DialogTipsText.text = "踢除成功";
                            await Task.Delay(1000);
                            _viewComponent.OnDialogCancelBtn();
                            _viewComponent.UpdatePlayerList(playList);
                            _viewComponent.PL_RoomTipsText.text = info;
                            _viewComponent.AddMessageItem("系统", info);
                        }
                        _ = subsequentHandle();
                    }
                    else
                    {
                        _viewComponent.UpdatePlayerList(playList);
                        _viewComponent.PL_RoomTipsText.text = info;
                        _viewComponent.AddMessageItem("系统", info);
                    }
                }
            }
            else
            {
                _viewComponent.PL_RoomTipsText.text = info;
            }
        }
        private void HandleChangePrepareState(bool result, string info, string playerId, bool prepareState)
        {
            if (result)
            {
                if (playerId == PlayerManager.Instance.LocalPlayer.UserID)
                {
                    _viewComponent.RP_PrepareBtn.GetComponentInChildren<Text>().text = prepareState ? "取消准备" : "准备";
                }
                _viewComponent.FindPlayerItem(playerId).GetComponent<PlayerItem>().PrepareState = prepareState;
            }
            else
            {
                _viewComponent.PL_RoomTipsText.text = info;
            }
        }
        private void HandleSendMessage(string playerName, string message) => _viewComponent.AddMessageItem(playerName, message);
        private void HandleStartGame(bool result, string info)
        {
            if (result)
            {
                _viewComponent.PL_RoomTipsText.text = info;
                async Task subsequentHandle()
                {
                    await Task.Delay(1000);
                    UnityUtil.LoadScene("BattleScene1");
                }
                _ = subsequentHandle();
            }
            else
            {
                _viewComponent.PL_RoomTipsText.text = info;
            }
        }
    }
}
