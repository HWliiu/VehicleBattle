using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Model
{
    public class VehicleVO
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
        public void Deconstruct(out string id, out string name, out string type, out float attack, out float motility, out float defend, out int maxHealth, out int price, out string intro)
        {
            (id, name, type, attack, motility, defend, maxHealth, price, intro) = (VehicleID, VehicleName, VehicleType.ToString(), Attack, Motility, Defend, MaxHealth, Price, Intro);
        }
    }
    public enum VehicleType
    {
        Tank,
        Panzer
    }
}
