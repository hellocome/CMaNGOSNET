using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CMaNGOSNET.Common.Network
{
    public static class NetworkUtility
    {
        public static string GetRemoteIpAddress(SocketAsyncEventArgs args)
        {
            if(args == null || args.RemoteEndPoint == null)
            {
                return string.Empty;
            }

            IPEndPoint ipep = (IPEndPoint)args.RemoteEndPoint;

            return ipep.Address.ToString();
        }

        public static int GetRemotePort(SocketAsyncEventArgs args)
        {
            if (args == null || args.RemoteEndPoint == null)
            {
                return -1;
            }

            IPEndPoint ipep = (IPEndPoint)args.RemoteEndPoint;

            return ipep.Port;
        }
    
        public static int IPToInt(string address)
        {
            return BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes(), 0);
        }

        public static string IntToIP(int intAddress)
        {
            return new IPAddress(BitConverter.GetBytes(intAddress)).ToString();
        }
    }
}
