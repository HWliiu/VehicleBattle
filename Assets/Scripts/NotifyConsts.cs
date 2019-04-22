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
            public const string Succeed = "succeed";    //这两个比较特殊用的小写
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
            public const string UpdateVehiclePara = nameof(UpdateVehiclePara);
            public const string UpdateUserInfo = nameof(UpdateUserInfo);
        }
        /// <summary>
        /// 存储商店模块用到的命令
        /// </summary>
        public static class StoreNotification
        {

        }
        /// <summary>
        /// 存储仓库模块用到的命令
        /// </summary>
        public static class GarageNotification
        {

        }
        /// <summary>
        /// 存储房间模块用到的命令
        /// </summary>
        public static class RoomNotification
        {

        }
        /// <summary>
        /// 存储战斗模块用到的命令
        /// </summary>
        public static class BattleNotification
        {

        }
    }
}
