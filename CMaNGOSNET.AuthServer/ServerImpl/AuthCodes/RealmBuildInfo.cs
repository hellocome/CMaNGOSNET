using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.AuthServer.ServerImpl.AuthCodes
{
    public class RealmBuildInfo
    {
        public int Build;
        public int MajorVersion;
        public int MinorVersion;
        public int BugfixVersion;
        public int HotfixVersion;

        public RealmBuildInfo(int build, int majorVersion, int minorVersion, int bugfixVersion, int hotfixVersion)
        {
            Build = build;
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            BugfixVersion = bugfixVersion;
            HotfixVersion = hotfixVersion;
        }
    }
}
