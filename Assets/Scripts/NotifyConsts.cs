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
            public const string StartUp = "StartUp";
            public const string UpdateConnState = "UpdateConnState";
        }
        /// <summary>
        /// 存储登录注册模块用到的命令
        /// </summary>
        public static class LoginNotification
        {
            public const string RequestLogin = "RequestLogin";
            public const string RequestRegister = "RequestRegister";
            public const string RequestLogout = "RequestLogout";
            public const string LoginResult = "LoginResult";
            public const string RegisterResult = "RegisterResult";
            public const string LogoutResult = "LogoutResult";
        }
        /// <summary>
        /// 存储主菜单用到的命令
        /// </summary>
        public static class MainMenuNotification
        {

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
