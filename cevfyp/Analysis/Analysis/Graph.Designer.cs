﻿    namespace Analysis
    {
        partial class Graph
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
                this.display = new ZedGraph.ZedGraphControl();
                this.timer1 = new System.Windows.Forms.Timer(this.components);
                this.SuspendLayout();
                // 
                // display
                // 
                this.display.Location = new System.Drawing.Point(12, 24);
                this.display.Name = "display";
                this.display.ScrollGrace = 0;
                this.display.ScrollMaxX = 0;
                this.display.ScrollMaxY = 0;
                this.display.ScrollMaxY2 = 0;
                this.display.ScrollMinX = 0;
                this.display.ScrollMinY = 0;
                this.display.ScrollMinY2 = 0;
                this.display.Size = new System.Drawing.Size(411, 285);
                this.display.TabIndex = 0;
                // 
                // timer1
                // 
                this.timer1.Enabled = true;
                this.timer1.Interval = 1000;
                this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
                // 
                // Graph
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(435, 321);
                this.Controls.Add(this.display);
                this.Name = "Graph";
                this.Text = "Graph";
                this.Load += new System.EventHandler(this.Graph_Load);
                this.ResumeLayout(false);

            }

            #endregion

            private ZedGraph.ZedGraphControl display;
            private System.Windows.Forms.Timer timer1;

            }
    }