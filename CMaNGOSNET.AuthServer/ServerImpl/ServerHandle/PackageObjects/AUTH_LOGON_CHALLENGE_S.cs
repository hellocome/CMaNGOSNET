using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Network;
using CMaNGOSNET.AuthServer.ServerImpl.AuthCodes;
using CMaNGOSNET.Common.Security;

namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle.PackageObjects
{
    /*
        uint8   cmd;
        uint8   unk2;
        uint8   error;
        uint8   B[32];
        uint8   g_len;
        uint8   g;
        uint8   N_len;
        uint8   N[32];
        uint8   s[32];
        uint8   unk3[16];
        uint8   unk4;
         */
    public class AUTH_LOGON_CHALLENGE_S : PacketOutObject
    {
        public const int REQUIRED_SIZE = 35;

        public byte cmd;
        public readonly byte unk2 = 0x0;
        public byte error;
        public byte[] B;
        public readonly byte G_len = 1;
        public byte G;
        public readonly byte N_len = 32;
        public byte[] N;
        public byte[] S;
        public byte[] unk3;
        public readonly byte unk4 = 0;


        public byte SecurityFlags = 0x0;

        public AUTH_LOGON_CHALLENGE_S(PacketOut package)
        {
            Packet = package;
        }

        public override bool InitPacketObject()
        {
            Packet.WriteByte((byte)AuthCommand.CMD_AUTH_LOGON_CHALLENGE);
            Packet.WriteByte(unk2);

            return true;
        }

        public override void Build()
        {
            if (error == (byte)AuthResult.WOW_SUCCESS)
            {
                unk3 = CryptoUtility.GenerateRandomByteArray(16);

                Packet.WriteBytes(B);
                Packet.WriteByte(G_len);
                Packet.WriteByte(G);
                Packet.WriteByte(N_len);
                Packet.WriteBytes(N);
                Packet.WriteBytes(S);
                Packet.WriteBytes(unk3);
                Packet.WriteByte(unk4);


                Packet.WriteByte(SecurityFlags);

                if ((SecurityFlags & 0x01) > 0)
                {
                    Packet.WriteByte(0);

                    Packet.WriteBytes(CryptoUtility.GenerateArrayFillWith<byte>(0x0, 16));
                }

                if ((SecurityFlags & 0x02) > 0)               // Matrix input
                {
                    Packet.WriteBytes(CryptoUtility.GenerateArrayFillWith<byte>(0x0, 12));
                }

                if ((SecurityFlags & 0x04) > 0)               // Matrix input
                {
                    Packet.WriteByte(1);
                }
            }
            else
            {
                Packet.WriteByte(error);
            }
        }
    }
}
