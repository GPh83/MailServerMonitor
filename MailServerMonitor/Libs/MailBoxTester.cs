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

        internal static Histo Test(MailBoxConfig Cfg, string salt)
        {
            Histo his = new Histo();
            his.Name = Cfg.Name;

            SmtpTest(Cfg, his, salt);
            if (ImapTest(Cfg, his, salt))
            {
                System.Threading.Thread.Sleep(500);
                ImapRemoveTestMessages(Cfg, his, salt);
            }

            his.SendRecieveOk = his.StmpConnected && his.ImapConnected;
            return his;
        }

        private static bool SmtpTest(MailBoxConfig Cfg, Histo his, string salt)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Cfg.Smtp.Email, Cfg.Smtp.Email));
            message.To.Add(new MailboxAddress(Cfg.Imap.Email, Cfg.Imap.Email));
            message.Subject = TestSubject;

            his.SmtpServer = Cfg.Smtp.Server;
            his.SmtpEmail = Cfg.Smtp.Email;

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
                    client.Timeout = Cfg.Smtp.TimeOutMs;
                    client.Connect(Cfg.Smtp.Server, Cfg.Smtp.Port, Cfg.Smtp.UseSSL);
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
                    if (!String.IsNullOrEmpty(Cfg.Smtp.Login))
                    {
                        client.Authenticate(Cfg.Smtp.Login, Cfg.Smtp.GetPassword(salt));
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

        private static bool ImapTest(MailBoxConfig Cfg, Histo his, string salt)
        {
            his.ImapServer = Cfg.Imap.Server;
            his.ImapEmail = Cfg.Imap.Email;

            using (var client = new ImapClient())
            {
                Stopwatch delay = new Stopwatch();

                try
                {
                    delay.Start();
                    client.Timeout = Cfg.Imap.TimeOutMs;
                    client.Connect(Cfg.Imap.Server, Cfg.Imap.Port, Cfg.Imap.UseSSL);
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
                    if (!String.IsNullOrEmpty(Cfg.Imap.Login))
                    {
                        client.Authenticate(Cfg.Imap.Login, Cfg.Imap.GetPassword(salt));
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

        private static bool ImapRemoveTestMessages(MailBoxConfig Cfg, Histo his, string salt)
        {
            int n = 0;

            using (var client = new ImapClient())
            {
                try
                {
                    client.Timeout = Cfg.Imap.TimeOutMs;
                    client.Connect(Cfg.Imap.Server, Cfg.Imap.Port, Cfg.Imap.UseSSL);
                    if (!String.IsNullOrEmpty(Cfg.Imap.Login)) client.Authenticate(Cfg.Imap.Login, Cfg.Imap.GetPassword(salt));

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

