using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Numerics;
using CMaNGOSNET.Common.Numerics;

namespace CMaNGOSNET.Common.Security
{
    /*
         SRP Protocol Design

        SRP is the newest addition to a new class of strong authentication protocols that resist all the well-known passive and active attacks over the network. SRP borrows some elements from other key-exchange and identification protcols and adds some subtle modifications and refinements. The result is a protocol that preserves the strength and efficiency of the EKE family protocols while fixing some of their shortcomings.
        The following is a description of SRP-6 and 6a, the latest versions of SRP:

          N    A large safe prime (N = 2q+1, where q is prime)
               All arithmetic is done modulo N.
          g    A generator modulo N
          k    Multiplier parameter (k = H(N, g) in SRP-6a, k = 3 for legacy SRP-6)
          s    User's salt
          I    Username
          p    Cleartext Password
          H()  One-way hash function
          ^    (Modular) Exponentiation
          u    Random scrambling parameter
          a,b  Secret ephemeral values
          A,B  Public ephemeral values
          x    Private key (derived from p and s)
          v    Password verifier
        The host stores passwords using the following formula:
          x = H(s, p)               (s is chosen randomly)
          v = g^x                   (computes password verifier)
        The host then keeps {I, s, v} in its password database. The authentication protocol itself goes as follows:
        User -> Host:  I, A = g^a                  (identifies self, a = random number)
        Host -> User:  s, B = kv + g^b             (sends salt, b = random number)

                Both:  u = H(A, B)

                User:  x = H(s, p)                 (user enters password)
                User:  S = (B - kg^x) ^ (a + ux)   (computes session key)
                User:  K = H(S)

                Host:  S = (Av^u) ^ b              (computes session key)
                Host:  K = H(S)
        Now the two parties have a shared, strong session key K. To complete authentication, they need to prove to each other that their keys match. One possible way:
        User -> Host:  M = H(H(N) xor H(g), H(I), s, A, B, K)
        Host -> User:  H(A, M, K)
        The two parties also employ the following safeguards:
        The user will abort if he receives B == 0 (mod N) or u == 0.
        The host will abort if it detects that A == 0 (mod N).
        The user must show his proof of K first. If the server detects that the user's proof is incorrect, it must abort without showing its own proof of K.
     */
    public class SecureRemotePassword
    {
        private ISRPSetting srpSetting;

        public SecureRemotePassword(ISRPSetting setting)
        {
            srpSetting = setting;
        }

        public SecureRemotePassword()
        {
            srpSetting = new DefaultSRPSetting();

            B = BigInteger.Zero;
            Session = BigInteger.Zero;
            K = BigInteger.Zero;
            ReconnectProof = BigInteger.Zero;
        }

        /// <summary>
        /// N    A large safe prime (N = 2q+1, where q is prime) All arithmetic is done modulo N.
        /// </summary>
        public BigInteger N
        {
            get
            {
                return srpSetting.N;
            }
        }

        /// <summary>
        /// 'G'   in the spec. This number must be a generator in the finite field Modulus.
        /// </summary>
        public BigInteger G
        {
            get { return srpSetting.G; }
        }

        public BigInteger B
        {
            get;
            private set;
        }

        public BigInteger Session
        {
            get;
            private set;
        }

        public BigInteger K
        {
            get;
            private set;
        }

        public BigInteger M
        {
            get;
            private set;
        }

        public BigInteger ReconnectProof
        {
            get;
            private set;
        }

        public BigInteger Multiplier
        {
            get
            {
                if (srpSetting.SRPVersion == SRPVersion.SRP6a)
                {
                    return srpSetting.Hash(N, G);
                }
                else if (srpSetting.SRPVersion == SRPVersion.SRP6)
                {
                    return 3;
                }
                else
                {
                    throw new NotSupportedException("The version is not supported");
                }
            }
        }

