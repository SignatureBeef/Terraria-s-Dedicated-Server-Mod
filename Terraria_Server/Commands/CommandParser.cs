using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Commands;
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

        /*public bool runCommand(Terraria_Server.Commands.Commands.Command cmd, Sender sender)
        {
            switch (cmd)
            {
                case Terraria_Server.Commands.Commands.Command.CONSOLE_EXIT:
                    {
                        Commands.Exit(sender.getServer());
                        return true;
                    }
                default:
                    {
                        break;
                    }
            }
            return false;
        }*/

        public void parseConsoleCommand(string Line, Server server)
        {
            ConsoleSender cSender = new ConsoleSender(server);
            cSender.getConsoleCommand().setMessage(Line);
            cSender.getConsoleCommand().setSender(new Sender());
            //cSender.getConsoleCommand().getSender().setServer(server);
            server.getPluginManager().processHook(Hooks.CONSOLE_COMMAND, cSender.getConsoleCommand());
            if (cSender.getConsoleCommand().getCancelled())
            {
                return;
            }

            string[] commands = Line.Trim().ToLower().Split(' ');
            if (commands == null || commands.Length <= 0)
            {
                Console.WriteLine("Issue parsing Console Command for " + Hooks.CONSOLE_COMMAND.ToString());
                return;
            }
            switchCommands(commands, cSender.getConsoleCommand().getSender());
        }

        public void parsePlayerCommand(Player player, string Line)
        {
            PlayerCommandEvent cCommand = new PlayerCommandEvent();
            cCommand.setMessage(Line);
            cCommand.setSender(player);
            cCommand.setPlayer(player);
            server.getPluginManager().processHook(Hooks.PLAYER_COMMAND, cCommand);
            if (cCommand.getCancelled())
            {
                return;
            }

            string[] commands = Line.Trim().ToLower().Split(' ');
            if (commands == null || commands.Length <= 0)
            {
                Console.WriteLine("Issue parsing Player Command for " + Hooks.PLAYER_COMMAND.ToString() + " from " + player.name);
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
                        Console.WriteLine("No such command!");
                        break;
                    }
                case (int)Commands.Command.CONSOLE_EXIT:
                    {
                        Commands.Exit(sender.getServer());
                        break;
                    }
                case (int)Commands.Command.COMMAND_RELOAD:
                    {
                        if (sender is Player)
                        {
                            //((Player)sender).sendMessage("Reloading Plugins.");
                        }
                        else
                        {
                            Console.WriteLine("Reloading Plugins");
                        }
                        Commands.Reload(sender.getServer());
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Uknown Command Issue.");
                        break;
                    }

            }
        }

    }
}
