using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using CFlat.Bridge.Tcp.CommonEntity;

namespace CFlat.Bridge.Tcp.Action
{
    class IocpConnectAction : IocpActionBase
    {
        private Socket clientSocket { get; set; }
        private IPEndPoint clientEndPoint { get; set; }
        private IPEndPoint serverEndPoint { get; set; }
        private Action<Socket> onConnectedAction { get; set; }

        public IocpConnectAction(IPEndPoint clientEndPoint, IPEndPoint serverEndPoint, 
            Action<Socket> onConnectedAction, Action<string> onErrorAction)
            : base(onErrorAction)
        {
            this.clientSocket = null;
            this.clientEndPoint = clientEndPoint;
            this.serverEndPoint = serverEndPoint;
            this.onConnectedAction = onConnectedAction;
            
        }

        protected override void onIocpEvent(bool isSuccess, out bool continousAsyncCall)
        {
            if(isSuccess)
            {
                SocketOption.enableKeepAlive(clientSocket);
                onConnectedAction(clientSocket);
            }
            continousAsyncCall = false;
        }

        public void connect()
        {
            clientSocket = new Socket(clientEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            iocpAsyncDelegate = new IocpAsyncDelegate(clientSocket.ConnectAsync);
            iocpEventArgs.RemoteEndPoint = serverEndPoint;
            iocpOperation();
        }

        public void shutdown()
        {
            if(clientSocket.Connected)
            {
                clientSocket.Disconnect(false);
            }
        }
    }

}
