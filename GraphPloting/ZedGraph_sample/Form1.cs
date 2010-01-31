using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using ClassLibrary;

namespace Analysis
{
    public partial class Form1 : Form
    {
        Random ran = new Random();
        PointPairList list = new PointPairList();
        LineItem myCurve;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        private void SetSize()
        {
            zedGraphControl1.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            zedGraphControl1.Size = new Size(ClientRectangle.Width - 50,
                                    ClientRectangle.Height - 400);

            zedGraphControl2.Location = new Point(10, 250);
            zedGraphControl2.Size = new Size(ClientRectangle.Width - 50,
                                    ClientRectangle.Height - 400);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Setup the graph
            CreateGraph(zedGraphControl1);
            // Size the control to fill the form with a margin
            SetSize();

        }

        private void CreateGraph(ZedGraphControl zgc)
        {
            //***************Panel One**************

            // get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;

            // Set the Titles
            myPane.Title.Text = "Speed Versus Time";
            myPane.XAxis.Title.Text = "Time";
            myPane.YAxis.Title.Text = "Speed";

            // Make up some data arrays based on the Sine function
            double x0, y1, y2;
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            for (int i = 0; i < 36; i++)
            {
                x0 = (double)i + 5;
                y1 = 1.5 + Math.Sin((double)i * 0.2);
                y2 = 3.0 * (1.5 + Math.Sin((double)i * 0.2));
                list1.Add(x0, y1);
                list2.Add(x0, y2);
            }

            // Generate a red curve with diamond
            // symbols, and "Porsche" in the legend
            LineItem myCurve1 = myPane.AddCurve("Porsche",
                  list1, Color.Red, SymbolType.Diamond);

            // Generate a blue curve with circle
            // symbols, and "Piper" in the legend
            LineItem myCurve2 = myPane.AddCurve("Piper",
                  list2, Color.Blue, SymbolType.Circle);

            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zgc.AxisChange();
            
            /***************Panel two***************/
            this.zedGraphControl2.GraphPane.Title.Text = "Speed Versus Time";
            this.zedGraphControl2.GraphPane.XAxis.Title.Text = "Time";
            this.zedGraphControl2.GraphPane.YAxis.Title.Text = "Speed";
            this.zedGraphControl2.GraphPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;




            PingIP measure = new PingIP("218.250.54.17");
            xml ImportData = new xml("218.250.54.17", "Result", false);
            for (int i = 0; i <= 100; i++)
            {
                //Style one
                //double x = (double)new XDate(DateTime.Now.AddSeconds(-(100 - i)));
                //double y = ran.NextDouble();
                //list.Add(x, y);

                //Style two
                //double x = (double)new XDate(DateTime.Now.AddSeconds(-(100 - i)));
                //double y = 0;
                //list.Add(x, y);

                double x = Convert.ToInt32(ImportData.Read("Result", "Time"));
                double y = Convert.ToInt32(ImportData.Read("Result", "RecordSpeed"));

               // ImportData.ReadAttribute(

                list.Add(x, y);

            }

            DateTime dt = DateTime.Now;

            myCurve = zedGraphControl2.GraphPane.AddCurve("My Curve", list, Color.DarkGreen, SymbolType.None);

            this.zedGraphControl2.AxisChange();
            this.zedGraphControl2.Refresh();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            zedGraphControl2.GraphPane.XAxis.Scale.MaxAuto = true;
            double x = (double)new XDate(DateTime.Now);
            double y = ran.NextDouble();
            list.Add(x, y);

            //remove the first data
            if (list.Count >= 100)
                list.RemoveAt(0);
            
            this.zedGraphControl2.AxisChange();
            this.zedGraphControl2.Refresh();

        }
    }
}
