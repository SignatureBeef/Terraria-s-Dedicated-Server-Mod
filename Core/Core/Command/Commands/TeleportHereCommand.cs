using System;
using OTA.Command;
using Terraria;
using OTA;

namespace TDSM.Core.Command.Commands
{
    public class TeleportHereCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("tphere")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Teleport a player to yourself")
                .WithHelpText("<player>")
                .WithPermissionNode("tdsm.tphere")
                .Calls(this.TeleportHere);
        }

        /// <summary>
        /// Teleports specified player to sending player's location.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void TeleportHere(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                Player subject;

                if (args.TryPopOne(out subject))
                {
                    if (subject == null)
                    {
                        sender.Message("Cannot find player");
                        return;
                    }

                    subject.Teleport(player);

                    Tools.NotifyAllOps("Teleported " + subject.name + " to " +
                        player.name + " [" + sender.SenderName + "]", true);
                }
            }
            else
            {
                throw new CommandError("This command is for players only");
            }
        }
    }
}

