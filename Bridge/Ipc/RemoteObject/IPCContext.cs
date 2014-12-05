using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CFlat.Bridge.Ipc.CommonEntity;

namespace CFlat.Bridge.Ipc.RemoteObject
{
    public class IpcContext : MarshalByRefObject
    {
        #region Messaging objects


        #endregion

        #region Messaging Queues
        public static ConcurrentQueue<IpcMessage> requestQueue = new ConcurrentQueue<IpcMessage>();
        public static ConcurrentQueue<IpcMessage> responseQueue = new ConcurrentQueue<IpcMessage>();

        public static AutoResetEvent serverThreadEvent = new AutoResetEvent(false);
        public static AutoResetEvent clientThreadEvent = new AutoResetEvent(false);

        //public static object requestQueueMutex = new object();
        //public static object responseQueueMutex = new object();
        #endregion

        public void postRequest(IpcMessage message)
        {
            requestQueue.Enqueue(message);
            serverThreadEvent.Set();
        }

        //public AutoResetEvent getServerThreadEvent()
        //{
        //    return serverThreadEvent;
        //}

        public AutoResetEvent getClientThreadEvent()
        {
            return clientThreadEvent;
        }

        
        //public ConcurrentQueue<RemoteMessage> getRequestQueue()
        //{
        //    return requestQueue;
        //}

        public ConcurrentQueue<IpcMessage> getResponseQueue()
        {
            return responseQueue;
        }

    }
}
