using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 存储所有的命令常量
    /// </summary>
    public static class NotifyConsts
    {
        /// <summary>
        /// 存储通用的命令
        /// </summary>
        public static class CommonNotification
        {
            //Command命令
            public const string StartUp = nameof(StartUp);
            //Mediator感兴趣的事件
            public const string UpdateConnState = nameof(UpdateConnState);
            //特殊常量
            public const string Succeed = "succeed";
            public const string Failure = "failure";
        }
        /// <summary>
        /// 存储登录注册模块用到的命令
        /// </summary>
        public static class LoginNotification
        {
            //Command命令
            public const string RequestLogin = nameof(RequestLogin);
            public const string RequestRegister = nameof(RequestRegister);
            public const string RequestLogout = nameof(RequestLogout);
            //Mediator感兴趣的事件
            public const string LoginResult = nameof(LoginResult);
            public const string RegisterResult = nameof(RegisterResult);
            public const string LogoutResult = nameof(LogoutResult);
        }
        /// <summary>
        /// 存储主菜单用到的命令
        /// </summary>
        public static class MainMenuNotification
        {
            //Command命令
            public const string RequestChangePassword = nameof(RequestChangePassword);
            //Mediator感兴趣的事件
            public const string ChangePasswordResult = nameof(ChangePasswordResult);
            public const string UpdateUserInfo = nameof(UpdateUserInfo);
        }
        /// <summary>
        /// 存储商店模块用到的命令
        /// </summary>
        public static class StoreNotification
        {
            //Command命令
            public const string RequestStoreItemList = nameof(RequestStoreItemList);
            public const string RequestPurchaseItem = nameof(RequestPurchaseItem);
            //Mediator感兴趣的事件
            public const string StoreItemListResult = nameof(StoreItemListResult);
            public const string PurchaseItemResult = nameof(PurchaseItemResult);

        }
        /// <summary>
        /// 存储仓库模块用到的命令
        /// </summary>
        public static class GarageNotification
        {
            //Command命令
            public const string RequestChangeVehicle = nameof(RequestChangeVehicle);
            //Mediator感兴趣的事件
            public const string ChangeVehicleResult = nameof(ChangeVehicleResult);
        }
        /// <summary>
        /// 存储大厅模块用到的命令
        /// </summary>
        public static class LobbyNotification
        {
            //Command命令
            public const string RequestCreateRoom = nameof(RequestCreateRoom);
            public const string RequestSearchRoom = nameof(RequestSearchRoom);
            public const string RequestRefreshRoomList = nameof(RequestRefreshRoomList);
            public const string RequestJoinRoom = nameof(RequestJoinRoom);
            //Mediator感兴趣的事件
            public const string CreateRoomResult = nameof(CreateRoomResult);
            public const string SearchRoomResult = nameof(SearchRoomResult);
            public const string RefreshRoomListResult = nameof(RefreshRoomListResult);
            public const string JoinRoomResult = nameof(JoinRoomResult);

        }
        /// <summary>
        /// 存储房间模块用到的命令
        /// </summary>
        public static class RoomNotification
        {
            //Command命令
            public const string RequestExitRoom = nameof(RequestExitRoom);
            public const string RequestChangePrepareState = nameof(RequestChangePrepareState);
            public const string RequestStartGame = nameof(RequestStartGame);
            public const string RequestKickPlayer = nameof(RequestKickPlayer);
            public const string RequestSendMessage = nameof(RequestSendMessage);
            //Mediator感兴趣的事件
            public const string InitRoomInfo = nameof(InitRoomInfo);
            public const string NewPlayerJoinRoom = nameof(NewPlayerJoinRoom);

            public const string ExitRoomResult = nameof(ExitRoomResult);
            public const string ChangePrepareStateResult = nameof(ChangePrepareStateResult);
            public const string StartGameResult = nameof(StartGameResult);
            public const string KickPlayerResult = nameof(KickPlayerResult);
            public const string SendMessageResult = nameof(SendMessageResult);
        }
        /// <summary>
        /// 存储战斗模块用到的命令
        /// </summary>
        public static class BattleNotification
        {
            //Command命令
            //Mediator感兴趣的事件
        }
        /// <summary>
        /// 储存场景名称
        /// </summary>
        public static class SceneName
        {
            public const string LoginScene = nameof(LoginScene);
            public const string LoadingScene = nameof(LoadingScene);
            public const string MainMenuScene = nameof(MainMenuScene);
            public const string RoomScene = nameof(RoomScene);
        }
    }
}
