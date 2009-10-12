﻿using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using System.Net;
using System.Net.Sockets;
using ClassLibrary;

namespace Client
{
    class PeerHandler
    {
        //static int TRACKER_PORT = 1100;  //server listen port
        static int TREE_NO = 2;
        static int TrackerSLPort = 1500;

        private string trackIp;
        private string peerIp;

        int Cport = 0;             //control message port number
        ClientConfig cConfig = new ClientConfig();

        public int Cport1
        {
            get { return Cport; }
            set { Cport = value; }
        }

        //int D1port = 0;             //video data port number
        int[] Dport = new int[TREE_NO];

        public PeerHandler(string trackerIp)
        {
            cConfig.load("C:\\ClientConfig.xml");
            this.trackIp = trackerIp;
            for (int i = 0; i < TREE_NO; i++)
            {
                Dport[i] = 0;
            }
        }
//by Vinci: coonect to Tracker for peer ip 
        public bool findPeer()
        {
            TcpClient trackerTcpClient;
            NetworkStream trackerStream;
            try
            {
                trackerTcpClient = new TcpClient(trackIp, TrackerSLPort);
                trackerStream = trackerTcpClient.GetStream();


                byte[] responsePeerMsg = new byte[4];
                trackerStream.Read(responsePeerMsg, 0, responsePeerMsg.Length);

                int ipsize = BitConverter.ToInt16(responsePeerMsg, 0);

                byte[] responsePeerMsg2 = new byte[ipsize];
                trackerStream.Read(responsePeerMsg2, 0, responsePeerMsg2.Length);
                //string peerip = BitConverter.ToString(responsePeerMsg2, 0);
                this.peerIp = ByteArrayToString(responsePeerMsg2);
                //Cport = BitConverter.ToInt16(responseCMessage, 0);

                trackerTcpClient.Close();
                trackerStream.Close();

                return true;
            }
            catch
            {
            }
            return false;
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }

        public bool connectPeer()
        {
            //peerIp = trackIp;

            TcpClient connectServerClient;
            NetworkStream connectServerStream;
            try
            {
                connectServerClient = new TcpClient(peerIp, cConfig.ServerSLPort1);
                connectServerStream = connectServerClient.GetStream();


                Byte[] responseCMessage = new Byte[4];
                connectServerStream.Read(responseCMessage, 0, responseCMessage.Length);
                Cport = BitConverter.ToInt16(responseCMessage, 0);

                for (int i = 0; i < TREE_NO; i++)
                {
                    Byte[] responseDMessage = new Byte[4];
                    connectServerStream.Read(responseDMessage, 0, responseDMessage.Length);
                    Dport[i] = BitConverter.ToInt16(responseDMessage, 0);
                }

                connectServerStream.Close();
                connectServerClient.Close();

                //vcport = Cport + 200;

                //serverConnect = true;
                //checkClose = false;

                return true;
            }
            catch
            {
            }
            return false;
        }

        public TcpClient getDataConnect(int tree)
        {


            //be modify later
            //if (Cport != 0 && Dport != 0)
            //{
            //    try
            //    {
            //        ClientC = new TcpClient(sourceIp, Cport);
            //    }
            //    catch
            //    {
            //        return "Port " + Cport + " Unreachable!";
            //    }

            //    try
            //    {
            //        ClientD = new TcpClient(sourceIp, Dport);
            //    }
            //    catch
            //    {
            //        return "Port " + Dport + " Unreachable!";
            //    }

            //    return "OK2";
            //}
            //else
            //{
            //    return "No source can join!";
            //}
            TcpClient treeclient = new TcpClient(trackIp, Dport[tree]);
            return treeclient;
        }
    }
}
