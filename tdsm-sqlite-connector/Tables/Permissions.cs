using System;
using TDSM.API.Data;
using TDSM.API.Logging;

namespace TDSM.Data.SQLite
{
    public struct PermissionNode
    {
        public long Id { get; set; }

        public string Node { get; set; }

        public bool Deny { get; set; }
    }

    public class PermissionTable : CacheTable
    {
        private PermissionNode[] _data;

        internal PermissionNode[] Nodes
        {
            get
            { return _data; }
        }

        private class TableDefinition
        {
            public const String TableName = "Permissions";

            public static class ColumnNames
            {
                public const String Id = "Id";
                public const String Node = "Node";
                public const String Deny = "Deny";
            }

            public static readonly TableColumn[] Columns = new TableColumn[]
            {
                new TableColumn(ColumnNames.Id, typeof(Int64), true, true),
                new TableColumn(ColumnNames.Node, typeof(String), 255),
                new TableColumn(ColumnNames.Deny, typeof(Boolean))
            };

            public static bool Exists(SQLiteConnector conn)
            {
                using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
                {
                    bl.TableExists(TableName);

                    return ((IDataConnector)conn).Execute(bl);
                }
            }

            public static bool Create(SQLiteConnector conn)
            {
                using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
                {
                    bl.TableCreate(TableName, Columns);

                    return ((IDataConnector)conn).ExecuteNonQuery(bl) > 0;
                }
            }
        }

        public void Initialise(SQLiteConnector conn)
        {
            if (!TableDefinition.Exists(conn))
            {
                ProgramLog.Admin.Log("Permission node table does not exist and will now be created");
                TableDefinition.Create(conn);
            }

            this.Load(conn);
        }

        public override void Load(IDataConnector conn)
        {
            using (var sb = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                sb.SelectAll(TableDefinition.TableName);

                _data = conn.ExecuteArray<PermissionNode>(sb);
            }

            ProgramLog.Error.Log(this.GetType().Name + ": " + (_data == null ? "NULL" : _data.Length.ToString()));
        }

        public override void Save(IDataConnector conn)
        {
            throw new NotImplementedException();
        }
    }
}

