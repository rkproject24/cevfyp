using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ClassLibrary;

namespace TrackerServer
{
    public partial class Form1 : Form
    {
        static int tlPort = 1500;
        private xml PeerInfo;
        private ServerConfig sConfig;
        IPAddress localAddr;
        TcpListener TrackerListen;
        Thread listenerThread;
        int max_client;

        List<PeerNode> peerList; //store all the Peer include server in a list

        public Form1()
        {
            sConfig = new ServerConfig();
            sConfig.load("C:\\ServerConfig.xml");
            peerList = new List<PeerNode>();

            InitializeComponent();
        }



        private void btnOn_Click(object sender, EventArgs e)
        {
            start();
            btnOn.Enabled = false;
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            rtbClientlist.Text = "";
            listenerThread.Abort();
            TrackerListen.Stop();
            peerList.Clear();

            start();
        }

        private void listenForClients()
        {
            TrackerListen = new TcpListener(localAddr, tlPort);
            TrackerListen.Start();

            while (true)
            {
                TcpClient client = TrackerListen.AcceptTcpClient();
                NetworkStream cstream = client.GetStream();
                //TcpClient.Client.RemoteEndPoint.   

                IPAddress clientendpt = ((IPEndPoint)client.Client.RemoteEndPoint).Address; 

                try
                {
                    //Get the Peer type- server/client
                    byte[] responsePeerMsg = new byte[8];
                    cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                    string peertype = ByteArrayToString(responsePeerMsg);
                    //int temp = Int32.Parse(tbsendIp.Text);


                    if (peertype.Contains("<clientRequest>"))
                    {
                        byte[] peeripMsg;
                        if (File.Exists("PeerInfoT1.xml"))
                        {
                            FileStream file = new FileStream("PeerInfoT1.xml", FileMode.OpenOrCreate, FileAccess.Read);

                            // Create a new stream to read from a file
                            StreamReader sr = new StreamReader(file);

                            // Read contents of file into a string
                            string s1 = sr.ReadToEnd();

                            // Close StreamReader
                            sr.Close();

                            // Close file
                            file.Close();

                            file = new FileStream("PeerInfoT2.xml", FileMode.OpenOrCreate, FileAccess.Read);

                            // Create a new stream to read from a file
                            sr = new StreamReader(file);

                            // Read contents of file into a string
                            string s2 = sr.ReadToEnd();

                            // Close StreamReader
                            sr.Close();

                            // Close file
                            file.Close();


                            //byte[] peeripMsg = StrToByteArray(tbsendIp.Text);
                            //byte[] peeripMsg = StrToByteArray(peerList[0].Ip);         //get the server ip(temporary code)
                            //peeripMsg = StrToByteArray(peerList[peerList.Count - 1].Ip);
                            peeripMsg = StrToByteArray(s1+"@"+s2);

                            byte[] MsgLength = BitConverter.GetBytes(peeripMsg.Length);
                            cstream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                            cstream.Write(peeripMsg, 0, peeripMsg.Length);

                        }
                        else
                        {
                            peeripMsg = StrToByteArray("NOPEER");
                        }

                    }
                    else if (peertype.Contains("<clientReg>"))
                    {
                        //PeerNode clientNode = new PeerNode("127.0.0.1", 2, 2); //testing code
                        //PeerNode clientNode = new PeerNode(clientendpt.ToString(), 2, 2);

                        //clientNode.addParent(peerList[peerList.Count - 1].Ip);
                        //clientNode.addParent(peerList[0].Ip); 

                        //peerList.Add(clientNode);                               //set the clientNode into peerlist(temporary code)

                        //this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Client:" + clientNode.Ip + " connected to " + clientNode.ParentPeer[0] + "\n" });


                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        int MaxClient = BitConverter.ToInt16(responsePeerMsg, 0);
                        PeerNode serverNode = new PeerNode(clientendpt.ToString(), 0, MaxClient);

                        byte[] responsePeerMsg1 = new byte[4];
                        cstream.Read(responsePeerMsg1, 0, responsePeerMsg1.Length);

                        int layer = BitConverter.ToInt16(responsePeerMsg, 0);


                        //int cmdsize = BitConverter.ToInt16(responsePeerMsg, 0);
                        int DataNo = Convert.ToInt32(PeerInfo.Read("Info", "DataNo")) + 1;
                        PeerInfo.modify("Info", "DataNo", DataNo.ToString());
                        PeerInfo.Add("Info", "IP" + DataNo.ToString(), serverNode.Ip.ToString());
                        PeerInfo.Add("Info", serverNode.Ip.ToString(), layer.ToString());
                        


                        peerList.Add(serverNode);
                        this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Server " + clientendpt.ToString() + " started\n" });
                    }
                    else if (peertype.Contains("<serverReg>"))
                    {
                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                        //int cmdsize = BitConverter.ToInt16(responsePeerMsg, 0);

                        int MaxClient = BitConverter.ToInt16(responsePeerMsg, 0);

                        PeerNode serverNode = new PeerNode(clientendpt.ToString(), 0, MaxClient);

                        if (File.Exists("PeerInfoT1.xml"))
                        {
                            File.Delete("PeerInfoT1.xml");
                        }
                        //peerList.Add(serverNode);
                        PeerInfo = new xml("PeerInfoT1.xml", "Info");
                        PeerInfo.Add("Info", "DataNo","1");
                        PeerInfo.Add("Info", "IP1", serverNode.Ip.ToString());
                        PeerInfo.Add("Info", serverNode.Ip.ToString(), "0");

                        if (File.Exists("PeerInfoT2.xml"))
                        {
                            File.Delete("PeerInfoT2.xml");
                        }
                        PeerInfo = new xml("PeerInfoT2.xml","Info");
                        PeerInfo.Add("Info", "DataNo","1");
                        PeerInfo.Add("Info", "IP1", serverNode.Ip.ToString());
                        PeerInfo.Add("Info", serverNode.Ip.ToString(), "0");

                        this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Server " + clientendpt.ToString() + " started\n" });

                    }


                    
                    //break;
                }
                catch
                {
                    this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "One client join fail...\n" });
                    break;
                }

            }
        }

        private void start()
        {
            //this.max_client = sConfig.MaxClient;

            localAddr = IPAddress.Parse(sConfig.Serverip);
            //localAddr= new  IPAddress.Parse("")

            listenerThread = new Thread(new ThreadStart(listenForClients));
            listenerThread.IsBackground = true;
            listenerThread.Name = " listen_for_clients";
            listenerThread.Start();

            this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "START@" + localAddr.ToString() + "\n" });
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }
        private static string ByteArrayToString(byte[] bytes)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }

        private delegate void UpdateTextCallback(string message);
        public void UpdatertbClientlist(string message)
        {
            rtbClientlist.AppendText(message);
        }

    }
}
