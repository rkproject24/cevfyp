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

namespace Client
{
    class ClientHandler
    {
        //===============upload variable=============
        string CLIENTIP;// = "127.0.0.1";
       
        //static int PeerListenPort = 1100;
        int PeerListenPort;
        int max_peer;
        bool uploading = false; //indicate whether upload thread is started
        TcpListener listenPeer;
        UploadPortHandler upPorth;
        Thread listenerThread;

        //===================================

        int treeNO = 0;
        int chunkList_wIndex = 0;  //write index
        int chunkList_rIndex = 0;  //read index
        int tempSeq=0;
        int lb, ub, mid, tempResult;
        int virtualServerPort = 0;   //virtual server broadcast port number

        bool checkToBoardcast = false;
        bool serverConnect = false;
        bool vlcConnect = false;
        bool checkClose = false;

       
        string trackerIp = null;

        VlcHandler vlc;
        ChunkHandler ch;

        IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        List<Chunk> chunkList;
        List<TcpClient> ClientC;
        List<TcpClient> ClientD;
        List<List<Chunk>> treeChunkList;

        int[] treeCLWriteIndex;
        int[] treeCLReadIndex;
        int[] treeCLCurrentSeq;
        int[] treeReconnectState;
      

        List<Thread> receiveChunkThread;
        List<Thread> receiveControlThread;
        Thread broadcastVlcStreamingThread;
        Thread updateChunkListThread;

        TcpListener server;
        NetworkStream localvlcstream=null;

        private ClientForm mainFm;
        public delegate void UpdateTextCallback(string message);
        private PeerHandler peerh;

        ClientConfig cConfig = new ClientConfig();

        public ClientHandler(ClientForm mainFm)
        {
            this.mainFm = mainFm;
            vlc = new VlcHandler();
            ch = new ChunkHandler();

            cConfig.load("C:\\ClientConfig");
            chunkList = new List<Chunk>(cConfig.ChunkCapacity);

            mainFm.tbhostIP.Text = TcpApps.LocalIPAddress();
            mainFm.tbServerIp.Text = cConfig.Trackerip;

            string[] xmlList = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");
            foreach (string xmlfile in xmlList)
            {
                File.Delete(xmlfile);
            }
           
        }

        public void initialize()
        {
            treeChunkList = new List<List<Chunk>>(treeNO);
            treeCLWriteIndex = new int[treeNO];
            treeCLReadIndex = new int[treeNO];
            treeCLCurrentSeq = new int[treeNO];
            treeReconnectState = new int[treeNO];

            receiveChunkThread = new List<Thread>(treeNO);
            receiveControlThread=new List<Thread>(treeNO);

            ClientC = new List<TcpClient>(treeNO);
            ClientD = new List<TcpClient>(treeNO);

            for (int i = 0; i < treeNO; i++)
            {
                ClientC.Add(null);
                ClientD.Add(null);
            }


        }

        public string getSelfID(int tree_index)
        {
            return peerh.Selfid[tree_index];
        }


        public void getMute()
        {
            int checkMute = vlc.getMute();
            if (checkMute == 0)
                vlc.setMute(1);
            else
                vlc.setMute(0);

        }

        public string establishConnect(string tackerIp)
        {
            mainFm.tbWriteStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox1), new object[] { "" });


            string response = "";

            //connect tracker
            response = connectToTracker(tackerIp);

            /*if (response == "OK")
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
            */

            if (response == "OK")
            {
                bool check = false;
                PeerListenPort = TcpApps.RanPort(cConfig.LisPort, cConfig.LisPortup);

                for (int i = 0; i < treeNO; i++)
                {
                    check = connectToSources(i, false);

                    if (!check)
                    {
                        response = "One of Data Ports cannot join!!!!";

                        //unreg. already join tree
                        for (int k = 0; k <= i; k++)
                        {
                            if (ClientC[k] != null)
                                sendExit(k);
                        }

                        break;
                    }
                }

                if (check)
                {
                    virtualServerPort = peerh.Cport11 + cConfig.VlcPortBase;
                    serverConnect = true;
                    checkClose = false;

                    startUpload();

                    response = "OK3";
                }
            }
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

