using System;
using System.Net.Sockets;

namespace CMaNGOSNET.Common.Network
{
    public interface IMessageHandleManager
    {
        ITCPClient TCPClient
        {
            get;
        }

        PacketProcessResult ProcessMessage(ITCPClient client, SocketAsyncEventArgs args);
    }
}
