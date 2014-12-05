using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using CFlat.Utility;

namespace CFlat.Bridge.Tcp.Action
{
    class IocpAcceptAction : IocpActionBase
    { 
        private Socket listenerSocket { get; set; }
        private IPEndPoint listenerEndPoint { get; set; }
        private bool continousAccpet { get; set; }
        private Action<Socket> onAcceptedAction { get; set; }

        private const int initialPendingConnectionCount = 16;
        public IocpAcceptAction(IPEndPoint listenerEndPoint, Action<Socket> onAcceptedAction, Action<string> onErrorAction)
            : base (onErrorAction)
        {
            this.listenerEndPoint = listenerEndPoint;
            this.onAcceptedAction = onAcceptedAction;
            
        }

        protected sealed override void onIocpEvent(bool isSuccess, out bool continousAsyncCall)
        {
            if (isSuccess)
            {
                if (iocpEventArgs.AcceptSocket.Connected)
                {
                    onAcceptedAction(iocpEventArgs.AcceptSocket);
                }
            }
            // Socket must be cleared since the context object is being reused.
            iocpEventArgs.AcceptSocket = null;
            continousAsyncCall = continousAccpet;
        }

        public void accept()
        {
            listenerSocket = new Socket(listenerEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            iocpAsyncDelegate = new IocpAsyncDelegate(listenerSocket.AcceptAsync);
            if (listenerEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // Set dual-mode (IPv4 & IPv6) for the socket listener.
                // 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below,
                // based on http://blogs.msdn.com/wndp/archive/2006/10/24/creating-ip-agnostic-applications-part-2-dual-mode-sockets.aspx
                listenerSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                listenerSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, listenerEndPoint.Port));
            }
            else
            {
                // Associate the socket with the local endpoint.
                listenerSocket.Bind(listenerEndPoint);
            }
            Logger.debug("IocpListener: address {0}:{1} binded.", listenerEndPoint.Address.ToString(), listenerEndPoint.Port);
            listenerSocket.Listen(initialPendingConnectionCount);
            Logger.debug("IocpListener: address {0}:{1} start listen.", listenerEndPoint.Address.ToString(), listenerEndPoint.Port);
            
            continousAccpet = true;
            iocpOperation();
            Logger.debug("IocpListener: address {0}:{1} srart accept.", listenerEndPoint.Address.ToString(), listenerEndPoint.Port);
        }

        public void shutdown()
        {
            Logger.debug("IocpListener: address {0}:{1} stopping.", listenerEndPoint.Address.ToString(), listenerEndPoint.Port);
            
            continousAccpet = false;
            listenerSocket.Close();

            Logger.debug("IocpListener: address {0}:{1} stop.", listenerEndPoint.Address.ToString(), listenerEndPoint.Port);      
        
        }
    }

}
