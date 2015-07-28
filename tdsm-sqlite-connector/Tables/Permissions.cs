using System;
using TDSM.API.Data;
using TDSM.API.Logging;
using System.Linq;

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

        public static long InsertRecord(SQLiteConnector conn, string node, bool deny)
        {
            using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                bl.InsertInto(TableDefinition.TableName, 
                    new DataParameter(TableDefinition.ColumnNames.Node, node),
                    new DataParameter(TableDefinition.ColumnNames.Deny, deny)
                );

                return ((IDataConnector)conn).ExecuteInsert(bl);
            }
        }

        public long Insert(SQLiteConnector conn, string node, bool deny)
        {
            var id = InsertRecord(conn, node, deny);

            //Alternatively we could reload, but this shouldn't be called often
            if (id > 0L)
            {
                Array.Resize(ref _data, _data.Length + 1);
                _data[_data.Length - 1] = new PermissionNode()
                {
                    Id = id,
                    Node = node,
                    Deny = deny
                };
            }

            return id;
        }

        public PermissionNode? Find(string node, bool deny)
        {
            var matches = _data.Where(x => x.Node == node && x.Deny == deny).ToArray();
            if (matches.Length > 0)
                return matches[0];

            return null;
        }

        public long FindOrCreate(SQLiteConnector conn, string node, bool deny)
        {
            var existing = Find(node, deny);
            if (existing == null)
            {
                return Insert(conn, node, deny);
            }
            return existing.Value.Id;
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

//            ProgramLog.Error.Log(this.GetType().Name + ": " + (_data == null ? "NULL" : _data.Length.ToString()));
        }

        public override void Save(IDataConnector conn)
        {
            throw new NotImplementedException();
        }
    }
}

