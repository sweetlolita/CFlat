using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Bridge.Cfp.Gutter;
using CFlat.Bridge.Tcp.EndPoint;
using CFlat.Bridge.Cfp.PacketPresentation;
using CFlat.Utility;

namespace CFlat.Bridge.Cfp.EndPoint
{
    public class CfpClient : CfpGutter, CfpClientInterface
    {
        private TcpClient tcpClient { get; set; }
        public CfpClient(string clientIpAddress, int clientPort,
            string serverIpAddress, int serverPort, CfpGutterObserver cfpGutterObserver) : base(cfpGutterObserver)
        {
            tcpClient = new TcpClient(clientIpAddress, clientPort, serverIpAddress, serverPort, this);
          
        }

        public void start()
        {
            Logger.debug("CfpClient: starting...");
            tcpClient.start();
            Logger.debug("CfpClient: start.");
        }

        public void stop()
        {
            Logger.debug("CfpClient: stopping...");
            tcpClient.stop();
            Logger.debug("CfpClient: stop.");
        }

        public void send(PlaygroundBase playgroundBase)
        {
            PacketDataUnitBase packetDataUnit = new PacketDataUnitBase();
            packetDataUnit.jsonPayload = playgroundBase.toJson();
            byte[] rawBytes = packetDataUnit.rawBytes();
            tcpClient.send(rawBytes, 0, rawBytes.Length);
        }
    }
}
