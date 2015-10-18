using System;
using OTA.Command;
using OTA.Misc;
using System.Collections.Generic;
using System.Text;
using Terraria;
using TDSM.Core.Data.Permissions;
using OTA;
using OTA.Plugin;
using System.Linq;
using TDSM.Core.Data;
using TDSM.Core.Plugin.Hooks;

namespace TDSM.Core.Command
{
    /// <summary>
    /// Functionality to process sender commands, and the default vanilla commands as an OTA version.
    /// </summary>
    public class CommandParser
    {
        /// <summary>
        /// CommandParser constructor
        /// </summary>
        public CommandParser()
        {
            serverCommands = new Dictionary<String, CommandInfo>();

            AddCommand("exit")
            .SetDefaultUsage()
            .WithDescription("Shutdown the server and save.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Exit);
            AddCommand("exit-nosave")
            .SetDefaultUsage()
            .WithDescription("Shutdown the server without saving.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.ExitNoSave);
            AddCommand("clear")
            .SetDefaultUsage()
            .WithDescription("Clear the console window.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Clear);
            AddCommand("motd")
            .WithDescription("Print or change the message of the day.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.MOTD);
            AddCommand("save")
            .SetDefaultUsage()
            .WithDescription("Save the game world.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Save);
            AddCommand("playing")
            .SetDefaultUsage()
            .WithDescription("Shows the list of players.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Playing);
            AddCommand("kick")
            .WithDescription("Kicks a player from the server.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Kick);
            AddCommand("ban")
            .WithDescription("Bans a player from the server.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Ban);
            AddCommand("password")
            .WithDescription("Shows or changes to password.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Password);
            AddCommand("version")
            .SetDefaultUsage()
            .WithDescription("Print version number.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Version);
            AddCommand("maxplayers")
            .SetDefaultUsage()
            .WithDescription("Print the max number of players.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.MaxPlayers);
            AddCommand("time")
            .SetDefaultUsage()
            .WithDescription("Display game time.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Time);
            AddCommand("port")
            .SetDefaultUsage()
            .WithDescription("Print the listening port.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Port);
            AddCommand("dawn")
            .SetDefaultUsage()
            .WithDescription("Change time to dawn.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Dawn);
            AddCommand("noon")
            .SetDefaultUsage()
            .WithDescription("Change time to noon.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Noon);
            AddCommand("dusk")
            .SetDefaultUsage()
            .WithDescription("Change time to dusk.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Dusk);
            AddCommand("midnight")
            .SetDefaultUsage()
            .WithDescription("Change time to midnight.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Midnight);
            AddCommand("settle")
            .SetDefaultUsage()
            .WithDescription("Settle all water.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.Settle);
            AddCommand("fps")
            .SetDefaultUsage()
            .WithDescription("Toggle FPS monitoring.")
            .WithAccessLevel(AccessLevel.OP)
            .ByPermissionNode("terraria")
            .Calls(VanillaCommands.FPS);

            AddCommand("help")
            .WithAccessLevel(AccessLevel.PLAYER)
            .WithDescription("Displays the commands available to the user.")
            .WithPermissionNode("tdsm.help")
            .Calls(VanillaCommands.ShowHelp);
            AddCommand("plugins")
            .WithAccessLevel(AccessLevel.PLAYER)
            .WithDescription("Lists plugins running")
            .SetDefaultUsage()
            .WithPermissionNode("tdsm.plugins")
            .Calls(VanillaCommands.ListPlugins);
            AddCommand("plugin")
            .WithAccessLevel(AccessLevel.OP)
            .WithDescription("Manage and view plugins")
            .WithHelpText("list")
            .WithHelpText("stat")
            .WithHelpText("info <plugin>")
            .WithHelpText("enable <plugin>")
            .WithHelpText("disable <plugin>")
            .WithHelpText("reload [-clean] all|<plugin>")
            .WithHelpText("unload all|<plugin>")
            .WithHelpText("load [-replace] <file>")
            .WithPermissionNode("tdsm.plugin")
            .Calls(Entry.PluginCommand);
        }

        /// <summary>
        /// The default OTA/Vanilla commands
        /// </summary>
        public readonly Dictionary<String, CommandInfo> serverCommands;

        //        /// <summary>
        //        /// Reads active permission nodes, Player only.
        //        /// </summary>
        //        public void ReadPermissionNodes()
        //        {
        //            foreach (CommandInfo info in serverCommands.Values)
        //            {
        //                if (info.accessLevel == AccessLevel.PLAYER)
        //                {
        //                    if (info.node.Trim().Length > 0 && !Program.permissionManager.ActiveNodes.Contains(info.node))
        //                        Program.permissionManager.ActiveNodes.Add(info.node);
        //                }
        //            }
        //        }

        /// <summary>
        /// Registers new command
        /// </summary>
        /// <param name="prefix">The text attached to the / that will be registered as the command.</param>
        /// <returns>CommandInfo for new command</returns>
        public CommandInfo AddCommand(string prefix)
        {
            prefix = prefix.ToLower();
            if (serverCommands.ContainsKey(prefix))
                throw new ApplicationException("AddCommand: duplicate command: " + prefix);

            var cmd = new CommandInfo(prefix);
            serverCommands[prefix] = cmd;
            serverCommands["." + prefix] = cmd;

            return cmd;
        }

        static ConsoleSender consoleSender = new ConsoleSender();

        /// <summary>
        /// Parses new console command
        /// </summary>
        /// <param name="line">Command to parse</param>
        /// <param name="sender">Sending entity</param>
        public void ParseConsoleCommand(string line, ConsoleSender sender = null)
        {
            line = line.Trim();

            if (sender == null)
            {
                sender = consoleSender;
            }

            ParseAndProcess(sender, line);
        }

        /// <summary>
        /// Parses player commands
        /// </summary>
        /// <param name="player">Sending player</param>
        /// <param name="line">Command to parse</param>
        public void ParsePlayerCommand(Player player, string line)
        {
            if (line.StartsWith("/"))
            {
                line = line.Remove(0, 1);

                ParseAndProcess(player, line);
            }
        }

        /// <summary>
        /// Determines entity's ability to use command. Used when permissions plugin is running.
        /// </summary>
        /// <param name="cmd">Command to check</param>
        /// <param name="sender">Sender entity to check against</param>
        /// <returns>True if sender can use command, false if not</returns>
        public static bool CheckAccessLevel(CommandInfo cmd, ISender sender)
        {
            var perms = CheckPermissions(sender, cmd);
            //            if (Permissions.PermissionsManager.IsSet && perms == Permissions.Permission.Denied)
            //                return false;
            if (Data.Storage.IsAvailable && perms == Permission.Permitted)
                return true;

            return CheckAccessLevel(cmd.accessLevel, sender);
        }

        public static Func<AccessLevel, ISender, Boolean> ExtCheckAccessLevel;

        /// <summary>
        /// Determines the access level of the sender. Used when no permissions plugin is found.
        /// </summary>
        /// <param name="acc">Access level to check against</param>
        /// <param name="sender">Sender to check</param>
        /// <returns>True if sender has access level equal to or greater than acc</returns>
        public static bool CheckAccessLevel(AccessLevel acc, ISender sender)
        {
            if (sender is Player)
                return acc == AccessLevel.PLAYER || (acc == AccessLevel.OP && sender.Op);

            //            if (sender is RConSender)
            //                return acc <= AccessLevel.REMOTE_CONSOLE;

            if (ExtCheckAccessLevel != null && ExtCheckAccessLevel(acc, sender))
                return true;

            if (sender is ConsoleSender)
                return true;

            throw new NotImplementedException("Unexpected ISender implementation");
        }

        /// <summary>
        /// Gets the available commands for an access level.
        /// </summary>
        /// <returns>The available commands.</returns>
        /// <param name="access">Access.</param>
        public static Dictionary<string, CommandInfo> GetAvailableCommands(AccessLevel access)
        {
            var available = Entry.CommandParser.serverCommands.GetAvailableCommands(access);

            //We *may* not need this - but let's see how it goes
            foreach (var plg in PluginManager.EnumeratePlugins)
            {
                var additional = plg.GetPluginCommands().GetAvailableCommands(access)
                    .Where(x => !x.Key.StartsWith(plg.Name.ToLower() + '.'))
                    .ToArray();
                foreach (var pair in additional)
                {
                    //Override defaults
                    if (available.ContainsKey(pair.Key))
                        available[pair.Key] = pair.Value;
                    else
                        available.Add(pair.Key, pair.Value);
                }
            }

            return available;
        }

        /// <summary>
        /// Permissions checking for registered commands.
        /// </summary>
        /// <param name="sender">Entity to check permissions for</param>
        /// <param name="cmd">Command to check for permissions on</param>
        /// <returns>True if entity can use command.  False if not.</returns>
        public static Permission CheckPermissions(ISender sender, CommandInfo cmd)
        {
            if (cmd.node == null || sender is ConsoleSender || sender.Op)
                return Permission.Permitted;

            if (sender is BasePlayer && Storage.IsAvailable)
                return Storage.IsPermitted(cmd.node, sender as BasePlayer);

            return Permission.Denied;
        }

        /// <summary>
        /// Parses and process a command from a sender
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="line">Line.</param>
        public void ParseAndProcess(ISender sender, string line)
        {
            var ctx = new HookContext
            {
                Sender = sender,
                Player = sender as Player
            };
            
            ctx.Connection = ctx.Player != null ? ctx.Player.Connection.Socket : null;

            var hargs = new TDSMHookArgs.ServerCommand();

            try
            {
                CommandInfo info;

                var firstSpace = line.IndexOf(' ');

                if (firstSpace < 0)
                    firstSpace = line.Length;

                var prefix = line.Substring(0, firstSpace).ToLower();

                hargs.Prefix = prefix;

                if (CommandManager.FindStringCommand(prefix, out info))
                {
                    hargs.ArgumentString = (firstSpace < line.Length - 1 ? line.Substring(firstSpace + 1, line.Length - firstSpace - 1) : "").Trim();

                    TDSMHookPoints.ServerCommand.Invoke(ref ctx, ref hargs);

                    if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                        return;

                    if (ctx.Result != HookResult.CONTINUE && !CheckAccessLevel(info, sender))
                    {
                        sender.SendMessage("Permissions error", 255, 238, 130, 238);
                        return;
                    }

                    try
                    {
                        info.Run(sender, hargs.ArgumentString);
                    }
                    catch (NLua.Exceptions.LuaScriptException e)
                    {
                        if (e.IsNetException)
                        {
                            var ex = e.GetBaseException();
                            if (ex != null)
                            {
                                if (ex is CommandError)
                                {
                                    sender.SendMessage(prefix + ": " + ex.Message);
                                    info.ShowHelp(sender);
                                }
                            }
                        }
                    }
                    catch (ExitException e)
                    {
                        throw e;
                    }
                    catch (CommandError e)
                    {
                        sender.SendMessage(prefix + ": " + e.Message);
                        info.ShowHelp(sender);
                    }
                    return;
                }

                var args = new ArgumentList();
                var command = Tokenize(line, args);

                if (command != null)
                {
                    if (CommandManager.FindTokenCommand(command, out info))
                    {
                        hargs.Arguments = args;
                        hargs.ArgumentString = args.ToString();

                        foreach (BasePlugin plg in PluginManager._plugins.Values)
                        {
                            if (plg.GetPluginCommands().ContainsKey(command))
                            {
                                args.Plugin = plg;
                            }
                        }

                        TDSMHookPoints.ServerCommand.Invoke(ref ctx, ref hargs);

                        if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                            return;

                        if (ctx.Result != HookResult.CONTINUE && !CheckAccessLevel(info, sender))
                        {
                            sender.SendMessage("Permissions error", 255, 238, 130, 238);
                            return;
                        }

                        try
                        {
                            info.Run(sender, hargs.Arguments);
                        }
                        catch (NLua.Exceptions.LuaScriptException e)
                        {
                            if (e.IsNetException)
                            {
                                var ex = e.GetBaseException();
                                if (ex != null)
                                {
                                    if (ex is CommandError)
                                    {
                                        sender.SendMessage(command + ": " + ex.Message);
                                        info.ShowHelp(sender);
                                    }
                                }
                            }
                        }
                        catch (ExitException e)
                        {
                            throw e;
                        }
                        catch (CommandError e)
                        {
                            sender.SendMessage(command + ": " + e.Message);
                            info.ShowHelp(sender);
                        }
                        return;
                    }
                    else
                    {
                        sender.SendMessage(String.Format("No such command '{0}'.", command));
                    }
                }
            }
            catch (ExitException e)
            {
                throw e;
            }
            catch (TokenizerException e)
            {
                sender.SendMessage(e.Message);
            }
        }

        class TokenizerException : Exception
        {
            public TokenizerException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// Splits a command on spaces, with support for "parameters in quotes" and non-breaking\ spaces.
        /// Literal quotes need to be escaped like this: \"
        /// Literal backslashes need to escaped like this: \\
        /// Returns the first token
        /// </summary>
        /// <param name="command">Whole command line without trailing newline </param>
        /// <param name="args">An empty list to put the arguments in </param>
        public static string Tokenize(string command, List<string> args)
        {
            char l = '\0';
            var b = new StringBuilder();
            string result = null;
            int s = 0;

            foreach (char cc in command.Trim())
            {
                char c = cc;
                switch (s)
                {
                    case 0: // base state
                        {
                            if (c == '"' && l != '\\')
                                s = 1;
                            else if (c == ' ' && l != '\\' && b.Length > 0)
                            {
                                if (result == null)
                                    result = b.ToString();
                                else
                                    args.Add(b.ToString());
                                b.Length = 0;
                            }
                            else if ((c != '\\' && c != ' ') || l == '\\')
                            {
                                b.Append(c);
                                c = '\0';
                            }
                        }
                        break;

                    case 1: // inside quotes
                        {
                            if (c == '"' && l != '\\')
                                s = 0;
                            else if (c != '\\' || l == '\\')
                            {
                                b.Append(c);
                                c = '\0';
                            }
                        }
                        break;
                }
                l = c;
            }

            if (s == 1)
                throw new TokenizerException("Unmatched quote in command.");

            if (b.Length > 0)
            {
                if (result == null)
                    result = b.ToString();
                else
                    args.Add(b.ToString());
            }

            return result;
        }

        // for binary compatibility
        public static List<string> Tokenize(string command)
        {
            List<string> args = new List<string>();
            args.Insert(0, Tokenize(command, args));
            return args;
        }
    }
}

