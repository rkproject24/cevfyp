﻿namespace Server
{
    partial class PreferenceFm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbChunkSize = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbRecStream = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbstreamport = new System.Windows.Forms.TextBox();
            this.tbvideodir = new System.Windows.Forms.TextBox();
            this.tbStreamType = new System.Windows.Forms.TextBox();
            this.tbPlugin = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbDefaultIp = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.NudMaxClient = new System.Windows.Forms.NumericUpDown();
            this.NudTreeSize = new System.Windows.Forms.NumericUpDown();
            this.tbConport = new System.Windows.Forms.TextBox();
            this.tbDataport = new System.Windows.Forms.TextBox();
            this.tbSLPort = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudMaxClient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudTreeSize)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Plugin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "StreamType";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Video Directory";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Stream Port";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "Server List Port";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "Data port";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "Control Port Base";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "MaxClient/Tree";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 119);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 12);
            this.label9.TabIndex = 8;
            this.label9.Text = "No. of Tree";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbChunkSize);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.tbRecStream);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.tbstreamport);
            this.groupBox1.Controls.Add(this.tbvideodir);
            this.groupBox1.Controls.Add(this.tbStreamType);
            this.groupBox1.Controls.Add(this.tbPlugin);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(267, 156);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local VLC";
            // 
            // tbChunkSize
            // 
            this.tbChunkSize.Location = new System.Drawing.Point(218, 116);
            this.tbChunkSize.Name = "tbChunkSize";
            this.tbChunkSize.Size = new System.Drawing.Size(43, 22);
            this.tbChunkSize.TabIndex = 11;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(148, 119);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 12);
            this.label12.TabIndex = 10;
            this.label12.Text = "Chunk SIZE";
            // 
            // tbRecStream
            // 
            this.tbRecStream.Location = new System.Drawing.Point(87, 116);
            this.tbRecStream.Name = "tbRecStream";
            this.tbRecStream.Size = new System.Drawing.Size(55, 22);
            this.tbRecStream.TabIndex = 9;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 119);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 12);
            this.label11.TabIndex = 8;
            this.label11.Text = "Rec Stream Size";
            // 
            // tbstreamport
            // 
            this.tbstreamport.Location = new System.Drawing.Point(87, 88);
            this.tbstreamport.Name = "tbstreamport";
            this.tbstreamport.Size = new System.Drawing.Size(55, 22);
            this.tbstreamport.TabIndex = 7;
            // 
            // tbvideodir
            // 
            this.tbvideodir.Location = new System.Drawing.Point(87, 60);
            this.tbvideodir.Name = "tbvideodir";
            this.tbvideodir.Size = new System.Drawing.Size(174, 22);
            this.tbvideodir.TabIndex = 6;
            // 
            // tbStreamType
            // 
            this.tbStreamType.Location = new System.Drawing.Point(87, 34);
            this.tbStreamType.Name = "tbStreamType";
            this.tbStreamType.Size = new System.Drawing.Size(174, 22);
            this.tbStreamType.TabIndex = 5;
            // 
            // tbPlugin
            // 
            this.tbPlugin.Location = new System.Drawing.Point(87, 8);
            this.tbPlugin.Name = "tbPlugin";
            this.tbPlugin.Size = new System.Drawing.Size(174, 22);
            this.tbPlugin.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbDefaultIp);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.NudMaxClient);
            this.groupBox2.Controls.Add(this.NudTreeSize);
            this.groupBox2.Controls.Add(this.tbConport);
            this.groupBox2.Controls.Add(this.tbDataport);
            this.groupBox2.Controls.Add(this.tbSLPort);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(285, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(151, 181);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "P2P server";
            // 
            // tbDefaultIp
            // 
            this.tbDefaultIp.Location = new System.Drawing.Point(64, 144);
            this.tbDefaultIp.Name = "tbDefaultIp";
            this.tbDefaultIp.Size = new System.Drawing.Size(77, 22);
            this.tbDefaultIp.TabIndex = 16;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 147);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 12);
            this.label10.TabIndex = 15;
            this.label10.Text = "Default IP";
            // 
            // NudMaxClient
            // 
            this.NudMaxClient.Location = new System.Drawing.Point(100, 89);
            this.NudMaxClient.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudMaxClient.Name = "NudMaxClient";
            this.NudMaxClient.Size = new System.Drawing.Size(41, 22);
            this.NudMaxClient.TabIndex = 14;
            this.NudMaxClient.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // NudTreeSize
            // 
            this.NudTreeSize.Location = new System.Drawing.Point(100, 117);
            this.NudTreeSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudTreeSize.Name = "NudTreeSize";
            this.NudTreeSize.Size = new System.Drawing.Size(41, 22);
            this.NudTreeSize.TabIndex = 13;
            this.NudTreeSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // tbConport
            // 
            this.tbConport.Location = new System.Drawing.Point(100, 62);
            this.tbConport.Name = "tbConport";
            this.tbConport.Size = new System.Drawing.Size(41, 22);
            this.tbConport.TabIndex = 11;
            // 
            // tbDataport
            // 
            this.tbDataport.Location = new System.Drawing.Point(100, 34);
            this.tbDataport.Name = "tbDataport";
            this.tbDataport.Size = new System.Drawing.Size(41, 22);
            this.tbDataport.TabIndex = 10;
            // 
            // tbSLPort
            // 
            this.tbSLPort.Location = new System.Drawing.Point(100, 8);
            this.tbSLPort.Name = "tbSLPort";
            this.tbSLPort.Size = new System.Drawing.Size(41, 22);
            this.tbSLPort.TabIndex = 9;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(21, 174);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(47, 19);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(74, 174);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(48, 19);
            this.btnReset.TabIndex = 12;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // PreferenceFm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 205);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PreferenceFm";
            this.Text = "Preference";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudMaxClient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudTreeSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbstreamport;
        private System.Windows.Forms.TextBox tbvideodir;
        private System.Windows.Forms.TextBox tbStreamType;
        private System.Windows.Forms.TextBox tbPlugin;
        private System.Windows.Forms.NumericUpDown NudMaxClient;
        private System.Windows.Forms.NumericUpDown NudTreeSize;
        private System.Windows.Forms.TextBox tbConport;
        private System.Windows.Forms.TextBox tbDataport;
        private System.Windows.Forms.TextBox tbSLPort;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbDefaultIp;
        private System.Windows.Forms.TextBox tbRecStream;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbChunkSize;
    }
}