using System.Security.Cryptography;
using System.Text;

namespace TS.Shared.Util
{
    public partial class EncryptionUtil
    {
        #region AES加密解密

        public static string ToAES(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("秘钥不能为空");

            // 确保密钥长度为32字节(256位)
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = keyBytes;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            // 生成随机的初始化向量
            aesAlg.GenerateIV();
            byte[] iv = aesAlg.IV;

            // 创建加密器
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using MemoryStream msEncrypt = new MemoryStream();

            // 先写入IV，解密时需要用到
            msEncrypt.Write(iv, 0, iv.Length);

            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using StreamWriter swEncrypt = new StreamWriter(csEncrypt);
            // 写入要加密的数据
            swEncrypt.Write(plainText);

            // 返回包含IV和加密数据的Base64字符串
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string FromAES(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("秘钥不能为空");

            // 确保密钥长度为32字节(256位)
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = keyBytes;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            // 从加密数据中读取IV(前16字节)
            byte[] iv = new byte[16];
            Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
            aesAlg.IV = iv;

            // 创建解密器
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(cipherBytes, 16, cipherBytes.Length - 16);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            // 读取解密后的数据
            return srDecrypt.ReadToEnd();
        }

        /// <summary>
        /// 生成AES加密密钥(32字节)
        /// </summary>
        /// <returns>随机生成的AES密钥</returns>
        public static string CreateAesKey()
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.KeySize = 256;
            aesAlg.GenerateKey();
            return Convert.ToBase64String(aesAlg.Key);
        }

        #endregion

        #region MD5

        public static string ToMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        public static bool CheckMD5(string input, string md5, bool throwException = false)
        {
            string inputMD5 = ToMD5(input);
            var result = inputMD5.Equals(md5, StringComparison.OrdinalIgnoreCase);

            if (!result && throwException)
                throw new ArgumentException("MD5校验失败");

            return result;
        }

        #endregion

        #region SHA256

        public static string ToSHA256(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            using SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        public static bool CheckSHA256(string input, string sha256, bool throwException = false)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            if (string.IsNullOrEmpty(sha256))
                throw new ArgumentNullException(nameof(sha256));

            string inputSHA256 = ToSHA256(input);
            var result = inputSHA256.Equals(sha256, StringComparison.OrdinalIgnoreCase);

            if (!result && throwException)
                throw new ArgumentException("SHA256校验失败");

            return result;
        }

        #endregion

        #region Base64

        public static string ToBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes);
        }

        public static string FromBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            byte[] inputBytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(inputBytes);
        }

        #endregion
    }
}
