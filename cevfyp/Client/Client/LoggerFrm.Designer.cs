﻿namespace Client
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
            this.SuspendLayout();
            // 
            // rtbdownload
            // 
            this.rtbdownload.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbdownload.Location = new System.Drawing.Point(12, 12);
            this.rtbdownload.Name = "rtbdownload";
            this.rtbdownload.Size = new System.Drawing.Size(144, 287);
            this.rtbdownload.TabIndex = 0;
            this.rtbdownload.Text = "";
            // 
            // LoggerFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(168, 311);
            this.Controls.Add(this.rtbdownload);
            this.Name = "LoggerFrm";
            this.Text = "Logger";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox rtbdownload;
    }
}