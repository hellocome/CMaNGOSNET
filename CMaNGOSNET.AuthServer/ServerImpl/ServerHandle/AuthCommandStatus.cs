using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle
{
    public enum AuthCommandStatus
    {
        STATUS_CHALLENGE,
        STATUS_LOGON_PROOF,
        STATUS_RECON_PROOF,
        STATUS_PATCH,      // unused in CMaNGOSNET
        STATUS_AUTHED,
        STATUS_CLOSED
    }
}
