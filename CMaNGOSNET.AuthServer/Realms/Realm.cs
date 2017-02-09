using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CMaNGOSNET.AuthServer.Accounts;

namespace CMaNGOSNET.AuthServer.Realms
{
    public class Realm
    {
        public IPAddress ExternalAddress
        {
            get;
            set;
        }

        public IPAddress LocalAddress
        {
            get;
            set;
        }
        public IPAddress LocalSubnetMask
        {
            get;
            set;
        }
        public UInt32 Port
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public byte Icon
        {
            get;
            set;
        }

        public RealmFlags Flag
        {
            get;
            set;
        }

        public byte Timezone
        {
            get;
            set;
        }
        public UInt32 ID
        {
            get;
            set;
        }
        public AccountTypes AllowedSecurityLevel
        {
            get;
            set;
        }
        public float PopulationLevel
        {
            get;
            set;
        }

        public UInt32 Gamebuild
        {
            get;
            set;
        }
    }
}
