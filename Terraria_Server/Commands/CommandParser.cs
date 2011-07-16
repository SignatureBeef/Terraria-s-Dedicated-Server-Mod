using System;
using System.Text;
using System.Collections.Generic;
using Terraria_Server.Plugin;

namespace Terraria_Server.Commands
{
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
        }

        /// <summary>
        /// Parses new console command
        /// </summary>
        /// <param name="Line">Command to parse</param>
        /// <param name="server">Current Server instance</param>
        public void parseConsoleCommand(String Line, Server server)
        {
            ConsoleSender cSender = new ConsoleSender(server);
            cSender.ConsoleCommand.Message = Line;
            Sender sender = new Sender();
            sender.Op = true;
            cSender.ConsoleCommand.Sender = sender;
            server.PluginManager.processHook(Hooks.CONSOLE_COMMAND, cSender.ConsoleCommand);
            if (cSender.ConsoleCommand.Cancelled)
            {
                return;
            }

            try
            {
                var tokens = Tokenize (Line.Trim());

                if (tokens.Count > 0)
                {
                    switchCommands (tokens, cSender.ConsoleCommand.Sender);
                }
            }
            catch (TokenizerException e)
            {
                sender.sendMessage (e.Message);
            }
        }

        /// <summary>
        /// Parses player commands
        /// </summary>
        /// <param name="player">Sending player</param>
        /// <param name="Line">Command to parse</param>
        public void parsePlayerCommand(Player player, String Line)
        {
            if (Line.StartsWith("/"))
            {
                Line = Line.Remove(0, 1);
                
                try
                {
                    var tokens = Tokenize (Line.Trim());

                    if (tokens.Count > 0)
                    {
                        switchCommands (tokens, player);
                    }
                }
                catch (TokenizerException e)
                {
                    player.sendMessage (e.Message);
                }
            }
        }

        /// <summary>
        /// Executes command methods derived from parsing
        /// </summary>
        /// <param name="tokens">Command arguments to pass to methods</param>
        /// <param name="sender">Sending player</param>
        public void switchCommands (IList<string> tokens, ISender sender)
        {
            switch (Commands.getCommandValue(tokens[0]))
            {
                case (int)Commands.Command.NO_SUCH_COMMAND:
                    {
                        sender.sendMessage("No such command!");
                        return;
                    }
                case (int)Commands.Command.CONSOLE_EXIT:
                    {
                        Commands.Exit(Program.server, sender);
                        break;
                    }
                case (int)Commands.Command.COMMAND_RELOAD:
                    {
                        Commands.Reload(Program.server, sender);
                        break;
                    }
                case (int)Commands.Command.COMMAND_LIST:
                    {
                        if (sender is Player)
                        {
                            Commands.List(((Player)sender).whoAmi);
                        }
                        else
                        {
                            Program.tConsole.WriteLine(Commands.List(0, false));
                        }
                        break;
                    }
                case (int)Commands.Command.COMMAND_PLAYERS:
                    {
                        //same stuff, Just making it easier.
                        goto case (int)Commands.Command.COMMAND_LIST;
                    }
                case (int)Commands.Command.PLAYER_ME:
                    {
                        String Message = string.Join (" ", tokens);
                        if (Message.Length <= 3) { return; }

                        if (sender is Player)
                        {
                            Commands.Me_Say(Message.Remove(0, 3).Trim(), ((Player)sender).whoAmi);
                        }
                        else
                        {
                            Commands.Me_Say(Message.Remove(0, 4).Trim(), -1); //turn command list into a String and remove "say "
                        }
                        break;
                    }
                case (int)Commands.Command.CONSOLE_SAY:
                    {
                        goto case (int)Commands.Command.PLAYER_ME;
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
                case (int)Commands.Command.COMMAND_KICK:
                    {
                        Commands.Kick(sender, tokens);
                        break;
                    }
                case (int)Commands.Command.COMMAND_RESTART:
                    {
                        Commands.Restart(sender, server);
                        break;
                    }
                case (int)Commands.Command.COMMAND_STOP:
                    {
                        goto case (int)Commands.Command.CONSOLE_EXIT;
                    }
                case (int)Commands.Command.COMMAND_SLOTS:
                    {
                        Commands.Slots (sender, server);
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
		/// </summary>
		/// <param name="commands">Whole command line without trailing newline </param>
		public static List<string> Tokenize (string command)
		{
			char l = '\0';
			var result = new List<string> ();
			var b = new StringBuilder ();
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
							result.Add (b.ToString());
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
				result.Add (b.ToString());
			
			return result;
		}

    }
}
