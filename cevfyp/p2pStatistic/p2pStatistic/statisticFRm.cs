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
        plotgraph uploadXmldata;
        //int CurrentIndex = 0;
        int statisticListen;
        int currentDLogIndex;
        int currentULogIndex;

        IPAddress localAddr;
        TcpListener statisticListener;

        Thread listenerThread;
        public delegate void UpdateTextCallback(string message);
        public delegate void ResetTextCallback();

        bool graphCreated;
        bool uploadGraphCreated;
        private List<plotgraph> graphTreeData;
        private List<plotgraph> uploadGraphTreeData;
        private int totalTree;
        private int totalUpPort;
        private bool[] xmlWriting;
        private bool[] uploadXmlWriting;


        public statisticFRm()
        {
            InitializeComponent();

            //xmldata = new plotgraph(@"C:\Documents and Settings\eeuser\Desktop\cevfyp\cevfyp\Client\Client\bin\Debug\Tree0", false);
            //xmldata = new plotgraph("Tree0", false);

            localAddr = IPAddress.Parse(TcpApps.LocalIPAddress());
            totalTree = 0;
            totalUpPort = 0;

            graphCreated = false;
            xmlWriting = null;
            nudPort.Value = TcpApps.RanPort(1701, 1750);

            uploadGraphCreated = false;
            uploadXmlWriting = null;
            //upPort.Text = ((Int32)nudPort.Value + 50).ToString();
        }

        public void listenCommand()
        {
            statisticListener = new TcpListener(localAddr, statisticListen);
            statisticListener.Start();

            while (true)
            {
                try
                {
                    TcpClient client = statisticListener.AcceptTcpClient();
                    NetworkStream cstream = client.GetStream();
                    //TcpClient.Client.RemoteEndPoint.   
                    cstream.ReadTimeout = 1000;
                    IPAddress clientendpt = ((IPEndPoint)client.Client.RemoteEndPoint).Address;

                    //Get the Peer type- server/client
                    byte[] responsePeerMsg = new byte[10];
                    cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    string peertype = ByteArrayToString(responsePeerMsg);

                    if (peertype.Contains("<newCurve>"))
                    {


                        responsePeerMsg = new byte[8];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        string graphtype = ByteArrayToString(responsePeerMsg);


                        if (graphtype.Equals("download") && !graphCreated)
                        {
                            graphCreated = true;

                            byte[] treeMsg = new byte[4];
                            cstream.Read(treeMsg, 0, treeMsg.Length);
                            totalTree = BitConverter.ToInt32(treeMsg, 0);

                            //if (!(File.Exists("downloadNum.txt")))
                            //{
                            //    //File.Create("downloadNum.txt");
                            //    //File.OpenWrite("downloadNum.txt");
                            //    currentDLogIndex = 1;
                            //    Directory.CreateDirectory("download" + currentDLogIndex.ToString());
                            //    File.WriteAllText("downloadNum.txt", currentDLogIndex.ToString());
                            //}
                            //else 
                            //{
                            //    currentDLogIndex = Convert.ToInt32(File.ReadAllText("downloadNum.txt"));
                            //    currentDLogIndex++;
                            //    Directory.CreateDirectory("download" + currentDLogIndex.ToString());
                            //    File.WriteAllText("downloadNum.txt", currentDLogIndex.ToString());
                            //}
                            byte[] idMsg = new byte[4];
                            cstream.Read(idMsg, 0, idMsg.Length);
                            currentDLogIndex = BitConverter.ToInt32(idMsg, 0);
                            if (Directory.Exists("download" + currentDLogIndex.ToString()))
                                Directory.Delete("download" + currentDLogIndex.ToString(), true);
                            Directory.CreateDirectory("download" + currentDLogIndex.ToString());

                            xmldata = new plotgraph("download" + currentDLogIndex.ToString() + "\\Tree");
                            xmlWriting = new bool[totalTree];
                            graphTreeData = new List<plotgraph>(totalTree);
                            for (int i = 0; i < totalTree; i++)
                            {
                                graphTreeData.Add(new plotgraph("download" + currentDLogIndex.ToString() + "\\Tree" + i, true));
                                xmlWriting[i] = false;
                            }

                            xmldata.CreateGraph(zedGraphCon, totalTree, "Tree");

                        }
                        else if (!uploadGraphCreated)
                        {
                            byte[] treeMsg = new byte[4];
                            cstream.Read(treeMsg, 0, treeMsg.Length);
                            totalUpPort = BitConverter.ToInt32(treeMsg, 0);

                            uploadGraphCreated = true;
                            //if (!(File.Exists("uploadNum.txt")))
                            //{
                            //    //File.Create("downloadNum.txt");
                            //    //File.OpenWrite("downloadNum.txt");
                            //    currentULogIndex = 1;
                            //    Directory.CreateDirectory("upload" + currentULogIndex.ToString());
                            //    File.WriteAllText("uploadNum.txt", currentULogIndex.ToString());
                            //}
                            //else
                            //{
                            //    currentULogIndex = Convert.ToInt32(File.ReadAllText("uploadNum.txt"));
                            //    currentULogIndex++;
                            //    Directory.CreateDirectory("upload" + currentULogIndex.ToString());
                            //    File.WriteAllText("uploadNum.txt", currentULogIndex.ToString());
                            //}
                            byte[] idMsg = new byte[4];
                            cstream.Read(idMsg, 0, idMsg.Length);
                            currentULogIndex = BitConverter.ToInt32(idMsg, 0);
                            if (Directory.Exists("upload" + currentULogIndex.ToString()))
                                Directory.Delete("upload" + currentULogIndex.ToString(), true);
                            Directory.CreateDirectory("upload" + currentULogIndex.ToString());

                            uploadXmldata = new plotgraph("upload" + currentULogIndex.ToString() + "\\UploadTree");
                            uploadXmlWriting = new bool[totalUpPort];
                            uploadGraphTreeData = new List<plotgraph>(totalUpPort);
                            for (int i = 0; i < totalUpPort; i++)
                            {
                                uploadGraphTreeData.Add(new plotgraph("upload" + currentULogIndex.ToString() + "\\UploadTree" + i, true));
                                uploadXmlWriting[i] = false;
                            }
                            uploadXmldata.CreateGraph(zedGraphCon1, totalUpPort, "Port");

                        }

                    }
                    else if (peertype.Contains("<renewCur>"))
                    {
                        responsePeerMsg = new byte[8];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        string graphtype = ByteArrayToString(responsePeerMsg);

                        byte[] createdbyte;
                        //replay whether graph is created
                        if (graphtype.Equals("download"))
                        {
                            createdbyte = BitConverter.GetBytes(graphCreated);
                            cstream.Write(createdbyte, 0, createdbyte.Length);
                            if (!graphCreated)//if graph is not created, skip below
                                continue;
                        }
                        else
                        {
                            createdbyte = BitConverter.GetBytes(uploadGraphCreated);
                            cstream.Write(createdbyte, 0, createdbyte.Length);
                            if (!uploadGraphCreated)//if graph is not created, skip below
                                continue;
                        }

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

                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        int MsgSize2 = BitConverter.ToInt32(responsePeerMsg, 0);
                        byte[] responseStatMessage = new byte[MsgSize2];
                        cstream.Read(responseStatMessage, 0, responseStatMessage.Length);
                        string statD = ByteArrayToString(responseStatMessage);
                        string[] statS = statD.Split('@');
                        updatestat(statS);

                        if (graphtype.Equals("download"))
                        {
                            DateTime start = DateTime.FromBinary(long.Parse(messages[0]));
                            DateTime end = DateTime.FromBinary(long.Parse(messages[1]));
                            int size = Int32.Parse(messages[2]);

                            if (xmlWriting[tree] == false)
                            {
                                xmlWriting[tree] = true;
                                graphTreeData[tree].AddRecord(start, end, size);
                                xmlWriting[tree] = false;

                                this.UpLog.BeginInvoke(new UpdateTextCallback(this.updateDownLog), new object[] { "T" + tree + " S:" + start.TimeOfDay.ToString().Substring(0, 8) + " E:" + end.TimeOfDay.ToString().Substring(0, 8) + "\n" });
                                //this.DT1.inInvoke(new UpdateTextCallback(this.DT1), new object[] { size });
                            }
                        }
                        else
                        {
                            DateTime recordTime = DateTime.FromBinary(long.Parse(messages[1]));
                            int size = Int32.Parse(messages[2]);

                            if (uploadXmlWriting[tree] == false)
                            {
                                uploadXmlWriting[tree] = true;
                                uploadGraphTreeData[tree].AddRecord(recordTime, size);
                                uploadXmlWriting[tree] = false;
                                this.UpLog.BeginInvoke(new UpdateTextCallback(this.updateUpLog), new object[] { "T:" + tree + " " + recordTime.TimeOfDay.ToString().Substring(0, 8) + " " + size.ToString() + "\n" });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    Console.WriteLine(ex);
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
            try
            {
                if (graphCreated)
                    for (int i = 0; i < totalTree; i++)
                    {
                        if (xmlWriting[i] == false && xmlWriting != null && xmldata != null)
                        {
                            xmlWriting[i] = true;
                            xmldata.UpdateGraph(zedGraphCon, i, "download" + currentDLogIndex.ToString() + "\\Tree");
                            xmlWriting[i] = false;
                        }
                        this.zedGraphCon.AxisChange();
                        this.zedGraphCon.Refresh();
                    }
                if (uploadGraphCreated)
                {
                    for (int i = 0; i < totalUpPort; i++)
                    {
                        if (uploadXmlWriting[i] == false && uploadXmlWriting != null && uploadXmldata != null)
                        {
                            uploadXmlWriting[i] = true;
                            uploadXmldata.UpdateGraph(zedGraphCon1, i, "upload" + currentULogIndex.ToString() + "\\UploadTree");
                            uploadXmlWriting[i] = false;
                        }
                    }
                    this.zedGraphCon1.AxisChange();
                    this.zedGraphCon1.Refresh();
                }
                //CurrentIndex++;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Console.WriteLine(ex);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //InitializeComponent();
            //graphCreated = false;
            //xmlWriting = null;
            //localAddr = IPAddress.Parse(TcpApps.LocalIPAddress());
            //totalTree = 0;
            //nudPort.Value = TcpApps.RanPort(1701, 1800);
            //if (File.Exists("*.xml"))
            //    File.Delete("*.xml");
            foreach (string sFile in System.IO.Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml"))
            {
                System.IO.File.Delete(sFile);
            }
            statisticListen = (Int32)nudPort.Value;
            //uploadStatisticListen = Convert.ToInt32( upPort.Text);
            btnStart.Enabled = false;

            listenerThread = new Thread(new ThreadStart(listenCommand));
            listenerThread.IsBackground = true;
            listenerThread.Name = " listen_for_peers";
            listenerThread.Start();


            //uploadListenerThread = new Thread(new ThreadStart(uploadListenCommand));
            //uploadListenerThread.IsBackground = true;
            //uploadListenerThread.Name = " listen_for_uploadpeers";
            //uploadListenerThread.Start();
            timer1.Enabled = true;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                btnStart.Enabled = true;
                timer1.Enabled = false;

                statisticListener.Stop();
                listenerThread.Abort();


                //zedGraphCon.GraphPane.CurveList.Clear();
                //zedGraphCon.MasterPane = new ZedGraph.MasterPane();
                if (graphCreated)
                {
                    xmldata.clearGraph(zedGraphCon);
                    this.zedGraphCon.Refresh();
                }

                if (uploadGraphCreated)
                {
                    uploadXmldata.clearGraph(zedGraphCon1);
                    this.zedGraphCon1.Refresh();
                }

                graphCreated = false;
                uploadGraphCreated = false;
                //if (File.Exists("*.xml"))
                //    File.Delete("*.xml");
                foreach (string sFile in System.IO.Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml"))
                {
                    System.IO.File.Delete(sFile);
                }

                this.UpLog.BeginInvoke(new ResetTextCallback(this.resetUpLog), new object[] { });
                this.DownLog.BeginInvoke(new ResetTextCallback(this.resetDownLog), new object[] { });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private int AvgSpeedCalculation(int[] speed)
        {
            int sum = 0;
            for (int i = 0; i < totalTree; i++)
            {
                sum += speed[i];
            }
            return (sum / totalTree);
        }

        public void updatestat(string[] stat)
        {
            //string[] statS = new string [7];

            //for (int i = 0; i < 7; i++)
            //{
            //    statS[i] = stat[i].ToString();
            //}

            this.label2.BeginInvoke(new UpdateTextCallback(this.updatel2), new object[] { stat[0] });
            this.label3.BeginInvoke(new UpdateTextCallback(this.updatel3), new object[] { stat[1] });
            this.label4.BeginInvoke(new UpdateTextCallback(this.updatel4), new object[] { stat[2] });
            this.label5.BeginInvoke(new UpdateTextCallback(this.updatel5), new object[] { stat[3] });
            this.label6.BeginInvoke(new UpdateTextCallback(this.updatel6), new object[] { stat[4] });
            this.label7.BeginInvoke(new UpdateTextCallback(this.updatel7), new object[] { stat[5] });
            this.label8.BeginInvoke(new UpdateTextCallback(this.updatel8), new object[] { stat[6] });
        }

    }
}
