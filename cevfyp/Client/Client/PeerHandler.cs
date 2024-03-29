﻿using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Xml;
using System.Xml.Serialization;

using System.Net;
using System.Net.Sockets;
using ClassLibrary;
using System.IO;
using System.Threading;

namespace Client
{
    class PeerHandler
    {
        //static int TRACKER_PORT = 1100;  //server listen port
        string folder = "";
        string Peerlist_name = "PeerInfoT";
        int TREE_NO;
        static int GET_PORT_TIMEOUT = 10000;
        static int TOTAL_SELECT_PEER_TIME = 10;
        static bool connectLocal = true; //connect to local ip address

        public int RANDOM_PEER = 0;
        public int NO_NULLCHUNK = 1;

        //static int TrackerSLPort = 1500;

        private string trackIp;
        private PeerNode[] joinPeers;
        private ClientForm clientFrm;
        //private string[] selfid;//= new string[TREE_NO];
        private string selfid;
        private bool chunkNullUpdated;
        private bool savingchunkNull = false;
        private bool[] fastConnect;
        private int channelMaxID;

        public int currentCh;

        //int Cport = 0;             
        ClientConfig cConfig = new ClientConfig();
        //ServerConfig sConfig = new ServerConfig();
        PeerInfoAccessor treeAccessor;

        int peerListenPort = 0;

        /*public int Cport11
        {
            get { return Cport; }
            set { Cport = value; }
        }
         */
        public string TrackIp
        {
            get { return trackIp; }
            set { trackIp = value; }
        }
        public int ChannelMaxID
        {
            get { return channelMaxID; }
            set { channelMaxID = value; }
        }
        public string Selfid
        {
            get { return selfid; }
            set { selfid = value; }
        }
        public PeerNode[] JoinPeer
        {
            get { return joinPeers; }
            set { joinPeers = value; }
        }
        public int PeerListenPort
        {
            get { return peerListenPort; }
            set { peerListenPort = value; }
        }
        public bool ChunkNullUpdated
        {
            get { return chunkNullUpdated; }
            set { chunkNullUpdated = value; }
        }

        public bool[] FastConnect
        {
            get { return fastConnect; }
            set { fastConnect = value; }
        }

        //int D1port = 0;             //video data port number
        int[] Dport;// = new int[TREE_NO];
        int[] Cport;// = new int[TREE_NO];

        public int Cport11 = 0;
        public delegate void UpdateTextCallback(string message);

        public PeerHandler(string trackerIp, ClientForm clientFrm)
        {

            this.clientFrm = clientFrm;
            cConfig.load("C:\\ClientConfig");

            TREE_NO = 0;
            this.trackIp = trackerIp;

            //selfid = new string[1];
        }

