using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Database;
using CMaNGOSNET.Common.Logging;
using CMaNGOSNET.AuthServer.Database.MySQL;

namespace CMaNGOSNET.AuthServer.Database
{
    public class LoginManager 
    {
        private static ILoginDatabase instance = null;

        private static string connectionString = "";
        

        public static ILoginDatabase Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new MySQLLoginDatabase(connectionString);
                }

                return instance;
            }
        }

        private LoginManager()
        {

        }

        public bool Initialize()
        {
            Logger.Instance.Info("Initializing LoginManager");

            Logger.Instance.Info("LoginManager is Initialized");

            return true;

        }
    }
}
