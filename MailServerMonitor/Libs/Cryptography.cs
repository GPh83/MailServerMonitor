using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;

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

    }
}

