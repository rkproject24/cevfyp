﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ZedGraph_sample
{
    public interface GraphInterface
    {
        void importData(string IP);
        void exportData(string File);
    }

    public class PlotGraph : GraphInterface
    {
        private List<Data> DataList;

        public PlotGraph(string [] IP)
        {
            PingIP Target;
            do
            {


            }
            while (true);

        }

        public void importData(string FileN)
        {
            FileStream read = new FileStream(@File, FileMode.open, FileAccess.Read);
            System.Diagnostics.ProcessStartInfo GenTxt = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + "echo > Reading");
            //GenTxt.RedirectStandardOutput = true;
            GenTxt.UseShellExecute = false;
            GenTxt.CreateNoWindow = true;
            System.Diagnostics.Process test = new System.Diagnostics.Process();
            test.StartInfo = GenTxt;
            test.Start();

            try
            {
                Data import;
                read.Read();


            }
            finally
            {
                read.Close();
            }
        }

        public void SpeedTestExportData(string FileN)
        {
            FileStream read = new FileStream(@File, FileMode.open, FileAccess.Write);
            if (!File.Exists("Reading"))
            {
                try
                {

                }
                finally
                {
                    read.Close();
                }
            }

        }
    }
}
