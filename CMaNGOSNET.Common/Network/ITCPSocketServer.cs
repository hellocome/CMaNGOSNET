using System;
using System.Net;
using System.Net.Sockets;

namespace CMaNGOSNET.Common.Network
{
    #region Event Delegates

    public delegate void ClientConnectedHandler(object sender, ITCPClient client);
    public delegate void ClientDisconnectedHandler(ITCPClient client, bool forced);

    #endregion

    public interface ITCPSocketServer
    {
        event ClientConnectedHandler ClientConnected;
        event ClientDisconnectedHandler ClientDisconnected;

        SocketAsyncEventArgsPoolManager SocketAsyncEventArgsPoolManager
        {
            get;
        }

        bool IsRunning
        {
            get;
        }

        IPAddress IPAddress
        {
            get;
        }

        int Port
        {
            get;
        }

        IPEndPoint TcpEndPoint
        {
            get;
        }

        SocketListenerSettings ServerSettings
        {
            get;
        }
        
        int NumConnectedSockets
        {
            get;
        }
        
        void CloseClient(ITCPClient client, bool forced = true);

        void Init();

        bool Start();

        bool Stop();
    }
}
