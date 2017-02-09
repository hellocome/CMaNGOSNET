using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Network;

namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle
{
    public class AuthPacketOut : PacketOut
    {
        private const int headerSize = 1;

        public override int HeaderSize
        {
            get
            {
                return headerSize;
            }
        }

        public AuthPacketOut(byte[] array, int offset, int length)
            : base(array, offset, length)
        {

        }
    }
}
