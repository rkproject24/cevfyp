﻿using System;
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

namespace Client
{
    class ClientHandler
    {
        //static int CHUNKLIST_CAPACITY = 100;  //0-xxx
        //static int RECV_CHUNK_SIZE = 3182;
        //static int CHUNK_BUF = 1;
        //static int START_BUF = 2;
        //static int TREE_NO = 2;

        //===============upload variable=============
        string CLIENTIP = "127.0.0.1";
       
        //static int PeerListenPort = 1100;
        int PeerListenPort;
        int max_peer;
        bool uploading = false; //indicate whether upload thread is started
        TcpListener listenPeer;
        UploadPortHandler upPorth;

        Thread listenerThread;

        //===================================

        int treeNO = 0;
        //int[] chunkList_wIndex = new int[TREE_NO]; 
        int chunkList_wIndex = 0;  //write index
        int chunkList_rIndex = 0;  //read index
        //int oddList_wIndex = 0;
        //int oddList_rIndex = 0;  
        //int evenList_wIndex = 0;
        //int evenList_rIndex = 0;
        int tempSeq = 0;
        //int currentEvenNo = 0;
        //int currentOddNo = 0;
        int lb, ub, mid, tempResult;
        int resultIndex = 0;        
        int virtualServerPort = 0;   //virtual server broadcast port number

        bool checkToBoardcast = false;
        bool serverConnect = false;
        bool vlcConnect = false;
        bool checkClose = false;

        bool useOddSeqChunk = true;
        bool checkedFirstChunk = false;
        bool oddListAvailable = false;
        bool evenListAvailable = false;

        string trackerIp = null;

        VlcHandler vlc;
        ChunkHandler ch;

        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        List<Chunk> chunkList;// = new List<Chunk>(CHUNKLIST_CAPACITY);

        //List<Chunk> oddList;// = new List<Chunk>(CHUNKLIST_CAPACITY);
        //List<Chunk> evenList;// = new List<Chunk>(CHUNKLIST_CAPACITY);

        //yam:02-01-10
        List<List<Chunk>> treeChunkList;// = new List<List<Chunk>>(TREE_NO);
        int[] treeCLWriteIndex;// = new int[TREE_NO];
        int[] treeCLReadIndex;// = new int[TREE_NO];
        int[] treeCLCurrentSeq;// = new int[TREE_NO];
       // int[] treeCLWaitRound = new int[TREE_NO];


        List<Thread> recciveChunkThread;// = new List<Thread>(TREE_NO);
        Thread broadcastVlcStreamingThread;
        Thread updateChunkListThread;

        //TcpClient ClientC;
        List<TcpClient> ClientC;// = new List<TcpClient>(TREE_NO);
        List<TcpClient> ClientD;// = new List<TcpClient>(TREE_NO); //Tree list of data TCP

        TcpListener server;
        NetworkStream localvlcstream;

        private ClientForm mainFm;
        public delegate void UpdateTextCallback(string message);
        private PeerHandler peerh;

        bool playState = false; //yam:11-10-09

        ClientConfig cConfig = new ClientConfig();

        public ClientHandler(ClientForm mainFm)
        {
            this.mainFm = mainFm;
            vlc = new VlcHandler();
            ch = new ChunkHandler();

            cConfig.load("C:\\ClientConfig");
            chunkList = new List<Chunk>(cConfig.ChunkCapacity);
            

            string[] xmlList = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");
            foreach (string xmlfile in xmlList)
            {
                File.Delete(xmlfile);
            }
            //oddList = new List<Chunk>(cConfig.ChunkCapacity);
            //evenList = new List<Chunk>(cConfig.ChunkCapacity);

            //load config
            //cConfig = new ClientConfig("C:\\vlc-0.9.9", 1100, 2100, 2200, 2301, 200, 3, RECV_CHUNK_SIZE, CHUNKLIST_CAPACITY, CHUNK_BUF, START_BUF);
            //cConfig.save("C:\\ClientConfig.xml");
        }

        public void initialize()
        {
            treeChunkList = new List<List<Chunk>>(treeNO);
            treeCLWriteIndex = new int[treeNO];
            treeCLReadIndex = new int[treeNO];
            treeCLCurrentSeq = new int[treeNO];

            recciveChunkThread = new List<Thread>(treeNO);

            ClientC = new List<TcpClient>(treeNO);
            ClientD = new List<TcpClient>(treeNO); //Tree list of data TCP


        }

//yam:11-10-09
        public void setPlayState(bool state)
        {
            playState = state;
        }

