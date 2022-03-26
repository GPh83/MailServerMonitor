using System;
using System.Security.Cryptography;
using System.Text;

namespace MailServerMonitor.Libs
{
    /// <summary>
    /// String 3DES encryption
    /// </summary>
    public static class Cryptography
    {

        public static string Encrypt(string stringToEncrypt, string key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();
            DES.Key = Md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(key));

            DES.Mode = System.Security.Cryptography.CipherMode.ECB;
            byte[] Buffer = System.Text.Encoding.Unicode.GetBytes(stringToEncrypt);
            return Convert.ToBase64String(DES.CreateEncryptor().TransformFinalBlock(Buffer, 0, Buffer.Length));
        }


        public static string Decrypt(string encryptedString, string key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            try
            {
                MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();
                DES.Key = Md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(key));

                DES.Mode = System.Security.Cryptography.CipherMode.ECB;
                byte[] Buffer = Convert.FromBase64String(encryptedString);
                return System.Text.Encoding.Unicode.GetString(DES.CreateDecryptor().TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch
            {
                return null;
            }
        }


        // From https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
        public static class KeyGenerator
        {
            internal static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

            public static string GetUniqueKey(int size)
            {
                byte[] data = new byte[4 * size];
                using (var crypto = RandomNumberGenerator.Create())
                {
                    crypto.GetBytes(data);
                }
                StringBuilder result = new StringBuilder(size);
                for (int i = 0; i < size; i++)
                {
                    var rnd = BitConverter.ToUInt32(data, i * 4);
                    var idx = rnd % chars.Length;

                    result.Append(chars[idx]);
                }

                return result.ToString();
            }

            public static string GetUniqueKeyOriginal_BIASED(int size)
            {
                char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                byte[] data = new byte[size];
                using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                {
                    crypto.GetBytes(data);
                }
                StringBuilder result = new StringBuilder(size);
                foreach (byte b in data)
                {
                    result.Append(chars[b % (chars.Length)]);
                }
                return result.ToString();
            }
        }
    }
}

