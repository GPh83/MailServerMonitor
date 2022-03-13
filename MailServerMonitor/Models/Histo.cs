using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailServerMonitor.Models
{
    internal class Histo
    {
        public DateTime EventDateTime { get; set; } = DateTime.Now;

        public string eMail { get; set; }

        public string ServerName { get; set; }

        public long SmtpResponseTimeMs;

        public long ImapResponseTimeMs;

        public bool StmpConnected;

        public bool ImapConnected;

        public string ImapError;

        public string SmtpError;

        public bool SendRecieveOk;

        public string CSVLine(string sep = ";")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ServerName);
            sb.Append(sep);
            sb.Append(eMail);
            sb.Append(sep);
            sb.Append(EventDateTime);
            sb.Append(sep);
            sb.Append(SendRecieveOk);
            sb.Append(sep);
            sb.Append(StmpConnected);
            sb.Append(sep);
            sb.Append(SmtpResponseTimeMs);
            sb.Append(sep);
            sb.Append(SmtpError);
            sb.Append(sep);
            sb.Append(ImapConnected);
            sb.Append(sep);
            sb.Append(ImapResponseTimeMs);
            sb.Append(sep);
            sb.Append(ImapError);
            sb.AppendLine(sep);
            return sb.ToString();
        }

        public string CSVHeader(string sep = ";")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ServerName");
            sb.Append(sep);
            sb.Append("eMail");
            sb.Append(sep);
            sb.Append("EventDateTime");
            sb.Append(sep);
            sb.Append("SendRecieveOk");
            sb.Append(sep);
            sb.Append("StmpConnected");
            sb.Append(sep);
            sb.Append("SmtpResponseTimeMs");
            sb.Append(sep);
            sb.Append("SmtpError");
            sb.Append(sep);
            sb.Append("ImapConnected");
            sb.Append(sep);
            sb.Append("ImapResponseTimeMs");
            sb.Append(sep);
            sb.Append("ImapError");
            sb.AppendLine(sep);
            return sb.ToString();
        }
    }

}
