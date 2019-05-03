using System;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace KWedge
{
    public partial class Config : Form
    {
        GUI gui;
        ListBox ports;
        private static Config instance;
        public bool Running { get; set; }
        private static readonly object padlock = new object();
        public string DefaultPort { get; set; }
        ConfigData data;
        IFormatter formatter;
        static string filename = "data.dat";

        private Config()
        {
            InitializeComponent();
            gui = GUI.Instance;
            ports = gui.Ports;
            lstConfigPorts.DataSource = ports.DataSource;
            data = new ConfigData();
            formatter = new BinaryFormatter();
            DeserializeObject(filename, formatter);
        }
        public static Config Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Config();
                    }
                    return instance;
                }
            }
        }
        
        public void SerializeObject(string filename, IFormatter formatter)
        {
            data.DefaultPort = DefaultPort;
            FileStream stream = new FileStream(filename, FileMode.Create);
            formatter.Serialize(stream, data);
            stream.Close();
            stream.Dispose();
        }
        public void DeserializeObject(string filename, IFormatter formatter)
        {
            try
            {
                FileStream s = File.OpenRead(filename);
                try
                {
                    data = (ConfigData)formatter.Deserialize(s);
                }
                catch
                {

                }
                s.Close();
                s.Dispose();
            }
            catch(System.IO.FileNotFoundException)
            {
                SerializeObject(filename, formatter);
            }
            
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            DefaultPort = lstConfigPorts.Text;
            btnApply.Enabled = false;
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            BtnApply_Click(sender, e);
            SerializeObject(filename, formatter);
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
