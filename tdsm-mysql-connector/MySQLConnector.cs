using System;
using TDSM.API.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using TDSM.API.Logging;
using TDSM.API;

namespace TDSM.Data.MySQL
{
    public struct ProcedureParameter
    {
        public string Name { get; set; }

        public Type DataType { get; set; }

        public int? MinScale { get; set; }

        public int? MaxScale { get; set; }

        public ProcedureParameter(string name, Type dataType, int? minScale = null, int? maxScale = null) : this()
        {
            this.Name = name;

            this.DataType = dataType;

            this.MinScale = minScale;
            this.MaxScale = maxScale;
        }
    }

    public partial class MySQLConnector : IDataConnector
    {
        private MySqlConnection _connection;

        public QueryBuilder GetBuilder(string pluginName)
        {
            return new MySQLQueryBuilder(pluginName);
        }

        public MySQLConnector(string connectionString)
        {
            _connection = new MySqlConnection();
            _connection.ConnectionString = connectionString;
        }

        public void Open()
        {
            _connection.Open();

            InitialisePermissions();
        }

        bool IDataConnector.Execute(QueryBuilder builder)
        {
            if (!(builder is MySQLQueryBuilder))
                throw new InvalidOperationException("MySQLQueryBuilder expected");

            var ms = builder as MySQLQueryBuilder;

            using (builder)
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = builder.BuildCommand();
                cmd.CommandType = builder.CommandType;
                cmd.Parameters.AddRange(ms.Parameters.ToArray());

                using (var rdr = cmd.ExecuteReader())
                {
                    return rdr.HasRows;
                }
            }
        }

