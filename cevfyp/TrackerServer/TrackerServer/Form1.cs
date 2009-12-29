using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
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
            //xml test = new xml("text.xml", "test");

            //string[] testT = {"a","b","c" };
            //string[] testV = {"1","2","3" };
            //test.Add("node1", testT, testV);
            //test.Add("signleN", "Type", "Value");
            //string a = test.Read("node1","a");
            //string b = test.Read("signleN", "Type");
            //MessageBox.Show("name =a :" + a + " " + b);

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
                    byte[] responsePeerMsg = new byte[11];
                    cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                    string peertype = ByteArrayToString(responsePeerMsg);
                    //int temp = Int32.Parse(tbsendIp.Text);
                    //MessageBox.Show(peertype);

                    if (peertype.Contains("<clientReq>"))
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

                        }
                        else
                        {
                            peeripMsg = StrToByteArray("NOPEER");
                        }
                        byte[] MsgLength = BitConverter.GetBytes(peeripMsg.Length);
                        cstream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                        cstream.Write(peeripMsg, 0, peeripMsg.Length);

                    }
                    //else if (peertype.Contains("<clientReg>"))
                    //{
                    //    responsePeerMsg = new byte[4];
                    //    cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    //    int MaxClient = BitConverter.ToInt16(responsePeerMsg, 0);
                    //    PeerNode serverNode = new PeerNode(clientendpt.ToString(), 0, MaxClient);

                    //    byte[] responsePeerMsg1 = new byte[4];
                    //    cstream.Read(responsePeerMsg1, 0, responsePeerMsg1.Length);

                    //    int layer = Convert.ToInt32(BitConverter.ToString(responsePeerMsg1, 0));

                    //    //int cmdsize = BitConverter.ToInt16(responsePeerMsg, 0);



                    //    //PeerInfo.modify("Info", "DataNo", DataNo.ToString());

                    //    PeerInfo = new xml("PeerInfoT1.xml", "Info");
                    //    int DataNum = PeerInfo.GetElementNum();
                    //    DataNum++;
                    //    //MessageBox.Show(layer.ToString());
                    //    layer++;
                    //    //MessageBox.Show(layer.ToString());
                    //    string[] InfoN1 = { "IP", "Layer" };
                    //    string[] Value1 = { serverNode.Ip.ToString(), layer.ToString() };
                    //    PeerInfo.Add(DataNum.ToString(), InfoN1, Value1);



                    //    responsePeerMsg1 = new byte[4];
                    //    cstream.Read(responsePeerMsg1, 0, responsePeerMsg1.Length);

                    //    layer = BitConverter.ToInt16(responsePeerMsg1, 0);
                    //    PeerInfo = new xml("PeerInfoT2.xml", "Info");

                    //    //int cmdsize = BitConverter.ToInt16(responsePeerMsg, 0);
                    //    DataNum = PeerInfo.GetElementNum();
                    //    DataNum++;
                    //    layer++;
                    //    string[] InfoN2 = { "IP", "Layer" };
                    //    string[] Value2 = { serverNode.Ip.ToString(), layer.ToString() };
                    //    PeerInfo.Add(DataNum.ToString(), InfoN2, Value2);


                    //    peerList.Add(serverNode);
                    //    this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Server " + clientendpt.ToString() + " started\n" });
                    //}
                    else if (peertype.Contains("<serverReg>"))
                    {
                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                        int MaxClient = BitConverter.ToInt16(responsePeerMsg, 0);

                        PeerNode serverNode = new PeerNode("0", clientendpt.ToString(), MaxClient);
                        serverNode.Layer = 0;
                        //PeerInfo = new xml("PeerInfoT1.xml", "Info", true);

                        ////PeerInfo = new xml("PeerInfoT1.xml", "Info");
                        //string[] InfoN1 = {"IP", "Layer" };
                        //string[] Value1 = {serverNode.Ip.ToString(), "1" };
                        //PeerInfo.Add("Peer", InfoN1, Value1);


                        //PeerInfo = new xml("PeerInfoT2.xml","Info", true);
                        //string[] InfoN2 = {"IP", "Layer" };
                        //string[] Value2 = {serverNode.Ip.ToString(), "1" };
                        //PeerInfo.Add("Peer", InfoN2, Value2);

//by vinci:
                        if (File.Exists("PeerInfoT1.xml"))
                            File.Delete("PeerInfoT1.xml");
                        PeerInfoAccessor TreeAccess1 = new PeerInfoAccessor("PeerInfoT1.xml");
                        TreeAccess1.addPeer(serverNode);


                        if (File.Exists("PeerInfoT2.xml"))
                            File.Delete("PeerInfoT2.xml");
                        PeerInfoAccessor TreeAccess2 = new PeerInfoAccessor("PeerInfoT2.xml");
                        TreeAccess2.addPeer(serverNode);
                        
                        this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Server " + clientendpt.ToString() + " started\n" });

                    }

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
