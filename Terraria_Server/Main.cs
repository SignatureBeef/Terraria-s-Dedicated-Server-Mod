//#define CATCHERROR_NPCUPDATES

using System.Threading;
using System.Diagnostics;
using System;
using System.Text;
using System.Net;
using System.IO;
using Terraria_Server.Misc;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;
using Terraria_Server.Plugins;

namespace Terraria_Server
{
	public static class Main
	{
		public const Int32 MAX_TILE_SETS = 150;
		public const Int32 MAX_WALL_SETS = 32;
		public const Int32 MAX_BUFFS = 41;
		public const Int32 MAX_NPC_NAMES = 147;
		public const Int32 MAX_CHESTS = 1000;
		public const Int32 MAX_SIGNS = 1000;

		public static bool Xmas { get; set; }

		public static Item trashItem { get; set; }

		public static bool[] debuff = new bool[MAX_BUFFS];
		public const int maxItemText = 100;
		public const int MAX_PLAYERS = 255;
		public const int maxItemTypes = 586;
		public const int maxProjectileTypes = 109;
		public const int maxProjectiles = 1000;
		public const int maxNPCTypes = 74;
		public const double dayLength = 54000.0;

		public static bool stopSpawns = false;
		public static bool SpawnsOverride = false;

		public static bool ignoreErrors = true;
		public static bool webProtect = false;

		public static float leftWorld = 0f;
		public static float rightWorld = 134400f;
		public static float topWorld = 0f;
		public static float bottomWorld = 38400f;

		public static int maxTilesX = -1;
		public static int maxTilesY = -1;

		public static int maxSectionsX = maxTilesX / 200;
		public static int maxSectionsY = maxTilesY / 150;

		//[Obsolete("Replaced by SlotManager.MaxSlots")]
		//public static int maxNetplayers = 254;

		public static int dungeonX;
		public static int dungeonY;
		public static Liquid[] liquid = new Liquid[Liquid.resLiquid];
		public static LiquidBuffer[] liquidBuffer = new LiquidBuffer[10000];
		public static string worldName = "";
		public static int worldID;
		public static double worldSurface;
		public static double rockLayer;
		public static Color[] teamColor = new Color[5];
		public static Color tileColor;
		public static bool dayTime = true;

		public static bool UseTimeLock { get; set; }

		private static double _time = 13500.0;
		public static double Time
		{
			get { return _time; }
			set
			{
				if (UseTimeLock)
					return;

				_time = value;
			}
		}

		public static int moonPhase = 0;
		public static bool bloodMoon = false;
		public static int checkForSpawns = 0;
		public static int helpText = 0;
		public static int evilTiles;

		public static float harpNote = 0f;
		public static bool[] tileMergeDirt = new bool[MAX_TILE_SETS];
		public static bool[] tileCut = new bool[MAX_TILE_SETS];
		public static bool[] tileAlch = new bool[MAX_TILE_SETS];
		public static int[] tileShine = new int[MAX_TILE_SETS];
		public static bool[] wallHouse = new bool[MAX_WALL_SETS];
		public static bool[] tileStone = new bool[MAX_TILE_SETS];
		public static bool[] tileWaterDeath = new bool[MAX_TILE_SETS];
		public static bool[] tileLavaDeath = new bool[MAX_TILE_SETS];
		public static bool[] tileTable = new bool[MAX_TILE_SETS];
		public static bool[] tileBlockLight = new bool[MAX_TILE_SETS];
		public static bool[] tileDungeon = new bool[MAX_TILE_SETS];
		public static bool[] tileSolidTop = new bool[MAX_TILE_SETS];
		public static bool[] tileSolid = new bool[MAX_TILE_SETS];
		public static bool[] tileNoAttach = new bool[MAX_TILE_SETS];
		public static bool[] tileNoFail = new bool[MAX_TILE_SETS];
		public static bool[] tileFrameImportant = new bool[MAX_TILE_SETS];
		public static string[] chrName = new string[MAX_NPC_NAMES];

		[ThreadStatic]
		static Random threadRand;

		public static Random rand
		{
			get
			{
				if (threadRand == null)
					threadRand = new Random((int)DateTime.Now.Ticks);
				return threadRand;
			}
			set { }
		}

