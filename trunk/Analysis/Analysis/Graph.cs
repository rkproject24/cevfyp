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
    public partial class Graph : Form
    {
        Random ran = new Random();
        PointPairList list = new PointPairList();
        LineItem myCurve;
        int CurrentIndex;


        public Graph()
        {
            InitializeComponent();
        }


        private void Graph_Load(object sender, EventArgs e)
        {
            display.Location = new Point(10, 10);
            display.Size = new Size(ClientRectangle.Width - 50, ClientRectangle.Height - 400);

            this.display.GraphPane.Title.Text = "Speed Versus Time";
            this.display.GraphPane.XAxis.Title.Text = "Time";
            this.display.GraphPane.YAxis.Title.Text = "Speed";
            this.display.GraphPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;



            //PingIP measure = new PingIP("yahoo.com");
            xml ImportData = new xml("yahoo.com", "DataBate", false);

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

                double y = Convert.ToInt32(ImportData.Read("Record", "id", i.ToString(), "Speed"));
                double x = Convert.ToInt32(ImportData.Read("Record", "id", i.ToString(), "RecordTime"));

                // ImportData.ReadAttribute(

                list.Add(x, y);


            }

            CurrentIndex = 100;

            DateTime dt = DateTime.Now;

            myCurve = display.GraphPane.AddCurve("My Curve", list, Color.DarkGreen, SymbolType.None);

            this.display.AxisChange();
            this.display.Refresh();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            display.GraphPane.XAxis.Scale.MaxAuto = true;
            double x = (double)new XDate(DateTime.Now);
            double y = ran.NextDouble();
            //PingIP measure = new PingIP("yahoo.com");
            xml ImportData = new xml("yahoo.com", "Record", false);
            y = Convert.ToInt32(ImportData.Read("Record", "id", CurrentIndex.ToString(), "Speed"));
            x = Convert.ToInt32(ImportData.Read("Record", "id", CurrentIndex.ToString(), "RecordTime"));
            list.Add(x, y);

            CurrentIndex++;

            //remove the first data
            if (list.Count >= 100)
                list.RemoveAt(0);

            this.display.AxisChange();
            this.display.Refresh();

        }



    }
}
