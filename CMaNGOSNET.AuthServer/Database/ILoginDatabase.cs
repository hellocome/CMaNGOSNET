using System;
using CMaNGOSNET.AuthServer.Accounts;
using System.Data;

namespace CMaNGOSNET.AuthServer.Database
{
    public interface ILoginDatabase
    {
        void ExecuteLoginDatabaseStatement(LoginDatabaseStatements statement, params object[] paras);
        void ExecuteLoginDatabaseStatement(LoginDatabaseStatements statement);

        IDataReader QueryLoginDatabaseStatementSync(LoginDatabaseStatements statement);
        IDataReader QueryLoginDatabaseStatementSync(LoginDatabaseStatements statement, params object[] paras);


        Account QueryLoginAccount(string login);

        bool QuerySessionKey(string login, out string session, out byte level);

        bool QueryAccoutId(string login, out int id);
    }
}
