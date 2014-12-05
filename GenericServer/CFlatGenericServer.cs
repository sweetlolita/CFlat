using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Utility;
using CFlat.GenericServer.Context;

namespace CFlat.GenericServer
{
    public class CFlatGenericServer : SingleListServer<CfgsContext>
    {
        private ActivityMap activityMap { get; set; }
        public CFlatGenericServer()
            : base()
        {
            Logger.debug("CFlatGenericServer: constructed.");
        }

        public void registerActivity(string business, string method, ActivityBase activity)
        {
            activityMap.put(makeVerb(business, method), activity);
        }

        public void unregisterActivity(string business, string method)
        {
            activityMap.remove(makeVerb(business, method));
        }

        private string makeVerb(string business, string method)
        {
            return business + "/" + method;
        }

        public override void start()
        {
            Logger.debug("CFlatGenericServer: starting...");
            base.start();
        }

        public override void stop()
        {
            Logger.debug("CFlatGenericServer: stopping...");
            base.stop();
        }

        protected override void onStart()
        {
            Logger.debug("CFlatGenericServer: start.");
        }

        protected override void onStop()
        {
            Logger.debug("CFlatGenericServer: stop.");
        }

        protected override void handleRequest(CfgsContext context)
        {
            Logger.debug("CFlatGenericServer: handle request = {0}/{1} param = {2}",
                context.request.business, context.request.method,
                Logger.logObjectList(context.request.param));
            try
            {
                string verb = makeVerb(context.request.business, context.request.method);
                ActivityBase activity = activityMap.search(verb);
                if (activity != null)
                {
                    Logger.debug("a {0} action is delivered to its handler.", verb);
                    activity.act(context.request.param);
                    context.response.isSuccess = true;
                }
                else
                {
                    throw new CFlatExceptionBase("cannot find handler to deal with " + verb + "action.");
                }
            }
            catch(Exception ex)
            {
                context.response.isSuccess = false;
                context.response.errorMessage = ex.Message;
                Logger.error("CFlatGenericServer: skip this request for reason: {0}", ex.Message);
            }
            finally
            {
                context.response.onResponsed();
            }
        }
    
    }
}
