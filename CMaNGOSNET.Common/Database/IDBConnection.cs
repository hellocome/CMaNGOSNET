using System;
using System.Collections.Generic;
using System.Data;

namespace CMaNGOSNET.Common.Database
{
    public interface IDBConnection : IDisposable
    {
        string ConnectionString
        {
            get;
        }

        Guid GUID
        {
            get;
        }


        int Execute(string str);

        int Execute(string str, params object[] paras);

        void ExecuteAsync(string str);

        void ExecuteAsync(string str, params object[] paras);

        void Execute(PreparedStatement str, params object[] paras);

        void Execute(PreparedStatement str);

        void ExecutePreparedStatement(int index, params object[] paras);

        void ExecutePreparedStatement(int index);

        IDataReader QuerySync(int index);

        IDataReader QuerySync(int index, params object[] paras);

        IDataReader QuerySync(PreparedStatement str);

        IDataReader QuerySync(PreparedStatement str, params object[] paras);
    }
}
