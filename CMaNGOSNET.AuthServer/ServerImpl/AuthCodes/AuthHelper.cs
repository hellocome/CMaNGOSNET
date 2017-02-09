using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.AuthServer.ServerImpl.AuthCodes
{
    public static class AuthHelper
    {
        static Dictionary<int, RealmBuildInfo> PostBcAcceptedClientBuilds = new Dictionary<int, RealmBuildInfo>()
        {
            { 15595, new RealmBuildInfo(15595, 4, 3, 4, ' ') },
            { 14545, new RealmBuildInfo(14545, 4, 2, 2, ' ') },
            { 13623, new RealmBuildInfo(13623, 4, 0, 6, 'a') },
            { 13930, new RealmBuildInfo(13930, 3, 3, 5, 'a') },                                  // 3.3.5a China Mainland build
            { 12340, new RealmBuildInfo(12340, 3, 3, 5, 'a') },
            { 11723, new RealmBuildInfo(11723, 3, 3, 3, 'a') },
            { 11403, new RealmBuildInfo(11403, 3, 3, 2, ' ') },
            { 11159, new RealmBuildInfo(11159, 3, 3, 0, 'a') },
            { 10505, new RealmBuildInfo(10505, 3, 2, 2, 'a') },
            { 9947,  new RealmBuildInfo(9947,  3, 1, 3, ' ') },
            { 8606,  new RealmBuildInfo(8606,  2, 4, 3, ' ') },
            //{ 0,     new RealmBuildInfo(0,     0, 0, 0, ' ') }     // terminator\
        };

        static Dictionary<int, RealmBuildInfo> PreBcAcceptedClientBuilds = new Dictionary<int, RealmBuildInfo>()
        {
            { 6141, new RealmBuildInfo(6141,  1, 12, 3, ' ')},
            { 6005, new RealmBuildInfo(6005,  1, 12, 2, ' ')},
            { 5875, new RealmBuildInfo(5875,  1, 12, 1, ' ')},
            //{ 0,    new RealmBuildInfo(0,     0, 0,  0, ' ')}                                   // terminator
        };

        public static RealmBuildInfo GetBuildInfo(int build)
        {
            if(IsPostBCAcceptedClientBuild(build))
            {
                return PostBcAcceptedClientBuilds[build];
            }

            if (IsPreBCAcceptedClientBuild(build))
            {
                return PreBcAcceptedClientBuilds[build];
            }

            return null;
        }

        public static bool IsAcceptedClientBuild(int build)
        {
            return (IsPostBCAcceptedClientBuild(build) || IsPreBCAcceptedClientBuild(build));
        }

        public static bool IsPostBCAcceptedClientBuild(int build)
        {
            if (PostBcAcceptedClientBuilds.ContainsKey(build))
            {
                return true;
            }

            return false;
        }

        public static bool IsPreBCAcceptedClientBuild(int build)
        {
            if(PreBcAcceptedClientBuilds.ContainsKey(build))
            {
                return true;
            }

            return false;
        }
    }
}
