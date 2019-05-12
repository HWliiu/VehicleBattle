using PureMVC.Patterns.Proxy;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GameClient.Service;
using System;
using GameClient.Common;

namespace GameClient.Model
{
    class LoginProxy : Proxy
    {
        //对数据模型的操作，不论是对于客户端还是对于服务端，后续的添加和修改接口只在Proxy中完成
        private RequestInterceptor _loginInterceptor;
        private RequestInterceptor _registerInterceptor;
        private RequestInterceptor _logoutInterceptor;
        public LoginProxy(string proxyName, object data = null) : base(proxyName, data)
        {
            _loginInterceptor = new RequestInterceptor(1f);
            _loginInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.LoginNotification.LoginResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
            _registerInterceptor = new RequestInterceptor(1f);
            _registerInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.LoginNotification.RegisterResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
            _logoutInterceptor = new RequestInterceptor(1f);
            _logoutInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.LoginNotification.LogoutResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
        }
        public void RequestLogin(string username, string password)
        {
            if (_loginInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.LoginNotification.RequestLogin },
                    {
                        "Paras", new JObject
                        {
                            { "UserName",username },
                            { "Password",password }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _loginInterceptor.BeginWaitResponseAsync();
            }
        }
        public void LoginResult(JObject jsonData)
        {
            _loginInterceptor.EndWaitResponse();
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                //更新VO
                var userId = (string)jsonData.SelectToken("Paras.UserInfo.Id");
                var userName = (string)jsonData.SelectToken("Paras.UserInfo.UserName");
                var token = (string)jsonData.SelectToken("Paras.UserInfo.Token");
                var experience = (int)jsonData.SelectToken("Paras.UserInfo.Experience");
                var money = (int)jsonData.SelectToken("Paras.UserInfo.Money");
                var level = (int)jsonData.SelectToken("Paras.UserInfo.Level");
                var curVehicleId = (string)jsonData.SelectToken("Paras.UserInfo.CurVehicleId");
                var registerTime = (string)jsonData.SelectToken("Paras.UserInfo.RegisterTime");
                var loginTime = (string)jsonData.SelectToken("Paras.UserInfo.LastLoginTime");
                List<VehicleVO> vehicleList = new List<VehicleVO>();
                VehicleVO curVehicle = null;
                foreach (JToken jToken in jsonData.SelectTokens("Paras.UserInfo.VehicleList").Children())
                {
                    var vehicleId = (string)jToken.SelectToken("Id");
                    var vehicleName = (string)jToken.SelectToken("Name");
                    var vehicleType = (string)jToken.SelectToken("Type");
                    var vehicleAttack = (float)jToken.SelectToken("Attack");
                    var vehicleMotility = (float)jToken.SelectToken("Motility");
                    var vehicleDefend = (float)jToken.SelectToken("Defend");
                    var vehicleMaxHealth = (int)(float)jToken.SelectToken("MaxHealth");
                    var vehiclePrice = (int)jToken.SelectToken("Price");
                    var vehicleIntro = (string)jToken.SelectToken("Intro");
                    VehicleVO vehicle = new VehicleVO(vehicleId, vehicleName, Enum.TryParse(vehicleType, true, out VehicleType type) ? type : throw new InvalidCastException(nameof(vehicleType)), vehicleAttack, vehicleMotility, vehicleDefend, vehicleMaxHealth, vehiclePrice, vehicleIntro);
                    vehicleList.Add(vehicle);
                    if (vehicle.VehicleID == curVehicleId)
                    {
                        curVehicle = vehicle;
                    }
                }

                PlayerManager.Instance.InitLocalPlayer(userId, userName, token, level, experience, money, registerTime, loginTime, curVehicle, vehicleList);

                SendNotification(NotifyConsts.LoginNotification.LoginResult, Tuple.Create(true, info), nameof(Tuple<bool, string>));
            }
            else
            {
                SendNotification(NotifyConsts.LoginNotification.LoginResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
            }
        }
        public void RequestRegister(string username, string password)
        {
            if (_registerInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.LoginNotification.RequestRegister },
                    {
                        "Paras", new JObject
                        {
                            { "UserName",username },
                            { "Password",password }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _registerInterceptor.BeginWaitResponseAsync();
            }
        }

        public void RegisterResult(JObject jsonData)
        {
            _registerInterceptor.EndWaitResponse();
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                SendNotification(NotifyConsts.LoginNotification.RegisterResult, Tuple.Create(true, info), nameof(Tuple<bool, string>));
            }
            else
            {
                SendNotification(NotifyConsts.LoginNotification.RegisterResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
            }
        }

        public void RequestLogout()
        {
            if (_logoutInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.LoginNotification.RequestLogout },
                    {
                        "Paras", new JObject
                        {
                            { "UserId",PlayerManager.Instance.LocalPlayer.UserID },
                            { "Token",PlayerManager.Instance.LocalPlayer.Token }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _logoutInterceptor.BeginWaitResponseAsync();
            }
        }

        public void LogoutResult(JObject jsonData)
        {
            _logoutInterceptor.EndWaitResponse();
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                SendNotification(NotifyConsts.LoginNotification.LogoutResult, Tuple.Create(true, info), nameof(Tuple<bool, string>));
            }
            else
            {
                SendNotification(NotifyConsts.LoginNotification.LogoutResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
            }
        }

        public override void OnRegister()
        {
            base.OnRegister();
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }
    }
}
