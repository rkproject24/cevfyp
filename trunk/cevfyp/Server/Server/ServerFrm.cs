using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassLibrary;

namespace Server
{

    public partial class ServerFrm : Form
    {
        //by vinci
        static int STOP = 0;
        static int PLAY = 1;

        int playstate = STOP;

        ServerHandler sevhandle;

        public void UpdateTextBox1(string message)
        {
            textBox1.Text = message;
        }

        public void UpdateTextBox2(string message)
        {
            textBox2.Text = message;
        }

        public void UpdateTextBox3(string message)
        {
            textBox3.Text = message;
        }

        public void UpdateRichTextBox1(string message)
        {
            richTextBox1.AppendText(message);
        }

        public void UpdateRichTextBox2(string message)
        {
            richTextBox2.AppendText(message);
        }

        public ServerFrm()
        {
            InitializeComponent();
            sevhandle = new ServerHandler(this);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) //Play
        {
            sevhandle.play();

            button1.Enabled = false;
            playstate = PLAY;
        }

        private void button2_Click(object sender, EventArgs e) //Pause
        {
            sevhandle.pause();
        }

        private void button3_Click(object sender, EventArgs e) //Stop
        {
            sevhandle.stop();
            button1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e) //Start
        {
            sevhandle.start();
            button4.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)  //Mute
        {
            sevhandle.mute();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PreferenceFm preferencefm = new PreferenceFm();
            preferencefm.Show();
        }



    } //end form class
} //end namespace
