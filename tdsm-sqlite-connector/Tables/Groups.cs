using System;
using TDSM.API.Data;
using TDSM.API.Logging;

namespace TDSM.Data.SQLite
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
                ProgramLog.Admin.Log("Permission group table does not exist and will now be created");
                TableDefinition.Create(conn);
            }
        }
    }
}

