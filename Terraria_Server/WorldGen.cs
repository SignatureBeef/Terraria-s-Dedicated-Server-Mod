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

        public static bool EmptyTileCheck(int startX, int endX, int startY, int endY, World world, int ignoreStyle = -1)
        {
            if (startX < 0)
            {
                return false;
            }
            if (endX >= Statics.maxTilesX)
            {
                return false;
            }
            if (startY < 0)
            {
                return false;
            }
            if (endY >= Statics.maxTilesY)
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
                                    if ((bestX > 10) && (bestX < (Statics.maxTilesX - 10)))
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
                if (maxTilesX > Statics.maxTilesX)
                {
                    maxTilesX = Statics.maxTilesX;
                }
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (maxTilesY > Statics.maxTilesY)
                {
                    maxTilesY = Statics.maxTilesY;
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
            for (int i = 0; i < ((Statics.maxTilesX * Statics.maxTilesY) * num); i++)
            {
                int num5 = genRand.Next(10, Statics.maxTilesX - 10);
                int num6 = genRand.Next(10, ((int)world.getWorldSurface()) - 1);
                int num7 = num5 - 1;
                int num8 = num5 + 2;
                int num9 = num6 - 1;
                int num10 = num6 + 2;
                if (num7 < 10)
                {
                    num7 = 10;
                }
                if (num8 > (Statics.maxTilesX - 10))
                {
                    num8 = Statics.maxTilesX - 10;
                }
                if (num9 < 10)
                {
                    num9 = 10;
                }
                if (num10 > (Statics.maxTilesY - 10))
                {
                    num10 = Statics.maxTilesY - 10;
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
            for (int j = 0; j < ((Statics.maxTilesX * Statics.maxTilesY) * num2); j++)
            {
                int num29 = genRand.Next(10, Statics.maxTilesX - 10);
                int num30 = genRand.Next(((int)world.getWorldSurface()) + 2, Statics.maxTilesY - 200);
                int num31 = num29 - 1;
                int num32 = num29 + 2;
                int num33 = num30 - 1;
                int num34 = num30 + 2;
                if (num31 < 10)
                {
                    num31 = 10;
                }
                if (num32 > (Statics.maxTilesX - 10))
                {
                    num32 = Statics.maxTilesX - 10;
                }
                if (num33 < 10)
                {
                    num33 = 10;
                }
                if (num34 > (Statics.maxTilesY - 10))
                {
                    num34 = Statics.maxTilesY - 10;
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
                float num55 = Statics.maxTilesX / 0x1068;
                if (Statics.rand.Next(0x1f40) < (10f * num55))
                {
                    int num56 = 12;
                    int num57 = Statics.rand.Next(Statics.maxTilesX - 50) + 100;
                    num57 *= 0x10;
                    int num58 = Statics.rand.Next((int)(Statics.maxTilesY * 0.05)) * 0x10;
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
            if (num8 >= Statics.maxTilesX)
            {
                num8 = Statics.maxTilesX - 1;
            }
            if (num9 < 0)
            {
                num9 = 0;
            }
            if (maxTilesX > Statics.maxTilesX)
            {
                maxTilesX = Statics.maxTilesX;
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
                (Statics.maxTilesX - 10)) && (world.getNPCs()[npc].homeTileY < Statics.maxTilesY)))
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
            if (((i >= 0) && (j >= 0)) && ((i < Statics.maxTilesX) && (j < Statics.maxTilesY)))
            {
                if (world.getTile()[i, j] == null)
                {
                    world.getTile()[i, j] = new Tile();
                }
                if (world.getTile()[i, j].wall > 0)
                {
                    genRand.Next(3);
                    //Main.PlaySound(0, i * 0x10, j * 0x10, 1);
                    //int num = 10;
                    //if (fail)
                    //{
                    //    num = 3;
                    //}
                    /*for (int k = 0; k < num; k++)
                    {
                        //int type = 0;
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
                        //Color newColor = new Color();
                        //Dust.NewDust(new Vector2((float)(i * 0x10), (float)(j * 0x10)), 0x10, 0x10, type, 0f, 0f, 0, newColor, 1f);
                    }*/
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
            if ((((i >= 0) && (j >= 0)) && ((i < Statics.maxTilesX) && (j < Statics.maxTilesY))) && ((world.getTile()[i, j] != null) && (world.getTile()[i, j].wall > 0)))
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
                    if ((i + 1) >= Statics.maxTilesX)
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
                    if ((j + 1) >= Statics.maxTilesY)
                    {
                        num6 = wall;
                        num7 = wall;
                        num8 = wall;
                    }
                    if (((i - 1) >= 0) && (world.getTile()[i - 1, j] != null))
                    {
                        num4 = world.getTile()[i - 1, j].wall;
                    }
                    if (((i + 1) < Statics.maxTilesX) && (world.getTile()[i + 1, j] != null))
                    {
                        num5 = world.getTile()[i + 1, j].wall;
                    }
                    if (((j - 1) >= 0) && (world.getTile()[i, j - 1] != null))
                    {
                        num2 = world.getTile()[i, j - 1].wall;
                    }
                    if (((j + 1) < Statics.maxTilesY) && (world.getTile()[i, j + 1] != null))
                    {
                        num7 = world.getTile()[i, j + 1].wall;
                    }
                    if ((((i - 1) >= 0) && ((j - 1) >= 0)) && (world.getTile()[i - 1, j - 1] != null))
                    {
                        num = world.getTile()[i - 1, j - 1].wall;
                    }
                    if ((((i + 1) < Statics.maxTilesX) && ((j - 1) >= 0)) && (world.getTile()[i + 1, j - 1] != null))
                    {
                        num3 = world.getTile()[i + 1, j - 1].wall;
                    }
                    if ((((i - 1) >= 0) && ((j + 1) < Statics.maxTilesY)) && (world.getTile()[i - 1, j + 1] != null))
                    {
                        num6 = world.getTile()[i - 1, j + 1].wall;
                    }
                    if ((((i + 1) < Statics.maxTilesX) && ((j + 1) < Statics.maxTilesY)) && (world.getTile()[i + 1, j + 1] != null))
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
            for (int i = 1; i < (Statics.maxTilesX - 1); i++)
            {
                for (int j = Statics.maxTilesY - 2; j > 0; j--)
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
            int maxTilesX = Statics.maxTilesX;
            int num4 = i + 1;
            int num5 = j - 1;
            if ((j + 1) >= Statics.maxTilesY)
            {
                num = type;
            }
            if ((((i - 1) >= 0) && (world.getTile()[i - 1, j] != null)) && world.getTile()[i - 1, j].active)
            {
                byte num6 = world.getTile()[i - 1, j].type;
            }
            if ((((i + 1) < Statics.maxTilesX) && (world.getTile()[i + 1, j] != null)) && world.getTile()[i + 1, j].active)
            {
                byte num7 = world.getTile()[i + 1, j].type;
            }
            if ((((j - 1) >= 0) && (world.getTile()[i, j - 1] != null)) && world.getTile()[i, j - 1].active)
            {
                byte num8 = world.getTile()[i, j - 1].type;
            }
            if ((((j + 1) < Statics.maxTilesY) && (world.getTile()[i, j + 1] != null)) && world.getTile()[i, j + 1].active)
            {
                num = world.getTile()[i, j + 1].type;
            }
            if ((((i - 1) >= 0) && ((j - 1) >= 0)) && ((world.getTile()[i - 1, j - 1] != null) && world.getTile()[i - 1, j - 1].active))
            {
                byte num9 = world.getTile()[i - 1, j - 1].type;
            }
            if ((((i + 1) < Statics.maxTilesX) && ((j - 1) >= 0)) && ((world.getTile()[i + 1, j - 1] != null) && world.getTile()[i + 1, j - 1].active))
            {
                byte num10 = world.getTile()[i + 1, j - 1].type;
            }
            if ((((i - 1) >= 0) && ((j + 1) < Statics.maxTilesY)) && ((world.getTile()[i - 1, j + 1] != null) && world.getTile()[i - 1, j + 1].active))
            {
                byte num11 = world.getTile()[i - 1, j + 1].type;
            }
            if ((((i + 1) < Statics.maxTilesX) && ((j + 1) < Statics.maxTilesY)) && ((world.getTile()[i + 1, j + 1] != null) && world.getTile()[i + 1, j + 1].active))
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
            if (((x >= 5) && (x <= (Statics.maxTilesX - 5))) && ((y >= 5) && (y <= (Statics.maxTilesY - 5))))
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
                if (num2 > Statics.maxTilesX)
                {
                    return false;
                }
                if (num3 < 0)
                {
                    return false;
                }
                if (num4 > Statics.maxTilesY)
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
            if (((x >= 5) && (x <= (Statics.maxTilesX - 5))) && ((y >= 5) && (y <= (Statics.maxTilesY - 5))))
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
            if (((i >= 0) && (j >= 0)) && ((i < Statics.maxTilesX) && (j < Statics.maxTilesY)))
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
                    if ((((j + 1) < Statics.maxTilesY) && world.getTile()[i, j + 1].active) && ((((world.getTile()[i, j + 1].type == 2) && (type == 3)) || ((world.getTile()[i, j + 1].type == 0x17) && (type == 0x18))) || ((world.getTile()[i, j + 1].type == 0x4e) && (type == 3))))
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
                    if ((((j + 1) < Statics.maxTilesY) && world.getTile()[i, j + 1].active) && (world.getTile()[i, j + 1].type == 60))
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
                    if ((((j + 1) < Statics.maxTilesY) && world.getTile()[i, j + 1].active) && (world.getTile()[i, j + 1].type == 70))
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
                        //Color newColor = new Color();
                        //Dust.NewDust(new Vector2((float)(i * 0x10), (float)(j * 0x10)), 0x10, 0x10, 14, 0f, 0f, 0, newColor, 1f);
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
                if (((x < 10) || (y < 10)) || ((x >= (Statics.maxTilesX - 10)) || (y >= (lastMaxTilesY - 10))))
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
                    //Gore.NewGore(new Vector2((float)(i * 0x10), (float)(j * 0x10)), new Vector2(), 0x33);
                    //Gore.NewGore(new Vector2((float)(i * 0x10), (float)(j * 0x10)), new Vector2(), 0x34);
                    //Gore.NewGore(new Vector2((float)(i * 0x10), (float)(j * 0x10)), new Vector2(), 0x35);
                    if (Statics.rand == null)
                    {
                        Statics.rand = new Random();
                    }
                    int num8 = Statics.rand.Next(10);
                    int player = Player.FindClosest(new Vector2((float)(i * 0x10), (float)(j * 0x10)), 0x10, 0x10, world);
                    //Player.FindClosest(new Vector2((float)(i * 0x10), (float)(j * 0x10)), 0x10, 0x10, world)
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
                if ((((num >= 0) && (num2 <= Statics.maxTilesX)) && (num3 >= 0)) && (num4 <= Statics.maxTilesY))
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
            if (((i >= 0) && (j >= 0)) && ((i < Statics.maxTilesX) && (j < Statics.maxTilesY)))
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
                                //Color newColor = new Color();
                                //Dust.NewDust(new Vector2((float)(i * 0x10), (float)(j * 0x10)), 0x10, 0x10, type, 0f, 0f, 0, newColor, 1f);
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
            if (i >= 0 && j >= 0 && i < Statics.maxTilesX && j < Statics.maxTilesY && world.getTile()[i, j] != null)
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
                    if (i + 1 >= Statics.maxTilesX)
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
                    if (j + 1 >= Statics.maxTilesY)
                    {
                        num6 = num9;
                        num7 = num9;
                        num8 = num9;
                    }
                    if (i - 1 >= 0 && world.getTile()[i - 1, j] != null && world.getTile()[i - 1, j].active)
                    {
                        num4 = (int)world.getTile()[i - 1, j].type;
                    }
                    if (i + 1 < Statics.maxTilesX && world.getTile()[i + 1, j] != null && world.getTile()[i + 1, j].active)
                    {
                        num5 = (int)world.getTile()[i + 1, j].type;
                    }
                    if (j - 1 >= 0 && world.getTile()[i, j - 1] != null && world.getTile()[i, j - 1].active)
                    {
                        num2 = (int)world.getTile()[i, j - 1].type;
                    }
                    if (j + 1 < Statics.maxTilesY && world.getTile()[i, j + 1] != null && world.getTile()[i, j + 1].active)
                    {
                        num7 = (int)world.getTile()[i, j + 1].type;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0 && world.getTile()[i - 1, j - 1] != null && world.getTile()[i - 1, j - 1].active)
                    {
                        num = (int)world.getTile()[i - 1, j - 1].type;
                    }
                    if (i + 1 < Statics.maxTilesX && j - 1 >= 0 && world.getTile()[i + 1, j - 1] != null && world.getTile()[i + 1, j - 1].active)
                    {
                        num3 = (int)world.getTile()[i + 1, j - 1].type;
                    }
                    if (i - 1 >= 0 && j + 1 < Statics.maxTilesY && world.getTile()[i - 1, j + 1] != null && world.getTile()[i - 1, j + 1].active)
                    {
                        num6 = (int)world.getTile()[i - 1, j + 1].type;
                    }
                    if (i + 1 < Statics.maxTilesX && j + 1 < Statics.maxTilesY && world.getTile()[i + 1, j + 1] != null && world.getTile()[i + 1, j + 1].active)
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
        
        /*public static void EveryTileFrame(World world)
        {
            noLiquidCheck = true;
            for (int i = 0; i < Statics.maxTilesX; i++)
            {
                float num2 = ((float)i) / ((float)Statics.maxTilesX);
                for (int i_ = 0; i_ < preserve; i_++)
                {
                    Console.Write("\b");
                }
                string text = "Finding tile frames: " + ((int)((num2 * 100f) + 1f)) + "%";
                Console.Write(text);
                preserve = text.Length;
                for (int j = 0; j < Statics.maxTilesY; j++)
                {
                    TileFrame(i, j, world, true, false);
                    WallFrame(i, j, world, true);
                }
            }
            noLiquidCheck = false;
        }*/

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
            if (num > (Statics.maxTilesX - 2))
            {
                num = Statics.maxTilesX - 2;
            }
            if (num3 > (Statics.maxTilesY - 2))
            {
                num3 = Statics.maxTilesY - 2;
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

        public static Random genRand { get; set; }

        /*public static World loadWorld(String WorldPath, Server server)
        {
            World world = new World(server, Statics.maxTilesX, Statics.maxTilesY);

            if (genRand == null)
            {
                genRand = new Random((int) DateTime.Now.Ticks);
            }
            using (FileStream stream = new FileStream(WorldPath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    //try
                    //{
                        if (reader.ReadInt32() > Statics.currentRelease)
                        {
                            //Main.menuMode = 15;
                            //Main.statusText = "Incompatible world file!";
                            Console.WriteLine("Incompatible world file!");
                            //loadFailed = true;
                            reader.Close();
                            return null;
                        }

                        Console.WriteLine("Compatible world file!");
                        //Main.worldName = reader.ReadString();
                        world.setName(reader.ReadString());
                        //Main.worldID = reader.ReadInt32();
                        world.setId(reader.ReadInt32());
                        //Main.leftWorld = reader.ReadInt32();
                        Statics.leftWorld = reader.ReadInt32();
                        //Main.rightWorld = reader.ReadInt32();
                        Statics.rightWorld = reader.ReadInt32();
                        //Main.topWorld = reader.ReadInt32();
                        Statics.topWorld = reader.ReadInt32();
                        //Main.bottomWorld = reader.ReadInt32();
                        Statics.bottomWorld = reader.ReadInt32();
                        //Main.maxTilesY = reader.ReadInt32();
                        Statics.maxTilesY = reader.ReadInt32();
                        //Main.maxTilesX = reader.ReadInt32();
                        Statics.maxTilesX = reader.ReadInt32();
                        clearWorld(world);
                        //Main.spawnTileX = reader.ReadInt32();
                        Statics.spawnTileX = reader.ReadInt32();
                        //Main.spawnTileY = reader.ReadInt32();
                        Statics.spawnTileY = reader.ReadInt32();
                        //Main.worldSurface = reader.ReadDouble();
                        world.setWorldSurface(reader.ReadDouble());
                        //Main.rockLayer = reader.ReadDouble();
                        world.setRockLayer(reader.ReadDouble());
                        //tempTime = reader.ReadDouble();
                        world.setTime(reader.ReadDouble());
                        //tempDayTime = reader.ReadBoolean();
                        world.setDayTime(reader.ReadBoolean());
                        //tempMoonPhase = reader.ReadInt32();
                        world.setMoonPhase(reader.ReadInt32());
                        //tempBloodMoon = reader.(ReadBoolean);
                        world.setBloodMoon(reader.ReadBoolean());
                        //Main.dungeonX = reader.ReadInt32();
                        world.setDungeonX(reader.ReadInt32());
                        //Main.dungeonY = reader.ReadInt32();
                        world.setDungeonX(reader.ReadInt32());
                        NPC.downedBoss1 = reader.ReadBoolean();
                        NPC.downedBoss2 = reader.ReadBoolean();
                        NPC.downedBoss3 = reader.ReadBoolean();

                        world.setShadowOrbSmashed(reader.ReadBoolean());
                        world.setSpawnMeteor(reader.ReadBoolean());
                        world.setShadowOrbCount(reader.ReadByte());

                        world.setInvasionDelay(reader.ReadInt32());
                        world.setInvasionSize(reader.ReadInt32());
                        world.setInvasionType(reader.ReadInt32());
                        world.setInvasionX(reader.ReadDouble());
                    
                        for (int i = 0; i < Statics.maxTilesX; i++)
                        {
                            float num3 = ((float) i) / ((float) Statics.maxTilesX);
                            //Main.statusText = "Loading world data: " + ((int) ((num3 * 100f) + 1f)) + "%";
                            for (int i_ = 0; i_ < preserve; i_++)
                            {
                                Console.Write("\b");
                            }
                            string text = "Loading world data: " + ((int)((num3 * 100f) + 1f)) + "%";
                            Console.Write(text);
                            preserve = text.Length;

                            for (int n = 0; n < Statics.maxTilesY; n++)
                            {
                                Tile tilez = world.getTile()[i, n];
                                tilez.active = reader.ReadBoolean();
                                if (tilez.active)
                                {
                                    tilez.type = reader.ReadByte();
                                    //Console.Write("{" + n.ToString() + "}");
                                    if (Statics.tileFrameImportant[tilez.type])
                                    {
                                        tilez.frameX = reader.ReadInt16();
                                        tilez.frameY = reader.ReadInt16();
                                    }
                                    else
                                    {
                                        tilez.frameX = -1;
                                        tilez.frameY = -1;
                                    }
                                }
                                tilez.lighted = reader.ReadBoolean();
                                if (reader.ReadBoolean())
                                {
                                    tilez.wall = reader.ReadByte();
                                }
                                if (reader.ReadBoolean())
                                {
                                    tilez.liquid = reader.ReadByte();
                                    tilez.lava = reader.ReadBoolean();
                                }
                                world.setTile(tilez, i, n);
                            }
                        }
                        Console.WriteLine();
                        for (int j = 0; j < 0x3e8; j++)
                        {
                            if (reader.ReadBoolean())
                            {
                                Chest[] chests = world.getChests();
                                chests[j] = new Chest();
                                chests[j].x = reader.ReadInt32();
                                chests[j].y = reader.ReadInt32();
                                for (int num6 = 0; num6 < Chest.maxItems; num6++)
                                {
                                    chests[j].item[num6] = new Item();
                                    byte num7 = reader.ReadByte();
                                    if (num7 > 0)
                                    {
                                        string itemName = reader.ReadString();
                                        chests[j].item[num6].SetDefaults(itemName);
                                        chests[j].item[num6].stack = num7;
                                    }
                                }
                                world.setChests(chests);
                            }
                        }
                        for (int k = 0; k < 1000; k++)
                        {
                            if (reader.ReadBoolean())
                            {
                                string str2 = reader.ReadString();
                                int num9 = reader.ReadInt32();
                                int num10 = reader.ReadInt32();
                                if (world.getTile()[num9, num10].active && (world.getTile()[num9, num10].type == 0x37))
                                {
                                    Sign[] signs = world.getSigns();
                                    signs[k] = new Sign();
                                    signs[k].x = num9;
                                    signs[k].y = num10;
                                    signs[k].text = str2;
                                    world.setSigns(signs);
                                }
                            }
                        }
                        bool flag = reader.ReadBoolean();
                        for (int m = 0; flag; m++)
                        {
                            //world.getNPCs()[m].SetDefaults(reader.ReadString());
                            reader.ReadString();
                            NPC[] npcs = world.getNPCs();
                            //npcs[m].position.X = reader.ReadSingle();
                            //npcs[m].position.Y = reader.ReadSingle();
                            //npcs[m].homeless = reader.ReadBoolean();
                            //npcs[m].homeTileX = reader.ReadInt32();
                            //npcs[m].homeTileY = reader.ReadInt32();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadBoolean();
                            reader.ReadInt32();
                            reader.ReadInt32();
                            flag = reader.ReadBoolean();
                            world.setNPCs(npcs);
                        }
                        reader.Close();
                        gen = true;
                        waterLine = Statics.maxTilesY;
                        Liquid.QuickWater(world, 2, -1, -1);
                        WaterCheck(world);
                        int num12 = 0;
                        Liquid.quickSettle = true;
                        int num13 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                        float num14 = 0f;
                        while ((Liquid.numLiquid > 0) && (num12 < 0x186a0))
                        {
                            num12++;
                            float num15 = ((float) (num13 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer))) / ((float) num13);
                            if ((Liquid.numLiquid + LiquidBuffer.numLiquidBuffer) > num13)
                            {
                                num13 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                            }
                            if (num15 > num14)
                            {
                                num14 = num15;
                            }
                            else
                            {
                                num15 = num14;
                            }
                           // Main.statusText = "Settling liquids: " + ((int) (((num15 * 100f) / 2f) + 50f)) + "%";
                            for (int i_ = 0; i_ < preserve; i_++)
                            {
                                Console.Write("\b");
                            }
                            string text = "Settling liquids: " + ((int)(((num15 * 100f) / 2f) + 50f)) + "%";
                            Console.Write(text);
                            preserve = text.Length;
                            Liquid.UpdateLiquid(world);
                        }
                        Liquid.quickSettle = false;
                        WaterCheck(world);
                        gen = false;
                   // }
                   // catch (Exception exception)
                   // {
                        //Main.menuMode = 15;
                        //Main.statusText = exception.ToString();
                      //  Console.WriteLine(exception.ToString());
                        //loadFailed = true;
                      //  try
                     //   {
                     //       reader.Close();
                     //   }
                     //   catch
                     //   {
                     //   }
                     //   return null;
                    //}
                    //loadFailed = false;
                    //return null;
                }
            }
            return world;
        }*/

        public static World loadWorld(String WorldPath, Server server)
        {
            World world = server.getWorld();
            world.setServer(server);

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
                            //Main.statusText = "Incompatible world file!";
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
                        Statics.leftWorld = binaryReader.ReadInt32();
                        //Main.rightWorld = reader.ReadInt32();
                        Statics.rightWorld = binaryReader.ReadInt32();
                        //Main.topWorld = reader.ReadInt32();
                        Statics.topWorld = binaryReader.ReadInt32();
                        //Main.bottomWorld = reader.ReadInt32();
                        Statics.bottomWorld = binaryReader.ReadInt32();
                        //Main.maxTilesY = reader.ReadInt32();
                        Statics.maxTilesY = binaryReader.ReadInt32();
                        //Main.maxTilesX = reader.ReadInt32();
                        Statics.maxTilesX = binaryReader.ReadInt32();
                        clearWorld(world);
                        //Main.spawnTileX = reader.ReadInt32();
                        Statics.spawnTileX = binaryReader.ReadInt32();
                        //Main.spawnTileY = reader.ReadInt32();
                        Statics.spawnTileY = binaryReader.ReadInt32();
                        //Main.worldSurface = reader.ReadDouble();
                        world.setWorldSurface(binaryReader.ReadDouble());
                        //Main.rockLayer = reader.ReadDouble();
                        world.setRockLayer(binaryReader.ReadDouble());
                        //tempTime = reader.ReadDouble();
                        world.setTime(binaryReader.ReadDouble());
                        //tempDayTime = reader.ReadBoolean();
                        world.setDayTime(binaryReader.ReadBoolean());
                        //tempMoonPhase = reader.ReadInt32();
                        world.setMoonPhase(binaryReader.ReadInt32());
                        //tempBloodMoon = reader.(ReadBoolean);
                        world.setBloodMoon(binaryReader.ReadBoolean());
                        //Main.dungeonX = reader.ReadInt32();
                        world.setDungeonX(binaryReader.ReadInt32());
                        //Main.dungeonY = reader.ReadInt32();
                        world.setDungeonX(binaryReader.ReadInt32());
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

                        for (int i = 0; i < Statics.maxTilesX; i++)
                        {
                            float num2 = (float)i / (float)Statics.maxTilesX;
                            for (int i_ = 0; i_ < preserve; i_++)
                            {
                                Console.Write("\b");
                            }
                            string text = "Loading world data: " + ((int)((num2 * 100f) + 1f)) + "%";
                            Console.Write(text);
                            preserve = text.Length;

                            for (int j = 0; j < Statics.maxTilesY; j++)
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
                        WorldGen.waterLine = Statics.maxTilesY;
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
                            //Main.statusText = "Settling liquids: " + (int)(num9 * 100f / 2f + 50f) + "%";
                            for (int i_ = 0; i_ < preserve; i_++)
                            {
                                Console.Write("\b");
                            }
                            string text = "Settling liquids: " + (int)(num9 * 100f / 2f + 50f) + "%";
                            Console.Write(text);
                            preserve = text.Length;
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
                        //Main.statusText = exception.ToString();
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
                    try {
                        binaryReader.Close();
                    } catch {
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
            //Main.dungeonX = 0;
            world.setDungeonX(0);
            //Main.dungeonY = 0;
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
            if (((Statics.netMode == 1) || (lastMaxTilesX > Statics.maxTilesX)) || (lastMaxTilesY > Statics.maxTilesY))
            {
                for (int num = 0; num < lastMaxTilesX; num++)
                {
                    float num2 = ((float)num) / ((float)lastMaxTilesX);
                    //Main.statusText = "Freeing unused resources: " + ((int)((num2 * 100f) + 1f)) + "%";
                    //Console.WriteLine("Freeing unused resources: " + ((int)((num2 * 100f) + 1f)) + "%");
                    for (int i_ = 0; i_ < preserve; i_++)
                    {
                        Console.Write("\b");
                    }
                    string text = "Freeing unused resources: " + ((int)((num2 * 100f) + 1f)) + "%";
                    Console.Write(text);
                    preserve = text.Length;
                    for (int num3 = 0; num3 < lastMaxTilesY; num3++)
                    {
                        world.getTile()[num, num3] = null;
                    }
                }
                Console.WriteLine();
            }
            lastMaxTilesX = Statics.maxTilesX;
            lastMaxTilesY = Statics.maxTilesY;
            if (Statics.netMode != 1)
            {
                for (int num4 = 0; num4 < Statics.maxTilesX; num4++)
                {
                    float num5 = ((float)num4) / ((float)Statics.maxTilesX);
                    //Main.statusText = "Resetting game objects: " + ((int)((num5 * 100f) + 1f)) + "%";
                    //Console.WriteLine("Resetting game objects: " + ((int)((num5 * 100f) + 1f)) + "%");
                    for (int i_ = 0; i_ < preserve; i_++)
                    {
                        Console.Write("\b");
                    }
                    string text = "Resetting game objects: " + ((int)((num5 * 100f) + 1f)) + "%";
                    Console.Write(text);
                    preserve = text.Length;
                    for (int num6 = 0; num6 < Statics.maxTilesY; num6++)
                    {
                        world.getTile()[num4, num6] = new Tile();

                    }
                }
                Console.WriteLine();
            }
            for (int i = 0; i < 0x7d0; i++)
            {
                //world.getDust()[i] = new Dust();
            }
            for (int j = 0; j < 200; j++)
            {
                //world.getGore()[j] = new Gore();
            }
            for (int k = 0; k < 200; k++)
            {
                world.getItemList()[k] = new Item();
            }
            for (int m = 0; m < 0x3e8; m++)
            {
                //world.getNPCs()[m] = new NPC();
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
            setWorldSize();
            //worldCleared = true;
        }

        public static void setWorldSize()
        {
            Statics.bottomWorld = Statics.maxTilesY * 0x10;
            Statics.rightWorld = Statics.maxTilesX * 0x10;
            Statics.maxSectionsX = Statics.maxTilesX / 200;
            Statics.maxSectionsY = Statics.maxTilesY / 150;
        }

        public static bool noLiquidCheck { get; set; }

        public static bool mergeUp { get; set; }

        public static bool mergeDown { get; set; }

        public static bool mergeLeft { get; set; }

        public static bool mergeRight { get; set; }

        public static bool destroyObject { get; set; }

        public static bool spawnMeteor { get; set; }

        public static bool shadowOrbSmashed { get; set; }

        public static int shadowOrbCount { get; set; }

        public static int roomX1 { get; set; }

        public static int roomX2 { get; set; }

        public static int roomY1 { get; set; }

        public static int roomY2 { get; set; }

        public static int numRoomTiles { get; set; }

        public static bool canSpawn { get; set; }
        
        public static bool[] houseTile = new bool[80];

        public static int maxRoomTiles = 0x76c;
        public static int[] roomX = new int[maxRoomTiles];
        public static int[] roomY = new int[maxRoomTiles];

        public static int waterLine { get; set; }

        public static bool spawnEye { get; set; }

        public static int spawnNPC { get; set; }

        public static int spawnDelay { get; set; }
        
        public static void EveryTileFrame(World world)
        {
            noLiquidCheck = true;
            for (int i = 0; i < Statics.maxTilesX; i++)
            {
                float num2 = ((float)i) / ((float)Statics.maxTilesX);
                //Main.statusText = "Finding tile frames: " + ((int)((num2 * 100f) + 1f)) + "%";

                for (int i_ = 0; i_ < preserve; i_++)
                {
                    Console.Write("\b");
                }
                string text = "Finding tile frames: " + ((int)((num2 * 100f) + 1f)) + "%";
                Console.Write(text);
                preserve = text.Length;
                for (int j = 0; j < Statics.maxTilesY; j++)
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
            if ((i < 50) || (i > (Statics.maxTilesX - 50)))
            {
                return false;
            }
            if ((j < 50) || (j > (Statics.maxTilesY - 50)))
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
                float num4 = Statics.maxTilesX / 0x1068;
                int num5 = (int)(400f * num4);
                for (int j = 5; j < (Statics.maxTilesX - 5); j++)
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
                    float num8 = Statics.maxTilesX * 0.08f;
                    int num9 = Statics.rand.Next(50, Statics.maxTilesX - 50);
                    while ((num9 > (Statics.spawnTileX - num8)) && (num9 < (Statics.spawnTileX + num8)))
                    {
                        num9 = Statics.rand.Next(50, Statics.maxTilesX - 50);
                    }
                    for (int m = Statics.rand.Next(100); m < Statics.maxTilesY; m++)
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


        public static bool stopDrops { get; set; }
    }
}
