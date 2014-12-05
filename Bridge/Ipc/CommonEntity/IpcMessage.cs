using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CFlat.Utility;

namespace CFlat.Bridge.Ipc.CommonEntity
{
    [Serializable]
    public class IpcMessage
    {
        public Guid guid { get; private set; }
        //public string business { get; set; }
        //public string method { get; set; }
        public string request { get; set; }
        public string response { get; set; }

        [NonSerialized]
        internal EventWaitHandle syncHandle;


        internal IpcMessage(bool isSync, string request)
        {
            this.guid = Guid.NewGuid();
            this.request = request;
            if (isSync)
            {
                syncHandle = new EventWaitHandle(false, EventResetMode.AutoReset, this.guid.ToString());
            }
            else
            {
                this.syncHandle = null;
            }
        }

        public void wait()
        {
            try
            {
                EventWaitHandle handle = EventWaitHandle.OpenExisting(this.guid.ToString());
                handle.WaitOne(-1);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                Logger.debug("IpcMessage: async message should not wait.");
            }
        }

        public void notify()
        {
            try
            {
                Logger.info("IpcServer: IpcServer - - -> IpcBridge.");
                EventWaitHandle handle = EventWaitHandle.OpenExisting(this.guid.ToString());
                handle.Set();
                Logger.info("IpcServer: IpcServer =====> IpcBridge.");
            }catch (WaitHandleCannotBeOpenedException)
            {
                Logger.debug("IpcMessage: async message should not notify.");
            }
        }
    }

}
