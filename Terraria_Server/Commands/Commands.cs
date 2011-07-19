using System;
using System.Text;
using System.Collections.Generic;

using Terraria_Server;
using System.Threading;
using Terraria_Server.Collections;
using Terraria_Server.Misc;

namespace Terraria_Server.Commands
{
    public class Commands
    {
        /// <summary>
        /// Enumerates command IDs
        /// </summary>
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
            COMMAND_TIME = 11,
            COMMAND_GIVE = 12,
            PLAYER_SPAWNNPC = 13,
            COMMAND_TELEPORT = 14,
            PLAYER_TPHERE = 15,
            COMMAND_SETTLEWATER = 16,
            COMMAND_OP = 17,
            COMMAND_DEOP = 18,
            PLAYER_OPLOGIN = 19,
            PLAYER_OPLOGOUT = 20,
            COMMAND_NPCSPAWN = 21,
            COMMAND_KICK = 22,
            COMMAND_RESTART = 23,
            COMMAND_STOP = 24,
            COMMAND_SLOTS = 25,
        }
        /// <summary>
        /// Defines the string values for the command names
        /// </summary>
        public static String[] CommandDefinition = new String[] {   "exit",         "reload",       "list",
                                                                    "players",      "me",           "say",
                                                                    "save-all",     "help",         "whitelist",
                                                                    "ban",          "unban",        "time",
                                                                    "give",         "spawnnpc",     "tp",
																	"tphere",       "settle",       "op",
                                                                    "deop",         "oplogin",      "oplogout",
                                                                    "npcspawns",    "kick",         "restart",
                                                                    "stop",         "slots"};
        /// <summary>
        /// Defines help text for each command
        /// </summary>
        public static String[] CommandInformation = new String[] {  "Stop & Close The Server.",
                                                                    "Reload Plugins.",
                                                                    "Show Online Players.",
                                                                    "Show Online Players.",
                                                                    "Talk in 3rd Person.",
                                                                    "Send A Console Message To Online Players.", 
                                                                    "Trigger a World Save.", 
                                                                    "Show this Help. (Also /help <page>)", 
                                                                    "add:remove to the whitelist.", 
                                                                    "Ban a Player.", 
                                                                    "Un-Ban a Player.", 
                                                                    "Set Time with: set <time>:day:dusk:dawn:noon:night:now",
                                                                    "Give Player an item (/give <player> <amount> <item name:id>)",
                                                                    "Spawn a NPC (/spawnnpc <amount> <name:id>)",
                                                                    "Teleport Player to Player.",
                                                                    "Teleport a Player to You.",
                                                                    "Settle Water.",
                                                                    "Set a player to OP",
                                                                    "De-OP a player.",
                                                                    "Log in as OP: /oplogin <password>",
                                                                    "Log out of OP status.",
                                                                    "Toggle the state of NPC Spawning.",
                                                                    "Kicks a player from the server.", 
                                                                    "Restarts the server.",
                                                                    "Stop & Close The Server.",
                                                                    "Check the state of connection slots"};

        /// <summary>
        /// Defines permission required to use the command at the specified index.  1 = requires op, 0 = any player
        /// </summary>
        public static int[] CommandPermission = new int[] { 1, /* 0 */  1, /* 1 */  0, /* 2 */  0, /* 3 */  0, /* 4 */ 
                                                            1, /* 5 */  1, /* 6 */  0, /* 7 */  1, /* 8 */  1, /* 9 */ 
                                                            1, /* 10 */ 1, /* 11 */ 1, /* 12 */ 1, /* 13 */ 1, /* 14 */ 
                                                            1, /* 15 */ 1, /* 16 */ 1, /* 17 */ 1, /* 18 */ 0, /* 19 */ 
                                                            0, /* 20 */ 1, /* 21 */ 1, /* 22 */ 1, /* 23 */ 1, /* 24 */ 
                                                            1, /* 25 */ };

