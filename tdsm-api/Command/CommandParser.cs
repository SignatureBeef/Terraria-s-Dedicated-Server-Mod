using System;
using System.Collections.Generic;
using System.Text;
using TDSM.API.Plugin;
using TDSM.API.Misc;
using System.Linq;


#if Full_API
using Terraria;
#endif

namespace TDSM.API.Command
{
    public enum AccessLevel : int
    {
        PLAYER = 1,
        OP,
        REMOTE_CONSOLE,
        CONSOLE,
    }

    public class CommandInfo
    {
        internal string description;
        internal List<string> helpText = new List<string>();
        internal string node;
        internal AccessLevel accessLevel = AccessLevel.OP;
        internal Action<ISender, ArgumentList> tokenCallback;
        internal Action<ISender, string> stringCallback;

        internal event Action<CommandInfo> BeforeEvent;
        internal event Action<CommandInfo> AfterEvent;

        internal string _prefix;
        internal bool _defaultHelp;
        internal bool _oldHelpStyle;

        internal NLua.LuaFunction LuaCallback;

        //		public int HelpTextCount
        //		{
        //			get
        //			{ return _defaultHelp ? 1 + helpText.Count : helpText.Count; }
        //		}

        public string Prefix
        {
            get
            { return _prefix; }
        }

        public string Node
        {
            get
            { return node; }
        }

        internal CommandInfo(string prefix)
        {
            _prefix = prefix;
        }

        internal void InitFrom(CommandInfo other)
        {
            description = other.description;
            helpText = other.helpText;
            accessLevel = other.accessLevel;
            tokenCallback = other.tokenCallback;
            stringCallback = other.stringCallback;
            LuaCallback = other.LuaCallback;
            ClearEvents();
        }

        internal void ClearCallbacks()
        {
            tokenCallback = null;
            stringCallback = null;
            LuaCallback = null;
        }

        public CommandInfo WithDescription(string desc)
        {
            description = desc;
            return this;
        }

        public CommandInfo WithHelpText(string help)
        {
            helpText.Add(help);
            return this;
        }

        public CommandInfo SetDefaultUsage()
        {
            _defaultHelp = true;
            return this;
        }

        public CommandInfo SetOldHelpStyle()
        {
            _oldHelpStyle = true;
            return this;
        }

        public CommandInfo WithAccessLevel(AccessLevel accessLevel)
        {
            this.accessLevel = accessLevel;
            return this;
        }

        public CommandInfo WithAccessLevel(int accessLevel) //LUA...
        {
            this.accessLevel = (AccessLevel)accessLevel;
            return this;
        }

        public CommandInfo WithPermissionNode(string node)
        {
            this.node = node;
            return this;
        }

        internal CommandInfo WithPermissionNode()
        {
            const String tdsm = "tdsm.";
            this.node = tdsm + this._prefix;
            return this;
        }

        public CommandInfo Calls(Action<ISender, ArgumentList> callback)
        {
            tokenCallback = callback;
            return this;
        }

        public CommandInfo Calls(Action<ISender, string> callback)
        {
            stringCallback = callback;
            return this;
        }

        public CommandInfo LuaCall(NLua.LuaFunction callback)
        {
            LuaCallback = callback;
            return this;
        }

        public void ShowHelp(ISender sender, bool noHelp = false)
        {
            if (helpText.Count == 0 && noHelp)
            {
                // Disabled this since it's not needed. There will usually be a description. But there should be some checks on if these are actually set, especially for plugins.
                //sender.SendMessage("No help text specified.");
                return;
            }

#if Full_API
            if (!_oldHelpStyle)
            {
                const String Push = "       ";
                string command = (sender is Player ? "/" : String.Empty) + _prefix;
                if (_defaultHelp)
                    sender.SendMessage("Usage: " + command);

                bool first = !_defaultHelp;
                foreach (var line in helpText)
                {
                    if (first)
                    {
                        first = false;
                        sender.SendMessage("Usage: " + command + " " + line);
                    }
                    else
                        sender.SendMessage(Push + command + " " + line);
                }
            }
            else
            {
                foreach (var line in helpText)
                    sender.SendMessage(line);
            }
#endif
        }

        public void ShowDescription(ISender sender, int padd)
        {
#if Full_API
            var space = String.Empty;
            for (var x = 0; x < padd - this._prefix.Length; x++)
                space += ' ';
            sender.SendMessage((sender is Player ? "/" : String.Empty) + _prefix +
                space + " - " + (this.description ?? "No description specified")
            );
#endif
        }

