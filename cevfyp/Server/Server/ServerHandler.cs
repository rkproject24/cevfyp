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

        static int TrackerSLPort = 1500;

        int slPort;
        int max_client;
        int max_tree;
        int seqNumber = 1;

        TcpListener listenServer;
        Thread listenerThread;
        Thread getStreamingThread;

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

          
        }

        //By Vinci
        public void reloadUI()
        {
            mainFm.tbMaxClient.Text = sConfig.MaxClient.ToString();
            mainFm.tbServerIp.Text = sConfig.Serverip;
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
        }


        public void pause()
        {
            //vlc.pause();
            seqNumber = 1;
            mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { "" });
            mainFm.richTextBox1.Clear();
            mainFm.richTextBox2.Clear();

        }

        public void stop()
        {
            vlc.stop();
            //seqNumber = 1;
           // mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { "" });
            //mainFm.richTextBox1.Clear();
            getStreamingThread.Abort(); //by vinci
        }

        public bool start()
        {
            //by vinci: update the online status to Tracker
            if (!UpdateTracker())
            {
                mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "Tracker unreachable!\n" });
            }
            else
            {
                //mainFm.tbMaxClient.Text = sConfig.MaxClient.ToString();
                //mainFm.tbServerIp.Text = sConfig.Serverip;
                sConfig.load("C:\\ServerConfig");
                reloadUI();
                //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "startport be4\n" });
                this.max_client = sConfig.MaxClient;
                this.max_tree = sConfig.TreeSize;
                ph = new PortHandler(max_client,max_tree, sConfig.Serverip, mainFm);
                ph.startTreePort();//ph.startPort();
                //mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "startport after\n" });

                localAddr = IPAddress.Parse(sConfig.Serverip);
                listenerThread = new Thread(new ThreadStart(listenForClients));
                listenerThread.IsBackground = true;
                listenerThread.Name = " listen_for_clients";
                listenerThread.Start();
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
            TcpClient trackerTcp;
            NetworkStream trackerStream;

            try
            {
                trackerTcp = new TcpClient(mainFm.tbTracker.Text, TrackerSLPort);
                trackerStream = trackerTcp.GetStream();

                //define server type
                byte[] clienttype = StrToByteArray("<serverReg>");
                trackerStream.Write(clienttype, 0, clienttype.Length);


                //byte[] cmdbyte = StrToByteArray("start");
                //trackerStream.Write(cmdbyte, 0, cmdbyte.Length);

                slPort = TcpApps.RanPort(1100, 1120);

                byte[] ListenPortbyte = BitConverter.GetBytes(slPort); //register the number of tree in tracker
                trackerStream.Write(ListenPortbyte, 0, ListenPortbyte.Length);

                byte[] treeSizebyte = BitConverter.GetBytes(sConfig.TreeSize); //register the number of tree in tracker
                trackerStream.Write(treeSizebyte, 0, treeSizebyte.Length);

                byte[] maxcbyte = BitConverter.GetBytes(sConfig.MaxClient);
                trackerStream.Write(maxcbyte, 0, maxcbyte.Length);

                trackerTcp.Close();
                trackerStream.Close();

                return true;
            }
            catch
            {
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
            listenServer = new TcpListener(localAddr, slPort);
            listenServer.Start();

            while (true)
            {
                //listenServer.Start();
                mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "Port[" + slPort + "]:Listening...\n" });
                TcpClient client = listenServer.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                
  //yam:10-10-09
               // bool sendCPort = false;
              //  bool sendD1Port = false;
              //  bool sendD2Port = false;


               

                try
                {
                  /*  for (int i = 0; i < max_client; i++)
                    {
                        if (ph.getTreeCListClient(1, i) == null) //ph.getCListClient(i) == null
                        {
                            temp = ph.getTreeCListPort(1, i);//temp = ph.getCListPort(i);
                            Byte[] cMessage = BitConverter.GetBytes(temp);
                            stream.Write(cMessage, 0, cMessage.Length);
                            sendCPort = true;
                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Cport:"+temp.ToString()+"\n" });

                            break;
                        }

                    }

                    for (int i = 0; i < max_client; i++)
                    {
                        if (ph.getTreeDListClient(1, i) == null) //ph.getDListClient(i) == null
                        {
                            temp=ph.getTreeDListPort(1,i);//temp = ph.getDListPort(i, 1);
                            Byte[] d1Message = BitConverter.GetBytes(temp);
                            stream.Write(d1Message, 0, d1Message.Length);
                            sendD1Port = true;
                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "D1port:" + temp.ToString() + "\n" });

                            break;
                        }
                    }

                    for (int i = 0; i < max_client; i++)
                    {
                        if (ph.getTreeDListClient(2, i) == null) //ph.getDListClient(i) == null
                        {
                            temp = ph.getTreeDListPort(2, i);//temp = ph.getDListPort(i, 2);
                            Byte[] d2Message = BitConverter.GetBytes(temp);
                            stream.Write(d2Message, 0, d2Message.Length);
                            sendD2Port = true;
                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "D2port:" + temp.ToString() + "\n" });

                            break;
                        }
                    }
                   
                    
                if (sendCPort == false || sendD1Port==false || sendD2Port==false)
                {
                    Byte[] cMessage = BitConverter.GetBytes(0000);   //0000 mean no C port can join
                    stream.Write(cMessage, 0, cMessage.Length);

                    Byte[] dMessage = BitConverter.GetBytes(0000);
                    stream.Write(dMessage, 0, dMessage.Length);     //d1

                    stream.Write(dMessage, 0, dMessage.Length);     //d2
                }
                */
                   

                    /* 
                       bool sendCPortState = false;
                       bool sendDPortState = false;
                      
                      for (int i = 1; i <= max_tree; i++)
                     {
                         for (int j = 0; i < max_client; j++)
                         {
                             Byte[] cMessage;
                             Byte[] dMessage;
                             sendCPortState = false;
                             sendDPortState = false;

                             if (ph.getTreeCListClient(i, j) == null)
                                 sendCPortState = true;
                              
                             if (ph.getTreeDListClient(i, j) == null)
                                 sendDPortState = true;
                              

                             if (sendCPortState == true && sendDPortState == true)
                             {
                                 temp = ph.getTreeCListPort(i, j);
                                 cMessage = BitConverter.GetBytes(temp);
                                 stream.Write(cMessage, 0, cMessage.Length);
                                 mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Cport:" + temp.ToString() + "\n" });

                                 temp = ph.getTreeDListPort(i, j);
                                 dMessage = BitConverter.GetBytes(temp);
                                 stream.Write(dMessage, 0, dMessage.Length);
                                 mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Dport:" + temp.ToString() + "\n" });

                                 break;
                             }
                         }


                         if (sendCPortState != true && sendDPortState != true)
                         {
                             Byte[] cMessage = BitConverter.GetBytes(0000);   //0000 mean no C port can join
                             stream.Write(cMessage, 0, cMessage.Length);

                             Byte[] dMessage = BitConverter.GetBytes(0000);
                             stream.Write(dMessage, 0, dMessage.Length); 
                         }
                     }
                     */

                    int total_req_num = 0;
                    int req_tree_num = 0;
                    int tempC_num, tempD_num;
                   
                    Byte[] cMessage;
                    Byte[] dMessage;

                    //client require how many tree
                    byte[] responseMessage = new Byte[4];
                    stream.Read(responseMessage, 0, responseMessage.Length);
                   total_req_num= BitConverter.ToInt16(responseMessage, 0);
                 
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
                                mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Cport:" + tempC_num.ToString() + "\n" });

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

                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());

                    mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "One client join fail...\n" });
                    break;
                }

                stream.Dispose();
                client = null;
                

                Thread.Sleep(50);
            }//end while loop
            listenServer.Stop();
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }
       
        private void getStreaming()
        {
            TcpClient getClient = new TcpClient("127.0.0.1", 1234);

            Byte[] getMessage = System.Text.Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost:127.0.0.1\r\nConnection:Close\r\n\r\n");
            NetworkStream vlcStream = getClient.GetStream();
            vlcStream.Write(getMessage, 0, getMessage.Length);

            Byte[] responseMessage = new Byte[256];
            Int32 responseMessageBytes1 = vlcStream.Read(responseMessage, 0, responseMessage.Length);
            String responseString = System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes1);
           // mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { responseString });

            Chunk streamingChunk = new Chunk();
           // byte[] responseData = new byte[sConfig.ReceiveStreamSize];
            int responseMessageBytes;

            byte[] sendMessage = new byte[sConfig.ChunkSize];


            while (responseMessageBytes1 != 0)
            {
                byte[] responseData = new byte[sConfig.ReceiveStreamSize];
                responseMessageBytes = vlcStream.Read(responseData, 0, responseData.Length);
                if (responseMessageBytes == 0)
                {
                    Thread.Sleep(5);
                    continue;
                }

                streamingChunk = ch.streamingToChunk(responseMessageBytes, responseData, seqNumber);
               
                //yam:10-10-09
              //  if (streamingChunk.seq % 2 != 0)
               //     ph.setOddListChunk(streamingChunk);
              //  else
               //     ph.setEvenListChunk(streamingChunk);


                //yam:01-01-10
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

    }
}
