using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.Common
{
    public class VehicleItem : MonoBehaviour
    {
        private string _vehicleID;
        private string _vehicleName;
        private string _vehicleType;
        private float _attack;
        private float _motility;
        private float _defend;
        private int _maxHealth;
        private int _price;
        private string _intro;

        public string VehicleID { get => _vehicleID; set => _vehicleID = value; }
        public string VehicleName
        {
            get => _vehicleName;
            set
            {
                _vehicleName = value;
                (gameObject.transform.Find("VehicleNameText")?.GetComponent<Text>()).text = _vehicleName;
            }
        }
        public string VehicleType { get => _vehicleType; set => _vehicleType = value; }
        public float Attack
        {
            get => _attack;
            set
            {
                _attack = value;
                (gameObject.transform.Find("AttackText")?.GetComponent<Text>()).text = _attack.ToString("F1");
            }
        }
        public float Motility
        {
            get => _motility;
            set
            {
                _motility = value;
                (gameObject.transform.Find("MotilityText")?.GetComponent<Text>()).text = _motility.ToString("F1");
            }
        }
        public float Defend
        {
            get => _defend;
            set
            {
                _defend = value;
                (gameObject.transform.Find("DefendText")?.GetComponent<Text>()).text = _defend.ToString("F1");
            }
        }
        public int MaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                (gameObject.transform.Find("MaxHealthText")?.GetComponent<Text>()).text = _maxHealth.ToString();
            }
        }
        public int Price
        {
            get => _price;
            set
            {
                _price = value;
                (gameObject.transform.Find("PriceText")?.GetComponent<Text>()).text = _price.ToString();
            }
        }
        public string Intro { get => _intro; set => _intro = value; }
    }
}
