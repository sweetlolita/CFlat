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
    public class IpcServer : SingleListServer<IpcMessage>
    {
        #region Private fields
        private IpcServerChannel channel;

        public delegate void IpcServerEventHandler(object sender, IpcMessage args);
        public event IpcServerEventHandler IpcServerEvent;

        #endregion

        #region Constructor
        public IpcServer() : base(IpcContext.requestQueue, IpcContext.serverThreadEvent)
        {
            channel = new IpcServerChannel(IpcConfig.channelDescriptor);
            Logger.debug("IpcServer: constructed.");
        }
        #endregion

        #region Threading
        public override void start()
        {
            Logger.debug("IpcServer: starting...");
            base.start();
        }

        public override void stop()
        {
            Logger.debug("IpcServer: stopping...");
            base.stop();
        }

        protected override void onStart()
        {
            IpcOpen();
            Logger.debug("IpcServer: start.");
        }

        protected override void onStop()
        {
            IpcClose();
            Logger.debug("IpcServer: stop.");
        }

        void IpcOpen()
        {
            try
            {
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(IpcContext),
                    IpcConfig.objectDescriptor,
                    WellKnownObjectMode.Singleton);
            }
            catch (System.Exception ex)
            {
                Logger.error("IpcServer: open with error: {0}.", ex.Message);
            }
        }

        void IpcClose()
        {
            channel.StopListening(null);
            ChannelServices.UnregisterChannel(channel);
        }
        #endregion

        #region Logical functions
        protected override void handleRequest(IpcMessage message)
        {
            Logger.info("IpcServer: IpcBridge =====> IpcServer.");
            Logger.debug("IpcServer: msg = {0}", message.request);
            IpcServerEvent(this, message);
        }

        public void postResponse(IpcMessage response)
        {
            Logger.info("IpcServer: IpcServer - - -> IpcBridge.");
            IpcContext.responseQueue.Enqueue(response);
            IpcContext.clientThreadEvent.Set();
            Logger.info("IpcServer: IpcServer =====> IpcBridge.");
        }

        #endregion
    }
}
