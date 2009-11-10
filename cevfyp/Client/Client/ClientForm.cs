using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.IO;
//using System.Runtime.InteropServices;
using ClassLibrary;

namespace Client
{
    public partial class ClientForm : Form
    {
       // static int chunkList_capacity = 1000;  //0-xxx
       // static int SERVER_PORT = 1100;  //server listen port
      
        //VlcHandler vlc = new VlcHandler();
        //IPAddress localAddr = IPAddress.Parse("127.0.0.1");
       // List<Chunk> chunkList = new List<Chunk>(chunkList_capacity);

       

        ClientHandler clientHandler;

      //  public delegate void UpdateTextCallback(string message);

        public void UpdateTextBox1(string message)
        {
            tbWriteStatus.Text = message;
        }

        public void UpdateTextBox2(string message)
        {
            tbReadStatus.Text = message;
        }

        public void UpdateTextBox3(string message)
        {
            tbStatus.Text = message;
        }

        public void UpdateTextBox4(string message)
        {
            tbServerIp.Text = message;
        }

        public ClientForm()
        {
            InitializeComponent();
            clientHandler = new ClientHandler(this);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)  //Connect
        {
            string response;
            response = clientHandler.connectToServer(tbServerIp.Text); //connect tracker

            if (response == "OK")
                response = clientHandler.connectToSource();
            else
                MessageBox.Show(response);

            if (response == "OK2")
            {
                btnDisconnect.Enabled = true;
                clientHandler.startThread();
            }
            else
                MessageBox.Show(response);
        }
         
        private void button2_Click(object sender, EventArgs e)
        {
            clientHandler.getMute();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnDisconnect.Enabled = false;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            btnDisconnect.Enabled = false;
            clientHandler.disconectall();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            clientHandler.disconectall();
        }

        private void preferenceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Preference serverPre = new Preference();
            serverPre.Show();
        }

     

      







    }//end form class
}// end namespace
