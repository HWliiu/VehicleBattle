using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.Common
{
    public class PlayerItem : MonoBehaviour
    {
        private string _userId;
        private string _userName;
        private string _level;
        private bool _prepareState;
        private string _vehicleName;
        private float _attack;
        private float _motility;
        private float _defend;
        private float _maxHealth;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                gameObject.transform.Find("PlayerNameText").GetComponent<Text>().text = _userName;
            }
        }
        public string Level
        {
            get => _level;
            set
            {
                _level = value;
                gameObject.transform.Find("PlayerLevelText").GetComponent<Text>().text = "Lv" + _level;
            }
        }
        public bool PrepareState
        {
            get => _prepareState;
            set
            {
                _prepareState = value;
                gameObject.transform.Find("PrepareStateText").GetComponent<Text>().text = _prepareState ? "已准备" : "未准备";
            }
        }
        public string VehicleName { get => _vehicleName; set => _vehicleName = value; }
        public float Attack { get => _attack; set => _attack = value; }
        public float Motility { get => _motility; set => _motility = value; }
        public float Defend { get => _defend; set => _defend = value; }
        public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public string UserId { get => _userId; set => _userId = value; }
    }
}
