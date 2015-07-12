using System;
using tdsm.api.Data;
using MySql.Data.MySqlClient;

namespace TDSM.Data.MySQL
{
    public class MySQLConnector : IDataConnector
    {
        static readonly MySQLQueryBuilder _builder = new MySQLQueryBuilder();

        private MySqlConnection _connection;

        public QueryBuilder GetBuilder()
        {
            return _builder;
        }

        public MySQLConnector(string connectionString)
        {
            _connection = new MySqlConnection();
            _connection.ConnectionString = connectionString;
        }

        public void Open()
        {
            _connection.Open();
        }

        public override string ToString()
        {
            return "[MySQLConnector]";
        }
    }

    public class MySQLQueryBuilder : QueryBuilder
    {

    }
}