        public void getMute()
        {
            int checkMute = vlc.getMute();
            if (checkMute == 0)
                vlc.setMute(1);
            else
                vlc.setMute(0);

        }

        /*
        public string LocalIPAddress()
        {
            IPHostEntry host;
           string localIP = "";
           host = Dns.GetHostEntry(Dns.GetHostName());
           foreach (IPAddress ip in host.AddressList)
           {
               if (ip.AddressFamily.ToString() == "InterNetwork")
                   localIP = ip.ToString();
           }
           return localIP;
        }
        */

        public string establishConnect(string tackerIp)
        {
            string response = "";

            //connect tracker
            response = connectToTracker(tackerIp);

            if (response == "OK")
                response = connectToPeer();
            else
                return response;
            //=======================================

            //response = connectToPeer();

            if (response == "OK2")
                response = connectToSource();
            else
                return response;

            if (response == "OK3")
                return response = "";

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

        private string connectToPeer()
        {
            //if(peerh.PeerIp.Equals("NOPEER"))
            //    return "NO Peer available in Peer list!";
            if (peerh.connectPeer())
            {
                //virtualServerPort = peerh.Cport1 + cConfig.VlcPortBase;
                //serverConnect = true;
                //checkClose = false;
                return "OK2";
            }
            else
            {
                return "Peer Unreachable!";
            }
        }

        private string connectToSource()
        {
            //virtualServerPort = peerh.Cport1 + cConfig.VlcPortBase;
            virtualServerPort = peerh.Cport11+ cConfig.VlcPortBase;
            serverConnect = true;
            checkClose = false;

            PeerListenPort = TcpApps.RanPort(1100, 1120);
        //by Yam
            try
            {
                    //ClientC = new TcpClient(sourceIp, peerh.Cport1);
               /*
                ClientC = new TcpClient(peerh.PeerIp.Ip, peerh.Cport1);
              
                for (int i = 0; i < TREE_NO; i++)
                {
                    ClientD.Add(peerh.getDataConnect(i));

                    peerh.registerToTracker(i, peerh.PeerIp.Layer.ToString()); //by vinci: register To Tree in Tracker
                }
                */

                for (int i = 0; i < treeNO; i++)
                {
                    ClientC.Add(peerh.getControlConnect(i));
                    ClientD.Add(peerh.getDataConnect(i));

                    //peerh.registerToTracker(i, PeerListenPort, peerh.PeerIp.Layer.ToString()); //by vinci: register To Tree in Tracker
                    peerh.registerToTracker(i, PeerListenPort, peerh.PeerIp.Layer.ToString()); //by vinci: register To Tree in Tracker
                }

                

                return "OK3";
            }
            catch
            {
                return "One of Data Ports cannot join!!!!";
            }

        }

        public void startThread()
        {
            tempSeq = 0;

            for (int i = 0; i < treeNO; i++)
            {
                List<Chunk> chunkLists = new List<Chunk>(cConfig.ChunkCapacity);
                treeChunkList.Add(chunkLists);

                Thread DRecvThread = new Thread(delegate() { receiveTreeChunk(ClientD[i], i); });
                DRecvThread.IsBackground = true;
                DRecvThread.Name = " DRecv_handle_" + i;
                DRecvThread.Start();
                Thread.Sleep(10);
                recciveChunkThread.Add(DRecvThread);

            }

                //startUpload();
            
                updateChunkListThread = new Thread(new ThreadStart(updateTreeChunkList));
                updateChunkListThread.IsBackground = true;
                updateChunkListThread.Name = "update_ChunkList";
                updateChunkListThread.Start();

                broadcastVlcStreamingThread = new Thread(new ThreadStart(broadcastVlcStreaming));
                broadcastVlcStreamingThread.IsBackground = true;
                broadcastVlcStreamingThread.Name = "broadcast_VlcStreaming";
                broadcastVlcStreamingThread.Start();

                vlc.play(mainFm.panel1, virtualServerPort);
        }

        public void startUpload()
        {
            if (!uploading)
            {
                
                CLIENTIP = mainFm.tbhostIP.Text.ToString();


                //start uploading thread
                this.max_peer = cConfig.MaxPeer;
                upPorth = new UploadPortHandler(cConfig, CLIENTIP, mainFm, treeNO);
                upPorth.startTreePort();
                //mainFm.rtbupload.AppendText("upPorth.startPort()");

                //localAddr = IPAddress.Parse(sConfig.Serverip);
                listenerThread = new Thread(new ThreadStart(listenForClients));
                listenerThread.IsBackground = true;
                listenerThread.Name = " listen_for_peers";
                listenerThread.Start();

                uploading = true;
            }
        }

        public void sendMessage(object tcpClinet)
        {
            TcpClient clientC = (TcpClient)tcpClinet;
            NetworkStream stream = clientC.GetStream();
            try
            {
                    if (checkClose == true)
                    {
                        byte[] sendMessage = System.Text.Encoding.ASCII.GetBytes("Exit");
                        stream.Write(sendMessage, 0, sendMessage.Length);
                      
                        stream.Close();
                        clientC.Close();
                    }
            }
            catch
            {
                stream.Close();
                clientC.Close();
            }
        }

        public void receiveTreeChunk(TcpClient tcpClient, int tree_index)
        {
            TcpClient clientD = tcpClient;
            NetworkStream stream = clientD.GetStream();
            BinaryFormatter bf = new BinaryFormatter();


            try
            {
                // byte[] responseMessage = new byte[cConfig.ChunkSize];
                Chunk streamingChunk;
                int responseMessageBytes;

                while (serverConnect)
                {
                    byte[] responseMessage = new byte[cConfig.ChunkSize];
                    responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);

                    streamingChunk = (Chunk)ch.byteToChunk(bf, responseMessage);

                    if (streamingChunk == null)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    //by vinci: send responseBytes directly to peer
                    //this.uploadToPeer(responseMessageBytes);
                    if(uploading)
                    {
                        upPorth.setChunkList(streamingChunk, tree_index);
                    }

                    //by yam:04-01-10
                    int write_index = treeCLWriteIndex[tree_index];

                    if (treeChunkList[tree_index].Count <= cConfig.ChunkCapacity)
                        treeChunkList[tree_index].Add(streamingChunk);
                    else
                        treeChunkList[tree_index][write_index] = streamingChunk;

                    if (write_index == cConfig.ChunkCapacity)
                        treeCLWriteIndex[tree_index] = 0;
                    else
                        treeCLWriteIndex[tree_index] += 1;

                    treeCLCurrentSeq[tree_index] = streamingChunk.seq;

                  
                  //  if(tree_index==0)
                      //  mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { treeChunkList[0][write_index].seq.ToString()+" " });
                       // mainFm.tbWriteStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox1), new object[] { treeCLWriteIndex[tree_index].ToString() });

                   // if (tree_index == 1)
                     //   mainFm.rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { treeChunkList[1][write_index].seq.ToString() + " " });
                       // mainFm.tbReadStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { treeChunkList[1][write_index].seq.ToString() });

                      //if (tree_index == 2)
                    //    mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { treeChunkList[2][write_index].seq.ToString() + " " });
                       //   mainFm.tbStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox3), new object[] { treeChunkList[2][write_index].seq.ToString() + " " });

                      
                    Thread.Sleep(20);

                } //end while loop
                stream.Close();
                clientD.Close();
            }
            catch (Exception ex)
            {
                stream.Close();
                clientD.Close();
                MessageBox.Show(ex.ToString());
            }

        }

