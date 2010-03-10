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
        static int REPLY_CHUNK_TIMEOUT = 5000;
        static int REC_SEND_DIFF = 5;

        int max_client;
        int max_tree;
        int tempResult;

        bool localterminate = false;

        IPAddress localAddr;
        ChunkHandler ch;
        ClientConfig cConfig;

        private ClientForm clientFm;
        private ClientHandler clientMain;
        private delegate void UpdateTextCallback(string message);

        List<List<cPort>> treeCPortList;
        List<List<dPort>> treeDPortList;
        List<List<Chunk>> treeChunkList;
        List<List<int>> treeSeqList;
        List<Thread> CThreadList = new List<Thread>();
        List<Thread> DThreadList = new List<Thread>();
        // List<List<Thread>> treeCThreadList;
        //List<List<Thread>> treeDThreadList;
        //List<List<TcpListener>> treeCPListener;
        //List<List<TcpListener>> treeDPListener;
        
        Thread replyChunkThread;
        TcpListener replyChunkListener=null;
        int replyChunkPort = 0;

        int[] treeCLWriteIndex;
        int[] treeCLReadIndex;
        int[] treeCLCurrentSeq;
        int[] treeCLState;
        int[] DOfflineState;
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

            treeCPortList = new List<List<cPort>>(maxTree);
            treeDPortList = new List<List<dPort>>(maxTree);
            treeChunkList = new List<List<Chunk>>(maxTree);
            treeSeqList = new List<List<int>>(maxTree);
            //treeCThreadList = new List<List<Thread>>(maxTree);
            // treeDThreadList = new List<List<Thread>>(maxTree);
            //treeCPListener = new List<List<TcpListener>>(maxTree);
            //treeDPListener = new List<List<TcpListener>>(maxTree);

            treeCLWriteIndex = new int[maxTree];
            treeCLReadIndex = new int[maxTree];
            treeCLCurrentSeq = new int[maxTree];
            treeCLState = new int[maxTree];
            DOfflineState = new int[maxTree * this.max_client];

            treeCPListener = new TcpListener[maxTree * this.max_client];
            treeDPListener = new TcpListener[maxTree * this.max_client];

            createTreeChunkList(maxTree, CHUNKLIST_CAPACITY);
            createTreePortList(maxTree, this.max_client);
            // createTreeThreadList(maxTree, maxClient);
        }


        private void createTreePortList(int maxTree, int maxClient)
        {
            for (int i = 0; i < maxTree; i++)
            {
                List<cPort> CPortList = new List<cPort>(maxClient);
                List<dPort> DPortList = new List<dPort>(maxClient);
                treeCPortList.Add(CPortList);
                treeDPortList.Add(DPortList);

                // List<TcpListener> CPListener = new List<TcpListener>(maxClient);
                // List<TcpListener> DPListener = new List<TcpListener>(maxClient);
                //treeCPListener.Add(CPListener);
                //treeDPListener.Add(DPListener);
            }
        }

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
           

            for (int i = 0; i < maxTree; i++)
            {
                List<Chunk> chunkList = new List<Chunk>(chunkListCapacity);
                treeChunkList.Add(chunkList);

                List<int> seqList = new List<int>(chunkListCapacity);
                for (int j = 0; j < chunkListCapacity; j++)
                {
                    //seqList = new List<int>(CHUNKLIST_CAPACITY);
                    seqList.Add(0);

                }
                treeSeqList.Add(seqList);
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

            if (treeChunkList[tree_index].Count <=(CHUNKLIST_CAPACITY-1))
                treeChunkList[tree_index].Add(sChunk);
            else
                treeChunkList[tree_index][write_index] = sChunk;

            treeSeqList[tree_index][write_index] = sChunk.seq;

            if (write_index == (CHUNKLIST_CAPACITY-1))
                treeCLWriteIndex[tree_index] = 0;
            else
                treeCLWriteIndex[tree_index] += 1;

            treeCLCurrentSeq[tree_index] = sChunk.seq;

        }

        public TcpClient getTreeCListClient(int tree_num, int cList_index)
        {
            return treeCPortList[tree_num - 1][cList_index].clientC;
        }

        public TcpClient getTreeDListClient(int tree_num, int dList_index)
        {
            return treeDPortList[tree_num - 1][dList_index].clientD;
        }

        public int getTreeCListPort(int tree_num, int cList_index)
        {
            return treeCPortList[tree_num - 1][cList_index].PortC;
        }

        public int getTreeDListPort(int tree_num, int dList_index)
        {
            return treeDPortList[tree_num - 1][dList_index].PortD;
        }

        private void delClientFromTreeDList(int dList_index, int tree_index)
        {
            if (treeDPortList[tree_index][dList_index].peerId != -1)
            {
                int port_num = treeDPortList[tree_index][dList_index].PortD;

                treeDPortList[tree_index][dList_index].clientD.Close();

                dPort dport = new dPort();
                dport.clientD = null;
                dport.PortD = port_num;
                dport.peerId = -1;

                treeDPortList[tree_index][dList_index] = dport;
                //mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T:" + tree_index + " D:" + " unRge \n" });
            }
        }

        private void delClientFromTreeCList(int cList_index, int tree_index)
        {
            if (treeCPortList[tree_index][cList_index].peerId != -1)
            {
                int port_num = treeCPortList[tree_index][cList_index].PortC;

                treeCPortList[tree_index][cList_index].clientC.Close();

                cPort cport = new cPort();
                cport.clientC = null;
                cport.PortC = port_num;
                cport.peerId = -1;

                treeCPortList[tree_index][cList_index] = cport;
                //mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T:" + tree_index + " C:" + " unReg \n" });
            }
        }

        public void startTreePort()
        {
            localterminate = false; //indicate whether it is local terminate

            for (int i = 0; i < max_tree; i++)
            {
                for (int j = 0; j < max_client; j++)
                {
                    Thread CPortThread = new Thread(delegate() { TreePortHandle_Cport(j, i); });
                    CPortThread.IsBackground = true;
                    CPortThread.Name = " Cport_handle_" + i + "_" + j;
                    CPortThread.Start();
                    Thread.Sleep(20);
                    CThreadList.Add(CPortThread);

                    Thread DPortThread = new Thread(delegate() { TreePortHandle_Dport(j, i); });
                    DPortThread.IsBackground = true;
                    DPortThread.Name = " Dport_handle_" + i + "_" + j;
                    DPortThread.Start();
                    Thread.Sleep(20);
                    DThreadList.Add(DPortThread);

                }

                // treeCThreadList.Add(CThreadList);
                // treeDThreadList.Add(DThreadList);
            }

            //start pull chunk thread
            replyChunkPort=TcpApps.RanPort(PULL_CHUNK_PORT_BASE,PULL_CHUNK_PORT_UP);
            
            replyChunkThread= new Thread(new ThreadStart(replyChunk));
            replyChunkThread.IsBackground = true;
            replyChunkThread.Name = "reply_Chunk";
            replyChunkThread.Start();
        }

        private void TreePortHandle_Dport(int DThreadList_index, int tree_index)
        {
            byte[] sendMessage = new byte[cConfig.ChunkSize];
            byte[] waitingMessage = new byte[cConfig.ChunkSize];

            int tempSeq = tree_index + 1;// 0;
            int tempRead_index = 0;
            //int resultIndex = 0;
            bool firstRun = true;

            int ran_port = TcpApps.RanPort(cConfig.Dport, cConfig.Dataportup);

            NetworkStream stream = null;
            TcpClient DPortClient = null;
            //TcpListener DportListener = new TcpListener(localAddr,ran_port);
            //treeDPListener[tree_index].Add(new TcpListener(localAddr, ran_port));
            treeDPListener[(tree_index * max_client) + DThreadList_index] = new TcpListener(localAddr, ran_port);

            bool portStarted = false;
            while (!portStarted)
            {
                try
                {
                    //DportListener.Start(1);
                    //treeDPListener[tree_index][DThreadList_index].Start(1);
                    treeDPListener[(tree_index * max_client) + DThreadList_index].Start(1);
                    portStarted = true;
                }
                catch (Exception ex)
                {
                    ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "]" + ran_port +"\n"+ex.ToString() });
                    //clientFm.rtbupload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "]" + ex.ToString() });
                    Thread.Sleep(20);
                    ran_port = TcpApps.RanPort(cConfig.Dport, cConfig.Dataportup);
                    treeDPListener[(tree_index * max_client) + DThreadList_index] = new TcpListener(localAddr, ran_port);
                }
                Thread.Sleep(10);
            }

            dPort dp = new dPort();
            dp.PortD = ran_port;
            dp.clientD = null;
            dp.peerId = -1;
            treeDPortList[tree_index].Add(dp);

            while (true)
            {


                try
                {
                    //DPortClient = DportListener.AcceptTcpClient();
                    //DPortClient = treeDPListener[tree_index][DThreadList_index].AcceptTcpClient();
                    DPortClient = treeDPListener[(tree_index * max_client) + DThreadList_index].AcceptTcpClient();

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

                    string peerId = messages[0];
                    int listenPort = Int32.Parse(messages[1]);
                    int maxPeer= Int32.Parse(messages[2]);

                    dPort dpt = new dPort();
                    dpt.clientD = DPortClient;
                    dpt.PortD = ran_port;
                    dpt.peerId = Int32.Parse(peerId);
                    treeDPortList[tree_index][DThreadList_index] = dpt;

                    
                    //register the child
                    while (!registerToTracker(tree_index, peerId, childAddress, listenPort, "0", maxPeer))
                    {
                        //registerToTracker(tree_index, peerId, childAddress, listenPort, "0", maxPeer);
                        Random random = new Random();
                        Thread.Sleep(random.Next(50, 100));
                    }

                    while (true)
                    {
                        //if control port dead which cause this case happen
                        if (treeDPortList[tree_index][DThreadList_index].clientD == null)
                        {
                            ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] D:" + ran_port + " exit~\n" });
                            stream.Close();
                            DPortClient.Close();
                            firstRun = true;
                            break;
                        }

                        if (treeCLState[tree_index] == 2)
                        {
                            //  DateTime dt= DateTime.Now;
                            // dt.ToBinary();

                            firstRun = true;
                            waitingMessage = System.Text.Encoding.ASCII.GetBytes(clientMain.waitMsg);//System.Text.Encoding.ASCII.GetBytes("Wait" + "@" + clientMain.getSelfID(tree_index));
                            stream.Write(waitingMessage, 0, waitingMessage.Length);
                            Thread.Sleep(20);

                            continue;


                        }
                        else if (treeCLState[tree_index] == 0)
                        {
                          //  DateTime dt= DateTime.Now;
                           // dt.ToBinary();

                            firstRun = true;
                            waitingMessage = System.Text.Encoding.ASCII.GetBytes("Wait"+"@"+clientMain.getSelfID(tree_index)+"@");
                            stream.Write(waitingMessage, 0, waitingMessage.Length);
                            Thread.Sleep(20);

                            continue;


                        }

                        if (firstRun == true && treeChunkList[tree_index].Count > 1)
                        {
                          
                          //  tempSeq = treeCLCurrentSeq[tree_index];
                            tempRead_index = treeCLWriteIndex[tree_index] - 1;
                            tempSeq =treeChunkList[tree_index][tempRead_index].seq;
                            firstRun = false;
                        }

                        //by yam: not seach method
                        if (treeChunkList[tree_index].Count > 1 && tempSeq <= treeCLCurrentSeq[tree_index])
                        {

                            //sendMessage = ch.chunkToByte(treeChunkList[tree_index][tempRead_index], cConfig.ChunkSize);
                            //stream.Write(sendMessage, 0, sendMessage.Length);
                            int readWrite_different = 0;
                            if (treeCLWriteIndex[tree_index] > tempRead_index)
                                readWrite_different = treeCLWriteIndex[tree_index] - tempRead_index;
                            else
                                readWrite_different = (treeCLWriteIndex[tree_index] + CHUNKLIST_CAPACITY) - tempRead_index;

                            if (!(readWrite_different > REC_SEND_DIFF))
                            {
                                sendMessage = ch.chunkToByte(treeChunkList[tree_index][tempRead_index], cConfig.ChunkSize);
                                stream.Write(sendMessage, 0, sendMessage.Length);
                            }
                            else
                                ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "Skip chunk: " + tempRead_index + "\n" });

                            if (tempSeq == 2147483647)
                                tempSeq = tree_index + 1;
                            else
                                tempSeq += max_tree;

                            if (tempRead_index == (CHUNKLIST_CAPACITY-1))
                                tempRead_index = 0;
                            else
                                tempRead_index += 1;
                        }

                       // stream.Flush();

                        Thread.Sleep(10);
                    }
                }
                catch(Exception ex)
                {

                    if (ex.ToString().Contains("disposed object"))
                    {
                        ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] D:" + ran_port + " exit*\n" });
                        
                        if (stream != null)
                            stream.Close();
                        if (DPortClient != null)
                            DPortClient.Close();

                        firstRun = true;
                        continue;
                    }

                    ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] D:" + ran_port + " exit\n" });

                    if (!localterminate && treeDPortList[tree_index][DThreadList_index].clientD != null)
                    {
                        DOfflineState[(tree_index * max_client) + DThreadList_index] = 1;

                        //treeCPListener[(tree_index * max_client) + DThreadList_index].Stop();
                        //CThreadList[(tree_index * max_client) + DThreadList_index].Abort();

                        //Thread CPortThread = new Thread(delegate() { TreePortHandle_Cport(DThreadList_index, tree_index); });
                        //CPortThread.IsBackground = true;
                        //CPortThread.Name = " Cport_handle_" + tree_index + "_" + DThreadList_index;
                        //CPortThread.Start();
                        //Thread.Sleep(20);
                        //CThreadList[(tree_index * max_client) + DThreadList_index] = CPortThread;


                    }
                    if (stream != null)
                        stream.Close();

                    if (DPortClient != null)
                        DPortClient.Close();

                    firstRun = true;

                    if (localterminate)
                        break;
                    
                }
                Thread.Sleep(10);

            }
        }

        private void TreePortHandle_Cport(int CThreadList_index, int tree_index)
        {
            byte[] responseMessage = new Byte[4];
            int ran_port = TcpApps.RanPort(cConfig.CportBase, cConfig.Conportup);

            TcpClient CPortClient = null;
            NetworkStream stream = null;
            treeCPListener[(tree_index * max_client) + CThreadList_index] = new TcpListener(localAddr, ran_port);

            bool portStarted = false;
            while (!portStarted)
            {
                try
                {
                    treeCPListener[(tree_index * max_client) + CThreadList_index].Start(1);
                    portStarted = true;
                }
                catch (Exception ex)
                {
                    ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "]" + ran_port + "\n" + ex.ToString() });
                    //clientFm.rtbupload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "]" + ex.ToString() });
                    Thread.Sleep(20);
                    ran_port = TcpApps.RanPort(cConfig.CportBase, cConfig.Conportup);
                    treeCPListener[(tree_index * max_client) + CThreadList_index] = new TcpListener(localAddr, ran_port); //ran a new port if crash
                }
                Thread.Sleep(10);
            }

            cPort cp = new cPort();
            cp.PortC = ran_port;
            cp.clientC = null;
            treeCPortList[tree_index].Add(cp);


            while (true)
            {


                try
                {

                    CPortClient = treeCPListener[(tree_index * max_client) + CThreadList_index].AcceptTcpClient();

                  //  CPortClient.NoDelay = true;

                    stream = CPortClient.GetStream();

                    //get child peer info
                    byte[] responsePeerMsg = new byte[4];
                    stream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);
                    byte[] responsePeerMsg2 = new byte[MsgSize];
                    stream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                    string peerId = ByteArrayToString(responsePeerMsg2);

                    cPort cpt = new cPort();
                    cpt.clientC = CPortClient;
                    cpt.PortC = ran_port;
                    cpt.peerId = Int32.Parse(peerId);
                    treeCPortList[tree_index][CThreadList_index] = cpt;

                    while (true)
                    {
                        stream.ReadTimeout = 10000;
                        int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
                        string responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);


                        if (responseString == "Exit" || DOfflineState[(tree_index * max_client) + CThreadList_index] == 1)
                        {
                           // if (DOfflineState[(tree_index * max_client) + CThreadList_index] == 1)
                             //   Thread.Sleep(800);
                            ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] C:" + ran_port + " exit~ [" + responseString + ":" + DOfflineState[(tree_index * max_client) + CThreadList_index].ToString() + "]\n" });
                            unregister(tree_index, treeCPortList[tree_index][CThreadList_index].peerId);
                            ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] Peer:" + treeCPortList[tree_index][CThreadList_index].peerId + " unRge~\n" });
                            delClientFromTreeDList(CThreadList_index, tree_index);
                            delClientFromTreeCList(CThreadList_index, tree_index);
                            DOfflineState[(tree_index * max_client) + CThreadList_index] = 0;
                            
                            stream.Close();
                            CPortClient.Close();
                            break;
                        }

                        if (responseString == "Wait")
                        {
                            Thread.Sleep(20);
                            continue;
                        }

                        Thread.Sleep(20);
                    }
                }
                catch(Exception ex)
                {
                    ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] C:" + ran_port + "exit\n" });

                    if (!localterminate)
                    {
                        unregister(tree_index, treeCPortList[tree_index][CThreadList_index].peerId);
                        ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T[" + tree_index + "] Peer:" + treeCPortList[tree_index][CThreadList_index].peerId + " unRge \n" });
                        delClientFromTreeDList(CThreadList_index, tree_index);
                        delClientFromTreeCList(CThreadList_index, tree_index);
                        DOfflineState[(tree_index * max_client) + CThreadList_index] = 0;
                    }

                    if (stream != null)
                        stream.Close();

                    if (CPortClient != null)
                        CPortClient.Close();
                    //Thread.Sleep(10);
                    if (localterminate)
                        break;

                }
                Thread.Sleep(10);
            }
        }

        private bool unregister(int tree, int peerId)
        {
            while(true)
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
                           
                        if(connectTrackerStream!=null)
                           connectTrackerStream.Close();
                        if(connectTracker!=null)
                           connectTracker.Close();
                        Thread.Sleep(20);
                       // return false;
                    }
                }

            }
            return true;
        }

        // Write for tracker registration.
        private bool registerToTracker(int tree, string peerId,IPAddress childAddress, int listenPort, string layer, int maxPeer)
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
            catch(Exception ex)
            {
                ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "register ex:\n"+ex });
                return false;
            }

            return true;
        }

        private void replyChunk()
        {
            TcpClient client = null;
            NetworkStream stream = null;
            int target_tree, result_index, reqSeq;
            Chunk ck = new Chunk();
            ck.seq = 0;

            try
            {
                // mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { PeerListenPort.ToString() +"\n"});
                replyChunkListener = new TcpListener(localAddr, replyChunkPort);
                replyChunkListener.Start();
            }
            catch (Exception ex)
            {
                ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] {"replyPort: "+ex.ToString()});
            }

            ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "IP:" + localAddr.ToString() + "\nPort[" + replyChunkPort.ToString() + "]:Listening~\n" });
            
            while (true)
            {
                try
                {
                    client = replyChunkListener.AcceptTcpClient();
                    stream = client.GetStream();
                    stream.ReadTimeout = REPLY_CHUNK_TIMEOUT;


                    byte[] sendMessage = new byte[cConfig.ChunkSize];
                    byte[] responseMessage2 = new byte[4];
                    stream.Read(responseMessage2, 0, responseMessage2.Length);
                    reqSeq = BitConverter.ToInt16(responseMessage2, 0);

                    target_tree = clientMain.calcTreeIndex(reqSeq);

                
                    result_index = search(target_tree, 0, cConfig.ChunkCapacity - 1, reqSeq);

                    if (result_index != -1)
                        sendMessage = ch.chunkToByte(treeChunkList[target_tree][result_index], cConfig.ChunkSize);
                    else
                       sendMessage = ch.chunkToByte(ck, cConfig.ChunkSize);

                    stream.Write(sendMessage, 0, sendMessage.Length);

                    ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "uploadChunk\n" });
                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show("replyThread:"+ex.ToString());

                    if (!localterminate)  
                        ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "Peer pull chunk fail.\n" });
                    else
                        ((LoggerFrm)clientFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "Listen port close~\n" });

                    if (stream != null)
                        stream.Close();
                    if (client != null)
                        client.Close();
                }

                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();

                Thread.Sleep(20);
            }


        }

        public int getReplyChunkPort()
        {
            return replyChunkPort;
        }

        private int searchChunk(int list_index, int rIndex, int wIndex, int target)
        {
            if (wIndex < rIndex)
            {
                tempResult = search(list_index, rIndex, cConfig.ChunkCapacity-1, target);
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
                if(treeSeqList[list_index][r_index]==target) 
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

            for (int j = 0; j < CThreadList.Count; j++)
            {
                treeDPListener[j].Stop();
                DThreadList[j].Abort();

                treeCPListener[j].Stop();
                CThreadList[j].Abort();
            }

        }


    }//end class
}
