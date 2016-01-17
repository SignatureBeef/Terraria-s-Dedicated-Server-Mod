using System;
using OTA;
using OTA.Command;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class RefreshCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("refresh")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Redownload the area around you from the server")
                .WithHelpText("Usage:    refresh")
                .WithPermissionNode("tdsm.refresh")
                .Calls(this.Refresh);
        }

        /// <summary>
        /// Refreshes a players area
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Refresh(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            var player = sender as Player;

            if (player == null)
            {
                sender.Message(255, "This is a player command");
                return;
            }

            if (player.whoAmI < 0)
                return;

            if (!player.IsOp())
            {
                var diff = DateTime.Now - player.GetLastCostlyCommand();

                if (diff < TimeSpan.FromSeconds(30))
                {
                    sender.Message(255, "You must wait {0:0} more seconds before using this command.", 30.0 - diff.TotalSeconds);
                    return;
                }

                player.SetLastCostlyCommand(DateTime.Now);
            }

            NetMessage.SendTileSquare(player.whoAmI, (int)(player.position.X / 16), (int)(player.position.Y / 16), 32);
        }
    }
}

