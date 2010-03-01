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
        public List<PeerNode> getPeersByParent(string parentid)
        {
            List<PeerNode> childPeers = new List<PeerNode>();
            int maxid = getMaxId();
            for (int i = 1; i <= maxid; i++)
            {
                string fileParentid = RPI.Read("Peer", "ID", i.ToString(), "Parentid");
                if (fileParentid.Equals(parentid))
                {
                    childPeers.Add(getPeer(i.ToString()));
                }
                    
            }
            return childPeers;
        }
        public PeerNode getPeer(string id)
        {
            string ip = getIP(id);
            if (ip.Equals("")) //return NULL if the node is not exist in the list
                return null;
            string layer = getLayer(id);
            string listenPort = RPI.Read("Peer", "ID", id, "listenPort");
            string parentid = RPI.Read("Peer", "ID", id, "Parentid");
            string nullChunk = RPI.Read("Peer", "ID", id, "NullChunkTotal");

            //try
            //{
            if (nullChunk == null)
            {
                return new PeerNode(id, ip, Int32.Parse(layer), Int32.Parse(listenPort), parentid);
            }
            return new PeerNode(id, ip, Int32.Parse(layer), Int32.Parse(listenPort), parentid, Int32.Parse(nullChunk));
            //}
            //catch
            //{
            //    return null;
            //}
        }

        public PeerNode getRandomPeer()
        {
            string id = RPI.ReadRandom("Peer", "ID");
            return getPeer(id);
        }

        public void addPeer(PeerNode peer)
        {
            string[] attributes = {"ID"};
            string[] attributesValue = {peer.Id};

            string[] Info = { "IP", "Layer", "listenPort", "Parentid", "NullChunkTotal" };
            string[] Value = { peer.Ip, peer.Layer.ToString(), peer.ListenPort.ToString(), peer.Parentid,peer.NullChunkTotal.ToString() };

            RPI.Add("Peer", Info, Value, attributes, attributesValue);
            
        }
        public bool deletePeer(PeerNode peer)
        {
            string attributesValue =  peer.Id ;

            return RPI.deleteInnerNode("Peer", "ID", attributesValue);
        }

        public void initialize(int treeSize)
        {
            RPI.AddAttribute("Info", "MaxId", "0");
            //Thread.Sleep(500);
            RPI.AddAttribute("Info", "treeSize", treeSize.ToString());
        }


        public void setMaxId(int MaxId)
        {
            RPI.modifyAttribute("Info", "MaxId", MaxId.ToString());
        }

        public int getMaxId()
        {
            string maxId = RPI.ReadAttribute("Info", "MaxId");
            if (maxId.Equals(""))
                return -1;
            return Int32.Parse(maxId);
        }

        public void setTreeSize(int MaxId)
        {
            RPI.modifyAttribute("Info", "treeSize", MaxId.ToString());
        }

        public int getTreeSize()
        {
            return Int32.Parse(RPI.ReadAttribute("Info", "treeSize"));
        }

        //public List<string> getPeerPrefix(PeerNode peer)
        //{
        //    List<string> prefix = new List<string>();
        //    PeerNode searchNode = peer;
        //    while (!searchNode.Parentid.Equals( "-1"))
        //    {
        //        searchNode = getPeer(searchNode.Parentid);
        //        if (searchNode == null)
        //            break;
        //        prefix.Add(searchNode.Parentid);

        //    }
        //    //prefix.Add("-1");
        //    return prefix;
        //}

        public bool checkchild(PeerNode peer, string selfid) // return true if peer is its child 
        {
            //if (peer.Id.Equals(selfid))
            //    return true;
            //List<string> prefix = getPeerPrefix(peer);        
            //foreach(string node in prefix)
            //{
            //    if(node.Equals(selfid))
            //        return true;
            //}
            //return false;

            if (peer.Id.Equals(selfid))
                return true;
            while (!(peer.Parentid).Equals(selfid))
            {
                if (peer.Parentid.Equals("-1"))
                    return false;
                if (peer.Parentid.Equals("-2"))
                    break;
                peer = getPeer(peer.Parentid);

            }
            return true;
        }

        public bool reconecting(string id)
        {
            string ip = getIP(id);
            if (ip.Equals("")) //return NULL if the node is not exist in the list
                return true;
            string parentid = RPI.Read("Peer", "ID", id, "Parentid");
            if (parentid.Equals("-2"))
                return true;
            return false;
        }

        public bool hasPeerInList()
        {
            return RPI.hasNode();
        }

        public bool load()
        {
            return RPI.load();
        }

        //public bool sortLoad(string attribute)
        //{
        //    return RPI.sortLoad("/Info/Peer", attribute);
        //}

        public PeerNode getLeastChunkNullPeer()
        {
            RPI.sortLoad("/Info/Peer", "NullChunkTotal");
            string id = RPI.ReadByIndex("Peer", "ID",0);
            return getPeer(id);
        }
    }
}
