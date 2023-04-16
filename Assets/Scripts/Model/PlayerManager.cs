using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameClient.Model
{
    class PlayerManager
    {
        private PlayerManager()
        {
            _playerDict = new Dictionary<string, PlayerVO>();
        }
        private static readonly Lazy<PlayerManager> _instance = new Lazy<PlayerManager>(() => new PlayerManager());
        public static PlayerManager Instance => _instance.Value;

        public LocalPlayerVO LocalPlayer { get => _localPlayer; }
        public PlayerVO RoomOwner { get; set; }

        private LocalPlayerVO _localPlayer;
        private Dictionary<string, PlayerVO> _playerDict;

        public void InitLocalPlayer(string userID, string userName, string token, int level, int experience, int money, string registerTime, string loginTime, VehicleVO curVehicle, List<VehicleVO> vehicleList)
        {
            _localPlayer = new LocalPlayerVO(userID, userName, token, level, experience, money, registerTime, loginTime, curVehicle, vehicleList);
            _playerDict.Add(_localPlayer.UserID, _localPlayer);
        }

        public void AddNetPlayer(PlayerVO player)
        {
            if (player.UserID != _localPlayer.UserID)
            {
                _playerDict.Add(player.UserID, player);
            }
        }

        public void RemoveNetPlayer(string userId)
        {
            if (userId != _localPlayer.UserID)
            {
                _playerDict.Remove(userId);
            }
        }

        public void RemoveAllNetPlayer()
        {
            _playerDict.Clear();
            _playerDict.Add(_localPlayer.UserID, _localPlayer);
        }
        public void RemoveAllPlayer()
        {
            _playerDict.Clear();
            _localPlayer = null;
        }

        public PlayerVO GetPlayer(string userId) => _playerDict.TryGetValue(userId, out PlayerVO playerVO) ? playerVO : null;
        public string[] PlayerOrder;
    }
}