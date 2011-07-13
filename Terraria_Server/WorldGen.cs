using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Terraria_Server.Events;
using Terraria_Server.Commands;
using Terraria_Server.Plugin;
using Terraria_Server.Misc;
using Terraria_Server.Shops;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;

namespace Terraria_Server
{
    internal class WorldGen
    {
        
        private const int RECTANGLE_OFFSET = 25;
        private const int TILE_OFFSET = 15;
        private const int TILES_OFFSET_2 = 10;
        private const int TILE_OFFSET_3 = 16;
        private const int TILE_OFFSET_4 = 23;
        private const int TILE_SCALE = 16;
        private const int TREE_RADIUS = 2;

        private static object padlock = new object();
        public static int lavaLine;
        public static int waterLine;
        public static bool noTileActions = false;
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
        public static String statusText = "";
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
        public static bool[] houseTile = new bool[86];
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
        private static int JungleX = 0;
        private static int hellChest = 0;

        public static int numDungeons;
        public static int ficount;

        public static void SpawnNPC(int x, int y)
        {
            if (Main.wallHouse[(int)Main.tile.At(x, y).Wall])
            {
                WorldGen.canSpawn = true;
            }
            if (!WorldGen.canSpawn)
            {
                return;
            }
            if (!WorldGen.StartRoomCheck(x, y))
            {
                return;
            }
            if (!WorldGen.RoomNeeds(WorldGen.spawnNPC))
            {
                return;
            }
            WorldGen.ScoreRoom(-1);
            if (WorldGen.hiScore > 0)
            {
                int num = -1;
                for (int i = 0; i < 1000; i++)
                {
                    if (Main.npcs[i].Active && Main.npcs[i].homeless && Main.npcs[i].Type == WorldGen.spawnNPC)
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
                        for (int j = 0; j < 255; j++)
                        {
                            if (Main.players[j].Active)
                            {
                                Rectangle rectangle = new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].Width, Main.players[j].Height);
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
                        for (int k = 1; k < 500; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                if (l == 0)
                                {
                                    num2 = WorldGen.bestX + k;
                                }
                                else
                                {
                                    num2 = WorldGen.bestX - k;
                                }
                                if (num2 > 10 && num2 < Main.maxTilesX - 10)
                                {
                                    int num4 = WorldGen.bestY - k;
                                    double num5 = (double)(WorldGen.bestY + k);
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
                                        if (Main.tile.At(num2, num3).Active && Main.tileSolid[(int)Main.tile.At(num2, num3).Type])
                                        {
                                            if (!Collision.SolidTiles(num2 - 1, num2 + 1, num3 - 3, num3 - 1))
                                            {
                                                flag = true;
                                                Rectangle value2 = new Rectangle(num2 * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, num3 * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                for (int m = 0; m < 255; m++)
                                                {
                                                    if (Main.players[m].Active)
                                                    {
                                                        Rectangle rectangle2 = new Rectangle((int)Main.players[m].Position.X, (int)Main.players[m].Position.Y, Main.players[m].Width, Main.players[m].Height);
                                                        if (rectangle2.Intersects(value2))
                                                        {
                                                            flag = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            num6++;
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
                    int num7 = NPC.NewNPC(num2 * 16, num3 * 16, WorldGen.spawnNPC, 1);
                    Main.npcs[num7].homeTileX = WorldGen.bestX;
                    Main.npcs[num7].homeTileY = WorldGen.bestY;
                    if (num2 < WorldGen.bestX)
                    {
                        Main.npcs[num7].direction = 1;
                    }
                    else
                    {
                        if (num2 > WorldGen.bestX)
                        {
                            Main.npcs[num7].direction = -1;
                        }
                    }
                    Main.npcs[num7].netUpdate = true;
                    
                    NetMessage.SendData(25, -1, -1, Main.npcs[num7].Name + " has arrived!", 255, 50f, 125f, 255f);
                }
                else
                {
                    WorldGen.spawnNPC = 0;
                    Main.npcs[num].homeTileX = WorldGen.bestX;
                    Main.npcs[num].homeTileY = WorldGen.bestY;
                    Main.npcs[num].homeless = false;
                }
                WorldGen.spawnNPC = 0;
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
            if (Main.npcs[npc].homeTileX > 10 && Main.npcs[npc].homeTileY > 10 && Main.npcs[npc].homeTileX < Main.maxTilesX - 10 && Main.npcs[npc].homeTileY < Main.maxTilesY)
            {
                WorldGen.canSpawn = false;
                WorldGen.StartRoomCheck(Main.npcs[npc].homeTileX, Main.npcs[npc].homeTileY - 1);
                if (!WorldGen.canSpawn)
                {
                    for (int i = Main.npcs[npc].homeTileX - 1; i < Main.npcs[npc].homeTileX + 2; i++)
                    {
                        int num = Main.npcs[npc].homeTileY - 1;
                        while (num < Main.npcs[npc].homeTileY + 2 && !WorldGen.StartRoomCheck(i, num))
                        {
                            num++;
                        }
                    }
                }
                if (!WorldGen.canSpawn)
                {
                    int num2 = 10;
                    for (int j = Main.npcs[npc].homeTileX - num2; j <= Main.npcs[npc].homeTileX + num2; j += 2)
                    {
                        int num3 = Main.npcs[npc].homeTileY - num2;
                        while (num3 <= Main.npcs[npc].homeTileY + num2 && !WorldGen.StartRoomCheck(j, num3))
                        {
                            num3 += 2;
                        }
                    }
                }
                if (WorldGen.canSpawn)
                {
                    WorldGen.RoomNeeds(Main.npcs[npc].Type);
                    if (WorldGen.canSpawn)
                    {
                        WorldGen.ScoreRoom(npc);
                    }
                    if (WorldGen.canSpawn && WorldGen.hiScore > 0)
                    {
                        Main.npcs[npc].homeTileX = WorldGen.bestX;
                        Main.npcs[npc].homeTileY = WorldGen.bestY;
                        Main.npcs[npc].homeless = false;
                        WorldGen.canSpawn = false;
                        return;
                    }
                    Main.npcs[npc].homeless = true;
                    return;
                }
                else
                {
                    Main.npcs[npc].homeless = true;
                }
            }
        }
        
        public static void ScoreRoom(int ignoreNPC = -1)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.npcs[i].Active && Main.npcs[i].townNPC && ignoreNPC != i && !Main.npcs[i].homeless)
                {
                    for (int j = 0; j < WorldGen.numRoomTiles; j++)
                    {
                        if (Main.npcs[i].homeTileX == WorldGen.roomX[j] && Main.npcs[i].homeTileY == WorldGen.roomY[j])
                        {
                            bool flag = false;
                            for (int k = 0; k < WorldGen.numRoomTiles; k++)
                            {
                                if (Main.npcs[i].homeTileX == WorldGen.roomX[k] && Main.npcs[i].homeTileY - 1 == WorldGen.roomY[k])
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
            for (int l = num3 + 1; l < num4; l++)
            {
                for (int m = num5 + 2; m < num6 + 2; m++)
                {
                    if (Main.tile.At(l, m).Active)
                    {
                        if (Main.tile.At(l, m).Type == 23 || Main.tile.At(l, m).Type == 24 || Main.tile.At(l, m).Type == 25 || Main.tile.At(l, m).Type == 32)
                        {
                            Main.evilTiles++;
                        }
                        else
                        {
                            if (Main.tile.At(l, m).Type == 27)
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
            for (int n = num3 + 1; n < num4; n++)
            {
                for (int num8 = num5 + 2; num8 < num6 + 2; num8++)
                {
                    if (Main.tile.At(n, num8).Active)
                    {
                        num = num7;
                        if (Main.tileSolid[(int)Main.tile.At(n, num8).Type] && !Main.tileSolidTop[(int)Main.tile.At(n, num8).Type] && !Collision.SolidTiles(n - 1, n + 1, num8 - 3, num8 - 1) && Main.tile.At(n - 1, num8).Active && Main.tileSolid[(int)Main.tile.At(n - 1, num8).Type] && Main.tile.At(n + 1, num8).Active && Main.tileSolid[(int)Main.tile.At(n + 1, num8).Type])
                        {
                            for (int num9 = n - 2; num9 < n + 3; num9++)
                            {
                                for (int num10 = num8 - 4; num10 < num8; num10++)
                                {
                                    if (Main.tile.At(num9, num10).Active)
                                    {
                                        if (num9 == n)
                                        {
                                            num -= 15;
                                        }
                                        else
                                        {
                                            if (Main.tile.At(num9, num10).Type == 10 || Main.tile.At(num9, num10).Type == 11)
                                            {
                                                num -= 20;
                                            }
                                            else
                                            {
                                                if (Main.tileSolid[(int)Main.tile.At(num9, num10).Type])
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
                                for (int num11 = 0; num11 < WorldGen.numRoomTiles; num11++)
                                {
                                    if (WorldGen.roomX[num11] == n && WorldGen.roomY[num11] == num8)
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                                if (flag2)
                                {
                                    WorldGen.hiScore = num;
                                    WorldGen.bestX = n;
                                    WorldGen.bestY = num8;
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
            for (int i = 0; i < 86; i++)
            {
                WorldGen.houseTile[i] = false;
            }
            WorldGen.canSpawn = true;
            if (Main.tile.At(x, y).Active && Main.tileSolid[(int)Main.tile.At(x, y).Type])
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
            if (!WorldGen.canSpawn)
            {
                return;
            }
            if (x < 10 || y < 10 || x >= Main.maxTilesX - 10 || y >= WorldGen.lastMaxTilesY - 10)
            {
                WorldGen.canSpawn = false;
                return;
            }
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
                return;
            }
            if (Main.tile.At(x, y).Active)
            {
                WorldGen.houseTile[(int)Main.tile.At(x, y).Type] = true;
                if (Main.tileSolid[(int)Main.tile.At(x, y).Type] || Main.tile.At(x, y).Type == 11)
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
            for (int j = -2; j < 3; j++)
            {
                if (Main.wallHouse[(int)Main.tile.At(x + j, y).Wall])
                {
                    flag = true;
                }
                if (Main.tile.At(x + j, y).Active && (Main.tileSolid[(int)Main.tile.At(x + j, y).Type] || Main.tile.At(x + j, y).Type == 11))
                {
                    flag = true;
                }
                if (Main.wallHouse[(int)Main.tile.At(x, y + j).Wall])
                {
                    flag2 = true;
                }
                if (Main.tile.At(x, y + j).Active && (Main.tileSolid[(int)Main.tile.At(x, y + j).Type] || Main.tile.At(x, y + j).Type == 11))
                {
                    flag2 = true;
                }
            }
            if (!flag || !flag2)
            {
                WorldGen.canSpawn = false;
                return;
            }
            for (int k = x - 1; k < x + 2; k++)
            {
                for (int l = y - 1; l < y + 2; l++)
                {
                    if ((k != x || l != y) && WorldGen.canSpawn)
                    {
                        WorldGen.CheckRoom(k, l);
                    }
                }
            }
        }
        
        public static void dropMeteor()
        {
            bool flag = true;
            int num = 0;
            for (int i = 0; i < 255; i++)
            {
                if (Main.players[i].Active)
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
                int num5 = 5;
                while ((double)num5 < Main.worldSurface)
                {
                    if (Main.tile.At(j, num5).Active && Main.tile.At(j, num5).Type == 37)
                    {
                        num2++;
                        if (num2 > num4)
                        {
                            return;
                        }
                    }
                    num5++;
                }
            }
            while (!flag)
            {
                float num6 = (float)Main.maxTilesX * 0.08f;
                int num7 = Main.rand.Next(50, Main.maxTilesX - 50);
                while ((float)num7 > (float)Main.spawnTileX - num6 && (float)num7 < (float)Main.spawnTileX + num6)
                {
                    num7 = Main.rand.Next(50, Main.maxTilesX - 50);
                }
                for (int k = Main.rand.Next(100); k < Main.maxTilesY; k++)
                {
                    if (Main.tile.At(num7, k).Active && Main.tileSolid[(int)Main.tile.At(num7, k).Type])
                    {
                        flag = WorldGen.meteor(num7, k);
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
        
        public static bool meteor(int x, int y)
        {
            if (x < 50 || x > Main.maxTilesX - 50)
            {
                return false;
            }
            if (y < 50 || y > Main.maxTilesY - 50)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle((x - RECTANGLE_OFFSET) * TILE_SCALE, (y - RECTANGLE_OFFSET) * TILE_SCALE, 
                RECTANGLE_OFFSET * 2 * TILE_SCALE, RECTANGLE_OFFSET * 2 * TILE_SCALE);

            BaseEntity entity;
            for (int i = 0; i < Main.MAX_PLAYERS; i++)
            {
                entity = Main.players[i];
                if (entity.Active && entity.Intersects(rectangle))
                {
                    return false;
                }
            }

            for (int i = 0; i < NPC.MAX_NPCS; i++)
            {
                entity = Main.npcs[i];
                if (entity.Active && entity.Intersects(rectangle))
                {
                    return false;
                }
            }

            for (int modX = x - RECTANGLE_OFFSET; modX < x + RECTANGLE_OFFSET; modX++)
            {
                for (int modY = y - RECTANGLE_OFFSET; modY < y + RECTANGLE_OFFSET; modY++)
                {
                    if (Main.tile.At(modX, modY).Active && Main.tile.At(modX, modY).Type == 21)
                    {
                        return false;
                    }
                }
            }

            WorldGen.stopDrops = true;
            for (int num2 = x - TILE_OFFSET; num2 < x + TILE_OFFSET; num2++)
            {
                for (int num3 = y - TILE_OFFSET; num3 < y + TILE_OFFSET; num3++)
                {
                    if (num3 > y + Main.rand.Next(-2, 3) - 5 && (double)(Math.Abs(x - num2) + Math.Abs(y - num3)) < (double)TILE_OFFSET * 1.5 + (double)Main.rand.Next(-5, 5))
                    {
                        if (!Main.tileSolid[(int)Main.tile.At(num2, num3).Type])
                        {
                            Main.tile.At(num2, num3).SetActive (false);
                        }
                        Main.tile.At(num2, num3).SetType (37);
                    }
                }
            }

            for (int num4 = x - TILES_OFFSET_2; num4 < x + TILES_OFFSET_2; num4++)
            {
                for (int num5 = y - TILES_OFFSET_2; num5 < y + TILES_OFFSET_2; num5++)
                {
                    if (num5 > y + Main.rand.Next(-2, 3) - 5 && Math.Abs(x - num4) + Math.Abs(y - num5) < TILES_OFFSET_2 + Main.rand.Next(-3, 4))
                    {
                        Main.tile.At(num4, num5).SetActive (false);
                    }
                }
            }
            
            for (int num6 = x - TILE_OFFSET_3; num6 < x + TILE_OFFSET_3; num6++)
            {
                for (int num7 = y - TILE_OFFSET_3; num7 < y + TILE_OFFSET_3; num7++)
                {
                    if (Main.tile.At(num6, num7).Type == 5 || Main.tile.At(num6, num7).Type == 32)
                    {
                        WorldGen.KillTile(num6, num7, false, false, false);
                    }
                    WorldGen.SquareTileFrame(num6, num7, true);
                    WorldGen.SquareWallFrame(num6, num7, true);
                }
            }

            for (int num8 = x - TILE_OFFSET_4; num8 < x + TILE_OFFSET_4; num8++)
            {
                for (int num9 = y - TILE_OFFSET_4; num9 < y + TILE_OFFSET_4; num9++)
                {
                    if (Main.tile.At(num8, num9).Active && Main.rand.Next(10) == 0 && (double)(Math.Abs(x - num8) + Math.Abs(y - num9)) < (double)TILE_OFFSET_4 * 1.3)
                    {
                        if (Main.tile.At(num8, num9).Type == 5 || Main.tile.At(num8, num9).Type == 32)
                        {
                            WorldGen.KillTile(num8, num9, false, false, false);
                        }
                        Main.tile.At(num8, num9).SetType (37);
                        WorldGen.SquareTileFrame(num8, num9, true);
                    }
                }
            }
            WorldGen.stopDrops = false;
            
            NetMessage.SendData(25, -1, -1, "A meteorite has landed!", 255, 50f, 255f, 130f);
            NetMessage.SendTileSquare(-1, x, y, 30);
            return true;
        }
        
        public static void setWorldSize()
        {
            Main.bottomWorld = (float)(Main.maxTilesY * 16);
            Main.rightWorld = (float)(Main.maxTilesX * 16);
            Main.maxSectionsX = Main.maxTilesX / 200;
            Main.maxSectionsY = Main.maxTilesY / 150;
        }
        
        public static void saveAndPlayCallBack(object threadContext)
        {
            WorldGen.saveWorld(Program.server.World.SavePath, false);
        }
        
        public static void saveAndPlay()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveAndPlayCallBack), 1);
        }
        
        public static void saveToonWhilePlayingCallBack(object threadContext)
        {
            Player.SavePlayer(Main.players[Main.myPlayer]);
        }
        
        public static void saveToonWhilePlaying()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveToonWhilePlayingCallBack), 1);
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
            if (WorldGen.lastMaxTilesX > Main.maxTilesX || WorldGen.lastMaxTilesY > Main.maxTilesY)
            {
                for (int i = 0; i < WorldGen.lastMaxTilesX; i++)
                {
                    float num = (float)i / (float)WorldGen.lastMaxTilesX;
                    Program.printData("Freeing unused resources: " + (int)(num * 100f + 1f) + "%", true);
                    for (int j = 0; j < WorldGen.lastMaxTilesY; j++)
                    {
                        Main.tile.CreateTileAt (i, j);
                    }
                }
                Program.tConsole.WriteLine();
            }
            WorldGen.lastMaxTilesX = Main.maxTilesX;
            WorldGen.lastMaxTilesY = Main.maxTilesY;
            
            for (int k = 0; k < Main.maxTilesX; k++)
            {
                float num2 = (float)k / (float)Main.maxTilesX;
                Program.printData("Resetting game objects: " + (int)(num2 * 100f + 1f) + "%", true);
                for (int l = 0; l < Main.maxTilesY; l++)
                {
                    Main.tile.CreateTileAt (k, l);
                }
            }
            //for (int m = 0; m < 1000; m++)
            //{
            //    Main.dust[m] = new Dust();
            //}
            //for (int n = 0; n < 200; n++)
            //{
            //    Main.gore[n] = new Gore();
            //}
            for (int num3 = 0; num3 < 200; num3++)
            {
                Main.item[num3] = new Item();
            }
            for (int num4 = 0; num4 < 1000; num4++)
            {
                Main.npcs[num4] = new NPC();
            }
            for (int num5 = 0; num5 < 1000; num5++)
            {
                Main.projectile[num5] = new Projectile();
            }
            for (int num6 = 0; num6 < 1000; num6++)
            {
                Main.chest[num6] = null;
            }
            for (int num7 = 0; num7 < 1000; num7++)
            {
                Main.sign[num7] = null;
            }
            for (int num8 = 0; num8 < Liquid.resLiquid; num8++)
            {
                Main.liquid[num8] = new Liquid();
            }
            for (int num9 = 0; num9 < 10000; num9++)
            {
                Main.liquidBuffer[num9] = new LiquidBuffer();
            }
            WorldGen.setWorldSize();
            WorldGen.worldCleared = true;
            Program.tConsole.WriteLine();
        }
        
        public static void saveWorld(String savePath, bool resetTime = false)
        {
            if (savePath == null)
            {
                return;
            }

            if (WorldGen.saveLock)
            {
                return;
            }

            try
            {
                WorldGen.saveLock = true;
                lock (WorldGen.padlock)
                {
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
                        
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();
                            String tempPath = savePath + ".sav";
                            using (FileStream fileStream = new FileStream(tempPath, FileMode.Create))
                            {
                                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                                {
                                    binaryWriter.Write(Statics.CURRENT_TERRARIA_RELEASE);
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

                                    for (int x = 0; x < Main.maxTilesX; x++)
                                    {
                                        float num = (float)x / (float)Main.maxTilesX;
                                        Program.printData("Saving world data: " + (int)(num * 100f + 1f) + "%", true);
                                        for (int y = 0; y < Main.maxTilesY; y++)
                                        {
                                            TileRef tile = Main.tile.At(x, y);
                                            binaryWriter.Write(tile.Active);
                                            if (tile.Active)
                                            {
                                                binaryWriter.Write(tile.Type);
                                                if (Main.tileFrameImportant[(int)tile.Type])
                                                {
                                                    binaryWriter.Write(tile.FrameX);
                                                    binaryWriter.Write(tile.FrameY);
                                                }
                                            }

                                            binaryWriter.Write(tile.Lighted);
                                            if (Main.tile.At(x, y).Wall > 0)
                                            {
                                                binaryWriter.Write(true);
                                                binaryWriter.Write(tile.Wall);
                                            }
                                            else
                                            {
                                                binaryWriter.Write(false);
                                            }

                                            if (tile.Liquid > 0)
                                            {
                                                binaryWriter.Write(true);
                                                binaryWriter.Write(tile.Liquid);
                                                binaryWriter.Write(tile.Lava);
                                            }
                                            else
                                            {
                                                binaryWriter.Write(false);
                                            }
                                        }
                                    }

                                    Chest chest;
                                    for (int i = 0; i < 1000; i++)
                                    {
                                        chest = Main.chest[i];
                                        if (chest == null)
                                        {
                                            binaryWriter.Write(false);
                                        }
                                        else
                                        {
                                            binaryWriter.Write(true);
                                            binaryWriter.Write(chest.x);
                                            binaryWriter.Write(chest.y);
                                            for (int l = 0; l < Chest.MAX_ITEMS; l++)
                                            {
                                                binaryWriter.Write((byte)chest.contents[l].Stack);
                                                if (chest.contents[l].Stack > 0)
                                                {
                                                    binaryWriter.Write(chest.contents[l].Name);
                                                }
                                            }
                                        }
                                    }

                                    Sign sign;
                                    for (int i = 0; i < 1000; i++)
                                    {
                                        sign = Main.sign[i];
                                        if (sign == null || sign.text == null)
                                        {
                                            binaryWriter.Write(false);
                                        }
                                        else
                                        {
                                            binaryWriter.Write(true);
                                            binaryWriter.Write(sign.text);
                                            binaryWriter.Write(sign.x);
                                            binaryWriter.Write(sign.y);
                                        }
                                    }

                                    NPC npc;
                                    for (int i = 0; i < 1000; i++)
                                    {
                                        npc = Main.npcs[i];
                                        if (npc.Active && npc.townNPC)
                                        {
                                            binaryWriter.Write(true);
                                            binaryWriter.Write(npc.Name);
                                            binaryWriter.Write(npc.Position.X);
                                            binaryWriter.Write(npc.Position.Y);
                                            binaryWriter.Write(npc.homeless);
                                            binaryWriter.Write(npc.homeTileX);
                                            binaryWriter.Write(npc.homeTileY);
                                        }
                                    }

                                    binaryWriter.Write(false);
                                    binaryWriter.Write(true);
                                    binaryWriter.Write(Main.worldName);
                                    binaryWriter.Write(Main.worldID);
                                    binaryWriter.Close();
                                    fileStream.Close();
                                    Program.tConsole.WriteLine();
                                    if (File.Exists(savePath))
                                    {
                                        Program.tConsole.WriteLine("Backing up world file...");
                                        String destFileName = savePath + ".bak";
                                        File.Copy(savePath, destFileName, true);
                                        try
                                        {
                                            File.Delete(destFileName);
                                        }
                                        catch (Exception e)
                                        {
                                            Program.tConsole.WriteLine("Exception removing " + destFileName);
                                            Program.tConsole.WriteLine(e.Message);
                                            Program.tConsole.WriteLine(e.StackTrace);
                                        }
                                        File.Move(savePath, destFileName);
                                    }
                                }

                                try
                                {
                                    File.Move(tempPath, savePath);
                                }
                                catch (Exception e)
                                {
                                    Program.tConsole.WriteLine("Exception moving " + tempPath);
                                    Program.tConsole.WriteLine(e.Message);
                                    Program.tConsole.WriteLine(e.StackTrace);
                                }

                                try
                                {
                                    File.Delete(tempPath);
                                }
                                catch (Exception e)
                                {
                                    Program.tConsole.WriteLine("Exception removing " + tempPath);
                                    Program.tConsole.WriteLine(e.Message);
                                    Program.tConsole.WriteLine(e.StackTrace);
                                }
                            }
                            stopwatch.Stop();
                            Program.tConsole.WriteLine("Save duration: " + stopwatch.Elapsed.Seconds + " Second(s)");
                            WorldGen.saveLock = false;
                    }
                }
            }
            catch (Exception e)
            {
                Program.tConsole.WriteLine("Exception Saving the World ");
                Program.tConsole.WriteLine(e.Message);
                Program.tConsole.WriteLine(e.StackTrace);
            }
        }
        
        public static void loadWorld()
        {
            if (!File.Exists(Program.server.World.SavePath) && Main.autoGen)
            {
                for (int i = Program.server.World.SavePath.Length - 1; i >= 0; i--)
                {
                    if (Program.server.World.SavePath.Substring(i, 1) == "\\")
                    {
                        String path = Program.server.World.SavePath.Substring(0, i);
                        Directory.CreateDirectory(path);
                        break;
                    }
                }
                WorldGen.clearWorld();
                WorldGen.generateWorld(-1);
                WorldGen.saveWorld(Program.server.World.SavePath, false);
            }
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
            }
            using (FileStream fileStream = new FileStream(Program.server.World.SavePath, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    try
                    {
                        WorldGen.loadFailed = false;
                        WorldGen.loadSuccess = false;
                        int num = binaryReader.ReadInt32();
                        if (num > Statics.CURRENT_TERRARIA_RELEASE)
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
                        else
                        {
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
                            for (int j = 0; j < Main.maxTilesX; j++)
                            {
                                float num2 = (float)j / (float)Main.maxTilesX;
                                Program.printData("Loading world data: " + (int)(num2 * 100f + 1f) + "%", true);
                                for (int k = 0; k < Main.maxTilesY; k++)
                                {
                                    Main.tile.At(j, k).SetActive (binaryReader.ReadBoolean());
                                    if (Main.tile.At(j, k).Active)
                                    {
                                        Main.tile.At(j, k).SetType (binaryReader.ReadByte());
                                        if (Main.tileFrameImportant[(int)Main.tile.At(j, k).Type])
                                        {
                                            Main.tile.At(j, k).SetFrameX (binaryReader.ReadInt16());
                                            Main.tile.At(j, k).SetFrameY (binaryReader.ReadInt16());
                                        }
                                        else
                                        {
                                            Main.tile.At(j, k).SetFrameX (-1);
                                            Main.tile.At(j, k).SetFrameY (-1);
                                        }
                                    }
                                    Main.tile.At(j, k).SetLighted (binaryReader.ReadBoolean());
                                    if (binaryReader.ReadBoolean())
                                    {
                                        Main.tile.At(j, k).SetWall (binaryReader.ReadByte());
                                    }
                                    if (binaryReader.ReadBoolean())
                                    {
                                        Main.tile.At(j, k).SetLiquid (binaryReader.ReadByte());
                                        Main.tile.At(j, k).SetLava (binaryReader.ReadBoolean());
                                    }
                                }
                            }
                            for (int l = 0; l < 1000; l++)
                            {
                                if (binaryReader.ReadBoolean())
                                {
                                    Main.chest[l] = new Chest();
                                    Main.chest[l].x = binaryReader.ReadInt32();
                                    Main.chest[l].y = binaryReader.ReadInt32();
                                    for (int m = 0; m < Chest.MAX_ITEMS; m++)
                                    {
                                        Main.chest[l].contents[m] = new Item();
                                        int stack = binaryReader.ReadByte();
                                        if (stack > 0)
                                        {
                                            String defaults = Item.VersionName(binaryReader.ReadString(), num);
                                            Main.chest[l].contents[m] = Registries.Item.Create(defaults, stack);
                                        }
                                    }
                                }
                            }
                            for (int n = 0; n < 1000; n++)
                            {
                                if (binaryReader.ReadBoolean())
                                {
                                    String text = binaryReader.ReadString();
                                    int num3 = binaryReader.ReadInt32();
                                    int num4 = binaryReader.ReadInt32();
                                    if (Main.tile.At(num3, num4).Active && (Main.tile.At(num3, num4).Type == 55 || Main.tile.At(num3, num4).Type == 85))
                                    {
                                        Main.sign[n] = new Sign();
                                        Main.sign[n].x = num3;
                                        Main.sign[n].y = num4;
                                        Main.sign[n].text = text;
                                    }
                                }
                            }
                            bool flag = binaryReader.ReadBoolean();
                            int num5 = 0;
                            while (flag)
                            {
                                String NPCName = binaryReader.ReadString();
                                Main.npcs[num5] = Registries.NPC.Create(NPCName);
                                Main.npcs[num5].Position.X = binaryReader.ReadSingle();
                                Main.npcs[num5].Position.Y = binaryReader.ReadSingle();
                                Main.npcs[num5].homeless = binaryReader.ReadBoolean();
                                Main.npcs[num5].homeTileX = binaryReader.ReadInt32();
                                Main.npcs[num5].homeTileY = binaryReader.ReadInt32();
                                flag = binaryReader.ReadBoolean();
                                num5++;
                            }
                            if (num >= 7)
                            {
                                bool flag2 = binaryReader.ReadBoolean();
                                String a = binaryReader.ReadString();
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
                            Program.tConsole.WriteLine();
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
                                    float num10 = (float)(num8 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num8;
                                    if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num8)
                                    {
                                        num8 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                                    }
                                    if (num10 > num9)
                                    {
                                        num9 = num10;
                                    }
                                    else
                                    {
                                        num10 = num9;
                                    }
                                    Program.printData("Settling liquids: " + (int)(num10 * 100f / 2f + 50f) + "%", true);
                                    Liquid.UpdateLiquid();
                                }
                                Liquid.quickSettle = false;
                                Program.tConsole.WriteLine();
                                Program.printData("Performing Water Check", true);
                                WorldGen.WaterCheck();
                                WorldGen.gen = false;
                                Program.tConsole.WriteLine();
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
            WorldGen.hellChest = 0;
            WorldGen.JungleX = 0;
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

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                float num10 = (float)i / (float)Main.maxTilesX;
                Program.printData("Generating world terrain: " + (int)(num10 * 100f + 1f) + "%", true);
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
                if ((i < 275 || i > Main.maxTilesX - 275) && num3 > (double)Main.maxTilesY * 0.25)
                {
                    num3 = (double)Main.maxTilesY * 0.25;
                    num2 = 1;
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
                int num11 = 0;
                while ((double)num11 < num3)
                {
                    Main.tile.At(i, num11).SetActive (false);
                    Main.tile.At(i, num11).SetLighted (true);
                    Main.tile.At(i, num11).SetFrameX (-1);
                    Main.tile.At(i, num11).SetFrameY (-1);
                    num11++;
                }
                for (int j = (int)num3; j < Main.maxTilesY; j++)
                {
                    if ((double)j < num4)
                    {
                        Main.tile.At(i, j).SetActive (true);
                        Main.tile.At(i, j).SetType (0);
                        Main.tile.At(i, j).SetFrameX (-1);
                        Main.tile.At(i, j).SetFrameY (-1);
                    }
                    else
                    {
                        Main.tile.At(i, j).SetActive (true);
                        Main.tile.At(i, j).SetType (1);
                        Main.tile.At(i, j).SetFrameX (-1);
                        Main.tile.At(i, j).SetFrameY (-1);
                    }
                }
            }
            Main.worldSurface = num6 + 25.0;
            Main.rockLayer = num8;
            double num12 = (double)((int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6);
            Main.rockLayer = Main.worldSurface + num12;
            WorldGen.waterLine = (int)(Main.rockLayer + (double)Main.maxTilesY) / 2;
            WorldGen.waterLine += WorldGen.genRand.Next(-100, 20);
            WorldGen.lavaLine = WorldGen.waterLine + WorldGen.genRand.Next(50, 80);
            int num13 = 0;
            Program.tConsole.WriteLine();
            Program.printData("Adding sand...");
            int num14 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.0007), (int)((double)Main.maxTilesX * 0.002));
            num14 += 2;
            for (int k = 0; k < num14; k++)
            {
                int num15 = WorldGen.genRand.Next(Main.maxTilesX);
                while ((float)num15 > (float)Main.maxTilesX * 0.45f && (float)num15 < (float)Main.maxTilesX * 0.55f)
                {
                    num15 = WorldGen.genRand.Next(Main.maxTilesX);
                }
                int num16 = WorldGen.genRand.Next(35, 90);
                if (k == 1)
                {
                    float num17 = (float)(Main.maxTilesX / 4200);
                    num16 += (int)((float)WorldGen.genRand.Next(20, 40) * num17);
                }
                if (WorldGen.genRand.Next(3) == 0)
                {
                    num16 *= 2;
                }
                if (k == 1)
                {
                    num16 *= 3;
                }
                int num18 = num15 - num16;
                num16 = WorldGen.genRand.Next(35, 90);
                if (WorldGen.genRand.Next(3) == 0)
                {
                    num16 *= 2;
                }
                if (k == 1)
                {
                    num16 *= 3;
                }
                int num19 = num15 + num16;
                if (num18 < 0)
                {
                    num18 = 0;
                }
                if (num19 > Main.maxTilesX)
                {
                    num19 = Main.maxTilesX;
                }
                if (k == 0)
                {
                    num18 = 0;
                    num19 = WorldGen.genRand.Next(260, 300);
                    if (num9 == 1)
                    {
                        num19 += 40;
                    }
                }
                else
                {
                    if (k == 2)
                    {
                        num18 = Main.maxTilesX - WorldGen.genRand.Next(260, 300);
                        num19 = Main.maxTilesX;
                        if (num9 == -1)
                        {
                            num18 -= 40;
                        }
                    }
                }
                int num20 = WorldGen.genRand.Next(50, 100);
                for (int l = num18; l < num19; l++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num20 += WorldGen.genRand.Next(-1, 2);
                        if (num20 < 50)
                        {
                            num20 = 50;
                        }
                        if (num20 > 100)
                        {
                            num20 = 100;
                        }
                    }
                    int num21 = 0;
                    while ((double)num21 < Main.worldSurface)
                    {
                        if (Main.tile.At(l, num21).Active)
                        {
                            int num22 = num20;
                            if (l - num18 < num22)
                            {
                                num22 = l - num18;
                            }
                            if (num19 - l < num22)
                            {
                                num22 = num19 - l;
                            }
                            num22 += WorldGen.genRand.Next(5);
                            for (int m = num21; m < num21 + num22; m++)
                            {
                                if (l > num18 + WorldGen.genRand.Next(5) && l < num19 - WorldGen.genRand.Next(5))
                                {
                                    Main.tile.At(l, m).SetType (53);
                                }
                            }
                            break;
                        }
                        num21++;
                    }
                }
            }
            for (int n = 0; n < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-06); n++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), (double)WorldGen.genRand.Next(15, 70), WorldGen.genRand.Next(20, 130), 53, false, 0f, 0f, false, true);
            }
            WorldGen.numMCaves = 0;
            Program.tConsole.WriteLine();
            Program.printData("Generating hills...");
            for (int num23 = 0; num23 < (int)((double)Main.maxTilesX * 0.0008); num23++)
            {
                int num24 = 0;
                bool flag = false;
                bool flag2 = false;
                int num25 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
                while (!flag2)
                {
                    flag2 = true;
                    while (num25 > Main.maxTilesX / 2 - 100 && num25 < Main.maxTilesX / 2 + 100)
                    {
                        num25 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
                    }
                    for (int num26 = 0; num26 < WorldGen.numMCaves; num26++)
                    {
                        if (num25 > WorldGen.mCaveX[num26] - 50 && num25 < WorldGen.mCaveX[num26] + 50)
                        {
                            num24++;
                            flag2 = false;
                            break;
                        }
                    }
                    if (num24 >= 200)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    int num27 = 0;
                    while ((double)num27 < Main.worldSurface)
                    {
                        if (Main.tile.At(num25, num27).Active)
                        {
                            WorldGen.Mountinater(num25, num27);
                            WorldGen.mCaveX[WorldGen.numMCaves] = num25;
                            WorldGen.mCaveY[WorldGen.numMCaves] = num27;
                            WorldGen.numMCaves++;
                            break;
                        }
                        num27++;
                    }
                }
            }
            Program.tConsole.WriteLine();
            for (int num28 = 1; num28 < Main.maxTilesX - 1; num28++)
            {
                float num29 = (float)num28 / (float)Main.maxTilesX;
                Program.printData("Putting dirt behind dirt: " + (int)(num29 * 100f + 1f) + "%", true);
                bool flag3 = false;
                num13 += WorldGen.genRand.Next(-1, 2);
                if (num13 < 0)
                {
                    num13 = 0;
                }
                if (num13 > 10)
                {
                    num13 = 10;
                }
                int num30 = 0;
                while ((double)num30 < Main.worldSurface + 10.0 && (double)num30 <= Main.worldSurface + (double)num13)
                {
                    if (flag3)
                    {
                        Main.tile.At(num28, num30).SetWall (2);
                    }
                    if (Main.tile.At(num28, num30).Active && Main.tile.At(num28 - 1, num30).Active && Main.tile.At(num28 + 1, num30).Active && Main.tile.At(num28, num30 + 1).Active && Main.tile.At(num28 - 1, num30 + 1).Active && Main.tile.At(num28 + 1, num30 + 1).Active)
                    {
                        flag3 = true;
                    }
                    num30++;
                }
            }
            Program.tConsole.WriteLine();
            Program.printData("Placing rocks in the dirt...", true);
            for (int num31 = 0; num31 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num31++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5 + 1), (double)WorldGen.genRand.Next(4, 15), WorldGen.genRand.Next(5, 40), 1, false, 0f, 0f, false, true);
            }
            for (int num32 = 0; num32 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num32++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6 + 1), (double)WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
            }
            for (int num33 = 0; num33 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0045); num33++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8 + 1), (double)WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(2, 23), 1, false, 0f, 0f, false, true);
            }
            Program.tConsole.WriteLine();
            Program.printData("Placing dirt in the rocks...", true);
            for (int num34 = 0; num34 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.005); num34++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 0, false, 0f, 0f, false, true);
            }
            Program.tConsole.WriteLine();
            Program.printData("Adding clay...", true);
            for (int num35 = 0; num35 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num35++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5), (double)WorldGen.genRand.Next(4, 14), WorldGen.genRand.Next(10, 50), 40, false, 0f, 0f, false, true);
            }
            for (int num36 = 0; num36 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 5E-05); num36++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6 + 1), (double)WorldGen.genRand.Next(8, 14), WorldGen.genRand.Next(15, 45), 40, false, 0f, 0f, false, true);
            }
            for (int num37 = 0; num37 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num37++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8 + 1), (double)WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(5, 50), 40, false, 0f, 0f, false, true);
            }
            for (int num38 = 5; num38 < Main.maxTilesX - 5; num38++)
            {
                int num39 = 1;
                while ((double)num39 < Main.worldSurface - 1.0)
                {
                    if (Main.tile.At(num38, num39).Active)
                    {
                        for (int num40 = num39; num40 < num39 + 5; num40++)
                        {
                            if (Main.tile.At(num38, num40).Type == 40)
                            {
                                Main.tile.At(num38, num40).SetType (0);
                            }
                        }
                        break;
                    }
                    num39++;
                }
            }
            Program.tConsole.WriteLine();
            for (int num41 = 0; num41 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015); num41++)
            {
                float num42 = (float)((double)num41 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015));
                Program.printData("Making random holes: " + (int)(num42 * 100f + 1f) + "%", true);
                int type = -1;
                if (WorldGen.genRand.Next(5) == 0)
                {
                    type = -2;
                }
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 20), type, false, 0f, 0f, false, true);
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, Main.maxTilesY), (double)WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(7, 30), type, false, 0f, 0f, false, true);
            }
            Program.tConsole.WriteLine();
            for (int num43 = 0; num43 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num43++)
            {
                float num44 = (float)((double)num43 / ((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05));
                Program.printData("Generating small caves: " + (int)(num44 * 100f + 1f) + "%", true);
                if (num8 <= (double)Main.maxTilesY)
                {
                    int type2 = -1;
                    if (WorldGen.genRand.Next(6) == 0)
                    {
                        type2 = -2;
                    }
                    WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num8 + 1), (double)WorldGen.genRand.Next(5, 15), WorldGen.genRand.Next(30, 200), type2, false, 0f, 0f, false, true);
                }
            }
            Program.tConsole.WriteLine();
            for (int num45 = 0; num45 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015); num45++)
            {
                float num46 = (float)((double)num45 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015));
                Program.printData("Generating large caves: " + (int)(num46 * 100f + 1f) + "%", true);
                if (num8 <= (double)Main.maxTilesY)
                {
                    int type3 = -1;
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        type3 = -2;
                    }
                    WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num8, Main.maxTilesY), (double)WorldGen.genRand.Next(6, 20), WorldGen.genRand.Next(50, 300), type3, false, 0f, 0f, false, true);
                }
            }
            Program.tConsole.WriteLine();
            Program.printData("Generating surface caves...", true);
            for (int num47 = 0; num47 < (int)((double)Main.maxTilesX * 0.0025); num47++)
            {
                int num48 = WorldGen.genRand.Next(0, Main.maxTilesX);
                int num49 = 0;
                while ((double)num49 < num6)
                {
                    if (Main.tile.At(num48, num49).Active)
                    {
                        WorldGen.TileRunner(num48, num49, (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(5, 50), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 1f, false, true);
                        break;
                    }
                    num49++;
                }
            }
            for (int num50 = 0; num50 < (int)((double)Main.maxTilesX * 0.0007); num50++)
            {
                int num48 = WorldGen.genRand.Next(0, Main.maxTilesX);
                int num51 = 0;
                while ((double)num51 < num6)
                {
                    if (Main.tile.At(num48, num51).Active)
                    {
                        WorldGen.TileRunner(num48, num51, (double)WorldGen.genRand.Next(10, 15), WorldGen.genRand.Next(50, 130), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        break;
                    }
                    num51++;
                }
            }
            for (int num52 = 0; num52 < (int)((double)Main.maxTilesX * 0.0003); num52++)
            {
                int num48 = WorldGen.genRand.Next(0, Main.maxTilesX);
                int num53 = 0;
                while ((double)num53 < num6)
                {
                    if (Main.tile.At(num48, num53).Active)
                    {
                        WorldGen.TileRunner(num48, num53, (double)WorldGen.genRand.Next(12, 25), WorldGen.genRand.Next(150, 500), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 4f, false, true);
                        WorldGen.TileRunner(num48, num53, (double)WorldGen.genRand.Next(8, 17), WorldGen.genRand.Next(60, 200), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        WorldGen.TileRunner(num48, num53, (double)WorldGen.genRand.Next(5, 13), WorldGen.genRand.Next(40, 170), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        break;
                    }
                    num53++;
                }
            }
            for (int num54 = 0; num54 < (int)((double)Main.maxTilesX * 0.0004); num54++)
            {
                int num48 = WorldGen.genRand.Next(0, Main.maxTilesX);
                int num55 = 0;
                while ((double)num55 < num6)
                {
                    if (Main.tile.At(num48, num55).Active)
                    {
                        WorldGen.TileRunner(num48, num55, (double)WorldGen.genRand.Next(7, 12), WorldGen.genRand.Next(150, 250), -1, false, 0f, 1f, true, true);
                        break;
                    }
                    num55++;
                }
            }
            for (int num56 = 0; num56 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002); num56++)
            {
                int num57 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
                int num58 = WorldGen.genRand.Next((int)num5, (int)num6);
                if (num58 >= Main.maxTilesY)
                {
                    num58 = Main.maxTilesY - 2;
                }
                if (Main.tile.At(num57 - 1, num58).Active && Main.tile.At(num57 - 1, num58).Type == 0 && Main.tile.At(num57 + 1, num58).Active && Main.tile.At(num57 + 1, num58).Type == 0 && Main.tile.At(num57, num58 - 1).Active && Main.tile.At(num57, num58 - 1).Type == 0 && Main.tile.At(num57, num58 + 1).Active && Main.tile.At(num57, num58 + 1).Type == 0)
                {
                    Main.tile.At(num57, num58).SetActive (true);
                    Main.tile.At(num57, num58).SetType (2);
                }
                num57 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
                num58 = WorldGen.genRand.Next(0, (int)num5);
                if (num58 >= Main.maxTilesY)
                {
                    num58 = Main.maxTilesY - 2;
                }
                if (Main.tile.At(num57 - 1, num58).Active && Main.tile.At(num57 - 1, num58).Type == 0 && Main.tile.At(num57 + 1, num58).Active && Main.tile.At(num57 + 1, num58).Type == 0 && Main.tile.At(num57, num58 - 1).Active && Main.tile.At(num57, num58 - 1).Type == 0 && Main.tile.At(num57, num58 + 1).Active && Main.tile.At(num57, num58 + 1).Type == 0)
                {
                    Main.tile.At(num57, num58).SetActive (true);
                    Main.tile.At(num57, num58).SetType (2);
                }
            }
            Program.tConsole.WriteLine();
            Program.printData("Generating jungle: 0%", true);
            float num59 = (float)(Main.maxTilesX / 4200);
            num59 *= 1.5f;
            int num60 = 0;
            float num61 = (float)WorldGen.genRand.Next(15, 30) * 0.01f;
            if (num9 == -1)
            {
                num61 = 1f - num61;
                num60 = (int)((float)Main.maxTilesX * num61);
            }
            else
            {
                num60 = (int)((float)Main.maxTilesX * num61);
            }
            int num62 = (int)((double)Main.maxTilesY + Main.rockLayer) / 2;
            num60 += WorldGen.genRand.Next((int)(-100f * num59), (int)(101f * num59));
            num62 += WorldGen.genRand.Next((int)(-100f * num59), (int)(101f * num59));
            int num63 = num60;
            int num64 = num62;
            WorldGen.TileRunner(num60, num62, (double)WorldGen.genRand.Next((int)(250f * num59), (int)(500f * num59)), WorldGen.genRand.Next(50, 150), 59, false, (float)(num9 * 3), 0f, false, true);
            Program.printData("Generating jungle: 15%", true);
            num60 += WorldGen.genRand.Next((int)(-250f * num59), (int)(251f * num59));
            num62 += WorldGen.genRand.Next((int)(-150f * num59), (int)(151f * num59));
            int num65 = num60;
            int num66 = num62;
            int num67 = num60;
            int num68 = num62;
            WorldGen.TileRunner(num60, num62, (double)WorldGen.genRand.Next((int)(250f * num59), (int)(500f * num59)), WorldGen.genRand.Next(50, 150), 59, false, 0f, 0f, false, true);
            Program.printData("Generating jungle: 30%", true);
            num60 += WorldGen.genRand.Next((int)(-400f * num59), (int)(401f * num59));
            num62 += WorldGen.genRand.Next((int)(-150f * num59), (int)(151f * num59));
            int num69 = num60;
            int num70 = num62;
            WorldGen.TileRunner(num60, num62, (double)WorldGen.genRand.Next((int)(250f * num59), (int)(500f * num59)), WorldGen.genRand.Next(50, 150), 59, false, (float)(num9 * -3), 0f, false, true);
            Program.printData("Generating jungle: 45%", true);
            num60 = (num63 + num65 + num69) / 3;
            num62 = (num64 + num66 + num70) / 3;
            WorldGen.TileRunner(num60, num62, (double)WorldGen.genRand.Next((int)(400f * num59), (int)(600f * num59)), 10000, 59, false, 0f, -20f, true, true);
            WorldGen.JungleRunner(num60, num62);
            Program.printData("Generating jungle: 60%", true);
            num60 = num67;
            num62 = num68;
            int num71 = 0;
            while ((float)num71 <= 20f * num59)
            {
                Program.printData("Generating jungle: " + (int)(60f + (float)num71 / num59) + "%", true);
                num60 += WorldGen.genRand.Next((int)(-5f * num59), (int)(6f * num59));
                num62 += WorldGen.genRand.Next((int)(-5f * num59), (int)(6f * num59));
                WorldGen.TileRunner(num60, num62, (double)WorldGen.genRand.Next(40, 100), WorldGen.genRand.Next(300, 500), 59, false, 0f, 0f, false, true);
                num71++;
            }
            int num72 = 0;
            while ((float)num72 <= 10f * num59)
            {
                Program.printData("Generating jungle: " + (int)(80f + (float)num72 / num59 * 2f) + "%", true);
                num60 = num67 + WorldGen.genRand.Next((int)(-600f * num59), (int)(600f * num59));
                num62 = num68 + WorldGen.genRand.Next((int)(-200f * num59), (int)(200f * num59));
                while (num60 < 1 || num60 >= Main.maxTilesX - 1 || num62 < 1 || num62 >= Main.maxTilesY - 1 || Main.tile.At(num60, num62).Type != 59)
                {
                    num60 = num67 + WorldGen.genRand.Next((int)(-600f * num59), (int)(600f * num59));
                    num62 = num68 + WorldGen.genRand.Next((int)(-200f * num59), (int)(200f * num59));
                }
                int num73 = 0;
                while ((float)num73 < 8f * num59)
                {
                    num60 += WorldGen.genRand.Next(-30, 31);
                    num62 += WorldGen.genRand.Next(-30, 31);
                    int type4 = -1;
                    if (WorldGen.genRand.Next(7) == 0)
                    {
                        type4 = -2;
                    }
                    WorldGen.TileRunner(num60, num62, (double)WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(30, 70), type4, false, 0f, 0f, false, true);
                    num73++;
                }
                num72++;
            }
            int num74 = 0;
            while ((float)num74 <= 300f * num59)
            {
                num60 = num67 + WorldGen.genRand.Next((int)(-600f * num59), (int)(600f * num59));
                num62 = num68 + WorldGen.genRand.Next((int)(-200f * num59), (int)(200f * num59));
                while (num60 < 1 || num60 >= Main.maxTilesX - 1 || num62 < 1 || num62 >= Main.maxTilesY - 1 || Main.tile.At(num60, num62).Type != 59)
                {
                    num60 = num67 + WorldGen.genRand.Next((int)(-600f * num59), (int)(600f * num59));
                    num62 = num68 + WorldGen.genRand.Next((int)(-200f * num59), (int)(200f * num59));
                }
                WorldGen.TileRunner(num60, num62, (double)WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
                if (WorldGen.genRand.Next(4) == 0)
                {
                    int type5 = WorldGen.genRand.Next(63, 69);
                    WorldGen.TileRunner(num60 + WorldGen.genRand.Next(-1, 2), num62 + WorldGen.genRand.Next(-1, 2), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(4, 8), type5, false, 0f, 0f, false, true);
                }
                num74++;
            }
            num60 = num67;
            num62 = num68;
            float num75 = (float)WorldGen.genRand.Next(6, 10);
            float num76 = (float)(Main.maxTilesX / 4200);
            num75 *= num76;
            int num77 = 0;
            while ((float)num77 < num75)
            {
                bool flag4 = true;
                while (flag4)
                {
                    num60 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                    num62 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 300);
                    if (Main.tile.At(num60, num62).Type == 59)
                    {
                        flag4 = false;
                        int num78 = WorldGen.genRand.Next(2, 4);
                        int num79 = WorldGen.genRand.Next(2, 4);
                        for (int num80 = num60 - num78 - 1; num80 <= num60 + num78 + 1; num80++)
                        {
                            for (int num81 = num62 - num79 - 1; num81 <= num62 + num79 + 1; num81++)
                            {
                                Main.tile.At(num80, num81).SetActive (true);
                                Main.tile.At(num80, num81).SetType (45);
                                Main.tile.At(num80, num81).SetLiquid (0);
                                Main.tile.At(num80, num81).SetLava (false);
                            }
                        }
                        for (int num82 = num60 - num78; num82 <= num60 + num78; num82++)
                        {
                            for (int num83 = num62 - num79; num83 <= num62 + num79; num83++)
                            {
                                Main.tile.At(num82, num83).SetActive (false);
                                Main.tile.At(num82, num83).SetWall (10);
                            }
                        }
                        bool flag5 = false;
                        int num84 = 0;
                        while (!flag5 && num84 < 100)
                        {
                            num84++;
                            int num85 = WorldGen.genRand.Next(num60 - num78, num60 + num78 + 1);
                            int num86 = WorldGen.genRand.Next(num62 - num79, num62 + num79 - 2);
                            WorldGen.PlaceTile(num85, num86, 4, true, false, -1, 0);
                            if (Main.tile.At(num85, num86).Type == 4)
                            {
                                flag5 = true;
                            }
                        }
                        for (int num87 = num60 - num78 - 1; num87 <= num60 + num78 + 1; num87++)
                        {
                            for (int num88 = num62 + num79 - 2; num88 <= num62 + num79; num88++)
                            {
                                Main.tile.At(num87, num88).SetActive (false);
                            }
                        }
                        for (int num89 = num60 - num78 - 1; num89 <= num60 + num78 + 1; num89++)
                        {
                            for (int num90 = num62 + num79 - 2; num90 <= num62 + num79 - 1; num90++)
                            {
                                Main.tile.At(num89, num90).SetActive (false);
                            }
                        }
                        for (int num91 = num60 - num78 - 1; num91 <= num60 + num78 + 1; num91++)
                        {
                            int num92 = 4;
                            int num93 = num62 + num79 + 2;
                            while (!Main.tile.At(num91, num93).Active && num93 < Main.maxTilesY && num92 > 0)
                            {
                                Main.tile.At(num91, num93).SetActive (true);
                                Main.tile.At(num91, num93).SetType (59);
                                num93++;
                                num92--;
                            }
                        }
                        num78 -= WorldGen.genRand.Next(1, 3);
                        int num94 = num62 - num79 - 2;
                        while (num78 > -1)
                        {
                            for (int num95 = num60 - num78 - 1; num95 <= num60 + num78 + 1; num95++)
                            {
                                Main.tile.At(num95, num94).SetActive (true);
                                Main.tile.At(num95, num94).SetType (45);
                            }
                            num78 -= WorldGen.genRand.Next(1, 3);
                            num94--;
                        }
                        WorldGen.JChestX[WorldGen.numJChests] = num60;
                        WorldGen.JChestY[WorldGen.numJChests] = num62;
                        WorldGen.numJChests++;
                    }
                }
                num77++;
            }
            for (int num96 = 0; num96 < Main.maxTilesX; num96++)
            {
                for (int num97 = 0; num97 < Main.maxTilesY; num97++)
                {
                    if (Main.tile.At(num96, num97).Active)
                    {
                        WorldGen.SpreadGrass(num96, num97, 59, 60, true);
                    }
                }
            }
            WorldGen.numIslandHouses = 0;
            WorldGen.houseCount = 0;
            Program.tConsole.WriteLine();
            Program.printData("Generating floating islands...", true);
            for (int num98 = 0; num98 < ficount; num98++)
            {
                int num99 = 0;
                bool flag6 = false;
                int num100 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
                bool flag7 = false;
                while (!flag7)
                {
                    flag7 = true;
                    while (num100 > Main.maxTilesX / 2 - 80 && num100 < Main.maxTilesX / 2 + 80)
                    {
                        num100 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
                    }
                    for (int num101 = 0; num101 < WorldGen.numIslandHouses; num101++)
                    {
                        if (num100 > WorldGen.fihX[num101] - 80 && num100 < WorldGen.fihX[num101] + 80)
                        {
                            num99++;
                            flag7 = false;
                            break;
                        }
                    }
                    if (num99 >= 200)
                    {
                        flag6 = true;
                        break;
                    }
                }
                if (!flag6)
                {
                    int num102 = 200;
                    while ((double)num102 < Main.worldSurface)
                    {
                        if (Main.tile.At(num100, num102).Active)
                        {
                            int num103 = num100;
                            int num104 = WorldGen.genRand.Next(90, num102 - 100);
                            while ((double)num104 > num5 - 50.0)
                            {
                                num104--;
                            }
                            WorldGen.FloatingIsland(num103, num104);
                            WorldGen.fihX[WorldGen.numIslandHouses] = num103;
                            WorldGen.fihY[WorldGen.numIslandHouses] = num104;
                            WorldGen.numIslandHouses++;
                            break;
                        }
                        num102++;
                    }
                }
            }
            Program.tConsole.WriteLine();
            Program.printData("Adding mushroom patches...", true);
            for (int num105 = 0; num105 < Main.maxTilesX / 300; num105++)
            {
                int i2 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.3), (int)((double)Main.maxTilesX * 0.7));
                int j2 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);
                WorldGen.ShroomPatch(i2, j2);
            }
            for (int num106 = 0; num106 < Main.maxTilesX; num106++)
            {
                for (int num107 = (int)Main.worldSurface; num107 < Main.maxTilesY; num107++)
                {
                    if (Main.tile.At(num106, num107).Active)
                    {
                        WorldGen.SpreadGrass(num106, num107, 59, 70, false);
                    }
                }
            }
            Program.tConsole.WriteLine();
            Program.printData("Placing mud in the dirt...", true);
            for (int num108 = 0; num108 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001); num108++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 59, false, 0f, 0f, false, true);
            }
            Program.tConsole.WriteLine();
            Program.printData("Adding shinies...", true);
            for (int num109 = 0; num109 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); num109++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), 7, false, 0f, 0f, false, true);
            }
            for (int num110 = 0; num110 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num110++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), 7, false, 0f, 0f, false, true);
            }
            for (int num111 = 0; num111 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num111++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 7, false, 0f, 0f, false, true);
            }
            for (int num112 = 0; num112 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num112++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(2, 5), 6, false, 0f, 0f, false, true);
            }
            for (int num113 = 0; num113 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num113++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), 6, false, 0f, 0f, false, true);
            }
            for (int num114 = 0; num114 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num114++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 6, false, 0f, 0f, false, true);
            }
            for (int num115 = 0; num115 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num115++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), 9, false, 0f, 0f, false, true);
            }
            for (int num116 = 0; num116 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00017); num116++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
            }
            for (int num117 = 0; num117 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00017); num117++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
            }
            for (int num118 = 0; num118 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num118++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
            }
            for (int num119 = 0; num119 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num119++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5 - 20), (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
            }
            for (int num120 = 0; num120 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num120++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 4), WorldGen.genRand.Next(3, 6), 22, false, 0f, 0f, false, true);
            }
            Program.tConsole.WriteLine();
            Program.printData("Adding webs...", true);
            for (int num121 = 0; num121 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001); num121++)
            {
                int num122 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                int num123 = WorldGen.genRand.Next((int)num5, Main.maxTilesY - 20);
                if (num121 < WorldGen.numMCaves)
                {
                    num122 = WorldGen.mCaveX[num121];
                    num123 = WorldGen.mCaveY[num121];
                }
                if (!Main.tile.At(num122, num123).Active)
                {
                    if ((double)num123 <= Main.worldSurface)
                    {
                        if (Main.tile.At(num122, num123).Wall <= 0)
                        {
                            goto IL_2C05;
                        }
                    }
                    while (!Main.tile.At(num122, num123).Active && num123 > (int)num5)
                    {
                        num123--;
                    }
                    num123++;
                    int num124 = 1;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num124 = -1;
                    }
                    while (!Main.tile.At(num122, num123).Active && num122 > 10 && num122 < Main.maxTilesX - 10)
                    {
                        num122 += num124;
                    }
                    num122 -= num124;
                    if ((double)num123 > Main.worldSurface || Main.tile.At(num122, num123).Wall > 0)
                    {
                        WorldGen.TileRunner(num122, num123, (double)WorldGen.genRand.Next(4, 13), WorldGen.genRand.Next(2, 5), 51, true, (float)num124, -1f, false, false);
                    }
                }
            IL_2C05: ;
            }
            Program.tConsole.WriteLine();
            Program.printData("Creating underworld: 0%", true);
            int num125 = Main.maxTilesY - WorldGen.genRand.Next(150, 190);
            for (int num126 = 0; num126 < Main.maxTilesX; num126++)
            {
                num125 += WorldGen.genRand.Next(-3, 4);
                if (num125 < Main.maxTilesY - 190)
                {
                    num125 = Main.maxTilesY - 190;
                }
                if (num125 > Main.maxTilesY - 160)
                {
                    num125 = Main.maxTilesY - 160;
                }
                for (int num127 = num125 - 20 - WorldGen.genRand.Next(3); num127 < Main.maxTilesY; num127++)
                {
                    if (num127 >= num125)
                    {
                        Main.tile.At(num126, num127).SetActive (false);
                        Main.tile.At(num126, num127).SetLava (false);
                        Main.tile.At(num126, num127).SetLiquid (0);
                    }
                    else
                    {
                        Main.tile.At(num126, num127).SetType (57);
                    }
                }
            }
            int num128 = Main.maxTilesY - WorldGen.genRand.Next(40, 70);
            for (int num129 = 10; num129 < Main.maxTilesX - 10; num129++)
            {
                num128 += WorldGen.genRand.Next(-10, 11);
                if (num128 > Main.maxTilesY - 60)
                {
                    num128 = Main.maxTilesY - 60;
                }
                if (num128 < Main.maxTilesY - 100)
                {
                    num128 = Main.maxTilesY - 120;
                }
                for (int num130 = num128; num130 < Main.maxTilesY - 10; num130++)
                {
                    if (!Main.tile.At(num129, num130).Active)
                    {
                        Main.tile.At(num129, num130).SetLava (true);
                        Main.tile.At(num129, num130).SetLiquid (255);
                    }
                }
            }
            for (int num131 = 0; num131 < Main.maxTilesX; num131++)
            {
                if (WorldGen.genRand.Next(50) == 0)
                {
                    int num132 = Main.maxTilesY - 65;
                    while (!Main.tile.At(num131, num132).Active && num132 > Main.maxTilesY - 135)
                    {
                        num132--;
                    }
                    WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), num132 + WorldGen.genRand.Next(20, 50), (double)WorldGen.genRand.Next(15, 20), 1000, 57, true, 0f, (float)WorldGen.genRand.Next(1, 3), true, true);
                }
            }
            Liquid.QuickWater(-2, -1, -1);
            for (int num133 = 0; num133 < Main.maxTilesX; num133++)
            {
                float num134 = (float)num133 / (float)(Main.maxTilesX - 1);
                Program.printData("Creating underworld: " + (int)(num134 * 100f / 2f + 50f) + "%", true);
                if (WorldGen.genRand.Next(13) == 0)
                {
                    int num135 = Main.maxTilesY - 65;
                    while ((Main.tile.At(num133, num135).Liquid > 0 || Main.tile.At(num133, num135).Active) && num135 > Main.maxTilesY - 140)
                    {
                        num135--;
                    }
                    WorldGen.TileRunner(num133, num135 - WorldGen.genRand.Next(2, 5), (double)WorldGen.genRand.Next(5, 30), 1000, 57, true, 0f, (float)WorldGen.genRand.Next(1, 3), true, true);
                    float num136 = (float)WorldGen.genRand.Next(1, 3);
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        num136 *= 0.5f;
                    }
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        WorldGen.TileRunner(num133, num135 - WorldGen.genRand.Next(2, 5), (double)((int)((float)WorldGen.genRand.Next(5, 15) * num136)), (int)((float)WorldGen.genRand.Next(10, 15) * num136), 57, true, 1f, 0.3f, false, true);
                    }
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num136 = (float)WorldGen.genRand.Next(1, 3);
                        WorldGen.TileRunner(num133, num135 - WorldGen.genRand.Next(2, 5), (double)((int)((float)WorldGen.genRand.Next(5, 15) * num136)), (int)((float)WorldGen.genRand.Next(10, 15) * num136), 57, true, -1f, 0.3f, false, true);
                    }
                    WorldGen.TileRunner(num133 + WorldGen.genRand.Next(-10, 10), num135 + WorldGen.genRand.Next(-10, 10), (double)WorldGen.genRand.Next(5, 15), WorldGen.genRand.Next(5, 10), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        WorldGen.TileRunner(num133 + WorldGen.genRand.Next(-10, 10), num135 + WorldGen.genRand.Next(-10, 10), (double)WorldGen.genRand.Next(10, 30), WorldGen.genRand.Next(10, 20), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        WorldGen.TileRunner(num133 + WorldGen.genRand.Next(-15, 15), num135 + WorldGen.genRand.Next(-15, 10), (double)WorldGen.genRand.Next(15, 30), WorldGen.genRand.Next(5, 20), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
                    }
                }
            }
            for (int num137 = 0; num137 < Main.maxTilesX; num137++)
            {
                if (!Main.tile.At(num137, Main.maxTilesY - 145).Active)
                {
                    Main.tile.At(num137, Main.maxTilesY - 145).SetLiquid (255);
                    Main.tile.At(num137, Main.maxTilesY - 145).SetLava (true);
                }
                if (!Main.tile.At(num137, Main.maxTilesY - 144).Active)
                {
                    Main.tile.At(num137, Main.maxTilesY - 144).SetLiquid (255);
                    Main.tile.At(num137, Main.maxTilesY - 144).SetLava (true);
                }
            }
            for (int num138 = 0; num138 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002); num138++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(Main.maxTilesY - 140, Main.maxTilesY), (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), 58, false, 0f, 0f, false, true);
            }
            WorldGen.AddHellHouses();
            int num139 = WorldGen.genRand.Next(2, (int)((double)Main.maxTilesX * 0.005));
            Program.tConsole.WriteLine();
            for (int num140 = 0; num140 < num139; num140++)
            {
                float num141 = (float)num140 / (float)num139;
                Program.printData("Adding water bodies: " + (int)(num141 * 100f) + "%", true);
                int num142 = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                while (num142 > Main.maxTilesX / 2 - 50 && num142 < Main.maxTilesX / 2 + 50)
                {
                    num142 = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                }
                int num143 = (int)num5 - 20;
                while (!Main.tile.At(num142, num143).Active)
                {
                    num143++;
                }
                WorldGen.Lakinater(num142, num143);
            }

            Program.tConsole.WriteLine();

            int x = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.05), (int)((double)Main.maxTilesX * 0.2)); //Left?
            int x2 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.8), (int)((double)Main.maxTilesX * 0.95)); //Right?
            int y = 0;

            if (numDungeons >= 2) //Custom Dungeons (0 allowed?)
            {
                numDungeons = 2; //Limt dungeons at 2, May not fit?
            }
            else //1 dungeon
            {
                if (num9 != -1) 
                {
                    x = x2; //X axis = right
                }
            }

            for (int dnum = 0; dnum < numDungeons; dnum++)
            {
                y = (int)((Main.rockLayer + (double)Main.maxTilesY) / 2.0) + WorldGen.genRand.Next(-200, 200); //Generate a custon Y each time
                if (dnum == 1)
                {
                    //num9 = direction
                    x = ((x == x2) ? x : x2); //we want the opposite of the original
                }
                WorldGen.MakeDungeon(x, y, 41, 7);
            }

            if (num9 != -1)
            {
                num9 = 1;
            }

            int num144 = 0;

            while ((double)num144 < (double)Main.maxTilesX * 0.00045)
            {
                float num145 = (float)((double)num144 / ((double)Main.maxTilesX * 0.00045));
                Program.printData("Making the world evil: " + (int)(num145 * 100f) + "%", true);
                bool flag8 = false;
                int num146 = 0;
                int num147 = 0;
                int num148 = 0;
                while (!flag8)
                {
                    int num149 = 0;
                    flag8 = true;
                    int num150 = Main.maxTilesX / 2;
                    int num151 = 200;
                    num146 = WorldGen.genRand.Next(320, Main.maxTilesX - 320);
                    num147 = num146 - WorldGen.genRand.Next(200) - 100;
                    num148 = num146 + WorldGen.genRand.Next(200) + 100;
                    if (num147 < 285)
                    {
                        num147 = 285;
                    }
                    if (num148 > Main.maxTilesX - 285)
                    {
                        num148 = Main.maxTilesX - 285;
                    }
                    if (num146 > num150 - num151 && num146 < num150 + num151)
                    {
                        flag8 = false;
                    }
                    if (num147 > num150 - num151 && num147 < num150 + num151)
                    {
                        flag8 = false;
                    }
                    if (num148 > num150 - num151 && num148 < num150 + num151)
                    {
                        flag8 = false;
                    }
                    for (int num152 = num147; num152 < num148; num152++)
                    {
                        for (int num153 = 0; num153 < (int)Main.worldSurface; num153 += 5)
                        {
                            if (Main.tile.At(num152, num153).Active && Main.tileDungeon[(int)Main.tile.At(num152, num153).Type])
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
                    if (num149 < 200 && WorldGen.JungleX > num147 && WorldGen.JungleX < num148)
                    {
                        num149++;
                        flag8 = false;
                    }
                }
                int num154 = 0;
                for (int num155 = num147; num155 < num148; num155++)
                {
                    if (num154 > 0)
                    {
                        num154--;
                    }
                    if (num155 == num146 || num154 == 0)
                    {
                        int num156 = (int)num5;
                        while ((double)num156 < Main.worldSurface - 1.0)
                        {
                            if (Main.tile.At(num155, num156).Active || Main.tile.At(num155, num156).Wall > 0)
                            {
                                if (num155 == num146)
                                {
                                    num154 = 20;
                                    WorldGen.ChasmRunner(num155, num156, WorldGen.genRand.Next(150) + 150, true);
                                    break;
                                }
                                if (WorldGen.genRand.Next(35) == 0 && num154 == 0)
                                {
                                    num154 = 30;
                                    bool makeOrb = true;
                                    WorldGen.ChasmRunner(num155, num156, WorldGen.genRand.Next(50) + 50, makeOrb);
                                    break;
                                }
                                break;
                            }
                            else
                            {
                                num156++;
                            }
                        }
                    }
                    int num157 = (int)num5;
                    while ((double)num157 < Main.worldSurface - 1.0)
                    {
                        if (Main.tile.At(num155, num157).Active)
                        {
                            int num158 = num157 + WorldGen.genRand.Next(10, 14);
                            for (int num159 = num157; num159 < num158; num159++)
                            {
                                if ((Main.tile.At(num155, num159).Type == 59 || Main.tile.At(num155, num159).Type == 60) && num155 >= num147 + WorldGen.genRand.Next(5) && num155 < num148 - WorldGen.genRand.Next(5))
                                {
                                    Main.tile.At(num155, num159).SetType (0);
                                }
                            }
                            break;
                        }
                        num157++;
                    }
                }
                double num160 = Main.worldSurface + 40.0;
                for (int num161 = num147; num161 < num148; num161++)
                {
                    num160 += (double)WorldGen.genRand.Next(-2, 3);
                    if (num160 < Main.worldSurface + 30.0)
                    {
                        num160 = Main.worldSurface + 30.0;
                    }
                    if (num160 > Main.worldSurface + 50.0)
                    {
                        num160 = Main.worldSurface + 50.0;
                    }
                    int num48 = num161;
                    bool flag9 = false;
                    int num162 = (int)num5;
                    while ((double)num162 < num160)
                    {
                        if (Main.tile.At(num48, num162).Active)
                        {
                            if (Main.tile.At(num48, num162).Type == 53 && num48 >= num147 + WorldGen.genRand.Next(5) && num48 <= num148 - WorldGen.genRand.Next(5))
                            {
                                Main.tile.At(num48, num162).SetType (0);
                            }
                            if (Main.tile.At(num48, num162).Type == 0 && (double)num162 < Main.worldSurface - 1.0 && !flag9)
                            {
                                WorldGen.SpreadGrass(num48, num162, 0, 23, true);
                            }
                            flag9 = true;
                            if (Main.tile.At(num48, num162).Type == 1 && num48 >= num147 + WorldGen.genRand.Next(5) && num48 <= num148 - WorldGen.genRand.Next(5))
                            {
                                Main.tile.At(num48, num162).SetType (25);
                            }
                            if (Main.tile.At(num48, num162).Type == 2)
                            {
                                Main.tile.At(num48, num162).SetType (23);
                            }
                        }
                        num162++;
                    }
                }
                for (int num163 = num147; num163 < num148; num163++)
                {
                    for (int num164 = 0; num164 < Main.maxTilesY - 50; num164++)
                    {
                        if (Main.tile.At(num163, num164).Active && Main.tile.At(num163, num164).Type == 31)
                        {
                            int num165 = num163 - 13;
                            int num166 = num163 + 13;
                            int num167 = num164 - 13;
                            int num168 = num164 + 13;
                            for (int num169 = num165; num169 < num166; num169++)
                            {
                                if (num169 > 10 && num169 < Main.maxTilesX - 10)
                                {
                                    for (int num170 = num167; num170 < num168; num170++)
                                    {
                                        if (Math.Abs(num169 - num163) + Math.Abs(num170 - num164) < 9 + WorldGen.genRand.Next(11) && WorldGen.genRand.Next(3) != 0 && Main.tile.At(num169, num170).Type != 31)
                                        {
                                            Main.tile.At(num169, num170).SetActive (true);
                                            Main.tile.At(num169, num170).SetType (25);
                                            if (Math.Abs(num169 - num163) <= 1 && Math.Abs(num170 - num164) <= 1)
                                            {
                                                Main.tile.At(num169, num170).SetActive (false);
                                            }
                                        }
                                        if (Main.tile.At(num169, num170).Type != 31 && Math.Abs(num169 - num163) <= 2 + WorldGen.genRand.Next(3) && Math.Abs(num170 - num164) <= 2 + WorldGen.genRand.Next(3))
                                        {
                                            Main.tile.At(num169, num170).SetActive (false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                num144++;
            }
            Program.tConsole.WriteLine();
            Program.printData("Generating mountain caves...", true);
            for (int num171 = 0; num171 < WorldGen.numMCaves; num171++)
            {
                int i3 = WorldGen.mCaveX[num171];
                int j3 = WorldGen.mCaveY[num171];
                WorldGen.CaveOpenater(i3, j3);
                WorldGen.Cavinator(i3, j3, WorldGen.genRand.Next(40, 50));
            }
            int num172 = 0;
            int num173 = 0;
            int num174 = 20;
            int num175 = Main.maxTilesX - 20;
            Program.tConsole.WriteLine();
            Program.printData("Creating beaches...", true);
            for (int num176 = 0; num176 < 2; num176++)
            {
                int num177 = 0;
                int num178 = 0;
                if (num176 == 0)
                {
                    num177 = 0;
                    num178 = WorldGen.genRand.Next(125, 200) + 50;
                    if (num9 == 1)
                    {
                        num178 = 275;
                    }
                    int num179 = 0;
                    float num180 = 1f;
                    int num181 = 0;
                    while (!Main.tile.At(num178 - 1, num181).Active)
                    {
                        num181++;
                    }
                    num172 = num181;
                    num181 += WorldGen.genRand.Next(1, 5);
                    for (int num182 = num178 - 1; num182 >= num177; num182--)
                    {
                        num179++;
                        if (num179 < 3)
                        {
                            num180 += (float)WorldGen.genRand.Next(10, 20) * 0.2f;
                        }
                        else
                        {
                            if (num179 < 6)
                            {
                                num180 += (float)WorldGen.genRand.Next(10, 20) * 0.15f;
                            }
                            else
                            {
                                if (num179 < 9)
                                {
                                    num180 += (float)WorldGen.genRand.Next(10, 20) * 0.1f;
                                }
                                else
                                {
                                    if (num179 < 15)
                                    {
                                        num180 += (float)WorldGen.genRand.Next(10, 20) * 0.07f;
                                    }
                                    else
                                    {
                                        if (num179 < 50)
                                        {
                                            num180 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
                                        }
                                        else
                                        {
                                            if (num179 < 75)
                                            {
                                                num180 += (float)WorldGen.genRand.Next(10, 20) * 0.04f;
                                            }
                                            else
                                            {
                                                if (num179 < 100)
                                                {
                                                    num180 += (float)WorldGen.genRand.Next(10, 20) * 0.03f;
                                                }
                                                else
                                                {
                                                    if (num179 < 125)
                                                    {
                                                        num180 += (float)WorldGen.genRand.Next(10, 20) * 0.02f;
                                                    }
                                                    else
                                                    {
                                                        if (num179 < 150)
                                                        {
                                                            num180 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
                                                        }
                                                        else
                                                        {
                                                            if (num179 < 175)
                                                            {
                                                                num180 += (float)WorldGen.genRand.Next(10, 20) * 0.005f;
                                                            }
                                                            else
                                                            {
                                                                if (num179 < 200)
                                                                {
                                                                    num180 += (float)WorldGen.genRand.Next(10, 20) * 0.001f;
                                                                }
                                                                else
                                                                {
                                                                    if (num179 < 230)
                                                                    {
                                                                        num180 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num179 < 235)
                                                                        {
                                                                            num180 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num179 < 240)
                                                                            {
                                                                                num180 += (float)WorldGen.genRand.Next(10, 20) * 0.1f;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num179 < 245)
                                                                                {
                                                                                    num180 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num179 < 255)
                                                                                    {
                                                                                        num180 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
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
                        if (num179 == 235)
                        {
                            num175 = num182;
                        }
                        if (num179 == 235)
                        {
                            num174 = num182;
                        }
                        int num183 = WorldGen.genRand.Next(15, 20);
                        int num184 = 0;
                        while ((float)num184 < (float)num181 + num180 + (float)num183)
                        {
                            if ((float)num184 < (float)num181 + num180 * 0.75f - 3f)
                            {
                                Main.tile.At(num182, num184).SetActive (false);
                                if (num184 > num181)
                                {
                                    Main.tile.At(num182, num184).SetLiquid (255);
                                }
                                else
                                {
                                    if (num184 == num181)
                                    {
                                        Main.tile.At(num182, num184).SetLiquid (127);
                                    }
                                }
                            }
                            else
                            {
                                if (num184 > num181)
                                {
                                    Main.tile.At(num182, num184).SetType (53);
                                    Main.tile.At(num182, num184).SetActive (true);
                                }
                            }
                            Main.tile.At(num182, num184).SetWall (0);
                            num184++;
                        }
                    }
                }
                else
                {
                    num177 = Main.maxTilesX - WorldGen.genRand.Next(125, 200) - 50;
                    num178 = Main.maxTilesX;
                    if (num9 == -1)
                    {
                        num177 = Main.maxTilesX - 275;
                    }
                    float num185 = 1f;
                    int num186 = 0;
                    int num187 = 0;
                    while (!Main.tile.At(num177, num187).Active)
                    {
                        num187++;
                    }
                    num173 = num187;
                    num187 += WorldGen.genRand.Next(1, 5);
                    for (int num188 = num177; num188 < num178; num188++)
                    {
                        num186++;
                        if (num186 < 3)
                        {
                            num185 += (float)WorldGen.genRand.Next(10, 20) * 0.2f;
                        }
                        else
                        {
                            if (num186 < 6)
                            {
                                num185 += (float)WorldGen.genRand.Next(10, 20) * 0.15f;
                            }
                            else
                            {
                                if (num186 < 9)
                                {
                                    num185 += (float)WorldGen.genRand.Next(10, 20) * 0.1f;
                                }
                                else
                                {
                                    if (num186 < 15)
                                    {
                                        num185 += (float)WorldGen.genRand.Next(10, 20) * 0.07f;
                                    }
                                    else
                                    {
                                        if (num186 < 50)
                                        {
                                            num185 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
                                        }
                                        else
                                        {
                                            if (num186 < 75)
                                            {
                                                num185 += (float)WorldGen.genRand.Next(10, 20) * 0.04f;
                                            }
                                            else
                                            {
                                                if (num186 < 100)
                                                {
                                                    num185 += (float)WorldGen.genRand.Next(10, 20) * 0.03f;
                                                }
                                                else
                                                {
                                                    if (num186 < 125)
                                                    {
                                                        num185 += (float)WorldGen.genRand.Next(10, 20) * 0.02f;
                                                    }
                                                    else
                                                    {
                                                        if (num186 < 150)
                                                        {
                                                            num185 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
                                                        }
                                                        else
                                                        {
                                                            if (num186 < 175)
                                                            {
                                                                num185 += (float)WorldGen.genRand.Next(10, 20) * 0.005f;
                                                            }
                                                            else
                                                            {
                                                                if (num186 < 200)
                                                                {
                                                                    num185 += (float)WorldGen.genRand.Next(10, 20) * 0.001f;
                                                                }
                                                                else
                                                                {
                                                                    if (num186 < 230)
                                                                    {
                                                                        num185 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num186 < 235)
                                                                        {
                                                                            num185 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num186 < 240)
                                                                            {
                                                                                num185 += (float)WorldGen.genRand.Next(10, 20) * 0.1f;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (num186 < 245)
                                                                                {
                                                                                    num185 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (num186 < 255)
                                                                                    {
                                                                                        num185 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
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
                        if (num186 == 235)
                        {
                            num175 = num188;
                        }
                        int num189 = WorldGen.genRand.Next(15, 20);
                        int num190 = 0;
                        while ((float)num190 < (float)num187 + num185 + (float)num189)
                        {
                            if ((float)num190 < (float)num187 + num185 * 0.75f - 3f && (double)num190 < Main.worldSurface - 2.0)
                            {
                                Main.tile.At(num188, num190).SetActive (false);
                                if (num190 > num187)
                                {
                                    Main.tile.At(num188, num190).SetLiquid (255);
                                }
                                else
                                {
                                    if (num190 == num187)
                                    {
                                        Main.tile.At(num188, num190).SetLiquid (127);
                                    }
                                }
                            }
                            else
                            {
                                if (num190 > num187)
                                {
                                    Main.tile.At(num188, num190).SetType (53);
                                    Main.tile.At(num188, num190).SetActive (true);
                                }
                            }
                            Main.tile.At(num188, num190).SetWall (0);
                            num190++;
                        }
                    }
                }
            }
            while (!Main.tile.At(num174, num172).Active)
            {
                num172++;
            }
            num172++;
            while (!Main.tile.At(num175, num173).Active)
            {
                num173++;
            }
            num173++;
            Program.tConsole.WriteLine();
            Program.printData("Adding gems...", true);
            for (int num191 = 63; num191 <= 68; num191++)
            {
                float num192 = 0f;
                if (num191 == 67)
                {
                    num192 = (float)Main.maxTilesX * 0.5f;
                }
                else
                {
                    if (num191 == 66)
                    {
                        num192 = (float)Main.maxTilesX * 0.45f;
                    }
                    else
                    {
                        if (num191 == 63)
                        {
                            num192 = (float)Main.maxTilesX * 0.3f;
                        }
                        else
                        {
                            if (num191 == 65)
                            {
                                num192 = (float)Main.maxTilesX * 0.25f;
                            }
                            else
                            {
                                if (num191 == 64)
                                {
                                    num192 = (float)Main.maxTilesX * 0.1f;
                                }
                                else
                                {
                                    if (num191 == 68)
                                    {
                                        num192 = (float)Main.maxTilesX * 0.05f;
                                    }
                                }
                            }
                        }
                    }
                }
                num192 *= 0.2f;
                int num193 = 0;
                while ((float)num193 < num192)
                {
                    int num194 = WorldGen.genRand.Next(0, Main.maxTilesX);
                    int num195 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                    while (Main.tile.At(num194, num195).Type != 1)
                    {
                        num194 = WorldGen.genRand.Next(0, Main.maxTilesX);
                        num195 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                    }
                    WorldGen.TileRunner(num194, num195, (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(3, 7), num191, false, 0f, 0f, false, true);
                    num193++;
                }
            }
            Program.tConsole.WriteLine();
            for (int num196 = 0; num196 < Main.maxTilesX; num196++)
            {
                float num197 = (float)num196 / (float)(Main.maxTilesX - 1);
                Program.printData("Gravitating sand: " + (int)(num197 * 100f) + "%", true);
                for (int num198 = Main.maxTilesY - 5; num198 > 0; num198--)
                {
                    if (Main.tile.At(num196, num198).Active && Main.tile.At(num196, num198).Type == 53)
                    {
                        int num199 = num198;
                        while (!Main.tile.At(num196, num199 + 1).Active && num199 < Main.maxTilesY - 5)
                        {
                            Main.tile.At(num196, num199 + 1).SetActive (true);
                            Main.tile.At(num196, num199 + 1).SetType (53);
                            num199++;
                        }
                    }
                }
            }
            Program.tConsole.WriteLine();
            for (int num200 = 3; num200 < Main.maxTilesX - 3; num200++)
            {
                float num201 = (float)num200 / (float)Main.maxTilesX;
                Program.printData("Cleaning up dirt backgrounds: " + (int)(num201 * 100f + 1f) + "%", true);
                bool flag10 = true;
                int num202 = 0;
                while ((double)num202 < Main.worldSurface)
                {
                    if (flag10)
                    {
                        if (Main.tile.At(num200, num202).Wall == 2)
                        {
                            Main.tile.At(num200, num202).SetWall (0);
                        }
                        if (Main.tile.At(num200, num202).Type != 53)
                        {
                            if (Main.tile.At(num200 - 1, num202).Wall == 2)
                            {
                                Main.tile.At(num200 - 1, num202).SetWall (0);
                            }
                            if (Main.tile.At(num200 - 2, num202).Wall == 2 && WorldGen.genRand.Next(2) == 0)
                            {
                                Main.tile.At(num200 - 2, num202).SetWall (0);
                            }
                            if (Main.tile.At(num200 - 3, num202).Wall == 2 && WorldGen.genRand.Next(2) == 0)
                            {
                                Main.tile.At(num200 - 3, num202).SetWall (0);
                            }
                            if (Main.tile.At(num200 + 1, num202).Wall == 2)
                            {
                                Main.tile.At(num200 + 1, num202).SetWall (0);
                            }
                            if (Main.tile.At(num200 + 2, num202).Wall == 2 && WorldGen.genRand.Next(2) == 0)
                            {
                                Main.tile.At(num200 + 2, num202).SetWall (0);
                            }
                            if (Main.tile.At(num200 + 3, num202).Wall == 2 && WorldGen.genRand.Next(2) == 0)
                            {
                                Main.tile.At(num200 + 3, num202).SetWall (0);
                            }
                            if (Main.tile.At(num200, num202).Active)
                            {
                                flag10 = false;
                            }
                        }
                    }
                    else
                    {
                        if (Main.tile.At(num200, num202).Wall == 0 && Main.tile.At(num200, num202 + 1).Wall == 0 && Main.tile.At(num200, num202 + 2).Wall == 0 && Main.tile.At(num200, num202 + 3).Wall == 0 && Main.tile.At(num200, num202 + 4).Wall == 0 && Main.tile.At(num200 - 1, num202).Wall == 0 && Main.tile.At(num200 + 1, num202).Wall == 0 && Main.tile.At(num200 - 2, num202).Wall == 0 && Main.tile.At(num200 + 2, num202).Wall == 0 && !Main.tile.At(num200, num202).Active && !Main.tile.At(num200, num202 + 1).Active && !Main.tile.At(num200, num202 + 2).Active && !Main.tile.At(num200, num202 + 3).Active)
                        {
                            flag10 = true;
                        }
                    }
                    num202++;
                }
            }
            Program.tConsole.WriteLine();
            for (int num203 = 0; num203 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num203++)
            {
                float num204 = (float)((double)num203 / ((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05));
                Program.printData("Placing altars: " + (int)(num204 * 100f + 1f) + "%", true);
                bool flag11 = false;
                int num205 = 0;
                while (!flag11)
                {
                    int num206 = WorldGen.genRand.Next(1, Main.maxTilesX);
                    int num207 = (int)(num6 + 20.0);
                    WorldGen.Place3x2(num206, num207, 26);
                    if (Main.tile.At(num206, num207).Type == 26)
                    {
                        flag11 = true;
                    }
                    else
                    {
                        num205++;
                        if (num205 >= 10000)
                        {
                            flag11 = true;
                        }
                    }
                }
            }
            for (int num208 = 0; num208 < Main.maxTilesX; num208++)
            {
                int num48 = num208;
                int num209 = (int)num5;
                while ((double)num209 < Main.worldSurface - 1.0)
                {
                    if (Main.tile.At(num48, num209).Active)
                    {
                        if (Main.tile.At(num48, num209).Type == 60)
                        {
                            Main.tile.At(num48, num209 - 1).SetLiquid (255);
                            Main.tile.At(num48, num209 - 2).SetLiquid (255);
                            break;
                        }
                        break;
                    }
                    else
                    {
                        num209++;
                    }
                }
            }
            for (int num210 = 400; num210 < Main.maxTilesX - 400; num210++)
            {
                int num48 = num210;
                int num211 = (int)num5;
                while ((double)num211 < Main.worldSurface - 1.0)
                {
                    if (Main.tile.At(num48, num211).Active)
                    {
                        if (Main.tile.At(num48, num211).Type == 53)
                        {
                            int num212 = num211;
                            while ((double)num212 > num5)
                            {
                                num212--;
                                Main.tile.At(num48, num212).SetLiquid (0);
                            }
                            break;
                        }
                        break;
                    }
                    else
                    {
                        num211++;
                    }
                }
            }
            Program.tConsole.WriteLine();
            Liquid.QuickWater(3, -1, -1);
            WorldGen.WaterCheck();
            int num213 = 0;
            Liquid.quickSettle = true;
            while (num213 < 10)
            {
                int num214 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                num213++;
                float num215 = 0f;
                while (Liquid.numLiquid > 0)
                {
                    float num216 = (float)(num214 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num214;
                    if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num214)
                    {
                        num214 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                    }
                    if (num216 > num215)
                    {
                        num215 = num216;
                    }
                    else
                    {
                        num216 = num215;
                    }
                    if (num213 == 1)
                    {
                        Program.printData("Settling liquids: " + (int)(num216 * 100f / 3f + 33f) + "%", true);
                    }
                    int num217 = 10;
                    if (num213 <= num217)
                    {
                        goto IL_4CC6;
                    }
                IL_4CC6:
                    Liquid.UpdateLiquid();
                }
                WorldGen.WaterCheck();
                Program.printData("Settling liquids: " + (int)((float)num213 * 10f / 3f + 66f) + "%", true);
            }
            Liquid.quickSettle = false;
            Program.tConsole.WriteLine();
            for (int num218 = 0; num218 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.2E-05); num218++)
            {
                float num219 = (float)((double)num218 / ((double)(Main.maxTilesX * Main.maxTilesY) * 2.2E-05));
                Program.printData("Placing life crystals: " + (int)(num219 * 100f + 1f) + "%", true);
                bool flag12 = false;
                int num220 = 0;
                while (!flag12)
                {
                    if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY)))
                    {
                        flag12 = true;
                    }
                    else
                    {
                        num220++;
                        if (num220 >= 10000)
                        {
                            flag12 = true;
                        }
                    }
                }
            }
            Program.tConsole.WriteLine();
            for (int num221 = 0; num221 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 1.7E-05); num221++)
            {
                float num222 = (float)((double)num221 / ((double)(Main.maxTilesX * Main.maxTilesY) * 1.7E-05));
                Program.printData("Hiding treasure: " + (int)(num222 * 100f + 1f) + "%", true);
                bool flag13 = false;
                int num223 = 0;
                while (!flag13)
                {
                    int num224 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                    int num225 = WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY - 20);
                    while (Main.tile.At(num224, num225).Wall > 0)
                    {
                        num224 = WorldGen.genRand.Next(1, Main.maxTilesX);
                        num225 = WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY - 20);
                    }
                    if (WorldGen.AddBuriedChest(num224, num225, 0, false))
                    {
                        flag13 = true;
                    }
                    else
                    {
                        num223++;
                        if (num223 >= 5000)
                        {
                            flag13 = true;
                        }
                    }
                }
            }
            Program.tConsole.WriteLine();
            for (int num226 = 0; num226 < (int)((double)Main.maxTilesX * 0.005); num226++)
            {
                float num227 = (float)((double)num226 / ((double)Main.maxTilesX * 0.005));
                Program.printData("Hiding more treasure: " + (int)(num227 * 100f + 1f) + "%", true);
                bool flag14 = false;
                int num228 = 0;
                while (!flag14)
                {
                    int num229 = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                    int num230 = WorldGen.genRand.Next((int)num5, (int)Main.worldSurface);
                    bool flag15 = false;
                    if (Main.tile.At(num229, num230).Wall == 2)
                    {
                        flag15 = true;
                    }
                    if (flag15 && WorldGen.AddBuriedChest(num229, num230, 0, true))
                    {
                        flag14 = true;
                    }
                    else
                    {
                        num228++;
                        if (num228 >= 3000)
                        {
                            flag14 = true;
                        }
                    }
                }
            }
            int num231 = 0;
            for (int num232 = 0; num232 < WorldGen.numJChests; num232++)
            {
                num231++;
                int contain = 211;
                if (num231 == 1)
                {
                    contain = 211;
                }
                else
                {
                    if (num231 == 2)
                    {
                        contain = 212;
                    }
                    else
                    {
                        if (num231 == 3)
                        {
                            contain = 213;
                        }
                    }
                }
                if (num231 > 3)
                {
                    num231 = 0;
                }
                if (!WorldGen.AddBuriedChest(WorldGen.JChestX[num232] + WorldGen.genRand.Next(2), WorldGen.JChestY[num232], contain, false))
                {
                    for (int num233 = WorldGen.JChestX[num232]; num233 <= WorldGen.JChestX[num232] + 1; num233++)
                    {
                        for (int num234 = WorldGen.JChestY[num232]; num234 <= WorldGen.JChestY[num232] + 1; num234++)
                        {
                            WorldGen.KillTile(num233, num234, false, false, false);
                        }
                    }
                    WorldGen.AddBuriedChest(WorldGen.JChestX[num232], WorldGen.JChestY[num232], contain, false);
                }
            }
            float num235 = (float)(Main.maxTilesX / 4200);
            int num236 = 0;
            int num237 = 0;
            while ((float)num237 < 10f * num235)
            {
                int contain2 = 0;
                num236++;
                if (num236 == 1)
                {
                    contain2 = 186;
                }
                else
                {
                    if (num236 == 2)
                    {
                        contain2 = 277;
                    }
                    else
                    {
                        contain2 = 187;
                        num236 = 0;
                    }
                }
                bool flag16 = false;
                while (!flag16)
                {
                    int num238 = WorldGen.genRand.Next(1, Main.maxTilesX);
                    int num239 = WorldGen.genRand.Next(1, Main.maxTilesY - 200);
                    while (Main.tile.At(num238, num239).Liquid < 200 || Main.tile.At(num238, num239).Lava)
                    {
                        num238 = WorldGen.genRand.Next(1, Main.maxTilesX);
                        num239 = WorldGen.genRand.Next(1, Main.maxTilesY - 200);
                    }
                    flag16 = WorldGen.AddBuriedChest(num238, num239, contain2, true);
                }
                num237++;
            }
            for (int num240 = 0; num240 < WorldGen.numIslandHouses; num240++)
            {
                WorldGen.IslandHouse(WorldGen.fihX[num240], WorldGen.fihY[num240]);
            }
            Program.tConsole.WriteLine();
            for (int num241 = 0; num241 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); num241++)
            {
                float num242 = (float)((double)num241 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008));
                Program.printData("Placing breakables: " + (int)(num242 * 100f + 1f) + "%", true);
                bool flag17 = false;
                int num243 = 0;
                while (!flag17)
                {
                    int num244 = WorldGen.genRand.Next((int)num6, Main.maxTilesY - 10);
                    if ((double)num242 > 0.93)
                    {
                        num244 = Main.maxTilesY - 150;
                    }
                    else
                    {
                        if ((double)num242 > 0.75)
                        {
                            num244 = (int)num5;
                        }
                    }
                    int num245 = WorldGen.genRand.Next(1, Main.maxTilesX);
                    bool flag18 = false;
                    for (int num246 = num244; num246 < Main.maxTilesY; num246++)
                    {
                        if (!flag18)
                        {
                            if (Main.tile.At(num245, num246).Active && Main.tileSolid[(int)Main.tile.At(num245, num246).Type] && !Main.tile.At(num245, num246 - 1).Lava)
                            {
                                flag18 = true;
                            }
                        }
                        else
                        {
                            if (WorldGen.PlacePot(num245, num246, 28))
                            {
                                flag17 = true;
                                break;
                            }
                            num243++;
                            if (num243 >= 10000)
                            {
                                flag17 = true;
                                break;
                            }
                        }
                    }
                }
            }
            Program.tConsole.WriteLine();
            for (int num247 = 0; num247 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0001); num247++)
            {
                float num248 = (float)((double)num247 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0001));
                Program.printData("Placing hellforges: " + (int)(num248 * 100f + 1f) + "%", true);
                bool flag19 = false;
                int num249 = 0;
                while (!flag19)
                {
                    int num250 = WorldGen.genRand.Next(1, Main.maxTilesX);
                    int num251 = WorldGen.genRand.Next(Main.maxTilesY - 250, Main.maxTilesY - 5);
                    try
                    {
                        if (Main.tile.At(num250, num251).Wall == 13)
                        {
                            while (!Main.tile.At(num250, num251).Active)
                            {
                                num251++;
                            }
                            num251--;
                            WorldGen.PlaceTile(num250, num251, 77, false, false, -1, 0);
                            if (Main.tile.At(num250, num251).Type == 77)
                            {
                                flag19 = true;
                            }
                            else
                            {
                                num249++;
                                if (num249 >= 10000)
                                {
                                    flag19 = true;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            Program.tConsole.WriteLine();
            Program.printData("Spreading grass...", true);
            for (int num252 = 0; num252 < Main.maxTilesX; num252++)
            {
                int num48 = num252;
                bool flag20 = true;
                int num253 = 0;
                while ((double)num253 < Main.worldSurface - 1.0)
                {
                    if (Main.tile.At(num48, num253).Active)
                    {
                        if (flag20 && Main.tile.At(num48, num253).Type == 0)
                        {
                            WorldGen.SpreadGrass(num48, num253, 0, 2, true);
                        }
                        if ((double)num253 > num6)
                        {
                            break;
                        }
                        flag20 = false;
                    }
                    else
                    {
                        if (Main.tile.At(num48, num253).Wall == 0)
                        {
                            flag20 = true;
                        }
                    }
                    num253++;
                }
            }
            Program.tConsole.WriteLine();
            Program.printData("Growing cacti...", true);
            for (int num254 = 5; num254 < Main.maxTilesX - 5; num254++)
            {
                if (WorldGen.genRand.Next(8) == 0)
                {
                    int num255 = 0;
                    while ((double)num255 < Main.worldSurface - 1.0)
                    {
                        if (Main.tile.At(num254, num255).Active && Main.tile.At(num254, num255).Type == 53 && !Main.tile.At(num254, num255 - 1).Active && Main.tile.At(num254, num255 - 1).Wall == 0)
                        {
                            if (num254 < 250 || num254 > Main.maxTilesX - 250)
                            {
                                if (Main.tile.At(num254, num255 - 2).Liquid == 255 && Main.tile.At(num254, num255 - 3).Liquid == 255 && Main.tile.At(num254, num255 - 4).Liquid == 255)
                                {
                                    WorldGen.PlaceTile(num254, num255 - 1, 81, true, false, -1, 0);
                                }
                            }
                            else
                            {
                                if (num254 > 400 && num254 < Main.maxTilesX - 400)
                                {
                                    WorldGen.PlantCactus(num254, num255);
                                }
                            }
                        }
                        num255++;
                    }
                }
            }
            int num256 = 5;
            bool flag21 = true;
            while (flag21)
            {
                int num257 = Main.maxTilesX / 2 + WorldGen.genRand.Next(-num256, num256 + 1);
                for (int num258 = 0; num258 < Main.maxTilesY; num258++)
                {
                    if (Main.tile.At(num257, num258).Active)
                    {
                        Main.spawnTileX = num257;
                        Main.spawnTileY = num258;
                        Main.tile.At(num257, num258 - 1).SetLighted (true);
                        break;
                    }
                }
                flag21 = false;
                num256++;
                if ((double)Main.spawnTileY > Main.worldSurface)
                {
                    flag21 = true;
                }
                if (Main.tile.At(Main.spawnTileX, Main.spawnTileY - 1).Liquid > 0)
                {
                    flag21 = true;
                }
            }
            int num259 = 10;
            while ((double)Main.spawnTileY > Main.worldSurface)
            {
                int num260 = WorldGen.genRand.Next(Main.maxTilesX / 2 - num259, Main.maxTilesX / 2 + num259);
                for (int num261 = 0; num261 < Main.maxTilesY; num261++)
                {
                    if (Main.tile.At(num260, num261).Active)
                    {
                        Main.spawnTileX = num260;
                        Main.spawnTileY = num261;
                        Main.tile.At(num260, num261 - 1).SetLighted (true);
                        break;
                    }
                }
                num259++;
            }
            int GuideIndex = NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, 22, 0);
            Main.npcs[GuideIndex].homeTileX = Main.spawnTileX;
            Main.npcs[GuideIndex].homeTileY = Main.spawnTileY;
            Main.npcs[GuideIndex].direction = 1;
            Main.npcs[GuideIndex].homeless = true;
            Program.tConsole.WriteLine();
            Program.printData("Planting sunflowers...", true);
            int num263 = 0;
            while ((double)num263 < (double)Main.maxTilesX * 0.002)
            {
                int num264 = 0;
                int num265 = 0;
                int arg_5A6F_0 = Main.maxTilesX / 2;
                int num266 = WorldGen.genRand.Next(Main.maxTilesX);
                num264 = num266 - WorldGen.genRand.Next(10) - 7;
                num265 = num266 + WorldGen.genRand.Next(10) + 7;
                if (num264 < 0)
                {
                    num264 = 0;
                }
                if (num265 > Main.maxTilesX - 1)
                {
                    num265 = Main.maxTilesX - 1;
                }
                for (int num267 = num264; num267 < num265; num267++)
                {
                    int num268 = 1;
                    while ((double)num268 < Main.worldSurface - 1.0)
                    {
                        if (Main.tile.At(num267, num268).Type == 2 && Main.tile.At(num267, num268).Active && !Main.tile.At(num267, num268 - 1).Active)
                        {
                            WorldGen.PlaceTile(num267, num268 - 1, 27, true, false, -1, 0);
                        }
                        if (Main.tile.At(num267, num268).Active)
                        {
                            break;
                        }
                        num268++;
                    }
                }
                num263++;
            }
            Program.tConsole.WriteLine();
            Program.printData("Planting trees...", true);
            int num269 = 0;
            while ((double)num269 < (double)Main.maxTilesX * 0.003)
            {
                int num270 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int num271 = WorldGen.genRand.Next(25, 50);
                for (int num272 = num270 - num271; num272 < num270 + num271; num272++)
                {
                    int num273 = 20;
                    while ((double)num273 < Main.worldSurface)
                    {
                        WorldGen.GrowEpicTree(num272, num273);
                        num273++;
                    }
                }
                num269++;
            }
            WorldGen.AddTrees();
            Program.tConsole.WriteLine();
            Program.printData("Planting herbs...", true);
            for (int num274 = 0; num274 < Main.maxTilesX * 2; num274++)
            {
                WorldGen.PlantAlch();
            }
            Program.tConsole.WriteLine();
            Program.printData("Planting weeds...", true);
            WorldGen.AddPlants();
            for (int num275 = 0; num275 < Main.maxTilesX; num275++)
            {
                for (int num276 = 0; num276 < Main.maxTilesY; num276++)
                {
                    if (Main.tile.At(num275, num276).Active)
                    {
                        if (num276 >= (int)Main.worldSurface && Main.tile.At(num275, num276).Type == 70 && !Main.tile.At(num275, num276 - 1).Active)
                        {
                            WorldGen.GrowShroom(num275, num276);
                            if (!Main.tile.At(num275, num276 - 1).Active)
                            {
                                WorldGen.PlaceTile(num275, num276 - 1, 71, true, false, -1, 0);
                            }
                        }
                        if (Main.tile.At(num275, num276).Type == 60 && !Main.tile.At(num275, num276 - 1).Active)
                        {
                            WorldGen.PlaceTile(num275, num276 - 1, 61, true, false, -1, 0);
                        }
                    }
                }
            }
            Program.tConsole.WriteLine();
            Program.printData("Growing vines...", true);
            for (int num277 = 0; num277 < Main.maxTilesX; num277++)
            {
                int num278 = 0;
                int num279 = 0;
                while ((double)num279 < Main.worldSurface)
                {
                    if (num278 > 0 && !Main.tile.At(num277, num279).Active)
                    {
                        Main.tile.At(num277, num279).SetActive (true);
                        Main.tile.At(num277, num279).SetType (52);
                        num278--;
                    }
                    else
                    {
                        num278 = 0;
                    }
                    if (Main.tile.At(num277, num279).Active && Main.tile.At(num277, num279).Type == 2 && WorldGen.genRand.Next(5) < 3)
                    {
                        num278 = WorldGen.genRand.Next(1, 10);
                    }
                    num279++;
                }
                num278 = 0;
                for (int num280 = 0; num280 < Main.maxTilesY; num280++)
                {
                    if (num278 > 0 && !Main.tile.At(num277, num280).Active)
                    {
                        Main.tile.At(num277, num280).SetActive (true);
                        Main.tile.At(num277, num280).SetType (62);
                        num278--;
                    }
                    else
                    {
                        num278 = 0;
                    }
                    if (Main.tile.At(num277, num280).Active && Main.tile.At(num277, num280).Type == 60 && WorldGen.genRand.Next(5) < 3)
                    {
                        num278 = WorldGen.genRand.Next(1, 10);
                    }
                }
            }
            Program.tConsole.WriteLine();
            Program.printData("Planting flowers...", true);
            int num281 = 0;
            while ((double)num281 < (double)Main.maxTilesX * 0.005)
            {
                int num282 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                int num283 = WorldGen.genRand.Next(5, 15);
                int num284 = WorldGen.genRand.Next(15, 30);
                int num285 = 1;
                while ((double)num285 < Main.worldSurface - 1.0)
                {
                    if (Main.tile.At(num282, num285).Active)
                    {
                        for (int num286 = num282 - num283; num286 < num282 + num283; num286++)
                        {
                            for (int num287 = num285 - num284; num287 < num285 + num284; num287++)
                            {
                                if (Main.tile.At(num286, num287).Type == 3 || Main.tile.At(num286, num287).Type == 24)
                                {
                                    Main.tile.At(num286, num287).SetFrameX ((short)(WorldGen.genRand.Next(6, 8) * 18));
                                }
                            }
                        }
                        break;
                    }
                    num285++;
                }
                num281++;
            }
            Program.tConsole.WriteLine();
            Program.printData("Planting mushrooms...", true);
            int num288 = 0;
            while ((double)num288 < (double)Main.maxTilesX * 0.002)
            {
                int num289 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                int num290 = WorldGen.genRand.Next(4, 10);
                int num291 = WorldGen.genRand.Next(15, 30);
                int num292 = 1;
                while ((double)num292 < Main.worldSurface - 1.0)
                {
                    if (Main.tile.At(num289, num292).Active)
                    {
                        for (int num293 = num289 - num290; num293 < num289 + num290; num293++)
                        {
                            for (int num294 = num292 - num291; num294 < num292 + num291; num294++)
                            {
                                if (Main.tile.At(num293, num294).Type == 3 || Main.tile.At(num293, num294).Type == 24)
                                {
                                    Main.tile.At(num293, num294).SetFrameX (144);
                                }
                            }
                        }
                        break;
                    }
                    num292++;
                }
                num288++;
            }
            WorldGen.gen = false;
        }

        private static void GrowTreeShared(int i, int y, int freeTilesAbove, int minHeight, int maxHeight)
        {
            bool flag = false;
            bool flag2 = false;
            int num3 = WorldGen.genRand.Next(minHeight, maxHeight);
            int num4;
            for (int j = freeTilesAbove - num3; j < freeTilesAbove; j++)
            {
                Main.tile.At(i, j).SetFrameNumber ((byte)WorldGen.genRand.Next(3));
                Main.tile.At(i, j).SetActive (true);
                Main.tile.At(i, j).SetType (5);
                num4 = WorldGen.genRand.Next(3);
                int num5 = WorldGen.genRand.Next(10);
                if (j == freeTilesAbove - 1 || j == freeTilesAbove - num3)
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
                        Main.tile.At(i, j).SetFrameX (0);
                        Main.tile.At(i, j).SetFrameY (66);
                    }
                    if (num4 == 1)
                    {
                        Main.tile.At(i, j).SetFrameX (0);
                        Main.tile.At(i, j).SetFrameY (88);
                    }
                    if (num4 == 2)
                    {
                        Main.tile.At(i, j).SetFrameX (0);
                        Main.tile.At(i, j).SetFrameY (110);
                    }
                }
                else
                {
                    if (num5 == 2)
                    {
                        if (num4 == 0)
                        {
                            Main.tile.At(i, j).SetFrameX (22);
                            Main.tile.At(i, j).SetFrameY (0);
                        }
                        if (num4 == 1)
                        {
                            Main.tile.At(i, j).SetFrameX (22);
                            Main.tile.At(i, j).SetFrameY (22);
                        }
                        if (num4 == 2)
                        {
                            Main.tile.At(i, j).SetFrameX (22);
                            Main.tile.At(i, j).SetFrameY (44);
                        }
                    }
                    else
                    {
                        if (num5 == 3)
                        {
                            if (num4 == 0)
                            {
                                Main.tile.At(i, j).SetFrameX (44);
                                Main.tile.At(i, j).SetFrameY (66);
                            }
                            if (num4 == 1)
                            {
                                Main.tile.At(i, j).SetFrameX (44);
                                Main.tile.At(i, j).SetFrameY (88);
                            }
                            if (num4 == 2)
                            {
                                Main.tile.At(i, j).SetFrameX (44);
                                Main.tile.At(i, j).SetFrameY (110);
                            }
                        }
                        else
                        {
                            if (num5 == 4)
                            {
                                if (num4 == 0)
                                {
                                    Main.tile.At(i, j).SetFrameX (22);
                                    Main.tile.At(i, j).SetFrameY (66);
                                }
                                if (num4 == 1)
                                {
                                    Main.tile.At(i, j).SetFrameX (22);
                                    Main.tile.At(i, j).SetFrameY (88);
                                }
                                if (num4 == 2)
                                {
                                    Main.tile.At(i, j).SetFrameX (22);
                                    Main.tile.At(i, j).SetFrameY (110);
                                }
                            }
                            else
                            {
                                if (num5 == 5)
                                {
                                    if (num4 == 0)
                                    {
                                        Main.tile.At(i, j).SetFrameX (88);
                                        Main.tile.At(i, j).SetFrameY (0);
                                    }
                                    if (num4 == 1)
                                    {
                                        Main.tile.At(i, j).SetFrameX (88);
                                        Main.tile.At(i, j).SetFrameY (22);
                                    }
                                    if (num4 == 2)
                                    {
                                        Main.tile.At(i, j).SetFrameX (88);
                                        Main.tile.At(i, j).SetFrameY (44);
                                    }
                                }
                                else
                                {
                                    if (num5 == 6)
                                    {
                                        if (num4 == 0)
                                        {
                                            Main.tile.At(i, j).SetFrameX (66);
                                            Main.tile.At(i, j).SetFrameY (66);
                                        }
                                        if (num4 == 1)
                                        {
                                            Main.tile.At(i, j).SetFrameX (66);
                                            Main.tile.At(i, j).SetFrameY (88);
                                        }
                                        if (num4 == 2)
                                        {
                                            Main.tile.At(i, j).SetFrameX (66);
                                            Main.tile.At(i, j).SetFrameY (110);
                                        }
                                    }
                                    else
                                    {
                                        if (num5 == 7)
                                        {
                                            if (num4 == 0)
                                            {
                                                Main.tile.At(i, j).SetFrameX (110);
                                                Main.tile.At(i, j).SetFrameY (66);
                                            }
                                            if (num4 == 1)
                                            {
                                                Main.tile.At(i, j).SetFrameX (110);
                                                Main.tile.At(i, j).SetFrameY (88);
                                            }
                                            if (num4 == 2)
                                            {
                                                Main.tile.At(i, j).SetFrameX (110);
                                                Main.tile.At(i, j).SetFrameY (110);
                                            }
                                        }
                                        else
                                        {
                                            if (num4 == 0)
                                            {
                                                Main.tile.At(i, j).SetFrameX (0);
                                                Main.tile.At(i, j).SetFrameY (0);
                                            }
                                            if (num4 == 1)
                                            {
                                                Main.tile.At(i, j).SetFrameX (0);
                                                Main.tile.At(i, j).SetFrameY (22);
                                            }
                                            if (num4 == 2)
                                            {
                                                Main.tile.At(i, j).SetFrameX (0);
                                                Main.tile.At(i, j).SetFrameY (44);
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
                    Main.tile.At(i - 1, j).SetActive (true);
                    Main.tile.At(i - 1, j).SetType (5);
                    num4 = WorldGen.genRand.Next(3);
                    if (WorldGen.genRand.Next(3) < 2)
                    {
                        if (num4 == 0)
                        {
                            Main.tile.At(i - 1, j).SetFrameX (44);
                            Main.tile.At(i - 1, j).SetFrameY (198);
                        }
                        if (num4 == 1)
                        {
                            Main.tile.At(i - 1, j).SetFrameX (44);
                            Main.tile.At(i - 1, j).SetFrameY (220);
                        }
                        if (num4 == 2)
                        {
                            Main.tile.At(i - 1, j).SetFrameX (44);
                            Main.tile.At(i - 1, j).SetFrameY (242);
                        }
                    }
                    else
                    {
                        if (num4 == 0)
                        {
                            Main.tile.At(i - 1, j).SetFrameX (66);
                            Main.tile.At(i - 1, j).SetFrameY (0);
                        }
                        if (num4 == 1)
                        {
                            Main.tile.At(i - 1, j).SetFrameX (66);
                            Main.tile.At(i - 1, j).SetFrameY (22);
                        }
                        if (num4 == 2)
                        {
                            Main.tile.At(i - 1, j).SetFrameX (66);
                            Main.tile.At(i - 1, j).SetFrameY (44);
                        }
                    }
                }
                if (num5 == 6 || num5 == 7)
                {
                    Main.tile.At(i + 1, j).SetActive (true);
                    Main.tile.At(i + 1, j).SetType (5);
                    num4 = WorldGen.genRand.Next(3);
                    if (WorldGen.genRand.Next(3) < 2)
                    {
                        if (num4 == 0)
                        {
                            Main.tile.At(i + 1, j).SetFrameX (66);
                            Main.tile.At(i + 1, j).SetFrameY (198);
                        }
                        if (num4 == 1)
                        {
                            Main.tile.At(i + 1, j).SetFrameX (66);
                            Main.tile.At(i + 1, j).SetFrameY (220);
                        }
                        if (num4 == 2)
                        {
                            Main.tile.At(i + 1, j).SetFrameX (66);
                            Main.tile.At(i + 1, j).SetFrameY (242);
                        }
                    }
                    else
                    {
                        if (num4 == 0)
                        {
                            Main.tile.At(i + 1, j).SetFrameX (88);
                            Main.tile.At(i + 1, j).SetFrameY (66);
                        }
                        if (num4 == 1)
                        {
                            Main.tile.At(i + 1, j).SetFrameX (88);
                            Main.tile.At(i + 1, j).SetFrameY (88);
                        }
                        if (num4 == 2)
                        {
                            Main.tile.At(i + 1, j).SetFrameX (88);
                            Main.tile.At(i + 1, j).SetFrameY (110);
                        }
                    }
                }
            }
            int num6 = WorldGen.genRand.Next(3);
            if (num6 == 0 || num6 == 1)
            {
                Main.tile.At(i + 1, freeTilesAbove - 1).SetActive (true);
                Main.tile.At(i + 1, freeTilesAbove - 1).SetType (5);
                num4 = WorldGen.genRand.Next(3);
                if (num4 == 0)
                {
                    Main.tile.At(i + 1, freeTilesAbove - 1).SetFrameX (22);
                    Main.tile.At(i + 1, freeTilesAbove - 1).SetFrameY (132);
                }
                if (num4 == 1)
                {
                    Main.tile.At(i + 1, freeTilesAbove - 1).SetFrameX (22);
                    Main.tile.At(i + 1, freeTilesAbove - 1).SetFrameY (154);
                }
                if (num4 == 2)
                {
                    Main.tile.At(i + 1, freeTilesAbove - 1).SetFrameX (22);
                    Main.tile.At(i + 1, freeTilesAbove - 1).SetFrameY (176);
                }
            }
            if (num6 == 0 || num6 == 2)
            {
                Main.tile.At(i - 1, freeTilesAbove - 1).SetActive (true);
                Main.tile.At(i - 1, freeTilesAbove - 1).SetType (5);
                num4 = WorldGen.genRand.Next(3);
                if (num4 == 0)
                {
                    Main.tile.At(i - 1, freeTilesAbove - 1).SetFrameX (44);
                    Main.tile.At(i - 1, freeTilesAbove - 1).SetFrameY (132);
                }
                if (num4 == 1)
                {
                    Main.tile.At(i - 1, freeTilesAbove - 1).SetFrameX (44);
                    Main.tile.At(i - 1, freeTilesAbove - 1).SetFrameY (154);
                }
                if (num4 == 2)
                {
                    Main.tile.At(i - 1, freeTilesAbove - 1).SetFrameX (44);
                    Main.tile.At(i - 1, freeTilesAbove - 1).SetFrameY (176);
                }
            }
            num4 = WorldGen.genRand.Next(3);
            if (num6 == 0)
            {
                if (num4 == 0)
                {
                    Main.tile.At(i, freeTilesAbove - 1).SetFrameX (88);
                    Main.tile.At(i, freeTilesAbove - 1).SetFrameY (132);
                }
                if (num4 == 1)
                {
                    Main.tile.At(i, freeTilesAbove - 1).SetFrameX (88);
                    Main.tile.At(i, freeTilesAbove - 1).SetFrameY (154);
                }
                if (num4 == 2)
                {
                    Main.tile.At(i, freeTilesAbove - 1).SetFrameX (88);
                    Main.tile.At(i, freeTilesAbove - 1).SetFrameY (176);
                }
            }
            else
            {
                if (num6 == 1)
                {
                    if (num4 == 0)
                    {
                        Main.tile.At(i, freeTilesAbove - 1).SetFrameX (0);
                        Main.tile.At(i, freeTilesAbove - 1).SetFrameY (132);
                    }
                    if (num4 == 1)
                    {
                        Main.tile.At(i, freeTilesAbove - 1).SetFrameX (0);
                        Main.tile.At(i, freeTilesAbove - 1).SetFrameY (154);
                    }
                    if (num4 == 2)
                    {
                        Main.tile.At(i, freeTilesAbove - 1).SetFrameX (0);
                        Main.tile.At(i, freeTilesAbove - 1).SetFrameY (176);
                    }
                }
                else
                {
                    if (num6 == 2)
                    {
                        if (num4 == 0)
                        {
                            Main.tile.At(i, freeTilesAbove - 1).SetFrameX (66);
                            Main.tile.At(i, freeTilesAbove - 1).SetFrameY (132);
                        }
                        if (num4 == 1)
                        {
                            Main.tile.At(i, freeTilesAbove - 1).SetFrameX (66);
                            Main.tile.At(i, freeTilesAbove - 1).SetFrameY (154);
                        }
                        if (num4 == 2)
                        {
                            Main.tile.At(i, freeTilesAbove - 1).SetFrameX (66);
                            Main.tile.At(i, freeTilesAbove - 1).SetFrameY (176);
                        }
                    }
                }
            }
            if (WorldGen.genRand.Next(3) < 2)
            {
                num4 = WorldGen.genRand.Next(3);
                if (num4 == 0)
                {
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameX (22);
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameY (198);
                }
                if (num4 == 1)
                {
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameX (22);
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameY (220);
                }
                if (num4 == 2)
                {
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameX (22);
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameY (242);
                }
            }
            else
            {
                num4 = WorldGen.genRand.Next(3);
                if (num4 == 0)
                {
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameX (0);
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameY (198);
                }
                if (num4 == 1)
                {
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameX (0);
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameY (220);
                }
                if (num4 == 2)
                {
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameX (0);
                    Main.tile.At(i, freeTilesAbove - num3).SetFrameY (242);
                }
            }
            WorldGen.RangeFrame(i - 2, freeTilesAbove - num3 - 1, i + 2, freeTilesAbove + 1);
            
            NetMessage.SendTileSquare(-1, i, (int)((double)freeTilesAbove - (double)num3 * 0.5), num3 + 1);
        }

        public static bool GrowEpicTree(int x, int y)
        {
            int freeTilesAbove = y;
            while (Main.tile.At(x, freeTilesAbove).Type == 20)
            {
                freeTilesAbove++;
            }

            TileRef leftTile = Main.tile.At(x - 1, freeTilesAbove);
            TileRef rightTile = Main.tile.At(x + 1, freeTilesAbove);
            if (Main.tile.At(x, freeTilesAbove).Active
                && Main.tile.At(x, freeTilesAbove).Type == 2
                && Main.tile.At(x, freeTilesAbove - 1).Wall == 0
                && Main.tile.At(x, freeTilesAbove - 1).Liquid == 0
                && leftTile.Active
                && leftTile.Type == 2
                && rightTile.Active
                && rightTile.Type == 2)
            {
                if (WorldGen.EmptyTileCheck(x - TREE_RADIUS, x + TREE_RADIUS, freeTilesAbove - 55, freeTilesAbove - 1, 20))
                {
                    GrowTreeShared(x, y, freeTilesAbove, 20, 30);
                    return true;
                }
            }
            return false;
        }

        public static void GrowTree(int x, int y)
        {
            int freeTilesAbove = y;
            while (Main.tile.At(x, freeTilesAbove).Type == 20)
            {
                freeTilesAbove++;
            }

            if ((Main.tile.At(x - 1, freeTilesAbove - 1).Liquid != 0
                || Main.tile.At(x - 1, freeTilesAbove - 1).Liquid != 0
                || Main.tile.At(x + 1, freeTilesAbove - 1).Liquid != 0)
                && Main.tile.At(x, freeTilesAbove).Type != 60)
            {
                return;
            }

            if (IsValidTreeRootTile(Main.tile.At(x, freeTilesAbove))
                && Main.tile.At(x, freeTilesAbove - 1).Wall == 0
                && IsValidTreeRootTile(Main.tile.At(x - 1, freeTilesAbove))
                && IsValidTreeRootTile(Main.tile.At(x + 1, freeTilesAbove)))
            {
                if (WorldGen.EmptyTileCheck(x - TREE_RADIUS, x + TREE_RADIUS, freeTilesAbove - 14, freeTilesAbove - 1, 20))
                {
                    GrowTreeShared(x, y, freeTilesAbove, 5, 15);
                }
            }
        }

        public static Boolean IsValidTreeRootTile(TileRef tile)
        {
            return tile.Active && (tile.Type == 2 || tile.Type == 23 || tile.Type == 60);
        }
        
        public static void GrowShroom(int i, int y)
        {
            if (Main.tile.At(i - 1, y - 1).Lava || Main.tile.At(i - 1, y - 1).Lava || Main.tile.At(i + 1, y - 1).Lava)
            {
                return;
            }
            if (Main.tile.At(i, y).Active && Main.tile.At(i, y).Type == 70 && Main.tile.At(i, y - 1).Wall == 0 && Main.tile.At(i - 1, y).Active && Main.tile.At(i - 1, y).Type == 70 && Main.tile.At(i + 1, y).Active && Main.tile.At(i + 1, y).Type == 70 && WorldGen.EmptyTileCheck(i - 2, i + 2, y - 13, y - 1, 71))
            {
                int num = WorldGen.genRand.Next(4, 11);
                int num2;
                for (int j = y - num; j < y; j++)
                {
                    Main.tile.At(i, j).SetFrameNumber ((byte)WorldGen.genRand.Next(3));
                    Main.tile.At(i, j).SetActive (true);
                    Main.tile.At(i, j).SetType (72);
                    num2 = WorldGen.genRand.Next(3);
                    if (num2 == 0)
                    {
                        Main.tile.At(i, j).SetFrameX (0);
                        Main.tile.At(i, j).SetFrameY (0);
                    }
                    if (num2 == 1)
                    {
                        Main.tile.At(i, j).SetFrameX (0);
                        Main.tile.At(i, j).SetFrameY (18);
                    }
                    if (num2 == 2)
                    {
                        Main.tile.At(i, j).SetFrameX (0);
                        Main.tile.At(i, j).SetFrameY (36);
                    }
                }
                num2 = WorldGen.genRand.Next(3);
                if (num2 == 0)
                {
                    Main.tile.At(i, y - num).SetFrameX (36);
                    Main.tile.At(i, y - num).SetFrameY (0);
                }
                if (num2 == 1)
                {
                    Main.tile.At(i, y - num).SetFrameX (36);
                    Main.tile.At(i, y - num).SetFrameY (18);
                }
                if (num2 == 2)
                {
                    Main.tile.At(i, y - num).SetFrameX (36);
                    Main.tile.At(i, y - num).SetFrameY (36);
                }
                WorldGen.RangeFrame(i - 2, y - num - 1, i + 2, y + 1);
                NetMessage.SendTileSquare(-1, i, (int)((double)y - (double)num * 0.5), num + 1);
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
            if (startX < 0)
            {
                return false;
            }
            if (endX >= Main.maxTilesX)
            {
                return false;
            }
            if (startY < 0)
            {
                return false;
            }
            if (endY >= Main.maxTilesY)
            {
                return false;
            }
            for (int i = startX; i < endX + 1; i++)
            {
                for (int j = startY; j < endY + 1; j++)
                {
                    if (Main.tile.At(i, j).Active)
                    {
                        if (ignoreStyle == -1)
                        {
                            return false;
                        }
                        if (ignoreStyle == 11 && Main.tile.At(i, j).Type != 11)
                        {
                            return false;
                        }
                        if (ignoreStyle == 20 && Main.tile.At(i, j).Type != 20 && Main.tile.At(i, j).Type != 3 && Main.tile.At(i, j).Type != 24 && Main.tile.At(i, j).Type != 61 && Main.tile.At(i, j).Type != 32 && Main.tile.At(i, j).Type != 69 && Main.tile.At(i, j).Type != 73 && Main.tile.At(i, j).Type != 74)
                        {
                            return false;
                        }
                        if (ignoreStyle == 71 && Main.tile.At(i, j).Type != 71)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
        public static bool PlaceDoor(int i, int j, int type)
        {
            bool result;
            try
            {
                if (Main.tile.At(i, j - 2).Active && Main.tileSolid[(int)Main.tile.At(i, j - 2).Type] && Main.tile.At(i, j + 2).Active && Main.tileSolid[(int)Main.tile.At(i, j + 2).Type])
                {
                    Main.tile.At(i, j - 1).SetActive (true);
                    Main.tile.At(i, j - 1).SetType (10);
                    Main.tile.At(i, j - 1).SetFrameY (0);
                    Main.tile.At(i, j - 1).SetFrameX ((short)(WorldGen.genRand.Next(3) * 18));
                    Main.tile.At(i, j).SetActive (true);
                    Main.tile.At(i, j).SetType (10);
                    Main.tile.At(i, j).SetFrameY (18);
                    Main.tile.At(i, j).SetFrameX ((short)(WorldGen.genRand.Next(3) * 18));
                    Main.tile.At(i, j + 1).SetActive (true);
                    Main.tile.At(i, j + 1).SetType (10);
                    Main.tile.At(i, j + 1).SetFrameY (36);
                    Main.tile.At(i, j + 1).SetFrameX ((short)(WorldGen.genRand.Next(3) * 18));
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

        public static bool CloseDoor(int i, int j, bool forced = false, DoorOpener opener = DoorOpener.SERVER, ISender sender = null)
        {
            if (sender == null)
            {
                ConsoleSender cSender = new ConsoleSender(Program.server);
                cSender.ConsoleCommand.Sender = new Sender();
                sender = cSender;
            }

            DoorStateChangeEvent doorEvent = new DoorStateChangeEvent();
            doorEvent.Sender = sender;
            doorEvent.X = i;
            doorEvent.Y = j;
            doorEvent.Direction = 1;
            doorEvent.Opener = opener;
            doorEvent.isOpened = forced;
            Program.server.PluginManager.processHook(Hooks.DOOR_STATECHANGE, doorEvent);
            if (doorEvent.Cancelled)
            {
                return true;
            }

            int num = 0;
            int num2 = i;
            int num3 = j;
            
            int frameX = (int)Main.tile.At(i, j).FrameX;
            int frameY = (int)Main.tile.At(i, j).FrameY;
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
            if (!forced)
            {
                for (int k = num3; k < num3 + 3; k++)
                {
                    if (!Collision.EmptyTile(num2, k, true))
                    {
                        return false;
                    }
                }
            }
            for (int l = num4; l < num4 + 2; l++)
            {
                for (int m = num3; m < num3 + 3; m++)
                {
                    if (l == num2)
                    {
                        Main.tile.At(l, m).SetType (10);
                        Main.tile.At(l, m).SetFrameX ((short)(WorldGen.genRand.Next(3) * 18));
                    }
                    else
                    {
                        Main.tile.At(l, m).SetActive (false);
                    }
                }
            }
            for (int n = num2 - 1; n <= num2 + 1; n++)
            {
                for (int num5 = num3 - 1; num5 <= num3 + 2; num5++)
                {
                    WorldGen.TileFrame(n, num5, false, false);
                }
            }
            return true;
        }
        
        public static bool AddLifeCrystal(int i, int j)
        {
            int k = j;
            while (k < Main.maxTilesY)
            {
                if (Main.tile.At(i, k).Active && Main.tileSolid[(int)Main.tile.At(i, k).Type])
                {
                    int num = k - 1;
                    if (Main.tile.At(i, num - 1).Lava || Main.tile.At(i - 1, num - 1).Lava)
                    {
                        return false;
                    }
                    if (!WorldGen.EmptyTileCheck(i - 1, i, num - 1, num, -1))
                    {
                        return false;
                    }
                    Main.tile.At(i - 1, num - 1).SetActive (true);
                    Main.tile.At(i - 1, num - 1).SetType (12);
                    Main.tile.At(i - 1, num - 1).SetFrameX (0);
                    Main.tile.At(i - 1, num - 1).SetFrameY (0);
                    Main.tile.At(i, num - 1).SetActive (true);
                    Main.tile.At(i, num - 1).SetType (12);
                    Main.tile.At(i, num - 1).SetFrameX (18);
                    Main.tile.At(i, num - 1).SetFrameY (0);
                    Main.tile.At(i - 1, num).SetActive (true);
                    Main.tile.At(i - 1, num).SetType (12);
                    Main.tile.At(i - 1, num).SetFrameX (0);
                    Main.tile.At(i - 1, num).SetFrameY (18);
                    Main.tile.At(i, num).SetActive (true);
                    Main.tile.At(i, num).SetType (12);
                    Main.tile.At(i, num).SetFrameX (18);
                    Main.tile.At(i, num).SetFrameY (18);
                    return true;
                }
                else
                {
                    k++;
                }
            }
            return false;
        }
        
        public static void AddShadowOrb(int x, int y)
        {
            if (x < 10 || x > Main.maxTilesX - 10)
            {
                return;
            }
            if (y < 10 || y > Main.maxTilesY - 10)
            {
                return;
            }
            Main.tile.At(x - 1, y - 1).SetActive (true);
            Main.tile.At(x - 1, y - 1).SetType (31);
            Main.tile.At(x - 1, y - 1).SetFrameX (0);
            Main.tile.At(x - 1, y - 1).SetFrameY (0);
            Main.tile.At(x, y - 1).SetActive (true);
            Main.tile.At(x, y - 1).SetType (31);
            Main.tile.At(x, y - 1).SetFrameX (18);
            Main.tile.At(x, y - 1).SetFrameY (0);
            Main.tile.At(x - 1, y).SetActive (true);
            Main.tile.At(x - 1, y).SetType (31);
            Main.tile.At(x - 1, y).SetFrameX (0);
            Main.tile.At(x - 1, y).SetFrameY (18);
            Main.tile.At(x, y).SetActive (true);
            Main.tile.At(x, y).SetType (31);
            Main.tile.At(x, y).SetFrameX (18);
            Main.tile.At(x, y).SetFrameY (18);
        }
        
        public static void AddHellHouses()
        {
            int num = (int)((double)Main.maxTilesX * 0.25);
            for (int i = num; i < Main.maxTilesX - num; i++)
            {
                int num2 = Main.maxTilesY - 40;
                while (Main.tile.At(i, num2).Active || Main.tile.At(i, num2).Liquid > 0)
                {
                    num2--;
                }
                if (Main.tile.At(i, num2 + 1).Active)
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
            for (int l = 0; l < num3; l++)
            {
                int num6 = WorldGen.genRand.Next(5, 9);
                num4 += num6;
                WorldGen.HellRoom(i, num4, num, num6);
            }
            for (int m = i - num / 2; m <= i + num / 2; m++)
            {
                num4 = j;
                while (num4 < Main.maxTilesY && ((Main.tile.At(m, num4).Active && Main.tile.At(m, num4).Type == 76) || Main.tile.At(m, num4).Wall == 13))
                {
                    num4++;
                }
                int num7 = 6 + WorldGen.genRand.Next(3);
                while (num4 < Main.maxTilesY && !Main.tile.At(m, num4).Active)
                {
                    num7--;
                    Main.tile.At(m, num4).SetActive (true);
                    Main.tile.At(m, num4).SetType (57);
                    num4++;
                    if (num7 <= 0)
                    {
                        break;
                    }
                }
            }
            int num8 = 0;
            int num9 = 0;
            num4 = j;
            while (num4 < Main.maxTilesY && ((Main.tile.At(i, num4).Active && Main.tile.At(i, num4).Type == 76) || Main.tile.At(i, num4).Wall == 13))
            {
                num4++;
            }
            num4--;
            num9 = num4;
            while ((Main.tile.At(i, num4).Active && Main.tile.At(i, num4).Type == 76) || Main.tile.At(i, num4).Wall == 13)
            {
                num4--;
                if (Main.tile.At(i, num4).Active && Main.tile.At(i, num4).Type == 76)
                {
                    int num10 = WorldGen.genRand.Next(i - num / 2 + 1, i + num / 2 - 1);
                    int num11 = WorldGen.genRand.Next(i - num / 2 + 1, i + num / 2 - 1);
                    if (num10 > num11)
                    {
                        int num12 = num10;
                        num10 = num11;
                        num11 = num12;
                    }
                    if (num10 == num11)
                    {
                        if (num10 < i)
                        {
                            num11++;
                        }
                        else
                        {
                            num10--;
                        }
                    }
                    for (int n = num10; n <= num11; n++)
                    {
                        if (Main.tile.At(n, num4 - 1).Wall == 13)
                        {
                            Main.tile.At(n, num4).SetWall (13);
                        }
                        Main.tile.At(n, num4).SetType (19);
                        Main.tile.At(n, num4).SetActive (true);
                    }
                    num4--;
                }
            }
            num8 = num4;
            float num13 = (float)((num9 - num8) * num);
            float num14 = num13 * 0.02f;
            int num15 = 0;
            while ((float)num15 < num14)
            {
                int num16 = WorldGen.genRand.Next(i - num / 2, i + num / 2 + 1);
                int num17 = WorldGen.genRand.Next(num8, num9);
                int num18 = WorldGen.genRand.Next(3, 8);
                for (int num19 = num16 - num18; num19 <= num16 + num18; num19++)
                {
                    for (int num20 = num17 - num18; num20 <= num17 + num18; num20++)
                    {
                        float num21 = (float)Math.Abs(num19 - num16);
                        float num22 = (float)Math.Abs(num20 - num17);
                        double num23 = Math.Sqrt((double)(num21 * num21 + num22 * num22));
                        if (num23 < (double)num18 * 0.4)
                        {
                            try
                            {
                                if (Main.tile.At(num19, num20).Type == 76 || Main.tile.At(num19, num20).Type == 19)
                                {
                                    Main.tile.At(num19, num20).SetActive (false);
                                }
                                Main.tile.At(num19, num20).SetWall (0);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                num15++;
            }
        }
        
        public static void HellRoom(int i, int j, int width, int height)
        {
            for (int k = i - width / 2; k <= i + width / 2; k++)
            {
                for (int l = j - height; l <= j; l++)
                {
                    try
                    {
                        Main.tile.At(k, l).SetActive (true);
                        Main.tile.At(k, l).SetType (76);
                        Main.tile.At(k, l).SetLiquid (0);
                        Main.tile.At(k, l).SetLava (false);
                    }
                    catch
                    {
                    }
                }
            }
            for (int m = i - width / 2 + 1; m <= i + width / 2 - 1; m++)
            {
                for (int n = j - height + 1; n <= j - 1; n++)
                {
                    try
                    {
                        Main.tile.At(m, n).SetActive (false);
                        Main.tile.At(m, n).SetWall (13);
                        Main.tile.At(m, n).SetLiquid (0);
                        Main.tile.At(m, n).SetLava (false);
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
                Program.printData("Creating dungeon: " + (int)((num4 - num3) / num4 * 60f) + "%", true);
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
                    int num10 = WorldGen.dungeonX;
                    int num11 = WorldGen.dungeonY;
                    WorldGen.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, true);
                    WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
                    WorldGen.dungeonX = num10;
                    WorldGen.dungeonY = num11;
                }
                WorldGen.DungeonStairs(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            }
            WorldGen.DungeonEnt(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            Program.printData("Creating dungeon: 65%", true);
            for (int j = 0; j < WorldGen.numDRooms; j++)
            {
                for (int k = WorldGen.dRoomL[j]; k <= WorldGen.dRoomR[j]; k++)
                {
                    if (!Main.tile.At(k, WorldGen.dRoomT[j] - 1).Active)
                    {
                        WorldGen.DPlatX[WorldGen.numDPlats] = k;
                        WorldGen.DPlatY[WorldGen.numDPlats] = WorldGen.dRoomT[j] - 1;
                        WorldGen.numDPlats++;
                        break;
                    }
                }
                for (int l = WorldGen.dRoomL[j]; l <= WorldGen.dRoomR[j]; l++)
                {
                    if (!Main.tile.At(l, WorldGen.dRoomB[j] + 1).Active)
                    {
                        WorldGen.DPlatX[WorldGen.numDPlats] = l;
                        WorldGen.DPlatY[WorldGen.numDPlats] = WorldGen.dRoomB[j] + 1;
                        WorldGen.numDPlats++;
                        break;
                    }
                }
                for (int m = WorldGen.dRoomT[j]; m <= WorldGen.dRoomB[j]; m++)
                {
                    if (!Main.tile.At(WorldGen.dRoomL[j] - 1, m).Active)
                    {
                        WorldGen.DDoorX[WorldGen.numDDoors] = WorldGen.dRoomL[j] - 1;
                        WorldGen.DDoorY[WorldGen.numDDoors] = m;
                        WorldGen.DDoorPos[WorldGen.numDDoors] = -1;
                        WorldGen.numDDoors++;
                        break;
                    }
                }
                for (int n = WorldGen.dRoomT[j]; n <= WorldGen.dRoomB[j]; n++)
                {
                    if (!Main.tile.At(WorldGen.dRoomR[j] + 1, n).Active)
                    {
                        WorldGen.DDoorX[WorldGen.numDDoors] = WorldGen.dRoomR[j] + 1;
                        WorldGen.DDoorY[WorldGen.numDDoors] = n;
                        WorldGen.DDoorPos[WorldGen.numDDoors] = 1;
                        WorldGen.numDDoors++;
                        break;
                    }
                }
            }
            Program.printData("Creating dungeon: 70%", true);
            int num12 = 0;
            int num13 = 1000;
            int num14 = 0;
            while (num14 < Main.maxTilesX / 100)
            {
                num12++;
                int num15 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
                int num16 = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);
                int num17 = num15;
                if ((int)Main.tile.At(num15, num16).Wall == wallType && !Main.tile.At(num15, num16).Active)
                {
                    int num18 = 1;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num18 = -1;
                    }
                    while (!Main.tile.At(num15, num16).Active)
                    {
                        num16 += num18;
                    }
                    if (Main.tile.At(num15 - 1, num16).Active && Main.tile.At(num15 + 1, num16).Active && !Main.tile.At(num15 - 1, num16 - num18).Active && !Main.tile.At(num15 + 1, num16 - num18).Active)
                    {
                        num14++;
                        int num19 = WorldGen.genRand.Next(5, 13);
                        while (Main.tile.At(num15 - 1, num16).Active && Main.tile.At(num15, num16 + num18).Active && Main.tile.At(num15, num16).Active && !Main.tile.At(num15, num16 - num18).Active && num19 > 0)
                        {
                            Main.tile.At(num15, num16).SetType (48);
                            if (!Main.tile.At(num15 - 1, num16 - num18).Active && !Main.tile.At(num15 + 1, num16 - num18).Active)
                            {
                                Main.tile.At(num15, num16 - num18).SetType (48);
                                Main.tile.At(num15, num16 - num18).SetActive (true);
                            }
                            num15--;
                            num19--;
                        }
                        num19 = WorldGen.genRand.Next(5, 13);
                        num15 = num17 + 1;
                        while (Main.tile.At(num15 + 1, num16).Active && Main.tile.At(num15, num16 + num18).Active && Main.tile.At(num15, num16).Active && !Main.tile.At(num15, num16 - num18).Active && num19 > 0)
                        {
                            Main.tile.At(num15, num16).SetType (48);
                            if (!Main.tile.At(num15 - 1, num16 - num18).Active && !Main.tile.At(num15 + 1, num16 - num18).Active)
                            {
                                Main.tile.At(num15, num16 - num18).SetType (48);
                                Main.tile.At(num15, num16 - num18).SetActive (true);
                            }
                            num15++;
                            num19--;
                        }
                    }
                }
                if (num12 > num13)
                {
                    num12 = 0;
                    num14++;
                }
            }
            num12 = 0;
            num13 = 1000;
            num14 = 0;
            Program.printData("Creating dungeon: 75%", true);
            while (num14 < Main.maxTilesX / 100)
            {
                num12++;
                int num20 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
                int num21 = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);
                int num22 = num21;
                if ((int)Main.tile.At(num20, num21).Wall == wallType && !Main.tile.At(num20, num21).Active)
                {
                    int num23 = 1;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num23 = -1;
                    }
                    while (num20 > 5 && num20 < Main.maxTilesX - 5 && !Main.tile.At(num20, num21).Active)
                    {
                        num20 += num23;
                    }
                    if (Main.tile.At(num20, num21 - 1).Active && Main.tile.At(num20, num21 + 1).Active && !Main.tile.At(num20 - num23, num21 - 1).Active && !Main.tile.At(num20 - num23, num21 + 1).Active)
                    {
                        num14++;
                        int num24 = WorldGen.genRand.Next(5, 13);
                        while (Main.tile.At(num20, num21 - 1).Active && Main.tile.At(num20 + num23, num21).Active && Main.tile.At(num20, num21).Active && !Main.tile.At(num20 - num23, num21).Active && num24 > 0)
                        {
                            Main.tile.At(num20, num21).SetType (48);
                            if (!Main.tile.At(num20 - num23, num21 - 1).Active && !Main.tile.At(num20 - num23, num21 + 1).Active)
                            {
                                Main.tile.At(num20 - num23, num21).SetType (48);
                                Main.tile.At(num20 - num23, num21).SetActive (true);
                            }
                            num21--;
                            num24--;
                        }
                        num24 = WorldGen.genRand.Next(5, 13);
                        num21 = num22 + 1;
                        while (Main.tile.At(num20, num21 + 1).Active && Main.tile.At(num20 + num23, num21).Active && Main.tile.At(num20, num21).Active && !Main.tile.At(num20 - num23, num21).Active && num24 > 0)
                        {
                            Main.tile.At(num20, num21).SetType (48);
                            if (!Main.tile.At(num20 - num23, num21 - 1).Active && !Main.tile.At(num20 - num23, num21 + 1).Active)
                            {
                                Main.tile.At(num20 - num23, num21).SetType (48);
                                Main.tile.At(num20 - num23, num21).SetActive (true);
                            }
                            num21++;
                            num24--;
                        }
                    }
                }
                if (num12 > num13)
                {
                    num12 = 0;
                    num14++;
                }
            }
            Program.printData("Creating dungeon: 80%", true);
            for (int num25 = 0; num25 < WorldGen.numDDoors; num25++)
            {
                int num26 = WorldGen.DDoorX[num25] - 10;
                int num27 = WorldGen.DDoorX[num25] + 10;
                int num28 = 100;
                int num29 = 0;
                for (int num30 = num26; num30 < num27; num30++)
                {
                    bool flag = true;
                    int num31 = WorldGen.DDoorY[num25];
                    while (!Main.tile.At(num30, num31).Active)
                    {
                        num31--;
                    }
                    if (!Main.tileDungeon[(int)Main.tile.At(num30, num31).Type])
                    {
                        flag = false;
                    }
                    int num32 = num31;
                    num31 = WorldGen.DDoorY[num25];
                    while (!Main.tile.At(num30, num31).Active)
                    {
                        num31++;
                    }
                    if (!Main.tileDungeon[(int)Main.tile.At(num30, num31).Type])
                    {
                        flag = false;
                    }
                    int num33 = num31;
                    if (num33 - num32 >= 3)
                    {
                        int num34 = num30 - 20;
                        int num35 = num30 + 20;
                        int num36 = num33 - 10;
                        int num37 = num33 + 10;
                        for (int num38 = num34; num38 < num35; num38++)
                        {
                            for (int num39 = num36; num39 < num37; num39++)
                            {
                                if (Main.tile.At(num38, num39).Active && Main.tile.At(num38, num39).Type == 10)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (flag)
                        {
                            for (int num40 = num33 - 3; num40 < num33; num40++)
                            {
                                for (int num41 = num30 - 3; num41 <= num30 + 3; num41++)
                                {
                                    if (Main.tile.At(num41, num40).Active)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (flag && num33 - num32 < 20)
                        {
                            bool flag2 = false;
                            if (WorldGen.DDoorPos[num25] == 0 && num33 - num32 < num28)
                            {
                                flag2 = true;
                            }
                            if (WorldGen.DDoorPos[num25] == -1 && num30 > num29)
                            {
                                flag2 = true;
                            }
                            if (WorldGen.DDoorPos[num25] == 1 && (num30 < num29 || num29 == 0))
                            {
                                flag2 = true;
                            }
                            if (flag2)
                            {
                                num29 = num30;
                                num28 = num33 - num32;
                            }
                        }
                    }
                }
                if (num28 < 20)
                {
                    int num42 = num29;
                    int num43 = WorldGen.DDoorY[num25];
                    int num44 = num43;
                    while (!Main.tile.At(num42, num43).Active)
                    {
                        Main.tile.At(num42, num43).SetActive (false);
                        num43++;
                    }
                    while (!Main.tile.At(num42, num44).Active)
                    {
                        num44--;
                    }
                    num43--;
                    num44++;
                    for (int num45 = num44; num45 < num43 - 2; num45++)
                    {
                        Main.tile.At(num42, num45).SetActive (true);
                        Main.tile.At(num42, num45).SetType ((byte)tileType);
                    }
                    WorldGen.PlaceTile(num42, num43, 10, true, false, -1, 0);
                    num42--;
                    int num46 = num43 - 3;
                    while (!Main.tile.At(num42, num46).Active)
                    {
                        num46--;
                    }
                    if (num43 - num46 < num43 - num44 + 5 && Main.tileDungeon[(int)Main.tile.At(num42, num46).Type])
                    {
                        for (int num47 = num43 - 4 - WorldGen.genRand.Next(3); num47 > num46; num47--)
                        {
                            Main.tile.At(num42, num47).SetActive (true);
                            Main.tile.At(num42, num47).SetType ((byte)tileType);
                        }
                    }
                    num42 += 2;
                    num46 = num43 - 3;
                    while (!Main.tile.At(num42, num46).Active)
                    {
                        num46--;
                    }
                    if (num43 - num46 < num43 - num44 + 5 && Main.tileDungeon[(int)Main.tile.At(num42, num46).Type])
                    {
                        for (int num48 = num43 - 4 - WorldGen.genRand.Next(3); num48 > num46; num48--)
                        {
                            Main.tile.At(num42, num48).SetActive (true);
                            Main.tile.At(num42, num48).SetType ((byte)tileType);
                        }
                    }
                    num43++;
                    num42--;
                    Main.tile.At(num42 - 1, num43).SetActive (true);
                    Main.tile.At(num42 - 1, num43).SetType ((byte)tileType);
                    Main.tile.At(num42 + 1, num43).SetActive (true);
                    Main.tile.At(num42 + 1, num43).SetType ((byte)tileType);
                }
            }
            Program.printData("Creating dungeon: 85%", true);
            for (int num49 = 0; num49 < WorldGen.numDPlats; num49++)
            {
                int num50 = WorldGen.DPlatX[num49];
                int num51 = WorldGen.DPlatY[num49];
                int num52 = Main.maxTilesX;
                int num53 = 10;
                for (int num54 = num51 - 5; num54 <= num51 + 5; num54++)
                {
                    int num55 = num50;
                    int num56 = num50;
                    bool flag3 = false;
                    if (Main.tile.At(num55, num54).Active)
                    {
                        flag3 = true;
                    }
                    else
                    {
                        while (!Main.tile.At(num55, num54).Active)
                        {
                            num55--;
                            if (!Main.tileDungeon[(int)Main.tile.At(num55, num54).Type])
                            {
                                flag3 = true;
                            }
                        }
                        while (!Main.tile.At(num56, num54).Active)
                        {
                            num56++;
                            if (!Main.tileDungeon[(int)Main.tile.At(num56, num54).Type])
                            {
                                flag3 = true;
                            }
                        }
                    }
                    if (!flag3 && num56 - num55 <= num53)
                    {
                        bool flag4 = true;
                        int num57 = num50 - num53 / 2 - 2;
                        int num58 = num50 + num53 / 2 + 2;
                        int num59 = num54 - 5;
                        int num60 = num54 + 5;
                        for (int num61 = num57; num61 <= num58; num61++)
                        {
                            for (int num62 = num59; num62 <= num60; num62++)
                            {
                                if (Main.tile.At(num61, num62).Active && Main.tile.At(num61, num62).Type == 19)
                                {
                                    flag4 = false;
                                    break;
                                }
                            }
                        }
                        for (int num63 = num54 + 3; num63 >= num54 - 5; num63--)
                        {
                            if (Main.tile.At(num50, num63).Active)
                            {
                                flag4 = false;
                                break;
                            }
                        }
                        if (flag4)
                        {
                            num52 = num54;
                            break;
                        }
                    }
                }
                if (num52 > num51 - 10 && num52 < num51 + 10)
                {
                    int num64 = num50;
                    int num65 = num52;
                    int num66 = num50 + 1;
                    while (!Main.tile.At(num64, num65).Active)
                    {
                        Main.tile.At(num64, num65).SetActive (true);
                        Main.tile.At(num64, num65).SetType (19);
                        num64--;
                    }
                    while (!Main.tile.At(num66, num65).Active)
                    {
                        Main.tile.At(num66, num65).SetActive (true);
                        Main.tile.At(num66, num65).SetType (19);
                        num66++;
                    }
                }
            }
            Program.printData("Creating dungeon: 90%", true);
            num12 = 0;
            num13 = 1000;
            num14 = 0;
            while (num14 < Main.maxTilesX / 20)
            {
                num12++;
                int num67 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
                int num68 = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
                bool flag5 = true;
                if ((int)Main.tile.At(num67, num68).Wall == wallType && !Main.tile.At(num67, num68).Active)
                {
                    int num69 = 1;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num69 = -1;
                    }
                    while (flag5 && !Main.tile.At(num67, num68).Active)
                    {
                        num67 -= num69;
                        if (num67 < 5 || num67 > Main.maxTilesX - 5)
                        {
                            flag5 = false;
                        }
                        else
                        {
                            if (Main.tile.At(num67, num68).Active && !Main.tileDungeon[(int)Main.tile.At(num67, num68).Type])
                            {
                                flag5 = false;
                            }
                        }
                    }
                    if (flag5 && Main.tile.At(num67, num68).Active && Main.tileDungeon[(int)Main.tile.At(num67, num68).Type] && Main.tile.At(num67, num68 - 1).Active && Main.tileDungeon[(int)Main.tile.At(num67, num68 - 1).Type] && Main.tile.At(num67, num68 + 1).Active && Main.tileDungeon[(int)Main.tile.At(num67, num68 + 1).Type])
                    {
                        num67 += num69;
                        for (int num70 = num67 - 3; num70 <= num67 + 3; num70++)
                        {
                            for (int num71 = num68 - 3; num71 <= num68 + 3; num71++)
                            {
                                if (Main.tile.At(num70, num71).Active && Main.tile.At(num70, num71).Type == 19)
                                {
                                    flag5 = false;
                                    break;
                                }
                            }
                        }
                        if (flag5 && (!Main.tile.At(num67, num68 - 1).Active & !Main.tile.At(num67, num68 - 2).Active & !Main.tile.At(num67, num68 - 3).Active))
                        {
                            int num72 = num67;
                            int num73 = num67;
                            while (num72 > WorldGen.dMinX && num72 < WorldGen.dMaxX && !Main.tile.At(num72, num68).Active && !Main.tile.At(num72, num68 - 1).Active && !Main.tile.At(num72, num68 + 1).Active)
                            {
                                num72 += num69;
                            }
                            num72 = Math.Abs(num67 - num72);
                            bool flag6 = false;
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                flag6 = true;
                            }
                            if (num72 > 5)
                            {
                                for (int num74 = WorldGen.genRand.Next(1, 4); num74 > 0; num74--)
                                {
                                    Main.tile.At(num67, num68).SetActive (true);
                                    Main.tile.At(num67, num68).SetType (19);
                                    if (flag6)
                                    {
                                        WorldGen.PlaceTile(num67, num68 - 1, 50, true, false, -1, 0);
                                        if (WorldGen.genRand.Next(50) == 0 && Main.tile.At(num67, num68 - 1).Type == 50)
                                        {
                                            Main.tile.At(num67, num68 - 1).SetFrameX (90);
                                        }
                                    }
                                    num67 += num69;
                                }
                                num12 = 0;
                                num14++;
                                if (!flag6 && WorldGen.genRand.Next(2) == 0)
                                {
                                    num67 = num73;
                                    num68--;
                                    int num75 = WorldGen.genRand.Next(2);
                                    if (num75 == 0)
                                    {
                                        num75 = 13;
                                    }
                                    else
                                    {
                                        if (num75 == 1)
                                        {
                                            num75 = 49;
                                        }
                                    }
                                    WorldGen.PlaceTile(num67, num68, num75, true, false, -1, 0);
                                    if (Main.tile.At(num67, num68).Type == 13)
                                    {
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            Main.tile.At(num67, num68).SetFrameX (18);
                                        }
                                        else
                                        {
                                            Main.tile.At(num67, num68).SetFrameX (36);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (num12 > num13)
                {
                    num12 = 0;
                    num14++;
                }
            }
            Program.printData("Creating dungeon: 95%", true);
            for (int num76 = 0; num76 < WorldGen.numDRooms; num76++)
            {
                int num77 = 0;
                while (num77 < 1000)
                {
                    int num78 = (int)((double)WorldGen.dRoomSize[num76] * 0.4);
                    int i2 = WorldGen.dRoomX[num76] + WorldGen.genRand.Next(-num78, num78 + 1);
                    int j2 = WorldGen.dRoomY[num76] + WorldGen.genRand.Next(-num78, num78 + 1);
                    int num79 = 0;
                    int num80 = num76;
                    if (num80 == 0)
                    {
                        num79 = 113;
                    }
                    else
                    {
                        if (num80 == 1)
                        {
                            num79 = 155;
                        }
                        else
                        {
                            if (num80 == 2)
                            {
                                num79 = 156;
                            }
                            else
                            {
                                if (num80 == 3)
                                {
                                    num79 = 157;
                                }
                                else
                                {
                                    if (num80 == 4)
                                    {
                                        num79 = 163;
                                    }
                                    else
                                    {
                                        if (num80 == 5)
                                        {
                                            num79 = 164;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (num79 == 0 && WorldGen.genRand.Next(2) == 0)
                    {
                        num77 = 1000;
                    }
                    else
                    {
                        if (WorldGen.AddBuriedChest(i2, j2, num79, false))
                        {
                            num77 += 1000;
                        }
                        num77++;
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
            num12 = 0;
            num13 = 1000;
            num14 = 0;
            while (num14 < Main.maxTilesX / 40)
            {
                num12++;
                int num81 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
                int num82 = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
                if ((int)Main.tile.At(num81, num82).Wall == wallType)
                {
                    int num83 = num82;
                    while (num83 > WorldGen.dMinY)
                    {
                        if (Main.tile.At(num81, num83 - 1).Active && (int)Main.tile.At(num81, num83 - 1).Type == tileType)
                        {
                            bool flag7 = false;
                            for (int num84 = num81 - 15; num84 < num81 + 15; num84++)
                            {
                                for (int num85 = num83 - 15; num85 < num83 + 15; num85++)
                                {
                                    if (num84 > 0 && num84 < Main.maxTilesX && num85 > 0 && num85 < Main.maxTilesY && Main.tile.At(num84, num85).Type == 42)
                                    {
                                        flag7 = true;
                                        break;
                                    }
                                }
                            }
                            if (Main.tile.At(num81 - 1, num83).Active || Main.tile.At(num81 + 1, num83).Active || Main.tile.At(num81 - 1, num83 + 1).Active || Main.tile.At(num81 + 1, num83 + 1).Active || Main.tile.At(num81, num83 + 2).Active)
                            {
                                flag7 = true;
                            }
                            if (flag7)
                            {
                                break;
                            }
                            WorldGen.Place1x2Top(num81, num83, 42);
                            if (Main.tile.At(num81, num83).Type == 42)
                            {
                                num12 = 0;
                                num14++;
                                break;
                            }
                            break;
                        }
                        else
                        {
                            num83--;
                        }
                    }
                }
                if (num12 > num13)
                {
                    num14++;
                    num12 = 0;
                }
            }
            Console.WriteLine();
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
                if ((double)value2.Y < Main.worldSurface - 5.0 && Main.tile.At(num8, (int)((double)value2.Y - num - 6.0 + (double)num9)).Wall == 0 && Main.tile.At(num8, (int)((double)value2.Y - num - 7.0 + (double)num9)).Wall == 0 && Main.tile.At(num8, (int)((double)value2.Y - num - 8.0 + (double)num9)).Wall == 0)
                {
                    WorldGen.dSurface = true;
                    WorldGen.TileRunner(num8, (int)((double)value2.Y - num - 6.0 + (double)num9), (double)WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(10, 20), -1, false, 0f, -1f, false, true);
                }
                for (int l = num3; l < num4; l++)
                {
                    for (int m = num5; m < num6; m++)
                    {
                        Main.tile.At(l, m).SetLiquid (0);
                        if ((int)Main.tile.At(l, m).Wall != wallType)
                        {
                            Main.tile.At(l, m).SetWall (0);
                            Main.tile.At(l, m).SetActive (true);
                            Main.tile.At(l, m).SetType ((byte)tileType);
                        }
                    }
                }
                for (int n = num3 + 1; n < num4 - 1; n++)
                {
                    for (int num10 = num5 + 1; num10 < num6 - 1; num10++)
                    {
                        WorldGen.PlaceWall(n, num10, wallType, true);
                    }
                }
                int num11 = 0;
                if (WorldGen.genRand.Next((int)num) == 0)
                {
                    num11 = WorldGen.genRand.Next(1, 3);
                }
                num3 = (int)((double)value2.X - num * 0.5 - (double)num11);
                num4 = (int)((double)value2.X + num * 0.5 + (double)num11);
                num5 = (int)((double)value2.Y - num * 0.5 - (double)num11);
                num6 = (int)((double)value2.Y + num * 0.5 + (double)num11);
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
                for (int num12 = num3; num12 < num4; num12++)
                {
                    for (int num13 = num5; num13 < num6; num13++)
                    {
                        Main.tile.At(num12, num13).SetActive (false);
                        WorldGen.PlaceWall(num12, num13, wallType, true);
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
            Vector2 value = default(Vector2);
            double num = (double)WorldGen.genRand.Next(4, 6);
            Vector2 vector = default(Vector2);
            Vector2 value2 = default(Vector2);
            int num2 = 1;
            Vector2 value3;
            value3.X = (float)i;
            value3.Y = (float)j;
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
                    value2.Y = 0f;
                    value2.X = (float)(-(float)num2);
                    value.Y = 0f;
                    value.X = (float)num2;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            value.Y = -0.2f;
                        }
                        else
                        {
                            value.Y = 0.2f;
                        }
                    }
                }
                else
                {
                    num += 1.0;
                    value.Y = (float)num2;
                    value.X = 0f;
                    vector.X = 0f;
                    vector.Y = (float)num2;
                    value2.X = 0f;
                    value2.Y = (float)(-(float)num2);
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            value.X = 0.3f;
                        }
                        else
                        {
                            value.X = -0.3f;
                        }
                    }
                    else
                    {
                        k /= 2;
                    }
                }
                if (WorldGen.lastDungeonHall != value2)
                {
                    flag = true;
                }
            }
            if (!forceX)
            {
                if (value3.X > (float)(WorldGen.lastMaxTilesX - 200))
                {
                    num2 = -1;
                    vector.Y = 0f;
                    vector.X = (float)num2;
                    value.Y = 0f;
                    value.X = (float)num2;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            value.Y = -0.2f;
                        }
                        else
                        {
                            value.Y = 0.2f;
                        }
                    }
                }
                else
                {
                    if (value3.X < 200f)
                    {
                        num2 = 1;
                        vector.Y = 0f;
                        vector.X = (float)num2;
                        value.Y = 0f;
                        value.X = (float)num2;
                        if (WorldGen.genRand.Next(3) == 0)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                value.Y = -0.2f;
                            }
                            else
                            {
                                value.Y = 0.2f;
                            }
                        }
                    }
                    else
                    {
                        if (value3.Y > (float)(WorldGen.lastMaxTilesY - 300))
                        {
                            num2 = -1;
                            num += 1.0;
                            value.Y = (float)num2;
                            value.X = 0f;
                            vector.X = 0f;
                            vector.Y = (float)num2;
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    value.X = 0.3f;
                                }
                                else
                                {
                                    value.X = -0.3f;
                                }
                            }
                        }
                        else
                        {
                            if ((double)value3.Y < Main.rockLayer)
                            {
                                num2 = 1;
                                num += 1.0;
                                value.Y = (float)num2;
                                value.X = 0f;
                                vector.X = 0f;
                                vector.Y = (float)num2;
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        value.X = 0.3f;
                                    }
                                    else
                                    {
                                        value.X = -0.3f;
                                    }
                                }
                            }
                            else
                            {
                                if (value3.X < (float)(Main.maxTilesX / 2) && (double)value3.X > (double)Main.maxTilesX * 0.25)
                                {
                                    num2 = -1;
                                    vector.Y = 0f;
                                    vector.X = (float)num2;
                                    value.Y = 0f;
                                    value.X = (float)num2;
                                    if (WorldGen.genRand.Next(3) == 0)
                                    {
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            value.Y = -0.2f;
                                        }
                                        else
                                        {
                                            value.Y = 0.2f;
                                        }
                                    }
                                }
                                else
                                {
                                    if (value3.X > (float)(Main.maxTilesX / 2) && (double)value3.X < (double)Main.maxTilesX * 0.75)
                                    {
                                        num2 = 1;
                                        vector.Y = 0f;
                                        vector.X = (float)num2;
                                        value.Y = 0f;
                                        value.X = (float)num2;
                                        if (WorldGen.genRand.Next(3) == 0)
                                        {
                                            if (WorldGen.genRand.Next(2) == 0)
                                            {
                                                value.Y = -0.2f;
                                            }
                                            else
                                            {
                                                value.Y = 0.2f;
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
                WorldGen.DDoorX[WorldGen.numDDoors] = (int)value3.X;
                WorldGen.DDoorY[WorldGen.numDDoors] = (int)value3.Y;
                WorldGen.DDoorPos[WorldGen.numDDoors] = 0;
                WorldGen.numDDoors++;
            }
            else
            {
                WorldGen.DPlatX[WorldGen.numDPlats] = (int)value3.X;
                WorldGen.DPlatY[WorldGen.numDPlats] = (int)value3.Y;
                WorldGen.numDPlats++;
            }
            WorldGen.lastDungeonHall = vector;
            while (k > 0)
            {
                if (vector.X > 0f && value3.X > (float)(Main.maxTilesX - 100))
                {
                    k = 0;
                }
                else
                {
                    if (vector.X < 0f && value3.X < 100f)
                    {
                        k = 0;
                    }
                    else
                    {
                        if (vector.Y > 0f && value3.Y > (float)(Main.maxTilesY - 100))
                        {
                            k = 0;
                        }
                        else
                        {
                            if (vector.Y < 0f && (double)value3.Y < Main.rockLayer + 50.0)
                            {
                                k = 0;
                            }
                        }
                    }
                }
                k--;
                int num3 = (int)((double)value3.X - num - 4.0 - (double)WorldGen.genRand.Next(6));
                int num4 = (int)((double)value3.X + num + 4.0 + (double)WorldGen.genRand.Next(6));
                int num5 = (int)((double)value3.Y - num - 4.0 - (double)WorldGen.genRand.Next(6));
                int num6 = (int)((double)value3.Y + num + 4.0 + (double)WorldGen.genRand.Next(6));
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
                        Main.tile.At(l, m).SetLiquid (0);
                        if (Main.tile.At(l, m).Wall == 0)
                        {
                            Main.tile.At(l, m).SetActive (true);
                            Main.tile.At(l, m).SetType ((byte)tileType);
                        }
                    }
                }
                for (int n = num3 + 1; n < num4 - 1; n++)
                {
                    for (int num7 = num5 + 1; num7 < num6 - 1; num7++)
                    {
                        WorldGen.PlaceWall(n, num7, wallType, true);
                    }
                }
                int num8 = 0;
                if (value.Y == 0f && WorldGen.genRand.Next((int)num + 1) == 0)
                {
                    num8 = WorldGen.genRand.Next(1, 3);
                }
                else
                {
                    if (value.X == 0f && WorldGen.genRand.Next((int)num - 1) == 0)
                    {
                        num8 = WorldGen.genRand.Next(1, 3);
                    }
                    else
                    {
                        if (WorldGen.genRand.Next((int)num * 3) == 0)
                        {
                            num8 = WorldGen.genRand.Next(1, 3);
                        }
                    }
                }
                num3 = (int)((double)value3.X - num * 0.5 - (double)num8);
                num4 = (int)((double)value3.X + num * 0.5 + (double)num8);
                num5 = (int)((double)value3.Y - num * 0.5 - (double)num8);
                num6 = (int)((double)value3.Y + num * 0.5 + (double)num8);
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
                for (int num9 = num3; num9 < num4; num9++)
                {
                    for (int num10 = num5; num10 < num6; num10++)
                    {
                        Main.tile.At(num9, num10).SetActive (false);
                        Main.tile.At(num9, num10).SetWall ((byte)wallType);
                    }
                }
                value3 += value;
            }
            WorldGen.dungeonX = (int)value3.X;
            WorldGen.dungeonY = (int)value3.Y;
            if (vector.Y == 0f)
            {
                WorldGen.DDoorX[WorldGen.numDDoors] = (int)value3.X;
                WorldGen.DDoorY[WorldGen.numDDoors] = (int)value3.Y;
                WorldGen.DDoorPos[WorldGen.numDDoors] = 0;
                WorldGen.numDDoors++;
                return;
            }
            WorldGen.DPlatX[WorldGen.numDPlats] = (int)value3.X;
            WorldGen.DPlatY[WorldGen.numDPlats] = (int)value3.Y;
            WorldGen.numDPlats++;
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
                        Main.tile.At(l, m).SetLiquid (0);
                        if (Main.tile.At(l, m).Wall == 0)
                        {
                            Main.tile.At(l, m).SetActive (true);
                            Main.tile.At(l, m).SetType ((byte)tileType);
                        }
                    }
                }
                for (int n = num6 + 1; n < num7 - 1; n++)
                {
                    for (int num10 = num8 + 1; num10 < num9 - 1; num10++)
                    {
                        WorldGen.PlaceWall(n, num10, wallType, true);
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
                for (int num11 = num6; num11 < num7; num11++)
                {
                    for (int num12 = num8; num12 < num9; num12++)
                    {
                        Main.tile.At(num11, num12).SetActive (false);
                        Main.tile.At(num11, num12).SetWall ((byte)wallType);
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
                    Main.tile.At(k, l).SetLiquid (0);
                    if ((int)Main.tile.At(k, l).Wall != wallType)
                    {
                        Main.tile.At(k, l).SetWall (0);
                        if (k > num4 + 1 && k < num5 - 2 && l > num6 + 1 && l < num7 - 2)
                        {
                            WorldGen.PlaceWall(k, l, wallType, true);
                        }
                        Main.tile.At(k, l).SetActive (true);
                        Main.tile.At(k, l).SetType ((byte)tileType);
                    }
                }
            }
            int num8 = num4;
            int num9 = num4 + 5 + WorldGen.genRand.Next(4);
            int num10 = num6 - 3 - WorldGen.genRand.Next(3);
            int num11 = num6;
            for (int m = num8; m < num9; m++)
            {
                for (int n = num10; n < num11; n++)
                {
                    if ((int)Main.tile.At(m, n).Wall != wallType)
                    {
                        Main.tile.At(m, n).SetActive (true);
                        Main.tile.At(m, n).SetType ((byte)tileType);
                    }
                }
            }
            num8 = num5 - 5 - WorldGen.genRand.Next(4);
            num9 = num5;
            num10 = num6 - 3 - WorldGen.genRand.Next(3);
            num11 = num6;
            for (int num12 = num8; num12 < num9; num12++)
            {
                for (int num13 = num10; num13 < num11; num13++)
                {
                    if ((int)Main.tile.At(num12, num13).Wall != wallType)
                    {
                        Main.tile.At(num12, num13).SetActive (true);
                        Main.tile.At(num12, num13).SetType ((byte)tileType);
                    }
                }
            }
            int num14 = 1 + WorldGen.genRand.Next(2);
            int num15 = 2 + WorldGen.genRand.Next(4);
            int num16 = 0;
            for (int num17 = num4; num17 < num5; num17++)
            {
                for (int num18 = num6 - num14; num18 < num6; num18++)
                {
                    if ((int)Main.tile.At(num17, num18).Wall != wallType)
                    {
                        Main.tile.At(num17, num18).SetActive (true);
                        Main.tile.At(num17, num18).SetType ((byte)tileType);
                    }
                }
                num16++;
                if (num16 >= num15)
                {
                    num17 += num15;
                    num16 = 0;
                }
            }
            for (int num19 = num4; num19 < num5; num19++)
            {
                for (int num20 = num7; num20 < num7 + 100; num20++)
                {
                    WorldGen.PlaceWall(num19, num20, 2, true);
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
            for (int num21 = num4; num21 < num5; num21++)
            {
                for (int num22 = num6; num22 < num7; num22++)
                {
                    WorldGen.PlaceWall(num21, num22, wallType, true);
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
            for (int num23 = num4; num23 < num5; num23++)
            {
                for (int num24 = num6; num24 < num7; num24++)
                {
                    Main.tile.At(num23, num24).SetWall ((byte)wallType);
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
            for (int num25 = num4; num25 < num5; num25++)
            {
                for (int num26 = num6; num26 < num7; num26++)
                {
                    Main.tile.At(num25, num26).SetActive (false);
                    Main.tile.At(num25, num26).SetWall ((byte)wallType);
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
            for (int num27 = num4; num27 < num5; num27++)
            {
                for (int num28 = num6; num28 < num7; num28++)
                {
                    if ((int)Main.tile.At(num27, num28).Wall != wallType)
                    {
                        bool flag = true;
                        if (num3 < 0)
                        {
                            if ((double)num27 < (double)vector.X - num * 0.5)
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            if ((double)num27 > (double)vector.X + num * 0.5 - 1.0)
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            Main.tile.At(num27, num28).SetWall (0);
                            Main.tile.At(num27, num28).SetActive (true);
                            Main.tile.At(num27, num28).SetType ((byte)tileType);
                        }
                    }
                }
            }
            for (int num29 = num4; num29 < num5; num29++)
            {
                for (int num30 = num7; num30 < num7 + 100; num30++)
                {
                    WorldGen.PlaceWall(num29, num30, 2, true);
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
            for (int num31 = num8; num31 < num9; num31++)
            {
                for (int num32 = num10; num32 < num11; num32++)
                {
                    if ((int)Main.tile.At(num31, num32).Wall != wallType)
                    {
                        Main.tile.At(num31, num32).SetActive (true);
                        Main.tile.At(num31, num32).SetType ((byte)tileType);
                    }
                }
            }
            num8 = num5 - 5 - WorldGen.genRand.Next(4);
            num9 = num5;
            num10 = num6 - 3 - WorldGen.genRand.Next(3);
            num11 = num6;
            for (int num33 = num8; num33 < num9; num33++)
            {
                for (int num34 = num10; num34 < num11; num34++)
                {
                    if ((int)Main.tile.At(num33, num34).Wall != wallType)
                    {
                        Main.tile.At(num33, num34).SetActive (true);
                        Main.tile.At(num33, num34).SetType ((byte)tileType);
                    }
                }
            }
            num14 = 1 + WorldGen.genRand.Next(2);
            num15 = 2 + WorldGen.genRand.Next(4);
            num16 = 0;
            if (num3 < 0)
            {
                num5++;
            }
            for (int num35 = num4 + 1; num35 < num5 - 1; num35++)
            {
                for (int num36 = num6 - num14; num36 < num6; num36++)
                {
                    if ((int)Main.tile.At(num35, num36).Wall != wallType)
                    {
                        Main.tile.At(num35, num36).SetActive (true);
                        Main.tile.At(num35, num36).SetType ((byte)tileType);
                    }
                }
                num16++;
                if (num16 >= num15)
                {
                    num35 += num15;
                    num16 = 0;
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
            for (int num37 = num4; num37 < num5; num37++)
            {
                for (int num38 = num6; num38 < num7; num38++)
                {
                    Main.tile.At(num37, num38).SetWall (0);
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
            for (int num39 = num4; num39 < num5; num39++)
            {
                for (int num40 = num6; num40 < num7; num40++)
                {
                    Main.tile.At(num39, num40).SetActive (false);
                    Main.tile.At(num39, num40).SetWall (0);
                }
            }
            for (int num41 = num4; num41 < num5; num41++)
            {
                if (!Main.tile.At(num41, num7).Active)
                {
                    Main.tile.At(num41, num7).SetActive (true);
                    Main.tile.At(num41, num7).SetType (19);
                }
            }
            Main.dungeonX = (int)vector.X;
            Main.dungeonY = num7;
            int num42 = NPC.NewNPC(WorldGen.dungeonX * 16 + 8, WorldGen.dungeonY * 16, 37, 0);
            Main.npcs[num42].homeless = false;
            Main.npcs[num42].homeTileX = Main.dungeonX;
            Main.npcs[num42].homeTileY = Main.dungeonY;
            if (num3 == 1)
            {
                int num43 = 0;
                for (int num44 = num5; num44 < num5 + 25; num44++)
                {
                    num43++;
                    for (int num45 = num7 + num43; num45 < num7 + 25; num45++)
                    {
                        Main.tile.At(num44, num45).SetActive (true);
                        Main.tile.At(num44, num45).SetType ((byte)tileType);
                    }
                }
            }
            else
            {
                int num46 = 0;
                for (int num47 = num4; num47 > num4 - 25; num47--)
                {
                    num46++;
                    for (int num48 = num7 + num46; num48 < num7 + 25; num48++)
                    {
                        Main.tile.At(num47, num48).SetActive (true);
                        Main.tile.At(num47, num48).SetType ((byte)tileType);
                    }
                }
            }
            num14 = 1 + WorldGen.genRand.Next(2);
            num15 = 2 + WorldGen.genRand.Next(4);
            num16 = 0;
            num4 = (int)((double)vector.X - num * 0.5);
            num5 = (int)((double)vector.X + num * 0.5);
            num4 += 2;
            num5 -= 2;
            for (int num49 = num4; num49 < num5; num49++)
            {
                for (int num50 = num6; num50 < num7; num50++)
                {
                    WorldGen.PlaceWall(num49, num50, wallType, true);
                }
                num16++;
                if (num16 >= num15)
                {
                    num49 += num15 * 2;
                    num16 = 0;
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
            for (int num51 = num4; num51 < num5; num51++)
            {
                for (int num52 = num6; num52 < num7; num52++)
                {
                    Main.tile.At(num51, num52).SetActive (false);
                }
            }
            if (num3 < 0)
            {
                vector.X -= 1f;
            }
            WorldGen.PlaceTile((int)vector.X, (int)vector.Y + 1, 10, false, false, -1, 0);
        }


        public static bool AddBuriedChest(int i, int j, int contain = 0, bool notNearOtherChests = false)
        {
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
            }
            int k = j;
            while (k < Main.maxTilesY)
            {
                if (Main.tile.At(i,k).Active && Main.tileSolid[(int)Main.tile.At(i, k).Type])
                {
                    int num = k;
                    int style = 0;
                    if ((double)num >= Main.worldSurface + 25.0 || contain > 0)
                    {
                        style = 1;
                    }
                    int num2 = WorldGen.PlaceChest(i - 1, num - 1, 21, notNearOtherChests, style);
                    if (num2 >= 0)
                    {
                        int num3 = 0;
                        while (num3 == 0)
                        {
                            if ((double)num < Main.worldSurface + 25.0)
                            {
                                if (contain > 0)
                                {
                                    Main.chest[num2].contents[num3] = Registries.Item.Create(contain);
                                    num3++;
                                }
                                else
                                {
                                    int num4 = WorldGen.genRand.Next(6);
                                    if (num4 == 0)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(280);
                                    }
                                    if (num4 == 1)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(281);
                                    }
                                    if (num4 == 2)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(284);
                                    }
                                    if (num4 == 3)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(282);
                                        Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(50, 75);
                                    }
                                    if (num4 == 4)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(279);
                                        Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(25, 50);
                                    }
                                    if (num4 == 5)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(285);
                                    }
                                    num3++;
                                }
                                if (WorldGen.genRand.Next(3) == 0)
                                {
                                    Main.chest[num2].contents[num3] = Registries.Item.Create(168);
                                    Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(3, 6);
                                    num3++;
                                }
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    int num5 = WorldGen.genRand.Next(2);
                                    int stack = WorldGen.genRand.Next(8) + 3;
                                    if (num5 == 0)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(20);
                                    }
                                    if (num5 == 1)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(22);
                                    }
                                    Main.chest[num2].contents[num3].Stack = stack;
                                    num3++;
                                }
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    int num6 = WorldGen.genRand.Next(2);
                                    int stack2 = WorldGen.genRand.Next(26) + 25;
                                    if (num6 == 0)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(40);
                                    }
                                    if (num6 == 1)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(42);
                                    }
                                    Main.chest[num2].contents[num3].Stack = stack2;
                                    num3++;
                                }
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    int num7 = WorldGen.genRand.Next(1);
                                    int stack3 = WorldGen.genRand.Next(3) + 3;
                                    if (num7 == 0)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(28);
                                    }
                                    Main.chest[num2].contents[num3].Stack = stack3;
                                    num3++;
                                }
                                if (WorldGen.genRand.Next(3) > 0)
                                {
                                    int num8 = WorldGen.genRand.Next(4);
                                    int stack4 = WorldGen.genRand.Next(1, 3);
                                    if (num8 == 0)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(292);
                                    }
                                    if (num8 == 1)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(298);
                                    }
                                    if (num8 == 2)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(299);
                                    }
                                    if (num8 == 3)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(290);
                                    }
                                    Main.chest[num2].contents[num3].Stack = stack4;
                                    num3++;
                                }
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    int num9 = WorldGen.genRand.Next(2);
                                    int stack5 = WorldGen.genRand.Next(11) + 10;
                                    if (num9 == 0)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(8);
                                    }
                                    if (num9 == 1)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(31);
                                    }
                                    Main.chest[num2].contents[num3].Stack = stack5;
                                    num3++;
                                }
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    Main.chest[num2].contents[num3] = Registries.Item.Create(72);
                                    Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(10, 30);
                                    num3++;
                                }
                            }
                            else
                            {
                                if ((double)num < Main.rockLayer)
                                {
                                    if (contain > 0)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(contain);
                                        num3++;
                                    }
                                    else
                                    {
                                        int num10 = WorldGen.genRand.Next(7);
                                        if (num10 == 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(49);
                                        }
                                        if (num10 == 1)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(50);
                                        }
                                        if (num10 == 2)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(52);
                                        }
                                        if (num10 == 3)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(53);
                                        }
                                        if (num10 == 4)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(54);
                                        }
                                        if (num10 == 5)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(55);
                                        }
                                        if (num10 == 6)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(51);
                                            Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(26) + 25;
                                        }
                                        num3++;
                                    }
                                    if (WorldGen.genRand.Next(3) == 0)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(166);
                                        Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(10, 20);
                                        num3++;
                                    }
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        int num11 = WorldGen.genRand.Next(2);
                                        int stack6 = WorldGen.genRand.Next(10) + 5;
                                        if (num11 == 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(22);
                                        }
                                        if (num11 == 1)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(21);
                                        }
                                        Main.chest[num2].contents[num3].Stack = stack6;
                                        num3++;
                                    }
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        int num12 = WorldGen.genRand.Next(2);
                                        int stack7 = WorldGen.genRand.Next(25) + 25;
                                        if (num12 == 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(40);
                                        }
                                        if (num12 == 1)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(42);
                                        }
                                        Main.chest[num2].contents[num3].Stack = stack7;
                                        num3++;
                                    }
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        int num13 = WorldGen.genRand.Next(1);
                                        int stack8 = WorldGen.genRand.Next(3) + 3;
                                        if (num13 == 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(28);
                                        }
                                        Main.chest[num2].contents[num3].Stack = stack8;
                                        num3++;
                                    }
                                    if (WorldGen.genRand.Next(3) > 0)
                                    {
                                        int num14 = WorldGen.genRand.Next(7);
                                        int stack9 = WorldGen.genRand.Next(1, 3);
                                        if (num14 == 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(289);
                                        }
                                        if (num14 == 1)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(298);
                                        }
                                        if (num14 == 2)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(299);
                                        }
                                        if (num14 == 3)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(290);
                                        }
                                        if (num14 == 4)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(303);
                                        }
                                        if (num14 == 5)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(291);
                                        }
                                        if (num14 == 6)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(304);
                                        }
                                        Main.chest[num2].contents[num3].Stack = stack9;
                                        num3++;
                                    }
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        int stack10 = WorldGen.genRand.Next(11) + 10;
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(8);
                                        Main.chest[num2].contents[num3].Stack = stack10;
                                        num3++;
                                    }
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        Main.chest[num2].contents[num3] = Registries.Item.Create(72);
                                        Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(50, 90);
                                        num3++;
                                    }
                                }
                                else
                                {
                                    if (num < Main.maxTilesY - 250)
                                    {
                                        if (contain > 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(contain);
                                            num3++;
                                        }
                                        else
                                        {
                                            int num15 = WorldGen.genRand.Next(7);
                                            if (num15 == 2 && WorldGen.genRand.Next(2) == 0)
                                            {
                                                num15 = WorldGen.genRand.Next(7);
                                            }
                                            if (num15 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(49);
                                            }
                                            if (num15 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(50);
                                            }
                                            if (num15 == 2)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(52);
                                            }
                                            if (num15 == 3)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(53);
                                            }
                                            if (num15 == 4)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(54);
                                            }
                                            if (num15 == 5)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(55);
                                            }
                                            if (num15 == 6)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(51);
                                                Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(26) + 25;
                                            }
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(3) == 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(167);
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            int num16 = WorldGen.genRand.Next(2);
                                            int stack11 = WorldGen.genRand.Next(8) + 3;
                                            if (num16 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(19);
                                            }
                                            if (num16 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(21);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack11;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            int num17 = WorldGen.genRand.Next(2);
                                            int stack12 = WorldGen.genRand.Next(26) + 25;
                                            if (num17 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(41);
                                            }
                                            if (num17 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(279);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack12;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            int num18 = WorldGen.genRand.Next(1);
                                            int stack13 = WorldGen.genRand.Next(3) + 3;
                                            if (num18 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(188);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack13;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(3) > 0)
                                        {
                                            int num19 = WorldGen.genRand.Next(6);
                                            int stack14 = WorldGen.genRand.Next(1, 3);
                                            if (num19 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(296);
                                            }
                                            if (num19 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(295);
                                            }
                                            if (num19 == 2)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(299);
                                            }
                                            if (num19 == 3)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(302);
                                            }
                                            if (num19 == 4)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(303);
                                            }
                                            if (num19 == 5)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(305);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack14;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(3) > 1)
                                        {
                                            int num20 = WorldGen.genRand.Next(4);
                                            int stack15 = WorldGen.genRand.Next(1, 3);
                                            if (num20 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(301);
                                            }
                                            if (num20 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(302);
                                            }
                                            if (num20 == 2)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(297);
                                            }
                                            if (num20 == 3)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(304);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack15;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            int num21 = WorldGen.genRand.Next(2);
                                            int stack16 = WorldGen.genRand.Next(15) + 15;
                                            if (num21 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(8);
                                            }
                                            if (num21 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(282);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack16;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(73);
                                            Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(1, 3);
                                            num3++;
                                        }
                                    }
                                    else
                                    {
                                        if (contain > 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(contain);
                                            num3++;
                                        }
                                        else
                                        {
                                            if (WorldGen.hellChest == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(274);
                                            }
                                            else
                                            {
                                                int num22 = WorldGen.genRand.Next(4);
                                                if (num22 == 0)
                                                {
                                                    Main.chest[num2].contents[num3] = Registries.Item.Create(49);
                                                }
                                                if (num22 == 1)
                                                {
                                                    Main.chest[num2].contents[num3] = Registries.Item.Create(50);
                                                }
                                                if (num22 == 2)
                                                {
                                                    Main.chest[num2].contents[num3] = Registries.Item.Create(53);
                                                }
                                                if (num22 == 3)
                                                {
                                                    Main.chest[num2].contents[num3] = Registries.Item.Create(54);
                                                }
                                            }
                                            num3++;
                                            WorldGen.hellChest++;
                                        }
                                        if (WorldGen.genRand.Next(3) == 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(167);
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            int num23 = WorldGen.genRand.Next(2);
                                            int stack17 = WorldGen.genRand.Next(15) + 15;
                                            if (num23 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(117);
                                            }
                                            if (num23 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(19);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack17;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            int num24 = WorldGen.genRand.Next(2);
                                            int stack18 = WorldGen.genRand.Next(25) + 50;
                                            if (num24 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(265);
                                            }
                                            if (num24 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(278);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack18;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            int num25 = WorldGen.genRand.Next(2);
                                            int stack19 = WorldGen.genRand.Next(15) + 15;
                                            if (num25 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(226);
                                            }
                                            if (num25 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(227);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack19;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(4) > 0)
                                        {
                                            int num26 = WorldGen.genRand.Next(7);
                                            int stack20 = WorldGen.genRand.Next(1, 3);
                                            if (num26 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(296);
                                            }
                                            if (num26 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(295);
                                            }
                                            if (num26 == 2)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(293);
                                            }
                                            if (num26 == 3)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(288);
                                            }
                                            if (num26 == 4)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(294);
                                            }
                                            if (num26 == 5)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(297);
                                            }
                                            if (num26 == 6)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(304);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack20;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(3) > 0)
                                        {
                                            int num27 = WorldGen.genRand.Next(5);
                                            int stack21 = WorldGen.genRand.Next(1, 3);
                                            if (num27 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(305);
                                            }
                                            if (num27 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(301);
                                            }
                                            if (num27 == 2)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(302);
                                            }
                                            if (num27 == 3)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(288);
                                            }
                                            if (num27 == 4)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(300);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack21;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            int num28 = WorldGen.genRand.Next(2);
                                            int stack22 = WorldGen.genRand.Next(15) + 15;
                                            if (num28 == 0)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(8);
                                            }
                                            if (num28 == 1)
                                            {
                                                Main.chest[num2].contents[num3] = Registries.Item.Create(282);
                                            }
                                            Main.chest[num2].contents[num3].Stack = stack22;
                                            num3++;
                                        }
                                        if (WorldGen.genRand.Next(2) == 0)
                                        {
                                            Main.chest[num2].contents[num3] = Registries.Item.Create(73);
                                            Main.chest[num2].contents[num3].Stack = WorldGen.genRand.Next(2, 5);
                                            num3++;
                                        }
                                    }
                                }
                            }
                        }
                        return true;
                    }
                    return false;
                }
                else
                {
                    k++;
                }
            }
            return false;
        }


        public static bool OpenDoor(int x, int y, int direction, bool state = false, DoorOpener opener = DoorOpener.SERVER, ISender sender = null)
        {
            if (sender == null)
            {
                ConsoleSender cSender = new ConsoleSender(Program.server);
                cSender.ConsoleCommand.Sender = new Sender();
                sender = cSender;
            }

            DoorStateChangeEvent doorEvent = new DoorStateChangeEvent();
            doorEvent.Sender = sender;
            doorEvent.X = x;
            doorEvent.Y = y;
            doorEvent.Direction = direction;
            doorEvent.Opener = opener;
            doorEvent.isOpened = state;
            Program.server.PluginManager.processHook(Hooks.DOOR_STATECHANGE, doorEvent);
            if (doorEvent.Cancelled)
            {
                return true;
            }

            int num = 0;
            if (Main.tile.At(x, y - 1).FrameY == 0 && Main.tile.At(x, y - 1).Type == Main.tile.At(x, y).Type)
            {
                num = y - 1;
            }
            else
            {
                if (Main.tile.At(x, y - 2).FrameY == 0 && Main.tile.At(x, y - 2).Type == Main.tile.At(x, y).Type)
                {
                    num = y - 2;
                }
                else
                {
                    if (Main.tile.At(x, y + 1).FrameY == 0 && Main.tile.At(x, y + 1).Type == Main.tile.At(x, y).Type)
                    {
                        num = y + 1;
                    }
                    else
                    {
                        num = y;
                    }
                }
            }
            int num2 = x;
            short num3 = 0;
            int num4;
            if (direction == -1)
            {
                num2 = x - 1;
                num3 = 36;
                num4 = x - 1;
            }
            else
            {
                num2 = x;
                num4 = x + 1;
            }
            bool flag = true;
            for (int k = num; k < num + 3; k++)
            {
                if (Main.tile.At(num4, k).Active)
                {
                    if (Main.tile.At(num4, k).Type != 3 && Main.tile.At(num4, k).Type != 24 && Main.tile.At(num4, k).Type != 52 && Main.tile.At(num4, k).Type != 61 && Main.tile.At(num4, k).Type != 62 && Main.tile.At(num4, k).Type != 69 && Main.tile.At(num4, k).Type != 71 && Main.tile.At(num4, k).Type != 73 && Main.tile.At(num4, k).Type != 74)
                    {
                        flag = false;
                        break;
                    }
                    WorldGen.KillTile(num4, k, false, false, false);
                }
            }
            if (flag)
            {
                Main.tile.At(num2, num).SetActive (true);
                Main.tile.At(num2, num).SetType (11);
                Main.tile.At(num2, num).SetFrameY (0);
                Main.tile.At(num2, num).SetFrameX (num3);
                Main.tile.At(num2 + 1, num).SetActive (true);
                Main.tile.At(num2 + 1, num).SetType (11);
                Main.tile.At(num2 + 1, num).SetFrameY (0);
                Main.tile.At(num2 + 1, num).SetFrameX ((short)(num3 + 18));
                Main.tile.At(num2, num + 1).SetActive (true);
                Main.tile.At(num2, num + 1).SetType (11);
                Main.tile.At(num2, num + 1).SetFrameY (18);
                Main.tile.At(num2, num + 1).SetFrameX (num3);
                Main.tile.At(num2 + 1, num + 1).SetActive (true);
                Main.tile.At(num2 + 1, num + 1).SetType (11);
                Main.tile.At(num2 + 1, num + 1).SetFrameY (18);
                Main.tile.At(num2 + 1, num + 1).SetFrameX ((short)(num3 + 18));
                Main.tile.At(num2, num + 2).SetActive (true);
                Main.tile.At(num2, num + 2).SetType (11);
                Main.tile.At(num2, num + 2).SetFrameY (36);
                Main.tile.At(num2, num + 2).SetFrameX (num3);
                Main.tile.At(num2 + 1, num + 2).SetActive (true);
                Main.tile.At(num2 + 1, num + 2).SetType (11);
                Main.tile.At(num2 + 1, num + 2).SetFrameY (36);
                Main.tile.At(num2 + 1, num + 2).SetFrameX ((short)(num3 + 18));
                for (int l = num2 - 1; l <= num2 + 2; l++)
                {
                    for (int m = num - 1; m <= num + 2; m++)
                    {
                        WorldGen.TileFrame(l, m, false, false);
                    }
                }
            }
            return flag;
        }
        
        public static void Check1x2(int x, int j, byte type)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }
            int num = j;
            bool flag = true;
            if (Main.tile.At(x, num).FrameY == 18)
            {
                num--;
            }
            if (Main.tile.At(x, num).FrameY == 0 && Main.tile.At(x, num + 1).FrameY == 18 && Main.tile.At(x, num).Type == type && Main.tile.At(x, num + 1).Type == type)
            {
                flag = false;
            }
            if (!Main.tile.At(x, num + 2).Active || !Main.tileSolid[(int)Main.tile.At(x, num + 2).Type])
            {
                flag = true;
            }
            if (Main.tile.At(x, num + 2).Type != 2 && Main.tile.At(x, num).Type == 20)
            {
                flag = true;
            }
            if (flag)
            {
                WorldGen.destroyObject = true;
                if (Main.tile.At(x, num).Type == type)
                {
                    WorldGen.KillTile(x, num, false, false, false);
                }
                if (Main.tile.At(x, num + 1).Type == type)
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
        
        public static void CheckOnTable1x1(int x, int y, int type)
        {
            if ((!Main.tile.At(x, y + 1).Active || !Main.tileTable[(int)Main.tile.At(x, y + 1).Type]))
            {
                if (type == 78)
                {
                    if (!Main.tile.At(x, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x, y + 1).Type])
                    {
                        WorldGen.KillTile(x, y, false, false, false);
                        return;
                    }
                }
                else
                {
                    WorldGen.KillTile(x, y, false, false, false);
                }
            }
        }
        
        public static void CheckSign(int x, int y, int type)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }
            int num = x - 2;
            int num2 = x + 3;
            int num3 = y - 2;
            int num4 = y + 3;
            if (num < 0)
            {
                return;
            }
            if (num2 > Main.maxTilesX)
            {
                return;
            }
            if (num3 < 0)
            {
                return;
            }
            if (num4 > Main.maxTilesY)
            {
                return;
            }
            bool flag = false;
            int k = (int)(Main.tile.At(x, y).FrameX / 18);
            int num5 = (int)(Main.tile.At(x, y).FrameY / 18);
            while (k > 1)
            {
                k -= 2;
            }
            int num6 = x - k;
            int num7 = y - num5;
            int num8 = (int)(Main.tile.At(num6, num7).FrameX / 18 / 2);
            num = num6;
            num2 = num6 + 2;
            num3 = num7;
            num4 = num7 + 2;
            k = 0;
            for (int l = num; l < num2; l++)
            {
                num5 = 0;
                for (int m = num3; m < num4; m++)
                {
                    if (!Main.tile.At(l, m).Active || (int)Main.tile.At(l, m).Type != type)
                    {
                        flag = true;
                        break;
                    }
                    if ((int)(Main.tile.At(l, m).FrameX / 18) != k + num8 * 2 || (int)(Main.tile.At(l, m).FrameY / 18) != num5)
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
                if (type == 85)
                {
                    if (Main.tile.At(num6, num7 + 2).Active && Main.tileSolid[(int)Main.tile.At(num6, num7 + 2).Type] && Main.tile.At(num6 + 1, num7 + 2).Active && Main.tileSolid[(int)Main.tile.At(num6 + 1, num7 + 2).Type])
                    {
                        num8 = 0;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (Main.tile.At(num6, num7 + 2).Active && Main.tileSolid[(int)Main.tile.At(num6, num7 + 2).Type] && Main.tile.At(num6 + 1, num7 + 2).Active && Main.tileSolid[(int)Main.tile.At(num6 + 1, num7 + 2).Type])
                    {
                        num8 = 0;
                    }
                    else
                    {
                        if (Main.tile.At(num6, num7 - 1).Active && Main.tileSolid[(int)Main.tile.At(num6, num7 - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6, num7 - 1).Type] && Main.tile.At(num6 + 1, num7 - 1).Active && Main.tileSolid[(int)Main.tile.At(num6 + 1, num7 - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 + 1, num7 - 1).Type])
                        {
                            num8 = 1;
                        }
                        else
                        {
                            if (Main.tile.At(num6 - 1, num7).Active && Main.tileSolid[(int)Main.tile.At(num6 - 1, num7).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 - 1, num7).Type] && Main.tile.At(num6 - 1, num7 + 1).Active && Main.tileSolid[(int)Main.tile.At(num6 - 1, num7 + 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 - 1, num7 + 1).Type])
                            {
                                num8 = 2;
                            }
                            else
                            {
                                if (Main.tile.At(num6 + 2, num7).Active && Main.tileSolid[(int)Main.tile.At(num6 + 2, num7).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 + 2, num7).Type] && Main.tile.At(num6 + 2, num7 + 1).Active && Main.tileSolid[(int)Main.tile.At(num6 + 2, num7 + 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 + 2, num7 + 1).Type])
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
            }
            if (flag)
            {
                WorldGen.destroyObject = true;
                for (int n = num; n < num2; n++)
                {
                    for (int num9 = num3; num9 < num4; num9++)
                    {
                        if ((int)Main.tile.At(n, num9).Type == type)
                        {
                            WorldGen.KillTile(n, num9, false, false, false);
                        }
                    }
                }
                Sign.KillSign(num6, num7);
                if (type == 85)
                {
                    Item.NewItem(x * 16, y * 16, 32, 32, 321, 1, false);
                }
                else
                {
                    Item.NewItem(x * 16, y * 16, 32, 32, 171, 1, false);
                }
                WorldGen.destroyObject = false;
                return;
            }
            int num10 = 36 * num8;
            for (int num11 = 0; num11 < 2; num11++)
            {
                for (int num12 = 0; num12 < 2; num12++)
                {
                    Main.tile.At(num6 + num11, num7 + num12).SetActive (true);
                    Main.tile.At(num6 + num11, num7 + num12).SetType ((byte)type);
                    Main.tile.At(num6 + num11, num7 + num12).SetFrameX ((short)(num10 + 18 * num11));
                    Main.tile.At(num6 + num11, num7 + num12).SetFrameY ((short)(18 * num12));
                }
            }
        }
        
        public static bool PlaceSign(int x, int y, int type)
        {
            int num = x - 2;
            int num2 = x + 3;
            int num3 = y - 2;
            int num4 = y + 3;
            if (num < 0)
            {
                return false;
            }
            if (num2 > Main.maxTilesX)
            {
                return false;
            }
            if (num3 < 0)
            {
                return false;
            }
            if (num4 > Main.maxTilesY)
            {
                return false;
            }
            int num5 = x;
            int num6 = y;
            int num7 = 0;
            if (type == 55)
            {
                if (Main.tile.At(x, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x, y + 1).Type] && Main.tile.At(x + 1, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x + 1, y + 1).Type])
                {
                    num6--;
                    num7 = 0;
                }
                else
                {
                    if (Main.tile.At(x, y - 1).Active && Main.tileSolid[(int)Main.tile.At(x, y - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(x, y - 1).Type] && Main.tile.At(x + 1, y - 1).Active && Main.tileSolid[(int)Main.tile.At(x + 1, y - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(x + 1, y - 1).Type])
                    {
                        num7 = 1;
                    }
                    else
                    {
                        if (Main.tile.At(x - 1, y).Active && Main.tileSolid[(int)Main.tile.At(x - 1, y).Type] && !Main.tileSolidTop[(int)Main.tile.At(x - 1, y).Type] && Main.tile.At(x - 1, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x - 1, y + 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(x - 1, y + 1).Type])
                        {
                            num7 = 2;
                        }
                        else
                        {
                            if (!Main.tile.At(x + 1, y).Active || !Main.tileSolid[(int)Main.tile.At(x + 1, y).Type] || Main.tileSolidTop[(int)Main.tile.At(x + 1, y).Type] || !Main.tile.At(x + 1, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x + 1, y + 1).Type] || Main.tileSolidTop[(int)Main.tile.At(x + 1, y + 1).Type])
                            {
                                return false;
                            }
                            num5--;
                            num7 = 3;
                        }
                    }
                }
            }
            else
            {
                if (type == 85)
                {
                    if (!Main.tile.At(x, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x, y + 1).Type] || !Main.tile.At(x + 1, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x + 1, y + 1).Type])
                    {
                        return false;
                    }
                    num6--;
                    num7 = 0;
                }
            }
            if (Main.tile.At(num5, num6).Active || Main.tile.At(num5 + 1, num6).Active || Main.tile.At(num5, num6 + 1).Active || Main.tile.At(num5 + 1, num6 + 1).Active)
            {
                return false;
            }
            int num8 = 36 * num7;
            for (int k = 0; k < 2; k++)
            {
                for (int l = 0; l < 2; l++)
                {
                    Main.tile.At(num5 + k, num6 + l).SetActive (true);
                    Main.tile.At(num5 + k, num6 + l).SetType ((byte)type);
                    Main.tile.At(num5 + k, num6 + l).SetFrameX ((short)(num8 + 18 * k));
                    Main.tile.At(num5 + k, num6 + l).SetFrameY ((short)(18 * l));
                }
            }
            return true;
        }
        
        public static void PlaceOnTable1x1(int x, int y, int type)
        {
            bool flag = false;
            if (!Main.tile.At(x, y).Active && Main.tile.At(x, y + 1).Active && Main.tileTable[(int)Main.tile.At(x, y + 1).Type])
            {
                flag = true;
            }
            if (type == 78 && !Main.tile.At(x, y).Active && Main.tile.At(x, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x, y + 1).Type])
            {
                flag = true;
            }
            if (flag)
            {
                Main.tile.At(x, y).SetActive (true);
                Main.tile.At(x, y).SetFrameX (0);
                Main.tile.At(x, y).SetFrameY (0);
                Main.tile.At(x, y).SetType ((byte)type);
                if (type == 50)
                {
                    Main.tile.At(x, y).SetFrameX ((short)(18 * WorldGen.genRand.Next(5)));
                }
            }
        }
        
        public static bool PlaceAlch(int x, int y, int style)
        {
            if (!Main.tile.At(x, y).Active && Main.tile.At(x, y + 1).Active)
            {
                bool flag = false;
                if (style == 0)
                {
                    if (Main.tile.At(x, y + 1).Type != 2 && Main.tile.At(x, y + 1).Type != 78)
                    {
                        flag = true;
                    }
                    if (Main.tile.At(x, y).Liquid > 0)
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (style == 1)
                    {
                        if (Main.tile.At(x, y + 1).Type != 60 && Main.tile.At(x, y + 1).Type != 78)
                        {
                            flag = true;
                        }
                        if (Main.tile.At(x, y).Liquid > 0)
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        if (style == 2)
                        {
                            if (Main.tile.At(x, y + 1).Type != 0 && Main.tile.At(x, y + 1).Type != 59 && Main.tile.At(x, y + 1).Type != 78)
                            {
                                flag = true;
                            }
                            if (Main.tile.At(x, y).Liquid > 0)
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            if (style == 3)
                            {
                                if (Main.tile.At(x, y + 1).Type != 23 && Main.tile.At(x, y + 1).Type != 25 && Main.tile.At(x, y + 1).Type != 78)
                                {
                                    flag = true;
                                }
                                if (Main.tile.At(x, y).Liquid > 0)
                                {
                                    flag = true;
                                }
                            }
                            else
                            {
                                if (style == 4)
                                {
                                    if (Main.tile.At(x, y + 1).Type != 53 && Main.tile.At(x, y + 1).Type != 78)
                                    {
                                        flag = true;
                                    }
                                    if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
                                    {
                                        flag = true;
                                    }
                                }
                                else
                                {
                                    if (style == 5)
                                    {
                                        if (Main.tile.At(x, y + 1).Type != 57 && Main.tile.At(x, y + 1).Type != 78)
                                        {
                                            flag = true;
                                        }
                                        if (Main.tile.At(x, y).Liquid > 0 && !Main.tile.At(x, y).Lava)
                                        {
                                            flag = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!flag)
                {
                    Main.tile.At(x, y).SetActive (true);
                    Main.tile.At(x, y).SetType (82);
                    Main.tile.At(x, y).SetFrameX ((short)(18 * style));
                    Main.tile.At(x, y).SetFrameY (0);
                    return true;
                }
            }
            return false;
        }
        
        public static void GrowAlch(int x, int y)
        {
            if (Main.tile.At(x, y).Active)
            {
                if (Main.tile.At(x, y).Type == 82 && WorldGen.genRand.Next(50) == 0)
                {
                    Main.tile.At(x, y).SetType (83);
                    
                    NetMessage.SendTileSquare(-1, x, y, 1);
                    WorldGen.SquareTileFrame(x, y, true);
                    return;
                }
                if (Main.tile.At(x, y).FrameX == 36)
                {
                    if (Main.tile.At(x, y).Type == 83)
                    {
                        Main.tile.At(x, y).SetType (84);
                    }
                    else
                    {
                        Main.tile.At(x, y).SetType (83);
                    }
                    NetMessage.SendTileSquare(-1, x, y, 1);
                }
            }
        }
        
        public static void PlantAlch()
        {
            int num = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
            int num2 = 0;
            if (WorldGen.genRand.Next(40) == 0)
            {
                num2 = WorldGen.genRand.Next((int)(Main.rockLayer + (double)Main.maxTilesY) / 2, Main.maxTilesY - 20);
            }
            else
            {
                if (WorldGen.genRand.Next(10) == 0)
                {
                    num2 = WorldGen.genRand.Next(0, Main.maxTilesY - 20);
                }
                else
                {
                    num2 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
                }
            }
            while (num2 < Main.maxTilesY - 20 && !Main.tile.At(num, num2).Active)
            {
                num2++;
            }
            if (Main.tile.At(num, num2).Active && !Main.tile.At(num, num2 - 1).Active && Main.tile.At(num, num2 - 1).Liquid == 0)
            {
                if (Main.tile.At(num, num2).Type == 2)
                {
                    WorldGen.PlaceAlch(num, num2 - 1, 0);
                }
                if (Main.tile.At(num, num2).Type == 60)
                {
                    WorldGen.PlaceAlch(num, num2 - 1, 1);
                }
                if (Main.tile.At(num, num2).Type == 0 || Main.tile.At(num, num2).Type == 59)
                {
                    WorldGen.PlaceAlch(num, num2 - 1, 2);
                }
                if (Main.tile.At(num, num2).Type == 23 || Main.tile.At(num, num2).Type == 25)
                {
                    WorldGen.PlaceAlch(num, num2 - 1, 3);
                }
                if (Main.tile.At(num, num2).Type == 53)
                {
                    WorldGen.PlaceAlch(num, num2 - 1, 4);
                }
                if (Main.tile.At(num, num2).Type == 57)
                {
                    WorldGen.PlaceAlch(num, num2 - 1, 5);
                }
                if (Main.tile.At(num, num2 - 1).Active)
                {
                    NetMessage.SendTileSquare(-1, num, num2 - 1, 1);
                }
            }
        }
        
        public static void CheckAlch(int x, int y)
        {
            bool flag = false;
            if (!Main.tile.At(x, y + 1).Active)
            {
                flag = true;
            }
            int num = (int)(Main.tile.At(x, y).FrameX / 18);
            Main.tile.At(x, y).SetFrameY (0);
            if (!flag)
            {
                if (num == 0)
                {
                    if (Main.tile.At(x, y + 1).Type != 2 && Main.tile.At(x, y + 1).Type != 78)
                    {
                        flag = true;
                    }
                    if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (num == 1)
                    {
                        if (Main.tile.At(x, y + 1).Type != 60 && Main.tile.At(x, y + 1).Type != 78)
                        {
                            flag = true;
                        }
                        if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        if (num == 2)
                        {
                            if (Main.tile.At(x, y + 1).Type != 0 && Main.tile.At(x, y + 1).Type != 59 && Main.tile.At(x, y + 1).Type != 78)
                            {
                                flag = true;
                            }
                            if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            if (num == 3)
                            {
                                if (Main.tile.At(x, y + 1).Type != 23 && Main.tile.At(x, y + 1).Type != 25 && Main.tile.At(x, y + 1).Type != 78)
                                {
                                    flag = true;
                                }
                                if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
                                {
                                    flag = true;
                                }
                            }
                            else
                            {
                                if (num == 4)
                                {
                                    if (Main.tile.At(x, y + 1).Type != 53 && Main.tile.At(x, y + 1).Type != 78)
                                    {
                                        flag = true;
                                    }
                                    if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
                                    {
                                        flag = true;
                                    }
                                    if (Main.tile.At(x, y).Type != 82 && !Main.tile.At(x, y).Lava)
                                    {
                                        if (Main.tile.At(x, y).Liquid > 16)
                                        {
                                            if (Main.tile.At(x, y).Type == 83)
                                            {
                                                Main.tile.At(x, y).SetType (84);
                                                
                                                NetMessage.SendTileSquare(-1, x, y, 1);
                                            }
                                        }
                                        else
                                        {
                                            if (Main.tile.At(x, y).Type == 84)
                                            {
                                                Main.tile.At(x, y).SetType (83);
                                                
                                                NetMessage.SendTileSquare(-1, x, y, 1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (num == 5)
                                    {
                                        if (Main.tile.At(x, y + 1).Type != 57 && Main.tile.At(x, y + 1).Type != 78)
                                        {
                                            flag = true;
                                        }
                                        if (Main.tile.At(x, y).Liquid > 0 && !Main.tile.At(x, y).Lava)
                                        {
                                            flag = true;
                                        }
                                        if (Main.tile.At(x, y).Type != 82 && Main.tile.At(x, y).Lava && Main.tile.At(x, y).Type != 82 && Main.tile.At(x, y).Lava)
                                        {
                                            if (Main.tile.At(x, y).Liquid > 16)
                                            {
                                                if (Main.tile.At(x, y).Type == 83)
                                                {
                                                    Main.tile.At(x, y).SetType (84);
                                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                                }
                                            }
                                            else
                                            {
                                                if (Main.tile.At(x, y).Type == 84)
                                                {
                                                    Main.tile.At(x, y).SetType (83);
                                                    NetMessage.SendTileSquare(-1, x, y, 1);
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
            if (flag)
            {
                WorldGen.KillTile(x, y, false, false, false);
            }
        }
        
        public static void Place1x2(int x, int y, int type)
        {
            short frameX = 0;
            if (type == 20)
            {
                frameX = (short)(WorldGen.genRand.Next(3) * 18);
            }
            if (Main.tile.At(x, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x, y + 1).Type] && !Main.tile.At(x, y - 1).Active)
            {
                Main.tile.At(x, y - 1).SetActive (true);
                Main.tile.At(x, y - 1).SetFrameY (0);
                Main.tile.At(x, y - 1).SetFrameX (frameX);
                Main.tile.At(x, y - 1).SetType ((byte)type);
                Main.tile.At(x, y).SetActive (true);
                Main.tile.At(x, y).SetFrameY (18);
                Main.tile.At(x, y).SetFrameX (frameX);
                Main.tile.At(x, y).SetType ((byte)type);
            }
        }
                
        public static void Place1x2Top(int x, int y, int type)
        {
            short frameX = 0;
            if (Main.tile.At(x, y - 1).Active && Main.tileSolid[(int)Main.tile.At(x, y - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(x, y - 1).Type] && !Main.tile.At(x, y + 1).Active)
            {
                Main.tile.At(x, y).SetActive (true);
                Main.tile.At(x, y).SetFrameY (0);
                Main.tile.At(x, y).SetFrameX (frameX);
                Main.tile.At(x, y).SetType ((byte)type);
                Main.tile.At(x, y + 1).SetActive (true);
                Main.tile.At(x, y + 1).SetFrameY (18);
                Main.tile.At(x, y + 1).SetFrameX (frameX);
                Main.tile.At(x, y + 1).SetType ((byte)type);
            }
        }

        private static TileRef GetTile(int x, int y)
        {
            return Main.tile.At(x, y);
        }

        private static TileRef[,] GetTiles(int x, int y)
        {
            TileRef[,] tiles = new TileRef[3,3];
            for (int modX = 0; modX < 3; modX++)
            {
                for (int modY = 0; modY < 3; modY++)
                {
                    tiles[modX, modY] = GetTile(x - 1 + modX, y - 1 + modY);
                }
            }
            return tiles;
        }

        public static void Check1x2Top(int x, int y, byte type)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }

            TileRef[,] tiles = GetTiles(x, y);
            if (tiles[1, 1].FrameY == 18)
            {
                tiles = GetTiles(x, --y);
            }

            bool flag = true;
            if (tiles[1, 2].FrameY == 0 && tiles[1, 2].FrameY == 18 && tiles[1, 1].Type == type && tiles[1, 2].Type == type)
            {
                flag = false;

                if (!tiles[1, 0].Active || !Main.tileSolid[(int)tiles[1, 0].Type] || Main.tileSolidTop[(int)tiles[1, 0].Type])
                {
                    flag = true;
                }
            }

            if (flag)
            {
                WorldGen.destroyObject = true;
                if (tiles[1, 1].Type == type)
                {
                    WorldGen.KillTile(x, y, false, false, false);
                }
                if (tiles[1, 2].Type == type)
                {
                    WorldGen.KillTile(x, y + 1, false, false, false);
                }
                if (type == 42)
                {
                    Item.NewItem(x * 16, y * 16, 32, 32, 136, 1, false);
                }
                WorldGen.destroyObject = false;
            }
        }
        
        public static void Check2x1(int x, int y, byte type)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }

            TileRef[,] tiles = GetTiles(x, y);
            if (tiles[1,1].FrameX == 18)
            {
                tiles = GetTiles(--x, y);
            }

            bool flag = true;
            if (tiles[1, 1].FrameX == 0 && tiles[2, 1].FrameX == 18 && tiles[1, 1].Type == type && tiles[2, 1].Type == type)
            {
                flag = false;
                if(!tiles[1, 2].Active || !tiles[2, 2].Active)
                {
                    flag = true;
                } 
                else if(type == 29)
                {
                    if(!Main.tileTable[(int)tiles[1, 2].Type] || !Main.tileTable[(int)tiles[2, 2].Type])
                    {
                        flag = true;
                    }
                }
                else if (type != 29 && (!Main.tileSolid[(int)tiles[1, 2].Type] || !Main.tileSolid[(int)tiles[2, 2].Type]))
                {
                    flag = true;
                }
            }

            if (flag)
            {
                WorldGen.destroyObject = true;
                if (tiles[1,1].Type == type)
                {
                    WorldGen.KillTile(x, y, false, false, false);
                }
                if (tiles[2,1].Type == type)
                {
                    WorldGen.KillTile(x + 1, y, false, false, false);
                }
                if (type == 16)
                {
                    Item.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 35, 1, false);
                }
                if (type == 18)
                {
                    Item.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 36, 1, false);
                }
                if (type == 29)
                {
                    Item.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 87, 1, false);
                }
                WorldGen.destroyObject = false;
            }
        }
        
        public static void Place2x1(int x, int y, int type)
        {
            TileRef[,] tiles = GetTiles(x, y);

            bool flag = false;
            if (type != 29 && tiles[1, 2].Active && tiles[2, 2].Active && Main.tileSolid[(int)tiles[1, 2].Type]
                && Main.tileSolid[(int)tiles[2, 2].Type] && !tiles[1, 1].Active && !tiles[2, 1].Active)
            {
                flag = true;
            }
            else if (type == 29 && tiles[1, 2].Active && tiles[2, 2].Active && Main.tileTable[(int)tiles[1, 2].Type]
                && Main.tileTable[(int)tiles[2, 2].Type] && !tiles[1, 1].Active && !tiles[2, 1].Active)
            {
                flag = true;
            }

            if (flag)
            {
                tiles[1, 1].SetActive (true);
                tiles[1, 1].SetFrameY (0);
                tiles[1, 1].SetFrameX (0);
                tiles[1, 1].SetType ((byte)type);
                tiles[2, 1].SetActive (true);
                tiles[2, 1].SetFrameY (0);
                tiles[2, 1].SetFrameX (18);
                tiles[2, 1].SetType ((byte)type);
            }
        }
        
        public static void Check4x2(int i, int j, int type)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }
            bool flag = false;
            int num = i;
            num += (int)(Main.tile.At(i, j).FrameX / 18 * -1);
            if (type == 79 && Main.tile.At(i, j).FrameX >= 72)
            {
                num += 4;
            }
            int num2 = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
            for (int k = num; k < num + 4; k++)
            {
                for (int l = num2; l < num2 + 2; l++)
                {
                    int num3 = (k - num) * 18;
                    if (type == 79 && Main.tile.At(i, j).FrameX >= 72)
                    {
                        num3 = (k - num + 4) * 18;
                    }
                    if (!Main.tile.At(k, l).Active || (int)Main.tile.At(k, l).Type != type || (int)Main.tile.At(k, l).FrameX != num3 || (int)Main.tile.At(k, l).FrameY != (l - num2) * 18)
                    {
                        flag = true;
                    }
                }
                if (!Main.tile.At(k, num2 + 2).Active || !Main.tileSolid[(int)Main.tile.At(k, num2 + 2).Type])
                {
                    flag = true;
                }
            }
            if (flag)
            {
                WorldGen.destroyObject = true;
                for (int m = num; m < num + 4; m++)
                {
                    for (int n = num2; n < num2 + 3; n++)
                    {
                        if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
                        {
                            WorldGen.KillTile(m, n, false, false, false);
                        }
                    }
                }
                if (type == 79)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 224, 1, false);
                }
                WorldGen.destroyObject = false;
                for (int num4 = num - 1; num4 < num + 4; num4++)
                {
                    for (int num5 = num2 - 1; num5 < num2 + 4; num5++)
                    {
                        WorldGen.TileFrame(num4, num5, false, false);
                    }
                }
            }
        }
        
        public static void Check2x2(int i, int j, int type)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }
            bool flag = false;
            int num = i + (int)(Main.tile.At(i, j).FrameX / 18 * -1);
            int num2 = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
            for (int k = num; k < num + 2; k++)
            {
                for (int l = num2; l < num2 + 2; l++)
                {
                    if (!Main.tile.At(k, l).Active || (int)Main.tile.At(k, l).Type != type || (int)Main.tile.At(k, l).FrameX != (k - num) * 18 || (int)Main.tile.At(k, l).FrameY != (l - num2) * 18)
                    {
                        flag = true;
                    }
                }
                if (!Main.tile.At(k, num2 + 2).Active || !Main.tileSolid[(int)Main.tile.At(k, num2 + 2).Type])
                {
                    flag = true;
                }
            }
            if (flag)
            {
                WorldGen.destroyObject = true;
                for (int m = num; m < num + 2; m++)
                {
                    for (int n = num2; n < num2 + 3; n++)
                    {
                        if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
                        {
                            WorldGen.KillTile(m, n, false, false, false);
                        }
                    }
                }
                if (type == 85)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 321, 1, false);
                }
                WorldGen.destroyObject = false;
                for (int num3 = num - 1; num3 < num + 3; num3++)
                {
                    for (int num4 = num2 - 1; num4 < num2 + 3; num4++)
                    {
                        WorldGen.TileFrame(num3, num4, false, false);
                    }
                }
            }
        }
        
        public static void Check3x2(int i, int j, int type)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }
            bool flag = false;
            int num = i + (int)(Main.tile.At(i, j).FrameX / 18 * -1);
            int num2 = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
            for (int k = num; k < num + 3; k++)
            {
                for (int l = num2; l < num2 + 2; l++)
                {
                    if (!Main.tile.At(k, l).Active || (int)Main.tile.At(k, l).Type != type || (int)Main.tile.At(k, l).FrameX != (k - num) * 18 || (int)Main.tile.At(k, l).FrameY != (l - num2) * 18)
                    {
                        flag = true;
                    }
                }
                if (!Main.tile.At(k, num2 + 2).Active || !Main.tileSolid[(int)Main.tile.At(k, num2 + 2).Type])
                {
                    flag = true;
                }
            }
            if (flag)
            {
                WorldGen.destroyObject = true;
                for (int m = num; m < num + 3; m++)
                {
                    for (int n = num2; n < num2 + 3; n++)
                    {
                        if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
                        {
                            WorldGen.KillTile(m, n, false, false, false);
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
                for (int num3 = num - 1; num3 < num + 4; num3++)
                {
                    for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
                    {
                        WorldGen.TileFrame(num3, num4, false, false);
                    }
                }
            }
        }
        
        public static void Place4x2(int x, int y, int type, int direction = -1)
        {
            if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
            {
                return;
            }
            bool flag = true;
            for (int i = x - 1; i < x + 3; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    if (Main.tile.At(i, j).Active)
                    {
                        flag = false;
                    }
                }
                if (!Main.tile.At(i, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(i, y + 1).Type])
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
                Main.tile.At(x - 1, y - 1).SetActive (true);
                Main.tile.At(x - 1, y - 1).SetFrameY (0);
                Main.tile.At(x - 1, y - 1).SetFrameX (num);
                Main.tile.At(x - 1, y - 1).SetType ((byte)type);
                Main.tile.At(x, y - 1).SetActive (true);
                Main.tile.At(x, y - 1).SetFrameY (0);
                Main.tile.At(x, y - 1).SetFrameX ((short)(18 + num));
                Main.tile.At(x, y - 1).SetType ((byte)type);
                Main.tile.At(x + 1, y - 1).SetActive (true);
                Main.tile.At(x + 1, y - 1).SetFrameY (0);
                Main.tile.At(x + 1, y - 1).SetFrameX ((short)(36 + num));
                Main.tile.At(x + 1, y - 1).SetType ((byte)type);
                Main.tile.At(x + 2, y - 1).SetActive (true);
                Main.tile.At(x + 2, y - 1).SetFrameY (0);
                Main.tile.At(x + 2, y - 1).SetFrameX ((short)(54 + num));
                Main.tile.At(x + 2, y - 1).SetType ((byte)type);
                Main.tile.At(x - 1, y).SetActive (true);
                Main.tile.At(x - 1, y).SetFrameY (18);
                Main.tile.At(x - 1, y).SetFrameX (num);
                Main.tile.At(x - 1, y).SetType ((byte)type);
                Main.tile.At(x, y).SetActive (true);
                Main.tile.At(x, y).SetFrameY (18);
                Main.tile.At(x, y).SetFrameX ((short)(18 + num));
                Main.tile.At(x, y).SetType ((byte)type);
                Main.tile.At(x + 1, y).SetActive (true);
                Main.tile.At(x + 1, y).SetFrameY (18);
                Main.tile.At(x + 1, y).SetFrameX ((short)(36 + num));
                Main.tile.At(x + 1, y).SetType ((byte)type);
                Main.tile.At(x + 2, y).SetActive (true);
                Main.tile.At(x + 2, y).SetFrameY (18);
                Main.tile.At(x + 2, y).SetFrameX ((short)(54 + num));
                Main.tile.At(x + 2, y).SetType ((byte)type);
            }
        }
        
        public static void Place2x2(int x, int y, int type)
        {
            if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
            {
                return;
            }
            bool flag = true;
            for (int i = x - 1; i < x + 1; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    if (Main.tile.At(i, j).Active)
                    {
                        flag = false;
                    }
                }
                if (!Main.tile.At(i, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(i, y + 1).Type])
                {
                    flag = false;
                }
            }
            if (flag)
            {
                Main.tile.At(x - 1, y - 1).SetActive (true);
                Main.tile.At(x - 1, y - 1).SetFrameY (0);
                Main.tile.At(x - 1, y - 1).SetFrameX (0);
                Main.tile.At(x - 1, y - 1).SetType ((byte)type);
                Main.tile.At(x, y - 1).SetActive (true);
                Main.tile.At(x, y - 1).SetFrameY (0);
                Main.tile.At(x, y - 1).SetFrameX (18);
                Main.tile.At(x, y - 1).SetType ((byte)type);
                Main.tile.At(x - 1, y).SetActive (true);
                Main.tile.At(x - 1, y).SetFrameY (18);
                Main.tile.At(x - 1, y).SetFrameX (0);
                Main.tile.At(x - 1, y).SetType ((byte)type);
                Main.tile.At(x, y).SetActive (true);
                Main.tile.At(x, y).SetFrameY (18);
                Main.tile.At(x, y).SetFrameX (18);
                Main.tile.At(x, y).SetType ((byte)type);
            }
        }
        
        public static void Place3x2(int x, int y, int type)
        {
            if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
            {
                return;
            }
            bool flag = true;
            for (int i = x - 1; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    if (Main.tile.At(i, j).Active)
                    {
                        flag = false;
                    }
                }
                if (!Main.tile.At(i, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(i, y + 1).Type])
                {
                    flag = false;
                }
            }
            if (flag)
            {
                Main.tile.At(x - 1, y - 1).SetActive (true);
                Main.tile.At(x - 1, y - 1).SetFrameY (0);
                Main.tile.At(x - 1, y - 1).SetFrameX (0);
                Main.tile.At(x - 1, y - 1).SetType ((byte)type);
                Main.tile.At(x, y - 1).SetActive (true);
                Main.tile.At(x, y - 1).SetFrameY (0);
                Main.tile.At(x, y - 1).SetFrameX (18);
                Main.tile.At(x, y - 1).SetType ((byte)type);
                Main.tile.At(x + 1, y - 1).SetActive (true);
                Main.tile.At(x + 1, y - 1).SetFrameY (0);
                Main.tile.At(x + 1, y - 1).SetFrameX (36);
                Main.tile.At(x + 1, y - 1).SetType ((byte)type);
                Main.tile.At(x - 1, y).SetActive (true);
                Main.tile.At(x - 1, y).SetFrameY (18);
                Main.tile.At(x - 1, y).SetFrameX (0);
                Main.tile.At(x - 1, y).SetType ((byte)type);
                Main.tile.At(x, y).SetActive (true);
                Main.tile.At(x, y).SetFrameY (18);
                Main.tile.At(x, y).SetFrameX (18);
                Main.tile.At(x, y).SetType ((byte)type);
                Main.tile.At(x + 1, y).SetActive (true);
                Main.tile.At(x + 1, y).SetFrameY (18);
                Main.tile.At(x + 1, y).SetFrameX (36);
                Main.tile.At(x + 1, y).SetType ((byte)type);
            }
        }
        
        public static void Check3x3(int i, int j, int type)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }
            bool flag = false;
            int num = i + (int)(Main.tile.At(i, j).FrameX / 18 * -1);
            int num2 = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
            for (int k = num; k < num + 3; k++)
            {
                for (int l = num2; l < num2 + 3; l++)
                {
                    if (!Main.tile.At(k, l).Active || (int)Main.tile.At(k, l).Type != type || (int)Main.tile.At(k, l).FrameX != (k - num) * 18 || (int)Main.tile.At(k, l).FrameY != (l - num2) * 18)
                    {
                        flag = true;
                    }
                }
            }
            if (!Main.tile.At(num + 1, num2 - 1).Active || !Main.tileSolid[(int)Main.tile.At(num + 1, num2 - 1).Type] || Main.tileSolidTop[(int)Main.tile.At(num + 1, num2 - 1).Type])
            {
                flag = true;
            }
            if (flag)
            {
                WorldGen.destroyObject = true;
                for (int m = num; m < num + 3; m++)
                {
                    for (int n = num2; n < num2 + 3; n++)
                    {
                        if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
                        {
                            WorldGen.KillTile(m, n, false, false, false);
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
                for (int num3 = num - 1; num3 < num + 4; num3++)
                {
                    for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
                    {
                        WorldGen.TileFrame(num3, num4, false, false);
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
                    if (Main.tile.At(i, j).Active)
                    {
                        flag = false;
                    }
                }
            }
            if (!Main.tile.At(x, y - 1).Active || !Main.tileSolid[(int)Main.tile.At(x, y - 1).Type] || Main.tileSolidTop[(int)Main.tile.At(x, y - 1).Type])
            {
                flag = false;
            }
            if (flag)
            {
                Main.tile.At(x - 1, y).SetActive (true);
                Main.tile.At(x - 1, y).SetFrameY (0);
                Main.tile.At(x - 1, y).SetFrameX (0);
                Main.tile.At(x - 1, y).SetType ((byte)type);
                Main.tile.At(x, y).SetActive (true);
                Main.tile.At(x, y).SetFrameY (0);
                Main.tile.At(x, y).SetFrameX (18);
                Main.tile.At(x, y).SetType ((byte)type);
                Main.tile.At(x + 1, y).SetActive (true);
                Main.tile.At(x + 1, y).SetFrameY (0);
                Main.tile.At(x + 1, y).SetFrameX (36);
                Main.tile.At(x + 1, y).SetType ((byte)type);
                Main.tile.At(x - 1, y + 1).SetActive (true);
                Main.tile.At(x - 1, y + 1).SetFrameY (18);
                Main.tile.At(x - 1, y + 1).SetFrameX (0);
                Main.tile.At(x - 1, y + 1).SetType ((byte)type);
                Main.tile.At(x, y + 1).SetActive (true);
                Main.tile.At(x, y + 1).SetFrameY (18);
                Main.tile.At(x, y + 1).SetFrameX (18);
                Main.tile.At(x, y + 1).SetType ((byte)type);
                Main.tile.At(x + 1, y + 1).SetActive (true);
                Main.tile.At(x + 1, y + 1).SetFrameY (18);
                Main.tile.At(x + 1, y + 1).SetFrameX (36);
                Main.tile.At(x + 1, y + 1).SetType ((byte)type);
                Main.tile.At(x - 1, y + 2).SetActive (true);
                Main.tile.At(x - 1, y + 2).SetFrameY (36);
                Main.tile.At(x - 1, y + 2).SetFrameX (0);
                Main.tile.At(x - 1, y + 2).SetType ((byte)type);
                Main.tile.At(x, y + 2).SetActive (true);
                Main.tile.At(x, y + 2).SetFrameY (36);
                Main.tile.At(x, y + 2).SetFrameX (18);
                Main.tile.At(x, y + 2).SetType ((byte)type);
                Main.tile.At(x + 1, y + 2).SetActive (true);
                Main.tile.At(x + 1, y + 2).SetFrameY (36);
                Main.tile.At(x + 1, y + 2).SetFrameX (36);
                Main.tile.At(x + 1, y + 2).SetType ((byte)type);
            }
        }
        
        public static void PlaceSunflower(int x, int y, int type = 27)
        {
            if ((double)y > Main.worldSurface - 1.0)
            {
                return;
            }
            bool flag = true;
            for (int i = x; i < x + 2; i++)
            {
                for (int j = y - 3; j < y + 1; j++)
                {
                    if (Main.tile.At(i, j).Active || Main.tile.At(i, j).Wall > 0)
                    {
                        flag = false;
                    }
                }
                if (!Main.tile.At(i, y + 1).Active || Main.tile.At(i, y + 1).Type != 2)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                for (int k = 0; k < 2; k++)
                {
                    for (int l = -3; l < 1; l++)
                    {
                        int num = k * 18 + WorldGen.genRand.Next(3) * 36;
                        int num2 = (l + 3) * 18;
                        Main.tile.At(x + k, y + l).SetActive (true);
                        Main.tile.At(x + k, y + l).SetFrameX ((short)num);
                        Main.tile.At(x + k, y + l).SetFrameY ((short)num2);
                        Main.tile.At(x + k, y + l).SetType ((byte)type);
                    }
                }
            }
        }
        
        public static void CheckSunflower(int i, int j, int type = 27)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }
            bool flag = false;
            int k = 0;
            k += (int)(Main.tile.At(i, j).FrameX / 18);
            int num = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
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
                    int n;
                    for (n = (int)(Main.tile.At(l, m).FrameX / 18); n > 1; n -= 2)
                    {
                    }
                    if (!Main.tile.At(l, m).Active || (int)Main.tile.At(l, m).Type != type || n != l - k || (int)Main.tile.At(l, m).FrameY != (m - num) * 18)
                    {
                        flag = true;
                    }
                }
                if (!Main.tile.At(l, num + 4).Active || Main.tile.At(l, num + 4).Type != 2)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                WorldGen.destroyObject = true;
                for (int num2 = k; num2 < k + 2; num2++)
                {
                    for (int num3 = num; num3 < num + 4; num3++)
                    {
                        if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
                        {
                            WorldGen.KillTile(num2, num3, false, false, false);
                        }
                    }
                }
                Item.NewItem(i * 16, j * 16, 32, 32, 63, 1, false);
                WorldGen.destroyObject = false;
            }
        }
        
        public static bool PlacePot(int x, int y, int type = 28)
        {
            bool flag = true;
            for (int i = x; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    if (Main.tile.At(i, j).Active)
                    {
                        flag = false;
                    }
                }
                if (!Main.tile.At(i, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(i, y + 1).Type])
                {
                    flag = false;
                }
            }
            if (flag)
            {
                for (int k = 0; k < 2; k++)
                {
                    for (int l = -1; l < 1; l++)
                    {
                        int num = k * 18 + WorldGen.genRand.Next(3) * 36;
                        int num2 = (l + 1) * 18;
                        Main.tile.At(x + k, y + l).SetActive (true);
                        Main.tile.At(x + k, y + l).SetFrameX ((short)num);
                        Main.tile.At(x + k, y + l).SetFrameY ((short)num2);
                        Main.tile.At(x + k, y + l).SetType ((byte)type);
                    }
                }
                return true;
            }
            return false;
        }
        
        public static bool CheckCactus(int i, int j)
        {
            int num = j;
            int num2 = i;
            while (Main.tile.At(num2, num).Active && Main.tile.At(num2, num).Type == 80)
            {
                num++;
                if (!Main.tile.At(num2, num).Active || Main.tile.At(num2, num).Type != 80)
                {
                    if (Main.tile.At(num2 - 1, num).Active && Main.tile.At(num2 - 1, num).Type == 80 && Main.tile.At(num2 - 1, num - 1).Active && Main.tile.At(num2 - 1, num - 1).Type == 80 && num2 >= i)
                    {
                        num2--;
                    }
                    if (Main.tile.At(num2 + 1, num).Active && Main.tile.At(num2 + 1, num).Type == 80 && Main.tile.At(num2 + 1, num - 1).Active && Main.tile.At(num2 + 1, num - 1).Type == 80 && num2 <= i)
                    {
                        num2++;
                    }
                }
            }
            if (!Main.tile.At(num2, num).Active || Main.tile.At(num2, num).Type != 53)
            {
                WorldGen.KillTile(i, j, false, false, false);
                return true;
            }
            if (i != num2)
            {
                if ((!Main.tile.At(i, j + 1).Active || Main.tile.At(i, j + 1).Type != 80) && (!Main.tile.At(i - 1, j).Active || Main.tile.At(i - 1, j).Type != 80) && (!Main.tile.At(i + 1, j).Active || Main.tile.At(i + 1, j).Type != 80))
                {
                    WorldGen.KillTile(i, j, false, false, false);
                    return true;
                }
            }
            else
            {
                if (i == num2 && (!Main.tile.At(i, j + 1).Active || (Main.tile.At(i, j + 1).Type != 80 && Main.tile.At(i, j + 1).Type != 53)))
                {
                    WorldGen.KillTile(i, j, false, false, false);
                    return true;
                }
            }
            return false;
        }
        
        public static void PlantCactus(int i, int j)
        {
            WorldGen.GrowCactus(i, j);
            for (int k = 0; k < 150; k++)
            {
                int i2 = WorldGen.genRand.Next(i - 1, i + 2);
                int j2 = WorldGen.genRand.Next(j - 10, j + 2);
                WorldGen.GrowCactus(i2, j2);
            }
        }
        
        public static void CactusFrame(int i, int j)
        {
            try
            {
                int num = j;
                int num2 = i;
                if (!WorldGen.CheckCactus(i, j))
                {
                    while (Main.tile.At(num2, num).Active && Main.tile.At(num2, num).Type == 80)
                    {
                        num++;
                        if (!Main.tile.At(num2, num).Active || Main.tile.At(num2, num).Type != 80)
                        {
                            if (Main.tile.At(num2 - 1, num).Active && Main.tile.At(num2 - 1, num).Type == 80 && Main.tile.At(num2 - 1, num - 1).Active && Main.tile.At(num2 - 1, num - 1).Type == 80 && num2 >= i)
                            {
                                num2--;
                            }
                            if (Main.tile.At(num2 + 1, num).Active && Main.tile.At(num2 + 1, num).Type == 80 && Main.tile.At(num2 + 1, num - 1).Active && Main.tile.At(num2 + 1, num - 1).Type == 80 && num2 <= i)
                            {
                                num2++;
                            }
                        }
                    }
                    num--;
                    int num3 = i - num2;
                    num2 = i;
                    num = j;
                    int type = (int)Main.tile.At(i - 2, j).Type;
                    int num4 = (int)Main.tile.At(i - 1, j).Type;
                    int num5 = (int)Main.tile.At(i + 1, j).Type;
                    int num6 = (int)Main.tile.At(i, j - 1).Type;
                    int num7 = (int)Main.tile.At(i, j + 1).Type;
                    int num8 = (int)Main.tile.At(i - 1, j + 1).Type;
                    int num9 = (int)Main.tile.At(i + 1, j + 1).Type;
                    if (!Main.tile.At(i - 1, j).Active)
                    {
                        num4 = -1;
                    }
                    if (!Main.tile.At(i + 1, j).Active)
                    {
                        num5 = -1;
                    }
                    if (!Main.tile.At(i, j - 1).Active)
                    {
                        num6 = -1;
                    }
                    if (!Main.tile.At(i, j + 1).Active)
                    {
                        num7 = -1;
                    }
                    if (!Main.tile.At(i - 1, j + 1).Active)
                    {
                        num8 = -1;
                    }
                    if (!Main.tile.At(i + 1, j + 1).Active)
                    {
                        num9 = -1;
                    }
                    short num10 = Main.tile.At(i, j).FrameX;
                    short num11 = Main.tile.At(i, j).FrameY;
                    if (num3 == 0)
                    {
                        if (num6 != 80)
                        {
                            if (num4 == 80 && num5 == 80 && num8 != 80 && num9 != 80 && type != 80)
                            {
                                num10 = 90;
                                num11 = 0;
                            }
                            else
                            {
                                if (num4 == 80 && num8 != 80 && type != 80)
                                {
                                    num10 = 72;
                                    num11 = 0;
                                }
                                else
                                {
                                    if (num5 == 80 && num9 != 80)
                                    {
                                        num10 = 18;
                                        num11 = 0;
                                    }
                                    else
                                    {
                                        num10 = 0;
                                        num11 = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (num4 == 80 && num5 == 80 && num8 != 80 && num9 != 80 && type != 80)
                            {
                                num10 = 90;
                                num11 = 36;
                            }
                            else
                            {
                                if (num4 == 80 && num8 != 80 && type != 80)
                                {
                                    num10 = 72;
                                    num11 = 36;
                                }
                                else
                                {
                                    if (num5 == 80 && num9 != 80)
                                    {
                                        num10 = 18;
                                        num11 = 36;
                                    }
                                    else
                                    {
                                        if (num7 >= 0 && Main.tileSolid[num7])
                                        {
                                            num10 = 0;
                                            num11 = 36;
                                        }
                                        else
                                        {
                                            num10 = 0;
                                            num11 = 18;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (num3 == -1)
                        {
                            if (num5 == 80)
                            {
                                if (num6 != 80 && num7 != 80)
                                {
                                    num10 = 108;
                                    num11 = 36;
                                }
                                else
                                {
                                    if (num7 != 80)
                                    {
                                        num10 = 54;
                                        num11 = 36;
                                    }
                                    else
                                    {
                                        if (num6 != 80)
                                        {
                                            num10 = 54;
                                            num11 = 0;
                                        }
                                        else
                                        {
                                            num10 = 54;
                                            num11 = 18;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (num6 != 80)
                                {
                                    num10 = 54;
                                    num11 = 0;
                                }
                                else
                                {
                                    num10 = 54;
                                    num11 = 18;
                                }
                            }
                        }
                        else
                        {
                            if (num3 == 1)
                            {
                                if (num4 == 80)
                                {
                                    if (num6 != 80 && num7 != 80)
                                    {
                                        num10 = 108;
                                        num11 = 16;
                                    }
                                    else
                                    {
                                        if (num7 != 80)
                                        {
                                            num10 = 36;
                                            num11 = 36;
                                        }
                                        else
                                        {
                                            if (num6 != 80)
                                            {
                                                num10 = 36;
                                                num11 = 0;
                                            }
                                            else
                                            {
                                                num10 = 36;
                                                num11 = 18;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (num6 != 80)
                                    {
                                        num10 = 36;
                                        num11 = 0;
                                    }
                                    else
                                    {
                                        num10 = 36;
                                        num11 = 18;
                                    }
                                }
                            }
                        }
                    }
                    if (num10 != Main.tile.At(i, j).FrameX || num11 != Main.tile.At(i, j).FrameY)
                    {
                        Main.tile.At(i, j).SetFrameX (num10);
                        Main.tile.At(i, j).SetFrameY (num11);
                        WorldGen.SquareTileFrame(i, j, true);
                    }
                }
            }
            catch
            {
                Main.tile.At(i, j).SetFrameX (0);
                Main.tile.At(i, j).SetFrameY (0);
            }
        }
        
        public static void GrowCactus(int i, int j)
        {
            int num = j;
            int num2 = i;
            if (!Main.tile.At(i, j).Active)
            {
                return;
            }
            if (Main.tile.At(i, j - 1).Liquid > 0)
            {
                return;
            }
            if (Main.tile.At(i, j).Type != 53 && Main.tile.At(i, j).Type != 80)
            {
                return;
            }
            if (Main.tile.At(i, j).Type == 53)
            {
                if (Main.tile.At(i, j - 1).Active || Main.tile.At(i - 1, j - 1).Active || Main.tile.At(i + 1, j - 1).Active)
                {
                    return;
                }
                int num3 = 0;
                int num4 = 0;
                for (int k = i - 6; k <= i + 6; k++)
                {
                    for (int l = j - 3; l <= j + 1; l++)
                    {
                        try
                        {
                            if (Main.tile.At(k, l).Active)
                            {
                                if (Main.tile.At(k, l).Type == 80)
                                {
                                    num3++;
                                    if (num3 >= 4)
                                    {
                                        return;
                                    }
                                }
                                if (Main.tile.At(k, l).Type == 53)
                                {
                                    num4++;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                if (num4 > 10)
                {
                    Main.tile.At(i, j - 1).SetActive (true);
                    Main.tile.At(i, j - 1).SetType (80);
                    
                    NetMessage.SendTileSquare(-1, i, j - 1, 1);
                    WorldGen.SquareTileFrame(num2, num - 1, true);
                    return;
                }
                return;
            }
            else
            {
                if (Main.tile.At(i, j).Type != 80)
                {
                    return;
                }
                while (Main.tile.At(num2, num).Active && Main.tile.At(num2, num).Type == 80)
                {
                    num++;
                    if (!Main.tile.At(num2, num).Active || Main.tile.At(num2, num).Type != 80)
                    {
                        if (Main.tile.At(num2 - 1, num).Active && Main.tile.At(num2 - 1, num).Type == 80 && Main.tile.At(num2 - 1, num - 1).Active && Main.tile.At(num2 - 1, num - 1).Type == 80 && num2 >= i)
                        {
                            num2--;
                        }
                        if (Main.tile.At(num2 + 1, num).Active && Main.tile.At(num2 + 1, num).Type == 80 && Main.tile.At(num2 + 1, num - 1).Active && Main.tile.At(num2 + 1, num - 1).Type == 80 && num2 <= i)
                        {
                            num2++;
                        }
                    }
                }
                num--;
                int num5 = num - j;
                int num6 = i - num2;
                num2 = i - num6;
                num = j;
                int num7 = 11 - num5;
                int num8 = 0;
                for (int m = num2 - 2; m <= num2 + 2; m++)
                {
                    for (int n = num - num7; n <= num + num5; n++)
                    {
                        if (Main.tile.At(m, n).Active && Main.tile.At(m, n).Type == 80)
                        {
                            num8++;
                        }
                    }
                }
                if (num8 < WorldGen.genRand.Next(11, 13))
                {
                    num2 = i;
                    num = j;
                    if (num6 == 0)
                    {
                        if (num5 == 0)
                        {
                            if (Main.tile.At(num2, num - 1).Active)
                            {
                                return;
                            }
                            Main.tile.At(num2, num - 1).SetActive (true);
                            Main.tile.At(num2, num - 1).SetType (80);
                            WorldGen.SquareTileFrame(num2, num - 1, true);
                            
                            NetMessage.SendTileSquare(-1, num2, num - 1, 1);
                            return;
                        }
                        else
                        {
                            bool flag = false;
                            bool flag2 = false;
                            if (Main.tile.At(num2, num - 1).Active && Main.tile.At(num2, num - 1).Type == 80)
                            {
                                if (!Main.tile.At(num2 - 1, num).Active && !Main.tile.At(num2 - 2, num + 1).Active && !Main.tile.At(num2 - 1, num - 1).Active && !Main.tile.At(num2 - 1, num + 1).Active && !Main.tile.At(num2 - 2, num).Active)
                                {
                                    flag = true;
                                }
                                if (!Main.tile.At(num2 + 1, num).Active && !Main.tile.At(num2 + 2, num + 1).Active && !Main.tile.At(num2 + 1, num - 1).Active && !Main.tile.At(num2 + 1, num + 1).Active && !Main.tile.At(num2 + 2, num).Active)
                                {
                                    flag2 = true;
                                }
                            }
                            int num9 = WorldGen.genRand.Next(3);
                            if (num9 == 0 && flag)
                            {
                                Main.tile.At(num2 - 1, num).SetActive (true);
                                Main.tile.At(num2 - 1, num).SetType (80);
                                WorldGen.SquareTileFrame(num2 - 1, num, true);
                                NetMessage.SendTileSquare(-1, num2 - 1, num, 1);
                                return;
                            }
                            else
                            {
                                if (num9 == 1 && flag2)
                                {
                                    Main.tile.At(num2 + 1, num).SetActive (true);
                                    Main.tile.At(num2 + 1, num).SetType (80);
                                    WorldGen.SquareTileFrame(num2 + 1, num, true);
                                    NetMessage.SendTileSquare(-1, num2 + 1, num, 1);

                                    return;
                                }
                                else
                                {
                                    if (num5 >= WorldGen.genRand.Next(2, 8))
                                    {
                                        return;
                                    }
                                    if (Main.tile.At(num2 - 1, num - 1).Active)
                                    {
                                        byte arg_5E0_0 = Main.tile.At(num2 - 1, num - 1).Type;
                                    }
                                    if (Main.tile.At(num2 + 1, num - 1).Active && Main.tile.At(num2 + 1, num - 1).Type == 80)
                                    {
                                        return;
                                    }
                                    Main.tile.At(num2, num - 1).SetActive (true);
                                    Main.tile.At(num2, num - 1).SetType (80);
                                    WorldGen.SquareTileFrame(num2, num - 1, true);
                                    NetMessage.SendTileSquare(-1, num2, num - 1, 1);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Main.tile.At(num2, num - 1).Active || Main.tile.At(num2, num - 2).Active || Main.tile.At(num2 + num6, num - 1).Active || !Main.tile.At(num2 - num6, num - 1).Active || Main.tile.At(num2 - num6, num - 1).Type != 80)
                        {
                            return;
                        }
                        Main.tile.At(num2, num - 1).SetActive (true);
                        Main.tile.At(num2, num - 1).SetType (80);
                        WorldGen.SquareTileFrame(num2, num - 1, true);
                        
                        NetMessage.SendTileSquare(-1, num2, num - 1, 1);
                        return;
                    }
                }
            }
        }
        
        public static void CheckPot(int i, int j, int type = 28)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }
            bool flag = false;
            int k = 0;
            k += (int)(Main.tile.At(i, j).FrameX / 18);
            int num = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
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
                    int n;
                    for (n = (int)(Main.tile.At(l, m).FrameX / 18); n > 1; n -= 2)
                    {
                    }
                    if (!Main.tile.At(l, m).Active || (int)Main.tile.At(l, m).Type != type || n != l - k || (int)Main.tile.At(l, m).FrameY != (m - num) * 18)
                    {
                        flag = true;
                    }
                }
                if (!Main.tile.At(l, num + 2).Active || !Main.tileSolid[(int)Main.tile.At(l, num + 2).Type])
                {
                    flag = true;
                }
            }
            if (flag)
            {
                WorldGen.destroyObject = true;
                for (int num2 = k; num2 < k + 2; num2++)
                {
                    for (int num3 = num; num3 < num + 2; num3++)
                    {
                        if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
                        {
                            WorldGen.KillTile(num2, num3, false, false, false);
                        }
                    }
                }
                if (WorldGen.genRand.Next(50) == 0)
                {
                    if ((double)j < Main.worldSurface)
                    {
                        int num4 = WorldGen.genRand.Next(4);
                        if (num4 == 0)
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, 292, 1, false);
                        }
                        if (num4 == 1)
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, 298, 1, false);
                        }
                        if (num4 == 2)
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, 299, 1, false);
                        }
                        if (num4 == 3)
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, 290, 1, false);
                        }
                    }
                    else
                    {
                        if ((double)j < Main.rockLayer)
                        {
                            int num5 = WorldGen.genRand.Next(7);
                            if (num5 == 0)
                            {
                                Item.NewItem(i * 16, j * 16, 16, 16, 289, 1, false);
                            }
                            if (num5 == 1)
                            {
                                Item.NewItem(i * 16, j * 16, 16, 16, 298, 1, false);
                            }
                            if (num5 == 2)
                            {
                                Item.NewItem(i * 16, j * 16, 16, 16, 299, 1, false);
                            }
                            if (num5 == 3)
                            {
                                Item.NewItem(i * 16, j * 16, 16, 16, 290, 1, false);
                            }
                            if (num5 == 4)
                            {
                                Item.NewItem(i * 16, j * 16, 16, 16, 303, 1, false);
                            }
                            if (num5 == 5)
                            {
                                Item.NewItem(i * 16, j * 16, 16, 16, 291, 1, false);
                            }
                            if (num5 == 6)
                            {
                                Item.NewItem(i * 16, j * 16, 16, 16, 304, 1, false);
                            }
                        }
                        else
                        {
                            if (j < Main.maxTilesY - 200)
                            {
                                int num6 = WorldGen.genRand.Next(10);
                                if (num6 == 0)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 296, 1, false);
                                }
                                if (num6 == 1)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 295, 1, false);
                                }
                                if (num6 == 2)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 299, 1, false);
                                }
                                if (num6 == 3)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 302, 1, false);
                                }
                                if (num6 == 4)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 303, 1, false);
                                }
                                if (num6 == 5)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 305, 1, false);
                                }
                                if (num6 == 6)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 301, 1, false);
                                }
                                if (num6 == 7)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 302, 1, false);
                                }
                                if (num6 == 8)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 297, 1, false);
                                }
                                if (num6 == 9)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 304, 1, false);
                                }
                            }
                            else
                            {
                                int num7 = WorldGen.genRand.Next(12);
                                if (num7 == 0)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 296, 1, false);
                                }
                                if (num7 == 1)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 295, 1, false);
                                }
                                if (num7 == 2)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 293, 1, false);
                                }
                                if (num7 == 3)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 288, 1, false);
                                }
                                if (num7 == 4)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 294, 1, false);
                                }
                                if (num7 == 5)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 297, 1, false);
                                }
                                if (num7 == 6)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 304, 1, false);
                                }
                                if (num7 == 7)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 305, 1, false);
                                }
                                if (num7 == 8)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 301, 1, false);
                                }
                                if (num7 == 9)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 302, 1, false);
                                }
                                if (num7 == 10)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 288, 1, false);
                                }
                                if (num7 == 11)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 300, 1, false);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int num8 = Main.rand.Next(10);
                    if (num8 == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLife < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLifeMax)
                    {
                        Item.NewItem(i * 16, j * 16, 16, 16, 58, 1, false);
                    }
                    else
                    {
                        if (num8 == 1 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statMana < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statManaMax)
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, 184, 1, false);
                        }
                        else
                        {
                            if (num8 == 2)
                            {
                                int stack = Main.rand.Next(3) + 1;
                                if (Main.tile.At(i, j).Liquid > 0)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 282, stack, false);
                                }
                                else
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 8, stack, false);
                                }
                            }
                            else
                            {
                                if (num8 == 3)
                                {
                                    int stack2 = Main.rand.Next(8) + 3;
                                    int type2 = 40;
                                    if (j > Main.maxTilesY - 200)
                                    {
                                        type2 = 265;
                                    }
                                    Item.NewItem(i * 16, j * 16, 16, 16, type2, stack2, false);
                                }
                                else
                                {
                                    if (num8 == 4)
                                    {
                                        int type3 = 28;
                                        if (j > Main.maxTilesY - 200)
                                        {
                                            type3 = 188;
                                        }
                                        Item.NewItem(i * 16, j * 16, 16, 16, type3, 1, false);
                                    }
                                    else
                                    {
                                        if (num8 == 5 && (double)j > Main.rockLayer)
                                        {
                                            int stack3 = Main.rand.Next(4) + 1;
                                            Item.NewItem(i * 16, j * 16, 16, 16, 166, stack3, false);
                                        }
                                        else
                                        {
                                            float num9 = (float)(200 + WorldGen.genRand.Next(-100, 101));
                                            if ((double)j < Main.worldSurface)
                                            {
                                                num9 *= 0.5f;
                                            }
                                            else
                                            {
                                                if ((double)j < Main.rockLayer)
                                                {
                                                    num9 *= 0.75f;
                                                }
                                                else
                                                {
                                                    if (j > Main.maxTilesY - 250)
                                                    {
                                                        num9 *= 1.25f;
                                                    }
                                                }
                                            }
                                            num9 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
                                            if (Main.rand.Next(5) == 0)
                                            {
                                                num9 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
                                            }
                                            if (Main.rand.Next(10) == 0)
                                            {
                                                num9 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
                                            }
                                            if (Main.rand.Next(15) == 0)
                                            {
                                                num9 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
                                            }
                                            if (Main.rand.Next(20) == 0)
                                            {
                                                num9 *= 1f + (float)Main.rand.Next(40, 81) * 0.01f;
                                            }
                                            if (Main.rand.Next(25) == 0)
                                            {
                                                num9 *= 1f + (float)Main.rand.Next(50, 101) * 0.01f;
                                            }
                                            while ((int)num9 > 0)
                                            {
                                                if (num9 > 1000000f)
                                                {
                                                    int num10 = (int)(num9 / 1000000f);
                                                    if (num10 > 50 && Main.rand.Next(2) == 0)
                                                    {
                                                        num10 /= Main.rand.Next(3) + 1;
                                                    }
                                                    if (Main.rand.Next(2) == 0)
                                                    {
                                                        num10 /= Main.rand.Next(3) + 1;
                                                    }
                                                    num9 -= (float)(1000000 * num10);
                                                    Item.NewItem(i * 16, j * 16, 16, 16, 74, num10, false);
                                                }
                                                else
                                                {
                                                    if (num9 > 10000f)
                                                    {
                                                        int num11 = (int)(num9 / 10000f);
                                                        if (num11 > 50 && Main.rand.Next(2) == 0)
                                                        {
                                                            num11 /= Main.rand.Next(3) + 1;
                                                        }
                                                        if (Main.rand.Next(2) == 0)
                                                        {
                                                            num11 /= Main.rand.Next(3) + 1;
                                                        }
                                                        num9 -= (float)(10000 * num11);
                                                        Item.NewItem(i * 16, j * 16, 16, 16, 73, num11, false);
                                                    }
                                                    else
                                                    {
                                                        if (num9 > 100f)
                                                        {
                                                            int num12 = (int)(num9 / 100f);
                                                            if (num12 > 50 && Main.rand.Next(2) == 0)
                                                            {
                                                                num12 /= Main.rand.Next(3) + 1;
                                                            }
                                                            if (Main.rand.Next(2) == 0)
                                                            {
                                                                num12 /= Main.rand.Next(3) + 1;
                                                            }
                                                            num9 -= (float)(100 * num12);
                                                            Item.NewItem(i * 16, j * 16, 16, 16, 72, num12, false);
                                                        }
                                                        else
                                                        {
                                                            int num13 = (int)num9;
                                                            if (num13 > 50 && Main.rand.Next(2) == 0)
                                                            {
                                                                num13 /= Main.rand.Next(3) + 1;
                                                            }
                                                            if (Main.rand.Next(2) == 0)
                                                            {
                                                                num13 /= Main.rand.Next(4) + 1;
                                                            }
                                                            if (num13 < 1)
                                                            {
                                                                num13 = 1;
                                                            }
                                                            num9 -= (float)num13;
                                                            Item.NewItem(i * 16, j * 16, 16, 16, 71, num13, false);
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
                WorldGen.destroyObject = false;
            }
        }
        
        public static int PlaceChest(int x, int y, int type = 21, bool notNearOtherChests = false, int style = 0)
        {
            bool flag = true;
            int num = -1;
            for (int modX = x; modX < x + 2; modX++)
            {
                for (int modY = y - 1; modY < y + 1; modY++)
                {
                    TileRef tile = GetTile(modX, modY);
                    if (tile.Active || tile.Lava)
                    {
                        flag = false;
                    }
                }

                if (!Main.tile.At(modX, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(modX, y + 1).Type])
                {
                    flag = false;
                }
            }
            if (flag && notNearOtherChests)
            {
                for (int k = x - 30; k < x + 30; k++)
                {
                    for (int l = y - 10; l < y + 10; l++)
                    {
                        try
                        {
                            if (Main.tile.At(k, l).Active && Main.tile.At(k, l).Type == 21)
                            {
                                flag = false;
                                return -1;
                            }
                        }
                        catch
                        {
                        }
                    }
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
                Main.tile.At(x, y - 1).SetActive (true);
                Main.tile.At(x, y - 1).SetFrameY (0);
                Main.tile.At(x, y - 1).SetFrameX ((short)(36 * style));
                Main.tile.At(x, y - 1).SetType ((byte)type);
                Main.tile.At(x + 1, y - 1).SetActive (true);
                Main.tile.At(x + 1, y - 1).SetFrameY (0);
                Main.tile.At(x + 1, y - 1).SetFrameX ((short)(18 + 36 * style));
                Main.tile.At(x + 1, y - 1).SetType ((byte)type);
                Main.tile.At(x, y).SetActive (true);
                Main.tile.At(x, y).SetFrameY (18);
                Main.tile.At(x, y).SetFrameX ((short)(36 * style));
                Main.tile.At(x, y).SetType ((byte)type);
                Main.tile.At(x + 1, y).SetActive (true);
                Main.tile.At(x + 1, y).SetFrameY (18);
                Main.tile.At(x + 1, y).SetFrameX ((short)(18 + 36 * style));
                Main.tile.At(x + 1, y).SetType ((byte)type);
            }
            return num;
        }
        
        public static void CheckChest(int i, int j, int type)
        {
            if (WorldGen.destroyObject)
            {
                return;
            }
            bool flag = false;
            int k = 0;
            k += (int)(Main.tile.At(i, j).FrameX / 18);
            int num = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
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
                    int n;
                    for (n = (int)(Main.tile.At(l, m).FrameX / 18); n > 1; n -= 2)
                    {
                    }
                    if (!Main.tile.At(l, m).Active || (int)Main.tile.At(l, m).Type != type || n != l - k || (int)Main.tile.At(l, m).FrameY != (m - num) * 18)
                    {
                        flag = true;
                    }
                }
                if (!Main.tile.At(l, num + 2).Active || !Main.tileSolid[(int)Main.tile.At(l, num + 2).Type])
                {
                    flag = true;
                }
            }
            if (flag)
            {
                int type2 = 48;
                if (Main.tile.At(i, j).FrameX >= 36)
                {
                    type2 = 306;
                }
                WorldGen.destroyObject = true;
                for (int num2 = k; num2 < k + 2; num2++)
                {
                    for (int num3 = num; num3 < num + 3; num3++)
                    {
                        if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
                        {
                            Chest.DestroyChest(num2, num3);
                            WorldGen.KillTile(num2, num3, false, false, false);
                        }
                    }
                }
                Item.NewItem(i * 16, j * 16, 32, 32, type2, 1, false);
                WorldGen.destroyObject = false;
            }
        }
        
        public static bool PlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1, int style = 0)
        {
            if (type >= 86)
            {
                return false;
            }
            bool result = false;
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
            {
                if (forced || Collision.EmptyTile(i, j, false) || !Main.tileSolid[type] || (type == 23 && Main.tile.At(i, j).Type == 0 && Main.tile.At(i, j).Active) || (type == 2 && Main.tile.At(i, j).Type == 0 && Main.tile.At(i, j).Active) || (type == 60 && Main.tile.At(i, j).Type == 59 && Main.tile.At(i, j).Active) || (type == 70 && Main.tile.At(i, j).Type == 59 && Main.tile.At(i, j).Active))
                {
                    if (type == 23 && (Main.tile.At(i, j).Type != 0 || !Main.tile.At(i, j).Active))
                    {
                        return false;
                    }
                    if (type == 2 && (Main.tile.At(i, j).Type != 0 || !Main.tile.At(i, j).Active))
                    {
                        return false;
                    }
                    if (type == 60 && (Main.tile.At(i, j).Type != 59 || !Main.tile.At(i, j).Active))
                    {
                        return false;
                    }
                    if (type == 81)
                    {
                        if (Main.tile.At(i - 1, j).Active || Main.tile.At(i + 1, j).Active || Main.tile.At(i, j - 1).Active)
                        {
                            return false;
                        }
                        if (!Main.tile.At(i, j + 1).Active || !Main.tileSolid[(int)Main.tile.At(i, j + 1).Type])
                        {
                            return false;
                        }
                    }
                    if (Main.tile.At(i, j).Liquid > 0 && (type == 3 || type == 4 || type == 20 || type == 24 || type == 27 || type == 32 || type == 51 || type == 69 || type == 72))
                    {
                        return false;
                    }
                    Main.tile.At(i, j).SetFrameY (0);
                    Main.tile.At(i, j).SetFrameX (0);
                    if (type == 3 || type == 24)
                    {
                        if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active && ((Main.tile.At(i, j + 1).Type == 2 && type == 3) || (Main.tile.At(i, j + 1).Type == 23 && type == 24) || (Main.tile.At(i, j + 1).Type == 78 && type == 3)))
                        {
                            if (type == 24 && WorldGen.genRand.Next(13) == 0)
                            {
                                Main.tile.At(i, j).SetActive (true);
                                Main.tile.At(i, j).SetType (32);
                                WorldGen.SquareTileFrame(i, j, true);
                            }
                            else
                            {
                                if (Main.tile.At(i, j + 1).Type == 78)
                                {
                                    Main.tile.At(i, j).SetActive (true);
                                    Main.tile.At(i, j).SetType ((byte)type);
                                    Main.tile.At(i, j).SetFrameX ((short)(WorldGen.genRand.Next(2) * 18 + 108));
                                }
                                else
                                {
                                    if (Main.tile.At(i, j).Wall == 0 && Main.tile.At(i, j + 1).Wall == 0)
                                    {
                                        if (WorldGen.genRand.Next(50) == 0 || (type == 24 && WorldGen.genRand.Next(40) == 0))
                                        {
                                            Main.tile.At(i, j).SetActive (true);
                                            Main.tile.At(i, j).SetType ((byte)type);
                                            Main.tile.At(i, j).SetFrameX (144);
                                        }
                                        else
                                        {
                                            if (WorldGen.genRand.Next(35) == 0)
                                            {
                                                Main.tile.At(i, j).SetActive (true);
                                                Main.tile.At(i, j).SetType ((byte)type);
                                                Main.tile.At(i, j).SetFrameX ((short)(WorldGen.genRand.Next(2) * 18 + 108));
                                            }
                                            else
                                            {
                                                Main.tile.At(i, j).SetActive (true);
                                                Main.tile.At(i, j).SetType ((byte)type);
                                                Main.tile.At(i, j).SetFrameX ((short)(WorldGen.genRand.Next(6) * 18));
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
                            if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 60)
                            {
                                if (WorldGen.genRand.Next(10) == 0 && (double)j > Main.worldSurface)
                                {
                                    Main.tile.At(i, j).SetActive (true);
                                    Main.tile.At(i, j).SetType (69);
                                    WorldGen.SquareTileFrame(i, j, true);
                                }
                                else
                                {
                                    if (WorldGen.genRand.Next(15) == 0 && (double)j > Main.worldSurface)
                                    {
                                        Main.tile.At(i, j).SetActive (true);
                                        Main.tile.At(i, j).SetType ((byte)type);
                                        Main.tile.At(i, j).SetFrameX (144);
                                    }
                                    else
                                    {
                                        if (WorldGen.genRand.Next(1000) == 0 && (double)j > Main.rockLayer)
                                        {
                                            Main.tile.At(i, j).SetActive (true);
                                            Main.tile.At(i, j).SetType ((byte)type);
                                            Main.tile.At(i, j).SetFrameX (162);
                                        }
                                        else
                                        {
                                            Main.tile.At(i, j).SetActive (true);
                                            Main.tile.At(i, j).SetType ((byte)type);
                                            if ((double)j > Main.rockLayer)
                                            {
                                                Main.tile.At(i, j).SetFrameX ((short)(WorldGen.genRand.Next(8) * 18));
                                            }
                                            else
                                            {
                                                Main.tile.At(i, j).SetFrameX ((short)(WorldGen.genRand.Next(6) * 18));
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
                                if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 70)
                                {
                                    Main.tile.At(i, j).SetActive (true);
                                    Main.tile.At(i, j).SetType ((byte)type);
                                    Main.tile.At(i, j).SetFrameX ((short)(WorldGen.genRand.Next(5) * 18));
                                }
                            }
                            else
                            {
                                if (type == 4)
                                {
                                    if ((Main.tile.At(i - 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i - 1, j).Type] || (Main.tile.At(i - 1, j).Type == 5 && Main.tile.At(i - 1, j - 1).Type == 5 && Main.tile.At(i - 1, j + 1).Type == 5))) || (Main.tile.At(i + 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i + 1, j).Type] || (Main.tile.At(i + 1, j).Type == 5 && Main.tile.At(i + 1, j - 1).Type == 5 && Main.tile.At(i + 1, j + 1).Type == 5))) || (Main.tile.At(i, j + 1).Active && Main.tileSolid[(int)Main.tile.At(i, j + 1).Type]))
                                    {
                                        Main.tile.At(i, j).SetActive (true);
                                        Main.tile.At(i, j).SetType ((byte)type);
                                        WorldGen.SquareTileFrame(i, j, true);
                                    }
                                }
                                else
                                {
                                    if (type == 10)
                                    {
                                        if (!Main.tile.At(i, j - 1).Active && !Main.tile.At(i, j - 2).Active && Main.tile.At(i, j - 3).Active && Main.tileSolid[(int)Main.tile.At(i, j - 3).Type])
                                        {
                                            WorldGen.PlaceDoor(i, j - 1, type);
                                            WorldGen.SquareTileFrame(i, j, true);
                                        }
                                        else
                                        {
                                            if (Main.tile.At(i, j + 1).Active || Main.tile.At(i, j + 2).Active || !Main.tile.At(i, j + 3).Active || !Main.tileSolid[(int)Main.tile.At(i, j + 3).Type])
                                            {
                                                return false;
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
                                                        if (Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 2)
                                                        {
                                                            WorldGen.Place1x2(i, j, type);
                                                            WorldGen.SquareTileFrame(i, j, true);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (type == 15)
                                                        {
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
                                                                        WorldGen.PlaceChest(i, j, type, false, style);
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
                                                                                    if (type == 55 || type == 85)
                                                                                    {
                                                                                        WorldGen.PlaceSign(i, j, type);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Main.tileAlch[type])
                                                                                        {
                                                                                            WorldGen.PlaceAlch(i, j, style);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (type != 85)
                                                                                            {
                                                                                                if (type == 79)
                                                                                                {
                                                                                                    int direction = 1;
                                                                                                    if (plr > -1)
                                                                                                    {
                                                                                                        direction = Main.players[plr].direction;
                                                                                                    }
                                                                                                    WorldGen.Place4x2(i, j, type, direction);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (type == 81)
                                                                                                    {
                                                                                                        Main.tile.At(i, j).SetFrameX ((short)(26 * WorldGen.genRand.Next(6)));
                                                                                                        Main.tile.At(i, j).SetActive (true);
                                                                                                        Main.tile.At(i, j).SetType ((byte)type);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Main.tile.At(i, j).SetActive (true);
                                                                                                        Main.tile.At(i, j).SetType ((byte)type);
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
                    if (Main.tile.At(i, j).Active && !mute)
                    {
                        WorldGen.SquareTileFrame(i, j, true);
                        result = true;
                    }
                }
            }
            return result;
        }
        
        public static void KillWall(int i, int j, bool fail = false)
        {
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
            {
                if (Main.tile.At(i, j).Wall > 0)
                {
                    if (fail)
                    {
                        WorldGen.SquareWallFrame(i, j, true);
                        return;
                    }
                    int num2 = 0;
                    if (Main.tile.At(i, j).Wall == 1)
                    {
                        num2 = 26;
                    }
                    if (Main.tile.At(i, j).Wall == 4)
                    {
                        num2 = 93;
                    }
                    if (Main.tile.At(i, j).Wall == 5)
                    {
                        num2 = 130;
                    }
                    if (Main.tile.At(i, j).Wall == 6)
                    {
                        num2 = 132;
                    }
                    if (Main.tile.At(i, j).Wall == 7)
                    {
                        num2 = 135;
                    }
                    if (Main.tile.At(i, j).Wall == 8)
                    {
                        num2 = 138;
                    }
                    if (Main.tile.At(i, j).Wall == 9)
                    {
                        num2 = 140;
                    }
                    if (Main.tile.At(i, j).Wall == 10)
                    {
                        num2 = 142;
                    }
                    if (Main.tile.At(i, j).Wall == 11)
                    {
                        num2 = 144;
                    }
                    if (Main.tile.At(i, j).Wall == 12)
                    {
                        num2 = 146;
                    }
                    if (num2 > 0)
                    {
                        Item.NewItem(i * 16, j * 16, 16, 16, num2, 1, false);
                    }
                    Main.tile.At(i, j).SetWall (0);
                    WorldGen.SquareWallFrame(i, j, true);
                }
            }
        }
        
        public static void KillTile(int i, int j, bool fail = false, bool effectOnly = false, bool noItem = false, Player player = null)
        {
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
            {
                if (Main.tile.At(i, j).Active)
                {
                    if (j >= 1 && Main.tile.At(i, j - 1).Active && ((Main.tile.At(i, j - 1).Type == 5 && Main.tile.At(i, j).Type != 5) || (Main.tile.At(i, j - 1).Type == 21 && Main.tile.At(i, j).Type != 21) || (Main.tile.At(i, j - 1).Type == 26 && Main.tile.At(i, j).Type != 26) || (Main.tile.At(i, j - 1).Type == 72 && Main.tile.At(i, j).Type != 72) || (Main.tile.At(i, j - 1).Type == 12 && Main.tile.At(i, j).Type != 12)) && (Main.tile.At(i, j - 1).Type != 5 || ((Main.tile.At(i, j - 1).FrameX != 66 || Main.tile.At(i, j - 1).FrameY < 0 || Main.tile.At(i, j - 1).FrameY > 44) && (Main.tile.At(i, j - 1).FrameX != 88 || Main.tile.At(i, j - 1).FrameY < 66 || Main.tile.At(i, j - 1).FrameY > 110) && Main.tile.At(i, j - 1).FrameY < 198)))
                    {
                        return;
                    }
                    if (!effectOnly && !WorldGen.stopDrops)
                    {
                        if (Main.tile.At(i, j).Type == 3)
                        {
                            if (Main.tile.At(i, j).FrameX == 144)
                            {
                                Item.NewItem(i * 16, j * 16, 16, 16, 5, 1, false);
                            }
                        }
                        else
                        {
                            if (Main.tile.At(i, j).Type == 24)
                            {
                                if (Main.tile.At(i, j).FrameX == 144)
                                {
                                    Item.NewItem(i * 16, j * 16, 16, 16, 60, 1, false);
                                }
                            }
                        }
                    }
                    if (effectOnly)
                    {
                        return;
                    }
                    if (fail)
                    {
                        if (Main.tile.At(i, j).Type == 2 || Main.tile.At(i, j).Type == 23)
                        {
                            Main.tile.At(i, j).SetType (0);
                        }
                        if (Main.tile.At(i, j).Type == 60 || Main.tile.At(i, j).Type == 70)
                        {
                            Main.tile.At(i, j).SetType (59);
                        }
                        WorldGen.SquareTileFrame(i, j, true);
                        return;
                    }
                    if (Main.tile.At(i, j).Type == 21)
                    {
                        int l = (int)(Main.tile.At(i, j).FrameX / 18);
                        int y = j - (int)(Main.tile.At(i, j).FrameY / 18);
                        while (l > 1)
                        {
                            l -= 2;
                        }
                        l = i - l;
                        if (!Chest.DestroyChest(l, y))
                        {
                            return;
                        }
                    }
                    if (!noItem && !WorldGen.stopDrops)
                    {
                        int num4 = 0;
                        if (Main.tile.At(i, j).Type == 0 || Main.tile.At(i, j).Type == 2)
                        {
                            num4 = 2;
                        }
                        else
                        {
                            if (Main.tile.At(i, j).Type == 1)
                            {
                                num4 = 3;
                            }
                            else
                            {
                                if (Main.tile.At(i, j).Type == 3 || Main.tile.At(i, j).Type == 73)
                                {
                                    if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].HasItem(281))
                                    {
                                        num4 = 283;
                                    }
                                }
                                else
                                {
                                    if (Main.tile.At(i, j).Type == 4)
                                    {
                                        num4 = 8;
                                    }
                                    else
                                    {
                                        if (Main.tile.At(i, j).Type == 5)
                                        {
                                            if (Main.tile.At(i, j).FrameX >= 22 && Main.tile.At(i, j).FrameY >= 198)
                                            {
                                                if (WorldGen.genRand.Next(2) == 0)
                                                {
                                                    int num5 = j;
                                                    while ((!Main.tile.At(i, num5).Active || !Main.tileSolid[(int)Main.tile.At(i, num5).Type] || Main.tileSolidTop[(int)Main.tile.At(i, num5).Type]))
                                                    {
                                                        num5++;
                                                    }
                                                    if (Main.tile.At(i, num5).Type == 2)
                                                    {
                                                        num4 = 27;
                                                    }
                                                    else
                                                    {
                                                        num4 = 9;
                                                    }
                                                }
                                                else
                                                {
                                                    num4 = 9;
                                                }
                                            }
                                            else
                                            {
                                                num4 = 9;
                                            }
                                        }
                                        else
                                        {
                                            if (Main.tile.At(i, j).Type == 6)
                                            {
                                                num4 = 11;
                                            }
                                            else
                                            {
                                                if (Main.tile.At(i, j).Type == 7)
                                                {
                                                    num4 = 12;
                                                }
                                                else
                                                {
                                                    if (Main.tile.At(i, j).Type == 8)
                                                    {
                                                        num4 = 13;
                                                    }
                                                    else
                                                    {
                                                        if (Main.tile.At(i, j).Type == 9)
                                                        {
                                                            num4 = 14;
                                                        }
                                                        else
                                                        {
                                                            if (Main.tile.At(i, j).Type == 13)
                                                            {
                                                                if (Main.tile.At(i, j).FrameX == 18)
                                                                {
                                                                    num4 = 28;
                                                                }
                                                                else
                                                                {
                                                                    if (Main.tile.At(i, j).FrameX == 36)
                                                                    {
                                                                        num4 = 110;
                                                                    }
                                                                    else
                                                                    {
                                                                        num4 = 31;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (Main.tile.At(i, j).Type == 19)
                                                                {
                                                                    num4 = 94;
                                                                }
                                                                else
                                                                {
                                                                    if (Main.tile.At(i, j).Type == 22)
                                                                    {
                                                                        num4 = 56;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.tile.At(i, j).Type == 23)
                                                                        {
                                                                            num4 = 2;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Main.tile.At(i, j).Type == 25)
                                                                            {
                                                                                num4 = 61;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Main.tile.At(i, j).Type == 30)
                                                                                {
                                                                                    num4 = 9;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (Main.tile.At(i, j).Type == 33)
                                                                                    {
                                                                                        num4 = 105;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Main.tile.At(i, j).Type == 37)
                                                                                        {
                                                                                            num4 = 116;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Main.tile.At(i, j).Type == 38)
                                                                                            {
                                                                                                num4 = 129;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.tile.At(i, j).Type == 39)
                                                                                                {
                                                                                                    num4 = 131;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Main.tile.At(i, j).Type == 40)
                                                                                                    {
                                                                                                        num4 = 133;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Main.tile.At(i, j).Type == 41)
                                                                                                        {
                                                                                                            num4 = 134;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Main.tile.At(i, j).Type == 43)
                                                                                                            {
                                                                                                                num4 = 137;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (Main.tile.At(i, j).Type == 44)
                                                                                                                {
                                                                                                                    num4 = 139;
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (Main.tile.At(i, j).Type == 45)
                                                                                                                    {
                                                                                                                        num4 = 141;
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (Main.tile.At(i, j).Type == 46)
                                                                                                                        {
                                                                                                                            num4 = 143;
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (Main.tile.At(i, j).Type == 47)
                                                                                                                            {
                                                                                                                                num4 = 145;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (Main.tile.At(i, j).Type == 48)
                                                                                                                                {
                                                                                                                                    num4 = 147;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (Main.tile.At(i, j).Type == 49)
                                                                                                                                    {
                                                                                                                                        num4 = 148;
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (Main.tile.At(i, j).Type == 51)
                                                                                                                                        {
                                                                                                                                            num4 = 150;
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (Main.tile.At(i, j).Type == 53)
                                                                                                                                            {
                                                                                                                                                num4 = 169;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (Main.tile.At(i, j).Type != 54)
                                                                                                                                                {
                                                                                                                                                    if (Main.tile.At(i, j).Type == 56)
                                                                                                                                                    {
                                                                                                                                                        num4 = 173;
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (Main.tile.At(i, j).Type == 57)
                                                                                                                                                        {
                                                                                                                                                            num4 = 172;
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            if (Main.tile.At(i, j).Type == 58)
                                                                                                                                                            {
                                                                                                                                                                num4 = 174;
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (Main.tile.At(i, j).Type == 60)
                                                                                                                                                                {
                                                                                                                                                                    num4 = 176;
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (Main.tile.At(i, j).Type == 70)
                                                                                                                                                                    {
                                                                                                                                                                        num4 = 176;
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        if (Main.tile.At(i, j).Type == 75)
                                                                                                                                                                        {
                                                                                                                                                                            num4 = 192;
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            if (Main.tile.At(i, j).Type == 76)
                                                                                                                                                                            {
                                                                                                                                                                                num4 = 214;
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (Main.tile.At(i, j).Type == 78)
                                                                                                                                                                                {
                                                                                                                                                                                    num4 = 222;
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (Main.tile.At(i, j).Type == 81)
                                                                                                                                                                                    {
                                                                                                                                                                                        num4 = 275;
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (Main.tile.At(i, j).Type == 80)
                                                                                                                                                                                        {
                                                                                                                                                                                            num4 = 276;
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (Main.tile.At(i, j).Type == 61 || Main.tile.At(i, j).Type == 74)
                                                                                                                                                                                            {
                                                                                                                                                                                                if (Main.tile.At(i, j).FrameX == 162)
                                                                                                                                                                                                {
                                                                                                                                                                                                    num4 = 223;
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (Main.tile.At(i, j).FrameX >= 108 && Main.tile.At(i, j).FrameX <= 126 && (double)j > Main.rockLayer)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (WorldGen.genRand.Next(2) == 0)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            num4 = 208;
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (WorldGen.genRand.Next(100) == 0)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            num4 = 195;
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (Main.tile.At(i, j).Type == 59 || Main.tile.At(i, j).Type == 60)
                                                                                                                                                                                                {
                                                                                                                                                                                                    num4 = 176;
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (Main.tile.At(i, j).Type == 71 || Main.tile.At(i, j).Type == 72)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (WorldGen.genRand.Next(50) == 0)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            num4 = 194;
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (WorldGen.genRand.Next(2) == 0)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                num4 = 183;
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (Main.tile.At(i, j).Type >= 63 && Main.tile.At(i, j).Type <= 68)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            num4 = (int)(Main.tile.At(i, j).Type - 63 + 177);
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (Main.tile.At(i, j).Type == 50)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                if (Main.tile.At(i, j).FrameX == 90)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    num4 = 165;
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    num4 = 149;
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else
                                                                                                                                                                                                            {
                                                                                                                                                                                                                if (Main.tileAlch[(int)Main.tile.At(i, j).Type] && Main.tile.At(i, j).Type > 82)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    int num6 = (int)(Main.tile.At(i, j).FrameX / 18);
                                                                                                                                                                                                                    bool flag = false;
                                                                                                                                                                                                                    if (Main.tile.At(i, j).Type == 84)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        flag = true;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    if (num6 == 0 && Main.dayTime)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        flag = true;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    if (num6 == 1 && !Main.dayTime)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        flag = true;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    if (num6 == 3 && Main.bloodMoon)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        flag = true;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    num4 = 313 + num6;
                                                                                                                                                                                                                    if (flag)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        Item.NewItem(i * 16, j * 16, 16, 16, 307 + num6, WorldGen.genRand.Next(1, 4), false);
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
                                    }
                                }
                            }
                        }
                        if (num4 > 0)
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, num4, 1, false);
                        }
                    }
                    Main.tile.At(i, j).SetActive (false);
                    if (Main.tileSolid[(int)Main.tile.At(i, j).Type])
                    {
                        Main.tile.At(i, j).SetLighted (false);
                    }
                    Main.tile.At(i, j).SetFrameX (-1);
                    Main.tile.At(i, j).SetFrameY (-1);
                    Main.tile.At(i, j).SetFrameNumber (0);
                    Main.tile.At(i, j).SetType (0);
                    WorldGen.SquareTileFrame(i, j, true);
                }
            }
        }
        
        public static bool PlayerLOS(int x, int y)
        {
            Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
            for (int i = 0; i < 255; i++)
            {
                if (Main.players[i].Active)
                {
                    Rectangle value = new Rectangle((int)((double)Main.players[i].Position.X + (double)Main.players[i].Width * 0.5 - (double)NPC.sWidth * 0.6), (int)((double)Main.players[i].Position.Y + (double)Main.players[i].Height * 0.5 - (double)NPC.sHeight * 0.6), (int)((double)NPC.sWidth * 1.2), (int)((double)NPC.sHeight * 1.2));
                    if (rectangle.Intersects(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public static void UpdateWorld()
        {
            Liquid.skipCount++;
            if (Liquid.skipCount > 1)
            {
                Liquid.UpdateLiquid();
                Liquid.skipCount = 0;
            }
            float num = 3E-05f;
            float num2 = 1.5E-05f;
            bool flag = false;
            WorldGen.spawnDelay++;
            if (Main.invasionType > 0)
            {
                WorldGen.spawnDelay = 0;
            }
            if (WorldGen.spawnDelay >= 20)
            {
                flag = true;
                WorldGen.spawnDelay = 0;
                if (WorldGen.spawnNPC != 37)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        if (Main.npcs[i].Active && Main.npcs[i].homeless && Main.npcs[i].townNPC)
                        {
                            WorldGen.spawnNPC = Main.npcs[i].Type;
                            break;
                        }
                    }
                }
            }
            int num3 = 0;

            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random();
            }

            while ((float)num3 < (float)(Main.maxTilesX * Main.maxTilesY) * num)
            {
                int num4 = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int num5 = WorldGen.genRand.Next(10, (int)Main.worldSurface - 1);
                int num6 = num4 - 1;
                int num7 = num4 + 2;
                int num8 = num5 - 1;
                int num9 = num5 + 2;
                if (num6 < 10)
                {
                    num6 = 10;
                }
                if (num7 > Main.maxTilesX - 10)
                {
                    num7 = Main.maxTilesX - 10;
                }
                if (num8 < 10)
                {
                    num8 = 10;
                }
                if (num9 > Main.maxTilesY - 10)
                {
                    num9 = Main.maxTilesY - 10;
                }
                if (true)
                {
                    if (Main.tileAlch[(int)Main.tile.At(num4, num5).Type])
                    {
                        WorldGen.GrowAlch(num4, num5);
                    }
                    if (Main.tile.At(num4, num5).Liquid > 32)
                    {
                        if (Main.tile.At(num4, num5).Active && (Main.tile.At(num4, num5).Type == 3 || Main.tile.At(num4, num5).Type == 20 || Main.tile.At(num4, num5).Type == 24 || Main.tile.At(num4, num5).Type == 27 || Main.tile.At(num4, num5).Type == 73))
                        {
                            WorldGen.KillTile(num4, num5, false, false, false);
                            NetMessage.SendData(17, -1, -1, "", 0, (float)num4, (float)num5);
                        }
                    }
                    else
                    {
                        if (Main.tile.At(num4, num5).Active)
                        {
                            if (Main.tile.At(num4, num5).Type == 80)
                            {
                                if (WorldGen.genRand.Next(15) == 0)
                                {
                                    WorldGen.GrowCactus(num4, num5);
                                }
                            }
                            else
                            {
                                if (Main.tile.At(num4, num5).Type == 53)
                                {
                                    if (!Main.tile.At(num4, num8).Active)
                                    {
                                        if (num4 < 250 || num4 > Main.maxTilesX - 250)
                                        {
                                            if (WorldGen.genRand.Next(500) == 0 && Main.tile.At(num4, num8).Liquid == 255 && Main.tile.At(num4, num8 - 1).Liquid == 255 && Main.tile.At(num4, num8 - 2).Liquid == 255 && Main.tile.At(num4, num8 - 3).Liquid == 255 && Main.tile.At(num4, num8 - 4).Liquid == 255)
                                            {
                                                WorldGen.PlaceTile(num4, num8, 81, true, false, -1, 0);
                                                if (Main.tile.At(num4, num8).Active)
                                                {
                                                    NetMessage.SendTileSquare(-1, num4, num8, 1);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (num4 > 400 && num4 < Main.maxTilesX - 400 && WorldGen.genRand.Next(300) == 0)
                                            {
                                                WorldGen.GrowCactus(num4, num5);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Main.tile.At(num4, num5).Type == 78)
                                    {
                                        if (!Main.tile.At(num4, num8).Active)
                                        {
                                            WorldGen.PlaceTile(num4, num8, 3, true, false, -1, 0);
                                            if (Main.tile.At(num4, num8).Active)
                                            {
                                                NetMessage.SendTileSquare(-1, num4, num8, 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile.At(num4, num5).Type == 2 || Main.tile.At(num4, num5).Type == 23 || Main.tile.At(num4, num5).Type == 32)
                                        {
                                            int num10 = (int)Main.tile.At(num4, num5).Type;
                                            if (!Main.tile.At(num4, num8).Active && WorldGen.genRand.Next(12) == 0 && num10 == 2)
                                            {
                                                WorldGen.PlaceTile(num4, num8, 3, true, false, -1, 0);
                                                if (Main.tile.At(num4, num8).Active)
                                                {
                                                    NetMessage.SendTileSquare(-1, num4, num8, 1);
                                                }
                                            }
                                            if (!Main.tile.At(num4, num8).Active && WorldGen.genRand.Next(10) == 0 && num10 == 23)
                                            {
                                                WorldGen.PlaceTile(num4, num8, 24, true, false, -1, 0);
                                                if (Main.tile.At(num4, num8).Active)
                                                {
                                                    NetMessage.SendTileSquare(-1, num4, num8, 1);
                                                }
                                            }
                                            bool flag2 = false;
                                            for (int j = num6; j < num7; j++)
                                            {
                                                for (int k = num8; k < num9; k++)
                                                {
                                                    if ((num4 != j || num5 != k) && Main.tile.At(j, k).Active)
                                                    {
                                                        if (num10 == 32)
                                                        {
                                                            num10 = 23;
                                                        }
                                                        if (Main.tile.At(j, k).Type == 0 || (num10 == 23 && Main.tile.At(j, k).Type == 2))
                                                        {
                                                            WorldGen.SpreadGrass(j, k, 0, num10, false);
                                                            if (num10 == 23)
                                                            {
                                                                WorldGen.SpreadGrass(j, k, 2, num10, false);
                                                            }
                                                            if ((int)Main.tile.At(j, k).Type == num10)
                                                            {
                                                                WorldGen.SquareTileFrame(j, k, true);
                                                                flag2 = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (flag2)
                                            {
                                                NetMessage.SendTileSquare(-1, num4, num5, 3);
                                            }
                                        }
                                        else
                                        {
                                            if (Main.tile.At(num4, num5).Type == 20 && WorldGen.genRand.Next(20) == 0 && !WorldGen.PlayerLOS(num4, num5))
                                            {
                                                WorldGen.GrowTree(num4, num5);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Main.tile.At(num4, num5).Type == 3 && WorldGen.genRand.Next(20) == 0 && Main.tile.At(num4, num5).FrameX < 144)
                            {
                                Main.tile.At(num4, num5).SetType (73);
                                NetMessage.SendTileSquare(-1, num4, num5, 3);
                            }
                            if (Main.tile.At(num4, num5).Type == 32 && WorldGen.genRand.Next(3) == 0)
                            {
                                int num11 = num4;
                                int num12 = num5;
                                int num13 = 0;
                                if (Main.tile.At(num11 + 1, num12).Active && Main.tile.At(num11 + 1, num12).Type == 32)
                                {
                                    num13++;
                                }
                                if (Main.tile.At(num11 - 1, num12).Active && Main.tile.At(num11 - 1, num12).Type == 32)
                                {
                                    num13++;
                                }
                                if (Main.tile.At(num11, num12 + 1).Active && Main.tile.At(num11, num12 + 1).Type == 32)
                                {
                                    num13++;
                                }
                                if (Main.tile.At(num11, num12 - 1).Active && Main.tile.At(num11, num12 - 1).Type == 32)
                                {
                                    num13++;
                                }
                                if (num13 < 3 || Main.tile.At(num4, num5).Type == 23)
                                {
                                    int num14 = WorldGen.genRand.Next(4);
                                    if (num14 == 0)
                                    {
                                        num12--;
                                    }
                                    else
                                    {
                                        if (num14 == 1)
                                        {
                                            num12++;
                                        }
                                        else
                                        {
                                            if (num14 == 2)
                                            {
                                                num11--;
                                            }
                                            else
                                            {
                                                if (num14 == 3)
                                                {
                                                    num11++;
                                                }
                                            }
                                        }
                                    }
                                    if (!Main.tile.At(num11, num12).Active)
                                    {
                                        num13 = 0;
                                        if (Main.tile.At(num11 + 1, num12).Active && Main.tile.At(num11 + 1, num12).Type == 32)
                                        {
                                            num13++;
                                        }
                                        if (Main.tile.At(num11 - 1, num12).Active && Main.tile.At(num11 - 1, num12).Type == 32)
                                        {
                                            num13++;
                                        }
                                        if (Main.tile.At(num11, num12 + 1).Active && Main.tile.At(num11, num12 + 1).Type == 32)
                                        {
                                            num13++;
                                        }
                                        if (Main.tile.At(num11, num12 - 1).Active && Main.tile.At(num11, num12 - 1).Type == 32)
                                        {
                                            num13++;
                                        }
                                        if (num13 < 2)
                                        {
                                            int num15 = 7;
                                            int num16 = num11 - num15;
                                            int num17 = num11 + num15;
                                            int num18 = num12 - num15;
                                            int num19 = num12 + num15;
                                            bool flag3 = false;
                                            for (int l = num16; l < num17; l++)
                                            {
                                                for (int m = num18; m < num19; m++)
                                                {
                                                    if (Math.Abs(l - num11) * 2 + Math.Abs(m - num12) < 9 && Main.tile.At(l, m).Active && Main.tile.At(l, m).Type == 23 && Main.tile.At(l, m - 1).Active && Main.tile.At(l, m - 1).Type == 32 && Main.tile.At(l, m - 1).Liquid == 0)
                                                    {
                                                        flag3 = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (flag3)
                                            {
                                                Main.tile.At(num11, num12).SetType (32);
                                                Main.tile.At(num11, num12).SetActive (true);
                                                WorldGen.SquareTileFrame(num11, num12, true);
                                                
                                                NetMessage.SendTileSquare(-1, num11, num12, 3);
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
                                WorldGen.SpawnNPC(num4, num5);
                            }
                        }
                    }
                    if (Main.tile.At(num4, num5).Active)
                    {
                        if ((Main.tile.At(num4, num5).Type == 2 || Main.tile.At(num4, num5).Type == 52) && WorldGen.genRand.Next(40) == 0 && !Main.tile.At(num4, num5 + 1).Active && !Main.tile.At(num4, num5 + 1).Lava)
                        {
                            bool flag4 = false;
                            for (int n = num5; n > num5 - 10; n--)
                            {
                                if (Main.tile.At(num4, n).Active && Main.tile.At(num4, n).Type == 2)
                                {
                                    flag4 = true;
                                    break;
                                }
                            }
                            if (flag4)
                            {
                                int num20 = num4;
                                int num21 = num5 + 1;
                                Main.tile.At(num20, num21).SetType (52);
                                Main.tile.At(num20, num21).SetActive (true);
                                WorldGen.SquareTileFrame(num20, num21, true);
                                NetMessage.SendTileSquare(-1, num20, num21, 3);
                            }
                        }
                        if (Main.tile.At(num4, num5).Type == 60)
                        {
                            int type = (int)Main.tile.At(num4, num5).Type;
                            if (!Main.tile.At(num4, num8).Active && WorldGen.genRand.Next(7) == 0)
                            {
                                WorldGen.PlaceTile(num4, num8, 61, true, false, -1, 0);
                                if (Main.tile.At(num4, num8).Active)
                                {
                                    NetMessage.SendTileSquare(-1, num4, num8, 1);
                                }
                            }
                            else
                            {
                                if (WorldGen.genRand.Next(500) == 0 && (!Main.tile.At(num4, num8).Active || Main.tile.At(num4, num8).Type == 61 || Main.tile.At(num4, num8).Type == 74 || Main.tile.At(num4, num8).Type == 69) && !WorldGen.PlayerLOS(num4, num5))
                                {
                                    WorldGen.GrowTree(num4, num5);
                                }
                            }
                            bool flag5 = false;
                            for (int num22 = num6; num22 < num7; num22++)
                            {
                                for (int num23 = num8; num23 < num9; num23++)
                                {
                                    if ((num4 != num22 || num5 != num23) && Main.tile.At(num22, num23).Active && Main.tile.At(num22, num23).Type == 59)
                                    {
                                        WorldGen.SpreadGrass(num22, num23, 59, type, false);
                                        if ((int)Main.tile.At(num22, num23).Type == type)
                                        {
                                            WorldGen.SquareTileFrame(num22, num23, true);
                                            flag5 = true;
                                        }
                                    }
                                }
                            }
                            if (flag5)
                            {
                                NetMessage.SendTileSquare(-1, num4, num5, 3);
                            }
                        }
                        if (Main.tile.At(num4, num5).Type == 61 && WorldGen.genRand.Next(3) == 0 && Main.tile.At(num4, num5).FrameX < 144)
                        {
                            Main.tile.At(num4, num5).SetType (74);
                            NetMessage.SendTileSquare(-1, num4, num5, 3);
                        }
                        if ((Main.tile.At(num4, num5).Type == 60 || Main.tile.At(num4, num5).Type == 62) && WorldGen.genRand.Next(15) == 0 && !Main.tile.At(num4, num5 + 1).Active && !Main.tile.At(num4, num5 + 1).Lava)
                        {
                            bool flag6 = false;
                            for (int num24 = num5; num24 > num5 - 10; num24--)
                            {
                                if (Main.tile.At(num4, num24).Active && Main.tile.At(num4, num24).Type == 60)
                                {
                                    flag6 = true;
                                    break;
                                }
                            }
                            if (flag6)
                            {
                                int num25 = num4;
                                int num26 = num5 + 1;
                                Main.tile.At(num25, num26).SetType (62);
                                Main.tile.At(num25, num26).SetActive (true);
                                WorldGen.SquareTileFrame(num25, num26, true);
                                NetMessage.SendTileSquare(-1, num25, num26, 3);
                            }
                        }
                    }
                }
                num3++;
            }
            int num27 = 0;
            while ((float)num27 < (float)(Main.maxTilesX * Main.maxTilesY) * num2)
            {
                int num28 = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int num29 = WorldGen.genRand.Next((int)Main.worldSurface + 2, Main.maxTilesY - 20);
                int num30 = num28 - 1;
                int num31 = num28 + 2;
                int num32 = num29 - 1;
                int num33 = num29 + 2;
                if (num30 < 10)
                {
                    num30 = 10;
                }
                if (num31 > Main.maxTilesX - 10)
                {
                    num31 = Main.maxTilesX - 10;
                }
                if (num32 < 10)
                {
                    num32 = 10;
                }
                if (num33 > Main.maxTilesY - 10)
                {
                    num33 = Main.maxTilesY - 10;
                }
                if (true)
                {
                    if (Main.tileAlch[(int)Main.tile.At(num28, num29).Type])
                    {
                        WorldGen.GrowAlch(num28, num29);
                    }
                    if (Main.tile.At(num28, num29).Liquid <= 32)
                    {
                        if (Main.tile.At(num28, num29).Active)
                        {
                            if (Main.tile.At(num28, num29).Type == 60)
                            {
                                int type2 = (int)Main.tile.At(num28, num29).Type;
                                if (!Main.tile.At(num28, num32).Active && WorldGen.genRand.Next(10) == 0)
                                {
                                    WorldGen.PlaceTile(num28, num32, 61, true, false, -1, 0);
                                    if (Main.tile.At(num28, num32).Active)
                                    {
                                        NetMessage.SendTileSquare(-1, num28, num32, 1);
                                    }
                                }
                                bool flag7 = false;
                                for (int num34 = num30; num34 < num31; num34++)
                                {
                                    for (int num35 = num32; num35 < num33; num35++)
                                    {
                                        if ((num28 != num34 || num29 != num35) && Main.tile.At(num34, num35).Active && Main.tile.At(num34, num35).Type == 59)
                                        {
                                            WorldGen.SpreadGrass(num34, num35, 59, type2, false);
                                            if ((int)Main.tile.At(num34, num35).Type == type2)
                                            {
                                                WorldGen.SquareTileFrame(num34, num35, true);
                                                flag7 = true;
                                            }
                                        }
                                    }
                                }
                                if (flag7)
                                {
                                    NetMessage.SendTileSquare(-1, num28, num29, 3);
                                }
                            }
                            if (Main.tile.At(num28, num29).Type == 61 && WorldGen.genRand.Next(3) == 0 && Main.tile.At(num28, num29).FrameX < 144)
                            {
                                Main.tile.At(num28, num29).SetType (74);
                                NetMessage.SendTileSquare(-1, num28, num29, 3);
                            }
                            if ((Main.tile.At(num28, num29).Type == 60 || Main.tile.At(num28, num29).Type == 62) && WorldGen.genRand.Next(5) == 0 && !Main.tile.At(num28, num29 + 1).Active && !Main.tile.At(num28, num29 + 1).Lava)
                            {
                                bool flag8 = false;
                                for (int num36 = num29; num36 > num29 - 10; num36--)
                                {
                                    if (Main.tile.At(num28, num36).Active && Main.tile.At(num28, num36).Type == 60)
                                    {
                                        flag8 = true;
                                        break;
                                    }
                                }
                                if (flag8)
                                {
                                    int num37 = num28;
                                    int num38 = num29 + 1;
                                    Main.tile.At(num37, num38).SetType (62);
                                    Main.tile.At(num37, num38).SetActive (true);
                                    WorldGen.SquareTileFrame(num37, num38, true);
                                    NetMessage.SendTileSquare(-1, num37, num38, 3);
                                }
                            }
                            if (Main.tile.At(num28, num29).Type == 69 && WorldGen.genRand.Next(3) == 0)
                            {
                                int num39 = num28;
                                int num40 = num29;
                                int num41 = 0;
                                if (Main.tile.At(num39 + 1, num40).Active && Main.tile.At(num39 + 1, num40).Type == 69)
                                {
                                    num41++;
                                }
                                if (Main.tile.At(num39 - 1, num40).Active && Main.tile.At(num39 - 1, num40).Type == 69)
                                {
                                    num41++;
                                }
                                if (Main.tile.At(num39, num40 + 1).Active && Main.tile.At(num39, num40 + 1).Type == 69)
                                {
                                    num41++;
                                }
                                if (Main.tile.At(num39, num40 - 1).Active && Main.tile.At(num39, num40 - 1).Type == 69)
                                {
                                    num41++;
                                }
                                if (num41 < 3 || Main.tile.At(num28, num29).Type == 60)
                                {
                                    int num42 = WorldGen.genRand.Next(4);
                                    if (num42 == 0)
                                    {
                                        num40--;
                                    }
                                    else
                                    {
                                        if (num42 == 1)
                                        {
                                            num40++;
                                        }
                                        else
                                        {
                                            if (num42 == 2)
                                            {
                                                num39--;
                                            }
                                            else
                                            {
                                                if (num42 == 3)
                                                {
                                                    num39++;
                                                }
                                            }
                                        }
                                    }
                                    if (!Main.tile.At(num39, num40).Active)
                                    {
                                        num41 = 0;
                                        if (Main.tile.At(num39 + 1, num40).Active && Main.tile.At(num39 + 1, num40).Type == 69)
                                        {
                                            num41++;
                                        }
                                        if (Main.tile.At(num39 - 1, num40).Active && Main.tile.At(num39 - 1, num40).Type == 69)
                                        {
                                            num41++;
                                        }
                                        if (Main.tile.At(num39, num40 + 1).Active && Main.tile.At(num39, num40 + 1).Type == 69)
                                        {
                                            num41++;
                                        }
                                        if (Main.tile.At(num39, num40 - 1).Active && Main.tile.At(num39, num40 - 1).Type == 69)
                                        {
                                            num41++;
                                        }
                                        if (num41 < 2)
                                        {
                                            int num43 = 7;
                                            int num44 = num39 - num43;
                                            int num45 = num39 + num43;
                                            int num46 = num40 - num43;
                                            int num47 = num40 + num43;
                                            bool flag9 = false;
                                            for (int num48 = num44; num48 < num45; num48++)
                                            {
                                                for (int num49 = num46; num49 < num47; num49++)
                                                {
                                                    if (Math.Abs(num48 - num39) * 2 + Math.Abs(num49 - num40) < 9 && Main.tile.At(num48, num49).Active && Main.tile.At(num48, num49).Type == 60 && Main.tile.At(num48, num49 - 1).Active && Main.tile.At(num48, num49 - 1).Type == 69 && Main.tile.At(num48, num49 - 1).Liquid == 0)
                                                    {
                                                        flag9 = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (flag9)
                                            {
                                                Main.tile.At(num39, num40).SetType (69);
                                                Main.tile.At(num39, num40).SetActive (true);
                                                WorldGen.SquareTileFrame(num39, num40, true);
                                                NetMessage.SendTileSquare(-1, num39, num40, 3);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Main.tile.At(num28, num29).Type == 70)
                            {
                                int type3 = (int)Main.tile.At(num28, num29).Type;
                                if (!Main.tile.At(num28, num32).Active && WorldGen.genRand.Next(10) == 0)
                                {
                                    WorldGen.PlaceTile(num28, num32, 71, true, false, -1, 0);
                                    if (Main.tile.At(num28, num32).Active)
                                    {
                                        NetMessage.SendTileSquare(-1, num28, num32, 1);
                                    }
                                }
                                if (WorldGen.genRand.Next(200) == 0 && !WorldGen.PlayerLOS(num28, num29))
                                {
                                    WorldGen.GrowShroom(num28, num29);
                                }
                                bool flag10 = false;
                                for (int num50 = num30; num50 < num31; num50++)
                                {
                                    for (int num51 = num32; num51 < num33; num51++)
                                    {
                                        if ((num28 != num50 || num29 != num51) && Main.tile.At(num50, num51).Active && Main.tile.At(num50, num51).Type == 59)
                                        {
                                            WorldGen.SpreadGrass(num50, num51, 59, type3, false);
                                            if ((int)Main.tile.At(num50, num51).Type == type3)
                                            {
                                                WorldGen.SquareTileFrame(num50, num51, true);
                                                flag10 = true;
                                            }
                                        }
                                    }
                                }
                                if (flag10)
                                {
                                    NetMessage.SendTileSquare(-1, num28, num29, 3);
                                }
                            }
                        }
                        else
                        {
                            if (flag && WorldGen.spawnNPC > 0)
                            {
                                WorldGen.SpawnNPC(num28, num29);
                            }
                        }
                    }
                }
                num27++;
            }
            if (Main.rand.Next(100) == 0)
            {
                WorldGen.PlantAlch();
            }
            if (!Main.dayTime)
            {
                float num52 = (float)(Main.maxTilesX / 4200);
                if ((float)Main.rand.Next(8000) < 10f * num52)
                {
                    int num53 = 12;
                    int num54 = Main.rand.Next(Main.maxTilesX - 50) + 100;
                    num54 *= 16;
                    int num55 = Main.rand.Next((int)((double)Main.maxTilesY * 0.05));
                    num55 *= 16;
                    Vector2 vector = new Vector2((float)num54, (float)num55);
                    float num56 = (float)Main.rand.Next(-100, 101);
                    float num57 = (float)(Main.rand.Next(200) + 100);
                    float num58 = (float)Math.Sqrt((double)(num56 * num56 + num57 * num57));
                    num58 = (float)num53 / num58;
                    num56 *= num58;
                    num57 *= num58;
                    Projectile.NewProjectile(vector.X, vector.Y, num56, num57, ProjectileType.FALLEN_STAR, 1000, 10f, Main.myPlayer);
                }
            }
        }
        
        public static void PlaceWall(int i, int j, int type, bool mute = false)
        {
            if ((int)Main.tile.At(i, j).Wall != type)
            {
                for (int k = i - 1; k < i + 2; k++)
                {
                    for (int l = j - 1; l < j + 2; l++)
                    {
                        if (Main.tile.At(k, l).Wall > 0 && (int)Main.tile.At(k, l).Wall != type)
                        {
                            return;
                        }
                    }
                }
                Main.tile.At(i, j).SetWall ((byte)type);
                WorldGen.SquareWallFrame(i, j, true);
            }
        }
        
        public static void AddPlants()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 1; j < Main.maxTilesY; j++)
                {
                    if (Main.tile.At(i, j).Type == 2 && Main.tile.At(i, j).Active)
                    {
                        if (!Main.tile.At(i, j - 1).Active)
                        {
                            WorldGen.PlaceTile(i, j - 1, 3, true, false, -1, 0);
                        }
                    }
                    else
                    {
                        if (Main.tile.At(i, j).Type == 23 && Main.tile.At(i, j).Active && !Main.tile.At(i, j - 1).Active)
                        {
                            WorldGen.PlaceTile(i, j - 1, 24, true, false, -1, 0);
                        }
                    }
                }
            }
        }
        
        public static void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true)
        {
            if ((int)Main.tile.At(i, j).Type != dirt || !Main.tile.At(i, j).Active || ((double)j < Main.worldSurface && grass == 70) || ((double)j >= Main.worldSurface && dirt == 0))
            {
                return;
            }
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
                    if (!Main.tile.At(k, l).Active || !Main.tileSolid[(int)Main.tile.At(k, l).Type])
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (!flag)
            {
                if (grass == 23 && Main.tile.At(i, j - 1).Type == 27)
                {
                    return;
                }
                Main.tile.At(i, j).SetType ((byte)grass);
                for (int m = num; m < num2; m++)
                {
                    for (int n = num3; n < num4; n++)
                    {
                        if (Main.tile.At(m, n).Active && (int)Main.tile.At(m, n).Type == dirt && repeat)
                        {
                            WorldGen.SpreadGrass(m, n, dirt, grass, true);
                        }
                    }
                }
            }
        }
        
        public static void ChasmRunnerSideways(int i, int j, int direction, int steps)
        {
            float num = (float)steps;
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j;
            Vector2 value2;
            value2.X = (float)WorldGen.genRand.Next(10, 21) * 0.1f * (float)direction;
            value2.Y = (float)WorldGen.genRand.Next(-10, 10) * 0.01f;
            double num2 = (double)(WorldGen.genRand.Next(5) + 7);
            while (num2 > 0.0)
            {
                if (num > 0f)
                {
                    num2 += (double)WorldGen.genRand.Next(3);
                    num2 -= (double)WorldGen.genRand.Next(3);
                    if (num2 < 7.0)
                    {
                        num2 = 7.0;
                    }
                    if (num2 > 20.0)
                    {
                        num2 = 20.0;
                    }
                    if (num == 1f && num2 < 10.0)
                    {
                        num2 = 10.0;
                    }
                }
                else
                {
                    num2 -= (double)WorldGen.genRand.Next(4);
                }
                if ((double)value.Y > Main.rockLayer && num > 0f)
                {
                    num = 0f;
                }
                num -= 1f;
                int num3 = (int)((double)value.X - num2 * 0.5);
                int num4 = (int)((double)value.X + num2 * 0.5);
                int num5 = (int)((double)value.Y - num2 * 0.5);
                int num6 = (int)((double)value.Y + num2 * 0.5);
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (num4 > Main.maxTilesX - 1)
                {
                    num4 = Main.maxTilesX - 1;
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
                        if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num2 * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile.At(k, l).Type != 31 && Main.tile.At(k, l).Type != 22)
                        {
                            Main.tile.At(k, l).SetActive (false);
                        }
                    }
                }
                value += value2;
                value2.Y += (float)WorldGen.genRand.Next(-10, 10) * 0.1f;
                if (value.Y < (float)(j - 20))
                {
                    value2.Y += (float)WorldGen.genRand.Next(20) * 0.01f;
                }
                if (value.Y > (float)(j + 20))
                {
                    value2.Y -= (float)WorldGen.genRand.Next(20) * 0.01f;
                }
                if ((double)value2.Y < -0.5)
                {
                    value2.Y = -0.5f;
                }
                if ((double)value2.Y > 0.5)
                {
                    value2.Y = 0.5f;
                }
                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.01f;
                if (direction == -1)
                {
                    if ((double)value2.X > -0.5)
                    {
                        value2.X = -0.5f;
                    }
                    if (value2.X < -2f)
                    {
                        value2.X = -2f;
                    }
                }
                else
                {
                    if (direction == 1)
                    {
                        if ((double)value2.X < 0.5)
                        {
                            value2.X = 0.5f;
                        }
                        if (value2.X > 2f)
                        {
                            value2.X = 2f;
                        }
                    }
                }
                num3 = (int)((double)value.X - num2 * 1.1);
                num4 = (int)((double)value.X + num2 * 1.1);
                num5 = (int)((double)value.Y - num2 * 1.1);
                num6 = (int)((double)value.Y + num2 * 1.1);
                if (num3 < 1)
                {
                    num3 = 1;
                }
                if (num4 > Main.maxTilesX - 1)
                {
                    num4 = Main.maxTilesX - 1;
                }
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesY)
                {
                    num6 = Main.maxTilesY;
                }
                for (int m = num3; m < num4; m++)
                {
                    for (int n = num5; n < num6; n++)
                    {
                        if ((double)(Math.Abs((float)m - value.X) + Math.Abs((float)n - value.Y)) < num2 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile.At(m, n).Wall != 3)
                        {
                            if (Main.tile.At(m, n).Type != 25 && n > j + WorldGen.genRand.Next(3, 20))
                            {
                                Main.tile.At(m, n).SetActive (true);
                            }
                            Main.tile.At(m, n).SetActive (true);
                            if (Main.tile.At(m, n).Type != 31 && Main.tile.At(m, n).Type != 22)
                            {
                                Main.tile.At(m, n).SetType (25);
                            }
                            if (Main.tile.At(m, n).Wall == 2)
                            {
                                Main.tile.At(m, n).SetWall (0);
                            }
                        }
                    }
                }
                for (int num7 = num3; num7 < num4; num7++)
                {
                    for (int num8 = num5; num8 < num6; num8++)
                    {
                        if ((double)(Math.Abs((float)num7 - value.X) + Math.Abs((float)num8 - value.Y)) < num2 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile.At(num7, num8).Wall != 3)
                        {
                            if (Main.tile.At(num7, num8).Type != 31 && Main.tile.At(num7, num8).Type != 22)
                            {
                                Main.tile.At(num7, num8).SetType (25);
                            }
                            Main.tile.At(num7, num8).SetActive (true);
                            WorldGen.PlaceWall(num7, num8, 3, true);
                        }
                    }
                }
            }
            if (WorldGen.genRand.Next(3) == 0)
            {
                int num9 = (int)value.X;
                int num10 = (int)value.Y;
                while (!Main.tile.At(num9, num10).Active)
                {
                    num10++;
                }
                WorldGen.TileRunner(num9, num10, (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(3, 7), 22, false, 0f, 0f, false, true);
            }
        }
        
        public static void ChasmRunner(int i, int j, int steps, bool makeOrb = false)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            if (!makeOrb)
            {
                flag2 = true;
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
                    if ((double)value.Y > Main.worldSurface + 45.0)
                    {
                        num3 -= (double)WorldGen.genRand.Next(4);
                    }
                }
                if ((double)value.Y > Main.rockLayer && num > 0f)
                {
                    num = 0f;
                }
                num -= 1f;
                if (!flag && (double)value.Y > Main.worldSurface + 20.0)
                {
                    flag = true;
                    WorldGen.ChasmRunnerSideways((int)value.X, (int)value.Y, -1, WorldGen.genRand.Next(20, 40));
                    WorldGen.ChasmRunnerSideways((int)value.X, (int)value.Y, 1, WorldGen.genRand.Next(20, 40));
                }
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
                            if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num3 * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile.At(k, l).Type != 31 && Main.tile.At(k, l).Type != 22)
                            {
                                Main.tile.At(k, l).SetActive (false);
                            }
                        }
                    }
                }
                if (num <= 2f && (double)value.Y < Main.worldSurface + 45.0)
                {
                    num = 2f;
                }
                if (num <= 0f)
                {
                    if (!flag2)
                    {
                        flag2 = true;
                        WorldGen.AddShadowOrb((int)value.X, (int)value.Y);
                    }
                    else
                    {
                        if (!flag3)
                        {
                            flag3 = false;
                            bool flag4 = false;
                            int num8 = 0;
                            while (!flag4)
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
                                    if (Main.tile.At(num9, num10).Type == 26)
                                    {
                                        flag4 = true;
                                    }
                                    else
                                    {
                                        num8++;
                                        if (num8 >= 10000)
                                        {
                                            flag4 = true;
                                        }
                                    }
                                }
                                else
                                {
                                    flag4 = true;
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
                for (int m = num4; m < num5; m++)
                {
                    for (int n = num6; n < num7; n++)
                    {
                        if ((double)(Math.Abs((float)m - value.X) + Math.Abs((float)n - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            if (Main.tile.At(m, n).Type != 25 && n > j + WorldGen.genRand.Next(3, 20))
                            {
                                Main.tile.At(m, n).SetActive (true);
                            }
                            if (steps <= num2)
                            {
                                Main.tile.At(m, n).SetActive (true);
                            }
                            if (Main.tile.At(m, n).Type != 31)
                            {
                                Main.tile.At(m, n).SetType (25);
                            }
                            if (Main.tile.At(m, n).Wall == 2)
                            {
                                Main.tile.At(m, n).SetWall (0);
                            }
                        }
                    }
                }
                for (int num11 = num4; num11 < num5; num11++)
                {
                    for (int num12 = num6; num12 < num7; num12++)
                    {
                        if ((double)(Math.Abs((float)num11 - value.X) + Math.Abs((float)num12 - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            if (Main.tile.At(num11, num12).Type != 31)
                            {
                                Main.tile.At(num11, num12).SetType (25);
                            }
                            if (steps <= num2)
                            {
                                Main.tile.At(num11, num12).SetActive (true);
                            }
                            if (num12 > j + WorldGen.genRand.Next(3, 20))
                            {
                                WorldGen.PlaceWall(num11, num12, 3, true);
                            }
                        }
                    }
                }
            }
        }
        
        public static void JungleRunner(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(5, 11);
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j;
            Vector2 value2;
            value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            value2.Y = (float)WorldGen.genRand.Next(10, 20) * 0.1f;
            int num2 = 0;
            bool flag = true;
            while (flag)
            {
                if ((double)value.Y < Main.worldSurface)
                {
                    int num3 = (int)value.X;
                    int num4 = (int)value.Y;
                    if (Main.tile.At(num3, num4).Wall == 0 && !Main.tile.At(num3, num4).Active && Main.tile.At(num3, num4 - 3).Wall == 0 && !Main.tile.At(num3, num4 - 3).Active && Main.tile.At(num3, num4 - 1).Wall == 0 && !Main.tile.At(num3, num4 - 1).Active && Main.tile.At(num3, num4 - 4).Wall == 0 && !Main.tile.At(num3, num4 - 4).Active && Main.tile.At(num3, num4 - 2).Wall == 0 && !Main.tile.At(num3, num4 - 2).Active && Main.tile.At(num3, num4 - 5).Wall == 0 && !Main.tile.At(num3, num4 - 5).Active)
                    {
                        flag = false;
                    }
                }
                WorldGen.JungleX = (int)value.X;
                num += (double)((float)WorldGen.genRand.Next(-20, 21) * 0.1f);
                if (num < 5.0)
                {
                    num = 5.0;
                }
                if (num > 10.0)
                {
                    num = 10.0;
                }
                int num5 = (int)((double)value.X - num * 0.5);
                int num6 = (int)((double)value.X + num * 0.5);
                int num7 = (int)((double)value.Y - num * 0.5);
                int num8 = (int)((double)value.Y + num * 0.5);
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num6 > Main.maxTilesX)
                {
                    num6 = Main.maxTilesX;
                }
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesY)
                {
                    num8 = Main.maxTilesY;
                }
                for (int k = num5; k < num6; k++)
                {
                    for (int l = num7; l < num8; l++)
                    {
                        if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            WorldGen.KillTile(k, l, false, false, false);
                        }
                    }
                }
                num2++;
                if (num2 > 10 && WorldGen.genRand.Next(50) < num2)
                {
                    num2 = 0;
                    int num9 = -2;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num9 = 2;
                    }
                    WorldGen.TileRunner((int)value.X, (int)value.Y, (double)WorldGen.genRand.Next(3, 20), WorldGen.genRand.Next(10, 100), -1, false, (float)num9, 0f, false, true);
                }
                value += value2;
                value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.01f;
                if (value2.Y > 0f)
                {
                    value2.Y = 0f;
                }
                if (value2.Y < -2f)
                {
                    value2.Y = -2f;
                }
                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
                if (value.X < (float)(i - 200))
                {
                    value2.X += (float)WorldGen.genRand.Next(5, 21) * 0.1f;
                }
                if (value.X > (float)(i + 200))
                {
                    value2.X -= (float)WorldGen.genRand.Next(5, 21) * 0.1f;
                }
                if ((double)value2.X > 1.5)
                {
                    value2.X = 1.5f;
                }
                if ((double)value2.X < -1.5)
                {
                    value2.X = -1.5f;
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
                                if (type == -2 && Main.tile.At(k, l).Active && (l < WorldGen.waterLine || l > WorldGen.lavaLine))
                                {
                                    Main.tile.At(k, l).SetLiquid (255);
                                    if (l > WorldGen.lavaLine)
                                    {
                                        Main.tile.At(k, l).SetLava (true);
                                    }
                                }
                                Main.tile.At(k, l).SetActive (false);
                            }
                            else
                            {
                                if ((overRide || !Main.tile.At(k, l).Active) && (type != 40 || Main.tile.At(k, l).Type != 53) && (!Main.tileStone[type] || Main.tile.At(k, l).Type == 1) && Main.tile.At(k, l).Type != 45 && (Main.tile.At(k, l).Type != 1 || type != 59 || (double)l >= Main.worldSurface + (double)WorldGen.genRand.Next(-50, 50)))
                                {
                                    if (Main.tile.At(k, l).Type != 53 || (double)l >= Main.worldSurface)
                                    {
                                        Main.tile.At(k, l).SetType ((byte)type);
                                    }
                                    else
                                    {
                                        if (type == 59)
                                        {
                                            Main.tile.At(k, l).SetType ((byte)type);
                                        }
                                    }
                                }
                                if (addTile)
                                {
                                    Main.tile.At(k, l).SetActive (true);
                                    Main.tile.At(k, l).SetLiquid (0);
                                    Main.tile.At(k, l).SetLava (false);
                                }
                                if (noYChange && (double)l < Main.worldSurface && type != 59)
                                {
                                    Main.tile.At(k, l).SetWall (2);
                                }
                                if (type == 59 && l > WorldGen.waterLine && Main.tile.At(k, l).Liquid > 0)
                                {
                                    Main.tile.At(k, l).SetLava (false);
                                    Main.tile.At(k, l).SetLiquid (0);
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
                    if (type != 59 && num < 3.0)
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
                                Main.tile.At(k, l).SetActive (true);
                                if (Main.tile.At(k, l).Type == 59)
                                {
                                    Main.tile.At(k, l).SetType (0);
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
                                Main.tile.At(m, n).SetWall (2);
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
                if (Main.tile.At((int)vector.X, k).Active)
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
            for (int l = num4; l <= num5; l++)
            {
                for (int m = num6; m < num7; m++)
                {
                    Main.tile.At(l, m).SetActive (true);
                    Main.tile.At(l, m).SetType (type);
                    Main.tile.At(l, m).SetWall (0);
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
            for (int n = num4; n <= num5; n++)
            {
                for (int num8 = num6; num8 < num7; num8++)
                {
                    if (Main.tile.At(n, num8).Wall == 0)
                    {
                        Main.tile.At(n, num8).SetActive (false);
                        Main.tile.At(n, num8).SetWall (wall);
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile.At(num11, num10).SetActive (false);
                Main.tile.At(num11, num10 - 1).SetActive (false);
                Main.tile.At(num11, num10 - 2).SetActive (false);
            }
            WorldGen.PlaceTile(num9, num10, 10, true, false, -1, 0);
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
            WorldGen.AddBuriedChest(i, num10 - 3, contain, false);
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
                        if (num10 < num7 * 0.4 && !Main.tile.At(k, l).Active)
                        {
                            Main.tile.At(k, l).SetActive (true);
                            Main.tile.At(k, l).SetType (0);
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
                            if (Main.tile.At(k, l).Active)
                            {
                                Main.tile.At(k, l).SetLiquid (255);
                            }
                            Main.tile.At(k, l).SetActive (false);
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
                                if (Main.tile.At(k, l).Type != 59)
                                {
                                    Main.tile.At(k, l).SetActive (false);
                                }
                            }
                            else
                            {
                                Main.tile.At(k, l).SetType (59);
                            }
                            Main.tile.At(k, l).SetLiquid (0);
                            Main.tile.At(k, l).SetLava (false);
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
                            Main.tile.At(l, m).SetActive (false);
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
                if (Main.tile.At((int)value.X, (int)value.Y).Wall == 0)
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
                            Main.tile.At(l, m).SetActive (false);
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
                    Main.tile.At(i, j).SetCheckingLiquid (false);
                    if (Main.tile.At(i, j).Liquid > 0 && Main.tile.At(i, j).Active && Main.tileSolid[(int)Main.tile.At(i, j).Type] && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
                    {
                        Main.tile.At(i, j).SetLiquid (0);
                    }
                    else
                    {
                        if (Main.tile.At(i, j).Liquid > 0)
                        {
                            if (Main.tile.At(i, j).Active)
                            {
                                if (Main.tileWaterDeath[(int)Main.tile.At(i, j).Type])
                                {
                                    WorldGen.KillTile(i, j, false, false, false);
                                }
                                if (Main.tile.At(i, j).Lava && Main.tileLavaDeath[(int)Main.tile.At(i, j).Type])
                                {
                                    WorldGen.KillTile(i, j, false, false, false);
                                }
                            }
                            if ((!Main.tile.At(i, j + 1).Active || !Main.tileSolid[(int)Main.tile.At(i, j + 1).Type] || Main.tileSolidTop[(int)Main.tile.At(i, j + 1).Type]) && Main.tile.At(i, j + 1).Liquid < 255)
                            {
                                if (Main.tile.At(i, j + 1).Liquid > 250)
                                {
                                    Main.tile.At(i, j + 1).SetLiquid (255);
                                }
                                else
                                {
                                    Liquid.AddWater(i, j);
                                }
                            }
                            if ((!Main.tile.At(i - 1, j).Active || !Main.tileSolid[(int)Main.tile.At(i - 1, j).Type] || Main.tileSolidTop[(int)Main.tile.At(i - 1, j).Type]) && Main.tile.At(i - 1, j).Liquid != Main.tile.At(i, j).Liquid)
                            {
                                Liquid.AddWater(i, j);
                            }
                            else
                            {
                                if ((!Main.tile.At(i + 1, j).Active || !Main.tileSolid[(int)Main.tile.At(i + 1, j).Type] || Main.tileSolidTop[(int)Main.tile.At(i + 1, j).Type]) && Main.tile.At(i + 1, j).Liquid != Main.tile.At(i, j).Liquid)
                                {
                                    Liquid.AddWater(i, j);
                                }
                            }
                            if (Main.tile.At(i, j).Lava)
                            {
                                if (Main.tile.At(i - 1, j).Liquid > 0 && !Main.tile.At(i - 1, j).Lava)
                                {
                                    Liquid.AddWater(i, j);
                                }
                                else
                                {
                                    if (Main.tile.At(i + 1, j).Liquid > 0 && !Main.tile.At(i + 1, j).Lava)
                                    {
                                        Liquid.AddWater(i, j);
                                    }
                                    else
                                    {
                                        if (Main.tile.At(i, j - 1).Liquid > 0 && !Main.tile.At(i, j - 1).Lava)
                                        {
                                            Liquid.AddWater(i, j);
                                        }
                                        else
                                        {
                                            if (Main.tile.At(i, j + 1).Liquid > 0 && !Main.tile.At(i, j + 1).Lava)
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
            WorldGen.noTileActions = true;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                float num = (float)i / (float)Main.maxTilesX;
                Program.printData("Finding tile frames: " + (int)(num * 100f + 1f) + "%", true);
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    WorldGen.TileFrame(i, j, true, false);
                    WorldGen.WallFrame(i, j, true);
                }
            }
            WorldGen.noLiquidCheck = false;
            WorldGen.noTileActions = false;
        }
        
        public static void PlantCheck(int i, int j)
        {
            int num = -1;
            int type = (int)Main.tile.At(i, j).Type;
            int arg_19_0 = i - 1;
            int arg_23_0 = i + 1;
            int arg_22_0 = Main.maxTilesX;
            int arg_29_0 = j - 1;
            if (j + 1 >= Main.maxTilesY)
            {
                num = type;
            }
            if (i - 1 >= 0 && Main.tile.At(i - 1, j).Active)
            {
                byte arg_74_0 = Main.tile.At(i - 1, j).Type;
            }
            if (i + 1 < Main.maxTilesX && Main.tile.At(i + 1, j).Active)
            {
                byte arg_B7_0 = Main.tile.At(i + 1, j).Type;
            }
            if (j - 1 >= 0 && Main.tile.At(i, j - 1).Active)
            {
                byte arg_F6_0 = Main.tile.At(i, j - 1).Type;
            }
            if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active)
            {
                num = (int)Main.tile.At(i, j + 1).Type;
            }
            if (i - 1 >= 0 && j - 1 >= 0 && Main.tile.At(i - 1, j - 1).Active)
            {
                byte arg_184_0 = Main.tile.At(i - 1, j - 1).Type;
            }
            if (i + 1 < Main.maxTilesX && j - 1 >= 0 && Main.tile.At(i + 1, j - 1).Active)
            {
                byte arg_1D3_0 = Main.tile.At(i + 1, j - 1).Type;
            }
            if (i - 1 >= 0 && j + 1 < Main.maxTilesY && Main.tile.At(i - 1, j + 1).Active)
            {
                byte arg_222_0 = Main.tile.At(i - 1, j + 1).Type;
            }
            if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY && Main.tile.At(i + 1, j + 1).Active)
            {
                byte arg_275_0 = Main.tile.At(i + 1, j + 1).Type;
            }
            if ((type == 3 && num != 2 && num != 78) || (type == 24 && num != 23) || (type == 61 && num != 60) || (type == 71 && num != 70) || (type == 73 && num != 2 && num != 78) || (type == 74 && num != 60))
            {
                WorldGen.KillTile(i, j, false, false, false);
            }
        }
        
        public static void WallFrame(int i, int j, bool resetFrame = false)
        {
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY && Main.tile.At(i, j).Wall > 0)
            {
                int num = -1;
                int num2 = -1;
                int num3 = -1;
                int num4 = -1;
                int num5 = -1;
                int num6 = -1;
                int num7 = -1;
                int num8 = -1;
                int wall = (int)Main.tile.At(i, j).Wall;
                if (wall == 0)
                {
                    return;
                }
                byte arg_89_0 = Main.tile.At(i, j).WallFrameX;
                byte arg_9B_0 = Main.tile.At(i, j).WallFrameY;
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
                if (i - 1 >= 0)
                {
                    num4 = (int)Main.tile.At(i - 1, j).Wall;
                }
                if (i + 1 < Main.maxTilesX)
                {
                    num5 = (int)Main.tile.At(i + 1, j).Wall;
                }
                if (j - 1 >= 0)
                {
                    num2 = (int)Main.tile.At(i, j - 1).Wall;
                }
                if (j + 1 < Main.maxTilesY)
                {
                    num7 = (int)Main.tile.At(i, j + 1).Wall;
                }
                if (i - 1 >= 0 && j - 1 >= 0)
                {
                    num = (int)Main.tile.At(i - 1, j - 1).Wall;
                }
                if (i + 1 < Main.maxTilesX && j - 1 >= 0)
                {
                    num3 = (int)Main.tile.At(i + 1, j - 1).Wall;
                }
                if (i - 1 >= 0 && j + 1 < Main.maxTilesY)
                {
                    num6 = (int)Main.tile.At(i - 1, j + 1).Wall;
                }
                if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY)
                {
                    num8 = (int)Main.tile.At(i + 1, j + 1).Wall;
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
                    Main.tile.At(i, j).SetWallFrameNumber ((byte)num9);
                }
                else
                {
                    num9 = (int)Main.tile.At(i, j).WallFrameNumber;
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
                Main.tile.At(i, j).SetWallFrameX ((byte)rectangle.X);
                Main.tile.At(i, j).SetWallFrameY ((byte)rectangle.Y);
            }
        }

        public static void TileFrame(int i, int j, bool resetFrame = false, bool noBreak = false)
        {
            if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
            {
                if (Main.tile.At(i, j).Liquid > 0 && !WorldGen.noLiquidCheck)
                {
                    Liquid.AddWater(i, j);
                }
                if (Main.tile.At(i, j).Active)
                {
                    if (noBreak && Main.tileFrameImportant[(int)Main.tile.At(i, j).Type])
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
                    int num9 = (int)Main.tile.At(i, j).Type;
                    if (Main.tileStone[num9])
                    {
                        num9 = 1;
                    }
                    int frameX = (int)Main.tile.At(i, j).FrameX;
                    int frameY = (int)Main.tile.At(i, j).FrameY;
                    Rectangle rectangle;
                    rectangle.X = -1;
                    rectangle.Y = -1;
                    if (num9 == 3 || num9 == 24 || num9 == 61 || num9 == 71 || num9 == 73 || num9 == 74)
                    {
                        WorldGen.PlantCheck(i, j);
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
                    if (i - 1 >= 0 && Main.tile.At(i - 1, j).Active)
                    {
                        num4 = (int)Main.tile.At(i - 1, j).Type;
                    }
                    if (i + 1 < Main.maxTilesX && Main.tile.At(i + 1, j).Active)
                    {
                        num5 = (int)Main.tile.At(i + 1, j).Type;
                    }
                    if (j - 1 >= 0 && Main.tile.At(i, j - 1).Active)
                    {
                        num2 = (int)Main.tile.At(i, j - 1).Type;
                    }
                    if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active)
                    {
                        num7 = (int)Main.tile.At(i, j + 1).Type;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0 && Main.tile.At(i - 1, j - 1).Active)
                    {
                        num = (int)Main.tile.At(i - 1, j - 1).Type;
                    }
                    if (i + 1 < Main.maxTilesX && j - 1 >= 0 && Main.tile.At(i + 1, j - 1).Active)
                    {
                        num3 = (int)Main.tile.At(i + 1, j - 1).Type;
                    }
                    if (i - 1 >= 0 && j + 1 < Main.maxTilesY && Main.tile.At(i - 1, j + 1).Active)
                    {
                        num6 = (int)Main.tile.At(i - 1, j + 1).Type;
                    }
                    if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY && Main.tile.At(i + 1, j + 1).Active)
                    {
                        num8 = (int)Main.tile.At(i + 1, j + 1).Type;
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
                            Main.tile.At(i, j).SetFrameX (0);
                            return;
                        }
                        if ((num4 >= 0 && Main.tileSolid[num4] && !Main.tileNoAttach[num4]) || (num4 == 5 && num == 5 && num6 == 5))
                        {
                            Main.tile.At(i, j).SetFrameX (22);
                            return;
                        }
                        if ((num5 >= 0 && Main.tileSolid[num5] && !Main.tileNoAttach[num5]) || (num5 == 5 && num3 == 5 && num8 == 5))
                        {
                            Main.tile.At(i, j).SetFrameX (44);
                            return;
                        }
                        WorldGen.KillTile(i, j, false, false, false);
                        return;
                    }
                    else
                    {
                        if (num9 == 80)
                        {
                            WorldGen.CactusFrame(i, j);
                            return;
                        }
                        if (num9 == 12 || num9 == 31)
                        {
                            if (!WorldGen.destroyObject)
                            {
                                int num10 = i;
                                int num11 = j;
                                if (Main.tile.At(i, j).FrameX == 0)
                                {
                                    num10 = i;
                                }
                                else
                                {
                                    num10 = i - 1;
                                }
                                if (Main.tile.At(i, j).FrameY == 0)
                                {
                                    num11 = j;
                                }
                                else
                                {
                                    num11 = j - 1;
                                }
                                if ((!Main.tile.At(num10, num11).Active || (int)Main.tile.At(num10, num11).Type != num9 || !Main.tile.At(num10 + 1, num11).Active || (int)Main.tile.At(num10 + 1, num11).Type != num9 || !Main.tile.At(num10, num11 + 1).Active || (int)Main.tile.At(num10, num11 + 1).Type != num9 || !Main.tile.At(num10 + 1, num11 + 1).Active || (int)Main.tile.At(num10 + 1, num11 + 1).Type != num9))
                                {
                                    WorldGen.destroyObject = true;
                                    if ((int)Main.tile.At(num10, num11).Type == num9)
                                    {
                                        WorldGen.KillTile(num10, num11, false, false, false);
                                    }
                                    if ((int)Main.tile.At(num10 + 1, num11).Type == num9)
                                    {
                                        WorldGen.KillTile(num10 + 1, num11, false, false, false);
                                    }
                                    if ((int)Main.tile.At(num10, num11 + 1).Type == num9)
                                    {
                                        WorldGen.KillTile(num10, num11 + 1, false, false, false);
                                    }
                                    if ((int)Main.tile.At(num10 + 1, num11 + 1).Type == num9)
                                    {
                                        WorldGen.KillTile(num10 + 1, num11 + 1, false, false, false);
                                    }
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
                                                for (int k = 0; k < 255; k++)
                                                {
                                                    float num16 = Math.Abs(Main.players[k].Position.X - num13) + Math.Abs(Main.players[k].Position.Y - num14);
                                                    if (num16 < num15 || num15 == -1f)
                                                    {
                                                        num15 = num16;
                                                    }
                                                }
                                                NPC.SpawnOnPlayer(Main.players[0], 0, 13);
                                            }
                                            else
                                            {
                                                String text = "A horrible chill goes down your spine...";
                                                if (WorldGen.shadowOrbCount == 2)
                                                {
                                                    text = "Screams echo around you...";
                                                }
                                                NetMessage.SendData(25, -1, -1, text, 255, 50f, 255f, 130f);
                                            }
                                        }
                                    }
                                    WorldGen.destroyObject = false;
                                }
                            }
                            return;
                        }
                        if (num9 == 19)
                        {
                            if (num4 == num9 && num5 == num9)
                            {
                                if (Main.tile.At(i, j).FrameNumber == 0)
                                {
                                    rectangle.X = 0;
                                    rectangle.Y = 0;
                                }
                                if (Main.tile.At(i, j).FrameNumber == 1)
                                {
                                    rectangle.X = 0;
                                    rectangle.Y = 18;
                                }
                                if (Main.tile.At(i, j).FrameNumber == 2)
                                {
                                    rectangle.X = 0;
                                    rectangle.Y = 36;
                                }
                            }
                            else
                            {
                                if (num4 == num9 && num5 == -1)
                                {
                                    if (Main.tile.At(i, j).FrameNumber == 0)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 0;
                                    }
                                    if (Main.tile.At(i, j).FrameNumber == 1)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 18;
                                    }
                                    if (Main.tile.At(i, j).FrameNumber == 2)
                                    {
                                        rectangle.X = 18;
                                        rectangle.Y = 36;
                                    }
                                }
                                else
                                {
                                    if (num4 == -1 && num5 == num9)
                                    {
                                        if (Main.tile.At(i, j).FrameNumber == 0)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 0;
                                        }
                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 18;
                                        }
                                        if (Main.tile.At(i, j).FrameNumber == 2)
                                        {
                                            rectangle.X = 36;
                                            rectangle.Y = 36;
                                        }
                                    }
                                    else
                                    {
                                        if (num4 != num9 && num5 == num9)
                                        {
                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 0;
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 18;
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                            {
                                                rectangle.X = 54;
                                                rectangle.Y = 36;
                                            }
                                        }
                                        else
                                        {
                                            if (num4 == num9 && num5 != num9)
                                            {
                                                if (Main.tile.At(i, j).FrameNumber == 0)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 0;
                                                }
                                                if (Main.tile.At(i, j).FrameNumber == 1)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 18;
                                                }
                                                if (Main.tile.At(i, j).FrameNumber == 2)
                                                {
                                                    rectangle.X = 72;
                                                    rectangle.Y = 36;
                                                }
                                            }
                                            else
                                            {
                                                if (num4 != num9 && num4 != -1 && num5 == -1)
                                                {
                                                    if (Main.tile.At(i, j).FrameNumber == 0)
                                                    {
                                                        rectangle.X = 108;
                                                        rectangle.Y = 0;
                                                    }
                                                    if (Main.tile.At(i, j).FrameNumber == 1)
                                                    {
                                                        rectangle.X = 108;
                                                        rectangle.Y = 18;
                                                    }
                                                    if (Main.tile.At(i, j).FrameNumber == 2)
                                                    {
                                                        rectangle.X = 108;
                                                        rectangle.Y = 36;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 == -1 && num5 != num9 && num5 != -1)
                                                    {
                                                        if (Main.tile.At(i, j).FrameNumber == 0)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 0;
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 18;
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 2)
                                                        {
                                                            rectangle.X = 126;
                                                            rectangle.Y = 36;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Main.tile.At(i, j).FrameNumber == 0)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 0;
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                                        {
                                                            rectangle.X = 90;
                                                            rectangle.Y = 18;
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 2)
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
                                    int frameY2 = (int)Main.tile.At(i, j).FrameY;
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
                                    if (!Main.tile.At(i, num17 - 1).Active || !Main.tileSolid[(int)Main.tile.At(i, num17 - 1).Type])
                                    {
                                        flag = true;
                                    }
                                    if (!Main.tile.At(i, num17 + 3).Active || !Main.tileSolid[(int)Main.tile.At(i, num17 + 3).Type])
                                    {
                                        flag = true;
                                    }
                                    if (!Main.tile.At(i, num17).Active || (int)Main.tile.At(i, num17).Type != num9)
                                    {
                                        flag = true;
                                    }
                                    if (!Main.tile.At(i, num17 + 1).Active || (int)Main.tile.At(i, num17 + 1).Type != num9)
                                    {
                                        flag = true;
                                    }
                                    if (!Main.tile.At(i, num17 + 2).Active || (int)Main.tile.At(i, num17 + 2).Type != num9)
                                    {
                                        flag = true;
                                    }
                                    if (flag)
                                    {
                                        WorldGen.destroyObject = true;
                                        WorldGen.KillTile(i, num17, false, false, false);
                                        WorldGen.KillTile(i, num17 + 1, false, false, false);
                                        WorldGen.KillTile(i, num17 + 2, false, false, false);
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
                                    int num18 = 0;
                                    int num19 = i;
                                    int num20 = j;
                                    int frameX2 = (int)Main.tile.At(i, j).FrameX;
                                    int frameY3 = (int)Main.tile.At(i, j).FrameY;
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
                                    if (!Main.tile.At(num19, num20 - 1).Active || !Main.tileSolid[(int)Main.tile.At(num19, num20 - 1).Type] || !Main.tile.At(num19, num20 + 3).Active || !Main.tileSolid[(int)Main.tile.At(num19, num20 + 3).Type])
                                    {
                                        flag2 = true;
                                        WorldGen.destroyObject = true;
                                        Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
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
                                            if (!flag2 && (Main.tile.At(l, m).Type != 11 || !Main.tile.At(l, m).Active))
                                            {
                                                WorldGen.destroyObject = true;
                                                Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
                                                flag2 = true;
                                                l = num21;
                                                m = num20;
                                            }
                                            if (flag2)
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
                            if (num9 == 55 || num9 == 85)
                            {
                                WorldGen.CheckSign(i, j, num9);
                                return;
                            }
                            if (num9 == 79)
                            {
                                WorldGen.Check4x2(i, j, num9);
                                return;
                            }
                            if (num9 == 85)
                            {
                                WorldGen.Check2x2(i, j, num9);
                                return;
                            }
                            if (num9 == 81)
                            {
                                if (num4 != -1 || num2 != -1 || num5 != -1)
                                {
                                    WorldGen.KillTile(i, j, false, false, false);
                                    return;
                                }
                                if (num7 < 0 || !Main.tileSolid[num7])
                                {
                                    WorldGen.KillTile(i, j, false, false, false);
                                }
                                return;
                            }
                            else
                            {
                                if (Main.tileAlch[num9])
                                {
                                    WorldGen.CheckAlch(i, j);
                                    return;
                                }
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
                                if (num2 != num9 && Main.tile.At(i, j).FrameX == 0)
                                {
                                    Main.tile.At(i, j).SetFrameNumber ((byte)WorldGen.genRand.Next(3));
                                    if (Main.tile.At(i, j).FrameNumber == 0)
                                    {
                                        Main.tile.At(i, j).SetFrameX (18);
                                        Main.tile.At(i, j).SetFrameY (0);
                                    }
                                    if (Main.tile.At(i, j).FrameNumber == 1)
                                    {
                                        Main.tile.At(i, j).SetFrameX (18);
                                        Main.tile.At(i, j).SetFrameY (18);
                                    }
                                    if (Main.tile.At(i, j).FrameNumber == 2)
                                    {
                                        Main.tile.At(i, j).SetFrameX (18);
                                        Main.tile.At(i, j).SetFrameY (36);
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
                            if (Main.tile.At(i, j).FrameX >= 22 && Main.tile.At(i, j).FrameX <= 44 && Main.tile.At(i, j).FrameY >= 132 && Main.tile.At(i, j).FrameY <= 176)
                            {
                                if ((num4 != num9 && num5 != num9) || num7 != 2)
                                {
                                    WorldGen.KillTile(i, j, false, false, false);
                                }
                            }
                            else
                            {
                                if ((Main.tile.At(i, j).FrameX == 88 && Main.tile.At(i, j).FrameY >= 0 && Main.tile.At(i, j).FrameY <= 44) || (Main.tile.At(i, j).FrameX == 66 && Main.tile.At(i, j).FrameY >= 66 && Main.tile.At(i, j).FrameY <= 130) || (Main.tile.At(i, j).FrameX == 110 && Main.tile.At(i, j).FrameY >= 66 && Main.tile.At(i, j).FrameY <= 110) || (Main.tile.At(i, j).FrameX == 132 && Main.tile.At(i, j).FrameY >= 0 && Main.tile.At(i, j).FrameY <= 176))
                                {
                                    if (num4 == num9 && num5 == num9)
                                    {
                                        if (Main.tile.At(i, j).FrameNumber == 0)
                                        {
                                            Main.tile.At(i, j).SetFrameX (110);
                                            Main.tile.At(i, j).SetFrameY (66);
                                        }
                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                        {
                                            Main.tile.At(i, j).SetFrameX (110);
                                            Main.tile.At(i, j).SetFrameY (88);
                                        }
                                        if (Main.tile.At(i, j).FrameNumber == 2)
                                        {
                                            Main.tile.At(i, j).SetFrameX (110);
                                            Main.tile.At(i, j).SetFrameY (110);
                                        }
                                    }
                                    else
                                    {
                                        if (num4 == num9)
                                        {
                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                            {
                                                Main.tile.At(i, j).SetFrameX (88);
                                                Main.tile.At(i, j).SetFrameY (0);
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                            {
                                                Main.tile.At(i, j).SetFrameX (88);
                                                Main.tile.At(i, j).SetFrameY (22);
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                            {
                                                Main.tile.At(i, j).SetFrameX (88);
                                                Main.tile.At(i, j).SetFrameY (44);
                                            }
                                        }
                                        else
                                        {
                                            if (num5 == num9)
                                            {
                                                if (Main.tile.At(i, j).FrameNumber == 0)
                                                {
                                                    Main.tile.At(i, j).SetFrameX (66);
                                                    Main.tile.At(i, j).SetFrameY (66);
                                                }
                                                if (Main.tile.At(i, j).FrameNumber == 1)
                                                {
                                                    Main.tile.At(i, j).SetFrameX (66);
                                                    Main.tile.At(i, j).SetFrameY (88);
                                                }
                                                if (Main.tile.At(i, j).FrameNumber == 2)
                                                {
                                                    Main.tile.At(i, j).SetFrameX (66);
                                                    Main.tile.At(i, j).SetFrameY (110);
                                                }
                                            }
                                            else
                                            {
                                                if (Main.tile.At(i, j).FrameNumber == 0)
                                                {
                                                    Main.tile.At(i, j).SetFrameX (0);
                                                    Main.tile.At(i, j).SetFrameY (0);
                                                }
                                                if (Main.tile.At(i, j).FrameNumber == 1)
                                                {
                                                    Main.tile.At(i, j).SetFrameX (0);
                                                    Main.tile.At(i, j).SetFrameY (22);
                                                }
                                                if (Main.tile.At(i, j).FrameNumber == 2)
                                                {
                                                    Main.tile.At(i, j).SetFrameX (0);
                                                    Main.tile.At(i, j).SetFrameY (44);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (Main.tile.At(i, j).FrameY >= 132 && Main.tile.At(i, j).FrameY <= 176 && (Main.tile.At(i, j).FrameX == 0 || Main.tile.At(i, j).FrameX == 66 || Main.tile.At(i, j).FrameX == 88))
                            {
                                if (num7 != 2)
                                {
                                    WorldGen.KillTile(i, j, false, false, false);
                                }
                                if (num4 != num9 && num5 != num9)
                                {
                                    if (Main.tile.At(i, j).FrameNumber == 0)
                                    {
                                        Main.tile.At(i, j).SetFrameX (0);
                                        Main.tile.At(i, j).SetFrameY (0);
                                    }
                                    if (Main.tile.At(i, j).FrameNumber == 1)
                                    {
                                        Main.tile.At(i, j).SetFrameX (0);
                                        Main.tile.At(i, j).SetFrameY (22);
                                    }
                                    if (Main.tile.At(i, j).FrameNumber == 2)
                                    {
                                        Main.tile.At(i, j).SetFrameX (0);
                                        Main.tile.At(i, j).SetFrameY (44);
                                    }
                                }
                                else
                                {
                                    if (num4 != num9)
                                    {
                                        if (Main.tile.At(i, j).FrameNumber == 0)
                                        {
                                            Main.tile.At(i, j).SetFrameX (0);
                                            Main.tile.At(i, j).SetFrameY (132);
                                        }
                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                        {
                                            Main.tile.At(i, j).SetFrameX (0);
                                            Main.tile.At(i, j).SetFrameY (154);
                                        }
                                        if (Main.tile.At(i, j).FrameNumber == 2)
                                        {
                                            Main.tile.At(i, j).SetFrameX (0);
                                            Main.tile.At(i, j).SetFrameY (176);
                                        }
                                    }
                                    else
                                    {
                                        if (num5 != num9)
                                        {
                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                            {
                                                Main.tile.At(i, j).SetFrameX (66);
                                                Main.tile.At(i, j).SetFrameY (132);
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                            {
                                                Main.tile.At(i, j).SetFrameX (66);
                                                Main.tile.At(i, j).SetFrameY (154);
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                            {
                                                Main.tile.At(i, j).SetFrameX (66);
                                                Main.tile.At(i, j).SetFrameY (176);
                                            }
                                        }
                                        else
                                        {
                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                            {
                                                Main.tile.At(i, j).SetFrameX (88);
                                                Main.tile.At(i, j).SetFrameY (132);
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                            {
                                                Main.tile.At(i, j).SetFrameX (88);
                                                Main.tile.At(i, j).SetFrameY (154);
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                            {
                                                Main.tile.At(i, j).SetFrameX (88);
                                                Main.tile.At(i, j).SetFrameY (176);
                                            }
                                        }
                                    }
                                }
                            }
                            if ((Main.tile.At(i, j).FrameX == 66 && (Main.tile.At(i, j).FrameY == 0 || Main.tile.At(i, j).FrameY == 22 || Main.tile.At(i, j).FrameY == 44)) || (Main.tile.At(i, j).FrameX == 88 && (Main.tile.At(i, j).FrameY == 66 || Main.tile.At(i, j).FrameY == 88 || Main.tile.At(i, j).FrameY == 110)) || (Main.tile.At(i, j).FrameX == 44 && (Main.tile.At(i, j).FrameY == 198 || Main.tile.At(i, j).FrameY == 220 || Main.tile.At(i, j).FrameY == 242)) || (Main.tile.At(i, j).FrameX == 66 && (Main.tile.At(i, j).FrameY == 198 || Main.tile.At(i, j).FrameY == 220 || Main.tile.At(i, j).FrameY == 242)))
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
                                    if (num2 != num9 && Main.tile.At(i, j).FrameY < 198 && ((Main.tile.At(i, j).FrameX != 22 && Main.tile.At(i, j).FrameX != 44) || Main.tile.At(i, j).FrameY < 132))
                                    {
                                        if (num4 == num9 || num5 == num9)
                                        {
                                            if (num7 == num9)
                                            {
                                                if (num4 == num9 && num5 == num9)
                                                {
                                                    if (Main.tile.At(i, j).FrameNumber == 0)
                                                    {
                                                        Main.tile.At(i, j).SetFrameX (132);
                                                        Main.tile.At(i, j).SetFrameY (132);
                                                    }
                                                    if (Main.tile.At(i, j).FrameNumber == 1)
                                                    {
                                                        Main.tile.At(i, j).SetFrameX (132);
                                                        Main.tile.At(i, j).SetFrameY (154);
                                                    }
                                                    if (Main.tile.At(i, j).FrameNumber == 2)
                                                    {
                                                        Main.tile.At(i, j).SetFrameX (132);
                                                        Main.tile.At(i, j).SetFrameY (176);
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 == num9)
                                                    {
                                                        if (Main.tile.At(i, j).FrameNumber == 0)
                                                        {
                                                            Main.tile.At(i, j).SetFrameX (132);
                                                            Main.tile.At(i, j).SetFrameY (0);
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                                        {
                                                            Main.tile.At(i, j).SetFrameX (132);
                                                            Main.tile.At(i, j).SetFrameY (22);
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 2)
                                                        {
                                                            Main.tile.At(i, j).SetFrameX (132);
                                                            Main.tile.At(i, j).SetFrameY (44);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 == num9)
                                                        {
                                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX (132);
                                                                Main.tile.At(i, j).SetFrameY (66);
                                                            }
                                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX (132);
                                                                Main.tile.At(i, j).SetFrameY (88);
                                                            }
                                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX (132);
                                                                Main.tile.At(i, j).SetFrameY (110);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (num4 == num9 && num5 == num9)
                                                {
                                                    if (Main.tile.At(i, j).FrameNumber == 0)
                                                    {
                                                        Main.tile.At(i, j).SetFrameX (154);
                                                        Main.tile.At(i, j).SetFrameY (132);
                                                    }
                                                    if (Main.tile.At(i, j).FrameNumber == 1)
                                                    {
                                                        Main.tile.At(i, j).SetFrameX (154);
                                                        Main.tile.At(i, j).SetFrameY (154);
                                                    }
                                                    if (Main.tile.At(i, j).FrameNumber == 2)
                                                    {
                                                        Main.tile.At(i, j).SetFrameX (154);
                                                        Main.tile.At(i, j).SetFrameY (176);
                                                    }
                                                }
                                                else
                                                {
                                                    if (num4 == num9)
                                                    {
                                                        if (Main.tile.At(i, j).FrameNumber == 0)
                                                        {
                                                            Main.tile.At(i, j).SetFrameX (154);
                                                            Main.tile.At(i, j).SetFrameY (0);
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                                        {
                                                            Main.tile.At(i, j).SetFrameX (154);
                                                            Main.tile.At(i, j).SetFrameY (22);
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 2)
                                                        {
                                                            Main.tile.At(i, j).SetFrameX (154);
                                                            Main.tile.At(i, j).SetFrameY (44);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num5 == num9)
                                                        {
                                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX (154);
                                                                Main.tile.At(i, j).SetFrameY (66);
                                                            }
                                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX (154);
                                                                Main.tile.At(i, j).SetFrameY (88);
                                                            }
                                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX (154);
                                                                Main.tile.At(i, j).SetFrameY (110);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                            {
                                                Main.tile.At(i, j).SetFrameX (110);
                                                Main.tile.At(i, j).SetFrameY (0);
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                            {
                                                Main.tile.At(i, j).SetFrameX (110);
                                                Main.tile.At(i, j).SetFrameY (22);
                                            }
                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                            {
                                                Main.tile.At(i, j).SetFrameX (110);
                                                Main.tile.At(i, j).SetFrameY (44);
                                            }
                                        }
                                    }
                                }
                            }
                            rectangle.X = (int)Main.tile.At(i, j).FrameX;
                            rectangle.Y = (int)Main.tile.At(i, j).FrameY;
                        }
                        if (Main.tileFrameImportant[(int)Main.tile.At(i, j).Type])
                        {
                            return;
                        }
                        int num22 = 0;
                        if (resetFrame)
                        {
                            num22 = WorldGen.genRand.Next(0, 3);
                            Main.tile.At(i, j).SetFrameNumber ((byte)num22);
                        }
                        else
                        {
                            num22 = (int)Main.tile.At(i, j).FrameNumber;
                        }
                        if (num9 == 0)
                        {
                            for (int n = 0; n < 86; n++)
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
                            for (int num23 = 0; num23 < 86; num23++)
                            {
                                if (num23 == 1 || num23 == 6 || num23 == 7 || num23 == 8 || num23 == 9 || num23 == 22 || num23 == 25 || num23 == 37 || num23 == 40 || num23 == 53 || num23 == 56 || num23 == 59)
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
                        Main.tile.At(i, j).SetFrameX ((short)rectangle.X);
                        Main.tile.At(i, j).SetFrameY ((short)rectangle.Y);
                        if (num9 == 52 || num9 == 62)
                        {
                            if (true)
                            {
                                if (!Main.tile.At(i, j - 1).Active)
                                {
                                    num2 = -1;
                                }
                                else
                                {
                                    num2 = (int)Main.tile.At(i, j - 1).Type;
                                }
                            }
                            //else
                            //{
                            //    num2 = num9;
                            //}
                            if (num2 != num9 && num2 != 2 && num2 != 60)
                            {
                                WorldGen.KillTile(i, j, false, false, false);
                            }
                        }
                        if (!WorldGen.noTileActions && (num9 == 53 || ((num9 == 59 || num9 == 57) && WorldGen.genRand.Next(5) == 0)))
                        {
                            if (!Main.tile.At(i, j + 1).Active)
                            {
                                bool flag4 = true;
                                if (Main.tile.At(i, j - 1).Active && Main.tile.At(i, j - 1).Type == 21)
                                {
                                    flag4 = false;
                                }
                                if (flag4)
                                {
                                    ProjectileType type2;
                                    if (num9 == 59)
                                    {
                                        type2 = ProjectileType.BALL_MUD;
                                    }
                                    if (num9 == 57)
                                    {
                                        type2 = ProjectileType.BALL_ASH;
                                    }
                                    else
                                    {
                                        type2 = ProjectileType.BALL_SAND_DROP;
                                    }
                                    Main.tile.At(i, j).SetActive (false);
                                    int num26 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0.41f, type2, 10, 0f, Main.myPlayer);
                                    Main.projectile[num26].Velocity.Y = 0.5f;
                                    Projectile expr_65A3_cp_0 = Main.projectile[num26];
                                    expr_65A3_cp_0.Position.Y = expr_65A3_cp_0.Position.Y + 2f;
                                    Main.projectile[num26].ai[0] = 1f;
                                    NetMessage.SendTileSquare(-1, i, j, 1);
                                    WorldGen.SquareTileFrame(i, j, true);
                                }
                            }
                        }
                        if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
                        {
                            bool flag5 = WorldGen.mergeUp;
                            bool flag6 = WorldGen.mergeDown;
                            bool flag7 = WorldGen.mergeLeft;
                            bool flag8 = WorldGen.mergeRight;
                            WorldGen.TileFrame(i - 1, j, false, false);
                            WorldGen.TileFrame(i + 1, j, false, false);
                            WorldGen.TileFrame(i, j - 1, false, false);
                            WorldGen.TileFrame(i, j + 1, false, false);
                            WorldGen.mergeUp = flag5;
                            WorldGen.mergeDown = flag6;
                            WorldGen.mergeLeft = flag7;
                            WorldGen.mergeRight = flag8;
                        }
                    }
                }
            }
        }
    }
}
