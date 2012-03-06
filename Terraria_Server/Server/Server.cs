using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <summary>
        /// Simple solution for Login Systems to set so other plugins knows there is another instance running 
        /// </summary>
        public static bool UsingLoginSystem { get; set; }

        /// <summary>
        /// Items which the player should be kicked for attemting to join the server
        /// </summary>
        public static List<String> RejectedItems { get; set; }
        
        /// <summary>
        /// Gets the White list
        /// </summary>
        public static DataRegister WhiteList { get; set; }

        /// <summary>
        /// Gets the Ban list
        /// </summary>
        public static DataRegister BanList { get; set; }

        /// <summary>
        /// Gets the OP list
        /// </summary>
        public static DataRegister OpList { get; set; }

        /// <summary>
        /// Allows explosions if set
        ///     e.g. Dynamite
        /// </summary>
        public static bool AllowExplosions { get; set; }

        /// <summary>
        /// Allows the TDCM Client to use RPG on the server.
        /// </summary>
        public static bool AllowTDCMRPG { get; set; }
        
        /// <summary>
        /// When the server is ran, Data needs to be set
        /// </summary>
        /// <param name="PlayerCap"></param>
        /// <param name="myWhiteList"></param>
        /// <param name="myBanList"></param>
        /// <param name="myOpList"></param>
        public static void InitializeData(int PlayerCap, string myWhiteList, string myBanList, string myOpList)
        {
            UsingLoginSystem = false;
            
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
					RejectedItems.Add(rejItem[i].Trim());
            }

            AllowExplosions = Program.properties.AllowExplosions;
            AllowTDCMRPG = Program.properties.AllowTDCMRPG;
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
            foreach (Player player in Main.players)
            {
                if (player.Active && player.Name.ToLower().Equals(lowercaseName))
					return player;
            }
            return null;
        }

		/// <summary>
		/// Send a message to all online OPs
		/// </summary>
		/// <param name="Message"></param>
		/// <param name="writeToConsole"></param>
		/// <param name="Logger"></param>
		public static void notifyOps(string Message, bool writeToConsole = true, SendingLogger Logger = SendingLogger.CONSOLE)
		{
			if (Statics.cmdMessages)
			{
				foreach (Player player in Main.players)
				{
					if (player.Active && player.Op)
						NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmi, -1, Message, 255, 176f, 196, 222f);
				}
			}

			if (writeToConsole) ProgramLog.Admin.Log(Message, Logger);
		}
		
		/// <summary>
		/// Sends a Message to all Connected Clients
		/// </summary>
		/// <param name="Message"></param>
		/// <param name="writeToConsole"></param>
		/// <param name="Logger"></param>
		public static void notifyAll(string Message, bool writeToConsole = true, SendingLogger Logger = SendingLogger.CONSOLE)
		{
			NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, Message, 255, 238f, 130f, 238f);
			if (writeToConsole) ProgramLog.Admin.Log(Message, Logger);
		}

		/// <summary>
		/// Sends a Message to all Connected Clients
		/// </summary>
		/// <param name="Message"></param>
		/// <param name="ChatColour"></param>
		/// <param name="writeToConsole"></param>
		/// <param name="Logger"></param>
		public static void notifyAll(string Message, Color ChatColour, bool writeToConsole = true, SendingLogger Logger = SendingLogger.CONSOLE)
		{
			NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, Message, 255, ChatColour.R, ChatColour.G, ChatColour.B);
			if (writeToConsole) ProgramLog.Admin.Log(Message, Logger);
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
			return (from x in Main.npcs where x.Active select x).Count();
        }

        /// <summary>
        /// Gets/Sets the maximum allowed NPCs
        /// </summary>
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

        /// <summary>
        /// Gets/Sets the max spawn rate of NPCs
        /// </summary>
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

        /// <summary>
        /// Checks whether an item is rejected in this server
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool RejectedItemsContains(string item)
        {
            if (!String.IsNullOrEmpty(item))
            {
                foreach (string rItem in RejectedItems)
                {
                    if (rItem.Trim().Replace(" ", String.Empty) == item.Trim().Replace(" ", String.Empty))
						return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks online players for a matching name part
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
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
					matches.Add(player);
            }

            return matches;
        }
        
        /// <summary>
        /// Tries to find an item by Type or Name via Definitions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ItemIdOrName"></param>
        /// <param name="ItemList"></param>
        /// <returns></returns>
        public static bool TryFindItem<T>(T ItemIdOrName, out List<ItemInfo> ItemList)
        {
			ItemList = new List<ItemInfo>();

            foreach (var pair in Registries.Item.TypesById)
            {
                var items = pair.Value as List<Item>;

                if (ItemIdOrName is Int32)
                {
                    var itemT = Int32.Parse(ItemIdOrName.ToString());

                    foreach (var item in items)
                    {
                        var type = item.Type;
						if (type == itemT && !ItemList.ContainsType(type))
							ItemList.Add(new ItemInfo()
							{
								Type = type,
								NetID = item.NetID
							});
                    }
                }
                else if (ItemIdOrName is String)
                {
                    var findItem = CleanName(ItemIdOrName as String);

                    foreach (var item in items)
                    {
                        var type = item.Type;
                        var curItem = CleanName(item.Name);

						if (curItem == findItem && !ItemList.ContainsType(type))
							ItemList.Add(new ItemInfo()
							{
								Type = type,
								NetID = item.NetID
							});
                    }
                }
            }

            foreach (var pair in Registries.Item.TypesByName)
            {
                var item = pair.Value as Item;

                if (ItemIdOrName is Int32)
                {
                    var itemT = Int32.Parse(ItemIdOrName.ToString());
                    var type = item.Type;

                    if (type == itemT && !ItemList.ContainsType(type))
						ItemList.Add(new ItemInfo()
						{
							Type = type,
							NetID = item.NetID
						});
                }
                else if (ItemIdOrName is String)
                {
                    var type = item.Type;
                    var findItem = CleanName(ItemIdOrName as String);
                    var curItem = CleanName(item.Name);

					if (curItem == findItem && !ItemList.ContainsType(type))
						ItemList.Add(new ItemInfo()
						{
							Type = type,
							NetID = item.NetID
						});
                }
            }

            return ItemList.Count > 0;
        }

        /// <summary>
        /// Uses the undefined item method to find an item by Type.
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ItemList"></param>
        /// <returns></returns>
		public static bool TryFindItemByType(int ItemID, out List<ItemInfo> ItemList)
        {
            return TryFindItem(ItemID, out ItemList);
        }

        /// <summary>
        /// Uses the undefined item method to find an item by Name.
        /// </summary>
        /// <param name="ItemName"></param>
        /// <param name="ItemList"></param>
        /// <returns></returns>
        public static bool TryFindItemByName(string ItemName, out List<ItemInfo> ItemList)
        {
            return TryFindItem(ItemName, out ItemList);
        }

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
				for (var i = 0; i < NetPlay.maxConnections; i++)
				{
					var ply = Main.players[i];
					if (ply.Active && ply.Name.Trim() != String.Empty)
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
    }

	public struct ItemInfo
	{
		public int NetID { get; set; }
		public int Type { get; set; }
	}
}
