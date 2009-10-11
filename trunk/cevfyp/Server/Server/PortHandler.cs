using System;
using System.Collections.Generic;
using System.Linq;
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
        
        static int NUM_D1PORT_BASE = 1200;
        static int NUM_D2PORT_BASE = 1250;

        static int NUM_CPORT_BASE = 1301;
        static int TREE_NO = 2;             //number of tree
      
        List<cPort> CPortList;
        List<Thread> CThreadList;

        List<dPort> D1PortList;  //odd
        List<dPort> D2PortList;  //even
  
 //yam:10-10-09
        List<Thread> D1ThreadList; 
        List<Thread> D2ThreadList;
        static int CHUNKLIST_CAPACITY = 200;
        List<Chunk> oddList;
        List<Chunk> evenList;
        int currentOddNo = 0;
        int currentEvenNo = 0;
        int oddList_wIndex = 0;
        int oddList_rIndex = 0;
        int evenList_wIndex = 0;
        int evenList_rIndex = 0;
        ChunkHandler ch;
        ServerConfig sConfig;
        int lb, ub, mid, tempResult;
       


        //TcpListener listenD1port = null;
       // TcpListener listenD2port = null;
     
       // List<TcpListener> TcpListenerList;

       // Thread PortThread_Dport = null;
        TcpClient CPortClient = null;

        private ServerFrm mainFm;
        private delegate void UpdateTextCallback(string message);

        public PortHandler(int maxClient, string serverip, ServerFrm mainFm)
        {
            this.mainFm = mainFm;
            this.max_client = maxClient;
            localAddr = IPAddress.Parse(serverip);
            CPortList = new List<cPort>(max_client);

            D1PortList = new List<dPort>(max_client);
            D2PortList = new List<dPort>(max_client);

 //yam:10-10-09
            D1ThreadList = new List<Thread>(max_client);
            D2ThreadList = new List<Thread>(max_client);
            oddList = new List<Chunk>(CHUNKLIST_CAPACITY);
            evenList = new List<Chunk>(CHUNKLIST_CAPACITY);
            ch = new ChunkHandler();
            sConfig = new ServerConfig();

            CThreadList = new List<Thread>(max_client);
            //TcpListenerList = new List<TcpListener>(max_client);

            for (int i = 0; i < max_client; i++)
            {
                cPort c = new cPort();
                c.PortC = NUM_CPORT_BASE + i;
                c.clientC = null;
                CPortList.Add(c);

                dPort d1 = new dPort();
                d1.PortD = NUM_D1PORT_BASE + i;//yam:10-10-09
                d1.clientD = null;
                D1PortList.Add(d1);

                dPort d2 = new dPort();
                d2.PortD = NUM_D2PORT_BASE + i;//yam:10-10-09
                d2.clientD = null;
                D2PortList.Add(d2);
            }
        }

    

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