        //private string connectToPeer()
        //{
        //    //if(peerh.PeerIp.Equals("NOPEER"))
        //    //    return "NO Peer available in Peer list!";
        //    while(true)
        //    {
        //        if (peerh.connectPeer())
        //        {
        //            return "OK2";
        //        }
        //        Thread.Sleep(50);
        //    }
        //    //else
        //    //{
        //    //    return "Peer Unreachable!";
        //    //}
        //}

        private bool connectToSources(int tree_index,bool reconnectUse)
        {
            bool conPeer = false;
          
                conPeer = peerh.connectPeers(tree_index+1);

                if (conPeer)
                {

                        ClientC[tree_index]=peerh.getControlConnect(tree_index);
                        ClientD[tree_index]=peerh.getDataConnect(tree_index);
                   

                    if (ClientC[tree_index] == null || ClientD[tree_index] == null)
                        return false;

                    if (!reconnectUse)
                        peerh.registerToTracker(tree_index, PeerListenPort, peerh.JoinPeer[tree_index].Layer.ToString()); //by vinci: register To Tree in Tracker

                    

                  
                     //---reconnection cause cross-thread problem in here.

                        //mainFm.Text = "Client:";

                        //for (int i = 0; i < peerh.Selfid.Length; i++)
                        //{
                        //    mainFm.Text += peerh.Selfid[i] + ",";
                        //}

                    //mainFm.Text += peerh.Selfid[tree_index] + ",";

                    return true;
                }
               
            return false;
        
        

        }

        //private string connectToSource()
        //{
        //    virtualServerPort = peerh.Cport11+ cConfig.VlcPortBase;
        //    serverConnect = true;
        //    checkClose = false;

        //    PeerListenPort = TcpApps.RanPort(1100, 1120);
  
        //    try
        //    {
        //        for (int i = 0; i < treeNO; i++)
        //        {
        //            ClientC.Add(peerh.getControlConnect(i));
        //            ClientD.Add(peerh.getDataConnect(i));

        //            //peerh.registerToTracker(i, PeerListenPort, peerh.PeerIp.Layer.ToString()); //by vinci: register To Tree in Tracker
        //            peerh.registerToTracker(i, PeerListenPort, peerh.JoinPeer[i].Layer.ToString()); //by vinci: register To Tree in Tracker
        //        }

        //        mainFm.Text = "Client:";
        //        for (int i = 0; i < peerh.Selfid.Length; i++)
        //        {
        //            mainFm.Text += peerh.Selfid[i]+ ",";
        //        }

        //        //automatic start listen
        //        startUpload();

        //        return "OK3";
        //    }
        //    catch
        //    {
        //        return "One of Data Ports cannot join!!!!";
        //    }

        //}

       /* private bool connectToSource2(int tree_index)
        {
            serverConnect = true;
            checkClose = false;
            int i = tree_index;

            try
            {
                ClientC[tree_index] = peerh.getControlConnect(i);
                ClientD[tree_index] = peerh.getDataConnect(i);

                mainFm.Text = "Client:";
                for (int j = 0; j < peerh.Selfid.Length; j++)
                {
                    mainFm.Text += peerh.Selfid[j] + ",";
                }

                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("connectSrc Probelm\n"+ex.ToString());
                return false;
            }

        }
        */

        private void reconnection(int tree_index)
        {
            bool prlist = false;
            //bool conPeer = false;
            bool conSource = false;
            bool changeParent = false;

            while(!(prlist && conSource && changeParent))
            {
               
                //prlist = peerh.downloadPeerlist2(tree_index);
                //conPeer = peerh.connectPeer2(tree_index + 1);
              // conSource = connectToSource2(tree_index);

                prlist = peerh.downloadPeerlist(tree_index, true);
                conSource = connectToSources(tree_index,true);
                changeParent = peerh.changeParent(tree_index);//register to tracker for new parent
                Thread.Sleep(10);
            }
            //treeReconnectState[tree_index] = 0;
            

        }


