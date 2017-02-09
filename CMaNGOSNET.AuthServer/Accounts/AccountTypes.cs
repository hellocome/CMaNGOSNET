using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.AuthServer.Accounts
{
    public enum AccountTypes : byte
    {
        SEC_PLAYER = 0,
        SEC_MODERATOR = 1,
        SEC_GAMEMASTER = 2,
        SEC_ADMINISTRATOR = 3,
        SEC_CONSOLE = 4                                  // must be always last in list, accounts must have less security level always also
    }
}