		public static TileCollection tile;
		public static Item[] item = new Item[201];
		public static NPC[] npcs = new NPC[NPC.MAX_NPCS + 1];
		public static Projectile[] projectile = new Projectile[1001];
		public static Chest[] chest = new Chest[MAX_CHESTS];
		public static Sign[] sign = new Sign[MAX_SIGNS];
		public static Vector2 screenPosition;
		public static int screenWidth = 800;
		public static int screenHeight = 600;
		public static bool playerInventory = false;
		public const int myPlayer = 255;
		public static Player[] players = new Player[MAX_PLAYERS + 1];
		public static int spawnTileX;
		public static int spawnTileY;
		public static InvasionType invasionType = 0;
		public static double invasionX = 0.0;
		public static int invasionSize = 0;
		public static int invasionDelay = 0;
		public static int invasionWarn = 0;
		public static int[] npcFrameCount = new int[]
		{
			1,			2, 			2, 			3, 			6, 			2, 		
			2, 			1, 			1,			1, 			1, 			1, 
			1, 			1, 			1,			1, 			2, 			16, 
			14,			16, 		14, 		15,			16, 		2, 
			10, 		1, 			16, 		16, 		16,			3,
			1, 			15, 		3, 			1, 			3, 			1, 
			1, 			16, 		16, 		1, 			1, 			1, 
			3, 			3, 			15, 		3, 			7, 			7, 
			4, 			5, 			5, 			5, 			3, 			3, 
			16, 		6, 			3, 			6, 			6, 			2, 
			5, 			3, 			2, 			7, 			7, 			4, 
			2, 			8, 			1, 			5, 			1, 			2, 
			4, 			16, 		5, 			4, 			4, 			15, 
			15, 		15, 		15, 		2, 			4, 			6, 
			6, 			18, 		16, 		1, 			1, 			1, 
			1, 			1, 			1, 			4, 			3, 			1, 
			1, 			1, 			1, 			1, 			1, 			5, 
			6, 			7, 			16, 		1, 			1, 			16, 
			16, 		12, 		20, 		21, 		1, 			2, 
			2, 			3, 			6, 			1, 			1, 			1, 
			15, 		4, 			11, 		1, 			14, 		6, 
			6, 			3, 			1, 			2, 			2, 			1, 
			3, 			4, 			1, 			2, 			1, 			4, 
			2, 			1, 			15, 		3, 			16, 		4, 
			5, 			7, 			3
		};
		public static int timeOut = 120;
		public static int NetplayCounter;
		public static int lastItemUpdate;
		public static int maxNPCUpdates = 15;
		public static int maxItemUpdates = 10;
		public static bool autoPass = false;
		public static int zoneX = 99;
		public static int zoneY = 87;

		/// <summary>
		/// Checks whether an invasion is occurring.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsInvasionOccurring(InvasionType type)
		{
			return type == invasionType;
		}

		/// <summary>
		/// Checks whether an invasion is occurring.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="transform">Used when the SpawnNPC packet receives a message about a new Invasion</param>
		/// <returns></returns>
		public static bool IsInvasionOccurring(int type, bool transform = false)
		{
			if (transform)
			{
				if (type == -1 || type == -2)
					type *= -1;
				else return false;
			}

			return IsInvasionOccurring((InvasionType)type);
		}

