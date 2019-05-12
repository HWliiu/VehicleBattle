using Newtonsoft.Json.Linq;
using PureMVC.Patterns.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Model
{
    class RoomProxy : Proxy
    {
        private LocalPlayerVO _localPlayer;

        public RoomProxy(string proxyName, object data = null) : base(proxyName, data)
        {
        }

        public void NetPlayerJoinRoom(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            var playerId = (string)jsonData.SelectToken("Paras.PlayerInfo.PlayerId");
            var playerName = (string)jsonData.SelectToken("Paras.PlayerInfo.PlayerName");
            var playerLevel = (string)jsonData.SelectToken("Paras.PlayerInfo.PlayerLevel");
            var prepareState = (bool)jsonData.SelectToken("Paras.PlayerInfo.PrepareState");

            var vehicleId = (string)jsonData.SelectToken("Paras.PlayerInfo.VehicleInfo.Id");
            var vehicleName = (string)jsonData.SelectToken("Paras.PlayerInfo.VehicleInfo.Name");
            var vehicleType = (string)jsonData.SelectToken("Paras.PlayerInfo.VehicleInfo.Type");
            var vehicleAttack = (float)jsonData.SelectToken("Paras.PlayerInfo.VehicleInfo.Attack");
            var vehicleDefend = (float)jsonData.SelectToken("Paras.PlayerInfo.VehicleInfo.Defend");
            var vehicleMotility = (float)jsonData.SelectToken("Paras.PlayerInfo.VehicleInfo.Motility");
            var vehicleMaxHealth = (float)jsonData.SelectToken("Paras.PlayerInfo.VehicleInfo.MaxHealth");

            var type = Enum.TryParse(vehicleType, true, out VehicleType vt) ? vt : throw new InvalidCastException(nameof(vehicleType));
            var vehicle = new VehicleVO(vehicleId, vehicleName, type, vehicleAttack, vehicleMotility, vehicleDefend, (int)vehicleMaxHealth, 0, null);
            var player = new NetPlayerVO(playerId, playerName, int.Parse(playerLevel), vehicle)
            {
                PrepareState = prepareState,
            };
            PlayerManager.Instance.AddNetPlayer(playerId, playerName, int.Parse(playerLevel), vehicle);
            SendNotification(NotifyConsts.RoomNotification.NetPlayerJoinRoom, player, nameof(NetPlayerVO));
        }

        public void RequestExitRoom() { }
        public void ExitRoomResult(JObject jsonData) { }

        public void RequestChangePrepareState() { }
        public void ChangePrepareStateResult(JObject jsonData) { }

        public void RequestStartGame() { }
        public void StartGameResult(JObject jsonData) { }

        public void RequestKickPlayer() { }
        public void KickPlayerResult(JObject jsonData) { }

        public void RequestSendMessage() { }
        public void SendMessageResult(JObject jsonData) { }

    }
}
