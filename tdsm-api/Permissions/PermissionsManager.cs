
using System;
namespace tdsm.api.Permissions
{
    public static class PermissionsManager
    {
        private static IPermissionHandler _handler;

        /// <summary>
        /// Used to detect if there is an existing handler set
        /// </summary>
        public static bool IsSet
        {
            get
            { return _handler != null; }
        }

        /// <summary>
        /// Sets the permission handler
        /// </summary>
        /// <param name="handler"></param>
        public static void SetHandler(IPermissionHandler handler)
        {
            if (_handler == null) _handler = handler;
            else lock (_handler) _handler = handler;
        }

        /// <summary>
        /// Used to determine if a player has access to a particular node.
        /// Generally used by plugins.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Permission IsPermitted(string node, BasePlayer player)
        {
            lock (_handler) return _handler.IsPermitted(node, player);
        }

        /// <summary>
        /// Used to determine if a player has access to a particular node.
        /// Generally used by plugins.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Permission IsPermittedForGroup(string node, Func<System.Collections.Generic.Dictionary<String, String>, Boolean> includeGroupsWith)
        {
            lock (_handler) return _handler.IsPermittedForGroup(node, includeGroupsWith);
        }
    }

    /// <summary>
    /// The interface behind custom permissions handlers
    /// </summary>
    public interface IPermissionHandler
    {
        Permission IsPermitted(string node, BasePlayer player);
        Permission IsPermittedForGroup(string node, Func<System.Collections.Generic.Dictionary<String, String>, Boolean> includeGroupsWith = null);
    }

    public enum Permission : byte
    {
        NodeNonExistent = 0,
        Denied,
        Permitted
    }
}