		public static void Initialize()
		{
			if (Program.properties != null)
			{
				stopSpawns = Program.properties.StopNPCSpawning;
				SpawnsOverride = Program.properties.NPCSpawnsOverride;
			}

			NPC.ClearNames();
			NPC.SetNames();

			//foreach (var buff in new int[] { 20, 24, 31, 39 })
			//    pvpBuff[buff] = true;

			foreach (var buff in new int[] { 20, 21, 22, 23, 24, 25, 28, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 })
				debuff[buff] = true;

			foreach (var i in new int[] { 3, 4, 5, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20, 21, 24, 26, 27, 28, 29, 
				31, 33, 34, 35, 36, 42, 50, 55, 61, 71, 72, 73, 74, 77, 78, 79, 81, 82, 83, 84, 85, 86, 87, 88, 89, 
				90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 110, 113, 114, 125, 126, 
				128, 129, 132, 133, 134, 135, 136, 137, 138, 139, 141, 142, 143, 144, 149 })
				tileFrameImportant[i] = true;

			foreach (var i in new int[] { 63, 64, 65, 66, 67, 68, 130, 131 })
				tileStone[i] = true;

			foreach (var i in new int[] { 0, 1, 2, 6, 7, 8, 9, 10, 22, 23, 25, 30, 32, 37, 38, 39, 40, 41, 43, 44, 45, 
				46, 47, 48, 52, 53, 56, 57, 58, 59, 60, 62, 63, 64, 65, 66, 67, 68, 70, 75, 76, 107, 108, 109, 111, 112, 
				115, 116, 117, 118, 119, 120, 121, 122, 123, 130, 131, 137, 138, 140, 145, 146, 147, 148 })
				tileBlockLight[i] = true;

			foreach (var i in new int[] { 0, 1, 2, 6, 7, 8, 9, 10, 19, 22, 23, 25, 30, 37, 38, 39, 40, 41, 43, 44, 45, 46, 
				47, 48, 53, 54, 56, 57, 58, 59, 60, 63, 64, 65, 66, 67, 68, 70, 75, 76, 107, 108, 109, 111, 112, 116, 117, 
				118, 119, 120, 121, 122, 123, 127, 130, 137, 138, 140, 145, 146, 147, 148 })
				tileSolid[i] = true;

			foreach (var i in new int[] { 1, 6, 7, 8, 9, 22, 25, 30, 37, 38, 39, 40, 41, 43, 44, 45, 46, 47, 53, 56, 107, 
				108, 111, 112, 116, 117, 118, 119, 120, 121, 122, 123, 140, 145, 146, 147, 148 })
				tileMergeDirt[i] = true;

			foreach (var i in new int[] { 3, 24, 28, 32, 51, 52, 61, 62, 69, 71, 73, 74, 82, 83, 84, 110, 113, 115 })
				tileCut[i] = true;

			foreach (var i in new int[] { 3, 4, 24, 32, 50, 61, 69, 73, 74, 82, 83, 84, 110, 113 })
				tileNoFail[i] = true;

			foreach (var i in new int[] { 1, 4, 5, 6, 10, 11, 12, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 29, 30, 31 })
				wallHouse[i] = true;

			foreach (var i in new int[] { 82, 83, 84 })
				tileAlch[i] = true;

			foreach (var i in new int[] { 3, 5, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 27, 28, 29, 32, 33, 34, 35, 36, 
				42, 49, 50, 52, 55, 61, 62, 69, 71, 72, 73, 74, 79, 80, 81, 86, 87, 88, 89, 91, 92, 93, 94, 95, 96, 97, 98, 
				99, 100, 101, 102, 103, 104, 106, 110, 113, 115, 125, 126 })
				tileLavaDeath[i] = true;

			foreach (var i in new int[] { 3, 4, 10, 13, 14, 15, 16, 17, 18, 19, 20, 21, 27, 50, 86, 87, 88, 89, 90, 91, 92, 
				93, 94, 95, 96, 97, 98, 99, 101, 102, 110, 114 })
				tileNoAttach[i] = true;

			foreach (var i in new int[] { 41, 43, 44 })
				tileDungeon[i] = true;

			foreach (var i in new int[] { 14, 18, 19, 87, 88, 101, 114 })
				tileTable[i] = true;

			foreach (var i in new int[] { 4, 51, 93, 98 })
				tileWaterDeath[i] = true;


			foreach (var i in new int[] { 8, 12 })
				tileShine[i] = 1000;
			foreach (var i in new int[] { 9 })
				tileShine[i] = 1050;
			foreach (var i in new int[] { 7 })
				tileShine[i] = 1100;
			foreach (var i in new int[] { 6, 22 })
				tileShine[i] = 1150;
			foreach (var i in new int[] { 21 })
				tileShine[i] = 1200;
			foreach (var i in new int[] { 122 })
				tileShine[i] = 1800;
			foreach (var i in new int[] { 121 })
				tileShine[i] = 1850;
			foreach (var i in new int[] { 45 })
				tileShine[i] = 1900;
			foreach (var i in new int[] { 46 })
				tileShine[i] = 2000;
			foreach (var i in new int[] { 47 })
				tileShine[i] = 2100;
			foreach (var i in new int[] { 63, 64, 65, 66, 67, 68, 108 })
				tileShine[i] = 900;
			foreach (var i in new int[] { 111 })
				tileShine[i] = 850;
			foreach (var i in new int[] { 107 })
				tileShine[i] = 950;
			foreach (var i in new int[] { 129 })
				tileShine[i] = 300;
			foreach (var i in new int[] { 109, 110, 117, 116 })
				tileShine[i] = 9000;
			foreach (var i in new int[] { 118 })
				tileShine[i] = 8000;
			foreach (var i in new int[] { 125 })
				tileShine[i] = 600;


			foreach (var i in new int[] { 14, 16, 18, 19, 87, 88, 101, 114 })
				tileSolidTop[i] = true;


			for (int l = 0; l < Item.MAX_ITEMS + 1; l++)
				item[l] = new Item();
			for (int m = 0; m < NPC.MAX_NPCS + 1; m++)
			{
				npcs[m] = new NPC();
				npcs[m].whoAmI = m;
			}
			for (int i = 0; i < MAX_PLAYERS + 1; i++)
				players[i] = new Player();
			for (int num2 = 0; num2 < 1001; num2++)
				projectile[num2] = new Projectile();
			for (int num10 = 0; num10 < Liquid.resLiquid; num10++)
				liquid[num10] = new Liquid();
			for (int num11 = 0; num11 < 10000; num11++)
				liquidBuffer[num11] = new LiquidBuffer();

			teamColor[0] = new Color(255, 255, 255);
			teamColor[1] = new Color(230, 40, 20);
			teamColor[2] = new Color(20, 200, 30);
			teamColor[3] = new Color(75, 90, 255);
			teamColor[4] = new Color(200, 180, 0);

			NetPlay.Init();
		}

