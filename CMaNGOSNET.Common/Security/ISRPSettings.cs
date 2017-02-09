using System;
using System.IO;
using System.Security.Cryptography;
using System.Numerics;

namespace CMaNGOSNET.Common.Security
{
    public class HashDataBroker
    {
        public byte[] HashData
        {
            get;
            private set;
        }

        public HashDataBroker(byte[] hashData)
        {
            HashData = hashData;
        }

        public static implicit operator HashDataBroker(BigInteger bi)
        {
            return new HashDataBroker(bi.ToByteArray());
        }

        public static implicit operator HashDataBroker(byte[] bytes)
        {
            return new HashDataBroker(bytes);
        }

        public static implicit operator HashDataBroker(string str)
        {
            return new HashDataBroker(System.Text.Encoding.UTF8.GetBytes(str));
        }
    }


    public enum SRPVersion
    {
        SRP6,
        SRP6a,
    }


    public interface ISRPSetting
    {
        BigInteger N { get; }
        BigInteger G { get; }

        SRPVersion SRPVersion { get; }

        uint KeyLength { get; }

        BigInteger Hash(params HashDataBroker[] biParams);
       

    }


    public class DefaultSRPSetting : ISRPSetting
    {
        // Reversed
        // 894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7
        // B79B3E2A87823CAB8F5EBFBF8EB10108535006298B5BADBD5B53E1895E644B89
        private static BigInteger n;
        private static BigInteger g = new BigInteger(7);
        private static uint keyLength = 32;
        private static SRPVersion ver = SRPVersion.SRP6;

        static DefaultSRPSetting()
        {
            n = BigInteger.Parse("B79B3E2A87823CAB8F5EBFBF8EB10108535006298B5BADBD5B53E1895E644B89", System.Globalization.NumberStyles.HexNumber);
        }


        public SRPVersion SRPVersion
        {
            get
            {
                return ver;
            }
        }

        public BigInteger N
        {
            get
            {
                return n;
            }
        }

        public BigInteger G
        {
            get
            {
                return g;
            }
        }

        public uint KeyLength
        {
            get
            {
                return keyLength;
            }
        }
        

        public BigInteger Hash(params HashDataBroker[] biParams)
        {
            return ComputeSHAHash(biParams);
        }


        #region Internal Helper
        internal static BigInteger ComputeSHAHash(params HashDataBroker[] biParams)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                foreach (HashDataBroker bi in biParams)
                {
                    ms.Write(bi.HashData, 0, bi.HashData.Length);
                }

                byte[] res = SHAHash.ComputeHash(ms.ToArray());

                return new BigInteger(res);
            }
        }

        /// <summary>
        /// Hashing function for the instance.
        /// </summary>
        /// <remarks>MD5 or other SHA hashes are usable, though SHA1 is more standard for SRP.</remarks>
        [NonSerialized]
        public static readonly HashAlgorithm SHAHash = new SHA1Managed();

        #endregion
    }
}
