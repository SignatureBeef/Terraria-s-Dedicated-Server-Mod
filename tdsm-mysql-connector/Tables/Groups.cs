using System;
using TDSM.API.Data;
using TDSM.API.Logging;
using TDSM.API.Command;
using System.Linq;

namespace TDSM.Data.MySQL.Tables
{
    public class GroupTable
    {
        private class TableDefinition
        {
            public const String TableName = "Groups";

            public static class ColumnNames
            {
                public const String Id = "Id";
                public const String Name = "Name";
                public const String ApplyToGuests = "ApplyToGuests";
                public const String Parent = "Parent";
                public const String Chat_Red = "Chat_Red";
                public const String Chat_Green = "Chat_Green";
                public const String Chat_Blue = "Chat_Blue";
                public const String Chat_Prefix = "Chat_Prefix";
                public const String Chat_Suffix = "Chat_Suffix";
            }

            // `id` int(11) NOT NULL AUTO_INCREMENT,
            // `name` varchar(255) NOT NULL,
            // `parent` int(255) DEFAULT NULL,
            // `chat_color` varchar(11) DEFAULT NULL,
            // `chat_prefix` varchar(255) DEFAULT NULL,
            // `chat_suffix` varchar(255) DEFAULT NULL,
            // PRIMARY KEY(`id`),
            // KEY `restrict_group_parent` (`parent`),
            // CONSTRAINT `restrict_group_parent` FOREIGN KEY(`parent`) REFERENCES `Restrict_Group` (`id`) ON DELETE SET NULL
            //) ENGINE=InnoDB AUTO_INCREMENT = 3 DEFAULT CHARSET = latin1;

            public static readonly TableColumn[] Columns = new TableColumn[]
            {
                new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
                new TableColumn(ColumnNames.Name, typeof(String), 255),
                new TableColumn(ColumnNames.ApplyToGuests, typeof(Boolean)),
                new TableColumn(ColumnNames.Parent, typeof(String), 255, true),
                new TableColumn(ColumnNames.Chat_Red, typeof(Byte)),
                new TableColumn(ColumnNames.Chat_Green, typeof(Byte)),
                new TableColumn(ColumnNames.Chat_Blue, typeof(Byte)),
                new TableColumn(ColumnNames.Chat_Prefix, typeof(String), 10, true),
                new TableColumn(ColumnNames.Chat_Suffix, typeof(String), 10, true)
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
                try
                {
                    using (var bl = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
                    {
                        bl.TableCreate(TableName, Columns);

                        ((IDataConnector)conn).ExecuteNonQuery(bl);
                    }

                    //Set defaults
                    var pc = CommandParser.GetAvailableCommands(AccessLevel.PLAYER);
                    var ad = CommandParser.GetAvailableCommands(AccessLevel.OP);
                    var op = CommandParser.GetAvailableCommands(AccessLevel.CONSOLE); //Funny how these have now changed

                    CreateGroup("Guest", true, null, 255, 255, 255, pc
                        .Where(x => !String.IsNullOrEmpty(x.Value.Node))
                        .Select(x => x.Value.Node)
                        .Distinct()
                        .ToArray(), conn);
                    CreateGroup("Admin", false, "Guest", 240, 131, 77, ad
                        .Where(x => !String.IsNullOrEmpty(x.Value.Node))
                        .Select(x => x.Value.Node)
                        .Distinct()
                        .ToArray(), conn);
                    CreateGroup("Operator", false, "Admin", 77, 166, 240, op
                        .Where(x => !String.IsNullOrEmpty(x.Value.Node))
                        .Select(x => x.Value.Node)
                        .Distinct()
                        .ToArray(), conn);

                    return true;
                }
                catch (Exception e)
                {
                    ProgramLog.Log(e);
                    return false;
                }
            }

            static void CreateGroup(string name, bool guest, string parent, byte r, byte g, byte b, string[] nodes, MySQLConnector conn)
            {
                long id;
                using (var bl = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
                {
                    bl.InsertInto(TableName, 
                        new DataParameter(ColumnNames.Name, name),
                        new DataParameter(ColumnNames.ApplyToGuests, guest),
                        new DataParameter(ColumnNames.Parent, parent),
                        new DataParameter(ColumnNames.Chat_Red, r),
                        new DataParameter(ColumnNames.Chat_Green, g),
                        new DataParameter(ColumnNames.Chat_Blue, b)
                    );

                    id = ((IDataConnector)conn).ExecuteInsert(bl);
                }

                foreach (var nd in nodes)
                {
                    var nodeId = PermissionTable.Insert(conn, nd, false);
                    GroupPermissions.Insert(conn, id, nodeId);
                }
            }
        }

        public void Initialise(MySQLConnector conn)
        {
            if (!TableDefinition.Exists(conn))
            {
                ProgramLog.Admin.Log("Group table does not exist and will now be created");
                TableDefinition.Create(conn);
            }
        }
    }
}

