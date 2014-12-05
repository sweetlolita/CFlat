using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Utility;

namespace CFlat.Bridge.Tcp.Session
{
    class IocpSessionPool
    {
        private ConcurrentBag<IocpSession> sessionBag { get; set; }
        private Func<IocpSession> itemConstructor { get; set; }
        
        public IocpSessionPool(Func<IocpSession> itemConstructor)
        {
            if (itemConstructor == null)
            {
                throw new IocpException("session pool item constructor cannot be null.");
            }
            this.sessionBag = new ConcurrentBag<IocpSession>();
            this.itemConstructor = itemConstructor;
            Logger.debug("TcpServer: IocpSessionPool constructed.");
        }

        public void put(IocpSession session)
        {
            if (session == null) 
            {
                throw new IocpException("session put back to pool cannot be null"); 
            }
            Guid savedSessionId = session.sessionId;
            session.sessionId = Guid.Empty;
            sessionBag.Add(session);
            Logger.debug("TcpServer: session {0} has put back into IocpSessionPool.", savedSessionId);
        }

        public IocpSession take()
        {
            IocpSession session;
            if (sessionBag.TryTake(out session) == false)
            {
                Logger.debug("requires more context instance from IocpSessionPool, create one.");
                session = itemConstructor();
            }
            session.sessionId = Guid.NewGuid();
            Logger.debug("TcpServer: session {0} has took away from IocpSessionPool.", session.sessionId);
            return session;
        }

        public int available()
        {
            return sessionBag.Count;
        }


    }
}
