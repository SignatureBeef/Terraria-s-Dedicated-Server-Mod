using System;
using TDSM.API.Data;
using TDSM.API;
using TDSM.Data.MySQL.Tables;
using System.IO;
using TDSM.API.Logging;

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
            if (!Procedures.IsPermitted.Exists(this))
            {
                ProgramLog.Admin.Log("Permission procedure does not exist and will now be created");
                Procedures.IsPermitted.Create(this);
            }
        }

        private Permission IsPermitted(string node, bool isGuest, string authentication = null)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure("SqlPermissions_IsPermitted", "prm", 
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
            return null;
        }

        bool IPermissionHandler.AddOrUpdateGroup(string name, bool applyToGuests = false, string parent = null, byte r = 255, byte g = 255, byte b = 255, string prefix = null, string suffix = null)
        {   
            return false;
        }

        bool IPermissionHandler.RemoveGroup(string name)
        {
            return false;
        }

        bool IPermissionHandler.AddGroupNode(string groupName, string node, bool deny = false)
        {
            return false;
        }

        bool IPermissionHandler.RemoveGroupNode(string groupName, string node, bool deny = false)
        {
            return false;
        }

        string[] IPermissionHandler.GroupList()
        {
            return null;
        }

        TDSM.API.Data.PermissionNode[] IPermissionHandler.GroupNodes(string groupName)
        {
            return null;
        }

        bool IPermissionHandler.AddUserToGroup(string username, string groupName)
        {
            return false;
        }

        bool IPermissionHandler.RemoveUserFromGroup(string username, string groupName)
        {
            return false;
        }

        bool IPermissionHandler.AddNodeToUser(string username, string node, bool deny = false)
        {
            return false;
        }

        bool IPermissionHandler.RemoveNodeFromUser(string username, string node, bool deny = false)
        {
            return false;
        }

        string[] IPermissionHandler.UserGroupList(string username)
        {
            return null;
        }

        TDSM.API.Data.PermissionNode[] IPermissionHandler.UserNodes(string username)
        {
            return null;
        }
    }
}

