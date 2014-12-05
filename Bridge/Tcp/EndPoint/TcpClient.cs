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
    public class TcpClient : IocpSessionObserver
    {
        private IocpSession session { get; set; }
        private IocpConnector connector { get; set; }
        private TcpEndPointObserver tcpEndPointObserver { get; set; }
        public TcpClient(string clientIpAddress, int clientPort,
            string serverIpAddress, int serverPort,
            TcpEndPointObserver tcpEndPointObserver)
        {
            this.tcpEndPointObserver = tcpEndPointObserver;
            session = new IocpSession(this);
            connector = new IocpConnector(
                new IPEndPoint(IPAddress.Parse(clientIpAddress), clientPort),
                new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort),
                this);
        }

        public void start()
        {
            Logger.debug("TcpClient: starting...");
            connector.start();
            Logger.debug("TcpClient: start.");
        }

        public void stop()
        {
            Logger.debug("TcpClient: stopping...");
            session.dispose(this, EventArgs.Empty);
            connector.stop();
            Logger.debug("TcpClient: stop.");
        }

        public void send(string buffer)
        {
            session.send(System.Text.Encoding.ASCII.GetBytes(buffer), 0, buffer.Length);
        }

        public void send(byte[] buffer, int offset, int count)
        {
            session.send(buffer, offset, count);
        }

        public void onSessionConnected(Socket remoteSocket)
        {
            session.attachSocket(remoteSocket);
            session.recv();
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
            if (tcpEndPointObserver != null)
            {
                tcpEndPointObserver.onTcpDisconnected(Guid.Empty);
            }
        }


        public void onSessionError(Guid sessionId, string errorMessage)
        {
            if (tcpEndPointObserver != null)
            {
                tcpEndPointObserver.onTcpError(sessionId, errorMessage);
            }
        }
    }
}
