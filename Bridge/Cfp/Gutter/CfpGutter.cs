using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Bridge.Tcp.EndPoint;
using CFlat.Utility;
using CFlat.Bridge.Cfp.Presentation;

namespace CFlat.Bridge.Cfp.Gutter
{
    public class CfpGutter : TcpEndPointObserver
    {
        private ActivityMap activityMap { get; set; }
        protected CfpGutter()
        {
            activityMap = new ActivityMap();
        }

        public void registerActivity(string verb, ActivityBase activity)
        {
            activityMap.put(verb, activity);
        }

        public void unregisterActivity(string verb)
        {
            activityMap.remove(verb);
        }

        //data from transport layer
        void TcpEndPointObserver.onTcpData(Guid sessionId, Descriptor descriptor)
        {
            try
            {
                //deliver to presentation layer
                PacketDataUnitBase packetDataUnit = null;
                PacketDataUnitParser packetDataUnitParser = new PacketDataUnitParser(descriptor);
                while (packetDataUnitParser.parseIfHasNext(out packetDataUnit))
                {
                    string verb = packetDataUnit.verb;
                    ActivityBase activity = activityMap.search(verb);
                    if (activity != null)
                    {
                        Logger.debug("a {0} action is delivered to its handler.", verb);
                        activity.act(new List<object> { sessionId, packetDataUnit.jsonPayload });
                    }
                    else
                    {
                        Logger.error("cannot find handler to deal with {0} action.", verb);
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
            throw new NotImplementedException();
        }

        void TcpEndPointObserver.onTcpDisconnected(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        void TcpEndPointObserver.onTcpError(Guid sessionId, string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
