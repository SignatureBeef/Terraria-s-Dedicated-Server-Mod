using System;
using Terraria_Server.Misc;
namespace Terraria_Server
{
    public class World
    {

        public enum MAP_SIZE : int
        {
            SMALL_X = 4200,
            MEDIUM_X = 6300,
            LARGE_X = 8400,
            SMALL_Y = 1200,
            MEDIUM_Y = 1800,
            LARGE_Y = 2400
        }

        private const float DEFAULT_LEFT_WORLD = 0.0f;
        private const float DEFAULT_RIGHT_WORLD = 134400f;
        private const float DEFAULT_TOP_WORLD = 0.0f;
        private const float DEFAULT_BOTTOM_WORLD = 38400f;
        
        private int maxTilesX;
        private int maxTilesY;

        public World(int MaxTilesX, int MaxTilesY) : this(null, MaxTilesX, MaxTilesY){}

        public World(Server server, int MaxTilesX, int MaxTilesY)
        {
            BottomWorld = DEFAULT_BOTTOM_WORLD;
            TopWorld = DEFAULT_TOP_WORLD;
            LeftWorld = DEFAULT_LEFT_WORLD;
            RightWorld = DEFAULT_RIGHT_WORLD;
            maxTilesY = MaxTilesY;
            maxTilesX = MaxTilesX;

            Server = server;

            UpdateWorldCoords(false);
        }

        public String Name { get; set; }

        public Server Server { get; set; }

        public int Seed { get; set; }

        public int Id { get; set; }

        public String SavePath { get; set; }

        public int MaxSectionsY { get; set; }

        public int MaxSectionsX { get; set; }

        public float BottomWorld { get; set; }

        public float TopWorld { get; set; }

        public float RightWorld { get; set; }

        public float LeftWorld { get; set; }

        public void UpdateWorldCoords(bool useWorld) {
            if (useWorld)
            {
                maxTilesX = (int)RightWorld / 16 + 1;
                maxTilesY = (int)BottomWorld / 16 + 1;
            }
            MaxSectionsX = maxTilesX / 200;
            MaxSectionsY = maxTilesY / 150;
        }

        public double Time
        {
            get
            {
                return Main.time;
            }
        }

        public void setTime(double Time, bool baseDay = false, bool dayTime = true)
        {
            Main.time = Time;
            Main.dayTime = dayTime;

            if (baseDay)
            {
                if (Main.time > Main.dayLength)
                {
                    Main.dayTime = true;
                }
                else
                {
                    Main.dayTime = false;
                }
            }
            if(Main.dayTime)
                Main.bloodMoon = false;
        }

        public int MaxTilesY
        {
            get {
                return maxTilesY;
            }
        }

        public void setMaxTilesY(int MaxTilesY, bool updateSection = true)
        {
            maxTilesY = MaxTilesY;
            if (updateSection)
            {
                UpdateWorldCoords(false);
            }
        }

        public int MaxTilesX
        {
            get
            {
                return maxTilesX;
            }
        }

        public void setMaxTilesX(int MaxTilesX, bool updateSection = true)
        {
            maxTilesX = MaxTilesX;
            if (updateSection)
            {
                UpdateWorldCoords(false);
            }
        }

        public bool isTileValid(int TileX, int TileY)
        {
            return (TileX >= 0 && TileX <= maxTilesX && TileY >= 0 && TileY <= maxTilesY);
        }

        public bool isTileClear(int TileX, int TileY)
        {
            return (!Main.tile.At(TileX, TileY).Active);
        }

        public static Vector2 GetRandomClearTile(int TileX, int TileY, int Attempts, bool forceRange = false, int RangeX = 0, int RangeY = 0)
        {
            Vector2 tileLocation = new Vector2(0, 0);
            try
            {
                if (Main.rand == null)
                {
                    Main.rand = new Random();
                }

                if (!forceRange)
                {
                    RangeX = (Main.tile.SizeX) - TileX;
                    RangeY = (Main.tile.SizeY) - TileY;
                }

                for (int i = 0; i < Attempts; i++)
                {
                    tileLocation.X = TileX + ((Main.rand.Next(RangeX * -1, RangeX)) / 2);
                    tileLocation.Y = TileY + ((Main.rand.Next(RangeY * -1, RangeY)) / 2);
                    if ((Program.server.World.isTileValid((int)tileLocation.X, (int)tileLocation.Y) && 
                        Program.server.World.isTileClear((int)tileLocation.X, (int)tileLocation.Y)))
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {

            }
            if (tileLocation.X == 0 && tileLocation.Y == 0)
            {
                return new Vector2(TileX, TileY);
            }
            return tileLocation;
        }

        public int SpawnX
        {
            get
            {
                return Main.spawnTileX;
            }
            set
            {
                Main.spawnTileX = value;
            }
        }

        public int SpawnY
        {
            get
            {
                return Main.spawnTileY;
            }
            set
            {
                Main.spawnTileY = value;
            }
        }

    }
}
