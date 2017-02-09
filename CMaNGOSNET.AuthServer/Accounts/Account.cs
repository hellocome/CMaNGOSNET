using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Numerics;
using System.ComponentModel.DataAnnotations;
using CMaNGOSNET.Common.Logging;
using CMaNGOSNET.AuthServer.Database;
using CMaNGOSNET.Common.Network;
using CMaNGOSNET.Common.Security;
using CMaNGOSNET.Common.Numerics;

namespace CMaNGOSNET.AuthServer.Accounts
{
    // SELECT a.sha_pass_hash, a.id, a.locked, a.lock_country, a.last_ip, aa.gmlevel, a.v, a.s, a.token_key FROM account
    // a LEFT JOIN account_access aa ON (a.id = aa.id) WHERE a.username = ?", CONNECTION_SYNCH);
    public class Account
    {
        public Account()
        {
            V = 0;
            S = 0;
            SessionKey = 0;
        }

        public AccountTypes AccountSecurityLevel
        {
            get;
            private set;
        }

        #region Props
        private bool valid = false;
        public bool Valid
        {
            get
            {
                return valid;
            }

            set
            {
                valid = value;
            }
        }

        public AccountAccess AccountAccess
        {
            get;
            set;
        }

        public int AccountId
        {
            get;
            set;
        }


        [MaxLength(32)]
        public string Username
        {
            get;
            set;
        }

        [MaxLength(40)]
        public string SHAPassHash
        {
            get;
            set;
        }
        
        public BigInteger SessionKey
        {
            get;
            set;
        }
        
        public BigInteger V
        {
            get;
            set;
        }

        public BigInteger S
        {
            get;
            set;
        }

        public bool ValidV
        {
            get;
            set;
        }

        public bool ValidS
        {
            get;
            set;
        }

        [MaxLength(100)]
        public string TokenKey
        {
            get;
            set;
        }


        public DateTime Joindate
        {
            get;
            set;
        }

        [MaxLength(255)]
        public string EMail
        {
            get;
            set;
        }

        [MaxLength(255)]
        public string RegEMail
        {
            get;
            set;
        }


        [MaxLength(15)]
        public string LastIP
        {
            get;
            set;
        }

        [MaxLength(15)]
        public string LastAttemptIP
        {
            get;
            set;
        }

        public int FailedLogins
        {
            get;
            set;
        }


        public byte Locked
        {
            get;
            set;
        }

        [MaxLength(2)]
        public string LockCountry
        {
            get;
            set;
        }


        public DateTime LastLogin
        {
            get;
            set;
        }

        public byte Online
        {
            get;
            set;
        }

        public byte Expansion
        {
            get;
            set;
        }


        public Int64 MuteTime
        {
            get;
            set;
        }

        [MaxLength(255)]
        public string MuteReason
        {
            get;
            set;
        }

        [MaxLength(50)]
        public string MuteBy
        {
            get;
            set;
        }

        public byte Locale
        {
            get;
            set;
        }

        [MaxLength(3)]
        public string OS
        {
            get;
            set;
        }

        public int Recruiter
        {
            get;
            set;
        }

        public string IP
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public string Country
        {
            get;
            set;
        }
        #endregion


        public bool IsLockedToIP(string currentLoginIP)
        {
            if (Locked == 1)
            {
                Logger.Instance.Debug("[AuthChallenge] Player address is {0}", currentLoginIP);

                if (string.IsNullOrEmpty(LastIP))
                {
                    Logger.Instance.Debug("[AuthChallenge] Account {0} is locked to IP - {1}", Username, LastIP);

                    if (!LastIP.Equals(currentLoginIP))
                    {
                        Logger.Instance.Debug("[AuthChallenge] Account IP differs - Lock account");
                        return true;
                    }
                }

                Logger.Instance.Debug("[AuthChallenge] Account IP matches");
            }

            return false;
        }

        public bool IsLockedToCountry(string currentLoginIP)
        {
            if(string.IsNullOrEmpty(LockCountry) || LockCountry.Equals("00"))
            {
                Logger.Instance.Debug("[AuthChallenge] Account {0} is not locked to country", Username);
            }
            else if(!string.IsNullOrEmpty(LockCountry))
            {
                int ip = NetworkUtility.IPToInt(currentLoginIP);
                IDataReader reader = LoginManager.Instance.QueryLoginDatabaseStatementSync(LoginDatabaseStatements.LOGIN_SEL_LOGON_COUNTRY, ip);

                if(reader.Read())
                {
                    string loginCountry = reader.GetString(0);

                    if (!LockCountry.Equals(loginCountry))
                    {
                        Logger.Instance.Debug("[AuthChallenge] Account country differs.");
                        return true;
                    }
                }
            }

            return false;
        }

        public AccountBanState GetAccountBanState()
        {
            IDataReader reader = LoginManager.Instance.QueryLoginDatabaseStatementSync(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_BANNED, AccountId);

            if (reader.Read())
            {
                int bandate = reader.GetInt32(0);
                int unbandate = reader.GetInt32(0);

                if(bandate == unbandate)
                {
                    return AccountBanState.BANNED;
                }
                else
                {
                    return AccountBanState.SUSPENDED;
                }
            }

            return AccountBanState.OK;
        }

        public bool UpdateVSField()
        {
            string v, s;
            SecureRemotePassword srp = new SecureRemotePassword();
            
            if(srp.SetVSFields(SHAPassHash, out v, out s))
            {
                ValidV = V.TrySetHexStr(v);
                ValidS = S.TrySetHexStr(s);

                if(ValidV && ValidS)
                {
                    LoginManager.Instance.ExecuteLoginDatabaseStatement(LoginDatabaseStatements.LOGIN_UPD_VS, v, s);
                    return true;
                }
            }

            return false;
        }


        public void UpdateUPDLogonProof(string kHexString, string ip, int port, string country, string os)
        {
            this.IP = ip;
            this.Port = port;
            this.Country = country;
            this.Locale = (byte)CMaNGOSNET.Common.Global.LocaleName.GetLocaleByName(country);
            this.OS = os;

            LoginManager.Instance.ExecuteLoginDatabaseStatement(LoginDatabaseStatements.LOGIN_UPD_LOGONPROOF,
                kHexString,
                IP,
                Port,
                OS,
                Username,
                Locale
                );

            Logger.Instance.Debug("[AuthChallenge] account {0} from {1}:{2} is using '{3}' locale (4)", Username, IP, Port, Country, Locale);
        }


        public void UpdateGMLevel(byte gmLevel)
        {
            if (this.AccountAccess != null)
            {
                this.AccountAccess.GMLevel = (AccountTypes)gmLevel;
            }
        }
    }
}
