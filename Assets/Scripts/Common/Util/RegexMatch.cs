using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameClient.Common
{
    static class RegexMatch
    {
        private static readonly string userNamePattern = @"^[\S+]{3,10}$";
        private static readonly string passwordPattern = @"^[0-9a-zA-z]{6,16}$";
        public static bool UserNameMatch(string username)
        {
            if (Regex.IsMatch(username, userNamePattern)) return true;
            return false;
        }
        public static bool PasswordMatch(string password)
        {
            if (Regex.IsMatch(password, passwordPattern)) return true;
            return false;
        }
    }
}
