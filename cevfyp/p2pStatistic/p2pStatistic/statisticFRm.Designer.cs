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
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.SuspendLayout();
            // 
            // zedGraphCon
            // 
            this.zedGraphCon.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphCon.Location = new System.Drawing.Point(12, 12);
            this.zedGraphCon.Name = "zedGraphCon";
            this.zedGraphCon.ScrollGrace = 0;
            this.zedGraphCon.ScrollMaxX = 0;
            this.zedGraphCon.ScrollMaxY = 0;
            this.zedGraphCon.ScrollMaxY2 = 0;
            this.zedGraphCon.ScrollMinX = 0;
            this.zedGraphCon.ScrollMinY = 0;
            this.zedGraphCon.ScrollMinY2 = 0;
            this.zedGraphCon.Size = new System.Drawing.Size(382, 256);
            this.zedGraphCon.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 274);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Listen Port";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(151, 274);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(53, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(76, 274);
            this.nudPort.Maximum = new decimal(new int[] {
            65532,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(69, 20);
            this.nudPort.TabIndex = 4;
            // 
            // statisticFRm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 303);
            this.Controls.Add(this.nudPort);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.zedGraphCon);
            this.Name = "statisticFRm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphCon;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.NumericUpDown nudPort;
    }
}

