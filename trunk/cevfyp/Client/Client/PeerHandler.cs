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

namespace Client
{
    class PeerHandler
    {
        //static int TRACKER_PORT = 1100;  //server listen port
        const string Peerlist_name = "PeerInfoT"; 
        //static int TREE_NO = 2;
        public int tree_num=2;

        static int TrackerSLPort = 1500;

        private string trackIp;
        private PeerNode joinPeer;

        private ClientForm clientFrm;
       private string[] selfid;// = new string[TREE_NO];
       
        //int Cport = 0;             
        ClientConfig cConfig = new ClientConfig();
        ServerConfig sConfig = new ServerConfig();
        PeerInfoAccessor treeAccessor;

        


        /*public int Cport11
        {
            get { return Cport; }
            set { Cport = value; }
        }
         */
         
        public PeerNode PeerIp
        {
            get { return joinPeer; }
            set { joinPeer = value; }
        }


        //int D1port = 0;             //video data port number
        int[] Dport ;//= new int[TREE_NO];
        int[] Cport;//= new int[TREE_NO];

        public int Cport11 = 0;


        public PeerHandler(string trackerIp, ClientForm clientFrm)
        {
           
            this.clientFrm = clientFrm;
            cConfig.load("C:\\ClientConfig");
           
            selfid = new string[tree_num];
            Dport = new int[tree_num];
            Cport = new int[tree_num];

            
            
            this.trackIp = trackerIp;
            //for (int i = 0; i < TREE_NO; i++)
           // {
           //     Dport[i] = 0;
           // }

            for (int i = 0; i < tree_num; i++)
            {
                Dport[i] = 0;
                Cport[i] = 0;
            }
        }
        //by Vinci: coonect to Tracker for peer ip 
        public bool findTracker()
        {
          /*  for (int i = 0; i < TREE_NO; i++)
            {
                downloadPeerlist(i);
            }
            */

            for (int i = 0; i < tree_num; i++)
            {
                downloadPeerlist(i);
            }

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
            return true;
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

                if (File.Exists(PeerFileName))
                    File.Delete(PeerFileName);
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
                System.Windows.Forms.MessageBox.Show("Tree:" +tree + "\n" +ex.ToString());
            }
            return false;
        }


        // Write for tracker registration.
        public bool registerToTracker(int tree, string layer)
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

                string sendstr = tree + "@" + selfid[tree] + "@" + this.cConfig.MaxPeer + "@" + layer;
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
            //peerIp = trackIp;
            //if (peerIp.Equals("NOPEER")) //check peer IP message from Tracker, if Tracker give "NOPEER" means no parent to join
            //{

            //select a peer to connect
            PeerNode conNode = selectPeer();
            joinPeer = conNode;


            TcpClient connectServerClient;
            NetworkStream connectServerStream;
            try
            {
                int temp = cConfig.SLPort;
                connectServerClient = new TcpClient(joinPeer.Ip, temp);
                connectServerStream = connectServerClient.GetStream();

                /*
                Byte[] responseCMessage = new Byte[4];
                connectServerStream.Read(responseCMessage, 0, responseCMessage.Length);
                Cport = BitConverter.ToInt16(responseCMessage, 0);

                for (int i = 0; i < TREE_NO; i++)
                {
                    Byte[] responseDMessage = new Byte[4];
                    connectServerStream.Read(responseDMessage, 0, responseDMessage.Length);
                    Dport[i] = BitConverter.ToInt16(responseDMessage, 0);
                }
                */

                for (int i = 0; i < tree_num; i++)
                {
                    Byte[] responseCMessage = new Byte[4];
                    connectServerStream.Read(responseCMessage, 0, responseCMessage.Length);
                    Cport[i] = BitConverter.ToInt16(responseCMessage, 0);
                    
                    Cport11 = Cport[i];
                    
                    Byte[] responseDMessage = new Byte[4];
                    connectServerStream.Read(responseDMessage, 0, responseDMessage.Length);
                    Dport[i] = BitConverter.ToInt16(responseDMessage, 0);
                }

                connectServerStream.Close();
                connectServerClient.Close();

                //vcport = Cport + 200;

                //serverConnect = true;
                //checkClose = false;

                return true;
            }
            catch
            {
            }
            //}
            return false;
        }


        //selecting Peer for conection
        private PeerNode selectPeer()
        {
            PeerInfoAccessor peerAccess = new PeerInfoAccessor("PeerInfoT0");
            //return peerAccess.getPeer("0"); //select the server ip as default
            return peerAccess.getPeer(peerAccess.getMaxId().ToString());

        }


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
            TcpClient treeclient = new TcpClient(joinPeer.Ip, Dport[tree]);
            clientFrm.rtbdownload.AppendText("tree[" + tree + "] " + joinPeer + ":" + Dport[tree] + "\n");
            return treeclient;
        }

        public TcpClient getControlConnect(int tree)
        {


            TcpClient treeclient = new TcpClient(joinPeer.Ip, Cport[tree]);
          //  clientFrm.rtbdownload.AppendText("tree[" + tree + "] " + joinPeer + ":" + Cport[tree] + "\n");
            return treeclient;
        }

    }
}
