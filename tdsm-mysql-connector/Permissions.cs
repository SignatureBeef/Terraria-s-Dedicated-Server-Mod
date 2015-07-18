using System;
using TDSM.API.Data;
using TDSM.API;
using TDSM.Data.MySQL.Tables;

namespace TDSM.Data.MySQL
{
    public partial class MySQLConnector
    {
        private GroupTable _groups;
        private NodeTable _nodes;

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
            _nodes = new NodeTable();

            _groups.Initialise(this);
            _nodes.Initialise(this);
        }

        private Permission IsPermitted(string node, bool isGuest, string authentication = null)
        {
            //Login in a sql routine
            return Permission.Denied;
        }
    }
}

