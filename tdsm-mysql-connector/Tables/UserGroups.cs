using System;
using TDSM.API.Data;
using TDSM.API.Logging;

namespace TDSM.Data.MySQL
{
    public class UserGroupsTable
    //Essentially an extension to the tdsm_user table
    //However, it's not tied into tdsm, rather it's specific to this plugin
    {
        private class TableDefinition
        {
            public const String TableName = "UserGroups";

            public static class ColumnNames
            {
                public const String Id = "Id";
                public const String UserId = "UserId";
                public const String GroupId = "GroupId";
            }

            public static readonly TableColumn[] Columns = new TableColumn[]
            {
                new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
                new TableColumn(ColumnNames.UserId, typeof(Int32)),
                new TableColumn(ColumnNames.GroupId, typeof(Int32))
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

        public void Initialise(MySQLConnector conn)
        {
            if (!TableDefinition.Exists(conn))
            {
                ProgramLog.Admin.Log("Permission user table does not exist and will now be created");
                TableDefinition.Create(conn);
            }
        }
    }
}

