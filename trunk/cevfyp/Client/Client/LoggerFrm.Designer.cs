namespace Client
{
    partial class LoggerFrm
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
            this.rtbdownload = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lbSpeed = new System.Windows.Forms.Label();
            this.rtbpull = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtbdownload
            // 
            this.rtbdownload.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbdownload.Location = new System.Drawing.Point(14, 12);
            this.rtbdownload.Name = "rtbdownload";
            this.rtbdownload.Size = new System.Drawing.Size(144, 211);
            this.rtbdownload.TabIndex = 0;
            this.rtbdownload.Text = "";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(9, 368);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP";
            // 
            // tbIP
            // 
            this.tbIP.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tbIP.Location = new System.Drawing.Point(69, 365);
            this.tbIP.Name = "tbIP";
            this.tbIP.Size = new System.Drawing.Size(87, 22);
            this.tbIP.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(12, 388);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Speed";
            // 
            // lbSpeed
            // 
            this.lbSpeed.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lbSpeed.AutoSize = true;
            this.lbSpeed.ForeColor = System.Drawing.Color.Red;
            this.lbSpeed.Location = new System.Drawing.Point(66, 388);
            this.lbSpeed.Name = "lbSpeed";
            this.lbSpeed.Size = new System.Drawing.Size(42, 12);
            this.lbSpeed.TabIndex = 4;
            this.lbSpeed.Text = "lbSpeed";
            // 
            // rtbpull
            // 
            this.rtbpull.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbpull.Location = new System.Drawing.Point(14, 229);
            this.rtbpull.Name = "rtbpull";
            this.rtbpull.Size = new System.Drawing.Size(144, 99);
            this.rtbpull.TabIndex = 5;
            this.rtbpull.Text = "";
            // 
            // LoggerFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(168, 408);
            this.Controls.Add(this.rtbpull);
            this.Controls.Add(this.lbSpeed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbdownload);
            this.Name = "LoggerFrm";
            this.Text = "Logger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.RichTextBox rtbdownload;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tbIP;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label lbSpeed;
        public System.Windows.Forms.RichTextBox rtbpull;
    }
}