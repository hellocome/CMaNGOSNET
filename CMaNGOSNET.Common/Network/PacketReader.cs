using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace CMaNGOSNET.Common.Network
{
    public class PacketReader : BinaryReader
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;

        public PacketReader(byte[] array, int offset, int length)
            : base(new MemoryStream(array, offset, length), DefaultEncoding)
        {

        }
    }
}
