using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KWedge
{
    [Serializable]
    class ConfigData : ISerializable
    {
        public string DefaultPort { get; set; }

        public ConfigData()
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Default Port", DefaultPort, typeof(string));
        }
        public ConfigData(SerializationInfo info, StreamingContext context)
        {
            DefaultPort = info.GetString("Default Port");
        }
    }
}
