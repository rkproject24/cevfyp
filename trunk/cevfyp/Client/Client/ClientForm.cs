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
using System.IO;
//using System.Runtime.InteropServices;
using ClassLibrary;
using Crom.Controls.Docking;
using Analysis;

namespace Client
{
    public partial class ClientForm :XCoolForm.XCoolForm
    {
        private const string code = "a6402b80-2ebd-4fd3-8930-024a6201d001";
        private const string code2 = "a6402b80-2ebd-4fd3-8930-024a6201d002";
        private const string code3 = "a6402b80-2ebd-4fd3-8930-024a6201d003";
        private const string code4 = "a6402b80-2ebd-4fd3-8930-024a6201d004";
        public Form downloadFrm, uploadFrm, playFrm, speedFrm;
        private DockStateSerializer _serializer = null;
        public plotgraph graphTreeData;

       // static int chunkList_capacity = 1000;  //0-xxx
       // static int SERVER_PORT = 1100;  //server listen port
      
        //VlcHandler vlc = new VlcHandler();
        //IPAddress localAddr = IPAddress.Parse("127.0.0.1");
       // List<Chunk> chunkList = new List<Chunk>(chunkList_capacity);

       // XmlThemeLoader xtl =new XmlThemeLoader();

        ClientHandler clientHandler;

      //  public delegate void UpdateTextCallback(string message);

        //public void UpdateTextBox1(string message)
        //{
        //    tbWriteStatus.Text = message;
        //}

        public void UpdateTBox1(string message)
        {
            textBox1.Text = message;
        }
        public void UpdateTBox2(string message)
        {
            textBox2.Text = message;
        }
        public void UpdateTBox3(string message)
        {
            textBox3.Text = message;
        }

        public void UpdateTextBox2(string message)
        {
            tbReadStatus.Text = message;
        }

        //public void UpdateTextBox3(string message)
        //{
        //    tbStatus.Text = message;
        //}

        //public void UpdateTextBox4(string message)
        //{
        //    tbServerIp.Text = message;
        //}
        public void UpdateRtbUpload(string message)
        {
            ((LoggerFrm)uploadFrm).rtbdownload.AppendText(message);
            //rtbupload.AppendText(message);
        }

        public void UpdateRtbDownload(string message)
        {
            ((LoggerFrm)downloadFrm).rtbdownload.AppendText(message);
            //rtbdownload.AppendText(message);
        }

        public void UpdateMainFmText(string message)
        {
            this.Text = message;
        }

        public void UpdateLabel5(string message)
        {
            label5.Text = message;
        }

        //public void updateGraph(plotgraph data)
        //{
        //    ((SpeedFrm)speedFrm).setData(data);
        //}


        public ClientForm()
        {
           
            InitializeComponent();
            _serializer = new DockStateSerializer(_docker);
        //_docker.PreviewRenderer = new CustomPreviewRenderer();
        //_docker.ShowContextMenu += OnDockerShowContextMenu;
        //_docker.FormClosing += OnDockerFormClosing;
        //_docker.FormClosed += OnDockerFormClosed;

            graphTreeData = new plotgraph("Tree0", true);
            

            downloadFrm = CreateTestForm(new Guid(code));
            //form1.Show();
            DockableFormInfo info1 = _docker.Add(downloadFrm, zAllowedDock.All, new Guid(code));
            info1.ShowContextMenuButton = false;
            info1.ShowCloseButton = false;

            _docker.DockForm(info1, DockStyle.Left, zDockMode.Outer);
            _docker.SetHeight(info1, 310);
      
            this.uploadFrm = CreateTestForm(new Guid(code2));
            //form1.Show();
            DockableFormInfo info2 = _docker.Add(uploadFrm, zAllowedDock.All, new Guid(code2));
            info2.ShowContextMenuButton = false;

            _docker.DockForm(info2, DockStyle.Right, zDockMode.Outer);
            _docker.SetHeight(info2, 310);
            info2.ShowCloseButton = false;

            this.playFrm = CreateTestForm(new Guid(code3));
            ////form1.Show();
            DockableFormInfo info3 = _docker.Add(playFrm, zAllowedDock.All, new Guid(code3));
            info3.ShowContextMenuButton = false;
            info3.ShowCloseButton = false;
            _docker.DockForm(info3, DockStyle.Fill, zDockMode.Outer);
            _docker.SetHeight(info3, 310);

            //SpeedFrm speed = new SpeedFrm("tree0");
            //this.speedFrm = CreateTestForm(new Guid(code4), speed);
            ////form1.Show();
            //DockableFormInfo info4 = _docker.Add(speedFrm, zAllowedDock.All, new Guid(code4));
            //info4.ShowContextMenuButton = false;
            //_docker.DockForm(info4, DockStyle.Bottom, zDockMode.Outer);
            //_docker.SetHeight(info4, 310);
            //info4.ShowCloseButton = false;
            

            
            //_docker.SetAutoHide(info1, true);
            //_docker.SetAutoHide(info2, true);
            clientHandler = new ClientHandler(this);
        }


        private static Form CreateTestForm(int left, int top, int width, int height, Color backColor, string caption)
        {
            Form form = new Form();
            form.Bounds = new Rectangle(left, top, width, height);
            form.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            TextBox text = new TextBox();
            text.Multiline = true;
            text.Parent = form;
            text.Dock = DockStyle.Fill;
            text.BackColor = backColor;
            form.Text = caption;
            form.TopLevel = false;

            return form;
        }

