using System;
using System.Linq;
using TDSM.API.Data;
using TDSM.API;
using TDSM.API.Logging;

namespace TDSM.Data.SQLite
{
    public partial class SQLiteConnector
    {
        private GroupTable _groups;
        private PermissionTable _nodes;
        private GroupPermissionsTable _groupPerms;
        private UserPermissionsTable _userPerms;
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
            _userPerms = new UserPermissionsTable();
            _groupPerms = new GroupPermissionsTable();
            _users = new UserGroupsTable();

            _nodes.Initialise(this);
            _userPerms.Initialise(this);
            _groupPerms.Initialise(this);
            _users.Initialise(this);

            //Used to create default permissions
            _groups.Initialise(this);

//            try
//            {
//                ProgramLog.Error.Log("tdsm.help: " + IsPermitted("tdsm.help", true, "DeathCradle"));
//                ProgramLog.Error.Log("tdsm.oplogin: " + IsPermitted("tdsm.oplogin", true, "DeathCradle"));
//                ProgramLog.Error.Log("tdsm.exit: " + IsPermitted("tdsm.exit", true, "DeathCradle"));
//                ProgramLog.Error.Log("tdsm.exit,a: " + IsPermitted("tdsm.exit", false, "DeathCradle"));
//            }
//            catch (Exception e)
//            {
//                ProgramLog.Log(e);
//            }
        }

        PermissionNode[] GetPermissionByNodeForUser(long userId, string node)
        {
            if (null == _userPerms.UserNodes || null == _nodes.Nodes)
                return null;
            return (from un in _userPerms.UserNodes
                             join nd in _nodes.Nodes on un.PermissionId equals nd.Id
                             where un.UserId == userId && (nd.Node == node || nd.Node == "*")
                             select nd).ToArray();

            //            select 1
            //            from tdsm_users u
            //            left join SqlPermissions_UserPermissions up on u.Id = up.UserId
            //                left join SqlPermissions_Permissions nd on up.PermissionId = nd.Id
            //                    where u.Id = vUserId
            //                and nd.Node = prmNode
        }

        PermissionNode[] GetPermissionByNodeForGroup(long groupId, string node)
        {
            if (null == _groupPerms.GroupPermissions || null == _nodes.Nodes)
                return null;
            return (from gp in _groupPerms.GroupPermissions
                             join nd in _nodes.Nodes on gp.PermissionId equals nd.Id
                             where gp.GroupId == groupId && (nd.Node == node || nd.Node == "*")
                             select nd).ToArray();

//            select 1
//            from SqlPermissions_GroupPermissions gp
//            left join SqlPermissions_Permissions pm on gp.PermissionId = pm.Id
//                    where gp.GroupId = vGroupId
//                and pm.Node = prmNode
//                and pm.Deny = 0
        }

        long GetUsersGroup(long userId)
        {
            if (null == _users.UserGroup)
                return 0L;
            return (from ug in _users.UserGroup
                             where ug.UserId == userId
                             select ug.GroupId).FirstOrDefault();
        }

        long? GetParentForGroup(long groupId)
        {
            if (null == _groups.Groups)
                return null;
            string parentName = (from grp in _groups.Groups
                                          where grp.Id == groupId
                                          select grp.Parent).FirstOrDefault();

            if (String.IsNullOrEmpty(parentName))
                return null;

            var matches = (from grp in _groups.Groups
                                    where grp.Name == parentName
                                    select grp.Id).ToArray();

            if (matches != null && matches.Length > 0)
                return matches[0];

            return null;
        }

        bool GuestGroupsHasNode(string node)
        {
            if (null == _groups.Groups || null == _groupPerms.GroupPermissions || null == _nodes.Nodes)
                return false;
            return (from grp in _groups.Groups
                             join gp in _groupPerms.GroupPermissions on grp.Id equals gp.GroupId
                             join nd in _nodes.Nodes on gp.PermissionId equals nd.Id
                             where (grp.ApplyToGuests && nd.Node == node && !nd.Deny)
                             select 1
            ).Count() > 0;
        }

