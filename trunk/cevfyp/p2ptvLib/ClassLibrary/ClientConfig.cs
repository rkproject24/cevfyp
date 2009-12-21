using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ClassLibrary
{

    public class ClientConfig
    {
        private string pluginPath;
        private int ServerSLPort;
        //private int vlcStreamPortBase;
        private int LisPort;
        private int Dataport;
        private int vlcPortBase;
        private int ConportBase;

        private int maxPeer;
        private int chunkSize;
        private int chunkCapacity;
        private int chunkBuf;
        private int startBuf;


        public ClientConfig()
        {
         this.pluginPath = "";
         this.maxPeer = 0;
         this.chunkSize = 0;
         this.chunkCapacity = 0;
         this.ServerSLPort = 0;
         //this.vlcStreamPortBase = 0;
         this.LisPort = 0;
         this.Dataport = 0;
         this.vlcPortBase = 0;
         this.ConportBase = 0;
         this.chunkBuf = 0;
         this.startBuf = 0;
        }

        public ClientConfig(string pluginPath, int ServerSLPort, int LisPort, int Dataport, int ConportBase, int vlcPortBase, int maxPeer, int chunkSize, int chunkCapacity, int chunkBuf, int startBuf)
        {
            this.pluginPath = pluginPath;

            this.ServerSLPort = ServerSLPort;
            //this.vlcStreamPortBase = vlcStreamPortBase;
            this.LisPort = LisPort;
            this.Dataport = Dataport;
            this.vlcPortBase = vlcPortBase;
            this.ConportBase = ConportBase;
            this.chunkBuf = chunkBuf;
            this.startBuf = startBuf;
            this.maxPeer = maxPeer;
            this.chunkSize = chunkSize;
            this.chunkCapacity = chunkCapacity;
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
        
        public int SLPort
        {
            get { return LisPort; }
            set { LisPort = value; }
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

            xml store = new xml(fileName, "client",true);

            string[] type = {"pluginPath", "maxPeer", "chunkSize", "chunkCapacity", "ServerSLPort", "LisPort", "Dataport", "vlcPortBase", "ConportBase", "chunkBuf", "startBuf" };

            string[] value = {this.pluginPath, this.maxPeer.ToString(), this.chunkSize.ToString(), this.chunkCapacity.ToString(), this.ServerSLPort.ToString(), this.LisPort.ToString(), this.Dataport.ToString(), this.vlcPortBase.ToString(), this.ConportBase.ToString(), this.chunkBuf.ToString(), this.startBuf.ToString()};

            store.Add(type, value);
        }

        public void load(string fileName)
        {
            xml load = new xml(fileName, "client");
       
            this.pluginPath = load.Read("client", "pluginPath");
            this.maxPeer = Convert.ToInt32(load.Read("client", "maxPeer"));
            this.chunkSize = Convert.ToInt32(load.Read("client", "chunkSize"));
            this.chunkCapacity = Convert.ToInt32(load.Read("client", "chunkCapacity"));
            this.ServerSLPort = Convert.ToInt32(load.Read("client", "ServerSLPort"));
            //this.vlcStreamPortBase = Convert.ToInt32(load.Read("client", "vlcStreamPortBase"));
            this.LisPort = Convert.ToInt32(load.Read("client", "LisPort"));
            this.Dataport = Convert.ToInt32(load.Read("client", "Dataport"));
            this.vlcPortBase = Convert.ToInt32(load.Read("client", "vlcPortBase"));
            this.ConportBase = Convert.ToInt32(load.Read("client", "ConportBase"));
            this.chunkBuf = Convert.ToInt32(load.Read("client", "chunkBuf"));
            this.startBuf = Convert.ToInt32(load.Read("client", "startBuf"));
        }

    }
}
