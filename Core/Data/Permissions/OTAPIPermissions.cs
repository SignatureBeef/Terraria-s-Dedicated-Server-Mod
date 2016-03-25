using System;
using OTA;
using OTA.Permissions;
using OTA.Command;

namespace TDSM.Core.Data.Permissions
{
    public class OTAPIPermissions : OTA.Permissions.IPermissionHandler
    {
        public Permission GetPlayerPermission(ISender sender, string node)
        {
            if (sender is BasePlayer)
                return Storage.IsPermitted(node, sender as BasePlayer);
            else if (sender is ConsoleSender)
                return Permission.Permitted;

            return Permission.NoPermission;
        }
    }
}
