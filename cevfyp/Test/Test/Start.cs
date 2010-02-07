using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Analysis;
using ZedGraph;
using ClassLibrary;

namespace Test
{
    public partial class Start : Form
    {
        plotgraph test;
        //PointPairList list = new PointPairList();
        LineItem myCurve;
        int CurrentIndex=0;

        public Start()
        {
            InitializeComponent();
            test = new plotgraph("yahoo.com", true);
            test.CreateGraph(display);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //test = new plotgraph("yahoo.com", true);
            //test.CreateGraph(display);
            ////CurrentIndex = 100;

            //this.display.AxisChange();
            //this.display.Refresh();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            test.AddRecord(DateTime.Now, DateTime.Now.AddMinutes(CurrentIndex), 512000);
            test.UpdateGraph(display);

            this.display.AxisChange();
            this.display.Refresh();

            CurrentIndex++;
        }



    
    }
}
