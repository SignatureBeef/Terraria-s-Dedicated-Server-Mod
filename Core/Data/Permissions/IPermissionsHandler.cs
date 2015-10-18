using System;
using OTA;
using TDSM.Core.Data.Models;

namespace TDSM.Core.Data.Permissions
{
    /// <summary>
    /// The interface behind custom permissions handlers
    /// </summary>
    public interface IPermissionHandler
    {
        Permission IsPermitted(string node, BasePlayer player);

        #region "Management"

        /// <summary>
        /// Find a group by name
        /// </summary>
        /// <returns>The group.</returns>
        Group FindGroup(string name);

        /// <summary>
        /// Add the or update a group.
        /// </summary>
        /// <returns><c>true</c>, if or update group was added, <c>false</c> otherwise.</returns>
        bool AddOrUpdateGroup(string name, bool applyToGuests = false, string parent = null, byte r = 255, byte g = 255, byte b = 255, string prefix = null, string suffix = null);

        /// <summary>
        /// Remove a group from the data store.
        /// </summary>
        /// <returns><c>true</c>, if the group was removed, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        bool RemoveGroup(string name);

        /// <summary>
        /// Add a group node to the data store
        /// </summary>
        /// <returns><c>true</c>, if the group node was added, <c>false</c> otherwise.</returns>
        bool AddGroupNode(string groupName, string node, Permission permission);

        /// <summary>
        /// Remove a group node from the data store
        /// </summary>
        /// <returns><c>true</c>, if the group node was removed, <c>false</c> otherwise.</returns>
        bool RemoveGroupNode(string groupName, string node, Permission permission);

        /// <summary>
        /// Fetches the list of group names from the data store.
        /// </summary>
        string[] GroupList();

        /// <summary>
        /// Fetch the list of nodes for a group
        /// </summary>
        NodePermission[] GroupNodes(string groupName);

        /// <summary>
        /// Add a user to a group.
        /// </summary>
        /// <returns><c>true</c>, if the user was added to the group, <c>false</c> otherwise.</returns>
        bool AddUserToGroup(string username, string groupName);

        /// <summary>
        /// Remove a user from a group
        /// </summary>
        /// <returns><c>true</c>, if the user was removed from the group, <c>false</c> otherwise.</returns>
        bool RemoveUserFromGroup(string username, string groupName);

        /// <summary>
        /// Add a node to the user.
        /// </summary>
        /// <returns><c>true</c>, if the node was added to the user, <c>false</c> otherwise.</returns>
        bool AddNodeToUser(string username, string node, Permission permission);

        /// <summary>
        /// Removed a node from a user
        /// </summary>
        /// <returns><c>true</c>, if the node was removed from the user, <c>false</c> otherwise.</returns>
        bool RemoveNodeFromUser(string username, string node, Permission permission);

        /// <summary>
        /// Fetch the group names a user is associated to
        /// </summary>
        /// <remarks>Currently should always be 1</remarks>
        string[] UserGroupList(string username);

        /// <summary>
        /// Fetch the nodes a user has specific access to
        /// </summary>
        /// <returns>The nodes.</returns>
        /// <param name="username">Username.</param>
        NodePermission[] UserNodes(string username);

        /// <summary>
        /// Fetches the lowest inherited group
        /// </summary>
        /// <returns>The inherited group for user.</returns>
        /// <param name="username">Username.</param>
        Group GetInheritedGroupForUser(string username);

        #endregion
    }
}

