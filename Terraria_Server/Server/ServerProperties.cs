using System;
using System.IO;
using Terraria_Server.Misc;

namespace Terraria_Server
{
    public class ServerProperties : PropertiesFile
    {
        private const bool DEFAULT_AUTOMATIC_UPDATES = false;
        private const String DEFAULT_GREETING = "Welcome to a TDSM Server!";
        private const String DEFAULT_MAP_SIZE = "small";
        private const int DEFAULT_MAX_PLAYERS = 8;
        private const bool DEFAULT_NPC_DOOR_OPEN_CANCEL = false;
        private const int DEFAULT_PORT = 7777;
        private const int DEFAULT_SEED = -1;
        private const String DEFAULT_SERVER_IP = "0.0.0.0";
        private const bool DEFAULT_USE_CUSTOM_TILES = false;
        private const bool DEFAULT_USE_CUSTOM_GEN_OPTS = false;
        private const bool DEFAULT_WHITE_LIST = false;
        private const String DEFAULT_WORLD = "world1.wld";
        private const String DEFAULT_PID_FILE = "";
        private const bool DEFAULT_SIMPLE_LOOP = false;

        private const String AUTOMATIC_UPDATES = "allowupdates";
        private const String DUNGEON_AMOUNT = "opt-numdungeons";
        private const String FLOATING_ISLAND_AMOUNT = "opt-num-floating-islands";
        private const String GREETING = "greeting";
        private const String MAP_SIZE = "opt-mapsize";
        private const String MAX_PLAYERS = "maxplayers";
        private const String MAX_TILES_X = "opt-maxtilesx";
        private const String MAX_TILES_Y = "opt-maxtilesy";
        private const String NPC_DOOR_OPEN_CANCEL = "npc-cancelopendoor";
        private const String PASSWORD = "server-password";
        private const String PORT = "port";
        private const String SEED = "opt-seed";
        private const String SERVER_IP = "serverip";
        private const String USE_CUSTOM_TILES = "opt-usecustomtiles";
        private const String USE_CUSTOM_GEN_OPTS = "opt-custom-worldgen";
        private const String WHITE_LIST = "whitelist";
        private const String WORLD_PATH = "worldpath";
        private const String PID_FILE = "pid-file";
        private const String SIMPLE_LOOP = "simple-loop";

        public ServerProperties(String propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = MaxPlayers;
            temp = ServerIP;
            temp = Port;
            temp = Greeting;
            temp = WorldPath;
            temp = Password;
            temp = AutomaticUpdates;
            temp = NPCDoorOpenCancel;
            temp = Seed;
            temp = MapSize;
            temp = UseCustomTiles;
            temp = MaxTilesX;
            temp = MaxTilesY;
            temp = UseCustomGenOpts;
            temp = DungeonAmount;
            temp = FloatingIslandAmount;
            temp = PIDFile;
            temp = SimpleLoop;
        }

        public int MaxPlayers
        {
            get
            {
                return getValue(MAX_PLAYERS, DEFAULT_MAX_PLAYERS);
            }
            set
            {
                setValue(MAX_PLAYERS, value);
            }
        }

        public int Port
        {
            get
            {
                return getValue(PORT, DEFAULT_PORT);
            }
            set
            {
                setValue(PORT, value);
            }
        }

        public String Greeting
        {
            get
            {
                return getValue(GREETING, DEFAULT_GREETING);
            }
            set
            {
                setValue(GREETING, value);
            }
        }

        public String ServerIP
        {
            get
            {
                return getValue(SERVER_IP, DEFAULT_SERVER_IP);
            }
            set
            {
                setValue(SERVER_IP, value);
            }
        }

        public String WorldPath
        {
            get
            {
                return getValue(WORLD_PATH, Statics.WorldPath + Path.DirectorySeparatorChar + DEFAULT_WORLD);
            }
            set
            {
                setValue(WORLD_PATH, value);
            }
        }

        public String Password
        {
            get
            {
                return getValue(PASSWORD, String.Empty);
            }
            set
            {
                setValue(PASSWORD, value);
            }
        }

        public int Seed
        {
            get
            {
                return getValue(SEED, DEFAULT_SEED);
            }
            set
            {
                setValue(SEED, value);
            }
        }

        public int MaxTilesX
        {
            get
            {
                return getValue(MAX_TILES_X, World.MAP_SIZE.SMALL_X);
            }
            set
            {

                setValue(MAX_TILES_X, value);
            }
        }