        /// <summary>
        /// Utility for converting a String array into a single string
        /// </summary>
        /// <param name="Array">Array of strings to convert</param>
        /// <returns>Single string representation of Array</returns>
        public static String MergeStringArray(String[] Array)
        {
            StringBuilder sb = new StringBuilder();
            if (Array != null && Array.Length > 0)
            {
                for (int i = 0; i < Array.Length; i++)
                {
                    if (Array[i] != null)
                    {
                        sb.Append(Array[i] + " ");
                    }
                }
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Gets the enumerated value of a command by name
        /// </summary>
        /// <param name="Command">Name of command</param>
        /// <returns>ID of command if it exists, NO_SUCH_COMMAND if not</returns>
        public static int getCommandValue(String Command)
        {
            for (int i = 0; i < CommandDefinition.Length; i++)
            {
                if (CommandDefinition[i] != null && CommandDefinition[i].Equals(Command.ToLower().Trim()))
                {
                    return i;
                }
            }
            return (int)Commands.Command.NO_SUCH_COMMAND;
        }

        /// <summary>
        /// Executes the save/stop routine for the server
        /// </summary>
        /// <param name="server">Current instance of Server to stop</param>
        /// <param name="sender">Player/Console instance</param>
        public static void Exit(Server server, ISender sender)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            Program.server.notifyOps("Stopping Server...");
            server.StopServer();
        }

        /// <summary>
        /// Executes the plugin reload method
        /// </summary>
        /// <param name="server">Current instance of Server</param>
        /// <param name="sender">Player/Console instance</param>
        public static void Reload(Server server, ISender sender)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            Program.server.notifyOps("Reloading Plugins.");
            server.PluginManager.ReloadPlugins();
        }

        /// <summary>
        /// Sends the list of currently active players to the requesting player
        /// </summary>
        /// <param name="playerIndex">Index of player requesting list</param>
        /// <param name="sendPlayer">Unknown?  Unused</param>
        /// <returns>A copy of the current players list</returns>
        public static String List(int playerIndex = 0, bool sendPlayer = true)
        {
            String playerList = "";
            foreach(Player player in Main.players)
            {
                if (player.Active)
                {
                    playerList += ", " + player.Name;
                }
            }
            if (playerList.StartsWith(","))
            {
                playerList = playerList.Remove(0, 2);
            }
            if (sendPlayer)
            {
                NetMessage.SendData(25, playerIndex, -1, "Current players: " + playerList.Trim() + ".", 255, 255f, 240f, 20f);
            }
            return "Current players: " + playerList.Trim() + ".";
        }

        /// <summary>
        /// Sends message to the chat, /me format for a player, "Server" for the console
        /// </summary>
        /// <param name="Message">Message to send</param>
        /// <param name="playerIndex">Sending player's index number</param>
        public static void Me_Say(String Message, int playerIndex = -1)
        {
            if (Message != null && Message.Trim().Length > 0)
            {
                if (playerIndex >= 0) //-1 Means Console :3
                {
                    NetMessage.SendData(25, -1, -1, "*" + Main.players[playerIndex].Name + " " + Message, 255, 200f, 100f);
                }
                else
                {
                    NetMessage.SendData(25, -1, -1, "Server: " + Message, 255, 238f, 130f, 238f);
                }
            }
        }

        /// <summary>
        /// Executes the world data save routine
        /// <param name="sender">Player/Console instance (null to just save)</param>
        /// </summary>
        public static void SaveAll(ISender sender)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            Program.server.notifyOps("Saving World...");

            WorldGen.saveWorld(Program.server.World.SavePath, false);
            while (WorldGen.saveLock)
            {
            }

            Program.server.notifyOps("Saving Data...");

            Program.server.BanList.Save();
            Program.server.WhiteList.Save();

