namespace TrackerServer
{
    partial class TrackerMainFrm
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
            this.rtbClientlist = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbbTree = new System.Windows.Forms.ComboBox();
            this.btnOn = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.preferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReset = new System.Windows.Forms.Button();
            this.TreelistView = new System.Windows.Forms.ListView();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbClientlist
            // 
            this.rtbClientlist.Location = new System.Drawing.Point(12, 58);
            this.rtbClientlist.Name = "rtbClientlist";
            this.rtbClientlist.Size = new System.Drawing.Size(256, 166);
            this.rtbClientlist.TabIndex = 0;
            this.rtbClientlist.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Online Client";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tree";
            // 
            // cbbTree
            // 
            this.cbbTree.FormattingEnabled = true;
            this.cbbTree.Location = new System.Drawing.Point(288, 35);
            this.cbbTree.Name = "cbbTree";
            this.cbbTree.Size = new System.Drawing.Size(77, 20);
            this.cbbTree.TabIndex = 3;
            this.cbbTree.SelectedIndexChanged += new System.EventHandler(this.cbbTree_SelectedIndexChanged);
            // 
            // btnOn
            // 
            this.btnOn.Location = new System.Drawing.Point(12, 230);
            this.btnOn.Name = "btnOn";
            this.btnOn.Size = new System.Drawing.Size(75, 21);
            this.btnOn.TabIndex = 4;
            this.btnOn.Text = "On";
            this.btnOn.UseVisualStyleBackColor = true;
            this.btnOn.Click += new System.EventHandler(this.btnOn_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferenceToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(120, 26);
            // 
            // preferenceToolStripMenuItem
            // 
            this.preferenceToolStripMenuItem.Name = "preferenceToolStripMenuItem";
            this.preferenceToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.preferenceToolStripMenuItem.Text = "Preference";
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(94, 230);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // TreelistView
            // 
            this.TreelistView.Location = new System.Drawing.Point(274, 58);
            this.TreelistView.Name = "TreelistView";
            this.TreelistView.Size = new System.Drawing.Size(239, 166);
            this.TreelistView.TabIndex = 6;
            this.TreelistView.UseCompatibleStateImageBehavior = false;
            // 
            // TrackerMainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 269);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnOn);
            this.Controls.Add(this.TreelistView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbbTree);
            this.Controls.Add(this.rtbClientlist);
            this.Name = "TrackerMainFrm";
            this.Text = "Tracker";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbClientlist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbbTree;
        private System.Windows.Forms.Button btnOn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem preferenceToolStripMenuItem;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ListView TreelistView;
    }
}

