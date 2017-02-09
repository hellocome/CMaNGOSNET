using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace CMaNGOSNET.Common.Network
{
    public abstract class MessageHandleManager : IMessageHandleManager
    {
        public ITCPClient TCPClient
        {
            get;

            protected set;
        }

        public abstract PacketProcessResult ProcessMessage(ITCPClient client, SocketAsyncEventArgs args);


        protected virtual void SendPacket(PacketOut packet)
        {
            TCPClient.BeginSend(packet.GetFinalizedPacket());
        }
    }
}