            Program.server.notifyOps("Saving Complete.");
        }
        
        /// <summary>
        /// Sends the help list to the requesting player's chat
        /// </summary>
        /// <param name="sender">Requesting player</param>
        /// <param name="commands">Specific commands to send help on, if player provided any</param>
        public static void ShowHelp (ISender sender, IList<string> commands = null)
        {
            if (sender is Player)
            {
                if (commands == null)
                {
                    for (int i = 0; i < CommandDefinition.Length; i++)
                    {
                        bool show = false;

                        if (((Player)sender).Op || CommandPermission[i] == 0)
                        {
                            show = true;
                        }

                        if (show)
                        {
                            ((Player)sender).sendMessage(CommandDefinition[i] + " - " + CommandInformation[i]);
                        }
                    }
                }
                else
                {
                    int maxPages = (CommandDefinition.Length / 5) + 1;
                    if (maxPages > 0 && commands.Count > 1 && commands[1] != null)
                    {
                        try
                        {
                            int selectingPage = Int32.Parse(commands[1].Trim());

                            if (selectingPage < maxPages)
                            {
                                for (int i = 0; i < maxPages; i++)
                                {
                                    if ((selectingPage <= i))
                                    {
                                        selectingPage = i * ((CommandDefinition.Length / 5) + 1);
                                        break;
                                    }
                                }
                                
                                int toPage = CommandDefinition.Length;
                                if (selectingPage + 5 < toPage)
                                {
                                    toPage = selectingPage + 5;
                                }

                                for (int i = selectingPage; i < toPage; i++)
                                {
                                    bool show = false;

                                    if (((Player)sender).Op || CommandPermission[i] == 0)
                                    {
                                        show = true;
                                    }

                                    if (show)
                                    {
                                        ((Player)sender).sendMessage(CommandDefinition[i] + " - " + CommandInformation[i]);
                                    }
                                }
                            }
                            else
                            {
                                sender.sendMessage("Invalid page! Use: 0 -> " + (maxPages - 1).ToString());
                            }
                        }
                        catch (Exception)
                        {
                            ShowHelp(sender);
                        }
                    }
                    else
                    {
                        ShowHelp(sender);
                    }
                }
            }
            else
            {
                for (int i = 0; i < CommandDefinition.Length; i++)
                {
                    Program.tConsole.WriteLine("\t" + CommandDefinition[i] + " - " + CommandInformation[i].Replace("/", ""));
                }
            }
        }

        /// <summary>
        /// Adds or removes specified player to/from the white list
        /// </summary>
        /// <param name="sender">Player that sent command</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        public static void WhiteList(ISender sender, IList<string> commands)
        {
            // /whitelist <add:remove> <player>
            // arg  0         1           2
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            if (commands != null && commands.Count > 2)
            {
                if (commands[1] != null && commands[2] != null && commands[1].Length > 0 && commands[2].Length > 0)
                {
                    String caseType = "ADD";

                    switch (commands[1].Trim().ToUpper())
                    {
                        case "ADD":
                            {
                                Program.server.WhiteList.addException(commands[2]);
                                break;
                            }
                        case "REMOVE":
                            {
                                Program.server.WhiteList.removeException(commands[2]);
                                caseType = "REMOVE";
                                break;
                            }
                        default:
                            {
                                goto ERROR;
                            }

                    }

                    Program.server.notifyOps(sender.Name + " used WhiteList command " + caseType + " for: " + commands[2], true);

                    if (!Program.server.WhiteList.Save())
                    {
                        Program.server.notifyOps("WhiteList Failed to Save due to " + sender.Name + "'s command", true);
                    }
                    return;
                }
            }
        ERROR:
            sender.sendMessage("Command args Error!");
        }

        /// <summary>
        /// Adds or removes player to/from the ban list
        /// </summary>
        /// <param name="sender">Player that sent command</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        public static void BanList(ISender sender, IList<string> commands)
        {
            // /ban  <player>
            // /unban <player>

            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            if (commands != null && commands.Count > 1)
            {
                if (commands[0] != null && commands[0].Length > 0)
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
                                //We now should check to make sure they are off the server...
                                Player banee = Program.server.GetPlayerByName(commands[1]);

                                if (banee == null)
                                {
                                    foreach (Player player in Program.server.PlayerList)
                                    {
                                        var ip = Netplay.slots[player.whoAmi].remoteAddress.Split(':')[0];
                                        if (ip == commands[1])
                                        {
                                            banee = player;
                                        }
                                    }
                                }

                                Program.server.BanList.addException(commands[1]);

                                if (banee != null)
                                {
                                    banee.Kick("You have been banned from this Server.");
                                    Program.server.BanList.addException(Netplay.slots[banee.whoAmi].
                                        remoteAddress.Split(':')[0]);
                                }
                                break;
                            }
                        case 1:
                            {
                                Program.server.BanList.removeException(commands[1]);
                                break;
                            }
                        default:
                            {
                                goto ERROR;
                            }

                    }

                    Program.server.notifyOps(sender.Name + " used Ban command case " + caseType + " for: " + commands[1], true);

                    if (!Program.server.BanList.Save())
                    {
                        Program.server.notifyOps("BanList Failed to Save due to " + sender.Name + "'s command", true);
                    }
                    return;
                }
            }

        ERROR:
            sender.sendMessage("Command Error!");
        }

        /// <summary>
        /// Sets the time in the game
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        public static void Time(ISender sender, IList<string> commands)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            if (commands != null && commands.Count > 1)
            {
                if (commands[1] != null && commands[1].Length > 0)
                {
                    String caseType = commands[1].Trim().ToLower();

                    switch (caseType)
                    {
                        case "set":
                            {
                                if (commands.Count > 2 && commands[2] != null && commands[2].Length > 0)
                                {
                                    try
                                    {
                                    Program.server.World.setTime(Double.Parse(commands[2]), true);
                                    } catch(Exception) {
                                        goto ERROR;
                                    }
                                }
                                else
                                {
                                    goto ERROR;
                                }
                                break;
                            }
                        case "day":
                            {
                                Program.server.World.setTime(13500.0);
                                break;
                            }
                        case "dawn":
                            {
                                Program.server.World.setTime(0);
                                break;
                            }
                        case "dusk":
                            {
                                Program.server.World.setTime(0, false, false);
                                break;
                            }
                        case "noon":
                            {
                                Program.server.World.setTime(27000.0);
                                break;
                            }
                        case "night":
                            {
                                Program.server.World.setTime(16200.0, false, false);
                                break;
                            }
                        case "now":
                            {
                                String AP = "AM";
                                double time = Main.time;
                                if (!Main.dayTime)
                                {
                                    time += 54000.0;
                                }
                                time = time / 86400.0 * 24.0;
                                double num2 = 7.5; //stuffs me at this stage
                                time = time - num2 - 12.0;
                                if (time < 0.0)
                                {
                                    time += 24.0;
                                }
                                if (time >= 12.0)
                                {
                                    AP = "PM";
                                }
                                int Hours = (int)time;
                                double Minutes = time - (double)Hours;
                                String MinuteString = (Minutes * 60.0).ToString();
                                if (Minutes < 10.0)
                                {
                                    MinuteString = "0" + MinuteString;
                                }
                                if (Hours > 12)
                                {
                                    Hours -= 12;
                                }
                                if (Hours == 0)
                                {
                                    Hours = 12;
                                }
                                if (MinuteString.Length > 2)
                                {
                                    MinuteString = MinuteString.Substring(0, 2);
                                }
                                sender.sendMessage("Current Time: " + Hours + ":" + MinuteString + " " + AP);
                                return;
                            }
                        default:
                            {
                                goto ERROR;
                            }
                    }
                    NetMessage.SendData((int)Packet.WORLD_DATA); //Update Data
                    Program.server.notifyAll("Time set to " + Server.time.ToString() + " by " + sender.Name);
                    return;
                }
            }

        ERROR:
            sender.sendMessage("Command Error!");
        }

        /// <summary>
        /// Gives specified item to the specified player
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        public static void Give(ISender sender, IList<string> commands)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }
            // /give <player> <stack> <name> 
            if (commands.Count > 3 && commands[1] != null && commands[2] != null && commands[3] != null &&
                commands[1].Trim().Length > 0 && commands[2].Trim().Length > 0 && commands[3].Trim().Length > 0)
            {
                String playerName = commands[1].Trim();
                String itemName = string.Join (" ", commands);
                itemName = itemName.Remove(0, itemName.IndexOf(" " + commands[3]));

                Player player = Program.server.GetPlayerByName(playerName);
                if (player != null)
                {
                    Item[] items = new Item[Main.maxItemTypes];
                    for (int i = 0; i < Main.maxItemTypes; i++)
                    {
                        items[i] = Registries.Item.Create(i);
                    }

                    Item item = null;
                    itemName = itemName.Replace(" ", "").ToLower();
                    for (int i = 0; i < Main.maxItemTypes; i++)
                    {
                        if (items[i].Name != null)
                        {
                            String genItemName = items[i].Name.Replace(" ", "").Trim().ToLower();
                            if (genItemName == itemName)
                            {
                                item = items[i];
                            }
                        }
                    }

                    int itemType = -1;
                    bool assumed = false;
                    if (item != null)
                    {
                        itemType = item.Type;
                    }
                    else
                    {
                        int assumedItem;
                        try
                        {
                            assumedItem = Int32.Parse(itemName);
                        }
                        catch (Exception)
                        {
                            sender.sendMessage("Item '" + itemName + "' not found!");
                            return;
                        }

                        for (int i = 0; i < Main.maxItemTypes; i++)
                        {
                            if (items[i].Type == assumedItem)
                            {
                                itemType = items[i].Type;
                                assumed = true;
                                break;
                            }
                        }

                        if (!assumed)
                        {
                            sender.sendMessage("Item '" + itemName + "' not found!");
                            return;
                        }
                    }

                    //Clear Data
                    for (int i = 0; i < Main.maxItemTypes; i++)
                    {
                        items[i] = null;
                    }
                    items = null;

                    if (itemType != -1)
                    {

                        int stackSize;
                        try
                        {
                            stackSize = Int32.Parse(commands[2]);
                        }
                        catch (Exception)
                        {
                            stackSize = 1;
                        }

                        Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, itemType, stackSize, false);

                        Program.server.notifyOps("Giving " + player.Name + " some " + itemType.ToString() + " {" + sender.Name + "}", true);

                        return;
                    }
                }
                else
                {
                    sender.sendMessage("Player '" + playerName + "' not found!");
                    return;
                }
            }
            else
            {
                goto ERROR;
            }

        ERROR:
            sender.sendMessage("Command Error!");
        }

        /// <summary>
        /// Spawns specified NPC type
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        public static void SpawnNPC(ISender sender, IList<string> commands)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }

                // /spawnnpc <amount> <name:id>
                if (commands.Count > 2 && commands[1] != null && commands[2] != null
                    && commands[1].Trim().Length > 0 && commands[2].Trim().Length > 0)
                {
                    String npcName = string.Join (" ", commands);
                    npcName = npcName.Remove(0, npcName.IndexOf(" " + commands[2])).Replace(" ", "").ToLower();

                    NPC[] npcs = new NPC[Main.maxItemTypes];
                    for (int i = 0; i < Main.maxItemTypes; i++)
                    {
                        npcs[i] = Registries.NPC.Create(i);
                    }

                    int npcType = -1;
                    String NpcName = "";
                    for (int i = 0; i < Main.maxNPCTypes; i++)
                    {
                        if (npcs[i] != null)
                        {
                            if (npcs[i].Name != null && npcs[i].Name.Trim().Length > 0)
                            {
                                String npc = npcs[i].Name.Trim().Replace(" ", "").ToLower();
                                if (npc == npcName)
                                {
                                    npcType = npcs[i].Type;
                                    NpcName = npcs[i].Name;
                                }
                            }
                        }
                    }

                    if (npcType == -1)
                    {
                        int assumedItem;
                        try
                        {
                            assumedItem = Int32.Parse(npcName);
                        }
                        catch (Exception)
                        {
                            sender.sendMessage("NPC Type '" + npcName + "' not found!");
                            return;
                        }

                        bool assumed = false;
                        for (int i = 0; i < Main.maxNPCTypes; i++)
                        {
                            if (npcs[i].Type == assumedItem)
                            {
                                npcType = npcs[i].Type;
                                NpcName = npcs[i].Name;
                                assumed = true;
                                break;
                            }
                        }

                        if (!assumed)
                        {
                            sender.sendMessage("Invalid NPC Type '" + npcName + "'!");
                            return;
                        }
                    }

                    int amount = 1;
                    try
                    {
                        amount = Int32.Parse(commands[1]);
                    }
                    catch (Exception)
                    {
                        sender.sendMessage("Invalid NPC Type '" + npcName + "'!");
                        return;
                    }

                    for (int i = 0; i < Main.maxNPCTypes; i++)
                    {
                        npcs[i] = null;
                    }
                    npcs = null;

                    if (amount >= 0)
                    {
                        for (int i = 0; i < amount; i++)
                        {
                            Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 100, 50);
                            int index = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), npcType);
                            //Main.npcs[index] = Registries.NPC.Create(NpcName);
                            //Extend Registries.NPC to include:
                            //Main.npcs[index].Name = "Green Slime";
                            //Main.npcs[index].Active = true;
                            //Main.npcs[index].Type = 1;
                            //NPC.SpawnNPC();
                            Main.npcs[index] = Registries.NPC.Create(NpcName);
                        }

                        Program.server.notifyOps("Spawned " + amount.ToString() + " of " +
                            npcType.ToString() + " {" + player.Name + "}", true);
                        return;
                    }

                }
                else
                {
                    goto ERROR;
                }

            ERROR:
                sender.sendMessage("Command Error!");
            }
        }

        /// <summary>
        /// Teleports sender to specified player's location
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        public static void Teleport(ISender sender, IList<string> commands)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            // /tp <player> <toplayer>
            if (commands.Count > 2 && commands[1] != null && commands[2] != null && commands[1].Trim().Length > 0 && commands[2].Trim().Length > 0)
            {
                Player player = Program.server.GetPlayerByName(commands[1].Trim());
                Player toplayer = Program.server.GetPlayerByName(commands[2].Trim());

                if (player == null || toplayer == null)
                {
                    sender.sendMessage("Could not find a Player on the Server");
                    return;
                }

                player.teleportTo(toplayer);

                Program.server.notifyOps("Teleported " + player.Name + " to " +
                    toplayer.Name + " {" + sender.Name + "}", true);

                return;
            }
            else
            {
                goto ERROR;
            }

        ERROR:
            sender.sendMessage("Command Error!");
        }

        /// <summary>
        /// Teleports specified player to sending player's location
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        public static void TeleportHere(ISender sender, IList<string> commands)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }

                // /tp <player> <toplayer>
                if (commands.Count > 1 && commands[1] != null && commands[1].Trim().Length > 0)
                {
                    Player toplayer = Program.server.GetPlayerByName(commands[1].Trim());

                    if (toplayer == null)
                    {
                        sender.sendMessage("Could not find a Player on the Server");
                        return;
                    }

                    toplayer.teleportTo(player);

                    Program.server.notifyOps("Teleported " + toplayer.Name + " to " +
                        player.Name + " {" + sender.Name + "}", true);

                    return;
                }
            }
            else
            {
                goto ERROR;
            }

        ERROR:
            sender.sendMessage("Command Error!");
        }

        /// <summary>
        /// Settles water like in the startup routine
        /// </summary>
        /// <param name="sender">Sending player</param>
        public static void SettleWater(ISender sender)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            if (!Liquid.panicMode)
            {
                sender.sendMessage("Settling Liquids...");
                Liquid.StartPanic();
                sender.sendMessage("Complete.");
            }
            else
            {
                sender.sendMessage("Liquids are already settling");
            }
        }

        /// <summary>
        /// Ops or deops the specified player
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        /// <param name="deop">Boolean value representing command's op type: True = deop command, false = op</param>
        public static void OP(ISender sender, IList<string> commands, bool deop = false)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            if (((deop && commands.Count > 1) || (!deop && commands.Count > 2)) 
                && commands[1] != null 
                && (deop || commands[2] != null) 
                && commands[1].Trim().Length > 0 
                && (deop || commands[2].Trim().Length > 0))
            {
                String player_OP = commands[1].Trim().ToLower();

                if (deop)
                {
                    Program.server.notifyOps("De-Opping " + player_OP + " {" + sender.Name + "}", true);

                    if (Player.isInOpList(player_OP, Program.server))
                    {
                        Program.server.OpList.removeException(player_OP + ":" + Player.getPassword(player_OP, Program.server));
                    }
                }
                else
                {
                    String player_Password = commands[2].Trim().ToLower();
                    Program.server.notifyOps("Opping " + player_OP + " {" + sender.Name + "}", true);
                    Program.server.OpList.addException(player_OP + ":" + player_Password, true, player_OP.Length);
                }

                if (!Program.server.OpList.Save())
                {
                    Program.server.notifyOps("OpList Failed to Save due to " + sender.Name + "'s command", true);
                } 
                return;
            }
            else
            {
                goto ERROR;
            }

        ERROR:
            sender.sendMessage("Command Error!");
        }

        /// <summary>
        /// Handles all op login/logout commands
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        /// <param name="logout">Boolean: True means command was oplogout, false means oplogin</param>
        public static void OPLoginOut(ISender sender, IList<string> commands, bool logout = false)
        {
            if (sender is Player)
            {
                if (logout)
                {
                    if (sender.Op)
                    {
                        sender.Op = false;
                        sender.sendMessage("Successfully Logged Out.");
                    }
                    return;
                }

                if (commands.Count > 1 && commands[1] != null && commands[1].Trim().Length > 0)
                {
                    String player_Password = commands[1].Trim().ToLower();

                    if (Player.isInOpList(sender.Name, Program.server))
                    {
                        if (((Player)sender).getPassword().Trim().ToLower() == player_Password)
                        {
                            sender.Op = true;
                            sender.sendMessage("Successfully Logged in as OP.");
                        }
                        else
                        {
                            sender.sendMessage("Incorrect OP Password.");
                            return;
                        }
                    }
                    else
                    {
                        sender.sendMessage("You need to be Assiged OP Privledges.");
                        return;
                    }
                    return;
                }
                else
                {
                    goto ERROR;
                }
            ERROR:
                sender.sendMessage("Command Error!");
            }
        }

        /// <summary>
        /// Enables or disables NPC spawning
        /// </summary>
        /// <param name="sender">Sending player</param>
        public static void NPCSpawns(ISender sender)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            Main.stopSpawns = !Main.stopSpawns;

            if (Main.stopSpawns)
            {
                sender.sendMessage("NPC Spawning is now off!");
            }
            else
            {
                sender.sendMessage("NPC Spawning is now on!");
            }
        }

        /// <summary>
        /// Kicks the specified player out of the server
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="commands">Array of command arguments passed from CommandParser</param>
        public static void Kick(ISender sender, IList<string> commands)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            if (commands != null && commands.Count > 1)
            {
                if (commands[0] != null && commands[0].Length > 0)
                {
                    Player banee = Program.server.GetPlayerByName(commands[1]);

                    if (banee != null)
                    {
                        Program.server.notifyOps (sender.Name + " has kicked " + (banee.Name ?? commands[1]), true);
                        banee.Kick("You have been kicked from this server.");
                    }
                    else
                    {
                        sender.sendMessage("Cannot find player online!.");
                    }
                    
                    return;
                }
            }
            sender.sendMessage("Command Error!");
        }
        
        /// <summary>
        /// Restarts the server
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="server">Current Server instance</param>
        public static void Restart(ISender sender, Server server)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                if (!player.Op)
                {
                    player.sendMessage("You Cannot Perform That Action.", 255, 238f, 130f, 238f);
                    return;
                }
            }

            Statics.keepRunning = true;
            server.StopServer();
            while (Statics.serverStarted) { Thread.Sleep (10); }
            Program.tConsole.WriteLine("Starting the Server");
            server.Initialize();
            WorldGen.loadWorld();
            Program.updateThread = new Thread(Program.UpdateLoop);
            Program.updateThread.Name = "UpdateLoop";
            server.StartServer();
            Statics.keepRunning = false;
        }
		
		public static void Slots (ISender sender, Server server)
		{
			if (sender is Player)
				return;
			
			for (int i = 0; i < 255; i++)
			{
				var slot = Netplay.slots[i];
				var player = Main.players[i];
				
				if (slot.state != SlotState.VACANT)
				{
					var name = "";
					if (player != null)
					{
						name = ", " + (player.Name ?? "<null>");
						if (player.AuthenticatedAs != null)
						{
							if (player.Name == player.AuthenticatedAs)
								name = name + " (auth'd)";
							else
								name = name + " (auth'd as " + player.AuthenticatedAs + ")";
						}
					}
					sender.sendMessage (string.Format ("slot {0}: {1}, {2}{3}, {4} {5}", i, slot.state, slot.remoteAddress, name, NetMessage.buffer[i].totalData, NetMessage.buffer[i].sideBufferBytes));
				}
			}
		}
    }
}
