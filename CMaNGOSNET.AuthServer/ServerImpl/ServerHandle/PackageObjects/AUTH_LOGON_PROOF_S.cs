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
    class AUTH_LOGON_PROOF_S : PacketOutObject
    {
        public const int REQUIRED_SIZE = 32;

        public byte cmd;
        public byte error;
        public byte[] M2; // 20
        public UInt32 AccountFlags;
        public UInt32 SurveyId;
        public UInt16 unk3;

        public AUTH_LOGON_PROOF_S(PacketOut package)
        {
            Packet = package;
        }

        public override bool InitPacketObject()
        {
            cmd = (byte)AuthCommand.CMD_AUTH_LOGON_PROOF;
            return true;
        }

        public override void Build()
        {
            Packet.WriteByte(cmd);

            if (error == (byte)AuthResult.WOW_SUCCESS)
            {
                AccountFlags = 0x00800000;
                SurveyId = 0;
                unk3 = 0;
                
                Packet.WriteByte(error);
                Packet.WriteBytes(M2);
                Packet.WriteUInt32(AccountFlags);
                Packet.WriteUInt32(SurveyId);
                Packet.WriteUInt16(unk3);
            }
            else
            {
                Packet.WriteByte(error);
                Packet.WriteByte(3);
                Packet.WriteByte(0);
            }
        }
    }
}