        internal void Run(ISender sender, string args)
        {
            if (BeforeEvent != null)
                BeforeEvent(this);

            try
            {
                if (!CommandParser.CheckAccessLevel(this, sender))
                {
                    sender.SendMessage("You cannot perform that action.", 255, 238, 130, 238);
                    return;
                }

                if (stringCallback != null)
                    stringCallback(sender, args);
                else if (LuaCallback != null)
                    LuaCallback.Call(this, sender, args);
                else
                    sender.SendMessage("This command is no longer available", 255, 238, 130, 238);
            }
            finally
            {
                if (AfterEvent != null)
                    AfterEvent(this);
            }
        }

        internal void Run(ISender sender, ArgumentList args)
        {
            if (BeforeEvent != null)
                BeforeEvent(this);

            try
            {
                if (!CommandParser.CheckAccessLevel(this, sender))
                {
                    sender.SendMessage("You cannot perform that action.", 255, 238, 130, 238);
                    return;
                }

                if (tokenCallback != null)
                    tokenCallback(sender, args);
                else if (LuaCallback != null)
                    LuaCallback.Call(this, sender, args);
                else
                    sender.SendMessage("This command is no longer available", 255, 238, 130, 238);
            }
            finally
            {
                if (AfterEvent != null)
                    AfterEvent(this);
            }
        }

        internal void ClearEvents()
        {
            AfterEvent = null;
            BeforeEvent = null;
        }
    }

