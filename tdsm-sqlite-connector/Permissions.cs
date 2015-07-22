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

            _groups.Initialise(this);
            _nodes.Initialise(this);
            _userPerms.Initialise(this);
            _groupPerms.Initialise(this);
            _users.Initialise(this);

            ProgramLog.Error.Log("tdsm.help: " + IsPermitted("tdsm.help", true, "DeathCradle"));
            ProgramLog.Error.Log("tdsm.oplogin: " + IsPermitted("tdsm.oplogin", true, "DeathCradle"));
            ProgramLog.Error.Log("tdsm.exit: " + IsPermitted("tdsm.exit", true, "DeathCradle"));
            ProgramLog.Error.Log("tdsm.exit,a: " + IsPermitted("tdsm.exit", false, "DeathCradle"));
        }

        PermissionNode[] GetPermissionByNodeForUser(long userId, string node)
        {
            if (null == _userPerms.UserNodes || null == _nodes.Nodes)
                return null;
            return (from un in _userPerms.UserNodes
                             join nd in _nodes.Nodes on un.PermissionId equals nd.Id
                             where un.UserId == userId
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
                             where gp.GroupId == groupId
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

        private Permission IsPermitted(string prmNode, bool prmIsGuest, string prmAuthentication = null)
        {
            var vPermissionValue = Permission.Denied;
            var vUserId = 0L;
            var vGroupId = 0L;
            var vPrevGroupId = 0L;
            var vNodeFound = false;

            if (prmIsGuest == false && prmAuthentication != null && prmAuthentication.Length > 0)
            {
                var user = AuthenticatedUsers.GetUser(prmAuthentication);
                if (user != null)
                {
                    vUserId = user.Value.Id;
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
    }
}

