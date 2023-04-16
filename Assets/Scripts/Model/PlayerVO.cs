using GameClient.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameClient.Model
{
    public class PlayerVO
    {
        public PlayerVO(string userID, string userName, int level, VehicleVO curVehicle)
        {
            UserID = userID ?? throw new ArgumentNullException(nameof(userID));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Level = level;
            CurVehicle = curVehicle ?? throw new ArgumentNullException(nameof(curVehicle));
        }

        public readonly string UserID;
        public readonly string UserName;

        public int Level { get; set; }
        public bool PrepareState { get; set; }
        public int Health
        {
            get => _health;
            set
            {
                if (_health != value)
                {
                    _health = _health < 0 ? 0 : _health > CurVehicle.MaxHealth ? CurVehicle.MaxHealth : value;
                    AppFacade.Instance.SendNotification(NotifyConsts.BattleNotification.UpdateHealthState, this, null);
                }
            }
        }
        private int _health;
        public VehicleVO CurVehicle { get; set; }
        public VehicleController VehicleController { get; set; }
    }
}
