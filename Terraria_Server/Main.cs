
using System.Diagnostics;
using System;
using System.Text;
using System.Net;
using System.IO;
using Terraria_Server.Misc;
using Terraria_Server.Shops;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
namespace Terraria_Server
{
	public class Main
	{
        public static int alwaysSpawn = 0;
        public static bool autoGen = false;
		private const int MF_BYPOSITION = 1024;
		public const int sectionWidth = 200;
		public const int sectionHeight = 150;
        public static int numDust = 1000;
        public const int maxTileSets = 86;
        public const int maxWallTypes = 14;
        public const int maxBackgrounds = 7;
        public const int maxDust = 1000;
        public const int maxCombatText = 100;
        public const int maxItemText = 100;
        public const int MAX_PLAYERS = 255;
        public const int maxChests = 1000;
        public const int maxItemTypes = 327;
        public const int maxItems = 200;
        public const int maxBuffs = 19;
        public const int maxProjectileTypes = 55;
        public const int maxProjectiles = 1000;
        public const int maxNPCTypes = 70;
        public const int maxGoreTypes = 99;
        public const int maxGore = 200;
        public const int maxInventory = 44;
        public const int maxItemSounds = 16;
        public const int maxNPCHitSounds = 3;
        public const int maxNPCKilledSounds = 4;
        public const int maxLiquidTypes = 2;
        public const int maxMusic = 8;
        public const int numArmorHead = 29;
        public const int numArmorBody = 17;
        public const int numArmorLegs = 16;
        public const double dayLength = 54000.0;
        public const double nightLength = 32400.0;
        public const int maxStars = 130;
        public const int maxStarTypes = 5;
        public const int maxClouds = 100;
        public const int maxCloudTypes = 4;
        public const int maxHair = 17;

		public static bool grabSun = false;
		//public static bool debugMode = false;
		public static bool godMode = false;
        public static bool stopSpawns = false;
		public static bool dumbAI = false;
		public static bool skipMenu = false;
		public static bool lightTiles = false;
		public static bool verboseNetplay = false;
		public static bool stopTimeOuts = false;
		public static bool showSpam = false;
		public static bool showItemOwner = false;
		public static bool ignoreErrors = false;
		public static bool webProtect = false;
		public static bool showSplash = true;
        public static String defaultIP = "";
        private Process tServer = new Process();
		private static bool webAuth = false;
		public static int updateTime = 0;
		public static int drawTime = 0;
		public static int frameRate = 0;
		public static bool frameRelease = false;
		public static bool showFrameRate = false;
		public static int magmaBGFrame = 0;
		public static int magmaBGFrameCounter = 0;
		public static int saveTimer = 0;
		public static bool autoJoin = false;
		public static bool serverStarting = false;
		public static float leftWorld = 0f;
		public static float rightWorld = 134400f;
		public static float topWorld = 0f;
		public static float bottomWorld = 38400f;
		public static int maxTilesX = (int)Main.rightWorld / 16 + 1;
		public static int maxTilesY = (int)Main.bottomWorld / 16 + 1;
		public static int maxSectionsX = Main.maxTilesX / 200;
		public static int maxSectionsY = Main.maxTilesY / 150;
		public static int maxNetplayers = 255;
		public static float caveParrallax = 1f;
		public static int dungeonX;
		public static int dungeonY;
		public static Liquid[] liquid = new Liquid[Liquid.resLiquid];
		public static LiquidBuffer[] liquidBuffer = new LiquidBuffer[10000];
		public static bool dedServ = true;
		public int curMusic;
		public int newMusic;
		public static String statusText = "";
		public static String worldName = "";
		public static int worldID;
		public static int background = 0;
		public static Color tileColor;
		public static double worldSurface;
		public static double rockLayer;
		public static Color[] teamColor = new Color[5];
		public static bool dayTime = true;
		public static double time = 13500.0;
		public static int moonPhase = 0;
		public static short sunModY = 0;
		public static short moonModY = 0;
		public static bool grabSky = false;
		public static bool bloodMoon = false;
		public static int checkForSpawns = 0;
		public static int helpText = 0;
		public static int numStars;
		public static int cloudLimit = 100;
		public static int numClouds = Main.cloudLimit;
		public static float windSpeed = 0f;
		public static float windSpeedSpeed = 0f;
		public static bool resetClouds = true;
		public static int evilTiles;
		public static int meteorTiles;
		public static int jungleTiles;
		public static int dungeonTiles;
		public static int fadeCounter = 0;


