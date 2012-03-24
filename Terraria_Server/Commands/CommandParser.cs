using System;
using System.Text;
using System.Collections.Generic;
using Terraria_Server.Plugins;
using Terraria_Server.RemoteConsole;
using Terraria_Server.Logging;
using Terraria_Server.Misc;
using Terraria_Server.Permissions;
using Terraria_Server.Language;

namespace Terraria_Server.Commands
{
	public enum AccessLevel : int
	{
		PLAYER,
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

		internal void InitFrom(CommandInfo other)
		{
			description = other.description;
			helpText = other.helpText;
			accessLevel = other.accessLevel;
			tokenCallback = other.tokenCallback;
			stringCallback = other.stringCallback;
			ClearEvents();
		}

		internal void ClearCallbacks()
		{
			tokenCallback = null;
			stringCallback = null;
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

		public CommandInfo WithAccessLevel(AccessLevel accessLevel)
		{
			this.accessLevel = accessLevel;
			return this;
		}

		public CommandInfo WithPermissionNode(string node)
		{
			this.node = node;
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

		public void ShowHelp(ISender sender)
		{
			foreach (var line in helpText)
				sender.sendMessage(line);
		}

		internal void Run(ISender sender, string args)
		{
			if (BeforeEvent != null)
				BeforeEvent(this);

			try
			{
				if (!CommandParser.CheckAccessLevel(this, sender))
				{
					sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
					return;
				}

				if (stringCallback != null)
					stringCallback(sender, args);
				else
					sender.sendMessage(Languages.ExpiredCommandMessage, 255, 238, 130, 238);
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
					sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
					return;
				}

				if (tokenCallback != null)
					tokenCallback(sender, args);
				else
					sender.sendMessage(Languages.ExpiredCommandMessage, 255, 238, 130, 238);
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

			AddCommand("exit")
				.WithDescription(Languages.CommandDescription_Exit)
				.WithAccessLevel(AccessLevel.CONSOLE)
				.WithPermissionNode("tdsm.admin")
				.Calls(Commands.Exit);

			AddCommand("stop")
				.WithDescription(Languages.CommandDescription_Exit)
				.WithAccessLevel(AccessLevel.CONSOLE)
				.WithPermissionNode("tdsm.admin")
				.Calls(Commands.Exit);

			AddCommand("save-all")
				.WithDescription(Languages.CommandDescription_SaveAll)
				.WithAccessLevel(AccessLevel.OP)
				.WithPermissionNode("tdsm.admin")
				.Calls(Commands.SaveAll);

			AddCommand("reload")
				.WithDescription(Languages.CommandDescription_ReloadPlugins)
				.WithAccessLevel(AccessLevel.REMOTE_CONSOLE)
				.WithPermissionNode("tdsm.plugin")
				.Calls(Commands.Reload);

			AddCommand("list")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_OnlinePlayers)
				.WithPermissionNode("tdsm.who")
				.Calls(Commands.List);

			AddCommand("who")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_OnlinePlayers)
				.WithPermissionNode("tdsm.who")
				.Calls(Commands.List);

			AddCommand("players")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_OnlinePlayers)
				.WithPermissionNode("tdsm.who")
				.Calls(Commands.OldList);

			// this is what the server crawler expects
			AddCommand("playing")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_OnlinePlayers)
				.WithPermissionNode("tdsm.who")
				.Calls(Commands.OldList);

