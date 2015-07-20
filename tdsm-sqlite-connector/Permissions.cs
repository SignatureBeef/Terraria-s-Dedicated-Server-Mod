using System;
using TDSM.API.Data;
using TDSM.API;

namespace TDSM.Data.SQLite
{
    public partial class SQLiteConnector
    {
        private GroupTable _groups;
        private PermissionTable _nodes;
        private GroupPermissions _groupPerms;
        private UserPermissions _userPerms;
        private UserGroupsTable _users;

        Permission IPermissionHandler.IsPermitted(string node, BasePlayer player)
        {
//            if (player != null)
//            {
//                if (player.AuthenticatedAs != null)
//                    return IsPermitted(node, false, player.AuthenticatedAs);
//
//                return IsPermitted(node, true);
//            }

            return Permission.Denied;
        }

        void InitialisePermissions()
        {
            _groups = new GroupTable();
            _nodes = new PermissionTable();
            _userPerms = new UserPermissions();
            _groupPerms = new GroupPermissions();
            _users = new UserGroupsTable();

            _groups.Initialise(this);
            _nodes.Initialise(this);
            _userPerms.Initialise(this);
            _groupPerms.Initialise(this);
            _users.Initialise(this);
        }

//        private Permission IsPermitted(string node, bool isGuest, string authentication = null)
//        {
//            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
//            {
//                sb.ExecuteProcedure("SqlPermissions_IsPermitted", "prm", 
//                    new DataParameter("Node", node),
//                    new DataParameter("IsGuest", isGuest),
//                    new DataParameter("Authentication", authentication)
//                );
//
//                return (Permission)Storage.ExecuteScalar<Int32>(sb);
//            }
//        }
    }
}

