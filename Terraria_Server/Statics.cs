using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public static class Statics
    {

        //public static int currentRelease = 3;
        public static int currentRelease = 4;
        //public static string versionNumber = "v1.0.2";
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
        //public static float leftWorld = 0.0f;
        //public static float rightWorld = 134400f;
        //public static float topWorld = 0.0f;
        //public static float bottomWorld = 38400f;
        //public static int maxTilesX = (int)world.getRightWorld() / 16 + 1;
        //public static int maxTilesY = (int)world.getBottomWorld() / 16 + 1;
        //public static int maxSectionsX = world.getMaxTilesX() / 200;
        //public static int maxSectionsY = world.getMaxTilesY() / 150;
        public static bool verboseNetplay = false;
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

        public static bool saveLock = false;

        public static int chatLength = 600;
        public static Color[] teamColor = new Color[5];
        
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
        public static int netPlayCounter;
        public static bool stopTimeOuts = false;
        public static int timeOut = 120;

        public static bool webProtect = false;

        public static int screenWidth = 600;
        public static int screenHeight = 800;
        public static int evilTiles;

        public static bool serverStarted = false;

        public static Vector2 screenPosition = new Vector2();
        public static Vector2 screenLastPosition = new Vector2();

        //public static string SavePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\Terraria";
        public static string SavePath = Environment.CurrentDirectory;
        public static string WorldPath = Statics.SavePath + "\\Worlds";
        public static string PlayerPath = Statics.SavePath + "\\Players";

        [ThreadStatic]
        //public static NetPlay Netplay;
        public static Random rand;

        public static bool isRunning = false;


        public static bool webAuth { get; set; }

        public static bool editSign { get; set; }

        public static int jungleTiles { get; set; }

        public static int meteorTiles { get; set; }

        public static int dungeonTiles { get; set; }
    }
}
