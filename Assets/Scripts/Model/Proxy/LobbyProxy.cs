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
    class LobbyProxy : Proxy
    {
        private LocalPlayerVO _localPlayer;
        public LobbyProxy(string proxyName, object data = null) : base(proxyName, data)
        {
        }

        public void RequestCreateRoom(string roomName, string roomMode, string roomMap)
        {
            JObject o = new JObject
            {
                { "Command", NotifyConsts.LobbyNotification.RequestCreateRoom },
                {
                    "Paras", new JObject
                    {
                        { "UserId",_localPlayer.UserID },
                        { "Token",_localPlayer.Token },
                        { "RoomName",roomName },
                        { "RoomMode",roomMode },
                        { "RoomMap",roomMap },
                    }
                }
            };
            _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
        }
        public void CreateRoomResult(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                var roomId = (string)jsonData.SelectToken("Paras.RoomInfo.RoomId");
                var roomName = (string)jsonData.SelectToken("Paras.RoomInfo.RoomName");
                var ownerName = (string)jsonData.SelectToken("Paras.RoomInfo.OwnerName");
                var roomMode = (string)jsonData.SelectToken("Paras.RoomInfo.RoomMode");
                var roomMap = (string)jsonData.SelectToken("Paras.RoomInfo.RoomMap");
                var playerNum = (string)jsonData.SelectToken("Paras.RoomInfo.PlayerNum");

                //SendNotification(NotifyConsts.LobbyNotification.CreateRoomResult, Tuple.Create(true, info, curVehicleId), nameof(Tuple<bool, string, string>));
            }
            else
            {
                SendNotification(NotifyConsts.LobbyNotification.CreateRoomResult, Tuple.Create<bool, string, string>(false, info, null), nameof(Tuple<bool, string, string>));
            }
        }

        public void RequestSearchRoom(string roomId)
        {
            JObject o = new JObject
            {
                { "Command", NotifyConsts.LobbyNotification.RequestSearchRoom },
                {
                    "Paras", new JObject
                    {
                        { "UserId",_localPlayer.UserID },
                        { "Token",_localPlayer.Token },
                        { "RoomId",roomId },
                    }
                }
            };
            _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
        }
        public void SearchRoomResult(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
        }

        public void RequestRefreshRoomList()
        {
            JObject o = new JObject
            {
                { "Command", NotifyConsts.LobbyNotification.RequestRefreshRoomList },
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
        public void RefreshRoomListResult(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
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
    }
}
