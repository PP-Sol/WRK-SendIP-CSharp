using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace SendIP
{
    class Program
    {

        // Function writes file with IP address to remote file on ftp server specified by settings
        // -1 File was not uploaded
        //  1 File was successfully uploaded
        static int UploadFile(Settings AppPar)
        {
            int vrat = -1;

            try
            {
                string hostFtp  = AppPar.getParByName("Application.Ftp.Host");
                string loginFtp = AppPar.getParByName("Application.Ftp.Login");
                string passFtp  = AppPar.getParByName("Application.Ftp.Password");
                string fileFtp  = AppPar.getParByName("Application.Ftp.IpStorage");
                string fileIp   = AppPar.getParByName("Application.File.IpStorage");

                Ftp ftpClient = new Ftp(@hostFtp, loginFtp, passFtp);
                ftpClient.upload(fileFtp, @fileIp);
                vrat = 1;
                ftpClient = null;
            }
            catch (Exception e) 
            {
            
            }

            return vrat;
        }

        // Function sends email containing current IP address to email addresses, which are specified in file emails
        // -1 Function was not successful
        //  0 Email cannot be sent
        //  1 Emails were successfully sent
        static int SendIPmail(Settings AppPar, string ip)
        {
            int vrat = -1;
            int res;
            int i;

            try
            {
                // Reading email
                string emailFile = AppPar.getParByName("Application.File.Email");
                string email = TextFile.readFileStr(emailFile);

                // Reading list of email addresses
                string emListFilePath = AppPar.getParByName("Application.File.MailList");
                ArrayList emListFile = TextFile.readFile(emListFilePath);

                // Actual date and time
                DateTime dateAndTime = DateTime.Now;

                // Replace codes in email
                email = email.Replace("xx.xx.xx.xx", ip);
                email = email.Replace("DxxxD", dateAndTime.ToString());

                // Addresses
                string[] addresses = new string[emListFile.Count];
                for (i = 0; i < addresses.Length; i++) addresses[i] = emListFile[i].ToString();

                string mailServer = AppPar.getParByName("Application.Mail.Server");
                int mailPort = Convert.ToInt16(AppPar.getParByName("Application.Mail.Port"));
                string mailLogin = AppPar.getParByName("Application.Mail.Login");
                string mailPass = AppPar.getParByName("Application.Mail.Password");
                string mailFrom = "sendIp@valcon-int.com";
                string mailSub = "Valcon Virtual Server IP Address";
                //string host, int port, string login, string pass

                Mail mailAcc = new Mail(mailServer, mailPort, mailLogin, mailPass);

                res = mailAcc.SendMsg(mailFrom, addresses, mailSub, email);

                if (res == 1)
                {
                    vrat = 1;
                }
                else
                {
                    vrat = 0;
                }
            }
            catch (Exception e) { }

            return vrat;
        }

        // Function gets IP addreses from 3 specified servers and decides correct IP address ( 2oo3 )
        // "null" There was a problem with connection
        // "none" No address acquired or acquired addresses are not matching 
        public static string GetIP(Settings AppSett, ref MsgBuff appMsgBuf)
        {
            string vrat = "None";

            try
            {
                // Getting servers for IP address
                string server1 = AppSett.getParByName("Application.IP.Server1");
                string server2 = AppSett.getParByName("Application.IP.Server2");
                string server3 = AppSett.getParByName("Application.IP.Server3");

                // Getting raw IP addresses
                string ip1_r = IPCollect.GetPublicIP(server1);
                string ip2_r = IPCollect.GetPublicIP2(server2);
                string ip3_r = IPCollect.GetPublicIP2(server2);

                // Trim all adresses
                string patt1 = @"((2[0-5][0-5])|(0?[0-9][0-9])|((0{2})?[0-9])|(1[0-9][0-9]))\.((2[0-5][0-5])|(0?[0-9][0-9])|((0{2})?[0-9])|(1[0-9][0-9]))\.((2[0-5][0-5])|(0?[0-9][0-9])|((0{2})?[0-9])|(1[0-9][0-9]))\.((2[0-5][0-5])|(0?[0-9][0-9])|((0{2})?[0-9])|(1[0-9][0-9]))";
                string ip1 = Settings.getTextRgx(@ip1_r, patt1);
                string ip2 = Settings.getTextRgx(@ip2_r, patt1);
                string ip3 = Settings.getTextRgx(@ip3_r, patt1);

                // Deciding correct IP address
                if (((ip1 != null) && (ip2 != null)) || ((ip1 != null) && (ip3 != null)) || ((ip2 != null) && (ip3 != null)))
                {
                    if (ip1 == ip2)
                    {
                        vrat = ip1;
                    }
                    if (ip1 == ip3)
                    {
                        vrat = ip1;
                    }
                    if (ip2 == ip3)
                    {
                        vrat = ip2;
                    }

                    if (vrat != "None")
                    {
                        appMsgBuf.Add("Info", "New IP address acquired successfully");
                    }
                }

            }
            catch (Exception e)
            {
                vrat = null;
                appMsgBuf.Add("Error", "New IP address cannot be acquired!");
            }

            return vrat;
        }

        // Function gets old IP address from local storage 
        // "000.000.000.000" There was a problem with file reading 
        public static string GetOldIP(Settings AppSett, ref MsgBuff appMsgBuf)
        {
            string vrat = "xxx.xxx.xxx.xxx";

            try
            {
                string path = AppSett.getParByName("Application.File.IpStorage");
                string IpFile = TextFile.readFileStr(path);

                // Get old IP
                string patt1 = @"((2[0-5][0-5])|(0?[0-9][0-9])|((0{2})?[0-9])|(1[0-9][0-9]))\.((2[0-5][0-5])|(0?[0-9][0-9])|((0{2})?[0-9])|(1[0-9][0-9]))\.((2[0-5][0-5])|(0?[0-9][0-9])|((0{2})?[0-9])|(1[0-9][0-9]))\.((2[0-5][0-5])|(0?[0-9][0-9])|((0{2})?[0-9])|(1[0-9][0-9]))";
                string ip1 = Settings.getTextRgx(@IpFile, patt1);

                vrat = ip1;
                appMsgBuf.Add("Info", "Old IP address acquired successfully");

            }
            catch (Exception e)
            {
                vrat = "xxx.xxx.xxx.xxx";
                appMsgBuf.Add("Warn","Old IP address cannot be acquired!");
            }

            return vrat;
        }

        // Function writes local IP file
        // -1 Function was not successful
        //  1 File wss successfully written
        static int WriteIPLocal(Settings AppPar, string ip, DateTime TimeDate, ref MsgBuff appMsgBuf)
        {
            int vrat = -1;
            bool res;
            int i;
            
            ArrayList cont = new ArrayList();
            cont.Add("Send IP");
            cont.Add("Date and time: " + TimeDate.ToString());
            cont.Add("Current Valcon Virtual Server IP Address: "+ip);

            string path = AppPar.getParByName("Application.File.IpStorage");

            try
            {
                res = TextFile.writeFile(path, cont);

                if (res)
                {
                    vrat = 1;
                    appMsgBuf.Add("Info", "IP address written successfully in local storage");
                }
            }
            catch (Exception e) 
            {
                appMsgBuf.Add("Warn", "IP address was not written in local storage!");    
            }

            return vrat;
        }

        static void RunApp(ref long numCheck, ref long numOfBadChecks, Settings appPar,ref string ipOld, ref string ipAdd, ref MsgBuff appMsgBuf, int left, int top)
        {

            bool ftpEnab;
            bool mailEnab;

            long num = numCheck + numOfBadChecks + 1;
            appMsgBuf.Add("Info", "Beginning IP check number: " + num.ToString());
            appMsgBuf.Print(left, top);

            try
            {
                ftpEnab = Convert.ToBoolean(appPar.getParByName("Application.Ftp.Enabled"));
                mailEnab = Convert.ToBoolean(appPar.getParByName("Application.Mail.Enabled"));
            }
            catch (Exception e)
            {
                ftpEnab = false;
                mailEnab = false;
            }


            // Getting an old IP address
            ipOld = GetOldIP(appPar, ref appMsgBuf);
            appMsgBuf.Print(left, top);

            // Print old IP address
            Console.SetCursorPosition(63, 2);
            Console.Write(ipOld);

            // Getting an new IP address
            ipAdd = GetIP(appPar, ref appMsgBuf);
            appMsgBuf.Print(left, top);

            if ((ipAdd != null) && (ipAdd != "None"))
            {

                // Print new IP address
                Console.SetCursorPosition(63, 3);
                Console.Write(ipAdd);

                numCheck++;

                DateTime dateAndTime = DateTime.Now;

                // Write local file
                int res2 = WriteIPLocal(appPar, ipAdd, dateAndTime, ref appMsgBuf);
                appMsgBuf.Print(left, top);

                if (res2 == 1)
                {

                    //FTP Module
                    if (ftpEnab)
                    {

                        int res3 = UploadFile(appPar);

                        if (res3 == 1) appMsgBuf.Add("Info", "Remote file on ftp was successfully written");
                        else appMsgBuf.Add("Error", "Remote file on ftp was not written!");
                        appMsgBuf.Print(left, top);
                    }

                    //Mail Module
                    if (mailEnab)
                    {


                        if (ipAdd != ipOld)
                        {
                            int res3 = SendIPmail(appPar, ipAdd);

                            if (res3 == 1) appMsgBuf.Add("Info", "Emails were successfully sent");
                            else appMsgBuf.Add("Error", "Emails were not sent!");
                            appMsgBuf.Print(left, top);
                        }
                        else
                        {
                            appMsgBuf.Add("Info", "Old and new IP addresses are equal. Emails were not sent.");
                            appMsgBuf.Print(left, top);
                        }

                    }
                }


            }
            else
            {
                ipAdd = ipOld;

                numOfBadChecks++;

                Console.SetCursorPosition(63, 3);
                Console.Write("xxx.xxx.xxx.xxx");
            }
        
        }

        static void StaticScreen(int LeftPos, int TopPos)
        {
            int i;
            Console.SetCursorPosition(LeftPos, TopPos);
            Console.Write("Send IP 1.0   Petr Prochazka (prochazka@valcon-int.com)  (Press ESC to exit)");
            Console.SetCursorPosition(LeftPos, TopPos+2);
            Console.Write("[ App Mode");
            Console.SetCursorPosition(LeftPos, TopPos + 3);
            Console.Write("[ FTP Module");
            Console.SetCursorPosition(LeftPos, TopPos + 4);
            Console.Write("[ Mail Module");
            Console.SetCursorPosition(LeftPos, TopPos + 6);
            Console.Write("[ Settings file");
            Console.SetCursorPosition(LeftPos, TopPos + 7);
            Console.Write("[ Local Storage");
            Console.SetCursorPosition(LeftPos, TopPos + 8);
            Console.Write("[ Remote Storage");
            Console.SetCursorPosition(LeftPos, TopPos + 9);
            Console.Write("[ Email addresses list");
            Console.SetCursorPosition(LeftPos, TopPos + 10);
            Console.Write("[ Log File");
            Console.SetCursorPosition(LeftPos, TopPos + 12);
            Console.Write("[ IP Check Server 1");
            Console.SetCursorPosition(LeftPos, TopPos + 13);
            Console.Write("[ IP Check Server 2");
            Console.SetCursorPosition(LeftPos, TopPos + 14);
            Console.Write("[ IP Check Server 3");
            Console.SetCursorPosition(LeftPos, TopPos + 16);
            Console.Write("[ FTP Server");
            Console.SetCursorPosition(LeftPos, TopPos + 17);
            Console.Write("[ SMTP Server");
            Console.SetCursorPosition(LeftPos+50, TopPos + 2);
            Console.Write("[ Old IP ] =");
            Console.SetCursorPosition(LeftPos + 50, TopPos + 3);
            Console.Write("[ New IP ] =");
            Console.SetCursorPosition(LeftPos, TopPos + 19);
            Console.Write("[ Last check");
            Console.SetCursorPosition(LeftPos, TopPos + 20);
            Console.Write("[ Next check in");
            Console.SetCursorPosition(LeftPos, TopPos + 22);
            Console.Write("[ Num of good checks     ] =");
            Console.SetCursorPosition(LeftPos, TopPos + 23);
            Console.Write("[ Num of bad checks      ] =");

            Console.SetCursorPosition(LeftPos + 25, TopPos + 2);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 3);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 4);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 6);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 7);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 8);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 9);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 10);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 12);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 13);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 14);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 16);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 17);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 19);
            Console.Write("] =");
            Console.SetCursorPosition(LeftPos + 25, TopPos + 20);
            Console.Write("] =        sec");

            Console.SetCursorPosition(LeftPos, TopPos + 25);
            for (i = 0; i < 80; i++) Console.Write("-");
  
            Console.SetCursorPosition(LeftPos, TopPos + 26);
            Console.Write(" Messages:");
        }

        public static void ClearLine(int left, int top)
        {
            int i;
            Console.SetCursorPosition(left, top);
            for (i = left; i < Console.BufferWidth; i++) Console.Write(" ");
        }

        public static void FillPar(Settings appSett, string settFile, string ApMode, int LeftPos, int TopPos)
        {
            string ftpMode  = appSett.getParByName("Application.Ftp.Enabled");
            string mailMode = appSett.getParByName("Application.Mail.Enabled");

            if (ftpMode == "true") ftpMode = "enabled";
            else ftpMode = "disabled";

            if (mailMode == "true") mailMode = "enabled";
            else mailMode = "disabled";

            Console.SetCursorPosition(LeftPos + 29, TopPos + 2);
            Console.Write(ApMode);

            Console.SetCursorPosition(LeftPos + 29, TopPos + 3);
            Console.Write(ftpMode);

            Console.SetCursorPosition(LeftPos + 29, TopPos + 4);
            Console.Write(mailMode);

            Console.SetCursorPosition(LeftPos + 29, TopPos + 6);
            Console.Write(settFile);
            Console.SetCursorPosition(LeftPos + 29, TopPos + 7);
            Console.Write(appSett.getParByName("Application.File.IpStorage"));
            Console.SetCursorPosition(LeftPos + 29, TopPos + 8);
            Console.Write(appSett.getParByName("Application.Ftp.IpStorage"));
            Console.SetCursorPosition(LeftPos + 29, TopPos + 9);
            Console.Write(appSett.getParByName("Application.File.MailList"));
            Console.SetCursorPosition(LeftPos + 29, TopPos + 10);
            Console.Write(appSett.getParByName("Application.File.Log"));
            Console.SetCursorPosition(LeftPos + 29, TopPos + 12);
            Console.Write(appSett.getParByName("Application.IP.Server1"));
            Console.SetCursorPosition(LeftPos + 29, TopPos + 13);
            Console.Write(appSett.getParByName("Application.IP.Server2"));
            Console.SetCursorPosition(LeftPos + 29, TopPos + 14);
            Console.Write(appSett.getParByName("Application.IP.Server3"));
            Console.SetCursorPosition(LeftPos + 29, TopPos + 16);
            Console.Write(appSett.getParByName("Application.Ftp.Host"));
            Console.SetCursorPosition(LeftPos + 29, TopPos + 17);
            Console.Write(appSett.getParByName("Application.Mail.Server"));
        
        }

        static void Main(string[] args)
        {
            string settFile = "settings.ini";
            string modePar = "-n";
            int res;
            int i;
            string ipAdd = "";
            string ipOld = "";
            int msgLeft = 0;
            int msgTop  = 28;
            long numOfChecks = 0;
            long numOfBadChecks = 0;
            int numPar = args.Length;
            string logFile;

            Console.CursorVisible = false;
            Console.WindowHeight = 55;
            Console.WindowWidth = Console.BufferWidth;

            // Mode Determination
            if (numPar >= 1)
            {
                modePar = args[0];

                if (args.Length >= 2)
                {
                    settFile = args[1];

                }
                
            }
            else
            {
                modePar = "-n";
            }

            if ((modePar == "-o") || (modePar == "-n"))
            {
                string appMode = "none";

                if (modePar == "-o") appMode = "one-time";
                if (modePar == "-n") appMode = "normal";
                
                
                StaticScreen(0, 0);

                Settings appPar = new Settings();
                MsgBuff appMsg = new MsgBuff(20, 10000, "");

                res = appPar.Update(settFile);

                // Loading was successful
                if (res == 1)
                {
                    appMsg.Add("Info", "Reading of " + settFile + " was successful");
                    appMsg.Print(msgLeft, msgTop);

                    logFile = appPar.getParByName("Application.File.Log");
                    appMsg.LogFile = logFile;

                    FillPar(appPar,settFile,appMode, 0, 0);

                    int key = 0;
                    int repSec = Convert.ToInt16(appPar.getParByName("Application.App.Repeat"));
                    if (repSec < 60) repSec = 60;
                    if (repSec > 86400) repSec = 86400;


                    RunApp(ref numOfChecks,ref numOfBadChecks, appPar, ref ipOld, ref ipAdd, ref appMsg, msgLeft, msgTop);
                    DateTime lastRun = DateTime.Now;
                    Console.SetCursorPosition(29, 19);
                    Console.Write(lastRun.ToString());
                    Console.SetCursorPosition(29, 22);
                    Console.Write(numOfChecks.ToString());
                    Console.SetCursorPosition(29, 23);
                    Console.Write(numOfBadChecks.ToString());

                    appMsg.WriteLog();

                    do
                    {
                            
                        if(modePar=="-n")
                        {
                            // Time difference from last check
                            DateTime casAkt = DateTime.Now;
                            long elapsedTicks = casAkt.Ticks - lastRun.Ticks;
                            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                            double diff = repSec - elapsedSpan.TotalSeconds;
                            if (diff < 1) diff = 0;

                            // Print difference on screen
                            Console.SetCursorPosition(29, 20);
                            Console.Write("     ");
                            Console.SetCursorPosition(29, 20);
                            Console.Write("{0:N0}", diff);

                            if (diff < 1)
                            {
                                ClearLine(63, 2);
                                ClearLine(63, 3);

                                RunApp(ref numOfChecks, ref numOfBadChecks, appPar, ref ipOld, ref ipAdd, ref appMsg, msgLeft, msgTop);
                                lastRun = DateTime.Now;
                                Console.SetCursorPosition(29, 19);
                                Console.Write(lastRun.ToString());
                                Console.SetCursorPosition(29, 22);
                                Console.Write(numOfChecks.ToString());
                                Console.SetCursorPosition(29, 23);
                                Console.Write(numOfBadChecks.ToString());
                                appMsg.WriteLog();
                            }
                            else Thread.Sleep(900);
                        }
                        

                        if (Console.KeyAvailable)
                        {
                            key = Convert.ToInt16(Console.ReadKey().Key);
                        }

                    }
                    while (key != 27);
                }
                else
                { 
                    appMsg.Add("Error", "Reading of "+settFile+" was not successful. Terminating!");
                    appMsg.Print(msgLeft, msgTop);
                }

            }
            else
            {
                Console.WriteLine("usage: sendip.exe [-o -n] <settings file>");
                Console.ReadLine();
            }

        }
    }
}
