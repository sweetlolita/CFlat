using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Utility;

namespace CFlat.Bridge.Tcp.Session
{
    class IocpSessionMap
    {
        private ConcurrentDictionary<Guid, IocpSession> sessionDict { get; set; }

        public IocpSessionMap()
        {
            sessionDict = new ConcurrentDictionary<Guid, IocpSession>();
        }
        public void put(Guid keyGuid, IocpSession session)
        {
            if (session == null || session.sessionId == Guid.Empty)
            {
                throw new IocpException("session put to map cannot be null or empty guid.");
            }
            if (sessionDict.TryAdd(keyGuid, session) == false)
            {
                throw new IocpException(string.Format("session {0} by key {1} cannot put into map.",
                    session.sessionId, keyGuid));
            }
            Logger.debug("TcpServer: session {0} by key {1} has put into IocpSessionMap.",
                session.sessionId, keyGuid);
        }

        public IocpSession take(Guid keyGuid)
        {
            IocpSession session;
            if (sessionDict.TryRemove(keyGuid, out session) == false)
            {
                throw new IocpException(string.Format("key {1} cannot get any value from map.",
                     keyGuid));
            }
            Logger.debug("TcpServer: session {0} by key {1} has taken away from IocpSessionMap.",
                session.sessionId, keyGuid);
            return session;
        }

        public IocpSession search(Guid keyGuid)
        {
            IocpSession session;
            if (sessionDict.TryGetValue(keyGuid, out session) == false)
            {
                throw new IocpException(string.Format("key {1} cannot get any value from map.",
                     keyGuid));
            }
            return session;
        }
    }
}
