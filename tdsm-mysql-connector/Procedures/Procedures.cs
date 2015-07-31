//#define TESTING
using System;
using TDSM.API.Data;
using System.Linq;
using TDSM.API.Logging;

namespace TDSM.Data.MySQL
{
    public static class Procedures
    {
        public const String AddGroupNode = "AddGroupNode";
        public const String AddNodeToUser = "AddNodeToUser";
        public const String AddOrUpdateGroup = "AddOrUpdateGroup";
        public const String AddUserToGroup = "AddUserToGroup";
        public const String FindGroup = "FindGroup";
        public const String GroupNodes = "GroupNodes";
        public const String GroupList = "GroupList";
        public const String IsPermitted = "IsPermitted";
        public const String RemoveGroup = "RemoveGroup";
        public const String RemoveGroupNode = "RemoveGroupNode";
        public const String RemoveNodeFromUser = "RemoveNodeFromUser";
        public const String RemoveUserFromGroup = "RemoveUserFromGroup";
        public const String UserGroupList = "UserGroupList";
        public const String UserNodes = "UserNodes";

        static StoredProcedure[] _procedures = new StoredProcedure[]
        {
            new StoredProcedure(AddGroupNode),
            new StoredProcedure(AddNodeToUser),
            new StoredProcedure(AddOrUpdateGroup),
            new StoredProcedure(AddUserToGroup),
            new StoredProcedure(FindGroup),
            new StoredProcedure(GroupList),
            new StoredProcedure(GroupNodes),
            new StoredProcedure(IsPermitted),
            new StoredProcedure(RemoveGroup),
            new StoredProcedure(RemoveGroupNode),
            new StoredProcedure(RemoveNodeFromUser),
            new StoredProcedure(RemoveUserFromGroup),
            new StoredProcedure(UserGroupList),
            new StoredProcedure(UserNodes)
        };

        public static void Init(MySQLConnector conn)
        {
            foreach (var proc in _procedures)
            {
                #if TESTING
                if (proc.Exists(conn))
                    proc.Drop(conn);
                #endif
                if (!proc.Exists(conn))
                {
                    ProgramLog.Admin.Log("{0} procedure does not exist and will now be created", proc.Name);
                    proc.Create(conn);
                }
            }
            _procedures = null;
        }
    }

    public class StoredProcedure
    {
        public string Name { get; set; }

        public StoredProcedure(string name)
        {
            this.Name = name;
        }

        public bool Drop(MySQLConnector conn)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ProcedureDrop(Name);

                return ((IDataConnector)conn).Execute(sb);
            }
        }

        public bool Exists(MySQLConnector conn)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ProcedureExists(Name);

                return ((IDataConnector)conn).Execute(sb);
            }
        }

        public bool Create(MySQLConnector conn)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                var proc = PluginContent.GetResource("TDSM.Data.MySQL.Procedures.Files." + Name + ".sql");

                sb.CommandType = System.Data.CommandType.Text;
                sb.CommandText = proc;

                return ((IDataConnector)conn).Execute(sb);
            }
        }
    }
}

