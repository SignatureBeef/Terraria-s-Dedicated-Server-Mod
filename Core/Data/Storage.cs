using System;
using OTA;
using TDSM.Core.Data.Models;
using System.Linq;
using TDSM.Core;
using TDSM.Core.Data.Permissions;

namespace TDSM.Core.Data
{
    /// <summary>
    /// Direct access to the active Data Connector.
    /// </summary>
    /// <remarks>Plugins use this</remarks>
    public static class Storage
    {
        //        private static readonly object _sync = new object();
        //        private static IDataConnector _connector;

        /// <summary>
        /// Gets a value indicating if there is a connector available.
        /// </summary>
        /// <value><c>true</c> if is available; otherwise, <c>false</c>.</value>
        public static bool IsAvailable
        {
            internal set;
            get;
            //            get
            //            {
            //                return OTAContext.HasConnection();
            //            }
        }

        /// <summary>
        /// Determines if a player is permitted for a node
        /// </summary>
        /// <returns><c>true</c> if is permitted the specified node player; otherwise, <c>false</c>.</returns>
        /// <param name="node">Node.</param>
        /// <param name="player">Player.</param>
        public static Permission IsPermitted(string node, BasePlayer player)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            if (player != null)
            {
                var auth = player.GetAuthenticatedAs();
                if (auth != null)
                    return IsPermitted(node, false, auth);

                return IsPermitted(node, true);
            }

            return Permission.Denied;
        }

        private static Permission IsPermitted(string prmNode, bool prmIsGuest, string prmAuthentication = null)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            var vPermissionValue = Permission.Denied;
            var vUserId = 0;
            var vGroupId = 0;
            var vPrevGroupId = 0;
            var vNodeFound = false;

            using (var ctx = new TContext())
            {
                if (prmIsGuest == false && prmAuthentication != null && prmAuthentication.Length > 0)
                {
                    var user = ctx.GetUser(prmAuthentication).Single();
                    if (user != null)
                    {
                        vUserId = user.Id;

                        if (user.Operator)
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
                        var nodes = ctx.GetPermissionByNodeForUser(vUserId, prmNode).ToArray();
                        if (nodes != null && nodes.Length > 0)
                        {
                            if (nodes.Where(x => x.Permission == Permission.Denied).Count() == 0)
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

                            var grp = ctx.GetUserGroups(vUserId).FirstOrDefault();
                            vGroupId = 0;
                            if (grp != null) vGroupId = grp.Id;
                            if (vGroupId > 0)
                            {
                                vPrevGroupId = vGroupId;
                                vNodeFound = false;

                                while (vGroupId > 0 && !vNodeFound)
                                {
                                    /* Check group permissions */

                                    var groupPermissions = ctx.GetPermissionByNodeForGroup(vGroupId, prmNode);

                                    if (groupPermissions.Where(x => x.Permission == Permission.Denied).Count() > 0)
                                    {
                                        vPermissionValue = Permission.Denied;
                                        vGroupId = 0;
                                        vNodeFound = true;
                                    }
                                    else if (groupPermissions.Where(x => x.Permission == Permission.Permitted).Count() > 0)
                                    {
                                        vPermissionValue = Permission.Permitted;
                                        vGroupId = 0;
                                        vNodeFound = true;
                                    }
                                    else
                                    {
                                        var par = ctx.GetParentForGroup(vGroupId).FirstOrDefault();
                                        if (par != null)
                                        {
                                            vGroupId = par.Id;
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
                    if (ctx.GuestGroupHasNode(prmNode, Permission.Permitted))
                    {
                        vPermissionValue = Permission.Permitted;
                        vNodeFound = true;
                    }
                }

                return vPermissionValue;
            }
        }

        /// <summary>
        /// Find a group by name
        /// </summary>
        /// <returns>The group.</returns>
        /// <param name="name">Name.</param>
        public static Group FindGroup(string name)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext()) return ctx.Groups.SingleOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Add or update a group
        /// </summary>
        /// <returns><c>true</c>, if the update group was added/updated, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        /// <param name="applyToGuests">If set to <c>true</c>, the group will be applied to guests.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="r">The red chat component.</param>
        /// <param name="g">The green chat component.</param>
        /// <param name="b">The blue chat component.</param>
        /// <param name="prefix">Prefix.</param>
        /// <param name="suffix">Suffix.</param>
        public static Group AddOrUpdateGroup(string name, bool applyToGuests = false, string parent = null, byte r = 255, byte g = 255, byte b = 255, string prefix = null, string suffix = null)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                var group = ctx.Groups.SingleOrDefault(x => x.Name == name);
                if (group != null)
                {
                    group.ApplyToGuests = applyToGuests;
                    group.Parent = parent;
                    group.Chat_Red = r;
                    group.Chat_Green = g;
                    group.Chat_Blue = b;
                    group.Chat_Prefix = prefix;
                    group.Chat_Suffix = suffix;
                }
                else
                {
                    ctx.Groups.Add(group = new Group()
                        {
                            Name = name,
                            ApplyToGuests = applyToGuests,
                            Parent = parent,
                            Chat_Red = r,
                            Chat_Green = g,
                            Chat_Blue = b,
                            Chat_Prefix = prefix,
                            Chat_Suffix = suffix
                        });
                }

                ctx.SaveChanges();

                return group;
            }
        }

        public static NodePermission FindOrCreateNode(string node, Permission permission)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                var existing = ctx.Nodes.SingleOrDefault(x => x.Node == node && x.Permission == permission);
                if (existing != null) return existing;
                else
                {
                    ctx.Nodes.Add(existing = new NodePermission()
                        {
                            Node = node,
                            Permission = permission
                        });

                    ctx.SaveChanges();

                    return existing;
                }
            }
        }

