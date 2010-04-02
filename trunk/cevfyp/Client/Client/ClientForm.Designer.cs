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
            this.components = new System.ComponentModel.Container();
            this._docker = new Crom.Controls.Docking.DockContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dowloadSpeedTimer = new System.Windows.Forms.Timer(this.components);
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.imageButton1 = new Andy.UI.ImageButton();
            this.imageButton2 = new Andy.UI.ImageButton();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
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
            this._docker.Size = new System.Drawing.Size(765, 405);
            this._docker.TabIndex = 0;
            this._docker.TitleBarGradientColor1 = System.Drawing.SystemColors.WindowFrame;
            this._docker.TitleBarGradientColor2 = System.Drawing.SystemColors.ControlDarkDark;
            this._docker.TitleBarGradientSelectedColor1 = System.Drawing.SystemColors.WindowFrame;
            this._docker.TitleBarGradientSelectedColor2 = System.Drawing.Color.Silver;
            this._docker.TitleBarTextColor = System.Drawing.Color.White;
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
            this.panel2.Size = new System.Drawing.Size(765, 405);
            this.panel2.TabIndex = 23;
            // 
            // dowloadSpeedTimer
            // 
            this.dowloadSpeedTimer.Enabled = true;
            this.dowloadSpeedTimer.Interval = 1000;
            this.dowloadSpeedTimer.Tick += new System.EventHandler(this.dowloadSpeedTimer_Tick);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(297, 24);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            65532,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown1.TabIndex = 28;
            this.numericUpDown1.Value = new decimal(new int[] {
            1701,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(353, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 23);
            this.button1.TabIndex = 27;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(294, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Statistic";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(152, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "ID";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(155, 24);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(70, 20);
            this.textBox4.TabIndex = 4;
            // 
            // imageButton1
            // 
            this.imageButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.imageButton1.ForeColor = System.Drawing.Color.Transparent;
            this.imageButton1.HoverImage = global::Client.Properties.Resources.Play_Hot_icon;
            this.imageButton1.Location = new System.Drawing.Point(3, 19);
            this.imageButton1.Name = "imageButton1";
            this.imageButton1.NormalImage = global::Client.Properties.Resources.Play_Normal_icon;
            this.imageButton1.PushedImage = global::Client.Properties.Resources.Play_Pressed_icon;
            this.imageButton1.Size = new System.Drawing.Size(43, 31);
            this.imageButton1.TabIndex = 23;
            this.imageButton1.Text = "imageButton1";
            // 
            // imageButton2
            // 
            this.imageButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.imageButton2.DisabledImage = global::Client.Properties.Resources.Stop_Normal_Blue_icon;
            this.imageButton2.HoverImage = global::Client.Properties.Resources.Stop_Hot_icon;
            this.imageButton2.Location = new System.Drawing.Point(52, 19);
            this.imageButton2.Name = "imageButton2";
            this.imageButton2.NormalImage = global::Client.Properties.Resources.Stop_Normal_Blue_icon;
            this.imageButton2.PushedImage = global::Client.Properties.Resources.Stop_Pressed_Blue_icon;
            this.imageButton2.Size = new System.Drawing.Size(45, 29);
            this.imageButton2.TabIndex = 23;
            this.imageButton2.Text = "imageButton1";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(769, 468);
            this.Controls.Add(this.panel2);
            this.Name = "ClientForm";
            this.Padding = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Crom.Controls.Docking.DockContainer _docker;
        public System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Timer dowloadSpeedTimer;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Andy.UI.ImageButton imageButton1;
        private Andy.UI.ImageButton imageButton2;
        private System.Windows.Forms.TextBox textBox4;
    }
}

