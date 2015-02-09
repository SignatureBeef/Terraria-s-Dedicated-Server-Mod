using System;
using System.Linq;
using Microsoft.Xna.Framework;
#if Full_API
using Terraria;
using System.Collections.Generic;
#endif

namespace tdsm.api
{
    public static class Tools
    {
        private static Action<String, Object[]> _WriteLineMethod = Console.WriteLine;

        public static void WriteLine(string fmt, params object[] args)
        {
            lock (_WriteLineMethod)
                _WriteLineMethod(fmt, args);
        }

        public static void WriteLine(Exception e)
        {
            lock (_WriteLineMethod)
                _WriteLineMethod(String.Format("{0}", e), null);
        }

        public static void SetWriteLineMethod(Action<String, Object[]> method)
        {
            lock (_WriteLineMethod)
                _WriteLineMethod = method;
        }

        public static void NotifyAllPlayers(string message, Color color, bool writeToConsole = true) //, SendingLogger Logger = SendingLogger.CONSOLE)
        {
#if Full_API
            foreach (var player in Main.player)
            {
                if (player.active)
                    NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmi, -1, message, color.A, color.R, color.G, color.B);
            }
#endif

            if (writeToConsole) Tools.WriteLine(message);
        }

        public static void NotifyAllOps(string message, bool writeToConsole = true) //, SendingLogger Logger = SendingLogger.CONSOLE)
        {
#if Full_API
            foreach (var player in Main.player)
            {
                if (player.active && player.Op)
                    NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmi, -1, message, 255, 176f, 196, 222f);
            }
#endif

