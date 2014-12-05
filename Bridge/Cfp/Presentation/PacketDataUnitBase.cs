using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Bridge.Cfp.Presentation
{
    class PacketDataUnitBase
    {
        public string verb { get; set; }
        public string jsonPayload { get; set; }

        public string rawMessage()
        {
            return verb + "\r\n" + jsonPayload + "\r\n" + " \r\n";
        }

        public byte[] rawBytes()
        {
            return System.Text.Encoding.Default.GetBytes(rawMessage());
        }
    }
}
