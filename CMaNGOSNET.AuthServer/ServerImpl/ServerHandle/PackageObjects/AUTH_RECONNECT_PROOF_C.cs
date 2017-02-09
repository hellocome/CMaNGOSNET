using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Network;


namespace CMaNGOSNET.AuthServer.ServerImpl.ServerHandle.PackageObjects
{
    public class AUTH_RECONNECT_PROOF_C : PacketInObject
    {
        public const int REQUIRED_SIZE = 58;

        public byte Cmd;
        public BigInteger R1; // 16
        public BigInteger R2; // 20
        public BigInteger R3; // 20
        public byte number_of_keys;

        public AUTH_RECONNECT_PROOF_C(PacketIn packet)
        {
            PacketRequiredSize = REQUIRED_SIZE;
            Packet = packet;
        }

        protected override bool ReadPackage()
        {
            Cmd = Packet.ReadByte();
            R1 = new BigInteger(Packet.ReadBytes(16));
            R2 = new BigInteger(Packet.ReadBytes(20));
            R3 = new BigInteger(Packet.ReadBytes(20));

            number_of_keys = Packet.ReadByte();

            return true;
        }
    }
}
