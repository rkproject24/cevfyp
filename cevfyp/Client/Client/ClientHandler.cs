using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ClassLibrary;

using Analysis;

namespace Client
{
    public class ClientHandler
    {
        //===============upload variable=============
        string CLIENTIP;// = "127.0.0.1";
        //const int maxNullCount = 3;
        //const int READTIMEOUT = 2000;

        //static int PeerListenPort = 1100;
        int PeerListenPort;
        int max_peer;
        bool uploading = false; //indicate whether upload thread is started
        TcpListener listenPeer;
        UploadPortHandler upPorth;
        Thread listenerThread;

        //===================================
        static int RECEIVE_RANGE = 100;
        static int SEND_PORT_READ_TIMEOUT = 5000;
        static int SEND_PORT_WEITE_TIMEOUT = 3000;

        static int PULL_PORT_WRITE_TIMEOUT = 2000;
        static int PULL_PORT_READ_TIMEOUT = 2000;
        static int PULL_STREAM_WRITE_TIMEOUT = 2000;
        static int PULL_STREAM_READ_TIMEOUT = 2000;

        static int PULL_STREAM_CONNECT_TIMEOUT = 2000;
        static int PULL_PORT_CONNECT_TIMEOUT = 2000;

        static int VLC_WRITE_TIMEOUT = 1500;

        static int PULL_FAIL_NUM = 3;
        public string configPath = "C:\\ClientConfig";

        public int treeNO = 0;
        //int chunkList_wIndex = 0;  //write index
        int[] chunkList_wIndex;
        int chunkList_rIndex = 0;  //read index
        int tempSeq = 0, playingSeq = 0;
        int tempResult;
        int virtualServerPort = 0;   //virtual server broadcast port number
        int avgSeqNum = 0;

        //===============download & play statistic=============
        int MissPlayChunk=0, PlayedChunk=0;
        int[] ReconMiss;
        int[] PushMiss;
        int[] WaitMiss;
        int[] BeforeRconPlayingSeq;
        int[] AfterRconPlayingSeq;
        int[] ReconCount;
        int[] ChunkToPlayBuffer;
        int[] ReceiveChunk;
        int[] ReceiveNull;

        //===============Pull statistic=============
        int PullMissChunk=0;
        int NoPull = 0;
        int RecoverNotFind = 0;
        int RecoverFind = 0;
        int RecoverToPlayBuffer = 0;
        int RecoverLateToPlayBuffer = 0;
        int RecoverChunkInBufferAlready = 0;
        int RecoverToUpLoadBuffer = 0;
        int RecoverLateToUpLoadBuffer = 0;
        

        bool serverConnect = false;
        bool vlcConnect = false;
        bool checkClose = false;
        bool idChange = false;

        bool firstRun = true;
        bool checkStart = false;

        string trackerIp = null;
        public VlcHandler vlc;
        ChunkHandler ch;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        List<Chunk> chunkList;
        List<TcpClient> ClientC;
        List<TcpClient> ClientD;
        List<List<Chunk>> treeChunkList;
        List<int> missingChunk;
        List<List<int>> treeSeqMap;
        List<PeerNode> pullPnList;

        int[] treeCLWriteIndex;
        int[] treeCLReadIndex;
        int[] treeCLCurrentSeq;
        int[] treeCLRealInSeq;
        int[] treeCLLastReadSeq;
        int[] treeReconnectState;
        int[] treeCLStartRead;
        int[] lastMissSeq;
        bool[] firstWrite;

        int missCLWriteIndex;
        int missCLReadIndex;
        bool missCLRWRevrse = false, firstMiss = false;
        List<int> missChunkList;

        List<Thread> receiveChunkThread;
        List<Thread> receiveControlThread;
        List<Thread> graphThreads;
        Thread broadcastVlcStreamingThread;
        Thread updateChunkListThread;
        Thread updateFmIDThread;
        Thread requireChunkThread;
        Thread findAvgSeqNumThread;
        Thread sendStat;

        TcpListener server;
        NetworkStream localvlcstream = null;
        TcpClient pullClient = null;
        NetworkStream pullStream = null;
        private ClientForm mainFm;
        public delegate void UpdateTextCallback(string message);
        //public delegate void UpdateGraphCallback(plotgraph graphdata);
        //public delegate void StopVlcPlayer();

        private PeerHandler peerh;
        ClientConfig cConfig = new ClientConfig();
        StatisticHandler statHandler;
        public string[] waitMsg;

        public int[] downloadSpeed;
        bool[] readWriteReverse;
        bool[] fastReconnectReq;
        bool[] fr_asking;

        public int currentCh;
        //int maxChannel;
        int firstAttempTime = 6;
        bool playStarted = false;
        public int bitrate;

        public ClientHandler(ClientForm mainFm)
        {
            this.mainFm = mainFm;
            vlc = new VlcHandler(configPath);
            ch = new ChunkHandler();
            statHandler = new StatisticHandler();

            //cConfig.load("C:\\ClientConfig");

            //mainFm.tbhostIP.Text = TcpApps.LocalIPAddress();
            //mainFm.tbServerIp.Text = cConfig.Trackerip;

            //((LoggerFrm)mainFm.downloadFrm).tbIP.Text = cConfig.Trackerip;
            ((LoggerFrm)mainFm.uploadFrm).tbIP.Text = TcpApps.LocalIPAddress();

            string[] xmlList = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");
            foreach (string xmlfile in xmlList)
            {
                File.Delete(xmlfile);
            }

        }


        public void initialize()
        {
            firstRun = true;
            checkStart = false;

            MissPlayChunk = 0;
            PlayedChunk = 0;
            PullMissChunk = 0;
            NoPull = 0;
            RecoverNotFind = 0;
            RecoverFind = 0;
            RecoverToPlayBuffer = 0;
            RecoverLateToPlayBuffer = 0;
            RecoverChunkInBufferAlready = 0;
            RecoverToUpLoadBuffer = 0;
            RecoverLateToUpLoadBuffer = 0;
            avgSeqNum = 0;

            chunkList = new List<Chunk>(cConfig.ChunkCapacity);
            //currentCh=0;
            //maxChannel=0;
            chunkList_wIndex = new int[treeNO];
            downloadSpeed = new int[treeNO];

            treeChunkList = new List<List<Chunk>>(treeNO);
            treeCLWriteIndex = new int[treeNO];
            treeCLReadIndex = new int[treeNO];
            treeCLCurrentSeq = new int[treeNO];
            treeCLRealInSeq = new int[treeNO];
            treeCLLastReadSeq = new int[treeNO];
            treeCLStartRead = new int[treeNO];
            treeReconnectState = new int[treeNO];
            readWriteReverse = new bool[treeNO];
            fastReconnectReq = new bool[treeNO];
            fr_asking = new bool[treeNO];
            waitMsg = new string[treeNO];

            firstWrite = new bool[treeNO];
            lastMissSeq = new int[treeNO];
            PushMiss = new int[treeNO];
            ReconMiss = new int[treeNO];
            AfterRconPlayingSeq = new int[treeNO];
            BeforeRconPlayingSeq = new int[treeNO];
            WaitMiss = new int[treeNO];
            ReconCount = new int[treeNO];
            ChunkToPlayBuffer = new int[treeNO];
            ReceiveChunk = new int[treeNO];
            ReceiveNull = new int[treeNO];

            graphThreads = new List<Thread>(treeNO);
            receiveChunkThread = new List<Thread>(treeNO);
            receiveControlThread = new List<Thread>(treeNO);
            ClientC = new List<TcpClient>(treeNO);
            ClientD = new List<TcpClient>(treeNO);
            missingChunk = new List<int>();
            treeSeqMap = new List<List<int>>(treeNO);
            pullPnList = new List<PeerNode>(treeNO);

            //missCLWriteIndex = new int[treeNO];
            //missCLReadIndex = new int[treeNO];
            missChunkList = new List<int>(cConfig.ChunkCapacity);

            bitrate = 0;
            for (int i = 0; i < cConfig.ChunkCapacity; i++)
            {
                Chunk ck = new Chunk();
                chunkList.Add(ck);

                missChunkList.Add(0);

            }

            for (int i = 0; i < treeNO; i++)
            {
                List<int> seqList = new List<int>(cConfig.ChunkCapacity);

                for (int j = 0; j < cConfig.ChunkCapacity; j++)
                {
                    seqList.Add(0);
                }

                ClientC.Add(null);
                ClientD.Add(null);

                readWriteReverse[i] = false;
                treeSeqMap.Add(seqList);

                treeCLStartRead[i] = -1;
            }

        }

        public string getSelfID(int tree_index)
        {
            return peerh.Selfid;//[tree_index];
        }


        public void getMute()
        {
            if (vlc.getPlayingState())
            {
                int checkMute = vlc.getMute();
                if (checkMute == 0)
                    vlc.setMute(1);
                else
                    vlc.setMute(0);
            }
        }

