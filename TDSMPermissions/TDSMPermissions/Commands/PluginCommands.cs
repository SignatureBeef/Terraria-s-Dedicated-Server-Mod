using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using Terraria_Server.Permissions;
using Terraria_Server.Commands;
using Terraria_Server.Misc;

using TDSMPermissions;

namespace TDSMPermissions.Commands
{
    public class PluginCommands
    {
		public static void PermissionsCommand(ISender sender, ArgumentList args)
		{
			if (Node.IsPermitted("permissions.test", (Player)sender))
				sender.sendMessage("You have permission.");

		}
    }
}
