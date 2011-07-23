using System;
using Terraria_Server.Logging;
using Terraria_Server.WorldMod;

namespace Terraria_Server
{
    public class Liquid
    {
        public static int skipCount = 0;
        public static int stuckCount = 0;
        public static int stuckAmount = 0;
        public static int cycles = 10;
        public static int resLiquid = 5000;
        public static int maxLiquid = 5000;
        public static int numLiquid;
        public static bool stuck = false;
        public static bool quickFall = false;
        public static bool quickSettle = false;
        private static int wetCounter;
        public static int panicCounter = 0;
        public static bool panicMode = false;
        public static int panicY = 0;
        public int x;
        public int y;
        public int kill;
        public int delay;

        public static double QuickWater(int verbose = 0, int minY = -1, int maxY = -1)
        {
            int num = 0;

            if (minY == -1)
            {
                minY = 3;
            }

            if (maxY == -1)
            {
                maxY = Main.maxTilesY - 3;
            }

            for (int i = maxY; i >= minY; i--)
            {
                if (verbose > 0)
                {
                    float num2 = (float)(maxY - i) / (float)(maxY - minY + 1);
                    num2 /= (float)verbose;
                    Main.statusText = "Settling liquids: " + (int)(num2 * 100f + 1f) + "%";
                }
                else if (verbose < 0)
                {
                    float num3 = (float)(maxY - i) / (float)(maxY - minY + 1);
                    num3 /= (float)(-(float)verbose);
                    Main.statusText = "Creating underworld: " + (int)(num3 * 100f + 1f) + "%";

                }

                for (int j = 0; j < 2; j++)
                {
                    int num4 = 2;
                    int num5 = Main.maxTilesX - 2;
                    int num6 = 1;
                    if (j == 1)
                    {
                        num4 = Main.maxTilesX - 2;
                        num5 = 2;
                        num6 = -1;
                    }
                    for (int num7 = num4; num7 != num5; num7 += num6)
                    {
                        if (Main.tile.At(num7, i).Liquid > 0)
                        {
                            int num8 = -num6;
                            bool flag = false;
                            int num9 = num7;
                            int num10 = i;
                            bool flag2 = Main.tile.At(num7, i).Lava;
                            byte b = Main.tile.At(num7, i).Liquid;
                            Main.tile.At(num7, i).SetLiquid (0);
                            bool flag3 = true;
                            int num11 = 0;
                            while (flag3 && num9 > 3 && num9 < Main.maxTilesX - 3 && num10 < Main.maxTilesY - 3)
                            {
                                flag3 = false;
                                while (Main.tile.At(num9, num10 + 1).Liquid == 0 && num10 < Main.maxTilesY - 5 && (!Main.tile.At(num9, num10 + 1).Active || !Main.tileSolid[(int)Main.tile.At(num9, num10 + 1).Type] || Main.tileSolidTop[(int)Main.tile.At(num9, num10 + 1).Type]))
                                {
                                    flag = true;
                                    num8 = num6;
                                    num11 = 0;
                                    flag3 = true;
                                    num10++;
                                    if (num10 > WorldModify.waterLine)
                                    {
                                        flag2 = true;
                                    }
                                }
                                if (Main.tile.At(num9, num10 + 1).Liquid > 0 && Main.tile.At(num9, num10 + 1).Liquid < 255 && Main.tile.At(num9, num10 + 1).Lava == flag2)
                                {
                                    int num12 = (int)(255 - Main.tile.At(num9, num10 + 1).Liquid);
                                    if (num12 > (int)b)
                                    {
                                        num12 = (int)b;
                                    }
                                    TileRef expr_25A = Main.tile.At(num9, num10 + 1);
                                    expr_25A.SetLiquid ((byte) (expr_25A.Liquid + (byte)num12));
                                    b -= (byte)num12;
                                    if (b <= 0)
                                    {
                                        num++;
                                        break;
                                    }
                                }
                                if (num11 == 0)
                                {
                                    if (Main.tile.At(num9 + num8, num10).Liquid == 0 && (!Main.tile.At(num9 + num8, num10).Active || !Main.tileSolid[(int)Main.tile.At(num9 + num8, num10).Type] || Main.tileSolidTop[(int)Main.tile.At(num9 + num8, num10).Type]))
                                    {
                                        num11 = num8;
                                    }
                                    else
                                    {
                                        if (Main.tile.At(num9 - num8, num10).Liquid == 0 && (!Main.tile.At(num9 - num8, num10).Active || !Main.tileSolid[(int)Main.tile.At(num9 - num8, num10).Type] || Main.tileSolidTop[(int)Main.tile.At(num9 - num8, num10).Type]))
                                        {
                                            num11 = -num8;
                                        }
                                    }
                                }
                                if (num11 != 0 && Main.tile.At(num9 + num11, num10).Liquid == 0 && (!Main.tile.At(num9 + num11, num10).Active || !Main.tileSolid[(int)Main.tile.At(num9 + num11, num10).Type] || Main.tileSolidTop[(int)Main.tile.At(num9 + num11, num10).Type]))
                                {
                                    flag3 = true;
                                    num9 += num11;
                                }
                                if (flag && !flag3)
                                {
                                    flag = false;
                                    flag3 = true;
                                    num8 = -num6;
                                    num11 = 0;
                                }
                            }
                            if (num7 != num9 && i != num10)
                            {
                                num++;
                            }
                            Main.tile.At(num9, num10).SetLiquid (b);
                            Main.tile.At(num9, num10).SetLava (flag2);
                            if (Main.tile.At(num9 - 1, num10).Liquid > 0 && Main.tile.At(num9 - 1, num10).Lava != flag2)
                            {
                                if (flag2)
                                {
                                    Liquid.LavaCheck(num9, num10);
                                }
                                else
                                {
                                    Liquid.LavaCheck(num9 - 1, num10);
                                }
                            }
                            else
                            {
                                if (Main.tile.At(num9 + 1, num10).Liquid > 0 && Main.tile.At(num9 + 1, num10).Lava != flag2)
                                {
                                    if (flag2)
                                    {
                                        Liquid.LavaCheck(num9, num10);
                                    }
                                    else
                                    {
                                        Liquid.LavaCheck(num9 + 1, num10);
                                    }
                                }
                                else
                                {
                                    if (Main.tile.At(num9, num10 - 1).Liquid > 0 && Main.tile.At(num9, num10 - 1).Lava != flag2)
                                    {
                                        if (flag2)
                                        {
                                            Liquid.LavaCheck(num9, num10);
                                        }
                                        else
                                        {
                                            Liquid.LavaCheck(num9, num10 - 1);
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile.At(num9, num10 + 1).Liquid > 0 && Main.tile.At(num9, num10 + 1).Lava != flag2)
                                        {
                                            if (flag2)
                                            {
                                                Liquid.LavaCheck(num9, num10);
                                            }
                                            else
                                            {
                                                Liquid.LavaCheck(num9, num10 + 1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return (double)num;
        }
        
        public void Update()
        {
            if (Main.tile.At(this.x, this.y).Active && Main.tileSolid[(int)Main.tile.At(this.x, this.y).Type] && !Main.tileSolidTop[(int)Main.tile.At(this.x, this.y).Type])
            {
                if (Main.tile.At(this.x, this.y).Type != 10)
                {
                    Main.tile.At(this.x, this.y).SetLiquid (0);
                }
                this.kill = 9;
                return;
            }
            byte liquid = Main.tile.At(this.x, this.y).Liquid;
            float num = 0f;
            
            if (this.y > Main.maxTilesY - 200 && !Main.tile.At(this.x, this.y).Lava && Main.tile.At(this.x, this.y).Liquid > 0)
            {
                byte b = 2;
                if (Main.tile.At(this.x, this.y).Liquid < b)
                {
                    b = Main.tile.At(this.x, this.y).Liquid;
                }
                TileRef expr_16F = Main.tile.At(this.x, this.y);
                expr_16F.SetLiquid ((byte) (expr_16F.Liquid - b));
            }

            if (Main.tile.At(this.x, this.y).Liquid == 0)
            {
                this.kill = 9;
                return;
            }

            if (Main.tile.At(this.x, this.y).Lava)
            {
                Liquid.LavaCheck(this.x, this.y);
                if (!Liquid.quickFall)
                {
                    if (this.delay < 5)
                    {
                        this.delay++;
                        return;
                    }
                    this.delay = 0;
                }
            }
            else
            {
                if (Main.tile.At(this.x - 1, this.y).Lava)
                {
                    Liquid.AddWater(this.x - 1, this.y);
                }
                if (Main.tile.At(this.x + 1, this.y).Lava)
                {
                    Liquid.AddWater(this.x + 1, this.y);
                }
                if (Main.tile.At(this.x, this.y - 1).Lava)
                {
                    Liquid.AddWater(this.x, this.y - 1);
                }
                if (Main.tile.At(this.x, this.y + 1).Lava)
                {
                    Liquid.AddWater(this.x, this.y + 1);
                }
            }

            if ((!Main.tile.At(this.x, this.y + 1).Active || !Main.tileSolid[(int)Main.tile.At(this.x, this.y + 1).Type] || Main.tileSolidTop[(int)Main.tile.At(this.x, this.y + 1).Type]) && (Main.tile.At(this.x, this.y + 1).Liquid <= 0 || Main.tile.At(this.x, this.y + 1).Lava == Main.tile.At(this.x, this.y).Lava) && Main.tile.At(this.x, this.y + 1).Liquid < 255)
            {
                num = (float)(255 - Main.tile.At(this.x, this.y + 1).Liquid);
                if (num > (float)Main.tile.At(this.x, this.y).Liquid)
                {
                    num = (float)Main.tile.At(this.x, this.y).Liquid;
                }
                TileRef expr_42E = Main.tile.At(this.x, this.y);
                expr_42E.SetLiquid ((byte) (expr_42E.Liquid - (byte)num));
                TileRef expr_455 = Main.tile.At(this.x, this.y + 1);
                expr_455.SetLiquid ((byte) (expr_455.Liquid + (byte)num));
                Main.tile.At(this.x, this.y + 1).SetLava (Main.tile.At(this.x, this.y).Lava);
                Liquid.AddWater(this.x, this.y + 1);
                Main.tile.At(this.x, this.y + 1).SetSkipLiquid (true);
                Main.tile.At(this.x, this.y).SetSkipLiquid (true);
                if (Main.tile.At(this.x, this.y).Liquid > 250)
                {
                    Main.tile.At(this.x, this.y).SetLiquid (255);
                }
                else
                {
                    Liquid.AddWater(this.x - 1, this.y);
                    Liquid.AddWater(this.x + 1, this.y);
                }
            }

            if (Main.tile.At(this.x, this.y).Liquid > 0)
            {
                bool flag = true;
                bool flag2 = true;
                bool flag3 = true;
                bool flag4 = true;
                if (Main.tile.At(this.x - 1, this.y).Active && Main.tileSolid[(int)Main.tile.At(this.x - 1, this.y).Type] && !Main.tileSolidTop[(int)Main.tile.At(this.x - 1, this.y).Type])
                {
                    flag = false;
                }
                else if (Main.tile.At(this.x - 1, this.y).Liquid > 0 && Main.tile.At(this.x - 1, this.y).Lava != Main.tile.At(this.x, this.y).Lava)
                {
                    flag = false;
                }
                else if (Main.tile.At(this.x - 2, this.y).Active && Main.tileSolid[(int)Main.tile.At(this.x - 2, this.y).Type] && !Main.tileSolidTop[(int)Main.tile.At(this.x - 2, this.y).Type])
                {
                    flag3 = false;
                }
                else if (Main.tile.At(this.x - 2, this.y).Liquid == 0)
                {
                    flag3 = false;
                }
                else if (Main.tile.At(this.x - 2, this.y).Liquid > 0 && Main.tile.At(this.x - 2, this.y).Lava != Main.tile.At(this.x, this.y).Lava)
                {
                    flag3 = false;
                }

                if (Main.tile.At(this.x + 1, this.y).Active && Main.tileSolid[(int)Main.tile.At(this.x + 1, this.y).Type] && !Main.tileSolidTop[(int)Main.tile.At(this.x + 1, this.y).Type])
                {
                    flag2 = false;
                }
                else
                {
                    if (Main.tile.At(this.x + 1, this.y).Liquid > 0 && Main.tile.At(this.x + 1, this.y).Lava != Main.tile.At(this.x, this.y).Lava)
                    {
                        flag2 = false;
                    }
                    else
                    {
                        if (Main.tile.At(this.x + 2, this.y).Active && Main.tileSolid[(int)Main.tile.At(this.x + 2, this.y).Type] && !Main.tileSolidTop[(int)Main.tile.At(this.x + 2, this.y).Type])
                        {
                            flag4 = false;
                        }
                        else
                        {
                            if (Main.tile.At(this.x + 2, this.y).Liquid == 0)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                if (Main.tile.At(this.x + 2, this.y).Liquid > 0 && Main.tile.At(this.x + 2, this.y).Lava != Main.tile.At(this.x, this.y).Lava)
                                {
                                    flag4 = false;
                                }
                            }
                        }
                    }
                }
                int num2 = 0;
                if (Main.tile.At(this.x, this.y).Liquid < 3)
                {
                    num2 = -1;
                }
                if (flag && flag2)
                {
                    if (flag3 && flag4)
                    {
                        bool flag5 = true;
                        bool flag6 = true;
                        if (Main.tile.At(this.x - 3, this.y).Active && Main.tileSolid[(int)Main.tile.At(this.x - 3, this.y).Type] && !Main.tileSolidTop[(int)Main.tile.At(this.x - 3, this.y).Type])
                        {
                            flag5 = false;
                        }
                        else
                        {
                            if (Main.tile.At(this.x - 3, this.y).Liquid == 0)
                            {
                                flag5 = false;
                            }
                            else
                            {
                                if (Main.tile.At(this.x - 3, this.y).Lava != Main.tile.At(this.x, this.y).Lava)
                                {
                                    flag5 = false;
                                }
                            }
                        }
                        if (Main.tile.At(this.x + 3, this.y).Active && Main.tileSolid[(int)Main.tile.At(this.x + 3, this.y).Type] && !Main.tileSolidTop[(int)Main.tile.At(this.x + 3, this.y).Type])
                        {
                            flag6 = false;
                        }
                        else
                        {
                            if (Main.tile.At(this.x + 3, this.y).Liquid == 0)
                            {
                                flag6 = false;
                            }
                            else
                            {
                                if (Main.tile.At(this.x + 3, this.y).Lava != Main.tile.At(this.x, this.y).Lava)
                                {
                                    flag6 = false;
                                }
                            }
                        }
                        if (flag5 && flag6)
                        {
                            num = (float)((int)(Main.tile.At(this.x - 1, this.y).Liquid + Main.tile.At(this.x + 1, this.y).Liquid + Main.tile.At(this.x - 2, this.y).Liquid + Main.tile.At(this.x + 2, this.y).Liquid + Main.tile.At(this.x - 3, this.y).Liquid + Main.tile.At(this.x + 3, this.y).Liquid + Main.tile.At(this.x, this.y).Liquid) + num2);
                            num = (float)Math.Round((double)(num / 7f));
                            int num3 = 0;
                            Main.tile.At(this.x - 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x - 1, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 1, this.y);
                                Main.tile.At(this.x - 1, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num3++;
                            }
                            Main.tile.At(this.x + 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x + 1, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 1, this.y);
                                Main.tile.At(this.x + 1, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num3++;
                            }
                            Main.tile.At(this.x - 2, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x - 2, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 2, this.y);
                                Main.tile.At(this.x - 2, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num3++;
                            }
                            Main.tile.At(this.x + 2, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x + 2, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 2, this.y);
                                Main.tile.At(this.x + 2, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num3++;
                            }
                            Main.tile.At(this.x - 3, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x - 3, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 3, this.y);
                                Main.tile.At(this.x - 3, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num3++;
                            }
                            Main.tile.At(this.x + 3, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x + 3, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 3, this.y);
                                Main.tile.At(this.x + 3, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num3++;
                            }
                            if (Main.tile.At(this.x - 1, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 1, this.y);
                            }
                            if (Main.tile.At(this.x + 1, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 1, this.y);
                            }
                            if (Main.tile.At(this.x - 2, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 2, this.y);
                            }
                            if (Main.tile.At(this.x + 2, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 2, this.y);
                            }
                            if (Main.tile.At(this.x - 3, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 3, this.y);
                            }
                            if (Main.tile.At(this.x + 3, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 3, this.y);
                            }
                            if (num3 != 6 || Main.tile.At(this.x, this.y - 1).Liquid <= 0)
                            {
                                Main.tile.At(this.x, this.y).SetLiquid ((byte)num);
                            }
                        }
                        else
                        {
                            int num4 = 0;
                            num = (float)((int)(Main.tile.At(this.x - 1, this.y).Liquid + Main.tile.At(this.x + 1, this.y).Liquid + Main.tile.At(this.x - 2, this.y).Liquid + Main.tile.At(this.x + 2, this.y).Liquid + Main.tile.At(this.x, this.y).Liquid) + num2);
                            num = (float)Math.Round((double)(num / 5f));
                            Main.tile.At(this.x - 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x - 1, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 1, this.y);
                                Main.tile.At(this.x - 1, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num4++;
                            }
                            Main.tile.At(this.x + 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x + 1, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 1, this.y);
                                Main.tile.At(this.x + 1, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num4++;
                            }
                            Main.tile.At(this.x - 2, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x - 2, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 2, this.y);
                                Main.tile.At(this.x - 2, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num4++;
                            }
                            Main.tile.At(this.x + 2, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x + 2, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 2, this.y);
                                Main.tile.At(this.x + 2, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num4++;
                            }
                            if (Main.tile.At(this.x - 1, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 1, this.y);
                            }
                            if (Main.tile.At(this.x + 1, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 1, this.y);
                            }
                            if (Main.tile.At(this.x - 2, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 2, this.y);
                            }
                            if (Main.tile.At(this.x + 2, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 2, this.y);
                            }
                            if (num4 != 4 || Main.tile.At(this.x, this.y - 1).Liquid <= 0)
                            {
                                Main.tile.At(this.x, this.y).SetLiquid ((byte)num);
                            }
                        }
                    }
                    else
                    {
                        if (flag3)
                        {
                            num = (float)((int)(Main.tile.At(this.x - 1, this.y).Liquid + Main.tile.At(this.x + 1, this.y).Liquid + Main.tile.At(this.x - 2, this.y).Liquid + Main.tile.At(this.x, this.y).Liquid) + num2);
                            num = (float)Math.Round((double)(num / 4f) + 0.001);
                            Main.tile.At(this.x - 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x - 1, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x - 1, this.y);
                                Main.tile.At(this.x - 1, this.y).SetLiquid ((byte)num);
                            }
                            Main.tile.At(this.x + 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x + 1, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 1, this.y);
                                Main.tile.At(this.x + 1, this.y).SetLiquid ((byte)num);
                            }
                            Main.tile.At(this.x - 2, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x - 2, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                            {
                                Main.tile.At(this.x - 2, this.y).SetLiquid ((byte)num);
                                Liquid.AddWater(this.x - 2, this.y);
                            }
                            Main.tile.At(this.x, this.y).SetLiquid ((byte)num);
                        }
                        else
                        {
                            if (flag4)
                            {
                                num = (float)((int)(Main.tile.At(this.x - 1, this.y).Liquid + Main.tile.At(this.x + 1, this.y).Liquid + Main.tile.At(this.x + 2, this.y).Liquid + Main.tile.At(this.x, this.y).Liquid) + num2);
                                num = (float)Math.Round((double)(num / 4f) + 0.001);
                                Main.tile.At(this.x - 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                                if (Main.tile.At(this.x - 1, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                                {
                                    Liquid.AddWater(this.x - 1, this.y);
                                    Main.tile.At(this.x - 1, this.y).SetLiquid ((byte)num);
                                }
                                Main.tile.At(this.x + 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                                if (Main.tile.At(this.x + 1, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                                {
                                    Liquid.AddWater(this.x + 1, this.y);
                                    Main.tile.At(this.x + 1, this.y).SetLiquid ((byte)num);
                                }
                                Main.tile.At(this.x + 2, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                                if (Main.tile.At(this.x + 2, this.y).Liquid != (byte)num || Main.tile.At(this.x, this.y).Liquid != (byte)num)
                                {
                                    Main.tile.At(this.x + 2, this.y).SetLiquid ((byte)num);
                                    Liquid.AddWater(this.x + 2, this.y);
                                }
                                Main.tile.At(this.x, this.y).SetLiquid ((byte)num);
                            }
                            else
                            {
                                num = (float)((int)(Main.tile.At(this.x - 1, this.y).Liquid + Main.tile.At(this.x + 1, this.y).Liquid + Main.tile.At(this.x, this.y).Liquid) + num2);
                                num = (float)Math.Round((double)(num / 3f) + 0.001);
                                Main.tile.At(this.x - 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                                if (Main.tile.At(this.x - 1, this.y).Liquid != (byte)num)
                                {
                                    Main.tile.At(this.x - 1, this.y).SetLiquid ((byte)num);
                                }
                                if (Main.tile.At(this.x, this.y).Liquid != (byte)num || Main.tile.At(this.x - 1, this.y).Liquid != (byte)num)
                                {
                                    Liquid.AddWater(this.x - 1, this.y);
                                }
                                Main.tile.At(this.x + 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                                if (Main.tile.At(this.x + 1, this.y).Liquid != (byte)num)
                                {
                                    Main.tile.At(this.x + 1, this.y).SetLiquid ((byte)num);
                                }
                                if (Main.tile.At(this.x, this.y).Liquid != (byte)num || Main.tile.At(this.x + 1, this.y).Liquid != (byte)num)
                                {
                                    Liquid.AddWater(this.x + 1, this.y);
                                }
                                Main.tile.At(this.x, this.y).SetLiquid ((byte)num);
                            }
                        }
                    }
                }
                else
                {
                    if (flag)
                    {
                        num = (float)((int)(Main.tile.At(this.x - 1, this.y).Liquid + Main.tile.At(this.x, this.y).Liquid) + num2);
                        num = (float)Math.Round((double)(num / 2f) + 0.001);
                        if (Main.tile.At(this.x - 1, this.y).Liquid != (byte)num)
                        {
                            Main.tile.At(this.x - 1, this.y).SetLiquid ((byte)num);
                        }
                        Main.tile.At(this.x - 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                        if (Main.tile.At(this.x, this.y).Liquid != (byte)num || Main.tile.At(this.x - 1, this.y).Liquid != (byte)num)
                        {
                            Liquid.AddWater(this.x - 1, this.y);
                        }
                        Main.tile.At(this.x, this.y).SetLiquid ((byte)num);
                    }
                    else
                    {
                        if (flag2)
                        {
                            num = (float)((int)(Main.tile.At(this.x + 1, this.y).Liquid + Main.tile.At(this.x, this.y).Liquid) + num2);
                            num = (float)Math.Round((double)(num / 2f) + 0.001);
                            if (Main.tile.At(this.x + 1, this.y).Liquid != (byte)num)
                            {
                                Main.tile.At(this.x + 1, this.y).SetLiquid ((byte)num);
                            }
                            Main.tile.At(this.x + 1, this.y).SetLava (Main.tile.At(this.x, this.y).Lava);
                            if (Main.tile.At(this.x, this.y).Liquid != (byte)num || Main.tile.At(this.x + 1, this.y).Liquid != (byte)num)
                            {
                                Liquid.AddWater(this.x + 1, this.y);
                            }
                            Main.tile.At(this.x, this.y).SetLiquid ((byte)num);
                        }
                    }
                }
            }
            if (Main.tile.At(this.x, this.y).Liquid == liquid)
            {
                this.kill++;
                return;
            }
            if (Main.tile.At(this.x, this.y).Liquid == 254 && liquid == 255)
            {
                Main.tile.At(this.x, this.y).SetLiquid (255);
                this.kill++;
                return;
            }
            Liquid.AddWater(this.x, this.y - 1);
            this.kill = 0;
        }
        
        public static void StartPanic()
        {
            if (!Liquid.panicMode)
            {
                WorldModify.waterLine = Main.maxTilesY;
                Liquid.numLiquid = 0;
                LiquidBuffer.numLiquidBuffer = 0;
                Liquid.panicCounter = 0;
                Liquid.panicMode = true;
                Liquid.panicY = Main.maxTilesY - 3;
                if (Main.dedServ)
                {
                    ProgramLog.Log ("Forcing water to settle.");
                }
            }
        }
        
        public static void UpdateLiquid()
        {
            Liquid.cycles = 25;
            Liquid.maxLiquid = 5000;

            if (!WorldModify.gen)
            {
                if (!Liquid.panicMode)
                {
                    if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > 4000)
                    {
                        Liquid.panicCounter++;
                        if (Liquid.panicCounter > 1800 || Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > 13500)
                        {
                            Liquid.StartPanic();
                        }
                    }
                    else
                    {
                        Liquid.panicCounter = 0;
                    }
                }

                if (Liquid.panicMode)
                {
                    int num = 0;
                    while (Liquid.panicY >= 3 && num < 5)
                    {
                        num++;
                        Liquid.QuickWater(0, Liquid.panicY, Liquid.panicY);
                        Liquid.panicY--;
                        if (Liquid.panicY < 3)
                        {
                            ProgramLog.Log ("Water has been settled.");
                            Liquid.panicCounter = 0;
                            Liquid.panicMode = false;
                            WorldModify.WaterCheck();
                            for (int i = 0; i < 255; i++)
                            {
                                for (int j = 0; j < Main.maxSectionsX; j++)
                                {
                                    for (int k = 0; k < Main.maxSectionsY; k++)
                                    {
                                        Netplay.slots[i].tileSection[j, k] = false;
                                    }
                                }
                            }
                        }
                    }
                    return;
                }
            }

            if (Liquid.quickSettle || Liquid.numLiquid > 2000)
            {
                Liquid.quickFall = true;
            }
            else
            {
                Liquid.quickFall = false;
            }
            
            Liquid.wetCounter++;
            
            int num2 = Liquid.maxLiquid / Liquid.cycles;
            
            int num3 = num2 * (Liquid.wetCounter - 1);
            
            int num4 = num2 * Liquid.wetCounter;

            if (Liquid.wetCounter == Liquid.cycles)
            {
                num4 = Liquid.numLiquid;
            }

            if (num4 > Liquid.numLiquid)
            {
                num4 = Liquid.numLiquid;
                Liquid.wetCounter = Liquid.cycles;
            }

            if (Liquid.quickFall)
            {
                for (int l = num3; l < num4; l++)
                {
                    Main.liquid[l].delay = 10;
                    Main.liquid[l].Update();
                    Main.tile.At(Main.liquid[l].x, Main.liquid[l].y).SetSkipLiquid (false);
                }
            }
            else
            {
                for (int m = num3; m < num4; m++)
                {
                    
                    if (!Main.tile.At(Main.liquid[m].x, Main.liquid[m].y).SkipLiquid)
                    {
                        Main.liquid[m].Update();
                    }
                    else
                    {
                        Main.tile.At(Main.liquid[m].x, Main.liquid[m].y).SetSkipLiquid (false);
                    }
                }
            }

            if (Liquid.wetCounter >= Liquid.cycles)
            {
                Liquid.wetCounter = 0;
                for (int n = Liquid.numLiquid - 1; n >= 0; n--)
                {
                    if (Main.liquid[n].kill > 3)
                    {
                        Liquid.DelWater(n);
                    }
                }

                int num5 = Liquid.maxLiquid - (Liquid.maxLiquid - Liquid.numLiquid);

                if (num5 > LiquidBuffer.numLiquidBuffer)
                {
                    num5 = LiquidBuffer.numLiquidBuffer;
                }

                for (int num6 = 0; num6 < num5; num6++)
                {
                    Main.tile.At(Main.liquidBuffer[0].x, Main.liquidBuffer[0].y).SetCheckingLiquid (false);
                    Liquid.AddWater(Main.liquidBuffer[0].x, Main.liquidBuffer[0].y);
                    LiquidBuffer.DelBuffer(0);
                }

                if (Liquid.numLiquid > 0 && Liquid.numLiquid > Liquid.stuckAmount - 50 && Liquid.numLiquid < Liquid.stuckAmount + 50)
                {
                    Liquid.stuckCount++;
                    if (Liquid.stuckCount >= 10000)
                    {
                        Liquid.stuck = true;
                        for (int num7 = Liquid.numLiquid - 1; num7 >= 0; num7--)
                        {
                            Liquid.DelWater(num7);
                        }
                        Liquid.stuck = false;
                        Liquid.stuckCount = 0;
                        return;
                    }
                }
                else
                {
                    Liquid.stuckCount = 0;
                    Liquid.stuckAmount = Liquid.numLiquid;
                }
            }
        }

        public static void AddWater(int x, int y)
        {
            if (Main.tile.At(x, y).CheckingLiquid)
            {
                return;
            }
            if (x >= Main.maxTilesX - 5 || y >= Main.maxTilesY - 5)
            {
                return;
            }
            if (x < 5 || y < 5)
            {
                return;
            }
            if (Main.tile.At(x, y).Liquid == 0)
            {
                return;
            }
            if (Liquid.numLiquid >= Liquid.maxLiquid - 1)
            {
                LiquidBuffer.AddBuffer(x, y);
                return;
            }
            Main.tile.At(x, y).SetCheckingLiquid (true);
            Main.liquid[Liquid.numLiquid].kill = 0;
            Main.liquid[Liquid.numLiquid].x = x;
            Main.liquid[Liquid.numLiquid].y = y;
            Main.liquid[Liquid.numLiquid].delay = 0;
            Main.tile.At(x, y).SetSkipLiquid (false);
            Liquid.numLiquid++;
            
            NetMessage.SendWater(x, y);

            if (Main.tile.At(x, y).Active && (Main.tileWaterDeath[(int)Main.tile.At(x, y).Type] || (Main.tile.At(x, y).Lava && Main.tileLavaDeath[(int)Main.tile.At(x, y).Type])))
            {
                if (WorldModify.gen)
                {
                    Main.tile.At(x, y).SetActive (false);
                    return;
                }
                WorldModify.KillTile(x, y, false, false, false);
                NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y);
            }
        }
        
        public static void LavaCheck(int x, int y)
        {
            if ((Main.tile.At(x - 1, y).Liquid > 0 && !Main.tile.At(x - 1, y).Lava) || (Main.tile.At(x + 1, y).Liquid > 0 && !Main.tile.At(x + 1, y).Lava) || (Main.tile.At(x, y - 1).Liquid > 0 && !Main.tile.At(x, y - 1).Lava))
            {
                int num = 0;

                if (!Main.tile.At(x - 1, y).Lava)
                {
                    num += (int)Main.tile.At(x - 1, y).Liquid;
                    Main.tile.At(x - 1, y).SetLiquid (0);
                }

                if (!Main.tile.At(x + 1, y).Lava)
                {
                    num += (int)Main.tile.At(x + 1, y).Liquid;
                    Main.tile.At(x + 1, y).SetLiquid (0);
                }

                if (!Main.tile.At(x, y - 1).Lava)
                {
                    num += (int)Main.tile.At(x, y - 1).Liquid;
                    Main.tile.At(x, y - 1).SetLiquid (0);
                }

                if (num >= 128 && !Main.tile.At(x, y).Active)
                {
                    ClearLava(x, y);
                    WorldModify.PlaceTile(x, y, 56, true, true, -1, 0);
                    WorldModify.SquareTileFrame(x, y, true);

                    NetMessage.SendTileSquare(-1, x - 1, y - 1, 3);
                    return;
                }
            }
            else if (Main.tile.At(x, y + 1).Liquid > 0 && !Main.tile.At(x, y + 1).Lava && !Main.tile.At(x, y + 1).Active)
            {
                ClearLava(x, y);
                WorldModify.PlaceTile(x, y + 1, 56, true, true, -1, 0);
                WorldModify.SquareTileFrame(x, y + 1, true);
                
                NetMessage.SendTileSquare(-1, x - 1, y, 3);
            }
        }

        private static void ClearLava(int x, int y)
        {
            Main.tile.At(x, y).SetLiquid (0);
            Main.tile.At(x, y).SetLava (false);
        }
        
        public static void DelWater(int liquidIndex)
        {
            int x = Main.liquid[liquidIndex].x;
            int y = Main.liquid[liquidIndex].y;

            if (Main.tile.At(x, y).Liquid < 2)
            {
                Main.tile.At(x, y).SetLiquid (0);
            }
            else if (Main.tile.At(x, y).Liquid < 20)
            {
                if ((Main.tile.At(x - 1, y).Liquid < Main.tile.At(x, y).Liquid && (!Main.tile.At(x - 1, y).Active || !Main.tileSolid[(int)Main.tile.At(x - 1, y).Type] || Main.tileSolidTop[(int)Main.tile.At(x - 1, y).Type])) || (Main.tile.At(x + 1, y).Liquid < Main.tile.At(x, y).Liquid && (!Main.tile.At(x + 1, y).Active || !Main.tileSolid[(int)Main.tile.At(x + 1, y).Type] || Main.tileSolidTop[(int)Main.tile.At(x + 1, y).Type])) || (Main.tile.At(x, y + 1).Liquid < 255 && (!Main.tile.At(x, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x, y + 1).Type] || Main.tileSolidTop[(int)Main.tile.At(x, y + 1).Type])))
                {
                    Main.tile.At(x, y).SetLiquid (0);
                }
            }
            else if (Main.tile.At(x, y + 1).Liquid < 255 && (!Main.tile.At(x, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x, y + 1).Type] || Main.tileSolidTop[(int)Main.tile.At(x, y + 1).Type]) && !Liquid.stuck)
            {
                Main.liquid[liquidIndex].kill = 0;
                return;
            }

            if (Main.tile.At(x, y).Liquid == 0)
            {
                Main.tile.At(x, y).SetLava (false);
            }
            else if (Main.tile.At(x, y).Lava)
            {
                Liquid.LavaCheck(x, y);
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (Main.tile.At(i, j).Active)
                        {
                            if (Main.tile.At(i, j).Type == 2 || Main.tile.At(i, j).Type == 23)
                            {
                                Main.tile.At(i, j).SetType (0);
                                WorldModify.SquareTileFrame(i, j, true);
                                
                                NetMessage.SendTileSquare(-1, x, y, 3);
                            }
                            else if (Main.tile.At(i, j).Type == 60 || Main.tile.At(i, j).Type == 70)
                            {
                                Main.tile.At(i, j).SetType (59);
                                WorldModify.SquareTileFrame(i, j, true);
                                
                                NetMessage.SendTileSquare(-1, x, y, 3);
                            }
                        }
                    }
                }
            }

            
            NetMessage.SendWater(x, y);

            Liquid.numLiquid--;
            Main.tile.At(Main.liquid[liquidIndex].x, Main.liquid[liquidIndex].y).SetCheckingLiquid (false);
            Main.liquid[liquidIndex].x = Main.liquid[Liquid.numLiquid].x;
            Main.liquid[liquidIndex].y = Main.liquid[Liquid.numLiquid].y;
            Main.liquid[liquidIndex].kill = Main.liquid[Liquid.numLiquid].kill;
            if (Main.tileAlch[(int)Main.tile.At(x, y).Type])
            {
                WorldModify.CheckAlch(x, y);
            }
        }
    }
}
