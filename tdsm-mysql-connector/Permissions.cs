using System;
using TDSM.API.Data;
using TDSM.API;
using TDSM.Data.MySQL.Tables;
using System.IO;
using TDSM.API.Logging;
using System.Linq;

namespace TDSM.Data.MySQL
{
    public partial class MySQLConnector
    {
        private GroupTable _groups;
        private PermissionTable _nodes;
        private GroupPermissions _groupPerms;
        private UserPermissions _userPerms;
        private UserGroupsTable _users;

        Permission IPermissionHandler.IsPermitted(string node, BasePlayer player)
        {
            if (player != null)
            {
                if (player.AuthenticatedAs != null)
                    return IsPermitted(node, false, player.AuthenticatedAs);

                return IsPermitted(node, true);
            }

            return Permission.Denied;
        }

        void InitialisePermissions()
        {
            _groups = new GroupTable();
            _nodes = new PermissionTable();
            _userPerms = new UserPermissions();
            _groupPerms = new GroupPermissions();
            _users = new UserGroupsTable();

            _nodes.Initialise(this);
            _userPerms.Initialise(this);
            _groupPerms.Initialise(this);
            _users.Initialise(this);

            //Used to create default permissions
            _groups.Initialise(this);

            //Initialise procedures
            Procedures.Init(this);
        }

        private Permission IsPermitted(string node, bool isGuest, string authentication = null)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.IsPermitted, "prm", 
                    new DataParameter("Node", node),
                    new DataParameter("IsGuest", isGuest),
                    new DataParameter("Authentication", authentication)
                );

                return (Permission)Storage.ExecuteScalar<Int32>(sb);
            }
        }

        internal IPermissionHandler PermissionsHandler
        {
            get
            {
                return (IPermissionHandler)this;
            }
        }

        Group IPermissionHandler.FindGroup(string name)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.FindGroup, "prm", 
                    new DataParameter("Name", name)
                );

                var arr = Storage.ExecuteArray<Group>(sb);
                if (arr != null && arr.Length > 0)
                    return arr[0];
            }

            return null;
        }

        bool IPermissionHandler.AddOrUpdateGroup(string name, bool applyToGuests, string parent, byte r, byte g, byte b, string prefix, string suffix)
        {   
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.AddOrUpdateGroup, "prm", 
                    new DataParameter("Name", name), 
                    new DataParameter("ApplyToGuests", applyToGuests), 
                    new DataParameter("Parent", parent), 
                    new DataParameter("R", r), 
                    new DataParameter("G", g), 
                    new DataParameter("B", b), 
                    new DataParameter("Prefix", prefix), 
                    new DataParameter("Suffix", suffix)
                );

                return Storage.ExecuteScalar<Int64>(sb) > 0;
            }
        }

        bool IPermissionHandler.RemoveGroup(string name)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.RemoveGroup, "prm", 
                    new DataParameter("Name", name)
                );

                return Storage.ExecuteNonQuery(sb) > 0;
            }
        }

        bool IPermissionHandler.AddGroupNode(string groupName, string node, bool deny)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.AddGroupNode, "prm", 
                    new DataParameter("GroupName", groupName), 
                    new DataParameter("Node", node), 
                    new DataParameter("Deny", deny)
                );

                return Storage.ExecuteScalar<Int64>(sb) > 0;
            }
        }

        bool IPermissionHandler.RemoveGroupNode(string groupName, string node, bool deny)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.RemoveGroupNode, "prm", 
                    new DataParameter("GroupName", groupName), 
                    new DataParameter("Node", node), 
                    new DataParameter("Deny", deny)
                );

                return Storage.ExecuteScalar<Int64>(sb) > 0;
            }
        }

        struct GroupList
        {
            public string Name { get; set; }
        }

        string[] IPermissionHandler.GroupList()
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.GroupList);

                var lst = Storage.ExecuteArray<GroupList>(sb);
                if (lst != null)
                    return lst.Select(x => x.Name).ToArray();
            }
            return null;
        }

        TDSM.API.Data.PermissionNode[] IPermissionHandler.GroupNodes(string groupName)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.GroupNodes, "prm", 
                    new DataParameter("GroupName", groupName)
                );

                return Storage.ExecuteArray<TDSM.API.Data.PermissionNode>(sb);
            }
        }

        bool IPermissionHandler.AddUserToGroup(string username, string groupName)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.AddUserToGroup, "prm", 
                    new DataParameter("UserName", username),
                    new DataParameter("GroupName", groupName)
                );

                return Storage.ExecuteScalar<Int64>(sb) > 0;
            }
        }

        bool IPermissionHandler.RemoveUserFromGroup(string username, string groupName)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.RemoveUserFromGroup, "prm", 
                    new DataParameter("UserName", username),
                    new DataParameter("GroupName", groupName)
                );

                return Storage.ExecuteScalar<Int64>(sb) > 0;
            }
        }

        bool IPermissionHandler.AddNodeToUser(string username, string node, bool deny)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.AddNodeToUser, "prm", 
                    new DataParameter("UserName", username),
                    new DataParameter("Node", node),
                    new DataParameter("Deny", deny)
                );

                return Storage.ExecuteScalar<Int64>(sb) > 0;
            }
        }

        bool IPermissionHandler.RemoveNodeFromUser(string username, string node, bool deny)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.RemoveNodeFromUser, "prm", 
                    new DataParameter("UserName", username),
                    new DataParameter("Node", node),
                    new DataParameter("Deny", deny)
                );

                return Storage.ExecuteScalar<Int64
                    >(sb) > 0;
            }
        }

        struct UserGroupList
        {
            public string Name { get; set; }
        }

        string[] IPermissionHandler.UserGroupList(string username)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.UserGroupList, "prm", 
                    new DataParameter("UserName", username)
                );

                var lst = Storage.ExecuteArray<UserGroupList>(sb);
                if (lst != null)
                    return lst.Select(x => x.Name).ToArray();
            }
            return null;
        }

        TDSM.API.Data.PermissionNode[] IPermissionHandler.UserNodes(string username)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure(Procedures.UserNodes, "prm", 
                    new DataParameter("UserName", username)
                );

                return Storage.ExecuteArray<TDSM.API.Data.PermissionNode>(sb);
            }
        }
    }
}

