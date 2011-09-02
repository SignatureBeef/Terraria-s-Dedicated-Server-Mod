using System;
using System.Text;
using System.Collections.Generic;
using Terraria_Server.Plugin;
using Terraria_Server.Events;
using Terraria_Server.RemoteConsole;
using Terraria_Server.Logging;

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

            AddCommand("exit")
                .WithDescription("Stop the server, save the world then exit program.")
                .WithAccessLevel(AccessLevel.CONSOLE)
                .Calls(Commands.Exit);

            AddCommand("stop")
                .WithDescription("Stop the server, save the world then exit program.")
                .WithAccessLevel(AccessLevel.CONSOLE)
                .Calls(Commands.Exit);

            AddCommand("save-all")
                .WithDescription("Save all world data and backup.")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.SaveAll);
            
            AddCommand("reload")
                .WithDescription ("Reload plugins.")
                .WithAccessLevel (AccessLevel.REMOTE_CONSOLE)
                .Calls (Commands.Reload);
                
            AddCommand("list")
                .WithAccessLevel (AccessLevel.PLAYER)
                .WithDescription ("List active players (also: who, players, online).")
                .Calls (Commands.List);
                
            AddCommand("who")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("List active players (also: who, players, online).")
                .Calls(Commands.List);

            AddCommand("players")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("List active players (also: who, players, online).")
                .Calls(Commands.OldList);

            // this is what the server crawler expects
            AddCommand("playing")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("List active players (also: who, players, online).")
                .Calls(Commands.OldList);

            AddCommand("online")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("List active players (also: who, players, online).")
                .Calls(Commands.List);
                
            AddCommand("me")
                .WithAccessLevel (AccessLevel.PLAYER)
                .WithDescription ("Broadcast a message in third person.")
                .Calls (Commands.Action);
                
            AddCommand("say")
                .WithAccessLevel (AccessLevel.PLAYER)
                .WithDescription ("Broadcast a message in first person.")
                .Calls (Commands.Say);
                
            AddCommand("slots")
                .WithDescription ("Display information about occupied player slots.")
                .WithHelpText ("Usage:   slots [-d] [-p]")
                .WithHelpText ("Options:")
                .WithHelpText ("         -d    display information helpful in debugging")
                .WithHelpText ("         -p    display additional player information")
                .Calls (Commands.Slots);

            AddCommand("kick")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription ("Kick a player by name or slot.")
                .WithHelpText ("Usage:   kick name")
                .WithHelpText ("         kick -s number")
                .Calls(Commands.Kick);

            AddCommand("ban")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Ban a player by Name or IP")
                .WithHelpText("Usage:   ban <name>")
                .WithHelpText("         ban <ip>")
                .Calls(Commands.Ban);

            AddCommand("unban")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("UnBan a player by Name or IP")
                .WithHelpText("Usage:   unban <name>")
                .Calls(Commands.UnBan);

            AddCommand("whitelist")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Add or remove a player or IP to the whitelist")
                .WithHelpText("Usage:   whitelist -add <name:ip>")
                .WithHelpText("         whitelist -remove <name:ip>")
                .Calls(Commands.WhiteList);
            
            AddCommand("rcon")
                .WithDescription ("Manage remote console access.")
                .WithAccessLevel (AccessLevel.REMOTE_CONSOLE)
                .WithHelpText ("Usage:   rcon load       - reload login database")
                .WithHelpText ("         rcon list       - list rcon connections")
                .WithHelpText ("         rcon cut <name> - cut off rcon connections")
                .WithHelpText ("         rcon ban <name> - cut off rcon connections and revoke access")
                .Calls (RConServer.RConCommand);
            
            AddCommand("status")
                .WithDescription ("Check the server's status")
                .WithHelpText ("Usage:   status")
                .Calls(Commands.Status);

            AddCommand("time")
                .WithDescription("Change the time of the World.")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("Usage:   time -set <time>")
                .WithHelpText("         time -now")
                .WithHelpText("         time day")
                .WithHelpText("         time dawn")
                .WithHelpText("         time dusk")
                .WithHelpText("         time noon")
                .WithHelpText("         time night")
                .Calls(Commands.Time);

            AddCommand("help")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Get a printout of active commands.")
                .WithHelpText("Usage:   help")
                .WithHelpText("         help <page>")
                .Calls(Commands.ShowHelp);

            AddCommand("give")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Give a player items.")
                .WithHelpText("Usage:   give <player> <amount> <itemname:itemid>")
                .Calls(Commands.Give);

            AddCommand("spawnnpc")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Spawn an NPC near a player.")
                .WithHelpText("Usage:   spawnnpc <amount> \"<name:id>\" \"<player>\"")
                .Calls(Commands.SpawnNPC);

            AddCommand("tp")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Teleport a player to another player.")
                .WithHelpText("Usage:   tp <player> <toplayer> - another player")
                .WithHelpText("         tp <player> <x> <y>")
                .WithHelpText("         tp <toplayer>          - yourself")
                .WithHelpText("         tp <x> <y>")
                .WithHelpText("         tp                     - yourself to spawn")
                .Calls(Commands.Teleport);

            AddCommand("tphere")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Teleport a player to yourself.")
                .WithHelpText("Usage:   tphere <player>")
                .Calls(Commands.TeleportHere);

            AddCommand("settle")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Settle Liquids.")
                .WithHelpText("Usage:   settle")
                .Calls(Commands.SettleWater);

            AddCommand("op")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Op a player")
                .WithHelpText("Usage:   op <password> <player>")
                .Calls(Commands.OpPlayer);

            AddCommand("deop")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("De-Op a player")
                .WithHelpText("Usage:   deop <player>")
                .Calls(Commands.DeopPlayer);

            AddCommand("oplogin")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("OP Login System.")
                .WithHelpText("Usage:   oplogin <password>")
                .Calls(Commands.OpLogin);

            AddCommand("oplogout")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("OP Logout System.")
                .WithHelpText("Usage:   oplogout")
                .Calls(Commands.OpLogout);

            AddCommand("npcspawns")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Toggle the state of NPC Spawning.")
                .WithHelpText("Usage:   npcspawns")
                .Calls(Commands.NPCSpawns);

            AddCommand("restart")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Restart the Server.")
                .WithHelpText("Usage:   restart")
                .Calls(Commands.Restart);
            
            AddCommand("purge")
                .WithAccessLevel (AccessLevel.OP)
                .WithDescription ("Purge the map of items, NPCs or projectiles.")
                .WithHelpText ("Usage:    purge all")
                .WithHelpText ("          purge proj[ectiles]")
                .WithHelpText ("          purge item[s]")
                .WithHelpText ("          purge npc[s]")
                .Calls(Commands.Purge);

            AddCommand("plugins")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("List currently enabled plugins.")
                .WithHelpText("Usage:    plugins")
                .Calls(Commands.ListPlugins);

            AddCommand("plugin")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Enable/disable and get details about specific plugins.")
                .WithHelpText("Usage:    plugin list")
                .WithHelpText("          plugin info <plugin>")
                .WithHelpText("          plugin enable <plugin>")
                .WithHelpText("          plugin disable <plugin>")
                .Calls(Commands.ManagePlugins);

            AddCommand("spawnboss")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Summon a Boss to the world. <Any Combination>")
                .WithDescription("  (No if no player is entered it will be a random online player)")
                .WithDescription("  (-night is mainly for Eye of Cthulu, but will set time to night for any Boss.)")
                .WithHelpText("Usage:    spawnboss -<eye, skeletron, eater, kingslime>")
                .WithHelpText("          spawnboss -eye -night")
                .WithHelpText("          spawnboss -eater -eye...")
                .WithHelpText("          spawnboss <boss> -player <name>")
                .Calls(Commands.SummonBoss);

            AddCommand("itemrej")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Add or remove an item form the whitelist.")
                .WithHelpText("Usage:    itemrej -add/-remove <id:name>")
                .Calls(Commands.ItemRejection);

            AddCommand("explosions")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Toggle the allowing of explosions for the server.")
                .WithHelpText("Usage:    explosions")
                .Calls(Commands.Explosions);

            AddCommand("maxplayers")
                .WithAccessLevel (AccessLevel.REMOTE_CONSOLE)
                .WithDescription ("Set the maximum number of player slots.")
                .WithHelpText ("Usage:    maxplayers <num> - set the max number of slots")
                .WithHelpText ("          maxplayers <num> <num> - also set the number of overlimit slots")
                .Calls (Terraria_Server.Networking.SlotManager.MaxPlayersCommand);
            
            AddCommand("q")
                .WithAccessLevel (AccessLevel.OP)
                .WithDescription ("List connections waiting in queues.")
                .WithHelpText ("Usage:    q")
                .Calls (Terraria_Server.Networking.SlotManager.QCommand);
            
            AddCommand ("refresh")
                .WithAccessLevel (AccessLevel.PLAYER)
                .WithDescription ("Redownload the area around you from the server.")
                .WithHelpText ("Usage:    refresh")
                .Calls (Commands.Refresh);
        }
       
        public readonly Dictionary<string, CommandInfo> serverCommands;
        
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
        
        public static bool CheckAccessLevel (CommandInfo cmd, ISender sender)
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
                    if (FindTokenCommand(command, out info))
                    {
                        if (!CheckAccessLevel(info, sender))
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
                    else
                    {
                        sender.sendMessage("No such command.");
                    }
                }
            }
            catch (TokenizerException e)
            {
                sender.sendMessage (e.Message);
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
						else if ((c != '\\' && c != ' ') || l == '\\')
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
