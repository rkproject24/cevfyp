using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class ControlFrm : Form
    {
        private ClientForm clientFrm;
        private ClientHandler clientHandler;

        public ControlFrm(ClientForm clientFrm, ClientHandler clientHandler)
        {
            InitializeComponent();
            this.clientFrm = clientFrm;
            this.clientHandler = clientHandler;
            btnDisconnect.Enabled = false;
            //clientHandler.downChannelList(((LoggerFrm)clientFrm.downloadFrm).tbIP.Text.ToString());
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string response = clientHandler.establishConnect();
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

        private void soundBtn_Click(object sender, EventArgs e)
        {
            clientHandler.getMute();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            btnDisconnect.Enabled = false;
            clientHandler.closeAllThread();
            btnConnect.Enabled = true;
        }

        private void btnStatistic_Click(object sender, EventArgs e)
        {
            btnStatistic.Enabled = false;
            clientHandler.startStatistic((Int32)nudStatisticPort.Value);
        }



        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            clientHandler.setVolume(trackBar1.Value);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (clientHandler.downChannelList(((LoggerFrm)clientFrm.downloadFrm).tbIP.Text.ToString()))
                ((ControlFrm)clientFrm.controlFrm).cbChannel.SelectedIndex = 0;

        }

       
    }
}
