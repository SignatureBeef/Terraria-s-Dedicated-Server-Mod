using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public static class Statics
    {

        public static bool isLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static string getWorldPath
        {
            get
            {
                return Statics.SavePath + systemSeperator + "Worlds";
            }
        }

        public static string getPlayerPath
        {
            get
            {
                return Statics.SavePath + systemSeperator + "Players";
            }
        }

        public static string getPluginPath
        {
            get
            {
                return Statics.SavePath + systemSeperator + "Plugins";
            }
        }

        public static string getDataPath
        {
            get
            {
                return Statics.SavePath + systemSeperator + "Data";
            }
        }

        public static string systemSeperator = "\\";

        public static int currentRelease = 4;
        public static string versionNumber = "v1.0.3";
        /*
         * @netMode
         * 
         *  0 = Not connected
         *  1 = Client Connecting/Connected
         *  2 = Server Running/Starting
         * 
         */
        public static int netMode = 2; //server mode
        public static int myPlayer = 0; //current selected player
        public static bool ignoreErrors = false;
        public static bool verboseNetPlay = false;
        public static bool[] wallHouse = new bool[14];
        public static bool[] tileStone = new bool[80];
        public static bool[] tileWaterDeath = new bool[80];
        public static bool[] tileLavaDeath = new bool[80];
        public static bool[] tileTable = new bool[80];
        public static bool[] tileBlockLight = new bool[80];
        public static bool[] tileDungeon = new bool[80];
        public static bool[] tileSolidTop = new bool[80];
        public static bool[] tileSolid = new bool[80];
        public static bool[] tileNoAttach = new bool[80];
        public static bool[] tileNoFail = new bool[80];
        public static bool[] tileFrameImportant = new bool[91];
        public static Recipe[] recipe = new Recipe[Recipe.maxRecipes];
        public static int[] availableRecipe = new int[Recipe.maxRecipes];
        public static float[] availableRecipeY = new float[Recipe.maxRecipes];
        public static int numAvailableRecipes;
        public static int focusRecipe;
        public static bool playerInventory = false;
        public static int spawnTileX;
        public static int spawnTileY;

        public static NPC[] npc = new NPC[1001];
        public static bool signBubble = false;
        public static int signX = 0;
        public static int signY = 0;
        public static bool dedicated = true;

        public static bool saveLock = false;

        public static int chatLength = 600;
        public static Color[] teamColor = new Color[5];
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
			14, 
			16, 
			2, 
			10, 
			1, 
			16, 
			16, 
			16, 
			3, 
			1, 
			14, 
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
			14, 
			3
		};
        
        public static bool fixedTiming = false;
        public static bool IsActive = false;
        public static bool IsFixedTimeStep = false;

        public static int saveTimer = 0;
        public static int updateTime = 0;
        public static int drawTime = 0;
        public static int cloudLimit = 100;
        public static int checkForSpawns = 0;

        public static int lastNPCUpdate;
        public static int lastItemUpdate;
        public static int maxNPCUpdates = 15;
        public static int maxItemUpdates = 10;
        public static int NetPlayCounter;
        public static bool stopTimeOuts = false;
        public static int timeOut = 120;

        public static bool webProtect = false;

        public static int screenWidth = 600;
        public static int screenHeight = 800;
        public static int evilTiles;

        public static bool serverStarted = false;

        public static Vector2 screenPosition = new Vector2();
        public static Vector2 screenLastPosition = new Vector2();

        //public static string SavePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\Terraria_Server";
        public static string SavePath = Environment.CurrentDirectory;

        //public static string WorldPath = Statics.SavePath + "\\Worlds";
        //public static string PlayerPath = Statics.SavePath + "\\Players";

        [ThreadStatic]
        //public static NetPlay NetPlay;
        public static Random rand;

        public static bool isRunning = false;


        public static bool webAuth = false;
        public static int meteorTiles;
        public static int jungleTiles;
        public static int dungeonTiles;
        public static bool editSign = false;
        public static string npcChatText = "";
        public static bool dumbAI = false;
    }
}
