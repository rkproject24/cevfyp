using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassLibrary;

namespace Server
{
    public partial class PreferenceFm : Form
    {
        private ServerConfig sConfig= new ServerConfig();

        public PreferenceFm()
        {
            InitializeComponent();
            loadConfig();
        }

        private void loadConfig()
        {
            sConfig.load("C:\\ServerConfig.xml");

            tbConport.Text = sConfig.CportBase.ToString();
            tbDataport.Text = sConfig.Dport.ToString();
            tbPlugin.Text  = sConfig.PluginPath;
            tbSLPort.Text = sConfig.SLPort.ToString();
            tbstreamport.Text = sConfig.VlcStreamPort.ToString();
            tbStreamType.Text  = sConfig.StreamType;
            tbvideodir.Text  = sConfig.VideoDir;
            NudMaxClient.Value = sConfig.MaxClient;
            NudTreeSize.Value = sConfig.TreeSize;
            tbDefaultIp.Text = sConfig.Serverip;
            tbRecStream.Text = sConfig.ReceiveStreamSize.ToString();
            tbChunkSize.Text = sConfig.ChunkSize.ToString();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            loadConfig();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            sConfig.CportBase = Convert.ToInt32(tbConport.Text);
            sConfig.Dport = Convert.ToInt32(tbDataport.Text);
            sConfig.PluginPath = tbPlugin.Text;
            sConfig.SLPort = Convert.ToInt32(tbSLPort.Text);
            sConfig.VlcStreamPort = Convert.ToInt32(tbstreamport.Text);
            sConfig.StreamType = tbStreamType.Text;
            sConfig.VideoDir = tbvideodir.Text;
            sConfig.MaxClient = Convert.ToInt32(NudMaxClient.Value);
            sConfig.TreeSize = Convert.ToInt32(NudTreeSize.Value);
            sConfig.Serverip = tbDefaultIp.Text;
            sConfig.ReceiveStreamSize = Convert.ToInt32(tbRecStream.Text);
            sConfig.ChunkSize = Convert.ToInt32(tbChunkSize.Text);

            sConfig.save("C:\\ServerConfig.xml");

            this.Close();
        }


    }
}
