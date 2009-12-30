using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClassLibrary;

    public class PeerInfoAccessor
    {
        public xml RPI;
        public string xmlFile;

        public PeerInfoAccessor(string fileName)
        {
            this.RPI = new xml(fileName, "Info", false);
            RPI.AddAttribute("Info", "MaxId", "0");
            this.xmlFile = fileName;
        }

        public int getEleNum()
        {
            return RPI.GetElementNum();
        }

        private string getIP(string index)
        {
            return RPI.Read("Peer", "ID", index, "IP");
        }

        private string getLayer(string index)
        {
            return RPI.Read("Peer", "ID", index, "Layer");
        }

        public string getLayerWithIP(string IP)
        {
            string tempIP;
            int eleNum = RPI.GetElementNum();
            for (int i = 0; i < eleNum; i++)
            {
                tempIP = getIP(i.ToString());
                if (string.Compare(tempIP, IP) == 0)
                    return getLayer(i.ToString());
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
        */

//by vinci
        //public PeerNode[] getPeerlist()
        //{

        //}

        public PeerNode getPeer(string id)
        {
            string ip = getIP(id);
            string layer = getLayer(id);
            return new PeerNode(id, ip, Int32.Parse(layer));
        }

        public void addPeer(PeerNode peer)
        {
            string[] attributes = {"ID"};
            string[] attributesValue = {peer.Id};

            string[] Info = {"IP", "Layer" };
            string[] Value = { peer.Ip.ToString(), peer.Layer.ToString() };
            RPI.Add("Peer", Info, Value, attributes, attributesValue);
            
        }

        public void setMaxId(int MaxId)
        {
            RPI.modifyAttribute("Info", "MaxId", MaxId.ToString());
        }

        public int getMaxId()
        {

            return Int32.Parse(RPI.ReadAttribute("Info", "MaxId"));
        }
    }
}
