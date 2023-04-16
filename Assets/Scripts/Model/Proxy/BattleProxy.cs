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
    class BattleProxy : Proxy
    {
        private LocalPlayerVO _localPlayer;
        private JObject _moveTransformObject;
        private JObject _fireTransformObject;
        public BattleProxy(string proxyName, object data = null) : base(proxyName, data)
        {
            _moveTransformObject = new JObject
            {
                    { "Command", NotifyConsts.BattleNotification.UpLoadTransformState },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", PlayerManager.Instance.LocalPlayer.UserID },
                            { "Token", PlayerManager.Instance.LocalPlayer.Token },
                            { "MoveInstruct",new JArray(0,0,0,0,0,0,0,0,0,0,0,0,0)}
                        }
                    }
            };
            _fireTransformObject = new JObject
            {
                    { "Command", NotifyConsts.BattleNotification.UpLoadFireState },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", PlayerManager.Instance.LocalPlayer.UserID },
                            { "Token", PlayerManager.Instance.LocalPlayer.Token },
                            { "FireInstruct",new JArray(0,0,0,0,0,0,0,0,0,0)}
                        }
                    }
            };
        }

        public void UpLoadTransformState(Vector3 vehiclePosition, Quaternion vehicleRotation, Quaternion turretRotation, float moveInputValue, float turnInputValue)
        {
            _moveTransformObject["Paras"]["MoveInstruct"][0] = vehiclePosition.x;
            _moveTransformObject["Paras"]["MoveInstruct"][1] = vehiclePosition.y;
            _moveTransformObject["Paras"]["MoveInstruct"][2] = vehiclePosition.z;

            _moveTransformObject["Paras"]["MoveInstruct"][3] = vehicleRotation.x;
            _moveTransformObject["Paras"]["MoveInstruct"][4] = vehicleRotation.y;
            _moveTransformObject["Paras"]["MoveInstruct"][5] = vehicleRotation.z;
            _moveTransformObject["Paras"]["MoveInstruct"][6] = vehicleRotation.w;

            _moveTransformObject["Paras"]["MoveInstruct"][7] = turretRotation.x;
            _moveTransformObject["Paras"]["MoveInstruct"][8] = turretRotation.y;
            _moveTransformObject["Paras"]["MoveInstruct"][9] = turretRotation.z;
            _moveTransformObject["Paras"]["MoveInstruct"][10] = turretRotation.w;

            _moveTransformObject["Paras"]["MoveInstruct"][11] = moveInputValue;
            _moveTransformObject["Paras"]["MoveInstruct"][12] = turnInputValue;

            _ = NetworkService.Instance.SendCommandAsync(_moveTransformObject.ToString(Formatting.None));
        }
        public void UpdateTransformState(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string playerId = (string)jsonData.SelectToken("Paras.UserId");
            float vehiclePositionX = (float)jsonData.SelectToken("Paras.MoveInstruct[0]");
            float vehiclePositionY = (float)jsonData.SelectToken("Paras.MoveInstruct[1]");
            float vehiclePositionZ = (float)jsonData.SelectToken("Paras.MoveInstruct[2]");
            float vehicleRotationX = (float)jsonData.SelectToken("Paras.MoveInstruct[3]");
            float vehicleRotationY = (float)jsonData.SelectToken("Paras.MoveInstruct[4]");
            float vehicleRotationZ = (float)jsonData.SelectToken("Paras.MoveInstruct[5]");
            float vehicleRotationW = (float)jsonData.SelectToken("Paras.MoveInstruct[6]");
            float turretRotationX = (float)jsonData.SelectToken("Paras.MoveInstruct[7]");
            float turretRotationY = (float)jsonData.SelectToken("Paras.MoveInstruct[8]");
            float turretRotationZ = (float)jsonData.SelectToken("Paras.MoveInstruct[9]");
            float turretRotationW = (float)jsonData.SelectToken("Paras.MoveInstruct[10]");
            float moveInputValue = (float)jsonData.SelectToken("Paras.MoveInstruct[11]");
            float turnInputValue = (float)jsonData.SelectToken("Paras.MoveInstruct[12]");

            var player = PlayerManager.Instance.GetPlayer(playerId);
            if (player != null)
            {
                player.CurVehicle.VehiclePosition = new Vector3(vehiclePositionX, vehiclePositionY, vehiclePositionZ);
                player.CurVehicle.VehicleRotation = new Quaternion(vehicleRotationX, vehicleRotationY, vehicleRotationZ, vehicleRotationW);
                player.CurVehicle.TurretRotation = new Quaternion(turretRotationX, turretRotationY, turretRotationZ, turretRotationW);
                player.CurVehicle.MoveInputValue = moveInputValue;
                player.CurVehicle.TurnInputValue = turnInputValue;

                SendNotification(NotifyConsts.BattleNotification.UpdateTransformState, player, null);
            }
        }

        public void UpLoadFireState(Vector3 fireTransformPosition, Quaternion fireTransformRotation, Vector3 fireForce)
        {
            _fireTransformObject["Paras"]["FireInstruct"][0] = fireTransformPosition.x;
            _fireTransformObject["Paras"]["FireInstruct"][1] = fireTransformPosition.y;
            _fireTransformObject["Paras"]["FireInstruct"][2] = fireTransformPosition.z;

            _fireTransformObject["Paras"]["FireInstruct"][3] = fireTransformRotation.x;
            _fireTransformObject["Paras"]["FireInstruct"][4] = fireTransformRotation.y;
            _fireTransformObject["Paras"]["FireInstruct"][5] = fireTransformRotation.z;
            _fireTransformObject["Paras"]["FireInstruct"][6] = fireTransformRotation.w;

            _fireTransformObject["Paras"]["FireInstruct"][7] = fireForce.x;
            _fireTransformObject["Paras"]["FireInstruct"][8] = fireForce.y;
            _fireTransformObject["Paras"]["FireInstruct"][9] = fireForce.z;

            _ = NetworkService.Instance.SendCommandAsync(_fireTransformObject.ToString(Formatting.None));
        }
        public void UpdateFireState(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
            string playerId = (string)jsonData.SelectToken("Paras.UserId");
            float firePositionX = (float)jsonData.SelectToken("Paras.FireInstruct[0]");
            float firePositionY = (float)jsonData.SelectToken("Paras.FireInstruct[1]");
            float firePositionZ = (float)jsonData.SelectToken("Paras.FireInstruct[2]");
            float fireRotationX = (float)jsonData.SelectToken("Paras.FireInstruct[3]");
            float fireRotationY = (float)jsonData.SelectToken("Paras.FireInstruct[4]");
            float fireRotationZ = (float)jsonData.SelectToken("Paras.FireInstruct[5]");
            float fireRotationW = (float)jsonData.SelectToken("Paras.FireInstruct[6]");
            float fireForceX = (float)jsonData.SelectToken("Paras.FireInstruct[7]");
            float fireForceY = (float)jsonData.SelectToken("Paras.FireInstruct[8]");
            float fireForceZ = (float)jsonData.SelectToken("Paras.FireInstruct[9]");

            var player = PlayerManager.Instance.GetPlayer(playerId);
            if (player != null)
            {
                player.CurVehicle.FireTransformPosition = new Vector3(firePositionX, firePositionY, firePositionZ);
                player.CurVehicle.FireTransformRotation = new Quaternion(fireRotationX, fireRotationY, fireRotationZ, fireRotationW);
                player.CurVehicle.FireForce = new Vector3(fireForceX, fireForceY, fireForceZ);

                SendNotification(NotifyConsts.BattleNotification.UpdateFireState, player, null);
            }
        }

        public void UpLoadHealthState(int health)
        {
            JObject o = new JObject
                {
                    { "Command", NotifyConsts.BattleNotification.UpLoadHealthState },
                    {
                        "Paras", new JObject
                        {
                            { "UserId", _localPlayer.UserID },
                            { "Token", _localPlayer.Token },
                            { "Health",health }
                        }
                    }
                };
            _ = NetworkService.Instance.SendCommandAsync(o.ToString(Formatting.None));
        }
        public void UpdateHealthState(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
            string playerId = (string)jsonData.SelectToken("Paras.PlayerId");
            int health = (int)jsonData.SelectToken("Paras.Health");

            var player = PlayerManager.Instance.GetPlayer(playerId);
            player.Health = health;
            if (player.Health < 0)
            {
                PlayerManager.Instance.RemoveNetPlayer(playerId);
            }
        }

        public void PlayerExit(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            string info = (string)jsonData.SelectToken("Paras.Info");
            string exitPlayerId = (string)jsonData.SelectToken("Paras.ExitPlayerId");
            SendNotification(NotifyConsts.BattleNotification.PlayerExit, Tuple.Create(info, exitPlayerId), null);
            PlayerManager.Instance.RemoveNetPlayer(exitPlayerId);
        }

        public void EndGame(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
            int rank = (int)jsonData.SelectToken("Paras.Rank");
            int experence = (int)jsonData.SelectToken("Paras.Experence");
            int money = (int)jsonData.SelectToken("Paras.Money");
            int totalExperence = (int)jsonData.SelectToken("Paras.TotalExperence");
            int totalMoney = (int)jsonData.SelectToken("Paras.TotalMoney");
            PlayerManager.Instance.LocalPlayer.Experience = totalExperence;
            PlayerManager.Instance.LocalPlayer.Money = totalMoney;

            SendNotification(NotifyConsts.BattleNotification.EndGame, Tuple.Create(rank, experence, money), null);
            PlayerManager.Instance.RemoveAllNetPlayer();
        }

        public override void OnRegister()
        {
            _localPlayer = PlayerManager.Instance.LocalPlayer;

            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.BattleNotification.UpdateTransformState, UpdateTransformState);
            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.BattleNotification.UpdateFireState, UpdateFireState);
            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.BattleNotification.UpdateHealthState, UpdateHealthState);
            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.BattleNotification.EndGame, EndGame);

            CommandDispatcher.Instance.CommandDict.Add(NotifyConsts.RoomNotification.ExitRoomResult, PlayerExit);
        }

        public override void OnRemove()
        {
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.BattleNotification.UpdateTransformState);
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.BattleNotification.UpdateFireState);
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.BattleNotification.UpdateHealthState);
            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.BattleNotification.EndGame);

            CommandDispatcher.Instance.CommandDict.Remove(NotifyConsts.RoomNotification.ExitRoomResult);
        }
    }
}
