using System;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace TDSM.API.Data
{
    public interface IDataConnector
    {
        QueryBuilder GetBuilder(string pluginName);

        QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type);

        void Open();

        bool Execute(QueryBuilder builder);

        T ExecuteScalar<T>(QueryBuilder builder);

        DataSet ExecuteDataSet(QueryBuilder builder);

        T[] ExecuteArray<T>(QueryBuilder builder);
        //where T : new();
    }

    public abstract class QueryBuilder : IDisposable
    {
        private string _plugin;
        private StringBuilder _sb;
        private System.Data.CommandType _type;

        //Simple builder
        public QueryBuilder(string pluginName)
        {
            _sb = new StringBuilder();

            _plugin = pluginName;
            _type = CommandType.Text;
        }

        //Command builder, essentially just for parameterised queries
        public QueryBuilder(string pluginName, string command, System.Data.CommandType type)
        {
            _sb = new StringBuilder();

            _sb.Append(command);
            _plugin = pluginName;
            System.Data.CommandType _type = type;
        }

        protected void Append(string fmt, params object[] args)
        {
            if (args == null || args.Length == 0)
                _sb.Append(fmt);
            else
                _sb.Append(args);
        }

        void IDisposable.Dispose()
        {
            _sb.Clear();
            _sb = null;
            _plugin = null;
        }

        public abstract QueryBuilder AddParam(string name, object value);

        public abstract QueryBuilder TableExists(string name);

        public abstract QueryBuilder TableCreate(string name, params TableColumn[] columns);

        public abstract QueryBuilder TableDrop(string name);

        public abstract QueryBuilder ProcedureExists(string name);

        public abstract QueryBuilder ProcedureCreate(string name, string contents, params DataParameter[] parameters);

        public abstract QueryBuilder ProcedureDrop(string name);

        public abstract QueryBuilder SelectAll(string tableName, params WhereFilter[] clause);

        public abstract QueryBuilder SelectFrom(string tableName, string[] expression = null, params WhereFilter[] clause);

        public abstract QueryBuilder Count();

        public abstract QueryBuilder DeleteFrom(string tableName, params WhereFilter[] clause);

        public abstract QueryBuilder InsertInto(string tableName, params DataParameter[] values);

        public abstract QueryBuilder Update(string tableName, DataParameter[] values, params WhereFilter[] clause);

        protected string GetTableName(string tableName)
        {
            return _plugin + '_' + tableName;
        }

        public CommandType CommandType
        {
            get
            { return _type; }
        }

        public string BuildCommand()
        {
            return null;
        }
    }

    public struct DataParameter
    {

    }

    public struct TableColumn
    {

    }

    public struct WhereFilter
    {

    }

    public static class Storage
    {
        private static readonly object _sync = new object();
        private static IDataConnector _connector;

        public static bool IsAvailable
        {
            get
            { return _connector != null; }
        }

        public static void SetConnector(IDataConnector connector, bool throwWhenSet = true)
        {
            lock (_sync)
            {
                if (_connector != null && throwWhenSet)
                {
                    throw new InvalidOperationException(String.Format("Attempted to load '{0}' when a '{1}' was already loaded", connector.ToString(), _connector.ToString()));
                }
                _connector = connector;
            }
        }

        public static QueryBuilder GetBuilder(string pluginName)
        {
            return _connector.GetBuilder(pluginName);
        }

        public static QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type)
        {
            return _connector.GetBuilder(pluginName, command, type);
        }

        public static bool Execute(QueryBuilder builder)
        {
            return _connector.Execute(builder);
        }

        public static T ExecuteScalar<T>(QueryBuilder builder)
        {
            return _connector.ExecuteScalar<T>(builder);
        }

        public static DataSet ExecuteDataSet(QueryBuilder builder)
        {
            return _connector.ExecuteDataSet(builder);
        }

        public static T[] ExecuteArray<T>(QueryBuilder builder) //where T : new()
        {
            return _connector.ExecuteArray<T>(builder);
        }
    }
}