        UserDetails? GetUser(string username)
        {
            using (var bl = this.GetBuilder(AuthenticatedUsers.SQLSafeName))
            {
                bl.SelectFrom(AuthenticatedUsers.UserTable.TableName, new string[]
                    {
                        AuthenticatedUsers.UserTable.ColumnNames.Id,
                        AuthenticatedUsers.UserTable.ColumnNames.Username,
                        AuthenticatedUsers.UserTable.ColumnNames.Password, 
                        AuthenticatedUsers.UserTable.ColumnNames.Operator 
                    }, new WhereFilter(AuthenticatedUsers.UserTable.ColumnNames.Username, username));

                var res = ((IDataConnector)this).ExecuteArray<UserDetails>(bl);
                if (res != null && res.Length > 0)
                    return res[0];

                return null;
            }
        }

        private Permission IsPermitted(string prmNode, bool prmIsGuest, string prmAuthentication = null)
        {
            var vPermissionValue = Permission.Denied;
            var vUserId = 0L;
            var vGroupId = 0L;
            var vPrevGroupId = 0L;
            var vNodeFound = false;

            if (prmIsGuest == false && prmAuthentication != null && prmAuthentication.Length > 0)
            {
                var user = GetUser(prmAuthentication);
                if (user != null)
                {
                    vUserId = user.Value.Id;

                    if (user.Value.Operator)
                        return  Permission.Permitted;
                }

                if (vUserId > 0)
                {
                    /*
                        If the user has specific nodes then use them
                        If not then search for a group
                        If still none then try the guest permissions
                    */

                    /*Do we have any nodes?*/
                    var nodes = GetPermissionByNodeForUser(vUserId, prmNode);
                    if (nodes != null && nodes.Length > 0)
                    {
                        if (nodes.Where(x => x.Deny).Count() == 0)
                        {
                            vPermissionValue = Permission.Permitted;
                            vNodeFound = true;
                        }
                        else
                        {
                            vPermissionValue = Permission.Denied;
                            vNodeFound = true;
                        }
                    }
                    else
                    {
                        /*
                            For each group, see if it has a permission
                            Else, if it has a parent recheck.
                            Else guestMode
                        */

                        vGroupId = GetUsersGroup(vUserId);
                        if (vGroupId > 0)
                        {
                            vPrevGroupId = vGroupId;
                            vNodeFound = false;

                            while (vGroupId > 0 && !vNodeFound)
                            {
                                /* Check group permissions */

                                var groupPermissions = GetPermissionByNodeForGroup(vGroupId, prmNode);

                                if (groupPermissions.Where(x => x.Deny).Count() > 0)
                                {
                                    vPermissionValue = Permission.Denied;
                                    vGroupId = 0;
                                    vNodeFound = true;
                                }
                                else if (groupPermissions.Where(x => x.Deny).Count() > 0)
                                {
                                    vPermissionValue = Permission.Permitted;
                                    vGroupId = 0;
                                    vNodeFound = true;
                                }
                                else
                                {
                                    var par = GetParentForGroup(vGroupId);
                                    if (par.HasValue)
                                    {
                                        vGroupId = par.Value;
                                        if (vPrevGroupId == vGroupId)
                                        {
                                            vGroupId = 0;
                                        }

                                        vPrevGroupId = vGroupId;
                                    }
                                    else
                                    {
                                        vGroupId = 0;
                                    }
                                }
                            }
                        }
                        if (!vNodeFound)
                        {
                            prmIsGuest = true;
                        }
                    }
                }
                else
                {
                    /* Invalid user - try guest */
                    prmIsGuest = true;
                }
            }

            if (!vNodeFound && prmIsGuest)
            {
                if (GuestGroupsHasNode(prmNode))
                {
                    vPermissionValue = Permission.Permitted;
                    vNodeFound = true;
                }
            }

            return vPermissionValue;
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
            return _groups.Groups.Where(x => x.Name == name).Select(x => x).FirstOrDefault();
        }

