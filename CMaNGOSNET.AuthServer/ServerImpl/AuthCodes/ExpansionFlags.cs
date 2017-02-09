using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.AuthServer.ServerImpl.AuthCodes
{
    public enum ExpansionFlags
    {
        POST_BC_EXP_FLAG = 0x2,
        PRE_BC_EXP_FLAG = 0x1,
        NO_VALID_EXP_FLAG = 0x0
    }
}
