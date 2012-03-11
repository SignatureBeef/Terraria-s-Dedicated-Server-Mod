using System;
using System.IO;
using Terraria_Server.Misc;
using Terraria_Server.Commands;

namespace Terraria_Server
{
    public class ServerProperties : PropertiesFile
    {
#region Default Values
        private const bool      DEFAULT_ALLOW_BACKUPS			= true;
        private const bool      DEFAULT_ALLOW_EXPLOSIONS        = true;
        private const bool      DEFAULT_ALLOW_TDCMRPG           = true;
        private const bool		DEFAULT_ALWAYS_GENERATE_SNOW	= false;
        private const bool      DEFAULT_AUTOMATIC_UPDATES       = false;
		private const int		DEFAULT_BACKUP_MINUTE_INTERVAL	= 60;
        private const bool      DEFAULT_BUFFER_LIQUID_UPDATES   = false;
        private const bool      DEFAULT_COLLECT_GARBAGE			= true;
        private const int       DEFAULT_EXIT_USERS              = -1;
        private const string    DEFAULT_GREETING                = "Welcome to a TDSM Server!@         ~ tdsm.org ~";
        private const bool		DEFAULT_GENERATE_JUNGLE			= true;
        private const bool		DEFAULT_GENERATE_SNOW			= true;
        private const bool      DEFAULT_HACKED_DATA             = false;
        private const string    DEFAULT_HARDCORE_DEATH_ACTION   = "none";
        private const bool      DEFAULT_LOG_ROTATION            = true;
        private const string    DEFAULT_MAP_SIZE                = "small";
        private const int       DEFAULT_MAX_PLAYERS             = 8;
        private const int       DEFAULT_MAX_RESPAWNTIME         = 0;
        private const bool      DEFAULT_NPC_DOOR_OPEN_CANCEL    = false;
        private const bool      DEFAULT_NPCSPAWN_OVERRIDE		= true;
        private const int       DEFAULT_OVERLIMIT_SLOTS         = 1;
        private const string    DEFAULT_PID_FILE                = "";
        private const int		DEFAULT_PURGE_BACKUPS_AFTER     = 120;
        private const int       DEFAULT_PORT                    = 7777;
        private const string    DEFAULT_REJECT_ITEMS            = "";
        private const string    DEFAULT_RCON_BIND_ADDRESS       = "127.0.0.1:7023";
        private const bool      DEFAULT_SIMPLE_LOOP             = true;
        private const int       DEFAULT_SEND_QUEUE_QUOTA        = 1024;
        private const string    DEFAULT_SEED                    = "-1";
        private const string    DEFAULT_SERVER_IP               = "0.0.0.0";
        private const int       DEFAULT_SPAWN_NPC_MAX           = 100;
        private const bool      DEFAULT_STOP_NPC_SPAWNING       = false;
        private const bool      DEFAULT_STOP_UPDATES_WHEN_EMPTY = true;
        private const string    DEFAULT_TILESQUARE_MESSAGES     = "ignore";
        private const bool      DEFAULT_USE_CUSTOM_TILES        = false;
        private const bool      DEFAULT_USE_CUSTOM_GEN_OPTS     = false;
        private const bool      DEFAULT_WHITE_LIST              = false;
        private const string    DEFAULT_WORLD                   = "world1.wld";
#endregion

#region Key Values
        private const string    ALLOW_BACKUPS					= "allow-backups";
        private const string    ALLOW_EXPLOSIONS                = "explosions";
        private const string    ALLOW_TDCMRPG                   = "allow-tdcmrpg";
        private const string    AUTOMATIC_UPDATES               = "allowupdates";
        private const string    ALWAYS_GENERATE_SNOW			= "always-generate-snow";
        private const string    BACKUP_MINUTE_INTERVAL			= "backup-minutes-interval";
        private const string    BUFFER_LIQUID_UPDATES           = "buffer-liquid-updates";
        private const string    COLLECT_GARBAGE					= "collect-garbage";
        private const string    EXIT_USERS                      = "exitaccesslevel";
        private const string    DUNGEON_AMOUNT                  = "opt-numdungeons";
        private const string    FLOATING_ISLAND_AMOUNT          = "opt-num-floating-islands";
        private const string    GREETING                        = "greeting";
        private const string    GENERATE_JUNGLE					= "generate-jungle";
        private const string    GENERATE_SNOW					= "generate-snow";
        private const string    HACKED_DATA                     = "hackeddata";
        private const string    HARDCORE_DEATH_ACTION           = "hardcore-death-action";
        private const string    LOG_ROTATION                    = "log-rotation";
        private const string    MAP_SIZE                        = "opt-mapsize";
        private const string    MAX_PLAYERS                     = "maxplayers";
        private const string    MAX_RESPAWNTIME                 = "max-respawn-time";
        private const string    MAX_TILES_X                     = "opt-maxtilesx";
        private const string    MAX_TILES_Y                     = "opt-maxtilesy";
        private const string    NPC_DOOR_OPEN_CANCEL            = "npc-cancelopendoor";
		private const string	NPCSPAWN_OVERRIDE				= "npcspawns-override";
        private const string    OVERLIMIT_SLOTS                 = "overlimit-slots";
        private const string    PASSWORD                        = "server-password";
        private const string    PID_FILE                        = "pid-file";
        private const string    PURGE_BACKUPS_AFTER             = "purge-backups-after-xmins";
        private const string    PORT                            = "port";
        private const string    REJECT_ITEMS                    = "rejectplayeritems";
        private const string    RCON_BIND_ADDRESS               = "rcon-bind-address";
        private const string    RCON_HASH_ONCE                  = "rcon-hash-nonce";
        private const string    SIMPLE_LOOP                     = "simple-loop";
        private const string    SEED                            = "opt-seed";
        private const string    SEND_QUEUE_QUOTA                = "send-queue-quota";
        private const string    SERVER_IP                       = "serverip";
        private const string    SPAWN_NPC_MAX                   = "spawnnpc-max";
        private const string    STOP_NPC_SPAWNING               = "stop-npc-spawning";
        private const string    STOP_UPDATES_WHEN_EMPTY         = "stop-updates-when-empty";
        private const string    TILESQUARE_MESSAGES             = "tile-square-messages";
        private const string    USE_CUSTOM_TILES                = "opt-usecustomtiles";
        private const string    USE_CUSTOM_GEN_OPTS             = "opt-custom-worldgen";
        private const string    WHITE_LIST                      = "whitelist";
        private const string    WORLD_PATH                      = "worldpath";
#endregion

