using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Logging;

namespace CMaNGOSNET.Common.Network
{
    public abstract class PacketIn : PacketReader
    {
        public abstract int HeaderSize { get; }

        public PacketIn(byte[] array, int offset, int length)
            : base(array, offset, length)
        {

        }

        public int Position
        {
            get { return (int)BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public int Length
        {
            get { return (int)BaseStream.Length; }
        }

        public int ContentLength
        {
            get { return (int)BaseStream.Length - HeaderSize; }
        }

        public int RemainingLength
        {
            get { return (int)(BaseStream.Length - BaseStream.Position); }
        }

        protected bool EnsureData(int length)
        {
            if (Length - Position < length)
            {
                Logger.Instance.Warn("Not enough data available - Available: {0}, Required: {1}", Length - Position, length);
                return false;
            }

            return true;
        }

        public UInt16 ReadUInt16BigEndian()
        {
            return (UInt16)((ReadByte() << 8) | ReadByte());
        }

        public UInt32 ReadUInt32BigEndian()
        {
            return (UInt32)((ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte());
        }
        
    }
}
