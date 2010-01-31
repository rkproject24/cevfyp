using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using ClassLibrary;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Analysis
{
    /// 
    /// The Main Ping Class
    /// 
    public class PingIP
    {
        //Declare some Constant Variables
        const int SOCKET_ERROR = -1;
        const int ICMP_ECHO = 8;
        /// 
        /// The Starting Point of the Class
        /// It Takes the Hostname parameter
        /// 
        public PingIP(string Input)
        {
            long result;
            try{
            if (Input.Length == 0)
            {
                MessageBox.Show("Please Input IP.");
            }
                xml PingResult = new xml(Input,"Result",true);

                while (true)
                {
                    //call the method "PingHost" and pass the HostName as a parameter
                    //int Result = PingHost(Input);
                    
                    Ping SpeedTest = new Ping();
                    PingOptions choice = new PingOptions();
                    
                    string data = "";
                    for (int g=0; g<1024; g++)
                    {
                        data = data + "a";
                    }

                    choice.DontFragment = true;
                    byte[] buffer = Encoding.ASCII.GetBytes(data);
                    int timeout = 1200;
                    PingReply respond = SpeedTest.Send(Input, timeout, buffer, choice);
                    
                    
                        //MessageBox.Show("Address: {0}", respond.Address.ToString());
                        //MessageBox.Show("RoundTrip time: {0}", respond.RoundtripTime.ToString());
                        //MessageBox.Show("Time to live: {0}", respond.Options.Ttl.ToString());
                        //MessageBox.Show("Don't fragment: {0}", respond.Options.DontFragment.ToString());
                        //MessageBox.Show("Buffer size: {0}", respond.Buffer.Length.ToString());
                    if (respond.Status == IPStatus.Success)
                    {
                        result = respond.Options.Ttl * 1024/8 / respond.RoundtripTime;
                    }
                    else result = 0;
                    int Record = 0;
                    while (!File.Exists("Reading"))
                    {
                        string[] type = { "RecordSpeed", "Time" };
                        string[] value = { result.ToString(), (System.DateTime.Now.Hour * 10000 + System.DateTime.Now.Minute * 100 + System.DateTime.Now.Second).ToString() };
                        string[] attriN = { "id"};
                        //string[] attriV = {(System.DateTime.Now.Hour * 3600 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second).ToString()};
                        string[] attriV = { Record.ToString() };

                        PingResult.Add("Record", type, value, attriN, attriV);

                        break;
                    }
                    break;

                }
            }
            catch
            {
                MessageBox.Show("Invalid Input.");
            }
        }
        /// 
        /// This method takes the "hostname" of the server
        /// and then it ping's it and shows the response time
        /// 
        public static int PingHost(string host)
        {
            //Declare the IPHostEntry 
            IPHostEntry serverHE, fromHE;
            int nBytes = 0;
            int dwStart = 0, dwStop = 0;
            //Initilize a Socket of the Type ICMP
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            // Get the server endpoint
            try
            {
                serverHE = Dns.GetHostByName(host);
            }
            catch (Exception)
            {
                Console.WriteLine("Host not found"); // fail
                return 0;
            }
            // Convert the server IP_EndPoint to an EndPoint
            IPEndPoint ipepServer = new IPEndPoint(serverHE.AddressList[0], 0);
            EndPoint epServer = (ipepServer);
            // Set the receiving endpoint to the client machine
            fromHE = Dns.GetHostByName(Dns.GetHostName());
            IPEndPoint ipEndPointFrom = new IPEndPoint(fromHE.AddressList[0], 0);
            EndPoint EndPointFrom = (ipEndPointFrom);
            int PacketSize = 0;
            IcmpPacket packet = new IcmpPacket();
            // Construct the packet to send
            packet.Type = ICMP_ECHO; //8
            packet.SubCode = 0;
            packet.CheckSum = UInt16.Parse("0");
            packet.Identifier = UInt16.Parse("45");
            packet.SequenceNumber = UInt16.Parse("0");
            int PingData = 32; // sizeof(IcmpPacket) - 8;
            packet.Data = new Byte[PingData];
            //Initilize the Packet.Data
            for (int i = 0; i < PingData; i++)
            {
                packet.Data[i] = (byte)'#';
            }
            //Variable to hold the total Packet size
            PacketSize = PingData + 8;
            Byte[] icmp_pkt_buffer = new Byte[PacketSize];
            Int32 Index = 0;
            //Call a Method Serialize which counts
            //The total number of Bytes in the Packet
            Index = Serialize(packet, icmp_pkt_buffer, PacketSize, PingData);
            //Error in Packet Size
            if (Index == -1)
            {
                Console.WriteLine("Error in Making Packet");
                return 0;
            }
            // now get this critter into a UInt16 array
            //Get the Half size of the Packet
            Double double_length = Convert.ToDouble(Index);
            Double dtemp = Math.Ceiling(double_length / 2);
            int cksum_buffer_length = Convert.ToInt32(dtemp);
            //Create a Byte Array
            UInt16[] cksum_buffer = new UInt16[cksum_buffer_length];
            //Code to initialize the Uint16 array 
            int icmp_header_buffer_index = 0;
            for (int i = 0; i < cksum_buffer_length; i++)
            {
                cksum_buffer[i] =
                BitConverter.ToUInt16(icmp_pkt_buffer, icmp_header_buffer_index);
                icmp_header_buffer_index += 2;
            }
            //Call a method which will return a checksum 
            UInt16 u_cksum = checksum(cksum_buffer, cksum_buffer_length);
            //Save the checksum to the Packet
            packet.CheckSum = u_cksum;
            // Now that we have the checksum, serialize the packet again
            Byte[] sendbuf = new Byte[PacketSize];
            //again check the packet size
            Index = Serialize(packet, sendbuf, PacketSize, PingData);
            //if there is a error report it
            if (Index == -1)
            {
                Console.WriteLine("Error in Making Packet");
                return 0;
            }
            dwStart = System.Environment.TickCount; // Start timing
            //send the Pack over the socket
            if ((nBytes = socket.SendTo(sendbuf, PacketSize, 0, epServer)) == SOCKET_ERROR)
            {
                Console.WriteLine("Socket Error cannot Send Packet");
            }
            // Initialize the buffers. The receive buffer is the size of the
            // ICMP header plus the IP header (20 bytes)
            Byte[] ReceiveBuffer = new Byte[256];
            nBytes = 0;
            //Receive the bytes
            bool recd = false;
            int timeout = 0;
            //loop for checking the time of the server responding 
            while (!recd)
            {
                nBytes = socket.ReceiveFrom(ReceiveBuffer, 256, 0, ref EndPointFrom);
                if (nBytes == SOCKET_ERROR)
                {
                    Console.WriteLine("Host not Responding");
                    recd = true;
                    break;
                }
                else if (nBytes > 0)
                {
                    dwStop = System.Environment.TickCount - dwStart; // stop timing
                    //Console.WriteLine("Reply from " + epServer.ToString() + " in " + dwStop + "MS :Bytes Received" + nBytes);
                    
                    recd = true;
                    return (nBytes / dwStop);
                }
                timeout = System.Environment.TickCount - dwStart;
                if (timeout > 1000)
                {
                    Console.WriteLine("Time Out");
                    recd = true;

                }
            }
            //close the socket
            socket.Close();
            return 0;
        }
        /// 
        /// This method get the Packet and calculates the total size 
        /// of the Pack by converting it to byte array
        /// 
        public static Int32 Serialize(IcmpPacket packet, Byte[] Buffer,
        Int32 PacketSize, Int32 PingData)
        {
            Int32 cbReturn = 0;
            // serialize the struct into the array
            int Index = 0;
            Byte[] b_type = new Byte[1];
            b_type[0] = (packet.Type);
            Byte[] b_code = new Byte[1];
            b_code[0] = (packet.SubCode);
            Byte[] b_cksum = BitConverter.GetBytes(packet.CheckSum);
            Byte[] b_id = BitConverter.GetBytes(packet.Identifier);
            Byte[] b_seq = BitConverter.GetBytes(packet.SequenceNumber);
            // Console.WriteLine("Serialize type ");
            Array.Copy(b_type, 0, Buffer, Index, b_type.Length);
            Index += b_type.Length;
            // Console.WriteLine("Serialize code ");
            Array.Copy(b_code, 0, Buffer, Index, b_code.Length);
            Index += b_code.Length;
            // Console.WriteLine("Serialize cksum ");
            Array.Copy(b_cksum, 0, Buffer, Index, b_cksum.Length);
            Index += b_cksum.Length;
            // Console.WriteLine("Serialize id ");
            Array.Copy(b_id, 0, Buffer, Index, b_id.Length);
            Index += b_id.Length;
            Array.Copy(b_seq, 0, Buffer, Index, b_seq.Length);
            Index += b_seq.Length;
            // copy the data 
            Array.Copy(packet.Data, 0, Buffer, Index, PingData);
            Index += PingData;
            if (Index != PacketSize/* sizeof(IcmpPacket) */)
            {
                cbReturn = -1;
                return cbReturn;
            }
            cbReturn = Index;
            return cbReturn;
        }
        /// 
        /// This Method has the algorithm to make a checksum 
        /// 
        public static UInt16 checksum(UInt16[] buffer, int size)
        {
            Int32 cksum = 0;
            int counter;
            counter = 0;
            while (size > 0)
            {
                UInt16 val = buffer[counter];
                cksum += Convert.ToInt32(buffer[counter]);
                counter += 1;
                size -= 1;
            }
            cksum = (cksum >> 16) + (cksum & 0xffff);
            cksum += (cksum >> 16);
            return (UInt16)(~cksum);
        }
    } // class ping
    /// 
    /// Class that holds the Pack information
    /// 
    public class IcmpPacket
    {
        public Byte Type; // type of message
        public Byte SubCode; // type of sub code
        public UInt16 CheckSum; // ones complement checksum of struct
        public UInt16 Identifier; // identifier
        public UInt16 SequenceNumber; // sequence number 
        public Byte[] Data;
    } // class IcmpPacket
}