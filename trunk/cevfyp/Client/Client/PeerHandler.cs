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
        static int TREE_NO = 2;
        static int TrackerSLPort = 1500;

        private string trackIp;
        private string peerIp;
        private ClientForm clientFrm;


        int Cport = 0;             //control message port number
        ClientConfig cConfig = new ClientConfig();

        public int Cport1
        {
            get { return Cport; }
            set { Cport = value; }
        }
        public string PeerIp
        {
            get { return peerIp; }
            set { peerIp = value; }
        }

        //int D1port = 0;             //video data port number
        int[] Dport = new int[TREE_NO];

        public PeerHandler(string trackerIp, ClientForm clientFrm)
        {
            this.clientFrm = clientFrm;
            cConfig.load("C:\\ClientConfig.xml");
            this.trackIp = trackerIp;
            for (int i = 0; i < TREE_NO; i++)
            {
                Dport[i] = 0;
            }
        }
        //by Vinci: coonect to Tracker for peer ip 
        public bool findTracker()
        {
            TcpClient trackerTcpClient;
            NetworkStream trackerStream;
            /*try
            {
                trackerTcpClient = new TcpClient(trackIp, TrackerSLPort);
                trackerStream = trackerTcpClient.GetStream();

                //define client type
                Byte[] clienttype = StrToByteArray("<client>");
                trackerStream.Write(clienttype, 0, clienttype.Length);


                byte[] responsePeerMsg = new byte[4];
                trackerStream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                int ipsize = BitConverter.ToInt16(responsePeerMsg, 0);

                byte[] responsePeerMsg2 = new byte[ipsize];
                trackerStream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                //string peerip = BitConverter.ToString(responsePeerMsg2, 0);
                this.peerIp = ByteArrayToString(responsePeerMsg2);
                //Cport = BitConverter.ToInt16(responseCMessage, 0);

                trackerTcpClient.Close();
                trackerStream.Close();

                return true;
            }*/
            try
            {
                trackerTcpClient = new TcpClient(trackIp, TrackerSLPort);
                trackerStream = trackerTcpClient.GetStream();

                //define client type
                Byte[] clienttype = StrToByteArray("<clientReq>");
                trackerStream.Write(clienttype, 0, clienttype.Length);


                byte[] responsePeerMsg = new byte[4];
                trackerStream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                int xmlsize = BitConverter.ToInt16(responsePeerMsg, 0);

                byte[] responsePeerMsg2 = new byte[xmlsize];
                trackerStream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                //string peerip = BitConverter.ToString(responsePeerMsg2, 0);
                //this.peerIp = ByteArrayToString(responsePeerMsg2);
                //Cport = BitConverter.ToInt16(responseCMessage, 0);
                string xmlContent = ByteArrayToString(responsePeerMsg2);

                string[] xmlTrees = xmlContent.Split('@');

                if (File.Exists("PeerInfoT1.xml"))
                    File.Delete("PeerInfoT1.xml");
                // Specify file, instructions, and privelegdes
                FileStream file = new FileStream("PeerInfoT1.xml", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(file);
                sw.Write(xmlTrees[0]);
                sw.Close();
                file.Close();

                if (File.Exists("PeerInfoT2.xml"))
                    File.Delete("PeerInfoT2.xml");
                file = new FileStream("PeerInfoT2.xml", FileMode.OpenOrCreate, FileAccess.Write);
                sw = new StreamWriter(file);
                sw.Write(xmlTrees[1]);
                sw.Close();
                file.Close();

                virtualResponse();

                trackerTcpClient.Close();
                trackerStream.Close();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            return false;
        }

        //For Testing Only
        public void virtualResponse()
        {
            XmlDocument read = new XmlDocument();
            read.Load("PeerInfoT1.xml");
            string PeerNoT1 = read.SelectSingleNode("Info").Attributes["DataNo"].Value;

            string IPT1 = read.SelectSingleNode("Info").Attributes["IP" + PeerNoT1].Value;
            string Layer1 = read.SelectSingleNode("Info").Attributes["Layer" + PeerNoT1].Value;
            //xml readT2 = new xml("G:\\PeerInfoT2.xml","Info");
            //string PeerNoT1 =readT1.Read("Info","DataNo");
            //string IPT1 = readT1.Read("Info", "IP" + PeerNoT1);
            read.Load("PeerInfoT2.xml");
            string PeerNoT2 = read.SelectSingleNode("Info").Attributes["DataNo"].Value;

            string IPT2 = read.SelectSingleNode("Info").Attributes["IP" + PeerNoT2].Value;
            string Layer2 = read.SelectSingleNode("Info").Attributes["Layer" + PeerNoT2].Value;
            //string PeerNoT2 = readT2.Read("Info", "DataNo");
            //string IPT2 = readT2.Read("Info", "IP"+ PeerNoT2);
            System.Windows.Forms.MessageBox.Show(IPT1);
            this.peerIp = IPT1;
            SendRespond(Layer1, Layer2);
            return;
        }

        // Write for tracker registration.
        public bool SendRespond(string layerT1, string layerT2)
        {
            TcpClient connectTracker;
            NetworkStream connectTrackerStream;
            try
            {

                connectTracker = new TcpClient(trackIp, TrackerSLPort);
                connectTrackerStream = connectTracker.GetStream();

                //define client type
                Byte[] clienttype = StrToByteArray("<clientReg>");

                //byte[] MsgLength = BitConverter.GetBytes(clientLayer.Length);

                connectTrackerStream.Write(clienttype, 0, clienttype.Length);

                Byte[] maxClient = StrToByteArray(this.cConfig.MaxPeer.ToString());
                connectTrackerStream.Write(maxClient, 0, maxClient.Length);

                //connectTrackerStream.Write(MsgLength, 0, MsgLength.Length);
                Byte[] clientLayer = StrToByteArray(layerT1);
                connectTrackerStream.Write(clientLayer, 0, clientLayer.Length);

                clientLayer = StrToByteArray(layerT2);
                connectTrackerStream.Write(clientLayer, 0, clientLayer.Length);

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
            TcpClient connectServerClient;
            NetworkStream connectServerStream;
            try
            {
                connectServerClient = new TcpClient(peerIp, cConfig.ServerSLPort1);
                connectServerStream = connectServerClient.GetStream();


                Byte[] responseCMessage = new Byte[4];
                connectServerStream.Read(responseCMessage, 0, responseCMessage.Length);
                Cport = BitConverter.ToInt16(responseCMessage, 0);

                for (int i = 0; i < TREE_NO; i++)
                {
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
            TcpClient treeclient = new TcpClient(peerIp, Dport[tree]);
            clientFrm.rtbdownload.AppendText("tree[" + tree + "] " + peerIp + ":" + Dport[tree] + "\n");
            return treeclient;
        }
    }
}
