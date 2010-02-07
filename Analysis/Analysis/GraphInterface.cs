using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ClassLibrary;
using ZedGraph;
using System.Drawing;
using System.Windows.Forms;

namespace Analysis
{
    public interface graphinterface
    {
       int speedCalculate(DateTime startTime, DateTime end, int size);
       
       void AddRecord(DateTime startTime, DateTime end, int size);
       // void importData(string ip);
       //GraphPane CreateGraph(GraphPane newpanel);
       void CreateGraph(ZedGraphControl newpanel);
       void UpdateGraph(ZedGraphControl newpanel);
       //void cleanGraphScreen();
       void RefreshGraph();
       int MaxSpeed();
     }

    public class plotgraph : graphinterface
    {
        private string HostName;
        private int id;
        PointPairList list;
        int CurrentIndex;
        LineItem myCurve;

        public plotgraph(string host, Boolean refresh)
        {
            //PingIP Target;
            HostName = host;
            list = new PointPairList();
            //myCurve = new LineItem("Speed");
            CurrentIndex = 0;
            if (refresh)
            {
                xml Target = new xml(host, "DataBase", refresh);
                id = 0;
                Target.AddAttribute("DataBase", "Maxid", id.ToString());
            }
            else
            {
                xml Target = new xml(host, "DataBase", refresh);
                id = Int32.Parse(Target.ReadAttribute("DataBase", "Maxid"));
            }
        }

        public string GetHostName()
        {
            return HostName;
        }

        public void AddRecord(DateTime startTime, DateTime end, int size)
        {
            xml Target = new xml(HostName, "DataBase", false);
            string record_time = end.ToString();
            int speed = speedCalculate(startTime, end,size);

            string[] type = { "RecordTime", "Speed"};
            string[] value = { record_time, speed.ToString()};
            string[] attriN = { "id", "Host"};
            //string[] attriV = {(System.DateTime.Now.Hour * 3600 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second).ToString()};
            string[] attriV = { id.ToString(),HostName};

            Target.Add("Record", type, value, attriN, attriV);
            id++;
            Target.modifyAttribute("DataBase", "Maxid", id.ToString());
        }


        public int speedCalculate(DateTime starttime ,DateTime end, int size)
        {
            try
            {
                int time = (end.Hour - starttime.Hour) * 3600 + (end.Minute - starttime.Minute) * 60 + (end.Second - starttime.Second);
                return size / time;
            }
            catch
            { return 0; }
        }

        public void RefreshGraph()
        { }

        public int MaxSpeed()
        {
            return 0;
        }

        public void CreateGraph(ZedGraphControl display)
        {
            //for (int i = 0; i < 100; i++)
            //{
            //    AddRecord(DateTime.Now.AddHours(-20), DateTime.Now.AddMinutes(i + 1).AddHours(-20), 512000);

            //}

            //CurrentIndex = 100;

            display.GraphPane.Title.Text = "Speed Versus Time";
            display.GraphPane.XAxis.Title.Text = "Time";
            display.GraphPane.YAxis.Title.Text = "Speed";
            display.GraphPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;

            myCurve = display.GraphPane.AddCurve("Speed Curve", list, Color.DarkGreen, SymbolType.None);
            //PingIP measure = new PingIP("yahoo.com");
            //xml ImportData = new xml("yahoo.com", "DataBase", false);
            //for (int i = 0; i < 100; i++)
            //{

            //    double y = Convert.ToInt32(ImportData.Read("Record", "id", i.ToString(), "Speed"));

            //    double x = (double)new XDate(Convert.ToDateTime(ImportData.Read("Record", "id", i.ToString(), "RecordTime")));
            //    list.Add(x, y);
            //}
            //LineItem myCurve = display.GraphPane.AddCurve("My Curve", list, Color.DarkGreen, SymbolType.None);
            
        }

        public void UpdateGraph(ZedGraphControl display)
        {
            //AddRecord(DateTime.Now.AddHours(-20), DateTime.Now.AddMinutes(CurrentIndex).AddHours(-20), 512000);

            display.GraphPane.XAxis.Scale.MaxAuto = true;

            xml ImportData = new xml("yahoo.com", "DataBase", false);
            double y = Convert.ToInt32(ImportData.Read("Record", "id", CurrentIndex.ToString(), "Speed"));
            double x = (double)new XDate(Convert.ToDateTime(ImportData.Read("Record", "id", CurrentIndex.ToString(), "RecordTime")));
            list.Add(x, y);
            //myCurve.AddPoint(new PointPair(x, y));
            //remove the first data
            if (list.Count >= 50)
                list.RemoveAt(0);
            //LineItem myCurve = display.GraphPane.AddCurve("My Curve", list, Color.DarkGreen, SymbolType.None);
            myCurve.AddPoint(new PointPair(x, y));
            CurrentIndex++;



            //this.display.AxisChange();
            //this.display.Refresh();
        }

        

    }
}
