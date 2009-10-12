using System;
using System.Collections.Generic;
using System.Linq;
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
        private int Dataport;
        private int ConportBase;
        private int TreeSizes;
        private string serverip;
        private string trackerip;


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
            Dataport = 0;
            ConportBase = 0;
            TreeSize = 0;
            serverip = "";
            trackerip = "";
            receiveStreamSize = 0;
            chunkSize = 0;
        }

        public ServerConfig(string pluginPath, string streamType, string videoDir, int maxClient, int vlcStreamPort, int SLisPort, int Dataport, int ConportBase, int TreeSizes, string serverip, int receiveStreamSize, int chunkSize, string trackerip)
        {
            this.pluginPath = pluginPath;
            this.streamType = streamType;
            this.videoDir = videoDir;
            this.maxClient = maxClient;
            this.vlcStreamPort = vlcStreamPort;
            this.SLisPort = SLisPort;
            this.Dataport = Dataport;
            this.ConportBase = ConportBase;
            this.TreeSize = TreeSize;
            this.serverip = serverip;
            this.chunkSize = chunkSize;
            this.receiveStreamSize = receiveStreamSize;
            this.trackerip = trackerip;
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
        public int Dport
        {
            get { return Dataport; }
            set { Dataport = value; }
        }
        public int CportBase
        {
            get { return ConportBase; }
            set { ConportBase = value; }
        }

        public int TreeSize
        {
            get { return TreeSizes; }
            set { TreeSizes = value; }
        }

        public string Serverip
        {
            get { return serverip; }
            set { serverip = value; }
        }
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

        public void save(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            xml store = new xml(fileName, "server");
            store.Add("server", "pluginPath", this.pluginPath);
            store.Add("server", "streamType", this.streamType);
            store.Add("server", "videoDir", this.videoDir);
            store.Add("server", "maxClient", this.maxClient.ToString());
            store.Add("server", "vlcStreamPort", this.vlcStreamPort.ToString());
            store.Add("server", "SLisPort", this.SLisPort.ToString());
            store.Add("server", "Dataport", this.Dataport.ToString());
            store.Add("server", "ConportBase", this.ConportBase.ToString());
            store.Add("server", "TreeSize", this.TreeSize.ToString());
            store.Add("server", "serverip", this.serverip);
            store.Add("server", "trackerip", this.trackerip);
            store.Add("server", "chunkSize", this.chunkSize.ToString());
            store.Add("server", "receiveStreamSize", this.receiveStreamSize.ToString());
        }

        public void load(string fileName)
        {
            xml load = new xml(fileName, "server");

            this.pluginPath = load.Read("server", "pluginPath");
            this.streamType = load.Read("server", "streamType");
            this.videoDir = load.Read("server", "videoDir");
            this.maxClient = Convert.ToInt32(load.Read("server", "maxClient"));
            this.vlcStreamPort = Convert.ToInt32(load.Read("server", "vlcStreamPort"));
            this.SLisPort = Convert.ToInt32(load.Read("server", "SLisPort"));
            this.Dataport = Convert.ToInt32(load.Read("server", "Dataport"));
            this.ConportBase = Convert.ToInt32(load.Read("server", "ConportBase"));
            this.TreeSize = Convert.ToInt32(load.Read("server", "TreeSize"));
            this.serverip = load.Read("server", "serverip");
            this.trackerip = load.Read("server", "trackerip");
            this.receiveStreamSize = Convert.ToInt32(load.Read("server", "receiveStreamSize"));
            this.chunkSize = Convert.ToInt32(load.Read("server", "chunkSize"));
        }


    }
}
