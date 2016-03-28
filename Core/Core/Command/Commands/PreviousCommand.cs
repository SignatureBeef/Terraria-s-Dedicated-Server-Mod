using System;
using OTA;
using OTA.Command;
using OTA.Logging;
using Terraria;
using OTA.Commands;

namespace TDSM.Core.Command.Commands
{
    public class PreviousCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("!")
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
                    CommandManager.Parser.ParseAndProcess(player, Core.CommandDictionary[player.name]);
                    ProgramLog.Log("Executed {0}'s previous command: {1}", player.name, Core.CommandDictionary[player.name]);
                }
                else
                    sender.SendMessage("No Previous Command", 255, 255, 20, 20);
                //ProgramLog.Log("{0}", ctx.Player.name); //, args.Prefix + " " + args.ArgumentString);
            }
            if (sender is ConsoleSender)
            {
                if (Core.CommandDictionary.ContainsKey("CONSOLE"))
                    CommandManager.Parser.ParseAndProcess(CommandParser.ConsoleSender, Core.CommandDictionary["CONSOLE"]);
                else
                    sender.SendMessage("No Previous Command", 255, 255, 20, 20);
            }
        }
    }
}

