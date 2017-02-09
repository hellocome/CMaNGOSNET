using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using CMaNGOSNET.Common.Network;
using CMaNGOSNET.Common.Logging;

namespace CMaNGOSNET.Common.Network
{
    public abstract class TCPClientBase : ITCPClient
    {
        private bool disposed = false;

        public TCPClientBase(ITCPSocketServer server, Socket socket)
        {
            TCPClientUID = Guid.NewGuid();
            Server = server;
            TcpSocket = socket;
            
            SocketEventArgs = Server.SocketAsyncEventArgsPoolManager.AcquirerSocketAsyncEventArgs();
            SocketEventArgs.Completed += ReceiveAsyncComplete;
        }

        public Guid TCPClientUID { get; private set; }

        public ITCPSocketServer Server { get; private set; }

        public IPAddress ClientAddress
        {
            get
            {
                return (TcpSocket != null && TcpSocket.RemoteEndPoint != null) ? ((IPEndPoint)TcpSocket.RemoteEndPoint).Address : null;
            }
        }

        public int Port
        {
            get
            {
                return (TcpSocket != null && TcpSocket.RemoteEndPoint != null) ? ((IPEndPoint)TcpSocket.RemoteEndPoint).Port : -1;
            }
        }

        public Socket tcpSocket;
        /// <summary>
        /// Gets/Sets the socket this client is using for TCP communication.
        /// </summary>
        public Socket TcpSocket
        {
            get
            {
                return tcpSocket;
            }

            private set
            {
                if(tcpSocket != null && tcpSocket.Connected)
                {
                    tcpSocket.Close();
                }

                tcpSocket = value;
            }
        }

        public SocketAsyncEventArgs SocketEventArgs
        {
            get;
            private set;
        }

        public bool IsConnected
        {
            get
            {
                return (TcpSocket != null) ? TcpSocket.Connected : false;
            }
        }

        private void ReceiveAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            ProcessReceive(args);
        }

        public void BeginReceive()
        {
            bool willRaiseEvent = TcpSocket.ReceiveAsync(SocketEventArgs);

            if (!willRaiseEvent)
            {
                ProcessReceive(SocketEventArgs);
            }
        }

        public virtual void BeginSend()
        {
            throw new NotSupportedException("BeginSend() is not supported");
        }

        public void BeginSend(byte[] packet)
        {
            if (packet == null)
            {
                return;
            }

            BeginSendInternal(packet, 0, packet.Length);
        }

		public void BeginSend(byte[] packet, int offset, int length)
        {
            BeginSendInternal(packet, offset, length);
        }

        protected void BeginSendInternal(byte[] packet, int offset, int length)
        {
            if (packet == null || offset >= packet.Length || length <= 0)
            {
                return;
            }

            IDataHoldingUserToken token = (IDataHoldingUserToken)SocketEventArgs.UserToken;

            if (TcpSocket != null && TcpSocket.Connected && SocketEventArgs != null && token != null)
            {
                Array.Copy(packet, offset, SocketEventArgs.Buffer, token.SendOffset, length);

                TcpSocket.SendAsync(SocketEventArgs);
            }
            else
            {
                Server.CloseClient(this);
            }
        }

        public void Connect(string host, int port)
        {

        }

        public void Connect(IPAddress addr, int port)
        {

        }

        public void ProcessReceive(SocketAsyncEventArgs args)
        {
            try
            {
                int bytesReceived = args.BytesTransferred;

                if (bytesReceived == 0)
                {
                    Server.CloseClient(this, true);
                }
                else
                {
                    IDataHoldingUserToken token = (IDataHoldingUserToken)args.UserToken;
                    token.RemainingReceiveDataCount += bytesReceived;

                    PacketProcessResult result = ProcessIncomingData(args);

                    if (result == PacketProcessResult.SUCCESSFUL)
                    {
                        token.ResetReceivedCount();
                    }
                    else
                    {
                        if (result == PacketProcessResult.INCOMPLETE_MESSAGE)
                        {
                            Array.Copy(args.Buffer, token.ProcessedReceiveDataOffset, args.Buffer, token.ReceiveOffset, token.RemainingReceiveDataCount);

                            token.ProcessedReceiveDataOffset = token.ReceiveOffset;

                            SocketEventArgs.SetBuffer(token.ReceiveOffset + token.RemainingReceiveDataCount, 
                                token.RemainingReceiveDataBufferLength);
                        }
                        else
                        {
                            Server.CloseClient(this, true);
                            return;
                        }
                    }

                    BeginReceive();
                }
            }
            catch (ObjectDisposedException)
            {
                Server.CloseClient(this, true);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                Server.CloseClient(this, true);
            }
        }

        public abstract PacketProcessResult ProcessIncomingData(SocketAsyncEventArgs args);

        public void StartReceive()
        {
            BeginReceive();
        }

        public void CleanClient()
        {
            try
            {
                Server.SocketAsyncEventArgsPoolManager.ReleaseSocketAsyncEventArgs(SocketEventArgs);
                SocketEventArgs.Completed -= ReceiveAsyncComplete;
            }
            catch (Exception)
            {
            }

            try
            {
                if (TcpSocket != null && TcpSocket.Connected)
                {
                    TcpSocket.Close();
                }
            }
            catch (Exception) { }
        }

        public void StartSend()
        {
            BeginSend();
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    CleanClient();
                }

                disposed = true;
            }
        }

        #endregion
    }
}
