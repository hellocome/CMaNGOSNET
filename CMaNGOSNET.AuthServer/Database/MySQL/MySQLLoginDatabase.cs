using System;
using System.Collections.Generic;
using System.Data;
using CMaNGOSNET.Common.Database.MySQL;
using CMaNGOSNET.Common.Database;
using CMaNGOSNET.AuthServer.Accounts;
using CMaNGOSNET.Common.Numerics;

namespace CMaNGOSNET.AuthServer.Database.MySQL
{
    public class MySQLLoginDatabase : MySQLConnectionBase, ILoginDatabase
    {
        public override Dictionary<int, PreparedStatement> PreparedStatementMap
        {
            get
            {
                return MySQLLoginDatabaseStatements.PreparedStatementMap;
            }
        }

        public MySQLLoginDatabase(string connectionString) : base(connectionString)
        {

        }
        
        
        public void ExecuteLoginDatabaseStatement(LoginDatabaseStatements statement, params object[] paras)
        {
            int index = (int)statement;

            ExecutePreparedStatement(index, paras);

        }

        public void ExecuteLoginDatabaseStatement(LoginDatabaseStatements statement)
        {
            int index = (int)statement;

            ExecutePreparedStatement(index);
        }

        public IDataReader QueryLoginDatabaseStatementSync(LoginDatabaseStatements statement)
        {
            int index = (int)statement;

            return QuerySync(index);
        }

        public IDataReader QueryLoginDatabaseStatementSync(LoginDatabaseStatements statement, params object[] paras)
        {
            int index = (int)statement;

            return QuerySync(index, paras);
        }

        public Account QueryLoginAccount(string login)
        {
            Account acc = new Account();

            IDataReader reader = QueryLoginDatabaseStatementSync(LoginDatabaseStatements.LOGIN_SEL_LOGONCHALLENGE, login);

            if(reader.Read())
            {
                acc.Username = login;
                acc.SHAPassHash = reader.GetString(0);
                acc.AccountId = reader.GetInt32(1);
                acc.Locked = reader.GetByte(2);
                acc.LockCountry = reader.GetString(3);
                acc.LastIP = reader.GetString(4);

                acc.AccountAccess = new AccountAccess(acc);
                acc.AccountAccess.GMLevel = (AccountTypes)reader.GetByte(5);

                acc.ValidV = acc.V.TrySetHexStr(reader.GetString(6));
                acc.ValidS = acc.S.TrySetHexStr(reader.GetString(7));

                acc.TokenKey = reader.GetString(8);

                acc.Valid = true;
            }

            return acc;
        }


        public bool QuerySessionKey(string login, out string session, out byte level)
        {
            session = string.Empty;
            level = 0;

            IDataReader reader = LoginManager.Instance.QueryLoginDatabaseStatementSync(LoginDatabaseStatements.LOGIN_SEL_SESSIONKEY, login);

            if (reader.Read())
            {
                session = reader.GetString(0);
                level = reader.GetByte(2);

                return true;
            }

            return false;
        }


        public bool QueryAccoutId(string login, out int id)
        {
            id = 0;

            IDataReader reader = LoginManager.Instance.QueryLoginDatabaseStatementSync(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_ID_BY_NAME, login);

            if (reader.Read())
            {
                id = reader.GetInt32(0);

                return true;
            }

            return false;
        }
    }
}
