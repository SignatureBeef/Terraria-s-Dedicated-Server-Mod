using Terraria_Server.Plugin;
using System;

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

            if (Line.Contains("\""))
            {
                String[] commands = new String[Line.Substring(0, Line.IndexOf("\"")).Split(' ').Length + Line.Substring(Line.LastIndexOf("\"")).Split(' ').Length - 1];
                String[] temp = Line.Substring(0, Line.IndexOf("\"")).Trim().Split(' ');
                String[] temp2 = Line.Substring(Line.LastIndexOf("\"") + 1).Trim().Split(' ');
                String[] temp3 = new String[temp.Length + 1];
                temp.CopyTo(temp3, 0);

                temp3[temp3.Length - 1] = Line.Substring(Line.IndexOf("\""), Line.LastIndexOf("\"") - Line.IndexOf("\"")).Remove(0,1);

                temp3.CopyTo(commands, 0);
                temp2.CopyTo(commands, temp3.Length);

                if (commands == null || commands.Length <= 0)
                {
                    Program.tConsole.WriteLine("Issue parsing Console Command for " + Hooks.CONSOLE_COMMAND.ToString());
                    return;
                }
                switchCommands(commands, cSender.ConsoleCommand.Sender);

            }
            else
            {
                String[] commands = Line.Trim().ToLower().Split(' ');

                if (commands == null || commands.Length <= 0)
                {
                    Program.tConsole.WriteLine("Issue parsing Console Command for " + Hooks.CONSOLE_COMMAND.ToString());
                    return;
                }
                switchCommands(commands, cSender.ConsoleCommand.Sender);
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
            }
            if (Line.Contains("\""))
            {
                String[] commands = new String[Line.Substring(0, Line.IndexOf("\"")).Split(' ').Length + Line.Substring(Line.LastIndexOf("\"")).Split(' ').Length - 1];
                String[] temp = Line.Substring(0, Line.IndexOf("\"")).Trim().Split(' ');
                String[] temp2 = Line.Substring(Line.LastIndexOf("\"") + 1).Trim().Split(' ');
                String[] temp3 = new String[temp.Length + 1];
                temp.CopyTo(temp3, 0);

                temp3[temp3.Length - 1] = Line.Substring(Line.IndexOf("\""), Line.LastIndexOf("\"") - Line.IndexOf("\"")).Replace("\"", "");

                temp3.CopyTo(commands, 0);
                temp2.CopyTo(commands, temp3.Length);

                if (commands == null || commands.Length <= 0)
                {
                    Program.tConsole.WriteLine("Issue parsing Player Command for " + Hooks.PLAYER_COMMAND.ToString() + " from " + player.Name);
                    return;
                }
                switchCommands(commands, player);

            }
            else
            {
                String[] commands = Line.Trim().ToLower().Split(' ');

                if (commands == null || commands.Length <= 0)
                {
                    Program.tConsole.WriteLine("Issue parsing Player Command for " + Hooks.PLAYER_COMMAND.ToString() + " from " + player.Name);
                    return;
                }
                switchCommands(commands, player);
            }
        }

        /// <summary>
        /// Executes command methods derived from parsing
        /// </summary>
        /// <param name="commands">Command arguments to pass to methods</param>
        /// <param name="sender">Sending player</param>
        public void switchCommands(String[] commands, ISender sender)
        {
            switch (Commands.getCommandValue(commands[0]))
            {
                case (int)Commands.Command.NO_SUCH_COMMAND:
                    {
                        Program.tConsole.WriteLine("No such command!");
                        return;
                    }
                case (int)Commands.Command.CONSOLE_EXIT:
                    {
                        if (sender is Player)
                        {
                            Player player = (Player)sender;
                            if (!player.Op)
                            {
                                NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmi, -1, "You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                                return;
                            }
                        }

                        Program.server.notifyOps("Stopping Server...");
                        Commands.Exit(Program.server);
                        break;
                    }
                case (int)Commands.Command.COMMAND_RELOAD:
                    {
                        if (sender is Player)
                        {
                            Player player = (Player)sender;
                            if (!player.Op)
                            {
                                NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmi, -1, "You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                                return;
                            }
                        }

                        Program.server.notifyOps("Reloading Plugins.");
                        Commands.Reload(Program.server);
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
                        String Message = Commands.MergeStringArray(commands);
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
                        if (sender is Player)
                        {
                            Player player = (Player)sender;
                            if (!player.Op)
                            {
                                NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmi, -1, "You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                                return;
                            }
                        }
                        Commands.SaveAll();
                        break;
                    }
                case (int)Commands.Command.COMMAND_HELP:
                    {
                        if (commands.Length > 1)
                        {
                            Commands.ShowHelp(sender, commands);
                        }
                        else
                        {
                            Commands.ShowHelp(sender);
                        }
                        break;
                    }
                case (int)Commands.Command.COMMAND_WHITELIST:
                    {
                        Commands.WhiteList(sender, commands);
                        break;
                    }
                case (int)Commands.Command.COMMAND_BAN:
                    {
                        Commands.BanList(sender, commands);
                        break;
                    }
                case (int)Commands.Command.COMMAND_UNBAN:
                    {
                        Commands.BanList(sender, commands);
                        break;
                    }
                case (int)Commands.Command.COMMAND_TIME:
                    {
                        Commands.Time(sender, commands);
                        break;
                    }
                case (int)Commands.Command.COMMAND_GIVE:
                    {
                        Commands.Give(sender, commands);
                        break;
                    }
                case (int)Commands.Command.PLAYER_SPAWNNPC:
                    {
                        Commands.SpawnNPC(sender, commands);
                        break;
                    }
                case (int)Commands.Command.COMMAND_TELEPORT:
                    {
                        Commands.Teleport(sender, commands);
                        break;
                    }
                case (int)Commands.Command.PLAYER_TPHERE:
                    {
                        Commands.TeleportHere(sender, commands);
                        break;
                    }
                case (int)Commands.Command.COMMAND_SETTLEWATER:
                    {
                        Commands.SettleWater(sender);
                        break;
                    }
                case (int)Commands.Command.COMMAND_OP:
                    {
                        Commands.OP(sender, commands);
                        break;
                    }
                case (int)Commands.Command.COMMAND_DEOP:
                    {
                        Commands.OP(sender, commands, true);
                        break;
                    }
                case (int)Commands.Command.PLAYER_OPLOGIN:
                    {
                        Commands.OPLoginOut(sender, commands);
                        break;
                    }
                case (int)Commands.Command.PLAYER_OPLOGOUT:
                    {
                        Commands.OPLoginOut(sender, commands, true);
                        break;
                    }
                case (int)Commands.Command.COMMAND_NPCSPAWN:
                    {
                        Commands.NPCSpawns(sender);
                        break;
                    }
                case (int)Commands.Command.COMMAND_KICK:
                    {
                        Commands.Kick(sender, commands);
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

    }
}
