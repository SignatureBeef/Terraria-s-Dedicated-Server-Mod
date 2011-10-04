using System;
using System.Collections.Generic;

using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Collections;

namespace Terraria_Server
{
    ///<Summary>
    /// Provides access to the majority of Server Data
    ///</Summary>
    public static class Server
    {
        public static List<String> RejectedItems { get; set; }

        public static World World { get; set; }
        
        // Summary:
        //      Gets the White list 
        public static DataRegister WhiteList { get; set; }

        // Summary:
        //       Gets the Ban list
        public static DataRegister BanList { get; set; }

        // Summary:
        //       Gets the OP list
        public static DataRegister OpList { get; set; }

        public static bool AllowExplosions { get; set; }

        public static bool AllowTDCMRPG { get; set; }
        
        public static void InitializeData(World NewWorld, int PlayerCap, string myWhiteList, string myBanList, string myOpList)
        {
            //Main.maxNetplayers = PlayerCap;
            World = NewWorld;
            //PluginManager = new PluginManager(Statics.PluginPath, Statics.LibrariesPath);
            
            WhiteList = new DataRegister(myWhiteList);
            WhiteList.Load();
            BanList = new DataRegister(myBanList);
            BanList.Load();
            OpList = new DataRegister(myOpList);
            OpList.Load();

            RejectedItems = new List<String>();
            string[] rejItem = Program.properties.RejectedItems.Split(',');
            for (int i = 0; i < rejItem.Length; i++)
            {
                if (rejItem[i].Trim().Length > 0)
                {
                    RejectedItems.Add(rejItem[i].Trim());
                }
            }

            AllowExplosions = Program.properties.AllowExplosions;
            AllowTDCMRPG = Program.properties.AllowTDCMRPG;
        }

        // Summary:
        //       Gets a specified Online Player
        //       Input name must already be cleaned of spaces
        public static Player GetPlayerByName(string name)
        {
            string lowercaseName = name.ToLower();
            foreach (Player player in Main.players)
            {
                if (player.Active && player.Name.ToLower().Equals(lowercaseName))
                {
                    return player;
                }
            }
            return null;
        }
        
        // Summary:
        //       Send a message to all online OPs
        public static void notifyOps(string Message, bool writeToConsole = true)
        {
            if (Statics.cmdMessages)
            {
                foreach (Player player in Main.players)
                {
                    if (player.Active && player.Op)
                    {
                        NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmi, -1, Message, 255, 176f, 196, 222f);
                    }
                }
            }
            if (writeToConsole)
            {
                ProgramLog.Admin.Log(Message);
            }
        }

        public static void notifyOps(string format, bool writeToConsole = true, params object[] args)
        {
            notifyOps(String.Format(format, args), writeToConsole);
        }

