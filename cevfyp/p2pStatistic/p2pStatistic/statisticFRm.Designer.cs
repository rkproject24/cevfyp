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
            this.DL_Log = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.T1Sp = new System.Windows.Forms.Label();
            this.Download = new System.Windows.Forms.GroupBox();
            this.Upload = new System.Windows.Forms.GroupBox();
            this.UT = new System.Windows.Forms.Label();
            this.DT = new System.Windows.Forms.Label();
            this.TotalPL = new System.Windows.Forms.Label();
            this.TPL = new System.Windows.Forms.Label();
            this.TPP = new System.Windows.Forms.Label();
            this.TotalPP = new System.Windows.Forms.Label();
            this.TPM = new System.Windows.Forms.Label();
            this.TotalPM = new System.Windows.Forms.Label();
            this.TPR = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.TOU = new System.Windows.Forms.Label();
            this.TOD = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.Download.SuspendLayout();
            this.Upload.SuspendLayout();
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
            this.btnStart.Location = new System.Drawing.Point(194, 54);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(53, 21);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(196, 26);
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
            this.btnReset.Location = new System.Drawing.Point(253, 54);
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
            this.DownLog.Location = new System.Drawing.Point(0, 11);
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
            this.UpLog.Size = new System.Drawing.Size(175, 213);
            this.UpLog.TabIndex = 12;
            // 
            // DL_Log
            // 
            this.DL_Log.AutoSize = true;
            this.DL_Log.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.DL_Log.Location = new System.Drawing.Point(12, 3);
            this.DL_Log.Name = "DL_Log";
            this.DL_Log.Size = new System.Drawing.Size(87, 12);
            this.DL_Log.TabIndex = 13;
            this.DL_Log.Text = "Download Log";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(12, 260);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "Upload Log";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(194, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Listen Port";
            // 
            // T1Sp
            // 
            this.T1Sp.AutoSize = true;
            this.T1Sp.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.T1Sp.Location = new System.Drawing.Point(186, 89);
            this.T1Sp.Name = "T1Sp";
            this.T1Sp.Size = new System.Drawing.Size(69, 12);
            this.T1Sp.TabIndex = 16;
            this.T1Sp.Text = "Avg Speed ";
            // 
            // Download
            // 
            this.Download.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Download.Controls.Add(this.btnStart);
            this.Download.Controls.Add(this.btnReset);
            this.Download.Controls.Add(this.DownLog);
            this.Download.Controls.Add(this.label1);
            this.Download.Controls.Add(this.nudPort);
            this.Download.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Download.Location = new System.Drawing.Point(373, 15);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(328, 239);
            this.Download.TabIndex = 22;
            this.Download.TabStop = false;
            this.Download.Text = "Download";
            // 
            // Upload
            // 
            this.Upload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Upload.Controls.Add(this.UpLog);
            this.Upload.Controls.Add(this.T1Sp);
            this.Upload.Controls.Add(this.label4);
            this.Upload.Controls.Add(this.DT);
            this.Upload.Controls.Add(this.UT);
            this.Upload.Controls.Add(this.label5);
            this.Upload.Controls.Add(this.TotalPL);
            this.Upload.Controls.Add(this.TOU);
            this.Upload.Controls.Add(this.TPL);
            this.Upload.Controls.Add(this.TOD);
            this.Upload.Controls.Add(this.TotalPP);
            this.Upload.Controls.Add(this.label12);
            this.Upload.Controls.Add(this.TPP);
            this.Upload.Controls.Add(this.label13);
            this.Upload.Controls.Add(this.TotalPM);
            this.Upload.Controls.Add(this.label14);
            this.Upload.Controls.Add(this.TPM);
            this.Upload.Controls.Add(this.label10);
            this.Upload.Controls.Add(this.TPR);
            this.Upload.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Upload.Location = new System.Drawing.Point(367, 275);
            this.Upload.Name = "Upload";
            this.Upload.Size = new System.Drawing.Size(351, 251);
            this.Upload.TabIndex = 23;
            this.Upload.TabStop = false;
            this.Upload.Text = "Upoad";
            // 
            // UT
            // 
            this.UT.AutoSize = true;
            this.UT.ForeColor = System.Drawing.Color.Red;
            this.UT.Location = new System.Drawing.Point(281, 119);
            this.UT.Name = "UT";
            this.UT.Size = new System.Drawing.Size(48, 12);
            this.UT.TabIndex = 30;
            this.UT.Text = "Avg US";
            // 
            // DT
            // 
            this.DT.AutoSize = true;
            this.DT.ForeColor = System.Drawing.Color.Red;
            this.DT.Location = new System.Drawing.Point(281, 107);
            this.DT.Name = "DT";
            this.DT.Size = new System.Drawing.Size(48, 12);
            this.DT.TabIndex = 27;
            this.DT.Text = "Avg DS";
            // 
            // TotalPL
            // 
            this.TotalPL.AutoSize = true;
            this.TotalPL.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TotalPL.Location = new System.Drawing.Point(184, 138);
            this.TotalPL.Name = "TotalPL";
            this.TotalPL.Size = new System.Drawing.Size(78, 12);
            this.TotalPL.TabIndex = 32;
            this.TotalPL.Text = "Tol Pkt Lost:";
            // 
            // TPL
            // 
            this.TPL.AutoSize = true;
            this.TPL.ForeColor = System.Drawing.Color.Red;
            this.TPL.Location = new System.Drawing.Point(279, 138);
            this.TPL.Name = "TPL";
            this.TPL.Size = new System.Drawing.Size(89, 12);
            this.TPL.TabIndex = 33;
            this.TPL.Text = "Packet Number";
            // 
            // TPP
            // 
            this.TPP.AutoSize = true;
            this.TPP.ForeColor = System.Drawing.Color.Red;
            this.TPP.Location = new System.Drawing.Point(279, 161);
            this.TPP.Name = "TPP";
            this.TPP.Size = new System.Drawing.Size(89, 12);
            this.TPP.TabIndex = 35;
            this.TPP.Text = "Packet Number";
            // 
            // TotalPP
            // 
            this.TotalPP.AutoSize = true;
            this.TotalPP.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TotalPP.Location = new System.Drawing.Point(184, 161);
            this.TotalPP.Name = "TotalPP";
            this.TotalPP.Size = new System.Drawing.Size(89, 12);
            this.TotalPP.TabIndex = 34;
            this.TotalPP.Text = "Tol Pkt Pulled:";
            // 
            // TPM
            // 
            this.TPM.AutoSize = true;
            this.TPM.ForeColor = System.Drawing.Color.Red;
            this.TPM.Location = new System.Drawing.Point(279, 183);
            this.TPM.Name = "TPM";
            this.TPM.Size = new System.Drawing.Size(89, 12);
            this.TPM.TabIndex = 37;
            this.TPM.Text = "Packet Number";
            // 
            // TotalPM
            // 
            this.TotalPM.AutoSize = true;
            this.TotalPM.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TotalPM.Location = new System.Drawing.Point(184, 183);
            this.TotalPM.Name = "TotalPM";
            this.TotalPM.Size = new System.Drawing.Size(79, 12);
            this.TotalPM.TabIndex = 36;
            this.TotalPM.Text = "Tol Pkt Miss:";
            // 
            // TPR
            // 
            this.TPR.AutoSize = true;
            this.TPR.ForeColor = System.Drawing.Color.Red;
            this.TPR.Location = new System.Drawing.Point(280, 206);
            this.TPR.Name = "TPR";
            this.TPR.Size = new System.Drawing.Size(89, 12);
            this.TPR.TabIndex = 39;
            this.TPR.Text = "Packet Number";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label10.Location = new System.Drawing.Point(184, 206);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 12);
            this.label10.TabIndex = 38;
            this.label10.Text = "Tol Pkt Recover:";
            // 
            // TOU
            // 
            this.TOU.AutoSize = true;
            this.TOU.ForeColor = System.Drawing.Color.Red;
            this.TOU.Location = new System.Drawing.Point(278, 66);
            this.TOU.Name = "TOU";
            this.TOU.Size = new System.Drawing.Size(31, 12);
            this.TOU.TabIndex = 46;
            this.TOU.Text = "TOU";
            // 
            // TOD
            // 
            this.TOD.AutoSize = true;
            this.TOD.ForeColor = System.Drawing.Color.Red;
            this.TOD.Location = new System.Drawing.Point(278, 54);
            this.TOD.Name = "TOD";
            this.TOD.Size = new System.Drawing.Size(31, 12);
            this.TOD.TabIndex = 45;
            this.TOD.Text = "TOD";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(193, 66);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 12);
            this.label12.TabIndex = 43;
            this.label12.Text = "Upload:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(193, 54);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 42;
            this.label13.Text = "Download:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label14.Location = new System.Drawing.Point(186, 36);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(91, 12);
            this.label14.TabIndex = 41;
            this.label14.Text = "Total Overhead";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(193, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 12);
            this.label4.TabIndex = 48;
            this.label4.Text = "Upload:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(193, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 47;
            this.label5.Text = "Download:";
            // 
            // statisticFRm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ClientSize = new System.Drawing.Size(734, 519);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.zedGraphCon1);
            this.Controls.Add(this.DL_Log);
            this.Controls.Add(this.zedGraphCon);
            this.Controls.Add(this.Download);
            this.Controls.Add(this.Upload);
            this.MaximizeBox = false;
            this.Name = "statisticFRm";
            this.Text = "Statistic";
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.Download.ResumeLayout(false);
            this.Download.PerformLayout();
            this.Upload.ResumeLayout(false);
            this.Upload.PerformLayout();
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

        public void updateUpSpeed(string US)
        {
            this.UT.Text = US;
        }

        public void updateDownSpeed(string DS)
        {
            this.DT.Text = DS;
        }

        public void updateTPL(string PL)
        {
            this.TPL.Text = PL;
        }

        public void updateTPP(string PP)
        {
            this.TPP.Text = PP;
        }

        public void updateTPM(string PM)
        {
            this.TPM.Text = PM;
        }

        public void updateTPR(string PR)
        {
            this.TPR.Text = PR;
        }

        public void updateTOD(string OD)
        {
            this.TOD.Text = OD;
        }

        public void updateTOU(string OU)
        {
            this.TOU.Text = OU;
        }

        private ZedGraph.ZedGraphControl zedGraphCon;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btnReset;
        private ZedGraph.ZedGraphControl zedGraphCon1;
        private System.Windows.Forms.TextBox DownLog;
        private System.Windows.Forms.TextBox UpLog;
        private System.Windows.Forms.Label DL_Log;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label T1Sp;
        private System.Windows.Forms.GroupBox Download;
        private System.Windows.Forms.GroupBox Upload;
        private System.Windows.Forms.Label UT;
        private System.Windows.Forms.Label DT;
        private System.Windows.Forms.Label TotalPL;
        private System.Windows.Forms.Label TPL;
        private System.Windows.Forms.Label TPP;
        private System.Windows.Forms.Label TotalPP;
        private System.Windows.Forms.Label TPM;
        private System.Windows.Forms.Label TotalPM;
        private System.Windows.Forms.Label TPR;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label TOU;
        private System.Windows.Forms.Label TOD;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}