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
    class LobbyProxy : Proxy
    {
        private LocalPlayerVO _localPlayer;
        private RequestInterceptor _createRoomInterceptor;
        private RequestInterceptor _searchRoomInterceptor;
        private RequestInterceptor _refreshRoomListInterceptor;
        private RequestInterceptor _joinRoomInterceptor;
        public LobbyProxy(string proxyName, object data = null) : base(proxyName, data)
        {
            _createRoomInterceptor = new RequestInterceptor(3f);
            _searchRoomInterceptor = new RequestInterceptor(1f);
            _refreshRoomListInterceptor = new RequestInterceptor(1f);
            _refreshRoomListInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.LobbyNotification.RefreshRoomListResult, Tuple.Create<bool, string, List<RoomVO>>(false, info, null), nameof(Tuple<bool, string, List<RoomVO>>));
            _joinRoomInterceptor = new RequestInterceptor(1f);
        }

        public void RequestCreateRoom(string roomName, string roomMode, string roomMap)
        {
            if (_createRoomInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.LobbyNotification.RequestCreateRoom },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", _localPlayer.UserID },
                            { "Token", _localPlayer.Token },
                            { "RoomName", roomName },
                            { "RoomMode", roomMode },
                            { "RoomMap", roomMap },
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _createRoomInterceptor.BeginWaitResponseAsync();
            }
        }
        public void CreateRoomResult(JObject jsonData)
        {
            _createRoomInterceptor.EndWaitResponse();
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
                var ownerId = (string)jsonData.SelectToken("Paras.RoomInfo.OwnerId");
                var ownerName = (string)jsonData.SelectToken("Paras.RoomInfo.OwnerName");
                var roomMode = (string)jsonData.SelectToken("Paras.RoomInfo.RoomMode");
                var roomMap = (string)jsonData.SelectToken("Paras.RoomInfo.RoomMap");
                var playerNum = (int)jsonData.SelectToken("Paras.RoomInfo.PlayerNum");
                var room = new RoomVO(roomId, roomName, ownerId, ownerName, Enum.TryParse(roomMode, true, out RoomMode roomModeType) ? roomModeType : throw new InvalidCastException(nameof(roomMode)), Enum.TryParse(roomMap, true, out RoomMap roomMapType) ? roomMapType : throw new InvalidCastException(nameof(roomMap)), playerNum);

                var playerList = new List<PlayerVO>();
                foreach (JToken jToken in jsonData.SelectTokens("Paras.RoomInfo.PlayerList").Children())
                {
                    var playerId = (string)jToken.SelectToken("PlayerId");
                    var playerName = (string)jToken.SelectToken("PlayerName");
                    var playerLevel = (string)jToken.SelectToken("PlayerLevel");
                    var prepareState = (bool)jToken.SelectToken("PrepareState");
                    //var playerTeam = (string)jToken.SelectToken("PlayerTeam");

                    var vehicleId = (string)jToken.SelectToken("VehicleInfo.Id");
                    var vehicleName = (string)jToken.SelectToken("VehicleInfo.Name");
                    var vehicleType = (string)jToken.SelectToken("VehicleInfo.Type");
                    var vehicleAttack = (float)jToken.SelectToken("VehicleInfo.Attack");
                    var vehicleDefend = (float)jToken.SelectToken("VehicleInfo.Defend");
                    var vehicleMotility = (float)jToken.SelectToken("VehicleInfo.Motility");
                    var vehicleMaxHealth = (float)jToken.SelectToken("VehicleInfo.MaxHealth");

                    var type = Enum.TryParse(vehicleType, true, out VehicleType vt) ? vt : throw new InvalidCastException(nameof(vehicleType));
                    var vehicle = new VehicleVO(vehicleId, vehicleName, type, vehicleAttack, vehicleMotility, vehicleDefend, (int)vehicleMaxHealth, 0, null);
                    var player = new NetPlayerVO(playerId, playerName, int.Parse(playerLevel), vehicle)
                    {
                        PrepareState = prepareState,
                        //Team = Enum.TryParse(playerTeam, true, out Team team) ? team : throw new InvalidCastException(nameof(playerTeam))
                    };
                    playerList.Add(player);
                }

                SendNotification(NotifyConsts.LobbyNotification.CreateRoomResult, Tuple.Create(true, info), nameof(Tuple<bool, string>));
                SendNotification(NotifyConsts.RoomNotification.InitRoomInfo, Tuple.Create(room, playerList), nameof(Tuple<RoomVO, List<PlayerVO>>));
            }
            else
            {
                SendNotification(NotifyConsts.LobbyNotification.CreateRoomResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
            }
        }

        public void RequestSearchRoom(string roomId)
        {
            if (_searchRoomInterceptor.AllowRequest())
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
                _ = _searchRoomInterceptor.BeginWaitResponseAsync();
            }
        }
        public void SearchRoomResult(JObject jsonData)
        {
            _searchRoomInterceptor.EndWaitResponse();
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                List<RoomVO> roomList = new List<RoomVO>();
                var roomId = (string)jsonData.SelectToken("Paras.RoomInfo.RoomId");
                var roomName = (string)jsonData.SelectToken("Paras.RoomInfo.RoomName");
                var ownerId = (string)jsonData.SelectToken("Paras.RoomInfo.OwnerId");
                var ownerName = (string)jsonData.SelectToken("Paras.RoomInfo.OwnerName");
                var roomMode = (string)jsonData.SelectToken("Paras.RoomInfo.RoomMode");
                var roomMap = (string)jsonData.SelectToken("Paras.RoomInfo.RoomMap");
                var playerNum = (int)jsonData.SelectToken("Paras.RoomInfo.PlayerNum");
                RoomVO room = new RoomVO(roomId, roomName, ownerId, ownerName, Enum.TryParse(roomMode, true, out RoomMode roomModeType) ? roomModeType : throw new InvalidCastException(nameof(roomMode)), Enum.TryParse(roomMap, true, out RoomMap roomMapType) ? roomMapType : throw new InvalidCastException(nameof(roomMap)), playerNum);
                roomList.Add(room);
                SendNotification(NotifyConsts.LobbyNotification.SearchRoomResult, Tuple.Create(true, info), nameof(Tuple<bool, string>));
                SendNotification(NotifyConsts.LobbyNotification.RefreshRoomListResult, Tuple.Create(true, "", roomList), nameof(Tuple<bool, string, List<RoomVO>>));
            }
            else
            {
                SendNotification(NotifyConsts.LobbyNotification.SearchRoomResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
            }
        }

        public void RequestRefreshRoomList()
        {
            if (_refreshRoomListInterceptor.AllowRequest())
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
                _ = _refreshRoomListInterceptor.BeginWaitResponseAsync();
            }
        }
        public void RefreshRoomListResult(JObject jsonData)
        {
            _refreshRoomListInterceptor.EndWaitResponse();
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                List<RoomVO> roomList = new List<RoomVO>();
                foreach (JToken jToken in jsonData.SelectTokens("Paras.RoomList").Children())
                {
                    var roomId = (string)jToken.SelectToken("RoomInfo.RoomId");
                    var roomName = (string)jToken.SelectToken("RoomInfo.RoomName");
                    var ownerId = (string)jToken.SelectToken("RoomInfo.OwnerId");
                    var ownerName = (string)jToken.SelectToken("RoomInfo.OwnerName");
                    var roomMode = (string)jToken.SelectToken("RoomInfo.RoomMode");
                    var roomMap = (string)jToken.SelectToken("RoomInfo.RoomMap");
                    var playerNum = (int)jToken.SelectToken("RoomInfo.PlayerNum");
                    RoomVO room = new RoomVO(roomId, roomName, ownerId, ownerName, Enum.TryParse(roomMode, true, out RoomMode roomModeType) ? roomModeType : throw new InvalidCastException(nameof(roomMode)), Enum.TryParse(roomMap, true, out RoomMap roomMapType) ? roomMapType : throw new InvalidCastException(nameof(roomMap)), playerNum);
                    roomList.Add(room);
                }
                SendNotification(NotifyConsts.LobbyNotification.RefreshRoomListResult, Tuple.Create(true, info, roomList), nameof(Tuple<bool, string, List<RoomVO>>));
            }
            else
            {
                SendNotification(NotifyConsts.LobbyNotification.RefreshRoomListResult, Tuple.Create<bool, string, List<RoomVO>>(false, info, null), nameof(Tuple<bool, string, List<RoomVO>>));
            }
        }

        public void RequestJoinRoom(string roomId)
        {
            if (_joinRoomInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.LobbyNotification.RequestJoinRoom },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", _localPlayer.UserID },
                            { "Token", _localPlayer.Token },
                            { "RoomId", roomId }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _joinRoomInterceptor.BeginWaitResponseAsync();
            }
        }
        public void JoinRoomResult(JObject jsonData)
        {
            _joinRoomInterceptor.EndWaitResponse();
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
                var ownerId = (string)jsonData.SelectToken("Paras.RoomInfo.OwnerId");
                var ownerName = (string)jsonData.SelectToken("Paras.RoomInfo.OwnerName");
                var roomMode = (string)jsonData.SelectToken("Paras.RoomInfo.RoomMode");
                var roomMap = (string)jsonData.SelectToken("Paras.RoomInfo.RoomMap");
                var playerNum = (int)jsonData.SelectToken("Paras.RoomInfo.PlayerNum");
                var room = new RoomVO(roomId, roomName, ownerId, ownerName, Enum.TryParse(roomMode, true, out RoomMode roomModeType) ? roomModeType : throw new InvalidCastException(nameof(roomMode)), Enum.TryParse(roomMap, true, out RoomMap roomMapType) ? roomMapType : throw new InvalidCastException(nameof(roomMap)), playerNum);

                var playerList = new List<PlayerVO>();
                foreach (JToken jToken in jsonData.SelectTokens("Paras.RoomInfo.PlayerList").Children())
                {
                    var playerId = (string)jToken.SelectToken("PlayerId");
                    var playerName = (string)jToken.SelectToken("PlayerName");
                    var playerLevel = (string)jToken.SelectToken("PlayerLevel");
                    var prepareState = (bool)jToken.SelectToken("PrepareState");
                    //var playerTeam = (string)jToken.SelectToken("PlayerTeam");

                    var vehicleId = (string)jToken.SelectToken("VehicleInfo.Id");
                    var vehicleName = (string)jToken.SelectToken("VehicleInfo.Name");
                    var vehicleType = (string)jToken.SelectToken("VehicleInfo.Type");
                    var vehicleAttack = (float)jToken.SelectToken("VehicleInfo.Attack");
                    var vehicleDefend = (float)jToken.SelectToken("VehicleInfo.Defend");
                    var vehicleMotility = (float)jToken.SelectToken("VehicleInfo.Motility");
                    var vehicleMaxHealth = (float)jToken.SelectToken("VehicleInfo.MaxHealth");

                    var type = Enum.TryParse(vehicleType, true, out VehicleType vt) ? vt : throw new InvalidCastException(nameof(vehicleType));
                    var vehicle = new VehicleVO(vehicleId, vehicleName, type, vehicleAttack, vehicleMotility, vehicleDefend, (int)vehicleMaxHealth, 0, null);
                    var player = new NetPlayerVO(playerId, playerName, int.Parse(playerLevel), vehicle)
                    {
                        PrepareState = prepareState,
                        //Team = Enum.TryParse(playerTeam, true, out Team team) ? team : throw new InvalidCastException(nameof(playerTeam))
                    };
                    playerList.Add(player);

                    if (playerId != _localPlayer.UserID)
                    {
                        PlayerManager.Instance.AddNetPlayer(playerId, playerName, int.Parse(playerLevel), vehicle);
                    }
                }

                SendNotification(NotifyConsts.LobbyNotification.JoinRoomResult, Tuple.Create(true, info), nameof(Tuple<bool, string>));
                SendNotification(NotifyConsts.RoomNotification.InitRoomInfo, Tuple.Create(room, playerList), nameof(Tuple<RoomVO, List<PlayerVO>>));
            }
            else
            {
                SendNotification(NotifyConsts.LobbyNotification.JoinRoomResult, Tuple.Create(false, info), nameof(Tuple<bool, string>));
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