        private void updateTreeChunkList()
        {
            while (true)
            {
                if (treeChunkList[0].Count > 0)   //Problem: Cause slow start when there is many sub stream
                {
                    tempSeq = treeChunkList[0][0].seq;
                    break;
                }
                Thread.Sleep(30);
            }

            int remainder_num,check_num;

            while (true)
            {
                for (int i = 0; i < treeNO; i++)
                {
                    
                    //check the tagert seqnumber belong to which tree
                    remainder_num = tempSeq % treeNO;
                    if (remainder_num== 0)
                       check_num=treeNO - 1;
                    else
                       check_num=remainder_num - 1;

                   if (tempSeq <= treeCLCurrentSeq[i] && check_num==i)
                    {
                       //by yam : search method
                        int r_index = searchChunk(i,treeCLReadIndex[i],treeCLWriteIndex[i], tempSeq);
                        if (r_index != -1)
                        {
                            //add to main chunk list
                            if (chunkList.Count <= cConfig.ChunkCapacity)
                                chunkList.Add(treeChunkList[i][r_index]);
                            else
                                chunkList[chunkList_wIndex] = treeChunkList[i][r_index];

                            if (chunkList_wIndex == cConfig.ChunkCapacity)
                                chunkList_wIndex = 0;
                            else
                                chunkList_wIndex += 1;

                            if (r_index == cConfig.ChunkCapacity)
                                treeCLReadIndex[i] = 0;
                            else
                                treeCLReadIndex[i] = r_index;

                             //mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "tree:" + i+ " : "+ treeChunkList[i][r_index].seq + "\n" });

                        }
                        if (tempSeq == 2147483647)
                            tempSeq = 1;
                        else
                            tempSeq += 1; 
                    }
                }
                Thread.Sleep(30);
            }
        }
       
