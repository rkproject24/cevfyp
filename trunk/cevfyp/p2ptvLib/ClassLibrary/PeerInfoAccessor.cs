using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClassLibrary;
    using System.Threading;

    public class PeerInfoAccessor
    {
        public xml RPI;
        public string xmlFile;

        public PeerInfoAccessor(string fileName)
        {
            this.RPI = new xml(fileName, "Info", false);
            //RPI.AddAttribute("Info", "MaxId", "0");
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
            string listenPort = RPI.Read("Peer", "ID", id, "listenPort");

            return new PeerNode(id, ip, Int32.Parse(layer), Int32.Parse(listenPort));
        }

        public void addPeer(PeerNode peer)
        {
            string[] attributes = {"ID"};
            string[] attributesValue = {peer.Id};

            string[] Info = {"IP", "Layer", "listenPort" };
            string[] Value = { peer.Ip, peer.Layer.ToString(), peer.ListenPort.ToString() };

            RPI.Add("Peer", Info, Value, attributes, attributesValue);
            
        }

        public void initialize(int treeSize)
        {
            RPI.AddAttribute("Info", "MaxId", "0");
            Thread.Sleep(500);
            RPI.AddAttribute("Info", "treeSize", treeSize.ToString());
        }


        public void setMaxId(int MaxId)
        {
            RPI.modifyAttribute("Info", "MaxId", MaxId.ToString());
        }

        public int getMaxId()
        {

            return Int32.Parse(RPI.ReadAttribute("Info", "MaxId"));
        }

        public void setTreeSize(int MaxId)
        {
            RPI.modifyAttribute("Info", "treeSize", MaxId.ToString());
        }

        public int getTreeSize()
        {

            return Int32.Parse(RPI.ReadAttribute("Info", "treeSize"));
        }
    }
}
