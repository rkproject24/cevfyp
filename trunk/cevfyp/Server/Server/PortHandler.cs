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

namespace Server
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

    class PortHandler
    {
        //static int TrackerSLPort = 1500;
        static int CHUNKLIST_CAPACITY = 500;
        static int PULL_CHUNK_PORT_BASE = 0;
        static int PULL_CHUNK_PORT_UP = 0;
        static int REPLY_PULL_READ_TIMEOUT = 2000;
        static int REPLY_PULL_WRITE_TIMEOUT = 2000;
        static int REC_SEND_DIFF = 25;

        int max_client;
        int max_tree;

        bool localterminate = false;

        IPAddress localAddr;
        ChunkHandler ch;
        ServerConfig sConfig;
        VlcHandler vlchandle;

        private ServerFrm mainFm;
        private delegate void UpdateTextCallback(string message);

        //List<List<cPort>> treeCPortList;
        //List<List<dPort>> treeDPortList;
        List<cPort> CPortList;
        List<dPort> DPortList;

        List<List<Chunk>> treeChunkList;
        List<Thread> CThreadList = new List<Thread>();
        List<Thread> DThreadList = new List<Thread>();
        List<List<int>> treeSeqList;
        // List<List<Thread>> treeCThreadList;
        //List<List<Thread>> treeDThreadList;
        //List<List<TcpListener>> treeCPListener;
        //List<List<TcpListener>> treeDPListener;
        Thread replyChunkThread;
        int replyChunkPort;
        TcpListener replyChunkListener = null;

        bool[] readWriteReverse;
        int[] treeCLWriteIndex;
        int[] treeCLReadIndex;
        int[] treeCLCurrentSeq;
        int[] treeCLState;
        int[] DOfflineState;
        TcpListener[] treeCPListener;
        TcpListener[] treeDPListener;

        int channelid;
        int bitRates;

        public int BitRates
        {
            get { return bitRates; }
            set { bitRates = value; }

        }



        public int Channelid
        {
            get { return channelid; }
            set { channelid = value; }
        }

        public PortHandler( int maxClient, string serverip, ServerFrm mainFm,VlcHandler vlchandle)
        {
            this.mainFm = mainFm;
            this.max_client = maxClient;
            this.vlchandle = vlchandle;
            //this.max_tree = maxTree;
            localAddr = IPAddress.Parse(serverip);

            ch = new ChunkHandler();
            sConfig = new ServerConfig();
            sConfig.load("C:\\ServerConfig");
            PULL_CHUNK_PORT_BASE = sConfig.SLisPortup + 1;
            PULL_CHUNK_PORT_UP = sConfig.SLisPortup + 20;
        }

        public void init(int maxTree)
        {
            this.max_tree = maxTree;
            //treeCPortList = new List<List<cPort>>(maxTree);
            //treeDPortList = new List<List<dPort>>(maxTree);
            CPortList = new List<cPort>(max_client);
            DPortList = new List<dPort>(max_client);
            treeSeqList = new List<List<int>>(max_tree);

            treeChunkList = new List<List<Chunk>>(max_tree);
            //treeCThreadList = new List<List<Thread>>(maxTree);
            // treeDThreadList = new List<List<Thread>>(maxTree);
            //treeCPListener = new List<List<TcpListener>>(maxTree);
            //treeDPListener = new List<List<TcpListener>>(maxTree);

            readWriteReverse = new bool[max_tree];
            treeCLWriteIndex = new int[max_tree];
            treeCLReadIndex = new int[max_tree];
            treeCLCurrentSeq = new int[max_tree];
            treeCLState = new int[max_tree];
            DOfflineState = new int[max_client];

            //treeCPListener = new TcpListener[maxTree * maxClient];
            //treeDPListener = new TcpListener[maxTree * maxClient];
            treeCPListener = new TcpListener[max_client];
            treeDPListener = new TcpListener[max_client];

            createTreeChunkList(max_tree, CHUNKLIST_CAPACITY);
            //createTreePortList(maxTree, maxClient);
            // createTreeThreadList(maxTree, maxClient);  
        }

        //private void createTreePortList(int maxTree,int maxClient)
        //{
        //    for (int i = 0; i < maxTree; i++)
        //    {
        //        //List<cPort> CPortList = new List<cPort>(maxClient);
        //        //List<dPort> DPortList = new List<dPort>(maxClient);
        //        //treeCPortList.Add(CPortList);
        //        //treeDPortList.Add(DPortList);

        //        //readWriteReverse[i] = false;

        //       // List<TcpListener> CPListener = new List<TcpListener>(maxClient);
        //       // List<TcpListener> DPListener = new List<TcpListener>(maxClient);
        //        //treeCPListener.Add(CPListener);
        //        //treeDPListener.Add(DPListener);
        //    }
        //}

        /*
        public void createTreeThreadList(int maxTree, int maxClient)
        {
            for (int i = 0; i < maxTree; i++)
            {
               List<Thread> CThreadList = new List<Thread>(max_client);
               List<Thread> DThreadList = new List<Thread>(max_client);
               treeCThreadList.Add(CThreadList);
               treeDThreadList.Add(DThreadList);
            }
        }
        */

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


        public void setChunkList(Chunk streamingChunk, int tree_index)
        {
            int write_index = treeCLWriteIndex[tree_index];

            Chunk sChunk = Chunk.Copy(streamingChunk);

            //if (treeChunkList[tree_index].Count <= (CHUNKLIST_CAPACITY - 1))
            //    treeChunkList[tree_index].Add(sChunk);
            //else
            treeChunkList[tree_index][write_index] = sChunk;

            treeSeqList[tree_index][write_index] = sChunk.seq;

            if (write_index == (CHUNKLIST_CAPACITY - 1))
            {
                treeCLWriteIndex[tree_index] = 0;
                //readWriteReverse[tree_index] = true;
            }
            else
                treeCLWriteIndex[tree_index] += 1;

            treeCLCurrentSeq[tree_index] = sChunk.seq;

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

            //for (int i = 0; i < max_tree; i++)
            // {
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

            // treeCThreadList.Add(CThreadList);
            // treeDThreadList.Add(DThreadList);
            //}

            //start pull chunk thread
            replyChunkPort = TcpApps.RanPort(PULL_CHUNK_PORT_BASE, PULL_CHUNK_PORT_UP);

            replyChunkThread = new Thread(new ThreadStart(replyChunk));
            replyChunkThread.IsBackground = true;
            replyChunkThread.Name = "reply_Chunk";
            replyChunkThread.Start();
        }

        private void TreePortHandle_Dport(int DThreadList_index)
        {
            byte[] sendMessage = new byte[sConfig.ChunkSize];
            byte[] waitingMessage = new byte[sConfig.ChunkSize];

            int tempSeq = 0;//= tree_index + 1;
            int tempRead_index = 0;
            bool firstRun = true;
            int tree_index = 0;
            int ran_port = TcpApps.RanPort(sConfig.Dport, sConfig.Dataportup);

            NetworkStream stream = null;
            TcpClient DPortClient = null;

            // treeDPListener[(tree_index * max_client) + DThreadList_index] = new TcpListener(localAddr, ran_port);

            bool portStarted = false;

            while (!portStarted)
            {
                try
                {
                    treeDPListener[DThreadList_index] = new TcpListener(localAddr, ran_port);
                    treeDPListener[DThreadList_index].Start(1);
                    portStarted = true;
                }
                catch (Exception ex)
                {
                    // mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree_index + "]" + ex.ToString() });
                    ran_port = TcpApps.RanPort(sConfig.Dport, sConfig.Dataportup);
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
                    //DPortClient = DportListener.AcceptTcpClient();
                    //DPortClient = treeDPListener[tree_index][DThreadList_index].AcceptTcpClient();
                    DPortClient = treeDPListener[DThreadList_index].AcceptTcpClient();
                    DPortClient.NoDelay = true;
                    //DPortClient.SendBufferSize = sConfig.ChunkSize;
                    stream = DPortClient.GetStream();
                    mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + DThreadList_index + "] peer in\n" });

                    //get peer ip
                    IPAddress childAddress = ((IPEndPoint)DPortClient.Client.RemoteEndPoint).Address;
                    //mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "Peer in\n" });
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
                    //mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + DThreadList_index + "]register be4\n" });
                    while (!registerToTracker(tree_index, peerId, childAddress, listenPort, "0"))
                    {
                        Random random = new Random();
                        Thread.Sleep(random.Next(30, 60));
                    }
                    //mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + DThreadList_index + "]register after\n" });
                    while (true)
                    {
                        //if control port dead which cause this case happen
                        if (DPortList[DThreadList_index].clientD == null)
                        {
                            mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree_index + "] D:" + ran_port + " exit~\n" });
                            stream.Close();
                            DPortClient.Close();
                            firstRun = true;
                            break;
                        }

                        if (treeCLState[tree_index] == 0)
                        {
                            //DateTime dt = DateTime.Now;
                            //dt.ToBinary();

                            waitingMessage = System.Text.Encoding.ASCII.GetBytes("Wait" + "@0@");
                            //  stream.Write(waitingMessage, 0, waitingMessage.Length);

                            // waitingMessage = System.Text.Encoding.ASCII.GetBytes("Wait");
                            stream.Write(waitingMessage, 0, waitingMessage.Length);
                            Thread.Sleep(10);
                            stream.Flush();
                            continue;
                        }

                        //if (treeChunkList[tree_index].Count < 1)
                        //{
                        //    Thread.Sleep(10);
                        //    continue;
                        //}

                        if (treeCLWriteIndex[tree_index] == 0 && firstRun)
                        {
                            Thread.Sleep(10);
                            continue;
                        }

                        if (firstRun == true)
                        {
                            tempSeq = treeCLCurrentSeq[tree_index];
                            tempRead_index = treeCLWriteIndex[tree_index] - 1;
                            firstRun = false;
                        }



                        //by yam: not seach method
                        if (tempSeq <= treeCLCurrentSeq[tree_index])
                        // if ((!readWriteReverse[tree_index] && tempRead_index < treeCLWriteIndex[tree_index]) || (readWriteReverse[tree_index] && tempRead_index < CHUNKLIST_CAPACITY))
                        {
                            int readWrite_different = 0;
                            if (treeCLWriteIndex[tree_index] > tempRead_index)
                                readWrite_different = treeCLWriteIndex[tree_index] - tempRead_index;
                            else
                                readWrite_different = (treeCLWriteIndex[tree_index] + CHUNKLIST_CAPACITY) - tempRead_index;


                            if (!(readWrite_different > REC_SEND_DIFF))
                            {
                                Chunk tempChunk = treeChunkList[tree_index][tempRead_index];
                                sendMessage = ch.chunkToByte(tempChunk, sConfig.ChunkSize);
                                stream.Write(sendMessage, 0, sendMessage.Length);
                                stream.Flush();
                            }
                            else
                                mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "Skip chunk: " + tempRead_index + "\n" });

                            if (tempSeq == 2147483647)
                                tempSeq = tree_index + 1;
                            else
                                tempSeq += max_tree;

                            if (tempRead_index == (CHUNKLIST_CAPACITY - 1))
                            {
                                tempRead_index = 0;
                                //readWriteReverse[tree_index] = false;
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
                        mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree_index + "] D:" + ran_port + " exit*\n" });

                        if (stream != null)
                            stream.Close();
                        if (DPortClient != null)
                            DPortClient.Close();

                        firstRun = true;
                        Thread.Sleep(10);
                        continue;
                    }

                    mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree_index + "] D:" + ran_port + " exit\n" });

                    if (!localterminate && DPortList[DThreadList_index].clientD != null)
                    {
                        DOfflineState[DThreadList_index] = 1;

                    }
                    if (stream != null)
                        stream.Close();

                    if (DPortClient != null)
                        DPortClient.Close();

                    firstRun = true;
                }

                Thread.Sleep(10);
            }
        }

        private void TreePortHandle_Cport(int CThreadList_index)
        {

            byte[] waitingMessage = new byte[4];
            byte[] responseMessage = new byte[4];
            int ran_port = TcpApps.RanPort(sConfig.CportBase, sConfig.Conportup);
            int tree_index = 0;
            TcpClient CPortClient = null;
            NetworkStream stream = null;

            bool portStarted = false;

            while (!portStarted)
            {
                try
                {
                    treeCPListener[CThreadList_index] = new TcpListener(localAddr, ran_port);
                    treeCPListener[CThreadList_index].Start(1);
                    portStarted = true;
                }
                catch (Exception ex)
                {
                    //mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree_index + "]" + ex.ToString() });
                    ran_port = TcpApps.RanPort(sConfig.CportBase, sConfig.Conportup);
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
                    stream = CPortClient.GetStream();

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
                        stream.Flush();

                        if (responseString.Equals("Exit") || DOfflineState[CThreadList_index] == 1)
                        {
                            // if (DOfflineState[(tree_index * max_client) + CThreadList_index] == 1)
                            //Thread.Sleep(800);

                            mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree_index + "] C:" + ran_port + " exit~ [" + responseString + ":" + DOfflineState[CThreadList_index].ToString() + "]\n" });
                            unregister(tree_index, CPortList[CThreadList_index].peerId);
                            mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree_index + "] Peer:" + CPortList[CThreadList_index].peerId + " unRge~ \n" });
                            delClientFromTreeDList(CThreadList_index);
                            delClientFromTreeCList(CThreadList_index);
                            DOfflineState[CThreadList_index] = 0;

                            stream.Close();
                            CPortClient.Close();
                            break;
                        }

                        if (responseString.Equals("Wait"))
                        {
                            //send the bitrate of current video
                            string sendstr = "bitRate@" + bitRates;//vlchandle.getBitRate();
                            byte[] sendbyte = StrToByteArray(sendstr);
                            byte[] MsgLength = BitConverter.GetBytes(sendstr.Length);
                            stream.Write(MsgLength, 0, MsgLength.Length); //send size of id
                            //Thread.Sleep(10);
                            stream.Write(sendbyte, 0, sendbyte.Length);

                          //  mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "sendBitRate" });
                            Thread.Sleep(20);
                            continue;
                        }

                        Thread.Sleep(20);
                    }
                }
                catch
                {
                    mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree_index + "] C:" + ran_port + "exit\n" });

                    if (!localterminate)
                    {
                        unregister(tree_index, CPortList[CThreadList_index].peerId);
                        mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree_index + "] Peer:" + CPortList[CThreadList_index].peerId + " unRge \n" });
                        delClientFromTreeDList(CThreadList_index);
                        delClientFromTreeCList(CThreadList_index);
                        DOfflineState[CThreadList_index] = 0;
                    }

                    if (stream != null)
                        stream.Close();

                    if (CPortClient != null)
                        CPortClient.Close();
                }

                Thread.Sleep(10);
            }
        }

        private bool unregister(int tree, int peerId)
        {
            if (peerId != -1)
            {
                TcpClient connectTracker = null;
                NetworkStream connectTrackerStream = null;
                try
                {
                    // mainFm.tbTracker.Text, TrackerSLPort
                    connectTracker = new TcpClient(mainFm.tbTracker.Text, sConfig.TrackerPort);
                    connectTrackerStream = connectTracker.GetStream();

                    //define client message type
                    Byte[] clienttype = StrToByteArray("<unRegists>");
                    connectTrackerStream.Write(clienttype, 0, clienttype.Length);

                    string sendstr = channelid+"@"+ tree + "@" + peerId + "@0";
                    Byte[] sendbyte = StrToByteArray(sendstr);
                    //connectTrackerStream.Write(sendbyte, 0, sendbyte.Length);

                    byte[] MsgLength = BitConverter.GetBytes(sendstr.Length);
                    connectTrackerStream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                    connectTrackerStream.Write(sendbyte, 0, sendbyte.Length);


                    connectTrackerStream.Close();
                    connectTracker.Close();
                }
                catch
                {
                    mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T[" + tree + "] Peer:" + peerId + " unRge fail\n" });

                    if (connectTrackerStream != null)
                        connectTrackerStream.Close();
                    if (connectTracker != null)
                        connectTracker.Close();

                    return false;
                }
            }

            return true;
        }
        private void replyChunk()
        {
            TcpClient client = null;
            NetworkStream stream = null;
            int target_tree, result_index, reqSeq;
            byte[] responseData = new byte[sConfig.ReceiveStreamSize];
            Chunk ck = new Chunk();
            ck = ch.streamingToChunk(sConfig.ReceiveStreamSize, responseData, 0);

            bool portStarted = false;
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

            mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "IP:" + localAddr.ToString() + "\nPort[" + replyChunkPort.ToString() + "]:Listening~\n" });

            while (true)
            {
                try
                {
                    mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_waiting\n" });

                    client = replyChunkListener.AcceptTcpClient();
                    client.NoDelay = true;
                    //client.SendBufferSize = sConfig.ChunkSize;
                    stream = client.GetStream();
                    stream.ReadTimeout = REPLY_PULL_READ_TIMEOUT;
                    stream.WriteTimeout = REPLY_PULL_WRITE_TIMEOUT;

                    // mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_ready\n" });

                    while (true)
                    {
                        mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_receive\n" });

                        byte[] sendMessage = new byte[sConfig.ChunkSize];
                        byte[] responseMessage2 = new byte[4];
                        //Chunk resultChunk = new Chunk();

                        stream.Read(responseMessage2, 0, responseMessage2.Length);
                        stream.Flush();
                        reqSeq = BitConverter.ToInt16(responseMessage2, 0);

                        mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Seq:" + reqSeq + "\n" });

                        if (reqSeq > 0)
                        {
                            target_tree = calcTreeIndex(reqSeq);

                            // resultChunk = searchReqChunk(target_tree, reqSeq);

                            // sendMessage = ch.chunkToByte(resultChunk, sConfig.ChunkSize);
                            // stream.Write(sendMessage, 0, sendMessage.Length);
                            // stream.Flush();

                            result_index = search(target_tree, 0, CHUNKLIST_CAPACITY - 1, reqSeq);

                            if (result_index != -1)
                                sendMessage = ch.chunkToByte(treeChunkList[target_tree][result_index], sConfig.ChunkSize);
                            else
                                sendMessage = ch.chunkToByte(ck, sConfig.ChunkSize);

                            stream.Write(sendMessage, 0, sendMessage.Length);
                            stream.Flush();

                            if (result_index != -1)
                                mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "uploadedChunk:" + treeChunkList[target_tree][result_index].seq + "\n" });
                            else
                                mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "uploadedChunk:" + 0 + "\n" });

                        }
                        else
                        {

                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_receive:" + reqSeq + "\n" });

                            sendMessage = ch.chunkToByte(ck, sConfig.ChunkSize);
                            stream.Write(sendMessage, 0, sendMessage.Length);
                            stream.Flush();

                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "uploadedChunk:" + 0 + "~\n" });

                            //break;
                        }

                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show("replyThread:"+ex.ToString());

                    if (!localterminate)
                        mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_fail\n" });
                    else
                        mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "Listen port close~\n" });
                }

                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();

                Thread.Sleep(10);
            }


        }


        /*private void replyChunk()
        {
            TcpClient client = null;
            NetworkStream stream = null;
            int target_tree, result_index, reqSeq;
            byte[] responseData = new byte[sConfig.ReceiveStreamSize];
            //byte[] receiveMessage = new byte[4];
            //byte[] reponseMessage = new byte[4];
            Chunk ck = new Chunk();
            ck = ch.streamingToChunk(sConfig.ReceiveStreamSize, responseData, 0);

            bool portStarted = false;
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

            mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "IP:" + localAddr.ToString() + "\nPort[" + replyChunkPort.ToString() + "]:Listening~\n" });

            while (true)
            {
                try
                {
                    mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_waiting\n" });

                    client = replyChunkListener.AcceptTcpClient();
                    client.NoDelay = true;
                    // client.SendBufferSize = sConfig.ChunkSize;
                    stream = client.GetStream();
                    stream.ReadTimeout = REPLY_CHUNK_TIMEOUT;
                    stream.WriteTimeout = 2000;

                    mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_receive\n" });

                    while (true)
                    {
                        // mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_receive\n" });

                        byte[] sendMessage = new byte[sConfig.ChunkSize];

                        byte[] receiveMessage = new byte[4];
                        stream.Read(receiveMessage, 0, receiveMessage.Length);
                        stream.Flush();
                        //string responType = ByteArrayToString(receiveMessage);
                        reqSeq = BitConverter.ToInt16(receiveMessage, 0);

                        //if (responType.Equals("wait"))
                        if (reqSeq == -1)
                        {
                            //reponseMessage = StrToByteArray("wait");
                            byte[] reponseMessage = BitConverter.GetBytes(-1);
                            stream.Write(reponseMessage, 0, reponseMessage.Length);
                            stream.Flush();
                            Thread.Sleep(10);
                            continue;
                        }

                        //reqSeq = Convert.ToInt32(responType);

                        mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Seq:" + reqSeq + "\n" });

                        if (reqSeq > 0)
                        {
                            target_tree = calcTreeIndex(reqSeq);

                            // resultChunk = searchReqChunk(target_tree, reqSeq);
                            // sendMessage = ch.chunkToByte(resultChunk, sConfig.ChunkSize);
                            // stream.Write(sendMessage, 0, sendMessage.Length);
                            // stream.Flush();

                            result_index = search(target_tree, 0, CHUNKLIST_CAPACITY - 1, reqSeq);

                            if (result_index != -1)
                                sendMessage = ch.chunkToByte(treeChunkList[target_tree][result_index], sConfig.ChunkSize);
                            else
                                sendMessage = ch.chunkToByte(ck, sConfig.ChunkSize);

                            stream.Write(sendMessage, 0, sendMessage.Length);
                            stream.Flush();

                            if (result_index != -1)
                                mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "uploadedChunk:" + treeChunkList[target_tree][result_index].seq + "\n" });
                            else
                                mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "uploadedChunk:" + 0 + "\n" });

                        }
                        else
                        {

                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_receive:" + reqSeq + "\n" });

                            sendMessage = ch.chunkToByte(ck, sConfig.ChunkSize);
                            stream.Write(sendMessage, 0, sendMessage.Length);
                            stream.Flush();

                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "uploadedChunk:" + 0 + "~\n" });

                            //break;
                        }

                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show("replyThread:"+ex.ToString());

                    if (!localterminate)
                        mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Pull_fail\n" });
                    else
                        mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "Listen port close~\n" });


                }

                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();

                Thread.Sleep(10);
            }


        }
        */

        public Chunk searchReqChunk(int target_tree, int reqSeq)
        {
            Chunk ck = new Chunk();
            ck.seq = 0;
            int result_index;

            if (treeChunkList[target_tree].Count < CHUNKLIST_CAPACITY)
                result_index = search(target_tree, 0, treeCLWriteIndex[target_tree], reqSeq);
            else
                result_index = search(target_tree, 0, CHUNKLIST_CAPACITY - 1, reqSeq);

            if (result_index != -1)
                return treeChunkList[target_tree][result_index];
            else
                return ck;

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

        //private int search(List<Chunk> list, int rIndex, int wIndex, int target)
        //{
        //    int lb, ub, tempResult;
        //    if (wIndex < rIndex)
        //    {
        //        lb = rIndex;
        //        ub = CHUNKLIST_CAPACITY;
        //        tempResult = binarySearch(list, lb, ub, target);
        //        if (tempResult != -1)
        //            return tempResult;
        //        else
        //        {
        //            lb = 0;
        //            ub = wIndex - 1;
        //            tempResult = binarySearch(list, lb, ub, target);
        //            return tempResult;
        //        }
        //    }
        //    else
        //    {
        //        lb = rIndex;
        //        ub = wIndex - 1;

        //        tempResult = binarySearch(list, lb, ub, target);
        //        return tempResult;
        //    }
        //}

        //private int binarySearch(List<Chunk> list, int lb, int ub, int target)
        //{
        //    int mid;
        //    for (; lb <= ub; )
        //    {
        //        mid = (lb + ub) / 2;

        //        if (list[mid].seq == target)
        //            return mid;
        //        else if (target > list[mid].seq)
        //            lb = mid + 1;
        //        else
        //            ub = mid - 1;
        //    }
        //    return -1;
        //}

        //calculate tree_index of chunk
        public int calcTreeIndex(int seqNum)
        {
            int result = (seqNum - 1) % max_tree;
            return result;
        }

        public int getReplyChunkPort()
        {
            return replyChunkPort;
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

            for (int j = 0; j < CThreadList.Count; j++)
            {
                treeDPListener[j].Stop();
                DThreadList[j].Abort();

                treeCPListener[j].Stop();
                CThreadList[j].Abort();
            }



        }
        // Write for tracker registration.
        private bool registerToTracker(int tree, string peerId, IPAddress childAddress, int listenPort, string layer)
        {
            TcpClient connectTracker;
            NetworkStream connectTrackerStream;
            try
            {
                connectTracker = new TcpClient(mainFm.tbTracker.Text, sConfig.TrackerPort);
                //connectTracker = new TcpClient(clientFm.tbServerIp.Text, cConfig.TrackerPort);
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
                string sendstr = childAddress.ToString() + "@" + listenPort +"@"+ channelid  + "@" + tree + "@" + peerId  + "@" + layer + "@0";
                Byte[] sendbyte = StrToByteArray(sendstr);
                //connectTrackerStream.Write(sendbyte, 0, sendbyte.Length);

                byte[] MsgLength = BitConverter.GetBytes(sendstr.Length);
                connectTrackerStream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                connectTrackerStream.Write(sendbyte, 0, sendbyte.Length);

                connectTracker.Close();
                connectTrackerStream.Close();

            }
            catch
            {
                //((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "register ex:\n" + ex });
                mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Register fail\n" });
                return false;
            }

            return true;
        }



    }//end class
}//end namespace
