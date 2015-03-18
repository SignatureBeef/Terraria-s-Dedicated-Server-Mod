using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace tdsm.core
{
    public static class World
    {
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

            if (!Terraria.Main.dayTime) time += 54000.0;

            return time;
        }

        public static void SetTime(double time, bool dayTime = true)
        {
            Terraria.Main.time = time;
            Terraria.Main.dayTime = dayTime;

            if (Terraria.Main.bloodMoon)
                Terraria.Main.bloodMoon = false;
        }

        public static bool IsTileValid(int x, int y)
        {
            return (x >= 0 && x <= Main.maxTilesX && y >= 0 && y <= Main.maxTilesY);
        }

        public static bool IsTileClear(int x, int y)
        {
            return !Main.tile[x, y].active();
        }

        public static bool IsTileTheSame(Tile tile1, Tile tile2)
        {
            if (tile1.active() != tile2.active())
                return false;

            if (tile1.active())
            {
                if (tile1.type != tile2.type)
                    return false;

                if (Main.tileFrameImportant[(int)tile1.type])
                {
                    if ((tile1.frameX != tile2.frameX) || (tile1.frameX != tile2.frameX))
                        return false;
                }
            }
            return
                tile1.wall == tile2.wall
                &&
                tile1.liquid == tile2.liquid
                &&
                tile1.lava() == tile2.lava()
                &&
                tile1.wire() == tile2.wire()
                &&
                tile1.wire2() == tile2.wire2()
                &&
                tile1.wire3() == tile2.wire3();
        }

        public static Vector2 GetRandomClearTile(int x, int y, int Attempts, bool forceRange = false, int RangeX = 0, int RangeY = 0)
        {
            Vector2 tileLocation = new Vector2(0, 0);
            try
            {
                if (Main.rand == null)
                    Main.rand = new Random();

                if (!forceRange)
                {
                    RangeX = (Main.tile.GetLength(0)) - x;
                    RangeY = (Main.tile.GetLength(1)) - y;
                }

                for (int i = 0; i < Attempts; i++)
                {
                    tileLocation.X = x + ((Main.rand.Next(RangeX * -1, RangeX)) / 2);
                    tileLocation.Y = y + ((Main.rand.Next(RangeY * -1, RangeY)) / 2);
                    if ((IsTileValid((int)tileLocation.X, (int)tileLocation.Y) &&
                        IsTileClear((int)tileLocation.X, (int)tileLocation.Y)))
                    {
                        break;
                    }
                }
            }
            catch (Exception) { }

            if (tileLocation.X == 0 && tileLocation.Y == 0)
                return new Vector2(x, y);

            return tileLocation;
        }
    }
}
