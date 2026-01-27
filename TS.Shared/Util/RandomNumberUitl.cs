using System.Security.Cryptography;

namespace TS.Shared.Util
{
    public class RandomNumberUitl
    {
        public static string New(int length = 64)
        {
            var randomNumber = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
