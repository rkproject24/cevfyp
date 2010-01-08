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
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct dPort
    {
        public TcpClient clientD;
        public int PortD;

    }

    class UploadPortHandler
    {
        IPAddress localAddr;

        int max_client;
        int max_tree;

        static int CHUNKLIST_CAPACITY = 200;
        // static int NUM_D1PORT_BASE = 1200;
        //static int NUM_D2PORT_BASE = 1250;
        // static int NUM_CPORT_BASE = 1301;

        /* List<cPort> CPortList;
         List<dPort> D1PortList;  //odd
         List<dPort> D2PortList;  //even
  
         //yam:10-10-09
         List<Thread> CThreadList;
         List<Thread> D1ThreadList; 
         List<Thread> D2ThreadList;
        
         List<Chunk> oddList;
         List<Chunk> evenList;
      
         int currentOddNo = 0;
         int currentEvenNo = 0;
         int oddList_wIndex = 0;
         int oddList_rIndex = 0;
         int evenList_wIndex = 0;
         int evenList_rIndex = 0;
       */

        ChunkHandler ch;
        ClientConfig cConfig;

        //TcpClient CPortClient = null;

        //yam:01-01-10
        List<List<cPort>> treeCPortList;
        List<List<dPort>> treeDPortList;
        List<List<Chunk>> treeChunkList;
        // List<List<Thread>> treeCThreadList;
        //List<List<Thread>> treeDThreadList;
        //List<int> treeCLWriteIndex; //Each chunk list current write index
        //List<int> treeCLReadIndex;
        int[] treeCLWriteIndex;
        int[] treeCLReadIndex;
        int[] treeCLCurrentSeq;// = new int[TREE_NO];

        List<Thread> CThreadList = new List<Thread>();
        List<Thread> DThreadList = new List<Thread>();

        int current_num = 0;

        private ClientForm clientFm;
        private delegate void UpdateTextCallback(string message);

        public UploadPortHandler(ClientConfig cConfig, string serverip, ClientForm clientFm, int maxTree)
        {
            this.clientFm = clientFm;
            this.max_client = cConfig.MaxPeer;
            this.max_tree = maxTree;

            localAddr = IPAddress.Parse(serverip);

            ch = new ChunkHandler();
            //sConfig = new ServerConfig();
            //cConfig = new ClientConfig();
            this.cConfig = cConfig;

            //yam:01-01-10
            treeCPortList = new List<List<cPort>>(maxTree);
            treeDPortList = new List<List<dPort>>(maxTree);
            treeChunkList = new List<List<Chunk>>(maxTree);
            //treeCThreadList = new List<List<Thread>>(maxTree);
            // treeDThreadList = new List<List<Thread>>(maxTree);
            //treeCLWriteIndex = new List<int>(maxTree);
            //treeCLReadIndex = new List<int>(maxTree);
            treeCLWriteIndex = new int[maxTree];
            treeCLReadIndex = new int[maxTree];
            treeCLCurrentSeq = new int[maxTree];


            createTreeChunkList(maxTree, CHUNKLIST_CAPACITY);
            createTreePortList(maxTree, max_client);
        }

                //yam:01-01-10
        public void createTreePortList(int maxTree,int maxClient)
        {
            for (int i = 0; i < maxTree; i++)
            {
                List<cPort> CPortList = new List<cPort>(maxClient);
                List<dPort> DPortList = new List<dPort>(maxClient);
                treeCPortList.Add(CPortList);
                treeDPortList.Add(DPortList);
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

        public void createTreeChunkList(int maxTree,int chunkListCapacity)
        {
            for (int i = 0; i < maxTree; i++)
            {
                List<Chunk> chunkList = new List<Chunk>(chunkListCapacity);
                treeChunkList.Add(chunkList);

                //treeCLWriteIndex.Add(0);
               // treeCLReadIndex.Add(0);
            }
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

            
            current_num = sChunk.seq;
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

        public void delClientFromTreeDList(int dList_index,int tree_index,int port_num)
        {
            dPort dport = new dPort();
            dport.clientD = null;
            dport.PortD = port_num;

            treeDPortList[tree_index][dList_index] = dport;
        }

       // List<Thread> CThreadList = new List<Thread>();
        //List<Thread> DThreadList = new List<Thread>();

        public void startTreePort()
        {
            for (int i = 0; i < max_tree; i++)
            {
               // List<Thread> CThreadList = new List<Thread>(max_client);
                //List<Thread> DThreadList = new List<Thread>(max_client);

                for (int j = 0; j < max_client; j++)
                {
                    Thread CPortThread = new Thread(delegate() { TreePortHandle_Cport(j,i); });
                    CPortThread.IsBackground = true;
                    CPortThread.Name = " Cport_handle_" +i + j;
                    CPortThread.Start();
                    Thread.Sleep(100);
                    CThreadList.Add(CPortThread);
                  
                    Thread DPortThread = new Thread(delegate() { TreePortHandle_Dport(j,i); });
                    DPortThread.IsBackground = true;
                    DPortThread.Name = " Dport_handle_" +i+ j;
                    DPortThread.Start();
                    Thread.Sleep(100);
                    DThreadList.Add(DPortThread);

                }

               // treeCThreadList.Add(CThreadList);
               // treeDThreadList.Add(DThreadList);

            }
        }

        private void TreePortHandle_Dport(int DThreadList_index, int tree_index)
        {
            NetworkStream stream;
            byte[] sendMessage = new byte[cConfig.ChunkSize];
            byte[] responseMessage = new byte[20];

            int tempSeq = tree_index + 1;
            int tempRead_index = 0;
            bool firstRun = true;

            int resultIndex = 0;
            TcpClient DPortClient;

            int ran_port = TcpApps.RanPort(1200, 1400);
            TcpListener DportListener = new TcpListener(localAddr, ran_port);
            DportListener.Start(1);

            dPort dp = new dPort();
            dp.PortD = ran_port;
            dp.clientD = null;
            treeDPortList[tree_index].Add(dp);

            while (true)
            {

                DPortClient = DportListener.AcceptTcpClient();

                dPort dpt = new dPort();
                dpt.clientD = DPortClient;
                dpt.PortD = ran_port;
                treeDPortList[tree_index][DThreadList_index] = dpt;
                stream = DPortClient.GetStream();

                try
                {
                    while (true)
                    {
                        if (firstRun == true && treeChunkList[tree_index].Count > 1)
                        {
                            tempSeq = treeCLCurrentSeq[tree_index];
                            firstRun = false;
                        }

                        
                        //by yam: using search method
                        /*if (treeChunkList[tree_index].Count > 1 && tempSeq <= treeCLCurrentSeq[tree_index])
                        {

                            resultIndex = search(treeChunkList[tree_index], treeCLReadIndex[tree_index], treeCLWriteIndex[tree_index], tempSeq);

                            if (resultIndex != -1)
                            {
                                sendMessage = ch.chunkToByte(treeChunkList[tree_index][resultIndex], cConfig.ChunkSize);
                                stream.Write(sendMessage, 0, sendMessage.Length);
                                treeCLReadIndex[tree_index] = resultIndex;
                            }

                            if (tempSeq == 2147483647)
                                tempSeq = tree_index + 1;
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    delClientFromTreeDList(DThreadList_index, tree_index, ran_port);
                    stream = null;
                    //tempSeq = tree_index + 1;
                    tempSeq = 0;
                    firstRun = true;
                }
            }
        }

        private void TreePortHandle_Cport(int CThreadList_index, int tree_index)
        {
            int ran_port = TcpApps.RanPort(1401, 1600);
            TcpClient CPortClient = null;
            TcpListener CPortListener = new TcpListener(localAddr, ran_port);

            cPort cp = new cPort();
            cp.PortC = ran_port;
            cp.clientC = null;
            treeCPortList[tree_index].Add(cp);


            while (true)
            {
                CPortListener.Start(1);
                CPortClient = CPortListener.AcceptTcpClient();

                cPort cpt = new cPort();
                cpt.clientC = CPortClient;
                cpt.PortC = ran_port;
                treeCPortList[tree_index][CThreadList_index] = cpt;

                checkTreeDisconnect(CPortClient, ran_port, tree_index, CThreadList_index);
                CPortClient = null;
                CPortListener.Stop();
            }
        }

        private void checkTreeDisconnect(TcpClient client, int port_num, int tree_index, int CPortList_index)
        {
            NetworkStream stream = client.GetStream();
            Byte[] responseMessage = new Byte[256];
            cPort cp = new cPort();
            cp.PortC = port_num;

            try
            {
                while (true)
                {
                    //cp.clientC = client;
                    //treeCPortList[tree_index][CPortList_index] = cp;

                    int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
                    string responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);

                    if (responseString == "Exit")
                    {
                        // MessageBox.Show("Client[" + listNo + "] disconected");
                        cp.clientC = null;
                        treeCPortList[tree_index][CPortList_index] = cp;

                        int portD = treeDPortList[tree_index][CPortList_index].PortD;
                        delClientFromTreeDList(CPortList_index, tree_index, portD);

                        break;
                    }
                }
            }
            catch
            {
                //  MessageBox.Show("~~Client["+listNo+"] disconected");
                cp.clientC = null;
                treeCPortList[tree_index][CPortList_index] = cp;
            }
            stream = null;

        }

        ////yam:10-10-09
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
      
    }//end class
}
