using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.Model
{
    public class UserVO
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public int Experience { get; set; }
        public int Money { get; set; }
        public int Level { get; set; }
        private UserVO()
        {
        }
        private static readonly Lazy<UserVO> _instance = new Lazy<UserVO>(() => new UserVO());
        public static UserVO Instance => _instance.Value;
    }
}
