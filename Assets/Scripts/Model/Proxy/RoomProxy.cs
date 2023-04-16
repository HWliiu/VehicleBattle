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
using UnityEngine;

namespace GameClient.Model
{
    class RoomProxy : Proxy
    {
        private LocalPlayerVO _localPlayer;
        private RequestInterceptor _exitRoomInterceptor;
        private RequestInterceptor _changePrepareStateInterceptor;
        private RequestInterceptor _kickPlayerInterceptor;
        private RequestInterceptor _startGameInterceptor;

        public RoomProxy(string proxyName, object data = null) : base(proxyName, data)
        {
            _exitRoomInterceptor = new RequestInterceptor(3f);
            _exitRoomInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.RoomNotification.ExitRoomResult, Tuple.Create<bool, string, string, List<PlayerVO>>(false, info, null, null), null);
            _changePrepareStateInterceptor = new RequestInterceptor(1f);
            _changePrepareStateInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.RoomNotification.ChangePrepareStateResult, Tuple.Create<bool, string, string, bool>(false, info, null, false), null);
            _kickPlayerInterceptor = new RequestInterceptor(1f);
            _kickPlayerInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.RoomNotification.KickPlayerResult, Tuple.Create<bool, string, string, List<PlayerVO>>(false, info, null, null), null);
            _startGameInterceptor = new RequestInterceptor(1f);
            _startGameInterceptor.OnRequestStateChange += (info) => SendNotification(NotifyConsts.RoomNotification.StartGameResult, Tuple.Create(false, info), null);
        }

        public void RequestExitRoom()
        {
            if (_exitRoomInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.RoomNotification.RequestExitRoom },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", _localPlayer.UserID },
                            { "Token", _localPlayer.Token }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _exitRoomInterceptor.BeginWaitResponseAsync();
            }
        }
        public void ExitRoomResult(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                string exitPlayerId = (string)jsonData.SelectToken("Paras.ExitPlayerId");
                if (exitPlayerId == _localPlayer.UserID)
                {
                    _exitRoomInterceptor.EndWaitResponse();
                    PlayerManager.Instance.RoomOwner = null;
                    PlayerManager.Instance.RemoveAllNetPlayer();
                    SendNotification(NotifyConsts.RoomNotification.ExitRoomResult, Tuple.Create<bool, string, string, List<PlayerVO>>(true, info, exitPlayerId, null), null);
                }
                else
                {
                    if (PlayerManager.Instance.RoomOwner.UserID == exitPlayerId)
                    {
                        string newOwnerId = (string)jsonData.SelectToken("Paras.RoomInfo.OwnerId");
                        PlayerManager.Instance.RoomOwner = PlayerManager.Instance.GetPlayer(newOwnerId);
                    }
                    PlayerManager.Instance.RemoveNetPlayer(exitPlayerId);
                    //剩余的玩家列表
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

                        var vehicle = new VehicleVO(vehicleId, vehicleName, Enum.TryParse(vehicleType, true, out VehicleType vt) ? vt : throw new InvalidCastException(nameof(vehicleType)), vehicleAttack, vehicleMotility, vehicleDefend, (int)vehicleMaxHealth, 0, null);

                        var player = new PlayerVO(playerId, playerName, int.Parse(playerLevel), vehicle)
                        {
                            PrepareState = prepareState,
                            //Team = Enum.TryParse(playerTeam, true, out Team team) ? team : throw new InvalidCastException(nameof(playerTeam))
                        };
                        playerList.Add(player); //用于界面显示
                    }

                    SendNotification(NotifyConsts.RoomNotification.ExitRoomResult, Tuple.Create<bool, string, string, List<PlayerVO>>(true, info, exitPlayerId, playerList), null);
                }
            }
            else
            {
                _exitRoomInterceptor.EndWaitResponse();
                SendNotification(NotifyConsts.RoomNotification.ExitRoomResult, Tuple.Create<bool, string, string, List<PlayerVO>>(false, info, null, null), null);
            }
        }

        public void RequestChangePrepareState(bool prepareState)
        {
            if (_changePrepareStateInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.RoomNotification.RequestChangePrepareState },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", _localPlayer.UserID },
                            { "Token", _localPlayer.Token },
                            { "PrepareState", prepareState }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _changePrepareStateInterceptor.BeginWaitResponseAsync();
            }
        }
        public void ChangePrepareStateResult(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                string playerId = (string)jsonData.SelectToken("Paras.PlayerId");
                bool prepareState = (bool)jsonData.SelectToken("Paras.PrepareState");
                if (playerId == _localPlayer.UserID)
                {
                    _changePrepareStateInterceptor.EndWaitResponse();
                }
                PlayerManager.Instance.GetPlayer(playerId).PrepareState = prepareState;
                SendNotification(NotifyConsts.RoomNotification.ChangePrepareStateResult, Tuple.Create<bool, string, string, bool>(true, info, playerId, prepareState), null);
            }
            else
            {
                _changePrepareStateInterceptor.EndWaitResponse();
                SendNotification(NotifyConsts.RoomNotification.ChangePrepareStateResult, Tuple.Create<bool, string, string, bool>(false, info, null, false), null);
            }
        }

        public void RequestStartGame()
        {
            if (_startGameInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.RoomNotification.RequestStartGame },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", _localPlayer.UserID },
                            { "Token", _localPlayer.Token }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _startGameInterceptor.BeginWaitResponseAsync();
            }
        }
        public void StartGameResult(JObject jsonData)
        {
            _startGameInterceptor.EndWaitResponse();
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                PlayerManager.Instance.PlayerOrder = jsonData.SelectTokens("Paras.PlayerOrder").Children().Select(t => (string)t).ToArray();
                SendNotification(NotifyConsts.RoomNotification.StartGameResult, Tuple.Create(true, info), null);
            }
            else
            {
                SendNotification(NotifyConsts.RoomNotification.StartGameResult, Tuple.Create(false, info), null);
            }
        }

        public void RequestKickPlayer(string playerId)
        {
            if (_kickPlayerInterceptor.AllowRequest())
            {
                JObject o = new JObject
                {
                    { "Command", NotifyConsts.RoomNotification.RequestKickPlayer },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", _localPlayer.UserID },
                            { "Token", _localPlayer.Token },
                            { "PlayerId", playerId }
                        }
                    }
                };
                _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
                _ = _kickPlayerInterceptor.BeginWaitResponseAsync();
            }
        }
        public void KickPlayerResult(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
            if (PlayerManager.Instance.RoomOwner.UserID == _localPlayer.UserID)
            {
                _kickPlayerInterceptor.EndWaitResponse();
            }
            string result = (string)jsonData.SelectToken("Paras.Result");
            string info = (string)jsonData.SelectToken("Paras.Info");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                string kickPlayerId = (string)jsonData.SelectToken("Paras.KickPlayerId");
                if (kickPlayerId == _localPlayer.UserID)
                {
                    PlayerManager.Instance.RoomOwner = null;
                    PlayerManager.Instance.RemoveAllNetPlayer();
                    SendNotification(NotifyConsts.RoomNotification.KickPlayerResult, Tuple.Create<bool, string, string, List<PlayerVO>>(true, info, kickPlayerId, null), null);
                }
                else
                {
                    PlayerManager.Instance.RemoveNetPlayer(kickPlayerId);
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

                        var vehicle = new VehicleVO(vehicleId, vehicleName, Enum.TryParse(vehicleType, true, out VehicleType vt) ? vt : throw new InvalidCastException(nameof(vehicleType)), vehicleAttack, vehicleMotility, vehicleDefend, (int)vehicleMaxHealth, 0, null);

                        var player = new PlayerVO(playerId, playerName, int.Parse(playerLevel), vehicle)
                        {
                            PrepareState = prepareState,
                            //Team = Enum.TryParse(playerTeam, true, out Team team) ? team : throw new InvalidCastException(nameof(playerTeam))
                        };
                        playerList.Add(player); //用于界面显示
                    }
                    SendNotification(NotifyConsts.RoomNotification.KickPlayerResult, Tuple.Create<bool, string, string, List<PlayerVO>>(true, info, kickPlayerId, playerList), null);
                }
            }
            else
            {
                SendNotification(NotifyConsts.RoomNotification.KickPlayerResult, Tuple.Create<bool, string, string, List<PlayerVO>>(false, info, null, null), null);
            }
        }

        public void RequestSendMessage(string message)
        {
            JObject o = new JObject
                {
                    { "Command", NotifyConsts.RoomNotification.RequestSendMessage },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", _localPlayer.UserID },
                            { "Token", _localPlayer.Token },
                            { "Message", message }
                        }
                    }
                };
            _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
        }
        public void SendMessageResult(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string result = (string)jsonData.SelectToken("Paras.Result");
            if (result == NotifyConsts.CommonNotification.Succeed)
            {
                string playerName = (string)jsonData.SelectToken("Paras.PlayerName");
                string message = (string)jsonData.SelectToken("Paras.Message");
                SendNotification(NotifyConsts.RoomNotification.SendMessageResult, Tuple.Create(playerName, message), null);
            }
        }

        public override void OnRegister()
        {
            _localPlayer = PlayerManager.Instance.LocalPlayer;

            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.RoomNotification.ExitRoomResult, ExitRoomResult);
            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.RoomNotification.ChangePrepareStateResult, ChangePrepareStateResult);
            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.RoomNotification.StartGameResult, StartGameResult);
            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.RoomNotification.KickPlayerResult, KickPlayerResult);
            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.RoomNotification.SendMessageResult, SendMessageResult);
        }

        public override void OnRemove()
        {
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.RoomNotification.ExitRoomResult);
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.RoomNotification.ChangePrepareStateResult);
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.RoomNotification.StartGameResult);
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.RoomNotification.KickPlayerResult);
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.RoomNotification.SendMessageResult);
        }
    }
}
