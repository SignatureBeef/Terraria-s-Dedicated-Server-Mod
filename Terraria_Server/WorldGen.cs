using System;
using System.IO;
using System.Threading;

namespace Terraria_Server
{
    internal class WorldGen
    {
        public static int lavaLine;
        public static int waterLine;
        public static bool spawnEye = false;
        public static bool gen = false;
        public static bool shadowOrbSmashed = false;
        public static int shadowOrbCount = 0;
        public static bool spawnMeteor = false;
        public static bool loadFailed = false;
        public static bool loadSuccess = false;
        public static bool worldCleared = false;
        public static bool worldBackup = false;
        public static bool loadBackup = false;
        private static int lastMaxTilesX = 0;
        private static int lastMaxTilesY = 0;
        public static bool saveLock = false;
        private static bool mergeUp = false;
        private static bool mergeDown = false;
        private static bool mergeLeft = false;
        private static bool mergeRight = false;
        private static int tempMoonPhase = Main.moonPhase;
        private static bool tempDayTime = Main.dayTime;
        private static bool tempBloodMoon = Main.bloodMoon;
        private static double tempTime = Main.time;
        private static bool stopDrops = false;
        public static bool noLiquidCheck = false;
        [ThreadStatic]
        public static Random genRand = new Random();
        public static string statusText = "";
        private static bool destroyObject = false;
        public static int spawnDelay = 0;
        public static int spawnNPC = 0;
        public static int maxRoomTiles = 1900;
        public static int numRoomTiles;
        public static int[] roomX = new int[WorldGen.maxRoomTiles];
        public static int[] roomY = new int[WorldGen.maxRoomTiles];
        public static int roomX1;
        public static int roomX2;
        public static int roomY1;
        public static int roomY2;
        public static bool canSpawn;
        public static bool[] houseTile = new bool[80];
        public static int bestX = 0;
        public static int bestY = 0;
        public static int hiScore = 0;
        public static int dungeonX;
        public static int dungeonY;
        public static Vector2 lastDungeonHall = default(Vector2);
        public static int maxDRooms = 100;
        public static int numDRooms = 0;
        public static int[] dRoomX = new int[WorldGen.maxDRooms];
        public static int[] dRoomY = new int[WorldGen.maxDRooms];
        public static int[] dRoomSize = new int[WorldGen.maxDRooms];
        private static bool[] dRoomTreasure = new bool[WorldGen.maxDRooms];
        private static int[] dRoomL = new int[WorldGen.maxDRooms];
        private static int[] dRoomR = new int[WorldGen.maxDRooms];
        private static int[] dRoomT = new int[WorldGen.maxDRooms];
        private static int[] dRoomB = new int[WorldGen.maxDRooms];
        private static int numDDoors;
        private static int[] DDoorX = new int[300];
        private static int[] DDoorY = new int[300];
        private static int[] DDoorPos = new int[300];
        private static int numDPlats;
        private static int[] DPlatX = new int[300];
        private static int[] DPlatY = new int[300];
        private static int[] JChestX = new int[100];
        private static int[] JChestY = new int[100];
        private static int numJChests = 0;
        public static int dEnteranceX = 0;
        public static bool dSurface = false;
        private static double dxStrength1;
        private static double dyStrength1;
        private static double dxStrength2;
        private static double dyStrength2;
        private static int dMinX;
        private static int dMaxX;
        private static int dMinY;
        private static int dMaxY;
        private static int numIslandHouses = 0;
        private static int houseCount = 0;
        private static int[] fihX = new int[300];
        private static int[] fihY = new int[300];
        private static int numMCaves = 0;
        private static int[] mCaveX = new int[300];
        private static int[] mCaveY = new int[300];
        public static int numDungeons;
        public static int ficount;

        public static Tile cloneTile(Tile tile, int X = 0, int Y = 0)
        {
            return new Tile
            {
                type = tile.type,
                liquid = tile.liquid,
                active = tile.active,
                frameNumber = tile.frameNumber,
                wallFrameX = tile.wallFrameX,
                wallFrameY = tile.wallFrameY,
                lava = tile.lava,
                lighted = tile.lighted,
                frameX = tile.frameX,
                frameY = tile.frameY,
                tileX = X,
                tileY = Y
            };
        }

        public static void SpawnNPC(int x, int y)
        {
            if (Main.wallHouse[(int)Main.tile[x, y].wall])
            {
                WorldGen.canSpawn = true;
            }
            if (WorldGen.canSpawn)
            {
                if (WorldGen.StartRoomCheck(x, y))
                {
                    if (WorldGen.RoomNeeds(WorldGen.spawnNPC))
                    {
                        WorldGen.ScoreRoom(-1);
                        if (WorldGen.hiScore > 0)
                        {
                            int num = -1;
                            for (int i = 0; i < 1000; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].homeless && Main.npc[i].type == WorldGen.spawnNPC)
                                {
                                    num = i;
                                    break;
                                }
                            }
                            if (num == -1)
                            {
                                int num2 = WorldGen.bestX;
                                int num3 = WorldGen.bestY;
                                bool flag = false;
                                if (!flag)
                                {
                                    flag = true;
                                    Rectangle value = new Rectangle(num2 * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, num3 * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                    for (int i = 0; i < 255; i++)
                                    {
                                        if (Main.player[i].active)
                                        {
                                            Rectangle rectangle = new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height);
                                            if (rectangle.Intersects(value))
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (!flag)
                                {
                                    for (int j = 1; j < 500; j++)
                                    {
                                        for (int k = 0; k < 2; k++)
                                        {
                                            if (k == 0)
                                            {
                                                num2 = WorldGen.bestX + j;
                                            }
                                            else
                                            {
                                                num2 = WorldGen.bestX - j;
                                            }
                                            if (num2 > 10 && num2 < Main.maxTilesX - 10)
                                            {
                                                int num4 = WorldGen.bestY - j;
                                                double num5 = (double)(WorldGen.bestY + j);
                                                if (num4 < 10)
                                                {
                                                    num4 = 10;
                                                }
                                                if (num5 > Main.worldSurface)
                                                {
                                                    num5 = Main.worldSurface;
                                                }
                                                int num6 = num4;
                                                while ((double)num6 < num5)
                                                {
                                                    num3 = num6;
                                                    if (Main.tile[num2, num3].active && Main.tileSolid[(int)Main.tile[num2, num3].type])
                                                    {
                                                        if (!Collision.SolidTiles(num2 - 1, num2 + 1, num3 - 3, num3 - 1))
                                                        {
                                                            flag = true;
                                                            Rectangle value = new Rectangle(num2 * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, num3 * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                            for (int i = 0; i < 255; i++)
                                                            {
                                                                if (Main.player[i].active)
                                                                {
                                                                    Rectangle rectangle = new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height);
                                                                    if (rectangle.Intersects(value))
                                                                    {
                                                                        flag = false;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    num6++;
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
                                int num7 = NPC.NewNPC(num2 * 16, num3 * 16, WorldGen.spawnNPC, 1);
                                Main.npc[num7].homeTileX = WorldGen.bestX;
                                Main.npc[num7].homeTileY = WorldGen.bestY;
                                if (num2 < WorldGen.bestX)
                                {
                                    Main.npc[num7].direction = 1;
                                }
                                else
                                {
                                    if (num2 > WorldGen.bestX)
                                    {
                                        Main.npc[num7].direction = -1;
                                    }
                                }
                                Main.npc[num7].netUpdate = true;
                                if (Main.netMode == 0)
                                {
                                    //Main.NewText(Main.npc[num7].name + " has arrived!", 50, 125, 255);
                                }
                                else
                                {
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(25, -1, -1, Main.npc[num7].name + " has arrived!", 255, 50f, 125f, 255f);
                                    }
                                }
                            }
                            else
                            {
                                WorldGen.spawnNPC = 0;
                                Main.npc[num].homeTileX = WorldGen.bestX;
                                Main.npc[num].homeTileY = WorldGen.bestY;
                                Main.npc[num].homeless = false;
                            }
                            WorldGen.spawnNPC = 0;
                        }
                    }
                }
            }
        }
        
        public static bool RoomNeeds(int npcType)
        {
            if (WorldGen.houseTile[15] && (WorldGen.houseTile[14] || WorldGen.houseTile[18]) && (WorldGen.houseTile[4] || WorldGen.houseTile[33] || WorldGen.houseTile[34] || WorldGen.houseTile[35] || WorldGen.houseTile[36] || WorldGen.houseTile[42] || WorldGen.houseTile[49]) && (WorldGen.houseTile[10] || WorldGen.houseTile[11] || WorldGen.houseTile[19]))
            {
                WorldGen.canSpawn = true;
            }
            else
            {
                WorldGen.canSpawn = false;
            }
            return WorldGen.canSpawn;
        }
        
        public static void QuickFindHome(int npc)
        {
            if (Main.npc[npc].homeTileX > 10 && Main.npc[npc].homeTileY > 10 && Main.npc[npc].homeTileX < Main.maxTilesX - 10 && Main.npc[npc].homeTileY < Main.maxTilesY)
            {
                WorldGen.canSpawn = false;
                WorldGen.StartRoomCheck(Main.npc[npc].homeTileX, Main.npc[npc].homeTileY - 1);
                if (!WorldGen.canSpawn)
                {
                    for (int i = Main.npc[npc].homeTileX - 1; i < Main.npc[npc].homeTileX + 2; i++)
                    {
                        for (int j = Main.npc[npc].homeTileY - 1; j < Main.npc[npc].homeTileY + 2; j++)
                        {
                            if (WorldGen.StartRoomCheck(i, j))
                            {
                                break;
                            }
                        }
                    }
                }
                if (!WorldGen.canSpawn)
                {
                    int num = 10;
                    for (int i = Main.npc[npc].homeTileX - num; i <= Main.npc[npc].homeTileX + num; i += 2)
                    {
                        for (int j = Main.npc[npc].homeTileY - num; j <= Main.npc[npc].homeTileY + num; j += 2)
                        {
                            if (WorldGen.StartRoomCheck(i, j))
                            {
                                break;
                            }
                        }
                    }
                }
                if (WorldGen.canSpawn)
                {
                    WorldGen.RoomNeeds(Main.npc[npc].type);
                    if (WorldGen.canSpawn)
                    {
                        WorldGen.ScoreRoom(npc);
                    }
                    if (WorldGen.canSpawn && WorldGen.hiScore > 0)
                    {
                        Main.npc[npc].homeTileX = WorldGen.bestX;
                        Main.npc[npc].homeTileY = WorldGen.bestY;
                        Main.npc[npc].homeless = false;
                        WorldGen.canSpawn = false;
                    }
                    else
                    {
                        Main.npc[npc].homeless = true;
                    }
                }
                else
                {
                    Main.npc[npc].homeless = true;
                }
            }
        }
        
        public static void ScoreRoom(int ignoreNPC = -1)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.npc[i].active && Main.npc[i].townNPC && ignoreNPC != i && !Main.npc[i].homeless)
                {
                    for (int j = 0; j < WorldGen.numRoomTiles; j++)
                    {
                        if (Main.npc[i].homeTileX == WorldGen.roomX[j] && Main.npc[i].homeTileY == WorldGen.roomY[j])
                        {
                            bool flag = false;
                            for (int k = 0; k < WorldGen.numRoomTiles; k++)
                            {
                                if (Main.npc[i].homeTileX == WorldGen.roomX[k] && Main.npc[i].homeTileY - 1 == WorldGen.roomY[k])
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                WorldGen.hiScore = -1;
                                return;
                            }
                        }
                    }
                }
            }
            WorldGen.hiScore = 0;
            int num = 0;
            int num2 = 0;
            int num3 = WorldGen.roomX1 - NPC.sWidth / 2 / 16 - 1 - 21;
            int num4 = WorldGen.roomX2 + NPC.sWidth / 2 / 16 + 1 + 21;
            int num5 = WorldGen.roomY1 - NPC.sHeight / 2 / 16 - 1 - 21;
            int num6 = WorldGen.roomY2 + NPC.sHeight / 2 / 16 + 1 + 21;
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 >= Main.maxTilesX)
            {
                num4 = Main.maxTilesX - 1;
            }
            if (num5 < 0)
            {
                num5 = 0;
            }
            if (num6 > Main.maxTilesX)
            {
                num6 = Main.maxTilesX;
            }
            for (int i = num3 + 1; i < num4; i++)
            {
                for (int j = num5 + 2; j < num6 + 2; j++)
                {
                    if (Main.tile[i, j].active)
                    {
                        if (Main.tile[i, j].type == 23 || Main.tile[i, j].type == 24 || Main.tile[i, j].type == 25 || Main.tile[i, j].type == 32)
                        {
                            Main.evilTiles++;
                        }
                        else
                        {
                            if (Main.tile[i, j].type == 27)
                            {
                                Main.evilTiles -= 5;
                            }
                        }
                    }
                }
            }
            if (num2 < 50)
            {
                num2 = 0;
            }
            int num7 = -num2;
            if (num7 <= -250)
            {
                WorldGen.hiScore = num7;
                return;
            }
            num3 = WorldGen.roomX1;
            num4 = WorldGen.roomX2;
            num5 = WorldGen.roomY1;
            num6 = WorldGen.roomY2;
            for (int i = num3 + 1; i < num4; i++)
            {
                for (int j = num5 + 2; j < num6 + 2; j++)
                {
                    if (Main.tile[i, j].active)
                    {
                        num = num7;
                        if (Main.tileSolid[(int)Main.tile[i, j].type] && !Main.tileSolidTop[(int)Main.tile[i, j].type])
                        {
                            if (!Collision.SolidTiles(i - 1, i + 1, j - 3, j - 1))
                            {
                                if (Main.tile[i - 1, j].active && Main.tileSolid[(int)Main.tile[i - 1, j].type] && Main.tile[i + 1, j].active && Main.tileSolid[(int)Main.tile[i + 1, j].type])
                                {
                                    for (int l = i - 2; l < i + 3; l++)
                                    {
                                        for (int m = j - 4; m < j; m++)
                                        {
                                            if (Main.tile[l, m].active)
                                            {
                                                if (l == i)
                                                {
                                                    num -= 15;
                                                }
                                                else
                                                {
                                                    if (Main.tile[l, m].type == 10 || Main.tile[l, m].type == 11)
                                                    {
                                                        num -= 20;
                                                    }
                                                    else
                                                    {
                                                        if (Main.tileSolid[(int)Main.tile[l, m].type])
                                                        {
                                                            num -= 5;
                                                        }
                                                        else
                                                        {
                                                            num += 5;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (num > WorldGen.hiScore)
                                    {
                                        bool flag2 = false;
                                        for (int n = 0; n < WorldGen.numRoomTiles; n++)
                                        {
                                            if (WorldGen.roomX[n] == i && WorldGen.roomY[n] == j)
                                            {
                                                flag2 = true;
                                                break;
                                            }
                                        }
                                        if (flag2)
                                        {
                                            WorldGen.hiScore = num;
                                            WorldGen.bestX = i;
                                            WorldGen.bestY = j;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static bool StartRoomCheck(int x, int y)
        {
            WorldGen.roomX1 = x;
            WorldGen.roomX2 = x;
            WorldGen.roomY1 = y;
            WorldGen.roomY2 = y;
            WorldGen.numRoomTiles = 0;
            for (int i = 0; i < 80; i++)
            {
                WorldGen.houseTile[i] = false;
            }
            WorldGen.canSpawn = true;
            if (Main.tile[x, y].active && Main.tileSolid[(int)Main.tile[x, y].type])
            {
                WorldGen.canSpawn = false;
            }
            WorldGen.CheckRoom(x, y);
            if (WorldGen.numRoomTiles < 60)
            {
                WorldGen.canSpawn = false;
            }
            return WorldGen.canSpawn;
        }
        
        public static void CheckRoom(int x, int y)
        {
            if (WorldGen.canSpawn)
            {
                if (x < 10 || y < 10 || x >= Main.maxTilesX - 10 || y >= WorldGen.lastMaxTilesY - 10)
                {
                    WorldGen.canSpawn = false;
                }
                else
                {
                    for (int i = 0; i < WorldGen.numRoomTiles; i++)
                    {
                        if (WorldGen.roomX[i] == x && WorldGen.roomY[i] == y)
                        {
                            return;
                        }
                    }
                    WorldGen.roomX[WorldGen.numRoomTiles] = x;
                    WorldGen.roomY[WorldGen.numRoomTiles] = y;
                    WorldGen.numRoomTiles++;
                    if (WorldGen.numRoomTiles >= WorldGen.maxRoomTiles)
                    {
                        WorldGen.canSpawn = false;
                    }
                    else
                    {
                        if (Main.tile[x, y].active)
                        {
                            WorldGen.houseTile[(int)Main.tile[x, y].type] = true;
                            if (Main.tileSolid[(int)Main.tile[x, y].type] || Main.tile[x, y].type == 11)
                            {
                                return;
                            }
                        }
                        if (x < WorldGen.roomX1)
                        {
                            WorldGen.roomX1 = x;
                        }
                        if (x > WorldGen.roomX2)
                        {
                            WorldGen.roomX2 = x;
                        }
                        if (y < WorldGen.roomY1)
                        {
                            WorldGen.roomY1 = y;
                        }
                        if (y > WorldGen.roomY2)
                        {
                            WorldGen.roomY2 = y;
                        }
                        bool flag = false;
                        bool flag2 = false;
                        for (int i = -2; i < 3; i++)
                        {
                            if (Main.wallHouse[(int)Main.tile[x + i, y].wall])
                            {
                                flag = true;
                            }
                            if (Main.tile[x + i, y].active && (Main.tileSolid[(int)Main.tile[x + i, y].type] || Main.tile[x + i, y].type == 11))
                            {
                                flag = true;
                            }
                            if (Main.wallHouse[(int)Main.tile[x, y + i].wall])
                            {
                                flag2 = true;
                            }
                            if (Main.tile[x, y + i].active && (Main.tileSolid[(int)Main.tile[x, y + i].type] || Main.tile[x, y + i].type == 11))
                            {
                                flag2 = true;
                            }
                        }
                        if (!flag || !flag2)
                        {
                            WorldGen.canSpawn = false;
                        }
                        else
                        {
                            for (int i = x - 1; i < x + 2; i++)
                            {
                                for (int j = y - 1; j < y + 2; j++)
                                {
                                    if (i != x || j != y)
                                    {
                                        if (WorldGen.canSpawn)
                                        {
                                            WorldGen.CheckRoom(i, j);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static void dropMeteor()
        {
            bool flag = true;
            int num = 0;
            if (Main.netMode != 1)
            {
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        flag = false;
                        break;
                    }
                }
                int num2 = 0;
                float num3 = (float)(Main.maxTilesX / 4200);
                int num4 = (int)(400f * num3);
                for (int j = 5; j < Main.maxTilesX - 5; j++)
                {
                    int k = 5;
                    while ((double)k < Main.worldSurface)
                    {
                        if (Main.tile[j, k].active)
                        {
                            if (Main.tile[j, k].type == 37)
                            {
                                num2++;
                                if (num2 > num4)
                                {
                                    return;
                                }
                            }
                        }
                        k++;
                    }
                }
                while (!flag)
                {
                    float num5 = (float)Main.maxTilesX * 0.08f;
                    int j = Main.rand.Next(50, Main.maxTilesX - 50);
                    while ((float)j > (float)Main.spawnTileX - num5 && (float)j < (float)Main.spawnTileX + num5)
                    {
                        j = Main.rand.Next(50, Main.maxTilesX - 50);
                    }
                    for (int k = Main.rand.Next(100); k < Main.maxTilesY; k++)
                    {
                        if (Main.tile[j, k].active)
                        {
                            if (Main.tileSolid[(int)Main.tile[j, k].type])
                            {
                                flag = WorldGen.meteor(j, k);
                                break;
                            }
                        }
                    }
                    num++;
                    if (num >= 100)
                    {
                        break;
                    }
                }
            }
        }
        
        public static bool meteor(int i, int j)
        {
            bool result;
            if (i < 50 || i > Main.maxTilesX - 50)
            {
                result = false;
            }
            else
            {
                if (j < 50 || j > Main.maxTilesY - 50)
                {
                    result = false;
                }
                else
                {
                    int num = 25;
                    Rectangle rectangle = new Rectangle((i - num) * 16, (j - num) * 16, num * 2 * 16, num * 2 * 16);
                    for (int k = 0; k < 255; k++)
                    {
                        if (Main.player[k].active)
                        {
                            Rectangle value = new Rectangle((int)(Main.player[k].position.X + (float)(Main.player[k].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                            if (rectangle.Intersects(value))
                            {
                                result = false;
                                return result;
                            }
                        }
                    }
                    for (int k = 0; k < 1000; k++)
                    {
                        if (Main.npc[k].active)
                        {
                            Rectangle value2 = new Rectangle((int)Main.npc[k].position.X, (int)Main.npc[k].position.Y, Main.npc[k].width, Main.npc[k].height);
                            if (rectangle.Intersects(value2))
                            {
                                result = false;
                                return result;
                            }
                        }
                    }
                    for (int l = i - num; l < i + num; l++)
                    {
                        for (int m = j - num; m < j + num; m++)
                        {
                            if (Main.tile[l, m].active)
                            {
                                if (Main.tile[l, m].type == 21)
                                {
                                    result = false;
                                    return result;
                                }
                            }
                        }
                    }
                    WorldGen.stopDrops = true;
                    num = 15;
                    for (int l = i - num; l < i + num; l++)
                    {
                        for (int m = j - num; m < j + num; m++)
                        {
                            if (m > j + Main.rand.Next(-2, 3) - 5)
                            {
                                if ((double)(Math.Abs(i - l) + Math.Abs(j - m)) < (double)num * 1.5 + (double)Main.rand.Next(-5, 5))
                                {
                                    if (!Main.tileSolid[(int)Main.tile[l, m].type])
                                    {
                                        Main.tile[l, m].active = false;
                                    }
                                    Main.tile[l, m].type = 37;
                                }
                            }
                        }
                    }
                    num = 10;
                    for (int l = i - num; l < i + num; l++)
                    {
                        for (int m = j - num; m < j + num; m++)
                        {
                            if (m > j + Main.rand.Next(-2, 3) - 5)
                            {
                                if (Math.Abs(i - l) + Math.Abs(j - m) < num + Main.rand.Next(-3, 4))
                                {
                                    Main.tile[l, m].active = false;
                                }
                            }
                        }
                    }
                    num = 16;
                    for (int l = i - num; l < i + num; l++)
                    {
                        for (int m = j - num; m < j + num; m++)
                        {
                            if (Main.tile[l, m].type == 5 || Main.tile[l, m].type == 32)
                            {
                                WorldGen.KillTile(l, m, false, false, false);
                            }
                            WorldGen.SquareTileFrame(l, m, true);
                            WorldGen.SquareWallFrame(l, m, true);
                        }
                    }
                    num = 23;
                    for (int l = i - num; l < i + num; l++)
                    {
                        for (int m = j - num; m < j + num; m++)
                        {
                            if (Main.tile[l, m].active && Main.rand.Next(10) == 0)
                            {
                                if ((double)(Math.Abs(i - l) + Math.Abs(j - m)) < (double)num * 1.3)
                                {
                                    if (Main.tile[l, m].type == 5 || Main.tile[l, m].type == 32)
                                    {
                                        WorldGen.KillTile(l, m, false, false, false);
                                    }
                                    Main.tile[l, m].type = 37;
                                    WorldGen.SquareTileFrame(l, m, true);
                                }
                            }
                        }
                    }
                    WorldGen.stopDrops = false;
                    if (Main.netMode == 0)
                    {
                        //Main.NewText("A meteorite has landed!", 50, 255, 130);
                    }
                    else
                    {
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(25, -1, -1, "A meteorite has landed!", 255, 50f, 255f, 130f);
                        }
                    }
                    if (Main.netMode != 1)
                    {
                        NetMessage.SendTileSquare(-1, i, j, 30);
                    }
                    result = true;
                }
            }
            return result;
        }
        
        public static void setWorldSize()
        {
            Main.bottomWorld = (float)(Main.maxTilesY * 16);
            Main.rightWorld = (float)(Main.maxTilesX * 16);
            Main.maxSectionsX = Main.maxTilesX / 200;
            Main.maxSectionsY = Main.maxTilesY / 150;
        }
                
        public static void SaveAndQuitCallBack(object threadContext)
        {
            Main.menuMode = 10;
            Main.gameMenu = true;
            Player.SavePlayer(Main.player[Main.myPlayer]);
            if (Main.netMode == 0)
            {
                WorldGen.saveWorld(Program.server.getWorld().getSavePath(), false);
                //Main.PlaySound(10, -1, -1, 1);
            }
            else
            {
                NetPlay.disconnect = true;
                Main.netMode = 0;
            }
            Main.menuMode = 0;
        }
        
        public static void SaveAndQuit()
        {
            //Main.PlaySound(11, -1, -1, 1);
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.SaveAndQuitCallBack), 1);
        }
        
        public static void playWorldCallBack(object threadContext)
        {
            if (Main.rand == null)
            {
                Main.rand = new Random((int)DateTime.Now.Ticks);
            }
            for (int i = 0; i < 255; i++)
            {
                if (i != Main.myPlayer)
                {
                    Main.player[i].active = false;
                }
            }
            WorldGen.loadWorld();
            if (WorldGen.loadFailed || !WorldGen.loadSuccess)
            {
                WorldGen.loadWorld();
                if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                {
                    if (File.Exists(Program.server.getWorld().getSavePath() + ".bak"))
                    {
                        WorldGen.worldBackup = true;
                    }
                    else
                    {
                        WorldGen.worldBackup = false;
                    }
                    if (!Main.dedServ)
                    {
                        if (WorldGen.worldBackup)
                        {
                            Main.menuMode = 200;
                        }
                        else
                        {
                            Main.menuMode = 201;
                        }
                        return;
                    }
                    if (!WorldGen.worldBackup)
                    {
                        Console.WriteLine("Load failed!  No backup found.");
                        return;
                    }
                    File.Copy(Program.server.getWorld().getSavePath() + ".bak", Program.server.getWorld().getSavePath(), true);
                    File.Delete(Program.server.getWorld().getSavePath() + ".bak");
                    WorldGen.loadWorld();
                    if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                    {
                        WorldGen.loadWorld();
                        if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                        {
                            Console.WriteLine("Load failed!");
                            return;
                        }
                    }
                }
            }
            WorldGen.EveryTileFrame();
            if (Main.gameMenu)
            {
                Main.gameMenu = false;
            }
            Main.player[Main.myPlayer].Spawn();
            Main.player[Main.myPlayer].UpdatePlayer(Main.myPlayer);
            Main.dayTime = WorldGen.tempDayTime;
            Main.time = WorldGen.tempTime;
            Main.moonPhase = WorldGen.tempMoonPhase;
            Main.bloodMoon = WorldGen.tempBloodMoon;
            //Main.PlaySound(11, -1, -1, 1);
            Main.resetClouds = true;
        }
        
        public static void playWorld()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.playWorldCallBack), 1);
        }
        
        public static void saveAndPlayCallBack(object threadContext)
        {
            WorldGen.saveWorld(Program.server.getWorld().getSavePath(), false);
        }
        
        public static void saveAndPlay()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveAndPlayCallBack), 1);
        }
        
        public static void saveToonWhilePlayingCallBack(object threadContext)
        {
            Player.SavePlayer(Main.player[Main.myPlayer]);
        }
        
        public static void saveToonWhilePlaying()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveToonWhilePlayingCallBack), 1);
        }
        
        public static void serverLoadWorldCallBack(object threadContext)
        {
            WorldGen.loadWorld();
            if (WorldGen.loadFailed || !WorldGen.loadSuccess)
            {
                WorldGen.loadWorld();
                if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                {
                    if (File.Exists(Program.server.getWorld().getSavePath() + ".bak"))
                    {
                        WorldGen.worldBackup = true;
                    }
                    else
                    {
                        WorldGen.worldBackup = false;
                    }
                    if (!Main.dedServ)
                    {
                        if (WorldGen.worldBackup)
                        {
                            Main.menuMode = 200;
                        }
                        else
                        {
                            Main.menuMode = 201;
                        }
                        return;
                    }
                    if (!WorldGen.worldBackup)
                    {
                        Console.WriteLine("Load failed!  No backup found.");
                        return;
                    }
                    File.Copy(Program.server.getWorld().getSavePath() + ".bak", Program.server.getWorld().getSavePath(), true);
                    File.Delete(Program.server.getWorld().getSavePath() + ".bak");
                    WorldGen.loadWorld();
                    if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                    {
                        WorldGen.loadWorld();
                        if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                        {
                            Console.WriteLine("Load failed!");
                            return;
                        }
                    }
                }
            }
            //Main.PlaySound(10, -1, -1, 1);
            NetPlay.StartServer();
            Main.dayTime = WorldGen.tempDayTime;
            Main.time = WorldGen.tempTime;
            Main.moonPhase = WorldGen.tempMoonPhase;
            Main.bloodMoon = WorldGen.tempBloodMoon;
        }
        
        public static void serverLoadWorld()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.serverLoadWorldCallBack), 1);
        }
        
        public static void clearWorld()
        {
            WorldGen.spawnEye = false;
            WorldGen.spawnNPC = 0;
            WorldGen.shadowOrbCount = 0;
            Main.helpText = 0;
            Main.dungeonX = 0;
            Main.dungeonY = 0;
            NPC.downedBoss1 = false;
            NPC.downedBoss2 = false;
            NPC.downedBoss3 = false;
            WorldGen.shadowOrbSmashed = false;
            WorldGen.spawnMeteor = false;
            WorldGen.stopDrops = false;
            Main.invasionDelay = 0;
            Main.invasionType = 0;
            Main.invasionSize = 0;
            Main.invasionWarn = 0;
            Main.invasionX = 0.0;
            WorldGen.noLiquidCheck = false;
            Liquid.numLiquid = 0;
            LiquidBuffer.numLiquidBuffer = 0;

            WorldGen.lastMaxTilesX = Main.maxTilesX;
            WorldGen.lastMaxTilesY = Main.maxTilesY;
            if (Main.netMode != 1)
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    float num = (float)i / (float)Main.maxTilesX;
                    Program.printData("Resetting game objects: " + (int)(num * 100f + 1f) + "%");
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        Main.tile[i, j] = new Tile();
                    }
                }
            }
            for (int i = 0; i < 2000; i++)
            {
                Main.dust[i] = new Dust();
            }
            for (int i = 0; i < 200; i++)
            {
                Main.gore[i] = new Gore();
            }
            for (int i = 0; i < 200; i++)
            {
                Main.item[i] = new Item();
            }
            for (int i = 0; i < 1000; i++)
            {
                Main.npc[i] = new NPC();
            }
            for (int i = 0; i < 1000; i++)
            {
                Main.projectile[i] = new Projectile();
            }
            for (int i = 0; i < 1000; i++)
            {
                Main.chest[i] = null;
            }
            for (int i = 0; i < 1000; i++)
            {
                Main.sign[i] = null;
            }
            for (int i = 0; i < Liquid.resLiquid; i++)
            {
                Main.liquid[i] = new Liquid();
            }
            for (int i = 0; i < 10000; i++)
            {
                Main.liquidBuffer[i] = new LiquidBuffer();
            }
            WorldGen.setWorldSize();
            WorldGen.worldCleared = true;

            Console.WriteLine();
        }
        
        public static void saveWorld(string savePath, bool resetTime = false)
        {
            if (!WorldGen.saveLock)
            {
                WorldGen.saveLock = true;

                if (!Main.skipMenu)
                {
                    bool value = Main.dayTime;
                    WorldGen.tempTime = Main.time;
                    WorldGen.tempMoonPhase = Main.moonPhase;
                    WorldGen.tempBloodMoon = Main.bloodMoon;
                    if (resetTime)
                    {
                        value = true;
                        WorldGen.tempTime = 13500.0;
                        WorldGen.tempMoonPhase = 0;
                        WorldGen.tempBloodMoon = false;
                    }
                    if (savePath != null)
                    {
                        string text = savePath + ".sav";
                        using (FileStream fileStream = new FileStream(text, FileMode.Create))
                        {
                            using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                            {
                                binaryWriter.Write(Statics.currentRelease);
                                binaryWriter.Write(Main.worldName);
                                binaryWriter.Write(Main.worldID);
                                binaryWriter.Write((int)Main.leftWorld);
                                binaryWriter.Write((int)Main.rightWorld);
                                binaryWriter.Write((int)Main.topWorld);
                                binaryWriter.Write((int)Main.bottomWorld);
                                binaryWriter.Write(Main.maxTilesY);
                                binaryWriter.Write(Main.maxTilesX);
                                binaryWriter.Write(Main.spawnTileX);
                                binaryWriter.Write(Main.spawnTileY);
                                binaryWriter.Write(Main.worldSurface);
                                binaryWriter.Write(Main.rockLayer);
                                binaryWriter.Write(WorldGen.tempTime);
                                binaryWriter.Write(value);
                                binaryWriter.Write(WorldGen.tempMoonPhase);
                                binaryWriter.Write(WorldGen.tempBloodMoon);
                                binaryWriter.Write(Main.dungeonX);
                                binaryWriter.Write(Main.dungeonY);
                                binaryWriter.Write(NPC.downedBoss1);
                                binaryWriter.Write(NPC.downedBoss2);
                                binaryWriter.Write(NPC.downedBoss3);
                                binaryWriter.Write(WorldGen.shadowOrbSmashed);
                                binaryWriter.Write(WorldGen.spawnMeteor);
                                binaryWriter.Write((byte)WorldGen.shadowOrbCount);
                                binaryWriter.Write(Main.invasionDelay);
                                binaryWriter.Write(Main.invasionSize);
                                binaryWriter.Write(Main.invasionType);
                                binaryWriter.Write(Main.invasionX);
                                for (int i = 0; i < Main.maxTilesX; i++)
                                {
                                    float num = (float)i / (float)Main.maxTilesX;
                                    Program.printData("Saving world data: " + (int)(num * 100f + 1f) + "%");
                                    for (int j = 0; j < Main.maxTilesY; j++)
                                    {
                                        lock (Main.tile[i, j])
                                        {
                                            binaryWriter.Write(Main.tile[i, j].active);
                                            if (Main.tile[i, j].active)
                                            {
                                                binaryWriter.Write(Main.tile[i, j].type);
                                                if (Main.tileFrameImportant[(int)Main.tile[i, j].type])
                                                {
                                                    binaryWriter.Write(Main.tile[i, j].frameX);
                                                    binaryWriter.Write(Main.tile[i, j].frameY);
                                                }
                                            }
                                            binaryWriter.Write(Main.tile[i, j].lighted);
                                            if (Main.tile[i, j].wall > 0)
                                            {
                                                binaryWriter.Write(true);
                                                binaryWriter.Write(Main.tile[i, j].wall);
                                            }
                                            else
                                            {
                                                binaryWriter.Write(false);
                                            }
                                            if (Main.tile[i, j].liquid > 0)
                                            {
                                                binaryWriter.Write(true);
                                                binaryWriter.Write(Main.tile[i, j].liquid);
                                                binaryWriter.Write(Main.tile[i, j].lava);
                                            }
                                            else
                                            {
                                                binaryWriter.Write(false);
                                            }
                                        }
                                    }
                                }
                                Console.WriteLine();
                                for (int i = 0; i < 1000; i++)
                                {
                                    if (Main.chest[i] == null)
                                    {
                                        binaryWriter.Write(false);
                                    }
                                    else
                                    {
                                        lock (Main.chest[i])
                                        {
                                            binaryWriter.Write(true);
                                            binaryWriter.Write(Main.chest[i].x);
                                            binaryWriter.Write(Main.chest[i].y);
                                            for (int j = 0; j < Chest.maxItems; j++)
                                            {
                                                binaryWriter.Write((byte)Main.chest[i].item[j].stack);
                                                if (Main.chest[i].item[j].stack > 0)
                                                {
                                                    binaryWriter.Write(Main.chest[i].item[j].name);
                                                }
                                            }
                                        }
                                    }
                                }
                                for (int i = 0; i < 1000; i++)
                                {
                                    if (Main.sign[i] == null || Main.sign[i].text == null)
                                    {
                                        binaryWriter.Write(false);
                                    }
                                    else
                                    {
                                        lock (Main.sign[i])
                                        {
                                            binaryWriter.Write(true);
                                            binaryWriter.Write(Main.sign[i].text);
                                            binaryWriter.Write(Main.sign[i].x);
                                            binaryWriter.Write(Main.sign[i].y);
                                        }
                                    }
                                }
                                for (int i = 0; i < 1000; i++)
                                {
                                    lock (Main.npc[i])
                                    {
                                        if (Main.npc[i].active && Main.npc[i].townNPC)
                                        {
                                            binaryWriter.Write(true);
                                            binaryWriter.Write(Main.npc[i].name);
                                            binaryWriter.Write(Main.npc[i].position.X);
                                            binaryWriter.Write(Main.npc[i].position.Y);
                                            binaryWriter.Write(Main.npc[i].homeless);
                                            binaryWriter.Write(Main.npc[i].homeTileX);
                                            binaryWriter.Write(Main.npc[i].homeTileY);
                                        }
                                    }
                                }
                                binaryWriter.Write(false);
                                binaryWriter.Write(true);
                                binaryWriter.Write(Main.worldName);
                                binaryWriter.Write(Main.worldID);
                                binaryWriter.Close();
                                fileStream.Close();
                                if (File.Exists(savePath))
                                {
                                    Program.printData("Backing up world file...");
                                    string destFileName = savePath + ".bak";
                                    File.Copy(savePath, destFileName, true);
                                }
                                Console.WriteLine();
                                File.Copy(text, savePath, true);
                                File.Delete(text);
                            }
                        }
                        WorldGen.saveLock = false;
                    }
                }
            }
        }
        
        public static void loadWorld()
        {
            if (!File.Exists(Program.server.getWorld().getSavePath()))
            {
                if (Main.autoGen)
                {
                    for (int i = Program.server.getWorld().getSavePath().Length - 1; i >= 0; i--)
                    {
                        if (Program.server.getWorld().getSavePath().Substring(i, 1) == "\\")
                        {
                            string path = Program.server.getWorld().getSavePath().Substring(0, i);
                            Directory.CreateDirectory(path);
                            break;
                        }
                    }
                    WorldGen.clearWorld();
                    WorldGen.generateWorld(-1);
                    WorldGen.saveWorld(Program.server.getWorld().getSavePath(), false);
                }
            }
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
            }
            using (FileStream fileStream = new FileStream(Program.server.getWorld().getSavePath(), FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    try
                    {
                        WorldGen.loadFailed = false;
                        WorldGen.loadSuccess = false;
                        int num = binaryReader.ReadInt32();
                        if (num > Statics.currentRelease)
                        {
                            Console.WriteLine("Incompatible World File!");
                            WorldGen.loadFailed = true;
                            WorldGen.loadSuccess = false;
                            try
                            {
                                binaryReader.Close();
                                fileStream.Close();
                            }
                            catch
                            {
                            }
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Compatible World File");
                            Main.worldName = binaryReader.ReadString();
                            Main.worldID = binaryReader.ReadInt32();
                            Main.leftWorld = (float)binaryReader.ReadInt32();
                            Main.rightWorld = (float)binaryReader.ReadInt32();
                            Main.topWorld = (float)binaryReader.ReadInt32();
                            Main.bottomWorld = (float)binaryReader.ReadInt32();
                            Main.maxTilesY = binaryReader.ReadInt32();
                            Main.maxTilesX = binaryReader.ReadInt32();

                            WorldGen.clearWorld();

                            Main.spawnTileX = binaryReader.ReadInt32();
                            Main.spawnTileY = binaryReader.ReadInt32();
                            Main.worldSurface = binaryReader.ReadDouble();
                            Main.rockLayer = binaryReader.ReadDouble();
                            WorldGen.tempTime = binaryReader.ReadDouble();
                            WorldGen.tempDayTime = binaryReader.ReadBoolean();
                            WorldGen.tempMoonPhase = binaryReader.ReadInt32();
                            WorldGen.tempBloodMoon = binaryReader.ReadBoolean();
                            Main.dungeonX = binaryReader.ReadInt32();
                            Main.dungeonY = binaryReader.ReadInt32();
                            NPC.downedBoss1 = binaryReader.ReadBoolean();
                            NPC.downedBoss2 = binaryReader.ReadBoolean();
                            NPC.downedBoss3 = binaryReader.ReadBoolean();
                            WorldGen.shadowOrbSmashed = binaryReader.ReadBoolean();
                            WorldGen.spawnMeteor = binaryReader.ReadBoolean();
                            WorldGen.shadowOrbCount = (int)binaryReader.ReadByte();
                            Main.invasionDelay = binaryReader.ReadInt32();
                            Main.invasionSize = binaryReader.ReadInt32();
                            Main.invasionType = binaryReader.ReadInt32();
                            Main.invasionX = binaryReader.ReadDouble();

                            for (int i = 0; i < Main.maxTilesX; i++)
                            {
                                float num2 = (float)i / (float)Main.maxTilesX;
                                Program.printData("Loading world data: " + (int)(num2 * 100f + 1f) + "%");
                                for (int j = 0; j < Main.maxTilesY; j++)
                                {
                                    Main.tile[i, j].active = binaryReader.ReadBoolean();
                                    if (Main.tile[i, j].active)
                                    {
                                        Main.tile[i, j].type = binaryReader.ReadByte();
                                        if (Main.tileFrameImportant[(int)Main.tile[i, j].type])
                                        {
                                            Main.tile[i, j].frameX = binaryReader.ReadInt16();
                                            Main.tile[i, j].frameY = binaryReader.ReadInt16();
                                        }
                                        else
                                        {
                                            Main.tile[i, j].frameX = -1;
                                            Main.tile[i, j].frameY = -1;
                                        }
                                    }
                                    Main.tile[i, j].lighted = binaryReader.ReadBoolean();
                                    if (binaryReader.ReadBoolean())
                                    {
                                        Main.tile[i, j].wall = binaryReader.ReadByte();
                                    }
                                    if (binaryReader.ReadBoolean())
                                    {
                                        Main.tile[i, j].liquid = binaryReader.ReadByte();
                                        Main.tile[i, j].lava = binaryReader.ReadBoolean();
                                    }
                                }
                            }
                            for (int i = 0; i < 1000; i++)
                            {
                                if (binaryReader.ReadBoolean())
                                {
                                    Main.chest[i] = new Chest();
                                    Main.chest[i].x = binaryReader.ReadInt32();
                                    Main.chest[i].y = binaryReader.ReadInt32();
                                    for (int j = 0; j < Chest.maxItems; j++)
                                    {
                                        Main.chest[i].item[j] = new Item();
                                        byte b = binaryReader.ReadByte();
                                        if (b > 0)
                                        {
                                            string defaults = Item.VersionName(binaryReader.ReadString(), num);
                                            Main.chest[i].item[j].SetDefaults(defaults);
                                            Main.chest[i].item[j].stack = (int)b;
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < 1000; i++)
                            {
                                if (binaryReader.ReadBoolean())
                                {
                                    string text = binaryReader.ReadString();
                                    int num3 = binaryReader.ReadInt32();
                                    int num4 = binaryReader.ReadInt32();
                                    if (Main.tile[num3, num4].active && Main.tile[num3, num4].type == 55)
                                    {
                                        Main.sign[i] = new Sign();
                                        Main.sign[i].x = num3;
                                        Main.sign[i].y = num4;
                                        Main.sign[i].text = text;
                                    }
                                }
                            }
                            bool flag = binaryReader.ReadBoolean();
                            int num5 = 0;
                            while (flag)
                            {
                                Main.npc[num5].SetDefaults(binaryReader.ReadString());
                                Main.npc[num5].position.X = binaryReader.ReadSingle();
                                Main.npc[num5].position.Y = binaryReader.ReadSingle();
                                Main.npc[num5].homeless = binaryReader.ReadBoolean();
                                Main.npc[num5].homeTileX = binaryReader.ReadInt32();
                                Main.npc[num5].homeTileY = binaryReader.ReadInt32();
                                flag = binaryReader.ReadBoolean();
                                num5++;
                            }
                            if (num >= 7)
                            {
                                bool flag2 = binaryReader.ReadBoolean();
                                string a = binaryReader.ReadString();
                                int num6 = binaryReader.ReadInt32();
                                if (!flag2 || !(a == Main.worldName) || num6 != Main.worldID)
                                {
                                    WorldGen.loadSuccess = false;
                                    WorldGen.loadFailed = true;
                                    binaryReader.Close();
                                    fileStream.Close();
                                    return;
                                }
                                WorldGen.loadSuccess = true;
                            }
                            else
                            {
                                WorldGen.loadSuccess = true;
                            }
                            binaryReader.Close();
                            fileStream.Close();
                            Console.WriteLine();
                            if (!WorldGen.loadFailed && WorldGen.loadSuccess)
                            {
                                WorldGen.gen = true;
                                WorldGen.waterLine = Main.maxTilesY;
                                Liquid.QuickWater(2, -1, -1);
                                WorldGen.WaterCheck();
                                int num7 = 0;
                                Liquid.quickSettle = true;
                                int num8 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                                float num9 = 0f;
                                while (Liquid.numLiquid > 0 && num7 < 100000)
                                {
                                    num7++;
                                    float num2 = (float)(num8 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num8;
                                    if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num8)
                                    {
                                        num8 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                                    }
                                    if (num2 > num9)
                                    {
                                        num9 = num2;
                                    }
                                    else
                                    {
                                        num2 = num9;
                                    }
                                    Program.printData("Settling liquids: " + (int)(num2 * 100f / 2f + 50f) + "%");
                                    Liquid.UpdateLiquid();
                                }
                                Liquid.quickSettle = false;
                                WorldGen.WaterCheck();
                                WorldGen.gen = false;
                                Console.WriteLine();
                            }
                        }
                    }
                    catch
                    {
                        WorldGen.loadFailed = true;
                        WorldGen.loadSuccess = false;
                        try
                        {
                            binaryReader.Close();
                            fileStream.Close();
                        }
                        catch
                        {
                        }
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
        
        public static void generateWorld(int seed = -1)
        {
            WorldGen.gen = true;
            WorldGen.resetGen();
            if (seed > 0)
            {
                WorldGen.genRand = new Random(seed);
            }
            else
            {
                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
            }
            Main.worldID = WorldGen.genRand.Next(2147483647);
            int num = 0;
            int num2 = 0;
            double num3 = (double)Main.maxTilesY * 0.3;
            num3 *= (double)WorldGen.genRand.Next(90, 110) * 0.005;
            double num4 = num3 + (double)Main.maxTilesY * 0.2;
            num4 *= (double)WorldGen.genRand.Next(90, 110) * 0.01;
            double num5 = num3;
            double num6 = num3;
            double num7 = num4;
            double num8 = num4;
            int num9 = 0;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num9 = -1;
            }
            else
            {
                num9 = 1;
            }
            int i;
            for (i = 0; i < Main.maxTilesX; i++)
            {
                float num10 = (float)i / (float)Main.maxTilesX;
                Program.printData("Generating world terrain: " + (int)(num10 * 100f + 1f) + "%");
                if (num3 < num5)
                {
                    num5 = num3;
                }
                if (num3 > num6)
                {
                    num6 = num3;
                }
                if (num4 < num7)
                {
                    num7 = num4;
                }
                if (num4 > num8)
                {
                    num8 = num4;
                }
                if (num2 <= 0)
                {
                    num = WorldGen.genRand.Next(0, 5);
                    num2 = WorldGen.genRand.Next(5, 40);
                    if (num == 0)
                    {
                        num2 *= (int)((double)WorldGen.genRand.Next(5, 30) * 0.2);
                    }
                }
                num2--;
                if (num == 0)
                {
                    while (WorldGen.genRand.Next(0, 7) == 0)
                    {
                        num3 += (double)WorldGen.genRand.Next(-1, 2);
                    }
                }
                else
                {
                    if (num == 1)
                    {
                        while (WorldGen.genRand.Next(0, 4) == 0)
                        {
                            num3 -= 1.0;
                        }
                        while (WorldGen.genRand.Next(0, 10) == 0)
                        {
                            num3 += 1.0;
                        }
                    }
                    else
                    {
                        if (num == 2)
                        {
                            while (WorldGen.genRand.Next(0, 4) == 0)
                            {
                                num3 += 1.0;
                            }
                            while (WorldGen.genRand.Next(0, 10) == 0)
                            {
                                num3 -= 1.0;
                            }
                        }
                        else
                        {
                            if (num == 3)
                            {
                                while (WorldGen.genRand.Next(0, 2) == 0)
                                {
                                    num3 -= 1.0;
                                }
                                while (WorldGen.genRand.Next(0, 6) == 0)
                                {
                                    num3 += 1.0;
                                }
                            }
                            else
                            {
                                if (num == 4)
                                {
                                    while (WorldGen.genRand.Next(0, 2) == 0)
                                    {
                                        num3 += 1.0;
                                    }
                                    while (WorldGen.genRand.Next(0, 5) == 0)
                                    {
                                        num3 -= 1.0;
                                    }
                                }
                            }
                        }
                    }
                }
                if (num3 < (double)Main.maxTilesY * 0.15)
                {
                    num3 = (double)Main.maxTilesY * 0.15;
                    num2 = 0;
                }
                else
                {
                    if (num3 > (double)Main.maxTilesY * 0.3)
                    {
                        num3 = (double)Main.maxTilesY * 0.3;
                        num2 = 0;
                    }
                }
                while (WorldGen.genRand.Next(0, 3) == 0)
                {
                    num4 += (double)WorldGen.genRand.Next(-2, 3);
                }
                if (num4 < num3 + (double)Main.maxTilesY * 0.05)
                {
                    num4 += 1.0;
                }
                if (num4 > num3 + (double)Main.maxTilesY * 0.35)
                {
                    num4 -= 1.0;
                }
                int j = 0;
                while ((double)j < num3)
                {
                    Main.tile[i, j].active = false;
                    Main.tile[i, j].lighted = true;
                    Main.tile[i, j].frameX = -1;
                    Main.tile[i, j].frameY = -1;
                    j++;
                }
                for (j = (int)num3; j < Main.maxTilesY; j++)
                {
                    if ((double)j < num4)
                    {
                        Main.tile[i, j].active = true;
                        Main.tile[i, j].type = 0;
                        Main.tile[i, j].frameX = -1;
                        Main.tile[i, j].frameY = -1;
                    }
                    else
                    {
                        Main.tile[i, j].active = true;
                        Main.tile[i, j].type = 1;
                        Main.tile[i, j].frameX = -1;
                        Main.tile[i, j].frameY = -1;
                    }
                }
            }
            Main.worldSurface = num6 + 5.0;
            Main.rockLayer = num8;
            double num11 = (double)((int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6);
            Main.rockLayer = Main.worldSurface + num11;
            WorldGen.waterLine = (int)(Main.rockLayer + (double)Main.maxTilesY) / 2;
            WorldGen.waterLine += WorldGen.genRand.Next(-100, 20);
            WorldGen.lavaLine = WorldGen.waterLine + WorldGen.genRand.Next(50, 80);
            int num12 = 0;
            Console.WriteLine();
            Program.printData("Adding sand...");
            int num13 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.0007), (int)((double)Main.maxTilesX * 0.002));
            num13 += 2;
            int k;
            int l;
            for (k = 0; k < num13; k++)
            {
                int num14 = WorldGen.genRand.Next(Main.maxTilesX);
                while ((float)num14 > (float)Main.maxTilesX * 0.45f && (float)num14 < (float)Main.maxTilesX * 0.55f)
                {
                    num14 = WorldGen.genRand.Next(Main.maxTilesX);
                }
                int num15 = WorldGen.genRand.Next(15, 90);
                if (WorldGen.genRand.Next(3) == 0)
                {
                    num15 *= 2;
                }
                int num16 = num14 - num15;
                num15 = WorldGen.genRand.Next(15, 90);
                if (WorldGen.genRand.Next(3) == 0)
                {
                    num15 *= 2;
                }
                int num17 = num14 + num15;
                if (num16 < 0)
                {
                    num16 = 0;
                }
                if (num17 > Main.maxTilesX)
                {
                    num17 = Main.maxTilesX;
                }
                if (k == 0)
                {
                    num16 = 0;
                    num17 = WorldGen.genRand.Next(250, 300);
                }
                else
                {
                    if (k == 2)
                    {
                        num16 = Main.maxTilesX - WorldGen.genRand.Next(250, 300);
                        num17 = Main.maxTilesX;
                    }
                }
                int num18 = WorldGen.genRand.Next(50, 100);
                for (i = num16; i < num17; i++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num18 += WorldGen.genRand.Next(-1, 2);
                        if (num18 < 50)
                        {
                            num18 = 50;
                        }
                        if (num18 > 100)
                        {
                            num18 = 100;
                        }
                    }
                    l = 0;
                    while ((double)l < Main.worldSurface)
                    {
                        if (Main.tile[i, l].active)
                        {
                            int num19 = num18;
                            if (i - num16 < num19)
                            {
                                num19 = i - num16;
                            }
                            if (num17 - i < num19)
                            {
                                num19 = num17 - i;
                            }
                            num19 += WorldGen.genRand.Next(5);
                            for (int j = l; j < l + num19; j++)
                            {
                                if (i > num16 + WorldGen.genRand.Next(5) && i < num17 - WorldGen.genRand.Next(5))
                                {
                                    Main.tile[i, j].type = 53;
                                }
                            }
                            break;
                        }
                        l++;
                    }
                }
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-06); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), (double)WorldGen.genRand.Next(15, 70), WorldGen.genRand.Next(20, 130), 53, false, 0f, 0f, false, true);
            }
            WorldGen.numMCaves = 0;
            Console.WriteLine();
            Program.printData("Generating hills...");
            int m;
            for (k = 0; k < (int)((double)Main.maxTilesX * 0.0008); k++)
            {
                int num20 = 0;
                bool flag = false;
                bool flag2 = false;
                m = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
                while (!flag2)
                {
                    flag2 = true;
                    while (m > Main.maxTilesX / 2 - 100 && m < Main.maxTilesX / 2 + 100)
                    {
                        m = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
                    }
                    for (i = 0; i < WorldGen.numMCaves; i++)
                    {
                        if (m > WorldGen.mCaveX[i] - 50 && m < WorldGen.mCaveX[i] + 50)
                        {
                            num20++;
                            flag2 = false;
                            break;
                        }
                    }
                    if (num20 >= 200)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    l = 0;
                    while ((double)l < Main.worldSurface)
                    {
                        if (Main.tile[m, l].active)
                        {
                            WorldGen.Mountinater(m, l);
                            WorldGen.mCaveX[WorldGen.numMCaves] = m;
                            WorldGen.mCaveY[WorldGen.numMCaves] = l;
                            WorldGen.numMCaves++;
                            break;
                        }
                        l++;
                    }
                }
            }
            Console.WriteLine();
            for (i = 1; i < Main.maxTilesX - 1; i++)
            {
                float num10 = (float)i / (float)Main.maxTilesX;
                Program.printData("Putting dirt behind dirt: " + (int)(num10 * 100f + 1f) + "%");
                bool flag3 = false;
                num12 += WorldGen.genRand.Next(-1, 2);
                if (num12 < 0)
                {
                    num12 = 0;
                }
                if (num12 > 10)
                {
                    num12 = 10;
                }
                int j = 0;
                while ((double)j < Main.worldSurface + 10.0)
                {
                    if ((double)j > Main.worldSurface + (double)num12)
                    {
                        break;
                    }
                    if (flag3)
                    {
                        Main.tile[i, j].wall = 2;
                    }
                    if (Main.tile[i, j].active && Main.tile[i - 1, j].active && Main.tile[i + 1, j].active && Main.tile[i, j + 1].active && Main.tile[i - 1, j + 1].active && Main.tile[i + 1, j + 1].active)
                    {
                        flag3 = true;
                    }
                    j++;
                }
            }
            Console.WriteLine();
            Program.printData("Placing rocks in the dirt...");
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5 + 1), (double)WorldGen.genRand.Next(4, 15), WorldGen.genRand.Next(5, 40), 1, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6 + 1), (double)WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0045); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8 + 1), (double)WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(2, 23), 1, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            Program.printData("Placing dirt in the rocks...");
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.005); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 0, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            Program.printData("Adding clay...");
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5), (double)WorldGen.genRand.Next(4, 14), WorldGen.genRand.Next(10, 50), 40, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 5E-05); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6 + 1), (double)WorldGen.genRand.Next(8, 14), WorldGen.genRand.Next(15, 45), 40, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8 + 1), (double)WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(5, 50), 40, false, 0f, 0f, false, true);
            }
            for (i = 5; i < Main.maxTilesX - 5; i++)
            {
                int j = 1;
                while ((double)j < Main.worldSurface - 1.0)
                {
                    if (Main.tile[i, j].active)
                    {
                        for (l = j; l < j + 5; l++)
                        {
                            if (Main.tile[i, l].type == 40)
                            {
                                Main.tile[i, l].type = 0;
                            }
                        }
                        break;
                    }
                    j++;
                }
            }
            Console.WriteLine();
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015); k++)
            {
                float num10 = (float)((double)k / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015));
                Program.printData("Making random holes: " + (int)(num10 * 100f + 1f) + "%");
                int type = -1;
                if (WorldGen.genRand.Next(5) == 0)
                {
                    type = -2;
                }
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 20), type, false, 0f, 0f, false, true);
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, Main.maxTilesY), (double)WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(7, 30), type, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); k++)
            {
                float num10 = (float)((double)k / ((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05));
                Program.printData("Generating small caves: " + (int)(num10 * 100f + 1f) + "%");
                if (num8 <= (double)Main.maxTilesY)
                {
                    int type = -1;
                    if (WorldGen.genRand.Next(6) == 0)
                    {
                        type = -2;
                    }
                    WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num8 + 1), (double)WorldGen.genRand.Next(5, 15), WorldGen.genRand.Next(30, 200), type, false, 0f, 0f, false, true);
                }
            }
            Console.WriteLine();
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015); k++)
            {
                float num10 = (float)((double)k / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015));
                Program.printData("Generating large caves: " + (int)(num10 * 100f + 1f) + "%");
                if (num8 <= (double)Main.maxTilesY)
                {
                    int type = -1;
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        type = -2;
                    }
                    WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num8, Main.maxTilesY), (double)WorldGen.genRand.Next(6, 20), WorldGen.genRand.Next(50, 300), type, false, 0f, 0f, false, true);
                }
            }
            Console.WriteLine();
            Program.printData("Generating surface caves...");
            for (k = 0; k < (int)((double)Main.maxTilesX * 0.0025); k++)
            {
                int num21 = WorldGen.genRand.Next(0, Main.maxTilesX);
                int num22 = 0;
                while ((double)num22 < num6)
                {
                    if (Main.tile[num21, num22].active)
                    {
                        WorldGen.TileRunner(num21, num22, (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(5, 50), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 1f, false, true);
                        break;
                    }
                    num22++;
                }
            }
            for (k = 0; k < (int)((double)Main.maxTilesX * 0.0007); k++)
            {
                int num21 = WorldGen.genRand.Next(0, Main.maxTilesX);
                int num22 = 0;
                while ((double)num22 < num6)
                {
                    if (Main.tile[num21, num22].active)
                    {
                        WorldGen.TileRunner(num21, num22, (double)WorldGen.genRand.Next(10, 15), WorldGen.genRand.Next(50, 130), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        break;
                    }
                    num22++;
                }
            }
            for (k = 0; k < (int)((double)Main.maxTilesX * 0.0003); k++)
            {
                int num21 = WorldGen.genRand.Next(0, Main.maxTilesX);
                int num22 = 0;
                while ((double)num22 < num6)
                {
                    if (Main.tile[num21, num22].active)
                    {
                        WorldGen.TileRunner(num21, num22, (double)WorldGen.genRand.Next(12, 25), WorldGen.genRand.Next(150, 500), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 4f, false, true);
                        WorldGen.TileRunner(num21, num22, (double)WorldGen.genRand.Next(8, 17), WorldGen.genRand.Next(60, 200), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        WorldGen.TileRunner(num21, num22, (double)WorldGen.genRand.Next(5, 13), WorldGen.genRand.Next(40, 170), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        break;
                    }
                    num22++;
                }
            }
            for (k = 0; k < (int)((double)Main.maxTilesX * 0.0004); k++)
            {
                int num21 = WorldGen.genRand.Next(0, Main.maxTilesX);
                int num22 = 0;
                while ((double)num22 < num6)
                {
                    if (Main.tile[num21, num22].active)
                    {
                        WorldGen.TileRunner(num21, num22, (double)WorldGen.genRand.Next(7, 12), WorldGen.genRand.Next(150, 250), -1, false, 0f, 1f, true, true);
                        break;
                    }
                    num22++;
                }
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002); k++)
            {
                int num23 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
                int num24 = WorldGen.genRand.Next((int)num5, (int)num6);
                if (num24 >= Main.maxTilesY)
                {
                    num24 = Main.maxTilesY - 2;
                }
                if (Main.tile[num23 - 1, num24].active && Main.tile[num23 - 1, num24].type == 0 && Main.tile[num23 + 1, num24].active && Main.tile[num23 + 1, num24].type == 0 && Main.tile[num23, num24 - 1].active && Main.tile[num23, num24 - 1].type == 0 && Main.tile[num23, num24 + 1].active && Main.tile[num23, num24 + 1].type == 0)
                {
                    Main.tile[num23, num24].active = true;
                    Main.tile[num23, num24].type = 2;
                }
                num23 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
                num24 = WorldGen.genRand.Next(0, (int)num5);
                if (num24 >= Main.maxTilesY)
                {
                    num24 = Main.maxTilesY - 2;
                }
                if (Main.tile[num23 - 1, num24].active && Main.tile[num23 - 1, num24].type == 0 && Main.tile[num23 + 1, num24].active && Main.tile[num23 + 1, num24].type == 0 && Main.tile[num23, num24 - 1].active && Main.tile[num23, num24 - 1].type == 0 && Main.tile[num23, num24 + 1].active && Main.tile[num23, num24 + 1].type == 0)
                {
                    Main.tile[num23, num24].active = true;
                    Main.tile[num23, num24].type = 2;
                }
            }
            Console.WriteLine();
            Program.printData("Generating jungle: 0%");
            float num25 = (float)(Main.maxTilesX / 4200);
            num25 *= 1.5f;
            m = 0;
            float num26 = (float)WorldGen.genRand.Next(15, 30) * 0.01f;
            if (num9 == -1)
            {
                num26 = 1f - num26;
                m = (int)((float)Main.maxTilesX * num26);
            }
            else
            {
                m = (int)((float)Main.maxTilesX * num26);
            }
            l = (int)((double)Main.maxTilesY + Main.rockLayer) / 2;
            m += WorldGen.genRand.Next((int)(-100f * num25), (int)(101f * num25));
            l += WorldGen.genRand.Next((int)(-100f * num25), (int)(101f * num25));
            int num27 = m;
            int num28 = l;
            WorldGen.TileRunner(m, l, (double)WorldGen.genRand.Next((int)(250f * num25), (int)(500f * num25)), WorldGen.genRand.Next(50, 150), 59, false, (float)(num9 * 3), 0f, false, true);
            Program.printData("Generating jungle: 15%");
            m += WorldGen.genRand.Next((int)(-250f * num25), (int)(251f * num25));
            l += WorldGen.genRand.Next((int)(-150f * num25), (int)(151f * num25));
            int num29 = m;
            int num30 = l;
            int num31 = m;
            int num32 = l;
            WorldGen.TileRunner(m, l, (double)WorldGen.genRand.Next((int)(250f * num25), (int)(500f * num25)), WorldGen.genRand.Next(50, 150), 59, false, 0f, 0f, false, true);
            Program.printData("Generating jungle: 30%");
            m += WorldGen.genRand.Next((int)(-400f * num25), (int)(401f * num25));
            l += WorldGen.genRand.Next((int)(-150f * num25), (int)(151f * num25));
            int num33 = m;
            int num34 = l;
            WorldGen.TileRunner(m, l, (double)WorldGen.genRand.Next((int)(250f * num25), (int)(500f * num25)), WorldGen.genRand.Next(50, 150), 59, false, (float)(num9 * -3), 0f, false, true);
            Program.printData("Generating jungle: 45%");
            m = (num27 + num29 + num33) / 3;
            l = (num28 + num30 + num34) / 3;
            WorldGen.TileRunner(m, l, (double)WorldGen.genRand.Next((int)(250f * num25), (int)(500f * num25)), 10000, 59, false, 0f, -20f, true, true);
            Program.printData("Generating jungle: 60%");
            m = num31;
            l = num32;
            i = 0;
            while ((float)i <= 20f * num25)
            {
                Program.printData("Generating jungle: " + (int)(60f + (float)i / num25) + "%");
                m += WorldGen.genRand.Next((int)(-5f * num25), (int)(6f * num25));
                l += WorldGen.genRand.Next((int)(-5f * num25), (int)(6f * num25));
                WorldGen.TileRunner(m, l, (double)WorldGen.genRand.Next(40, 100), WorldGen.genRand.Next(300, 500), 59, false, 0f, 0f, false, true);
                i++;
            }
            k = 0;
            while ((float)k <= 10f * num25)
            {
                Program.printData("Generating jungle: " + (int)(80f + (float)k / num25 * 2f) + "%");
                m = num31 + WorldGen.genRand.Next((int)(-600f * num25), (int)(600f * num25));
                l = num32 + WorldGen.genRand.Next((int)(-200f * num25), (int)(200f * num25));
                while (m < 1 || m >= Main.maxTilesX - 1 || l < 1 || l >= Main.maxTilesY - 1 || Main.tile[m, l].type != 59)
                {
                    m = num31 + WorldGen.genRand.Next((int)(-600f * num25), (int)(600f * num25));
                    l = num32 + WorldGen.genRand.Next((int)(-200f * num25), (int)(200f * num25));
                }
                i = 0;
                while ((float)i < 8f * num25)
                {
                    m += WorldGen.genRand.Next(-30, 31);
                    l += WorldGen.genRand.Next(-30, 31);
                    int type = -1;
                    if (WorldGen.genRand.Next(7) == 0)
                    {
                        type = -2;
                    }
                    WorldGen.TileRunner(m, l, (double)WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(30, 70), type, false, 0f, 0f, false, true);
                    i++;
                }
                k++;
            }
            k = 0;
            while ((float)k <= 300f * num25)
            {
                m = num31 + WorldGen.genRand.Next((int)(-600f * num25), (int)(600f * num25));
                l = num32 + WorldGen.genRand.Next((int)(-200f * num25), (int)(200f * num25));
                while (m < 1 || m >= Main.maxTilesX - 1 || l < 1 || l >= Main.maxTilesY - 1 || Main.tile[m, l].type != 59)
                {
                    m = num31 + WorldGen.genRand.Next((int)(-600f * num25), (int)(600f * num25));
                    l = num32 + WorldGen.genRand.Next((int)(-200f * num25), (int)(200f * num25));
                }
                WorldGen.TileRunner(m, l, (double)WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
                if (WorldGen.genRand.Next(4) == 0)
                {
                    i = WorldGen.genRand.Next(63, 69);
                    WorldGen.TileRunner(m + WorldGen.genRand.Next(-1, 2), l + WorldGen.genRand.Next(-1, 2), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(4, 8), i, false, 0f, 0f, false, true);
                }
                k++;
            }
            m = num31;
            l = num32;
            float num35 = (float)WorldGen.genRand.Next(6, 10);
            float num36 = (float)(Main.maxTilesX / 4200);
            num35 *= num36;
            i = 0;
            while ((float)i < num35)
            {
                bool flag4 = true;
                while (flag4)
                {
                    m = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                    l = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 300);
                    if (Main.tile[m, l].type == 59)
                    {
                        flag4 = false;
                        int n = WorldGen.genRand.Next(2, 4);
                        int num37 = WorldGen.genRand.Next(2, 4);
                        for (int num38 = m - n - 1; num38 <= m + n + 1; num38++)
                        {
                            for (int num39 = l - num37 - 1; num39 <= l + num37 + 1; num39++)
                            {
                                Main.tile[num38, num39].active = true;
                                Main.tile[num38, num39].type = 45;
                                Main.tile[num38, num39].liquid = 0;
                                Main.tile[num38, num39].lava = false;
                            }
                        }
                        for (int num38 = m - n; num38 <= m + n; num38++)
                        {
                            for (int num39 = l - num37; num39 <= l + num37; num39++)
                            {
                                Main.tile[num38, num39].active = false;
                                Main.tile[num38, num39].wall = 10;
                            }
                        }
                        bool flag5 = false;
                        int num40 = 0;
                        while (!flag5 && num40 < 100)
                        {
                            num40++;
                            int num38 = WorldGen.genRand.Next(m - n, m + n + 1);
                            int num39 = WorldGen.genRand.Next(l - num37, l + num37 - 2);
                            WorldGen.PlaceTile(num38, num39, 4, true, false, -1);
                            if (Main.tile[num38, num39].type == 4)
                            {
                                flag5 = true;
                            }
                        }
                        for (int num38 = m - n - 1; num38 <= m + n + 1; num38++)
                        {
                            for (int num39 = l + num37 - 2; num39 <= l + num37; num39++)
                            {
                                Main.tile[num38, num39].active = false;
                            }
                        }
                        for (int num38 = m - n - 1; num38 <= m + n + 1; num38++)
                        {
                            for (int num39 = l + num37 - 2; num39 <= l + num37 - 1; num39++)
                            {
                                Main.tile[num38, num39].active = false;
                            }
                        }
                        for (int num38 = m - n - 1; num38 <= m + n + 1; num38++)
                        {
                            int num41 = 4;
                            int num39 = l + num37 + 2;
                            while (!Main.tile[num38, num39].active && num39 < Main.maxTilesY && num41 > 0)
                            {
                                Main.tile[num38, num39].active = true;
                                Main.tile[num38, num39].type = 59;
                                num39++;
                                num41--;
                            }
                        }
                        n -= WorldGen.genRand.Next(1, 3);
                        int num42 = l - num37 - 2;
                        while (n > -1)
                        {
                            for (int num38 = m - n - 1; num38 <= m + n + 1; num38++)
                            {
                                Main.tile[num38, num42].active = true;
                                Main.tile[num38, num42].type = 45;
                            }
                            n -= WorldGen.genRand.Next(1, 3);
                            num42--;
                        }
                        WorldGen.JChestX[WorldGen.numJChests] = m;
                        WorldGen.JChestY[WorldGen.numJChests] = l;
                        WorldGen.numJChests++;
                    }
                }
                i++;
            }
            for (i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j].active)
                    {
                        WorldGen.SpreadGrass(i, j, 59, 60, true);
                    }
                }
            }
            WorldGen.numIslandHouses = 0;
            WorldGen.houseCount = 0;
            Console.WriteLine();
            Program.printData("Generating floating islands...");
            for (k = 0; k < ficount; k++)
            {
                int num43 = 0;
                bool flag = false;
                m = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
                bool flag2 = false;
                while (!flag2)
                {
                    flag2 = true;
                    while (m > Main.maxTilesX / 2 - 80 && m < Main.maxTilesX / 2 + 80)
                    {
                        m = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
                    }
                    for (i = 0; i < WorldGen.numIslandHouses; i++)
                    {
                        if (m > WorldGen.fihX[i] - 80 && m < WorldGen.fihX[i] + 80)
                        {
                            num43++;
                            flag2 = false;
                            break;
                        }
                    }
                    if (num43 >= 200)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    l = 200;
                    while ((double)l < Main.worldSurface)
                    {
                        if (Main.tile[m, l].active)
                        {
                            i = m;
                            int j = WorldGen.genRand.Next(90, l - 100);
                            while ((double)j > num5 - 50.0)
                            {
                                j--;
                            }
                            WorldGen.FloatingIsland(i, j);
                            WorldGen.fihX[WorldGen.numIslandHouses] = i;
                            WorldGen.fihY[WorldGen.numIslandHouses] = j;
                            WorldGen.numIslandHouses++;
                            break;
                        }
                        l++;
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Adding mushroom patches...");
            for (i = 0; i < Main.maxTilesX / 300; i++)
            {
                m = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.3), (int)((double)Main.maxTilesX * 0.7));
                l = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);
                WorldGen.ShroomPatch(m, l);
            }
            for (i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = (int)Main.worldSurface; j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j].active)
                    {
                        WorldGen.SpreadGrass(i, j, 59, 70, false);
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Placing mud in the dirt...");
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 59, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            Program.printData("Adding shinies...");
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), 7, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), 7, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 7, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(2, 5), 6, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), 6, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 6, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), 9, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00017); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00017); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5 - 20), (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
            }
            Console.WriteLine();
            Program.printData("Adding webs...");
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001); k++)
            {
                int num38 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                int num39 = WorldGen.genRand.Next((int)num5, Main.maxTilesY - 20);
                if (k < WorldGen.numMCaves)
                {
                    num38 = WorldGen.mCaveX[k];
                    num39 = WorldGen.mCaveY[k];
                }
                if (!Main.tile[num38, num39].active)
                {
                    if ((double)num39 > Main.worldSurface || Main.tile[num38, num39].wall > 0)
                    {
                        while (!Main.tile[num38, num39].active && num39 > (int)num5)
                        {
                            num39--;
                        }
                        num39++;
                        int num44 = 1;
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            num44 = -1;
                        }
                        while (!Main.tile[num38, num39].active && num38 > 10 && num38 < Main.maxTilesX - 10)
                        {
                            num38 += num44;
                        }
                        num38 -= num44;
                        if ((double)num39 > Main.worldSurface || Main.tile[num38, num39].wall > 0)
                        {
                            WorldGen.TileRunner(num38, num39, (double)WorldGen.genRand.Next(4, 13), WorldGen.genRand.Next(2, 5), 51, true, (float)num44, -1f, false, false);
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Creating underworld: 0%");
            int num45 = Main.maxTilesY - WorldGen.genRand.Next(150, 190);
            for (m = 0; m < Main.maxTilesX; m++)
            {
                num45 += WorldGen.genRand.Next(-3, 4);
                if (num45 < Main.maxTilesY - 190)
                {
                    num45 = Main.maxTilesY - 190;
                }
                if (num45 > Main.maxTilesY - 160)
                {
                    num45 = Main.maxTilesY - 160;
                }
                for (l = num45 - 20 - WorldGen.genRand.Next(3); l < Main.maxTilesY; l++)
                {
                    if (l >= num45)
                    {
                        Main.tile[m, l].active = false;
                        Main.tile[m, l].lava = false;
                        Main.tile[m, l].liquid = 0;
                    }
                    else
                    {
                        Main.tile[m, l].type = 57;
                    }
                }
            }
            int num46 = Main.maxTilesY - WorldGen.genRand.Next(40, 70);
            for (i = 10; i < Main.maxTilesX - 10; i++)
            {
                num46 += WorldGen.genRand.Next(-10, 11);
                if (num46 > Main.maxTilesY - 60)
                {
                    num46 = Main.maxTilesY - 60;
                }
                if (num46 < Main.maxTilesY - 100)
                {
                    num46 = Main.maxTilesY - 120;
                }
                for (int j = num46; j < Main.maxTilesY - 10; j++)
                {
                    if (!Main.tile[i, j].active)
                    {
                        Main.tile[i, j].lava = true;
                        Main.tile[i, j].liquid = 255;
                    }
                }
            }
            for (i = 0; i < Main.maxTilesX; i++)
            {
                if (WorldGen.genRand.Next(50) == 0)
                {
                    l = Main.maxTilesY - 65;
                    while (!Main.tile[i, l].active && l > Main.maxTilesY - 135)
                    {
                        l--;
                    }
                    WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), l + WorldGen.genRand.Next(20, 50), (double)WorldGen.genRand.Next(15, 20), 1000, 57, true, 0f, (float)WorldGen.genRand.Next(1, 3), true, true);
                }
            }
            Liquid.QuickWater(-2, -1, -1);
            for (i = 0; i < Main.maxTilesX; i++)
            {
                float num10 = (float)i / (float)(Main.maxTilesX - 1);
                Program.printData("Creating underworld: " + (int)(num10 * 100f / 2f + 50f) + "%");
                if (WorldGen.genRand.Next(13) == 0)
                {
                    l = Main.maxTilesY - 65;
                    while ((Main.tile[i, l].liquid > 0 || Main.tile[i, l].active) && l > Main.maxTilesY - 140)
                    {
                        l--;
                    }
                    WorldGen.TileRunner(i, l - WorldGen.genRand.Next(2, 5), (double)WorldGen.genRand.Next(5, 30), 1000, 57, true, 0f, (float)WorldGen.genRand.Next(1, 3), true, true);
                    num25 = (float)WorldGen.genRand.Next(1, 3);
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        num25 *= 0.5f;
                    }
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        WorldGen.TileRunner(i, l - WorldGen.genRand.Next(2, 5), (double)((int)((float)WorldGen.genRand.Next(5, 15) * num25)), (int)((float)WorldGen.genRand.Next(10, 15) * num25), 57, true, 1f, 0.3f, false, true);
                    }
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num25 = (float)WorldGen.genRand.Next(1, 3);
                        WorldGen.TileRunner(i, l - WorldGen.genRand.Next(2, 5), (double)((int)((float)WorldGen.genRand.Next(5, 15) * num25)), (int)((float)WorldGen.genRand.Next(10, 15) * num25), 57, true, -1f, 0.3f, false, true);
                    }
                    WorldGen.TileRunner(i + WorldGen.genRand.Next(-10, 10), l + WorldGen.genRand.Next(-10, 10), (double)WorldGen.genRand.Next(5, 15), WorldGen.genRand.Next(5, 10), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        WorldGen.TileRunner(i + WorldGen.genRand.Next(-10, 10), l + WorldGen.genRand.Next(-10, 10), (double)WorldGen.genRand.Next(10, 30), WorldGen.genRand.Next(10, 20), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        WorldGen.TileRunner(i + WorldGen.genRand.Next(-15, 15), l + WorldGen.genRand.Next(-15, 10), (double)WorldGen.genRand.Next(15, 30), WorldGen.genRand.Next(5, 20), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
                    }
                }
            }
            for (i = 0; i < Main.maxTilesX; i++)
            {
                if (!Main.tile[i, Main.maxTilesY - 145].active)
                {
                    Main.tile[i, Main.maxTilesY - 145].liquid = 255;
                    Main.tile[i, Main.maxTilesY - 145].lava = true;
                }
                if (!Main.tile[i, Main.maxTilesY - 144].active)
                {
                    Main.tile[i, Main.maxTilesY - 144].liquid = 255;
                    Main.tile[i, Main.maxTilesY - 144].lava = true;
                }
            }
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(Main.maxTilesY - 140, Main.maxTilesY), (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), 58, false, 0f, 0f, false, true);
            }
            WorldGen.AddHellHouses();
            Console.WriteLine();
            int num47 = WorldGen.genRand.Next(2, (int)((double)Main.maxTilesX * 0.005));
            for (k = 0; k < num47; k++)
            {
                float num10 = (float)k / (float)num47;
                Program.printData("Adding water bodies: " + (int)(num10 * 100f) + "%");
                i = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                while (i > Main.maxTilesX / 2 - 50 && i < Main.maxTilesX / 2 + 50)
                {
                    i = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                }
                int j = (int)num5 - 20;
                while (!Main.tile[i, j].active)
                {
                    j++;
                }
                WorldGen.Lakinater(i, j);
            }
            m = 0;
            if (num9 == -1)
            {
                m = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.05), (int)((double)Main.maxTilesX * 0.2));
                num9 = -1;
            }
            else
            {
                m = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.8), (int)((double)Main.maxTilesX * 0.95));
                num9 = 1;
            }
            l = (int)((Main.rockLayer + (double)Main.maxTilesY) / 2.0) + WorldGen.genRand.Next(-200, 200);
            Console.WriteLine();
            for (int dnum = 0; dnum < numDungeons; dnum++)
                WorldGen.MakeDungeon(m, l, 41, 7);
            int num48 = 0;
            Console.WriteLine();
            while ((double)num48 < (double)Main.maxTilesX * 0.00045)
            {
                float num10 = (float)((double)num48 / ((double)Main.maxTilesX * 0.00045));
                Program.printData("Making the world evil: " + (int)(num10 * 100f) + "%");
                bool flag2 = false;
                int num49 = 0;
                int num50 = 0;
                int num51 = 0;
                while (!flag2)
                {
                    flag2 = true;
                    int num52 = Main.maxTilesX / 2;
                    int num53 = 200;
                    num49 = WorldGen.genRand.Next(Main.maxTilesX);
                    num50 = num49 - WorldGen.genRand.Next(150) - 175;
                    num51 = num49 + WorldGen.genRand.Next(150) + 175;
                    if (num50 < 0)
                    {
                        num50 = 0;
                    }
                    if (num51 > Main.maxTilesX)
                    {
                        num51 = Main.maxTilesX;
                    }
                    if (num49 > num52 - num53 && num49 < num52 + num53)
                    {
                        flag2 = false;
                    }
                    if (num50 > num52 - num53 && num50 < num52 + num53)
                    {
                        flag2 = false;
                    }
                    if (num51 > num52 - num53 && num51 < num52 + num53)
                    {
                        flag2 = false;
                    }
                    for (i = num50; i < num51; i++)
                    {
                        for (int j = 0; j < (int)Main.worldSurface; j += 5)
                        {
                            if (Main.tile[i, j].active && Main.tileDungeon[(int)Main.tile[i, j].type])
                            {
                                flag2 = false;
                                break;
                            }
                            if (!flag2)
                            {
                                break;
                            }
                        }
                    }
                }
                int num54 = 0;
                for (k = num50; k < num51; k++)
                {
                    if (num54 > 0)
                    {
                        num54--;
                    }
                    int num22;
                    if (k == num49 || num54 == 0)
                    {
                        num22 = (int)num5;
                        while ((double)num22 < Main.worldSurface - 1.0)
                        {
                            if (Main.tile[k, num22].active || Main.tile[k, num22].wall > 0)
                            {
                                if (k == num49)
                                {
                                    num54 = 20;
                                    WorldGen.ChasmRunner(k, num22, WorldGen.genRand.Next(150) + 150, true);
                                }
                                else
                                {
                                    if (WorldGen.genRand.Next(35) == 0)
                                    {
                                        if (num54 == 0)
                                        {
                                            num54 = 30;
                                            bool makeOrb = true;
                                            WorldGen.ChasmRunner(k, num22, WorldGen.genRand.Next(50) + 50, makeOrb);
                                        }
                                    }
                                }
                                break;
                            }
                            num22++;
                        }
                    }
                    num22 = (int)num5;
                    while ((double)num22 < Main.worldSurface - 1.0)
                    {
                        if (Main.tile[k, num22].active)
                        {
                            int num55 = num22 + WorldGen.genRand.Next(10, 14);
                            for (int num56 = num22; num56 < num55; num56++)
                            {
                                if (Main.tile[k, num56].type == 59 || Main.tile[k, num56].type == 60)
                                {
                                    if (k >= num50 + WorldGen.genRand.Next(5) && k < num51 - WorldGen.genRand.Next(5))
                                    {
                                        Main.tile[k, num56].type = 0;
                                    }
                                }
                            }
                            break;
                        }
                        num22++;
                    }
                }
                double num57 = Main.worldSurface + 40.0;
                for (k = num50; k < num51; k++)
                {
                    num57 += (double)WorldGen.genRand.Next(-2, 3);
                    if (num57 < Main.worldSurface + 30.0)
                    {
                        num57 = Main.worldSurface + 30.0;
                    }
                    if (num57 > Main.worldSurface + 50.0)
                    {
                        num57 = Main.worldSurface + 50.0;
                    }
                    int num21 = k;
                    bool flag6 = false;
                    int num22 = (int)num5;
                    while ((double)num22 < num57)
                    {
                        if (Main.tile[num21, num22].active)
                        {
                            if (Main.tile[num21, num22].type == 0 && (double)num22 < Main.worldSurface - 1.0 && !flag6)
                            {
                                WorldGen.SpreadGrass(num21, num22, 0, 23, true);
                            }
                            flag6 = true;
                            if (Main.tile[num21, num22].type == 1)
                            {
                                if (num21 >= num50 + WorldGen.genRand.Next(5) && num21 <= num51 - WorldGen.genRand.Next(5))
                                {
                                    Main.tile[num21, num22].type = 25;
                                }
                            }
                            if (Main.tile[num21, num22].type == 2)
                            {
                                Main.tile[num21, num22].type = 23;
                            }
                        }
                        num22++;
                    }
                }
                for (i = num50; i < num51; i++)
                {
                    for (int j = 0; j < Main.maxTilesY - 50; j++)
                    {
                        if (Main.tile[i, j].active && Main.tile[i, j].type == 31)
                        {
                            int num58 = i - 13;
                            int num59 = i + 13;
                            int num60 = j - 13;
                            int num61 = j + 13;
                            for (m = num58; m < num59; m++)
                            {
                                if (m > 10 && m < Main.maxTilesX - 10)
                                {
                                    for (l = num60; l < num61; l++)
                                    {
                                        if (Math.Abs(m - i) + Math.Abs(l - j) < 9 + WorldGen.genRand.Next(11) && WorldGen.genRand.Next(3) != 0)
                                        {
                                            if (Main.tile[m, l].type != 31)
                                            {
                                                Main.tile[m, l].active = true;
                                                Main.tile[m, l].type = 25;
                                                if (Math.Abs(m - i) <= 1 && Math.Abs(l - j) <= 1)
                                                {
                                                    Main.tile[m, l].active = false;
                                                }
                                            }
                                        }
                                        if (Main.tile[m, l].type != 31)
                                        {
                                            if (Math.Abs(m - i) <= 2 + WorldGen.genRand.Next(3) && Math.Abs(l - j) <= 2 + WorldGen.genRand.Next(3))
                                            {
                                                Main.tile[m, l].active = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                num48++;
            }
            Console.WriteLine();
            Program.printData("Generating mountain caves...");
            for (i = 0; i < WorldGen.numMCaves; i++)
            {
                m = WorldGen.mCaveX[i];
                l = WorldGen.mCaveY[i];
                WorldGen.CaveOpenater(m, l);
                WorldGen.Cavinator(m, l, WorldGen.genRand.Next(40, 50));
            }
            Console.WriteLine();
            Program.printData("Creating beaches...");
            for (int num62 = 0; num62 < 2; num62++)
            {
                if (num62 == 0)
                {
                    int num58 = 0;
                    int num59 = WorldGen.genRand.Next(125, 200);
                    float num63 = 1f;
                    l = 0;
                    while (!Main.tile[num59 - 1, l].active)
                    {
                        l++;
                    }
                    for (i = num59 - 1; i >= num58; i--)
                    {
                        num63 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
                        int j = 0;
                        while ((float)j < (float)l + num63)
                        {
                            if ((float)j < (float)l + num63 * 0.75f - 3f)
                            {
                                Main.tile[i, j].active = false;
                                if (j > l)
                                {
                                    Main.tile[i, j].liquid = 255;
                                }
                                else
                                {
                                    if (j == l)
                                    {
                                        Main.tile[i, j].liquid = 127;
                                    }
                                }
                            }
                            else
                            {
                                if (j > l)
                                {
                                    Main.tile[i, j].type = 53;
                                    Main.tile[i, j].active = true;
                                }
                            }
                            Main.tile[i, j].wall = 0;
                            j++;
                        }
                    }
                }
                else
                {
                    int num58 = Main.maxTilesX - WorldGen.genRand.Next(125, 200);
                    int num59 = Main.maxTilesX;
                    float num63 = 1f;
                    l = 0;
                    while (!Main.tile[num58, l].active)
                    {
                        l++;
                    }
                    for (i = num58; i < num59; i++)
                    {
                        num63 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
                        int j = 0;
                        while ((float)j < (float)l + num63)
                        {
                            if ((float)j < (float)l + num63 * 0.75f - 3f)
                            {
                                Main.tile[i, j].active = false;
                                if (j > l)
                                {
                                    Main.tile[i, j].liquid = 255;
                                }
                                else
                                {
                                    if (j == l)
                                    {
                                        Main.tile[i, j].liquid = 127;
                                    }
                                }
                            }
                            else
                            {
                                if (j > l)
                                {
                                    Main.tile[i, j].type = 53;
                                    Main.tile[i, j].active = true;
                                }
                            }
                            Main.tile[i, j].wall = 0;
                            j++;
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Adding gems...");
            for (i = 63; i <= 68; i++)
            {
                float num64 = 0f;
                if (i == 67)
                {
                    num64 = (float)Main.maxTilesX * 0.5f;
                }
                else
                {
                    if (i == 66)
                    {
                        num64 = (float)Main.maxTilesX * 0.45f;
                    }
                    else
                    {
                        if (i == 63)
                        {
                            num64 = (float)Main.maxTilesX * 0.3f;
                        }
                        else
                        {
                            if (i == 65)
                            {
                                num64 = (float)Main.maxTilesX * 0.25f;
                            }
                            else
                            {
                                if (i == 64)
                                {
                                    num64 = (float)Main.maxTilesX * 0.1f;
                                }
                                else
                                {
                                    if (i == 68)
                                    {
                                        num64 = (float)Main.maxTilesX * 0.05f;
                                    }
                                }
                            }
                        }
                    }
                }
                num64 *= 0.2f;
                int num65 = 0;
                while ((float)num65 < num64)
                {
                    m = WorldGen.genRand.Next(0, Main.maxTilesX);
                    l = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                    while (Main.tile[m, l].type != 1)
                    {
                        m = WorldGen.genRand.Next(0, Main.maxTilesX);
                        l = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                    }
                    WorldGen.TileRunner(m, l, (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(3, 7), i, false, 0f, 0f, false, true);
                    num65++;
                }
            }
            Console.WriteLine();
            for (i = 0; i < Main.maxTilesX; i++)
            {
                float num10 = (float)i / (float)(Main.maxTilesX - 1);
                Program.printData("Gravitating sand: " + (int)(num10 * 100f) + "%");
                for (int j = Main.maxTilesY - 5; j > 0; j--)
                {
                    if (Main.tile[i, j].active && Main.tile[i, j].type == 53)
                    {
                        l = j;
                        while (!Main.tile[i, l + 1].active && l < Main.maxTilesY - 5)
                        {
                            Main.tile[i, l + 1].active = true;
                            Main.tile[i, l + 1].type = 53;
                            l++;
                        }
                    }
                }
            }
            Console.WriteLine();
            for (i = 3; i < Main.maxTilesX - 3; i++)
            {
                float num10 = (float)i / (float)Main.maxTilesX;
                Program.printData("Cleaning up dirt backgrounds: " + (int)(num10 * 100f + 1f) + "%");
                int j = 0;
                while ((double)j < Main.worldSurface)
                {
                    if (Main.tile[i, j].wall == 2)
                    {
                        Main.tile[i, j].wall = 0;
                    }
                    if (Main.tile[i, j].type != 53)
                    {
                        if (Main.tile[i - 1, j].wall == 2)
                        {
                            Main.tile[i - 1, j].wall = 0;
                        }
                        if (Main.tile[i - 2, j].wall == 2 && WorldGen.genRand.Next(2) == 0)
                        {
                            Main.tile[i - 2, j].wall = 0;
                        }
                        if (Main.tile[i - 3, j].wall == 2 && WorldGen.genRand.Next(2) == 0)
                        {
                            Main.tile[i - 3, j].wall = 0;
                        }
                        if (Main.tile[i + 1, j].wall == 2)
                        {
                            Main.tile[i + 1, j].wall = 0;
                        }
                        if (Main.tile[i + 2, j].wall == 2 && WorldGen.genRand.Next(2) == 0)
                        {
                            Main.tile[i + 2, j].wall = 0;
                        }
                        if (Main.tile[i + 3, j].wall == 2 && WorldGen.genRand.Next(2) == 0)
                        {
                            Main.tile[i + 3, j].wall = 0;
                        }
                        if (Main.tile[i, j].active)
                        {
                            break;
                        }
                    }
                    j++;
                }
            }
            Console.WriteLine();
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); k++)
            {
                float num10 = (float)((double)k / ((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05));
                Program.printData("Placing altars: " + (int)(num10 * 100f + 1f) + "%");
                bool flag7 = false;
                int num66 = 0;
                while (!flag7)
                {
                    int num38 = WorldGen.genRand.Next(1, Main.maxTilesX);
                    int num39 = (int)(num6 + 20.0);
                    WorldGen.Place3x2(num38, num39, 26);
                    if (Main.tile[num38, num39].type == 26)
                    {
                        flag7 = true;
                    }
                    else
                    {
                        num66++;
                        if (num66 >= 10000)
                        {
                            flag7 = true;
                        }
                    }
                }
            }
            for (k = 0; k < Main.maxTilesX; k++)
            {
                int num21 = k;
                int num22 = (int)num5;
                while ((double)num22 < Main.worldSurface - 1.0)
                {
                    if (Main.tile[num21, num22].active)
                    {
                        if (Main.tile[num21, num22].type == 60)
                        {
                            Main.tile[num21, num22 - 1].liquid = 255;
                            Main.tile[num21, num22 - 2].liquid = 255;
                        }
                        break;
                    }
                    num22++;
                }
            }
            Console.WriteLine();
            Liquid.QuickWater(3, -1, -1);
            WorldGen.WaterCheck();
            int num67 = 0;
            Liquid.quickSettle = true;
            while (num67 < 10)
            {
                int num68 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                num67++;
                float num69 = 0f;
                while (Liquid.numLiquid > 0)
                {
                    float num10 = (float)(num68 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num68;
                    if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num68)
                    {
                        num68 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                    }
                    if (num10 > num69)
                    {
                        num69 = num10;
                    }
                    else
                    {
                        num10 = num69;
                    }
                    if (num67 == 1)
                    {
                        Program.printData("Settling liquids: " + (int)(num10 * 100f / 3f + 33f) + "%");
                    }
                    int num70 = 10;
                    if (num67 > num70)
                    {
                        num70 = num67;
                    }
                    Liquid.UpdateLiquid();
                }
                WorldGen.WaterCheck();
                Program.printData("Settling liquids: " + (int)((float)num67 * 10f / 3f + 66f) + "%");
            }
            Liquid.quickSettle = false;
            Console.WriteLine();
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.5E-05); k++)
            {
                float num10 = (float)((double)k / ((double)(Main.maxTilesX * Main.maxTilesY) * 2.5E-05));
                Program.printData("Placing life crystals: " + (int)(num10 * 100f + 1f) + "%");
                bool flag8 = false;
                int num71 = 0;
                while (!flag8)
                {
                    if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY)))
                    {
                        flag8 = true;
                    }
                    else
                    {
                        num71++;
                        if (num71 >= 10000)
                        {
                            flag8 = true;
                        }
                    }
                }
            }
            Console.WriteLine();
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 1.8E-05); k++)
            {
                float num10 = (float)((double)k / ((double)(Main.maxTilesX * Main.maxTilesY) * 1.8E-05));
                Program.printData("Hiding treasure: " + (int)(num10 * 100f + 1f) + "%");
                bool flag9 = false;
                int num72 = 0;
                while (!flag9)
                {
                    if (WorldGen.AddBuriedChest(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY), 0))
                    {
                        flag9 = true;
                    }
                    else
                    {
                        num72++;
                        if (num72 >= 10000)
                        {
                            flag9 = true;
                        }
                    }
                }
            }
            int num73 = 0;
            for (k = 0; k < WorldGen.numJChests; k++)
            {
                num73++;
                int contain = 211;
                if (num73 == 1)
                {
                    contain = 211;
                }
                else
                {
                    if (num73 == 2)
                    {
                        contain = 212;
                    }
                    else
                    {
                        if (num73 == 3)
                        {
                            contain = 213;
                        }
                    }
                }
                if (num73 > 3)
                {
                    num73 = 0;
                }
                if (!WorldGen.AddBuriedChest(WorldGen.JChestX[k] + WorldGen.genRand.Next(2), WorldGen.JChestY[k], contain))
                {
                    for (m = WorldGen.JChestX[k]; m <= WorldGen.JChestX[k] + 1; m++)
                    {
                        for (l = WorldGen.JChestY[k]; l <= WorldGen.JChestY[k] + 1; l++)
                        {
                            WorldGen.KillTile(m, l, false, false, false);
                        }
                    }
                    WorldGen.AddBuriedChest(WorldGen.JChestX[k], WorldGen.JChestY[k], contain);
                }
            }
            float num74 = (float)(Main.maxTilesX / 4200);
            int num75 = 0;
            k = 0;
            while ((float)k < 10f * num74)
            {
                int contain2 = 0;
                num75++;
                if (num75 == 1)
                {
                    contain2 = 186;
                }
                else
                {
                    contain2 = 187;
                    num75 = 0;
                }
                bool flag9 = false;
                while (!flag9)
                {
                    m = WorldGen.genRand.Next(1, Main.maxTilesX);
                    l = WorldGen.genRand.Next(1, Main.maxTilesY - 200);
                    while (Main.tile[m, l].liquid < 200 || Main.tile[m, l].lava)
                    {
                        m = WorldGen.genRand.Next(1, Main.maxTilesX);
                        l = WorldGen.genRand.Next(1, Main.maxTilesY - 200);
                    }
                    flag9 = WorldGen.AddBuriedChest(m, l, contain2);
                }
                k++;
            }
            for (i = 0; i < WorldGen.numIslandHouses; i++)
            {
                WorldGen.IslandHouse(WorldGen.fihX[i], WorldGen.fihY[i]);
            }
            Console.WriteLine();
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); k++)
            {
                float num10 = (float)((double)k / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008));
                Program.printData("Placing breakables: " + (int)(num10 * 100f + 1f) + "%");
                bool flag10 = false;
                int num76 = 0;
                while (!flag10)
                {
                    int num77 = WorldGen.genRand.Next((int)num6, Main.maxTilesY - 10);
                    if ((double)num10 > 0.93)
                    {
                        num77 = Main.maxTilesY - 150;
                    }
                    else
                    {
                        if ((double)num10 > 0.75)
                        {
                            num77 = (int)num5;
                        }
                    }
                    int num38 = WorldGen.genRand.Next(1, Main.maxTilesX);
                    bool flag11 = false;
                    for (int num39 = num77; num39 < Main.maxTilesY; num39++)
                    {
                        if (!flag11)
                        {
                            if (Main.tile[num38, num39].active && Main.tileSolid[(int)Main.tile[num38, num39].type])
                            {
                                if (!Main.tile[num38, num39 - 1].lava)
                                {
                                    flag11 = true;
                                }
                            }
                        }
                        else
                        {
                            if (WorldGen.PlacePot(num38, num39, 28))
                            {
                                flag10 = true;
                                break;
                            }
                            num76++;
                            if (num76 >= 10000)
                            {
                                flag10 = true;
                                break;
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
            for (k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 1E-05); k++)
            {
                float num10 = (float)((double)k / ((double)(Main.maxTilesX * Main.maxTilesY) * 1E-05));
                Program.printData("Placing hellforges: " + (int)(num10 * 100f + 1f) + "%");
                bool flag12 = false;
                int num78 = 0;
                while (!flag12)
                {
                    int num38 = WorldGen.genRand.Next(1, Main.maxTilesX);
                    int num39 = WorldGen.genRand.Next(Main.maxTilesY - 250, Main.maxTilesY - 5);
                    try
                    {
                        if (Main.tile[num38, num39].wall == 13)
                        {
                            while (!Main.tile[num38, num39].active)
                            {
                                num39++;
                            }
                            num39--;
                            WorldGen.PlaceTile(num38, num39, 77, false, false, -1);
                            if (Main.tile[num38, num39].type == 77)
                            {
                                flag12 = true;
                            }
                            else
                            {
                                num78++;
                                if (num78 >= 10000)
                                {
                                    flag12 = true;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Spreading grass...");
            for (k = 0; k < Main.maxTilesX; k++)
            {
                int num21 = k;
                bool flag13 = true;
                int num22 = 0;
                while ((double)num22 < Main.worldSurface - 1.0)
                {
                    if (Main.tile[num21, num22].active)
                    {
                        if (flag13 && Main.tile[num21, num22].type == 0)
                        {
                            WorldGen.SpreadGrass(num21, num22, 0, 2, true);
                        }
                        if ((double)num22 > num6)
                        {
                            break;
                        }
                        flag13 = false;
                    }
                    else
                    {
                        if (Main.tile[num21, num22].wall == 0)
                        {
                            flag13 = true;
                        }
                    }
                    num22++;
                }
            }
            int num79 = 5;
            bool flag14 = true;
            while (flag14)
            {
                i = Main.maxTilesX / 2 + WorldGen.genRand.Next(-num79, num79 + 1);
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j].active)
                    {
                        Main.spawnTileX = i;
                        Main.spawnTileY = j;
                        Main.tile[i, j - 1].lighted = true;
                        break;
                    }
                }
                flag14 = false;
                num79++;
                if ((double)Main.spawnTileY > Main.worldSurface)
                {
                    flag14 = true;
                }
                if (Main.tile[Main.spawnTileX, Main.spawnTileY - 1].liquid > 0)
                {
                    flag14 = true;
                }
            }
            int num80 = 10;
            Console.WriteLine();
            while ((double)Main.spawnTileY > Main.worldSurface)
            {
                i = WorldGen.genRand.Next(Main.maxTilesX / 2 - num80, Main.maxTilesX / 2 + num80);
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j].active)
                    {
                        Main.spawnTileX = i;
                        Main.spawnTileY = j;
                        Main.tile[i, j - 1].lighted = true;
                        break;
                    }
                }
                num80++;
            }
            int num81 = NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, 22, 0);
            Main.npc[num81].homeTileX = Main.spawnTileX;
            Main.npc[num81].homeTileY = Main.spawnTileY;
            Main.npc[num81].direction = 1;
            Main.npc[num81].homeless = true;
            Program.printData("Planting sunflowers...");
            num48 = 0;
            while ((double)num48 < (double)Main.maxTilesX * 0.002)
            {
                int num82 = 0;
                int num70 = 0;
                int num83 = Main.maxTilesX / 2;
                int num84 = WorldGen.genRand.Next(Main.maxTilesX);
                num82 = num84 - WorldGen.genRand.Next(10) - 7;
                num70 = num84 + WorldGen.genRand.Next(10) + 7;
                if (num82 < 0)
                {
                    num82 = 0;
                }
                if (num70 > Main.maxTilesX - 1)
                {
                    num70 = Main.maxTilesX - 1;
                }
                for (i = num82; i < num70; i++)
                {
                    int j = 1;
                    while ((double)j < Main.worldSurface - 1.0)
                    {
                        if (Main.tile[i, j].type == 2 && Main.tile[i, j].active)
                        {
                            if (!Main.tile[i, j - 1].active)
                            {
                                WorldGen.PlaceTile(i, j - 1, 27, true, false, -1);
                            }
                        }
                        if (Main.tile[i, j].active)
                        {
                            break;
                        }
                        j++;
                    }
                }
                num48++;
            }
            Console.WriteLine();
            Program.printData("Planting trees...");
            num48 = 0;
            while ((double)num48 < (double)Main.maxTilesX * 0.003)
            {
                m = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int num85 = WorldGen.genRand.Next(25, 50);
                for (i = m - num85; i < m + num85; i++)
                {
                    int j = 20;
                    while ((double)j < Main.worldSurface)
                    {
                        WorldGen.GrowEpicTree(i, j);
                        j++;
                    }
                }
                num48++;
            }
            WorldGen.AddTrees();
            Console.WriteLine();
            Program.printData("Planting weeds...");
            WorldGen.AddPlants();
            for (i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j].active)
                    {
                        if (j >= (int)Main.worldSurface && Main.tile[i, j].type == 70 && !Main.tile[i, j - 1].active)
                        {
                            WorldGen.GrowShroom(i, j);
                            if (!Main.tile[i, j - 1].active)
                            {
                                WorldGen.PlaceTile(i, j - 1, 71, true, false, -1);
                            }
                        }
                        if (Main.tile[i, j].type == 60 && !Main.tile[i, j - 1].active)
                        {
                            WorldGen.PlaceTile(i, j - 1, 61, true, false, -1);
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Growing vines...");
            for (i = 0; i < Main.maxTilesX; i++)
            {
                int num86 = 0;
                int j = 0;
                while ((double)j < Main.worldSurface)
                {
                    if (num86 > 0 && !Main.tile[i, j].active)
                    {
                        Main.tile[i, j].active = true;
                        Main.tile[i, j].type = 52;
                        num86--;
                    }
                    else
                    {
                        num86 = 0;
                    }
                    if (Main.tile[i, j].active && Main.tile[i, j].type == 2)
                    {
                        if (WorldGen.genRand.Next(5) < 3)
                        {
                            num86 = WorldGen.genRand.Next(1, 10);
                        }
                    }
                    j++;
                }
                num86 = 0;
                for (j = 0; j < Main.maxTilesY; j++)
                {
                    if (num86 > 0 && !Main.tile[i, j].active)
                    {
                        Main.tile[i, j].active = true;
                        Main.tile[i, j].type = 62;
                        num86--;
                    }
                    else
                    {
                        num86 = 0;
                    }
                    if (Main.tile[i, j].active && Main.tile[i, j].type == 60)
                    {
                        if (WorldGen.genRand.Next(5) < 3)
                        {
                            num86 = WorldGen.genRand.Next(1, 10);
                        }
                    }
                }
            }
            Console.WriteLine();
            Program.printData("Planting flowers...");
            num48 = 0;
            while ((double)num48 < (double)Main.maxTilesX * 0.005)
            {
                i = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                int num87 = WorldGen.genRand.Next(5, 15);
                int num88 = WorldGen.genRand.Next(15, 30);
                int j = 1;
                while ((double)j < Main.worldSurface - 1.0)
                {
                    if (Main.tile[i, j].active)
                    {
                        for (m = i - num87; m < i + num87; m++)
                        {
                            for (l = j - num88; l < j + num88; l++)
                            {
                                if (Main.tile[m, l].type == 3 || Main.tile[m, l].type == 24)
                                {
                                    Main.tile[m, l].frameX = (short)(WorldGen.genRand.Next(6, 8) * 18);
                                }
                            }
                        }
                        break;
                    }
                    j++;
                }
                num48++;
            }
            Console.WriteLine();
            Program.printData("Planting mushrooms...");
            num48 = 0;
            while ((double)num48 < (double)Main.maxTilesX * 0.002)
            {
                i = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                int num87 = WorldGen.genRand.Next(4, 10);
                int num88 = WorldGen.genRand.Next(15, 30);
                int j = 1;
                while ((double)j < Main.worldSurface - 1.0)
                {
                    if (Main.tile[i, j].active)
                    {
                        for (m = i - num87; m < i + num87; m++)
                        {
                            for (l = j - num88; l < j + num88; l++)
                            {
                                if (Main.tile[m, l].type == 3 || Main.tile[m, l].type == 24)
                                {
                                    Main.tile[m, l].frameX = 144;
                                }
                            }
                        }
                        break;
                    }
                    j++;
                }
                num48++;
            }
            WorldGen.gen = false;
        }
        
        public static bool GrowEpicTree(int i, int y)
        {
            int num = y;
            while (Main.tile[i, num].type == 20)
            {
                num++;
            }
            bool result;
            if (Main.tile[i, num].active)
            {
                if (Main.tile[i, num].type == 2 && Main.tile[i, num - 1].wall == 0 && Main.tile[i, num - 1].liquid == 0)
                {
                    if (Main.tile[i - 1, num].active && Main.tile[i - 1, num].type == 2 && Main.tile[i + 1, num].active && Main.tile[i + 1, num].type == 2)
                    {
                        int num2 = 2;
                        if (WorldGen.EmptyTileCheck(i - num2, i + num2, num - 55, num - 1, 20))
                        {
                            bool flag = false;
                            bool flag2 = false;
                            int num3 = WorldGen.genRand.Next(20, 30);
                            int num4;
                            for (int j = num - num3; j < num; j++)
                            {
                                Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
                                Main.tile[i, j].active = true;
                                Main.tile[i, j].type = 5;
                                num4 = WorldGen.genRand.Next(3);
                                int num5 = WorldGen.genRand.Next(10);
                                if (j == num - 1 || j == num - num3)
                                {
                                    num5 = 0;
                                }
                                while (((num5 == 5 || num5 == 7) && flag) || ((num5 == 6 || num5 == 7) && flag2))
                                {
                                    num5 = WorldGen.genRand.Next(10);
                                }
                                flag = false;
                                flag2 = false;
                                if (num5 == 5 || num5 == 7)
                                {
                                    flag = true;
                                }
                                if (num5 == 6 || num5 == 7)
                                {
                                    flag2 = true;
                                }
                                if (num5 == 1)
                                {
                                    if (num4 == 0)
                                    {
                                        Main.tile[i, j].frameX = 0;
                                        Main.tile[i, j].frameY = 66;
                                    }
                                    if (num4 == 1)
                                    {
                                        Main.tile[i, j].frameX = 0;
                                        Main.tile[i, j].frameY = 88;
                                    }
                                    if (num4 == 2)
                                    {
                                        Main.tile[i, j].frameX = 0;
                                        Main.tile[i, j].frameY = 110;
                                    }
                                }
                                else
                                {
                                    if (num5 == 2)
                                    {
                                        if (num4 == 0)
                                        {
                                            Main.tile[i, j].frameX = 22;
                                            Main.tile[i, j].frameY = 0;
                                        }
                                        if (num4 == 1)
                                        {
                                            Main.tile[i, j].frameX = 22;
                                            Main.tile[i, j].frameY = 22;
                                        }
                                        if (num4 == 2)
                                        {
                                            Main.tile[i, j].frameX = 22;
                                            Main.tile[i, j].frameY = 44;
                                        }
                                    }
                                    else
                                    {
                                        if (num5 == 3)
                                        {
                                            if (num4 == 0)
                                            {
                                                Main.tile[i, j].frameX = 44;
                                                Main.tile[i, j].frameY = 66;
                                            }
                                            if (num4 == 1)
                                            {
                                                Main.tile[i, j].frameX = 44;
                                                Main.tile[i, j].frameY = 88;
                                            }
                                            if (num4 == 2)
                                            {
                                                Main.tile[i, j].frameX = 44;
                                                Main.tile[i, j].frameY = 110;
                                            }
                                        }
                                        else
                                        {
                                            if (num5 == 4)
                                            {
                                                if (num4 == 0)
                                                {
                                                    Main.tile[i, j].frameX = 22;
                                                    Main.tile[i, j].frameY = 66;
                                                }
                                                if (num4 == 1)
                                                {
                                                    Main.tile[i, j].frameX = 22;
                                                    Main.tile[i, j].frameY = 88;
                                                }
                                                if (num4 == 2)
                                                {
                                                    Main.tile[i, j].frameX = 22;
                                                    Main.tile[i, j].frameY = 110;
                                                }
                                            }
                                            else
                                            {
                                                if (num5 == 5)
                                                {
                                                    if (num4 == 0)
                                                    {
                                                        Main.tile[i, j].frameX = 88;
                                                        Main.tile[i, j].frameY = 0;
                                                    }
                                                    if (num4 == 1)
                                                    {
                                                        Main.tile[i, j].frameX = 88;
                                                        Main.tile[i, j].frameY = 22;
                                                    }
                                                    if (num4 == 2)
                                                    {
                                                        Main.tile[i, j].frameX = 88;
                                                        Main.tile[i, j].frameY = 44;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num5 == 6)
                                                    {
                                                        if (num4 == 0)
                                                        {
                                                            Main.tile[i, j].frameX = 66;
                                                            Main.tile[i, j].frameY = 66;
                                                        }
                                                        if (num4 == 1)
                                                        {
                                                            Main.tile[i, j].frameX = 66;
                                                            Main.tile[i, j].frameY = 88;
                                                        }
                                                        if (num4 == 2)
                                                        {
                                                            Main.tile[i, j].frameX = 66;
                                                            Main.tile[i, j].frameY = 110;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 == 7)
                                                        {
                                                            if (num4 == 0)
                                                            {
                                                                Main.tile[i, j].frameX = 110;
                                                                Main.tile[i, j].frameY = 66;
                                                            }
                                                            if (num4 == 1)
                                                            {
                                                                Main.tile[i, j].frameX = 110;
                                                                Main.tile[i, j].frameY = 88;
                                                            }
                                                            if (num4 == 2)
                                                            {
                                                                Main.tile[i, j].frameX = 110;
                                                                Main.tile[i, j].frameY = 110;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num4 == 0)
                                                            {
                                                                Main.tile[i, j].frameX = 0;
                                                                Main.tile[i, j].frameY = 0;
                                                            }
                                                            if (num4 == 1)
                                                            {
                                                                Main.tile[i, j].frameX = 0;
                                                                Main.tile[i, j].frameY = 22;
                                                            }
                                                            if (num4 == 2)
                                                            {
                                                                Main.tile[i, j].frameX = 0;
                                                                Main.tile[i, j].frameY = 44;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (num5 == 5 || num5 == 7)
                                {
                                    Main.tile[i - 1, j].active = true;
                                    Main.tile[i - 1, j].type = 5;
                                    num4 = WorldGen.genRand.Next(3);
                                    if (WorldGen.genRand.Next(3) < 2)
                                    {
                                        if (num4 == 0)
                                        {
                                            Main.tile[i - 1, j].frameX = 44;
                                            Main.tile[i - 1, j].frameY = 198;
                                        }
                                        if (num4 == 1)
                                        {
                                            Main.tile[i - 1, j].frameX = 44;
                                            Main.tile[i - 1, j].frameY = 220;
                                        }
                                        if (num4 == 2)
                                        {
                                            Main.tile[i - 1, j].frameX = 44;
                                            Main.tile[i - 1, j].frameY = 242;
                                        }
                                    }
                                    else
                                    {
                                        if (num4 == 0)
                                        {
                                            Main.tile[i - 1, j].frameX = 66;
                                            Main.tile[i - 1, j].frameY = 0;
                                        }
                                        if (num4 == 1)
                                        {
                                            Main.tile[i - 1, j].frameX = 66;
                                            Main.tile[i - 1, j].frameY = 22;
                                        }
                                        if (num4 == 2)
                                        {
                                            Main.tile[i - 1, j].frameX = 66;
                                            Main.tile[i - 1, j].frameY = 44;
                                        }
                                    }
                                }
                                if (num5 == 6 || num5 == 7)
                                {
                                    Main.tile[i + 1, j].active = true;
                                    Main.tile[i + 1, j].type = 5;
                                    num4 = WorldGen.genRand.Next(3);
                                    if (WorldGen.genRand.Next(3) < 2)
                                    {
                                        if (num4 == 0)
                                        {
                                            Main.tile[i + 1, j].frameX = 66;
                                            Main.tile[i + 1, j].frameY = 198;
                                        }
                                        if (num4 == 1)
                                        {
                                            Main.tile[i + 1, j].frameX = 66;
                                            Main.tile[i + 1, j].frameY = 220;
                                        }
                                        if (num4 == 2)
                                        {
                                            Main.tile[i + 1, j].frameX = 66;
                                            Main.tile[i + 1, j].frameY = 242;
                                        }
                                    }
                                    else
                                    {
                                        if (num4 == 0)
                                        {
                                            Main.tile[i + 1, j].frameX = 88;
                                            Main.tile[i + 1, j].frameY = 66;
                                        }
                                        if (num4 == 1)
                                        {
                                            Main.tile[i + 1, j].frameX = 88;
                                            Main.tile[i + 1, j].frameY = 88;
                                        }
                                        if (num4 == 2)
                                        {
                                            Main.tile[i + 1, j].frameX = 88;
                                            Main.tile[i + 1, j].frameY = 110;
                                        }
                                    }
                                }
                            }
                            int num6 = WorldGen.genRand.Next(3);
                            if (num6 == 0 || num6 == 1)
                            {
                                Main.tile[i + 1, num - 1].active = true;
                                Main.tile[i + 1, num - 1].type = 5;
                                num4 = WorldGen.genRand.Next(3);
                                if (num4 == 0)
                                {
                                    Main.tile[i + 1, num - 1].frameX = 22;
                                    Main.tile[i + 1, num - 1].frameY = 132;
                                }
                                if (num4 == 1)
                                {
                                    Main.tile[i + 1, num - 1].frameX = 22;
                                    Main.tile[i + 1, num - 1].frameY = 154;
                                }
                                if (num4 == 2)
                                {
                                    Main.tile[i + 1, num - 1].frameX = 22;
                                    Main.tile[i + 1, num - 1].frameY = 176;
                                }
                            }
                            if (num6 == 0 || num6 == 2)
                            {
                                Main.tile[i - 1, num - 1].active = true;
                                Main.tile[i - 1, num - 1].type = 5;
                                num4 = WorldGen.genRand.Next(3);
                                if (num4 == 0)
                                {
                                    Main.tile[i - 1, num - 1].frameX = 44;
                                    Main.tile[i - 1, num - 1].frameY = 132;
                                }
                                if (num4 == 1)
                                {
                                    Main.tile[i - 1, num - 1].frameX = 44;
                                    Main.tile[i - 1, num - 1].frameY = 154;
                                }
                                if (num4 == 2)
                                {
                                    Main.tile[i - 1, num - 1].frameX = 44;
                                    Main.tile[i - 1, num - 1].frameY = 176;
                                }
                            }
                            num4 = WorldGen.genRand.Next(3);
                            if (num6 == 0)
                            {
                                if (num4 == 0)
                                {
                                    Main.tile[i, num - 1].frameX = 88;
                                    Main.tile[i, num - 1].frameY = 132;
                                }
                                if (num4 == 1)
                                {
                                    Main.tile[i, num - 1].frameX = 88;
                                    Main.tile[i, num - 1].frameY = 154;
                                }
                                if (num4 == 2)
                                {
                                    Main.tile[i, num - 1].frameX = 88;
                                    Main.tile[i, num - 1].frameY = 176;
                                }
                            }
                            else
                            {
                                if (num6 == 1)
                                {
                                    if (num4 == 0)
                                    {
                                        Main.tile[i, num - 1].frameX = 0;
                                        Main.tile[i, num - 1].frameY = 132;
                                    }
                                    if (num4 == 1)
                                    {
                                        Main.tile[i, num - 1].frameX = 0;
                                        Main.tile[i, num - 1].frameY = 154;
                                    }
                                    if (num4 == 2)
                                    {
                                        Main.tile[i, num - 1].frameX = 0;
                                        Main.tile[i, num - 1].frameY = 176;
                                    }
                                }
                                else
                                {
                                    if (num6 == 2)
                                    {
                                        if (num4 == 0)
                                        {
                                            Main.tile[i, num - 1].frameX = 66;
                                            Main.tile[i, num - 1].frameY = 132;
                                        }
                                        if (num4 == 1)
                                        {
                                            Main.tile[i, num - 1].frameX = 66;
                                            Main.tile[i, num - 1].frameY = 154;
                                        }
                                        if (num4 == 2)
                                        {
                                            Main.tile[i, num - 1].frameX = 66;
                                            Main.tile[i, num - 1].frameY = 176;
                                        }
                                    }
                                }
                            }
                            if (WorldGen.genRand.Next(3) < 2)
                            {
                                num4 = WorldGen.genRand.Next(3);
                                if (num4 == 0)
                                {
                                    Main.tile[i, num - num3].frameX = 22;
                                    Main.tile[i, num - num3].frameY = 198;
                                }
                                if (num4 == 1)
                                {
                                    Main.tile[i, num - num3].frameX = 22;
                                    Main.tile[i, num - num3].frameY = 220;
                                }
                                if (num4 == 2)
                                {
                                    Main.tile[i, num - num3].frameX = 22;
                                    Main.tile[i, num - num3].frameY = 242;
                                }
                            }
                            else
                            {
                                num4 = WorldGen.genRand.Next(3);
                                if (num4 == 0)
                                {
                                    Main.tile[i, num - num3].frameX = 0;
                                    Main.tile[i, num - num3].frameY = 198;
                                }
                                if (num4 == 1)
                                {
                                    Main.tile[i, num - num3].frameX = 0;
                                    Main.tile[i, num - num3].frameY = 220;
                                }
                                if (num4 == 2)
                                {
                                    Main.tile[i, num - num3].frameX = 0;
                                    Main.tile[i, num - num3].frameY = 242;
                                }
                            }
                            WorldGen.RangeFrame(i - 2, num - num3 - 1, i + 2, num + 1);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, i, (int)((double)num - (double)num3 * 0.5), num3 + 1);
                            }
                            result = true;
                            return result;
                        }
                    }
                }
            }
            result = false;
            return result;
        }
        
        public static void GrowTree(int i, int y)
        {
            int num = y;
            while (Main.tile[i, num].type == 20)
            {
                num++;
            }
            if ((Main.tile[i - 1, num - 1].liquid == 0 && Main.tile[i - 1, num - 1].liquid == 0 && Main.tile[i + 1, num - 1].liquid == 0) || Main.tile[i, num].type == 60)
            {
                if (Main.tile[i, num].active)
                {
                    if ((Main.tile[i, num].type == 2 || Main.tile[i, num].type == 23 || Main.tile[i, num].type == 60) && Main.tile[i, num - 1].wall == 0)
                    {
                        if (Main.tile[i - 1, num].active && (Main.tile[i - 1, num].type == 2 || Main.tile[i - 1, num].type == 23 || Main.tile[i - 1, num].type == 60) && Main.tile[i + 1, num].active && (Main.tile[i + 1, num].type == 2 || Main.tile[i + 1, num].type == 23 || Main.tile[i + 1, num].type == 60))
                        {
                            int num2 = 2;
                            if (WorldGen.EmptyTileCheck(i - num2, i + num2, num - 14, num - 1, 20))
                            {
                                bool flag = false;
                                bool flag2 = false;
                                int num3 = WorldGen.genRand.Next(5, 15);
                                int num4;
                                for (int j = num - num3; j < num; j++)
                                {
                                    Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
                                    Main.tile[i, j].active = true;
                                    Main.tile[i, j].type = 5;
                                    num4 = WorldGen.genRand.Next(3);
                                    int num5 = WorldGen.genRand.Next(10);
                                    if (j == num - 1 || j == num - num3)
                                    {
                                        num5 = 0;
                                    }
                                    while (((num5 == 5 || num5 == 7) && flag) || ((num5 == 6 || num5 == 7) && flag2))
                                    {
                                        num5 = WorldGen.genRand.Next(10);
                                    }
                                    flag = false;
                                    flag2 = false;
                                    if (num5 == 5 || num5 == 7)
                                    {
                                        flag = true;
                                    }
                                    if (num5 == 6 || num5 == 7)
                                    {
                                        flag2 = true;
                                    }
                                    if (num5 == 1)
                                    {
                                        if (num4 == 0)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 66;
                                        }
                                        if (num4 == 1)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 88;
                                        }
                                        if (num4 == 2)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 110;
                                        }
                                    }
                                    else
                                    {
                                        if (num5 == 2)
                                        {
                                            if (num4 == 0)
                                            {
                                                Main.tile[i, j].frameX = 22;
                                                Main.tile[i, j].frameY = 0;
                                            }
                                            if (num4 == 1)
                                            {
                                                Main.tile[i, j].frameX = 22;
                                                Main.tile[i, j].frameY = 22;
                                            }
                                            if (num4 == 2)
                                            {
                                                Main.tile[i, j].frameX = 22;
                                                Main.tile[i, j].frameY = 44;
                                            }
                                        }
                                        else
                                        {
                                            if (num5 == 3)
                                            {
                                                if (num4 == 0)
                                                {
                                                    Main.tile[i, j].frameX = 44;
                                                    Main.tile[i, j].frameY = 66;
                                                }
                                                if (num4 == 1)
                                                {
                                                    Main.tile[i, j].frameX = 44;
                                                    Main.tile[i, j].frameY = 88;
                                                }
                                                if (num4 == 2)
                                                {
                                                    Main.tile[i, j].frameX = 44;
                                                    Main.tile[i, j].frameY = 110;
                                                }
                                            }
                                            else
                                            {
                                                if (num5 == 4)
                                                {
                                                    if (num4 == 0)
                                                    {
                                                        Main.tile[i, j].frameX = 22;
                                                        Main.tile[i, j].frameY = 66;
                                                    }
                                                    if (num4 == 1)
                                                    {
                                                        Main.tile[i, j].frameX = 22;
                                                        Main.tile[i, j].frameY = 88;
                                                    }
                                                    if (num4 == 2)
                                                    {
                                                        Main.tile[i, j].frameX = 22;
                                                        Main.tile[i, j].frameY = 110;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num5 == 5)
                                                    {
                                                        if (num4 == 0)
                                                        {
                                                            Main.tile[i, j].frameX = 88;
                                                            Main.tile[i, j].frameY = 0;
                                                        }
                                                        if (num4 == 1)
                                                        {
                                                            Main.tile[i, j].frameX = 88;
                                                            Main.tile[i, j].frameY = 22;
                                                        }
                                                        if (num4 == 2)
                                                        {
                                                            Main.tile[i, j].frameX = 88;
                                                            Main.tile[i, j].frameY = 44;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 == 6)
                                                        {
                                                            if (num4 == 0)
                                                            {
                                                                Main.tile[i, j].frameX = 66;
                                                                Main.tile[i, j].frameY = 66;
                                                            }
                                                            if (num4 == 1)
                                                            {
                                                                Main.tile[i, j].frameX = 66;
                                                                Main.tile[i, j].frameY = 88;
                                                            }
                                                            if (num4 == 2)
                                                            {
                                                                Main.tile[i, j].frameX = 66;
                                                                Main.tile[i, j].frameY = 110;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num5 == 7)
                                                            {
                                                                if (num4 == 0)
                                                                {
                                                                    Main.tile[i, j].frameX = 110;
                                                                    Main.tile[i, j].frameY = 66;
                                                                }
                                                                if (num4 == 1)
                                                                {
                                                                    Main.tile[i, j].frameX = 110;
                                                                    Main.tile[i, j].frameY = 88;
                                                                }
                                                                if (num4 == 2)
                                                                {
                                                                    Main.tile[i, j].frameX = 110;
                                                                    Main.tile[i, j].frameY = 110;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num4 == 0)
                                                                {
                                                                    Main.tile[i, j].frameX = 0;
                                                                    Main.tile[i, j].frameY = 0;
                                                                }
                                                                if (num4 == 1)
                                                                {
                                                                    Main.tile[i, j].frameX = 0;
                                                                    Main.tile[i, j].frameY = 22;
                                                                }
                                                                if (num4 == 2)
                                                                {
                                                                    Main.tile[i, j].frameX = 0;
                                                                    Main.tile[i, j].frameY = 44;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (num5 == 5 || num5 == 7)
                                    {
                                        Main.tile[i - 1, j].active = true;
                                        Main.tile[i - 1, j].type = 5;
                                        num4 = WorldGen.genRand.Next(3);
                                        if (WorldGen.genRand.Next(3) < 2)
                                        {
                                            if (num4 == 0)
                                            {
                                                Main.tile[i - 1, j].frameX = 44;
                                                Main.tile[i - 1, j].frameY = 198;
                                            }
                                            if (num4 == 1)
                                            {
                                                Main.tile[i - 1, j].frameX = 44;
                                                Main.tile[i - 1, j].frameY = 220;
                                            }
                                            if (num4 == 2)
                                            {
                                                Main.tile[i - 1, j].frameX = 44;
                                                Main.tile[i - 1, j].frameY = 242;
                                            }
                                        }
                                        else
                                        {
                                            if (num4 == 0)
                                            {
                                                Main.tile[i - 1, j].frameX = 66;
                                                Main.tile[i - 1, j].frameY = 0;
                                            }
                                            if (num4 == 1)
                                            {
                                                Main.tile[i - 1, j].frameX = 66;
                                                Main.tile[i - 1, j].frameY = 22;
                                            }
                                            if (num4 == 2)
                                            {
                                                Main.tile[i - 1, j].frameX = 66;
                                                Main.tile[i - 1, j].frameY = 44;
                                            }
                                        }
                                    }
                                    if (num5 == 6 || num5 == 7)
                                    {
                                        Main.tile[i + 1, j].active = true;
                                        Main.tile[i + 1, j].type = 5;
                                        num4 = WorldGen.genRand.Next(3);
                                        if (WorldGen.genRand.Next(3) < 2)
                                        {
                                            if (num4 == 0)
                                            {
                                                Main.tile[i + 1, j].frameX = 66;
                                                Main.tile[i + 1, j].frameY = 198;
                                            }
                                            if (num4 == 1)
                                            {
                                                Main.tile[i + 1, j].frameX = 66;
                                                Main.tile[i + 1, j].frameY = 220;
                                            }
                                            if (num4 == 2)
                                            {
                                                Main.tile[i + 1, j].frameX = 66;
                                                Main.tile[i + 1, j].frameY = 242;
                                            }
                                        }
                                        else
                                        {
                                            if (num4 == 0)
                                            {
                                                Main.tile[i + 1, j].frameX = 88;
                                                Main.tile[i + 1, j].frameY = 66;
                                            }
                                            if (num4 == 1)
                                            {
                                                Main.tile[i + 1, j].frameX = 88;
                                                Main.tile[i + 1, j].frameY = 88;
                                            }
                                            if (num4 == 2)
                                            {
                                                Main.tile[i + 1, j].frameX = 88;
                                                Main.tile[i + 1, j].frameY = 110;
                                            }
                                        }
                                    }
                                }
                                int num6 = WorldGen.genRand.Next(3);
                                if (num6 == 0 || num6 == 1)
                                {
                                    Main.tile[i + 1, num - 1].active = true;
                                    Main.tile[i + 1, num - 1].type = 5;
                                    num4 = WorldGen.genRand.Next(3);
                                    if (num4 == 0)
                                    {
                                        Main.tile[i + 1, num - 1].frameX = 22;
                                        Main.tile[i + 1, num - 1].frameY = 132;
                                    }
                                    if (num4 == 1)
                                    {
                                        Main.tile[i + 1, num - 1].frameX = 22;
                                        Main.tile[i + 1, num - 1].frameY = 154;
                                    }
                                    if (num4 == 2)
                                    {
                                        Main.tile[i + 1, num - 1].frameX = 22;
                                        Main.tile[i + 1, num - 1].frameY = 176;
                                    }
                                }
                                if (num6 == 0 || num6 == 2)
                                {
                                    Main.tile[i - 1, num - 1].active = true;
                                    Main.tile[i - 1, num - 1].type = 5;
                                    num4 = WorldGen.genRand.Next(3);
                                    if (num4 == 0)
                                    {
                                        Main.tile[i - 1, num - 1].frameX = 44;
                                        Main.tile[i - 1, num - 1].frameY = 132;
                                    }
                                    if (num4 == 1)
                                    {
                                        Main.tile[i - 1, num - 1].frameX = 44;
                                        Main.tile[i - 1, num - 1].frameY = 154;
                                    }
                                    if (num4 == 2)
                                    {
                                        Main.tile[i - 1, num - 1].frameX = 44;
                                        Main.tile[i - 1, num - 1].frameY = 176;
                                    }
                                }
                                num4 = WorldGen.genRand.Next(3);
                                if (num6 == 0)
                                {
                                    if (num4 == 0)
                                    {
                                        Main.tile[i, num - 1].frameX = 88;
                                        Main.tile[i, num - 1].frameY = 132;
                                    }
                                    if (num4 == 1)
                                    {
                                        Main.tile[i, num - 1].frameX = 88;
                                        Main.tile[i, num - 1].frameY = 154;
                                    }
                                    if (num4 == 2)
                                    {
                                        Main.tile[i, num - 1].frameX = 88;
                                        Main.tile[i, num - 1].frameY = 176;
                                    }
                                }
                                else
                                {
                                    if (num6 == 1)
                                    {
                                        if (num4 == 0)
                                        {
                                            Main.tile[i, num - 1].frameX = 0;
                                            Main.tile[i, num - 1].frameY = 132;
                                        }
                                        if (num4 == 1)
                                        {
                                            Main.tile[i, num - 1].frameX = 0;
                                            Main.tile[i, num - 1].frameY = 154;
                                        }
                                        if (num4 == 2)
                                        {
                                            Main.tile[i, num - 1].frameX = 0;
                                            Main.tile[i, num - 1].frameY = 176;
                                        }
                                    }
                                    else
                                    {
                                        if (num6 == 2)
                                        {
                                            if (num4 == 0)
                                            {
                                                Main.tile[i, num - 1].frameX = 66;
                                                Main.tile[i, num - 1].frameY = 132;
                                            }
                                            if (num4 == 1)
                                            {
                                                Main.tile[i, num - 1].frameX = 66;
                                                Main.tile[i, num - 1].frameY = 154;
                                            }
                                            if (num4 == 2)
                                            {
                                                Main.tile[i, num - 1].frameX = 66;
                                                Main.tile[i, num - 1].frameY = 176;
                                            }
                                        }
                                    }
                                }
                                if (WorldGen.genRand.Next(3) < 2)
                                {
                                    num4 = WorldGen.genRand.Next(3);
                                    if (num4 == 0)
                                    {
                                        Main.tile[i, num - num3].frameX = 22;
                                        Main.tile[i, num - num3].frameY = 198;
                                    }
                                    if (num4 == 1)
                                    {
                                        Main.tile[i, num - num3].frameX = 22;
                                        Main.tile[i, num - num3].frameY = 220;
                                    }
                                    if (num4 == 2)
                                    {
                                        Main.tile[i, num - num3].frameX = 22;
                                        Main.tile[i, num - num3].frameY = 242;
                                    }
                                }
                                else
                                {
                                    num4 = WorldGen.genRand.Next(3);
                                    if (num4 == 0)
                                    {
                                        Main.tile[i, num - num3].frameX = 0;
                                        Main.tile[i, num - num3].frameY = 198;
                                    }
                                    if (num4 == 1)
                                    {
                                        Main.tile[i, num - num3].frameX = 0;
                                        Main.tile[i, num - num3].frameY = 220;
                                    }
                                    if (num4 == 2)
                                    {
                                        Main.tile[i, num - num3].frameX = 0;
                                        Main.tile[i, num - num3].frameY = 242;
                                    }
                                }
                                WorldGen.RangeFrame(i - 2, num - num3 - 1, i + 2, num + 1);
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendTileSquare(-1, i, (int)((double)num - (double)num3 * 0.5), num3 + 1);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static void GrowShroom(int i, int y)
        {
            if (!Main.tile[i - 1, y - 1].lava && !Main.tile[i - 1, y - 1].lava && !Main.tile[i + 1, y - 1].lava)
            {
                if (Main.tile[i, y].active)
                {
                    if (Main.tile[i, y].type == 70 && Main.tile[i, y - 1].wall == 0)
                    {
                        if (Main.tile[i - 1, y].active && Main.tile[i - 1, y].type == 70 && Main.tile[i + 1, y].active && Main.tile[i + 1, y].type == 70)
                        {
                            if (WorldGen.EmptyTileCheck(i - 2, i + 2, y - 13, y - 1, 71))
                            {
                                int num = WorldGen.genRand.Next(4, 11);
                                int num2;
                                for (int j = y - num; j < y; j++)
                                {
                                    Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
                                    Main.tile[i, j].active = true;
                                    Main.tile[i, j].type = 72;
                                    num2 = WorldGen.genRand.Next(3);
                                    if (num2 == 0)
                                    {
                                        Main.tile[i, j].frameX = 0;
                                        Main.tile[i, j].frameY = 0;
                                    }
                                    if (num2 == 1)
                                    {
                                        Main.tile[i, j].frameX = 0;
                                        Main.tile[i, j].frameY = 18;
                                    }
                                    if (num2 == 2)
                                    {
                                        Main.tile[i, j].frameX = 0;
                                        Main.tile[i, j].frameY = 36;
                                    }
                                }
                                num2 = WorldGen.genRand.Next(3);
                                if (num2 == 0)
                                {
                                    Main.tile[i, y - num].frameX = 36;
                                    Main.tile[i, y - num].frameY = 0;
                                }
                                if (num2 == 1)
                                {
                                    Main.tile[i, y - num].frameX = 36;
                                    Main.tile[i, y - num].frameY = 18;
                                }
                                if (num2 == 2)
                                {
                                    Main.tile[i, y - num].frameX = 36;
                                    Main.tile[i, y - num].frameY = 36;
                                }
                                WorldGen.RangeFrame(i - 2, y - num - 1, i + 2, y + 1);
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendTileSquare(-1, i, (int)((double)y - (double)num * 0.5), num + 1);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static void AddTrees()
        {
            for (int i = 1; i < Main.maxTilesX - 1; i++)
            {
                int num = 20;
                while ((double)num < Main.worldSurface)
                {
                    WorldGen.GrowTree(i, num);
                    num++;
                }
            }
        }
        
        public static bool EmptyTileCheck(int startX, int endX, int startY, int endY, int ignoreStyle = -1)
        {
            bool result;
            if (startX < 0)
            {
                result = false;
            }
            else
            {
                if (endX >= Main.maxTilesX)
                {
                    result = false;
                }
                else
                {
                    if (startY < 0)
                    {
                        result = false;
                    }
                    else
                    {
                        if (endY >= Main.maxTilesY)
                        {
                            result = false;
                        }
                        else
                        {
                            for (int i = startX; i < endX + 1; i++)
                            {
                                for (int j = startY; j < endY + 1; j++)
                                {
                                    if (Main.tile[i, j].active)
                                    {
                                        if (ignoreStyle == -1)
                                        {
                                            result = false;
                                            return result;
                                        }
                                        if (ignoreStyle == 11 && Main.tile[i, j].type != 11)
                                        {
                                            result = false;
                                            return result;
                                        }
                                        if (ignoreStyle == 20 && Main.tile[i, j].type != 20 && Main.tile[i, j].type != 3 && Main.tile[i, j].type != 24 && Main.tile[i, j].type != 61 && Main.tile[i, j].type != 32 && Main.tile[i, j].type != 69 && Main.tile[i, j].type != 73 && Main.tile[i, j].type != 74)
                                        {
                                            result = false;
                                            return result;
                                        }
                                        if (ignoreStyle == 71 && Main.tile[i, j].type != 71)
                                        {
                                            result = false;
                                            return result;
                                        }
                                    }
                                }
                            }
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
        
        public static bool PlaceDoor(int i, int j, int type)
        {
            bool result;
            try
            {
                if (Main.tile[i, j - 2].active && Main.tileSolid[(int)Main.tile[i, j - 2].type] && Main.tile[i, j + 2].active && Main.tileSolid[(int)Main.tile[i, j + 2].type])
                {
                    Main.tile[i, j - 1].active = true;
                    Main.tile[i, j - 1].type = 10;
                    Main.tile[i, j - 1].frameY = 0;
                    Main.tile[i, j - 1].frameX = (short)(WorldGen.genRand.Next(3) * 18);
                    Main.tile[i, j].active = true;
                    Main.tile[i, j].type = 10;
                    Main.tile[i, j].frameY = 18;
                    Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(3) * 18);
                    Main.tile[i, j + 1].active = true;
                    Main.tile[i, j + 1].type = 10;
                    Main.tile[i, j + 1].frameY = 36;
                    Main.tile[i, j + 1].frameX = (short)(WorldGen.genRand.Next(3) * 18);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
        
        public static bool CloseDoor(int i, int j, bool forced = false)
        {
            int num = 0;
            int num2 = i;
            int num3 = j;
            if (Main.tile[i, j] == null)
            {
                Main.tile[i, j] = new Tile();
            }
            int frameX = (int)Main.tile[i, j].frameX;
            int frameY = (int)Main.tile[i, j].frameY;
            if (frameX == 0)
            {
                num2 = i;
                num = 1;
            }
            else
            {
                if (frameX == 18)
                {
                    num2 = i - 1;
                    num = 1;
                }
                else
                {
                    if (frameX == 36)
                    {
                        num2 = i + 1;
                        num = -1;
                    }
                    else
                    {
                        if (frameX == 54)
                        {
                            num2 = i;
                            num = -1;
                        }
                    }
                }
            }
            if (frameY == 0)
            {
                num3 = j;
            }
            else
            {
                if (frameY == 18)
                {
                    num3 = j - 1;
                }
                else
                {
                    if (frameY == 36)
                    {
                        num3 = j - 2;
                    }
                }
            }
            int num4 = num2;
            if (num == -1)
            {
                num4 = num2 - 1;
            }
            bool result;
            if (!forced)
            {
                for (int k = num3; k < num3 + 3; k++)
                {
                    if (!Collision.EmptyTile(num2, k, true))
                    {
                        result = false;
                        return result;
                    }
                }
            }
            for (int l = num4; l < num4 + 2; l++)
            {
                for (int k = num3; k < num3 + 3; k++)
                {
                    if (l == num2)
                    {
                        if (Main.tile[l, k] == null)
                        {
                            Main.tile[l, k] = new Tile();
                        }
                        Main.tile[l, k].type = 10;
                        Main.tile[l, k].frameX = (short)(WorldGen.genRand.Next(3) * 18);
                    }
                    else
                    {
                        if (Main.tile[l, k] == null)
                        {
                            Main.tile[l, k] = new Tile();
                        }
                        Main.tile[l, k].active = false;
                    }
                }
            }
            for (int num5 = num2 - 1; num5 <= num2 + 1; num5++)
            {
                for (int num6 = num3 - 1; num6 <= num3 + 2; num6++)
                {
                    WorldGen.TileFrame(num5, num6, false, false);
                }
            }
            //Main.PlaySound(9, i * 16, j * 16, 1);
            result = true;
            return result;
        }
        
        public static bool AddLifeCrystal(int i, int j)
        {
            bool result;
            for (int k = j; k < Main.maxTilesY; k++)
            {
                if (Main.tile[i, k].active && Main.tileSolid[(int)Main.tile[i, k].type])
                {
                    int num = k - 1;
                    if (Main.tile[i, num - 1].lava || Main.tile[i - 1, num - 1].lava)
                    {
                        result = false;
                    }
                    else
                    {
                        if (!WorldGen.EmptyTileCheck(i - 1, i, num - 1, num, -1))
                        {
                            result = false;
                        }
                        else
                        {
                            Main.tile[i - 1, num - 1].active = true;
                            Main.tile[i - 1, num - 1].type = 12;
                            Main.tile[i - 1, num - 1].frameX = 0;
                            Main.tile[i - 1, num - 1].frameY = 0;
                            Main.tile[i, num - 1].active = true;
                            Main.tile[i, num - 1].type = 12;
                            Main.tile[i, num - 1].frameX = 18;
                            Main.tile[i, num - 1].frameY = 0;
                            Main.tile[i - 1, num].active = true;
                            Main.tile[i - 1, num].type = 12;
                            Main.tile[i - 1, num].frameX = 0;
                            Main.tile[i - 1, num].frameY = 18;
                            Main.tile[i, num].active = true;
                            Main.tile[i, num].type = 12;
                            Main.tile[i, num].frameX = 18;
                            Main.tile[i, num].frameY = 18;
                            result = true;
                        }
                    }
                    return result;
                }
            }
            result = false;
            return result;
        }
        
        public static void AddShadowOrb(int x, int y)
        {
            if (x >= 10 && x <= Main.maxTilesX - 10)
            {
                if (y >= 10 && y <= Main.maxTilesY - 10)
                {
                    Main.tile[x - 1, y - 1].active = true;
                    Main.tile[x - 1, y - 1].type = 31;
                    Main.tile[x - 1, y - 1].frameX = 0;
                    Main.tile[x - 1, y - 1].frameY = 0;
                    Main.tile[x, y - 1].active = true;
                    Main.tile[x, y - 1].type = 31;
                    Main.tile[x, y - 1].frameX = 18;
                    Main.tile[x, y - 1].frameY = 0;
                    Main.tile[x - 1, y].active = true;
                    Main.tile[x - 1, y].type = 31;
                    Main.tile[x - 1, y].frameX = 0;
                    Main.tile[x - 1, y].frameY = 18;
                    Main.tile[x, y].active = true;
                    Main.tile[x, y].type = 31;
                    Main.tile[x, y].frameX = 18;
                    Main.tile[x, y].frameY = 18;
                }
            }
        }
        
        public static void AddHellHouses()
        {
            int num = (int)((double)Main.maxTilesX * 0.25);
            for (int i = num; i < Main.maxTilesX - num; i++)
            {
                int num2 = Main.maxTilesY - 40;
                while (Main.tile[i, num2].active || Main.tile[i, num2].liquid > 0)
                {
                    num2--;
                }
                if (Main.tile[i, num2 + 1].active)
                {
                    WorldGen.HellHouse(i, num2);
                    i += WorldGen.genRand.Next(15, 80);
                }
            }
        }
        
        public static void HellHouse(int i, int j)
        {
            int num = WorldGen.genRand.Next(8, 20);
            int num2 = WorldGen.genRand.Next(3);
            int num3 = WorldGen.genRand.Next(7);
            int num4 = j;
            for (int k = 0; k < num2; k++)
            {
                int num5 = WorldGen.genRand.Next(5, 9);
                WorldGen.HellRoom(i, num4, num, num5);
                num4 -= num5;
            }
            num4 = j;
            for (int k = 0; k < num3; k++)
            {
                int num5 = WorldGen.genRand.Next(5, 9);
                num4 += num5;
                WorldGen.HellRoom(i, num4, num, num5);
            }
            for (int num6 = i - num / 2; num6 <= i + num / 2; num6++)
            {
                num4 = j;
                while (num4 < Main.maxTilesY && ((Main.tile[num6, num4].active && Main.tile[num6, num4].type == 76) || Main.tile[num6, num4].wall == 13))
                {
                    num4++;
                }
                int num7 = 6 + WorldGen.genRand.Next(3);
                while (num4 < Main.maxTilesY && !Main.tile[num6, num4].active)
                {
                    num7--;
                    Main.tile[num6, num4].active = true;
                    Main.tile[num6, num4].type = 57;
                    num4++;
                    if (num7 <= 0)
                    {
                        break;
                    }
                }
            }
            num4 = j;
            while (num4 < Main.maxTilesY && ((Main.tile[i, num4].active && Main.tile[i, num4].type == 76) || Main.tile[i, num4].wall == 13))
            {
                num4++;
            }
            num4--;
            int num8 = num4;
            while ((Main.tile[i, num4].active && Main.tile[i, num4].type == 76) || Main.tile[i, num4].wall == 13)
            {
                num4--;
                if (Main.tile[i, num4].active && Main.tile[i, num4].type == 76)
                {
                    int num9 = WorldGen.genRand.Next(i - num / 2 + 1, i + num / 2 - 1);
                    int num10 = WorldGen.genRand.Next(i - num / 2 + 1, i + num / 2 - 1);
                    if (num9 > num10)
                    {
                        int num11 = num9;
                        num9 = num10;
                        num10 = num11;
                    }
                    if (num9 == num10)
                    {
                        if (num9 < i)
                        {
                            num10++;
                        }
                        else
                        {
                            num9--;
                        }
                    }
                    for (int num6 = num9; num6 <= num10; num6++)
                    {
                        if (Main.tile[num6, num4 - 1].wall == 13)
                        {
                            Main.tile[num6, num4].wall = 13;
                        }
                        Main.tile[num6, num4].type = 19;
                        Main.tile[num6, num4].active = true;
                    }
                    num4--;
                }
            }
            int num12 = num4;
            float num13 = (float)((num8 - num12) * num);
            float num14 = num13 * 0.02f;
            int num15 = 0;
            while ((float)num15 < num14)
            {
                int num16 = WorldGen.genRand.Next(i - num / 2, i + num / 2 + 1);
                int num17 = WorldGen.genRand.Next(num12, num8);
                int num18 = WorldGen.genRand.Next(3, 8);
                for (int num6 = num16 - num18; num6 <= num16 + num18; num6++)
                {
                    for (int num19 = num17 - num18; num19 <= num17 + num18; num19++)
                    {
                        float num20 = (float)Math.Abs(num6 - num16);
                        float num21 = (float)Math.Abs(num19 - num17);
                        double num22 = Math.Sqrt((double)(num20 * num20 + num21 * num21));
                        if (num22 < (double)num18 * 0.4)
                        {
                            if (Main.tile[num6, num19].type == 76 || Main.tile[num6, num19].type == 19)
                            {
                                Main.tile[num6, num19].active = false;
                            }
                            Main.tile[num6, num19].wall = 0;
                        }
                    }
                }
                num15++;
            }
        }
        
        public static void HellRoom(int i, int j, int width, int height)
        {
            for (int num = i - width / 2; num <= i + width / 2; num++)
            {
                for (int num2 = j - height; num2 <= j; num2++)
                {
                    try
                    {
                        Main.tile[num, num2].active = true;
                        Main.tile[num, num2].type = 76;
                        Main.tile[num, num2].liquid = 0;
                        Main.tile[num, num2].lava = false;
                    }
                    catch
                    {
                    }
                }
            }
            for (int num = i - width / 2 + 1; num <= i + width / 2 - 1; num++)
            {
                for (int num2 = j - height + 1; num2 <= j - 1; num2++)
                {
                    try
                    {
                        Main.tile[num, num2].active = false;
                        Main.tile[num, num2].wall = 13;
                        Main.tile[num, num2].liquid = 0;
                        Main.tile[num, num2].lava = false;
                    }
                    catch
                    {
                    }
                }
            }
        }
        
        public static void MakeDungeon(int x, int y, int tileType = 41, int wallType = 7)
        {
            int num = WorldGen.genRand.Next(3);
            int num2 = WorldGen.genRand.Next(3);
            if (num == 1)
            {
                tileType = 43;
            }
            else
            {
                if (num == 2)
                {
                    tileType = 44;
                }
            }
            if (num2 == 1)
            {
                wallType = 8;
            }
            else
            {
                if (num2 == 2)
                {
                    wallType = 9;
                }
            }
            WorldGen.numDDoors = 0;
            WorldGen.numDPlats = 0;
            WorldGen.numDRooms = 0;
            WorldGen.dungeonX = x;
            WorldGen.dungeonY = y;
            WorldGen.dMinX = x;
            WorldGen.dMaxX = x;
            WorldGen.dMinY = y;
            WorldGen.dMaxY = y;
            WorldGen.dxStrength1 = (double)WorldGen.genRand.Next(25, 30);
            WorldGen.dyStrength1 = (double)WorldGen.genRand.Next(20, 25);
            WorldGen.dxStrength2 = (double)WorldGen.genRand.Next(35, 50);
            WorldGen.dyStrength2 = (double)WorldGen.genRand.Next(10, 15);
            float num3 = (float)(Main.maxTilesX / 60);
            num3 += (float)WorldGen.genRand.Next(0, (int)(num3 / 3f));
            float num4 = num3;
            int num5 = 5;
            WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            while (num3 > 0f)
            {
                if (WorldGen.dungeonX < WorldGen.dMinX)
                {
                    WorldGen.dMinX = WorldGen.dungeonX;
                }
                if (WorldGen.dungeonX > WorldGen.dMaxX)
                {
                    WorldGen.dMaxX = WorldGen.dungeonX;
                }
                if (WorldGen.dungeonY > WorldGen.dMaxY)
                {
                    WorldGen.dMaxY = WorldGen.dungeonY;
                }
                num3 -= 1f;
                Program.printData("Creating dungeon: " + (int)((num4 - num3) / num4 * 60f) + "%");
                if (num5 > 0)
                {
                    num5--;
                }
                if (num5 == 0 & WorldGen.genRand.Next(3) == 0)
                {
                    num5 = 5;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        int num6 = WorldGen.dungeonX;
                        int num7 = WorldGen.dungeonY;
                        WorldGen.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, false);
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            WorldGen.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, false);
                        }
                        WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
                        WorldGen.dungeonX = num6;
                        WorldGen.dungeonY = num7;
                    }
                    else
                    {
                        WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
                    }
                }
                else
                {
                    WorldGen.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, false);
                }
            }
            WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            int num8 = WorldGen.dRoomX[0];
            int num9 = WorldGen.dRoomY[0];
            for (int i = 0; i < WorldGen.numDRooms; i++)
            {
                if (WorldGen.dRoomY[i] < num9)
                {
                    num8 = WorldGen.dRoomX[i];
                    num9 = WorldGen.dRoomY[i];
                }
            }
            WorldGen.dungeonX = num8;
            WorldGen.dungeonY = num9;
            WorldGen.dEnteranceX = num8;
            WorldGen.dSurface = false;
            num5 = 5;
            while (!WorldGen.dSurface)
            {
                if (num5 > 0)
                {
                    num5--;
                }
                if ((num5 == 0 & WorldGen.genRand.Next(5) == 0) && (double)WorldGen.dungeonY > Main.worldSurface + 50.0)
                {
                    num5 = 10;
                    int num6 = WorldGen.dungeonX;
                    int num7 = WorldGen.dungeonY;
                    WorldGen.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, true);
                    WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
                    WorldGen.dungeonX = num6;
                    WorldGen.dungeonY = num7;
                }
                WorldGen.DungeonStairs(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            }
            WorldGen.DungeonEnt(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            Program.printData("Creating dungeon: 65%");
            for (int j = 0; j < WorldGen.numDRooms; j++)
            {
                for (int i = WorldGen.dRoomL[j]; i <= WorldGen.dRoomR[j]; i++)
                {
                    if (!Main.tile[i, WorldGen.dRoomT[j] - 1].active)
                    {
                        WorldGen.DPlatX[WorldGen.numDPlats] = i;
                        WorldGen.DPlatY[WorldGen.numDPlats] = WorldGen.dRoomT[j] - 1;
                        WorldGen.numDPlats++;
                        break;
                    }
                }
                for (int i = WorldGen.dRoomL[j]; i <= WorldGen.dRoomR[j]; i++)
                {
                    if (!Main.tile[i, WorldGen.dRoomB[j] + 1].active)
                    {
                        WorldGen.DPlatX[WorldGen.numDPlats] = i;
                        WorldGen.DPlatY[WorldGen.numDPlats] = WorldGen.dRoomB[j] + 1;
                        WorldGen.numDPlats++;
                        break;
                    }
                }
                for (int num10 = WorldGen.dRoomT[j]; num10 <= WorldGen.dRoomB[j]; num10++)
                {
                    if (!Main.tile[WorldGen.dRoomL[j] - 1, num10].active)
                    {
                        WorldGen.DDoorX[WorldGen.numDDoors] = WorldGen.dRoomL[j] - 1;
                        WorldGen.DDoorY[WorldGen.numDDoors] = num10;
                        WorldGen.DDoorPos[WorldGen.numDDoors] = -1;
                        WorldGen.numDDoors++;
                        break;
                    }
                }
                for (int num10 = WorldGen.dRoomT[j]; num10 <= WorldGen.dRoomB[j]; num10++)
                {
                    if (!Main.tile[WorldGen.dRoomR[j] + 1, num10].active)
                    {
                        WorldGen.DDoorX[WorldGen.numDDoors] = WorldGen.dRoomR[j] + 1;
                        WorldGen.DDoorY[WorldGen.numDDoors] = num10;
                        WorldGen.DDoorPos[WorldGen.numDDoors] = 1;
                        WorldGen.numDDoors++;
                        break;
                    }
                }
            }
            Program.printData("Creating dungeon: 70%");
            int num11 = 0;
            int num12 = 1000;
            int k = 0;
            while (k < Main.maxTilesX / 125)
            {
                num11++;
                int num13 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
                int l = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);
                int num14 = num13;
                if ((int)Main.tile[num13, l].wall == wallType && !Main.tile[num13, l].active)
                {
                    int num15 = 1;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num15 = -1;
                    }
                    while (!Main.tile[num13, l].active)
                    {
                        l += num15;
                    }
                    if (Main.tile[num13 - 1, l].active && Main.tile[num13 + 1, l].active && !Main.tile[num13 - 1, l - num15].active && !Main.tile[num13 + 1, l - num15].active)
                    {
                        k++;
                        int num16 = WorldGen.genRand.Next(5, 10);
                        while (Main.tile[num13 - 1, l].active && Main.tile[num13, l + num15].active && Main.tile[num13, l].active && !Main.tile[num13, l - num15].active && num16 > 0)
                        {
                            Main.tile[num13, l].type = 48;
                            if (!Main.tile[num13 - 1, l - num15].active && !Main.tile[num13 + 1, l - num15].active)
                            {
                                Main.tile[num13, l - num15].type = 48;
                                Main.tile[num13, l - num15].active = true;
                            }
                            num13--;
                            num16--;
                        }
                        num16 = WorldGen.genRand.Next(5, 10);
                        num13 = num14 + 1;
                        while (Main.tile[num13 + 1, l].active && Main.tile[num13, l + num15].active && Main.tile[num13, l].active && !Main.tile[num13, l - num15].active && num16 > 0)
                        {
                            Main.tile[num13, l].type = 48;
                            if (!Main.tile[num13 - 1, l - num15].active && !Main.tile[num13 + 1, l - num15].active)
                            {
                                Main.tile[num13, l - num15].type = 48;
                                Main.tile[num13, l - num15].active = true;
                            }
                            num13++;
                            num16--;
                        }
                    }
                }
                if (num11 > num12)
                {
                    num11 = 0;
                    k++;
                }
            }
            num11 = 0;
            num12 = 1000;
            k = 0;
            Program.printData("Creating dungeon: 75%");
            while (k < Main.maxTilesX / 125)
            {
                num11++;
                int num13 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
                int l = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);
                int num17 = l;
                if ((int)Main.tile[num13, l].wall == wallType && !Main.tile[num13, l].active)
                {
                    int num15 = 1;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num15 = -1;
                    }
                    while (num13 > 5 && num13 < Main.maxTilesX - 5 && !Main.tile[num13, l].active)
                    {
                        num13 += num15;
                    }
                    if (Main.tile[num13, l - 1].active && Main.tile[num13, l + 1].active && !Main.tile[num13 - num15, l - 1].active && !Main.tile[num13 - num15, l + 1].active)
                    {
                        k++;
                        int num16 = WorldGen.genRand.Next(5, 10);
                        while (Main.tile[num13, l - 1].active && Main.tile[num13 + num15, l].active && Main.tile[num13, l].active && !Main.tile[num13 - num15, l].active && num16 > 0)
                        {
                            Main.tile[num13, l].type = 48;
                            if (!Main.tile[num13 - num15, l - 1].active && !Main.tile[num13 - num15, l + 1].active)
                            {
                                Main.tile[num13 - num15, l].type = 48;
                                Main.tile[num13 - num15, l].active = true;
                            }
                            l--;
                            num16--;
                        }
                        num16 = WorldGen.genRand.Next(5, 10);
                        l = num17 + 1;
                        while (Main.tile[num13, l + 1].active && Main.tile[num13 + num15, l].active && Main.tile[num13, l].active && !Main.tile[num13 - num15, l].active && num16 > 0)
                        {
                            Main.tile[num13, l].type = 48;
                            if (!Main.tile[num13 - num15, l - 1].active && !Main.tile[num13 - num15, l + 1].active)
                            {
                                Main.tile[num13 - num15, l].type = 48;
                                Main.tile[num13 - num15, l].active = true;
                            }
                            l++;
                            num16--;
                        }
                    }
                }
                if (num11 > num12)
                {
                    num11 = 0;
                    k++;
                }
            }
            Program.printData("Creating dungeon: 80%");
            for (int j = 0; j < WorldGen.numDDoors; j++)
            {
                int num18 = WorldGen.DDoorX[j] - 10;
                int num19 = WorldGen.DDoorX[j] + 10;
                int num20 = 100;
                int num21 = 0;
                for (int i = num18; i < num19; i++)
                {
                    bool flag = true;
                    int num10 = WorldGen.DDoorY[j];
                    while (!Main.tile[i, num10].active)
                    {
                        num10--;
                    }
                    if (!Main.tileDungeon[(int)Main.tile[i, num10].type])
                    {
                        flag = false;
                    }
                    int num22 = num10;
                    num10 = WorldGen.DDoorY[j];
                    while (!Main.tile[i, num10].active)
                    {
                        num10++;
                    }
                    if (!Main.tileDungeon[(int)Main.tile[i, num10].type])
                    {
                        flag = false;
                    }
                    int num23 = num10;
                    if (num23 - num22 >= 3)
                    {
                        int num24 = i - 20;
                        int num25 = i + 20;
                        int num26 = num23 - 10;
                        int num27 = num23 + 10;
                        for (int m = num24; m < num25; m++)
                        {
                            for (int n = num26; n < num27; n++)
                            {
                                if (Main.tile[m, n].active && Main.tile[m, n].type == 10)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (flag)
                        {
                            for (int num28 = num23 - 3; num28 < num23; num28++)
                            {
                                for (int num29 = i - 3; num29 <= i + 3; num29++)
                                {
                                    if (Main.tile[num29, num28].active)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (flag)
                        {
                            if (num23 - num22 < 20)
                            {
                                bool flag2 = false;
                                if (WorldGen.DDoorPos[j] == 0 && num23 - num22 < num20)
                                {
                                    flag2 = true;
                                }
                                if (WorldGen.DDoorPos[j] == -1 && i > num21)
                                {
                                    flag2 = true;
                                }
                                if (WorldGen.DDoorPos[j] == 1 && (i < num21 || num21 == 0))
                                {
                                    flag2 = true;
                                }
                                if (flag2)
                                {
                                    num21 = i;
                                    num20 = num23 - num22;
                                }
                            }
                        }
                    }
                }
                if (num20 < 20)
                {
                    int i = num21;
                    int num10 = WorldGen.DDoorY[j];
                    int num30 = num10;
                    while (!Main.tile[i, num10].active)
                    {
                        Main.tile[i, num10].active = false;
                        num10++;
                    }
                    while (!Main.tile[i, num30].active)
                    {
                        num30--;
                    }
                    num10--;
                    num30++;
                    for (int num31 = num30; num31 < num10 - 2; num31++)
                    {
                        Main.tile[i, num31].active = true;
                        Main.tile[i, num31].type = (byte)tileType;
                    }
                    WorldGen.PlaceTile(i, num10, 10, true, false, -1);
                    i--;
                    int num32 = num10 - 3;
                    while (!Main.tile[i, num32].active)
                    {
                        num32--;
                    }
                    if (num10 - num32 < num10 - num30 + 5 && Main.tileDungeon[(int)Main.tile[i, num32].type])
                    {
                        for (int n = num10 - 4 - WorldGen.genRand.Next(3); n > num32; n--)
                        {
                            Main.tile[i, n].active = true;
                            Main.tile[i, n].type = (byte)tileType;
                        }
                    }
                    i += 2;
                    num32 = num10 - 3;
                    while (!Main.tile[i, num32].active)
                    {
                        num32--;
                    }
                    if (num10 - num32 < num10 - num30 + 5 && Main.tileDungeon[(int)Main.tile[i, num32].type])
                    {
                        for (int n = num10 - 4 - WorldGen.genRand.Next(3); n > num32; n--)
                        {
                            Main.tile[i, n].active = true;
                            Main.tile[i, n].type = (byte)tileType;
                        }
                    }
                    num10++;
                    i--;
                    Main.tile[i - 1, num10].active = true;
                    Main.tile[i - 1, num10].type = (byte)tileType;
                    Main.tile[i + 1, num10].active = true;
                    Main.tile[i + 1, num10].type = (byte)tileType;
                }
            }
            Program.printData("Creating dungeon: 85%");
            for (int j = 0; j < WorldGen.numDPlats; j++)
            {
                int i = WorldGen.DPlatX[j];
                int num10 = WorldGen.DPlatY[j];
                int num33 = Main.maxTilesX;
                int num34 = 10;
                for (int num35 = num10 - 5; num35 <= num10 + 5; num35++)
                {
                    int num36 = i;
                    int num37 = i;
                    bool flag3 = false;
                    if (Main.tile[num36, num35].active)
                    {
                        flag3 = true;
                    }
                    else
                    {
                        while (!Main.tile[num36, num35].active)
                        {
                            num36--;
                            if (!Main.tileDungeon[(int)Main.tile[num36, num35].type])
                            {
                                flag3 = true;
                            }
                        }
                        while (!Main.tile[num37, num35].active)
                        {
                            num37++;
                            if (!Main.tileDungeon[(int)Main.tile[num37, num35].type])
                            {
                                flag3 = true;
                            }
                        }
                    }
                    if (!flag3)
                    {
                        if (num37 - num36 <= num34)
                        {
                            bool flag4 = true;
                            int num24 = i - num34 / 2 - 2;
                            int num25 = i + num34 / 2 + 2;
                            int num26 = num35 - 5;
                            int num27 = num35 + 5;
                            for (int m = num24; m <= num25; m++)
                            {
                                for (int n = num26; n <= num27; n++)
                                {
                                    if (Main.tile[m, n].active && Main.tile[m, n].type == 19)
                                    {
                                        flag4 = false;
                                        break;
                                    }
                                }
                            }
                            for (int n = num35 + 3; n >= num35 - 5; n--)
                            {
                                if (Main.tile[i, n].active)
                                {
                                    flag4 = false;
                                    break;
                                }
                            }
                            if (flag4)
                            {
                                num33 = num35;
                                break;
                            }
                        }
                    }
                }
                if (num33 > num10 - 10 && num33 < num10 + 10)
                {
                    int num36 = i;
                    int num35 = num33;
                    int num37 = i + 1;
                    while (!Main.tile[num36, num35].active)
                    {
                        Main.tile[num36, num35].active = true;
                        Main.tile[num36, num35].type = 19;
                        num36--;
                    }
                    while (!Main.tile[num37, num35].active)
                    {
                        Main.tile[num37, num35].active = true;
                        Main.tile[num37, num35].type = 19;
                        num37++;
                    }
                }
            }
            Program.printData("Creating dungeon: 90%");
            num11 = 0;
            num12 = 1000;
            k = 0;
            while (k < Main.maxTilesX / 20)
            {
                num11++;
                int num13 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
                int l = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
                bool flag5 = true;
                if ((int)Main.tile[num13, l].wall == wallType && !Main.tile[num13, l].active)
                {
                    int num15 = 1;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num15 = -1;
                    }
                    while (flag5 && !Main.tile[num13, l].active)
                    {
                        num13 -= num15;
                        if (num13 < 5 || num13 > Main.maxTilesX - 5)
                        {
                            flag5 = false;
                        }
                        else
                        {
                            if (Main.tile[num13, l].active && !Main.tileDungeon[(int)Main.tile[num13, l].type])
                            {
                                flag5 = false;
                            }
                        }
                    }
                    if (flag5)
                    {
                        if (Main.tile[num13, l].active && Main.tileDungeon[(int)Main.tile[num13, l].type] && Main.tile[num13, l - 1].active && Main.tileDungeon[(int)Main.tile[num13, l - 1].type] && Main.tile[num13, l + 1].active && Main.tileDungeon[(int)Main.tile[num13, l + 1].type])
                        {
                            num13 += num15;
                            for (int num38 = num13 - 3; num38 <= num13 + 3; num38++)
                            {
                                for (int num39 = l - 3; num39 <= l + 3; num39++)
                                {
                                    if (Main.tile[num38, num39].active && Main.tile[num38, num39].type == 19)
                                    {
                                        flag5 = false;
                                        break;
                                    }
                                }
                            }
                            if (flag5 && (!Main.tile[num13, l - 1].active & !Main.tile[num13, l - 2].active & !Main.tile[num13, l - 3].active))
                            {
                                int num40 = num13;
                                int num14 = num13;
                                while (num40 > WorldGen.dMinX && num40 < WorldGen.dMaxX && !Main.tile[num40, l].active && !Main.tile[num40, l - 1].active && !Main.tile[num40, l + 1].active)
                                {
                                    num40 += num15;
                                }
                                num40 = Math.Abs(num13 - num40);
                                bool flag6 = false;
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    flag6 = true;
                                }
                                if (num40 > 5)
                                {
                                    for (int num41 = WorldGen.genRand.Next(1, 4); num41 > 0; num41--)
                                    {
                                        Main.tile[num13, l].active = true;
                                        Main.tile[num13, l].type = 19;
                                        if (flag6)
                                        {
                                            WorldGen.PlaceTile(num13, l - 1, 50, true, false, -1);
                                            if (WorldGen.genRand.Next(50) == 0)
                                            {
                                                if (Main.tile[num13, l - 1].type == 50)
                                                {
                                                    Main.tile[num13, l - 1].frameX = 90;
                                                }
                                            }
                                        }
                                        num13 += num15;
                                    }
                                    num11 = 0;
                                    k++;
                                    if (!flag6 && WorldGen.genRand.Next(2) == 0)
                                    {
                                        num13 = num14;
                                        l--;
                                        int num42 = WorldGen.genRand.Next(2);
                                        if (num42 == 0)
                                        {
                                            num42 = 13;
                                        }
                                        else
                                        {
                                            if (num42 == 1)
                                            {
                                                num42 = 49;
                                            }
                                        }
                                        WorldGen.PlaceTile(num13, l, num42, true, false, -1);
                                        if (Main.tile[num13, l].type == 13)
                                        {
                                            if (WorldGen.genRand.Next(2) == 0)
                                            {
                                                Main.tile[num13, l].frameX = 18;
                                            }
                                            else
                                            {
                                                Main.tile[num13, l].frameX = 36;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (num11 > num12)
                {
                    num11 = 0;
                    k++;
                }
            }
            Program.printData("Creating dungeon: 95%");
            for (int i = 0; i < WorldGen.numDRooms; i++)
            {
                int num43 = 0;
                while (num43 < 1000)
                {
                    int num44 = (int)((double)WorldGen.dRoomSize[i] * 0.4);
                    int i2 = WorldGen.dRoomX[i] + WorldGen.genRand.Next(-num44, num44 + 1);
                    int j2 = WorldGen.dRoomY[i] + WorldGen.genRand.Next(-num44, num44 + 1);
                    int num45 = 0;
                    int num46 = i;
                    if (num46 == 0)
                    {
                        num45 = 113;
                    }
                    else
                    {
                        if (num46 == 1)
                        {
                            num45 = 155;
                        }
                        else
                        {
                            if (num46 == 2)
                            {
                                num45 = 156;
                            }
                            else
                            {
                                if (num46 == 3)
                                {
                                    num45 = 157;
                                }
                                else
                                {
                                    if (num46 == 4)
                                    {
                                        num45 = 163;
                                    }
                                    else
                                    {
                                        if (num46 == 5)
                                        {
                                            num45 = 164;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (num45 == 0 && WorldGen.genRand.Next(2) == 0)
                    {
                        num43 = 1000;
                    }
                    else
                    {
                        if (WorldGen.AddBuriedChest(i2, j2, num45))
                        {
                            num43 += 1000;
                        }
                        num43++;
                    }
                }
            }
            WorldGen.dMinX -= 25;
            WorldGen.dMaxX += 25;
            WorldGen.dMinY -= 25;
            WorldGen.dMaxY += 25;
            if (WorldGen.dMinX < 0)
            {
                WorldGen.dMinX = 0;
            }
            if (WorldGen.dMaxX > Main.maxTilesX)
            {
                WorldGen.dMaxX = Main.maxTilesX;
            }
            if (WorldGen.dMinY < 0)
            {
                WorldGen.dMinY = 0;
            }
            if (WorldGen.dMaxY > Main.maxTilesY)
            {
                WorldGen.dMaxY = Main.maxTilesY;
            }
            num11 = 0;
            num12 = 1000;
            k = 0;
            while (k < Main.maxTilesX / 20)
            {
                num11++;
                int num13 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
                int num47 = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
                if ((int)Main.tile[num13, num47].wall == wallType)
                {
                    for (int l = num47; l > WorldGen.dMinY; l--)
                    {
                        if (Main.tile[num13, l - 1].active && (int)Main.tile[num13, l - 1].type == tileType)
                        {
                            bool flag7 = false;
                            for (int num38 = num13 - 15; num38 < num13 + 15; num38++)
                            {
                                for (int num39 = l - 15; num39 < l + 15; num39++)
                                {
                                    if (num38 > 0 && num38 < Main.maxTilesX && num39 > 0 && num39 < Main.maxTilesY)
                                    {
                                        if (Main.tile[num38, num39].type == 42)
                                        {
                                            flag7 = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (Main.tile[num13 - 1, l].active || Main.tile[num13 + 1, l].active || Main.tile[num13 - 1, l + 1].active || Main.tile[num13 + 1, l + 1].active || Main.tile[num13, l + 2].active)
                            {
                                flag7 = true;
                            }
                            if (!flag7)
                            {
                                WorldGen.Place1x2Top(num13, l, 42);
                                if (Main.tile[num13, l].type == 42)
                                {
                                    num11 = 0;
                                    k++;
                                }
                            }
                            break;
                        }
                    }
                }
                if (num11 > num12)
                {
                    k++;
                    num11 = 0;
                }
            }
        }
        
        public static void DungeonStairs(int i, int j, int tileType, int wallType)
        {
            Vector2 value = default(Vector2);
            double num = (double)WorldGen.genRand.Next(5, 9);
            int num2 = 1;
            Vector2 value2;
            value2.X = (float)i;
            value2.Y = (float)j;
            int k = WorldGen.genRand.Next(10, 30);
            if (i > WorldGen.dEnteranceX)
            {
                num2 = -1;
            }
            else
            {
                num2 = 1;
            }
            value.Y = -1f;
            value.X = (float)num2;
            if (WorldGen.genRand.Next(3) == 0)
            {
                value.X *= 0.5f;
            }
            else
            {
                if (WorldGen.genRand.Next(3) == 0)
                {
                    value.Y *= 2f;
                }
            }
            while (k > 0)
            {
                k--;
                int num3 = (int)((double)value2.X - num - 4.0 - (double)WorldGen.genRand.Next(6));
                int num4 = (int)((double)value2.X + num + 4.0 + (double)WorldGen.genRand.Next(6));
                int num5 = (int)((double)value2.Y - num - 4.0);
                int num6 = (int)((double)value2.Y + num + 4.0 + (double)WorldGen.genRand.Next(6));
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                int num7 = 1;
                if (value2.X > (float)(Main.maxTilesX / 2))
                {
                    num7 = -1;
                }
                int num8 = (int)(value2.X + (float)WorldGen.dxStrength1 * 0.6f * (float)num7 + (float)WorldGen.dxStrength2 * (float)num7);
                int num9 = (int)(WorldGen.dyStrength2 * 0.5);
                if ((double)value2.Y < Main.worldSurface - 5.0 && Main.tile[num8, (int)((double)value2.Y - num - 6.0 + (double)num9)].wall == 0 && Main.tile[num8, (int)((double)value2.Y - num - 7.0 + (double)num9)].wall == 0 && Main.tile[num8, (int)((double)value2.Y - num - 8.0 + (double)num9)].wall == 0)
                {
                    WorldGen.dSurface = true;
                    WorldGen.TileRunner(num8, (int)((double)value2.Y - num - 6.0 + (double)num9), (double)WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(10, 20), -1, false, 0f, -1f, false, true);
                }
                for (int l = num3; l < num4; l++)
                {
                    for (int m = num5; m < num6; m++)
                    {
                        Main.tile[l, m].liquid = 0;
                        if ((int)Main.tile[l, m].wall != wallType)
                        {
                            Main.tile[l, m].wall = 0;
                            Main.tile[l, m].active = true;
                            Main.tile[l, m].type = (byte)tileType;
                        }
                    }
                }
                for (int l = num3 + 1; l < num4 - 1; l++)
                {
                    for (int m = num5 + 1; m < num6 - 1; m++)
                    {
                        WorldGen.PlaceWall(l, m, wallType, true);
                    }
                }
                int num10 = 0;
                if (WorldGen.genRand.Next((int)num) == 0)
                {
                    num10 = WorldGen.genRand.Next(1, 3);
                }
                num3 = (int)((double)value2.X - num * 0.5 - (double)num10);
                num4 = (int)((double)value2.X + num * 0.5 + (double)num10);
                num5 = (int)((double)value2.Y - num * 0.5 - (double)num10);
                num6 = (int)((double)value2.Y + num * 0.5 + (double)num10);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                for (int l = num3; l < num4; l++)
                {
                    for (int m = num5; m < num6; m++)
                    {
                        Main.tile[l, m].active = false;
                        WorldGen.PlaceWall(l, m, wallType, true);
                    }
                }
                if (WorldGen.dSurface)
                {
                    k = 0;
                }
                value2 += value;
            }
            WorldGen.dungeonX = (int)value2.X;
            WorldGen.dungeonY = (int)value2.Y;
        }

        public static void DungeonHalls(int i, int j, int tileType, int wallType, bool forceX = false)
        {
            Vector2 b = default(Vector2);
            double num = (double)WorldGen.genRand.Next(4, 6);
            Vector2 vector = default(Vector2);
            Vector2 b2 = default(Vector2);
            int num2 = 1;
            Vector2 a;
            a.X = (float)i;
            a.Y = (float)j;
            int k = WorldGen.genRand.Next(35, 80);
            if (forceX)
            {
                k += 20;
                WorldGen.lastDungeonHall = default(Vector2);
            }
            else
            {
                if (WorldGen.genRand.Next(5) == 0)
                {
                    num *= 2.0;
                    k /= 2;
                }
            }
            bool flag = false;
            while (!flag)
            {
                if (WorldGen.genRand.Next(2) == 0)
                {
                    num2 = -1;
                }
                else
                {
                    num2 = 1;
                }
                bool flag2 = false;
                if (WorldGen.genRand.Next(2) == 0)
                {
                    flag2 = true;
                }
                if (forceX)
                {
                    flag2 = true;
                }
                if (flag2)
                {
                    vector.Y = 0f;
                    vector.X = (float)num2;
                    b2.Y = 0f;
                    b2.X = (float)(-(float)num2);
                    b.Y = 0f;
                    b.X = (float)num2;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            b.Y = -0.2f;
                        }
                        else
                        {
                            b.Y = 0.2f;
                        }
                    }
                }
                else
                {
                    num += 1.0;
                    b.Y = (float)num2;
                    b.X = 0f;
                    vector.X = 0f;
                    vector.Y = (float)num2;
                    b2.X = 0f;
                    b2.Y = (float)(-(float)num2);
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            b.X = 0.3f;
                        }
                        else
                        {
                            b.X = -0.3f;
                        }
                    }
                    else
                    {
                        k /= 2;
                    }
                }
                if (WorldGen.lastDungeonHall != b2)
                {
                    flag = true;
                }
            }
            if (!forceX)
            {
                if (a.X > (float)(WorldGen.lastMaxTilesX - 200))
                {
                    num2 = -1;
                    vector.Y = 0f;
                    vector.X = (float)num2;
                    b.Y = 0f;
                    b.X = (float)num2;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            b.Y = -0.2f;
                        }
                        else
                        {
                            b.Y = 0.2f;
                        }
                    }
                }
                else
                {
                    if (a.X < 200f)
                    {
                        num2 = 1;
                        vector.Y = 0f;
                        vector.X = (float)num2;
                        b.Y = 0f;
                        b.X = (float)num2;
                        if (WorldGen.genRand.Next(3) == 0)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                b.Y = -0.2f;
                            }
                            else
                            {
                                b.Y = 0.2f;
                            }
                        }
                    }
                    else
                    {
                        if (a.Y > (float)(WorldGen.lastMaxTilesY + 200))
                        {
                            num2 = -1;
                            num += 1.0;
                            b.Y = (float)num2;
                            b.X = 0f;
                            vector.X = 0f;
                            vector.Y = (float)num2;
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    b.X = 0.3f;
                                }
                                else
                                {
                                    b.X = -0.3f;
                                }
                            }
                        }
                        else
                        {
                            if ((double)a.Y < Main.rockLayer)
                            {
                                num2 = 1;
                                num += 1.0;
                                b.Y = (float)num2;
                                b.X = 0f;
                                vector.X = 0f;
                                vector.Y = (float)num2;
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        b.X = 0.3f;
                                    }
                                    else
                                    {
                                        b.X = -0.3f;
                                    }
                                }
                            }
                            else
                            {
                                if (a.X < (float)(Main.maxTilesX / 2) && (double)a.X > (double)Main.maxTilesX * 0.25)
                                {
                                    num2 = -1;
                                    vector.Y = 0f;
                                    vector.X = (float)num2;
                                    b.Y = 0f;
                                    b.X = (float)num2;
                                    if (WorldGen.genRand.Next(3) == 0)
                                    {
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            b.Y = -0.2f;
                                        }
                                        else
                                        {
                                            b.Y = 0.2f;
                                        }
                                    }
                                }
                                else
                                {
                                    if (a.X > (float)(Main.maxTilesX / 2) && (double)a.X < (double)Main.maxTilesX * 0.75)
                                    {
                                        num2 = 1;
                                        vector.Y = 0f;
                                        vector.X = (float)num2;
                                        b.Y = 0f;
                                        b.X = (float)num2;
                                        if (WorldGen.genRand.Next(3) == 0)
                                        {
                                            if (WorldGen.genRand.Next(2) == 0)
                                            {
                                                b.Y = -0.2f;
                                            }
                                            else
                                            {
                                                b.Y = 0.2f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (vector.Y == 0f)
            {
                WorldGen.DDoorX[WorldGen.numDDoors] = (int)a.X;
                WorldGen.DDoorY[WorldGen.numDDoors] = (int)a.Y;
                WorldGen.DDoorPos[WorldGen.numDDoors] = 0;
                WorldGen.numDDoors++;
            }
            else
            {
                WorldGen.DPlatX[WorldGen.numDPlats] = (int)a.X;
                WorldGen.DPlatY[WorldGen.numDPlats] = (int)a.Y;
                WorldGen.numDPlats++;
            }
            WorldGen.lastDungeonHall = vector;
            while (k > 0)
            {
                if (vector.X > 0f && a.X > (float)(Main.maxTilesX - 100))
                {
                    k = 0;
                }
                else
                {
                    if (vector.X < 0f && a.X < 100f)
                    {
                        k = 0;
                    }
                    else
                    {
                        if (vector.Y > 0f && a.Y > (float)(Main.maxTilesY - 100))
                        {
                            k = 0;
                        }
                        else
                        {
                            if (vector.Y < 0f && (double)a.Y < Main.rockLayer + 50.0)
                            {
                                k = 0;
                            }
                        }
                    }
                }
                k--;
                int num3 = (int)((double)a.X - num - 4.0) - WorldGen.genRand.Next(6);
                int num4 = (int)((double)a.X + num + 4.0) + WorldGen.genRand.Next(6);
                int num5 = (int)((double)a.Y - num - 4.0) - WorldGen.genRand.Next(6);
                int num6 = (int)((double)a.Y + num + 4.0) + WorldGen.genRand.Next(6);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                for (int l = num3; l < num4; l++)
                {
                    for (int m = num5; m < num6; m++)
                    {
                        Main.tile[l, m].liquid = 0;
                        if (Main.tile[l, m].wall == 0)
                        {
                            Main.tile[l, m].active = true;
                            Main.tile[l, m].type = (byte)tileType;
                        }
                    }
                }
                for (int l = num3 + 1; l < num4 - 1; l++)
                {
                    for (int m = num5 + 1; m < num6 - 1; m++)
                    {
                        WorldGen.PlaceWall(l, m, wallType, true);
                    }
                }
                int num7 = 0;
                if (b.Y == 0f && WorldGen.genRand.Next((int)num + 1) == 0)
                {
                    num7 = WorldGen.genRand.Next(1, 3);
                }
                else
                {
                    if (b.X == 0f && WorldGen.genRand.Next((int)num - 1) == 0)
                    {
                        num7 = WorldGen.genRand.Next(1, 3);
                    }
                    else
                    {
                        if (WorldGen.genRand.Next((int)num * 3) == 0)
                        {
                            num7 = WorldGen.genRand.Next(1, 3);
                        }
                    }
                }
                num3 = (int)((double)a.X - num * 0.5) - num7;
                num4 = (int)((double)a.X + num * 0.5) + num7;
                num5 = (int)((double)a.Y - num * 0.5) - num7;
                num6 = (int)((double)a.Y + num * 0.5) + num7;
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                for (int l = num3; l < num4; l++)
                {
                    for (int m = num5; m < num6; m++)
                    {
                        Main.tile[l, m].active = false;
                        Main.tile[l, m].wall = (byte)wallType;
                    }
                }
                a += b;
            }
            WorldGen.dungeonX = (int)a.X;
            WorldGen.dungeonY = (int)a.Y;
            if (vector.Y == 0f)
            {
                WorldGen.DDoorX[WorldGen.numDDoors] = (int)a.X;
                WorldGen.DDoorY[WorldGen.numDDoors] = (int)a.Y;
                WorldGen.DDoorPos[WorldGen.numDDoors] = 0;
                WorldGen.numDDoors++;
            }
            else
            {
                WorldGen.DPlatX[WorldGen.numDPlats] = (int)a.X;
                WorldGen.DPlatY[WorldGen.numDPlats] = (int)a.Y;
                WorldGen.numDPlats++;
            }
        }
        
        public static void DungeonRoom(int i, int j, int tileType, int wallType)
        {
            double num = (double)WorldGen.genRand.Next(15, 30);
            Vector2 value;
            value.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            value.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            Vector2 value2;
            value2.X = (float)i;
            value2.Y = (float)j - (float)num / 2f;
            int k = WorldGen.genRand.Next(10, 20);
            double num2 = (double)value2.X;
            double num3 = (double)value2.X;
            double num4 = (double)value2.Y;
            double num5 = (double)value2.Y;
            while (k > 0)
            {
                k--;
                int num6 = (int)((double)value2.X - num * 0.800000011920929 - 5.0);
                int num7 = (int)((double)value2.X + num * 0.800000011920929 + 5.0);
                int num8 = (int)((double)value2.Y - num * 0.800000011920929 - 5.0);
                int num9 = (int)((double)value2.Y + num * 0.800000011920929 + 5.0);
                if (num6 < 0)
                {
                    num6 = 0;
                }
                if (num7 > Main.maxTilesX)
                {
                    num7 = Main.maxTilesX;
                }
                if (num8 < 0)
                {
                    num8 = 0;
                }
                if (num9 > Main.maxTilesY)
                {
                    num9 = Main.maxTilesY;
                }
                for (int l = num6; l < num7; l++)
                {
                    for (int m = num8; m < num9; m++)
                    {
                        Main.tile[l, m].liquid = 0;
                        if (Main.tile[l, m].wall == 0)
                        {
                            Main.tile[l, m].active = true;
                            Main.tile[l, m].type = (byte)tileType;
                        }
                    }
                }
                for (int l = num6 + 1; l < num7 - 1; l++)
                {
                    for (int m = num8 + 1; m < num9 - 1; m++)
                    {
                        WorldGen.PlaceWall(l, m, wallType, true);
                    }
                }
                num6 = (int)((double)value2.X - num * 0.5);
                num7 = (int)((double)value2.X + num * 0.5);
                num8 = (int)((double)value2.Y - num * 0.5);
                num9 = (int)((double)value2.Y + num * 0.5);
                if (num6 < 0)
                {
                    num6 = 0;
                }
                if (num7 > Main.maxTilesX)
                {
                    num7 = Main.maxTilesX;
                }
                if (num8 < 0)
                {
                    num8 = 0;
                }
                if (num9 > Main.maxTilesY)
                {
                    num9 = Main.maxTilesY;
                }
                if ((double)num6 < num2)
                {
                    num2 = (double)num6;
                }
                if ((double)num7 > num3)
                {
                    num3 = (double)num7;
                }
                if ((double)num8 < num4)
                {
                    num4 = (double)num8;
                }
                if ((double)num9 > num5)
                {
                    num5 = (double)num9;
                }
                for (int l = num6; l < num7; l++)
                {
                    for (int m = num8; m < num9; m++)
                    {
                        Main.tile[l, m].active = false;
                        Main.tile[l, m].wall = (byte)wallType;
                    }
                }
                value2 += value;
                value.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                value.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                if (value.X > 1f)
                {
                    value.X = 1f;
                }
                if (value.X < -1f)
                {
                    value.X = -1f;
                }
                if (value.Y > 1f)
                {
                    value.Y = 1f;
                }
                if (value.Y < -1f)
                {
                    value.Y = -1f;
                }
            }
            WorldGen.dRoomX[WorldGen.numDRooms] = (int)value2.X;
            WorldGen.dRoomY[WorldGen.numDRooms] = (int)value2.Y;
            WorldGen.dRoomSize[WorldGen.numDRooms] = (int)num;
            WorldGen.dRoomL[WorldGen.numDRooms] = (int)num2;
            WorldGen.dRoomR[WorldGen.numDRooms] = (int)num3;
            WorldGen.dRoomT[WorldGen.numDRooms] = (int)num4;
            WorldGen.dRoomB[WorldGen.numDRooms] = (int)num5;
            WorldGen.dRoomTreasure[WorldGen.numDRooms] = false;
            WorldGen.numDRooms++;
        }
        
        public static void DungeonEnt(int i, int j, int tileType, int wallType)
        {
            double num = WorldGen.dxStrength1;
            double num2 = WorldGen.dyStrength1;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j - (float)num2 / 2f;
            WorldGen.dMinY = (int)vector.Y;
            int num3 = 1;
            if (i > Main.maxTilesX / 2)
            {
                num3 = -1;
            }
            int num4 = (int)((double)vector.X - num * 0.60000002384185791 - (double)WorldGen.genRand.Next(2, 5));
            int num5 = (int)((double)vector.X + num * 0.60000002384185791 + (double)WorldGen.genRand.Next(2, 5));
            int num6 = (int)((double)vector.Y - num2 * 0.60000002384185791 - (double)WorldGen.genRand.Next(2, 5));
            int num7 = (int)((double)vector.Y + num2 * 0.60000002384185791 + (double)WorldGen.genRand.Next(8, 16));
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6; l < num7; l++)
                {
                    Main.tile[k, l].liquid = 0;
                    if ((int)Main.tile[k, l].wall != wallType)
                    {
                        Main.tile[k, l].wall = 0;
                        if (k > num4 + 1 && k < num5 - 2 && l > num6 + 1 && l < num7 - 2)
                        {
                            WorldGen.PlaceWall(k, l, wallType, true);
                        }
                        Main.tile[k, l].active = true;
                        Main.tile[k, l].type = (byte)tileType;
                    }
                }
            }
            int num8 = num4;
            int num9 = num4 + 5 + WorldGen.genRand.Next(4);
            int num10 = num6 - 3 - WorldGen.genRand.Next(3);
            int num11 = num6;
            for (int k = num8; k < num9; k++)
            {
                for (int l = num10; l < num11; l++)
                {
                    if ((int)Main.tile[k, l].wall != wallType)
                    {
                        Main.tile[k, l].active = true;
                        Main.tile[k, l].type = (byte)tileType;
                    }
                }
            }
            num8 = num5 - 5 - WorldGen.genRand.Next(4);
            num9 = num5;
            num10 = num6 - 3 - WorldGen.genRand.Next(3);
            num11 = num6;
            for (int k = num8; k < num9; k++)
            {
                for (int l = num10; l < num11; l++)
                {
                    if ((int)Main.tile[k, l].wall != wallType)
                    {
                        Main.tile[k, l].active = true;
                        Main.tile[k, l].type = (byte)tileType;
                    }
                }
            }
            int num12 = 1 + WorldGen.genRand.Next(2);
            int num13 = 2 + WorldGen.genRand.Next(4);
            int num14 = 0;
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6 - num12; l < num6; l++)
                {
                    if ((int)Main.tile[k, l].wall != wallType)
                    {
                        Main.tile[k, l].active = true;
                        Main.tile[k, l].type = (byte)tileType;
                    }
                }
                num14++;
                if (num14 >= num13)
                {
                    k += num13;
                    num14 = 0;
                }
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num7; l < num7 + 100; l++)
                {
                    WorldGen.PlaceWall(k, l, 2, true);
                }
            }
            num4 = (int)((double)vector.X - num * 0.60000002384185791);
            num5 = (int)((double)vector.X + num * 0.60000002384185791);
            num6 = (int)((double)vector.Y - num2 * 0.60000002384185791);
            num7 = (int)((double)vector.Y + num2 * 0.60000002384185791);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6; l < num7; l++)
                {
                    WorldGen.PlaceWall(k, l, wallType, true);
                }
            }
            num4 = (int)((double)vector.X - num * 0.6 - 1.0);
            num5 = (int)((double)vector.X + num * 0.6 + 1.0);
            num6 = (int)((double)vector.Y - num2 * 0.6 - 1.0);
            num7 = (int)((double)vector.Y + num2 * 0.6 + 1.0);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6; l < num7; l++)
                {
                    Main.tile[k, l].wall = (byte)wallType;
                }
            }
            num4 = (int)((double)vector.X - num * 0.5);
            num5 = (int)((double)vector.X + num * 0.5);
            num6 = (int)((double)vector.Y - num2 * 0.5);
            num7 = (int)((double)vector.Y + num2 * 0.5);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6; l < num7; l++)
                {
                    Main.tile[k, l].active = false;
                    Main.tile[k, l].wall = (byte)wallType;
                }
            }
            WorldGen.DPlatX[WorldGen.numDPlats] = (int)vector.X;
            WorldGen.DPlatY[WorldGen.numDPlats] = num7;
            WorldGen.numDPlats++;
            vector.X += (float)num * 0.6f * (float)num3;
            vector.Y += (float)num2 * 0.5f;
            num = WorldGen.dxStrength2;
            num2 = WorldGen.dyStrength2;
            vector.X += (float)num * 0.55f * (float)num3;
            vector.Y -= (float)num2 * 0.5f;
            num4 = (int)((double)vector.X - num * 0.60000002384185791 - (double)WorldGen.genRand.Next(1, 3));
            num5 = (int)((double)vector.X + num * 0.60000002384185791 + (double)WorldGen.genRand.Next(1, 3));
            num6 = (int)((double)vector.Y - num2 * 0.60000002384185791 - (double)WorldGen.genRand.Next(1, 3));
            num7 = (int)((double)vector.Y + num2 * 0.60000002384185791 + (double)WorldGen.genRand.Next(6, 16));
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6; l < num7; l++)
                {
                    if ((int)Main.tile[k, l].wall != wallType)
                    {
                        bool flag = true;
                        if (num3 < 0)
                        {
                            if ((double)k < (double)vector.X - num * 0.5)
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            if ((double)k > (double)vector.X + num * 0.5 - 1.0)
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            Main.tile[k, l].wall = 0;
                            Main.tile[k, l].active = true;
                            Main.tile[k, l].type = (byte)tileType;
                        }
                    }
                }
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num7; l < num7 + 100; l++)
                {
                    WorldGen.PlaceWall(k, l, 2, true);
                }
            }
            num4 = (int)((double)vector.X - num * 0.5);
            num5 = (int)((double)vector.X + num * 0.5);
            num8 = num4;
            if (num3 < 0)
            {
                num8++;
            }
            num9 = num8 + 5 + WorldGen.genRand.Next(4);
            num10 = num6 - 3 - WorldGen.genRand.Next(3);
            num11 = num6;
            for (int k = num8; k < num9; k++)
            {
                for (int l = num10; l < num11; l++)
                {
                    if ((int)Main.tile[k, l].wall != wallType)
                    {
                        Main.tile[k, l].active = true;
                        Main.tile[k, l].type = (byte)tileType;
                    }
                }
            }
            num8 = num5 - 5 - WorldGen.genRand.Next(4);
            num9 = num5;
            num10 = num6 - 3 - WorldGen.genRand.Next(3);
            num11 = num6;
            for (int k = num8; k < num9; k++)
            {
                for (int l = num10; l < num11; l++)
                {
                    if ((int)Main.tile[k, l].wall != wallType)
                    {
                        Main.tile[k, l].active = true;
                        Main.tile[k, l].type = (byte)tileType;
                    }
                }
            }
            num12 = 1 + WorldGen.genRand.Next(2);
            num13 = 2 + WorldGen.genRand.Next(4);
            num14 = 0;
            if (num3 < 0)
            {
                num5++;
            }
            for (int k = num4 + 1; k < num5 - 1; k++)
            {
                for (int l = num6 - num12; l < num6; l++)
                {
                    if ((int)Main.tile[k, l].wall != wallType)
                    {
                        Main.tile[k, l].active = true;
                        Main.tile[k, l].type = (byte)tileType;
                    }
                }
                num14++;
                if (num14 >= num13)
                {
                    k += num13;
                    num14 = 0;
                }
            }
            num4 = (int)((double)vector.X - num * 0.6);
            num5 = (int)((double)vector.X + num * 0.6);
            num6 = (int)((double)vector.Y - num2 * 0.6);
            num7 = (int)((double)vector.Y + num2 * 0.6);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6; l < num7; l++)
                {
                    Main.tile[k, l].wall = 0;
                }
            }
            num4 = (int)((double)vector.X - num * 0.5);
            num5 = (int)((double)vector.X + num * 0.5);
            num6 = (int)((double)vector.Y - num2 * 0.5);
            num7 = (int)((double)vector.Y + num2 * 0.5);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6; l < num7; l++)
                {
                    Main.tile[k, l].active = false;
                    Main.tile[k, l].wall = 0;
                }
            }
            for (int k = num4; k < num5; k++)
            {
                if (!Main.tile[k, num7].active)
                {
                    Main.tile[k, num7].active = true;
                    Main.tile[k, num7].type = 19;
                }
            }
            Main.dungeonX = (int)vector.X;
            Main.dungeonY = num7;
            int num15 = NPC.NewNPC(WorldGen.dungeonX * 16 + 8, WorldGen.dungeonY * 16, 37, 0);
            Main.npc[num15].homeless = false;
            Main.npc[num15].homeTileX = Main.dungeonX;
            Main.npc[num15].homeTileY = Main.dungeonY;
            if (num3 == 1)
            {
                int num16 = 0;
                for (int m = num5; m < num5 + 25; m++)
                {
                    num16++;
                    for (int n = num7 + num16; n < num7 + 25; n++)
                    {
                        Main.tile[m, n].active = true;
                        Main.tile[m, n].type = (byte)tileType;
                    }
                }
            }
            else
            {
                int num16 = 0;
                for (int m = num4; m > num4 - 25; m--)
                {
                    num16++;
                    for (int n = num7 + num16; n < num7 + 25; n++)
                    {
                        Main.tile[m, n].active = true;
                        Main.tile[m, n].type = (byte)tileType;
                    }
                }
            }
            num12 = 1 + WorldGen.genRand.Next(2);
            num13 = 2 + WorldGen.genRand.Next(4);
            num14 = 0;
            num4 = (int)((double)vector.X - num * 0.5);
            num5 = (int)((double)vector.X + num * 0.5);
            num4 += 2;
            num5 -= 2;
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6; l < num7; l++)
                {
                    WorldGen.PlaceWall(k, l, wallType, true);
                }
                num14++;
                if (num14 >= num13)
                {
                    k += num13 * 2;
                    num14 = 0;
                }
            }
            vector.X -= (float)num * 0.6f * (float)num3;
            vector.Y += (float)num2 * 0.5f;
            num = 15.0;
            num2 = 3.0;
            vector.Y -= (float)num2 * 0.5f;
            num4 = (int)((double)vector.X - num * 0.5);
            num5 = (int)((double)vector.X + num * 0.5);
            num6 = (int)((double)vector.Y - num2 * 0.5);
            num7 = (int)((double)vector.Y + num2 * 0.5);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int k = num4; k < num5; k++)
            {
                for (int l = num6; l < num7; l++)
                {
                    Main.tile[k, l].active = false;
                }
            }
            if (num3 < 0)
            {
                vector.X -= 1f;
            }
            WorldGen.PlaceTile((int)vector.X, (int)vector.Y + 1, 10, false, false, -1);
        }
        
        public static bool AddBuriedChest(int i, int j, int contain = 0)
        {
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
            }
            bool result;
            for (int k = j; k < Main.maxTilesY; k++)
            {
                if (Main.tile[i, k].active && Main.tileSolid[(int)Main.tile[i, k].type])
                {
                    int num = k;
                    int num2 = WorldGen.PlaceChest(i - 1, num - 1, 21);
                    if (num2 >= 0)
                    {
                        int num3 = 0;
                        while (num3 == 0)
                        {
                            if (contain > 0)
                            {
                                Main.chest[num2].item[num3].SetDefaults(contain);
                                num3++;
                            }
                            else
                            {
                                int num4 = WorldGen.genRand.Next(7);
                                if (num4 == 2 && WorldGen.genRand.Next(2) == 0)
                                {
                                    num4 = WorldGen.genRand.Next(7);
                                }
                                if (num4 == 0)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(49);
                                }
                                if (num4 == 1)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(50);
                                }
                                if (num4 == 2)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(52);
                                }
                                if (num4 == 3)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(53);
                                }
                                if (num4 == 4)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(54);
                                }
                                if (num4 == 5)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(55);
                                }
                                if (num4 == 6)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(51);
                                    Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(26) + 25;
                                }
                                num3++;
                            }
                            if (WorldGen.genRand.Next(3) == 0)
                            {
                                Main.chest[num2].item[num3].SetDefaults(167);
                                num3++;
                            }
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                int num4 = WorldGen.genRand.Next(4);
                                int stack = WorldGen.genRand.Next(8) + 3;
                                if (num4 == 0)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(19);
                                }
                                if (num4 == 1)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(20);
                                }
                                if (num4 == 2)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(21);
                                }
                                if (num4 == 3)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(22);
                                }
                                Main.chest[num2].item[num3].stack = stack;
                                num3++;
                            }
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                int num4 = WorldGen.genRand.Next(2);
                                int stack = WorldGen.genRand.Next(26) + 25;
                                if (num4 == 0)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(40);
                                }
                                if (num4 == 1)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(42);
                                }
                                Main.chest[num2].item[num3].stack = stack;
                                num3++;
                            }
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                int num4 = WorldGen.genRand.Next(1);
                                int stack = WorldGen.genRand.Next(3) + 3;
                                if (num4 == 0)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(28);
                                }
                                Main.chest[num2].item[num3].stack = stack;
                                num3++;
                            }
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                int num4 = WorldGen.genRand.Next(2);
                                int stack = WorldGen.genRand.Next(11) + 10;
                                if (num4 == 0)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(8);
                                }
                                if (num4 == 1)
                                {
                                    Main.chest[num2].item[num3].SetDefaults(31);
                                }
                                Main.chest[num2].item[num3].stack = stack;
                                num3++;
                            }
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                Main.chest[num2].item[num3].SetDefaults(73);
                                Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(1, 3);
                                num3++;
                            }
                        }
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    return result;
                }
            }
            result = false;
            return result;
        }
        
        public static bool OpenDoor(int i, int j, int direction)
        {
            int num = 0;
            if (Main.tile[i, j - 1] == null)
            {
                Main.tile[i, j - 1] = new Tile();
            }
            if (Main.tile[i, j - 2] == null)
            {
                Main.tile[i, j - 2] = new Tile();
            }
            if (Main.tile[i, j + 1] == null)
            {
                Main.tile[i, j + 1] = new Tile();
            }
            if (Main.tile[i, j] == null)
            {
                Main.tile[i, j] = new Tile();
            }
            if (Main.tile[i, j - 1].frameY == 0 && Main.tile[i, j - 1].type == Main.tile[i, j].type)
            {
                num = j - 1;
            }
            else
            {
                if (Main.tile[i, j - 2].frameY == 0 && Main.tile[i, j - 2].type == Main.tile[i, j].type)
                {
                    num = j - 2;
                }
                else
                {
                    if (Main.tile[i, j + 1].frameY == 0 && Main.tile[i, j + 1].type == Main.tile[i, j].type)
                    {
                        num = j + 1;
                    }
                    else
                    {
                        num = j;
                    }
                }
            }
            int num2 = i;
            short num3 = 0;
            int num4;
            if (direction == -1)
            {
                num2 = i - 1;
                num3 = 36;
                num4 = i - 1;
            }
            else
            {
                num2 = i;
                num4 = i + 1;
            }
            bool flag = true;
            for (int k = num; k < num + 3; k++)
            {
                if (Main.tile[num4, k] == null)
                {
                    Main.tile[num4, k] = new Tile();
                }
                if (Main.tile[num4, k].active)
                {
                    if (Main.tile[num4, k].type != 3 && Main.tile[num4, k].type != 24 && Main.tile[num4, k].type != 52 && Main.tile[num4, k].type != 61 && Main.tile[num4, k].type != 62 && Main.tile[num4, k].type != 69 && Main.tile[num4, k].type != 71 && Main.tile[num4, k].type != 73 && Main.tile[num4, k].type != 74)
                    {
                        flag = false;
                        break;
                    }
                    WorldGen.KillTile(num4, k, false, false, false);
                }
            }
            if (flag)
            {
                //Main.PlaySound(8, i * 16, j * 16, 1);
                Main.tile[num2, num].active = true;
                Main.tile[num2, num].type = 11;
                Main.tile[num2, num].frameY = 0;
                Main.tile[num2, num].frameX = num3;
                if (Main.tile[num2 + 1, num] == null)
                {
                    Main.tile[num2 + 1, num] = new Tile();
                }
                Main.tile[num2 + 1, num].active = true;
                Main.tile[num2 + 1, num].type = 11;
                Main.tile[num2 + 1, num].frameY = 0;
                Main.tile[num2 + 1, num].frameX = (short)(((int)num3) + 18);
                if (Main.tile[num2, num + 1] == null)
                {
                    Main.tile[num2, num + 1] = new Tile();
                }
                Main.tile[num2, num + 1].active = true;
                Main.tile[num2, num + 1].type = 11;
                Main.tile[num2, num + 1].frameY = 18;
                Main.tile[num2, num + 1].frameX = num3;
                if (Main.tile[num2 + 1, num + 1] == null)
                {
                    Main.tile[num2 + 1, num + 1] = new Tile();
                }
                Main.tile[num2 + 1, num + 1].active = true;
                Main.tile[num2 + 1, num + 1].type = 11;
                Main.tile[num2 + 1, num + 1].frameY = 18;
                Main.tile[num2 + 1, num + 1].frameX = (short)(((int)num3) + 18);
                if (Main.tile[num2, num + 2] == null)
                {
                    Main.tile[num2, num + 2] = new Tile();
                }
                Main.tile[num2, num + 2].active = true;
                Main.tile[num2, num + 2].type = 11;
                Main.tile[num2, num + 2].frameY = 36;
                Main.tile[num2, num + 2].frameX = num3;
                if (Main.tile[num2 + 1, num + 2] == null)
                {
                    Main.tile[num2 + 1, num + 2] = new Tile();
                }
                Main.tile[num2 + 1, num + 2].active = true;
                Main.tile[num2 + 1, num + 2].type = 11;
                Main.tile[num2 + 1, num + 2].frameY = 36;
                Main.tile[num2 + 1, num + 2].frameX = (short)(((int)num3) + 18);
                for (int num5 = num2 - 1; num5 <= num2 + 2; num5++)
                {
                    for (int num6 = num - 1; num6 <= num + 2; num6++)
                    {
                        WorldGen.TileFrame(num5, num6, false, false);
                    }
                }
            }
            return flag;
        }
        
        public static void Check1x2(int x, int j, byte type)
        {
            if (!WorldGen.destroyObject)
            {
                int num = j;
                bool flag = true;
                if (Main.tile[x, num] == null)
                {
                    Main.tile[x, num] = new Tile();
                }
                if (Main.tile[x, num + 1] == null)
                {
                    Main.tile[x, num + 1] = new Tile();
                }
                if (Main.tile[x, num].frameY == 18)
                {
                    num--;
                }
                if (Main.tile[x, num] == null)
                {
                    Main.tile[x, num] = new Tile();
                }
                if (Main.tile[x, num].frameY == 0 && Main.tile[x, num + 1].frameY == 18)
                {
                    if (Main.tile[x, num].type == type && Main.tile[x, num + 1].type == type)
                    {
                        flag = false;
                    }
                }
                if (Main.tile[x, num + 2] == null)
                {
                    Main.tile[x, num + 2] = new Tile();
                }
                if (!Main.tile[x, num + 2].active || !Main.tileSolid[(int)Main.tile[x, num + 2].type])
                {
                    flag = true;
                }
                if (Main.tile[x, num + 2].type != 2 && Main.tile[x, num].type == 20)
                {
                    flag = true;
                }
                if (flag)
                {
                    WorldGen.destroyObject = true;
                    if (Main.tile[x, num].type == type)
                    {
                        WorldGen.KillTile(x, num, false, false, false);
                    }
                    if (Main.tile[x, num + 1].type == type)
                    {
                        WorldGen.KillTile(x, num + 1, false, false, false);
                    }
                    if (type == 15)
                    {
                        Item.NewItem(x * 16, num * 16, 32, 32, 34, 1, false);
                    }
                    WorldGen.destroyObject = false;
                }
            }
        }
        
        public static void CheckOnTable1x1(int x, int y, int type)
        {
            if (Main.tile[x, y + 1] != null)
            {
                if (!Main.tile[x, y + 1].active || !Main.tileTable[(int)Main.tile[x, y + 1].type])
                {
                    if (type == 78)
                    {
                        if (!Main.tile[x, y + 1].active || !Main.tileSolid[(int)Main.tile[x, y + 1].type])
                        {
                            WorldGen.KillTile(x, y, false, false, false);
                        }
                    }
                    else
                    {
                        WorldGen.KillTile(x, y, false, false, false);
                    }
                }
            }
        }
        
        public static void CheckSign(int x, int y, int type)
        {
            if (!WorldGen.destroyObject)
            {
                int num = x - 2;
                int num2 = x + 3;
                int num3 = y - 2;
                int num4 = y + 3;
                if (num >= 0)
                {
                    if (num2 <= Main.maxTilesX)
                    {
                        if (num3 >= 0)
                        {
                            if (num4 <= Main.maxTilesY)
                            {
                                bool flag = false;
                                for (int i = num; i < num2; i++)
                                {
                                    for (int j = num3; j < num4; j++)
                                    {
                                        if (Main.tile[i, j] == null)
                                        {
                                            Main.tile[i, j] = new Tile();
                                        }
                                    }
                                }
                                int k = (int)(Main.tile[x, y].frameX / 18);
                                int num5 = (int)(Main.tile[x, y].frameY / 18);
                                while (k > 1)
                                {
                                    k -= 2;
                                }
                                int num6 = x - k;
                                int num7 = y - num5;
                                int num8 = (int)(Main.tile[num6, num7].frameX / 18 / 2);
                                num = num6;
                                num2 = num6 + 2;
                                num3 = num7;
                                num4 = num7 + 2;
                                k = 0;
                                for (int i = num; i < num2; i++)
                                {
                                    num5 = 0;
                                    for (int j = num3; j < num4; j++)
                                    {
                                        if (!Main.tile[i, j].active || (int)Main.tile[i, j].type != type)
                                        {
                                            flag = true;
                                            break;
                                        }
                                        if ((int)(Main.tile[i, j].frameX / 18) != k + num8 * 2 || (int)(Main.tile[i, j].frameY / 18) != num5)
                                        {
                                            flag = true;
                                            break;
                                        }
                                        num5++;
                                    }
                                    k++;
                                }
                                if (!flag)
                                {
                                    if (Main.tile[num6, num7 + 2].active && Main.tileSolid[(int)Main.tile[num6, num7 + 2].type] && Main.tile[num6 + 1, num7 + 2].active && Main.tileSolid[(int)Main.tile[num6 + 1, num7 + 2].type])
                                    {
                                        num8 = 0;
                                    }
                                    else
                                    {
                                        if (Main.tile[num6, num7 - 1].active && Main.tileSolid[(int)Main.tile[num6, num7 - 1].type] && !Main.tileSolidTop[(int)Main.tile[num6, num7 - 1].type] && Main.tile[num6 + 1, num7 - 1].active && Main.tileSolid[(int)Main.tile[num6 + 1, num7 - 1].type] && !Main.tileSolidTop[(int)Main.tile[num6 + 1, num7 - 1].type])
                                        {
                                            num8 = 1;
                                        }
                                        else
                                        {
                                            if (Main.tile[num6 - 1, num7].active && Main.tileSolid[(int)Main.tile[num6 - 1, num7].type] && !Main.tileSolidTop[(int)Main.tile[num6 - 1, num7].type] && Main.tile[num6 - 1, num7 + 1].active && Main.tileSolid[(int)Main.tile[num6 - 1, num7 + 1].type] && !Main.tileSolidTop[(int)Main.tile[num6 - 1, num7 + 1].type])
                                            {
                                                num8 = 2;
                                            }
                                            else
                                            {
                                                if (Main.tile[num6 + 2, num7].active && Main.tileSolid[(int)Main.tile[num6 + 2, num7].type] && !Main.tileSolidTop[(int)Main.tile[num6 + 2, num7].type] && Main.tile[num6 + 2, num7 + 1].active && Main.tileSolid[(int)Main.tile[num6 + 2, num7 + 1].type] && !Main.tileSolidTop[(int)Main.tile[num6 + 2, num7 + 1].type])
                                                {
                                                    num8 = 3;
                                                }
                                                else
                                                {
                                                    flag = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (flag)
                                {
                                    WorldGen.destroyObject = true;
                                    for (int i = num; i < num2; i++)
                                    {
                                        for (int j = num3; j < num4; j++)
                                        {
                                            if ((int)Main.tile[i, j].type == type)
                                            {
                                                WorldGen.KillTile(i, j, false, false, false);
                                            }
                                        }
                                    }
                                    Sign.KillSign(num6, num7);
                                    Item.NewItem(x * 16, y * 16, 32, 32, 171, 1, false);
                                    WorldGen.destroyObject = false;
                                }
                                else
                                {
                                    int num9 = 36 * num8;
                                    for (int l = 0; l < 2; l++)
                                    {
                                        for (int m = 0; m < 2; m++)
                                        {
                                            Main.tile[num6 + l, num7 + m].active = true;
                                            Main.tile[num6 + l, num7 + m].type = (byte)type;
                                            Main.tile[num6 + l, num7 + m].frameX = (short)(num9 + 18 * l);
                                            Main.tile[num6 + l, num7 + m].frameY = (short)(18 * m);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static bool PlaceSign(int x, int y, int type)
        {
            int num = x - 2;
            int num2 = x + 3;
            int num3 = y - 2;
            int num4 = y + 3;
            bool result;
            if (num < 0)
            {
                result = false;
            }
            else
            {
                if (num2 > Main.maxTilesX)
                {
                    result = false;
                }
                else
                {
                    if (num3 < 0)
                    {
                        result = false;
                    }
                    else
                    {
                        if (num4 > Main.maxTilesY)
                        {
                            result = false;
                        }
                        else
                        {
                            for (int i = num; i < num2; i++)
                            {
                                for (int j = num3; j < num4; j++)
                                {
                                    if (Main.tile[i, j] == null)
                                    {
                                        Main.tile[i, j] = new Tile();
                                    }
                                }
                            }
                            int num5 = x;
                            int num6 = y;
                            int num7 = 0;
                            if (Main.tile[x, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type] && Main.tile[x + 1, y + 1].active && Main.tileSolid[(int)Main.tile[x + 1, y + 1].type])
                            {
                                num6--;
                                num7 = 0;
                            }
                            else
                            {
                                if (Main.tile[x, y - 1].active && Main.tileSolid[(int)Main.tile[x, y - 1].type] && !Main.tileSolidTop[(int)Main.tile[x, y - 1].type] && Main.tile[x + 1, y - 1].active && Main.tileSolid[(int)Main.tile[x + 1, y - 1].type] && !Main.tileSolidTop[(int)Main.tile[x + 1, y - 1].type])
                                {
                                    num7 = 1;
                                }
                                else
                                {
                                    if (Main.tile[x - 1, y].active && Main.tileSolid[(int)Main.tile[x - 1, y].type] && !Main.tileSolidTop[(int)Main.tile[x - 1, y].type] && Main.tile[x - 1, y + 1].active && Main.tileSolid[(int)Main.tile[x - 1, y + 1].type] && !Main.tileSolidTop[(int)Main.tile[x - 1, y + 1].type])
                                    {
                                        num7 = 2;
                                    }
                                    else
                                    {
                                        if (!Main.tile[x + 1, y].active || !Main.tileSolid[(int)Main.tile[x + 1, y].type] || Main.tileSolidTop[(int)Main.tile[x + 1, y].type] || !Main.tile[x + 1, y + 1].active || !Main.tileSolid[(int)Main.tile[x + 1, y + 1].type] || Main.tileSolidTop[(int)Main.tile[x + 1, y + 1].type])
                                        {
                                            result = false;
                                            return result;
                                        }
                                        num5--;
                                        num7 = 3;
                                    }
                                }
                            }
                            if (Main.tile[num5, num6].active || Main.tile[num5 + 1, num6].active || Main.tile[num5, num6 + 1].active || Main.tile[num5 + 1, num6 + 1].active)
                            {
                                result = false;
                            }
                            else
                            {
                                int num8 = 36 * num7;
                                for (int k = 0; k < 2; k++)
                                {
                                    for (int l = 0; l < 2; l++)
                                    {
                                        Main.tile[num5 + k, num6 + l].active = true;
                                        Main.tile[num5 + k, num6 + l].type = (byte)type;
                                        Main.tile[num5 + k, num6 + l].frameX = (short)(num8 + 18 * k);
                                        Main.tile[num5 + k, num6 + l].frameY = (short)(18 * l);
                                    }
                                }
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }
        
        public static void PlaceOnTable1x1(int x, int y, int type)
        {
            bool flag = false;
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if (!Main.tile[x, y].active && Main.tile[x, y + 1].active && Main.tileTable[(int)Main.tile[x, y + 1].type])
            {
                flag = true;
            }
            if (type == 78)
            {
                if (!Main.tile[x, y].active && Main.tile[x, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type])
                {
                    flag = true;
                }
            }
            if (flag)
            {
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameX = 0;
                Main.tile[x, y].frameY = 0;
                Main.tile[x, y].type = (byte)type;
                if (type == 50)
                {
                    Main.tile[x, y].frameX = (short)(18 * WorldGen.genRand.Next(5));
                }
            }
        }
        
        public static void Place1x2(int x, int y, int type)
        {
            short frameX = 0;
            if (type == 20)
            {
                frameX = (short)(WorldGen.genRand.Next(3) * 18);
            }
            if (Main.tile[x, y - 1] == null)
            {
                Main.tile[x, y - 1] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if (Main.tile[x, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type] && !Main.tile[x, y - 1].active)
            {
                Main.tile[x, y - 1].active = true;
                Main.tile[x, y - 1].frameY = 0;
                Main.tile[x, y - 1].frameX = frameX;
                Main.tile[x, y - 1].type = (byte)type;
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 18;
                Main.tile[x, y].frameX = frameX;
                Main.tile[x, y].type = (byte)type;
            }
        }
        
        public static void Place1x2Top(int x, int y, int type)
        {
            short frameX = 0;
            if (Main.tile[x, y - 1] == null)
            {
                Main.tile[x, y - 1] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if (Main.tile[x, y - 1].active && Main.tileSolid[(int)Main.tile[x, y - 1].type] && !Main.tileSolidTop[(int)Main.tile[x, y - 1].type] && !Main.tile[x, y + 1].active)
            {
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 0;
                Main.tile[x, y].frameX = frameX;
                Main.tile[x, y].type = (byte)type;
                Main.tile[x, y + 1].active = true;
                Main.tile[x, y + 1].frameY = 18;
                Main.tile[x, y + 1].frameX = frameX;
                Main.tile[x, y + 1].type = (byte)type;
            }
        }
        
        public static void Check1x2Top(int x, int j, byte type)
        {
            if (!WorldGen.destroyObject)
            {
                int num = j;
                bool flag = true;
                if (Main.tile[x, num] == null)
                {
                    Main.tile[x, num] = new Tile();
                }
                if (Main.tile[x, num + 1] == null)
                {
                    Main.tile[x, num + 1] = new Tile();
                }
                if (Main.tile[x, num].frameY == 18)
                {
                    num--;
                }
                if (Main.tile[x, num] == null)
                {
                    Main.tile[x, num] = new Tile();
                }
                if (Main.tile[x, num].frameY == 0 && Main.tile[x, num + 1].frameY == 18)
                {
                    if (Main.tile[x, num].type == type && Main.tile[x, num + 1].type == type)
                    {
                        flag = false;
                    }
                }
                if (Main.tile[x, num - 1] == null)
                {
                    Main.tile[x, num - 1] = new Tile();
                }
                if (!Main.tile[x, num - 1].active || !Main.tileSolid[(int)Main.tile[x, num - 1].type] || Main.tileSolidTop[(int)Main.tile[x, num - 1].type])
                {
                    flag = true;
                }
                if (flag)
                {
                    WorldGen.destroyObject = true;
                    if (Main.tile[x, num].type == type)
                    {
                        WorldGen.KillTile(x, num, false, false, false);
                    }
                    if (Main.tile[x, num + 1].type == type)
                    {
                        WorldGen.KillTile(x, num + 1, false, false, false);
                    }
                    if (type == 42)
                    {
                        Item.NewItem(x * 16, num * 16, 32, 32, 136, 1, false);
                    }
                    WorldGen.destroyObject = false;
                }
            }
        }
        
        public static void Check2x1(int i, int y, byte type)
        {
            if (!WorldGen.destroyObject)
            {
                int num = i;
                bool flag = true;
                if (Main.tile[num, y] == null)
                {
                    Main.tile[num, y] = new Tile();
                }
                if (Main.tile[num + 1, y] == null)
                {
                    Main.tile[num + 1, y] = new Tile();
                }
                if (Main.tile[num, y + 1] == null)
                {
                    Main.tile[num, y + 1] = new Tile();
                }
                if (Main.tile[num + 1, y + 1] == null)
                {
                    Main.tile[num + 1, y + 1] = new Tile();
                }
                if (Main.tile[num, y].frameX == 18)
                {
                    num--;
                }
                if (Main.tile[num, y].frameX == 0 && Main.tile[num + 1, y].frameX == 18)
                {
                    if (Main.tile[num, y].type == type && Main.tile[num + 1, y].type == type)
                    {
                        flag = false;
                    }
                }
                if (type == 29)
                {
                    if (!Main.tile[num, y + 1].active || !Main.tileTable[(int)Main.tile[num, y + 1].type])
                    {
                        flag = true;
                    }
                    if (!Main.tile[num + 1, y + 1].active || !Main.tileTable[(int)Main.tile[num + 1, y + 1].type])
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (!Main.tile[num, y + 1].active || !Main.tileSolid[(int)Main.tile[num, y + 1].type])
                    {
                        flag = true;
                    }
                    if (!Main.tile[num + 1, y + 1].active || !Main.tileSolid[(int)Main.tile[num + 1, y + 1].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    WorldGen.destroyObject = true;
                    if (Main.tile[num, y].type == type)
                    {
                        WorldGen.KillTile(num, y, false, false, false);
                    }
                    if (Main.tile[num + 1, y].type == type)
                    {
                        WorldGen.KillTile(num + 1, y, false, false, false);
                    }
                    if (type == 16)
                    {
                        Item.NewItem(num * 16, y * 16, 32, 32, 35, 1, false);
                    }
                    if (type == 18)
                    {
                        Item.NewItem(num * 16, y * 16, 32, 32, 36, 1, false);
                    }
                    if (type == 29)
                    {
                        Item.NewItem(num * 16, y * 16, 32, 32, 87, 1, false);
                        //Main.PlaySound(13, i * 16, y * 16, 1);
                    }
                    WorldGen.destroyObject = false;
                }
            }
        }
        
        public static void Place2x1(int x, int y, int type)
        {
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            if (Main.tile[x + 1, y] == null)
            {
                Main.tile[x + 1, y] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if (Main.tile[x + 1, y + 1] == null)
            {
                Main.tile[x + 1, y + 1] = new Tile();
            }
            bool flag = false;
            if (type != 29 && Main.tile[x, y + 1].active && Main.tile[x + 1, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type] && Main.tileSolid[(int)Main.tile[x + 1, y + 1].type] && !Main.tile[x, y].active && !Main.tile[x + 1, y].active)
            {
                flag = true;
            }
            else
            {
                if (type == 29 && Main.tile[x, y + 1].active && Main.tile[x + 1, y + 1].active && Main.tileTable[(int)Main.tile[x, y + 1].type] && Main.tileTable[(int)Main.tile[x + 1, y + 1].type] && !Main.tile[x, y].active && !Main.tile[x + 1, y].active)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 0;
                Main.tile[x, y].frameX = 0;
                Main.tile[x, y].type = (byte)type;
                Main.tile[x + 1, y].active = true;
                Main.tile[x + 1, y].frameY = 0;
                Main.tile[x + 1, y].frameX = 18;
                Main.tile[x + 1, y].type = (byte)type;
            }
        }
        
        public static void Check4x2(int i, int j, int type)
        {
            if (!WorldGen.destroyObject)
            {
                bool flag = false;
                int num = i;
                num += (int)(Main.tile[i, j].frameX / 18 * -1);
                if (type == 79 && Main.tile[i, j].frameX >= 72)
                {
                    num += 4;
                }
                int num2 = j + (int)(Main.tile[i, j].frameY / 18 * -1);
                for (int k = num; k < num + 4; k++)
                {
                    for (int l = num2; l < num2 + 2; l++)
                    {
                        int num3 = (k - num) * 18;
                        if (type == 79 && Main.tile[i, j].frameX >= 72)
                        {
                            num3 = (k - num + 4) * 18;
                        }
                        if (Main.tile[k, l] == null)
                        {
                            Main.tile[k, l] = new Tile();
                        }
                        if (!Main.tile[k, l].active || (int)Main.tile[k, l].type != type || (int)Main.tile[k, l].frameX != num3 || (int)Main.tile[k, l].frameY != (l - num2) * 18)
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[k, num2 + 2] == null)
                    {
                        Main.tile[k, num2 + 2] = new Tile();
                    }
                    if (!Main.tile[k, num2 + 2].active || !Main.tileSolid[(int)Main.tile[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    WorldGen.destroyObject = true;
                    for (int k = num; k < num + 4; k++)
                    {
                        for (int l = num2; l < num2 + 3; l++)
                        {
                            if ((int)Main.tile[k, l].type == type && Main.tile[k, l].active)
                            {
                                WorldGen.KillTile(k, l, false, false, false);
                            }
                        }
                    }
                    if (type == 79)
                    {
                        Item.NewItem(i * 16, j * 16, 32, 32, 224, 1, false);
                    }
                    WorldGen.destroyObject = false;
                    for (int k = num - 1; k < num + 4; k++)
                    {
                        for (int l = num2 - 1; l < num2 + 4; l++)
                        {
                            WorldGen.TileFrame(k, l, false, false);
                        }
                    }
                }
            }
        }
        
        public static void Check3x2(int i, int j, int type)
        {
            if (!WorldGen.destroyObject)
            {
                bool flag = false;
                int num = i + (int)(Main.tile[i, j].frameX / 18 * -1);
                int num2 = j + (int)(Main.tile[i, j].frameY / 18 * -1);
                for (int k = num; k < num + 3; k++)
                {
                    for (int l = num2; l < num2 + 2; l++)
                    {
                        if (Main.tile[k, l] == null)
                        {
                            Main.tile[k, l] = new Tile();
                        }
                        if (!Main.tile[k, l].active || (int)Main.tile[k, l].type != type || (int)Main.tile[k, l].frameX != (k - num) * 18 || (int)Main.tile[k, l].frameY != (l - num2) * 18)
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[k, num2 + 2] == null)
                    {
                        Main.tile[k, num2 + 2] = new Tile();
                    }
                    if (!Main.tile[k, num2 + 2].active || !Main.tileSolid[(int)Main.tile[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    WorldGen.destroyObject = true;
                    for (int k = num; k < num + 3; k++)
                    {
                        for (int l = num2; l < num2 + 3; l++)
                        {
                            if ((int)Main.tile[k, l].type == type && Main.tile[k, l].active)
                            {
                                WorldGen.KillTile(k, l, false, false, false);
                            }
                        }
                    }
                    if (type == 14)
                    {
                        Item.NewItem(i * 16, j * 16, 32, 32, 32, 1, false);
                    }
                    else
                    {
                        if (type == 17)
                        {
                            Item.NewItem(i * 16, j * 16, 32, 32, 33, 1, false);
                        }
                        else
                        {
                            if (type == 77)
                            {
                                Item.NewItem(i * 16, j * 16, 32, 32, 221, 1, false);
                            }
                        }
                    }
                    WorldGen.destroyObject = false;
                    for (int k = num - 1; k < num + 4; k++)
                    {
                        for (int l = num2 - 1; l < num2 + 4; l++)
                        {
                            WorldGen.TileFrame(k, l, false, false);
                        }
                    }
                }
            }
        }
        
        public static void Place4x2(int x, int y, int type, int direction = -1)
        {
            if (x >= 5 && x <= Main.maxTilesX - 5 && y >= 5 && y <= Main.maxTilesY - 5)
            {
                bool flag = true;
                for (int i = x - 1; i < x + 3; i++)
                {
                    for (int j = y - 1; j < y + 1; j++)
                    {
                        if (Main.tile[i, j] == null)
                        {
                            Main.tile[i, j] = new Tile();
                        }
                        if (Main.tile[i, j].active)
                        {
                            flag = false;
                        }
                    }
                    if (Main.tile[i, y + 1] == null)
                    {
                        Main.tile[i, y + 1] = new Tile();
                    }
                    if (!Main.tile[i, y + 1].active || !Main.tileSolid[(int)Main.tile[i, y + 1].type])
                    {
                        flag = false;
                    }
                }
                short num = 0;
                if (direction == 1)
                {
                    num = 72;
                }
                if (flag)
                {
                    Main.tile[x - 1, y - 1].active = true;
                    Main.tile[x - 1, y - 1].frameY = 0;
                    Main.tile[x - 1, y - 1].frameX = num;
                    Main.tile[x - 1, y - 1].type = (byte)type;
                    Main.tile[x, y - 1].active = true;
                    Main.tile[x, y - 1].frameY = 0;
                    Main.tile[x, y - 1].frameX = (short)(18 + (int)num);
                    Main.tile[x, y - 1].type = (byte)type;
                    Main.tile[x + 1, y - 1].active = true;
                    Main.tile[x + 1, y - 1].frameY = 0;
                    Main.tile[x + 1, y - 1].frameX = (short)(36 + (int)num);
                    Main.tile[x + 1, y - 1].type = (byte)type;
                    Main.tile[x + 2, y - 1].active = true;
                    Main.tile[x + 2, y - 1].frameY = 0;
                    Main.tile[x + 2, y - 1].frameX = (short)(54 + (int)num);
                    Main.tile[x + 2, y - 1].type = (byte)type;
                    Main.tile[x - 1, y].active = true;
                    Main.tile[x - 1, y].frameY = 18;
                    Main.tile[x - 1, y].frameX = num;
                    Main.tile[x - 1, y].type = (byte)type;
                    Main.tile[x, y].active = true;
                    Main.tile[x, y].frameY = 18;
                    Main.tile[x, y].frameX = (short)(18 + (int)num);
                    Main.tile[x, y].type = (byte)type;
                    Main.tile[x + 1, y].active = true;
                    Main.tile[x + 1, y].frameY = 18;
                    Main.tile[x + 1, y].frameX = (short)(36 + (int)num);
                    Main.tile[x + 1, y].type = (byte)type;
                    Main.tile[x + 2, y].active = true;
                    Main.tile[x + 2, y].frameY = 18;
                    Main.tile[x + 2, y].frameX = (short)(54 + (int)num);
                    Main.tile[x + 2, y].type = (byte)type;
                }
            }
        }
        
        public static void Place3x2(int x, int y, int type)
        {
            if (x >= 5 && x <= Main.maxTilesX - 5 && y >= 5 && y <= Main.maxTilesY - 5)
            {
                bool flag = true;
                for (int i = x - 1; i < x + 2; i++)
                {
                    for (int j = y - 1; j < y + 1; j++)
                    {
                        if (Main.tile[i, j] == null)
                        {
                            Main.tile[i, j] = new Tile();
                        }
                        if (Main.tile[i, j].active)
                        {
                            flag = false;
                        }
                    }
                    if (Main.tile[i, y + 1] == null)
                    {
                        Main.tile[i, y + 1] = new Tile();
                    }
                    if (!Main.tile[i, y + 1].active || !Main.tileSolid[(int)Main.tile[i, y + 1].type])
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    Main.tile[x - 1, y - 1].active = true;
                    Main.tile[x - 1, y - 1].frameY = 0;
                    Main.tile[x - 1, y - 1].frameX = 0;
                    Main.tile[x - 1, y - 1].type = (byte)type;
                    Main.tile[x, y - 1].active = true;
                    Main.tile[x, y - 1].frameY = 0;
                    Main.tile[x, y - 1].frameX = 18;
                    Main.tile[x, y - 1].type = (byte)type;
                    Main.tile[x + 1, y - 1].active = true;
                    Main.tile[x + 1, y - 1].frameY = 0;
                    Main.tile[x + 1, y - 1].frameX = 36;
                    Main.tile[x + 1, y - 1].type = (byte)type;
                    Main.tile[x - 1, y].active = true;
                    Main.tile[x - 1, y].frameY = 18;
                    Main.tile[x - 1, y].frameX = 0;
                    Main.tile[x - 1, y].type = (byte)type;
                    Main.tile[x, y].active = true;
                    Main.tile[x, y].frameY = 18;
                    Main.tile[x, y].frameX = 18;
                    Main.tile[x, y].type = (byte)type;
                    Main.tile[x + 1, y].active = true;
                    Main.tile[x + 1, y].frameY = 18;
                    Main.tile[x + 1, y].frameX = 36;
                    Main.tile[x + 1, y].type = (byte)type;
                }
            }
        }
        
        public static void Check3x3(int i, int j, int type)
        {
            if (!WorldGen.destroyObject)
            {
                bool flag = false;
                int num = i + (int)(Main.tile[i, j].frameX / 18 * -1);
                int num2 = j + (int)(Main.tile[i, j].frameY / 18 * -1);
                for (int k = num; k < num + 3; k++)
                {
                    for (int l = num2; l < num2 + 3; l++)
                    {
                        if (Main.tile[k, l] == null)
                        {
                            Main.tile[k, l] = new Tile();
                        }
                        if (!Main.tile[k, l].active || (int)Main.tile[k, l].type != type || (int)Main.tile[k, l].frameX != (k - num) * 18 || (int)Main.tile[k, l].frameY != (l - num2) * 18)
                        {
                            flag = true;
                        }
                    }
                }
                if (Main.tile[num + 1, num2 - 1] == null)
                {
                    Main.tile[num + 1, num2 - 1] = new Tile();
                }
                if (!Main.tile[num + 1, num2 - 1].active || !Main.tileSolid[(int)Main.tile[num + 1, num2 - 1].type] || Main.tileSolidTop[(int)Main.tile[num + 1, num2 - 1].type])
                {
                    flag = true;
                }
                if (flag)
                {
                    WorldGen.destroyObject = true;
                    for (int k = num; k < num + 3; k++)
                    {
                        for (int l = num2; l < num2 + 3; l++)
                        {
                            if ((int)Main.tile[k, l].type == type && Main.tile[k, l].active)
                            {
                                WorldGen.KillTile(k, l, false, false, false);
                            }
                        }
                    }
                    if (type == 34)
                    {
                        Item.NewItem(i * 16, j * 16, 32, 32, 106, 1, false);
                    }
                    else
                    {
                        if (type == 35)
                        {
                            Item.NewItem(i * 16, j * 16, 32, 32, 107, 1, false);
                        }
                        else
                        {
                            if (type == 36)
                            {
                                Item.NewItem(i * 16, j * 16, 32, 32, 108, 1, false);
                            }
                        }
                    }
                    WorldGen.destroyObject = false;
                    for (int k = num - 1; k < num + 4; k++)
                    {
                        for (int l = num2 - 1; l < num2 + 4; l++)
                        {
                            WorldGen.TileFrame(k, l, false, false);
                        }
                    }
                }
            }
        }
        
        public static void Place3x3(int x, int y, int type)
        {
            bool flag = true;
            for (int i = x - 1; i < x + 2; i++)
            {
                for (int j = y; j < y + 3; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                    }
                    if (Main.tile[i, j].active)
                    {
                        flag = false;
                    }
                }
            }
            if (Main.tile[x, y - 1] == null)
            {
                Main.tile[x, y - 1] = new Tile();
            }
            if (!Main.tile[x, y - 1].active || !Main.tileSolid[(int)Main.tile[x, y - 1].type] || Main.tileSolidTop[(int)Main.tile[x, y - 1].type])
            {
                flag = false;
            }
            if (flag)
            {
                Main.tile[x - 1, y].active = true;
                Main.tile[x - 1, y].frameY = 0;
                Main.tile[x - 1, y].frameX = 0;
                Main.tile[x - 1, y].type = (byte)type;
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 0;
                Main.tile[x, y].frameX = 18;
                Main.tile[x, y].type = (byte)type;
                Main.tile[x + 1, y].active = true;
                Main.tile[x + 1, y].frameY = 0;
                Main.tile[x + 1, y].frameX = 36;
                Main.tile[x + 1, y].type = (byte)type;
                Main.tile[x - 1, y + 1].active = true;
                Main.tile[x - 1, y + 1].frameY = 18;
                Main.tile[x - 1, y + 1].frameX = 0;
                Main.tile[x - 1, y + 1].type = (byte)type;
                Main.tile[x, y + 1].active = true;
                Main.tile[x, y + 1].frameY = 18;
                Main.tile[x, y + 1].frameX = 18;
                Main.tile[x, y + 1].type = (byte)type;
                Main.tile[x + 1, y + 1].active = true;
                Main.tile[x + 1, y + 1].frameY = 18;
                Main.tile[x + 1, y + 1].frameX = 36;
                Main.tile[x + 1, y + 1].type = (byte)type;
                Main.tile[x - 1, y + 2].active = true;
                Main.tile[x - 1, y + 2].frameY = 36;
                Main.tile[x - 1, y + 2].frameX = 0;
                Main.tile[x - 1, y + 2].type = (byte)type;
                Main.tile[x, y + 2].active = true;
                Main.tile[x, y + 2].frameY = 36;
                Main.tile[x, y + 2].frameX = 18;
                Main.tile[x, y + 2].type = (byte)type;
                Main.tile[x + 1, y + 2].active = true;
                Main.tile[x + 1, y + 2].frameY = 36;
                Main.tile[x + 1, y + 2].frameX = 36;
                Main.tile[x + 1, y + 2].type = (byte)type;
            }
        }
        
        public static void PlaceSunflower(int x, int y, int type = 27)
        {
            if ((double)y <= Main.worldSurface - 1.0)
            {
                bool flag = true;
                for (int i = x; i < x + 2; i++)
                {
                    for (int j = y - 3; j < y + 1; j++)
                    {
                        if (Main.tile[i, j] == null)
                        {
                            Main.tile[i, j] = new Tile();
                        }
                        if (Main.tile[i, j].active || Main.tile[i, j].wall > 0)
                        {
                            flag = false;
                        }
                    }
                    if (Main.tile[i, y + 1] == null)
                    {
                        Main.tile[i, y + 1] = new Tile();
                    }
                    if (!Main.tile[i, y + 1].active || Main.tile[i, y + 1].type != 2)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = -3; j < 1; j++)
                        {
                            int num = i * 18 + WorldGen.genRand.Next(3) * 36;
                            int num2 = (j + 3) * 18;
                            Main.tile[x + i, y + j].active = true;
                            Main.tile[x + i, y + j].frameX = (short)num;
                            Main.tile[x + i, y + j].frameY = (short)num2;
                            Main.tile[x + i, y + j].type = (byte)type;
                        }
                    }
                }
            }
        }
        
        public static void CheckSunflower(int i, int j, int type = 27)
        {
            if (!WorldGen.destroyObject)
            {
                bool flag = false;
                int k = 0;
                k += (int)(Main.tile[i, j].frameX / 18);
                int num = j + (int)(Main.tile[i, j].frameY / 18 * -1);
                while (k > 1)
                {
                    k -= 2;
                }
                k *= -1;
                k += i;
                for (int l = k; l < k + 2; l++)
                {
                    for (int m = num; m < num + 4; m++)
                    {
                        if (Main.tile[l, m] == null)
                        {
                            Main.tile[l, m] = new Tile();
                        }
                        int n;
                        for (n = (int)(Main.tile[l, m].frameX / 18); n > 1; n -= 2)
                        {
                        }
                        if (!Main.tile[l, m].active || (int)Main.tile[l, m].type != type || n != l - k || (int)Main.tile[l, m].frameY != (m - num) * 18)
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[l, num + 4] == null)
                    {
                        Main.tile[l, num + 4] = new Tile();
                    }
                    if (!Main.tile[l, num + 4].active || Main.tile[l, num + 4].type != 2)
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    WorldGen.destroyObject = true;
                    for (int l = k; l < k + 2; l++)
                    {
                        for (int m = num; m < num + 4; m++)
                        {
                            if ((int)Main.tile[l, m].type == type && Main.tile[l, m].active)
                            {
                                WorldGen.KillTile(l, m, false, false, false);
                            }
                        }
                    }
                    Item.NewItem(i * 16, j * 16, 32, 32, 63, 1, false);
                    WorldGen.destroyObject = false;
                }
            }
        }
        
        public static bool PlacePot(int x, int y, int type = 28)
        {
            bool flag = true;
            for (int i = x; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                    }
                    if (Main.tile[i, j].active)
                    {
                        flag = false;
                    }
                }
                if (Main.tile[i, y + 1] == null)
                {
                    Main.tile[i, y + 1] = new Tile();
                }
                if (!Main.tile[i, y + 1].active || !Main.tileSolid[(int)Main.tile[i, y + 1].type])
                {
                    flag = false;
                }
            }
            bool result;
            if (flag)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = -1; j < 1; j++)
                    {
                        int num = i * 18 + WorldGen.genRand.Next(3) * 36;
                        int num2 = (j + 1) * 18;
                        Main.tile[x + i, y + j].active = true;
                        Main.tile[x + i, y + j].frameX = (short)num;
                        Main.tile[x + i, y + j].frameY = (short)num2;
                        Main.tile[x + i, y + j].type = (byte)type;
                    }
                }
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        
        public static void CheckPot(int i, int j, int type = 28)
        {
            if (!WorldGen.destroyObject)
            {
                bool flag = false;
                int k = 0;
                k += (int)(Main.tile[i, j].frameX / 18);
                int num = j + (int)(Main.tile[i, j].frameY / 18 * -1);
                while (k > 1)
                {
                    k -= 2;
                }
                k *= -1;
                k += i;
                for (int l = k; l < k + 2; l++)
                {
                    for (int m = num; m < num + 2; m++)
                    {
                        if (Main.tile[l, m] == null)
                        {
                            Main.tile[l, m] = new Tile();
                        }
                        int n;
                        for (n = (int)(Main.tile[l, m].frameX / 18); n > 1; n -= 2)
                        {
                        }
                        if (!Main.tile[l, m].active || (int)Main.tile[l, m].type != type || n != l - k || (int)Main.tile[l, m].frameY != (m - num) * 18)
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[l, num + 2] == null)
                    {
                        Main.tile[l, num + 2] = new Tile();
                    }
                    if (!Main.tile[l, num + 2].active || !Main.tileSolid[(int)Main.tile[l, num + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    WorldGen.destroyObject = true;
                    //Main.PlaySound(13, i * 16, j * 16, 1);
                    for (int l = k; l < k + 2; l++)
                    {
                        for (int m = num; m < num + 2; m++)
                        {
                            if ((int)Main.tile[l, m].type == type && Main.tile[l, m].active)
                            {
                                WorldGen.KillTile(l, m, false, false, false);
                            }
                        }
                    }
                    Vector2 arg_26D_0 = new Vector2((float)(i * 16), (float)(j * 16));
                    Vector2 velocity = default(Vector2);
                    Gore.NewGore(arg_26D_0, velocity, 51);
                    Vector2 arg_28E_0 = new Vector2((float)(i * 16), (float)(j * 16));
                    velocity = default(Vector2);
                    Gore.NewGore(arg_28E_0, velocity, 52);
                    Vector2 arg_2AF_0 = new Vector2((float)(i * 16), (float)(j * 16));
                    velocity = default(Vector2);
                    Gore.NewGore(arg_2AF_0, velocity, 53);
                    int num2 = Main.rand.Next(10);
                    if (num2 == 0 && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLife < Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLifeMax)
                    {
                        Item.NewItem(i * 16, j * 16, 16, 16, 58, 1, false);
                    }
                    else
                    {
                        if (num2 == 1 && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statMana < Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statManaMax)
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, 184, 1, false);
                        }
                        else
                        {
                            if (num2 == 2)
                            {
                                int num3 = Main.rand.Next(3) + 1;
                                Item.NewItem(i * 16, j * 16, 16, 16, 8, num3, false);
                            }
                            else
                            {
                                if (num2 == 3)
                                {
                                    int num3 = Main.rand.Next(8) + 3;
                                    Item.NewItem(i * 16, j * 16, 16, 16, 40, num3, false);
                                }
                                else
                                {
                                    if (num2 == 4)
                                    {
                                        Item.NewItem(i * 16, j * 16, 16, 16, 28, 1, false);
                                    }
                                    else
                                    {
                                        if (num2 == 5)
                                        {
                                            int num3 = Main.rand.Next(4) + 1;
                                            Item.NewItem(i * 16, j * 16, 16, 16, 166, num3, false);
                                        }
                                        else
                                        {
                                            float num4 = (float)(200 + WorldGen.genRand.Next(-100, 101));
                                            num4 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
                                            if (Main.rand.Next(5) == 0)
                                            {
                                                num4 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
                                            }
                                            if (Main.rand.Next(10) == 0)
                                            {
                                                num4 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
                                            }
                                            if (Main.rand.Next(15) == 0)
                                            {
                                                num4 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
                                            }
                                            if (Main.rand.Next(20) == 0)
                                            {
                                                num4 *= 1f + (float)Main.rand.Next(40, 81) * 0.01f;
                                            }
                                            if (Main.rand.Next(25) == 0)
                                            {
                                                num4 *= 1f + (float)Main.rand.Next(50, 101) * 0.01f;
                                            }
                                            while ((int)num4 > 0)
                                            {
                                                if (num4 > 1000000f)
                                                {
                                                    int num3 = (int)(num4 / 1000000f);
                                                    if (num3 > 50 && Main.rand.Next(2) == 0)
                                                    {
                                                        num3 /= Main.rand.Next(3) + 1;
                                                    }
                                                    if (Main.rand.Next(2) == 0)
                                                    {
                                                        num3 /= Main.rand.Next(3) + 1;
                                                    }
                                                    num4 -= (float)(1000000 * num3);
                                                    Item.NewItem(i * 16, j * 16, 16, 16, 74, num3, false);
                                                }
                                                else
                                                {
                                                    if (num4 > 10000f)
                                                    {
                                                        int num3 = (int)(num4 / 10000f);
                                                        if (num3 > 50 && Main.rand.Next(2) == 0)
                                                        {
                                                            num3 /= Main.rand.Next(3) + 1;
                                                        }
                                                        if (Main.rand.Next(2) == 0)
                                                        {
                                                            num3 /= Main.rand.Next(3) + 1;
                                                        }
                                                        num4 -= (float)(10000 * num3);
                                                        Item.NewItem(i * 16, j * 16, 16, 16, 73, num3, false);
                                                    }
                                                    else
                                                    {
                                                        if (num4 > 100f)
                                                        {
                                                            int num3 = (int)(num4 / 100f);
                                                            if (num3 > 50 && Main.rand.Next(2) == 0)
                                                            {
                                                                num3 /= Main.rand.Next(3) + 1;
                                                            }
                                                            if (Main.rand.Next(2) == 0)
                                                            {
                                                                num3 /= Main.rand.Next(3) + 1;
                                                            }
                                                            num4 -= (float)(100 * num3);
                                                            Item.NewItem(i * 16, j * 16, 16, 16, 72, num3, false);
                                                        }
                                                        else
                                                        {
                                                            int num3 = (int)num4;
                                                            if (num3 > 50 && Main.rand.Next(2) == 0)
                                                            {
                                                                num3 /= Main.rand.Next(3) + 1;
                                                            }
                                                            if (Main.rand.Next(2) == 0)
                                                            {
                                                                num3 /= Main.rand.Next(4) + 1;
                                                            }
                                                            if (num3 < 1)
                                                            {
                                                                num3 = 1;
                                                            }
                                                            num4 -= (float)num3;
                                                            Item.NewItem(i * 16, j * 16, 16, 16, 71, num3, false);
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
                    WorldGen.destroyObject = false;
                }
            }
        }
        
        public static int PlaceChest(int x, int y, int type = 21)
        {
            bool flag = true;
            int num = -1;
            for (int i = x; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                    }
                    if (Main.tile[i, j].active)
                    {
                        flag = false;
                    }
                    if (Main.tile[i, j].lava)
                    {
                        flag = false;
                    }
                }
                if (Main.tile[i, y + 1] == null)
                {
                    Main.tile[i, y + 1] = new Tile();
                }
                if (!Main.tile[i, y + 1].active || !Main.tileSolid[(int)Main.tile[i, y + 1].type])
                {
                    flag = false;
                }
            }
            if (flag)
            {
                num = Chest.CreateChest(x, y - 1);
                if (num == -1)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                Main.tile[x, y - 1].active = true;
                Main.tile[x, y - 1].frameY = 0;
                Main.tile[x, y - 1].frameX = 0;
                Main.tile[x, y - 1].type = (byte)type;
                Main.tile[x + 1, y - 1].active = true;
                Main.tile[x + 1, y - 1].frameY = 0;
                Main.tile[x + 1, y - 1].frameX = 18;
                Main.tile[x + 1, y - 1].type = (byte)type;
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 18;
                Main.tile[x, y].frameX = 0;
                Main.tile[x, y].type = (byte)type;
                Main.tile[x + 1, y].active = true;
                Main.tile[x + 1, y].frameY = 18;
                Main.tile[x + 1, y].frameX = 18;
                Main.tile[x + 1, y].type = (byte)type;
            }
            return num;
        }
        
        public static void CheckChest(int i, int j, int type)
        {
            if (!WorldGen.destroyObject)
            {
                bool flag = false;
                int num = i + (int)(Main.tile[i, j].frameX / 18 * -1);
                int num2 = j + (int)(Main.tile[i, j].frameY / 18 * -1);
                for (int k = num; k < num + 2; k++)
                {
                    for (int l = num2; l < num2 + 2; l++)
                    {
                        if (Main.tile[k, l] == null)
                        {
                            Main.tile[k, l] = new Tile();
                        }
                        if (!Main.tile[k, l].active || (int)Main.tile[k, l].type != type || (int)Main.tile[k, l].frameX != (k - num) * 18 || (int)Main.tile[k, l].frameY != (l - num2) * 18)
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[k, num2 + 2] == null)
                    {
                        Main.tile[k, num2 + 2] = new Tile();
                    }
                    if (!Main.tile[k, num2 + 2].active || !Main.tileSolid[(int)Main.tile[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    WorldGen.destroyObject = true;
                    for (int k = num; k < num + 2; k++)
                    {
                        for (int l = num2; l < num2 + 3; l++)
                        {
                            if ((int)Main.tile[k, l].type == type && Main.tile[k, l].active)
                            {
                                WorldGen.KillTile(k, l, false, false, false);
                            }
                        }
                    }
                    Item.NewItem(i * 16, j * 16, 32, 32, 48, 1, false);
                    WorldGen.destroyObject = false;
                }
            }
        }
        
        public static bool PlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1)
        {
            bool flag = false;
            bool result;
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
            {
                if (Main.tile[i, j] == null)
                {
                    Main.tile[i, j] = new Tile();
                }
                if (forced || Collision.EmptyTile(i, j, false) || !Main.tileSolid[type] || (type == 23 && Main.tile[i, j].type == 0 && Main.tile[i, j].active) || (type == 2 && Main.tile[i, j].type == 0 && Main.tile[i, j].active) || (type == 60 && Main.tile[i, j].type == 59 && Main.tile[i, j].active) || (type == 70 && Main.tile[i, j].type == 59 && Main.tile[i, j].active))
                {
                    if (type == 23 && (Main.tile[i, j].type != 0 || !Main.tile[i, j].active))
                    {
                        result = false;
                        return result;
                    }
                    if (type == 2 && (Main.tile[i, j].type != 0 || !Main.tile[i, j].active))
                    {
                        result = false;
                        return result;
                    }
                    if (type == 60 && (Main.tile[i, j].type != 59 || !Main.tile[i, j].active))
                    {
                        result = false;
                        return result;
                    }
                    if (Main.tile[i, j].liquid > 0)
                    {
                        if (type == 3 || type == 4 || type == 20 || type == 24 || type == 27 || type == 32 || type == 51 || type == 69 || type == 72)
                        {
                            result = false;
                            return result;
                        }
                    }
                    Main.tile[i, j].frameY = 0;
                    Main.tile[i, j].frameX = 0;
                    if (type == 3 || type == 24)
                    {
                        if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1].active && ((Main.tile[i, j + 1].type == 2 && type == 3) || (Main.tile[i, j + 1].type == 23 && type == 24) || (Main.tile[i, j + 1].type == 78 && type == 3)))
                        {
                            if (type == 24 && WorldGen.genRand.Next(13) == 0)
                            {
                                Main.tile[i, j].active = true;
                                Main.tile[i, j].type = 32;
                                WorldGen.SquareTileFrame(i, j, true);
                            }
                            else
                            {
                                if (Main.tile[i, j + 1].type == 78)
                                {
                                    Main.tile[i, j].active = true;
                                    Main.tile[i, j].type = (byte)type;
                                    Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(2) * 18 + 108);
                                }
                                else
                                {
                                    if (Main.tile[i, j].wall == 0)
                                    {
                                        if (Main.tile[i, j + 1].wall == 0)
                                        {
                                            if (WorldGen.genRand.Next(50) == 0)
                                            {
                                                Main.tile[i, j].active = true;
                                                Main.tile[i, j].type = (byte)type;
                                                Main.tile[i, j].frameX = 144;
                                            }
                                            else
                                            {
                                                if (WorldGen.genRand.Next(35) == 0)
                                                {
                                                    Main.tile[i, j].active = true;
                                                    Main.tile[i, j].type = (byte)type;
                                                    Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(2) * 18 + 108);
                                                }
                                                else
                                                {
                                                    Main.tile[i, j].active = true;
                                                    Main.tile[i, j].type = (byte)type;
                                                    Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(6) * 18);
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
                        if (type == 61)
                        {
                            if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1].active && Main.tile[i, j + 1].type == 60)
                            {
                                if (WorldGen.genRand.Next(10) == 0 && (double)j > Main.worldSurface)
                                {
                                    Main.tile[i, j].active = true;
                                    Main.tile[i, j].type = 69;
                                    WorldGen.SquareTileFrame(i, j, true);
                                }
                                else
                                {
                                    if (WorldGen.genRand.Next(15) == 0 && (double)j > Main.worldSurface)
                                    {
                                        Main.tile[i, j].active = true;
                                        Main.tile[i, j].type = (byte)type;
                                        Main.tile[i, j].frameX = 144;
                                    }
                                    else
                                    {
                                        if (WorldGen.genRand.Next(1000) == 0 && (double)j > Main.rockLayer)
                                        {
                                            Main.tile[i, j].active = true;
                                            Main.tile[i, j].type = (byte)type;
                                            Main.tile[i, j].frameX = 162;
                                        }
                                        else
                                        {
                                            Main.tile[i, j].active = true;
                                            Main.tile[i, j].type = (byte)type;
                                            if ((double)j > Main.rockLayer)
                                            {
                                                Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(8) * 18);
                                            }
                                            else
                                            {
                                                Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(6) * 18);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (type == 71)
                            {
                                if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1].active && Main.tile[i, j + 1].type == 70)
                                {
                                    Main.tile[i, j].active = true;
                                    Main.tile[i, j].type = (byte)type;
                                    Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(5) * 18);
                                }
                            }
                            else
                            {
                                if (type == 4)
                                {
                                    if (Main.tile[i - 1, j] == null)
                                    {
                                        Main.tile[i - 1, j] = new Tile();
                                    }
                                    if (Main.tile[i + 1, j] == null)
                                    {
                                        Main.tile[i + 1, j] = new Tile();
                                    }
                                    if (Main.tile[i, j + 1] == null)
                                    {
                                        Main.tile[i, j + 1] = new Tile();
                                    }
                                    if ((Main.tile[i - 1, j].active && (Main.tileSolid[(int)Main.tile[i - 1, j].type] || (Main.tile[i - 1, j].type == 5 && Main.tile[i - 1, j - 1].type == 5 && Main.tile[i - 1, j + 1].type == 5))) || (Main.tile[i + 1, j].active && (Main.tileSolid[(int)Main.tile[i + 1, j].type] || (Main.tile[i + 1, j].type == 5 && Main.tile[i + 1, j - 1].type == 5 && Main.tile[i + 1, j + 1].type == 5))) || (Main.tile[i, j + 1].active && Main.tileSolid[(int)Main.tile[i, j + 1].type]))
                                    {
                                        Main.tile[i, j].active = true;
                                        Main.tile[i, j].type = (byte)type;
                                        WorldGen.SquareTileFrame(i, j, true);
                                    }
                                }
                                else
                                {
                                    if (type == 10)
                                    {
                                        if (Main.tile[i, j - 1] == null)
                                        {
                                            Main.tile[i, j - 1] = new Tile();
                                        }
                                        if (Main.tile[i, j - 2] == null)
                                        {
                                            Main.tile[i, j - 2] = new Tile();
                                        }
                                        if (Main.tile[i, j - 3] == null)
                                        {
                                            Main.tile[i, j - 3] = new Tile();
                                        }
                                        if (Main.tile[i, j + 1] == null)
                                        {
                                            Main.tile[i, j + 1] = new Tile();
                                        }
                                        if (Main.tile[i, j + 2] == null)
                                        {
                                            Main.tile[i, j + 2] = new Tile();
                                        }
                                        if (Main.tile[i, j + 3] == null)
                                        {
                                            Main.tile[i, j + 3] = new Tile();
                                        }
                                        if (!Main.tile[i, j - 1].active && !Main.tile[i, j - 2].active && Main.tile[i, j - 3].active && Main.tileSolid[(int)Main.tile[i, j - 3].type])
                                        {
                                            WorldGen.PlaceDoor(i, j - 1, type);
                                            WorldGen.SquareTileFrame(i, j, true);
                                        }
                                        else
                                        {
                                            if (Main.tile[i, j + 1].active || Main.tile[i, j + 2].active || !Main.tile[i, j + 3].active || !Main.tileSolid[(int)Main.tile[i, j + 3].type])
                                            {
                                                result = false;
                                                return result;
                                            }
                                            WorldGen.PlaceDoor(i, j + 1, type);
                                            WorldGen.SquareTileFrame(i, j, true);
                                        }
                                    }
                                    else
                                    {
                                        if (type == 34 || type == 35 || type == 36)
                                        {
                                            WorldGen.Place3x3(i, j, type);
                                            WorldGen.SquareTileFrame(i, j, true);
                                        }
                                        else
                                        {
                                            if (type == 13 || type == 33 || type == 49 || type == 50 || type == 78)
                                            {
                                                WorldGen.PlaceOnTable1x1(i, j, type);
                                                WorldGen.SquareTileFrame(i, j, true);
                                            }
                                            else
                                            {
                                                if (type == 14 || type == 26)
                                                {
                                                    WorldGen.Place3x2(i, j, type);
                                                    WorldGen.SquareTileFrame(i, j, true);
                                                }
                                                else
                                                {
                                                    if (type == 20)
                                                    {
                                                        if (Main.tile[i, j + 1] == null)
                                                        {
                                                            Main.tile[i, j + 1] = new Tile();
                                                        }
                                                        if (Main.tile[i, j + 1].active && Main.tile[i, j + 1].type == 2)
                                                        {
                                                            WorldGen.Place1x2(i, j, type);
                                                            WorldGen.SquareTileFrame(i, j, true);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (type == 15)
                                                        {
                                                            if (Main.tile[i, j - 1] == null)
                                                            {
                                                                Main.tile[i, j - 1] = new Tile();
                                                            }
                                                            if (Main.tile[i, j] == null)
                                                            {
                                                                Main.tile[i, j] = new Tile();
                                                            }
                                                            WorldGen.Place1x2(i, j, type);
                                                            WorldGen.SquareTileFrame(i, j, true);
                                                        }
                                                        else
                                                        {
                                                            if (type == 16 || type == 18 || type == 29)
                                                            {
                                                                WorldGen.Place2x1(i, j, type);
                                                                WorldGen.SquareTileFrame(i, j, true);
                                                            }
                                                            else
                                                            {
                                                                if (type == 17 || type == 77)
                                                                {
                                                                    WorldGen.Place3x2(i, j, type);
                                                                    WorldGen.SquareTileFrame(i, j, true);
                                                                }
                                                                else
                                                                {
                                                                    if (type == 21)
                                                                    {
                                                                        WorldGen.PlaceChest(i, j, type);
                                                                        WorldGen.SquareTileFrame(i, j, true);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (type == 27)
                                                                        {
                                                                            WorldGen.PlaceSunflower(i, j, 27);
                                                                            WorldGen.SquareTileFrame(i, j, true);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (type == 28)
                                                                            {
                                                                                WorldGen.PlacePot(i, j, 28);
                                                                                WorldGen.SquareTileFrame(i, j, true);
                                                                            }
                                                                            else
                                                                            {
                                                                                if (type == 42)
                                                                                {
                                                                                    WorldGen.Place1x2Top(i, j, type);
                                                                                    WorldGen.SquareTileFrame(i, j, true);
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (type == 55)
                                                                                    {
                                                                                        WorldGen.PlaceSign(i, j, type);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (type == 79)
                                                                                        {
                                                                                            int direction = 1;
                                                                                            if (plr > -1)
                                                                                            {
                                                                                                direction = Main.player[plr].direction;
                                                                                            }
                                                                                            WorldGen.Place4x2(i, j, type, direction);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Main.tile[i, j].active = true;
                                                                                            Main.tile[i, j].type = (byte)type;
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
                    if (Main.tile[i, j].active && !mute)
                    {
                        WorldGen.SquareTileFrame(i, j, true);
                        flag = true;
                        //Main.PlaySound(0, i * 16, j * 16, 1);
                        if (type == 22)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, 14, 0f, 0f, 0, default(Color), 1f);
                            }
                        }
                    }
                }
            }
            result = flag;
            return result;
        }
        
        public static void KillWall(int i, int j, bool fail = false)
        {
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
            {
                if (Main.tile[i, j] == null)
                {
                    Main.tile[i, j] = new Tile();
                }
                if (Main.tile[i, j].wall > 0)
                {
                    int num = WorldGen.genRand.Next(3);
                    //Main.PlaySound(0, i * 16, j * 16, 1);
                    int num2 = 10;
                    if (fail)
                    {
                        num2 = 3;
                    }
                    for (int k = 0; k < num2; k++)
                    {
                        int type = 0;
                        if (Main.tile[i, j].wall == 1 || Main.tile[i, j].wall == 5 || Main.tile[i, j].wall == 6 || Main.tile[i, j].wall == 7 || Main.tile[i, j].wall == 8 || Main.tile[i, j].wall == 9)
                        {
                            type = 1;
                        }
                        if (Main.tile[i, j].wall == 3)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                type = 14;
                            }
                            else
                            {
                                type = 1;
                            }
                        }
                        if (Main.tile[i, j].wall == 4)
                        {
                            type = 7;
                        }
                        if (Main.tile[i, j].wall == 12)
                        {
                            type = 9;
                        }
                        if (Main.tile[i, j].wall == 10)
                        {
                            type = 10;
                        }
                        if (Main.tile[i, j].wall == 11)
                        {
                            type = 11;
                        }
                        Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, type, 0f, 0f, 0, default(Color), 1f);
                    }
                    if (fail)
                    {
                        WorldGen.SquareWallFrame(i, j, true);
                    }
                    else
                    {
                        int num3 = 0;
                        if (Main.tile[i, j].wall == 1)
                        {
                            num3 = 26;
                        }
                        if (Main.tile[i, j].wall == 4)
                        {
                            num3 = 93;
                        }
                        if (Main.tile[i, j].wall == 5)
                        {
                            num3 = 130;
                        }
                        if (Main.tile[i, j].wall == 6)
                        {
                            num3 = 132;
                        }
                        if (Main.tile[i, j].wall == 7)
                        {
                            num3 = 135;
                        }
                        if (Main.tile[i, j].wall == 8)
                        {
                            num3 = 138;
                        }
                        if (Main.tile[i, j].wall == 9)
                        {
                            num3 = 140;
                        }
                        if (Main.tile[i, j].wall == 10)
                        {
                            num3 = 142;
                        }
                        if (Main.tile[i, j].wall == 11)
                        {
                            num3 = 144;
                        }
                        if (Main.tile[i, j].wall == 12)
                        {
                            num3 = 146;
                        }
                        if (num3 > 0)
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, num3, 1, false);
                        }
                        Main.tile[i, j].wall = 0;
                        WorldGen.SquareWallFrame(i, j, true);
                    }
                }
            }
        }
        
        public static void KillTile(int i, int j, bool fail = false, bool effectOnly = false, bool noItem = false)
        {
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
            {
                if (Main.tile[i, j] == null)
                {
                    Main.tile[i, j] = new Tile();
                }
                if (Main.tile[i, j].active)
                {
                    if (j >= 1 && Main.tile[i, j - 1] == null)
                    {
                        Main.tile[i, j - 1] = new Tile();
                    }
                    if (j >= 1 && Main.tile[i, j - 1].active && ((Main.tile[i, j - 1].type == 5 && Main.tile[i, j].type != 5) || (Main.tile[i, j - 1].type == 21 && Main.tile[i, j].type != 21) || ((Main.tile[i, j - 1].type == 26 && Main.tile[i, j].type != 26) || (Main.tile[i, j - 1].type == 72 && Main.tile[i, j].type != 72)) || (Main.tile[i, j - 1].type == 12 && Main.tile[i, j].type != 12)))
                    {
                        if (Main.tile[i, j - 1].type != 5)
                        {
                            return;
                        }
                        if ((Main.tile[i, j - 1].frameX != 66 || Main.tile[i, j - 1].frameY < 0 || Main.tile[i, j - 1].frameY > 44) && (Main.tile[i, j - 1].frameX != 88 || Main.tile[i, j - 1].frameY < 66 || Main.tile[i, j - 1].frameY > 110) && Main.tile[i, j - 1].frameY < 198)
                        {
                            return;
                        }
                    }
                    if (!effectOnly && !WorldGen.stopDrops)
                    {
                        if (Main.tile[i, j].type == 3)
                        {
                            //Main.PlaySound(6, i * 16, j * 16, 1);
                            if (Main.tile[i, j].frameX == 144)
                            {
                                Item.NewItem(i * 16, j * 16, 16, 16, 5, 1, false);
                            }
                        }
                        else
                        {
                            if (Main.tile[i, j].type == 24)
                            {
                                //Main.PlaySound(6, i * 16, j * 16, 1);
                                if (Main.tile[i, j].frameX == 144)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 60, 1, false);
                                }
                            }
                            else
                            {
                                if (Main.tile[i, j].type == 32 || Main.tile[i, j].type == 51 || Main.tile[i, j].type == 52 || Main.tile[i, j].type == 61 || Main.tile[i, j].type == 62 || Main.tile[i, j].type == 69 || Main.tile[i, j].type == 71 || Main.tile[i, j].type == 73 || Main.tile[i, j].type == 74)
                                {
                                    //Main.PlaySound(6, i * 16, j * 16, 1);
                                }
                                else
                                {
                                    if (Main.tile[i, j].type == 1 || Main.tile[i, j].type == 6 || Main.tile[i, j].type == 7 || Main.tile[i, j].type == 8 || Main.tile[i, j].type == 9 || Main.tile[i, j].type == 22 || Main.tile[i, j].type == 25 || Main.tile[i, j].type == 37 || Main.tile[i, j].type == 38 || Main.tile[i, j].type == 39 || Main.tile[i, j].type == 41 || Main.tile[i, j].type == 43 || Main.tile[i, j].type == 44 || Main.tile[i, j].type == 45 || Main.tile[i, j].type == 46 || Main.tile[i, j].type == 47 || Main.tile[i, j].type == 48 || Main.tile[i, j].type == 56 || Main.tile[i, j].type == 58 || Main.tile[i, j].type == 63 || Main.tile[i, j].type == 64 || Main.tile[i, j].type == 65 || Main.tile[i, j].type == 66 || Main.tile[i, j].type == 67 || Main.tile[i, j].type == 68 || Main.tile[i, j].type == 75 || Main.tile[i, j].type == 76)
                                    {
                                        //Main.PlaySound(21, i * 16, j * 16, 1);
                                    }
                                    else
                                    {
                                        //Main.PlaySound(0, i * 16, j * 16, 1);
                                    }
                                }
                            }
                        }
                    }
                    int num = 10;
                    if (fail)
                    {
                        num = 3;
                    }
                    for (int k = 0; k < num; k++)
                    {
                        int num2 = 0;
                        if (Main.tile[i, j].type == 0)
                        {
                            num2 = 0;
                        }
                        if (Main.tile[i, j].type == 1 || Main.tile[i, j].type == 16 || Main.tile[i, j].type == 17 || Main.tile[i, j].type == 38 || Main.tile[i, j].type == 39 || Main.tile[i, j].type == 41 || Main.tile[i, j].type == 43 || Main.tile[i, j].type == 44 || Main.tile[i, j].type == 48 || Main.tileStone[(int)Main.tile[i, j].type])
                        {
                            num2 = 1;
                        }
                        if (Main.tile[i, j].type == 4 || Main.tile[i, j].type == 33)
                        {
                            num2 = 6;
                        }
                        if (Main.tile[i, j].type == 5 || Main.tile[i, j].type == 10 || Main.tile[i, j].type == 11 || Main.tile[i, j].type == 14 || Main.tile[i, j].type == 15 || Main.tile[i, j].type == 19 || Main.tile[i, j].type == 21 || Main.tile[i, j].type == 30)
                        {
                            num2 = 7;
                        }
                        if (Main.tile[i, j].type == 2)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                num2 = 0;
                            }
                            else
                            {
                                num2 = 2;
                            }
                        }
                        if (Main.tile[i, j].type == 6 || Main.tile[i, j].type == 26)
                        {
                            num2 = 8;
                        }
                        if (Main.tile[i, j].type == 7 || Main.tile[i, j].type == 34 || Main.tile[i, j].type == 47)
                        {
                            num2 = 9;
                        }
                        if (Main.tile[i, j].type == 8 || Main.tile[i, j].type == 36 || Main.tile[i, j].type == 45)
                        {
                            num2 = 10;
                        }
                        if (Main.tile[i, j].type == 9 || Main.tile[i, j].type == 35 || Main.tile[i, j].type == 42 || Main.tile[i, j].type == 46)
                        {
                            num2 = 11;
                        }
                        if (Main.tile[i, j].type == 12)
                        {
                            num2 = 12;
                        }
                        if (Main.tile[i, j].type == 3 || Main.tile[i, j].type == 73)
                        {
                            num2 = 3;
                        }
                        if (Main.tile[i, j].type == 13 || Main.tile[i, j].type == 54)
                        {
                            num2 = 13;
                        }
                        if (Main.tile[i, j].type == 22)
                        {
                            num2 = 14;
                        }
                        if (Main.tile[i, j].type == 28 || Main.tile[i, j].type == 78)
                        {
                            num2 = 22;
                        }
                        if (Main.tile[i, j].type == 29)
                        {
                            num2 = 23;
                        }
                        if (Main.tile[i, j].type == 40)
                        {
                            num2 = 28;
                        }
                        if (Main.tile[i, j].type == 49)
                        {
                            num2 = 29;
                        }
                        if (Main.tile[i, j].type == 50)
                        {
                            num2 = 22;
                        }
                        if (Main.tile[i, j].type == 51)
                        {
                            num2 = 30;
                        }
                        if (Main.tile[i, j].type == 52)
                        {
                            num2 = 3;
                        }
                        if (Main.tile[i, j].type == 53)
                        {
                            num2 = 32;
                        }
                        if (Main.tile[i, j].type == 56 || Main.tile[i, j].type == 75)
                        {
                            num2 = 37;
                        }
                        if (Main.tile[i, j].type == 57)
                        {
                            num2 = 36;
                        }
                        if (Main.tile[i, j].type == 59)
                        {
                            num2 = 38;
                        }
                        if (Main.tile[i, j].type == 61 || Main.tile[i, j].type == 62 || Main.tile[i, j].type == 74)
                        {
                            num2 = 40;
                        }
                        if (Main.tile[i, j].type == 69)
                        {
                            num2 = 7;
                        }
                        if (Main.tile[i, j].type == 71 || Main.tile[i, j].type == 72)
                        {
                            num2 = 26;
                        }
                        if (Main.tile[i, j].type == 70)
                        {
                            num2 = 17;
                        }
                        if (Main.tile[i, j].type == 2)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                num2 = 38;
                            }
                            else
                            {
                                num2 = 39;
                            }
                        }
                        if (Main.tile[i, j].type == 58 || Main.tile[i, j].type == 76 || Main.tile[i, j].type == 77)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                num2 = 6;
                            }
                            else
                            {
                                num2 = 25;
                            }
                        }
                        if (Main.tile[i, j].type == 37)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                num2 = 6;
                            }
                            else
                            {
                                num2 = 23;
                            }
                        }
                        if (Main.tile[i, j].type == 32)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                num2 = 14;
                            }
                            else
                            {
                                num2 = 24;
                            }
                        }
                        if (Main.tile[i, j].type == 23 || Main.tile[i, j].type == 24)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                num2 = 14;
                            }
                            else
                            {
                                num2 = 17;
                            }
                        }
                        if (Main.tile[i, j].type == 25 || Main.tile[i, j].type == 31)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                num2 = 14;
                            }
                            else
                            {
                                num2 = 1;
                            }
                        }
                        if (Main.tile[i, j].type == 20)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                num2 = 7;
                            }
                            else
                            {
                                num2 = 2;
                            }
                        }
                        if (Main.tile[i, j].type == 27)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                num2 = 3;
                            }
                            else
                            {
                                num2 = 19;
                            }
                        }
                        if ((Main.tile[i, j].type == 34 || Main.tile[i, j].type == 35 || Main.tile[i, j].type == 36 || Main.tile[i, j].type == 42) && Main.rand.Next(2) == 0)
                        {
                            num2 = 6;
                        }
                        if (num2 >= 0)
                        {
                            Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, num2, 0f, 0f, 0, default(Color), 1f);
                        }
                    }
                    if (!effectOnly)
                    {
                        if (fail)
                        {
                            if (Main.tile[i, j].type == 2 || Main.tile[i, j].type == 23)
                            {
                                Main.tile[i, j].type = 0;
                            }
                            if (Main.tile[i, j].type == 60)
                            {
                                Main.tile[i, j].type = 59;
                            }
                            WorldGen.SquareTileFrame(i, j, true);
                        }
                        else
                        {
                            if (Main.tile[i, j].type == 21)
                            {
                                if (Main.netMode != 1)
                                {
                                    int x = i - (int)(Main.tile[i, j].frameX / 18);
                                    int y = j - (int)(Main.tile[i, j].frameY / 18);
                                    if (!Chest.DestroyChest(x, y))
                                    {
                                        return;
                                    }
                                }
                            }
                            if (!noItem && !WorldGen.stopDrops)
                            {
                                int num3 = 0;
                                if (Main.tile[i, j].type == 0 || Main.tile[i, j].type == 2)
                                {
                                    num3 = 2;
                                }
                                else
                                {
                                    if (Main.tile[i, j].type == 1)
                                    {
                                        num3 = 3;
                                    }
                                    else
                                    {
                                        if (Main.tile[i, j].type == 4)
                                        {
                                            num3 = 8;
                                        }
                                        else
                                        {
                                            if (Main.tile[i, j].type == 5)
                                            {
                                                if (Main.tile[i, j].frameX >= 22 && Main.tile[i, j].frameY >= 198)
                                                {
                                                    if (WorldGen.genRand.Next(2) == 0)
                                                    {
                                                        num3 = 27;
                                                    }
                                                    else
                                                    {
                                                        num3 = 9;
                                                    }
                                                }
                                                else
                                                {
                                                    num3 = 9;
                                                }
                                            }
                                            else
                                            {
                                                if (Main.tile[i, j].type == 6)
                                                {
                                                    num3 = 11;
                                                }
                                                else
                                                {
                                                    if (Main.tile[i, j].type == 7)
                                                    {
                                                        num3 = 12;
                                                    }
                                                    else
                                                    {
                                                        if (Main.tile[i, j].type == 8)
                                                        {
                                                            num3 = 13;
                                                        }
                                                        else
                                                        {
                                                            if (Main.tile[i, j].type == 9)
                                                            {
                                                                num3 = 14;
                                                            }
                                                            else
                                                            {
                                                                if (Main.tile[i, j].type == 13)
                                                                {
                                                                    //Main.PlaySound(13, i * 16, j * 16, 1);
                                                                    if (Main.tile[i, j].frameX == 18)
                                                                    {
                                                                        num3 = 28;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.tile[i, j].frameX == 36)
                                                                        {
                                                                            num3 = 110;
                                                                        }
                                                                        else
                                                                        {
                                                                            num3 = 31;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (Main.tile[i, j].type == 19)
                                                                    {
                                                                        num3 = 94;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.tile[i, j].type == 22)
                                                                        {
                                                                            num3 = 56;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Main.tile[i, j].type == 23)
                                                                            {
                                                                                num3 = 2;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Main.tile[i, j].type == 25)
                                                                                {
                                                                                    num3 = 61;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (Main.tile[i, j].type == 30)
                                                                                    {
                                                                                        num3 = 9;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Main.tile[i, j].type == 33)
                                                                                        {
                                                                                            num3 = 105;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Main.tile[i, j].type == 37)
                                                                                            {
                                                                                                num3 = 116;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.tile[i, j].type == 38)
                                                                                                {
                                                                                                    num3 = 129;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Main.tile[i, j].type == 39)
                                                                                                    {
                                                                                                        num3 = 131;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Main.tile[i, j].type == 40)
                                                                                                        {
                                                                                                            num3 = 133;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Main.tile[i, j].type == 41)
                                                                                                            {
                                                                                                                num3 = 134;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (Main.tile[i, j].type == 43)
                                                                                                                {
                                                                                                                    num3 = 137;
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (Main.tile[i, j].type == 44)
                                                                                                                    {
                                                                                                                        num3 = 139;
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (Main.tile[i, j].type == 45)
                                                                                                                        {
                                                                                                                            num3 = 141;
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (Main.tile[i, j].type == 46)
                                                                                                                            {
                                                                                                                                num3 = 143;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (Main.tile[i, j].type == 47)
                                                                                                                                {
                                                                                                                                    num3 = 145;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (Main.tile[i, j].type == 48)
                                                                                                                                    {
                                                                                                                                        num3 = 147;
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (Main.tile[i, j].type == 49)
                                                                                                                                        {
                                                                                                                                            num3 = 148;
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (Main.tile[i, j].type == 51)
                                                                                                                                            {
                                                                                                                                                num3 = 150;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (Main.tile[i, j].type == 53)
                                                                                                                                                {
                                                                                                                                                    num3 = 169;
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (Main.tile[i, j].type == 54)
                                                                                                                                                    {
                                                                                                                                                        //Main.PlaySound(13, i * 16, j * 16, 1);
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (Main.tile[i, j].type == 56)
                                                                                                                                                        {
                                                                                                                                                            num3 = 173;
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            if (Main.tile[i, j].type == 57)
                                                                                                                                                            {
                                                                                                                                                                num3 = 172;
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (Main.tile[i, j].type == 58)
                                                                                                                                                                {
                                                                                                                                                                    num3 = 174;
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (Main.tile[i, j].type == 60)
                                                                                                                                                                    {
                                                                                                                                                                        num3 = 176;
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        if (Main.tile[i, j].type == 70)
                                                                                                                                                                        {
                                                                                                                                                                            num3 = 176;
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            if (Main.tile[i, j].type == 75)
                                                                                                                                                                            {
                                                                                                                                                                                num3 = 192;
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (Main.tile[i, j].type == 76)
                                                                                                                                                                                {
                                                                                                                                                                                    num3 = 214;
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (Main.tile[i, j].type == 78)
                                                                                                                                                                                    {
                                                                                                                                                                                        num3 = 222;
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (Main.tile[i, j].type == 61 || Main.tile[i, j].type == 74)
                                                                                                                                                                                        {
                                                                                                                                                                                            if (Main.tile[i, j].frameX == 162)
                                                                                                                                                                                            {
                                                                                                                                                                                                num3 = 223;
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (Main.tile[i, j].frameX >= 108 && Main.tile[i, j].frameX <= 126 && (double)j > Main.rockLayer)
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (WorldGen.genRand.Next(2) == 0)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        num3 = 208;
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (WorldGen.genRand.Next(100) == 0)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        num3 = 195;
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (Main.tile[i, j].type == 59 || Main.tile[i, j].type == 60)
                                                                                                                                                                                            {
                                                                                                                                                                                                num3 = 176;
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (Main.tile[i, j].type == 71 || Main.tile[i, j].type == 72)
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (WorldGen.genRand.Next(50) == 0)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        num3 = 194;
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        num3 = 183;
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (Main.tile[i, j].type >= 63 && Main.tile[i, j].type <= 68)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        num3 = (int)(Main.tile[i, j].type - 63 + 177);
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (Main.tile[i, j].type == 50)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (Main.tile[i, j].frameX == 90)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                num3 = 165;
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else
                                                                                                                                                                                                            {
                                                                                                                                                                                                                num3 = 149;
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
                                    }
                                }
                                if (num3 > 0)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, num3, 1, false);
                                }
                            }
                            Main.tile[i, j].active = false;
                            if (Main.tileSolid[(int)Main.tile[i, j].type])
                            {
                                Main.tile[i, j].lighted = false;
                            }
                            Main.tile[i, j].frameX = -1;
                            Main.tile[i, j].frameY = -1;
                            Main.tile[i, j].frameNumber = 0;
                            Main.tile[i, j].type = 0;
                            WorldGen.SquareTileFrame(i, j, true);
                        }
                    }
                }
            }
        }
        
        public static bool PlayerLOS(int x, int y)
        {
            Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
            bool result;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    Rectangle value = new Rectangle((int)((double)Main.player[i].position.X + (double)Main.player[i].width * 0.5 - (double)NPC.sWidth * 0.6), (int)((double)Main.player[i].position.Y + (double)Main.player[i].height * 0.5 - (double)NPC.sHeight * 0.6), (int)((double)NPC.sWidth * 1.2), (int)((double)NPC.sHeight * 1.2));
                    if (rectangle.Intersects(value))
                    {
                        result = true;
                        return result;
                    }
                }
            }
            result = false;
            return result;
        }
        
        public static void UpdateWorld()
        {
            Liquid.skipCount++;
            if (Liquid.skipCount > 1)
            {
                Liquid.UpdateLiquid();
                Liquid.skipCount = 0;
            }
            float num = 4E-05f;
            float num2 = 2E-05f;
            bool flag = false;
            WorldGen.spawnDelay++;
            if (Main.invasionType > 0)
            {
                WorldGen.spawnDelay = 0;
            }
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random();
            }
            if (Main.rand == null)
            {
                Main.rand = new Random();
            }
            if (WorldGen.spawnDelay >= 20)
            {
                flag = true;
                WorldGen.spawnDelay = 0;
                for (int i = 0; i < 1000; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].homeless && Main.npc[i].townNPC)
                    {
                        WorldGen.spawnNPC = Main.npc[i].type;
                        break;
                    }
                }
            }
            int num3 = 0;
            while ((float)num3 < (float)(Main.maxTilesX * Main.maxTilesY) * num)
            {
                int i = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int num4 = WorldGen.genRand.Next(10, (int)Main.worldSurface - 1);
                int num5 = i - 1;
                int num6 = i + 2;
                int num7 = num4 - 1;
                int num8 = num4 + 2;
                if (num5 < 10)
                {
                    num5 = 10;
                }
                if (num6 > Main.maxTilesX - 10)
                {
                    num6 = Main.maxTilesX - 10;
                }
                if (num7 < 10)
                {
                    num7 = 10;
                }
                if (num8 > Main.maxTilesY - 10)
                {
                    num8 = Main.maxTilesY - 10;
                }
                if (Main.tile[i, num4] != null)
                {
                    if (Main.tile[i, num4].liquid > 32)
                    {
                        if (Main.tile[i, num4].active)
                        {
                            if (Main.tile[i, num4].type == 3 || Main.tile[i, num4].type == 20 || Main.tile[i, num4].type == 24 || Main.tile[i, num4].type == 27 || Main.tile[i, num4].type == 73)
                            {
                                WorldGen.KillTile(i, num4, false, false, false);
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendData(17, -1, -1, "", 0, (float)i, (float)num4, 0f);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Main.tile[i, num4].active)
                        {
                            if (Main.tile[i, num4].type == 78)
                            {
                                if (!Main.tile[i, num7].active)
                                {
                                    WorldGen.PlaceTile(i, num7, 3, true, false, -1);
                                    if (Main.netMode == 2 && Main.tile[i, num7].active)
                                    {
                                        NetMessage.SendTileSquare(-1, i, num7, 1);
                                    }
                                }
                            }
                            else
                            {
                                if (Main.tile[i, num4].type == 2 || Main.tile[i, num4].type == 23 || Main.tile[i, num4].type == 32)
                                {
                                    int num9 = (int)Main.tile[i, num4].type;
                                    if (!Main.tile[i, num7].active && WorldGen.genRand.Next(10) == 0 && num9 == 2)
                                    {
                                        WorldGen.PlaceTile(i, num7, 3, true, false, -1);
                                        if (Main.netMode == 2 && Main.tile[i, num7].active)
                                        {
                                            NetMessage.SendTileSquare(-1, i, num7, 1);
                                        }
                                    }
                                    if (!Main.tile[i, num7].active && WorldGen.genRand.Next(10) == 0 && num9 == 23)
                                    {
                                        WorldGen.PlaceTile(i, num7, 24, true, false, -1);
                                        if (Main.netMode == 2 && Main.tile[i, num7].active)
                                        {
                                            NetMessage.SendTileSquare(-1, i, num7, 1);
                                        }
                                    }
                                    bool flag2 = false;
                                    for (int j = num5; j < num6; j++)
                                    {
                                        for (int k = num7; k < num8; k++)
                                        {
                                            if (i != j || num4 != k)
                                            {
                                                if (Main.tile[j, k].active)
                                                {
                                                    if (num9 == 32)
                                                    {
                                                        num9 = 23;
                                                    }
                                                    if (Main.tile[j, k].type == 0 || (num9 == 23 && Main.tile[j, k].type == 2))
                                                    {
                                                        WorldGen.SpreadGrass(j, k, 0, num9, false);
                                                        if (num9 == 23)
                                                        {
                                                            WorldGen.SpreadGrass(j, k, 2, num9, false);
                                                        }
                                                        if ((int)Main.tile[j, k].type == num9)
                                                        {
                                                            WorldGen.SquareTileFrame(j, k, true);
                                                            flag2 = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (Main.netMode == 2 && flag2)
                                    {
                                        NetMessage.SendTileSquare(-1, i, num4, 3);
                                    }
                                }
                                else
                                {
                                    if (Main.tile[i, num4].type == 20)
                                    {
                                        if (!WorldGen.PlayerLOS(i, num4) && WorldGen.genRand.Next(10) == 0)
                                        {
                                            WorldGen.GrowTree(i, num4);
                                        }
                                    }
                                }
                            }
                            if (Main.tile[i, num4].type == 3 && WorldGen.genRand.Next(10) == 0)
                            {
                                if (Main.tile[i, num4].frameX < 144)
                                {
                                    Main.tile[i, num4].type = 73;
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendTileSquare(-1, i, num4, 3);
                                    }
                                }
                            }
                            if (Main.tile[i, num4].type == 32 && WorldGen.genRand.Next(3) == 0)
                            {
                                int num10 = i;
                                int l = num4;
                                int num11 = 0;
                                if (Main.tile[num10 + 1, l].active && Main.tile[num10 + 1, l].type == 32)
                                {
                                    num11++;
                                }
                                if (Main.tile[num10 - 1, l].active && Main.tile[num10 - 1, l].type == 32)
                                {
                                    num11++;
                                }
                                if (Main.tile[num10, l + 1].active && Main.tile[num10, l + 1].type == 32)
                                {
                                    num11++;
                                }
                                if (Main.tile[num10, l - 1].active && Main.tile[num10, l - 1].type == 32)
                                {
                                    num11++;
                                }
                                if (num11 < 3 || Main.tile[i, num4].type == 23)
                                {
                                    int num12 = WorldGen.genRand.Next(4);
                                    if (num12 == 0)
                                    {
                                        l--;
                                    }
                                    else
                                    {
                                        if (num12 == 1)
                                        {
                                            l++;
                                        }
                                        else
                                        {
                                            if (num12 == 2)
                                            {
                                                num10--;
                                            }
                                            else
                                            {
                                                if (num12 == 3)
                                                {
                                                    num10++;
                                                }
                                            }
                                        }
                                    }
                                    if (!Main.tile[num10, l].active)
                                    {
                                        num11 = 0;
                                        if (Main.tile[num10 + 1, l].active && Main.tile[num10 + 1, l].type == 32)
                                        {
                                            num11++;
                                        }
                                        if (Main.tile[num10 - 1, l].active && Main.tile[num10 - 1, l].type == 32)
                                        {
                                            num11++;
                                        }
                                        if (Main.tile[num10, l + 1].active && Main.tile[num10, l + 1].type == 32)
                                        {
                                            num11++;
                                        }
                                        if (Main.tile[num10, l - 1].active && Main.tile[num10, l - 1].type == 32)
                                        {
                                            num11++;
                                        }
                                        if (num11 < 2)
                                        {
                                            int num13 = 7;
                                            int num14 = num10 - num13;
                                            int num15 = num10 + num13;
                                            int num16 = l - num13;
                                            int num17 = l + num13;
                                            bool flag3 = false;
                                            for (int m = num14; m < num15; m++)
                                            {
                                                for (int n = num16; n < num17; n++)
                                                {
                                                    if (Math.Abs(m - num10) * 2 + Math.Abs(n - l) < 9)
                                                    {
                                                        if (Main.tile[m, n].active && Main.tile[m, n].type == 23)
                                                        {
                                                            if (Main.tile[m, n - 1].active && Main.tile[m, n - 1].type == 32 && Main.tile[m, n - 1].liquid == 0)
                                                            {
                                                                flag3 = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (flag3)
                                            {
                                                Main.tile[num10, l].type = 32;
                                                Main.tile[num10, l].active = true;
                                                WorldGen.SquareTileFrame(num10, l, true);
                                                if (Main.netMode == 2)
                                                {
                                                    NetMessage.SendTileSquare(-1, num10, l, 3);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (Main.tile[i, num4].type == 2 || Main.tile[i, num4].type == 52)
                            {
                                if (WorldGen.genRand.Next(5) == 0)
                                {
                                    if (!Main.tile[i, num4 + 1].active && !Main.tile[i, num4 + 1].lava)
                                    {
                                        bool flag3 = false;
                                        for (int l = num4; l > num4 - 10; l--)
                                        {
                                            if (Main.tile[i, l].active && Main.tile[i, l].type == 2)
                                            {
                                                flag3 = true;
                                                break;
                                            }
                                        }
                                        if (flag3)
                                        {
                                            int num10 = i;
                                            int l = num4 + 1;
                                            Main.tile[num10, l].type = 52;
                                            Main.tile[num10, l].active = true;
                                            WorldGen.SquareTileFrame(num10, l, true);
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendTileSquare(-1, num10, l, 3);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (flag && WorldGen.spawnNPC > 0)
                            {
                                WorldGen.SpawnNPC(i, num4);
                            }
                        }
                    }
                    if (Main.tile[i, num4].active && Main.tile[i, num4].type == 60)
                    {
                        int num9 = (int)Main.tile[i, num4].type;
                        if (!Main.tile[i, num7].active && WorldGen.genRand.Next(7) == 0)
                        {
                            WorldGen.PlaceTile(i, num7, 61, true, false, -1);
                            if (Main.netMode == 2 && Main.tile[i, num7].active)
                            {
                                NetMessage.SendTileSquare(-1, i, num7, 1);
                            }
                        }
                        else
                        {
                            if (WorldGen.genRand.Next(50) == 0 && (!Main.tile[i, num7].active || Main.tile[i, num7].type == 61 || Main.tile[i, num7].type == 74 || Main.tile[i, num7].type == 69))
                            {
                                if (!WorldGen.PlayerLOS(i, num4))
                                {
                                    WorldGen.GrowTree(i, num4);
                                }
                            }
                        }
                        bool flag2 = false;
                        for (int j = num5; j < num6; j++)
                        {
                            for (int k = num7; k < num8; k++)
                            {
                                if (i != j || num4 != k)
                                {
                                    if (Main.tile[j, k].active)
                                    {
                                        if (Main.tile[j, k].type == 59)
                                        {
                                            WorldGen.SpreadGrass(j, k, 59, num9, false);
                                            if ((int)Main.tile[j, k].type == num9)
                                            {
                                                WorldGen.SquareTileFrame(j, k, true);
                                                flag2 = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Main.netMode == 2 && flag2)
                        {
                            NetMessage.SendTileSquare(-1, i, num4, 3);
                        }
                    }
                    if (Main.tile[i, num4].type == 61 && WorldGen.genRand.Next(3) == 0)
                    {
                        if (Main.tile[i, num4].frameX < 144)
                        {
                            Main.tile[i, num4].type = 74;
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, i, num4, 3);
                            }
                        }
                    }
                    if (Main.tile[i, num4].type == 60 || Main.tile[i, num4].type == 62)
                    {
                        if (WorldGen.genRand.Next(5) == 0)
                        {
                            if (!Main.tile[i, num4 + 1].active && !Main.tile[i, num4 + 1].lava)
                            {
                                bool flag3 = false;
                                for (int l = num4; l > num4 - 10; l--)
                                {
                                    if (Main.tile[i, l].active && Main.tile[i, l].type == 60)
                                    {
                                        flag3 = true;
                                        break;
                                    }
                                }
                                if (flag3)
                                {
                                    int num10 = i;
                                    int l = num4 + 1;
                                    Main.tile[num10, l].type = 62;
                                    Main.tile[num10, l].active = true;
                                    WorldGen.SquareTileFrame(num10, l, true);
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendTileSquare(-1, num10, l, 3);
                                    }
                                }
                            }
                        }
                    }
                }
                num3++;
            }
            num3 = 0;
            while ((float)num3 < (float)(Main.maxTilesX * Main.maxTilesY) * num2)
            {
                int i = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int num4 = WorldGen.genRand.Next((int)Main.worldSurface + 2, Main.maxTilesY - 200);
                int num5 = i - 1;
                int num6 = i + 2;
                int num7 = num4 - 1;
                int num8 = num4 + 2;
                if (num5 < 10)
                {
                    num5 = 10;
                }
                if (num6 > Main.maxTilesX - 10)
                {
                    num6 = Main.maxTilesX - 10;
                }
                if (num7 < 10)
                {
                    num7 = 10;
                }
                if (num8 > Main.maxTilesY - 10)
                {
                    num8 = Main.maxTilesY - 10;
                }
                if (Main.tile[i, num4] != null)
                {
                    if (Main.tile[i, num4].liquid <= 32)
                    {
                        if (Main.tile[i, num4].active)
                        {
                            if (Main.tile[i, num4].type == 60)
                            {
                                int num9 = (int)Main.tile[i, num4].type;
                                if (!Main.tile[i, num7].active && WorldGen.genRand.Next(10) == 0)
                                {
                                    WorldGen.PlaceTile(i, num7, 61, true, false, -1);
                                    if (Main.netMode == 2 && Main.tile[i, num7].active)
                                    {
                                        NetMessage.SendTileSquare(-1, i, num7, 1);
                                    }
                                }
                                bool flag2 = false;
                                for (int j = num5; j < num6; j++)
                                {
                                    for (int k = num7; k < num8; k++)
                                    {
                                        if (i != j || num4 != k)
                                        {
                                            if (Main.tile[j, k].active)
                                            {
                                                if (Main.tile[j, k].type == 59)
                                                {
                                                    WorldGen.SpreadGrass(j, k, 59, num9, false);
                                                    if ((int)Main.tile[j, k].type == num9)
                                                    {
                                                        WorldGen.SquareTileFrame(j, k, true);
                                                        flag2 = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (Main.netMode == 2 && flag2)
                                {
                                    NetMessage.SendTileSquare(-1, i, num4, 3);
                                }
                            }
                            if (Main.tile[i, num4].type == 61 && WorldGen.genRand.Next(3) == 0)
                            {
                                if (Main.tile[i, num4].frameX < 144)
                                {
                                    Main.tile[i, num4].type = 74;
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendTileSquare(-1, i, num4, 3);
                                    }
                                }
                            }
                            if (Main.tile[i, num4].type == 60 || Main.tile[i, num4].type == 62)
                            {
                                if (WorldGen.genRand.Next(5) == 0)
                                {
                                    if (!Main.tile[i, num4 + 1].active && !Main.tile[i, num4 + 1].lava)
                                    {
                                        bool flag3 = false;
                                        for (int l = num4; l > num4 - 10; l--)
                                        {
                                            if (Main.tile[i, l].active && Main.tile[i, l].type == 60)
                                            {
                                                flag3 = true;
                                                break;
                                            }
                                        }
                                        if (flag3)
                                        {
                                            int num10 = i;
                                            int l = num4 + 1;
                                            Main.tile[num10, l].type = 62;
                                            Main.tile[num10, l].active = true;
                                            WorldGen.SquareTileFrame(num10, l, true);
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendTileSquare(-1, num10, l, 3);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Main.tile[i, num4].type == 69 && WorldGen.genRand.Next(3) == 0)
                            {
                                int num10 = i;
                                int l = num4;
                                int num11 = 0;
                                if (Main.tile[num10 + 1, l].active && Main.tile[num10 + 1, l].type == 69)
                                {
                                    num11++;
                                }
                                if (Main.tile[num10 - 1, l].active && Main.tile[num10 - 1, l].type == 69)
                                {
                                    num11++;
                                }
                                if (Main.tile[num10, l + 1].active && Main.tile[num10, l + 1].type == 69)
                                {
                                    num11++;
                                }
                                if (Main.tile[num10, l - 1].active && Main.tile[num10, l - 1].type == 69)
                                {
                                    num11++;
                                }
                                if (num11 < 3 || Main.tile[i, num4].type == 60)
                                {
                                    int num12 = WorldGen.genRand.Next(4);
                                    if (num12 == 0)
                                    {
                                        l--;
                                    }
                                    else
                                    {
                                        if (num12 == 1)
                                        {
                                            l++;
                                        }
                                        else
                                        {
                                            if (num12 == 2)
                                            {
                                                num10--;
                                            }
                                            else
                                            {
                                                if (num12 == 3)
                                                {
                                                    num10++;
                                                }
                                            }
                                        }
                                    }
                                    if (!Main.tile[num10, l].active)
                                    {
                                        num11 = 0;
                                        if (Main.tile[num10 + 1, l].active && Main.tile[num10 + 1, l].type == 69)
                                        {
                                            num11++;
                                        }
                                        if (Main.tile[num10 - 1, l].active && Main.tile[num10 - 1, l].type == 69)
                                        {
                                            num11++;
                                        }
                                        if (Main.tile[num10, l + 1].active && Main.tile[num10, l + 1].type == 69)
                                        {
                                            num11++;
                                        }
                                        if (Main.tile[num10, l - 1].active && Main.tile[num10, l - 1].type == 69)
                                        {
                                            num11++;
                                        }
                                        if (num11 < 2)
                                        {
                                            int num13 = 7;
                                            int num14 = num10 - num13;
                                            int num15 = num10 + num13;
                                            int num16 = l - num13;
                                            int num17 = l + num13;
                                            bool flag3 = false;
                                            for (int m = num14; m < num15; m++)
                                            {
                                                for (int n = num16; n < num17; n++)
                                                {
                                                    if (Math.Abs(m - num10) * 2 + Math.Abs(n - l) < 9)
                                                    {
                                                        if (Main.tile[m, n].active && Main.tile[m, n].type == 60)
                                                        {
                                                            if (Main.tile[m, n - 1].active && Main.tile[m, n - 1].type == 69 && Main.tile[m, n - 1].liquid == 0)
                                                            {
                                                                flag3 = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (flag3)
                                            {
                                                Main.tile[num10, l].type = 69;
                                                Main.tile[num10, l].active = true;
                                                WorldGen.SquareTileFrame(num10, l, true);
                                                if (Main.netMode == 2)
                                                {
                                                    NetMessage.SendTileSquare(-1, num10, l, 3);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (Main.tile[i, num4].type == 70)
                            {
                                int num9 = (int)Main.tile[i, num4].type;
                                if (!Main.tile[i, num7].active && WorldGen.genRand.Next(10) == 0)
                                {
                                    WorldGen.PlaceTile(i, num7, 71, true, false, -1);
                                    if (Main.netMode == 2 && Main.tile[i, num7].active)
                                    {
                                        NetMessage.SendTileSquare(-1, i, num7, 1);
                                    }
                                }
                                bool flag2 = false;
                                for (int j = num5; j < num6; j++)
                                {
                                    for (int k = num7; k < num8; k++)
                                    {
                                        if (i != j || num4 != k)
                                        {
                                            if (Main.tile[j, k].active)
                                            {
                                                if (Main.tile[j, k].type == 59)
                                                {
                                                    WorldGen.SpreadGrass(j, k, 59, num9, false);
                                                    if ((int)Main.tile[j, k].type == num9)
                                                    {
                                                        WorldGen.SquareTileFrame(j, k, true);
                                                        flag2 = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (Main.netMode == 2 && flag2)
                                {
                                    NetMessage.SendTileSquare(-1, i, num4, 3);
                                }
                            }
                        }
                        else
                        {
                            if (flag && WorldGen.spawnNPC > 0)
                            {
                                WorldGen.SpawnNPC(i, num4);
                            }
                        }
                    }
                }
                num3++;
            }
            if (!Main.dayTime)
            {
                float num18 = (float)(Main.maxTilesX / 4200);
                if ((float)Main.rand.Next(8000) < 10f * num18)
                {
                    int num19 = 12;
                    int num20 = Main.rand.Next(Main.maxTilesX - 50) + 100;
                    num20 *= 16;
                    int num21 = Main.rand.Next((int)((double)Main.maxTilesY * 0.05));
                    num21 *= 16;
                    Vector2 vector = new Vector2((float)num20, (float)num21);
                    float num22 = (float)Main.rand.Next(-100, 101);
                    float num23 = (float)(Main.rand.Next(200) + 100);
                    float num24 = (float)Math.Sqrt((double)(num22 * num22 + num23 * num23));
                    num24 = (float)num19 / num24;
                    num22 *= num24;
                    num23 *= num24;
                    Projectile.NewProjectile(vector.X, vector.Y, num22, num23, 12, 1000, 10f, Main.myPlayer);
                }
            }
        }
        
        public static void PlaceWall(int i, int j, int type, bool mute = false)
        {
            if (Main.tile[i, j] == null)
            {
                Main.tile[i, j] = new Tile();
            }
            if ((int)Main.tile[i, j].wall != type)
            {
                for (int k = i - 1; k < i + 2; k++)
                {
                    for (int l = j - 1; l < j + 2; l++)
                    {
                        if (Main.tile[k, l] == null)
                        {
                            Main.tile[k, l] = new Tile();
                        }
                        if (Main.tile[k, l].wall > 0 && (int)Main.tile[k, l].wall != type)
                        {
                            return;
                        }
                    }
                }
                Main.tile[i, j].wall = (byte)type;
                WorldGen.SquareWallFrame(i, j, true);
                if (!mute)
                {
                    //Main.PlaySound(0, i * 16, j * 16, 1);
                }
            }
        }
        
        public static void AddPlants()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 1; j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j].type == 2 && Main.tile[i, j].active)
                    {
                        if (!Main.tile[i, j - 1].active)
                        {
                            WorldGen.PlaceTile(i, j - 1, 3, true, false, -1);
                        }
                    }
                    else
                    {
                        if (Main.tile[i, j].type == 23 && Main.tile[i, j].active)
                        {
                            if (!Main.tile[i, j - 1].active)
                            {
                                WorldGen.PlaceTile(i, j - 1, 24, true, false, -1);
                            }
                        }
                    }
                }
            }
        }
        
        public static void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true)
        {
            if (i > Main.maxTilesX || j > Main.maxTilesY)
            {
                return;
            }
            if ((int)Main.tile[i, j].type == dirt && Main.tile[i, j].active && ((double)j >= Main.worldSurface || grass != 70) && ((double)j < Main.worldSurface || dirt != 0))
            {
                int num = i - 1;
                int num2 = i + 2;
                int num3 = j - 1;
                int num4 = j + 2;
                if (num < 0)
                {
                    num = 0;
                }
                if (num2 > Main.maxTilesX)
                {
                    num2 = Main.maxTilesX;
                }
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > Main.maxTilesY)
                {
                    num4 = Main.maxTilesY;
                }
                bool flag = true;
                for (int k = num; k < num2; k++)
                {
                    for (int l = num3; l < num4; l++)
                    {
                        if (!Main.tile[k, l].active || !Main.tileSolid[(int)Main.tile[k, l].type])
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    if (grass != 23 || Main.tile[i, j - 1].type != 27)
                    {
                        Main.tile[i, j].type = (byte)grass;
                        for (int k = num; k < num2; k++)
                        {
                            for (int l = num3; l < num4; l++)
                            {
                                if (Main.tile[k, l].active && (int)Main.tile[k, l].type == dirt)
                                {
                                    if (repeat)
                                    {
                                        WorldGen.SpreadGrass(k, l, dirt, grass, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static void ChasmRunner(int i, int j, int steps, bool makeOrb = false)
        {
            bool flag = false;
            bool flag2 = false;
            if (!makeOrb)
            {
                flag = true;
            }
            float num = (float)steps;
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j;
            Vector2 value2;
            value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            value2.Y = (float)WorldGen.genRand.Next(11) * 0.2f + 0.5f;
            int num2 = 5;
            double num3 = (double)(WorldGen.genRand.Next(5) + 7);
            while (num3 > 0.0)
            {
                if (num > 0f)
                {
                    num3 += (double)WorldGen.genRand.Next(3);
                    num3 -= (double)WorldGen.genRand.Next(3);
                    if (num3 < 7.0)
                    {
                        num3 = 7.0;
                    }
                    if (num3 > 20.0)
                    {
                        num3 = 20.0;
                    }
                    if (num == 1f && num3 < 10.0)
                    {
                        num3 = 10.0;
                    }
                }
                else
                {
                    num3 -= (double)WorldGen.genRand.Next(4);
                }
                if ((double)value.Y > Main.rockLayer && num > 0f)
                {
                    num = 0f;
                }
                num -= 1f;
                int num4;
                int num5;
                int num6;
                int num7;
                if (num > (float)num2)
                {
                    num4 = (int)((double)value.X - num3 * 0.5);
                    num5 = (int)((double)value.X + num3 * 0.5);
                    num6 = (int)((double)value.Y - num3 * 0.5);
                    num7 = (int)((double)value.Y + num3 * 0.5);
                    if (num4 < 0)
                    {
                        num4 = 0;
                    }
                    if (num5 > Main.maxTilesX - 1)
                    {
                        num5 = Main.maxTilesX - 1;
                    }
                    if (num6 < 0)
                    {
                        num6 = 0;
                    }
                    if (num7 > Main.maxTilesY)
                    {
                        num7 = Main.maxTilesY;
                    }
                    for (int k = num4; k < num5; k++)
                    {
                        for (int l = num6; l < num7; l++)
                        {
                            if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num3 * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                            {
                                Main.tile[k, l].active = false;
                            }
                        }
                    }
                }
                if (num <= 0f)
                {
                    if (!flag)
                    {
                        flag = true;
                        WorldGen.AddShadowOrb((int)value.X, (int)value.Y);
                    }
                    else
                    {
                        if (!flag2)
                        {
                            flag2 = false;
                            bool flag3 = false;
                            int num8 = 0;
                            while (!flag3)
                            {
                                int num9 = WorldGen.genRand.Next((int)value.X - 25, (int)value.X + 25);
                                int num10 = WorldGen.genRand.Next((int)value.Y - 50, (int)value.Y);
                                if (num9 < 5)
                                {
                                    num9 = 5;
                                }
                                if (num9 > Main.maxTilesX - 5)
                                {
                                    num9 = Main.maxTilesX - 5;
                                }
                                if (num10 < 5)
                                {
                                    num10 = 5;
                                }
                                if (num10 > Main.maxTilesY - 5)
                                {
                                    num10 = Main.maxTilesY - 5;
                                }
                                if ((double)num10 > Main.worldSurface)
                                {
                                    WorldGen.Place3x2(num9, num10, 26);
                                    if (Main.tile[num9, num10].type == 26)
                                    {
                                        flag3 = true;
                                    }
                                    else
                                    {
                                        num8++;
                                        if (num8 >= 10000)
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
                }
                value += value2;
                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.01f;
                if ((double)value2.X > 0.3)
                {
                    value2.X = 0.3f;
                }
                if ((double)value2.X < -0.3)
                {
                    value2.X = -0.3f;
                }
                num4 = (int)((double)value.X - num3 * 1.1);
                num5 = (int)((double)value.X + num3 * 1.1);
                num6 = (int)((double)value.Y - num3 * 1.1);
                num7 = (int)((double)value.Y + num3 * 1.1);
                if (num4 < 1)
                {
                    num4 = 1;
                }
                if (num5 > Main.maxTilesX - 1)
                {
                    num5 = Main.maxTilesX - 1;
                }
                if (num6 < 0)
                {
                    num6 = 0;
                }
                if (num7 > Main.maxTilesY)
                {
                    num7 = Main.maxTilesY;
                }
                for (int k = num4; k < num5; k++)
                {
                    for (int l = num6; l < num7; l++)
                    {
                        if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            if (Main.tile[k, l].type != 25 && l > j + WorldGen.genRand.Next(3, 20))
                            {
                                Main.tile[k, l].active = true;
                            }
                            if (steps <= num2)
                            {
                                Main.tile[k, l].active = true;
                            }
                            if (Main.tile[k, l].type != 31)
                            {
                                Main.tile[k, l].type = 25;
                            }
                            if (Main.tile[k, l].wall == 2)
                            {
                                Main.tile[k, l].wall = 0;
                            }
                        }
                    }
                }
                for (int k = num4; k < num5; k++)
                {
                    for (int l = num6; l < num7; l++)
                    {
                        if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            if (Main.tile[k, l].type != 31)
                            {
                                Main.tile[k, l].type = 25;
                            }
                            if (steps <= num2)
                            {
                                Main.tile[k, l].active = true;
                            }
                            if (l > j + WorldGen.genRand.Next(3, 20))
                            {
                                WorldGen.PlaceWall(k, l, 3, true);
                            }
                        }
                    }
                }
            }
        }
        
        public static void TileRunner(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool noYChange = false, bool overRide = true)
        {
            double num = strength;
            float num2 = (float)steps;
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j;
            Vector2 value2;
            value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            value2.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            if (speedX != 0f || speedY != 0f)
            {
                value2.X = speedX;
                value2.Y = speedY;
            }
            while (num > 0.0 && num2 > 0f)
            {
                if (value.Y < 0f && num2 > 0f && type == 59)
                {
                    num2 = 0f;
                }
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
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                for (int k = num3; k < num4; k++)
                {
                    for (int l = num5; l < num6; l++)
                    {
                        if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < strength * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            if (type < 0)
                            {
                                if (type == -2 && Main.tile[k, l].active)
                                {
                                    if (l < WorldGen.waterLine || l > WorldGen.lavaLine)
                                    {
                                        Main.tile[k, l].liquid = 255;
                                        if (l > WorldGen.lavaLine)
                                        {
                                            Main.tile[k, l].lava = true;
                                        }
                                    }
                                }
                                Main.tile[k, l].active = false;
                            }
                            else
                            {
                                if (overRide || !Main.tile[k, l].active)
                                {
                                    if (type != 40 || Main.tile[k, l].type != 53)
                                    {
                                        if (!Main.tileStone[type] || Main.tile[k, l].type == 1)
                                        {
                                            if (Main.tile[k, l].type != 45)
                                            {
                                                if (Main.tile[k, l].type != 1 || type != 59 || (double)l >= Main.worldSurface + (double)WorldGen.genRand.Next(-50, 50))
                                                {
                                                    if (Main.tile[k, l].type != 53 || type != 59)
                                                    {
                                                        Main.tile[k, l].type = (byte)type;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (addTile)
                                {
                                    Main.tile[k, l].active = true;
                                    Main.tile[k, l].liquid = 0;
                                    Main.tile[k, l].lava = false;
                                }
                                if (noYChange && (double)l < Main.worldSurface && type != 59)
                                {
                                    Main.tile[k, l].wall = 2;
                                }
                                if (type == 59)
                                {
                                    if (l > WorldGen.waterLine && Main.tile[k, l].liquid > 0)
                                    {
                                        Main.tile[k, l].lava = false;
                                        Main.tile[k, l].liquid = 0;
                                    }
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
                    if (type != 59)
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
                }
                if (type == 59 && !noYChange)
                {
                    if ((double)value2.Y > 0.5)
                    {
                        value2.Y = 0.5f;
                    }
                    if ((double)value2.Y < -0.5)
                    {
                        value2.Y = -0.5f;
                    }
                    if ((double)value.Y < Main.rockLayer + 100.0)
                    {
                        value2.Y = 1f;
                    }
                    if (value.Y > (float)(Main.maxTilesY - 300))
                    {
                        value2.Y = -1f;
                    }
                }
            }
        }
        
        public static void FloatingIsland(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(80, 120);
            float num2 = (float)WorldGen.genRand.Next(20, 25);
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j;
            Vector2 value2;
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
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
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
                                Main.tile[k, l].active = true;
                                if (Main.tile[k, l].type == 59)
                                {
                                    Main.tile[k, l].type = 0;
                                }
                            }
                        }
                    }
                }
                WorldGen.TileRunner(WorldGen.genRand.Next(num3 + 10, num4 - 10), (int)((double)value.Y + num7 * 0.1 + 5.0), (double)WorldGen.genRand.Next(5, 10), WorldGen.genRand.Next(10, 15), 0, true, 0f, 2f, true, true);
                num3 = (int)((double)value.X - num * 0.4);
                num4 = (int)((double)value.X + num * 0.4);
                num5 = (int)((double)value.Y - num * 0.4);
                num6 = (int)((double)value.Y + num * 0.4);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                for (int k = num3; k < num4; k++)
                {
                    for (int l = num5; l < num6; l++)
                    {
                        if ((float)l > value.Y + 2f)
                        {
                            float num9 = Math.Abs((float)k - value.X);
                            float num10 = Math.Abs((float)l - value.Y) * 2f;
                            double num11 = Math.Sqrt((double)(num9 * num9 + num10 * num10));
                            if (num11 < num7 * 0.4)
                            {
                                Main.tile[k, l].wall = 2;
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
        
        public static void IslandHouse(int i, int j)
        {
            byte type = (byte)WorldGen.genRand.Next(45, 48);
            byte wall = (byte)WorldGen.genRand.Next(10, 13);
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(7, 12);
            int num3 = WorldGen.genRand.Next(5, 7);
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].active)
                {
                    vector.Y = (float)(k - 1);
                    break;
                }
            }
            vector.X = (float)i;
            int num4 = (int)(vector.X - (float)num2 - 2f);
            int num5 = (int)(vector.X + (float)num2 + 2f);
            int num6 = (int)(vector.Y - (float)num3 - 2f);
            int num7 = (int)(vector.Y + 2f + (float)WorldGen.genRand.Next(3, 5));
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int num8 = num4; num8 <= num5; num8++)
            {
                for (int l = num6; l < num7; l++)
                {
                    Main.tile[num8, l].active = true;
                    Main.tile[num8, l].type = type;
                    Main.tile[num8, l].wall = 0;
                }
            }
            num4 = (int)(vector.X - (float)num2);
            num5 = (int)(vector.X + (float)num2);
            num6 = (int)(vector.Y - (float)num3);
            num7 = (int)(vector.Y + 1f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int num8 = num4; num8 <= num5; num8++)
            {
                for (int l = num6; l < num7; l++)
                {
                    if (Main.tile[num8, l].wall == 0)
                    {
                        Main.tile[num8, l].active = false;
                        Main.tile[num8, l].wall = wall;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active = false;
                Main.tile[num11, num10 - 1].active = false;
                Main.tile[num11, num10 - 2].active = false;
            }
            WorldGen.PlaceTile(num9, num10, 10, true, false, -1);
            int contain = 0;
            int num12 = WorldGen.houseCount;
            if (num12 > 2)
            {
                num12 = WorldGen.genRand.Next(3);
            }
            if (num12 == 0)
            {
                contain = 159;
            }
            else
            {
                if (num12 == 1)
                {
                    contain = 65;
                }
                else
                {
                    if (num12 == 2)
                    {
                        contain = 158;
                    }
                }
            }
            WorldGen.AddBuriedChest(i, num10 - 3, contain);
            WorldGen.houseCount++;
        }
        
        public static void Mountinater(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(80, 120);
            float num2 = (float)WorldGen.genRand.Next(40, 55);
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j + num2 / 2f;
            Vector2 value2;
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
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                double num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                for (int k = num3; k < num4; k++)
                {
                    for (int l = num5; l < num6; l++)
                    {
                        float num8 = Math.Abs((float)k - value.X);
                        float num9 = Math.Abs((float)l - value.Y);
                        double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        if (num10 < num7 * 0.4)
                        {
                            if (!Main.tile[k, l].active)
                            {
                                Main.tile[k, l].active = true;
                                Main.tile[k, l].type = 0;
                            }
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
        
        public static void Lakinater(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(25, 50);
            double num2 = num;
            float num3 = (float)WorldGen.genRand.Next(30, 80);
            if (WorldGen.genRand.Next(5) == 0)
            {
                num *= 1.5;
                num2 *= 1.5;
                num3 *= 1.2f;
            }
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j - num3 * 0.3f;
            Vector2 value2;
            value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            value2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.1f;
            while (num > 0.0 && num3 > 0f)
            {
                if ((double)value.Y + num2 * 0.5 > Main.worldSurface)
                {
                    num3 = 0f;
                }
                num -= (double)WorldGen.genRand.Next(3);
                num3 -= 1f;
                int num4 = (int)((double)value.X - num * 0.5);
                int num5 = (int)((double)value.X + num * 0.5);
                int num6 = (int)((double)value.Y - num * 0.5);
                int num7 = (int)((double)value.Y + num * 0.5);
                if (num4 < 0)
                {
                    num4 = 0;
                }
                if (num5 > Main.maxTilesX)
                {
                    num5 = Main.maxTilesX;
                }
                if (num6 < 0)
                {
                    num6 = 0;
                }
                if (num7 > Main.maxTilesY)
                {
                    num7 = Main.maxTilesY;
                }
                num2 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                for (int k = num4; k < num5; k++)
                {
                    for (int l = num6; l < num7; l++)
                    {
                        float num8 = Math.Abs((float)k - value.X);
                        float num9 = Math.Abs((float)l - value.Y);
                        double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        if (num10 < num2 * 0.4)
                        {
                            if (Main.tile[k, l].active)
                            {
                                Main.tile[k, l].liquid = 255;
                            }
                            Main.tile[k, l].active = false;
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
                if ((double)value2.Y > 1.5)
                {
                    value2.Y = 1.5f;
                }
                if ((double)value2.Y < 0.5)
                {
                    value2.Y = 0.5f;
                }
            }
        }
        
        public static void ShroomPatch(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(40, 70);
            double num2 = num;
            float num3 = (float)WorldGen.genRand.Next(10, 20);
            if (WorldGen.genRand.Next(5) == 0)
            {
                num *= 1.5;
                num2 *= 1.5;
                num3 *= 1.2f;
            }
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j - num3 * 0.3f;
            Vector2 value2;
            value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            value2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.1f;
            while (num > 0.0 && num3 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(3);
                num3 -= 1f;
                int num4 = (int)((double)value.X - num * 0.5);
                int num5 = (int)((double)value.X + num * 0.5);
                int num6 = (int)((double)value.Y - num * 0.5);
                int num7 = (int)((double)value.Y + num * 0.5);
                if (num4 < 0)
                {
                    num4 = 0;
                }
                if (num5 > Main.maxTilesX)
                {
                    num5 = Main.maxTilesX;
                }
                if (num6 < 0)
                {
                    num6 = 0;
                }
                if (num7 > Main.maxTilesY)
                {
                    num7 = Main.maxTilesY;
                }
                num2 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                for (int k = num4; k < num5; k++)
                {
                    for (int l = num6; l < num7; l++)
                    {
                        float num8 = Math.Abs((float)k - value.X);
                        float num9 = Math.Abs(((float)l - value.Y) * 2.3f);
                        double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        if (num10 < num2 * 0.4)
                        {
                            if ((double)l < (double)value.Y + num2 * 0.05)
                            {
                                if (Main.tile[k, l].type != 59)
                                {
                                    Main.tile[k, l].active = false;
                                }
                            }
                            else
                            {
                                Main.tile[k, l].type = 59;
                            }
                            Main.tile[k, l].liquid = 0;
                            Main.tile[k, l].lava = false;
                        }
                    }
                }
                value += value2;
                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                if (value2.X > 1f)
                {
                    value2.X = 0.1f;
                }
                if (value2.X < -1f)
                {
                    value2.X = -1f;
                }
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
        
        public static void Cavinator(int i, int j, int steps)
        {
            double num = (double)WorldGen.genRand.Next(7, 15);
            int num2 = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num2 = -1;
            }
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j;
            int k = WorldGen.genRand.Next(20, 40);
            Vector2 value2;
            value2.Y = (float)WorldGen.genRand.Next(10, 20) * 0.01f;
            value2.X = (float)num2;
            while (k > 0)
            {
                k--;
                int num3 = (int)((double)value.X - num * 0.5);
                int num4 = (int)((double)value.X + num * 0.5);
                int num5 = (int)((double)value.Y - num * 0.5);
                int num6 = (int)((double)value.Y + num * 0.5);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                double num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                for (int l = num3; l < num4; l++)
                {
                    for (int m = num5; m < num6; m++)
                    {
                        float num8 = Math.Abs((float)l - value.X);
                        float num9 = Math.Abs((float)m - value.Y);
                        double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        if (num10 < num7 * 0.4)
                        {
                            Main.tile[l, m].active = false;
                        }
                    }
                }
                value += value2;
                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                if (value2.X > (float)num2 + 0.5f)
                {
                    value2.X = (float)num2 + 0.5f;
                }
                if (value2.X < (float)num2 - 0.5f)
                {
                    value2.X = (float)num2 - 0.5f;
                }
                if (value2.Y > 2f)
                {
                    value2.Y = 2f;
                }
                if (value2.Y < 0f)
                {
                    value2.Y = 0f;
                }
            }
            if (steps > 0 && (double)((int)value.Y) < Main.rockLayer + 50.0)
            {
                WorldGen.Cavinator((int)value.X, (int)value.Y, steps - 1);
            }
        }
        
        public static void CaveOpenater(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(7, 12);
            int num2 = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num2 = -1;
            }
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j;
            int k = 100;
            Vector2 value2;
            value2.Y = 0f;
            value2.X = (float)num2;
            while (k > 0)
            {
                if (Main.tile[(int)value.X, (int)value.Y].wall == 0)
                {
                    k = 0;
                }
                k--;
                int num3 = (int)((double)value.X - num * 0.5);
                int num4 = (int)((double)value.X + num * 0.5);
                int num5 = (int)((double)value.Y - num * 0.5);
                int num6 = (int)((double)value.Y + num * 0.5);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > Main.maxTilesX)
                {
                    num4 = Main.maxTilesX;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                double num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                for (int l = num3; l < num4; l++)
                {
                    for (int m = num5; m < num6; m++)
                    {
                        float num8 = Math.Abs((float)l - value.X);
                        float num9 = Math.Abs((float)m - value.Y);
                        double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        if (num10 < num7 * 0.4)
                        {
                            Main.tile[l, m].active = false;
                        }
                    }
                }
                value += value2;
                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                if (value2.X > (float)num2 + 0.5f)
                {
                    value2.X = (float)num2 + 0.5f;
                }
                if (value2.X < (float)num2 - 0.5f)
                {
                    value2.X = (float)num2 - 0.5f;
                }
                if (value2.Y > 0f)
                {
                    value2.Y = 0f;
                }
                if ((double)value2.Y < -0.5)
                {
                    value2.Y = -0.5f;
                }
            }
        }
        
        public static void SquareTileFrame(int i, int j, bool resetFrame = true)
        {
            WorldGen.TileFrame(i - 1, j - 1, false, false);
            WorldGen.TileFrame(i - 1, j, false, false);
            WorldGen.TileFrame(i - 1, j + 1, false, false);
            WorldGen.TileFrame(i, j - 1, false, false);
            WorldGen.TileFrame(i, j, resetFrame, false);
            WorldGen.TileFrame(i, j + 1, false, false);
            WorldGen.TileFrame(i + 1, j - 1, false, false);
            WorldGen.TileFrame(i + 1, j, false, false);
            WorldGen.TileFrame(i + 1, j + 1, false, false);
        }
        
        public static void SquareWallFrame(int i, int j, bool resetFrame = true)
        {
            WorldGen.WallFrame(i - 1, j - 1, false);
            WorldGen.WallFrame(i - 1, j, false);
            WorldGen.WallFrame(i - 1, j + 1, false);
            WorldGen.WallFrame(i, j - 1, false);
            WorldGen.WallFrame(i, j, resetFrame);
            WorldGen.WallFrame(i, j + 1, false);
            WorldGen.WallFrame(i + 1, j - 1, false);
            WorldGen.WallFrame(i + 1, j, false);
            WorldGen.WallFrame(i + 1, j + 1, false);
        }
        
        public static void SectionTileFrame(int startX, int startY, int endX, int endY)
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
            if (num > Main.maxTilesX - 2)
            {
                num = Main.maxTilesX - 2;
            }
            if (num3 > Main.maxTilesY - 2)
            {
                num3 = Main.maxTilesY - 2;
            }
            for (int i = num - 1; i < num2 + 1; i++)
            {
                for (int j = num3 - 1; j < num4 + 1; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                    }
                    WorldGen.TileFrame(i, j, true, true);
                    WorldGen.WallFrame(i, j, true);
                }
            }
        }
        
        public static void RangeFrame(int startX, int startY, int endX, int endY)
        {
            int num = endX + 1;
            int num2 = endY + 1;
            for (int i = startX - 1; i < num + 1; i++)
            {
                for (int j = startY - 1; j < num2 + 1; j++)
                {
                    WorldGen.TileFrame(i, j, false, false);
                    WorldGen.WallFrame(i, j, false);
                }
            }
        }
        
        public static void WaterCheck()
        {
            Liquid.numLiquid = 0;
            LiquidBuffer.numLiquidBuffer = 0;
            for (int i = 1; i < Main.maxTilesX - 1; i++)
            {
                for (int j = Main.maxTilesY - 2; j > 0; j--)
                {
                    Main.tile[i, j].checkingLiquid = false;
                    if (Main.tile[i, j].liquid > 0 && Main.tile[i, j].active && Main.tileSolid[(int)Main.tile[i, j].type] && !Main.tileSolidTop[(int)Main.tile[i, j].type])
                    {
                        Main.tile[i, j].liquid = 0;
                    }
                    else
                    {
                        if (Main.tile[i, j].liquid > 0)
                        {
                            if (Main.tile[i, j].active)
                            {
                                if (Main.tileWaterDeath[(int)Main.tile[i, j].type])
                                {
                                    WorldGen.KillTile(i, j, false, false, false);
                                }
                                if (Main.tile[i, j].lava)
                                {
                                    if (Main.tileLavaDeath[(int)Main.tile[i, j].type])
                                    {
                                        WorldGen.KillTile(i, j, false, false, false);
                                    }
                                }
                            }
                            if ((!Main.tile[i, j + 1].active || !Main.tileSolid[(int)Main.tile[i, j + 1].type] || Main.tileSolidTop[(int)Main.tile[i, j + 1].type]) && Main.tile[i, j + 1].liquid < 255)
                            {
                                if (Main.tile[i, j + 1].liquid > 250)
                                {
                                    Main.tile[i, j + 1].liquid = 255;
                                }
                                else
                                {
                                    Liquid.AddWater(i, j);
                                }
                            }
                            if ((!Main.tile[i - 1, j].active || !Main.tileSolid[(int)Main.tile[i - 1, j].type] || Main.tileSolidTop[(int)Main.tile[i - 1, j].type]) && Main.tile[i - 1, j].liquid != Main.tile[i, j].liquid)
                            {
                                Liquid.AddWater(i, j);
                            }
                            else
                            {
                                if ((!Main.tile[i + 1, j].active || !Main.tileSolid[(int)Main.tile[i + 1, j].type] || Main.tileSolidTop[(int)Main.tile[i + 1, j].type]) && Main.tile[i + 1, j].liquid != Main.tile[i, j].liquid)
                                {
                                    Liquid.AddWater(i, j);
                                }
                            }
                            if (Main.tile[i, j].lava)
                            {
                                if (Main.tile[i - 1, j].liquid > 0 && !Main.tile[i - 1, j].lava)
                                {
                                    Liquid.AddWater(i, j);
                                }
                                else
                                {
                                    if (Main.tile[i + 1, j].liquid > 0 && !Main.tile[i + 1, j].lava)
                                    {
                                        Liquid.AddWater(i, j);
                                    }
                                    else
                                    {
                                        if (Main.tile[i, j - 1].liquid > 0 && !Main.tile[i, j - 1].lava)
                                        {
                                            Liquid.AddWater(i, j);
                                        }
                                        else
                                        {
                                            if (Main.tile[i, j + 1].liquid > 0 && !Main.tile[i, j + 1].lava)
                                            {
                                                Liquid.AddWater(i, j);
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
        
        public static void EveryTileFrame()
        {
            WorldGen.noLiquidCheck = true;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                float num = (float)i / (float)Main.maxTilesX;
                Program.printData("Finding tile frames: " + (int)(num * 100f + 1f) + "%");
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    WorldGen.TileFrame(i, j, true, false);
                    WorldGen.WallFrame(i, j, true);
                }
            }
            WorldGen.noLiquidCheck = false;
        }
        
        public static void PlantCheck(int i, int j)
        {
            int num = -1;
            int type = (int)Main.tile[i, j].type;
            if (i - 1 >= 0)
            {
                if (i + 1 < Main.maxTilesX)
                {
                    if (j - 1 < 0)
                    {
                        if (j + 1 >= Main.maxTilesY)
                        {
                            num = type;
                        }
                        if (i - 1 >= 0 && Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
                        {
                            int type2 = (int)Main.tile[i - 1, j].type;
                        }
                        if (i + 1 < Main.maxTilesX && Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
                        {
                            int type3 = (int)Main.tile[i + 1, j].type;
                        }
                        if (j - 1 >= 0 && Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
                        {
                            int type4 = (int)Main.tile[i, j - 1].type;
                        }
                        if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
                        {
                            num = (int)Main.tile[i, j + 1].type;
                        }
                        if (i - 1 >= 0 && j - 1 >= 0 && Main.tile[i - 1, j - 1] != null && Main.tile[i - 1, j - 1].active)
                        {
                            int type5 = (int)Main.tile[i - 1, j - 1].type;
                        }
                        if (i + 1 < Main.maxTilesX && j - 1 >= 0 && Main.tile[i + 1, j - 1] != null && Main.tile[i + 1, j - 1].active)
                        {
                            int type6 = (int)Main.tile[i + 1, j - 1].type;
                        }
                        if (i - 1 >= 0 && j + 1 < Main.maxTilesY && Main.tile[i - 1, j + 1] != null && Main.tile[i - 1, j + 1].active)
                        {
                            int type7 = (int)Main.tile[i - 1, j + 1].type;
                        }
                        if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY && Main.tile[i + 1, j + 1] != null && Main.tile[i + 1, j + 1].active)
                        {
                            int type8 = (int)Main.tile[i + 1, j + 1].type;
                        }
                        if ((type == 3 && num != 2 && num != 78) || (type == 24 && num != 23) || (type == 61 && num != 60) || (type == 71 && num != 70) || (type == 73 && num != 2 && num != 78) || (type == 74 && num != 60))
                        {
                            WorldGen.KillTile(i, j, false, false, false);
                        }
                        return;
                    }
                }
            }
        }
        
        public static void WallFrame(int i, int j, bool resetFrame = false)
        {
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY && Main.tile[i, j] != null && Main.tile[i, j].wall > 0)
            {
                int num = -1;
                int num2 = -1;
                int num3 = -1;
                int num4 = -1;
                int num5 = -1;
                int num6 = -1;
                int num7 = -1;
                int num8 = -1;
                int wall = (int)Main.tile[i, j].wall;
                if (wall != 0)
                {
                    int wallFrameX = (int)Main.tile[i, j].wallFrameX;
                    int wallFrameY = (int)Main.tile[i, j].wallFrameY;
                    Rectangle rectangle;
                    rectangle.X = -1;
                    rectangle.Y = -1;
                    if (i - 1 < 0)
                    {
                        num = wall;
                        num4 = wall;
                        num6 = wall;
                    }
                    if (i + 1 >= Main.maxTilesX)
                    {
                        num3 = wall;
                        num5 = wall;
                        num8 = wall;
                    }
                    if (j - 1 < 0)
                    {
                        num = wall;
                        num2 = wall;
                        num3 = wall;
                    }
                    if (j + 1 >= Main.maxTilesY)
                    {
                        num6 = wall;
                        num7 = wall;
                        num8 = wall;
                    }
                    if (i - 1 >= 0 && Main.tile[i - 1, j] != null)
                    {
                        num4 = (int)Main.tile[i - 1, j].wall;
                    }
                    if (i + 1 < Main.maxTilesX && Main.tile[i + 1, j] != null)
                    {
                        num5 = (int)Main.tile[i + 1, j].wall;
                    }
                    if (j - 1 >= 0 && Main.tile[i, j - 1] != null)
                    {
                        num2 = (int)Main.tile[i, j - 1].wall;
                    }
                    if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1] != null)
                    {
                        num7 = (int)Main.tile[i, j + 1].wall;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0 && Main.tile[i - 1, j - 1] != null)
                    {
                        num = (int)Main.tile[i - 1, j - 1].wall;
                    }
                    if (i + 1 < Main.maxTilesX && j - 1 >= 0 && Main.tile[i + 1, j - 1] != null)
                    {
                        num3 = (int)Main.tile[i + 1, j - 1].wall;
                    }
                    if (i - 1 >= 0 && j + 1 < Main.maxTilesY && Main.tile[i - 1, j + 1] != null)
                    {
                        num6 = (int)Main.tile[i - 1, j + 1].wall;
                    }
                    if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY && Main.tile[i + 1, j + 1] != null)
                    {
                        num8 = (int)Main.tile[i + 1, j + 1].wall;
                    }
                    if (wall == 2)
                    {
                        if (j == (int)Main.worldSurface)
                        {
                            num7 = wall;
                            num6 = wall;
                            num8 = wall;
                        }
                        else
                        {
                            if (j >= (int)Main.worldSurface)
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
                    }
                    int num9 = 0;
                    if (resetFrame)
                    {
                        num9 = WorldGen.genRand.Next(0, 3);
                        Main.tile[i, j].wallFrameNumber = (byte)num9;
                    }
                    else
                    {
                        num9 = (int)Main.tile[i, j].wallFrameNumber;
                    }
                    if (rectangle.X < 0 || rectangle.Y < 0)
                    {
                        if (num2 == wall && num7 == wall && (num4 == wall & num5 == wall))
                        {
                            if (num != wall && num3 != wall)
                            {
                                if (num9 == 0)
                                {
                                    rectangle.X = 108;
                                    rectangle.Y = 18;
                                }
                                if (num9 == 1)
                                {
                                    rectangle.X = 126;
                                    rectangle.Y = 18;
                                }
                                if (num9 == 2)
                                {
                                    rectangle.X = 144;
                                    rectangle.Y = 18;
                                }
                            }
                            else
                            {
                                if (num6 != wall && num8 != wall)
                                {
                                    if (num9 == 0)
                                    {
                                        rectangle.X = 108;
                                        rectangle.Y = 36;
                                    }
                                    if (num9 == 1)
                                    {
                                        rectangle.X = 126;
                                        rectangle.Y = 36;
                                    }
                                    if (num9 == 2)
                                    {
                                        rectangle.X = 144;
                                        rectangle.Y = 36;
                                    }
                                }
                                else
                                {
                                    if (num != wall && num6 != wall)
                                    {
                                        if (num9 == 0)
                                        {
                                            rectangle.X = 180;
                                            rectangle.Y = 0;
                                        }
                                        if (num9 == 1)
                                        {
                                            rectangle.X = 180;
                                            rectangle.Y = 18;
                                        }
                                        if (num9 == 2)
                                        {
                                            rectangle.X = 180;
                                            rectangle.Y = 36;
                                        }
                                    }
                                    else
                                    {
                                        if (num3 != wall && num8 != wall)
                                        {
                                            if (num9 == 0)
                                            {
                                                rectangle.X = 198;
                                                rectangle.Y = 0;
                                            }
                                            if (num9 == 1)
                                            {
                                                rectangle.X = 198;
                                                rectangle.Y = 18;
                                            }
                                            if (num9 == 2)
                                            {
                                                rectangle.X = 198;
                                                rectangle.Y = 36;
                                            }
                                        }
                                        else
                                        {
                                            if (num9 == 0)
                                            {
                                                rectangle.X = 18;
                                                rectangle.Y = 18;
                                            }
                                            if (num9 == 1)
                                            {
                                                rectangle.X = 36;
                                                rectangle.Y = 18;
                                            }
                                            if (num9 == 2)
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
                            if (num2 != wall && num7 == wall && (num4 == wall & num5 == wall))
                            {
                                if (num9 == 0)
                                {
                                    rectangle.X = 18;
                                    rectangle.Y = 0;
                                }
                                if (num9 == 1)
                                {
                                    rectangle.X = 36;
                                    rectangle.Y = 0;
                                }
                                if (num9 == 2)
                                {
                                    rectangle.X = 54;
                                    rectangle.Y = 0;
                                }
                            }
                            else
                            {
                                if (num2 == wall && num7 != wall && (num4 == wall & num5 == wall))
                                {
                                    if (num9 == 0)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 36;
                                    }
                                    if (num9 == 1)
                                    {
                                        rectangle.X = 36;
                                        rectangle.Y = 36;
                                    }
                                    if (num9 == 2)
                                    {
                                        rectangle.X = 54;
                                        rectangle.Y = 36;
                                    }
                                }
                                else
                                {
                                    if (num2 == wall && num7 == wall && (num4 != wall & num5 == wall))
                                    {
                                        if (num9 == 0)
                                        {
                                            rectangle.X = 0;
                                            rectangle.Y = 0;
                                        }
                                        if (num9 == 1)
                                        {
                                            rectangle.X = 0;
                                            rectangle.Y = 18;
                                        }
                                        if (num9 == 2)
                                        {
                                            rectangle.X = 0;
                                            rectangle.Y = 36;
                                        }
                                    }
                                    else
                                    {
                                        if (num2 == wall && num7 == wall && (num4 == wall & num5 != wall))
                                        {
                                            if (num9 == 0)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 0;
                                            }
                                            if (num9 == 1)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 18;
                                            }
                                            if (num9 == 2)
                                            {
                                                rectangle.X = 72;
                                                rectangle.Y = 36;
                                            }
                                        }
                                        else
                                        {
                                            if (num2 != wall && num7 == wall && (num4 != wall & num5 == wall))
                                            {
                                                if (num9 == 0)
                                                {
                                                    rectangle.X = 0;
                                                    rectangle.Y = 54;
                                                }
                                                if (num9 == 1)
                                                {
                                                    rectangle.X = 36;
                                                    rectangle.Y = 54;
                                                }
                                                if (num9 == 2)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 54;
                                                }
                                            }
                                            else
                                            {
                                                if (num2 != wall && num7 == wall && (num4 == wall & num5 != wall))
                                                {
                                                    if (num9 == 0)
                                                    {
                                                        rectangle.X = 18;
                                                        rectangle.Y = 54;
                                                    }
                                                    if (num9 == 1)
                                                    {
                                                        rectangle.X = 54;
                                                        rectangle.Y = 54;
                                                    }
                                                    if (num9 == 2)
                                                    {
                                                        rectangle.X = 90;
                                                        rectangle.Y = 54;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num2 == wall && num7 != wall && (num4 != wall & num5 == wall))
                                                    {
                                                        if (num9 == 0)
                                                        {
                                                            rectangle.X = 0;
                                                            rectangle.Y = 72;
                                                        }
                                                        if (num9 == 1)
                                                        {
                                                            rectangle.X = 36;
                                                            rectangle.Y = 72;
                                                        }
                                                        if (num9 == 2)
                                                        {
                                                            rectangle.X = 72;
                                                            rectangle.Y = 72;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num2 == wall && num7 != wall && (num4 == wall & num5 != wall))
                                                        {
                                                            if (num9 == 0)
                                                            {
                                                                rectangle.X = 18;
                                                                rectangle.Y = 72;
                                                            }
                                                            if (num9 == 1)
                                                            {
                                                                rectangle.X = 54;
                                                                rectangle.Y = 72;
                                                            }
                                                            if (num9 == 2)
                                                            {
                                                                rectangle.X = 90;
                                                                rectangle.Y = 72;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num2 == wall && num7 == wall && (num4 != wall & num5 != wall))
                                                            {
                                                                if (num9 == 0)
                                                                {
                                                                    rectangle.X = 90;
                                                                    rectangle.Y = 0;
                                                                }
                                                                if (num9 == 1)
                                                                {
                                                                    rectangle.X = 90;
                                                                    rectangle.Y = 18;
                                                                }
                                                                if (num9 == 2)
                                                                {
                                                                    rectangle.X = 90;
                                                                    rectangle.Y = 36;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num2 != wall && num7 != wall && (num4 == wall & num5 == wall))
                                                                {
                                                                    if (num9 == 0)
                                                                    {
                                                                        rectangle.X = 108;
                                                                        rectangle.Y = 72;
                                                                    }
                                                                    if (num9 == 1)
                                                                    {
                                                                        rectangle.X = 126;
                                                                        rectangle.Y = 72;
                                                                    }
                                                                    if (num9 == 2)
                                                                    {
                                                                        rectangle.X = 144;
                                                                        rectangle.Y = 72;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num2 != wall && num7 == wall && (num4 != wall & num5 != wall))
                                                                    {
                                                                        if (num9 == 0)
                                                                        {
                                                                            rectangle.X = 108;
                                                                            rectangle.Y = 0;
                                                                        }
                                                                        if (num9 == 1)
                                                                        {
                                                                            rectangle.X = 126;
                                                                            rectangle.Y = 0;
                                                                        }
                                                                        if (num9 == 2)
                                                                        {
                                                                            rectangle.X = 144;
                                                                            rectangle.Y = 0;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num2 == wall && num7 != wall && (num4 != wall & num5 != wall))
                                                                        {
                                                                            if (num9 == 0)
                                                                            {
                                                                                rectangle.X = 108;
                                                                                rectangle.Y = 54;
                                                                            }
                                                                            if (num9 == 1)
                                                                            {
                                                                                rectangle.X = 126;
                                                                                rectangle.Y = 54;
                                                                            }
                                                                            if (num9 == 2)
                                                                            {
                                                                                rectangle.X = 144;
                                                                                rectangle.Y = 54;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num2 != wall && num7 != wall && (num4 != wall & num5 == wall))
                                                                            {
                                                                                if (num9 == 0)
                                                                                {
                                                                                    rectangle.X = 162;
                                                                                    rectangle.Y = 0;
                                                                                }
                                                                                if (num9 == 1)
                                                                                {
                                                                                    rectangle.X = 162;
                                                                                    rectangle.Y = 18;
                                                                                }
                                                                                if (num9 == 2)
                                                                                {
                                                                                    rectangle.X = 162;
                                                                                    rectangle.Y = 36;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num2 != wall && num7 != wall && (num4 == wall & num5 != wall))
                                                                                {
                                                                                    if (num9 == 0)
                                                                                    {
                                                                                        rectangle.X = 216;
                                                                                        rectangle.Y = 0;
                                                                                    }
                                                                                    if (num9 == 1)
                                                                                    {
                                                                                        rectangle.X = 216;
                                                                                        rectangle.Y = 18;
                                                                                    }
                                                                                    if (num9 == 2)
                                                                                    {
                                                                                        rectangle.X = 216;
                                                                                        rectangle.Y = 36;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num2 != wall && num7 != wall && (num4 != wall & num5 != wall))
                                                                                    {
                                                                                        if (num9 == 0)
                                                                                        {
                                                                                            rectangle.X = 162;
                                                                                            rectangle.Y = 54;
                                                                                        }
                                                                                        if (num9 == 1)
                                                                                        {
                                                                                            rectangle.X = 180;
                                                                                            rectangle.Y = 54;
                                                                                        }
                                                                                        if (num9 == 2)
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
                        if (num9 <= 0)
                        {
                            rectangle.X = 18;
                            rectangle.Y = 18;
                        }
                        if (num9 == 1)
                        {
                            rectangle.X = 36;
                            rectangle.Y = 18;
                        }
                        if (num9 >= 2)
                        {
                            rectangle.X = 54;
                            rectangle.Y = 18;
                        }
                    }
                    Main.tile[i, j].wallFrameX = (byte)rectangle.X;
                    Main.tile[i, j].wallFrameY = (byte)rectangle.Y;
                }
            }
        }
        
        public static void TileFrame(int i, int j, bool resetFrame = false, bool noBreak = false)
        {
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY && Main.tile[i, j] != null)
            {
                if (Main.tile[i, j].liquid > 0 && Main.netMode != 1 && !WorldGen.noLiquidCheck)
                {
                    Liquid.AddWater(i, j);
                }
                if (Main.tile[i, j].active)
                {
                    if (!noBreak || !Main.tileFrameImportant[(int)Main.tile[i, j].type])
                    {
                        int num = -1;
                        int num2 = -1;
                        int num3 = -1;
                        int num4 = -1;
                        int num5 = -1;
                        int num6 = -1;
                        int num7 = -1;
                        int num8 = -1;
                        int num9 = (int)Main.tile[i, j].type;
                        if (Main.tileStone[num9])
                        {
                            num9 = 1;
                        }
                        int frameX = (int)Main.tile[i, j].frameX;
                        int frameY = (int)Main.tile[i, j].frameY;
                        Rectangle rectangle;
                        rectangle.X = -1;
                        rectangle.Y = -1;
                        if (num9 == 3 || num9 == 24 || num9 == 61 || num9 == 71 || num9 == 73 || num9 == 74)
                        {
                            WorldGen.PlantCheck(i, j);
                        }
                        else
                        {
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
                            if (i + 1 >= Main.maxTilesX)
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
                            if (j + 1 >= Main.maxTilesY)
                            {
                                num6 = num9;
                                num7 = num9;
                                num8 = num9;
                            }
                            if (i - 1 >= 0 && Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
                            {
                                num4 = (int)Main.tile[i - 1, j].type;
                            }
                            if (i + 1 < Main.maxTilesX && Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
                            {
                                num5 = (int)Main.tile[i + 1, j].type;
                            }
                            if (j - 1 >= 0 && Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
                            {
                                num2 = (int)Main.tile[i, j - 1].type;
                            }
                            if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
                            {
                                num7 = (int)Main.tile[i, j + 1].type;
                            }
                            if (i - 1 >= 0 && j - 1 >= 0 && Main.tile[i - 1, j - 1] != null && Main.tile[i - 1, j - 1].active)
                            {
                                num = (int)Main.tile[i - 1, j - 1].type;
                            }
                            if (i + 1 < Main.maxTilesX && j - 1 >= 0 && Main.tile[i + 1, j - 1] != null && Main.tile[i + 1, j - 1].active)
                            {
                                num3 = (int)Main.tile[i + 1, j - 1].type;
                            }
                            if (i - 1 >= 0 && j + 1 < Main.maxTilesY && Main.tile[i - 1, j + 1] != null && Main.tile[i - 1, j + 1].active)
                            {
                                num6 = (int)Main.tile[i - 1, j + 1].type;
                            }
                            if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY && Main.tile[i + 1, j + 1] != null && Main.tile[i + 1, j + 1].active)
                            {
                                num8 = (int)Main.tile[i + 1, j + 1].type;
                            }
                            if (num4 >= 0 && Main.tileStone[num4])
                            {
                                num4 = 1;
                            }
                            if (num5 >= 0 && Main.tileStone[num5])
                            {
                                num5 = 1;
                            }
                            if (num2 >= 0 && Main.tileStone[num2])
                            {
                                num2 = 1;
                            }
                            if (num7 >= 0 && Main.tileStone[num7])
                            {
                                num7 = 1;
                            }
                            if (num >= 0 && Main.tileStone[num])
                            {
                                num = 1;
                            }
                            if (num3 >= 0 && Main.tileStone[num3])
                            {
                                num3 = 1;
                            }
                            if (num6 >= 0 && Main.tileStone[num6])
                            {
                                num6 = 1;
                            }
                            if (num8 >= 0 && Main.tileStone[num8])
                            {
                                num8 = 1;
                            }
                            if (num9 == 4)
                            {
                                if (num7 >= 0 && Main.tileSolid[num7] && !Main.tileNoAttach[num7])
                                {
                                    Main.tile[i, j].frameX = 0;
                                }
                                else
                                {
                                    if ((num4 >= 0 && Main.tileSolid[num4] && !Main.tileNoAttach[num4]) || (num4 == 5 && num == 5 && num6 == 5))
                                    {
                                        Main.tile[i, j].frameX = 22;
                                    }
                                    else
                                    {
                                        if ((num5 >= 0 && Main.tileSolid[num5] && !Main.tileNoAttach[num5]) || (num5 == 5 && num3 == 5 && num8 == 5))
                                        {
                                            Main.tile[i, j].frameX = 44;
                                        }
                                        else
                                        {
                                            WorldGen.KillTile(i, j, false, false, false);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (num9 == 12 || num9 == 31)
                                {
                                    if (!WorldGen.destroyObject)
                                    {
                                        int num10 = i;
                                        int num11 = j;
                                        if (Main.tile[i, j].frameX == 0)
                                        {
                                            num10 = i;
                                        }
                                        else
                                        {
                                            num10 = i - 1;
                                        }
                                        if (Main.tile[i, j].frameY == 0)
                                        {
                                            num11 = j;
                                        }
                                        else
                                        {
                                            num11 = j - 1;
                                        }
                                        if (Main.tile[num10, num11] != null && Main.tile[num10 + 1, num11] != null && Main.tile[num10, num11 + 1] != null && Main.tile[num10 + 1, num11 + 1] != null)
                                        {
                                            if (!Main.tile[num10, num11].active || (int)Main.tile[num10, num11].type != num9 || !Main.tile[num10 + 1, num11].active || (int)Main.tile[num10 + 1, num11].type != num9 || !Main.tile[num10, num11 + 1].active || (int)Main.tile[num10, num11 + 1].type != num9 || !Main.tile[num10 + 1, num11 + 1].active || (int)Main.tile[num10 + 1, num11 + 1].type != num9)
                                            {
                                                WorldGen.destroyObject = true;
                                                if ((int)Main.tile[num10, num11].type == num9)
                                                {
                                                    WorldGen.KillTile(num10, num11, false, false, false);
                                                }
                                                if ((int)Main.tile[num10 + 1, num11].type == num9)
                                                {
                                                    WorldGen.KillTile(num10 + 1, num11, false, false, false);
                                                }
                                                if ((int)Main.tile[num10, num11 + 1].type == num9)
                                                {
                                                    WorldGen.KillTile(num10, num11 + 1, false, false, false);
                                                }
                                                if ((int)Main.tile[num10 + 1, num11 + 1].type == num9)
                                                {
                                                    WorldGen.KillTile(num10 + 1, num11 + 1, false, false, false);
                                                }
                                                if (Main.netMode != 1)
                                                {
                                                    if (num9 == 12)
                                                    {
                                                        Item.NewItem(num10 * 16, num11 * 16, 32, 32, 29, 1, false);
                                                    }
                                                    else
                                                    {
                                                        if (num9 == 31)
                                                        {
                                                            if (WorldGen.genRand.Next(2) == 0)
                                                            {
                                                                WorldGen.spawnMeteor = true;
                                                            }
                                                            int num12 = Main.rand.Next(5);
                                                            if (!WorldGen.shadowOrbSmashed)
                                                            {
                                                                num12 = 0;
                                                            }
                                                            if (num12 == 0)
                                                            {
                                                                Item.NewItem(num10 * 16, num11 * 16, 32, 32, 96, 1, false);
                                                                int stack = WorldGen.genRand.Next(25, 51);
                                                                Item.NewItem(num10 * 16, num11 * 16, 32, 32, 97, stack, false);
                                                            }
                                                            else
                                                            {
                                                                if (num12 == 1)
                                                                {
                                                                    Item.NewItem(num10 * 16, num11 * 16, 32, 32, 64, 1, false);
                                                                }
                                                                else
                                                                {
                                                                    if (num12 == 2)
                                                                    {
                                                                        Item.NewItem(num10 * 16, num11 * 16, 32, 32, 162, 1, false);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num12 == 3)
                                                                        {
                                                                            Item.NewItem(num10 * 16, num11 * 16, 32, 32, 115, 1, false);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num12 == 4)
                                                                            {
                                                                                Item.NewItem(num10 * 16, num11 * 16, 32, 32, 111, 1, false);
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
                                                                for (int k = 0; k < 255; k++)
                                                                {
                                                                    float num16 = Math.Abs(Main.player[k].position.X - num13) + Math.Abs(Main.player[k].position.Y - num14);
                                                                    if (num16 < num15 || num15 == -1f)
                                                                    {
                                                                        plr = 0;
                                                                        num15 = num16;
                                                                    }
                                                                }
                                                                NPC.SpawnOnPlayer(plr, 13);
                                                            }
                                                            else
                                                            {
                                                                string text = "A horrible chill goes down your spine...";
                                                                if (WorldGen.shadowOrbCount == 2)
                                                                {
                                                                    text = "Screams echo around you...";
                                                                }
                                                                if (Main.netMode == 0)
                                                                {
                                                                    //Main.NewText(text, 50, 255, 130);
                                                                }
                                                                else
                                                                {
                                                                    if (Main.netMode == 2)
                                                                    {
                                                                        NetMessage.SendData(25, -1, -1, text, 255, 50f, 255f, 130f);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                //Main.PlaySound(13, i * 16, j * 16, 1);
                                                WorldGen.destroyObject = false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (num9 == 19)
                                    {
                                        if (num4 == num9 && num5 == num9)
                                        {
                                            if (Main.tile[i, j].frameNumber == 0)
                                            {
                                                rectangle.X = 0;
                                                rectangle.Y = 0;
                                            }
                                            if (Main.tile[i, j].frameNumber == 1)
                                            {
                                                rectangle.X = 0;
                                                rectangle.Y = 18;
                                            }
                                            if (Main.tile[i, j].frameNumber == 2)
                                            {
                                                rectangle.X = 0;
                                                rectangle.Y = 36;
                                            }
                                        }
                                        else
                                        {
                                            if (num4 == num9 && num5 == -1)
                                            {
                                                if (Main.tile[i, j].frameNumber == 0)
                                                {
                                                    rectangle.X = 18;
                                                    rectangle.Y = 0;
                                                }
                                                if (Main.tile[i, j].frameNumber == 1)
                                                {
                                                    rectangle.X = 18;
                                                    rectangle.Y = 18;
                                                }
                                                if (Main.tile[i, j].frameNumber == 2)
                                                {
                                                    rectangle.X = 18;
                                                    rectangle.Y = 36;
                                                }
                                            }
                                            else
                                            {
                                                if (num4 == -1 && num5 == num9)
                                                {
                                                    if (Main.tile[i, j].frameNumber == 0)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 0;
                                                    }
                                                    if (Main.tile[i, j].frameNumber == 1)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 18;
                                                    }
                                                    if (Main.tile[i, j].frameNumber == 2)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 36;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 != num9 && num5 == num9)
                                                    {
                                                        if (Main.tile[i, j].frameNumber == 0)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 0;
                                                        }
                                                        if (Main.tile[i, j].frameNumber == 1)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 18;
                                                        }
                                                        if (Main.tile[i, j].frameNumber == 2)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 36;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num4 == num9 && num5 != num9)
                                                        {
                                                            if (Main.tile[i, j].frameNumber == 0)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 0;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 1)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 18;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 2)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 36;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num4 != num9 && num4 != -1 && num5 == -1)
                                                            {
                                                                if (Main.tile[i, j].frameNumber == 0)
                                                                {
                                                                    rectangle.X = 108;
                                                                    rectangle.Y = 0;
                                                                }
                                                                if (Main.tile[i, j].frameNumber == 1)
                                                                {
                                                                    rectangle.X = 108;
                                                                    rectangle.Y = 18;
                                                                }
                                                                if (Main.tile[i, j].frameNumber == 2)
                                                                {
                                                                    rectangle.X = 108;
                                                                    rectangle.Y = 36;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num4 == -1 && num5 != num9 && num5 != -1)
                                                                {
                                                                    if (Main.tile[i, j].frameNumber == 0)
                                                                    {
                                                                        rectangle.X = 126;
                                                                        rectangle.Y = 0;
                                                                    }
                                                                    if (Main.tile[i, j].frameNumber == 1)
                                                                    {
                                                                        rectangle.X = 126;
                                                                        rectangle.Y = 18;
                                                                    }
                                                                    if (Main.tile[i, j].frameNumber == 2)
                                                                    {
                                                                        rectangle.X = 126;
                                                                        rectangle.Y = 36;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (Main.tile[i, j].frameNumber == 0)
                                                                    {
                                                                        rectangle.X = 90;
                                                                        rectangle.Y = 0;
                                                                    }
                                                                    if (Main.tile[i, j].frameNumber == 1)
                                                                    {
                                                                        rectangle.X = 90;
                                                                        rectangle.Y = 18;
                                                                    }
                                                                    if (Main.tile[i, j].frameNumber == 2)
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
                                                int frameY2 = (int)Main.tile[i, j].frameY;
                                                int num11 = j;
                                                bool flag = false;
                                                if (frameY2 == 0)
                                                {
                                                    num11 = j;
                                                }
                                                if (frameY2 == 18)
                                                {
                                                    num11 = j - 1;
                                                }
                                                if (frameY2 == 36)
                                                {
                                                    num11 = j - 2;
                                                }
                                                if (Main.tile[i, num11 - 1] == null)
                                                {
                                                    Main.tile[i, num11 - 1] = new Tile();
                                                }
                                                if (Main.tile[i, num11 + 3] == null)
                                                {
                                                    Main.tile[i, num11 + 3] = new Tile();
                                                }
                                                if (Main.tile[i, num11 + 2] == null)
                                                {
                                                    Main.tile[i, num11 + 2] = new Tile();
                                                }
                                                if (Main.tile[i, num11 + 1] == null)
                                                {
                                                    Main.tile[i, num11 + 1] = new Tile();
                                                }
                                                if (Main.tile[i, num11] == null)
                                                {
                                                    Main.tile[i, num11] = new Tile();
                                                }
                                                if (!Main.tile[i, num11 - 1].active || !Main.tileSolid[(int)Main.tile[i, num11 - 1].type])
                                                {
                                                    flag = true;
                                                }
                                                if (!Main.tile[i, num11 + 3].active || !Main.tileSolid[(int)Main.tile[i, num11 + 3].type])
                                                {
                                                    flag = true;
                                                }
                                                if (!Main.tile[i, num11].active || (int)Main.tile[i, num11].type != num9)
                                                {
                                                    flag = true;
                                                }
                                                if (!Main.tile[i, num11 + 1].active || (int)Main.tile[i, num11 + 1].type != num9)
                                                {
                                                    flag = true;
                                                }
                                                if (!Main.tile[i, num11 + 2].active || (int)Main.tile[i, num11 + 2].type != num9)
                                                {
                                                    flag = true;
                                                }
                                                if (flag)
                                                {
                                                    WorldGen.destroyObject = true;
                                                    WorldGen.KillTile(i, num11, false, false, false);
                                                    WorldGen.KillTile(i, num11 + 1, false, false, false);
                                                    WorldGen.KillTile(i, num11 + 2, false, false, false);
                                                    Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
                                                }
                                                WorldGen.destroyObject = false;
                                            }
                                            return;
                                        }
                                        if (num9 == 11)
                                        {
                                            if (!WorldGen.destroyObject)
                                            {
                                                int num17 = 0;
                                                int num10 = i;
                                                int num11 = j;
                                                int frameX2 = (int)Main.tile[i, j].frameX;
                                                int frameY2 = (int)Main.tile[i, j].frameY;
                                                bool flag = false;
                                                if (frameX2 == 0)
                                                {
                                                    num10 = i;
                                                    num17 = 1;
                                                }
                                                else
                                                {
                                                    if (frameX2 == 18)
                                                    {
                                                        num10 = i - 1;
                                                        num17 = 1;
                                                    }
                                                    else
                                                    {
                                                        if (frameX2 == 36)
                                                        {
                                                            num10 = i + 1;
                                                            num17 = -1;
                                                        }
                                                        else
                                                        {
                                                            if (frameX2 == 54)
                                                            {
                                                                num10 = i;
                                                                num17 = -1;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (frameY2 == 0)
                                                {
                                                    num11 = j;
                                                }
                                                else
                                                {
                                                    if (frameY2 == 18)
                                                    {
                                                        num11 = j - 1;
                                                    }
                                                    else
                                                    {
                                                        if (frameY2 == 36)
                                                        {
                                                            num11 = j - 2;
                                                        }
                                                    }
                                                }
                                                if (Main.tile[num10, num11 + 3] == null)
                                                {
                                                    Main.tile[num10, num11 + 3] = new Tile();
                                                }
                                                if (Main.tile[num10, num11 - 1] == null)
                                                {
                                                    Main.tile[num10, num11 - 1] = new Tile();
                                                }
                                                if (!Main.tile[num10, num11 - 1].active || !Main.tileSolid[(int)Main.tile[num10, num11 - 1].type] || !Main.tile[num10, num11 + 3].active || !Main.tileSolid[(int)Main.tile[num10, num11 + 3].type])
                                                {
                                                    flag = true;
                                                    WorldGen.destroyObject = true;
                                                    Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
                                                }
                                                int num18 = num10;
                                                if (num17 == -1)
                                                {
                                                    num18 = num10 - 1;
                                                }
                                                for (int l = num18; l < num18 + 2; l++)
                                                {
                                                    for (int m = num11; m < num11 + 3; m++)
                                                    {
                                                        if (!flag)
                                                        {
                                                            if (Main.tile[l, m].type != 11 || !Main.tile[l, m].active)
                                                            {
                                                                WorldGen.destroyObject = true;
                                                                Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
                                                                flag = true;
                                                                l = num18;
                                                                m = num11;
                                                            }
                                                        }
                                                        if (flag)
                                                        {
                                                            WorldGen.KillTile(l, m, false, false, false);
                                                        }
                                                    }
                                                }
                                                WorldGen.destroyObject = false;
                                            }
                                            return;
                                        }
                                        if (num9 == 34 || num9 == 35 || num9 == 36)
                                        {
                                            WorldGen.Check3x3(i, j, (int)((byte)num9));
                                            return;
                                        }
                                        if (num9 == 15 || num9 == 20)
                                        {
                                            WorldGen.Check1x2(i, j, (byte)num9);
                                            return;
                                        }
                                        if (num9 == 14 || num9 == 17 || num9 == 26 || num9 == 77)
                                        {
                                            WorldGen.Check3x2(i, j, (int)((byte)num9));
                                            return;
                                        }
                                        if (num9 == 16 || num9 == 18 || num9 == 29)
                                        {
                                            WorldGen.Check2x1(i, j, (byte)num9);
                                            return;
                                        }
                                        if (num9 == 13 || num9 == 33 || num9 == 49 || num9 == 50 || num9 == 78)
                                        {
                                            WorldGen.CheckOnTable1x1(i, j, (int)((byte)num9));
                                            return;
                                        }
                                        if (num9 == 21)
                                        {
                                            WorldGen.CheckChest(i, j, (int)((byte)num9));
                                            return;
                                        }
                                        if (num9 == 27)
                                        {
                                            WorldGen.CheckSunflower(i, j, 27);
                                            return;
                                        }
                                        if (num9 == 28)
                                        {
                                            WorldGen.CheckPot(i, j, 28);
                                            return;
                                        }
                                        if (num9 == 42)
                                        {
                                            WorldGen.Check1x2Top(i, j, (byte)num9);
                                            return;
                                        }
                                        if (num9 == 55)
                                        {
                                            WorldGen.CheckSign(i, j, num9);
                                            return;
                                        }
                                        if (num9 == 79)
                                        {
                                            WorldGen.Check4x2(i, j, num9);
                                            return;
                                        }
                                    }
                                    if (num9 == 72)
                                    {
                                        if (num7 != num9 && num7 != 70)
                                        {
                                            WorldGen.KillTile(i, j, false, false, false);
                                        }
                                        else
                                        {
                                            if (num2 != num9 && Main.tile[i, j].frameX == 0)
                                            {
                                                Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
                                                if (Main.tile[i, j].frameNumber == 0)
                                                {
                                                    Main.tile[i, j].frameX = 18;
                                                    Main.tile[i, j].frameY = 0;
                                                }
                                                if (Main.tile[i, j].frameNumber == 1)
                                                {
                                                    Main.tile[i, j].frameX = 18;
                                                    Main.tile[i, j].frameY = 18;
                                                }
                                                if (Main.tile[i, j].frameNumber == 2)
                                                {
                                                    Main.tile[i, j].frameX = 18;
                                                    Main.tile[i, j].frameY = 36;
                                                }
                                            }
                                        }
                                    }
                                    if (num9 == 5)
                                    {
                                        if (num7 == 23)
                                        {
                                            num7 = 2;
                                        }
                                        if (num7 == 60)
                                        {
                                            num7 = 2;
                                        }
                                        if (Main.tile[i, j].frameX >= 22 && Main.tile[i, j].frameX <= 44 && Main.tile[i, j].frameY >= 132 && Main.tile[i, j].frameY <= 176)
                                        {
                                            if ((num4 != num9 && num5 != num9) || num7 != 2)
                                            {
                                                WorldGen.KillTile(i, j, false, false, false);
                                            }
                                        }
                                        else
                                        {
                                            if ((Main.tile[i, j].frameX == 88 && Main.tile[i, j].frameY >= 0 && Main.tile[i, j].frameY <= 44) || (Main.tile[i, j].frameX == 66 && Main.tile[i, j].frameY >= 66 && Main.tile[i, j].frameY <= 130) || (Main.tile[i, j].frameX == 110 && Main.tile[i, j].frameY >= 66 && Main.tile[i, j].frameY <= 110) || (Main.tile[i, j].frameX == 132 && Main.tile[i, j].frameY >= 0 && Main.tile[i, j].frameY <= 176))
                                            {
                                                if (num4 == num9 && num5 == num9)
                                                {
                                                    if (Main.tile[i, j].frameNumber == 0)
                                                    {
                                                        Main.tile[i, j].frameX = 110;
                                                        Main.tile[i, j].frameY = 66;
                                                    }
                                                    if (Main.tile[i, j].frameNumber == 1)
                                                    {
                                                        Main.tile[i, j].frameX = 110;
                                                        Main.tile[i, j].frameY = 88;
                                                    }
                                                    if (Main.tile[i, j].frameNumber == 2)
                                                    {
                                                        Main.tile[i, j].frameX = 110;
                                                        Main.tile[i, j].frameY = 110;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 == num9)
                                                    {
                                                        if (Main.tile[i, j].frameNumber == 0)
                                                        {
                                                            Main.tile[i, j].frameX = 88;
                                                            Main.tile[i, j].frameY = 0;
                                                        }
                                                        if (Main.tile[i, j].frameNumber == 1)
                                                        {
                                                            Main.tile[i, j].frameX = 88;
                                                            Main.tile[i, j].frameY = 22;
                                                        }
                                                        if (Main.tile[i, j].frameNumber == 2)
                                                        {
                                                            Main.tile[i, j].frameX = 88;
                                                            Main.tile[i, j].frameY = 44;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 == num9)
                                                        {
                                                            if (Main.tile[i, j].frameNumber == 0)
                                                            {
                                                                Main.tile[i, j].frameX = 66;
                                                                Main.tile[i, j].frameY = 66;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 1)
                                                            {
                                                                Main.tile[i, j].frameX = 66;
                                                                Main.tile[i, j].frameY = 88;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 2)
                                                            {
                                                                Main.tile[i, j].frameX = 66;
                                                                Main.tile[i, j].frameY = 110;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (Main.tile[i, j].frameNumber == 0)
                                                            {
                                                                Main.tile[i, j].frameX = 0;
                                                                Main.tile[i, j].frameY = 0;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 1)
                                                            {
                                                                Main.tile[i, j].frameX = 0;
                                                                Main.tile[i, j].frameY = 22;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 2)
                                                            {
                                                                Main.tile[i, j].frameX = 0;
                                                                Main.tile[i, j].frameY = 44;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (Main.tile[i, j].frameY >= 132 && Main.tile[i, j].frameY <= 176)
                                        {
                                            if (Main.tile[i, j].frameX == 0 || Main.tile[i, j].frameX == 66 || Main.tile[i, j].frameX == 88)
                                            {
                                                if (num7 != 2)
                                                {
                                                    WorldGen.KillTile(i, j, false, false, false);
                                                }
                                                if (num4 != num9 && num5 != num9)
                                                {
                                                    if (Main.tile[i, j].frameNumber == 0)
                                                    {
                                                        Main.tile[i, j].frameX = 0;
                                                        Main.tile[i, j].frameY = 0;
                                                    }
                                                    if (Main.tile[i, j].frameNumber == 1)
                                                    {
                                                        Main.tile[i, j].frameX = 0;
                                                        Main.tile[i, j].frameY = 22;
                                                    }
                                                    if (Main.tile[i, j].frameNumber == 2)
                                                    {
                                                        Main.tile[i, j].frameX = 0;
                                                        Main.tile[i, j].frameY = 44;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 != num9)
                                                    {
                                                        if (Main.tile[i, j].frameNumber == 0)
                                                        {
                                                            Main.tile[i, j].frameX = 0;
                                                            Main.tile[i, j].frameY = 132;
                                                        }
                                                        if (Main.tile[i, j].frameNumber == 1)
                                                        {
                                                            Main.tile[i, j].frameX = 0;
                                                            Main.tile[i, j].frameY = 154;
                                                        }
                                                        if (Main.tile[i, j].frameNumber == 2)
                                                        {
                                                            Main.tile[i, j].frameX = 0;
                                                            Main.tile[i, j].frameY = 176;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 != num9)
                                                        {
                                                            if (Main.tile[i, j].frameNumber == 0)
                                                            {
                                                                Main.tile[i, j].frameX = 66;
                                                                Main.tile[i, j].frameY = 132;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 1)
                                                            {
                                                                Main.tile[i, j].frameX = 66;
                                                                Main.tile[i, j].frameY = 154;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 2)
                                                            {
                                                                Main.tile[i, j].frameX = 66;
                                                                Main.tile[i, j].frameY = 176;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (Main.tile[i, j].frameNumber == 0)
                                                            {
                                                                Main.tile[i, j].frameX = 88;
                                                                Main.tile[i, j].frameY = 132;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 1)
                                                            {
                                                                Main.tile[i, j].frameX = 88;
                                                                Main.tile[i, j].frameY = 154;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 2)
                                                            {
                                                                Main.tile[i, j].frameX = 88;
                                                                Main.tile[i, j].frameY = 176;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if ((Main.tile[i, j].frameX == 66 && (Main.tile[i, j].frameY == 0 || Main.tile[i, j].frameY == 22 || Main.tile[i, j].frameY == 44)) || (Main.tile[i, j].frameX == 88 && (Main.tile[i, j].frameY == 66 || Main.tile[i, j].frameY == 88 || Main.tile[i, j].frameY == 110)) || (Main.tile[i, j].frameX == 44 && (Main.tile[i, j].frameY == 198 || Main.tile[i, j].frameY == 220 || Main.tile[i, j].frameY == 242)) || (Main.tile[i, j].frameX == 66 && (Main.tile[i, j].frameY == 198 || Main.tile[i, j].frameY == 220 || Main.tile[i, j].frameY == 242)))
                                        {
                                            if (num4 != num9 && num5 != num9)
                                            {
                                                WorldGen.KillTile(i, j, false, false, false);
                                            }
                                        }
                                        else
                                        {
                                            if (num7 == -1 || num7 == 23)
                                            {
                                                WorldGen.KillTile(i, j, false, false, false);
                                            }
                                            else
                                            {
                                                if (num2 != num9 && Main.tile[i, j].frameY < 198)
                                                {
                                                    if ((Main.tile[i, j].frameX != 22 && Main.tile[i, j].frameX != 44) || Main.tile[i, j].frameY < 132)
                                                    {
                                                        if (num4 == num9 || num5 == num9)
                                                        {
                                                            if (num7 == num9)
                                                            {
                                                                if (num4 == num9 && num5 == num9)
                                                                {
                                                                    if (Main.tile[i, j].frameNumber == 0)
                                                                    {
                                                                        Main.tile[i, j].frameX = 132;
                                                                        Main.tile[i, j].frameY = 132;
                                                                    }
                                                                    if (Main.tile[i, j].frameNumber == 1)
                                                                    {
                                                                        Main.tile[i, j].frameX = 132;
                                                                        Main.tile[i, j].frameY = 154;
                                                                    }
                                                                    if (Main.tile[i, j].frameNumber == 2)
                                                                    {
                                                                        Main.tile[i, j].frameX = 132;
                                                                        Main.tile[i, j].frameY = 176;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num4 == num9)
                                                                    {
                                                                        if (Main.tile[i, j].frameNumber == 0)
                                                                        {
                                                                            Main.tile[i, j].frameX = 132;
                                                                            Main.tile[i, j].frameY = 0;
                                                                        }
                                                                        if (Main.tile[i, j].frameNumber == 1)
                                                                        {
                                                                            Main.tile[i, j].frameX = 132;
                                                                            Main.tile[i, j].frameY = 22;
                                                                        }
                                                                        if (Main.tile[i, j].frameNumber == 2)
                                                                        {
                                                                            Main.tile[i, j].frameX = 132;
                                                                            Main.tile[i, j].frameY = 44;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num5 == num9)
                                                                        {
                                                                            if (Main.tile[i, j].frameNumber == 0)
                                                                            {
                                                                                Main.tile[i, j].frameX = 132;
                                                                                Main.tile[i, j].frameY = 66;
                                                                            }
                                                                            if (Main.tile[i, j].frameNumber == 1)
                                                                            {
                                                                                Main.tile[i, j].frameX = 132;
                                                                                Main.tile[i, j].frameY = 88;
                                                                            }
                                                                            if (Main.tile[i, j].frameNumber == 2)
                                                                            {
                                                                                Main.tile[i, j].frameX = 132;
                                                                                Main.tile[i, j].frameY = 110;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num4 == num9 && num5 == num9)
                                                                {
                                                                    if (Main.tile[i, j].frameNumber == 0)
                                                                    {
                                                                        Main.tile[i, j].frameX = 154;
                                                                        Main.tile[i, j].frameY = 132;
                                                                    }
                                                                    if (Main.tile[i, j].frameNumber == 1)
                                                                    {
                                                                        Main.tile[i, j].frameX = 154;
                                                                        Main.tile[i, j].frameY = 154;
                                                                    }
                                                                    if (Main.tile[i, j].frameNumber == 2)
                                                                    {
                                                                        Main.tile[i, j].frameX = 154;
                                                                        Main.tile[i, j].frameY = 176;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num4 == num9)
                                                                    {
                                                                        if (Main.tile[i, j].frameNumber == 0)
                                                                        {
                                                                            Main.tile[i, j].frameX = 154;
                                                                            Main.tile[i, j].frameY = 0;
                                                                        }
                                                                        if (Main.tile[i, j].frameNumber == 1)
                                                                        {
                                                                            Main.tile[i, j].frameX = 154;
                                                                            Main.tile[i, j].frameY = 22;
                                                                        }
                                                                        if (Main.tile[i, j].frameNumber == 2)
                                                                        {
                                                                            Main.tile[i, j].frameX = 154;
                                                                            Main.tile[i, j].frameY = 44;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num5 == num9)
                                                                        {
                                                                            if (Main.tile[i, j].frameNumber == 0)
                                                                            {
                                                                                Main.tile[i, j].frameX = 154;
                                                                                Main.tile[i, j].frameY = 66;
                                                                            }
                                                                            if (Main.tile[i, j].frameNumber == 1)
                                                                            {
                                                                                Main.tile[i, j].frameX = 154;
                                                                                Main.tile[i, j].frameY = 88;
                                                                            }
                                                                            if (Main.tile[i, j].frameNumber == 2)
                                                                            {
                                                                                Main.tile[i, j].frameX = 154;
                                                                                Main.tile[i, j].frameY = 110;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (Main.tile[i, j].frameNumber == 0)
                                                            {
                                                                Main.tile[i, j].frameX = 110;
                                                                Main.tile[i, j].frameY = 0;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 1)
                                                            {
                                                                Main.tile[i, j].frameX = 110;
                                                                Main.tile[i, j].frameY = 22;
                                                            }
                                                            if (Main.tile[i, j].frameNumber == 2)
                                                            {
                                                                Main.tile[i, j].frameX = 110;
                                                                Main.tile[i, j].frameY = 44;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        rectangle.X = (int)Main.tile[i, j].frameX;
                                        rectangle.Y = (int)Main.tile[i, j].frameY;
                                    }
                                    if (!Main.tileFrameImportant[(int)Main.tile[i, j].type])
                                    {
                                        int num19 = 0;
                                        if (resetFrame)
                                        {
                                            num19 = WorldGen.genRand.Next(0, 3);
                                            Main.tile[i, j].frameNumber = (byte)num19;
                                        }
                                        else
                                        {
                                            num19 = (int)Main.tile[i, j].frameNumber;
                                        }
                                        if (num9 == 0)
                                        {
                                            for (int n = 0; n < 80; n++)
                                            {
                                                if (n == 1 || n == 6 || n == 7 || n == 8 || n == 9 || n == 22 || n == 25 || n == 37 || n == 40 || n == 53 || n == 56 || n == 59)
                                                {
                                                    if (num2 == n)
                                                    {
                                                        WorldGen.TileFrame(i, j - 1, false, false);
                                                        if (WorldGen.mergeDown)
                                                        {
                                                            num2 = num9;
                                                        }
                                                    }
                                                    if (num7 == n)
                                                    {
                                                        WorldGen.TileFrame(i, j + 1, false, false);
                                                        if (WorldGen.mergeUp)
                                                        {
                                                            num7 = num9;
                                                        }
                                                    }
                                                    if (num4 == n)
                                                    {
                                                        WorldGen.TileFrame(i - 1, j, false, false);
                                                        if (WorldGen.mergeRight)
                                                        {
                                                            num4 = num9;
                                                        }
                                                    }
                                                    if (num5 == n)
                                                    {
                                                        WorldGen.TileFrame(i + 1, j, false, false);
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
                                                    WorldGen.TileFrame(i, j - 1, false, false);
                                                    if (WorldGen.mergeDown)
                                                    {
                                                        num2 = num9;
                                                    }
                                                }
                                                if (num7 == 58)
                                                {
                                                    WorldGen.TileFrame(i, j + 1, false, false);
                                                    if (WorldGen.mergeUp)
                                                    {
                                                        num7 = num9;
                                                    }
                                                }
                                                if (num4 == 58)
                                                {
                                                    WorldGen.TileFrame(i - 1, j, false, false);
                                                    if (WorldGen.mergeRight)
                                                    {
                                                        num4 = num9;
                                                    }
                                                }
                                                if (num5 == 58)
                                                {
                                                    WorldGen.TileFrame(i + 1, j, false, false);
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
                                            }
                                        }
                                        if (num9 == 1 || num9 == 6 || num9 == 7 || num9 == 8 || num9 == 9 || num9 == 22 || num9 == 25 || num9 == 37 || num9 == 40 || num9 == 53 || num9 == 56 || num9 == 59)
                                        {
                                            for (int n = 0; n < 80; n++)
                                            {
                                                if (n == 1 || n == 6 || n == 7 || n == 8 || n == 9 || n == 22 || n == 25 || n == 37 || n == 40 || n == 53 || n == 56 || n == 59)
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
                                        if (num9 == 32)
                                        {
                                            if (num7 == 23)
                                            {
                                                num7 = num9;
                                            }
                                        }
                                        if (num9 == 69)
                                        {
                                            if (num7 == 60)
                                            {
                                                num7 = num9;
                                            }
                                        }
                                        if (num9 == 51)
                                        {
                                            if (num2 > -1 && !Main.tileNoAttach[num2])
                                            {
                                                num2 = num9;
                                            }
                                            if (num7 > -1 && !Main.tileNoAttach[num7])
                                            {
                                                num7 = num9;
                                            }
                                            if (num4 > -1 && !Main.tileNoAttach[num4])
                                            {
                                                num4 = num9;
                                            }
                                            if (num5 > -1 && !Main.tileNoAttach[num5])
                                            {
                                                num5 = num9;
                                            }
                                            if (num > -1 && !Main.tileNoAttach[num])
                                            {
                                                num = num9;
                                            }
                                            if (num3 > -1 && !Main.tileNoAttach[num3])
                                            {
                                                num3 = num9;
                                            }
                                            if (num6 > -1 && !Main.tileNoAttach[num6])
                                            {
                                                num6 = num9;
                                            }
                                            if (num8 > -1 && !Main.tileNoAttach[num8])
                                            {
                                                num8 = num9;
                                            }
                                        }
                                        if (num2 > -1 && !Main.tileSolid[num2] && num2 != num9)
                                        {
                                            num2 = -1;
                                        }
                                        if (num7 > -1 && !Main.tileSolid[num7] && num7 != num9)
                                        {
                                            num7 = -1;
                                        }
                                        if (num4 > -1 && !Main.tileSolid[num4] && num4 != num9)
                                        {
                                            num4 = -1;
                                        }
                                        if (num5 > -1 && !Main.tileSolid[num5] && num5 != num9)
                                        {
                                            num5 = -1;
                                        }
                                        if (num > -1 && !Main.tileSolid[num] && num != num9)
                                        {
                                            num = -1;
                                        }
                                        if (num3 > -1 && !Main.tileSolid[num3] && num3 != num9)
                                        {
                                            num3 = -1;
                                        }
                                        if (num6 > -1 && !Main.tileSolid[num6] && num6 != num9)
                                        {
                                            num6 = -1;
                                        }
                                        if (num8 > -1 && !Main.tileSolid[num8] && num8 != num9)
                                        {
                                            num8 = -1;
                                        }
                                        if (num9 == 2 || num9 == 23 || num9 == 60 || num9 == 70)
                                        {
                                            int num20 = 0;
                                            if (num9 == 60 || num9 == 70)
                                            {
                                                num20 = 59;
                                            }
                                            else
                                            {
                                                if (num9 == 2)
                                                {
                                                    if (num2 == 23)
                                                    {
                                                        num2 = num20;
                                                    }
                                                    if (num7 == 23)
                                                    {
                                                        num7 = num20;
                                                    }
                                                    if (num4 == 23)
                                                    {
                                                        num4 = num20;
                                                    }
                                                    if (num5 == 23)
                                                    {
                                                        num5 = num20;
                                                    }
                                                    if (num == 23)
                                                    {
                                                        num = num20;
                                                    }
                                                    if (num3 == 23)
                                                    {
                                                        num3 = num20;
                                                    }
                                                    if (num6 == 23)
                                                    {
                                                        num6 = num20;
                                                    }
                                                    if (num8 == 23)
                                                    {
                                                        num8 = num20;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num9 == 23)
                                                    {
                                                        if (num2 == 2)
                                                        {
                                                            num2 = num20;
                                                        }
                                                        if (num7 == 2)
                                                        {
                                                            num7 = num20;
                                                        }
                                                        if (num4 == 2)
                                                        {
                                                            num4 = num20;
                                                        }
                                                        if (num5 == 2)
                                                        {
                                                            num5 = num20;
                                                        }
                                                        if (num == 2)
                                                        {
                                                            num = num20;
                                                        }
                                                        if (num3 == 2)
                                                        {
                                                            num3 = num20;
                                                        }
                                                        if (num6 == 2)
                                                        {
                                                            num6 = num20;
                                                        }
                                                        if (num8 == 2)
                                                        {
                                                            num8 = num20;
                                                        }
                                                    }
                                                }
                                            }
                                            if (num2 != num9 && num2 != num20 && (num7 == num9 || num7 == num20))
                                            {
                                                if (num4 == num20 && num5 == num9)
                                                {
                                                    if (num19 == 0)
                                                    {
                                                        rectangle.X = 0;
                                                        rectangle.Y = 198;
                                                    }
                                                    if (num19 == 1)
                                                    {
                                                        rectangle.X = 18;
                                                        rectangle.Y = 198;
                                                    }
                                                    if (num19 == 2)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 198;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 == num9 && num5 == num20)
                                                    {
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 198;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 72;
                                                            rectangle.Y = 198;
                                                        }
                                                        if (num19 == 2)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 198;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (num7 != num9 && num7 != num20 && (num2 == num9 || num2 == num20))
                                                {
                                                    if (num4 == num20 && num5 == num9)
                                                    {
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 0;
                                                            rectangle.Y = 216;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 18;
                                                            rectangle.Y = 216;
                                                        }
                                                        if (num19 == 2)
                                                        {
                                                            rectangle.X = 36;
                                                            rectangle.Y = 216;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num4 == num9 && num5 == num20)
                                                        {
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 54;
                                                                rectangle.Y = 216;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 216;
                                                            }
                                                            if (num19 == 2)
                                                            {
                                                                rectangle.X = 90;
                                                                rectangle.Y = 216;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 != num9 && num4 != num20 && (num5 == num9 || num5 == num20))
                                                    {
                                                        if (num2 == num20 && num7 == num9)
                                                        {
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 144;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 162;
                                                            }
                                                            if (num19 == 2)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 180;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num7 == num9 && num5 == num2)
                                                            {
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 72;
                                                                    rectangle.Y = 90;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 72;
                                                                    rectangle.Y = 108;
                                                                }
                                                                if (num19 == 2)
                                                                {
                                                                    rectangle.X = 72;
                                                                    rectangle.Y = 126;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 != num9 && num5 != num20 && (num4 == num9 || num4 == num20))
                                                        {
                                                            if (num2 == num20 && num7 == num9)
                                                            {
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 90;
                                                                    rectangle.Y = 144;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 90;
                                                                    rectangle.Y = 162;
                                                                }
                                                                if (num19 == 2)
                                                                {
                                                                    rectangle.X = 90;
                                                                    rectangle.Y = 180;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num7 == num9 && num5 == num2)
                                                                {
                                                                    if (num19 == 0)
                                                                    {
                                                                        rectangle.X = 90;
                                                                        rectangle.Y = 90;
                                                                    }
                                                                    if (num19 == 1)
                                                                    {
                                                                        rectangle.X = 90;
                                                                        rectangle.Y = 108;
                                                                    }
                                                                    if (num19 == 2)
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
                                                                    if (num8 == num20)
                                                                    {
                                                                        if (num19 == 0)
                                                                        {
                                                                            rectangle.X = 108;
                                                                            rectangle.Y = 324;
                                                                        }
                                                                        if (num19 == 1)
                                                                        {
                                                                            rectangle.X = 126;
                                                                            rectangle.Y = 324;
                                                                        }
                                                                        if (num19 == 2)
                                                                        {
                                                                            rectangle.X = 144;
                                                                            rectangle.Y = 324;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num3 == num20)
                                                                        {
                                                                            if (num19 == 0)
                                                                            {
                                                                                rectangle.X = 108;
                                                                                rectangle.Y = 342;
                                                                            }
                                                                            if (num19 == 1)
                                                                            {
                                                                                rectangle.X = 126;
                                                                                rectangle.Y = 342;
                                                                            }
                                                                            if (num19 == 2)
                                                                            {
                                                                                rectangle.X = 144;
                                                                                rectangle.Y = 342;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num6 == num20)
                                                                            {
                                                                                if (num19 == 0)
                                                                                {
                                                                                    rectangle.X = 108;
                                                                                    rectangle.Y = 360;
                                                                                }
                                                                                if (num19 == 1)
                                                                                {
                                                                                    rectangle.X = 126;
                                                                                    rectangle.Y = 360;
                                                                                }
                                                                                if (num19 == 2)
                                                                                {
                                                                                    rectangle.X = 144;
                                                                                    rectangle.Y = 360;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num == num20)
                                                                                {
                                                                                    if (num19 == 0)
                                                                                    {
                                                                                        rectangle.X = 108;
                                                                                        rectangle.Y = 378;
                                                                                    }
                                                                                    if (num19 == 1)
                                                                                    {
                                                                                        rectangle.X = 126;
                                                                                        rectangle.Y = 378;
                                                                                    }
                                                                                    if (num19 == 2)
                                                                                    {
                                                                                        rectangle.X = 144;
                                                                                        rectangle.Y = 378;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num19 == 0)
                                                                                    {
                                                                                        rectangle.X = 144;
                                                                                        rectangle.Y = 234;
                                                                                    }
                                                                                    if (num19 == 1)
                                                                                    {
                                                                                        rectangle.X = 198;
                                                                                        rectangle.Y = 234;
                                                                                    }
                                                                                    if (num19 == 2)
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
                                                                        if (num19 == 0)
                                                                        {
                                                                            rectangle.X = 36;
                                                                            rectangle.Y = 306;
                                                                        }
                                                                        if (num19 == 1)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 306;
                                                                        }
                                                                        if (num19 == 2)
                                                                        {
                                                                            rectangle.X = 72;
                                                                            rectangle.Y = 306;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num3 != num9 && num6 != num9)
                                                                        {
                                                                            if (num19 == 0)
                                                                            {
                                                                                rectangle.X = 90;
                                                                                rectangle.Y = 306;
                                                                            }
                                                                            if (num19 == 1)
                                                                            {
                                                                                rectangle.X = 108;
                                                                                rectangle.Y = 306;
                                                                            }
                                                                            if (num19 == 2)
                                                                            {
                                                                                rectangle.X = 126;
                                                                                rectangle.Y = 306;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num != num9 && num3 == num9 && num6 == num9 && num8 == num9)
                                                                            {
                                                                                if (num19 == 0)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 108;
                                                                                }
                                                                                if (num19 == 1)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 144;
                                                                                }
                                                                                if (num19 == 2)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 180;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num == num9 && num3 != num9 && num6 == num9 && num8 == num9)
                                                                                {
                                                                                    if (num19 == 0)
                                                                                    {
                                                                                        rectangle.X = 36;
                                                                                        rectangle.Y = 108;
                                                                                    }
                                                                                    if (num19 == 1)
                                                                                    {
                                                                                        rectangle.X = 36;
                                                                                        rectangle.Y = 144;
                                                                                    }
                                                                                    if (num19 == 2)
                                                                                    {
                                                                                        rectangle.X = 36;
                                                                                        rectangle.Y = 180;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num == num9 && num3 == num9 && num6 != num9 && num8 == num9)
                                                                                    {
                                                                                        if (num19 == 0)
                                                                                        {
                                                                                            rectangle.X = 54;
                                                                                            rectangle.Y = 90;
                                                                                        }
                                                                                        if (num19 == 1)
                                                                                        {
                                                                                            rectangle.X = 54;
                                                                                            rectangle.Y = 126;
                                                                                        }
                                                                                        if (num19 == 2)
                                                                                        {
                                                                                            rectangle.X = 54;
                                                                                            rectangle.Y = 162;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (num == num9 && num3 == num9 && num6 == num9 && num8 != num9)
                                                                                        {
                                                                                            if (num19 == 0)
                                                                                            {
                                                                                                rectangle.X = 36;
                                                                                                rectangle.Y = 90;
                                                                                            }
                                                                                            if (num19 == 1)
                                                                                            {
                                                                                                rectangle.X = 36;
                                                                                                rectangle.Y = 126;
                                                                                            }
                                                                                            if (num19 == 2)
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
                                                                if (num2 == num9 && num7 == num20 && num4 == num9 && num5 == num9 && num == -1 && num3 == -1)
                                                                {
                                                                    if (num19 == 0)
                                                                    {
                                                                        rectangle.X = 108;
                                                                        rectangle.Y = 18;
                                                                    }
                                                                    if (num19 == 1)
                                                                    {
                                                                        rectangle.X = 126;
                                                                        rectangle.Y = 18;
                                                                    }
                                                                    if (num19 == 2)
                                                                    {
                                                                        rectangle.X = 144;
                                                                        rectangle.Y = 18;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num2 == num20 && num7 == num9 && num4 == num9 && num5 == num9 && num6 == -1 && num8 == -1)
                                                                    {
                                                                        if (num19 == 0)
                                                                        {
                                                                            rectangle.X = 108;
                                                                            rectangle.Y = 36;
                                                                        }
                                                                        if (num19 == 1)
                                                                        {
                                                                            rectangle.X = 126;
                                                                            rectangle.Y = 36;
                                                                        }
                                                                        if (num19 == 2)
                                                                        {
                                                                            rectangle.X = 144;
                                                                            rectangle.Y = 36;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num2 == num9 && num7 == num9 && num4 == num20 && num5 == num9 && num3 == -1 && num8 == -1)
                                                                        {
                                                                            if (num19 == 0)
                                                                            {
                                                                                rectangle.X = 198;
                                                                                rectangle.Y = 0;
                                                                            }
                                                                            if (num19 == 1)
                                                                            {
                                                                                rectangle.X = 198;
                                                                                rectangle.Y = 18;
                                                                            }
                                                                            if (num19 == 2)
                                                                            {
                                                                                rectangle.X = 198;
                                                                                rectangle.Y = 36;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num20 && num == -1 && num6 == -1)
                                                                            {
                                                                                if (num19 == 0)
                                                                                {
                                                                                    rectangle.X = 180;
                                                                                    rectangle.Y = 0;
                                                                                }
                                                                                if (num19 == 1)
                                                                                {
                                                                                    rectangle.X = 180;
                                                                                    rectangle.Y = 18;
                                                                                }
                                                                                if (num19 == 2)
                                                                                {
                                                                                    rectangle.X = 180;
                                                                                    rectangle.Y = 36;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num2 == num9 && num7 == num20 && num4 == num9 && num5 == num9)
                                                                                {
                                                                                    if (num3 != -1)
                                                                                    {
                                                                                        if (num19 == 0)
                                                                                        {
                                                                                            rectangle.X = 54;
                                                                                            rectangle.Y = 108;
                                                                                        }
                                                                                        if (num19 == 1)
                                                                                        {
                                                                                            rectangle.X = 54;
                                                                                            rectangle.Y = 144;
                                                                                        }
                                                                                        if (num19 == 2)
                                                                                        {
                                                                                            rectangle.X = 54;
                                                                                            rectangle.Y = 180;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (num != -1)
                                                                                        {
                                                                                            if (num19 == 0)
                                                                                            {
                                                                                                rectangle.X = 36;
                                                                                                rectangle.Y = 108;
                                                                                            }
                                                                                            if (num19 == 1)
                                                                                            {
                                                                                                rectangle.X = 36;
                                                                                                rectangle.Y = 144;
                                                                                            }
                                                                                            if (num19 == 2)
                                                                                            {
                                                                                                rectangle.X = 36;
                                                                                                rectangle.Y = 180;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num2 == num20 && num7 == num9 && num4 == num9 && num5 == num9)
                                                                                    {
                                                                                        if (num8 != -1)
                                                                                        {
                                                                                            if (num19 == 0)
                                                                                            {
                                                                                                rectangle.X = 54;
                                                                                                rectangle.Y = 90;
                                                                                            }
                                                                                            if (num19 == 1)
                                                                                            {
                                                                                                rectangle.X = 54;
                                                                                                rectangle.Y = 126;
                                                                                            }
                                                                                            if (num19 == 2)
                                                                                            {
                                                                                                rectangle.X = 54;
                                                                                                rectangle.Y = 162;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (num6 != -1)
                                                                                            {
                                                                                                if (num19 == 0)
                                                                                                {
                                                                                                    rectangle.X = 36;
                                                                                                    rectangle.Y = 90;
                                                                                                }
                                                                                                if (num19 == 1)
                                                                                                {
                                                                                                    rectangle.X = 36;
                                                                                                    rectangle.Y = 126;
                                                                                                }
                                                                                                if (num19 == 2)
                                                                                                {
                                                                                                    rectangle.X = 36;
                                                                                                    rectangle.Y = 162;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num20)
                                                                                        {
                                                                                            if (num != -1)
                                                                                            {
                                                                                                if (num19 == 0)
                                                                                                {
                                                                                                    rectangle.X = 54;
                                                                                                    rectangle.Y = 90;
                                                                                                }
                                                                                                if (num19 == 1)
                                                                                                {
                                                                                                    rectangle.X = 54;
                                                                                                    rectangle.Y = 126;
                                                                                                }
                                                                                                if (num19 == 2)
                                                                                                {
                                                                                                    rectangle.X = 54;
                                                                                                    rectangle.Y = 162;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (num6 != -1)
                                                                                                {
                                                                                                    if (num19 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 54;
                                                                                                        rectangle.Y = 108;
                                                                                                    }
                                                                                                    if (num19 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 54;
                                                                                                        rectangle.Y = 144;
                                                                                                    }
                                                                                                    if (num19 == 2)
                                                                                                    {
                                                                                                        rectangle.X = 54;
                                                                                                        rectangle.Y = 180;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (num2 == num9 && num7 == num9 && num4 == num20 && num5 == num9)
                                                                                            {
                                                                                                if (num3 != -1)
                                                                                                {
                                                                                                    if (num19 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 36;
                                                                                                        rectangle.Y = 90;
                                                                                                    }
                                                                                                    if (num19 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 36;
                                                                                                        rectangle.Y = 126;
                                                                                                    }
                                                                                                    if (num19 == 2)
                                                                                                    {
                                                                                                        rectangle.X = 36;
                                                                                                        rectangle.Y = 162;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (num8 != -1)
                                                                                                    {
                                                                                                        if (num19 == 0)
                                                                                                        {
                                                                                                            rectangle.X = 36;
                                                                                                            rectangle.Y = 108;
                                                                                                        }
                                                                                                        if (num19 == 1)
                                                                                                        {
                                                                                                            rectangle.X = 36;
                                                                                                            rectangle.Y = 144;
                                                                                                        }
                                                                                                        if (num19 == 2)
                                                                                                        {
                                                                                                            rectangle.X = 36;
                                                                                                            rectangle.Y = 180;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if ((num2 == num20 && num7 == num9 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num20 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num20 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num20))
                                                                                                {
                                                                                                    if (num19 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 18;
                                                                                                        rectangle.Y = 18;
                                                                                                    }
                                                                                                    if (num19 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 36;
                                                                                                        rectangle.Y = 18;
                                                                                                    }
                                                                                                    if (num19 == 2)
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
                                            if ((num2 == num9 || num2 == num20) && (num7 == num9 || num7 == num20) && (num4 == num9 || num4 == num20) && (num5 == num9 || num5 == num20))
                                            {
                                                if (num != num9 && num != num20 && (num3 == num9 || num3 == num20) && (num6 == num9 || num6 == num20) && (num8 == num9 || num8 == num20))
                                                {
                                                    if (num19 == 0)
                                                    {
                                                        rectangle.X = 54;
                                                        rectangle.Y = 108;
                                                    }
                                                    if (num19 == 1)
                                                    {
                                                        rectangle.X = 54;
                                                        rectangle.Y = 144;
                                                    }
                                                    if (num19 == 2)
                                                    {
                                                        rectangle.X = 54;
                                                        rectangle.Y = 180;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num3 != num9 && num3 != num20 && (num == num9 || num == num20) && (num6 == num9 || num6 == num20) && (num8 == num9 || num8 == num20))
                                                    {
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 36;
                                                            rectangle.Y = 108;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 36;
                                                            rectangle.Y = 144;
                                                        }
                                                        if (num19 == 2)
                                                        {
                                                            rectangle.X = 36;
                                                            rectangle.Y = 180;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num6 != num9 && num6 != num20 && (num == num9 || num == num20) && (num3 == num9 || num3 == num20) && (num8 == num9 || num8 == num20))
                                                        {
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 54;
                                                                rectangle.Y = 90;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 54;
                                                                rectangle.Y = 126;
                                                            }
                                                            if (num19 == 2)
                                                            {
                                                                rectangle.X = 54;
                                                                rectangle.Y = 162;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num8 != num9 && num8 != num20 && (num == num9 || num == num20) && (num6 == num9 || num6 == num20) && (num3 == num9 || num3 == num20))
                                                            {
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 36;
                                                                    rectangle.Y = 90;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 36;
                                                                    rectangle.Y = 126;
                                                                }
                                                                if (num19 == 2)
                                                                {
                                                                    rectangle.X = 36;
                                                                    rectangle.Y = 162;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (num2 != num20 && num2 != num9 && num7 == num9 && num4 != num20 && num4 != num9 && num5 == num9 && num8 != num20 && num8 != num9)
                                            {
                                                if (num19 == 0)
                                                {
                                                    rectangle.X = 90;
                                                    rectangle.Y = 270;
                                                }
                                                if (num19 == 1)
                                                {
                                                    rectangle.X = 108;
                                                    rectangle.Y = 270;
                                                }
                                                if (num19 == 2)
                                                {
                                                    rectangle.X = 126;
                                                    rectangle.Y = 270;
                                                }
                                            }
                                            else
                                            {
                                                if (num2 != num20 && num2 != num9 && num7 == num9 && num4 == num9 && num5 != num20 && num5 != num9 && num6 != num20 && num6 != num9)
                                                {
                                                    if (num19 == 0)
                                                    {
                                                        rectangle.X = 144;
                                                        rectangle.Y = 270;
                                                    }
                                                    if (num19 == 1)
                                                    {
                                                        rectangle.X = 162;
                                                        rectangle.Y = 270;
                                                    }
                                                    if (num19 == 2)
                                                    {
                                                        rectangle.X = 180;
                                                        rectangle.Y = 270;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num7 != num20 && num7 != num9 && num2 == num9 && num4 != num20 && num4 != num9 && num5 == num9 && num3 != num20 && num3 != num9)
                                                    {
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 288;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 108;
                                                            rectangle.Y = 288;
                                                        }
                                                        if (num19 == 2)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 288;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num7 != num20 && num7 != num9 && num2 == num9 && num4 == num9 && num5 != num20 && num5 != num9 && num != num20 && num != num9)
                                                        {
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 144;
                                                                rectangle.Y = 288;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 162;
                                                                rectangle.Y = 288;
                                                            }
                                                            if (num19 == 2)
                                                            {
                                                                rectangle.X = 180;
                                                                rectangle.Y = 288;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num2 != num9 && num2 != num20 && num7 == num9 && num4 == num9 && num5 == num9 && num6 != num9 && num6 != num20 && num8 != num9 && num8 != num20)
                                                            {
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 144;
                                                                    rectangle.Y = 216;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 198;
                                                                    rectangle.Y = 216;
                                                                }
                                                                if (num19 == 2)
                                                                {
                                                                    rectangle.X = 252;
                                                                    rectangle.Y = 216;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num7 != num9 && num7 != num20 && num2 == num9 && num4 == num9 && num5 == num9 && num != num9 && num != num20 && num3 != num9 && num3 != num20)
                                                                {
                                                                    if (num19 == 0)
                                                                    {
                                                                        rectangle.X = 144;
                                                                        rectangle.Y = 252;
                                                                    }
                                                                    if (num19 == 1)
                                                                    {
                                                                        rectangle.X = 198;
                                                                        rectangle.Y = 252;
                                                                    }
                                                                    if (num19 == 2)
                                                                    {
                                                                        rectangle.X = 252;
                                                                        rectangle.Y = 252;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num4 != num9 && num4 != num20 && num7 == num9 && num2 == num9 && num5 == num9 && num3 != num9 && num3 != num20 && num8 != num9 && num8 != num20)
                                                                    {
                                                                        if (num19 == 0)
                                                                        {
                                                                            rectangle.X = 126;
                                                                            rectangle.Y = 234;
                                                                        }
                                                                        if (num19 == 1)
                                                                        {
                                                                            rectangle.X = 180;
                                                                            rectangle.Y = 234;
                                                                        }
                                                                        if (num19 == 2)
                                                                        {
                                                                            rectangle.X = 234;
                                                                            rectangle.Y = 234;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num5 != num9 && num5 != num20 && num7 == num9 && num2 == num9 && num4 == num9 && num != num9 && num != num20 && num6 != num9 && num6 != num20)
                                                                        {
                                                                            if (num19 == 0)
                                                                            {
                                                                                rectangle.X = 162;
                                                                                rectangle.Y = 234;
                                                                            }
                                                                            if (num19 == 1)
                                                                            {
                                                                                rectangle.X = 216;
                                                                                rectangle.Y = 234;
                                                                            }
                                                                            if (num19 == 2)
                                                                            {
                                                                                rectangle.X = 270;
                                                                                rectangle.Y = 234;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num2 != num20 && num2 != num9 && (num7 == num20 || num7 == num9) && num4 == num20 && num5 == num20)
                                                                            {
                                                                                if (num19 == 0)
                                                                                {
                                                                                    rectangle.X = 36;
                                                                                    rectangle.Y = 270;
                                                                                }
                                                                                if (num19 == 1)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 270;
                                                                                }
                                                                                if (num19 == 2)
                                                                                {
                                                                                    rectangle.X = 72;
                                                                                    rectangle.Y = 270;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num7 != num20 && num7 != num9 && (num2 == num20 || num2 == num9) && num4 == num20 && num5 == num20)
                                                                                {
                                                                                    if (num19 == 0)
                                                                                    {
                                                                                        rectangle.X = 36;
                                                                                        rectangle.Y = 288;
                                                                                    }
                                                                                    if (num19 == 1)
                                                                                    {
                                                                                        rectangle.X = 54;
                                                                                        rectangle.Y = 288;
                                                                                    }
                                                                                    if (num19 == 2)
                                                                                    {
                                                                                        rectangle.X = 72;
                                                                                        rectangle.Y = 288;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num4 != num20 && num4 != num9 && (num5 == num20 || num5 == num9) && num2 == num20 && num7 == num20)
                                                                                    {
                                                                                        if (num19 == 0)
                                                                                        {
                                                                                            rectangle.X = 0;
                                                                                            rectangle.Y = 270;
                                                                                        }
                                                                                        if (num19 == 1)
                                                                                        {
                                                                                            rectangle.X = 0;
                                                                                            rectangle.Y = 288;
                                                                                        }
                                                                                        if (num19 == 2)
                                                                                        {
                                                                                            rectangle.X = 0;
                                                                                            rectangle.Y = 306;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (num5 != num20 && num5 != num9 && (num4 == num20 || num4 == num9) && num2 == num20 && num7 == num20)
                                                                                        {
                                                                                            if (num19 == 0)
                                                                                            {
                                                                                                rectangle.X = 18;
                                                                                                rectangle.Y = 270;
                                                                                            }
                                                                                            if (num19 == 1)
                                                                                            {
                                                                                                rectangle.X = 18;
                                                                                                rectangle.Y = 288;
                                                                                            }
                                                                                            if (num19 == 2)
                                                                                            {
                                                                                                rectangle.X = 18;
                                                                                                rectangle.Y = 306;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (num2 == num9 && num7 == num20 && num4 == num20 && num5 == num20)
                                                                                            {
                                                                                                if (num19 == 0)
                                                                                                {
                                                                                                    rectangle.X = 198;
                                                                                                    rectangle.Y = 288;
                                                                                                }
                                                                                                if (num19 == 1)
                                                                                                {
                                                                                                    rectangle.X = 216;
                                                                                                    rectangle.Y = 288;
                                                                                                }
                                                                                                if (num19 == 2)
                                                                                                {
                                                                                                    rectangle.X = 234;
                                                                                                    rectangle.Y = 288;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (num2 == num20 && num7 == num9 && num4 == num20 && num5 == num20)
                                                                                                {
                                                                                                    if (num19 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 198;
                                                                                                        rectangle.Y = 270;
                                                                                                    }
                                                                                                    if (num19 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 216;
                                                                                                        rectangle.Y = 270;
                                                                                                    }
                                                                                                    if (num19 == 2)
                                                                                                    {
                                                                                                        rectangle.X = 234;
                                                                                                        rectangle.Y = 270;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (num2 == num20 && num7 == num20 && num4 == num9 && num5 == num20)
                                                                                                    {
                                                                                                        if (num19 == 0)
                                                                                                        {
                                                                                                            rectangle.X = 198;
                                                                                                            rectangle.Y = 306;
                                                                                                        }
                                                                                                        if (num19 == 1)
                                                                                                        {
                                                                                                            rectangle.X = 216;
                                                                                                            rectangle.Y = 306;
                                                                                                        }
                                                                                                        if (num19 == 2)
                                                                                                        {
                                                                                                            rectangle.X = 234;
                                                                                                            rectangle.Y = 306;
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (num2 == num20 && num7 == num20 && num4 == num20 && num5 == num9)
                                                                                                        {
                                                                                                            if (num19 == 0)
                                                                                                            {
                                                                                                                rectangle.X = 144;
                                                                                                                rectangle.Y = 306;
                                                                                                            }
                                                                                                            if (num19 == 1)
                                                                                                            {
                                                                                                                rectangle.X = 162;
                                                                                                                rectangle.Y = 306;
                                                                                                            }
                                                                                                            if (num19 == 2)
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
                                            if (num2 != num9 && num2 != num20 && num7 == num9 && num4 == num9 && num5 == num9)
                                            {
                                                if ((num6 == num20 || num6 == num9) && num8 != num20 && num8 != num9)
                                                {
                                                    if (num19 == 0)
                                                    {
                                                        rectangle.X = 0;
                                                        rectangle.Y = 324;
                                                    }
                                                    if (num19 == 1)
                                                    {
                                                        rectangle.X = 18;
                                                        rectangle.Y = 324;
                                                    }
                                                    if (num19 == 2)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 324;
                                                    }
                                                }
                                                else
                                                {
                                                    if ((num8 == num20 || num8 == num9) && num6 != num20 && num6 != num9)
                                                    {
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 324;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 72;
                                                            rectangle.Y = 324;
                                                        }
                                                        if (num19 == 2)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 324;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (num7 != num9 && num7 != num20 && num2 == num9 && num4 == num9 && num5 == num9)
                                                {
                                                    if ((num == num20 || num == num9) && num3 != num20 && num3 != num9)
                                                    {
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 0;
                                                            rectangle.Y = 342;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 18;
                                                            rectangle.Y = 342;
                                                        }
                                                        if (num19 == 2)
                                                        {
                                                            rectangle.X = 36;
                                                            rectangle.Y = 342;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((num3 == num20 || num3 == num9) && num != num20 && num != num9)
                                                        {
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 54;
                                                                rectangle.Y = 342;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 342;
                                                            }
                                                            if (num19 == 2)
                                                            {
                                                                rectangle.X = 90;
                                                                rectangle.Y = 342;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 != num9 && num4 != num20 && num2 == num9 && num7 == num9 && num5 == num9)
                                                    {
                                                        if ((num3 == num20 || num3 == num9) && num8 != num20 && num8 != num9)
                                                        {
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 54;
                                                                rectangle.Y = 360;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 360;
                                                            }
                                                            if (num19 == 2)
                                                            {
                                                                rectangle.X = 90;
                                                                rectangle.Y = 360;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if ((num8 == num20 || num8 == num9) && num3 != num20 && num3 != num9)
                                                            {
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 0;
                                                                    rectangle.Y = 360;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 18;
                                                                    rectangle.Y = 360;
                                                                }
                                                                if (num19 == 2)
                                                                {
                                                                    rectangle.X = 36;
                                                                    rectangle.Y = 360;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 != num9 && num5 != num20 && num2 == num9 && num7 == num9 && num4 == num9)
                                                        {
                                                            if ((num == num20 || num == num9) && num6 != num20 && num6 != num9)
                                                            {
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 0;
                                                                    rectangle.Y = 378;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 18;
                                                                    rectangle.Y = 378;
                                                                }
                                                                if (num19 == 2)
                                                                {
                                                                    rectangle.X = 36;
                                                                    rectangle.Y = 378;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if ((num6 == num20 || num6 == num9) && num != num20 && num != num9)
                                                                {
                                                                    if (num19 == 0)
                                                                    {
                                                                        rectangle.X = 54;
                                                                        rectangle.Y = 378;
                                                                    }
                                                                    if (num19 == 1)
                                                                    {
                                                                        rectangle.X = 72;
                                                                        rectangle.Y = 378;
                                                                    }
                                                                    if (num19 == 2)
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
                                            if ((num2 == num9 || num2 == num20) && (num7 == num9 || num7 == num20) && (num4 == num9 || num4 == num20) && (num5 == num9 || num5 == num20) && num != -1 && num3 != -1 && num6 != -1 && num8 != -1)
                                            {
                                                if (num19 == 0)
                                                {
                                                    rectangle.X = 18;
                                                    rectangle.Y = 18;
                                                }
                                                if (num19 == 1)
                                                {
                                                    rectangle.X = 36;
                                                    rectangle.Y = 18;
                                                }
                                                if (num19 == 2)
                                                {
                                                    rectangle.X = 54;
                                                    rectangle.Y = 18;
                                                }
                                            }
                                            if (num2 == num20)
                                            {
                                                num2 = -2;
                                            }
                                            if (num7 == num20)
                                            {
                                                num7 = -2;
                                            }
                                            if (num4 == num20)
                                            {
                                                num4 = -2;
                                            }
                                            if (num5 == num20)
                                            {
                                                num5 = -2;
                                            }
                                            if (num == num20)
                                            {
                                                num = -2;
                                            }
                                            if (num3 == num20)
                                            {
                                                num3 = -2;
                                            }
                                            if (num6 == num20)
                                            {
                                                num6 = -2;
                                            }
                                            if (num8 == num20)
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
                                                    if (num19 == 0)
                                                    {
                                                        rectangle.X = 144;
                                                        rectangle.Y = 108;
                                                    }
                                                    if (num19 == 1)
                                                    {
                                                        rectangle.X = 162;
                                                        rectangle.Y = 108;
                                                    }
                                                    if (num19 == 2)
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
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 144;
                                                            rectangle.Y = 90;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 162;
                                                            rectangle.Y = 90;
                                                        }
                                                        if (num19 == 2)
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
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 162;
                                                                rectangle.Y = 126;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 162;
                                                                rectangle.Y = 144;
                                                            }
                                                            if (num19 == 2)
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
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 144;
                                                                    rectangle.Y = 126;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 144;
                                                                    rectangle.Y = 144;
                                                                }
                                                                if (num19 == 2)
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
                                                                    if (num19 == 0)
                                                                    {
                                                                        rectangle.X = 36;
                                                                        rectangle.Y = 90;
                                                                    }
                                                                    if (num19 == 1)
                                                                    {
                                                                        rectangle.X = 36;
                                                                        rectangle.Y = 126;
                                                                    }
                                                                    if (num19 == 2)
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
                                                                        if (num19 == 0)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 90;
                                                                        }
                                                                        if (num19 == 1)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 126;
                                                                        }
                                                                        if (num19 == 2)
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
                                                                            if (num19 == 0)
                                                                            {
                                                                                rectangle.X = 36;
                                                                                rectangle.Y = 108;
                                                                            }
                                                                            if (num19 == 1)
                                                                            {
                                                                                rectangle.X = 36;
                                                                                rectangle.Y = 144;
                                                                            }
                                                                            if (num19 == 2)
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
                                                                                if (num19 == 0)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 108;
                                                                                }
                                                                                if (num19 == 1)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 144;
                                                                                }
                                                                                if (num19 == 2)
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
                                                                                    if (num19 == 0)
                                                                                    {
                                                                                        rectangle.X = 180;
                                                                                        rectangle.Y = 126;
                                                                                    }
                                                                                    if (num19 == 1)
                                                                                    {
                                                                                        rectangle.X = 180;
                                                                                        rectangle.Y = 144;
                                                                                    }
                                                                                    if (num19 == 2)
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
                                                                                        if (num19 == 0)
                                                                                        {
                                                                                            rectangle.X = 144;
                                                                                            rectangle.Y = 180;
                                                                                        }
                                                                                        if (num19 == 1)
                                                                                        {
                                                                                            rectangle.X = 162;
                                                                                            rectangle.Y = 180;
                                                                                        }
                                                                                        if (num19 == 2)
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
                                                                                            if (num19 == 0)
                                                                                            {
                                                                                                rectangle.X = 198;
                                                                                                rectangle.Y = 90;
                                                                                            }
                                                                                            if (num19 == 1)
                                                                                            {
                                                                                                rectangle.X = 198;
                                                                                                rectangle.Y = 108;
                                                                                            }
                                                                                            if (num19 == 2)
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
                                                                                                if (num19 == 0)
                                                                                                {
                                                                                                    rectangle.X = 198;
                                                                                                    rectangle.Y = 144;
                                                                                                }
                                                                                                if (num19 == 1)
                                                                                                {
                                                                                                    rectangle.X = 198;
                                                                                                    rectangle.Y = 162;
                                                                                                }
                                                                                                if (num19 == 2)
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
                                                                                                    if (num19 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 216;
                                                                                                        rectangle.Y = 144;
                                                                                                    }
                                                                                                    if (num19 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 216;
                                                                                                        rectangle.Y = 162;
                                                                                                    }
                                                                                                    if (num19 == 2)
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
                                                                                                        if (num19 == 0)
                                                                                                        {
                                                                                                            rectangle.X = 216;
                                                                                                            rectangle.Y = 90;
                                                                                                        }
                                                                                                        if (num19 == 1)
                                                                                                        {
                                                                                                            rectangle.X = 216;
                                                                                                            rectangle.Y = 108;
                                                                                                        }
                                                                                                        if (num19 == 2)
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
                                                                                                            if (num19 == 0)
                                                                                                            {
                                                                                                                rectangle.X = 108;
                                                                                                                rectangle.Y = 198;
                                                                                                            }
                                                                                                            if (num19 == 1)
                                                                                                            {
                                                                                                                rectangle.X = 126;
                                                                                                                rectangle.Y = 198;
                                                                                                            }
                                                                                                            if (num19 == 2)
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
                                                                                                                    if (num19 == 0)
                                                                                                                    {
                                                                                                                        rectangle.X = 18;
                                                                                                                        rectangle.Y = 108;
                                                                                                                    }
                                                                                                                    if (num19 == 1)
                                                                                                                    {
                                                                                                                        rectangle.X = 18;
                                                                                                                        rectangle.Y = 144;
                                                                                                                    }
                                                                                                                    if (num19 == 2)
                                                                                                                    {
                                                                                                                        rectangle.X = 18;
                                                                                                                        rectangle.Y = 180;
                                                                                                                    }
                                                                                                                }
                                                                                                                if (num3 == -2)
                                                                                                                {
                                                                                                                    if (num19 == 0)
                                                                                                                    {
                                                                                                                        rectangle.X = 0;
                                                                                                                        rectangle.Y = 108;
                                                                                                                    }
                                                                                                                    if (num19 == 1)
                                                                                                                    {
                                                                                                                        rectangle.X = 0;
                                                                                                                        rectangle.Y = 144;
                                                                                                                    }
                                                                                                                    if (num19 == 2)
                                                                                                                    {
                                                                                                                        rectangle.X = 0;
                                                                                                                        rectangle.Y = 180;
                                                                                                                    }
                                                                                                                }
                                                                                                                if (num6 == -2)
                                                                                                                {
                                                                                                                    if (num19 == 0)
                                                                                                                    {
                                                                                                                        rectangle.X = 18;
                                                                                                                        rectangle.Y = 90;
                                                                                                                    }
                                                                                                                    if (num19 == 1)
                                                                                                                    {
                                                                                                                        rectangle.X = 18;
                                                                                                                        rectangle.Y = 126;
                                                                                                                    }
                                                                                                                    if (num19 == 2)
                                                                                                                    {
                                                                                                                        rectangle.X = 18;
                                                                                                                        rectangle.Y = 162;
                                                                                                                    }
                                                                                                                }
                                                                                                                if (num8 == -2)
                                                                                                                {
                                                                                                                    if (num19 == 0)
                                                                                                                    {
                                                                                                                        rectangle.X = 0;
                                                                                                                        rectangle.Y = 90;
                                                                                                                    }
                                                                                                                    if (num19 == 1)
                                                                                                                    {
                                                                                                                        rectangle.X = 0;
                                                                                                                        rectangle.Y = 126;
                                                                                                                    }
                                                                                                                    if (num19 == 2)
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
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 234;
                                                            rectangle.Y = 0;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 252;
                                                            rectangle.Y = 0;
                                                        }
                                                        if (num19 == 2)
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
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 234;
                                                                rectangle.Y = 18;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 252;
                                                                rectangle.Y = 18;
                                                            }
                                                            if (num19 == 2)
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
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 234;
                                                                    rectangle.Y = 36;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 252;
                                                                    rectangle.Y = 36;
                                                                }
                                                                if (num19 == 2)
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
                                                                    if (num19 == 0)
                                                                    {
                                                                        rectangle.X = 234;
                                                                        rectangle.Y = 54;
                                                                    }
                                                                    if (num19 == 1)
                                                                    {
                                                                        rectangle.X = 252;
                                                                        rectangle.Y = 54;
                                                                    }
                                                                    if (num19 == 2)
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
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 72;
                                                            rectangle.Y = 144;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 72;
                                                            rectangle.Y = 162;
                                                        }
                                                        if (num19 == 2)
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
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 90;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 72;
                                                                rectangle.Y = 108;
                                                            }
                                                            if (num19 == 2)
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
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 90;
                                                                rectangle.Y = 144;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 90;
                                                                rectangle.Y = 162;
                                                            }
                                                            if (num19 == 2)
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
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 90;
                                                                    rectangle.Y = 90;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 90;
                                                                    rectangle.Y = 108;
                                                                }
                                                                if (num19 == 2)
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
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 0;
                                                                    rectangle.Y = 198;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 18;
                                                                    rectangle.Y = 198;
                                                                }
                                                                if (num19 == 2)
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
                                                                    if (num19 == 0)
                                                                    {
                                                                        rectangle.X = 54;
                                                                        rectangle.Y = 198;
                                                                    }
                                                                    if (num19 == 1)
                                                                    {
                                                                        rectangle.X = 72;
                                                                        rectangle.Y = 198;
                                                                    }
                                                                    if (num19 == 2)
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
                                                                    if (num19 == 0)
                                                                    {
                                                                        rectangle.X = 0;
                                                                        rectangle.Y = 216;
                                                                    }
                                                                    if (num19 == 1)
                                                                    {
                                                                        rectangle.X = 18;
                                                                        rectangle.Y = 216;
                                                                    }
                                                                    if (num19 == 2)
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
                                                                        if (num19 == 0)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 216;
                                                                        }
                                                                        if (num19 == 1)
                                                                        {
                                                                            rectangle.X = 72;
                                                                            rectangle.Y = 216;
                                                                        }
                                                                        if (num19 == 2)
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
                                                                        if (num19 == 0)
                                                                        {
                                                                            rectangle.X = 108;
                                                                            rectangle.Y = 216;
                                                                        }
                                                                        if (num19 == 1)
                                                                        {
                                                                            rectangle.X = 108;
                                                                            rectangle.Y = 234;
                                                                        }
                                                                        if (num19 == 2)
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
                                                                            if (num19 == 0)
                                                                            {
                                                                                rectangle.X = 126;
                                                                                rectangle.Y = 144;
                                                                            }
                                                                            if (num19 == 1)
                                                                            {
                                                                                rectangle.X = 126;
                                                                                rectangle.Y = 162;
                                                                            }
                                                                            if (num19 == 2)
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
                                                                                if (num19 == 0)
                                                                                {
                                                                                    rectangle.X = 126;
                                                                                    rectangle.Y = 90;
                                                                                }
                                                                                if (num19 == 1)
                                                                                {
                                                                                    rectangle.X = 126;
                                                                                    rectangle.Y = 108;
                                                                                }
                                                                                if (num19 == 2)
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
                                                                            if (num19 == 0)
                                                                            {
                                                                                rectangle.X = 162;
                                                                                rectangle.Y = 198;
                                                                            }
                                                                            if (num19 == 1)
                                                                            {
                                                                                rectangle.X = 180;
                                                                                rectangle.Y = 198;
                                                                            }
                                                                            if (num19 == 2)
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
                                                                                if (num19 == 0)
                                                                                {
                                                                                    rectangle.X = 0;
                                                                                    rectangle.Y = 252;
                                                                                }
                                                                                if (num19 == 1)
                                                                                {
                                                                                    rectangle.X = 18;
                                                                                    rectangle.Y = 252;
                                                                                }
                                                                                if (num19 == 2)
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
                                                                                    if (num19 == 0)
                                                                                    {
                                                                                        rectangle.X = 54;
                                                                                        rectangle.Y = 252;
                                                                                    }
                                                                                    if (num19 == 1)
                                                                                    {
                                                                                        rectangle.X = 72;
                                                                                        rectangle.Y = 252;
                                                                                    }
                                                                                    if (num19 == 2)
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
                                                                            if (num19 == 0)
                                                                            {
                                                                                rectangle.X = 108;
                                                                                rectangle.Y = 144;
                                                                            }
                                                                            if (num19 == 1)
                                                                            {
                                                                                rectangle.X = 108;
                                                                                rectangle.Y = 162;
                                                                            }
                                                                            if (num19 == 2)
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
                                                                                if (num19 == 0)
                                                                                {
                                                                                    rectangle.X = 108;
                                                                                    rectangle.Y = 90;
                                                                                }
                                                                                if (num19 == 1)
                                                                                {
                                                                                    rectangle.X = 108;
                                                                                    rectangle.Y = 108;
                                                                                }
                                                                                if (num19 == 2)
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
                                                                                    if (num19 == 0)
                                                                                    {
                                                                                        rectangle.X = 0;
                                                                                        rectangle.Y = 234;
                                                                                    }
                                                                                    if (num19 == 1)
                                                                                    {
                                                                                        rectangle.X = 18;
                                                                                        rectangle.Y = 234;
                                                                                    }
                                                                                    if (num19 == 2)
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
                                                                                        if (num19 == 0)
                                                                                        {
                                                                                            rectangle.X = 54;
                                                                                            rectangle.Y = 234;
                                                                                        }
                                                                                        if (num19 == 1)
                                                                                        {
                                                                                            rectangle.X = 72;
                                                                                            rectangle.Y = 234;
                                                                                        }
                                                                                        if (num19 == 2)
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
                                                    if (num19 == 0)
                                                    {
                                                        rectangle.X = 108;
                                                        rectangle.Y = 18;
                                                    }
                                                    if (num19 == 1)
                                                    {
                                                        rectangle.X = 126;
                                                        rectangle.Y = 18;
                                                    }
                                                    if (num19 == 2)
                                                    {
                                                        rectangle.X = 144;
                                                        rectangle.Y = 18;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num6 != num9 && num8 != num9)
                                                    {
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 108;
                                                            rectangle.Y = 36;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 36;
                                                        }
                                                        if (num19 == 2)
                                                        {
                                                            rectangle.X = 144;
                                                            rectangle.Y = 36;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num != num9 && num6 != num9)
                                                        {
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 180;
                                                                rectangle.Y = 0;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 180;
                                                                rectangle.Y = 18;
                                                            }
                                                            if (num19 == 2)
                                                            {
                                                                rectangle.X = 180;
                                                                rectangle.Y = 36;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num3 != num9 && num8 != num9)
                                                            {
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 198;
                                                                    rectangle.Y = 0;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 198;
                                                                    rectangle.Y = 18;
                                                                }
                                                                if (num19 == 2)
                                                                {
                                                                    rectangle.X = 198;
                                                                    rectangle.Y = 36;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 18;
                                                                    rectangle.Y = 18;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 36;
                                                                    rectangle.Y = 18;
                                                                }
                                                                if (num19 == 2)
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
                                                    if (num19 == 0)
                                                    {
                                                        rectangle.X = 18;
                                                        rectangle.Y = 0;
                                                    }
                                                    if (num19 == 1)
                                                    {
                                                        rectangle.X = 36;
                                                        rectangle.Y = 0;
                                                    }
                                                    if (num19 == 2)
                                                    {
                                                        rectangle.X = 54;
                                                        rectangle.Y = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 == num9))
                                                    {
                                                        if (num19 == 0)
                                                        {
                                                            rectangle.X = 18;
                                                            rectangle.Y = 36;
                                                        }
                                                        if (num19 == 1)
                                                        {
                                                            rectangle.X = 36;
                                                            rectangle.Y = 36;
                                                        }
                                                        if (num19 == 2)
                                                        {
                                                            rectangle.X = 54;
                                                            rectangle.Y = 36;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 == num9))
                                                        {
                                                            if (num19 == 0)
                                                            {
                                                                rectangle.X = 0;
                                                                rectangle.Y = 0;
                                                            }
                                                            if (num19 == 1)
                                                            {
                                                                rectangle.X = 0;
                                                                rectangle.Y = 18;
                                                            }
                                                            if (num19 == 2)
                                                            {
                                                                rectangle.X = 0;
                                                                rectangle.Y = 36;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num2 == num9 && num7 == num9 && (num4 == num9 & num5 != num9))
                                                            {
                                                                if (num19 == 0)
                                                                {
                                                                    rectangle.X = 72;
                                                                    rectangle.Y = 0;
                                                                }
                                                                if (num19 == 1)
                                                                {
                                                                    rectangle.X = 72;
                                                                    rectangle.Y = 18;
                                                                }
                                                                if (num19 == 2)
                                                                {
                                                                    rectangle.X = 72;
                                                                    rectangle.Y = 36;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 == num9))
                                                                {
                                                                    if (num19 == 0)
                                                                    {
                                                                        rectangle.X = 0;
                                                                        rectangle.Y = 54;
                                                                    }
                                                                    if (num19 == 1)
                                                                    {
                                                                        rectangle.X = 36;
                                                                        rectangle.Y = 54;
                                                                    }
                                                                    if (num19 == 2)
                                                                    {
                                                                        rectangle.X = 72;
                                                                        rectangle.Y = 54;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (num2 != num9 && num7 == num9 && (num4 == num9 & num5 != num9))
                                                                    {
                                                                        if (num19 == 0)
                                                                        {
                                                                            rectangle.X = 18;
                                                                            rectangle.Y = 54;
                                                                        }
                                                                        if (num19 == 1)
                                                                        {
                                                                            rectangle.X = 54;
                                                                            rectangle.Y = 54;
                                                                        }
                                                                        if (num19 == 2)
                                                                        {
                                                                            rectangle.X = 90;
                                                                            rectangle.Y = 54;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 == num9))
                                                                        {
                                                                            if (num19 == 0)
                                                                            {
                                                                                rectangle.X = 0;
                                                                                rectangle.Y = 72;
                                                                            }
                                                                            if (num19 == 1)
                                                                            {
                                                                                rectangle.X = 36;
                                                                                rectangle.Y = 72;
                                                                            }
                                                                            if (num19 == 2)
                                                                            {
                                                                                rectangle.X = 72;
                                                                                rectangle.Y = 72;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 != num9))
                                                                            {
                                                                                if (num19 == 0)
                                                                                {
                                                                                    rectangle.X = 18;
                                                                                    rectangle.Y = 72;
                                                                                }
                                                                                if (num19 == 1)
                                                                                {
                                                                                    rectangle.X = 54;
                                                                                    rectangle.Y = 72;
                                                                                }
                                                                                if (num19 == 2)
                                                                                {
                                                                                    rectangle.X = 90;
                                                                                    rectangle.Y = 72;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 != num9))
                                                                                {
                                                                                    if (num19 == 0)
                                                                                    {
                                                                                        rectangle.X = 90;
                                                                                        rectangle.Y = 0;
                                                                                    }
                                                                                    if (num19 == 1)
                                                                                    {
                                                                                        rectangle.X = 90;
                                                                                        rectangle.Y = 18;
                                                                                    }
                                                                                    if (num19 == 2)
                                                                                    {
                                                                                        rectangle.X = 90;
                                                                                        rectangle.Y = 36;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 == num9))
                                                                                    {
                                                                                        if (num19 == 0)
                                                                                        {
                                                                                            rectangle.X = 108;
                                                                                            rectangle.Y = 72;
                                                                                        }
                                                                                        if (num19 == 1)
                                                                                        {
                                                                                            rectangle.X = 126;
                                                                                            rectangle.Y = 72;
                                                                                        }
                                                                                        if (num19 == 2)
                                                                                        {
                                                                                            rectangle.X = 144;
                                                                                            rectangle.Y = 72;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 != num9))
                                                                                        {
                                                                                            if (num19 == 0)
                                                                                            {
                                                                                                rectangle.X = 108;
                                                                                                rectangle.Y = 0;
                                                                                            }
                                                                                            if (num19 == 1)
                                                                                            {
                                                                                                rectangle.X = 126;
                                                                                                rectangle.Y = 0;
                                                                                            }
                                                                                            if (num19 == 2)
                                                                                            {
                                                                                                rectangle.X = 144;
                                                                                                rectangle.Y = 0;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 != num9))
                                                                                            {
                                                                                                if (num19 == 0)
                                                                                                {
                                                                                                    rectangle.X = 108;
                                                                                                    rectangle.Y = 54;
                                                                                                }
                                                                                                if (num19 == 1)
                                                                                                {
                                                                                                    rectangle.X = 126;
                                                                                                    rectangle.Y = 54;
                                                                                                }
                                                                                                if (num19 == 2)
                                                                                                {
                                                                                                    rectangle.X = 144;
                                                                                                    rectangle.Y = 54;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 == num9))
                                                                                                {
                                                                                                    if (num19 == 0)
                                                                                                    {
                                                                                                        rectangle.X = 162;
                                                                                                        rectangle.Y = 0;
                                                                                                    }
                                                                                                    if (num19 == 1)
                                                                                                    {
                                                                                                        rectangle.X = 162;
                                                                                                        rectangle.Y = 18;
                                                                                                    }
                                                                                                    if (num19 == 2)
                                                                                                    {
                                                                                                        rectangle.X = 162;
                                                                                                        rectangle.Y = 36;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 != num9))
                                                                                                    {
                                                                                                        if (num19 == 0)
                                                                                                        {
                                                                                                            rectangle.X = 216;
                                                                                                            rectangle.Y = 0;
                                                                                                        }
                                                                                                        if (num19 == 1)
                                                                                                        {
                                                                                                            rectangle.X = 216;
                                                                                                            rectangle.Y = 18;
                                                                                                        }
                                                                                                        if (num19 == 2)
                                                                                                        {
                                                                                                            rectangle.X = 216;
                                                                                                            rectangle.Y = 36;
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 != num9))
                                                                                                        {
                                                                                                            if (num19 == 0)
                                                                                                            {
                                                                                                                rectangle.X = 162;
                                                                                                                rectangle.Y = 54;
                                                                                                            }
                                                                                                            if (num19 == 1)
                                                                                                            {
                                                                                                                rectangle.X = 180;
                                                                                                                rectangle.Y = 54;
                                                                                                            }
                                                                                                            if (num19 == 2)
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
                                            if (num19 <= 0)
                                            {
                                                rectangle.X = 18;
                                                rectangle.Y = 18;
                                            }
                                            if (num19 == 1)
                                            {
                                                rectangle.X = 36;
                                                rectangle.Y = 18;
                                            }
                                            if (num19 >= 2)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 18;
                                            }
                                        }
                                        Main.tile[i, j].frameX = (short)rectangle.X;
                                        Main.tile[i, j].frameY = (short)rectangle.Y;
                                        if (num9 == 52 || num9 == 62)
                                        {
                                            if (Main.tile[i, j - 1] != null)
                                            {
                                                if (!Main.tile[i, j - 1].active)
                                                {
                                                    num2 = -1;
                                                }
                                                else
                                                {
                                                    num2 = (int)Main.tile[i, j - 1].type;
                                                }
                                            }
                                            else
                                            {
                                                num2 = num9;
                                            }
                                            if (num2 != num9 && num2 != 2 && num2 != 60)
                                            {
                                                WorldGen.KillTile(i, j, false, false, false);
                                            }
                                        }
                                        if (num9 == 53)
                                        {
                                            if (Main.netMode == 0)
                                            {
                                                if (Main.tile[i, j + 1] != null)
                                                {
                                                    if (!Main.tile[i, j + 1].active)
                                                    {
                                                        bool flag2 = true;
                                                        if (Main.tile[i, j - 1].active && Main.tile[i, j - 1].type == 21)
                                                        {
                                                            flag2 = false;
                                                        }
                                                        if (flag2)
                                                        {
                                                            Main.tile[i, j].active = false;
                                                            Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0.41f, 31, 10, 0f, Main.myPlayer);
                                                            WorldGen.SquareTileFrame(i, j, true);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Main.netMode == 2)
                                                {
                                                    if (Main.tile[i, j + 1] != null)
                                                    {
                                                        if (!Main.tile[i, j + 1].active)
                                                        {
                                                            bool flag2 = true;
                                                            if (Main.tile[i, j - 1].active && Main.tile[i, j - 1].type == 21)
                                                            {
                                                                flag2 = false;
                                                            }
                                                            if (flag2)
                                                            {
                                                                Main.tile[i, j].active = false;
                                                                int num21 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0.41f, 31, 10, 0f, Main.myPlayer);
                                                                Main.projectile[num21].velocity.Y = 0.5f;
                                                                Projectile expr_8AFE_cp_0 = Main.projectile[num21];
                                                                expr_8AFE_cp_0.position.Y = expr_8AFE_cp_0.position.Y + 2f;
                                                                NetMessage.SendTileSquare(-1, i, j, 1);
                                                                WorldGen.SquareTileFrame(i, j, true);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
                                        {
                                            bool flag3 = WorldGen.mergeUp;
                                            bool flag4 = WorldGen.mergeDown;
                                            bool flag5 = WorldGen.mergeLeft;
                                            bool flag6 = WorldGen.mergeRight;
                                            WorldGen.TileFrame(i - 1, j, false, false);
                                            WorldGen.TileFrame(i + 1, j, false, false);
                                            WorldGen.TileFrame(i, j - 1, false, false);
                                            WorldGen.TileFrame(i, j + 1, false, false);
                                            WorldGen.mergeUp = flag3;
                                            WorldGen.mergeDown = flag4;
                                            WorldGen.mergeLeft = flag5;
                                            WorldGen.mergeRight = flag6;
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
