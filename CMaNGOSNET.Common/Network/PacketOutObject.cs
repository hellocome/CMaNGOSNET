using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.Common.Network
{
    public abstract class PacketOutObject
    {
        public PacketOut Packet
        {
            get;
            protected set;
        }

        public abstract bool InitPacketObject();
        public abstract void Build();
    }
}
