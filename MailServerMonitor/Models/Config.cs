using System;
using System.Collections.Generic;

namespace MailServerMonitor.Models
{
    internal class Config
    {
        /// <summary>
        /// 0=One time, X=Ask every X minutes
        /// </summary>
        public int AskMin { get; set; } = 0;

        public List<MailBoxConfig> MailBoxes { get; set; } = new List<MailBoxConfig>();

        public string CSVName { get; set; } = "MailServerMonitor.csv";

        public string Key { get; set; }

    }

    internal class MailBoxConfig
    {
        public string Name { get; set; }

        public ServerConfig Imap { get; set; } = new ServerConfig { Port = 993 };

        public ServerConfig Smtp { get; set; } = new ServerConfig { Port = 465 };

    }


    internal class ServerConfig
    {
        public string Email { get; set; }

        public string Server { get; set; }

        public int TimeOutMs { get; set; } = 30000;

        public string Login { get; set; }

        public string EncryptedPassword { get; set; }

        public int Port { get; set; }

        public bool UseSSL { get; set; } = true;


        internal String GetPassword(string salt)
        {
            return Libs.Cryptography.Decrypt(EncryptedPassword, "3Ag-ZFQmR_b4kXHo" + salt);
        }

        internal void SetPassword(string uncryptedPassword, string salt)
        {
            EncryptedPassword = Libs.Cryptography.Encrypt(uncryptedPassword, "3Ag-ZFQmR_b4kXHo" + salt);
        }
    }
}