        public ServerProperties(string propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = null;
			temp = AllowBackups;
            temp = AllowExplosions;
            temp = AllowTDCMRPG;
			temp = AlwaysGenerateSnow;
			temp = AutomaticUpdates;
			temp = BackupInterval;
			temp = BufferLiquidUpdates;
			temp = CollectGarbage;
            temp = DungeonAmount;
            temp = ExitAccessLevel;
            temp = FloatingIslandAmount;
			temp = GenerateJungle;
			temp = GenerateSnow;
            temp = Greeting;
            temp = HackedData;
            temp = HardcoreDeathAction;
            temp = LogRotation;
            temp = MapSize;
            temp = MaxPlayers;
            temp = MaxRespawnTime;
            temp = MaxTilesX;
			temp = MaxTilesY;
			temp = NPCDoorOpenCancel;
			temp = NPCSpawnsOverride;
            temp = OverlimitSlots;
            temp = Password;
            temp = PIDFile;
			temp = PurgeBackupsMinutes;
            temp = Port;
            temp = RejectedItems;
            temp = RConBindAddress;
            temp = RConHashNonce;
            temp = Seed;
            temp = SendQueueQuota;
            temp = ServerIP;
            temp = SimpleLoop;
            temp = SpawnNPCMax;
            temp = StopNPCSpawning;
            temp = StopUpdatesWhenEmpty;
            temp = TileSquareMessages;
            temp = UseCustomTiles;
            temp = UseCustomGenOpts;
            temp = WorldPath;
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

        public string Greeting
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

        public string ServerIP
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

        public string WorldPath
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

        public string Password
        {
            get
            {
                return getValue(PASSWORD, string.Empty);
            }
            set
            {
                setValue(PASSWORD, value);
            }
        }

        public string Seed
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

        public int[] GetMapSizes()
        {
            string CustomTiles = base.getValue(MAP_SIZE);

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

        public string MapSize
        {
            get
            {
                string CustomTiles = getValue(MAP_SIZE, DEFAULT_MAP_SIZE);
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

        public string PIDFile
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

        public bool HackedData
        {
            get
            {
                return getValue(HACKED_DATA, DEFAULT_HACKED_DATA);
            }
            set
            {
                setValue(HACKED_DATA, value);
            }
        }
        
		public string RConBindAddress
		{
            get { return getValue(RCON_BIND_ADDRESS, DEFAULT_RCON_BIND_ADDRESS); }
			set { setValue(RCON_BIND_ADDRESS, value); }
		}
		
		public string RConHashNonce
		{
			get {
                var val = getValue(RCON_HASH_ONCE);
				if (val != null)
					return val;
				
				var bytes = new byte [4];
				(new Random ((int) DateTime.Now.Ticks)).NextBytes (bytes);
				val = String.Format ("rcon_{0:x2}{1:x2}{2:x2}{3:x2}", bytes[0], bytes[1], bytes[2], bytes[3]);

                setValue(RCON_HASH_ONCE, val);
				return val;
			}
			set
			{
                Program.properties.setValue(RCON_HASH_ONCE, value);
			}
		}
		
		public bool LogRotation
		{
            get { return getValue(LOG_ROTATION, DEFAULT_LOG_ROTATION); }
            set { setValue(LOG_ROTATION, value); }
		}
		
		public int SpawnNPCMax
		{
            get { return getValue(SPAWN_NPC_MAX, DEFAULT_SPAWN_NPC_MAX); }
            set { setValue(SPAWN_NPC_MAX, value); }
		}
		
		public bool AllowExplosions
		{
            get { return getValue(ALLOW_EXPLOSIONS, DEFAULT_ALLOW_EXPLOSIONS); }
            set { setValue(ALLOW_EXPLOSIONS, value); }
		}

        public string RejectedItems
		{
            get { return getValue(REJECT_ITEMS, DEFAULT_REJECT_ITEMS); }
            set { setValue(REJECT_ITEMS, value); }
		}
		
		public int OverlimitSlots
		{
            get { return getValue(OVERLIMIT_SLOTS, DEFAULT_OVERLIMIT_SLOTS); }
            set { setValue(OVERLIMIT_SLOTS, value); }
		}
		
		public bool BufferLiquidUpdates
		{
            get { return getValue(BUFFER_LIQUID_UPDATES, DEFAULT_BUFFER_LIQUID_UPDATES); }
            set { setValue(BUFFER_LIQUID_UPDATES, value); }
		}
		
		public bool StopUpdatesWhenEmpty
		{
            get { return getValue(STOP_UPDATES_WHEN_EMPTY, DEFAULT_STOP_UPDATES_WHEN_EMPTY); }
            set { setValue(STOP_UPDATES_WHEN_EMPTY, value); }
		}
		
		public int MaxRespawnTime
		{
            get { return getValue(MAX_RESPAWNTIME, 0); }
            set { setValue(MAX_RESPAWNTIME, value); }
		}
		
		// TODO: cache this somehow so we weren't doing string comparisons
		public string HardcoreDeathAction
		{
            get { return getValue(HARDCORE_DEATH_ACTION, DEFAULT_HARDCORE_DEATH_ACTION); }
            set { setValue(HARDCORE_DEATH_ACTION, value); }
		}
		
		// TODO: cache this somehow so we weren't doing string comparisons
		public string TileSquareMessages
		{
            get { return getValue(TILESQUARE_MESSAGES, DEFAULT_TILESQUARE_MESSAGES); }
            set { setValue(TILESQUARE_MESSAGES, value); }
		}

        public int SendQueueQuota
        {
            get { return getValue(SEND_QUEUE_QUOTA, DEFAULT_SEND_QUEUE_QUOTA); }
            set { setValue(SEND_QUEUE_QUOTA, value); }
        }

        public bool StopNPCSpawning
        {
            get { return getValue(STOP_NPC_SPAWNING, DEFAULT_STOP_NPC_SPAWNING); }
            set { setValue(STOP_NPC_SPAWNING, value); }
        }

        public int ExitAccessLevel
        {
            get { return getValue(EXIT_USERS, DEFAULT_EXIT_USERS); }
            set { setValue(EXIT_USERS, value); }
        }

		public bool AllowTDCMRPG
		{
			get { return getValue(ALLOW_TDCMRPG, DEFAULT_ALLOW_TDCMRPG); }
			set { setValue(ALLOW_TDCMRPG, value); }
		}

		public bool CollectGarbage
		{
			get { return getValue(COLLECT_GARBAGE, DEFAULT_COLLECT_GARBAGE); }
			set { setValue(COLLECT_GARBAGE, value); }
		}

		public bool AlwaysGenerateSnow
		{
			get { return getValue(ALWAYS_GENERATE_SNOW, DEFAULT_ALWAYS_GENERATE_SNOW); }
			set { setValue(ALWAYS_GENERATE_SNOW, value); }
		}

		public bool GenerateJungle
		{
			get { return getValue(GENERATE_JUNGLE, DEFAULT_GENERATE_JUNGLE); }
			set { setValue(GENERATE_JUNGLE, value); }
		}

		public bool GenerateSnow
		{
			get { return getValue(GENERATE_SNOW, DEFAULT_GENERATE_SNOW); }
			set { setValue(GENERATE_SNOW, value); }
		}

		//Cache me
		public bool NPCSpawnsOverride
		{
			get { return getValue(NPCSPAWN_OVERRIDE, DEFAULT_NPCSPAWN_OVERRIDE); }
			set { setValue(NPCSPAWN_OVERRIDE, value); }
		}

		public int BackupInterval
		{
			get { return getValue(BACKUP_MINUTE_INTERVAL, DEFAULT_BACKUP_MINUTE_INTERVAL); }
			set { setValue(BACKUP_MINUTE_INTERVAL, value); }
		}

		public bool AllowBackups
		{
			get { return getValue(ALLOW_BACKUPS, DEFAULT_ALLOW_BACKUPS); }
			set { setValue(ALLOW_BACKUPS, value); }
		}

		public int PurgeBackupsMinutes
		{
			get { return getValue(PURGE_BACKUPS_AFTER, DEFAULT_PURGE_BACKUPS_AFTER); }
			set { setValue(PURGE_BACKUPS_AFTER, value); }
		}
    }
}
