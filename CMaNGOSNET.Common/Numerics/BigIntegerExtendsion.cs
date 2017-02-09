using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Text;

namespace CMaNGOSNET.Common.Numerics
{
    public static class BigIntegerExtendsion
    {
        //***********************************************************************
        // Modulo Exponentiation
        //***********************************************************************
        public static BigInteger ModPow(this BigInteger a, BigInteger e, BigInteger m)
        {
            return BigInteger.ModPow(a, e, m);
        }

        public static bool TrySetHexStr(this BigInteger a, string str)
        {
            try
            {
                a = BigInteger.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ToHexStr(this BigInteger a)
        {
            return DataEncoder.Ascii2HexString(a.ToByteArray());
        }
    }
}