        public static bool[] tileCut = new bool[86];
        public static bool[] tileAlch = new bool[86];
        public static int[] tileShine = new int[86];
        public static bool[] tileStone = new bool[86];
        public static bool[] tileWaterDeath = new bool[86];
        public static bool[] tileLavaDeath = new bool[86];
        public static bool[] tileTable = new bool[86];
        public static bool[] tileBlockLight = new bool[86];
        public static bool[] tileDungeon = new bool[86];
        public static bool[] tileSolidTop = new bool[86];
        public static bool[] tileSolid = new bool[86];
        public static bool[] tileNoAttach = new bool[86];
        public static bool[] tileNoFail = new bool[86];
        public static bool[] tileFrameImportant = new bool[86];

		[ThreadStatic]
		public static Random rand;
		public static float[] musicFade = new float[7];
		public static float musicVolume = 0.75f;
		public static float soundVolume = 1f;
		public static bool[] wallHouse = new bool[14];
		public static int[] backgroundWidth = new int[7];
		public static int[] backgroundHeight = new int[7];
        public static bool tilesLoaded = false;
        public static Tile[,] tile = new Tile[Main.maxTilesX, Main.maxTilesY];
		public static Dust[] dust = new Dust[2000];
		public static Item[] item = new Item[201];
		public static NPC[] npcs = new NPC[NPC.MAX_NPCS + 1];
		public static Gore[] gore = new Gore[201];
		public static Projectile[] projectile = new Projectile[1001];
		public static Chest[] chest = new Chest[1000];
		public static Sign[] sign = new Sign[1000];
		public static Vector2 screenPosition;
		public static Vector2 screenLastPosition;
		public static int screenWidth = 800;
		public static int screenHeight = 600;
		public static int chatLength = 600;
		public static bool chatMode = false;
		public static bool chatRelease = false;
		public static int numChatLines = 7;
		public static String chatText = "";
		public static bool inputTextEnter = false;
		public static float[] hotbarScale = new float[]
		{
			1f, 
			0.75f, 
			0.75f, 
			0.75f, 
			0.75f, 
			0.75f, 
			0.75f, 
			0.75f, 
			0.75f, 
			0.75f
		};
		public static byte mouseTextColor = 0;
		public static int mouseTextColorChange = 1;
		public static bool mouseLeftRelease = false;
		public static bool mouseRightRelease = false;
		public static bool playerInventory = false;
		public static int stackSplit;
		public static int stackCounter = 0;
		public static int stackDelay = 7;
		public static Item mouseItem = new Item();
		public static bool hasFocus = true;
		public static Recipe[] recipe = new Recipe[Recipe.maxRecipes];
		public static int[] availableRecipe = new int[Recipe.maxRecipes];
		public static float[] availableRecipeY = new float[Recipe.maxRecipes];
		public static int numAvailableRecipes;
		public static int focusRecipe;
		public static int myPlayer = 0;
		public static Player[] players = new Player[MAX_PLAYERS+1];
		public static int spawnTileX;
		public static int spawnTileY;
		public static bool npcChatRelease = false;
		public static bool editSign = false;
		public static String signText = "";
		public static String npcChatText = "";
		public static bool npcChatFocus1 = false;
		public static bool npcChatFocus2 = false;
		public static int npcShop = 0;
		public Chest[] shops = new Chest[6];
		private static Item toolTip = new Item();
		public static String motd = "";
		public bool toggleFullscreen;
		private int[] displayWidth = new int[99];
		private int[] displayHeight = new int[99];
		public static bool gameMenu = true;
		public static Player[] loadPlayer = new Player[5];
		public static String[] loadPlayerPath = new String[5];
		public static String playerPathName;
		public static String[] loadWorld = new String[999];
		public static String[] loadWorldPath = new String[999];
		public static String worldPathName;
		public static int invasionType = 0;
		public static double invasionX = 0.0;
		public static int invasionSize = 0;
		public static int invasionDelay = 0;
		public static int invasionWarn = 0;
        public static int[] npcFrameCount = new int[]
		{
			1, 
			2, 
			2, 
			3, 
			6, 
			2, 
			2, 
			1, 
			1, 
			1, 
			1, 
			1, 
			1, 
			1, 
			1, 
			1, 
			2, 
			16, 
			14, 
			16, 
			14, 
			15, 
			16, 
			2, 
			10, 
			1, 
			16, 
			16, 
			16, 
			3, 
			1, 
			15, 
			3, 
			1, 
			3, 
			1, 
			1, 
			16, 
			16, 
			1, 
			1, 
			1, 
			3, 
			3, 
			15, 
			3, 
			7, 
			7, 
			4, 
			5, 
			5, 
			5, 
			3, 
			3, 
			16, 
			6, 
			3, 
			6, 
			6, 
			2, 
			5, 
			3, 
			2, 
			7, 
			7, 
			4, 
			2, 
			8, 
			1, 
			5
		};
		public static Player clientPlayer = new Player();
		public static String getIP = Main.defaultIP;
		public static String getPort = Convert.ToString(Netplay.serverPort);
		public static bool menuMultiplayer = false;
		public static bool menuServer = false;
		public static int timeOut = 120;
		public static int NetplayCounter;
		public static int lastNPCUpdate;
		public static int lastItemUpdate;
		public static int maxNPCUpdates = 15;
		public static int maxItemUpdates = 10;
		public static String cUp = "W";
		public static String cLeft = "A";
		public static String cDown = "S";
		public static String cRight = "D";
		public static String cJump = "Space";
		public static String cThrowItem = "Q";
		public static String cInv = "Escape";
		public static Color mouseColor = new Color(255, 50, 95);
        public static Color cursorColor = new Color(255, 255, 255);
		public static int cursorColorDirection = 1;
		public static float cursorAlpha = 0f;
		public static float cursorScale = 0f;
		public static bool signBubble = false;
		public static int signX = 0;
		public static int signY = 0;
		public static bool hideUI = false;
		public static bool releaseUI = false;
		public static bool fixedTiming = false;
		public static String oldStatusText = "";
		public static bool autoShutdown = false;
		private static int maxMenuItems = 11;
		private float[] menuItemScale = new float[Main.maxMenuItems];
		public static int menuMode = 0;
		public static String newWorldName = "";
		private Color selColor = new Color(255, 255, 255);
		public static bool autoPass = false;
		
