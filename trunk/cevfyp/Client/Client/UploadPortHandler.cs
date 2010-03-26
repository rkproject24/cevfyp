using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClassLibrary;
using System.Net.NetworkInformation;

namespace Client
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct cPort
    {
        public int PortC;
        public TcpClient clientC;
        public int peerId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct dPort
    {
        public TcpClient clientD;
        public int PortD;
        public int peerId;
    }

    class UploadPortHandler
    {
        //static int TrackerSLPort = 1500;
        static int CHUNKLIST_CAPACITY = 0;
        static int PULL_CHUNK_PORT_BASE = 0;
        static int PULL_CHUNK_PORT_UP = 0;
        static int REPLY_CHUNK_TIMEOUT = 2000;
        static int REC_SEND_DIFF = 5;

        int max_client;
        int max_tree;
        int tempResult;
        int replyChunkPort = 0;
        bool localterminate = false;

        IPAddress localAddr;
        ChunkHandler ch;
        ClientConfig cConfig;

        private ClientForm clientFm;
        private ClientHandler clientMain;
        private delegate void UpdateTextCallback(string message);

        // List<List<cPort>> treeCPortList;
        // List<List<dPort>> treeDPortList;
        List<cPort> CPortList;
        List<dPort> DPortList;

        List<List<Chunk>> treeChunkList;
        List<List<int>> treeSeqList;
        List<Thread> CThreadList = new List<Thread>();
        List<Thread> DThreadList = new List<Thread>();
        //List<List<int>> treeIndexState;
        Thread replyChunkThread;
        TcpListener replyChunkListener = null;

        bool[,] readWriteReverse;
        int[] treeCLWriteIndex;
        int[] treeCLReadIndex;
        int[] treeCLCurrentSeq;
        int[] treeCLState;
        int[] DOfflineState;
        int[] treeCLLastSeq;
        TcpListener[] treeCPListener;
        TcpListener[] treeDPListener;

        public UploadPortHandler(ClientConfig cConfig, string serverip, ClientForm clientFm, int maxTree, ClientHandler clientMain)
        {
            this.clientFm = clientFm;
            this.max_client = cConfig.MaxPeer;
            this.max_tree = maxTree;
            this.clientMain = clientMain;

            localAddr = IPAddress.Parse(serverip);

            ch = new ChunkHandler();
            this.cConfig = cConfig;
            CHUNKLIST_CAPACITY = cConfig.ChunkCapacity;
            PULL_CHUNK_PORT_BASE = cConfig.LisPortup + 1;
            PULL_CHUNK_PORT_UP = cConfig.LisPortup + 20;

            //treeCPortList = new List<List<cPort>>(maxTree);
            //treeDPortList = new List<List<dPort>>(maxTree);
            CPortList = new List<cPort>(this.max_client);
            DPortList = new List<dPort>(this.max_client);

            treeChunkList = new List<List<Chunk>>(maxTree);
            treeSeqList = new List<List<int>>(maxTree);

            readWriteReverse = new bool[maxTree, this.max_client];
            treeCLWriteIndex = new int[maxTree];
            treeCLReadIndex = new int[maxTree];
            treeCLCurrentSeq = new int[maxTree];
            treeCLState = new int[maxTree];
            treeCLLastSeq = new int[maxTree];
            DOfflineState = new int[this.max_client];


            treeCPListener = new TcpListener[this.max_client];
            treeDPListener = new TcpListener[this.max_client];

            createTreeChunkList(maxTree, CHUNKLIST_CAPACITY);
            //createTreePortList(maxTree, this.max_client);
            // createTreeThreadList(maxTree, maxClient);
        }


        //private void createTreePortList(int maxTree, int maxClient)
        //{
        //    for (int i = 0; i < maxTree; i++)
        //    {
        //        List<cPort> CPortList = new List<cPort>(maxClient);
        //        List<dPort> DPortList = new List<dPort>(maxClient);
        //        treeCPortList.Add(CPortList);
        //        treeDPortList.Add(DPortList);

        //        readWriteReverse[i] = false;

        //    }
        //}


        private void createTreeChunkList(int maxTree, int chunkListCapacity)
        {
            Chunk ck = new Chunk();

            for (int i = 0; i < maxTree; i++)
            {
                List<Chunk> chunkList = new List<Chunk>(chunkListCapacity);
                List<int> seqList = new List<int>(chunkListCapacity);

                for (int j = 0; j < chunkListCapacity; j++)
                {
                    seqList.Add(0);
                    chunkList.Add(ck);
                }

                treeSeqList.Add(seqList);
                treeChunkList.Add(chunkList);
            }
        }

        //set chunk list state. [send video chunk(1) or send wait message(0)]
        public void setTreeCLState(int tree_index, int state)
        {
            treeCLState[tree_index] = state;
        }

        public int getTreeCLState(int tree_index)
        {
            return treeCLState[tree_index];
        }

        public void setChunkList(Chunk streamingChunk, int tree_index)
        {
            int write_index = treeCLWriteIndex[tree_index];

            Chunk sChunk = Chunk.Copy(streamingChunk);

            //if (treeChunkList[tree_index].Count <= (CHUNKLIST_CAPACITY - 1))
              //  treeChunkList[tree_index].Add(sChunk);
            //else
                treeChunkList[tree_index][write_index] = sChunk;

            treeSeqList[tree_index][write_index] = sChunk.seq;

            if (write_index == (CHUNKLIST_CAPACITY - 1))
            {
                treeCLWriteIndex[tree_index] = 0;

                for (int j = 0; j < max_client; j++)
                {
                    readWriteReverse[tree_index, j] = true;
                }

            }
            else
                treeCLWriteIndex[tree_index] += 1;
        }

        public TcpClient getTreeCListClient(int cList_index)
        {
            //return treeCPortList[tree_num - 1][cList_index].clientC;
            return CPortList[cList_index].clientC;

        }

        public TcpClient getTreeDListClient(int dList_index)
        {
            //return treeDPortList[tree_num - 1][dList_index].clientD;
            return DPortList[dList_index].clientD;
        }

        public int getTreeCListPort(int cList_index)
        {
            //return treeCPortList[tree_num - 1][cList_index].PortC;
            return CPortList[cList_index].PortC;
        }

        public int getTreeDListPort(int dList_index)
        {
            // return treeDPortList[tree_num - 1][dList_index].PortD;
            return DPortList[dList_index].PortD;
        }

        private void delClientFromTreeDList(int dList_index)
        {
            if (DPortList[dList_index].peerId != -1)
            {
                int port_num = DPortList[dList_index].PortD;

                DPortList[dList_index].clientD.Close();

                dPort dport = new dPort();
                dport.clientD = null;
                dport.PortD = port_num;
                dport.peerId = -1;

                DPortList[dList_index] = dport;
                //mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T:" + tree_index + " D:" + " unRge \n" });
            }
        }

        private void delClientFromTreeCList(int cList_index)
        {
            if (CPortList[cList_index].peerId != -1)
            {
                int port_num = CPortList[cList_index].PortC;

                CPortList[cList_index].clientC.Close();

                cPort cport = new cPort();
                cport.clientC = null;
                cport.PortC = port_num;
                cport.peerId = -1;

                CPortList[cList_index] = cport;
                //mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T:" + tree_index + " C:" + " unReg \n" });
            }
        }

        public void startTreePort()
        {
            localterminate = false; //indicate whether it is local terminate

            // for (int i = 0; i < max_tree; i++)
            //  {
            for (int j = 0; j < max_client; j++)
            {
                Thread CPortThread = new Thread(delegate() { TreePortHandle_Cport(j); });
                CPortThread.IsBackground = true;
                CPortThread.Name = " Cport_handle_" + "_" + j;
                CPortThread.Start();
                Thread.Sleep(20);
                CThreadList.Add(CPortThread);

                Thread DPortThread = new Thread(delegate() { TreePortHandle_Dport(j); });
                DPortThread.IsBackground = true;
                DPortThread.Name = " Dport_handle_" + "_" + j;
                DPortThread.Start();
                Thread.Sleep(20);
                DThreadList.Add(DPortThread);

            }

            // }

            //start pull chunk thread
            //replyChunkPort=TcpApps.RanPort(PULL_CHUNK_PORT_BASE,PULL_CHUNK_PORT_UP);

            replyChunkThread = new Thread(new ThreadStart(replyChunk));
            replyChunkThread.IsBackground = true;
            replyChunkThread.Name = "reply_Chunk";
            replyChunkThread.Start();
        }

        private void TreePortHandle_Dport(int DThreadList_index)
        {
            byte[] sendMessage = new byte[cConfig.ChunkSize];
            byte[] waitingMessage = new byte[cConfig.ChunkSize];

            int tempSeq = 0;// = tree_index + 1;// 0;
            int tempRead_index = 0;
            //int resultIndex = 0;
            bool firstRun = true;
            int tree_index = 0;
            int ran_port = TcpApps.RanPort(cConfig.Dport, cConfig.Dataportup);

            NetworkStream stream = null;
            TcpClient DPortClient = null;

            //treeDPListener[(tree_index * max_client) + DThreadList_index] = new TcpListener(localAddr, ran_port);

            bool portStarted = false;
            while (!portStarted)
            {
                try
                {
                    //if (ran_port == -1)
                    //{
                    //    ran_port = TcpApps.RanPort(cConfig.Dport, cConfig.Dataportup);
                    //   continue;
                    treeDPListener[DThreadList_index] = new TcpListener(localAddr, ran_port);
                    treeDPListener[DThreadList_index].Start(1); portStarted = true;
                    break;
                }
                catch (Exception ex)
                {
                    // ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "]" + ran_port +"\n"+ex.ToString() });
                    //Thread.Sleep(20);
                    ran_port = TcpApps.RanPort(cConfig.Dport, cConfig.Dataportup);
                    // treeDPListener[(tree_index * max_client) + DThreadList_index] = new TcpListener(localAddr, ran_port);
                }

                Thread.Sleep(10);
            }

            dPort dp = new dPort();
            dp.PortD = ran_port;
            dp.clientD = null;
            dp.peerId = -1;
            DPortList.Add(dp);

            while (true)
            {


                try
                {
                    DPortClient = treeDPListener[DThreadList_index].AcceptTcpClient();
                    DPortClient.NoDelay = true;
                    DPortClient.SendBufferSize = cConfig.ChunkSize;
                    stream = DPortClient.GetStream();

                    //get peer ip
                    IPAddress childAddress = ((IPEndPoint)DPortClient.Client.RemoteEndPoint).Address;

                    //get the peer id
                    byte[] responsePeerMsg = new byte[4];
                    stream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);
                    byte[] responsePeerMsg2 = new byte[MsgSize];
                    stream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                    string str = ByteArrayToString(responsePeerMsg2);
                    string[] messages = str.Split('@');


                    //get the tree index
                    stream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    tree_index = BitConverter.ToInt32(responsePeerMsg, 0);

                    string peerId = messages[0];
                    int listenPort = Int32.Parse(messages[1]);
                    int maxPeer = Int32.Parse(messages[2]);

                    dPort dpt = new dPort();
                    dpt.clientD = DPortClient;
                    dpt.PortD = ran_port;
                    dpt.peerId = Int32.Parse(peerId);
                    DPortList[DThreadList_index] = dpt;


                    //register the child
                    while (!registerToTracker(tree_index, peerId, childAddress, listenPort, "0", maxPeer))
                    {
                        //registerToTracker(tree_index, peerId, childAddress, listenPort, "0", maxPeer);
                        Random random = new Random();
                        Thread.Sleep(random.Next(50, 100));
                    }

                    firstRun = true;

                    while (true)
                    {
                        //if control port dead which cause this case happen
                        if (DPortList[DThreadList_index].clientD == null)
                        {
                            ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] D:" + ran_port + " exit~\n" });
                            stream.Close();
                            DPortClient.Close();
                            //firstRun = true;
                            break;
                        }

                        if (treeCLState[tree_index] == 2)
                        {
                            firstRun = true;
                            waitingMessage = System.Text.Encoding.ASCII.GetBytes(clientMain.waitMsg[tree_index]);//System.Text.Encoding.ASCII.GetBytes("Wait" + "@" + clientMain.getSelfID(tree_index));
                            stream.Write(waitingMessage, 0, waitingMessage.Length);
                            Thread.Sleep(20);
                            stream.Flush();
                            continue;


                        }
                        else if (treeCLState[tree_index] == 0)
                        {
                            firstRun = true;
                            waitingMessage = System.Text.Encoding.ASCII.GetBytes("Wait" + "@" + clientMain.getSelfID(tree_index) + "@");
                            stream.Write(waitingMessage, 0, waitingMessage.Length);
                            Thread.Sleep(20);
                            stream.Flush();
                            continue;

                        }

                        if (treeCLWriteIndex[tree_index] == 0 && firstRun)
                        {
                            Thread.Sleep(10);
                            continue;
                        }

                        //if (treeChunkList[tree_index].Count < 1)
                        //    continue;


                        if (firstRun == true)
                        {

                            if (treeCLWriteIndex[tree_index] != 0)
                                tempRead_index = treeCLWriteIndex[tree_index] - 1;
                            else
                                tempRead_index = CHUNKLIST_CAPACITY - 1;

                            ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "SendF:T[" + tree_index + "] " + tempRead_index + ":" + treeChunkList[tree_index][tempRead_index].seq + "\n" });

                            // treeCLLastSeq[tree_index] = treeChunkList[tree_index][tempRead_index].seq;
                            firstRun = false;
                            readWriteReverse[tree_index, DThreadList_index] = false;
                        }


                        if ((!readWriteReverse[tree_index, DThreadList_index] && tempRead_index < treeCLWriteIndex[tree_index]) || (readWriteReverse[tree_index, DThreadList_index] && tempRead_index < cConfig.ChunkCapacity))
                        {

                            int readWrite_different = 0;
                            if (treeCLWriteIndex[tree_index] >= tempRead_index)
                                readWrite_different = treeCLWriteIndex[tree_index] - tempRead_index;
                            else
                                readWrite_different = (treeCLWriteIndex[tree_index] + CHUNKLIST_CAPACITY) - tempRead_index;

                            if (!(readWrite_different > REC_SEND_DIFF))
                            {
                                sendMessage = ch.chunkToByte(treeChunkList[tree_index][tempRead_index], cConfig.ChunkSize);
                                stream.Write(sendMessage, 0, sendMessage.Length);
                                stream.Flush();
                                //((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "S:T[" + tree_index + "] " + tempRead_index + ":" + treeChunkList[tree_index][tempRead_index].seq + "\n" });

                            }
                            else
                                ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "Skip chunk:T[" + tree_index + "] " + tempRead_index + ":" + treeChunkList[tree_index][tempRead_index].seq + "\n" });

                            if (tempRead_index == (CHUNKLIST_CAPACITY - 1))
                            {
                                tempRead_index = 0;
                                readWriteReverse[tree_index, DThreadList_index] = false;
                            }
                            else
                                tempRead_index += 1;
                        }



                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {

                    if (ex.ToString().Contains("disposed object"))
                    {
                        ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] D:" + ran_port + " exit*\n" });

                        if (stream != null)
                            stream.Close();
                        if (DPortClient != null)
                            DPortClient.Close();

                        //firstRun = true;
                        Thread.Sleep(10);
                        continue;
                    }

                    ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] D:" + ran_port + " exit\n" });

                    if (!localterminate && DPortList[DThreadList_index].clientD != null)
                    {
                        DOfflineState[DThreadList_index] = 1;
                    }
                    if (stream != null)
                        stream.Close();
                    if (DPortClient != null)
                        DPortClient.Close();

                    //firstRun = true;

                    if (localterminate)
                        break;

                }
                Thread.Sleep(10);

            }
        }

        private void TreePortHandle_Cport(int CThreadList_index)
        {
            byte[] responseMessage = new byte[4];
            byte[] waitingMessage = new byte[4];
            int ran_port = TcpApps.RanPort(cConfig.CportBase, cConfig.Conportup);
            int tree_index = 0;
            TcpClient CPortClient = null;
            NetworkStream stream = null;
            //treeCPListener[(tree_index * max_client) + CThreadList_index] = new TcpListener(localAddr, ran_port);

            bool portStarted = false;
            while (!portStarted)
            {
                try
                {
                    // if (ran_port == -1)
                    // {
                    //     ran_port = TcpApps.RanPort(cConfig.CportBase, cConfig.Conportup);
                    //     continue;
                    // }
                    treeCPListener[CThreadList_index] = new TcpListener(localAddr, ran_port);
                    treeCPListener[CThreadList_index].Start(1);
                    portStarted = true;
                }
                catch (Exception ex)
                {
                    //((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "]" + ran_port + "\n" + ex.ToString() });
                    // Thread.Sleep(20);
                    ran_port = TcpApps.RanPort(cConfig.CportBase, cConfig.Conportup);
                    //treeCPListener[(tree_index * max_client) + CThreadList_index] = new TcpListener(localAddr, ran_port); //ran a new port if crash
                }
                Thread.Sleep(10);
            }

            cPort cp = new cPort();
            cp.PortC = ran_port;
            cp.clientC = null;
            CPortList.Add(cp);


            while (true)
            {


                try
                {

                    CPortClient = treeCPListener[CThreadList_index].AcceptTcpClient();
                    //  CPortClient.NoDelay = true;
                    stream = CPortClient.GetStream();

                    //get child peer info
                    byte[] responsePeerMsg = new byte[4];
                    stream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);
                    byte[] responsePeerMsg2 = new byte[MsgSize];
                    stream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                    string peerId = ByteArrayToString(responsePeerMsg2);

                    stream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    tree_index = BitConverter.ToInt32(responsePeerMsg, 0);

                    cPort cpt = new cPort();
                    cpt.clientC = CPortClient;
                    cpt.PortC = ran_port;
                    cpt.peerId = Int32.Parse(peerId);
                    CPortList[CThreadList_index] = cpt;

                    stream.ReadTimeout = 3000;
                    stream.WriteTimeout = 3000;
                    while (true)
                    {

                        int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
                        string responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);

                        if (responseString.Equals("Exit") || DOfflineState[CThreadList_index] == 1)
                        {
                            ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] C:" + ran_port + " exit~ [" + responseString + ":" + DOfflineState[CThreadList_index].ToString() + "]\n" });
                            unregister(tree_index, CPortList[CThreadList_index].peerId);
                            ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] Peer:" + CPortList[CThreadList_index].peerId + " unRge~\n" });
                            delClientFromTreeDList(CThreadList_index);
                            delClientFromTreeCList(CThreadList_index);
                            DOfflineState[CThreadList_index] = 0;

                            stream.Close();
                            CPortClient.Close();
                            break;
                        }

                        if (responseString.Equals("Wait"))
                        {
                            Thread.Sleep(20);
                            continue;
                        }

                        if (responseString.Equals("Conn"))
                        {
                            waitingMessage = System.Text.Encoding.ASCII.GetBytes("Nooo");

                            for (int i = 0; i < max_client; i++)
                            {
                                if (getTreeCListClient(i) == null && getTreeDListClient(i) == null)
                                {
                                    waitingMessage = System.Text.Encoding.ASCII.GetBytes("Have");
                                    break;
                                }
                            }

                            stream.Write(waitingMessage, 0, waitingMessage.Length);
                            stream.Flush();

                        }

                        Thread.Sleep(20);
                    }
                }
                catch (Exception ex)
                {
                    ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] C:" + ran_port + "exit\n" });

                    if (!localterminate)
                    {
                        unregister(tree_index, CPortList[CThreadList_index].peerId);
                        ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] Peer:" + CPortList[CThreadList_index].peerId + " unRge \n" });
                        delClientFromTreeDList(CThreadList_index);
                        delClientFromTreeCList(CThreadList_index);
                        DOfflineState[CThreadList_index] = 0;
                    }

                    if (stream != null)
                        stream.Close();

                    if (CPortClient != null)
                        CPortClient.Close();

                    if (localterminate)
                        break;

                }
                Thread.Sleep(10);
            }
        }

        private bool unregister(int tree, int peerId)
        {
            while (true)
            {
                if (peerId != -1)
                {
                    TcpClient connectTracker = null;
                    NetworkStream connectTrackerStream = null;
                    try
                    {
                        // mainFm.tbTracker.Text, TrackerSLPort
                        connectTracker = new TcpClient(((LoggerFrm)clientFm.downloadFrm).tbIP.Text, cConfig.TrackerPort);
                        connectTrackerStream = connectTracker.GetStream();


                        //define client message type
                        byte[] clienttype = StrToByteArray("<unRegists>");
                        connectTrackerStream.Write(clienttype, 0, clienttype.Length);

                        string sendstr = tree + "@" + peerId + "@" + clientMain.getSelfID(tree);//add the selfid for tracker to check who start unreg event
                        byte[] sendbyte = StrToByteArray(sendstr);
                        //connectTrackerStream.Write(sendbyte, 0, sendbyte.Length);

                        byte[] MsgLength = BitConverter.GetBytes(sendstr.Length);
                        connectTrackerStream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                        connectTrackerStream.Write(sendbyte, 0, sendbyte.Length);

                        connectTrackerStream.Close();
                        connectTracker.Close();


                        break;
                    }
                    catch
                    {
                        ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree + "] Peer:" + peerId + " unRge fail\n" });

                        if (connectTrackerStream != null)
                            connectTrackerStream.Close();
                        if (connectTracker != null)
                            connectTracker.Close();
                        Thread.Sleep(20);
                        // return false;
                    }
                }

            }
            return true;
        }

        // Write for tracker registration.
        private bool registerToTracker(int tree, string peerId, IPAddress childAddress, int listenPort, string layer, int maxPeer)
        {
            TcpClient connectTracker;
            NetworkStream connectTrackerStream;
            try
            {
                //((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree + "] Peer:" + peerId + " register start\n" });
                connectTracker = new TcpClient(((LoggerFrm)clientFm.downloadFrm).tbIP.Text, cConfig.TrackerPort);
                connectTrackerStream = connectTracker.GetStream();

                //define client message type
                Byte[] clienttype = StrToByteArray("<cRegister>");
                connectTrackerStream.Write(clienttype, 0, clienttype.Length);

                ////send id
                //Byte[] idbyte = StrToByteArray(selfid);
                //connectTrackerStream.Write(idbyte, 0, idbyte.Length);

                //Byte[] maxClient = StrToByteArray(this.cConfig.MaxPeer.ToString());
                //connectTrackerStream.Write(maxClient, 0, maxClient.Length);

                ////connectTrackerStream.Write(MsgLength, 0, MsgLength.Length);
                //Byte[] clientLayer = StrToByteArray(layerT1);
                //connectTrackerStream.Write(clientLayer, 0, clientLayer.Length);

                //clientLayer = StrToByteArray(layerT2);
                //connectTrackerStream.Write(clientLayer, 0, clientLayer.Length);


                //string sendstr = listenPort + "@" + tree + "@" + peerId + "@" + this.cConfig.MaxPeer + "@" + layer;
                string sendstr = childAddress.ToString() + "@" + listenPort + "@" + tree + "@" + peerId + "@" + maxPeer + "@" + layer + "@" + clientMain.getSelfID(tree);
                Byte[] sendbyte = StrToByteArray(sendstr);
                //connectTrackerStream.Write(sendbyte, 0, sendbyte.Length);

                byte[] MsgLength = BitConverter.GetBytes(sendstr.Length);
                connectTrackerStream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                connectTrackerStream.Write(sendbyte, 0, sendbyte.Length);

                connectTracker.Close();
                connectTrackerStream.Close();
                ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree + "] Peer:" + peerId + " registered\n" });
            }
            catch (Exception ex)
            {
                ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "register ex:\n" + ex });
                return false;
            }

            return true;
        }

        private void replyChunk()
        {
            TcpClient client = null;
            NetworkStream stream = null;
            int target_tree, result_index, reqSeq;
            byte[] responseData = new byte[cConfig.ChunkSize - 182];
            Chunk ck = new Chunk();
            ck = ch.streamingToChunk(cConfig.ChunkSize - 182, responseData, 0);

            bool portStarted = false;

            replyChunkPort = TcpApps.RanPort(PULL_CHUNK_PORT_BASE, PULL_CHUNK_PORT_UP);
            while (!portStarted)
            {

                try
                {

                    replyChunkListener = new TcpListener(localAddr, replyChunkPort);
                    replyChunkListener.Start(1);
                    portStarted = true;
                }
                catch (Exception ex)
                {
                    replyChunkPort = TcpApps.RanPort(PULL_CHUNK_PORT_BASE, PULL_CHUNK_PORT_UP);
                }
                Thread.Sleep(10);
            }

            printOnUL_PULL("IP:" + localAddr.ToString() + "\nPort[" + replyChunkPort.ToString() + "]:Listening~\n");

            while (true)
            {
                try
                {
                    printOnUL_PULL("Pull_waiting\n");
                    //((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "Pull_waiting\n" });
                    client = replyChunkListener.AcceptTcpClient();
                    client.NoDelay = true;
                    client.SendBufferSize = cConfig.ChunkSize;

                    stream = client.GetStream();
                    stream.ReadTimeout = REPLY_CHUNK_TIMEOUT;
                    stream.WriteTimeout = 2000;

                    while (true)
                    {
                        printOnUL_PULL("Pull_receive\n");
                        //((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "Pull_receive\n" });
                        byte[] sendMessage = new byte[cConfig.ChunkSize];
                        byte[] responseMessage2 = new byte[4];
                        stream.Read(responseMessage2, 0, responseMessage2.Length);
                        reqSeq = BitConverter.ToInt16(responseMessage2, 0);

                        printOnUL_PULL("Seq:" + reqSeq + "\n");
                        //((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "Seq:" + reqSeq + "\n" });

                        if (reqSeq > 0)
                        {
                            target_tree = clientMain.calcTreeIndex(reqSeq);


                            result_index = search(target_tree, 0, cConfig.ChunkCapacity - 1, reqSeq);

                            if (result_index != -1)
                                sendMessage = ch.chunkToByte(treeChunkList[target_tree][result_index], cConfig.ChunkSize);
                            else
                                sendMessage = ch.chunkToByte(ck, cConfig.ChunkSize);

                            stream.Write(sendMessage, 0, sendMessage.Length);

                            stream.Flush();

                            if (result_index != -1)
                                printOnUL_PULL("uploadChunk:" + reqSeq + "\n");

                            else
                            {
                                printOnUL_PULL("uploadChunk:" + 0 + "\n");
                                //printOnUL_PULL("uploadsize:" + sendMessage.Length + "\n");
                            }
                        }
                        else
                        {
                            printOnUL_PULL("Pull_receive:" + reqSeq + "\n");
                            sendMessage = ch.chunkToByte(ck, cConfig.ChunkSize);
                            stream.Write(sendMessage, 0, sendMessage.Length);
                            stream.Flush();
                            printOnUL_PULL("uploadChunk:" + 0 + "~\n");

                            //break;
                        }
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show("replyThread:"+ex.ToString());

                    if (!localterminate)
                        printOnUL_PULL("Peer_fail" + "\n");
                    // ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "Peer_fail"+ex.ToString()+"\n" });
                    else
                        printOnUL_PULL("Listen port close~\n");
                    //((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "Listen port close~\n" });

                }

                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();

                Thread.Sleep(10);
            }


        }

        public void printOnUL_PULL(string message)
        {
            ((LoggerFrm)clientFm.uploadFrm).rtbpull.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbULPull), new object[] { message });
        }

        public int getReplyChunkPort()
        {
            return replyChunkPort;
        }

        private int searchChunk(int list_index, int rIndex, int wIndex, int target)
        {
            if (wIndex < rIndex)
            {
                tempResult = search(list_index, rIndex, cConfig.ChunkCapacity - 1, target);
                if (tempResult != -1)
                    return tempResult;
                else
                {
                    tempResult = search(list_index, 0, wIndex - 1, target);
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
                // if (treeChunkList[list_index][r_index].seq == target)
                if (treeSeqList[list_index][r_index] == target)
                    return r_index;

                r_index += 1;
            }
            return -1;
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }
        private static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public void closeCDPortThread()
        {
            this.localterminate = true;

            replyChunkListener.Stop();
            replyChunkThread.Abort();

            for (int i = 0; i < max_tree; i++)
            {
                for (int j = 0; j < max_client; j++)
                {
                    readWriteReverse[i, j] = false;
                }
            }

            for (int j = 0; j < CThreadList.Count; j++)
            {
                treeDPListener[j].Stop();
                DThreadList[j].Abort();

                treeCPListener[j].Stop();
                CThreadList[j].Abort();
            }


            for (int i = 0; i < max_tree; i++)
            {
                treeChunkList[i].Clear();
                treeCLWriteIndex[i] = 0;
                treeCLReadIndex[i] = 0;
                treeCLCurrentSeq[i] = 0;

            }
        }


    }//end class
}
