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
	public class Main
	{
        //public static bool npcChatRelease = false;
        //public static bool npcChatFocus1 = false;
        //public static bool npcChatFocus2 = false;
		public const Int32 MAX_TILE_SETS = 145;
		public const Int32 MAX_WALL_SETS = 29;
		public const Int32 MAX_BUFFS = 40;
		public const Int32 MAX_NAMES = 142;
        
        public static Item trashItem { get; set; }
        public static bool[] debuff = new bool[MAX_BUFFS];
        public const int maxItemText = 100;
        public const int MAX_PLAYERS = 255;
        public const int maxChests = 1000;
        public const int maxItemTypes = 586;
        public const int maxProjectileTypes = 109;
        public const int maxProjectiles = 1000;
        public const int maxNPCTypes = 74;
        public const double dayLength = 54000.0;
        public static bool stopSpawns = false;
		public static bool ignoreErrors = true;
		public static bool webProtect = false;
		//private static bool webAuth = false;
		public static float leftWorld = 0f;
		public static float rightWorld = 134400f;
		public static float topWorld = 0f;
		public static float bottomWorld = 38400f;
		public static int maxTilesX = -1; //(int)Main.rightWorld / 16 + 1;
		public static int maxTilesY = -1; //(int)Main.bottomWorld / 16 + 1;
		public static int maxSectionsX = Main.maxTilesX / 200;
		public static int maxSectionsY = Main.maxTilesY / 150;
		//[Obsolete("Replaced by SlotManager.MaxSlots")]
		//public static int maxNetplayers = 254;
		public static int dungeonX;
		public static int dungeonY;
		public static Liquid[] liquid = new Liquid[Liquid.resLiquid];
		public static LiquidBuffer[] liquidBuffer = new LiquidBuffer[10000];
		public static bool dedServ = true;
        public static string worldName = "";
		public static int worldID;
		public static double worldSurface;
		public static double rockLayer;
        public static Color[] teamColor = new Color[5];
        public static Color tileColor;
		public static bool dayTime = true;
		public static double time = 13500.0;
		public static int moonPhase = 0;
		public static bool bloodMoon = false;
		public static int checkForSpawns = 0;
		public static int helpText = 0;
        public static int evilTiles;

        // [TODO] 1.1
        public static float harpNote = 0f;

		public static bool[] tileMergeDirt = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileCut = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileAlch = new bool[Main.MAX_TILE_SETS];
		public static int[] tileShine = new int[Main.MAX_TILE_SETS];
		public static bool[] wallHouse = new bool[Main.MAX_WALL_SETS];
		public static bool[] tileStone = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileWaterDeath = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileLavaDeath = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileTable = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileBlockLight = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileDungeon = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileSolidTop = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileSolid = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileNoAttach = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileNoFail = new bool[Main.MAX_TILE_SETS];
		public static bool[] tileFrameImportant = new bool[Main.MAX_TILE_SETS];

		public static string[] chrName = new string[MAX_NAMES];

		[ThreadStatic]
		static Random threadRand;
		
		public static Random rand
		{
			get
			{
				if (threadRand == null) threadRand = new Random ((int)DateTime.Now.Ticks);
				return threadRand;
			}			
			set
			{
			}
		}
		
        public static TileCollection tile;
		public static Item[] item = new Item[201];
		public static NPC[] npcs = new NPC[NPC.MAX_NPCS + 1];
		public static Projectile[] projectile = new Projectile[1001];
		public static Chest[] chest = new Chest[1000];
		public static Sign[] sign = new Sign[1000];
		public static Vector2 screenPosition;
		public static Vector2 screenLastPosition;
		public static int screenWidth = 800;
		public static int screenHeight = 600;
		public static bool playerInventory = false;
		public const int myPlayer = 255;
		public static Player[] players = new Player[MAX_PLAYERS+1];
		public static int spawnTileX;
		public static int spawnTileY;
		public static bool editSign = false;
        public static string signText = "";
        public static string npcChatText = "";

		public static int invasionType = 0;
		public static double invasionX = 0.0;
		public static int invasionSize = 0;
		public static int invasionDelay = 0;
		public static int invasionWarn = 0;
        public static int[] npcFrameCount = new int[]
		{
			1, 2, 2, 3, 6, 2, 2, 1, 1 ,1, 
			1, 1, 1, 1, 1, 1, 2, 16, 14, 16, 
			14, 15, 16, 2, 10, 1, 16, 16, 16, 3, 
			1, 15, 3, 1, 3, 1, 1, 16, 16, 1, 
			1, 1, 3, 3, 15, 3, 7, 7, 4, 5, 
			5, 5, 3, 3, 16, 6, 3, 6, 6, 2, 5, 
			3, 2, 7, 7, 4, 2, 8, 1, 5, 1, 
			2, 4, 16
		};
		public static int timeOut = 120;
		public static int NetplayCounter;
		public static int lastItemUpdate;
		public static int maxNPCUpdates = 15;
		public static int maxItemUpdates = 10;
		public static bool autoPass = false;
		
        public static void Initialize()
		{
            //if (Main.webProtect)
            //{
            //    getAuth();
            //    while (!Main.webAuth)
            //    {
            //        Statics.IsActive = false;
            //    }
            //}

            Main.stopSpawns = Program.properties.StopNPCSpawning;

            Main.tileShine[6] = 1150;
            Main.tileShine[7] = 1100;
            Main.tileShine[8] = 1000;
            Main.tileShine[9] = 1050;
            Main.tileShine[12] = 1000;
            Main.tileShine[21] = 1000;
            Main.tileShine[22] = 1150;
            Main.tileShine[45] = 1900;
            Main.tileShine[46] = 2000;
            Main.tileShine[47] = 2100;
            Main.tileShine[63] = 900;
            Main.tileShine[64] = 900;
            Main.tileShine[65] = 900;
            Main.tileShine[66] = 900;
            Main.tileShine[67] = 900;
            Main.tileShine[68] = 900;

            foreach (int i in new int[] { 20, 21, 22, 23, 24, 25 })
            {
                Main.debuff[i] = true;
            }

            foreach (int i in new int[] { 3, 24, 28, 32, 51, 52, 61, 62, 69, 71, 73, 74, 82, 83, 84 })
            {
                Main.tileCut[i] = true;
            }

            foreach (int i in new int[] { 82, 83, 84 })
            {
                Main.tileAlch[i] = true;
            }

            foreach (int i in new int[] { 3, 5, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20, 21, 24, 26, 27, 28, 29, 
                31, 33, 34, 35, 36, 42, 50, 55, 61, 71, 72, 73, 74, 77, 78, 79, 81, 82, 83, 84, 85, 86, 87, 88,
                89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106 })
            {
                Main.tileFrameImportant[i] = true;
            }

            foreach (int i in new int[] { 3, 5, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 27, 28, 29, 32, 33,
                34, 35, 36, 42, 49, 50, 52, 55, 61, 62, 69, 71, 72, 73, 74, 79, 80, 81, 86, 87, 88, 89, 91, 92,
                93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106 })
            {
                Main.tileLavaDeath[i] = true;
            }

            foreach (int i in new int[] { 0, 1, 2, 6, 7, 8, 9, 10, 22, 23, 25, 30, 32, 37, 38, 39, 40, 41, 43, 44, 
                45, 46, 47, 48, 51, 52, 53, 56, 57, 58, 59, 60, 62, 63, 64, 65, 66, 67, 68, 70, 75, 76 })
            {
                Main.tileBlockLight[i] = true;
            }

            foreach (int i in new int[] { 14, 16, 18, 19, 87, 88, 101 })
            {
                Main.tileSolidTop[i] = true;
            }

            foreach (int i in new int[] { 3, 4, 10, 13, 14, 15, 16, 17, 18, 19, 20, 21, 27, 50, 86, 87, 88, 89,
                90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 101, 102 })
            {
                Main.tileNoAttach[i] = true;
            }

            foreach (int i in new int[] { 14, 18, 19, 87, 88, 101 })
            {
                Main.tileTable[i] = true;
            }

            foreach (int i in new int[] { 4, 51, 93, 98 })
            {
                Main.tileWaterDeath[i] = true;
            }

            foreach (int i in new int[] { 1, 4, 5, 6, 10, 11, 12, 16, 17, 18, 19, 20 })
            {
                Main.wallHouse[i] = true;
            }

            foreach (int i in new int[] { 3, 4, 24, 32, 50, 61, 69, 73, 74, 82, 83, 84 })
            {
                Main.tileNoFail[i] = true;
            }

            foreach (int i in new int[] {0, 1, 2, 6, 7, 8, 9, 10, 19, 22, 23, 25, 30, 37, 38, 39, 40, 41, 43, 44, 45, 46,
                47, 48, 53, 54, 56, 57, 58, 59, 60, 63, 64, 65, 66, 67, 68, 70, 75, 76})
            {
                Main.tileSolid[i] = true;
            }

            foreach (int i in new int[] { 3, 4, 5, 11 })
            {
                Main.tileSolid[i] = false;
            }

            foreach (int i in new int[] { 63, 64, 65, 66, 67, 68 })
            {
                Main.tileStone[i] = false;
            }

            foreach (int i in new int[] { 41, 43, 44 })
            {
                Main.tileDungeon[i] = true;
            }
            for (int l = 0; l < 201; l++)
            {
                Main.item[l] = new Item();
            }
            for (int m = 0; m < NPC.MAX_NPCS + 1; m++)
            {
                Main.npcs[m] = new NPC();
                Main.npcs[m].whoAmI = m;
            }
            for (int i = 0; i < MAX_PLAYERS+1; i++)
            {
                Main.players[i] = new Player();
            }
            for (int num2 = 0; num2 < 1001; num2++)
            {
                Main.projectile[num2] = new Projectile();
            }
            for (int num10 = 0; num10 < Liquid.resLiquid; num10++)
            {
                Main.liquid[num10] = new Liquid();
            }
            for (int num11 = 0; num11 < 10000; num11++)
            {
                Main.liquidBuffer[num11] = new LiquidBuffer();
            }

            Main.teamColor[0] = new Color(255, 255, 255);
            Main.teamColor[1] = new Color(230, 40, 20);
            Main.teamColor[2] = new Color(20, 200, 30);
            Main.teamColor[3] = new Color(75, 90, 255);
			Main.teamColor[4] = new Color(200, 180, 0);

			NPC.SetNames();

			NetPlay.Init();
		}

        private static void UpdateInvasion()
		{
			if (Main.invasionType > 0)
			{
				if (Main.invasionSize <= 0)
				{
					Main.InvasionWarning();
					Main.invasionType = 0;
					Main.invasionDelay = 7;
				}
				if (Main.invasionX == (double)Main.spawnTileX)
				{
					return;
				}
				float num = 0.2f;
				if (Main.invasionX > (double)Main.spawnTileX)
				{
					Main.invasionX -= (double)num;
					if (Main.invasionX <= (double)Main.spawnTileX)
					{
						Main.invasionX = (double)Main.spawnTileX;
						Main.InvasionWarning();
					}
					else
					{
						Main.invasionWarn--;
					}
				}
				else
				{
					if (Main.invasionX < (double)Main.spawnTileX)
					{
						Main.invasionX += (double)num;
						if (Main.invasionX >= (double)Main.spawnTileX)
						{
							Main.invasionX = (double)Main.spawnTileX;
							Main.InvasionWarning();
						}
						else
						{
							Main.invasionWarn--;
						}
					}
				}
				if (Main.invasionWarn <= 0)
				{
					Main.invasionWarn = 3600;
					Main.InvasionWarning();
				}
			}
		}
		
        private static void InvasionWarning()
		{
			if (Main.invasionType == 0)
			{
				return;
			}
            string text = "";
			if (Main.invasionSize <= 0)
			{
				text = "The goblin army has been defeated!";
			}
			else
			{
				if (Main.invasionX < (double)Main.spawnTileX)
				{
					text = "A goblin army is approaching from the west!";
				}
				else
				{
					if (Main.invasionX > (double)Main.spawnTileX)
					{
						text = "A goblin army is approaching from the east!";
					}
					else
					{
						text = "The goblin army has arrived!";
					}
				}
			}
            
            NetMessage.SendData(25, -1, -1, text, 255, 175f, 75f, 255f);
		}
		
        public static void StartInvasion()
		{
			if (!WorldModify.shadowOrbSmashed)
			{
				return;
			}

			if (Main.invasionType == 0 && Main.invasionDelay == 0)
			{
				int playerCount = 0;
				foreach(Player player in Main.players)
				{
                    if (player.Active && player.statLifeMax >= 200)
					{
						playerCount++;
					}
				}

				if (playerCount > 0)
				{
					Main.invasionType = 1;
					Main.invasionSize = 100 + 50 * playerCount;
					Main.invasionWarn = 0;
					if (Main.rand.Next(2) == 0)
					{
						Main.invasionX = 0.0;
						return;
					}
					Main.invasionX = (double)Main.maxTilesX;
				}
			}
		}

        private static void UpdateServer()
        {
            Main.NetplayCounter++;
            if (Main.NetplayCounter > 3600)
            {
                NetMessage.SendData(7);
                //NetMessage.SyncPlayers();
                Main.NetplayCounter = 0;
            }
			if (Main.NetplayCounter % 30 == 0)
			{
				for (int i = 0; i < 255; i++)
				{
					var player = Main.players[i];
					var rows = player.rowsToRectify;
					if (player.Active && rows.Count > 0)
					{
						bool locked = false;
						try
						{
							locked = Monitor.TryEnter (rows);
							if (!locked)
							{
								//ProgramLog.Debug.Log ("not acquired!");
								continue;
							}
							
							var conn = player.Connection;
							if (conn == null) continue;
							
							int Y1 = int.MaxValue;
							int Y2 = 0;
							int X1 = int.MaxValue;
							int X2 = 0;
							
							var msg = NetMessage.PrepareThreadInstance ();
							bool warn = false;
							
							if (rows.Count <= 150)
							{
								foreach (var kv in rows)
								{
									var y = kv.Key;
									var x1 = (int) (kv.Value >> 16);
									var x2 = (int) (kv.Value & 0xffff);
									
									if (x1 > x2) continue;
									
									var len = x2 - x1 + 1;
									
									if (len > 200)
									{
										warn = true;
										break;
									}
									
									if (y < Y1) Y1 = y;
									if (y > Y2) Y2 = y;
									if (x1 < X1) X1 = x1;
									if (x2 > X2) X2 = x2;
									
									if (conn.CompressionVersion == 1)
										msg.TileRowCompressed (len, x1, y);
									else
										msg.SendTileRow (len, x1, y);
								}
							}
							else
								warn = true;
							
							rows.Clear ();
							
							if (warn)
							{
								msg.Clear ();
								msg.PlayerChat (255, "<Server> Your view of the map is out of sync.", 255, 128, 128);
								msg.Send (conn);
							}
							else if (msg.Written > 0)
							{
								msg.SendTileConfirm (X1 / 200, Y1 / 150, X2 / 200, Y2 / 150);
								msg.Send (conn);
							}
						}
						finally
						{
							if (locked) Monitor.Exit (rows);
						}
					}
				}
			}
            for (int i = 0; i < 255; i++)
            {
                if (Main.players[i].Active && NetPlay.slots[i].state >= SlotState.CONNECTED)
                {
                    NetPlay.slots[i].SpamUpdate();
                }
            }
            Math.IEEERemainder((double)Main.NetplayCounter, 60.0);
            if (Math.IEEERemainder((double)Main.NetplayCounter, 360.0) == 0.0)
            {
                bool flag2 = true;
                int num = Main.lastItemUpdate;
                int num2 = 0;
                while (flag2)
                {
                    num++;
                    if (num >= 200)
                    {
                        num = 0;
                    }
                    num2++;
                    if (!Main.item[num].Active || Main.item[num].Owner == 255)
                    {
                        NetMessage.SendData(21, -1, -1, "", num);
                    }
                    if (num2 >= Main.maxItemUpdates || num == Main.lastItemUpdate)
                    {
                        flag2 = false;
                    }
                }
                Main.lastItemUpdate = num;
            }

            for (int i = 0; i < 200; i++)
            {
                if (Main.item[i].Active && (Main.item[i].Owner == 255 || !Main.players[Main.item[i].Owner].Active))
                {
                    Main.item[i].FindOwner(i);
                }
            }

            for (int i = 0; i < 255; i++)
            {
//                if (Netplay.slots[i].state >= SlotState.CONNECTED)
//                {
//                    //Netplay.slots[i].timeOut++;
//                    if (/*!Main.stopTimeOuts && */Netplay.slots[i].timeOut > 60 * Main.timeOut)
//                    {
//                        Netplay.slots[i].Kick ("Timed out.");
//                    }
//                }

                Player player = Main.players[i];
                if (player.Active)
                {
                    int sectionX = NetPlay.GetSectionX((int)(player.Position.X / 16f));
                    int sectionY = NetPlay.GetSectionY((int)(player.Position.Y / 16f));
                    int num3 = 0;
                    for (int j = sectionX - 1; j < sectionX + 2; j++)
                    {
                        for (int k = sectionY - 1; k < sectionY + 2; k++)
                        {
                            if (j >= 0 && j < Main.maxSectionsX && k >= 0 && k < Main.maxSectionsY)
                            {
                                if (!NetPlay.slots[i].tileSection[j, k])
                                {
                                    num3++;
                                }
                            }
                        }
                    }

                    if (num3 > 0)
                    {
                        int num4 = num3 * 150;
                        NetMessage.SendData(9, i, -1, "Receiving tile data", num4);
                        NetPlay.slots[i].statusText2 = "is receiving tile data";
                        NetPlay.slots[i].statusMax += num4;
                        for (int j = sectionX - 1; j < sectionX + 2; j++)
                        {
                            for (int k = sectionY - 1; k < sectionY + 2; k++)
                            {
                                if (j >= 0 && j < Main.maxSectionsX && k >= 0 && k < Main.maxSectionsY)
                                {
                                    if (!NetPlay.slots[i].tileSection[j, k])
                                    {
                                        NetMessage.SendSection(i, j, k);
                                        NetMessage.SendData(11, i, -1, "", j, (float)k, (float)j, (float)k);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var conn = player.Connection;
                        if (conn != null) conn.Flush ();
                    }
                }
                else
                {
	                var conn = player.Connection;
	                if (conn != null) conn.Flush ();
                }
            }
        }

        //static TimeChangedEvent timeEvent = new TimeChangedEvent();
        private static void UpdateTime()
        {
            Main.time += 1.0;
            
            //Server.PluginManager.processHook(Hooks.TIME_CHANGED, timeEvent);

            if (!Main.dayTime)
            {
                if (WorldModify.spawnEye)
                {
                    if (Main.time > 4860.0)
                    {
                        int count = 0;
                        foreach(Player player in Main.players)
                        {
                            if (player.Active && !player.dead && (double)player.Position.Y < Main.worldSurface * 16.0)
                            {
                                NPC.SpawnOnPlayer(player, count, 4);
                                WorldModify.spawnEye = false;
                                break;
                            }
                            count++;
                        }
                    }
                }

                if (Main.time > 32400.0)
                {
                    if (Main.invasionDelay > 0)
                    {
                        Main.invasionDelay--;
                    }
                    WorldModify.spawnNPC = 0;
                    Main.checkForSpawns = 0;
                    Main.time = 0.0;
                    Main.bloodMoon = false;
                    Main.dayTime = true;
                    Main.moonPhase++;
                    if (Main.moonPhase >= 8)
                    {
                        Main.moonPhase = 0;
                    }
                    
                    NetMessage.SendData(7);
                    WorldIO.SaveWorldThreaded();

                    
                    if (Main.rand.Next(15) == 0)
                    {
                        Main.StartInvasion();
                    }
                }
                if (Main.time > 16200.0 && WorldModify.spawnMeteor)
                {
                    WorldModify.spawnMeteor = false;
                    WorldModify.dropMeteor();
                }
            }
            else
            {
                if (Main.time > 54000.0)
                {
                    WorldModify.spawnNPC = 0;
                    Main.checkForSpawns = 0;
                    if (Main.rand.Next(50) == 0 && WorldModify.shadowOrbSmashed)
                    {
                        WorldModify.spawnMeteor = true;
                    }
                    if (!NPC.downedBoss1)
                    {
                        bool flag = false;
                        foreach(Player player in Main.players)
                        {
                            if (player.Active && player.statLifeMax >= 200)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag && Main.rand.Next(3) == 0)
                        {
                            int num = 0;
                            for (int i = 0; i < NPC.MAX_NPCS; i++)
                            {
                                if (Main.npcs[i].Active)
                                {
                                    if (Main.npcs[i].townNPC)
                                    {
                                        num++;
                                    }
                                }
                            }
                            if (num >= 4)
                            {
                                WorldModify.spawnEye = true;
                                
                                NetMessage.SendData(25, -1, -1, "You feel an evil presence watching you...", 255, 50f, 255f, 130f);
                            }
                        }
                    }
                    if (!WorldModify.spawnEye && Main.moonPhase != 4 && Main.rand.Next(7) == 0)
                    {
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.players[i].Active && Main.players[i].statLifeMax > 100)
                            {
                                Main.bloodMoon = true;
                                break;
                            }
                        }
                        if (Main.bloodMoon)
                        {
                            NetMessage.SendData(25, -1, -1, "The Blood Moon is rising...", 255, 50f, 255f, 130f);
                        }
                    }
                    Main.time = 0.0;
                    Main.dayTime = false;
                    
                    NetMessage.SendData(7);
                }
                Main.checkForSpawns++;
                if (Main.checkForSpawns >= 7200)
                {
                    int num2 = 0;
                    for (int i = 0; i < 255; i++)
                    {
                        if (Main.players[i].Active)
                        {
                            num2++;
                        }
                    }
                    Main.checkForSpawns = 0;
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
                    for (int i = 0; i < NPC.MAX_NPCS; i++)
                    {
                        if (Main.npcs[i].Active && Main.npcs[i].townNPC)
                        {
                            if (Main.npcs[i].type != NPCType.N37_OLD_MAN && !Main.npcs[i].homeless)
                            {
                                WorldModify.QuickFindHome(i);
                            }
                            else
                            {
                                num8++;
                            }
                            switch (Main.npcs[i].type)
                            {
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
                            }
                            num11++;
                        }
                    }
                    if (WorldModify.spawnNPC == 0)
                    {
                        int num12 = 0;
                        bool flag2 = false;
                        int num13 = 0;
                        bool flag3 = false;
                        bool flag4 = false;
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.players[i].Active)
                            {
                                for (int j = 0; j < 44; j++)
                                {
                                    if (Main.players[i].inventory[j] != null & Main.players[i].inventory[j].Stack > 0)
                                    {
                                        if (Main.players[i].inventory[j].Type == 71)
                                        {
                                            num12 += Main.players[i].inventory[j].Stack;
                                        }
                                        if (Main.players[i].inventory[j].Type == 72)
                                        {
                                            num12 += Main.players[i].inventory[j].Stack * 100;
                                        }
                                        if (Main.players[i].inventory[j].Type == 73)
                                        {
                                            num12 += Main.players[i].inventory[j].Stack * 10000;
                                        }
                                        if (Main.players[i].inventory[j].Type == 74)
                                        {
                                            num12 += Main.players[i].inventory[j].Stack * 1000000;
                                        }
                                        if (Main.players[i].inventory[j].Ammo == ProjectileType.N14_BULLET || Main.players[i].inventory[j].UseAmmo == ProjectileType.N14_BULLET)
                                        {
                                            flag3 = true;
                                        }
                                        if (Main.players[i].inventory[j].Type == 166 || Main.players[i].inventory[j].Type == 167 || Main.players[i].inventory[j].Type == 168 || Main.players[i].inventory[j].Type == 235)
                                        {
                                            flag4 = true;
                                        }
                                    }
                                }
                                int num14 = Main.players[i].statLifeMax / 20;
                                if (num14 > 5)
                                {
                                    flag2 = true;
                                }
                                num13 += num14;
                            }
                        }
                        if (num7 < 1)
                        {
                            WorldModify.spawnNPC = 22;
                        }
                        if ((double)num12 > 5000.0 && num3 < 1)
                        {
                            WorldModify.spawnNPC = 17;
                        }
                        if (flag2 && num4 < 1)
                        {
                            WorldModify.spawnNPC = 18;
                        }
                        if (flag3 && num6 < 1)
                        {
                            WorldModify.spawnNPC = 19;
                        }
                        if ((NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3) && num5 < 1)
                        {
                            WorldModify.spawnNPC = 20;
                        }
                        if (flag4 && num3 > 0 && num9 < 1)
                        {
                            WorldModify.spawnNPC = 38;
                        }
                        if (NPC.downedBoss3 && num10 < 1)
                        {
                            WorldModify.spawnNPC = 54;
                        }
                        if (num12 > 100000 && num3 < 2 && num2 > 2)
                        {
                            WorldModify.spawnNPC = 17;
                        }
                        if (num13 >= 20 && num4 < 2 && num2 > 2)
                        {
                            WorldModify.spawnNPC = 18;
                        }
                        if (num12 > 5000000 && num3 < 3 && num2 > 4)
                        {
                            WorldModify.spawnNPC = 17;
                        }
                        if (!NPC.downedBoss3 && num8 == 0)
                        {
                            int num15 = NPC.NewNPC(Main.dungeonX * 16 + 8, Main.dungeonY * 16, 37, 0);
                            Main.npcs[num15].homeless = false;
                            Main.npcs[num15].homeTileX = Main.dungeonX;
                            Main.npcs[num15].homeTileY = Main.dungeonY;
                        }
                    }
                }
            }
        }
		
		public static int DamageVar (float dmg)
		{
			float num = dmg * (1f + (float)Main.rand.Next(-15, 16) * 0.01f);
			return (int)Math.Round((double)num);
		}
		
        public static double CalculateDamage(int Damage, int Defense)
		{
			double num = (double)Damage - (double)Defense * 0.5;
			if (num < 1.0)
			{
				num = 1.0;
			}
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
		
		public static void Update (Stopwatch s)
		{
			int count = 0;
			
			int timeUpdateErrors = 0;
			int worldUpdateErrors = 0;
			int invasionUpdateErrors = 0;
			int serverUpdateErrors = 0;
			
			var start = s.Elapsed;
			foreach(Player player in Main.players)
			{
				try
				{
					player.UpdatePlayer(count);
				}
				catch (Exception e)
				{
					if (! Main.ignoreErrors) throw;
					
					ProgramLog.Log (e, String.Format ("Player update error, slot={0}, address={1}, name={2}",
						player.whoAmi, player.IPAddress, player.Name != null ? string.Concat ('"', player.Name, '"') : "<null>"));
					
					player.Kick ("Server malfunction, please reconnect.");
				}
				count++;
			}
			LastPlayerUpdateTime = s.Elapsed - start;
			
			lock (updatingNPCs)
			{
				start = s.Elapsed;
				
				NPC.SpawnNPC();
				
				foreach (Player player in Main.players)
				{
					player.activeNPCs = 0;
					player.townNPCs = 0;
                }

                if (Main.WallOfFlesh >= 0 && !Main.npcs[Main.WallOfFlesh].Active)
                    Main.WallOfFlesh = -1;
				
				for (int i = 0; i < NPC.MAX_NPCS; i++)
				{
//					if (Main.npcs[i] == null)
//					{
//						ProgramLog.Debug.Log ("NPC[{0}] is null", i);
//						continue;
//					}
					
					try
					{
						NPC.UpdateNPC(i);
					}
					catch (Exception e)
					{
						if (! Main.ignoreErrors) throw;
						
						var npc = Main.npcs[i];
						ProgramLog.Log (e, String.Format ("NPC update error, id={0}, type={1}, name={2}",
						i, npc.Type, npc.Name));
						
						Main.npcs[i] = Registries.NPC.Default;
						Main.npcs[i].netUpdate = true;
					}
				}
				
				LastNPCUpdateTime = s.Elapsed - start;
			}
			
			lock (updatingProjectiles)
			{
				start = s.Elapsed;
				
				for (int i = 0; i < 1000; i++)
				{
//					if (Main.projectile[i] == null)
//					{
//						ProgramLog.Debug.Log ("Projectile[{0}] is null", i);
//						continue;
//					}
					
					try
					{
					    Main.projectile[i].Update(i);
					}
					catch (Exception e)
					{
						if (! Main.ignoreErrors) throw;
						
						var proj = Main.projectile[i];
						ProgramLog.Log (e, String.Format ("Projectile update error, i={0}, id={1}, owner={2}, type={3}",
							i, proj.identity, proj.Owner, proj.Type));
						//Main.projectile[i] = new Projectile();
						Projectile.Reset (i);
					}
				}
				
				LastProjectileUpdateTime = s.Elapsed - start;
			}
			
			lock (updatingItems)
			{
				start = s.Elapsed;
				
				for (int i = 0; i < 200; i++)
				{
//					if (Main.item[i] == null)
//					{
//						ProgramLog.Debug.Log ("Item[{0}] is null", i);
//						continue;
//					}
					
					try
					{
						Main.item[i].UpdateItem(i);
					}
					catch (Exception e)
					{
						if (! Main.ignoreErrors) throw;
						
						var item = Main.item[i];
						ProgramLog.Log (e, String.Format ("Projectile update error, i={0}, type={1}, owner={2}, stack={3}",
							i, item.Type, item.Owner, item.Stack));
						Main.item[i] = new Item();
					}
				}
				
				LastItemUpdateTime = s.Elapsed - start;
			}
			
			start = s.Elapsed;
			try
			{
				Main.UpdateTime ();
				timeUpdateErrors = 0;
			}
			catch (Exception e)
			{
				if (++timeUpdateErrors >= 5 || ! Main.ignoreErrors) throw;
				
				ProgramLog.Log (e, "Time update error");
				Main.checkForSpawns = 0;
			}
			LastTimeUpdateTime = s.Elapsed - start;
			
			start = s.Elapsed;
			try
			{
				WorldModify.UpdateWorld ();
				worldUpdateErrors = 0;
			}
			catch (Exception e)
			{
				if (++worldUpdateErrors >= 5 || ! Main.ignoreErrors) throw;
				
				ProgramLog.Log (e, "World update error");
			}
			LastWorldUpdateTime = s.Elapsed - start;

			start = s.Elapsed;
			try
			{
				Main.UpdateInvasion ();
				invasionUpdateErrors = 0;
			}
			catch (Exception e)
			{
				if (++invasionUpdateErrors >= 5 || ! Main.ignoreErrors) throw;
				
				ProgramLog.Log (e, "Invasion update error");
			}
			LastInvasionUpdateTime = s.Elapsed - start;
			
			start = s.Elapsed;
			try
			{
				Main.UpdateServer ();
				serverUpdateErrors = 0;
			}
			catch (Exception e)
			{
				if (++serverUpdateErrors >= 5 || ! Main.ignoreErrors) throw;
				
				ProgramLog.Log (e, "Server update error");
			}
			LastServerUpdateTime = s.Elapsed - start;
		}

        public static int WallOfFlesh { get; set; }

        // [TODO] 1.1
        public static bool hardMode { get; set; }
    }
}
