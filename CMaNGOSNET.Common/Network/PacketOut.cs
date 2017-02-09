using System;
using System.IO;
using System.Text;

namespace CMaNGOSNET.Common.Network
{
    public abstract class PacketOut : PacketWritter
    {
        public abstract int HeaderSize { get; }

        public PacketOut(byte[] array, int offset, int length)
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

            set
            {
                BaseStream.SetLength(value + HeaderSize);
            }
        }

        public void Fill(byte val, int num)
        {
            for (int i = 0; i < num; ++i)
            {
                Write(val);
            }
        }

        public void FillZero(int len)
        {
            for (var i = 0; i < len; ++i)
            {
                Write((byte)0);
            }
        }

        protected virtual void FinalizeWrite()
        {
        }

        public byte[] GetFinalizedPacket()
        {
            FinalizeWrite();
            byte[] bytes = new byte[Length];
            BaseStream.Position = 0;
            BaseStream.Read(bytes, 0, Length);
            return bytes;
        }

        protected static void Reverse<T>(T[] buffer)
        {
            Reverse(buffer, buffer.Length);
        }

        protected static void Reverse<T>(T[] buffer, int length)
        {
            for (int i = 0; i < length / 2; i++)
            {
                T temp = buffer[i];
                buffer[i] = buffer[length - i - 1];
                buffer[length - i - 1] = temp;
            }
        }
    }
}
