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
                
        IPAddress localAddr;
        TcpListener statisticListener;
        
        Thread listenerThread;
        public delegate void UpdateTextCallback(string message);

        bool graphCreated;
        bool uploadGraphCreated;
        private List<plotgraph> graphTreeData;
        private List<plotgraph> uploadGraphTreeData;
        private int totalTree;
        private bool[] xmlWriting;
        private bool[] uploadXmlWriting;


        public statisticFRm()
        {
            InitializeComponent();

            //xmldata = new plotgraph(@"C:\Documents and Settings\eeuser\Desktop\cevfyp\cevfyp\Client\Client\bin\Debug\Tree0", false);
            //xmldata = new plotgraph("Tree0", false);

            localAddr = IPAddress.Parse(TcpApps.LocalIPAddress());
            totalTree = 0;

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
                        byte[] treeMsg = new byte[4];
                        cstream.Read(treeMsg, 0, treeMsg.Length);
                        totalTree = BitConverter.ToInt32(treeMsg, 0);

                        responsePeerMsg = new byte[8];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        string graphtype = ByteArrayToString(responsePeerMsg);

                        if (graphtype.Equals("download"))
                        {
                            xmldata = new plotgraph("Tree");
                            xmlWriting = new bool[totalTree];
                            graphTreeData = new List<plotgraph>(totalTree);
                            for (int i = 0; i < totalTree; i++)
                            {
                                graphTreeData.Add(new plotgraph("Tree" + i, true));
                                xmlWriting[i] = false;
                            }

                            xmldata.CreateGraph(zedGraphCon, totalTree, "Tree");
                            graphCreated = true;
                        }
                        else
                        {
                            uploadXmldata = new plotgraph("UploadTree");
                            uploadXmlWriting = new bool[totalTree];
                            uploadGraphTreeData = new List<plotgraph>(totalTree);
                            for (int i = 0; i < totalTree; i++)
                            {
                                uploadGraphTreeData.Add(new plotgraph("UploadTree" + i, true));
                                uploadXmlWriting[i] = false;
                            }
                            uploadXmldata.CreateGraph(zedGraphCon1, totalTree, "Port");
                            uploadGraphCreated = true;
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
                                this.UpLog.BeginInvoke(new UpdateTextCallback(this.updateUpLog), new object[] { "T:"+tree + " " + recordTime.TimeOfDay.ToString().Substring(0, 8) + " " + size.ToString() + "\n" });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
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
                        xmldata.UpdateGraph(zedGraphCon, i, "Tree");
                        xmlWriting[i] = false;
                    }
                    this.zedGraphCon.AxisChange();
                    this.zedGraphCon.Refresh();
                }
                if (uploadGraphCreated)
                {
                    for (int i = 0; i < totalTree; i++)
                    {
                        if (uploadXmlWriting[i] == false && uploadXmlWriting != null && uploadXmldata != null)
                        {
                            uploadXmlWriting[i] = true;
                            uploadXmldata.UpdateGraph(zedGraphCon1, i, "UploadTree");
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
            foreach (string sFile in System.IO.Directory.GetFiles(Directory.GetCurrentDirectory() , "*.xml"))
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
            }
            catch(Exception ex) {
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

        public void updateOD(int OD)
        {
            this.TOD.BeginInvoke(new UpdateTextCallback(this.updateTOD), new object[] { OD.ToString() });
        }

        public void updateOU(int OU)
        {
            this.TOU.BeginInvoke(new UpdateTextCallback(this.updateTOU), new object[] { OU.ToString() });
        }

        public void updateAvgDS(int AvgDown)
        {
            this.DT.BeginInvoke(new UpdateTextCallback(this.updateDownSpeed), new object[] { AvgDown.ToString() });
        }

        public void updateAvgUS(int Avgup)
        {
            this.UT.BeginInvoke(new UpdateTextCallback(this.updateUpSpeed), new object[] { Avgup.ToString() });
        }

        public void updatePL(int PL)
        {
            this.TPL.BeginInvoke(new UpdateTextCallback(this.updateTPL), new object[] { PL.ToString() });
        }

        public void updatePP(int PP)
        {
            this.TPP.BeginInvoke(new UpdateTextCallback(this.updateTPP), new object[] { PP.ToString() });
        }

        public void updatePM(int PM)
        {
            this.TPM.BeginInvoke(new UpdateTextCallback(this.updateTPM), new object[] { PM.ToString() });
        }

        public void updatePR(int PR)
        {
            this.TPR.BeginInvoke(new UpdateTextCallback(this.updateTPR), new object[] { PR.ToString() });
        }
    }
}
