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

 //yam:10-10-09
       
        // static int CHUNKLIST_CAPACITY = 200;
       // List<Chunk> oddList;
       // List<Chunk> evenList;
       // int currentOddNo = 0;
      //  int currentEvenNo = 0;
       // int oddList_wIndex = 0;
        //int oddList_rIndex = 0;
        //int evenList_wIndex = 0;
        //int evenList_rIndex = 0;
        //int tempSeq = 0;
        
        int max_client;
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
        //test
        public ServerHandler(ServerFrm mainFm)
        {
            this.mainFm = mainFm;
            //sConfig = new ServerConfig(mainFm.tblibsrc.Text, "ts", mainFm.tbfilesrc.Text, 3, 1234, SLPORT, DPORT_BASE, 1301, TREE_NO);
            //sConfig.save("tempconfig.xml");

            sConfig = new ServerConfig();
            sConfig.load("C:\\ServerConfig.xml");
            mainFm.tbMaxClient.Text = sConfig.MaxClient.ToString();
            mainFm.tbServerIp.Text = sConfig.Serverip;
            mainFm.tbfilesrc.Text = sConfig.VideoDir;

            //oddList = new List<Chunk>(CHUNKLIST_CAPACITY);
           // evenList = new List<Chunk>(CHUNKLIST_CAPACITY);
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
            vlc.pause();
        }

        public void stop()
        {
            vlc.stop();
            seqNumber = 1;
            mainFm.textBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateTextBox2), new object[] { "" });
            mainFm.richTextBox1.Clear();
            getStreamingThread.Abort(); //by vinci
        }

        public void start()
        {
            mainFm.tbMaxClient.Text = sConfig.MaxClient.ToString();
            mainFm.tbServerIp.Text = sConfig.Serverip;

            this.max_client = sConfig.MaxClient;
            ph = new PortHandler(max_client,sConfig.Serverip,mainFm);
            ph.startPort();

            localAddr = IPAddress.Parse(sConfig.Serverip);
            listenerThread = new Thread(new ThreadStart(listenForClients));
            listenerThread.IsBackground = true;
            listenerThread.Name = " listen_for_clients";
            listenerThread.Start();
        }

        public void mute()
        {
            int checkMute = vlc.getMute();
            if (checkMute == 0)
                vlc.setMute(1);
            else
                vlc.setMute(0);
        }

        private String readStreamToString(NetworkStream stream, int size)
        {
            byte[] responseMessage = new byte[size];
            Int32 responseMessageBytes = stream.Read(responseMessage, 0, responseMessage.Length);
            return System.Text.Encoding.ASCII.GetString(responseMessage, 0, responseMessageBytes);
        }

        private void listenForClients()
        {
            listenServer = new TcpListener(localAddr, sConfig.SLPort);
            listenServer.Start();

            int temp;

            while (true)
            {
                mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "Port[" + sConfig.SLPort.ToString() + "]:Listening...\n" });
                TcpClient client = listenServer.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                
  //yam:10-10-09
                bool sendCPort = false;
                bool sendD1Port = false;
                bool sendD2Port = false;
                
                try
                {
                    for (int i = 0; i < max_client; i++)
                    {
                        if (ph.getCListClient(i) == null)
                        {
                            temp = ph.getCListPort(i);
                            Byte[] cMessage = BitConverter.GetBytes(temp);
                            stream.Write(cMessage, 0, cMessage.Length);
                            sendCPort = true;
                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "Cport:"+temp.ToString()+"\n" });

                            break;
                        }
                    }

                    for (int i = 0; i < max_client; i++)
                    {
                        if (ph.getDListClient(i, 1) == null)
                        {
                            temp = ph.getDListPort(i, 1);
                            Byte[] d1Message = BitConverter.GetBytes(temp);
                            stream.Write(d1Message, 0, d1Message.Length);
                            sendD1Port = true;
                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "D1port:" + temp.ToString() + "\n" });

                            break;
                        }
                    }

                    for (int i = 0; i < max_client; i++)
                    {
                        if (ph.getDListClient(i, 2) == null)
                        {
                            temp = ph.getDListPort(i, 2);
                            Byte[] d2Message = BitConverter.GetBytes(temp);
                            stream.Write(d2Message, 0, d2Message.Length);
                            sendD2Port = true;
                            mainFm.richTextBox1.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox1), new object[] { "D2port:" + temp.ToString() + "\n" });

                            break;
                        }
                    }
                }
                catch
                {
                    mainFm.richTextBox2.BeginInvoke(new UpdateTextCallback(mainFm.UpdateRichTextBox2), new object[] { "One client join fail...\n" });
                    break;
                }

                if (sendCPort == false || sendD1Port==false || sendD2Port==false)
                {
                    Byte[] cMessage = BitConverter.GetBytes(0000);   //0000 mean no C port can join
                    stream.Write(cMessage, 0, cMessage.Length);

                    Byte[] dMessage = BitConverter.GetBytes(0000);
                    stream.Write(dMessage, 0, dMessage.Length);     //d1

                    stream.Write(dMessage, 0, dMessage.Length);     //d2
                }

            }//end while loop
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

            NetworkStream stream; 

            Chunk streamingChunk = new Chunk();
           // byte[] responseData = new byte[sConfig.ReceiveStreamSize];
            int responseMessageBytes;

            byte[] sendMessage = new byte[sConfig.ChunkSize];

            bool oddChunk = true;

            while (responseMessageBytes1 != 0)
            {
                byte[] responseData = new byte[sConfig.ReceiveStreamSize];
                responseMessageBytes = vlcStream.Read(responseData, 0, responseData.Length);
                if (responseMessageBytes == 0)
                {
                    continue;
                }

                streamingChunk = ch.streamingToChunk(responseMessageBytes, responseData, seqNumber);
               
                /*//handle multiple tree
                if (streamingChunk.seq % 2 != 0)
                    oddChunk = true;
                else
                    oddChunk = false;
                */

//yam:10-10-09
                if (streamingChunk.seq % 2 != 0)
                    ph.setOddListChunk(streamingChunk);
                else
                    ph.setEvenListChunk(streamingChunk);



                    /*
                     for (int i = 0; i < max_client; i++)
                     {
                         try
                         {

                             if (oddChunk==true)
                             {
                                 if (ph.getDListClient(i,1) == null)
                                   continue;
                                 stream = ph.getDListClient(i,1).GetStream();
                                 sendMessage = ch.chunkToByte(streamingChunk, sConfig.ChunkSize);
                                 stream.Write(sendMessage, 0, sendMessage.Length);
                        
                             }
                             else
                             {
                                 if (ph.getDListClient(i, 2) == null)
                                     continue;
                                 stream = ph.getDListClient(i,2).GetStream();
                                 sendMessage = ch.chunkToByte(streamingChunk, sConfig.ChunkSize);
                                 stream.Write(sendMessage, 0, sendMessage.Length);
                           
                             }
                             stream.Flush();

                         }
                         catch
                         {
                             //ph.delClientFromDList(i);
                         }
                     }
                     */

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
