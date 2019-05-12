using GameClient.Common;
using GameClient.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PureMVC.Patterns.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Model
{
    class MainMenuProxy : Proxy
    {
        private LocalPlayerVO _localPlayer;
        private RequestInterceptor _changePwdInterceptor;
        public MainMenuProxy(string proxyName, object data = null) : base(proxyName, data)
        {
            _changePwdInterceptor = new RequestInterceptor(1f);
            _changePwdInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.MainMenuNotification.ChangePasswordResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _localPlayer = PlayerManager.Instance.LocalPlayer;
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }

        public void RequestChangePassword(string oldPassword, string newPassword)
        {
            if (_changePwdInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.MainMenuNotification.RequestChangePassword },
                    {
                        "Paras", new JObject
                        {
                            { "UserId",_localPlayer.UserID },
                            { "Token",_localPlayer.Token },
                            { "OldPassword",oldPassword },
                            { "NewPassword",newPassword }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _changePwdInterceptor.BeginWaitResponseAsync();
            }
        }
        public void ChangePasswordResult(JObject jsonData)
        {
            _changePwdInterceptor.EndWaitResponse();
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                SendNotification(NotifyConsts.MainMenuNotification.ChangePasswordResult, Tuple.Create(true, info), nameof(Tuple<bool, string>));
            }
            else
            {
                SendNotification(NotifyConsts.MainMenuNotification.ChangePasswordResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
            }
        }
    }
}
