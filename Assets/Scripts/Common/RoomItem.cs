using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.Common
{
    public class RoomItem : MonoBehaviour
    {
        private string _roomId;
        private string _roomName;
        private string _ownerId;
        private string _ownerName;
        private string _roomMode;
        private string _roomMap;
        private string _playerNum;

        public string RoomId
        {
            get => _roomId;
            set
            {
                _roomId = value;
                gameObject.transform.Find("RoomIdText").GetComponent<Text>().text = _roomId;
            }
        }
        public string RoomName
        {
            get => _roomName;
            set
            {
                _roomName = value;
                gameObject.transform.Find("RoomNameText").GetComponent<Text>().text = _roomName;
            }
        }
        public string OwnerId { get => _ownerId; set => _ownerId = value; }
        public string OwnerName
        {
            get => _ownerName;
            set
            {
                _ownerName = value;
                gameObject.transform.Find("OwnerNameText").GetComponent<Text>().text = _ownerName;
            }
        }
        public string RoomMode { get => _roomMode; set => _roomMode = value; }
        public string RoomMap { get => _roomMap; set => _roomMap = value; }
        public string PlayerNum
        {
            get => _playerNum;
            set
            {
                _playerNum = value;
                gameObject.transform.Find("PlayerNumText").GetComponent<Text>().text = $"{_playerNum}/8";
            }
        }
    }
}
