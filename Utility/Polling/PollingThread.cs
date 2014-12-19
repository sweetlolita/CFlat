using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CFlat.Utility
{
    public abstract class PollingThread
    {
        private Thread thread { get; set; }
        private AutoResetEvent terminalEvent { get; set; }
        private int pollingPeriod { get; set; }
        public Action<string> onError { get; set; }
        public PollingThread(int pollingPeriod)
        {
            this.pollingPeriod = pollingPeriod;
            terminalEvent = new AutoResetEvent(false);
        }

        static void invoke(object poller)
        {
            PollingThread transPoller = poller as PollingThread;
            transPoller.polling();
        }

        public virtual void start()
        {
            thread = new Thread(invoke);
            thread.Start(this);
        }

        public virtual void stop()
        {
            terminalEvent.Set();
        }
        private void polling()
        {
            try
            {
                onStart();
                while (!terminalEvent.WaitOne(0))
                {
                    onPolling();
                    if(pollingPeriod > 0)
                    {
                        Thread.Sleep(pollingPeriod);
                    }
                }
            }
            catch (System.Exception ex)
            {
                stop();
                Logger.error(ex.Message);
                if (onError != null)
                    onError(ex.Message);
            }
            finally
            {
                onStop();
            }
        }
        protected abstract void onPolling();
        protected virtual void onStart()
        {

        }
        protected virtual void onStop()
        {

        }
    }
}
