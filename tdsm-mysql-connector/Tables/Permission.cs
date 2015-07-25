using System;
using TDSM.API.Data;
using TDSM.API.Logging;

namespace TDSM.Data.MySQL.Tables
{
    public class PermissionTable
    {
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
                new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
                new TableColumn(ColumnNames.Node, typeof(String), 255),
                new TableColumn(ColumnNames.Deny, typeof(Boolean))
            };

            public static bool Exists(MySQLConnector conn)
            {
                using (var bl = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
                {
                    bl.TableExists(TableName);

                    return ((IDataConnector)conn).Execute(bl);
                }
            }

            public static bool Create(MySQLConnector conn)
            {
                using (var bl = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
                {
                    bl.TableCreate(TableName, Columns);

                    return ((IDataConnector)conn).ExecuteNonQuery(bl) > 0;
                }
            }
        }

        public static long Insert(MySQLConnector conn, string node, bool deny)
        {
            using (var bl = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                bl.InsertInto(TableDefinition.TableName, 
                    new DataParameter(TableDefinition.ColumnNames.Node, node),
                    new DataParameter(TableDefinition.ColumnNames.Deny, deny)
                );

                return ((IDataConnector)conn).ExecuteInsert(bl);
            }
        }

        public void Initialise(MySQLConnector conn)
        {
            if (!TableDefinition.Exists(conn))
            {
                ProgramLog.Admin.Log("Permission node table does not exist and will now be created");
                TableDefinition.Create(conn);
            }
        }
    }
}

