using System;
using System.Linq;
using TDSM.API.Data;
using TDSM.API.Logging;

namespace TDSM.Data.SQLite
{
    public struct UserPermisison
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long PermissionId { get; set; }
    }

    public class UserPermissionsTable : CacheTable
    {
        private UserPermisison[] _data;

        internal UserPermisison[] UserNodes
        {
            get
            { return _data; }
        }

        private class TableDefinition
        {
            public const String TableName = "UserPermissions";

            public static class ColumnNames
            {
                public const String Id = "Id";
                public const String UserId = "UserId";
                public const String PermissionId = "PermissionId";
            }

            public static readonly TableColumn[] Columns = new TableColumn[]
            {
                new TableColumn(ColumnNames.Id, typeof(Int64), true, true),
                new TableColumn(ColumnNames.UserId, typeof(Int64)),
                new TableColumn(ColumnNames.PermissionId, typeof(Int64))
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

        public static long InsertRecord(SQLiteConnector conn, long userId, long permissionId)
        {
            using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                bl.InsertInto(TableDefinition.TableName, 
                    new DataParameter(TableDefinition.ColumnNames.UserId, userId),
                    new DataParameter(TableDefinition.ColumnNames.PermissionId, permissionId)
                );

                return ((IDataConnector)conn).ExecuteInsert(bl);
            }
        }

        public static bool DeleteRecord(SQLiteConnector conn, long userId, long permissionId)
        {
            using (var bl = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                bl.Delete(TableDefinition.TableName,
                    new WhereFilter(TableDefinition.ColumnNames.UserId, userId.ToString()),
                    new WhereFilter(TableDefinition.ColumnNames.PermissionId, permissionId.ToString())
                );

                return ((IDataConnector)conn).ExecuteNonQuery(bl) > 0;
            }
        }

        public bool Delete(SQLiteConnector conn, long userId, long permissionId)
        {
            var res = DeleteRecord(conn, userId, permissionId);

            //Alternatively we could reload, but this shouldn't be called often
            if (res)
            {
                _data = _data.Where(x => x.UserId != userId || x.PermissionId != permissionId).ToArray();
            }

            return res;
        }

        public long Insert(SQLiteConnector conn, long userId, long permissionId)
        {
            var id = InsertRecord(conn, userId, permissionId);

            //Alternatively we could reload, but this shouldn't be called often
            if (id > 0L)
            {
                if (_data == null)
                    _data = new TDSM.Data.SQLite.UserPermisison[0];
                Array.Resize(ref _data, _data.Length + 1);
                _data[_data.Length - 1] = new UserPermisison()
                {
                    Id = id,
                    UserId = userId,
                    PermissionId = permissionId
                };
            }

            return id;
        }

        public void Initialise(SQLiteConnector conn)
        {
            if (!TableDefinition.Exists(conn))
            {
                ProgramLog.Admin.Log("User permissions table does not exist and will now be created");
                TableDefinition.Create(conn);
            }

            this.Load(conn);
        }

        public override void Load(IDataConnector conn)
        {
            using (var sb = new SQLiteQueryBuilder(Plugin.SQLSafeName))
            {
                sb.SelectAll(TableDefinition.TableName);

                _data = conn.ExecuteArray<UserPermisison>(sb);
            }

//            ProgramLog.Error.Log(this.GetType().Name + ": " + (_data == null ? "NULL" : _data.Length.ToString()));
        }

        public override void Save(IDataConnector conn)
        {
            throw new NotImplementedException();
        }
    }
}

