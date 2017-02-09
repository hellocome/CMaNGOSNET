using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Network;
using CMaNGOSNET.AuthServer.ServerImpl.AuthCodes;
using CMaNGOSNET.Common.Security;

namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle.PackageObjects
{
    public class RealmList_S : PacketOutObject
    {
        public override bool InitPacketObject()
        {
            Packet.WriteByte((byte)AuthCommand.CMD_AUTH_LOGON_CHALLENGE);

            return true;
        }

        public override void Build()
        {

        }
    }
}
