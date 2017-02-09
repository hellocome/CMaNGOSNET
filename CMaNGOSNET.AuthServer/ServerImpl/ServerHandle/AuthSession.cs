using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Data;
using System.Numerics;

using CMaNGOSNET.Common.Network;
using CMaNGOSNET.Common.Logging;
using CMaNGOSNET.Common.Security;
using CMaNGOSNET.Common.Collection;
using CMaNGOSNET.Common.Numerics;
using CMaNGOSNET.Common.Global;
using CMaNGOSNET.Common.Text;

using CMaNGOSNET.AuthServer.ServerImpl.ServerHandle.PackageObjects;
using CMaNGOSNET.AuthServer.ServerImpl.AuthCodes;
using CMaNGOSNET.AuthServer.Database;
using CMaNGOSNET.AuthServer.Accounts;


namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle
{
    public class AuthSession : MessageHandleManager
    {
        private string login;
        private ExpansionFlags expversion;
        private UInt16 build;

        private Dictionary<AuthCommand, AuthHandle> AuthHandlerTable = new Dictionary<AuthCommand, AuthHandle>();

        private AuthCommandStatus commandStatus;

        private SecureRemotePassword srp;
        private Account account;
        private string ip;
        private int port;
        private string country;
        private string os;

        public AuthSession()
        {
            commandStatus = AuthCommandStatus.STATUS_CHALLENGE;
            expversion = ExpansionFlags.NO_VALID_EXP_FLAG;

            login = string.Empty;
            srp = new SecureRemotePassword(new DefaultSRPSetting());

            RegisterHandle();
        }

 

        private void RegisterHandle()
        {
            AuthHandlerTable.Add(AuthCommand.CMD_AUTH_LOGON_CHALLENGE, new AuthHandle(AuthCommand.CMD_AUTH_LOGON_CHALLENGE, AuthCommandStatus.STATUS_CHALLENGE, new Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult>(HandleLogonChallenge)));
            AuthHandlerTable.Add(AuthCommand.CMD_AUTH_LOGON_PROOF, new AuthHandle(AuthCommand.CMD_AUTH_LOGON_PROOF, AuthCommandStatus.STATUS_LOGON_PROOF, new Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult>(HandleLogonProof)));
            AuthHandlerTable.Add(AuthCommand.CMD_AUTH_RECONNECT_CHALLENGE, new AuthHandle(AuthCommand.CMD_AUTH_RECONNECT_CHALLENGE, AuthCommandStatus.STATUS_CHALLENGE, new Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult>(HandleReconnectChallenge)));
            AuthHandlerTable.Add(AuthCommand.CMD_AUTH_RECONNECT_PROOF, new AuthHandle(AuthCommand.CMD_AUTH_RECONNECT_PROOF, AuthCommandStatus.STATUS_RECON_PROOF, new Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult>(HandleReconnectProof)));
            AuthHandlerTable.Add(AuthCommand.CMD_REALM_LIST, new AuthHandle(AuthCommand.CMD_REALM_LIST, AuthCommandStatus.STATUS_AUTHED, new Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult>(HandleRealmList)));
            AuthHandlerTable.Add(AuthCommand.CMD_XFER_ACCEPT, new AuthHandle(AuthCommand.CMD_XFER_ACCEPT, AuthCommandStatus.STATUS_PATCH, new Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult>(HandleXferAccept)));
            AuthHandlerTable.Add(AuthCommand.CMD_XFER_RESUME, new AuthHandle(AuthCommand.CMD_XFER_RESUME, AuthCommandStatus.STATUS_PATCH, new Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult>(HandleXferResume)));
            AuthHandlerTable.Add(AuthCommand.CMD_XFER_CANCEL, new AuthHandle(AuthCommand.CMD_XFER_CANCEL, AuthCommandStatus.STATUS_PATCH, new Func<SocketAsyncEventArgs, AuthPacketIn, PacketProcessResult>(HandleXferCancel)));
        }

        public override PacketProcessResult ProcessMessage(ITCPClient client, SocketAsyncEventArgs args)
        {
            try
            {
                AuthCommand authCmd;

                TCPClient = client;

                DataHoldingUserToken userToken = (DataHoldingUserToken)args.UserToken;

                AuthPacketIn packet = new AuthPacketIn(args.Buffer, userToken.ProcessedReceiveDataOffset, userToken.RemainingReceiveDataBufferLength);

                if(!packet.GetCommand(out authCmd))
                {
                    return PacketProcessResult.BAD_MESSAGE;
                }
                else
                {
                    AuthHandle handle = AuthHandlerTable[authCmd];

                    if(commandStatus != handle.Status)
                    {
                        return PacketProcessResult.INVALID_MESSAGE;
                    }

                    return handle.Handler(args, packet);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
                return PacketProcessResult.FATAL;
            }
        }

        public PacketProcessResult HandleLogonChallenge(SocketAsyncEventArgs args, AuthPacketIn packet)
        {
            commandStatus = AuthCommandStatus.STATUS_CLOSED;
            Logger.Instance.Debug("Entering HandleLogonChallenge");

            DataHoldingUserToken userToken = (DataHoldingUserToken)args.UserToken;
            AuthPacketOut packetOut = new AuthPacketOut(args.Buffer, userToken.SendOffset, userToken.SendSize);
            AUTH_LOGON_CHALLENGE_S challengeResponse = new AUTH_LOGON_CHALLENGE_S(packetOut);

            AUTH_LOGON_CHALLENGE_C challenge = new AUTH_LOGON_CHALLENGE_C(packet);

            if (!challenge.InitPacketObject())
            {
                return PacketProcessResult.BAD_MESSAGE;
            }

            login = challenge.I;
            build = challenge.Build;
            expversion = (AuthHelper.IsPostBCAcceptedClientBuild(challenge.Build) ? ExpansionFlags.POST_BC_EXP_FLAG : (
                AuthHelper.IsPreBCAcceptedClientBuild(challenge.Build) ? ExpansionFlags.PRE_BC_EXP_FLAG : ExpansionFlags.NO_VALID_EXP_FLAG));


            LoginManager.Instance.ExecuteLoginDatabaseStatement(LoginDatabaseStatements.LOGIN_DEL_EXPIRED_IP_BANS);

            ip = NetworkUtility.GetRemoteIpAddress(args);
            port = NetworkUtility.GetRemotePort(args);
            country = LocaleName.GetLocaleNameFromByMessageByteName(challenge.Country);
            os = DataEncoder.Ascii2HexString(challenge.OS);

            IDataReader reader = LoginManager.Instance.QueryLoginDatabaseStatementSync(LoginDatabaseStatements.LOGIN_SEL_IP_BANNED);

            if (reader.Read())
            {
                challengeResponse.error = (byte)AuthResult.WOW_FAIL_BANNED;
                Logger.Instance.Info("{0}:{1} [AuthChallenge] Banned ip tries to login!", ip, port);
            }
            else
            {
                if (string.IsNullOrEmpty(login))
                {
                    challengeResponse.error = (byte)AuthResult.WOW_FAIL_UNKNOWN_ACCOUNT;
                    Logger.Instance.Info("{0}:{1} [AuthChallenge] Empty account tries to login!", ip, port);
                }
                else
                {
                    account = LoginManager.Instance.QueryLoginAccount(login);

                    if (!account.Valid)
                    {
                        challengeResponse.error = (byte)AuthResult.WOW_FAIL_UNKNOWN_ACCOUNT;
                        Logger.Instance.Info("{0}:{1} [AuthChallenge] Unknown account tries to login!", ip, port);
                    }
                    else
                    {
                        bool locked = false;
                        if (!locked)
                        {
                            if (locked = account.IsLockedToIP(ip))
                            {
                                challengeResponse.error = (byte)AuthResult.WOW_FAIL_LOCKED_ENFORCED;
                            }
                        }

                        if (!locked)
                        {
                            if (locked = account.IsLockedToCountry(ip))
                            {
                                challengeResponse.error = (byte)AuthResult.WOW_FAIL_UNLOCKABLE_LOCK;
                            }
                        }

                        if (!locked)
                        {
                            LoginManager.Instance.ExecuteLoginDatabaseStatement(LoginDatabaseStatements.LOGIN_UPD_EXPIRED_ACCOUNT_BANS);
                            AccountBanState banState = account.GetAccountBanState();

                            if (banState == AccountBanState.BANNED)
                            {
                                challengeResponse.error = (byte)AuthResult.WOW_FAIL_BANNED;
                            }
                            else if (banState == AccountBanState.SUSPENDED)
                            {
                                challengeResponse.error = (byte)AuthResult.WOW_FAIL_SUSPENDED;
                            }
                            else
                            {
                                
                                bool validVS = true;
                                if (!account.ValidV || !account.ValidS)
                                {
                                    if (!account.UpdateVSField())
                                    {
                                        validVS = false;
                                    }
                                }

                                if (!validVS)
                                {
                                    challengeResponse.error = (byte)AuthResult.WOW_FAIL_UNKNOWN_ACCOUNT;

                                }
                                else
                                {
                                    BigInteger B = srp.CalculateB(account.V);

                                    if (AuthHelper.IsAcceptedClientBuild(build))
                                        challengeResponse.error = (byte)AuthResult.WOW_SUCCESS;
                                    else
                                        challengeResponse.error = (byte)AuthResult.WOW_FAIL_VERSION_INVALID;

                                    challengeResponse.B = B.ToByteArray().SubArray(32);
                                    challengeResponse.G = srp.G.ToByteArray()[0];
                                    challengeResponse.N = srp.N.ToByteArray().SubArray(32);
                                    challengeResponse.S = account.S.ToByteArray().SubArray(32);


                                    if (!string.IsNullOrEmpty(account.TokenKey))
                                    {
                                        challengeResponse.SecurityFlags = 4;
                                    }

                                    ///- All good, await client's proof
                                    commandStatus = AuthCommandStatus.STATUS_LOGON_PROOF;
                                }
                            }
                        }
                    }
                }
            }

            challengeResponse.Build();

            SendPacket(challengeResponse.Packet);

            return PacketProcessResult.SUCCESSFUL;
        }

        public PacketProcessResult HandleLogonProof(SocketAsyncEventArgs args, AuthPacketIn packet)
        {
            commandStatus = AuthCommandStatus.STATUS_CLOSED;

            AUTH_LOGON_PROOF_C logonProof = new AUTH_LOGON_PROOF_C(packet);

            DataHoldingUserToken userToken = (DataHoldingUserToken)args.UserToken;
            AuthPacketOut packetOut = new AuthPacketOut(args.Buffer, userToken.SendOffset, userToken.SendSize);
            AUTH_LOGON_PROOF_S logonProofResponse = new AUTH_LOGON_PROOF_S(packetOut);

            if (expversion == ExpansionFlags.NO_VALID_EXP_FLAG)
            {
                // Check if we have the appropriate patch on the disk
                Logger.Instance.Debug("network", "Client with invalid version, patching is not implemented");
                return PacketProcessResult.BAD_MESSAGE;
            }

            BigInteger A = new BigInteger(logonProof.A);

            if(A.IsZero || srp.B.IsZero)
            {
                return PacketProcessResult.BAD_MESSAGE;
            }
            
            BigInteger m = srp.ComputeM(login, A, account.V, account.S);

            if(m.Equals(logonProof.M1))
            {
                account.UpdateUPDLogonProof(srp.K.ToHexStr(), ip, port, country, os);

                BigInteger clientProofM = srp.ComputeClientProofM(A);

                if (logonProof.TokenKey != null)
                {
                     // to do, we assume the token is empty and pass!!!
                     //
                }


                logonProofResponse.error = (byte)AuthResult.WOW_SUCCESS;
                logonProofResponse.Build();
                SendPacket(logonProofResponse.Packet);

                commandStatus = AuthCommandStatus.STATUS_AUTHED;
            }
            else
            {
                logonProofResponse.error = (byte)AuthResult.WOW_FAIL_UNKNOWN_ACCOUNT;
                logonProofResponse.Build();
                SendPacket(logonProofResponse.Packet);

                
                // To do
                /*Login failed!!!!!!!!!!, update database*/
            }

            return PacketProcessResult.SUCCESSFUL;
        }

        public PacketProcessResult HandleReconnectChallenge(SocketAsyncEventArgs args, AuthPacketIn packet)
        {
            Logger.Instance.Debug("Entering HandleLogonChallenge");
            commandStatus = AuthCommandStatus.STATUS_CLOSED;

            DataHoldingUserToken userToken = (DataHoldingUserToken)args.UserToken;
            AuthPacketOut packetOut = new AuthPacketOut(args.Buffer, userToken.SendOffset, userToken.SendSize);

            AUTH_LOGON_CHALLENGE_C challenge = new AUTH_LOGON_CHALLENGE_C(packet);

            if (!challenge.InitPacketObject())
            {
                return PacketProcessResult.BAD_MESSAGE;
            }

            login = challenge.I;
            byte gmLevel;
            string sessionKey;
            if(!LoginManager.Instance.QuerySessionKey(login, out sessionKey, out gmLevel))
            {
                return PacketProcessResult.INVALID_MESSAGE;
            }
            
            build = challenge.Build;
            expversion = (AuthHelper.IsPostBCAcceptedClientBuild(challenge.Build) ? ExpansionFlags.POST_BC_EXP_FLAG : (
                AuthHelper.IsPreBCAcceptedClientBuild(challenge.Build) ? ExpansionFlags.PRE_BC_EXP_FLAG : ExpansionFlags.NO_VALID_EXP_FLAG));

            LoginManager.Instance.ExecuteLoginDatabaseStatement(LoginDatabaseStatements.LOGIN_DEL_EXPIRED_IP_BANS);

            ip = NetworkUtility.GetRemoteIpAddress(args);
            port = NetworkUtility.GetRemotePort(args);
            country = LocaleName.GetLocaleNameFromByMessageByteName(challenge.Country);
            os = DataEncoder.Ascii2HexString(challenge.OS);

            srp.UpdateK(sessionKey);
            account.UpdateGMLevel(gmLevel);
            BigInteger reconnectProof = srp.UpdateReconnectProof();

            packetOut.WriteByte((byte)AuthCommand.CMD_AUTH_LOGON_PROOF);
            packetOut.WriteByte((byte)0x00);
            packetOut.WriteBytes(reconnectProof);
            packetOut.WriteBytes(CryptoUtility.GenerateArrayFillWith<byte>(0, 16));

            SendPacket(packetOut);

            commandStatus = AuthCommandStatus.STATUS_RECON_PROOF;

            return PacketProcessResult.SUCCESSFUL;
        }

        public PacketProcessResult HandleReconnectProof(SocketAsyncEventArgs args, AuthPacketIn packet)
        {
            commandStatus = AuthCommandStatus.STATUS_CLOSED;

            DataHoldingUserToken userToken = (DataHoldingUserToken)args.UserToken;
            AuthPacketOut packetOut = new AuthPacketOut(args.Buffer, userToken.SendOffset, userToken.SendSize);

            AUTH_RECONNECT_PROOF_C challenge = new AUTH_RECONNECT_PROOF_C(packet);

            if (!challenge.InitPacketObject() || srp.ReconnectProof.IsZero || string.IsNullOrEmpty(login))
            {
                return PacketProcessResult.BAD_MESSAGE;
            }
            

            if (srp.ValidateReconnectProof(login, challenge.R1, challenge.R2))
            {
                packetOut.WriteByte((byte)AuthCommand.CMD_AUTH_RECONNECT_PROOF);
                packetOut.WriteByte((byte)0x00);
                packetOut.WriteBytes(CryptoUtility.GenerateArrayFillWith<byte>(0, 2));
                SendPacket(packetOut);

                commandStatus = AuthCommandStatus.STATUS_AUTHED;
                return PacketProcessResult.SUCCESSFUL;
            }
            else
            {
                return PacketProcessResult.BAD_MESSAGE;
            }
        }

        public PacketProcessResult HandleRealmList(SocketAsyncEventArgs args, AuthPacketIn packet)
        {
            int id;
            commandStatus = AuthCommandStatus.STATUS_CLOSED;

            if (!LoginManager.Instance.QueryAccoutId(login, out id))
            {
                Logger.Instance.Error("'{0}:{1}' [ERROR] user {2} tried to login but we cannot find him in the database.", ip, port, login);
                return PacketProcessResult.BAD_MESSAGE;
            }

            return PacketProcessResult.SUCCESSFUL;
        }

        public PacketProcessResult HandleXferAccept(SocketAsyncEventArgs args, AuthPacketIn packet)
        {
            return PacketProcessResult.SUCCESSFUL;
        }

        public PacketProcessResult HandleXferResume(SocketAsyncEventArgs args, AuthPacketIn packet)
        {
            return PacketProcessResult.SUCCESSFUL;
        }

        public PacketProcessResult HandleXferCancel(SocketAsyncEventArgs args, AuthPacketIn packet)
        {
            return PacketProcessResult.SUCCESSFUL;
        }
    }
}
