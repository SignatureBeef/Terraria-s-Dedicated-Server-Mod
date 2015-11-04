using System;
using Terraria;
using OTA.Command;
using System.Linq;

namespace TDSM.Core.Command.Commands
{
    public class PlayerListCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("list")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.List);

            Core.AddCommand("who")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.List);

            Core.AddCommand("players")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.OldList);

            // this is what the server crawler expects
            Core.AddCommand("playing")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.OldList);

            Core.AddCommand("online")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.List);
        }

        /// <summary>
        /// Prints a Playerlist.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void OldList(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            var online = String.Empty;
            if (sender is ConsoleSender)
            {
                var players = from p in Main.player
                                          where p.active
                                          select String.Format("{0} ({1})", p.name, p.IPAddress);

                online = String.Join(", ", players);
            }
            else
            {
                var players = from p in Main.player
                                          where p.active
                                          select p.name;

                online = String.Join(", ", players);
            }

            if (String.IsNullOrEmpty(online))
                sender.Message("No players online.", 255, 255, 240, 20);
            else
                sender.Message("Current players: " + online, 255, 255, 240, 20);
        }

        /// <summary>
        /// Prints a player list, Possibly readable by bots.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void List(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            var players = from p in Terraria.Main.player
                                   where p.active && !p.Op
                                   select p.name;
            var ops = from p in Terraria.Main.player
                               where p.active && p.Op
                               select p.name;

            var pn = players.Count();
            var on = ops.Count();

            if (on + pn == 0)
            {
                sender.Message("No players online.");
                return;
            }

            string ps = String.Empty;
            string os = String.Empty;

            if (pn > 0)
                ps = (on > 0 ? " | Players: " : "Players: ") + String.Join(", ", players);

            if (on > 0)
                os = "Ops: " + String.Join(", ", ops);

            sender.SendMessage(string.Concat(os, ps, " (", on + pn, "/", Netplay.MaxConnections, ")"), 255, 255, 240, 20);
        }
    }
}