        public static BigInteger RandomNumber(uint size)
        {
            return new BigInteger(CryptoUtility.GenerateRandomByteArray(size));
        }


        /*
            The host stores passwords using the following formula:
              x = H(s, p)               (s is chosen randomly)
              v = g^x                   (computes password verifier) 

            The host then keeps {I, s, v} in its password database. The authentication protocol itself goes as follows:
        */
        public BigInteger ComputesPasswordVerifier(BigInteger salt, BigInteger passwordHash)
        {
            BigInteger x = srpSetting.Hash(salt, passwordHash);

            return G.ModPow(x, N);
        }

        public BigInteger CalculateB(BigInteger verifier)
        {
            BigInteger r = RandomNumber(srpSetting.KeyLength);
            BigInteger b = (Multiplier * verifier) + G.ModPow(r, N);

            b %= N;

            if(b < 0)
            {
                b += N;
            }

            B = b;
            return B; 
        }

        public bool SetVSFields(string rI, out string v, out string s)
        {
            BigInteger salt = RandomNumber(srpSetting.KeyLength);
            BigInteger verifier = 0;
            BigInteger passwordHash = 0;

            v = string.Empty;
            s = salt.ToHexStr();

            if (passwordHash.TrySetHexStr(rI))
            {
                verifier = ComputesPasswordVerifier(salt, passwordHash);
                v = verifier.ToHexStr();

                return true;
            }
            else
            {
                return false;
            }

        }

        public BigInteger ComputeM(string login, BigInteger A, BigInteger v, BigInteger salt)
        {
            if (A.IsZero || B.IsZero || v.IsZero)
            {
                return BigInteger.Zero;
            }
            else
            {
                BigInteger u = srpSetting.Hash(A, B);
                Session = (A * v.ModPow(u, N)).ModPow(B, N);

                if (Session.ToByteArray().Length != 32)
                {
                    return BigInteger.Zero;
                }
                else
                {
                    byte[] t = new byte[32];
                    byte[] temp = new byte[16];
                    byte[] vK = new byte[40];

                    Array.Copy(A.ToByteArray(), t, 32);

                    for (int i = 0; i < temp.Length; i++)
                        temp[i] = t[i * 2];

                    byte[] hashTemp = srpSetting.Hash(temp).ToByteArray();

                    for (int i = 0; i < 20; i++)
                        vK[i * 2] = hashTemp[i];

                    for (int i = 0; i < 16; i++)
                        temp[i] = t[i * 2 + 1];

                    hashTemp = srpSetting.Hash(temp).ToByteArray();

                    for (int i = 0; i < 20; i++)
                        vK[i * 2 + 1] = hashTemp[i];

                    // K: session key hash
                    K = new BigInteger(vK);

                    byte[] nHash = srpSetting.Hash(N).ToByteArray();
                    byte[] gHash = srpSetting.Hash(G).ToByteArray();

                    byte[] hash = new byte[20];

                    for (int i = 0; i < hash.Length; i++)
                        hash[i] = (byte)(nHash[i] ^ gHash[i]);

                    byte[] hashUserName = srpSetting.Hash(login).ToByteArray();

                    M = srpSetting.Hash(hash, hashUserName, salt, A, B, K);
                }

                return M;
            }
        }

        public BigInteger ComputeClientProofM(BigInteger A)
        {
            return srpSetting.Hash(A, M, K);
        }

        public void UpdateK(string hexString)
        {
            if (!K.TrySetHexStr(hexString))
            {
                K = BigInteger.Zero;
            }
        }


        public BigInteger UpdateReconnectProof()
        {
            ReconnectProof = new BigInteger(CryptoUtility.GenerateRandomByteArray(16));
            return ReconnectProof;
        }

        public bool ValidateReconnectProof(string login, BigInteger r1, BigInteger r2)
        {
            if (r2.Equals(srpSetting.Hash(login, r1, ReconnectProof, K)))
            {
                return true;
            }

            return false;
        }
    }
}
