using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

using CMaNGOSNET.Common.Network;

namespace CMaNGOSNET.AuthServer.ServerImpl
{
    public sealed class AuthenticationServer : TCPSocketServerBase
    {
        public AuthenticationServer(SocketListenerSettings settings) : base(settings)
        {

        }

        public bool IsShuttingDown
        {
            get;
            private set;
        }

        public bool IsPreparingShutdown
        {
            get;
            private set;
        }

        protected override ITCPClient CreateClient(Socket socket)
        {
            return new AuthClient(this, socket);
        }
    }
}
