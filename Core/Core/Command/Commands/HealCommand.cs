using System;
using OTA.Command;
using Terraria;
using OTA;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Command.Commands
{
    public class HealCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("heal")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Heals one or all players.")
                .WithHelpText("<player>")
                .WithHelpText("-all")
                .WithPermissionNode("tdsm.heal")
                .Calls(this.Heal);
        }

        /// <summary>
        /// Heals one or all players.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="message">Message to send</param>
        public void Heal(ISender sender, ArgumentList args)
        {
            Player subject;

            if (args.TryPopOne(out subject))
            {
                subject = sender as Player;
                if (subject == null)
                {
                    sender.Message("Need a heal target");
                    return;
                }

                NetMessage.SendData((int)Packet.HEAL_PLAYER, -1, -1, String.Empty, subject.whoAmI, (float)subject.statLifeMax);
                subject.Message("You have been healed!", Color.Green);
            }
            else if (args.TryPop("-all"))
            {
                foreach (var plr in Main.player)
                {
                    if (plr.active)
                    {
                        NetMessage.SendData((int)Packet.HEAL_PLAYER, -1, -1, String.Empty, plr.whoAmI, (float)plr.statLifeMax);
                        plr.Message("You have been healed!", Color.Green);
                    }
                }
            }
            else if (sender is Player)
            {
                var plr = sender as Player;
                NetMessage.SendData((int)Packet.HEAL_PLAYER, -1, -1, String.Empty, plr.whoAmI, (float)plr.statLifeMax);
                plr.Message("You have been healed!", Color.Green);
            }
            else throw new CommandError("Nobody specified to heal");
        }
    }
}

