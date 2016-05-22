using System;
using OTA;
using OTA.Command;
using Terraria;
using System.Linq;

namespace TDSM.Core.Command.Commands
{
    public class ConnectionCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("conn")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.conn")
                .WithHelpText("count")
                .WithHelpText("kill <slot>")
                .WithHelpText("check")
                .Calls((ISender sender, ArgumentList args) =>
                {
                    switch (args.GetString(0).ToLower())
                    {
                        case "count":
                            sender.Message("Active connectons: " + (Netplay.Clients.Count(x => x.IsActive)));
                            break;

                        case "kill":
                            var at = args.GetInt(1);
                            var client = Netplay.Clients[at];
                            if (client != null && client.IsActive)
                            {
                                client.PendingTermination = true;
                            }
                            break;

                        case "check":
                            for (var i = 0; i < Main.player.Length; i++)
                            {
                                var player = Main.player[i];
                                var conn = Netplay.Clients[i];

                                if (conn == null && player == null) continue;
                                if (player != null && conn != null)
                                {
                                    if (player.active != conn.IsActive)
                                    {
                                        sender.Message($"Slot {i}, player active: {player.active}, conn active: {conn.IsActive}.");
                                    }
                                }
                                else if (player == null && conn.IsActive)
                                {
                                    sender.Message($"Player at slot {i} is null, but the connection instance is active.");
                                }
                                else if (conn == null && player.active)
                                {
                                    sender.Message($"Connection at slot {i} is null, but the player instance is active.");
                                }
                            }
                            break;
                    }
                });
        }
    }
}

