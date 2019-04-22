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
        public MainMenuProxy(string proxyName, object data = null) : base(proxyName, data)
        {
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
            JObject o = new JObject
            {
                { "Command", NotifyConsts.MainMenuNotification.RequestChangePassword },
                {
                    "Paras", new JObject
                    {
                        { "UserId",_localPlayer.UserID},
                        { "Token",_localPlayer.Token},
                        { "OldPassword",oldPassword},
                        { "NewPassword",newPassword}
                    }
                }
            };
            _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
        }
        public void ChangePasswordResult(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
        }
    }
}