//yam:10-10-09
        public int getDListPort(int listIndex,int tree)
        {
            if (tree == 1)
                return D1PortList[listIndex].PortD;
            else
                return D2PortList[listIndex].PortD;
        }

 //yam:10-10-09
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
//yam:10-10-09
        public void setOddListChunk(Chunk streamingChunk)
        {
            Chunk streamingChunkOdd = Chunk.Copy(streamingChunk);
         
            if (oddList.Count() <= CHUNKLIST_CAPACITY)
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
            
            if (evenList.Count() <= CHUNKLIST_CAPACITY)
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
                /*
                Thread PortThread = new Thread(delegate() { PortHandle_Dport(i,1); });
                PortThread.IsBackground = true;
                PortThread.Name = " D1port_handle_" + i;
                PortThread.Start();
                Thread.Sleep(100);
                D1ThreadList.Add(PortThread);

                Thread PortThread2 = new Thread(delegate() { PortHandle_Dport(i,2); });
                PortThread2.IsBackground = true;
                PortThread2.Name = " D2port_handle_" + i;
                PortThread2.Start();
                Thread.Sleep(100);
                D2ThreadList.Add(PortThread);
                */

            for (int j = 0; j < max_client; j++)
            {
               
                Thread PortThread = new Thread(delegate() { PortHandle_Cport(j); });
                PortThread.IsBackground = true;
                PortThread.Name = " port_handle_"+j;
                PortThread.Start();
                Thread.Sleep(100);
                CThreadList.Add(PortThread);

//yam:11-10-09
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

            TcpListener listenD1port = new TcpListener(localAddr, (NUM_D1PORT_BASE + clientNo));
            listenD1port.Start(1);
          
                while (true)
                {
                    D1PortClient = listenD1port.AcceptTcpClient();
                    dPort d1 = new dPort();
                    d1.clientD = D1PortClient;
                    d1.PortD = (NUM_D1PORT_BASE + clientNo);
                    stream = D1PortClient.GetStream();

                  /*  for (int i = 0; i < max_client; i++)
                    {
                        if (D1PortList[i].clientD == null)
                        {
                            D1PortList[i] = d1;
                            tempListIndex = i;
                            break;
                        }
                    }
                    */
                    D1PortList[clientNo]=d1;

                    try
                    {
                        while (true)
                        {
                           if (oddList.Count > 1 && tempSeq<=currentOddNo)
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

                                    if (tempSeq == 2147483647)
                                        tempSeq = 1;
                                    else
                                        tempSeq += 2;
                                
                                }
                            }
                            Thread.Sleep((20+clientNo));
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
            //int tempListIndex = 0;
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

                   /* for (int i = 0; i < max_client; i++)
                    {
                        if (D2PortList[i].clientD == null)
                        {
                            D2PortList[i] = d2;
                            tempListIndex = i;
                            break;
                        }
                    }
                */
                    D2PortList[clientNo] = d2;

                    try
                    {
                        while (true)
                        {
                         
                            if (evenList.Count > 1 && tempSeq <= currentEvenNo)
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

                                    if (tempSeq == 2147483647)
                                        tempSeq = 2;
                                    else
                                        tempSeq += 2;
                                }
                            }
                            Thread.Sleep((20 + clientNo));
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


 //yam:10-10-09 
       /* 
        private void PortHandle_Dport(int clientNo,int treeNo)
        {
            int treeno = treeNo;
            int tempListIndex = 0;
            NetworkStream stream;
            byte[] sendMessage = new byte[sConfig.ChunkSize];
            byte[] responseMessage = new byte[4];
            TcpListener listenD1port, listenD2port;

            if (treeno == 1)
            {
                listenD1port = new TcpListener(localAddr, (NUM_D1PORT_BASE + clientNo));
               // listenD1port.Start(1);
            }
            else
            {
                listenD2port = new TcpListener(localAddr, (NUM_D2PORT_BASE + clientNo));
                listenD2port.Start(1);
            }

       
                while (true)
                {
                    if (treeno == 1)
                    {
                        listenD1port.Start(1);
                        D1PortClient = listenD1port.AcceptTcpClient();
                        dPort d1 = new dPort();
                        d1.clientD = D1PortClient;
                        d1.PortD = (NUM_D1PORT_BASE + clientNo);

                        for (int i = 0; i < max_client; i++)
                        {
                            if (D1PortList[i].clientD == null)
                            {
                                D1PortList[i] = d1;
                                tempListIndex = i;
                                break;
                            }
                        }
                        


                        //---Send odd streaming chunk to peer---//
                        try
                        {
                            while (true)
                            {
                                stream = D1PortClient.GetStream();
                                int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);


                                //sendMessage = ch.chunkToByte(streamingChunk, sConfig.ChunkSize);
                                //stream.Write(sendMessage, 0, sendMessage.Length);

                                Thread.Sleep(20);
                            }
                        }
                        catch
                        {
                            delClientFromDList(tempListIndex, 1, NUM_D1PORT_BASE + clientNo);
                            stream = null;
                            D1PortClient.Close();
                           
                           // D1PortClient = null;
                            
                        }

                    }
                    else
                    {
                       // listenD2port.Start(1);
                        D2PortClient = listenD2port.AcceptTcpClient();
                        dPort d2 = new dPort();
                        d2.clientD = D2PortClient;
                        d2.PortD = (NUM_D2PORT_BASE + clientNo);

                        for (int i = 0; i < max_client; i++)
                        {
                            if (D2PortList[i].clientD == null)
                            {
                                D2PortList[i] = d2;
                                tempListIndex = i;
                                break;
                            }
                        }

                       //---Send even streaming chunk to peer---//
                        try
                        {
                            while (true)
                            {
                                stream = D2PortClient.GetStream();
                                int responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);


                                Thread.Sleep(20);
                            }
                        }
                        catch
                        {
                            delClientFromDList(tempListIndex, 2, NUM_D2PORT_BASE + clientNo);
                            stream = null;
                            D2PortClient.Close();
                            
                        }


                    }

                } //end while loop
        

        }
        */


       

       
        private void PortHandle_Cport(int clientNo)
        {
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
            int tempListIndex = 0;
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
      
    }//end class
}//end namespace
