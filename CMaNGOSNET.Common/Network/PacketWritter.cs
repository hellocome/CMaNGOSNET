using System;
using System.IO;
using System.Text;
using System.Numerics;

namespace CMaNGOSNET.Common.Network
{
    public class PacketWritter : BinaryWriter
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;

        public PacketWritter(byte[] array, int offset, int length) : 
            base(new MemoryStream(array, offset, length), DefaultEncoding)
        {
        }

        public virtual void WriteUInt32(UInt32 val)
        {
            Write(BitConverter.GetBytes(val));
        }

        public virtual void WriteUInt16(UInt16 val)
        {
            Write(BitConverter.GetBytes(val));
        }

        public virtual void WriteByte(byte val)
        {
            Write(val);
        }

        public virtual void WriteByte(int val)
        {
            Write((byte)val);
        }

        public virtual void WriteByte(uint val)
        {
            Write((byte)val);
        }

        public virtual void WriteByte(UInt16 val)
        {
            Write((byte)val);
        }

        public virtual void WriteByte(Int16 val)
        {
            Write((byte)val);
        }


        public virtual void WriteBytes(byte[] val)
        {
            Write(val);
        }

        public virtual void WriteBytes(BigInteger val)
        {
            Write(val.ToByteArray());
        }

        public virtual void WriteBytes(byte[] val, int index, int count)
        {
            Write(val, index, count);
        }
    }
}
