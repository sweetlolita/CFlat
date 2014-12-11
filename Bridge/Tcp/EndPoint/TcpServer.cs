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
    public class TcpServer : IocpSessionObserver
    {
        private IocpSessionPool pendingSessions { get; set; }
        private IocpSessionMap runningSessions { get; set; }
        private IocpListener listener { get; set; }
        private TcpEndPointObserver tcpEndPointObserver { get; set; }

        private const int initialSessionPoolSize = 64;

        public TcpServer(string ipAddress, int port, TcpEndPointObserver tcpEndPointObserver)
        {
            this.tcpEndPointObserver = tcpEndPointObserver;
            listener = new IocpListener(new IPEndPoint(IPAddress.Parse(ipAddress), port), this);
            pendingSessions = new IocpSessionPool(() => new IocpSession(this));
            runningSessions = new IocpSessionMap();
        }

        public void start()
        {
            Logger.debug("TcpServer: starting...");
            listener.start();
            Logger.debug("TcpServer: start.");
        }

        public void stop()
        {
            Logger.debug("TcpServer: stopping...");
            listener.stop();
            Logger.debug("TcpServer: stop.");
        }

        public void onSessionConnected(Socket remoteSocket)
        {
            Logger.debug("TcpServer: client: {0}:{1} connected.",
                    (remoteSocket.RemoteEndPoint as IPEndPoint).Address.ToString(),
                    (remoteSocket.RemoteEndPoint as IPEndPoint).Port);
 	        IocpSession newSession = pendingSessions.take();
            Logger.debug("TcpServer: session {0} assigned to client: {1}:{2}",
                    newSession.sessionId,
                    (remoteSocket.RemoteEndPoint as IPEndPoint).Address.ToString(),
                    (remoteSocket.RemoteEndPoint as IPEndPoint).Port);
            runningSessions.put(newSession.sessionId, newSession);
            Logger.debug("TcpServer: attaching session {0} assigned to client: {1}:{2}",
                    newSession.sessionId,
                    (remoteSocket.RemoteEndPoint as IPEndPoint).Address.ToString(),
                    (remoteSocket.RemoteEndPoint as IPEndPoint).Port);
            newSession.attachSocket(remoteSocket);
            if (tcpEndPointObserver != null)
            {
                tcpEndPointObserver.onTcpConnected(newSession.sessionId);
            }
            newSession.recv();
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
            IocpSession endSession = runningSessions.take(sessionId);
            pendingSessions.put(endSession);
            if (tcpEndPointObserver != null)
            {
                tcpEndPointObserver.onTcpDisconnected(sessionId);
            }
        }

        public void onSessionError(Guid sessionId, string errorMessage)
        {
            if (tcpEndPointObserver != null)
            {
                tcpEndPointObserver.onTcpError(sessionId, errorMessage);
            }
        }

        public void send(Guid sessionId, byte[] data, int offset, int count)
        {
            IocpSession dstSession = runningSessions.search(sessionId);
            dstSession.send(data, offset, count);
        }



    }

}
