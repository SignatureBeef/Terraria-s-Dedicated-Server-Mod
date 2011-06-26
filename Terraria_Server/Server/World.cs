
using System;
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

        public string SavePath { get; set; }

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

        public double getTime()
        {
            return Main.time;
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
        }

        public int getMaxTilesY()
        {
            return maxTilesY;
        }

        public void setMaxTilesY(int MaxTilesY, bool updateSection = true)
        {
            maxTilesY = MaxTilesY;
            if (updateSection)
            {
                UpdateWorldCoords(false);
            }
        }

        public int getMaxTilesX()
        {
            return maxTilesX;
        }

        public void setMaxTilesX(int MaxTilesX, bool updateSection = true)
        {
            maxTilesX = MaxTilesX;
            if (updateSection)
            {
                UpdateWorldCoords(false);
            }
        }

        public Tile getHighestTile(int axisX) //tile format?
        {
            int tallest = -1;

            for (int i = 0; i < maxTilesY; i++)
            {
                if (Server.tile[axisX, i] != null && Server.tile[axisX, i].active)
                {
                    if (i > tallest)
                    {
                        tallest = i;
                    }
                }
            }
            if (tallest >= 0)
            {
                return WorldGen.cloneTile(Server.tile[axisX, tallest], axisX, tallest);
            }
            return null;
        }

    }

}
