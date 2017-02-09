using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CMaNGOSNET.Common.Resources;
using CMaNGOSNET.Common.Logging;
using CMaNGOSNET.Common.IO;

namespace CMaNGOSNET.Common.Network
{
    public abstract class TCPSocketServerBase : IDisposable, ITCPSocketServer
    {
        #region Variables
        protected bool running = false;
        protected bool disposed = false;
        protected Socket tcpSocket = null;

        // represents a large reusable set of buffers for all socket operations
        protected Semaphore maxNumberAcceptedClients;
        protected ConcurrentDictionary<Guid, ITCPClient> ClientCollection;
        #endregion

        #region Events
        public event ClientConnectedHandler ClientConnected;
        public event ClientDisconnectedHandler ClientDisconnected;
        #endregion

        #region Fields
        public SocketAsyncEventArgsPoolManager SocketAsyncEventArgsPoolManager
        {
            get;
            protected set;
        }

        public bool IsRunning
        {
            get { return running; }
            set
            {
                if (running != value)
                {
                    running = value;
                }
            }
        }

        public IPAddress IPAddress
        {
            get
            {
                return TcpEndPoint.Address;
            }
        }

        public int Port
        {
            get
            {
                return TcpEndPoint.Port;
            }
        }

        public IPEndPoint TcpEndPoint
        {
            get { return ServerSettings.EndPoint; }
        }

        public SocketListenerSettings ServerSettings
        {
            get;
            private set;
        }

        protected int numConnectedSockets;
        public int NumConnectedSockets
        {
            get
            {
                return numConnectedSockets;
            }
        }
        #endregion

        #region Constructor and Initialize
        public TCPSocketServerBase(SocketListenerSettings settings)
        {
            ServerSettings = settings;
            maxNumberAcceptedClients = new Semaphore(ServerSettings.MaxConnections, ServerSettings.MaxConnections);

            SocketAsyncEventArgsPoolManager = new SocketAsyncEventArgsPoolManager(ServerSettings.BufferSize, ServerSettings.MaxConnections);

            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ClientCollection = new ConcurrentDictionary<Guid, ITCPClient>();
        }

        public virtual void Init()
        {
            SocketAsyncEventArgsPoolManager.Init();
        }

        #endregion

        #region Client Handling

        protected void AddClient(ITCPClient client)
        {
            if (client == null)
            {
                return;
            }

            lock (ClientCollection)
            {
                if (ClientCollection.ContainsKey(client.TCPClientUID))
                {
                    CloseClient(ClientCollection[client.TCPClientUID], true);
                }

                ClientCollection[client.TCPClientUID] = client;
            }
        }

        protected void RemoveClient(ITCPClient client)
        {
            if (client == null)
            {
                return;
            }

            lock (ClientCollection)
            {
                ITCPClient tempClient = null;
                ClientCollection.TryRemove(client.TCPClientUID, out tempClient);
            }
        }

        public void CloseClient(ITCPClient client, bool forced = true)
        {
            try
            {
                if (client == null)
                {
                    return;
                }

                RemoveClient(client);

                CloseClientAcceptSocket(client.TcpSocket);

                client.CleanClient();

                OnClientDisconnected(client, forced);
            }
            catch (ObjectDisposedException)
            {

            }
            catch (Exception e)
            {
                Logger.Instance.Error("Could not disconnect client", e);
            }
        }

        protected void CloseClientAcceptSocket(Socket socket)
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Send);
                    socket.Close();

                    Interlocked.Decrement(ref numConnectedSockets);
                    maxNumberAcceptedClients.Release();

                    Logger.Instance.Info("A client has been disconnected. Clients connected: {0}", numConnectedSockets);
                }
            }
            catch (Exception)
            {

            }
        }


        public void RemoveAllClients()
        {
            lock (ClientCollection)
            {
                foreach (ITCPClient client in ClientCollection.Values)
                {
                    try
                    {
                        client.CleanClient();
                        CloseClientAcceptSocket(client.TcpSocket);

                        OnClientDisconnected(client, true);
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                ClientCollection.Clear();
            }
        }

        protected virtual bool OnClientConnected(ITCPClient client)
        {
            ClientConnectedHandler handler = ClientConnected;
            if (handler != null)
                handler(this, client);

            return true;
        }

        protected virtual void OnClientDisconnected(ITCPClient client, bool forced)
        {
            ClientDisconnectedHandler handler = ClientDisconnected;

            if (handler != null)
                handler(client, forced);

            client.Dispose();
        }

        #endregion

        #region Connection Management Method
        public virtual bool Start()
        {
            if (running)
            {
                IsRunning = true;

                Logger.Instance.Info("TCP Server is already started");
                return false;
            }

            try
            {
                Logger.Instance.Info("TCP Server is starting");
                tcpSocket.Bind(TcpEndPoint);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Could not bind to Address {0}: {1}", TcpEndPoint, ex);
                return false;
            }

            tcpSocket.Listen(ServerSettings.Backlog);
            tcpSocket.NoDelay = ServerSettings.NoDelay;

            StartAccept(null);

            IsRunning = true;
            Logger.Instance.Info("TCP Server is started");

            return true;
        }

        public virtual bool Stop()
        {
            try
            {
                RemoveAllClients();

                running = false;
                IsRunning = false;

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
                return false;
            }
        }

        protected abstract ITCPClient CreateClient(Socket socket);

        private void AcceptEventCompleted(object sender, SocketAsyncEventArgs args)
        {
            ProcessAccept(args);
        }

        private void ProcessAccept(SocketAsyncEventArgs args)
        {
            try
            {
                if (!running)
                {
                    Logger.Instance.Info(ServerMessage.SERVER_IS_NOT_RUNNING);
                    return;
                }
                
                Interlocked.Increment(ref numConnectedSockets);

                if (args.SocketError != SocketError.Success)
                {
                    //Let's destroy this socket, since it could be bad.
                    IDataHoldingUserToken theAcceptOpToken = (IDataHoldingUserToken)args.UserToken;
                    Logger.Instance.Error("SocketError, accept id: " + theAcceptOpToken.DataHoldingUserTokenGUID);

                    // before return start accept in final;
                    CloseClientAcceptSocket(args.AcceptSocket);

                    return;
                }


                

                // Start accept again;
                ITCPClient client = CreateClient(args.AcceptSocket);

                AddClient(client);

                OnClientConnected(client);

                Logger.Instance.Info("A client has connected. {0}:{1} Clients connected: {2}", client.ClientAddress.ToString(), client.Port, numConnectedSockets);

                client.StartReceive();

            }
            catch (ObjectDisposedException)
            {

            }
            catch (SocketException e)
            {
                // TODO: Add a proper exception handling for the different SocketExceptions Error Codes.
                Logger.Instance.Warn(ServerMessage.SocketExceptionAsyncAccept, e);
            }
            catch (Exception e)
            {
                Logger.Instance.Fatal(ServerMessage.FatalAsyncAccept, e);
            }
            finally
            {
                StartAccept(args);
            }
        }

        protected void StartAccept(SocketAsyncEventArgs args)
        {
            // Block if there are too many connections
            maxNumberAcceptedClients.WaitOne();

            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += AcceptEventCompleted;
            }
            else
            {
                args.AcceptSocket = null;
            }
            
            bool willRaiseEvent = tcpSocket.AcceptAsync(args);
            if (!willRaiseEvent)
            {
                ProcessAccept(args);
            }
        }



        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if (disposing)
                {
                    maxNumberAcceptedClients.Dispose();
                    // managed resource, disposable objects
                    Stop();
                }

                disposed = true;
            }
        }

        #endregion
    }
}
