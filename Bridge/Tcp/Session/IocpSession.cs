using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using CFlat.Bridge.Tcp.Action;
using CFlat.Utility;
using System.Threading;

namespace CFlat.Bridge.Tcp.Session
{
    class IocpSession
    {
        public Guid sessionId { get; set; }
        private Socket socket { get; set; }
        private IocpSendAction sendAction { get; set; }
        private IocpReceiveAction recvAction { get; set; }
        private IocpSessionObserver observer { get; set; }
        private ConcurrentQueue<Descriptor> sendQueue { get; set; }
        private int sendQueueStatus;
        private static int sendQueueIdle = 0;
        private static int sendQueueRunning = 1;
        private void clearSendQueue()
        {
            Descriptor ignored;
            while (sendQueue.TryDequeue(out ignored)) { };
        }

        public IocpSession(IocpSessionObserver observer)
        {
            socket = null;
            sessionId = Guid.Empty;
            this.observer = observer;
            sendAction = new IocpSendAction(
                ((bytesSent) => this.onIocpSendActionEvent(bytesSent)),
                ((errorMessage) => { observer.onSessionError(sessionId, errorMessage); }));
            recvAction = new IocpReceiveAction(
                (descriptor) => this.onIocpReceiveActionEvent(descriptor),
                ((errorMessage) => { observer.onSessionError(sessionId, errorMessage); }));
            sendQueue = new ConcurrentQueue<Descriptor>();
            sendQueueStatus = sendQueueIdle;
            Logger.debug("IocpSession: constructed.");
        }

        public void attachSocket(Socket socket)
        {
            this.socket = socket;
            sendAction.attachSocket(socket);
            recvAction.attachSocket(socket);
            IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
            Logger.info("TcpSession: session {0} starts. remote address = {1}:{2}",
                sessionId, remoteIpEndPoint.Address, remoteIpEndPoint.Port);
            
        }

        public void detachSocket()
        {
            if(socket != null)
            {
                IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
                Logger.info("TcpSession: session {0} ends. remote address = {1}:{2}",
                    sessionId, remoteIpEndPoint.Address, remoteIpEndPoint.Port);
                sendAction.detachSocket();
                recvAction.detachSocket();
                socket.Close();
                socket = null;
                clearSendQueue();
            }
            
        }

        private void onIocpReceiveActionEvent(Descriptor descriptor)
        {
            Logger.debug("IocpSession: session {0} on receive event ", sessionId);
            if (descriptor == null)
            {
                Logger.debug("IocpSession: session {0} receives nothing as Disconnect signal.", sessionId);
                dispose(this, EventArgs.Empty);
                observer.onSessionDisconnected(sessionId);
            }
            else
            {
                Logger.debug("IocpSession: session {0} receives {1} bytes of data. \n hex data: {2}, \n ascii string data: {3}",
                    sessionId, 
                    descriptor.desLength, 
                    BitConverter.ToString(descriptor.des, 0, descriptor.desLength),
                    System.Text.Encoding.ASCII.GetString(descriptor.des, 0, descriptor.desLength));
                digest(descriptor);
            }
        }

        private void onIocpSendActionEvent(int bytesSent)
        {
            Logger.debug("IocpSession: session {0} sends {1} byte(s) of data.", sessionId, bytesSent);
            sendNextItem();
        }

        private void digest(Descriptor descriptor)
        {
            observer.onSessionData(sessionId, descriptor);
        }

        public void send(byte[] buffer, int offset, int count)
        {
            DescriptorBuffer descriptorBuffer = DescriptorBuffer.create(buffer, offset, count, count);
            sendQueue.Enqueue(descriptorBuffer);

            if (Interlocked.CompareExchange(ref sendQueueStatus, sendQueueRunning, sendQueueIdle) == sendQueueIdle)
            {
                sendNextItem();
            }
            
        }

        private void sendNextItem()
        {
            Descriptor sendBuffer;
            if (sendQueue.TryDequeue(out sendBuffer) == true)
            {
                sendAction.send(sendBuffer.des, 0, sendBuffer.desLength);
            }
            else
            {
                sendQueueStatus = sendQueueIdle;
            }
        }

        public void recv()
        {
            recvAction.recv();
        }

        public void dispose(object sender, EventArgs args)
        {
            detachSocket();
            Logger.debug("IocpSession: session {0} disposed.", sessionId);
            //observer.onSessionDisconnected(sessionId);
        }

    }

}
