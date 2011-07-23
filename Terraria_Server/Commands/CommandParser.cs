using System;
using System.Text;
using System.Collections.Generic;
using Terraria_Server.Plugin;
using Terraria_Server.Events;
using Terraria_Server.RemoteConsole;

namespace Terraria_Server.Commands
{
    public enum AccessLevel
    {
        PLAYER,
        OP,
        REMOTE_CONSOLE,
        CONSOLE,
    }

    public class CommandInfo
    {
        internal string description;
        internal List<string> helpText = new List<string> ();
        internal AccessLevel accessLevel = AccessLevel.OP;
        internal Action<Server, ISender, ArgumentList> tokenCallback;
        internal Action<Server, ISender, string> stringCallback;
        
        public CommandInfo WithDescription (string desc)
        {
            description = desc;
            return this;
        }
        
        public CommandInfo WithHelpText (string help)
        {
            helpText.Add (help);
            return this;
        }
        
        public CommandInfo WithAccessLevel (AccessLevel accessLevel)
        {
            this.accessLevel = accessLevel;
            return this;
        }
        
        public CommandInfo Calls (Action<Server, ISender, ArgumentList> callback)
        {
            tokenCallback = callback;
            return this;
        }

        public CommandInfo Calls (Action<Server, ISender, string> callback)
        {
            stringCallback = callback;
            return this;
        }
        
        public void ShowHelp (ISender sender)
        {
            foreach (var line in helpText)
                sender.sendMessage (line);
        }
    }

    public class CommandParser
    {
        /// <summary>
        /// Server instance current CommandParser runs on
        /// </summary>
        public Server server = null;

        /// <summary>
        /// CommandParser constructor
        /// </summary>
        /// <param name="Server">Current Server instance</param>
        public CommandParser(Server Server)
        {
            server = Server;
            serverCommands = new Dictionary<string, CommandInfo> ();
            
            AddCommand ("exit")
                .WithDescription ("Stop the server, save the world then exit program.")
                .WithAccessLevel (AccessLevel.CONSOLE)
                .Calls (Commands.Exit);
            
            AddCommand ("reload")
                .WithDescription ("Reload plugins.")
                .WithAccessLevel (AccessLevel.REMOTE_CONSOLE)
                .Calls (Commands.Reload);
                
            AddCommand ("list")
                .WithAccessLevel (AccessLevel.PLAYER)
                .WithDescription ("List active players (also: who, players, online).")
                .Calls (Commands.List);
                
            AddCommand ("who")
                .WithAccessLevel (AccessLevel.PLAYER)
                .Calls(Commands.List);

            AddCommand("players")
                .WithAccessLevel (AccessLevel.PLAYER)
                .Calls(Commands.OldList);

            AddCommand("online")
                .WithAccessLevel (AccessLevel.PLAYER)
                .Calls(Commands.List);
                
            AddCommand ("me")
                .WithAccessLevel (AccessLevel.PLAYER)
                .WithDescription ("Broadcast a message in third person.")
                .Calls (Commands.Action);
                
            AddCommand ("say")
                .WithAccessLevel (AccessLevel.PLAYER)
                .WithDescription ("Broadcast a message in first person.")
                .Calls (Commands.Say);
                
            AddCommand ("slots")
                .WithDescription ("Display information about occupied player slots.")
                .WithHelpText ("Usage:   slots [-d] [-p]")
                .WithHelpText ("Options:")
                .WithHelpText ("         -d    display information helpful in debugging")
                .WithHelpText ("         -p    display additional player information")
                .Calls (Commands.Slots);
            
            AddCommand ("kick")
                .WithDescription ("Kick a player by name or slot.")
                .WithHelpText ("Usage:   kick name")
                .WithHelpText ("         kick -s number")
                .Calls (Commands.Kick);
            
            AddCommand ("rcon")
                .WithDescription ("Manage remote console access.")
                .WithAccessLevel (AccessLevel.REMOTE_CONSOLE)
                .WithHelpText ("Usage:   rcon load       - reload login database")
                .WithHelpText ("         rcon list       - list rcon connections")
                .WithHelpText ("         rcon cut <name> - cut off rcon connections")
                .WithHelpText ("         rcon ban <name> - cut off rcon connections and revoke access")
                .Calls (RConServer.RConCommand);
            
            AddCommand ("status")
                .WithDescription ("Check the server's status")
                .WithHelpText ("Usage:   status")
                .Calls (Commands.Status);
        }
       
