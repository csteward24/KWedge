using System;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace KWedge
{

    class SerialHandler : IDisposable
    {
        private GUI form = GUI.Instance;
        private string data = "";
        private readonly string pattern = @"91\d{7}";
        public SerialPort SerialPort { get; set; }
        public bool Running { get; set; } = true;
        Log log = Log.Instance;

        public SerialHandler(String port)
        {
            SerialPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One)
            {
                Handshake = Handshake.None,
                WriteTimeout = 500
            };
            SerialPort.Open();
        }
        //main loop
        public void Serial()
        {
            Debug.WriteLine("serial thread started");

            while (Running)
            {
                Thread.Sleep(100);
                try
                {
                    data = SerialPort.ReadExisting();
                    Match match = Regex.Match(data, pattern);
                    if (match.Success)
                    {
                        data = match.Value + "\n";
                        Debug.WriteLine(data);
                        form.Invoke(form.datastate, data);
                        WriteLog(data);
                    }
                }
                catch
                {
                    Debug.WriteLine("Could not read serial data");
                    break;
                }
            }
        }
        public void Dispose() => SerialPort.Close();

        public void ClosePort()
        {
            SerialPort.Close();
            Running = false;
        }
        private void WriteLog(String data) => log.Write.Invoke(data); 
    }
}
