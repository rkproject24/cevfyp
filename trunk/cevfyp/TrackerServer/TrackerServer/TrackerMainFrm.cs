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
    public partial class TrackerMainFrm : Form
    {
        int treeNo;
        const string Peerlist_name = "PeerInfoT";
        const int tlPort = 1500;

        private xml PeerInfo;
        //private ServerConfig sConfig;
        IPAddress localAddr;
        TcpListener TrackerListen;
        Thread listenerThread;
        int max_client;

       // List<PeerNode> peerList; //store all the Peer include server in a list

        public TrackerMainFrm()
        {
            //sConfig = new ServerConfig();
            //sConfig.load("C:\\ServerConfig");
            //peerList = new List<PeerNode>();
            treeNo= 0;        
            InitializeComponent();
        }

        private void btnOn_Click(object sender, EventArgs e)
        {
            start();
            btnOn.Enabled = false;
            btnReset.Enabled = true;
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            rtbClientlist.Text = "";
            cbbTree.Items.Clear();
            TreelistView.Columns.Clear();

            listenerThread.Abort();
            TrackerListen.Stop();
            //peerList.Clear();

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

                    if (peertype.Contains("<clientReq>"))
                    {
                        byte[] treeMsg = new byte[4];
                        cstream.Read(treeMsg, 0, treeMsg.Length);
                        int treeNo = BitConverter.ToInt32(treeMsg, 0);

                        byte[] peeripMsg;
                        //if (File.Exists(Peerlist_name + "1" + ".xml"))
                        if (File.Exists(Peerlist_name + treeNo + ".xml"))
                        {
                            FileStream file = new FileStream(Peerlist_name + treeNo+ ".xml", FileMode.OpenOrCreate, FileAccess.Read);
                            StreamReader sr = new StreamReader(file);
                            string s1 = sr.ReadToEnd();
                            sr.Close();
                            file.Close();


                            //file = new FileStream(Peerlist_name + "2.xml", FileMode.OpenOrCreate, FileAccess.Read);
                            //sr = new StreamReader(file);
                            //string s2 = sr.ReadToEnd();
                            //sr.Close();
                            //file.Close();

                            //peeripMsg = StrToByteArray(s1 + "@" + s2);
                            peeripMsg = StrToByteArray(s1);

                        }
                        else
                        {
                            peeripMsg = StrToByteArray("NOPEER");
                        }
                        byte[] MsgLength = BitConverter.GetBytes(peeripMsg.Length);
                        cstream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                        cstream.Write(peeripMsg, 0, peeripMsg.Length);

                    }
                    else if (peertype.Contains("<clientReg>"))
                    {
                        responsePeerMsg = new byte[4];


                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);

                        byte[] responsePeerMsg2 = new byte[MsgSize];
                        cstream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                        string MsgContent = ByteArrayToString(responsePeerMsg2);
                        string[] messages = MsgContent.Split('@');

                        int listenPort = Int32.Parse(messages[0]);
                        int treeNo = Int32.Parse(messages[1]);
                        string clientid = messages[2];
                        int MaxClient = Int32.Parse( messages[3]);
                        int layer = Int32.Parse(messages[4]);

                        //int MaxClient = BitConverter.ToInt16(responsePeerMsg, 0);

                        //byte[] responsePeerMsg1 = new byte[4];
                        //cstream.Read(responsePeerMsg1, 0, responsePeerMsg1.Length);

                        //int layer = Convert.ToInt32(BitConverter.ToString(responsePeerMsg1, 0));

                        PeerNode clientNode = new PeerNode(clientid, clientendpt.ToString(), MaxClient, listenPort);
                        clientNode.Layer = layer;

                        PeerInfoAccessor TreeAccess = new PeerInfoAccessor(Peerlist_name + treeNo);
                        //Thread.Sleep(400);
                        TreeAccess.addPeer(clientNode);
                        //Thread.Sleep(400);
                        TreeAccess.setMaxId(Int32.Parse(clientid));

                        //PeerNode clientNodeB = new PeerNode(clientid, clientendpt.ToString(), MaxClient);
                        //clientNodeB.Layer = layerB;
                        //PeerInfoAccessor TreeAccess2 = new PeerInfoAccessor(Peerlist_name + "1");
                        //TreeAccess2.addPeer(clientNodeB);
                        //TreeAccess2.setMaxId(Int32.Parse(clientid));

                        ////PeerInfo.modify("Info", "DataNo", DataNo.ToString());

                        //PeerInfo = new xml("PeerInfoT1.xml", "Info");
                        //int DataNum = PeerInfo.GetElementNum();
                        //DataNum++;
                        ////MessageBox.Show(layer.ToString());
                        //layer++;
                        ////MessageBox.Show(layer.ToString());
                        //string[] InfoN1 = { "IP", "Layer" };
                        //string[] Value1 = { clientNode.Ip.ToString(), layer.ToString() };
                        //PeerInfo.Add(DataNum.ToString(), InfoN1, Value1);



                        //responsePeerMsg1 = new byte[4];
                        //cstream.Read(responsePeerMsg1, 0, responsePeerMsg1.Length);

                        //layer = BitConverter.ToInt16(responsePeerMsg1, 0);
                        //PeerInfo = new xml("PeerInfoT2.xml", "Info");

                        ////int cmdsize = BitConverter.ToInt16(responsePeerMsg, 0);
                        //DataNum = PeerInfo.GetElementNum();
                        //DataNum++;
                        //layer++;
                        //string[] InfoN2 = { "IP", "Layer" };
                        //string[] Value2 = { clientNode.Ip.ToString(), layer.ToString() };
                        //PeerInfo.Add(DataNum.ToString(), InfoN2, Value2);

                        //peerList.Add(clientNode);

                        this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { treeNo +": Peer:" + clientNode.Id +" ip:" + clientendpt.ToString() + " connected to Peer\n" });
                    }
                    else if (peertype.Contains("<serverReg>"))
                    {
                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        int listenPort = BitConverter.ToInt16(responsePeerMsg, 0);

                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        treeNo = BitConverter.ToInt16(responsePeerMsg, 0);

                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        int MaxClient = BitConverter.ToInt16(responsePeerMsg, 0);

                        PeerNode serverNode = new PeerNode("0", clientendpt.ToString(), MaxClient, listenPort);
                        serverNode.Layer = 0;

                        //by vinci:                       
                        //for (int i = 0; i < treeNo; i++)
                        //{
                        //    //remove the old xml
                        //    if (File.Exists(Peerlist_name + i + ".xml"))
                        //        File.Delete(Peerlist_name + i + ".xml");
                        //}
                        
                        for (int i = 0; i < treeNo; i++)
                        {
                            PeerInfoAccessor TreeAccess = new PeerInfoAccessor(Peerlist_name + i);
                            //Thread.Sleep(400);
                            TreeAccess.initialize(treeNo);
                            //Thread.Sleep(400);
                            TreeAccess.addPeer(serverNode);

                            this.cbbTree.BeginInvoke(new UpdateTextCallback(TreeComboBox), new object[] {"T"+i });
                        }



                        this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Server " + clientendpt.ToString() + " started\n" });

                    }

                }
                catch(Exception ex)
                {
                    //this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "One client join fail...\n" });
                    this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { ex+"\n" });
                    //break;
                }
                Thread.Sleep(50);
            }
        }

        private void start()
        {
            string[] xmlList = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");
            foreach (string xmlfile in xmlList)
            {
                File.Delete(xmlfile);
            }
            //this.max_client = sConfig.MaxClient;

            localAddr = IPAddress.Parse(TcpApps.LocalIPAddress());
            //localAddr= new  IPAddress.Parse("")

            listenerThread = new Thread(new ThreadStart(listenForClients));
            listenerThread.IsBackground = true;
            listenerThread.Name = " listen_for_clients";
            listenerThread.Start();

            buildlistView();
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

        public void TreeComboBox(string message)
        {
            cbbTree.Items.Add(message);
        }

        private void buildlistView()
        {
            // Create a new ListView control.
            //ListView listView1 = new ListView();
            //listView1.Bounds = new Rectangle(new Point(10, 10), new Size(300, 200));

            // Set the view to show details.
            TreelistView.View = View.Details;
            // Allow the user to edit item text.
            TreelistView.LabelEdit = false;
            // Allow the user to rearrange columns.
            TreelistView.AllowColumnReorder = false;
            // Display check boxes.
            //TreelistView.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            TreelistView.FullRowSelect = true;
            // Display grid lines.
            TreelistView.GridLines = true;
            // Sort the items in the list in ascending order.
            TreelistView.Sorting = SortOrder.Ascending;

            // Create columns for the items and subitems.
            TreelistView.Columns.Add("ID",40);
            TreelistView.Columns.Add("IP Address",80);
            TreelistView.Columns.Add("ListenPort");
            TreelistView.Columns.Add("Layer");
        }

        private void updateTreelistView(int tree)
        {
            TreelistView.Items.Clear();

            PeerInfoAccessor TreeAccess = new PeerInfoAccessor(Peerlist_name + tree);
            for (int i = 0; i <= TreeAccess.getMaxId(); i++)
            {
                PeerNode displayNode = TreeAccess.getPeer(i.ToString());
                ListViewItem item = new ListViewItem(displayNode.Id, i);
                item.SubItems.Add(displayNode.Ip);
                item.SubItems.Add(displayNode.ListenPort.ToString());
                item.SubItems.Add(displayNode.Layer.ToString());

                TreelistView.Items.Add(item);
            }


            //// Create three items and three sets of subitems for each item.
            //ListViewItem item1 = new ListViewItem("Peer1", 0);
            //// Place a check mark next to the item.
            ////item1.Checked = true;
            //item1.SubItems.Add("1");
            //item1.SubItems.Add("2");
            //item1.SubItems.Add("3");


            //Add the items to the ListView.
            //TreelistView.Items.AddRange(new ListViewItem[] { item1, item2, item3 });
            //TreelistView.Items.AddRange(item);
        }

        private void cbbTree_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateTreelistView(cbbTree.SelectedIndex);
        }

    }
}
