using System;
using TDSM.API.Data;
using TDSM.API;
using TDSM.Data.MySQL.Tables;

namespace TDSM.Data.MySQL
{
    public partial class MySQLConnector
    {
        private GroupTable _groups;
        private PermissionTable _nodes;
        private GroupPermissions _groupPerms;
        private UserPermissions _userPerms;
        private UsersTable _users;

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
            _users = new UsersTable();

            _groups.Initialise(this);
            _nodes.Initialise(this);
            _userPerms.Initialise(this);
            _groupPerms.Initialise(this);
            _users.Initialise(this);
        }

        private Permission IsPermitted(string node, bool isGuest, string authentication = null)
        {
            //sql routine
            return Permission.Denied;
        }
    }
}

