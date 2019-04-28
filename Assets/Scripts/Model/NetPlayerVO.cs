using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Model
{
    public class NetPlayerVO : PlayerVO
    {
        public NetPlayerVO(string userID, string userName, int level, VehicleVO curVehicle) : base(userID, userName, level, curVehicle)
        {
        }
    }
}
