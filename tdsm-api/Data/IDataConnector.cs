using System;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace TDSM.API.Data
{
    public class Group
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool ApplyToGuests { get; set; }

        public string Parent { get; set; }

        public byte Chat_Red { get; set; }

        public byte Chat_Green { get; set; }

        public byte Chat_Blue { get; set; }

        public string Chat_Prefix { get; set; }

        public string Chat_Suffix { get; set; }
    }

    public struct PermissionNode
    {
        public string Node { get; set; }

        public bool Deny { get; set; }
    }

    /// <summary>
    /// The interface behind custom permissions handlers
    /// </summary>
    public interface IPermissionHandler
    {
        Permission IsPermitted(string node, BasePlayer player);

        #region "Management"

        Group FindGroup(string name);

        bool AddOrUpdateGroup(string name, bool applyToGuests = false, string parent = null, byte r = 255, byte g = 255, byte b = 255, string prefix = null, string suffix = null);

        bool RemoveGroup(string name);

        bool AddGroupNode(string groupName, string node, bool deny = false);

        bool RemoveGroupNode(string groupName, string node, bool deny = false);

        string[] GroupList();

        PermissionNode[] GroupNodes(string groupName);

        bool AddUserToGroup(string username, string groupName);

        bool RemoveUserFromGroup(string username, string groupName);

        bool AddNodeToUser(string username, string node, bool deny = false);

        bool RemoveNodeFromUser(string username, string node, bool deny = false);

        string[] UserGroupList(string username);

        PermissionNode[] UserNodes(string username);

        #endregion
    }

    public enum Permission : byte
    {
        Denied = 0,
        Permitted
    }

    public interface IDataConnector : IPermissionHandler
    {
        QueryBuilder GetBuilder(string pluginName);

        //        QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type);

        void Open();

        bool Execute(QueryBuilder builder);

        long ExecuteInsert(QueryBuilder builder);

        int ExecuteNonQuery(QueryBuilder builder);

        T ExecuteScalar<T>(QueryBuilder builder);

        DataSet ExecuteDataSet(QueryBuilder builder);

        T[] ExecuteArray<T>(QueryBuilder builder) where T : new();
    }

    public abstract class QueryBuilder : IDisposable
    {
        private string _plugin;
        private System.Text.StringBuilder _sb;
        private System.Data.CommandType _type;

        //Simple builder
        public QueryBuilder(string pluginName)
        {
            _sb = new System.Text.StringBuilder();

            _plugin = pluginName;
            _type = CommandType.Text;
        }

        //        //Command builder, essentially just for parameterised queries
        //        public QueryBuilder(string pluginName, string command, System.Data.CommandType type)
        //        {
        //            _sb = new System.Text.StringBuilder();
        //
        //            _sb.Append(command);
        //            _plugin = pluginName;
        //            System.Data.CommandType _type = type;
        //        }

        protected QueryBuilder Append(string fmt, params object[] args)
        {
            if (args == null || args.Length == 0)
                _sb.Append(fmt);
            else
                _sb.Append(String.Format(fmt, args));

            return this;
        }

        void IDisposable.Dispose()
        {
            if (_sb != null)
            {
                _sb.Clear();
                _sb = null;
            }
            _plugin = null;
        }

        public abstract QueryBuilder AddParam(string name, object value, string prefix = "prm");

        public abstract QueryBuilder TableExists(string name);

        public abstract QueryBuilder TableCreate(string name, params TableColumn[] columns);

        public abstract QueryBuilder TableDrop(string name);

        //        public virtual QueryBuilder ProcedureExists(string name){
        //            return this.Append("select 1 from information_schema.routines where routine_type='procedure' and
        //        }
        //
        //        public abstract QueryBuilder ProcedureCreate(string name, string contents, params DataParameter[] parameters);
        //
        //        public abstract QueryBuilder ProcedureDrop(string name);

        //        public abstract QueryBuilder ExecuteProcedure(string name, string prefix = "prm", params DataParameter[] parameters);

        public abstract QueryBuilder Select(params string[] expression);

        public abstract QueryBuilder All();

        public abstract QueryBuilder From(string tableName);

        public abstract QueryBuilder Where(params WhereFilter[] clause);

        public abstract QueryBuilder Count(string expression = null);

        public abstract QueryBuilder Delete();

        public virtual QueryBuilder Delete(string tableName, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.Delete().From(tableName);
            return this.Delete().From(tableName).Where(clause);
        }

        public abstract QueryBuilder InsertInto(string tableName, params DataParameter[] values);

        public abstract QueryBuilder UpdateValues(string tableName, DataParameter[] values);

        public virtual QueryBuilder Update(string tableName, DataParameter[] values, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.UpdateValues(tableName, values);
            return this.UpdateValues(tableName, values).Where(clause);
        }

        public virtual QueryBuilder SelectAll(string tableName, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.Select().All().From(tableName);
            return this.Select().All().From(tableName).Where(clause);
        }

        public virtual QueryBuilder SelectFrom(string tableName, string[] expression = null, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.Select(expression).From(tableName);
            return this.Select(expression).From(tableName).Where(clause);
        }

        //public virtual QueryBuilder If()
        //{
        //    return this.Append("IF ");
        //}
        //public virtual QueryBuilder Not()
        //{
        //    return this.Append("NOT ");
        //}

        //public virtual QueryBuilder Exists()
        //{
        //    return this.Append("EXISTS ");
        //}

        //public virtual QueryBuilder Else()
        //{
        //    return this.Append("ELSE ");
        //}

        //public virtual QueryBuilder OpenBracket()
        //{
        //    return this.Append("( ");
        //}

        //public virtual QueryBuilder CloseBracket()
        //{
        //    return this.Append(") ");
        //}

        protected string GetObjectName(string name)
        {
            return _plugin + '_' + name;
        }

        public CommandType CommandType
        {
            get
            { return _type; }
            set
            { _type = value; }
        }

        public string CommandText
        {
            get
            { return _sb.ToString(); }
            set
            {
                _sb.Clear();
                _sb.Append(value);
            }
        }

        public virtual string BuildCommand()
        {
            return _sb.ToString();
        }
    }

    public struct DataParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DataParameter(string name, object value)
            : this()
        {
            this.Name = name;
            this.Value = value;
        }
    }

    public struct TableColumn
    {
        public string Name { get; set; }

        public object DefaultValue { get; set; }

        public Type DataType { get; set; }

        public bool AutoIncrement { get; set; }

        public bool PrimaryKey { get; set; }

        public bool Nullable { get; set; }

        public int? MinScale { get; set; }

        public int? MaxScale { get; set; }

        public TableColumn(string name, Type dataType, bool autoIncrement, bool primaryKey, bool allowNulls = false)
            : this()
        {
            this.Name = name;
            this.DefaultValue = null;
            this.DataType = dataType;
            this.AutoIncrement = autoIncrement;
            this.PrimaryKey = primaryKey;
            this.Nullable = allowNulls;
            this.MinScale = null;
            this.MaxScale = null;
        }

        public TableColumn(string name, Type dataType, bool allowNulls = false)
            : this()
        {
            this.Name = name;
            this.DefaultValue = null;
            this.DataType = dataType;
            this.AutoIncrement = false;
            this.PrimaryKey = false;
            this.Nullable = allowNulls;
            this.MinScale = null;
            this.MaxScale = null;
        }

        public TableColumn(string name, Type dataType, int scale, bool allowNulls = false)
            : this()
        {
            this.Name = name;
            this.DefaultValue = null;
            this.DataType = dataType;
            this.AutoIncrement = false;
            this.PrimaryKey = false;
            this.Nullable = allowNulls;
            this.MinScale = scale;
            this.MaxScale = null;
        }
    }

    public struct WhereFilter
    {
        public string Column { get; set; }

        public string Value { get; set; }

        public WhereExpression Expression { get; set; }

        public WhereFilter(string column, string value, WhereExpression expression = WhereExpression.EqualTo)
            : this()
        {
            this.Expression = expression;
            this.Column = column;
            this.Value = value;
        }
    }

    public enum WhereExpression : byte
    {
        EqualTo,
        Like,
        NotEqualTo
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

            AuthenticatedUsers.Initialise();
        }

        public static QueryBuilder GetBuilder(string pluginName)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.GetBuilder(pluginName);
        }

        //        public static QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type)
        //        {
        //            if (_connector == null)
        //                throw new InvalidOperationException("No connector attached");
        //            return _connector.GetBuilder(pluginName, command, type);
        //        }

        public static bool Execute(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.Execute(builder);
        }

        public static long ExecuteInsert(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteInsert(builder);
        }

        public static int ExecuteNonQuery(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteNonQuery(builder);
        }

        public static T ExecuteScalar<T>(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteScalar<T>(builder);
        }

        public static DataSet ExecuteDataSet(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteDataSet(builder);
        }

        public static T[] ExecuteArray<T>(QueryBuilder builder) where T : new()
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteArray<T>(builder);
        }

        public static Permission IsPermitted(string node, BasePlayer player)
        {
            if (_connector == null)
                return player.Op ? Permission.Permitted : Permission.Denied;
            return _connector.IsPermitted(node, player);
        }

        //Management
        public static Group FindGroup(string name)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.FindGroup(name);
        }

        public static bool AddOrUpdateGroup(string name, bool applyToGuests = false, string parent = null, byte r = 255, byte g = 255, byte b = 255, string prefix = null, string suffix = null)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.AddOrUpdateGroup(name, applyToGuests, parent, r, g, b, prefix, suffix);
        }

        public static bool RemoveGroup(string name)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.RemoveGroup(name);
        }

        public static bool AddGroupNode(string groupName, string node, bool deny = false)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.AddGroupNode(groupName, node, deny);
        }

        public static bool RemoveGroupNode(string groupName, string node, bool deny = false)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.RemoveGroupNode(groupName, node, deny);
        }

        public static string[] GroupList()
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.GroupList();
        }

        public static PermissionNode[] GroupNodes(string groupName)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.GroupNodes(groupName);
        }

        public static bool AddUserToGroup(string username, string groupName)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.AddUserToGroup(username, groupName);
        }

        public static bool RemoveUserFromGroup(string username, string groupName)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.RemoveUserFromGroup(username, groupName);
        }

        public static bool AddNodeToUser(string username, string node, bool deny = false)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.AddNodeToUser(username, node, deny);
        }

        public static bool RemoveNodeFromUser(string username, string node, bool deny = false)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.RemoveNodeFromUser(username, node, deny);
        }

        public static string[] UserGroupList(string username)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.UserGroupList(username);
        }

        public static PermissionNode[] UserNodes(string username)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.UserNodes(username);
        }
    }
}

