using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.Common.Database
{
    public enum ConnectionFlags
    {
        CONNECTION_ASYNC = 0x1,
        CONNECTION_SYNCH = 0x2,
        CONNECTION_BOTH = CONNECTION_ASYNC | CONNECTION_SYNCH
    };
}