        // Summary:
        //       Sends a Message to all Connected Clients
        public static void notifyAll(string Message, bool writeToConsole = true)
        {
            NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, Message, 255, 238f, 130f, 238f);
            if (writeToConsole)
            {
                ProgramLog.Admin.Log(Message);
            }
        }

        // Summary:
        //       Sends a Message to all Connected Clients
        public static void notifyAll(string Message, Color ChatColour, bool writeToConsole = true)
        {
            NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, Message, 255, ChatColour.R, ChatColour.G, ChatColour.B);
            if (writeToConsole)
            {
                ProgramLog.Admin.Log(Message);
            }
        }

        // Summary:
        //       Get the array of Active NPCs
        public static NPC[] ActiveNPCs()
        {
            NPC[] npcs = null;

            int npcCount = 0;
            for (int i = 0; i < NPC.MAX_NPCS; i++)
            {
                if (Main.npcs[i].Active)
                {
                    npcCount++;
                }
            }

            if (npcCount > 0)
            {
                npcs = new NPC[npcCount];
                npcCount = 0;
                for (int i = 0; i < Main.npcs.Length-1; i++)
                {
                    if (Main.npcs[i].Active)
                    {
                        npcs[npcCount] = Main.npcs[i];
                        npcCount++;
                    }
                }
            }
            
            return npcs;
        }

        // Summary:
        //       Gets the total of all active NPCs
        public static int ActiveNPCCount()
        {
            int npcCount = 0;
            for (int i = 0; i < Main.npcs.Length - 1; i++)
            {
                if (Main.npcs[i].Active)
                {
                    npcCount++;
                }
            }
            return npcCount;
        }

        // Summary:
        //       Gets/Sets the maximum allowed NPCs
        public static int MaxNPCs
        {
            get
            {
                return NPC.maxSpawns;
            }
            set
            {
                NPC.defaultMaxSpawns = value;
                NPC.maxSpawns = value;
            }
        }

        // Summary:
        //       Gets/Sets the max spawn rate of NPCs\
        public static int SpawnRate
        {
            get
            {
                return NPC.spawnRate;
            }
            set
            {
                NPC.defaultSpawnRate = value;
                NPC.spawnRate = value;
            }
        }

        public static bool isValidLocation(Vector2 point, bool defaultResist = true)
        {
            if (point != null && (defaultResist) ? (point != default(Vector2)) : true)
                if (point.X <= Main.maxTilesX && point.X >= 0)
                {
                    if (point.Y <= Main.maxTilesY && point.Y >= 0)
                    {
                        return true;
                    }
                }

            return false;
        }

        public static bool RejectedItemsContains(string item)
        {
            if (item != null)
            {
                foreach (string rItem in RejectedItems)
                {
                    if (rItem.Trim().Replace(" ", "") == item.Trim().Replace(" ", ""))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<Player> FindPlayerByPart(string partName, bool ignoreCase = true)
        {
            List<Player> matches = new List<Player>();

            foreach (var player in Main.players)
            {
                if (player.Name == null)
                    continue;

                string playerName = player.Name;

                if (ignoreCase)
                    playerName = playerName.ToLower();

                if (playerName.StartsWith((ignoreCase) ? partName.ToLower() : partName))
                {
                    matches.Add(player);
                }
            }

            return matches;
        }

        public static bool TryFindItem<T>(T ItemIdOrName, out List<Int32> ItemList)
        {
            ItemList = new List<Int32>();

            Item[] items = new Item[Main.maxItemTypes];
            for (int i = 0; i < Main.maxItemTypes; i++)
            {
                items[i] = Registries.Item.Create(i);
            }

            string cleanedName = ItemIdOrName.ToString();
            int itemID = -1;
            if (ItemIdOrName is String)
            {
                cleanedName = cleanedName.Replace(" ", "").ToLower();
            }
            else if (ItemIdOrName is Int32)
            {
                itemID = Int32.Parse(ItemIdOrName.ToString());
            }

            for (int i = 0; i < Main.maxItemTypes; i++)
            {
                if (ItemIdOrName is String)
                {
                    if (items[i].Name != null)
                    {
                        string fndItemName = items[i].Name.Replace(" ", "").Trim().ToLower();
                        if (fndItemName == cleanedName)
                        {
                            ItemList.Add(items[i].Type);
                        }
                    }
                }
                else if (ItemIdOrName is Int32)
                {
                    if (items[i].Type == itemID)
                    {
                        ItemList.Add(items[i].Type);
                    }
                }
                
            }

            //Clear Data
            for (int i = 0; i < Main.maxItemTypes; i++)
            {
                items[i] = null;
            }
            items = null;

            return ItemList.Count > 0;
        }

        public static bool TryFindItemByType(int ItemID, out List<Int32> ItemList)
        {
            return TryFindItem(ItemID, out ItemList);
        }

        public static bool TryFindItemByName(string ItemName, out List<Int32> ItemList)
        {
            return TryFindItem(ItemName, out ItemList);
        }

    }
}
