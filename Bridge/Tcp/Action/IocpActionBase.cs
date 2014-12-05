using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CFlat.Bridge.Tcp.Action
{
    abstract class IocpActionBase
    {
        protected delegate bool IocpAsyncDelegate(SocketAsyncEventArgs args);
        protected IocpAsyncDelegate iocpAsyncDelegate { get; set; }
        protected SocketAsyncEventArgs iocpEventArgs { get; private set; }
        private Action<string> onErrorAction { get; set; }
        protected IocpActionBase(Action<string> onErrorAction)
        {
            this.onErrorAction = onErrorAction;
            iocpEventArgs = new SocketAsyncEventArgs();
            iocpEventArgs.UserToken = this;
            iocpEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(this.onIocpEventBase);
        }

        protected abstract void onIocpEvent(bool isSuccess, out bool continousAsyncCall);

        protected void iocpOperation()
        {
            bool continousAsyncCall = true;
            while (continousAsyncCall == true &&
                //false if I/O operation completed synchronously
                iocpAsyncDelegate(iocpEventArgs) == false)
            {
                continousAsyncCall = false;
                onIocpEvent(isIocpSuccess(), out continousAsyncCall);
            }
        }

        protected void onIocpEventBase(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            //incoming param socketAsyncEventArgs should be exactly the same as wrapped iocpEventArgs
            if (socketAsyncEventArgs != iocpEventArgs)
            {
                throw new IocpException("Iocp Event Args not match.");
            }
            bool continousAsyncCall = false;
            onIocpEvent(isIocpSuccess(), out continousAsyncCall);
            while (continousAsyncCall == true &&
                //false if I/O operation completed synchronously
                iocpAsyncDelegate(socketAsyncEventArgs) == false)
            {
                continousAsyncCall = false;
                onIocpEvent(isIocpSuccess(), out continousAsyncCall);
            }
        }

        protected bool isIocpSuccess()
        {
            if (iocpEventArgs.SocketError == SocketError.OperationAborted)
            {
                //ignored operation cancel
                return false;
            }
            else if (iocpEventArgs.SocketError != SocketError.Success)
            {
                onErrorAction(iocpEventArgs.SocketError.ToString());
                return false;
            }
            else
            {
                return true;
            }
        }


    }

}
