using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMaNGOSNET.AuthServer.Accounts
{
    public class AccountAccess
    {
        public Account Account
        {
            get;
            private set;
        }

        public AccountTypes GMLevel
        {
            get; set;
        }

        public List<int> RealmID
        {
            get; set;
        }

        internal AccountAccess(Account account)
        {
            Account = account;
            RealmID = new List<int>();
        }
    }
}
