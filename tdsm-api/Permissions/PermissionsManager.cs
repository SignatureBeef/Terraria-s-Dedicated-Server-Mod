
namespace tdsm.api.Permissions
{
    public static class PermissionsManager
    {
        private static IPermissionHandler _handler;

        public static bool IsSet
        {
            get
            {
                return _handler != null;
            }
        }

        public static void SetHandler(IPermissionHandler handler)
        {
            if (_handler == null) _handler = handler;
            else lock (_handler) _handler = handler;
        }

        public static bool IsRestricted(string node, BasePlayer player)
        {
            return _handler.IsRestricted(node, player);
        }
    }

    public interface IPermissionHandler
    {
        bool IsRestricted(string node, BasePlayer player);
    }
}
