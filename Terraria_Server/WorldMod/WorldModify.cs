using System;
using System.IO;
using System.Diagnostics;
using Terraria_Server.Events;
using Terraria_Server.Commands;
using Terraria_Server.Plugin;
using Terraria_Server.Misc;
using Terraria_Server.Shops;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.Logging;
using Terraria_Server.Networking;

namespace Terraria_Server.WorldMod
{
	public class WorldModify
	{
		private const int RECTANGLE_OFFSET = 25;
		private const int TILE_OFFSET = 15;
		private const int TILES_OFFSET_2 = 10;
		private const int TILE_OFFSET_3 = 16;
		private const int TILE_OFFSET_4 = 23;
        private const int TILE_SCALE = 16;
        private const int TREE_RADIUS = 2;
        private const int MAX_TILE_SETS = 107;

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
		public static bool worldBackup = false;
		public static bool loadBackup = false;
		public static int lastMaxTilesX = 0;
		public static int lastMaxTilesY = 0;
		public static bool saveLock = false;
		private static bool mergeUp = false;
		private static bool mergeDown = false;
		private static bool mergeLeft = false;
		private static bool mergeRight = false;
		public static bool stopDrops = false;
		public static bool noLiquidCheck = false;
		
		public static object playerEditLock = new object();
		
		[ThreadStatic]
		static Random threadRand;
		
		public static Random genRand
		{
			get
			{
				if (threadRand == null) threadRand = new Random ((int)DateTime.Now.Ticks);
				return threadRand;
			}
			
			set
			{
			}
		}

        public static string statusText = "";
		
		// not sure about this, but sure looks like it was supposed to be thread static
		[ThreadStatic]
		private static bool destroyObject = false;
		
		public static int spawnDelay = 0;
		public static int spawnNPC = 0;
		public static int maxRoomTiles = 1900;
		public static int numRoomTiles;
		public static int[] roomX = new int[maxRoomTiles];
		public static int[] roomY = new int[maxRoomTiles];
		public static int roomX1;
		public static int roomX2;
		public static int roomY1;
		public static int roomY2;
		public static bool canSpawn;
        public static bool[] houseTile = new bool[MAX_TILE_SETS];
		public static int bestX = 0;
		public static int bestY = 0;
		public static int hiScore = 0;
		public static int ficount;

