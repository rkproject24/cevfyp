using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    public class PeerNode
    {
        private string id;
        private string ip;
        private int maxClient;
        

        private List<string> childPeer;
        private int layer;




        public PeerNode(string id, string ip, int maxClient)
        {
           // parentPeer = new List<string>(maxParent);
            this.id = id;
            this.ip = ip;
            this.layer = 0;
            childPeer = new List<string>(maxClient);
        }

        public void addChild(string ip)
        {
            childPeer.Add(ip);
        }

        //public void addParent(string ip)
        //{
        //    parentPeer.Add(ip);
        //}
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }
        public int Layer
        {
            get { return layer; }
            set { layer = value; }
        }
        public int MaxClient
        {
            get { return maxClient; }
            set { maxClient = value; }
        }
        public List<string> ChildPeer
        {
            get { return childPeer; }
            set { childPeer = value; }
        }
        //private List<string> parentPeer;

        //public List<string> ParentPeer
        //{
        //    get { return parentPeer; }
        //    set { parentPeer = value; }
        //}
    }
}
