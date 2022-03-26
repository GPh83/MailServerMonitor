using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MailServerMonitor.Models;


/// <summary>
/// MailServerMonitor
/// </summary>
/// <remarks>
/// # Licence 
/// MailServerMonitor Copyright(C) 2022 Philippe GRAILLE.
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
/// Unless required by applicable law or agreed to in writing, **software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND**, either express or implied.
/// See the License for the specific language governing permissions and limitations under the License.
/// </remarks>
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
                            Console.WriteLine("{0} : {1}", i, mb.Name);
                            Console.WriteLine("    SMTP = Email : {0}, Server : {1}:{2}", mb.Smtp.Email, mb.Smtp.Server, mb.Smtp.Port);
                            Console.WriteLine("    IMAP = Email : {0}, Server : {1}:{2}", mb.Imap.Email, mb.Imap.Server, mb.Imap.Port);

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
                        Console.WriteLine("");
                        Console.WriteLine("Licensed under the Apache License, Version 2.0. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0");
                        Console.WriteLine("Software distributed under the License is distributed on an 'AS IS' BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND");

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
                DataService.config.Key = Libs.Cryptography.KeyGenerator.GetUniqueKeyOriginal_BIASED(12);
                DataService.config.MailBoxes.Add(GetNewMailBox());
                DataService.SaveConfig();
            }
        }

        private static MailBoxConfig GetNewMailBox()
        {
            var mb = new MailBoxConfig();
            Console.Write("Name : ");
            mb.Name = Console.ReadLine();
            Console.Write("\nSMTP : \n");
            Console.Write("  Email : ");
            mb.Smtp.Email = Console.ReadLine();
            Console.Write("\n  Server : ");
            mb.Smtp.Server = Console.ReadLine();
            Console.Write("\n  Login : ");
            mb.Smtp.Login = Console.ReadLine();
            Console.Write("\n  Password : ");
            mb.Smtp.SetPassword(Console.ReadLine(), DataService.config.Key);
            Console.Write("\nIMAP : \n");
            Console.Write("  Email : ");
            mb.Imap.Email = Console.ReadLine();
            Console.Write("\n  Server : ");
            mb.Imap.Server = Console.ReadLine();
            Console.Write("\n  Login : ");
            mb.Imap.Login = Console.ReadLine();
            Console.Write("\n  Password : ");
            mb.Imap.SetPassword(Console.ReadLine(), DataService.config.Key);
            Console.WriteLine("\nEdit config.json for other values.");
            return mb;
        }


        private static void CheckMailBoxes()
        {
            Stopwatch delay = new Stopwatch();

            delay.Start();
            do
            {
                foreach (MailBoxConfig mb in DataService.config.MailBoxes)
                {
                    Console.WriteLine("{0}   Check : {1}", DateTime.Now.ToString("g"),mb.Name);
                    var his = Libs.MailBoxTester.Test(mb, DataService.config.Key);

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
                }

                // Sleep
                while((delay.ElapsedMilliseconds/60000) < DataService.config.AskMin) Thread.Sleep(1000);
                delay.Restart();
            } while (DataService.config.AskMin > 0);
        }
    }
}
