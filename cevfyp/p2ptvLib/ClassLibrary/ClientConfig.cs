using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClassLibrary
{

    public class ClientConfig
    {
        private string pluginPath;
        private int ServerSLPort;
        //private int vlcStreamPortBase;
        private int lisPort;
        private int lisPortup;
        private int Dataport;
        private int dataportup;
        private int vlcPortBase;
        private int vlcPortup;
        private int ConportBase;
        private int conportup;
        private int maxPeer;
        private int chunkSize;
        private int chunkCapacity;
        private int chunkBuf;
        private int startBuf;

        private string trackerip;
        private int trackerPort;
        private bool localdisplay;
        private int maxNullChunk;
        private int readStreamTimeout;


        public ClientConfig()
        {
            this.pluginPath = "";
            this.maxPeer = 0;
            this.chunkSize = 0;
            this.chunkCapacity = 0;
            this.ServerSLPort = 0;
            //this.vlcStreamPortBase = 0;
            this.lisPort = 0;
            this.Dataport = 0;
            this.vlcPortBase = 0;
            this.vlcPortup = 0;
            this.ConportBase = 0;
            this.chunkBuf = 0;
            this.startBuf = 0;
            trackerip = "";
            trackerPort = 0;
            this.lisPortup = 0;
            this.dataportup = 0;
            this.conportup = 0;
            this.localdisplay = true;

            this.maxNullChunk = 9999;
            this.readStreamTimeout = 9999;
        }

        public ClientConfig(string pluginPath, int ServerSLPort, int LisPort, int Dataport, int ConportBase, int vlcPortBase, int vlcPortup, int maxPeer, int chunkSize, int chunkCapacity, int chunkBuf, int startBuf, string trackerip, int trackerPort, int lisPortup, int dataportup, int conportup, bool localdisplay, int maxNullChunk, int readStreamTimeout)
        {
            this.pluginPath = pluginPath;

            this.ServerSLPort = ServerSLPort;
            //this.vlcStreamPortBase = vlcStreamPortBase;
            this.lisPort = LisPort;
            this.Dataport = Dataport;
            this.vlcPortBase = vlcPortBase;
            this.vlcPortup = vlcPortup;
            this.ConportBase = ConportBase;
            this.chunkBuf = chunkBuf;
            this.startBuf = startBuf;
            this.maxPeer = maxPeer;
            this.chunkSize = chunkSize;
            this.chunkCapacity = chunkCapacity;
            this.trackerip = trackerip;
            this.trackerPort = trackerPort;
            this.lisPortup = lisPortup;
            this.dataportup = dataportup;
            this.conportup = conportup;
            this.localdisplay = localdisplay;

            this.maxNullChunk = maxNullChunk;
            this.readStreamTimeout = readStreamTimeout;
        }

        public string PluginPath
        {
            get { return pluginPath; }
            set { pluginPath = value; }
        }
        public int MaxPeer
        {
            get { return maxPeer; }
            set { maxPeer = value; }
        }
        public int ChunkSize
        {
            get { return chunkSize; }
            set { chunkSize = value; }
        }

        public int ChunkCapacity
        {
            get { return chunkCapacity; }
            set { chunkCapacity = value; }
        }

        public int ServerSLPort1
        {
            get { return ServerSLPort; }
            set { ServerSLPort = value; }
        }

        //public int VlcStreamPortBase
        //{
        //    get { return vlcStreamPortBase; }
        //    set { vlcStreamPortBase = value; }
        //}

        public int LisPort
        {
            get { return lisPort; }
            set { lisPort = value; }
        }

        public int Dport
        {
            get { return Dataport; }
            set { Dataport = value; }
        }


        public int VlcPortBase
        {
            get { return vlcPortBase; }
            set { vlcPortBase = value; }
        }
        public int VlcPortup
        {
            get { return vlcPortup; }
            set { vlcPortup = value; }
        }

        public int CportBase
        {
            get { return ConportBase; }
            set { ConportBase = value; }
        }



        public int ChunkBuf
        {
            get { return chunkBuf; }
            set { chunkBuf = value; }
        }


        public int StartBuf
        {
            get { return startBuf; }
            set { startBuf = value; }
        }
        public string Trackerip
        {
            get { return trackerip; }
            set { trackerip = value; }
        }


        public int TrackerPort
        {
            get { return trackerPort; }
            set { trackerPort = value; }
        }
        public int LisPortup
        {
            get { return lisPortup; }
            set { lisPortup = value; }
        }
        public int Dataportup
        {
            get { return dataportup; }
            set { dataportup = value; }
        }
        public int Conportup
        {
            get { return conportup; }
            set { conportup = value; }
        }
        public bool Localdisplay
        {
            get { return localdisplay; }
            set { localdisplay = value; }
        }
        public int MaxNullChunk
        {
            get { return maxNullChunk; }
            set { maxNullChunk = value; }
        }
        public int ReadStreamTimeout
        {
            get { return readStreamTimeout; }
            set { readStreamTimeout = value; }
        }
        /*      public void save_old(string fileName)
              {
                  if (File.Exists(fileName))
                  {
                      File.Delete(fileName);
                  }

                  xml store = new xml(fileName, "client");
                  store.Add("client", "pluginPath", this.pluginPath);
                  store.Add("client", "maxPeer", this.maxPeer.ToString());
                  store.Add("client", "chunkSize", this.chunkSize.ToString());
                  store.Add("client", "chunkCapacity", this.chunkCapacity.ToString());
                  store.Add("client", "ServerSLPort", this.ServerSLPort.ToString());
                  //store.Add("client", "vlcStreamPortBase", this.vlcStreamPortBase.ToString());
                  store.Add("client", "LisPort", this.LisPort.ToString());
                  store.Add("client", "Dataport", this.Dataport.ToString());
                  store.Add("client", "vlcPortBase", this.vlcPortBase.ToString());
                  store.Add("client", "ConportBase", this.ConportBase.ToString());
                  store.Add("client", "chunkBuf", this.chunkBuf.ToString());
                  store.Add("client", "startBuf", this.startBuf.ToString());
              }*/


        public void save(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            xml store = new xml(fileName, "client", true);

            string[] type = { "pluginPath", "maxPeer", "chunkSize", "chunkCapacity", "ServerSLPort", "LisPort", "Dataport", "vlcPortBase", "vlcPortUp", "ConportBase", "chunkBuf", "startBuf", "trackerip", "trackerPort", "LisPortup", "dataportup", "conportup", "display", "readStreamTimeout", "maxNullChunk" };

            string[] value = { this.pluginPath, this.maxPeer.ToString(), this.chunkSize.ToString(), this.chunkCapacity.ToString(), this.ServerSLPort.ToString(), this.lisPort.ToString(), this.Dataport.ToString(), this.vlcPortBase.ToString(), this.vlcPortup.ToString(), this.ConportBase.ToString(), 
                                 this.chunkBuf.ToString(), this.startBuf.ToString(), this.trackerip, this.trackerPort.ToString(), this.lisPortup.ToString(), dataportup.ToString(), conportup.ToString(), localdisplay.ToString(), this.readStreamTimeout.ToString(), this.maxNullChunk.ToString() };

            store.Add(type, value);
        }

        public void load(string fileName)
        {
            xml load = new xml(fileName, "client", false);

            this.pluginPath = load.Read("client", "pluginPath");
            this.maxPeer = Convert.ToInt32(load.Read("client", "maxPeer"));
            this.chunkSize = Convert.ToInt32(load.Read("client", "chunkSize"));
            this.chunkCapacity = Convert.ToInt32(load.Read("client", "chunkCapacity"));
            this.ServerSLPort = Convert.ToInt32(load.Read("client", "ServerSLPort"));
            //this.vlcStreamPortBase = Convert.ToInt32(load.Read("client", "vlcStreamPortBase"));
            this.lisPort = Convert.ToInt32(load.Read("client", "LisPort"));
            this.Dataport = Convert.ToInt32(load.Read("client", "Dataport"));
            this.vlcPortBase = Convert.ToInt32(load.Read("client", "vlcPortBase"));
            this.vlcPortup = Convert.ToInt32(load.Read("client", "vlcPortUp"));
            this.ConportBase = Convert.ToInt32(load.Read("client", "ConportBase"));
            this.chunkBuf = Convert.ToInt32(load.Read("client", "chunkBuf"));
            this.startBuf = Convert.ToInt32(load.Read("client", "startBuf"));

            this.trackerip = load.Read("client", "trackerip");
            this.trackerPort = Convert.ToInt32(load.Read("client", "trackerPort"));
            this.lisPortup = Convert.ToInt32(load.Read("client", "LisPortup"));
            this.dataportup = Convert.ToInt32(load.Read("client", "dataportup"));
            this.conportup = Convert.ToInt32(load.Read("client", "conportup"));
            this.localdisplay = Convert.ToBoolean(load.Read("client", "display"));

            this.maxNullChunk = Convert.ToInt32(load.Read("client", "maxNullChunk"));
            this.readStreamTimeout = Convert.ToInt32(load.Read("client", "readStreamTimeout"));
        }

    }
}
