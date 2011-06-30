using Terraria_Server.Plugin;

namespace Terraria_Server.Commands
{
    public class CommandParser
    {
        public Server server = null;

        public CommandParser(Server Server)
        {
            server = Server;
        }

        public void parseConsoleCommand(string Line, Server server)
        {
            ConsoleSender cSender = new ConsoleSender(server);
            cSender.ConsoleCommand.Message = Line;
            cSender.ConsoleCommand.Sender = new Sender();
            server.getPluginManager().processHook(Hooks.CONSOLE_COMMAND, cSender.ConsoleCommand);
            if (cSender.ConsoleCommand.Cancelled)
            {
                return;
            }

            string[] commands = Line.Trim().ToLower().Split(' ');
            if (commands == null || commands.Length <= 0)
            {
                Program.tConsole.WriteLine("Issue parsing Console Command for " + Hooks.CONSOLE_COMMAND.ToString());
                return;
            }
            switchCommands(commands, cSender.ConsoleCommand.Sender);
        }

        public void parsePlayerCommand(Player player, string Line)
        {
            if (Line.StartsWith("/"))
            {
                Line = Line.Remove(0, 1);
            }
            string[] commands = Line.Trim().ToLower().Split(' ');
            if (commands == null || commands.Length <= 0)
            {
                Program.tConsole.WriteLine("Issue parsing Player Command for " + Hooks.PLAYER_COMMAND.ToString() + " from " + player.name);
                return;
            }
            switchCommands(commands, player);
        }

        public void switchCommands(string[] commands, Sender sender)
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
                        Commands.Exit(sender.getServer());
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
                        Commands.Reload(sender.getServer());
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
                        string Message = Commands.MergeStringArray(commands);
                        if (Message.Length <= 3) { return; }

                        if (sender is Player)
                        {
                            Commands.Me_Say(Message.Remove(0, 3).Trim(), ((Player)sender).whoAmi);
                        }
                        else
                        {
                            Commands.Me_Say(Message.Remove(0, 4).Trim(), -1); //turn command list into a string and remove "say "
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
                default:
                    {
                        Program.tConsole.WriteLine("Uknown Command Issued.");
                        break;
                    }
            }
        }

    }
}
