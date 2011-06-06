using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Commands
{
    public class Commands
    {

        public enum Command
        {
            NO_SUCH_COMMAND = -1,
            CONSOLE_EXIT = 0,
            COMMAND_RELOAD = 1
        }

        public static string[] CommandDefinition = new string[] { "exit", "reload" };

        public static int getCommandValue(string Command) {
            for (int i = 0; i < CommandDefinition.Length; i++)
            {
                if(CommandDefinition[i] != null && CommandDefinition[i].Equals(Command.ToLower().Trim())) {
                    return i;
                }
            }
            return (int)Commands.Command.NO_SUCH_COMMAND;
        }

        public static void Exit(Server server)
        {
            server.StopServer();
        }

        public static void Reload(Server server)
        {
            server.getPluginManager().ReloadPlugins();
        }

    }
}
