namespace p2pStatistic
{
    partial class statisticFRm
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
            this.components = new System.ComponentModel.Container();
            this.zedGraphCon = new ZedGraph.ZedGraphControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnStart = new System.Windows.Forms.Button();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.btnReset = new System.Windows.Forms.Button();
            this.zedGraphCon1 = new ZedGraph.ZedGraphControl();
            this.DownLog = new System.Windows.Forms.TextBox();
            this.UpLog = new System.Windows.Forms.TextBox();
            this.DL = new System.Windows.Forms.Label();
            this.US = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.T1Sp = new System.Windows.Forms.Label();
            this.Download_Log = new System.Windows.Forms.GroupBox();
            this.Upload_Log = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DT = new System.Windows.Forms.Label();
            this.UT = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.TotalPP = new System.Windows.Forms.Label();
            this.TPP = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.Download_Log.SuspendLayout();
            this.Upload_Log.SuspendLayout();
            this.SuspendLayout();
            // 
            // zedGraphCon
            // 
            this.zedGraphCon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphCon.Location = new System.Drawing.Point(13, 15);
            this.zedGraphCon.Name = "zedGraphCon";
            this.zedGraphCon.ScrollGrace = 0;
            this.zedGraphCon.ScrollMaxX = 0;
            this.zedGraphCon.ScrollMaxY = 0;
            this.zedGraphCon.ScrollMaxY2 = 0;
            this.zedGraphCon.ScrollMinX = 0;
            this.zedGraphCon.ScrollMinY = 0;
            this.zedGraphCon.ScrollMinY2 = 0;
            this.zedGraphCon.Size = new System.Drawing.Size(348, 239);
            this.zedGraphCon.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Interval = 1500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(213, 78);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(53, 21);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(215, 50);
            this.nudPort.Maximum = new decimal(new int[] {
            65532,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(53, 22);
            this.nudPort.TabIndex = 4;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(272, 78);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(53, 21);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // zedGraphCon1
            // 
            this.zedGraphCon1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphCon1.Location = new System.Drawing.Point(12, 275);
            this.zedGraphCon1.Name = "zedGraphCon1";
            this.zedGraphCon1.ScrollGrace = 0;
            this.zedGraphCon1.ScrollMaxX = 0;
            this.zedGraphCon1.ScrollMaxY = 0;
            this.zedGraphCon1.ScrollMaxY2 = 0;
            this.zedGraphCon1.ScrollMinX = 0;
            this.zedGraphCon1.ScrollMinY = 0;
            this.zedGraphCon1.ScrollMinY2 = 0;
            this.zedGraphCon1.Size = new System.Drawing.Size(349, 236);
            this.zedGraphCon1.TabIndex = 7;
            // 
            // DownLog
            // 
            this.DownLog.AcceptsTab = true;
            this.DownLog.AllowDrop = true;
            this.DownLog.Location = new System.Drawing.Point(5, 13);
            this.DownLog.Multiline = true;
            this.DownLog.Name = "DownLog";
            this.DownLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DownLog.Size = new System.Drawing.Size(188, 214);
            this.DownLog.TabIndex = 11;
            // 
            // UpLog
            // 
            this.UpLog.Location = new System.Drawing.Point(6, 19);
            this.UpLog.Multiline = true;
            this.UpLog.Name = "UpLog";
            this.UpLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.UpLog.Size = new System.Drawing.Size(187, 213);
            this.UpLog.TabIndex = 12;
            // 
            // DL
            // 
            this.DL.AutoSize = true;
            this.DL.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.DL.Location = new System.Drawing.Point(12, 3);
            this.DL.Name = "DL";
            this.DL.Size = new System.Drawing.Size(61, 12);
            this.DL.TabIndex = 13;
            this.DL.Text = "Download";
            // 
            // US
            // 
            this.US.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.US.AutoSize = true;
            this.US.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.US.Location = new System.Drawing.Point(12, 260);
            this.US.Name = "US";
            this.US.Size = new System.Drawing.Size(45, 12);
            this.US.TabIndex = 14;
            this.US.Text = "Upload";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(213, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Listen Port";
            // 
            // T1Sp
            // 
            this.T1Sp.AutoSize = true;
            this.T1Sp.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.T1Sp.Location = new System.Drawing.Point(196, 89);
            this.T1Sp.Name = "T1Sp";
            this.T1Sp.Size = new System.Drawing.Size(69, 12);
            this.T1Sp.TabIndex = 16;
            this.T1Sp.Text = "Avg Speed ";
            // 
            // Download_Log
            // 
            this.Download_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Download_Log.Controls.Add(this.btnStart);
            this.Download_Log.Controls.Add(this.btnReset);
            this.Download_Log.Controls.Add(this.DownLog);
            this.Download_Log.Controls.Add(this.label1);
            this.Download_Log.Controls.Add(this.nudPort);
            this.Download_Log.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Download_Log.Location = new System.Drawing.Point(367, 15);
            this.Download_Log.Name = "Download_Log";
            this.Download_Log.Size = new System.Drawing.Size(363, 239);
            this.Download_Log.TabIndex = 22;
            this.Download_Log.TabStop = false;
            this.Download_Log.Text = "Download Log";
            // 
            // Upload_Log
            // 
            this.Upload_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Upload_Log.Controls.Add(this.UpLog);
            this.Upload_Log.Controls.Add(this.T1Sp);
            this.Upload_Log.Controls.Add(this.label4);
            this.Upload_Log.Controls.Add(this.DT);
            this.Upload_Log.Controls.Add(this.UT);
            this.Upload_Log.Controls.Add(this.label5);
            this.Upload_Log.Controls.Add(this.TotalPP);
            this.Upload_Log.Controls.Add(this.TPP);
            this.Upload_Log.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Upload_Log.Location = new System.Drawing.Point(367, 266);
            this.Upload_Log.Name = "Upload_Log";
            this.Upload_Log.Size = new System.Drawing.Size(365, 251);
            this.Upload_Log.TabIndex = 23;
            this.Upload_Log.TabStop = false;
            this.Upload_Log.Text = "Upoad Log";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(203, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 12);
            this.label4.TabIndex = 48;
            this.label4.Text = "Upload:";
            // 
            // DT
            // 
            this.DT.AutoSize = true;
            this.DT.ForeColor = System.Drawing.Color.Red;
            this.DT.Location = new System.Drawing.Point(291, 107);
            this.DT.Name = "DT";
            this.DT.Size = new System.Drawing.Size(48, 12);
            this.DT.TabIndex = 27;
            this.DT.Text = "Avg DS";
            // 
            // UT
            // 
            this.UT.AutoSize = true;
            this.UT.ForeColor = System.Drawing.Color.Red;
            this.UT.Location = new System.Drawing.Point(291, 119);
            this.UT.Name = "UT";
            this.UT.Size = new System.Drawing.Size(48, 12);
            this.UT.TabIndex = 30;
            this.UT.Text = "Avg US";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(203, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 47;
            this.label5.Text = "Download:";
            // 
            // TotalPP
            // 
            this.TotalPP.AutoSize = true;
            this.TotalPP.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TotalPP.Location = new System.Drawing.Point(194, 161);
            this.TotalPP.Name = "TotalPP";
            this.TotalPP.Size = new System.Drawing.Size(89, 12);
            this.TotalPP.TabIndex = 34;
            this.TotalPP.Text = "Tol Pkt Pulled:";
            // 
            // TPP
            // 
            this.TPP.AutoSize = true;
            this.TPP.ForeColor = System.Drawing.Color.Red;
            this.TPP.Location = new System.Drawing.Point(292, 161);
            this.TPP.Name = "TPP";
            this.TPP.Size = new System.Drawing.Size(71, 12);
            this.TPP.TabIndex = 35;
            this.TPP.Text = "Pkt Number";
            // 
            // statisticFRm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ClientSize = new System.Drawing.Size(734, 519);
            this.Controls.Add(this.US);
            this.Controls.Add(this.zedGraphCon1);
            this.Controls.Add(this.DL);
            this.Controls.Add(this.zedGraphCon);
            this.Controls.Add(this.Download_Log);
            this.Controls.Add(this.Upload_Log);
            this.MaximizeBox = false;
            this.Name = "statisticFRm";
            this.Text = "Statistic";
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.Download_Log.ResumeLayout(false);
            this.Download_Log.PerformLayout();
            this.Upload_Log.ResumeLayout(false);
            this.Upload_Log.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public void updateDownLog(string message)
        {
            this.DownLog.AppendText(message);
        }


        public void updateUpLog(string message)
        {
            this.UpLog.AppendText(message);
        }

        public void resetDownLog()
        {
            this.DownLog.Text = "";
        }


        public void resetUpLog()
        {
            this.UpLog.Text = "";
        }

        public void updateUpSpeed(string US)
        {
            this.UT.Text = US;
        }

        public void updateDownSpeed(string DS)
        {
            this.DT.Text = DS;
        }

        //public void updateTPL(string PL)
        //{
        //    this.TPL.Text = PL;
        //}

        public void updateTPP(string PP)
        {
            this.TPP.Text = PP;
        }

        //public void updateTPM(string PM)
        //{
        //    this.TPM.Text = PM;
        //}

        //public void updateTPR(string PR)
        //{
        //    this.TPR.Text = PR;
        //}

        //public void updateTOD(string OD)
        //{
        //    this.TOD.Text = OD;
        //}

        //public void updateTOU(string OU)
        //{
        //    this.TOU.Text = OU;
        //}

        private ZedGraph.ZedGraphControl zedGraphCon;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btnReset;
        private ZedGraph.ZedGraphControl zedGraphCon1;
        private System.Windows.Forms.TextBox DownLog;
        private System.Windows.Forms.TextBox UpLog;
        private System.Windows.Forms.Label DL;
        private System.Windows.Forms.Label US;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label T1Sp;
        private System.Windows.Forms.GroupBox Download_Log;
        private System.Windows.Forms.GroupBox Upload_Log;
        private System.Windows.Forms.Label UT;
        private System.Windows.Forms.Label DT;
        private System.Windows.Forms.Label TPP;
        private System.Windows.Forms.Label TotalPP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}