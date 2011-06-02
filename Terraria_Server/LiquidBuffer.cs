using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class LiquidBuffer
    {
        public const int maxLiquidBuffer = 0x2710;
        public static int numLiquidBuffer;
        public int x;
        public int y;

        public static void AddBuffer(int x, int y, World world)
        {
            if ((numLiquidBuffer != 0x270f) && !world.getTile()[x, y].checkingLiquid)
            {
                world.getTile()[x, y].checkingLiquid = true;
                world.getLiquidBuffer()[numLiquidBuffer].x = x;
                world.getLiquidBuffer()[numLiquidBuffer].y = y;
                numLiquidBuffer++;
            }
        }

        public static void DelBuffer(int l, World world)
        {
            numLiquidBuffer--;
            world.getLiquidBuffer()[l].x = world.getLiquidBuffer()[numLiquidBuffer].x;
            world.getLiquidBuffer()[l].y = world.getLiquidBuffer()[numLiquidBuffer].y;
        }
    }
}
