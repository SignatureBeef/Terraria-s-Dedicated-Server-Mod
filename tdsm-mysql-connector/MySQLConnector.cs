using System;
using tdsm.api.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace TDSM.Data.MySQL
{
    public class MySQLConnector : IDataConnector
    {
        private MySqlConnection _connection;

        public QueryBuilder GetBuilder(string pluginName)
        {
            return new MySQLQueryBuilder(pluginName);
        }

        public QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type)
        {
            return new MySQLQueryBuilder(pluginName, command, type);
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

        T IDataConnector.ExecuteScalar<T>(QueryBuilder builder)
        {
            return default(T);
        }

        DataSet IDataConnector.ExecuteDataSet(QueryBuilder builder)
        {
            return null;
        }

        T[] IDataConnector.ExecuteArray<T>(QueryBuilder builder)
        {
            return null;
        }

        public override string ToString()
        {
            return "[MySQLConnector]";
        }
    }

    public class MySQLQueryBuilder : QueryBuilder
    {
        public   MySQLQueryBuilder(string pluginName)
            : base(pluginName)
        {
        }

        public   MySQLQueryBuilder(string pluginName, string command, System.Data.CommandType type)
            : base(pluginName, command, type)
        {

        }


        public override QueryBuilder AddParam(string name, object value)
        {
            return this;
        }

        public override QueryBuilder TableExists(string name)
        {
            return this;
        }

        public override QueryBuilder TableCreate(string name, params TableColumn[] columns)
        {
            return this;
        }

        public  override QueryBuilder TableDrop(string name)
        {
            return this;
        }

        public override QueryBuilder ProcedureExists(string name)
        {
            return this;
        }

        public override QueryBuilder ProcedureCreate(string name, string contents, params DataParameter[] parameters)
        {
            return this;
        }

        public override QueryBuilder ProcedureDrop(string name)
        {
            return this;
        }

        public override QueryBuilder SelectAll(string tableName, params WhereFilter[] clause)
        {
            return this;
        }

        public override QueryBuilder SelectFrom(string tableName, string[] expression = null, params WhereFilter[] clause)
        {
            return this;
        }

        public override QueryBuilder Count()
        {
            return this;
        }

        public override QueryBuilder DeleteFrom(string tableName, params WhereFilter[] clause)
        {
            return this;
        }

        public override QueryBuilder InsertInto(string tableName, params DataParameter[] values)
        {
            return this;
        }

        public override QueryBuilder Update(string tableName, DataParameter[] values, params WhereFilter[] clause)
        {
            return this;
        }
    }
}

