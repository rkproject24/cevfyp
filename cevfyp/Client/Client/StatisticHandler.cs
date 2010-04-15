using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using ClassLibrary;
using System.IO;
using System.Threading;
using Analysis;

namespace Client
{
    class StatisticHandler
    {
        int statisticListen;// = 1999;

        string localAddr;
        string statisticXmlDir;
        private List<plotgraph> graphTreeData;
        private bool connected, upConnected;
        private List<string[]> cmdstr;

        Thread sendStatThread;
        private bool enable;

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }
        public int StatisticListen
        {
            get { return statisticListen; }
            set { statisticListen = value; }
        }


        public StatisticHandler()
        {
            connected = false;
            localAddr = TcpApps.LocalIPAddress();
            statisticXmlDir = Directory.GetCurrentDirectory();
            enable = false;
            cmdstr = new List<string[]>();

            sendStatThread = new Thread(sendStat);
            sendStatThread.IsBackground = true;
            sendStatThread.Name = "sendStatThread";
            sendStatThread.Start();
        }

        private bool createGraph(int totaltree, string type)
        {

            //graphTreeData = new List<plotgraph>(totaltree);
            //for (int i=0; i < totaltree; i++)
            //    graphTreeData.Add(new plotgraph("Tree" + i, true));

            TcpClient trackerTcpClient = null;
            NetworkStream statisticStream = null;
            //string PeerFileName = Peerlist_name + tree + ".xml";
            try
            {
                trackerTcpClient = new TcpClient(localAddr, statisticListen);
                statisticStream = trackerTcpClient.GetStream();

                //define client type
                Byte[] clienttype = StrToByteArray("<newCurve>");
                statisticStream.Write(clienttype, 0, clienttype.Length);

                byte[] treeNo = BitConverter.GetBytes(totaltree);
                statisticStream.Write(treeNo, 0, treeNo.Length);

                byte[] graphtype = StrToByteArray(type);
                statisticStream.Write(graphtype, 0, graphtype.Length);

                //byte[] statisticXmlDirByte = StrToByteArray(statisticXmlDir);
                //byte[] MsgLength = BitConverter.GetBytes(statisticXmlDirByte.Length);
                //statisticStream.Write(MsgLength, 0, MsgLength.Length); //send size of ip
                //statisticStream.Write(statisticXmlDirByte, 0, statisticXmlDirByte.Length);
                statisticStream.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool updateCurve(DateTime start, DateTime end, int size, int tree, int treeTotal, string type)
        {
            //graphTreeData[tree].AddRecord(start, end, size);
            if (enable)
            {
                string[] commandStr = new string[6];
                commandStr[0] = start.ToBinary().ToString();
                commandStr[1] = end.ToBinary().ToString();
                commandStr[2] = size + "";
                commandStr[3] = tree + "";
                commandStr[4] = treeTotal + "";
                commandStr[5] = type;
                cmdstr.Add(commandStr);
                //sendStat(start, end, size , tree, treeTotal, type);

                //sendStatThread.Start(start, end, size, tree, treeTotal, type);
                //sendStatThread.Join();
            }
            return true;
        }

        public void sendStat()
        {
            TcpClient trackerTcpClient = null;
            NetworkStream statisticStream = null;
            //string PeerFileName = Peerlist_name + tree + ".xml";
            while (true)
            {
                if (cmdstr.Count > 0)
                {
                    try
                    {
                        trackerTcpClient = new TcpClient(localAddr, statisticListen);
                        statisticStream = trackerTcpClient.GetStream();
                        statisticStream.ReadTimeout = 1000;
                        //define client type
                        byte[] clienttype = StrToByteArray("<renewCur>");
                        statisticStream.Write(clienttype, 0, clienttype.Length);

                        byte[] graphtype = StrToByteArray(cmdstr[0][5]);
                        statisticStream.Write(graphtype, 0, graphtype.Length);

                        byte[] recoonectMsg = new byte[1];
                        statisticStream.Read(recoonectMsg, 0, recoonectMsg.Length);
                        connected = BitConverter.ToBoolean(recoonectMsg, 0);

                        if (connected)
                        {
                            //byte[] treeNo = BitConverter.GetBytes(tree);
                            byte[] treeNo = BitConverter.GetBytes(Int32.Parse(cmdstr[0][3]));
                            statisticStream.Write(treeNo, 0, treeNo.Length);

                            //string commandStr = start.ToBinary() + "@" + end.ToBinary() + "@" + size;
                            //if (graphtype.Equals("<upload>"))
                            //    Console.WriteLine(commandStr);


                            byte[] commandStrByte = StrToByteArray(cmdstr[0][0] + "@" + cmdstr[0][1] + "@" + cmdstr[0][2]);
                            byte[] MsgLength = BitConverter.GetBytes(commandStrByte.Length);
                            statisticStream.Write(MsgLength, 0, MsgLength.Length);
                            statisticStream.Write(commandStrByte, 0, commandStrByte.Length);
                            cmdstr.RemoveAt(0);
                        }
                        else
                        {
                            createGraph(Int32.Parse(cmdstr[0][4]), cmdstr[0][5]);
                            //updateCurve(start, end, size, tree, treeTotal, cmdstr[0][5]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                Thread.Sleep(1500);
            }
            //return true;
        }


        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }
    }
}
