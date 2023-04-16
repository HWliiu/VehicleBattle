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
    class GarageProxy : Proxy
    {
        private LocalPlayerVO _localPlayer;
        private RequestInterceptor _changeVehicleInterceptor;
        public GarageProxy(string proxyName, object data = null) : base(proxyName, data)
        {
            _changeVehicleInterceptor = new RequestInterceptor(1f);
            _changeVehicleInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.GarageNotification.ChangeVehicleResult, Tuple.Create<bool, string, string>(false, info, null), nameof(Tuple<bool, string, string>));
        }
        public void RequestChangeVehicle(string vehicleId)
        {
            if (_changeVehicleInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.GarageNotification.RequestChangeVehicle },
                    {
                        "Paras", new JObject
                        {
                            { "UserId",_localPlayer.UserID },
                            { "Token",_localPlayer.Token },
                            { "VehicleId",vehicleId }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _changeVehicleInterceptor.BeginWaitResponseAsync();
            }
        }
        public void ChangeVehicleResult(JObject jsonData)
        {
            _changeVehicleInterceptor.EndWaitResponse();
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                var curVehicleId = (string)jsonData.SelectToken("Paras.UserInfo.CurVehicleId");
                _localPlayer.CurVehicle = _localPlayer.FindVehicle(curVehicleId);

                SendNotification(NotifyConsts.GarageNotification.ChangeVehicleResult, Tuple.Create(true, info, curVehicleId), nameof(Tuple<bool, string, string>));
                SendNotification(NotifyConsts.MainMenuNotification.UpdateUserInfo, _localPlayer, nameof(LocalPlayerVO));
            }
            else
            {
                SendNotification(NotifyConsts.GarageNotification.ChangeVehicleResult, Tuple.Create<bool, string, string>(false, info, null), nameof(Tuple<bool, string, string>));
            }
        }
        public override void OnRegister()
        {
            _localPlayer = PlayerManager.Instance.LocalPlayer;

            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.GarageNotification.ChangeVehicleResult, ChangeVehicleResult);
        }

        public override void OnRemove() => CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.GarageNotification.ChangeVehicleResult);
    }
}
