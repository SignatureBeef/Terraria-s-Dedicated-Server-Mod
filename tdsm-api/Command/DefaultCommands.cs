using System;
using System.Linq;
using System.Collections.Generic;

#if Full_API
using Terraria;
using TDSM.API.Callbacks;

namespace TDSM.API.Command
{
    /// <summary>
    /// Contains the default command implementation that can be overidden by plugins
    /// These commands are the bare minimum for a vanilla server.
    /// </summary>
    public static class DefaultCommands
    {
        //TODO sort commands in alphabetical order.

        /// <summary>
        /// Clears the console text.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Clear(ISender sender, ArgumentList args)
        {
            if (sender is ConsoleSender)
            {
                Console.Clear();
            }
            else
                sender.Message("clear: This is a console only command");
        }

        /// <summary>
        /// Saves world then exits server.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Exit(ISender sender, ArgumentList args)
        {
            Tools.NotifyAllOps("Exiting...");

            Terraria.IO.WorldFile.saveWorld(false);
            Netplay.disconnect = true;
        }

        /// <summary>
        /// Exits server without first saving the world.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void ExitNoSave(ISender sender, ArgumentList args)
        {
            Tools.NotifyAllOps("Exiting without saving...");
            Netplay.disconnect = true;
        }

        /// <summary>
        /// Prints the message of the day.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void MOTD(ISender sender, string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                if (Main.motd == String.Empty)
                {
                    sender.Message("Welcome to " + Main.worldName + "!");
                }
                else
                {
                    sender.Message("MOTD: " + Main.motd);
                }
            }
            else
            {
                Main.motd = message;
            }
        }

        /// <summary>
        /// Prints the current users playing.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Playing(ISender sender, ArgumentList args)
        {
            var count = 0;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    count++;
                    sender.Message("{0} ({1})", Main.player[i].name, Netplay.Clients[i].RemoteAddress());
                }
            }
            if (count == 0)
                sender.Message("No players connected.");
            else if (count == 1)
                sender.Message("1 player connected.");
            else
                sender.Message(count + " players connected.");
        }

        /// <summary>
        /// Lists currently enabled plugins.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void ListPlugins(ISender sender, ArgumentList args)
        {
            if (PluginManager.PluginCount > 0)
            {
                string plugins = String.Empty;

                foreach (var plugin in PluginManager.EnumeratePlugins)
                {
                    if (!plugin.IsEnabled || plugin.Name.Trim().Length > 0)
                    {
                        var name = plugin.Name.Trim();
                        if (!String.IsNullOrEmpty(plugin.Version))
                        {
                            name += " (" + plugin.Version + ")";
                        }
                        plugins += ", " + name;
                    }
                }
                if (plugins.StartsWith(","))
                {
                    plugins = plugins.Remove(0, 1).Trim(); //Remove the ', ' from the start and trim the ends
                }
                sender.Message("Loaded plugins: " + plugins + ".");
            }
            else
            {
                sender.Message("No plugins loaded.");
            }
        }

        /// <summary>
        /// Kicks a user from the server.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Kick(ISender sender, string player)
        {
            if (String.IsNullOrEmpty(player))
            {
                sender.Message("Usage: kick <player>");
            }
            else
            {
                bool found = false;
                var lowered = player.ToLower();
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active && Main.player[i].name.ToLower() == lowered)
                    {
                        NetMessage.SendData(2, i, -1, "Kicked from server.", 0, 0f, 0f, 0f, 0);
                        found = true;
                    }
                }
                if (!found)
                    sender.Message("Failed to find a player by the name of {0}", player);
            }
        }

        /// <summary>
        /// Bans a user from the server.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Ban(ISender sender, string player)
        {
            if (String.IsNullOrEmpty(player))
            {
                sender.Message("Usage: ban <player>");
            }
            else
            {
                bool found = false;
                var lowered = player.ToLower();
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active && Main.player[i].name.ToLower() == lowered)
                    {
                        Callbacks.NetplayCallback.AddBan(i);
                        NetMessage.SendData(2, i, -1, "Banned from server.", 0, 0f, 0f, 0f, 0);
                        found = true;
                    }
                }
                if (!found)
                    sender.Message("Failed to find a player by the name of {0}", player);
            }
        }

        /// <summary>
        /// Displays the current server password if no parameter is supplied, else it will be set.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Password(ISender sender, string password)
        {
            if (String.IsNullOrEmpty(Netplay.ServerPassword))
            {
                if (String.IsNullOrEmpty(password))
                {
                    sender.Message("No password set.");
                }
                else
                {
                    Netplay.ServerPassword = password;
                    sender.Message("Password: " + Netplay.ServerPassword);
                }
            }
            else
            {
                if (String.IsNullOrEmpty(password))
                {
                    Netplay.ServerPassword = String.Empty;
                    sender.Message("Password disabled.");
                }
                else
                {
                    Netplay.ServerPassword = password;
                    sender.Message("Password: " + Netplay.ServerPassword);
                }
            }
        }

        /// <summary>
        /// Displays the current Terraria version
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Version(ISender sender, ArgumentList args)
        {
            sender.Message("Terraria Server " + Main.versionNumber);
            sender.Message("TDSM API Version " + Globals.Build + Globals.PhaseToSuffix(Globals.BuildPhase));
        }

        /// <summary>
        /// Displays the player limit
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void MaxPlayers(ISender sender, ArgumentList args)
        {
            sender.Message("Player limit: " + Main.maxNetPlayers);
        }

        /// <summary>
        /// Displays the current world time
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Time(ISender sender, ArgumentList args)
        {
            string text3 = "AM";
            double num = Main.time;
            if (!Main.dayTime)
            {
                num += 54000.0;
            }
            num = num / 86400.0 * 24.0;
            double num2 = 7.5;
            num = num - num2 - 12.0;
            if (num < 0.0)
            {
                num += 24.0;
            }
            if (num >= 12.0)
            {
                text3 = "PM";
            }
            int num3 = (int)num;
            double num4 = num - (double)num3;
            num4 = (double)((int)(num4 * 60.0));
            string text4 = string.Concat(num4);
            if (num4 < 10.0)
            {
                text4 = "0" + text4;
            }
            if (num3 > 12)
            {
                num3 -= 12;
            }
            if (num3 == 0)
            {
                num3 = 12;
            }
            sender.Message(string.Concat(new object[]
                    {
                        "Time: ",
                        num3,
                        ":",
                        text4,
                        " ",
                        text3
                    }));
        }

        /// <summary>
        /// Displays the current port
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Port(ISender sender, ArgumentList args)
        {
            sender.Message("Port: " + Netplay.ListenPort);
        }

        /// <summary>
        /// Changes the time to dawn
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Dawn(ISender sender, ArgumentList args)
        {
            Main.dayTime = true;
            Main.time = 0.0;
            NetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);

            sender.Message("Time set to dawn");
        }

        /// <summary>
        /// Changes the time to noon
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Noon(ISender sender, ArgumentList args)
        {
            Main.dayTime = true;
            Main.time = 27000.0;
            NetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);

            sender.Message("Time set to noon");
        }

        /// <summary>
        /// Changes the time to dusk
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Dusk(ISender sender, ArgumentList args)
        {
            Main.dayTime = false;
            Main.time = 0.0;
            NetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);

            sender.Message("Time set to dusk");
        }

        /// <summary>
        /// Changes the time to midnight
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Midnight(ISender sender, ArgumentList args)
        {
            Main.dayTime = false;
            Main.time = 16200.0;
            NetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);

            sender.Message("Time set to midnight");
        }

        /// <summary>
        /// Settles liquids
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Settle(ISender sender, ArgumentList args)
        {
            if (!Liquid.panicMode)
            {
                if (sender is Player)
                    sender.Message("Forcing water to settle.");
                Liquid.StartPanic();
            }
            else
            {
                sender.Message("Water is already settling");
            }
        }

        /// <summary>
        /// Toggles server FPS timing
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void FPS(ISender sender, ArgumentList args)
        {
            if (sender is ConsoleSender)
            {
                if (!Main.dedServFPS)
                {
                    Main.dedServFPS = true;
                    Main.fpsTimer.Reset();
                }
                else
                {
                    Main.dedServCount1 = 0;
                    Main.dedServCount2 = 0;
                    Main.dedServFPS = false;
                }
            }
            else
                throw new CommandError("This is a console only command");
        }

        /// <summary>
        /// Saves to world
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Save(ISender sender, ArgumentList args)
        {
            Terraria.IO.WorldFile.saveWorld(false);
        }

        /// <summary>
        /// Shows the help.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        public static void ShowHelp(ISender sender, ArgumentList args)
        {
            var commands = sender.GetAvailableCommands();
            if (commands != null && commands.Count > 0)
            {
                int page = 0;
                if (!args.TryGetInt(0, out page))
                {
                    if (args.Count > 0)
                    {
                        var command = args.GetString(0);
                        if (commands.ContainsKey(command))
                        {
                            sender.SendMessage(commands[command].description);
                            commands[command].ShowHelp(sender, true);
                            return;
                        }
                        else
                            throw new CommandError("No such command: " + command);
                    }
                }
                else
                    page--;

                //				const Int32 MaxLines = 5;
                var maxLines = sender is Player ? 5 : 15;
                var lineOffset = page * maxLines;
                var maxPages = (int)Math.Ceiling(commands.Count / (double)maxLines);

                if (page >= 0 && page < maxPages)
                {
                    var cmds = new List<CommandInfo>();
                    var sorted = commands
                        .OrderBy(x => x.Key.ToLower())
                        .Select(x => x.Value)
                        .ToArray();
                    for (var i = lineOffset; i < lineOffset + maxLines; i++)
                    {
                        if (i < sorted.Length)
                            cmds.Add(sorted[i]);
                    }

                    var prefixMax = cmds
                        .Select(x => x.Prefix.Length)
                        .OrderByDescending(x => x)
                        .First();
                    foreach (var cmd in cmds)
                        cmd.ShowDescription(sender, prefixMax);

                    sender.SendMessage(String.Format("[Page {0} / {1}]", page + 1, maxPages));
                }
                else
                {
                    sender.SendMessage("Usage:");
                    sender.SendMessage("    help <command> - Get help for a command.");
                    sender.SendMessage("    help <page> - View a list of commands. Valid page numbers are 1 to " + maxPages + ".");
                    sender.SendMessage("Examples:");
                    sender.SendMessage("    help oplogin");
                    sender.SendMessage("    help 1");
                }
            }
            else
                sender.SendMessage("You have no available commands.");
        }
    }
}
#endif