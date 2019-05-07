using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.Common
{
    public class PlayerItem : MonoBehaviour
    {
        private string _userName;
        private string _level;
        private string _prepareState;
        private string _vehicleName;
        private string _attack;
        private string _motility;
        private string _defend;
        private string _maxHealth;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                (gameObject.transform.Find("PlayerNameText")?.GetComponent<Text>()).text = _userName;
            }
        }
        public string Level
        {
            get => _level;
            set
            {
                _level = value;
                (gameObject.transform.Find("PlayerLevelText")?.GetComponent<Text>()).text = _level;
            }
        }
        public string PrepareState
        {
            get => _prepareState;
            set
            {
                _prepareState = value;
                (gameObject.transform.Find("PrepareStateText")?.GetComponent<Text>()).text = _prepareState;
            }
        }
        public string VehicleName { get => _vehicleName; set => _vehicleName = value; }
        public string Attack { get => _attack; set => _attack = value; }
        public string Motility { get => _motility; set => _motility = value; }
        public string Defend { get => _defend; set => _defend = value; }
        public string MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    }
}