        private void broadcastVlcStreaming()
        {
            server = new TcpListener(localAddr, virtualServerPort);
            server.Start();

            TcpClient client = server.AcceptTcpClient();
            localvlcstream = client.GetStream();

            byte[] sendMessage = System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-type: application/octet-stream\r\nCache-Control: no-cache\r\n\r\n");
            localvlcstream.Write(sendMessage, 0, sendMessage.Length);

            if (client.Connected == true) vlcConnect = true;

            byte[] sendClientChunk;

            while (client.Connected == true && vlcConnect)
            {
                checkbuff();
                if (checkToBoardcast == true)
                {
                    sendClientChunk = new byte[chunkList[chunkList_rIndex].bytes];
                    Array.Copy(chunkList[chunkList_rIndex].streamingData, 0, sendClientChunk, 0, chunkList[chunkList_rIndex].bytes);
                    localvlcstream.Write(sendClientChunk, 0, sendClientChunk.Length);

                   mainFm.tbReadStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { chunkList_rIndex.ToString() + ":" + chunkList[chunkList_rIndex].seq });

                    if (chunkList_rIndex == cConfig.ChunkCapacity)
                        chunkList_rIndex = 0;
                    else
                        chunkList_rIndex += 1;

                }
                Thread.Sleep(10);
            }
            localvlcstream.Close();
            client.Close();
        }
      
        private int searchChunk(int list_index, int rIndex, int wIndex, int target)
        {
           if (wIndex < rIndex)
            {
                lb = rIndex;
                ub = cConfig.ChunkCapacity;
                tempResult = binarySearch(list_index, lb, ub, target);
                if (tempResult != -1)
                    return tempResult;
                else
                {
                    lb = 0;
                    ub = wIndex - 1;
                    tempResult = binarySearch(list_index, lb, ub, target);
                    return tempResult;
                }
            }
            else
           {
                lb = rIndex;
                ub = wIndex -1;

                tempResult = binarySearch(list_index, lb, ub, target);
                return tempResult;
            }
        }

        private int binarySearch(int list_index, int lb, int ub, int target)
        {
            for (; lb <= ub; )
            {
                mid = (lb + ub) / 2;

                if (treeChunkList[list_index][mid].seq == target)
                    return mid;
                else if (target > treeChunkList[list_index][mid].seq)
                    lb = mid + 1;
                else
                    ub = mid - 1;
            }
            return -1;
        }



        private void checkbuff() // by vinci:TO keep buffer for chunklist
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

