using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class Lighting
    {
        public const int offScreenTiles = 21;
        public static int lightPasses = 2;
        public static int lightSkip = 1;
        private static float lightColor = 0f;
        public static int lightCounter = 0;
        private static int firstTileX;
        private static int lastTileX;
        private static int firstTileY;
        private static int lastTileY;
        public static float[,] color = new float[Main.screenWidth + 42 + 10, Main.screenHeight + 42 + 10];
        private static int maxTempLights = 2000;
        private static int[] tempLightX = new int[Lighting.maxTempLights];
        private static int[] tempLightY = new int[Lighting.maxTempLights];
        private static float[] tempLight = new float[Lighting.maxTempLights];
        private static int tempLightCount;
        private static int firstToLightX;
        private static int firstToLightY;
        private static int lastToLightX;
        private static int lastToLightY;
        public static bool resize = false;
        private static float negLight = 0.04f;
        private static float negLight2 = 0.16f;

        private static void LightColor(int x, int y)
        {
            int num = x - firstToLightX;
            int num2 = y - firstToLightY;
            try
            {
                TileRef tile = Main.tile.At(x, y);

                if (Lighting.color[num, num2] > Lighting.lightColor)
                {
                    Lighting.lightColor = Lighting.color[num, num2];
                }
                else if (Lighting.lightColor == 0f)
                {
                    return;
                }
                Lighting.color[num, num2] = Lighting.lightColor;

                float num3 = Lighting.negLight;
                if (tile.Active && Main.tileBlockLight[(int)tile.Type])
                {
                    num3 = Lighting.negLight2;
                }
                float num4 = Lighting.lightColor - num3;
                if (num4 < 0f)
                {
                    Lighting.lightColor = 0f;
                }
                else
                {
                    Lighting.lightColor -= num3;
                    if (Lighting.lightColor > 0f && (!tile.Active || !Main.tileSolid[(int)tile.Type]) && (double)y < Main.worldSurface)
                    {
                        Main.tile.At(x, y).SetLighted(true);
                    }
                }

                tile = default(TileRef);
            }
            catch
            {
            }
        }

        public static void LightTiles(int firstX, int lastX, int firstY, int lastY)
        {
            Lighting.firstTileX = firstX;
            Lighting.lastTileX = lastX;
            Lighting.firstTileY = firstY;
            Lighting.lastTileY = lastY;

            if ((Lighting.lightCounter > Lighting.lightSkip) || Lighting.resize)
            {
                Lighting.lightCounter = 0;
                Lighting.resize = false;
                Lighting.firstToLightX = Lighting.firstTileX - 21;
                Lighting.firstToLightY = Lighting.firstTileY - 21;
                Lighting.lastToLightX = Lighting.lastTileX + 21;
                Lighting.lastToLightY = Lighting.lastTileY + 21;
                for (int i = 0; i < Main.screenWidth / 16 + 42 + 10; i++)
                {
                    for (int j = 0; j < Main.screenHeight / 16 + 42 + 10; j++)
                    {
                        Lighting.color[i, j] = 0f;
                    }
                }
                for (int k = 0; k < Lighting.tempLightCount; k++)
                {
                    if (Lighting.tempLightX[k] - Lighting.firstTileX + 21 >= 0 && Lighting.tempLightX[k] - Lighting.firstTileX + 21 < Main.screenWidth / 16 + 42 + 10 && Lighting.tempLightY[k] - Lighting.firstTileY + 21 >= 0 && Lighting.tempLightY[k] - Lighting.firstTileY + 21 < Main.screenHeight / 16 + 42 + 10)
                    {
                        Lighting.color[Lighting.tempLightX[k] - Lighting.firstTileX + 21, Lighting.tempLightY[k] - Lighting.firstTileY + 21] = Lighting.tempLight[k];
                    }
                }
                Lighting.tempLightCount = 0;
                Main.evilTiles = 0;
                Main.meteorTiles = 0;
                Main.jungleTiles = 0;
                Main.dungeonTiles = 0;
                for (int x = Lighting.firstToLightX; x < Lighting.lastToLightX; x++)
                {
                    for (int y = Lighting.firstToLightY; y < Lighting.lastToLightY; y++)
                    {
                        if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
                        {
                            if (Main.tile.At(x, y).Active)
                            {
                                if (Main.tile.At(x, y).Type == 23 || Main.tile.At(x, y).Type == 24 || Main.tile.At(x, y).Type == 25 || Main.tile.At(x, y).Type == 32)
                                {
                                    Main.evilTiles++;
                                }
                                else
                                {
                                    if (Main.tile.At(x, y).Type == 27)
                                    {
                                        Main.evilTiles -= 5;
                                    }
                                    else
                                    {
                                        if (Main.tile.At(x, y).Type == 37)
                                        {
                                            Main.meteorTiles++;
                                        }
                                        else
                                        {
                                            if (Main.tileDungeon[(int)Main.tile.At(x, y).Type])
                                            {
                                                Main.dungeonTiles++;
                                            }
                                            else
                                            {
                                                if (Main.tile.At(x, y).Type == 60 || Main.tile.At(x, y).Type == 61 || Main.tile.At(x, y).Type == 62 || Main.tile.At(x, y).Type == 74)
                                                {
                                                    Main.jungleTiles++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (Main.tile.At(x, y).Lava)
                            {
                                float num = (float)(Main.tile.At(x, y).Liquid / 255) * 0.4f + 0.1f;
                                if (Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < num)
                                {
                                    Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = num;
                                }
                            }
                            if ((!Main.tile.At(x, y).Active || !Main.tileSolid[(int)Main.tile.At(x, y).Type] || Main.tile.At(x, y).Type == 22 || Main.tile.At(x, y).Type == 37 || Main.tile.At(x, y).Type == 58 || Main.tile.At(x, y).Type == 70 || Main.tile.At(x, y).Type == 76) && (Main.tile.At(x, y).Lighted || Main.tile.At(x, y).Type == 4 || Main.tile.At(x, y).Type == 17 || Main.tile.At(x, y).Type == 31 || Main.tile.At(x, y).Type == 33 || Main.tile.At(x, y).Type == 34 || Main.tile.At(x, y).Type == 35 || Main.tile.At(x, y).Type == 36 || Main.tile.At(x, y).Type == 37 || Main.tile.At(x, y).Type == 42 || Main.tile.At(x, y).Type == 49 || Main.tile.At(x, y).Type == 58 || Main.tile.At(x, y).Type == 61 || Main.tile.At(x, y).Type == 70 || Main.tile.At(x, y).Type == 71 || Main.tile.At(x, y).Type == 72 || Main.tile.At(x, y).Type == 76 || Main.tile.At(x, y).Type == 77 || Main.tile.At(x, y).Type == 19 || Main.tile.At(x, y).Type == 22 || Main.tile.At(x, y).Type == 26 || Main.tile.At(x, y).Type == 84 || Main.tile.At(x, y).Type == 92 || Main.tile.At(x, y).Type == 93 || Main.tile.At(x, y).Type == 95 || Main.tile.At(x, y).Type == 98 || Main.tile.At(x, y).Type == 100))
                            {
                                if (Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] * 255f < (float)Main.tileColor.R && (float)Main.tileColor.R > Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] * 255f && Main.tile.At(x, y).Wall == 0 && (double)y < Main.worldSurface && Main.tile.At(x, y).Liquid < 200)
                                {
                                    Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = (float)Main.tileColor.R / 255f;
                                }
                                if (Main.tile.At(x, y).Type == 93 && Main.tile.At(x, y).FrameY == 0)
                                {
                                    Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 1f;
                                }
                                else
                                {
                                    if (Main.tile.At(x, y).Type == 98 && Main.tile.At(x, y).FrameY == 0)
                                    {
                                        Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 1f;
                                    }
                                    else
                                    {
                                        if (Main.tile.At(x, y).Type == 100 && Main.tile.At(x, y).FrameY == 0)
                                        {
                                            Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 1f;
                                        }
                                        else
                                        {
                                            if (Main.tile.At(x, y).Type == 92 && Main.tile.At(x, y).FrameY <= 18)
                                            {
                                                Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 1f;
                                            }
                                            else
                                            {
                                                if (Main.tile.At(x, y).Type == 4 || Main.tile.At(x, y).Type == 33 || Main.tile.At(x, y).Type == 34 || Main.tile.At(x, y).Type == 35 || Main.tile.At(x, y).Type == 36 || Main.tile.At(x, y).Type == 98)
                                                {
                                                    Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 1f;
                                                }
                                                else
                                                {
                                                    if (Main.tile.At(x, y).Type == 17 && Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < 0.8f)
                                                    {
                                                        Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 0.8f;
                                                    }
                                                    else
                                                    {
                                                        if (Main.tile.At(x, y).Type == 77 && Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < 0.8f)
                                                        {
                                                            Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 0.8f;
                                                        }
                                                        else
                                                        {
                                                            if (Main.tile.At(x, y).Type == 37 && Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < 0.6f)
                                                            {
                                                                Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 0.6f;
                                                            }
                                                            else
                                                            {
                                                                if (Main.tile.At(x, y).Type == 22 && Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < 0.2f)
                                                                {
                                                                    Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 0.2f;
                                                                }
                                                                else
                                                                {
                                                                    if (Main.tile.At(x, y).Type == 42 && Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < 0.75f)
                                                                    {
                                                                        Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 0.75f;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.tile.At(x, y).Type == 95 && Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < 0.85f)
                                                                        {
                                                                            Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 0.85f;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Main.tile.At(x, y).Type == 49 && Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < 0.75f)
                                                                            {
                                                                                Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 0.75f;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Main.tile.At(x, y).Type == 70 || Main.tile.At(x, y).Type == 71 || Main.tile.At(x, y).Type == 72)
                                                                                {
                                                                                    float num2 = (float)Main.rand.Next(48, 52) * 0.01f;
                                                                                    if (Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < num2)
                                                                                    {
                                                                                        Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = num2;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (Main.tile.At(x, y).Type == 61 && Main.tile.At(x, y).FrameX == 144 && Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < 0.75f)
                                                                                    {
                                                                                        Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 0.75f;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Main.tile.At(x, y).Type == 31 || Main.tile.At(x, y).Type == 26)
                                                                                        {
                                                                                            float num3 = (float)Main.rand.Next(-5, 6) * 0.01f;
                                                                                            if (Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < 0.4f + num3)
                                                                                            {
                                                                                                Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = 0.4f + num3;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Main.tile.At(x, y).Type == 84)
                                                                                            {
                                                                                                int num4 = (int)(Main.tile.At(x, y).FrameX / 18);
                                                                                                float num5 = 0f;
                                                                                                //if (num4 == 2)
                                                                                                //{
                                                                                                //    float num6 = (float)(260 - (int)Main.mouseTextColor);
                                                                                                //    num6 /= 200f;
                                                                                                //    if (num6 > 1f)
                                                                                                //    {
                                                                                                //        num6 = 1f;
                                                                                                //    }
                                                                                                //    if (num6 < 0f)
                                                                                                //    {
                                                                                                //        num6 = 0f;
                                                                                                //    }
                                                                                                //    num5 = num6;
                                                                                                //}
                                                                                                if (num4 == 5)
                                                                                                {
                                                                                                    num5 = 0.7f;
                                                                                                }
                                                                                                if (Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] < num5)
                                                                                                {
                                                                                                    Lighting.color[x - Lighting.firstToLightX, y - Lighting.firstToLightY] = num5;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Lighting.negLight = 0.04f;
                Lighting.negLight2 = 0.16f;
                //if (Main.player[Main.myPlayer].nightVision)
                //{
                //    Lighting.negLight -= 0.013f;
                //    Lighting.negLight2 -= 0.04f;
                //}
                //if (Main.player[Main.myPlayer].blind)
                //{
                //    Lighting.negLight += 0.03f;
                //    Lighting.negLight2 += 0.06f;
                //}
                for (int n = 0; n < Lighting.lightPasses; n++)
                {
                    for (int num7 = Lighting.firstToLightX; num7 < Lighting.lastToLightX; num7++)
                    {
                        Lighting.lightColor = 0f;
                        for (int num8 = Lighting.firstToLightY; num8 < Lighting.lastToLightY; num8++)
                        {
                            Lighting.LightColor(num7, num8);
                        }
                    }
                    for (int num9 = Lighting.firstToLightX; num9 < Lighting.lastToLightX; num9++)
                    {
                        Lighting.lightColor = 0f;
                        for (int num10 = Lighting.lastToLightY; num10 >= Lighting.firstToLightY; num10--)
                        {
                            Lighting.LightColor(num9, num10);
                        }
                    }
                    for (int num11 = Lighting.firstToLightY; num11 < Lighting.lastToLightY; num11++)
                    {
                        Lighting.lightColor = 0f;
                        for (int num12 = Lighting.firstToLightX; num12 < Lighting.lastToLightX; num12++)
                        {
                            Lighting.LightColor(num12, num11);
                        }
                    }
                    for (int num13 = Lighting.firstToLightY; num13 < Lighting.lastToLightY; num13++)
                    {
                        Lighting.lightColor = 0f;
                        for (int num14 = Lighting.lastToLightX; num14 >= Lighting.firstToLightX; num14--)
                        {
                            Lighting.LightColor(num14, num13);
                        }
                    }
                }
                return;
            }
            Lighting.tempLightCount = 0;
            int num15 = Main.screenWidth / 16 + 42 + 10;
            int num16 = Main.screenHeight / 16 + 42 + 10;
            if ((int)(Main.screenPosition.X / 16f) < (int)(Main.screenLastPosition.X / 16f))
            {
                for (int num17 = num15 - 1; num17 > 1; num17--)
                {
                    for (int num18 = 0; num18 < num16; num18++)
                    {
                        Lighting.color[num17, num18] = Lighting.color[num17 - 1, num18];
                    }
                }
            }
            else
            {
                if ((int)(Main.screenPosition.X / 16f) > (int)(Main.screenLastPosition.X / 16f))
                {
                    for (int num19 = 0; num19 < num15 - 1; num19++)
                    {
                        for (int num20 = 0; num20 < num16; num20++)
                        {
                            Lighting.color[num19, num20] = Lighting.color[num19 + 1, num20];
                        }
                    }
                }
            }
            if ((int)(Main.screenPosition.Y / 16f) < (int)(Main.screenLastPosition.Y / 16f))
            {
                for (int num21 = num16 - 1; num21 > 1; num21--)
                {
                    for (int num22 = 0; num22 < num15; num22++)
                    {
                        Lighting.color[num22, num21] = Lighting.color[num22, num21 - 1];
                    }
                }
                return;
            }
            if ((int)(Main.screenPosition.Y / 16f) > (int)(Main.screenLastPosition.Y / 16f))
            {
                for (int num23 = 0; num23 < num16 - 1; num23++)
                {
                    for (int num24 = 0; num24 < num15; num24++)
                    {
                        Lighting.color[num24, num23] = Lighting.color[num24, num23 + 1];
                    }
                }
            }
        }
    }
}