		private static void UpdateInvasion()
		{
			if (invasionType > 0)
			{
				if (invasionSize <= 0)
				{
					if (invasionType == InvasionType.GOBLIN_ARMY)
					{
						NPC.downedGoblins = true;
						NetMessage.SendData(7);
					}
					else if (invasionType == InvasionType.FROST_LEGION)
						NPC.downedFrost = true;

					InvasionWarning();
					invasionType = 0;
					invasionDelay = 7;
				}
				if (invasionX == (double)spawnTileX)
					return;
				float num = 0.2f;
				if (invasionX > (double)spawnTileX)
				{
					invasionX -= (double)num;
					if (invasionX <= (double)spawnTileX)
					{
						invasionX = (double)spawnTileX;
						InvasionWarning();
					}
					else
						invasionWarn--;
				}
				else if (invasionX < (double)spawnTileX)
				{
					invasionX += (double)num;
					if (invasionX >= (double)spawnTileX)
					{
						invasionX = (double)spawnTileX;
						InvasionWarning();
					}
					else
						invasionWarn--;
				}
				if (invasionWarn <= 0)
				{
					invasionWarn = 3600;
					InvasionWarning();
				}
			}
		}

		private static void InvasionWarning()
		{
			var info = String.Empty;
			var type = (invasionType == InvasionType.FROST_LEGION) ? "The Frost Legion" : "A goblin army";

			if (invasionSize <= 0)
				info = type + " has been defeated!";
			else if (invasionX < (double)spawnTileX)
				info = type + " is approaching from the west!";
			else if (invasionX > (double)spawnTileX)
				info = type + " is approaching from the east!";
			else if (invasionType == InvasionType.FROST_LEGION)
				info = type + " has arrived!";

			if (info != String.Empty)
				NetMessage.SendData(25, -1, -1, info, 255, 175f, 75f, 255f);
		}

		/// <summary>
		/// Invokes a new invasion based on the type.
		/// </summary>
		/// <param name="type"></param>
		public static void StartInvasion(InvasionType type = InvasionType.GOBLIN_ARMY)
		{
			if (invasionType == 0 && invasionDelay == 0)
			{
				int healthy = 0;
				for (int i = 0; i < MAX_PLAYERS; i++)
				{
					if (players[i].Active && players[i].statLifeMax >= 200)
						healthy++;
				}

				if (healthy > 0)
				{
					invasionType = type;
					invasionSize = 80 + 40 * healthy;
					invasionWarn = 0;

					if (rand.Next(2) == 0)
					{
						invasionX = 0.0;
						return;
					}

					invasionX = (double)maxTilesX;
				}
			}
		}