            if (writeToConsole) Tools.WriteLine(message);
        }

        /// <summary>
        /// Gets a specified Online Player
        /// Input name must already be cleaned of spaces
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Player GetPlayerByName(string name)
        {
            string lowercaseName = name.ToLower();
            foreach (Player player in Main.player)
            {
                if (player.active && player.Name.ToLower().Equals(lowercaseName))
                    return player;
            }
            return null;
        }

        /// <summary>
        /// Gets the total of all active NPCs
        /// </summary>
        /// <returns></returns>
        public static int ActiveNPCCount()
        {
            /*int npcCount = 0;
            for (int i = 0; i < Main.npcs.Length - 1; i++)
            {
                if (Main.npcs[i].Active)
					npcCount++;
            }
            return npcCount;*/
            return (from x in Main.npc where x.active select x).Count();
        }

        /// <summary>
        /// Finds a valid location for such things as NPC Spawning
        /// </summary>
        /// <param name="point"></param>
        /// <param name="defaultResist"></param>
        /// <returns></returns>
        public static bool IsValidLocation(Vector2 point, bool defaultResist = true)
        {
            if (point != null && (defaultResist) ? (point != default(Vector2)) : true)
                if (point.X <= Main.maxTilesX && point.X >= 0)
                {
                    if (point.Y <= Main.maxTilesY && point.Y >= 0)
                        return true;
                }

            return false;
        }

        ///// <summary>
        ///// Checks whether an item is rejected in this server
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //public static bool RejectedItemsContains(string item)
        //{
        //    if (!String.IsNullOrEmpty(item))
        //    {
        //        foreach (string rItem in RejectedItems)
        //        {
        //            if (rItem.Trim().Replace(" ", String.Empty) == item.Trim().Replace(" ", String.Empty))
        //                return true;
        //        }
        //    }
        //    return false;
        //}

        /// <summary>
        /// Checks online players for a matching name part
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static List<Player> FindPlayerByPart(string partName, bool ignoreCase = true)
        {
            List<Player> matches = new List<Player>();

            foreach (var player in Main.player)
            {
                if (player.Name == null)
                    continue;

                string playerName = player.Name;

                if (ignoreCase)
                    playerName = playerName.ToLower();

                if (playerName.StartsWith((ignoreCase) ? partName.ToLower() : partName))
                    matches.Add(player);
            }

            return matches;
        }

        ///// <summary>
        ///// Tries to find an item by Type or Name via Definitions
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="ItemIdOrName"></param>
        ///// <param name="ItemList"></param>
        ///// <returns></returns>
        //public static bool TryFindItem<T>(T ItemIdOrName, out List<ItemInfo> ItemList)
        //{
        //    ItemList = new List<ItemInfo>();

        //    foreach (var pair in Registries.Item.TypesById)
        //    {
        //        var items = pair.Value as List<Item>;

        //        if (ItemIdOrName is Int32)
        //        {
        //            var itemT = Int32.Parse(ItemIdOrName.ToString());

        //            foreach (var item in items)
        //            {
        //                var type = item.Type;
        //                if (type == itemT && !ItemList.ContainsType(type))
        //                    ItemList.Add(new ItemInfo()
        //                    {
        //                        Type = type,
        //                        NetID = item.NetID
        //                    });
        //            }
        //        }
        //        else if (ItemIdOrName is String)
        //        {
        //            var findItem = CleanName(ItemIdOrName as String);

        //            foreach (var item in items)
        //            {
        //                var type = item.Type;
        //                var curItem = CleanName(item.Name);

        //                if (curItem == findItem && !ItemList.ContainsType(type))
        //                    ItemList.Add(new ItemInfo()
        //                    {
        //                        Type = type,
        //                        NetID = item.NetID
        //                    });
        //            }
        //        }
        //    }

        //    foreach (var pair in Registries.Item.TypesByName)
        //    {
        //        var item = pair.Value as Item;

        //        if (ItemIdOrName is Int32)
        //        {
        //            var itemT = Int32.Parse(ItemIdOrName.ToString());
        //            var type = item.Type;

        //            if (type == itemT && !ItemList.ContainsType(type))
        //                ItemList.Add(new ItemInfo()
        //                {
        //                    Type = type,
        //                    NetID = item.NetID
        //                });
        //        }
        //        else if (ItemIdOrName is String)
        //        {
        //            var type = item.Type;
        //            var findItem = CleanName(ItemIdOrName as String);
        //            var curItem = CleanName(item.Name);

        //            if (curItem == findItem && !ItemList.ContainsType(type))
        //                ItemList.Add(new ItemInfo()
        //                {
        //                    Type = type,
        //                    NetID = item.NetID
        //                });
        //        }
        //    }

        //    return ItemList.Count > 0;
        //}

        ///// <summary>
        ///// Uses the undefined item method to find an item by Type.
        ///// </summary>
        ///// <param name="ItemID"></param>
        ///// <param name="ItemList"></param>
        ///// <returns></returns>
        //public static bool TryFindItemByType(int itemId, out List<ItemInfo> ItemList)
        //{
        //    return itemId(ItemID, out ItemList);
        //}

        ///// <summary>
        ///// Uses the undefined item method to find an item by Name.
        ///// </summary>
        ///// <param name="ItemName"></param>
        ///// <param name="ItemList"></param>
        ///// <returns></returns>
        //public static bool TryFindItemByName(string name, out List<ItemInfo> ItemList)
        //{
        //    return TryFindItem(name, out ItemList);
        //}

        /// <summary>
        /// Used to clean the names of Items or NPC's for parsing
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanName(string input)
        {
            return input.Replace(" ", String.Empty).ToLower();
        }

        /// <summary>
        /// Attempts to find the first online player
        ///		Usually the Slot Manager assigns them to the lowest possible index
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool TryGetFirstOnlinePlayer(out Player player)
        {
            player = null;
            try
            {
                for (var i = 0; i < Main.player.Length; i++)
                {
                    var ply = Main.player[i];
                    if (ply.active && ply.Name.Trim() != String.Empty)
                    {
                        player = ply;
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool ContainsType(this List<ItemInfo> list, int type)
        {
            return list.Where(x => x.Type == type).Count() > 0;
        }

        /// <summary>
        /// Checks if there are any active NPCs of specified type
        /// </summary>
        /// <param name="type">TypeId of NPC to check for</param>
        /// <returns>True if active, false if not</returns>
        public static bool IsNPCSummoned(int type)
        {
            //for (int i = 0; i < Main.npc.Length; i++)
            //{
            //    NPC npc = Main.npc[i];
            //    if (npc != null && npc.active && npc.type == type)
            //        return true;
            //}
            //return false;
            return NPC.AnyNPCs(type);
        }

        /// <summary>
        /// Checks if there are any active NPCs of specified name
        /// </summary>
        /// <param name="Name">Name of NPC to check for</param>
        /// <returns>True if active, false if not</returns>
        public static bool IsNPCSummoned(string name)
        {
            int Id;
            return TryFindNPCByName(name, out Id);
        }

        public static bool TryFindNPCByName(string name, out int id)
        {
            id = default(Int32);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (npc != null && npc.active && npc.name == name)
                {
                    id = npc.whoAmI;
                    return true;
                }
            }
            return false;
        }

        public static int AvailableNPCSlots
        {
            get
            {
                return Main.npc
                  .Where(x => x == null || !x.active)
                  .Count();
            }
        }

        public static int UsedNPCSlots
        {
            get
            {
                return Main.npc
                  .Where(x => x != null && x.active)
                  .Count();
            }
        }

        public static int AvailableItemSlots
        {
            get
            {
                return Main.item
                  .Where(x => x == null || !x.active)
                  .Count();
            }
        }

        public static int UsedItemSlots
        {
            get
            {
                return Main.item
                  .Where(x => x != null && x.active)
                  .Count();
            }
        }

        public static RuntimePlatform RuntimePlatform
        {
            get
            { return Type.GetType("Mono.Runtime") != null ? RuntimePlatform.Mono : RuntimePlatform.Microsoft; }
        }
    }

    public struct ItemInfo
    {
        public int NetID { get; set; }
        public int Type { get; set; }
    }

    public enum RuntimePlatform
    {
        Microsoft = 1, //.net
        Mono
    }
}