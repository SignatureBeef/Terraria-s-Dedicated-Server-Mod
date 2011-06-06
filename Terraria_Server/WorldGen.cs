
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
		public static bool worldCleared = false;
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
		public static Random genRand;
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
		public static Vector2 lastDungeonHall = new Vector2();
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
		public static void SpawnNPC(int x, int y)
		{
			if (Main.wallHouse[(int)Main.tile[x, y].wall])
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
						Rectangle value = new Rectangle(num2 * 16 + 8 - Main.screenWidth / 2 - NPC.safeRangeX, num3 * 16 + 8 - Main.screenHeight / 2 - NPC.safeRangeY, Main.screenWidth + NPC.safeRangeX * 2, Main.screenHeight + NPC.safeRangeY * 2);
						for (int j = 0; j < 255; j++)
						{
							if (Main.player[j].active)
							{
								Rectangle rectangle = new Rectangle((int)Main.player[j].position.X, (int)Main.player[j].position.Y, Main.player[j].width, Main.player[j].height);
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
										if (Main.tile[num2, num3].active && Main.tileSolid[(int)Main.tile[num2, num3].type])
										{
											if (!Collision.SolidTiles(num2 - 1, num2 + 1, num3 - 3, num3 - 1))
											{
												flag = true;
												Rectangle value2 = new Rectangle(num2 * 16 + 8 - Main.screenWidth / 2 - NPC.safeRangeX, num3 * 16 + 8 - Main.screenHeight / 2 - NPC.safeRangeY, Main.screenWidth + NPC.safeRangeX * 2, Main.screenHeight + NPC.safeRangeY * 2);
												for (int m = 0; m < 255; m++)
												{
													if (Main.player[m].active)
													{
														Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X, (int)Main.player[m].position.Y, Main.player[m].width, Main.player[m].height);
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
						int num = Main.npc[npc].homeTileY - 1;
						while (num < Main.npc[npc].homeTileY + 2 && !WorldGen.StartRoomCheck(i, num))
						{
							num++;
						}
					}
				}
				if (!WorldGen.canSpawn)
				{
					int num2 = 10;
					for (int j = Main.npc[npc].homeTileX - num2; j <= Main.npc[npc].homeTileX + num2; j += 2)
					{
						int num3 = Main.npc[npc].homeTileY - num2;
						while (num3 <= Main.npc[npc].homeTileY + num2 && !WorldGen.StartRoomCheck(j, num3))
						{
							num3 += 2;
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
						return;
					}
					Main.npc[npc].homeless = true;
					return;
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
			int num3 = WorldGen.roomX1 - Main.screenWidth / 2 / 16 - 1 - 21;
			int num4 = WorldGen.roomX2 + Main.screenWidth / 2 / 16 + 1 + 21;
			int num5 = WorldGen.roomY1 - Main.screenHeight / 2 / 16 - 1 - 21;
			int num6 = WorldGen.roomY2 + Main.screenHeight / 2 / 16 + 1 + 21;
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
					if (Main.tile[l, m].active)
					{
						if (Main.tile[l, m].type == 23 || Main.tile[l, m].type == 24 || Main.tile[l, m].type == 25 || Main.tile[l, m].type == 32)
						{
							Main.evilTiles++;
						}
						else
						{
							if (Main.tile[l, m].type == 27)
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
					if (Main.tile[n, num8].active)
					{
						num = num7;
						if (Main.tileSolid[(int)Main.tile[n, num8].type] && !Main.tileSolidTop[(int)Main.tile[n, num8].type] && !Collision.SolidTiles(n - 1, n + 1, num8 - 3, num8 - 1) && Main.tile[n - 1, num8].active && Main.tileSolid[(int)Main.tile[n - 1, num8].type] && Main.tile[n + 1, num8].active && Main.tileSolid[(int)Main.tile[n + 1, num8].type])
						{
							for (int num9 = n - 2; num9 < n + 3; num9++)
							{
								for (int num10 = num8 - 4; num10 < num8; num10++)
								{
									if (Main.tile[num9, num10].active)
									{
										if (num9 == n)
										{
											num -= 15;
										}
										else
										{
											if (Main.tile[num9, num10].type == 10 || Main.tile[num9, num10].type == 11)
											{
												num -= 20;
											}
											else
											{
												if (Main.tileSolid[(int)Main.tile[num9, num10].type])
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
			for (int j = -2; j < 3; j++)
			{
				if (Main.wallHouse[(int)Main.tile[x + j, y].wall])
				{
					flag = true;
				}
				if (Main.tile[x + j, y].active && (Main.tileSolid[(int)Main.tile[x + j, y].type] || Main.tile[x + j, y].type == 11))
				{
					flag = true;
				}
				if (Main.wallHouse[(int)Main.tile[x, y + j].wall])
				{
					flag2 = true;
				}
				if (Main.tile[x, y + j].active && (Main.tileSolid[(int)Main.tile[x, y + j].type] || Main.tile[x, y + j].type == 11))
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
			if (Main.netMode == 1)
			{
				return;
			}
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
				int num5 = 5;
				while ((double)num5 < Main.worldSurface)
				{
					if (Main.tile[j, num5].active && Main.tile[j, num5].type == 37)
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
					if (Main.tile[num7, k].active && Main.tileSolid[(int)Main.tile[num7, k].type])
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
		public static bool meteor(int i, int j)
		{
			if (i < 50 || i > Main.maxTilesX - 50)
			{
				return false;
			}
			if (j < 50 || j > Main.maxTilesY - 50)
			{
				return false;
			}
			int num = 25;
			Rectangle rectangle = new Rectangle((i - num) * 16, (j - num) * 16, num * 2 * 16, num * 2 * 16);
			for (int k = 0; k < 255; k++)
			{
				if (Main.player[k].active)
				{
					Rectangle value = new Rectangle((int)(Main.player[k].position.X + (float)(Main.player[k].width / 2) - (float)(Main.screenWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2) - (float)(Main.screenHeight / 2) - (float)NPC.safeRangeY), Main.screenWidth + NPC.safeRangeX * 2, Main.screenHeight + NPC.safeRangeY * 2);
					if (rectangle.Intersects(value))
					{
						return false;
					}
				}
			}
			for (int l = 0; l < 1000; l++)
			{
				if (Main.npc[l].active)
				{
					Rectangle value2 = new Rectangle((int)Main.npc[l].position.X, (int)Main.npc[l].position.Y, Main.npc[l].width, Main.npc[l].height);
					if (rectangle.Intersects(value2))
					{
						return false;
					}
				}
			}
			for (int m = i - num; m < i + num; m++)
			{
				for (int n = j - num; n < j + num; n++)
				{
					if (Main.tile[m, n].active && Main.tile[m, n].type == 21)
					{
						return false;
					}
				}
			}
			WorldGen.stopDrops = true;
			num = 15;
			for (int num2 = i - num; num2 < i + num; num2++)
			{
				for (int num3 = j - num; num3 < j + num; num3++)
				{
					if (num3 > j + Main.rand.Next(-2, 3) - 5 && (double)(Math.Abs(i - num2) + Math.Abs(j - num3)) < (double)num * 1.5 + (double)Main.rand.Next(-5, 5))
					{
						if (!Main.tileSolid[(int)Main.tile[num2, num3].type])
						{
							Main.tile[num2, num3].active = false;
						}
						Main.tile[num2, num3].type = 37;
					}
				}
			}
			num = 10;
			for (int num4 = i - num; num4 < i + num; num4++)
			{
				for (int num5 = j - num; num5 < j + num; num5++)
				{
					if (num5 > j + Main.rand.Next(-2, 3) - 5 && Math.Abs(i - num4) + Math.Abs(j - num5) < num + Main.rand.Next(-3, 4))
					{
						Main.tile[num4, num5].active = false;
					}
				}
			}
			num = 16;
			for (int num6 = i - num; num6 < i + num; num6++)
			{
				for (int num7 = j - num; num7 < j + num; num7++)
				{
					if (Main.tile[num6, num7].type == 5 || Main.tile[num6, num7].type == 32)
					{
						WorldGen.KillTile(num6, num7, false, false, false);
					}
					WorldGen.SquareTileFrame(num6, num7, true);
					WorldGen.SquareWallFrame(num6, num7, true);
				}
			}
			num = 23;
			for (int num8 = i - num; num8 < i + num; num8++)
			{
				for (int num9 = j - num; num9 < j + num; num9++)
				{
					if (Main.tile[num8, num9].active && Main.rand.Next(10) == 0 && (double)(Math.Abs(i - num8) + Math.Abs(j - num9)) < (double)num * 1.3)
					{
						if (Main.tile[num8, num9].type == 5 || Main.tile[num8, num9].type == 32)
						{
							WorldGen.KillTile(num8, num9, false, false, false);
						}
						Main.tile[num8, num9].type = 37;
						WorldGen.SquareTileFrame(num8, num9, true);
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
			return true;
		}
		public static void setWorldSize()
		{
			Main.bottomWorld = (float)(Main.maxTilesY * 16);
			Main.rightWorld = (float)(Main.maxTilesX * 16);
			Main.maxSectionsX = Main.maxTilesX / 200;
			Main.maxSectionsY = Main.maxTilesY / 150;
		}
		public static void worldGenCallBack(object threadContext)
		{
			//Main.PlaySound(10, -1, -1, 1);
			//WorldGen.clearWorld();
			//WorldGen.generateWorld(-1);
            //WorldGen.saveWorld(Program.server.getWorld().getSavePath(), true);
			//Main.LoadWorlds();
            //if (Main.menuMode == 10)
            //{
            //    Main.menuMode = 6;
            //}
			//Main.PlaySound(10, -1, -1, 1);
		}
		public static void CreateNewWorld()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.worldGenCallBack), 1);
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
			if (WorldGen.loadFailed)
			{
				return;
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
			if (WorldGen.loadFailed)
			{
				return;
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
			if (Main.netMode == 1 || WorldGen.lastMaxTilesX > Main.maxTilesX || WorldGen.lastMaxTilesY > Main.maxTilesY)
			{
				for (int i = 0; i < WorldGen.lastMaxTilesX; i++)
				{
					float num = (float)i / (float)WorldGen.lastMaxTilesX;
					Program.printData("Freeing unused resources: " + (int)(num * 100f + 1f) + "%");
					for (int j = 0; j < WorldGen.lastMaxTilesY; j++)
					{
						Main.tile[i, j] = null;
					}
				}
                Console.WriteLine();
            }
			WorldGen.lastMaxTilesX = Main.maxTilesX;
			WorldGen.lastMaxTilesY = Main.maxTilesY;
			if (Main.netMode != 1)
			{
				for (int k = 0; k < Main.maxTilesX; k++)
				{
					float num2 = (float)k / (float)Main.maxTilesX;
					Program.printData("Resetting game objects: " + (int)(num2 * 100f + 1f) + "%");
					for (int l = 0; l < Main.maxTilesY; l++)
					{
						Main.tile[k, l] = new Tile();
					}
				}
                Console.WriteLine();
            }
			for (int m = 0; m < 2000; m++)
			{
				Main.dust[m] = new Dust();
			}
			for (int n = 0; n < 200; n++)
			{
				Main.gore[n] = new Gore();
			}
			for (int num3 = 0; num3 < 200; num3++)
			{
				Main.item[num3] = new Item();
			}
			for (int num4 = 0; num4 < 1000; num4++)
			{
				Main.npc[num4] = new NPC();
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
		}
		public static void saveWorld(string savePath, bool resetTime = false)
		{
			if (WorldGen.saveLock)
			{
				return;
			}
			WorldGen.saveLock = true;
			if (Main.skipMenu)
			{
				return;
			}
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
            if (savePath == null)
			{
				return;
			}
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
					for (int k = 0; k < 1000; k++)
					{
						if (Main.chest[k] == null)
						{
							binaryWriter.Write(false);
						}
						else
						{
							lock (Main.chest[k])
							{
								binaryWriter.Write(true);
								binaryWriter.Write(Main.chest[k].x);
								binaryWriter.Write(Main.chest[k].y);
								for (int l = 0; l < Chest.maxItems; l++)
								{
									binaryWriter.Write((byte)Main.chest[k].item[l].stack);
									if (Main.chest[k].item[l].stack > 0)
									{
										binaryWriter.Write(Main.chest[k].item[l].name);
									}
								}
							}
						}
					}
					for (int m = 0; m < 1000; m++)
					{
						if (Main.sign[m] == null || Main.sign[m].text == null)
						{
							binaryWriter.Write(false);
						}
						else
						{
							lock (Main.sign[m])
							{
								binaryWriter.Write(true);
								binaryWriter.Write(Main.sign[m].text);
								binaryWriter.Write(Main.sign[m].x);
								binaryWriter.Write(Main.sign[m].y);
							}
						}
					}
					for (int n = 0; n < 1000; n++)
					{
						lock (Main.npc[n])
						{
							if (Main.npc[n].active && Main.npc[n].townNPC)
							{
								binaryWriter.Write(true);
								binaryWriter.Write(Main.npc[n].name);
								binaryWriter.Write(Main.npc[n].position.X);
								binaryWriter.Write(Main.npc[n].position.Y);
								binaryWriter.Write(Main.npc[n].homeless);
								binaryWriter.Write(Main.npc[n].homeTileX);
								binaryWriter.Write(Main.npc[n].homeTileY);
							}
						}
					}
					binaryWriter.Write(false);
					binaryWriter.Close();
					if (File.Exists(savePath))
                    {
                        Console.WriteLine();
                        Console.Write("Backing up world file...");
                        string destFileName = savePath + ".bak";
                        File.Copy(savePath, destFileName, true);
					}
                    File.Copy(text, savePath, true);
					File.Delete(text);
				}
			}
            WorldGen.saveLock = false;
            Console.WriteLine();
		}
		public static void loadWorld()
		{
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
						int num = binaryReader.ReadInt32();
                        if (num > Statics.currentRelease)
                        {
                            Main.menuMode = 15;
                            Console.WriteLine("Incompatible World File!");
                            WorldGen.loadFailed = true;
                            binaryReader.Close();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Compatible World File");
                        }

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
						for (int k = 0; k < 1000; k++)
						{
							if (binaryReader.ReadBoolean())
							{
								Main.chest[k] = new Chest();
								Main.chest[k].x = binaryReader.ReadInt32();
								Main.chest[k].y = binaryReader.ReadInt32();
								for (int l = 0; l < Chest.maxItems; l++)
								{
									Main.chest[k].item[l] = new Item();
									byte b = binaryReader.ReadByte();
									if (b > 0)
									{
										string defaults = Item.VersionName(binaryReader.ReadString(), num);
										Main.chest[k].item[l].SetDefaults(defaults);
										Main.chest[k].item[l].stack = (int)b;
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
								if (Main.tile[num3, num4].active && Main.tile[num3, num4].type == 55)
								{
									Main.sign[m] = new Sign();
									Main.sign[m].x = num3;
									Main.sign[m].y = num4;
									Main.sign[m].text = text;
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
						binaryReader.Close();
                        Console.WriteLine();
                        WorldGen.gen = true;
						WorldGen.waterLine = Main.maxTilesY;
						Liquid.QuickWater(2, -1, -1);
						WorldGen.WaterCheck();
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
							Program.printData("Settling liquids: " + (int)(num9 * 100f / 2f + 50f) + "%");
							Liquid.UpdateLiquid();
						}
						Liquid.quickSettle = false;
						WorldGen.WaterCheck();
						WorldGen.gen = false;
					}
					catch (Exception arg_61D_0)
					{
						Exception exception = arg_61D_0;
						Main.menuMode = 15;
						Main.statusText = exception.ToString();
						WorldGen.loadFailed = true;
						try
						{
							binaryReader.Close();
						}
						catch
						{
						}
						return;
					}
                    Console.WriteLine();
                    WorldGen.loadFailed = false;
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

            Console.WriteLine("World Size: " + Main.maxTilesX + ", " + Main.maxTilesY);

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
				int num11 = 0;
				while ((double)num11 < num3)
				{
					Main.tile[i, num11].active = false;
					Main.tile[i, num11].lighted = true;
					Main.tile[i, num11].frameX = -1;
					Main.tile[i, num11].frameY = -1;
					num11++;
				}
				for (int j = (int)num3; j < Main.maxTilesY; j++)
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
			double num12 = (double)((int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6);
			Main.rockLayer = Main.worldSurface + num12;
			WorldGen.waterLine = (int)(Main.rockLayer + (double)Main.maxTilesY) / 2;
			WorldGen.waterLine += WorldGen.genRand.Next(-100, 20);
			WorldGen.lavaLine = WorldGen.waterLine + WorldGen.genRand.Next(50, 80);
			int num13 = 0;
            Console.WriteLine();
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
				int num16 = WorldGen.genRand.Next(15, 90);
				if (WorldGen.genRand.Next(3) == 0)
				{
					num16 *= 2;
				}
				int num17 = num15 - num16;
				num16 = WorldGen.genRand.Next(15, 90);
				if (WorldGen.genRand.Next(3) == 0)
				{
					num16 *= 2;
				}
				int num18 = num15 + num16;
				if (num17 < 0)
				{
					num17 = 0;
				}
				if (num18 > Main.maxTilesX)
				{
					num18 = Main.maxTilesX;
				}
				if (k == 0)
				{
					num17 = 0;
					num18 = WorldGen.genRand.Next(250, 300);
				}
				else
				{
					if (k == 2)
					{
						num17 = Main.maxTilesX - WorldGen.genRand.Next(250, 300);
						num18 = Main.maxTilesX;
					}
				}
				int num19 = WorldGen.genRand.Next(50, 100);
				for (int l = num17; l < num18; l++)
				{
					if (WorldGen.genRand.Next(2) == 0)
					{
						num19 += WorldGen.genRand.Next(-1, 2);
						if (num19 < 50)
						{
							num19 = 50;
						}
						if (num19 > 100)
						{
							num19 = 100;
						}
					}
					int num20 = 0;
					while ((double)num20 < Main.worldSurface)
					{
						if (Main.tile[l, num20].active)
						{
							int num21 = num19;
							if (l - num17 < num21)
							{
								num21 = l - num17;
							}
							if (num18 - l < num21)
							{
								num21 = num18 - l;
							}
							num21 += WorldGen.genRand.Next(5);
							for (int m = num20; m < num20 + num21; m++)
							{
								if (l > num17 + WorldGen.genRand.Next(5) && l < num18 - WorldGen.genRand.Next(5))
								{
									Main.tile[l, m].type = 53;
								}
							}
							break;
						}
						num20++;
					}
				}
			}
			for (int n = 0; n < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-06); n++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), (double)WorldGen.genRand.Next(15, 70), WorldGen.genRand.Next(20, 130), 53, false, 0f, 0f, false, true);
			}
			WorldGen.numMCaves = 0;
            Console.WriteLine();
            Program.printData("Generating hills...");
			for (int num22 = 0; num22 < (int)((double)Main.maxTilesX * 0.0008); num22++)
			{
				int num23 = 0;
				bool flag = false;
				bool flag2 = false;
				int num24 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
				while (!flag2)
				{
					flag2 = true;
					while (num24 > Main.maxTilesX / 2 - 100 && num24 < Main.maxTilesX / 2 + 100)
					{
						num24 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
					}
					for (int num25 = 0; num25 < WorldGen.numMCaves; num25++)
					{
						if (num24 > WorldGen.mCaveX[num25] - 50 && num24 < WorldGen.mCaveX[num25] + 50)
						{
							num23++;
							flag2 = false;
							break;
						}
					}
					if (num23 >= 200)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					int num26 = 0;
					while ((double)num26 < Main.worldSurface)
					{
						if (Main.tile[num24, num26].active)
						{
							WorldGen.Mountinater(num24, num26);
							WorldGen.mCaveX[WorldGen.numMCaves] = num24;
							WorldGen.mCaveY[WorldGen.numMCaves] = num26;
							WorldGen.numMCaves++;
							break;
						}
						num26++;
					}
				}
			}
            Console.WriteLine();
            for (int num27 = 1; num27 < Main.maxTilesX - 1; num27++)
			{
				float num28 = (float)num27 / (float)Main.maxTilesX;
				Program.printData("Puttin dirt behind dirt: " + (int)(num28 * 100f + 1f) + "%");
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
				int num29 = 0;
				while ((double)num29 < Main.worldSurface + 10.0 && (double)num29 <= Main.worldSurface + (double)num13)
				{
					if (flag3)
					{
						Main.tile[num27, num29].wall = 2;
					}
					if (Main.tile[num27, num29].active && Main.tile[num27 - 1, num29].active && Main.tile[num27 + 1, num29].active && Main.tile[num27, num29 + 1].active && Main.tile[num27 - 1, num29 + 1].active && Main.tile[num27 + 1, num29 + 1].active)
					{
						flag3 = true;
					}
					num29++;
				}
			}
			WorldGen.numIslandHouses = 0;
			WorldGen.houseCount = 0;
            Console.WriteLine();
            Program.printData("Generating floating islands...");
			for (int num30 = 0; num30 < (int)((double)Main.maxTilesX * 0.0008); num30++)
			{
				int num31 = 0;
				bool flag4 = false;
				int num32 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
				bool flag5 = false;
				while (!flag5)
				{
					flag5 = true;
					while (num32 > Main.maxTilesX / 2 - 80 && num32 < Main.maxTilesX / 2 + 80)
					{
						num32 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
					}
					for (int num33 = 0; num33 < WorldGen.numIslandHouses; num33++)
					{
						if (num32 > WorldGen.fihX[num33] - 80 && num32 < WorldGen.fihX[num33] + 80)
						{
							num31++;
							flag5 = false;
							break;
						}
					}
					if (num31 >= 200)
					{
						flag4 = true;
						break;
					}
				}
				if (!flag4)
				{
					int num34 = 200;
					while ((double)num34 < Main.worldSurface)
					{
						if (Main.tile[num32, num34].active)
						{
							int num35 = num32;
							int num36 = WorldGen.genRand.Next(100, num34 - 100);
							while ((double)num36 > num5 - 50.0)
							{
								num36--;
							}
							WorldGen.FloatingIsland(num35, num36);
							WorldGen.fihX[WorldGen.numIslandHouses] = num35;
							WorldGen.fihY[WorldGen.numIslandHouses] = num36;
							WorldGen.numIslandHouses++;
							break;
						}
						num34++;
					}
				}
			}
            Console.WriteLine();
            Program.printData("Placing rocks in the dirt...");
			for (int num37 = 0; num37 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num37++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5 + 1), (double)WorldGen.genRand.Next(4, 15), WorldGen.genRand.Next(5, 40), 1, false, 0f, 0f, false, true);
			}
			for (int num38 = 0; num38 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num38++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6 + 1), (double)WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
			}
			for (int num39 = 0; num39 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0045); num39++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8 + 1), (double)WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(2, 23), 1, false, 0f, 0f, false, true);
			}
            Console.WriteLine();
            Program.printData("Placing dirt in the rocks...");
			for (int num40 = 0; num40 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.005); num40++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 0, false, 0f, 0f, false, true);
			}
            Console.WriteLine();
            Program.printData("Adding clay...");
			for (int num41 = 0; num41 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num41++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5), (double)WorldGen.genRand.Next(4, 14), WorldGen.genRand.Next(10, 50), 40, false, 0f, 0f, false, true);
			}
			for (int num42 = 0; num42 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 5E-05); num42++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6 + 1), (double)WorldGen.genRand.Next(8, 14), WorldGen.genRand.Next(15, 45), 40, false, 0f, 0f, false, true);
			}
			for (int num43 = 0; num43 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num43++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8 + 1), (double)WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(5, 50), 40, false, 0f, 0f, false, true);
			}
			for (int num44 = 5; num44 < Main.maxTilesX - 5; num44++)
			{
				int num45 = 1;
				while ((double)num45 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num44, num45].active)
					{
						for (int num46 = num45; num46 < num45 + 5; num46++)
						{
							if (Main.tile[num44, num46].type == 40)
							{
								Main.tile[num44, num46].type = 0;
							}
						}
						break;
					}
					num45++;
				}
			}
            Console.WriteLine();
            for (int num47 = 0; num47 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015); num47++)
			{
				float num48 = (float)((double)num47 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015));
				Program.printData("Making random holes: " + (int)(num48 * 100f + 1f) + "%");
				int type = -1;
				if (WorldGen.genRand.Next(5) == 0)
				{
					type = -2;
				}
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 20), type, false, 0f, 0f, false, true);
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, Main.maxTilesY), (double)WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(7, 30), type, false, 0f, 0f, false, true);
			}
            Console.WriteLine();
            for (int num49 = 0; num49 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num49++)
			{
				float num50 = (float)((double)num49 / ((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05));
				Program.printData("Generating small caves: " + (int)(num50 * 100f + 1f) + "%");
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
            Console.WriteLine();
            for (int num51 = 0; num51 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015); num51++)
			{
				float num52 = (float)((double)num51 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015));
				Program.printData("Generating large caves: " + (int)(num52 * 100f + 1f) + "%");
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
            Console.WriteLine();
            Program.printData("Generating surface caves...");
			for (int num53 = 0; num53 < (int)((double)Main.maxTilesX * 0.0025); num53++)
			{
				int num54 = WorldGen.genRand.Next(0, Main.maxTilesX);
				int num55 = 0;
				while ((double)num55 < num6)
				{
					if (Main.tile[num54, num55].active)
					{
						WorldGen.TileRunner(num54, num55, (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(5, 50), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 1f, false, true);
						break;
					}
					num55++;
				}
			}
			for (int num56 = 0; num56 < (int)((double)Main.maxTilesX * 0.0007); num56++)
			{
				int num54 = WorldGen.genRand.Next(0, Main.maxTilesX);
				int num57 = 0;
				while ((double)num57 < num6)
				{
					if (Main.tile[num54, num57].active)
					{
						WorldGen.TileRunner(num54, num57, (double)WorldGen.genRand.Next(10, 15), WorldGen.genRand.Next(50, 130), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
						break;
					}
					num57++;
				}
			}
			for (int num58 = 0; num58 < (int)((double)Main.maxTilesX * 0.0003); num58++)
			{
				int num54 = WorldGen.genRand.Next(0, Main.maxTilesX);
				int num59 = 0;
				while ((double)num59 < num6)
				{
					if (Main.tile[num54, num59].active)
					{
						WorldGen.TileRunner(num54, num59, (double)WorldGen.genRand.Next(12, 25), WorldGen.genRand.Next(150, 500), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 4f, false, true);
						WorldGen.TileRunner(num54, num59, (double)WorldGen.genRand.Next(8, 17), WorldGen.genRand.Next(60, 200), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
						WorldGen.TileRunner(num54, num59, (double)WorldGen.genRand.Next(5, 13), WorldGen.genRand.Next(40, 170), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
						break;
					}
					num59++;
				}
			}
			for (int num60 = 0; num60 < (int)((double)Main.maxTilesX * 0.0004); num60++)
			{
				int num54 = WorldGen.genRand.Next(0, Main.maxTilesX);
				int num61 = 0;
				while ((double)num61 < num6)
				{
					if (Main.tile[num54, num61].active)
					{
						WorldGen.TileRunner(num54, num61, (double)WorldGen.genRand.Next(7, 12), WorldGen.genRand.Next(150, 250), -1, false, 0f, 1f, true, true);
						break;
					}
					num61++;
				}
			}
			for (int num62 = 0; num62 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002); num62++)
			{
				int num63 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
				int num64 = WorldGen.genRand.Next((int)num5, (int)num6);
				if (num64 >= Main.maxTilesY)
				{
					num64 = Main.maxTilesY - 2;
				}
				if (Main.tile[num63 - 1, num64].active && Main.tile[num63 - 1, num64].type == 0 && Main.tile[num63 + 1, num64].active && Main.tile[num63 + 1, num64].type == 0 && Main.tile[num63, num64 - 1].active && Main.tile[num63, num64 - 1].type == 0 && Main.tile[num63, num64 + 1].active && Main.tile[num63, num64 + 1].type == 0)
				{
					Main.tile[num63, num64].active = true;
					Main.tile[num63, num64].type = 2;
				}
				num63 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
				num64 = WorldGen.genRand.Next(0, (int)num5);
				if (num64 >= Main.maxTilesY)
				{
					num64 = Main.maxTilesY - 2;
				}
				if (Main.tile[num63 - 1, num64].active && Main.tile[num63 - 1, num64].type == 0 && Main.tile[num63 + 1, num64].active && Main.tile[num63 + 1, num64].type == 0 && Main.tile[num63, num64 - 1].active && Main.tile[num63, num64 - 1].type == 0 && Main.tile[num63, num64 + 1].active && Main.tile[num63, num64 + 1].type == 0)
				{
					Main.tile[num63, num64].active = true;
					Main.tile[num63, num64].type = 2;
				}
			}
            Console.WriteLine();
            Program.printData("Generating underground jungle: 0%");
			float num65 = (float)(Main.maxTilesX / 4200);
			num65 *= 1.5f;
			int num66 = 0;
			if (num9 == -1)
			{
				num66 = (int)((float)Main.maxTilesX * 0.8f);
			}
			else
			{
				num66 = (int)((float)Main.maxTilesX * 0.2f);
			}
			int num67 = (int)((double)Main.maxTilesY + Main.rockLayer) / 2;
			num66 += WorldGen.genRand.Next((int)(-100f * num65), (int)(101f * num65));
			num67 += WorldGen.genRand.Next((int)(-100f * num65), (int)(101f * num65));
			WorldGen.TileRunner(num66, num67, (double)WorldGen.genRand.Next((int)(250f * num65), (int)(500f * num65)), WorldGen.genRand.Next(50, 150), 59, false, (float)(num9 * 3), 0f, false, true);
			Program.printData("Generating underground jungle: 20%");
			num66 += WorldGen.genRand.Next((int)(-250f * num65), (int)(251f * num65));
			num67 += WorldGen.genRand.Next((int)(-150f * num65), (int)(151f * num65));
			int num68 = num66;
			int num69 = num67;
			WorldGen.TileRunner(num66, num67, (double)WorldGen.genRand.Next((int)(250f * num65), (int)(500f * num65)), WorldGen.genRand.Next(50, 150), 59, false, 0f, 0f, false, true);
			Program.printData("Generating underground jungle: 40%");
			num66 += WorldGen.genRand.Next((int)(-400f * num65), (int)(401f * num65));
			num67 += WorldGen.genRand.Next((int)(-150f * num65), (int)(151f * num65));
			WorldGen.TileRunner(num66, num67, (double)WorldGen.genRand.Next((int)(250f * num65), (int)(500f * num65)), WorldGen.genRand.Next(50, 150), 59, false, (float)(num9 * -3), 0f, false, true);
			Program.printData("Generating underground jungle: 60%");
			num66 = num68;
			num67 = num69;
			int num70 = 0;
			while ((float)num70 <= 20f * num65)
			{
				Program.printData("Generating underground jungle: " + (int)(60f + (float)num70 / num65) + "%");
				num66 += WorldGen.genRand.Next((int)(-5f * num65), (int)(6f * num65));
				num67 += WorldGen.genRand.Next((int)(-5f * num65), (int)(6f * num65));
				WorldGen.TileRunner(num66, num67, (double)WorldGen.genRand.Next(40, 100), WorldGen.genRand.Next(300, 500), 59, false, 0f, 0f, false, true);
				num70++;
			}
			int num71 = 0;
			while ((float)num71 <= 10f * num65)
			{
				Program.printData("Generating underground jungle: " + (int)(80f + (float)num71 / num65 * 2f) + "%");
				num66 = num68 + WorldGen.genRand.Next((int)(-600f * num65), (int)(600f * num65));
				num67 = num69 + WorldGen.genRand.Next((int)(-200f * num65), (int)(200f * num65));
				while (num66 < 1 || num66 >= Main.maxTilesX - 1 || num67 < 1 || num67 >= Main.maxTilesY - 1 || Main.tile[num66, num67].type != 59)
				{
					num66 = num68 + WorldGen.genRand.Next((int)(-600f * num65), (int)(600f * num65));
					num67 = num69 + WorldGen.genRand.Next((int)(-200f * num65), (int)(200f * num65));
				}
				int num72 = 0;
				while ((float)num72 < 8f * num65)
				{
					num66 += WorldGen.genRand.Next(-30, 31);
					num67 += WorldGen.genRand.Next(-30, 31);
					int type4 = -1;
					if (WorldGen.genRand.Next(7) == 0)
					{
						type4 = -2;
					}
					WorldGen.TileRunner(num66, num67, (double)WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(30, 70), type4, false, 0f, 0f, false, true);
					num72++;
				}
				num71++;
			}
			int num73 = 0;
			while ((float)num73 <= 300f * num65)
			{
				num66 = num68 + WorldGen.genRand.Next((int)(-600f * num65), (int)(600f * num65));
				num67 = num69 + WorldGen.genRand.Next((int)(-200f * num65), (int)(200f * num65));
				while (num66 < 1 || num66 >= Main.maxTilesX - 1 || num67 < 1 || num67 >= Main.maxTilesY - 1 || Main.tile[num66, num67].type != 59)
				{
					num66 = num68 + WorldGen.genRand.Next((int)(-600f * num65), (int)(600f * num65));
					num67 = num69 + WorldGen.genRand.Next((int)(-200f * num65), (int)(200f * num65));
				}
				WorldGen.TileRunner(num66, num67, (double)WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
				if (WorldGen.genRand.Next(4) == 0)
				{
					int type5 = WorldGen.genRand.Next(63, 69);
					WorldGen.TileRunner(num66 + WorldGen.genRand.Next(-1, 2), num67 + WorldGen.genRand.Next(-1, 2), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(4, 8), type5, false, 0f, 0f, false, true);
				}
				num73++;
			}
			num66 = num68;
			num67 = num69;
			float num74 = (float)WorldGen.genRand.Next(6, 10);
			float num75 = (float)(Main.maxTilesX / 4200);
			num74 *= num75;
			int num76 = 0;
			while ((float)num76 < num74)
			{
				bool flag6 = true;
				while (flag6)
				{
					num66 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					num67 = WorldGen.genRand.Next(20, Main.maxTilesY - 300);
					if (Main.tile[num66, num67].type == 59)
					{
						flag6 = false;
						int num77 = WorldGen.genRand.Next(2, 4);
						int num78 = WorldGen.genRand.Next(2, 4);
						for (int num79 = num66 - num77 - 1; num79 <= num66 + num77 + 1; num79++)
						{
							for (int num80 = num67 - num78 - 1; num80 <= num67 + num78 + 1; num80++)
							{
								Main.tile[num79, num80].active = true;
								Main.tile[num79, num80].type = 45;
								Main.tile[num79, num80].liquid = 0;
								Main.tile[num79, num80].lava = false;
							}
						}
						for (int num81 = num66 - num77; num81 <= num66 + num77; num81++)
						{
							for (int num82 = num67 - num78; num82 <= num67 + num78; num82++)
							{
								Main.tile[num81, num82].active = false;
								Main.tile[num81, num82].wall = 10;
							}
						}
						bool flag7 = false;
						int num83 = 0;
						while (!flag7 && num83 < 100)
						{
							num83++;
							int num84 = WorldGen.genRand.Next(num66 - num77, num66 + num77 + 1);
							int num85 = WorldGen.genRand.Next(num67 - num78, num67 + num78 - 2);
							WorldGen.PlaceTile(num84, num85, 4, true, false, -1);
							if (Main.tile[num84, num85].type == 4)
							{
								flag7 = true;
							}
						}
						for (int num86 = num66 - num77 - 1; num86 <= num66 + num77 + 1; num86++)
						{
							for (int num87 = num67 + num78 - 2; num87 <= num67 + num78; num87++)
							{
								Main.tile[num86, num87].active = false;
							}
						}
						for (int num88 = num66 - num77 - 1; num88 <= num66 + num77 + 1; num88++)
						{
							for (int num89 = num67 + num78 - 2; num89 <= num67 + num78 - 1; num89++)
							{
								Main.tile[num88, num89].active = false;
							}
						}
						for (int num90 = num66 - num77 - 1; num90 <= num66 + num77 + 1; num90++)
						{
							int num91 = 4;
							int num92 = num67 + num78 + 2;
							while (!Main.tile[num90, num92].active && num92 < Main.maxTilesY && num91 > 0)
							{
								Main.tile[num90, num92].active = true;
								Main.tile[num90, num92].type = 59;
								num92++;
								num91--;
							}
						}
						num77 -= WorldGen.genRand.Next(1, 3);
						int num93 = num67 - num78 - 2;
						while (num77 > -1)
						{
							for (int num94 = num66 - num77 - 1; num94 <= num66 + num77 + 1; num94++)
							{
								Main.tile[num94, num93].active = true;
								Main.tile[num94, num93].type = 45;
							}
							num77 -= WorldGen.genRand.Next(1, 3);
							num93--;
						}
						WorldGen.JChestX[WorldGen.numJChests] = num66;
						WorldGen.JChestY[WorldGen.numJChests] = num67;
						WorldGen.numJChests++;
					}
				}
				num76++;
			}
			for (int num95 = 0; num95 < Main.maxTilesX; num95++)
			{
				for (int num96 = (int)Main.worldSurface; num96 < Main.maxTilesY; num96++)
				{
					if (Main.tile[num95, num96].active)
					{
						WorldGen.SpreadGrass(num95, num96, 59, 60, false);
					}
				}
			}
			Console.WriteLine();
			Program.printData("Adding mushroom patches...");
			for (int num97 = 0; num97 < Main.maxTilesX / 300; num97++)
			{
				int i2 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.3), (int)((double)Main.maxTilesX * 0.7));
				int j2 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);
				WorldGen.ShroomPatch(i2, j2);
			}
			for (int num98 = 0; num98 < Main.maxTilesX; num98++)
			{
				for (int num99 = (int)Main.worldSurface; num99 < Main.maxTilesY; num99++)
				{
					if (Main.tile[num98, num99].active)
					{
						WorldGen.SpreadGrass(num98, num99, 59, 70, false);
					}
				}
			}
            Console.WriteLine();
            Program.printData("Placing mud in the dirt...");
			for (int num100 = 0; num100 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001); num100++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 59, false, 0f, 0f, false, true);
			}
            Console.WriteLine();
            Program.printData("Adding shinies...");
			for (int num101 = 0; num101 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); num101++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), 7, false, 0f, 0f, false, true);
			}
			for (int num102 = 0; num102 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num102++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), 7, false, 0f, 0f, false, true);
			}
			for (int num103 = 0; num103 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num103++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 7, false, 0f, 0f, false, true);
			}
			for (int num104 = 0; num104 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num104++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(2, 5), 6, false, 0f, 0f, false, true);
			}
			for (int num105 = 0; num105 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num105++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), 6, false, 0f, 0f, false, true);
			}
			for (int num106 = 0; num106 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num106++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 6, false, 0f, 0f, false, true);
			}
			for (int num107 = 0; num107 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num107++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), 9, false, 0f, 0f, false, true);
			}
			for (int num108 = 0; num108 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00017); num108++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
			}
			for (int num109 = 0; num109 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00017); num109++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
			}
			for (int num110 = 0; num110 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num110++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
			}
			for (int num111 = 0; num111 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num111++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5 - 20), (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
			}
            Console.WriteLine();
            Program.printData("Adding webs...");
			for (int num112 = 0; num112 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001); num112++)
			{
				int num113 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
				int num114 = WorldGen.genRand.Next((int)num5, Main.maxTilesY - 20);
				if (num112 < WorldGen.numMCaves)
				{
					num113 = WorldGen.mCaveX[num112];
					num114 = WorldGen.mCaveY[num112];
				}
				if (!Main.tile[num113, num114].active)
				{
					if ((double)num114 <= Main.worldSurface)
					{
						if (Main.tile[num113, num114].wall <= 0)
						{
							goto IL_2A58;
						}
					}
					while (!Main.tile[num113, num114].active && num114 > (int)num5)
					{
						num114--;
					}
					num114++;
					int num115 = 1;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num115 = -1;
					}
					while (!Main.tile[num113, num114].active && num113 > 10 && num113 < Main.maxTilesX - 10)
					{
						num113 += num115;
					}
					num113 -= num115;
					if ((double)num114 > Main.worldSurface || Main.tile[num113, num114].wall > 0)
					{
						WorldGen.TileRunner(num113, num114, (double)WorldGen.genRand.Next(4, 13), WorldGen.genRand.Next(2, 5), 51, true, (float)num115, -1f, false, false);
					}
				}
				IL_2A58:;
			}
            Console.WriteLine();
            Program.printData("Creating underworld: 0%");
			int num116 = Main.maxTilesY - WorldGen.genRand.Next(150, 190);
			for (int num117 = 0; num117 < Main.maxTilesX; num117++)
			{
				num116 += WorldGen.genRand.Next(-3, 4);
				if (num116 < Main.maxTilesY - 190)
				{
					num116 = Main.maxTilesY - 190;
				}
				if (num116 > Main.maxTilesY - 160)
				{
					num116 = Main.maxTilesY - 160;
				}
				for (int num118 = num116 - 20 - WorldGen.genRand.Next(3); num118 < Main.maxTilesY; num118++)
				{
					if (num118 >= num116)
					{
						Main.tile[num117, num118].active = false;
						Main.tile[num117, num118].lava = false;
						Main.tile[num117, num118].liquid = 0;
					}
					else
					{
						Main.tile[num117, num118].type = 57;
					}
				}
			}
			int num119 = Main.maxTilesY - WorldGen.genRand.Next(40, 70);
			for (int num120 = 10; num120 < Main.maxTilesX - 10; num120++)
			{
				num119 += WorldGen.genRand.Next(-10, 11);
				if (num119 > Main.maxTilesY - 60)
				{
					num119 = Main.maxTilesY - 60;
				}
				if (num119 < Main.maxTilesY - 100)
				{
					num119 = Main.maxTilesY - 120;
				}
				for (int num121 = num119; num121 < Main.maxTilesY - 10; num121++)
				{
					if (!Main.tile[num120, num121].active)
					{
						Main.tile[num120, num121].lava = true;
						Main.tile[num120, num121].liquid = 255;
					}
				}
			}
			for (int num122 = 0; num122 < Main.maxTilesX; num122++)
			{
				if (WorldGen.genRand.Next(50) == 0)
				{
					int num123 = Main.maxTilesY - 65;
					while (!Main.tile[num122, num123].active && num123 > Main.maxTilesY - 135)
					{
						num123--;
					}
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), num123 + WorldGen.genRand.Next(20, 50), (double)WorldGen.genRand.Next(15, 20), 1000, 57, true, 0f, (float)WorldGen.genRand.Next(1, 3), true, true);
				}
			}
			Liquid.QuickWater(-2, -1, -1);
			for (int num124 = 0; num124 < Main.maxTilesX; num124++)
			{
				float num125 = (float)num124 / (float)(Main.maxTilesX - 1);
				Program.printData("Creating underworld: " + (int)(num125 * 100f / 2f + 50f) + "%");
				if (WorldGen.genRand.Next(13) == 0)
				{
					int num126 = Main.maxTilesY - 65;
					while ((Main.tile[num124, num126].liquid > 0 || Main.tile[num124, num126].active) && num126 > Main.maxTilesY - 140)
					{
						num126--;
					}
					WorldGen.TileRunner(num124, num126 - WorldGen.genRand.Next(2, 5), (double)WorldGen.genRand.Next(5, 30), 1000, 57, true, 0f, (float)WorldGen.genRand.Next(1, 3), true, true);
					float num127 = (float)WorldGen.genRand.Next(1, 3);
					if (WorldGen.genRand.Next(3) == 0)
					{
						num127 *= 0.5f;
					}
					if (WorldGen.genRand.Next(2) == 0)
					{
						WorldGen.TileRunner(num124, num126 - WorldGen.genRand.Next(2, 5), (double)((int)((float)WorldGen.genRand.Next(5, 15) * num127)), (int)((float)WorldGen.genRand.Next(10, 15) * num127), 57, true, 1f, 0.3f, false, true);
					}
					if (WorldGen.genRand.Next(2) == 0)
					{
						num127 = (float)WorldGen.genRand.Next(1, 3);
						WorldGen.TileRunner(num124, num126 - WorldGen.genRand.Next(2, 5), (double)((int)((float)WorldGen.genRand.Next(5, 15) * num127)), (int)((float)WorldGen.genRand.Next(10, 15) * num127), 57, true, -1f, 0.3f, false, true);
					}
					WorldGen.TileRunner(num124 + WorldGen.genRand.Next(-10, 10), num126 + WorldGen.genRand.Next(-10, 10), (double)WorldGen.genRand.Next(5, 15), WorldGen.genRand.Next(5, 10), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
					if (WorldGen.genRand.Next(3) == 0)
					{
						WorldGen.TileRunner(num124 + WorldGen.genRand.Next(-10, 10), num126 + WorldGen.genRand.Next(-10, 10), (double)WorldGen.genRand.Next(10, 30), WorldGen.genRand.Next(10, 20), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
					}
					if (WorldGen.genRand.Next(5) == 0)
					{
						WorldGen.TileRunner(num124 + WorldGen.genRand.Next(-15, 15), num126 + WorldGen.genRand.Next(-15, 10), (double)WorldGen.genRand.Next(15, 30), WorldGen.genRand.Next(5, 20), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
					}
				}
			}
			for (int num128 = 0; num128 < Main.maxTilesX; num128++)
			{
				if (!Main.tile[num128, Main.maxTilesY - 145].active)
				{
					Main.tile[num128, Main.maxTilesY - 145].liquid = 255;
					Main.tile[num128, Main.maxTilesY - 145].lava = true;
				}
				if (!Main.tile[num128, Main.maxTilesY - 144].active)
				{
					Main.tile[num128, Main.maxTilesY - 144].liquid = 255;
					Main.tile[num128, Main.maxTilesY - 144].lava = true;
				}
			}
			for (int num129 = 0; num129 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002); num129++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(Main.maxTilesY - 140, Main.maxTilesY), (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), 58, false, 0f, 0f, false, true);
			}
			WorldGen.AddHellHouses();
			int num130 = WorldGen.genRand.Next(2, (int)((double)Main.maxTilesX * 0.005));
            Console.WriteLine();
            for (int num131 = 0; num131 < num130; num131++)
			{
				float num132 = (float)num131 / (float)num130;
				Program.printData("Adding water bodies: " + (int)(num132 * 100f) + "%");
				int num133 = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
				while (num133 > Main.maxTilesX / 2 - 50 && num133 < Main.maxTilesX / 2 + 50)
				{
					num133 = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
				}
				int num134 = (int)num5 - 20;
				while (!Main.tile[num133, num134].active)
				{
					num134++;
				}
				WorldGen.Lakinater(num133, num134);
			}
			int x = 0;
			if (num9 == -1)
			{
				x = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.05), (int)((double)Main.maxTilesX * 0.2));
				num9 = -1;
			}
			else
			{
				x = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.8), (int)((double)Main.maxTilesX * 0.95));
				num9 = 1;
			}
            int y = (int)((Main.rockLayer + (double)Main.maxTilesY) / 2.0) + WorldGen.genRand.Next(-200, 200);
            Console.WriteLine();
			WorldGen.MakeDungeon(x, y, 41, 7);
			int num135 = 0;
            Console.WriteLine();
            while ((double)num135 < (double)Main.maxTilesX * 0.0004)
			{
				float num136 = (float)((double)num135 / ((double)Main.maxTilesX * 0.0004));
				Program.printData("Making the world evil: " + (int)(num136 * 100f) + "%");
				bool flag8 = false;
				int num137 = 0;
				int num138 = 0;
				int num139 = 0;
				while (!flag8)
				{
					flag8 = true;
					int num140 = Main.maxTilesX / 2;
					int num141 = 200;
					num137 = WorldGen.genRand.Next(Main.maxTilesX);
					num138 = num137 - WorldGen.genRand.Next(150) - 175;
					num139 = num137 + WorldGen.genRand.Next(150) + 175;
					if (num138 < 0)
					{
						num138 = 0;
					}
					if (num139 > Main.maxTilesX)
					{
						num139 = Main.maxTilesX;
					}
					if (num137 > num140 - num141 && num137 < num140 + num141)
					{
						flag8 = false;
					}
					if (num138 > num140 - num141 && num138 < num140 + num141)
					{
						flag8 = false;
					}
					if (num139 > num140 - num141 && num139 < num140 + num141)
					{
						flag8 = false;
					}
					for (int num142 = num138; num142 < num139; num142++)
					{
						for (int num143 = 0; num143 < (int)Main.worldSurface; num143 += 5)
						{
							if (Main.tile[num142, num143].active && Main.tileDungeon[(int)Main.tile[num142, num143].type])
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
				int num144 = 0;
				for (int num145 = num138; num145 < num139; num145++)
				{
					if (num144 > 0)
					{
						num144--;
					}
					if (num145 == num137 || num144 == 0)
					{
						int num146 = (int)num5;
						while ((double)num146 < Main.worldSurface - 1.0)
						{
							if (Main.tile[num145, num146].active || Main.tile[num145, num146].wall > 0)
							{
								if (num145 == num137)
								{
									num144 = 20;
									WorldGen.ChasmRunner(num145, num146, WorldGen.genRand.Next(150) + 150, true);
									break;
								}
								if (WorldGen.genRand.Next(30) == 0 && num144 == 0)
								{
									num144 = 20;
									bool makeOrb = false;
									if (WorldGen.genRand.Next(2) == 0)
									{
										makeOrb = true;
									}
									WorldGen.ChasmRunner(num145, num146, WorldGen.genRand.Next(50) + 50, makeOrb);
									break;
								}
								break;
							}
							else
							{
								num146++;
							}
						}
					}
				}
				double num147 = Main.worldSurface + 40.0;
				for (int num148 = num138; num148 < num139; num148++)
				{
					num147 += (double)WorldGen.genRand.Next(-2, 3);
					if (num147 < Main.worldSurface + 30.0)
					{
						num147 = Main.worldSurface + 30.0;
					}
					if (num147 > Main.worldSurface + 50.0)
					{
						num147 = Main.worldSurface + 50.0;
					}
					int num54 = num148;
					bool flag9 = false;
					int num149 = (int)num5;
					while ((double)num149 < num147)
					{
						if (Main.tile[num54, num149].active)
						{
							if (Main.tile[num54, num149].type == 0 && (double)num149 < Main.worldSurface - 1.0 && !flag9)
							{
								WorldGen.SpreadGrass(num54, num149, 0, 23, true);
							}
							flag9 = true;
							if (Main.tile[num54, num149].type == 1 && num54 >= num138 + WorldGen.genRand.Next(5) && num54 <= num139 - WorldGen.genRand.Next(5))
							{
								Main.tile[num54, num149].type = 25;
							}
							if (Main.tile[num54, num149].type == 2)
							{
								Main.tile[num54, num149].type = 23;
							}
						}
						num149++;
					}
				}
				for (int num150 = num138; num150 < num139; num150++)
				{
					for (int num151 = 0; num151 < Main.maxTilesY - 50; num151++)
					{
						if (Main.tile[num150, num151].active && Main.tile[num150, num151].type == 31)
						{
							int num152 = num150 - 13;
							int num153 = num150 + 13;
							int num154 = num151 - 13;
							int num155 = num151 + 13;
							for (int num156 = num152; num156 < num153; num156++)
							{
								if (num156 > 10 && num156 < Main.maxTilesX - 10)
								{
									for (int num157 = num154; num157 < num155; num157++)
									{
										if (Math.Abs(num156 - num150) + Math.Abs(num157 - num151) < 9 + WorldGen.genRand.Next(11) && WorldGen.genRand.Next(3) != 0 && Main.tile[num156, num157].type != 31)
										{
											Main.tile[num156, num157].active = true;
											Main.tile[num156, num157].type = 25;
											if (Math.Abs(num156 - num150) <= 1 && Math.Abs(num157 - num151) <= 1)
											{
												Main.tile[num156, num157].active = false;
											}
										}
										if (Main.tile[num156, num157].type != 31 && Math.Abs(num156 - num150) <= 2 + WorldGen.genRand.Next(3) && Math.Abs(num157 - num151) <= 2 + WorldGen.genRand.Next(3))
										{
											Main.tile[num156, num157].active = false;
										}
									}
								}
							}
						}
					}
				}
				num135++;
			}
            Console.WriteLine();
            Program.printData("Generating mountain caves...");
			for (int num158 = 0; num158 < WorldGen.numMCaves; num158++)
			{
				int i3 = WorldGen.mCaveX[num158];
				int j3 = WorldGen.mCaveY[num158];
				WorldGen.CaveOpenater(i3, j3);
				WorldGen.Cavinator(i3, j3, WorldGen.genRand.Next(40, 50));
			}
            Console.WriteLine();
            Program.printData("Creating beaches...");
			for (int num159 = 0; num159 < 2; num159++)
			{
				if (num159 == 0)
				{
					int num160 = 0;
					int num161 = WorldGen.genRand.Next(125, 200);
					float num162 = 1f;
					int num163 = 0;
					while (!Main.tile[num161 - 1, num163].active)
					{
						num163++;
					}
					for (int num164 = num161 - 1; num164 >= num160; num164--)
					{
						num162 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
						int num165 = 0;
						while ((float)num165 < (float)num163 + num162)
						{
							if ((float)num165 < (float)num163 + num162 * 0.75f - 3f)
							{
								Main.tile[num164, num165].active = false;
								if (num165 > num163)
								{
									Main.tile[num164, num165].liquid = 255;
								}
								else
								{
									if (num165 == num163)
									{
										Main.tile[num164, num165].liquid = 127;
									}
								}
							}
							else
							{
								if (num165 > num163)
								{
									Main.tile[num164, num165].type = 53;
									Main.tile[num164, num165].active = true;
								}
							}
							Main.tile[num164, num165].wall = 0;
							num165++;
						}
					}
				}
				else
				{
					int num160 = Main.maxTilesX - WorldGen.genRand.Next(125, 200);
					int num161 = Main.maxTilesX;
					float num166 = 1f;
					int num167 = 0;
					while (!Main.tile[num160, num167].active)
					{
						num167++;
					}
					for (int num168 = num160; num168 < num161; num168++)
					{
						num166 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
						int num169 = 0;
						while ((float)num169 < (float)num167 + num166)
						{
							if ((float)num169 < (float)num167 + num166 * 0.75f - 3f)
							{
								Main.tile[num168, num169].active = false;
								if (num169 > num167)
								{
									Main.tile[num168, num169].liquid = 255;
								}
								else
								{
									if (num169 == num167)
									{
										Main.tile[num168, num169].liquid = 127;
									}
								}
							}
							else
							{
								if (num169 > num167)
								{
									Main.tile[num168, num169].type = 53;
									Main.tile[num168, num169].active = true;
								}
							}
							Main.tile[num168, num169].wall = 0;
							num169++;
						}
					}
				}
			}
            Console.WriteLine();
            Program.printData("Adding gems...");
			for (int num170 = 63; num170 <= 68; num170++)
			{
				float num171 = 0f;
				if (num170 == 67)
				{
					num171 = (float)Main.maxTilesX * 0.5f;
				}
				else
				{
					if (num170 == 66)
					{
						num171 = (float)Main.maxTilesX * 0.45f;
					}
					else
					{
						if (num170 == 63)
						{
							num171 = (float)Main.maxTilesX * 0.3f;
						}
						else
						{
							if (num170 == 65)
							{
								num171 = (float)Main.maxTilesX * 0.25f;
							}
							else
							{
								if (num170 == 64)
								{
									num171 = (float)Main.maxTilesX * 0.1f;
								}
								else
								{
									if (num170 == 68)
									{
										num171 = (float)Main.maxTilesX * 0.05f;
									}
								}
							}
						}
					}
				}
				num171 *= 0.2f;
				int num172 = 0;
				while ((float)num172 < num171)
				{
					int num173 = WorldGen.genRand.Next(0, Main.maxTilesX);
					int num174 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
					while (Main.tile[num173, num174].type != 1)
					{
						num173 = WorldGen.genRand.Next(0, Main.maxTilesX);
						num174 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
					}
					WorldGen.TileRunner(num173, num174, (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(3, 7), num170, false, 0f, 0f, false, true);
					num172++;
				}
			}
            Console.WriteLine();
            for (int num175 = 0; num175 < Main.maxTilesX; num175++)
			{
				float num176 = (float)num175 / (float)(Main.maxTilesX - 1);
				Program.printData("Gravitating sand: " + (int)(num176 * 100f) + "%");
				for (int num177 = Main.maxTilesY - 5; num177 > 0; num177--)
				{
					if (Main.tile[num175, num177].active && Main.tile[num175, num177].type == 53)
					{
						int num178 = num177;
						while (!Main.tile[num175, num178 + 1].active && num178 < Main.maxTilesY - 5)
						{
							Main.tile[num175, num178 + 1].active = true;
							Main.tile[num175, num178 + 1].type = 53;
							num178++;
						}
					}
				}
			}
            Console.WriteLine();
            for (int num179 = 3; num179 < Main.maxTilesX - 3; num179++)
			{
				float num180 = (float)num179 / (float)Main.maxTilesX;
				Program.printData("Cleaning up dirt backgrounds: " + (int)(num180 * 100f + 1f) + "%");
				int num181 = 0;
				while ((double)num181 < Main.worldSurface)
				{
					if (Main.tile[num179, num181].wall == 2)
					{
						Main.tile[num179, num181].wall = 0;
					}
					if (Main.tile[num179, num181].type != 53)
					{
						if (Main.tile[num179 - 1, num181].wall == 2)
						{
							Main.tile[num179 - 1, num181].wall = 0;
						}
						if (Main.tile[num179 - 2, num181].wall == 2 && WorldGen.genRand.Next(2) == 0)
						{
							Main.tile[num179 - 2, num181].wall = 0;
						}
						if (Main.tile[num179 - 3, num181].wall == 2 && WorldGen.genRand.Next(2) == 0)
						{
							Main.tile[num179 - 3, num181].wall = 0;
						}
						if (Main.tile[num179 + 1, num181].wall == 2)
						{
							Main.tile[num179 + 1, num181].wall = 0;
						}
						if (Main.tile[num179 + 2, num181].wall == 2 && WorldGen.genRand.Next(2) == 0)
						{
							Main.tile[num179 + 2, num181].wall = 0;
						}
						if (Main.tile[num179 + 3, num181].wall == 2 && WorldGen.genRand.Next(2) == 0)
						{
							Main.tile[num179 + 3, num181].wall = 0;
						}
						if (Main.tile[num179, num181].active)
						{
							break;
						}
					}
					num181++;
				}
			}
            Console.WriteLine();
            for (int num182 = 0; num182 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num182++)
			{
				float num183 = (float)((double)num182 / ((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05));
				Program.printData("Placing alters: " + (int)(num183 * 100f + 1f) + "%");
				bool flag10 = false;
				int num184 = 0;
				while (!flag10)
				{
					int num185 = WorldGen.genRand.Next(1, Main.maxTilesX);
					int num186 = (int)(num6 + 20.0);
					WorldGen.Place3x2(num185, num186, 26);
					if (Main.tile[num185, num186].type == 26)
					{
						flag10 = true;
					}
					else
					{
						num184++;
						if (num184 >= 10000)
						{
							flag10 = true;
						}
					}
				}
			}
            Console.WriteLine();
            Liquid.QuickWater(3, -1, -1);
			WorldGen.WaterCheck();
			int num187 = 0;
			Liquid.quickSettle = true;
			while (num187 < 10)
			{
				int num188 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
				num187++;
				float num189 = 0f;
				while (Liquid.numLiquid > 0)
				{
					float num190 = (float)(num188 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num188;
					if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num188)
					{
						num188 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
					}
					if (num190 > num189)
					{
						num189 = num190;
					}
					else
					{
						num190 = num189;
					}
					if (num187 == 1)
					{
						Program.printData("Settling liquids: " + (int)(num190 * 100f / 3f + 33f) + "%");
					}
					int num191 = 10;
					if (num187 <= num191)
					{
						goto IL_41A9;
					}
					IL_41A9:
					Liquid.UpdateLiquid();
				}
				WorldGen.WaterCheck();
				Program.printData("Settling liquids: " + (int)((float)num187 * 10f / 3f + 66f) + "%");
			}
			Liquid.quickSettle = false;
            Console.WriteLine();
            for (int num192 = 0; num192 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.5E-05); num192++)
			{
				float num193 = (float)((double)num192 / ((double)(Main.maxTilesX * Main.maxTilesY) * 2.5E-05));
				Program.printData("Placing life crystals: " + (int)(num193 * 100f + 1f) + "%");
				bool flag11 = false;
				int num194 = 0;
				while (!flag11)
				{
					if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY)))
					{
						flag11 = true;
					}
					else
					{
						num194++;
						if (num194 >= 10000)
						{
							flag11 = true;
						}
					}
				}
			}
            Console.WriteLine();
            for (int num195 = 0; num195 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 1.8E-05); num195++)
			{
				float num196 = (float)((double)num195 / ((double)(Main.maxTilesX * Main.maxTilesY) * 1.8E-05));
				Program.printData("Hiding treasure: " + (int)(num196 * 100f + 1f) + "%");
				bool flag12 = false;
				int num197 = 0;
				while (!flag12)
				{
					if (WorldGen.AddBuriedChest(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY), 0))
					{
						flag12 = true;
					}
					else
					{
						num197++;
						if (num197 >= 10000)
						{
							flag12 = true;
						}
					}
				}
			}
			int num198 = 0;
			for (int num199 = 0; num199 < WorldGen.numJChests; num199++)
			{
				num198++;
				int contain = 211;
				if (num198 == 1)
				{
					contain = 211;
				}
				else
				{
					if (num198 == 2)
					{
						contain = 212;
					}
					else
					{
						if (num198 == 3)
						{
							contain = 213;
						}
					}
				}
				if (num198 > 3)
				{
					num198 = 0;
				}
				if (!WorldGen.AddBuriedChest(WorldGen.JChestX[num199] + WorldGen.genRand.Next(2), WorldGen.JChestY[num199], contain))
				{
					for (int num200 = WorldGen.JChestX[num199]; num200 <= WorldGen.JChestX[num199] + 1; num200++)
					{
						for (int num201 = WorldGen.JChestY[num199]; num201 <= WorldGen.JChestY[num199] + 1; num201++)
						{
							WorldGen.KillTile(num200, num201, false, false, false);
						}
					}
					WorldGen.AddBuriedChest(WorldGen.JChestX[num199], WorldGen.JChestY[num199], contain);
				}
			}
			float num202 = (float)(Main.maxTilesX / 4200);
			int num203 = 0;
			int num204 = 0;
			while ((float)num204 < 10f * num202)
			{
				int contain2 = 0;
				num203++;
				if (num203 == 1)
				{
					contain2 = 186;
				}
				else
				{
					contain2 = 187;
					num203 = 0;
				}
				bool flag13 = false;
				while (!flag13)
				{
					int num205 = WorldGen.genRand.Next(1, Main.maxTilesX);
					int num206 = WorldGen.genRand.Next(1, Main.maxTilesY - 200);
					while (Main.tile[num205, num206].liquid < 200 || Main.tile[num205, num206].lava)
					{
						num205 = WorldGen.genRand.Next(1, Main.maxTilesX);
						num206 = WorldGen.genRand.Next(1, Main.maxTilesY - 200);
					}
					flag13 = WorldGen.AddBuriedChest(num205, num206, contain2);
				}
				num204++;
			}
			for (int num207 = 0; num207 < WorldGen.numIslandHouses; num207++)
			{
				WorldGen.IslandHouse(WorldGen.fihX[num207], WorldGen.fihY[num207]);
			}
            Console.WriteLine();
            for (int num208 = 0; num208 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); num208++)
			{
				float num209 = (float)((double)num208 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008));
				Program.printData("Placing breakables: " + (int)(num209 * 100f + 1f) + "%");
				bool flag14 = false;
				int num210 = 0;
				while (!flag14)
				{
					int num211 = WorldGen.genRand.Next((int)num6, Main.maxTilesY - 10);
					if ((double)num209 > 0.93)
					{
						num211 = Main.maxTilesY - 150;
					}
					else
					{
						if ((double)num209 > 0.75)
						{
							num211 = (int)num5;
						}
					}
					int num212 = WorldGen.genRand.Next(1, Main.maxTilesX);
					bool flag15 = false;
					for (int num213 = num211; num213 < Main.maxTilesY; num213++)
					{
						if (!flag15)
						{
							if (Main.tile[num212, num213].active && Main.tileSolid[(int)Main.tile[num212, num213].type] && !Main.tile[num212, num213 - 1].lava)
							{
								flag15 = true;
							}
						}
						else
						{
							if (WorldGen.PlacePot(num212, num213, 28))
							{
								flag14 = true;
								break;
							}
							num210++;
							if (num210 >= 10000)
							{
								flag14 = true;
								break;
							}
						}
					}
				}
			}
            Console.WriteLine();
            for (int num214 = 0; num214 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 1E-05); num214++)
			{
				float num215 = (float)((double)num214 / ((double)(Main.maxTilesX * Main.maxTilesY) * 1E-05));
				Program.printData("Placing hellforges: " + (int)(num215 * 100f + 1f) + "%");
				bool flag16 = false;
				int num216 = 0;
				while (!flag16)
				{
					int num217 = WorldGen.genRand.Next(1, Main.maxTilesX);
					int num218 = WorldGen.genRand.Next(Main.maxTilesY - 250, Main.maxTilesY - 5);
					try
					{
						if (Main.tile[num217, num218].wall == 13)
						{
							while (!Main.tile[num217, num218].active)
							{
								num218++;
							}
							num218--;
							WorldGen.PlaceTile(num217, num218, 77, false, false, -1);
							if (Main.tile[num217, num218].type == 77)
							{
								flag16 = true;
							}
							else
							{
								num216++;
								if (num216 >= 10000)
								{
									flag16 = true;
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
			for (int num219 = 0; num219 < Main.maxTilesX; num219++)
			{
				int num54 = num219;
				bool flag17 = true;
				int num220 = 0;
				while ((double)num220 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num54, num220].active)
					{
						if (flag17 && Main.tile[num54, num220].type == 0)
						{
							WorldGen.SpreadGrass(num54, num220, 0, 2, true);
						}
						if ((double)num220 > num6)
						{
							break;
						}
						flag17 = false;
					}
					else
					{
						if (Main.tile[num54, num220].wall == 0)
						{
							flag17 = true;
						}
					}
					num220++;
				}
			}
			int num221 = 5;
			bool flag18 = true;
			while (flag18)
			{
				int num222 = Main.maxTilesX / 2 + WorldGen.genRand.Next(-num221, num221 + 1);
				for (int num223 = 0; num223 < Main.maxTilesY; num223++)
				{
					if (Main.tile[num222, num223].active)
					{
						Main.spawnTileX = num222;
						Main.spawnTileY = num223;
						Main.tile[num222, num223 - 1].lighted = true;
						break;
					}
				}
				flag18 = false;
				num221++;
				if ((double)Main.spawnTileY > Main.worldSurface)
				{
					flag18 = true;
				}
				if (Main.tile[Main.spawnTileX, Main.spawnTileY - 1].liquid > 0)
				{
					flag18 = true;
				}
			}
			int num224 = 10;
			while ((double)Main.spawnTileY > Main.worldSurface)
			{
				int num225 = WorldGen.genRand.Next(Main.maxTilesX / 2 - num224, Main.maxTilesX / 2 + num224);
				for (int num226 = 0; num226 < Main.maxTilesY; num226++)
				{
					if (Main.tile[num225, num226].active)
					{
						Main.spawnTileX = num225;
						Main.spawnTileY = num226;
						Main.tile[num225, num226 - 1].lighted = true;
						break;
					}
				}
				num224++;
			}
			int num227 = NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, 22, 0);
			Main.npc[num227].homeTileX = Main.spawnTileX;
			Main.npc[num227].homeTileY = Main.spawnTileY;
			Main.npc[num227].direction = 1;
			Main.npc[num227].homeless = true;
            Console.WriteLine();
            Program.printData("Planting sunflowers...");
			int num228 = 0;
			while ((double)num228 < (double)Main.maxTilesX * 0.002)
			{
				int num229 = 0;
				int num230 = 0;
				int arg_4AF8_0 = Main.maxTilesX / 2;
				int num231 = WorldGen.genRand.Next(Main.maxTilesX);
				num229 = num231 - WorldGen.genRand.Next(10) - 7;
				num230 = num231 + WorldGen.genRand.Next(10) + 7;
				if (num229 < 0)
				{
					num229 = 0;
				}
				if (num230 > Main.maxTilesX - 1)
				{
					num230 = Main.maxTilesX - 1;
				}
				for (int num232 = num229; num232 < num230; num232++)
				{
					int num233 = 1;
					while ((double)num233 < Main.worldSurface - 1.0)
					{
						if (Main.tile[num232, num233].type == 1 && Main.tile[num232, num233].active)
						{
							Main.tile[num232, num233].type = 2;
						}
						if (Main.tile[num232 + 1, num233].type == 1 && Main.tile[num232 + 1, num233].active)
						{
							Main.tile[num232 + 1, num233].type = 2;
						}
						if (Main.tile[num232, num233].type == 2 && Main.tile[num232, num233].active && !Main.tile[num232, num233 - 1].active)
						{
							WorldGen.PlaceTile(num232, num233 - 1, 27, true, false, -1);
						}
						if (Main.tile[num232, num233].active)
						{
							break;
						}
						num233++;
					}
				}
				num228++;
			}
            Console.WriteLine();
            Program.printData("Planting trees...");
			int num234 = 0;
			while ((double)num234 < (double)Main.maxTilesX * 0.003)
			{
				int num235 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
				int num236 = WorldGen.genRand.Next(25, 50);
				for (int num237 = num235 - num236; num237 < num235 + num236; num237++)
				{
					int num238 = 20;
					while ((double)num238 < Main.worldSurface)
					{
						if (Main.tile[num237, num238].active)
						{
							if (Main.tile[num237, num238].type == 1)
							{
								Main.tile[num237, num238].type = 2;
							}
							if (Main.tile[num237, num238 + 1].type == 1)
							{
								Main.tile[num237, num238 + 1].type = 2;
								break;
							}
							break;
						}
						else
						{
							num238++;
						}
					}
				}
				for (int num239 = num235 - num236; num239 < num235 + num236; num239++)
				{
					int num240 = 20;
					while ((double)num240 < Main.worldSurface)
					{
						WorldGen.GrowEpicTree(num239, num240);
						num240++;
					}
				}
				num234++;
			}
			WorldGen.AddTrees();
            Console.WriteLine();
            Program.printData("Planting weeds...");
			WorldGen.AddPlants();
			for (int num241 = 0; num241 < Main.maxTilesX; num241++)
			{
				for (int num242 = (int)Main.worldSurface; num242 < Main.maxTilesY; num242++)
				{
					if (Main.tile[num241, num242].active)
					{
						if (Main.tile[num241, num242].type == 70 && !Main.tile[num241, num242 - 1].active)
						{
							WorldGen.GrowShroom(num241, num242);
							if (!Main.tile[num241, num242 - 1].active)
							{
								WorldGen.PlaceTile(num241, num242 - 1, 71, true, false, -1);
							}
						}
						if (Main.tile[num241, num242].type == 60 && !Main.tile[num241, num242 - 1].active)
						{
							WorldGen.PlaceTile(num241, num242 - 1, 61, true, false, -1);
						}
					}
				}
			}
            Console.WriteLine();
            Program.printData("Growing vines...");
			for (int num243 = 0; num243 < Main.maxTilesX; num243++)
			{
				int num244 = 0;
				int num245 = 0;
				while ((double)num245 < Main.worldSurface)
				{
					if (num244 > 0 && !Main.tile[num243, num245].active)
					{
						Main.tile[num243, num245].active = true;
						Main.tile[num243, num245].type = 52;
						num244--;
					}
					else
					{
						num244 = 0;
					}
					if (Main.tile[num243, num245].active && Main.tile[num243, num245].type == 2 && WorldGen.genRand.Next(5) < 3)
					{
						num244 = WorldGen.genRand.Next(1, 10);
					}
					num245++;
				}
				num244 = 0;
				for (int num246 = (int)Main.worldSurface; num246 < Main.maxTilesY; num246++)
				{
					if (num244 > 0 && !Main.tile[num243, num246].active)
					{
						Main.tile[num243, num246].active = true;
						Main.tile[num243, num246].type = 62;
						num244--;
					}
					else
					{
						num244 = 0;
					}
					if (Main.tile[num243, num246].active && Main.tile[num243, num246].type == 60 && WorldGen.genRand.Next(5) < 3)
					{
						num244 = WorldGen.genRand.Next(1, 10);
					}
				}
			}
            Console.WriteLine();
            Program.printData("Planting flowers...");
			int num247 = 0;
			while ((double)num247 < (double)Main.maxTilesX * 0.005)
			{
				int num248 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
				int num249 = WorldGen.genRand.Next(5, 15);
				int num250 = WorldGen.genRand.Next(15, 30);
				int num251 = 1;
				while ((double)num251 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num248, num251].active)
					{
						for (int num252 = num248 - num249; num252 < num248 + num249; num252++)
						{
							for (int num253 = num251 - num250; num253 < num251 + num250; num253++)
							{
								if (Main.tile[num252, num253].type == 3 || Main.tile[num252, num253].type == 24)
								{
									Main.tile[num252, num253].frameX = (short)(WorldGen.genRand.Next(6, 8) * 18);
								}
							}
						}
						break;
					}
					num251++;
				}
				num247++;
			}
            Console.WriteLine();
            Program.printData("Planting mushrooms...");
			int num254 = 0;
			while ((double)num254 < (double)Main.maxTilesX * 0.002)
			{
				int num255 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
				int num256 = WorldGen.genRand.Next(4, 10);
				int num257 = WorldGen.genRand.Next(15, 30);
				int num258 = 1;
				while ((double)num258 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num255, num258].active)
					{
						for (int num259 = num255 - num256; num259 < num255 + num256; num259++)
						{
							for (int num260 = num258 - num257; num260 < num258 + num257; num260++)
							{
								if (Main.tile[num259, num260].type == 3 || Main.tile[num259, num260].type == 24)
								{
									Main.tile[num259, num260].frameX = 144;
								}
							}
						}
						break;
					}
					num258++;
				}
				num254++;
			}
			WorldGen.gen = false;
		}
		public static void GrowEpicTree(int i, int y)
		{
			int num = y;
			while (Main.tile[i, num].type == 20)
			{
				num++;
			}
			if (Main.tile[i, num].active && Main.tile[i, num].type == 2 && Main.tile[i, num - 1].wall == 0 && Main.tile[i, num - 1].liquid == 0 && Main.tile[i - 1, num].active && Main.tile[i - 1, num].type == 2 && Main.tile[i + 1, num].active && Main.tile[i + 1, num].type == 2 && WorldGen.EmptyTileCheck(i - 2, i + 2, num - 55, num - 1, 20))
			{
				bool flag = false;
				bool flag2 = false;
				int num2 = WorldGen.genRand.Next(20, 30);
				int num3;
				for (int j = num - num2; j < num; j++)
				{
					Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
					Main.tile[i, j].active = true;
					Main.tile[i, j].type = 5;
					num3 = WorldGen.genRand.Next(3);
					int num4 = WorldGen.genRand.Next(10);
					if (j == num - 1 || j == num - num2)
					{
						num4 = 0;
					}
					while (((num4 == 5 || num4 == 7) && flag) || ((num4 == 6 || num4 == 7) && flag2))
					{
						num4 = WorldGen.genRand.Next(10);
					}
					flag = false;
					flag2 = false;
					if (num4 == 5 || num4 == 7)
					{
						flag = true;
					}
					if (num4 == 6 || num4 == 7)
					{
						flag2 = true;
					}
					if (num4 == 1)
					{
						if (num3 == 0)
						{
							Main.tile[i, j].frameX = 0;
							Main.tile[i, j].frameY = 66;
						}
						if (num3 == 1)
						{
							Main.tile[i, j].frameX = 0;
							Main.tile[i, j].frameY = 88;
						}
						if (num3 == 2)
						{
							Main.tile[i, j].frameX = 0;
							Main.tile[i, j].frameY = 110;
						}
					}
					else
					{
						if (num4 == 2)
						{
							if (num3 == 0)
							{
								Main.tile[i, j].frameX = 22;
								Main.tile[i, j].frameY = 0;
							}
							if (num3 == 1)
							{
								Main.tile[i, j].frameX = 22;
								Main.tile[i, j].frameY = 22;
							}
							if (num3 == 2)
							{
								Main.tile[i, j].frameX = 22;
								Main.tile[i, j].frameY = 44;
							}
						}
						else
						{
							if (num4 == 3)
							{
								if (num3 == 0)
								{
									Main.tile[i, j].frameX = 44;
									Main.tile[i, j].frameY = 66;
								}
								if (num3 == 1)
								{
									Main.tile[i, j].frameX = 44;
									Main.tile[i, j].frameY = 88;
								}
								if (num3 == 2)
								{
									Main.tile[i, j].frameX = 44;
									Main.tile[i, j].frameY = 110;
								}
							}
							else
							{
								if (num4 == 4)
								{
									if (num3 == 0)
									{
										Main.tile[i, j].frameX = 22;
										Main.tile[i, j].frameY = 66;
									}
									if (num3 == 1)
									{
										Main.tile[i, j].frameX = 22;
										Main.tile[i, j].frameY = 88;
									}
									if (num3 == 2)
									{
										Main.tile[i, j].frameX = 22;
										Main.tile[i, j].frameY = 110;
									}
								}
								else
								{
									if (num4 == 5)
									{
										if (num3 == 0)
										{
											Main.tile[i, j].frameX = 88;
											Main.tile[i, j].frameY = 0;
										}
										if (num3 == 1)
										{
											Main.tile[i, j].frameX = 88;
											Main.tile[i, j].frameY = 22;
										}
										if (num3 == 2)
										{
											Main.tile[i, j].frameX = 88;
											Main.tile[i, j].frameY = 44;
										}
									}
									else
									{
										if (num4 == 6)
										{
											if (num3 == 0)
											{
												Main.tile[i, j].frameX = 66;
												Main.tile[i, j].frameY = 66;
											}
											if (num3 == 1)
											{
												Main.tile[i, j].frameX = 66;
												Main.tile[i, j].frameY = 88;
											}
											if (num3 == 2)
											{
												Main.tile[i, j].frameX = 66;
												Main.tile[i, j].frameY = 110;
											}
										}
										else
										{
											if (num4 == 7)
											{
												if (num3 == 0)
												{
													Main.tile[i, j].frameX = 110;
													Main.tile[i, j].frameY = 66;
												}
												if (num3 == 1)
												{
													Main.tile[i, j].frameX = 110;
													Main.tile[i, j].frameY = 88;
												}
												if (num3 == 2)
												{
													Main.tile[i, j].frameX = 110;
													Main.tile[i, j].frameY = 110;
												}
											}
											else
											{
												if (num3 == 0)
												{
													Main.tile[i, j].frameX = 0;
													Main.tile[i, j].frameY = 0;
												}
												if (num3 == 1)
												{
													Main.tile[i, j].frameX = 0;
													Main.tile[i, j].frameY = 22;
												}
												if (num3 == 2)
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
					if (num4 == 5 || num4 == 7)
					{
						Main.tile[i - 1, j].active = true;
						Main.tile[i - 1, j].type = 5;
						num3 = WorldGen.genRand.Next(3);
						if (WorldGen.genRand.Next(3) < 2)
						{
							if (num3 == 0)
							{
								Main.tile[i - 1, j].frameX = 44;
								Main.tile[i - 1, j].frameY = 198;
							}
							if (num3 == 1)
							{
								Main.tile[i - 1, j].frameX = 44;
								Main.tile[i - 1, j].frameY = 220;
							}
							if (num3 == 2)
							{
								Main.tile[i - 1, j].frameX = 44;
								Main.tile[i - 1, j].frameY = 242;
							}
						}
						else
						{
							if (num3 == 0)
							{
								Main.tile[i - 1, j].frameX = 66;
								Main.tile[i - 1, j].frameY = 0;
							}
							if (num3 == 1)
							{
								Main.tile[i - 1, j].frameX = 66;
								Main.tile[i - 1, j].frameY = 22;
							}
							if (num3 == 2)
							{
								Main.tile[i - 1, j].frameX = 66;
								Main.tile[i - 1, j].frameY = 44;
							}
						}
					}
					if (num4 == 6 || num4 == 7)
					{
						Main.tile[i + 1, j].active = true;
						Main.tile[i + 1, j].type = 5;
						num3 = WorldGen.genRand.Next(3);
						if (WorldGen.genRand.Next(3) < 2)
						{
							if (num3 == 0)
							{
								Main.tile[i + 1, j].frameX = 66;
								Main.tile[i + 1, j].frameY = 198;
							}
							if (num3 == 1)
							{
								Main.tile[i + 1, j].frameX = 66;
								Main.tile[i + 1, j].frameY = 220;
							}
							if (num3 == 2)
							{
								Main.tile[i + 1, j].frameX = 66;
								Main.tile[i + 1, j].frameY = 242;
							}
						}
						else
						{
							if (num3 == 0)
							{
								Main.tile[i + 1, j].frameX = 88;
								Main.tile[i + 1, j].frameY = 66;
							}
							if (num3 == 1)
							{
								Main.tile[i + 1, j].frameX = 88;
								Main.tile[i + 1, j].frameY = 88;
							}
							if (num3 == 2)
							{
								Main.tile[i + 1, j].frameX = 88;
								Main.tile[i + 1, j].frameY = 110;
							}
						}
					}
				}
				int num5 = WorldGen.genRand.Next(3);
				if (num5 == 0 || num5 == 1)
				{
					Main.tile[i + 1, num - 1].active = true;
					Main.tile[i + 1, num - 1].type = 5;
					num3 = WorldGen.genRand.Next(3);
					if (num3 == 0)
					{
						Main.tile[i + 1, num - 1].frameX = 22;
						Main.tile[i + 1, num - 1].frameY = 132;
					}
					if (num3 == 1)
					{
						Main.tile[i + 1, num - 1].frameX = 22;
						Main.tile[i + 1, num - 1].frameY = 154;
					}
					if (num3 == 2)
					{
						Main.tile[i + 1, num - 1].frameX = 22;
						Main.tile[i + 1, num - 1].frameY = 176;
					}
				}
				if (num5 == 0 || num5 == 2)
				{
					Main.tile[i - 1, num - 1].active = true;
					Main.tile[i - 1, num - 1].type = 5;
					num3 = WorldGen.genRand.Next(3);
					if (num3 == 0)
					{
						Main.tile[i - 1, num - 1].frameX = 44;
						Main.tile[i - 1, num - 1].frameY = 132;
					}
					if (num3 == 1)
					{
						Main.tile[i - 1, num - 1].frameX = 44;
						Main.tile[i - 1, num - 1].frameY = 154;
					}
					if (num3 == 2)
					{
						Main.tile[i - 1, num - 1].frameX = 44;
						Main.tile[i - 1, num - 1].frameY = 176;
					}
				}
				num3 = WorldGen.genRand.Next(3);
				if (num5 == 0)
				{
					if (num3 == 0)
					{
						Main.tile[i, num - 1].frameX = 88;
						Main.tile[i, num - 1].frameY = 132;
					}
					if (num3 == 1)
					{
						Main.tile[i, num - 1].frameX = 88;
						Main.tile[i, num - 1].frameY = 154;
					}
					if (num3 == 2)
					{
						Main.tile[i, num - 1].frameX = 88;
						Main.tile[i, num - 1].frameY = 176;
					}
				}
				else
				{
					if (num5 == 1)
					{
						if (num3 == 0)
						{
							Main.tile[i, num - 1].frameX = 0;
							Main.tile[i, num - 1].frameY = 132;
						}
						if (num3 == 1)
						{
							Main.tile[i, num - 1].frameX = 0;
							Main.tile[i, num - 1].frameY = 154;
						}
						if (num3 == 2)
						{
							Main.tile[i, num - 1].frameX = 0;
							Main.tile[i, num - 1].frameY = 176;
						}
					}
					else
					{
						if (num5 == 2)
						{
							if (num3 == 0)
							{
								Main.tile[i, num - 1].frameX = 66;
								Main.tile[i, num - 1].frameY = 132;
							}
							if (num3 == 1)
							{
								Main.tile[i, num - 1].frameX = 66;
								Main.tile[i, num - 1].frameY = 154;
							}
							if (num3 == 2)
							{
								Main.tile[i, num - 1].frameX = 66;
								Main.tile[i, num - 1].frameY = 176;
							}
						}
					}
				}
				if (WorldGen.genRand.Next(3) < 2)
				{
					num3 = WorldGen.genRand.Next(3);
					if (num3 == 0)
					{
						Main.tile[i, num - num2].frameX = 22;
						Main.tile[i, num - num2].frameY = 198;
					}
					if (num3 == 1)
					{
						Main.tile[i, num - num2].frameX = 22;
						Main.tile[i, num - num2].frameY = 220;
					}
					if (num3 == 2)
					{
						Main.tile[i, num - num2].frameX = 22;
						Main.tile[i, num - num2].frameY = 242;
					}
				}
				else
				{
					num3 = WorldGen.genRand.Next(3);
					if (num3 == 0)
					{
						Main.tile[i, num - num2].frameX = 0;
						Main.tile[i, num - num2].frameY = 198;
					}
					if (num3 == 1)
					{
						Main.tile[i, num - num2].frameX = 0;
						Main.tile[i, num - num2].frameY = 220;
					}
					if (num3 == 2)
					{
						Main.tile[i, num - num2].frameX = 0;
						Main.tile[i, num - num2].frameY = 242;
					}
				}
				WorldGen.RangeFrame(i - 2, num - num2 - 1, i + 2, num + 1);
				if (Main.netMode == 2)
				{
					NetMessage.SendTileSquare(-1, i, (int)((double)num - (double)num2 * 0.5), num2 + 1);
				}
			}
		}
		public static void GrowTree(int i, int y)
		{
			int num = y;
			while (Main.tile[i, num].type == 20)
			{
				num++;
			}
			if (Main.tile[i - 1, num - 1].liquid != 0 || Main.tile[i - 1, num - 1].liquid != 0 || Main.tile[i + 1, num - 1].liquid != 0)
			{
				return;
			}
			if (Main.tile[i, num].active && (Main.tile[i, num].type == 2 || Main.tile[i, num].type == 23) && Main.tile[i, num - 1].wall == 0 && Main.tile[i - 1, num].active && (Main.tile[i - 1, num].type == 2 || Main.tile[i - 1, num].type == 23) && Main.tile[i + 1, num].active && (Main.tile[i + 1, num].type == 2 || Main.tile[i + 1, num].type == 23) && WorldGen.EmptyTileCheck(i - 2, i + 2, num - 14, num - 1, 20))
			{
				bool flag = false;
				bool flag2 = false;
				int num2 = WorldGen.genRand.Next(5, 15);
				int num3;
				for (int j = num - num2; j < num; j++)
				{
					Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
					Main.tile[i, j].active = true;
					Main.tile[i, j].type = 5;
					num3 = WorldGen.genRand.Next(3);
					int num4 = WorldGen.genRand.Next(10);
					if (j == num - 1 || j == num - num2)
					{
						num4 = 0;
					}
					while (((num4 == 5 || num4 == 7) && flag) || ((num4 == 6 || num4 == 7) && flag2))
					{
						num4 = WorldGen.genRand.Next(10);
					}
					flag = false;
					flag2 = false;
					if (num4 == 5 || num4 == 7)
					{
						flag = true;
					}
					if (num4 == 6 || num4 == 7)
					{
						flag2 = true;
					}
					if (num4 == 1)
					{
						if (num3 == 0)
						{
							Main.tile[i, j].frameX = 0;
							Main.tile[i, j].frameY = 66;
						}
						if (num3 == 1)
						{
							Main.tile[i, j].frameX = 0;
							Main.tile[i, j].frameY = 88;
						}
						if (num3 == 2)
						{
							Main.tile[i, j].frameX = 0;
							Main.tile[i, j].frameY = 110;
						}
					}
					else
					{
						if (num4 == 2)
						{
							if (num3 == 0)
							{
								Main.tile[i, j].frameX = 22;
								Main.tile[i, j].frameY = 0;
							}
							if (num3 == 1)
							{
								Main.tile[i, j].frameX = 22;
								Main.tile[i, j].frameY = 22;
							}
							if (num3 == 2)
							{
								Main.tile[i, j].frameX = 22;
								Main.tile[i, j].frameY = 44;
							}
						}
						else
						{
							if (num4 == 3)
							{
								if (num3 == 0)
								{
									Main.tile[i, j].frameX = 44;
									Main.tile[i, j].frameY = 66;
								}
								if (num3 == 1)
								{
									Main.tile[i, j].frameX = 44;
									Main.tile[i, j].frameY = 88;
								}
								if (num3 == 2)
								{
									Main.tile[i, j].frameX = 44;
									Main.tile[i, j].frameY = 110;
								}
							}
							else
							{
								if (num4 == 4)
								{
									if (num3 == 0)
									{
										Main.tile[i, j].frameX = 22;
										Main.tile[i, j].frameY = 66;
									}
									if (num3 == 1)
									{
										Main.tile[i, j].frameX = 22;
										Main.tile[i, j].frameY = 88;
									}
									if (num3 == 2)
									{
										Main.tile[i, j].frameX = 22;
										Main.tile[i, j].frameY = 110;
									}
								}
								else
								{
									if (num4 == 5)
									{
										if (num3 == 0)
										{
											Main.tile[i, j].frameX = 88;
											Main.tile[i, j].frameY = 0;
										}
										if (num3 == 1)
										{
											Main.tile[i, j].frameX = 88;
											Main.tile[i, j].frameY = 22;
										}
										if (num3 == 2)
										{
											Main.tile[i, j].frameX = 88;
											Main.tile[i, j].frameY = 44;
										}
									}
									else
									{
										if (num4 == 6)
										{
											if (num3 == 0)
											{
												Main.tile[i, j].frameX = 66;
												Main.tile[i, j].frameY = 66;
											}
											if (num3 == 1)
											{
												Main.tile[i, j].frameX = 66;
												Main.tile[i, j].frameY = 88;
											}
											if (num3 == 2)
											{
												Main.tile[i, j].frameX = 66;
												Main.tile[i, j].frameY = 110;
											}
										}
										else
										{
											if (num4 == 7)
											{
												if (num3 == 0)
												{
													Main.tile[i, j].frameX = 110;
													Main.tile[i, j].frameY = 66;
												}
												if (num3 == 1)
												{
													Main.tile[i, j].frameX = 110;
													Main.tile[i, j].frameY = 88;
												}
												if (num3 == 2)
												{
													Main.tile[i, j].frameX = 110;
													Main.tile[i, j].frameY = 110;
												}
											}
											else
											{
												if (num3 == 0)
												{
													Main.tile[i, j].frameX = 0;
													Main.tile[i, j].frameY = 0;
												}
												if (num3 == 1)
												{
													Main.tile[i, j].frameX = 0;
													Main.tile[i, j].frameY = 22;
												}
												if (num3 == 2)
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
					if (num4 == 5 || num4 == 7)
					{
						Main.tile[i - 1, j].active = true;
						Main.tile[i - 1, j].type = 5;
						num3 = WorldGen.genRand.Next(3);
						if (WorldGen.genRand.Next(3) < 2)
						{
							if (num3 == 0)
							{
								Main.tile[i - 1, j].frameX = 44;
								Main.tile[i - 1, j].frameY = 198;
							}
							if (num3 == 1)
							{
								Main.tile[i - 1, j].frameX = 44;
								Main.tile[i - 1, j].frameY = 220;
							}
							if (num3 == 2)
							{
								Main.tile[i - 1, j].frameX = 44;
								Main.tile[i - 1, j].frameY = 242;
							}
						}
						else
						{
							if (num3 == 0)
							{
								Main.tile[i - 1, j].frameX = 66;
								Main.tile[i - 1, j].frameY = 0;
							}
							if (num3 == 1)
							{
								Main.tile[i - 1, j].frameX = 66;
								Main.tile[i - 1, j].frameY = 22;
							}
							if (num3 == 2)
							{
								Main.tile[i - 1, j].frameX = 66;
								Main.tile[i - 1, j].frameY = 44;
							}
						}
					}
					if (num4 == 6 || num4 == 7)
					{
						Main.tile[i + 1, j].active = true;
						Main.tile[i + 1, j].type = 5;
						num3 = WorldGen.genRand.Next(3);
						if (WorldGen.genRand.Next(3) < 2)
						{
							if (num3 == 0)
							{
								Main.tile[i + 1, j].frameX = 66;
								Main.tile[i + 1, j].frameY = 198;
							}
							if (num3 == 1)
							{
								Main.tile[i + 1, j].frameX = 66;
								Main.tile[i + 1, j].frameY = 220;
							}
							if (num3 == 2)
							{
								Main.tile[i + 1, j].frameX = 66;
								Main.tile[i + 1, j].frameY = 242;
							}
						}
						else
						{
							if (num3 == 0)
							{
								Main.tile[i + 1, j].frameX = 88;
								Main.tile[i + 1, j].frameY = 66;
							}
							if (num3 == 1)
							{
								Main.tile[i + 1, j].frameX = 88;
								Main.tile[i + 1, j].frameY = 88;
							}
							if (num3 == 2)
							{
								Main.tile[i + 1, j].frameX = 88;
								Main.tile[i + 1, j].frameY = 110;
							}
						}
					}
				}
				int num5 = WorldGen.genRand.Next(3);
				if (num5 == 0 || num5 == 1)
				{
					Main.tile[i + 1, num - 1].active = true;
					Main.tile[i + 1, num - 1].type = 5;
					num3 = WorldGen.genRand.Next(3);
					if (num3 == 0)
					{
						Main.tile[i + 1, num - 1].frameX = 22;
						Main.tile[i + 1, num - 1].frameY = 132;
					}
					if (num3 == 1)
					{
						Main.tile[i + 1, num - 1].frameX = 22;
						Main.tile[i + 1, num - 1].frameY = 154;
					}
					if (num3 == 2)
					{
						Main.tile[i + 1, num - 1].frameX = 22;
						Main.tile[i + 1, num - 1].frameY = 176;
					}
				}
				if (num5 == 0 || num5 == 2)
				{
					Main.tile[i - 1, num - 1].active = true;
					Main.tile[i - 1, num - 1].type = 5;
					num3 = WorldGen.genRand.Next(3);
					if (num3 == 0)
					{
						Main.tile[i - 1, num - 1].frameX = 44;
						Main.tile[i - 1, num - 1].frameY = 132;
					}
					if (num3 == 1)
					{
						Main.tile[i - 1, num - 1].frameX = 44;
						Main.tile[i - 1, num - 1].frameY = 154;
					}
					if (num3 == 2)
					{
						Main.tile[i - 1, num - 1].frameX = 44;
						Main.tile[i - 1, num - 1].frameY = 176;
					}
				}
				num3 = WorldGen.genRand.Next(3);
				if (num5 == 0)
				{
					if (num3 == 0)
					{
						Main.tile[i, num - 1].frameX = 88;
						Main.tile[i, num - 1].frameY = 132;
					}
					if (num3 == 1)
					{
						Main.tile[i, num - 1].frameX = 88;
						Main.tile[i, num - 1].frameY = 154;
					}
					if (num3 == 2)
					{
						Main.tile[i, num - 1].frameX = 88;
						Main.tile[i, num - 1].frameY = 176;
					}
				}
				else
				{
					if (num5 == 1)
					{
						if (num3 == 0)
						{
							Main.tile[i, num - 1].frameX = 0;
							Main.tile[i, num - 1].frameY = 132;
						}
						if (num3 == 1)
						{
							Main.tile[i, num - 1].frameX = 0;
							Main.tile[i, num - 1].frameY = 154;
						}
						if (num3 == 2)
						{
							Main.tile[i, num - 1].frameX = 0;
							Main.tile[i, num - 1].frameY = 176;
						}
					}
					else
					{
						if (num5 == 2)
						{
							if (num3 == 0)
							{
								Main.tile[i, num - 1].frameX = 66;
								Main.tile[i, num - 1].frameY = 132;
							}
							if (num3 == 1)
							{
								Main.tile[i, num - 1].frameX = 66;
								Main.tile[i, num - 1].frameY = 154;
							}
							if (num3 == 2)
							{
								Main.tile[i, num - 1].frameX = 66;
								Main.tile[i, num - 1].frameY = 176;
							}
						}
					}
				}
				if (WorldGen.genRand.Next(3) < 2)
				{
					num3 = WorldGen.genRand.Next(3);
					if (num3 == 0)
					{
						Main.tile[i, num - num2].frameX = 22;
						Main.tile[i, num - num2].frameY = 198;
					}
					if (num3 == 1)
					{
						Main.tile[i, num - num2].frameX = 22;
						Main.tile[i, num - num2].frameY = 220;
					}
					if (num3 == 2)
					{
						Main.tile[i, num - num2].frameX = 22;
						Main.tile[i, num - num2].frameY = 242;
					}
				}
				else
				{
					num3 = WorldGen.genRand.Next(3);
					if (num3 == 0)
					{
						Main.tile[i, num - num2].frameX = 0;
						Main.tile[i, num - num2].frameY = 198;
					}
					if (num3 == 1)
					{
						Main.tile[i, num - num2].frameX = 0;
						Main.tile[i, num - num2].frameY = 220;
					}
					if (num3 == 2)
					{
						Main.tile[i, num - num2].frameX = 0;
						Main.tile[i, num - num2].frameY = 242;
					}
				}
				WorldGen.RangeFrame(i - 2, num - num2 - 1, i + 2, num + 1);
				if (Main.netMode == 2)
				{
					NetMessage.SendTileSquare(-1, i, (int)((double)num - (double)num2 * 0.5), num2 + 1);
				}
			}
		}
		public static void GrowShroom(int i, int y)
		{
			if (Main.tile[i - 1, y - 1].lava || Main.tile[i - 1, y - 1].lava || Main.tile[i + 1, y - 1].lava)
			{
				return;
			}
			if (Main.tile[i, y].active && Main.tile[i, y].type == 70 && Main.tile[i, y - 1].wall == 0 && Main.tile[i - 1, y].active && Main.tile[i - 1, y].type == 70 && Main.tile[i + 1, y].active && Main.tile[i + 1, y].type == 70 && WorldGen.EmptyTileCheck(i - 2, i + 2, y - 13, y - 1, 71))
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
					if (Main.tile[i, j].active)
					{
						if (ignoreStyle == -1)
						{
							return false;
						}
						if (ignoreStyle == 11 && Main.tile[i, j].type != 11)
						{
							return false;
						}
						if (ignoreStyle == 20 && Main.tile[i, j].type != 20 && Main.tile[i, j].type != 3)
						{
							return false;
						}
						if (ignoreStyle == 71 && Main.tile[i, j].type != 71)
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
						if (Main.tile[l, m] == null)
						{
							Main.tile[l, m] = new Tile();
						}
						Main.tile[l, m].type = 10;
						Main.tile[l, m].frameX = (short)(WorldGen.genRand.Next(3) * 18);
					}
					else
					{
						if (Main.tile[l, m] == null)
						{
							Main.tile[l, m] = new Tile();
						}
						Main.tile[l, m].active = false;
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
			//Main.PlaySound(9, i * 16, j * 16, 1);
			return true;
		}
		public static bool AddLifeCrystal(int i, int j)
		{
			int k = j;
			while (k < Main.maxTilesY)
			{
				if (Main.tile[i, k].active && Main.tileSolid[(int)Main.tile[i, k].type])
				{
					int num = k - 1;
					if (Main.tile[i, num - 1].lava || Main.tile[i - 1, num - 1].lava)
					{
						return false;
					}
					if (!WorldGen.EmptyTileCheck(i - 1, i, num - 1, num, -1))
					{
						return false;
					}
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
			for (int l = 0; l < num3; l++)
			{
				int num6 = WorldGen.genRand.Next(5, 9);
				num4 += num6;
				WorldGen.HellRoom(i, num4, num, num6);
			}
			for (int m = i - num / 2; m <= i + num / 2; m++)
			{
				num4 = j;
				while (num4 < Main.maxTilesY && ((Main.tile[m, num4].active && Main.tile[m, num4].type == 76) || Main.tile[m, num4].wall == 13))
				{
					num4++;
				}
				int num7 = 6 + WorldGen.genRand.Next(3);
				while (num4 < Main.maxTilesY && !Main.tile[m, num4].active)
				{
					num7--;
					Main.tile[m, num4].active = true;
					Main.tile[m, num4].type = 57;
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
					for (int n = num9; n <= num10; n++)
					{
						if (Main.tile[n, num4 - 1].wall == 13)
						{
							Main.tile[n, num4].wall = 13;
						}
						Main.tile[n, num4].type = 19;
						Main.tile[n, num4].active = true;
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
				for (int num19 = num16 - num18; num19 <= num16 + num18; num19++)
				{
					for (int num20 = num17 - num18; num20 <= num17 + num18; num20++)
					{
						float num21 = (float)Math.Abs(num19 - num16);
						float num22 = (float)Math.Abs(num20 - num17);
						double num23 = Math.Sqrt((double)(num21 * num21 + num22 * num22));
						if (num23 < (double)num18 * 0.4)
						{
							if (Main.tile[num19, num20].type == 76 || Main.tile[num19, num20].type == 19)
							{
								Main.tile[num19, num20].active = false;
							}
							Main.tile[num19, num20].wall = 0;
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
						Main.tile[k, l].active = true;
						Main.tile[k, l].type = 76;
						Main.tile[k, l].liquid = 0;
						Main.tile[k, l].lava = false;
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
						Main.tile[m, n].active = false;
						Main.tile[m, n].wall = 13;
						Main.tile[m, n].liquid = 0;
						Main.tile[m, n].lava = false;
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
			Program.printData("Creating dungeon: 65%");
			for (int j = 0; j < WorldGen.numDRooms; j++)
			{
				for (int k = WorldGen.dRoomL[j]; k <= WorldGen.dRoomR[j]; k++)
				{
					if (!Main.tile[k, WorldGen.dRoomT[j] - 1].active)
					{
						WorldGen.DPlatX[WorldGen.numDPlats] = k;
						WorldGen.DPlatY[WorldGen.numDPlats] = WorldGen.dRoomT[j] - 1;
						WorldGen.numDPlats++;
						break;
					}
				}
				for (int l = WorldGen.dRoomL[j]; l <= WorldGen.dRoomR[j]; l++)
				{
					if (!Main.tile[l, WorldGen.dRoomB[j] + 1].active)
					{
						WorldGen.DPlatX[WorldGen.numDPlats] = l;
						WorldGen.DPlatY[WorldGen.numDPlats] = WorldGen.dRoomB[j] + 1;
						WorldGen.numDPlats++;
						break;
					}
				}
				for (int m = WorldGen.dRoomT[j]; m <= WorldGen.dRoomB[j]; m++)
				{
					if (!Main.tile[WorldGen.dRoomL[j] - 1, m].active)
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
					if (!Main.tile[WorldGen.dRoomR[j] + 1, n].active)
					{
						WorldGen.DDoorX[WorldGen.numDDoors] = WorldGen.dRoomR[j] + 1;
						WorldGen.DDoorY[WorldGen.numDDoors] = n;
						WorldGen.DDoorPos[WorldGen.numDDoors] = 1;
						WorldGen.numDDoors++;
						break;
					}
				}
			}
			Program.printData("Creating dungeon: 70%");
			int num12 = 0;
			int num13 = 1000;
			int num14 = 0;
			while (num14 < Main.maxTilesX / 125)
			{
				num12++;
				int num15 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int num16 = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);
				int num17 = num15;
				if ((int)Main.tile[num15, num16].wall == wallType && !Main.tile[num15, num16].active)
				{
					int num18 = 1;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num18 = -1;
					}
					while (!Main.tile[num15, num16].active)
					{
						num16 += num18;
					}
					if (Main.tile[num15 - 1, num16].active && Main.tile[num15 + 1, num16].active && !Main.tile[num15 - 1, num16 - num18].active && !Main.tile[num15 + 1, num16 - num18].active)
					{
						num14++;
						int num19 = WorldGen.genRand.Next(5, 10);
						while (Main.tile[num15 - 1, num16].active && Main.tile[num15, num16 + num18].active && Main.tile[num15, num16].active && !Main.tile[num15, num16 - num18].active && num19 > 0)
						{
							Main.tile[num15, num16].type = 48;
							if (!Main.tile[num15 - 1, num16 - num18].active && !Main.tile[num15 + 1, num16 - num18].active)
							{
								Main.tile[num15, num16 - num18].type = 48;
								Main.tile[num15, num16 - num18].active = true;
							}
							num15--;
							num19--;
						}
						num19 = WorldGen.genRand.Next(5, 10);
						num15 = num17 + 1;
						while (Main.tile[num15 + 1, num16].active && Main.tile[num15, num16 + num18].active && Main.tile[num15, num16].active && !Main.tile[num15, num16 - num18].active && num19 > 0)
						{
							Main.tile[num15, num16].type = 48;
							if (!Main.tile[num15 - 1, num16 - num18].active && !Main.tile[num15 + 1, num16 - num18].active)
							{
								Main.tile[num15, num16 - num18].type = 48;
								Main.tile[num15, num16 - num18].active = true;
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
			Program.printData("Creating dungeon: 75%");
			while (num14 < Main.maxTilesX / 125)
			{
				num12++;
				int num20 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int num21 = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);
				int num22 = num21;
				if ((int)Main.tile[num20, num21].wall == wallType && !Main.tile[num20, num21].active)
				{
					int num23 = 1;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num23 = -1;
					}
					while (num20 > 5 && num20 < Main.maxTilesX - 5 && !Main.tile[num20, num21].active)
					{
						num20 += num23;
					}
					if (Main.tile[num20, num21 - 1].active && Main.tile[num20, num21 + 1].active && !Main.tile[num20 - num23, num21 - 1].active && !Main.tile[num20 - num23, num21 + 1].active)
					{
						num14++;
						int num24 = WorldGen.genRand.Next(5, 10);
						while (Main.tile[num20, num21 - 1].active && Main.tile[num20 + num23, num21].active && Main.tile[num20, num21].active && !Main.tile[num20 - num23, num21].active && num24 > 0)
						{
							Main.tile[num20, num21].type = 48;
							if (!Main.tile[num20 - num23, num21 - 1].active && !Main.tile[num20 - num23, num21 + 1].active)
							{
								Main.tile[num20 - num23, num21].type = 48;
								Main.tile[num20 - num23, num21].active = true;
							}
							num21--;
							num24--;
						}
						num24 = WorldGen.genRand.Next(5, 10);
						num21 = num22 + 1;
						while (Main.tile[num20, num21 + 1].active && Main.tile[num20 + num23, num21].active && Main.tile[num20, num21].active && !Main.tile[num20 - num23, num21].active && num24 > 0)
						{
							Main.tile[num20, num21].type = 48;
							if (!Main.tile[num20 - num23, num21 - 1].active && !Main.tile[num20 - num23, num21 + 1].active)
							{
								Main.tile[num20 - num23, num21].type = 48;
								Main.tile[num20 - num23, num21].active = true;
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
			Program.printData("Creating dungeon: 80%");
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
					while (!Main.tile[num30, num31].active)
					{
						num31--;
					}
					if (!Main.tileDungeon[(int)Main.tile[num30, num31].type])
					{
						flag = false;
					}
					int num32 = num31;
					num31 = WorldGen.DDoorY[num25];
					while (!Main.tile[num30, num31].active)
					{
						num31++;
					}
					if (!Main.tileDungeon[(int)Main.tile[num30, num31].type])
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
								if (Main.tile[num38, num39].active && Main.tile[num38, num39].type == 10)
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
									if (Main.tile[num41, num40].active)
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
					while (!Main.tile[num42, num43].active)
					{
						Main.tile[num42, num43].active = false;
						num43++;
					}
					while (!Main.tile[num42, num44].active)
					{
						num44--;
					}
					num43--;
					num44++;
					for (int num45 = num44; num45 < num43 - 2; num45++)
					{
						Main.tile[num42, num45].active = true;
						Main.tile[num42, num45].type = (byte)tileType;
					}
					WorldGen.PlaceTile(num42, num43, 10, true, false, -1);
					num42--;
					int num46 = num43 - 3;
					while (!Main.tile[num42, num46].active)
					{
						num46--;
					}
					if (num43 - num46 < num43 - num44 + 5 && Main.tileDungeon[(int)Main.tile[num42, num46].type])
					{
						for (int num47 = num43 - 4 - WorldGen.genRand.Next(3); num47 > num46; num47--)
						{
							Main.tile[num42, num47].active = true;
							Main.tile[num42, num47].type = (byte)tileType;
						}
					}
					num42 += 2;
					num46 = num43 - 3;
					while (!Main.tile[num42, num46].active)
					{
						num46--;
					}
					if (num43 - num46 < num43 - num44 + 5 && Main.tileDungeon[(int)Main.tile[num42, num46].type])
					{
						for (int num48 = num43 - 4 - WorldGen.genRand.Next(3); num48 > num46; num48--)
						{
							Main.tile[num42, num48].active = true;
							Main.tile[num42, num48].type = (byte)tileType;
						}
					}
					num43++;
					num42--;
					Main.tile[num42 - 1, num43].active = true;
					Main.tile[num42 - 1, num43].type = (byte)tileType;
					Main.tile[num42 + 1, num43].active = true;
					Main.tile[num42 + 1, num43].type = (byte)tileType;
				}
			}
			Program.printData("Creating dungeon: 85%");
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
					if (Main.tile[num55, num54].active)
					{
						flag3 = true;
					}
					else
					{
						while (!Main.tile[num55, num54].active)
						{
							num55--;
							if (!Main.tileDungeon[(int)Main.tile[num55, num54].type])
							{
								flag3 = true;
							}
						}
						while (!Main.tile[num56, num54].active)
						{
							num56++;
							if (!Main.tileDungeon[(int)Main.tile[num56, num54].type])
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
								if (Main.tile[num61, num62].active && Main.tile[num61, num62].type == 19)
								{
									flag4 = false;
									break;
								}
							}
						}
						for (int num63 = num54 + 3; num63 >= num54 - 5; num63--)
						{
							if (Main.tile[num50, num63].active)
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
					while (!Main.tile[num64, num65].active)
					{
						Main.tile[num64, num65].active = true;
						Main.tile[num64, num65].type = 19;
						num64--;
					}
					while (!Main.tile[num66, num65].active)
					{
						Main.tile[num66, num65].active = true;
						Main.tile[num66, num65].type = 19;
						num66++;
					}
				}
			}
			Program.printData("Creating dungeon: 90%");
			num12 = 0;
			num13 = 1000;
			num14 = 0;
			while (num14 < Main.maxTilesX / 20)
			{
				num12++;
				int num67 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int num68 = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
				bool flag5 = true;
				if ((int)Main.tile[num67, num68].wall == wallType && !Main.tile[num67, num68].active)
				{
					int num69 = 1;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num69 = -1;
					}
					while (flag5 && !Main.tile[num67, num68].active)
					{
						num67 -= num69;
						if (num67 < 5 || num67 > Main.maxTilesX - 5)
						{
							flag5 = false;
						}
						else
						{
							if (Main.tile[num67, num68].active && !Main.tileDungeon[(int)Main.tile[num67, num68].type])
							{
								flag5 = false;
							}
						}
					}
					if (flag5 && Main.tile[num67, num68].active && Main.tileDungeon[(int)Main.tile[num67, num68].type] && Main.tile[num67, num68 - 1].active && Main.tileDungeon[(int)Main.tile[num67, num68 - 1].type] && Main.tile[num67, num68 + 1].active && Main.tileDungeon[(int)Main.tile[num67, num68 + 1].type])
					{
						num67 += num69;
						for (int num70 = num67 - 3; num70 <= num67 + 3; num70++)
						{
							for (int num71 = num68 - 3; num71 <= num68 + 3; num71++)
							{
								if (Main.tile[num70, num71].active && Main.tile[num70, num71].type == 19)
								{
									flag5 = false;
									break;
								}
							}
						}
						if (flag5 && (!Main.tile[num67, num68 - 1].active & !Main.tile[num67, num68 - 2].active & !Main.tile[num67, num68 - 3].active))
						{
							int num72 = num67;
							int num73 = num67;
							while (num72 > WorldGen.dMinX && num72 < WorldGen.dMaxX && !Main.tile[num72, num68].active && !Main.tile[num72, num68 - 1].active && !Main.tile[num72, num68 + 1].active)
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
									Main.tile[num67, num68].active = true;
									Main.tile[num67, num68].type = 19;
									if (flag6)
									{
										WorldGen.PlaceTile(num67, num68 - 1, 50, true, false, -1);
										if (WorldGen.genRand.Next(50) == 0 && Main.tile[num67, num68 - 1].type == 50)
										{
											Main.tile[num67, num68 - 1].frameX = 90;
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
									WorldGen.PlaceTile(num67, num68, num75, true, false, -1);
									if (Main.tile[num67, num68].type == 13)
									{
										if (WorldGen.genRand.Next(2) == 0)
										{
											Main.tile[num67, num68].frameX = 18;
										}
										else
										{
											Main.tile[num67, num68].frameX = 36;
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
			Program.printData("Creating dungeon: 95%");
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
						if (WorldGen.AddBuriedChest(i2, j2, num79))
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
			while (num14 < Main.maxTilesX / 20)
			{
				num12++;
				int num81 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int num82 = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
				if ((int)Main.tile[num81, num82].wall == wallType)
				{
					int num83 = num82;
					while (num83 > WorldGen.dMinY)
					{
						if (Main.tile[num81, num83 - 1].active && (int)Main.tile[num81, num83 - 1].type == tileType)
						{
							bool flag7 = false;
							for (int num84 = num81 - 15; num84 < num81 + 15; num84++)
							{
								for (int num85 = num83 - 15; num85 < num83 + 15; num85++)
								{
									if (num84 > 0 && num84 < Main.maxTilesX && num85 > 0 && num85 < Main.maxTilesY && Main.tile[num84, num85].type == 42)
									{
										flag7 = true;
										break;
									}
								}
							}
							if (Main.tile[num81 - 1, num83].active || Main.tile[num81 + 1, num83].active || Main.tile[num81 - 1, num83 + 1].active || Main.tile[num81 + 1, num83 + 1].active || Main.tile[num81, num83 + 2].active)
							{
								flag7 = true;
							}
							if (flag7)
							{
								break;
							}
							WorldGen.Place1x2Top(num81, num83, 42);
							if (Main.tile[num81, num83].type == 42)
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
		}
		public static void DungeonStairs(int i, int j, int tileType, int wallType)
		{
			Vector2 value = new Vector2();
			double num = (double)WorldGen.genRand.Next(5, 9);
			int num2 = 1;
            Vector2 value2 = new Vector2();
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
						Main.tile[num12, num13].active = false;
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
			Vector2 value = new Vector2();
			double num = (double)WorldGen.genRand.Next(4, 6);
			Vector2 vector = new Vector2();
			Vector2 value2 = new Vector2();
			int num2 = 1;
            Vector2 value3 = new Vector2();
			value3.X = (float)i;
			value3.Y = (float)j;
			int k = WorldGen.genRand.Next(35, 80);
			if (forceX)
			{
				k += 20;
				WorldGen.lastDungeonHall = new Vector2();
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
						if (value3.Y > (float)(WorldGen.lastMaxTilesY + 200))
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
						Main.tile[l, m].liquid = 0;
						if (Main.tile[l, m].wall == 0)
						{
							Main.tile[l, m].active = true;
							Main.tile[l, m].type = (byte)tileType;
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
						Main.tile[num9, num10].active = false;
						Main.tile[num9, num10].wall = (byte)wallType;
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
            Vector2 value = new Vector2();
			value.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            Vector2 value2 = new Vector2();
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
						Main.tile[num11, num12].active = false;
						Main.tile[num11, num12].wall = (byte)wallType;
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
            Vector2 vector = new Vector2();
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
			for (int m = num8; m < num9; m++)
			{
				for (int n = num10; n < num11; n++)
				{
					if ((int)Main.tile[m, n].wall != wallType)
					{
						Main.tile[m, n].active = true;
						Main.tile[m, n].type = (byte)tileType;
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
					if ((int)Main.tile[num12, num13].wall != wallType)
					{
						Main.tile[num12, num13].active = true;
						Main.tile[num12, num13].type = (byte)tileType;
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
					if ((int)Main.tile[num17, num18].wall != wallType)
					{
						Main.tile[num17, num18].active = true;
						Main.tile[num17, num18].type = (byte)tileType;
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
					Main.tile[num23, num24].wall = (byte)wallType;
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
					Main.tile[num25, num26].active = false;
					Main.tile[num25, num26].wall = (byte)wallType;
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
					if ((int)Main.tile[num27, num28].wall != wallType)
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
							Main.tile[num27, num28].wall = 0;
							Main.tile[num27, num28].active = true;
							Main.tile[num27, num28].type = (byte)tileType;
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
					if ((int)Main.tile[num31, num32].wall != wallType)
					{
						Main.tile[num31, num32].active = true;
						Main.tile[num31, num32].type = (byte)tileType;
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
					if ((int)Main.tile[num33, num34].wall != wallType)
					{
						Main.tile[num33, num34].active = true;
						Main.tile[num33, num34].type = (byte)tileType;
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
					if ((int)Main.tile[num35, num36].wall != wallType)
					{
						Main.tile[num35, num36].active = true;
						Main.tile[num35, num36].type = (byte)tileType;
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
					Main.tile[num37, num38].wall = 0;
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
					Main.tile[num39, num40].active = false;
					Main.tile[num39, num40].wall = 0;
				}
			}
			for (int num41 = num4; num41 < num5; num41++)
			{
				if (!Main.tile[num41, num7].active)
				{
					Main.tile[num41, num7].active = true;
					Main.tile[num41, num7].type = 19;
				}
			}
			Main.dungeonX = (int)vector.X;
			Main.dungeonY = num7;
			int num42 = NPC.NewNPC(WorldGen.dungeonX * 16 + 8, WorldGen.dungeonY * 16, 37, 0);
			Main.npc[num42].homeless = false;
			Main.npc[num42].homeTileX = Main.dungeonX;
			Main.npc[num42].homeTileY = Main.dungeonY;
			if (num3 == 1)
			{
				int num43 = 0;
				for (int num44 = num5; num44 < num5 + 25; num44++)
				{
					num43++;
					for (int num45 = num7 + num43; num45 < num7 + 25; num45++)
					{
						Main.tile[num44, num45].active = true;
						Main.tile[num44, num45].type = (byte)tileType;
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
						Main.tile[num47, num48].active = true;
						Main.tile[num47, num48].type = (byte)tileType;
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
					Main.tile[num51, num52].active = false;
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
			int k = j;
			while (k < Main.maxTilesY)
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
								int num5 = WorldGen.genRand.Next(4);
								int stack = WorldGen.genRand.Next(8) + 3;
								if (num5 == 0)
								{
									Main.chest[num2].item[num3].SetDefaults(19);
								}
								if (num5 == 1)
								{
									Main.chest[num2].item[num3].SetDefaults(20);
								}
								if (num5 == 2)
								{
									Main.chest[num2].item[num3].SetDefaults(21);
								}
								if (num5 == 3)
								{
									Main.chest[num2].item[num3].SetDefaults(22);
								}
								Main.chest[num2].item[num3].stack = stack;
								num3++;
							}
							if (WorldGen.genRand.Next(2) == 0)
							{
								int num6 = WorldGen.genRand.Next(2);
								int stack2 = WorldGen.genRand.Next(26) + 25;
								if (num6 == 0)
								{
									Main.chest[num2].item[num3].SetDefaults(40);
								}
								if (num6 == 1)
								{
									Main.chest[num2].item[num3].SetDefaults(42);
								}
								Main.chest[num2].item[num3].stack = stack2;
								num3++;
							}
							if (WorldGen.genRand.Next(2) == 0)
							{
								int num7 = WorldGen.genRand.Next(1);
								int stack3 = WorldGen.genRand.Next(3) + 3;
								if (num7 == 0)
								{
									Main.chest[num2].item[num3].SetDefaults(28);
								}
								Main.chest[num2].item[num3].stack = stack3;
								num3++;
							}
							if (WorldGen.genRand.Next(2) == 0)
							{
								int num8 = WorldGen.genRand.Next(2);
								int stack4 = WorldGen.genRand.Next(11) + 10;
								if (num8 == 0)
								{
									Main.chest[num2].item[num3].SetDefaults(8);
								}
								if (num8 == 1)
								{
									Main.chest[num2].item[num3].SetDefaults(31);
								}
								Main.chest[num2].item[num3].stack = stack4;
								num3++;
							}
							if (WorldGen.genRand.Next(2) == 0)
							{
								Main.chest[num2].item[num3].SetDefaults(73);
								Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(1, 3);
								num3++;
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
				Main.tile[num2 + 1, num].frameX = ((short)(num3 + 18));
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
				Main.tile[num2 + 1, num + 1].frameX = ((short)(num3 + 18));
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
				Main.tile[num2 + 1, num + 2].frameX = ((short)(num3 + 18));
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
			if (Main.tile[x, num].frameY == 0 && Main.tile[x, num + 1].frameY == 18 && Main.tile[x, num].type == type && Main.tile[x, num + 1].type == type)
			{
				flag = false;
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
		public static void CheckOnTable1x1(int x, int y, int type)
		{
			if (Main.tile[x, y + 1] != null && (!Main.tile[x, y + 1].active || !Main.tileTable[(int)Main.tile[x, y + 1].type]))
			{
				if (type == 78)
				{
					if (!Main.tile[x, y + 1].active || !Main.tileSolid[(int)Main.tile[x, y + 1].type])
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
			for (int l = num; l < num2; l++)
			{
				num5 = 0;
				for (int m = num3; m < num4; m++)
				{
					if (!Main.tile[l, m].active || (int)Main.tile[l, m].type != type)
					{
						flag = true;
						break;
					}
					if ((int)(Main.tile[l, m].frameX / 18) != k + num8 * 2 || (int)(Main.tile[l, m].frameY / 18) != num5)
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
				for (int n = num; n < num2; n++)
				{
					for (int num9 = num3; num9 < num4; num9++)
					{
						if ((int)Main.tile[n, num9].type == type)
						{
							WorldGen.KillTile(n, num9, false, false, false);
						}
					}
				}
				Sign.KillSign(num6, num7);
				Item.NewItem(x * 16, y * 16, 32, 32, 171, 1, false);
				WorldGen.destroyObject = false;
				return;
			}
			int num10 = 36 * num8;
			for (int num11 = 0; num11 < 2; num11++)
			{
				for (int num12 = 0; num12 < 2; num12++)
				{
					Main.tile[num6 + num11, num7 + num12].active = true;
					Main.tile[num6 + num11, num7 + num12].type = (byte)type;
					Main.tile[num6 + num11, num7 + num12].frameX = (short)(num10 + 18 * num11);
					Main.tile[num6 + num11, num7 + num12].frameY = (short)(18 * num12);
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
							return false;
						}
						num5--;
						num7 = 3;
					}
				}
			}
			if (Main.tile[num5, num6].active || Main.tile[num5 + 1, num6].active || Main.tile[num5, num6 + 1].active || Main.tile[num5 + 1, num6 + 1].active)
			{
				return false;
			}
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
			return true;
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
			if (type == 78 && !Main.tile[x, y].active && Main.tile[x, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type])
			{
				flag = true;
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
			if (WorldGen.destroyObject)
			{
				return;
			}
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
			if (Main.tile[x, num].frameY == 0 && Main.tile[x, num + 1].frameY == 18 && Main.tile[x, num].type == type && Main.tile[x, num + 1].type == type)
			{
				flag = false;
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
		public static void Check2x1(int i, int y, byte type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
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
			if (Main.tile[num, y] == null)
			{
				Main.tile[num, y] = new Tile();
			}
			if (Main.tile[num, y].frameX == 0 && Main.tile[num + 1, y].frameX == 18 && Main.tile[num, y].type == type && Main.tile[num + 1, y].type == type)
			{
				flag = false;
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
			if (WorldGen.destroyObject)
			{
				return;
			}
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
				for (int m = num; m < num + 4; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile[m, n].type == type && Main.tile[m, n].active)
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
		public static void Check3x2(int i, int j, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
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
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile[m, n].type == type && Main.tile[m, n].active)
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
				Main.tile[x, y - 1].frameX = ((short)(18 + num));
				Main.tile[x, y - 1].type = (byte)type;
				Main.tile[x + 1, y - 1].active = true;
				Main.tile[x + 1, y - 1].frameY = 0;
				Main.tile[x + 1, y - 1].frameX = ((short)(36 + num));
				Main.tile[x + 1, y - 1].type = (byte)type;
				Main.tile[x + 2, y - 1].active = true;
				Main.tile[x + 2, y - 1].frameY = 0;
				Main.tile[x + 2, y - 1].frameX = ((short)(54 + num));
				Main.tile[x + 2, y - 1].type = (byte)type;
				Main.tile[x - 1, y].active = true;
				Main.tile[x - 1, y].frameY = 18;
				Main.tile[x - 1, y].frameX = num;
				Main.tile[x - 1, y].type = (byte)type;
				Main.tile[x, y].active = true;
				Main.tile[x, y].frameY = 18;
				Main.tile[x, y].frameX = ((short)(18 + num));
				Main.tile[x, y].type = (byte)type;
				Main.tile[x + 1, y].active = true;
				Main.tile[x + 1, y].frameY = 18;
				Main.tile[x + 1, y].frameX =((short)(36 + num));
				Main.tile[x + 1, y].type = (byte)type;
				Main.tile[x + 2, y].active = true;
				Main.tile[x + 2, y].frameY = 18;
				Main.tile[x + 2, y].frameX = ((short)(54 + num));
				Main.tile[x + 2, y].type = (byte)type;
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
		public static void Check3x3(int i, int j, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
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
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile[m, n].type == type && Main.tile[m, n].active)
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
			if ((double)y > Main.worldSurface - 1.0)
			{
				return;
			}
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
				for (int k = 0; k < 2; k++)
				{
					for (int l = -3; l < 1; l++)
					{
						int num = k * 18 + WorldGen.genRand.Next(3) * 36;
						int num2 = (l + 3) * 18;
						Main.tile[x + k, y + l].active = true;
						Main.tile[x + k, y + l].frameX = (short)num;
						Main.tile[x + k, y + l].frameY = (short)num2;
						Main.tile[x + k, y + l].type = (byte)type;
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
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 4; num3++)
					{
						if ((int)Main.tile[num2, num3].type == type && Main.tile[num2, num3].active)
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
				for (int k = 0; k < 2; k++)
				{
					for (int l = -1; l < 1; l++)
					{
						int num = k * 18 + WorldGen.genRand.Next(3) * 36;
						int num2 = (l + 1) * 18;
						Main.tile[x + k, y + l].active = true;
						Main.tile[x + k, y + l].frameX = (short)num;
						Main.tile[x + k, y + l].frameY = (short)num2;
						Main.tile[x + k, y + l].type = (byte)type;
					}
				}
				return true;
			}
			return false;
		}
		public static void CheckPot(int i, int j, int type = 28)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
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
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 2; num3++)
					{
						if ((int)Main.tile[num2, num3].type == type && Main.tile[num2, num3].active)
						{
							WorldGen.KillTile(num2, num3, false, false, false);
						}
					}
				}
				Gore.NewGore(new Vector2((float)(i * 16), (float)(j * 16)), new Vector2(), 51);
				Gore.NewGore(new Vector2((float)(i * 16), (float)(j * 16)), new Vector2(), 52);
				Gore.NewGore(new Vector2((float)(i * 16), (float)(j * 16)), new Vector2(), 53);
				int num4 = Main.rand.Next(10);
				if (num4 == 0 && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLife < Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLifeMax)
				{
					Item.NewItem(i * 16, j * 16, 16, 16, 58, 1, false);
				}
				else
				{
					if (num4 == 1 && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statMana < Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statManaMax)
					{
						Item.NewItem(i * 16, j * 16, 16, 16, 184, 1, false);
					}
					else
					{
						if (num4 == 2)
						{
							int stack = Main.rand.Next(3) + 1;
							Item.NewItem(i * 16, j * 16, 16, 16, 8, stack, false);
						}
						else
						{
							if (num4 == 3)
							{
								int stack2 = Main.rand.Next(8) + 3;
								Item.NewItem(i * 16, j * 16, 16, 16, 40, stack2, false);
							}
							else
							{
								if (num4 == 4)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 28, 1, false);
								}
								else
								{
									if (num4 == 5)
									{
										int stack3 = Main.rand.Next(4) + 1;
										Item.NewItem(i * 16, j * 16, 16, 16, 166, stack3, false);
									}
									else
									{
										float num5 = (float)(200 + WorldGen.genRand.Next(-100, 101));
										num5 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
										if (Main.rand.Next(5) == 0)
										{
											num5 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
										}
										if (Main.rand.Next(10) == 0)
										{
											num5 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
										}
										if (Main.rand.Next(15) == 0)
										{
											num5 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
										}
										if (Main.rand.Next(20) == 0)
										{
											num5 *= 1f + (float)Main.rand.Next(40, 81) * 0.01f;
										}
										if (Main.rand.Next(25) == 0)
										{
											num5 *= 1f + (float)Main.rand.Next(50, 101) * 0.01f;
										}
										while ((int)num5 > 0)
										{
											if (num5 > 1000000f)
											{
												int num6 = (int)(num5 / 1000000f);
												if (num6 > 50 && Main.rand.Next(2) == 0)
												{
													num6 /= Main.rand.Next(3) + 1;
												}
												if (Main.rand.Next(2) == 0)
												{
													num6 /= Main.rand.Next(3) + 1;
												}
												num5 -= (float)(1000000 * num6);
												Item.NewItem(i * 16, j * 16, 16, 16, 74, num6, false);
											}
											else
											{
												if (num5 > 10000f)
												{
													int num7 = (int)(num5 / 10000f);
													if (num7 > 50 && Main.rand.Next(2) == 0)
													{
														num7 /= Main.rand.Next(3) + 1;
													}
													if (Main.rand.Next(2) == 0)
													{
														num7 /= Main.rand.Next(3) + 1;
													}
													num5 -= (float)(10000 * num7);
													Item.NewItem(i * 16, j * 16, 16, 16, 73, num7, false);
												}
												else
												{
													if (num5 > 100f)
													{
														int num8 = (int)(num5 / 100f);
														if (num8 > 50 && Main.rand.Next(2) == 0)
														{
															num8 /= Main.rand.Next(3) + 1;
														}
														if (Main.rand.Next(2) == 0)
														{
															num8 /= Main.rand.Next(3) + 1;
														}
														num5 -= (float)(100 * num8);
														Item.NewItem(i * 16, j * 16, 16, 16, 72, num8, false);
													}
													else
													{
														int num9 = (int)num5;
														if (num9 > 50 && Main.rand.Next(2) == 0)
														{
															num9 /= Main.rand.Next(3) + 1;
														}
														if (Main.rand.Next(2) == 0)
														{
															num9 /= Main.rand.Next(4) + 1;
														}
														if (num9 < 1)
														{
															num9 = 1;
														}
														num5 -= (float)num9;
														Item.NewItem(i * 16, j * 16, 16, 16, 71, num9, false);
													}
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
			if (WorldGen.destroyObject)
			{
				return;
			}
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
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile[m, n].type == type && Main.tile[m, n].active)
						{
							WorldGen.KillTile(m, n, false, false, false);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, 48, 1, false);
				WorldGen.destroyObject = false;
			}
		}
		public static bool PlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1)
		{
			bool result = false;
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
						return false;
					}
					if (type == 2 && (Main.tile[i, j].type != 0 || !Main.tile[i, j].active))
					{
						return false;
					}
					if (type == 60 && (Main.tile[i, j].type != 59 || !Main.tile[i, j].active))
					{
						return false;
					}
					if (Main.tile[i, j].liquid > 0 && (type == 3 || type == 4 || type == 20 || type == 24 || type == 27 || type == 32 || type == 51 || type == 61 || type == 69 || type == 72 || type == 73))
					{
						return false;
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
									if (Main.tile[i, j].wall == 0 && Main.tile[i, j + 1].wall == 0)
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
					else
					{
						if (type == 61)
						{
							if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1].active && Main.tile[i, j + 1].type == 60)
							{
								if (WorldGen.genRand.Next(10) == 0)
								{
									Main.tile[i, j].active = true;
									Main.tile[i, j].type = 69;
									WorldGen.SquareTileFrame(i, j, true);
								}
								else
								{
									if (WorldGen.genRand.Next(15) == 0)
									{
										Main.tile[i, j].active = true;
										Main.tile[i, j].type = (byte)type;
										Main.tile[i, j].frameX = 144;
									}
									else
									{
										if (WorldGen.genRand.Next(1000) == 0)
										{
											Main.tile[i, j].active = true;
											Main.tile[i, j].type = (byte)type;
											Main.tile[i, j].frameX = 162;
										}
										else
										{
											Main.tile[i, j].active = true;
											Main.tile[i, j].type = (byte)type;
											Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(8) * 18);
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
						result = true;
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
					WorldGen.genRand.Next(3);
					//Main.PlaySound(0, i * 16, j * 16, 1);
					int num = 10;
					if (fail)
					{
						num = 3;
					}
					for (int k = 0; k < num; k++)
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
						return;
					}
					int num2 = 0;
					if (Main.tile[i, j].wall == 1)
					{
						num2 = 26;
					}
					if (Main.tile[i, j].wall == 4)
					{
						num2 = 93;
					}
					if (Main.tile[i, j].wall == 5)
					{
						num2 = 130;
					}
					if (Main.tile[i, j].wall == 6)
					{
						num2 = 132;
					}
					if (Main.tile[i, j].wall == 7)
					{
						num2 = 135;
					}
					if (Main.tile[i, j].wall == 8)
					{
						num2 = 138;
					}
					if (Main.tile[i, j].wall == 9)
					{
						num2 = 140;
					}
					if (Main.tile[i, j].wall == 10)
					{
						num2 = 142;
					}
					if (Main.tile[i, j].wall == 11)
					{
						num2 = 144;
					}
					if (Main.tile[i, j].wall == 12)
					{
						num2 = 146;
					}
					if (num2 > 0)
					{
						Item.NewItem(i * 16, j * 16, 16, 16, num2, 1, false);
					}
					Main.tile[i, j].wall = 0;
					WorldGen.SquareWallFrame(i, j, true);
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
					if (j >= 1 && Main.tile[i, j - 1].active && ((Main.tile[i, j - 1].type == 5 && Main.tile[i, j].type != 5) || (Main.tile[i, j - 1].type == 21 && Main.tile[i, j].type != 21) || (Main.tile[i, j - 1].type == 26 && Main.tile[i, j].type != 26) || (Main.tile[i, j - 1].type == 72 && Main.tile[i, j].type != 72) || (Main.tile[i, j - 1].type == 12 && Main.tile[i, j].type != 12)) && (Main.tile[i, j - 1].type != 5 || ((Main.tile[i, j - 1].frameX != 66 || Main.tile[i, j - 1].frameY < 0 || Main.tile[i, j - 1].frameY > 44) && (Main.tile[i, j - 1].frameX != 88 || Main.tile[i, j - 1].frameY < 66 || Main.tile[i, j - 1].frameY > 110) && Main.tile[i, j - 1].frameY < 198)))
					{
						return;
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
					if (effectOnly)
					{
						return;
					}
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
						return;
					}
					if (Main.tile[i, j].type == 21 && Main.netMode != 1)
					{
						int x = i - (int)(Main.tile[i, j].frameX / 18);
						int y = j - (int)(Main.tile[i, j].frameY / 18);
						if (!Chest.DestroyChest(x, y))
						{
							return;
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
																																														if (Main.tile[i, j].frameX >= 108 && Main.tile[i, j].frameX <= 126)
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
		public static bool PlayerLOS(int x, int y)
		{
			Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					Rectangle value = new Rectangle((int)((double)Main.player[i].position.X + (double)Main.player[i].width * 0.5 - (double)Main.screenWidth * 0.6), (int)((double)Main.player[i].position.Y + (double)Main.player[i].height * 0.5 - (double)Main.screenHeight * 0.6), (int)((double)Main.screenWidth * 1.2), (int)((double)Main.screenHeight * 1.2));
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
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random();
            }
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
				if (Main.tile[num4, num5] != null)
				{
					if (Main.tile[num4, num5].liquid > 32)
					{
						if (Main.tile[num4, num5].active && (Main.tile[num4, num5].type == 3 || Main.tile[num4, num5].type == 20 || Main.tile[num4, num5].type == 24 || Main.tile[num4, num5].type == 27 || Main.tile[num4, num5].type == 73))
						{
							WorldGen.KillTile(num4, num5, false, false, false);
							if (Main.netMode == 2)
							{
								NetMessage.SendData(17, -1, -1, "", 0, (float)num4, (float)num5, 0f);
							}
						}
					}
					else
					{
						if (Main.tile[num4, num5].active)
						{
							if (Main.tile[num4, num5].type == 78)
							{
								if (!Main.tile[num4, num8].active)
								{
									WorldGen.PlaceTile(num4, num8, 3, true, false, -1);
									if (Main.netMode == 2 && Main.tile[num4, num8].active)
									{
										NetMessage.SendTileSquare(-1, num4, num8, 1);
									}
								}
							}
							else
							{
								if (Main.tile[num4, num5].type == 2 || Main.tile[num4, num5].type == 23 || Main.tile[num4, num5].type == 32)
								{
									int num10 = (int)Main.tile[num4, num5].type;
									if (!Main.tile[num4, num8].active && WorldGen.genRand.Next(10) == 0 && num10 == 2)
									{
										WorldGen.PlaceTile(num4, num8, 3, true, false, -1);
										if (Main.netMode == 2 && Main.tile[num4, num8].active)
										{
											NetMessage.SendTileSquare(-1, num4, num8, 1);
										}
									}
									if (!Main.tile[num4, num8].active && WorldGen.genRand.Next(10) == 0 && num10 == 23)
									{
										WorldGen.PlaceTile(num4, num8, 24, true, false, -1);
										if (Main.netMode == 2 && Main.tile[num4, num8].active)
										{
											NetMessage.SendTileSquare(-1, num4, num8, 1);
										}
									}
									bool flag2 = false;
									for (int j = num6; j < num7; j++)
									{
										for (int k = num8; k < num9; k++)
										{
											if ((num4 != j || num5 != k) && Main.tile[j, k].active)
											{
												if (num10 == 32)
												{
													num10 = 23;
												}
												if (Main.tile[j, k].type == 0 || (num10 == 23 && Main.tile[j, k].type == 2))
												{
													WorldGen.SpreadGrass(j, k, 0, num10, false);
													if (num10 == 23)
													{
														WorldGen.SpreadGrass(j, k, 2, num10, false);
													}
													if ((int)Main.tile[j, k].type == num10)
													{
														WorldGen.SquareTileFrame(j, k, true);
														flag2 = true;
													}
												}
											}
										}
									}
									if (Main.netMode == 2 && flag2)
									{
										NetMessage.SendTileSquare(-1, num4, num5, 3);
									}
								}
								else
								{
									if (Main.tile[num4, num5].type == 20 && !WorldGen.PlayerLOS(num4, num5) && WorldGen.genRand.Next(5) == 0)
									{
										WorldGen.GrowTree(num4, num5);
									}
								}
							}
							if (Main.tile[num4, num5].type == 3 && WorldGen.genRand.Next(10) == 0 && Main.tile[num4, num5].frameX < 144)
							{
								Main.tile[num4, num5].type = 73;
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num4, num5, 3);
								}
							}
							if (Main.tile[num4, num5].type == 32 && WorldGen.genRand.Next(3) == 0)
							{
								int num11 = num4;
								int num12 = num5;
								int num13 = 0;
								if (Main.tile[num11 + 1, num12].active && Main.tile[num11 + 1, num12].type == 32)
								{
									num13++;
								}
								if (Main.tile[num11 - 1, num12].active && Main.tile[num11 - 1, num12].type == 32)
								{
									num13++;
								}
								if (Main.tile[num11, num12 + 1].active && Main.tile[num11, num12 + 1].type == 32)
								{
									num13++;
								}
								if (Main.tile[num11, num12 - 1].active && Main.tile[num11, num12 - 1].type == 32)
								{
									num13++;
								}
								if (num13 < 3 || Main.tile[num4, num5].type == 23)
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
									if (!Main.tile[num11, num12].active)
									{
										num13 = 0;
										if (Main.tile[num11 + 1, num12].active && Main.tile[num11 + 1, num12].type == 32)
										{
											num13++;
										}
										if (Main.tile[num11 - 1, num12].active && Main.tile[num11 - 1, num12].type == 32)
										{
											num13++;
										}
										if (Main.tile[num11, num12 + 1].active && Main.tile[num11, num12 + 1].type == 32)
										{
											num13++;
										}
										if (Main.tile[num11, num12 - 1].active && Main.tile[num11, num12 - 1].type == 32)
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
													if (Math.Abs(l - num11) * 2 + Math.Abs(m - num12) < 9 && Main.tile[l, m].active && Main.tile[l, m].type == 23 && Main.tile[l, m - 1].active && Main.tile[l, m - 1].type == 32 && Main.tile[l, m - 1].liquid == 0)
													{
														flag3 = true;
														break;
													}
												}
											}
											if (flag3)
											{
												Main.tile[num11, num12].type = 32;
												Main.tile[num11, num12].active = true;
												WorldGen.SquareTileFrame(num11, num12, true);
												if (Main.netMode == 2)
												{
													NetMessage.SendTileSquare(-1, num11, num12, 3);
												}
											}
										}
									}
								}
							}
							if ((Main.tile[num4, num5].type == 2 || Main.tile[num4, num5].type == 52) && WorldGen.genRand.Next(5) == 0 && !Main.tile[num4, num5 + 1].active && !Main.tile[num4, num5 + 1].lava)
							{
								bool flag4 = false;
								for (int n = num5; n > num5 - 10; n--)
								{
									if (Main.tile[num4, n].active && Main.tile[num4, n].type == 2)
									{
										flag4 = true;
										break;
									}
								}
								if (flag4)
								{
									int num20 = num4;
									int num21 = num5 + 1;
									Main.tile[num20, num21].type = 52;
									Main.tile[num20, num21].active = true;
									WorldGen.SquareTileFrame(num20, num21, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num20, num21, 3);
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
				}
				num3++;
			}
			int num22 = 0;
			while ((float)num22 < (float)(Main.maxTilesX * Main.maxTilesY) * num2)
			{
				int num23 = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
				int num24 = WorldGen.genRand.Next((int)Main.worldSurface + 2, Main.maxTilesY - 200);
				int num25 = num23 - 1;
				int num26 = num23 + 2;
				int num27 = num24 - 1;
				int num28 = num24 + 2;
				if (num25 < 10)
				{
					num25 = 10;
				}
				if (num26 > Main.maxTilesX - 10)
				{
					num26 = Main.maxTilesX - 10;
				}
				if (num27 < 10)
				{
					num27 = 10;
				}
				if (num28 > Main.maxTilesY - 10)
				{
					num28 = Main.maxTilesY - 10;
				}
				if (Main.tile[num23, num24] != null)
				{
					if (Main.tile[num23, num24].liquid > 32)
					{
						if (Main.tile[num23, num24].active && (Main.tile[num23, num24].type == 61 || Main.tile[num23, num24].type == 74))
						{
							WorldGen.KillTile(num23, num24, false, false, false);
							if (Main.netMode == 2)
							{
								NetMessage.SendData(17, -1, -1, "", 0, (float)num23, (float)num24, 0f);
							}
						}
					}
					else
					{
						if (Main.tile[num23, num24].active)
						{
							if (Main.tile[num23, num24].type == 60)
							{
								int type = (int)Main.tile[num23, num24].type;
								if (!Main.tile[num23, num27].active && WorldGen.genRand.Next(10) == 0)
								{
									WorldGen.PlaceTile(num23, num27, 61, true, false, -1);
									if (Main.netMode == 2 && Main.tile[num23, num27].active)
									{
										NetMessage.SendTileSquare(-1, num23, num27, 1);
									}
								}
								bool flag5 = false;
								for (int num29 = num25; num29 < num26; num29++)
								{
									for (int num30 = num27; num30 < num28; num30++)
									{
										if ((num23 != num29 || num24 != num30) && Main.tile[num29, num30].active && Main.tile[num29, num30].type == 59)
										{
											WorldGen.SpreadGrass(num29, num30, 59, type, false);
											if ((int)Main.tile[num29, num30].type == type)
											{
												WorldGen.SquareTileFrame(num29, num30, true);
												flag5 = true;
											}
										}
									}
								}
								if (Main.netMode == 2 && flag5)
								{
									NetMessage.SendTileSquare(-1, num23, num24, 3);
								}
							}
							if (Main.tile[num23, num24].type == 61 && WorldGen.genRand.Next(3) == 0 && Main.tile[num23, num24].frameX < 144)
							{
								Main.tile[num23, num24].type = 74;
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num23, num24, 3);
								}
							}
							if ((Main.tile[num23, num24].type == 60 || Main.tile[num23, num24].type == 62) && WorldGen.genRand.Next(5) == 0 && !Main.tile[num23, num24 + 1].active && !Main.tile[num23, num24 + 1].lava)
							{
								bool flag6 = false;
								for (int num31 = num24; num31 > num24 - 10; num31--)
								{
									if (Main.tile[num23, num31].active && Main.tile[num23, num31].type == 60)
									{
										flag6 = true;
										break;
									}
								}
								if (flag6)
								{
									int num32 = num23;
									int num33 = num24 + 1;
									Main.tile[num32, num33].type = 62;
									Main.tile[num32, num33].active = true;
									WorldGen.SquareTileFrame(num32, num33, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num32, num33, 3);
									}
								}
							}
							if (Main.tile[num23, num24].type == 69 && WorldGen.genRand.Next(3) == 0)
							{
								int num34 = num23;
								int num35 = num24;
								int num36 = 0;
								if (Main.tile[num34 + 1, num35].active && Main.tile[num34 + 1, num35].type == 69)
								{
									num36++;
								}
								if (Main.tile[num34 - 1, num35].active && Main.tile[num34 - 1, num35].type == 69)
								{
									num36++;
								}
								if (Main.tile[num34, num35 + 1].active && Main.tile[num34, num35 + 1].type == 69)
								{
									num36++;
								}
								if (Main.tile[num34, num35 - 1].active && Main.tile[num34, num35 - 1].type == 69)
								{
									num36++;
								}
								if (num36 < 3 || Main.tile[num23, num24].type == 60)
								{
									int num37 = WorldGen.genRand.Next(4);
									if (num37 == 0)
									{
										num35--;
									}
									else
									{
										if (num37 == 1)
										{
											num35++;
										}
										else
										{
											if (num37 == 2)
											{
												num34--;
											}
											else
											{
												if (num37 == 3)
												{
													num34++;
												}
											}
										}
									}
									if (!Main.tile[num34, num35].active)
									{
										num36 = 0;
										if (Main.tile[num34 + 1, num35].active && Main.tile[num34 + 1, num35].type == 69)
										{
											num36++;
										}
										if (Main.tile[num34 - 1, num35].active && Main.tile[num34 - 1, num35].type == 69)
										{
											num36++;
										}
										if (Main.tile[num34, num35 + 1].active && Main.tile[num34, num35 + 1].type == 69)
										{
											num36++;
										}
										if (Main.tile[num34, num35 - 1].active && Main.tile[num34, num35 - 1].type == 69)
										{
											num36++;
										}
										if (num36 < 2)
										{
											int num38 = 7;
											int num39 = num34 - num38;
											int num40 = num34 + num38;
											int num41 = num35 - num38;
											int num42 = num35 + num38;
											bool flag7 = false;
											for (int num43 = num39; num43 < num40; num43++)
											{
												for (int num44 = num41; num44 < num42; num44++)
												{
													if (Math.Abs(num43 - num34) * 2 + Math.Abs(num44 - num35) < 9 && Main.tile[num43, num44].active && Main.tile[num43, num44].type == 60 && Main.tile[num43, num44 - 1].active && Main.tile[num43, num44 - 1].type == 69 && Main.tile[num43, num44 - 1].liquid == 0)
													{
														flag7 = true;
														break;
													}
												}
											}
											if (flag7)
											{
												Main.tile[num34, num35].type = 69;
												Main.tile[num34, num35].active = true;
												WorldGen.SquareTileFrame(num34, num35, true);
												if (Main.netMode == 2)
												{
													NetMessage.SendTileSquare(-1, num34, num35, 3);
												}
											}
										}
									}
								}
							}
							if (Main.tile[num23, num24].type == 70)
							{
								int type2 = (int)Main.tile[num23, num24].type;
								if (!Main.tile[num23, num27].active && WorldGen.genRand.Next(10) == 0)
								{
									WorldGen.PlaceTile(num23, num27, 71, true, false, -1);
									if (Main.netMode == 2 && Main.tile[num23, num27].active)
									{
										NetMessage.SendTileSquare(-1, num23, num27, 1);
									}
								}
								bool flag8 = false;
								for (int num45 = num25; num45 < num26; num45++)
								{
									for (int num46 = num27; num46 < num28; num46++)
									{
										if ((num23 != num45 || num24 != num46) && Main.tile[num45, num46].active && Main.tile[num45, num46].type == 59)
										{
											WorldGen.SpreadGrass(num45, num46, 59, type2, false);
											if ((int)Main.tile[num45, num46].type == type2)
											{
												WorldGen.SquareTileFrame(num45, num46, true);
												flag8 = true;
											}
										}
									}
								}
								if (Main.netMode == 2 && flag8)
								{
									NetMessage.SendTileSquare(-1, num23, num24, 3);
								}
							}
						}
						else
						{
							if (flag && WorldGen.spawnNPC > 0)
							{
								WorldGen.SpawnNPC(num23, num24);
							}
						}
					}
				}
				num22++;
			}
			if (!Main.dayTime)
			{
				float num47 = (float)(Main.maxTilesX / 4200);
				if ((float)Main.rand.Next(8000) < 10f * num47)
				{
					int num48 = 12;
					int num49 = Main.rand.Next(Main.maxTilesX - 50) + 100;
					num49 *= 16;
					int num50 = Main.rand.Next((int)((double)Main.maxTilesY * 0.05));
					num50 *= 16;
					Vector2 vector = new Vector2((float)num49, (float)num50);
					float num51 = (float)Main.rand.Next(-100, 101);
					float num52 = (float)(Main.rand.Next(200) + 100);
					float num53 = (float)Math.Sqrt((double)(num51 * num51 + num52 * num52));
					num53 = (float)num48 / num53;
					num51 *= num53;
					num52 *= num53;
					Projectile.NewProjectile(vector.X, vector.Y, num51, num52, 12, 1000, 10f, Main.myPlayer);
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
						if (Main.tile[i, j].type == 23 && Main.tile[i, j].active && !Main.tile[i, j - 1].active)
						{
							WorldGen.PlaceTile(i, j - 1, 24, true, false, -1);
						}
					}
				}
			}
		}
		public static void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true)
		{
			if ((int)Main.tile[i, j].type != dirt || !Main.tile[i, j].active || ((double)j >= Main.worldSurface && dirt != 59))
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
					if (!Main.tile[k, l].active || !Main.tileSolid[(int)Main.tile[k, l].type])
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				if (grass == 23 && Main.tile[i, j - 1].type == 27)
				{
					return;
				}
				Main.tile[i, j].type = (byte)grass;
				for (int m = num; m < num2; m++)
				{
					for (int n = num3; n < num4; n++)
					{
						if (Main.tile[m, n].active && (int)Main.tile[m, n].type == dirt && repeat)
						{
							WorldGen.SpreadGrass(m, n, dirt, grass, true);
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
            Vector2 value = new Vector2();
			value.X = (float)i;
			value.Y = (float)j;
            Vector2 value2 = new Vector2();
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
				for (int m = num4; m < num5; m++)
				{
					for (int n = num6; n < num7; n++)
					{
						if ((double)(Math.Abs((float)m - value.X) + Math.Abs((float)n - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
						{
							if (Main.tile[m, n].type != 25 && n > j + WorldGen.genRand.Next(3, 20))
							{
								Main.tile[m, n].active = true;
							}
							if (steps <= num2)
							{
								Main.tile[m, n].active = true;
							}
							if (Main.tile[m, n].type != 31)
							{
								Main.tile[m, n].type = 25;
							}
							if (Main.tile[m, n].wall == 2)
							{
								Main.tile[m, n].wall = 0;
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
							if (Main.tile[num11, num12].type != 31)
							{
								Main.tile[num11, num12].type = 25;
							}
							if (steps <= num2)
							{
								Main.tile[num11, num12].active = true;
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
		public static void TileRunner(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool noYChange = false, bool overRide = true)
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
								if (type == -2 && Main.tile[k, l].active && (l < WorldGen.waterLine || l > WorldGen.lavaLine))
								{
									Main.tile[k, l].liquid = 255;
									if (l > WorldGen.lavaLine)
									{
										Main.tile[k, l].lava = true;
									}
								}
								Main.tile[k, l].active = false;
							}
							else
							{
								if ((overRide || !Main.tile[k, l].active) && (type != 40 || Main.tile[k, l].type != 53) && (!Main.tileStone[type] || Main.tile[k, l].type == 1) && Main.tile[k, l].type != 45)
								{
									Main.tile[k, l].type = (byte)type;
								}
								if (addTile)
								{
									Main.tile[k, l].active = true;
									Main.tile[k, l].liquid = 0;
									Main.tile[k, l].lava = false;
								}
								if (noYChange && (double)l < Main.worldSurface)
								{
									Main.tile[k, l].wall = 2;
								}
								if (type == 59 && l > WorldGen.waterLine && Main.tile[k, l].liquid > 0)
								{
									Main.tile[k, l].lava = false;
									Main.tile[k, l].liquid = 0;
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
								Main.tile[m, n].wall = 2;
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
			for (int l = num4; l <= num5; l++)
			{
				for (int m = num6; m < num7; m++)
				{
					Main.tile[l, m].active = true;
					Main.tile[l, m].type = type;
					Main.tile[l, m].wall = 0;
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
					if (Main.tile[n, num8].wall == 0)
					{
						Main.tile[n, num8].active = false;
						Main.tile[n, num8].wall = wall;
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
						if (num10 < num7 * 0.4 && !Main.tile[k, l].active)
						{
							Main.tile[k, l].active = true;
							Main.tile[k, l].type = 0;
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
            Vector2 value = new Vector2();
			value.X = (float)i;
			value.Y = (float)j - num3 * 0.3f;
            Vector2 value2 = new Vector2();
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
            Vector2 value = new Vector2();
			value.X = (float)i;
			value.Y = (float)j - num3 * 0.3f;
            Vector2 value2 = new Vector2();
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
            Vector2 value = new Vector2();
			value.X = (float)i;
			value.Y = (float)j;
			int k = WorldGen.genRand.Next(20, 40);
            Vector2 value2 = new Vector2();
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
            Vector2 value = new Vector2();
			value.X = (float)i;
			value.Y = (float)j;
			int k = 100;
            Vector2 value2 = new Vector2();
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
								if (Main.tile[i, j].lava && Main.tileLavaDeath[(int)Main.tile[i, j].type])
								{
									WorldGen.KillTile(i, j, false, false, false);
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
			int arg_19_0 = i - 1;
			int arg_23_0 = i + 1;
			int arg_22_0 = Main.maxTilesX;
			int arg_29_0 = j - 1;
			if (j + 1 >= Main.maxTilesY)
			{
				num = type;
			}
			if (i - 1 >= 0 && Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
			{
				byte arg_74_0 = Main.tile[i - 1, j].type;
			}
			if (i + 1 < Main.maxTilesX && Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
			{
				byte arg_B7_0 = Main.tile[i + 1, j].type;
			}
			if (j - 1 >= 0 && Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
			{
				byte arg_F6_0 = Main.tile[i, j - 1].type;
			}
			if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
			{
				num = (int)Main.tile[i, j + 1].type;
			}
			if (i - 1 >= 0 && j - 1 >= 0 && Main.tile[i - 1, j - 1] != null && Main.tile[i - 1, j - 1].active)
			{
				byte arg_184_0 = Main.tile[i - 1, j - 1].type;
			}
			if (i + 1 < Main.maxTilesX && j - 1 >= 0 && Main.tile[i + 1, j - 1] != null && Main.tile[i + 1, j - 1].active)
			{
				byte arg_1D3_0 = Main.tile[i + 1, j - 1].type;
			}
			if (i - 1 >= 0 && j + 1 < Main.maxTilesY && Main.tile[i - 1, j + 1] != null && Main.tile[i - 1, j + 1].active)
			{
				byte arg_222_0 = Main.tile[i - 1, j + 1].type;
			}
			if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY && Main.tile[i + 1, j + 1] != null && Main.tile[i + 1, j + 1].active)
			{
				byte arg_275_0 = Main.tile[i + 1, j + 1].type;
			}
			if ((type == 3 && num != 2 && num != 78) || (type == 24 && num != 23) || (type == 61 && num != 60) || (type == 71 && num != 70) || (type == 73 && num != 2 && num != 78) || (type == 74 && num != 60))
			{
				WorldGen.KillTile(i, j, false, false, false);
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
				if (wall == 0)
				{
					return;
				}
				byte arg_89_0 = Main.tile[i, j].wallFrameX;
				byte arg_9B_0 = Main.tile[i, j].wallFrameY;
				Rectangle rectangle = new Rectangle();
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
					if (noBreak && Main.tileFrameImportant[(int)Main.tile[i, j].type])
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
					int num9 = (int)Main.tile[i, j].type;
					if (Main.tileStone[num9])
					{
						num9 = 1;
					}
					int frameX = (int)Main.tile[i, j].frameX;
					int frameY = (int)Main.tile[i, j].frameY;
                    Rectangle rectangle = new Rectangle();
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
							return;
						}
						if ((num4 >= 0 && Main.tileSolid[num4] && !Main.tileNoAttach[num4]) || (num4 == 5 && num == 5 && num6 == 5))
						{
							Main.tile[i, j].frameX = 22;
							return;
						}
						if ((num5 >= 0 && Main.tileSolid[num5] && !Main.tileNoAttach[num5]) || (num5 == 5 && num3 == 5 && num8 == 5))
						{
							Main.tile[i, j].frameX = 44;
							return;
						}
						WorldGen.KillTile(i, j, false, false, false);
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
								if (Main.tile[num10, num11] != null && Main.tile[num10 + 1, num11] != null && Main.tile[num10, num11 + 1] != null && Main.tile[num10 + 1, num11 + 1] != null && (!Main.tile[num10, num11].active || (int)Main.tile[num10, num11].type != num9 || !Main.tile[num10 + 1, num11].active || (int)Main.tile[num10 + 1, num11].type != num9 || !Main.tile[num10, num11 + 1].active || (int)Main.tile[num10, num11 + 1].type != num9 || !Main.tile[num10 + 1, num11 + 1].active || (int)Main.tile[num10 + 1, num11 + 1].type != num9))
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
									if (Main.tile[i, num17 - 1] == null)
									{
										Main.tile[i, num17 - 1] = new Tile();
									}
									if (Main.tile[i, num17 + 3] == null)
									{
										Main.tile[i, num17 + 3] = new Tile();
									}
									if (Main.tile[i, num17 + 2] == null)
									{
										Main.tile[i, num17 + 2] = new Tile();
									}
									if (Main.tile[i, num17 + 1] == null)
									{
										Main.tile[i, num17 + 1] = new Tile();
									}
									if (Main.tile[i, num17] == null)
									{
										Main.tile[i, num17] = new Tile();
									}
									if (!Main.tile[i, num17 - 1].active || !Main.tileSolid[(int)Main.tile[i, num17 - 1].type])
									{
										flag = true;
									}
									if (!Main.tile[i, num17 + 3].active || !Main.tileSolid[(int)Main.tile[i, num17 + 3].type])
									{
										flag = true;
									}
									if (!Main.tile[i, num17].active || (int)Main.tile[i, num17].type != num9)
									{
										flag = true;
									}
									if (!Main.tile[i, num17 + 1].active || (int)Main.tile[i, num17 + 1].type != num9)
									{
										flag = true;
									}
									if (!Main.tile[i, num17 + 2].active || (int)Main.tile[i, num17 + 2].type != num9)
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
									int frameX2 = (int)Main.tile[i, j].frameX;
									int frameY3 = (int)Main.tile[i, j].frameY;
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
									if (Main.tile[num19, num20 + 3] == null)
									{
										Main.tile[num19, num20 + 3] = new Tile();
									}
									if (Main.tile[num19, num20 - 1] == null)
									{
										Main.tile[num19, num20 - 1] = new Tile();
									}
									if (!Main.tile[num19, num20 - 1].active || !Main.tileSolid[(int)Main.tile[num19, num20 - 1].type] || !Main.tile[num19, num20 + 3].active || !Main.tileSolid[(int)Main.tile[num19, num20 + 3].type])
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
											if (!flag2 && (Main.tile[l, m].type != 11 || !Main.tile[l, m].active))
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
							if (Main.tile[i, j].frameY >= 132 && Main.tile[i, j].frameY <= 176 && (Main.tile[i, j].frameX == 0 || Main.tile[i, j].frameX == 66 || Main.tile[i, j].frameX == 88))
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
									if (num2 != num9 && Main.tile[i, j].frameY < 198 && ((Main.tile[i, j].frameX != 22 && Main.tile[i, j].frameX != 44) || Main.tile[i, j].frameY < 132))
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
							rectangle.X = (int)Main.tile[i, j].frameX;
							rectangle.Y = (int)Main.tile[i, j].frameY;
						}
						if (Main.tileFrameImportant[(int)Main.tile[i, j].type])
						{
							return;
						}
						int num22 = 0;
						if (resetFrame)
						{
							num22 = WorldGen.genRand.Next(0, 3);
							Main.tile[i, j].frameNumber = (byte)num22;
						}
						else
						{
							num22 = (int)Main.tile[i, j].frameNumber;
						}
						if (num9 == 0)
						{
							for (int n = 0; n < 80; n++)
							{
								if (n == 1 || n == 6 || n == 7 || n == 8 || n == 9 || n == 22 || n == 25 || n == 37 || n == 40 || n == 53 || n == 56)
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
								else
								{
									if (num9 == 1)
									{
										if (num2 == 59)
										{
											WorldGen.TileFrame(i, j - 1, false, false);
											if (WorldGen.mergeDown)
											{
												num2 = num9;
											}
										}
										if (num7 == 59)
										{
											WorldGen.TileFrame(i, j + 1, false, false);
											if (WorldGen.mergeUp)
											{
												num7 = num9;
											}
										}
										if (num4 == 59)
										{
											WorldGen.TileFrame(i - 1, j, false, false);
											if (WorldGen.mergeRight)
											{
												num4 = num9;
											}
										}
										if (num5 == 59)
										{
											WorldGen.TileFrame(i + 1, j, false, false);
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
								if (Main.tile[i, j + 1] != null && !Main.tile[i, j + 1].active)
								{
									bool flag3 = true;
									if (Main.tile[i, j - 1].active && Main.tile[i, j - 1].type == 21)
									{
										flag3 = false;
									}
									if (flag3)
									{
										Main.tile[i, j].active = false;
										Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0.41f, 31, 10, 0f, Main.myPlayer);
										WorldGen.SquareTileFrame(i, j, true);
									}
								}
							}
							else
							{
								if (Main.netMode == 2 && Main.tile[i, j + 1] != null && !Main.tile[i, j + 1].active)
								{
									bool flag4 = true;
									if (Main.tile[i, j - 1].active && Main.tile[i, j - 1].type == 21)
									{
										flag4 = false;
									}
									if (flag4)
									{
										Main.tile[i, j].active = false;
										int num25 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0.41f, 31, 10, 0f, Main.myPlayer);
										Main.projectile[num25].velocity.Y = 0.5f;
										Projectile expr_6540_cp_0 = Main.projectile[num25];
										expr_6540_cp_0.position.Y = expr_6540_cp_0.position.Y + 2f;
										NetMessage.SendTileSquare(-1, i, j, 1);
										WorldGen.SquareTileFrame(i, j, true);
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