        public void treeInitial()
        {
            ////create directory for each client program
            //if (Directory.Exists(selfid[0]))
            //    Directory.Delete(selfid[0], true);
            //Directory.CreateDirectory(selfid[0]);
            //Peerlist_name = selfid[0] + "\\" + Peerlist_name;

            //selfid = new string[TREE_NO];
            Dport = new int[TREE_NO];
            Cport = new int[TREE_NO];
            fastConnect = new bool[TREE_NO];

            for (int i = 0; i < TREE_NO; i++)
            {
                Dport[i] = 0;
                Cport[i] = 0;
            }
        }
        public bool getSelfid()
        {
            TcpClient trackerTcpClient = null;
            NetworkStream trackerStream = null;
            try
            {
                trackerTcpClient = new TcpClient();
                IAsyncResult MyResult = trackerTcpClient.BeginConnect(trackIp, cConfig.TrackerPort, null, null);
                MyResult.AsyncWaitHandle.WaitOne(500, true);
                if (!MyResult.IsCompleted)
                {
                    trackerTcpClient.Close();
                    return false;
                }
                else if (trackerTcpClient.Connected == true)
                {
                    trackerStream = trackerTcpClient.GetStream();

                    trackerStream.WriteTimeout = 800;
                    trackerStream.ReadTimeout = 800;
                    //define client type
                    byte[] request = StrToByteArray("<selfidReq>");
                    trackerStream.Write(request, 0, request.Length);

                    byte[] channelbyte = BitConverter.GetBytes(currentCh);
                    trackerStream.Write(channelbyte, 0, channelbyte.Length);

                    byte[] responsePeerMsg = new byte[4];
                    trackerStream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    this.selfid = BitConverter.ToInt32(responsePeerMsg, 0)+"";
                    trackerStream.Close();
                    trackerTcpClient.Close();
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
        public int getChannelList()
        {
            TcpClient trackerTcpClient = null;
            NetworkStream trackerStream = null;
            try
            {
                trackerTcpClient = new TcpClient();
                IAsyncResult MyResult = trackerTcpClient.BeginConnect(trackIp, cConfig.TrackerPort, null, null);
                MyResult.AsyncWaitHandle.WaitOne(500, true);
                if (!MyResult.IsCompleted)
                {
                    trackerTcpClient.Close();
                    return -1;
                }
                else if (trackerTcpClient.Connected == true)
                {
                    trackerStream = trackerTcpClient.GetStream();

                    trackerStream.WriteTimeout = 800;
                    trackerStream.ReadTimeout = 800;
                    //define client type
                    byte[] request = StrToByteArray("<channList>");
                    trackerStream.Write(request, 0, request.Length);

                    byte[] responsePeerMsg = new byte[4];
                    trackerStream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    channelMaxID = BitConverter.ToInt32(responsePeerMsg, 0);
                    trackerStream.Close();
                    trackerTcpClient.Close();
                    if (channelMaxID == 0)
                        return -1;
                }
                else
                    return -1;
            }
            catch
            {
                return -1;
            }
            return channelMaxID;
        }

        //by Vinci: coonect to Tracker for peer ip 
        public int findTracker()
        {
            TcpClient trackerTcpClient = null;
            NetworkStream trackerStream = null;
            try
            {
                //trackerTcpClient = new TcpClient(trackIp, cConfig.TrackerPort);

                trackerTcpClient = new TcpClient();
                IAsyncResult MyResult = trackerTcpClient.BeginConnect(trackIp, cConfig.TrackerPort, null, null);
                MyResult.AsyncWaitHandle.WaitOne(1000, true);
                if (!MyResult.IsCompleted)
                {
                    trackerTcpClient.Close();
                    return -1; 
                }

                trackerStream = trackerTcpClient.GetStream();

                //define client type
                byte[] request = StrToByteArray("<treeSizes>");
                trackerStream.Write(request, 0, request.Length);

                request  = BitConverter.GetBytes(currentCh);
                trackerStream.Write(request, 0, request.Length);

                byte[] responsePeerMsg = new byte[4];
                trackerStream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                this.TREE_NO = BitConverter.ToInt32(responsePeerMsg, 0);
            }
            catch
            {
                return -1;
            }

            treeInitial();
            joinPeers = new PeerNode[TREE_NO]; //define size of JoinpeerNode array
            chunkNullUpdated = false;


            return TREE_NO;
        }

        public bool downloadPeerlist( int tree, bool reConnectUse)
        {
            TcpClient trackerTcpClient = null;
            NetworkStream trackerStream = null;

            try
            {
                trackerTcpClient = new TcpClient(trackIp, cConfig.TrackerPort);
                trackerStream = trackerTcpClient.GetStream();
                trackerStream.ReadTimeout = 3000;
                //define client type
                Byte[] clienttype = StrToByteArray("<clientReq>");
                trackerStream.Write(clienttype, 0, clienttype.Length);

                byte[] channelbyte = BitConverter.GetBytes(currentCh);
                trackerStream.Write(channelbyte, 0, channelbyte.Length);

                byte[] treeNo = BitConverter.GetBytes(tree);
                trackerStream.Write(treeNo, 0, treeNo.Length);

                ////port for receive peer list
                //int recPeerListPort = TcpApps.RanPort(1801, 1900);
                //byte[] PeerListPortbyte = BitConverter.GetBytes(recPeerListPort);
                //trackerStream.Write(PeerListPortbyte, 0, PeerListPortbyte.Length);

                //trackerTcpClient.Close();
                //trackerStream.Close();

                TcpClient tracker = null;
                NetworkStream peerListstream = null;
                IPAddress receiveAddr = IPAddress.Parse(TcpApps.LocalIPAddress());
                //TcpApps.RanPort()
                try
                {
                    int recPeerListPort;
                    TcpListener listenTracker;
                    while (true)
                    {
                        try
                        {
                            recPeerListPort = TcpApps.RanPort(1801, 1900);
                            // mainFm.rtbupload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { PeerListenPort.ToString() +"\n"});
                            
                            listenTracker = new TcpListener(receiveAddr, recPeerListPort);
                            listenTracker.Start();
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(10);
                        }
                    }

                    //port for receive peer list                   
                    byte[] PeerListPortbyte = BitConverter.GetBytes(recPeerListPort);
                    trackerStream.Write(PeerListPortbyte, 0, PeerListPortbyte.Length);
                    trackerTcpClient.Close();
                    trackerStream.Close();

                    tracker = listenTracker.AcceptTcpClient();
                    tracker.ReceiveTimeout = 3000;
                    peerListstream = tracker.GetStream();

                    byte[] reconnect;
                    byte[] responsePeerMsg;
                    if (reConnectUse)
                    {
                        reconnect = BitConverter.GetBytes(true);
                        peerListstream.Write(reconnect, 0, reconnect.Length);
                        //By Vinci: Reconnect=====================================================
                        //Byte[] sendbyte = BitConverter.GetBytes(Int32.Parse(Selfid[tree]));
                        Byte[] sendbyte = BitConverter.GetBytes(Int32.Parse(Selfid));

                        //connectTrackerStream.Write(sendbyte, 0, sendbyte.Length);

                        //byte[] MsgLength = BitConverter.GetBytes(Selfid[tree].Length);
                        //peerListstream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                        peerListstream.Write(sendbyte, 0, sendbyte.Length);

                        //=========================================================================
                    }
                    else
                    {
                        reconnect = BitConverter.GetBytes(false);
                        peerListstream.Write(reconnect, 0, reconnect.Length);

                        //responsePeerMsg = new byte[4];
                        //peerListstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        //this.selfid[tree] = BitConverter.ToInt32(responsePeerMsg, 0).ToString();
                    }

                    if (folder.Equals(""))
                    {
                        folder = "CH" + this.currentCh + "_" + this.selfid;//this.selfid[tree];
                        if (Directory.Exists(folder))
                            Directory.Delete(folder, true);
                        Directory.CreateDirectory(folder);
                    }
                    string PeerFileName = folder + "\\CH" + currentCh + Peerlist_name + tree + ".xml";

                    responsePeerMsg = new byte[4];
                    peerListstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                    int xmlsize = BitConverter.ToInt32(responsePeerMsg, 0);
                    
                    //solve continuous read problem
                    Thread.Sleep(10);

                    byte[] responsePeerMsg2 = new byte[xmlsize];
                    int readSize=0;
                    do
                    {
                        readSize += peerListstream.Read(responsePeerMsg2, readSize, responsePeerMsg2.Length-readSize);
                    }
                    while (readSize < xmlsize);

                    string xmlContent = ByteArrayToString(responsePeerMsg2);
                    Console.WriteLine("XML size:" + xmlsize);
                    Console.WriteLine(xmlContent.Substring(xmlContent.LastIndexOf('<')));
                    // System.Windows.Forms.MessageBox.Show(xmlContent);

                    //string[] xmlTrees = xmlContent.Split('@');

                    //if (reConnectUse)
                    //{
                        if (File.Exists(PeerFileName))
                            File.Delete(PeerFileName);
                    //}

                    // Specify file, instructions, and privelegdes
                    FileStream file = new FileStream(PeerFileName, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(file);
                    sw.Write(xmlContent);
                    sw.Close();
                    file.Close();

                    peerListstream.Close();
                    tracker.Close();
                }
                catch (Exception ex)
                {
                    //((LoggerFrm)mainFm.uploadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRtbUpload), new object[] { ex.ToString()});

                    if (peerListstream != null)
                        peerListstream.Close();
                    if (tracker != null)
                        tracker.Close();

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show("Tree:" + tree + "\n" + ex.ToString());

                if (trackerStream != null)
                    trackerStream.Close();
                if (trackerTcpClient != null)
                    trackerTcpClient.Close();


                return false;

            }
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }
        public static string ByteArrayToString(byte[] bytes)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }


        public bool connectPeers(int tree_num, bool reconnectUse)  //connect to Peer to get the port no of Cport Dport
        {
            TcpClient connectServerClient = null;
            NetworkStream connectServerStream = null;
            int i = tree_num;
            int total_select_round = 0;
            bool joined = false;
            PeerNode conNode = null;
            try
            {
                while (!joined)
                {
                    if (total_select_round > TOTAL_SELECT_PEER_TIME)
                        return false;               //or down peer list again 

                    //select a peer to connect
                    if (reconnectUse)
                        conNode = selectPeer(i - 1, true, NO_NULLCHUNK);
                    else
                        conNode = selectPeer(i - 1, true, RANDOM_PEER);

                    if (conNode == null)
                    {
                       // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "conNode=null T[" + (i - 1) + "] \n" });
                        return false;
                    }

                    //**************** connection of loacl address*****************
                    if (conNode.Ip == TcpApps.LocalIPAddress() && !connectLocal)
                    {
                       // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "selfip hit T[" + (i - 1) + "] \n" });
                        removePeer(conNode, i - 1);
                       conNode = null;
                        Thread.Sleep(20);
                        continue;
                    }

                    joinPeers[i - 1] = conNode;

                    connectServerClient = new TcpClient();
                    IAsyncResult MyResult = connectServerClient.BeginConnect(joinPeers[i - 1].Ip, joinPeers[i - 1].ListenPort, null, null);
                    MyResult.AsyncWaitHandle.WaitOne(500, true);
                    if (!MyResult.IsCompleted)
                    {

                        if (connectServerClient != null)
                            connectServerClient.Close();

                        removePeer(conNode, i - 1);
                        //if (conNode != null)
                        //    ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "SelectPeer " + conNode.Id + " remove T:" + (i - 1) + "**\n" });

                        return false;

                    }
                    else if (connectServerClient.Connected == true)
                    {
                        connectServerStream = connectServerClient.GetStream();
                        connectServerStream.ReadTimeout = GET_PORT_TIMEOUT;

                        byte[] reqtype = StrToByteArray("treesReq");
                        connectServerStream.Write(reqtype, 0, reqtype.Length);

                        //how many tree req.
                        byte[] message = BitConverter.GetBytes(1);
                        connectServerStream.Write(message, 0, message.Length);

                        // getTreePort(connectServerStream, i);

                        //which tree
                        // byte[] amessage = BitConverter.GetBytes(i);
                        //connectServerStream.Write(amessage, 0, amessage.Length);

                        byte[] responseMessage = new byte[4];

                        connectServerStream.Read(responseMessage, 0, responseMessage.Length);
                        Cport[i - 1] = BitConverter.ToInt16(responseMessage, 0);

                        connectServerStream.Read(responseMessage, 0, responseMessage.Length);
                        Dport[i - 1] = BitConverter.ToInt16(responseMessage, 0);

                        //check whether there is available port in target peer

                        if (Cport[i - 1] != 0 && Dport[i - 1] != 0)// && Cport[i - 1] != 1 && Dport[i - 1] != 1)
                            joined = true;
                        else
                        {
                            // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "SelectPeer " + conNode.Id + " remove T:" + (i - 1) + "\n" });

                            removePeer(conNode, i - 1);
                           // if (conNode != null)
                               // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "SelectPeer " + conNode.Id + " remove T:" + (i - 1) + " ~\n" });
                            conNode = null;
                        }

                        Cport11 = Cport[i - 1];

                        total_select_round += 1;

                        connectServerStream.Close();
                        connectServerClient.Close();

                    }
                    //connectServerClient = new TcpClient(joinPeers[i - 1].Ip, joinPeers[i - 1].ListenPort);
                    //connectServerStream = connectServerClient.GetStream();
                    //connectServerStream.ReadTimeout = GET_PORT_TIMEOUT;

                    //byte[] reqtype = StrToByteArray("treesReq");
                    ////byte[] reqtype = BitConverter.GetBytes(1111);
                    //connectServerStream.Write(reqtype, 0, reqtype.Length);

                    ////how many tree req.
                    //byte[] message = BitConverter.GetBytes(1);
                    //connectServerStream.Write(message, 0, message.Length);

                    //// getTreePort(connectServerStream, i);

                    ////which tree
                    //// byte[] amessage = BitConverter.GetBytes(i);
                    ////connectServerStream.Write(amessage, 0, amessage.Length);

                    //byte[] responseMessage = new byte[4];

                    //connectServerStream.Read(responseMessage, 0, responseMessage.Length);
                    //Cport[i - 1] = BitConverter.ToInt16(responseMessage, 0);

                    //connectServerStream.Read(responseMessage, 0, responseMessage.Length);
                    //Dport[i - 1] = BitConverter.ToInt16(responseMessage, 0);

                    ////check whether there is available port in target peer

                    //if (Cport[i - 1] != 0 && Dport[i - 1] != 0)// && Cport[i - 1] != 1 && Dport[i - 1] != 1)
                    //    joined = true;
                    //else
                    //{
                    //    // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "SelectPeer " + conNode.Id + " remove T:" + (i - 1) + "\n" });

                    //    removePeer(conNode, i - 1);
                    //    if (conNode != null)
                    //        ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "SelectPeer " + conNode.Id + " remove T:" + (i - 1) + " ~\n" });
                    //    conNode = null;
                    //}

                    //Cport11 = Cport[i - 1];

                    //total_select_round += 1;

                    //connectServerStream.Close();
                    //connectServerClient.Close();

                    Thread.Sleep(10);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (connectServerStream != null) connectServerStream.Close();
                if (connectServerClient != null) connectServerClient.Close();

                //remove peer from list
                removePeer(conNode, i - 1);
                //if (conNode != null)
                  //  ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "SelectPeer " + conNode.Id + " remove T:" + (i - 1) + "\n" });

                return false;
            }


        }

        public bool hasPeer(int tree)
        {
            treeAccessor = new PeerInfoAccessor(folder + "\\CH" + currentCh + Peerlist_name + tree);
            while (!treeAccessor.load())
            {
                Thread.Sleep(20);
                //treeAccessor.load();
            }
            return treeAccessor.hasPeerInList();

        }

        public void removePeer(PeerNode node, int tree)
        {
            if (node != null)
            {
                treeAccessor = new PeerInfoAccessor(folder + "\\CH" + currentCh + Peerlist_name + tree);
                //while (!treeAccessor.load())
                //{
                //    Thread.Sleep(20);
                //    //treeAccessor.load();
                //}
                while (!treeAccessor.deletePeer(node))
                {
                    Thread.Sleep(20);
                }

            }
        }

        public TcpClient getDataConnect(int tree)
        {
            TcpClient treeclient = null;
            NetworkStream stream = null;
            try
            {
                treeclient = new TcpClient(joinPeers[tree].Ip, Dport[tree]);

                //send selfId
                //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "T[" + tree + "] ID:" + joinPeers[tree].Id + " " + joinPeers[tree].Ip + ":" + Dport[tree] + " GDS\n" });
                ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "T[" + tree + "] ID:" + joinPeers[tree].Id + ":" + Dport[tree] + " D\n" });

                stream = treeclient.GetStream();
                string sendstr = selfid + "@" + PeerListenPort + "@" + this.cConfig.MaxPeer; 
                //selfid[tree] + "@" + PeerListenPort + "@" + this.cConfig.MaxPeer;
                Byte[] sendbyte = StrToByteArray(sendstr);
                byte[] MsgLength = BitConverter.GetBytes(sendstr.Length);
                stream.Write(MsgLength, 0, MsgLength.Length); //send size of id
                stream.Write(sendbyte, 0, sendbyte.Length);

                byte[] message = BitConverter.GetBytes(tree);
                stream.Write(message, 0, message.Length);


                // clientFrm.rtbdownload.AppendText("T[" + tree + "] ID:" + joinPeers[tree].Id + " " + joinPeers[tree].Ip + ":" + Dport[tree] + "\n");
                //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "T[" + tree + "] ID:" + joinPeers[tree].Id + " " + joinPeers[tree].Ip + ":" + Dport[tree] + "\n" });

                // stream.Close();
                return treeclient;
            }
            catch
            {
                ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "T[" + tree + "] ID:" + joinPeers[tree].Id + " " + joinPeers[tree].Ip + ":" + Dport[tree] + " GDSfail\n" });
                if (stream != null)
                    stream.Close();
                if (treeclient != null)
                    treeclient.Close();

                treeclient = null;
                return treeclient;
            }

        }

        public TcpClient getControlConnect(int tree)
        {
            TcpClient treeclient = null;
            NetworkStream stream = null;
            try
            {
                treeclient = new TcpClient(joinPeers[tree].Ip, Cport[tree]);

                //send selfId
                // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "T[" + tree + "] ID:" + joinPeers[tree].Id + " " + joinPeers[tree].Ip + ":" + Cport[tree] + " GCS\n" });
                ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "T[" + tree + "] ID:" + joinPeers[tree].Id + ":" + Cport[tree] + " C\n" });

                stream = treeclient.GetStream();
                string sendstr = selfid;//[tree];
                Byte[] sendbyte = StrToByteArray(sendstr);
                byte[] MsgLength = BitConverter.GetBytes(sendstr.Length);
                stream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                stream.Write(sendbyte, 0, sendbyte.Length);

                byte[] message = BitConverter.GetBytes(tree);
                stream.Write(message, 0, message.Length);

                //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "tree[" + tree + "] " + joinPeers[tree].Id + ":" + Cport[tree] + "\n" });
                // stream.Close();
                return treeclient;
            }
            catch
            {
                ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "T[" + tree + "] ID:" + joinPeers[tree].Id + " " + joinPeers[tree].Ip + ":" + Cport[tree] + " GCS fail\n" });
                if (stream != null)
                    stream.Close();
                if (treeclient != null)
                    treeclient.Close();

