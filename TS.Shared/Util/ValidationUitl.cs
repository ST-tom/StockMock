using System.Text.RegularExpressions;
using TS.Shared.Extension;

namespace Shared.Utils
{
    public class ValidationUitl
    {
        public static bool IsEmailAddress(string strEmailAddress)
        {
            if ( strEmailAddress.IsNullOrEmpty())
                return false;

            return Regex.IsMatch(strEmailAddress, @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");
        }
    }
}
