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
            this.Download_Log = new System.Windows.Forms.GroupBox();
            this.Upload_Log = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ReceiveNull = new System.Windows.Forms.Label();
            this.ReceiveChunk = new System.Windows.Forms.Label();
            this.CTPB = new System.Windows.Forms.Label();
            this.RecCount = new System.Windows.Forms.Label();
            this.WaitMiss = new System.Windows.Forms.Label();
            this.PushMiss = new System.Windows.Forms.Label();
            this.RecMiss = new System.Windows.Forms.Label();
            this.Title = new System.Windows.Forms.Label();
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
            this.btnStart.Location = new System.Drawing.Point(216, 119);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(53, 21);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(218, 91);
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
            this.btnReset.Location = new System.Drawing.Point(275, 119);
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
            this.label1.Location = new System.Drawing.Point(216, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Listen Port";
            // 
            // Download_Log
            // 
            this.Download_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Download_Log.Controls.Add(this.DownLog);
            this.Download_Log.Controls.Add(this.label8);
            this.Download_Log.Controls.Add(this.Title);
            this.Download_Log.Controls.Add(this.label3);
            this.Download_Log.Controls.Add(this.label2);
            this.Download_Log.Controls.Add(this.RecMiss);
            this.Download_Log.Controls.Add(this.label4);
            this.Download_Log.Controls.Add(this.ReceiveNull);
            this.Download_Log.Controls.Add(this.label7);
            this.Download_Log.Controls.Add(this.PushMiss);
            this.Download_Log.Controls.Add(this.RecCount);
            this.Download_Log.Controls.Add(this.label5);
            this.Download_Log.Controls.Add(this.CTPB);
            this.Download_Log.Controls.Add(this.ReceiveChunk);
            this.Download_Log.Controls.Add(this.label6);
            this.Download_Log.Controls.Add(this.WaitMiss);
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
            this.Upload_Log.Controls.Add(this.btnStart);
            this.Upload_Log.Controls.Add(this.btnReset);
            this.Upload_Log.Controls.Add(this.UpLog);
            this.Upload_Log.Controls.Add(this.label1);
            this.Upload_Log.Controls.Add(this.nudPort);
            this.Upload_Log.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Upload_Log.Location = new System.Drawing.Point(367, 266);
            this.Upload_Log.Name = "Upload_Log";
            this.Upload_Log.Size = new System.Drawing.Size(365, 251);
            this.Upload_Log.TabIndex = 23;
            this.Upload_Log.TabStop = false;
            this.Upload_Log.Text = "Upoad Log";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(309, 184);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 12);
            this.label8.TabIndex = 27;
            this.label8.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(310, 162);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(12, 12);
            this.label7.TabIndex = 26;
            this.label7.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(310, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(12, 12);
            this.label6.TabIndex = 25;
            this.label6.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(309, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 12);
            this.label5.TabIndex = 24;
            this.label5.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(310, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 12);
            this.label4.TabIndex = 23;
            this.label4.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(310, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 12);
            this.label3.TabIndex = 22;
            this.label3.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(310, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 12);
            this.label2.TabIndex = 21;
            this.label2.Text = "0";
            // 
            // ReceiveNull
            // 
            this.ReceiveNull.AutoSize = true;
            this.ReceiveNull.Location = new System.Drawing.Point(214, 184);
            this.ReceiveNull.Name = "ReceiveNull";
            this.ReceiveNull.Size = new System.Drawing.Size(81, 12);
            this.ReceiveNull.TabIndex = 20;
            this.ReceiveNull.Text = "ReceiveNull :";
            // 
            // ReceiveChunk
            // 
            this.ReceiveChunk.AutoSize = true;
            this.ReceiveChunk.Location = new System.Drawing.Point(214, 162);
            this.ReceiveChunk.Name = "ReceiveChunk";
            this.ReceiveChunk.Size = new System.Drawing.Size(94, 12);
            this.ReceiveChunk.TabIndex = 19;
            this.ReceiveChunk.Text = "ReceiveChunk :";
            // 
            // CTPB
            // 
            this.CTPB.AutoSize = true;
            this.CTPB.Location = new System.Drawing.Point(214, 141);
            this.CTPB.Name = "CTPB";
            this.CTPB.Size = new System.Drawing.Size(46, 12);
            this.CTPB.TabIndex = 18;
            this.CTPB.Text = "CTPB :";
            // 
            // RecCount
            // 
            this.RecCount.AutoSize = true;
            this.RecCount.Location = new System.Drawing.Point(214, 120);
            this.RecCount.Name = "RecCount";
            this.RecCount.Size = new System.Drawing.Size(68, 12);
            this.RecCount.TabIndex = 17;
            this.RecCount.Text = "RecCount :";
            // 
            // WaitMiss
            // 
            this.WaitMiss.AutoSize = true;
            this.WaitMiss.Location = new System.Drawing.Point(214, 97);
            this.WaitMiss.Name = "WaitMiss";
            this.WaitMiss.Size = new System.Drawing.Size(64, 12);
            this.WaitMiss.TabIndex = 16;
            this.WaitMiss.Text = "WaitMiss :";
            // 
            // PushMiss
            // 
            this.PushMiss.AutoSize = true;
            this.PushMiss.Location = new System.Drawing.Point(214, 73);
            this.PushMiss.Name = "PushMiss";
            this.PushMiss.Size = new System.Drawing.Size(64, 12);
            this.PushMiss.TabIndex = 15;
            this.PushMiss.Text = "PushMiss :";
            // 
            // RecMiss
            // 
            this.RecMiss.AutoSize = true;
            this.RecMiss.Location = new System.Drawing.Point(214, 51);
            this.RecMiss.Name = "RecMiss";
            this.RecMiss.Size = new System.Drawing.Size(59, 12);
            this.RecMiss.TabIndex = 14;
            this.RecMiss.Text = "RecMiss :";
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Location = new System.Drawing.Point(216, 18);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(93, 12);
            this.Title.TabIndex = 13;
            this.Title.Text = "Statistic Record";
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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

        public void updatel2(string l2)
        {
            this.label2.Text = l2;
        }

        public void updatel3(string l3)
        {
            this.label3.Text = l3;
        }

        public void updatel4(string l4)
        {
            this.label4.Text = l4;
        }

        public void updatel5(string l5)
        {
            this.label5.Text = l5;
        }

        public void updatel6(string l6)
        {
            this.label6.Text = l6;
        }

        public void updatel7(string l7)
        {
            this.label7.Text = l7;
        }

        public void updatel8(string l8)
        {
            this.label8.Text = l8;
        }

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
        private System.Windows.Forms.GroupBox Download_Log;
        private System.Windows.Forms.GroupBox Upload_Log;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label ReceiveNull;
        private System.Windows.Forms.Label ReceiveChunk;
        private System.Windows.Forms.Label CTPB;
        private System.Windows.Forms.Label RecCount;
        private System.Windows.Forms.Label WaitMiss;
        private System.Windows.Forms.Label PushMiss;
        private System.Windows.Forms.Label RecMiss;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}