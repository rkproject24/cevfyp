using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClassLibrary
{

    public class ServerConfig
    {
        private string pluginPath;
        private string streamType;
        private int maxClient;
        private int vlcStreamPort;
        private int SLisPort;
        private int sLisPortup;
        private int Dataport;
        private int dataportup;
        private int ConportBase;
        private int conportup;
        private int TreeSizes;
        //private string serverip;
        private string trackerip;
        private int trackerPort;

        private string videoDir;
        private int chunkSize;
        private int receiveStreamSize;

        public ServerConfig()
        {
            pluginPath = "";
            streamType = "";
            videoDir = "";
            maxClient = 0;
            vlcStreamPort = 0;
            SLisPort = 0;
            sLisPortup = 0;
            Dataport = 0;
            dataportup = 0;
            ConportBase = 0;
            conportup = 0;
            TreeSize = 0;
            //serverip = "";
            trackerip = "";
            trackerPort = 0;
            receiveStreamSize = 0;
            chunkSize = 0;

        }

        //public ServerConfig(string pluginPath, string streamType, string videoDir, int maxClient, int vlcStreamPort, int SLisPort, int Dataport, int ConportBase, int TreeSizes, string serverip, int receiveStreamSize, int chunkSize, string trackerip)
        public ServerConfig(string pluginPath, string streamType, string videoDir, int maxClient, int vlcStreamPort, int SLisPort, int sLisPortup, int Dataport, int dataportup, int ConportBase, int conportup, int TreeSizes, int receiveStreamSize, int chunkSize, string trackerip, int trackerPort)
        {
            this.pluginPath = pluginPath;
            this.streamType = streamType;
            this.videoDir = videoDir;
            this.maxClient = maxClient;
            this.vlcStreamPort = vlcStreamPort;
            this.SLisPort = SLisPort;
            this.sLisPortup = sLisPortup;
            this.Dataport = Dataport;
            this.dataportup = dataportup;
            this.ConportBase = ConportBase;
            this.conportup = conportup;
            this.TreeSize = TreeSize;
            //this.serverip = serverip;
            this.chunkSize = chunkSize;
            this.receiveStreamSize = receiveStreamSize;
            this.trackerip = trackerip;
            this.trackerPort = trackerPort;
        }

        public string PluginPath
        {
            get { return pluginPath; }
            set { pluginPath = value; }
        }
        public string StreamType
        {
            get { return streamType; }
            set { streamType = value; }
        }
        public string VideoDir
        {
            get { return videoDir; }
            set { videoDir = value; }
        }   
        public int MaxClient
        {
            get { return maxClient; }
            set { maxClient = value; }
        }
        public int VlcStreamPort
        {
            get { return vlcStreamPort; }
            set { vlcStreamPort = value; }
        }
        public int SLPort
        {
            get { return SLisPort; }
            set { SLisPort = value; }
        }
        public int SLisPortup
        {
            get { return sLisPortup; }
            set { sLisPortup = value; }
        }
        public int Dport
        {
            get { return Dataport; }
            set { Dataport = value; }
        }
        public int Dataportup
        {
            get { return dataportup; }
            set { dataportup = value; }
        }
        public int CportBase
        {
            get { return ConportBase; }
            set { ConportBase = value; }
        }
        public int Conportup
        {
            get { return conportup; }
            set { conportup = value; }
        }
        public int TreeSize
        {
            get { return TreeSizes; }
            set { TreeSizes = value; }
        }

        //public string Serverip
        //{
        //    get { return serverip; }
        //    set { serverip = value; }
        //}
        public int ReceiveStreamSize
        {
            get { return receiveStreamSize; }
            set { receiveStreamSize = value; }
        }
        public int ChunkSize
        {
            get { return chunkSize; }
            set { chunkSize = value; }
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

        public void save(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            xml store = new xml(fileName, "server",true);

            //string[] type = { "pluginPath", "streamType", "videoDir", "maxClient", "vlcStreamPort", "SLisPort", "Dataport", "ConportBase", "TreeSize", "serverip", "trackerip", "chunkSize","receiveStreamSize"};
            //string[] value = {this.pluginPath, this.streamType, this.videoDir, this.maxClient.ToString(), this.vlcStreamPort.ToString(), this.SLisPort.ToString(), this.Dataport.ToString(), this.ConportBase.ToString(), this.TreeSize.ToString(), this.serverip, this.trackerip, this.chunkSize.ToString(), this.receiveStreamSize.ToString()};
            string[] type = { "pluginPath", "streamType", "videoDir", "maxClient", "vlcStreamPort", "SLisPort", "sLisPortup", "Dataport", "dataportup", "ConportBase", "conportup", "TreeSize", "trackerip","trackerPort", "chunkSize", "receiveStreamSize" };
            string[] value = { this.pluginPath, this.streamType, this.videoDir, this.maxClient.ToString(), this.vlcStreamPort.ToString(), this.SLisPort.ToString(), this.sLisPortup.ToString(), this.Dataport.ToString(), this.Dataportup.ToString(), this.ConportBase.ToString(), this.Conportup.ToString(), this.TreeSize.ToString(), this.trackerip,this.trackerPort.ToString(), this.chunkSize.ToString(), this.receiveStreamSize.ToString() };

            store.Add(type, value);

        }

        public void load(string fileName)
        {
            xml load = new xml(fileName, "server", false);

            bool checkLoad = load.load();

            this.pluginPath = load.Read("server", "pluginPath");
            this.streamType = load.Read("server", "streamType");
            this.videoDir = load.Read("server", "videoDir");
            this.maxClient = Convert.ToInt32(load.Read("server", "maxClient"));
            this.vlcStreamPort = Convert.ToInt32(load.Read("server", "vlcStreamPort"));
            this.SLisPort = Convert.ToInt32(load.Read("server", "SLisPort"));
            this.SLisPortup = Convert.ToInt32(load.Read("server", "sLisPortup"));
            this.Dataport = Convert.ToInt32(load.Read("server", "Dataport"));
            this.Dataportup = Convert.ToInt32(load.Read("server", "dataportup"));
            this.ConportBase = Convert.ToInt32(load.Read("server", "ConportBase"));
            this.Conportup = Convert.ToInt32(load.Read("server", "conportup"));
            this.TreeSize = Convert.ToInt32(load.Read("server", "TreeSize"));
            //this.serverip = load.Read("server", "serverip");
            this.trackerip = load.Read("server", "trackerip");
            this.trackerPort = Convert.ToInt32(load.Read("server", "trackerPort"));
            this.receiveStreamSize = Convert.ToInt32(load.Read("server", "receiveStreamSize"));
            this.chunkSize = Convert.ToInt32(load.Read("server", "chunkSize"));
        }


    }
}
