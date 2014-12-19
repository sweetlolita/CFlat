using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using CFlat.Utility;
using CFlat.Bridge.Tcp.Session;
using CFlat.Bridge.Tcp.Action;

namespace CFlat.Bridge.Tcp.EndPoint
{
    public class TcpClient : PollingThread , IocpSessionObserver
    {
        private IocpSession session { get; set; }
        private IocpConnector connector { get; set; }
        private TcpEndPointObserver tcpEndPointObserver { get; set; }
        private enum ConnectionStatus
        {
            Connecting,
            Connected,
            //Disconnecting,
            Disconnected
        };
        private ConnectionStatus connectionStatus { get; set; }

        public TcpClient(string clientIpAddress, int clientPort,
            string serverIpAddress, int serverPort,
            TcpEndPointObserver tcpEndPointObserver) : base(5000)
        {
            this.tcpEndPointObserver = tcpEndPointObserver;
            session = new IocpSession(this);
            connector = new IocpConnector(
                new IPEndPoint(IPAddress.Parse(clientIpAddress), clientPort),
                new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort),
                this);
            connectionStatus = ConnectionStatus.Disconnected;
        }

        public void send(byte[] buffer, int offset, int count)
        {
            session.send(buffer, offset, count);
        }

        public void onSessionConnected(Socket remoteSocket)
        {
            session.attachSocket(remoteSocket);
            session.recv();
            connectionStatus = ConnectionStatus.Connected;
            if (tcpEndPointObserver != null)
            {
                tcpEndPointObserver.onTcpConnected(Guid.Empty);
            }
        }

        public void onSessionData(Guid sessionId, Descriptor descriptor)
        {
            if(tcpEndPointObserver != null)
            {
                tcpEndPointObserver.onTcpData(sessionId, descriptor);
            }

        }

        public void onSessionDisconnected(Guid sessionId)
        {
            Logger.debug("TcpClient: disconnected.",
                    sessionId);
            connectionStatus = ConnectionStatus.Disconnected;
            if (tcpEndPointObserver != null)
            {
                tcpEndPointObserver.onTcpDisconnected(Guid.Empty);
            }
        }

        public void onSessionError(Guid sessionId, string errorMessage)
        {
            if (connectionStatus == ConnectionStatus.Connecting)
            {
                connectionStatus = ConnectionStatus.Disconnected;
            }
            if (tcpEndPointObserver != null)
            {
                tcpEndPointObserver.onTcpError(sessionId, errorMessage);
            }
        }

        protected override void onPolling()
        {
            switch (connectionStatus)
            {
                case ConnectionStatus.Connected:
                    break;
                case ConnectionStatus.Connecting:
                    break;
                case ConnectionStatus.Disconnected:
                    {
                        Logger.debug("TcpClient: start a new connect action...");
                        connector.start();
                        connectionStatus = ConnectionStatus.Connecting;
                        break;
                    }
            }
        }

        protected override void onStart()
        {
            Logger.debug("TcpClient: start.");
        }

        protected override void onStop()
        {
            if(connectionStatus == ConnectionStatus.Connected ||
                connectionStatus == ConnectionStatus.Connecting)
            {
                session.dispose(this, EventArgs.Empty);
                connector.stop();
            }
            Logger.debug("TcpClient: stop.");
        }
    }
}