        public void startThread()
        {
            tempSeq = 0;

            for (int i = 0; i < treeNO; i++)
            {
                List<Chunk> chunkLists = new List<Chunk>(cConfig.ChunkCapacity);
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


        private void receiveTreeChunk(int tree_index)
        {
            TcpClient clientD = null;
            NetworkStream stream = null;
            BinaryFormatter bf = new BinaryFormatter();
            
            byte[] responseMessage = new byte[cConfig.ChunkSize];
            Chunk streamingChunk;
            int responseMessageBytes;

            while (serverConnect)
            {
                try
                {
                    
                    if (ClientD[tree_index] != null && treeReconnectState[tree_index]==0)
                    {
                        clientD = ClientD[tree_index];
                        stream = clientD.GetStream();
                    }
                    else
                    {
                        upPorth.setTreeCLState(tree_index, 0);

                        if (tree_index == 0)
                            mainFm.textBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { "Reconnecting.." });
                        if (tree_index == 1)
                            mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { "Reconnecting.." });
                        if (tree_index == 2)
                            mainFm.textBox3.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { "Reconnecting.." });

                        Thread.Sleep(20);
                        continue;
                    }


                    while (true)
                    {
                        if (ClientD[tree_index] == null)
                        {
                            stream.Close();
                            clientD.Close();

                            upPorth.setTreeCLState(tree_index, 0);

                            if (tree_index == 0)
                                mainFm.textBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { "Reconnecting~" });
                            if (tree_index == 1)
                                mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { "Reconnecting~" });
                            if (tree_index == 2)
                                mainFm.textBox3.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { "Reconnecting~" });

                            mainFm.rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] D exit~\n" });

