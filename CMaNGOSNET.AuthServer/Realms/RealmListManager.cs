using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data;
using System.Timers;
using CMaNGOSNET.Common.Logging;
using CMaNGOSNET.AuthServer.Database;
using CMaNGOSNET.AuthServer.Accounts;

namespace CMaNGOSNET.AuthServer.Realms
{
    
    // Storage object for a realm
    public class RealmListManager
    {
        private static RealmListManager instance = new RealmListManager();
        public static RealmListManager Instance
        {
            get
            {
                return instance;
            }
        }

        private Timer timer = new Timer();
        private List<Realm> realmList = new List<Realm>();

        public List<Realm> RealmList
        {
            get
            {
                return realmList;
            }
        }

        public int UpdateInterval
        {
            get;
            set;
        }

        public RealmListManager()
        {
            UpdateInterval = 20 * 1000;

            timer.Interval = UpdateInterval;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateRealms();
        }

        public void UpdateRealms()
        {
            Logger.Instance.Debug("Updating Realm List...");

            lock (realmList)
            {
                realmList.Clear();
                IDataReader reader = LoginManager.Instance.QueryLoginDatabaseStatementSync(LoginDatabaseStatements.LOGIN_SEL_REALMLIST);

                while (reader.Read())
                {
                    Realm realm = new Realm();

                    realm.ID = (UInt32)reader.GetInt32(0);
                    realm.Name = reader.GetString(1);
                    realm.ExternalAddress = IPAddress.Parse(reader.GetString(2));
                    realm.LocalAddress = IPAddress.Parse(reader.GetString(3));
                    realm.LocalSubnetMask = IPAddress.Parse(reader.GetString(4));

                    realm.Port = (UInt16)reader.GetInt16(5);
                    realm.Icon = reader.GetByte(6);
                    realm.Flag = (RealmFlags)reader.GetByte(7);
                    realm.Timezone = reader.GetByte(8);
                    realm.AllowedSecurityLevel = (AccountTypes)reader.GetByte(9);
                    realm.PopulationLevel = reader.GetFloat(10);
                    realm.Gamebuild = (UInt32)reader.GetInt32(11);

                    realmList.Add(realm);
                }
            }
        }

    }
}
