namespace Client
{
    partial class ClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbWriteStatus = new System.Windows.Forms.TextBox();
            this.tbReadStatus = new System.Windows.Forms.TextBox();
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.tbServerIp = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.preferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferenceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rtbupload = new System.Windows.Forms.RichTextBox();
            this.btnListenPeer = new System.Windows.Forms.Button();
            this.tbhostIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rtbdownload = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(129, 55);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(322, 260);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(5, 324);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(54, 25);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbWriteStatus
            // 
            this.tbWriteStatus.Location = new System.Drawing.Point(137, 325);
            this.tbWriteStatus.Name = "tbWriteStatus";
            this.tbWriteStatus.Size = new System.Drawing.Size(83, 20);
            this.tbWriteStatus.TabIndex = 3;
            // 
            // tbReadStatus
            // 
            this.tbReadStatus.Location = new System.Drawing.Point(226, 325);
            this.tbReadStatus.Name = "tbReadStatus";
            this.tbReadStatus.Size = new System.Drawing.Size(100, 20);
            this.tbReadStatus.TabIndex = 4;
            // 
            // tbStatus
            // 
            this.tbStatus.Location = new System.Drawing.Point(226, 356);
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.Size = new System.Drawing.Size(100, 20);
            this.tbStatus.TabIndex = 5;
            // 
            // tbServerIp
            // 
            this.tbServerIp.Location = new System.Drawing.Point(137, 355);
            this.tbServerIp.Name = "tbServerIp";
            this.tbServerIp.Size = new System.Drawing.Size(83, 20);
            this.tbServerIp.TabIndex = 6;
            this.tbServerIp.Text = "192.168.1.3";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 353);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(43, 25);
            this.button2.TabIndex = 7;
            this.button2.Text = "mute";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 356);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Tracker ip";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(65, 324);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(66, 24);
            this.btnDisconnect.TabIndex = 9;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferenceToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(599, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // preferenceToolStripMenuItem
            // 
            this.preferenceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferenceToolStripMenuItem1});
            this.preferenceToolStripMenuItem.Name = "preferenceToolStripMenuItem";
            this.preferenceToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.preferenceToolStripMenuItem.Text = "Tools";
            // 
            // preferenceToolStripMenuItem1
            // 
            this.preferenceToolStripMenuItem1.Name = "preferenceToolStripMenuItem1";
            this.preferenceToolStripMenuItem1.Size = new System.Drawing.Size(130, 22);
            this.preferenceToolStripMenuItem1.Text = "Preference";
            this.preferenceToolStripMenuItem1.Click += new System.EventHandler(this.preferenceToolStripMenuItem1_Click);
            // 
            // rtbupload
            // 
            this.rtbupload.Location = new System.Drawing.Point(457, 54);
            this.rtbupload.Name = "rtbupload";
            this.rtbupload.Size = new System.Drawing.Size(136, 261);
            this.rtbupload.TabIndex = 11;
            this.rtbupload.Text = "";
            // 
            // btnListenPeer
            // 
            this.btnListenPeer.Location = new System.Drawing.Point(453, 353);
            this.btnListenPeer.Name = "btnListenPeer";
            this.btnListenPeer.Size = new System.Drawing.Size(75, 25);
            this.btnListenPeer.TabIndex = 12;
            this.btnListenPeer.Text = "ListenPeer";
            this.btnListenPeer.UseVisualStyleBackColor = true;
            this.btnListenPeer.Click += new System.EventHandler(this.btnListenPeer_Click);
            // 
            // tbhostIP
            // 
            this.tbhostIP.Location = new System.Drawing.Point(453, 325);
            this.tbhostIP.Name = "tbhostIP";
            this.tbhostIP.ReadOnly = true;
            this.tbhostIP.Size = new System.Drawing.Size(94, 20);
            this.tbhostIP.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(396, 329);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "HostingIP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(454, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Upload log";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Download log";
            // 
            // rtbdownload
            // 
            this.rtbdownload.Location = new System.Drawing.Point(5, 56);
            this.rtbdownload.Name = "rtbdownload";
            this.rtbdownload.Size = new System.Drawing.Size(118, 260);
            this.rtbdownload.TabIndex = 17;
            this.rtbdownload.Text = "";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 381);
            this.Controls.Add(this.rtbdownload);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbhostIP);
            this.Controls.Add(this.btnListenPeer);
            this.Controls.Add(this.rtbupload);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tbServerIp);
            this.Controls.Add(this.tbStatus);
            this.Controls.Add(this.tbReadStatus);
            this.Controls.Add(this.tbWriteStatus);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ClientForm";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button btnConnect;
        public System.Windows.Forms.TextBox tbWriteStatus;
        public System.Windows.Forms.TextBox tbReadStatus;
        public System.Windows.Forms.TextBox tbStatus;
        public System.Windows.Forms.TextBox tbServerIp;
        public System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem preferenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferenceToolStripMenuItem1;
        public System.Windows.Forms.RichTextBox rtbupload;
        private System.Windows.Forms.Button btnListenPeer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.RichTextBox rtbdownload;
        public System.Windows.Forms.TextBox tbhostIP;
    }
}

