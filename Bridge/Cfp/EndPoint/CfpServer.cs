using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Bridge.Tcp.EndPoint;
using CFlat.Bridge.Cfp.Gutter;
using CFlat.Utility;

namespace CFlat.Bridge.Cfp.EndPoint
{
    public class CfpServer : CfpGutter
    {
        private TcpServer tcpServer { get; set; }

        public CfpServer(string ipAddress, int port)
        {
            tcpServer = new TcpServer(ipAddress, port, this);
        }

        public void start()
        {
            Logger.debug("CfpServer: starting...");
            tcpServer.start();
            Logger.debug("CfpServer: start.");
        }

        public void stop()
        {
            Logger.debug("CfpServer: stopping...");
            tcpServer.stop();
            Logger.debug("CfpServer: stop.");
        }
    }
}
