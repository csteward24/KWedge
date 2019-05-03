using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KWedge
{
    public sealed partial class Log : Form
    {
        private static Log instance = null;
        private static readonly object padlock = new object();

        public delegate void WriteEvent(String data);
        public WriteEvent Write;
        private Log()
        {
            InitializeComponent();
            Write = new WriteEvent(addLogItem);
        }
        public static Log Instance
        {
            get
            {
                lock(padlock)
                {
                    if (instance == null)
                    {
                        instance = new Log();
                    }
                    return instance;
                }
            }
        }

        private void addLogItem(String data)
        {
            String[] item = { data, DateTime.Now.ToString() };
            listView1.Items.Add(new ListViewItem(item));
        }

        private void Log_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