        int IDataConnector.ExecuteNonQuery(QueryBuilder builder)
        {
            if (!(builder is MySQLQueryBuilder))
                throw new InvalidOperationException("MySQLQueryBuilder expected");

            var ms = builder as MySQLQueryBuilder;

            using (builder)
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = builder.BuildCommand();
                cmd.CommandType = builder.CommandType;
                cmd.Parameters.AddRange(ms.Parameters.ToArray());

                using (var rdr = cmd.ExecuteReader())
                {
                    return rdr.RecordsAffected;
                }
            }
        }

        long IDataConnector.ExecuteInsert(QueryBuilder builder)
        {
            if (!(builder is MySQLQueryBuilder))
                throw new InvalidOperationException("MySQLQueryBuilder expected");

            var ms = builder as MySQLQueryBuilder;

            using (builder)
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = builder.BuildCommand();
                cmd.CommandType = builder.CommandType;
                cmd.Parameters.AddRange(ms.Parameters.ToArray());

                cmd.ExecuteNonQuery();

                return cmd.LastInsertedId;
            }
        }

        T IDataConnector.ExecuteScalar<T>(QueryBuilder builder)
        {
            if (!(builder is MySQLQueryBuilder))
                throw new InvalidOperationException("MySQLQueryBuilder expected");

            var ms = builder as MySQLQueryBuilder;

            using (builder)
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = builder.BuildCommand();
                cmd.CommandType = builder.CommandType;
                cmd.Parameters.AddRange(ms.Parameters.ToArray());

                return (T)cmd.ExecuteScalar();
            }
        }

        DataSet IDataConnector.ExecuteDataSet(QueryBuilder builder)
        {
            if (!(builder is MySQLQueryBuilder))
                throw new InvalidOperationException("MySQLQueryBuilder expected");

            var ms = builder as MySQLQueryBuilder;

            using (builder)
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = builder.BuildCommand();
                cmd.CommandType = builder.CommandType;
                cmd.Parameters.AddRange(ms.Parameters.ToArray());

                using (var da = new MySqlDataAdapter(cmd))
                {
                    var ds = new DataSet();

                    da.Fill(ds);

                    return ds;
                }
            }
        }

        T[] IDataConnector.ExecuteArray<T>(QueryBuilder builder)
        {
            var ds = (this as IDataConnector).ExecuteDataSet(builder);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var records = new T[ds.Tables[0].Rows.Count];
                var tp = typeof(T);

                for (var x = 0; x < ds.Tables[0].Rows.Count; x++)
                {
                    object boxed = new T();
                    for (var cx = 0; cx < ds.Tables[0].Columns.Count; cx++)
                    {
                        var col = ds.Tables[0].Columns[cx];

                        var val = ds.Tables[0].Rows[x].ItemArray[cx];
                        if (DBNull.Value == val)
                        {
                            continue;
                        }

                        var fld = tp.GetField(col.ColumnName);
                        if (fld != null)
                            fld.SetValue(boxed, val);
                        else
                        {
                            var prop = tp.GetProperty(col.ColumnName);
                            if (prop != null)
                                prop.SetValue(boxed, val, null);
                        }
                    }
                    records[x] = (T)boxed;
                }

                return records;
            }

            return null;
        }

        public override string ToString()
        {
            return "[MySQLConnector]";
        }
    }

    public class MySQLQueryBuilder : QueryBuilder
    {
        private List<MySql.Data.MySqlClient.MySqlParameter> _params;

        public List<MySql.Data.MySqlClient.MySqlParameter> Parameters
        {
            get
            { return _params; }
        }

        public MySQLQueryBuilder(string pluginName)
            : base(pluginName)
        {
            _params = new List<MySql.Data.MySqlClient.MySqlParameter>();
        }

        public QueryBuilder ExecuteProcedure(string name, string prefix = "prm", params DataParameter[] parameters)
        {
            Append("CALL `{0}`(", base.GetObjectName(name));

            if (parameters != null && parameters.Length > 0)
            {
                for (var x = 0; x < parameters.Length; x++)
                {
                    var xp = parameters[x];

                    var paramKey = prefix + xp.Name;
                    _params.Add(new MySqlParameter(paramKey, xp.Value));
                    Append("?");

                    if (x + 1 < parameters.Length)
                        Append(",");
                }
            }

            Append(");");
            return this;
        }

        public override QueryBuilder AddParam(string name, object value, string prefix = "prm")
        {
            var paramKey = prefix + name;
            _params.Add(new MySqlParameter(paramKey, value));
            return this;
        }

        public override QueryBuilder TableExists(string name)
        {
            Append("SHOW TABLES LIKE '{0}'", base.GetObjectName(name));
            return this;
        }

        public override QueryBuilder TableCreate(string name, params TableColumn[] columns)
        {
            Append("CREATE TABLE {0} (", base.GetObjectName(name));

            if (columns != null && columns.Length > 0)
            {
                for (var x = 0; x < columns.Length; x++)
                {
                    var col = columns[x];

                    Append("`");
                    Append(col.Name);
                    Append("`");

                    /* http://dev.mysql.com/doc/refman/5.6/en/integer-types.html */
                    if (col.DataType == typeof(Byte))
                    {
                        Append(" TINYINT UNSIGNED");
                    }
                    else if (col.DataType == typeof(Int16))
                    {
                        Append(" SMALLINT");
                    }
                    else if (col.DataType == typeof(UInt16))
                    {
                        Append(" SMALLINT UNSIGNED");
                    }
                    else if (col.DataType == typeof(Int32))
                    {
                        Append(" INT");
                    }
                    else if (col.DataType == typeof(UInt32))
                    {
                        Append(" INT UNSIGNED");
                    }
                    else if (col.DataType == typeof(Int64))
                    {
                        Append(" BIGINT");
                    }
                    else if (col.DataType == typeof(UInt64))
                    {
                        Append(" BIGINT UNSIGNED");
                    }
                    else if (col.DataType == typeof(String))
                    {
                        var isVarChar = col.MinScale.HasValue && !col.MaxScale.HasValue;
                        if (isVarChar)
                        {
                            Append(" VARCHAR(");
                            Append(col.MinScale.Value.ToString());
                            Append(")");
                        }
                        else
                        {
                            Append(" TEXT");
                        }
                    }
                    else if (col.DataType == typeof(DateTime))
                    {
                        Append(" TIMESTAMP");
                    }
                    else if (col.DataType == typeof(Boolean))
                    {
                        Append(" BOOLEAN");
                    }
                    else
                    {
                        throw new NotSupportedException(String.Format("Data type for column '{0}' is not supported", col.Name));
                    }

                    if (col.AutoIncrement) //TODO check for numerics
                    {
                        Append(" AUTO_INCREMENT");
                    }
                    if (col.PrimaryKey) //TODO check for numerics
                    {
                        Append(" PRIMARY KEY");
                    }
                    if (col.Nullable)
                    {
                        Append(" NULL");
                    }
                    else
                    {
                        Append(" NOT NULL");
                    }

                    if (x + 1 < columns.Length)
                        Append(",");
                }
            }
            Append(")");

            return this;
        }

        public override QueryBuilder TableDrop(string name)
        {
            Append("DROP TABLE IF EXISTS '{0}'", base.GetObjectName(name));
            return this;
        }

        public QueryBuilder ProcedureExists(string name)
        {
            const String Fmt = "select 1 from information_schema.routines where routine_type='procedure' and routine_schema = DATABASE() and routine_name = '{0}';";
            return this.Append(Fmt, base.GetObjectName(name));
        }

        public QueryBuilder ProcedureCreate(string name, string contents, params ProcedureParameter[] parameters)
        {
            Append("CREATE PROCEDURE {0} (", base.GetObjectName(name));

            if (parameters != null && parameters.Length > 0)
            {
                for (var x = 0; x < parameters.Length; x++)
                {
                    var prm = parameters[x];

                    Append("IN `");
                    Append(prm.Name);
                    Append("`");

                    if (prm.DataType == typeof(Byte))
                    {
                        Append(" TINYINT");
                    }
                    else if (prm.DataType == typeof(Int16))
                    {
                        Append(" SMALLINT");
                    }
                    else if (prm.DataType == typeof(Int32))
                    {
                        Append(" INT");
                    }
                    else if (prm.DataType == typeof(Int64))
                    {
                        Append(" BIGINT");
                    }
                    else if (prm.DataType == typeof(String))
                    {
                        var isVarChar = prm.MinScale.HasValue && !prm.MaxScale.HasValue;
                        if (isVarChar)
                        {
                            Append(" VARCHAR(");
                            Append(prm.MinScale.Value.ToString());
                            Append(")");
                        }
                        else
                        {
                            Append(" TEXT");
                        }
                    }
                    else if (prm.DataType == typeof(DateTime))
                    {
                        Append(" TIMESTAMP");
                    }
                    else if (prm.DataType == typeof(Boolean))
                    {
                        Append(" BIT");
                    }
                    else
                    {
                        throw new NotSupportedException(String.Format("Data type for parameter '{0}' is not supported", prm.Name));
                    }

                    if (x + 1 < parameters.Length)
                        Append(",");
                }
            }
            Append(")");

            Append(contents);

            return this;
        }

        public QueryBuilder ProcedureDrop(string name)
        {
            return this.Append("DROP PROCEDURE `{0}`", base.GetObjectName(name));
        }

        public override QueryBuilder Select(params string[] expression)
        {
            Append("SELECT ");

            if (expression != null && expression.Length > 0)
            {
                Append(String.Join(",", expression));

                return this.Append(" ");
            }

            return this;
        }

        public override QueryBuilder All()
        {
            Append("* ");
            return this;
        }

        public override QueryBuilder From(string tableName)
        {
            Append("FROM ");
            Append(base.GetObjectName(tableName));
            Append(" ");
            return this;
        }

        public override QueryBuilder Where(params WhereFilter[] clause)
        {
            Append("WHERE ");

            if (clause != null && clause.Length > 0)
            {
                for (var x = 0; x < clause.Length; x++)
                {
                    if (x > 0)
                        Append("AND ");

                    var xp = clause[x];

                    Append(xp.Column);

                    switch (xp.Expression)
                    {
                        case WhereExpression.EqualTo:
                            Append(" = ");
                            break;
                        case WhereExpression.NotEqualTo:
                            Append(" = ");
                            break;
                        case WhereExpression.Like:
                            Append(" LIKE ");
                            break;
                    }

                    var paramKey = "prm" + xp.Column;
                    _params.Add(new MySqlParameter(paramKey, xp.Value));
                    Append("?");
                    Append(" ");
                }
            }

            return this;
        }

        public override QueryBuilder Count(string expression = null)
        {
            Append("COUNT(");
            Append(expression ?? "*");
            return Append(") ");
            //return this.Append(fmt, String.Format("COUNT({0})", expression ?? "*"));
        }

        public override QueryBuilder Delete()
        {
            Append("DELETE ");
            return this;
        }

        public override QueryBuilder InsertInto(string tableName, params DataParameter[] values)
        {
            Append("INSERT INTO ");
            Append(base.GetObjectName(tableName));

            if (values != null && values.Length > 0)
            {
                //Columns
                Append(" ( ");
                for (var x = 0; x < values.Length; x++)
                {
                    Append(values[x].Name);

                    if (x + 1 < values.Length)
                        Append(",");
                }
                Append(" ) ");

                //Values
                Append(" VALUES ( ");
                for (var x = 0; x < values.Length; x++)
                {
                    var prm = values[x];
                    var paramKey = "prm" + prm.Name;

                    Append("?");
                    if (x + 1 < values.Length)
                        Append(",");

                    _params.Add(new MySqlParameter(paramKey, prm.Value));
                }
                Append(" ) ");
            }
            return this;
        }

        public override QueryBuilder UpdateValues(string tableName, DataParameter[] values)
        {
            Append("UPDATE ");
            Append(base.GetObjectName(tableName));

            if (values != null && values.Length > 0)
            {
                Append(" SET ");

                for (var x = 0; x < values.Length; x++)
                {
                    var prm = values[x];
                    var paramKey = "prm" + prm.Name;

                    Append(prm.Name);
                    Append("=");
                    Append("?");
                    Append(" ");
                    
                    if (x + 1 < values.Length)
                        Append(",");

                    _params.Add(new MySqlParameter(paramKey, prm.Value));
                }
            }

            return this;
        }
    }
}

