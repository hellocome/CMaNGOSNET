using System;
using System.Net.Sockets;
using CMaNGOSNET.Common.Network;

namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle
{
    public class AuthHandle
    {
        public AuthCommand Command
        {
            get;
            private set;
        }

        public AuthCommandStatus Status
        {
            get;
            private set;
        }

        public Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult> Handler
        {
            get;
            private set;
        }

        public AuthHandle(AuthCommand command, AuthCommandStatus status, Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult> handler)
        {
            Command = command;
            Status = status;
            Handler = handler;
        }
    }
}
