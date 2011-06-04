using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Terraria_Server
{
    public class WorldGen
    {
        public static bool gen = false;
        private static int lastMaxTilesX = 0;
        private static int lastMaxTilesY = 0;
        private static int preserve = 0;
        public static int bestX = 0;
        public static int bestY = 0;
        public static int hiScore = 0;

        public static Random genRand { get; set; }
        public static int waterLine;
        public static bool shadowOrbSmashed = false;
        public static int spawnDelay = 0;
        public static bool spawnEye = false;
        public static bool spawnMeteor = false;
        public static int spawnNPC = 0;
        public static int numRoomTiles;
        public static bool[] houseTile = new bool[80];
        private static int[] JChestX = new int[100];
        private static int[] JChestY = new int[100];
        public static int maxRoomTiles = 0x76c;
        public static int maxDRooms = 100;
        public static int[] roomX = new int[maxRoomTiles];
        public static int roomX1;
        public static int roomX2;
		private static int[] fihX = new int[300];
		private static int[] fihY = new int[300];
        public static int[] roomY = new int[maxRoomTiles];
        public static int roomY1;
        public static int roomY2;
        public static bool canSpawn;
        private static bool destroyObject = false;
        private static bool mergeDown = false;
        private static bool mergeLeft = false;
        private static bool mergeRight = false;
        private static bool mergeUp = false;
        public static bool noLiquidCheck = false;
        public static int shadowOrbCount = 0;
        public static bool worldCleared = false;
        private static bool stopDrops = false;
        private static int[] mCaveX = new int[300];
        private static int[] mCaveY = new int[300];


        private static int dMaxX;
        private static int dMaxY;
        private static int dMinX;
        private static int dMinY;
        private static double dxStrength1;
        private static double dxStrength2;
        private static double dyStrength1;
        private static double dyStrength2;
        private static int[] DPlatX = new int[300];
        private static int[] DPlatY = new int[300];
        private static int[] dRoomB = new int[maxDRooms];
        private static int[] dRoomL = new int[maxDRooms];
        private static int[] dRoomR = new int[maxDRooms];
        public static int[] dRoomSize = new int[maxDRooms];
        private static int[] dRoomT = new int[maxDRooms];
        private static bool[] dRoomTreasure = new bool[maxDRooms];
        public static int[] dRoomX = new int[maxDRooms];
        public static int[] dRoomY = new int[maxDRooms];
        private static int[] DDoorPos = new int[300];
        private static int[] DDoorX = new int[300];
        private static int[] DDoorY = new int[300];
        public static bool dSurface = false;


        /*private static bool tempBloodMoon = Main.bloodMoon;
        private static bool tempDayTime = Main.dayTime;
        private static int tempMoonPhase = Main.moonPhase;
        private static double tempTime = Main.time;*/

        public static void IslandHouse(int i, int j, World world)
        {
            byte num = (byte)genRand.Next(0x2d, 0x30);
            byte num2 = (byte)genRand.Next(10, 13);
            Vector2 vector = new Vector2((float)i, (float)j);
            int num7 = 1;
            if (genRand.Next(2) == 0)
            {
                num7 = -1;
            }
            int num8 = genRand.Next(7, 12);
            int num9 = genRand.Next(5, 7);
            vector.X = i + ((num8 + 2) * num7);
            for (int k = j - 15; k < (j + 30); k++)
            {
                if (world.getTile()[(int)vector.X, k].active)
                {
                    vector.Y = k - 1;
                    break;
                }
            }
            vector.X = i;
            int num3 = (int)((vector.X - num8) - 2f);
            int maxTilesX = (int)((vector.X + num8) + 2f);
            int num4 = (int)((vector.Y - num9) - 2f);
            int maxTilesY = ((int)(vector.Y + 2f)) + genRand.Next(3, 5);
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int m = num3; m <= maxTilesX; m++)
            {
                for (int num12 = num4; num12 < maxTilesY; num12++)
                {
                    world.getTile()[m, num12].active = true;
                    world.getTile()[m, num12].type = num;
                    world.getTile()[m, num12].wall = 0;
                }
            }
            num3 = ((int)vector.X) - num8;
            maxTilesX = ((int)vector.X) + num8;
            num4 = ((int)vector.Y) - num9;
            maxTilesY = (int)(vector.Y + 1f);
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int n = num3; n <= maxTilesX; n++)
            {
                for (int num14 = num4; num14 < maxTilesY; num14++)
                {
                    if (world.getTile()[n, num14].wall == 0)
                    {
                        world.getTile()[n, num14].active = false;
                        world.getTile()[n, num14].wall = num2;
                    }
                }
            }
            int num15 = i + ((num8 + 1) * num7);
            int y = (int)vector.Y;
            for (int num17 = num15 - 2; num17 <= (num15 + 2); num17++)
            {
                world.getTile()[num17, y].active = false;
                world.getTile()[num17, y - 1].active = false;
                world.getTile()[num17, y - 2].active = false;
            }
            PlaceTile(num15, y, world, 10, true, false, -1);
            int contain = 0;
            int houseCount = WorldGen.houseCount;
            if (houseCount > 2)
            {
                houseCount = genRand.Next(3);
            }
            switch (houseCount)
            {
                case 0:
                    contain = 0x9f;
                    break;

                case 1:
                    contain = 0x41;
                    break;

                case 2:
                    contain = 0x9e;
                    break;
            }
            AddBuriedChest(i, y - 3, world, contain);
            WorldGen.houseCount++;
        }

        public static void GrowEpicTree(int i, int y, World world)
        {
            int num2;
            int num = y;
            while (world.getTile()[i, num].type == 20)
            {
                num++;
            }
            if ((((!world.getTile()[i, num].active || (world.getTile()[i, num].type != 2)) || 
                ((world.getTile()[i, num - 1].wall != 0) || (world.getTile()[i, num - 1].liquid != 0))) || 
                ((!world.getTile()[i - 1, num].active || (world.getTile()[i - 1, num].type != 2)) || 
                (!world.getTile()[i + 1, num].active || (world.getTile()[i + 1, num].type != 2)))) || 
                !EmptyTileCheck(i - 2, i + 2, num - 0x37, num - 1, world, 20))
            {
                return;
            }
            bool flag = false;
            bool flag2 = false;
            int num4 = genRand.Next(20, 30);
            for (int j = num - num4; j < num; j++)
            {
                world.getTile()[i, j].frameNumber = (byte)genRand.Next(3);
                world.getTile()[i, j].active = true;
                world.getTile()[i, j].type = 5;
                num2 = genRand.Next(3);
                int num3 = genRand.Next(10);
                if ((j == (num - 1)) || (j == (num - num4)))
                {
                    num3 = 0;
                }
                while ((((num3 == 5) || (num3 == 7)) && flag) || (((num3 == 6) || (num3 == 7)) && flag2))
                {
                    num3 = genRand.Next(10);
                }
                flag = false;
                flag2 = false;
                if ((num3 == 5) || (num3 == 7))
                {
                    flag = true;
                }
                if ((num3 == 6) || (num3 == 7))
                {
                    flag2 = true;
                }
                if (num3 == 1)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 2)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0x16;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0x2c;
                            break;
                    }
                }
                else if (num3 == 3)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x2c;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x2c;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x2c;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 4)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 5)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x58;
                            world.getTile()[i, j].frameY = 0;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x58;
                            world.getTile()[i, j].frameY = 0x16;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x58;
                            world.getTile()[i, j].frameY = 0x2c;
                            break;
                    }
                }
                else if (num3 == 6)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x42;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x42;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x42;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 7)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 110;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 110;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 110;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x16;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x2c;
                            break;
                    }
                }
                if ((num3 == 5) || (num3 == 7))
                {
                    world.getTile()[i - 1, j].active = true;
                    world.getTile()[i - 1, j].type = 5;
                    num2 = genRand.Next(3);
                    if (genRand.Next(3) < 2)
                    {
                        switch (num2)
                        {
                            case 0:
                                world.getTile()[i - 1, j].frameX = 0x2c;
                                world.getTile()[i - 1, j].frameY = 0xc6;
                                break;

                            case 1:
                                world.getTile()[i - 1, j].frameX = 0x2c;
                                world.getTile()[i - 1, j].frameY = 220;
                                break;

                            case 2:
                                world.getTile()[i - 1, j].frameX = 0x2c;
                                world.getTile()[i - 1, j].frameY = 0xf2;
                                break;
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 0:
                                world.getTile()[i - 1, j].frameX = 0x42;
                                world.getTile()[i - 1, j].frameY = 0;
                                break;

                            case 1:
                                world.getTile()[i - 1, j].frameX = 0x42;
                                world.getTile()[i - 1, j].frameY = 0x16;
                                break;

                            case 2:
                                world.getTile()[i - 1, j].frameX = 0x42;
                                world.getTile()[i - 1, j].frameY = 0x2c;
                                break;
                        }
                    }
                }
                if ((num3 == 6) || (num3 == 7))
                {
                    world.getTile()[i + 1, j].active = true;
                    world.getTile()[i + 1, j].type = 5;
                    num2 = genRand.Next(3);
                    if (genRand.Next(3) < 2)
                    {
                        switch (num2)
                        {
                            case 0:
                                world.getTile()[i + 1, j].frameX = 0x42;
                                world.getTile()[i + 1, j].frameY = 0xc6;
                                break;

                            case 1:
                                world.getTile()[i + 1, j].frameX = 0x42;
                                world.getTile()[i + 1, j].frameY = 220;
                                break;

                            case 2:
                                world.getTile()[i + 1, j].frameX = 0x42;
                                world.getTile()[i + 1, j].frameY = 0xf2;
                                break;
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 0:
                                world.getTile()[i + 1, j].frameX = 0x58;
                                world.getTile()[i + 1, j].frameY = 0x42;
                                break;

                            case 1:
                                world.getTile()[i + 1, j].frameX = 0x58;
                                world.getTile()[i + 1, j].frameY = 0x58;
                                break;

                            case 2:
                                world.getTile()[i + 1, j].frameX = 0x58;
                                world.getTile()[i + 1, j].frameY = 110;
                                break;
                        }
                    }
                }
            }
            int num6 = genRand.Next(3);
            if ((num6 == 0) || (num6 == 1))
            {
                world.getTile()[i + 1, num - 1].active = true;
                world.getTile()[i + 1, num - 1].type = 5;
                switch (genRand.Next(3))
                {
                    case 0:
                        world.getTile()[i + 1, num - 1].frameX = 0x16;
                        world.getTile()[i + 1, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i + 1, num - 1].frameX = 0x16;
                        world.getTile()[i + 1, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i + 1, num - 1].frameX = 0x16;
                        world.getTile()[i + 1, num - 1].frameY = 0xb0;
                        goto Label_0A36;
                }
            }
        Label_0A36:
            if ((num6 == 0) || (num6 == 2))
            {
                world.getTile()[i - 1, num - 1].active = true;
                world.getTile()[i - 1, num - 1].type = 5;
                switch (genRand.Next(3))
                {
                    case 0:
                        world.getTile()[i - 1, num - 1].frameX = 0x2c;
                        world.getTile()[i - 1, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i - 1, num - 1].frameX = 0x2c;
                        world.getTile()[i - 1, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i - 1, num - 1].frameX = 0x2c;
                        world.getTile()[i - 1, num - 1].frameY = 0xb0;
                        break;
                }
            }
            num2 = genRand.Next(3);
            if (num6 == 0)
            {
                switch (num2)
                {
                    case 0:
                        world.getTile()[i, num - 1].frameX = 0x58;
                        world.getTile()[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i, num - 1].frameX = 0x58;
                        world.getTile()[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i, num - 1].frameX = 0x58;
                        world.getTile()[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            else if (num6 == 1)
            {
                switch (num2)
                {
                    case 0:
                        world.getTile()[i, num - 1].frameX = 0;
                        world.getTile()[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i, num - 1].frameX = 0;
                        world.getTile()[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i, num - 1].frameX = 0;
                        world.getTile()[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            else if (num6 == 2)
            {
                switch (num2)
                {
                    case 0:
                        world.getTile()[i, num - 1].frameX = 0x42;
                        world.getTile()[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i, num - 1].frameX = 0x42;
                        world.getTile()[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i, num - 1].frameX = 0x42;
                        world.getTile()[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            if (genRand.Next(3) < 2)
            {
                switch (genRand.Next(3))
                {
                    case 0:
                        world.getTile()[i, num - num4].frameX = 0x16;
                        world.getTile()[i, num - num4].frameY = 0xc6;
                        break;

                    case 1:
                        world.getTile()[i, num - num4].frameX = 0x16;
                        world.getTile()[i, num - num4].frameY = 220;
                        break;

                    case 2:
                        world.getTile()[i, num - num4].frameX = 0x16;
                        world.getTile()[i, num - num4].frameY = 0xf2;
                        break;
                }
            }
            else
            {
                switch (genRand.Next(3))
                {
                    case 0:
                        world.getTile()[i, num - num4].frameX = 0;
                        world.getTile()[i, num - num4].frameY = 0xc6;
                        break;

                    case 1:
                        world.getTile()[i, num - num4].frameX = 0;
                        world.getTile()[i, num - num4].frameY = 220;
                        break;

                    case 2:
                        world.getTile()[i, num - num4].frameX = 0;
                        world.getTile()[i, num - num4].frameY = 0xf2;
                        break;
                }
            }
            RangeFrame(i - 2, (num - num4) - 1, i + 2, num + 1, world);
            if (Statics.netMode == 2)
            {
                NetMessage.SendTileSquare(-1, i, num - ((int)(num4 * 0.5)), num4 + 1, world);
            }
        }

        public static void GrowShroom(int i, int y, World world)
        {
            int num = y;
            if (((!world.getTile()[i - 1, num - 1].lava && !world.getTile()[i - 1, num - 1].lava) && 
                !world.getTile()[i + 1, num - 1].lava) && (((world.getTile()[i, num].active && 
                (world.getTile()[i, num].type == 70)) && ((world.getTile()[i, num - 1].wall == 0) 
                && world.getTile()[i - 1, num].active)) && (((world.getTile()[i - 1, num].type == 70) 
                && world.getTile()[i + 1, num].active) && ((world.getTile()[i + 1, num].type == 70) && 
                EmptyTileCheck(i - 2, i + 2, num - 13, num - 1, world, 0x47)))))
            {
                int num3 = genRand.Next(4, 11);
                for (int j = num - num3; j < num; j++)
                {
                    world.getTile()[i, j].frameNumber = (byte)genRand.Next(3);
                    world.getTile()[i, j].active = true;
                    world.getTile()[i, j].type = 0x48;
                    switch (genRand.Next(3))
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x12;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x24;
                            break;
                    }
                }
                switch (genRand.Next(3))
                {
                    case 0:
                        world.getTile()[i, num - num3].frameX = 0x24;
                        world.getTile()[i, num - num3].frameY = 0;
                        break;

                    case 1:
                        world.getTile()[i, num - num3].frameX = 0x24;
                        world.getTile()[i, num - num3].frameY = 0x12;
                        break;

                    case 2:
                        world.getTile()[i, num - num3].frameX = 0x24;
                        world.getTile()[i, num - num3].frameY = 0x24;
                        break;
                }
                RangeFrame(i - 2, (num - num3) - 1, i + 2, num + 1, world);
                if (Statics.netMode == 2)
                {
                    NetMessage.SendTileSquare(-1, i, num - ((int)(num3 * 0.5)), num3 + 1, world);
                }
            }
        }

        public static void AddTrees(World world)
        {
            for (int i = 1; i < (world.getMaxTilesX() - 1); i++)
            {
                for (int j = 20; j < world.getWorldSurface(); j++)
                {
                    GrowTree(i, j, world);
                }
            }
        }

        public static void CaveOpenater(int i, int j, World world)
        {
            Vector2 vector = new Vector2();
            Vector2 vector2 = new Vector2();
            double num5 = genRand.Next(7, 12);
            double num6 = num5;
            int num7 = 1;
            if (genRand.Next(2) == 0)
            {
                num7 = -1;
            }
            vector.X = i;
            vector.Y = j;
            int num8 = 100;
            vector2.Y = 0f;
            vector2.X = num7;
            while (num8 > 0)
            {
                if (world.getTile()[(int)vector.X, (int)vector.Y].wall == 0)
                {
                    num8 = 0;
                }
                num8--;
                int num = (int)(vector.X - (num5 * 0.5));
                int maxTilesX = (int)(vector.X + (num5 * 0.5));
                int num2 = (int)(vector.Y - (num5 * 0.5));
                int maxTilesY = (int)(vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        float num11 = Math.Abs((float)(k - vector.X));
                        float num12 = Math.Abs((float)(m - vector.Y));
                        if (Math.Sqrt((double)((num11 * num11) + (num12 * num12))) < (num6 * 0.4))
                        {
                            world.getTile()[k, m].active = false;
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > (num7 + 0.5f))
                {
                    vector2.X = num7 + 0.5f;
                }
                if (vector2.X < (num7 - 0.5f))
                {
                    vector2.X = num7 - 0.5f;
                }
                if (vector2.Y > 0f)
                {
                    vector2.Y = 0f;
                }
                if (vector2.Y < -0.5)
                {
                    vector2.Y = -0.5f;
                }
            }
        }

        public static void Cavinator(int i, int j, int steps, World world)
        {
            Vector2 vector = new Vector2();
            Vector2 vector2 = new Vector2();
            double num5 = genRand.Next(7, 15);
            double num6 = num5;
            int num7 = 1;
            if (genRand.Next(2) == 0)
            {
                num7 = -1;
            }
            vector.X = i;
            vector.Y = j;
            int num8 = genRand.Next(20, 40);
            vector2.Y = genRand.Next(10, 20) * 0.01f;
            vector2.X = num7;
            while (num8 > 0)
            {
                num8--;
                int num = (int)(vector.X - (num5 * 0.5));
                int maxTilesX = (int)(vector.X + (num5 * 0.5));
                int num2 = (int)(vector.Y - (num5 * 0.5));
                int maxTilesY = (int)(vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        float num11 = Math.Abs((float)(k - vector.X));
                        float num12 = Math.Abs((float)(m - vector.Y));
                        if (Math.Sqrt((double)((num11 * num11) + (num12 * num12))) < (num6 * 0.4))
                        {
                            world.getTile()[k, m].active = false;
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > (num7 + 0.5f))
                {
                    vector2.X = num7 + 0.5f;
                }
                if (vector2.X < (num7 - 0.5f))
                {
                    vector2.X = num7 - 0.5f;
                }
                if (vector2.Y > 2f)
                {
                    vector2.Y = 2f;
                }
                if (vector2.Y < 0f)
                {
                    vector2.Y = 0f;
                }
            }
            if ((steps > 0) && (((int)vector.Y) < (world.getRockLayer() + 50.0)))
            {
                Cavinator((int)vector.X, (int)vector.Y, steps - 1, world);
            }
        }

        public static void AddShadowOrb(int x, int y, World world)
        {
            if (((x >= 10) && (x <= (world.getMaxTilesX() - 10))) && ((y >= 10) && (y <= (world.getMaxTilesY() - 10))))
            {
                world.getTile()[x - 1, y - 1].active = true;
                world.getTile()[x - 1, y - 1].type = 0x1f;
                world.getTile()[x - 1, y - 1].frameX = 0;
                world.getTile()[x - 1, y - 1].frameY = 0;
                world.getTile()[x, y - 1].active = true;
                world.getTile()[x, y - 1].type = 0x1f;
                world.getTile()[x, y - 1].frameX = 0x12;
                world.getTile()[x, y - 1].frameY = 0;
                world.getTile()[x - 1, y].active = true;
                world.getTile()[x - 1, y].type = 0x1f;
                world.getTile()[x - 1, y].frameX = 0;
                world.getTile()[x - 1, y].frameY = 0x12;
                world.getTile()[x, y].active = true;
                world.getTile()[x, y].type = 0x1f;
                world.getTile()[x, y].frameX = 0x12;
                world.getTile()[x, y].frameY = 0x12;
            }

        }
        
        public static void ChasmRunner(int i, int j, int steps, World world, bool makeOrb = false)
        {
            Vector2 vector = new Vector2();
            Vector2 vector2 = new Vector2();
            bool flag = false;
            bool flag2 = false;
            if (!makeOrb)
            {
                flag = true;
            }
            float num5 = steps;
            vector.X = i;
            vector.Y = j;
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = (genRand.Next(11) * 0.2f) + 0.5f;
            int num6 = 5;
            double num7 = genRand.Next(5) + 7;
            while (num7 > 0.0)
            {
                int num;
                int num2;
                int num3;
                int maxTilesY;
                if (num5 > 0f)
                {
                    num7 += genRand.Next(3);
                    num7 -= genRand.Next(3);
                    if (num7 < 7.0)
                    {
                        num7 = 7.0;
                    }
                    if (num7 > 20.0)
                    {
                        num7 = 20.0;
                    }
                    if ((num5 == 1f) && (num7 < 10.0))
                    {
                        num7 = 10.0;
                    }
                }
                else
                {
                    num7 -= genRand.Next(4);
                }
                if ((vector.Y > world.getRockLayer()) && (num5 > 0f))
                {
                    num5 = 0f;
                }
                num5--;
                if (num5 > num6)
                {
                    num = (int)(vector.X - (num7 * 0.5));
                    num3 = (int)(vector.X + (num7 * 0.5));
                    num2 = (int)(vector.Y - (num7 * 0.5));
                    maxTilesY = (int)(vector.Y + (num7 * 0.5));
                    if (num < 0)
                    {
                        num = 0;
                    }
                    if (num3 > (world.getMaxTilesX() - 1))
                    {
                        num3 = world.getMaxTilesX() - 1;
                    }
                    if (num2 < 0)
                    {
                        num2 = 0;
                    }
                    if (maxTilesY > world.getMaxTilesY())
                    {
                        maxTilesY = world.getMaxTilesY();
                    }
                    for (int n = num; n < num3; n++)
                    {
                        for (int num9 = num2; num9 < maxTilesY; num9++)
                        {
                            if ((Math.Abs((float)(n - vector.X)) + Math.Abs((float)(num9 - vector.Y))) < ((num7 * 0.5) * (1.0 + (genRand.Next(-10, 11) * 0.015))))
                            {
                                world.getTile()[n, num9].active = false;
                            }
                        }
                    }
                }
                if (num5 <= 0f)
                {
                    if (!flag)
                    {
                        flag = true;
                        AddShadowOrb((int)vector.X, (int)vector.Y, world);
                    }
                    else if (!flag2)
                    {
                        flag2 = false;
                        bool flag3 = false;
                        int num10 = 0;
                        while (!flag3)
                        {
                            int x = genRand.Next(((int)vector.X) - 0x19, ((int)vector.X) + 0x19);
                            int y = genRand.Next(((int)vector.Y) - 50, (int)vector.Y);
                            if (x < 5)
                            {
                                x = 5;
                            }
                            if (x > (world.getMaxTilesX() - 5))
                            {
                                x = world.getMaxTilesX() - 5;
                            }
                            if (y < 5)
                            {
                                y = 5;
                            }
                            if (y > (world.getMaxTilesY() - 5))
                            {
                                y = world.getMaxTilesY() - 5;
                            }
                            if (y > world.getWorldSurface())
                            {
                                Place3x2(x, y, 0x1a, world);
                                if (world.getTile()[x, y].type == 0x1a)
                                {
                                    flag3 = true;
                                }
                                else
                                {
                                    num10++;
                                    if (num10 >= 0x2710)
                                    {
                                        flag3 = true;
                                    }
                                }
                            }
                            else
                            {
                                flag3 = true;
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.01f;
                if (vector2.X > 0.3)
                {
                    vector2.X = 0.3f;
                }
                if (vector2.X < -0.3)
                {
                    vector2.X = -0.3f;
                }
                num = (int)(vector.X - (num7 * 1.1));
                num3 = (int)(vector.X + (num7 * 1.1));
                num2 = (int)(vector.Y - (num7 * 1.1));
                maxTilesY = (int)(vector.Y + (num7 * 1.1));
                if (num < 1)
                {
                    num = 1;
                }
                if (num3 > (world.getMaxTilesX() - 1))
                {
                    num3 = world.getMaxTilesX() - 1;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                for (int k = num; k < num3; k++)
                {
                    for (int num14 = num2; num14 < maxTilesY; num14++)
                    {
                        if ((Math.Abs((float)(k - vector.X)) + Math.Abs((float)(num14 - vector.Y))) < ((num7 * 1.1) * (1.0 + (genRand.Next(-10, 11) * 0.015))))
                        {
                            if ((world.getTile()[k, num14].type != 0x19) && (num14 > (j + genRand.Next(3, 20))))
                            {
                                world.getTile()[k, num14].active = true;
                            }
                            if (steps <= num6)
                            {
                                world.getTile()[k, num14].active = true;
                            }
                            if (world.getTile()[k, num14].type != 0x1f)
                            {
                                world.getTile()[k, num14].type = 0x19;
                            }
                            if (world.getTile()[k, num14].wall == 2)
                            {
                                world.getTile()[k, num14].wall = 0;
                            }
                        }
                    }
                }
                for (int m = num; m < num3; m++)
                {
                    for (int num16 = num2; num16 < maxTilesY; num16++)
                    {
                        if ((Math.Abs((float)(m - vector.X)) + Math.Abs((float)(num16 - vector.Y))) < ((num7 * 1.1) * (1.0 + (genRand.Next(-10, 11) * 0.015))))
                        {
                            if (world.getTile()[m, num16].type != 0x1f)
                            {
                                world.getTile()[m, num16].type = 0x19;
                            }
                            if (steps <= num6)
                            {
                                world.getTile()[m, num16].active = true;
                            }
                            if (num16 > (j + genRand.Next(3, 20)))
                            {
                                PlaceWall(m, num16, 3, world, true);
                            }
                        }
                    }
                }
            }
        }

        public static void DungeonEnt(int i, int j, int tileType, int wallType, World world)
        {
            Vector2 vector = new Vector2();
            double num5 = dxStrength1;
            double num6 = dyStrength1;
            vector.X = i;
            vector.Y = j - (((float)num6) / 2f);
            dMinY = (int)vector.Y;
            int num7 = 1;
            if (i > (world.getMaxTilesX() / 2))
            {
                num7 = -1;
            }
            int num = ((int)(vector.X - (num5 * 0.60000002384185791))) - genRand.Next(2, 5);
            int maxTilesX = ((int)(vector.X + (num5 * 0.60000002384185791))) + genRand.Next(2, 5);
            int num2 = ((int)(vector.Y - (num6 * 0.60000002384185791))) - genRand.Next(2, 5);
            int maxTilesY = ((int)(vector.Y + (num6 * 0.60000002384185791))) + genRand.Next(8, 0x10);
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int k = num; k < maxTilesX; k++)
            {
                for (int num9 = num2; num9 < maxTilesY; num9++)
                {
                    world.getTile()[k, num9].liquid = 0;
                    if (world.getTile()[k, num9].wall != wallType)
                    {
                        world.getTile()[k, num9].wall = 0;
                        if (((k > (num + 1)) && (k < (maxTilesX - 2))) && ((num9 > (num2 + 1)) && (num9 < (maxTilesY - 2))))
                        {
                            PlaceWall(k, num9, wallType, world, true);
                        }
                        world.getTile()[k, num9].active = true;
                        world.getTile()[k, num9].type = (byte)tileType;
                    }
                }
            }
            int num10 = num;
            int num11 = (num + 5) + genRand.Next(4);
            int num12 = (num2 - 3) - genRand.Next(3);
            int num13 = num2;
            for (int m = num10; m < num11; m++)
            {
                for (int num15 = num12; num15 < num13; num15++)
                {
                    if (world.getTile()[m, num15].wall != wallType)
                    {
                        world.getTile()[m, num15].active = true;
                        world.getTile()[m, num15].type = (byte)tileType;
                    }
                }
            }
            num10 = (maxTilesX - 5) - genRand.Next(4);
            num11 = maxTilesX;
            num12 = (num2 - 3) - genRand.Next(3);
            num13 = num2;
            for (int n = num10; n < num11; n++)
            {
                for (int num17 = num12; num17 < num13; num17++)
                {
                    if (world.getTile()[n, num17].wall != wallType)
                    {
                        world.getTile()[n, num17].active = true;
                        world.getTile()[n, num17].type = (byte)tileType;
                    }
                }
            }
            int num18 = 1 + genRand.Next(2);
            int num19 = 2 + genRand.Next(4);
            int num20 = 0;
            for (int num21 = num; num21 < maxTilesX; num21++)
            {
                for (int num22 = num2 - num18; num22 < num2; num22++)
                {
                    if (world.getTile()[num21, num22].wall != wallType)
                    {
                        world.getTile()[num21, num22].active = true;
                        world.getTile()[num21, num22].type = (byte)tileType;
                    }
                }
                num20++;
                if (num20 >= num19)
                {
                    num21 += num19;
                    num20 = 0;
                }
            }
            for (int num23 = num; num23 < maxTilesX; num23++)
            {
                for (int num24 = maxTilesY; num24 < (maxTilesY + 100); num24++)
                {
                    PlaceWall(num23, num24, 2, world, true);
                }
            }
            num = (int)(vector.X - (num5 * 0.60000002384185791));
            maxTilesX = (int)(vector.X + (num5 * 0.60000002384185791));
            num2 = (int)(vector.Y - (num6 * 0.60000002384185791));
            maxTilesY = (int)(vector.Y + (num6 * 0.60000002384185791));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int num25 = num; num25 < maxTilesX; num25++)
            {
                for (int num26 = num2; num26 < maxTilesY; num26++)
                {
                    PlaceWall(num25, num26, wallType, world, true);
                }
            }
            num = (int)((vector.X - (num5 * 0.6)) - 1.0);
            maxTilesX = (int)((vector.X + (num5 * 0.6)) + 1.0);
            num2 = (int)((vector.Y - (num6 * 0.6)) - 1.0);
            maxTilesY = (int)((vector.Y + (num6 * 0.6)) + 1.0);
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int num27 = num; num27 < maxTilesX; num27++)
            {
                for (int num28 = num2; num28 < maxTilesY; num28++)
                {
                    world.getTile()[num27, num28].wall = (byte)wallType;
                }
            }
            num = (int)(vector.X - (num5 * 0.5));
            maxTilesX = (int)(vector.X + (num5 * 0.5));
            num2 = (int)(vector.Y - (num6 * 0.5));
            maxTilesY = (int)(vector.Y + (num6 * 0.5));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int num29 = num; num29 < maxTilesX; num29++)
            {
                for (int num30 = num2; num30 < maxTilesY; num30++)
                {
                    world.getTile()[num29, num30].active = false;
                    world.getTile()[num29, num30].wall = (byte)wallType;
                }
            }
            DPlatX[numDPlats] = (int)vector.X;
            DPlatY[numDPlats] = maxTilesY;
            numDPlats++;
            vector.X += (((float)num5) * 0.6f) * num7;
            vector.Y += ((float)num6) * 0.5f;
            num5 = dxStrength2;
            num6 = dyStrength2;
            vector.X += (((float)num5) * 0.55f) * num7;
            vector.Y -= ((float)num6) * 0.5f;
            num = ((int)(vector.X - (num5 * 0.60000002384185791))) - genRand.Next(1, 3);
            maxTilesX = ((int)(vector.X + (num5 * 0.60000002384185791))) + genRand.Next(1, 3);
            num2 = ((int)(vector.Y - (num6 * 0.60000002384185791))) - genRand.Next(1, 3);
            maxTilesY = ((int)(vector.Y + (num6 * 0.60000002384185791))) + genRand.Next(6, 0x10);
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int num31 = num; num31 < maxTilesX; num31++)
            {
                for (int num32 = num2; num32 < maxTilesY; num32++)
                {
                    if (world.getTile()[num31, num32].wall != wallType)
                    {
                        bool flag = true;
                        if (num7 < 0)
                        {
                            if (num31 < (vector.X - (num5 * 0.5)))
                            {
                                flag = false;
                            }
                        }
                        else if (num31 > ((vector.X + (num5 * 0.5)) - 1.0))
                        {
                            flag = false;
                        }
                        if (flag)
                        {
                            world.getTile()[num31, num32].wall = 0;
                            world.getTile()[num31, num32].active = true;
                            world.getTile()[num31, num32].type = (byte)tileType;
                        }
                    }
                }
            }
            for (int num33 = num; num33 < maxTilesX; num33++)
            {
                for (int num34 = maxTilesY; num34 < (maxTilesY + 100); num34++)
                {
                    PlaceWall(num33, num34, 2, world, true);
                }
            }
            num = (int)(vector.X - (num5 * 0.5));
            maxTilesX = (int)(vector.X + (num5 * 0.5));
            num10 = num;
            if (num7 < 0)
            {
                num10++;
            }
            num11 = (num10 + 5) + genRand.Next(4);
            num12 = (num2 - 3) - genRand.Next(3);
            num13 = num2;
            for (int num35 = num10; num35 < num11; num35++)
            {
                for (int num36 = num12; num36 < num13; num36++)
                {
                    if (world.getTile()[num35, num36].wall != wallType)
                    {
                        world.getTile()[num35, num36].active = true;
                        world.getTile()[num35, num36].type = (byte)tileType;
                    }
                }
            }
            num10 = (maxTilesX - 5) - genRand.Next(4);
            num11 = maxTilesX;
            num12 = (num2 - 3) - genRand.Next(3);
            num13 = num2;
            for (int num37 = num10; num37 < num11; num37++)
            {
                for (int num38 = num12; num38 < num13; num38++)
                {
                    if (world.getTile()[num37, num38].wall != wallType)
                    {
                        world.getTile()[num37, num38].active = true;
                        world.getTile()[num37, num38].type = (byte)tileType;
                    }
                }
            }
            num18 = 1 + genRand.Next(2);
            num19 = 2 + genRand.Next(4);
            num20 = 0;
            if (num7 < 0)
            {
                maxTilesX++;
            }
            for (int num39 = num + 1; num39 < (maxTilesX - 1); num39++)
            {
                for (int num40 = num2 - num18; num40 < num2; num40++)
                {
                    if (world.getTile()[num39, num40].wall != wallType)
                    {
                        world.getTile()[num39, num40].active = true;
                        world.getTile()[num39, num40].type = (byte)tileType;
                    }
                }
                num20++;
                if (num20 >= num19)
                {
                    num39 += num19;
                    num20 = 0;
                }
            }
            num = (int)(vector.X - (num5 * 0.6));
            maxTilesX = (int)(vector.X + (num5 * 0.6));
            num2 = (int)(vector.Y - (num6 * 0.6));
            maxTilesY = (int)(vector.Y + (num6 * 0.6));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int num41 = num; num41 < maxTilesX; num41++)
            {
                for (int num42 = num2; num42 < maxTilesY; num42++)
                {
                    world.getTile()[num41, num42].wall = 0;
                }
            }
            num = (int)(vector.X - (num5 * 0.5));
            maxTilesX = (int)(vector.X + (num5 * 0.5));
            num2 = (int)(vector.Y - (num6 * 0.5));
            maxTilesY = (int)(vector.Y + (num6 * 0.5));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int num43 = num; num43 < maxTilesX; num43++)
            {
                for (int num44 = num2; num44 < maxTilesY; num44++)
                {
                    world.getTile()[num43, num44].active = false;
                    world.getTile()[num43, num44].wall = 0;
                }
            }
            for (int num45 = num; num45 < maxTilesX; num45++)
            {
                if (!world.getTile()[num45, maxTilesY].active)
                {
                    world.getTile()[num45, maxTilesY].active = true;
                    world.getTile()[num45, maxTilesY].type = 0x13;
                }
            }
            world.setDungeonX((int)vector.X);
            world.setDungeonY(maxTilesY);
            int index = NPC.NewNPC((dungeonX * 0x10) + 8, dungeonY * 0x10, world, 0x25, 0);
            world.getNPCs()[index].homeless = false;
            world.getNPCs()[index].homeTileX = world.getDungeonX();
            world.getNPCs()[index].homeTileY = world.getDungeonY();
            if (num7 == 1)
            {
                int num47 = 0;
                for (int num48 = maxTilesX; num48 < (maxTilesX + 0x19); num48++)
                {
                    num47++;
                    for (int num49 = maxTilesY + num47; num49 < (maxTilesY + 0x19); num49++)
                    {
                        world.getTile()[num48, num49].active = true;
                        world.getTile()[num48, num49].type = (byte)tileType;
                    }
                }
            }
            else
            {
                int num50 = 0;
                for (int num51 = num; num51 > (num - 0x19); num51--)
                {
                    num50++;
                    for (int num52 = maxTilesY + num50; num52 < (maxTilesY + 0x19); num52++)
                    {
                        world.getTile()[num51, num52].active = true;
                        world.getTile()[num51, num52].type = (byte)tileType;
                    }
                }
            }
            num18 = 1 + genRand.Next(2);
            num19 = 2 + genRand.Next(4);
            num20 = 0;
            num = (int)(vector.X - (num5 * 0.5));
            maxTilesX = (int)(vector.X + (num5 * 0.5));
            num += 2;
            maxTilesX -= 2;
            for (int num53 = num; num53 < maxTilesX; num53++)
            {
                for (int num54 = num2; num54 < maxTilesY; num54++)
                {
                    PlaceWall(num53, num54, wallType, world, true);
                }
                num20++;
                if (num20 >= num19)
                {
                    num53 += num19 * 2;
                    num20 = 0;
                }
            }
            vector.X -= (((float)num5) * 0.6f) * num7;
            vector.Y += ((float)num6) * 0.5f;
            num5 = 15.0;
            num6 = 3.0;
            vector.Y -= ((float)num6) * 0.5f;
            num = (int)(vector.X - (num5 * 0.5));
            maxTilesX = (int)(vector.X + (num5 * 0.5));
            num2 = (int)(vector.Y - (num6 * 0.5));
            maxTilesY = (int)(vector.Y + (num6 * 0.5));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > world.getMaxTilesY())
            {
                maxTilesY = world.getMaxTilesY();
            }
            for (int num55 = num; num55 < maxTilesX; num55++)
            {
                for (int num56 = num2; num56 < maxTilesY; num56++)
                {
                    world.getTile()[num55, num56].active = false;
                }
            }
            if (num7 < 0)
            {
                vector.X--;
            }
            PlaceTile((int)vector.X, ((int)vector.Y) + 1, world, 10, false, false, -1);
        }

        public static void DungeonHalls(int i, int j, int tileType, int wallType, World world, bool forceX = false)
        {
            Vector2 vector = new Vector2();
            Vector2 vector2 = new Vector2();
            double num5 = genRand.Next(4, 6);
            Vector2 vector3 = new Vector2();
            Vector2 vector4 = new Vector2();
            int num6 = 1;
            vector.X = i;
            vector.Y = j;
            int num7 = genRand.Next(0x23, 80);
            if (forceX)
            {
                num7 += 20;
                lastDungeonHall = new Vector2();
            }
            else if (genRand.Next(5) == 0)
            {
                num5 *= 2.0;
                num7 /= 2;
            }
            bool flag = false;
            while (!flag)
            {
                if (genRand.Next(2) == 0)
                {
                    num6 = -1;
                }
                else
                {
                    num6 = 1;
                }
                bool flag2 = false;
                if (genRand.Next(2) == 0)
                {
                    flag2 = true;
                }
                if (forceX)
                {
                    flag2 = true;
                }
                if (flag2)
                {
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector4.Y = 0f;
                    vector4.X = -num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
                else
                {
                    num5++;
                    vector2.Y = num6;
                    vector2.X = 0f;
                    vector3.X = 0f;
                    vector3.Y = num6;
                    vector4.X = 0f;
                    vector4.Y = -num6;
                    if (genRand.Next(2) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.X = 0.3f;
                        }
                        else
                        {
                            vector2.X = -0.3f;
                        }
                    }
                    else
                    {
                        num7 /= 2;
                    }
                }
                if (lastDungeonHall != vector4)
                {
                    flag = true;
                }
            }
            if (!forceX)
            {
                if (vector.X > (lastMaxTilesX - 200))
                {
                    num6 = -1;
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
                else if (vector.X < 200f)
                {
                    num6 = 1;
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
                else if (vector.Y > (lastMaxTilesY + 200))
                {
                    num6 = -1;
                    num5++;
                    vector2.Y = num6;
                    vector2.X = 0f;
                    vector3.X = 0f;
                    vector3.Y = num6;
                    if (genRand.Next(2) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.X = 0.3f;
                        }
                        else
                        {
                            vector2.X = -0.3f;
                        }
                    }
                }
                else if (vector.Y < world.getRockLayer())
                {
                    num6 = 1;
                    num5++;
                    vector2.Y = num6;
                    vector2.X = 0f;
                    vector3.X = 0f;
                    vector3.Y = num6;
                    if (genRand.Next(2) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.X = 0.3f;
                        }
                        else
                        {
                            vector2.X = -0.3f;
                        }
                    }
                }
                else if ((vector.X < (world.getMaxTilesX() / 2)) && (vector.X > (world.getMaxTilesX() * 0.25)))
                {
                    num6 = -1;
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
                else if ((vector.X > (world.getMaxTilesX() / 2)) && (vector.X < (world.getMaxTilesX() * 0.75)))
                {
                    num6 = 1;
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
            }
            if (vector3.Y == 0f)
            {
                DDoorX[numDDoors] = (int)vector.X;
                DDoorY[numDDoors] = (int)vector.Y;
                DDoorPos[numDDoors] = 0;
                numDDoors++;
            }
            else
            {
                DPlatX[numDPlats] = (int)vector.X;
                DPlatY[numDPlats] = (int)vector.Y;
                numDPlats++;
            }
            lastDungeonHall = vector3;
            while (num7 > 0)
            {
                if ((vector3.X > 0f) && (vector.X > (world.getMaxTilesX() - 100)))
                {
                    num7 = 0;
                }
                else if ((vector3.X < 0f) && (vector.X < 100f))
                {
                    num7 = 0;
                }
                else if ((vector3.Y > 0f) && (vector.Y > (world.getMaxTilesY() - 100)))
                {
                    num7 = 0;
                }
                else if ((vector3.Y < 0f) && (vector.Y < (world.getRockLayer() + 50.0)))
                {
                    num7 = 0;
                }
                num7--;
                int num = ((int)((vector.X - num5) - 4.0)) - genRand.Next(6);
                int maxTilesX = ((int)((vector.X + num5) + 4.0)) + genRand.Next(6);
                int num2 = ((int)((vector.Y - num5) - 4.0)) - genRand.Next(6);
                int maxTilesY = ((int)((vector.Y + num5) + 4.0)) + genRand.Next(6);
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int num9 = num2; num9 < maxTilesY; num9++)
                    {
                        world.getTile()[k, num9].liquid = 0;
                        if (world.getTile()[k, num9].wall == 0)
                        {
                            world.getTile()[k, num9].active = true;
                            world.getTile()[k, num9].type = (byte)tileType;
                        }
                    }
                }
                for (int m = num + 1; m < (maxTilesX - 1); m++)
                {
                    for (int num11 = num2 + 1; num11 < (maxTilesY - 1); num11++)
                    {
                        PlaceWall(m, num11, wallType, world, true);
                    }
                }
                int num12 = 0;
                if ((vector2.Y == 0f) && (genRand.Next(((int)num5) + 1) == 0))
                {
                    num12 = genRand.Next(1, 3);
                }
                else if ((vector2.X == 0f) && (genRand.Next(((int)num5) - 1) == 0))
                {
                    num12 = genRand.Next(1, 3);
                }
                else if (genRand.Next(((int)num5) * 3) == 0)
                {
                    num12 = genRand.Next(1, 3);
                }
                num = ((int)(vector.X - (num5 * 0.5))) - num12;
                maxTilesX = ((int)(vector.X + (num5 * 0.5))) + num12;
                num2 = ((int)(vector.Y - (num5 * 0.5))) - num12;
                maxTilesY = ((int)(vector.Y + (num5 * 0.5))) + num12;
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                for (int n = num; n < maxTilesX; n++)
                {
                    for (int num14 = num2; num14 < maxTilesY; num14++)
                    {
                        world.getTile()[n, num14].active = false;
                        world.getTile()[n, num14].wall = (byte)wallType;
                    }
                }
                vector += vector2;
            }
            dungeonX = (int)vector.X;
            dungeonY = (int)vector.Y;
            if (vector3.Y == 0f)
            {
                DDoorX[numDDoors] = (int)vector.X;
                DDoorY[numDDoors] = (int)vector.Y;
                DDoorPos[numDDoors] = 0;
                numDDoors++;
            }
            else
            {
                DPlatX[numDPlats] = (int)vector.X;
                DPlatY[numDPlats] = (int)vector.Y;
                numDPlats++;
            }
        }

        public static void DungeonRoom(int i, int j, int tileType, int wallType, World world)
        {
            Vector2 vector = new Vector2();
            Vector2 vector2 = new Vector2();
            double num5 = genRand.Next(15, 30);
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = genRand.Next(-10, 11) * 0.1f;
            vector.X = i;
            vector.Y = j - (((float)num5) / 2f);
            int num6 = genRand.Next(10, 20);
            double x = vector.X;
            double num8 = vector.X;
            double y = vector.Y;
            double num10 = vector.Y;
            while (num6 > 0)
            {
                num6--;
                int num = (int)((vector.X - (num5 * 0.800000011920929)) - 5.0);
                int maxTilesX = (int)((vector.X + (num5 * 0.800000011920929)) + 5.0);
                int num2 = (int)((vector.Y - (num5 * 0.800000011920929)) - 5.0);
                int maxTilesY = (int)((vector.Y + (num5 * 0.800000011920929)) + 5.0);
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int num12 = num2; num12 < maxTilesY; num12++)
                    {
                        world.getTile()[k, num12].liquid = 0;
                        if (world.getTile()[k, num12].wall == 0)
                        {
                            world.getTile()[k, num12].active = true;
                            world.getTile()[k, num12].type = (byte)tileType;
                        }
                    }
                }
                for (int m = num + 1; m < (maxTilesX - 1); m++)
                {
                    for (int num14 = num2 + 1; num14 < (maxTilesY - 1); num14++)
                    {
                        PlaceWall(m, num14, wallType, world, true);
                    }
                }
                num = (int)(vector.X - (num5 * 0.5));
                maxTilesX = (int)(vector.X + (num5 * 0.5));
                num2 = (int)(vector.Y - (num5 * 0.5));
                maxTilesY = (int)(vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                if (num < x)
                {
                    x = num;
                }
                if (maxTilesX > num8)
                {
                    num8 = maxTilesX;
                }
                if (num2 < y)
                {
                    y = num2;
                }
                if (maxTilesY > num10)
                {
                    num10 = maxTilesY;
                }
                for (int n = num; n < maxTilesX; n++)
                {
                    for (int num16 = num2; num16 < maxTilesY; num16++)
                    {
                        world.getTile()[n, num16].active = false;
                        world.getTile()[n, num16].wall = (byte)wallType;
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (vector2.Y > 1f)
                {
                    vector2.Y = 1f;
                }
                if (vector2.Y < -1f)
                {
                    vector2.Y = -1f;
                }
            }
            dRoomX[numDRooms] = (int)vector.X;
            dRoomY[numDRooms] = (int)vector.Y;
            dRoomSize[numDRooms] = (int)num5;
            dRoomL[numDRooms] = (int)x;
            dRoomR[numDRooms] = (int)num8;
            dRoomT[numDRooms] = (int)y;
            dRoomB[numDRooms] = (int)num10;
            dRoomTreasure[numDRooms] = false;
            numDRooms++;
        }

        public static void DungeonStairs(int i, int j, int tileType, int wallType, World world)
        {
            Vector2 vector = new Vector2();
            Vector2 vector2 = new Vector2();
            double num5 = genRand.Next(5, 9);
            int num6 = 1;
            vector.X = i;
            vector.Y = j;
            int num7 = genRand.Next(10, 30);
            if (i > dEnteranceX)
            {
                num6 = -1;
            }
            else
            {
                num6 = 1;
            }
            vector2.Y = -1f;
            vector2.X = num6;
            if (genRand.Next(3) == 0)
            {
                vector2.X *= 0.5f;
            }
            else if (genRand.Next(3) == 0)
            {
                vector2.Y *= 2f;
            }
            while (num7 > 0)
            {
                num7--;
                int num = ((int)((vector.X - num5) - 4.0)) - genRand.Next(6);
                int maxTilesX = ((int)((vector.X + num5) + 4.0)) + genRand.Next(6);
                int num2 = (int)((vector.Y - num5) - 4.0);
                int maxTilesY = ((int)((vector.Y + num5) + 4.0)) + genRand.Next(6);
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                int num8 = 1;
                if (vector.X > (world.getMaxTilesX() / 2))
                {
                    num8 = -1;
                }
                int num9 = (int)((vector.X + ((((float)dxStrength1) * 0.6f) * num8)) + (((float)dxStrength2) * num8));
                int num10 = (int)(dyStrength2 * 0.5);
                if (((vector.Y < (world.getWorldSurface() - 5.0)) && (world.getTile()[num9, ((int)((vector.Y - num5) - 6.0)) + num10].wall == 0)) && ((world.getTile()[num9, ((int)((vector.Y - num5) - 7.0)) + num10].wall == 0) && (world.getTile()[num9, ((int)((vector.Y - num5) - 8.0)) + num10].wall == 0)))
                {
                    dSurface = true;
                    TileRunner(num9, ((int)((vector.Y - num5) - 6.0)) + num10, world, (double)genRand.Next(0x19, 0x23), genRand.Next(10, 20), -1, false, 0f, -1f, false, true);
                }
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int num12 = num2; num12 < maxTilesY; num12++)
                    {
                        world.getTile()[k, num12].liquid = 0;
                        if (world.getTile()[k, num12].wall != wallType)
                        {
                            world.getTile()[k, num12].wall = 0;
                            world.getTile()[k, num12].active = true;
                            world.getTile()[k, num12].type = (byte)tileType;
                        }
                    }
                }
                for (int m = num + 1; m < (maxTilesX - 1); m++)
                {
                    for (int num14 = num2 + 1; num14 < (maxTilesY - 1); num14++)
                    {
                        PlaceWall(m, num14, wallType, world, true);
                    }
                }
                int num15 = 0;
                if (genRand.Next((int)num5) == 0)
                {
                    num15 = genRand.Next(1, 3);
                }
                num = ((int)(vector.X - (num5 * 0.5))) - num15;
                maxTilesX = ((int)(vector.X + (num5 * 0.5))) + num15;
                num2 = ((int)(vector.Y - (num5 * 0.5))) - num15;
                maxTilesY = ((int)(vector.Y + (num5 * 0.5))) + num15;
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                for (int n = num; n < maxTilesX; n++)
                {
                    for (int num17 = num2; num17 < maxTilesY; num17++)
                    {
                        world.getTile()[n, num17].active = false;
                        PlaceWall(n, num17, wallType, world, true);
                    }
                }
                if (dSurface)
                {
                    num7 = 0;
                }
                vector += vector2;
            }
            dungeonX = (int)vector.X;
            dungeonY = (int)vector.Y;
        }

        public static void MakeDungeon(int x, int y, World world, int tileType = 0x29, int wallType = 7)
        {
            int num = genRand.Next(3);
            int num2 = genRand.Next(3);
            switch (num)
            {
                case 1:
                    tileType = 0x2b;
                    break;

                case 2:
                    tileType = 0x2c;
                    break;
            }
            switch (num2)
            {
                case 1:
                    wallType = 8;
                    break;

                case 2:
                    wallType = 9;
                    break;
            }
            numDDoors = 0;
            numDPlats = 0;
            numDRooms = 0;
            WorldGen.dungeonX = x;
            WorldGen.dungeonY = y;
            dMinX = x;
            dMaxX = x;
            dMinY = y;
            dMaxY = y;
            dxStrength1 = genRand.Next(0x19, 30);
            dyStrength1 = genRand.Next(20, 0x19);
            dxStrength2 = genRand.Next(0x23, 50);
            dyStrength2 = genRand.Next(10, 15);
            float num3 = world.getMaxTilesX() / 60;
            num3 += genRand.Next(0, (int)(num3 / 3f));
            float num4 = num3;
            int num5 = 5;
            DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world);
            while (num3 > 0f)
            {
                if (WorldGen.dungeonX < dMinX)
                {
                    dMinX = WorldGen.dungeonX;
                }
                if (WorldGen.dungeonX > dMaxX)
                {
                    dMaxX = WorldGen.dungeonX;
                }
                if (WorldGen.dungeonY > dMaxY)
                {
                    dMaxY = WorldGen.dungeonY;
                }
                num3--;
                Program.printData("Creating dungeon: " + ((int)(((num4 - num3) / num4) * 60f)) + "%");
                if (num5 > 0)
                {
                    num5--;
                }
                if ((num5 == 0) & (genRand.Next(3) == 0))
                {
                    num5 = 5;
                    if (genRand.Next(2) == 0)
                    {
                        int dungeonX = WorldGen.dungeonX;
                        int dungeonY = WorldGen.dungeonY;
                        DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world, false);
                        if (genRand.Next(2) == 0)
                        {
                            DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world, false);
                        }
                        DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world);
                        WorldGen.dungeonX = dungeonX;
                        WorldGen.dungeonY = dungeonY;
                    }
                    else
                    {
                        DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world);
                    }
                }
                else
                {
                    DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world, false);
                }
            }
            DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world);
            int num8 = dRoomX[0];
            int num9 = dRoomY[0];
            for (int i = 0; i < numDRooms; i++)
            {
                if (dRoomY[i] < num9)
                {
                    num8 = dRoomX[i];
                    num9 = dRoomY[i];
                }
            }
            WorldGen.dungeonX = num8;
            WorldGen.dungeonY = num9;
            dEnteranceX = num8;
            dSurface = false;
            num5 = 5;
            while (!dSurface)
            {
                if (num5 > 0)
                {
                    num5--;
                }
                if (((num5 == 0) & (genRand.Next(5) == 0)) && (WorldGen.dungeonY > (world.getWorldSurface() + 50.0)))
                {
                    num5 = 10;
                    int num11 = WorldGen.dungeonX;
                    int num12 = WorldGen.dungeonY;
                    DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world, true);
                    DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world);
                    WorldGen.dungeonX = num11;
                    WorldGen.dungeonY = num12;
                }
                DungeonStairs(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world);
            }
            DungeonEnt(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, world);
            Program.printData("Creating dungeon: 65%");
            for (int j = 0; j < numDRooms; j++)
            {
                for (int num14 = dRoomL[j]; num14 <= dRoomR[j]; num14++)
                {
                    if (!world.getTile()[num14, dRoomT[j] - 1].active)
                    {
                        DPlatX[numDPlats] = num14;
                        DPlatY[numDPlats] = dRoomT[j] - 1;
                        numDPlats++;
                        break;
                    }
                }
                for (int num15 = dRoomL[j]; num15 <= dRoomR[j]; num15++)
                {
                    if (!world.getTile()[num15, dRoomB[j] + 1].active)
                    {
                        DPlatX[numDPlats] = num15;
                        DPlatY[numDPlats] = dRoomB[j] + 1;
                        numDPlats++;
                        break;
                    }
                }
                for (int num16 = dRoomT[j]; num16 <= dRoomB[j]; num16++)
                {
                    if (!world.getTile()[dRoomL[j] - 1, num16].active)
                    {
                        DDoorX[numDDoors] = dRoomL[j] - 1;
                        DDoorY[numDDoors] = num16;
                        DDoorPos[numDDoors] = -1;
                        numDDoors++;
                        break;
                    }
                }
                for (int num17 = dRoomT[j]; num17 <= dRoomB[j]; num17++)
                {
                    if (!world.getTile()[dRoomR[j] + 1, num17].active)
                    {
                        DDoorX[numDDoors] = dRoomR[j] + 1;
                        DDoorY[numDDoors] = num17;
                        DDoorPos[numDDoors] = 1;
                        numDDoors++;
                        break;
                    }
                }
            }
            Program.printData("Creating dungeon: 70%");
            int num18 = 0;
            int num19 = 0x3e8;
            int num20 = 0;
            while (num20 < (world.getMaxTilesX() / 0x7d))
            {
                num18++;
                int num21 = genRand.Next(dMinX, dMaxX);
                int num22 = genRand.Next(((int)world.getWorldSurface()) + 0x19, dMaxY);
                int num23 = num21;
                if ((world.getTile()[num21, num22].wall == wallType) && !world.getTile()[num21, num22].active)
                {
                    int num24 = 1;
                    if (genRand.Next(2) == 0)
                    {
                        num24 = -1;
                    }
                    while (!world.getTile()[num21, num22].active)
                    {
                        num22 += num24;
                    }
                    if ((world.getTile()[num21 - 1, num22].active && world.getTile()[num21 + 1, num22].active) && (!world.getTile()[num21 - 1, num22 - num24].active && !world.getTile()[num21 + 1, num22 - num24].active))
                    {
                        num20++;
                        int num25 = genRand.Next(5, 10);
                        while ((world.getTile()[num21 - 1, num22].active && world.getTile()[num21, num22 + num24].active) && ((world.getTile()[num21, num22].active && !world.getTile()[num21, num22 - num24].active) && (num25 > 0)))
                        {
                            world.getTile()[num21, num22].type = 0x30;
                            if (!world.getTile()[num21 - 1, num22 - num24].active && !world.getTile()[num21 + 1, num22 - num24].active)
                            {
                                world.getTile()[num21, num22 - num24].type = 0x30;
                                world.getTile()[num21, num22 - num24].active = true;
                            }
                            num21--;
                            num25--;
                        }
                        num25 = genRand.Next(5, 10);
                        num21 = num23 + 1;
                        while ((world.getTile()[num21 + 1, num22].active && world.getTile()[num21, num22 + num24].active) && ((world.getTile()[num21, num22].active && !world.getTile()[num21, num22 - num24].active) && (num25 > 0)))
                        {
                            world.getTile()[num21, num22].type = 0x30;
                            if (!world.getTile()[num21 - 1, num22 - num24].active && !world.getTile()[num21 + 1, num22 - num24].active)
                            {
                                world.getTile()[num21, num22 - num24].type = 0x30;
                                world.getTile()[num21, num22 - num24].active = true;
                            }
                            num21++;
                            num25--;
                        }
                    }
                }
                if (num18 > num19)
                {
                    num18 = 0;
                    num20++;
                }
            }
            num18 = 0;
            num19 = 0x3e8;
            num20 = 0;
            Program.printData("Creating dungeon: 75%");
            while (num20 < (world.getMaxTilesX() / 0x7d))
            {
                num18++;
                int num26 = genRand.Next(dMinX, dMaxX);
                int num27 = genRand.Next(((int)world.getWorldSurface()) + 0x19, dMaxY);
                int num28 = num27;
                if ((world.getTile()[num26, num27].wall == wallType) && !world.getTile()[num26, num27].active)
                {
                    int num29 = 1;
                    if (genRand.Next(2) == 0)
                    {
                        num29 = -1;
                    }
                    while (((num26 > 5) && (num26 < (world.getMaxTilesX() - 5))) && !world.getTile()[num26, num27].active)
                    {
                        num26 += num29;
                    }
                    if ((world.getTile()[num26, num27 - 1].active && world.getTile()[num26, num27 + 1].active) && (!world.getTile()[num26 - num29, num27 - 1].active && !world.getTile()[num26 - num29, num27 + 1].active))
                    {
                        num20++;
                        int num30 = genRand.Next(5, 10);
                        while ((world.getTile()[num26, num27 - 1].active && world.getTile()[num26 + num29, num27].active) && ((world.getTile()[num26, num27].active && !world.getTile()[num26 - num29, num27].active) && (num30 > 0)))
                        {
                            world.getTile()[num26, num27].type = 0x30;
                            if (!world.getTile()[num26 - num29, num27 - 1].active && !world.getTile()[num26 - num29, num27 + 1].active)
                            {
                                world.getTile()[num26 - num29, num27].type = 0x30;
                                world.getTile()[num26 - num29, num27].active = true;
                            }
                            num27--;
                            num30--;
                        }
                        num30 = genRand.Next(5, 10);
                        num27 = num28 + 1;
                        while ((world.getTile()[num26, num27 + 1].active && world.getTile()[num26 + num29, num27].active) && ((world.getTile()[num26, num27].active && !world.getTile()[num26 - num29, num27].active) && (num30 > 0)))
                        {
                            world.getTile()[num26, num27].type = 0x30;
                            if (!world.getTile()[num26 - num29, num27 - 1].active && !world.getTile()[num26 - num29, num27 + 1].active)
                            {
                                world.getTile()[num26 - num29, num27].type = 0x30;
                                world.getTile()[num26 - num29, num27].active = true;
                            }
                            num27++;
                            num30--;
                        }
                    }
                }
                if (num18 > num19)
                {
                    num18 = 0;
                    num20++;
                }
            }
            Program.printData("Creating dungeon: 80%");
            for (int k = 0; k < numDDoors; k++)
            {
                int num32 = DDoorX[k] - 10;
                int num33 = DDoorX[k] + 10;
                int num34 = 100;
                int num35 = 0;
                int num36 = 0;
                int num37 = 0;
                for (int num38 = num32; num38 < num33; num38++)
                {
                    bool flag = true;
                    int num39 = DDoorY[k];
                    while (!world.getTile()[num38, num39].active)
                    {
                        num39--;
                    }
                    if (!Statics.tileDungeon[world.getTile()[num38, num39].type])
                    {
                        flag = false;
                    }
                    num36 = num39;
                    num39 = DDoorY[k];
                    while (!world.getTile()[num38, num39].active)
                    {
                        num39++;
                    }
                    if (!Statics.tileDungeon[world.getTile()[num38, num39].type])
                    {
                        flag = false;
                    }
                    num37 = num39;
                    if ((num37 - num36) >= 3)
                    {
                        int num40 = num38 - 20;
                        int num41 = num38 + 20;
                        int num42 = num37 - 10;
                        int num43 = num37 + 10;
                        for (int num44 = num40; num44 < num41; num44++)
                        {
                            for (int num45 = num42; num45 < num43; num45++)
                            {
                                if (world.getTile()[num44, num45].active && (world.getTile()[num44, num45].type == 10))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (flag)
                        {
                            for (int num46 = num37 - 3; num46 < num37; num46++)
                            {
                                for (int num47 = num38 - 3; num47 <= (num38 + 3); num47++)
                                {
                                    if (world.getTile()[num47, num46].active)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (flag && ((num37 - num36) < 20))
                        {
                            bool flag2 = false;
                            if ((DDoorPos[k] == 0) && ((num37 - num36) < num34))
                            {
                                flag2 = true;
                            }
                            if ((DDoorPos[k] == -1) && (num38 > num35))
                            {
                                flag2 = true;
                            }
                            if ((DDoorPos[k] == 1) && ((num38 < num35) || (num35 == 0)))
                            {
                                flag2 = true;
                            }
                            if (flag2)
                            {
                                num35 = num38;
                                num34 = num37 - num36;
                            }
                        }
                    }
                }
                if (num34 < 20)
                {
                    int num48 = num35;
                    int num49 = DDoorY[k];
                    int num50 = num49;
                    while (!world.getTile()[num48, num49].active)
                    {
                        world.getTile()[num48, num49].active = false;
                        num49++;
                    }
                    while (!world.getTile()[num48, num50].active)
                    {
                        num50--;
                    }
                    num49--;
                    num50++;
                    for (int num51 = num50; num51 < (num49 - 2); num51++)
                    {
                        world.getTile()[num48, num51].active = true;
                        world.getTile()[num48, num51].type = (byte)tileType;
                    }
                    PlaceTile(num48, num49, world, 10, true, false, -1);
                    num48--;
                    int num52 = num49 - 3;
                    while (!world.getTile()[num48, num52].active)
                    {
                        num52--;
                    }
                    if (((num49 - num52) < ((num49 - num50) + 5)) && Statics.tileDungeon[world.getTile()[num48, num52].type])
                    {
                        for (int num53 = (num49 - 4) - genRand.Next(3); num53 > num52; num53--)
                        {
                            world.getTile()[num48, num53].active = true;
                            world.getTile()[num48, num53].type = (byte)tileType;
                        }
                    }
                    num48 += 2;
                    num52 = num49 - 3;
                    while (!world.getTile()[num48, num52].active)
                    {
                        num52--;
                    }
                    if (((num49 - num52) < ((num49 - num50) + 5)) && Statics.tileDungeon[world.getTile()[num48, num52].type])
                    {
                        for (int num54 = (num49 - 4) - genRand.Next(3); num54 > num52; num54--)
                        {
                            world.getTile()[num48, num54].active = true;
                            world.getTile()[num48, num54].type = (byte)tileType;
                        }
                    }
                    num49++;
                    num48--;
                    world.getTile()[num48 - 1, num49].active = true;
                    world.getTile()[num48 - 1, num49].type = (byte)tileType;
                    world.getTile()[num48 + 1, num49].active = true;
                    world.getTile()[num48 + 1, num49].type = (byte)tileType;
                }
            }
            Program.printData("Creating dungeon: 85%");
            for (int m = 0; m < numDPlats; m++)
            {
                int num56 = DPlatX[m];
                int num57 = DPlatY[m];
                int maxTilesX = world.getMaxTilesX();
                int num59 = 10;
                for (int num60 = num57 - 5; num60 <= (num57 + 5); num60++)
                {
                    int num61 = num56;
                    int num62 = num56;
                    bool flag3 = false;
                    if (!world.getTile()[num61, num60].active)
                    {
                        goto Label_10D8;
                    }
                    flag3 = true;
                    goto Label_1128;
                Label_10B4:
                    num61--;
                    if (!Statics.tileDungeon[world.getTile()[num61, num60].type])
                    {
                        flag3 = true;
                    }
                Label_10D8:
                    if (!world.getTile()[num61, num60].active)
                    {
                        goto Label_10B4;
                    }
                    while (!world.getTile()[num62, num60].active)
                    {
                        num62++;
                        if (!Statics.tileDungeon[world.getTile()[num62, num60].type])
                        {
                            flag3 = true;
                        }
                    }
                Label_1128:
                    if (!flag3 && ((num62 - num61) <= num59))
                    {
                        bool flag4 = true;
                        int num63 = (num56 - (num59 / 2)) - 2;
                        int num64 = (num56 + (num59 / 2)) + 2;
                        int num65 = num60 - 5;
                        int num66 = num60 + 5;
                        for (int num67 = num63; num67 <= num64; num67++)
                        {
                            for (int num68 = num65; num68 <= num66; num68++)
                            {
                                if (world.getTile()[num67, num68].active && (world.getTile()[num67, num68].type == 0x13))
                                {
                                    flag4 = false;
                                    break;
                                }
                            }
                        }
                        for (int num69 = num60 + 3; num69 >= (num60 - 5); num69--)
                        {
                            if (world.getTile()[num56, num69].active)
                            {
                                flag4 = false;
                                break;
                            }
                        }
                        if (flag4)
                        {
                            maxTilesX = num60;
                            break;
                        }
                    }
                }
                if ((maxTilesX > (num57 - 10)) && (maxTilesX < (num57 + 10)))
                {
                    int num70 = num56;
                    int num71 = maxTilesX;
                    int num72 = num56 + 1;
                    while (!world.getTile()[num70, num71].active)
                    {
                        world.getTile()[num70, num71].active = true;
                        world.getTile()[num70, num71].type = 0x13;
                        num70--;
                    }
                    while (!world.getTile()[num72, num71].active)
                    {
                        world.getTile()[num72, num71].active = true;
                        world.getTile()[num72, num71].type = 0x13;
                        num72++;
                    }
                }
            }
            Program.printData("Creating dungeon: 90%");
            num18 = 0;
            num19 = 0x3e8;
            num20 = 0;
            while (num20 < (world.getMaxTilesX() / 20))
            {
                num18++;
                int num73 = genRand.Next(dMinX, dMaxX);
                int num74 = genRand.Next(dMinY, dMaxY);
                bool flag5 = true;
                if ((world.getTile()[num73, num74].wall == wallType) && !world.getTile()[num73, num74].active)
                {
                    int num75 = 1;
                    if (genRand.Next(2) == 0)
                    {
                        num75 = -1;
                    }
                    while (flag5 && !world.getTile()[num73, num74].active)
                    {
                        num73 -= num75;
                        if ((num73 < 5) || (num73 > (world.getMaxTilesX() - 5)))
                        {
                            flag5 = false;
                        }
                        else if (world.getTile()[num73, num74].active && !Statics.tileDungeon[world.getTile()[num73, num74].type])
                        {
                            flag5 = false;
                        }
                    }
                    if (((flag5 && world.getTile()[num73, num74].active) && (Statics.tileDungeon[world.getTile()[num73, num74].type] && world.getTile()[num73, num74 - 1].active)) && ((Statics.tileDungeon[world.getTile()[num73, num74 - 1].type] && world.getTile()[num73, num74 + 1].active) && Statics.tileDungeon[world.getTile()[num73, num74 + 1].type]))
                    {
                        num73 += num75;
                        for (int num76 = num73 - 3; num76 <= (num73 + 3); num76++)
                        {
                            for (int num77 = num74 - 3; num77 <= (num74 + 3); num77++)
                            {
                                if (world.getTile()[num76, num77].active && (world.getTile()[num76, num77].type == 0x13))
                                {
                                    flag5 = false;
                                    break;
                                }
                            }
                        }
                        if (flag5 && ((!world.getTile()[num73, num74 - 1].active & !world.getTile()[num73, num74 - 2].active) & !world.getTile()[num73, num74 - 3].active))
                        {
                            int num78 = num73;
                            int num79 = num73;
                            while (((num78 > dMinX) && (num78 < dMaxX)) && ((!world.getTile()[num78, num74].active && !world.getTile()[num78, num74 - 1].active) && !world.getTile()[num78, num74 + 1].active))
                            {
                                num78 += num75;
                            }
                            num78 = Math.Abs((int)(num73 - num78));
                            bool flag6 = false;
                            if (genRand.Next(2) == 0)
                            {
                                flag6 = true;
                            }
                            if (num78 > 5)
                            {
                                for (int num80 = genRand.Next(1, 4); num80 > 0; num80--)
                                {
                                    world.getTile()[num73, num74].active = true;
                                    world.getTile()[num73, num74].type = 0x13;
                                    if (flag6)
                                    {
                                        PlaceTile(num73, num74 - 1, world, 50, true, false, -1);
                                        if ((genRand.Next(50) == 0) && (world.getTile()[num73, num74 - 1].type == 50))
                                        {
                                            world.getTile()[num73, num74 - 1].frameX = 90;
                                        }
                                    }
                                    num73 += num75;
                                }
                                num18 = 0;
                                num20++;
                                if (!flag6 && (genRand.Next(2) == 0))
                                {
                                    num73 = num79;
                                    num74--;
                                    int type = genRand.Next(2);
                                    switch (type)
                                    {
                                        case 0:
                                            type = 13;
                                            break;

                                        case 1:
                                            type = 0x31;
                                            break;
                                    }
                                    PlaceTile(num73, num74, world, type, true, false, -1);
                                    if (world.getTile()[num73, num74].type == 13)
                                    {
                                        if (genRand.Next(2) == 0)
                                        {
                                            world.getTile()[num73, num74].frameX = 0x12;
                                        }
                                        else
                                        {
                                            world.getTile()[num73, num74].frameX = 0x24;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (num18 > num19)
                {
                    num18 = 0;
                    num20++;
                }
            }
            Program.printData("Creating dungeon: 95%");
            for (int n = 0; n < numDRooms; n++)
            {
                int num83 = 0;
                while (num83 < 0x3e8)
                {
                    int num84 = (int)(dRoomSize[n] * 0.4);
                    int num85 = dRoomX[n] + genRand.Next(-num84, num84 + 1);
                    int num86 = dRoomY[n] + genRand.Next(-num84, num84 + 1);
                    int contain = 0;
                    switch (n)
                    {
                        case 0:
                            contain = 0x71;
                            break;

                        case 1:
                            contain = 0x9b;
                            break;

                        case 2:
                            contain = 0x9c;
                            break;

                        case 3:
                            contain = 0x9d;
                            break;

                        case 4:
                            contain = 0xa3;
                            break;

                        case 5:
                            contain = 0xa4;
                            break;
                    }
                    if ((contain == 0) && (genRand.Next(2) == 0))
                    {
                        num83 = 0x3e8;
                    }
                    else
                    {
                        if (AddBuriedChest(num85, num86, world, contain))
                        {
                            num83 += 0x3e8;
                        }
                        num83++;
                    }
                }
            }
            dMinX -= 0x19;
            dMaxX += 0x19;
            dMinY -= 0x19;
            dMaxY += 0x19;
            if (dMinX < 0)
            {
                dMinX = 0;
            }
            if (dMaxX > world.getMaxTilesX())
            {
                dMaxX = world.getMaxTilesX();
            }
            if (dMinY < 0)
            {
                dMinY = 0;
            }
            if (dMaxY > world.getMaxTilesY())
            {
                dMaxY = world.getMaxTilesY();
            }
            num18 = 0;
            num19 = 0x3e8;
            num20 = 0;
            while (num20 < (world.getMaxTilesX() / 20))
            {
                num18++;
                int num89 = genRand.Next(dMinX, dMaxX);
                int num90 = genRand.Next(dMinY, dMaxY);
                if (world.getTile()[num89, num90].wall == wallType)
                {
                    for (int num91 = num90; num91 > dMinY; num91--)
                    {
                        if (world.getTile()[num89, num91 - 1].active && (world.getTile()[num89, num91 - 1].type == tileType))
                        {
                            bool flag7 = false;
                            for (int num92 = num89 - 15; num92 < (num89 + 15); num92++)
                            {
                                for (int num93 = num91 - 15; num93 < (num91 + 15); num93++)
                                {
                                    if ((((num92 > 0) && (num92 < world.getMaxTilesX())) && ((num93 > 0) && (num93 < world.getMaxTilesY()))) && (world.getTile()[num92, num93].type == 0x2a))
                                    {
                                        flag7 = true;
                                        break;
                                    }
                                }
                            }
                            if ((world.getTile()[num89 - 1, num91].active || world.getTile()[num89 + 1, num91].active) || ((world.getTile()[num89 - 1, num91 + 1].active || world.getTile()[num89 + 1, num91 + 1].active) || world.getTile()[num89, num91 + 2].active))
                            {
                                flag7 = true;
                            }
                            if (!flag7)
                            {
                                Place1x2Top(num89, num91, 0x2a, world);
                                if (world.getTile()[num89, num91].type == 0x2a)
                                {
                                    num18 = 0;
                                    num20++;
                                }
                            }
                            break;
                        }
                    }
                }
                if (num18 > num19)
                {
                    num20++;
                    num18 = 0;
                }
            }
        }

        public static bool AddBuriedChest(int i, int j, World world, int contain = 0)
        {
            if (genRand == null)
            {
                genRand = new Random((int)DateTime.Now.Ticks);
            }
            for (int k = j; k < world.getMaxTilesY(); k++)
            {
                if (world.getTile()[i, k].active && Statics.tileSolid[world.getTile()[i, k].type])
                {
                    int num2 = i;
                    int num3 = k;
                    int index = PlaceChest(num2 - 1, num3 - 1, world, 0x15);
                    if (index < 0)
                    {
                        return false;
                    }
                    int num5 = 0;
                    while (num5 == 0)
                    {
                        if (contain > 0)
                        {
                            world.getChests()[index].item[num5].SetDefaults(contain);
                            num5++;
                        }
                        else
                        {
                            switch (genRand.Next(7))
                            {
                                case 0:
                                    world.getChests()[index].item[num5].SetDefaults(0x31);
                                    break;

                                case 1:
                                    world.getChests()[index].item[num5].SetDefaults(50);
                                    break;

                                case 2:
                                    world.getChests()[index].item[num5].SetDefaults(0x34);
                                    break;

                                case 3:
                                    world.getChests()[index].item[num5].SetDefaults(0x35);
                                    break;

                                case 4:
                                    world.getChests()[index].item[num5].SetDefaults(0x36);
                                    break;

                                case 5:
                                    world.getChests()[index].item[num5].SetDefaults(0x37);
                                    break;

                                case 6:
                                    world.getChests()[index].item[num5].SetDefaults(0x33);
                                    world.getChests()[index].item[num5].stack = genRand.Next(0x1a) + 0x19;
                                    break;
                            }
                            num5++;
                        }
                        if (genRand.Next(3) == 0)
                        {
                            world.getChests()[index].item[num5].SetDefaults(0xa7);
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            int num7 = genRand.Next(4);
                            int num8 = genRand.Next(8) + 3;
                            switch (num7)
                            {
                                case 0:
                                    world.getChests()[index].item[num5].SetDefaults(0x13);
                                    break;

                                case 1:
                                    world.getChests()[index].item[num5].SetDefaults(20);
                                    break;

                                case 2:
                                    world.getChests()[index].item[num5].SetDefaults(0x15);
                                    break;

                                case 3:
                                    world.getChests()[index].item[num5].SetDefaults(0x16);
                                    break;
                            }
                            world.getChests()[index].item[num5].stack = num8;
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            int num9 = genRand.Next(2);
                            int num10 = genRand.Next(0x1a) + 0x19;
                            switch (num9)
                            {
                                case 0:
                                    world.getChests()[index].item[num5].SetDefaults(40);
                                    break;

                                case 1:
                                    world.getChests()[index].item[num5].SetDefaults(0x2a);
                                    break;
                            }
                            world.getChests()[index].item[num5].stack = num10;
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            int num11 = genRand.Next(1);
                            int num12 = genRand.Next(3) + 3;
                            if (num11 == 0)
                            {
                                world.getChests()[index].item[num5].SetDefaults(0x1c);
                            }
                            world.getChests()[index].item[num5].stack = num12;
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            int num13 = genRand.Next(2);
                            int num14 = genRand.Next(11) + 10;
                            switch (num13)
                            {
                                case 0:
                                    world.getChests()[index].item[num5].SetDefaults(8);
                                    break;

                                case 1:
                                    world.getChests()[index].item[num5].SetDefaults(0x1f);
                                    break;
                            }
                            world.getChests()[index].item[num5].stack = num14;
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            world.getChests()[index].item[num5].SetDefaults(0x49);
                            world.getChests()[index].item[num5].stack = genRand.Next(1, 3);
                            num5++;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
                
        public static void ShroomPatch(int i, int j, World world)
        {
            Vector2 vector = new Vector2();
            Vector2 vector2 = new Vector2();
            double num5 = genRand.Next(40, 70);
            double num6 = num5;
            float num7 = genRand.Next(10, 20);
            if (genRand.Next(5) == 0)
            {
                num5 *= 1.5;
                num6 *= 1.5;
                num7 *= 1.2f;
            }
            vector.X = i;
            vector.Y = j - (num7 * 0.3f);
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = genRand.Next(-20, -10) * 0.1f;
            while ((num5 > 0.0) && (num7 > 0f))
            {
                num5 -= genRand.Next(3);
                num7--;
                int num = (int)(vector.X - (num5 * 0.5));
                int maxTilesX = (int)(vector.X + (num5 * 0.5));
                int num2 = (int)(vector.Y - (num5 * 0.5));
                int maxTilesY = (int)(vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        float num10 = Math.Abs((float)(k - vector.X));
                        float num11 = Math.Abs((float)((m - vector.Y) * 2.3f));
                        if (Math.Sqrt((double)((num10 * num10) + (num11 * num11))) < (num6 * 0.4))
                        {
                            if (m < (vector.Y + (num6 * 0.05)))
                            {
                                if (world.getTile()[k, m].type != 0x3b)
                                {
                                    world.getTile()[k, m].active = false;
                                }
                            }
                            else
                            {
                                world.getTile()[k, m].type = 0x3b;
                            }
                            world.getTile()[k, m].liquid = 0;
                            world.getTile()[k, m].lava = false;
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 0.1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (vector2.Y > 1f)
                {
                    vector2.Y = 1f;
                }
                if (vector2.Y < -1f)
                {
                    vector2.Y = -1f;
                }
            }
        }

        public static void HellHouse(int i, int j, World world)
        {
            int width = genRand.Next(8, 20);
            int num2 = genRand.Next(3);
            int num3 = genRand.Next(7);
            int num4 = i;
            int num5 = j;
            for (int k = 0; k < num2; k++)
            {
                int height = genRand.Next(5, 9);
                HellRoom(num4, num5, width, height, world);
                num5 -= height;
            }
            num5 = j;
            for (int m = 0; m < num3; m++)
            {
                int num9 = genRand.Next(5, 9);
                num5 += num9;
                HellRoom(num4, num5, width, num9, world);
            }
            for (int n = i - (width / 2); n <= (i + (width / 2)); n++)
            {
                num5 = j;
                while ((num5 < world.getMaxTilesY()) && ((world.getTile()[n, num5].active && (world.getTile()[n, num5].type == 0x4c)) || (world.getTile()[n, num5].wall == 13)))
                {
                    num5++;
                }
                int num11 = 6 + genRand.Next(3);
                while ((num5 < world.getMaxTilesY()) && !world.getTile()[n, num5].active)
                {
                    num11--;
                    world.getTile()[n, num5].active = true;
                    world.getTile()[n, num5].type = 0x39;
                    num5++;
                    if (num11 <= 0)
                    {
                        break;
                    }
                }
            }
            int minValue = 0;
            int maxValue = 0;
            num5 = j;
            while ((num5 < world.getMaxTilesY()) && ((world.getTile()[i, num5].active && (world.getTile()[i, num5].type == 0x4c)) || (world.getTile()[i, num5].wall == 13)))
            {
                num5++;
            }
            num5--;
            maxValue = num5;
            while ((world.getTile()[i, num5].active && (world.getTile()[i, num5].type == 0x4c)) || (world.getTile()[i, num5].wall == 13))
            {
                num5--;
                if (world.getTile()[i, num5].active && (world.getTile()[i, num5].type == 0x4c))
                {
                    int num14 = genRand.Next((i - (width / 2)) + 1, (i + (width / 2)) - 1);
                    int num15 = genRand.Next((i - (width / 2)) + 1, (i + (width / 2)) - 1);
                    if (num14 > num15)
                    {
                        int num16 = num14;
                        num14 = num15;
                        num15 = num16;
                    }
                    if (num14 == num15)
                    {
                        if (num14 < i)
                        {
                            num15++;
                        }
                        else
                        {
                            num14--;
                        }
                    }
                    for (int num17 = num14; num17 <= num15; num17++)
                    {
                        if (world.getTile()[num17, num5 - 1].wall == 13)
                        {
                            world.getTile()[num17, num5].wall = 13;
                        }
                        world.getTile()[num17, num5].type = 0x13;
                        world.getTile()[num17, num5].active = true;
                    }
                    num5--;
                }
            }
            minValue = num5;
            float num18 = (maxValue - minValue) * width;
            float num19 = num18 * 0.02f;
            for (int num20 = 0; num20 < num19; num20++)
            {
                int num21 = genRand.Next(i - (width / 2), (i + (width / 2)) + 1);
                int num22 = genRand.Next(minValue, maxValue);
                int num23 = genRand.Next(3, 8);
                for (int num24 = num21 - num23; num24 <= (num21 + num23); num24++)
                {
                    for (int num25 = num22 - num23; num25 <= (num22 + num23); num25++)
                    {
                        float num26 = Math.Abs((int)(num24 - num21));
                        float num27 = Math.Abs((int)(num25 - num22));
                        if (Math.Sqrt((double)((num26 * num26) + (num27 * num27))) < (num23 * 0.4))
                        {
                            if ((world.getTile()[num24, num25].type == 0x4c) || (world.getTile()[num24, num25].type == 0x13))
                            {
                                world.getTile()[num24, num25].active = false;
                            }
                            world.getTile()[num24, num25].wall = 0;
                        }
                    }
                }
            }
        }

        public static void HellRoom(int i, int j, int width, int height, World world)
        {
            for (int k = i - (width / 2); k <= (i + (width / 2)); k++)
            {
                for (int n = j - height; n <= j; n++)
                {
                    world.getTile()[k, n].active = true;
                    world.getTile()[k, n].type = 0x4c;
                    world.getTile()[k, n].liquid = 0;
                    world.getTile()[k, n].lava = false;
                }
            }
            for (int m = (i - (width / 2)) + 1; m <= ((i + (width / 2)) - 1); m++)
            {
                for (int num4 = (j - height) + 1; num4 <= (j - 1); num4++)
                {
                    world.getTile()[m, num4].active = false;
                    world.getTile()[m, num4].wall = 13;
                    world.getTile()[m, num4].liquid = 0;
                    world.getTile()[m, num4].lava = false;
                }
            }
        }

        public static void AddHellHouses(World world)
        {
            int num = (int)(world.getMaxTilesX() * 0.25);
            for (int i = num; i < (world.getMaxTilesX() - num); i++)
            {
                int j = world.getMaxTilesY() - 40;
                while (world.getTile()[i, j].active || (world.getTile()[i, j].liquid > 0))
                {
                    j--;
                }
                if (world.getTile()[i, j + 1].active)
                {
                    HellHouse(i, j, world);
                    i += genRand.Next(15, 80);
                }
            }
        }

        public static bool AddLifeCrystal(int i, int j, World world)
        {
            for (int k = j; k < world.getMaxTilesY(); k++)
            {
                if (world.getTile()[i, k].active && Statics.tileSolid[world.getTile()[i, k].type])
                {
                    int endX = i;
                    int endY = k - 1;
                    if (world.getTile()[endX, endY - 1].lava || world.getTile()[endX - 1, endY - 1].lava)
                    {
                        return false;
                    }
                    if (!EmptyTileCheck(endX - 1, endX, endY - 1, endY, world, -1))
                    {
                        return false;
                    }
                    world.getTile()[endX - 1, endY - 1].active = true;
                    world.getTile()[endX - 1, endY - 1].type = 12;
                    world.getTile()[endX - 1, endY - 1].frameX = 0;
                    world.getTile()[endX - 1, endY - 1].frameY = 0;
                    world.getTile()[endX, endY - 1].active = true;
                    world.getTile()[endX, endY - 1].type = 12;
                    world.getTile()[endX, endY - 1].frameX = 0x12;
                    world.getTile()[endX, endY - 1].frameY = 0;
                    world.getTile()[endX - 1, endY].active = true;
                    world.getTile()[endX - 1, endY].type = 12;
                    world.getTile()[endX - 1, endY].frameX = 0;
                    world.getTile()[endX - 1, endY].frameY = 0x12;
                    world.getTile()[endX, endY].active = true;
                    world.getTile()[endX, endY].type = 12;
                    world.getTile()[endX, endY].frameX = 0x12;
                    world.getTile()[endX, endY].frameY = 0x12;
                    return true;
                }
            }
            return false;
        }

        public static void Lakinater(int i, int j, World world)
        {
            Vector2 vector = new Vector2();
            Vector2 vector2 = new Vector2();
            double num5 = genRand.Next(0x19, 50);
            double num6 = num5;
            float num7 = genRand.Next(30, 80);
            if (genRand.Next(5) == 0)
            {
                num5 *= 1.5;
                num6 *= 1.5;
                num7 *= 1.2f;
            }
            vector.X = i;
            vector.Y = j - (num7 * 0.3f);
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = genRand.Next(-20, -10) * 0.1f;
            while ((num5 > 0.0) && (num7 > 0f))
            {
                if ((vector.Y + (num6 * 0.5)) > world.getWorldSurface())
                {
                    num7 = 0f;
                }
                num5 -= genRand.Next(3);
                num7--;
                int num = (int)(vector.X - (num5 * 0.5));
                int maxTilesX = (int)(vector.X + (num5 * 0.5));
                int num2 = (int)(vector.Y - (num5 * 0.5));
                int maxTilesY = (int)(vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        float num10 = Math.Abs((float)(k - vector.X));
                        float num11 = Math.Abs((float)(m - vector.Y));
                        if (Math.Sqrt((double)((num10 * num10) + (num11 * num11))) < (num6 * 0.4))
                        {
                            if (world.getTile()[k, m].active)
                            {
                                world.getTile()[k, m].liquid = 0xff;
                            }
                            world.getTile()[k, m].active = false;
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 0.5)
                {
                    vector2.X = 0.5f;
                }
                if (vector2.X < -0.5)
                {
                    vector2.X = -0.5f;
                }
                if (vector2.Y > 1.5)
                {
                    vector2.Y = 1.5f;
                }
                if (vector2.Y < 0.5)
                {
                    vector2.Y = 0.5f;
                }
            }
        }

        public static void AddPlants(World world)
        {
            for (int i = 0; i < world.getMaxTilesX(); i++)
            {
                for (int j = 1; j < world.getMaxTilesY(); j++)
                {
                    if ((world.getTile()[i, j].type == 2) && world.getTile()[i, j].active)
                    {
                        if (!world.getTile()[i, j - 1].active)
                        {
                            PlaceTile(i, j - 1, world, 3, true, false, -1);
                        }
                    }
                    else if (((world.getTile()[i, j].type == 0x17) && world.getTile()[i, j].active) && !world.getTile()[i, j - 1].active)
                    {
                        PlaceTile(i, j - 1, world, 0x18, true, false, -1);
                    }
                }
            }
        }

        public static void FloatingIsland(int i, int j, World world)
        {
            double num = (double)WorldGen.genRand.Next(80, 120);
            float num2 = (float)WorldGen.genRand.Next(20, 25);
            Vector2 value = new Vector2();
            value.X = (float)i;
            value.Y = (float)j;
            Vector2 value2 = new Vector2();
            value2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (value2.X > -2f && value2.X < 2f)
            {
                value2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            value2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num3 = (int)((double)value.X - num * 0.5);
                int num4 = (int)((double)value.X + num * 0.5);
                int num5 = (int)((double)value.Y - num * 0.5);
                int num6 = (int)((double)value.Y + num * 0.5);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > world.getMaxTilesX())
                {
                    num4 = world.getMaxTilesX();
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > world.getMaxTilesX())
                {
                    num6 = world.getMaxTilesX();
                }
                double num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                float num8 = value.Y + 1f;
                for (int k = num3; k < num4; k++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num8 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num8 < value.Y)
                    {
                        num8 = value.Y;
                    }
                    if (num8 > value.Y + 2f)
                    {
                        num8 = value.Y + 2f;
                    }
                    for (int l = num5; l < num6; l++)
                    {
                        if ((float)l > num8)
                        {
                            float num9 = Math.Abs((float)k - value.X);
                            float num10 = Math.Abs((float)l - value.Y) * 2f;
                            double num11 = Math.Sqrt((double)(num9 * num9 + num10 * num10));
                            if (num11 < num7 * 0.4)
                            {
                                world.getTile()[k, l].active = true;
                            }
                        }
                    }
                }
                WorldGen.TileRunner(WorldGen.genRand.Next(num3 + 10, num4 - 10), (int)((double)value.Y + num7 * 0.1 + 5.0), world, (double)WorldGen.genRand.Next(5, 10), WorldGen.genRand.Next(10, 15), 0, true, 0f, 2f, true, true);
                num3 = (int)((double)value.X - num * 0.4);
                num4 = (int)((double)value.X + num * 0.4);
                num5 = (int)((double)value.Y - num * 0.4);
                num6 = (int)((double)value.Y + num * 0.4);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > world.getMaxTilesX())
                {
                    num4 = world.getMaxTilesX();
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > world.getMaxTilesX())
                {
                    num6 = world.getMaxTilesX();
                }
                num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                for (int m = num3; m < num4; m++)
                {
                    for (int n = num5; n < num6; n++)
                    {
                        if ((float)n > value.Y + 2f)
                        {
                            float num12 = Math.Abs((float)m - value.X);
                            float num13 = Math.Abs((float)n - value.Y) * 2f;
                            double num14 = Math.Sqrt((double)(num12 * num12 + num13 * num13));
                            if (num14 < num7 * 0.4)
                            {
                                world.getTile()[m, n].wall = 2;
                            }
                        }
                    }
                }
                value += value2;
                value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                if (value2.X > 1f)
                {
                    value2.X = 1f;
                }
                if (value2.X < -1f)
                {
                    value2.X = -1f;
                }
                if ((double)value2.Y > 0.2)
                {
                    value2.Y = -0.2f;
                }
                if ((double)value2.Y < -0.2)
                {
                    value2.Y = -0.2f;
                }
            }
        }

        public static void Mountinater(int i, int j, World world)
        {
            double num = (double)WorldGen.genRand.Next(80, 120);
            float num2 = (float)WorldGen.genRand.Next(40, 55);
            Vector2 value = new Vector2();
            value.X = (float)i;
            value.Y = (float)j + num2 / 2f;
            Vector2 value2 = new Vector2();
            value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            value2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.1f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num3 = (int)((double)value.X - num * 0.5);
                int num4 = (int)((double)value.X + num * 0.5);
                int num5 = (int)((double)value.Y - num * 0.5);
                int num6 = (int)((double)value.Y + num * 0.5);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > world.getMaxTilesX())
                {
                    num4 = world.getMaxTilesX();
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > world.getMaxTilesX())
                {
                    num6 = world.getMaxTilesX();
                }
                double num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                for (int k = num3; k < num4; k++)
                {
                    for (int l = num5; l < num6; l++)
                    {
                        float num8 = Math.Abs((float)k - value.X);
                        float num9 = Math.Abs((float)l - value.Y);
                        double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        if (num10 < num7 * 0.4 && !world.getTile()[k, l].active)
                        {
                            world.getTile()[k, l].active = true;
                            world.getTile()[k, l].type = 0;
                        }
                    }
                }
                value += value2;
                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                if ((double)value2.X > 0.5)
                {
                    value2.X = 0.5f;
                }
                if ((double)value2.X < -0.5)
                {
                    value2.X = -0.5f;
                }
                if ((double)value2.Y > -0.5)
                {
                    value2.Y = -0.5f;
                }
                if ((double)value2.Y < -1.5)
                {
                    value2.Y = -1.5f;
                }
            }
        }

        public static bool EmptyTileCheck(int startX, int endX, int startY, int endY, World world, int ignoreStyle = -1)
        {
            if (startX < 0)
            {
                return false;
            }
            if (endX >= world.getMaxTilesX())
            {
                return false;
            }
            if (startY < 0)
            {
                return false;
            }
            if (endY >= world.getMaxTilesY())
            {
                return false;
            }
            for (int i = startX; i < (endX + 1); i++)
            {
                for (int j = startY; j < (endY + 1); j++)
                {
                    if (world.getTile()[i, j].active)
                    {
                        if (ignoreStyle == -1)
                        {
                            return false;
                        }
                        if ((ignoreStyle == 11) && (world.getTile()[i, j].type != 11))
                        {
                            return false;
                        }
                        if (((ignoreStyle == 20) && (world.getTile()[i, j].type != 20)) && (world.getTile()[i, j].type != 3))
                        {
                            return false;
                        }
                        if ((ignoreStyle == 0x47) && (world.getTile()[i, j].type != 0x47))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static void SpawnNPC(int x, int y, World world)
        {
            if (Statics.wallHouse[world.getTile()[x, y].wall])
            {
                canSpawn = true;
            }
            if ((canSpawn && StartRoomCheck(x, y, world)) && RoomNeeds(spawnNPC))
            {
                ScoreRoom(world, -1);
                if (hiScore > 0)
                {
                    int index = -1;
                    for (int i = 0; i < 0x3e8; i++)
                    {
                        if ((world.getNPCs()[i].active && world.getNPCs()[i].homeless) && (world.getNPCs()[i].type == spawnNPC))
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index != -1)
                    {
                        spawnNPC = 0;
                        world.getNPCs()[index].homeTileX = WorldGen.bestX;
                        world.getNPCs()[index].homeTileY = WorldGen.bestY;
                        world.getNPCs()[index].homeless = false;
                    }
                    else
                    {
                        int bestX = WorldGen.bestX;
                        int bestY = WorldGen.bestY;
                        bool flag = false;
                        if (!flag)
                        {
                            flag = true;
                            Rectangle rectangle = new Rectangle((((bestX * 0x10) + 8) - (Statics.screenWidth / 2)) - NPC.safeRangeX,
                                (((bestY * 0x10) + 8) - (Statics.screenHeight / 2)) - NPC.safeRangeY, Statics.screenWidth + (NPC.safeRangeX * 2),
                                Statics.screenHeight + (NPC.safeRangeY * 2));
                            for (int j = 0; j < 8; j++)
                            {
                                if (world.getPlayerList()[j].active)
                                {
                                    Rectangle rectangle2 = new Rectangle((int)world.getPlayerList()[j].position.X,
                                        (int)world.getPlayerList()[j].position.Y, world.getPlayerList()[j].width, world.getPlayerList()[j].height);
                                    if (rectangle2.Intersects(rectangle))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!flag)
                        {
                            for (int k = 1; k < 500; k++)
                            {
                                for (int m = 0; m < 2; m++)
                                {
                                    if (m == 0)
                                    {
                                        bestX = WorldGen.bestX + k;
                                    }
                                    else
                                    {
                                        bestX = WorldGen.bestX - k;
                                    }
                                    if ((bestX > 10) && (bestX < (world.getMaxTilesX() - 10)))
                                    {
                                        int num8 = WorldGen.bestY - k;
                                        double worldSurface = WorldGen.bestY + k;
                                        if (num8 < 10)
                                        {
                                            num8 = 10;
                                        }
                                        if (worldSurface > world.getWorldSurface())
                                        {
                                            worldSurface = world.getWorldSurface();
                                        }
                                        for (int n = num8; n < worldSurface; n++)
                                        {
                                            bestY = n;
                                            if (world.getTile()[bestX, bestY].active && Statics.tileSolid[world.getTile()[bestX, bestY].type])
                                            {
                                                if (!Collision.SolidTiles(bestX - 1, bestX + 1, bestY - 3, bestY - 1, world))
                                                {
                                                    flag = true;
                                                    Rectangle rectangle3 = new Rectangle((((bestX * 0x10) + 8) - (Statics.screenWidth / 2)) -
                                                        NPC.safeRangeX, (((bestY * 0x10) + 8) - (Statics.screenHeight / 2)) - NPC.safeRangeY,
                                                        Statics.screenWidth + (NPC.safeRangeX * 2), Statics.screenHeight + (NPC.safeRangeY * 2));
                                                    for (int num11 = 0; num11 < 8; num11++)
                                                    {
                                                        if (world.getPlayerList()[num11].active)
                                                        {
                                                            Rectangle rectangle4 = new Rectangle((int)world.getPlayerList()[num11].position.X,
                                                                (int)world.getPlayerList()[num11].position.Y, world.getPlayerList()[num11].width,
                                                                world.getPlayerList()[num11].height);
                                                            if (rectangle4.Intersects(rectangle3))
                                                            {
                                                                flag = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    if (flag)
                                    {
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    break;
                                }
                            }
                        }
                        int num12 = NPC.NewNPC(bestX * 0x10, bestY * 0x10, world, spawnNPC, 1);
                        world.getNPCs()[num12].homeTileX = WorldGen.bestX;
                        world.getNPCs()[num12].homeTileY = WorldGen.bestY;
                        if (bestX < WorldGen.bestX)
                        {
                            world.getNPCs()[num12].direction = 1;
                        }
                        else if (bestX > WorldGen.bestX)
                        {
                            world.getNPCs()[num12].direction = -1;
                        }
                        world.getNPCs()[num12].netUpdate = true;
                        if (Statics.netMode == 0)
                        {
                            //Main.NewText(world.getNPCs()[num12].name + " has arrived!", 50, 0x7d, 0xff);
                        }
                        else if (Statics.netMode == 2)
                        {
                            NetMessage.SendData(0x19, world, -1, -1, world.getNPCs()[num12].name + " has arrived!", 8, 50f, 125f, 255f);
                        }
                    }
                    spawnNPC = 0;
                }
            }
        }

        public static bool PlayerLOS(int x, int y, World world)
        {
            Rectangle rectangle = new Rectangle(x * 0x10, y * 0x10, 0x10, 0x10);
            for (int i = 0; i < 8; i++)
            {
                if (world.getPlayerList()[i].active)
                {
                    Rectangle rectangle2 = new Rectangle((int)((world.getPlayerList()[i].position.X + (world.getPlayerList()[i].width * 0.5)) - (Statics.screenWidth * 0.6)),
                        (int)((world.getPlayerList()[i].position.Y + (world.getPlayerList()[i].height * 0.5)) - (Statics.screenHeight * 0.6)),
                        (int)(Statics.screenWidth * 1.2), (int)(Statics.screenHeight * 1.2));
                    if (rectangle.Intersects(rectangle2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void GrowTree(int i, int y, World world)
        {
            int num2;
            int num = y;
            while (world.getTile()[i, num].type == 20)
            {
                num++;
            }
            if (((world.getTile()[i - 1, num - 1].liquid != 0) || (world.getTile()[i - 1, num - 1].liquid != 0)) ||
                (world.getTile()[i + 1, num - 1].liquid != 0))
            {
                return;
            }
            if (((!world.getTile()[i, num].active || (world.getTile()[i, num].type != 2)) || ((world.getTile()[i, num - 1].wall != 0) ||
                !world.getTile()[i - 1, num].active)) || (((world.getTile()[i - 1, num].type != 2) ||
                !world.getTile()[i + 1, num].active) || ((world.getTile()[i + 1, num].type != 2) ||
                !EmptyTileCheck(i - 2, i + 2, num - 14, num - 1, world, 20))))
            {
                return;
            }
            bool flag = false;
            bool flag2 = false;
            int num4 = genRand.Next(5, 15);
            for (int j = num - num4; j < num; j++)
            {
                world.getTile()[i, j].frameNumber = (byte)genRand.Next(3);
                world.getTile()[i, j].active = true;
                world.getTile()[i, j].type = 5;
                num2 = genRand.Next(3);
                int num3 = genRand.Next(10);
                if ((j == (num - 1)) || (j == (num - num4)))
                {
                    num3 = 0;
                }
                while ((((num3 == 5) || (num3 == 7)) && flag) || (((num3 == 6) || (num3 == 7)) && flag2))
                {
                    num3 = genRand.Next(10);
                }
                flag = false;
                flag2 = false;
                if ((num3 == 5) || (num3 == 7))
                {
                    flag = true;
                }
                if ((num3 == 6) || (num3 == 7))
                {
                    flag2 = true;
                }
                if (num3 == 1)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 2)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0x16;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0x2c;
                            break;
                    }
                }
                else if (num3 == 3)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x2c;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x2c;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x2c;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 4)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x16;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 5)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x58;
                            world.getTile()[i, j].frameY = 0;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x58;
                            world.getTile()[i, j].frameY = 0x16;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x58;
                            world.getTile()[i, j].frameY = 0x2c;
                            break;
                    }
                }
                else if (num3 == 6)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0x42;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0x42;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0x42;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 7)
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 110;
                            world.getTile()[i, j].frameY = 0x42;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 110;
                            world.getTile()[i, j].frameY = 0x58;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 110;
                            world.getTile()[i, j].frameY = 110;
                            break;
                    }
                }
                else
                {
                    switch (num2)
                    {
                        case 0:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0;
                            break;

                        case 1:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x16;
                            break;

                        case 2:
                            world.getTile()[i, j].frameX = 0;
                            world.getTile()[i, j].frameY = 0x2c;
                            break;
                    }
                }
                if ((num3 == 5) || (num3 == 7))
                {
                    world.getTile()[i - 1, j].active = true;
                    world.getTile()[i - 1, j].type = 5;
                    num2 = genRand.Next(3);
                    if (genRand.Next(3) < 2)
                    {
                        switch (num2)
                        {
                            case 0:
                                world.getTile()[i - 1, j].frameX = 0x2c;
                                world.getTile()[i - 1, j].frameY = 0xc6;
                                break;

                            case 1:
                                world.getTile()[i - 1, j].frameX = 0x2c;
                                world.getTile()[i - 1, j].frameY = 220;
                                break;

                            case 2:
                                world.getTile()[i - 1, j].frameX = 0x2c;
                                world.getTile()[i - 1, j].frameY = 0xf2;
                                break;
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 0:
                                world.getTile()[i - 1, j].frameX = 0x42;
                                world.getTile()[i - 1, j].frameY = 0;
                                break;

                            case 1:
                                world.getTile()[i - 1, j].frameX = 0x42;
                                world.getTile()[i - 1, j].frameY = 0x16;
                                break;

                            case 2:
                                world.getTile()[i - 1, j].frameX = 0x42;
                                world.getTile()[i - 1, j].frameY = 0x2c;
                                break;
                        }
                    }
                }
                if ((num3 == 6) || (num3 == 7))
                {
                    world.getTile()[i + 1, j].active = true;
                    world.getTile()[i + 1, j].type = 5;
                    num2 = genRand.Next(3);
                    if (genRand.Next(3) < 2)
                    {
                        switch (num2)
                        {
                            case 0:
                                world.getTile()[i + 1, j].frameX = 0x42;
                                world.getTile()[i + 1, j].frameY = 0xc6;
                                break;

                            case 1:
                                world.getTile()[i + 1, j].frameX = 0x42;
                                world.getTile()[i + 1, j].frameY = 220;
                                break;

                            case 2:
                                world.getTile()[i + 1, j].frameX = 0x42;
                                world.getTile()[i + 1, j].frameY = 0xf2;
                                break;
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 0:
                                world.getTile()[i + 1, j].frameX = 0x58;
                                world.getTile()[i + 1, j].frameY = 0x42;
                                break;

                            case 1:
                                world.getTile()[i + 1, j].frameX = 0x58;
                                world.getTile()[i + 1, j].frameY = 0x58;
                                break;

                            case 2:
                                world.getTile()[i + 1, j].frameX = 0x58;
                                world.getTile()[i + 1, j].frameY = 110;
                                break;
                        }
                    }
                }
            }
            int num6 = genRand.Next(3);
            if ((num6 == 0) || (num6 == 1))
            {
                world.getTile()[i + 1, num - 1].active = true;
                world.getTile()[i + 1, num - 1].type = 5;
                switch (genRand.Next(3))
                {
                    case 0:
                        world.getTile()[i + 1, num - 1].frameX = 0x16;
                        world.getTile()[i + 1, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i + 1, num - 1].frameX = 0x16;
                        world.getTile()[i + 1, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i + 1, num - 1].frameX = 0x16;
                        world.getTile()[i + 1, num - 1].frameY = 0xb0;
                        goto Label_0A63;
                }
            }
        Label_0A63:
            if ((num6 == 0) || (num6 == 2))
            {
                world.getTile()[i - 1, num - 1].active = true;
                world.getTile()[i - 1, num - 1].type = 5;
                switch (genRand.Next(3))
                {
                    case 0:
                        world.getTile()[i - 1, num - 1].frameX = 0x2c;
                        world.getTile()[i - 1, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i - 1, num - 1].frameX = 0x2c;
                        world.getTile()[i - 1, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i - 1, num - 1].frameX = 0x2c;
                        world.getTile()[i - 1, num - 1].frameY = 0xb0;
                        break;
                }
            }
            num2 = genRand.Next(3);
            if (num6 == 0)
            {
                switch (num2)
                {
                    case 0:
                        world.getTile()[i, num - 1].frameX = 0x58;
                        world.getTile()[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i, num - 1].frameX = 0x58;
                        world.getTile()[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i, num - 1].frameX = 0x58;
                        world.getTile()[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            else if (num6 == 1)
            {
                switch (num2)
                {
                    case 0:
                        world.getTile()[i, num - 1].frameX = 0;
                        world.getTile()[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i, num - 1].frameX = 0;
                        world.getTile()[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i, num - 1].frameX = 0;
                        world.getTile()[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            else if (num6 == 2)
            {
                switch (num2)
                {
                    case 0:
                        world.getTile()[i, num - 1].frameX = 0x42;
                        world.getTile()[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        world.getTile()[i, num - 1].frameX = 0x42;
                        world.getTile()[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        world.getTile()[i, num - 1].frameX = 0x42;
                        world.getTile()[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            if (genRand.Next(3) < 2)
            {
                switch (genRand.Next(3))
                {
                    case 0:
                        world.getTile()[i, num - num4].frameX = 0x16;
                        world.getTile()[i, num - num4].frameY = 0xc6;
                        break;

                    case 1:
                        world.getTile()[i, num - num4].frameX = 0x16;
                        world.getTile()[i, num - num4].frameY = 220;
                        break;

                    case 2:
                        world.getTile()[i, num - num4].frameX = 0x16;
                        world.getTile()[i, num - num4].frameY = 0xf2;
                        break;
                }
            }
            else
            {
                switch (genRand.Next(3))
                {
                    case 0:
                        world.getTile()[i, num - num4].frameX = 0;
                        world.getTile()[i, num - num4].frameY = 0xc6;
                        break;

                    case 1:
                        world.getTile()[i, num - num4].frameX = 0;
                        world.getTile()[i, num - num4].frameY = 220;
                        break;

                    case 2:
                        world.getTile()[i, num - num4].frameX = 0;
                        world.getTile()[i, num - num4].frameY = 0xf2;
                        break;
                }
            }
            RangeFrame(i - 2, (num - num4) - 1, i + 2, num + 1, world);
            if (Statics.netMode == 2)
            {
                NetMessage.SendTileSquare(-1, i, num - ((int)(num4 * 0.5)), num4 + 1, world);
            }
        }

        public static void SpreadGrass(int i, int j, World world, int dirt = 0, int grass = 2, bool repeat = true)
        {
            if (((world.getTile()[i, j].type == dirt) && world.getTile()[i, j].active) && ((j < world.getWorldSurface()) || (dirt == 0x3b)))
            {
                int num = i - 1;
                int maxTilesX = i + 2;
                int num3 = j - 1;
                int maxTilesY = j + 2;
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (maxTilesY > world.getMaxTilesY())
                {
                    maxTilesY = world.getMaxTilesY();
                }
                bool flag = true;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num3; m < maxTilesY; m++)
                    {
                        if (!world.getTile()[k, m].active || !Statics.tileSolid[world.getTile()[k, m].type])
                        {
                            flag = false;
                            goto Label_00BB;
                        }
                    }
                Label_00BB: ;
                }
                if (!flag && ((grass != 0x17) || (world.getTile()[i, j - 1].type != 0x1b)))
                {
                    world.getTile()[i, j].type = (byte)grass;
                    for (int n = num; n < maxTilesX; n++)
                    {
                        for (int num8 = num3; num8 < maxTilesY; num8++)
                        {
                            if ((world.getTile()[n, num8].active && (world.getTile()[n, num8].type == dirt)) && repeat)
                            {
                                SpreadGrass(n, num8, world, dirt, grass, true);
                            }
                        }
                    }
                }
            }
        }

        public static void UpdateWorld(World world)
        {
            Liquid.skipCount++;
            if (Liquid.skipCount > 1)
            {
                Liquid.UpdateLiquid(world);
                Liquid.skipCount = 0;
            }
            float num = 4E-05f;
            float num2 = 2E-05f;
            bool flag = false;
            spawnDelay++;
            if (world.getInvasionType() > 0)
            {
                spawnDelay = 0;
            }
            if (spawnDelay >= 20)
            {
                flag = true;
                spawnDelay = 0;
                for (int k = 0; k < 0x3e8; k++)
                {
                    if ((world.getNPCs()[k].active && world.getNPCs()[k].homeless) && world.getNPCs()[k].townNPC)
                    {
                        spawnNPC = world.getNPCs()[k].type;
                        break;
                    }
                }
            }
            for (int i = 0; i < ((world.getMaxTilesX() * world.getMaxTilesY()) * num); i++)
            {
                int num5 = genRand.Next(10, world.getMaxTilesX() - 10);
                int num6 = genRand.Next(10, ((int)world.getWorldSurface()) - 1);
                int num7 = num5 - 1;
                int num8 = num5 + 2;
                int num9 = num6 - 1;
                int num10 = num6 + 2;
                if (num7 < 10)
                {
                    num7 = 10;
                }
                if (num8 > (world.getMaxTilesX() - 10))
                {
                    num8 = world.getMaxTilesX() - 10;
                }
                if (num9 < 10)
                {
                    num9 = 10;
                }
                if (num10 > (world.getMaxTilesY() - 10))
                {
                    num10 = world.getMaxTilesY() - 10;
                }
                if (world.getTile()[num5, num6] != null)
                {
                    if (world.getTile()[num5, num6].liquid > 0x20)
                    {
                        if (world.getTile()[num5, num6].active && (((world.getTile()[num5, num6].type == 3) || (world.getTile()[num5, num6].type == 20)) || (((world.getTile()[num5, num6].type == 0x18) || (world.getTile()[num5, num6].type == 0x1b)) || (world.getTile()[num5, num6].type == 0x49))))
                        {
                            KillTile(num5, num6, world, false, false, false);
                            if (Statics.netMode == 2)
                            {
                                NetMessage.SendData(0x11, world, -1, -1, "", 0, (float)num5, (float)num6, 0f);
                            }
                        }
                    }
                    else if (world.getTile()[num5, num6].active)
                    {
                        if (world.getTile()[num5, num6].type == 0x4e)
                        {
                            if (!world.getTile()[num5, num9].active)
                            {
                                PlaceTile(num5, num9, world, 3, true, false, -1);
                                if ((Statics.netMode == 2) && world.getTile()[num5, num9].active)
                                {
                                    NetMessage.SendTileSquare(-1, num5, num9, 1, world);
                                }
                            }
                        }
                        else if (((world.getTile()[num5, num6].type == 2) || (world.getTile()[num5, num6].type == 0x17)) || (world.getTile()[num5, num6].type == 0x20))
                        {
                            int type = world.getTile()[num5, num6].type;
                            if ((!world.getTile()[num5, num9].active && (genRand.Next(10) == 0)) && (type == 2))
                            {
                                PlaceTile(num5, num9, world, 3, true, false, -1);
                                if ((Statics.netMode == 2) && world.getTile()[num5, num9].active)
                                {
                                    NetMessage.SendTileSquare(-1, num5, num9, 1, world);
                                }
                            }
                            if ((!world.getTile()[num5, num9].active && (genRand.Next(10) == 0)) && (type == 0x17))
                            {
                                PlaceTile(num5, num9, world, 0x18, true, false, -1);
                                if ((Statics.netMode == 2) && world.getTile()[num5, num9].active)
                                {
                                    NetMessage.SendTileSquare(-1, num5, num9, 1, world);
                                }
                            }
                            bool flag2 = false;
                            for (int m = num7; m < num8; m++)
                            {
                                for (int n = num9; n < num10; n++)
                                {
                                    if (((num5 != m) || (num6 != n)) && world.getTile()[m, n].active)
                                    {
                                        if (type == 0x20)
                                        {
                                            type = 0x17;
                                        }
                                        if ((world.getTile()[m, n].type == 0) || ((type == 0x17) && (world.getTile()[m, n].type == 2)))
                                        {
                                            SpreadGrass(m, n, world, 0, type, false);
                                            if (type == 0x17)
                                            {
                                                SpreadGrass(m, n, world, 2, type, false);
                                            }
                                            if (world.getTile()[m, n].type == type)
                                            {
                                                SquareTileFrame(m, n, world, true);
                                                flag2 = true;
                                            }
                                        }
                                    }
                                }
                            }
                            if ((Statics.netMode == 2) && flag2)
                            {
                                NetMessage.SendTileSquare(-1, num5, num6, 3, world);
                            }
                        }
                        else if (((world.getTile()[num5, num6].type == 20) && !PlayerLOS(num5, num6, world)) && (genRand.Next(5) == 0))
                        {
                            GrowTree(num5, num6, world);
                        }
                        if (((world.getTile()[num5, num6].type == 3) && (genRand.Next(10) == 0)) && (world.getTile()[num5, num6].frameX < 0x90))
                        {
                            world.getTile()[num5, num6].type = 0x49;
                            if (Statics.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num5, num6, 3, world);
                            }
                        }
                        if ((world.getTile()[num5, num6].type == 0x20) && (genRand.Next(3) == 0))
                        {
                            int num14 = num5;
                            int num15 = num6;
                            int num16 = 0;
                            if (world.getTile()[num14 + 1, num15].active && (world.getTile()[num14 + 1, num15].type == 0x20))
                            {
                                num16++;
                            }
                            if (world.getTile()[num14 - 1, num15].active && (world.getTile()[num14 - 1, num15].type == 0x20))
                            {
                                num16++;
                            }
                            if (world.getTile()[num14, num15 + 1].active && (world.getTile()[num14, num15 + 1].type == 0x20))
                            {
                                num16++;
                            }
                            if (world.getTile()[num14, num15 - 1].active && (world.getTile()[num14, num15 - 1].type == 0x20))
                            {
                                num16++;
                            }
                            if ((num16 < 3) || (world.getTile()[num5, num6].type == 0x17))
                            {
                                switch (genRand.Next(4))
                                {
                                    case 0:
                                        num15--;
                                        break;

                                    case 1:
                                        num15++;
                                        break;

                                    case 2:
                                        num14--;
                                        break;

                                    case 3:
                                        num14++;
                                        break;
                                }
                                if (!world.getTile()[num14, num15].active)
                                {
                                    num16 = 0;
                                    if (world.getTile()[num14 + 1, num15].active && (world.getTile()[num14 + 1, num15].type == 0x20))
                                    {
                                        num16++;
                                    }
                                    if (world.getTile()[num14 - 1, num15].active && (world.getTile()[num14 - 1, num15].type == 0x20))
                                    {
                                        num16++;
                                    }
                                    if (world.getTile()[num14, num15 + 1].active && (world.getTile()[num14, num15 + 1].type == 0x20))
                                    {
                                        num16++;
                                    }
                                    if (world.getTile()[num14, num15 - 1].active && (world.getTile()[num14, num15 - 1].type == 0x20))
                                    {
                                        num16++;
                                    }
                                    if (num16 < 2)
                                    {
                                        int num18 = 7;
                                        int num19 = num14 - num18;
                                        int num20 = num14 + num18;
                                        int num21 = num15 - num18;
                                        int num22 = num15 + num18;
                                        bool flag3 = false;
                                        for (int num23 = num19; num23 < num20; num23++)
                                        {
                                            for (int num24 = num21; num24 < num22; num24++)
                                            {
                                                if ((((((Math.Abs((int)(num23 - num14)) * 2) + Math.Abs((int)(num24 - num15))) < 9) && world.getTile()[num23, num24].active) && ((world.getTile()[num23, num24].type == 0x17) && world.getTile()[num23, num24 - 1].active)) && ((world.getTile()[num23, num24 - 1].type == 0x20) && (world.getTile()[num23, num24 - 1].liquid == 0)))
                                                {
                                                    flag3 = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (flag3)
                                        {
                                            world.getTile()[num14, num15].type = 0x20;
                                            world.getTile()[num14, num15].active = true;
                                            SquareTileFrame(num14, num15, world, true);
                                            if (Statics.netMode == 2)
                                            {
                                                NetMessage.SendTileSquare(-1, num14, num15, 3, world);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (((world.getTile()[num5, num6].type == 2) || (world.getTile()[num5, num6].type == 0x34)) && (((genRand.Next(5) == 0) && !world.getTile()[num5, num6 + 1].active) && !world.getTile()[num5, num6 + 1].lava))
                        {
                            bool flag4 = false;
                            for (int num25 = num6; num25 > (num6 - 10); num25--)
                            {
                                if (world.getTile()[num5, num25].active && (world.getTile()[num5, num25].type == 2))
                                {
                                    flag4 = true;
                                    break;
                                }
                            }
                            if (flag4)
                            {
                                int num26 = num5;
                                int num27 = num6 + 1;
                                world.getTile()[num26, num27].type = 0x34;
                                world.getTile()[num26, num27].active = true;
                                SquareTileFrame(num26, num27, world, true);
                                if (Statics.netMode == 2)
                                {
                                    NetMessage.SendTileSquare(-1, num26, num27, 3, world);
                                }
                            }
                        }
                    }
                    else if (flag && (spawnNPC > 0))
                    {
                        SpawnNPC(num5, num6, world);
                    }
                }
            }
            for (int j = 0; j < ((world.getMaxTilesX() * world.getMaxTilesY()) * num2); j++)
            {
                int num29 = genRand.Next(10, world.getMaxTilesX() - 10);
                int num30 = genRand.Next(((int)world.getWorldSurface()) + 2, world.getMaxTilesY() - 200);
                int num31 = num29 - 1;
                int num32 = num29 + 2;
                int num33 = num30 - 1;
                int num34 = num30 + 2;
                if (num31 < 10)
                {
                    num31 = 10;
                }
                if (num32 > (world.getMaxTilesX() - 10))
                {
                    num32 = world.getMaxTilesX() - 10;
                }
                if (num33 < 10)
                {
                    num33 = 10;
                }
                if (num34 > (world.getMaxTilesY() - 10))
                {
                    num34 = world.getMaxTilesY() - 10;
                }
                if (world.getTile()[num29, num30] != null)
                {
                    if (world.getTile()[num29, num30].liquid > 0x20)
                    {
                        if (world.getTile()[num29, num30].active && ((world.getTile()[num29, num30].type == 0x3d) || (world.getTile()[num29, num30].type == 0x4a)))
                        {
                            KillTile(num29, num30, world, false, false, false);
                            if (Statics.netMode == 2)
                            {
                                NetMessage.SendData(0x11, world, -1, -1, "", 0, (float)num29, (float)num30, 0f);
                            }
                        }
                        continue;
                    }
                    if (world.getTile()[num29, num30].active)
                    {
                        if (world.getTile()[num29, num30].type == 60)
                        {
                            int grass = world.getTile()[num29, num30].type;
                            if (!world.getTile()[num29, num33].active && (genRand.Next(10) == 0))
                            {
                                PlaceTile(num29, num33, world, 0x3d, true, false, -1);
                                if ((Statics.netMode == 2) && world.getTile()[num29, num33].active)
                                {
                                    NetMessage.SendTileSquare(-1, num29, num33, 1, world);
                                }
                            }
                            bool flag5 = false;
                            for (int num36 = num31; num36 < num32; num36++)
                            {
                                for (int num37 = num33; num37 < num34; num37++)
                                {
                                    if (((num29 != num36) || (num30 != num37)) && (world.getTile()[num36, num37].active && (world.getTile()[num36, num37].type == 0x3b)))
                                    {
                                        SpreadGrass(num36, num37, world, 0x3b, grass, false);
                                        if (world.getTile()[num36, num37].type == grass)
                                        {
                                            SquareTileFrame(num36, num37, world, true);
                                            flag5 = true;
                                        }
                                    }
                                }
                            }
                            if ((Statics.netMode == 2) && flag5)
                            {
                                NetMessage.SendTileSquare(-1, num29, num30, 3, world);
                            }
                        }
                        if (((world.getTile()[num29, num30].type == 0x3d) && (genRand.Next(3) == 0)) && (world.getTile()[num29, num30].frameX < 0x90))
                        {
                            world.getTile()[num29, num30].type = 0x4a;
                            if (Statics.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num29, num30, 3, world);
                            }
                        }
                        if (((world.getTile()[num29, num30].type == 60) || (world.getTile()[num29, num30].type == 0x3e)) && (((genRand.Next(5) == 0) && !world.getTile()[num29, num30 + 1].active) && !world.getTile()[num29, num30 + 1].lava))
                        {
                            bool flag6 = false;
                            for (int num38 = num30; num38 > (num30 - 10); num38--)
                            {
                                if (world.getTile()[num29, num38].active && (world.getTile()[num29, num38].type == 60))
                                {
                                    flag6 = true;
                                    break;
                                }
                            }
                            if (flag6)
                            {
                                int num39 = num29;
                                int num40 = num30 + 1;
                                world.getTile()[num39, num40].type = 0x3e;
                                world.getTile()[num39, num40].active = true;
                                SquareTileFrame(num39, num40, world, true);
                                if (Statics.netMode == 2)
                                {
                                    NetMessage.SendTileSquare(-1, num39, num40, 3, world);
                                }
                            }
                        }
                        if ((world.getTile()[num29, num30].type == 0x45) && (genRand.Next(3) == 0))
                        {
                            int num41 = num29;
                            int num42 = num30;
                            int num43 = 0;
                            if (world.getTile()[num41 + 1, num42].active && (world.getTile()[num41 + 1, num42].type == 0x45))
                            {
                                num43++;
                            }
                            if (world.getTile()[num41 - 1, num42].active && (world.getTile()[num41 - 1, num42].type == 0x45))
                            {
                                num43++;
                            }
                            if (world.getTile()[num41, num42 + 1].active && (world.getTile()[num41, num42 + 1].type == 0x45))
                            {
                                num43++;
                            }
                            if (world.getTile()[num41, num42 - 1].active && (world.getTile()[num41, num42 - 1].type == 0x45))
                            {
                                num43++;
                            }
                            if ((num43 < 3) || (world.getTile()[num29, num30].type == 60))
                            {
                                switch (genRand.Next(4))
                                {
                                    case 0:
                                        num42--;
                                        break;

                                    case 1:
                                        num42++;
                                        break;

                                    case 2:
                                        num41--;
                                        break;

                                    case 3:
                                        num41++;
                                        break;
                                }
                                if (!world.getTile()[num41, num42].active)
                                {
                                    num43 = 0;
                                    if (world.getTile()[num41 + 1, num42].active && (world.getTile()[num41 + 1, num42].type == 0x45))
                                    {
                                        num43++;
                                    }
                                    if (world.getTile()[num41 - 1, num42].active && (world.getTile()[num41 - 1, num42].type == 0x45))
                                    {
                                        num43++;
                                    }
                                    if (world.getTile()[num41, num42 + 1].active && (world.getTile()[num41, num42 + 1].type == 0x45))
                                    {
                                        num43++;
                                    }
                                    if (world.getTile()[num41, num42 - 1].active && (world.getTile()[num41, num42 - 1].type == 0x45))
                                    {
                                        num43++;
                                    }
                                    if (num43 < 2)
                                    {
                                        int num45 = 7;
                                        int num46 = num41 - num45;
                                        int num47 = num41 + num45;
                                        int num48 = num42 - num45;
                                        int num49 = num42 + num45;
                                        bool flag7 = false;
                                        for (int num50 = num46; num50 < num47; num50++)
                                        {
                                            for (int num51 = num48; num51 < num49; num51++)
                                            {
                                                if ((((((Math.Abs((int)(num50 - num41)) * 2) + Math.Abs((int)(num51 - num42))) < 9) &&
                                                    world.getTile()[num50, num51].active) && ((world.getTile()[num50, num51].type == 60)
                                                    && world.getTile()[num50, num51 - 1].active)) && ((world.getTile()[num50, num51 - 1].type == 0x45)
                                                    && (world.getTile()[num50, num51 - 1].liquid == 0)))
                                                {
                                                    flag7 = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (flag7)
                                        {
                                            world.getTile()[num41, num42].type = 0x45;
                                            world.getTile()[num41, num42].active = true;
                                            SquareTileFrame(num41, num42, world, true);
                                            if (Statics.netMode == 2)
                                            {
                                                NetMessage.SendTileSquare(-1, num41, num42, 3, world);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (world.getTile()[num29, num30].type == 70)
                        {
                            int num52 = world.getTile()[num29, num30].type;
                            if (!world.getTile()[num29, num33].active && (genRand.Next(10) == 0))
                            {
                                PlaceTile(num29, num33, world, 0x47, true, false, -1);
                                if ((Statics.netMode == 2) && world.getTile()[num29, num33].active)
                                {
                                    NetMessage.SendTileSquare(-1, num29, num33, 1, world);
                                }
                            }
                            bool flag8 = false;
                            for (int num53 = num31; num53 < num32; num53++)
                            {
                                for (int num54 = num33; num54 < num34; num54++)
                                {
                                    if (((num29 != num53) || (num30 != num54)) && (world.getTile()[num53, num54].active && (world.getTile()[num53, num54].type == 0x3b)))
                                    {
                                        SpreadGrass(num53, num54, world, 0x3b, num52, false);
                                        if (world.getTile()[num53, num54].type == num52)
                                        {
                                            SquareTileFrame(num53, num54, world, true);
                                            flag8 = true;
                                        }
                                    }
                                }
                            }
                            if ((Statics.netMode == 2) && flag8)
                            {
                                NetMessage.SendTileSquare(-1, num29, num30, 3, world);
                            }
                        }
                        continue;
                    }
                    if (flag && (spawnNPC > 0))
                    {
                        SpawnNPC(num29, num30, world);
                    }
                }
            }
            if (!world.isDayTime())
            {
                float num55 = world.getMaxTilesX() / 0x1068;
                if (Statics.rand.Next(0x1f40) < (10f * num55))
                {
                    int num56 = 12;
                    int num57 = Statics.rand.Next(world.getMaxTilesX() - 50) + 100;
                    num57 *= 0x10;
                    int num58 = Statics.rand.Next((int)(world.getMaxTilesY() * 0.05)) * 0x10;
                    Vector2 vector = new Vector2((float)num57, (float)num58);
                    float speedX = Statics.rand.Next(-100, 0x65);
                    float speedY = Statics.rand.Next(200) + 100;
                    float num61 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                    num61 = ((float)num56) / num61;
                    speedX *= num61;
                    speedY *= num61;
                    Projectile.NewProjectile(vector.X, vector.Y, world, speedX, speedY, 12, 0x3e8, 10f, Statics.myPlayer);
                }
            }
        }

        public static void ScoreRoom(World world, int ignoreNPC = -1)
        {
            for (int i = 0; i < 0x3e8; i++)
            {
                if ((world.getNPCs()[i].active && world.getNPCs()[i].townNPC) && ((ignoreNPC != i) && !world.getNPCs()[i].homeless))
                {
                    for (int k = 0; k < numRoomTiles; k++)
                    {
                        if ((world.getNPCs()[i].homeTileX == roomX[k]) && (world.getNPCs()[i].homeTileY == roomY[k]))
                        {
                            bool flag = false;
                            for (int m = 0; m < numRoomTiles; m++)
                            {
                                if ((world.getNPCs()[i].homeTileX == roomX[m]) && ((world.getNPCs()[i].homeTileY - 1) == roomY[m]))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                hiScore = -1;
                                return;
                            }
                        }
                    }
                }
            }
            hiScore = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = ((roomX1 - ((Statics.screenWidth / 2) / 0x10)) - 1) - 0x15;
            int num8 = ((roomX2 + ((Statics.screenWidth / 2) / 0x10)) + 1) + 0x15;
            int num9 = ((roomY1 - ((Statics.screenHeight / 2) / 0x10)) - 1) - 0x15;
            int maxTilesX = ((roomY2 + ((Statics.screenHeight / 2) / 0x10)) + 1) + 0x15;
            if (num7 < 0)
            {
                num7 = 0;
            }
            if (num8 >= world.getMaxTilesX())
            {
                num8 = world.getMaxTilesX() - 1;
            }
            if (num9 < 0)
            {
                num9 = 0;
            }
            if (maxTilesX > world.getMaxTilesX())
            {
                maxTilesX = world.getMaxTilesX();
            }
            for (int j = num7 + 1; j < num8; j++)
            {
                for (int n = num9 + 2; n < (maxTilesX + 2); n++)
                {
                    if (world.getTile()[j, n].active)
                    {
                        if (((world.getTile()[j, n].type == 0x17) || (world.getTile()[j, n].type == 0x18)) || ((world.getTile()[j, n].type == 0x19) || (world.getTile()[j, n].type == 0x20)))
                        {
                            Statics.evilTiles++;
                        }
                        else if (world.getTile()[j, n].type == 0x1b)
                        {
                            Statics.evilTiles -= 5;
                        }
                    }
                }
            }
            if (num6 < 50)
            {
                num6 = 0;
            }
            num5 = -num6;
            if (num5 <= -250)
            {
                hiScore = num5;
            }
            else
            {
                num7 = roomX1;
                num8 = roomX2;
                num9 = roomY1;
                maxTilesX = roomY2;
                for (int num13 = num7 + 1; num13 < num8; num13++)
                {
                    for (int num14 = num9 + 2; num14 < (maxTilesX + 2); num14++)
                    {
                        if (!world.getTile()[num13, num14].active)
                        {
                            continue;
                        }
                        num4 = num5;
                        if (((Statics.tileSolid[world.getTile()[num13, num14].type] && !Statics.tileSolidTop[world.getTile()[num13, num14].type]) &&
                            (!Collision.SolidTiles(num13 - 1, num13 + 1, num14 - 3, num14 - 1, world) && world.getTile()[num13 - 1, num14].active)) &&
                            ((Statics.tileSolid[world.getTile()[num13 - 1, num14].type] && world.getTile()[num13 + 1, num14].active) &&
                            Statics.tileSolid[world.getTile()[num13 + 1, num14].type]))
                        {
                            for (int num15 = num13 - 2; num15 < (num13 + 3); num15++)
                            {
                                for (int num16 = num14 - 4; num16 < num14; num16++)
                                {
                                    if (world.getTile()[num15, num16].active)
                                    {
                                        if (num15 == num13)
                                        {
                                            num4 -= 15;
                                        }
                                        else if ((world.getTile()[num15, num16].type == 10) || (world.getTile()[num15, num16].type == 11))
                                        {
                                            num4 -= 20;
                                        }
                                        else if (Statics.tileSolid[world.getTile()[num15, num16].type])
                                        {
                                            num4 -= 5;
                                        }
                                        else
                                        {
                                            num4 += 5;
                                        }
                                    }
                                }
                            }
                            if (num4 > hiScore)
                            {
                                bool flag2 = false;
                                for (int num17 = 0; num17 < numRoomTiles; num17++)
                                {
                                    if ((roomX[num17] == num13) && (roomY[num17] == num14))
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                                if (flag2)
                                {
                                    hiScore = num4;
                                    bestX = num13;
                                    bestY = num14;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool RoomNeeds(int npcType)
        {
            if (((houseTile[15] && (houseTile[14] || houseTile[0x12])) && (((houseTile[4] || houseTile[0x21]) || (houseTile[0x22] || houseTile[0x23])) || ((houseTile[0x24] || houseTile[0x2a]) || houseTile[0x31]))) && ((houseTile[10] || houseTile[11]) || houseTile[0x13]))
            {
                canSpawn = true;
            }
            else
            {
                canSpawn = false;
            }
            return canSpawn;

        }

        public static void QuickFindHome(int npc, World world)
        {
            if (((world.getNPCs()[npc].homeTileX > 10) && (world.getNPCs()[npc].homeTileY > 10)) && ((world.getNPCs()[npc].homeTileX <
                (world.getMaxTilesX() - 10)) && (world.getNPCs()[npc].homeTileY < world.getMaxTilesY())))
            {
                canSpawn = false;
                StartRoomCheck(world.getNPCs()[npc].homeTileX, world.getNPCs()[npc].homeTileY - 1, world);
                if (!canSpawn)
                {
                    for (int i = world.getNPCs()[npc].homeTileX - 1; i < (world.getNPCs()[npc].homeTileX + 2); i++)
                    {
                        for (int j = world.getNPCs()[npc].homeTileY - 1; j < (world.getNPCs()[npc].homeTileY + 2); j++)
                        {
                            if (StartRoomCheck(i, j, world))
                            {
                                break;
                            }
                        }
                    }
                }
                if (!canSpawn)
                {
                    int num3 = 10;
                    for (int k = world.getNPCs()[npc].homeTileX - num3; k <= (world.getNPCs()[npc].homeTileX + num3); k += 2)
                    {
                        for (int m = world.getNPCs()[npc].homeTileY - num3; m <= (world.getNPCs()[npc].homeTileY + num3); m += 2)
                        {
                            if (StartRoomCheck(k, m, world))
                            {
                                break;
                            }
                        }
                    }
                }
                if (canSpawn)
                {
                    RoomNeeds(world.getNPCs()[npc].type);
                    if (canSpawn)
                    {
                        ScoreRoom(world, npc);
                    }
                    if (canSpawn && (hiScore > 0))
                    {
                        world.getNPCs()[npc].homeTileX = bestX;
                        world.getNPCs()[npc].homeTileY = bestY;
                        world.getNPCs()[npc].homeless = false;
                        canSpawn = false;
                    }
                    else
                    {
                        world.getNPCs()[npc].homeless = true;
                    }
                }
                else
                {
                    world.getNPCs()[npc].homeless = true;
                }
            }
        }

        public static void RangeFrame(int startX, int startY, int endX, int endY, World world)
        {
            int num = startX;
            int num2 = endX + 1;
            int num3 = startY;
            int num4 = endY + 1;
            for (int i = num - 1; i < (num2 + 1); i++)
            {
                for (int j = num3 - 1; j < (num4 + 1); j++)
                {
                    TileFrame(i, j, world, false, false);
                    WallFrame(i, j, world, false);
                }
            }
        }

        public static bool CloseDoor(int i, int j, World world, bool forced = false)
        {
            int num = 0;
            int num2 = i;
            int num3 = j;
            if (world.getTile()[i, j] == null)
            {
                world.getTile()[i, j] = new Tile();
            }
            int frameX = world.getTile()[i, j].frameX;
            int frameY = world.getTile()[i, j].frameY;
            switch (frameX)
            {
                case 0:
                    num2 = i;
                    num = 1;
                    break;

                case 0x12:
                    num2 = i - 1;
                    num = 1;
                    break;

                case 0x24:
                    num2 = i + 1;
                    num = -1;
                    break;

                case 0x36:
                    num2 = i;
                    num = -1;
                    break;
            }
            switch (frameY)
            {
                case 0:
                    num3 = j;
                    break;

                case 0x12:
                    num3 = j - 1;
                    break;

                case 0x24:
                    num3 = j - 2;
                    break;
            }
            int num6 = num2;
            if (num == -1)
            {
                num6 = num2 - 1;
            }
            if (!forced)
            {
                for (int n = num3; n < (num3 + 3); n++)
                {
                    if (!Collision.EmptyTile(num2, n, world, true))
                    {
                        return false;
                    }
                }
            }
            for (int k = num6; k < (num6 + 2); k++)
            {
                for (int num9 = num3; num9 < (num3 + 3); num9++)
                {
                    if (k == num2)
                    {
                        if (world.getTile()[k, num9] == null)
                        {
                            world.getTile()[k, num9] = new Tile();
                        }
                        world.getTile()[k, num9].type = 10;
                        world.getTile()[k, num9].frameX = (short)(genRand.Next(3) * 0x12);
                    }
                    else
                    {
                        if (world.getTile()[k, num9] == null)
                        {
                            world.getTile()[k, num9] = new Tile();
                        }
                        world.getTile()[k, num9].active = false;
                    }
                }
            }
            for (int m = num2 - 1; m <= (num2 + 1); m++)
            {
                for (int num11 = num3 - 1; num11 <= (num3 + 2); num11++)
                {
                    TileFrame(m, num11, world, false, false);
                }
            }
            //Main.PlaySound(9, i * 0x10, j * 0x10, 1);
            return true;
        }

        public static bool OpenDoor(int i, int j, int direction, World world)
        {
            int num3;
            int num = 0;
            if (world.getTile()[i, j - 1] == null)
            {
                world.getTile()[i, j - 1] = new Tile();
            }
            if (world.getTile()[i, j - 2] == null)
            {
                world.getTile()[i, j - 2] = new Tile();
            }
            if (world.getTile()[i, j + 1] == null)
            {
                world.getTile()[i, j + 1] = new Tile();
            }
            if (world.getTile()[i, j] == null)
            {
                world.getTile()[i, j] = new Tile();
            }
            if ((world.getTile()[i, j - 1].frameY == 0) && (world.getTile()[i, j - 1].type == world.getTile()[i, j].type))
            {
                num = j - 1;
            }
            else if ((world.getTile()[i, j - 2].frameY == 0) && (world.getTile()[i, j - 2].type == world.getTile()[i, j].type))
            {
                num = j - 2;
            }
            else if ((world.getTile()[i, j + 1].frameY == 0) && (world.getTile()[i, j + 1].type == world.getTile()[i, j].type))
            {
                num = j + 1;
            }
            else
            {
                num = j;
            }
            int num2 = i;
            short num4 = 0;
            if (direction == -1)
            {
                num2 = i - 1;
                num4 = 0x24;
                num3 = i - 1;
            }
            else
            {
                num2 = i;
                num3 = i + 1;
            }
            bool flag = true;
            for (int k = num; k < (num + 3); k++)
            {
                if (world.getTile()[num3, k] == null)
                {
                    world.getTile()[num3, k] = new Tile();
                }
                if (world.getTile()[num3, k].active)
                {
                    if ((((world.getTile()[num3, k].type == 3) || (world.getTile()[num3, k].type == 0x18)) || ((world.getTile()[num3, k].type == 0x34) || (world.getTile()[num3, k].type == 0x3d))) || (((world.getTile()[num3, k].type == 0x3e) || (world.getTile()[num3, k].type == 0x45)) || (((world.getTile()[num3, k].type == 0x47) || (world.getTile()[num3, k].type == 0x49)) || (world.getTile()[num3, k].type == 0x4a))))
                    {
                        KillTile(num3, k, world, false, false, false);
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag)
            {
                //Main.PlaySound(8, i * 0x10, j * 0x10, 1);
                world.getTile()[num2, num].active = true;
                world.getTile()[num2, num].type = 11;
                world.getTile()[num2, num].frameY = 0;
                world.getTile()[num2, num].frameX = num4;
                if (world.getTile()[num2 + 1, num] == null)
                {
                    world.getTile()[num2 + 1, num] = new Tile();
                }
                world.getTile()[num2 + 1, num].active = true;
                world.getTile()[num2 + 1, num].type = 11;
                world.getTile()[num2 + 1, num].frameY = 0;
                world.getTile()[num2 + 1, num].frameX = (short)(num4 + 0x12);
                if (world.getTile()[num2, num + 1] == null)
                {
                    world.getTile()[num2, num + 1] = new Tile();
                }
                world.getTile()[num2, num + 1].active = true;
                world.getTile()[num2, num + 1].type = 11;
                world.getTile()[num2, num + 1].frameY = 0x12;
                world.getTile()[num2, num + 1].frameX = num4;
                if (world.getTile()[num2 + 1, num + 1] == null)
                {
                    world.getTile()[num2 + 1, num + 1] = new Tile();
                }
                world.getTile()[num2 + 1, num + 1].active = true;
                world.getTile()[num2 + 1, num + 1].type = 11;
                world.getTile()[num2 + 1, num + 1].frameY = 0x12;
                world.getTile()[num2 + 1, num + 1].frameX = (short)(num4 + 0x12);
                if (world.getTile()[num2, num + 2] == null)
                {
                    world.getTile()[num2, num + 2] = new Tile();
                }
                world.getTile()[num2, num + 2].active = true;
                world.getTile()[num2, num + 2].type = 11;
                world.getTile()[num2, num + 2].frameY = 0x24;
                world.getTile()[num2, num + 2].frameX = num4;
                if (world.getTile()[num2 + 1, num + 2] == null)
                {
                    world.getTile()[num2 + 1, num + 2] = new Tile();
                }
                world.getTile()[num2 + 1, num + 2].active = true;
                world.getTile()[num2 + 1, num + 2].type = 11;
                world.getTile()[num2 + 1, num + 2].frameY = 0x24;
                world.getTile()[num2 + 1, num + 2].frameX = (short)(num4 + 0x12);
                for (int m = num2 - 1; m <= (num2 + 2); m++)
                {
                    for (int n = num - 1; n <= (num + 2); n++)
                    {
                        TileFrame(m, n, world, false, false);
                    }
                }
            }
            return flag;
        }

        public static void saveToonWhilePlaying(World world)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveToonWhilePlayingCallBack), world);
        }

        public static void saveToonWhilePlayingCallBack(object threadContext)
        {
            if (threadContext is World)
            {
                World world = (World)threadContext;
                Player.SavePlayer(world.getPlayerList()[Statics.myPlayer]);
            }
        }

        public static void SquareWallFrame(int i, int j, World world, bool resetFrame = true)
        {
            WallFrame(i - 1, j - 1, world, false);
            WallFrame(i - 1, j, world, false);
            WallFrame(i - 1, j + 1, world, false);
            WallFrame(i, j - 1, world, false);
            WallFrame(i, j, world, resetFrame);
            WallFrame(i, j + 1, world, false);
            WallFrame(i + 1, j - 1, world, false);
            WallFrame(i + 1, j, world, false);
            WallFrame(i + 1, j + 1, world, false);
        }

        public static void KillWall(int i, int j, World world, bool fail = false)
        {
            if (((i >= 0) && (j >= 0)) && ((i < world.getMaxTilesX()) && (j < world.getMaxTilesY())))
            {
                if (world.getTile()[i, j] == null)
                {
                    world.getTile()[i, j] = new Tile();
                }
                if (world.getTile()[i, j].wall > 0)
                {
                    genRand.Next(3);
                    //Main.PlaySound(0, i * 0x10, j * 0x10, 1);
                    int num = 10;
                    if (fail)
                    {
                        num = 3;
                    }
                    for (int k = 0; k < num; k++)
                    {
                        int type = 0;
                        if ((((world.getTile()[i, j].wall == 1) || (world.getTile()[i, j].wall == 5)) || ((world.getTile()[i, j].wall == 6) || (world.getTile()[i, j].wall == 7))) || ((world.getTile()[i, j].wall == 8) || (world.getTile()[i, j].wall == 9)))
                        {
                            type = 1;
                        }
                        if (world.getTile()[i, j].wall == 3)
                        {
                            if (genRand.Next(2) == 0)
                            {
                                type = 14;
                            }
                            else
                            {
                                type = 1;
                            }
                        }
                        if (world.getTile()[i, j].wall == 4)
                        {
                            type = 7;
                        }
                        if (world.getTile()[i, j].wall == 12)
                        {
                            type = 9;
                        }
                        if (world.getTile()[i, j].wall == 10)
                        {
                            type = 10;
                        }
                        if (world.getTile()[i, j].wall == 11)
                        {
                            type = 11;
                        }
                        Color newColor = new Color();
                        Dust.NewDust(new Vector2((float)(i * 0x10), (float)(j * 0x10)), world, 0x10, 0x10, type, 0f, 0f, 0, newColor, 1f);
                    }
                    if (fail)
                    {
                        SquareWallFrame(i, j, world, true);
                    }
                    else
                    {
                        int num4 = 0;
                        if (world.getTile()[i, j].wall == 1)
                        {
                            num4 = 0x1a;
                        }
                        if (world.getTile()[i, j].wall == 4)
                        {
                            num4 = 0x5d;
                        }
                        if (world.getTile()[i, j].wall == 5)
                        {
                            num4 = 130;
                        }
                        if (world.getTile()[i, j].wall == 6)
                        {
                            num4 = 0x84;
                        }
                        if (world.getTile()[i, j].wall == 7)
                        {
                            num4 = 0x87;
                        }
                        if (world.getTile()[i, j].wall == 8)
                        {
                            num4 = 0x8a;
                        }
                        if (world.getTile()[i, j].wall == 9)
                        {
                            num4 = 140;
                        }
                        if (world.getTile()[i, j].wall == 10)
                        {
                            num4 = 0x8e;
                        }
                        if (world.getTile()[i, j].wall == 11)
                        {
                            num4 = 0x90;
                        }
                        if (world.getTile()[i, j].wall == 12)
                        {
                            num4 = 0x92;
                        }
                        if (num4 > 0)
                        {
                            Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, num4, 1, false);
                        }
                        world.getTile()[i, j].wall = 0;
                        SquareWallFrame(i, j, world, true);
                    }
                }
            }
        }

        public static void PlaceWall(int i, int j, int type, World world, bool mute = false)
        {
            if (world.getTile()[i, j] == null)
            {
                world.getTile()[i, j] = new Tile();
            }
            if (world.getTile()[i, j].wall != type)
            {
                for (int k = i - 1; k < (i + 2); k++)
                {
                    for (int m = j - 1; m < (j + 2); m++)
                    {
                        if (world.getTile()[k, m] == null)
                        {
                            world.getTile()[k, m] = new Tile();
                        }
                        if ((world.getTile()[k, m].wall > 0) && (world.getTile()[k, m].wall != type))
                        {
                            return;
                        }
                    }
                }
                world.getTile()[i, j].wall = (byte)type;
                SquareWallFrame(i, j, world, true);
                if (!mute)
                {
                    //Main.PlaySound(0, i * 0x10, j * 0x10, 1);
                }
            }
        }

        public static void WallFrame(int i, int j, World world, bool resetFrame = false)
        {
            if ((((i >= 0) && (j >= 0)) && ((i < world.getMaxTilesX()) && (j < world.getMaxTilesY()))) && ((world.getTile()[i, j] != null) && (world.getTile()[i, j].wall > 0)))
            {
                int num = -1;
                int num2 = -1;
                int num3 = -1;
                int num4 = -1;
                int num5 = -1;
                int num6 = -1;
                int num7 = -1;
                int num8 = -1;
                int wall = world.getTile()[i, j].wall;
                if (wall != 0)
                {
                    Rectangle rectangle = new Rectangle();
                    byte wallFrameX = world.getTile()[i, j].wallFrameX;
                    byte wallFrameY = world.getTile()[i, j].wallFrameY;
                    rectangle.X = -1;
                    rectangle.Y = -1;
                    if ((i - 1) < 0)
                    {
                        num = wall;
                        num4 = wall;
                        num6 = wall;
                    }
                    if ((i + 1) >= world.getMaxTilesX())
                    {
                        num3 = wall;
                        num5 = wall;
                        num8 = wall;
                    }
                    if ((j - 1) < 0)
                    {
                        num = wall;
                        num2 = wall;
                        num3 = wall;
                    }
                    if ((j + 1) >= world.getMaxTilesY())
                    {
                        num6 = wall;
                        num7 = wall;
                        num8 = wall;
                    }
                    if (((i - 1) >= 0) && (world.getTile()[i - 1, j] != null))
                    {
                        num4 = world.getTile()[i - 1, j].wall;
                    }
                    if (((i + 1) < world.getMaxTilesX()) && (world.getTile()[i + 1, j] != null))
                    {
                        num5 = world.getTile()[i + 1, j].wall;
                    }
                    if (((j - 1) >= 0) && (world.getTile()[i, j - 1] != null))
                    {
                        num2 = world.getTile()[i, j - 1].wall;
                    }
                    if (((j + 1) < world.getMaxTilesY()) && (world.getTile()[i, j + 1] != null))
                    {
                        num7 = world.getTile()[i, j + 1].wall;
                    }
                    if ((((i - 1) >= 0) && ((j - 1) >= 0)) && (world.getTile()[i - 1, j - 1] != null))
                    {
                        num = world.getTile()[i - 1, j - 1].wall;
                    }
                    if ((((i + 1) < world.getMaxTilesX()) && ((j - 1) >= 0)) && (world.getTile()[i + 1, j - 1] != null))
                    {
                        num3 = world.getTile()[i + 1, j - 1].wall;
                    }
                    if ((((i - 1) >= 0) && ((j + 1) < world.getMaxTilesY())) && (world.getTile()[i - 1, j + 1] != null))
                    {
                        num6 = world.getTile()[i - 1, j + 1].wall;
                    }
                    if ((((i + 1) < world.getMaxTilesX()) && ((j + 1) < world.getMaxTilesY())) && (world.getTile()[i + 1, j + 1] != null))
                    {
                        num8 = world.getTile()[i + 1, j + 1].wall;
                    }
                    if (wall == 2)
                    {
                        if (j == ((int)world.getWorldSurface()))
                        {
                            num7 = wall;
                            num6 = wall;
                            num8 = wall;
                        }
                        else if (j >= ((int)world.getWorldSurface()))
                        {
                            num7 = wall;
                            num6 = wall;
                            num8 = wall;
                            num2 = wall;
                            num = wall;
                            num3 = wall;
                            num4 = wall;
                            num5 = wall;
                        }
                    }
                    int wallFrameNumber = 0;
                    if (resetFrame)
                    {
                        wallFrameNumber = genRand.Next(0, 3);
                        world.getTile()[i, j].wallFrameNumber = (byte)wallFrameNumber;
                    }
                    else
                    {
                        wallFrameNumber = world.getTile()[i, j].wallFrameNumber;
                    }
                    if ((rectangle.X < 0) || (rectangle.Y < 0))
                    {
                        if (((num2 == wall) && (num7 == wall)) && ((num4 == wall) & (num5 == wall)))
                        {
                            if ((num != wall) && (num3 != wall))
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 0x6c;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 1:
                                        rectangle.X = 0x7e;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 2:
                                        rectangle.X = 0x90;
                                        rectangle.Y = 0x12;
                                        break;
                                }
                            }
                            else if ((num6 != wall) && (num8 != wall))
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 0x6c;
                                        rectangle.Y = 0x24;
                                        break;

                                    case 1:
                                        rectangle.X = 0x7e;
                                        rectangle.Y = 0x24;
                                        break;

                                    case 2:
                                        rectangle.X = 0x90;
                                        rectangle.Y = 0x24;
                                        break;
                                }
                            }
                            else if ((num != wall) && (num6 != wall))
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 180;
                                        rectangle.Y = 0;
                                        break;

                                    case 1:
                                        rectangle.X = 180;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 2:
                                        rectangle.X = 180;
                                        rectangle.Y = 0x24;
                                        break;
                                }
                            }
                            else if ((num3 != wall) && (num8 != wall))
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 0xc6;
                                        rectangle.Y = 0;
                                        break;

                                    case 1:
                                        rectangle.X = 0xc6;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 2:
                                        rectangle.X = 0xc6;
                                        rectangle.Y = 0x24;
                                        break;
                                }
                            }
                            else
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 0x12;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 1:
                                        rectangle.X = 0x24;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 2:
                                        rectangle.X = 0x36;
                                        rectangle.Y = 0x12;
                                        break;
                                }
                            }
                        }
                        else if (((num2 != wall) && (num7 == wall)) && ((num4 == wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x12;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0x24;
                                    rectangle.Y = 0;
                                    break;

                                case 2:
                                    rectangle.X = 0x36;
                                    rectangle.Y = 0;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 != wall)) && ((num4 == wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x12;
                                    rectangle.Y = 0x24;
                                    break;

                                case 1:
                                    rectangle.X = 0x24;
                                    rectangle.Y = 0x24;
                                    break;

                                case 2:
                                    rectangle.X = 0x36;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 == wall)) && ((num4 != wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 0;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 == wall)) && ((num4 == wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 == wall)) && ((num4 != wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0;
                                    rectangle.Y = 0x36;
                                    break;

                                case 1:
                                    rectangle.X = 0x24;
                                    rectangle.Y = 0x36;
                                    break;

                                case 2:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0x36;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 == wall)) && ((num4 == wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x12;
                                    rectangle.Y = 0x36;
                                    break;

                                case 1:
                                    rectangle.X = 0x36;
                                    rectangle.Y = 0x36;
                                    break;

                                case 2:
                                    rectangle.X = 90;
                                    rectangle.Y = 0x36;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 != wall)) && ((num4 != wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0;
                                    rectangle.Y = 0x48;
                                    break;

                                case 1:
                                    rectangle.X = 0x24;
                                    rectangle.Y = 0x48;
                                    break;

                                case 2:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0x48;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 != wall)) && ((num4 == wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x12;
                                    rectangle.Y = 0x48;
                                    break;

                                case 1:
                                    rectangle.X = 0x36;
                                    rectangle.Y = 0x48;
                                    break;

                                case 2:
                                    rectangle.X = 90;
                                    rectangle.Y = 0x48;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 == wall)) && ((num4 != wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 90;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 90;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 90;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 != wall)) && ((num4 == wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x6c;
                                    rectangle.Y = 0x48;
                                    break;

                                case 1:
                                    rectangle.X = 0x7e;
                                    rectangle.Y = 0x48;
                                    break;

                                case 2:
                                    rectangle.X = 0x90;
                                    rectangle.Y = 0x48;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 == wall)) && ((num4 != wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x6c;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0x7e;
                                    rectangle.Y = 0;
                                    break;

                                case 2:
                                    rectangle.X = 0x90;
                                    rectangle.Y = 0;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 != wall)) && ((num4 != wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x6c;
                                    rectangle.Y = 0x36;
                                    break;

                                case 1:
                                    rectangle.X = 0x7e;
                                    rectangle.Y = 0x36;
                                    break;

                                case 2:
                                    rectangle.X = 0x90;
                                    rectangle.Y = 0x36;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 != wall)) && ((num4 != wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0xa2;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0xa2;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 0xa2;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 != wall)) && ((num4 == wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0xd8;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0xd8;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 0xd8;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 != wall)) && ((num4 != wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0xa2;
                                    rectangle.Y = 0x36;
                                    break;

                                case 1:
                                    rectangle.X = 180;
                                    rectangle.Y = 0x36;
                                    break;

                                case 2:
                                    rectangle.X = 0xc6;
                                    rectangle.Y = 0x36;
                                    break;
                            }
                        }
                    }
                    if ((rectangle.X <= -1) || (rectangle.Y <= -1))
                    {
                        if (wallFrameNumber <= 0)
                        {
                            rectangle.X = 0x12;
                            rectangle.Y = 0x12;
                        }
                        if (wallFrameNumber == 1)
                        {
                            rectangle.X = 0x24;
                            rectangle.Y = 0x12;
                        }
                        if (wallFrameNumber >= 2)
                        {
                            rectangle.X = 0x36;
                            rectangle.Y = 0x12;
                        }
                    }
                    world.getTile()[i, j].wallFrameX = (byte)rectangle.X;
                    world.getTile()[i, j].wallFrameY = (byte)rectangle.Y;
                }
            }
        }

        public static void WaterCheck(World world)
        {
            Liquid.numLiquid = 0;
            LiquidBuffer.numLiquidBuffer = 0;
            for (int i = 1; i < (world.getMaxTilesX() - 1); i++)
            {
                for (int j = world.getMaxTilesY() - 2; j > 0; j--)
                {
                    world.getTile()[i, j].checkingLiquid = false;
                    if (((world.getTile()[i, j].liquid > 0) && world.getTile()[i, j].active) && (Statics.tileSolid[world.getTile()[i, j].type] && !Statics.tileSolidTop[world.getTile()[i, j].type]))
                    {
                        world.getTile()[i, j].liquid = 0;
                    }
                    else if (world.getTile()[i, j].liquid > 0)
                    {
                        if (world.getTile()[i, j].active)
                        {
                            if (Statics.tileWaterDeath[world.getTile()[i, j].type])
                            {
                                KillTile(i, j, world, false, false, false);
                            }
                            if (world.getTile()[i, j].lava && Statics.tileLavaDeath[world.getTile()[i, j].type])
                            {
                                KillTile(i, j, world, false, false, false);
                            }
                        }
                        if (((!world.getTile()[i, j + 1].active || !Statics.tileSolid[world.getTile()[i, j + 1].type]) || Statics.tileSolidTop[world.getTile()[i, j + 1].type]) && (world.getTile()[i, j + 1].liquid < 0xff))
                        {
                            if (world.getTile()[i, j + 1].liquid > 250)
                            {
                                world.getTile()[i, j + 1].liquid = 0xff;
                            }
                            else
                            {
                                Liquid.AddWater(i, j, world);
                            }
                        }
                        if (((!world.getTile()[i - 1, j].active || !Statics.tileSolid[world.getTile()[i - 1, j].type]) || Statics.tileSolidTop[world.getTile()[i - 1, j].type]) && (world.getTile()[i - 1, j].liquid != world.getTile()[i, j].liquid))
                        {
                            Liquid.AddWater(i, j, world);
                        }
                        else if (((!world.getTile()[i + 1, j].active || !Statics.tileSolid[world.getTile()[i + 1, j].type]) || Statics.tileSolidTop[world.getTile()[i + 1, j].type]) && (world.getTile()[i + 1, j].liquid != world.getTile()[i, j].liquid))
                        {
                            Liquid.AddWater(i, j, world);
                        }
                        if (world.getTile()[i, j].lava)
                        {
                            if ((world.getTile()[i - 1, j].liquid > 0) && !world.getTile()[i - 1, j].lava)
                            {
                                Liquid.AddWater(i, j, world);
                            }
                            else if ((world.getTile()[i + 1, j].liquid > 0) && !world.getTile()[i + 1, j].lava)
                            {
                                Liquid.AddWater(i, j, world);
                            }
                            else if ((world.getTile()[i, j - 1].liquid > 0) && !world.getTile()[i, j - 1].lava)
                            {
                                Liquid.AddWater(i, j, world);
                            }
                            else if ((world.getTile()[i, j + 1].liquid > 0) && !world.getTile()[i, j + 1].lava)
                            {
                                Liquid.AddWater(i, j, world);
                            }
                        }
                    }
                }
            }
        }

        public static void PlantCheck(int i, int j, World world)
        {
            int num = -1;
            int type = world.getTile()[i, j].type;
            int num1 = i - 1;
            int maxTilesX = world.getMaxTilesX();
            int num4 = i + 1;
            int num5 = j - 1;
            if ((j + 1) >= world.getMaxTilesY())
            {
                num = type;
            }
            if ((((i - 1) >= 0) && (world.getTile()[i - 1, j] != null)) && world.getTile()[i - 1, j].active)
            {
                byte num6 = world.getTile()[i - 1, j].type;
            }
            if ((((i + 1) < world.getMaxTilesX()) && (world.getTile()[i + 1, j] != null)) && world.getTile()[i + 1, j].active)
            {
                byte num7 = world.getTile()[i + 1, j].type;
            }
            if ((((j - 1) >= 0) && (world.getTile()[i, j - 1] != null)) && world.getTile()[i, j - 1].active)
            {
                byte num8 = world.getTile()[i, j - 1].type;
            }
            if ((((j + 1) < world.getMaxTilesY()) && (world.getTile()[i, j + 1] != null)) && world.getTile()[i, j + 1].active)
            {
                num = world.getTile()[i, j + 1].type;
            }
            if ((((i - 1) >= 0) && ((j - 1) >= 0)) && ((world.getTile()[i - 1, j - 1] != null) && world.getTile()[i - 1, j - 1].active))
            {
                byte num9 = world.getTile()[i - 1, j - 1].type;
            }
            if ((((i + 1) < world.getMaxTilesX()) && ((j - 1) >= 0)) && ((world.getTile()[i + 1, j - 1] != null) && world.getTile()[i + 1, j - 1].active))
            {
                byte num10 = world.getTile()[i + 1, j - 1].type;
            }
            if ((((i - 1) >= 0) && ((j + 1) < world.getMaxTilesY())) && ((world.getTile()[i - 1, j + 1] != null) && world.getTile()[i - 1, j + 1].active))
            {
                byte num11 = world.getTile()[i - 1, j + 1].type;
            }
            if ((((i + 1) < world.getMaxTilesX()) && ((j + 1) < world.getMaxTilesY())) && ((world.getTile()[i + 1, j + 1] != null) && world.getTile()[i + 1, j + 1].active))
            {
                byte num12 = world.getTile()[i + 1, j + 1].type;
            }
            if ((((((type == 3) && (num != 2)) && (num != 0x4e)) || ((type == 0x18) && (num != 0x17))) || (((type == 0x3d) && (num != 60)) || ((type == 0x47) && (num != 70)))) || ((((type == 0x49) && (num != 2)) && (num != 0x4e)) || ((type == 0x4a) && (num != 60))))
            {
                KillTile(i, j, world, false, false, false);
            }
        }

        public static void Place1x2(int x, int y, int type, World world)
        {
            short num = 0;
            if (type == 20)
            {
                num = (short)(genRand.Next(3) * 0x12);
            }
            if (world.getTile()[x, y - 1] == null)
            {
                world.getTile()[x, y - 1] = new Tile();
            }
            if (world.getTile()[x, y + 1] == null)
            {
                world.getTile()[x, y + 1] = new Tile();
            }
            if ((world.getTile()[x, y + 1].active && Statics.tileSolid[world.getTile()[x, y + 1].type]) && !world.getTile()[x, y - 1].active)
            {
                world.getTile()[x, y - 1].active = true;
                world.getTile()[x, y - 1].frameY = 0;
                world.getTile()[x, y - 1].frameX = num;
                world.getTile()[x, y - 1].type = (byte)type;
                world.getTile()[x, y].active = true;
                world.getTile()[x, y].frameY = 0x12;
                world.getTile()[x, y].frameX = num;
                world.getTile()[x, y].type = (byte)type;
            }
        }

        public static void Place1x2Top(int x, int y, int type, World world)
        {
            short num = 0;
            if (world.getTile()[x, y - 1] == null)
            {
                world.getTile()[x, y - 1] = new Tile();
            }
            if (world.getTile()[x, y + 1] == null)
            {
                world.getTile()[x, y + 1] = new Tile();
            }
            if ((world.getTile()[x, y - 1].active && Statics.tileSolid[world.getTile()[x, y - 1].type]) && (!Statics.tileSolidTop[world.getTile()[x, y - 1].type] && !world.getTile()[x, y + 1].active))
            {
                world.getTile()[x, y].active = true;
                world.getTile()[x, y].frameY = 0;
                world.getTile()[x, y].frameX = num;
                world.getTile()[x, y].type = (byte)type;
                world.getTile()[x, y + 1].active = true;
                world.getTile()[x, y + 1].frameY = 0x12;
                world.getTile()[x, y + 1].frameX = num;
                world.getTile()[x, y + 1].type = (byte)type;
            }
        }

        public static void Place2x1(int x, int y, int type, World world)
        {
            if (world.getTile()[x, y] == null)
            {
                world.getTile()[x, y] = new Tile();
            }
            if (world.getTile()[x + 1, y] == null)
            {
                world.getTile()[x + 1, y] = new Tile();
            }
            if (world.getTile()[x, y + 1] == null)
            {
                world.getTile()[x, y + 1] = new Tile();
            }
            if (world.getTile()[x + 1, y + 1] == null)
            {
                world.getTile()[x + 1, y + 1] = new Tile();
            }
            bool flag = false;
            if ((((type != 0x1d) && world.getTile()[x, y + 1].active) && (world.getTile()[x + 1, y + 1].active && Statics.tileSolid[world.getTile()[x, y + 1].type])) && ((Statics.tileSolid[world.getTile()[x + 1, y + 1].type] && !world.getTile()[x, y].active) && !world.getTile()[x + 1, y].active))
            {
                flag = true;
            }
            else if ((((type == 0x1d) && world.getTile()[x, y + 1].active) && (world.getTile()[x + 1, y + 1].active && Statics.tileTable[world.getTile()[x, y + 1].type])) && ((Statics.tileTable[world.getTile()[x + 1, y + 1].type] && !world.getTile()[x, y].active) && !world.getTile()[x + 1, y].active))
            {
                flag = true;
            }
            if (flag)
            {
                world.getTile()[x, y].active = true;
                world.getTile()[x, y].frameY = 0;
                world.getTile()[x, y].frameX = 0;
                world.getTile()[x, y].type = (byte)type;
                world.getTile()[x + 1, y].active = true;
                world.getTile()[x + 1, y].frameY = 0;
                world.getTile()[x + 1, y].frameX = 0x12;
                world.getTile()[x + 1, y].type = (byte)type;
            }
        }

        public static void Place3x2(int x, int y, int type, World world)
        {
            if (((x >= 5) && (x <= (world.getMaxTilesX() - 5))) && ((y >= 5) && (y <= (world.getMaxTilesY() - 5))))
            {
                bool flag = true;
                for (int i = x - 1; i < (x + 2); i++)
                {
                    for (int j = y - 1; j < (y + 1); j++)
                    {
                        if (world.getTile()[i, j] == null)
                        {
                            world.getTile()[i, j] = new Tile();
                        }
                        if (world.getTile()[i, j].active)
                        {
                            flag = false;
                        }
                    }
                    if (world.getTile()[i, y + 1] == null)
                    {
                        world.getTile()[i, y + 1] = new Tile();
                    }
                    if (!world.getTile()[i, y + 1].active || !Statics.tileSolid[world.getTile()[i, y + 1].type])
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    world.getTile()[x - 1, y - 1].active = true;
                    world.getTile()[x - 1, y - 1].frameY = 0;
                    world.getTile()[x - 1, y - 1].frameX = 0;
                    world.getTile()[x - 1, y - 1].type = (byte)type;
                    world.getTile()[x, y - 1].active = true;
                    world.getTile()[x, y - 1].frameY = 0;
                    world.getTile()[x, y - 1].frameX = 0x12;
                    world.getTile()[x, y - 1].type = (byte)type;
                    world.getTile()[x + 1, y - 1].active = true;
                    world.getTile()[x + 1, y - 1].frameY = 0;
                    world.getTile()[x + 1, y - 1].frameX = 0x24;
                    world.getTile()[x + 1, y - 1].type = (byte)type;
                    world.getTile()[x - 1, y].active = true;
                    world.getTile()[x - 1, y].frameY = 0x12;
                    world.getTile()[x - 1, y].frameX = 0;
                    world.getTile()[x - 1, y].type = (byte)type;
                    world.getTile()[x, y].active = true;
                    world.getTile()[x, y].frameY = 0x12;
                    world.getTile()[x, y].frameX = 0x12;
                    world.getTile()[x, y].type = (byte)type;
                    world.getTile()[x + 1, y].active = true;
                    world.getTile()[x + 1, y].frameY = 0x12;
                    world.getTile()[x + 1, y].frameX = 0x24;
                    world.getTile()[x + 1, y].type = (byte)type;
                }
            }
        }

        public static int PlaceChest(int x, int y, World world, int type = 0x15)
        {
            bool flag = true;
            int num = -1;
            for (int i = x; i < (x + 2); i++)
            {
                for (int j = y - 1; j < (y + 1); j++)
                {
                    if (world.getTile()[i, j] == null)
                    {
                        world.getTile()[i, j] = new Tile();
                    }
                    if (world.getTile()[i, j].active)
                    {
                        flag = false;
                    }
                    if (world.getTile()[i, j].lava)
                    {
                        flag = false;
                    }
                }
                if (world.getTile()[i, y + 1] == null)
                {
                    world.getTile()[i, y + 1] = new Tile();
                }
                if (!world.getTile()[i, y + 1].active || !Statics.tileSolid[world.getTile()[i, y + 1].type])
                {
                    flag = false;
                }
            }
            if (flag)
            {
                num = Chest.CreateChest(x, y - 1, world);
                if (num == -1)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                world.getTile()[x, y - 1].active = true;
                world.getTile()[x, y - 1].frameY = 0;
                world.getTile()[x, y - 1].frameX = 0;
                world.getTile()[x, y - 1].type = (byte)type;
                world.getTile()[x + 1, y - 1].active = true;
                world.getTile()[x + 1, y - 1].frameY = 0;
                world.getTile()[x + 1, y - 1].frameX = 0x12;
                world.getTile()[x + 1, y - 1].type = (byte)type;
                world.getTile()[x, y].active = true;
                world.getTile()[x, y].frameY = 0x12;
                world.getTile()[x, y].frameX = 0;
                world.getTile()[x, y].type = (byte)type;
                world.getTile()[x + 1, y].active = true;
                world.getTile()[x + 1, y].frameY = 0x12;
                world.getTile()[x + 1, y].frameX = 0x12;
                world.getTile()[x + 1, y].type = (byte)type;
            }
            return num;
        }

        public static bool PlacePot(int x, int y, World world, int type = 0x1c)
        {
            bool flag = true;
            for (int i = x; i < (x + 2); i++)
            {
                for (int k = y - 1; k < (y + 1); k++)
                {
                    if (world.getTile()[i, k] == null)
                    {
                        world.getTile()[i, k] = new Tile();
                    }
                    if (world.getTile()[i, k].active)
                    {
                        flag = false;
                    }
                }
                if (world.getTile()[i, y + 1] == null)
                {
                    world.getTile()[i, y + 1] = new Tile();
                }
                if (!world.getTile()[i, y + 1].active || !Statics.tileSolid[world.getTile()[i, y + 1].type])
                {
                    flag = false;
                }
            }
            if (!flag)
            {
                return false;
            }
            for (int j = 0; j < 2; j++)
            {
                for (int m = -1; m < 1; m++)
                {
                    int num5 = (j * 0x12) + (genRand.Next(3) * 0x24);
                    int num6 = (m + 1) * 0x12;
                    world.getTile()[x + j, y + m].active = true;
                    world.getTile()[x + j, y + m].frameX = (short)num5;
                    world.getTile()[x + j, y + m].frameY = (short)num6;
                    world.getTile()[x + j, y + m].type = (byte)type;
                }
            }
            return true;
        }

        public static bool PlaceSign(int x, int y, int type, World world)
        {
            int num7;
            int num8;
            int num9;
            int num = x - 2;
            int num2 = x + 3;
            int num3 = y - 2;
            int num4 = y + 3;
            if (num >= 0)
            {
                if (num2 > world.getMaxTilesX())
                {
                    return false;
                }
                if (num3 < 0)
                {
                    return false;
                }
                if (num4 > world.getMaxTilesY())
                {
                    return false;
                }
                for (int j = num; j < num2; j++)
                {
                    for (int k = num3; k < num4; k++)
                    {
                        if (world.getTile()[j, k] == null)
                        {
                            world.getTile()[j, k] = new Tile();
                        }
                    }
                }
                num7 = x;
                num8 = y;
                num9 = 0;
                if ((world.getTile()[x, y + 1].active && Statics.tileSolid[world.getTile()[x, y + 1].type]) && (world.getTile()[x + 1, y + 1].active && Statics.tileSolid[world.getTile()[x + 1, y + 1].type]))
                {
                    num8--;
                    num9 = 0;
                    goto Label_02E8;
                }
                if (((world.getTile()[x, y - 1].active && Statics.tileSolid[world.getTile()[x, y - 1].type]) && (!Statics.tileSolidTop[world.getTile()[x, y - 1].type] && world.getTile()[x + 1, y - 1].active)) && (Statics.tileSolid[world.getTile()[x + 1, y - 1].type] && !Statics.tileSolidTop[world.getTile()[x + 1, y - 1].type]))
                {
                    num9 = 1;
                    goto Label_02E8;
                }
                if (((world.getTile()[x - 1, y].active && Statics.tileSolid[world.getTile()[x - 1, y].type]) && (!Statics.tileSolidTop[world.getTile()[x - 1, y].type] && world.getTile()[x - 1, y + 1].active)) && (Statics.tileSolid[world.getTile()[x - 1, y + 1].type] && !Statics.tileSolidTop[world.getTile()[x - 1, y + 1].type]))
                {
                    num9 = 2;
                    goto Label_02E8;
                }
                if (((world.getTile()[x + 1, y].active && Statics.tileSolid[world.getTile()[x + 1, y].type]) && (!Statics.tileSolidTop[world.getTile()[x + 1, y].type] && world.getTile()[x + 1, y + 1].active)) && (Statics.tileSolid[world.getTile()[x + 1, y + 1].type] && !Statics.tileSolidTop[world.getTile()[x + 1, y + 1].type]))
                {
                    num7--;
                    num9 = 3;
                    goto Label_02E8;
                }
            }
            return false;
        Label_02E8:
            if ((world.getTile()[num7, num8].active || world.getTile()[num7 + 1, num8].active) || (world.getTile()[num7, num8 + 1].active || world.getTile()[num7 + 1, num8 + 1].active))
            {
                return false;
            }
            int num10 = 0x24 * num9;
            for (int i = 0; i < 2; i++)
            {
                for (int m = 0; m < 2; m++)
                {
                    world.getTile()[num7 + i, num8 + m].active = true;
                    world.getTile()[num7 + i, num8 + m].type = (byte)type;
                    world.getTile()[num7 + i, num8 + m].frameX = (short)(num10 + (0x12 * i));
                    world.getTile()[num7 + i, num8 + m].frameY = (short)(0x12 * m);
                }
            }
            return true;
        }

        public static void PlaceSunflower(int x, int y, World world, int type = 0x1b)
        {
            if (y <= (world.getWorldSurface() - 1.0))
            {
                bool flag = true;
                for (int i = x; i < (x + 2); i++)
                {
                    for (int j = y - 3; j < (y + 1); j++)
                    {
                        if (world.getTile()[i, j] == null)
                        {
                            world.getTile()[i, j] = new Tile();
                        }
                        if (world.getTile()[i, j].active || (world.getTile()[i, j].wall > 0))
                        {
                            flag = false;
                        }
                    }
                    if (world.getTile()[i, y + 1] == null)
                    {
                        world.getTile()[i, y + 1] = new Tile();
                    }
                    if (!world.getTile()[i, y + 1].active || (world.getTile()[i, y + 1].type != 2))
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        for (int m = -3; m < 1; m++)
                        {
                            int num5 = (k * 0x12) + (genRand.Next(3) * 0x24);
                            int num6 = (m + 3) * 0x12;
                            world.getTile()[x + k, y + m].active = true;
                            world.getTile()[x + k, y + m].frameX = (short)num5;
                            world.getTile()[x + k, y + m].frameY = (short)num6;
                            world.getTile()[x + k, y + m].type = (byte)type;
                        }
                    }
                }
            }
        }

        public static void PlaceOnTable1x1(int x, int y, int type, World world)
        {
            bool flag = false;
            if (world.getTile()[x, y] == null)
            {
                world.getTile()[x, y] = new Tile();
            }
            if (world.getTile()[x, y + 1] == null)
            {
                world.getTile()[x, y + 1] = new Tile();
            }
            if ((!world.getTile()[x, y].active && world.getTile()[x, y + 1].active) && Statics.tileTable[world.getTile()[x, y + 1].type])
            {
                flag = true;
            }
            if (((type == 0x4e) && !world.getTile()[x, y].active) && (world.getTile()[x, y + 1].active && Statics.tileSolid[world.getTile()[x, y + 1].type]))
            {
                flag = true;
            }
            if (flag)
            {
                world.getTile()[x, y].active = true;
                world.getTile()[x, y].frameX = 0;
                world.getTile()[x, y].frameY = 0;
                world.getTile()[x, y].type = (byte)type;
                if (type == 50)
                {
                    world.getTile()[x, y].frameX = (short)(0x12 * genRand.Next(5));
                }
            }
        }

        public static void Place3x3(int x, int y, int type, World world)
        {
            bool flag = true;
            for (int i = x - 1; i < (x + 2); i++)
            {
                for (int j = y; j < (y + 3); j++)
                {
                    if (world.getTile()[i, j] == null)
                    {
                        world.getTile()[i, j] = new Tile();
                    }
                    if (world.getTile()[i, j].active)
                    {
                        flag = false;
                    }
                }
            }
            if (world.getTile()[x, y - 1] == null)
            {
                world.getTile()[x, y - 1] = new Tile();
            }
            if ((!world.getTile()[x, y - 1].active || !Statics.tileSolid[world.getTile()[x, y - 1].type]) || Statics.tileSolidTop[world.getTile()[x, y - 1].type])
            {
                flag = false;
            }
            if (flag)
            {
                world.getTile()[x - 1, y].active = true;
                world.getTile()[x - 1, y].frameY = 0;
                world.getTile()[x - 1, y].frameX = 0;
                world.getTile()[x - 1, y].type = (byte)type;
                world.getTile()[x, y].active = true;
                world.getTile()[x, y].frameY = 0;
                world.getTile()[x, y].frameX = 0x12;
                world.getTile()[x, y].type = (byte)type;
                world.getTile()[x + 1, y].active = true;
                world.getTile()[x + 1, y].frameY = 0;
                world.getTile()[x + 1, y].frameX = 0x24;
                world.getTile()[x + 1, y].type = (byte)type;
                world.getTile()[x - 1, y + 1].active = true;
                world.getTile()[x - 1, y + 1].frameY = 0x12;
                world.getTile()[x - 1, y + 1].frameX = 0;
                world.getTile()[x - 1, y + 1].type = (byte)type;
                world.getTile()[x, y + 1].active = true;
                world.getTile()[x, y + 1].frameY = 0x12;
                world.getTile()[x, y + 1].frameX = 0x12;
                world.getTile()[x, y + 1].type = (byte)type;
                world.getTile()[x + 1, y + 1].active = true;
                world.getTile()[x + 1, y + 1].frameY = 0x12;
                world.getTile()[x + 1, y + 1].frameX = 0x24;
                world.getTile()[x + 1, y + 1].type = (byte)type;
                world.getTile()[x - 1, y + 2].active = true;
                world.getTile()[x - 1, y + 2].frameY = 0x24;
                world.getTile()[x - 1, y + 2].frameX = 0;
                world.getTile()[x - 1, y + 2].type = (byte)type;
                world.getTile()[x, y + 2].active = true;
                world.getTile()[x, y + 2].frameY = 0x24;
                world.getTile()[x, y + 2].frameX = 0x12;
                world.getTile()[x, y + 2].type = (byte)type;
                world.getTile()[x + 1, y + 2].active = true;
                world.getTile()[x + 1, y + 2].frameY = 0x24;
                world.getTile()[x + 1, y + 2].frameX = 0x24;
                world.getTile()[x + 1, y + 2].type = (byte)type;
            }
        }

        public static void Place4x2(int x, int y, int type, World world, int direction = -1)
        {
            if (((x >= 5) && (x <= (world.getMaxTilesX() - 5))) && ((y >= 5) && (y <= (world.getMaxTilesY() - 5))))
            {
                bool flag = true;
                for (int i = x - 1; i < (x + 3); i++)
                {
                    for (int j = y - 1; j < (y + 1); j++)
                    {
                        if (world.getTile()[i, j] == null)
                        {
                            world.getTile()[i, j] = new Tile();
                        }
                        if (world.getTile()[i, j].active)
                        {
                            flag = false;
                        }
                    }
                    if (world.getTile()[i, y + 1] == null)
                    {
                        world.getTile()[i, y + 1] = new Tile();
                    }
                    if (!world.getTile()[i, y + 1].active || !Statics.tileSolid[world.getTile()[i, y + 1].type])
                    {
                        flag = false;
                    }
                }
                short num3 = 0;
                if (direction == 1)
                {
                    num3 = 0x48;
                }
                if (flag)
                {
                    world.getTile()[x - 1, y - 1].active = true;
                    world.getTile()[x - 1, y - 1].frameY = 0;
                    world.getTile()[x - 1, y - 1].frameX = num3;
                    world.getTile()[x - 1, y - 1].type = (byte)type;
                    world.getTile()[x, y - 1].active = true;
                    world.getTile()[x, y - 1].frameY = 0;
                    world.getTile()[x, y - 1].frameX = (short)(0x12 + num3);
                    world.getTile()[x, y - 1].type = (byte)type;
                    world.getTile()[x + 1, y - 1].active = true;
                    world.getTile()[x + 1, y - 1].frameY = 0;
                    world.getTile()[x + 1, y - 1].frameX = (short)(0x24 + num3);
                    world.getTile()[x + 1, y - 1].type = (byte)type;
                    world.getTile()[x + 2, y - 1].active = true;
                    world.getTile()[x + 2, y - 1].frameY = 0;
                    world.getTile()[x + 2, y - 1].frameX = (short)(0x36 + num3);
                    world.getTile()[x + 2, y - 1].type = (byte)type;
                    world.getTile()[x - 1, y].active = true;
                    world.getTile()[x - 1, y].frameY = 0x12;
                    world.getTile()[x - 1, y].frameX = num3;
                    world.getTile()[x - 1, y].type = (byte)type;
                    world.getTile()[x, y].active = true;
                    world.getTile()[x, y].frameY = 0x12;
                    world.getTile()[x, y].frameX = (short)(0x12 + num3);
                    world.getTile()[x, y].type = (byte)type;
                    world.getTile()[x + 1, y].active = true;
                    world.getTile()[x + 1, y].frameY = 0x12;
                    world.getTile()[x + 1, y].frameX = (short)(0x24 + num3);
                    world.getTile()[x + 1, y].type = (byte)type;
                    world.getTile()[x + 2, y].active = true;
                    world.getTile()[x + 2, y].frameY = 0x12;
                    world.getTile()[x + 2, y].frameX = (short)(0x36 + num3);
                    world.getTile()[x + 2, y].type = (byte)type;
                }
            }
        }

        public static bool PlaceDoor(int i, int j, int type, World world)
        {
            try
            {
                if ((world.getTile()[i, j - 2].active && Statics.tileSolid[world.getTile()[i, j - 2].type]) && (world.getTile()[i, j + 2].active && Statics.tileSolid[world.getTile()[i, j + 2].type]))
                {
                    world.getTile()[i, j - 1].active = true;
                    world.getTile()[i, j - 1].type = 10;
                    world.getTile()[i, j - 1].frameY = 0;
                    world.getTile()[i, j - 1].frameX = (short)(genRand.Next(3) * 0x12);
                    world.getTile()[i, j].active = true;
                    world.getTile()[i, j].type = 10;
                    world.getTile()[i, j].frameY = 0x12;
                    world.getTile()[i, j].frameX = (short)(genRand.Next(3) * 0x12);
                    world.getTile()[i, j + 1].active = true;
                    world.getTile()[i, j + 1].type = 10;
                    world.getTile()[i, j + 1].frameY = 0x24;
                    world.getTile()[i, j + 1].frameX = (short)(genRand.Next(3) * 0x12);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool PlaceTile(int i, int j, World world, int type, bool mute = false, bool forced = false, int plr = -1)
        {
            bool flag = false;
            if (((i >= 0) && (j >= 0)) && ((i < world.getMaxTilesX()) && (j < world.getMaxTilesY())))
            {
                if (world.getTile()[i, j] == null)
                {
                    world.getTile()[i, j] = new Tile();
                }
                if (((((!forced && !Collision.EmptyTile(i, j, world, false)) && Statics.tileSolid[type]) && (((type != 0x17) || (world.getTile()[i, j].type != 0)) || !world.getTile()[i, j].active)) && (((type != 2) || (world.getTile()[i, j].type != 0)) || !world.getTile()[i, j].active)) && ((((type != 60) || (world.getTile()[i, j].type != 0x3b)) || !world.getTile()[i, j].active) && (((type != 70) || (world.getTile()[i, j].type != 0x3b)) || !world.getTile()[i, j].active)))
                {
                    return flag;
                }
                if ((type == 0x17) && ((world.getTile()[i, j].type != 0) || !world.getTile()[i, j].active))
                {
                    return false;
                }
                if ((type == 2) && ((world.getTile()[i, j].type != 0) || !world.getTile()[i, j].active))
                {
                    return false;
                }
                if ((type == 60) && ((world.getTile()[i, j].type != 0x3b) || !world.getTile()[i, j].active))
                {
                    return false;
                }
                if ((world.getTile()[i, j].liquid > 0) && ((((type == 3) || (type == 4)) || ((type == 20) || (type == 0x18))) || ((((type == 0x1b) || (type == 0x20)) || ((type == 0x33) || (type == 0x3d))) || (((type == 0x45) || (type == 0x48)) || (type == 0x49)))))
                {
                    return false;
                }
                world.getTile()[i, j].frameY = 0;
                world.getTile()[i, j].frameX = 0;
                if ((type == 3) || (type == 0x18))
                {
                    if ((((j + 1) < world.getMaxTilesY()) && world.getTile()[i, j + 1].active) && ((((world.getTile()[i, j + 1].type == 2) && (type == 3)) || ((world.getTile()[i, j + 1].type == 0x17) && (type == 0x18))) || ((world.getTile()[i, j + 1].type == 0x4e) && (type == 3))))
                    {
                        if ((type == 0x18) && (genRand.Next(13) == 0))
                        {
                            world.getTile()[i, j].active = true;
                            world.getTile()[i, j].type = 0x20;
                            SquareTileFrame(i, j, world, true);
                        }
                        else if (world.getTile()[i, j + 1].type == 0x4e)
                        {
                            world.getTile()[i, j].active = true;
                            world.getTile()[i, j].type = (byte)type;
                            world.getTile()[i, j].frameX = (short)((genRand.Next(2) * 0x12) + 0x6c);
                        }
                        else if ((world.getTile()[i, j].wall == 0) && (world.getTile()[i, j + 1].wall == 0))
                        {
                            if (genRand.Next(50) == 0)
                            {
                                world.getTile()[i, j].active = true;
                                world.getTile()[i, j].type = (byte)type;
                                world.getTile()[i, j].frameX = 0x90;
                            }
                            else if (genRand.Next(0x23) == 0)
                            {
                                world.getTile()[i, j].active = true;
                                world.getTile()[i, j].type = (byte)type;
                                world.getTile()[i, j].frameX = (short)((genRand.Next(2) * 0x12) + 0x6c);
                            }
                            else
                            {
                                world.getTile()[i, j].active = true;
                                world.getTile()[i, j].type = (byte)type;
                                world.getTile()[i, j].frameX = (short)(genRand.Next(6) * 0x12);
                            }
                        }
                    }
                }
                else if (type == 0x3d)
                {
                    if ((((j + 1) < world.getMaxTilesY()) && world.getTile()[i, j + 1].active) && (world.getTile()[i, j + 1].type == 60))
                    {
                        if (genRand.Next(10) == 0)
                        {
                            world.getTile()[i, j].active = true;
                            world.getTile()[i, j].type = 0x45;
                            SquareTileFrame(i, j, world, true);
                        }
                        else if (genRand.Next(15) == 0)
                        {
                            world.getTile()[i, j].active = true;
                            world.getTile()[i, j].type = (byte)type;
                            world.getTile()[i, j].frameX = 0x90;
                        }
                        else if (genRand.Next(0x3e8) == 0)
                        {
                            world.getTile()[i, j].active = true;
                            world.getTile()[i, j].type = (byte)type;
                            world.getTile()[i, j].frameX = 0xa2;
                        }
                        else
                        {
                            world.getTile()[i, j].active = true;
                            world.getTile()[i, j].type = (byte)type;
                            world.getTile()[i, j].frameX = (short)(genRand.Next(8) * 0x12);
                        }
                    }
                }
                else if (type == 0x47)
                {
                    if ((((j + 1) < world.getMaxTilesY()) && world.getTile()[i, j + 1].active) && (world.getTile()[i, j + 1].type == 70))
                    {
                        world.getTile()[i, j].active = true;
                        world.getTile()[i, j].type = (byte)type;
                        world.getTile()[i, j].frameX = (short)(genRand.Next(5) * 0x12);
                    }
                }
                else if (type == 4)
                {
                    if (world.getTile()[i - 1, j] == null)
                    {
                        world.getTile()[i - 1, j] = new Tile();
                    }
                    if (world.getTile()[i + 1, j] == null)
                    {
                        world.getTile()[i + 1, j] = new Tile();
                    }
                    if (world.getTile()[i, j + 1] == null)
                    {
                        world.getTile()[i, j + 1] = new Tile();
                    }
                    if (((world.getTile()[i - 1, j].active && (Statics.tileSolid[world.getTile()[i - 1, j].type] || (((world.getTile()[i - 1, j].type == 5) && (world.getTile()[i - 1, j - 1].type == 5)) && (world.getTile()[i - 1, j + 1].type == 5)))) || (world.getTile()[i + 1, j].active && (Statics.tileSolid[world.getTile()[i + 1, j].type] || (((world.getTile()[i + 1, j].type == 5) && (world.getTile()[i + 1, j - 1].type == 5)) && (world.getTile()[i + 1, j + 1].type == 5))))) || (world.getTile()[i, j + 1].active && Statics.tileSolid[world.getTile()[i, j + 1].type]))
                    {
                        world.getTile()[i, j].active = true;
                        world.getTile()[i, j].type = (byte)type;
                        SquareTileFrame(i, j, world, true);
                    }
                }
                else if (type == 10)
                {
                    if (world.getTile()[i, j - 1] == null)
                    {
                        world.getTile()[i, j - 1] = new Tile();
                    }
                    if (world.getTile()[i, j - 2] == null)
                    {
                        world.getTile()[i, j - 2] = new Tile();
                    }
                    if (world.getTile()[i, j - 3] == null)
                    {
                        world.getTile()[i, j - 3] = new Tile();
                    }
                    if (world.getTile()[i, j + 1] == null)
                    {
                        world.getTile()[i, j + 1] = new Tile();
                    }
                    if (world.getTile()[i, j + 2] == null)
                    {
                        world.getTile()[i, j + 2] = new Tile();
                    }
                    if (world.getTile()[i, j + 3] == null)
                    {
                        world.getTile()[i, j + 3] = new Tile();
                    }
                    if ((world.getTile()[i, j - 1].active || world.getTile()[i, j - 2].active) || (!world.getTile()[i, j - 3].active || !Statics.tileSolid[world.getTile()[i, j - 3].type]))
                    {
                        if ((world.getTile()[i, j + 1].active || world.getTile()[i, j + 2].active) || (!world.getTile()[i, j + 3].active || !Statics.tileSolid[world.getTile()[i, j + 3].type]))
                        {
                            return false;
                        }
                        PlaceDoor(i, j + 1, type, world);
                        SquareTileFrame(i, j, world, true);
                    }
                    else
                    {
                        PlaceDoor(i, j - 1, type, world);
                        SquareTileFrame(i, j, world, true);
                    }
                }
                else if (((type == 0x22) || (type == 0x23)) || (type == 0x24))
                {
                    Place3x3(i, j, type, world);
                    SquareTileFrame(i, j, world, true);
                }
                else if (((type == 13) || (type == 0x21)) || (((type == 0x31) || (type == 50)) || (type == 0x4e)))
                {
                    PlaceOnTable1x1(i, j, type, world);
                    SquareTileFrame(i, j, world, true);
                }
                else if ((type == 14) || (type == 0x1a))
                {
                    Place3x2(i, j, type, world);
                    SquareTileFrame(i, j, world, true);
                }
                else if (type == 20)
                {
                    if (world.getTile()[i, j + 1] == null)
                    {
                        world.getTile()[i, j + 1] = new Tile();
                    }
                    if (world.getTile()[i, j + 1].active && (world.getTile()[i, j + 1].type == 2))
                    {
                        Place1x2(i, j, type, world);
                        SquareTileFrame(i, j, world, true);
                    }
                }
                else if (type == 15)
                {
                    if (world.getTile()[i, j - 1] == null)
                    {
                        world.getTile()[i, j - 1] = new Tile();
                    }
                    if (world.getTile()[i, j] == null)
                    {
                        world.getTile()[i, j] = new Tile();
                    }
                    Place1x2(i, j, type, world);
                    SquareTileFrame(i, j, world, true);
                }
                else if (((type == 0x10) || (type == 0x12)) || (type == 0x1d))
                {
                    Place2x1(i, j, type, world);
                    SquareTileFrame(i, j, world, true);
                }
                else if ((type == 0x11) || (type == 0x4d))
                {
                    Place3x2(i, j, type, world);
                    SquareTileFrame(i, j, world, true);
                }
                else if (type == 0x15)
                {
                    PlaceChest(i, j, world, type);
                    SquareTileFrame(i, j, world, true);
                }
                else if (type == 0x1b)
                {
                    PlaceSunflower(i, j, world, 0x1b);
                    SquareTileFrame(i, j, world, true);
                }
                else if (type == 0x1c)
                {
                    PlacePot(i, j, world, 0x1c);
                    SquareTileFrame(i, j, world, true);
                }
                else if (type == 0x2a)
                {
                    Place1x2Top(i, j, type, world);
                    SquareTileFrame(i, j, world, true);
                }
                else if (type == 0x37)
                {
                    PlaceSign(i, j, type, world);
                }
                else if (type == 0x4f)
                {
                    int direction = 1;
                    if (plr > -1)
                    {
                        direction = world.getPlayerList()[plr].direction;
                    }
                    Place4x2(i, j, type, world, direction);
                }
                else
                {
                    world.getTile()[i, j].active = true;
                    world.getTile()[i, j].type = (byte)type;
                }
                if (world.getTile()[i, j].active && !mute)
                {
                    SquareTileFrame(i, j, world, true);
                    flag = true;
                    //Main.PlaySound(0, i * 0x10, j * 0x10, 1);
                    if (type != 0x16)
                    {
                        return flag;
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        Color newColor = new Color();
                        Dust.NewDust(new Vector2((float)(i * 0x10), (float)(j * 0x10)), world, 0x10, 0x10, 14, 0f, 0f, 0, newColor, 1f);
                    }
                }
            }
            return flag;
        }

        public static bool StartRoomCheck(int x, int y, World world)
        {
            roomX1 = x;
            roomX2 = x;
            roomY1 = y;
            roomY2 = y;
            numRoomTiles = 0;
            for (int i = 0; i < 80; i++)
            {
                houseTile[i] = false;
            }
            canSpawn = true;
            if (world.getTile()[x, y].active && Statics.tileSolid[world.getTile()[x, y].type])
            {
                canSpawn = false;
            }
            CheckRoom(x, y, world);
            if (numRoomTiles < 60)
            {
                canSpawn = false;
            }
            return canSpawn;
        }

        public static void CheckRoom(int x, int y, World world)
        {
            if (canSpawn)
            {
                if (((x < 10) || (y < 10)) || ((x >= (world.getMaxTilesX() - 10)) || (y >= (lastMaxTilesY - 10))))
                {
                    canSpawn = false;
                }
                else
                {
                    for (int i = 0; i < numRoomTiles; i++)
                    {
                        if ((roomX[i] == x) && (roomY[i] == y))
                        {
                            return;
                        }
                    }
                    roomX[numRoomTiles] = x;
                    roomY[numRoomTiles] = y;
                    numRoomTiles++;
                    if (numRoomTiles >= maxRoomTiles)
                    {
                        canSpawn = false;
                    }
                    else
                    {
                        if (world.getTile()[x, y].active)
                        {
                            houseTile[world.getTile()[x, y].type] = true;
                            if (Statics.tileSolid[world.getTile()[x, y].type] || (world.getTile()[x, y].type == 11))
                            {
                                return;
                            }
                        }
                        if (x < roomX1)
                        {
                            roomX1 = x;
                        }
                        if (x > roomX2)
                        {
                            roomX2 = x;
                        }
                        if (y < roomY1)
                        {
                            roomY1 = y;
                        }
                        if (y > roomY2)
                        {
                            roomY2 = y;
                        }
                        bool flag = false;
                        bool flag2 = false;
                        for (int j = -2; j < 3; j++)
                        {
                            if (Statics.wallHouse[world.getTile()[x + j, y].wall])
                            {
                                flag = true;
                            }
                            if (world.getTile()[x + j, y].active && (Statics.tileSolid[world.getTile()[x + j, y].type] || (world.getTile()[x + j, y].type == 11)))
                            {
                                flag = true;
                            }
                            if (Statics.wallHouse[world.getTile()[x, y + j].wall])
                            {
                                flag2 = true;
                            }
                            if (world.getTile()[x, y + j].active && (Statics.tileSolid[world.getTile()[x, y + j].type] || (world.getTile()[x, y + j].type == 11)))
                            {
                                flag2 = true;
                            }
                        }
                        if (!flag || !flag2)
                        {
                            canSpawn = false;
                        }
                        else
                        {
                            for (int k = x - 1; k < (x + 2); k++)
                            {
                                for (int m = y - 1; m < (y + 2); m++)
                                {
                                    if (((k != x) || (m != y)) && canSpawn)
                                    {
                                        CheckRoom(k, m, world);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CheckPot(int i, int j, World world, int type = 0x1c)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = 0;
                int num2 = j;
                num += world.getTile()[i, j].frameX / 0x12;
                num2 += (world.getTile()[i, j].frameY / 0x12) * -1;
                while (num > 1)
                {
                    num -= 2;
                }
                num *= -1;
                num += i;
                for (int k = num; k < (num + 2); k++)
                {
                    for (int m = num2; m < (num2 + 2); m++)
                    {
                        if (world.getTile()[k, m] == null)
                        {
                            world.getTile()[k, m] = new Tile();
                        }
                        int num5 = world.getTile()[k, m].frameX / 0x12;
                        while (num5 > 1)
                        {
                            num5 -= 2;
                        }
                        if ((!world.getTile()[k, m].active || (world.getTile()[k, m].type != type)) || ((num5 != (k - num)) || (world.getTile()[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (world.getTile()[k, num2 + 2] == null)
                    {
                        world.getTile()[k, num2 + 2] = new Tile();
                    }
                    if (!world.getTile()[k, num2 + 2].active || !Statics.tileSolid[world.getTile()[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    //Main.PlaySound(13, i * 0x10, j * 0x10, 1);
                    for (int n = num; n < (num + 2); n++)
                    {
                        for (int num7 = num2; num7 < (num2 + 2); num7++)
                        {
                            if ((world.getTile()[n, num7].type == type) && world.getTile()[n, num7].active)
                            {
                                KillTile(n, num7, world, false, false, false);
                            }
                        }
                    }
                    Gore.NewGore(new Vector2((float)(i * 0x10), (float)(j * 0x10)), new Vector2(), 0x33, world);
                    Gore.NewGore(new Vector2((float)(i * 0x10), (float)(j * 0x10)), new Vector2(), 0x34, world);
                    Gore.NewGore(new Vector2((float)(i * 0x10), (float)(j * 0x10)), new Vector2(), 0x35, world);
                    if (Statics.rand == null)
                    {
                        Statics.rand = new Random();
                    }
                    int num8 = Statics.rand.Next(10);
                    int player = Player.FindClosest(new Vector2((float)(i * 0x10), (float)(j * 0x10)), 0x10, 0x10, world);
                    Player.FindClosest(new Vector2((float)(i * 0x10), (float)(j * 0x10)), 0x10, 0x10, world);
                    if ((num8 == 0) && (world.getPlayerList()[player] != null) && (world.getPlayerList()[player].statLife < world.getPlayerList()[player].statLifeMax))
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 0x3a, 1, false);
                    }
                    else if ((num8 == 1) && (world.getPlayerList()[player] != null) && (world.getPlayerList()[player].statMana < world.getPlayerList()[player].statManaMax))
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 0xb8, 1, false);
                    }
                    else if (num8 == 2)
                    {
                        int stack = Statics.rand.Next(3) + 1;
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 8, stack, false);
                    }
                    else if (num8 == 3)
                    {
                        int num10 = Statics.rand.Next(8) + 3;
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 40, num10, false);
                    }
                    else if (num8 == 4)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 0x1c, 1, false);
                    }
                    else if (num8 == 5)
                    {
                        int num11 = Statics.rand.Next(4) + 1;
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 0xa6, num11, false);
                    }
                    else
                    {
                        float num12 = 200 + genRand.Next(-100, 0x65);
                        num12 *= 1f + (Statics.rand.Next(-20, 0x15) * 0.01f);
                        if (Statics.rand.Next(5) == 0)
                        {
                            num12 *= 1f + (Statics.rand.Next(5, 11) * 0.01f);
                        }
                        if (Statics.rand.Next(10) == 0)
                        {
                            num12 *= 1f + (Statics.rand.Next(10, 0x15) * 0.01f);
                        }
                        if (Statics.rand.Next(15) == 0)
                        {
                            num12 *= 1f + (Statics.rand.Next(20, 0x29) * 0.01f);
                        }
                        if (Statics.rand.Next(20) == 0)
                        {
                            num12 *= 1f + (Statics.rand.Next(40, 0x51) * 0.01f);
                        }
                        if (Statics.rand.Next(0x19) == 0)
                        {
                            num12 *= 1f + (Statics.rand.Next(50, 0x65) * 0.01f);
                        }
                        while (((int)num12) > 0)
                        {
                            if (num12 > 1000000f)
                            {
                                int num13 = (int)(num12 / 1000000f);
                                if ((num13 > 50) && (Statics.rand.Next(2) == 0))
                                {
                                    num13 /= Statics.rand.Next(3) + 1;
                                }
                                if (Statics.rand.Next(2) == 0)
                                {
                                    num13 /= Statics.rand.Next(3) + 1;
                                }
                                num12 -= 0xf4240 * num13;
                                Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 0x4a, num13, false);
                            }
                            else
                            {
                                if (num12 > 10000f)
                                {
                                    int num14 = (int)(num12 / 10000f);
                                    if ((num14 > 50) && (Statics.rand.Next(2) == 0))
                                    {
                                        num14 /= Statics.rand.Next(3) + 1;
                                    }
                                    if (Statics.rand.Next(2) == 0)
                                    {
                                        num14 /= Statics.rand.Next(3) + 1;
                                    }
                                    num12 -= 0x2710 * num14;
                                    Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 0x49, num14, false);
                                    continue;
                                }
                                if (num12 > 100f)
                                {
                                    int num15 = (int)(num12 / 100f);
                                    if ((num15 > 50) && (Statics.rand.Next(2) == 0))
                                    {
                                        num15 /= Statics.rand.Next(3) + 1;
                                    }
                                    if (Statics.rand.Next(2) == 0)
                                    {
                                        num15 /= Statics.rand.Next(3) + 1;
                                    }
                                    num12 -= 100 * num15;
                                    Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 0x48, num15, false);
                                    continue;
                                }
                                int num16 = (int)num12;
                                if ((num16 > 50) && (Statics.rand.Next(2) == 0))
                                {
                                    num16 /= Statics.rand.Next(3) + 1;
                                }
                                if (Statics.rand.Next(2) == 0)
                                {
                                    num16 /= Statics.rand.Next(4) + 1;
                                }
                                if (num16 < 1)
                                {
                                    num16 = 1;
                                }
                                num12 -= num16;
                                Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 0x47, num16, false);
                            }
                        }
                    }
                    destroyObject = false;
                }
            }
        }

        public static void CheckSign(int x, int y, int type, World world)
        {
            if (!destroyObject)
            {
                int num = x - 2;
                int num2 = x + 3;
                int num3 = y - 2;
                int num4 = y + 3;
                if ((((num >= 0) && (num2 <= world.getMaxTilesX())) && (num3 >= 0)) && (num4 <= world.getMaxTilesY()))
                {
                    bool flag = false;
                    for (int i = num; i < num2; i++)
                    {
                        for (int k = num3; k < num4; k++)
                        {
                            if (world.getTile()[i, k] == null)
                            {
                                world.getTile()[i, k] = new Tile();
                            }
                        }
                    }
                    int num7 = world.getTile()[x, y].frameX / 0x12;
                    int num8 = world.getTile()[x, y].frameY / 0x12;
                    while (num7 > 1)
                    {
                        num7 -= 2;
                    }
                    int num9 = x - num7;
                    int num10 = y - num8;
                    int num11 = (world.getTile()[num9, num10].frameX / 0x12) / 2;
                    num = num9;
                    num2 = num9 + 2;
                    num3 = num10;
                    num4 = num10 + 2;
                    num7 = 0;
                    for (int j = num; j < num2; j++)
                    {
                        num8 = 0;
                        for (int m = num3; m < num4; m++)
                        {
                            if (!world.getTile()[j, m].active || (world.getTile()[j, m].type != type))
                            {
                                flag = true;
                                goto Label_017B;
                            }
                            if (((world.getTile()[j, m].frameX / 0x12) != (num7 + (num11 * 2))) || ((world.getTile()[j, m].frameY / 0x12) != num8))
                            {
                                flag = true;
                                goto Label_017B;
                            }
                            num8++;
                        }
                    Label_017B:
                        num7++;
                    }
                    if (!flag)
                    {
                        if ((world.getTile()[num9, num10 + 2].active && Statics.tileSolid[world.getTile()[num9, num10 + 2].type]) && (world.getTile()[num9 + 1, num10 + 2].active && Statics.tileSolid[world.getTile()[num9 + 1, num10 + 2].type]))
                        {
                            num11 = 0;
                        }
                        else if (((world.getTile()[num9, num10 - 1].active && Statics.tileSolid[world.getTile()[num9, num10 - 1].type]) && (!Statics.tileSolidTop[world.getTile()[num9, num10 - 1].type] && world.getTile()[num9 + 1, num10 - 1].active)) && (Statics.tileSolid[world.getTile()[num9 + 1, num10 - 1].type] && !Statics.tileSolidTop[world.getTile()[num9 + 1, num10 - 1].type]))
                        {
                            num11 = 1;
                        }
                        else if (((world.getTile()[num9 - 1, num10].active && Statics.tileSolid[world.getTile()[num9 - 1, num10].type]) && (!Statics.tileSolidTop[world.getTile()[num9 - 1, num10].type] && world.getTile()[num9 - 1, num10 + 1].active)) && (Statics.tileSolid[world.getTile()[num9 - 1, num10 + 1].type] && !Statics.tileSolidTop[world.getTile()[num9 - 1, num10 + 1].type]))
                        {
                            num11 = 2;
                        }
                        else if (((world.getTile()[num9 + 2, num10].active && Statics.tileSolid[world.getTile()[num9 + 2, num10].type]) && (!Statics.tileSolidTop[world.getTile()[num9 + 2, num10].type] && world.getTile()[num9 + 2, num10 + 1].active)) && (Statics.tileSolid[world.getTile()[num9 + 2, num10 + 1].type] && !Statics.tileSolidTop[world.getTile()[num9 + 2, num10 + 1].type]))
                        {
                            num11 = 3;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        destroyObject = true;
                        for (int n = num; n < num2; n++)
                        {
                            for (int num15 = num3; num15 < num4; num15++)
                            {
                                if (world.getTile()[n, num15].type == type)
                                {
                                    KillTile(n, num15, world, false, false, false);
                                }
                            }
                        }
                        Sign.KillSign(num9, num10, world);
                        Item.NewItem(x * 0x10, y * 0x10, world, 0x20, 0x20, 0xab, 1, false);
                        destroyObject = false;
                    }
                    else
                    {
                        int num16 = 0x24 * num11;
                        for (int num17 = 0; num17 < 2; num17++)
                        {
                            for (int num18 = 0; num18 < 2; num18++)
                            {
                                world.getTile()[num9 + num17, num10 + num18].active = true;
                                world.getTile()[num9 + num17, num10 + num18].type = (byte)type;
                                world.getTile()[num9 + num17, num10 + num18].frameX = (short)(num16 + (0x12 * num17));
                                world.getTile()[num9 + num17, num10 + num18].frameY = (short)(0x12 * num18);
                            }
                        }
                    }
                }
            }
        }

        public static void CheckSunflower(int i, int j, World world, int type = 0x1b)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = 0;
                int num2 = j;
                num += world.getTile()[i, j].frameX / 0x12;
                num2 += (world.getTile()[i, j].frameY / 0x12) * -1;
                while (num > 1)
                {
                    num -= 2;
                }
                num *= -1;
                num += i;
                for (int k = num; k < (num + 2); k++)
                {
                    for (int m = num2; m < (num2 + 4); m++)
                    {
                        if (world.getTile()[k, m] == null)
                        {
                            world.getTile()[k, m] = new Tile();
                        }
                        int num5 = world.getTile()[k, m].frameX / 0x12;
                        while (num5 > 1)
                        {
                            num5 -= 2;
                        }
                        if ((!world.getTile()[k, m].active || (world.getTile()[k, m].type != type)) || ((num5 != (k - num)) || (world.getTile()[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (world.getTile()[k, num2 + 4] == null)
                    {
                        world.getTile()[k, num2 + 4] = new Tile();
                    }
                    if (!world.getTile()[k, num2 + 4].active || (world.getTile()[k, num2 + 4].type != 2))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 2); n++)
                    {
                        for (int num7 = num2; num7 < (num2 + 4); num7++)
                        {
                            if ((world.getTile()[n, num7].type == type) && world.getTile()[n, num7].active)
                            {
                                KillTile(n, num7, world, false, false, false);
                            }
                        }
                    }
                    Item.NewItem(i * 0x10, j * 0x10, world, 0x20, 0x20, 0x3f, 1, false);
                    destroyObject = false;
                }
            }
        }

        public static void Check4x2(int i, int j, int type, World world)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = i;
                int num2 = j;
                num += (world.getTile()[i, j].frameX / 0x12) * -1;
                if ((type == 0x4f) && (world.getTile()[i, j].frameX >= 0x48))
                {
                    num += 4;
                }
                num2 += (world.getTile()[i, j].frameY / 0x12) * -1;
                for (int k = num; k < (num + 4); k++)
                {
                    for (int m = num2; m < (num2 + 2); m++)
                    {
                        int num5 = (k - num) * 0x12;
                        if ((type == 0x4f) && (world.getTile()[i, j].frameX >= 0x48))
                        {
                            num5 = ((k - num) + 4) * 0x12;
                        }
                        if (world.getTile()[k, m] == null)
                        {
                            world.getTile()[k, m] = new Tile();
                        }
                        if ((!world.getTile()[k, m].active || (world.getTile()[k, m].type != type)) || ((world.getTile()[k, m].frameX != num5) || (world.getTile()[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (world.getTile()[k, num2 + 2] == null)
                    {
                        world.getTile()[k, num2 + 2] = new Tile();
                    }
                    if (!world.getTile()[k, num2 + 2].active || !Statics.tileSolid[world.getTile()[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 4); n++)
                    {
                        for (int num7 = num2; num7 < (num2 + 3); num7++)
                        {
                            if ((world.getTile()[n, num7].type == type) && world.getTile()[n, num7].active)
                            {
                                KillTile(n, num7, world, false, false, false);
                            }
                        }
                    }
                    if (type == 0x4f)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x20, 0x20, 0xe0, 1, false);
                    }
                    destroyObject = false;
                    for (int num8 = num - 1; num8 < (num + 4); num8++)
                    {
                        for (int num9 = num2 - 1; num9 < (num2 + 4); num9++)
                        {
                            TileFrame(num8, num9, world, false, false);
                        }
                    }
                }
            }
        }

        public static void CheckChest(int i, int j, int type, World world)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = i;
                int num2 = j;
                num += (world.getTile()[i, j].frameX / 0x12) * -1;
                num2 += (world.getTile()[i, j].frameY / 0x12) * -1;
                for (int k = num; k < (num + 2); k++)
                {
                    for (int m = num2; m < (num2 + 2); m++)
                    {
                        if (world.getTile()[k, m] == null)
                        {
                            world.getTile()[k, m] = new Tile();
                        }
                        if ((!world.getTile()[k, m].active || (world.getTile()[k, m].type != type)) || ((world.getTile()[k, m].frameX != ((k - num) * 0x12)) || (world.getTile()[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (world.getTile()[k, num2 + 2] == null)
                    {
                        world.getTile()[k, num2 + 2] = new Tile();
                    }
                    if (!world.getTile()[k, num2 + 2].active || !Statics.tileSolid[world.getTile()[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 2); n++)
                    {
                        for (int num6 = num2; num6 < (num2 + 3); num6++)
                        {
                            if ((world.getTile()[n, num6].type == type) && world.getTile()[n, num6].active)
                            {
                                KillTile(n, num6, world, false, false, false);
                            }
                        }
                    }
                    Item.NewItem(i * 0x10, j * 0x10, world, 0x20, 0x20, 0x30, 1, false);
                    destroyObject = false;
                }
            }
        }

        public static void CheckOnTable1x1(int x, int y, int type, World world)
        {
            if ((world.getTile()[x, y + 1] != null) && (!world.getTile()[x, y + 1].active || !Statics.tileTable[world.getTile()[x, y + 1].type]))
            {
                if (type == 0x4e)
                {
                    if (!world.getTile()[x, y + 1].active || !Statics.tileSolid[world.getTile()[x, y + 1].type])
                    {
                        KillTile(x, y, world, false, false, false);
                    }
                }
                else
                {
                    KillTile(x, y, world, false, false, false);
                }
            }
        }

        public static void Check1x2(int x, int j, byte type, World world)
        {
            if (!destroyObject)
            {
                int num = j;
                bool flag = true;
                if (world.getTile()[x, num] == null)
                {
                    world.getTile()[x, num] = new Tile();
                }
                if (world.getTile()[x, num + 1] == null)
                {
                    world.getTile()[x, num + 1] = new Tile();
                }
                if (world.getTile()[x, num].frameY == 0x12)
                {
                    num--;
                }
                if (world.getTile()[x, num] == null)
                {
                    world.getTile()[x, num] = new Tile();
                }
                if (((world.getTile()[x, num].frameY == 0) && (world.getTile()[x, num + 1].frameY == 0x12)) && ((world.getTile()[x, num].type == type) && (world.getTile()[x, num + 1].type == type)))
                {
                    flag = false;
                }
                if (world.getTile()[x, num + 2] == null)
                {
                    world.getTile()[x, num + 2] = new Tile();
                }
                if (!world.getTile()[x, num + 2].active || !Statics.tileSolid[world.getTile()[x, num + 2].type])
                {
                    flag = true;
                }
                if ((world.getTile()[x, num + 2].type != 2) && (world.getTile()[x, num].type == 20))
                {
                    flag = true;
                }
                if (flag)
                {
                    destroyObject = true;
                    if (world.getTile()[x, num].type == type)
                    {
                        KillTile(x, num, world, false, false, false);
                    }
                    if (world.getTile()[x, num + 1].type == type)
                    {
                        KillTile(x, num + 1, world, false, false, false);
                    }
                    if (type == 15)
                    {
                        Item.NewItem(x * 0x10, num * 0x10, world, 0x20, 0x20, 0x22, 1, false);
                    }
                    destroyObject = false;
                }
            }
        }

        public static void Check1x2Top(int x, int j, byte type, World world)
        {
            if (!destroyObject)
            {
                int num = j;
                bool flag = true;
                if (world.getTile()[x, num] == null)
                {
                    world.getTile()[x, num] = new Tile();
                }
                if (world.getTile()[x, num + 1] == null)
                {
                    world.getTile()[x, num + 1] = new Tile();
                }
                if (world.getTile()[x, num].frameY == 0x12)
                {
                    num--;
                }
                if (world.getTile()[x, num] == null)
                {
                    world.getTile()[x, num] = new Tile();
                }
                if (((world.getTile()[x, num].frameY == 0) && (world.getTile()[x, num + 1].frameY == 0x12)) && ((world.getTile()[x, num].type == type) && (world.getTile()[x, num + 1].type == type)))
                {
                    flag = false;
                }
                if (world.getTile()[x, num - 1] == null)
                {
                    world.getTile()[x, num - 1] = new Tile();
                }
                if ((!world.getTile()[x, num - 1].active || !Statics.tileSolid[world.getTile()[x, num - 1].type]) || Statics.tileSolidTop[world.getTile()[x, num - 1].type])
                {
                    flag = true;
                }
                if (flag)
                {
                    destroyObject = true;
                    if (world.getTile()[x, num].type == type)
                    {
                        KillTile(x, num, world, false, false, false);
                    }
                    if (world.getTile()[x, num + 1].type == type)
                    {
                        KillTile(x, num + 1, world, false, false, false);
                    }
                    if (type == 0x2a)
                    {
                        Item.NewItem(x * 0x10, num * 0x10, world, 0x20, 0x20, 0x88, 1, false);
                    }
                    destroyObject = false;
                }
            }
        }

        public static void Check2x1(int i, int y, byte type, World world)
        {
            if (!destroyObject)
            {
                int num = i;
                bool flag = true;
                if (world.getTile()[num, y] == null)
                {
                    world.getTile()[num, y] = new Tile();
                }
                if (world.getTile()[num + 1, y] == null)
                {
                    world.getTile()[num + 1, y] = new Tile();
                }
                if (world.getTile()[num, y + 1] == null)
                {
                    world.getTile()[num, y + 1] = new Tile();
                }
                if (world.getTile()[num + 1, y + 1] == null)
                {
                    world.getTile()[num + 1, y + 1] = new Tile();
                }
                if (world.getTile()[num, y].frameX == 0x12)
                {
                    num--;
                }
                if (world.getTile()[num, y] == null)
                {
                    world.getTile()[num, y] = new Tile();
                }
                if (((world.getTile()[num, y].frameX == 0) && (world.getTile()[num + 1, y].frameX == 0x12)) && ((world.getTile()[num, y].type == type) && (world.getTile()[num + 1, y].type == type)))
                {
                    flag = false;
                }
                if (type == 0x1d)
                {
                    if (!world.getTile()[num, y + 1].active || !Statics.tileTable[world.getTile()[num, y + 1].type])
                    {
                        flag = true;
                    }
                    if (!world.getTile()[num + 1, y + 1].active || !Statics.tileTable[world.getTile()[num + 1, y + 1].type])
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (!world.getTile()[num, y + 1].active || !Statics.tileSolid[world.getTile()[num, y + 1].type])
                    {
                        flag = true;
                    }
                    if (!world.getTile()[num + 1, y + 1].active || !Statics.tileSolid[world.getTile()[num + 1, y + 1].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    if (world.getTile()[num, y].type == type)
                    {
                        KillTile(num, y, world, false, false, false);
                    }
                    if (world.getTile()[num + 1, y].type == type)
                    {
                        KillTile(num + 1, y, world, false, false, false);
                    }
                    if (type == 0x10)
                    {
                        Item.NewItem(num * 0x10, y * 0x10, world, 0x20, 0x20, 0x23, 1, false);
                    }
                    if (type == 0x12)
                    {
                        Item.NewItem(num * 0x10, y * 0x10, world, 0x20, 0x20, 0x24, 1, false);
                    }
                    if (type == 0x1d)
                    {
                        Item.NewItem(num * 0x10, y * 0x10, world, 0x20, 0x20, 0x57, 1, false);
                        //Main.PlaySound(13, i * 0x10, y * 0x10, 1);
                    }
                    destroyObject = false;
                }
            }
        }

        public static void Check3x2(int i, int j, int type, World world)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = i;
                int num2 = j;
                num += (world.getTile()[i, j].frameX / 0x12) * -1;
                num2 += (world.getTile()[i, j].frameY / 0x12) * -1;
                for (int k = num; k < (num + 3); k++)
                {
                    for (int m = num2; m < (num2 + 2); m++)
                    {
                        if (world.getTile()[k, m] == null)
                        {
                            world.getTile()[k, m] = new Tile();
                        }
                        if ((!world.getTile()[k, m].active || (world.getTile()[k, m].type != type)) || ((world.getTile()[k, m].frameX != ((k - num) * 0x12)) || (world.getTile()[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (world.getTile()[k, num2 + 2] == null)
                    {
                        world.getTile()[k, num2 + 2] = new Tile();
                    }
                    if (!world.getTile()[k, num2 + 2].active || !Statics.tileSolid[world.getTile()[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 3); n++)
                    {
                        for (int num6 = num2; num6 < (num2 + 3); num6++)
                        {
                            if ((world.getTile()[n, num6].type == type) && world.getTile()[n, num6].active)
                            {
                                KillTile(n, num6, world, false, false, false);
                            }
                        }
                    }
                    if (type == 14)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x20, 0x20, 0x20, 1, false);
                    }
                    else if (type == 0x11)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x20, 0x20, 0x21, 1, false);
                    }
                    else if (type == 0x4d)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x20, 0x20, 0xdd, 1, false);
                    }
                    destroyObject = false;
                    for (int num7 = num - 1; num7 < (num + 4); num7++)
                    {
                        for (int num8 = num2 - 1; num8 < (num2 + 4); num8++)
                        {
                            TileFrame(num7, num8, world, false, false);
                        }
                    }
                }
            }
        }

        public static void Check3x3(int i, int j, int type, World world)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = i;
                int num2 = j;
                num += (world.getTile()[i, j].frameX / 0x12) * -1;
                num2 += (world.getTile()[i, j].frameY / 0x12) * -1;
                for (int k = num; k < (num + 3); k++)
                {
                    for (int m = num2; m < (num2 + 3); m++)
                    {
                        if (world.getTile()[k, m] == null)
                        {
                            world.getTile()[k, m] = new Tile();
                        }
                        if ((!world.getTile()[k, m].active || (world.getTile()[k, m].type != type)) || ((world.getTile()[k, m].frameX != ((k - num) * 0x12)) || (world.getTile()[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                }
                if (world.getTile()[num + 1, num2 - 1] == null)
                {
                    world.getTile()[num + 1, num2 - 1] = new Tile();
                }
                if ((!world.getTile()[num + 1, num2 - 1].active || !Statics.tileSolid[world.getTile()[num + 1, num2 - 1].type]) || Statics.tileSolidTop[world.getTile()[num + 1, num2 - 1].type])
                {
                    flag = true;
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 3); n++)
                    {
                        for (int num6 = num2; num6 < (num2 + 3); num6++)
                        {
                            if ((world.getTile()[n, num6].type == type) && world.getTile()[n, num6].active)
                            {
                                KillTile(n, num6, world, false, false, false);
                            }
                        }
                    }
                    if (type == 0x22)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x20, 0x20, 0x6a, 1, false);
                    }
                    else if (type == 0x23)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x20, 0x20, 0x6b, 1, false);
                    }
                    else if (type == 0x24)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, world, 0x20, 0x20, 0x6c, 1, false);
                    }
                    destroyObject = false;
                    for (int num7 = num - 1; num7 < (num + 4); num7++)
                    {
                        for (int num8 = num2 - 1; num8 < (num2 + 4); num8++)
                        {
                            TileFrame(num7, num8, world, false, false);
                        }
                    }
                }
            }
        }

        public static void SquareTileFrame(int i, int j, World world, bool resetFrame = true)
        {
            TileFrame(i - 1, j - 1, world, false, false);
            TileFrame(i - 1, j, world, false, false);
            TileFrame(i - 1, j + 1, world, false, false);
            TileFrame(i, j - 1, world, false, false);
            TileFrame(i, j, world, resetFrame, false);
            TileFrame(i, j + 1, world, false, false);
            TileFrame(i + 1, j - 1, world, false, false);
            TileFrame(i + 1, j, world, false, false);
            TileFrame(i + 1, j + 1, world, false, false);
        }

        public static void KillTile(int i, int j, World world, bool fail = false, bool effectOnly = false, bool noItem = false)
        {
            if (((i >= 0) && (j >= 0)) && ((i < world.getMaxTilesX()) && (j < world.getMaxTilesY())))
            {
                if (world.getTile()[i, j] == null)
                {
                    world.getTile()[i, j] = new Tile();
                }
                if (world.getTile()[i, j].active)
                {
                    if ((j >= 1) && (world.getTile()[i, j - 1] == null))
                    {
                        world.getTile()[i, j - 1] = new Tile();
                    }
                    if ((((j < 1) || !world.getTile()[i, j - 1].active) || ((((world.getTile()[i, j - 1].type != 5) || (world.getTile()[i, j].type == 5)) && ((world.getTile()[i, j - 1].type != 0x15) || (world.getTile()[i, j].type == 0x15))) && ((((world.getTile()[i, j - 1].type != 0x1a) || (world.getTile()[i, j].type == 0x1a)) && ((world.getTile()[i, j - 1].type != 0x48) || (world.getTile()[i, j].type == 0x48))) && ((world.getTile()[i, j - 1].type != 12) || (world.getTile()[i, j].type == 12))))) || ((world.getTile()[i, j - 1].type == 5) && (((((world.getTile()[i, j - 1].frameX == 0x42) && (world.getTile()[i, j - 1].frameY >= 0)) && (world.getTile()[i, j - 1].frameY <= 0x2c)) || (((world.getTile()[i, j - 1].frameX == 0x58) && (world.getTile()[i, j - 1].frameY >= 0x42)) && (world.getTile()[i, j - 1].frameY <= 110))) || (world.getTile()[i, j - 1].frameY >= 0xc6))))
                    {
                        if (!effectOnly && !world.getStopDrops())
                        {
                            if (world.getTile()[i, j].type == 3)
                            {
                                //Main.PlaySound(6, i * 0x10, j * 0x10, 1);
                                if (world.getTile()[i, j].frameX == 0x90)
                                {
                                    Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 5, 1, false);
                                }
                            }
                            else if (world.getTile()[i, j].type == 0x18)
                            {
                                //Main.PlaySound(6, i * 0x10, j * 0x10, 1);
                                if (world.getTile()[i, j].frameX == 0x90)
                                {
                                    Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, 60, 1, false);
                                }
                            }
                            else if ((((world.getTile()[i, j].type == 0x20) || (world.getTile()[i, j].type == 0x33)) || ((world.getTile()[i, j].type == 0x34) || (world.getTile()[i, j].type == 0x3d))) || (((world.getTile()[i, j].type == 0x3e) || (world.getTile()[i, j].type == 0x45)) || (((world.getTile()[i, j].type == 0x47) || (world.getTile()[i, j].type == 0x49)) || (world.getTile()[i, j].type == 0x4a))))
                            {
                                //Main.PlaySound(6, i * 0x10, j * 0x10, 1);
                            }
                            else if ((((((world.getTile()[i, j].type == 1) || (world.getTile()[i, j].type == 6)) || ((world.getTile()[i, j].type == 7) || (world.getTile()[i, j].type == 8))) || (((world.getTile()[i, j].type == 9) || (world.getTile()[i, j].type == 0x16)) || ((world.getTile()[i, j].type == 0x19) || (world.getTile()[i, j].type == 0x25)))) || ((((world.getTile()[i, j].type == 0x26) || (world.getTile()[i, j].type == 0x27)) || ((world.getTile()[i, j].type == 0x29) || (world.getTile()[i, j].type == 0x2b))) || (((world.getTile()[i, j].type == 0x2c) || (world.getTile()[i, j].type == 0x2d)) || ((world.getTile()[i, j].type == 0x2e) || (world.getTile()[i, j].type == 0x2f))))) || ((((world.getTile()[i, j].type == 0x30) || (world.getTile()[i, j].type == 0x38)) || ((world.getTile()[i, j].type == 0x3a) || (world.getTile()[i, j].type == 0x3f))) || ((((world.getTile()[i, j].type == 0x40) || (world.getTile()[i, j].type == 0x41)) || ((world.getTile()[i, j].type == 0x42) || (world.getTile()[i, j].type == 0x43))) || (((world.getTile()[i, j].type == 0x44) || (world.getTile()[i, j].type == 0x4b)) || (world.getTile()[i, j].type == 0x4c)))))
                            {
                                //Main.PlaySound(0x15, i * 0x10, j * 0x10, 1);
                            }
                            else
                            {
                                //Main.PlaySound(0, i * 0x10, j * 0x10, 1);
                            }
                        }
                        int num = 10;
                        if (fail)
                        {
                            num = 3;
                        }
                        for (int k = 0; k < num; k++)
                        {
                            int type = 0;
                            if (world.getTile()[i, j].type == 0)
                            {
                                type = 0;
                            }
                            if ((((world.getTile()[i, j].type == 1) || (world.getTile()[i, j].type == 0x10)) || ((world.getTile()[i, j].type == 0x11) || (world.getTile()[i, j].type == 0x26))) || ((((world.getTile()[i, j].type == 0x27) || (world.getTile()[i, j].type == 0x29)) || ((world.getTile()[i, j].type == 0x2b) || (world.getTile()[i, j].type == 0x2c))) || ((world.getTile()[i, j].type == 0x30) || Statics.tileStone[world.getTile()[i, j].type])))
                            {
                                type = 1;
                            }
                            if ((world.getTile()[i, j].type == 4) || (world.getTile()[i, j].type == 0x21))
                            {
                                type = 6;
                            }
                            if ((((world.getTile()[i, j].type == 5) || (world.getTile()[i, j].type == 10)) || ((world.getTile()[i, j].type == 11) || (world.getTile()[i, j].type == 14))) || (((world.getTile()[i, j].type == 15) || (world.getTile()[i, j].type == 0x13)) || ((world.getTile()[i, j].type == 0x15) || (world.getTile()[i, j].type == 30))))
                            {
                                type = 7;
                            }
                            if (world.getTile()[i, j].type == 2)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 0;
                                }
                                else
                                {
                                    type = 2;
                                }
                            }
                            if ((world.getTile()[i, j].type == 6) || (world.getTile()[i, j].type == 0x1a))
                            {
                                type = 8;
                            }
                            if (((world.getTile()[i, j].type == 7) || (world.getTile()[i, j].type == 0x22)) || (world.getTile()[i, j].type == 0x2f))
                            {
                                type = 9;
                            }
                            if (((world.getTile()[i, j].type == 8) || (world.getTile()[i, j].type == 0x24)) || (world.getTile()[i, j].type == 0x2d))
                            {
                                type = 10;
                            }
                            if (((world.getTile()[i, j].type == 9) || (world.getTile()[i, j].type == 0x23)) || ((world.getTile()[i, j].type == 0x2a) || (world.getTile()[i, j].type == 0x2e)))
                            {
                                type = 11;
                            }
                            if (world.getTile()[i, j].type == 12)
                            {
                                type = 12;
                            }
                            if ((world.getTile()[i, j].type == 3) || (world.getTile()[i, j].type == 0x49))
                            {
                                type = 3;
                            }
                            if ((world.getTile()[i, j].type == 13) || (world.getTile()[i, j].type == 0x36))
                            {
                                type = 13;
                            }
                            if (world.getTile()[i, j].type == 0x16)
                            {
                                type = 14;
                            }
                            if ((world.getTile()[i, j].type == 0x1c) || (world.getTile()[i, j].type == 0x4e))
                            {
                                type = 0x16;
                            }
                            if (world.getTile()[i, j].type == 0x1d)
                            {
                                type = 0x17;
                            }
                            if (world.getTile()[i, j].type == 40)
                            {
                                type = 0x1c;
                            }
                            if (world.getTile()[i, j].type == 0x31)
                            {
                                type = 0x1d;
                            }
                            if (world.getTile()[i, j].type == 50)
                            {
                                type = 0x16;
                            }
                            if (world.getTile()[i, j].type == 0x33)
                            {
                                type = 30;
                            }
                            if (world.getTile()[i, j].type == 0x34)
                            {
                                type = 3;
                            }
                            if (world.getTile()[i, j].type == 0x35)
                            {
                                type = 0x20;
                            }
                            if ((world.getTile()[i, j].type == 0x38) || (world.getTile()[i, j].type == 0x4b))
                            {
                                type = 0x25;
                            }
                            if (world.getTile()[i, j].type == 0x39)
                            {
                                type = 0x24;
                            }
                            if (world.getTile()[i, j].type == 0x3b)
                            {
                                type = 0x26;
                            }
                            if (((world.getTile()[i, j].type == 0x3d) || (world.getTile()[i, j].type == 0x3e)) || (world.getTile()[i, j].type == 0x4a))
                            {
                                type = 40;
                            }
                            if (world.getTile()[i, j].type == 0x45)
                            {
                                type = 7;
                            }
                            if ((world.getTile()[i, j].type == 0x47) || (world.getTile()[i, j].type == 0x48))
                            {
                                type = 0x1a;
                            }
                            if (world.getTile()[i, j].type == 70)
                            {
                                type = 0x11;
                            }
                            if (world.getTile()[i, j].type == 2)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 0x26;
                                }
                                else
                                {
                                    type = 0x27;
                                }
                            }
                            if (((world.getTile()[i, j].type == 0x3a) || (world.getTile()[i, j].type == 0x4c)) || (world.getTile()[i, j].type == 0x4d))
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 6;
                                }
                                else
                                {
                                    type = 0x19;
                                }
                            }
                            if (world.getTile()[i, j].type == 0x25)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 6;
                                }
                                else
                                {
                                    type = 0x17;
                                }
                            }
                            if (world.getTile()[i, j].type == 0x20)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 14;
                                }
                                else
                                {
                                    type = 0x18;
                                }
                            }
                            if ((world.getTile()[i, j].type == 0x17) || (world.getTile()[i, j].type == 0x18))
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 14;
                                }
                                else
                                {
                                    type = 0x11;
                                }
                            }
                            if ((world.getTile()[i, j].type == 0x19) || (world.getTile()[i, j].type == 0x1f))
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 14;
                                }
                                else
                                {
                                    type = 1;
                                }
                            }
                            if (world.getTile()[i, j].type == 20)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 7;
                                }
                                else
                                {
                                    type = 2;
                                }
                            }
                            if (world.getTile()[i, j].type == 0x1b)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 3;
                                }
                                else
                                {
                                    type = 0x13;
                                }
                            }
                            if ((((world.getTile()[i, j].type == 0x22) || (world.getTile()[i, j].type == 0x23)) || ((world.getTile()[i, j].type == 0x24) || (world.getTile()[i, j].type == 0x2a))) && (Statics.rand.Next(2) == 0))
                            {
                                type = 6;
                            }
                            if (type >= 0)
                            {
                                Color newColor = new Color();
                                Dust.NewDust(new Vector2((float)(i * 0x10), (float)(j * 0x10)), world, 0x10, 0x10, type, 0f, 0f, 0, newColor, 1f);
                            }
                        }
                        if (!effectOnly)
                        {
                            if (fail)
                            {
                                if ((world.getTile()[i, j].type == 2) || (world.getTile()[i, j].type == 0x17))
                                {
                                    world.getTile()[i, j].type = 0;
                                }
                                if (world.getTile()[i, j].type == 60)
                                {
                                    world.getTile()[i, j].type = 0x3b;
                                }
                                SquareTileFrame(i, j, world, true);
                            }
                            else
                            {
                                if ((world.getTile()[i, j].type == 0x15) && (Statics.netMode != 1))
                                {
                                    int x = i - (world.getTile()[i, j].frameX / 0x12);
                                    int y = j - (world.getTile()[i, j].frameY / 0x12);
                                    if (!Chest.DestroyChest(x, y, world))
                                    {
                                        return;
                                    }
                                }
                                if (!noItem && !world.getStopDrops())
                                {
                                    int num6 = 0;
                                    if ((world.getTile()[i, j].type == 0) || (world.getTile()[i, j].type == 2))
                                    {
                                        num6 = 2;
                                    }
                                    else if (world.getTile()[i, j].type == 1)
                                    {
                                        num6 = 3;
                                    }
                                    else if (world.getTile()[i, j].type == 4)
                                    {
                                        num6 = 8;
                                    }
                                    else if (world.getTile()[i, j].type == 5)
                                    {
                                        if ((world.getTile()[i, j].frameX >= 0x16) && (world.getTile()[i, j].frameY >= 0xc6))
                                        {
                                            if (genRand.Next(2) == 0)
                                            {
                                                num6 = 0x1b;
                                            }
                                            else
                                            {
                                                num6 = 9;
                                            }
                                        }
                                        else
                                        {
                                            num6 = 9;
                                        }
                                    }
                                    else if (world.getTile()[i, j].type == 6)
                                    {
                                        num6 = 11;
                                    }
                                    else if (world.getTile()[i, j].type == 7)
                                    {
                                        num6 = 12;
                                    }
                                    else if (world.getTile()[i, j].type == 8)
                                    {
                                        num6 = 13;
                                    }
                                    else if (world.getTile()[i, j].type == 9)
                                    {
                                        num6 = 14;
                                    }
                                    else if (world.getTile()[i, j].type == 13)
                                    {
                                        //Main.PlaySound(13, i * 0x10, j * 0x10, 1);
                                        if (world.getTile()[i, j].frameX == 0x12)
                                        {
                                            num6 = 0x1c;
                                        }
                                        else if (world.getTile()[i, j].frameX == 0x24)
                                        {
                                            num6 = 110;
                                        }
                                        else
                                        {
                                            num6 = 0x1f;
                                        }
                                    }
                                    else if (world.getTile()[i, j].type == 0x13)
                                    {
                                        num6 = 0x5e;
                                    }
                                    else if (world.getTile()[i, j].type == 0x16)
                                    {
                                        num6 = 0x38;
                                    }
                                    else if (world.getTile()[i, j].type == 0x17)
                                    {
                                        num6 = 2;
                                    }
                                    else if (world.getTile()[i, j].type == 0x19)
                                    {
                                        num6 = 0x3d;
                                    }
                                    else if (world.getTile()[i, j].type == 30)
                                    {
                                        num6 = 9;
                                    }
                                    else if (world.getTile()[i, j].type == 0x21)
                                    {
                                        num6 = 0x69;
                                    }
                                    else if (world.getTile()[i, j].type == 0x25)
                                    {
                                        num6 = 0x74;
                                    }
                                    else if (world.getTile()[i, j].type == 0x26)
                                    {
                                        num6 = 0x81;
                                    }
                                    else if (world.getTile()[i, j].type == 0x27)
                                    {
                                        num6 = 0x83;
                                    }
                                    else if (world.getTile()[i, j].type == 40)
                                    {
                                        num6 = 0x85;
                                    }
                                    else if (world.getTile()[i, j].type == 0x29)
                                    {
                                        num6 = 0x86;
                                    }
                                    else if (world.getTile()[i, j].type == 0x2b)
                                    {
                                        num6 = 0x89;
                                    }
                                    else if (world.getTile()[i, j].type == 0x2c)
                                    {
                                        num6 = 0x8b;
                                    }
                                    else if (world.getTile()[i, j].type == 0x2d)
                                    {
                                        num6 = 0x8d;
                                    }
                                    else if (world.getTile()[i, j].type == 0x2e)
                                    {
                                        num6 = 0x8f;
                                    }
                                    else if (world.getTile()[i, j].type == 0x2f)
                                    {
                                        num6 = 0x91;
                                    }
                                    else if (world.getTile()[i, j].type == 0x30)
                                    {
                                        num6 = 0x93;
                                    }
                                    else if (world.getTile()[i, j].type == 0x31)
                                    {
                                        num6 = 0x94;
                                    }
                                    else if (world.getTile()[i, j].type == 0x33)
                                    {
                                        num6 = 150;
                                    }
                                    else if (world.getTile()[i, j].type == 0x35)
                                    {
                                        num6 = 0xa9;
                                    }
                                    else if (world.getTile()[i, j].type == 0x36)
                                    {
                                        //Main.PlaySound(13, i * 0x10, j * 0x10, 1);
                                    }
                                    else if (world.getTile()[i, j].type == 0x38)
                                    {
                                        num6 = 0xad;
                                    }
                                    else if (world.getTile()[i, j].type == 0x39)
                                    {
                                        num6 = 0xac;
                                    }
                                    else if (world.getTile()[i, j].type == 0x3a)
                                    {
                                        num6 = 0xae;
                                    }
                                    else if (world.getTile()[i, j].type == 60)
                                    {
                                        num6 = 0xb0;
                                    }
                                    else if (world.getTile()[i, j].type == 70)
                                    {
                                        num6 = 0xb0;
                                    }
                                    else if (world.getTile()[i, j].type == 0x4b)
                                    {
                                        num6 = 0xc0;
                                    }
                                    else if (world.getTile()[i, j].type == 0x4c)
                                    {
                                        num6 = 0xd6;
                                    }
                                    else if (world.getTile()[i, j].type == 0x4e)
                                    {
                                        num6 = 0xde;
                                    }
                                    else if ((world.getTile()[i, j].type == 0x3d) || (world.getTile()[i, j].type == 0x4a))
                                    {
                                        if (world.getTile()[i, j].frameX == 0xa2)
                                        {
                                            num6 = 0xdf;
                                        }
                                        else if (((world.getTile()[i, j].frameX >= 0x6c) && (world.getTile()[i, j].frameX <= 0x7e)) && (genRand.Next(2) == 0))
                                        {
                                            num6 = 0xd0;
                                        }
                                    }
                                    else if ((world.getTile()[i, j].type == 0x3b) || (world.getTile()[i, j].type == 60))
                                    {
                                        num6 = 0xb0;
                                    }
                                    else if ((world.getTile()[i, j].type == 0x47) || (world.getTile()[i, j].type == 0x48))
                                    {
                                        if (genRand.Next(50) == 0)
                                        {
                                            num6 = 0xc2;
                                        }
                                        else
                                        {
                                            num6 = 0xb7;
                                        }
                                    }
                                    else if ((world.getTile()[i, j].type == 0x4a) && (genRand.Next(100) == 0))
                                    {
                                        num6 = 0xc3;
                                    }
                                    else if ((world.getTile()[i, j].type >= 0x3f) && (world.getTile()[i, j].type <= 0x44))
                                    {
                                        num6 = (world.getTile()[i, j].type - 0x3f) + 0xb1;
                                    }
                                    else if (world.getTile()[i, j].type == 50)
                                    {
                                        if (world.getTile()[i, j].frameX == 90)
                                        {
                                            num6 = 0xa5;
                                        }
                                        else
                                        {
                                            num6 = 0x95;
                                        }
                                    }
                                    if (num6 > 0)
                                    {
                                        Item.NewItem(i * 0x10, j * 0x10, world, 0x10, 0x10, num6, 1, false);
                                    }
                                }
                                world.getTile()[i, j].active = false;
                                if (Statics.tileSolid[world.getTile()[i, j].type])
                                {
                                    world.getTile()[i, j].lighted = false;
                                }
                                world.getTile()[i, j].frameX = -1;
                                world.getTile()[i, j].frameY = -1;
                                world.getTile()[i, j].frameNumber = 0;
                                world.getTile()[i, j].type = 0;
                                SquareTileFrame(i, j, world, true);
                            }
                        }
                    }
                }
            }
        }

        public static void TileFrame(int i, int j, World world, bool resetFrame = false, bool noBreak = false)
        {
            if (i >= 0 && j >= 0 && i < world.getMaxTilesX() && j < world.getMaxTilesY() && world.getTile()[i, j] != null)
            {
                if (world.getTile()[i, j].liquid > 0 && Statics.netMode != 1 && !WorldGen.noLiquidCheck)
                {
                    Liquid.AddWater(i, j, world);
                }
                if (world.getTile()[i, j].active)
                {
                    if (noBreak && Statics.tileFrameImportant[(int)world.getTile()[i, j].type])
                    {
                        return;
                    }
                    int num = -1;
                    int num2 = -1;
                    int num3 = -1;
                    int num4 = -1;
                    int num5 = -1;
                    int num6 = -1;
                    int num7 = -1;
                    int num8 = -1;
                    int num9 = (int)world.getTile()[i, j].type;
                    if (Statics.tileStone[num9])
                    {
                        num9 = 1;
                    }
                    int frameX = (int)world.getTile()[i, j].frameX;
                    int frameY = (int)world.getTile()[i, j].frameY;
                    Rectangle rectangle = new Rectangle();
                    rectangle.X = -1;
                    rectangle.Y = -1;
                    if (num9 == 3 || num9 == 24 || num9 == 61 || num9 == 71 || num9 == 73 || num9 == 74)
                    {
                        WorldGen.PlantCheck(i, j, world);
                        return;
                    }
                    WorldGen.mergeUp = false;
                    WorldGen.mergeDown = false;
                    WorldGen.mergeLeft = false;
                    WorldGen.mergeRight = false;
                    if (i - 1 < 0)
                    {
                        num = num9;
                        num4 = num9;
                        num6 = num9;
                    }
                    if (i + 1 >= world.getMaxTilesX())
                    {
                        num3 = num9;
                        num5 = num9;
                        num8 = num9;
                    }
                    if (j - 1 < 0)
                    {
                        num = num9;
                        num2 = num9;
                        num3 = num9;
                    }
                    if (j + 1 >= world.getMaxTilesY())
                    {
                        num6 = num9;
                        num7 = num9;
                        num8 = num9;
                    }
                    if (i - 1 >= 0 && world.getTile()[i - 1, j] != null && world.getTile()[i - 1, j].active)
                    {
                        num4 = (int)world.getTile()[i - 1, j].type;
                    }
                    if (i + 1 < world.getMaxTilesX() && world.getTile()[i + 1, j] != null && world.getTile()[i + 1, j].active)
                    {
                        num5 = (int)world.getTile()[i + 1, j].type;
                    }
                    if (j - 1 >= 0 && world.getTile()[i, j - 1] != null && world.getTile()[i, j - 1].active)
                    {
                        num2 = (int)world.getTile()[i, j - 1].type;
                    }
                    if (j + 1 < world.getMaxTilesY() && world.getTile()[i, j + 1] != null && world.getTile()[i, j + 1].active)
                    {
                        num7 = (int)world.getTile()[i, j + 1].type;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0 && world.getTile()[i - 1, j - 1] != null && world.getTile()[i - 1, j - 1].active)
                    {
                        num = (int)world.getTile()[i - 1, j - 1].type;
                    }
                    if (i + 1 < world.getMaxTilesX() && j - 1 >= 0 && world.getTile()[i + 1, j - 1] != null && world.getTile()[i + 1, j - 1].active)
                    {
                        num3 = (int)world.getTile()[i + 1, j - 1].type;
                    }
                    if (i - 1 >= 0 && j + 1 < world.getMaxTilesY() && world.getTile()[i - 1, j + 1] != null && world.getTile()[i - 1, j + 1].active)
                    {
                        num6 = (int)world.getTile()[i - 1, j + 1].type;
                    }
                    if (i + 1 < world.getMaxTilesX() && j + 1 < world.getMaxTilesY() && world.getTile()[i + 1, j + 1] != null && world.getTile()[i + 1, j + 1].active)
                    {
                        num8 = (int)world.getTile()[i + 1, j + 1].type;
                    }
                    if (num4 >= 0 && Statics.tileStone[num4])
                    {
                        num4 = 1;
                    }
                    if (num5 >= 0 && Statics.tileStone[num5])
                    {
                        num5 = 1;
                    }
                    if (num2 >= 0 && Statics.tileStone[num2])
                    {
                        num2 = 1;
                    }
                    if (num7 >= 0 && Statics.tileStone[num7])
                    {
                        num7 = 1;
                    }
                    if (num >= 0 && Statics.tileStone[num])
                    {
                        num = 1;
                    }
                    if (num3 >= 0 && Statics.tileStone[num3])
                    {
                        num3 = 1;
                    }
                    if (num6 >= 0 && Statics.tileStone[num6])
                    {
                        num6 = 1;
                    }
                    if (num8 >= 0 && Statics.tileStone[num8])
                    {
                        num8 = 1;
                    }
                    if (num9 == 4)
                    {
                        if (num7 >= 0 && Statics.tileSolid[num7] && !Statics.tileNoAttach[num7])
                        {
                            world.getTile()[i, j].frameX = 0;
                            return;
                        }
                        if ((num4 >= 0 && Statics.tileSolid[num4] && !Statics.tileNoAttach[num4]) || (num4 == 5 && num == 5 && num6 == 5))
                        {
                            world.getTile()[i, j].frameX = 22;
                            return;
                        }
                        if ((num5 >= 0 && Statics.tileSolid[num5] && !Statics.tileNoAttach[num5]) || (num5 == 5 && num3 == 5 && num8 == 5))
                        {
                            world.getTile()[i, j].frameX = 44;
                            return;
                        }
                        WorldGen.KillTile(i, j, world, false, false, false);
                        return;
                    }
                    else
                    {
                        if (num9 == 12 || num9 == 31)
                        {
                            if (!WorldGen.destroyObject)
                            {
                                int num10 = i;
                                int num11 = j;
                                if (world.getTile()[i, j].frameX == 0)
                                {
                                    num10 = i;
                                }
                                else
                                {
                                    num10 = i - 1;
                                }
                                if (world.getTile()[i, j].frameY == 0)
                                {
                                    num11 = j;
                                }
                                else
                                {
                                    num11 = j - 1;
                                }
                                if (world.getTile()[num10, num11] != null && world.getTile()[num10 + 1, num11] != null && world.getTile()[num10, num11 + 1] != null && world.getTile()[num10 + 1, num11 + 1] != null && (!world.getTile()[num10, num11].active || (int)world.getTile()[num10, num11].type != num9 || !world.getTile()[num10 + 1, num11].active || (int)world.getTile()[num10 + 1, num11].type != num9 || !world.getTile()[num10, num11 + 1].active || (int)world.getTile()[num10, num11 + 1].type != num9 || !world.getTile()[num10 + 1, num11 + 1].active || (int)world.getTile()[num10 + 1, num11 + 1].type != num9))
                                {
                                    WorldGen.destroyObject = true;
                                    if ((int)world.getTile()[num10, num11].type == num9)
                                    {
                                        WorldGen.KillTile(num10, num11, world, false, false, false);
                                    }
                                    if ((int)world.getTile()[num10 + 1, num11].type == num9)
                                    {
                                        WorldGen.KillTile(num10 + 1, num11, world, false, false, false);
                                    }
                                    if ((int)world.getTile()[num10, num11 + 1].type == num9)
                                    {
                                        WorldGen.KillTile(num10, num11 + 1, world, false, false, false);
                                    }
                                    if ((int)world.getTile()[num10 + 1, num11 + 1].type == num9)
                                    {
                                        WorldGen.KillTile(num10 + 1, num11 + 1, world, false, false, false);
                                    }
                                    if (num9 == 12)
                                    {
                                        Item.NewItem(num10 * 16, num11 * 16, world, 32, 32, 29, 1, false);
                                    }
                                    else
                                    {
                                        if (num9 == 31)
                                        {
                                            if (WorldGen.genRand.Next(2) == 0)
                                            {
                                                WorldGen.spawnMeteor = true;
                                            }
                                            int num12 = Statics.rand.Next(5);
                                            if (!WorldGen.shadowOrbSmashed)
                                            {
                                                num12 = 0;
                                            }
                                            if (num12 == 0)
                                            {
                                                Item.NewItem(num10 * 16, num11 * 16, world, 32, 32, 96, 1, false);
                                                int stack = WorldGen.genRand.Next(25, 51);
                                                Item.NewItem(num10 * 16, num11 * 16, world, 32, 32, 97, stack, false);
                                            }
                                            else
                                            {
                                                if (num12 == 1)
                                                {
                                                    Item.NewItem(num10 * 16, num11 * 16, world, 32, 32, 64, 1, false);
                                                }
                                                else
                                                {
                                                    if (num12 == 2)
                                                    {
                                                        Item.NewItem(num10 * 16, num11 * 16, world, 32, 32, 162, 1, false);
                                                    }
                                                    else
                                                    {
                                                        if (num12 == 3)
                                                        {
                                                            Item.NewItem(num10 * 16, num11 * 16, world, 32, 32, 115, 1, false);
                                                        }
                                                        else
                                                        {
                                                            if (num12 == 4)
                                                            {
                                                                Item.NewItem(num10 * 16, num11 * 16, world, 32, 32, 111, 1, false);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            WorldGen.shadowOrbSmashed = true;
                                            WorldGen.shadowOrbCount++;
                                            if (WorldGen.shadowOrbCount >= 3)
                                            {
                                                WorldGen.shadowOrbCount = 0;
                                                float num13 = (float)(num10 * 16);
                                                float num14 = (float)(num11 * 16);
                                                float num15 = -1f;
                                                int plr = 0;
                                                for (int k = 0; k < 8; k++)
                                                {
                                                    float num16 = Math.Abs(world.getPlayerList()[k].position.X - num13) + Math.Abs(world.getPlayerList()[k].position.Y - num14);
                                                    if (num16 < num15 || num15 == -1f)
                                                    {
                                                        plr = 0;
                                                        num15 = num16;
                                                    }
                                                }
                                                NPC.SpawnOnPlayer(plr, 13, world);
                                            }
                                            else
                                            {
                                                string text = "A horrible chill goes down your spine...";
                                                if (WorldGen.shadowOrbCount == 2)
                                                {
                                                    text = "Screams echo around you...";
                                                }
                                                //if (Main.netMode == 0)
                                                //{
                                                //Main.NewText(text, 50, 255, 130);
                                                // }
                                                // else
                                                // {
                                                if (Statics.netMode == 2)
                                                {
                                                    NetMessage.SendData(25, world, -1, -1, text, 8, 50f, 255f, 130f);
                                                }
                                                //}
                                            }
                                        }
                                    }
                                    //Main.PlaySound(13, i * 16, j * 16, 1);
                                    WorldGen.destroyObject = false;
                                }
                            }
                            return;
                        }
                        if (num9 == 19)
                        {
                            if (num4 == num9 && num5 == num9)
                            {
                                if (world.getTile()[i, j].frameNumber == 0)
                                {
                                    rectangle.X = 0;
                                    rectangle.Y = 0;
                                }
                                if (world.getTile()[i, j].frameNumber == 1)
                                {
                                    rectangle.X = 0;
                                    rectangle.Y = 18;
                                }
                                if (world.getTile()[i, j].frameNumber == 2)
                                {
                                    rectangle.X = 0;
                                    rectangle.Y = 36;
                                }
                            }
                            else
                            {
                                if (num4 == num9 && num5 == -1)
                                {
                                    if (world.getTile()[i, j].frameNumber == 0)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 0;
                                    }
                                    if (world.getTile()[i, j].frameNumber == 1)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 18;
                                    }
                                    if (world.getTile()[i, j].frameNumber == 2)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 36;
                                    }
                                }
                                else
                                {
                                    if (num4 == -1 && num5 == num9)
                                    {
                                        if (world.getTile()[i, j].frameNumber == 0)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 0;
                                        }
                                        if (world.getTile()[i, j].frameNumber == 1)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 18;
                                        }
                                        if (world.getTile()[i, j].frameNumber == 2)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 36;
                                        }
                                    }
                                    else
                                    {
                                        if (num4 != num9 && num5 == num9)
                                        {
                                            if (world.getTile()[i, j].frameNumber == 0)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 0;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 1)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 18;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 2)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 36;
                                            }
                                        }
                                        else
                                        {
                                            if (num4 == num9 && num5 != num9)
                                            {
                                                if (world.getTile()[i, j].frameNumber == 0)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 0;
                                                }
                                                if (world.getTile()[i, j].frameNumber == 1)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 18;
                                                }
                                                if (world.getTile()[i, j].frameNumber == 2)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 36;
                                                }
                                            }
                                            else
                                            {
                                                if (num4 != num9 && num4 != -1 && num5 == -1)
                                                {
                                                    if (world.getTile()[i, j].frameNumber == 0)
                                                    {
                                                        rectangle.X = 108;
                                                        rectangle.Y = 0;
                                                    }
                                                    if (world.getTile()[i, j].frameNumber == 1)
                                                    {
                                                        rectangle.X = 108;
                                                        rectangle.Y = 18;
                                                    }
                                                    if (world.getTile()[i, j].frameNumber == 2)
                                                    {
                                                        rectangle.X = 108;
                                                        rectangle.Y = 36;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 == -1 && num5 != num9 && num5 != -1)
                                                    {
                                                        if (world.getTile()[i, j].frameNumber == 0)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 0;
                                                        }
                                                        if (world.getTile()[i, j].frameNumber == 1)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 18;
                                                        }
                                                        if (world.getTile()[i, j].frameNumber == 2)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 36;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (world.getTile()[i, j].frameNumber == 0)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 0;
                                                        }
                                                        if (world.getTile()[i, j].frameNumber == 1)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 18;
                                                        }
                                                        if (world.getTile()[i, j].frameNumber == 2)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 36;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (num9 == 10)
                            {
                                if (!WorldGen.destroyObject)
                                {
                                    int frameY2 = (int)world.getTile()[i, j].frameY;
                                    int num17 = j;
                                    bool flag = false;
                                    if (frameY2 == 0)
                                    {
                                        num17 = j;
                                    }
                                    if (frameY2 == 18)
                                    {
                                        num17 = j - 1;
                                    }
                                    if (frameY2 == 36)
                                    {
                                        num17 = j - 2;
                                    }
                                    if (world.getTile()[i, num17 - 1] == null)
                                    {
                                        world.getTile()[i, num17 - 1] = new Tile();
                                    }
                                    if (world.getTile()[i, num17 + 3] == null)
                                    {
                                        world.getTile()[i, num17 + 3] = new Tile();
                                    }
                                    if (world.getTile()[i, num17 + 2] == null)
                                    {
                                        world.getTile()[i, num17 + 2] = new Tile();
                                    }
                                    if (world.getTile()[i, num17 + 1] == null)
                                    {
                                        world.getTile()[i, num17 + 1] = new Tile();
                                    }
                                    if (world.getTile()[i, num17] == null)
                                    {
                                        world.getTile()[i, num17] = new Tile();
                                    }
                                    if (!world.getTile()[i, num17 - 1].active || !Statics.tileSolid[(int)world.getTile()[i, num17 - 1].type])
                                    {
                                        flag = true;
                                    }
                                    if (!world.getTile()[i, num17 + 3].active || !Statics.tileSolid[(int)world.getTile()[i, num17 + 3].type])
                                    {
                                        flag = true;
                                    }
                                    if (!world.getTile()[i, num17].active || (int)world.getTile()[i, num17].type != num9)
                                    {
                                        flag = true;
                                    }
                                    if (!world.getTile()[i, num17 + 1].active || (int)world.getTile()[i, num17 + 1].type != num9)
                                    {
                                        flag = true;
                                    }
                                    if (!world.getTile()[i, num17 + 2].active || (int)world.getTile()[i, num17 + 2].type != num9)
                                    {
                                        flag = true;
                                    }
                                    if (flag)
                                    {
                                        WorldGen.destroyObject = true;
                                        WorldGen.KillTile(i, num17, world, false, false, false);
                                        WorldGen.KillTile(i, num17 + 1, world, false, false, false);
                                        WorldGen.KillTile(i, num17 + 2, world, false, false, false);
                                        Item.NewItem(i * 16, j * 16, world, 16, 16, 25, 1, false);
                                    }
                                    WorldGen.destroyObject = false;
                                }
                                return;
                            }
                            if (num9 == 11)
                            {
                                if (!WorldGen.destroyObject)
                                {
                                    int num18 = 0;
                                    int num19 = i;
                                    int num20 = j;
                                    int frameX2 = (int)world.getTile()[i, j].frameX;
                                    int frameY3 = (int)world.getTile()[i, j].frameY;
                                    bool flag2 = false;
                                    if (frameX2 == 0)
                                    {
                                        num19 = i;
                                        num18 = 1;
                                    }
                                    else
                                    {
                                        if (frameX2 == 18)
                                        {
                                            num19 = i - 1;
                                            num18 = 1;
                                        }
                                        else
                                        {
                                            if (frameX2 == 36)
                                            {
                                                num19 = i + 1;
                                                num18 = -1;
                                            }
                                            else
                                            {
                                                if (frameX2 == 54)
                                                {
                                                    num19 = i;
                                                    num18 = -1;
                                                }
                                            }
                                        }
                                    }
                                    if (frameY3 == 0)
                                    {
                                        num20 = j;
                                    }
                                    else
                                    {
                                        if (frameY3 == 18)
                                        {
                                            num20 = j - 1;
                                        }
                                        else
                                        {
                                            if (frameY3 == 36)
                                            {
                                                num20 = j - 2;
                                            }
                                        }
                                    }
                                    if (world.getTile()[num19, num20 + 3] == null)
                                    {
                                        world.getTile()[num19, num20 + 3] = new Tile();
                                    }
                                    if (world.getTile()[num19, num20 - 1] == null)
                                    {
                                        world.getTile()[num19, num20 - 1] = new Tile();
                                    }
                                    if (!world.getTile()[num19, num20 - 1].active || !Statics.tileSolid[(int)world.getTile()[num19, num20 - 1].type] || !world.getTile()[num19, num20 + 3].active || !Statics.tileSolid[(int)world.getTile()[num19, num20 + 3].type])
                                    {
                                        flag2 = true;
                                        WorldGen.destroyObject = true;
                                        Item.NewItem(i * 16, j * 16, world, 16, 16, 25, 1, false);
                                    }
                                    int num21 = num19;
                                    if (num18 == -1)
                                    {
                                        num21 = num19 - 1;
                                    }
                                    for (int l = num21; l < num21 + 2; l++)
                                    {
                                        for (int m = num20; m < num20 + 3; m++)
                                        {
                                            if (!flag2 && (world.getTile()[l, m].type != 11 || !world.getTile()[l, m].active))
                                            {
                                                WorldGen.destroyObject = true;
                                                Item.NewItem(i * 16, j * 16, world, 16, 16, 25, 1, false);
                                                flag2 = true;
                                                l = num21;
                                                m = num20;
                                            }
                                            if (flag2)
                                            {
                                                WorldGen.KillTile(l, m, world, false, false, false);
                                            }
                                        }
                                    }
                                    WorldGen.destroyObject = false;
                                }
                                return;
                            }
                            if (num9 == 34 || num9 == 35 || num9 == 36)
                            {
                                WorldGen.Check3x3(i, j, num9, world);
                                return;
                            }
                            if (num9 == 15 || num9 == 20)
                            {
                                WorldGen.Check1x2(i, j, (byte)num9, world);
                                return;
                            }
                            if (num9 == 14 || num9 == 17 || num9 == 26 || num9 == 77)
                            {
                                WorldGen.Check3x2(i, j, num9, world);
                                return;
                            }
                            if (num9 == 16 || num9 == 18 || num9 == 29)
                            {
                                WorldGen.Check2x1(i, j, (byte)num9, world);
                                return;
                            }
                            if (num9 == 13 || num9 == 33 || num9 == 49 || num9 == 50 || num9 == 78)
                            {
                                WorldGen.CheckOnTable1x1(i, j, num9, world);
                                return;
                            }
                            if (num9 == 21)
                            {
                                WorldGen.CheckChest(i, j, num9, world);
                                return;
                            }
                            if (num9 == 27)
                            {
                                WorldGen.CheckSunflower(i, j, world, 27);
                                return;
                            }
                            if (num9 == 28)
                            {
                                WorldGen.CheckPot(i, j, world, 28);
                                return;
                            }
                            if (num9 == 42)
                            {
                                WorldGen.Check1x2Top(i, j, (byte)num9, world);
                                return;
                            }
                            if (num9 == 55)
                            {
                                WorldGen.CheckSign(i, j, num9, world);
                                return;
                            }
                            if (num9 == 79)
                            {
                                WorldGen.Check4x2(i, j, num9, world);
                                return;
                            }
                        }
                        if (num9 == 72)
                        {
                            if (num7 != num9 && num7 != 70)
                            {
                                WorldGen.KillTile(i, j, world, false, false, false);
                            }
                            else
                            {
                                if (num2 != num9 && world.getTile()[i, j].frameX == 0)
                                {
                                    world.getTile()[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
                                    if (world.getTile()[i, j].frameNumber == 0)
                                    {
                                        world.getTile()[i, j].frameX = 18;
                                        world.getTile()[i, j].frameY = 0;
                                    }
                                    if (world.getTile()[i, j].frameNumber == 1)
                                    {
                                        world.getTile()[i, j].frameX = 18;
                                        world.getTile()[i, j].frameY = 18;
                                    }
                                    if (world.getTile()[i, j].frameNumber == 2)
                                    {
                                        world.getTile()[i, j].frameX = 18;
                                        world.getTile()[i, j].frameY = 36;
                                    }
                                }
                            }
                        }
                        if (num9 == 5)
                        {
                            if (world.getTile()[i, j].frameX >= 22 && world.getTile()[i, j].frameX <= 44 && world.getTile()[i, j].frameY >= 132 && world.getTile()[i, j].frameY <= 176)
                            {
                                if ((num4 != num9 && num5 != num9) || num7 != 2)
                                {
                                    WorldGen.KillTile(i, j, world, false, false, false);
                                }
                            }
                            else
                            {
                                if ((world.getTile()[i, j].frameX == 88 && world.getTile()[i, j].frameY >= 0 && world.getTile()[i, j].frameY <= 44) || (world.getTile()[i, j].frameX == 66 && world.getTile()[i, j].frameY >= 66 && world.getTile()[i, j].frameY <= 130) || (world.getTile()[i, j].frameX == 110 && world.getTile()[i, j].frameY >= 66 && world.getTile()[i, j].frameY <= 110) || (world.getTile()[i, j].frameX == 132 && world.getTile()[i, j].frameY >= 0 && world.getTile()[i, j].frameY <= 176))
                                {
                                    if (num4 == num9 && num5 == num9)
                                    {
                                        if (world.getTile()[i, j].frameNumber == 0)
                                        {
                                            world.getTile()[i, j].frameX = 110;
                                            world.getTile()[i, j].frameY = 66;
                                        }
                                        if (world.getTile()[i, j].frameNumber == 1)
                                        {
                                            world.getTile()[i, j].frameX = 110;
                                            world.getTile()[i, j].frameY = 88;
                                        }
                                        if (world.getTile()[i, j].frameNumber == 2)
                                        {
                                            world.getTile()[i, j].frameX = 110;
                                            world.getTile()[i, j].frameY = 110;
                                        }
                                    }
                                    else
                                    {
                                        if (num4 == num9)
                                        {
                                            if (world.getTile()[i, j].frameNumber == 0)
                                            {
                                                world.getTile()[i, j].frameX = 88;
                                                world.getTile()[i, j].frameY = 0;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 1)
                                            {
                                                world.getTile()[i, j].frameX = 88;
                                                world.getTile()[i, j].frameY = 22;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 2)
                                            {
                                                world.getTile()[i, j].frameX = 88;
                                                world.getTile()[i, j].frameY = 44;
                                            }
                                        }
                                        else
                                        {
                                            if (num5 == num9)
                                            {
                                                if (world.getTile()[i, j].frameNumber == 0)
                                                {
                                                    world.getTile()[i, j].frameX = 66;
                                                    world.getTile()[i, j].frameY = 66;
                                                }
                                                if (world.getTile()[i, j].frameNumber == 1)
                                                {
                                                    world.getTile()[i, j].frameX = 66;
                                                    world.getTile()[i, j].frameY = 88;
                                                }
                                                if (world.getTile()[i, j].frameNumber == 2)
                                                {
                                                    world.getTile()[i, j].frameX = 66;
                                                    world.getTile()[i, j].frameY = 110;
                                                }
                                            }
                                            else
                                            {
                                                if (world.getTile()[i, j].frameNumber == 0)
                                                {
                                                    world.getTile()[i, j].frameX = 0;
                                                    world.getTile()[i, j].frameY = 0;
                                                }
                                                if (world.getTile()[i, j].frameNumber == 1)
                                                {
                                                    world.getTile()[i, j].frameX = 0;
                                                    world.getTile()[i, j].frameY = 22;
                                                }
                                                if (world.getTile()[i, j].frameNumber == 2)
                                                {
                                                    world.getTile()[i, j].frameX = 0;
                                                    world.getTile()[i, j].frameY = 44;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (world.getTile()[i, j].frameY >= 132 && world.getTile()[i, j].frameY <= 176 && (world.getTile()[i, j].frameX == 0 || world.getTile()[i, j].frameX == 66 || world.getTile()[i, j].frameX == 88))
                            {
                                if (num7 != 2)
                                {
                                    WorldGen.KillTile(i, j, world, false, false, false);
                                }
                                if (num4 != num9 && num5 != num9)
                                {
                                    if (world.getTile()[i, j].frameNumber == 0)
                                    {
                                        world.getTile()[i, j].frameX = 0;
                                        world.getTile()[i, j].frameY = 0;
                                    }
                                    if (world.getTile()[i, j].frameNumber == 1)
                                    {
                                        world.getTile()[i, j].frameX = 0;
                                        world.getTile()[i, j].frameY = 22;
                                    }
                                    if (world.getTile()[i, j].frameNumber == 2)
                                    {
                                        world.getTile()[i, j].frameX = 0;
                                        world.getTile()[i, j].frameY = 44;
                                    }
                                }
                                else
                                {
                                    if (num4 != num9)
                                    {
                                        if (world.getTile()[i, j].frameNumber == 0)
                                        {
                                            world.getTile()[i, j].frameX = 0;
                                            world.getTile()[i, j].frameY = 132;
                                        }
                                        if (world.getTile()[i, j].frameNumber == 1)
                                        {
                                            world.getTile()[i, j].frameX = 0;
                                            world.getTile()[i, j].frameY = 154;
                                        }
                                        if (world.getTile()[i, j].frameNumber == 2)
                                        {
                                            world.getTile()[i, j].frameX = 0;
                                            world.getTile()[i, j].frameY = 176;
                                        }
                                    }
                                    else
                                    {
                                        if (num5 != num9)
                                        {
                                            if (world.getTile()[i, j].frameNumber == 0)
                                            {
                                                world.getTile()[i, j].frameX = 66;
                                                world.getTile()[i, j].frameY = 132;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 1)
                                            {
                                                world.getTile()[i, j].frameX = 66;
                                                world.getTile()[i, j].frameY = 154;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 2)
                                            {
                                                world.getTile()[i, j].frameX = 66;
                                                world.getTile()[i, j].frameY = 176;
                                            }
                                        }
                                        else
                                        {
                                            if (world.getTile()[i, j].frameNumber == 0)
                                            {
                                                world.getTile()[i, j].frameX = 88;
                                                world.getTile()[i, j].frameY = 132;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 1)
                                            {
                                                world.getTile()[i, j].frameX = 88;
                                                world.getTile()[i, j].frameY = 154;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 2)
                                            {
                                                world.getTile()[i, j].frameX = 88;
                                                world.getTile()[i, j].frameY = 176;
                                            }
                                        }
                                    }
                                }
                            }
                            if ((world.getTile()[i, j].frameX == 66 && (world.getTile()[i, j].frameY == 0 || world.getTile()[i, j].frameY == 22 || world.getTile()[i, j].frameY == 44)) || (world.getTile()[i, j].frameX == 88 && (world.getTile()[i, j].frameY == 66 || world.getTile()[i, j].frameY == 88 || world.getTile()[i, j].frameY == 110)) || (world.getTile()[i, j].frameX == 44 && (world.getTile()[i, j].frameY == 198 || world.getTile()[i, j].frameY == 220 || world.getTile()[i, j].frameY == 242)) || (world.getTile()[i, j].frameX == 66 && (world.getTile()[i, j].frameY == 198 || world.getTile()[i, j].frameY == 220 || world.getTile()[i, j].frameY == 242)))
                            {
                                if (num4 != num9 && num5 != num9)
                                {
                                    WorldGen.KillTile(i, j, world, false, false, false);
                                }
                            }
                            else
                            {
                                if (num7 == -1 || num7 == 23)
                                {
                                    WorldGen.KillTile(i, j, world, false, false, false);
                                }
                                else
                                {
                                    if (num2 != num9 && world.getTile()[i, j].frameY < 198 && ((world.getTile()[i, j].frameX != 22 && world.getTile()[i, j].frameX != 44) || world.getTile()[i, j].frameY < 132))
                                    {
                                        if (num4 == num9 || num5 == num9)
                                        {
                                            if (num7 == num9)
                                            {
                                                if (num4 == num9 && num5 == num9)
                                                {
                                                    if (world.getTile()[i, j].frameNumber == 0)
                                                    {
                                                        world.getTile()[i, j].frameX = 132;
                                                        world.getTile()[i, j].frameY = 132;
                                                    }
                                                    if (world.getTile()[i, j].frameNumber == 1)
                                                    {
                                                        world.getTile()[i, j].frameX = 132;
                                                        world.getTile()[i, j].frameY = 154;
                                                    }
                                                    if (world.getTile()[i, j].frameNumber == 2)
                                                    {
                                                        world.getTile()[i, j].frameX = 132;
                                                        world.getTile()[i, j].frameY = 176;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 == num9)
                                                    {
                                                        if (world.getTile()[i, j].frameNumber == 0)
                                                        {
                                                            world.getTile()[i, j].frameX = 132;
                                                            world.getTile()[i, j].frameY = 0;
                                                        }
                                                        if (world.getTile()[i, j].frameNumber == 1)
                                                        {
                                                            world.getTile()[i, j].frameX = 132;
                                                            world.getTile()[i, j].frameY = 22;
                                                        }
                                                        if (world.getTile()[i, j].frameNumber == 2)
                                                        {
                                                            world.getTile()[i, j].frameX = 132;
                                                            world.getTile()[i, j].frameY = 44;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 == num9)
                                                        {
                                                            if (world.getTile()[i, j].frameNumber == 0)
                                                            {
                                                                world.getTile()[i, j].frameX = 132;
                                                                world.getTile()[i, j].frameY = 66;
                                                            }
                                                            if (world.getTile()[i, j].frameNumber == 1)
                                                            {
                                                                world.getTile()[i, j].frameX = 132;
                                                                world.getTile()[i, j].frameY = 88;
                                                            }
                                                            if (world.getTile()[i, j].frameNumber == 2)
                                                            {
                                                                world.getTile()[i, j].frameX = 132;
                                                                world.getTile()[i, j].frameY = 110;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (num4 == num9 && num5 == num9)
                                                {
                                                    if (world.getTile()[i, j].frameNumber == 0)
                                                    {
                                                        world.getTile()[i, j].frameX = 154;
                                                        world.getTile()[i, j].frameY = 132;
                                                    }
                                                    if (world.getTile()[i, j].frameNumber == 1)
                                                    {
                                                        world.getTile()[i, j].frameX = 154;
                                                        world.getTile()[i, j].frameY = 154;
                                                    }
                                                    if (world.getTile()[i, j].frameNumber == 2)
                                                    {
                                                        world.getTile()[i, j].frameX = 154;
                                                        world.getTile()[i, j].frameY = 176;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 == num9)
                                                    {
                                                        if (world.getTile()[i, j].frameNumber == 0)
                                                        {
                                                            world.getTile()[i, j].frameX = 154;
                                                            world.getTile()[i, j].frameY = 0;
                                                        }
                                                        if (world.getTile()[i, j].frameNumber == 1)
                                                        {
                                                            world.getTile()[i, j].frameX = 154;
                                                            world.getTile()[i, j].frameY = 22;
                                                        }
                                                        if (world.getTile()[i, j].frameNumber == 2)
                                                        {
                                                            world.getTile()[i, j].frameX = 154;
                                                            world.getTile()[i, j].frameY = 44;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 == num9)
                                                        {
                                                            if (world.getTile()[i, j].frameNumber == 0)
                                                            {
                                                                world.getTile()[i, j].frameX = 154;
                                                                world.getTile()[i, j].frameY = 66;
                                                            }
                                                            if (world.getTile()[i, j].frameNumber == 1)
                                                            {
                                                                world.getTile()[i, j].frameX = 154;
                                                                world.getTile()[i, j].frameY = 88;
                                                            }
                                                            if (world.getTile()[i, j].frameNumber == 2)
                                                            {
                                                                world.getTile()[i, j].frameX = 154;
                                                                world.getTile()[i, j].frameY = 110;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (world.getTile()[i, j].frameNumber == 0)
                                            {
                                                world.getTile()[i, j].frameX = 110;
                                                world.getTile()[i, j].frameY = 0;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 1)
                                            {
                                                world.getTile()[i, j].frameX = 110;
                                                world.getTile()[i, j].frameY = 22;
                                            }
                                            if (world.getTile()[i, j].frameNumber == 2)
                                            {
                                                world.getTile()[i, j].frameX = 110;
                                                world.getTile()[i, j].frameY = 44;
                                            }
                                        }
                                    }
                                }
                            }
                            rectangle.X = (int)world.getTile()[i, j].frameX;
                            rectangle.Y = (int)world.getTile()[i, j].frameY;
                        }
                        if (Statics.tileFrameImportant[(int)world.getTile()[i, j].type])
                        {
                            return;
                        }
                        int num22 = 0;
                        if (resetFrame)
                        {
                            num22 = WorldGen.genRand.Next(0, 3);
                            world.getTile()[i, j].frameNumber = (byte)num22;
                        }
                        else
                        {
                            num22 = (int)world.getTile()[i, j].frameNumber;
                        }
                        if (num9 == 0)
                        {
                            for (int n = 0; n < 80; n++)
                            {
                                if (n == 1 || n == 6 || n == 7 || n == 8 || n == 9 || n == 22 || n == 25 || n == 37 || n == 40 || n == 53 || n == 56)
                                {
                                    if (num2 == n)
                                    {
                                        WorldGen.TileFrame(i, j - 1, world, false, false);
                                        if (WorldGen.mergeDown)
                                        {
                                            num2 = num9;
                                        }
                                    }
                                    if (num7 == n)
                                    {
                                        WorldGen.TileFrame(i, j + 1, world, false, false);
                                        if (WorldGen.mergeUp)
                                        {
                                            num7 = num9;
                                        }
                                    }
                                    if (num4 == n)
                                    {
                                        WorldGen.TileFrame(i - 1, j, world, false, false);
                                        if (WorldGen.mergeRight)
                                        {
                                            num4 = num9;
                                        }
                                    }
                                    if (num5 == n)
                                    {
                                        WorldGen.TileFrame(i + 1, j, world, false, false);
                                        if (WorldGen.mergeLeft)
                                        {
                                            num5 = num9;
                                        }
                                    }
                                    if (num == n)
                                    {
                                        num = num9;
                                    }
                                    if (num3 == n)
                                    {
                                        num3 = num9;
                                    }
                                    if (num6 == n)
                                    {
                                        num6 = num9;
                                    }
                                    if (num8 == n)
                                    {
                                        num8 = num9;
                                    }
                                }
                            }
                            if (num2 == 2)
                            {
                                num2 = num9;
                            }
                            if (num7 == 2)
                            {
                                num7 = num9;
                            }
                            if (num4 == 2)
                            {
                                num4 = num9;
                            }
                            if (num5 == 2)
                            {
                                num5 = num9;
                            }
                            if (num == 2)
                            {
                                num = num9;
                            }
                            if (num3 == 2)
                            {
                                num3 = num9;
                            }
                            if (num6 == 2)
                            {
                                num6 = num9;
                            }
                            if (num8 == 2)
                            {
                                num8 = num9;
                            }
                            if (num2 == 23)
                            {
                                num2 = num9;
                            }
                            if (num7 == 23)
                            {
                                num7 = num9;
                            }
                            if (num4 == 23)
                            {
                                num4 = num9;
                            }
                            if (num5 == 23)
                            {
                                num5 = num9;
                            }
                            if (num == 23)
                            {
                                num = num9;
                            }
                            if (num3 == 23)
                            {
                                num3 = num9;
                            }
                            if (num6 == 23)
                            {
                                num6 = num9;
                            }
                            if (num8 == 23)
                            {
                                num8 = num9;
                            }
                        }
                        else
                        {
                            if (num9 == 57)
                            {
                                if (num2 == 58)
                                {
                                    WorldGen.TileFrame(i, j - 1, world, false, false);
                                    if (WorldGen.mergeDown)
                                    {
                                        num2 = num9;
                                    }
                                }
                                if (num7 == 58)
                                {
                                    WorldGen.TileFrame(i, j + 1, world, false, false);
                                    if (WorldGen.mergeUp)
                                    {
                                        num7 = num9;
                                    }
                                }
                                if (num4 == 58)
                                {
                                    WorldGen.TileFrame(i - 1, j, world, false, false);
                                    if (WorldGen.mergeRight)
                                    {
                                        num4 = num9;
                                    }
                                }
                                if (num5 == 58)
                                {
                                    WorldGen.TileFrame(i + 1, j, world, false, false);
                                    if (WorldGen.mergeLeft)
                                    {
                                        num5 = num9;
                                    }
                                }
                                if (num == 58)
                                {
                                    num = num9;
                                }
                                if (num3 == 58)
                                {
                                    num3 = num9;
                                }
                                if (num6 == 58)
                                {
                                    num6 = num9;
                                }
                                if (num8 == 58)
                                {
                                    num8 = num9;
                                }
                            }
                            else
                            {
                                if (num9 == 59)
                                {
                                    if (num2 == 60)
                                    {
                                        num2 = num9;
                                    }
                                    if (num7 == 60)
                                    {
                                        num7 = num9;
                                    }
                                    if (num4 == 60)
                                    {
                                        num4 = num9;
                                    }
                                    if (num5 == 60)
                                    {
                                        num5 = num9;
                                    }
                                    if (num == 60)
                                    {
                                        num = num9;
                                    }
                                    if (num3 == 60)
                                    {
                                        num3 = num9;
                                    }
                                    if (num6 == 60)
                                    {
                                        num6 = num9;
                                    }
                                    if (num8 == 60)
                                    {
                                        num8 = num9;
                                    }
                                    if (num2 == 70)
                                    {
                                        num2 = num9;
                                    }
                                    if (num7 == 70)
                                    {
                                        num7 = num9;
                                    }
                                    if (num4 == 70)
                                    {
                                        num4 = num9;
                                    }
                                    if (num5 == 70)
                                    {
                                        num5 = num9;
                                    }
                                    if (num == 70)
                                    {
                                        num = num9;
                                    }
                                    if (num3 == 70)
                                    {
                                        num3 = num9;
                                    }
                                    if (num6 == 70)
                                    {
                                        num6 = num9;
                                    }
                                    if (num8 == 70)
                                    {
                                        num8 = num9;
                                    }
                                }
                                else
                                {
                                    if (num9 == 1)
                                    {
                                        if (num2 == 59)
                                        {
                                            WorldGen.TileFrame(i, j - 1, world, false, false);
                                            if (WorldGen.mergeDown)
                                            {
                                                num2 = num9;
                                            }
                                        }
                                        if (num7 == 59)
                                        {
                                            WorldGen.TileFrame(i, j + 1, world, false, false);
                                            if (WorldGen.mergeUp)
                                            {
                                                num7 = num9;
                                            }
                                        }
                                        if (num4 == 59)
                                        {
                                            WorldGen.TileFrame(i - 1, j, world, false, false);
                                            if (WorldGen.mergeRight)
                                            {
                                                num4 = num9;
                                            }
                                        }
                                        if (num5 == 59)
                                        {
                                            WorldGen.TileFrame(i + 1, j, world, false, false);
                                            if (WorldGen.mergeLeft)
                                            {
                                                num5 = num9;
                                            }
                                        }
                                        if (num == 59)
                                        {
                                            num = num9;
                                        }
                                        if (num3 == 59)
                                        {
                                            num3 = num9;
                                        }
                                        if (num6 == 59)
                                        {
                                            num6 = num9;
                                        }
                                        if (num8 == 59)
                                        {
                                            num8 = num9;
                                        }
                                    }
                                }
                            }
                        }
                        if (num9 == 1 || num9 == 6 || num9 == 7 || num9 == 8 || num9 == 9 || num9 == 22 || num9 == 25 || num9 == 37 || num9 == 40 || num9 == 53 || num9 == 56)
                        {
                            for (int num23 = 0; num23 < 80; num23++)
                            {
                                if (num23 == 1 || num23 == 6 || num23 == 7 || num23 == 8 || num23 == 9 || num23 == 22 || num23 == 25 || num23 == 37 || num23 == 40 || num23 == 53 || num23 == 56)
                                {
                                    if (num2 == 0)
                                    {
                                        num2 = -2;
                                    }
                                    if (num7 == 0)
                                    {
                                        num7 = -2;
                                    }
                                    if (num4 == 0)
                                    {
                                        num4 = -2;
                                    }
                                    if (num5 == 0)
                                    {
                                        num5 = -2;
                                    }
                                    if (num == 0)
                                    {
                                        num = -2;
                                    }
                                    if (num3 == 0)
                                    {
                                        num3 = -2;
                                    }
                                    if (num6 == 0)
                                    {
                                        num6 = -2;
                                    }
                                    if (num8 == 0)
                                    {
                                        num8 = -2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (num9 == 58)
                            {
                                if (num2 == 57)
                                {
                                    num2 = -2;
                                }
                                if (num7 == 57)
                                {
                                    num7 = -2;
                                }
                                if (num4 == 57)
                                {
                                    num4 = -2;
                                }
                                if (num5 == 57)
                                {
                                    num5 = -2;
                                }
                                if (num == 57)
                                {
                                    num = -2;
                                }
                                if (num3 == 57)
                                {
                                    num3 = -2;
                                }
                                if (num6 == 57)
                                {
                                    num6 = -2;
                                }
                                if (num8 == 57)
                                {
                                    num8 = -2;
                                }
                            }
                            else
                            {
                                if (num9 == 59)
                                {
                                    if (num2 == 1)
                                    {
                                        num2 = -2;
                                    }
                                    if (num7 == 1)
                                    {
                                        num7 = -2;
                                    }
                                    if (num4 == 1)
                                    {
                                        num4 = -2;
                                    }
                                    if (num5 == 1)
                                    {
                                        num5 = -2;
                                    }
                                    if (num == 1)
                                    {
                                        num = -2;
                                    }
                                    if (num3 == 1)
                                    {
                                        num3 = -2;
                                    }
                                    if (num6 == 1)
                                    {
                                        num6 = -2;
                                    }
                                    if (num8 == 1)
                                    {
                                        num8 = -2;
                                    }
                                }
                            }
                        }
                        if (num9 == 32 && num7 == 23)
                        {
                            num7 = num9;
                        }
                        if (num9 == 69 && num7 == 60)
                        {
                            num7 = num9;
                        }
                        if (num9 == 51)
                        {
                            if (num2 > -1 && !Statics.tileNoAttach[num2])
                            {
                                num2 = num9;
                            }
                            if (num7 > -1 && !Statics.tileNoAttach[num7])
                            {
                                num7 = num9;
                            }
                            if (num4 > -1 && !Statics.tileNoAttach[num4])
                            {
                                num4 = num9;
                            }
                            if (num5 > -1 && !Statics.tileNoAttach[num5])
                            {
                                num5 = num9;
                            }
                            if (num > -1 && !Statics.tileNoAttach[num])
                            {
                                num = num9;
                            }
                            if (num3 > -1 && !Statics.tileNoAttach[num3])
                            {
                                num3 = num9;
                            }
                            if (num6 > -1 && !Statics.tileNoAttach[num6])
                            {
                                num6 = num9;
                            }
                            if (num8 > -1 && !Statics.tileNoAttach[num8])
                            {
                                num8 = num9;
                            }
                        }
                        if (num2 > -1 && !Statics.tileSolid[num2] && num2 != num9)
                        {
                            num2 = -1;
                        }
                        if (num7 > -1 && !Statics.tileSolid[num7] && num7 != num9)
                        {
                            num7 = -1;
                        }
                        if (num4 > -1 && !Statics.tileSolid[num4] && num4 != num9)
                        {
                            num4 = -1;
                        }
                        if (num5 > -1 && !Statics.tileSolid[num5] && num5 != num9)
                        {
                            num5 = -1;
                        }
                        if (num > -1 && !Statics.tileSolid[num] && num != num9)
                        {
                            num = -1;
                        }
                        if (num3 > -1 && !Statics.tileSolid[num3] && num3 != num9)
                        {
                            num3 = -1;
                        }
                        if (num6 > -1 && !Statics.tileSolid[num6] && num6 != num9)
                        {
                            num6 = -1;
                        }
                        if (num8 > -1 && !Statics.tileSolid[num8] && num8 != num9)
                        {
                            num8 = -1;
                        }
                        if (num9 == 2 || num9 == 23 || num9 == 60 || num9 == 70)
                        {
                            int num24 = 0;
                            if (num9 == 60 || num9 == 70)
                            {
                                num24 = 59;
                            }
                            else
                            {
                                if (num9 == 2)
                                {
                                    if (num2 == 23)
                                    {
                                        num2 = num24;
                                    }
                                    if (num7 == 23)
                                    {
                                        num7 = num24;
                                    }
                                    if (num4 == 23)
                                    {
                                        num4 = num24;
                                    }
                                    if (num5 == 23)
                                    {
                                        num5 = num24;
                                    }
                                    if (num == 23)
                                    {
                                        num = num24;
                                    }
                                    if (num3 == 23)
                                    {
                                        num3 = num24;
                                    }
                                    if (num6 == 23)
                                    {
                                        num6 = num24;
                                    }
                                    if (num8 == 23)
                                    {
                                        num8 = num24;
                                    }
                                }
                                else
                                {
                                    if (num9 == 23)
                                    {
                                        if (num2 == 2)
                                        {
                                            num2 = num24;
                                        }
                                        if (num7 == 2)
                                        {
                                            num7 = num24;
                                        }
                                        if (num4 == 2)
                                        {
                                            num4 = num24;
                                        }
                                        if (num5 == 2)
                                        {
                                            num5 = num24;
                                        }
                                        if (num == 2)
                                        {
                                            num = num24;
                                        }
                                        if (num3 == 2)
                                        {
                                            num3 = num24;
                                        }
                                        if (num6 == 2)
                                        {
                                            num6 = num24;
                                        }
                                        if (num8 == 2)
                                        {
                                            num8 = num24;
                                        }
                                    }
                                }
                            }
                            if (num2 != num9 && num2 != num24 && (num7 == num9 || num7 == num24))
                            {
                                if (num4 == num24 && num5 == num9)
                                {
                                    if (num22 == 0)
                                    {
                                        rectangle.X = 0;
                                        rectangle.Y = 198;
                                    }
                                    if (num22 == 1)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 198;
                                    }
                                    if (num22 == 2)
                                    {
                                        rectangle.X = 36;
                                        rectangle.Y = 198;
                                    }
                                }
                                else
                                {
                                    if (num4 == num9 && num5 == num24)
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 54;
                                            rectangle.Y = 198;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 72;
                                            rectangle.Y = 198;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 90;
                                            rectangle.Y = 198;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (num7 != num9 && num7 != num24 && (num2 == num9 || num2 == num24))
                                {
                                    if (num4 == num24 && num5 == num9)
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 0;
                                            rectangle.Y = 216;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 18;
                                            rectangle.Y = 216;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 216;
                                        }
                                    }
                                    else
                                    {
                                        if (num4 == num9 && num5 == num24)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 216;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 216;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 90;
                                                rectangle.Y = 216;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (num4 != num9 && num4 != num24 && (num5 == num9 || num5 == num24))
                                    {
                                        if (num2 == num24 && num7 == num9)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 144;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 162;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 180;
                                            }
                                        }
                                        else
                                        {
                                            if (num7 == num9 && num5 == num2)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 90;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 108;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 126;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (num5 != num9 && num5 != num24 && (num4 == num9 || num4 == num24))
                                        {
                                            if (num2 == num24 && num7 == num9)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 90;
                                                    rectangle.Y = 144;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 90;
                                                    rectangle.Y = 162;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 90;
                                                    rectangle.Y = 180;
                                                }
                                            }
                                            else
                                            {
                                                if (num7 == num9 && num5 == num2)
                                                {
                                                    if (num22 == 0)
                                                    {
                                                        rectangle.X = 90;
                                                        rectangle.Y = 90;
                                                    }
                                                    if (num22 == 1)
                                                    {
                                                        rectangle.X = 90;
                                                        rectangle.Y = 108;
                                                    }
                                                    if (num22 == 2)
                                                    {
                                                        rectangle.X = 90;
                                                        rectangle.Y = 126;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num9)
                                            {
                                                if (num != num9 && num3 != num9 && num6 != num9 && num8 != num9)
                                                {
                                                    if (num8 == num24)
                                                    {
                                                        if (num22 == 0)
                                                        {
                                                            rectangle.X = 108;
                                                            rectangle.Y = 324;
                                                        }
                                                        if (num22 == 1)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 324;
                                                        }
                                                        if (num22 == 2)
                                                        {
                                                            rectangle.X = 144;
                                                            rectangle.Y = 324;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num3 == num24)
                                                        {
                                                            if (num22 == 0)
                                                            {
                                                                rectangle.X = 108;
                                                                rectangle.Y = 342;
                                                            }
                                                            if (num22 == 1)
                                                            {
                                                                rectangle.X = 126;
                                                                rectangle.Y = 342;
                                                            }
                                                            if (num22 == 2)
                                                            {
                                                                rectangle.X = 144;
                                                                rectangle.Y = 342;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num6 == num24)
                                                            {
                                                                if (num22 == 0)
                                                                {
                                                                    rectangle.X = 108;
                                                                    rectangle.Y = 360;
                                                                }
                                                                if (num22 == 1)
                                                                {
                                                                    rectangle.X = 126;
                                                                    rectangle.Y = 360;
                                                                }
                                                                if (num22 == 2)
                                                                {
                                                                    rectangle.X = 144;
                                                                    rectangle.Y = 360;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num == num24)
                                                                {
                                                                    if (num22 == 0)
                                                                    {
                                                                        rectangle.X = 108;
                                                                        rectangle.Y = 378;
                                                                    }
                                                                    if (num22 == 1)
                                                                    {
                                                                        rectangle.X = 126;
                                                                        rectangle.Y = 378;
                                                                    }
                                                                    if (num22 == 2)
                                                                    {
                                                                        rectangle.X = 144;
                                                                        rectangle.Y = 378;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num22 == 0)
                                                                    {
                                                                        rectangle.X = 144;
                                                                        rectangle.Y = 234;
                                                                    }
                                                                    if (num22 == 1)
                                                                    {
                                                                        rectangle.X = 198;
                                                                        rectangle.Y = 234;
                                                                    }
                                                                    if (num22 == 2)
                                                                    {
                                                                        rectangle.X = 252;
                                                                        rectangle.Y = 234;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (num != num9 && num8 != num9)
                                                    {
                                                        if (num22 == 0)
                                                        {
                                                            rectangle.X = 36;
                                                            rectangle.Y = 306;
                                                        }
                                                        if (num22 == 1)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 306;
                                                        }
                                                        if (num22 == 2)
                                                        {
                                                            rectangle.X = 72;
                                                            rectangle.Y = 306;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num3 != num9 && num6 != num9)
                                                        {
                                                            if (num22 == 0)
                                                            {
                                                                rectangle.X = 90;
                                                                rectangle.Y = 306;
                                                            }
                                                            if (num22 == 1)
                                                            {
                                                                rectangle.X = 108;
                                                                rectangle.Y = 306;
                                                            }
                                                            if (num22 == 2)
                                                            {
                                                                rectangle.X = 126;
                                                                rectangle.Y = 306;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num != num9 && num3 == num9 && num6 == num9 && num8 == num9)
                                                            {
                                                                if (num22 == 0)
                                                                {
                                                                    rectangle.X = 54;
                                                                    rectangle.Y = 108;
                                                                }
                                                                if (num22 == 1)
                                                                {
                                                                    rectangle.X = 54;
                                                                    rectangle.Y = 144;
                                                                }
                                                                if (num22 == 2)
                                                                {
                                                                    rectangle.X = 54;
                                                                    rectangle.Y = 180;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num == num9 && num3 != num9 && num6 == num9 && num8 == num9)
                                                                {
                                                                    if (num22 == 0)
                                                                    {
                                                                        rectangle.X = 36;
                                                                        rectangle.Y = 108;
                                                                    }
                                                                    if (num22 == 1)
                                                                    {
                                                                        rectangle.X = 36;
                                                                        rectangle.Y = 144;
                                                                    }
                                                                    if (num22 == 2)
                                                                    {
                                                                        rectangle.X = 36;
                                                                        rectangle.Y = 180;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num == num9 && num3 == num9 && num6 != num9 && num8 == num9)
                                                                    {
                                                                        if (num22 == 0)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 90;
                                                                        }
                                                                        if (num22 == 1)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 126;
                                                                        }
                                                                        if (num22 == 2)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 162;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num == num9 && num3 == num9 && num6 == num9 && num8 != num9)
                                                                        {
                                                                            if (num22 == 0)
                                                                            {
                                                                                rectangle.X = 36;
                                                                                rectangle.Y = 90;
                                                                            }
                                                                            if (num22 == 1)
                                                                            {
                                                                                rectangle.X = 36;
                                                                                rectangle.Y = 126;
                                                                            }
                                                                            if (num22 == 2)
                                                                            {
                                                                                rectangle.X = 36;
                                                                                rectangle.Y = 162;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (num2 == num9 && num7 == num24 && num4 == num9 && num5 == num9 && num == -1 && num3 == -1)
                                                {
                                                    if (num22 == 0)
                                                    {
                                                        rectangle.X = 108;
                                                        rectangle.Y = 18;
                                                    }
                                                    if (num22 == 1)
                                                    {
                                                        rectangle.X = 126;
                                                        rectangle.Y = 18;
                                                    }
                                                    if (num22 == 2)
                                                    {
                                                        rectangle.X = 144;
                                                        rectangle.Y = 18;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num2 == num24 && num7 == num9 && num4 == num9 && num5 == num9 && num6 == -1 && num8 == -1)
                                                    {
                                                        if (num22 == 0)
                                                        {
                                                            rectangle.X = 108;
                                                            rectangle.Y = 36;
                                                        }
                                                        if (num22 == 1)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 36;
                                                        }
                                                        if (num22 == 2)
                                                        {
                                                            rectangle.X = 144;
                                                            rectangle.Y = 36;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num2 == num9 && num7 == num9 && num4 == num24 && num5 == num9 && num3 == -1 && num8 == -1)
                                                        {
                                                            if (num22 == 0)
                                                            {
                                                                rectangle.X = 198;
                                                                rectangle.Y = 0;
                                                            }
                                                            if (num22 == 1)
                                                            {
                                                                rectangle.X = 198;
                                                                rectangle.Y = 18;
                                                            }
                                                            if (num22 == 2)
                                                            {
                                                                rectangle.X = 198;
                                                                rectangle.Y = 36;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num24 && num == -1 && num6 == -1)
                                                            {
                                                                if (num22 == 0)
                                                                {
                                                                    rectangle.X = 180;
                                                                    rectangle.Y = 0;
                                                                }
                                                                if (num22 == 1)
                                                                {
                                                                    rectangle.X = 180;
                                                                    rectangle.Y = 18;
                                                                }
                                                                if (num22 == 2)
                                                                {
                                                                    rectangle.X = 180;
                                                                    rectangle.Y = 36;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num2 == num9 && num7 == num24 && num4 == num9 && num5 == num9)
                                                                {
                                                                    if (num3 != -1)
                                                                    {
                                                                        if (num22 == 0)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 108;
                                                                        }
                                                                        if (num22 == 1)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 144;
                                                                        }
                                                                        if (num22 == 2)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 180;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num != -1)
                                                                        {
                                                                            if (num22 == 0)
                                                                            {
                                                                                rectangle.X = 36;
                                                                                rectangle.Y = 108;
                                                                            }
                                                                            if (num22 == 1)
                                                                            {
                                                                                rectangle.X = 36;
                                                                                rectangle.Y = 144;
                                                                            }
                                                                            if (num22 == 2)
                                                                            {
                                                                                rectangle.X = 36;
                                                                                rectangle.Y = 180;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num2 == num24 && num7 == num9 && num4 == num9 && num5 == num9)
                                                                    {
                                                                        if (num8 != -1)
                                                                        {
                                                                            if (num22 == 0)
                                                                            {
                                                                                rectangle.X = 54;
                                                                                rectangle.Y = 90;
                                                                            }
                                                                            if (num22 == 1)
                                                                            {
                                                                                rectangle.X = 54;
                                                                                rectangle.Y = 126;
                                                                            }
                                                                            if (num22 == 2)
                                                                            {
                                                                                rectangle.X = 54;
                                                                                rectangle.Y = 162;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num6 != -1)
                                                                            {
                                                                                if (num22 == 0)
                                                                                {
                                                                                    rectangle.X = 36;
                                                                                    rectangle.Y = 90;
                                                                                }
                                                                                if (num22 == 1)
                                                                                {
                                                                                    rectangle.X = 36;
                                                                                    rectangle.Y = 126;
                                                                                }
                                                                                if (num22 == 2)
                                                                                {
                                                                                    rectangle.X = 36;
                                                                                    rectangle.Y = 162;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num24)
                                                                        {
                                                                            if (num != -1)
                                                                            {
                                                                                if (num22 == 0)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 90;
                                                                                }
                                                                                if (num22 == 1)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 126;
                                                                                }
                                                                                if (num22 == 2)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 162;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num6 != -1)
                                                                                {
                                                                                    if (num22 == 0)
                                                                                    {
                                                                                        rectangle.X = 54;
                                                                                        rectangle.Y = 108;
                                                                                    }
                                                                                    if (num22 == 1)
                                                                                    {
                                                                                        rectangle.X = 54;
                                                                                        rectangle.Y = 144;
                                                                                    }
                                                                                    if (num22 == 2)
                                                                                    {
                                                                                        rectangle.X = 54;
                                                                                        rectangle.Y = 180;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num2 == num9 && num7 == num9 && num4 == num24 && num5 == num9)
                                                                            {
                                                                                if (num3 != -1)
                                                                                {
                                                                                    if (num22 == 0)
                                                                                    {
                                                                                        rectangle.X = 36;
                                                                                        rectangle.Y = 90;
                                                                                    }
                                                                                    if (num22 == 1)
                                                                                    {
                                                                                        rectangle.X = 36;
                                                                                        rectangle.Y = 126;
                                                                                    }
                                                                                    if (num22 == 2)
                                                                                    {
                                                                                        rectangle.X = 36;
                                                                                        rectangle.Y = 162;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num8 != -1)
                                                                                    {
                                                                                        if (num22 == 0)
                                                                                        {
                                                                                            rectangle.X = 36;
                                                                                            rectangle.Y = 108;
                                                                                        }
                                                                                        if (num22 == 1)
                                                                                        {
                                                                                            rectangle.X = 36;
                                                                                            rectangle.Y = 144;
                                                                                        }
                                                                                        if (num22 == 2)
                                                                                        {
                                                                                            rectangle.X = 36;
                                                                                            rectangle.Y = 180;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if ((num2 == num24 && num7 == num9 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num24 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num24 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num24))
                                                                                {
                                                                                    if (num22 == 0)
                                                                                    {
                                                                                        rectangle.X = 18;
                                                                                        rectangle.Y = 18;
                                                                                    }
                                                                                    if (num22 == 1)
                                                                                    {
                                                                                        rectangle.X = 36;
                                                                                        rectangle.Y = 18;
                                                                                    }
                                                                                    if (num22 == 2)
                                                                                    {
                                                                                        rectangle.X = 54;
                                                                                        rectangle.Y = 18;
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
                            if ((num2 == num9 || num2 == num24) && (num7 == num9 || num7 == num24) && (num4 == num9 || num4 == num24) && (num5 == num9 || num5 == num24))
                            {
                                if (num != num9 && num != num24 && (num3 == num9 || num3 == num24) && (num6 == num9 || num6 == num24) && (num8 == num9 || num8 == num24))
                                {
                                    if (num22 == 0)
                                    {
                                        rectangle.X = 54;
                                        rectangle.Y = 108;
                                    }
                                    if (num22 == 1)
                                    {
                                        rectangle.X = 54;
                                        rectangle.Y = 144;
                                    }
                                    if (num22 == 2)
                                    {
                                        rectangle.X = 54;
                                        rectangle.Y = 180;
                                    }
                                }
                                else
                                {
                                    if (num3 != num9 && num3 != num24 && (num == num9 || num == num24) && (num6 == num9 || num6 == num24) && (num8 == num9 || num8 == num24))
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 108;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 144;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 180;
                                        }
                                    }
                                    else
                                    {
                                        if (num6 != num9 && num6 != num24 && (num == num9 || num == num24) && (num3 == num9 || num3 == num24) && (num8 == num9 || num8 == num24))
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 90;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 126;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 162;
                                            }
                                        }
                                        else
                                        {
                                            if (num8 != num9 && num8 != num24 && (num == num9 || num == num24) && (num6 == num9 || num6 == num24) && (num3 == num9 || num3 == num24))
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 36;
                                                    rectangle.Y = 90;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 36;
                                                    rectangle.Y = 126;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 36;
                                                    rectangle.Y = 162;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (num2 != num24 && num2 != num9 && num7 == num9 && num4 != num24 && num4 != num9 && num5 == num9 && num8 != num24 && num8 != num9)
                            {
                                if (num22 == 0)
                                {
                                    rectangle.X = 90;
                                    rectangle.Y = 270;
                                }
                                if (num22 == 1)
                                {
                                    rectangle.X = 108;
                                    rectangle.Y = 270;
                                }
                                if (num22 == 2)
                                {
                                    rectangle.X = 126;
                                    rectangle.Y = 270;
                                }
                            }
                            else
                            {
                                if (num2 != num24 && num2 != num9 && num7 == num9 && num4 == num9 && num5 != num24 && num5 != num9 && num6 != num24 && num6 != num9)
                                {
                                    if (num22 == 0)
                                    {
                                        rectangle.X = 144;
                                        rectangle.Y = 270;
                                    }
                                    if (num22 == 1)
                                    {
                                        rectangle.X = 162;
                                        rectangle.Y = 270;
                                    }
                                    if (num22 == 2)
                                    {
                                        rectangle.X = 180;
                                        rectangle.Y = 270;
                                    }
                                }
                                else
                                {
                                    if (num7 != num24 && num7 != num9 && num2 == num9 && num4 != num24 && num4 != num9 && num5 == num9 && num3 != num24 && num3 != num9)
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 90;
                                            rectangle.Y = 288;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 108;
                                            rectangle.Y = 288;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 126;
                                            rectangle.Y = 288;
                                        }
                                    }
                                    else
                                    {
                                        if (num7 != num24 && num7 != num9 && num2 == num9 && num4 == num9 && num5 != num24 && num5 != num9 && num != num24 && num != num9)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 144;
                                                rectangle.Y = 288;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 162;
                                                rectangle.Y = 288;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 180;
                                                rectangle.Y = 288;
                                            }
                                        }
                                        else
                                        {
                                            if (num2 != num9 && num2 != num24 && num7 == num9 && num4 == num9 && num5 == num9 && num6 != num9 && num6 != num24 && num8 != num9 && num8 != num24)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 144;
                                                    rectangle.Y = 216;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 198;
                                                    rectangle.Y = 216;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 252;
                                                    rectangle.Y = 216;
                                                }
                                            }
                                            else
                                            {
                                                if (num7 != num9 && num7 != num24 && num2 == num9 && num4 == num9 && num5 == num9 && num != num9 && num != num24 && num3 != num9 && num3 != num24)
                                                {
                                                    if (num22 == 0)
                                                    {
                                                        rectangle.X = 144;
                                                        rectangle.Y = 252;
                                                    }
                                                    if (num22 == 1)
                                                    {
                                                        rectangle.X = 198;
                                                        rectangle.Y = 252;
                                                    }
                                                    if (num22 == 2)
                                                    {
                                                        rectangle.X = 252;
                                                        rectangle.Y = 252;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 != num9 && num4 != num24 && num7 == num9 && num2 == num9 && num5 == num9 && num3 != num9 && num3 != num24 && num8 != num9 && num8 != num24)
                                                    {
                                                        if (num22 == 0)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 234;
                                                        }
                                                        if (num22 == 1)
                                                        {
                                                            rectangle.X = 180;
                                                            rectangle.Y = 234;
                                                        }
                                                        if (num22 == 2)
                                                        {
                                                            rectangle.X = 234;
                                                            rectangle.Y = 234;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 != num9 && num5 != num24 && num7 == num9 && num2 == num9 && num4 == num9 && num != num9 && num != num24 && num6 != num9 && num6 != num24)
                                                        {
                                                            if (num22 == 0)
                                                            {
                                                                rectangle.X = 162;
                                                                rectangle.Y = 234;
                                                            }
                                                            if (num22 == 1)
                                                            {
                                                                rectangle.X = 216;
                                                                rectangle.Y = 234;
                                                            }
                                                            if (num22 == 2)
                                                            {
                                                                rectangle.X = 270;
                                                                rectangle.Y = 234;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num2 != num24 && num2 != num9 && (num7 == num24 || num7 == num9) && num4 == num24 && num5 == num24)
                                                            {
                                                                if (num22 == 0)
                                                                {
                                                                    rectangle.X = 36;
                                                                    rectangle.Y = 270;
                                                                }
                                                                if (num22 == 1)
                                                                {
                                                                    rectangle.X = 54;
                                                                    rectangle.Y = 270;
                                                                }
                                                                if (num22 == 2)
                                                                {
                                                                    rectangle.X = 72;
                                                                    rectangle.Y = 270;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num7 != num24 && num7 != num9 && (num2 == num24 || num2 == num9) && num4 == num24 && num5 == num24)
                                                                {
                                                                    if (num22 == 0)
                                                                    {
                                                                        rectangle.X = 36;
                                                                        rectangle.Y = 288;
                                                                    }
                                                                    if (num22 == 1)
                                                                    {
                                                                        rectangle.X = 54;
                                                                        rectangle.Y = 288;
                                                                    }
                                                                    if (num22 == 2)
                                                                    {
                                                                        rectangle.X = 72;
                                                                        rectangle.Y = 288;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num4 != num24 && num4 != num9 && (num5 == num24 || num5 == num9) && num2 == num24 && num7 == num24)
                                                                    {
                                                                        if (num22 == 0)
                                                                        {
                                                                            rectangle.X = 0;
                                                                            rectangle.Y = 270;
                                                                        }
                                                                        if (num22 == 1)
                                                                        {
                                                                            rectangle.X = 0;
                                                                            rectangle.Y = 288;
                                                                        }
                                                                        if (num22 == 2)
                                                                        {
                                                                            rectangle.X = 0;
                                                                            rectangle.Y = 306;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num5 != num24 && num5 != num9 && (num4 == num24 || num4 == num9) && num2 == num24 && num7 == num24)
                                                                        {
                                                                            if (num22 == 0)
                                                                            {
                                                                                rectangle.X = 18;
                                                                                rectangle.Y = 270;
                                                                            }
                                                                            if (num22 == 1)
                                                                            {
                                                                                rectangle.X = 18;
                                                                                rectangle.Y = 288;
                                                                            }
                                                                            if (num22 == 2)
                                                                            {
                                                                                rectangle.X = 18;
                                                                                rectangle.Y = 306;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num2 == num9 && num7 == num24 && num4 == num24 && num5 == num24)
                                                                            {
                                                                                if (num22 == 0)
                                                                                {
                                                                                    rectangle.X = 198;
                                                                                    rectangle.Y = 288;
                                                                                }
                                                                                if (num22 == 1)
                                                                                {
                                                                                    rectangle.X = 216;
                                                                                    rectangle.Y = 288;
                                                                                }
                                                                                if (num22 == 2)
                                                                                {
                                                                                    rectangle.X = 234;
                                                                                    rectangle.Y = 288;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num2 == num24 && num7 == num9 && num4 == num24 && num5 == num24)
                                                                                {
                                                                                    if (num22 == 0)
                                                                                    {
                                                                                        rectangle.X = 198;
                                                                                        rectangle.Y = 270;
                                                                                    }
                                                                                    if (num22 == 1)
                                                                                    {
                                                                                        rectangle.X = 216;
                                                                                        rectangle.Y = 270;
                                                                                    }
                                                                                    if (num22 == 2)
                                                                                    {
                                                                                        rectangle.X = 234;
                                                                                        rectangle.Y = 270;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num2 == num24 && num7 == num24 && num4 == num9 && num5 == num24)
                                                                                    {
                                                                                        if (num22 == 0)
                                                                                        {
                                                                                            rectangle.X = 198;
                                                                                            rectangle.Y = 306;
                                                                                        }
                                                                                        if (num22 == 1)
                                                                                        {
                                                                                            rectangle.X = 216;
                                                                                            rectangle.Y = 306;
                                                                                        }
                                                                                        if (num22 == 2)
                                                                                        {
                                                                                            rectangle.X = 234;
                                                                                            rectangle.Y = 306;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (num2 == num24 && num7 == num24 && num4 == num24 && num5 == num9)
                                                                                        {
                                                                                            if (num22 == 0)
                                                                                            {
                                                                                                rectangle.X = 144;
                                                                                                rectangle.Y = 306;
                                                                                            }
                                                                                            if (num22 == 1)
                                                                                            {
                                                                                                rectangle.X = 162;
                                                                                                rectangle.Y = 306;
                                                                                            }
                                                                                            if (num22 == 2)
                                                                                            {
                                                                                                rectangle.X = 180;
                                                                                                rectangle.Y = 306;
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
                            if (num2 != num9 && num2 != num24 && num7 == num9 && num4 == num9 && num5 == num9)
                            {
                                if ((num6 == num24 || num6 == num9) && num8 != num24 && num8 != num9)
                                {
                                    if (num22 == 0)
                                    {
                                        rectangle.X = 0;
                                        rectangle.Y = 324;
                                    }
                                    if (num22 == 1)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 324;
                                    }
                                    if (num22 == 2)
                                    {
                                        rectangle.X = 36;
                                        rectangle.Y = 324;
                                    }
                                }
                                else
                                {
                                    if ((num8 == num24 || num8 == num9) && num6 != num24 && num6 != num9)
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 54;
                                            rectangle.Y = 324;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 72;
                                            rectangle.Y = 324;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 90;
                                            rectangle.Y = 324;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (num7 != num9 && num7 != num24 && num2 == num9 && num4 == num9 && num5 == num9)
                                {
                                    if ((num == num24 || num == num9) && num3 != num24 && num3 != num9)
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 0;
                                            rectangle.Y = 342;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 18;
                                            rectangle.Y = 342;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 342;
                                        }
                                    }
                                    else
                                    {
                                        if ((num3 == num24 || num3 == num9) && num != num24 && num != num9)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 342;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 342;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 90;
                                                rectangle.Y = 342;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (num4 != num9 && num4 != num24 && num2 == num9 && num7 == num9 && num5 == num9)
                                    {
                                        if ((num3 == num24 || num3 == num9) && num8 != num24 && num8 != num9)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 360;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 360;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 90;
                                                rectangle.Y = 360;
                                            }
                                        }
                                        else
                                        {
                                            if ((num8 == num24 || num8 == num9) && num3 != num24 && num3 != num9)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 0;
                                                    rectangle.Y = 360;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 18;
                                                    rectangle.Y = 360;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 36;
                                                    rectangle.Y = 360;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (num5 != num9 && num5 != num24 && num2 == num9 && num7 == num9 && num4 == num9)
                                        {
                                            if ((num == num24 || num == num9) && num6 != num24 && num6 != num9)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 0;
                                                    rectangle.Y = 378;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 18;
                                                    rectangle.Y = 378;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 36;
                                                    rectangle.Y = 378;
                                                }
                                            }
                                            else
                                            {
                                                if ((num6 == num24 || num6 == num9) && num != num24 && num != num9)
                                                {
                                                    if (num22 == 0)
                                                    {
                                                        rectangle.X = 54;
                                                        rectangle.Y = 378;
                                                    }
                                                    if (num22 == 1)
                                                    {
                                                        rectangle.X = 72;
                                                        rectangle.Y = 378;
                                                    }
                                                    if (num22 == 2)
                                                    {
                                                        rectangle.X = 90;
                                                        rectangle.Y = 378;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if ((num2 == num9 || num2 == num24) && (num7 == num9 || num7 == num24) && (num4 == num9 || num4 == num24) && (num5 == num9 || num5 == num24) && num != -1 && num3 != -1 && num6 != -1 && num8 != -1)
                            {
                                if (num22 == 0)
                                {
                                    rectangle.X = 18;
                                    rectangle.Y = 18;
                                }
                                if (num22 == 1)
                                {
                                    rectangle.X = 36;
                                    rectangle.Y = 18;
                                }
                                if (num22 == 2)
                                {
                                    rectangle.X = 54;
                                    rectangle.Y = 18;
                                }
                            }
                            if (num2 == num24)
                            {
                                num2 = -2;
                            }
                            if (num7 == num24)
                            {
                                num7 = -2;
                            }
                            if (num4 == num24)
                            {
                                num4 = -2;
                            }
                            if (num5 == num24)
                            {
                                num5 = -2;
                            }
                            if (num == num24)
                            {
                                num = -2;
                            }
                            if (num3 == num24)
                            {
                                num3 = -2;
                            }
                            if (num6 == num24)
                            {
                                num6 = -2;
                            }
                            if (num8 == num24)
                            {
                                num8 = -2;
                            }
                        }
                        if ((num9 == 1 || num9 == 2 || num9 == 6 || num9 == 7 || num9 == 8 || num9 == 9 || num9 == 22 || num9 == 23 || num9 == 25 || num9 == 37 || num9 == 40 || num9 == 53 || num9 == 56 || num9 == 58 || num9 == 59 || num9 == 60 || num9 == 70) && rectangle.X == -1 && rectangle.Y == -1)
                        {
                            if (num2 >= 0 && num2 != num9)
                            {
                                num2 = -1;
                            }
                            if (num7 >= 0 && num7 != num9)
                            {
                                num7 = -1;
                            }
                            if (num4 >= 0 && num4 != num9)
                            {
                                num4 = -1;
                            }
                            if (num5 >= 0 && num5 != num9)
                            {
                                num5 = -1;
                            }
                            if (num2 != -1 && num7 != -1 && num4 != -1 && num5 != -1)
                            {
                                if (num2 == -2 && num7 == num9 && num4 == num9 && num5 == num9)
                                {
                                    if (num22 == 0)
                                    {
                                        rectangle.X = 144;
                                        rectangle.Y = 108;
                                    }
                                    if (num22 == 1)
                                    {
                                        rectangle.X = 162;
                                        rectangle.Y = 108;
                                    }
                                    if (num22 == 2)
                                    {
                                        rectangle.X = 180;
                                        rectangle.Y = 108;
                                    }
                                    WorldGen.mergeUp = true;
                                }
                                else
                                {
                                    if (num2 == num9 && num7 == -2 && num4 == num9 && num5 == num9)
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 144;
                                            rectangle.Y = 90;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 162;
                                            rectangle.Y = 90;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 180;
                                            rectangle.Y = 90;
                                        }
                                        WorldGen.mergeDown = true;
                                    }
                                    else
                                    {
                                        if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == num9)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 162;
                                                rectangle.Y = 126;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 162;
                                                rectangle.Y = 144;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 162;
                                                rectangle.Y = 162;
                                            }
                                            WorldGen.mergeLeft = true;
                                        }
                                        else
                                        {
                                            if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == -2)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 144;
                                                    rectangle.Y = 126;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 144;
                                                    rectangle.Y = 144;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 144;
                                                    rectangle.Y = 162;
                                                }
                                                WorldGen.mergeRight = true;
                                            }
                                            else
                                            {
                                                if (num2 == -2 && num7 == num9 && num4 == -2 && num5 == num9)
                                                {
                                                    if (num22 == 0)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 90;
                                                    }
                                                    if (num22 == 1)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 126;
                                                    }
                                                    if (num22 == 2)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 162;
                                                    }
                                                    WorldGen.mergeUp = true;
                                                    WorldGen.mergeLeft = true;
                                                }
                                                else
                                                {
                                                    if (num2 == -2 && num7 == num9 && num4 == num9 && num5 == -2)
                                                    {
                                                        if (num22 == 0)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 90;
                                                        }
                                                        if (num22 == 1)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 126;
                                                        }
                                                        if (num22 == 2)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 162;
                                                        }
                                                        WorldGen.mergeUp = true;
                                                        WorldGen.mergeRight = true;
                                                    }
                                                    else
                                                    {
                                                        if (num2 == num9 && num7 == -2 && num4 == -2 && num5 == num9)
                                                        {
                                                            if (num22 == 0)
                                                            {
                                                                rectangle.X = 36;
                                                                rectangle.Y = 108;
                                                            }
                                                            if (num22 == 1)
                                                            {
                                                                rectangle.X = 36;
                                                                rectangle.Y = 144;
                                                            }
                                                            if (num22 == 2)
                                                            {
                                                                rectangle.X = 36;
                                                                rectangle.Y = 180;
                                                            }
                                                            WorldGen.mergeDown = true;
                                                            WorldGen.mergeLeft = true;
                                                        }
                                                        else
                                                        {
                                                            if (num2 == num9 && num7 == -2 && num4 == num9 && num5 == -2)
                                                            {
                                                                if (num22 == 0)
                                                                {
                                                                    rectangle.X = 54;
                                                                    rectangle.Y = 108;
                                                                }
                                                                if (num22 == 1)
                                                                {
                                                                    rectangle.X = 54;
                                                                    rectangle.Y = 144;
                                                                }
                                                                if (num22 == 2)
                                                                {
                                                                    rectangle.X = 54;
                                                                    rectangle.Y = 180;
                                                                }
                                                                WorldGen.mergeDown = true;
                                                                WorldGen.mergeRight = true;
                                                            }
                                                            else
                                                            {
                                                                if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == -2)
                                                                {
                                                                    if (num22 == 0)
                                                                    {
                                                                        rectangle.X = 180;
                                                                        rectangle.Y = 126;
                                                                    }
                                                                    if (num22 == 1)
                                                                    {
                                                                        rectangle.X = 180;
                                                                        rectangle.Y = 144;
                                                                    }
                                                                    if (num22 == 2)
                                                                    {
                                                                        rectangle.X = 180;
                                                                        rectangle.Y = 162;
                                                                    }
                                                                    WorldGen.mergeLeft = true;
                                                                    WorldGen.mergeRight = true;
                                                                }
                                                                else
                                                                {
                                                                    if (num2 == -2 && num7 == -2 && num4 == num9 && num5 == num9)
                                                                    {
                                                                        if (num22 == 0)
                                                                        {
                                                                            rectangle.X = 144;
                                                                            rectangle.Y = 180;
                                                                        }
                                                                        if (num22 == 1)
                                                                        {
                                                                            rectangle.X = 162;
                                                                            rectangle.Y = 180;
                                                                        }
                                                                        if (num22 == 2)
                                                                        {
                                                                            rectangle.X = 180;
                                                                            rectangle.Y = 180;
                                                                        }
                                                                        WorldGen.mergeUp = true;
                                                                        WorldGen.mergeDown = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num2 == -2 && num7 == num9 && num4 == -2 && num5 == -2)
                                                                        {
                                                                            if (num22 == 0)
                                                                            {
                                                                                rectangle.X = 198;
                                                                                rectangle.Y = 90;
                                                                            }
                                                                            if (num22 == 1)
                                                                            {
                                                                                rectangle.X = 198;
                                                                                rectangle.Y = 108;
                                                                            }
                                                                            if (num22 == 2)
                                                                            {
                                                                                rectangle.X = 198;
                                                                                rectangle.Y = 126;
                                                                            }
                                                                            WorldGen.mergeUp = true;
                                                                            WorldGen.mergeLeft = true;
                                                                            WorldGen.mergeRight = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num2 == num9 && num7 == -2 && num4 == -2 && num5 == -2)
                                                                            {
                                                                                if (num22 == 0)
                                                                                {
                                                                                    rectangle.X = 198;
                                                                                    rectangle.Y = 144;
                                                                                }
                                                                                if (num22 == 1)
                                                                                {
                                                                                    rectangle.X = 198;
                                                                                    rectangle.Y = 162;
                                                                                }
                                                                                if (num22 == 2)
                                                                                {
                                                                                    rectangle.X = 198;
                                                                                    rectangle.Y = 180;
                                                                                }
                                                                                WorldGen.mergeDown = true;
                                                                                WorldGen.mergeLeft = true;
                                                                                WorldGen.mergeRight = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num2 == -2 && num7 == -2 && num4 == num9 && num5 == -2)
                                                                                {
                                                                                    if (num22 == 0)
                                                                                    {
                                                                                        rectangle.X = 216;
                                                                                        rectangle.Y = 144;
                                                                                    }
                                                                                    if (num22 == 1)
                                                                                    {
                                                                                        rectangle.X = 216;
                                                                                        rectangle.Y = 162;
                                                                                    }
                                                                                    if (num22 == 2)
                                                                                    {
                                                                                        rectangle.X = 216;
                                                                                        rectangle.Y = 180;
                                                                                    }
                                                                                    WorldGen.mergeUp = true;
                                                                                    WorldGen.mergeDown = true;
                                                                                    WorldGen.mergeRight = true;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num2 == -2 && num7 == -2 && num4 == -2 && num5 == num9)
                                                                                    {
                                                                                        if (num22 == 0)
                                                                                        {
                                                                                            rectangle.X = 216;
                                                                                            rectangle.Y = 90;
                                                                                        }
                                                                                        if (num22 == 1)
                                                                                        {
                                                                                            rectangle.X = 216;
                                                                                            rectangle.Y = 108;
                                                                                        }
                                                                                        if (num22 == 2)
                                                                                        {
                                                                                            rectangle.X = 216;
                                                                                            rectangle.Y = 126;
                                                                                        }
                                                                                        WorldGen.mergeUp = true;
                                                                                        WorldGen.mergeDown = true;
                                                                                        WorldGen.mergeLeft = true;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (num2 == -2 && num7 == -2 && num4 == -2 && num5 == -2)
                                                                                        {
                                                                                            if (num22 == 0)
                                                                                            {
                                                                                                rectangle.X = 108;
                                                                                                rectangle.Y = 198;
                                                                                            }
                                                                                            if (num22 == 1)
                                                                                            {
                                                                                                rectangle.X = 126;
                                                                                                rectangle.Y = 198;
                                                                                            }
                                                                                            if (num22 == 2)
                                                                                            {
                                                                                                rectangle.X = 144;
                                                                                                rectangle.Y = 198;
                                                                                            }
                                                                                            WorldGen.mergeUp = true;
                                                                                            WorldGen.mergeDown = true;
                                                                                            WorldGen.mergeLeft = true;
                                                                                            WorldGen.mergeRight = true;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num9)
                                                                                            {
                                                                                                if (num == -2)
                                                                                                {
                                                                                                    if (num22 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 18;
                                                                                                        rectangle.Y = 108;
                                                                                                    }
                                                                                                    if (num22 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 18;
                                                                                                        rectangle.Y = 144;
                                                                                                    }
                                                                                                    if (num22 == 2)
                                                                                                    {
                                                                                                        rectangle.X = 18;
                                                                                                        rectangle.Y = 180;
                                                                                                    }
                                                                                                }
                                                                                                if (num3 == -2)
                                                                                                {
                                                                                                    if (num22 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 0;
                                                                                                        rectangle.Y = 108;
                                                                                                    }
                                                                                                    if (num22 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 0;
                                                                                                        rectangle.Y = 144;
                                                                                                    }
                                                                                                    if (num22 == 2)
                                                                                                    {
                                                                                                        rectangle.X = 0;
                                                                                                        rectangle.Y = 180;
                                                                                                    }
                                                                                                }
                                                                                                if (num6 == -2)
                                                                                                {
                                                                                                    if (num22 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 18;
                                                                                                        rectangle.Y = 90;
                                                                                                    }
                                                                                                    if (num22 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 18;
                                                                                                        rectangle.Y = 126;
                                                                                                    }
                                                                                                    if (num22 == 2)
                                                                                                    {
                                                                                                        rectangle.X = 18;
                                                                                                        rectangle.Y = 162;
                                                                                                    }
                                                                                                }
                                                                                                if (num8 == -2)
                                                                                                {
                                                                                                    if (num22 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 0;
                                                                                                        rectangle.Y = 90;
                                                                                                    }
                                                                                                    if (num22 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 0;
                                                                                                        rectangle.Y = 126;
                                                                                                    }
                                                                                                    if (num22 == 2)
                                                                                                    {
                                                                                                        rectangle.X = 0;
                                                                                                        rectangle.Y = 162;
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
                            else
                            {
                                if (num9 != 2 && num9 != 23 && num9 != 60 && num9 != 70)
                                {
                                    if (num2 == -1 && num7 == -2 && num4 == num9 && num5 == num9)
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 234;
                                            rectangle.Y = 0;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 252;
                                            rectangle.Y = 0;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 270;
                                            rectangle.Y = 0;
                                        }
                                        WorldGen.mergeDown = true;
                                    }
                                    else
                                    {
                                        if (num2 == -2 && num7 == -1 && num4 == num9 && num5 == num9)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 234;
                                                rectangle.Y = 18;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 252;
                                                rectangle.Y = 18;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 270;
                                                rectangle.Y = 18;
                                            }
                                            WorldGen.mergeUp = true;
                                        }
                                        else
                                        {
                                            if (num2 == num9 && num7 == num9 && num4 == -1 && num5 == -2)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 234;
                                                    rectangle.Y = 36;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 252;
                                                    rectangle.Y = 36;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 270;
                                                    rectangle.Y = 36;
                                                }
                                                WorldGen.mergeRight = true;
                                            }
                                            else
                                            {
                                                if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == -1)
                                                {
                                                    if (num22 == 0)
                                                    {
                                                        rectangle.X = 234;
                                                        rectangle.Y = 54;
                                                    }
                                                    if (num22 == 1)
                                                    {
                                                        rectangle.X = 252;
                                                        rectangle.Y = 54;
                                                    }
                                                    if (num22 == 2)
                                                    {
                                                        rectangle.X = 270;
                                                        rectangle.Y = 54;
                                                    }
                                                    WorldGen.mergeLeft = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (num2 != -1 && num7 != -1 && num4 == -1 && num5 == num9)
                                {
                                    if (num2 == -2 && num7 == num9)
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 72;
                                            rectangle.Y = 144;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 72;
                                            rectangle.Y = 162;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 72;
                                            rectangle.Y = 180;
                                        }
                                        WorldGen.mergeUp = true;
                                    }
                                    else
                                    {
                                        if (num7 == -2 && num2 == num9)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 90;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 108;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 126;
                                            }
                                            WorldGen.mergeDown = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (num2 != -1 && num7 != -1 && num4 == num9 && num5 == -1)
                                    {
                                        if (num2 == -2 && num7 == num9)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 90;
                                                rectangle.Y = 144;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 90;
                                                rectangle.Y = 162;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 90;
                                                rectangle.Y = 180;
                                            }
                                            WorldGen.mergeUp = true;
                                        }
                                        else
                                        {
                                            if (num7 == -2 && num2 == num9)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 90;
                                                    rectangle.Y = 90;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 90;
                                                    rectangle.Y = 108;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 90;
                                                    rectangle.Y = 126;
                                                }
                                                WorldGen.mergeDown = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (num2 == -1 && num7 == num9 && num4 != -1 && num5 != -1)
                                        {
                                            if (num4 == -2 && num5 == num9)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 0;
                                                    rectangle.Y = 198;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 18;
                                                    rectangle.Y = 198;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 36;
                                                    rectangle.Y = 198;
                                                }
                                                WorldGen.mergeLeft = true;
                                            }
                                            else
                                            {
                                                if (num5 == -2 && num4 == num9)
                                                {
                                                    if (num22 == 0)
                                                    {
                                                        rectangle.X = 54;
                                                        rectangle.Y = 198;
                                                    }
                                                    if (num22 == 1)
                                                    {
                                                        rectangle.X = 72;
                                                        rectangle.Y = 198;
                                                    }
                                                    if (num22 == 2)
                                                    {
                                                        rectangle.X = 90;
                                                        rectangle.Y = 198;
                                                    }
                                                    WorldGen.mergeRight = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (num2 == num9 && num7 == -1 && num4 != -1 && num5 != -1)
                                            {
                                                if (num4 == -2 && num5 == num9)
                                                {
                                                    if (num22 == 0)
                                                    {
                                                        rectangle.X = 0;
                                                        rectangle.Y = 216;
                                                    }
                                                    if (num22 == 1)
                                                    {
                                                        rectangle.X = 18;
                                                        rectangle.Y = 216;
                                                    }
                                                    if (num22 == 2)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 216;
                                                    }
                                                    WorldGen.mergeLeft = true;
                                                }
                                                else
                                                {
                                                    if (num5 == -2 && num4 == num9)
                                                    {
                                                        if (num22 == 0)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 216;
                                                        }
                                                        if (num22 == 1)
                                                        {
                                                            rectangle.X = 72;
                                                            rectangle.Y = 216;
                                                        }
                                                        if (num22 == 2)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 216;
                                                        }
                                                        WorldGen.mergeRight = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (num2 != -1 && num7 != -1 && num4 == -1 && num5 == -1)
                                                {
                                                    if (num2 == -2 && num7 == -2)
                                                    {
                                                        if (num22 == 0)
                                                        {
                                                            rectangle.X = 108;
                                                            rectangle.Y = 216;
                                                        }
                                                        if (num22 == 1)
                                                        {
                                                            rectangle.X = 108;
                                                            rectangle.Y = 234;
                                                        }
                                                        if (num22 == 2)
                                                        {
                                                            rectangle.X = 108;
                                                            rectangle.Y = 252;
                                                        }
                                                        WorldGen.mergeUp = true;
                                                        WorldGen.mergeDown = true;
                                                    }
                                                    else
                                                    {
                                                        if (num2 == -2)
                                                        {
                                                            if (num22 == 0)
                                                            {
                                                                rectangle.X = 126;
                                                                rectangle.Y = 144;
                                                            }
                                                            if (num22 == 1)
                                                            {
                                                                rectangle.X = 126;
                                                                rectangle.Y = 162;
                                                            }
                                                            if (num22 == 2)
                                                            {
                                                                rectangle.X = 126;
                                                                rectangle.Y = 180;
                                                            }
                                                            WorldGen.mergeUp = true;
                                                        }
                                                        else
                                                        {
                                                            if (num7 == -2)
                                                            {
                                                                if (num22 == 0)
                                                                {
                                                                    rectangle.X = 126;
                                                                    rectangle.Y = 90;
                                                                }
                                                                if (num22 == 1)
                                                                {
                                                                    rectangle.X = 126;
                                                                    rectangle.Y = 108;
                                                                }
                                                                if (num22 == 2)
                                                                {
                                                                    rectangle.X = 126;
                                                                    rectangle.Y = 126;
                                                                }
                                                                WorldGen.mergeDown = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (num2 == -1 && num7 == -1 && num4 != -1 && num5 != -1)
                                                    {
                                                        if (num4 == -2 && num5 == -2)
                                                        {
                                                            if (num22 == 0)
                                                            {
                                                                rectangle.X = 162;
                                                                rectangle.Y = 198;
                                                            }
                                                            if (num22 == 1)
                                                            {
                                                                rectangle.X = 180;
                                                                rectangle.Y = 198;
                                                            }
                                                            if (num22 == 2)
                                                            {
                                                                rectangle.X = 198;
                                                                rectangle.Y = 198;
                                                            }
                                                            WorldGen.mergeLeft = true;
                                                            WorldGen.mergeRight = true;
                                                        }
                                                        else
                                                        {
                                                            if (num4 == -2)
                                                            {
                                                                if (num22 == 0)
                                                                {
                                                                    rectangle.X = 0;
                                                                    rectangle.Y = 252;
                                                                }
                                                                if (num22 == 1)
                                                                {
                                                                    rectangle.X = 18;
                                                                    rectangle.Y = 252;
                                                                }
                                                                if (num22 == 2)
                                                                {
                                                                    rectangle.X = 36;
                                                                    rectangle.Y = 252;
                                                                }
                                                                WorldGen.mergeLeft = true;
                                                            }
                                                            else
                                                            {
                                                                if (num5 == -2)
                                                                {
                                                                    if (num22 == 0)
                                                                    {
                                                                        rectangle.X = 54;
                                                                        rectangle.Y = 252;
                                                                    }
                                                                    if (num22 == 1)
                                                                    {
                                                                        rectangle.X = 72;
                                                                        rectangle.Y = 252;
                                                                    }
                                                                    if (num22 == 2)
                                                                    {
                                                                        rectangle.X = 90;
                                                                        rectangle.Y = 252;
                                                                    }
                                                                    WorldGen.mergeRight = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num2 == -2 && num7 == -1 && num4 == -1 && num5 == -1)
                                                        {
                                                            if (num22 == 0)
                                                            {
                                                                rectangle.X = 108;
                                                                rectangle.Y = 144;
                                                            }
                                                            if (num22 == 1)
                                                            {
                                                                rectangle.X = 108;
                                                                rectangle.Y = 162;
                                                            }
                                                            if (num22 == 2)
                                                            {
                                                                rectangle.X = 108;
                                                                rectangle.Y = 180;
                                                            }
                                                            WorldGen.mergeUp = true;
                                                        }
                                                        else
                                                        {
                                                            if (num2 == -1 && num7 == -2 && num4 == -1 && num5 == -1)
                                                            {
                                                                if (num22 == 0)
                                                                {
                                                                    rectangle.X = 108;
                                                                    rectangle.Y = 90;
                                                                }
                                                                if (num22 == 1)
                                                                {
                                                                    rectangle.X = 108;
                                                                    rectangle.Y = 108;
                                                                }
                                                                if (num22 == 2)
                                                                {
                                                                    rectangle.X = 108;
                                                                    rectangle.Y = 126;
                                                                }
                                                                WorldGen.mergeDown = true;
                                                            }
                                                            else
                                                            {
                                                                if (num2 == -1 && num7 == -1 && num4 == -2 && num5 == -1)
                                                                {
                                                                    if (num22 == 0)
                                                                    {
                                                                        rectangle.X = 0;
                                                                        rectangle.Y = 234;
                                                                    }
                                                                    if (num22 == 1)
                                                                    {
                                                                        rectangle.X = 18;
                                                                        rectangle.Y = 234;
                                                                    }
                                                                    if (num22 == 2)
                                                                    {
                                                                        rectangle.X = 36;
                                                                        rectangle.Y = 234;
                                                                    }
                                                                    WorldGen.mergeLeft = true;
                                                                }
                                                                else
                                                                {
                                                                    if (num2 == -1 && num7 == -1 && num4 == -1 && num5 == -2)
                                                                    {
                                                                        if (num22 == 0)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 234;
                                                                        }
                                                                        if (num22 == 1)
                                                                        {
                                                                            rectangle.X = 72;
                                                                            rectangle.Y = 234;
                                                                        }
                                                                        if (num22 == 2)
                                                                        {
                                                                            rectangle.X = 90;
                                                                            rectangle.Y = 234;
                                                                        }
                                                                        WorldGen.mergeRight = true;
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
                        if (rectangle.X < 0 || rectangle.Y < 0)
                        {
                            if (num9 == 2 || num9 == 23 || num9 == 60 || num9 == 70)
                            {
                                if (num2 == -2)
                                {
                                    num2 = num9;
                                }
                                if (num7 == -2)
                                {
                                    num7 = num9;
                                }
                                if (num4 == -2)
                                {
                                    num4 = num9;
                                }
                                if (num5 == -2)
                                {
                                    num5 = num9;
                                }
                                if (num == -2)
                                {
                                    num = num9;
                                }
                                if (num3 == -2)
                                {
                                    num3 = num9;
                                }
                                if (num6 == -2)
                                {
                                    num6 = num9;
                                }
                                if (num8 == -2)
                                {
                                    num8 = num9;
                                }
                            }
                            if (num2 == num9 && num7 == num9 && (num4 == num9 & num5 == num9))
                            {
                                if (num != num9 && num3 != num9)
                                {
                                    if (num22 == 0)
                                    {
                                        rectangle.X = 108;
                                        rectangle.Y = 18;
                                    }
                                    if (num22 == 1)
                                    {
                                        rectangle.X = 126;
                                        rectangle.Y = 18;
                                    }
                                    if (num22 == 2)
                                    {
                                        rectangle.X = 144;
                                        rectangle.Y = 18;
                                    }
                                }
                                else
                                {
                                    if (num6 != num9 && num8 != num9)
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 108;
                                            rectangle.Y = 36;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 126;
                                            rectangle.Y = 36;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 144;
                                            rectangle.Y = 36;
                                        }
                                    }
                                    else
                                    {
                                        if (num != num9 && num6 != num9)
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 180;
                                                rectangle.Y = 0;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 180;
                                                rectangle.Y = 18;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 180;
                                                rectangle.Y = 36;
                                            }
                                        }
                                        else
                                        {
                                            if (num3 != num9 && num8 != num9)
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 198;
                                                    rectangle.Y = 0;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 198;
                                                    rectangle.Y = 18;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 198;
                                                    rectangle.Y = 36;
                                                }
                                            }
                                            else
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 18;
                                                    rectangle.Y = 18;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 36;
                                                    rectangle.Y = 18;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 54;
                                                    rectangle.Y = 18;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (num2 != num9 && num7 == num9 && (num4 == num9 & num5 == num9))
                                {
                                    if (num22 == 0)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 0;
                                    }
                                    if (num22 == 1)
                                    {
                                        rectangle.X = 36;
                                        rectangle.Y = 0;
                                    }
                                    if (num22 == 2)
                                    {
                                        rectangle.X = 54;
                                        rectangle.Y = 0;
                                    }
                                }
                                else
                                {
                                    if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 == num9))
                                    {
                                        if (num22 == 0)
                                        {
                                            rectangle.X = 18;
                                            rectangle.Y = 36;
                                        }
                                        if (num22 == 1)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 36;
                                        }
                                        if (num22 == 2)
                                        {
                                            rectangle.X = 54;
                                            rectangle.Y = 36;
                                        }
                                    }
                                    else
                                    {
                                        if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 == num9))
                                        {
                                            if (num22 == 0)
                                            {
                                                rectangle.X = 0;
                                                rectangle.Y = 0;
                                            }
                                            if (num22 == 1)
                                            {
                                                rectangle.X = 0;
                                                rectangle.Y = 18;
                                            }
                                            if (num22 == 2)
                                            {
                                                rectangle.X = 0;
                                                rectangle.Y = 36;
                                            }
                                        }
                                        else
                                        {
                                            if (num2 == num9 && num7 == num9 && (num4 == num9 & num5 != num9))
                                            {
                                                if (num22 == 0)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 0;
                                                }
                                                if (num22 == 1)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 18;
                                                }
                                                if (num22 == 2)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 36;
                                                }
                                            }
                                            else
                                            {
                                                if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 == num9))
                                                {
                                                    if (num22 == 0)
                                                    {
                                                        rectangle.X = 0;
                                                        rectangle.Y = 54;
                                                    }
                                                    if (num22 == 1)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 54;
                                                    }
                                                    if (num22 == 2)
                                                    {
                                                        rectangle.X = 72;
                                                        rectangle.Y = 54;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num2 != num9 && num7 == num9 && (num4 == num9 & num5 != num9))
                                                    {
                                                        if (num22 == 0)
                                                        {
                                                            rectangle.X = 18;
                                                            rectangle.Y = 54;
                                                        }
                                                        if (num22 == 1)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 54;
                                                        }
                                                        if (num22 == 2)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 54;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 == num9))
                                                        {
                                                            if (num22 == 0)
                                                            {
                                                                rectangle.X = 0;
                                                                rectangle.Y = 72;
                                                            }
                                                            if (num22 == 1)
                                                            {
                                                                rectangle.X = 36;
                                                                rectangle.Y = 72;
                                                            }
                                                            if (num22 == 2)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 72;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 != num9))
                                                            {
                                                                if (num22 == 0)
                                                                {
                                                                    rectangle.X = 18;
                                                                    rectangle.Y = 72;
                                                                }
                                                                if (num22 == 1)
                                                                {
                                                                    rectangle.X = 54;
                                                                    rectangle.Y = 72;
                                                                }
                                                                if (num22 == 2)
                                                                {
                                                                    rectangle.X = 90;
                                                                    rectangle.Y = 72;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 != num9))
                                                                {
                                                                    if (num22 == 0)
                                                                    {
                                                                        rectangle.X = 90;
                                                                        rectangle.Y = 0;
                                                                    }
                                                                    if (num22 == 1)
                                                                    {
                                                                        rectangle.X = 90;
                                                                        rectangle.Y = 18;
                                                                    }
                                                                    if (num22 == 2)
                                                                    {
                                                                        rectangle.X = 90;
                                                                        rectangle.Y = 36;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 == num9))
                                                                    {
                                                                        if (num22 == 0)
                                                                        {
                                                                            rectangle.X = 108;
                                                                            rectangle.Y = 72;
                                                                        }
                                                                        if (num22 == 1)
                                                                        {
                                                                            rectangle.X = 126;
                                                                            rectangle.Y = 72;
                                                                        }
                                                                        if (num22 == 2)
                                                                        {
                                                                            rectangle.X = 144;
                                                                            rectangle.Y = 72;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 != num9))
                                                                        {
                                                                            if (num22 == 0)
                                                                            {
                                                                                rectangle.X = 108;
                                                                                rectangle.Y = 0;
                                                                            }
                                                                            if (num22 == 1)
                                                                            {
                                                                                rectangle.X = 126;
                                                                                rectangle.Y = 0;
                                                                            }
                                                                            if (num22 == 2)
                                                                            {
                                                                                rectangle.X = 144;
                                                                                rectangle.Y = 0;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 != num9))
                                                                            {
                                                                                if (num22 == 0)
                                                                                {
                                                                                    rectangle.X = 108;
                                                                                    rectangle.Y = 54;
                                                                                }
                                                                                if (num22 == 1)
                                                                                {
                                                                                    rectangle.X = 126;
                                                                                    rectangle.Y = 54;
                                                                                }
                                                                                if (num22 == 2)
                                                                                {
                                                                                    rectangle.X = 144;
                                                                                    rectangle.Y = 54;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 == num9))
                                                                                {
                                                                                    if (num22 == 0)
                                                                                    {
                                                                                        rectangle.X = 162;
                                                                                        rectangle.Y = 0;
                                                                                    }
                                                                                    if (num22 == 1)
                                                                                    {
                                                                                        rectangle.X = 162;
                                                                                        rectangle.Y = 18;
                                                                                    }
                                                                                    if (num22 == 2)
                                                                                    {
                                                                                        rectangle.X = 162;
                                                                                        rectangle.Y = 36;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 != num9))
                                                                                    {
                                                                                        if (num22 == 0)
                                                                                        {
                                                                                            rectangle.X = 216;
                                                                                            rectangle.Y = 0;
                                                                                        }
                                                                                        if (num22 == 1)
                                                                                        {
                                                                                            rectangle.X = 216;
                                                                                            rectangle.Y = 18;
                                                                                        }
                                                                                        if (num22 == 2)
                                                                                        {
                                                                                            rectangle.X = 216;
                                                                                            rectangle.Y = 36;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 != num9))
                                                                                        {
                                                                                            if (num22 == 0)
                                                                                            {
                                                                                                rectangle.X = 162;
                                                                                                rectangle.Y = 54;
                                                                                            }
                                                                                            if (num22 == 1)
                                                                                            {
                                                                                                rectangle.X = 180;
                                                                                                rectangle.Y = 54;
                                                                                            }
                                                                                            if (num22 == 2)
                                                                                            {
                                                                                                rectangle.X = 198;
                                                                                                rectangle.Y = 54;
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
                        if (rectangle.X <= -1 || rectangle.Y <= -1)
                        {
                            if (num22 <= 0)
                            {
                                rectangle.X = 18;
                                rectangle.Y = 18;
                            }
                            if (num22 == 1)
                            {
                                rectangle.X = 36;
                                rectangle.Y = 18;
                            }
                            if (num22 >= 2)
                            {
                                rectangle.X = 54;
                                rectangle.Y = 18;
                            }
                        }
                        world.getTile()[i, j].frameX = (short)rectangle.X;
                        world.getTile()[i, j].frameY = (short)rectangle.Y;
                        if (num9 == 52 || num9 == 62)
                        {
                            if (world.getTile()[i, j - 1] != null)
                            {
                                if (!world.getTile()[i, j - 1].active)
                                {
                                    num2 = -1;
                                }
                                else
                                {
                                    num2 = (int)world.getTile()[i, j - 1].type;
                                }
                            }
                            else
                            {
                                num2 = num9;
                            }
                            if (num2 != num9 && num2 != 2 && num2 != 60)
                            {
                                WorldGen.KillTile(i, j, world, false, false, false);
                            }
                        }
                        if (num9 == 53)
                        {
                            if (Statics.netMode == 0)
                            {
                                if (world.getTile()[i, j + 1] != null && !world.getTile()[i, j + 1].active)
                                {
                                    bool flag3 = true;
                                    if (world.getTile()[i, j - 1].active && world.getTile()[i, j - 1].type == 21)
                                    {
                                        flag3 = false;
                                    }
                                    if (flag3)
                                    {
                                        world.getTile()[i, j].active = false;
                                        Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), world, 0f, 0.41f, 31, 10, 0f, Statics.myPlayer);
                                        WorldGen.SquareTileFrame(i, j, world, true);
                                    }
                                }
                            }
                            else
                            {
                                if (Statics.netMode == 2 && world.getTile()[i, j + 1] != null && !world.getTile()[i, j + 1].active)
                                {
                                    bool flag4 = true;
                                    if (world.getTile()[i, j - 1].active && world.getTile()[i, j - 1].type == 21)
                                    {
                                        flag4 = false;
                                    }
                                    if (flag4)
                                    {
                                        world.getTile()[i, j].active = false;
                                        int num25 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), world, 0f, 0.41f, 31, 10, 0f, Statics.myPlayer);
                                        world.getProjectile()[num25].velocity.Y = 0.5f;
                                        world.getProjectile()[num25].position.Y = world.getProjectile()[num25].position.Y + 2f;
                                        NetMessage.SendTileSquare(-1, i, j, 1, world);
                                        WorldGen.SquareTileFrame(i, j, world, true);
                                    }
                                }
                            }
                        }
                        if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
                        {
                            bool flag5 = WorldGen.mergeUp;
                            bool flag6 = WorldGen.mergeDown;
                            bool flag7 = WorldGen.mergeLeft;
                            bool flag8 = WorldGen.mergeRight;
                            WorldGen.TileFrame(i - 1, j, world, false, false);
                            WorldGen.TileFrame(i + 1, j, world, false, false);
                            WorldGen.TileFrame(i, j - 1, world, false, false);
                            WorldGen.TileFrame(i, j + 1, world, false, false);
                            WorldGen.mergeUp = flag5;
                            WorldGen.mergeDown = flag6;
                            WorldGen.mergeLeft = flag7;
                            WorldGen.mergeRight = flag8;
                        }
                    }
                }
            }
        }

        public static void SectionTileFrame(int startX, int startY, World world, int endX, int endY)
        {
            int num = startX * 200;
            int num2 = (endX + 1) * 200;
            int num3 = startY * 150;
            int num4 = (endY + 1) * 150;
            if (num < 1)
            {
                num = 1;
            }
            if (num3 < 1)
            {
                num3 = 1;
            }
            if (num > (world.getMaxTilesX() - 2))
            {
                num = world.getMaxTilesX() - 2;
            }
            if (num3 > (world.getMaxTilesY() - 2))
            {
                num3 = world.getMaxTilesY() - 2;
            }
            for (int i = num - 1; i < (num2 + 1); i++)
            {
                for (int j = num3 - 1; j < (num4 + 1); j++)
                {
                    if (world.getTile()[i, j] == null)
                    {
                        world.getTile()[i, j] = new Tile();
                    }
                    TileFrame(i, j, world, true, true);
                    WallFrame(i, j, world, true);
                }
            }
        }

        public static World loadWorld(String WorldPath, Server server)
        {
            World world = server.getWorld();
            world.setServer(server);
            world.setSavePath(WorldPath);
            /*string name = new FileInfo(WorldPath).Name;
            if (name.Contains("."))
            {
                name = name.Split('.')[0];
            }
            world.setName(name);*/

            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
            }
            using (FileStream fileStream = new FileStream(WorldPath, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    //try
                    //{
                    int num = binaryReader.ReadInt32();
                    if (num > Statics.currentRelease)
                    {
                        //Main.menuMode = 15;
                        ////Console.WriteLine("Incompatible world file!";
                        //WorldGen.loadFailed = true;
                        Console.WriteLine("Incompatible world file!");
                        binaryReader.Close();
                        return null;
                    }


                    Console.WriteLine("Compatible world file!");
                    //Main.worldName = reader.ReadString();
                    world.setName(binaryReader.ReadString());
                    //Main.worldID = reader.ReadInt32();
                    world.setId(binaryReader.ReadInt32());
                    //Main.leftWorld = reader.ReadInt32();
                    world.setLeftWorld(binaryReader.ReadInt32());
                    //Main.rightWorld = reader.ReadInt32();
                    world.setRightWorld(binaryReader.ReadInt32());
                    //Main.topWorld = reader.ReadInt32();
                    world.setTopWorld(binaryReader.ReadInt32());
                    //Main.bottomWorld = reader.ReadInt32();
                    world.setBottomWorld(binaryReader.ReadInt32());
                    //maxTilesY = reader.ReadInt32();
                    world.setMaxTilesY(binaryReader.ReadInt32());
                    //maxTilesX = reader.ReadInt32();
                    world.setMaxTilesX(binaryReader.ReadInt32());
                    clearWorld(world);
                    //Main.spawnTileX = reader.ReadInt32();
                    Statics.spawnTileX = binaryReader.ReadInt32();
                    //Main.spawnTileY = reader.ReadInt32();
                    Statics.spawnTileY = binaryReader.ReadInt32();
                    //world.getWorldSurface() = reader.ReadDouble();
                    world.setWorldSurface(binaryReader.ReadDouble());
                    //world.getRockLayer() = reader.ReadDouble();
                    world.setRockLayer(binaryReader.ReadDouble());
                    //tempTime = reader.ReadDouble();
                    world.setTime(binaryReader.ReadDouble());
                    //tempDayTime = reader.ReadBoolean();
                    world.setDayTime(binaryReader.ReadBoolean());
                    //tempMoonPhase = reader.ReadInt32();
                    world.setMoonPhase(binaryReader.ReadInt32());
                    //tempBloodMoon = reader.(ReadBoolean);
                    world.setBloodMoon(binaryReader.ReadBoolean());
                    //world.getDungeonX() = reader.ReadInt32();
                    world.setDungeonX(binaryReader.ReadInt32());
                    //world.getDungeonY() = reader.ReadInt32();
                    world.setDungeonY(binaryReader.ReadInt32());
                    NPC.downedBoss1 = binaryReader.ReadBoolean();
                    NPC.downedBoss2 = binaryReader.ReadBoolean();
                    NPC.downedBoss3 = binaryReader.ReadBoolean();

                    world.setShadowOrbSmashed(binaryReader.ReadBoolean());
                    world.setSpawnMeteor(binaryReader.ReadBoolean());
                    world.setShadowOrbCount(binaryReader.ReadByte());

                    world.setInvasionDelay(binaryReader.ReadInt32());
                    world.setInvasionSize(binaryReader.ReadInt32());
                    world.setInvasionType(binaryReader.ReadInt32());
                    world.setInvasionX(binaryReader.ReadDouble());

                    for (int i = 0; i < world.getMaxTilesX(); i++)
                    {
                        float num2 = (float)i / (float)world.getMaxTilesX();
                        Program.printData("Loading world data: " + ((int)((num2 * 100f) + 1f)) + "%");

                        for (int j = 0; j < world.getMaxTilesY(); j++)
                        {
                            world.getTile()[i, j].active = binaryReader.ReadBoolean();
                            if (world.getTile()[i, j].active)
                            {
                                world.getTile()[i, j].type = binaryReader.ReadByte();
                                if (Statics.tileFrameImportant[(int)world.getTile()[i, j].type])
                                {
                                    world.getTile()[i, j].frameX = binaryReader.ReadInt16();
                                    world.getTile()[i, j].frameY = binaryReader.ReadInt16();
                                }
                                else
                                {
                                    world.getTile()[i, j].frameX = -1;
                                    world.getTile()[i, j].frameY = -1;
                                }
                            }
                            world.getTile()[i, j].lighted = binaryReader.ReadBoolean();
                            if (binaryReader.ReadBoolean())
                            {
                                world.getTile()[i, j].wall = binaryReader.ReadByte();
                            }
                            if (binaryReader.ReadBoolean())
                            {
                                world.getTile()[i, j].liquid = binaryReader.ReadByte();
                                world.getTile()[i, j].lava = binaryReader.ReadBoolean();
                            }
                        }

                    }
                    for (int k = 0; k < 1000; k++)
                    {
                        if (binaryReader.ReadBoolean())
                        {
                            world.getChests()[k] = new Chest();
                            world.getChests()[k].x = binaryReader.ReadInt32();
                            world.getChests()[k].y = binaryReader.ReadInt32();
                            for (int l = 0; l < Chest.maxItems; l++)
                            {
                                world.getChests()[k].item[l] = new Item();
                                byte b = binaryReader.ReadByte();
                                if (b > 0)
                                {
                                    string defaults = binaryReader.ReadString();
                                    world.getChests()[k].item[l].SetDefaults(defaults);
                                    world.getChests()[k].item[l].stack = (int)b;
                                }
                            }
                        }
                    }
                    for (int m = 0; m < 1000; m++)
                    {
                        if (binaryReader.ReadBoolean())
                        {
                            string text = binaryReader.ReadString();
                            int num3 = binaryReader.ReadInt32();
                            int num4 = binaryReader.ReadInt32();
                            try
                            {
                                if (world.getTile()[num3, num4].active && world.getTile()[num3, num4].type == 55)
                                {
                                    world.getSigns()[m] = new Sign();
                                    world.getSigns()[m].x = num3;
                                    world.getSigns()[m].y = num4;
                                    world.getSigns()[m].text = text;
                                }
                            }
                            catch { }
                        }
                    }
                    bool flag = binaryReader.ReadBoolean();
                    int num5 = 0;
                    while (flag)
                    {
                        //i//f (world.getNPCs()[num5] == null)
                        //{
                        /*binaryReader.ReadString();
                        binaryReader.ReadSingle();
                        binaryReader.ReadSingle();
                        binaryReader.ReadBoolean();
                        binaryReader.ReadInt32();
                        binaryReader.ReadInt32();*/
                        world.getNPCs()[num5] = new NPC();
                        // }
                        //else
                        //{
                        world.getNPCs()[num5].SetDefaults(binaryReader.ReadString());
                        world.getNPCs()[num5].position.X = binaryReader.ReadSingle();
                        world.getNPCs()[num5].position.Y = binaryReader.ReadSingle();
                        world.getNPCs()[num5].homeless = binaryReader.ReadBoolean();
                        world.getNPCs()[num5].homeTileX = binaryReader.ReadInt32();
                        world.getNPCs()[num5].homeTileY = binaryReader.ReadInt32();
                        //}

                        flag = binaryReader.ReadBoolean();
                        num5++;
                    }
                    binaryReader.Close();
                    Console.WriteLine();
                    WorldGen.gen = true;
                    WorldGen.waterLine = world.getMaxTilesY();
                    Liquid.QuickWater(world, 2, -1, -1);
                    WorldGen.WaterCheck(world);
                    int num6 = 0;
                    Liquid.quickSettle = true;
                    int num7 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                    float num8 = 0f;
                    while (Liquid.numLiquid > 0 && num6 < 100000)
                    {
                        num6++;
                        float num9 = (float)(num7 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num7;
                        if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num7)
                        {
                            num7 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                        }
                        if (num9 > num8)
                        {
                            num8 = num9;
                        }
                        else
                        {
                            num9 = num8;
                        }
                        ////Console.WriteLine("Settling liquids: " + (int)(num9 * 100f / 2f + 50f) + "%";
                        Program.printData("Settling liquids: " + (int)(num9 * 100f / 2f + 50f) + "%");
                        Liquid.UpdateLiquid(world);
                    }
                    Liquid.quickSettle = false;
                    WorldGen.WaterCheck(world);
                    WorldGen.gen = false;
                    Console.WriteLine();
                    // }
                    //catch (Exception arg_617_0)
                    //{
                    //   Exception exception = arg_617_0;
                    ///Main.menuMode = 15;
                    ////Console.WriteLine(exception.ToString();
                    //    Console.WriteLine(exception.ToString());
                    //WorldGen.loadFailed = true;
                    //try
                    // {
                    //    binaryReader.Close();
                    //}
                    //catch
                    // {
                    // }
                    //   return null;
                    //}
                    //WorldGen.loadFailed = false;
                    try
                    {
                        binaryReader.Close();
                    }
                    catch
                    {
                    }
                    return world;
                }
            }
        }

        public static void clearWorld(World world)
        {
            //spawnEye = false;
            world.setSpawnEye(false);
            //spawnNPC = 0;
            world.setSpawnNPC(0);
            //shadowOrbCount = 0;
            world.setShadowOrbCount(0);
            //Main.helpText = 0;
            world.setSpawnEye(false);
            //world.getDungeonX() = 0;
            world.setDungeonX(0);
            //world.getDungeonY() = 0;
            world.setDungeonY(0);
            NPC.downedBoss1 = false;
            NPC.downedBoss2 = false;
            NPC.downedBoss3 = false;
            //shadowOrbSmashed = false;
            world.setShadowOrbSmashed(false);
            //spawnMeteor = false;
            world.setSpawnMeteor(false);
            //stopDrops = false;
            world.setStopDrops(false);
            //Main.invasionDelay = 0;
            world.setInvasionDelay(0);
            //Main.invasionType = 0;
            world.setInvasionType(0);
            //Main.invasionSize = 0;
            world.setInvasionSize(0);
            //Main.invasionWarn = 0;
            world.setInvasionWarn(0);
            //Main.invasionX = 0.0;
            world.setInvasionX(0);
            noLiquidCheck = false;
            
            Liquid.numLiquid = 0;
            LiquidBuffer.numLiquidBuffer = 0;
            string text = "";
            if (((Statics.netMode == 1) || (lastMaxTilesX > world.getMaxTilesX())) || (lastMaxTilesY > world.getMaxTilesY()))
            {
                for (int num = 0; num < lastMaxTilesX; num++)
                {
                    float num2 = ((float)num) / ((float)lastMaxTilesX);
                    ////Console.WriteLine("Freeing unused resources: " + ((int)((num2 * 100f) + 1f)) + "%";
                    //Console.WriteLine("Freeing unused resources: " + ((int)((num2 * 100f) + 1f)) + "%");
                    Program.printData("Freeing unused resources: " + ((int)((num2 * 100f) + 1f)) + "%");
                    for (int num3 = 0; num3 < lastMaxTilesY; num3++)
                    {
                        world.getTile()[num, num3] = null;
                    }
                }
                Console.WriteLine();
            }
            lastMaxTilesX = world.getMaxTilesX();
            lastMaxTilesY = world.getMaxTilesY();
            if (Statics.netMode != 1)
            {
                for (int num4 = 0; num4 < world.getMaxTilesX(); num4++)
                {
                    float num5 = ((float)num4) / ((float)world.getMaxTilesX());
                    ////Console.WriteLine("Resetting game objects: " + ((int)((num5 * 100f) + 1f)) + "%";
                    //Console.WriteLine("Resetting game objects: " + ((int)((num5 * 100f) + 1f)) + "%");
                    Program.printData("Resetting game objects: " + ((int)((num5 * 100f) + 1f)) + "%");
                    for (int num6 = 0; num6 < world.getMaxTilesY(); num6++)
                    {
                        world.getTile()[num4, num6] = new Tile();

                    }
                }
                Console.WriteLine();
            }
            for (int i = 0; i < 0x7d0; i++)
            {
                world.getDust()[i] = new Dust();
            }
            for (int j = 0; j < 200; j++)
            {
                world.getGore()[j] = new Gore();
            }
            for (int k = 0; k < 200; k++)
            {
                world.getItemList()[k] = new Item();
            }
            for (int m = 0; m < 0x3e8; m++)
            {
                world.getNPCs()[m] = new NPC();
            }
            for (int n = 0; n < 0x3e8; n++)
            {
                world.getProjectile()[n] = new Projectile();
            }
            for (int num12 = 0; num12 < 0x3e8; num12++)
            {
                world.getChests()[num12] = null;
            }
            for (int num13 = 0; num13 < 0x3e8; num13++)
            {
                world.getSigns()[num13] = null;
            }
            for (int num14 = 0; num14 < Liquid.resLiquid; num14++)
            {
                world.getLiquid()[num14] = new Liquid();
            }
            for (int num15 = 0; num15 < 0x2710; num15++)
            {
                world.getLiquidBuffer()[num15] = new LiquidBuffer();
            }
            setWorldSize(world);
            worldCleared = true;
        }

        public static void setWorldSize(World world)
        {
            world.setBottomWorld(world.getMaxTilesY() * 0x10);
            world.setRightWorld(world.getMaxTilesX() * 0x10);
            world.setMaxSectionsX(world.getMaxTilesX() / 200);
            world.setMaxSectionsY(world.getMaxTilesY() / 150);
        }

        public static void EveryTileFrame(World world)
        {
            noLiquidCheck = true;
            for (int i = 0; i < world.getMaxTilesX(); i++)
            {
                float num2 = ((float)i) / ((float)world.getMaxTilesX());
                ////Console.WriteLine
                Program.printData("Finding tile frames: " + ((int)((num2 * 100f) + 1f)) + "%");
                for (int j = 0; j < world.getMaxTilesY(); j++)
                {
                    TileFrame(i, j, world, true, false);
                    WallFrame(i, j, world, true);
                }
            }
            Console.WriteLine();
            noLiquidCheck = false;
        }

        public static bool meteor(int i, int j, World world)
        {
            if ((i < 50) || (i > (world.getMaxTilesX() - 50)))
            {
                return false;
            }
            if ((j < 50) || (j > (world.getMaxTilesY() - 50)))
            {
                return false;
            }
            int num = 0x19;
            Rectangle rectangle = new Rectangle((i - num) * 0x10, (j - num) * 0x10, (num * 2) * 0x10, (num * 2) * 0x10);
            for (int k = 0; k < 8; k++)
            {
                if (world.getPlayerList()[k].active)
                {
                    Rectangle rectangle2 = new Rectangle(((((int)world.getPlayerList()[k].position.X) + (world.getPlayerList()[k].width / 2)) -
                        (Statics.screenWidth / 2)) - NPC.safeRangeX, ((((int)world.getPlayerList()[k].position.Y) +
                        (world.getPlayerList()[k].height / 2)) - (Statics.screenHeight / 2)) - NPC.safeRangeY,
                        Statics.screenWidth + (NPC.safeRangeX * 2), Statics.screenHeight + (NPC.safeRangeY * 2));
                    if (rectangle.Intersects(rectangle2))
                    {
                        return false;
                    }
                }
            }
            for (int m = 0; m < 0x3e8; m++)
            {
                if (world.getNPCs()[m].active)
                {
                    Rectangle rectangle3 = new Rectangle((int)world.getNPCs()[m].position.X, (int)world.getNPCs()[m].position.Y, world.getNPCs()[m].width, world.getNPCs()[m].height);
                    if (rectangle.Intersects(rectangle3))
                    {
                        return false;
                    }
                }
            }
            for (int n = i - num; n < (i + num); n++)
            {
                for (int num5 = j - num; num5 < (j + num); num5++)
                {
                    if (world.getTile()[n, num5].active && (world.getTile()[n, num5].type == 0x15))
                    {
                        return false;
                    }
                }
            }
            stopDrops = true;
            num = 15;
            for (int num6 = i - num; num6 < (i + num); num6++)
            {
                for (int num7 = j - num; num7 < (j + num); num7++)
                {
                    if ((num7 > ((j + Statics.rand.Next(-2, 3)) - 5)) && ((Math.Abs((int)(i - num6)) + Math.Abs((int)(j - num7))) < ((num * 1.5) + Statics.rand.Next(-5, 5))))
                    {
                        if (!Statics.tileSolid[world.getTile()[num6, num7].type])
                        {
                            world.getTile()[num6, num7].active = false;
                        }
                        world.getTile()[num6, num7].type = 0x25;
                    }
                }
            }
            num = 10;
            for (int num8 = i - num; num8 < (i + num); num8++)
            {
                for (int num9 = j - num; num9 < (j + num); num9++)
                {
                    if ((num9 > ((j + Statics.rand.Next(-2, 3)) - 5)) && ((Math.Abs((int)(i - num8)) + Math.Abs((int)(j - num9))) < (num + Statics.rand.Next(-3, 4))))
                    {
                        world.getTile()[num8, num9].active = false;
                    }
                }
            }
            num = 0x10;
            for (int num10 = i - num; num10 < (i + num); num10++)
            {
                for (int num11 = j - num; num11 < (j + num); num11++)
                {
                    if ((world.getTile()[num10, num11].type == 5) || (world.getTile()[num10, num11].type == 0x20))
                    {
                        KillTile(num10, num11, world, false, false, false);
                    }
                    SquareTileFrame(num10, num11, world, true);
                    SquareWallFrame(num10, num11, world, true);
                }
            }
            num = 0x17;
            for (int num12 = i - num; num12 < (i + num); num12++)
            {
                for (int num13 = j - num; num13 < (j + num); num13++)
                {
                    if ((world.getTile()[num12, num13].active && (Statics.rand.Next(10) == 0)) && ((Math.Abs((int)(i - num12)) + Math.Abs((int)(j - num13))) < (num * 1.3)))
                    {
                        if ((world.getTile()[num12, num13].type == 5) || (world.getTile()[num12, num13].type == 0x20))
                        {
                            KillTile(num12, num13, world, false, false, false);
                        }
                        world.getTile()[num12, num13].type = 0x25;
                        SquareTileFrame(num12, num13, world, true);
                    }
                }
            }
            stopDrops = false;
            if (Statics.netMode == 2)
            {
                NetMessage.SendData(0x19, world, -1, -1, "A meteorite has landed!", 8, 50f, 255f, 130f);
            }
            if (Statics.netMode != 1)
            {
                NetMessage.SendTileSquare(-1, i, j, 30, world);
            }
            return true;
        }

        public static void dropMeteor(World world)
        {
            bool flag = true;
            int num = 0;
            if (Statics.netMode != 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (world.getPlayerList()[i].active)
                    {
                        flag = false;
                        break;
                    }
                }
                int num3 = 0;
                float num4 = world.getMaxTilesX() / 0x1068;
                int num5 = (int)(400f * num4);
                for (int j = 5; j < (world.getMaxTilesX() - 5); j++)
                {
                    for (int k = 5; k < world.getWorldSurface(); k++)
                    {
                        if (world.getTile()[j, k].active && (world.getTile()[j, k].type == 0x25))
                        {
                            num3++;
                            if (num3 > num5)
                            {
                                return;
                            }
                        }
                    }
                }
                while (!flag)
                {
                    float num8 = world.getMaxTilesX() * 0.08f;
                    int num9 = Statics.rand.Next(50, world.getMaxTilesX() - 50);
                    while ((num9 > (Statics.spawnTileX - num8)) && (num9 < (Statics.spawnTileX + num8)))
                    {
                        num9 = Statics.rand.Next(50, world.getMaxTilesX() - 50);
                    }
                    for (int m = Statics.rand.Next(100); m < world.getMaxTilesY(); m++)
                    {
                        if (world.getTile()[num9, m].active && Statics.tileSolid[world.getTile()[num9, m].type])
                        {
                            flag = meteor(num9, m, world);
                            break;
                        }
                    }
                    num++;
                    if (num >= 100)
                    {
                        return;
                    }
                }
            }
        }

        public static void saveWorld(World world, bool resetTime = false)
        {
            if (!Statics.saveLock)
            {
                Statics.saveLock = true;
                //if (!Main.skipMenu)
               // {
                    bool dayTime = world.isDayTime();
                    double tempTime = world.getTime();
                    int tempMoonPhase = world.getMoonPhase();
                    bool tempBloodMoon = world.isBloodMoon();
                    if (resetTime)
                    {
                        dayTime = true;
                        tempTime = 13500.0;
                        tempMoonPhase = 0;
                        tempBloodMoon = false;
                    }
                    if (world.getSavePath() != null)
                    {
                        string path = world.getSavePath() + ".sav";
                        string name = new FileInfo(world.getSavePath()).Name;
                        if (name.Contains("."))
                        {
                            name = name.Split('.')[0];
                        }
                        world.setName(name);
                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            using (BinaryWriter writer = new BinaryWriter(stream))
                            {
                                writer.Write(Statics.currentRelease);
                                writer.Write(world.getName());
                                writer.Write(world.getId());
                                writer.Write((int)world.getLeftWorld());
                                writer.Write((int)world.getRightWorld());
                                writer.Write((int)world.getTopWorld());
                                writer.Write((int)world.getBottomWorld());
                                writer.Write(world.getMaxTilesY());
                                writer.Write(world.getMaxTilesX());
                                writer.Write(Statics.spawnTileX);
                                writer.Write(Statics.spawnTileY);
                                writer.Write(world.getWorldSurface());
                                writer.Write(world.getRockLayer());
                                writer.Write(tempTime);
                                writer.Write(dayTime);
                                writer.Write(tempMoonPhase);
                                writer.Write(tempBloodMoon);
                                writer.Write(world.getDungeonX());
                                writer.Write(world.getDungeonY());
                                writer.Write(NPC.downedBoss1);
                                writer.Write(NPC.downedBoss2);
                                writer.Write(NPC.downedBoss3);
                                writer.Write(shadowOrbSmashed);
                                writer.Write(spawnMeteor);
                                writer.Write((byte)shadowOrbCount);
                                writer.Write(world.getInvasionDelay());
                                writer.Write(world.getInvasionSize());
                                writer.Write(world.getInvasionType());
                                writer.Write(world.getInvasionX());
                                for (int i = 0; i < world.getMaxTilesX(); i++)
                                {
                                    float num2 = ((float)i) / ((float)world.getMaxTilesX());
                                    ////Console.WriteLine;
                                    Program.printData("Saving world data: " + ((int)((num2 * 100f) + 1f)) + "%");

                                    for (int n = 0; n < world.getMaxTilesY(); n++)
                                    {
                                        writer.Write(world.getTile()[i, n].active);
                                        if (world.getTile()[i, n].active)
                                        {
                                            writer.Write(world.getTile()[i, n].type);
                                            if (Statics.tileFrameImportant[world.getTile()[i, n].type])
                                            {
                                                writer.Write(world.getTile()[i, n].frameX);
                                                writer.Write(world.getTile()[i, n].frameY);
                                            }
                                        }
                                        writer.Write(world.getTile()[i, n].lighted);
                                        if (world.getTile()[i, n].wall > 0)
                                        {
                                            writer.Write(true);
                                            writer.Write(world.getTile()[i, n].wall);
                                        }
                                        else
                                        {
                                            writer.Write(false);
                                        }
                                        if (world.getTile()[i, n].liquid > 0)
                                        {
                                            writer.Write(true);
                                            writer.Write(world.getTile()[i, n].liquid);
                                            writer.Write(world.getTile()[i, n].lava);
                                        }
                                        else
                                        {
                                            writer.Write(false);
                                        }
                                    }
                                }
                                for (int j = 0; j < 0x3e8; j++)
                                {
                                    if (world.getChests()[j] == null)
                                    {
                                        writer.Write(false);
                                    }
                                    else
                                    {
                                        writer.Write(true);
                                        writer.Write(world.getChests()[j].x);
                                        writer.Write(world.getChests()[j].y);
                                        for (int num5 = 0; num5 < Chest.maxItems; num5++)
                                        {
                                            writer.Write((byte)world.getChests()[j].item[num5].stack);
                                            if (world.getChests()[j].item[num5].stack > 0)
                                            {
                                                writer.Write(world.getChests()[j].item[num5].name);
                                            }
                                        }
                                    }
                                }
                                for (int k = 0; k < 0x3e8; k++)
                                {
                                    if ((world.getSigns()[k] == null) || (world.getSigns()[k].text == null))
                                    {
                                        writer.Write(false);
                                    }
                                    else
                                    {
                                        writer.Write(true);
                                        writer.Write(world.getSigns()[k].text);
                                        writer.Write(world.getSigns()[k].x);
                                        writer.Write(world.getSigns()[k].y);
                                    }
                                }
                                for (int m = 0; m < 0x3e8; m++)
                                {
                                    lock (world.getNPCs()[m])
                                    {
                                        if (world.getNPCs()[m].active && world.getNPCs()[m].townNPC)
                                        {
                                            writer.Write(true);
                                            writer.Write(world.getNPCs()[m].name);
                                            writer.Write(world.getNPCs()[m].position.X);
                                            writer.Write(world.getNPCs()[m].position.Y);
                                            writer.Write(world.getNPCs()[m].homeless);
                                            writer.Write(world.getNPCs()[m].homeTileX);
                                            writer.Write(world.getNPCs()[m].homeTileY);
                                        }
                                    }
                                }
                                writer.Write(false);
                                writer.Close();
                                ////Console.WriteLine("Backing up world file...";
                                Console.WriteLine();
                                Console.WriteLine("Backing up world file...");
                                string destFileName = world.getSavePath() + ".bak";
                                if (File.Exists(world.getSavePath()))
                                {
                                    File.Copy(world.getSavePath(), destFileName, true);
                                }
                                File.Copy(path, world.getSavePath(), true);
                                File.Delete(path);
                            }
                        }
                        Statics.saveLock = false;
                    }
                //}
            }
        }

        public static void saveAndPlay(World world)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveAndPlayCallBack), world);
        }

        public static void saveAndPlayCallBack(object threadContext)
        {
            if (threadContext is World)
            {
                saveWorld((World)threadContext, false);
            }
        }

        public static void TileRunner(int i, int j, World world, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool noYChange = false, bool overRide = true)
        {
            double num = strength;
            float num2 = (float)steps;
            Vector2 value = new Vector2();
            value.X = (float)i;
            value.Y = (float)j;
            Vector2 value2 = new Vector2();
            value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            value2.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            if (speedX != 0f || speedY != 0f)
            {
                value2.X = speedX;
                value2.Y = speedY;
            }
            while (num > 0.0 && num2 > 0f)
            {
                num = strength * (double)(num2 / (float)steps);
                num2 -= 1f;
                int num3 = (int)((double)value.X - num * 0.5);
                int num4 = (int)((double)value.X + num * 0.5);
                int num5 = (int)((double)value.Y - num * 0.5);
                int num6 = (int)((double)value.Y + num * 0.5);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > world.getMaxTilesX())
                {
                    num4 = world.getMaxTilesX();
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > world.getMaxTilesY())
                {
                    num6 = world.getMaxTilesY();
                }
                for (int k = num3; k < num4; k++)
                {
                    for (int l = num5; l < num6; l++)
                    {
                        if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < strength * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            if (type < 0)
                            {
                                if (type == -2 && world.getTile()[k, l].active && (l < WorldGen.waterLine || l > WorldGen.lavaLine))
                                {
                                    world.getTile()[k, l].liquid = 255;
                                    if (l > WorldGen.lavaLine)
                                    {
                                        world.getTile()[k, l].lava = true;
                                    }
                                }
                                world.getTile()[k, l].active = false;
                            }
                            else
                            {
                                if ((overRide || !world.getTile()[k, l].active) && (type != 40 || world.getTile()[k, l].type != 53) && (!Statics.tileStone[type] || world.getTile()[k, l].type == 1) && world.getTile()[k, l].type != 45)
                                {
                                    world.getTile()[k, l].type = (byte)type;
                                }
                                if (addTile)
                                {
                                    world.getTile()[k, l].active = true;
                                    world.getTile()[k, l].liquid = 0;
                                    world.getTile()[k, l].lava = false;
                                }
                                if (noYChange && (double)l < world.getWorldSurface())
                                {
                                    world.getTile()[k, l].wall = 2;
                                }
                                if (type == 59 && l > WorldGen.waterLine && world.getTile()[k, l].liquid > 0)
                                {
                                    world.getTile()[k, l].lava = false;
                                    world.getTile()[k, l].liquid = 0;
                                }
                            }
                        }
                    }
                }
                value += value2;
                if (num > 50.0)
                {
                    value += value2;
                    num2 -= 1f;
                    value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                    value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                    if (num > 100.0)
                    {
                        value += value2;
                        num2 -= 1f;
                        value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                        value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                        if (num > 150.0)
                        {
                            value += value2;
                            num2 -= 1f;
                            value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                            value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                            if (num > 200.0)
                            {
                                value += value2;
                                num2 -= 1f;
                                value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                if (num > 250.0)
                                {
                                    value += value2;
                                    num2 -= 1f;
                                    value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                    value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                    if (num > 300.0)
                                    {
                                        value += value2;
                                        num2 -= 1f;
                                        value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                        value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                        if (num > 400.0)
                                        {
                                            value += value2;
                                            num2 -= 1f;
                                            value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                            value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                            if (num > 500.0)
                                            {
                                                value += value2;
                                                num2 -= 1f;
                                                value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                if (num > 600.0)
                                                {
                                                    value += value2;
                                                    num2 -= 1f;
                                                    value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                    value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                    if (num > 700.0)
                                                    {
                                                        value += value2;
                                                        num2 -= 1f;
                                                        value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                        value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                        if (num > 800.0)
                                                        {
                                                            value += value2;
                                                            num2 -= 1f;
                                                            value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            if (num > 900.0)
                                                            {
                                                                value += value2;
                                                                num2 -= 1f;
                                                                value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
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
                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                if (value2.X > 1f)
                {
                    value2.X = 1f;
                }
                if (value2.X < -1f)
                {
                    value2.X = -1f;
                }
                if (!noYChange)
                {
                    value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                    if (value2.Y > 1f)
                    {
                        value2.Y = 1f;
                    }
                    if (value2.Y < -1f)
                    {
                        value2.Y = -1f;
                    }
                }
                else
                {
                    if (num < 3.0)
                    {
                        if (value2.Y > 1f)
                        {
                            value2.Y = 1f;
                        }
                        if (value2.Y < -1f)
                        {
                            value2.Y = -1f;
                        }
                    }
                }
                if (type == 59)
                {
                    if ((double)value2.Y > 0.5)
                    {
                        value2.Y = 0.5f;
                    }
                    if ((double)value2.Y < -0.5)
                    {
                        value2.Y = -0.5f;
                    }
                    if ((double)value.Y < world.getRockLayer() + 100.0)
                    {
                        value2.Y = 1f;
                    }
                    if (value.Y > (float)(world.getMaxTilesX() - 300))
                    {
                        value2.Y = -1f;
                    }
                }
            }
        }

        private static void resetGen()
        {
            WorldGen.numMCaves = 0;
            WorldGen.numIslandHouses = 0;
            WorldGen.houseCount = 0;
            WorldGen.dEnteranceX = 0;
            WorldGen.numDRooms = 0;
            WorldGen.numDDoors = 0;
            WorldGen.numDPlats = 0;
            WorldGen.numJChests = 0;
        }
        
        public static World generateWorld(int MaxTilesX, int MaxTilesY, int seed = -1, World world = null)
        {
            if (world == null)
            {
                world = new World(MaxTilesX, MaxTilesY);
            }
            gen = true;
            if (seed > 0)
            {
                genRand = new Random(seed);
            }
            else
            {
                genRand = new Random((int)DateTime.Now.Ticks);
            }
            world.setId(genRand.Next(0x7fffffff));
            //Main.worldID = genRand.Next(0x7fffffff);
            int num7 = 0;
            int num8 = 0;
            double num = world.getMaxTilesY() * 0.3;
            num *= genRand.Next(90, 110) * 0.005;
            double num4 = num + (world.getMaxTilesY() * 0.2);
            num4 *= genRand.Next(90, 110) * 0.01;
            double num2 = num;
            double num3 = num;
            double num5 = num4;
            double num6 = num4;
            int num9 = 0;
            if (genRand.Next(2) == 0)
            {
                num9 = -1;
            }
            else
            {
                num9 = 1;
            }
            for (int i = 0; i < world.getMaxTilesX(); i++)
            {
                float num11 = ((float)i) / ((float)world.getMaxTilesX());
                // Console.WriteLine("Generating world terrain: " + ((int)((num11 * 100f) + 1f)) + "%";
                Program.printData("Generating world terrain: " + ((int)((num11 * 100f) + 1f)) + "%");
                if (num < num2)
                {
                    num2 = num;
                }
                if (num > num3)
                {
                    num3 = num;
                }
                if (num4 < num5)
                {
                    num5 = num4;
                }
                if (num4 > num6)
                {
                    num6 = num4;
                }
                if (num8 <= 0)
                {
                    num7 = genRand.Next(0, 5);
                    num8 = genRand.Next(5, 40);
                    if (num7 == 0)
                    {
                        num8 *= (int)(genRand.Next(5, 30) * 0.2);
                    }
                }
                num8--;
                switch (num7)
                {
                    case 0:
                        while (genRand.Next(0, 7) == 0)
                        {
                            num += genRand.Next(-1, 2);
                        }
                        break;

                    case 1:
                        while (genRand.Next(0, 4) == 0)
                        {
                            num--;
                        }
                        while (genRand.Next(0, 10) == 0)
                        {
                            num++;
                        }
                        break;

                    case 2:
                        while (genRand.Next(0, 4) == 0)
                        {
                            num++;
                        }
                        while (genRand.Next(0, 10) == 0)
                        {
                            num--;
                        }
                        break;

                    case 3:
                        while (genRand.Next(0, 2) == 0)
                        {
                            num--;
                        }
                        while (genRand.Next(0, 6) == 0)
                        {
                            num++;
                        }
                        break;

                    case 4:
                        while (genRand.Next(0, 2) == 0)
                        {
                            num++;
                        }
                        while (genRand.Next(0, 5) == 0)
                        {
                            num--;
                        }
                        break;
                }
                if (num < (world.getMaxTilesY() * 0.15))
                {
                    num = world.getMaxTilesY() * 0.15;
                    num8 = 0;
                }
                else if (num > (world.getMaxTilesY() * 0.3))
                {
                    num = world.getMaxTilesY() * 0.3;
                    num8 = 0;
                }
                while (genRand.Next(0, 3) == 0)
                {
                    num4 += genRand.Next(-2, 3);
                }
                if (num4 < (num + (world.getMaxTilesY() * 0.05)))
                {
                    num4++;
                }
                if (num4 > (num + (world.getMaxTilesY() * 0.35)))
                {
                    num4--;
                }
                for (int num12 = 0; num12 < num; num12++)
                {
                    world.getTile()[i, num12].active = false;
                    world.getTile()[i, num12].lighted = true;
                    world.getTile()[i, num12].frameX = -1;
                    world.getTile()[i, num12].frameY = -1;
                }
                for (int num13 = (int)num; num13 < world.getMaxTilesY(); num13++)
                {
                    if (num13 < num4)
                    {
                        world.getTile()[i, num13].active = true;
                        world.getTile()[i, num13].type = 0;
                        world.getTile()[i, num13].frameX = -1;
                        world.getTile()[i, num13].frameY = -1;
                    }
                    else
                    {
                        world.getTile()[i, num13].active = true;
                        world.getTile()[i, num13].type = 1;
                        world.getTile()[i, num13].frameX = -1;
                        world.getTile()[i, num13].frameY = -1;
                    }
                }
            }
            world.setWorldSurface(num3 + 5.0);
            //world.getWorldSurface() = num3 + 5.0;
            world.setRockLayer(num6);
            //world.getRockLayer() = num6;
            double num14 = ((int)((world.getRockLayer() - world.getWorldSurface()) / 6.0)) * 6;
            world.setRockLayer(world.getWorldSurface() + num14);
            waterLine = (((int)world.getRockLayer()) + world.getMaxTilesY()) / 2;
            waterLine += genRand.Next(-100, 20);
            lavaLine = waterLine + genRand.Next(50, 80);
            int num15 = 0;
            Console.WriteLine();
            Program.printData("Adding sand...");
            int num16 = genRand.Next((int)(world.getMaxTilesX() * 0.0007), (int)(world.getMaxTilesX() * 0.002)) + 2;
            for (int j = 0; j < num16; j++)
            {
                int num18 = genRand.Next(world.getMaxTilesX());
                while ((num18 > (world.getMaxTilesX() * 0.45f)) && (num18 < (world.getMaxTilesX() * 0.55f)))
                {
                    num18 = genRand.Next(world.getMaxTilesX());
                }
                int num19 = genRand.Next(15, 90);
                if (genRand.Next(3) == 0)
                {
                    num19 *= 2;
                }
                int num20 = num18 - num19;
                num19 = genRand.Next(15, 90);
                if (genRand.Next(3) == 0)
                {
                    num19 *= 2;
                }
                int maxTilesX = num18 + num19;
                if (num20 < 0)
                {
                    num20 = 0;
                }
                if (maxTilesX > world.getMaxTilesX())
                {
                    maxTilesX = world.getMaxTilesX();
                }
                switch (j)
                {
                    case 0:
                        num20 = 0;
                        maxTilesX = genRand.Next(250, 300);
                        break;

                    case 2:
                        num20 = world.getMaxTilesX() - genRand.Next(250, 300);
                        maxTilesX = world.getMaxTilesX();
                        break;
                }
                int num22 = genRand.Next(50, 100);
                for (int num23 = num20; num23 < maxTilesX; num23++)
                {
                    if (genRand.Next(2) == 0)
                    {
                        num22 += genRand.Next(-1, 2);
                        if (num22 < 50)
                        {
                            num22 = 50;
                        }
                        if (num22 > 100)
                        {
                            num22 = 100;
                        }
                    }
                    for (int num24 = 0; num24 < world.getWorldSurface(); num24++)
                    {
                        if (world.getTile()[num23, num24].active)
                        {
                            int num25 = num22;
                            if ((num23 - num20) < num25)
                            {
                                num25 = num23 - num20;
                            }
                            if ((maxTilesX - num23) < num25)
                            {
                                num25 = maxTilesX - num23;
                            }
                            num25 += genRand.Next(5);
                            for (int num26 = num24; num26 < (num24 + num25); num26++)
                            {
                                if ((num23 > (num20 + genRand.Next(5))) && (num23 < (maxTilesX - genRand.Next(5))))
                                {
                                    world.getTile()[num23, num26].type = 0x35;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            for (int k = 0; k < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 8E-06)); k++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)world.getWorldSurface(), (int)world.getRockLayer()), world, (double)genRand.Next(15, 70), genRand.Next(20, 130), 0x35, false, 0f, 0f, false, true);
            }
            numMCaves = 0;
            Console.WriteLine(); 
            Program.printData("Generating hills...");
            for (int m = 0; m < ((int)(world.getMaxTilesX() * 0.0008)); m++)
            {
                int num29 = 0;
                bool flag = false;
                bool flag2 = false;
                int num30 = genRand.Next((int)(world.getMaxTilesX() * 0.25), (int)(world.getMaxTilesX() * 0.75));
                while (!flag2)
                {
                    flag2 = true;
                    while ((num30 > ((world.getMaxTilesX() / 2) - 100)) && (num30 < ((world.getMaxTilesX() / 2) + 100)))
                    {
                        num30 = genRand.Next((int)(world.getMaxTilesX() * 0.25), (int)(world.getMaxTilesX() * 0.75));
                    }
                    for (int num31 = 0; num31 < numMCaves; num31++)
                    {
                        if ((num30 > (mCaveX[num31] - 50)) && (num30 < (mCaveX[num31] + 50)))
                        {
                            num29++;
                            flag2 = false;
                            break;
                        }
                    }
                    if (num29 >= 200)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    for (int num32 = 0; num32 < world.getWorldSurface(); num32++)
                    {
                        if (world.getTile()[num30, num32].active)
                        {
                            Mountinater(num30, num32, world);
                            mCaveX[numMCaves] = num30;
                            mCaveY[numMCaves] = num32;
                            numMCaves++;
                            break;
                        }
                    }
                }
            }
            Console.WriteLine();
            for (int n = 1; n < (world.getMaxTilesX() - 1); n++)
            {
                float num34 = ((float)n) / ((float)world.getMaxTilesX());
                Program.printData("Putting dirt behind dirt: " + ((int)((num34 * 100f) + 1f)) + "%");
                bool flag3 = false;
                num15 += genRand.Next(-1, 2);
                if (num15 < 0)
                {
                    num15 = 0;
                }
                if (num15 > 10)
                {
                    num15 = 10;
                }
                for (int num35 = 0; num35 < (world.getWorldSurface() + 10.0); num35++)
                {
                    if (num35 > (world.getWorldSurface() + num15))
                    {
                        break;
                    }
                    if (flag3)
                    {
                        world.getTile()[n, num35].wall = 2;
                    }
                    if (((world.getTile()[n, num35].active && world.getTile()[n - 1, num35].active) && (world.getTile()[n + 1, num35].active && world.getTile()[n, num35 + 1].active)) && (world.getTile()[n - 1, num35 + 1].active && world.getTile()[n + 1, num35 + 1].active))
                    {
                        flag3 = true;
                    }
                }
            }
            numIslandHouses = 0;
            houseCount = 0;
            Console.WriteLine();
            Program.printData("Generating floating islands...");
            for (int num36 = 0; num36 < ((int)(world.getMaxTilesX() * 0.0008)); num36++)
            {
                int num37 = 0;
                bool flag4 = false;
                int num38 = genRand.Next((int)(world.getMaxTilesX() * 0.1), (int)(world.getMaxTilesX() * 0.9));
                bool flag5 = false;
                while (!flag5)
                {
                    flag5 = true;
                    while ((num38 > ((world.getMaxTilesX() / 2) - 80)) && (num38 < ((world.getMaxTilesX() / 2) + 80)))
                    {
                        num38 = genRand.Next((int)(world.getMaxTilesX() * 0.1), (int)(world.getMaxTilesX() * 0.9));
                    }
                    for (int num39 = 0; num39 < numIslandHouses; num39++)
                    {
                        if ((num38 > (fihX[num39] - 80)) && (num38 < (fihX[num39] + 80)))
                        {
                            num37++;
                            flag5 = false;
                            break;
                        }
                    }
                    if (num37 >= 200)
                    {
                        flag4 = true;
                        break;
                    }
                }
                if (!flag4)
                {
                    for (int num40 = 200; num40 < world.getWorldSurface(); num40++)
                    {
                        if (world.getTile()[num38, num40].active)
                        {
                            int num41 = num38;
                            int num42 = genRand.Next(100, num40 - 100);
                            while (num42 > (num2 - 50.0))
                            {
                                num42--;
                            }
                            FloatingIsland(num41, num42, world);
                            fihX[numIslandHouses] = num41;
                            fihY[numIslandHouses] = num42;
                            numIslandHouses++;
                            break;
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Placing rocks in the dirt...");
            for (int num43 = 0; num43 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.0002)); num43++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next(0, ((int)num2) + 1), world, (double)genRand.Next(4, 15), genRand.Next(5, 40), 1, false, 0f, 0f, false, true);
            }
            for (int num44 = 0; num44 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.0002)); num44++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num2, ((int)num3) + 1), world, (double)genRand.Next(4, 10), genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
            }
            for (int num45 = 0; num45 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.0045)); num45++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num3, ((int)num6) + 1), world, (double)genRand.Next(2, 7), genRand.Next(2, 0x17), 1, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            Program.printData("Placing dirt in the rocks...");
            for (int num46 = 0; num46 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.005)); num46++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num5, world.getMaxTilesY()), world, (double)genRand.Next(2, 6), genRand.Next(2, 40), 0, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            Program.printData("Adding clay...");
            for (int num47 = 0; num47 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 2E-05)); num47++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next(0, (int)num2), world, (double)genRand.Next(4, 14), genRand.Next(10, 50), 40, false, 0f, 0f, false, true);
            }
            for (int num48 = 0; num48 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 5E-05)); num48++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num2, ((int)num3) + 1), world, (double)genRand.Next(8, 14), genRand.Next(15, 0x2d), 40, false, 0f, 0f, false, true);
            }
            for (int num49 = 0; num49 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 2E-05)); num49++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num3, ((int)num6) + 1), world, (double)genRand.Next(8, 15), genRand.Next(5, 50), 40, false, 0f, 0f, false, true);
            }
            for (int num50 = 5; num50 < (world.getMaxTilesX() - 5); num50++)
            {
                for (int num51 = 1; num51 < (world.getWorldSurface() - 1.0); num51++)
                {
                    if (world.getTile()[num50, num51].active)
                    {
                        for (int num52 = num51; num52 < (num51 + 5); num52++)
                        {
                            if (world.getTile()[num50, num52].type == 40)
                            {
                                world.getTile()[num50, num52].type = 0;
                            }
                        }
                        break;
                    }
                }
            }
            Console.WriteLine();
            for (int num53 = 0; num53 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.0015)); num53++)
            {
                float num54 = (float)(((double)num53) / ((world.getMaxTilesX() * world.getMaxTilesY()) * 0.0015));
                Program.printData("Making random holes: " + ((int)((num54 * 100f) + 1f)) + "%");
                int type = -1;
                if (genRand.Next(5) == 0)
                {
                    type = -2;
                }
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num3, world.getMaxTilesY()), world, (double)genRand.Next(2, 5), genRand.Next(2, 20), type, false, 0f, 0f, false, true);
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num3, world.getMaxTilesY()), world, (double)genRand.Next(8, 15), genRand.Next(7, 30), type, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            for (int num56 = 0; num56 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 3E-05)); num56++)
            {
                float num57 = (float)(((double)num56) / ((world.getMaxTilesX() * world.getMaxTilesY()) * 3E-05));
                Program.printData("Generating small caves: " + ((int)((num57 * 100f) + 1f)) + "%");
                if (num6 <= world.getMaxTilesY())
                {
                    int num58 = -1;
                    if (genRand.Next(6) == 0)
                    {
                        num58 = -2;
                    }
                    TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num2, ((int)num6) + 1), world, (double)genRand.Next(5, 15), genRand.Next(30, 200), num58, false, 0f, 0f, false, true);
                }
            }
            Console.WriteLine();
            for (int num59 = 0; num59 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.00015)); num59++)
            {
                float num60 = (float)(((double)num59) / ((world.getMaxTilesX() * world.getMaxTilesY()) * 0.00015));
                Program.printData("Generating large caves: " + ((int)((num60 * 100f) + 1f)) + "%");
                if (num6 <= world.getMaxTilesY())
                {
                    int num61 = -1;
                    if (genRand.Next(10) == 0)
                    {
                        num61 = -2;
                    }
                    TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num6, world.getMaxTilesY()), world, (double)genRand.Next(6, 20), genRand.Next(50, 300), num61, false, 0f, 0f, false, true);
                }
            }
            int num62 = 0;
            Console.WriteLine();
            Program.printData("Generating surface caves...");
            for (int num63 = 0; num63 < ((int)(world.getMaxTilesX() * 0.0025)); num63++)
            {
                num62 = genRand.Next(0, world.getMaxTilesX());
                for (int num64 = 0; num64 < num3; num64++)
                {
                    if (world.getTile()[num62, num64].active)
                    {
                        TileRunner(num62, num64, world, (double)genRand.Next(3, 6), genRand.Next(5, 50), -1, false, genRand.Next(-10, 11) * 0.1f, 1f, false, true);
                        break;
                    }
                }
            }
            for (int num65 = 0; num65 < ((int)(world.getMaxTilesX() * 0.0007)); num65++)
            {
                num62 = genRand.Next(0, world.getMaxTilesX());
                for (int num66 = 0; num66 < num3; num66++)
                {
                    if (world.getTile()[num62, num66].active)
                    {
                        TileRunner(num62, num66, world, (double)genRand.Next(10, 15), genRand.Next(50, 130), -1, false, genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        break;
                    }
                }
            }
            for (int num67 = 0; num67 < ((int)(world.getMaxTilesX() * 0.0003)); num67++)
            {
                num62 = genRand.Next(0, world.getMaxTilesX());
                for (int num68 = 0; num68 < num3; num68++)
                {
                    if (world.getTile()[num62, num68].active)
                    {
                        TileRunner(num62, num68, world, (double)genRand.Next(12, 0x19), genRand.Next(150, 500), -1, false, genRand.Next(-10, 11) * 0.1f, 4f, false, true);
                        TileRunner(num62, num68, world, (double)genRand.Next(8, 0x11), genRand.Next(60, 200), -1, false, genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        TileRunner(num62, num68, world, (double)genRand.Next(5, 13), genRand.Next(40, 170), -1, false, genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        break;
                    }
                }
            }
            for (int num69 = 0; num69 < ((int)(world.getMaxTilesX() * 0.0004)); num69++)
            {
                num62 = genRand.Next(0, world.getMaxTilesX());
                for (int num70 = 0; num70 < num3; num70++)
                {
                    if (world.getTile()[num62, num70].active)
                    {
                        TileRunner(num62, num70, world, (double)genRand.Next(7, 12), genRand.Next(150, 250), -1, false, 0f, 1f, true, true);
                        break;
                    }
                }
            }
            for (int num73 = 0; num73 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.002)); num73++)
            {
                int num71 = genRand.Next(1, world.getMaxTilesX() - 1);
                int num72 = genRand.Next((int)num2, (int)num3);
                if (num72 >= world.getMaxTilesY())
                {
                    num72 = world.getMaxTilesY() - 2;
                }
                if (((world.getTile()[num71 - 1, num72].active && (world.getTile()[num71 - 1, num72].type == 0)) && (world.getTile()[num71 + 1, num72].active && (world.getTile()[num71 + 1, num72].type == 0))) && ((world.getTile()[num71, num72 - 1].active && (world.getTile()[num71, num72 - 1].type == 0)) && (world.getTile()[num71, num72 + 1].active && (world.getTile()[num71, num72 + 1].type == 0))))
                {
                    world.getTile()[num71, num72].active = true;
                    world.getTile()[num71, num72].type = 2;
                }
                num71 = genRand.Next(1, world.getMaxTilesX() - 1);
                num72 = genRand.Next(0, (int)num2);
                if (num72 >= world.getMaxTilesY())
                {
                    num72 = world.getMaxTilesY() - 2;
                }
                if (((world.getTile()[num71 - 1, num72].active && (world.getTile()[num71 - 1, num72].type == 0)) && (world.getTile()[num71 + 1, num72].active && (world.getTile()[num71 + 1, num72].type == 0))) && ((world.getTile()[num71, num72 - 1].active && (world.getTile()[num71, num72 - 1].type == 0)) && (world.getTile()[num71, num72 + 1].active && (world.getTile()[num71, num72 + 1].type == 0))))
                {
                    world.getTile()[num71, num72].active = true;
                    world.getTile()[num71, num72].type = 2;
                }
            }
            Console.WriteLine();
            Program.printData("Generating underground jungle: 0%");
            float num74 = world.getMaxTilesX() / 0x1068;
            num74 *= 1.5f;
            int num75 = 0;
            if (num9 == -1)
            {
                num75 = (int)(world.getMaxTilesX() * 0.8f);
            }
            else
            {
                num75 = (int)(world.getMaxTilesX() * 0.2f);
            }
            int num76 = (world.getMaxTilesY() + ((int)world.getRockLayer())) / 2;
            num75 += genRand.Next((int)(-100f * num74), (int)(101f * num74));
            num76 += genRand.Next((int)(-100f * num74), (int)(101f * num74));
            TileRunner(num75, num76, world, (double)genRand.Next((int)(250f * num74), (int)(500f * num74)), genRand.Next(50, 150), 0x3b, false, (float)(num9 * 3), 0f, false, true);
            Program.printData("Generating underground jungle: 20%");
            num75 += genRand.Next((int)(-250f * num74), (int)(251f * num74));
            num76 += genRand.Next((int)(-150f * num74), (int)(151f * num74));
            int num77 = num75;
            int num78 = num76;
            TileRunner(num75, num76, world, (double)genRand.Next((int)(250f * num74), (int)(500f * num74)), genRand.Next(50, 150), 0x3b, false, 0f, 0f, false, true);
            Program.printData("Generating underground jungle: 40%");
            num75 += genRand.Next((int)(-400f * num74), (int)(401f * num74));
            num76 += genRand.Next((int)(-150f * num74), (int)(151f * num74));
            TileRunner(num75, num76, world, (double)genRand.Next((int)(250f * num74), (int)(500f * num74)), genRand.Next(50, 150), 0x3b, false, (float)(num9 * -3), 0f, false, true);
            Program.printData("Generating underground jungle: 60%");
            num75 = num77;
            num76 = num78;
            for (int num79 = 0; num79 <= (20f * num74); num79++)
            {
                Program.printData("Generating underground jungle: " + ((int)(60f + (((float)num79) / num74))) + "%");
                num75 += genRand.Next((int)(-5f * num74), (int)(6f * num74));
                num76 += genRand.Next((int)(-5f * num74), (int)(6f * num74));
                TileRunner(num75, num76, world, (double)genRand.Next(40, 100), genRand.Next(300, 500), 0x3b, false, 0f, 0f, false, true);
            }
            for (int num80 = 0; num80 <= (10f * num74); num80++)
            {
                Program.printData("Generating underground jungle: " + ((int)(80f + ((((float)num80) / num74) * 2f))) + "%");
                num75 = num77 + genRand.Next((int)(-600f * num74), (int)(600f * num74));
                num76 = num78 + genRand.Next((int)(-200f * num74), (int)(200f * num74));
                while ((((num75 < 1) || (num75 >= (world.getMaxTilesX() - 1))) || ((num76 < 1) || (num76 >= (world.getMaxTilesY() - 1)))) || (world.getTile()[num75, num76].type != 0x3b))
                {
                    num75 = num77 + genRand.Next((int)(-600f * num74), (int)(600f * num74));
                    num76 = num78 + genRand.Next((int)(-200f * num74), (int)(200f * num74));
                }
                for (int num81 = 0; num81 < (8f * num74); num81++)
                {
                    num75 += genRand.Next(-30, 0x1f);
                    num76 += genRand.Next(-30, 0x1f);
                    int num82 = -1;
                    if (genRand.Next(7) == 0)
                    {
                        num82 = -2;
                    }
                    TileRunner(num75, num76, world, (double)genRand.Next(10, 20), genRand.Next(30, 70), num82, false, 0f, 0f, false, true);
                }
            }
            for (int num83 = 0; num83 <= (300f * num74); num83++)
            {
                num75 = num77 + genRand.Next((int)(-600f * num74), (int)(600f * num74));
                num76 = num78 + genRand.Next((int)(-200f * num74), (int)(200f * num74));
                while ((((num75 < 1) || (num75 >= (world.getMaxTilesX() - 1))) || ((num76 < 1) || (num76 >= (world.getMaxTilesY() - 1)))) || (world.getTile()[num75, num76].type != 0x3b))
                {
                    num75 = num77 + genRand.Next((int)(-600f * num74), (int)(600f * num74));
                    num76 = num78 + genRand.Next((int)(-200f * num74), (int)(200f * num74));
                }
                TileRunner(num75, num76, world, (double)genRand.Next(4, 10), genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
                if (genRand.Next(4) == 0)
                {
                    int num84 = genRand.Next(0x3f, 0x45);
                    TileRunner(num75 + genRand.Next(-1, 2), num76 + genRand.Next(-1, 2), world, (double)genRand.Next(3, 7), genRand.Next(4, 8), num84, false, 0f, 0f, false, true);
                }
            }
            num75 = num77;
            num76 = num78;
            float num85 = genRand.Next(6, 10);
            float num86 = world.getMaxTilesX() / 0x1068;
            num85 *= num86;
            for (int num87 = 0; num87 < num85; num87++)
            {
                bool flag6 = true;
                while (flag6)
                {
                    num75 = genRand.Next(20, world.getMaxTilesX() - 20);
                    num76 = genRand.Next(20, world.getMaxTilesY() - 300);
                    if (world.getTile()[num75, num76].type == 0x3b)
                    {
                        flag6 = false;
                        int num88 = genRand.Next(2, 4);
                        int num89 = genRand.Next(2, 4);
                        for (int num90 = (num75 - num88) - 1; num90 <= ((num75 + num88) + 1); num90++)
                        {
                            for (int num91 = (num76 - num89) - 1; num91 <= ((num76 + num89) + 1); num91++)
                            {
                                world.getTile()[num90, num91].active = true;
                                world.getTile()[num90, num91].type = 0x2d;
                                world.getTile()[num90, num91].liquid = 0;
                                world.getTile()[num90, num91].lava = false;
                            }
                        }
                        for (int num92 = num75 - num88; num92 <= (num75 + num88); num92++)
                        {
                            for (int num93 = num76 - num89; num93 <= (num76 + num89); num93++)
                            {
                                world.getTile()[num92, num93].active = false;
                                world.getTile()[num92, num93].wall = 10;
                            }
                        }
                        bool flag7 = false;
                        int num94 = 0;
                        while (!flag7 && (num94 < 100))
                        {
                            num94++;
                            int num95 = genRand.Next(num75 - num88, (num75 + num88) + 1);
                            int num96 = genRand.Next(num76 - num89, (num76 + num89) - 2);
                            PlaceTile(num95, num96, world, 4, true, false, -1);
                            if (world.getTile()[num95, num96].type == 4)
                            {
                                flag7 = true;
                            }
                        }
                        for (int num97 = (num75 - num88) - 1; num97 <= ((num75 + num88) + 1); num97++)
                        {
                            for (int num98 = (num76 + num89) - 2; num98 <= (num76 + num89); num98++)
                            {
                                world.getTile()[num97, num98].active = false;
                            }
                        }
                        for (int num99 = (num75 - num88) - 1; num99 <= ((num75 + num88) + 1); num99++)
                        {
                            for (int num100 = (num76 + num89) - 2; num100 <= ((num76 + num89) - 1); num100++)
                            {
                                world.getTile()[num99, num100].active = false;
                            }
                        }
                        for (int num101 = (num75 - num88) - 1; num101 <= ((num75 + num88) + 1); num101++)
                        {
                            int num102 = 4;
                            int num103 = (num76 + num89) + 2;
                            while ((!world.getTile()[num101, num103].active && (num103 < world.getMaxTilesY())) && (num102 > 0))
                            {
                                world.getTile()[num101, num103].active = true;
                                world.getTile()[num101, num103].type = 0x3b;
                                num103++;
                                num102--;
                            }
                        }
                        num88 -= genRand.Next(1, 3);
                        for (int num104 = (num76 - num89) - 2; num88 > -1; num104--)
                        {
                            for (int num105 = (num75 - num88) - 1; num105 <= ((num75 + num88) + 1); num105++)
                            {
                                world.getTile()[num105, num104].active = true;
                                world.getTile()[num105, num104].type = 0x2d;
                            }
                            num88 -= genRand.Next(1, 3);
                        }
                        JChestX[numJChests] = num75;
                        JChestY[numJChests] = num76;
                        numJChests++;
                    }
                }
            }
            for (int num106 = 0; num106 < world.getMaxTilesX(); num106++)
            {
                for (int num107 = (int)world.getWorldSurface(); num107 < world.getMaxTilesY(); num107++)
                {
                    if (world.getTile()[num106, num107].active)
                    {
                        SpreadGrass(num106, num107, world, 0x3b, 60, false);
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Adding mushroom patches...");
            for (int num108 = 0; num108 < (world.getMaxTilesX() / 300); num108++)
            {
                int num109 = genRand.Next((int)(world.getMaxTilesX() * 0.3), (int)(world.getMaxTilesX() * 0.7));
                int num110 = genRand.Next((int)world.getRockLayer(), world.getMaxTilesY() - 300);
                ShroomPatch(num109, num110, world);
            }
            for (int num111 = 0; num111 < world.getMaxTilesX(); num111++)
            {
                for (int num112 = (int)world.getWorldSurface(); num112 < world.getMaxTilesY(); num112++)
                {
                    if (world.getTile()[num111, num112].active)
                    {
                        SpreadGrass(num111, num112, world, 0x3b, 70, false);
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Placing mud in the dirt...");
            for (int num113 = 0; num113 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.001)); num113++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num5, world.getMaxTilesY()), world, (double)genRand.Next(2, 6), genRand.Next(2, 40), 0x3b, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            Program.printData("Adding shinies...");
            for (int num114 = 0; num114 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 6E-05)); num114++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num2, (int)num3), world, (double)genRand.Next(3, 6), genRand.Next(2, 6), 7, false, 0f, 0f, false, true);
            }
            for (int num115 = 0; num115 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 8E-05)); num115++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num3, (int)num6), world, (double)genRand.Next(3, 7), genRand.Next(3, 7), 7, false, 0f, 0f, false, true);
            }
            for (int num116 = 0; num116 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.0002)); num116++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num5, world.getMaxTilesY()), world, (double)genRand.Next(4, 9), genRand.Next(4, 8), 7, false, 0f, 0f, false, true);
            }
            for (int num117 = 0; num117 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 3E-05)); num117++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num2, (int)num3), world, (double)genRand.Next(3, 7), genRand.Next(2, 5), 6, false, 0f, 0f, false, true);
            }
            for (int num118 = 0; num118 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 8E-05)); num118++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num3, (int)num6), world, (double)genRand.Next(3, 6), genRand.Next(3, 6), 6, false, 0f, 0f, false, true);
            }
            for (int num119 = 0; num119 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.0002)); num119++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num5, world.getMaxTilesY()), world, (double)genRand.Next(4, 9), genRand.Next(4, 8), 6, false, 0f, 0f, false, true);
            }
            for (int num120 = 0; num120 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 3E-05)); num120++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num3, (int)num6), world, (double)genRand.Next(3, 6), genRand.Next(3, 6), 9, false, 0f, 0f, false, true);
            }
            for (int num121 = 0; num121 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.00017)); num121++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num5, world.getMaxTilesY()), world, (double)genRand.Next(4, 9), genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
            }
            for (int num122 = 0; num122 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.00017)); num122++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next(0, (int)num2), world, (double)genRand.Next(4, 9), genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
            }
            for (int num123 = 0; num123 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.00012)); num123++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next((int)num5, world.getMaxTilesY()), world, (double)genRand.Next(4, 8), genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
            }
            for (int num124 = 0; num124 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.00012)); num124++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next(0, ((int)num2) - 20), world, (double)genRand.Next(4, 8), genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            Program.printData("Adding webs...");
            for (int num125 = 0; num125 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.001)); num125++)
            {
                int num126 = genRand.Next(20, world.getMaxTilesX() - 20);
                int num127 = genRand.Next((int)num2, world.getMaxTilesY() - 20);
                if (num125 < numMCaves)
                {
                    num126 = mCaveX[num125];
                    num127 = mCaveY[num125];
                }
                if (!world.getTile()[num126, num127].active && ((num127 > world.getWorldSurface()) || (world.getTile()[num126, num127].wall > 0)))
                {
                    while (!world.getTile()[num126, num127].active && (num127 > ((int)num2)))
                    {
                        num127--;
                    }
                    num127++;
                    int num128 = 1;
                    if (genRand.Next(2) == 0)
                    {
                        num128 = -1;
                    }
                    while ((!world.getTile()[num126, num127].active && (num126 > 10)) && (num126 < (world.getMaxTilesX() - 10)))
                    {
                        num126 += num128;
                    }
                    num126 -= num128;
                    if ((num127 > world.getWorldSurface()) || (world.getTile()[num126, num127].wall > 0))
                    {
                        TileRunner(num126, num127, world, (double)genRand.Next(4, 13), genRand.Next(2, 5), 0x33, true, (float)num128, -1f, false, false);
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Creating underworld: 0%");
            int num129 = world.getMaxTilesY() - genRand.Next(150, 190);
            for (int num130 = 0; num130 < world.getMaxTilesX(); num130++)
            {
                num129 += genRand.Next(-3, 4);
                if (num129 < (world.getMaxTilesY() - 190))
                {
                    num129 = world.getMaxTilesY() - 190;
                }
                if (num129 > (world.getMaxTilesY() - 160))
                {
                    num129 = world.getMaxTilesY() - 160;
                }
                for (int num131 = (num129 - 20) - genRand.Next(3); num131 < world.getMaxTilesY(); num131++)
                {
                    if (num131 >= num129)
                    {
                        world.getTile()[num130, num131].active = false;
                        world.getTile()[num130, num131].lava = false;
                        world.getTile()[num130, num131].liquid = 0;
                    }
                    else
                    {
                        world.getTile()[num130, num131].type = 0x39;
                    }
                }
            }
            int num132 = world.getMaxTilesY() - genRand.Next(40, 70);
            for (int num133 = 10; num133 < (world.getMaxTilesX() - 10); num133++)
            {
                num132 += genRand.Next(-10, 11);
                if (num132 > (world.getMaxTilesY() - 60))
                {
                    num132 = world.getMaxTilesY() - 60;
                }
                if (num132 < (world.getMaxTilesY() - 100))
                {
                    num132 = world.getMaxTilesY() - 120;
                }
                for (int num134 = num132; num134 < (world.getMaxTilesY() - 10); num134++)
                {
                    if (!world.getTile()[num133, num134].active)
                    {
                        world.getTile()[num133, num134].lava = true;
                        world.getTile()[num133, num134].liquid = 0xff;
                    }
                }
            }
            for (int num135 = 0; num135 < world.getMaxTilesX(); num135++)
            {
                if (genRand.Next(50) == 0)
                {
                    int num136 = world.getMaxTilesY() - 0x41;
                    while (!world.getTile()[num135, num136].active && (num136 > (world.getMaxTilesY() - 0x87)))
                    {
                        num136--;
                    }
                    TileRunner(genRand.Next(0, world.getMaxTilesX()), num136 + genRand.Next(20, 50), world, (double)genRand.Next(15, 20), 0x3e8, 0x39, true, 0f, (float)genRand.Next(1, 3), true, true);
                }
            }
            Liquid.QuickWater(world, -2, -1, -1);
            for (int num137 = 0; num137 < world.getMaxTilesX(); num137++)
            {
                float num138 = ((float)num137) / ((float)(world.getMaxTilesX() - 1));
                Program.printData("Creating underworld: " + ((int)(((num138 * 100f) / 2f) + 50f)) + "%");
                if (genRand.Next(13) == 0)
                {
                    int num139 = world.getMaxTilesY() - 0x41;
                    while (((world.getTile()[num137, num139].liquid > 0) || world.getTile()[num137, num139].active) && (num139 > (world.getMaxTilesY() - 140)))
                    {
                        num139--;
                    }
                    TileRunner(num137, num139 - genRand.Next(2, 5), world, (double)genRand.Next(5, 30), 0x3e8, 0x39, true, 0f, (float)genRand.Next(1, 3), true, true);
                    float num140 = genRand.Next(1, 3);
                    if (genRand.Next(3) == 0)
                    {
                        num140 *= 0.5f;
                    }
                    if (genRand.Next(2) == 0)
                    {
                        TileRunner(num137, num139 - genRand.Next(2, 5), world, (double)((int)(genRand.Next(5, 15) * num140)), (int)(genRand.Next(10, 15) * num140), 0x39, true, 1f, 0.3f, false, true);
                    }
                    if (genRand.Next(2) == 0)
                    {
                        num140 = genRand.Next(1, 3);
                        TileRunner(num137, num139 - genRand.Next(2, 5), world, (double)((int)(genRand.Next(5, 15) * num140)), (int)(genRand.Next(10, 15) * num140), 0x39, true, -1f, 0.3f, false, true);
                    }
                    TileRunner(num137 + genRand.Next(-10, 10), num139 + genRand.Next(-10, 10), world, (double)genRand.Next(5, 15), genRand.Next(5, 10), -2, false, (float)genRand.Next(-1, 3), (float)genRand.Next(-1, 3), false, true);
                    if (genRand.Next(3) == 0)
                    {
                        TileRunner(num137 + genRand.Next(-10, 10), num139 + genRand.Next(-10, 10), world, (double)genRand.Next(10, 30), genRand.Next(10, 20), -2, false, (float)genRand.Next(-1, 3), (float)genRand.Next(-1, 3), false, true);
                    }
                    if (genRand.Next(5) == 0)
                    {
                        TileRunner(num137 + genRand.Next(-15, 15), num139 + genRand.Next(-15, 10), world, (double)genRand.Next(15, 30), genRand.Next(5, 20), -2, false, (float)genRand.Next(-1, 3), (float)genRand.Next(-1, 3), false, true);
                    }
                }
            }
            for (int num141 = 0; num141 < world.getMaxTilesX(); num141++)
            {
                if (!world.getTile()[num141, world.getMaxTilesY() - 0x91].active)
                {
                    world.getTile()[num141, world.getMaxTilesY() - 0x91].liquid = 0xff;
                    world.getTile()[num141, world.getMaxTilesY() - 0x91].lava = true;
                }
                if (!world.getTile()[num141, world.getMaxTilesY() - 0x90].active)
                {
                    world.getTile()[num141, world.getMaxTilesY() - 0x90].liquid = 0xff;
                    world.getTile()[num141, world.getMaxTilesY() - 0x90].lava = true;
                }
            }
            for (int num142 = 0; num142 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.002)); num142++)
            {
                TileRunner(genRand.Next(0, world.getMaxTilesX()), genRand.Next(world.getMaxTilesY() - 140, world.getMaxTilesY()), world, (double)genRand.Next(3, 8), genRand.Next(3, 8), 0x3a, false, 0f, 0f, false, true);
            }
            AddHellHouses(world);
            int num143 = genRand.Next(2, (int)(world.getMaxTilesX() * 0.005));
            Console.WriteLine();
            for (int num144 = 0; num144 < num143; num144++)
            {
                float num145 = ((float)num144) / ((float)num143);
                Program.printData("Adding water bodies: " + ((int)(num145 * 100f)) + "%");
                int num146 = genRand.Next(300, world.getMaxTilesX() - 300);
                while ((num146 > ((world.getMaxTilesX() / 2) - 50)) && (num146 < ((world.getMaxTilesX() / 2) + 50)))
                {
                    num146 = genRand.Next(300, world.getMaxTilesX() - 300);
                }
                int num147 = ((int)num2) - 20;
                while (!world.getTile()[num146, num147].active)
                {
                    num147++;
                }
                Lakinater(num146, num147, world);
            }
            int x = 0;
            if (num9 == -1)
            {
                x = genRand.Next((int)(world.getMaxTilesX() * 0.05), (int)(world.getMaxTilesX() * 0.2));
                num9 = -1;
            }
            else
            {
                x = genRand.Next((int)(world.getMaxTilesX() * 0.8), (int)(world.getMaxTilesX() * 0.95));
                num9 = 1;
            }
            int y = ((int)((world.getRockLayer() + world.getMaxTilesY()) / 2.0)) + genRand.Next(-200, 200);
            Console.WriteLine();
            MakeDungeon(x, y, world, 0x29, 7);

            Console.WriteLine();
            for (int num150 = 0; num150 < (world.getMaxTilesX() * 0.0004); num150++)
            {
                float num151 = (float)(((double)num150) / (world.getMaxTilesX() * 0.0004));
                Program.printData("Making the world evil: " + ((int)(num151 * 100f)) + "%");
                bool flag8 = false;
                int num152 = 0;
                int num153 = 0;
                int num154 = 0;
                while (!flag8)
                {
                    flag8 = true;
                    int num155 = world.getMaxTilesX() / 2;
                    int num156 = 200;
                    num152 = genRand.Next(world.getMaxTilesX());
                    num153 = (num152 - genRand.Next(150)) - 0xaf;
                    num154 = (num152 + genRand.Next(150)) + 0xaf;
                    if (num153 < 0)
                    {
                        num153 = 0;
                    }
                    if (num154 > world.getMaxTilesX())
                    {
                        num154 = world.getMaxTilesX();
                    }
                    if ((num152 > (num155 - num156)) && (num152 < (num155 + num156)))
                    {
                        flag8 = false;
                    }
                    if ((num153 > (num155 - num156)) && (num153 < (num155 + num156)))
                    {
                        flag8 = false;
                    }
                    if ((num154 > (num155 - num156)) && (num154 < (num155 + num156)))
                    {
                        flag8 = false;
                    }
                    for (int num157 = num153; num157 < num154; num157++)
                    {
                        for (int num158 = 0; num158 < ((int)world.getWorldSurface()); num158 += 5)
                        {
                            if (world.getTile()[num157, num158].active && Statics.tileDungeon[world.getTile()[num157, num158].type])
                            {
                                flag8 = false;
                                break;
                            }
                            if (!flag8)
                            {
                                break;
                            }
                        }
                    }
                }
                int num159 = 0;
                for (int num160 = num153; num160 < num154; num160++)
                {
                    if (num159 > 0)
                    {
                        num159--;
                    }
                    if ((num160 == num152) || (num159 == 0))
                    {
                        for (int num161 = (int)num2; num161 < (world.getWorldSurface() - 1.0); num161++)
                        {
                            if (world.getTile()[num160, num161].active || (world.getTile()[num160, num161].wall > 0))
                            {
                                if (num160 == num152)
                                {
                                    num159 = 20;
                                    ChasmRunner(num160, num161, genRand.Next(150) + 150, world, true);
                                }
                                else if ((genRand.Next(30) == 0) && (num159 == 0))
                                {
                                    num159 = 20;
                                    bool makeOrb = false;
                                    if (genRand.Next(2) == 0)
                                    {
                                        makeOrb = true;
                                    }
                                    ChasmRunner(num160, num161, genRand.Next(50) + 50, world, makeOrb);
                                }
                                break;
                            }
                        }
                    }
                }
                double num162 = world.getWorldSurface() + 40.0;
                for (int num163 = num153; num163 < num154; num163++)
                {
                    num162 += genRand.Next(-2, 3);
                    if (num162 < (world.getWorldSurface() + 30.0))
                    {
                        num162 = world.getWorldSurface() + 30.0;
                    }
                    if (num162 > (world.getWorldSurface() + 50.0))
                    {
                        num162 = world.getWorldSurface() + 50.0;
                    }
                    num62 = num163;
                    bool flag10 = false;
                    for (int num164 = (int)num2; num164 < num162; num164++)
                    {
                        if (world.getTile()[num62, num164].active)
                        {
                            if (((world.getTile()[num62, num164].type == 0) && (num164 < (world.getWorldSurface() - 1.0))) && !flag10)
                            {
                                SpreadGrass(num62, num164, world, 0, 0x17, true);
                            }
                            flag10 = true;
                            if (((world.getTile()[num62, num164].type == 1) && (num62 >= (num153 + genRand.Next(5)))) && (num62 <= (num154 - genRand.Next(5))))
                            {
                                world.getTile()[num62, num164].type = 0x19;
                            }
                            if (world.getTile()[num62, num164].type == 2)
                            {
                                world.getTile()[num62, num164].type = 0x17;
                            }
                        }
                    }
                }
                for (int num165 = num153; num165 < num154; num165++)
                {
                    for (int num166 = 0; num166 < (world.getMaxTilesY() - 50); num166++)
                    {
                        if (world.getTile()[num165, num166].active && (world.getTile()[num165, num166].type == 0x1f))
                        {
                            int num167 = num165 - 13;
                            int num168 = num165 + 13;
                            int num169 = num166 - 13;
                            int num170 = num166 + 13;
                            for (int num171 = num167; num171 < num168; num171++)
                            {
                                if ((num171 > 10) && (num171 < (world.getMaxTilesX() - 10)))
                                {
                                    for (int num172 = num169; num172 < num170; num172++)
                                    {
                                        if ((((Math.Abs((int)(num171 - num165)) + Math.Abs((int)(num172 - num166))) < (9 + genRand.Next(11))) && (genRand.Next(3) != 0)) && (world.getTile()[num171, num172].type != 0x1f))
                                        {
                                            world.getTile()[num171, num172].active = true;
                                            world.getTile()[num171, num172].type = 0x19;
                                            if ((Math.Abs((int)(num171 - num165)) <= 1) && (Math.Abs((int)(num172 - num166)) <= 1))
                                            {
                                                world.getTile()[num171, num172].active = false;
                                            }
                                        }
                                        if (((world.getTile()[num171, num172].type != 0x1f) && (Math.Abs((int)(num171 - num165)) <= (2 + genRand.Next(3)))) && (Math.Abs((int)(num172 - num166)) <= (2 + genRand.Next(3))))
                                        {
                                            world.getTile()[num171, num172].active = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Generating mountain caves...");
            for (int num173 = 0; num173 < numMCaves; num173++)
            {
                int num174 = mCaveX[num173];
                int num175 = mCaveY[num173];
                CaveOpenater(num174, num175, world);
                Cavinator(num174, num175, genRand.Next(40, 50), world);
            }
            Console.WriteLine();
            Program.printData("Creating beaches...");
            for (int num176 = 0; num176 < 2; num176++)
            {
                int num177 = 0;
                int num178 = 0;
                if (num176 == 0)
                {
                    num177 = 0;
                    num178 = genRand.Next(0x7d, 200);
                    float num179 = 1f;
                    int num180 = 0;
                    while (!world.getTile()[num178 - 1, num180].active)
                    {
                        num180++;
                    }
                    for (int num181 = num178 - 1; num181 >= num177; num181--)
                    {
                        num179 += genRand.Next(10, 20) * 0.05f;
                        for (int num182 = 0; num182 < (num180 + num179); num182++)
                        {
                            if (num182 < ((num180 + (num179 * 0.75f)) - 3f))
                            {
                                world.getTile()[num181, num182].active = false;
                                if (num182 > num180)
                                {
                                    world.getTile()[num181, num182].liquid = 0xff;
                                }
                                else if (num182 == num180)
                                {
                                    world.getTile()[num181, num182].liquid = 0x7f;
                                }
                            }
                            else if (num182 > num180)
                            {
                                world.getTile()[num181, num182].type = 0x35;
                                world.getTile()[num181, num182].active = true;
                            }
                            world.getTile()[num181, num182].wall = 0;
                        }
                    }
                }
                else
                {
                    num177 = world.getMaxTilesX() - genRand.Next(0x7d, 200);
                    num178 = world.getMaxTilesX();
                    float num183 = 1f;
                    int num184 = 0;
                    while (!world.getTile()[num177, num184].active)
                    {
                        num184++;
                    }
                    for (int num185 = num177; num185 < num178; num185++)
                    {
                        num183 += genRand.Next(10, 20) * 0.05f;
                        for (int num186 = 0; num186 < (num184 + num183); num186++)
                        {
                            if (num186 < ((num184 + (num183 * 0.75f)) - 3f))
                            {
                                world.getTile()[num185, num186].active = false;
                                if (num186 > num184)
                                {
                                    world.getTile()[num185, num186].liquid = 0xff;
                                }
                                else if (num186 == num184)
                                {
                                    world.getTile()[num185, num186].liquid = 0x7f;
                                }
                            }
                            else if (num186 > num184)
                            {
                                world.getTile()[num185, num186].type = 0x35;
                                world.getTile()[num185, num186].active = true;
                            }
                            world.getTile()[num185, num186].wall = 0;
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Adding gems...");
            for (int num187 = 0x3f; num187 <= 0x44; num187++)
            {
                float num188 = 0f;
                switch (num187)
                {
                    case 0x43:
                        num188 = world.getMaxTilesX() * 0.5f;
                        break;

                    case 0x42:
                        num188 = world.getMaxTilesX() * 0.45f;
                        break;

                    case 0x3f:
                        num188 = world.getMaxTilesX() * 0.3f;
                        break;

                    case 0x41:
                        num188 = world.getMaxTilesX() * 0.25f;
                        break;

                    case 0x40:
                        num188 = world.getMaxTilesX() * 0.1f;
                        break;

                    case 0x44:
                        num188 = world.getMaxTilesX() * 0.05f;
                        break;
                }
                num188 *= 0.2f;
                for (int num189 = 0; num189 < num188; num189++)
                {
                    int num190 = genRand.Next(0, world.getMaxTilesX());
                    int num191 = genRand.Next((int)world.getWorldSurface(), world.getMaxTilesY());
                    while (world.getTile()[num190, num191].type != 1)
                    {
                        num190 = genRand.Next(0, world.getMaxTilesX());
                        num191 = genRand.Next((int)world.getWorldSurface(), world.getMaxTilesY());
                    }
                    TileRunner(num190, num191, world, (double)genRand.Next(2, 6), genRand.Next(3, 7), num187, false, 0f, 0f, false, true);
                }
            }
            Console.WriteLine();
            for (int num192 = 0; num192 < world.getMaxTilesX(); num192++)
            {
                float num193 = ((float)num192) / ((float)(world.getMaxTilesX() - 1));
                Program.printData("Gravitating sand: " + ((int)(num193 * 100f)) + "%");
                for (int num194 = world.getMaxTilesY() - 5; num194 > 0; num194--)
                {
                    if (world.getTile()[num192, num194].active && (world.getTile()[num192, num194].type == 0x35))
                    {
                        for (int num195 = num194; !world.getTile()[num192, num195 + 1].active && (num195 < (world.getMaxTilesY() - 5)); num195++)
                        {
                            world.getTile()[num192, num195 + 1].active = true;
                            world.getTile()[num192, num195 + 1].type = 0x35;
                        }
                    }
                }
            }
            Console.WriteLine();
            for (int num196 = 3; num196 < (world.getMaxTilesX() - 3); num196++)
            {
                float num197 = ((float)num196) / ((float)world.getMaxTilesX());
                Program.printData("Cleaning up dirt backgrounds: " + ((int)((num197 * 100f) + 1f)) + "%");
                for (int num198 = 0; num198 < world.getWorldSurface(); num198++)
                {
                    if (world.getTile()[num196, num198].wall == 2)
                    {
                        world.getTile()[num196, num198].wall = 0;
                    }
                    if (world.getTile()[num196, num198].type != 0x35)
                    {
                        if (world.getTile()[num196 - 1, num198].wall == 2)
                        {
                            world.getTile()[num196 - 1, num198].wall = 0;
                        }
                        if ((world.getTile()[num196 - 2, num198].wall == 2) && (genRand.Next(2) == 0))
                        {
                            world.getTile()[num196 - 2, num198].wall = 0;
                        }
                        if ((world.getTile()[num196 - 3, num198].wall == 2) && (genRand.Next(2) == 0))
                        {
                            world.getTile()[num196 - 3, num198].wall = 0;
                        }
                        if (world.getTile()[num196 + 1, num198].wall == 2)
                        {
                            world.getTile()[num196 + 1, num198].wall = 0;
                        }
                        if ((world.getTile()[num196 + 2, num198].wall == 2) && (genRand.Next(2) == 0))
                        {
                            world.getTile()[num196 + 2, num198].wall = 0;
                        }
                        if ((world.getTile()[num196 + 3, num198].wall == 2) && (genRand.Next(2) == 0))
                        {
                            world.getTile()[num196 + 3, num198].wall = 0;
                        }
                        if (world.getTile()[num196, num198].active)
                        {
                            break;
                        }
                    }
                }
            }
            Console.WriteLine();
            for (int num199 = 0; num199 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 2E-05)); num199++)
            {
                float num200 = (float)(((double)num199) / ((world.getMaxTilesX() * world.getMaxTilesY()) * 2E-05));
                Program.printData("Placing alters: " + ((int)((num200 * 100f) + 1f)) + "%");
                bool flag11 = false;
                int num201 = 0;
                while (!flag11)
                {
                    int num202 = genRand.Next(1, world.getMaxTilesX());
                    int num203 = (int)(num3 + 20.0);
                    Place3x2(num202, num203, 0x1a, world);
                    if (world.getTile()[num202, num203].type == 0x1a)
                    {
                        flag11 = true;
                    }
                    else
                    {
                        num201++;
                        if (num201 >= 0x2710)
                        {
                            flag11 = true;
                        }
                    }
                }
            }
            Liquid.QuickWater(world, 3, -1, -1);
            WaterCheck(world);
            int num204 = 0;
            Liquid.quickSettle = true;
            //Console.WriteLine();
            while (num204 < 10)
            {
                int num205 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                num204++;
                float num206 = 0f;
                while (Liquid.numLiquid > 0)
                {
                    float num207 = ((float)(num205 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer))) / ((float)num205);
                    if ((Liquid.numLiquid + LiquidBuffer.numLiquidBuffer) > num205)
                    {
                        num205 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                    }
                    if (num207 > num206)
                    {
                        num206 = num207;
                    }
                    else
                    {
                        num207 = num206;
                    }
                    if (num204 == 1)
                    {
                        Program.printData("Settling liquids: " + ((int)(((num207 * 100f) / 3f) + 33f)) + "%");
                    }
                    int num208 = 10;
                    if (num204 > num208)
                    {
                        num208 = num204;
                    }
                    Liquid.UpdateLiquid(world);
                }
                WaterCheck(world);
                Program.printData("Settling liquids: " + ((int)(((num204 * 10f) / 3f) + 66f)) + "%");
            }
            Liquid.quickSettle = false;
            Console.WriteLine();
            for (int num209 = 0; num209 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 2.5E-05)); num209++)
            {
                float num210 = (float)(((double)num209) / ((world.getMaxTilesX() * world.getMaxTilesY()) * 2.5E-05));
                Program.printData("Placing life crystals: " + ((int)((num210 * 100f) + 1f)) + "%");
                bool flag12 = false;
                int num211 = 0;
                while (!flag12)
                {
                    if (AddLifeCrystal(genRand.Next(1, world.getMaxTilesX()), genRand.Next((int)(num3 + 20.0), world.getMaxTilesY()), world))
                    {
                        flag12 = true;
                    }
                    else
                    {
                        num211++;
                        if (num211 >= 0x2710)
                        {
                            flag12 = true;
                        }
                    }
                }
            }
            Console.WriteLine();
            for (int num212 = 0; num212 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 1.8E-05)); num212++)
            {
                float num213 = (float)(((double)num212) / ((world.getMaxTilesX() * world.getMaxTilesY()) * 1.8E-05));
                Program.printData("Hiding treasure: " + ((int)((num213 * 100f) + 1f)) + "%");
                bool flag13 = false;
                int num214 = 0;
                while (!flag13)
                {
                    if (AddBuriedChest(genRand.Next(1, world.getMaxTilesX()), genRand.Next((int)(num3 + 20.0), world.getMaxTilesY()), world, 0))
                    {
                        flag13 = true;
                    }
                    else
                    {
                        num214++;
                        if (num214 >= 0x2710)
                        {
                            flag13 = true;
                        }
                    }
                }
            }
            int num215 = 0;
            for (int num216 = 0; num216 < numJChests; num216++)
            {
                num215++;
                int contain = 0xd3;
                switch (num215)
                {
                    case 1:
                        contain = 0xd3;
                        break;

                    case 2:
                        contain = 0xd4;
                        break;

                    case 3:
                        contain = 0xd5;
                        break;
                }
                if (num215 > 3)
                {
                    num215 = 0;
                }
                if (!AddBuriedChest(JChestX[num216] + genRand.Next(2), JChestY[num216], world, contain))
                {
                    for (int num218 = JChestX[num216]; num218 <= (JChestX[num216] + 1); num218++)
                    {
                        for (int num219 = JChestY[num216]; num219 <= (JChestY[num216] + 1); num219++)
                        {
                            KillTile(num218, num219, world, false, false, false);
                        }
                    }
                    AddBuriedChest(JChestX[num216], JChestY[num216], world, contain);
                }
            }
            float num220 = world.getMaxTilesX() / 0x1068;
            int num221 = 0;
            for (int num222 = 0; num222 < (10f * num220); num222++)
            {
                int num224;
                int num225;
                int num223 = 0;
                num221++;
                if (num221 == 1)
                {
                    num223 = 0xba;
                }
                else
                {
                    num223 = 0xbb;
                    num221 = 0;
                }
                for (bool flag14 = false; !flag14; flag14 = AddBuriedChest(num224, num225, world, num223))
                {
                    num224 = genRand.Next(1, world.getMaxTilesX());
                    for (num225 = genRand.Next(1, world.getMaxTilesY() - 200); (world.getTile()[num224, num225].liquid < 200) || world.getTile()[num224, num225].lava; num225 = genRand.Next(1, world.getMaxTilesY() - 200))
                    {
                        num224 = genRand.Next(1, world.getMaxTilesX());
                    }
                }
            }
            for (int num226 = 0; num226 < numIslandHouses; num226++)
            {
                IslandHouse(fihX[num226], fihY[num226], world);
            }
            Console.WriteLine();
            for (int num227 = 0; num227 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 0.0008)); num227++)
            {
                float num228 = (float)(((double)num227) / ((world.getMaxTilesX() * world.getMaxTilesY()) * 0.0008));
                Program.printData("Placing breakables: " + ((int)((num228 * 100f) + 1f)) + "%");
                bool flag15 = false;
                int num229 = 0;
            Label_46C1:
                while (!flag15)
                {
                    int num230 = genRand.Next((int)num3, world.getMaxTilesY() - 10);
                    if (num228 > 0.93)
                    {
                        num230 = world.getMaxTilesY() - 150;
                    }
                    else if (num228 > 0.75)
                    {
                        num230 = (int)num2;
                    }
                    int num231 = genRand.Next(1, world.getMaxTilesX());
                    bool flag16 = false;
                    for (int num232 = num230; num232 < world.getMaxTilesY(); num232++)
                    {
                        if (!flag16)
                        {
                            if ((world.getTile()[num231, num232].active && Statics.tileSolid[world.getTile()[num231, num232].type])
                                && !world.getTile()[num231, num232 - 1].lava)
                            {
                                flag16 = true;
                            }
                        }
                        else
                        {
                            if (PlacePot(num231, num232, world, 0x1c))
                            {
                                flag15 = true;
                                goto Label_46C1;
                            }
                            num229++;
                            if (num229 >= 0x2710)
                            {
                                flag15 = true;
                                goto Label_46C1;
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
            for (int num233 = 0; num233 < ((int)((world.getMaxTilesX() * world.getMaxTilesY()) * 1E-05)); num233++)
            {
                float num234 = (float)(((double)num233) / ((world.getMaxTilesX() * world.getMaxTilesY()) * 1E-05));
                Program.printData("Placing hellforges: " + ((int)((num234 * 100f) + 1f)) + "%");
                bool flag17 = false;
                int num235 = 0;
                while (!flag17)
                {
                    int num236 = genRand.Next(1, world.getMaxTilesX());
                    int num237 = genRand.Next(world.getMaxTilesY() - 250, world.getMaxTilesY() - 5);
                    if (world.getTile()[num236, num237].wall == 13)
                    {
                        while (!world.getTile()[num236, num237].active)
                        {
                            num237++;
                        }
                        num237--;
                        PlaceTile(num236, num237, world, 0x4d, false, false, -1);
                        if (world.getTile()[num236, num237].type == 0x4d)
                        {
                            flag17 = true;
                        }
                        else
                        {
                            num235++;
                            if (num235 >= 0x2710)
                            {
                                flag17 = true;
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Spreading grass...");
            for (int num238 = 0; num238 < world.getMaxTilesX(); num238++)
            {
                num62 = num238;
                bool flag18 = true;
                for (int num239 = 0; num239 < (world.getWorldSurface() - 1.0); num239++)
                {
                    if (world.getTile()[num62, num239].active)
                    {
                        if (flag18 && (world.getTile()[num62, num239].type == 0))
                        {
                            SpreadGrass(num62, num239, world, 0, 2, true);
                        }
                        if (num239 > num3)
                        {
                            break;
                        }
                        flag18 = false;
                    }
                    else if (world.getTile()[num62, num239].wall == 0)
                    {
                        flag18 = true;
                    }
                }
            }
            int num240 = 5;
            bool flag19 = true;
            while (flag19)
            {
                int num241 = (world.getMaxTilesX() / 2) + genRand.Next(-num240, num240 + 1);
                for (int num242 = 0; num242 < world.getMaxTilesY(); num242++)
                {
                    if (world.getTile()[num241, num242].active)
                    {
                        Statics.spawnTileX = num241;
                        Statics.spawnTileY = num242;
                        world.getTile()[num241, num242 - 1].lighted = true;
                        break;
                    }
                }
                flag19 = false;
                num240++;
                if (Statics.spawnTileY > world.getWorldSurface())
                {
                    flag19 = true;
                }
                if (world.getTile()[Statics.spawnTileX, Statics.spawnTileY - 1].liquid > 0)
                {
                    flag19 = true;
                }
            }
            for (int num243 = 10; Statics.spawnTileY > world.getWorldSurface(); num243++)
            {
                int num244 = genRand.Next((world.getMaxTilesX() / 2) - num243, (world.getMaxTilesX() / 2) + num243);
                for (int num245 = 0; num245 < world.getMaxTilesY(); num245++)
                {
                    if (world.getTile()[num244, num245].active)
                    {
                        Statics.spawnTileX = num244;
                        Statics.spawnTileY = num245;
                        world.getTile()[num244, num245 - 1].lighted = true;
                        break;
                    }
                }
            }
            int index = NPC.NewNPC(Statics.spawnTileX * 0x10, Statics.spawnTileY * 0x10, world, 0x16, 0);
            world.getNPCs()[index].homeTileX = Statics.spawnTileX;
            world.getNPCs()[index].homeTileY = Statics.spawnTileY;
            world.getNPCs()[index].direction = 1;
            world.getNPCs()[index].homeless = true;
            Console.WriteLine();
            Program.printData("Planting sunflowers...");
            for (int num247 = 0; num247 < (world.getMaxTilesX() * 0.002); num247++)
            {
                int num248 = 0;
                int num249 = 0;
                int num250 = 0;
                int num1 = world.getMaxTilesX() / 2;
                num248 = genRand.Next(world.getMaxTilesX());
                num249 = (num248 - genRand.Next(10)) - 7;
                num250 = (num248 + genRand.Next(10)) + 7;
                if (num249 < 0)
                {
                    num249 = 0;
                }
                if (num250 > (world.getMaxTilesX() - 1))
                {
                    num250 = world.getMaxTilesX() - 1;
                }
                for (int num251 = num249; num251 < num250; num251++)
                {
                    for (int num252 = 1; num252 < (world.getWorldSurface() - 1.0); num252++)
                    {
                        if ((world.getTile()[num251, num252].type == 1) && world.getTile()[num251, num252].active)
                        {
                            world.getTile()[num251, num252].type = 2;
                        }
                        if ((world.getTile()[num251 + 1, num252].type == 1) && world.getTile()[num251 + 1, num252].active)
                        {
                            world.getTile()[num251 + 1, num252].type = 2;
                        }
                        if (((world.getTile()[num251, num252].type == 2) && world.getTile()[num251, num252].active) && !world.getTile()[num251, num252 - 1].active)
                        {
                            PlaceTile(num251, num252 - 1, world, 0x1b, true, false, -1);
                        }
                        if (world.getTile()[num251, num252].active)
                        {
                            break;
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Planting trees...");
            for (int num253 = 0; num253 < (world.getMaxTilesX() * 0.003); num253++)
            {
                int num254 = genRand.Next(50, world.getMaxTilesX() - 50);
                int num255 = genRand.Next(0x19, 50);
                for (int num256 = num254 - num255; num256 < (num254 + num255); num256++)
                {
                    for (int num257 = 20; num257 < world.getWorldSurface(); num257++)
                    {
                        if (world.getTile()[num256, num257].active)
                        {
                            if (world.getTile()[num256, num257].type == 1)
                            {
                                world.getTile()[num256, num257].type = 2;
                            }
                            if (world.getTile()[num256, num257 + 1].type == 1)
                            {
                                world.getTile()[num256, num257 + 1].type = 2;
                            }
                            break;
                        }
                    }
                }
                for (int num258 = num254 - num255; num258 < (num254 + num255); num258++)
                {
                    for (int num259 = 20; num259 < world.getWorldSurface(); num259++)
                    {
                        GrowEpicTree(num258, num259, world);
                    }
                }
            }
            AddTrees(world);
            Console.WriteLine();
            Program.printData("Planting weeds...");
            AddPlants(world);
            for (int num260 = 0; num260 < world.getMaxTilesX(); num260++)
            {
                for (int num261 = (int)world.getWorldSurface(); num261 < world.getMaxTilesY(); num261++)
                {
                    if (world.getTile()[num260, num261].active)
                    {
                        if ((world.getTile()[num260, num261].type == 70) && !world.getTile()[num260, num261 - 1].active)
                        {
                            GrowShroom(num260, num261, world);
                            if (!world.getTile()[num260, num261 - 1].active)
                            {
                                PlaceTile(num260, num261 - 1, world, 0x47, true, false, -1);
                            }
                        }
                        if ((world.getTile()[num260, num261].type == 60) && !world.getTile()[num260, num261 - 1].active)
                        {
                            PlaceTile(num260, num261 - 1, world, 0x3d, true, false, -1);
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Growing vines...");
            for (int num262 = 0; num262 < world.getMaxTilesX(); num262++)
            {
                int num263 = 0;
                for (int num264 = 0; num264 < world.getWorldSurface(); num264++)
                {
                    if ((num263 > 0) && !world.getTile()[num262, num264].active)
                    {
                        world.getTile()[num262, num264].active = true;
                        world.getTile()[num262, num264].type = 0x34;
                        num263--;
                    }
                    else
                    {
                        num263 = 0;
                    }
                    if ((world.getTile()[num262, num264].active && (world.getTile()[num262, num264].type == 2)) && (genRand.Next(5) < 3))
                    {
                        num263 = genRand.Next(1, 10);
                    }
                }
                num263 = 0;
                for (int num265 = (int)world.getWorldSurface(); num265 < world.getMaxTilesY(); num265++)
                {
                    if ((num263 > 0) && !world.getTile()[num262, num265].active)
                    {
                        world.getTile()[num262, num265].active = true;
                        world.getTile()[num262, num265].type = 0x3e;
                        num263--;
                    }
                    else
                    {
                        num263 = 0;
                    }
                    if ((world.getTile()[num262, num265].active && (world.getTile()[num262, num265].type == 60)) && (genRand.Next(5) < 3))
                    {
                        num263 = genRand.Next(1, 10);
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Planting flowers...");
            for (int num266 = 0; num266 < (world.getMaxTilesX() * 0.005); num266++)
            {
                int num267 = genRand.Next(20, world.getMaxTilesX() - 20);
                int num268 = genRand.Next(5, 15);
                int num269 = genRand.Next(15, 30);
                for (int num270 = 1; num270 < (world.getWorldSurface() - 1.0); num270++)
                {
                    if (world.getTile()[num267, num270].active)
                    {
                        for (int num271 = num267 - num268; num271 < (num267 + num268); num271++)
                        {
                            for (int num272 = num270 - num269; num272 < (num270 + num269); num272++)
                            {
                                if ((world.getTile()[num271, num272].type == 3) || (world.getTile()[num271, num272].type == 0x18))
                                {
                                    world.getTile()[num271, num272].frameX = (short)(genRand.Next(6, 8) * 0x12);
                                }
                            }
                        }
                        break;
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Planting mushrooms...");
            for (int num273 = 0; num273 < (world.getMaxTilesX() * 0.002); num273++)
            {
                int num274 = genRand.Next(20, world.getMaxTilesX() - 20);
                int num275 = genRand.Next(4, 10);
                int num276 = genRand.Next(15, 30);
                for (int num277 = 1; num277 < (world.getWorldSurface() - 1.0); num277++)
                {
                    if (world.getTile()[num274, num277].active)
                    {
                        for (int num278 = num274 - num275; num278 < (num274 + num275); num278++)
                        {
                            for (int num279 = num277 - num276; num279 < (num277 + num276); num279++)
                            {
                                if ((world.getTile()[num278, num279].type == 3) || (world.getTile()[num278, num279].type == 0x18))
                                {
                                    world.getTile()[num278, num279].frameX = 0x90;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            gen = false;
            return world;
        }
        
        public static int numMCaves { get; set; }

        public static int numIslandHouses { get; set; }

        public static int houseCount { get; set; }

        public static int dEnteranceX { get; set; }

        public static int numDRooms { get; set; }

        public static int numDDoors { get; set; }

        public static int numDPlats { get; set; }

        public static int numJChests { get; set; }

        public static int lavaLine { get; set; }

        public static Vector2 lastDungeonHall { get; set; }

        public static int dungeonY { get; set; }

        public static int dungeonX { get; set; }
    }
}
