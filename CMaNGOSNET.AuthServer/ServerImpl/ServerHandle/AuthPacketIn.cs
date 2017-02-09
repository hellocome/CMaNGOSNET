using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Network;

namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle
{
    public class AuthPacketIn : PacketIn
    {
        // AuthCommand
        private const int headerSize = 1;

        public override int HeaderSize
        {
            get
            {
                return headerSize;
            }
        }

        public AuthPacketIn(byte[] array, int offset, int length)
            : base(array, offset, length)
        {

        }

        public bool GetCommand(out AuthCommand cmd)
        {
            cmd = AuthCommand.CMD_AUTH_LOGON_CHALLENGE;

            if (EnsureData(1))
            {
                byte cmdByte = ReadByte();

                if (Enum.IsDefined(typeof(AuthCommand), cmdByte))
                {
                    cmd = (AuthCommand)ReadByte();
                    return true;
                }
            }

            return false;
        }
    }
}
