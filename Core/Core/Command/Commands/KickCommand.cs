using System;
using Terraria;
using OTA;
using OTA.Command;

namespace TDSM.Core.Command.Commands
{
    public class KickCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("kick")
                .WithDescription("Kicks a player from the server")
                .WithHelpText("<player> - Kicks the player specified.")
                .WithPermissionNode("tdsm.kick")
                .Calls(this.Kick);
        }

        /// <summary>
        /// Kicks a given Player from the server
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Kick(ISender sender, ArgumentList args)
        {
            if (args.TryPop("-s"))
            {
                int s;
                args.ParseOne(out s);

                var slot = Terraria.Netplay.Clients[s];

                #if TDSMServer
                if (slot.State() != SlotState.VACANT)
                {
                slot.Kick("You have been kicked by " + sender.SenderName + ".");

                var player = Main.player[s];
                if (player != null && player.name != null)
                NewNetMessage.SendData(25, -1, -1, player.name + " has been kicked by " + sender.SenderName + ".", 255);
                }
                else
                {
                sender.Message("Kick slot is empty");
                }
                #else
                NetMessage.SendData(2, slot.Id, -1, "Kicked from server.", 0, 0f, 0f, 0f, 0, 0, 0);
                #endif
            }
            else
            {
                Player player;
                args.ParseOne<Player>(out player);

                if (player.name == null)
                {
                    sender.Message("Kick player name is not set.");
                    return;
                }

                player.Kick("You have been kicked by " + sender.SenderName + ".");
                NetMessage.SendData(25, -1, -1, player.name + " has been kicked by " + sender.SenderName + ".", 255);
            }
        }
    }
}

