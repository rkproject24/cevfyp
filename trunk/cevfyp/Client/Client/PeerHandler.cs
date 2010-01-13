using System;
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
        string Peerlist_name = "PeerInfoT"; 
        int TREE_NO;
   

        static int TrackerSLPort = 1500;

        private string trackIp;
        private PeerNode[] joinPeers;
        private ClientForm clientFrm;
        private string[] selfid;//= new string[TREE_NO];
      
        //int Cport = 0;             
        ClientConfig cConfig = new ClientConfig();
        //ServerConfig sConfig = new ServerConfig();
        PeerInfoAccessor treeAccessor;

        


        /*public int Cport11
        {
            get { return Cport; }
            set { Cport = value; }
        }
         */
        public string[] Selfid
        {
            get { return selfid; }
            set { selfid = value; }
        }
        public PeerNode[] JoinPeer
        {
            get { return joinPeers; }
            set { joinPeers = value; }
        }

        //int D1port = 0;             //video data port number
        int[] Dport;// = new int[TREE_NO];
        int[] Cport;// = new int[TREE_NO];

        public int Cport11 = 0;


        public PeerHandler(string trackerIp, ClientForm clientFrm)
        {
           
            this.clientFrm = clientFrm;
            cConfig.load("C:\\ClientConfig");

            TREE_NO = 0;
            this.trackIp = trackerIp;

            selfid = new string[1];
        }

        public void treeInitial()
        {
            //create directory for each client program
            if (Directory.Exists(selfid[0]))
                Directory.Delete(selfid[0], true);
            Directory.CreateDirectory(selfid[0]);
            Peerlist_name = selfid[0] + "\\" + Peerlist_name;

            selfid = new string[TREE_NO];
            Dport = new int[TREE_NO];
            Cport = new int[TREE_NO];

            for (int i = 0; i < TREE_NO; i++)
            {
                Dport[i] = 0;
                Cport[i] = 0;
            }
        }

        //by Vinci: coonect to Tracker for peer ip 
        public int findTracker()
        {
            if (!downloadPeerlist(0))
                return -1;
            treeAccessor = new PeerInfoAccessor(Peerlist_name + "0");
            this.TREE_NO = treeAccessor.getTreeSize();
            treeInitial();

            for (int i = 0; i < TREE_NO; i++)
            {
                if (!downloadPeerlist(i))
                {
                    return -1;
                }
            }

            joinPeers = new PeerNode[TREE_NO]; //define size of JoinpeerNode array

            //TcpClient trackerTcpClient;
            //NetworkStream trackerStream;
            //try
            //{
            //    trackerTcpClient = new TcpClient(trackIp, TrackerSLPort);
            //    trackerStream = trackerTcpClient.GetStream();

            //    //define client type
            //    Byte[] clienttype = StrToByteArray("<clientReq>");
            //    trackerStream.Write(clienttype, 0, clienttype.Length);


            //    byte[] responsePeerMsg = new byte[4];
            //    trackerStream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

            //    int xmlsize = BitConverter.ToInt16(responsePeerMsg, 0);

            //    byte[] responsePeerMsg2 = new byte[xmlsize];
            //    trackerStream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
            //    //string peerip = BitConverter.ToString(responsePeerMsg2, 0);
            //    //this.peerIp = ByteArrayToString(responsePeerMsg2);
            //    //Cport = BitConverter.ToInt16(responseCMessage, 0);
            //    string xmlContent = ByteArrayToString(responsePeerMsg2);

            //    string[] xmlTrees = xmlContent.Split('@');

            //    if (File.Exists("PeerInfoT1.xml"))
            //        File.Delete("PeerInfoT1.xml");
            //    // Specify file, instructions, and privelegdes
            //    FileStream file = new FileStream("PeerInfoT1.xml", FileMode.OpenOrCreate, FileAccess.Write);
            //    StreamWriter sw = new StreamWriter(file);
            //    sw.Write(xmlTrees[0]);
            //    sw.Close();
            //    file.Close();

            //    if (File.Exists("PeerInfoT2.xml"))
            //        File.Delete("PeerInfoT2.xml");
            //    file = new FileStream("PeerInfoT2.xml", FileMode.OpenOrCreate, FileAccess.Write);
            //    sw = new StreamWriter(file);
            //    sw.Write(xmlTrees[1]);
            //    sw.Close();
            //    file.Close();

            //    treeAccessor1 = new PeerInfoAccessor("PeerInfoT1");
            //    this.selfid[0] = (treeAccessor1.getMaxId()+1).ToString();

            //    treeAccessor2 = new PeerInfoAccessor("PeerInfoT2");
            //    this.selfid[1] = (treeAccessor2.getMaxId() + 1).ToString();

            //    //selfid = xmlTrees[2];
            //    //virtualResponse();

            //    trackerTcpClient.Close();
            //    trackerStream.Close();

            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    //System.Windows.Forms.MessageBox.Show(ex.ToString());
            //}
            return TREE_NO;
        }

        public bool downloadPeerlist(int tree)
        {
            TcpClient trackerTcpClient;
            NetworkStream trackerStream;
            string PeerFileName = Peerlist_name + tree + ".xml";
            try
            {
                trackerTcpClient = new TcpClient(trackIp, TrackerSLPort);
                trackerStream = trackerTcpClient.GetStream();

                //define client type
                Byte[] clienttype = StrToByteArray("<clientReq>");
                trackerStream.Write(clienttype, 0, clienttype.Length);

                byte[] treeNo = BitConverter.GetBytes(tree);
                trackerStream.Write(treeNo, 0, treeNo.Length);

                byte[] responsePeerMsg = new byte[4];
                trackerStream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                int xmlsize = BitConverter.ToInt16(responsePeerMsg, 0);

                byte[] responsePeerMsg2 = new byte[xmlsize];
                trackerStream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);

                string xmlContent = ByteArrayToString(responsePeerMsg2);

                //string[] xmlTrees = xmlContent.Split('@');

                //if (File.Exists(PeerFileName))
                //    File.Delete(PeerFileName);
                // Specify file, instructions, and privelegdes
                FileStream file = new FileStream(PeerFileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(file);
                sw.Write(xmlContent);
                sw.Close();
                file.Close();

                //if (File.Exists("PeerInfoT2.xml"))
                //    File.Delete("PeerInfoT2.xml");
                //file = new FileStream("PeerInfoT2.xml", FileMode.OpenOrCreate, FileAccess.Write);
                //sw = new StreamWriter(file);
                //sw.Write(xmlTrees[1]);
                //sw.Close();
                //file.Close();

                treeAccessor = new PeerInfoAccessor(Peerlist_name + tree);
                this.selfid[tree] = (treeAccessor.getMaxId() + 1).ToString();

                //treeAccessor2 = new PeerInfoAccessor("PeerInfoT2");
                //this.selfid[1] = (treeAccessor2.getMaxId() + 1).ToString();

                //selfid = xmlTrees[2];
                //virtualResponse();

                trackerTcpClient.Close();
                trackerStream.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                //System.Windows.Forms.MessageBox.Show("Tree:" +tree + "\n" +ex.ToString());
            }
            return false;
        }


        // Write for tracker registration.
        public bool registerToTracker(int tree, int listenPort, string layer)
        {
            TcpClient connectTracker;
            NetworkStream connectTrackerStream;
            try
            {

                connectTracker = new TcpClient(trackIp, TrackerSLPort);
                connectTrackerStream = connectTracker.GetStream();

                //define client message type
                Byte[] clienttype = StrToByteArray("<clientReg>");
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

                string sendstr = listenPort + "@" +tree + "@" + selfid[tree] + "@" + this.cConfig.MaxPeer + "@" + layer;
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
            }

            return true;
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

        public bool connectPeer()  //connect to Peer to get the port no of Cport Dport
        {
            TcpClient connectServerClient;
            NetworkStream connectServerStream;
            try
            {

                //require each tree at each round.
                for (int i = 1; i <= TREE_NO; i++)
                {
                    bool joined = false;

                    while (!joined)
                    {
                        //select a peer to connect
                        PeerNode conNode = selectPeer(i - 1);
                        joinPeers[i - 1] = conNode;

                        connectServerClient = new TcpClient(joinPeers[i - 1].Ip, joinPeers[i - 1].ListenPort);
                        connectServerStream = connectServerClient.GetStream();

                        Byte[] message = BitConverter.GetBytes(1);
                        connectServerStream.Write(message, 0, message.Length);

                        // getTreePort(connectServerStream, i);

                        byte[] amessage = BitConverter.GetBytes(i);
                        connectServerStream.Write(amessage, 0, amessage.Length);

                        byte[] responseMessage = new Byte[4];
                        connectServerStream.Read(responseMessage, 0, responseMessage.Length);
                        Cport[i - 1] = BitConverter.ToInt16(responseMessage, 0);

                        connectServerStream.Read(responseMessage, 0, responseMessage.Length);
                        Dport[i - 1] = BitConverter.ToInt16(responseMessage, 0);

                        //check whether there is available port in target peer
                        if (Cport[i - 1] != 0 && Dport[i - 1] != 0)
                            joined = true;

                        Cport11 = Cport[i - 1];


                        //connectServerStream.Close();
                        //connectServerClient.Close();

                        connectServerStream.Dispose();
                        connectServerClient.Close();
                        connectServerClient = null;

                        Thread.Sleep(10);
                    }

                    //serverConnect = true;
                    //checkClose = false;
                }
                return true;
            }
            catch
            {
                return false;
            }
            //}
            return false;
        }

        //private void getTreePort(NetworkStream stream,int tree_num)
        //{

        //    byte[] message = BitConverter.GetBytes(tree_num);
        //    stream.Write(message, 0, message.Length);

        //    byte[] responseMessage = new Byte[4];
        //    stream.Read(responseMessage, 0, responseMessage.Length);
        //    Cport[tree_num-1] = BitConverter.ToInt16(responseMessage, 0);

        //    stream.Read(responseMessage, 0, responseMessage.Length);
        //    Dport[tree_num-1] = BitConverter.ToInt16(responseMessage, 0);

        //    Cport11 = Cport[tree_num - 1];


        //}


        public TcpClient getDataConnect(int tree)
        {


            //be modify later
            //if (Cport != 0 && Dport != 0)
            //{
            //    try
            //    {
            //        ClientC = new TcpClient(sourceIp, Cport);
            //    }
            //    catch
            //    {
            //        return "Port " + Cport + " Unreachable!";
            //    }

            //    try
            //    {
            //        ClientD = new TcpClient(sourceIp, Dport);
            //    }
            //    catch
            //    {
            //        return "Port " + Dport + " Unreachable!";
            //    }

            //    return "OK2";
            //}
            //else
            //{
            //    return "No source can join!";
            //}
            TcpClient treeclient = new TcpClient(joinPeers[tree].Ip, Dport[tree]);

            //send selfId
            NetworkStream stream = treeclient.GetStream();
            string sendstr = selfid[tree];
            Byte[] sendbyte = StrToByteArray(sendstr);
            byte[] MsgLength = BitConverter.GetBytes(sendstr.Length);
            stream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
            stream.Write(sendbyte, 0, sendbyte.Length);

            clientFrm.rtbdownload.AppendText("T[" + tree + "] ID:" + joinPeers[tree].Id + " " + joinPeers[tree].Ip + ":" + Dport[tree] + "\n");
            return treeclient;
        }

        public TcpClient getControlConnect(int tree)
        {
            TcpClient treeclient = new TcpClient(joinPeers[tree].Ip, Cport[tree]);

            //send selfId
            NetworkStream stream = treeclient.GetStream();
            string sendstr = selfid[tree];
            Byte[] sendbyte = StrToByteArray(sendstr);
            byte[] MsgLength = BitConverter.GetBytes(sendstr.Length);
            stream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
            stream.Write(sendbyte, 0, sendbyte.Length);

          //  clientFrm.rtbdownload.AppendText("tree[" + tree + "] " + joinPeer + ":" + Cport[tree] + "\n");
            return treeclient;
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        //selecting Peer for conection
        private PeerNode selectPeer(int tree)
        {
            //PeerInfoAccessor peerAccess = new PeerInfoAccessor("PeerInfoT0");
            PeerInfoAccessor peerAccess = new PeerInfoAccessor(Peerlist_name + tree);
            PeerNode tempPeer=null;

            while (tempPeer == null)
            {
                //clientFrm.rtbdownload.AppendText(RandomNumber(0, peerAccess.getMaxId()+1) + "\n");
                tempPeer = peerAccess.getPeer(RandomNumber(0, peerAccess.getMaxId() + 1).ToString());   //Random select
                //tempPeer = peerAccess.getPeer("0");                                               //select the server ip as default
                //tempPeer = peerAccess.getPeer(peerAccess.getMaxId().ToString());                  //select the last peer
            }
            return tempPeer;
        }

    }
}