        public string establishConnect()
        {
            string[] xmlList = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");
            foreach (string xmlfile in xmlList)
            {
                File.Delete(xmlfile);
            }

            //mainFm.tbWriteStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox1), new object[] { "" });
            if (((ControlFrm)mainFm.controlFrm).cbChannel.SelectedIndex == -1)
                return "Please select a channel!";
            else
                currentCh = ((ControlFrm)mainFm.controlFrm).cbChannel.SelectedIndex + 1;
            string response = "";

            //connect tracker
            response = connectToTracker(currentCh );

            //get selfid
            while (!peerh.getSelfid())
                return "No selfID";
            Console.WriteLine("getselfid()="+peerh.Selfid);

            if (response == "OK")
            {
                //bool[] check = new bool[treeNO];

                //open upload listen port
                CLIENTIP = ((LoggerFrm)mainFm.uploadFrm).tbIP.Text.ToString(); //mainFm.tbhostIP.Text.ToString();
                IPAddress uploadipAddr = IPAddress.Parse(CLIENTIP);
                PeerListenPort = TcpApps.RanPort(cConfig.LisPort, cConfig.LisPortup);
                while (true)
                {

                    try
                    {
                        // mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { PeerListenPort.ToString() +"\n"});
                        listenPeer = new TcpListener(uploadipAddr, PeerListenPort);
                        listenPeer.Start(1);
                        break;
                    }
                    catch (Exception ex)
                    {
                        //((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Listen fail:" + PeerListenPort+ "\n"+ex  });
                        Thread.Sleep(20);
                        PeerListenPort = TcpApps.RanPort(cConfig.LisPort, cConfig.LisPortup);
                    }
                }

                Thread[] connectSrcThread = new Thread[treeNO];
                bool[] conSrcThread = new bool[treeNO];
                //startUpload();


                for (int i = 0; i < treeNO; i++)
                {
                    conSrcThread[i] = false;
                    connectSrcThread[i] = new Thread(delegate() { conSrcThread[i] = connectToSources( i, false, true); });
                    connectSrcThread[i].IsBackground = true;
                    connectSrcThread[i].Name = " connectSrcThread_" + i;
                    connectSrcThread[i].Start();
                    Thread.Sleep(30);
                }

                
                for (int i = 0; i < treeNO; i++)
                {
                    int k = 0;
                    while (true)
                    {                     
                        if (!connectSrcThread[i].IsAlive)
                        {
                            if (conSrcThread[i] == true)
                                break;
                            else
                            {
                                connectSrcThread[i] = new Thread(delegate() { conSrcThread[i] = connectToSources(i, false, true); });
                                connectSrcThread[i].IsBackground = true;
                                connectSrcThread[i].Name = " connectSrcThread_" + i;
                                connectSrcThread[i].Start();
                                k++;
                                Thread.Sleep(60);
                                if (k > firstAttempTime)
                                {
                                    for (int j = 0; j < i; j++)
                                    {
                                        sendExit(j);
                                    }
                                    return "connectToSource fail!";
                                }
                            }
                        }
                        Thread.Sleep(10);
                    }
                }

                //if (check)
                //{
               // virtualServerPort = TcpApps.RanPort(cConfig.VlcPortBase, cConfig.VlcPortup);//peerh.Cport11 + cConfig.VlcPortBase;
                serverConnect = true;
                checkClose = false;
                idChange = true;



                response = "OK3";
                //}
            }
            else
                return response;


            if (response == "OK3")
            {
                return response = "";
            }


            return response;

        }

        public bool downChannelList(string tracker)
        {
            if (tracker.Equals(""))
            {
                cConfig.load(configPath);
                this.trackerIp = cConfig.Trackerip;
            }
            peerh = new PeerHandler(this.trackerIp, mainFm);

            int totalch = -1;
            totalch = peerh.getChannelList();
            if (totalch == -1)
                return false;
            else
            {
                ((ControlFrm)mainFm.controlFrm).cbChannel.Items.Clear();
                for (int i = 0; i < totalch; i++)
                    ((ControlFrm)mainFm.controlFrm).cbChannel.Items.Add("CH" + (i + 1));
            }
            return true;
        }
        private string connectToTracker(int channel)
        {
            cConfig.load(configPath);
            if (((LoggerFrm)mainFm.downloadFrm).tbIP.Text.Equals(""))
                ((LoggerFrm)mainFm.downloadFrm).tbIP.Text = cConfig.Trackerip;
            peerh.TrackIp = trackerIp;
            this.trackerIp = ((LoggerFrm)mainFm.downloadFrm).tbIP.Text;

            peerh.currentCh = channel;
            treeNO = peerh.findTracker();
            if (treeNO != -1)
            {
                initialize();
                return "OK";
            }
            else
            {
                return "Tracker " + trackerIp + " Unreachable!";
            }

        }


