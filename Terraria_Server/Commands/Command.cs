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
             * Player Messages: 255-0-0
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
            COMMAND_HELP = 7,
            COMMAND_WHITELIST = 8,
            COMMAND_BAN = 9,
            COMMAND_UNBAN = 10,
        }

        public static string[] CommandDefinition = new string[] {   "exit",         "reload",       "list",         
                                                                    "players",      "me",           "say",          
                                                                    "save-all",     "help",         "whitelist",
                                                                    "ban",          "unban" };
        public static string[] CommandInformation = new string[] {  "Stop & Close The Server", 
                                                                    "Reload Plugins", 
                                                                    "Show Online Players", 
                                                                    "Show Online Players", 
                                                                    "Talk in 3rd Person", 
                                                                    "Send A Console Message To Online Players", 
                                                                    "Trigger a World Save", 
                                                                    "Show this Help", 
                                                                    "add:remove to the whitelist", 
                                                                    "Ban a Player", 
                                                                    "Un-Ban a Player" };

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

        public static bool getGodMode()
        {
            return Program.server.getGodMode();
        }

        public static void setGodMode(bool GodMode)
        {
            Program.server.setGodMode(GodMode);
        }

        public static void ShowHelp(Sender sender)
        {
            if (sender is Player)
            {
                for (int i = 0; i < CommandDefinition.Length; i++)
                {
                    ((Player)sender).sendMessage(CommandDefinition[i] + " - " + CommandInformation[i]);
                }
            }
            else
            {
                for (int i = 0; i < CommandDefinition.Length; i++)
                {
                    Console.WriteLine("\t" + CommandDefinition[i] + " - " + CommandInformation[i]);
                }
            }
        }

        public static void WhiteList(Sender sender, string[] commands)
        {
            // /whitelist <add:remove> <player>
            // arg  0         1           2
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.isOp())
                {
                    NetMessage.SendData(25, player.whoAmi, -1, "You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            if (commands != null && commands.Length > 2)
            {
                if (commands[1] != null && commands[2] != null && commands[1].Length > 0 && commands[2].Length > 0)
                {
                    string caseType = "ADD";

                    switch (commands[1].Trim().ToUpper())
                    {
                        case "ADD":
                            {
                                Program.server.getWhiteList().addException(commands[2]);
                                break;
                            }
                        case "REMOVE":
                            {
                                Program.server.getWhiteList().removeException(commands[2]);
                                caseType = "REMOVE";
                                break;
                            }
                        default:
                            {
                                goto ERROR;
                            }

                    }

                    string Message = sender.getName() + " used WhiteList command " + caseType + " for: " + commands[2];
                    Program.server.notifyOps(Message);
                    sender.sendMessage(Message);

                    if (!Program.server.getWhiteList().Save())
                    {
                        Program.server.notifyOps("WhiteList Failed to Save due to " + sender.getName() + "'s command");
                    }
                    return;
                }
            }
        ERROR:
            sender.sendMessage("Command args Error!");
        }

        public static void BanList(Sender sender, string[] commands)
        {
            // /ban  <player>
            // /unban <player>

            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.isOp())
                {
                    NetMessage.SendData(25, player.whoAmi, -1, "You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            if (commands != null && commands.Length > 1)
            {
                if (commands[0] != null && commands[1].Length > 0)
                {
                    int caseType = -1;

                    if (commands[0].Trim().ToLower().Equals("ban"))
                    {
                        caseType = 0;
                    }
                    else if (commands[0].Trim().ToLower().Equals("unban"))
                    {
                        caseType = 1;
                    }

                    switch (caseType)
                    {
                        case 0:
                            {
                                Program.server.getBanList().addException(commands[1]);

                                //We now should check to make sure they are off the server...
                                Player banee = Program.server.GetPlayerByName(commands[1]);

                                if (banee == null)
                                {
                                    foreach (Player player in Program.server.getPlayerList())
                                    {
                                        if (NetPlay.serverSock[player.whoAmi].tcpClient.Client.RemoteEndPoint.ToString()
                                            .Split(':')[0].Equals(commands[1]))
                                        {
                                            banee = player;
                                        }
                                    }
                                }

                                if (banee != null)
                                {
                                    banee.Kick("You have been banned from this Server.");
                                }
                                break;
                            }
                        case 1:
                            {
                                Program.server.getBanList().removeException(commands[1]);
                                break;
                            }
                        default:
                            {
                                goto ERROR;
                            }

                    }

                    string Message = sender.getName() + " used Ban command case " + caseType + " for: " + commands[1];
                    Program.server.notifyOps(Message);
                    sender.sendMessage(Message);

                    if (!Program.server.getWhiteList().Save())
                    {
                        Program.server.notifyOps("BanList Failed to Save due to " + sender.getName() + "'s command");
                    }
                    return;
                }
            }
        ERROR:
            sender.sendMessage("Command Error!");
        }

    }
}
