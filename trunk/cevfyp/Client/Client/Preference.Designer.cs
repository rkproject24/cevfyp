namespace Client
{
    partial class Preference
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
            this.tbPlugin = new System.Windows.Forms.TextBox();
            this.tbVlcPort = new System.Windows.Forms.TextBox();
            this.tbChunkSize = new System.Windows.Forms.TextBox();
            this.tbChunkLenght = new System.Windows.Forms.TextBox();
            this.tbServerSLPort = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NudChunkBuf = new System.Windows.Forms.NumericUpDown();
            this.NudStartBuf = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbControlPort = new System.Windows.Forms.TextBox();
            this.tbDataPort = new System.Windows.Forms.TextBox();
            this.tbListPort = new System.Windows.Forms.TextBox();
            this.NudPeers = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnVLClib = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudChunkBuf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudStartBuf)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudPeers)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Plugin Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Play PortBase";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Chunk Size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Chunk lenght";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Max. Peers/Tree";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 79);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Server SLPort";
            // 
            // tbPlugin
            // 
            this.tbPlugin.Location = new System.Drawing.Point(85, 10);
            this.tbPlugin.Name = "tbPlugin";
            this.tbPlugin.ReadOnly = true;
            this.tbPlugin.Size = new System.Drawing.Size(73, 20);
            this.tbPlugin.TabIndex = 6;
            // 
            // tbVlcPort
            // 
            this.tbVlcPort.Location = new System.Drawing.Point(85, 41);
            this.tbVlcPort.Name = "tbVlcPort";
            this.tbVlcPort.Size = new System.Drawing.Size(100, 20);
            this.tbVlcPort.TabIndex = 7;
            // 
            // tbChunkSize
            // 
            this.tbChunkSize.Location = new System.Drawing.Point(86, 15);
            this.tbChunkSize.Name = "tbChunkSize";
            this.tbChunkSize.Size = new System.Drawing.Size(100, 20);
            this.tbChunkSize.TabIndex = 8;
            // 
            // tbChunkLenght
            // 
            this.tbChunkLenght.Location = new System.Drawing.Point(86, 46);
            this.tbChunkLenght.Name = "tbChunkLenght";
            this.tbChunkLenght.Size = new System.Drawing.Size(100, 20);
            this.tbChunkLenght.TabIndex = 9;
            // 
            // tbServerSLPort
            // 
            this.tbServerSLPort.Location = new System.Drawing.Point(86, 76);
            this.tbServerSLPort.Name = "tbServerSLPort";
            this.tbServerSLPort.Size = new System.Drawing.Size(100, 20);
            this.tbServerSLPort.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnVLClib);
            this.groupBox1.Controls.Add(this.NudChunkBuf);
            this.groupBox1.Controls.Add(this.NudStartBuf);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.tbVlcPort);
            this.groupBox1.Controls.Add(this.tbPlugin);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 139);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local Play";
            // 
            // NudChunkBuf
            // 
            this.NudChunkBuf.Location = new System.Drawing.Point(85, 105);
            this.NudChunkBuf.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudChunkBuf.Name = "NudChunkBuf";
            this.NudChunkBuf.Size = new System.Drawing.Size(100, 20);
            this.NudChunkBuf.TabIndex = 11;
            this.NudChunkBuf.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NudStartBuf
            // 
            this.NudStartBuf.Location = new System.Drawing.Point(85, 75);
            this.NudStartBuf.Name = "NudStartBuf";
            this.NudStartBuf.Size = new System.Drawing.Size(100, 20);
            this.NudStartBuf.TabIndex = 10;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 108);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(69, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "Chunk Buffer";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 77);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 13);
            this.label12.TabIndex = 8;
            this.label12.Text = "Start Buffer";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbControlPort);
            this.groupBox2.Controls.Add(this.tbDataPort);
            this.groupBox2.Controls.Add(this.tbListPort);
            this.groupBox2.Controls.Add(this.NudPeers);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(214, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(201, 139);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Upload";
            // 
            // tbControlPort
            // 
            this.tbControlPort.Location = new System.Drawing.Point(92, 74);
            this.tbControlPort.Name = "tbControlPort";
            this.tbControlPort.Size = new System.Drawing.Size(100, 20);
            this.tbControlPort.TabIndex = 15;
            // 
            // tbDataPort
            // 
            this.tbDataPort.Location = new System.Drawing.Point(92, 41);
            this.tbDataPort.Name = "tbDataPort";
            this.tbDataPort.Size = new System.Drawing.Size(100, 20);
            this.tbDataPort.TabIndex = 14;
            // 
            // tbListPort
            // 
            this.tbListPort.Location = new System.Drawing.Point(92, 10);
            this.tbListPort.Name = "tbListPort";
            this.tbListPort.Size = new System.Drawing.Size(100, 20);
            this.tbListPort.TabIndex = 13;
            // 
            // NudPeers
            // 
            this.NudPeers.Location = new System.Drawing.Point(92, 106);
            this.NudPeers.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudPeers.Name = "NudPeers";
            this.NudPeers.Size = new System.Drawing.Size(100, 20);
            this.NudPeers.TabIndex = 12;
            this.NudPeers.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(86, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Control PortBase";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Data Port";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "List Port";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbServerSLPort);
            this.groupBox3.Controls.Add(this.tbChunkLenght);
            this.groupBox3.Controls.Add(this.tbChunkSize);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(12, 160);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(195, 107);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Download";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(222, 209);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(306, 209);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 25);
            this.btnReset.TabIndex = 16;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnVLClib
            // 
            this.btnVLClib.Location = new System.Drawing.Point(160, 7);
            this.btnVLClib.Name = "btnVLClib";
            this.btnVLClib.Size = new System.Drawing.Size(25, 23);
            this.btnVLClib.TabIndex = 12;
            this.btnVLClib.Text = "...";
            this.btnVLClib.UseVisualStyleBackColor = true;
            this.btnVLClib.Click += new System.EventHandler(this.btnVLClib_Click);
            // 
            // Preference
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 276);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Preference";
            this.Text = "Preference";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudChunkBuf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudStartBuf)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudPeers)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbPlugin;
        private System.Windows.Forms.TextBox tbVlcPort;
        private System.Windows.Forms.TextBox tbChunkSize;
        private System.Windows.Forms.TextBox tbChunkLenght;
        private System.Windows.Forms.TextBox tbServerSLPort;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown NudPeers;
        private System.Windows.Forms.TextBox tbListPort;
        private System.Windows.Forms.TextBox tbControlPort;
        private System.Windows.Forms.TextBox tbDataPort;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.NumericUpDown NudChunkBuf;
        private System.Windows.Forms.NumericUpDown NudStartBuf;
        private System.Windows.Forms.Button btnVLClib;
    }
}