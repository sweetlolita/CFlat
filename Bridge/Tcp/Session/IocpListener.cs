using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using CFlat.Utility;
using CFlat.Bridge.Tcp.Action;

namespace CFlat.Bridge.Tcp.Session
{
    class IocpListener
    {
        
        private IocpAcceptAction acceptAction { get; set; }

        

        public IocpListener(IPEndPoint listenerEndPoint, IocpSessionObserver observer)
        {
            acceptAction = new IocpAcceptAction(listenerEndPoint,
                ((socket) => { observer.onSessionConnected(socket); }),
                ((errorMessage) => { observer.onSessionError(Guid.Empty, errorMessage); }));
        }
        public void start()
        {
            acceptAction.accept();
        }

        public void stop()
        {
            acceptAction.shutdown();
        }
    }
}
