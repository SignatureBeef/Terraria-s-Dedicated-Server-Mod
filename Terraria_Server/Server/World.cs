using System;
using Terraria_Server.Misc;
using Terraria_Server.Commands;

namespace Terraria_Server
{
    public static class World
    {
		public static readonly ISender Sender = new WorldSender ();
		public static string SavePath { get; set; }
		
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
		
        public static void SetTime(double Time, bool baseDay = false, bool dayTime = true)
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
		
        public static bool IsTileValid(int TileX, int TileY)
        {
            return (TileX >= 0 && TileX <= Main.maxTilesX && TileY >= 0 && TileY <= Main.maxTilesY);
        }

        public static bool IsTileClear(int TileX, int TileY)
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
                    if ((World.IsTileValid((int)tileLocation.X, (int)tileLocation.Y) && 
                        World.IsTileClear((int)tileLocation.X, (int)tileLocation.Y)))
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
    }
}
