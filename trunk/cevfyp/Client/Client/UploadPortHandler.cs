﻿using System;
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
        static int TrackerSLPort = 1500;
        static int CHUNKLIST_CAPACITY = 200;
        int max_client;
        int max_tree;

        bool localterminate = false;

        IPAddress localAddr;
        ChunkHandler ch;
        ClientConfig cConfig;

        private ClientForm clientFm;
        private delegate void UpdateTextCallback(string message);

        List<List<cPort>> treeCPortList;
        List<List<dPort>> treeDPortList;
        List<List<Chunk>> treeChunkList;
        List<Thread> CThreadList = new List<Thread>();
        List<Thread> DThreadList = new List<Thread>();
        // List<List<Thread>> treeCThreadList;
        //List<List<Thread>> treeDThreadList;
        //List<List<TcpListener>> treeCPListener;
        //List<List<TcpListener>> treeDPListener;

        int[] treeCLWriteIndex;
        int[] treeCLReadIndex;
        int[] treeCLCurrentSeq;
        int[] treeCLState;
        int[] DOfflineState;
        TcpListener[] treeCPListener;
        TcpListener[] treeDPListener;

        public UploadPortHandler(ClientConfig cConfig, string serverip, ClientForm clientFm, int maxTree)
        {
            this.clientFm = clientFm;
            this.max_client = cConfig.MaxPeer;
            this.max_tree = maxTree;

            localAddr = IPAddress.Parse(serverip);

            ch = new ChunkHandler();
            this.cConfig = cConfig;

            treeCPortList = new List<List<cPort>>(maxTree);
            treeDPortList = new List<List<dPort>>(maxTree);
            treeChunkList = new List<List<Chunk>>(maxTree);
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

            if (treeChunkList[tree_index].Count <= CHUNKLIST_CAPACITY)
                treeChunkList[tree_index].Add(sChunk);
            else
                treeChunkList[tree_index][write_index] = sChunk;

            if (write_index == CHUNKLIST_CAPACITY)
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
        }

        private void TreePortHandle_Dport(int DThreadList_index, int tree_index)
        {
            byte[] sendMessage = new byte[cConfig.ChunkSize];
            byte[] waitingMessage = new byte[cConfig.ChunkSize];

            int tempSeq = tree_index + 1;// 0;
            int tempRead_index = 0;
            //int resultIndex = 0;
            bool firstRun = true;

            int ran_port = TcpApps.RanPort(1200, 1400);

            NetworkStream stream = null;
            TcpClient DPortClient = null;
            //TcpListener DportListener = new TcpListener(localAddr,ran_port);
            //treeDPListener[tree_index].Add(new TcpListener(localAddr, ran_port));
            treeDPListener[(tree_index * max_client) + DThreadList_index] = new TcpListener(localAddr, ran_port);

            dPort dp = new dPort();
            dp.PortD = ran_port;
            dp.clientD = null;
            dp.peerId = -1;
            treeDPortList[tree_index].Add(dp);

            //DportListener.Start(1);
            //treeDPListener[tree_index][DThreadList_index].Start(1);
            treeDPListener[(tree_index * max_client) + DThreadList_index].Start(1);

            while (true)
            {


                try
                {
                    //DPortClient = DportListener.AcceptTcpClient();
                    //DPortClient = treeDPListener[tree_index][DThreadList_index].AcceptTcpClient();
                    DPortClient = treeDPListener[(tree_index * max_client) + DThreadList_index].AcceptTcpClient();
                    stream = DPortClient.GetStream();

                    //get the peer id
                    byte[] responsePeerMsg = new byte[4];
                    stream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);
                    byte[] responsePeerMsg2 = new byte[MsgSize];
                    stream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                    string peerId = ByteArrayToString(responsePeerMsg2);

                    dPort dpt = new dPort();
                    dpt.clientD = DPortClient;
                    dpt.PortD = ran_port;
                    dpt.peerId = Int32.Parse(peerId);
                    treeDPortList[tree_index][DThreadList_index] = dpt;


                    while (true)
                    {
                        //if control port dead which cause this case happen
                        if (treeDPortList[tree_index][DThreadList_index].clientD == null)
                        {
                            clientFm.rtbupload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T:" + tree_index + " D:" + ran_port + " exit~\n" });
                            stream.Close();
                            DPortClient.Close();
                            firstRun = true;
                            break;
                        }

                        if (treeCLState[tree_index] == 0)
                        {
                            firstRun = true;
                            waitingMessage = System.Text.Encoding.ASCII.GetBytes("Wait");
                            stream.Write(waitingMessage, 0, waitingMessage.Length);
                            Thread.Sleep(20);
                            continue;
                        }

                        if (firstRun == true && treeChunkList[tree_index].Count > 1)
                        {
                            tempSeq = treeCLCurrentSeq[tree_index];
                            tempRead_index = treeCLWriteIndex[tree_index] - 1;
                            firstRun = false;
                        }

                        //by yam: using search method
                        /*if (treeChunkList[tree_index].Count > 10 && tempSeq <= treeCLCurrentSeq[tree_index])
                        {

                            resultIndex = search(treeChunkList[tree_index], treeCLReadIndex[tree_index], treeCLWriteIndex[tree_index], tempSeq);

                            if (resultIndex != -1)
                            {
                                sendMessage = ch.chunkToByte(treeChunkList[tree_index][resultIndex], sConfig.ChunkSize);
                                stream.Write(sendMessage, 0, sendMessage.Length);
                                treeCLReadIndex[tree_index] = resultIndex;
                            }

                            if (tempSeq == 2147483647)
                                tempSeq = tree_index+1;
                            else
                                tempSeq += max_tree;
                        }
                        */

                        //by yam: not seach method
                        if (treeChunkList[tree_index].Count > 1 && tempSeq <= treeCLCurrentSeq[tree_index])
                        {
                            sendMessage = ch.chunkToByte(treeChunkList[tree_index][tempRead_index], cConfig.ChunkSize);
                            stream.Write(sendMessage, 0, sendMessage.Length);

                            if (tempSeq == 2147483647)
                                tempSeq = tree_index + 1;
                            else
                                tempSeq += max_tree;

                            if (tempRead_index == CHUNKLIST_CAPACITY)
                                tempRead_index = 0;
                            else
                                tempRead_index += 1;
                        }

                        Thread.Sleep(10);
                    }
                }
                catch
                {
                    clientFm.rtbupload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T:" + tree_index + " D:" + ran_port + " exit\n" });

                    if (!localterminate)
                        DOfflineState[(tree_index * max_client) + DThreadList_index] = 1;

                    if (stream != null)
                        stream.Close();

                    if (DPortClient != null)
                        DPortClient.Close();

                    firstRun = true;
                }


            }
        }

        private void TreePortHandle_Cport(int CThreadList_index, int tree_index)
        {
            byte[] responseMessage = new Byte[4];
            int ran_port = TcpApps.RanPort(1401, 1600);

            TcpClient CPortClient = null;
            NetworkStream stream = null;
            treeCPListener[(tree_index * max_client) + CThreadList_index] = new TcpListener(localAddr, ran_port);

            cPort cp = new cPort();
            cp.PortC = ran_port;
            cp.clientC = null;
            treeCPortList[tree_index].Add(cp);

            treeCPListener[(tree_index * max_client) + CThreadList_index].Start(1);

            while (true)
            {


                try
                {

                    CPortClient = treeCPListener[(tree_index * max_client) + CThreadList_index].AcceptTcpClient();
                    stream = CPortClient.GetStream();

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

                        int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
                        string responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);


                        if (responseString == "Exit" || DOfflineState[(tree_index * max_client) + CThreadList_index] == 1)
                        {
                            clientFm.rtbupload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T:" + tree_index + " C:" + ran_port + " exit~\n" });
                            unregister(tree_index, treeCPortList[tree_index][CThreadList_index].peerId);
                            clientFm.rtbupload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T:" + tree_index + " unRge \n" });
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
                catch
                {
                    clientFm.rtbupload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T:" + tree_index + " C:" + ran_port + "exit\n" });

                    if (!localterminate)
                    {
                        unregister(tree_index, treeCPortList[tree_index][CThreadList_index].peerId);
                        clientFm.rtbupload.BeginInvoke(new UpdateTextCallback(clientFm.UpdateRtbUpload), new object[] { "T:" + tree_index + " unRge \n" });
                        delClientFromTreeDList(CThreadList_index, tree_index);
                        delClientFromTreeCList(CThreadList_index, tree_index);
                        DOfflineState[(tree_index * max_client) + CThreadList_index] = 0;
                    }

                    if (stream != null)
                        stream.Close();

                    if (CPortClient != null)
                        CPortClient.Close();
                }

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
                    connectTracker = new TcpClient(clientFm.tbServerIp.Text, TrackerSLPort);
                    connectTrackerStream = connectTracker.GetStream();

                    //define client message type
                    Byte[] clienttype = StrToByteArray("<unRegists>");
                    connectTrackerStream.Write(clienttype, 0, clienttype.Length);

                    string sendstr = tree + "@" + peerId;
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
                    connectTrackerStream.Close();
                    connectTracker.Close();
                }
            }

            return true;
        }


        private int search(List<Chunk> list, int rIndex, int wIndex, int target)
        {
            int lb, ub, tempResult;
            if (wIndex < rIndex)
            {
                lb = rIndex;
                ub = CHUNKLIST_CAPACITY;
                tempResult = binarySearch(list, lb, ub, target);
                if (tempResult != -1)
                    return tempResult;
                else
                {
                    lb = 0;
                    ub = wIndex - 1;
                    tempResult = binarySearch(list, lb, ub, target);
                    return tempResult;
                }
            }
            else
            {
                lb = rIndex;
                ub = wIndex - 1;

                tempResult = binarySearch(list, lb, ub, target);
                return tempResult;
            }
        }

        private int binarySearch(List<Chunk> list, int lb, int ub, int target)
        {
            int mid;
            for (; lb <= ub; )
            {
                mid = (lb + ub) / 2;

                if (list[mid].seq == target)
                    return mid;
                else if (target > list[mid].seq)
                    lb = mid + 1;
                else
                    ub = mid - 1;
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