		private static void UpdateServer()
		{
			NetplayCounter++;
			if (NetplayCounter > 3600)
			{
				NetMessage.SendData(7);
				NetMessage.SyncNPCHomes();
				//NetMessage.SyncPlayers();
				NetplayCounter = 0;
			}
			if (NetplayCounter % 30 == 0)
				for (int i = 0; i < 255; i++)
				{
					var player = players[i];
					var rows = player.rowsToRectify;
					if (player.Active && rows.Count > 0)
					{
						bool locked = false;
						try
						{
							locked = Monitor.TryEnter(rows);
							if (!locked)
								//ProgramLog.Debug.Log ("not acquired!");
								continue;

							var conn = player.Connection;
							if (conn == null)
								continue;

							int Y1 = int.MaxValue;
							int Y2 = 0;
							int X1 = int.MaxValue;
							int X2 = 0;

							var msg = NetMessage.PrepareThreadInstance();
							bool warn = false;

							if (rows.Count <= 150)
								foreach (var kv in rows)
								{
									var y = kv.Key;
									var x1 = (int)(kv.Value >> 16);
									var x2 = (int)(kv.Value & 0xffff);

									if (x1 > x2)
										continue;

									var len = x2 - x1 + 1;

									if (len > 200)
									{
										warn = true;
										break;
									}

									if (y < Y1)
										Y1 = y;
									if (y > Y2)
										Y2 = y;
									if (x1 < X1)
										X1 = x1;
									if (x2 > X2)
										X2 = x2;

									if (conn.CompressionVersion == 1)
										msg.TileRowCompressed(len, x1, y);
									else
										msg.SendTileRow(len, x1, y);
								}
							else
								warn = true;

							rows.Clear();

							if (warn)
							{
								msg.Clear();
								msg.PlayerChat(255, "<Server> Your view of the map is out of sync.", 255, 128, 128);
								msg.Send(conn);
							}
							else if (msg.Written > 0)
							{
								msg.SendTileConfirm(X1 / 200, Y1 / 150, X2 / 200, Y2 / 150);
								msg.Send(conn);
							}
						}
						finally
						{
							if (locked)
								Monitor.Exit(rows);
						}
					}
				}

			for (int i = 0; i < 255; i++)
				if (players[i].Active && NetPlay.slots[i].state >= SlotState.CONNECTED)
					NetPlay.slots[i].SpamUpdate();

			Math.IEEERemainder((double)NetplayCounter, 60.0);
			if (Math.IEEERemainder((double)NetplayCounter, 360.0) == 0.0)
			{
				bool flag2 = true;
				int num = lastItemUpdate;
				int num2 = 0;
				while (flag2)
				{
					num++;
					if (num >= 200)
						num = 0;
					num2++;
					if (!item[num].Active || item[num].Owner == 255)
						NetMessage.SendData(21, -1, -1, "", num);
					if (num2 >= maxItemUpdates || num == lastItemUpdate)
						flag2 = false;
				}
				lastItemUpdate = num;
			}

			for (int i = 0; i < 200; i++)
				if (item[i].Active && (item[i].Owner == 255 || !players[item[i].Owner].Active))
					item[i].FindOwner(i);

			for (int i = 0; i < 255; i++)
			{
				//                if (Netplay.slots[i].state >= SlotState.CONNECTED)
				//                {
				//                    //Netplay.slots[i].timeOut++;
				//                    if (/*!stopTimeOuts && */Netplay.slots[i].timeOut > 60 * timeOut)
				//                    {
				//                        Netplay.slots[i].Kick ("Timed out.");
				//                    }
				//                }

				Player player = players[i];
				if (player.Active)
				{
					int sectionX = NetPlay.GetSectionX((int)(player.Position.X / 16f));
					int sectionY = NetPlay.GetSectionY((int)(player.Position.Y / 16f));
					int num3 = 0;
					for (int j = sectionX - 1; j < sectionX + 2; j++)
						for (int k = sectionY - 1; k < sectionY + 2; k++)
							if (j >= 0 && j < maxSectionsX && k >= 0 && k < maxSectionsY)
								if (!NetPlay.slots[i].tileSection[j, k])
									num3++;

					if (num3 > 0)
					{
						int num4 = num3 * 150;
						NetMessage.SendData(9, i, -1, "Receiving tile data", num4);
						NetPlay.slots[i].statusText2 = "is receiving tile data";
						NetPlay.slots[i].statusMax += num4;
						for (int j = sectionX - 1; j < sectionX + 2; j++)
							for (int k = sectionY - 1; k < sectionY + 2; k++)
								if (j >= 0 && j < maxSectionsX && k >= 0 && k < maxSectionsY)
									if (!NetPlay.slots[i].tileSection[j, k])
									{
										NetMessage.SendSection(i, j, k);
										NetMessage.SendData(11, i, -1, "", j, (float)k, (float)j, (float)k);
									}
					}
					else
					{
						var conn = player.Connection;
						if (conn != null)
							conn.Flush();
					}
				}
				else
				{
					var conn = player.Connection;
					if (conn != null)
						conn.Flush();
				}
			}
		}

