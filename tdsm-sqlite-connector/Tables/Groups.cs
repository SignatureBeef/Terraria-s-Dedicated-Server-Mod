using System;
using TDSM.API.Data;
using TDSM.API.Logging;
using TDSM.API.Command;
using System.Linq;

namespace TDSM.Data.SQLite
{
    //    public class Group
    //    {
    //        public long Id { get; set; }
    //
    //        public string Name { get; set; }
    //
    //        public bool ApplyToGuests { get; set; }
    //
    //        public string Parent { get; set; }
    //
    //        public byte Chat_Red { get; set; }
    //
    //        public byte Chat_Green { get; set; }
    //
    //        public byte Chat_Blue { get; set; }
    //
    //        public string Chat_Prefix { get; set; }
    //
    //        public string Chat_Suffix { get; set; }
    //    }

    public class GroupTable : CacheTable
    {
        private Group[] _data;

        internal Group[] Groups
        {
            get
            { return _data; }
        }

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
                new TableColumn(ColumnNames.Id, typeof(Int64), true, true),
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
                try
                {
                    using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
                    {
                        bl.TableCreate(TableName, Columns);

                        ((IDataConnector)conn).ExecuteNonQuery(bl);
                    }

                    //Set defaults
                    var pc = CommandParser.GetAvailableCommands(AccessLevel.PLAYER);
                    var ad = CommandParser.GetAvailableCommands(AccessLevel.OP);
                    var op = CommandParser.GetAvailableCommands(AccessLevel.CONSOLE); //Funny how these have now changed

                    CreateGroup("Guest", true, null, 255, 255, 255, conn, pc
                        .Where(x => !String.IsNullOrEmpty(x.Value.Node))
                        .Select(x => x.Value.Node)
                        .Distinct()
                        .ToArray());
                    CreateGroup("Admin", false, "Guest", 240, 131, 77, conn, ad
                        .Where(x => !String.IsNullOrEmpty(x.Value.Node))
                        .Select(x => x.Value.Node)
                        .Distinct()
                        .ToArray());
                    CreateGroup("Operator", false, "Admin", 77, 166, 240, conn, op
                        .Where(x => !String.IsNullOrEmpty(x.Value.Node))
                        .Select(x => x.Value.Node)
                        .Distinct()
                        .ToArray());

                    return true;
                }
                catch (Exception e)
                {
                    ProgramLog.Log(e);
                    return false;
                }
            }
        }

        public static long CreateGroup(string name, bool guest, string parent, byte r, byte g, byte b, SQLiteConnector conn, string[] nodes = null, string prefix = null, string suffix = null)
        {
            long id;
            using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                bl.InsertInto(TableDefinition.TableName, 
                    new DataParameter(TableDefinition.ColumnNames.Name, name),
                    new DataParameter(TableDefinition.ColumnNames.ApplyToGuests, guest),
                    new DataParameter(TableDefinition.ColumnNames.Parent, parent),
                    new DataParameter(TableDefinition.ColumnNames.Chat_Red, r),
                    new DataParameter(TableDefinition.ColumnNames.Chat_Green, g),
                    new DataParameter(TableDefinition.ColumnNames.Chat_Blue, b),
                    new DataParameter(TableDefinition.ColumnNames.Chat_Prefix, prefix),
                    new DataParameter(TableDefinition.ColumnNames.Chat_Suffix, suffix)
                );

                id = ((IDataConnector)conn).ExecuteInsert(bl);
            }

            if (nodes != null)
                foreach (var nd in nodes)
                {
                    var nodeId = PermissionTable.InsertRecord(conn, nd, false);
                    GroupPermissionsTable.InsertRecord(conn, id, nodeId);
                }

            return id;
        }

        public long Insert(string name, bool guest, string parent, byte r, byte g, byte b, SQLiteConnector conn, string[] nodes = null, string prefix = null, string suffix = null)
        {
            var id = CreateGroup(name, guest, parent, r, g, b, conn, nodes, prefix, suffix);

            //Alternatively we could reload, but this shouldn't be called often
            if (id > 0L)
            {
                if (_data == null)
                    _data = new Group[0];
                Array.Resize(ref _data, _data.Length + 1);
                _data[_data.Length - 1] = new Group()
                {
                    Id = id,
                    Name = name,
                    ApplyToGuests = guest,
                    Parent = parent,
                    Chat_Red = r,
                    Chat_Green = g,
                    Chat_Prefix = prefix,
                    Chat_Suffix = suffix
                };
            }

            return id;
        }

        public static bool UpdateGroup(string name, bool guest, string parent, byte r, byte g, byte b, SQLiteConnector conn, string prefix = null, string suffix = null)
        {
            using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                bl.Update(TableDefinition.TableName, new DataParameter[]
                    {
                        new DataParameter(TableDefinition.ColumnNames.Name, name),
                        new DataParameter(TableDefinition.ColumnNames.ApplyToGuests, guest),
                        new DataParameter(TableDefinition.ColumnNames.Parent, parent),
                        new DataParameter(TableDefinition.ColumnNames.Chat_Red, r),
                        new DataParameter(TableDefinition.ColumnNames.Chat_Green, g),
                        new DataParameter(TableDefinition.ColumnNames.Chat_Blue, b),
                        new DataParameter(TableDefinition.ColumnNames.Chat_Prefix, prefix),
                        new DataParameter(TableDefinition.ColumnNames.Chat_Suffix, suffix)
                    }, new WhereFilter(TableDefinition.ColumnNames.Name, name));

                return ((IDataConnector)conn).ExecuteNonQuery(bl) > 0;
            }
        }

        public static bool DeleteGroup(string name, SQLiteConnector conn)
        {
            using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                bl.Delete(TableDefinition.TableName, new WhereFilter(TableDefinition.ColumnNames.Name, name));

                return ((IDataConnector)conn).ExecuteNonQuery(bl) > 0;
            }
        }

        public bool Delete(string name, SQLiteConnector conn)
        {
            var res = DeleteGroup(name, conn);

            //Alternatively we could reload, but this shouldn't be called often
            if (res)
            {
                _data = _data.Where(x => x.Name != name).ToArray();
            }

            return res;
        }

        public void Initialise(SQLiteConnector conn)
        {
            if (!TableDefinition.Exists(conn))
            {
                ProgramLog.Admin.Log("Group table does not exist and will now be created");
                TableDefinition.Create(conn);
            }

            this.Load(conn);
        }

        public override void Load(IDataConnector conn)
        {
            using (var sb = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                sb.SelectAll(TableDefinition.TableName);

                _data = conn.ExecuteArray<Group>(sb);
            }

//            ProgramLog.Error.Log(this.GetType().Name + ": " + (_data == null ? "NULL" : _data.Length.ToString()));
        }

        public override void Save(IDataConnector conn)
        {
            throw new NotImplementedException();
        }
    }
}