    public class CommandParser
    {
        /// <summary>
        /// CommandParser constructor
        /// </summary>
        public CommandParser()
        {
            serverCommands = new Dictionary<String, CommandInfo>();

#if Full_API
            AddCommand("exit")
                .SetDefaultUsage()
                .WithDescription("Shutdown the server and save.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Exit);
            AddCommand("exit-nosave")
                .SetDefaultUsage()
                .WithDescription("Shutdown the server without saving.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.ExitNoSave);
            AddCommand("clear")
                .SetDefaultUsage()
                .WithDescription("Clear the console window.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Clear);
            AddCommand("motd")
                .WithDescription("Print or change the message of the day.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.MOTD);
            AddCommand("save")
                .SetDefaultUsage()
                .WithDescription("Save the game world.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Save);
            AddCommand("playing")
                .SetDefaultUsage()
                .WithDescription("Shows the list of players.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Playing);
            AddCommand("kick")
                .WithDescription("Kicks a player from the server.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Kick);
            AddCommand("ban")
                .WithDescription("Bans a player from the server.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Ban);
            AddCommand("password")
                .WithDescription("Shows or changes to password.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Password);
            AddCommand("version")
                .SetDefaultUsage()
                .WithDescription("Print version number.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Version);
            AddCommand("maxplayers")
                .SetDefaultUsage()
                .WithDescription("Print the max number of players.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.MaxPlayers);
            AddCommand("time")
                .SetDefaultUsage()
                .WithDescription("Display game time.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Time);
            AddCommand("port")
                .SetDefaultUsage()
                .WithDescription("Print the listening port.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Port);
            AddCommand("dawn")
                .SetDefaultUsage()
                .WithDescription("Change time to dawn.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Dawn);
            AddCommand("noon")
                .SetDefaultUsage()
                .WithDescription("Change time to noon.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Noon);
            AddCommand("dusk")
                .SetDefaultUsage()
                .WithDescription("Change time to dusk.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Dusk);
            AddCommand("midnight")
                .SetDefaultUsage()
                .WithDescription("Change time to midnight.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Midnight);
            AddCommand("settle")
                .SetDefaultUsage()
                .WithDescription("Settle all water.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.Settle);
            AddCommand("fps")
                .SetDefaultUsage()
                .WithDescription("Toggle FPS monitoring.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(DefaultCommands.FPS);

            AddCommand("help")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Displays the commands available to the user.")
                .WithPermissionNode("tdsm.help")
                .Calls(DefaultCommands.ShowHelp);
            AddCommand("plugins")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists plugins running")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.plugins")
                .Calls(DefaultCommands.ListPlugins);
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
                .Calls(PluginManager.PluginCommand);
#endif
        }

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

        #if Full_API
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
        #endif

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
            if (Data.Storage.IsAvailable && perms == Data.Permission.Denied)
                return false;

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
#if Full_API
            if (sender is Player)
                return acc == AccessLevel.PLAYER || (acc == AccessLevel.OP && sender.Op);
#endif
//            if (sender is RConSender)
//                return acc <= AccessLevel.REMOTE_CONSOLE;

            if (ExtCheckAccessLevel != null && ExtCheckAccessLevel(acc, sender))
                return true;

            if (sender is ConsoleSender)
                return true;
            
            throw new NotImplementedException("Unexpected ISender implementation");
        }

        public static Dictionary<string, CommandInfo> GetAvailableCommands(AccessLevel access)
        {
            var available = TDSM.API.Callbacks.UserInput.CommandParser.serverCommands.GetAvailableCommands(access);

            //We *may* not need this - but let's see how it goes
            foreach (var plg in PluginManager.EnumeratePlugins)
            {
                var additional = plg.commands.GetAvailableCommands(access)
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

        /* Old permissions
        /// <summary>
        /// Permissions checking for registered commands.
        /// </summary>
        /// <param name="sender">Entity to check permissions for</param>
        /// <param name="cmd">Command to check for permissions on</param>
        /// <returns>True if entity can use command.  False if not.</returns>
        public static Permissions.Permission CheckPermissions(ISender sender, CommandInfo cmd)
        {
            / *
             *  [TODO] Should a node return false, Since there is three possibilites, should it return false if permissions 
             *  is enabled and allow the normal OP system work or no access at all?
             * /
            if (cmd.node == null || sender is ConsoleSender || sender.Op)
                return Permissions.Permission.Permitted;

            if (sender is BasePlayer && Permissions.PermissionsManager.IsSet)
                return Permissions.PermissionsManager.IsPermitted(cmd.node, sender as BasePlayer);

            return Permissions.Permission.Denied;
        }*/

        /// <summary>
        /// Permissions checking for registered commands.
        /// </summary>
        /// <param name="sender">Entity to check permissions for</param>
        /// <param name="cmd">Command to check for permissions on</param>
        /// <returns>True if entity can use command.  False if not.</returns>
        public static Data.Permission CheckPermissions(ISender sender, CommandInfo cmd)
        {
            if (cmd.node == null || sender is ConsoleSender || sender.Op)
                return Data.Permission.Permitted;

            if (sender is BasePlayer && Data.Storage.IsAvailable)
                return Data.Storage.IsPermitted(cmd.node, sender as BasePlayer);

            return Data.Permission.Denied;
        }

        bool FindStringCommand(string prefix, out CommandInfo info)
        {
            info = null;

            foreach (var plugin in PluginManager.EnumeratePlugins)
            {
                lock (plugin.commands)
                {
                    //if (plugin.IsEnabled && plugin.commands.TryGetValue(prefix, out info) && info.stringCallback != null)
                    if (plugin.IsEnabled && plugin.commands.TryGetValue(prefix, out info))
                    {
                        return info.stringCallback != null;
                    }
                }
            }

            if (serverCommands.TryGetValue(prefix, out info) && info.stringCallback != null)
                return true;

            return false;
        }

        bool FindTokenCommand(string prefix, out CommandInfo info)
        {
            info = null;

            foreach (var plugin in PluginManager.EnumeratePlugins)
            {
                lock (plugin.commands)
                {
                    if (plugin.IsEnabled && plugin.commands.TryGetValue(prefix, out info) && (info.tokenCallback != null || info.LuaCallback != null))
                        return true;
                }
            }

            if (serverCommands.TryGetValue(prefix, out info) && info.tokenCallback != null)
                return true;

            return false;
        }

        public void ParseAndProcess(ISender sender, string line)
        {
            var ctx = new HookContext
            {
                Sender = sender,
#if Full_API
                Player = sender as Player
#endif
            };

#if Full_API
            ctx.Connection = ctx.Player != null ? ctx.Player.Connection.Socket : null;
#endif

            var hargs = new HookArgs.Command();

            try
            {
                CommandInfo info;

                var firstSpace = line.IndexOf(' ');

                if (firstSpace < 0)
                    firstSpace = line.Length;

                var prefix = line.Substring(0, firstSpace);

                hargs.Prefix = prefix;

                if (FindStringCommand(prefix, out info))
                {
                    hargs.ArgumentString = (firstSpace < line.Length - 1 ? line.Substring(firstSpace + 1, line.Length - firstSpace - 1) : "").Trim();

                    HookPoints.Command.Invoke(ref ctx, ref hargs);

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
                    if (FindTokenCommand(command, out info))
                    {
                        hargs.Arguments = args;

                        foreach (BasePlugin plg in PluginManager._plugins.Values)
                        {
                            if (plg.commands.ContainsKey(command))
                            {
                                args.Plugin = plg;
                            }
                        }

                        HookPoints.Command.Invoke(ref ctx, ref hargs);

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
