using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace TDSM.Core
{
    public enum WorldSize
    {
        Small,
        Medium,
        Large
    }

    public static class World
    {
        public static bool IsLarge(this WorldSize size)
        {
            return size == WorldSize.Large;
        }

        public static bool IsMedium(this WorldSize size)
        {
            return size == WorldSize.Medium;
        }

        public static bool IsSmall(this WorldSize size)
        {
            return size == WorldSize.Small;
        }

        public static WorldSize GetWorldSize()
        {
            if (Terraria.Main.maxTilesY <= 1200)
                return WorldSize.Small;
            if (Terraria.Main.maxTilesY <= 1800)
                return WorldSize.Medium;
            return WorldSize.Large;
        }

        public static void SetParsedTime(double time)
        {
            var dayTime = time >= 0.0 && time <= 54000.0;

            //If our parsed time is now into night time, remove the 15 hours of day time and set the day flag to false
            if (time > 54000.0)
            {
                time -= 54000.0;
                dayTime = false;
            }

            SetTime(time, dayTime);
        }

        public static double GetParsableTime()
        {
            var time = Terraria.Main.time;

            if (!Terraria.Main.dayTime)
                time += 54000.0;

            return time;
        }

        public static void SetTime(double time, bool dayTime = true)
        {
            Terraria.Main.time = time;
            Terraria.Main.dayTime = dayTime;

            if (Terraria.Main.bloodMoon)
                Terraria.Main.bloodMoon = false;

            if (Terraria.Main.eclipse)
                Terraria.Main.eclipse = false;
        }

        public static bool IsTileValid(int x, int y)
        {
            return (x >= 0 && x <= Main.maxTilesX && y >= 0 && y <= Main.maxTilesY);
        }

        public static bool IsTileClear(int x, int y)
        {
            #if MemTile
            return Main.tile[x, y] == TDSM.API.Memory.MemTile.Empty || !Main.tile[x, y].active() || Main.tile[x, y].inActive();
            #else
            return Main.tile[x, y] == null || !Main.tile[x, y].active() || Main.tile[x, y].inActive();
            #endif
        }

//        public static bool IsTileTheSame(Tile tile1, Tile tile2)
//        {
//            if (tile1.active() != tile2.active())
//                return false;
//
//            if (tile1.active())
//            {
//                if (tile1.type != tile2.type)
//                    return false;
//
//                if (Main.tileFrameImportant[(int)tile1.type])
//                {
//                    if ((tile1.frameX != tile2.frameX) || (tile1.frameX != tile2.frameX))
//                        return false;
//                }
//            }
//            return
//                tile1.wall == tile2.wall
//            &&
//            tile1.liquid == tile2.liquid
//            &&
//            tile1.lava() == tile2.lava()
//            &&
//            tile1.wire() == tile2.wire()
//            &&
//            tile1.wire2() == tile2.wire2()
//            &&
//            tile1.wire3() == tile2.wire3();
//        }

        public static Vector2 GetRandomClearTile(float x, float y, int attempts = 1, int rangeY = 50, int rangeX = 50)
        {
            return GetRandomClearTile((int)x, (int)y, attempts, rangeY, rangeX);
        }

        public static Vector2 GetRandomClearTile(int x, int y, int attempts = 1, int rangeX = 50, int rangeY = 50)
        {
            Vector2 tileLocation = new Vector2(0, 0);
            try
            {
                if (Main.rand == null)
                    Main.rand = new Random();

//                if (!forceRange)
//                {
//                    rangeX = (Main.tile.GetLength(0)) - x;
//                    rangeY = (Main.tile.GetLength(1)) - y;
//                }

                for (int i = 0; i < attempts; i++)
                {
                    tileLocation.X = x + ((Main.rand.Next(rangeX * -1, rangeX)) / 2);
                    tileLocation.Y = y + ((Main.rand.Next(rangeY * -1, rangeY)) / 2);
                    if ((IsTileValid((int)tileLocation.X, (int)tileLocation.Y) &&
                        IsTileClear((int)tileLocation.X, (int)tileLocation.Y)))
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }

            if (tileLocation.X == 0 && tileLocation.Y == 0)
                return new Vector2(x, y);

            return tileLocation;
        }
    }
}
