using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Model
{
    public class RoomVO
    {
        public readonly string RoomID;
        public readonly string RoomName;

        public RoomVO(string roomID, string roomName, string ownerId, string ownerName, RoomMode roomMode, RoomMap roomMap, int playerNum)
        {
            RoomID = roomID ?? throw new ArgumentNullException(nameof(roomID));
            RoomName = roomName ?? throw new ArgumentNullException(nameof(roomName));
            OwnerId = ownerId ?? throw new ArgumentNullException(nameof(ownerId));
            OwnerName = ownerName ?? throw new ArgumentNullException(nameof(ownerName));
            RoomMode = roomMode;
            RoomMap = roomMap;
            PlayerNum = playerNum;
        }

        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public RoomMode RoomMode { get; set; }
        public RoomMap RoomMap { get; set; }
        public int PlayerNum { get; set; }
        public void Deconstruct(out string roomID, out string roomName, out string ownerId, out string ownerName, out string roomMode, out string roomMap, out string playerNum)
        {
            (roomID, roomName, ownerId, ownerName, roomMode, roomMap, playerNum) = (RoomID, RoomName, OwnerId, OwnerName, RoomMode.ToString(), RoomMap.ToString(), PlayerNum.ToString());
        }
        public List<PlayerVO> PlayerList { get; set; }
    }
    public enum RoomMode
    {
        SingleMode,
        TeamMode
    }
    public enum RoomMap
    {
        Random,
        Map1,
        Map2,
        Map3
    }
}
