using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace SendIP
{
    class Mail
    {
        SmtpClient client;

        public Mail(string host, int port, string login, string pass)
        {
            try
            {
                client = new SmtpClient();
                client.Port = port;
                client.Host = host;
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(login, pass);
            }
            catch (Exception e)
            {
                client = null;
            }
        }

        public int SendMsg(string from, string[] address, string subject, string msg)
        {
            int vrat = -1;
            int i;

            if (client != null)
            {

                try
                {
                    for (i = 0; i < address.Length; i++)
                    {
                        MailMessage mm = new MailMessage(from, address[i], subject, msg);
                        mm.BodyEncoding = UTF8Encoding.UTF8;
                        mm.DeliveryNotificationOptions = DeliveryNotificationOptions.Never;
                        client.Send(mm);
                        mm.Dispose();
                    }
                    vrat = 1;
                }
                catch (Exception e)
                { 
                
                }
            
            }

            return vrat;
        }
        
    
    }
}
