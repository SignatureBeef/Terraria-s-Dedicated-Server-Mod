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
            /*
             * @Developers
             * Commands are sectioned into two parts.
             * Player & Console, Since this Class is 
             * for Player AND Console input there may be two types of commands.
             * 
             * e.g. Me (player) Say (Console)
             * 
             * They are devided as follows
             *      - COMMANDS_ (Both, Player & Console)
             *      - PLAYER_ (Player Only)
             *      - CONSOLE_ (Console Only)
             * 
             * Console Output to In-Game OPs are sent with the Colour: 176-196-222
             * Server Messages: 238-130-238
             * 
             */

            NO_SUCH_COMMAND = -1,
            CONSOLE_EXIT = 0,
            COMMAND_RELOAD = 1,
            COMMAND_LIST = 2,
            COMMAND_PLAYERS = 3,
            PLAYER_ME = 4,
            CONSOLE_SAY = 5,
            COMMAND_SAVE_ALL = 6,
        }

        public static string[] CommandDefinition = new string[] { "exit", "reload", "list", "players", "me", "say", "save-all" };

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

        public static string List(int playerIndex = 0, bool sendPlayer = true)
        {
            string playerList = "";
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    playerList += ", " + Main.player[i].name;
                }
            }
            if (playerList.StartsWith(","))
            {
                playerList = playerList.Remove(0, 2);
            }
            if (sendPlayer)
            {
                NetMessage.SendData(25, playerIndex, -1, "Current players: " + playerList.Trim() + ".", 255, 238f, 130f, 238f);
            }
            return "Current players: " + playerList.Trim() + ".";
        }

        public static void Me_Say(string Message, int playerIndex = -1)
        {
            if (Message != null && Message.Trim().Length > 0)
            {
                if (playerIndex >= 0) //-1 Means Console :3
                {
                    NetMessage.SendData(25, -1, -1, "*" + Main.player[playerIndex].name + " " + Message, 255, 200f, 100f, 0f);
                }
                else
                {
                    NetMessage.SendData(25, -1, -1, "Server: " + Message, 255, 238f, 130f, 238f);
                }
            }
        }

        public static void SaveAll()
        {
            Console.WriteLine("Saving World");
            Program.server.notifyOps("Saving World...");

            WorldGen.saveWorld(Program.server.getWorld().getSavePath(), false);
            while (WorldGen.saveLock)
            {
            }

            Program.server.notifyOps("Saving Complete.");

            Console.WriteLine("Saving Complete.");
        }

    }
}
