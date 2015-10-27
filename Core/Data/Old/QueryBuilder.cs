using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSM.Core.Data.Old
{
    /// <summary>
    /// The bare implementation of a query builder
    /// </summary>
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

        /// <summary>
        /// Adds a parameter.
        /// </summary>
        /// <returns>The parameter.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="prefix">Prefix.</param>
        public abstract QueryBuilder AddParam(string name, object value, string prefix = "prm");

        /// <summary>
        /// Check if a table exists
        /// </summary>
        /// <returns>The exists.</returns>
        /// <param name="name">Name.</param>
        public abstract QueryBuilder TableExists(string name);

        /// <summary>
        /// Creates a table
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="name">Name.</param>
        /// <param name="columns">Columns.</param>
        public abstract QueryBuilder TableCreate(string name, params TableColumn[] columns);

        /// <summary>
        /// Drops a table
        /// </summary>
        /// <returns>The drop.</returns>
        /// <param name="name">Name.</param>
        public abstract QueryBuilder TableDrop(string name);

        //        public virtual QueryBuilder ProcedureExists(string name){
        //            return this.Append("select 1 from information_schema.routines where routine_type='procedure' and
        //        }
        //
        //        public abstract QueryBuilder ProcedureCreate(string name, string contents, params DataParameter[] parameters);
        //
        //        public abstract QueryBuilder ProcedureDrop(string name);

        //        public abstract QueryBuilder ExecuteProcedure(string name, string prefix = "prm", params DataParameter[] parameters);

        /// <summary>
        /// Begins a SELECT query
        /// </summary>
        /// <param name="expression">Expression.</param>
        public abstract QueryBuilder Select(params string[] expression);

        /// <summary>
        /// Adds the all expression to the query
        /// </summary>
        public abstract QueryBuilder All();

        /// <summary>
        /// Adds the from table selector
        /// </summary>
        /// <param name="tableName">Table name.</param>
        public abstract QueryBuilder From(string tableName);

        /// <summary>
        /// Adds a filter on data
        /// </summary>
        /// <param name="clause">Clause.</param>
        public abstract QueryBuilder Where(params WhereFilter[] clause);

        //        public abstract QueryBuilder WhereNotExists(QueryBuilder bld);
        //
        //        public abstract QueryBuilder WhereExists(QueryBuilder bld);

        /// <summary>
        /// Adds the count expression
        /// </summary>
        /// <param name="expression">Expression.</param>
        public abstract QueryBuilder Count(string expression = null);

        /// <summary>
        /// Add a DELETE statement
        /// </summary>
        public abstract QueryBuilder Delete();

        /// <summary>
        /// Build a DELETE statement
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="clause">Clause.</param>
        public virtual QueryBuilder Delete(string tableName, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.Delete().From(tableName);
            return this.Delete().From(tableName).Where(clause);
        }

        /// <summary>
        /// Add an INSERT TO statement
        /// </summary>
        /// <returns>The into.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="values">Values.</param>
        public abstract QueryBuilder InsertInto(string tableName, params DataParameter[] values);

        /// <summary>
        /// Adds a UPDATE statement with specified columns and values
        /// </summary>
        /// <returns>The values.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="values">Values.</param>
        public abstract QueryBuilder UpdateValues(string tableName, DataParameter[] values);

        /// <summary>
        /// Builds an UPDATE query
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="values">Values.</param>
        /// <param name="clause">Clause.</param>
        public virtual QueryBuilder Update(string tableName, DataParameter[] values, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.UpdateValues(tableName, values);
            return this.UpdateValues(tableName, values).Where(clause);
        }

        /// <summary>
        /// Builds a SELECT ALL (*) query
        /// </summary>
        /// <returns>The all.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="clause">Clause.</param>
        public virtual QueryBuilder SelectAll(string tableName, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.Select().All().From(tableName);
            return this.Select().All().From(tableName).Where(clause);
        }

        /// <summary>
        /// Builds a SELECT [EXPRESSION] FROM query
        /// </summary>
        /// <returns>The from.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="expression">Expression.</param>
        /// <param name="clause">Clause.</param>
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

        /// <summary>
        /// Gets or sets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public CommandType CommandType
        {
            get
            { return _type; }
            set
            { _type = value; }
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>The command text.</value>
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

        /// <summary>
        /// Builds the command.
        /// </summary>
        /// <returns>The command.</returns>
        public virtual string BuildCommand()
        {
            return _sb.ToString();
        }
    }

}
