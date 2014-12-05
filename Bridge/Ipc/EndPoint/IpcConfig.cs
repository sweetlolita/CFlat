using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Bridge.Ipc.EndPoint
{
    class IpcConfig
    {
        public static string channelDescriptor = "IpcBridge";
        public static string objectDescriptor = "RemoteObject";
        public static string fullDescriptor = "ipc://" + channelDescriptor + "/" + objectDescriptor;

    }
}
