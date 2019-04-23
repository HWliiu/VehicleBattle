using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Model
{
    class VehicleVO
    {
        public string VehicleID { get; set; }
        public string VehicleName { get; set; }
        public VehicleType VehicleType { get; set; }
        public float Attack { get; set; }
        public float Motility { get; set; }
        public float Defend { get; set; }
        public int MaxHealth { get; set; }
        public int Price { get; set; }
        public string Intro { get; set; }
        public int Health
        {
            get => _health;
            set => _health = _health < 0 ? 0 : _health > MaxHealth ? MaxHealth : value;
        }

        // TODO: 添加位置信息,控制状态属性...

        private int _health;

        public VehicleVO(string vehicleID, string vehicleName, VehicleType vehicleType, float attack, float motility, float defend, int maxHealth, int price, string intro)
        {
            VehicleID = vehicleID ?? throw new ArgumentNullException(nameof(vehicleID));
            VehicleName = vehicleName ?? throw new ArgumentNullException(nameof(vehicleName));
            VehicleType = vehicleType;
            Attack = attack;
            Motility = motility;
            Defend = defend;
            MaxHealth = maxHealth;
            Health = MaxHealth;
            Price = price;
            Intro = intro ?? throw new ArgumentNullException(nameof(intro));
        }
    }
    enum VehicleType
    {
        Tank,
        Panzer
    }
}
