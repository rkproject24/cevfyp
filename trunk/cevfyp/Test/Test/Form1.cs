using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Analysis;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            plotgraph test = new plotgraph("yahoo.com");
            for (int i = 0; i < 50; i++)
            {
                test.AddRecord(DateTime.Now, DateTime.Now.AddMinutes(5), 512000);
                test.AddRecord(DateTime.Now.AddMinutes(11), DateTime.Now.AddMinutes(18), 512000);
            }
            test.CreateGraph();
        }
    }
}
