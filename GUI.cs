using System;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;


namespace KWedge
{
    public partial class GUI : Form
    {
        public delegate void DataStateEvent(string data);
        public DataStateEvent datastate;
        private SerialHandler serial;
        private Thread serialThread;
        private bool serialRunning = false;

        private static GUI instance;
        private static readonly object padlock = new object();

        public ListBox Ports
        {
            get { return lstPorts; }
        }

        private GUI()
        {
            InitializeComponent();
            EnumeratePorts();
            datastate = new DataStateEvent(GetData);
            //Register handler for lock/unlock event
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(HandleSessionSwitch);
        }
        //must manually unsubscribe from session switch event on finalization
        ~GUI()
        {
            SystemEvents.SessionSwitch -= HandleSessionSwitch;
        }
        public static GUI Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GUI();
                    }
                    return instance;
                }
            }
        }
        //clean up serial thread when X is clicked
        private void Form1_FormClosed(Object sender, FormClosedEventArgs e)
        {
            EndSerialThread();
            Log.Instance.Close();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            Debug.WriteLine("Resize detected");
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
            }
        }
        //restore window when notify icon is double clicked
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
        //invoked when new data is recieved in serial thread
        private void GetData(String str)
        {
            try
            {
                if (this != Form.ActiveForm)
                {
                    SendKeys.Send(str);
                }
                else
                {
                    Debug.WriteLine("Form has focus, supressing keystrokes");
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Debug.WriteLine("Could not send keystrokes");
            }
        }
        //populate port list on GUI
        private void EnumeratePorts()
        {
            lstPorts.DataSource = SerialPort.GetPortNames();
        }
        //Toggles serial thread on/off
        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!serialRunning)
            {
                StartSerialThread();
            }
            else
            {
                EndSerialThread();
            }
        }
        //starts serial thread and puts button in disconnect mode
        private void StartSerialThread()
        {
            serial = new SerialHandler(lstPorts.Text);
            serialThread = new Thread(serial.Serial);
            serialThread.Start();
            serialRunning = true;
            btnConnect.BackColor = Color.Crimson;
            btnConnect.Text = "Disconnect";
            lstPorts.Enabled = false;
        }
        //closes serial thread and puts button in connect mode
        private void EndSerialThread()
        {
            serial.ClosePort();
            serialThread.Join();
            serialRunning = false;
            btnConnect.BackColor = Color.Gainsboro;
            btnConnect.Text = "Connect";
            lstPorts.Enabled = true;
        }
        //starts/ends serial thread on lock/unlock
        void HandleSessionSwitch(object sender, SessionSwitchEventArgs a)
        {
            if (a.Reason == SessionSwitchReason.SessionLock){
                Debug.WriteLine("Lock detected, closing port");
                EndSerialThread();
            }
            if (a.Reason == SessionSwitchReason.SessionUnlock)
            {
                Debug.WriteLine("Unlock detected, starting serial thread");
                if (!serialRunning)
                {
                    StartSerialThread();
                }

            }
        }
        private void LogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.Instance.Show();
        }

        private void AddLogEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.Instance.Invoke(Log.Instance.Write, "Foobar");
        }

        private void ConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.Instance.ShowDialog();
        }
    }
}
