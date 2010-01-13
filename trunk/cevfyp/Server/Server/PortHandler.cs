using System;
using System.Collections.Generic;
//using System.Linq;
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
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct dPort
    {
        public TcpClient clientD;
        public int PortD;
       
    }

    class PortHandler
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
        ServerConfig sConfig;

       //TcpClient CPortClient = null;

        private ServerFrm mainFm;
        private delegate void UpdateTextCallback(string message);

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
        bool playState = false;


           public bool playStateB
        {
            get { return playState; }
            set { playState = value; }
        }
            
        public PortHandler(int maxClient,int maxTree, string serverip, ServerFrm mainFm)
        {
            this.mainFm = mainFm;
            this.max_client = maxClient;
            this.max_tree = maxTree;
            localAddr = IPAddress.Parse(serverip);

           /* CPortList = new List<cPort>(max_client);
            D1PortList = new List<dPort>(max_client);
            D2PortList = new List<dPort>(max_client);

            //yam:10-10-09
            D1ThreadList = new List<Thread>(max_client);
            D2ThreadList = new List<Thread>(max_client);
            CThreadList = new List<Thread>(max_client);

            oddList = new List<Chunk>(CHUNKLIST_CAPACITY);
            evenList = new List<Chunk>(CHUNKLIST_CAPACITY);
           */

            ch = new ChunkHandler();
            sConfig = new ServerConfig();


          /*  for (int i = 0; i < max_client; i++)
            {
                cPort c = new cPort();
                c.PortC = NUM_CPORT_BASE + i;
                c.clientC = null;
                CPortList.Add(c);

                dPort d1 = new dPort();
               
                //d1.PortD = tapps.RanPort(1201, 1300);
               d1.PortD = NUM_D1PORT_BASE+i;
                d1.clientD = null;
               D1PortList.Add(d1);

                dPort d2 = new dPort();
                d2.PortD = NUM_D2PORT_BASE + i;
                d2.clientD = null;
                D2PortList.Add(d2);
            }
            */

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


            createTreeChunkList(maxTree,CHUNKLIST_CAPACITY);
            createTreePortList(maxTree, maxClient);
           // createTreeThreadList(maxTree, maxClient);

           
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

            
           // current_num = sChunk.seq;
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

        public void delClientFromTreeDList(int dList_index,int tree_index)
        {
            int port_num = treeDPortList[tree_index][dList_index].PortD;

            dPort dport = new dPort();
            dport.clientD = null;
            dport.PortD = port_num;

            treeDPortList[tree_index][dList_index] = dport;
        }

        public void delClientFromTreeCList(int cList_index, int tree_index)
        {

            int port_num = treeCPortList[tree_index][cList_index].PortC;

            cPort cport = new cPort();
            cport.clientC = null;
            cport.PortC = port_num;

            treeCPortList[tree_index][cList_index] = cport;
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
                    CPortThread.Name = " Cport_handle_" +i+"_"+ j;
                    CPortThread.Start();
                    Thread.Sleep(20);
                    CThreadList.Add(CPortThread);
                  
                    Thread DPortThread = new Thread(delegate() { TreePortHandle_Dport(j,i); });
                    DPortThread.IsBackground = true;
                    DPortThread.Name = " Dport_handle_" +i+"_"+ j;
                    DPortThread.Start();
                    Thread.Sleep(20);
                    DThreadList.Add(DPortThread);

                }

               // treeCThreadList.Add(CThreadList);
               // treeDThreadList.Add(DThreadList);

            }
        }

        private void TreePortHandle_Dport(int DThreadList_index,int tree_index)
        {
            NetworkStream stream;
            byte[] sendMessage = new byte[sConfig.ChunkSize];
            byte[] waitingMessage = new byte[sConfig.ChunkSize];


            int tempSeq = tree_index + 1;// 0;
            int tempRead_index = 0;
            bool firstRun = true;
            //bool[] firstRun = new bool[max_tree]; //temp seq, bool for each tree 

            int resultIndex = 0;
            TcpClient DPortClient;

            int ran_port = TcpApps.RanPort(1200, 1400);
            TcpListener DportListener = new TcpListener(localAddr,ran_port);
            //DportListener.Start(1);

            dPort dp = new dPort();
            dp.PortD = ran_port;
            dp.clientD = null;
            treeDPortList[tree_index].Add(dp);

            while (true)
            {
                DportListener.Start(1);
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
                        //if control port dead which cause this case happen
                        if (treeDPortList[tree_index][DThreadList_index].clientD == null)
                        {
                            mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T:" + tree_index +" D:"+ran_port+ " exit~\n" });
                            stream = null;
                            firstRun = true;
                            break;
                        
                        
                        }
                        
                        if (playState==false)
                        {
                           // stream.WriteTimeout = 5000;
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
                          //  stream.WriteTimeout=5000;
                            sendMessage = ch.chunkToByte(treeChunkList[tree_index][tempRead_index], sConfig.ChunkSize);
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
                catch(Exception ex)
                {
                    mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T:" + tree_index + " D:" + ran_port + " exit\n" });


                    delClientFromTreeDList(DThreadList_index, tree_index);
                    delClientFromTreeCList(DThreadList_index, tree_index);

                    stream = null;
                    tempSeq = 0;
                    firstRun = true;

                }
                DPortClient = null;
                DportListener.Stop();
            }
        }

        private void TreePortHandle_Cport(int CThreadList_index,int tree_index)
        {
            int ran_port = TcpApps.RanPort(1401, 1600);
            TcpClient CPortClient = null;
            TcpListener CPortListener = new TcpListener(localAddr,ran_port);

            cPort cp = new cPort();
            cp.PortC = ran_port;
            cp.clientC = null;
            treeCPortList[tree_index].Add(cp);
            NetworkStream stream;
            Byte[] responseMessage = new Byte[4];

            while (true)
            {
                CPortListener.Start(1);
                CPortClient = CPortListener.AcceptTcpClient();

                cPort cpt = new cPort();
                cpt.clientC = CPortClient;
                cpt.PortC = ran_port;
                treeCPortList[tree_index][CThreadList_index] = cpt;
                stream = CPortClient.GetStream();
                


                //checkTreeDisconnect(CPortClient, ran_port, tree_index, CThreadList_index);

                try
                {
                    while (true)
                    {
                        //if streaming port dead which cause this case happen
                        if (treeCPortList[tree_index][CThreadList_index].clientC == null)
                        {
                            mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T:" + tree_index + " C:" + ran_port + " exit~\n" });
                            break;
                        }
                        stream.ReadTimeout = 5000;
                        int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
                        string responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);

                        if (responseString == "Exit")
                        {
                            cp.clientC = null;
                            treeCPortList[tree_index][CThreadList_index] = cp;

                            delClientFromTreeDList(CThreadList_index, tree_index);
                            mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T:" + tree_index + " C:" + ran_port + " exit!\n" });

                            break;
                        }

                        if (responseString == "Wait")
                        {
                            Thread.Sleep(20);
                            continue;

                        }
                    }
                }
                catch
                {
                    mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "T:" + tree_index + "C:" + ran_port + "exit\n" });

                    cpt.clientC = null;
                    treeCPortList[tree_index][CThreadList_index] = cp;

                    delClientFromTreeDList(CThreadList_index, tree_index);
                    stream = null;

                }

                CPortClient = null;
                CPortListener.Stop();
            }
        }

        /*private void checkTreeDisconnect(TcpClient client, int port_num,int tree_index,int CPortList_index)
        {
            NetworkStream stream = client.GetStream();
            Byte[] responseMessage = new Byte[4];
            cPort cp = new cPort();
            cp.PortC = port_num;

            try
            {
                while (true)
                {
                    
                    if (treeCPortList[tree_index][CPortList_index].clientC == null)
                        break;

                    stream.ReadTimeout = 10000;
                    int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
                    string responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);

                    if (responseString == "Exit")
                    {
                        cp.clientC = null;
                        treeCPortList[tree_index][CPortList_index] = cp;

                        delClientFromTreeDList(CPortList_index, tree_index);

                      ///if(treeDPortList[tree_index][CPortList_index].clientD==null)
                        //mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "treeD:"+tree_index+" del \n" });

                        //if (treeCPortList[tree_index][CPortList_index].clientC == null)
                          //  mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "treeC:" + tree_index + " del \n" });
                        
                   
                        break;
                    }

                   
                }
            }
            catch
            {
                 
               cp.clientC = null;
               treeCPortList[tree_index][CPortList_index] = cp;

               delClientFromTreeDList(CPortList_index, tree_index);
                
            }
            stream = null;

        }
     */ 
        
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


       /*
        public TcpClient getCListClient(int listIndex)
        {
            return CPortList[listIndex].clientC;
        }

        public TcpClient getDListClient(int listIndex, int tree)
        {
            if (tree == 1)
                return D1PortList[listIndex].clientD;
            else
                return D2PortList[listIndex].clientD;
        }

        public int getCListPort(int listIndex)
        {
            return CPortList[listIndex].PortC; 
        }

        public int getDListPort(int listIndex,int tree)
        {
            if (tree == 1)
                return D1PortList[listIndex].PortD;
            else
                return D2PortList[listIndex].PortD;
        }

        public void delClientFromDList(int listIndex,int treeNo,int portNo)
        {
            dPort d = new dPort();
            d.clientD=null;
            d.PortD = portNo;

            if(treeNo==1)
              D1PortList[listIndex] = d;
            else
              D2PortList[listIndex] = d;
        }

        public void setOddListChunk(Chunk streamingChunk)
        {
            Chunk streamingChunkOdd = Chunk.Copy(streamingChunk);

                if (oddList.Count <= CHUNKLIST_CAPACITY)
                    oddList.Add(streamingChunkOdd);
                else
                    oddList[oddList_wIndex] = streamingChunkOdd;

                if (oddList_wIndex == CHUNKLIST_CAPACITY)
                    oddList_wIndex = 0;
                else
                    oddList_wIndex += 1;

                currentOddNo = streamingChunkOdd.seq;
            
        }

        public void setEvenListChunk(Chunk streamingChunk)
        {
            Chunk streamingChunkEven = Chunk.Copy(streamingChunk);
            
            if (evenList.Count <= CHUNKLIST_CAPACITY)
                evenList.Add(streamingChunkEven);
            else
                evenList[evenList_wIndex] = streamingChunkEven;

            if (evenList_wIndex == CHUNKLIST_CAPACITY)
                evenList_wIndex = 0;
            else
                evenList_wIndex += 1;

            currentEvenNo = streamingChunkEven.seq;
        }

        public void startPort()
        {
            //yam:10-10-09 
            for (int j = 0; j < max_client; j++)
            {
               
                Thread PortThread = new Thread(delegate() { PortHandle_Cport(j); });
                PortThread.IsBackground = true;
                PortThread.Name = " port_handle_"+j;
                PortThread.Start();
                Thread.Sleep(100);
                CThreadList.Add(PortThread);

                Thread d1PortThread = new Thread(delegate() { PortHandle_D1port(j); });
                d1PortThread.IsBackground = true;
                d1PortThread.Name = " D1port_handle_" + j;
                d1PortThread.Start();
                Thread.Sleep(100);
                D1ThreadList.Add(d1PortThread);

                Thread d2PortThread = new Thread(delegate() { PortHandle_D2port(j); });
                d2PortThread.IsBackground = true;
                d2PortThread.Name = " D2port_handle_" + j;
                d2PortThread.Start();
                Thread.Sleep(100);
                D2ThreadList.Add(d2PortThread);
              
            }

        }

        //yam:11-10-09
        private void PortHandle_D1port(int clientNo)
        {
        
            
            //int tempListIndex = 0;
            NetworkStream stream;
            byte[] sendMessage = new byte[sConfig.ChunkSize];
            byte[] responseMessage = new byte[20];
           
            bool firstRun = true;
            int tempSeq = 0;
            int resultIndex = 0; 
            TcpClient D1PortClient;

           // int aaa = tapps.RanPort(1200, 1300);
            TcpListener listenD1port = new TcpListener(localAddr,(NUM_D1PORT_BASE + clientNo));
            listenD1port.Start(1);

            //dPort d11 = new dPort();
           // d11.PortD = aaa;
           // d11.clientD = null;
           // D1PortList.Add(d11);

                while (true)
                {
                    D1PortClient = listenD1port.AcceptTcpClient();

                    dPort d1 = new dPort();
                    d1.clientD = D1PortClient;
                    d1.PortD = (NUM_D1PORT_BASE + clientNo);
                    stream = D1PortClient.GetStream();

                    D1PortList[clientNo]=d1;

                    try
                    {
                        while (true)
                        {
                                if (oddList.Count > 10 && tempSeq <= currentOddNo)
                                {
                                    if (firstRun == true)
                                    {
                                        tempSeq = currentOddNo;
                                        firstRun = false;
                                    }

                                    resultIndex = search(oddList, oddList_rIndex, oddList_wIndex, tempSeq);

                                    if (resultIndex != -1)
                                    {

                                        sendMessage = ch.chunkToByte(oddList[resultIndex], sConfig.ChunkSize);
                                        stream.Write(sendMessage, 0, sendMessage.Length);
                                        oddList_rIndex = resultIndex;
                                    }
                                    
                                    
                                    if (tempSeq == 2147483647)
                                        tempSeq = 1;
                                    else
                                        tempSeq += 2;
                                }
                            
                            Thread.Sleep(5);
                        }
                    }
                    catch
                    {
                        delClientFromDList(clientNo, 1, NUM_D1PORT_BASE + clientNo);
                        stream = null;
                       //  startSendstate = false;
                         firstRun = true;
                        tempSeq = 0;
                    }
                }
         

        }

        private void PortHandle_D2port(int clientNo)
        {
            
            NetworkStream stream;
            byte[] sendMessage = new byte[sConfig.ChunkSize];
            byte[] responseMessage = new byte[20];
          
            bool firstRun = true;
            int tempSeq = 0;
            int resultIndex = 0; 
            TcpClient D2PortClient;

            TcpListener listenD2port = new TcpListener(localAddr, (NUM_D2PORT_BASE + clientNo));
            listenD2port.Start(1);

            while (true)
            {
                    D2PortClient = listenD2port.AcceptTcpClient();

                    dPort d2 = new dPort();
                    d2.clientD = D2PortClient;
                    d2.PortD = (NUM_D2PORT_BASE + clientNo);
                    stream = D2PortClient.GetStream();

                    D2PortList[clientNo] = d2;

                    try
                    {
                        while (true)
                        {

                            if (evenList.Count > 10 && tempSeq <= currentEvenNo)
                            {
                                if (firstRun == true)
                                {
                                    tempSeq = currentEvenNo;
                                    firstRun = false;
                                }

                                resultIndex = search(evenList, evenList_rIndex, evenList_wIndex, tempSeq);

                                if (resultIndex != -1)
                                {
                                    sendMessage = ch.chunkToByte(evenList[resultIndex], sConfig.ChunkSize);
                                    stream.Write(sendMessage, 0, sendMessage.Length);
                                    evenList_rIndex = resultIndex;
                                }

                                if (tempSeq == 2147483647)
                                    tempSeq = 2;
                                else
                                    tempSeq += 2;
                            }

                            Thread.Sleep(5);
                        }
                    }
                    catch
                    {
                        delClientFromDList(clientNo, 2, NUM_D2PORT_BASE + clientNo);
                        stream = null;
                        //startSendstate = false;
                         firstRun = true;
                        tempSeq = 0;
                    }
                }
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
       
        private void PortHandle_Cport(int clientNo)
        {
            TcpClient CPortClient = null;
            TcpListener listenPort = new TcpListener(localAddr, (NUM_CPORT_BASE + clientNo));
            //TcpListenerList.Add(listenPort);

                while (true)
                {
                    listenPort.Start(1);
                    CPortClient = listenPort.AcceptTcpClient();
                    checkDisconnect(CPortClient, (NUM_CPORT_BASE + clientNo), clientNo);
                    CPortClient = null;
                    listenPort.Stop();
                }
        }

        private void checkDisconnect(TcpClient client, int portNo, int listNo)
        {
            NetworkStream stream = client.GetStream();
            Byte[] responseMessage = new Byte[256];
            cPort c = new cPort();
            c.PortC = portNo;
           
            try
            {
                while (true)
                {
                    c.clientC = client;
                    CPortList[listNo] = c;
                  

                    int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
                    string responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);

                    if (responseString == "Exit")
                    {
               //         MessageBox.Show("Client[" + listNo + "] disconected");
                       c.clientC = null;
                       CPortList[listNo] = c;
                        break;
                    }
                }
            }
            catch
            {
             //   MessageBox.Show("~Client["+listNo+"] disconected");
                c.clientC = null;
                CPortList[listNo] = c;
            }
            stream = null;
          
        }
         */
      
    }//end class
}//end namespace
