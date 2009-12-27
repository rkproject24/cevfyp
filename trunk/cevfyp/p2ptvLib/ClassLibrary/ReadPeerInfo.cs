using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary
{
using System;
using System.Collections.Generic;
using System.Text;
using ClassLibrary;

public class ReadPeerInfo
{
        public xml RPI;
        public string xmlFile;

        public ReadPeerInfo(string fileName, string group)
        {
            this.RPI = new xml(fileName, group);
            this.xmlFile = fileName;
        }

        public int GetEleNum()
        {
            return RPI.GetElementNum();
        }

        public string GetIP(string index)
        {
            return RPI.Read(index,"IP");
        }

        public string GetLayer(string index)
        {
            return RPI.Read(index, "Layer");
        }

        public string GetLayerWithIP(string IP)
        {
            string tempIP;
            int eleNum = RPI.GetElementNum();
            for (int i = 0; i < eleNum; i++ )
            {
                tempIP = GetIP(i.ToString());
                if (string.Compare(tempIP, IP) == 0)
                    return GetLayer(i.ToString());
            }
            return "";
        }
/*
        public string deleteIP(string ip)
        {
            string tempIP;
            int eleNum = RPI.GetElementNum();
            for (int i = 0; i < eleNum; i++)
            {
                tempIP = GetIP(i.ToString());
                if (string.Compare(tempIP, ip) == 0)
                   RPI.delet
            }
            return "";
        }
  */  }
}
