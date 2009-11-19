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
            //this.max_client = sConfig.MaxClient;

            localAddr = IPAddress.Parse(sConfig.Serverip);
            //localAddr= new  IPAddress.Parse("")

            listenerThread = new Thread(new ThreadStart(listenForClients));
            listenerThread.IsBackground = true;
            listenerThread.Name = " listen_for_clients";
            listenerThread.Start();

            this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "START@" + localAddr.ToString() + "\n" });
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
                    if (peertype.Contains("<client>"))
                    {
                        byte[] peeripMsg;
                        if (peerList.Count <= 0)
                        {
                            peeripMsg = StrToByteArray("NOPEER");
                        }
                        else
                        {
                            //byte[] peeripMsg = StrToByteArray(tbsendIp.Text);
                            //byte[] peeripMsg = StrToByteArray(peerList[0].Ip);         //get the server ip(temporary code)
                            peeripMsg = StrToByteArray(peerList[peerList.Count - 1].Ip);


                            byte[] ipsize = BitConverter.GetBytes(peeripMsg.Length);
                            cstream.Write(ipsize, 0, ipsize.Length); //send size of ip
                            cstream.Write(peeripMsg, 0, peeripMsg.Length);

                            //PeerNode clientNode = new PeerNode("127.0.0.1", 2, 2); //testing code
                            PeerNode clientNode = new PeerNode(clientendpt.ToString(), 2, 2);

                            clientNode.addParent(peerList[peerList.Count - 1].Ip);
                            //clientNode.addParent(peerList[0].Ip); 

                            peerList.Add(clientNode);                               //set the clientNode into peerlist(temporary code)

                            this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Client:" + clientNode.Ip + " connected to " + clientNode.ParentPeer[0] + "\n" });
                        }
                    }
                    else if (peertype.Contains("<server>"))
                    {
                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                        //int cmdsize = BitConverter.ToInt16(responsePeerMsg, 0);

                        int MaxClient = BitConverter.ToInt16(responsePeerMsg, 0);

                        PeerNode serverNode = new PeerNode(clientendpt.ToString(), 0, MaxClient);
                        peerList.Add(serverNode);
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
