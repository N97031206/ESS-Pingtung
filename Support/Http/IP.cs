using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Support.Http
{
    public class IP
    {
        public static string GetClientIP(HttpRequestBase request)
        {
            string ip = (request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null &&
                request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ?
                request.ServerVariables["HTTP_X_FORWARDED_FOR"] :
                request.ServerVariables["HTTP_X_REAL_IP"];

            if (string.IsNullOrEmpty(ip))
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ipAddress in host.AddressList)
                {
                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip = ipAddress.ToString();
                    }
                }
            }
            return string.IsNullOrEmpty(ip) ? "" : ip;
        }
    }
}
