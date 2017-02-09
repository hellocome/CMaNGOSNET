using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.Common.Database
{
    public class PreparedStatement
    {
        public int PreparedStatementIndex
        {
            get;
            private set;
        }

        public string SQLString
        {
            get;
            private set;
        }

        public ConnectionFlags ConnectionFlag
        {
            get;
            private set;
        }

        public PreparedStatement(int index, string sqlString, ConnectionFlags flag = ConnectionFlags.CONNECTION_SYNCH)
        {
            PreparedStatementIndex = index;
            SQLString = sqlString;
            ConnectionFlag = flag;
        }
    }
}
