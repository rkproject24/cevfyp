using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace ClassLibrary
{
    public class Utility
    {
        public Utility()
        { }

        public bool TcpUsing(int port)
        {
            IPGlobalProperties ipGP = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endpoints = ipGP.GetActiveTcpListeners();
            if (endpoints == null || endpoints.Length == 0) return false;
            for (int i = 0; i < endpoints.Length; i++)
                if (endpoints[i].Port == port)
                    return true;
            return false;
        }

        public int RanPort(int begin, int end)
        {
            IPGlobalProperties ipGP = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endpoints = ipGP.GetActiveTcpListeners();

            Random ran = new Random();

            try
            {
                int timmer = (end - begin) * 2;
                bool found = false;
                do
                {
                    int port = ran.Next(begin, end + 1);
                    found = true;
                    for (int i = 0; i < endpoints.Length; i++)
                        if (endpoints[i].Port == port)
                            found = false;
                    if (found) return port;
                    timmer--;
                } while (!found && timmer > 0);
            }
            catch { return -1; }
            return -1;
        }
    }
}