			AddCommand("online")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_OnlinePlayers)
				.WithPermissionNode("tdsm.who")
				.Calls(Commands.List);

			AddCommand("me")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_Me)
				.WithPermissionNode("tdsm.me")
				.Calls(Commands.Action);

			AddCommand("say")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Say)
				.WithPermissionNode("tdsm.say")
				.Calls(Commands.Say);

			AddCommand("slots")
				.WithDescription(Languages.CommandDescription_Slots)
				.WithHelpText("Usage:   slots [-d] [-p]")
				.WithHelpText("Options:")
				.WithHelpText("         -d    display information helpful in debugging")
				.WithHelpText("         -p    display additional player information")
				.WithPermissionNode("tdsm.slots")
				.Calls(Commands.Slots);

			AddCommand("kick")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Kick)
				.WithHelpText("Usage:   kick name")
				.WithHelpText("         kick -s number")
				.WithPermissionNode("tdsm.kick")
				.Calls(Commands.Kick);

			AddCommand("ban")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Ban)
				.WithHelpText("Usage:   ban <name>")
				.WithHelpText("         ban <ip>")
				.WithPermissionNode("tdsm.ban")
				.Calls(Commands.Ban);

			AddCommand("unban")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_UnBan)
				.WithHelpText("Usage:   unban <name>")
				.WithPermissionNode("tdsm.unban")
				.Calls(Commands.UnBan);

			AddCommand("whitelist")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Whitelist)
				.WithHelpText("Usage:   whitelist -add <name:ip>")
				.WithHelpText("         whitelist -remove <name:ip>")
				.WithPermissionNode("tdsm.whitelist")
				.Calls(Commands.WhiteList);

			AddCommand("rcon")
				.WithDescription(Languages.CommandDescription_Rcon)
				.WithAccessLevel(AccessLevel.REMOTE_CONSOLE)
				.WithHelpText("Usage:   rcon load       - reload login database")
				.WithHelpText("         rcon list       - list rcon connections")
				.WithHelpText("         rcon cut <name> - cut off rcon connections")
				.WithHelpText("         rcon ban <name> - cut off rcon connections and revoke access")
				.WithHelpText("         rcon add <name> <password> - add an rcon user")
				.WithPermissionNode("tdsm.rcon")
				.Calls(RConServer.RConCommand);

			AddCommand("status")
				.WithDescription(Languages.CommandDescription_Status)
				.WithHelpText("Usage:   status")
				.WithPermissionNode("tdsm.status")
				.Calls(Commands.Status);

			AddCommand("time")
				.WithDescription(Languages.CommandDescription_Time)
				.WithAccessLevel(AccessLevel.OP)
				.WithHelpText("Usage:   time -set <time>")
				.WithHelpText("         time -now")
				.WithHelpText("         time day|dawn|dusk|noon|night")
				.WithPermissionNode("tdsm.time")
				.Calls(Commands.Time);

			AddCommand("help")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_Help)
				.WithHelpText("Usage:   help")
				.WithHelpText("         help <page>")
				.WithPermissionNode("tdsm.help")
				.Calls(Commands.ShowHelp);

			AddCommand("give")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Give)
				.WithHelpText("Usage:   give <player> <amount> <itemname:itemid> [-prefix]")
				.WithPermissionNode("tdsm.give")
				.Calls(Commands.Give);

			AddCommand("spawnnpc")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_SpawnNpc)
				.WithHelpText("Usage:   spawnnpc <amount> \"<name:id>\" \"<player>\"")
				.WithPermissionNode("tdsm.spawnnpc")
				.Calls(Commands.SpawnNPC);

			AddCommand("tp")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Teleport)
				.WithHelpText("Usage:   tp <player> <toplayer> - another player")
				.WithHelpText("         tp <player> <x> <y>")
				.WithHelpText("         tp <toplayer>          - yourself")
				.WithHelpText("         tp <x> <y>")
				.WithHelpText("         tp                     - yourself to spawn")
				.WithPermissionNode("tdsm.tp")
				.Calls(Commands.Teleport);

			AddCommand("tphere")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_TeleportHere)
				.WithHelpText("Usage:   tphere <player>")
				.WithPermissionNode("tdsm.tphere")
				.Calls(Commands.TeleportHere);

			AddCommand("settle")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_SettleLiquids)
				.WithHelpText("Usage:   settle")
				.WithPermissionNode("tdsm.settle")
				.Calls(Commands.SettleWater);

			AddCommand("op")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Op)
				.WithHelpText("Usage:   op <player> <password>")
				.WithPermissionNode("tdsm.op")
				.Calls(Commands.OpPlayer);

			AddCommand("deop")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_DeOp)
				.WithHelpText("Usage:   deop <player>")
				.WithPermissionNode("tdsm.deop")
				.Calls(Commands.DeopPlayer);

			AddCommand("oplogin")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_OpLogin)
				.WithHelpText("Usage:   oplogin <password>")
				.WithPermissionNode("tdsm.oplogin")
				.Calls(Commands.OpLogin);

			AddCommand("oplogout")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_OpLogout)
				.WithHelpText("Usage:   oplogout")
				.WithPermissionNode("tdsm.oplogout")
				.Calls(Commands.OpLogout);

			AddCommand("npcspawns")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_NpcSpawns)
				.WithHelpText("Usage:   npcspawns")
				.WithPermissionNode("tdsm.npcspawns")
				.Calls(Commands.NPCSpawns);

			AddCommand("restart")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Restart)
				.WithHelpText("Usage:   restart")
				.WithPermissionNode("tdsm.admin")
				.Calls(Commands.Restart);

			AddCommand("purge")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Purge)
				.WithHelpText("Usage:    purge all")
				.WithHelpText("          purge proj[ectiles]")
				.WithHelpText("          purge item[s]")
				.WithHelpText("          purge npc[s]")
				.WithPermissionNode("tdsm.purge")
				.Calls(Commands.Purge);

			AddCommand("plugins")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_Plugins)
				.WithHelpText("Usage:    plugins")
				.WithPermissionNode("tdsm.plugins")
				.Calls(Commands.ListPlugins);

			AddCommand("plugin")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Plugin)
				.WithHelpText("Usage:    plugin list")
				.WithHelpText("          plugin stat")
				.WithHelpText("          plugin info <plugin>")
				.WithHelpText("          plugin enable <plugin>")
				.WithHelpText("          plugin disable <plugin>")
				.WithHelpText("          plugin reload [-clean] all|<plugin>")
				.WithHelpText("          plugin unload all|<plugin>")
				.WithHelpText("          plugin load [-replace] <file>")
				.WithPermissionNode("tdsm.plugin")
				.Calls(PluginManager.PluginCommand);

			AddCommand("spawnboss")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_SpawnBoss)
				.WithDescription("  (No if no player is entered it will be a random online player)")
				.WithDescription("  (-night is mainly for Eye of Cthulu, but will set time to night for any Boss.)")
				.WithHelpText("Usage:    spawnboss <eye, skeletron, eater, kingslime, prime, twins, destroyer, wof>")
				.WithHelpText("          spawnboss eye night")
				.WithHelpText("          spawnboss eater eye...")
				.WithHelpText("          spawnboss <boss> -player <name>")
				.WithPermissionNode("tdsm.spawnboss")
				.Calls(Commands.SummonBoss);

			AddCommand("itemrej")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_ItemRej)
				.WithHelpText("Usage:    itemrej -add/-remove <id:name>")
				.WithPermissionNode("tdsm.itemrej")
				.Calls(Commands.ItemRejection);

			AddCommand("explosions")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Explosions)
				.WithHelpText("Usage:    explosions")
				.WithPermissionNode("tdsm.explosions")
				.Calls(Commands.Explosions);

			AddCommand("maxplayers")
				.WithAccessLevel(AccessLevel.REMOTE_CONSOLE)
				.WithDescription(Languages.CommandDescription_MaxPlayers)
				.WithHelpText("Usage:    maxplayers <num> - set the max number of slots")
				.WithHelpText("          maxplayers <num> <num> - also set the number of overlimit slots")
				.WithPermissionNode("tdsm.maxplayers")
				.Calls(Terraria_Server.Networking.SlotManager.MaxPlayersCommand);

			AddCommand("q")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Q)
				.WithHelpText("Usage:    q")
				.WithPermissionNode("tdsm.q")
				.Calls(Terraria_Server.Networking.SlotManager.QCommand);

			AddCommand("refresh")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription(Languages.CommandDescription_Refresh)
				.WithHelpText("Usage:    refresh")
				.WithPermissionNode("tdsm.refresh")
				.Calls(Commands.Refresh);

			/*AddCommand("rpg")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription("Toggle whether RPG Client can use this server.")
				.WithHelpText("Usage:    rpg")
				.WithPermissionNode("tdsm.rpg")
				.Calls(Commands.ToggleRPGClients);

			AddCommand("spawngiver")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription("Spawn a TDCM Quest Giver.")
				.WithHelpText("Usage:    spawngiver")
				.WithPermissionNode("tdsm.spawngiver")
				.Calls(Commands.SpawnQuestGiver);*/

			AddCommand("hardmode")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_HardMode)
				.WithPermissionNode("tdsm.hardmode")
				.Calls(Commands.HardMode);

			AddCommand("languagereload")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_LanguageReload)
				.WithPermissionNode("tdsm.languagereload")
				.Calls(Commands.LanguageReload);

			AddCommand("backup")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription(Languages.CommandDescription_Backups)
				.WithHelpText("Usage:    backup now")
				.WithHelpText("          backup purge <minutes>")
				.WithPermissionNode("tdsm.backup")
				.Calls(Commands.Backups);

			AddCommand("timelock")
				.WithAccessLevel(AccessLevel.OP)
				.WithDescription("Forces the time to stay at a certain point.")
				.WithHelpText("Usage:    timelock now")
				.WithHelpText("          timelock set day|dawn|dusk|noon|night")
				.WithHelpText("          timelock setat <time>")
				.WithHelpText("          timelock disable")
				.WithPermissionNode("tdsm.timelock")
				.Calls(Commands.Timelock);
		}

		public readonly Dictionary<String, CommandInfo> serverCommands;

		/// <summary>
		/// Reads active permission nodes, Player only.
		/// </summary>
		public void ReadPermissionNodes()
		{
			foreach (CommandInfo info in serverCommands.Values)
			{
				if (info.accessLevel == AccessLevel.PLAYER)
				{
					if (info.node.Trim().Length > 0 && !Program.permissionManager.ActiveNodes.Contains(info.node))
						Program.permissionManager.ActiveNodes.Add(info.node);
				}
			}
		}

		/// <summary>
		/// Registers new command
		/// </summary>
		/// <param name="prefix">The text attached to the / that will be registered as the command.</param>
		/// <returns>CommandInfo for new command</returns>
		public CommandInfo AddCommand(string prefix)
		{
			if (serverCommands.ContainsKey(prefix)) throw new ApplicationException("AddCommand: duplicate command: " + prefix);

			var cmd = new CommandInfo();
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
			return CheckPermissions(sender, cmd) ? true : CheckAccessLevel(cmd.accessLevel, sender);
		}

		/// <summary>
		/// Determines the access level of the sender. Used when no permissions plugin is found.
		/// </summary>
		/// <param name="acc">Access level to check against</param>
		/// <param name="sender">Sender to check</param>
		/// <returns>True if sender has access level equal to or greater than acc</returns>
		public static bool CheckAccessLevel(AccessLevel acc, ISender sender)
		{
			if (sender is Player) return acc == AccessLevel.PLAYER || (acc == AccessLevel.OP && sender.Op);
			if (sender is RConSender) return acc <= AccessLevel.REMOTE_CONSOLE;
			if (sender is ConsoleSender) return true;
			throw new NotImplementedException("Unexpected ISender implementation");
		}

		/// <summary>
		/// Permissions checking for registered commands.
		/// </summary>
		/// <param name="sender">Entity to check permissions for</param>
		/// <param name="cmd">Command to check for permissions on</param>
		/// <returns>True if entity can use command.  False if not.</returns>
		public static bool CheckPermissions(ISender sender, CommandInfo cmd)
		{
			/*
			 *  [TODO] Should a node return false, Since there is three possibilites, should it return false if permissions 
			 *  is enabled and allow the normal OP system work or no access at all?
			 */
			if (cmd.node == null || sender is ConsoleSender || sender.Op)
				return true;

			if (sender is Player && Program.permissionManager.IsPermittedImpl != null && Statics.PermissionsEnabled)
				return Program.permissionManager.IsPermittedImpl(cmd.node, sender as Player);

			return false;
		}

		bool FindStringCommand(string prefix, out CommandInfo info)
		{
			info = null;

			foreach (var plugin in PluginManager.EnumeratePlugins)
			{
				lock (plugin.commands)
				{
					if (plugin.IsEnabled && plugin.commands.TryGetValue(prefix, out info) && info.stringCallback != null)
						return true;
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
					if (plugin.IsEnabled && plugin.commands.TryGetValue(prefix, out info) && info.tokenCallback != null)
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
				Player = sender as Player,
			};

			ctx.Connection = ctx.Player != null ? ctx.Player.Connection : null;

			var hargs = new HookArgs.Command();

			try
			{
				CommandInfo info;

				var firstSpace = line.IndexOf(' ');

				if (firstSpace < 0) firstSpace = line.Length;

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
						sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
						return;
					}

					try
					{
						info.Run(sender, hargs.ArgumentString);
					}
					catch (ExitException e)
					{
						throw e;
					}
					catch (CommandError e)
					{
						sender.sendMessage(prefix + ": " + e.Message);
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

						foreach (BasePlugin plg in PluginManager.plugins.Values)
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
							sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
							return;
						}

						try
						{
							info.Run(sender, hargs.Arguments);
						}
						catch (ExitException e)
						{
							throw e;
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
			catch (ExitException e)
			{
				throw e;
			}
			catch (TokenizerException e)
			{
				sender.sendMessage(e.Message);
			}
		}

		class TokenizerException : Exception
		{
			public TokenizerException(string message) : base(message) { }
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