        public int MaxTilesY
        {
            get
            {
                return getValue(MAX_TILES_Y, World.MAP_SIZE.SMALL_Y);
            }
            set
            {
                setValue(MAX_TILES_Y, value);
            }
        }

        private int getValue(String key, World.MAP_SIZE mapSize)
        {
            return getValue(key, (int)mapSize);
        }

        public bool UseCustomTiles
        {
            get
            {
                return getValue(USE_CUSTOM_TILES, DEFAULT_USE_CUSTOM_TILES);
            }
            set
            {
                setValue(USE_CUSTOM_TILES, value);
            }
        }

        public int[] getMapSizes()
        {
            String CustomTiles = base.getValue(MAP_SIZE);

            if (CustomTiles == null)
            {
                return null;
            }
            switch (CustomTiles.Trim().ToLower())
            {
                case "small":
                    {
                        return new int[] { (int)World.MAP_SIZE.SMALL_X, (int)World.MAP_SIZE.SMALL_Y };
                    }
                case "medium":
                    {
                        return new int[] { (int)World.MAP_SIZE.MEDIUM_X, (int)World.MAP_SIZE.MEDIUM_Y };
                    }
                case "large":
                    {
                        return new int[] { (int)World.MAP_SIZE.LARGE_X, (int)World.MAP_SIZE.LARGE_Y };
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public String MapSize
        {
            get
            {
                String CustomTiles = getValue(MAP_SIZE, DEFAULT_MAP_SIZE);
                if (CustomTiles == null)
                {
                    return "small";
                }
                switch (CustomTiles.Trim().ToLower())
                {
                    case "small":
                        {
                            return "small";
                        }
                    case "medium":
                        {
                            return "medium";
                        }
                    case "large":
                        {
                            return "large";
                        }
                    default:
                        {
                            return "small";
                        }
                }
            }
            set
            {
                setValue(MAP_SIZE, value);
            }
        }

        public bool UseWhiteList
        {
            get
            {
                return getValue(WHITE_LIST, DEFAULT_WHITE_LIST);
            }
            set
            {
                setValue(WHITE_LIST, value);
            }
        }

        public bool NPCDoorOpenCancel
        {
            get
            {
                return getValue(NPC_DOOR_OPEN_CANCEL, DEFAULT_NPC_DOOR_OPEN_CANCEL);
            }
            set
            {
                setValue(NPC_DOOR_OPEN_CANCEL, value);
            }
        }

        public int DungeonAmount
        {
            get
            {
                int amount = getValue(DUNGEON_AMOUNT, -1);
                if(amount <= 0)
                {
                    amount = 1;
                    setValue(DUNGEON_AMOUNT, amount);
                }
                else if(amount > 10)
                {
                    amount = 10;
                    setValue(DUNGEON_AMOUNT, amount);
                }
                return amount;
            }
            set
            {
                setValue(DUNGEON_AMOUNT, value);
            }
        }

        public bool UseCustomGenOpts
        {
            get
            {
                return getValue(USE_CUSTOM_GEN_OPTS, DEFAULT_USE_CUSTOM_GEN_OPTS);
            }
            set
            {
                setValue(USE_CUSTOM_GEN_OPTS, value);
            }
        }

        public int FloatingIslandAmount
        {
            get
            {
                int amount = getValue(FLOATING_ISLAND_AMOUNT, -1);
                if (amount <= 0)
                {
                    amount = (int)((double)Main.maxTilesX * 0.0008);
                    setValue(FLOATING_ISLAND_AMOUNT, amount);
                }
                else if (amount > (int)((double)Main.maxTilesX * 0.0008) * 3)
                {
                    amount = (int)((double)Main.maxTilesX * 0.0008) * 3;
                    setValue(FLOATING_ISLAND_AMOUNT, amount);
                }
                return amount;
            }
            set
            {
                setValue(FLOATING_ISLAND_AMOUNT, value);
            }
        }

        public bool AutomaticUpdates
        {
            get
            {
                return getValue(AUTOMATIC_UPDATES, DEFAULT_AUTOMATIC_UPDATES);
            }
            set
            {
                setValue(AUTOMATIC_UPDATES, value);
            }
        }

        public String PIDFile
        {
            get
            {
                return getValue(PID_FILE, DEFAULT_PID_FILE);
            }
            set
            {
                setValue(PID_FILE, value);
            }
        }

        public bool SimpleLoop
        {
            get
            {
                return getValue(SIMPLE_LOOP, DEFAULT_SIMPLE_LOOP);
            }
            set
            {
                setValue(SIMPLE_LOOP, value);
            }
        }
    }
}
