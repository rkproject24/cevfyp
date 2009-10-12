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
            InitializeComponent();
            loadConfig();
        }

        private void loadConfig()
        {
            cConfig.load("C:\\ClientConfig.xml");

            tbChunkLenght.Text = cConfig.ChunkCapacity.ToString();
            tbPlugin.Text = cConfig.PluginPath;
            tbChunkSize.Text = cConfig.ChunkSize.ToString();
            tbControlPort.Text = cConfig.CportBase.ToString();
            tbDataPort.Text = cConfig.Dport.ToString();
            tbListPort.Text = cConfig.SLPort.ToString();
            tbServerSLPort.Text = cConfig.ServerSLPort1.ToString();
            tbVlcPort.Text = cConfig.VlcPortBase.ToString();
            NudStartBuf.Value = cConfig.StartBuf;
            NudChunkBuf.Value = cConfig.ChunkBuf;
            NudPeers.Value = cConfig.MaxPeer;
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
            cConfig.SLPort = Convert.ToInt32(tbListPort.Text);
            cConfig.ServerSLPort1 = Convert.ToInt32(tbServerSLPort.Text);
            cConfig.VlcPortBase = Convert.ToInt32(tbVlcPort.Text) ;
            cConfig.StartBuf = Convert.ToInt32(NudStartBuf.Value);
            cConfig.ChunkBuf = Convert.ToInt32(NudChunkBuf.Value);
            cConfig.MaxPeer = Convert.ToInt32(NudPeers.Value);

            cConfig.save("C:\\ClientConfig.xml");
            this.Close();
        }
    }
}
