﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    public class PeerNode
    {
        private string id;
        private string ip;
        //private int maxClient;
        private int listenPort;

        //private List<string> childPeer;
        //private int layer;
        private string parentid;
        private int nullChunkTotal;

        public PeerNode(string id, string ip, int listenPort,string parentid)
        {
           // parentPeer = new List<string>(maxParent);
            this.id = id;
            this.ip = ip;
            this.listenPort = listenPort;
            this.parentid= parentid;

            //this.layer = 0;
            this.nullChunkTotal = 0;
            //childPeer = new List<string>(maxClient);
        }

        public PeerNode(string id, List<string> peerstr)
        {
            // parentPeer = new List<string>(maxParent);
            this.id = id;
            this.ip = peerstr[0];
            this.listenPort = Int32.Parse(peerstr[1]);
            this.parentid = peerstr[2];

            if (peerstr.Count > 3)
                this.nullChunkTotal = Int32.Parse(peerstr[3]);
            else
                this.nullChunkTotal = 0;

        }

        public PeerNode(string id, string ip, int listenPort, string parentid, int nullChunkTotal)
        {
            this.id = id;
            this.ip = ip;
            this.listenPort = listenPort;
            this.parentid = parentid;

            //this.layer = 0;
            this.nullChunkTotal = nullChunkTotal;
            //childPeer = new List<string>(maxClient);
        }

        //public void addChild(string ip)
        //{
        //    childPeer.Add(ip);
        //}

        //public void addParent(string ip)
        //{
        //    parentPeer.Add(ip);
        //}
        public string Parentid
        {
            get { return parentid; }
            set { parentid = value; }
        }

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
        //public int Layer
        //{
        //    get { return layer; }
        //    set { layer = value; }
        //}
        //public int MaxClient
        //{
        //    get { return maxClient; }
        //    set { maxClient = value; }
        //}
        //public List<string> ChildPeer
        //{
        //    get { return childPeer; }
        //    set { childPeer = value; }
        //}
        public int ListenPort
        {
            get { return listenPort; }
            set { listenPort = value; }
        }
        public int NullChunkTotal
        {
            get { return nullChunkTotal; }
            set { nullChunkTotal = value; }
        }
        //private List<string> parentPeer;

        //public List<string> ParentPeer
        //{
        //    get { return parentPeer; }
        //    set { parentPeer = value; }
        //}
    }
}
