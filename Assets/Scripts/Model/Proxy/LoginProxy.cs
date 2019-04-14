using PureMVC.Patterns.Proxy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GameClient.Service;
using System;

namespace GameClient.Model
{
    public class LoginProxy : Proxy
    {
        //对数据模型的操作，不论是对于客户端还是对于服务端，后续的添加和修改接口只在Proxy中完成
        private UserVO _userVO;
        public LoginProxy(string proxyName, object data = null) : base(proxyName, data)
        {
            _userVO = (data as UserVO) ?? throw new Exception("data cast error");
        }
        public void RequestLogin(string username, string password)
        {
            JObject o = new JObject
            {
                { "Command", nameof(RequestLogin) },
                {
                    "Paras", new JObject
                    {
                        { "Username",username},
                        { "Password",password}
                    }
                }
            };
            _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
        }
        public void LoginResult()
        {

        }
        public void RequestRegister(string username, string password)
        {
            JObject o = new JObject
            {
                { "Command", nameof(RequestRegister) },
                {
                    "Paras", new JObject
                    {
                        { "Username",username},
                        { "Password",password}
                    }
                }
            };
            _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
        }

        public void RegisterResult()
        {

        }

        public void RequestLogout(string username)
        {

        }

        public void LogoutResult()
        {

        }
    }
}
