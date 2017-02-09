using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Network;

namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle.PackageObjects
{
    public class AUTH_LOGON_CHALLENGE_C : PacketInObject
    {
        public const int REQUIRED_SIZE = 35;

        public byte Cmd;
        public byte Error;
        public UInt16 Size;
        public byte[] Gamename; // 4 bytes
        public byte Version1;
        public byte Version2;
        public byte Version3;
        public UInt16 Build;
        public byte[] Platform; // 4 bytes
        public byte[] OS;       // 4 bytes
        public byte[] Country;  // 4 bytes
        public UInt32 Timezone_bias;
        public UInt32 IP;
        public byte I_len;
        public string I;

        public AUTH_LOGON_CHALLENGE_C(PacketIn packet)
        {
            PacketRequiredSize = REQUIRED_SIZE;
            Packet = packet;
        }

        protected override bool ReadPackage()
        {
            Cmd = Packet.ReadByte();
            Error = Packet.ReadByte();
            Size = Packet.ReadUInt16();

            if ((Packet.RemainingLength < Size) || (Size < REQUIRED_SIZE - 4))
                return false;

            Gamename = Packet.ReadBytes(4);
            Version1 = Packet.ReadByte();
            Version2 = Packet.ReadByte();
            Version3 = Packet.ReadByte();
            Build = Packet.ReadUInt16();
            Platform = Packet.ReadBytes(4);
            OS = Packet.ReadBytes(4);
            Country = Packet.ReadBytes(4);

            Timezone_bias = Packet.ReadUInt32();
            IP = Packet.ReadUInt32();

            I_len = Packet.ReadByte();
            I = new string(Packet.ReadChars((int)I_len));

            return true;
        }
    }
}
