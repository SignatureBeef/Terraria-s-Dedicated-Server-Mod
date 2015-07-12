using System;
using tdsm.api.Data;
using SQLite;

namespace TDSM.Data.SQLite
{
    public class SQLiteConnector : IDataConnector
    {
        static readonly SQLiteQueryBuilder _builder = new SQLiteQueryBuilder();

        private SQLiteConnection _database;

        public QueryBuilder GetBuilder()
        {
            return _builder;
        }

        public SQLiteConnector(string connectionString)
        {
            _database = new SQLiteConnection(connectionString);
        }

        public void Open()
        {
            
        }

        public override string ToString()
        {
            return "[SQLiteConnector]";
        }
    }

    public class SQLiteQueryBuilder : QueryBuilder
    {

    }
}