        private bool connectToSources( int tree_index, bool reconnectUse, bool downPeerList)
        {
            //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "connectToSources CH" + channel + " T" + tree_index +"\n"});
            //peerh.currentCh = channel;
            if (downPeerList)
            {
                if (!peerh.downloadPeerlist(tree_index, false))
                    return false;
            }
            bool conPeer = false;
            
            conPeer = peerh.connectPeers(tree_index + 1, reconnectUse);

            if (conPeer)
            {
                peerh.PeerListenPort = this.PeerListenPort;

                ClientC[tree_index] = peerh.getControlConnect(tree_index);
                ClientD[tree_index] = peerh.getDataConnect(tree_index);


                if (ClientD[tree_index] == null)
                {
                    //mainFm.rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Client D\n" });
                    ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Client D remove\n" });
                    peerh.removePeer(peerh.JoinPeer[tree_index], tree_index);
                    return false;

                }
                if (ClientC[tree_index] == null)
                {
                    //mainFm.rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Client C\n" });
                    ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Client C remove\n" });
                    peerh.removePeer(peerh.JoinPeer[tree_index], tree_index);
                    return false;

                }

                //move to register by parent
                //if (!reconnectUse)
                //    peerh.registerToTracker(tree_index, PeerListenPort, peerh.JoinPeer[tree_index].Layer.ToString()); //by vinci: register To Tree in Tracker


                ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "connect to Source OK T" + tree_index + "\n" });
                return true;
            }
            else
            {
                //    peerh.removePeer(peerh.JoinPeer[tree_index], tree_index);
                ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "conPeer=false T:" + tree_index + "\n" });


            }
            return false;



        }

        private void reconnection(int tree_index, string errType)
        {
            //peerh.updateTotalChunkNull(tree_index, peerh.JoinPeer[tree_index]);//update the totalChunkNull to xml
            ///errType: timeout / other

            //mainFm.rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "reconnect"});
            //int ConnectToSrcCount = 0;
            while (true)
            {
                //try
                //{
                // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "reconnecting T" + tree_index + "\n" });
                if (checkClose)
                    break;

                if (!peerh.hasPeer(tree_index))
                {
                    //  ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Has NO Peer T" + tree_index + "\n" });
                    if (!peerh.downloadPeerlist( tree_index, true))
                    {
                        //  ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "reconnect downloadPeerlist fail T" + tree_index + "\n" });
                        Thread.Sleep(20);
                        continue;
                    }
                    //else
                    //    ConnectToSrcCount = 0;
                }

                // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Peerlist T" + tree_index + " OK\n" });
                if (!connectToSources( tree_index, true, false))
                {
                    // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "reconnect Sources fail T" + tree_index + "\n" });
                    //ConnectToSrcCount++;
                    //if (ConnectToSrcCount > 100)
                    //    peerh.downloadPeerlist(tree_index, true);
                    Thread.Sleep(20);
                    continue;
                }
                //ConnectToSrcCount = 0;
                break;
            }
            //catch (Exception ex)
            //{
            //    ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "reconnection loop err T"+tree_index + ex + "\n" });
            //}

            //}
            //treeReconnectState[tree_index] = 0;
            if (!checkClose)
            {
                idChange = true;
                ReconCount[tree_index] += 1;
            }

        }


        public void startThread()
        {
            startUpload();
            tempSeq = 0;

            requireChunkThread = new Thread(new ThreadStart(pullChunk));
            requireChunkThread.IsBackground = true;
            requireChunkThread.Name = "require_Chunk";
            requireChunkThread.Start();

            for (int i = 0; i < treeNO; i++)
            {
                List<Chunk> chunkLists = new List<Chunk>(cConfig.ChunkCapacity);
                downloadSpeed[i] = 0;

                for (int j = 0; j < cConfig.ChunkCapacity; j++)
                {
                    Chunk ck = new Chunk();
                    ck.seq = 0;
                    chunkLists.Add(ck);
                }
                treeChunkList.Add(chunkLists);

                Thread DRecvThread = new Thread(delegate() { receiveTreeChunk(i); });
                DRecvThread.IsBackground = true;
                DRecvThread.Name = " DRecv_handle_" + i;
                DRecvThread.Start();
                Thread.Sleep(20);
                receiveChunkThread.Add(DRecvThread);

                Thread CRecvThread = new Thread(delegate() { receiveTreeControl(i); });
                CRecvThread.IsBackground = true;
                CRecvThread.Name = " CRecv_handle_" + i;
                CRecvThread.Start();
                Thread.Sleep(20);
                receiveControlThread.Add(CRecvThread);


            }
            // startUpload();

            //updateChunkListThread = new Thread(new ThreadStart(updateTreeChunkList));
            //updateChunkListThread.IsBackground = true;
            //updateChunkListThread.Name = "update_ChunkList";
            //updateChunkListThread.Start();

            broadcastVlcStreamingThread = new Thread(new ThreadStart(broadcastVlcStreaming));
            broadcastVlcStreamingThread.IsBackground = true;
            broadcastVlcStreamingThread.Name = "broadcast_VlcStreaming";
            broadcastVlcStreamingThread.Start();

            findAvgSeqNumThread = new Thread(new ThreadStart(findAvgSeqNum));
            findAvgSeqNumThread.IsBackground = true;
            findAvgSeqNumThread.Name = "find_avgSeqNum";
            findAvgSeqNumThread.Start();

            updateFmIDThread = new Thread(new ThreadStart(updateFmID));
            updateFmIDThread.IsBackground = true;
            updateFmIDThread.Name = "update_FMID";
            updateFmIDThread.Start();

            sendStat = new Thread(new ThreadStart(updatelog));
            sendStat.IsBackground = true;
            sendStat.Name = "update_log";
            sendStat.Start();

            //move to broadcast_VlcStreaming
            if (virtualServerPort == 0)
                Thread.Sleep(100);
            vlc.play(((PlaybackFrm)mainFm.playFrm), virtualServerPort);
            int soundvalue=((ControlFrm)mainFm.controlFrm).trackBar1.Value;
            vlc.setVolume(soundvalue*20);

         
        }



        public void startUpload()
        {
            if (!uploading)
            {
                //((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Start: \n" });
                //CLIENTIP = ((LoggerFrm)mainFm.uploadFrm).tbIP.Text.ToString(); //mainFm.tbhostIP.Text.ToString();

                //start uploading thread
                this.max_peer = cConfig.MaxPeer;
                upPorth = new UploadPortHandler(cConfig, CLIENTIP, mainFm, treeNO, this, statHandler);
                upPorth.startTreePort();


                //localAddr = IPAddress.Parse(sConfig.Serverip);
                listenerThread = new Thread(new ThreadStart(listenForClients));
                listenerThread.IsBackground = true;
                listenerThread.Name = " listen_for_peers";
                listenerThread.Start();

                uploading = true;
            }
        }

        private void updateFmID()
        {
            string str="";
            while (true)
            {
                
                if (idChange)
                {
                     str = "";
                    //string idStr = "";
                    ////mainFm.Text = "Client:";
                    //mainFm.BeginInvoke(new UpdateTextCallback(mainFm.UpdateLabel5), new object[] { "" });
                    ////((ControlFrm)mainFm.controlFrm).lbId.BeginInvoke(new UpdateTextCallback(mainFm.UpdateLabel5), new object[] { "" });
                    //for (int i = 0; i < peerh.Selfid.Length; i++)
                    //{
                    //    idStr += peerh.Selfid[i];
                    //}
                    //((ControlFrm)mainFm.controlFrm).lbId.BeginInvoke(new UpdateTextCallback(mainFm.UpdateLabel5), new object[] { idStr });
                   // mainFm.BeginInvoke(new UpdateTextCallback(mainFm.UpdateLabel5), new object[] { "Client:" + peerh.Selfid });

                    for (int i = 0; i < treeNO;i++ )
                    {
                        str += peerh.JoinPeer[i].Id;
                        str += ",";
                    }
                    mainFm.BeginInvoke(new UpdateTextCallback(mainFm.UpdateLabel5), new object[] {"Client:" + peerh.Selfid+ " ==>" + str });

                    // mainFm.Text += peerh.Selfid[0] + ",";

                    idChange = false;
                }

                Thread.Sleep(50);

            }


        }

        private void findAvgSeqNum()
        {
            while (true)
            {
                int totalSeq = 0;

                try
                {
                    for (int i = 0; i < treeNO; i++)
                    {
                        totalSeq += treeCLCurrentSeq[i];
                    }

                    avgSeqNum = totalSeq / treeNO;
                }
                catch(Exception ex)
                {
                    printOnDL("find avg error\n");
                }
                Thread.Sleep(2000);
            }

        }

        private void receiveTreeChunk(int tree_index)
        {
            TcpClient clientD = null;
            NetworkStream streams = null;
            BinaryFormatter bf = new BinaryFormatter();
            byte[] responseMessage = new byte[cConfig.ChunkSize];
            Chunk streamingChunk;
            bool firstConnectRound;
        
            while (serverConnect)
            {
                int checkNullCount = 0;
                try
                {
                    if (ClientD[tree_index] != null && treeReconnectState[tree_index] == 0)
                    {
                        clientD = ClientD[tree_index];
                        clientD.ReceiveBufferSize = cConfig.ChunkSize;
                        clientD.NoDelay = true;
                        streams = clientD.GetStream();
                        
                        streams.ReadTimeout = cConfig.ReadStreamTimeout;
                        AfterRconPlayingSeq[tree_index] = playingSeq;
                    }
                    else
                    {
                        //*********************Reconnection miss handle****************************
                        if (treeCLCurrentSeq[tree_index] < playingSeq + 20)
                            missChunkHandle(1, tree_index,0);
                           
                        upPorth.setTreeCLState(tree_index, 0);
                        printOnSeqTextBox(tree_index, "Recon..");
                        Thread.Sleep(10);
                        continue;
                    }

                    int receiveCount = 1;
                    DateTime start, end;
                    start = DateTime.Now;
                    firstConnectRound = true;
                    int dataRead;
                    string responseString;
                    string[] type;

                    while (true)
                    {
                        if (ClientD[tree_index] == null)
                        {
                            if (streams != null) streams.Close();
                            if (clientD != null) clientD.Close();

                            upPorth.setTreeCLState(tree_index, 0);
                            printOnSeqTextBox(tree_index, "Recon~");
                            printOnDL("T[" + tree_index + "] D exit~\n");
                            break;
                        }

                        //**************read the stream data***************
                        dataRead = 0;
                        responseString = "";
                        do
                        {
                            dataRead += streams.Read(responseMessage, dataRead, cConfig.ChunkSize - dataRead);
                            responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, dataRead);
                            type = responseString.Split('@');
                            if (type[0].Equals("Wait"))
                                break;
                        } while (dataRead < cConfig.ChunkSize);
                        streams.Flush();

                        //****************Wait message********************
                        if (type[0].Equals("Wait"))
                        {
                            //************Wait miss handle***************************
                            if (treeCLCurrentSeq[tree_index] < playingSeq + 20 && treeCLCurrentSeq[tree_index] != 0)
                                missChunkHandle(2, tree_index,0);

                            //************loop Error handle***************************
                            waitMsg[tree_index] = responseString;
                            if (type[1] == peerh.Selfid)//[tree_index])
                            {
                                printOnDL("loopErr Wait@" + type[1] + "\n");
                                PeerNode tempPn = peerh.JoinPeer[tree_index];
                                peerh.removePeer(tempPn, tree_index);
                                printOnDL("loopErr Remove ID:" + tempPn.Id + "\n");

                                ArgumentException argEx3 = new System.ArgumentException("loopErr");
                                throw argEx3;
                            }

                            upPorth.setTreeCLState(tree_index, 2);
                            downloadSpeed[tree_index] = 0;
                            printOnSeqTextBox(tree_index, "Wait~" + type[1].ToString());
                            firstConnectRound = true;
                            Thread.Sleep(10);
                            continue;
                        }

                        streamingChunk = (Chunk)ch.byteToChunk(bf, responseMessage);  //printOnDL("T[" + tree_index + "]:" + streamingChunk.seq + "\n");

                        ReceiveChunk[tree_index] += 1;

                        //***********************Chunk Null********************
                        if (streamingChunk == null)
                        {
                            ReceiveNull[tree_index] += 1;

                            printOnDL("T[" + tree_index + "] chunk null\n");
                            if (checkNullCount >= cConfig.MaxNullChunk)
                            {
                                printOnDL("chunk null Expcetion\n");
                                ArgumentException argEx = new System.ArgumentException("chunkNull");
                                throw argEx;
                            }

                            upPorth.setTreeCLState(tree_index, 0);
                            Thread.Sleep(10);
                            checkNullCount++;
                            peerh.JoinPeer[tree_index].NullChunkTotal++;
                            peerh.ChunkNullUpdated = true;
                            continue;
                        }
                        checkNullCount = 0;

                        //***********************Push miss handle************
                        //if (((ControlFrm)mainFm.controlFrm).PullChb.Checked && streamingChunk.seq != treeCLCurrentSeq[tree_index] + treeNO && firstConnectRound == false)
                        if (streamingChunk.seq != treeCLCurrentSeq[tree_index] + treeNO && firstConnectRound == false)
                          missChunkHandle(0, tree_index, streamingChunk.seq);

                        //********************Seqeunce num is out of range************
                        if (streamingChunk.seq < playingSeq - RECEIVE_RANGE)
                        {
                            printOnDL("T[" + tree_index + "]:" + streamingChunk.seq + "<" + playingSeq + "\n");
                            ArgumentException argEx2 = new System.ArgumentException("chunkNull");
                            throw argEx2;
                        }

                        //********************Seqeunce num below the Avg of sequnce num************
                        if (streamingChunk.seq < avgSeqNum - RECEIVE_RANGE)
                        {
                            printOnDL("T[" + tree_index + "]:< AvgSeqNum"+ avgSeqNum+"\n");
                            ArgumentException argEx2 = new System.ArgumentException("chunkNull");
                            throw argEx2;
                        }


                        //*******************First connection setting************
                        if (firstConnectRound)
                        {
                            if (streamingChunk.seq > treeCLCurrentSeq[tree_index])
                            {
                                treeCLCurrentSeq[tree_index] = streamingChunk.seq;
                                //if (streamingChunk.seq > playingSeq)
                                //    chunkList_rIndex = calcPlaybackIndex(streamingChunk.seq);
                            }
                            firstConnectRound = false;
                            //**treeCLStartRead[tree_index] = calcChunkIndex(streamingChunk.seq);
                            //**chunkList_rIndex = calcPlaybackIndex(streamingChunk.seq);
                        }

                        upPorth.setTreeCLState(tree_index, 1);

                        //*******************Set chunk to upload buffer*********************
                        if (streamingChunk.seq > treeCLCurrentSeq[tree_index])
                        {
                            
                            if (uploading)
                                upPorth.setChunkList(streamingChunk, tree_index);
                            treeCLCurrentSeq[tree_index] = streamingChunk.seq;
                        
                        }

                        //*******************Set chunk to playback buffer*********************
                        if (streamingChunk.seq > playingSeq)
                        {
                            int write_index = calcPlaybackIndex(streamingChunk.seq);
                            chunkList[write_index] = streamingChunk;
                            chunkList_wIndex[tree_index] = write_index;
                            if (!firstWrite[tree_index])
                                firstWrite[tree_index] = true;

                            ChunkToPlayBuffer[tree_index] += 1;
                        }

                        printOnSeqTextBox(tree_index, treeCLCurrentSeq[tree_index].ToString());

                        //*******************Speed test********************************************
                        if (receiveCount == 30)
                        {
                            end = DateTime.Now;
                            downloadSpeed[tree_index] = Convert.ToInt32(plotgraph.speedCalculate(start, end, cConfig.ChunkSize * 30 * 8) / 1000);
                            //((LoggerFrm)mainFm.downloadFrm).lbSpeed.BeginInvoke(new UpdateTextCallback(mainFm.UpdateDownloadSpeed), new object[] { Math.Round(speed).ToString() });
                            statHandler.updateCurve(start, end, cConfig.ChunkSize * 30 * 8, tree_index, treeNO, "download");
                            receiveCount = 1;
                            start = DateTime.Now;
                        }
                        receiveCount++;

                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    if (streams != null) streams.Close();
                    if (clientD != null) clientD.Close();
                    upPorth.setTreeCLState(tree_index, 0);
                    downloadSpeed[tree_index] = 0;

                    if (!checkClose)
                    {
                        BeforeRconPlayingSeq[tree_index] = treeCLCurrentSeq[tree_index];
                        //peerh.updateTotalChunkNull(tree_index, peerh.JoinPeer[tree_index]);//update the totalChunkNull to xml

                        if (ex.ToString().Contains("disposed object"))
                        {
                            //treeReconnectState[tree_index] = 2;
                            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] D other exception(disposed object)\n" });
                            Thread.Sleep(10);
                            continue;
                        }

                        if (ex.ToString().Contains("period of time"))
                        {
                            peerh.JoinPeer[tree_index].NullChunkTotal += 10;
                            peerh.ChunkNullUpdated = true;
                        }
                        if (ex.ToString().Contains("period of time") || ex.ToString().Contains("chunkNull") || ex.ToString().Contains("loopErr"))
                        {
                            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] D timeout\n" });
                            treeReconnectState[tree_index] = 1;
                        }
                        else
                        {
                            treeReconnectState[tree_index] = 2;
                            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] D other exception \n" });
                        }
                    }
                    if (checkClose)
                        printOnSeqTextBox(tree_index, "close ok");
                }
                Thread.Sleep(10);
            } //end while loop

        }

        private void receiveTreeControl(int tree_index)
        {
            TcpClient clientC = null;
            NetworkStream stream = null;
            byte[] sMessage = new byte[4];
            byte[] responseMessage = new byte[4];
            
            int videoBitRate = 0;
            while (true)
            {
                try
                {
                    if (ClientC[tree_index] != null)
                    {
                        clientC = ClientC[tree_index];
                        clientC.NoDelay = true;
                        stream = clientC.GetStream();
                        //stream.ReadTimeout = cConfig.ReadStreamTimeout;
                       // stream.WriteTimeout = cConfig.ReadStreamTimeout;
                    }

                    while (true)
                    {
                        if (treeReconnectState[tree_index] != 0 && !checkClose)
                        {
                            sMessage = System.Text.Encoding.ASCII.GetBytes("Exit");
                            stream.Write(sMessage, 0, sMessage.Length);

                            stream.Close();
                            clientC.Close();


                            if (ClientD[tree_index] != null)
                            {
                                ClientD[tree_index].Close();
                                ClientD[tree_index] = null;
                            }

                            if (ClientC[tree_index] != null)
                            {
                                ClientC[tree_index].Close();
                                ClientC[tree_index] = null;
                            }

                            if (treeReconnectState[tree_index] == 1)
                                reconnection(tree_index, "timeout");
                            else
                                reconnection(tree_index, "other");

                            treeReconnectState[tree_index] = 0;

                            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] C exit~\n" });

                            break;
                        }

                        if (treeReconnectState[tree_index] == 0 && !checkClose)
                        {
                            sMessage = System.Text.Encoding.ASCII.GetBytes("Wait");
                            stream.Write(sMessage, 0, sMessage.Length);

                            //--------------get the  bit rate message
                            //byte[] responsePeerMsg = new byte[4];
                            //stream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                            //int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);
                            //byte[] responsePeerMsg2 = new byte[MsgSize];
                            //stream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                            //string str = ByteArrayToString(responsePeerMsg2);
                            //string[] messages = str.Split('@');

                            //if (messages[0] == "bitRate")
                            //{
                            //    this.bitrate = Int32.Parse(messages[1]);
                            //    if (videoBitRate != bitrate)
                            //    {
                            //        videoBitRate = bitrate;
                            //        //printOnDL_PULL(videoBitRate + "\n");
                            //    }
                            //}
                            
                            Thread.Sleep(20);
                            continue;
                        }
                        Thread.Sleep(20);
                    }
                }
                catch (Exception ex)
                {
                    if (stream != null) stream.Close();
                    if (clientC != null) clientC.Close();

                    
                    if (ClientD[tree_index] != null)
                    {
                        ClientD[tree_index].Close();
                        ClientD[tree_index] = null;
                    }

                    if (ClientC[tree_index] != null)
                    {
                        ClientC[tree_index].Close();
                        ClientC[tree_index] = null;
                    }

                    if (!checkClose)
                        ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] C exit\n"  });
                    else
                        ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "]  close\n" });

                    if (!checkClose)
                    {
                        reconnection(tree_index, "other");
                        treeReconnectState[tree_index] = 0;
                        ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] C reconnted\n" });
                    }

                   // upPorth.setTreeCLState(tree_index, 0);

                  
                }

                Thread.Sleep(10);
            }
        }

        private void broadcastVlcStreaming()
        {
            TcpClient client = null;
            bool ranPort = false;
            //bool vlcRestart = false;
          //  bool checkStart = false;
          //  bool firstRun = true;
            Thread vlcRestartTh = null;
           // int tempMinRead = 0;
            while (true)
            {
                if (checkClose)
                    break;
                try
                {
                    if (!ranPort)
                    {
                        while (true)
                        {
                            try
                            {
                                virtualServerPort = TcpApps.RanPort(cConfig.VlcPortBase, cConfig.VlcPortup);//peerh.Cport11 + cConfig.VlcPortBase;
                                server = new TcpListener(localAddr, virtualServerPort);
                                server.Start();
                                ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "virtualServerPort:" + virtualServerPort + "\n" });
                                ranPort = true;
                                break;
                            }
                            catch
                            {
                                Thread.Sleep(10);
                            }
                        }
                    }
                    else
                    {
                        server.Start();
                        //Thread.Sleep(400);
                        ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "vlc start:" + virtualServerPort + "\n" });
                    
                    }


                    client = server.AcceptTcpClient();
                    ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "vlc join:" + virtualServerPort + "\n" });
                    //if (vlcRestart)
                    //{
                    //    vlc.play(((PlaybackFrm)mainFm.playFrm).playPanel, virtualServerPort);
                    //    vlcRestart = false;
                    //}       
                    localvlcstream = client.GetStream();
                    localvlcstream.WriteTimeout = VLC_WRITE_TIMEOUT;
                    
                    byte[] sendMessage = System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-type: application/octet-stream\r\nCache-Control: no-cache\r\n\r\n");
                    localvlcstream.Write(sendMessage, 0, sendMessage.Length);

                    if (client.Connected == true) vlcConnect = true;

                    byte[] sendClientChunk;
                    playStarted = false;

                    //********First run,setup the read index of the playback buffer.************
                    //if (firstRun)
                    //{
                    //    while (true)
                    //    {
                    //        for (int i = 0; i < treeNO; i++)
                    //        {
                    //            if (firstWrite[i])
                    //            {
                    //                checkStart = true;
                    //                if(chunkList_wIndex[i]<=chunkList_rIndex)
                    //                  chunkList_rIndex = chunkList_wIndex[i];
                    //            }
                    //            else
                    //                checkStart = false;
                    //        }

                    //        if (checkStart)
                    //        {
                    //            firstRun = false;
                    //            break;
                    //        }

                    //        Thread.Sleep(10);
                    //    }
                    //}


                    while (client.Connected == true && vlcConnect)
                    {
                        if (checkbuff())
                        {
                            if (chunkList[chunkList_rIndex].seq != 0 && chunkList[chunkList_rIndex].seq > playingSeq)
                            {

                                sendClientChunk = new byte[chunkList[chunkList_rIndex].bytes];
                                Array.Copy(chunkList[chunkList_rIndex].streamingData, 0, sendClientChunk, 0, chunkList[chunkList_rIndex].bytes);
                                localvlcstream.Write(sendClientChunk, 0, sendClientChunk.Length);
                                playingSeq = chunkList[chunkList_rIndex].seq;

                                ((ControlFrm)mainFm.controlFrm).lbPlaySeq.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { chunkList_rIndex.ToString() + ":" + playingSeq });
                                //mainFm.tbReadStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { chunkList_rIndex.ToString() + ":" + playingSeq });
                                PlayedChunk += 1;
                                // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "PC="+totalPlayChunk+"["+chunkList_rIndex+"]["+playingSeq+"]\n" });
                         
                            }
                            else
                            {
                                MissPlayChunk += 1;
                                //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "MPC=" + MissPlayChunk + " [" + chunkList_rIndex + "]:" + chunkList[chunkList_rIndex].seq + "\n" });
                         
                            }

                            if (chunkList_rIndex == cConfig.ChunkCapacity - 1)
                                chunkList_rIndex = 0;
                            else
                                chunkList_rIndex += 1;

                        }
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    if (localvlcstream != null)localvlcstream.Close();
                    if (client != null) client.Close();

                   //server.Stop();
                   // server = null;

                    if (!checkClose)
                    {
                        ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "vlcTimeout\n" });
                        //vlc.stop();
                        //Thread.Sleep(50);
                        ((ControlFrm)mainFm.controlFrm).lbPlaySeq.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { "reinitial VLC" });
                        
                        //ranPort = false;
                        //virtualServerPort = 0;
                        //while (true)
                        //{
                        //    try
                        //    {
                        //        virtualServerPort = TcpApps.RanPort(cConfig.VlcPortBase, cConfig.VlcPortup);//peerh.Cport11 + cConfig.VlcPortBase;
                        //        server = new TcpListener(localAddr, virtualServerPort);
                        //        server.Start();
                        //        //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "virtualServerPort:" + virtualServerPort + "\n" });
                        //        ranPort = true;
                        //        break;
                        //    }
                        //    catch
                        //    {
                        //        Thread.Sleep(10);
                        //    }
                        //}

                        //vlc.restart(virtualServerPort);
                        if (vlcRestartTh != null)
                        {
                            if (!vlcRestartTh.IsAlive)
                            {
                                vlcRestartTh = new Thread(delegate() { vlc.restart(virtualServerPort); });
                                vlcRestartTh.IsBackground = true;
                                vlcRestartTh.Name = "restartVLC";
                                vlcRestartTh.Start();
                            }
                        }

                       // vlcRestart = true;
                        //((PlaybackFrm)mainFm.playFrm).playPanel.BeginInvoke(Delegate.CreateDelegate(VlcHandler,
                        
                        //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { ex.ToString() });
                    }
                }
                Thread.Sleep(10);
            }

        }

        private void pullChunk()
        {
            int tempTargetSeq = 0, temp_tree_index = 0;
            int pull_port = 0, pull_notFind = 0;
            pullClient = null;
            pullStream = null;
            bool getport = false, getstream = false;
            PeerNode pn = null;
            Random rm = new Random();
            while (true)
            {
                if (((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                {
                    if (getport == false)
                    {                       
                        temp_tree_index = rm.Next(0, treeNO);

                        pn = pullGetPn(temp_tree_index);
                        if (pn == null)
                        {
                            Thread.Sleep(10); continue;
                        }

                        pull_port = pullGetPort(pn, temp_tree_index);
                        if (pull_port == 0)
                        {
                            Thread.Sleep(10); continue;
                        }
                        getport = true;
                    }

                    if (!firstMiss)
                    {
                        Thread.Sleep(10); continue;
                    }

                    if ((!missCLRWRevrse && missCLReadIndex < missCLWriteIndex) || (missCLRWRevrse && missCLReadIndex < cConfig.ChunkCapacity))
                    {
                        tempTargetSeq = missChunkList[missCLReadIndex];

                        if (missCLReadIndex == cConfig.ChunkCapacity - 1)
                        {
                            missCLReadIndex = 0; missCLRWRevrse = false;
                        }
                        else
                            missCLReadIndex += 1;

                        if (tempTargetSeq == 0)
                        {
                            // printOnDL_PULL("get seq error\n");
                            NoPull += 1;
                            Thread.Sleep(10); continue;
                        }

                        int chunkList_indexs = calcPlaybackIndex(tempTargetSeq);
                        if (chunkList[chunkList_indexs].seq == tempTargetSeq || tempTargetSeq < playingSeq)
                        {
                            //printOnDL_PULL("Seq:" + tempTargetSeq + " No pull\n");
                            NoPull += 1;
                            Thread.Sleep(10); continue;
                        }

                        //printOnDL_PULL("Seq:" + tempTargetSeq + " strart_pull\n");

                        if (getstream == false)
                        {
                            if (pullGetStream(pn, pull_port))
                                getstream = true;
                            else
                            {
                                NoPull += 1;
                                getstream = false;
                                getport = false;
                                Thread.Sleep(10); continue;
                            }
                        }

                        string result = pullFindChunk(tempTargetSeq);

                        if (result.Equals("notFind"))
                        {
                            pull_notFind += 1;
                            if (pull_notFind == PULL_FAIL_NUM)
                            {
                                getstream = false;
                                getport = false;
                                pull_notFind = 0;
                                if (pullStream != null) pullStream.Close();
                                if (pullClient != null) pullClient.Close();
                                Thread.Sleep(10); continue;
                            }
                        }

                        if (result.Equals("error"))
                        {
                            if (pullStream != null) pullStream.Close();
                            if (pullClient != null) pullClient.Close();
                            getstream = false;
                            getport = false;
                            Thread.Sleep(10); continue;
                        }
                    }
                    else
                    {  //release the stream for next pull process after the missing chunk list is emtpy
                        if (pullStream != null) pullStream.Close();
                        if (pullClient != null) pullClient.Close();
                        getstream = false;
                        Thread.Sleep(10);
                    }

                } //end if (clicked)
                Thread.Sleep(10);
            } //end while

        }

        private PeerNode pullGetPn(int temp_tree_index)
        {
            try
            {
                if (pullPnList.Count == 0)
                {
                    //for (int i = 0; i < treeNO; i++)
                    //{
                    //    pullPnList.Add(peerh.selectPeer(temp_tree_index, false, peerh.NO_NULLCHUNK));
                    //}
                    pullPnList = peerh.selectPeerList(temp_tree_index, false, treeNO);
                }
                PeerNode pn = pullPnList[0];
                pullPnList.RemoveAt(0);
                printOnDL_PULL("Pull_select_pn T:" + temp_tree_index + "\n");
                return pn;
              
            }
            catch
            {
                return null;
            }
        }

        private int pullGetPort(PeerNode pn, int temp_tree_index)
        {
            TcpClient connectClient = null;
            NetworkStream connectStream = null;
            byte[] portMessage = new byte[4];

            try
            {
                connectClient = new TcpClient();
                IAsyncResult MyResult = connectClient.BeginConnect(pn.Ip, pn.ListenPort, null, null);
                MyResult.AsyncWaitHandle.WaitOne(PULL_PORT_CONNECT_TIMEOUT, true);
                if (!MyResult.IsCompleted)
                {
                    printOnDL_PULL("Pull_Port_Timeout\n");
                    ArgumentException argEx2 = new System.ArgumentException("connection timeout");
                    throw argEx2;
                }
                else if (connectClient.Connected == true)
                {
                    connectStream = connectClient.GetStream();
                    connectStream.ReadTimeout = PULL_PORT_READ_TIMEOUT;
                    connectStream.WriteTimeout = PULL_PORT_WRITE_TIMEOUT;

                    byte[] reqtype = StrToByteArray("chunkReq");
                    connectStream.Write(reqtype, 0, reqtype.Length);
                    connectStream.Read(portMessage, 0, portMessage.Length);

                    if (connectClient != null) connectClient.Close();
                    if (connectStream != null) connectStream.Close();
                    printOnDL_PULL("Pull_Port_OK,ID:" + pn.Id + "\n");
                    return BitConverter.ToInt16(portMessage, 0);
                }

                ArgumentException argEx3 = new System.ArgumentException("connection timeout**");
                throw argEx3;

            }
            catch (Exception ex)
            {
                if (pn != null)
                {
                    pn.NullChunkTotal++;
                    peerh.updateTotalChunkNull(temp_tree_index, pn);//update the totalChunkNull to xml
                }
                printOnDL_PULL("Pull_Port_Fail\n");
                if (connectClient != null) connectClient.Close();
                if (connectStream != null) connectStream.Close();
                return 0;
            }
        }

        private bool pullGetStream(PeerNode pn,int pull_port)
        {
            try
            {
                pullClient = new TcpClient();
                IAsyncResult MyResult2 = pullClient.BeginConnect(pn.Ip, pull_port, null, null);
                MyResult2.AsyncWaitHandle.WaitOne(PULL_STREAM_CONNECT_TIMEOUT, true);
                if (!MyResult2.IsCompleted)
                {
                    printOnDL_PULL("Pull_Stream_Timeout\n");
                    ArgumentException argEx2 = new System.ArgumentException("connection timeout2");
                    throw argEx2;
                }
                else if (pullClient.Connected == true)
                {
                    pullClient.NoDelay = true;
                    pullStream = pullClient.GetStream();
                    pullStream.ReadTimeout = PULL_STREAM_READ_TIMEOUT;
                    pullStream.WriteTimeout = PULL_STREAM_WRITE_TIMEOUT;
                    printOnDL_PULL("Pull_Stream_OK\n");
                    return true;
                }

                ArgumentException argEx3 = new System.ArgumentException("connection timeout2**");
                throw argEx3;
            }
            catch
            {
                printOnDL_PULL("Pull_Stream_Fail\n");
                if (pullStream != null) pullStream.Close();
                if (pullClient != null) pullClient.Close();
                return false;
            }
        }

        private string pullFindChunk(int tempTargetSeq)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] responseMessage = new byte[cConfig.ChunkSize];
            Chunk streamingChunk;
            int dataRead = 0;

            try
            {
                byte[] message = BitConverter.GetBytes(tempTargetSeq);
                pullStream.Write(message, 0, message.Length);
                //pullStream.Flush();
                printOnDL_PULL("Seq:" + tempTargetSeq + " pulling\n");

                do
                {
                    dataRead += pullStream.Read(responseMessage, dataRead, cConfig.ChunkSize - dataRead);
                } while (dataRead < cConfig.ChunkSize);
               // pullStream.Flush();

                streamingChunk = (Chunk)ch.byteToChunk(bf, responseMessage);

                if (streamingChunk == null || streamingChunk.seq == 0)
                {
                    if (streamingChunk == null)
                        printOnDL_PULL("Seq:chunkNull\n");
                    else
                        printOnDL_PULL("Seq:" + tempTargetSeq.ToString() + " NotFind\n");

                    RecoverNotFind += 1;
                    return "notFind";
                }

                RecoverFind += 1;

                int chunkList_index = calcPlaybackIndex(streamingChunk.seq);

                if (streamingChunk.seq > playingSeq)
                {
                    if (streamingChunk.seq != chunkList[chunkList_index].seq)
                    {
                        chunkList[chunkList_index] = streamingChunk;

                        //if (chunkList_index > chunkList_wIndex)
                        //    chunkList_wIndex = chunkList_index;
                        if (chunkList_index > chunkList_wIndex[calcTreeIndex(streamingChunk.seq)])
                            chunkList_wIndex[calcTreeIndex(streamingChunk.seq)] = chunkList_index;

                        printOnDL_PULL("Seq:" + chunkList[chunkList_index].seq + " coverPB\n");
                        RecoverToPlayBuffer += 1;
                    }
                    else
                    {
                        printOnDL_PULL("Seq:" + chunkList[chunkList_index].seq + " Inlist\n");
                        RecoverChunkInBufferAlready += 1;
                    }
                }
                else
                {
                    printOnDL_PULL("Seq:" + streamingChunk.seq + " coverPBLate\n");
                    RecoverLateToPlayBuffer += 1;
                }

                if (uploading)
                {
                    int tIndex = calcTreeIndex(streamingChunk.seq);
                    upPorth.setChunkList2(streamingChunk, tIndex);

                    if (upPorth.getReadingSeq(tIndex) < streamingChunk.seq)
                    {
                        printOnDL_PULL("Seq:" + streamingChunk.seq + " coverUPB\n");
                        RecoverToUpLoadBuffer += 1;
                    }
                    else
                    {
                        printOnDL_PULL("Seq:" + tempTargetSeq + " coverUPBLate\n");
                        RecoverLateToUpLoadBuffer += 1;
                    }
                }

                return "find";
            }
            catch (Exception ex)
            {
                RecoverNotFind += 1;
                printOnDL_PULL("Pull_Error:" + "\n");
                return "error";
            }
        }

        private void addMissChunkToList(int missSeq)
        {
            int write_index = missCLWriteIndex;
            missChunkList[write_index] = missSeq;

            if (write_index == (cConfig.ChunkCapacity - 1))
            {
                missCLWriteIndex = 0;
                missCLRWRevrse = true;
            }
            else
                missCLWriteIndex += 1;

            firstMiss = true;
        }

        private void missChunkHandle(int type,int tree_index,int seqNum)
        {
            // 0= push miss, 1= reconnection miss,2 = wait miss
            int missBase=0;

            if (type == 0)
            {
                missBase = treeCLCurrentSeq[tree_index] + treeNO;
                while (missBase < seqNum)
                {
                    if (lastMissSeq[tree_index] != missBase)
                    {
                        //printOnDL_PULL("T[" + tree_index + "]:" + missBase + " Push miss add\n");
                        if (((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                        {
                            addMissChunkToList(missBase);
                            PullMissChunk += 1;
                      
                        }

                        PushMiss[tree_index] += 1;
                        lastMissSeq[tree_index] = missBase;
                    }

                   // PushMiss[tree_index] += 1;
                    missBase += treeNO;
                    Thread.Sleep(10);
                }
            }
            else
            {
                missBase = treeCLCurrentSeq[tree_index] + treeNO;
                if (lastMissSeq[tree_index] != missBase)
                {
                    //if (((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                    //addMissChunkToList(missBase);

                    lastMissSeq[tree_index] = missBase;
                    //printOnDL_PULL("T[" + tree_index + "]:" + missBase + " ReconMissAdd\n");
                }
                treeCLCurrentSeq[tree_index] += treeNO;
                if (type == 1)
                    ReconMiss[tree_index] += 1;
                else
                    WaitMiss[tree_index] += 1;
            }
        }

        //send exit to upper peer when local close App.
        private void sendExit(int tree_index)
        {
            TcpClient clientC = null;
            NetworkStream stream = null;
            byte[] sMessage = new byte[4];

            try
            {
                clientC = ClientC[tree_index];
                stream = clientC.GetStream();
                sMessage = System.Text.Encoding.ASCII.GetBytes("Exit");
                stream.Write(sMessage, 0, sMessage.Length);
            }
            catch (Exception ex)
            {
               printOnDL("T[" + tree_index + "] Send exit err\n");
            }

            if (stream != null) stream.Close();
            if (clientC != null) clientC.Close();
            ClientD[tree_index] = null;
            ClientC[tree_index] = null;
        }


        private bool checkbuff() // by vinci:TO keep buffer for chunklist
        {
            //by vinci
            //int indexdiff = chunkList_wIndex - chunkList_rIndex;
            int indexdiff;
            int lowest_seq = 2147483647;
            int lowest_wIndex = 0;
            int largest_seq = 0;

            //wait all thread write the fist index
            if (!checkStart)
            {
                while (true)
                {
                    for (int i = 0; i < treeNO; i++)
                    {
                        if (firstWrite[i])
                            checkStart = true;
                        else
                            checkStart = false;
                    }

                    if (checkStart)
                        break;

                    Thread.Sleep(10);
                }
            }

            for (int i = 0; i < treeNO; i++)
            {
                if (chunkList[chunkList_wIndex[i]].seq < lowest_seq)
                {
                    lowest_seq = chunkList[chunkList_wIndex[i]].seq;
                    lowest_wIndex = chunkList_wIndex[i];
                    // printOnDL(chunkList[chunkList_wIndex[i]].seq + "<" + lowest_seq + "\n");
                    if (firstRun)
                        chunkList_rIndex = lowest_wIndex;
                }

              


            }

            firstRun = false;

            if (chunkList_rIndex <= lowest_wIndex)
            {
                indexdiff = lowest_wIndex - chunkList_rIndex;
                //printOnDL("diff= " + indexdiff + "\n");
            }
            else
            {
                indexdiff = lowest_wIndex + cConfig.ChunkCapacity - chunkList_rIndex;
                //printOnDL("diff: " + indexdiff +"\n");
            }

           
            //   ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "checkBuff:"+indexdiff.ToString()+"\n" });

            if (indexdiff > cConfig.StartBuf)
            {
                //if (mainFm.tbStatus.Text != "Playing")
                //  mainFm.tbStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox3), new object[] { "Playing" });
                playStarted = true;
                return true;
            }
            else if (indexdiff > cConfig.ChunkBuf && playStarted)
            {
                //if (indexdiff <= 0)
                //{
                return true;
                //if (mainFm.tbStatus.Text != "Buffering...")
                //  mainFm.tbStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox3), new object[] { "Buffering..." });
                //}
            }
            ((ControlFrm)mainFm.controlFrm).lbPlaySeq.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { "Buffering..." });
            playStarted = false;
            return false;
        }

        //calculate tree_index of chunk
        private int calcTreeIndex(int seqNum)
        {
            return (seqNum - 1) % treeNO;
        }

        //calculate chunk_index of treeChunkList
        private int calcChunkIndex(int seqNum)
        {
            return ((seqNum - 1) / treeNO) % cConfig.ChunkCapacity;
        }

        //calculate chunk_index of playback list
        private int calcPlaybackIndex(int seqNum)
        {
            return (seqNum - 1) % cConfig.ChunkCapacity;
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }

        public void startStatistic(int port)
        {
            statHandler.StatisticListen = port; //(Int32)mainFm.nudStatisticPort.Value;
            statHandler.Enable = true;
            statHandler.setPeerhandler(peerh);
            //upPorth.startUploadStat();
        }

        public void setVolume(int volume)
        {
            if (vlc.getPlayingState())
                vlc.setVolume(volume * 20);
        }

        private void printOnDL(string message)
        {
            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { message });
        }

        private void printOnDL_PULL(string message)
        {
            ((LoggerFrm)mainFm.downloadFrm).rtbpull.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDLPull), new object[] { message });
        }

        private void printOnUL_PULL(string message)
        {
            ((LoggerFrm)mainFm.uploadFrm).rtbpull.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbULPull), new object[] { message });
        }

        private void printOnUL(string message)
        {
            ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { message });
        }

        private void printOnSeqTextBox(int tree_index, string str)
        {
            if (tree_index == 0)
                ((ControlFrm)mainFm.controlFrm).lbTree0.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { str });
            if (tree_index == 1)
                ((ControlFrm)mainFm.controlFrm).lbTree1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { str });
            if (tree_index == 2)
                ((ControlFrm)mainFm.controlFrm).lbTree2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { str });
        }

        private void downloadTxt(string ID,bool Pull,int WM,int RM,int PM,int Recon,int MPC,int PC,int CTPB,int RC,int CN,int TreeNo,int MPO,int chunkSize,int startBuff,int playbackBuffer,int vlcBuffer)
        {
            try
            {
                StreamWriter SW;
                string path = @"Result_DL.txt";

                string name = "ID,Pull,WaitMiss,ReconnectMiss,PushMiss,Reconnection,MissPlayChunk,PlayedChunk,ChunkToPlaybackBuffer,RecivedChunk,ChunkNull,TreeNo,Max portOut,chunkSize,startBuff,playbackBuffer,vlcBuffer(ms)";
                string result = ID + "," + Pull + "," + WM + "," + RM + "," + PM + "," + Recon + "," + MPC + "," + PC + ","
                    + CTPB + "," + RC + "," + CN + "," + TreeNo + "," + MPO + "," + chunkSize + "," + startBuff + "," + playbackBuffer + "," + vlcBuffer;

                if (!File.Exists(path))
                {
                    SW = File.CreateText(path);
                    SW.WriteLine(name);
                    SW.WriteLine(result);
                    SW.Close();
                }
                else
                {
                    SW = File.AppendText(path);
                    SW.WriteLine(result);
                    SW.Close();
                }

                printOnDL("write DLresult ok\n");
            }
            catch (Exception ex)
            {
                printOnDL("write DLresult error\n");
            }
        }

        private void pullTxt(string ID,bool Pull,int PMC,int NoPull,int RNF,int RF,int RTPB,int RLTPB,int RCIBA,int RTUB,int RLTUB)
        {
            try
            {
                StreamWriter SW;
                string path = @"Result_Pull.txt";

                string name = "ID,Pull,PullRequest,NoPull,NotFind,Find,RecoverSuccess,RecoverLate,AlreadyInPlayBuffer,RecoverToUpLoadBufferSuccess,RecoverToUpLoadBufferLate";
                string result = ID + "," + Pull + "," + PMC + "," + NoPull + "," + RNF + "," + RF + "," + RTPB + "," + RLTPB + "," + RCIBA + "," + RTUB + "," + RLTUB;

                if (!File.Exists(path))
                {
                    SW = File.CreateText(path);
                    SW.WriteLine(name);
                    SW.WriteLine(result);
                    SW.Close();
                }
                else
                {
                    SW = File.AppendText(path);
                    SW.WriteLine(result);
                    SW.Close();
                }

                printOnDL("write Pullresult ok\n");
            }
            catch (Exception ex)
            {
                printOnDL("write Pullresult error\n");
            }
        }
        public void updatelog()
        {
            while (true)
            {
                int totalCTPB = 0, totalRM = 0, totalPM = 0, totalWM = 0, totalRecon = 0, totalRC = 0, totalCN = 0;
                for (int i = 0; i < treeNO; i++)
                {
                    //printOnDL("\nT[" + i + "]beforeRecon=" + BeforeRconPlayingSeq[i] + "\n");
                    //printOnDL("afterRecon=" + AfterRconPlayingSeq[i] + "\n");
                    //printOnDL("RconCount=" + ReconCount[i] + "\n");
                    //printOnDL("RconMiss~=" + ReconMiss[i] + "\n");
                    //printOnDL("PushMiss~=" + PushMiss[i] + "\n");
                    //printOnDL("WaitMiss=" + WaitMiss[i] + "\n");
                    //printOnDL("ReceiveCount=" + ChunkToPlayBuffer[i] + "\n");
                    totalRM += ReconMiss[i];
                    totalPM += PushMiss[i];
                    totalWM += WaitMiss[i];
                    totalRecon += ReconCount[i];
                    totalCTPB += ChunkToPlayBuffer[i];
                    totalRC += ReceiveChunk[i];
                    totalCN += ReceiveNull[i];
                }
                statHandler.statLog = totalRM.ToString() + "@" + totalPM.ToString() + "@" + totalWM.ToString() + "@" + totalRecon.ToString() + "@" + totalCTPB.ToString() + "@" + totalRC.ToString() + "@" + totalCN.ToString();
                Thread.Sleep(1500);
            }

        }
      

        public void closeAllThread()
        {
            if (serverConnect == true)
            {
                checkClose = true;
                //Abort pull chunk thread.
                requireChunkThread.Abort();
                //stop upload
                listenPeer.Stop();
                listenerThread.Abort();
                upPorth.closeCDPortThread();
                vlcConnect = false;
                uploading = false;
                server.Stop();
                if(vlc.playing)
                    vlc.stop();
                broadcastVlcStreamingThread.Abort();
                vlcConnect = false;
                //updateChunkListThread.Abort();
                for (int i = 0; i < treeNO; i++)
                {
                    receiveChunkThread[i].Abort();
                    sendExit(i);
                    receiveControlThread[i].Abort();
                    Thread.Sleep(20);
                }
                serverConnect = false;
                receiveChunkThread.Clear();
                receiveControlThread.Clear();

                int totalCTPB = 0, totalRM = 0, totalPM = 0, totalWM = 0, totalRecon = 0, totalRC = 0,totalCN=0 ;
                for (int i = 0; i < treeNO; i++)
                {
                    //printOnDL("\nT[" + i + "]beforeRecon=" + BeforeRconPlayingSeq[i] + "\n");
                    //printOnDL("afterRecon=" + AfterRconPlayingSeq[i] + "\n");
                    //printOnDL("RconCount=" + ReconCount[i] + "\n");
                    //printOnDL("RconMiss~=" + ReconMiss[i] + "\n");
                    //printOnDL("PushMiss~=" + PushMiss[i] + "\n");
                    //printOnDL("WaitMiss=" + WaitMiss[i] + "\n");
                    //printOnDL("ReceiveCount=" + ChunkToPlayBuffer[i] + "\n");
                    totalRM += ReconMiss[i];
                    totalPM += PushMiss[i];
                    totalWM += WaitMiss[i];
                    totalRecon += ReconCount[i];
                    totalCTPB += ChunkToPlayBuffer[i];
                    totalRC += ReceiveChunk[i];
                    totalCN += ReceiveNull[i];
                }
                //statHandler.statLog = totalRM.ToString() + "@" + totalPM.ToString() + "@" + totalWM.ToString() + "@" + totalRecon.ToString() + "@" + totalCTPB.ToString() + "@" + totalRC.ToString() + "@" + totalCN.ToString();
                //printOnDL("\nTotalRconMiss~=" + totalRconMiss + "\n");
                //printOnDL("TotalPushMiss~=" + totalPushMiss + "\n");
                //printOnDL("TotalWaitMiss~=" + totalWaitMiss + "\n");
                //printOnDL("TotalRecover=" + RecoverChunk + "\n");
                //printOnDL("TotalReconnect=" + totalReconnect + "\n");
                //printOnDL("TotalReceive=" + totalReceive + "\n");
                //printOnDL("TotalMissPlayChunk=" + MissPlayChunk + "\n");
                //printOnDL("TotalPlayChunk=" + PlayedChunk + "\n");

                bool pullMode=false;
                 if (((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                pullMode=true;
    
                //downloadTxt(getSelfID(0),pullMode,totalWM,totalRM,totalPM,totalRecon,MissPlayChunk,PlayedChunk,totalCTPB,totalRC,totalCN,treeNO,cConfig.MaxPeer,cConfig.ChunkSize,cConfig.StartBuf,cConfig.ChunkCapacity,vlc.getHttpCaching());
                //pullTxt(getSelfID(0), pullMode,PullMissChunk,NoPull,RecoverNotFind,RecoverFind,RecoverToPlayBuffer,RecoverLateToPlayBuffer,RecoverChunkInBufferAlready,RecoverToUpLoadBuffer,RecoverLateToUpLoadBuffer);
                //upPorth.uploadTxt(getSelfID(0), pullMode);

                
                
                //chunkList_wIndex = 0;
                chunkList_rIndex = 0;
                virtualServerPort = 0;
                playingSeq = 0;
              //chunkList.Clear();
              //  if(pullPnList!=null)
              //  pullPnList.Clear();

                //for (int i = 0; i < treeNO; i++)
                //{
                //    treeChunkList[i].Clear();
                //    treeCLWriteIndex[i] = 0;
                //    treeCLReadIndex[i] = 0;
                //    treeCLCurrentSeq[i] = 0;
                //    treeReconnectState[i] = 0;
                //    readWriteReverse[i] = false;
                //    AfterRconPlayingSeq[i] = 0;
                //    BeforeRconPlayingSeq[i] = 0;
                //    ReconMiss[i] = 0;
                //    PushMiss[i] = 0;
                //    WaitMiss[i] = 0;
                //    ReconCount[i] = 0;
                //    waitMsg[i] = "";
                //}
                firstMiss = false;
            }
        }

        private void listenForClients()
        {
            //((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Start: \n" });
            TcpClient client = null;
            NetworkStream stream = null;
            int total_req_num = 0;
            int req_tree_num = 0;
            int lastPortPair = 0;

            ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "IP:" + listenPeer.LocalEndpoint.ToString() + "\nPort[" + PeerListenPort.ToString() + "]:Listening...\n" });

            while (true)
            {
                try
                {
                    client = listenPeer.AcceptTcpClient();
                    stream = client.GetStream();
                    stream.ReadTimeout = SEND_PORT_READ_TIMEOUT;
                    stream.WriteTimeout = SEND_PORT_WEITE_TIMEOUT;

                    total_req_num = 0;
                    req_tree_num = 0;
                    int tempC_num, tempD_num;

                    byte[] cMessage;
                    byte[] dMessage;

                    byte[] responseMessage = new byte[8];

                    stream.Read(responseMessage, 0, responseMessage.Length);
                    string reqType = ByteArrayToString(responseMessage);

                    if (reqType.Equals("treesReq"))
                    {
                        byte[] responseMessage2 = new byte[4];

                        //client require how many tree
                        stream.Read(responseMessage2, 0, responseMessage2.Length);
                        total_req_num = BitConverter.ToInt16(responseMessage2, 0);

                        for (int i = 0; i < total_req_num; i++)
                        {

                            //which tree ,client want to join.
                            bool sendPort = false;
                            // stream.Read(responseMessage2, 0, responseMessage2.Length);
                            // req_tree_num = BitConverter.ToInt16(responseMessage2, 0);

                            for (int j = lastPortPair; j < max_peer; j++)
                            {
                                if (upPorth.getTreeCListClient(j) == null && upPorth.getTreeDListClient(j) == null)
                                {
                                    tempC_num = upPorth.getTreeCListPort(j);
                                    cMessage = BitConverter.GetBytes(tempC_num);
                                    stream.Write(cMessage, 0, cMessage.Length);
                                    // ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "T[" + (req_tree_num - 1) + "] Cport:" + tempC_num.ToString() + " " });
                                    ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Cport:" + tempC_num + " " });

                                    tempD_num = upPorth.getTreeDListPort(j);
                                    dMessage = BitConverter.GetBytes(tempD_num);
                                    stream.Write(dMessage, 0, dMessage.Length);
                                    ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Dport:" + tempD_num + "\n" });

                                    sendPort = true;
                                    if (lastPortPair == max_peer)
                                        lastPortPair = 0;
                                    else
                                        lastPortPair++;
                                    break;
                                }
                            }
                            if (!sendPort)
                            {
                                for (int j = 0; j < lastPortPair; j++)
                                {
                                    if (upPorth.getTreeCListClient(j) == null && upPorth.getTreeDListClient(j) == null)
                                    {
                                        tempC_num = upPorth.getTreeCListPort(j);
                                        cMessage = BitConverter.GetBytes(tempC_num);
                                        stream.Write(cMessage, 0, cMessage.Length);
                                        // ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "T[" + (req_tree_num - 1) + "] Cport:" + tempC_num.ToString() + " " });
                                        ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Cport:" + tempC_num + " " });

                                        tempD_num = upPorth.getTreeDListPort(j);
                                        dMessage = BitConverter.GetBytes(tempD_num);
                                        stream.Write(dMessage, 0, dMessage.Length);
                                        ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Dport:" + tempD_num + "\n" });

                                        sendPort = true;
                                        if (lastPortPair == max_peer)
                                            lastPortPair = 0;
                                        else
                                            lastPortPair++;
                                        break;
                                    }
                                }
                            }

                            //No port provide, send "000" to client
                            if (sendPort != true)
                            {   // required tree number cant join
                                byte[] cMessage2 = BitConverter.GetBytes(0000);
                                stream.Write(cMessage2, 0, cMessage2.Length);
                                ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "NoPort\n" });

                                byte[] dMessage2 = BitConverter.GetBytes(0000);
                                stream.Write(dMessage2, 0, dMessage2.Length);
                                //((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "NoDPort\n" });
                            }
                        }
                    }

                    if (reqType.Equals("chunkReq"))
                    {
                        int replyPort = upPorth.getReplyChunkPort();
                        byte[] sendMessage = BitConverter.GetBytes(replyPort);
                        stream.Write(sendMessage, 0, sendMessage.Length);
                    }


                }
                catch (Exception ex)
                {
                    if (!checkClose)
                    {
                        if (ex.ToString().Contains("period of time"))
                            ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Join timeout exception..\n" });
                        else
                        ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Join other exception...\n" });
                    }
                    else
                        ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Listen port close\n" });

                    if (stream != null) stream.Close();
                    if (client != null) client.Close();
                }

                if (stream != null) stream.Close();
                if (client != null) client.Close();
                Thread.Sleep(50);
            }
        }

        //private void updateTreeChunkList()
        //{
        //    while (true)
        //    {
        //        if (treeChunkList[0][0].seq != 0)
        //        {
        //            tempSeq = treeChunkList[0][0].seq;
        //            chunkList_rIndex = calcPlaybackIndex(tempSeq); //set the start index of first chunk in chunk list

        //            break;
        //        }

        //        //****
        //        //if (treeCLStartRead[0] != -1)
        //        //{
        //        //    //tempSeq = treeChunkList[0][treeCLStartRead[0]].seq;
        //        //    chunkList_rIndex = calcPlaybackIndex(tempSeq); //set the start index of first chunk in chunk list

        //        //    break;
        //        //}

        //        Thread.Sleep(10);
        //    }

        //    Chunk ck = new Chunk();
        //    ck.seq = 0;
        //    int readIndex = 0, readingSeq = 0;

        //    while (true)
        //    {
        //        for (int i = 0; i < treeNO; i++)
        //        {

        //            //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + i + "]:" + treeCLLastReadSeq[i] + "::" + treeCLRealInSeq[i] + "\n" });

        //            if ((!readWriteReverse[i] && treeCLReadIndex[i] < treeCLWriteIndex[i]) || (readWriteReverse[i] && treeCLReadIndex[i] < cConfig.ChunkCapacity))
        //            {
        //                readIndex = treeCLReadIndex[i];
        //                readingSeq = treeChunkList[i][readIndex].seq;

        //                chunkList_wIndex = calcPlaybackIndex(readingSeq);

        //                if (treeChunkList[i][readIndex].seq != chunkList[chunkList_wIndex].seq && treeChunkList[i][readIndex].seq > playingSeq)
        //                {
        //                    chunkList[chunkList_wIndex] = treeChunkList[i][readIndex];
        //                    // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "S" + i + "W:" + chunkList_wIndex + " R:" + chunkList_rIndex + " \n" });
        //                }

        //                if (treeCLReadIndex[i] == cConfig.ChunkCapacity - 1)
        //                {
        //                    treeCLReadIndex[i] = 0;
        //                    readWriteReverse[i] = false;
        //                }
        //                else
        //                    treeCLReadIndex[i] += 1;
        //            }
        //        }

        //        Thread.Sleep(10);
        //    }
        //}

        //basic search method
        //private int searchChunk(int list_index, int rIndex, int wIndex, int target)
        //{
        //    if (wIndex < rIndex)
        //    {
        //        tempResult = search(list_index, rIndex, cConfig.ChunkCapacity - 1, target);
        //        if (tempResult != -1)
        //            return tempResult;
        //        else
        //        {
        //            tempResult = search(list_index, 0, wIndex, target);
        //            return tempResult;
        //        }
        //    }
        //    else
        //    {
        //        tempResult = search(list_index, rIndex, wIndex - 1, target);
        //        return tempResult;
        //    }
        //}

        //private int search(int list_index, int r_index, int w_index, int target)
        //{
        //    for (; r_index <= w_index; )
        //    {
        //        if (treeChunkList[list_index][r_index].seq == target)
        //            return r_index;

        //        r_index += 1;
        //    }
        //    return -1;
        //}

        //*********
        //private int searchDLMiss(int list_index, int r_index, int w_index, int target)
        //{

        //    for (; r_index <= w_index; )
        //    {
        //        if (treeSeqMap[list_index][r_index] == target)
        //            return r_index;

        //        r_index += 1;
        //    }
        //    return -1;
        //}

        /* private void requireChunk()   //keep one link in any time.
        {
            int tempTargetSeq = 0;
            int check_tree_index = 0;
            int pull_port = 0;
            int pull_fail = 0;
            int dataRead = 0;
            BinaryFormatter bf = new BinaryFormatter();
            byte[] responseMessage = new byte[cConfig.ChunkSize];
            byte[] portMessage = new byte[4];
            TcpClient connectClient = null;
            NetworkStream connectStream = null;
            Chunk streamingChunk;
            bool getport = false, getstream = false;
            PeerNode pn = null;

            while (true)
            {
               

                // if (missingChunk.Count > 0 && ((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                if (((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                {
                    if (getport == false)
                    {
                        while (true)
                        {
                            if (!((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                                break;

                            Random rm = new Random();
                            int temp_tree_index = rm.Next(0, treeNO);
                            printOnDL_PULL("Pull_select_pn T:" + temp_tree_index + "\n");

                            pn = peerh.selectPeer(temp_tree_index, false, peerh.NO_NULLCHUNK);

                            if (pn == null || pn.Id == peerh.Selfid[temp_tree_index])  //peerlist is using by other process.
                            {

                                printOnDL_PULL("Pull_selectPn=null\n");
                                Thread.Sleep(10);
                                continue;
                            }

                            // connect to peer ask port to pull chunk
                            try
                            {
                                connectClient = new TcpClient();
                                IAsyncResult MyResult = connectClient.BeginConnect(pn.Ip,pn.ListenPort, null, null);
                                MyResult.AsyncWaitHandle.WaitOne(1000, true);
                                if (!MyResult.IsCompleted)
                                {
                                    printOnDL_PULL("Pull_Port_Timeout\n");
                                    ArgumentException argEx2 = new System.ArgumentException("connection timeout");
                                    throw argEx2;
                                }

                                else if (connectClient.Connected == true)
                                {
                                    connectStream = connectClient.GetStream();
                                    connectStream.ReadTimeout = GET_CHUNK_TIMEOUT;
                                    connectStream.WriteTimeout = 2000;


                                    byte[] reqtype = StrToByteArray("chunkReq");
                                    connectStream.Write(reqtype, 0, reqtype.Length);
                                    //Thread.Sleep(10);
                                    connectStream.Read(portMessage, 0, portMessage.Length);
                                    pull_port = BitConverter.ToInt16(portMessage, 0);


                                    if (connectClient != null)
                                        connectClient.Close();
                                    if (connectStream != null)
                                        connectStream.Close();

                                }

                            }
                            catch (Exception ex)
                            {
                                if (pn != null)
                                {
                                    pn.NullChunkTotal++;
                                    peerh.updateTotalChunkNull(check_tree_index, pn);//update the totalChunkNull to xml
                                }
                                printOnDL_PULL("Pull_Port_Fail\n");
                                if (connectClient != null)
                                    connectClient.Close();
                                if (connectStream != null)
                                    connectStream.Close();

                                Thread.Sleep(10);
                                continue;
                            }

                            printOnDL_PULL("Pull_Port_ID:" + pn.Id + "\n");

                            getport = true;
                            break;
                        }

                        if (getport == false)
                        {
                            if (connectClient != null)
                                connectClient.Close();
                            if (connectStream != null)
                                connectStream.Close();
                            Thread.Sleep(10);
                            continue;
                        }

                    }


                    if (getstream == false)
                    {
                        try
                        {
                            connectClient = new TcpClient();
                            IAsyncResult MyResult2 = connectClient.BeginConnect(pn.Ip, pull_port, null, null);
                            MyResult2.AsyncWaitHandle.WaitOne(1000, true);
                            if (!MyResult2.IsCompleted)
                            {
                                printOnDL_PULL("Pull_Stream_Timeout\n");
                                ArgumentException argEx2 = new System.ArgumentException("connection timeout2");
                                throw argEx2;
                            }
                            else if (connectClient.Connected == true)
                            {
                                connectClient.NoDelay = true;
                                connectStream = connectClient.GetStream();
                                connectStream.ReadTimeout = GET_CHUNK_TIMEOUT;
                                connectStream.WriteTimeout = 2000;

                                getstream = true;
                                printOnDL_PULL("Pull_Stream_ID:" + pn.Id + "\n");

                            }

                        }
                        catch
                        {
                            printOnDL_PULL("Pull_Stream_Fail\n");

                            if (connectClient != null)
                                connectClient.Close();
                            if (connectStream != null)
                                connectStream.Close();

                            getstream = false;
                            getport = false;
                            Thread.Sleep(10);
                            continue;
                        }
                    }

                    while (true)
                    {
                        if (!((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                        {
                            printOnDL_PULL("Pull_close" + "\n");
                            getport = false;
                            getstream = false;

                            if (connectClient != null)
                                connectClient.Close();
                            if (connectStream != null)
                                connectStream.Close();
                            break;
                        }

                        try
                        {


                            if ((!missCLRWRevrse && missCLReadIndex < missCLWriteIndex) || (missCLRWRevrse && missCLReadIndex < cConfig.ChunkCapacity))
                            {

                                tempTargetSeq = missChunkList[missCLReadIndex];
                                int treeIndex = calcTreeIndex(tempTargetSeq);
                                if (tempTargetSeq != 0)
                                {
                                    int chunkList_indexs = calcPlaybackIndex(tempTargetSeq);
                                    //if (chunkList[chunkList_indexs].seq == tempTargetSeq || tempTargetSeq < playingSeq)
                                    //{

                                    //    //sendMessage = StrToByteArray("wait");
                                    //    byte[] sendMessage = BitConverter.GetBytes(-1);
                                    //    connectStream.Write(sendMessage, 0, sendMessage.Length);
                                    //    connectStream.Flush();
                                    //    byte[] receiveMessage = new byte[4];
                                    //    connectStream.Read(receiveMessage, 0, receiveMessage.Length);
                                    //    //string recStr = ByteArrayToString(receiveMessage);
                                    //    int result2 = BitConverter.ToInt16(receiveMessage, 0);

                                    //    printOnDL_PULL("T[" + treeIndex + "] Seq:" + tempTargetSeq + " No pull\n");
                                    //    //printOnDL_PULL("Seq:" + tempTargetSeq + " Playing:" + playingSeq + "\n"); 
                                    //}
                                    //else
                                    //{
                                  
                                        //which chunk seq. 
                                        //byte[] message = BitConverter.GetBytes(tempTargetSeq);
                                        byte[] sendMessage = BitConverter.GetBytes(tempTargetSeq);
                                        connectStream.Write(sendMessage, 0, sendMessage.Length);
                                        connectStream.Flush();

                                        printOnDL_PULL("T[" + treeIndex + "] Seq:" + tempTargetSeq + " pulling\n");

                                        dataRead = 0;
                                        do
                                        {
                                            dataRead += connectStream.Read(responseMessage, dataRead, cConfig.ChunkSize - dataRead);
                                        } while (dataRead < cConfig.ChunkSize);
                                        connectStream.Flush();

                                        streamingChunk = (Chunk)ch.byteToChunk(bf, responseMessage);

                                        if (streamingChunk != null && streamingChunk.seq != 0)  // find chunk success
                                        {
                                            int chunkList_index = calcPlaybackIndex(streamingChunk.seq);

                                            if (streamingChunk.seq > playingSeq)
                                            {
                                                if (streamingChunk.seq != chunkList[chunkList_index].seq)
                                                {
                                                    chunkList[chunkList_index] = streamingChunk;

                                                    if (chunkList_index > chunkList_wIndex)
                                                        chunkList_wIndex = chunkList_index;

                                                    totalRecover += 1;

                                                    printOnDL_PULL("T[" + treeIndex + "]Seq:" + chunkList[chunkList_index].seq + " RvrPlayBuf\n");
                                                }
                                                else
                                                    printOnDL_PULL("T[" + treeIndex + "]Seq:" + chunkList[chunkList_index].seq + " In list\n");
                                            }
                                            else
                                                printOnDL_PULL("T[" + treeIndex + "] Seq:" + streamingChunk.seq + " RvrPlayBufLate\n");

                                            if (uploading)
                                            {
                                                int tIndex = calcTreeIndex(streamingChunk.seq);
                                                upPorth.setChunkList2(streamingChunk, tIndex);

                                                if (upPorth.getReadingSeq(tIndex) < streamingChunk.seq)
                                                    printOnDL_PULL("Seq:" + chunkList[chunkList_index].seq + " RvrUpBuf\n");
                                                else
                                                    printOnDL_PULL("Seq:" + tempTargetSeq + " RvrUpBufLate\n");

                                                //printOnDL_PULL("Seq:" + upPorth.getReadingSeq(tIndex) + ":" + streamingChunk.seq + "\n");
                                            }

                                        }
                                        else
                                        {
                                            if (streamingChunk == null)
                                                printOnDL_PULL("Seq:chunkNull\n");
                                            else
                                                printOnDL_PULL("T[" + treeIndex + "] Seq:" + tempTargetSeq.ToString() + " NotFind\n");

                                            pull_fail += 1;
                                            if (pull_fail == PULL_FAIL_NUM)
                                            {
                                                getstream = false;
                                                getport = false;
                                                pull_fail = 0;
                                                if (connectClient != null)
                                                    connectClient.Close();
                                                if (connectStream != null)
                                                    connectStream.Close();
                                                break;
                                            }
                                        }


                                   // }

                                } //end if(targetSeq!=0)

                                if (missCLReadIndex == cConfig.ChunkCapacity - 1)
                                {
                                    missCLReadIndex = 0;
                                    missCLRWRevrse = false;
                                }
                                else
                                    missCLReadIndex += 1;

                            } //end if (miss chunk list hv missing packet)
                            else
                            {
                                byte[] sendMessage2 = BitConverter.GetBytes(-1);
                                connectStream.Write(sendMessage2, 0, sendMessage2.Length);
                                connectStream.Flush();
                                byte[] receiveMessage2 = new byte[4];
                                connectStream.Read(receiveMessage2, 0, receiveMessage2.Length);
                                int result = BitConverter.ToInt16(receiveMessage2, 0);
                                //string recStr = ByteArrayToString(receiveMessage);
                                //if(result==-1)
                                //    printOnDL_PULL("Pull_wait_send/rece\n");
                            }

                        }
                        catch (Exception ex)
                        {
                            if (((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                                printOnDL_PULL("Pull_Error:" + "\n");
                            else
                                printOnDL_PULL("Pull_close*" + "\n");

                            getport = false;
                            getstream = false;

                            if (connectClient != null)
                                connectClient.Close();
                            if (connectStream != null)
                                connectStream.Close();
                            break;
                        }

                        Thread.Sleep(10);
                    } //end inner while

                } // end if pull button click

                Thread.Sleep(10);
            } //end while

        }*/

    }// end class     
}// end namespace
