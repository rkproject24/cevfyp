using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ServerFrm serverMaininf = new ServerFrm();
            
            //ServerHandler sevhandle = new ServerHandler();
            //sevhandle.setView(serverMaininf);
            //serverMaininf.setHandler(sevhandle);
            
            Application.Run(serverMaininf);
        }
    }
}
