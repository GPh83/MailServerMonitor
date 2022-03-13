using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }

    internal class MailBoxConfig
    {
        public string eMail { get; set; }

        public int TimeOutMs { get; set; } = 30000;

        public string ServerName { get; set; }
        
        public string Login { get; set; }
        
        public string EncryptedPassword { get; set; }

        public int ImapPort { get; set; } = 993;

        public bool ImapSSL { get; set; } = true;

        public int SmtpPort { get; set; } = 465;

        public bool SmtpSSL { get; set; } = true;


        internal String GetPassword()
        {
            return Libs.Cryptography.Decrypt(EncryptedPassword, "dg5d4g699dfsf");
        }

        internal void SetPassword(String uncryptedPassword)
        {
            EncryptedPassword = Libs.Cryptography.Encrypt(uncryptedPassword, "dg5d4g699dfsf");
        }
    }


}
