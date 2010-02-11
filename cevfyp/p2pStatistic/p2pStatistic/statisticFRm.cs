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

using Analysis;
using ClassLibrary;

namespace p2pStatistic
{
    public partial class statisticFRm : Form
    {
        plotgraph xmldata;
        //int CurrentIndex = 0;
        int statisticListen;
        
        IPAddress localAddr;
        TcpListener statisticListener;
        Thread listenerThread;

        bool graphCreated;
        private List<plotgraph> graphTreeData;
        private int totalTree;
        private bool[] xmlWriting;


        public statisticFRm()
        {
            InitializeComponent();
            //xmldata = new plotgraph(@"C:\Documents and Settings\eeuser\Desktop\cevfyp\cevfyp\Client\Client\bin\Debug\Tree0", false);
            //xmldata = new plotgraph("Tree0", false);
            graphCreated = false;
            xmlWriting = null;
            localAddr = IPAddress.Parse(TcpApps.LocalIPAddress());
            totalTree = 0;
            nudPort.Value = TcpApps.RanPort(1701, 1800);


        }

        public void listenCommand()
        {
            statisticListener = new TcpListener(localAddr, statisticListen);
            statisticListener.Start();

            while (true)
            {
                TcpClient client = statisticListener.AcceptTcpClient();
                NetworkStream cstream = client.GetStream();
                //TcpClient.Client.RemoteEndPoint.   

                IPAddress clientendpt = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                try
                {
                    //Get the Peer type- server/client
                    byte[] responsePeerMsg = new byte[10];
                    cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                    string peertype = ByteArrayToString(responsePeerMsg);

                    if (peertype.Contains("<newCurve>"))
                    {
                        byte[] treeMsg = new byte[4];
                        cstream.Read(treeMsg, 0, treeMsg.Length);
                        totalTree = BitConverter.ToInt32(treeMsg, 0);
                        //responsePeerMsg = new byte[4];

                        //cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        //int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);

                        //byte[] responsePeerMsg2 = new byte[MsgSize];
                        //cstream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                        //string filePath = ByteArrayToString(responsePeerMsg2);

                        //string[] messages = MsgContent.Split('@');

                        //string filePath = messages[0];
                        //xmldata = new plotgraph(filePath);

                        xmldata = new plotgraph("");
                        xmlWriting = new bool[totalTree];
                        graphTreeData = new List<plotgraph>(totalTree);

                        for (int i = 0; i < totalTree; i++)
                        {
                            graphTreeData.Add(new plotgraph("Tree" + i, true));
                            xmlWriting[i] = false;
                        }

                        xmldata.CreateGraph(zedGraphCon, totalTree);
                        graphCreated = true;
                    }
                    else if (peertype.Contains("<renewCur>"))
                    {
                        //replay whether graph is created
                        byte[] createdbyte = BitConverter.GetBytes(graphCreated);
                        cstream.Write(createdbyte, 0, createdbyte.Length);

                        if (!graphCreated)//if graph is not created, skip below
                            continue;

                        byte[] treeMsg = new byte[4];
                        cstream.Read(treeMsg, 0, treeMsg.Length);
                        int tree = BitConverter.ToInt32(treeMsg, 0);

                        //addRecord
                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);

                        byte[] responsePeerMsg2 = new byte[MsgSize];
                        cstream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                        string MsgContent = ByteArrayToString(responsePeerMsg2);
                        string[] messages = MsgContent.Split('@');

                        //DateTime start = DateTime.Parse(messages[0]);
                        //DateTime end = DateTime.Parse(messages[1]);
                        DateTime start = DateTime.FromBinary(long.Parse(messages[0]));
                        DateTime end = DateTime.FromBinary(long.Parse(messages[1]));
                        int size = Int32.Parse(messages[2]);

                        if (xmlWriting[tree] == false)
                        {
                            xmlWriting[tree] = true;
                            graphTreeData[tree].AddRecord(start, end, size);
                            xmlWriting[tree] = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                    //Console.WriteLine(ex);
                }
                Thread.Sleep(50);
            }
            
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            for (int i = 0; i < totalTree; i++)
            {
                if (xmlWriting == null)//stop loop if no client connected
                    break;
                if (xmlWriting[i] == false)
                {
                    xmlWriting[i] = true;
                    xmldata.UpdateGraph(zedGraphCon, i);
                    xmlWriting[i] = false;
                }
            }

            this.zedGraphCon.AxisChange();
            this.zedGraphCon.Refresh();

            //CurrentIndex++;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            statisticListen = (Int32)nudPort.Value;

            listenerThread = new Thread(new ThreadStart(listenCommand));
            listenerThread.IsBackground = true;
            listenerThread.Name = " listen_for_peers";
            listenerThread.Start();
        }
    }
}
