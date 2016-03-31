using System;
using OTA;
using OTA.Permissions;
using OTA.Command;

namespace TDSM.Core.Data.Permissions
{
    public class OTAPIPermissions : OTA.Permissions.IPermissionHandler
    {
        public Permission GetPlayerPermission(ISender sender, string node, params object[] args)
        {
            if (sender is BasePlayer)
            {
                var permission = Storage.IsPermitted(node, sender as BasePlayer);
                if (permission == Permission.NoPermission)
                {
                    if (args != null && args.Length == 1)
                    {
                        var cmd = args[0] as TDSM.Core.Command.TDSMCommandInfo;
                        if (cmd != null)
                        {
                            if (cmd._accessLevel == Command.AccessLevel.PLAYER) return Permission.Permitted;
                        }
                    }
                }
                return permission;
            }
            else if (sender is ConsoleSender)
                return Permission.Permitted;

            return Permission.NoPermission;
        }
    }
}
