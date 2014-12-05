using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CFlat.Bridge.Tcp.Action
{
    class IocpSendAction : IocpActionBase
    {
        protected Socket socket { get; set; }
        private Action<int> onSentAction { get; set; }
        public IocpSendAction(Action<int> onSentAction, Action<string> onErrorAction)
            : base(onErrorAction)
        {
            this.onSentAction = onSentAction;
        }

        public void attachSocket(Socket socket)
        {
            this.socket = socket;
            this.iocpAsyncDelegate = new IocpAsyncDelegate(socket.SendAsync);
        }

        public void detachSocket()
        {
            this.iocpAsyncDelegate = null;
            try
            { 
                socket.Shutdown(SocketShutdown.Send);
                this.socket = null;
            }
            // throws if client process has already closed
            catch (Exception) { }
        }

        protected sealed override void onIocpEvent(bool isSuccess, out bool continousAsyncCall)
        {
            if (isSuccess)
            {
                onSentAction(iocpEventArgs.BytesTransferred);
            }
            continousAsyncCall = false;
        }

        public void send(byte[] buffer, int offset, int count)
        {
            if (buffer == null || count <= 0)
            {
                throw new IocpException("sending a null data.");
            }
            iocpEventArgs.SetBuffer(buffer, offset, count);
            iocpOperation();
        }

        public void shutdown()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) { }
        }
    }
}