        public static Form CreateTestForm(Guid identifier)
        {
            if (identifier == new Guid(code))
            {
                LoggerFrm result = new LoggerFrm();
                result.Bounds = new Rectangle(0, 0, 176, 345);
                result.Text = "Download";
                result.label1.Text = "TrackerIP";
                result.lbSpeed.Text = "0Kb";
                return result;
            }
            else if (identifier == new Guid(code2))
            {
                LoggerFrm result = new LoggerFrm();
                result.Bounds = new Rectangle(400, 0, 176, 345);
                result.Text = "Upload";
                result.label1.Text = "Hosting IP";
                result.lbSpeed.Text = "0Kb";
                return result;
            }
            else if (identifier == new Guid(code3))
            {
                PlaybackFrm result = new PlaybackFrm();
                result.Bounds = new Rectangle(200, 0, 640, 480);
                result.Text = "video";
                return result;
            }
            //else if (identifier == new Guid(code4))
            //{
                
            //    //SpeedFrm result = new SpeedFrm("tree0", graphTreeData);
            //    result.Bounds = new Rectangle(200, 100, 400, 200);
            //    result.Text = "speed";
            //    return result;
            //}

            throw new InvalidOperationException();
        }

        //public static Form CreateTestForm(Guid identifier, SpeedFrm result)
        //{
        //    result.Bounds = new Rectangle(200, 100, 400, 450);
        //    result.Text = "speed";
        //    return result;

        //    throw new InvalidOperationException();
        //}


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        //private void button1_Click(object sender, EventArgs e)  //Connect
        //{
        //    string response = clientHandler.establishConnect(tbServerIp.Text);
        //    if (!response.Equals(""))
        //    {
        //        MessageBox.Show(response);
        //    }
        //    else
        //    {
        //        btnDisconnect.Enabled = true;
        //        btnConnect.Enabled = false;
        //        clientHandler.startThread();
        //    }
        //    //string response;

        //    ////connect tracker
        //    //response = clientHandler.connectToTracker(tbServerIp.Text);

        //    //if (response == "OK")
        //    //    response = clientHandler.connectToSource();
        //    //else
        //    //    MessageBox.Show(response);
        //    ////=======================================

        //    //if (response == "OK2")
        //    //{
        //    //    btnDisconnect.Enabled = true;
        //    //    clientHandler.startThread();
        //    //}
        //    //else
        //    //    MessageBox.Show(response);
        //}
         
        private void button2_Click(object sender, EventArgs e)
        {
            clientHandler.getMute();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.IconHolder.HolderButtons.Add(new XCoolForm.XTitleBarIconHolder.XHolderButton(Client.Properties.Resources.System_Registry_48x48.GetThumbnailImage(20, 20, null, IntPtr.Zero), "Preference"));
            //this.IconHolder.HolderButtons[0].FrameBackImage = Client.Properties.Resources.System_Registry_48x48;
            this.IconHolder.HolderButtons[0].XHolderButtonCaptionColor = Color.Yellow;
            this.IconHolder.HolderButtons[0].XHolderButtonDescription = "";

            //this.StatusBar.BarHeight = 40;
            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(60, "ver. xxx"));
            this.XCoolFormHolderButtonClick += new XCoolFormHolderButtonClickHandler(ClientForm_XCoolFormHolderButtonClick);
            this.TitleBar.TitleBarCaption = "Client";
            this.TitleBar.TitleBarType = XCoolForm.XTitleBar.XTitleBarType.Rounded;

            btnDisconnect.Enabled = false;



        }

        private void ClientForm_XCoolFormHolderButtonClick(XCoolForm.XCoolForm.XCoolFormHolderButtonClickArgs e)
        {
              switch (e.ButtonIndex)
              {
                  case 0:
                      Preference serverPre = new Preference();
                      serverPre.Show();
                      //Form form1 = CreateTestForm(new Guid(code));
                      ////form1.Show();
                      //DockableFormInfo info1 = _docker.Add(form1, zAllowedDock.All, new Guid(code));
                      //info1.ShowContextMenuButton = false;
                      //_docker.DockForm(info1, DockStyle.Left, zDockMode.Outer);
                      //_docker.SetHeight(info1, 310);
                      break;
             }
        }

        //private void btnDisconnect_Click(object sender, EventArgs e)
        //{
        //    btnDisconnect.Enabled = false;
        //    clientHandler.closeAllThread();

           
        //    btnConnect.Enabled = true;
        //}

        //private void preferenceToolStripMenuItem1_Click(object sender, EventArgs e)
        //{
        //    Preference serverPre = new Preference();
        //    serverPre.Show();
        //}

        //private void btnListenPeer_Click(object sender, EventArgs e)
        //{
        //    clientHandler.startUpload();
        //}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbServerIp_TextChanged(object sender, EventArgs e)
        {

        }

       

   

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string response = clientHandler.establishConnect(((LoggerFrm)downloadFrm).tbIP.Text.ToString());
            if (!response.Equals(""))
            {
                MessageBox.Show(response);
                
            }
            else
            {
               
                btnDisconnect.Enabled = true;
                btnConnect.Enabled = false;
                clientHandler.startThread();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            btnDisconnect.Enabled = false;
            clientHandler.closeAllThread();
            btnConnect.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (clientHandler.vlc.playing)
            //    clientHandler.vlc.stop(); //to avoid GUI problem
            clientHandler.closeAllThread();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnStatistic.Enabled = false;
            clientHandler.startStatistic();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

    
       

  
       

      

    
     

      







    }//end form class
}// end namespace
