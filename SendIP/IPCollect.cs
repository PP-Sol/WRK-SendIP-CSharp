using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Net.NetworkInformation;
using System.Web;

namespace SendIP
{
    // Conversion.cs class
    // This class contains methods for conversion between user defined data types
    public static class IPCollect
    {
        public static string GetPublicIP(string WebDNS)
        {
            String direction = "None";

            try
            {
                WebRequest request = WebRequest.Create(WebDNS);
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    direction = stream.ReadToEnd();
                }

                //Search for the ip in the html
                int first = direction.IndexOf("Address: ") + 9;
                int last = direction.LastIndexOf("</body>");
                direction = direction.Substring(first, last - first);
            }
            catch (Exception e)
            {
                direction = null;
            }

            return direction;
        }

        public static string GetPublicIP2(string WebDNS)
        {
            string vrat = "None";

            try
            {
                WebClient webClient = new WebClient();
                vrat = webClient.DownloadString(WebDNS);
            }
            catch (Exception e)
            {
                vrat = null;
            }
            
            return vrat;
        }

        public static string GetPublicIP3()
        {
            string vrat = "None";

            try
            {
                vrat = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            }
            catch (Exception e)
            {
                vrat = null;
            }

            return vrat;
        }

        

    }
}
