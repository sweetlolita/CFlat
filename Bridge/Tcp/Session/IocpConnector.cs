using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using CFlat.Bridge.Tcp.Action;

namespace CFlat.Bridge.Tcp.Session
{
    class IocpConnector
    {
        private IocpConnectAction connectAction { get; set; }

        public IocpConnector(IPEndPoint clientEndPoint, IPEndPoint serverEndPoint, IocpSessionObserver observer)
        {
            connectAction = new IocpConnectAction(clientEndPoint, serverEndPoint,
                ((socket) => { observer.onSessionConnected(socket); }),
                ((errorMessage) => { observer.onSessionError(Guid.Empty, errorMessage); }));
        }

        public void start()
        {
            connectAction.connect();
        }

        public void stop()
        {
            connectAction.shutdown();
        }
    }
}
