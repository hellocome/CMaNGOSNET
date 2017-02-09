using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.Common.Network
{
    public abstract class PacketInObject
    {
        public PacketIn Packet
        {
            get;
            protected set;
        }

        public int PacketRequiredSize
        {
            get;
            protected set;
        }

        protected abstract bool ReadPackage();

        public bool InitPacketObject()
        {
            if (Packet.ContentLength < PacketRequiredSize)
            {
                return false;
            }

            return ReadPackage();
        }
    }
}