		private static void UpdateTime()
		{
			Time += 1.0;

			if (!dayTime)
			{
				if (WorldModify.spawnEye && Time > 4860.0)
					foreach (Player player in players)
					{
						if (player.Active && !player.dead && (double)player.Position.Y < worldSurface * 16.0)
						{
							NPC.SpawnOnPlayer(player.whoAmi, 4);
							WorldModify.spawnEye = false;
							break;
						}
					}

				if (Time > 32400.0)
				{
					checkXmas();

					if (invasionDelay > 0)
						invasionDelay--;

					WorldModify.spawnNPC = 0;
					checkForSpawns = 0;
					Time = 0.0;
					bloodMoon = false;
					dayTime = true;
					moonPhase++;
					if (moonPhase >= 8)
						moonPhase = 0;

					NetMessage.SendData(7);
					WorldIO.SaveWorldThreaded();

					if (WorldModify.shadowOrbSmashed)
					{
						var startInvasion = !NPC.downedGoblins ? rand.Next(3) == 0 : rand.Next(15) == 0;
						if (startInvasion)
							StartInvasion();
					}
				}
				if (Time > 16200.0 && WorldModify.spawnMeteor)
				{
					WorldModify.spawnMeteor = false;
					WorldModify.dropMeteor(null, null);
					return;
				}
			}
			else
			{
				bloodMoon = false;
				if (Time > 54000.0)
				{
					WorldModify.spawnNPC = 0;
					checkForSpawns = 0;
					if (rand.Next(50) == 0 && WorldModify.shadowOrbSmashed)
						WorldModify.spawnMeteor = true;

					if (!NPC.downedBoss1)
					{
						bool flag = false;
						foreach (Player player in players)
						{
							if (player.Active && player.statLifeMax >= 200 && player.statDefense > 10)
							{
								flag = true;
								break;
							}
						}
						if (flag && rand.Next(3) == 0)
						{
							int num = 0;
							for (int i = 0; i < NPC.MAX_NPCS; i++)
							{
								if (npcs[i].Active && npcs[i].townNPC)
									num++;
							}

							if (num >= 4)
							{
								WorldModify.spawnEye = true;

								NetMessage.SendData(25, -1, -1, "You feel an evil presence watching you...", 255, 50f, 255f, 130f);
							}
						}
					}
					if (!WorldModify.spawnEye && moonPhase != 4 && rand.Next(9) == 0)
					{
						for (int i = 0; i < 255; i++)
							if (players[i].Active && players[i].statLifeMax > 100)
							{
								bloodMoon = true;
								break;
							}

						if (bloodMoon)
							NetMessage.SendData(25, -1, -1, "The Blood Moon is rising...", 255, 50f, 255f, 130f);
					}
					Time = 0.0;
					dayTime = false;

					NetMessage.SendData(7);
				}

				//checkForSpawns++;
				if (++checkForSpawns >= 7200)
				{
					int num2 = 0;
					for (int i = 0; i < 255; i++)
						if (players[i].Active)
							num2++;

					checkForSpawns = 0;
					WorldModify.spawnNPC = 0;
					int num3 = 0;
					int num4 = 0;
					int num5 = 0;
					int num6 = 0;
					int num7 = 0;
					int num8 = 0;
					int num9 = 0;
					int num10 = 0;
					int num11 = 0;

					int goblin = 0, wizard = 0, mechanic = 0, santa = 0;

					for (int i = 0; i < NPC.MAX_NPCS; i++)
						if (npcs[i].Active && npcs[i].townNPC)
						{
							if (npcs[i].type != NPCType.N37_OLD_MAN && !npcs[i].homeless)
								WorldModify.QuickFindHome(null, i);

							switch (npcs[i].type)
							{
								case NPCType.N37_OLD_MAN:
									num8++;
									break;
								case NPCType.N17_MERCHANT:
									num3++;
									break;
								case NPCType.N18_NURSE:
									num4++;
									break;
								case NPCType.N19_ARMS_DEALER:
									num6++;
									break;
								case NPCType.N20_DRYAD:
									num5++;
									break;
								case NPCType.N22_GUIDE:
									num7++;
									break;
								case NPCType.N38_DEMOLITIONIST:
									num9++;
									break;
								case NPCType.N54_CLOTHIER:
									num10++;
									break;
								case NPCType.N107_GOBLIN_TINKERER:
									goblin++;
									break;
								case NPCType.N108_WIZARD:
									wizard++;
									break;
								case NPCType.N124_MECHANIC:
									mechanic++;
									break;
								case NPCType.N142_SANTA_CLAUS:
									santa++;
									break;
							}
							num11++;
						}
					if (WorldModify.spawnNPC == 0)
					{
						int num12 = 0;
						bool flag2 = false;
						int num13 = 0;
						bool flag3 = false;
						bool flag4 = false;
						for (int i = 0; i < 255; i++)
							if (players[i].Active)
							{
								for (int j = 0; j < 44; j++)
									if (players[i].inventory[j] != null & players[i].inventory[j].Stack > 0)
									{
										if (players[i].inventory[j].Type == 71)
											num12 += players[i].inventory[j].Stack;
										if (players[i].inventory[j].Type == 72)
											num12 += players[i].inventory[j].Stack * 100;
										if (players[i].inventory[j].Type == 73)
											num12 += players[i].inventory[j].Stack * 10000;
										if (players[i].inventory[j].Type == 74)
											num12 += players[i].inventory[j].Stack * 1000000;
										if (players[i].inventory[j].Ammo == ProjectileType.N14_BULLET || players[i].inventory[j].UseAmmo == ProjectileType.N14_BULLET)
											flag3 = true;
										if (players[i].inventory[j].Type == 166 || players[i].inventory[j].Type == 167 || players[i].inventory[j].Type == 168 || players[i].inventory[j].Type == 235)
											flag4 = true;
									}
								int num14 = players[i].statLifeMax / 20;
								if (num14 > 5)
									flag2 = true;
								num13 += num14;
							}

						if (!NPC.downedBoss3 && num8 == 0)
						{
							int num15 = NPC.NewNPC(dungeonX * 16 + 8, dungeonY * 16, 37, 0);
							npcs[num15].homeless = false;
							npcs[num15].homeTileX = dungeonX;
							npcs[num15].homeTileY = dungeonY;
						}

						if (WorldModify.spawnNPC == 0 && num7 < 1)
							WorldModify.spawnNPC = 22;
						if (WorldModify.spawnNPC == 0 && (double)num12 > 5000.0 && num3 < 1)
							WorldModify.spawnNPC = 17;
						if (WorldModify.spawnNPC == 0 && flag2 && num4 < 1)
							WorldModify.spawnNPC = 18;
						if (WorldModify.spawnNPC == 0 && flag3 && num6 < 1)
							WorldModify.spawnNPC = 19;
						if (WorldModify.spawnNPC == 0 && (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3) && num5 < 1)
							WorldModify.spawnNPC = 20;
						if (WorldModify.spawnNPC == 0 && flag4 && num3 > 0 && num9 < 1)
							WorldModify.spawnNPC = 38;
						if (WorldModify.spawnNPC == 0 && NPC.downedBoss3 && num10 < 1)
							WorldModify.spawnNPC = 54;
						if (WorldModify.spawnNPC == 0 && NPC.savedGoblin && goblin < 1)
							WorldModify.spawnNPC = 107;
						if (WorldModify.spawnNPC == 0 && NPC.savedWizard && wizard < 1)
							WorldModify.spawnNPC = 108;
						if (WorldModify.spawnNPC == 0 && NPC.savedMech && mechanic < 1)
							WorldModify.spawnNPC = 124;
						if (WorldModify.spawnNPC == 0 && NPC.downedFrost && santa < 1 && Xmas)
							WorldModify.spawnNPC = 142;
					}
				}
			}
		}

