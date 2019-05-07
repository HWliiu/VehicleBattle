using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.Model
{
    class PlayerManager
    {
        private PlayerManager()
        {
        }
        private static readonly Lazy<PlayerManager> _instance = new Lazy<PlayerManager>(() => new PlayerManager());
        public static PlayerManager Instance => _instance.Value;

        public LocalPlayerVO LocalPlayer { get; set; }

        public Dictionary<string, NetPlayerVO> _netPlayerDict;

        public void InitLocalPlayer(string userID, string userName, string token, int level, int experience, int money, string registerTime, string loginTime, VehicleVO curVehicle, List<VehicleVO> vehicleList)
        {
            LocalPlayer = new LocalPlayerVO(userID, userName, token, level, experience, money, registerTime, loginTime, curVehicle, vehicleList);
        }

        public void AddNetPlayer(string userID, string userName, int level, VehicleVO curVehicle) => _netPlayerDict.Add(userID, new NetPlayerVO(userID, userName, level, curVehicle));
        public void RemoveNetPlayer(string userId) => _netPlayerDict.Remove(userId);
        public NetPlayerVO GetNetPlayer(string userId) => _netPlayerDict.TryGetValue(userId, out NetPlayerVO netPlayerVO) ? netPlayerVO : null;
    }
}