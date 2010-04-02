namespace Client
{
    partial class ControlFrm
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
            this.lbTree0 = new System.Windows.Forms.Label();
            this.lbTree1 = new System.Windows.Forms.Label();
            this.lbTree2 = new System.Windows.Forms.Label();
            this.nudStatisticPort = new System.Windows.Forms.NumericUpDown();
            this.btnStatistic = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lbPlaySeq = new System.Windows.Forms.Label();
            this.PullChb = new System.Windows.Forms.CheckBox();
            this.soundBtn = new Andy.UI.ImageButton();
            this.btnDisconnect = new Andy.UI.ImageButton();
            this.btnConnect = new Andy.UI.ImageButton();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.cbChannel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudStatisticPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // lbTree0
            // 
            this.lbTree0.AutoSize = true;
            this.lbTree0.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbTree0.Location = new System.Drawing.Point(281, 20);
            this.lbTree0.Name = "lbTree0";
            this.lbTree0.Size = new System.Drawing.Size(35, 13);
            this.lbTree0.TabIndex = 26;
            this.lbTree0.Text = "Tree0";
            // 
            // lbTree1
            // 
            this.lbTree1.AutoSize = true;
            this.lbTree1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbTree1.Location = new System.Drawing.Point(281, 33);
            this.lbTree1.Name = "lbTree1";
            this.lbTree1.Size = new System.Drawing.Size(35, 13);
            this.lbTree1.TabIndex = 27;
            this.lbTree1.Text = "Tree1";
            // 
            // lbTree2
            // 
            this.lbTree2.AutoSize = true;
            this.lbTree2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbTree2.Location = new System.Drawing.Point(281, 48);
            this.lbTree2.Name = "lbTree2";
            this.lbTree2.Size = new System.Drawing.Size(35, 13);
            this.lbTree2.TabIndex = 28;
            this.lbTree2.Text = "Tree2";
            // 
            // nudStatisticPort
            // 
            this.nudStatisticPort.Location = new System.Drawing.Point(366, 17);
            this.nudStatisticPort.Maximum = new decimal(new int[] {
            65532,
            0,
            0,
            0});
            this.nudStatisticPort.Name = "nudStatisticPort";
            this.nudStatisticPort.Size = new System.Drawing.Size(50, 20);
            this.nudStatisticPort.TabIndex = 31;
            this.nudStatisticPort.Value = new decimal(new int[] {
            1701,
            0,
            0,
            0});
            // 
            // btnStatistic
            // 
            this.btnStatistic.ForeColor = System.Drawing.Color.Black;
            this.btnStatistic.Location = new System.Drawing.Point(366, 43);
            this.btnStatistic.Name = "btnStatistic";
            this.btnStatistic.Size = new System.Drawing.Size(50, 23);
            this.btnStatistic.TabIndex = 30;
            this.btnStatistic.Text = "Start";
            this.btnStatistic.UseVisualStyleBackColor = true;
            this.btnStatistic.Click += new System.EventHandler(this.btnStatistic_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(363, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Statistic";
            // 
            // lbPlaySeq
            // 
            this.lbPlaySeq.AutoSize = true;
            this.lbPlaySeq.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbPlaySeq.Location = new System.Drawing.Point(281, 1);
            this.lbPlaySeq.Name = "lbPlaySeq";
            this.lbPlaySeq.Size = new System.Drawing.Size(46, 13);
            this.lbPlaySeq.TabIndex = 33;
            this.lbPlaySeq.Text = "PlaySeq";
            // 
            // PullChb
            // 
            this.PullChb.AutoSize = true;
            this.PullChb.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.PullChb.Location = new System.Drawing.Point(182, 43);
            this.PullChb.Name = "PullChb";
            this.PullChb.Size = new System.Drawing.Size(73, 17);
            this.PullChb.TabIndex = 34;
            this.PullChb.Text = "Pull Mode";
            this.PullChb.UseVisualStyleBackColor = true;
            // 
            // soundBtn
            // 
            this.soundBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.soundBtn.ForeColor = System.Drawing.Color.Transparent;
            this.soundBtn.HoverImage = global::Client.Properties.Resources.Volume_Disabled_icon;
            this.soundBtn.Location = new System.Drawing.Point(212, 5);
            this.soundBtn.Name = "soundBtn";
            this.soundBtn.NormalImage = global::Client.Properties.Resources.Volume_Normal_Blue_icon;
            this.soundBtn.PushedImage = global::Client.Properties.Resources.Volume_Disabled_icon;
            this.soundBtn.Size = new System.Drawing.Size(43, 31);
            this.soundBtn.TabIndex = 35;
            this.soundBtn.Text = "imageButton1";
            this.soundBtn.Click += new System.EventHandler(this.soundBtn_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.btnDisconnect.DisabledImage = global::Client.Properties.Resources.Stop_Normal_Blue_icon;
            this.btnDisconnect.HoverImage = global::Client.Properties.Resources.Stop_Pressed_Blue_icon;
            this.btnDisconnect.Location = new System.Drawing.Point(51, 7);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.NormalImage = global::Client.Properties.Resources.Stop_Normal_Blue_icon;
            this.btnDisconnect.PushedImage = global::Client.Properties.Resources.Stop_Pressed_Blue_icon;
            this.btnDisconnect.Size = new System.Drawing.Size(45, 31);
            this.btnDisconnect.TabIndex = 25;
            this.btnDisconnect.Text = "imageButton1";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.btnConnect.ForeColor = System.Drawing.Color.Transparent;
            this.btnConnect.HoverImage = global::Client.Properties.Resources.Play_Hot_icon;
            this.btnConnect.Location = new System.Drawing.Point(2, 7);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.NormalImage = global::Client.Properties.Resources.Play_Normal_icon;
            this.btnConnect.PushedImage = global::Client.Properties.Resources.Play_Pressed_icon;
            this.btnConnect.Size = new System.Drawing.Size(43, 31);
            this.btnConnect.TabIndex = 24;
            this.btnConnect.Text = "imageButton1";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(102, 5);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(104, 45);
            this.trackBar1.TabIndex = 36;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // cbChannel
            // 
            this.cbChannel.FormattingEnabled = true;
            this.cbChannel.Location = new System.Drawing.Point(51, 41);
            this.cbChannel.Name = "cbChannel";
            this.cbChannel.Size = new System.Drawing.Size(64, 21);
            this.cbChannel.TabIndex = 37;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(-1, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Channel";
            // 
            // btnRefresh
            // 
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Location = new System.Drawing.Point(121, 39);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(55, 23);
            this.btnRefresh.TabIndex = 39;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // ControlFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(432, 73);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.PullChb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbChannel);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.soundBtn);
            this.Controls.Add(this.lbPlaySeq);
            this.Controls.Add(this.nudStatisticPort);
            this.Controls.Add(this.btnStatistic);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbTree2);
            this.Controls.Add(this.lbTree1);
            this.Controls.Add(this.lbTree0);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnConnect);
            this.Name = "ControlFrm";
            this.Text = "ControlFrm";
            ((System.ComponentModel.ISupportInitialize)(this.nudStatisticPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Andy.UI.ImageButton btnConnect;
        private Andy.UI.ImageButton btnDisconnect;
        public System.Windows.Forms.Label lbTree0;
        public System.Windows.Forms.Label lbTree1;
        public System.Windows.Forms.Label lbTree2;
        public System.Windows.Forms.NumericUpDown nudStatisticPort;
        private System.Windows.Forms.Button btnStatistic;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label lbPlaySeq;
        public System.Windows.Forms.CheckBox PullChb;
        private Andy.UI.ImageButton soundBtn;
        private System.Windows.Forms.TrackBar trackBar1;
        public System.Windows.Forms.ComboBox cbChannel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRefresh;
    }
}