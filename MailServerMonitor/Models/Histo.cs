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
        
        public string Name { get; set; }

        public string SmtpEmail { get; set; }

        public string ImapEmail { get; set; }

        public string SmtpServer { get; set; }

        public string ImapServer { get; set; }

        public long SmtpResponseTimeMs;

        public long ImapResponseTimeMs;

        public bool StmpConnected;

        public bool ImapConnected;

        public string ImapError;

        public string SmtpError;

        public bool SendRecieveOk;

        public int DeletedCount;

        public string CSVLine(string sep = ";")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(sep);
            sb.Append(EventDateTime);
            sb.Append(sep);
            sb.Append(SendRecieveOk);
            sb.Append(sep);
            sb.Append(SmtpEmail);
            sb.Append(sep);
            sb.Append(SmtpServer);
            sb.Append(sep);
            sb.Append(StmpConnected);
            sb.Append(sep);
            sb.Append(SmtpResponseTimeMs);
            sb.Append(sep);
            sb.Append(SmtpError);
            sb.Append(sep);
            sb.Append(ImapEmail);
            sb.Append(sep);
            sb.Append(ImapServer);
            sb.Append(sep);
            sb.Append(ImapConnected);
            sb.Append(sep);
            sb.Append(ImapResponseTimeMs);
            sb.Append(sep);
            sb.Append(ImapError);
            sb.Append(sep);
            sb.Append(DeletedCount);
            sb.Append(sep);
            sb.AppendLine();
            return sb.ToString();
        }

        public string CSVHeader(string sep = ";")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Name");
            sb.Append(sep);
            sb.Append("EventDateTime");
            sb.Append(sep);
            sb.Append("SendRecieveOk");
            sb.Append(sep);
            sb.Append("SmtpEmail");
            sb.Append(sep);
            sb.Append("SmtpServer");
            sb.Append(sep);
            sb.Append("StmpConnected");
            sb.Append(sep);
            sb.Append("SmtpResponseTimeMs");
            sb.Append(sep);
            sb.Append("SmtpError");
            sb.Append(sep);
            sb.Append("ImapEmail");
            sb.Append(sep);
            sb.Append("ImapServer");
            sb.Append(sep);
            sb.Append("ImapConnected");
            sb.Append(sep);
            sb.Append("ImapResponseTimeMs");
            sb.Append(sep);
            sb.Append("ImapError");
            sb.Append(sep);
            sb.Append("DeletedCount");
            sb.Append(sep);
            sb.AppendLine();
            return sb.ToString();
        }
    }

}
