using System.Linq;
using TDSM.Core.Data.Models;
using OTA.Permissions;
using System.Data;
using OTA.Data.Dapper.Extensions;
using System.Collections.Generic;
using Dapper;

namespace TDSM.Core.Data
{
#if ENTITY_FRAMEWORK_6 || ENTITY_FRAMEWORK_7
    public static class OTAContextExtensions
    {
        public static IQueryable<DbPlayer> GetUser(this TContext ctx, string name)
        {
            return ctx.Players.Where(x => x.Name == name);
        }

        public static IQueryable<Group> GetUserGroups(this TContext ctx, int userId)
        {
            return ctx.Players
                .Where(x => x.Id == userId)
                .Join(ctx.PlayerGroups, pl => pl.Id, pn => pn.UserId, (pl, pg) => pg)
                .Join(ctx.Groups, pn => pn.GroupId, gr => gr.Id, (a, b) => b);
        }

        public static IQueryable<NodePermission> GetPermissionByNodeForUser(this TContext ctx, int userId, string node)
        {
            return ctx.Players
                .Where(x => x.Id == userId)
                .Join(ctx.PlayerNodes, pl => pl.Id, pn => pn.UserId, (pl, pn) => pn)
                .Join(ctx.Nodes, pn => pn.NodeId, np => np.Id, (a, b) => b)
                .Where(n => n.Node == node);
        }

        public static IQueryable<NodePermission> GetPermissionByNodeForGroup(this TContext ctx, int groupId, string node)
        {
            return ctx.Groups
                .Where(x => x.Id == groupId)
                .Join(ctx.GroupNodes, gr => gr.Id, pn => pn.GroupId, (pl, gn) => gn)
                .Join(ctx.Nodes, pn => pn.NodeId, np => np.Id, (a, b) => b)
                .Where(n => n.Node == node);
        }

        public static IQueryable<Group> GetParentForGroup(this TContext ctx, int groupId)
        {
            return ctx.Groups
                .Where(x => x.Id == groupId)
                .Join(ctx.Groups, g => g.Parent, sg => sg.Name, (a, b) => b);
            //            return ctx.Groups
            //                .Where(x => x.Id == groupId)
            //                .Select(x => ctx.Groups.Where(y => y.Name == x.Parent).FirstOrDefault());
        }

        public static bool GuestGroupHasNode(this TContext ctx, string node, Permission permission)
        {
            return ctx.Groups
                .Where(x => x.ApplyToGuests)
                .Join(ctx.GroupNodes, gr => gr.Id, pn => pn.GroupId, (pl, gn) => gn)
                .Join(ctx.Nodes, pn => pn.NodeId, np => np.Id, (a, b) => b)
                .Any(n => n.Node == node && n.Permission == permission);
        }
    }
#elif DAPPER
    public static class OTAContextExtensions
    {
        public static IEnumerable<DbPlayer> GetPlayer(this IDbConnection ctx, string name)
        {
            return ctx.Where<DbPlayer>(new { Name = name });
        }

        public static IEnumerable<Group> GetPlayerGroups(this IDbConnection ctx, long playerId)
        {
            return ctx.Query<Group>($"select g.* from {typeof(DbPlayer).Name} p " +
                $"inner join {typeof(PlayerGroup).Name} pg on p.Id = pg.UserId " +
                $"inner join {typeof(Group).Name} g on pg.GroupId = g.Id " +
                "where p.Id = @PlayerId", new { PlayerId = playerId });
        }

        public static IEnumerable<PermissionNode> GetPermissionByNodeForPlayer(this IDbConnection ctx, long playerId, string node)
        {
            return ctx.Query<PermissionNode>($"select nd.* from {typeof(DbPlayer).Name} p " +
                $"inner join {typeof(PlayerNode).Name} pn on p.Id = pn.UserId " +
                $"inner join {typeof(PermissionNode).Name} nd on pn.NodeId = nd.Id " +
                "where p.Id = @PlayerId and nd.Node = @Node", new { PlayerId = playerId, Node = node });
        }

        public static IEnumerable<PermissionNode> GetPermissionByNodeForGroup(this IDbConnection ctx, long groupId, string node)
        {
            return ctx.Query<PermissionNode>($"select nd.* from {typeof(Group).Name} g " +
                $"inner join {typeof(GroupNode).Name} gn on g.Id = pn.GroupId " +
                $"inner join {typeof(PermissionNode).Name} nd on gn.NodeId = nd.Id " +
                "where g.Id = @GroupId and nd.Node = @Node", new { GroupId = groupId, Node = node });
        }

        public static IEnumerable<Group> GetParentForGroup(this IDbConnection ctx, long groupId)
        {
            return ctx.Query<Group>($"select p.* from {typeof(Group).Name} g " +
                $"inner join {typeof(Group).Name} p on g.Parent = p.Name " +
                "where g.Id = @GroupId", new { GroupId = groupId });
        }

        public static bool GuestGroupHasNode(this IDbConnection ctx, string node, Permission permission)
        {
            return ctx.ExecuteScalar<long>($"select count(nd.Id) from {typeof(Group).Name} g " +
                $"inner join {typeof(GroupNode).Name} gn on g.Id = pn.GroupId " +
                $"inner join {typeof(PermissionNode).Name} nd on gn.NodeId = nd.Id " +
                "where g.ApplyToGuests = 1 and nd.Node = @Node and nd.Permission = @Permission", new {Node = node, Permission = (int)permission }) > 0L;
        }
    }
#endif
}