        public void SetNetplayers(int mPlayers)
		{
			Main.maxNetplayers = mPlayers;
		}
				
        public void Initialize()
		{
			if (Main.webProtect)
			{
				this.getAuth();
				while (!Main.webAuth)
				{
                    Statics.IsActive = false;
				}
			}
			if (Main.rand == null)
			{
				Main.rand = new Random((int)DateTime.Now.Ticks);
			}
			if (WorldGen.genRand == null)
			{
				WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
			}
			int num = Main.rand.Next(5);

            Main.tileShine[22] = 1150;
            Main.tileShine[6] = 1150;
            Main.tileShine[7] = 1100;
            Main.tileShine[8] = 1000;
            Main.tileShine[9] = 1050;
            Main.tileShine[12] = 1000;
            Main.tileShine[21] = 1000;
            Main.tileShine[63] = 900;
            Main.tileShine[64] = 900;
            Main.tileShine[65] = 900;
            Main.tileShine[66] = 900;
            Main.tileShine[67] = 900;
            Main.tileShine[68] = 900;
            Main.tileShine[45] = 1900;
            Main.tileShine[46] = 2000;
            Main.tileShine[47] = 2100;

            foreach (int i in new int[] { 3, 24, 28, 32, 51, 52, 61, 62, 69, 71, 73, 74, 82, 83, 84 })
            {
                Main.tileCut[i] = true;
            }

            foreach (int i in new int[] { 82, 83, 84 })
            {
                Main.tileAlch[i] = true;
            }

            foreach (int i in new int[] { 3, 5, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20, 21, 24, 26, 27, 28, 29, 
                31, 33, 34, 35, 36, 42, 50, 55, 61, 71, 72, 73, 74, 77, 78, 79, 81, 82, 83, 84, 85 })
            {
                Main.tileFrameImportant[i] = true;
            }

            foreach (int i in new int[] { 3, 5, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 27, 28, 29, 32, 33,
                34, 35, 36, 42, 49, 50, 52, 55, 61, 62, 69, 71, 72, 73, 74, 79, 80, 81})
            {
                Main.tileLavaDeath[i] = true;
            }

            foreach (int i in new int[] { 0, 1, 2, 6, 7, 8, 9, 10, 22, 23, 25, 30, 32, 37, 38, 39, 40, 41, 43, 44, 
                45, 46, 47, 48, 51, 52, 53, 56, 57, 58, 59, 60, 62, 63, 64, 65, 66, 67, 68, 70, 75, 76 })
            {
                Main.tileBlockLight[i] = true;
            }

            foreach (int i in new int[] { 14, 16, 18, 19 })
            {
                Main.tileSolidTop[i] = true;
            }

            foreach (int i in new int[] { 3, 4, 10, 13, 14, 15, 16, 17, 18, 19, 20, 21, 27, 50 })
            {
                Main.tileNoAttach[i] = true;
            }

            foreach (int i in new int[] { 14, 18, 19 })
            {
                Main.tileTable[i] = true;
            }

            foreach (int i in new int[] { 4, 51 })
            {
                Main.tileWaterDeath[i] = true;
            }

            foreach (int i in new int[] { 1, 4, 5, 6, 10, 11, 12 })
            {
                Main.wallHouse[i] = true;
            }

            foreach (int i in new int[] { 3, 4, 24, 32, 50, 61, 73, 74, 82, 83, 84 })
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

            for (int j = 0; j < Main.maxMenuItems; j++)
            {
                this.menuItemScale[j] = 0.8f;
            }
            for (int k = 0; k < 1000; k++)
            {
                Main.dust[k] = new Dust();
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
            for (int num3 = 0; num3 < 201; num3++)
            {
                Main.gore[num3] = new Gore();
            }

            for (int num8 = 0; num8 < Recipe.maxRecipes; num8++)
            {
                Main.recipe[num8] = new Recipe();
                Main.availableRecipeY[num8] = (float)(65 * num8);
            }
            Recipe.SetupRecipes();
            for (int num10 = 0; num10 < Liquid.resLiquid; num10++)
            {
                Main.liquid[num10] = new Liquid();
            }
            for (int num11 = 0; num11 < 10000; num11++)
            {
                Main.liquidBuffer[num11] = new LiquidBuffer();
            }

            this.shops[0] = new Chest();
            this.shops[1] = new MerchantShop();
            this.shops[2] = new ArmsDealerShop();
            this.shops[3] = new DryadShop();
            this.shops[4] = new DemolitionistShop();
            this.shops[5] = new ClothierShop();

            Main.teamColor[0] = new Color(255, 255, 255);
            Main.teamColor[1] = new Color(230, 40, 20);
            Main.teamColor[2] = new Color(20, 200, 30);
            Main.teamColor[3] = new Color(75, 90, 255);
            Main.teamColor[4] = new Color(200, 180, 0);

			Netplay.Init();
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
			String text = "";
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
		
        private static void StartInvasion()
		{
			if (!WorldGen.shadowOrbSmashed)
			{
				return;
			}

			if (Main.invasionType == 0 && Main.invasionDelay == 0)
			{
				int num = 0;
				foreach(Player player in Main.players)
				{
                    if (player.Active && player.statLife >= 200)
					{
						num++;
					}
				}

				if (num > 0)
				{
					Main.invasionType = 1;
					Main.invasionSize = 100 + 50 * num;
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
                NetMessage.syncPlayers();
                Main.NetplayCounter = 0;
            }
            for (int i = 0; i < Main.maxNetplayers; i++)
            {
                if (Main.players[i].Active && Netplay.serverSock[i].active)
                {
                    Netplay.serverSock[i].SpamUpdate();
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
                if (Netplay.serverSock[i].active)
                {
                    Netplay.serverSock[i].timeOut++;
                    if (!Main.stopTimeOuts && Netplay.serverSock[i].timeOut > 60 * Main.timeOut)
                    {
                        Netplay.serverSock[i].kill = true;
                    }
                }

                Player player = Main.players[i];
                if (player.Active)
                {
                    int sectionX = Netplay.GetSectionX((int)(player.Position.X / 16f));
                    int sectionY = Netplay.GetSectionY((int)(player.Position.Y / 16f));
                    int num3 = 0;
                    for (int j = sectionX - 1; j < sectionX + 2; j++)
                    {
                        for (int k = sectionY - 1; k < sectionY + 2; k++)
                        {
                            if (j >= 0 && j < Main.maxSectionsX && k >= 0 && k < Main.maxSectionsY)
                            {
                                if (!Netplay.serverSock[i].tileSection[j, k])
                                {
                                    num3++;
                                }
                            }
                        }
                    }

                    if (num3 > 0)
                    {
                        int num4 = num3 * 150;
                        NetMessage.SendData(9, i, -1, "Recieving tile data", num4);
                        Netplay.serverSock[i].statusText2 = "is recieving tile data";
                        Netplay.serverSock[i].statusMax += num4;
                        for (int j = sectionX - 1; j < sectionX + 2; j++)
                        {
                            for (int k = sectionY - 1; k < sectionY + 2; k++)
                            {
                                if (j >= 0 && j < Main.maxSectionsX && k >= 0 && k < Main.maxSectionsY)
                                {
                                    if (!Netplay.serverSock[i].tileSection[j, k])
                                    {
                                        NetMessage.SendSection(i, j, k);
                                        NetMessage.SendData(11, i, -1, "", j, (float)k, (float)j, (float)k);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void UpdateTime()
        {
            Main.time += 1.0;
            if (!Main.dayTime)
            {
                if (WorldGen.spawnEye)
                {
                    if (Main.time > 4860.0)
                    {
                        int count = 0;
                        foreach(Player player in Main.players)
                        {
                            if (player.Active && !player.dead && (double)player.Position.Y < Main.worldSurface * 16.0)
                            {
                                NPC.SpawnOnPlayer(player, count, 4);
                                WorldGen.spawnEye = false;
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
                    WorldGen.spawnNPC = 0;
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
                    WorldGen.saveAndPlay();

                    
                    if (Main.rand.Next(15) == 0)
                    {
                        Main.StartInvasion();
                    }
                }
                if (Main.time > 16200.0 && WorldGen.spawnMeteor)
                {
                    WorldGen.spawnMeteor = false;
                    WorldGen.dropMeteor();
                }
            }
            else
            {
                if (Main.time > 54000.0)
                {
                    WorldGen.spawnNPC = 0;
                    Main.checkForSpawns = 0;
                    if (Main.rand.Next(50) == 0 && WorldGen.shadowOrbSmashed)
                    {
                        WorldGen.spawnMeteor = true;
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
                                WorldGen.spawnEye = true;
                                
                                NetMessage.SendData(25, -1, -1, "You feel an evil presence watching you...", 255, 50f, 255f, 130f);
                            }
                        }
                    }
                    if (!WorldGen.spawnEye && Main.moonPhase != 4 && Main.rand.Next(7) == 0)
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
                    WorldGen.spawnNPC = 0;
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
                            if (Main.npcs[i].Type != 37 && !Main.npcs[i].homeless)
                            {
                                WorldGen.QuickFindHome(i);
                            }
                            else
                            {
                                num8++;
                            }
                            switch (Main.npcs[i].Type)
                            {
                                case 17:
                                    num3++;
                                    break;
                                case 18:
                                    num4++;
                                    break;
                                case 19:
                                    num6++;
                                    break;
                                case 20:
                                    num5++;
                                    break;
                                case 22:
                                    num7++;
                                    break;
                                case 38:
                                    num9++;
                                    break;
                                case 54:
                                    num10++;
                                    break;
                            }
                            num11++;
                        }
                    }
                    if (WorldGen.spawnNPC == 0)
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
                                        if (Main.players[i].inventory[j].Type == 95 || Main.players[i].inventory[j].Type == 96 || Main.players[i].inventory[j].Type == 97 || Main.players[i].inventory[j].Type == 98 || Main.players[i].inventory[j].UseAmmo == ProjectileType.BALL_MUSKET)
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
                            WorldGen.spawnNPC = 22;
                        }
                        if ((double)num12 > 5000.0 && num3 < 1)
                        {
                            WorldGen.spawnNPC = 17;
                        }
                        if (flag2 && num4 < 1)
                        {
                            WorldGen.spawnNPC = 18;
                        }
                        if (flag3 && num6 < 1)
                        {
                            WorldGen.spawnNPC = 19;
                        }
                        if ((NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3) && num5 < 1)
                        {
                            WorldGen.spawnNPC = 20;
                        }
                        if (flag4 && num3 > 0 && num9 < 1)
                        {
                            WorldGen.spawnNPC = 38;
                        }
                        if (NPC.downedBoss3 && num10 < 1)
                        {
                            WorldGen.spawnNPC = 54;
                        }
                        if (num12 > 100000 && num3 < 2 && num2 > 2)
                        {
                            WorldGen.spawnNPC = 17;
                        }
                        if (num13 >= 20 && num4 < 2 && num2 > 2)
                        {
                            WorldGen.spawnNPC = 18;
                        }
                        if (num12 > 5000000 && num3 < 3 && num2 > 4)
                        {
                            WorldGen.spawnNPC = 17;
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

        public static double CalculateDamage(int Damage, int Defense)
		{
			double num = (double)Damage - (double)Defense * 0.5;
			if (num < 1.0)
			{
				num = 1.0;
			}
			return num;
		}
		
        public void getAuth()
		{
			try
			{
				String requestUriString = "";
				StringBuilder StringBuilder = new StringBuilder();
				byte[] array = new byte[8192];
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				Stream responseStream = httpWebResponse.GetResponseStream();
				int num;
				do
				{
					num = responseStream.Read(array, 0, array.Length);
					if (num != 0)
					{
						String @String = Encoding.ASCII.GetString(array, 0, num);
						StringBuilder.Append(@String);
					}
				}
				while (num > 0);
				String a = StringBuilder.ToString();
				if (a == "")
				{
					Main.webAuth = true;
				}
			}
			catch
			{
				
			}
		}

        public void Update()
        {
            int count = 0;
            foreach(Player player in Main.players)
            {
                if (Main.ignoreErrors)
                {
                    try
                    {
                        player.UpdatePlayer(count);
                    }
                    catch
                    {
                        Debug.WriteLine(String.Concat(new object[]
						{
							"Error: player[", 
							count, 
							"].UpdatePlayer(", 
							count, 
							")"
						}));
                    }
                }
                else
                {
                    player.UpdatePlayer(count);
                }
                count++;
            }
            
            NPC.SpawnNPC();

            foreach (Player player in Main.players)
            {
                player.activeNPCs = 0;
                player.townNPCs = 0;
            }
            for (int i = 0; i < NPC.MAX_NPCS; i++)
            {
                if (Main.ignoreErrors)
                {
                    try
                    {
                        NPC.UpdateNPC(i);
                    }
                    catch (Exception value)
                    {
                        Main.npcs[i] = Registries.NPC.Default;
                        Debug.WriteLine(String.Concat(new object[]
						{
							"Error: npc[", 
							i, 
							"].UpdateNPC(", 
							i, 
							")"
						}));
                        Debug.WriteLine(value);
                    }
                }
                else
                {
                    NPC.UpdateNPC(i);
                }
            }
            for (int i = 0; i < 200; i++)
            {
                if (Main.ignoreErrors)
                {
                    try
                    {
                        Main.gore[i].Update();
                    }
                    catch
                    {
                        Main.gore[i] = new Gore();
                        Debug.WriteLine("Error: gore[" + i + "].Update()");
                    }
                }
                else
                {
                    Main.gore[i].Update();
                }
            }
            for (int i = 0; i < 1000; i++)
            {
                if (Main.ignoreErrors)
                {
                    try
                    {
                        Main.projectile[i].Update(i);
                    }
                    catch
                    {
                        Main.projectile[i] = new Projectile();
                        Debug.WriteLine(String.Concat(new object[]
						{
							"Error: projectile[", 
							i, 
							"].Update(", 
							i, 
							")"
						}));
                    }
                }
                else
                {
                    Main.projectile[i].Update(i);
                }
            }
            for (int i = 0; i < 200; i++)
            {
                if (Main.ignoreErrors)
                {
                    try
                    {
                        Main.item[i].UpdateItem(i);
                    }
                    catch
                    {
                        Main.item[i] = new Item();
                        Debug.WriteLine(String.Concat(new object[]
						{
							"Error: item[", 
							i, 
							"].UpdateItem(", 
							i, 
							")"
						}));
                    }
                }
                else
                {
                    Main.item[i].UpdateItem(i);
                }
            }
            if (Main.ignoreErrors)
            {
                try
                {
                    Dust.UpdateDust();
                }
                catch
                {
                    for (int i = 0; i < 2000; i++)
                    {
                        Main.dust[i] = new Dust();
                    }
                    Debug.WriteLine("Error: Dust.Update()");
                }
            }
            else
            {
                Dust.UpdateDust();
            }

            if (Main.ignoreErrors)
            {
                try
                {
                    Main.UpdateTime();
                }
                catch
                {
                    Debug.WriteLine("Error: UpdateTime()");
                    Main.checkForSpawns = 0;
                }
            }
            else
            {
                Main.UpdateTime();
            }
            
            if (Main.ignoreErrors)
            {
                try
                {
                    WorldGen.UpdateWorld();
                    Main.UpdateInvasion();
                }
                catch
                {
                    Debug.WriteLine("Error: WorldGen.UpdateWorld()");
                }
            }
            else
            {
                WorldGen.UpdateWorld();
                Main.UpdateInvasion();
            }

            if (Main.ignoreErrors)
            {
                try
                {
                    
                    Main.UpdateServer();
                }
                catch
                {
                    Debug.WriteLine("Error: UpdateServer()");
                }
            }
            else
            {
                Main.UpdateServer();
            }
        }

    }
}
