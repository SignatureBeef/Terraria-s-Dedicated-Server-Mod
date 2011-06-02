using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class World
    {
        /*public static int[] getWorldSize(String size) {
            string nSize = size.ToLower().Trim();
            if (nSize.Equals("small"))
            {
                return new int[] { 4200, 1200};
            }
            else if (nSize.Equals("medium"))
            {
                
                return new int[] { 6300, 1800};
            }
            else if (nSize.Equals("large"))
            {
                return new int[] { 8400, 2400};
            }
            else
            {
                return null;
            }
        }*/


        private string name = "";
        private Server server = null;
        private int seed = -1; //default
        private int id = 0;
        private int moonPhase = 0;
        private double time = 0;
        private bool dayTime = true;
        private bool bloodMoon = false;
        private double worldSurface;
        private double rockLayer;
        private short sunModY = 0;
        private short moonModY = 0;
        private bool allowNPCSpawns = true;
        private int dungeonX;
        private int dungeonY;
        public Chest[] shop = null;

        private Tile[,] tile = null;
        private Player[] player = null;
        private Dust[] dust = null; //new Dust[2000];
        //private Star[] star = null; //new Star[130];
        private Item[] item = null; //new Item[201];
        private NPC[] npc = null; //new NPC[1001];
        private Gore[] gore = null; //new Gore[201];
        private Projectile[] projectile = null; //new Projectile[1001];
        //private static CombatText[] combatText = null; //new CombatText[100];
        private Chest[] chest = null; //new Chest[1000];
        private Sign[] sign = null; //new Sign[1000];
        public Liquid[] liquid = null; //new Liquid[Liquid.resLiquid];
        public LiquidBuffer[] liquidBuffer = null; //new LiquidBuffer[10000];



        private int shadowOrbCount = 0;
        private bool shadowOrbSmashed = false;
        private bool spawnMeteor = false;
        private int invasionSize = 0;
        private int invasionDelay = 0;
        private int invasionType = 0;
        private int invasionWarn = 0;
        private double invasionX = 0.0;
        private bool spawnEye = false;
        private bool stopDrops = false;
        private int spawnNPC = 0;


        public World(int maxTilesX, int maxTilesY)
        {
            tile = new Tile[maxTilesX, maxTilesY];
            player = new Player[9]; //use player cap?
            //item = new Item[201];
            projectile = new Projectile[1001];
            npc = new NPC[1001];
            item = new Item[201];
            shop = new Chest[5];
            chest = new Chest[1000];
            sign = new Sign[1000];
            liquid = new Liquid[Liquid.resLiquid];
            liquidBuffer = new LiquidBuffer[10000];
            dust = new Dust[2000];
            //star = new Star[130];
            gore = new Gore[201];
        }

        public World(Server Server, int maxTilesX, int maxTilesY)
        {
            tile = new Tile[maxTilesX, maxTilesY];
            player = new Player[9]; //use player cap?
            //item = new Item[201];
            projectile = new Projectile[1001];
            npc = new NPC[1001];
            item = new Item[201];
            chest = new Chest[1000];
            sign = new Sign[1000];
            shop = new Chest[5];
            liquid = new Liquid[Liquid.resLiquid];
            liquidBuffer = new LiquidBuffer[10000];
            dust = new Dust[2000];
            //star = new Star[130];
            gore = new Gore[201];
            server = Server;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string Name)
        {
            name = Name;
        }

        public Tile[,] getTile()
        {
            return tile;
        }

        public void setTile(Tile[,] Tiles)
        {
            tile = Tiles;
        }
        public void setTile(Tile Tile, int X, int Y)
        {
            tile[X, Y] = Tile;
        }
    
        public Server getServer()
        {
            return server;
        }

        public void setServer(Server Server)
        {
            server = Server;
        }

        public int getSeed()
        {
            return seed;
        }

        public void setSeed(int Seed)
        {
            seed = Seed;
        }

        public int getId()
        {
            return id;
        }

        public void setId(int ID)
        {
            id = ID;
        }

        public int getMoonPhase()
        {
            return moonPhase;
        }

        public void setMoonPhase(int MoonPhase)
        {
            moonPhase = MoonPhase;
        }
        
        public double getTime()
        {
            return time;
        }

        public void setTime(double Time)
        {
            time = Time;
        }

        public bool isDayTime()
        {
            return dayTime;
        }

        public void setDayTime(bool Day)
        {
            dayTime = Day;
        }

        public bool isBloodMoon()
        {
            return bloodMoon;
        }

        public void setBloodMoon(bool BloodMoon)
        {
            bloodMoon = BloodMoon;
        }

        public double getWorldSurface()
        {
            return worldSurface;
        }

        public void setWorldSurface(double WorldSurface)
        {
            worldSurface = WorldSurface;
        }

        public double getRockLayer()
        {
            return rockLayer;
        }

        public void setRockLayer(double RockLayer)
        {
            rockLayer = RockLayer;
        }

        public Chest[] getChests()
        {
            return chest;
        }

        public void setChests(Chest[] Chests)
        {
            chest = Chests;
        }

        public NPC[] getNPCs()
        {
            return npc;
        }

        public void setNPCs(NPC[] NPC)
        {
            npc = NPC;
        }

        public Projectile[] getProjectile()
        {
            return projectile;
        }

        public void setProjectile(Projectile[] Projectile)
        {
            projectile = Projectile;
        }

        public Item[] getItemList()
        {
            return item;
        }

        public void setItem(int itemIndex, Item Item)
        {
            item[itemIndex] = Item;
        }

        public Player[] getPlayerList()
        {
            return player;
        }

        public void setPlayerList(Player[] Player)
        {
            player = Player;
        }

        public Sign[] getSigns()
        {
            return sign;
        }

        public void setSigns(Sign[] Sign)
        {
            sign = Sign;
        }

        public short getMoonModY()
        {
            return moonModY;
        }

        public void setMoonModY(short MoonModY)
        {
            moonModY = MoonModY;
        }

        public short getSunModY()
        {
            return sunModY;
        }

        public void setSunModY(short SunModY)
        {
            sunModY = SunModY;
        }

        public bool isNPCSpawning()
        {
            return allowNPCSpawns;
        }

        public void setNPCSpawning(bool AllowNPCSpawning)
        {
            allowNPCSpawns = AllowNPCSpawning;
        }

        public int getDungeonX()
        {
            return dungeonX;
        }

        public void setDungeonX(int DungeonX)
        {
            dungeonX = DungeonX;
        }

        public int getDungeonY()
        {
            return dungeonY;
        }

        public void setDungeonY(int DungeonY)
        {
            dungeonY = DungeonY;
        }

        public int getShadowOrbCount()
        {
            return shadowOrbCount;
        }

        public void setShadowOrbCount(int ShadowOrbCount)
        {
            shadowOrbCount = ShadowOrbCount;
        }

        public bool getShadowOrbSmashed()
        {
            return shadowOrbSmashed;
        }

        public void setShadowOrbSmashed(bool ShadowOrbSmashed)
        {
            shadowOrbSmashed = ShadowOrbSmashed;
        }

        public bool getSpawnMeteor()
        {
            return spawnMeteor;
        }

        public void setSpawnMeteor(bool SetSpawnMeteor)
        {
            spawnMeteor = SetSpawnMeteor;
        }

        public int getInvasionSize()
        {
            return invasionSize;
        }

        public void setInvasionSize(int InvasionSize)
        {
            invasionSize = InvasionSize;
        }

        public int getInvasionDelay()
        {
            return invasionDelay;
        }

        public void setInvasionDelay(int InvasionDelay)
        {
            invasionDelay = InvasionDelay;
        }

        public int getInvasionType()
        {
            return invasionType;
        }

        public void setInvasionType(int InvasionType)
        {
            invasionType = InvasionType;
        }

        public double getInvasionX()
        {
            return invasionX;
        }

        public void setInvasionX(double InvasionX)
        {
            invasionX = InvasionX;
        }

        public int getInvasionWarn()
        {
            return invasionWarn;
        }

        public void setInvasionWarn(int InvasionWarn)
        {
            invasionWarn = InvasionWarn;
        }

        public bool getSpawnEye()
        {
            return spawnEye;
        }

        public void setSpawnEye(bool SpawnEye)
        {
            spawnEye = SpawnEye;
        }

        public bool getStopDrops()
        {
            return stopDrops;
        }

        public void setStopDrops(bool StopDrops)
        {
            stopDrops = StopDrops;
        }

        public int getSpawnNPC()
        {
            return spawnNPC;
        }

        public void setSpawnNPC(int SpawnNPC)
        {
            spawnNPC = SpawnNPC;
        }

        public Liquid[] getLiquid()
        {
            return liquid;
        }

        public void setLiquid(Liquid[] Liquid)
        {
            liquid = Liquid;
        }
        
        public LiquidBuffer[] getLiquidBuffer()
        {
            return liquidBuffer;
        }

        public void setLiquidBuffer(LiquidBuffer[] LiquidBuffer)
        {
            liquidBuffer = LiquidBuffer;
        }
        
        public Chest[] getShops()
        {
            return shop;
        }

        public void setShops(Chest[] Shops)
        {
            shop = Shops;
        }
        
        public Dust[] getDust()
        {
            return dust;
        }

        public void setDust(Dust[] Dust)
        {
            dust = Dust;
        }

        //public Star[] getStars()
        //{
        //    return star;
        //}

        //public void setStars(Star[] Stars)
        //{
        //    star = Stars;
        //}
       
        public Gore[] getGore()
        {
            return gore;
        }

        public void setGore(Gore[] Gore)
        {
            gore = Gore;
        }

    }

}
