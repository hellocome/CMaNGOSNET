using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.Common.Network
{
    public enum PacketProcessResult
    {
        SUCCESSFUL,
        INCOMPLETE_MESSAGE,
        BAD_MESSAGE,
        INVALID_MESSAGE,
        FATAL
    }
}
