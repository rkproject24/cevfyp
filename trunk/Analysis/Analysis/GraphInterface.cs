using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ClassLibrary;
using ZedGraph;
using System.Windows.Forms;

namespace Analysis
{
    public interface graphinterface
    {
       int speedCalculate(DateTime startTime, DateTime end, int size);
    
       void AddRecord(DateTime startTime, DateTime end, int size);
       // void importData(string ip);
       void CreateGraph();
       //void cleanGraphScreen();
       void RefreshGraph();
       int MaxSpeed();
     }

    public class plotgraph : graphinterface
    {
        private string HostName;
        private int id;

        public plotgraph(string host)
        {
            id = 0;
            //PingIP Target;
            HostName = host;
            xml Target = new xml(host,"Record",true);
        }

        public string GetHostName()
        {
            return HostName;
        }

        public void AddRecord(DateTime startTime, DateTime end, int size)
        {
            xml Target = new xml(HostName,"Record",false);
            string record_time = end.ToString();
            int speed = speedCalculate(startTime, end,size);

            string[] type = { "RecordTime", "Speed"};
            string[] value = { record_time, speed.ToString()};
            string[] attriN = { "id", "Host"};
            //string[] attriV = {(System.DateTime.Now.Hour * 3600 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second).ToString()};
            string[] attriV = { id.ToString(),HostName};

            Target.Add("Record", type, value, attriN, attriV);
            id++;
        }


        public int speedCalculate(DateTime starttime ,DateTime end, int size)
        {
            int time = (end.Hour - starttime.Hour)*3600+(end.Minute-starttime.Minute)*60+(end.Second-starttime.Second);
            return size/time; 
        }

        public void RefreshGraph()
        { }

        public int MaxSpeed()
        {
            return 0;
        }

        public void CreateGraph()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Graph());
        }

        

    }
}
