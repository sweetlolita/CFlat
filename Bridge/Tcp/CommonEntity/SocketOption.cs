using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CFlat.Bridge.Tcp.CommonEntity
{
    public class SocketOption
    {
        public static void enableKeepAlive(Socket socket)
        {
            long optionItemSize = sizeof(uint);
            byte[] inOptionValues = new byte[optionItemSize * 3];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)10000).CopyTo(inOptionValues, optionItemSize);
            BitConverter.GetBytes((uint)1000).CopyTo(inOptionValues, optionItemSize * 2);
            int a = socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }
    }
}
