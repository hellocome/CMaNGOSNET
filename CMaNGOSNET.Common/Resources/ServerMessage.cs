using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.Common.Resources
{
    public static class ServerMessage
    {
        public static readonly string SERVER_IS_NOT_RUNNING = "Server is not running!";
        public static readonly string AccountChallenge = "Client attempting to challenge auth for account: {0}";
        public static readonly string AccountCreated = "Successfully created account {0} (Role: {1}).";
        public static readonly string AccountCreationFailed = "Failed to create account {0}.";
        public static readonly string AccountNotFound = "Client attempted to log into non-existant account: {0}";
        public static readonly string AccountsCached = "Re-)Cached {0} Accounts.";
        public static readonly string AttemptedRequestForUnknownAccount = "A request for non-existant account '{0}' was made from {1} (Last Login from: {2})";
        public static readonly string AuthPacketParsed = "Packet '{0}' parsed successfully!";
        public static readonly string AuthServiceAlreadyListening = "IPC service address already bound to; make sure you aren't already running the Authentication server!";
        public static readonly string AutocreatingAccount = "Auto-creating account: {0}";
        public static readonly string CachingAccounts = "Caching Accounts...";
        public static readonly string CannotRetrieveAuthenticationInfo = "Could not retrieve Authentication info for account '{0}'!";
        public static readonly string ChallengeFailed = "Account Challenge failed";
        public static readonly string ClientConnected = "Client connected to the server. Address: {0}";
        public static readonly string ClientDisconnected = "Client disconnected from the server. Address: {0}";
        public static readonly string ClientNull = "Client cannot be null!";
        public static readonly string DatabaseFailure = "There was a fatal database error, server not started";
        public static readonly string HandlerAlreadyRegistered = "The PacketHandler for Packet {0} '{1}' has been overridden with '{2}'!";
        public static readonly string InvalidAuthPacket = "Invalid authentication packet!";
        public static readonly string InvalidClientProof = "Client-supplied proof of Account {0} did not match server-generated proof.";
        public static readonly string InvalidEmailAddress = "Invalid email address: {0}";
        public static readonly string InvalidHandlerMethodSignature = "Cannot create packet handler delegate from method '{0}': invalid method signature!";
        public static readonly string IPCServiceFailed = "IPC Service failed - Restarting in '{0}' seconds...";
        public static readonly string IPCServiceShutdown = "IPC Service stopped successfully.";
        public static readonly string IPCServiceStarted = "IPC Service started successfully - Listening on {0}";
        public static readonly string PacketHandleException = "An exception occured while trying to handle packet! Packet ID: '{0}'";
        public static readonly string PacketParseFailed = "Failed to parse packet properly! Given opcode: {0:X4}";
        public static readonly string PrivilegeConfigChanged = "Privilege configuration changed; reloading!";
        public static readonly string RealmDisconnected = "Realm disconnected: {0}";
        public static readonly string RealmRegistered = "New Realm registered: {0}";
        public static readonly string RealmUnregistered = "Realm unregistered: {0}";
        public static readonly string RealmUpdated = "Realm update from {0}";
        public static readonly string RegisteredAllHandlers = "Registered all packet handlers!";
        public static readonly string RegisteredHandler = "Registered handler for '{0}' for {1}";
        public static readonly string StartInitException = "Exception occured when starting/initializing: {0}";
        public static readonly string StopCleanup = "Stopping/cleaning up: {0}";
        public static readonly string StopCleanupException = "Exception occured when stopping/cleaning up: {0}";
        public static readonly string UDPNotImplemented = "UDP messages are not part of the protocol";
        public static readonly string UnhandledPacket = "Unhandled packet {0} ({1}), Size: {2} bytes";
        public static readonly string UnknownIPCLogin = "Unknown username or password given for IPC authentication! Username: '{0}'  Password: '{1}'";
        public static readonly string ListeningTCPSocket = "Listening to TCP socket on: {0}";
        public static readonly string NoNetworkAdapters = "No network adapters are available on the system";
        public static readonly string PathCannotBeNull = "Path cannot be be null or empty";
        public static readonly string ReadyForConnections = "Server is ready for connections";
        public static readonly string ServerNotRunning = "Server is no longer accepting connections";
        public static readonly string SocketExceptionAsyncAccept = "Encountered a socket exception while trying to accept a connection";
        public static readonly string FatalAsyncAccept = "tered a fatal error while trying to accept a connection. You might have to restart the server.";
    }
}

