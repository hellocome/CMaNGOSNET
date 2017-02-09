using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using CMaNGOSNET.Common.Network;
using CMaNGOSNET.AuthServer.ServerImpl.ServerHandle;

namespace CMaNGOSNET.AuthServer.ServerImpl
{
    public class AuthClient : TCPClientBase
    {
        AuthSession session = new AuthSession();

        public AuthClient(ITCPSocketServer server, Socket socket) : base(server, socket)
        {

        }

        public override PacketProcessResult ProcessIncomingData(SocketAsyncEventArgs args)
        {
            session.ProcessMessage(this, args);

            return PacketProcessResult.FATAL;
        }
    }
}
