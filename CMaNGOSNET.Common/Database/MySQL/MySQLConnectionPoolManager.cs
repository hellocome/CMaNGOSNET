using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMaNGOSNET.Common.Collection;

namespace CMaNGOSNET.Common.Database.MySQL
{
    public class DBConnectionPoolManager<T> : ObjectPool<T> where T : class, IDBConnection, IDisposable
    {
        private static DBConnectionPoolManager<T> instance;
        public static DBConnectionPoolManager<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBConnectionPoolManager<T>();
                }

                return instance;
            }
        }

        public DBConnectionPoolManager()
        {

        }
    }
}
