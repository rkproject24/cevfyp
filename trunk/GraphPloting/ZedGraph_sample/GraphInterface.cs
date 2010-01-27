using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ClassLibrary;

namespace Analysis
{
    public interface graphinterface
    {
        void importData(string ip);
        //void exportGraph(string file);
    }

    public class plotgraph : graphinterface
    {
        //private list<data> datalist;

        public plotgraph(string [] ip)
        {
            //PingIP Target;
            do
            {


            }
            while (true);

        }

        public void importData(string IP)
        {
            //FileStream read = new FileStream(@File, FileMode.open, FileAccess.Read);
            
            System.Diagnostics.ProcessStartInfo GenTxt = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + "echo > Reading");
            //GenTxt.RedirectStandardOutput = true;
            GenTxt.UseShellExecute = false;
            GenTxt.CreateNoWindow = true;
            System.Diagnostics.Process test = new System.Diagnostics.Process();
            test.StartInfo = GenTxt;
            test.Start();

            try
            {
                //Data import;
                //xml GetValue = new xml(IP + ".xml", "Time",false);
                //import.IP = IP;
                //import.time = GetValue.Read("Time", "value");
                //import.Speed = GetValue.Read("Time", "speed");
                PingIP measure = new PingIP(IP);

            }
            catch
            {
            //    read.Close();
            }
        }

        
       /* public void exportGraph(string FileN)
        {
            //FileStream read = new FileStream(@File, FileMode.open, FileAccess.Write);
            if (!File.Exists("Reading"))
            {
                xml readFile = new xml(FileN, "Result", false);
                try
                {

                }
                finally
                {
                    read.Close();
                }
            }

        }*/
        
    }
}