                            break;
                        }

                        stream = ClientD[tree_index].GetStream();
                        responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
                        string responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);
                        
                        if (responseString == "Wait")
                        {
                            upPorth.setTreeCLState(tree_index, 0);

                            if (tree_index == 0)
                                mainFm.textBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { "Wait~" });
                            if (tree_index == 1)
                                mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { "Wait~" });
                            if (tree_index == 2)
                                mainFm.textBox3.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { "Wait~" });

                            Thread.Sleep(10);
                            continue;
                        }

                        streamingChunk = (Chunk)ch.byteToChunk(bf, responseMessage);

                        if (streamingChunk == null)
                        {
                            upPorth.setTreeCLState(tree_index, 0);
                            Thread.Sleep(10);
                            continue;
                        }

                        upPorth.setTreeCLState(tree_index, 1);

                        //by vinci: send responseBytes directly to peer
                        //this.uploadToPeer(responseMessageBytes);
                        if (uploading)
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

                        if (tree_index == 0)
                            mainFm.textBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { treeCLCurrentSeq[tree_index].ToString() });
                        if (tree_index == 1)
                            mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { treeCLCurrentSeq[tree_index].ToString() });
                        if (tree_index == 2)
                            mainFm.textBox3.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { treeCLCurrentSeq[tree_index].ToString() });
                       
                    }
                }
                catch (Exception ex)
                {
                   // MessageBox.Show(ex.ToString());

                    if (stream != null)
                        stream.Close();
                    if (clientD != null)
                        clientD.Close();

                    if(!checkClose)
                    treeReconnectState[tree_index] = 1;
                    
                    upPorth.setTreeCLState(tree_index, 0);

                    if (checkClose)
                    {
                        if (tree_index == 0)
                            mainFm.textBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox1), new object[] { "close ok" });
                        if (tree_index == 1)
                            mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox2), new object[] { "close ok" });
                        if (tree_index == 2)
                            mainFm.textBox3.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTBox3), new object[] { "close ok" });
                    }

                    mainFm.rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] D exit\n" });
                }
            } //end while loop

        }

        private void receiveTreeControl(int tree_index)
        {

            TcpClient clientC = null;
            NetworkStream stream=null;
            byte[] sMessage = new byte[4];
            
            while (true)
            {
                try
                {
                    if (ClientC[tree_index] != null)
                    {
                        clientC = ClientC[tree_index];
                        stream = clientC.GetStream();
                    }
                   // else
                   // {
                    //    upPorth.setTreeCLState(tree_index, 0);
                     //   if (ClientD[tree_index] == null && ClientC[tree_index] == null && treeReconnectState[tree_index] == 1)
                     //       reconnection(tree_index);
                      //  Thread.Sleep(20);
                      //  continue;
                   // }


                    while (true)
                    {
                       if (treeReconnectState[tree_index]==1 && !checkClose)
                        {
                            sMessage = System.Text.Encoding.ASCII.GetBytes("Exit");
                            stream.Write(sMessage, 0, sMessage.Length);

                            stream.Close();
                            clientC.Close();

                            ClientD[tree_index] = null;
                            ClientC[tree_index] = null;

                            reconnection(tree_index);
                            treeReconnectState[tree_index] = 0;

                            break;
                        }
                        
                        if(treeReconnectState[tree_index]==0 && !checkClose)
                        {
                            stream = ClientC[tree_index].GetStream();
                            sMessage = System.Text.Encoding.ASCII.GetBytes("Wait");
                            stream.Write(sMessage, 0, sMessage.Length);

                            Thread.Sleep(20);
                            continue;
                        }

                        Thread.Sleep(20);
                    }
                }
                catch
                {
                    if (stream != null)
                        stream.Close();
                    if (clientC != null)
                        clientC.Close();

                    if (!checkClose)
                    {
                      
                        if (ClientD[tree_index] != null && ClientC[tree_index] != null)
                        {
                            ClientD[tree_index] = null;
                            ClientC[tree_index] = null;
                        }

                        treeReconnectState[tree_index] = 1;
                        reconnection(tree_index);
                        treeReconnectState[tree_index] = 0;
                    }

                    upPorth.setTreeCLState(tree_index, 0);
                    mainFm.rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] C exit\n" });

                }

                Thread.Sleep(10);
            }
         }

        private void sendExit(int tree_index)
        {
            TcpClient clientC=null;
            NetworkStream stream=null;
            byte[] sMessage = new byte[4];

            try
            {
                clientC = ClientC[tree_index];
                stream = clientC.GetStream();

                sMessage = System.Text.Encoding.ASCII.GetBytes("Exit");
                stream.Write(sMessage, 0, sMessage.Length);

               

            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                mainFm.rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { "T[" + tree_index + "] Send exit err\n" });
                
               
            }

            if (stream != null)
                stream.Close();
            if (clientC != null)
                clientC.Close();

            ClientD[tree_index] = null;
            ClientC[tree_index] = null;

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

                          //   mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "tree:" + i+ " : "+ treeChunkList[i][r_index].seq + "\n" });

                        }
                        if (tempSeq == 2147483647)
                            tempSeq = 1;
                        else
                            tempSeq += 1; 
                    }

                   if (check_num == i && ClientD[i] == null)
                   {

                       if (tempSeq == 2147483647)
                           tempSeq = 1;
                       else
                           tempSeq += 1; 

                   }
                }
                Thread.Sleep(10);
            }
        }
       
        private void broadcastVlcStreaming()
        {
            TcpClient client = null;

            try
            {
                server = new TcpListener(localAddr, virtualServerPort);
                server.Start();

                client = server.AcceptTcpClient();
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
            }
            catch(Exception ex)
            {
                if (localvlcstream != null)
                    localvlcstream.Close();

                if (client != null)
                    client.Close();

                mainFm.rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbDownload), new object[] { ex.ToString() });
                
            }
           
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
                    //if (mainFm.tbStatus.Text != "Buffering...")
                      //  mainFm.tbStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox3), new object[] { "Buffering..." });
                }
            }
            else if (indexdiff > cConfig.StartBuf)
            {
                //if (mainFm.tbStatus.Text != "Playing")
                  //  mainFm.tbStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox3), new object[] { "Playing" });
                checkToBoardcast = true;
            }
        }       

        public void closeAllThread()
        {
            if (serverConnect == true)
            {
                checkClose = true;
                mainFm.tbWriteStatus.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox1), new object[] { "Closing Thread.." });

                //stop upload
                listenPeer.Stop();
                listenerThread.Abort();
                upPorth.closeCDPortThread();

                serverConnect = false;
                vlcConnect = false;
                uploading = false;

                server.Stop();
                //broadcastVlcStreamingThread.Join(500);
                vlc.stop();
                broadcastVlcStreamingThread.Abort();

                updateChunkListThread.Abort();
                for (int i = 0; i < treeNO; i++)
                {
                 //   recciveChunkThread[i].Join(1000);
                    //receiveControlThread[i].Join(1000);

                    receiveChunkThread[i].Abort();
                    sendExit(i);
                    receiveControlThread[i].Abort();
                }
    
                //server.Stop();
                receiveChunkThread.Clear();
                receiveControlThread.Clear();
              //  ClientD.Clear();
                //ClientC.Clear();
                chunkList_wIndex = 0;
                chunkList_rIndex = 0;
                virtualServerPort = 0;
               
                //lb = 0;
                //ub = 0;
                //mid = 0;
                //tempResult = 0;

                chunkList.Clear();
                for (int i = 0; i < treeNO; i++)
                {
                    treeChunkList[i].Clear();
                    treeCLWriteIndex[i] = 0;
                    treeCLReadIndex[i] = 0;
                    treeCLCurrentSeq[i] = 0;
                    treeReconnectState[i] = 0;
                }

                //Thread.Sleep(500);
            }

        }

        private void listenForClients()
        {
            TcpClient client = null;
            NetworkStream stream = null;
            int total_req_num=0;
            int req_tree_num = 0;
            IPAddress uploadipAddr = IPAddress.Parse(CLIENTIP);

            try
            {
                listenPeer = new TcpListener(uploadipAddr, PeerListenPort);
                listenPeer.Start();
            }
            catch (Exception ex)
            {
                mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { ex.ToString()});
            
            }

            mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "IP:" + uploadipAddr.ToString() + "\nPort[" + PeerListenPort.ToString() + "]:Listening...\n" });
            
            while (true)
            {
                try
                {
                    client = listenPeer.AcceptTcpClient();
                    stream = client.GetStream();

                    total_req_num = 0;
                    req_tree_num = 0;
                    int tempC_num, tempD_num;

                    byte[] cMessage;
                    byte[] dMessage;

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
                                mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "T[" + (req_tree_num - 1) + "] Cport:" + tempC_num.ToString() + " " });

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
                            byte[] cMessage2 = BitConverter.GetBytes(0000);
                            stream.Write(cMessage2, 0, cMessage2.Length);
                            //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Cport:no port" + "\n" });

                            byte[] dMessage2 = BitConverter.GetBytes(0000);
                            stream.Write(dMessage2, 0, dMessage2.Length);
                            //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Dport:no port" + "\n" });
                        }

                    }

                    //if (stream != null)
                    //    stream.Close();
                    //if (client != null)
                    //    client.Close();

                    //stream=null;
                    //client=null;

                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show(ex.ToString());


                    if (!checkClose)
                    {   //Joining client disconnection cause this case run.
                        mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "One client join fail...\n" });
                    }
                    else
                    {
                        //confirm the stream in write state,and then send the exit to peer
                        if (total_req_num != 0 && req_tree_num != 0 && stream!=null && client!=null)
                        {
                            byte[] cMessage3 = BitConverter.GetBytes(0001);    
                            stream.Write(cMessage3, 0, cMessage3.Length);
                        }

                        mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { "Listen port close\n" });
                    }

                    //if (stream != null)
                    //    stream.Close();
                    //if (client != null)
                    //    client.Close();
                    
                    //stream=null;
                    //client=null;
                }

                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();

                stream = null;
                client = null;

                Thread.Sleep(50);
            }
        }

       

    }// end class     
}// end namespace