        readonly Dictionary<string, CommandInfo> serverCommands;
        
        public CommandInfo AddCommand (string prefix)
        {
            if (serverCommands.ContainsKey (prefix)) throw new ApplicationException ("AddCommand: duplicate command: " + prefix);
            
            var cmd = new CommandInfo ();
            serverCommands[prefix] = cmd;
            serverCommands["." + prefix] = cmd;
            
            return cmd;
        }
        
        static ConsoleSender consoleSender = new ConsoleSender ();
        /// <summary>
        /// Parses new console command
        /// </summary>
        /// <param name="line">Command to parse</param>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sender of the Command</param>
		public void ParseConsoleCommand (string line, Server server, ConsoleSender sender = null)
		{
			line = line.Trim();
		
			if (sender == null)
			{
				sender = consoleSender;
			}
			
			var ev = new ConsoleCommandEvent ();
			ev.Sender = sender;
			ev.Message = line;
			server.PluginManager.processHook (Hooks.CONSOLE_COMMAND, ev);
			if (ev.Cancelled)
			{
				return;
			}
			
			ParseAndProcess (sender, line);
		}

        /// <summary>
        /// Parses player commands
        /// </summary>
        /// <param name="player">Sending player</param>
        /// <param name="line">Command to parse</param>
        public void ParsePlayerCommand (Player player, string line)
        {
            if (line.StartsWith("/"))
            {
                line = line.Remove(0, 1);
                
                ParseAndProcess (player, line);
            }
        }
        
        static bool CheckAccessLevel (CommandInfo cmd, ISender sender)
        {
            if (sender is Player) return cmd.accessLevel == AccessLevel.PLAYER || (cmd.accessLevel == AccessLevel.OP && sender.Op);
            if (sender is RConSender) return cmd.accessLevel <= AccessLevel.REMOTE_CONSOLE;
            if (sender is ConsoleSender) return true;
            throw new NotImplementedException ("Unexpected ISender implementation");
        }
        
        bool FindStringCommand (string prefix, out CommandInfo info)
        {
            info = null;
            
            foreach (var plugin in server.PluginManager.Plugins.Values)
            {
                if (plugin.commands.TryGetValue (prefix, out info) && info.stringCallback != null)
                    return true;
            }
            
            if (serverCommands.TryGetValue (prefix, out info) && info.stringCallback != null)
                return true;
            
            return false;
        }
        
        bool FindTokenCommand (string prefix, out CommandInfo info)
        {
            info = null;
            
            foreach (var plugin in server.PluginManager.Plugins.Values)
            {
                if (plugin.commands.TryGetValue (prefix, out info) && info.tokenCallback != null)
                    return true;
            }
            
            if (serverCommands.TryGetValue (prefix, out info) && info.tokenCallback != null)
                return true;
            
            return false;
        }

        public void ParseAndProcess (ISender sender, string line)
        {
            try
            {
                CommandInfo info;
                
                var firstSpace = line.IndexOf (' ');
                
                if (firstSpace < 0) firstSpace = line.Length;
                
                var prefix = line.Substring (0, firstSpace);
                if (FindStringCommand (prefix, out info))
                {
                    if (! CheckAccessLevel (info, sender))
                    {
                        sender.sendMessage ("You cannot perform that action.", 255, 238, 130, 238);
                        return;
                    }
                    
                    try
                    {
                        var rest = firstSpace < line.Length - 1 ? line.Substring (firstSpace + 1, line.Length - firstSpace - 1) : ""; 
                        info.stringCallback (server, sender, rest.Trim());
                    }
                    catch (CommandError e)
                    {
                        sender.sendMessage (prefix + ": " + e.Message);
                        info.ShowHelp (sender);
                    }
                    return;
                }
                
                var args = new ArgumentList (server);
                var command = Tokenize (line, args);

                if (command != null)
                {
                    if (serverCommands.TryGetValue(command, out info) && info.tokenCallback != null)
                    {
                        if (! CheckAccessLevel (info, sender))
                        {
                            sender.sendMessage("You cannot perform that action.", 255, 238, 130, 238);
                            return;
                        }

                        try
                        {
                            info.tokenCallback(server, sender, args);
                        }
                        catch (CommandError e)
                        {
                            sender.sendMessage(command + ": " + e.Message);
                            info.ShowHelp(sender);
                        }
                        return;
                    }

                    switchCommands(command, args, sender);
                }
            }
            catch (TokenizerException e)
            {
                sender.sendMessage (e.Message);
            }
        }
        
