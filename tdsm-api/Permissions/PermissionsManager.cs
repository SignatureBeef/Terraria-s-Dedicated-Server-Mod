
using System;
namespace TDSM.API.Permissions
{
    public static class PermissionsManager
    {
        private static IPermissionHandler _handler;
        private static EventHandler<XmlNodeEventArgs> OnParseElement; //Im not so sure about a plugin hook for this at this stage.

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
//            var prev = _handler;
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

        public static void AttatchOnParse(EventHandler<XmlNodeEventArgs> callback)
        {
            OnParseElement += callback;
        }

        public static void RequestParse(IPermissionHandler handler, XmlNodeEventArgs args)
        {
            if (OnParseElement != null)
                OnParseElement.Invoke(handler, args);
        }
    }

    /// <summary>
    /// The interface behind custom permissions handlers
    /// </summary>
    public interface IPermissionHandler
    {
        Permission IsPermitted(string node, BasePlayer player);
        Permission IsPermittedForGroup(string node, Func<System.Collections.Generic.Dictionary<String, String>, Boolean> includeGroupsWith = null);

//        XmlNode[] ParseNodes(System.Xml.XmlNode node);
//        string[] ParseArray(System.Xml.XmlNode node);
    }

    //public abstract class PermissionsHandler
    //{
    //    public EventHandler<XmlNodeEventArgs> OnParseElement;

    //    public abstract Permission IsPermitted(string node, BasePlayer player);
    //    public abstract Permission IsPermittedForGroup(string node, Func<System.Collections.Generic.Dictionary<String, String>, Boolean> includeGroupsWith = null);
    //}

    public class XmlNodeEventArgs : EventArgs
    {
        //public string Name { get; set; }
        public System.Xml.XmlNode Node { get; set; }
    }

    public enum Permission : byte
    {
        NodeNonExistent = 0,
        Denied,
        Permitted
    }
}
