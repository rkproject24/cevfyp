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
            this.tbReadStatus = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.btnConnect = new Andy.UI.ImageButton();
            this.btnDisconnect = new Andy.UI.ImageButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.nudStatisticPort = new System.Windows.Forms.NumericUpDown();
            this.btnStatistic = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this._docker = new Crom.Controls.Docking.DockContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStatisticPort)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbReadStatus
            // 
            this.tbReadStatus.Location = new System.Drawing.Point(155, 24);
            this.tbReadStatus.Name = "tbReadStatus";
            this.tbReadStatus.Size = new System.Drawing.Size(70, 20);
            this.tbReadStatus.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(103, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(43, 25);
            this.button2.TabIndex = 7;
            this.button2.Text = "mute";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(231, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(56, 20);
            this.textBox1.TabIndex = 18;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(231, 26);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(56, 20);
            this.textBox2.TabIndex = 19;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(231, 50);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(56, 20);
            this.textBox3.TabIndex = 20;
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.btnConnect.ForeColor = System.Drawing.Color.Transparent;
            this.btnConnect.HoverImage = global::Client.Properties.Resources.Play_Hot_icon;
            this.btnConnect.Location = new System.Drawing.Point(3, 19);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.NormalImage = global::Client.Properties.Resources.Play_Normal_icon;
            this.btnConnect.PushedImage = global::Client.Properties.Resources.Play_Pressed_icon;
            this.btnConnect.Size = new System.Drawing.Size(43, 31);
            this.btnConnect.TabIndex = 23;
            this.btnConnect.Text = "imageButton1";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.btnDisconnect.DisabledImage = global::Client.Properties.Resources.Stop_Normal_Blue_icon;
            this.btnDisconnect.HoverImage = global::Client.Properties.Resources.Stop_Hot_icon;
            this.btnDisconnect.Location = new System.Drawing.Point(52, 19);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.NormalImage = global::Client.Properties.Resources.Stop_Normal_Blue_icon;
            this.btnDisconnect.PushedImage = global::Client.Properties.Resources.Stop_Pressed_Blue_icon;
            this.btnDisconnect.Size = new System.Drawing.Size(45, 29);
            this.btnDisconnect.TabIndex = 23;
            this.btnDisconnect.Text = "imageButton1";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.nudStatisticPort);
            this.panel1.Controls.Add(this.btnStatistic);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.btnConnect);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.btnDisconnect);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.tbReadStatus);
            this.panel1.Location = new System.Drawing.Point(188, 364);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(409, 69);
            this.panel1.TabIndex = 22;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.checkBox1.Location = new System.Drawing.Point(105, 52);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(73, 17);
            this.checkBox1.TabIndex = 29;
            this.checkBox1.Text = "Pull Mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // nudStatisticPort
            // 
            this.nudStatisticPort.Location = new System.Drawing.Point(297, 24);
            this.nudStatisticPort.Maximum = new decimal(new int[] {
            65532,
            0,
            0,
            0});
            this.nudStatisticPort.Name = "nudStatisticPort";
            this.nudStatisticPort.Size = new System.Drawing.Size(50, 20);
            this.nudStatisticPort.TabIndex = 28;
            this.nudStatisticPort.Value = new decimal(new int[] {
            1701,
            0,
            0,
            0});
            // 
            // btnStatistic
            // 
            this.btnStatistic.Location = new System.Drawing.Point(353, 22);
            this.btnStatistic.Name = "btnStatistic";
            this.btnStatistic.Size = new System.Drawing.Size(50, 23);
            this.btnStatistic.TabIndex = 27;
            this.btnStatistic.Text = "Start";
            this.btnStatistic.UseVisualStyleBackColor = true;
            this.btnStatistic.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(294, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Statistic";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label5.Location = new System.Drawing.Point(152, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "ID";
            // 
            // _docker
            // 
            this._docker.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._docker.BackColor = System.Drawing.Color.Transparent;
            this._docker.CanMoveByMouseFilledForms = true;
            this._docker.ForeColor = System.Drawing.Color.Transparent;
            this._docker.Location = new System.Drawing.Point(0, 0);
            this._docker.Name = "_docker";
            this._docker.Size = new System.Drawing.Size(765, 400);
            this._docker.TabIndex = 0;
            this._docker.TitleBarGradientColor1 = System.Drawing.SystemColors.Control;
            this._docker.TitleBarGradientColor2 = System.Drawing.Color.White;
            this._docker.TitleBarGradientSelectedColor1 = System.Drawing.Color.DarkGray;
            this._docker.TitleBarGradientSelectedColor2 = System.Drawing.Color.White;
            this._docker.TitleBarTextColor = System.Drawing.Color.Black;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this._docker);
            this.panel2.Location = new System.Drawing.Point(2, 34);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(765, 400);
            this.panel2.TabIndex = 23;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(769, 463);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "ClientForm";
            this.Padding = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStatisticPort)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TextBox tbReadStatus;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.TextBox textBox2;
        public System.Windows.Forms.TextBox textBox3;
        private Andy.UI.ImageButton btnConnect;
        private Andy.UI.ImageButton btnDisconnect;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnStatistic;
        public System.Windows.Forms.NumericUpDown nudStatisticPort;
        public System.Windows.Forms.CheckBox checkBox1;
        private Crom.Controls.Docking.DockContainer _docker;
        public System.Windows.Forms.Panel panel2;
    }
}

