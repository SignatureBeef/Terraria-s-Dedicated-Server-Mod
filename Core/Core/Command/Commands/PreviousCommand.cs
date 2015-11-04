using System;
using OTA.Command;
using OTA.Logging;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class PreviousCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("!")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithPermissionNode("tdsm.previous")
                .WithDescription("Runs the last command executed by you.")
                .Calls(PreviousCommandHandle);
        }

        private void PreviousCommandHandle(ISender sender, ArgumentList args)
        {
            var player = sender as Player;
            if (player != null)
            {
                if (Core.CommandDictionary.ContainsKey(player.name))
                {
                    Entry.CommandParser.ParsePlayerCommand(player, Core.CommandDictionary[player.name]);
                    ProgramLog.Log("Executed {0}'s previous command: {1}", player.name, Core.CommandDictionary[player.name]);
                }
                else
                    sender.SendMessage("No Previous Command", 255, 255, 20, 20);
                //ProgramLog.Log("{0}", ctx.Player.name); //, args.Prefix + " " + args.ArgumentString);
            }
            if (sender is ConsoleSender)
            {
                if (Core.CommandDictionary.ContainsKey("CONSOLE"))
                    Entry.CommandParser.ParseConsoleCommand(Core.CommandDictionary["CONSOLE"]);
                else
                    sender.SendMessage("No Previous Command", 255, 255, 20, 20);
            }
        }
    }
}

