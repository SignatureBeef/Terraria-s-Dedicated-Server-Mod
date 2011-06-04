using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class Liquid
    {
        public static int cycles = 10;
        public int delay;
        public int kill;
        public static int maxLiquid = 0x1388;
        public static int numLiquid;
        public static int panicCounter = 0;
        public static bool panicMode = false;
        public static int panicY = 0;
        public static bool quickFall = false;
        public static bool quickSettle = false;
        public static int resLiquid = 0x1388;
        public static int skipCount = 0;
        public static bool stuck = false;
        public static int stuckAmount = 0;
        public static int stuckCount = 0;
        private static int wetCounter;
        public int x;
        public int y;

        public static void AddWater(int x, int y, World world)
        {
            if ((((!world.getTile()[x, y].checkingLiquid && ((x < (world.getMaxTilesX() - 5)) && (y < (world.getMaxTilesY() - 5)))) && ((x >= 5) && (y >= 5))) && (world.getTile()[x, y] != null)) && (world.getTile()[x, y].liquid != 0))
            {
                if (numLiquid >= (maxLiquid - 1))
                {
                    LiquidBuffer.AddBuffer(x, y, world);
                }
                else
                {
                    world.getTile()[x, y].checkingLiquid = true;
                    world.getLiquid()[numLiquid].kill = 0;
                    world.getLiquid()[numLiquid].x = x;
                    world.getLiquid()[numLiquid].y = y;
                    world.getLiquid()[numLiquid].delay = 0;
                    world.getTile()[x, y].skipLiquid = false;
                    numLiquid++;
                    if (Statics.netMode == 2)
                    {
                        NetMessage.sendWater(x, y, world);
                    }
                    if (world.getTile()[x, y].active && (Statics.tileWaterDeath[world.getTile()[x, y].type] || (world.getTile()[x, y].lava && Statics.tileLavaDeath[world.getTile()[x, y].type])))
                    {
                        if (WorldGen.gen)
                        {
                            world.getTile()[x, y].active = false;
                        }
                        else
                        {
                            WorldGen.KillTile(x, y, world, false, false, false);
                            if (Statics.netMode == 2)
                            {
                                NetMessage.SendData(0x11, world, -1, -1, "", 0, (float)x, (float)y, 0f);
                            }
                        }
                    }
                }
            }
        }

        public static void DelWater(int l, World world)
        {
            int x = world.getLiquid()[l].x;
            int y = world.getLiquid()[l].y;
            if (world.getTile()[x, y].liquid < 2)
            {
                world.getTile()[x, y].liquid = 0;
            }
            else if (world.getTile()[x, y].liquid < 20)
            {
                if ((((world.getTile()[x - 1, y].liquid < world.getTile()[x, y].liquid) && ((!world.getTile()[x - 1, y].active || !Statics.tileSolid[world.getTile()[x - 1, y].type]) || Statics.tileSolidTop[world.getTile()[x - 1, y].type])) || ((world.getTile()[x + 1, y].liquid < world.getTile()[x, y].liquid) && ((!world.getTile()[x + 1, y].active || !Statics.tileSolid[world.getTile()[x + 1, y].type]) || Statics.tileSolidTop[world.getTile()[x + 1, y].type]))) || ((world.getTile()[x, y + 1].liquid < 0xff) && ((!world.getTile()[x, y + 1].active || !Statics.tileSolid[world.getTile()[x, y + 1].type]) || Statics.tileSolidTop[world.getTile()[x, y + 1].type])))
                {
                    world.getTile()[x, y].liquid = 0;
                }
            }
            else if (((world.getTile()[x, y + 1].liquid < 0xff) && ((!world.getTile()[x, y + 1].active || !Statics.tileSolid[world.getTile()[x, y + 1].type]) || Statics.tileSolidTop[world.getTile()[x, y + 1].type])) && !stuck)
            {
                world.getLiquid()[l].kill = 0;
                return;
            }
            if (world.getTile()[x, y].liquid == 0)
            {
                world.getTile()[x, y].lava = false;
            }
            else if (world.getTile()[x, y].lava)
            {
                LavaCheck(x, y, world);
            }
            if (Statics.netMode == 2)
            {
                NetMessage.sendWater(x, y, world);
            }
            numLiquid--;
            world.getTile()[world.getLiquid()[l].x, world.getLiquid()[l].y].checkingLiquid = false;
            world.getLiquid()[l].x = world.getLiquid()[numLiquid].x;
            world.getLiquid()[l].y = world.getLiquid()[numLiquid].y;
            world.getLiquid()[l].kill = world.getLiquid()[numLiquid].kill;
        }

        public static void LavaCheck(int x, int y, World world)
        {
            if ((((world.getTile()[x - 1, y].liquid > 0) && !world.getTile()[x - 1, y].lava) || ((world.getTile()[x + 1, y].liquid > 0) && !world.getTile()[x + 1, y].lava)) || ((world.getTile()[x, y - 1].liquid > 0) && !world.getTile()[x, y - 1].lava))
            {
                int num = 0;
                if (!world.getTile()[x - 1, y].lava)
                {
                    num += world.getTile()[x - 1, y].liquid;
                    world.getTile()[x - 1, y].liquid = 0;
                }
                if (!world.getTile()[x + 1, y].lava)
                {
                    num += world.getTile()[x + 1, y].liquid;
                    world.getTile()[x + 1, y].liquid = 0;
                }
                if (!world.getTile()[x, y - 1].lava)
                {
                    num += world.getTile()[x, y - 1].liquid;
                    world.getTile()[x, y - 1].liquid = 0;
                }
                if (num >= 0x80)
                {
                    world.getTile()[x, y].liquid = 0;
                    world.getTile()[x, y].lava = false;
                    WorldGen.PlaceTile(x, y, world, 0x38, true, true, -1);
                    WorldGen.SquareTileFrame(x, y, world, true);
                    if (Statics.netMode == 2)
                    {
                        NetMessage.SendTileSquare(-1, x - 1, y - 1, 3, world);
                    }
                }
            }
            else if ((world.getTile()[x, y + 1].liquid > 0) && !world.getTile()[x, y + 1].lava)
            {
                world.getTile()[x, y].liquid = 0;
                world.getTile()[x, y].lava = false;
                WorldGen.PlaceTile(x, y + 1, world, 0x38, true, true, -1);
                WorldGen.SquareTileFrame(x, y, world, true);
                if (Statics.netMode == 2)
                {
                    NetMessage.SendTileSquare(-1, x - 1, y, 3, world);
                }
            }
        }

        public static void NetAddWater(int x, int y, World world)
        {
            if (((((x < (world.getMaxTilesX() - 5)) && (y < (world.getMaxTilesY() - 5))) && ((x >= 5) && (y >= 5))) && (world.getTile()[x, y] != null)) && (world.getTile()[x, y].liquid != 0))
            {
                for (int i = 0; i < numLiquid; i++)
                {
                    if ((world.getLiquid()[i].x == x) && (world.getLiquid()[i].y == y))
                    {
                        world.getLiquid()[i].kill = 0;
                        world.getTile()[x, y].skipLiquid = true;
                        return;
                    }
                }
                if (numLiquid >= (maxLiquid - 1))
                {
                    LiquidBuffer.AddBuffer(x, y, world);
                }
                else
                {
                    world.getTile()[x, y].checkingLiquid = true;
                    world.getTile()[x, y].skipLiquid = true;
                    world.getLiquid()[numLiquid].kill = 0;
                    world.getLiquid()[numLiquid].x = x;
                    world.getLiquid()[numLiquid].y = y;
                    numLiquid++;
                    if (Statics.netMode == 2)
                    {
                        NetMessage.sendWater(x, y, world);
                    }
                    if (world.getTile()[x, y].active && (Statics.tileWaterDeath[world.getTile()[x, y].type] || (world.getTile()[x, y].lava && Statics.tileLavaDeath[world.getTile()[x, y].type])))
                    {
                        WorldGen.KillTile(x, y, world, false, false, false);
                        if (Statics.netMode == 2)
                        {
                            NetMessage.SendData(0x11, world, -1, -1, "", 0, (float)x, (float)y, 0f);
                        }
                    }
                }
            }
        }

        public static double QuickWater(World world, int verbose = 0, int minY = -1, int maxY = -1)
        {
            int num = 0;
            if (minY == -1)
            {
                minY = 3;
            }
            if (maxY == -1)
            {
                maxY = world.getMaxTilesY() - 3;
            }
            string text = "";
            int preserve = 0;
            for (int i = maxY; i >= minY; i--)
            {
                if (verbose > 0)
                {
                    float num3 = ((float)(maxY - i)) / ((float)((maxY - minY) + 1));
                    num3 /= (float)verbose;
                    ////Console.WriteLine("Settling liquids: " + ((int)((num3 * 100f) + 1f)) + "%";

                    for (int i_ = 0; i_ < preserve; i_++)
                    {
                        Console.Write("\b");
                    }
                    text = "Settling liquids: " + ((int)((num3 * 100f) + 1f)) + "%";
                    Console.Write(text);
                    preserve = text.Length;
                }
                else if (verbose < 0)
                {
                    float num4 = ((float)(maxY - i)) / ((float)((maxY - minY) + 1));
                    num4 /= (float)-verbose;
                    ////Console.WriteLine("Creating underworld: " + ((int)((num4 * 100f) + 1f)) + "%";
                    for (int i_ = 0; i_ < preserve; i_++)
                    {
                        Console.Write("\b");
                    }
                    text ="Creating underworld: " + ((int)((num4 * 100f) + 1f)) + "%";
                    Console.Write(text);
                    preserve = text.Length;
                }
                for (int j = 0; j < 2; j++)
                {
                    int num6 = 2;
                    int num7 = world.getMaxTilesX() - 2;
                    int num8 = 1;
                    if (j == 1)
                    {
                        num6 = world.getMaxTilesX() - 2;
                        num7 = 2;
                        num8 = -1;
                    }
                    for (int k = num6; k != num7; k += num8)
                    {
                        if (world.getTile()[k, i].liquid <= 0)
                        {
                            continue;
                        }
                        int num10 = -num8;
                        bool flag = false;
                        int x = k;
                        int y = i;
                        bool lava = world.getTile()[k, i].lava;
                        byte liquid = world.getTile()[k, i].liquid;
                        world.getTile()[k, i].liquid = 0;
                        bool flag3 = true;
                        int num14 = 0;
                        while ((flag3 && (x > 3)) && ((x < (world.getMaxTilesX() - 3)) && (y < (world.getMaxTilesY() - 3))))
                        {
                            flag3 = false;
                            while (((world.getTile()[x, y + 1].liquid == 0) && (y < (world.getMaxTilesY() - 5))) && ((!world.getTile()[x, y + 1].active || !Statics.tileSolid[world.getTile()[x, y + 1].type]) || Statics.tileSolidTop[world.getTile()[x, y + 1].type]))
                            {
                                flag = true;
                                num10 = num8;
                                num14 = 0;
                                flag3 = true;
                                y++;
                                if (y > WorldGen.waterLine)
                                {
                                    lava = true;
                                }
                            }
                            if (((world.getTile()[x, y + 1].liquid > 0) && (world.getTile()[x, y + 1].liquid < 0xff)) && (world.getTile()[x, y + 1].lava == lava))
                            {
                                int num15 = 0xff - world.getTile()[x, y + 1].liquid;
                                if (num15 > liquid)
                                {
                                    num15 = liquid;
                                }
                                Tile tile1 = world.getTile()[x, y + 1];
                                tile1.liquid = (byte)(tile1.liquid + ((byte)num15));
                                liquid = (byte)(liquid - ((byte)num15));
                                if (liquid <= 0)
                                {
                                    num++;
                                    break;
                                }
                            }
                            if (num14 == 0)
                            {
                                if ((world.getTile()[x + num10, y].liquid == 0) && ((!world.getTile()[x + num10, y].active || !Statics.tileSolid[world.getTile()[x + num10, y].type]) || Statics.tileSolidTop[world.getTile()[x + num10, y].type]))
                                {
                                    num14 = num10;
                                }
                                else if ((world.getTile()[x - num10, y].liquid == 0) && ((!world.getTile()[x - num10, y].active || !Statics.tileSolid[world.getTile()[x - num10, y].type]) || Statics.tileSolidTop[world.getTile()[x - num10, y].type]))
                                {
                                    num14 = -num10;
                                }
                            }
                            if (((num14 != 0) && (world.getTile()[x + num14, y].liquid == 0)) && ((!world.getTile()[x + num14, y].active || !Statics.tileSolid[world.getTile()[x + num14, y].type]) || Statics.tileSolidTop[world.getTile()[x + num14, y].type]))
                            {
                                flag3 = true;
                                x += num14;
                            }
                            if (flag && !flag3)
                            {
                                flag = false;
                                flag3 = true;
                                num10 = -num8;
                                num14 = 0;
                            }
                        }
                        if ((k != x) && (i != y))
                        {
                            num++;
                        }
                        world.getTile()[x, y].liquid = liquid;
                        world.getTile()[x, y].lava = lava;
                        if ((world.getTile()[x - 1, y].liquid > 0) && (world.getTile()[x - 1, y].lava != lava))
                        {
                            if (lava)
                            {
                                LavaCheck(x, y, world);
                            }
                            else
                            {
                                LavaCheck(x - 1, y, world);
                            }
                        }
                        else if ((world.getTile()[x + 1, y].liquid > 0) && (world.getTile()[x + 1, y].lava != lava))
                        {
                            if (lava)
                            {
                                LavaCheck(x, y, world);
                            }
                            else
                            {
                                LavaCheck(x + 1, y, world);
                            }
                        }
                        else if ((world.getTile()[x, y - 1].liquid > 0) && (world.getTile()[x, y - 1].lava != lava))
                        {
                            if (lava)
                            {
                                LavaCheck(x, y, world);
                            }
                            else
                            {
                                LavaCheck(x, y - 1, world);
                            }
                        }
                        else if ((world.getTile()[x, y + 1].liquid > 0) && (world.getTile()[x, y + 1].lava != lava))
                        {
                            if (lava)
                            {
                                LavaCheck(x, y, world);
                            }
                            else
                            {
                                LavaCheck(x, y + 1, world);
                            }
                        }
                    }
                }
            }
            return (double)num;
        }

        public void Update(World world)
        {
            if ((world.getTile()[this.x, this.y].active && Statics.tileSolid[world.getTile()[this.x, this.y].type]) && !Statics.tileSolidTop[world.getTile()[this.x, this.y].type])
            {
                if (world.getTile()[this.x, this.y].type != 10)
                {
                    world.getTile()[this.x, this.y].liquid = 0;
                }
                this.kill = 9;
            }
            else
            {
                byte liquid = world.getTile()[this.x, this.y].liquid;
                float num2 = 0f;
                if (world.getTile()[this.x, this.y].liquid == 0)
                {
                    this.kill = 9;
                }
                else
                {
                    if (world.getTile()[this.x, this.y].lava)
                    {
                        LavaCheck(this.x, this.y, world);
                        if (!quickFall)
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
                        if (world.getTile()[this.x - 1, this.y].lava)
                        {
                            AddWater(this.x - 1, this.y, world);
                        }
                        if (world.getTile()[this.x + 1, this.y].lava)
                        {
                            AddWater(this.x + 1, this.y, world);
                        }
                        if (world.getTile()[this.x, this.y - 1].lava)
                        {
                            AddWater(this.x, this.y - 1, world);
                        }
                        if (world.getTile()[this.x, this.y + 1].lava)
                        {
                            AddWater(this.x, this.y + 1, world);
                        }
                    }
                    if ((((!world.getTile()[this.x, this.y + 1].active || !Statics.tileSolid[world.getTile()[this.x, this.y + 1].type]) || Statics.tileSolidTop[world.getTile()[this.x, this.y + 1].type]) && ((world.getTile()[this.x, this.y + 1].liquid <= 0) || (world.getTile()[this.x, this.y + 1].lava == world.getTile()[this.x, this.y].lava))) && (world.getTile()[this.x, this.y + 1].liquid < 0xff))
                    {
                        num2 = 0xff - world.getTile()[this.x, this.y + 1].liquid;
                        if (num2 > world.getTile()[this.x, this.y].liquid)
                        {
                            num2 = world.getTile()[this.x, this.y].liquid;
                        }
                        Tile tile1 = world.getTile()[this.x, this.y];
                        tile1.liquid = (byte)(tile1.liquid - ((byte)num2));
                        Tile tile2 = world.getTile()[this.x, this.y + 1];
                        tile2.liquid = (byte)(tile2.liquid + ((byte)num2));
                        world.getTile()[this.x, this.y + 1].lava = world.getTile()[this.x, this.y].lava;
                        AddWater(this.x, this.y + 1, world);
                        world.getTile()[this.x, this.y + 1].skipLiquid = true;
                        world.getTile()[this.x, this.y].skipLiquid = true;
                        if (world.getTile()[this.x, this.y].liquid > 250)
                        {
                            world.getTile()[this.x, this.y].liquid = 0xff;
                        }
                        else
                        {
                            AddWater(this.x - 1, this.y, world);
                            AddWater(this.x + 1, this.y, world);
                        }
                    }
                    if (world.getTile()[this.x, this.y].liquid > 0)
                    {
                        bool flag = true;
                        bool flag2 = true;
                        bool flag3 = true;
                        bool flag4 = true;
                        if ((world.getTile()[this.x - 1, this.y].active && Statics.tileSolid[world.getTile()[this.x - 1, this.y].type]) && !Statics.tileSolidTop[world.getTile()[this.x - 1, this.y].type])
                        {
                            flag = false;
                        }
                        else if ((world.getTile()[this.x - 1, this.y].liquid > 0) && (world.getTile()[this.x - 1, this.y].lava != world.getTile()[this.x, this.y].lava))
                        {
                            flag = false;
                        }
                        else if ((world.getTile()[this.x - 2, this.y].active && Statics.tileSolid[world.getTile()[this.x - 2, this.y].type]) && !Statics.tileSolidTop[world.getTile()[this.x - 2, this.y].type])
                        {
                            flag3 = false;
                        }
                        else if (world.getTile()[this.x - 2, this.y].liquid == 0)
                        {
                            flag3 = false;
                        }
                        else if ((world.getTile()[this.x - 2, this.y].liquid > 0) && (world.getTile()[this.x - 2, this.y].lava != world.getTile()[this.x, this.y].lava))
                        {
                            flag3 = false;
                        }
                        if ((world.getTile()[this.x + 1, this.y].active && Statics.tileSolid[world.getTile()[this.x + 1, this.y].type]) && !Statics.tileSolidTop[world.getTile()[this.x + 1, this.y].type])
                        {
                            flag2 = false;
                        }
                        else if ((world.getTile()[this.x + 1, this.y].liquid > 0) && (world.getTile()[this.x + 1, this.y].lava != world.getTile()[this.x, this.y].lava))
                        {
                            flag2 = false;
                        }
                        else if ((world.getTile()[this.x + 2, this.y].active && Statics.tileSolid[world.getTile()[this.x + 2, this.y].type]) && !Statics.tileSolidTop[world.getTile()[this.x + 2, this.y].type])
                        {
                            flag4 = false;
                        }
                        else if (world.getTile()[this.x + 2, this.y].liquid == 0)
                        {
                            flag4 = false;
                        }
                        else if ((world.getTile()[this.x + 2, this.y].liquid > 0) && (world.getTile()[this.x + 2, this.y].lava != world.getTile()[this.x, this.y].lava))
                        {
                            flag4 = false;
                        }
                        int num3 = 0;
                        if (world.getTile()[this.x, this.y].liquid < 3)
                        {
                            num3 = -1;
                        }
                        if (flag && flag2)
                        {
                            if (flag3 && flag4)
                            {
                                bool flag5 = true;
                                bool flag6 = true;
                                if ((world.getTile()[this.x - 3, this.y].active && Statics.tileSolid[world.getTile()[this.x - 3, this.y].type]) && !Statics.tileSolidTop[world.getTile()[this.x - 3, this.y].type])
                                {
                                    flag5 = false;
                                }
                                else if (world.getTile()[this.x - 3, this.y].liquid == 0)
                                {
                                    flag5 = false;
                                }
                                else if (world.getTile()[this.x - 3, this.y].lava != world.getTile()[this.x, this.y].lava)
                                {
                                    flag5 = false;
                                }
                                if ((world.getTile()[this.x + 3, this.y].active && Statics.tileSolid[world.getTile()[this.x + 3, this.y].type]) && !Statics.tileSolidTop[world.getTile()[this.x + 3, this.y].type])
                                {
                                    flag6 = false;
                                }
                                else if (world.getTile()[this.x + 3, this.y].liquid == 0)
                                {
                                    flag6 = false;
                                }
                                else if (world.getTile()[this.x + 3, this.y].lava != world.getTile()[this.x, this.y].lava)
                                {
                                    flag6 = false;
                                }
                                if (flag5 && flag6)
                                {
                                    num2 = ((((((world.getTile()[this.x - 1, this.y].liquid + world.getTile()[this.x + 1, this.y].liquid) + world.getTile()[this.x - 2, this.y].liquid) + world.getTile()[this.x + 2, this.y].liquid) + world.getTile()[this.x - 3, this.y].liquid) + world.getTile()[this.x + 3, this.y].liquid) + world.getTile()[this.x, this.y].liquid) + num3;
                                    num2 = (float)Math.Round((double)(num2 / 7f));
                                    int num4 = 0;
                                    if (world.getTile()[this.x - 1, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x - 1, this.y, world);
                                        world.getTile()[this.x - 1, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    world.getTile()[this.x - 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if (world.getTile()[this.x + 1, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x + 1, this.y, world);
                                        world.getTile()[this.x + 1, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    world.getTile()[this.x + 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if (world.getTile()[this.x - 2, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x - 2, this.y, world);
                                        world.getTile()[this.x - 2, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    world.getTile()[this.x - 2, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if (world.getTile()[this.x + 2, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x + 2, this.y, world);
                                        world.getTile()[this.x + 2, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    world.getTile()[this.x + 2, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if (world.getTile()[this.x - 3, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x - 3, this.y, world);
                                        world.getTile()[this.x - 3, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    world.getTile()[this.x - 3, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if (world.getTile()[this.x + 3, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x + 3, this.y, world);
                                        world.getTile()[this.x + 3, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    world.getTile()[this.x + 3, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if ((world.getTile()[this.x - 1, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x - 1, this.y, world);
                                    }
                                    if ((world.getTile()[this.x + 1, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x + 1, this.y, world);
                                    }
                                    if ((world.getTile()[this.x - 2, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x - 2, this.y, world);
                                    }
                                    if ((world.getTile()[this.x + 2, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x + 2, this.y, world);
                                    }
                                    if ((world.getTile()[this.x - 3, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x - 3, this.y, world);
                                    }
                                    if ((world.getTile()[this.x + 3, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x + 3, this.y, world);
                                    }
                                    if ((num4 != 6) || (world.getTile()[this.x, this.y - 1].liquid <= 0))
                                    {
                                        world.getTile()[this.x, this.y].liquid = (byte)num2;
                                    }
                                }
                                else
                                {
                                    int num5 = 0;
                                    num2 = ((((world.getTile()[this.x - 1, this.y].liquid + world.getTile()[this.x + 1, this.y].liquid) + world.getTile()[this.x - 2, this.y].liquid) + world.getTile()[this.x + 2, this.y].liquid) + world.getTile()[this.x, this.y].liquid) + num3;
                                    num2 = (float)Math.Round((double)(num2 / 5f));
                                    if (world.getTile()[this.x - 1, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x - 1, this.y, world);
                                        world.getTile()[this.x - 1, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num5++;
                                    }
                                    world.getTile()[this.x - 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if (world.getTile()[this.x + 1, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x + 1, this.y, world);
                                        world.getTile()[this.x + 1, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num5++;
                                    }
                                    world.getTile()[this.x + 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if (world.getTile()[this.x - 2, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x - 2, this.y, world);
                                        world.getTile()[this.x - 2, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num5++;
                                    }
                                    world.getTile()[this.x - 2, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if (world.getTile()[this.x + 2, this.y].liquid != ((byte)num2))
                                    {
                                        AddWater(this.x + 2, this.y, world);
                                        world.getTile()[this.x + 2, this.y].liquid = (byte)num2;
                                    }
                                    else
                                    {
                                        num5++;
                                    }
                                    if ((world.getTile()[this.x - 1, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x - 1, this.y, world);
                                    }
                                    if ((world.getTile()[this.x + 1, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x + 1, this.y, world);
                                    }
                                    if ((world.getTile()[this.x - 2, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x - 2, this.y, world);
                                    }
                                    if ((world.getTile()[this.x + 2, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                    {
                                        AddWater(this.x + 2, this.y, world);
                                    }
                                    world.getTile()[this.x + 2, this.y].lava = world.getTile()[this.x, this.y].lava;
                                    if ((num5 != 4) || (world.getTile()[this.x, this.y - 1].liquid <= 0))
                                    {
                                        world.getTile()[this.x, this.y].liquid = (byte)num2;
                                    }
                                }
                            }
                            else if (flag3)
                            {
                                num2 = (((world.getTile()[this.x - 1, this.y].liquid + world.getTile()[this.x + 1, this.y].liquid) + world.getTile()[this.x - 2, this.y].liquid) + world.getTile()[this.x, this.y].liquid) + num3;
                                num2 = (float)Math.Round((double)((num2 / 4f) + 0.001));
                                if ((world.getTile()[this.x - 1, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                {
                                    AddWater(this.x - 1, this.y, world);
                                    world.getTile()[this.x - 1, this.y].liquid = (byte)num2;
                                }
                                world.getTile()[this.x - 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                if ((world.getTile()[this.x + 1, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                {
                                    AddWater(this.x + 1, this.y, world);
                                    world.getTile()[this.x + 1, this.y].liquid = (byte)num2;
                                }
                                world.getTile()[this.x + 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                if ((world.getTile()[this.x - 2, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                {
                                    world.getTile()[this.x - 2, this.y].liquid = (byte)num2;
                                    AddWater(this.x - 2, this.y, world);
                                }
                                world.getTile()[this.x - 2, this.y].lava = world.getTile()[this.x, this.y].lava;
                                world.getTile()[this.x, this.y].liquid = (byte)num2;
                            }
                            else if (flag4)
                            {
                                num2 = (((world.getTile()[this.x - 1, this.y].liquid + world.getTile()[this.x + 1, this.y].liquid) + world.getTile()[this.x + 2, this.y].liquid) + world.getTile()[this.x, this.y].liquid) + num3;
                                num2 = (float)Math.Round((double)((num2 / 4f) + 0.001));
                                if ((world.getTile()[this.x - 1, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                {
                                    AddWater(this.x - 1, this.y, world);
                                    world.getTile()[this.x - 1, this.y].liquid = (byte)num2;
                                }
                                world.getTile()[this.x - 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                if ((world.getTile()[this.x + 1, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                {
                                    AddWater(this.x + 1, this.y, world);
                                    world.getTile()[this.x + 1, this.y].liquid = (byte)num2;
                                }
                                world.getTile()[this.x + 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                if ((world.getTile()[this.x + 2, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x, this.y].liquid != ((byte)num2)))
                                {
                                    world.getTile()[this.x + 2, this.y].liquid = (byte)num2;
                                    AddWater(this.x + 2, this.y, world);
                                }
                                world.getTile()[this.x + 2, this.y].lava = world.getTile()[this.x, this.y].lava;
                                world.getTile()[this.x, this.y].liquid = (byte)num2;
                            }
                            else
                            {
                                num2 = ((world.getTile()[this.x - 1, this.y].liquid + world.getTile()[this.x + 1, this.y].liquid) + world.getTile()[this.x, this.y].liquid) + num3;
                                num2 = (float)Math.Round((double)((num2 / 3f) + 0.001));
                                if (world.getTile()[this.x - 1, this.y].liquid != ((byte)num2))
                                {
                                    world.getTile()[this.x - 1, this.y].liquid = (byte)num2;
                                }
                                if ((world.getTile()[this.x, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x - 1, this.y].liquid != ((byte)num2)))
                                {
                                    AddWater(this.x - 1, this.y, world);
                                }
                                world.getTile()[this.x - 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                if (world.getTile()[this.x + 1, this.y].liquid != ((byte)num2))
                                {
                                    world.getTile()[this.x + 1, this.y].liquid = (byte)num2;
                                }
                                if ((world.getTile()[this.x, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x + 1, this.y].liquid != ((byte)num2)))
                                {
                                    AddWater(this.x + 1, this.y, world);
                                }
                                world.getTile()[this.x + 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                                world.getTile()[this.x, this.y].liquid = (byte)num2;
                            }
                        }
                        else if (flag)
                        {
                            num2 = (world.getTile()[this.x - 1, this.y].liquid + world.getTile()[this.x, this.y].liquid) + num3;
                            num2 = (float)Math.Round((double)((num2 / 2f) + 0.001));
                            if (world.getTile()[this.x - 1, this.y].liquid != ((byte)num2))
                            {
                                world.getTile()[this.x - 1, this.y].liquid = (byte)num2;
                            }
                            if ((world.getTile()[this.x, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x - 1, this.y].liquid != ((byte)num2)))
                            {
                                AddWater(this.x - 1, this.y, world);
                            }
                            world.getTile()[this.x - 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                            world.getTile()[this.x, this.y].liquid = (byte)num2;
                        }
                        else if (flag2)
                        {
                            num2 = (world.getTile()[this.x + 1, this.y].liquid + world.getTile()[this.x, this.y].liquid) + num3;
                            num2 = (float)Math.Round((double)((num2 / 2f) + 0.001));
                            if (world.getTile()[this.x + 1, this.y].liquid != ((byte)num2))
                            {
                                world.getTile()[this.x + 1, this.y].liquid = (byte)num2;
                            }
                            if ((world.getTile()[this.x, this.y].liquid != ((byte)num2)) || (world.getTile()[this.x + 1, this.y].liquid != ((byte)num2)))
                            {
                                AddWater(this.x + 1, this.y, world);
                            }
                            world.getTile()[this.x + 1, this.y].lava = world.getTile()[this.x, this.y].lava;
                            world.getTile()[this.x, this.y].liquid = (byte)num2;
                        }
                    }
                    if (world.getTile()[this.x, this.y].liquid != liquid)
                    {
                        if ((world.getTile()[this.x, this.y].liquid == 0xfe) && (liquid == 0xff))
                        {
                            world.getTile()[this.x, this.y].liquid = 0xff;
                            this.kill++;
                        }
                        else
                        {
                            AddWater(this.x, this.y - 1, world);
                            this.kill = 0;
                        }
                    }
                    else
                    {
                        this.kill++;
                    }
                }
            }
        }

        // Terraria.Liquid
        public static void UpdateLiquid(World world)
        {
            if (Statics.netMode == 2)
            {
                Liquid.cycles = 25;
                Liquid.maxLiquid = 5000;
            }
            if (!WorldGen.gen)
            {
                if (!Liquid.panicMode)
                {
                    if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > 4000)
                    {
                        Liquid.panicCounter++;
                        if (Liquid.panicCounter > 1800 || Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > 13500)
                        {
                            WorldGen.waterLine = world.getMaxTilesY();
                            Liquid.numLiquid = 0;
                            LiquidBuffer.numLiquidBuffer = 0;
                            Liquid.panicCounter = 0;
                            Liquid.panicMode = true;
                            Liquid.panicY = world.getMaxTilesY() - 3;
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
                        Liquid.QuickWater(world, 0, Liquid.panicY, Liquid.panicY);
                        Liquid.panicY--;
                        if (Liquid.panicY < 3)
                        {
                            Liquid.panicCounter = 0;
                            Liquid.panicMode = false;
                            WorldGen.WaterCheck(world);
                            if (Statics.netMode == 2)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    for (int j = 0; j < world.getMaxSectionsX(); j++)
                                    {
                                        for (int k = 0; k < world.getMaxSectionsY(); k++)
                                        {
                                            world.getServer().getNetPlay().serverSock[i].tileSection[j, k] = false;
                                        }
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
                int arg_1B7_0 = Statics.netMode;
                Liquid.wetCounter = Liquid.cycles;
            }
            if (Liquid.quickFall)
            {
                for (int l = num3; l < num4; l++)
                {
                    world.getLiquid()[l].delay = 10;
                    world.getLiquid()[l].Update(world);
                    world.getTile()[world.getLiquid()[l].x, world.getLiquid()[l].y].skipLiquid = false;
                }
            }
            else
            {
                for (int m = num3; m < num4; m++)
                {
                    if (!world.getTile()[world.getLiquid()[m].x, world.getLiquid()[m].y].skipLiquid)
                    {
                        world.getLiquid()[m].Update(world);
                    }
                    else
                    {
                        world.getTile()[world.getLiquid()[m].x, world.getLiquid()[m].y].skipLiquid = false;
                    }
                }
            }
            if (Liquid.wetCounter >= Liquid.cycles)
            {
                Liquid.wetCounter = 0;
                for (int n = Liquid.numLiquid - 1; n >= 0; n--)
                {
                    if (world.getLiquid()[n].kill > 3)
                    {
                        Liquid.DelWater(n, world);
                    }
                }
                int num5 = Liquid.maxLiquid - (Liquid.maxLiquid - Liquid.numLiquid);
                if (num5 > LiquidBuffer.numLiquidBuffer)
                {
                    num5 = LiquidBuffer.numLiquidBuffer;
                }
                for (int num6 = 0; num6 < num5; num6++)
                {
                    world.getTile()[world.getLiquidBuffer()[0].x, world.getLiquidBuffer()[0].y].checkingLiquid = false;
                    Liquid.AddWater(world.getLiquidBuffer()[0].x, world.getLiquidBuffer()[0].y, world);
                    LiquidBuffer.DelBuffer(0, world);
                }
                if (Liquid.numLiquid > 0 && Liquid.numLiquid > Liquid.stuckAmount - 50 && Liquid.numLiquid < Liquid.stuckAmount + 50)
                {
                    Liquid.stuckCount++;
                    if (Liquid.stuckCount >= 10000)
                    {
                        Liquid.stuck = true;
                        for (int num7 = Liquid.numLiquid - 1; num7 >= 0; num7--)
                        {
                            Liquid.DelWater(num7, world);
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

    }
}
