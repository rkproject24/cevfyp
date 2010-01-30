using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassLibrary;

namespace Client
{
    public partial class Preference : Form
    {
        private ClientConfig cConfig = new ClientConfig();

        public Preference()
        {
           // this.TitleBar.TitleBarCaption = "Preference";
            InitializeComponent();
            loadConfig();
          
        }

        private void loadConfig()
        {
            cConfig.load("C:\\ClientConfig");

            tbChunkLenght.Text = cConfig.ChunkCapacity.ToString();
            tbPlugin.Text = cConfig.PluginPath;
            tbChunkSize.Text = cConfig.ChunkSize.ToString();
            tbControlPort.Text = cConfig.CportBase.ToString();
            tbDataPort.Text = cConfig.Dport.ToString();
            tbListPort.Text = cConfig.LisPort.ToString();
            tbVlcPort.Text = cConfig.VlcPortBase.ToString();
            NudStartBuf.Value = cConfig.StartBuf;
            NudChunkBuf.Value = cConfig.ChunkBuf;
            NudPeers.Value = cConfig.MaxPeer;
            cbDisplay.Checked = cConfig.Localdisplay;
            tbListPortup.Text = cConfig.LisPortup.ToString();
            tbDataPortup.Text = cConfig.Dataportup.ToString();
            tbControlPortup.Text = cConfig.Conportup.ToString();

            tbTrackerIp.Text = cConfig.Trackerip;
            tbTrackerPort.Text = cConfig.TrackerPort.ToString();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            loadConfig();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            cConfig.ChunkCapacity = Convert.ToInt32(tbChunkLenght.Text);
            cConfig.PluginPath = tbPlugin.Text;
            cConfig.ChunkSize = Convert.ToInt32(tbChunkSize.Text);
            cConfig.CportBase= Convert.ToInt32(tbControlPort.Text);
            cConfig.Dport = Convert.ToInt32(tbDataPort.Text);
            cConfig.LisPort = Convert.ToInt32(tbListPort.Text);
            cConfig.VlcPortBase = Convert.ToInt32(tbVlcPort.Text) ;
            cConfig.StartBuf = Convert.ToInt32(NudStartBuf.Value);
            cConfig.ChunkBuf = Convert.ToInt32(NudChunkBuf.Value);
            cConfig.MaxPeer = Convert.ToInt32(NudPeers.Value);

            cConfig.Trackerip = tbTrackerIp.Text;
            cConfig.TrackerPort = Convert.ToInt32(tbTrackerPort.Text);
            cConfig.Localdisplay = cbDisplay.Checked;
            cConfig.LisPortup = Convert.ToInt32(tbListPortup.Text);
            cConfig.Dataportup = Convert.ToInt32(tbDataPortup.Text);
            cConfig.Conportup = Convert.ToInt32(tbControlPortup.Text);

            cConfig.save("C:\\ClientConfig");
            this.Close();
        }

        private void btnVLClib_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            if (!path.SelectedPath.Equals(""))
                tbPlugin.Text = path.SelectedPath;
        }


      

        
    }
}
