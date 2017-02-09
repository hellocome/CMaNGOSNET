using System.Data;
using MySql.Data.MySqlClient;

using CMaNGOSNET.Common.Logging;
namespace CMaNGOSNET.Common.Database.MySQL
{
    public abstract class MySQLConnectionBase : DBConnectionBase
    {
        public MySQLConnectionBase(string connectionString) : base(connectionString)
        {
        }

        public override int Execute(string str)
        {
            return Execute(str, null);
        }

        public override int Execute(string str, params object[] paras)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                MySqlCommand command = PrepareDBCommand(connection, str, paras);

                return command.ExecuteNonQuery();
            }
        }

        public override void ExecuteAsync(string str)
        {
            ExecuteAsync(str, null);
        }

        public override void ExecuteAsync(string str, params object[] paras)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                MySqlCommand command = PrepareDBCommand(connection, str, paras);

                command.ExecuteNonQueryAsync();
            }
        }

        public override void Execute(PreparedStatement str, params object [] paras)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                MySqlCommand command = PrepareDBCommand(connection, str, paras);

                if (str.ConnectionFlag == ConnectionFlags.CONNECTION_SYNCH)
                {
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.ExecuteNonQueryAsync();
                }
            }
        }

        public override void Execute(PreparedStatement str)
        {
            Execute(str, null);
        }

        public override IDataReader QuerySync(PreparedStatement str)
        {
            return QuerySync(str, null);
        }

        public override IDataReader QuerySync(PreparedStatement str, params object[] paras)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                MySqlCommand command = PrepareDBCommand(connection, str, paras);

                return command.ExecuteReader();
            }
        }

        protected MySqlCommand PrepareDBCommand(MySqlConnection dbConnection, PreparedStatement str, params object[] paras)
        {
            MySqlCommand command = new MySqlCommand(str.SQLString, dbConnection);
            command.Prepare();

            if (paras != null)
            {
                foreach (object value in paras)
                {
                    command.Parameters.Add(value);
                }
            }

            return command;
        }

        protected MySqlCommand PrepareDBCommand(MySqlConnection dbConnection, string str, params object[] paras)
        {
            MySqlCommand command = new MySqlCommand(str, dbConnection);
            command.Prepare();

            if (paras != null)
            {
                foreach (object value in paras)
                {
                    command.Parameters.Add(value);
                }
            }

            return command;
        }

    }
}
