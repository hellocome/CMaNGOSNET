using System;
using System.Collections.Generic;
using System.Data;

namespace CMaNGOSNET.Common.Database.MySQL
{
    public abstract class DBConnectionBase : IDBConnection
    {
        public abstract Dictionary<int, PreparedStatement> PreparedStatementMap
        {
            get;
        }

        public string ConnectionString
        {
            get;
            private set;
        }

        public Guid GUID
        {
            get;
            private set;
        }

        public DBConnectionBase(string connectionString)
        {
            ConnectionString = connectionString;

            GUID = new Guid();
        }

        public void ExecutePreparedStatement(int index, params object[] paras)
        {
            if (PreparedStatementMap.ContainsKey(index))
            {
                Execute(PreparedStatementMap[index], paras);
            }
            else
            {
                throw new ArgumentException("The index is not reqistered!");
            }

        }

        public void ExecutePreparedStatement(int index)
        {
            if (PreparedStatementMap.ContainsKey(index))
            {
                Execute(PreparedStatementMap[index]);
            }
            else
            {
                throw new ArgumentException("The index is not reqistered!");
            }
        }

        public IDataReader QuerySync(int index)
        {
            if (PreparedStatementMap.ContainsKey(index))
            {
                return QuerySync(PreparedStatementMap[index]);
            }
            else
            {
                throw new ArgumentException("The index is not reqistered!");
            }
        }

        public IDataReader QuerySync(int index, params object[] paras)
        {
            if (PreparedStatementMap.ContainsKey(index))
            {
                return QuerySync(PreparedStatementMap[index], paras);
            }
            else
            {
                throw new ArgumentException("The index is not reqistered!");
            }
        }

        public abstract int Execute(string str);

        public abstract int Execute(string str, params object[] paras);

        public abstract void ExecuteAsync(string str);

        public abstract void ExecuteAsync(string str, params object[] paras);

        public abstract void Execute(PreparedStatement str, params object[] paras);

        public abstract void Execute(PreparedStatement str);

        public abstract IDataReader QuerySync(PreparedStatement str, params object[] paras);

        public abstract IDataReader QuerySync(PreparedStatement str);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // so that Dispose(false) isn't called later
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose all owned managed objects
            }

            // Release unmanaged resources
        }
    }
}
