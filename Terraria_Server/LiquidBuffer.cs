using System;

namespace Terraria_Server
{
    public class LiquidBuffer
    {
        public const int maxLiquidBuffer = 10000;
        public static int numLiquidBuffer;
        public int x;
        public int y;

        public static void AddBuffer(int x, int y)
        {
            if (LiquidBuffer.numLiquidBuffer == 9999)
            {
                return;
            }
            if (Main.tile[x, y].CheckingLiquid)
            {
                return;
            }
            Main.tile[x, y].CheckingLiquid = true;
            Main.liquidBuffer[LiquidBuffer.numLiquidBuffer].x = x;
            Main.liquidBuffer[LiquidBuffer.numLiquidBuffer].y = y;
            LiquidBuffer.numLiquidBuffer++;
        }

        public static void DelBuffer(int l)
        {
            LiquidBuffer.numLiquidBuffer--;
            Main.liquidBuffer[l].x = Main.liquidBuffer[LiquidBuffer.numLiquidBuffer].x;
            Main.liquidBuffer[l].y = Main.liquidBuffer[LiquidBuffer.numLiquidBuffer].y;
        }
    }
}
