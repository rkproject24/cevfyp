using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ClassLibrary;

namespace Server
{
    class ServerHandler
    {
        //static int SLPORT = 1100;       //server listen port
        //static int DPORT_BASE = 1200;    //streaming data port
        //static int RECV_STREAMING_SIZE = 3000;  //receive streaming packet from vlc server
        //static int SEND_CHUNK_SIZE = 3182;      //send chunk packet to client
        //static int TREE_NO = 2;             //number of tree

        //static int TrackerSLPort = 1500;
        //const bool loop = true;

        int slPort;
        int max_client;
        int max_tree;
        int seqNumber = 1;

        TcpListener listenServer;
        Thread listenerThread;
        Thread getStreamingThread;
        Thread reStreamingThread;

        VlcHandler vlc = new VlcHandler();
        PortHandler ph;
        ChunkHandler ch;
        private ServerConfig sConfig;

        IPAddress localAddr;

        private ServerFrm mainFm;
       
        public ServerHandler(ServerFrm mainFm)
        {
            this.mainFm = mainFm;
            //sConfig = new ServerConfig(mainFm.tblibsrc.Text, "ts", mainFm.tbfilesrc.Text, 3, 1234, SLPORT, DPORT_BASE, 1301, TREE_NO);
            //sConfig.save("tempconfig.xml");

            sConfig = new ServerConfig();
            sConfig.load("C:\\ServerConfig");
            reloadUI();
            //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "startport be4\n" });
            this.max_client = sConfig.MaxClient;
            this.max_tree = sConfig.TreeSize;
            ph = new PortHandler(max_client, max_tree, mainFm.tbServerIp.Text, mainFm);
          
        }

        //By Vinci
        public void reloadUI()
        {
            mainFm.tbMaxClient.Text = sConfig.MaxClient.ToString();
            //mainFm.tbServerIp.Text = sConfig.Serverip;
            mainFm.tbServerIp.Text = TcpApps.LocalIPAddress();
            mainFm.tbfilesrc.Text = sConfig.VideoDir;
            mainFm.tbTracker.Text = sConfig.Trackerip;
        }


        private delegate void UpdateTextCallback(string message);

        public void play()
        {

            vlc.streaming(mainFm.panel1, mainFm.tbfilesrc.Text);//, sConfig.PluginPath);
            if (getStreamingThread != null) getStreamingThread.Abort(); //by vinci

            ch = new ChunkHandler();

            getStreamingThread = new Thread(new ThreadStart(getStreaming));
            getStreamingThread.IsBackground = true;
            getStreamingThread.Name = "get_Streaming";
            Thread.Sleep(100);
            getStreamingThread.Start();
            
            for (int i = 0; i < max_tree; i++)
            {
                ph.setTreeCLState(i, 1);

            }
        }


        public void pause()
        {
            //vlc.pause();
            mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { "" });
            mainFm.richTextBox1.Clear();
            mainFm.richTextBox2.Clear();
           
        }

        public void stop(bool manualStop)
        {
            vlc.stop(manualStop);
            //seqNumber = 1;
           // mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { "" });
            //mainFm.richTextBox1.Clear();
            getStreamingThread.Abort(); //by vinci
        }

        public void replay()
        {
            while (true)
            {
                if (checkEnd && mainFm.cbRepeat.Checked)
                {
                    stop(false);
                    //vlc = new VlcHandler();
                    //Thread.Sleep(3000);
                    //mainFm.panel1 = new System.Windows.Forms.Panel();
                    play();
                    checkEnd = false;
                }

                Thread.Sleep(100);
            }

        }


