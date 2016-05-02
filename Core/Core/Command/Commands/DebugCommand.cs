using System;
using OTA.Command;
using Terraria;
using OTA;
using TDSM.Core.Data;
using TDSM.Core.Data.Management;
using TDSM.Core.Data.Models;
using OTA.Logging;

namespace TDSM.Core.Command.Commands
{
    public class DebugCommand : CoreCommand
    {
        public override void Initialise()
        {
            var api = AddCommand("debug")
                .WithPermissionNode("tdsm.debug")
                .WithAccessLevel(AccessLevel.CONSOLE)
                .WithDescription("Debug actions")
                .WithHelpText("programlog.debug")
                .Calls(Debug);
        }

        void Debug(ISender sender, ArgumentList args)
        {
            var cmd = args.GetString(0);

            switch (cmd)
            {
                case "programlog.debug":
                    ProgramLog.Debug.EnableConsoleOutput = !ProgramLog.Debug.EnableConsoleOutput;
                    sender.Message("ProgramLog.Debug.EnableConsoleOutput: " + ProgramLog.Debug.EnableConsoleOutput);
                    break;
                default:
                    throw new CommandError("Invalid sub command " + cmd);
            }
        }
    }
}

