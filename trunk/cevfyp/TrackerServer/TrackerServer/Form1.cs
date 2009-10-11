using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        public Form1()
        {
            sConfig = new ServerConfig();
            sConfig.load("C:\\ServerConfig.xml");
            //localAddr= new  IPAddress.Parse(
            InitializeComponent();
        }



        private void btnOn_Click(object sender, EventArgs e)
        {
            this.max_client = sConfig.MaxClient;

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

                EndPoint clientendpt = client.Client.RemoteEndPoint; 

                try
                {
                    //int temp = Int32.Parse(tbsendIp.Text);
                    Byte[] peeripMsg = StrToByteArray(tbsendIp.Text);

                    Byte[] ipsize = BitConverter.GetBytes( peeripMsg.Length);
                    cstream.Write(ipsize, 0, ipsize.Length); //send size of ip

                    cstream.Write(peeripMsg, 0, peeripMsg.Length);


                    this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Client " +clientendpt.ToString() + " connected\n" });
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


        private delegate void UpdateTextCallback(string message);
        public void UpdatertbClientlist(string message)
        {
            rtbClientlist.AppendText(message);
        }
    }
}