        // TODO: refactor remaining commands to be registered with AddCommand and remove this
        /// <summary>
        /// Executes command methods derived from parsing
        /// </summary>
        /// <param name="command">Command base to run</param>
        /// <param name="tokens">Command arguments to pass to methods</param>
        /// <param name="sender">Sending player</param>
        public void switchCommands (string command, IList<string> tokens, ISender sender)
        {
            tokens.Insert (0, command); // for compatibility with old code
            switch (Commands.getCommandValue (command))
            {
                case (int)Commands.Command.NO_SUCH_COMMAND:
                    {
                        sender.sendMessage("No such command!");
                        return;
                    }
                case (int)Commands.Command.COMMAND_SAVE_ALL:
                    {
                        Commands.SaveAll(sender);
                        break;
                    }
                case (int)Commands.Command.COMMAND_HELP:
                    {
                        if (tokens.Count > 1)
                        {
                            Commands.ShowHelp(sender, tokens);
                        }
                        else
                        {
                            Commands.ShowHelp(sender);
                        }
                        break;
                    }
                case (int)Commands.Command.COMMAND_WHITELIST:
                    {
                        Commands.WhiteList(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.COMMAND_BAN:
                    {
                        Commands.BanList(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.COMMAND_UNBAN:
                    {
                        Commands.BanList(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.COMMAND_TIME:
                    {
                        Commands.Time(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.COMMAND_GIVE:
                    {
                        Commands.Give(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.PLAYER_SPAWNNPC:
                    {
                        Commands.SpawnNPC(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.COMMAND_TELEPORT:
                    {
                        Commands.Teleport(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.PLAYER_TPHERE:
                    {
                        Commands.TeleportHere(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.COMMAND_SETTLEWATER:
                    {
                        Commands.SettleWater(sender);
                        break;
                    }
                case (int)Commands.Command.COMMAND_OP:
                    {
                        Commands.OP(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.COMMAND_DEOP:
                    {
                        Commands.OP(sender, tokens, true);
                        break;
                    }
                case (int)Commands.Command.PLAYER_OPLOGIN:
                    {
                        Commands.OPLoginOut(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.PLAYER_OPLOGOUT:
                    {
                        Commands.OPLoginOut(sender, tokens, true);
                        break;
                    }
                case (int)Commands.Command.COMMAND_NPCSPAWN:
                    {
                        Commands.NPCSpawns(sender);
                        break;
                    }
                case (int)Commands.Command.COMMAND_RESTART:
                    {
                        Commands.Restart(sender, server);
                        break;
                    }
                default:
                    {
                        Program.tConsole.WriteLine("Uknown Command Issued.");
                        break;
                    }
            }
        }
		
		class TokenizerException : Exception
		{
			public TokenizerException (string message) : base (message) {}
		}
		
		/// <summary>
		/// Splits a command on spaces, with support for "parameters in quotes" and non-breaking\ spaces.
		/// Literal quotes need to be escaped like this: \"
		/// Literal backslashes need to escaped like this: \\
		/// Returns the first token
		/// </summary>
        /// <param name="command">Whole command line without trailing newline </param>
        /// <param name="args">An empty list to put the arguments in </param>
		public static string Tokenize (string command, List<string> args)
		{
			char l = '\0';
			var b = new StringBuilder ();
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
								args.Add (b.ToString());
							b.Length = 0;
						}
						else if (c != '\\' || l == '\\')
						{
							b.Append (c);
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
							b.Append (c);
							c = '\0';
						}
					}
					break;
				}
				l = c;
			}
			
			if (s == 1)
				throw new TokenizerException ("Unmatched quote in command.");
			
			if (b.Length > 0)
			{
				if (result == null)
					result = b.ToString();
				else
					args.Add (b.ToString());
			}
			
			return result;
		}
		
		// for binary compatibility
		public static List<string> Tokenize (string command)
		{
			List<string> args = new List<string> ();
			args.Insert (0, Tokenize (command, args));
			return args;
		}
    }
}
