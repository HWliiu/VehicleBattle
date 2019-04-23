using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.Common
{
    public class VehicleItem : MonoBehaviour
    {
        private string vehicleID;
        private string vehicleName;
        private string vehicleType;
        private float attack;
        private float motility;
        private float defend;
        private int maxHealth;
        private int price;
        private string intro;

        public string VehicleID { get => vehicleID; set => vehicleID = value; }
        public string VehicleName
        {
            get => vehicleName;
            set
            {
                vehicleName = value;
                (gameObject.transform.Find("VehicleNameText")?.GetComponent<Text>()).text = vehicleName;
            }
        }
        public string VehicleType { get => vehicleType; set => vehicleType = value; }
        public float Attack
        {
            get => attack;
            set
            {
                attack = value;
                (gameObject.transform.Find("AttackText")?.GetComponent<Text>()).text = attack.ToString("F1");
            }
        }
        public float Motility
        {
            get => motility;
            set
            {
                motility = value;
                (gameObject.transform.Find("MotilityText")?.GetComponent<Text>()).text = motility.ToString("F1");
            }
        }
        public float Defend
        {
            get => defend;
            set
            {
                defend = value;
                (gameObject.transform.Find("DefendText")?.GetComponent<Text>()).text = defend.ToString("F1");
            }
        }
        public int MaxHealth
        {
            get => maxHealth;
            set
            {
                maxHealth = value;
                (gameObject.transform.Find("MaxHealthText")?.GetComponent<Text>()).text = maxHealth.ToString();
            }
        }
        public int Price
        {
            get => price;
            set
            {
                price = value;
                (gameObject.transform.Find("PriceText")?.GetComponent<Text>()).text = price.ToString();
            }
        }
        public string Intro { get => intro; set => intro = value; }
    }
}
