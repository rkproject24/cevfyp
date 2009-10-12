using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerServer
{
    class PeerNode
    {
        private string ip;
        private int maxClient;
        private List<string> childPeer;



        public PeerNode(string ip, int maxParent, int maxClient)
        {
            parentPeer = new List<string>(maxParent);
            childPeer = new List<string>(maxClient);
            this.ip = ip;
        }

        public void addChild(string ip)
        {
            childPeer.Add(ip);
        }

        public void addParent(string ip)
        {
            parentPeer.Add(ip);
        }

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }
        public List<string> ChildPeer
        {
            get { return childPeer; }
            set { childPeer = value; }
        }
        private List<string> parentPeer;

        public List<string> ParentPeer
        {
            get { return parentPeer; }
            set { parentPeer = value; }
        }
    }
}
