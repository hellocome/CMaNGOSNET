using System;
using System.Net;
using System.Net.Sockets;

namespace CMaNGOSNET.Common.Network
{
    public interface ITCPClient : IDisposable
    {
        Guid TCPClientUID { get; }

        ITCPSocketServer Server { get; }

        /// <summary>
        /// Gets the IP address of the client.
        /// </summary>
        IPAddress ClientAddress { get; }

        /// <summary>
        /// Gets the port the client is communicating on.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets/Sets the socket this client is using for TCP communication.
        /// </summary>
        Socket TcpSocket { get; }

        SocketAsyncEventArgs SocketEventArgs { get; }

        bool IsConnected { get; }

        /// <summary>
        /// Begins asynchronous TCP receiving for this client.
        /// </summary>
		void BeginReceive();

		/// <summary>
		/// Asynchronously sends a packet of data to the client.
		/// </summary>
		/// <param name="packet">An array of bytes containing the packet to be sent.</param>
		void BeginSend(byte[] packet);

        /// <summary>
        /// Asynchronously sends a packet of data to the client.
        /// </summary>
        /// <param name="packet">An array of bytes containing the packet to be sent.</param>
        /// <param name="length">The number of bytes to send starting at offset.</param>
        /// <param name="offset">The offset into packet where the sending begins.</param>
		void BeginSend(byte[] packet, int offset, int length);

        void Connect(string host, int port);

        void Connect(IPAddress addr, int port);

        void ProcessReceive(SocketAsyncEventArgs args);

        void StartReceive();

        void CleanClient();

        void StartSend();
    }
}