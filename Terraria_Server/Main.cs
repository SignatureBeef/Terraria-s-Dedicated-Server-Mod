
using System.Diagnostics;
using System;
using System.Text;
using System.Net;
using System.IO;
namespace Terraria_Server
{
	public class Main
	{
        public static string[] itemName = new string[327];
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
        public const int maxPlayers = 255;
        public const int maxChests = 1000;
        public const int maxItemTypes = 327;
        public const int maxItems = 200;
        public const int maxBuffs = 19;
        public const int maxProjectileTypes = 55;
        public const int maxProjectiles = 1000;
        public const int maxNPCTypes = 70;
        public const int maxNPCs = 1000;
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
		public static bool debugMode = false;
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
        public static string defaultIP = "";
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
		public static string statusText = "";
		public static string worldName = "";
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
		public static NPC[] npc = new NPC[1001];
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
		public static string chatText = "";
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
		public static Player[] player = new Player[256];
		public static int spawnTileX;
		public static int spawnTileY;
		public static bool npcChatRelease = false;
		public static bool editSign = false;
		public static string signText = "";
		public static string npcChatText = "";
		public static bool npcChatFocus1 = false;
		public static bool npcChatFocus2 = false;
		public static int npcShop = 0;
		public Chest[] shop = new Chest[6];
		private static Item toolTip = new Item();
		public static string motd = "";
		public bool toggleFullscreen;
		private int[] displayWidth = new int[99];
		private int[] displayHeight = new int[99];
		public static bool gameMenu = true;
		public static Player[] loadPlayer = new Player[5];
		public static string[] loadPlayerPath = new string[5];
		public static string playerPathName;
		public static string[] loadWorld = new string[999];
		public static string[] loadWorldPath = new string[999];
		public static string worldPathName;
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
		public static string getIP = Main.defaultIP;
		public static string getPort = Convert.ToString(Netplay.serverPort);
		public static bool menuMultiplayer = false;
		public static bool menuServer = false;
		public static int netMode = 2;
		public static int timeOut = 120;
		public static int NetplayCounter;
		public static int lastNPCUpdate;
		public static int lastItemUpdate;
		public static int maxNPCUpdates = 15;
		public static int maxItemUpdates = 10;
		public static string cUp = "W";
		public static string cLeft = "A";
		public static string cDown = "S";
		public static string cRight = "D";
		public static string cJump = "Space";
		public static string cThrowItem = "Q";
		public static string cInv = "Escape";
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
		public static string oldStatusText = "";
		public static bool autoShutdown = false;
		private static int maxMenuItems = 11;
		private float[] menuItemScale = new float[Main.maxMenuItems];
		public static int menuMode = 0;
		public static string newWorldName = "";
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
            Main.tileCut[3] = true;
            Main.tileCut[24] = true;
            Main.tileCut[28] = true;
            Main.tileCut[32] = true;
            Main.tileCut[51] = true;
            Main.tileCut[52] = true;
            Main.tileCut[61] = true;
            Main.tileCut[62] = true;
            Main.tileCut[69] = true;
            Main.tileCut[71] = true;
            Main.tileCut[73] = true;
            Main.tileCut[74] = true;
            Main.tileCut[82] = true;
            Main.tileCut[83] = true;
            Main.tileCut[84] = true;
            Main.tileAlch[82] = true;
            Main.tileAlch[83] = true;
            Main.tileAlch[84] = true;
            Main.tileFrameImportant[82] = true;
            Main.tileFrameImportant[83] = true;
            Main.tileFrameImportant[84] = true;
            Main.tileFrameImportant[85] = true;
            Main.tileSolid[0] = true;
            Main.tileBlockLight[0] = true;
            Main.tileSolid[1] = true;
            Main.tileBlockLight[1] = true;
            Main.tileSolid[2] = true;
            Main.tileBlockLight[2] = true;
            Main.tileSolid[3] = false;
            Main.tileNoAttach[3] = true;
            Main.tileNoFail[3] = true;
            Main.tileSolid[4] = false;
            Main.tileNoAttach[4] = true;
            Main.tileNoFail[4] = true;
            Main.tileNoFail[24] = true;
            Main.tileSolid[5] = false;
            Main.tileSolid[6] = true;
            Main.tileBlockLight[6] = true;
            Main.tileSolid[7] = true;
            Main.tileBlockLight[7] = true;
            Main.tileSolid[8] = true;
            Main.tileBlockLight[8] = true;
            Main.tileSolid[9] = true;
            Main.tileBlockLight[9] = true;
            Main.tileBlockLight[10] = true;
            Main.tileSolid[10] = true;
            Main.tileNoAttach[10] = true;
            Main.tileBlockLight[10] = true;
            Main.tileSolid[11] = false;
            Main.tileSolidTop[19] = true;
            Main.tileSolid[19] = true;
            Main.tileSolid[22] = true;
            Main.tileSolid[23] = true;
            Main.tileSolid[25] = true;
            Main.tileSolid[30] = true;
            Main.tileNoFail[32] = true;
            Main.tileBlockLight[32] = true;
            Main.tileSolid[37] = true;
            Main.tileBlockLight[37] = true;
            Main.tileSolid[38] = true;
            Main.tileBlockLight[38] = true;
            Main.tileSolid[39] = true;
            Main.tileBlockLight[39] = true;
            Main.tileSolid[40] = true;
            Main.tileBlockLight[40] = true;
            Main.tileSolid[41] = true;
            Main.tileBlockLight[41] = true;
            Main.tileSolid[43] = true;
            Main.tileBlockLight[43] = true;
            Main.tileSolid[44] = true;
            Main.tileBlockLight[44] = true;
            Main.tileSolid[45] = true;
            Main.tileBlockLight[45] = true;
            Main.tileSolid[46] = true;
            Main.tileBlockLight[46] = true;
            Main.tileSolid[47] = true;
            Main.tileBlockLight[47] = true;
            Main.tileSolid[48] = true;
            Main.tileBlockLight[48] = true;
            Main.tileSolid[53] = true;
            Main.tileBlockLight[53] = true;
            Main.tileSolid[54] = true;
            Main.tileBlockLight[52] = true;
            Main.tileSolid[56] = true;
            Main.tileBlockLight[56] = true;
            Main.tileSolid[57] = true;
            Main.tileBlockLight[57] = true;
            Main.tileSolid[58] = true;
            Main.tileBlockLight[58] = true;
            Main.tileSolid[59] = true;
            Main.tileBlockLight[59] = true;
            Main.tileSolid[60] = true;
            Main.tileBlockLight[60] = true;
            Main.tileSolid[63] = true;
            Main.tileBlockLight[63] = true;
            Main.tileStone[63] = true;
            Main.tileSolid[64] = true;
            Main.tileBlockLight[64] = true;
            Main.tileStone[64] = true;
            Main.tileSolid[65] = true;
            Main.tileBlockLight[65] = true;
            Main.tileStone[65] = true;
            Main.tileSolid[66] = true;
            Main.tileBlockLight[66] = true;
            Main.tileStone[66] = true;
            Main.tileSolid[67] = true;
            Main.tileBlockLight[67] = true;
            Main.tileStone[67] = true;
            Main.tileSolid[68] = true;
            Main.tileBlockLight[68] = true;
            Main.tileStone[68] = true;
            Main.tileSolid[75] = true;
            Main.tileBlockLight[75] = true;
            Main.tileSolid[76] = true;
            Main.tileBlockLight[76] = true;
            Main.tileSolid[70] = true;
            Main.tileBlockLight[70] = true;
            Main.tileBlockLight[51] = true;
            Main.tileNoFail[50] = true;
            Main.tileNoAttach[50] = true;
            Main.tileDungeon[41] = true;
            Main.tileDungeon[43] = true;
            Main.tileDungeon[44] = true;
            Main.tileBlockLight[30] = true;
            Main.tileBlockLight[25] = true;
            Main.tileBlockLight[23] = true;
            Main.tileBlockLight[22] = true;
            Main.tileBlockLight[62] = true;
            Main.tileSolidTop[18] = true;
            Main.tileSolidTop[14] = true;
            Main.tileSolidTop[16] = true;
            Main.tileNoAttach[20] = true;
            Main.tileNoAttach[19] = true;
            Main.tileNoAttach[13] = true;
            Main.tileNoAttach[14] = true;
            Main.tileNoAttach[15] = true;
            Main.tileNoAttach[16] = true;
            Main.tileNoAttach[17] = true;
            Main.tileNoAttach[18] = true;
            Main.tileNoAttach[19] = true;
            Main.tileNoAttach[21] = true;
            Main.tileNoAttach[27] = true;
            Main.tileFrameImportant[3] = true;
            Main.tileFrameImportant[5] = true;
            Main.tileFrameImportant[10] = true;
            Main.tileFrameImportant[11] = true;
            Main.tileFrameImportant[12] = true;
            Main.tileFrameImportant[13] = true;
            Main.tileFrameImportant[14] = true;
            Main.tileFrameImportant[15] = true;
            Main.tileFrameImportant[16] = true;
            Main.tileFrameImportant[17] = true;
            Main.tileFrameImportant[18] = true;
            Main.tileFrameImportant[20] = true;
            Main.tileFrameImportant[21] = true;
            Main.tileFrameImportant[24] = true;
            Main.tileFrameImportant[26] = true;
            Main.tileFrameImportant[27] = true;
            Main.tileFrameImportant[28] = true;
            Main.tileFrameImportant[29] = true;
            Main.tileFrameImportant[31] = true;
            Main.tileFrameImportant[33] = true;
            Main.tileFrameImportant[34] = true;
            Main.tileFrameImportant[35] = true;
            Main.tileFrameImportant[36] = true;
            Main.tileFrameImportant[42] = true;
            Main.tileFrameImportant[50] = true;
            Main.tileFrameImportant[55] = true;
            Main.tileFrameImportant[61] = true;
            Main.tileFrameImportant[71] = true;
            Main.tileFrameImportant[72] = true;
            Main.tileFrameImportant[73] = true;
            Main.tileFrameImportant[74] = true;
            Main.tileFrameImportant[77] = true;
            Main.tileFrameImportant[78] = true;
            Main.tileFrameImportant[79] = true;
            Main.tileFrameImportant[81] = true;
            Main.tileTable[14] = true;
            Main.tileTable[18] = true;
            Main.tileTable[19] = true;
            Main.tileWaterDeath[4] = true;
            Main.tileWaterDeath[51] = true;
            Main.tileLavaDeath[3] = true;
            Main.tileLavaDeath[5] = true;
            Main.tileLavaDeath[10] = true;
            Main.tileLavaDeath[11] = true;
            Main.tileLavaDeath[12] = true;
            Main.tileLavaDeath[13] = true;
            Main.tileLavaDeath[14] = true;
            Main.tileLavaDeath[15] = true;
            Main.tileLavaDeath[16] = true;
            Main.tileLavaDeath[17] = true;
            Main.tileLavaDeath[18] = true;
            Main.tileLavaDeath[19] = true;
            Main.tileLavaDeath[20] = true;
            Main.tileLavaDeath[27] = true;
            Main.tileLavaDeath[28] = true;
            Main.tileLavaDeath[29] = true;
            Main.tileLavaDeath[32] = true;
            Main.tileLavaDeath[33] = true;
            Main.tileLavaDeath[34] = true;
            Main.tileLavaDeath[35] = true;
            Main.tileLavaDeath[36] = true;
            Main.tileLavaDeath[42] = true;
            Main.tileLavaDeath[49] = true;
            Main.tileLavaDeath[50] = true;
            Main.tileLavaDeath[52] = true;
            Main.tileLavaDeath[55] = true;
            Main.tileLavaDeath[61] = true;
            Main.tileLavaDeath[62] = true;
            Main.tileLavaDeath[69] = true;
            Main.tileLavaDeath[71] = true;
            Main.tileLavaDeath[72] = true;
            Main.tileLavaDeath[73] = true;
            Main.tileLavaDeath[74] = true;
            Main.tileLavaDeath[79] = true;
            Main.tileLavaDeath[80] = true;
            Main.tileLavaDeath[81] = true;
            Main.wallHouse[1] = true;
            Main.wallHouse[4] = true;
            Main.wallHouse[5] = true;
            Main.wallHouse[6] = true;
            Main.wallHouse[10] = true;
            Main.wallHouse[11] = true;
            Main.wallHouse[12] = true;
            Main.tileNoFail[61] = true;
            Main.tileNoFail[73] = true;
            Main.tileNoFail[74] = true;
            Main.tileNoFail[82] = true;
            Main.tileNoFail[83] = true;
            Main.tileNoFail[84] = true;

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
            for (int m = 0; m < 1001; m++)
            {
                Main.npc[m] = new NPC();
                Main.npc[m].whoAmI = m;
            }
            for (int n = 0; n < 256; n++)
            {
                Main.player[n] = new Player();
            }
            for (int num2 = 0; num2 < 1001; num2++)
            {
                Main.projectile[num2] = new Projectile();
            }
            for (int num3 = 0; num3 < 201; num3++)
            {
                Main.gore[num3] = new Gore();
            }
            for (int num7 = 0; num7 < 327; num7++)
            {
                Item item = new Item();
                item.SetDefaults(num7, false);
                Main.itemName[num7] = item.name;
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
            this.shop[0] = new Chest();
            this.shop[1] = new Chest();
            this.shop[1].SetupShop(1);
            this.shop[2] = new Chest();
            this.shop[2].SetupShop(2);
            this.shop[3] = new Chest();
            this.shop[3].SetupShop(3);
            this.shop[4] = new Chest();
            this.shop[4].SetupShop(4);
            this.shop[5] = new Chest();
            this.shop[5].SetupShop(5);
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
			if (Main.netMode == 0)
			{
				return;
			}
			if (Main.netMode == 2)
			{
				NetMessage.SendData(25, -1, -1, text, 255, 175f, 75f, 255f);
			}
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
				for (int i = 0; i < 255; i++)
				{
					if (Main.player[i].active && Main.player[i].statLife >= 200)
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
                NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f);
                NetMessage.syncPlayers();
                Main.NetplayCounter = 0;
            }
            for (int i = 0; i < Main.maxNetplayers; i++)
            {
                if (Main.player[i].active && Netplay.serverSock[i].active)
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
                    if (!Main.item[num].active || Main.item[num].owner == 255)
                    {
                        NetMessage.SendData(21, -1, -1, "", num, 0f, 0f, 0f);
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
                if (Main.item[i].active && (Main.item[i].owner == 255 || !Main.player[Main.item[i].owner].active))
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
                if (Main.player[i].active)
                {
                    int sectionX = Netplay.GetSectionX((int)(Main.player[i].position.X / 16f));
                    int sectionY = Netplay.GetSectionY((int)(Main.player[i].position.Y / 16f));
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
                        NetMessage.SendData(9, i, -1, "Recieving tile data", num4, 0f, 0f, 0f);
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
                if (WorldGen.spawnEye && Main.netMode != 1)
                {
                    if (Main.time > 4860.0)
                    {
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.player[i].active && !Main.player[i].dead && (double)Main.player[i].position.Y < Main.worldSurface * 16.0)
                            {
                                NPC.SpawnOnPlayer(i, 4);
                                WorldGen.spawnEye = false;
                                break;
                            }
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
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f);
                        WorldGen.saveAndPlay();
                    }
                    if (Main.netMode != 1)
                    {
                        if (Main.rand.Next(15) == 0)
                        {
                            Main.StartInvasion();
                        }
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
                    if (Main.rand.Next(50) == 0 && Main.netMode != 1 && WorldGen.shadowOrbSmashed)
                    {
                        WorldGen.spawnMeteor = true;
                    }
                    if (!NPC.downedBoss1 && Main.netMode != 1)
                    {
                        bool flag = false;
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.player[i].active && Main.player[i].statLifeMax >= 200)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag && Main.rand.Next(3) == 0)
                        {
                            int num = 0;
                            for (int i = 0; i < 1000; i++)
                            {
                                if (Main.npc[i].active)
                                {
                                    if (Main.npc[i].townNPC)
                                    {
                                        num++;
                                    }
                                }
                            }
                            if (num >= 4)
                            {
                                WorldGen.spawnEye = true;
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendData(25, -1, -1, "You feel an evil presence watching you...", 255, 50f, 255f, 130f);
                                }
                            }
                        }
                    }
                    if (!WorldGen.spawnEye && Main.moonPhase != 4 && Main.rand.Next(7) == 0 && Main.netMode != 1)
                    {
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.player[i].active && Main.player[i].statLifeMax > 100)
                            {
                                Main.bloodMoon = true;
                                break;
                            }
                        }
                        if (Main.bloodMoon)
                        {
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendData(25, -1, -1, "The Blood Moon is rising...", 255, 50f, 255f, 130f);
                            }
                        }
                    }
                    Main.time = 0.0;
                    Main.dayTime = false;
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f);
                    }
                }
                if (Main.netMode != 1)
                {
                    Main.checkForSpawns++;
                    if (Main.checkForSpawns >= 7200)
                    {
                        int num2 = 0;
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.player[i].active)
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
                        for (int i = 0; i < 1000; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].townNPC)
                            {
                                if (Main.npc[i].type != 37 && !Main.npc[i].homeless)
                                {
                                    WorldGen.QuickFindHome(i);
                                }
                                else
                                {
                                    num8++;
                                }
                                if (Main.npc[i].type == 17)
                                {
                                    num3++;
                                }
                                if (Main.npc[i].type == 18)
                                {
                                    num4++;
                                }
                                if (Main.npc[i].type == 19)
                                {
                                    num6++;
                                }
                                if (Main.npc[i].type == 20)
                                {
                                    num5++;
                                }
                                if (Main.npc[i].type == 22)
                                {
                                    num7++;
                                }
                                if (Main.npc[i].type == 38)
                                {
                                    num9++;
                                }
                                if (Main.npc[i].type == 54)
                                {
                                    num10++;
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
                                if (Main.player[i].active)
                                {
                                    for (int j = 0; j < 44; j++)
                                    {
                                        if (Main.player[i].inventory[j] != null & Main.player[i].inventory[j].stack > 0)
                                        {
                                            if (Main.player[i].inventory[j].type == 71)
                                            {
                                                num12 += Main.player[i].inventory[j].stack;
                                            }
                                            if (Main.player[i].inventory[j].type == 72)
                                            {
                                                num12 += Main.player[i].inventory[j].stack * 100;
                                            }
                                            if (Main.player[i].inventory[j].type == 73)
                                            {
                                                num12 += Main.player[i].inventory[j].stack * 10000;
                                            }
                                            if (Main.player[i].inventory[j].type == 74)
                                            {
                                                num12 += Main.player[i].inventory[j].stack * 1000000;
                                            }
                                            if (Main.player[i].inventory[j].type == 95 || Main.player[i].inventory[j].type == 96 || Main.player[i].inventory[j].type == 97 || Main.player[i].inventory[j].type == 98 || Main.player[i].inventory[j].useAmmo == 14)
                                            {
                                                flag3 = true;
                                            }
                                            if (Main.player[i].inventory[j].type == 166 || Main.player[i].inventory[j].type == 167 || Main.player[i].inventory[j].type == 168 || Main.player[i].inventory[j].type == 235)
                                            {
                                                flag4 = true;
                                            }
                                        }
                                    }
                                    int num14 = Main.player[i].statLifeMax / 20;
                                    if (num14 > 5)
                                    {
                                        flag2 = true;
                                    }
                                    num13 += num14;
                                }
                            }
                            if (WorldGen.spawnNPC == 0 && num7 < 1)
                            {
                                WorldGen.spawnNPC = 22;
                            }
                            if (WorldGen.spawnNPC == 0 && (double)num12 > 5000.0 && num3 < 1)
                            {
                                WorldGen.spawnNPC = 17;
                            }
                            if (WorldGen.spawnNPC == 0 && flag2 && num4 < 1)
                            {
                                WorldGen.spawnNPC = 18;
                            }
                            if (WorldGen.spawnNPC == 0 && flag3 && num6 < 1)
                            {
                                WorldGen.spawnNPC = 19;
                            }
                            if (WorldGen.spawnNPC == 0 && (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3) && num5 < 1)
                            {
                                WorldGen.spawnNPC = 20;
                            }
                            if (WorldGen.spawnNPC == 0 && flag4 && num3 > 0 && num9 < 1)
                            {
                                WorldGen.spawnNPC = 38;
                            }
                            if (WorldGen.spawnNPC == 0 && NPC.downedBoss3 && num10 < 1)
                            {
                                WorldGen.spawnNPC = 54;
                            }
                            if (WorldGen.spawnNPC == 0 && num12 > 100000 && num3 < 2 && num2 > 2)
                            {
                                WorldGen.spawnNPC = 17;
                            }
                            if (WorldGen.spawnNPC == 0 && num13 >= 20 && num4 < 2 && num2 > 2)
                            {
                                WorldGen.spawnNPC = 18;
                            }
                            if (WorldGen.spawnNPC == 0 && num12 > 5000000 && num3 < 3 && num2 > 4)
                            {
                                WorldGen.spawnNPC = 17;
                            }
                            if (!NPC.downedBoss3 && num8 == 0)
                            {
                                int num15 = NPC.NewNPC(Main.dungeonX * 16 + 8, Main.dungeonY * 16, 37, 0);
                                Main.npc[num15].homeless = false;
                                Main.npc[num15].homeTileX = Main.dungeonX;
                                Main.npc[num15].homeTileY = Main.dungeonY;
                            }
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
				string requestUriString = "";
				StringBuilder stringBuilder = new StringBuilder();
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
						string @string = Encoding.ASCII.GetString(array, 0, num);
						stringBuilder.Append(@string);
					}
				}
				while (num > 0);
				string a = stringBuilder.ToString();
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
            for (int i = 0; i < 255; i++)
            {
                if (Main.ignoreErrors)
                {
                    try
                    {
                        Main.player[i].UpdatePlayer(i);
                    }
                    catch
                    {
                        Debug.WriteLine(string.Concat(new object[]
						{
							"Error: player[", 
							i, 
							"].UpdatePlayer(", 
							i, 
							")"
						}));
                    }
                }
                else
                {
                    Main.player[i].UpdatePlayer(i);
                }
            }
            if (Main.netMode != 1)
            {
                NPC.SpawnNPC();
            }
            for (int i = 0; i < 255; i++)
            {
                Main.player[i].activeNPCs = 0;
                Main.player[i].townNPCs = 0;
            }
            for (int i = 0; i < 1000; i++)
            {
                if (Main.ignoreErrors)
                {
                    try
                    {
                        Main.npc[i].UpdateNPC(i);
                    }
                    catch (Exception value)
                    {
                        Main.npc[i] = new NPC();
                        Debug.WriteLine(string.Concat(new object[]
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
                    Main.npc[i].UpdateNPC(i);
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
                        Debug.WriteLine(string.Concat(new object[]
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
                        Debug.WriteLine(string.Concat(new object[]
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
            if (Main.netMode != 1)
            {
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
            }
            if (Main.ignoreErrors)
            {
                try
                {
                    if (Main.netMode == 2)
                    {
                        Main.UpdateServer();
                    }
                }
                catch
                {
                    if (Main.netMode == 2)
                    {
                        Debug.WriteLine("Error: UpdateServer()");
                    }
                    else
                    {
                        Debug.WriteLine("Error: UpdateClient();");
                    }
                }
            }
            else
            {
                if (Main.netMode == 2)
                {
                    Main.UpdateServer();
                }
            }
        }

    }
}
