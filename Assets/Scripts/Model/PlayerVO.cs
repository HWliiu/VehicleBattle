using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Model
{
    public abstract class PlayerVO
    {
        protected PlayerVO(string userID, string userName, int level, VehicleVO curVehicle)
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
        public Team Team { get; set; }
        public VehicleVO CurVehicle { get; set; }
    }
    public enum Team
    {
        None,
        Red,
        Blue
    }
}