                treeclient = null;
                return treeclient;
            }
        }

        public bool updateTotalChunkNull(int treeNo, PeerNode peer)
        {
            if (!savingchunkNull)
            {
                savingchunkNull = true;

                treeAccessor = new PeerInfoAccessor(folder + "\\CH" + currentCh + Peerlist_name + treeNo);
                treeAccessor.deletePeer(peer);
                treeAccessor.addPeer(peer);
                chunkNullUpdated = false;

                savingchunkNull = false;
                return true;
            }
            else
                return false;
        }


        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        //selecting Peer for conection
        public PeerNode selectPeer(int tree, bool forceSaveXml, int type)
        {
            if (chunkNullUpdated)
            {
                bool saved = updateTotalChunkNull(tree, JoinPeer[tree]);//update the totalChunkNull to xml
                while (forceSaveXml && !saved)
                {
                    if (updateTotalChunkNull(tree, JoinPeer[tree]))
                        break;
                    Thread.Sleep(10);
                }
            }

            PeerNode tempPeer = null;
            treeAccessor = new PeerInfoAccessor(folder + "\\CH" + currentCh + Peerlist_name + tree);
            //bool checkLoad = treeAccessor.load();

            //if (!checkLoad)
            //    return tempPeer;

            //PeerNode tempPeer=null;

            //while (tempPeer == null)
            //{
            bool checkLoad = treeAccessor.load();
            if (!checkLoad)
            {
               // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "selectPeer: XMLLOADING\n" });

                return null;
                //Thread.Sleep(20);
                //continue;
            }
            //clientFrm.rtbdownload.AppendText(RandomNumber(0, peerAccess.getMaxId()+1) + "\n");
            //String id  = peerAccess.getPeer("0");                                               //select the server ip as default
            //string id = peerAccess.getMaxId().ToString();                  //select the last peer
            try
            {

                //if (treeAccessor.getMaxId() == -1)
                //{
                //    //int a = treeAccessor.getMaxId();
                //    ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "selectPeer: GETMAXID" + " T:" + tree + "\n" });

                //    //return null;
                //    int trytime = 5;
                //    for (int i = 0; i < trytime; i++)
                //    {
                //        Thread.Sleep(10);
                //        bool chec_kLoad = treeAccessor.load();
                //        if (treeAccessor.getMaxId() != -1)
                //            break;
                //    }

                //    if (treeAccessor.getMaxId() == -1)
                //        return null;

                //}   //continue;
                //string id = RandomNumber(0, treeAccessor.getMaxId() + 1).ToString();//Random select
                if (type == RANDOM_PEER)
                {
                    PeerNode target = treeAccessor.getRandomPeer();//treeAccessor.getPeer(id);
                    tempPeer = target;
                    //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "Ran_select peer:" + target.Id + " T:" + tree + "\n" });

                }
                else
                {
                    List<PeerNode> target = treeAccessor.getLeastChunkNullPeer(5);
                    tempPeer = target[RandomNumber(0, target.Count)];
                    //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "Non-Ran_select peer:" + tempPeer.Id + " T:" + tree + "\n" });

                }

                if (tempPeer.Id == selfid)//selfid[tree])
                {
                   // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "selectPeerID=selfID " + " T:" + tree + "\n" });
                    removePeer(tempPeer, tree);
                    //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "Remove selfID " + " T:" + tree + "\n" });

                    tempPeer = null;
                }
                //if(target == null)
                //    //continue;
                if (treeAccessor.reconecting(tempPeer.Id))//if peer is not exist, skip
                {
                    tempPeer = null;
                  //  ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "selectPeerID:" + tempPeer.Id + " peer is not exist\n" });

                }   //continue;

                //if (treeAccessor.checkchild(tempPeer, selfid[tree])) //if peer is child, skip it
                //{
                //    tempPeer = null;
                //    ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "peer is child\n" });

                //}
                //continue;
                //tempPeer = target;
            }
            catch (Exception ex)
            {
               // if (tempPeer != null)
                  //  ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "select peer " + tempPeer.Id + " T:" + tree + " error:\n" + ex.ToString() + "\n" });
                tempPeer = null;
                Thread.Sleep(20);
            }

            //}
            return tempPeer;
        }

        public List<PeerNode> selectPeerList(int tree, bool forceSaveXml, int size)
        {
            if (chunkNullUpdated)
            {
                bool saved = updateTotalChunkNull(tree, JoinPeer[tree]);//update the totalChunkNull to xml
                while (forceSaveXml && !saved)
                {
                    if (updateTotalChunkNull(tree, JoinPeer[tree]))
                        break;
                    Thread.Sleep(10);
                }
            }

            List<PeerNode> tempPeer = null;
            treeAccessor = new PeerInfoAccessor(folder + "\\CH" + currentCh + Peerlist_name + tree);

            bool checkLoad = treeAccessor.load();
            if (!checkLoad)
            {
                //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "selectPeer: XMLLOADING\n" });

                return null;
                //Thread.Sleep(20);
                //continue;
            }

            try
            {

                //if (type == RANDOM_PEER)
                //{
                //    PeerNode target = treeAccessor.getRandomPeer();//treeAccessor.getPeer(id);
                //    tempPeer = target;
                //    //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "Ran_select peer:" + target.Id + " T:" + tree + "\n" });

                //}
                //else
                //{
                    List<PeerNode> target = treeAccessor.getLeastChunkNullPeer(size);
                    tempPeer = target;
                    //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "Non-Ran_select peer:" + tempPeer.Id + " T:" + tree + "\n" });

                //}

                for (int i = 0; i < tempPeer.Count; i++)
                {
                    if (tempPeer[i].Id == selfid)//selfid[tree])
                    {
                       // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "selectPeerID=selfID " + " T:" + tree + "\n" });
                        removePeer(tempPeer[i], tree);
                        //((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "Remove selfID " + " T:" + tree + "\n" });

                        tempPeer = null;
                    }
                }
                //if(target == null)
                //    //continue;
                //if (treeAccessor.reconecting(tempPeer.Id))//if peer is not exist, skip
                //{
                //    tempPeer = null;
                //    ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "selectPeerID:" + tempPeer.Id + " peer is not exist\n" });

                //}   //continue;

            }
            catch (Exception ex)
            {
               // if (tempPeer != null)
                   // ((LoggerFrm)clientFrm.downloadFrm).rtbdownload.BeginInvoke(new UpdateTextCallback(clientFrm.UpdateRtbDownload), new object[] { "select peerlist   T:" + tree + " error:\n" + ex.ToString() + "\n" });
                tempPeer = null;
                Thread.Sleep(20);
            }

            return tempPeer;
        }


    }
}
