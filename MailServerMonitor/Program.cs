using System;
using System.IO;
using System.Threading;
using MailServerMonitor.Models;

namespace MailServerMonitor
{
    class Program
    {
        static internal Models.DataManager DataService;

        static void Main(string[] args)
        {
            Console.WriteLine("MailServerMonitor V{0}\n", typeof(Program).Assembly.GetName().Version);
            DataService = new DataManager();
            DataService.Start();

            // First run
            CheckConfig();

            // Command line
            if (args.Length > 0)
            {
                int i = 0;

                switch (args[0])
                {
                    // Delete
                    case "-d":
                        if (args.Length == 2)
                        {
                            if (int.TryParse(args[1], out i))
                            {
                                if (i > 0)
                                {
                                    DataService.config.MailBoxes.RemoveAt(i - 1);
                                    DataService.SaveConfig();
                                }
                                else Console.WriteLine("Invalid argument");
                            }
                            else
                                Console.WriteLine("Invalid argument");
                        }
                        else
                            Console.WriteLine("Invalid argument");
                        break;

                    // List
                    case "-l":
                        foreach (MailBoxConfig mb in DataService.config.MailBoxes)
                        {
                            i++;
                            Console.WriteLine("{0} : {1}", i, mb.eMail);
                            Console.WriteLine("    Server : {0}", mb.ServerName);
                        }
                        Console.WriteLine("");
                        Console.WriteLine("Other settings in config.json");
                        break;

                    // Add
                    case "-a":
                        DataService.config.MailBoxes.Add(GetNewMailBox());
                        DataService.SaveConfig();
                        break;

                    // Unique
                    case "-u":
                        DataService.config.AskMin = 0;
                        CheckMailBoxes();
                        break;

                    // Help
                    default:
                        Console.WriteLine("Check email server and log result in CSV file.");
                        Console.WriteLine("Command line : MailServerMonitor -d [Num] | -l | -a | -u");
                        Console.WriteLine("No argument : Run test, one time is AskMin=0 else every X minutes");
                        Console.WriteLine("One command : ");
                        Console.WriteLine("  -l : List mailboxes");
                        Console.WriteLine("  -a : Add a mailbox");
                        Console.WriteLine("  -u : One try only ");
                        Console.WriteLine("  -d [Num] : Delete the [Num] mailbox in config");
                        Console.WriteLine("Other settings in config.json");
                        break;
                }
            }
            else
                CheckMailBoxes();
        }

        static internal void CheckConfig()
        {
            if (DataService.config.MailBoxes.Count == 0)
            {
                DataService.config.MailBoxes.Add(GetNewMailBox());
                DataService.SaveConfig();
            }
        }

        private static MailBoxConfig GetNewMailBox()
        {
            var mb = new MailBoxConfig();
            Console.Write("eMail : ");
            mb.eMail = Console.ReadLine();
            Console.Write("\nServer : ");
            mb.ServerName = Console.ReadLine();
            Console.Write("\nLogin : ");
            mb.Login = Console.ReadLine();
            Console.Write("\nPassword : ");
            mb.SetPassword(Console.ReadLine());
            Console.WriteLine("\nEdit config.json for other values.");
            return mb;
        }
        private static void CheckMailBoxes()
        {
            do
            {
                foreach (MailBoxConfig mb in DataService.config.MailBoxes)
                {
                    Console.WriteLine("Check : {0}", mb.eMail);
                    var his = Libs.MailBoxTester.Test(mb);

                    DataService.AddToCSV(his);
                    if (his.StmpConnected)
                    {
                        Console.WriteLine("   SMTP : Ok  {0}ms", his.SmtpResponseTimeMs.ToString());
                    }
                    else
                    {
                        Console.WriteLine("   SMTP : Erreur  " + his.SmtpError);
                    }
                    if (his.ImapConnected)
                    {
                        Console.WriteLine("   IMAP : Ok  {0}ms", his.ImapResponseTimeMs.ToString());
                    }
                    else
                    {
                        Console.WriteLine("   IMAP : Erreur  " + his.ImapError);
                    }
                    Thread.Sleep(DataService.config.AskMin * 60000);
                }
            } while (DataService.config.AskMin > 0);
        }
    }
}
