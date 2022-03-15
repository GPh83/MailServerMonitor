using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailServerMonitor.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailServerMonitor.Libs
{
    internal class MailBoxTester
    {
        const string TestSubject = "MailServerMonitor : Test";

        internal static Histo Test(MailBoxConfig Cfg)
        {
            Histo his = new Histo();

            his.ServerName = Cfg.ServerName;
            his.eMail = Cfg.eMail;

            SmtpTest(Cfg, his);
            if (ImapTest(Cfg, his)) ImapRemoveTestMessages(Cfg, his);

            his.SendRecieveOk = his.StmpConnected && his.ImapConnected;
            return his;
        }

        private static bool SmtpTest(MailBoxConfig Cfg, Histo his)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Cfg.eMail, Cfg.eMail));
            message.To.Add(new MailboxAddress(Cfg.eMail, Cfg.eMail));
            message.Subject = TestSubject;

            message.Body = new TextPart("plain")
            {
                Text = @"Hey, it's just me !"
            };

            using (var client = new SmtpClient())
            {
                Stopwatch delay = new Stopwatch();

                try
                {
                    delay.Start();
                    client.Timeout = Cfg.TimeOutMs;
                    client.Connect(Cfg.ServerName, Cfg.SmtpPort, Cfg.SmtpSSL);
                }
                catch (Exception ex)
                {
                    his.SmtpResponseTimeMs = delay.ElapsedMilliseconds;
                    his.StmpConnected = false;
                    his.SmtpError = "Connect : " + ex.Message.Replace(Environment.NewLine, " | ");
                    return false;
                }

                try
                {
                    if (!String.IsNullOrEmpty(Cfg.Login))
                    {
                        client.Authenticate(Cfg.Login, Cfg.GetPassword());
                    }

                }
                catch (Exception ex)
                {
                    his.SmtpResponseTimeMs = delay.ElapsedMilliseconds;
                    his.StmpConnected = false;
                    his.SmtpError = "Authenticate : " + ex.Message.Replace(Environment.NewLine, " | ");
                    return false;
                }

                try
                {
                    his.StmpConnected = true;
                    client.Send(message);
                    client.Disconnect(true);
                    his.SmtpResponseTimeMs = delay.ElapsedMilliseconds;
                    return true;
                }
                catch (Exception ex)
                {
                    his.SmtpResponseTimeMs = delay.ElapsedMilliseconds;
                    his.StmpConnected = false;
                    his.SmtpError = "Send : " + ex.Message.Replace(Environment.NewLine, " | ");
                    return false;
                }
            }
        }

        private static bool ImapTest(MailBoxConfig Cfg, Histo his)
        {
            using (var client = new ImapClient())
            {
                Stopwatch delay = new Stopwatch();

                try
                {
                    delay.Start();
                    client.Timeout = Cfg.TimeOutMs;
                    client.Connect(Cfg.ServerName, Cfg.ImapPort, Cfg.ImapSSL);
                }
                catch (Exception ex)
                {
                    his.ImapResponseTimeMs = delay.ElapsedMilliseconds;
                    his.ImapConnected = false;
                    his.ImapError = "Connect : " + ex.Message.Replace(Environment.NewLine, " | ");
                    return false;
                }

                try
                {
                    if (!String.IsNullOrEmpty(Cfg.Login))
                    {
                        client.Authenticate(Cfg.Login, Cfg.GetPassword());
                    }

                }
                catch (Exception ex)
                {
                    his.ImapResponseTimeMs = delay.ElapsedMilliseconds;
                    his.ImapConnected = false;
                    his.ImapError = "Authenticate : " + ex.Message.Replace(Environment.NewLine, " | ");
                    return false;
                }


                try
                {
                    his.ImapConnected = true;
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);
                    //Console.WriteLine("Total messages: {0}", inbox.Count);
                    client.Disconnect(true);
                    his.ImapResponseTimeMs = delay.ElapsedMilliseconds;
                    return true;
                }
                catch (Exception ex)
                {
                    his.ImapResponseTimeMs = delay.ElapsedMilliseconds;
                    his.ImapConnected = false;
                    his.ImapError = "Recieve : " + ex.Message.Replace(Environment.NewLine, " | ");
                    return false;
                }

            }
        }

        private static bool ImapRemoveTestMessages(MailBoxConfig Cfg, Histo his)
        {
            int n=0;

            using (var client = new ImapClient())
            {
                try
                {
                    client.Timeout = Cfg.TimeOutMs;
                    client.Connect(Cfg.ServerName, Cfg.ImapPort, Cfg.ImapSSL);
                    if (!String.IsNullOrEmpty(Cfg.Login)) client.Authenticate(Cfg.Login, Cfg.GetPassword());

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

                    foreach (var summary in inbox.Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope))
                    {
                        if (summary.Envelope.Subject == TestSubject)
                        {
                            inbox.Store(summary.UniqueId, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });
                            n++;
                        }
                    }
                    inbox.Expunge();

                    client.Disconnect(true);
                    his.DeletedCount = n;
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }

}

