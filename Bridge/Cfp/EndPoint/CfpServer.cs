using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Bridge.Tcp.EndPoint;
using CFlat.Bridge.Cfp.Gutter;
using CFlat.Bridge.Cfp.PacketPresentation;
using CFlat.Utility;

namespace CFlat.Bridge.Cfp.EndPoint
{
    public class CfpServer : CfpGutter, CfpServerInterface
    {
        private TcpServer tcpServer { get; set; }

        public CfpServer(string ipAddress, int port, CfpGutterObserver cfpGutterObserver)
            : base(cfpGutterObserver)
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

        public void send(Guid sessionId, PlaygroundBase playgroundBase)
        {
            PacketDataUnitBase packetDataUnit = new PacketDataUnitBase();
            packetDataUnit.jsonPayload = playgroundBase.toJson();
            byte[] rawBytes = packetDataUnit.rawBytes();
            tcpServer.send(sessionId, rawBytes, 0, rawBytes.Length);
        }
    }
}
