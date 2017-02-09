using System;
using System.Net;

namespace CMaNGOSNET.Common.Network
{
    public class SocketListenerSettings
    {
        public static SocketListenerSettings DefaultSocketListenerSettings
        {
            get
            {
                return new SocketListenerSettings(250, 250, 20, 50, 10, 2048, 2048, 2, new IPEndPoint(IPAddress.Any, 8080), true);
            }
        }

        public SocketListenerSettings(int maxConnections,
        int excessSaeaObjectsInPool, int backlog, int maxSimultaneousAcceptOps,
        int receivePrefixLength, int receiveBufferSize, int sendPrefixLength,
        int opsToPreAlloc, IPEndPoint theLocalEndPoint, bool noDelay = true)
        {
            MaxConnections = maxConnections;
            NumberOfSaeaForRecSend = maxConnections + excessSaeaObjectsInPool;
            Backlog = backlog;
            MaxAcceptOps = maxSimultaneousAcceptOps;
            ReceivePrefixLength = receivePrefixLength;
            BufferSize = receiveBufferSize;
            SendPrefixLength = sendPrefixLength;
            OpsToPreAllocate = opsToPreAlloc;
            EndPoint = theLocalEndPoint;

            NoDelay = noDelay;
        }

        public bool NoDelay
        {
            get;
            private set;
        }

        public int MaxConnections
        {
            get;
            private set;
        }
        public int NumberOfSaeaForRecSend
        {
            get;
            private set;
        }
        public int Backlog
        {
            get;
            private set;
        }
        public int MaxAcceptOps
        {
            get;
            private set;
        }
        public int ReceivePrefixLength
        {
            get;
            private set;
        }
        public int BufferSize
        {
            get;
            private set;
        }
        public int SendPrefixLength
        {
            get;
            private set;
        }
        public int OpsToPreAllocate
        {
            get;
            private set;
        }
        public IPEndPoint EndPoint
        {
            get;
            private set;
        }
    }
}
