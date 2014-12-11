using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Bridge.Tcp.EndPoint;
using CFlat.Utility;
using CFlat.Bridge.Cfp.PacketPresentation;
using CFlat.Bridge.Cfp.CommonEntity;

namespace CFlat.Bridge.Cfp.Gutter
{
    public class CfpGutter : ActivityFactory, TcpEndPointObserver
    {
        private CfpGutterObserver cfpGutterObserver { get; set; }

        private delegate void TcpDataAsyncHandlerDelegate(Guid sessionId, Descriptor descriptor);
        protected CfpGutter(CfpGutterObserver cfpGutterObserver)
        {
            this.cfpGutterObserver = cfpGutterObserver;
        }

        //data from transport layer
        void TcpEndPointObserver.onTcpData(Guid sessionId, Descriptor descriptor)
        {
            TcpDataAsyncHandlerDelegate delegateMethod = new TcpDataAsyncHandlerDelegate(this.tcpDataAsyncHandler);
            DescriptorBuffer copiedBuffer = DescriptorBuffer.create(descriptor);
            delegateMethod.BeginInvoke(sessionId, copiedBuffer, null, null);
        }

        private void tcpDataAsyncHandler(Guid sessionId, Descriptor descriptor)
        {
            try
            {
                //deliver to presentation layer
                PacketDataUnitBase packetDataUnit = null;
                PacketDataUnitParser packetDataUnitParser = new PacketDataUnitParser(descriptor);
                while (packetDataUnitParser.parseIfHasNext(out packetDataUnit))
                {
                    Activity activity = createActivityFromJson(packetDataUnit.jsonPayload);
                    
                    if (activity != null)
                    {
                        CfpPlayer player = activity.player as CfpPlayer;
                        player.sessionId = sessionId;

                        Logger.debug("a {0} action is spawn.",
                            activity.playground.verb);
                        if(cfpGutterObserver != null)
                        {
                            cfpGutterObserver.onCfpActivity(activity);
                        }
                    }
                    else
                    {
                        Logger.error("cannot find handler : {0}", packetDataUnit.jsonPayload);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.error("CfpGutter: skip this tcp data for reason: {0}", ex.Message);
            }
        }

        void TcpEndPointObserver.onTcpConnected(Guid sessionId)
        {
            if (cfpGutterObserver != null)
            {
                cfpGutterObserver.onCfpConnected(sessionId);
            }
        }

        void TcpEndPointObserver.onTcpDisconnected(Guid sessionId)
        {
            if (cfpGutterObserver != null)
            {
                cfpGutterObserver.onCfpDisconnected(sessionId);
            }
        }

        void TcpEndPointObserver.onTcpError(Guid sessionId, string errorMessage)
        {
            if (cfpGutterObserver != null)
            {
                cfpGutterObserver.onCfpError(sessionId, errorMessage);
            }
        }
    }
}
