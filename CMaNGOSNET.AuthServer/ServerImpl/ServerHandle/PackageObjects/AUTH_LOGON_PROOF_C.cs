using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Network;

namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle.PackageObjects
{
    public class AUTH_LOGON_PROOF_C : PacketInObject
    {
        public const int REQUIRED_SIZE = 75;

        public byte Cmd;
        public byte[] A; //32
        public byte[] M1; //20
        public byte[] Crc_hash; // 20
        public byte Number_of_keys;
        public byte SecurityFlags;

        public byte TokenLength;
        public byte[] TokenKey;

        public AUTH_LOGON_PROOF_C(PacketIn packet)
        {
            PacketRequiredSize = REQUIRED_SIZE;
            Packet = packet;
        }

        protected override bool ReadPackage()
        {
            Cmd = Packet.ReadByte();
            A = Packet.ReadBytes(32);
            M1 = Packet.ReadBytes(20);
            Crc_hash = Packet.ReadBytes(20);

            Number_of_keys = Packet.ReadByte();
            SecurityFlags = Packet.ReadByte();

            if ((SecurityFlags & 0x04) > 0)
            {
                TokenLength = Packet.ReadByte();

                if (TokenLength > 0)
                {
                    TokenKey = Packet.ReadBytes(TokenLength);
                }
            }

            return true;
        }
    }
}