            if (indexdiff <= cConfig.ChunkBuf)
            {
                if (!(indexdiff < 0))
                {
                    checkToBoardcast = false;
                    if (mainFm.tbStatus.Text != "Buffering...")
                        mainFm.tbStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox3), new object[] { "Buffering..." });
                }
            }
            else if (indexdiff > cConfig.StartBuf)
            {
                if (mainFm.tbStatus.Text != "Playing")
                    mainFm.tbStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox3), new object[] { "Playing" });
                checkToBoardcast = true;
            }
        }       

        public void disconectall()
        {
            if (serverConnect == true)
            {
                serverConnect = false;
                vlcConnect = false;
                checkClose = true;

                //this.sendMessage(ClientC);
                for (int i = 0; i < treeNO; i++)
                {
                    this.sendMessage(ClientC[i]);
                }



                broadcastVlcStreamingThread.Join(1000);
                vlc.stop();

                for (int i = 0; i < treeNO; i++)
                {
                    recciveChunkThread[i].Join(1000);
                }
                //updateChunkListThread = null;
                updateChunkListThread.Abort();
                server.Stop();


                recciveChunkThread.Clear();
                ClientD.Clear();
                ClientC.Clear();


                chunkList_wIndex = 0;
                chunkList_rIndex = 0;
                //oddList_wIndex = 0;
                //oddList_rIndex = 0;
                //evenList_wIndex = 0;
                //evenList_rIndex = 0;
                virtualServerPort = 0;


                //currentEvenNo = 0;
                //currentOddNo = 0;
                resultIndex = 0;

                lb = 0;
                ub = 0;
                mid = 0;
                tempResult = 0;

                useOddSeqChunk = true;
                checkedFirstChunk = false;
                //oddListAvailable = false;
                //evenListAvailable = false;

                chunkList.Clear();
                //evenList.Clear();
                //oddList.Clear();

                for (int i = 0; i < treeNO; i++)
                {
                    treeChunkList[i].Clear();
                    treeCLWriteIndex[i] = 0;
                    treeCLReadIndex[i] = 0;
                    treeCLCurrentSeq[i] = 0;
                    
                }

                
            }
        }

        private void listenForClients()
        {
            IPAddress uploadipAddr = IPAddress.Parse(CLIENTIP);
            listenPeer = new TcpListener(uploadipAddr, PeerListenPort);
            listenPeer.Start();

            while (true)
            {
                //listenPeer.Start();
                mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "IP:" + uploadipAddr.ToString() + "\nPort[" + PeerListenPort.ToString() + "]:Listening...\n" });
                TcpClient client = listenPeer.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                //yam:10-10-09
                // bool sendCPort = false;
                //  bool sendD1Port = false;
                //  bool sendD2Port = false;

                try
                {
                    //listenPeer.Start();


                    /*  for (int i = 0; i < max_client; i++)
                      {
                          if (ph.getTreeCListClient(1, i) == null) //ph.getCListClient(i) == null
                          {
                              temp = ph.getTreeCListPort(1, i);//temp = ph.getCListPort(i);
                              Byte[] cMessage = BitConverter.GetBytes(temp);
                              stream.Write(cMessage, 0, cMessage.Length);
                              sendCPort = true;
                              mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Cport:"+temp.ToString()+"\n" });

                              break;
                          }

                      }

                      for (int i = 0; i < max_client; i++)
                      {
                          if (ph.getTreeDListClient(1, i) == null) //ph.getDListClient(i) == null
                          {
                              temp=ph.getTreeDListPort(1,i);//temp = ph.getDListPort(i, 1);
                              Byte[] d1Message = BitConverter.GetBytes(temp);
                              stream.Write(d1Message, 0, d1Message.Length);
                              sendD1Port = true;
                              mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "D1port:" + temp.ToString() + "\n" });

                              break;
                          }
                      }

                      for (int i = 0; i < max_client; i++)
                      {
                          if (ph.getTreeDListClient(2, i) == null) //ph.getDListClient(i) == null
                          {
                              temp = ph.getTreeDListPort(2, i);//temp = ph.getDListPort(i, 2);
                              Byte[] d2Message = BitConverter.GetBytes(temp);
                              stream.Write(d2Message, 0, d2Message.Length);
                              sendD2Port = true;
                              mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "D2port:" + temp.ToString() + "\n" });

                              break;
                          }
                      }
                   
                    
                  if (sendCPort == false || sendD1Port==false || sendD2Port==false)
                  {
                      Byte[] cMessage = BitConverter.GetBytes(0000);   //0000 mean no C port can join
                      stream.Write(cMessage, 0, cMessage.Length);

                      Byte[] dMessage = BitConverter.GetBytes(0000);
                      stream.Write(dMessage, 0, dMessage.Length);     //d1

                      stream.Write(dMessage, 0, dMessage.Length);     //d2
                  }
                  */


                    /* 
                       bool sendCPortState = false;
                       bool sendDPortState = false;
                      
                      for (int i = 1; i <= max_tree; i++)
                     {
                         for (int j = 0; i < max_client; j++)
                         {
                             Byte[] cMessage;
                             Byte[] dMessage;
                             sendCPortState = false;
                             sendDPortState = false;

                             if (ph.getTreeCListClient(i, j) == null)
                                 sendCPortState = true;
                              
                             if (ph.getTreeDListClient(i, j) == null)
                                 sendDPortState = true;
                              

                             if (sendCPortState == true && sendDPortState == true)
                             {
                                 temp = ph.getTreeCListPort(i, j);
                                 cMessage = BitConverter.GetBytes(temp);
                                 stream.Write(cMessage, 0, cMessage.Length);
                                 mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Cport:" + temp.ToString() + "\n" });

                                 temp = ph.getTreeDListPort(i, j);
                                 dMessage = BitConverter.GetBytes(temp);
                                 stream.Write(dMessage, 0, dMessage.Length);
                                 mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Dport:" + temp.ToString() + "\n" });

                                 break;
                             }
                         }


                         if (sendCPortState != true && sendDPortState != true)
                         {
                             Byte[] cMessage = BitConverter.GetBytes(0000);   //0000 mean no C port can join
                             stream.Write(cMessage, 0, cMessage.Length);

                             Byte[] dMessage = BitConverter.GetBytes(0000);
                             stream.Write(dMessage, 0, dMessage.Length); 
                         }
                     }
                     */

                    int total_req_num = 0;
                    int req_tree_num = 0;
                    int tempC_num, tempD_num;

                    Byte[] cMessage;
                    Byte[] dMessage;

                    //client require how many tree
                    byte[] responseMessage = new Byte[4];
                    stream.Read(responseMessage, 0, responseMessage.Length);
                    total_req_num = BitConverter.ToInt16(responseMessage, 0);

                    for (int i = 0; i < total_req_num; i++)
                    {
                        //which tree ,client want to join.
                        bool sendPort = false;
                        stream.Read(responseMessage, 0, responseMessage.Length);
                        req_tree_num = BitConverter.ToInt16(responseMessage, 0);


                        for (int j = 0; j < max_peer; j++)
                        {
                            if (upPorth.getTreeCListClient(req_tree_num, j) == null && upPorth.getTreeDListClient(req_tree_num, j) == null)
                            {
                                tempC_num = upPorth.getTreeCListPort(req_tree_num, j);
                                cMessage = BitConverter.GetBytes(tempC_num);
                                stream.Write(cMessage, 0, cMessage.Length);
                                mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Cport:" + tempC_num.ToString() + "\n" });

                                tempD_num = upPorth.getTreeDListPort(req_tree_num, j);
                                dMessage = BitConverter.GetBytes(tempD_num);
                                stream.Write(dMessage, 0, dMessage.Length);
                                mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Dport:" + tempD_num.ToString() + "\n" });

                                sendPort = true;
                                break;
                            }
                        }

                        if (sendPort != true)
                        {   // required tree number cant join
                            Byte[] cMessage2 = BitConverter.GetBytes(0000);
                            stream.Write(cMessage2, 0, cMessage2.Length);
                            //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Cport:no port" + "\n" });

                            Byte[] dMessage2 = BitConverter.GetBytes(0000);
                            stream.Write(dMessage2, 0, dMessage2.Length);
                            //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Dport:no port" + "\n" });
                        }

                    }

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());

                    mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "One client join fail...\n" });
                    break;
                }
                stream.Dispose();
                client = null;
                //listenPeer.Stop();
                Thread.Sleep(50);

            }//end while loop
            listenPeer.Stop();
        }

        //        public void receiveChunk(TcpClient tcpClient, int treeIndex)
        //        {
        //            TcpClient clientD = tcpClient;
        //            NetworkStream stream = clientD.GetStream();
        //            BinaryFormatter bf = new BinaryFormatter();


        //            try
        //            {
        //               // byte[] responseMessage = new byte[cConfig.ChunkSize];
        //                Chunk streamingChunk;
        //                int responseMessageBytes;

        //                while (serverConnect)
        //                {


        //                        byte[] responseMessage = new byte[cConfig.ChunkSize];
        //                        responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
        //                    //by vinci: send responseBytes directly to peer
        //                        //this.uploadToPeer(responseMessageBytes);

        //                        streamingChunk = (Chunk)ch.byteToChunk(bf, responseMessage);

        //                        if (streamingChunk == null)
        //                        {
        //                            Thread.Sleep(3);
        //                            continue;
        //                        }
        //                        //by yam
        //                        if (treeIndex == 0 && streamingChunk.seq % 2 != 0)
        //                        {
        //                            if (oddList.Count <= cConfig.ChunkCapacity)
        //                                oddList.Add(streamingChunk);
        //                            else
        //                                oddList[oddList_wIndex] = streamingChunk;

        //                            if (oddList_wIndex == cConfig.ChunkCapacity)
        //                                oddList_wIndex = 0;
        //                            else
        //                                oddList_wIndex += 1;

        //                            currentOddNo = streamingChunk.seq;
        ////by vinci: add chunk to odd upload buffer
        //                            if(uploading)
        //                                    upPorth.setOddListChunk(streamingChunk);

        //                        }
        //                        else
        //                        {
        //                            if (evenList.Count <= cConfig.ChunkCapacity)
        //                                evenList.Add(streamingChunk);
        //                            else
        //                                evenList[evenList_wIndex] = streamingChunk;

        //                            if (evenList_wIndex == cConfig.ChunkCapacity)
        //                                evenList_wIndex = 0;
        //                            else
        //                                evenList_wIndex += 1;

        //                            currentEvenNo = streamingChunk.seq;
        ////by vinci: add chunk to even upload buffer
        //                            if(uploading)
        //                                    upPorth.setEvenListChunk(streamingChunk);
        //                        }

        //                       // mainFm.tbWriteStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox1), new object[] { chunkList_wIndex.ToString() + ":" + streamingChunk.seq });
        //                        mainFm.tbWriteStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox1), new object[] { currentOddNo.ToString() + ":" + currentEvenNo.ToString() });

        //                        Thread.Sleep(3);

        //                } //end while loop
        //                stream.Close();
        //                clientD.Close();
        //            }
        //            catch(Exception ex)
        //            {
        //                stream.Close();
        //                clientD.Close();
        //                MessageBox.Show(ex.ToString());
        //            }

        //        }

        //private void updateChunkList()
        //{
        //    defineFistChunk();

        //    while (true)
        //    {
        //        if ((tempSeq) % 2 != 0)
        //        {
        //            if (tempSeq <= currentOddNo)
        //            {
        //                resultIndex = searchChunk(oddList, oddList_rIndex, oddList_wIndex, tempSeq);

        //                if (resultIndex != -1)
        //                    addToChunkList(oddList, ref oddList_rIndex, resultIndex);
        //               // else
        //                   // mainFm.tbWriteStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox1), new object[] { tempSeq.ToString() });

        //                incrementTempSeq();
        //            }
        //            else
        //                Thread.Sleep(40);

        //        }
        //        else
        //        {
        //            if (tempSeq <= currentEvenNo)
        //            {
        //                resultIndex = searchChunk(evenList, evenList_rIndex, evenList_wIndex, tempSeq);

        //                if (resultIndex != -1)
        //                    addToChunkList(evenList, ref evenList_rIndex, resultIndex);
        //                //else
        //                  //  mainFm.tbReadStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { tempSeq.ToString() });

        //                incrementTempSeq();
        //            }
        //            else
        //                Thread.Sleep(40);

        //        }
        //        Thread.Sleep(2);
        //    }
        //}

        //private void defineFistChunk()
        //{
        //    while (true)
        //    {
        //        if (oddList.Count > 10 && evenList.Count > 10)
        //        {
        //            if (oddList[0].seq < evenList[0].seq)
        //                tempSeq = oddList[0].seq;
        //            else
        //                tempSeq = evenList[0].seq;
        //            break;
        //        }
        //        Thread.Sleep(2);
        //    }
        //}

        //private void incrementTempSeq()
        //{
        //    if (tempSeq == 2147483647)
        //        tempSeq = 1;
        //    else
        //        tempSeq += 1;
        //}

        //private void addToChunkList(List<Chunk> list, ref int rIndex, int targetIndex)
        //{
        //    if (chunkList.Count <= cConfig.ChunkCapacity)
        //        chunkList.Add(list[targetIndex]);
        //    else
        //        chunkList[chunkList_wIndex] = list[targetIndex];

        //    if (chunkList_wIndex == cConfig.ChunkCapacity)
        //        chunkList_wIndex = 0;
        //    else
        //        chunkList_wIndex += 1;

        //    rIndex = targetIndex;
        //}

    }// end class     
}// end namespace
