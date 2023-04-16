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
    class StoreProxy : Proxy
    {
        private LocalPlayerVO _localPlayer;
        private RequestInterceptor _purchaseInterceptor;
        public StoreProxy(string proxyName, object data = null) : base(proxyName, data)
        {
            _purchaseInterceptor = new RequestInterceptor(1f);
            _purchaseInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.StoreNotification.PurchaseItemResult, Tuple.Create<bool, string, VehicleVO>(false, info, null), nameof(Tuple<bool, string, VehicleVO>));
        }
        public void RequestStoreItemList()
        {
            JObject o = new JObject
            {
                { "Command", NotifyConsts.StoreNotification.RequestStoreItemList },
                {
                    "Paras", new JObject
                    {
                        { "UserId",_localPlayer.UserID },
                        { "Token",_localPlayer.Token }
                    }
                }
            };
            _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
        }
        public void StoreItemListResult(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            List<VehicleVO> vehicleList = new List<VehicleVO>();
            foreach (JToken jToken in jsonData.SelectTokens("Paras.VehicleInfo").Children())
            {
                var vehicleId = (string)jToken.SelectToken("Id");
                var vehicleName = (string)jToken.SelectToken("Name");
                var vehicleType = (string)jToken.SelectToken("Type");
                var vehicleAttack = (float)jToken.SelectToken("Attack");
                var vehicleMotility = (float)jToken.SelectToken("Motility");
                var vehicleDefend = (float)jToken.SelectToken("Defend");
                var vehicleMaxHealth = (int)(float)jToken.SelectToken("MaxHealth");
                var vehiclePrice = (int)(float)jToken.SelectToken("Price");
                var vehicleIntro = (string)jToken.SelectToken("Intro");
                vehicleList.Add(new VehicleVO(vehicleId, vehicleName, Enum.TryParse(vehicleType, true, out VehicleType type) ? type : throw new InvalidCastException(nameof(vehicleType)), vehicleAttack, vehicleMotility, vehicleDefend, vehicleMaxHealth, vehiclePrice, vehicleIntro));
            }
            SendNotification(NotifyConsts.StoreNotification.StoreItemListResult, vehicleList, nameof(List<VehicleVO>));
        }
        public void RequestPurchaseItem(string itemId)
        {
            if (_purchaseInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.StoreNotification.RequestPurchaseItem },
                    {
                        "Paras", new JObject
                        {
                            { "UserId",_localPlayer.UserID },
                            { "Token",_localPlayer.Token },
                            { "VehicleId",itemId }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _purchaseInterceptor.BeginWaitResponseAsync();
            }
        }
        public void PurchaseItemResult(JObject jsonData)
        {
            _purchaseInterceptor.EndWaitResponse();
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                var userMoney = (int)jsonData.SelectToken("Paras.UserInfo.Money");
                _localPlayer.Money = userMoney;

                var vehicleId = (string)jsonData.SelectToken("Paras.VehicleInfo.Id");
                var vehicleName = (string)jsonData.SelectToken("Paras.VehicleInfo.Name");
                var vehicleType = (string)jsonData.SelectToken("Paras.VehicleInfo.Type");
                var vehicleAttack = (float)jsonData.SelectToken("Paras.VehicleInfo.Attack");
                var vehicleMotility = (float)jsonData.SelectToken("Paras.VehicleInfo.Motility");
                var vehicleDefend = (float)jsonData.SelectToken("Paras.VehicleInfo.Defend");
                var vehicleMaxHealth = (int)(float)jsonData.SelectToken("Paras.VehicleInfo.MaxHealth");
                var vehiclePrice = (int)jsonData.SelectToken("Paras.VehicleInfo.Price");
                var vehicleIntro = (string)jsonData.SelectToken("Paras.VehicleInfo.Intro");

                var vehicle = new VehicleVO(vehicleId, vehicleName, Enum.TryParse(vehicleType, true, out VehicleType type) ? type : throw new InvalidCastException(nameof(vehicleType)), vehicleAttack, vehicleMotility, vehicleDefend, vehicleMaxHealth, vehiclePrice, vehicleIntro);
                _localPlayer.VehicleList.Add(vehicle);

                SendNotification(NotifyConsts.StoreNotification.PurchaseItemResult, Tuple.Create(true, info, vehicle), nameof(Tuple<bool, string, VehicleVO>));
                SendNotification(NotifyConsts.MainMenuNotification.UpdateUserInfo, _localPlayer, nameof(LocalPlayerVO));
            }
            else
            {
                SendNotification(NotifyConsts.StoreNotification.PurchaseItemResult, Tuple.Create<bool, string, VehicleVO>(false, info, null), nameof(Tuple<bool, string, VehicleVO>));
            }

        }

        public override void OnRegister()
        {
            _localPlayer = PlayerManager.Instance.LocalPlayer;

            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.StoreNotification.PurchaseItemResult, PurchaseItemResult);
            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.StoreNotification.StoreItemListResult, StoreItemListResult);
        }

        public override void OnRemove()
        {
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.StoreNotification.PurchaseItemResult);
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.StoreNotification.StoreItemListResult);
        }
    }
}