        bool IPermissionHandler.AddOrUpdateGroup(string name, bool applyToGuests = false, string parent = null, byte r = 255, byte g = 255, byte b = 255, string prefix = null, string suffix = null)
        {   
            if (PermissionsHandler.FindGroup(name) == null)
            {
                return _groups.Insert(name, applyToGuests, parent, r, g, b, this, null, prefix, suffix) > 0L;
            }
            else
            {
                return GroupTable.UpdateGroup(name, applyToGuests, parent, r, g, b, this, prefix, suffix);
            }
        }

        bool IPermissionHandler.RemoveGroup(string name)
        {
            return _groups.Delete(name, this);
        }

        bool IPermissionHandler.AddGroupNode(string groupName, string node, bool deny = false)
        {
            var grp = PermissionsHandler.FindGroup(groupName);
            var permissionId = _nodes.FindOrCreate(this, node, deny);

            var groupNodeId = _groupPerms.Insert(this, grp.Id, permissionId);

            return groupNodeId > 0L;
        }

        bool IPermissionHandler.RemoveGroupNode(string groupName, string node, bool deny = false)
        {
            var grp = PermissionsHandler.FindGroup(groupName);
            var permission = _nodes.Find(node, deny);

            if (permission != null)
            {
                return _groupPerms.Delete(this, grp.Id, permission.Value.Id);
            }
            return false;
        }

        string[] IPermissionHandler.GroupList()
        {
            return _groups.Groups.Select(x => x.Name).OrderBy(x => x).ToArray();
        }

        TDSM.API.Data.PermissionNode[] IPermissionHandler.GroupNodes(string groupName)
        {
            var grp = PermissionsHandler.FindGroup(groupName);
            return (
                from x in _groupPerms.GroupPermissions
                         join y in _nodes.Nodes on x.PermissionId equals y.Id
                         where x.GroupId == grp.Id
                         select new TDSM.API.Data.PermissionNode()
            {
                Node = y.Node,
                Deny = y.Deny
            }
            ).ToArray();
        }

        bool IPermissionHandler.AddUserToGroup(string username, string groupName)
        {
            var grp = PermissionsHandler.FindGroup(groupName);
            var usr = AuthenticatedUsers.GetUser(username);

            if (usr != null && grp != null)
            {
                return _users.Insert(this, usr.Value.Id, grp.Id) > 0L;
            }
            return false;
        }

        bool IPermissionHandler.RemoveUserFromGroup(string username, string groupName)
        {
            var grp = PermissionsHandler.FindGroup(groupName);
            var usr = AuthenticatedUsers.GetUser(username);

            if (usr != null && grp != null)
            {
                return _users.Delete(this, usr.Value.Id, grp.Id);
            }
            return false;
        }

        bool IPermissionHandler.AddNodeToUser(string username, string node, bool deny = false)
        {
            var usr = AuthenticatedUsers.GetUser(username);
            var permissionId = _nodes.FindOrCreate(this, node, deny);

            var id = _userPerms.Insert(this, usr.Value.Id, permissionId);

            return id > 0L;
        }

        bool IPermissionHandler.RemoveNodeFromUser(string username, string node, bool deny = false)
        {
            var usr = AuthenticatedUsers.GetUser(username);
            var permission = _nodes.Find(node, deny);

            if (permission != null)
            {
                return _userPerms
                    .Delete(this, usr.Value.Id, permission.Value.Id);
            }
            return false;
        }

        string[] IPermissionHandler.UserGroupList(string username)
        {
            var usr = AuthenticatedUsers.GetUser(username);
            return (
                from x in _users.UserGroup
                         join y in _groups.Groups on x.GroupId equals y.Id
                         where x.UserId == usr.Value.Id
                         select y.Name
            ).ToArray();
        }

        TDSM.API.Data.PermissionNode[] IPermissionHandler.UserNodes(string username)
        {
            var usr = AuthenticatedUsers.GetUser(username);
            return (
                from x in _userPerms.UserNodes
                         join y in _nodes.Nodes on x.PermissionId equals y.Id
                         where x.UserId == usr.Value.Id
                         select new TDSM.API.Data.PermissionNode()
            {
                Node = y.Node,
                Deny = y.Deny
            }
            ).ToArray();
        }
    }
}

