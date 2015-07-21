using System;
using TDSM.API.Data;
using TDSM.API;

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
        }

        private Permission IsPermitted(string prmNode, bool prmIsGuest, string prmAuthentication = null)
        {
            var vPermissionValue = 0;
            var vUserId = 0;
            var vGroupId = 0;
            var vPrevGroupId = 0;
            var vNodeFound = 0;
            /*
                PermissionEnum values:
                    0   = Denied
                    1   = Permitted
            */

            if (prmIsGuest == false && prmAuthentication != null && prmAuthentication.Length > 0)
            {
                //vUserId = GetUserIfByAuth(prmAuthentication)

                if (vUserId > 0)
                {
                    /*
                        If the user has specific nodes then use them
                        If not then search for a group
                        If still none then try the guest permissions
                    */

                    /*

                    / *Do we have any nodes?* /
                if exists
                (
                    select 1
                    from tdsm_users u
                    left join SqlPermissions_UserPermissions up on u.Id = up.UserId
                    left join SqlPermissions_Permissions nd on up.PermissionId = nd.Id
                    where u.Id = vUserId
                    and nd.Node = prmNode
                ) then
                    if exists
                    (
                        select 1
                        from tdsm_users u
                        left join SqlPermissions_UserPermissions up on u.Id = up.UserId
                        left join SqlPermissions_Permissions nd on up.PermissionId = nd.Id
                        where u.Id = vUserId
                        and nd.Node = prmNode
                        and nd.Deny = 0
                    ) then
                        set vPermissionValue = 1;
                        set vNodeFound = 1;
                    else
                        set vPermissionValue = 0;
                        set vNodeFound = 1;
                    end if;
                else
                    / *
                        For each group, see if it has a permission
                        Else, if it has a parent recheck.
                        Else guestMode
                    * /

                    / * Get the first group * /
                    select GroupId
                    from SqlPermissions_Users u
                        where u.UserId = vUserId
                        limit 1
                        into vGroupId;

                    set vPrevGroupId = vGroupId;
                    set vNodeFound = 0;

                while (vGroupId is not null and vGroupId > 0 and vNodeFound = 0) do
                    / * Check group permissions * /
                    select vGroupId;
                if exists
                    (
                        select 1
                        from SqlPermissions_GroupPermissions gp
                        left join SqlPermissions_Permissions pm on gp.PermissionId = pm.Id
                        where gp.GroupId = vGroupId
                        and pm.Node = prmNode
                        and pm.Deny = 0
                    ) then
                    set vPermissionValue = 1;
                set vGroupId = 0;
                set vNodeFound = 1;
                elseif exists
                    (
                        select 1
                        from SqlPermissions_GroupPermissions gp
                        left join SqlPermissions_Permissions pm on gp.PermissionId = pm.Id
                        where gp.GroupId = vGroupId
                        and pm.Node = prmNode
                        and pm.Deny = 1
                    ) then
                    set vPermissionValue = 0;
                set vGroupId = 0;
                set vNodeFound = 1;
                else
                    select Id
                    from SqlPermissions_Groups g
                        where g.Name = (
                            select c.Parent
                            from SqlPermissions_Groups c
                            where c.Id = vGroupId
                            limit 1
                        )
                        limit 1
                        into vGroupId;

                if vPrevGroupId = vGroupId then
                        set vGroupId = 0;
                end if;

                set vPrevGroupId = vGroupId;
                end if;
                end while;

                if 1 <> vNodeFound then
                    set prmIsGuest = 1;
                end if;
                    end if;
                    */
                }
                else
                {
                    /* Invalid user - try guest */
                    prmIsGuest = true;
                }
            }

            /*
            if vNodeFound = 0 and prmIsGuest = 1 then
                    if exists
                        (
                            select 1
                            from SqlPermissions_Groups gr
                            left join SqlPermissions_GroupPermissions gp on gr.Id = gp.GroupId
                            left join SqlPermissions_Permissions nd on gp.PermissionId = nd.Id
                            where gr.ApplyToGuests = 1
                            and nd.Node = prmNode
                            and nd.Deny = 0
                        ) then
                        set vPermissionValue = 1;
                    set vNodeFound = 1;
            end if;
            end if;

            select vPermissionValue PermissionEnum;
           */

            return Permission.Denied;
        }
    }
}

