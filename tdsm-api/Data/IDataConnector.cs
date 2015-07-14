using System;
using System.Data;

namespace tdsm.api.Data
{
    public interface IDataConnector
    {
        QueryBuilder GetBuilder(string pluginName);

        QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type);

        void Open();

        bool Execute(QueryBuilder builder);

        T ExecuteScalar<T>(QueryBuilder builder);

        DataSet ExecuteDataSet(QueryBuilder builder);

        T[] ExecuteArray<T>(QueryBuilder builder); //where T : new();
    }

    public abstract class QueryBuilder : IDisposable
    {
        private string _plugin;
        private string _command;
        private System.Data.CommandType _type;

        //Simple builder
        public QueryBuilder(string pluginName)
        {
            _plugin = pluginName;
        }

        //Command builder, essentially just for parameterised queries
        public QueryBuilder(string pluginName, string command, System.Data.CommandType type)
        {
            _plugin = pluginName;
            _command = command;
            System.Data.CommandType _type = type;
        }

        void IDisposable.Dispose()
        {

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