        /// <summary>
        /// Remove a group
        /// </summary>
        /// <returns><c>true</c>, if group was removed, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public static bool RemoveGroup(string name)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                var range = ctx.Groups.RemoveRange(ctx.Groups.Where(x => x.Name == name));
                ctx.SaveChanges();

                return range.Any();
            }
        }

        /// <summary>
        /// Adds a node to a group
        /// </summary>
        /// <returns><c>true</c>, if group node was added, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        /// <param name="node">Node.</param>
        /// <param name="deny">If set to <c>true</c> deny.</param>
        public static bool AddGroupNode(string groupName, string node, Permission permission)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                var group = ctx.Groups.Where(x => x.Name == groupName).SingleOrDefault();
                var perm = FindOrCreateNode(node, permission);

                ctx.GroupNodes.Add(new GroupNode()
                    {
                        GroupId = group.Id,
                        NodeId = perm.Id
                    });

                ctx.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// Removes a node from a group
        /// </summary>
        /// <returns><c>true</c>, if group node was removed, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        /// <param name="node">Node.</param>
        /// <param name="deny">If set to <c>true</c> deny.</param>
        public static bool RemoveGroupNode(string groupName, string node, Permission permission)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                var range = ctx.GroupNodes.RemoveRange(
                                from grp in ctx.Groups
                                               join nds in ctx.GroupNodes on grp.Id equals nds.GroupId
                                               select nds
                            );

                ctx.SaveChanges();

                return range.Any();
            }
        }

        /// <summary>
        /// Fetches the group names available
        /// </summary>
        /// <returns>The list.</returns>
        public static string[] GroupList()
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                return ctx.Groups.Select(x => x.Name).ToArray();
            }
        }

        /// <summary>
        /// Fetches the nodes for a group
        /// </summary>
        /// <returns>The nodes.</returns>
        /// <param name="groupName">Group name.</param>
        public static NodePermission[] GroupNodes(string groupName)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                return ctx.Groups
                    .Where(g => g.Name == groupName)
                    .Join(ctx.GroupNodes, grp => grp.Id, gn => gn.GroupId, (a, b) => b)
                    .Join(ctx.Nodes, gp => gp.Id, nd => nd.Id, (a, b) => b)
                    .ToArray();
            }
        }

        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <returns><c>true</c>, if user to group was added, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="groupName">Group name.</param>
        public static bool AddUserToGroup(string username, string groupName)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                var user = ctx.Players.Single(x => x.Name == username);
                var group = ctx.Groups.Single(x => x.Name == groupName);

                //Temporary until the need for more than one group
                if (ctx.PlayerGroups.Any(x => x.GroupId > 0))
                    throw new NotSupportedException("A player can only be associated to one group, please assign a parent to the desired group");

                ctx.PlayerGroups.Add(new PlayerGroup()
                    {
                        GroupId = group.Id,
                        UserId = user.Id
                    });

                ctx.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// Removes a player from a group
        /// </summary>
        /// <returns><c>true</c>, if user from group was removed, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="groupName">Group name.</param>
        public static bool RemoveUserFromGroup(string username, string groupName)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                var user = ctx.Players.Single(x => x.Name == username);
                var group = ctx.Groups.Single(x => x.Name == groupName);

                var range = ctx.PlayerGroups.RemoveRange(ctx.PlayerGroups.Where(x =>
                    x.GroupId == group.Id &&
                                    x.UserId == user.Id
                                ));

                ctx.SaveChanges();

                return range.Any();
            }
        }

        /// <summary>
        /// Adds a specific node to a user
        /// </summary>
        /// <returns><c>true</c>, if node to user was added, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="node">Node.</param>
        /// <param name="deny">If set to <c>true</c> deny.</param>
        public static bool AddNodeToUser(string username, string node, Permission permission)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                var user = ctx.Players.Single(x => x.Name == username);
                var perm = FindOrCreateNode(node, permission);

                var range = ctx.PlayerNodes.RemoveRange(ctx.PlayerNodes.Where(x =>
                    x.NodeId == perm.Id &&
                                    x.UserId == user.Id
                                ));

                ctx.SaveChanges();

                return range.Any();
            }
        }

        /// <summary>
        /// Removes a specific node from a user
        /// </summary>
        /// <returns><c>true</c>, if node from user was removed, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="node">Node.</param>
        /// <param name="deny">If set to <c>true</c> deny.</param>
        public static bool RemoveNodeFromUser(string username, string node, Permission permission)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                var range = ctx.PlayerNodes.RemoveRange(
                                ctx.Players
                    .Where(p => p.Name == username)
                    .Join(ctx.PlayerNodes, x => x.Id, y => y.UserId, (a, b) => b)
                            );

                ctx.SaveChanges();

                return range.Any();
            }
        }

        /// <summary>
        /// Fetches the associated groups names for a user
        /// </summary>
        /// <returns>The group list.</returns>
        /// <param name="username">Username.</param>
        public static string[] UserGroupList(string username)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                return ctx.Players
                    .Where(p => p.Name == username)
                    .Join(ctx.PlayerGroups, pg => pg.Id, y => y.UserId, (a, b) => b)
                    .Join(ctx.Groups, pg => pg.Id, g => g.Id, (a, b) => b)
                    .Select(x => x.Name)
                    .ToArray();
            }
        }

        /// <summary>
        /// Fetches the list of nodes for a user
        /// </summary>
        /// <returns>The nodes.</returns>
        /// <param name="username">Username.</param>
        public static NodePermission[] UserNodes(string username)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                return ctx.Players
                    .Where(p => p.Name == username)
                    .Join(ctx.PlayerNodes, pn => pn.Id, y => y.UserId, (a, b) => b)
                    .Join(ctx.Nodes, pn => pn.Id, nd => nd.Id, (a, b) => b)
                    .ToArray();
            }
        }

        /// <summary>
        /// Fetches the lower most group for a player
        /// </summary>
        /// <remarks>There should always be one at this stage in OTA. The flexibility is just here.</remarks>
        /// <returns>The inherited group for user.</returns>
        /// <param name="username">Username.</param>
        public static Group GetInheritedGroupForUser(string username)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                return ctx.Players
                    .Where(x => x.Name == username)
                    .Join(ctx.PlayerGroups, pg => pg.Id, us => us.UserId, (a, b) => b)
                    .Join(ctx.Groups, pg => pg.GroupId, gr => gr.Id, (a, b) => b)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the inherited group for a player.
        /// </summary>
        /// <returns>The inherited group for a player.</returns>
        /// <param name="player">Player.</param>
        public static Group GetInheritedGroup(BasePlayer player)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            if (player.IsAuthenticated())
            {
                var grp = GetInheritedGroupForUser(player.name);
                if (grp != null) return grp;
            }

            var guestGroup = GetGuestGroup();
            if (guestGroup != null) return guestGroup;

            return null;
        }

        /// <summary>
        /// Gets the first guest group from the database
        /// </summary>
        /// <returns>The guest group.</returns>
        public static Group GetGuestGroup()
        {
            if (!IsAvailable)
                throw new InvalidOperationException("No connector attached");

            using (var ctx = new TContext())
            {
                return ctx.Groups.Where(x => x.ApplyToGuests).FirstOrDefault();
            }
        }
    }
}

