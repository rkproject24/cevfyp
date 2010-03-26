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
        static int SEND_PORT_TIMEOUT = 5000;
        static int GET_CHUNK_TIMEOUT = 2000;
        static int PULL_FAIL_NUM = 5;

        public int treeNO = 0;
        int chunkList_wIndex = 0;  //write index
        int chunkList_rIndex = 0;  //read index
        int tempSeq = 0, playingSeq = 0;
        int tempResult;
        int virtualServerPort = 0;   //virtual server broadcast port number
        int totalMissPlayChunk = 0, totalPlayChunk = 0;
        int totalRecover = 0;

        int[] ReconMiss;
        int[] PushMiss;
        int[] beforeRconPlaySeq;
        int[] afterRconPlaySeq;
        int[] WaitMiss;

        bool serverConnect = false;
        bool vlcConnect = false;
        bool checkClose = false;
        bool idChange = false;


        string trackerIp = null;
        public VlcHandler vlc;
        ChunkHandler ch;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        List<Chunk> chunkList;
        List<TcpClient> ClientC;
        List<TcpClient> ClientD;
        List<List<Chunk>> treeChunkList;
        List<int> missingChunk;

        int[] treeCLWriteIndex;
        int[] treeCLReadIndex;
        int[] treeCLCurrentSeq;
        int[] treeCLRealInSeq;
        int[] treeCLLastReadSeq;
        int[] treeReconnectState;

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

        TcpListener server;
        NetworkStream localvlcstream = null;
        private ClientForm mainFm;
        public delegate void UpdateTextCallback(string message);
        public delegate void UpdateGraphCallback(plotgraph graphdata);
        private PeerHandler peerh;
        ClientConfig cConfig = new ClientConfig();
        StatisticHandler statHandler;
        public string[] waitMsg;

        public int[] downloadSpeed;
        bool[] readWriteReverse;
        bool[] fastReconnectReq;
        bool[] fr_asking;

        public ClientHandler(ClientForm mainFm)
        {
            this.mainFm = mainFm;
            vlc = new VlcHandler();
            ch = new ChunkHandler();
            statHandler = new StatisticHandler();

            cConfig.load("C:\\ClientConfig");

            chunkList = new List<Chunk>(cConfig.ChunkCapacity);

            //mainFm.tbhostIP.Text = TcpApps.LocalIPAddress();
            //mainFm.tbServerIp.Text = cConfig.Trackerip;

            ((LoggerFrm)mainFm.downloadFrm).tbIP.Text = cConfig.Trackerip;
            ((LoggerFrm)mainFm.uploadFrm).tbIP.Text = TcpApps.LocalIPAddress();

            string[] xmlList = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");
            foreach (string xmlfile in xmlList)
            {
                File.Delete(xmlfile);
            }

        }


        public void initialize()
        {
            downloadSpeed = new int[treeNO];

            treeChunkList = new List<List<Chunk>>(treeNO);
            treeCLWriteIndex = new int[treeNO];
            treeCLReadIndex = new int[treeNO];
            treeCLCurrentSeq = new int[treeNO];
            treeCLRealInSeq = new int[treeNO];
            treeCLLastReadSeq = new int[treeNO];
            treeReconnectState = new int[treeNO];
            readWriteReverse = new bool[treeNO];
            fastReconnectReq = new bool[treeNO];
            fr_asking = new bool[treeNO];
            waitMsg = new string[treeNO];

            PushMiss = new int[treeNO];
            ReconMiss = new int[treeNO];
            afterRconPlaySeq = new int[treeNO];
            beforeRconPlaySeq = new int[treeNO];
            WaitMiss = new int[treeNO];

            graphThreads = new List<Thread>(treeNO);
            receiveChunkThread = new List<Thread>(treeNO);
            receiveControlThread = new List<Thread>(treeNO);
            ClientC = new List<TcpClient>(treeNO);
            ClientD = new List<TcpClient>(treeNO);
            missingChunk = new List<int>();


            //missCLWriteIndex = new int[treeNO];
            //missCLReadIndex = new int[treeNO];
            missChunkList = new List<int>(cConfig.ChunkCapacity);

            for (int i = 0; i < cConfig.ChunkCapacity; i++)
            {
                Chunk ck = new Chunk();
                chunkList.Add(ck);

                missChunkList.Add(0);

            }

            for (int i = 0; i < treeNO; i++)
            {
                ClientC.Add(null);
                ClientD.Add(null);

                readWriteReverse[i] = false;
            }
        }

        public string getSelfID(int tree_index)
        {
            return peerh.Selfid[tree_index];
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

        public string establishConnect(string tackerIp)
        {
            //mainFm.tbWriteStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox1), new object[] { "" });

            string response = "";

            //connect tracker
            response = connectToTracker(tackerIp);

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

                    //check = connectToSources(i, false);
                    //if (!check)
                    //{
                    //    response = "One of Data Ports cannot join!!!!";

                    //    //unreg. already join tree
                    //    for (int k = 0; k <= i; k++)
                    //    {
                    //        if (ClientC[k] != null)
                    //            sendExit(k);
                    //    }

                    //    break;
                    //}

                    //check[i] = false;
                    conSrcThread[i] = false;
                    connectSrcThread[i] = new Thread(delegate() { conSrcThread[i] = connectToSources(i, false, true); });
                    connectSrcThread[i].IsBackground = true;
                    connectSrcThread[i].Name = " connectSrcThread_" + i;
                    connectSrcThread[i].Start();
                    Thread.Sleep(30);
                }

                for (int i = 0; i < treeNO; i++)
                {
                    while (true)
                    {
                        //if (!connectSrcThread[i].IsAlive)
                        //    break;
                        if (!connectSrcThread[i].IsAlive)
                        {
                            if (conSrcThread[i] == true)
                                break;
                            else
                            {
                                for (int j = 0; j < i; j++)
                                {
                                    sendExit(j);
                                }
                                return "connectToSource fail!";
                            }
                        }
                        Thread.Sleep(10);
                    }
                }

                //if (check)
                //{
                virtualServerPort = TcpApps.RanPort(cConfig.VlcPortBase, cConfig.VlcPortup);//peerh.Cport11 + cConfig.VlcPortBase;
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

        private string connectToTracker(string serverIp)
        {
            this.trackerIp = serverIp;

            peerh = new PeerHandler(serverIp, mainFm);
            treeNO = peerh.findTracker();

            if (treeNO != -1)
            {
                initialize();
                return "OK";
            }
            else
            {
                return "Tracker " + serverIp + " Unreachable!";
            }

        }


        private bool connectToSources(int tree_index, bool reconnectUse, bool downPeerList)
        {
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
            //if (!checkClose)
            //    fastReconnectReq[tree_index] = true;

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
                    if (!peerh.downloadPeerlist(tree_index, true))
                    {
                        //  ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "reconnect downloadPeerlist fail T" + tree_index + "\n" });
                        Thread.Sleep(20);
                        continue;
                    }
                    //else
                    //    ConnectToSrcCount = 0;
                }

                // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Peerlist T" + tree_index + " OK\n" });
                if (!connectToSources(tree_index, true, false))
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
                //fastReconnectReq[tree_index] = false;
            }

        }


        public void startThread()
        {
            startUpload();
            tempSeq = 0;

            requireChunkThread = new Thread(new ThreadStart(requireChunk));
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

            updateChunkListThread = new Thread(new ThreadStart(updateTreeChunkList));
            updateChunkListThread.IsBackground = true;
            updateChunkListThread.Name = "update_ChunkList";
            updateChunkListThread.Start();

            broadcastVlcStreamingThread = new Thread(new ThreadStart(broadcastVlcStreaming));
            broadcastVlcStreamingThread.IsBackground = true;
            broadcastVlcStreamingThread.Name = "broadcast_VlcStreaming";
            broadcastVlcStreamingThread.Start();

            updateFmIDThread = new Thread(new ThreadStart(updateFmID));
            updateFmIDThread.IsBackground = true;
            updateFmIDThread.Name = "update_FMID";
            updateFmIDThread.Start();



            // vlc.play(mainFm.panel2, virtualServerPort);
            vlc.play(((PlaybackFrm)mainFm.playFrm).playPanel, virtualServerPort);
        }



        public void startUpload()
        {
            if (!uploading)
            {
                //((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Start: \n" });
                //CLIENTIP = ((LoggerFrm)mainFm.uploadFrm).tbIP.Text.ToString(); //mainFm.tbhostIP.Text.ToString();

                //start uploading thread
                this.max_peer = cConfig.MaxPeer;
                upPorth = new UploadPortHandler(cConfig, CLIENTIP, mainFm, treeNO, this);
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
            while (true)
            {
                if (idChange)
                {
                    string idStr = "";
                    //mainFm.Text = "Client:";
                    mainFm.BeginInvoke(new UpdateTextCallback(mainFm.UpdateLabel5), new object[] { "" });
                    //((ControlFrm)mainFm.controlFrm).lbId.BeginInvoke(new UpdateTextCallback(mainFm.UpdateLabel5), new object[] { "" });
                    for (int i = 0; i < peerh.Selfid.Length; i++)
                    {
                        idStr += peerh.Selfid[i] + ",";
                    }
                    //((ControlFrm)mainFm.controlFrm).lbId.BeginInvoke(new UpdateTextCallback(mainFm.UpdateLabel5), new object[] { idStr });
                    mainFm.BeginInvoke(new UpdateTextCallback(mainFm.UpdateLabel5), new object[] { "Client:" + idStr });

                    // mainFm.Text += peerh.Selfid[0] + ",";

                    idChange = false;
                }

                Thread.Sleep(50);

            }


        }




        private void receiveTreeChunk(int tree_index)
        {
            TcpClient clientD = null;
            NetworkStream streams = null;
            BinaryFormatter bf = new BinaryFormatter();

            byte[] responseMessage = new byte[cConfig.ChunkSize];
            Chunk streamingChunk;

            int lastMissSeq = 0, missSeq = 0;
            bool firstConnectRound;
            int checkReconMissCount = 0, checkWaitMissCount = 0;

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
                        checkReconMissCount = 0;
                        afterRconPlaySeq[tree_index] = playingSeq;

                    }
                    else
                    {
                        //***********Reconnection miss handle************
                        //if (((ControlFrm)mainFm.controlFrm).PullChb.Checked && treeCLCurrentSeq[tree_index] < playingSeq + 10)
                        //{
                        //    missSeq = treeCLCurrentSeq[tree_index] + treeNO;
                        //    if (lastMissSeq != (missSeq))
                        //    {
                        //       // printOnDL_PULL("T[" + tree_index + "]:" + missSeq + " ReconMiss\n");
                        //       // printOnDL_PULL("CRM:" + checkReconMissCount + "\n");
                        //        if (checkReconMissCount < 50)// || (checkReconMissCount % 30 == 0))
                        //        {
                        //           // missingChunk.Add(missSeq);
                        //            addMissChunkList(missSeq);
                        //            lastMissSeq = missSeq;
                        //            printOnDL_PULL("T[" + tree_index + "]:" + missSeq + " ReconMissAdd\n");

                        //        }
                        //    }
                        //    treeCLCurrentSeq[tree_index] += treeNO;
                        //    ReconMiss[tree_index] += 1;
                        //    checkReconMissCount += 1;


                        //}

                        upPorth.setTreeCLState(tree_index, 0);

                        if (tree_index == 0)
                            ((ControlFrm)mainFm.controlFrm).lbTree0.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { "Recon.." });
                        if (tree_index == 1)
                            ((ControlFrm)mainFm.controlFrm).lbTree1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { "Recon.." });
                        if (tree_index == 2)
                            ((ControlFrm)mainFm.controlFrm).lbTree2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { "Recon.." });


                        Thread.Sleep(10);
                        continue;
                    }

                    int receiveCount = 1;
                    DateTime start, end;
                    start = DateTime.Now;
                    firstConnectRound = true;
                    int dataRead;
                    string responseString;
                    //string[] types;
                    string[] type;
                    int missBase = 0;

                    while (true)
                    {

                        if (ClientD[tree_index] == null)
                        {
                            if (streams != null)
                                streams.Close();
                            if (clientD != null)
                                clientD.Close();

                            upPorth.setTreeCLState(tree_index, 0);

                            if (tree_index == 0)
                                ((ControlFrm)mainFm.controlFrm).lbTree0.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { "Recon~" });
                            if (tree_index == 1)
                                ((ControlFrm)mainFm.controlFrm).lbTree1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { "Recon~" });
                            if (tree_index == 2)
                                ((ControlFrm)mainFm.controlFrm).lbTree2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { "Recon~" });


                            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] D exit~\n" });

                            break;
                        }

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

                        //type = responseString.Split('@');

                        if (type[0].Equals("Wait"))
                        {
                            //***********Wait miss handle************
                            //if (((ControlFrm)mainFm.controlFrm).PullChb.Checked && treeCLCurrentSeq[tree_index] < playingSeq + 10 && treeCLCurrentSeq[tree_index] != 0)
                            //{
                            //    missSeq = treeCLCurrentSeq[tree_index] + treeNO;
                            //    if (lastMissSeq != (missSeq))
                            //    {
                            //       // printOnDL_PULL("T[" + tree_index + "]:" + missSeq + " WaitMiss\n");
                            //        if (checkWaitMissCount < 6 || (checkWaitMissCount % 30 == 0))
                            //        {
                            //            missingChunk.Add(missSeq);
                            //            lastMissSeq = missSeq;
                            //            printOnDL_PULL("T[" + tree_index + "]:" + missSeq + " WaitMissAdd\n");
                            //        }


                            //    }

                            //    treeCLCurrentSeq[tree_index] += treeNO;
                            //    WaitMiss[tree_index] += 1;
                            //    checkWaitMissCount += 1;

                            //}

                            waitMsg[tree_index] = responseString;
                            if (type[1] == peerh.Selfid[tree_index])
                            {
                                ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "loopErr Wait@" + type[1] + "\n" });
                                PeerNode tempPn = peerh.JoinPeer[tree_index];
                                peerh.removePeer(tempPn, tree_index);
                                ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "loopErr Remove ID:" + tempPn.Id + "\n" });

                                ArgumentException argEx3 = new System.ArgumentException("loopErr");
                                throw argEx3;
                            }

                            upPorth.setTreeCLState(tree_index, 2);
                            downloadSpeed[tree_index] = 0;
                            if (tree_index == 0)
                                ((ControlFrm)mainFm.controlFrm).lbTree0.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { "Wait~" + type[1].ToString() });

                            if (tree_index == 1)
                                ((ControlFrm)mainFm.controlFrm).lbTree1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { "Wait~" + type[1].ToString() });

                            if (tree_index == 2)
                                ((ControlFrm)mainFm.controlFrm).lbTree2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { "Wait~" + type[1].ToString() });


                            Thread.Sleep(10);
                            continue;
                        }

                        streamingChunk = (Chunk)ch.byteToChunk(bf, responseMessage);

                        // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "]:" + streamingChunk.seq + "\n" });

                        if (streamingChunk == null)
                        {

                            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] chunk null\n" });

                            //Add the missing chunk to missingChunk list
                            if (((ControlFrm)mainFm.controlFrm).PullChb.Checked && firstConnectRound != true)
                            {
                                missSeq = treeCLCurrentSeq[tree_index] + treeNO;
                                if (lastMissSeq != (missSeq))
                                {
                                    printOnDL_PULL("T[" + tree_index + "]:" + missSeq + " Null miss\n");
                                    // missingChunk.Add(missSeq);
                                    addMissChunkList(missSeq);
                                    lastMissSeq = missSeq;
                                }
                                treeCLCurrentSeq[tree_index] = missSeq;
                                PushMiss[tree_index] += 1;
                            }

                            if (checkNullCount >= cConfig.MaxNullChunk)
                            {
                                ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "chunk null Expcetion\n" });
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
                        else
                        {
                            checkNullCount = 0;

                            //***********Push miss handle************
                            if (((ControlFrm)mainFm.controlFrm).PullChb.Checked && streamingChunk.seq != treeCLCurrentSeq[tree_index] + treeNO && firstConnectRound == false)
                            {

                                //missSeq = treeCLCurrentSeq[tree_index] + treeNO;
                                //if (lastMissSeq != (missSeq))
                                //{
                                //   printOnDL_PULL("T[" + tree_index + "]:" + missSeq + " Push miss\n");
                                //    missingChunk.Add(missSeq);
                                //    lastMissSeq = missSeq;
                                //}
                                //PushMiss[tree_index] += 1;

                                missBase = treeCLCurrentSeq[tree_index] + treeNO;
                                //printOnDL_PULL("T[" + tree_index + "]:Push miss Start No:" + missBase + "\n");
                                while (missBase < streamingChunk.seq)
                                {
                                    missSeq = missBase;
                                    if (lastMissSeq != (missSeq))
                                    {
                                        printOnDL_PULL("T[" + tree_index + "]:" + missSeq + " Push miss add\n");
                                        //missingChunk.Add(missSeq);
                                        addMissChunkList(missSeq);
                                        lastMissSeq = missSeq;
                                    }
                                    PushMiss[tree_index] += 1;
                                    missBase += treeNO;
                                    // printOnDL_PULL("T[" + tree_index + "]:" + missSeq + " Push miss_test\n");
                                    Thread.Sleep(10);
                                }
                                // printOnDL_PULL("T[" + tree_index + "]Push miss End No:" + missBase + "\n");
                            }

                            //prevent calculate miss seq. in first receive round or firt chunk is null
                            if (firstConnectRound)
                            {
                                if (streamingChunk.seq > treeCLCurrentSeq[tree_index])
                                    treeCLCurrentSeq[tree_index] = streamingChunk.seq;
                                firstConnectRound = false;
                            }

                        }

                        upPorth.setTreeCLState(tree_index, 1);

                        //by vinci: send responseBytes directly to peer
                        //this.uploadToPeer(responseMessageBytes);
                        if (uploading)
                        {
                            upPorth.setChunkList(streamingChunk, tree_index);
                        }


                        if (streamingChunk.seq > playingSeq - RECEIVE_RANGE)
                        {
                            int write_index = treeCLWriteIndex[tree_index];

                            treeChunkList[tree_index][write_index] = streamingChunk;

                            if (write_index == cConfig.ChunkCapacity - 1)
                            {
                                treeCLWriteIndex[tree_index] = 0;
                                readWriteReverse[tree_index] = true;
                            }
                            else
                                treeCLWriteIndex[tree_index] += 1;

                            if (streamingChunk.seq > treeCLCurrentSeq[tree_index])
                                treeCLCurrentSeq[tree_index] = streamingChunk.seq;

                            if (tree_index == 0)
                                ((ControlFrm)mainFm.controlFrm).lbTree0.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { treeCLCurrentSeq[tree_index].ToString() });

                            if (tree_index == 1)
                                ((ControlFrm)mainFm.controlFrm).lbTree1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { treeCLCurrentSeq[tree_index].ToString() });

                            if (tree_index == 2)
                                ((ControlFrm)mainFm.controlFrm).lbTree2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { treeCLCurrentSeq[tree_index].ToString() });

                            //speed test
                            if (receiveCount == 30)
                            {
                                end = DateTime.Now;
                                plotgraph p = new plotgraph("tree" + tree_index);
                                //double speed =p.speedCalculate(start, end, cConfig.ChunkSize * 20 * 8)/1000;
                                downloadSpeed[tree_index] = Convert.ToInt32(p.speedCalculate(start, end, cConfig.ChunkSize * 30 * 8) / 1000);
                                //((LoggerFrm)mainFm.downloadFrm).lbSpeed.BeginInvoke(new UpdateTextCallback(mainFm.UpdateDownloadSpeed), new object[] { Math.Round(speed).ToString() });
                                statHandler.updateCurve(start, end, cConfig.ChunkSize * 30 * 8, tree_index, treeNO);
                                receiveCount = 1;
                                start = DateTime.Now;
                            }
                            receiveCount++;
                        }
                        else
                        {
                            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "]:" + streamingChunk.seq + "<" + playingSeq + "\n" });//treeCLRealInSeq[tree_index] + "\n" });
                            //treeCLCurrentSeq[tree_index] == streamingChunk.seq;

                            ArgumentException argEx2 = new System.ArgumentException("chunkNull");
                            throw argEx2;

                        }

                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    // if(!checkClose)
                    // MessageBox.Show(ex.ToString());

                    if (streams != null)
                        streams.Close();
                    if (clientD != null)
                        clientD.Close();

                    upPorth.setTreeCLState(tree_index, 0);
                    downloadSpeed[tree_index] = 0;
                    if (!checkClose)
                    {
                        beforeRconPlaySeq[tree_index] = treeCLCurrentSeq[tree_index];
                        //peerh.updateTotalChunkNull(tree_index, peerh.JoinPeer[tree_index]);//update the totalChunkNull to xml

                        if (ex.ToString().Contains("disposed object"))
                        {
                            //treeReconnectState[tree_index] = 2;
                            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] D other exception~ \n" });
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
                    {
                        if (tree_index == 0)
                            ((ControlFrm)mainFm.controlFrm).lbTree0.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { "close ok" });
                        //mainFm.textBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { "close ok" });
                        if (tree_index == 1)
                            ((ControlFrm)mainFm.controlFrm).lbTree1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { "close ok" });
                        //mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { "close ok" });
                        if (tree_index == 2)
                            ((ControlFrm)mainFm.controlFrm).lbTree2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { "close ok" });
                        //mainFm.textBox3.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { "close ok" });
                    }

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

            while (true)
            {
                try
                {
                    if (ClientC[tree_index] != null)
                    {
                        clientC = ClientC[tree_index];
                        stream = clientC.GetStream();
                        stream.ReadTimeout = 3000;
                    }

                    while (true)
                    {
                        if (treeReconnectState[tree_index] != 0 && !checkClose)
                        {
                            sMessage = System.Text.Encoding.ASCII.GetBytes("Exit");
                            stream.Write(sMessage, 0, sMessage.Length);

                            stream.Close();
                            clientC.Close();

                            ClientD[tree_index].Close();
                            ClientD[tree_index] = null;
                            ClientC[tree_index].Close();
                            ClientC[tree_index] = null;

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
                            //for (int i = 0; i < treeNO; i++)
                            //{
                            //    if (fastReconnectReq[i] && !fr_asking[i])
                            //    {
                            //        fr_asking[i] = true;

                            //        sMessage = System.Text.Encoding.ASCII.GetBytes("Conn");
                            //        stream.Write(sMessage, 0, sMessage.Length);
                            //        ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] FR Req T:" + i + "\n" });

                            //        int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
                            //        string responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);

                            //        if (responseString.Equals("Have"))
                            //        {
                            //            peerh.FastConnect[tree_index] = true;
                            //            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] FR OK T:" + i + "\n" });
                            //        }
                            //        else
                            //            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] FR Fail T:" + i + "\n" });

                            //        fastReconnectReq[i] = false;
                            //        fr_asking[i] = false;
                            //    }

                            //}

                            //stream = ClientC[tree_index].GetStream();
                            sMessage = System.Text.Encoding.ASCII.GetBytes("Wait");
                            stream.Write(sMessage, 0, sMessage.Length);

                            Thread.Sleep(20);
                            continue;
                        }

                        Thread.Sleep(20);
                    }
                }
                catch (Exception ex)
                {
                    if (stream != null)
                        stream.Close();
                    if (clientC != null)
                        clientC.Close();

                    if (ClientD[tree_index] != null && ClientC[tree_index] != null)
                    {
                        ClientD[tree_index].Close();
                        ClientD[tree_index] = null;
                        ClientC[tree_index].Close();
                        ClientC[tree_index] = null;
                    }

                    if (!checkClose)
                    {
                        reconnection(tree_index, "other");
                        treeReconnectState[tree_index] = 0;
                    }

                    upPorth.setTreeCLState(tree_index, 0);
                    if (!checkClose)
                        ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] C exit\n" });
                    else
                        ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "]  close\n" });

                }

                Thread.Sleep(10);
            }
        }

        private void updateTreeChunkList()
        {
            while (true)
            {
                if (treeChunkList[0][0].seq != 0)
                {
                    tempSeq = treeChunkList[0][0].seq;
                    chunkList_rIndex = calcPlaybackIndex(tempSeq); //set the start index of first chunk in chunk list

                    break;
                }
                Thread.Sleep(10);
            }

            Chunk ck = new Chunk();
            ck.seq = 0;
            int readIndex = 0, readingSeq = 0;

            while (true)
            {
                for (int i = 0; i < treeNO; i++)
                {

                    //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + i + "]:" + treeCLLastReadSeq[i] + "::" + treeCLRealInSeq[i] + "\n" });

                    if ((!readWriteReverse[i] && treeCLReadIndex[i] < treeCLWriteIndex[i]) || (readWriteReverse[i] && treeCLReadIndex[i] < cConfig.ChunkCapacity))
                    {

                        readIndex = treeCLReadIndex[i];
                        readingSeq = treeChunkList[i][readIndex].seq;

                        chunkList_wIndex = calcPlaybackIndex(readingSeq);

                        if (treeChunkList[i][readIndex].seq != chunkList[chunkList_wIndex].seq && treeChunkList[i][readIndex].seq > playingSeq)
                        {
                            chunkList[chunkList_wIndex] = treeChunkList[i][readIndex];



                            // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "S" + i + "W:" + chunkList_wIndex + " R:" + chunkList_rIndex + " \n" });
                        }

                        if (treeCLReadIndex[i] == cConfig.ChunkCapacity - 1)
                        {
                            treeCLReadIndex[i] = 0;
                            readWriteReverse[i] = false;
                        }
                        else
                            treeCLReadIndex[i] += 1;

                    }
                }

                Thread.Sleep(10);
            }
        }

        private void broadcastVlcStreaming()
        {
            TcpClient client = null;

            while (true)
            {
                try
                {

                    server = new TcpListener(localAddr, virtualServerPort);
                    ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "virtualServerPort:" + virtualServerPort + "\n" });
                    server.Start();

                    client = server.AcceptTcpClient();

                    localvlcstream = client.GetStream();

                    byte[] sendMessage = System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-type: application/octet-stream\r\nCache-Control: no-cache\r\n\r\n");
                    localvlcstream.Write(sendMessage, 0, sendMessage.Length);

                    if (client.Connected == true) vlcConnect = true;

                    byte[] sendClientChunk;

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
                            }
                            else
                            {
                                totalMissPlayChunk += 1;
                                //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "totalMissPlayChunk="+totalMissPlayChunk+"\n" });
                            }

                            totalPlayChunk += 1;

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
                    if (localvlcstream != null)
                        localvlcstream.Close();
                    if (client != null)
                        client.Close();

                    ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { ex.ToString() });
                }
                Thread.Sleep(10);
            }

        }


        private void requireChunk()
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
                //if (!((ControlFrm)mainFm.controlFrm).PullChb.Checked && missingChunk.Count > 0)
                //missingChunk.Clear();

                // if (missingChunk.Count > 0 && ((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                if (((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                {

                    //if (missingChunk.Count == 0 && getstream)
                    //getstream = false;

                    if (getport == false)
                    {
                        while (true)
                        {
                            if (!((ControlFrm)mainFm.controlFrm).PullChb.Checked)
                                break;

                            Random rm = new Random();
                            int temp_tree_index = rm.Next(0, treeNO);
                            printOnDL_PULL("Pull_select_pn T:" + temp_tree_index + "\n");


                            //if (!peerh.hasPeer(temp_tree_index))
                            //{
                            //    printOnDL_PULL("Pull_noPeerInList\n");
                            //    Thread.Sleep(10);
                            //    continue;
                            //    //if (!peerh.downloadPeerlist(temp_tree_index, true))
                            //    //{
                            //    //    printOnDL_PULL("Pull_dlPeerList_Fail\n");
                            //    //    Thread.Sleep(10);
                            //    //    continue;
                            //    //}
                            //}

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
                                connectClient = new TcpClient(pn.Ip, pn.ListenPort);
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
                            catch (Exception ex)
                            {
                                if (pn != null)
                                {
                                    pn.NullChunkTotal++;
                                    peerh.updateTotalChunkNull(check_tree_index, pn);//update the totalChunkNull to xml
                                }
                                // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Pull_Port_Fail\n" });
                                printOnDL_PULL("Pull_Port_Fail\n");
                                if (connectClient != null)
                                    connectClient.Close();
                                if (connectStream != null)
                                    connectStream.Close();

                                Thread.Sleep(10);
                                continue;
                            }

                            printOnDL_PULL("Pull_Port_OK,ID:" + pn.Id + "\n");
                            //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Pull_Port_OK,ID:" + pn.Id+ "\n" });

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

                    if (!firstMiss)
                    {
                        Thread.Sleep(10);
                        continue;

                    }

                    if ((!missCLRWRevrse && missCLReadIndex < missCLWriteIndex) || (missCLRWRevrse && missCLReadIndex < cConfig.ChunkCapacity))
                    {

                        tempTargetSeq = missChunkList[missCLReadIndex];
                        if (tempTargetSeq != 0)
                        {
                            int chunkList_indexs = calcPlaybackIndex(tempTargetSeq);
                            if (chunkList[chunkList_indexs].seq == tempTargetSeq || tempTargetSeq < playingSeq)
                            {
                                missingChunk.Remove(tempTargetSeq);
                                printOnDL_PULL("Seq:" + tempTargetSeq + " No pull\n");
                                //printOnDL_PULL("Seq:" + tempTargetSeq + " Playing:" + playingSeq + "\n"); 
                            }
                            else
                            {
                                printOnDL_PULL("Seq:" + tempTargetSeq + " strart_pull\n");

                                if (getstream == false)
                                {
                                    try
                                    {
                                        connectClient = new TcpClient(pn.Ip, pull_port);
                                        connectClient.NoDelay = true;
                                        connectClient.ReceiveBufferSize = cConfig.ChunkSize;
                                        connectStream = connectClient.GetStream();
                                        connectStream.ReadTimeout = GET_CHUNK_TIMEOUT;
                                        connectStream.WriteTimeout = 2000;

                                        getstream = true;
                                        printOnDL_PULL("Pull_Stream_OK\n");
                                    }
                                    catch
                                    {
                                        //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Pull_Stream_Fail\n" });
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

                                //connect to peer ask chunk
                                try
                                {

                                    //which chunk seq. 
                                    byte[] message = BitConverter.GetBytes(tempTargetSeq);
                                    connectStream.Write(message, 0, message.Length);
                                    connectStream.Flush();
                                    printOnDL_PULL("Seq:" + tempTargetSeq + " pulling\n");

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

                                                printOnDL_PULL("Seq:" + chunkList[chunkList_index].seq + " Recover\n");
                                            }
                                            else
                                                printOnDL_PULL("Seq:" + chunkList[chunkList_index].seq + " In list\n");
                                        }
                                        else
                                            printOnDL_PULL("Seq:" + streamingChunk.seq + " RecoverLate\n");

                                        if (uploading)
                                        {
                                            upPorth.setChunkList(streamingChunk, calcTreeIndex(streamingChunk.seq));
                                        }

                                    }
                                    else
                                    {
                                        if (streamingChunk == null)
                                            printOnDL_PULL("Seq:chunkNull\n");
                                        else
                                            printOnDL_PULL("Seq:" + tempTargetSeq.ToString() + " NotFind\n");

                                        pull_fail += 1;
                                        if (pull_fail == PULL_FAIL_NUM)
                                        {
                                            getstream = false;
                                            getport = false;
                                            pull_fail = 0;
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    printOnDL_PULL("Pull_Error:" + "\n");

                                    getport = false;
                                    getstream = false;

                                    if (connectClient != null)
                                        connectClient.Close();
                                    if (connectStream != null)
                                        connectStream.Close();
                                }
                            }

                        }

                        if (missCLReadIndex == cConfig.ChunkCapacity - 1)
                        {
                            missCLReadIndex = 0;
                            missCLRWRevrse = false;
                        }
                        else
                            missCLReadIndex += 1;

                    }

                    /*
                    if (missingChunk.Count > 0)
                    {
                        printOnDL_PULL("get targetSeq\n");

                        tempTargetSeq = missingChunk[0];

                        if (tempTargetSeq < 1)
                        {
                            //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:" + tempTargetSeq + "wrong\n" });
                            printOnDL_PULL("Seq:" + tempTargetSeq + "wrong\n");
                            missingChunk.Remove(tempTargetSeq);
                            Thread.Sleep(10);
                            continue;
                        }
                        check_tree_index = calcTreeIndex(tempTargetSeq);

                        //prevent ask chunk which already existed in playBack buffer or smaller than playingSeq
                        int chunkList_indexs = calcPlaybackIndex(tempTargetSeq);
                        if (chunkList[chunkList_indexs].seq == tempTargetSeq || tempTargetSeq < playingSeq)
                        {
                            missingChunk.Remove(tempTargetSeq);
                            printOnDL_PULL("Seq:" + tempTargetSeq + " No pull\n");
                            //printOnDL_PULL("Seq:" + tempTargetSeq + " Playing:" + playingSeq + "\n");
                            //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:" + tempTargetSeq + " No pull\n" });
                            //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:" + tempTargetSeq + " Playing:" + playingSeq + "\n" });

                            Thread.Sleep(10);
                            continue;
                        }

                        if (getstream == false)
                        {
                            try
                            {
                                connectClient = new TcpClient(pn.Ip, pull_port);
                                connectClient.NoDelay = true;
                                connectClient.ReceiveBufferSize = cConfig.ChunkSize;
                                connectStream = connectClient.GetStream();
                                connectStream.ReadTimeout = GET_CHUNK_TIMEOUT;
                                connectStream.WriteTimeout = 2000;

                                getstream = true;
                            }
                            catch
                            {
                                //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Pull_Stream_Fail\n" });
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


                        printOnDL_PULL("Seq:" + tempTargetSeq + " strart_pull\n");
                        //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:" + tempTargetSeq + " strart_pull\n" });

                        //connect to peer ask chunk
                        try
                        {
                            //which chunk seq. 
                            byte[] message = BitConverter.GetBytes(tempTargetSeq);
                            connectStream.Write(message, 0, message.Length);
                            connectStream.Flush();
                            printOnDL_PULL("Seq:" + tempTargetSeq + " pulling\n");
                            // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:" + tempTargetSeq + " pulling\n" });
                            //Thread.Sleep(10);

                            //connectStream.Read(responseMessage, 0, responseMessage.Length);
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

                                        //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:" + chunkList[chunkList_index].seq + " Recover\n" });
                                        printOnDL_PULL("Seq:" + chunkList[chunkList_index].seq + " Recover\n");
                                    }
                                    else
                                        printOnDL_PULL("Seq:" + chunkList[chunkList_index].seq + " In list\n");
                                    //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:" + chunkList[chunkList_index].seq + " In list\n" });
                                }
                                else
                                    printOnDL_PULL("Seq:" + streamingChunk.seq + " RecoverLate\n");
                                //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:" + streamingChunk.seq + " RecoverLate\n" });

                                if (uploading)
                                {
                                    upPorth.setChunkList(streamingChunk, calcTreeIndex(streamingChunk.seq));
                                }

                            }
                            else
                            {
                                if (streamingChunk == null)
                                    printOnDL_PULL("Seq:chunkNull\n");
                                // ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:chunkNull\n" });
                                else
                                    printOnDL_PULL("Seq:" + tempTargetSeq.ToString() + " NotFind\n");
                                //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Seq:" + tempTargetSeq.ToString() + " NotFind\n" });

                                pull_fail += 1;
                                if (pull_fail == PULL_FAIL_NUM)
                                {
                                    getstream = false;
                                    getport = false;
                                    pull_fail = 0;
                                    Thread.Sleep(10);
                                    continue;
                                }

                            }
                            missingChunk.Remove(tempTargetSeq);

                        }
                        catch (Exception ex)
                        {
                            //((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "Pull_Error:" + "\n" });
                            printOnDL_PULL("Pull_Error:" + "\n");

                            getport = false;
                            getstream = false;

                            if (connectClient != null)
                                connectClient.Close();
                            if (connectStream != null)
                                connectStream.Close();
                        }
                    } //end if(miss.count>0)
                    */

                } //end if (clicked)

                Thread.Sleep(10);
            } //end while

        }


        public void printOnDL(string message)
        {
            ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { message });
        }

        public void printOnDL_PULL(string message)
        {
            ((LoggerFrm)mainFm.downloadFrm).rtbpull.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDLPull), new object[] { message });
        }

        public void printOnUL_PULL(string message)
        {
            ((LoggerFrm)mainFm.uploadFrm).rtbpull.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbULPull), new object[] { message });
        }

        public void printOnUL(string message)
        {
            ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { message });
        }

        public void addMissChunkList(int missSeq)
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
                //MessageBox.Show(ex.ToString());
                ((LoggerFrm)mainFm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] Send exit err\n" });
            }

            if (stream != null)
                stream.Close();
            if (clientC != null)
                clientC.Close();

            ClientD[tree_index] = null;
            ClientC[tree_index] = null;

        }

        //calculate tree_index of chunk
        public int calcTreeIndex(int seqNum)
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

        //basic search method
        private int searchChunk(int list_index, int rIndex, int wIndex, int target)
        {
            if (wIndex < rIndex)
            {
                tempResult = search(list_index, rIndex, cConfig.ChunkCapacity - 1, target);
                if (tempResult != -1)
                    return tempResult;
                else
                {
                    tempResult = search(list_index, 0, wIndex, target);
                    return tempResult;
                }
            }
            else
            {
                tempResult = search(list_index, rIndex, wIndex - 1, target);
                return tempResult;
            }
        }

        private int search(int list_index, int r_index, int w_index, int target)
        {
            for (; r_index <= w_index; )
            {
                if (treeChunkList[list_index][r_index].seq == target)
                    return r_index;

                r_index += 1;
            }
            return -1;
        }

        private bool checkbuff() // by vinci:TO keep buffer for chunklist
        {
            //by vinci
            //int indexdiff = chunkList_wIndex - chunkList_rIndex;
            int indexdiff;
            if (chunkList_rIndex <= chunkList_wIndex)
            {
                indexdiff = chunkList_wIndex - chunkList_rIndex;
            }
            else
            {
                indexdiff = chunkList_wIndex + cConfig.ChunkCapacity - chunkList_rIndex;
            }

            //   ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "checkBuff:"+indexdiff.ToString()+"\n" });

            if (indexdiff <= cConfig.ChunkBuf)
            {
                //if (indexdiff <= 0)
                //{
                return false;
                //if (mainFm.tbStatus.Text != "Buffering...")
                //  mainFm.tbStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox3), new object[] { "Buffering..." });
                //}
            }
            else if (indexdiff > cConfig.StartBuf)
            {
                //if (mainFm.tbStatus.Text != "Playing")
                //  mainFm.tbStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox3), new object[] { "Playing" });
                return true;
            }
            return false;

        }

        public void closeAllThread()
        {

            if (serverConnect == true)
            {
                printOnDL("\ntotalRcover=" + totalRecover + "\n");
                printOnDL("totalMissPlayChunk=" + totalMissPlayChunk + "\n");
                printOnDL("totalPlayChunk=" + totalPlayChunk + "\n\n");
                for (int i = 0; i < treeNO; i++)
                {

                    printOnDL("T[" + i + "]beforeRecon=" + beforeRconPlaySeq[i] + "\n");
                    printOnDL("afterRecon=" + afterRconPlaySeq[i] + "\n");
                    printOnDL("RconMiss~=" + ReconMiss[i] + "\n");
                    printOnDL("PushMiss=" + PushMiss[i] + "\n");
                    printOnDL("WaitMiss=" + WaitMiss[i] + "\n\n");

                }

                checkClose = true;
                //mainFm.tbWriteStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox1), new object[] { "Closing Thread.." });

                //Abort pull chunk thread.
                requireChunkThread.Abort();

                //stop upload
                listenPeer.Stop();
                listenerThread.Abort();
                upPorth.closeCDPortThread();

                vlcConnect = false;
                uploading = false;

                server.Stop();
                vlc.stop();
                broadcastVlcStreamingThread.Abort();
                vlcConnect = false;

                updateChunkListThread.Abort();
                for (int i = 0; i < treeNO; i++)
                {
                    receiveChunkThread[i].Abort();
                    sendExit(i);
                    receiveControlThread[i].Abort();
                }
                serverConnect = false;

                receiveChunkThread.Clear();
                receiveControlThread.Clear();

                chunkList_wIndex = 0;
                chunkList_rIndex = 0;
                virtualServerPort = 0;
                //firstChunkIndex = -1;
                playingSeq = 0;
                //firstChunkRecv = false;
                totalMissPlayChunk = 0;
                totalPlayChunk = 0;
                totalRecover = 0;


                chunkList.Clear();
                for (int i = 0; i < treeNO; i++)
                {
                    treeChunkList[i].Clear();
                    treeCLWriteIndex[i] = 0;
                    treeCLReadIndex[i] = 0;
                    treeCLCurrentSeq[i] = 0;
                    treeReconnectState[i] = 0;

                    readWriteReverse[i] = false;
                    afterRconPlaySeq[i] = 0;
                    beforeRconPlaySeq[i] = 0;
                    ReconMiss[i] = 0;
                    PushMiss[i] = 0;
                    WaitMiss[i] = 0;
                    waitMsg[i] = "";
                }

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

            //===================moved to establish connection=======================================//
            //IPAddress uploadipAddr = IPAddress.Parse(CLIENTIP);

            //while(true)
            //{

            //    try
            //    {
            //       // mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { PeerListenPort.ToString() +"\n"});
            //        listenPeer = new TcpListener(uploadipAddr, PeerListenPort);
            //        listenPeer.Start();
            //        break;
            //    }
            //    catch (Exception ex)
            //    {
            //        //((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Listen fail:" + PeerListenPort+ "\n"+ex  });
            //        Thread.Sleep(20);
            //        PeerListenPort = TcpApps.RanPort(cConfig.LisPort, cConfig.LisPortup);
            //    }
            //}

            ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "IP:" + listenPeer.LocalEndpoint.ToString() + "\nPort[" + PeerListenPort.ToString() + "]:Listening...\n" });

            while (true)
            {
                try
                {
                    client = listenPeer.AcceptTcpClient();
                    stream = client.GetStream();
                    stream.ReadTimeout = SEND_PORT_TIMEOUT;

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


                            for (int j = 0; j < max_peer; j++)
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
                                    break;
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
                                //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Dport:no port" + "\n" });
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
                    //System.Windows.Forms.MessageBox.Show(ex.ToString());

                    if (!checkClose)
                    {   //Joining client disconnection cause this case run.
                        ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "One client join fail...\n" });
                    }
                    else
                        ((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Listen port close\n" });

                    if (stream != null)
                        stream.Close();
                    if (client != null)
                        client.Close();
                }

                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();

                Thread.Sleep(50);
            }
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
        }

        public void setVolume(int volume)
        {
            if (vlc.getPlayingState())
                vlc.setVolume(volume * 20);
        }

    }// end class     
}// end namespace
