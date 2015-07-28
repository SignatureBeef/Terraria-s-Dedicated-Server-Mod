using System;
using TDSM.API.Data;
using TDSM.API.Logging;
using System.Linq;

namespace TDSM.Data.SQLite
{
    public struct UserGroup
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long GroupId { get; set; }
    }

    public class UserGroupsTable : CacheTable
    //Essentially an extension to the tdsm_user table
    //However, it's not tied into tdsm, rather it's specific to this plugin
    {
        private UserGroup[] _data;

        internal UserGroup[] UserGroup
        {
            get
            { return _data; }
        }

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
                new TableColumn(ColumnNames.Id, typeof(Int64), true, true),
                new TableColumn(ColumnNames.UserId, typeof(Int64)),
                new TableColumn(ColumnNames.GroupId, typeof(Int64))
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

        public static long CreateLink(SQLiteConnector conn, long userId, long groupId)
        {
            using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                bl.InsertInto(TableDefinition.TableName, 
                    new DataParameter(TableDefinition.ColumnNames.UserId, userId),
                    new DataParameter(TableDefinition.ColumnNames.GroupId, groupId)
                );

                return ((IDataConnector)conn).ExecuteInsert(bl);
            }
        }

        public long Insert(SQLiteConnector conn, long userId, long groupId)
        {
            var id = CreateLink(conn, userId, groupId);

            //Alternatively we could reload, but this shouldn't be called often
            if (id > 0L)
            {
                if (_data == null)
                    _data = new TDSM.Data.SQLite.UserGroup[0];
                Array.Resize(ref _data, _data.Length + 1);
                _data[_data.Length - 1] = new TDSM.Data.SQLite.UserGroup()
                {
                    Id = id,
                    UserId = userId,
                    GroupId = groupId
                };
            }

            return id;
        }

        public static bool DeleteLink(SQLiteConnector conn, long userId, long groupId)
        {
            using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                bl.Delete(TableDefinition.TableName, 
                    new WhereFilter(TableDefinition.ColumnNames.UserId, userId.ToString()), 
                    new WhereFilter(TableDefinition.ColumnNames.GroupId, groupId.ToString()));

                return ((IDataConnector)conn).ExecuteNonQuery(bl) > 0;
            }
        }

        public bool Delete(SQLiteConnector conn, long userId, long groupId)
        {
            var res = DeleteLink(conn, userId, groupId);

            //Alternatively we could reload, but this shouldn't be called often
            if (res)
            {
                _data = _data.Where(x => x.UserId != userId || x.GroupId != groupId).ToArray();
            }

            return res;
        }

        public void Initialise(SQLiteConnector conn)
        {
            if (!TableDefinition.Exists(conn))
            {
                ProgramLog.Admin.Log("Permission user table does not exist and will now be created");
                TableDefinition.Create(conn);
            }

            this.Load(conn);
        }

        public override void Load(IDataConnector conn)
        {
            using (var sb = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                sb.SelectAll(TableDefinition.TableName);

                _data = conn.ExecuteArray<UserGroup>(sb);
            }

//            ProgramLog.Error.Log(this.GetType().Name + ": " + (_data == null ? "NULL" : _data.Length.ToString()));
        }

        public override void Save(IDataConnector conn)
        {
            throw new NotImplementedException();
        }
    }
}

