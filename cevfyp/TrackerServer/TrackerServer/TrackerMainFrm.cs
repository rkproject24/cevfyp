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
        int[] maxID;
        const string Peerlist_name = "PeerInfoT";
        const int tlPort = 1500;
        const int deleteChildTime = 5000;

        //private xml PeerInfo;
        //private ServerConfig sConfig;
        IPAddress localAddr;
        TcpListener TrackerListen;
        Thread listenerThread;

        public List<Thread> unRegChildThread;
        List<ChildUnregHandler> unreghandlers;
        List<Thread> peerlistThreads;
        //int max_client;

       // List<PeerNode> peerList; //store all the Peer include server in a list

        public TrackerMainFrm()
        {
            //sConfig = new ServerConfig();
            //sConfig.load("C:\\ServerConfig");
            //peerList = new List<PeerNode>();
            treeNo= 0;
            InitializeComponent();
            unRegChildThread = new List<Thread>();
            peerlistThreads = new List<Thread>();
        }

        private void btnOn_Click(object sender, EventArgs e)
        {
            start();
            btnOn.Enabled = false;
            btnReset.Enabled = true;

            //testXMLsort.sort("PeerInfoT0.xml", "/Info/Peer", "Parentid");
            //PeerInfoAccessor TreeAccess = new PeerInfoAccessor("PeerInfoT0");
            //TreeAccess.load();
            //bool checkLoad = TreeAccess.sortLoad();
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            rtbClientlist.Text = "";
            cbbTree.Items.Clear();
            TreelistView.Clear();

            listenerThread.Abort();
            TrackerListen.Stop();
            //peerList.Clear();
            treeNo = 0;
            start();
        }

        private void listenForClients()
        {
            TrackerListen = new TcpListener(localAddr, tlPort);
            TrackerListen.Start();

            while (true)
            {
                TcpClient client = TrackerListen.AcceptTcpClient();
                   

                IPAddress clientendpt = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                //this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "New connection to from " + clientendpt.ToString() + "\n" });
                
                try
                {
                    NetworkStream cstream = client.GetStream();
                    //Get the Peer type- server/client
                    byte[] responsePeerMsg = new byte[11];
                    cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                    string peertype = ByteArrayToString(responsePeerMsg);

                    if (peertype.Contains("<clientReq>"))
                    {
                        byte[] treeMsg = new byte[4];
                        cstream.Read(treeMsg, 0, treeMsg.Length);
                        int treeNo = BitConverter.ToInt32(treeMsg, 0);

                        responsePeerMsg = new byte[4];
                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        int peerListPort = BitConverter.ToInt32(responsePeerMsg, 0);

                        cstream.Close();
                        client.Close();

                        if (File.Exists(Peerlist_name + treeNo + ".xml"))
                        {
                            
                            Thread peerlistThread = new Thread(new ThreadStart(delegate() { sendPeerList(peerListPort, clientendpt, treeNo); }));
                            peerlistThread.IsBackground = true;
                            peerlistThread.Name = "Peerlist_to_" + peerListPort;
                            peerlistThread.Start();
                            peerlistThreads.Add(peerlistThread);

                        }
                        else
                        {
                            byte[] peeripMsg = StrToByteArray("NOPEER");
                        }

                    }
                    else if (peertype.Contains("<cRegister>"))
                    {
                        responsePeerMsg = new byte[4];


                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);

                        byte[] responsePeerMsg2 = new byte[MsgSize];
                        cstream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                        //client.Close();
                        

                        string MsgContent = ByteArrayToString(responsePeerMsg2);
                        string[] messages = MsgContent.Split('@');

                        string IP = messages[0];
                        int listenPort = Int32.Parse(messages[1]);
                        int treeNo = Int32.Parse(messages[2]);
                        string clientid = messages[3];
                        int MaxClient = Int32.Parse( messages[4]);
                        int layer = Int32.Parse(messages[5]);
                        string parentid = messages[6];

                        //int MaxClient = BitConverter.ToInt16(responsePeerMsg, 0);

                        //byte[] responsePeerMsg1 = new byte[4];
                        //cstream.Read(responsePeerMsg1, 0, responsePeerMsg1.Length);

                        //int layer = Convert.ToInt32(BitConverter.ToString(responsePeerMsg1, 0));

                        PeerNode clientNode = new PeerNode(clientid, IP, MaxClient, listenPort, parentid);
                        clientNode.Layer = layer;

                        PeerInfoAccessor TreeAccess = new PeerInfoAccessor(Peerlist_name + treeNo);
                        //bool checkLoad = TreeAccess.load();

                        //delete peer old record before add peer
                        PeerNode p1 = new PeerNode(clientid, "deleting", 0, 0, "-1");
                        TreeAccess.deletePeer(p1);

                        
                        TreeAccess.addPeer(clientNode);
                        //if(TreeAccess.getMaxId() < Int32.Parse(clientid)) //to handle re-register for maxId
                        //    TreeAccess.setMaxId(Int32.Parse(clientid));

                        //stop the unRegchild thread
                        foreach(Thread thread in unRegChildThread)
                        {
                            if (thread.Name.Equals("ChildUnreg_Tree:" + treeNo + ":" + clientid))
                                thread.Abort();
                        }
                        this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "T[" + treeNo + "] Peer:" + clientNode.Id + " ip:" + clientendpt.ToString() + " connected to "+parentid+"\n" });
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

                        PeerNode serverNode = new PeerNode("0", clientendpt.ToString(), MaxClient, listenPort, "-1");
                        serverNode.Layer = 0;

                        maxID = new int[treeNo];
                        for (int i = 0; i < treeNo; i++)
                        {
                            PeerInfoAccessor TreeAccess = new PeerInfoAccessor(Peerlist_name + i);
                            //Thread.Sleep(400);
                            TreeAccess.initialize(treeNo);
                            //Thread.Sleep(400);
                            TreeAccess.addPeer(serverNode);

                            maxID[i] = 0;
                            this.cbbTree.BeginInvoke(new UpdateTextCallback(TreeComboBox), new object[] {"T"+i });
                        }
                        
                        this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Server " + clientendpt.ToString() + " started\n" });

                    }
                    else if (peertype.Contains("<unRegists>"))
                    {
                        responsePeerMsg = new byte[4];


                        cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                        int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);

                        byte[] responsePeerMsg2 = new byte[MsgSize];
                        cstream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                        string MsgContent = ByteArrayToString(responsePeerMsg2);
                        string[] messages = MsgContent.Split('@');
                        int tree = Int32.Parse(messages[0]);
                        string peerId = messages[1];
                        string senderId = messages[2];
                        //string peerParentId = messages[2];
                        //==incompleted

                        PeerInfoAccessor TreeAccess = new PeerInfoAccessor(Peerlist_name + tree);
                        bool checkLoad = TreeAccess.load();

                        PeerNode p1 = TreeAccess.getPeer(peerId);
                        if (p1 == null)
                            continue;

                        //TreeAccess.deletePeer(p1);
                        //this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Peer:" + peerId + " unregister from tree:" + tree + "\n" });

                        if (p1.Parentid.Equals(senderId))
                        {
                            ////stop the unRegchild thread
                            //foreach (Thread thread in unRegChildThread)
                            //{
                            //    if (thread.Name.Equals("ChildUnreg_Tree:" + tree + ":" + senderId))
                            //        thread.Abort();
                            //}

                            TreeAccess.deletePeer(p1);

                            //PeerInfoAccessor treeAccessor = new PeerInfoAccessor(Peerlist_name + tree);
                            //List<PeerNode> childPeerList = TreeAccess.getPeersByParent(peerId);

                            //foreach (PeerNode child in childPeerList)
                            //{
                            //    if (child != null)
                            //    {
                            //        Thread unRegChild = new Thread(delegate() { new ChildUnregHandler(this, tree, child.Id, deleteChildTime, Peerlist_name); });
                            //        unRegChild.IsBackground = true;
                            //        unRegChild.Name = "ChildUnreg_Tree:" + tree + ":" + child.Id;
                            //        unRegChild.Start();
                            //        //Thread.Sleep(20);
                            //        unRegChildThread.Add(unRegChild);
                            //    }
                            //}

                            //p1 = TreeAccess.getPeer("1");
                            //if (p1 == null)
                            //    Console.WriteLine("Nothings");
                            this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Peer:" + peerId + " unregister from tree:" + tree + "by Peer:" + senderId + "\n" });
                        }
                    }
                    else if (peertype.Contains("<treeSizes>"))
                    {
                        byte[] treeNoByte = BitConverter.GetBytes(treeNo);
                        cstream.Write(treeNoByte, 0, treeNoByte.Length);
                    }
                    //else if (peertype.Contains("<changePar>"))
                    //{
                    //    responsePeerMsg = new byte[4];


                    //    cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                    //    int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);

                    //    byte[] responsePeerMsg2 = new byte[MsgSize];
                    //    cstream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                    //    string MsgContent = ByteArrayToString(responsePeerMsg2);
                    //    string[] messages = MsgContent.Split('@');

                    //    string tree = messages[0];
                    //    string peerId = messages[1];
                    //    string newParentId = messages[2];


                    //    bool changed = changeParent(tree, peerId, newParentId); //reply whether change parent sucess
                    //    byte[] changedbyte = BitConverter.GetBytes(changed);
                    //    cstream.Write(changedbyte, 0, changedbyte.Length);

                    //    this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "Peer:" + peerId + " change to Parent:" + newParentId + " in tree:" + tree + "\n" });
                    //}

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
            //maxID = 0;
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
            TreelistView.Columns.Add("ID",30);
            TreelistView.Columns.Add("IP Address",80);
            TreelistView.Columns.Add("ListenPort");
            //TreelistView.Columns.Add("Layer");
            TreelistView.Columns.Add("ParentID");
        }

        private void updateTreelistView(int tree)
        {
            TreelistView.Items.Clear();

            PeerInfoAccessor TreeAccess = new PeerInfoAccessor(Peerlist_name + tree);
            bool checkLoad = TreeAccess.load();

            for (int i = 0; i <= TreeAccess.getMaxId(); i++)
            {
                PeerNode displayNode = TreeAccess.getPeer(i.ToString());
                if (displayNode == null) //skip if the peer not exist in the list
                    continue;
                ListViewItem item = new ListViewItem(displayNode.Id, i);
                item.SubItems.Add(displayNode.Ip);
                item.SubItems.Add(displayNode.ListenPort.ToString());
                //item.SubItems.Add(displayNode.Layer.ToString());
                item.SubItems.Add(displayNode.Parentid.ToString());
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

        //public bool changeParent(string tree, string peerId, string newParentId)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            PeerInfoAccessor TreeAccess = new PeerInfoAccessor(Peerlist_name + tree);
        //            //bool checkLoad = TreeAccess.load();

        //            PeerNode p1 = TreeAccess.getPeer(peerId);
        //            if (p1 != null)
        //            {
        //                TreeAccess.deletePeer(p1);
        //                p1.Parentid = newParentId;
        //                //p1.ChangeParent = true;//mark the parent is just changed
        //                TreeAccess.addPeer(p1);
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            Thread.Sleep(20);
        //            this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "T[" + tree + "] Peer:" + peerId + " change Parent fail\n" });
        //            //return false;
        //        }
        //        return true;
        //    }
        //    return false;
            
        //}

        public void sendPeerList(int peerlistPort, IPAddress ip, int tree)
        {
            try
            {
            TcpClient trackerTcpClient = new TcpClient(ip.ToString(), peerlistPort);
            NetworkStream cstream = trackerTcpClient.GetStream();
           
                byte[] recoonectMsg = new byte[1];
                cstream.Read(recoonectMsg, 0, recoonectMsg.Length);
                bool recoonect = BitConverter.ToBoolean(recoonectMsg, 0);

                byte[] responsePeerMsg;
                if (recoonect)
                {
                    responsePeerMsg = new byte[4];
                    cstream.Read(responsePeerMsg, 0, responsePeerMsg.Length);
                   // int MsgSize = BitConverter.ToInt32(responsePeerMsg, 0);
                    string peerId = BitConverter.ToInt32(responsePeerMsg, 0).ToString();

                    //byte[] responsePeerMsg2 = new byte[MsgSize];
                    //cstream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                    //string peerId = ByteArrayToString(responsePeerMsg2);

                    //changeParent(treeNo.ToString(), peerId, "-2"); //-2 indicate the peer is reconnecting
                }
                else
                {
                    //PeerInfoAccessor TreeAccess = new PeerInfoAccessor(fileName);
                    //while (!TreeAccess.load())
                    //{
                    //    Thread.Sleep(10);
                    //}
                    //int newID = TreeAccess.getMaxId() + 1;
                    //while (!TreeAccess.setMaxId(newID))
                    //    Thread.Sleep(20);

                    maxID[tree]++;
                    PeerInfoAccessor TreeAccess = new PeerInfoAccessor(Peerlist_name + tree);
                    while (!TreeAccess.setMaxId(maxID[tree]))
                        Thread.Sleep(20);


                    byte[] newIDbyte = BitConverter.GetBytes(maxID[tree]);
                    cstream.Write(newIDbyte, 0, newIDbyte.Length);
                }

                FileStream file = new FileStream(Peerlist_name + tree + ".xml", FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader sr = new StreamReader(file);
                string s1 = sr.ReadToEnd();
                sr.Close();
                file.Close();

                byte[] peeripMsg = StrToByteArray(s1);

                //TcpClient trackerTcpClient = new TcpClient(ip.ToString(), peerlistPort);
                //NetworkStream cstream = trackerTcpClient.GetStream();

                byte[] MsgLength = BitConverter.GetBytes(peeripMsg.Length);
                cstream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                cstream.Write(peeripMsg, 0, peeripMsg.Length);
                cstream.Close();
            }
            catch(Exception ex)
            {
                this.rtbClientlist.BeginInvoke(new UpdateTextCallback(UpdatertbClientlist), new object[] { "fail to send Peerlist:" + Peerlist_name + tree + "\n" });
            }
        }

    }
}
