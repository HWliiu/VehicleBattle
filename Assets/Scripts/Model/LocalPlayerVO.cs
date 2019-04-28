using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Model
{
    public class LocalPlayerVO : PlayerVO
    {
        public readonly string Token;
        private int money;
        private int experience;

        public int Experience { get => experience; set => experience = value < 0 ? 0 : value; }
        public int Money { get => money; set => money = value < 0 ? 0 : value; }
        public string RegisterTime { get; set; }
        public string LoginTime { get; set; }
        public List<VehicleVO> VehicleList { get; set; }

        public LocalPlayerVO(string userID, string userName, string token, int level, int experience, int money, string registerTime, string loginTime, VehicleVO curVehicle, List<VehicleVO> vehicleList) : base(userID, userName, level, curVehicle)
        {
            (Token, Experience, Money, RegisterTime, LoginTime, VehicleList) = (token ?? throw new ArgumentNullException(nameof(token)), experience, money, registerTime, loginTime, vehicleList ?? throw new ArgumentNullException(nameof(vehicleList)));
        }
        public VehicleVO FindVehicle(string vehicleId) => (from r in VehicleList where r.VehicleID == vehicleId select r).First();
        public void Deconstruct(out string token, out string userID, out string userName) => (token, userID, userName) = (Token, UserID, UserName);
    }
}