		public static void SpawnNPC(int x, int y)
		{
			if (Main.wallHouse[(int)Main.tile.At(x, y).Wall])
			{
				WorldModify.canSpawn = true;
			}
			if (!WorldModify.canSpawn)
			{
				return;
			}
			if (!WorldModify.StartRoomCheck(x, y))
			{
				return;
			}
			if (!WorldModify.RoomNeeds(WorldModify.spawnNPC))
			{
				return;
			}
			WorldModify.ScoreRoom(-1);
			if (WorldModify.hiScore > 0)
			{
				int npcIndex = -1;
				for (int i = 0; i < 1000; i++)
				{
					if (Main.npcs[i].Active && Main.npcs[i].homeless && Main.npcs[i].Type == WorldModify.spawnNPC)
					{
                        npcIndex = i;
						break;
					}
				}
                if (npcIndex == -1)
				{
					int posX = WorldModify.bestX;
					int posY = WorldModify.bestY;
					bool flag = true;
                    Rectangle value = new Rectangle(posX * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, posY * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
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
					if (!flag)
					{
                        //Find a suitable home/spawn location?
						for (int k = 1; k < 500; k++)
						{
                            //See if it's in a 'flat' area?
							for (int l = 0; l < 2; l++)
							{
								if (l == 0)
								{
                                    posX = WorldModify.bestX + k;
								}
								else
								{
                                    posX = WorldModify.bestX - k;
								}
                                if (posX > 10 && posX < Main.maxTilesX - 10)
								{
									int num4 = WorldModify.bestY - k;
									double num5 = (double)(WorldModify.bestY + k);
									if (num4 < 10)
									{
										num4 = 10;
									}
									if (num5 > Main.worldSurface)
									{
										num5 = Main.worldSurface;
									}
									int relativeX = num4;
									while ((double)relativeX < num5)
									{
										posY = relativeX;
                                        if (Main.tile.At(posX, posY).Active && Main.tileSolid[(int)Main.tile.At(posX, posY).Type])
										{
                                            if (!Collision.SolidTiles(posX - 1, posX + 1, posY - 3, posY - 1))
											{
												flag = true;
                                                Rectangle value2 = new Rectangle(posX * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, posY * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
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
											relativeX++;
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
                    int townNPCIndex = NPC.NewNPC(posX * 16, posY * 16, WorldModify.spawnNPC, 1);
					Main.npcs[townNPCIndex].homeTileX = WorldModify.bestX;
					Main.npcs[townNPCIndex].homeTileY = WorldModify.bestY;
                    if (posX < WorldModify.bestX)
					{
						Main.npcs[townNPCIndex].direction = 1;
					}
					else
					{
                        if (posX > WorldModify.bestX)
						{
							Main.npcs[townNPCIndex].direction = -1;
						}
					}
					Main.npcs[townNPCIndex].netUpdate = true;
					
					NetMessage.SendData(25, -1, -1, Main.npcs[townNPCIndex].Name + " has arrived!", 255, 50f, 125f, 255f);
				}
				else
				{
					WorldModify.spawnNPC = 0;
					Main.npcs[npcIndex].homeTileX = WorldModify.bestX;
                    Main.npcs[npcIndex].homeTileY = WorldModify.bestY;
                    Main.npcs[npcIndex].homeless = false;
				}
				WorldModify.spawnNPC = 0;
			}
		}
		
		public static bool RoomNeeds(int npcType)
		{
            if ((WorldModify.houseTile[15] || WorldModify.houseTile[79] || WorldModify.houseTile[89] || WorldModify.houseTile[102]) &&
                (WorldModify.houseTile[14] || WorldModify.houseTile[18] || WorldModify.houseTile[87] || WorldModify.houseTile[88] ||
                    WorldModify.houseTile[90] || WorldModify.houseTile[101]) &&
                (WorldModify.houseTile[4] || WorldModify.houseTile[33] || WorldModify.houseTile[34] || WorldModify.houseTile[35] ||
                    WorldModify.houseTile[36] || WorldModify.houseTile[42] || WorldModify.houseTile[49] || WorldModify.houseTile[93] ||
                        WorldModify.houseTile[95] || WorldModify.houseTile[98] || WorldModify.houseTile[100]) &&
                (WorldModify.houseTile[10] || WorldModify.houseTile[11] || WorldModify.houseTile[19]))
            {
				WorldModify.canSpawn = true;
			}
			else
			{
				WorldModify.canSpawn = false;
			}
			return WorldModify.canSpawn;
		}
		
		public static void QuickFindHome(int npc)
		{
			if (Main.npcs[npc].homeTileX > 10 && Main.npcs[npc].homeTileY > 10 && Main.npcs[npc].homeTileX < Main.maxTilesX - 10 && Main.npcs[npc].homeTileY < Main.maxTilesY)
			{
				WorldModify.canSpawn = false;
				WorldModify.StartRoomCheck(Main.npcs[npc].homeTileX, Main.npcs[npc].homeTileY - 1);
				if (!WorldModify.canSpawn)
				{
					for (int i = Main.npcs[npc].homeTileX - 1; i < Main.npcs[npc].homeTileX + 2; i++)
					{
						int num = Main.npcs[npc].homeTileY - 1;
						while (num < Main.npcs[npc].homeTileY + 2 && !WorldModify.StartRoomCheck(i, num))
						{
							num++;
						}
					}
				}
				if (!WorldModify.canSpawn)
				{
					int num2 = 10;
					for (int j = Main.npcs[npc].homeTileX - num2; j <= Main.npcs[npc].homeTileX + num2; j += 2)
					{
						int num3 = Main.npcs[npc].homeTileY - num2;
						while (num3 <= Main.npcs[npc].homeTileY + num2 && !WorldModify.StartRoomCheck(j, num3))
						{
							num3 += 2;
						}
					}
				}
				if (WorldModify.canSpawn)
				{
					WorldModify.RoomNeeds(Main.npcs[npc].Type);
					if (WorldModify.canSpawn)
					{
						WorldModify.ScoreRoom(npc);
					}
					if (WorldModify.canSpawn && WorldModify.hiScore > 0)
					{
						Main.npcs[npc].homeTileX = WorldModify.bestX;
						Main.npcs[npc].homeTileY = WorldModify.bestY;
						Main.npcs[npc].homeless = false;
						WorldModify.canSpawn = false;
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
					for (int j = 0; j < WorldModify.numRoomTiles; j++)
					{
						if (Main.npcs[i].homeTileX == WorldModify.roomX[j] && Main.npcs[i].homeTileY == WorldModify.roomY[j])
						{
							bool flag = false;
							for (int k = 0; k < WorldModify.numRoomTiles; k++)
							{
								if (Main.npcs[i].homeTileX == WorldModify.roomX[k] && Main.npcs[i].homeTileY - 1 == WorldModify.roomY[k])
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								WorldModify.hiScore = -1;
								return;
							}
						}
					}
				}
			}
			WorldModify.hiScore = 0;
			int num = 0;
			int num2 = 0;
			int num3 = WorldModify.roomX1 - NPC.sWidth / 2 / 16 - 1 - 21;
			int num4 = WorldModify.roomX2 + NPC.sWidth / 2 / 16 + 1 + 21;
			int num5 = WorldModify.roomY1 - NPC.sHeight / 2 / 16 - 1 - 21;
			int num6 = WorldModify.roomY2 + NPC.sHeight / 2 / 16 + 1 + 21;
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
				WorldModify.hiScore = num7;
				return;
			}
			num3 = WorldModify.roomX1;
			num4 = WorldModify.roomX2;
			num5 = WorldModify.roomY1;
			num6 = WorldModify.roomY2;
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
										else if (Main.tile.At(num9, num10).Type == 10 || Main.tile.At(num9, num10).Type == 11)
										{
											num -= 20;
										}
										else if (Main.tileSolid[(int)Main.tile.At(num9, num10).Type])
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
							if (num > WorldModify.hiScore)
							{
								bool flag2 = false;
								for (int num11 = 0; num11 < WorldModify.numRoomTiles; num11++)
								{
									if (WorldModify.roomX[num11] == n && WorldModify.roomY[num11] == num8)
									{
										flag2 = true;
										break;
									}
								}
								if (flag2)
								{
									WorldModify.hiScore = num;
									WorldModify.bestX = n;
									WorldModify.bestY = num8;
								}
							}
						}
					}
				}
			}
		}
		
		public static bool StartRoomCheck(int x, int y)
		{
			WorldModify.roomX1 = x;
			WorldModify.roomX2 = x;
			WorldModify.roomY1 = y;
			WorldModify.roomY2 = y;
			WorldModify.numRoomTiles = 0;
            for (int i = 0; i < MAX_TILE_SETS; i++)
			{
				WorldModify.houseTile[i] = false;
			}
			WorldModify.canSpawn = true;
			if (Main.tile.At(x, y).Active && Main.tileSolid[(int)Main.tile.At(x, y).Type])
			{
				WorldModify.canSpawn = false;
			}
			WorldModify.CheckRoom(x, y);
			if (WorldModify.numRoomTiles < 60)
			{
				WorldModify.canSpawn = false;
			}
			return WorldModify.canSpawn;
		}
		
		public static void CheckRoom(int x, int y)
		{
			if (!WorldModify.canSpawn)
			{
				return;
			}
			if (x < 10 || y < 10 || x >= Main.maxTilesX - 10 || y >= WorldModify.lastMaxTilesY - 10)
			{
				WorldModify.canSpawn = false;
				return;
			}
			for (int i = 0; i < WorldModify.numRoomTiles; i++)
			{
				if (WorldModify.roomX[i] == x && WorldModify.roomY[i] == y)
				{
					return;
				}
			}
			WorldModify.roomX[WorldModify.numRoomTiles] = x;
			WorldModify.roomY[WorldModify.numRoomTiles] = y;
			WorldModify.numRoomTiles++;
			if (WorldModify.numRoomTiles >= WorldModify.maxRoomTiles)
			{
				WorldModify.canSpawn = false;
				return;
			}
			if (Main.tile.At(x, y).Active)
			{
				WorldModify.houseTile[(int)Main.tile.At(x, y).Type] = true;
				if (Main.tileSolid[(int)Main.tile.At(x, y).Type] || Main.tile.At(x, y).Type == 11)
				{
					return;
				}
			}
			if (x < WorldModify.roomX1)
			{
				WorldModify.roomX1 = x;
			}
			if (x > WorldModify.roomX2)
			{
				WorldModify.roomX2 = x;
			}
			if (y < WorldModify.roomY1)
			{
				WorldModify.roomY1 = y;
			}
			if (y > WorldModify.roomY2)
			{
				WorldModify.roomY2 = y;
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
				WorldModify.canSpawn = false;
				return;
			}
			for (int k = x - 1; k < x + 2; k++)
			{
				for (int l = y - 1; l < y + 2; l++)
				{
					if ((k != x || l != y) && WorldModify.canSpawn)
					{
						WorldModify.CheckRoom(k, l);
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
						flag = WorldModify.meteor(num7, k);
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

			WorldModify.stopDrops = true;
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
						WorldModify.KillTile(num6, num7, false, false, false);
					}
					WorldModify.SquareTileFrame(num6, num7, true);
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
							WorldModify.KillTile(num8, num9, false, false, false);
						}
						Main.tile.At(num8, num9).SetType (37);
						WorldModify.SquareTileFrame(num8, num9, true);
					}
				}
			}
			WorldModify.stopDrops = false;
			
			NetMessage.SendData(25, -1, -1, "A meteorite has landed!", 255, 50f, 255f, 130f);
			NetMessage.SendTileSquare(-1, x, y, 30);
			return true;
		}

		public static void GrowTreeShared(int i, int y, int freeTilesAbove, int minHeight, int maxHeight)
		{
			bool flag = false;
			bool flag2 = false;
			int num3 = WorldModify.genRand.Next(minHeight, maxHeight);
			int num4;
			for (int j = freeTilesAbove - num3; j < freeTilesAbove; j++)
			{
				Main.tile.At(i, j).SetFrameNumber ((byte)WorldModify.genRand.Next(3));
				Main.tile.At(i, j).SetActive (true);
				Main.tile.At(i, j).SetType (5);
				num4 = WorldModify.genRand.Next(3);
				int num5 = WorldModify.genRand.Next(10);
				if (j == freeTilesAbove - 1 || j == freeTilesAbove - num3)
				{
					num5 = 0;
				}
				while (((num5 == 5 || num5 == 7) && flag) || ((num5 == 6 || num5 == 7) && flag2))
				{
					num5 = WorldModify.genRand.Next(10);
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
                switch (num5)
                {
                    case 1:
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
                        break;
					case 2:
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
                        break;
					case 3:
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
                        break;
					case 4:
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
                        break;
					case 5:
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
                        break;
					case 6:
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
                        break;
					case 7:
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
                        break;
					default:
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
                        break;
				}
				if (num5 == 5 || num5 == 7)
				{
					Main.tile.At(i - 1, j).SetActive (true);
					Main.tile.At(i - 1, j).SetType (5);
					num4 = WorldModify.genRand.Next(3);
					if (WorldModify.genRand.Next(3) < 2)
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
					num4 = WorldModify.genRand.Next(3);
					if (WorldModify.genRand.Next(3) < 2)
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
			int num6 = WorldModify.genRand.Next(3);
			if (num6 == 0 || num6 == 1)
			{
				Main.tile.At(i + 1, freeTilesAbove - 1).SetActive (true);
				Main.tile.At(i + 1, freeTilesAbove - 1).SetType (5);
				num4 = WorldModify.genRand.Next(3);
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
				num4 = WorldModify.genRand.Next(3);
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
			num4 = WorldModify.genRand.Next(3);
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
			if (WorldModify.genRand.Next(3) < 2)
			{
				num4 = WorldModify.genRand.Next(3);
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
				num4 = WorldModify.genRand.Next(3);
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
			WorldModify.RangeFrame(i - 2, freeTilesAbove - num3 - 1, i + 2, freeTilesAbove + 1);
			
			NetMessage.SendTileSquare(-1, i, (int)((double)freeTilesAbove - (double)num3 * 0.5), num3 + 1);
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
				if (WorldModify.EmptyTileCheck(x - TREE_RADIUS, x + TREE_RADIUS, freeTilesAbove - 14, freeTilesAbove - 1, 20))
				{
					GrowTreeShared(x, y, freeTilesAbove, 5, 15);
				}
			}
		}

        public static bool IsValidTreeRootTile(TileRef tile)
		{
			return (tile.Active && (tile.Type == 2 || tile.Type == 23 || tile.Type == 60));
		}
		
		public static void GrowShroom(int i, int y)
		{
			if (Main.tile.At(i - 1, y - 1).Lava || Main.tile.At(i - 1, y - 1).Lava || Main.tile.At(i + 1, y - 1).Lava)
			{
				return;
			}
			if (Main.tile.At(i, y).Active && Main.tile.At(i, y).Type == 70 && Main.tile.At(i, y - 1).Wall == 0 && Main.tile.At(i - 1, y).Active && Main.tile.At(i - 1, y).Type == 70 && Main.tile.At(i + 1, y).Active && Main.tile.At(i + 1, y).Type == 70 && WorldModify.EmptyTileCheck(i - 2, i + 2, y - 13, y - 1, 71))
			{
				int num = WorldModify.genRand.Next(4, 11);
				int num2;
				for (int j = y - num; j < y; j++)
				{
					Main.tile.At(i, j).SetFrameNumber ((byte)WorldModify.genRand.Next(3));
					Main.tile.At(i, j).SetActive (true);
					Main.tile.At(i, j).SetType (72);
					num2 = WorldModify.genRand.Next(3);
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
				num2 = WorldModify.genRand.Next(3);
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
				WorldModify.RangeFrame(i - 2, y - num - 1, i + 2, y + 1);
				NetMessage.SendTileSquare(-1, i, (int)((double)y - (double)num * 0.5), num + 1);
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
					Main.tile.At(i, j - 1).SetFrameX ((short)(WorldModify.genRand.Next(3) * 18));
					Main.tile.At(i, j).SetActive (true);
					Main.tile.At(i, j).SetType (10);
					Main.tile.At(i, j).SetFrameY (18);
					Main.tile.At(i, j).SetFrameX ((short)(WorldModify.genRand.Next(3) * 18));
					Main.tile.At(i, j + 1).SetActive (true);
					Main.tile.At(i, j + 1).SetType (10);
					Main.tile.At(i, j + 1).SetFrameY (36);
					Main.tile.At(i, j + 1).SetFrameX ((short)(WorldModify.genRand.Next(3) * 18));
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

		public static bool CloseDoor(int x, int y, bool forced = false, DoorOpener opener = DoorOpener.SERVER, ISender sender = null)
		{
			if (sender == null)
			{
				sender = new ConsoleSender ();
			}

            if (Program.properties.NPCDoorOpenCancel && opener == DoorOpener.NPC)
                return false;

			DoorStateChangeEvent doorEvent = new DoorStateChangeEvent();
			doorEvent.Sender = sender;
			doorEvent.X = x;
			doorEvent.Y = y;
			doorEvent.Direction = 1;
			doorEvent.Opener = opener;
			doorEvent.isOpened = forced;
			Server.PluginManager.processHook(Hooks.DOOR_STATECHANGE, doorEvent);
			if (doorEvent.Cancelled)
			{
                NetMessage.SendData(19, -1, -1, "", 0, (float)x, (float)y, 0); //Inform the client of the update
				return false;
			}

			int num = 0;
			int num2 = x;
			int num3 = y;
			
			int frameX = (int)Main.tile.At(x, y).FrameX;
			int frameY = (int)Main.tile.At(x, y).FrameY;
			if (frameX == 0)
			{
				num2 = x;
				num = 1;
			}
			else if (frameX == 18)
			{
				num2 = x - 1;
				num = 1;
			}
			else if (frameX == 36)
			{
				num2 = x + 1;
				num = -1;
			}
			else if (frameX == 54)
			{
				num2 = x;
				num = -1;
			}
			if (frameY == 0)
			{
				num3 = y;
			}
			else if (frameY == 18)
			{
				num3 = y - 1;
			}
			else if (frameY == 36)
			{
				num3 = y - 2;
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
						Main.tile.At(l, m).SetFrameX ((short)(WorldModify.genRand.Next(3) * 18));
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
					WorldModify.TileFrame(n, num5, false, false);
				}
			}
			return true;
		}

		public static bool OpenDoor(int x, int y, int direction, bool state = false, DoorOpener opener = DoorOpener.SERVER, ISender sender = null)
		{
			if (sender == null)
			{
				sender = new ConsoleSender();
            }

            if (Program.properties.NPCDoorOpenCancel && opener == DoorOpener.NPC)
                return false;

			DoorStateChangeEvent doorEvent = new DoorStateChangeEvent();
			doorEvent.Sender = sender;
			doorEvent.X = x;
			doorEvent.Y = y;
			doorEvent.Direction = direction;
			doorEvent.Opener = opener;
			doorEvent.isOpened = state;
			Server.PluginManager.processHook(Hooks.DOOR_STATECHANGE, doorEvent);
			if (doorEvent.Cancelled)
			{
                NetMessage.SendData(19, -1, -1, "", 1, (float)x, (float)y, 0); //Inform the client of the update
				return false;
			}

			int num = 0;
			if (Main.tile.At(x, y - 1).FrameY == 0 && Main.tile.At(x, y - 1).Type == Main.tile.At(x, y).Type)
			{
				num = y - 1;
			}
			else if (Main.tile.At(x, y - 2).FrameY == 0 && Main.tile.At(x, y - 2).Type == Main.tile.At(x, y).Type)
			{
				num = y - 2;
			}
			else if (Main.tile.At(x, y + 1).FrameY == 0 && Main.tile.At(x, y + 1).Type == Main.tile.At(x, y).Type)
			{
				num = y + 1;
			}
			else
			{
				num = y;
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
					WorldModify.KillTile(num4, k, false, false, false);
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
						WorldModify.TileFrame(l, m, false, false);
					}
				}
			}
			return flag;
		}
		
		public static void Check1x2(int x, int j, byte type)
		{
			if (WorldModify.destroyObject)
			{
				return;
			}
			int num = j;
            bool flag = true;

            int FrameY = (int)Main.tile.At(x, num).FrameY;

            int num2 = 0;
            while (FrameY >= 40)
            {
                FrameY -= 40;
                num2++;
            }

            if (FrameY == 18)
			{
				num--;
			}
            if ((int)Main.tile.At(x, num).FrameY == 40 * num2 && (int)Main.tile.At(x, num + 1).FrameY == 40 * num2 + 18 && Main.tile.At(x, num).Type == type && Main.tile.At(x, num + 1).Type == type)
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
				WorldModify.destroyObject = true;
				if (Main.tile.At(x, num).Type == type)
				{
					WorldModify.KillTile(x, num, false, false, false);
				}
				if (Main.tile.At(x, num + 1).Type == type)
				{
					WorldModify.KillTile(x, num + 1, false, false, false);
				}
				if (type == 15)
				{
                    if (num2 == 1)
                    {
                        Item.NewItem(x * 16, num * 16, 32, 32, 358, 1, false);
                    }
                    else
                    {
                        Item.NewItem(x * 16, num * 16, 32, 32, 34, 1, false);
                    }
				}
				WorldModify.destroyObject = false;
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
						WorldModify.KillTile(x, y, false, false, false);
						return;
					}
				}
				else
				{
					WorldModify.KillTile(x, y, false, false, false);
				}
			}
		}
		
		public static void CheckSign(int x, int y, int type)
		{
			if (WorldModify.destroyObject)
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
				else if (Main.tile.At(num6, num7 + 2).Active && Main.tileSolid[(int)Main.tile.At(num6, num7 + 2).Type] && Main.tile.At(num6 + 1, num7 + 2).Active && Main.tileSolid[(int)Main.tile.At(num6 + 1, num7 + 2).Type])
				{
					num8 = 0;
				}
				else if (Main.tile.At(num6, num7 - 1).Active && Main.tileSolid[(int)Main.tile.At(num6, num7 - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6, num7 - 1).Type] && Main.tile.At(num6 + 1, num7 - 1).Active && Main.tileSolid[(int)Main.tile.At(num6 + 1, num7 - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 + 1, num7 - 1).Type])
				{
					num8 = 1;
				}
				else if (Main.tile.At(num6 - 1, num7).Active && Main.tileSolid[(int)Main.tile.At(num6 - 1, num7).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 - 1, num7).Type] && Main.tile.At(num6 - 1, num7 + 1).Active && Main.tileSolid[(int)Main.tile.At(num6 - 1, num7 + 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 - 1, num7 + 1).Type])
				{
					num8 = 2;
				}
				else if (Main.tile.At(num6 + 2, num7).Active && Main.tileSolid[(int)Main.tile.At(num6 + 2, num7).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 + 2, num7).Type] && Main.tile.At(num6 + 2, num7 + 1).Active && Main.tileSolid[(int)Main.tile.At(num6 + 2, num7 + 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 + 2, num7 + 1).Type])
				{
					num8 = 3;
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldModify.destroyObject = true;
				for (int n = num; n < num2; n++)
				{
					for (int num9 = num3; num9 < num4; num9++)
					{
						if ((int)Main.tile.At(n, num9).Type == type)
						{
							WorldModify.KillTile(n, num9, false, false, false);
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
				WorldModify.destroyObject = false;
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
				else if (Main.tile.At(x, y - 1).Active && Main.tileSolid[(int)Main.tile.At(x, y - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(x, y - 1).Type] && Main.tile.At(x + 1, y - 1).Active && Main.tileSolid[(int)Main.tile.At(x + 1, y - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(x + 1, y - 1).Type])
				{
					num7 = 1;
				}
				else if (Main.tile.At(x - 1, y).Active && Main.tileSolid[(int)Main.tile.At(x - 1, y).Type] && !Main.tileSolidTop[(int)Main.tile.At(x - 1, y).Type] && Main.tile.At(x - 1, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x - 1, y + 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(x - 1, y + 1).Type])
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
			else if (type == 85)
			{
				if (!Main.tile.At(x, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x, y + 1).Type] || !Main.tile.At(x + 1, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x + 1, y + 1).Type])
				{
					return false;
				}
				num6--;
				num7 = 0;
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
		
		public static void PlaceOnTable1x1(int x, int y, int type, int style = 0)
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
                Main.tile.At(x, y).SetFrameX((short)(style * 18));
				Main.tile.At(x, y).SetFrameY (0);
				Main.tile.At(x, y).SetType ((byte)type);
				if (type == 50)
				{
					Main.tile.At(x, y).SetFrameX ((short)(18 * WorldModify.genRand.Next(5)));
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
				else if (style == 1)
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
				else if (style == 2)
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
				else if (style == 3)
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
				else if (style == 4)
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
				else if (style == 5)
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
				if (Main.tile.At(x, y).Type == 82 && WorldModify.genRand.Next(50) == 0)
				{
					Main.tile.At(x, y).SetType (83);
					
					NetMessage.SendTileSquare(-1, x, y, 1);
					WorldModify.SquareTileFrame(x, y, true);
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
			int num = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
			int num2 = 0;
			if (WorldModify.genRand.Next(40) == 0)
			{
				num2 = WorldModify.genRand.Next((int)(Main.rockLayer + (double)Main.maxTilesY) / 2, Main.maxTilesY - 20);
			}
			else if (WorldModify.genRand.Next(10) == 0)
			{
				num2 = WorldModify.genRand.Next(0, Main.maxTilesY - 20);
			}
			else
			{
				num2 = WorldModify.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
			}
			while (num2 < Main.maxTilesY - 20 && !Main.tile.At(num, num2).Active)
			{
				num2++;
			}
			if (Main.tile.At(num, num2).Active && !Main.tile.At(num, num2 - 1).Active && Main.tile.At(num, num2 - 1).Liquid == 0)
			{
				if (Main.tile.At(num, num2).Type == 2)
				{
					WorldModify.PlaceAlch(num, num2 - 1, 0);
				}
				if (Main.tile.At(num, num2).Type == 60)
				{
					WorldModify.PlaceAlch(num, num2 - 1, 1);
				}
				if (Main.tile.At(num, num2).Type == 0 || Main.tile.At(num, num2).Type == 59)
				{
					WorldModify.PlaceAlch(num, num2 - 1, 2);
				}
				if (Main.tile.At(num, num2).Type == 23 || Main.tile.At(num, num2).Type == 25)
				{
					WorldModify.PlaceAlch(num, num2 - 1, 3);
				}
				if (Main.tile.At(num, num2).Type == 53)
				{
					WorldModify.PlaceAlch(num, num2 - 1, 4);
				}
				if (Main.tile.At(num, num2).Type == 57)
				{
					WorldModify.PlaceAlch(num, num2 - 1, 5);
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
				else if (num == 1)
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
				else if (num == 2)
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
				else if (num == 3)
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
				else if (num == 4)
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
						else if (Main.tile.At(x, y).Type == 84)
						{
							Main.tile.At(x, y).SetType (83);
												
							NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
				else if (num == 5)
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
						else if (Main.tile.At(x, y).Type == 84)
						{
							Main.tile.At(x, y).SetType (83);
							NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
			}
			if (flag)
			{
				WorldModify.KillTile(x, y, false, false, false);
			}
		}

        public static void Place1x2(int x, int y, int type, int style)
		{
			short frameX = 0;
			if (type == 20)
			{
				frameX = (short)(WorldModify.genRand.Next(3) * 18);
			}
			if (Main.tile.At(x, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x, y + 1).Type] && !Main.tile.At(x, y - 1).Active)
			{
                short frameHeight = (short)(style * 40);
				Main.tile.At(x, y - 1).SetActive (true);
                Main.tile.At(x, y - 1).SetFrameY(frameHeight);
				Main.tile.At(x, y - 1).SetFrameX (frameX);
				Main.tile.At(x, y - 1).SetType ((byte)type);
				Main.tile.At(x, y).SetActive (true);
                Main.tile.At(x, y).SetFrameY((short)(frameHeight + 18));
				Main.tile.At(x, y).SetFrameX (frameX);
				Main.tile.At(x, y).SetType ((byte)type);
			}
		}

        public static void PlaceBanner(int x, int y, int type, int style = 0)
        {
            int FrameLength = style * 18;
            if (Main.tile.At(x, y - 1).Active && Main.tileSolid[(int)Main.tile.At(x, y - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(x, y - 1).Type] && !Main.tile.At(x, y).Active && !Main.tile.At(x, y + 1).Active && !Main.tile.At(x, y + 2).Active)
            {
                Main.tile.At(x, y).SetActive(true);
                Main.tile.At(x, y).SetFrameY(0);
                Main.tile.At(x, y).SetFrameX((short)FrameLength);
                Main.tile.At(x, y).SetType((byte)type);
                Main.tile.At(x, y + 1).SetActive(true);
                Main.tile.At(x, y + 1).SetFrameY(18);
                Main.tile.At(x, y + 1).SetFrameX((short)FrameLength);
                Main.tile.At(x, y + 1).SetType((byte)type);
                Main.tile.At(x, y + 2).SetActive(true);
                Main.tile.At(x, y + 2).SetFrameY(36);
                Main.tile.At(x, y + 2).SetFrameX((short)FrameLength);
                Main.tile.At(x, y + 2).SetType((byte)type);
            }
        }

        public static void CheckBanner(int x, int j, byte type)
        {
            if (WorldModify.destroyObject)
            {
                return;
            }
            int num = j - (int)(Main.tile.At(x, j).FrameY / 18);
            int frameX = (int)Main.tile.At(x, j).FrameX;
            bool flag = false;
            for (int i = 0; i < 3; i++)
            {
                if (!Main.tile.At(x, num + i).Active)
                {
                    flag = true;
                }
                else
                {
                    if (Main.tile.At(x, num + i).Type != type)
                    {
                        flag = true;
                    }
                    else
                    {
                        if ((int)Main.tile.At(x, num + i).FrameY != i * 18)
                        {
                            flag = true;
                        }
                        else
                        {
                            if ((int)Main.tile.At(x, num + i).FrameX != frameX)
                            {
                                flag = true;
                            }
                        }
                    }
                }
            }
            if (!Main.tile.At(x, num - 1).Active)
            {
                flag = true;
            }
            if (!Main.tileSolid[(int)Main.tile.At(x, num - 1).Type])
            {
                flag = true;
            }
            if (Main.tileSolidTop[(int)Main.tile.At(x, num - 1).Type])
            {
                flag = true;
            }
            if (flag)
            {
                WorldModify.destroyObject = true;
                for (int k = 0; k < 3; k++)
                {
                    if (Main.tile.At(x, num + k).Type == type)
                    {
                        WorldModify.KillTile(x, num + k, false, false, false);
                    }
                }
                if (type == 91)
                {
                    int num2 = frameX / 18;
                    Item.NewItem(x * 16, (num + 1) * 16, 32, 32, 337 + num2, 1, false);
                }
                WorldModify.destroyObject = false;
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

		public static void Check1x2Top(int x, int y, byte type)
		{
			if (WorldModify.destroyObject)
			{
				return;
			}
			
			bool flag = true;
			
			if (Main.tile.At(x, y).FrameY == 18)
			{
				y--;
			}
			
			if (Main.tile.At(x, y).FrameY == 0
				&& Main.tile.At(x, y + 1).FrameY == 18
				&& Main.tile.At(x, y).Type == type
				&& Main.tile.At(x, y + 1).Type == type)
			{
				flag = false;
			}
			
			if (!Main.tile.At(x, y - 1).Active
				|| !Main.tileSolid[(int)Main.tile.At(x, y - 1).Type]
				|| Main.tileSolidTop[(int)Main.tile.At(x, y - 1).Type])
			{
				flag = true;
			}
			
			if (flag)
			{
				WorldModify.destroyObject = true;
				if (Main.tile.At(x, y).Type == type)
				{
					WorldModify.KillTile(x, y, false, false, false);
				}
				if (Main.tile.At(x, y + 1).Type == type)
				{
					WorldModify.KillTile(x, y + 1, false, false, false);
				}
				if (type == 42)
				{
					Item.NewItem(x * 16, y * 16, 32, 32, 136, 1, false);
				}
				WorldModify.destroyObject = false;
			}
		}
		
		public static void Check2x1(int x, int y, byte type)
		{
			if (WorldModify.destroyObject)
			{
				return;
			}
			
			bool flag = true;
			
			if (Main.tile.At(x, y).FrameX == 18)
			{
				x--;
			}
			
			if (Main.tile.At(x, y).FrameX == 0
				&& Main.tile.At(x + 1, y).FrameX == 18
				&& Main.tile.At(x, y).Type == type
				&& Main.tile.At(x + 1, y).Type == type)
			{
				flag = false;
			}
			
			if (type == 29 || type == 103)
			{
				if (!Main.tile.At(x, y + 1).Active || !Main.tileTable[(int)Main.tile.At(x, y + 1).Type])
				{
					flag = true;
				}
				if (!Main.tile.At(x + 1, y + 1).Active || !Main.tileTable[(int)Main.tile.At(x + 1, y + 1).Type])
				{
					flag = true;
				}
			}
			else
			{
				if (!Main.tile.At(x, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x, y + 1).Type])
				{
					flag = true;
				}
				if (!Main.tile.At(x + 1, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x + 1, y + 1).Type])
				{
					flag = true;
				}
			}
			
			if (flag)
			{
				WorldModify.destroyObject = true;
                if (Main.tile.At(x, y).Type == type)
				{
					WorldModify.KillTile(x, y, false, false, false);
				}
                if (Main.tile.At(x + 1, y).Type == type)
				{
					WorldModify.KillTile(x + 1, y, false, false, false);
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
                if (type == 103)
                {
                    Item.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 356, 1, false);
                }
                WorldModify.destroyObject = false;
                WorldModify.SquareTileFrame(x, y, true);
                WorldModify.SquareTileFrame(x + 1, y, true);
			}
		}
		
		public static void Place2x1(int x, int y, int type)
		{
			bool flag = false;
			if (type != 29 && type != 103
				&& Main.tile.At(x, y + 1).Active
				&& Main.tile.At(x + 1, y + 1).Active
				&& Main.tileSolid[(int)Main.tile.At(x, y + 1).Type]
				&& Main.tileSolid[(int)Main.tile.At(x + 1, y + 1).Type]
				&& !Main.tile.At(x, y).Active
				&& !Main.tile.At(x + 1, y).Active)
			{
				flag = true;
			}
			else
			{
				if ((type == 29 || type == 103)
					&& Main.tile.At(x, y + 1).Active
					&& Main.tile.At(x + 1, y + 1).Active
					&& Main.tileTable[(int)Main.tile.At(x, y + 1).Type]
					&& Main.tileTable[(int)Main.tile.At(x + 1, y + 1).Type]
					&& !Main.tile.At(x, y).Active
					&& !Main.tile.At(x + 1, y).Active)
				{
					flag = true;
				}
			}
			
			if (flag)
			{
				Main.tile.At(x, y).SetActive (true);
				Main.tile.At(x, y).SetFrameY (0);
				Main.tile.At(x, y).SetFrameX (0);
				Main.tile.At(x, y).SetType ((byte)type);
				Main.tile.At(x + 1, y).SetActive (true);
				Main.tile.At(x + 1, y).SetFrameY (0);
				Main.tile.At(x + 1, y).SetFrameX (18);
				Main.tile.At(x + 1, y).SetType ((byte)type);
			}
		}
		
		public static void Check4x2(int i, int j, int type)
		{
			if (WorldModify.destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i;
			num += (int)(Main.tile.At(i, j).FrameX / 18 * -1);
			if ((type == 79 || type == 90) && Main.tile.At(i, j).FrameX >= 72)
			{
				num += 4;
			}
			int num2 = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 4; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					int num3 = (k - num) * 18;
                    if ((type == 79 || type == 90) && Main.tile.At(i, j).FrameX >= 72)
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
				WorldModify.destroyObject = true;
				for (int m = num; m < num + 4; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							WorldModify.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 79)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 224, 1, false);
                }
                if (type == 90)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 336, 1, false);
                }
				WorldModify.destroyObject = false;
				for (int num4 = num - 1; num4 < num + 4; num4++)
				{
					for (int num5 = num2 - 1; num5 < num2 + 4; num5++)
					{
						WorldModify.TileFrame(num4, num5, false, false);
					}
				}
			}
		}
		
		public static void Check2x2(int i, int j, int type)
		{
			if (WorldModify.destroyObject)
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
                if (type == 95)
                {
                    if (!Main.tile.At(k, num2 - 1).Active || 
                        !Main.tileSolid[(int)Main.tile.At(k, num2 - 1).Type] || 
                        Main.tileSolidTop[(int)Main.tile.At(k, num2 - 1).Type])
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (!Main.tile.At(k, num2 + 2).Active || 
                        (!Main.tileSolid[(int)Main.tile.At(k, num2 + 2).Type] && 
                        !Main.tileTable[(int)Main.tile.At(k, num2 + 2).Type]))
                    {
                        flag = true;
                    }
                }
			}
			if (flag)
			{
				WorldModify.destroyObject = true;
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							WorldModify.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 85)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 321, 1, false);
				}
                if (type == 94)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 352, 1, false);
                }
                if (type == 95)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 344, 1, false);
                }
                if (type == 96)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 345, 1, false);
                }
                if (type == 97)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 346, 1, false);
                }
                if (type == 98)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 347, 1, false);
                }
                if (type == 99)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 348, 1, false);
                }
                if (type == 100)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 349, 1, false);
                }
				WorldModify.destroyObject = false;
				for (int num3 = num - 1; num3 < num + 3; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 3; num4++)
					{
						WorldModify.TileFrame(num3, num4, false, false);
					}
				}
			}
		}
		
		public static void Check3x2(int i, int j, int type)
		{
			if (WorldModify.destroyObject)
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
				WorldModify.destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							WorldModify.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 14)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 32, 1, false);
				}
				else if (type == 17)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 33, 1, false);
				}
				else if (type == 77)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 221, 1, false);
				}
                else if (type == 86)
			    {
				    Item.NewItem(i * 16, j * 16, 32, 32, 332, 1, false);
			    }
                else if (type == 87)
			    {
				    Item.NewItem(i * 16, j * 16, 32, 32, 333, 1, false);
			    }
                else if (type == 88)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 334, 1, false);
				}
                else if (type == 89)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 335, 1, false);
				}
				WorldModify.destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						WorldModify.TileFrame(num3, num4, false, false);
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

        public static void Place2x2(int x, int superY, int type)
		{
            int y = superY;
            if (type == 95)
            {
                y++;
            }
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
                    if (type == 98 && Main.tile.At(i, j).Liquid > 0)
                    {
                        flag = false;
                    }
				}
                if (type == 95)
                {
                    if (!Main.tile.At(i, y - 2).Active || 
                        !Main.tileSolid[(int)Main.tile.At(i, y - 2).Type] || 
                        Main.tileSolidTop[(int)Main.tile.At(i, y - 2).Type])
                    {
                        flag = false;
                    }
                }
                else
                {
                    if (!Main.tile.At(i, y + 1).Active || (
                        !Main.tileSolid[(int)Main.tile.At(i, y + 1).Type] && 
                        !Main.tileTable[(int)Main.tile.At(i, y + 1).Type]))
                    {
                        flag = false;
                    }
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
			if (WorldModify.destroyObject)
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
			if (type == 106)
			{
				for (int m = num; m < num + 3; m++)
				{
					if (!Main.tile.At(m, num2 + 3).Active || !Main.tileSolid[(int)Main.tile.At(m, num2 + 3).Type])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				if (!Main.tile.At(num + 1, num2 - 1).Active || !Main.tileSolid[(int)Main.tile.At(num + 1, num2 - 1).Type] || Main.tileSolidTop[(int)Main.tile.At(num + 1, num2 - 1).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldModify.destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							WorldModify.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 34)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 106, 1, false);
				}
				else if (type == 35)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 107, 1, false);
				}
				else if (type == 36)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 108, 1, false);
				}
				else
				{
					if (type == 106)
					{
						Item.NewItem(i * 16, j * 16, 32, 32, 363, 1, false);
					}
				}
				WorldModify.destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						WorldModify.TileFrame(num3, num4, false, false);
					}
				}
			}
		}
		
		public static void Place3x3(int x, int y, int type)
		{
			bool flag = true;
			int num = 0;
			if (type == 106)
			{
				num = -2;
				for (int i = x - 1; i < x + 2; i++)
				{
					for (int j = y - 2; j < y + 1; j++)
					{
						if (Main.tile.At(i, j).Active)
						{
							flag = false;
						}
					}
				}
				for (int k = x - 1; k < x + 2; k++)
				{
					if (!Main.tile.At(k, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(k, y + 1).Type])
					{
						flag = false;
						break;
					}
				}
			}
			else
			{
				for (int l = x - 1; l < x + 2; l++)
				{
					for (int m = y; m < y + 3; m++)
					{
						if (Main.tile.At(l, m).Active)
						{
							flag = false;
						}
					}
				}
				if (!Main.tile.At(x, y - 1).Active || !Main.tileSolid[(int)Main.tile.At(x, y - 1).Type] || Main.tileSolidTop[(int)Main.tile.At(x, y - 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				Main.tile.At(x - 1, y + num).SetActive (true);
				Main.tile.At(x - 1, y + num).SetFrameY(0);
				Main.tile.At(x - 1, y + num).SetFrameX(0);
				Main.tile.At(x - 1, y + num).SetType((byte)type);
				Main.tile.At(x, y + num).SetActive(true);
				Main.tile.At(x, y + num).SetFrameY(0);
				Main.tile.At(x, y + num).SetFrameX(18);
				Main.tile.At(x, y + num).SetType((byte)type);
				Main.tile.At(x + 1, y + num).SetActive(true);
				Main.tile.At(x + 1, y + num).SetFrameY(0);
				Main.tile.At(x + 1, y + num).SetFrameX(36);
				Main.tile.At(x + 1, y + num).SetType((byte)type);
				Main.tile.At(x - 1, y + 1 + num).SetActive(true);
				Main.tile.At(x - 1, y + 1 + num).SetFrameY(18);
				Main.tile.At(x - 1, y + 1 + num).SetFrameX(0);
				Main.tile.At(x - 1, y + 1 + num).SetType((byte)type);
				Main.tile.At(x, y + 1 + num).SetActive(true);
				Main.tile.At(x, y + 1 + num).SetFrameY(18);
				Main.tile.At(x, y + 1 + num).SetFrameX(18);
				Main.tile.At(x, y + 1 + num).SetType((byte)type);
				Main.tile.At(x + 1, y + 1 + num).SetActive(true);
				Main.tile.At(x + 1, y + 1 + num).SetFrameY(18);
				Main.tile.At(x + 1, y + 1 + num).SetFrameX(36);
				Main.tile.At(x + 1, y + 1 + num).SetType((byte)type);
				Main.tile.At(x - 1, y + 2 + num).SetActive(true);
				Main.tile.At(x - 1, y + 2 + num).SetFrameY(36);
				Main.tile.At(x - 1, y + 2 + num).SetFrameX(0);
				Main.tile.At(x - 1, y + 2 + num).SetType((byte)type);
				Main.tile.At(x, y + 2 + num).SetActive(true);
				Main.tile.At(x, y + 2 + num).SetFrameY(36);
				Main.tile.At(x, y + 2 + num).SetFrameX(18);
				Main.tile.At(x, y + 2 + num).SetType((byte)type);
				Main.tile.At(x + 1, y + 2 + num).SetActive(true);
				Main.tile.At(x + 1, y + 2 + num).SetFrameY(36);
				Main.tile.At(x + 1, y + 2 + num).SetFrameX(36);
				Main.tile.At(x + 1, y + 2 + num).SetType((byte)type);
			}
		}

        public static void Check3x4(int i, int j, int type)
        {
            if (WorldModify.destroyObject)
            {
                return;
            }
            bool flag = false;
            int num = i + (int)(Main.tile.At(i, j).FrameX / 18 * -1);
            int num2 = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
            for (int k = num; k < num + 3; k++)
            {
                for (int l = num2; l < num2 + 4; l++)
                {
                    if (!Main.tile.At(k, l).Active || (int)Main.tile.At(k, l).Type != type || 
                        (int)Main.tile.At(k, l).FrameX != (k - num) * 18 || (int)Main.tile.At(k, l).FrameY != (l - num2) * 18)
                    {
                        flag = true;
                    }
                }
                if (!Main.tile.At(k, num2 + 4).Active || !Main.tileSolid[(int)Main.tile.At(k, num2 + 4).Type])
                {
                    flag = true;
                }
            }
            if (flag)
            {
                WorldModify.destroyObject = true;
                for (int m = num; m < num + 3; m++)
                {
                    for (int n = num2; n < num2 + 4; n++)
                    {
                        if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
                        {
                            WorldModify.KillTile(m, n, false, false, false);
                        }
                    }
                }
                if (type == 101)
                {
                    Item.NewItem(i * 16, j * 16, 32, 32, 354, 1, false);
                }
                else
                {
                    if (type == 102)
                    {
                        Item.NewItem(i * 16, j * 16, 32, 32, 355, 1, false);
                    }
                }
                WorldModify.destroyObject = false;
                for (int num3 = num - 1; num3 < num + 4; num3++)
                {
                    for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
                    {
                        WorldModify.TileFrame(num3, num4, false, false);
                    }
                }
            }
        }

        public static void Check1xX(int x, int j, byte type)
        {
            if (WorldModify.destroyObject)
            {
                return;
            }
            int num = j - (int)(Main.tile.At(x, j).FrameY / 18);
            int frameX = (int)Main.tile.At(x, j).FrameX;
            int num2 = 3;
            if (type == 92)
            {
                num2 = 6;
            }
            bool flag = false;
            for (int i = 0; i < num2; i++)
            {
                if (!Main.tile.At(x, num + i).Active)
                {
                    flag = true;
                }
                else
                {
                    if (Main.tile.At(x, num + i).Type != type)
                    {
                        flag = true;
                    }
                    else
                    {
                        if ((int)Main.tile.At(x, num + i).FrameY != i * 18)
                        {
                            flag = true;
                        }
                        else
                        {
                            if ((int)Main.tile.At(x, num + i).FrameX != frameX)
                            {
                                flag = true;
                            }
                        }
                    }
                }
            }
            if (!Main.tile.At(x, num + num2).Active)
            {
                flag = true;
            }
            if (!Main.tileSolid[(int)Main.tile.At(x, num + num2).Type])
            {
                flag = true;
            }
            if (flag)
            {
                WorldModify.destroyObject = true;
                for (int k = 0; k < num2; k++)
                {
                    if (Main.tile.At(x, num + k).Type == type)
                    {
                        WorldModify.KillTile(x, num + k, false, false, false);
                    }
                }
                if (type == 92)
                {
                    Item.NewItem(x * 16, j * 16, 32, 32, 341, 1, false);
                }
                if (type == 93)
                {
                    Item.NewItem(x * 16, j * 16, 32, 32, 342, 1, false);
                }
                WorldModify.destroyObject = false;
            }
        }

        public static void Check2xX(int i, int j, byte type)
        {
            if (WorldModify.destroyObject)
            {
                return;
            }
            int num = i;
            if (Main.tile.At(i, j).FrameX == 18)
            {
                num--;
            }
            int num2 = j - (int)(Main.tile.At(num, j).FrameY / 18);
            int frameX = (int)Main.tile.At(num, num2).FrameX;
            int num3 = 3;
            if (type == 104)
            {
                num3 = 5;
            }
            bool flag = false;
            for (int k = 0; k < num3; k++)
            {
                if (!Main.tile.At(num, num2 + k).Active)
                {
                    flag = true;
                }
                else
                {
                    if (Main.tile.At(num, num2 + k).Type != type)
                    {
                        flag = true;
                    }
                    else
                    {
                        if ((int)Main.tile.At(num, num2 + k).FrameY != k * 18)
                        {
                            flag = true;
                        }
                        else
                        {
                            if ((int)Main.tile.At(num, num2 + k).FrameX != frameX)
                            {
                                flag = true;
                            }
                        }
                    }
                }
                if (!Main.tile.At(num + 1, num2 + k).Active)
                {
                    flag = true;
                }
                else
                {
                    if (Main.tile.At(num + 1, num2 + k).Type != type)
                    {
                        flag = true;
                    }
                    else
                    {
                        if ((int)Main.tile.At(num + 1, num2 + k).FrameY != k * 18)
                        {
                            flag = true;
                        }
                        else
                        {
                            if ((int)Main.tile.At(num + 1, num2 + k).FrameX != frameX + 18)
                            {
                                flag = true;
                            }
                        }
                    }
                }
            }
            if (!Main.tile.At(num, num2 + num3).Active)
            {
                flag = true;
            }
            if (!Main.tileSolid[(int)Main.tile.At(num, num2 + num3).Type])
            {
                flag = true;
            }
            if (!Main.tile.At(num + 1, num2 + num3).Active)
            {
                flag = true;
            }
            if (!Main.tileSolid[(int)Main.tile.At(num + 1, num2 + num3).Type])
            {
                flag = true;
            }
            if (flag)
            {
                WorldModify.destroyObject = true;
                for (int l = 0; l < num3; l++)
                {
                    if (Main.tile.At(num, num2 + l).Type == type)
                    {
                        WorldModify.KillTile(num, num2 + l, false, false, false);
                    }
                    if (Main.tile.At(num + 1, num2 + l).Type == type)
                    {
                        WorldModify.KillTile(num + 1, num2 + l, false, false, false);
                    }
                }
                if (type == 104)
                {
                    Item.NewItem(num * 16, j * 16, 32, 32, 359, 1, false);
                }
                if (type == 105)
                {
                    Item.NewItem(num * 16, j * 16, 32, 32, 360, 1, false);
                }
                WorldModify.destroyObject = false;
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
						int num = k * 18 + WorldModify.genRand.Next(3) * 36;
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
			if (WorldModify.destroyObject)
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
				WorldModify.destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 4; num3++)
					{
						if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
						{
							WorldModify.KillTile(num2, num3, false, false, false);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, 63, 1, false);
				WorldModify.destroyObject = false;
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
						int num = k * 18 + WorldModify.genRand.Next(3) * 36;
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
				WorldModify.KillTile(i, j, false, false, false);
				return true;
			}
			if (i != num2)
			{
				if ((!Main.tile.At(i, j + 1).Active || Main.tile.At(i, j + 1).Type != 80) && (!Main.tile.At(i - 1, j).Active || Main.tile.At(i - 1, j).Type != 80) && (!Main.tile.At(i + 1, j).Active || Main.tile.At(i + 1, j).Type != 80))
				{
					WorldModify.KillTile(i, j, false, false, false);
					return true;
				}
			}
			else if (i == num2 && (!Main.tile.At(i, j + 1).Active || (Main.tile.At(i, j + 1).Type != 80 && Main.tile.At(i, j + 1).Type != 53)))
			{
				WorldModify.KillTile(i, j, false, false, false);
				return true;
			}
			return false;
		}
		
		public static void PlantCactus(int i, int j)
		{
			WorldModify.GrowCactus(i, j);
			for (int k = 0; k < 150; k++)
			{
				int i2 = WorldModify.genRand.Next(i - 1, i + 2);
				int j2 = WorldModify.genRand.Next(j - 10, j + 2);
				WorldModify.GrowCactus(i2, j2);
			}
		}
		
		public static void CactusFrame(int i, int j)
		{
			try
			{
				int num = j;
				int num2 = i;
				if (!WorldModify.CheckCactus(i, j))
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
							else if (num4 == 80 && num8 != 80 && type != 80)
							{
								num10 = 72;
								num11 = 0;
							}
							else if (num5 == 80 && num9 != 80)
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
						else if (num4 == 80 && num5 == 80 && num8 != 80 && num9 != 80 && type != 80)
						{
							num10 = 90;
							num11 = 36;
						}
						else if (num4 == 80 && num8 != 80 && type != 80)
						{
							num10 = 72;
							num11 = 36;
						}
						else if (num5 == 80 && num9 != 80)
						{
							num10 = 18;
							num11 = 36;
						}
						else if (num7 >= 0 && Main.tileSolid[num7])
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
					else if (num3 == -1)
					{
						if (num5 == 80)
						{
							if (num6 != 80 && num7 != 80)
							{
								num10 = 108;
								num11 = 36;
							}
							else if (num7 != 80)
							{
								num10 = 54;
								num11 = 36;
							}
							else if (num6 != 80)
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
						else if (num6 != 80)
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
					else if (num3 == 1)
					{
						if (num4 == 80)
						{
							if (num6 != 80 && num7 != 80)
							{
								num10 = 108;
								num11 = 16;
							}
							else if (num7 != 80)
							{
								num10 = 36;
								num11 = 36;
							}
							else if (num6 != 80)
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
						else if (num6 != 80)
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
					if (num10 != Main.tile.At(i, j).FrameX || num11 != Main.tile.At(i, j).FrameY)
					{
						Main.tile.At(i, j).SetFrameX (num10);
						Main.tile.At(i, j).SetFrameY (num11);
						WorldModify.SquareTileFrame(i, j, true);
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
					WorldModify.SquareTileFrame(num2, num - 1, true);
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
				if (num8 < WorldModify.genRand.Next(11, 13))
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
							WorldModify.SquareTileFrame(num2, num - 1, true);
							
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
							int num9 = WorldModify.genRand.Next(3);
							if (num9 == 0 && flag)
							{
								Main.tile.At(num2 - 1, num).SetActive (true);
								Main.tile.At(num2 - 1, num).SetType (80);
								WorldModify.SquareTileFrame(num2 - 1, num, true);
								NetMessage.SendTileSquare(-1, num2 - 1, num, 1);
								return;
							}
							else if (num9 == 1 && flag2)
							{
								Main.tile.At(num2 + 1, num).SetActive (true);
								Main.tile.At(num2 + 1, num).SetType (80);
								WorldModify.SquareTileFrame(num2 + 1, num, true);
								NetMessage.SendTileSquare(-1, num2 + 1, num, 1);

								return;
							}
							else
							{
								if (num5 >= WorldModify.genRand.Next(2, 8))
								{
									return;
								}
                                //if (Main.tile.At(num2 - 1, num - 1).Active)
                                //{
                                //    byte arg_5E0_0 = Main.tile.At(num2 - 1, num - 1).Type;
                                //}
								if (Main.tile.At(num2 + 1, num - 1).Active && Main.tile.At(num2 + 1, num - 1).Type == 80)
								{
									return;
								}
								Main.tile.At(num2, num - 1).SetActive (true);
								Main.tile.At(num2, num - 1).SetType (80);
								WorldModify.SquareTileFrame(num2, num - 1, true);
								NetMessage.SendTileSquare(-1, num2, num - 1, 1);
								return;
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
						WorldModify.SquareTileFrame(num2, num - 1, true);
						
						NetMessage.SendTileSquare(-1, num2, num - 1, 1);
						return;
					}
				}
			}
		}
		
		public static void CheckPot(int i, int j, int type = 28)
		{
			if (WorldModify.destroyObject)
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
				WorldModify.destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 2; num3++)
					{
						if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
						{
							WorldModify.KillTile(num2, num3, false, false, false);
						}
					}
				}
				if (WorldModify.genRand.Next(50) == 0)
				{
					if ((double)j < Main.worldSurface)
					{
						int num4 = WorldModify.genRand.Next(4);
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
					else if ((double)j < Main.rockLayer)
					{
						int num5 = WorldModify.genRand.Next(7);
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
					else if (j < Main.maxTilesY - 200)
					{
						int num6 = WorldModify.genRand.Next(10);
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
						int num7 = WorldModify.genRand.Next(12);
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
				else
				{
					int num8 = Main.rand.Next(10);
					if (num8 == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLife < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLifeMax)
					{
						Item.NewItem(i * 16, j * 16, 16, 16, 58, 1, false);
					}
					else if (num8 == 1 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statMana < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statManaMax)
			        {
				        Item.NewItem(i * 16, j * 16, 16, 16, 184, 1, false);
			        }
			        else if (num8 == 2)
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
					else if (num8 == 3)
					{
						int stack2 = Main.rand.Next(8) + 3;
						int type2 = 40;
						if (j > Main.maxTilesY - 200)
						{
							type2 = 265;
						}
						Item.NewItem(i * 16, j * 16, 16, 16, type2, stack2, false);
					}
					else if (num8 == 4)
					{
						int type3 = 28;
						if (j > Main.maxTilesY - 200)
						{
							type3 = 188;
						}
						Item.NewItem(i * 16, j * 16, 16, 16, type3, 1, false);
					}
					else if (num8 == 5 && (double)j > Main.rockLayer)
					{
						int stack3 = Main.rand.Next(4) + 1;
						Item.NewItem(i * 16, j * 16, 16, 16, 166, stack3, false);
					}
					else
					{
						float num9 = (float)(200 + WorldModify.genRand.Next(-100, 101));
						if ((double)j < Main.worldSurface)
						{
							num9 *= 0.5f;
						}
						else if ((double)j < Main.rockLayer)
					    {
						    num9 *= 0.75f;
					    }
					    else if (j > Main.maxTilesY - 250)
					    {
						    num9 *= 1.25f;
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
							else if (num9 > 10000f)
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
							else if (num9 > 100f)
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
				WorldModify.destroyObject = false;
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
			if (WorldModify.destroyObject)
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
				if (Main.tile.At(i, j).FrameX >= 216)
				{
					type2 = 348;
				}
                else if (Main.tile.At(i, j).FrameX >= 180)
                {
                    type2 = 343;
                }
                else if (Main.tile.At(i, j).FrameX >= 108)
                {
                    type2 = 328;
                }
                else if (Main.tile.At(i, j).FrameX >= 36)
                {
                    type2 = 306;
                }
				WorldModify.destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 3; num3++)
					{
						if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
						{
							Chest.DestroyChest(num2, num3);
							WorldModify.KillTile(num2, num3, false, false, false);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, type2, 1, false);
				WorldModify.destroyObject = false;
			}
		}

        public static void Place1xX(int x, int y, int type, int style = 0)
        {
            int num = style * 18;
            int num2 = 3;
            if (type == 92)
            {
                num2 = 6;
            }
            bool flag = true;
            for (int i = y - num2 + 1; i < y + 1; i++)
            {
                if (Main.tile.At(x, i).Active)
                {
                    flag = false;
                }
                if (type == 93 && Main.tile.At(x, i).Liquid > 0)
                {
                    flag = false;
                }
            }
            if (flag && Main.tile.At(x, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x, y + 1).Type])
            {
                for (int j = 0; j < num2; j++)
                {
                    Main.tile.At(x, y - num2 + 1 + j).SetActive(true);
                    Main.tile.At(x, y - num2 + 1 + j).SetFrameY((short)(j * 18));
                    Main.tile.At(x, y - num2 + 1 + j).SetFrameX((short)num);
                    Main.tile.At(x, y - num2 + 1 + j).SetType((byte)type);
                }
            }
        }

        public static void Place2xX(int x, int y, int type, int style = 0)
        {
            int num = style * 18;
            int num2 = 3;
            if (type == 104)
            {
                num2 = 5;
            }
            bool flag = true;
            for (int i = y - num2 + 1; i < y + 1; i++)
            {
                if (Main.tile.At(x, i).Active)
                {
                    flag = false;
                }
                if (Main.tile.At(x + 1, i).Active)
                {
                    flag = false;
                }
            }
            if (flag && Main.tile.At(x, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x, y + 1).Type] &&
                Main.tile.At(x + 1, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x + 1, y + 1).Type])
            {
                for (int j = 0; j < num2; j++)
                {
                    Main.tile.At(x, y - num2 + 1 + j).SetActive(true);
                    Main.tile.At(x, y - num2 + 1 + j).SetFrameY((short)(j * 18));
                    Main.tile.At(x, y - num2 + 1 + j).SetFrameX((short)num);
                    Main.tile.At(x, y - num2 + 1 + j).SetType((byte)type);
                    Main.tile.At(x + 1, y - num2 + 1 + j).SetActive(true);
                    Main.tile.At(x + 1, y - num2 + 1 + j).SetFrameY((short)(j * 18));
                    Main.tile.At(x + 1, y - num2 + 1 + j).SetFrameX((short)(num + 18));
                    Main.tile.At(x + 1, y - num2 + 1 + j).SetType((byte)type);
                }
            }
        }
        
        public static void Place3x4(int x, int y, int type)
        {
            if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
            {
                return;
            }
            bool flag = true;
            for (int i = x - 1; i < x + 2; i++)
            {
                for (int j = y - 3; j < y + 1; j++)
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
                for (int k = -3; k <= 0; k++)
                {
                    short frameY = (short)((3 + k) * 18);
                    Main.tile.At(x - 1, y + k).SetActive(true);
                    Main.tile.At(x - 1, y + k).SetFrameY(frameY);
                    Main.tile.At(x - 1, y + k).SetFrameX(0);
                    Main.tile.At(x - 1, y + k).SetType((byte)type);
                    Main.tile.At(x, y + k).SetActive(true);
                    Main.tile.At(x, y + k).SetFrameY(frameY);
                    Main.tile.At(x, y + k).SetFrameX(18);
                    Main.tile.At(x, y + k).SetType((byte)type);
                    Main.tile.At(x + 1, y + k).SetActive(true);
                    Main.tile.At(x + 1, y + k).SetFrameY(frameY);
                    Main.tile.At(x + 1, y + k).SetFrameX(36);
                    Main.tile.At(x + 1, y + k).SetType((byte)type);
                }
            }
        }
        
		public static bool PlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1, int style = 0)
		{
            if (type >= MAX_TILE_SETS)
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
							if (type == 24 && WorldModify.genRand.Next(13) == 0)
							{
								Main.tile.At(i, j).SetActive (true);
								Main.tile.At(i, j).SetType (32);
								WorldModify.SquareTileFrame(i, j, true);
							}
							else if (Main.tile.At(i, j + 1).Type == 78)
							{
								Main.tile.At(i, j).SetActive (true);
								Main.tile.At(i, j).SetType ((byte)type);
								Main.tile.At(i, j).SetFrameX ((short)(WorldModify.genRand.Next(2) * 18 + 108));
							}
							else if (Main.tile.At(i, j).Wall == 0 && Main.tile.At(i, j + 1).Wall == 0)
							{
								if (WorldModify.genRand.Next(50) == 0 || (type == 24 && WorldModify.genRand.Next(40) == 0))
								{
									Main.tile.At(i, j).SetActive (true);
									Main.tile.At(i, j).SetType ((byte)type);
									Main.tile.At(i, j).SetFrameX (144);
								}
								else if (WorldModify.genRand.Next(35) == 0)
							    {
								    Main.tile.At(i, j).SetActive (true);
								    Main.tile.At(i, j).SetType ((byte)type);
								    Main.tile.At(i, j).SetFrameX ((short)(WorldModify.genRand.Next(2) * 18 + 108));
							    }
							    else
							    {
								    Main.tile.At(i, j).SetActive (true);
								    Main.tile.At(i, j).SetType ((byte)type);
								    Main.tile.At(i, j).SetFrameX ((short)(WorldModify.genRand.Next(6) * 18));
							    }
							}
						}
					}
					else if (type == 61)
					{
						if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 60)
						{
							if (WorldModify.genRand.Next(10) == 0 && (double)j > Main.worldSurface)
							{
								Main.tile.At(i, j).SetActive (true);
								Main.tile.At(i, j).SetType (69);
								WorldModify.SquareTileFrame(i, j, true);
							}
							else if (WorldModify.genRand.Next(15) == 0 && (double)j > Main.worldSurface)
							{
								Main.tile.At(i, j).SetActive (true);
								Main.tile.At(i, j).SetType ((byte)type);
								Main.tile.At(i, j).SetFrameX (144);
							}
							else if (WorldModify.genRand.Next(1000) == 0 && (double)j > Main.rockLayer)
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
									Main.tile.At(i, j).SetFrameX ((short)(WorldModify.genRand.Next(8) * 18));
								}
								else
								{
									Main.tile.At(i, j).SetFrameX ((short)(WorldModify.genRand.Next(6) * 18));
								}
							}
						}
					}
					else if (type == 71)
					{
						if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 70)
						{
							Main.tile.At(i, j).SetActive (true);
							Main.tile.At(i, j).SetType ((byte)type);
							Main.tile.At(i, j).SetFrameX ((short)(WorldModify.genRand.Next(5) * 18));
						}
					}
					else if (type == 4)
					{
						if ((Main.tile.At(i - 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i - 1, j).Type] || (Main.tile.At(i - 1, j).Type == 5 && Main.tile.At(i - 1, j - 1).Type == 5 && Main.tile.At(i - 1, j + 1).Type == 5))) || (Main.tile.At(i + 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i + 1, j).Type] || (Main.tile.At(i + 1, j).Type == 5 && Main.tile.At(i + 1, j - 1).Type == 5 && Main.tile.At(i + 1, j + 1).Type == 5))) || (Main.tile.At(i, j + 1).Active && Main.tileSolid[(int)Main.tile.At(i, j + 1).Type]))
						{
							Main.tile.At(i, j).SetActive (true);
							Main.tile.At(i, j).SetType ((byte)type);
							WorldModify.SquareTileFrame(i, j, true);
						}
					}
					else if (type == 10)
					{
						if (!Main.tile.At(i, j - 1).Active && !Main.tile.At(i, j - 2).Active && Main.tile.At(i, j - 3).Active && Main.tileSolid[(int)Main.tile.At(i, j - 3).Type])
						{
							WorldModify.PlaceDoor(i, j - 1, type);
							WorldModify.SquareTileFrame(i, j, true);
						}
						else
						{
							if (Main.tile.At(i, j + 1).Active || Main.tile.At(i, j + 2).Active || !Main.tile.At(i, j + 3).Active || !Main.tileSolid[(int)Main.tile.At(i, j + 3).Type])
							{
								return false;
							}
							WorldModify.PlaceDoor(i, j + 1, type);
							WorldModify.SquareTileFrame(i, j, true);
						}
					}
					else if (type == 34 || type == 35 || type == 36 || type == 106)
					{
						WorldModify.Place3x3(i, j, type);
						WorldModify.SquareTileFrame(i, j, true);
					}
					else if (type == 13 || type == 33 || type == 49 || type == 50 || type == 78)
					{
                        WorldModify.PlaceOnTable1x1(i, j, type, style);
						WorldModify.SquareTileFrame(i, j, true);
					}
                    else if (type == 14 || type == 26 || type == 86 || type == 87 || type == 88 || type == 89)
					{
						WorldModify.Place3x2(i, j, type);
						WorldModify.SquareTileFrame(i, j, true);
					}
					else if (type == 20)
					{
						if (Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 2)
						{
							WorldModify.Place1x2(i, j, type, style);
							WorldModify.SquareTileFrame(i, j, true);
						}
					}
					else if (type == 15)
					{
                        WorldModify.Place1x2(i, j, type, style);
						WorldModify.SquareTileFrame(i, j, true);
					}
                    else if (type == 16 || type == 18 || type == 29 || type == 103)
					{
						WorldModify.Place2x1(i, j, type);
						WorldModify.SquareTileFrame(i, j, true);
					}
                    else if (type == 92 || type == 93)
                    {
                        WorldModify.Place1xX(i, j, type, 0);
                        WorldModify.SquareTileFrame(i, j, true);
                    }
                    else if (type == 104 || type == 105)
                    {
                        WorldModify.Place2xX(i, j, type, 0);
                        WorldModify.SquareTileFrame(i, j, true);
                    }
					else if (type == 17 || type == 77)
					{
						WorldModify.Place3x2(i, j, type);
						WorldModify.SquareTileFrame(i, j, true);
					}
					else if (type == 21)
					{
						WorldModify.PlaceChest(i, j, type, false, style);
						WorldModify.SquareTileFrame(i, j, true);
					}
                    else if (type == 91)
                    {
                        WorldModify.PlaceBanner(i, j, type, style);
                        WorldModify.SquareTileFrame(i, j, true);
                    }
                    else if (type == 101 || type == 102)
                    {
                        WorldModify.Place3x4(i, j, type);
                        WorldModify.SquareTileFrame(i, j, true);
                    }
					else if (type == 27)
					{
						WorldModify.PlaceSunflower(i, j, 27);
						WorldModify.SquareTileFrame(i, j, true);
					}
					else if (type == 28)
					{
						WorldModify.PlacePot(i, j, 28);
						WorldModify.SquareTileFrame(i, j, true);
					}
					else if (type == 42)
					{
						WorldModify.Place1x2Top(i, j, type);
						WorldModify.SquareTileFrame(i, j, true);
					}
					else if (type == 55 || type == 85)
					{
						WorldModify.PlaceSign(i, j, type);
                    }
                    else if (Main.tileAlch[type])
                    {
                        WorldModify.PlaceAlch(i, j, style);
                    }
                    else if (type == 94 || type == 95 || type == 96 || type == 97 || type == 98 || type == 99 || type == 100)
                    {
                        WorldModify.Place2x2(i, j, type);
                    }
                    else if (type == 79 || type == 90)
                    {
                        int direction = 1;
                        if (plr > -1)
                        {
                            direction = Main.players[plr].direction;
                        }
                        WorldModify.Place4x2(i, j, type, direction);
                    }
                    else if (type == 81)
                    {
                        Main.tile.At(i, j).SetFrameX((short)(26 * WorldModify.genRand.Next(6)));
                        Main.tile.At(i, j).SetActive(true);
                        Main.tile.At(i, j).SetType((byte)type);
                    }
                    else
                    {
                        Main.tile.At(i, j).SetActive(true);
                        Main.tile.At(i, j).SetType((byte)type);
                    }
					if (Main.tile.At(i, j).Active && !mute)
					{
						WorldModify.SquareTileFrame(i, j, true);
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
                    if (Main.tile.At(i, j).Wall == 14)
					{
						num2 = 330;
					}
					if (Main.tile.At(i, j).Wall == 16)
					{
						num2 = 30;
					}
					if (Main.tile.At(i, j).Wall == 17)
					{
						num2 = 135;
					}
					if (Main.tile.At(i, j).Wall == 18)
					{
						num2 = 138;
					}
					if (Main.tile.At(i, j).Wall == 19)
					{
						num2 = 140;
					}
					if (Main.tile.At(i, j).Wall == 20)
					{
						num2 = 330;
					}
					if (num2 > 0)
					{
						Item.NewItem(i * 16, j * 16, 16, 16, num2, 1, false);
					}
					Main.tile.At(i, j).SetWall (0);
				}
			}
		}
		
		public static void KillTile(int x, int y, bool fail = false, bool effectOnly = false, bool noItem = false, Player player = null)
		{
			if (x >= 0 && y >= 0 && x < Main.maxTilesX && y < Main.maxTilesY)
            {
                TileRef tile = Main.tile.At(x, y);

				if (tile.Active)
				{
					if (y >= 1 && Main.tile.At(x, y - 1).Active && ((Main.tile.At(x, y - 1).Type == 5 && tile.Type != 5) || (Main.tile.At(x, y - 1).Type == 21 && tile.Type != 21) || (Main.tile.At(x, y - 1).Type == 26 && tile.Type != 26) || (Main.tile.At(x, y - 1).Type == 72 && tile.Type != 72) || (Main.tile.At(x, y - 1).Type == 12 && tile.Type != 12)) && (Main.tile.At(x, y - 1).Type != 5 || ((Main.tile.At(x, y - 1).FrameX != 66 || Main.tile.At(x, y - 1).FrameY < 0 || Main.tile.At(x, y - 1).FrameY > 44) && (Main.tile.At(x, y - 1).FrameX != 88 || Main.tile.At(x, y - 1).FrameY < 66 || Main.tile.At(x, y - 1).FrameY > 110) && Main.tile.At(x, y - 1).FrameY < 198)))
					{
						return;
					}
					if (!effectOnly && !WorldModify.stopDrops)
					{
						if (tile.Type == 3)
						{
							if (tile.FrameX == 144)
							{
								Item.NewItem(x * 16, y * 16, 16, 16, 5, 1, false);
							}
						}
						else if (tile.Type == 24)
						{
							if (tile.FrameX == 144)
							{
								Item.NewItem(x * 16, y * 16, 16, 16, 60, 1, false);
							}
						}
                    }
					if (effectOnly)
					{
						return;
					}
					if (fail)
					{
						if (tile.Type == 2 || tile.Type == 23)
						{
							tile.SetType (0);
						}
						if (tile.Type == 60 || tile.Type == 70)
						{
							tile.SetType (59);
						}
						WorldModify.SquareTileFrame(x, y, true);
						return;
					}
					if (tile.Type == 21)
					{
						int l = (int)(tile.FrameX / 18);
						int chestY = y - (int)(tile.FrameY / 18);
						while (l > 1)
						{
							l -= 2;
						}
						l = x - l;
                        if (!Chest.DestroyChest(l, chestY))
						{
							return;
						}
					}
					if (!noItem && !WorldModify.stopDrops)
					{
						int dropItem = 0;
						if (tile.Type == 0 || tile.Type == 2)
						{
							dropItem = 2;
						}
						else if (tile.Type == 1)
				        {
					        dropItem = 3;
				        }
				        else if (tile.Type == 3 || tile.Type == 73)
						{
							if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(x * 16), (float)(y * 16)), 16, 16)].HasItem(281))
							{
								dropItem = 283;
							}
						}
						else if (tile.Type == 4)
						{
							dropItem = 8;
						}
						else if (tile.Type == 5)
						{
							if (tile.FrameX >= 22 && tile.FrameY >= 198)
							{
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num5 = y;
									while ((!Main.tile.At(x, num5).Active || !Main.tileSolid[(int)Main.tile.At(x, num5).Type] || Main.tileSolidTop[(int)Main.tile.At(x, num5).Type]))
									{
										num5++;
									}
									if (Main.tile.At(x, num5).Type == 2)
									{
										dropItem = 27;
									}
									else
									{
										dropItem = 9;
									}
								}
								else
								{
									dropItem = 9;
								}
							}
							else
							{
								dropItem = 9;
							}
						}
						else if (tile.Type == 6)
						{
							dropItem = 11;
						}
						else if (tile.Type == 7)
						{
							dropItem = 12;
						}
						else if (tile.Type == 8)
						{
							dropItem = 13;
						}
						else if (tile.Type == 9)
						{
							dropItem = 14;
						}
						else if (tile.Type == 13)
						{
						    if (tile.FrameX == 18)
						    {
							    dropItem = 28;
						    }
						    else if (tile.FrameX == 36)
							{
								dropItem = 110;
							}
							else if (tile.FrameX == 54)
						    {
							    dropItem = 350;
						    }
                            else if (Main.tile.At(x, y).FrameX == 72)
							{
								dropItem = 351;
							}
							else
							{
								dropItem = 31;
							}
					    }
					    else if (tile.Type == 19)
						{
							dropItem = 94;
						}
						else if (tile.Type == 22)
						{
							dropItem = 56;
						}
						else if (tile.Type == 23)
						{
							dropItem = 2;
						}
						else if (tile.Type == 25)
						{
							dropItem = 61;
						}
						else if (tile.Type == 30)
						{
							dropItem = 9;
						}
						else if (tile.Type == 33)
						{
							dropItem = 105;
						}
						else if (tile.Type == 37)
						{
							dropItem = 116;
						}
						else if (tile.Type == 38)
						{
							dropItem = 129;
						}
						else if (tile.Type == 39)
				        {
					        dropItem = 131;
				        }
				        else if (tile.Type == 40)
						{
							dropItem = 133;
						}
						else if (tile.Type == 41)
						{
							dropItem = 134;
						}
						else if (tile.Type == 43)
					    {
						    dropItem = 137;
					    }
					    else if (tile.Type == 44)
						{
							dropItem = 139;
						}
						else if (tile.Type == 45)
						{
							dropItem = 141;
						}
						else if (tile.Type == 46)
						{
							dropItem = 143;
						}
						else if (tile.Type == 47)
						{
							dropItem = 145;
						}
						else if (tile.Type == 48)
						{
							dropItem = 147;
						}
						else if (tile.Type == 49)
						{
							dropItem = 148;
						}
						else if (tile.Type == 51)
						{
							dropItem = 150;
						}
						else if (tile.Type == 53)
						{
							dropItem = 169;
						}
						else if (tile.Type != 54)
						{
							if (tile.Type == 56)
							{
								dropItem = 173;
							}
							else if (tile.Type == 57)
							{
								dropItem = 172;
							}
							else if (tile.Type == 58)
							{
								dropItem = 174;
							}
							else if (tile.Type == 60)
							{
								dropItem = 176;
							}
							else if (tile.Type == 70)
							{
								dropItem = 176;
							}
							else if (tile.Type == 75)
							{
								dropItem = 192;
							}
							else if (tile.Type == 76)
							{
								dropItem = 214;
							}
							else if (tile.Type == 78)
							{
								dropItem = 222;
							}
							else if (tile.Type == 81)
							{
								dropItem = 275;
							}
							else if (tile.Type == 80)
							{
								dropItem = 276;
							}
							else if (tile.Type == 61 || tile.Type == 74)
							{
                                if (tile.FrameX == 144)
                                {
                                    Item.NewItem(x * 16, y * 16, 16, 16, 331, WorldModify.genRand.Next(1, 3), false);
                                }
                                if (tile.FrameX == 162)
								{
									dropItem = 223;
								}
                                else if (tile.FrameX >= 108 && tile.FrameX <= 126 && WorldModify.genRand.Next(100) == 0)
						        {
							        dropItem = 208;
						        }
						        else if (WorldModify.genRand.Next(100) == 0)
								{
									dropItem = 195;
								}
							}
							else if (tile.Type == 59 || tile.Type == 60)
						    {
							    dropItem = 176;
						    }
						    else if (tile.Type == 71 || tile.Type == 72)
							{
								if (WorldModify.genRand.Next(50) == 0)
								{
									dropItem = 194;
								}
								else if (WorldModify.genRand.Next(2) == 0)
								{
									dropItem = 183;
								}
							}
							else if (tile.Type >= 63 && tile.Type <= 68)
							{
								dropItem = (int)(tile.Type - 63 + 177);
							}
							else if (tile.Type == 50)
							{
								if (tile.FrameX == 90)
								{
									dropItem = 165;
								}
								else
								{
									dropItem = 149;
								}
							}
							else if (Main.tileAlch[(int)tile.Type] && tile.Type > 82)
							{
								int num6 = (int)(tile.FrameX / 18);
								bool flag = false;
								if (tile.Type == 84)
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
								dropItem = 313 + num6;
								if (flag)
								{
									Item.NewItem(x * 16, y * 16, 16, 16, 307 + num6, WorldModify.genRand.Next(1, 4), false);
								}
							}
						}
						if (dropItem > 0)
						{
							Item.NewItem(x * 16, y * 16, 16, 16, dropItem, 1, false);
						}
					}
					tile.SetActive (false);
					if (Main.tileSolid[(int)tile.Type])
					{
						tile.SetLighted (false);
					}
					tile.SetFrameX (-1);
					tile.SetFrameY (-1);
					tile.SetFrameNumber (0);
                    if (tile.Type == 58 && y > Main.maxTilesY - 200)
                    {
                        tile.SetLava(true);
                        tile.SetLiquid(128);
                    }
					tile.SetType (0);
					WorldModify.SquareTileFrame(x, y, true);
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
				bool buffer = false;
				if (Program.properties.BufferLiquidUpdates)
					NetMessage.UseLiquidUpdateBuffer = buffer = true;
				
				try
				{
					Liquid.UpdateLiquid();
				}
				finally
				{
					if (buffer)
					{
						NetMessage.UseLiquidUpdateBuffer = false;
						LiquidUpdateBuffer.FlushQueue ();
					}
				}
				Liquid.skipCount = 0;
			}
			float num = 3E-05f;
			float num2 = 1.5E-05f;
			bool flag = false;
			WorldModify.spawnDelay++;
			if (Main.invasionType > 0)
			{
				WorldModify.spawnDelay = 0;
			}
			if (WorldModify.spawnDelay >= 20)
			{
				flag = true;
				WorldModify.spawnDelay = 0;
				if (WorldModify.spawnNPC != 37)
				{
					for (int i = 0; i < 1000; i++)
					{
                        if (Main.npcs[i].Active && Main.npcs[i].homeless && Main.npcs[i].townNPC)
						{
							WorldModify.spawnNPC = Main.npcs[i].Type;
							break;
						}
					}
				}
			}
			int num3 = 0;

			if (WorldModify.genRand == null)
			{
				WorldModify.genRand = new Random();
			}

            TileRef Tile;
            TileRef Tile2;
			while ((float)num3 < (float)(Main.maxTilesX * Main.maxTilesY) * num)
			{
				int TileX = WorldModify.genRand.Next(10, Main.maxTilesX - 10);
				int TileY = WorldModify.genRand.Next(10, (int)Main.worldSurface - 1);
                Tile = Main.tile.At(TileX, TileY);
                int num6 = TileX - 1;
                int num7 = TileX + 2;
                int rTileY = TileY - 1;
                int num9 = TileY + 2;
				if (num6 < 10)
				{
					num6 = 10;
				}
				if (num7 > Main.maxTilesX - 10)
				{
					num7 = Main.maxTilesX - 10;
				}
				if (rTileY < 10)
				{
					rTileY = 10;
				}
				if (num9 > Main.maxTilesY - 10)
				{
					num9 = Main.maxTilesY - 10;
				}

                Tile2 = Main.tile.At(TileX, rTileY);

				if (true)
				{
					if (Main.tileAlch[(int)Tile.Type])
					{
                        WorldModify.GrowAlch(TileX, TileY);
					}
					if (Tile.Liquid > 32)
					{
						if (Tile.Active && (Tile.Type == 3 || Tile.Type == 20 || Tile.Type == 24 || Tile.Type == 27 || Tile.Type == 73))
						{
                            WorldModify.KillTile(TileX, TileY, false, false, false);
                            NetMessage.SendData(17, -1, -1, "", 0, (float)TileX, (float)TileY);
						}
					}
					else if (Tile.Active)
					{
						if (Tile.Type == 80)
						{
							if (WorldModify.genRand.Next(15) == 0)
							{
                                WorldModify.GrowCactus(TileX, TileY);
							}
						}
						else if (Tile.Type == 53)
						{
                            if (!Tile2.Active)
							{
                                if (TileX < 250 || TileX > Main.maxTilesX - 250)
								{
									if (WorldModify.genRand.Next(500) == 0 && Tile2.Liquid == 255 && Main.tile.At(TileX, rTileY - 1).Liquid == 255 && Main.tile.At(TileX, rTileY - 2).Liquid == 255 && Main.tile.At(TileX, rTileY - 3).Liquid == 255 && Main.tile.At(TileX, rTileY - 4).Liquid == 255)
									{
										WorldModify.PlaceTile(TileX, rTileY, 81, true, false, -1, 0);
										if (Tile2.Active)
										{
											NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
										}
									}
								}
								else if (TileX > 400 && TileX < Main.maxTilesX - 400 && WorldModify.genRand.Next(300) == 0)
								{
									WorldModify.GrowCactus(TileX, TileY);
								}
							}
						}
						else if (Tile.Type == 78)
						{
							if (!Tile2.Active)
							{
								WorldModify.PlaceTile(TileX, rTileY, 3, true, false, -1, 0);
								if (Tile2.Active)
								{
									NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
								}
							}
						}
						else if (Tile.Type == 2 || Tile.Type == 23 || Tile.Type == 32)
						{
							int num10 = (int)Tile.Type;
							if (!Tile2.Active && WorldModify.genRand.Next(12) == 0 && num10 == 2)
							{
								WorldModify.PlaceTile(TileX, rTileY, 3, true, false, -1, 0);
								if (Tile2.Active)
								{
									NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
								}
							}
							if (!Tile2.Active && WorldModify.genRand.Next(10) == 0 && num10 == 23)
							{
								WorldModify.PlaceTile(TileX, rTileY, 24, true, false, -1, 0);
								if (Tile2.Active)
								{
									NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
								}
							}
							bool flag2 = false;
							for (int j = num6; j < num7; j++)
							{
								for (int k = rTileY; k < num9; k++)
								{
									if ((TileX != j || TileY != k) && Main.tile.At(j, k).Active)
									{
										if (num10 == 32)
										{
											num10 = 23;
										}
										if (Main.tile.At(j, k).Type == 0 || (num10 == 23 && Main.tile.At(j, k).Type == 2))
										{
											WorldModify.SpreadGrass(j, k, 0, num10, false);
											if (num10 == 23)
											{
												WorldModify.SpreadGrass(j, k, 2, num10, false);
											}
											if ((int)Main.tile.At(j, k).Type == num10)
											{
												WorldModify.SquareTileFrame(j, k, true);
												flag2 = true;
											}
										}
									}
								}
							}
							if (flag2)
							{
								NetMessage.SendTileSquare(-1, TileX, TileY, 3);
							}
						}
						else if (Tile.Type == 20 && WorldModify.genRand.Next(20) == 0 && !WorldModify.PlayerLOS(TileX, TileY))
						{
							WorldModify.GrowTree(TileX, TileY);
						}
						if (Tile.Type == 3 && WorldModify.genRand.Next(20) == 0 && Tile.FrameX < 144)
						{
							Tile.SetType (73);
							NetMessage.SendTileSquare(-1, TileX, TileY, 3);
						}
						if (Tile.Type == 32 && WorldModify.genRand.Next(3) == 0)
						{
							int num11 = TileX;
							int num12 = TileY;
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
							if (num13 < 3 || Tile.Type == 23)
							{
								int num14 = WorldModify.genRand.Next(4);
								if (num14 == 0)
								{
									num12--;
								}
								else if (num14 == 1)
								{
									num12++;
								}
								else if (num14 == 2)
								{
									num11--;
								}
								else if (num14 == 3)
								{
									num11++;
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
											WorldModify.SquareTileFrame(num11, num12, true);
												
											NetMessage.SendTileSquare(-1, num11, num12, 3);
										}
									}
								}
							}
						}
					}
					else if (flag && WorldModify.spawnNPC > 0)
					{
						WorldModify.SpawnNPC(TileX, TileY);
					}
					if (Tile.Active)
					{
						if ((Tile.Type == 2 || Tile.Type == 52) && WorldModify.genRand.Next(40) == 0 && !Main.tile.At(TileX, TileY + 1).Active && !Main.tile.At(TileX, TileY + 1).Lava)
						{
							bool flag4 = false;
							for (int n = TileY; n > TileY - 10; n--)
							{
								if (Main.tile.At(TileX, n).Active && Main.tile.At(TileX, n).Type == 2)
								{
									flag4 = true;
									break;
								}
							}
							if (flag4)
							{
								int num20 = TileX;
								int num21 = TileY + 1;
								Main.tile.At(num20, num21).SetType (52);
								Main.tile.At(num20, num21).SetActive (true);
								WorldModify.SquareTileFrame(num20, num21, true);
								NetMessage.SendTileSquare(-1, num20, num21, 3);
							}
						}
						if (Tile.Type == 60)
						{
							int type = (int)Tile.Type;
							if (!Tile2.Active && WorldModify.genRand.Next(7) == 0)
							{
								WorldModify.PlaceTile(TileX, rTileY, 61, true, false, -1, 0);
								if (Tile2.Active)
								{
									NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
								}
							}
							else if (WorldModify.genRand.Next(500) == 0 && (!Tile2.Active || Tile2.Type == 61 || Tile2.Type == 74 || Tile2.Type == 69) && !WorldModify.PlayerLOS(TileX, TileY))
							{
								WorldModify.GrowTree(TileX, TileY);
							}
							bool flag5 = false;
							for (int num22 = num6; num22 < num7; num22++)
							{
								for (int num23 = rTileY; num23 < num9; num23++)
								{
									if ((TileX != num22 || TileY != num23) && Main.tile.At(num22, num23).Active && Main.tile.At(num22, num23).Type == 59)
									{
										WorldModify.SpreadGrass(num22, num23, 59, type, false);
										if ((int)Main.tile.At(num22, num23).Type == type)
										{
											WorldModify.SquareTileFrame(num22, num23, true);
											flag5 = true;
										}
									}
								}
							}
							if (flag5)
							{
								NetMessage.SendTileSquare(-1, TileX, TileY, 3);
							}
						}
						if (Tile.Type == 61 && WorldModify.genRand.Next(3) == 0 && Tile.FrameX < 144)
						{
							Tile.SetType (74);
							NetMessage.SendTileSquare(-1, TileX, TileY, 3);
						}
						if ((Tile.Type == 60 || Tile.Type == 62) && WorldModify.genRand.Next(15) == 0 && !Main.tile.At(TileX, TileY + 1).Active && !Main.tile.At(TileX, TileY + 1).Lava)
						{
							bool flag6 = false;
							for (int num24 = TileY; num24 > TileY - 10; num24--)
							{
								if (Main.tile.At(TileX, num24).Active && Main.tile.At(TileX, num24).Type == 60)
								{
									flag6 = true;
									break;
								}
							}
							if (flag6)
							{
								int num25 = TileX;
								int num26 = TileY + 1;
								Main.tile.At(num25, num26).SetType (62);
								Main.tile.At(num25, num26).SetActive (true);
								WorldModify.SquareTileFrame(num25, num26, true);
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
				int num28 = WorldModify.genRand.Next(10, Main.maxTilesX - 10);
				int num29 = WorldModify.genRand.Next((int)Main.worldSurface + 2, Main.maxTilesY - 20);
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
						WorldModify.GrowAlch(num28, num29);
					}
					if (Main.tile.At(num28, num29).Liquid <= 32)
					{
						if (Main.tile.At(num28, num29).Active)
						{
							if (Main.tile.At(num28, num29).Type == 60)
							{
								int type2 = (int)Main.tile.At(num28, num29).Type;
								if (!Main.tile.At(num28, num32).Active && WorldModify.genRand.Next(10) == 0)
								{
									WorldModify.PlaceTile(num28, num32, 61, true, false, -1, 0);
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
											WorldModify.SpreadGrass(num34, num35, 59, type2, false);
											if ((int)Main.tile.At(num34, num35).Type == type2)
											{
												WorldModify.SquareTileFrame(num34, num35, true);
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
							if (Main.tile.At(num28, num29).Type == 61 && WorldModify.genRand.Next(3) == 0 && Main.tile.At(num28, num29).FrameX < 144)
							{
								Main.tile.At(num28, num29).SetType (74);
								NetMessage.SendTileSquare(-1, num28, num29, 3);
							}
							if ((Main.tile.At(num28, num29).Type == 60 || Main.tile.At(num28, num29).Type == 62) && WorldModify.genRand.Next(5) == 0 && !Main.tile.At(num28, num29 + 1).Active && !Main.tile.At(num28, num29 + 1).Lava)
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
									WorldModify.SquareTileFrame(num37, num38, true);
									NetMessage.SendTileSquare(-1, num37, num38, 3);
								}
							}
							if (Main.tile.At(num28, num29).Type == 69 && WorldModify.genRand.Next(3) == 0)
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
									int num42 = WorldModify.genRand.Next(4);
									if (num42 == 0)
									{
										num40--;
									}
									else if (num42 == 1)
									{
										num40++;
									}
									else if (num42 == 2)
								    {
									    num39--;
								    }
								    else if (num42 == 3)
									{
										num39++;
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
												WorldModify.SquareTileFrame(num39, num40, true);
												NetMessage.SendTileSquare(-1, num39, num40, 3);
											}
										}
									}
								}
							}
							if (Main.tile.At(num28, num29).Type == 70)
							{
								int type3 = (int)Main.tile.At(num28, num29).Type;
								if (!Main.tile.At(num28, num32).Active && WorldModify.genRand.Next(10) == 0)
								{
									WorldModify.PlaceTile(num28, num32, 71, true, false, -1, 0);
									if (Main.tile.At(num28, num32).Active)
									{
										NetMessage.SendTileSquare(-1, num28, num32, 1);
									}
								}
								if (WorldModify.genRand.Next(200) == 0 && !WorldModify.PlayerLOS(num28, num29))
								{
									WorldModify.GrowShroom(num28, num29);
								}
								bool flag10 = false;
								for (int num50 = num30; num50 < num31; num50++)
								{
									for (int num51 = num32; num51 < num33; num51++)
									{
										if ((num28 != num50 || num29 != num51) && Main.tile.At(num50, num51).Active && Main.tile.At(num50, num51).Type == 59)
										{
											WorldModify.SpreadGrass(num50, num51, 59, type3, false);
											if ((int)Main.tile.At(num50, num51).Type == type3)
											{
												WorldModify.SquareTileFrame(num50, num51, true);
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
						else if (flag && WorldModify.spawnNPC > 0)
						{
							WorldModify.SpawnNPC(num28, num29);
						}
					}
				}
				num27++;
			}
			if (Main.rand.Next(100) == 0)
			{
				WorldModify.PlantAlch();
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
							bool flag = false;
							if (Main.tile.At(i, j).Wall == 0 && (type == 2 || type == 16) && (Main.tile.At(k, l).Wall == 2 || Main.tile.At(k, l).Wall == 16))
							{
								flag = true;
							}
							if (!flag)
							{
								return;
							}
						}
					}
				}
				Main.tile.At(i, j).SetWall ((byte)type);
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
							WorldModify.SpreadGrass(m, n, dirt, grass, true);
						}
					}
				}
			}
		}
		
		public static void SquareTileFrame(int i, int j, bool resetFrame = true)
		{
			WorldModify.TileFrame(i - 1, j - 1, false, false);
			WorldModify.TileFrame(i - 1, j, false, false);
			WorldModify.TileFrame(i - 1, j + 1, false, false);
			WorldModify.TileFrame(i, j - 1, false, false);
			WorldModify.TileFrame(i, j, resetFrame, false);
			WorldModify.TileFrame(i, j + 1, false, false);
			WorldModify.TileFrame(i + 1, j - 1, false, false);
			WorldModify.TileFrame(i + 1, j, false, false);
			WorldModify.TileFrame(i + 1, j + 1, false, false);
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
					WorldModify.TileFrame(i, j, true, true);
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
					WorldModify.TileFrame(i, j, false, false);
				}
			}
		}
		
		public static void WaterCheck(ProgressLogger prog = null)
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
					else if (Main.tile.At(i, j).Liquid > 0)
					{
						if (Main.tile.At(i, j).Active)
						{
							if (Main.tileWaterDeath[(int)Main.tile.At(i, j).Type])
							{
								WorldModify.KillTile(i, j, false, false, false);
							}
							if (Main.tile.At(i, j).Lava && Main.tileLavaDeath[(int)Main.tile.At(i, j).Type])
							{
								WorldModify.KillTile(i, j, false, false, false);
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
						else if ((!Main.tile.At(i + 1, j).Active || !Main.tileSolid[(int)Main.tile.At(i + 1, j).Type] || Main.tileSolidTop[(int)Main.tile.At(i + 1, j).Type]) && Main.tile.At(i + 1, j).Liquid != Main.tile.At(i, j).Liquid)
						{
							Liquid.AddWater(i, j);
						}
						if (Main.tile.At(i, j).Lava)
						{
							if (Main.tile.At(i - 1, j).Liquid > 0 && !Main.tile.At(i - 1, j).Lava)
							{
								Liquid.AddWater(i, j);
							}
							else if (Main.tile.At(i + 1, j).Liquid > 0 && !Main.tile.At(i + 1, j).Lava)
							{
								Liquid.AddWater(i, j);
							}
							else if (Main.tile.At(i, j - 1).Liquid > 0 && !Main.tile.At(i, j - 1).Lava)
							{
								Liquid.AddWater(i, j);
							}
							else if (Main.tile.At(i, j + 1).Liquid > 0 && !Main.tile.At(i, j + 1).Lava)
							{
								Liquid.AddWater(i, j);
							}
						}
					}
				}
                if (prog != null)
                    prog.Value++;
			}
		}
		
		public static void EveryTileFrame()
		{
			using (var prog = new ProgressLogger (Main.maxTilesX, "Finding tile frames"))
			{
				WorldModify.noLiquidCheck = true;
				WorldModify.noTileActions = true;
				for (int i = 0; i < Main.maxTilesX; i++)
				{
					prog.Value = i;
					
					for (int j = 0; j < Main.maxTilesY; j++)
					{
						WorldModify.TileFrame(i, j, true, false);
					}
				}
				WorldModify.noLiquidCheck = false;
				WorldModify.noTileActions = false;
			}
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
				WorldModify.KillTile(i, j, false, false, false);
			}
		}

        public static void TileFrame(int i, int j, bool resetFrame = false, bool noBreak = false)
        {
            if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5)
            {
                if (Main.tile.At(i, j).Liquid > 0 && !WorldModify.noLiquidCheck)
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
                    WorldModify.mergeUp = false;
                    WorldModify.mergeDown = false;
                    WorldModify.mergeLeft = false;
                    WorldModify.mergeRight = false;
                    if (Main.tile.At(i - 1, j).Active)
                    {
                        num4 = (int)Main.tile.At(i - 1, j).Type;
                    }
                    if (Main.tile.At(i + 1, j).Active)
                    {
                        num5 = (int)Main.tile.At(i + 1, j).Type;
                    }
                    if ( Main.tile.At(i, j - 1).Active)
                    {
                        num2 = (int)Main.tile.At(i, j - 1).Type;
                    }
                    if (Main.tile.At(i, j + 1).Active)
                    {
                        num7 = (int)Main.tile.At(i, j + 1).Type;
                    }
                    if (Main.tile.At(i - 1, j - 1).Active)
                    {
                        num = (int)Main.tile.At(i - 1, j - 1).Type;
                    }
                    if (Main.tile.At(i + 1, j - 1).Active)
                    {
                        num3 = (int)Main.tile.At(i + 1, j - 1).Type;
                    }
                    if (Main.tile.At(i - 1, j + 1).Active)
                    {
                        num6 = (int)Main.tile.At(i - 1, j + 1).Type;
                    }
                    if (Main.tile.At(i + 1, j + 1).Active)
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
                    if (num9 != 0 && num9 != 1)
                    {
                        if (num9 == 3 || num9 == 24 || num9 == 61 || num9 == 71 || num9 == 73 || num9 == 74)
                        {
                            WorldModify.PlantCheck(i, j);
                            return;
                        }
                        if (num9 == 4)
                        {
                            if (num7 >= 0 && Main.tileSolid[num7] && !Main.tileNoAttach[num7])
                            {
                                Main.tile.At(i, j).SetFrameX(0);
                                return;
                            }
                            if ((num4 >= 0 && Main.tileSolid[num4] && !Main.tileNoAttach[num4]) || (num4 == 5 && num == 5 && num6 == 5))
                            {
                                Main.tile.At(i, j).SetFrameX(22);
                                return;
                            }
                            if ((num5 >= 0 && Main.tileSolid[num5] && !Main.tileNoAttach[num5]) || (num5 == 5 && num3 == 5 && num8 == 5))
                            {
                                Main.tile.At(i, j).SetFrameX(44);
                                return;
                            }
                            WorldModify.KillTile(i, j, false, false, false);
                            return;
                        }
                        else
                        {
                            if (num9 == 80)
                            {
                                WorldModify.CactusFrame(i, j);
                                return;
                            }
                            if (num9 == 12 || num9 == 31)
                            {
                                if (!WorldModify.destroyObject)
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
                                    if ((!Main.tile.At(num10, num11).Active || (int)Main.tile.At(num10, num11).Type != num9 ||
                                        !Main.tile.At(num10 + 1, num11).Active || (int)Main.tile.At(num10 + 1, num11).Type != num9 ||
                                            !Main.tile.At(num10, num11 + 1).Active || (int)Main.tile.At(num10, num11 + 1).Type != num9 ||
                                                !Main.tile.At(num10 + 1, num11 + 1).Active || 
                                                    (int)Main.tile.At(num10 + 1, num11 + 1).Type != num9))
                                    {
                                        WorldModify.destroyObject = true;
                                        if ((int)Main.tile.At(num10, num11).Type == num9)
                                        {
                                            WorldModify.KillTile(num10, num11, false, false, false);
                                        }
                                        if ((int)Main.tile.At(num10 + 1, num11).Type == num9)
                                        {
                                            WorldModify.KillTile(num10 + 1, num11, false, false, false);
                                        }
                                        if ((int)Main.tile.At(num10, num11 + 1).Type == num9)
                                        {
                                            WorldModify.KillTile(num10, num11 + 1, false, false, false);
                                        }
                                        if ((int)Main.tile.At(num10 + 1, num11 + 1).Type == num9)
                                        {
                                            WorldModify.KillTile(num10 + 1, num11 + 1, false, false, false);
                                        }
										if (!WorldModify.noTileActions)
										{
											if (num9 == 12)
											{
												Item.NewItem(num10 * 16, num11 * 16, 32, 32, 29, 1, false);
											}
											else
											{
												if (num9 == 31)
												{
													if (WorldModify.genRand.Next(2) == 0)
													{
														WorldModify.spawnMeteor = true;
													}
													int num12 = Main.rand.Next(5);
													if (!WorldModify.shadowOrbSmashed)
													{
														num12 = 0;
													}
													if (num12 == 0)
													{
														Item.NewItem(num10 * 16, num11 * 16, 32, 32, 96, 1, false);
														int stack = WorldModify.genRand.Next(25, 51);
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
													WorldModify.shadowOrbSmashed = true;
													WorldModify.shadowOrbCount++;
													if (WorldModify.shadowOrbCount >= 3)
													{
														WorldModify.shadowOrbCount = 0;
														float num13 = (float)(num10 * 16);
														float num14 = (float)(num11 * 16);
														float num15 = -1f;
														int plr = 0;
														for (int k = 0; k < 255; k++)
														{
															float num16 = Math.Abs(Main.players[k].Position.X - num13) + Math.Abs(Main.players[k].Position.Y - num14);
															if (num16 < num15 || num15 == -1f)
															{
																plr = 0;
																num15 = num16;
															}
														}
														NPC.SpawnOnPlayer(Main.players[plr], plr, 13); //Check me
													}
													else
													{
														string text = "A horrible chill goes down your spine...";
														if (WorldModify.shadowOrbCount == 2)
														{
															text = "Screams echo around you...";
														}
														NetMessage.SendData(25, -1, -1, text, 255, 50f, 255f, 130f, 0);
													}
												}
											}
										}
                                        WorldModify.destroyObject = false;
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
                                    if (!WorldModify.destroyObject)
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
                                            WorldModify.destroyObject = true;
                                            WorldModify.KillTile(i, num17, false, false, false);
                                            WorldModify.KillTile(i, num17 + 1, false, false, false);
                                            WorldModify.KillTile(i, num17 + 2, false, false, false);
                                            Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
                                        }
                                        WorldModify.destroyObject = false;
                                    }
                                    return;
                                }
                                if (num9 == 11)
                                {
                                    if (!WorldModify.destroyObject)
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
                                        if (!Main.tile.At(num19, num20 - 1).Active ||
                                            !Main.tileSolid[(int)Main.tile.At(num19, num20 - 1).Type] ||
                                            !Main.tile.At(num19, num20 + 3).Active ||
                                            !Main.tileSolid[(int)Main.tile.At(num19, num20 + 3).Type])
                                        {
                                            flag2 = true;
                                            WorldModify.destroyObject = true;
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
                                                    WorldModify.destroyObject = true;
                                                    Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
                                                    flag2 = true;
                                                    l = num21;
                                                    m = num20;
                                                }
                                                if (flag2)
                                                {
                                                    WorldModify.KillTile(l, m, false, false, false);
                                                }
                                            }
                                        }
                                        WorldModify.destroyObject = false;
                                    }
                                    return;
                                }
								if (num9 == 34 || num9 == 35 || num9 == 36 || num9 == 106)
                                {
                                    WorldModify.Check3x3(i, j, (int)((byte)num9));
                                    return;
                                }
                                if (num9 == 15 || num9 == 20)
                                {
                                    WorldModify.Check1x2(i, j, (byte)num9);
                                    return;
                                }
                                if (num9 == 14 || num9 == 17 || num9 == 26 || num9 == 77 || num9 == 86 || num9 == 87 || num9 == 88 || num9 == 89)
                                {
                                    WorldModify.Check3x2(i, j, (int)((byte)num9));
                                    return;
                                }
                                if (num9 == 16 || num9 == 18 || num9 == 29 || num9 == 103)
                                {
                                    WorldModify.Check2x1(i, j, (byte)num9);
                                    return;
                                }
                                if (num9 == 13 || num9 == 33 || num9 == 49 || num9 == 50 || num9 == 78)
                                {
                                    WorldModify.CheckOnTable1x1(i, j, (int)((byte)num9));
                                    return;
                                }
                                if (num9 == 21)
                                {
                                    WorldModify.CheckChest(i, j, (int)((byte)num9));
                                    return;
                                }
                                if (num9 == 27)
                                {
                                    WorldModify.CheckSunflower(i, j, 27);
                                    return;
                                }
                                if (num9 == 28)
                                {
                                    WorldModify.CheckPot(i, j, 28);
                                    return;
                                }
                                if (num9 == 91)
                                {
                                    WorldModify.CheckBanner(i, j, (byte)num9);
                                    return;
                                }
                                if (num9 == 92 || num9 == 93)
                                {
                                    WorldModify.Check1xX(i, j, (byte)num9);
                                    return;
                                }
                                if (num9 == 104 || num9 == 105)
                                {
                                    WorldModify.Check2xX(i, j, (byte)num9);
                                }
                                else
                                {
                                    if (num9 == 101 || num9 == 102)
                                    {
                                        WorldModify.Check3x4(i, j, (int)((byte)num9));
                                        return;
                                    }
                                    if (num9 == 42)
                                    {
                                        WorldModify.Check1x2Top(i, j, (byte)num9);
                                        return;
                                    }
                                    if (num9 == 55 || num9 == 85)
                                    {
                                        WorldModify.CheckSign(i, j, num9);
                                        return;
                                    }
                                    if (num9 == 79 || num9 == 90)
                                    {
                                        WorldModify.Check4x2(i, j, num9);
                                        return;
                                    }
                                    if (num9 == 85 || num9 == 94 || num9 == 95 || num9 == 96 || num9 == 97 || num9 == 98 || num9 == 99 || num9 == 100)
                                    {
                                        WorldModify.Check2x2(i, j, num9);
                                        return;
                                    }
                                    if (num9 == 81)
                                    {
                                        if (num4 != -1 || num2 != -1 || num5 != -1)
                                        {
                                            WorldModify.KillTile(i, j, false, false, false);
                                            return;
                                        }
                                        if (num7 < 0 || !Main.tileSolid[num7])
                                        {
                                            WorldModify.KillTile(i, j, false, false, false);
                                        }
                                        return;
                                    }
                                    else
                                    {
                                        if (Main.tileAlch[num9])
                                        {
                                            WorldModify.CheckAlch(i, j);
                                            return;
                                        }
                                        if (num9 == 72)
                                        {
                                            if (num7 != num9 && num7 != 70)
                                            {
                                                WorldModify.KillTile(i, j, false, false, false);
                                            }
                                            else
                                            {
                                                if (num2 != num9 && Main.tile.At(i, j).FrameX == 0)
                                                {
                                                    Main.tile.At(i, j).SetFrameNumber((byte)WorldModify.genRand.Next(3));
                                                    if (Main.tile.At(i, j).FrameNumber == 0)
                                                    {
                                                        Main.tile.At(i, j).SetFrameX(18);
                                                        Main.tile.At(i, j).SetFrameY(0);
                                                    }
                                                    if (Main.tile.At(i, j).FrameNumber == 1)
                                                    {
                                                        Main.tile.At(i, j).SetFrameX(18);
                                                        Main.tile.At(i, j).SetFrameY(18);
                                                    }
                                                    if (Main.tile.At(i, j).FrameNumber == 2)
                                                    {
                                                        Main.tile.At(i, j).SetFrameX(18);
                                                        Main.tile.At(i, j).SetFrameY(36);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
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
                                                        WorldModify.KillTile(i, j, false, false, false);
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
                                                                Main.tile.At(i, j).SetFrameX(110);
                                                                Main.tile.At(i, j).SetFrameY(66);
                                                            }
                                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX(110);
                                                                Main.tile.At(i, j).SetFrameY(88);
                                                            }
                                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX(110);
                                                                Main.tile.At(i, j).SetFrameY(110);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num4 == num9)
                                                            {
                                                                if (Main.tile.At(i, j).FrameNumber == 0)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(88);
                                                                    Main.tile.At(i, j).SetFrameY(0);
                                                                }
                                                                if (Main.tile.At(i, j).FrameNumber == 1)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(88);
                                                                    Main.tile.At(i, j).SetFrameY(22);
                                                                }
                                                                if (Main.tile.At(i, j).FrameNumber == 2)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(88);
                                                                    Main.tile.At(i, j).SetFrameY(44);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (num5 == num9)
                                                                {
                                                                    if (Main.tile.At(i, j).FrameNumber == 0)
                                                                    {
                                                                        Main.tile.At(i, j).SetFrameX(66);
                                                                        Main.tile.At(i, j).SetFrameY(66);
                                                                    }
                                                                    if (Main.tile.At(i, j).FrameNumber == 1)
                                                                    {
                                                                        Main.tile.At(i, j).SetFrameX(66);
                                                                        Main.tile.At(i, j).SetFrameY(88);
                                                                    }
                                                                    if (Main.tile.At(i, j).FrameNumber == 2)
                                                                    {
                                                                        Main.tile.At(i, j).SetFrameX(66);
                                                                        Main.tile.At(i, j).SetFrameY(110);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (Main.tile.At(i, j).FrameNumber == 0)
                                                                    {
                                                                        Main.tile.At(i, j).SetFrameX(0);
                                                                        Main.tile.At(i, j).SetFrameY(0);
                                                                    }
                                                                    if (Main.tile.At(i, j).FrameNumber == 1)
                                                                    {
                                                                        Main.tile.At(i, j).SetFrameX(0);
                                                                        Main.tile.At(i, j).SetFrameY(22);
                                                                    }
                                                                    if (Main.tile.At(i, j).FrameNumber == 2)
                                                                    {
                                                                        Main.tile.At(i, j).SetFrameX(0);
                                                                        Main.tile.At(i, j).SetFrameY(44);
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
                                                        WorldModify.KillTile(i, j, false, false, false);
                                                    }
                                                    if (num4 != num9 && num5 != num9)
                                                    {
                                                        if (Main.tile.At(i, j).FrameNumber == 0)
                                                        {
                                                            Main.tile.At(i, j).SetFrameX(0);
                                                            Main.tile.At(i, j).SetFrameY(0);
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                                        {
                                                            Main.tile.At(i, j).SetFrameX(0);
                                                            Main.tile.At(i, j).SetFrameY(22);
                                                        }
                                                        if (Main.tile.At(i, j).FrameNumber == 2)
                                                        {
                                                            Main.tile.At(i, j).SetFrameX(0);
                                                            Main.tile.At(i, j).SetFrameY(44);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num4 != num9)
                                                        {
                                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX(0);
                                                                Main.tile.At(i, j).SetFrameY(132);
                                                            }
                                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX(0);
                                                                Main.tile.At(i, j).SetFrameY(154);
                                                            }
                                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                                            {
                                                                Main.tile.At(i, j).SetFrameX(0);
                                                                Main.tile.At(i, j).SetFrameY(176);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (num5 != num9)
                                                            {
                                                                if (Main.tile.At(i, j).FrameNumber == 0)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(66);
                                                                    Main.tile.At(i, j).SetFrameY(132);
                                                                }
                                                                if (Main.tile.At(i, j).FrameNumber == 1)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(66);
                                                                    Main.tile.At(i, j).SetFrameY(154);
                                                                }
                                                                if (Main.tile.At(i, j).FrameNumber == 2)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(66);
                                                                    Main.tile.At(i, j).SetFrameY(176);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (Main.tile.At(i, j).FrameNumber == 0)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(88);
                                                                    Main.tile.At(i, j).SetFrameY(132);
                                                                }
                                                                if (Main.tile.At(i, j).FrameNumber == 1)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(88);
                                                                    Main.tile.At(i, j).SetFrameY(154);
                                                                }
                                                                if (Main.tile.At(i, j).FrameNumber == 2)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(88);
                                                                    Main.tile.At(i, j).SetFrameY(176);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if ((Main.tile.At(i, j).FrameX == 66 && (Main.tile.At(i, j).FrameY == 0 || Main.tile.At(i, j).FrameY == 22 || Main.tile.At(i, j).FrameY == 44)) || (Main.tile.At(i, j).FrameX == 88 && (Main.tile.At(i, j).FrameY == 66 || Main.tile.At(i, j).FrameY == 88 || Main.tile.At(i, j).FrameY == 110)) || (Main.tile.At(i, j).FrameX == 44 && (Main.tile.At(i, j).FrameY == 198 || Main.tile.At(i, j).FrameY == 220 || Main.tile.At(i, j).FrameY == 242)) || (Main.tile.At(i, j).FrameX == 66 && (Main.tile.At(i, j).FrameY == 198 || Main.tile.At(i, j).FrameY == 220 || Main.tile.At(i, j).FrameY == 242)))
                                                {
                                                    if (num4 != num9 && num5 != num9)
                                                    {
                                                        WorldModify.KillTile(i, j, false, false, false);
                                                    }
                                                }
                                                else
                                                {
                                                    if (num7 == -1 || num7 == 23)
                                                    {
                                                        WorldModify.KillTile(i, j, false, false, false);
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
                                                                            Main.tile.At(i, j).SetFrameX(132);
                                                                            Main.tile.At(i, j).SetFrameY(132);
                                                                        }
                                                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                                                        {
                                                                            Main.tile.At(i, j).SetFrameX(132);
                                                                            Main.tile.At(i, j).SetFrameY(154);
                                                                        }
                                                                        if (Main.tile.At(i, j).FrameNumber == 2)
                                                                        {
                                                                            Main.tile.At(i, j).SetFrameX(132);
                                                                            Main.tile.At(i, j).SetFrameY(176);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num4 == num9)
                                                                        {
                                                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                                                            {
                                                                                Main.tile.At(i, j).SetFrameX(132);
                                                                                Main.tile.At(i, j).SetFrameY(0);
                                                                            }
                                                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                                                            {
                                                                                Main.tile.At(i, j).SetFrameX(132);
                                                                                Main.tile.At(i, j).SetFrameY(22);
                                                                            }
                                                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                                                            {
                                                                                Main.tile.At(i, j).SetFrameX(132);
                                                                                Main.tile.At(i, j).SetFrameY(44);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num5 == num9)
                                                                            {
                                                                                if (Main.tile.At(i, j).FrameNumber == 0)
                                                                                {
                                                                                    Main.tile.At(i, j).SetFrameX(132);
                                                                                    Main.tile.At(i, j).SetFrameY(66);
                                                                                }
                                                                                if (Main.tile.At(i, j).FrameNumber == 1)
                                                                                {
                                                                                    Main.tile.At(i, j).SetFrameX(132);
                                                                                    Main.tile.At(i, j).SetFrameY(88);
                                                                                }
                                                                                if (Main.tile.At(i, j).FrameNumber == 2)
                                                                                {
                                                                                    Main.tile.At(i, j).SetFrameX(132);
                                                                                    Main.tile.At(i, j).SetFrameY(110);
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
                                                                            Main.tile.At(i, j).SetFrameX(154);
                                                                            Main.tile.At(i, j).SetFrameY(132);
                                                                        }
                                                                        if (Main.tile.At(i, j).FrameNumber == 1)
                                                                        {
                                                                            Main.tile.At(i, j).SetFrameX(154);
                                                                            Main.tile.At(i, j).SetFrameY(154);
                                                                        }
                                                                        if (Main.tile.At(i, j).FrameNumber == 2)
                                                                        {
                                                                            Main.tile.At(i, j).SetFrameX(154);
                                                                            Main.tile.At(i, j).SetFrameY(176);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num4 == num9)
                                                                        {
                                                                            if (Main.tile.At(i, j).FrameNumber == 0)
                                                                            {
                                                                                Main.tile.At(i, j).SetFrameX(154);
                                                                                Main.tile.At(i, j).SetFrameY(0);
                                                                            }
                                                                            if (Main.tile.At(i, j).FrameNumber == 1)
                                                                            {
                                                                                Main.tile.At(i, j).SetFrameX(154);
                                                                                Main.tile.At(i, j).SetFrameY(22);
                                                                            }
                                                                            if (Main.tile.At(i, j).FrameNumber == 2)
                                                                            {
                                                                                Main.tile.At(i, j).SetFrameX(154);
                                                                                Main.tile.At(i, j).SetFrameY(44);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num5 == num9)
                                                                            {
                                                                                if (Main.tile.At(i, j).FrameNumber == 0)
                                                                                {
                                                                                    Main.tile.At(i, j).SetFrameX(154);
                                                                                    Main.tile.At(i, j).SetFrameY(66);
                                                                                }
                                                                                if (Main.tile.At(i, j).FrameNumber == 1)
                                                                                {
                                                                                    Main.tile.At(i, j).SetFrameX(154);
                                                                                    Main.tile.At(i, j).SetFrameY(88);
                                                                                }
                                                                                if (Main.tile.At(i, j).FrameNumber == 2)
                                                                                {
                                                                                    Main.tile.At(i, j).SetFrameX(154);
                                                                                    Main.tile.At(i, j).SetFrameY(110);
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
                                                                    Main.tile.At(i, j).SetFrameX(110);
                                                                    Main.tile.At(i, j).SetFrameY(0);
                                                                }
                                                                if (Main.tile.At(i, j).FrameNumber == 1)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(110);
                                                                    Main.tile.At(i, j).SetFrameY(22);
                                                                }
                                                                if (Main.tile.At(i, j).FrameNumber == 2)
                                                                {
                                                                    Main.tile.At(i, j).SetFrameX(110);
                                                                    Main.tile.At(i, j).SetFrameY(44);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                rectangle.X = (int)Main.tile.At(i, j).FrameX;
                                                rectangle.Y = (int)Main.tile.At(i, j).FrameY;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Main.tileFrameImportant[(int)Main.tile.At(i, j).Type])
                    {
                        return;
                    }
                    int num22 = 0;
                    if (resetFrame)
                    {
                        num22 = WorldModify.genRand.Next(0, 3);
                        Main.tile.At(i, j).SetFrameNumber((byte)num22);
                    }
                    else
                    {
                        num22 = (int)Main.tile.At(i, j).FrameNumber;
                    }
                    if (num9 == 0)
                    {
                        if (num2 >= 0 && Main.tileMergeDirt[num2])
                        {
                            WorldModify.TileFrame(i, j - 1, false, false);
                            if (WorldModify.mergeDown)
                            {
                                num2 = num9;
                            }
                        }
                        if (num7 >= 0 && Main.tileMergeDirt[num7])
                        {
                            WorldModify.TileFrame(i, j + 1, false, false);
                            if (WorldModify.mergeUp)
                            {
                                num7 = num9;
                            }
                        }
                        if (num4 >= 0 && Main.tileMergeDirt[num4])
                        {
                            WorldModify.TileFrame(i - 1, j, false, false);
                            if (WorldModify.mergeRight)
                            {
                                num4 = num9;
                            }
                        }
                        if (num5 >= 0 && Main.tileMergeDirt[num5])
                        {
                            WorldModify.TileFrame(i + 1, j, false, false);
                            if (WorldModify.mergeLeft)
                            {
                                num5 = num9;
                            }
                        }
                        if (num >= 0 && Main.tileMergeDirt[num])
                        {
                            num = num9;
                        }
                        if (num3 >= 0 && Main.tileMergeDirt[num3])
                        {
                            num3 = num9;
                        }
                        if (num6 >= 0 && Main.tileMergeDirt[num6])
                        {
                            num6 = num9;
                        }
                        if (num8 >= 0 && Main.tileMergeDirt[num8])
                        {
                            num8 = num9;
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
                                WorldModify.TileFrame(i, j - 1, false, false);
                                if (WorldModify.mergeDown)
                                {
                                    num2 = num9;
                                }
                            }
                            if (num7 == 58)
                            {
                                WorldModify.TileFrame(i, j + 1, false, false);
                                if (WorldModify.mergeUp)
                                {
                                    num7 = num9;
                                }
                            }
                            if (num4 == 58)
                            {
                                WorldModify.TileFrame(i - 1, j, false, false);
                                if (WorldModify.mergeRight)
                                {
                                    num4 = num9;
                                }
                            }
                            if (num5 == 58)
                            {
                                WorldModify.TileFrame(i + 1, j, false, false);
                                if (WorldModify.mergeLeft)
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
                    if (Main.tileMergeDirt[num9])
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
                    else
                    {
                        if (num9 == 69)
                        {
                            if (num7 == 60)
                            {
                                num7 = num9;
                            }
                        }
                        else
                        {
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
                        int num23 = 0;
                        if (num9 == 60 || num9 == 70)
                        {
                            num23 = 59;
                        }
                        else
                        {
                            if (num9 == 2)
                            {
                                if (num2 == 23)
                                {
                                    num2 = num23;
                                }
                                if (num7 == 23)
                                {
                                    num7 = num23;
                                }
                                if (num4 == 23)
                                {
                                    num4 = num23;
                                }
                                if (num5 == 23)
                                {
                                    num5 = num23;
                                }
                                if (num == 23)
                                {
                                    num = num23;
                                }
                                if (num3 == 23)
                                {
                                    num3 = num23;
                                }
                                if (num6 == 23)
                                {
                                    num6 = num23;
                                }
                                if (num8 == 23)
                                {
                                    num8 = num23;
                                }
                            }
                            else
                            {
                                if (num9 == 23)
                                {
                                    if (num2 == 2)
                                    {
                                        num2 = num23;
                                    }
                                    if (num7 == 2)
                                    {
                                        num7 = num23;
                                    }
                                    if (num4 == 2)
                                    {
                                        num4 = num23;
                                    }
                                    if (num5 == 2)
                                    {
                                        num5 = num23;
                                    }
                                    if (num == 2)
                                    {
                                        num = num23;
                                    }
                                    if (num3 == 2)
                                    {
                                        num3 = num23;
                                    }
                                    if (num6 == 2)
                                    {
                                        num6 = num23;
                                    }
                                    if (num8 == 2)
                                    {
                                        num8 = num23;
                                    }
                                }
                            }
                        }
                        if (num2 != num9 && num2 != num23 && (num7 == num9 || num7 == num23))
                        {
                            if (num4 == num23 && num5 == num9)
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
                                if (num4 == num9 && num5 == num23)
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
                            if (num7 != num9 && num7 != num23 && (num2 == num9 || num2 == num23))
                            {
                                if (num4 == num23 && num5 == num9)
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
                                    if (num4 == num9 && num5 == num23)
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
                                if (num4 != num9 && num4 != num23 && (num5 == num9 || num5 == num23))
                                {
                                    if (num2 == num23 && num7 == num9)
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
                                    if (num5 != num9 && num5 != num23 && (num4 == num9 || num4 == num23))
                                    {
                                        if (num2 == num23 && num7 == num9)
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
                                                if (num8 == num23)
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
                                                    if (num3 == num23)
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
                                                        if (num6 == num23)
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
                                                            if (num == num23)
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
                                            if (num2 == num9 && num7 == num23 && num4 == num9 && num5 == num9 && num == -1 && num3 == -1)
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
                                                if (num2 == num23 && num7 == num9 && num4 == num9 && num5 == num9 && num6 == -1 && num8 == -1)
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
                                                    if (num2 == num9 && num7 == num9 && num4 == num23 && num5 == num9 && num3 == -1 && num8 == -1)
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
                                                        if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num23 && num == -1 && num6 == -1)
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
                                                            if (num2 == num9 && num7 == num23 && num4 == num9 && num5 == num9)
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
                                                                if (num2 == num23 && num7 == num9 && num4 == num9 && num5 == num9)
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
                                                                    if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num23)
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
                                                                        if (num2 == num9 && num7 == num9 && num4 == num23 && num5 == num9)
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
                                                                            if ((num2 == num23 && num7 == num9 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num23 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num23 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num23))
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
                        if ((num2 == num9 || num2 == num23) && (num7 == num9 || num7 == num23) && (num4 == num9 || num4 == num23) && (num5 == num9 || num5 == num23))
                        {
                            if (num != num9 && num != num23 && (num3 == num9 || num3 == num23) && (num6 == num9 || num6 == num23) && (num8 == num9 || num8 == num23))
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
                                if (num3 != num9 && num3 != num23 && (num == num9 || num == num23) && (num6 == num9 || num6 == num23) && (num8 == num9 || num8 == num23))
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
                                    if (num6 != num9 && num6 != num23 && (num == num9 || num == num23) && (num3 == num9 || num3 == num23) && (num8 == num9 || num8 == num23))
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
                                        if (num8 != num9 && num8 != num23 && (num == num9 || num == num23) && (num6 == num9 || num6 == num23) && (num3 == num9 || num3 == num23))
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
                        if (num2 != num23 && num2 != num9 && num7 == num9 && num4 != num23 && num4 != num9 && num5 == num9 && num8 != num23 && num8 != num9)
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
                            if (num2 != num23 && num2 != num9 && num7 == num9 && num4 == num9 && num5 != num23 && num5 != num9 && num6 != num23 && num6 != num9)
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
                                if (num7 != num23 && num7 != num9 && num2 == num9 && num4 != num23 && num4 != num9 && num5 == num9 && num3 != num23 && num3 != num9)
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
                                    if (num7 != num23 && num7 != num9 && num2 == num9 && num4 == num9 && num5 != num23 && num5 != num9 && num != num23 && num != num9)
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
                                        if (num2 != num9 && num2 != num23 && num7 == num9 && num4 == num9 && num5 == num9 && num6 != num9 && num6 != num23 && num8 != num9 && num8 != num23)
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
                                            if (num7 != num9 && num7 != num23 && num2 == num9 && num4 == num9 && num5 == num9 && num != num9 && num != num23 && num3 != num9 && num3 != num23)
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
                                                if (num4 != num9 && num4 != num23 && num7 == num9 && num2 == num9 && num5 == num9 && num3 != num9 && num3 != num23 && num8 != num9 && num8 != num23)
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
                                                    if (num5 != num9 && num5 != num23 && num7 == num9 && num2 == num9 && num4 == num9 && num != num9 && num != num23 && num6 != num9 && num6 != num23)
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
                                                        if (num2 != num23 && num2 != num9 && (num7 == num23 || num7 == num9) && num4 == num23 && num5 == num23)
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
                                                            if (num7 != num23 && num7 != num9 && (num2 == num23 || num2 == num9) && num4 == num23 && num5 == num23)
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
                                                                if (num4 != num23 && num4 != num9 && (num5 == num23 || num5 == num9) && num2 == num23 && num7 == num23)
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
                                                                    if (num5 != num23 && num5 != num9 && (num4 == num23 || num4 == num9) && num2 == num23 && num7 == num23)
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
                                                                        if (num2 == num9 && num7 == num23 && num4 == num23 && num5 == num23)
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
                                                                            if (num2 == num23 && num7 == num9 && num4 == num23 && num5 == num23)
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
                                                                                if (num2 == num23 && num7 == num23 && num4 == num9 && num5 == num23)
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
                                                                                    if (num2 == num23 && num7 == num23 && num4 == num23 && num5 == num9)
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
                        if (num2 != num9 && num2 != num23 && num7 == num9 && num4 == num9 && num5 == num9)
                        {
                            if ((num6 == num23 || num6 == num9) && num8 != num23 && num8 != num9)
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
                                if ((num8 == num23 || num8 == num9) && num6 != num23 && num6 != num9)
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
                            if (num7 != num9 && num7 != num23 && num2 == num9 && num4 == num9 && num5 == num9)
                            {
                                if ((num == num23 || num == num9) && num3 != num23 && num3 != num9)
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
                                    if ((num3 == num23 || num3 == num9) && num != num23 && num != num9)
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
                                if (num4 != num9 && num4 != num23 && num2 == num9 && num7 == num9 && num5 == num9)
                                {
                                    if ((num3 == num23 || num3 == num9) && num8 != num23 && num8 != num9)
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
                                        if ((num8 == num23 || num8 == num9) && num3 != num23 && num3 != num9)
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
                                    if (num5 != num9 && num5 != num23 && num2 == num9 && num7 == num9 && num4 == num9)
                                    {
                                        if ((num == num23 || num == num9) && num6 != num23 && num6 != num9)
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
                                            if ((num6 == num23 || num6 == num9) && num != num23 && num != num9)
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
                        if ((num2 == num9 || num2 == num23) && (num7 == num9 || num7 == num23) && (num4 == num9 || num4 == num23) && (num5 == num9 || num5 == num23) && num != -1 && num3 != -1 && num6 != -1 && num8 != -1)
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
                        if (num2 == num23)
                        {
                            num2 = -2;
                        }
                        if (num7 == num23)
                        {
                            num7 = -2;
                        }
                        if (num4 == num23)
                        {
                            num4 = -2;
                        }
                        if (num5 == num23)
                        {
                            num5 = -2;
                        }
                        if (num == num23)
                        {
                            num = -2;
                        }
                        if (num3 == num23)
                        {
                            num3 = -2;
                        }
                        if (num6 == num23)
                        {
                            num6 = -2;
                        }
                        if (num8 == num23)
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
                                WorldModify.mergeUp = true;
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
                                    WorldModify.mergeDown = true;
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
                                        WorldModify.mergeLeft = true;
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
                                            WorldModify.mergeRight = true;
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
                                                WorldModify.mergeUp = true;
                                                WorldModify.mergeLeft = true;
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
                                                    WorldModify.mergeUp = true;
                                                    WorldModify.mergeRight = true;
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
                                                        WorldModify.mergeDown = true;
                                                        WorldModify.mergeLeft = true;
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
                                                            WorldModify.mergeDown = true;
                                                            WorldModify.mergeRight = true;
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
                                                                WorldModify.mergeLeft = true;
                                                                WorldModify.mergeRight = true;
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
                                                                    WorldModify.mergeUp = true;
                                                                    WorldModify.mergeDown = true;
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
                                                                        WorldModify.mergeUp = true;
                                                                        WorldModify.mergeLeft = true;
                                                                        WorldModify.mergeRight = true;
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
                                                                            WorldModify.mergeDown = true;
                                                                            WorldModify.mergeLeft = true;
                                                                            WorldModify.mergeRight = true;
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
                                                                                WorldModify.mergeUp = true;
                                                                                WorldModify.mergeDown = true;
                                                                                WorldModify.mergeRight = true;
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
                                                                                    WorldModify.mergeUp = true;
                                                                                    WorldModify.mergeDown = true;
                                                                                    WorldModify.mergeLeft = true;
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
                                                                                        WorldModify.mergeUp = true;
                                                                                        WorldModify.mergeDown = true;
                                                                                        WorldModify.mergeLeft = true;
                                                                                        WorldModify.mergeRight = true;
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
                                    WorldModify.mergeDown = true;
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
                                        WorldModify.mergeUp = true;
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
                                            WorldModify.mergeRight = true;
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
                                                WorldModify.mergeLeft = true;
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
                                    WorldModify.mergeUp = true;
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
                                        WorldModify.mergeDown = true;
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
                                        WorldModify.mergeUp = true;
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
                                            WorldModify.mergeDown = true;
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
                                            WorldModify.mergeLeft = true;
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
                                                WorldModify.mergeRight = true;
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
                                                WorldModify.mergeLeft = true;
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
                                                    WorldModify.mergeRight = true;
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
                                                    WorldModify.mergeUp = true;
                                                    WorldModify.mergeDown = true;
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
                                                        WorldModify.mergeUp = true;
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
                                                            WorldModify.mergeDown = true;
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
                                                        WorldModify.mergeLeft = true;
                                                        WorldModify.mergeRight = true;
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
                                                            WorldModify.mergeLeft = true;
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
                                                                WorldModify.mergeRight = true;
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
                                                        WorldModify.mergeUp = true;
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
                                                            WorldModify.mergeDown = true;
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
                                                                WorldModify.mergeLeft = true;
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
                                                                    WorldModify.mergeRight = true;
                                                                }
                                                            }
                                                        }
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
                    Main.tile.At(i, j).SetFrameX((short)rectangle.X);
                    Main.tile.At(i, j).SetFrameY((short)rectangle.Y);
                    if (num9 == 52 || num9 == 62)
                    {
                        if (!Main.tile.At(i, j - 1).Active)
                        {
                            num2 = -1;
                        }
                        else
                        {
                            num2 = (int)Main.tile.At(i, j - 1).Type;
                        }
                        if (num2 != num9 && num2 != 2 && num2 != 60)
                        {
                            WorldModify.KillTile(i, j, false, false, false);
                        }
                    }
                    if (!WorldModify.noTileActions && num9 == 53)
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
                                    int type2 = 31;
                                    if (num9 == 59)
                                    {
                                        type2 = 39;
                                    }
                                    if (num9 == 57)
                                    {
                                        type2 = 40;
                                    }
                                    Main.tile.At(i, j).SetActive(false);
                                    int num25 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0.41f, type2, 10, 0f, Main.myPlayer);
                                    Main.projectile[num25].Velocity.Y = 0.5f;
                                    Projectile expr_6501_cp_0 = Main.projectile[num25];
                                    expr_6501_cp_0.Position.Y = expr_6501_cp_0.Position.Y + 2f;
                                    Main.projectile[num25].ai[0] = 1f;
                                    NetMessage.SendTileSquare(-1, i, j, 1);
                                    WorldModify.SquareTileFrame(i, j, true);
                                }
                            }
                    }
                    if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
                    {
                        bool flag5 = WorldModify.mergeUp;
                        bool flag6 = WorldModify.mergeDown;
                        bool flag7 = WorldModify.mergeLeft;
                        bool flag8 = WorldModify.mergeRight;
                        WorldModify.TileFrame(i - 1, j, false, false);
                        WorldModify.TileFrame(i + 1, j, false, false);
                        WorldModify.TileFrame(i, j - 1, false, false);
                        WorldModify.TileFrame(i, j + 1, false, false);
                        WorldModify.mergeUp = flag5;
                        WorldModify.mergeDown = flag6;
                        WorldModify.mergeLeft = flag7;
                        WorldModify.mergeRight = flag8;
                    }
                }
            }
        }
    }
}
