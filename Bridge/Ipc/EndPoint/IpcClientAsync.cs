using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

using CFlat.Utility;
using CFlat.Bridge.Ipc.RemoteObject;
using CFlat.Bridge.Ipc.CommonEntity;

namespace CFlat.Bridge.Ipc.EndPoint
{
    //warning: async mode is not under test
    public class IpcClientAsync : SingleListServer<IpcMessage>
    {
        #region Private fields
        private IpcContext ipcContext { get; set; }

        public delegate void IpcClientEventHandler(object sender, IpcMessage args);
        private event IpcClientEventHandler IpcClientEvent;
        #endregion

        #region Constructor
        public IpcClientAsync()
            : base(getContext().getResponseQueue(), getContext().getClientThreadEvent())
        {
            this.ipcContext = getContext();
        }

        public static IpcContext getContext()
        {
            return (IpcContext)Activator.GetObject(
                 typeof(IpcContext), IpcConfig.fullDescriptor);
        }
        #endregion

        #region Threading
        public override void start()
        {
            Logger.debug("IpcClientAsync: Staring...");
            base.start();
        }

        public override void stop()
        {
            Logger.debug("IpcClientAsync: Stopping...");
            base.stop();
        }
        #endregion

        #region Logical functions

        public Guid postMessage(string request)
        {
            try
            {
                IpcMessage msg = new IpcMessage(false, request);
                ipcContext.postRequest(msg);
                return msg.guid;
            }
            catch (System.Exception ex)
            {
                Logger.error("IpcClient: post with error: {0}.", ex.Message);
                return Guid.Empty;
            }
        }

        protected override void handleRequest(IpcMessage message)
        {
            Logger.debug("onMessageResponse. msg = {0}",
                message.request);
            IpcClientEvent(this, message);
        }

        public void registerIpcClientEventHandler(IpcClientEventHandler handler)
        {
            this.IpcClientEvent = handler;
        }



        #endregion
    }
}
