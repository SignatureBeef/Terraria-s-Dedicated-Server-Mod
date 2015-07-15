using System;
using TDSM.API.Data;
using SQLite;
using System.Data;

namespace TDSM.Data.SQLite
{
    public class SQLiteConnector : IDataConnector
    {
        private SQLiteConnection _database;

        public QueryBuilder GetBuilder(string pluginName)
        {
            return new SQLiteQueryBuilder(pluginName);
        }

        public QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type)
        {
            return new SQLiteQueryBuilder(pluginName, command, type);
        }

        public SQLiteConnector(string connectionString)
        {
            _database = new SQLiteConnection(connectionString);
        }

        public void Open()
        {
            
        }

        bool IDataConnector.Execute(QueryBuilder builder)
        {
            return false;
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
            return "[SQLiteConnector]";
        }
    }

    public class SQLiteQueryBuilder : QueryBuilder
    {
        public   SQLiteQueryBuilder(string pluginName)
            : base(pluginName)
        {
        }

        public   SQLiteQueryBuilder(string pluginName, string command, System.Data.CommandType type)
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

