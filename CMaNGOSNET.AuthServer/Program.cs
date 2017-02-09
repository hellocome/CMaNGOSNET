using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.AuthServer.ServerImpl;
using CMaNGOSNET.Common.Network;

namespace CMaNGOSNET.AuthServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CMaNGOSNET.Common.Logging.Logger.InitLogger("AuthServer");
            AuthenticationServer server = new AuthenticationServer(SocketListenerSettings.DefaultSocketListenerSettings);
            server.Init();
            server.Start();

            Console.Read();
        }
    }
}
