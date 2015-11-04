using System;
using TDSM.Core.RemoteConsole;

namespace TDSM.Core.Command.Commands
{
    public class RconCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("rcon")
                .WithDescription("Manage remote console access.")
                .WithAccessLevel(AccessLevel.REMOTE_CONSOLE)
                .WithHelpText("load       - reload login database")
                .WithHelpText("list       - list rcon connections")
                .WithHelpText("cut <name> - cut off rcon connections")
                .WithHelpText("ban <name> - cut off rcon connections and revoke access")
                .WithHelpText("add <name> <password> - add an rcon user")
                .WithPermissionNode("tdsm.rcon")
                .Calls(RConServer.RConCommand);
        }
    }
}

