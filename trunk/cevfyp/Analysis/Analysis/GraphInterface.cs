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
       //int speedCalculate(DateTime startTime, DateTime end, int size);
       
       void AddRecord(DateTime startTime, DateTime end, int size);
       // void importData(string ip);
       //GraphPane CreateGraph(GraphPane newpanel);
       void CreateGraph(ZedGraphControl newpanel, int totalTree, string curveName);
       void UpdateGraph(ZedGraphControl newpanel, int tree, string target);
       //void cleanGraphScreen();
       void RefreshGraph();
       int MaxSpeed();
     }

    public class plotgraph : graphinterface
    {
        private Color[] curveColor = { Color.Red, Color.Blue, Color.SpringGreen, Color.Yellow, Color.Silver,Color.Purple };

        private string HostName;
        private int id;
        PointPairList[] list;
        //int CurrentIndex;
        LineItem[] myCurve;
        int[] previousPrint;

        public plotgraph(string host, Boolean refresh)
        {
            //PingIP Target;
            HostName = host;
            //list = new PointPairList();
            //myCurve = new LineItem("Speed");
            //CurrentIndex = 0;
            //previousPrint = -1;
            if (refresh)
            {
                xml Target = new xml(host, "DataBase", refresh);
                id = 0;
                Target.AddAttribute("DataBase", "Maxid", id.ToString());
            }
            else
            {
                xml Target = new xml(HostName, "DataBase", refresh);
                id = Int32.Parse(Target.ReadAttribute("DataBase", "Maxid"));
            }
        }

        public plotgraph(string host)
        {
            //PingIP Target;
            HostName = host;
            //list = new PointPairList();
            //myCurve = new LineItem("Speed");
            //CurrentIndex = 0;
            //previousPrint = -1;
        }

        public string GetHostName()
        {
            return HostName;
        }

        public void AddRecord(DateTime startTime, DateTime end, int size)
        {
            xml Target = new xml(HostName, "DataBase", false);
            string record_time = end.ToString();
            double speed = internalSpeedCalculate(startTime, end,size);

            string[] type = { "RecordTime", "Speed"};
            string[] value = { record_time, speed.ToString()};
            string[] attriN = { "id", "Host"};
            //string[] attriV = {(System.DateTime.Now.Hour * 3600 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second).ToString()};
            string[] attriV = { id.ToString(),HostName};

            Target.Add("Record", type, value, attriN, attriV);
            Target.modifyAttribute("DataBase", "Maxid", id.ToString());
            id++;
            //Target.modifyAttribute("DataBase", "Maxid", id.ToString());
        }

        public void AddRecord(DateTime RecordTime, int speed)
        {
            xml Target = new xml(HostName, "DataBase", false);
            string[] type = { "RecordTime", "Speed" };
            string[] value = { RecordTime.ToString(), speed.ToString() };
            string[] attriN = { "id", "Host" };
            //string[] attriV = {(System.DateTime.Now.Hour * 3600 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second).ToString()};
            string[] attriV = { id.ToString(), HostName };

            Target.Add("Record", type, value, attriN, attriV);
            Target.modifyAttribute("DataBase", "Maxid", id.ToString());
            id++;
        }


        public static int speedCalculate(DateTime starttime ,DateTime end, int sizeInBit)
        {
            try
            {
                int time = (end.Hour - starttime.Hour) * 3600 + (end.Minute - starttime.Minute) * 60 + (end.Second - starttime.Second);
                int result = sizeInBit / time;
                Console.WriteLine(result);
                //double time = (end.Hour - starttime.Hour) * 3600 + (end.Minute - starttime.Minute) * 60 + (end.Second - starttime.Second) + (end.Millisecond - starttime.Millisecond)*0.001;
                return result;
            }
            catch
            { return 0; }
        }

        public int internalSpeedCalculate(DateTime starttime, DateTime end, int sizeInBit)
        {
            try
            {
                int time = (end.Hour - starttime.Hour) * 3600 + (end.Minute - starttime.Minute) * 60 + (end.Second - starttime.Second);
                int result = sizeInBit / time;
                Console.WriteLine(result);
                //double time = (end.Hour - starttime.Hour) * 3600 + (end.Minute - starttime.Minute) * 60 + (end.Second - starttime.Second) + (end.Millisecond - starttime.Millisecond)*0.001;
                return result;
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

        public void CreateGraph(ZedGraphControl display, int totalTree, string curveName)
        {
            //for (int i = 0; i < 100; i++)
            //{
            //    AddRecord(DateTime.Now.AddHours(-20), DateTime.Now.AddMinutes(i + 1).AddHours(-20), 512000);

            //}

            //CurrentIndex = 100;

            display.GraphPane.Title.Text = "Speed Versus Time";
            display.GraphPane.XAxis.Title.Text = "Time(sec)";
            display.GraphPane.YAxis.Title.Text = "Speed(bit/s)";
            display.GraphPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;

            list = new PointPairList[totalTree];
            myCurve = new LineItem[totalTree];
            previousPrint = new int[totalTree];
            for (int i = 0; i < totalTree; i++)
            {
                previousPrint[i] = -1;
                list[i] = new PointPairList();
                myCurve[i] = display.GraphPane.AddCurve(curveName + i, list[i], curveColor[(i % 6)], SymbolType.None);
            }
            
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


        public void clearGraph(ZedGraphControl display)
        {
            display.GraphPane.CurveList.Clear();
            //display.MasterPane = new MasterPane();
            
        }

        public void UpdateGraph(ZedGraphControl display, int tree, string target)
        {
            //AddRecord(DateTime.Now.AddHours(-20), DateTime.Now.AddMinutes(CurrentIndex).AddHours(-20), 512000);

            display.GraphPane.XAxis.Scale.MaxAuto = true;
            
            xml ImportData = new xml(target + tree, "DataBase", false);
            ImportData.load();
            int index = Int32.Parse(ImportData.ReadAttribute("DataBase", "Maxid"));
            if (index > previousPrint[tree])
            {
                try
                {
                    previousPrint[tree]++;// = index;
                    int y = Convert.ToInt32(ImportData.Read("Record", "id", previousPrint[tree].ToString(), "Speed"));
                    double x = (double)new XDate(Convert.ToDateTime(ImportData.Read("Record", "id", previousPrint[tree].ToString(), "RecordTime")));
                    list[tree].Add(x, y);
                    //myCurve.AddPoint(new PointPair(x, y));
                    //remove the first data
                    if (list[tree].Count >= 50)
                    {
                        list[tree].RemoveAt(0);
                        ImportData.deleteInnerNode("Record", "id", (previousPrint[tree]-50).ToString());
                    }
                    //LineItem myCurve = display.GraphPane.AddCurve("My Curve", list, Color.DarkGreen, SymbolType.None);
                    //myCurve[tree].AddPoint(new PointPair(x, y));
                    //CurrentIndex++;
                }
                catch(Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                    //Console.WriteLine(ex);
                }
            }
            
            //this.display.AxisChange();
            //this.display.Refresh();
        }

        

    }
}