        public bool start()
        {
            sConfig.load("C:\\ServerConfig");
            //by vinci: update the online status to Tracker
            if (!UpdateTracker())
            {
                mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "Tracker unreachable!\n" });
            }
            else
            {
                //mainFm.tbMaxClient.Text = sConfig.MaxClient.ToString();
                //mainFm.tbServerIp.Text = sConfig.Serverip;
                
                //reloadUI();
                ////mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "startport be4\n" });
                //this.max_client = sConfig.MaxClient;
                //this.max_tree = sConfig.TreeSize;
                //ph = new PortHandler(max_client, max_tree, mainFm.tbServerIp.Text, mainFm);
                ph.startTreePort();
                //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "startport after\n" });

                localAddr = IPAddress.Parse(mainFm.tbServerIp.Text);
                listenerThread = new Thread(new ThreadStart(listenForClients));
                listenerThread.IsBackground = true;
                listenerThread.Name = " listen_for_clients";
                listenerThread.Start();

                reStreamingThread = new Thread(new ThreadStart(replay));
                reStreamingThread.IsBackground = true;
                reStreamingThread.Name = "re_Streaming";
                Thread.Sleep(100);
                reStreamingThread.Start();


                return true;
            }
            return false;
        }

        public void mute()
        {
            int checkMute = vlc.getMute();

            if (checkMute == 0)
                vlc.setMute(1);
            else
                vlc.setMute(0);
        }

        //private String readStreamToString(NetworkStream stream, int size)
        //{
        //    byte[] responseMessage = new byte[size];
        //    Int32 responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
        //    return System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);
        //}

        private bool UpdateTracker()
        {
            TcpClient trackerTcp = null;
            NetworkStream trackerStream = null;


            try
            {
                trackerTcp = new TcpClient(mainFm.tbTracker.Text, sConfig.TrackerPort);
                trackerStream = trackerTcp.GetStream();

                //define server type
                byte[] clienttype = StrToByteArray("<serverReg>");
                trackerStream.Write(clienttype, 0, clienttype.Length);


                //byte[] cmdbyte = StrToByteArray("start");
                //trackerStream.Write(cmdbyte, 0, cmdbyte.Length);

                slPort = TcpApps.RanPort(sConfig.SLPort, sConfig.SLisPortup);

                byte[] ListenPortbyte = BitConverter.GetBytes(slPort); //register the number of tree in tracker
                trackerStream.Write(ListenPortbyte, 0, ListenPortbyte.Length);

                byte[] treeSizebyte = BitConverter.GetBytes(sConfig.TreeSize); //register the number of tree in tracker
                trackerStream.Write(treeSizebyte, 0, treeSizebyte.Length);

                byte[] maxcbyte = BitConverter.GetBytes(sConfig.MaxClient);
                trackerStream.Write(maxcbyte, 0, maxcbyte.Length);

                trackerStream.Close();
                trackerTcp.Close();

                return true;
            }
            catch
            {
                if(trackerStream!=null)
                  trackerStream.Close();
                if(trackerTcp!=null)
                  trackerTcp.Close();
            }
            return false;

        }
        private static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }


        private void listenForClients()
        {
            TcpClient client = null;
            NetworkStream stream = null;

            try
            {
                listenServer = new TcpListener(localAddr, slPort);
                listenServer.Start();
            }
            catch(Exception ex)
            {
                mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { ex.ToString() });
            }

            mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "Port[" + slPort + "]:Listening...\n" });
            while (true)
            {
                try
                {
                    client = listenServer.AcceptTcpClient();
                    stream = client.GetStream();

                    int total_req_num = 0;
                    int req_tree_num = 0;
                    int tempC_num, tempD_num;

                    byte[] cMessage;
                    byte[] dMessage;

                    //client require how many tree
                    byte[] responseMessage = new Byte[4];
                    stream.Read(responseMessage, 0, responseMessage.Length);
                    total_req_num = BitConverter.ToInt16(responseMessage, 0);

                    for (int i = 0; i < total_req_num; i++)
                    {
                        //which tree ,client want to join.
                        bool sendPort = false;
                        stream.Read(responseMessage, 0, responseMessage.Length);
                        req_tree_num = BitConverter.ToInt16(responseMessage, 0);


                        for (int j = 0; j < max_client; j++)
                        {
                            if (ph.getTreeCListClient(req_tree_num, j) == null && ph.getTreeDListClient(req_tree_num, j) == null)
                            {
                                tempC_num = ph.getTreeCListPort(req_tree_num, j);
                                cMessage = BitConverter.GetBytes(tempC_num);
                                stream.Write(cMessage, 0, cMessage.Length);
                                mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] {"T["+ (req_tree_num-1) + "] Cport:" + tempC_num.ToString() + " " });

                                tempD_num = ph.getTreeDListPort(req_tree_num, j);
                                dMessage = BitConverter.GetBytes(tempD_num);
                                stream.Write(dMessage, 0, dMessage.Length);
                                mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Dport:" + tempD_num.ToString() + "\n" });

                                sendPort = true;
                                break;
                            }
                        }

                        if (sendPort != true)
                        {   // required tree number cant join
                            Byte[] cMessage2 = BitConverter.GetBytes(0000);
                            stream.Write(cMessage2, 0, cMessage2.Length);
                            //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Cport:no port" + "\n" });

                            Byte[] dMessage2 = BitConverter.GetBytes(0000);
                            stream.Write(dMessage2, 0, dMessage2.Length);
                            //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Dport:no port" + "\n" });
                        }

                    }

                    if (stream != null)
                        stream.Close();
                    if(client != null)
                        client.Close();

                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show(ex.ToString());

                    mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "One client join fail...\n" });
                   
                    if (stream != null)
                        stream.Close();
                    if (client != null)
                        client.Close();
                    
                }

                Thread.Sleep(50);
            }

        }

        public static string ByteArrayToString(byte[] bytes)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }

        bool checkEnd = false;

        private void getStreaming()
        {
            TcpClient getClient = new TcpClient(TcpApps.LocalIPAddress(), sConfig.VlcStreamPort);
            NetworkStream vlcStream = getClient.GetStream();

            try
            {
                byte[] getMessage = System.Text.Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost:127.0.0.1\r\nConnection:Close\r\n\r\n");
                vlcStream.Write(getMessage, 0, getMessage.Length);

                byte[] responseMessage = new byte[256];
                Int32 responseMessageBytes1 = vlcStream.Read(responseMessage, 0, responseMessage.Length);
                String responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes1);
                //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { responseString });

                Chunk streamingChunk = new Chunk();
                //byte[] responseData = new byte[sConfig.ReceiveStreamSize];
                int responseMessageBytes;

                byte[] sendMessage = new byte[sConfig.ChunkSize];


                while (responseMessageBytes1 != 0)
                {
                    // If read timeout(5 sec.) , we assume the movie has finished playing
                    vlcStream.ReadTimeout = 500; 

                    byte[] responseData = new byte[sConfig.ReceiveStreamSize];
                    responseMessageBytes = vlcStream.Read(responseData, 0, responseData.Length);
                    if (responseMessageBytes == 0)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    streamingChunk = ch.streamingToChunk(responseMessageBytes, responseData, seqNumber);

                    int remainder_number = streamingChunk.seq % max_tree;

                    if (remainder_number == 0)
                        ph.setChunkList(streamingChunk, max_tree - 1);
                    else
                        ph.setChunkList(streamingChunk, remainder_number - 1);

                    mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { seqNumber.ToString() });

                    if (seqNumber == 2147483647)
                        seqNumber = 1;
                    else
                        seqNumber += 1;

                    Thread.Sleep(20);
                }
            }
            catch(Exception ex)
            {
                for (int i = 0; i < max_tree; i++)
                {
                    ph.setTreeCLState(i, 0);
                }

                vlcStream.Close();
                getClient.Close();

                checkEnd = true;
                //vlc.stop();
            }
        }

        //public void closeThread()
        //{
        //    listenServer.Stop();
        //    listenerThread.Abort();

        //    vlc.stop();
        //    getStreamingThread.Abort();
        //}


    }
}