		public static int DamageVar(float dmg)
		{
			float num = dmg * (1f + (float)rand.Next(-15, 16) * 0.01f);
			return (int)Math.Round((double)num);
		}

		public static double CalculateDamage(int Damage, int Defense)
		{
			double num = (double)Damage - (double)Defense * 0.5;
			if (num < 1.0)
				num = 1.0;
			return num;
		}

		// these be locks
		public static object updatingNPCs = new object();
		public static object updatingItems = new object();
		public static object updatingProjectiles = new object();
		public static int WallOfFlesh_B;
		public static int WallOfFlesh_T;

		public static TimeSpan LastPlayerUpdateTime { get; private set; }

		public static TimeSpan LastNPCUpdateTime { get; private set; }

		public static TimeSpan LastItemUpdateTime { get; private set; }

		public static TimeSpan LastProjectileUpdateTime { get; private set; }

		public static TimeSpan LastTimeUpdateTime { get; private set; }

		public static TimeSpan LastWorldUpdateTime { get; private set; }

		public static TimeSpan LastInvasionUpdateTime { get; private set; }

		public static TimeSpan LastServerUpdateTime { get; private set; }

		public static void Update(Stopwatch s)
		{
			int count = 0;

			int timeUpdateErrors = 0;
			int worldUpdateErrors = 0;
			int invasionUpdateErrors = 0;
			int serverUpdateErrors = 0;

			var start = s.Elapsed;
			foreach (Player player in players)
			{
				try
				{
					player.UpdatePlayer(null, null, count);
				}
				catch (Exception e)
				{
					if (!ignoreErrors)
						throw;

					ProgramLog.Log(e, String.Format("Player update error, slot={0}, address={1}, name={2}",
						player.whoAmi, player.IPAddress, player.Name != null ? string.Concat('"', player.Name, '"') : "<null>"));

					player.Kick("Server malfunction, please reconnect.");
				}
				count++;
			}
			LastPlayerUpdateTime = s.Elapsed - start;

			lock (updatingNPCs)
			{
				start = s.Elapsed;

				NPC.SpawnNPC();

				foreach (Player player in players)
				{
					player.ActiveNPCs = 0;
					player.TownNPCs = 0;
				}

				if (WallOfFlesh >= 0)
				{
					var WoF = npcs[WallOfFlesh];
					var isWoF = WoF.type == NPCType.N113_WALL_OF_FLESH || WoF.type == NPCType.N114_WALL_OF_FLESH_EYE;
					if (!isWoF && WoF.Active || !WoF.Active && isWoF)
						WallOfFlesh = -1;
				}

				for (int i = 0; i < NPC.MAX_NPCS; i++)
					//					if (npcs[i] == null)
					//					{
					//						ProgramLog.Debug.Log ("NPC[{0}] is null", i);
					//						continue;
					//					}
#if CATCHERROR_NPCUPDATES
					try
					{
#endif
					NPC.UpdateNPC(i);
#if CATCHERROR_NPCUPDATES
					}
					catch (Exception e)
					{
						if (!ignoreErrors)
							throw;

						var npc = npcs[i];
						ProgramLog.Log(e, String.Format("NPC update error, id={0}, type={1}, name={2}",
						i, npc.Type, npc.Name));

						npcs[i] = Registries.NPC.Default;
						npcs[i].netUpdate = true;
					}
#endif

				LastNPCUpdateTime = s.Elapsed - start;
			}

			lock (updatingProjectiles)
			{
				start = s.Elapsed;

				for (int i = 0; i < 1000; i++)
					//					if (projectile[i] == null)
					//					{
					//						ProgramLog.Debug.Log ("Projectile[{0}] is null", i);
					//						continue;
					//					}

					try
					{
						projectile[i].Update(Terraria_Server.Messages.TileBreakMessage.staticEditor.ITileAt, Terraria_Server.Messages.TileBreakMessage.staticEditor.Sandbox, i);
					}
					catch (Exception e)
					{
						if (!ignoreErrors)
							throw;

						var proj = projectile[i];
						ProgramLog.Log(e, String.Format("Projectile update error, i={0}, id={1}, owner={2}, type={3}",
							i, proj.identity, proj.Owner, proj.Type));
						//projectile[i] = new Projectile();
						Projectile.Reset(i);
					}

				LastProjectileUpdateTime = s.Elapsed - start;
			}

			lock (updatingItems)
			{
				start = s.Elapsed;

				for (int i = 0; i < 200; i++)
					//					if (item[i] == null)
					//					{
					//						ProgramLog.Debug.Log ("Item[{0}] is null", i);
					//						continue;
					//					}

					try
					{
						item[i].UpdateItem(null, i);
					}
					catch (Exception e)
					{
						if (!ignoreErrors)
							throw;

						var itm = item[i];
						ProgramLog.Log(e, String.Format("Projectile update error, i={0}, type={1}, owner={2}, stack={3}",
							i, itm.Type, itm.Owner, itm.Stack));
						item[i] = new Item();
					}

				LastItemUpdateTime = s.Elapsed - start;
			}

			start = s.Elapsed;
			try
			{
				UpdateTime();
				timeUpdateErrors = 0;
			}
			catch (Exception e)
			{
				if (++timeUpdateErrors >= 5 || !ignoreErrors)
					throw;

				ProgramLog.Log(e, "Time update error");
				checkForSpawns = 0;
			}
			LastTimeUpdateTime = s.Elapsed - start;

			start = s.Elapsed;
			try
			{
				WorldModify.UpdateWorld(null, null, World.Sender);
				worldUpdateErrors = 0;
			}
			catch (Exception e)
			{
				if (++worldUpdateErrors >= 5 || !ignoreErrors)
					throw;

				ProgramLog.Log(e, "World update error");
			}
			LastWorldUpdateTime = s.Elapsed - start;

			start = s.Elapsed;
			try
			{
				UpdateInvasion();
				invasionUpdateErrors = 0;
			}
			catch (Exception e)
			{
				if (++invasionUpdateErrors >= 5 || !ignoreErrors)
					throw;

				ProgramLog.Log(e, "Invasion update error");
			}
			LastInvasionUpdateTime = s.Elapsed - start;

			start = s.Elapsed;
			try
			{
				UpdateServer();
				serverUpdateErrors = 0;
			}
			catch (Exception e)
			{
				if (++serverUpdateErrors >= 5 || !ignoreErrors)
					throw;

				ProgramLog.Log(e, "Server update error");
			}
			LastServerUpdateTime = s.Elapsed - start;
		}

		public static int WallOfFlesh { get; set; }

		// [TODO] 1.1
		public static bool hardMode { get; set; }

		public static void checkXmas()
		{
			DateTime now = DateTime.Now;
			Xmas = now.Day >= 15 && now.Month == 12;
		}
	}
}
