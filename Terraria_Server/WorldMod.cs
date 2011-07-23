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
using Terraria_Server.Logging;
using Terraria_Server.World;

namespace Terraria_Server
{
	internal class WorldMod
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
		public static int lastMaxTilesX = 0;
		public static int lastMaxTilesY = 0;
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
		public static int[] roomX = new int[maxRoomTiles];
		public static int[] roomY = new int[maxRoomTiles];
		public static int roomX1;
		public static int roomX2;
		public static int roomY1;
		public static int roomY2;
		public static bool canSpawn;
		public static bool[] houseTile = new bool[86];
		public static int bestX = 0;
		public static int bestY = 0;
		public static int hiScore = 0;
		public static int ficount;

		public static void SpawnNPC(int x, int y)
		{
			if (Main.wallHouse[(int)Main.tile.At(x, y).Wall])
			{
				WorldMod.canSpawn = true;
			}
			if (!WorldMod.canSpawn)
			{
				return;
			}
			if (!WorldMod.StartRoomCheck(x, y))
			{
				return;
			}
			if (!WorldMod.RoomNeeds(WorldMod.spawnNPC))
			{
				return;
			}
			WorldMod.ScoreRoom(-1);
			if (WorldMod.hiScore > 0)
			{
				int num = -1;
				for (int i = 0; i < 1000; i++)
				{
					if (Main.npcs[i].Active && Main.npcs[i].homeless && Main.npcs[i].Type == WorldMod.spawnNPC)
					{
						num = i;
						break;
					}
				}
				if (num == -1)
				{
					int num2 = WorldMod.bestX;
					int num3 = WorldMod.bestY;
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
									num2 = WorldMod.bestX + k;
								}
								else
								{
									num2 = WorldMod.bestX - k;
								}
								if (num2 > 10 && num2 < Main.maxTilesX - 10)
								{
									int num4 = WorldMod.bestY - k;
									double num5 = (double)(WorldMod.bestY + k);
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
					int num7 = NPC.NewNPC(num2 * 16, num3 * 16, WorldMod.spawnNPC, 1);
					Main.npcs[num7].homeTileX = WorldMod.bestX;
					Main.npcs[num7].homeTileY = WorldMod.bestY;
					if (num2 < WorldMod.bestX)
					{
						Main.npcs[num7].direction = 1;
					}
					else
					{
						if (num2 > WorldMod.bestX)
						{
							Main.npcs[num7].direction = -1;
						}
					}
					Main.npcs[num7].netUpdate = true;
					
					NetMessage.SendData(25, -1, -1, Main.npcs[num7].Name + " has arrived!", 255, 50f, 125f, 255f);
				}
				else
				{
					WorldMod.spawnNPC = 0;
					Main.npcs[num].homeTileX = WorldMod.bestX;
					Main.npcs[num].homeTileY = WorldMod.bestY;
					Main.npcs[num].homeless = false;
				}
				WorldMod.spawnNPC = 0;
			}
		}
		
		public static bool RoomNeeds(int npcType)
		{
			if (WorldMod.houseTile[15] && (WorldMod.houseTile[14] || WorldMod.houseTile[18]) && (WorldMod.houseTile[4] || WorldMod.houseTile[33] || WorldMod.houseTile[34] || WorldMod.houseTile[35] || WorldMod.houseTile[36] || WorldMod.houseTile[42] || WorldMod.houseTile[49]) && (WorldMod.houseTile[10] || WorldMod.houseTile[11] || WorldMod.houseTile[19]))
			{
				WorldMod.canSpawn = true;
			}
			else
			{
				WorldMod.canSpawn = false;
			}
			return WorldMod.canSpawn;
		}
		
		public static void QuickFindHome(int npc)
		{
			if (Main.npcs[npc].homeTileX > 10 && Main.npcs[npc].homeTileY > 10 && Main.npcs[npc].homeTileX < Main.maxTilesX - 10 && Main.npcs[npc].homeTileY < Main.maxTilesY)
			{
				WorldMod.canSpawn = false;
				WorldMod.StartRoomCheck(Main.npcs[npc].homeTileX, Main.npcs[npc].homeTileY - 1);
				if (!WorldMod.canSpawn)
				{
					for (int i = Main.npcs[npc].homeTileX - 1; i < Main.npcs[npc].homeTileX + 2; i++)
					{
						int num = Main.npcs[npc].homeTileY - 1;
						while (num < Main.npcs[npc].homeTileY + 2 && !WorldMod.StartRoomCheck(i, num))
						{
							num++;
						}
					}
				}
				if (!WorldMod.canSpawn)
				{
					int num2 = 10;
					for (int j = Main.npcs[npc].homeTileX - num2; j <= Main.npcs[npc].homeTileX + num2; j += 2)
					{
						int num3 = Main.npcs[npc].homeTileY - num2;
						while (num3 <= Main.npcs[npc].homeTileY + num2 && !WorldMod.StartRoomCheck(j, num3))
						{
							num3 += 2;
						}
					}
				}
				if (WorldMod.canSpawn)
				{
					WorldMod.RoomNeeds(Main.npcs[npc].Type);
					if (WorldMod.canSpawn)
					{
						WorldMod.ScoreRoom(npc);
					}
					if (WorldMod.canSpawn && WorldMod.hiScore > 0)
					{
						Main.npcs[npc].homeTileX = WorldMod.bestX;
						Main.npcs[npc].homeTileY = WorldMod.bestY;
						Main.npcs[npc].homeless = false;
						WorldMod.canSpawn = false;
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
					for (int j = 0; j < WorldMod.numRoomTiles; j++)
					{
						if (Main.npcs[i].homeTileX == WorldMod.roomX[j] && Main.npcs[i].homeTileY == WorldMod.roomY[j])
						{
							bool flag = false;
							for (int k = 0; k < WorldMod.numRoomTiles; k++)
							{
								if (Main.npcs[i].homeTileX == WorldMod.roomX[k] && Main.npcs[i].homeTileY - 1 == WorldMod.roomY[k])
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								WorldMod.hiScore = -1;
								return;
							}
						}
					}
				}
			}
			WorldMod.hiScore = 0;
			int num = 0;
			int num2 = 0;
			int num3 = WorldMod.roomX1 - NPC.sWidth / 2 / 16 - 1 - 21;
			int num4 = WorldMod.roomX2 + NPC.sWidth / 2 / 16 + 1 + 21;
			int num5 = WorldMod.roomY1 - NPC.sHeight / 2 / 16 - 1 - 21;
			int num6 = WorldMod.roomY2 + NPC.sHeight / 2 / 16 + 1 + 21;
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
				WorldMod.hiScore = num7;
				return;
			}
			num3 = WorldMod.roomX1;
			num4 = WorldMod.roomX2;
			num5 = WorldMod.roomY1;
			num6 = WorldMod.roomY2;
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
							if (num > WorldMod.hiScore)
							{
								bool flag2 = false;
								for (int num11 = 0; num11 < WorldMod.numRoomTiles; num11++)
								{
									if (WorldMod.roomX[num11] == n && WorldMod.roomY[num11] == num8)
									{
										flag2 = true;
										break;
									}
								}
								if (flag2)
								{
									WorldMod.hiScore = num;
									WorldMod.bestX = n;
									WorldMod.bestY = num8;
								}
							}
						}
					}
				}
			}
		}
		
		public static bool StartRoomCheck(int x, int y)
		{
			WorldMod.roomX1 = x;
			WorldMod.roomX2 = x;
			WorldMod.roomY1 = y;
			WorldMod.roomY2 = y;
			WorldMod.numRoomTiles = 0;
			for (int i = 0; i < 86; i++)
			{
				WorldMod.houseTile[i] = false;
			}
			WorldMod.canSpawn = true;
			if (Main.tile.At(x, y).Active && Main.tileSolid[(int)Main.tile.At(x, y).Type])
			{
				WorldMod.canSpawn = false;
			}
			WorldMod.CheckRoom(x, y);
			if (WorldMod.numRoomTiles < 60)
			{
				WorldMod.canSpawn = false;
			}
			return WorldMod.canSpawn;
		}
		
		public static void CheckRoom(int x, int y)
		{
			if (!WorldMod.canSpawn)
			{
				return;
			}
			if (x < 10 || y < 10 || x >= Main.maxTilesX - 10 || y >= WorldMod.lastMaxTilesY - 10)
			{
				WorldMod.canSpawn = false;
				return;
			}
			for (int i = 0; i < WorldMod.numRoomTiles; i++)
			{
				if (WorldMod.roomX[i] == x && WorldMod.roomY[i] == y)
				{
					return;
				}
			}
			WorldMod.roomX[WorldMod.numRoomTiles] = x;
			WorldMod.roomY[WorldMod.numRoomTiles] = y;
			WorldMod.numRoomTiles++;
			if (WorldMod.numRoomTiles >= WorldMod.maxRoomTiles)
			{
				WorldMod.canSpawn = false;
				return;
			}
			if (Main.tile.At(x, y).Active)
			{
				WorldMod.houseTile[(int)Main.tile.At(x, y).Type] = true;
				if (Main.tileSolid[(int)Main.tile.At(x, y).Type] || Main.tile.At(x, y).Type == 11)
				{
					return;
				}
			}
			if (x < WorldMod.roomX1)
			{
				WorldMod.roomX1 = x;
			}
			if (x > WorldMod.roomX2)
			{
				WorldMod.roomX2 = x;
			}
			if (y < WorldMod.roomY1)
			{
				WorldMod.roomY1 = y;
			}
			if (y > WorldMod.roomY2)
			{
				WorldMod.roomY2 = y;
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
				WorldMod.canSpawn = false;
				return;
			}
			for (int k = x - 1; k < x + 2; k++)
			{
				for (int l = y - 1; l < y + 2; l++)
				{
					if ((k != x || l != y) && WorldMod.canSpawn)
					{
						WorldMod.CheckRoom(k, l);
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
						flag = WorldMod.meteor(num7, k);
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

			WorldMod.stopDrops = true;
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
						WorldMod.KillTile(num6, num7, false, false, false);
					}
					WorldMod.SquareTileFrame(num6, num7, true);
					WorldMod.SquareWallFrame(num6, num7, true);
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
							WorldMod.KillTile(num8, num9, false, false, false);
						}
						Main.tile.At(num8, num9).SetType (37);
						WorldMod.SquareTileFrame(num8, num9, true);
					}
				}
			}
			WorldMod.stopDrops = false;
			
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
			WorldMod.saveWorld(Program.server.World.SavePath, false);
		}
		
		public static void saveAndPlay()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldMod.saveAndPlayCallBack), 1);
		}
		
		public static void saveToonWhilePlayingCallBack(object threadContext)
		{
			Player.SavePlayer(Main.players[Main.myPlayer]);
		}
		
		public static void saveToonWhilePlaying()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldMod.saveToonWhilePlayingCallBack), 1);
		}
		
		public static void clearWorld()
		{
			WorldMod.spawnEye = false;
			WorldMod.spawnNPC = 0;
			WorldMod.shadowOrbCount = 0;
			Main.helpText = 0;
			Main.dungeonX = 0;
			Main.dungeonY = 0;
			NPC.downedBoss1 = false;
			NPC.downedBoss2 = false;
			NPC.downedBoss3 = false;
			WorldMod.shadowOrbSmashed = false;
			WorldMod.spawnMeteor = false;
			WorldMod.stopDrops = false;
			Main.invasionDelay = 0;
			Main.invasionType = 0;
			Main.invasionSize = 0;
			Main.invasionWarn = 0;
			Main.invasionX = 0.0;
			WorldMod.noLiquidCheck = false;
			Liquid.numLiquid = 0;
			LiquidBuffer.numLiquidBuffer = 0;
			
			if (WorldMod.lastMaxTilesX > Main.maxTilesX || WorldMod.lastMaxTilesY > Main.maxTilesY)
			{
				using (var prog = new ProgressLogger (WorldMod.lastMaxTilesX - 1, "Freeing unused resources"))
					for (int i = 0; i < WorldMod.lastMaxTilesX; i++)
					{
						prog.Value = i;
						for (int j = 0; j < WorldMod.lastMaxTilesY; j++)
						{
							Main.tile.CreateTileAt (i, j);
						}
					}
			}
			WorldMod.lastMaxTilesX = Main.maxTilesX;
			WorldMod.lastMaxTilesY = Main.maxTilesY;

			Server.tile = new TileCollection(Main.maxTilesX, Main.maxTilesY);
			
			using (var prog = new ProgressLogger (Main.maxTilesX - 1, "Resetting game objects"))
				for (int k = 0; k < Main.maxTilesX; k++)
				{
					prog.Value = k;
					for (int l = 0; l < Main.maxTilesY; l++)
					{
						Main.tile.CreateTileAt (k, l);
					}
				}
			
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
			WorldMod.setWorldSize();
			WorldMod.worldCleared = true;
		}
		
		public static void saveWorld(String savePath, bool resetTime = false)
		{
			if (savePath == null)
			{
				return;
			}

			if (WorldMod.saveLock)
			{
				return;
			}

			try
			{
				WorldMod.saveLock = true;
				lock (WorldMod.padlock)
				{
					bool value = Main.dayTime;
					WorldMod.tempTime = Main.time;
					WorldMod.tempMoonPhase = Main.moonPhase;
					WorldMod.tempBloodMoon = Main.bloodMoon;

					if (resetTime)
					{
						value = true;
						WorldMod.tempTime = 13500.0;
						WorldMod.tempMoonPhase = 0;
						WorldMod.tempBloodMoon = false;
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
							binaryWriter.Write(WorldMod.tempTime);
							binaryWriter.Write(value);
							binaryWriter.Write(WorldMod.tempMoonPhase);
							binaryWriter.Write(WorldMod.tempBloodMoon);
							binaryWriter.Write(Main.dungeonX);
							binaryWriter.Write(Main.dungeonY);
							binaryWriter.Write(NPC.downedBoss1);
							binaryWriter.Write(NPC.downedBoss2);
							binaryWriter.Write(NPC.downedBoss3);
							binaryWriter.Write(WorldMod.shadowOrbSmashed);
							binaryWriter.Write(WorldMod.spawnMeteor);
							binaryWriter.Write((byte)WorldMod.shadowOrbCount);
							binaryWriter.Write(Main.invasionDelay);
							binaryWriter.Write(Main.invasionSize);
							binaryWriter.Write(Main.invasionType);
							binaryWriter.Write(Main.invasionX);

							using (var prog = new ProgressLogger (Main.maxTilesX - 1, "Saving world data"))
							{
								for (int x = 0; x < Main.maxTilesX; x++)
								{
									prog.Value = x;
									
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

							if (File.Exists(savePath))
							{
								ProgramLog.Log ("Backing up world file...");
								String destFileName = savePath + ".bak";
								File.Copy(savePath, destFileName, true);
								try
								{
									File.Delete(destFileName);
								}
								catch (Exception e)
								{
									ProgramLog.Log (e, "Exception removing " + destFileName);
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
							ProgramLog.Log (e, "Exception moving " + tempPath);
						}

						try
						{
							File.Delete(tempPath);
						}
						catch (Exception e)
						{
							ProgramLog.Log (e, "Exception removing " + tempPath);
						}
					}
					stopwatch.Stop();
					ProgramLog.Log ("Save duration: " + stopwatch.Elapsed.Seconds + " Second(s)");
					WorldMod.saveLock = false;
				}
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception saving the world");
			}
		}
		
		public static void loadWorld()
		{
			if (WorldMod.genRand == null)
			{
				WorldMod.genRand = new Random((int)DateTime.Now.Ticks);
			}
			using (FileStream fileStream = new FileStream(Program.server.World.SavePath, FileMode.Open))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					try
					{
						WorldMod.loadFailed = false;
						WorldMod.loadSuccess = false;
						int Terraria_Release = binaryReader.ReadInt32();
						if (Terraria_Release > Statics.CURRENT_TERRARIA_RELEASE)
						{
							WorldMod.loadFailed = true;
							WorldMod.loadSuccess = false;
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
							Main.maxSectionsX = Main.maxTilesX / 200;
							Main.maxSectionsY = Main.maxTilesY / 150;
							WorldMod.clearWorld();
							Main.spawnTileX = binaryReader.ReadInt32();
							Main.spawnTileY = binaryReader.ReadInt32();
							Main.worldSurface = binaryReader.ReadDouble();
							Main.rockLayer = binaryReader.ReadDouble();
							WorldMod.tempTime = binaryReader.ReadDouble();
							WorldMod.tempDayTime = binaryReader.ReadBoolean();
							WorldMod.tempMoonPhase = binaryReader.ReadInt32();
							WorldMod.tempBloodMoon = binaryReader.ReadBoolean();
							Main.dungeonX = binaryReader.ReadInt32();
							Main.dungeonY = binaryReader.ReadInt32();
							NPC.downedBoss1 = binaryReader.ReadBoolean();
							NPC.downedBoss2 = binaryReader.ReadBoolean();
							NPC.downedBoss3 = binaryReader.ReadBoolean();
							WorldMod.shadowOrbSmashed = binaryReader.ReadBoolean();
							WorldMod.spawnMeteor = binaryReader.ReadBoolean();
							WorldMod.shadowOrbCount = (int)binaryReader.ReadByte();
							Main.invasionDelay = binaryReader.ReadInt32();
							Main.invasionSize = binaryReader.ReadInt32();
							Main.invasionType = binaryReader.ReadInt32();
							Main.invasionX = binaryReader.ReadDouble();
							
							using (var prog = new ProgressLogger (Main.maxTilesX - 1, "Loading world tiles"))
							{
								for (int j = 0; j < Main.maxTilesX; j++)
								{
									prog.Value = j;
									
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
											String defaults = Item.VersionName(binaryReader.ReadString(), Terraria_Release);
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
							if (Terraria_Release >= 7)
							{
								bool flag2 = binaryReader.ReadBoolean();
								String a = binaryReader.ReadString();
								int num6 = binaryReader.ReadInt32();
								if (!flag2 || !(a == Main.worldName) || num6 != Main.worldID)
								{
									WorldMod.loadSuccess = false;
									WorldMod.loadFailed = true;
									binaryReader.Close();
									fileStream.Close();
									return;
								}
								WorldMod.loadSuccess = true;
							}
							else
							{
								WorldMod.loadSuccess = true;
							}
							binaryReader.Close();
							fileStream.Close();

							if (!WorldMod.loadFailed && WorldMod.loadSuccess)
							{
								WorldMod.gen = true;
								WorldMod.waterLine = Main.maxTilesY;
								Liquid.QuickWater(2, -1, -1);
								WorldMod.WaterCheck();
								int num7 = 0;
								Liquid.quickSettle = true;
								int num8 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
								float num9 = 0f;
								
								using (var prog = new ProgressLogger (100, "Settling liquids"))
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
										
										prog.Value = (int) (num10 * 50f + 50f);
										
										Liquid.UpdateLiquid();
									}
								Liquid.quickSettle = false;

								ProgramLog.Log ("Performing Water Check");
								WorldMod.WaterCheck();
								WorldMod.gen = false;
							}
						}
					}
					catch
					{
						WorldMod.loadFailed = true;
						WorldMod.loadSuccess = false;
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

		public static void GrowTreeShared(int i, int y, int freeTilesAbove, int minHeight, int maxHeight)
		{
			bool flag = false;
			bool flag2 = false;
			int num3 = WorldMod.genRand.Next(minHeight, maxHeight);
			int num4;
			for (int j = freeTilesAbove - num3; j < freeTilesAbove; j++)
			{
				Main.tile.At(i, j).SetFrameNumber ((byte)WorldMod.genRand.Next(3));
				Main.tile.At(i, j).SetActive (true);
				Main.tile.At(i, j).SetType (5);
				num4 = WorldMod.genRand.Next(3);
				int num5 = WorldMod.genRand.Next(10);
				if (j == freeTilesAbove - 1 || j == freeTilesAbove - num3)
				{
					num5 = 0;
				}
				while (((num5 == 5 || num5 == 7) && flag) || ((num5 == 6 || num5 == 7) && flag2))
				{
					num5 = WorldMod.genRand.Next(10);
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
					num4 = WorldMod.genRand.Next(3);
					if (WorldMod.genRand.Next(3) < 2)
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
					num4 = WorldMod.genRand.Next(3);
					if (WorldMod.genRand.Next(3) < 2)
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
			int num6 = WorldMod.genRand.Next(3);
			if (num6 == 0 || num6 == 1)
			{
				Main.tile.At(i + 1, freeTilesAbove - 1).SetActive (true);
				Main.tile.At(i + 1, freeTilesAbove - 1).SetType (5);
				num4 = WorldMod.genRand.Next(3);
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
				num4 = WorldMod.genRand.Next(3);
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
			num4 = WorldMod.genRand.Next(3);
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
			if (WorldMod.genRand.Next(3) < 2)
			{
				num4 = WorldMod.genRand.Next(3);
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
				num4 = WorldMod.genRand.Next(3);
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
			WorldMod.RangeFrame(i - 2, freeTilesAbove - num3 - 1, i + 2, freeTilesAbove + 1);
			
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
				if (WorldMod.EmptyTileCheck(x - TREE_RADIUS, x + TREE_RADIUS, freeTilesAbove - 14, freeTilesAbove - 1, 20))
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
			if (Main.tile.At(i, y).Active && Main.tile.At(i, y).Type == 70 && Main.tile.At(i, y - 1).Wall == 0 && Main.tile.At(i - 1, y).Active && Main.tile.At(i - 1, y).Type == 70 && Main.tile.At(i + 1, y).Active && Main.tile.At(i + 1, y).Type == 70 && WorldMod.EmptyTileCheck(i - 2, i + 2, y - 13, y - 1, 71))
			{
				int num = WorldMod.genRand.Next(4, 11);
				int num2;
				for (int j = y - num; j < y; j++)
				{
					Main.tile.At(i, j).SetFrameNumber ((byte)WorldMod.genRand.Next(3));
					Main.tile.At(i, j).SetActive (true);
					Main.tile.At(i, j).SetType (72);
					num2 = WorldMod.genRand.Next(3);
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
				num2 = WorldMod.genRand.Next(3);
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
				WorldMod.RangeFrame(i - 2, y - num - 1, i + 2, y + 1);
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
					Main.tile.At(i, j - 1).SetFrameX ((short)(WorldMod.genRand.Next(3) * 18));
					Main.tile.At(i, j).SetActive (true);
					Main.tile.At(i, j).SetType (10);
					Main.tile.At(i, j).SetFrameY (18);
					Main.tile.At(i, j).SetFrameX ((short)(WorldMod.genRand.Next(3) * 18));
					Main.tile.At(i, j + 1).SetActive (true);
					Main.tile.At(i, j + 1).SetType (10);
					Main.tile.At(i, j + 1).SetFrameY (36);
					Main.tile.At(i, j + 1).SetFrameX ((short)(WorldMod.genRand.Next(3) * 18));
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
				sender = new ConsoleSender ();
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
			else if (frameX == 18)
			{
				num2 = i - 1;
				num = 1;
			}
			else if (frameX == 36)
			{
				num2 = i + 1;
				num = -1;
			}
			else if (frameX == 54)
			{
				num2 = i;
				num = -1;
			}
			if (frameY == 0)
			{
				num3 = j;
			}
			else if (frameY == 18)
			{
				num3 = j - 1;
			}
			else if (frameY == 36)
			{
				num3 = j - 2;
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
						Main.tile.At(l, m).SetFrameX ((short)(WorldMod.genRand.Next(3) * 18));
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
					WorldMod.TileFrame(n, num5, false, false);
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
					WorldMod.KillTile(num4, k, false, false, false);
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
						WorldMod.TileFrame(l, m, false, false);
					}
				}
			}
			return flag;
		}
		
		public static void Check1x2(int x, int j, byte type)
		{
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				if (Main.tile.At(x, num).Type == type)
				{
					WorldMod.KillTile(x, num, false, false, false);
				}
				if (Main.tile.At(x, num + 1).Type == type)
				{
					WorldMod.KillTile(x, num + 1, false, false, false);
				}
				if (type == 15)
				{
					Item.NewItem(x * 16, num * 16, 32, 32, 34, 1, false);
				}
				WorldMod.destroyObject = false;
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
						WorldMod.KillTile(x, y, false, false, false);
						return;
					}
				}
				else
				{
					WorldMod.KillTile(x, y, false, false, false);
				}
			}
		}
		
		public static void CheckSign(int x, int y, int type)
		{
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				for (int n = num; n < num2; n++)
				{
					for (int num9 = num3; num9 < num4; num9++)
					{
						if ((int)Main.tile.At(n, num9).Type == type)
						{
							WorldMod.KillTile(n, num9, false, false, false);
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
				WorldMod.destroyObject = false;
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
					Main.tile.At(x, y).SetFrameX ((short)(18 * WorldMod.genRand.Next(5)));
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
				if (Main.tile.At(x, y).Type == 82 && WorldMod.genRand.Next(50) == 0)
				{
					Main.tile.At(x, y).SetType (83);
					
					NetMessage.SendTileSquare(-1, x, y, 1);
					WorldMod.SquareTileFrame(x, y, true);
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
			int num = WorldMod.genRand.Next(20, Main.maxTilesX - 20);
			int num2 = 0;
			if (WorldMod.genRand.Next(40) == 0)
			{
				num2 = WorldMod.genRand.Next((int)(Main.rockLayer + (double)Main.maxTilesY) / 2, Main.maxTilesY - 20);
			}
			else if (WorldMod.genRand.Next(10) == 0)
			{
				num2 = WorldMod.genRand.Next(0, Main.maxTilesY - 20);
			}
			else
			{
				num2 = WorldMod.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
			}
			while (num2 < Main.maxTilesY - 20 && !Main.tile.At(num, num2).Active)
			{
				num2++;
			}
			if (Main.tile.At(num, num2).Active && !Main.tile.At(num, num2 - 1).Active && Main.tile.At(num, num2 - 1).Liquid == 0)
			{
				if (Main.tile.At(num, num2).Type == 2)
				{
					WorldMod.PlaceAlch(num, num2 - 1, 0);
				}
				if (Main.tile.At(num, num2).Type == 60)
				{
					WorldMod.PlaceAlch(num, num2 - 1, 1);
				}
				if (Main.tile.At(num, num2).Type == 0 || Main.tile.At(num, num2).Type == 59)
				{
					WorldMod.PlaceAlch(num, num2 - 1, 2);
				}
				if (Main.tile.At(num, num2).Type == 23 || Main.tile.At(num, num2).Type == 25)
				{
					WorldMod.PlaceAlch(num, num2 - 1, 3);
				}
				if (Main.tile.At(num, num2).Type == 53)
				{
					WorldMod.PlaceAlch(num, num2 - 1, 4);
				}
				if (Main.tile.At(num, num2).Type == 57)
				{
					WorldMod.PlaceAlch(num, num2 - 1, 5);
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
				WorldMod.KillTile(x, y, false, false, false);
			}
		}
		
		public static void Place1x2(int x, int y, int type)
		{
			short frameX = 0;
			if (type == 20)
			{
				frameX = (short)(WorldMod.genRand.Next(3) * 18);
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
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				if (tiles[1, 1].Type == type)
				{
					WorldMod.KillTile(x, y, false, false, false);
				}
				if (tiles[1, 2].Type == type)
				{
					WorldMod.KillTile(x, y + 1, false, false, false);
				}
				if (type == 42)
				{
					Item.NewItem(x * 16, y * 16, 32, 32, 136, 1, false);
				}
				WorldMod.destroyObject = false;
			}
		}
		
		public static void Check2x1(int x, int y, byte type)
		{
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				if (tiles[1,1].Type == type)
				{
					WorldMod.KillTile(x, y, false, false, false);
				}
				if (tiles[2,1].Type == type)
				{
					WorldMod.KillTile(x + 1, y, false, false, false);
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
				WorldMod.destroyObject = false;
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
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				for (int m = num; m < num + 4; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							WorldMod.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 79)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 224, 1, false);
				}
				WorldMod.destroyObject = false;
				for (int num4 = num - 1; num4 < num + 4; num4++)
				{
					for (int num5 = num2 - 1; num5 < num2 + 4; num5++)
					{
						WorldMod.TileFrame(num4, num5, false, false);
					}
				}
			}
		}
		
		public static void Check2x2(int i, int j, int type)
		{
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							WorldMod.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 85)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 321, 1, false);
				}
				WorldMod.destroyObject = false;
				for (int num3 = num - 1; num3 < num + 3; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 3; num4++)
					{
						WorldMod.TileFrame(num3, num4, false, false);
					}
				}
			}
		}
		
		public static void Check3x2(int i, int j, int type)
		{
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							WorldMod.KillTile(m, n, false, false, false);
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
				WorldMod.destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						WorldMod.TileFrame(num3, num4, false, false);
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
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							WorldMod.KillTile(m, n, false, false, false);
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
				WorldMod.destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						WorldMod.TileFrame(num3, num4, false, false);
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
						int num = k * 18 + WorldMod.genRand.Next(3) * 36;
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
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 4; num3++)
					{
						if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
						{
							WorldMod.KillTile(num2, num3, false, false, false);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, 63, 1, false);
				WorldMod.destroyObject = false;
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
						int num = k * 18 + WorldMod.genRand.Next(3) * 36;
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
				WorldMod.KillTile(i, j, false, false, false);
				return true;
			}
			if (i != num2)
			{
				if ((!Main.tile.At(i, j + 1).Active || Main.tile.At(i, j + 1).Type != 80) && (!Main.tile.At(i - 1, j).Active || Main.tile.At(i - 1, j).Type != 80) && (!Main.tile.At(i + 1, j).Active || Main.tile.At(i + 1, j).Type != 80))
				{
					WorldMod.KillTile(i, j, false, false, false);
					return true;
				}
			}
			else if (i == num2 && (!Main.tile.At(i, j + 1).Active || (Main.tile.At(i, j + 1).Type != 80 && Main.tile.At(i, j + 1).Type != 53)))
			{
				WorldMod.KillTile(i, j, false, false, false);
				return true;
			}
			return false;
		}
		
		public static void PlantCactus(int i, int j)
		{
			WorldMod.GrowCactus(i, j);
			for (int k = 0; k < 150; k++)
			{
				int i2 = WorldMod.genRand.Next(i - 1, i + 2);
				int j2 = WorldMod.genRand.Next(j - 10, j + 2);
				WorldMod.GrowCactus(i2, j2);
			}
		}
		
		public static void CactusFrame(int i, int j)
		{
			try
			{
				int num = j;
				int num2 = i;
				if (!WorldMod.CheckCactus(i, j))
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
						WorldMod.SquareTileFrame(i, j, true);
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
					WorldMod.SquareTileFrame(num2, num - 1, true);
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
				if (num8 < WorldMod.genRand.Next(11, 13))
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
							WorldMod.SquareTileFrame(num2, num - 1, true);
							
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
							int num9 = WorldMod.genRand.Next(3);
							if (num9 == 0 && flag)
							{
								Main.tile.At(num2 - 1, num).SetActive (true);
								Main.tile.At(num2 - 1, num).SetType (80);
								WorldMod.SquareTileFrame(num2 - 1, num, true);
								NetMessage.SendTileSquare(-1, num2 - 1, num, 1);
								return;
							}
							else if (num9 == 1 && flag2)
							{
								Main.tile.At(num2 + 1, num).SetActive (true);
								Main.tile.At(num2 + 1, num).SetType (80);
								WorldMod.SquareTileFrame(num2 + 1, num, true);
								NetMessage.SendTileSquare(-1, num2 + 1, num, 1);

								return;
							}
							else
							{
								if (num5 >= WorldMod.genRand.Next(2, 8))
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
								WorldMod.SquareTileFrame(num2, num - 1, true);
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
						WorldMod.SquareTileFrame(num2, num - 1, true);
						
						NetMessage.SendTileSquare(-1, num2, num - 1, 1);
						return;
					}
				}
			}
		}
		
		public static void CheckPot(int i, int j, int type = 28)
		{
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 2; num3++)
					{
						if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
						{
							WorldMod.KillTile(num2, num3, false, false, false);
						}
					}
				}
				if (WorldMod.genRand.Next(50) == 0)
				{
					if ((double)j < Main.worldSurface)
					{
						int num4 = WorldMod.genRand.Next(4);
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
						int num5 = WorldMod.genRand.Next(7);
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
						int num6 = WorldMod.genRand.Next(10);
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
						int num7 = WorldMod.genRand.Next(12);
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
						float num9 = (float)(200 + WorldMod.genRand.Next(-100, 101));
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
				WorldMod.destroyObject = false;
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
			if (WorldMod.destroyObject)
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
				WorldMod.destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 3; num3++)
					{
						if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
						{
							Chest.DestroyChest(num2, num3);
							WorldMod.KillTile(num2, num3, false, false, false);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, type2, 1, false);
				WorldMod.destroyObject = false;
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
							if (type == 24 && WorldMod.genRand.Next(13) == 0)
							{
								Main.tile.At(i, j).SetActive (true);
								Main.tile.At(i, j).SetType (32);
								WorldMod.SquareTileFrame(i, j, true);
							}
							else if (Main.tile.At(i, j + 1).Type == 78)
							{
								Main.tile.At(i, j).SetActive (true);
								Main.tile.At(i, j).SetType ((byte)type);
								Main.tile.At(i, j).SetFrameX ((short)(WorldMod.genRand.Next(2) * 18 + 108));
							}
							else if (Main.tile.At(i, j).Wall == 0 && Main.tile.At(i, j + 1).Wall == 0)
							{
								if (WorldMod.genRand.Next(50) == 0 || (type == 24 && WorldMod.genRand.Next(40) == 0))
								{
									Main.tile.At(i, j).SetActive (true);
									Main.tile.At(i, j).SetType ((byte)type);
									Main.tile.At(i, j).SetFrameX (144);
								}
								else if (WorldMod.genRand.Next(35) == 0)
							    {
								    Main.tile.At(i, j).SetActive (true);
								    Main.tile.At(i, j).SetType ((byte)type);
								    Main.tile.At(i, j).SetFrameX ((short)(WorldMod.genRand.Next(2) * 18 + 108));
							    }
							    else
							    {
								    Main.tile.At(i, j).SetActive (true);
								    Main.tile.At(i, j).SetType ((byte)type);
								    Main.tile.At(i, j).SetFrameX ((short)(WorldMod.genRand.Next(6) * 18));
							    }
							}
						}
					}
					else if (type == 61)
					{
						if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 60)
						{
							if (WorldMod.genRand.Next(10) == 0 && (double)j > Main.worldSurface)
							{
								Main.tile.At(i, j).SetActive (true);
								Main.tile.At(i, j).SetType (69);
								WorldMod.SquareTileFrame(i, j, true);
							}
							else if (WorldMod.genRand.Next(15) == 0 && (double)j > Main.worldSurface)
							{
								Main.tile.At(i, j).SetActive (true);
								Main.tile.At(i, j).SetType ((byte)type);
								Main.tile.At(i, j).SetFrameX (144);
							}
							else if (WorldMod.genRand.Next(1000) == 0 && (double)j > Main.rockLayer)
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
									Main.tile.At(i, j).SetFrameX ((short)(WorldMod.genRand.Next(8) * 18));
								}
								else
								{
									Main.tile.At(i, j).SetFrameX ((short)(WorldMod.genRand.Next(6) * 18));
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
							Main.tile.At(i, j).SetFrameX ((short)(WorldMod.genRand.Next(5) * 18));
						}
					}
					else if (type == 4)
					{
						if ((Main.tile.At(i - 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i - 1, j).Type] || (Main.tile.At(i - 1, j).Type == 5 && Main.tile.At(i - 1, j - 1).Type == 5 && Main.tile.At(i - 1, j + 1).Type == 5))) || (Main.tile.At(i + 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i + 1, j).Type] || (Main.tile.At(i + 1, j).Type == 5 && Main.tile.At(i + 1, j - 1).Type == 5 && Main.tile.At(i + 1, j + 1).Type == 5))) || (Main.tile.At(i, j + 1).Active && Main.tileSolid[(int)Main.tile.At(i, j + 1).Type]))
						{
							Main.tile.At(i, j).SetActive (true);
							Main.tile.At(i, j).SetType ((byte)type);
							WorldMod.SquareTileFrame(i, j, true);
						}
					}
					else if (type == 10)
					{
						if (!Main.tile.At(i, j - 1).Active && !Main.tile.At(i, j - 2).Active && Main.tile.At(i, j - 3).Active && Main.tileSolid[(int)Main.tile.At(i, j - 3).Type])
						{
							WorldMod.PlaceDoor(i, j - 1, type);
							WorldMod.SquareTileFrame(i, j, true);
						}
						else
						{
							if (Main.tile.At(i, j + 1).Active || Main.tile.At(i, j + 2).Active || !Main.tile.At(i, j + 3).Active || !Main.tileSolid[(int)Main.tile.At(i, j + 3).Type])
							{
								return false;
							}
							WorldMod.PlaceDoor(i, j + 1, type);
							WorldMod.SquareTileFrame(i, j, true);
						}
					}
					else if (type == 34 || type == 35 || type == 36)
					{
						WorldMod.Place3x3(i, j, type);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 13 || type == 33 || type == 49 || type == 50 || type == 78)
					{
						WorldMod.PlaceOnTable1x1(i, j, type);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 14 || type == 26)
					{
						WorldMod.Place3x2(i, j, type);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 20)
					{
						if (Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 2)
						{
							WorldMod.Place1x2(i, j, type);
							WorldMod.SquareTileFrame(i, j, true);
						}
					}
					else if (type == 15)
					{
						WorldMod.Place1x2(i, j, type);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 16 || type == 18 || type == 29)
					{
						WorldMod.Place2x1(i, j, type);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 17 || type == 77)
					{
						WorldMod.Place3x2(i, j, type);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 21)
					{
						WorldMod.PlaceChest(i, j, type, false, style);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 27)
					{
						WorldMod.PlaceSunflower(i, j, 27);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 28)
					{
						WorldMod.PlacePot(i, j, 28);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 42)
					{
						WorldMod.Place1x2Top(i, j, type);
						WorldMod.SquareTileFrame(i, j, true);
					}
					else if (type == 55 || type == 85)
					{
						WorldMod.PlaceSign(i, j, type);
					}
					else if (Main.tileAlch[type])
					{
						WorldMod.PlaceAlch(i, j, style);
					}
					else if (type != 85)
					{
						if (type == 79)
						{
							int direction = 1;
							if (plr > -1)
							{
								direction = Main.players[plr].direction;
							}
							WorldMod.Place4x2(i, j, type, direction);
						}
						else if (type == 81)
						{
							Main.tile.At(i, j).SetFrameX ((short)(26 * WorldMod.genRand.Next(6)));
							Main.tile.At(i, j).SetActive (true);
							Main.tile.At(i, j).SetType ((byte)type);
						}
						else
						{
							Main.tile.At(i, j).SetActive (true);
							Main.tile.At(i, j).SetType ((byte)type);
						}
					}
					if (Main.tile.At(i, j).Active && !mute)
					{
						WorldMod.SquareTileFrame(i, j, true);
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
						WorldMod.SquareWallFrame(i, j, true);
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
					WorldMod.SquareWallFrame(i, j, true);
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
					if (!effectOnly && !WorldMod.stopDrops)
					{
						if (Main.tile.At(i, j).Type == 3)
						{
							if (Main.tile.At(i, j).FrameX == 144)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 5, 1, false);
							}
						}
						else if (Main.tile.At(i, j).Type == 24)
						{
							if (Main.tile.At(i, j).FrameX == 144)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 60, 1, false);
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
						WorldMod.SquareTileFrame(i, j, true);
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
					if (!noItem && !WorldMod.stopDrops)
					{
						int num4 = 0;
						if (Main.tile.At(i, j).Type == 0 || Main.tile.At(i, j).Type == 2)
						{
							num4 = 2;
						}
						else if (Main.tile.At(i, j).Type == 1)
				        {
					        num4 = 3;
				        }
				        else if (Main.tile.At(i, j).Type == 3 || Main.tile.At(i, j).Type == 73)
						{
							if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].HasItem(281))
							{
								num4 = 283;
							}
						}
						else if (Main.tile.At(i, j).Type == 4)
						{
							num4 = 8;
						}
						else if (Main.tile.At(i, j).Type == 5)
						{
							if (Main.tile.At(i, j).FrameX >= 22 && Main.tile.At(i, j).FrameY >= 198)
							{
								if (WorldMod.genRand.Next(2) == 0)
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
						else if (Main.tile.At(i, j).Type == 6)
						{
							num4 = 11;
						}
						else if (Main.tile.At(i, j).Type == 7)
						{
							num4 = 12;
						}
						else if (Main.tile.At(i, j).Type == 8)
						{
							num4 = 13;
						}
						else if (Main.tile.At(i, j).Type == 9)
						{
							num4 = 14;
						}
						else if (Main.tile.At(i, j).Type == 13)
						{
						    if (Main.tile.At(i, j).FrameX == 18)
						    {
							    num4 = 28;
						    }
						    else if (Main.tile.At(i, j).FrameX == 36)
							{
								num4 = 110;
							}
							else
							{
								num4 = 31;
							}
					    }
					    else if (Main.tile.At(i, j).Type == 19)
						{
							num4 = 94;
						}
						else if (Main.tile.At(i, j).Type == 22)
						{
							num4 = 56;
						}
						else if (Main.tile.At(i, j).Type == 23)
						{
							num4 = 2;
						}
						else if (Main.tile.At(i, j).Type == 25)
						{
							num4 = 61;
						}
						else if (Main.tile.At(i, j).Type == 30)
						{
							num4 = 9;
						}
						else if (Main.tile.At(i, j).Type == 33)
						{
							num4 = 105;
						}
						else if (Main.tile.At(i, j).Type == 37)
						{
							num4 = 116;
						}
						else if (Main.tile.At(i, j).Type == 38)
						{
							num4 = 129;
						}
						else if (Main.tile.At(i, j).Type == 39)
				        {
					        num4 = 131;
				        }
				        else if (Main.tile.At(i, j).Type == 40)
						{
							num4 = 133;
						}
						else if (Main.tile.At(i, j).Type == 41)
						{
							num4 = 134;
						}
						else if (Main.tile.At(i, j).Type == 43)
					    {
						    num4 = 137;
					    }
					    else if (Main.tile.At(i, j).Type == 44)
						{
							num4 = 139;
						}
						else if (Main.tile.At(i, j).Type == 45)
						{
							num4 = 141;
						}
						else if (Main.tile.At(i, j).Type == 46)
						{
							num4 = 143;
						}
						else if (Main.tile.At(i, j).Type == 47)
						{
							num4 = 145;
						}
						else if (Main.tile.At(i, j).Type == 48)
						{
							num4 = 147;
						}
						else if (Main.tile.At(i, j).Type == 49)
						{
							num4 = 148;
						}
						else if (Main.tile.At(i, j).Type == 51)
						{
							num4 = 150;
						}
						else if (Main.tile.At(i, j).Type == 53)
						{
							num4 = 169;
						}
						else if (Main.tile.At(i, j).Type != 54)
						{
							if (Main.tile.At(i, j).Type == 56)
							{
								num4 = 173;
							}
							else if (Main.tile.At(i, j).Type == 57)
							{
								num4 = 172;
							}
							else if (Main.tile.At(i, j).Type == 58)
							{
								num4 = 174;
							}
							else if (Main.tile.At(i, j).Type == 60)
							{
								num4 = 176;
							}
							else if (Main.tile.At(i, j).Type == 70)
							{
								num4 = 176;
							}
							else if (Main.tile.At(i, j).Type == 75)
							{
								num4 = 192;
							}
							else if (Main.tile.At(i, j).Type == 76)
							{
								num4 = 214;
							}
							else if (Main.tile.At(i, j).Type == 78)
							{
								num4 = 222;
							}
							else if (Main.tile.At(i, j).Type == 81)
							{
								num4 = 275;
							}
							else if (Main.tile.At(i, j).Type == 80)
							{
								num4 = 276;
							}
							else if (Main.tile.At(i, j).Type == 61 || Main.tile.At(i, j).Type == 74)
							{
								if (Main.tile.At(i, j).FrameX == 162)
								{
									num4 = 223;
								}
								else if (Main.tile.At(i, j).FrameX >= 108 && Main.tile.At(i, j).FrameX <= 126 && (double)j > Main.rockLayer)
						        {
							        if (WorldMod.genRand.Next(2) == 0)
							        {
								        num4 = 208;
							        }
						        }
						        else if (WorldMod.genRand.Next(100) == 0)
								{
									num4 = 195;
								}
							}
							else if (Main.tile.At(i, j).Type == 59 || Main.tile.At(i, j).Type == 60)
						    {
							    num4 = 176;
						    }
						    else if (Main.tile.At(i, j).Type == 71 || Main.tile.At(i, j).Type == 72)
							{
								if (WorldMod.genRand.Next(50) == 0)
								{
									num4 = 194;
								}
								else if (WorldMod.genRand.Next(2) == 0)
								{
									num4 = 183;
								}
							}
							else if (Main.tile.At(i, j).Type >= 63 && Main.tile.At(i, j).Type <= 68)
							{
								num4 = (int)(Main.tile.At(i, j).Type - 63 + 177);
							}
							else if (Main.tile.At(i, j).Type == 50)
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
							else if (Main.tileAlch[(int)Main.tile.At(i, j).Type] && Main.tile.At(i, j).Type > 82)
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
									Item.NewItem(i * 16, j * 16, 16, 16, 307 + num6, WorldMod.genRand.Next(1, 4), false);
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
					WorldMod.SquareTileFrame(i, j, true);
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
			WorldMod.spawnDelay++;
			if (Main.invasionType > 0)
			{
				WorldMod.spawnDelay = 0;
			}
			if (WorldMod.spawnDelay >= 20)
			{
				flag = true;
				WorldMod.spawnDelay = 0;
				if (WorldMod.spawnNPC != 37)
				{
					for (int i = 0; i < 1000; i++)
					{
						if (Main.npcs[i].Active && Main.npcs[i].homeless && Main.npcs[i].townNPC)
						{
							WorldMod.spawnNPC = Main.npcs[i].Type;
							break;
						}
					}
				}
			}
			int num3 = 0;

			if (WorldMod.genRand == null)
			{
				WorldMod.genRand = new Random();
			}

			while ((float)num3 < (float)(Main.maxTilesX * Main.maxTilesY) * num)
			{
				int num4 = WorldMod.genRand.Next(10, Main.maxTilesX - 10);
				int num5 = WorldMod.genRand.Next(10, (int)Main.worldSurface - 1);
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
						WorldMod.GrowAlch(num4, num5);
					}
					if (Main.tile.At(num4, num5).Liquid > 32)
					{
						if (Main.tile.At(num4, num5).Active && (Main.tile.At(num4, num5).Type == 3 || Main.tile.At(num4, num5).Type == 20 || Main.tile.At(num4, num5).Type == 24 || Main.tile.At(num4, num5).Type == 27 || Main.tile.At(num4, num5).Type == 73))
						{
							WorldMod.KillTile(num4, num5, false, false, false);
							NetMessage.SendData(17, -1, -1, "", 0, (float)num4, (float)num5);
						}
					}
					else if (Main.tile.At(num4, num5).Active)
					{
						if (Main.tile.At(num4, num5).Type == 80)
						{
							if (WorldMod.genRand.Next(15) == 0)
							{
								WorldMod.GrowCactus(num4, num5);
							}
						}
						else if (Main.tile.At(num4, num5).Type == 53)
						{
							if (!Main.tile.At(num4, num8).Active)
							{
								if (num4 < 250 || num4 > Main.maxTilesX - 250)
								{
									if (WorldMod.genRand.Next(500) == 0 && Main.tile.At(num4, num8).Liquid == 255 && Main.tile.At(num4, num8 - 1).Liquid == 255 && Main.tile.At(num4, num8 - 2).Liquid == 255 && Main.tile.At(num4, num8 - 3).Liquid == 255 && Main.tile.At(num4, num8 - 4).Liquid == 255)
									{
										WorldMod.PlaceTile(num4, num8, 81, true, false, -1, 0);
										if (Main.tile.At(num4, num8).Active)
										{
											NetMessage.SendTileSquare(-1, num4, num8, 1);
										}
									}
								}
								else if (num4 > 400 && num4 < Main.maxTilesX - 400 && WorldMod.genRand.Next(300) == 0)
								{
									WorldMod.GrowCactus(num4, num5);
								}
							}
						}
						else if (Main.tile.At(num4, num5).Type == 78)
						{
							if (!Main.tile.At(num4, num8).Active)
							{
								WorldMod.PlaceTile(num4, num8, 3, true, false, -1, 0);
								if (Main.tile.At(num4, num8).Active)
								{
									NetMessage.SendTileSquare(-1, num4, num8, 1);
								}
							}
						}
						else if (Main.tile.At(num4, num5).Type == 2 || Main.tile.At(num4, num5).Type == 23 || Main.tile.At(num4, num5).Type == 32)
						{
							int num10 = (int)Main.tile.At(num4, num5).Type;
							if (!Main.tile.At(num4, num8).Active && WorldMod.genRand.Next(12) == 0 && num10 == 2)
							{
								WorldMod.PlaceTile(num4, num8, 3, true, false, -1, 0);
								if (Main.tile.At(num4, num8).Active)
								{
									NetMessage.SendTileSquare(-1, num4, num8, 1);
								}
							}
							if (!Main.tile.At(num4, num8).Active && WorldMod.genRand.Next(10) == 0 && num10 == 23)
							{
								WorldMod.PlaceTile(num4, num8, 24, true, false, -1, 0);
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
											WorldMod.SpreadGrass(j, k, 0, num10, false);
											if (num10 == 23)
											{
												WorldMod.SpreadGrass(j, k, 2, num10, false);
											}
											if ((int)Main.tile.At(j, k).Type == num10)
											{
												WorldMod.SquareTileFrame(j, k, true);
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
						else if (Main.tile.At(num4, num5).Type == 20 && WorldMod.genRand.Next(20) == 0 && !WorldMod.PlayerLOS(num4, num5))
						{
							WorldMod.GrowTree(num4, num5);
						}
						if (Main.tile.At(num4, num5).Type == 3 && WorldMod.genRand.Next(20) == 0 && Main.tile.At(num4, num5).FrameX < 144)
						{
							Main.tile.At(num4, num5).SetType (73);
							NetMessage.SendTileSquare(-1, num4, num5, 3);
						}
						if (Main.tile.At(num4, num5).Type == 32 && WorldMod.genRand.Next(3) == 0)
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
								int num14 = WorldMod.genRand.Next(4);
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
											WorldMod.SquareTileFrame(num11, num12, true);
												
											NetMessage.SendTileSquare(-1, num11, num12, 3);
										}
									}
								}
							}
						}
					}
					else if (flag && WorldMod.spawnNPC > 0)
					{
						WorldMod.SpawnNPC(num4, num5);
					}
					if (Main.tile.At(num4, num5).Active)
					{
						if ((Main.tile.At(num4, num5).Type == 2 || Main.tile.At(num4, num5).Type == 52) && WorldMod.genRand.Next(40) == 0 && !Main.tile.At(num4, num5 + 1).Active && !Main.tile.At(num4, num5 + 1).Lava)
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
								WorldMod.SquareTileFrame(num20, num21, true);
								NetMessage.SendTileSquare(-1, num20, num21, 3);
							}
						}
						if (Main.tile.At(num4, num5).Type == 60)
						{
							int type = (int)Main.tile.At(num4, num5).Type;
							if (!Main.tile.At(num4, num8).Active && WorldMod.genRand.Next(7) == 0)
							{
								WorldMod.PlaceTile(num4, num8, 61, true, false, -1, 0);
								if (Main.tile.At(num4, num8).Active)
								{
									NetMessage.SendTileSquare(-1, num4, num8, 1);
								}
							}
							else if (WorldMod.genRand.Next(500) == 0 && (!Main.tile.At(num4, num8).Active || Main.tile.At(num4, num8).Type == 61 || Main.tile.At(num4, num8).Type == 74 || Main.tile.At(num4, num8).Type == 69) && !WorldMod.PlayerLOS(num4, num5))
							{
								WorldMod.GrowTree(num4, num5);
							}
							bool flag5 = false;
							for (int num22 = num6; num22 < num7; num22++)
							{
								for (int num23 = num8; num23 < num9; num23++)
								{
									if ((num4 != num22 || num5 != num23) && Main.tile.At(num22, num23).Active && Main.tile.At(num22, num23).Type == 59)
									{
										WorldMod.SpreadGrass(num22, num23, 59, type, false);
										if ((int)Main.tile.At(num22, num23).Type == type)
										{
											WorldMod.SquareTileFrame(num22, num23, true);
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
						if (Main.tile.At(num4, num5).Type == 61 && WorldMod.genRand.Next(3) == 0 && Main.tile.At(num4, num5).FrameX < 144)
						{
							Main.tile.At(num4, num5).SetType (74);
							NetMessage.SendTileSquare(-1, num4, num5, 3);
						}
						if ((Main.tile.At(num4, num5).Type == 60 || Main.tile.At(num4, num5).Type == 62) && WorldMod.genRand.Next(15) == 0 && !Main.tile.At(num4, num5 + 1).Active && !Main.tile.At(num4, num5 + 1).Lava)
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
								WorldMod.SquareTileFrame(num25, num26, true);
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
				int num28 = WorldMod.genRand.Next(10, Main.maxTilesX - 10);
				int num29 = WorldMod.genRand.Next((int)Main.worldSurface + 2, Main.maxTilesY - 20);
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
						WorldMod.GrowAlch(num28, num29);
					}
					if (Main.tile.At(num28, num29).Liquid <= 32)
					{
						if (Main.tile.At(num28, num29).Active)
						{
							if (Main.tile.At(num28, num29).Type == 60)
							{
								int type2 = (int)Main.tile.At(num28, num29).Type;
								if (!Main.tile.At(num28, num32).Active && WorldMod.genRand.Next(10) == 0)
								{
									WorldMod.PlaceTile(num28, num32, 61, true, false, -1, 0);
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
											WorldMod.SpreadGrass(num34, num35, 59, type2, false);
											if ((int)Main.tile.At(num34, num35).Type == type2)
											{
												WorldMod.SquareTileFrame(num34, num35, true);
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
							if (Main.tile.At(num28, num29).Type == 61 && WorldMod.genRand.Next(3) == 0 && Main.tile.At(num28, num29).FrameX < 144)
							{
								Main.tile.At(num28, num29).SetType (74);
								NetMessage.SendTileSquare(-1, num28, num29, 3);
							}
							if ((Main.tile.At(num28, num29).Type == 60 || Main.tile.At(num28, num29).Type == 62) && WorldMod.genRand.Next(5) == 0 && !Main.tile.At(num28, num29 + 1).Active && !Main.tile.At(num28, num29 + 1).Lava)
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
									WorldMod.SquareTileFrame(num37, num38, true);
									NetMessage.SendTileSquare(-1, num37, num38, 3);
								}
							}
							if (Main.tile.At(num28, num29).Type == 69 && WorldMod.genRand.Next(3) == 0)
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
									int num42 = WorldMod.genRand.Next(4);
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
												WorldMod.SquareTileFrame(num39, num40, true);
												NetMessage.SendTileSquare(-1, num39, num40, 3);
											}
										}
									}
								}
							}
							if (Main.tile.At(num28, num29).Type == 70)
							{
								int type3 = (int)Main.tile.At(num28, num29).Type;
								if (!Main.tile.At(num28, num32).Active && WorldMod.genRand.Next(10) == 0)
								{
									WorldMod.PlaceTile(num28, num32, 71, true, false, -1, 0);
									if (Main.tile.At(num28, num32).Active)
									{
										NetMessage.SendTileSquare(-1, num28, num32, 1);
									}
								}
								if (WorldMod.genRand.Next(200) == 0 && !WorldMod.PlayerLOS(num28, num29))
								{
									WorldMod.GrowShroom(num28, num29);
								}
								bool flag10 = false;
								for (int num50 = num30; num50 < num31; num50++)
								{
									for (int num51 = num32; num51 < num33; num51++)
									{
										if ((num28 != num50 || num29 != num51) && Main.tile.At(num50, num51).Active && Main.tile.At(num50, num51).Type == 59)
										{
											WorldMod.SpreadGrass(num50, num51, 59, type3, false);
											if ((int)Main.tile.At(num50, num51).Type == type3)
											{
												WorldMod.SquareTileFrame(num50, num51, true);
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
						else if (flag && WorldMod.spawnNPC > 0)
						{
							WorldMod.SpawnNPC(num28, num29);
						}
					}
				}
				num27++;
			}
			if (Main.rand.Next(100) == 0)
			{
				WorldMod.PlantAlch();
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
				WorldMod.SquareWallFrame(i, j, true);
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
							WorldMod.SpreadGrass(m, n, dirt, grass, true);
						}
					}
				}
			}
		}
		
		public static void SquareTileFrame(int i, int j, bool resetFrame = true)
		{
			WorldMod.TileFrame(i - 1, j - 1, false, false);
			WorldMod.TileFrame(i - 1, j, false, false);
			WorldMod.TileFrame(i - 1, j + 1, false, false);
			WorldMod.TileFrame(i, j - 1, false, false);
			WorldMod.TileFrame(i, j, resetFrame, false);
			WorldMod.TileFrame(i, j + 1, false, false);
			WorldMod.TileFrame(i + 1, j - 1, false, false);
			WorldMod.TileFrame(i + 1, j, false, false);
			WorldMod.TileFrame(i + 1, j + 1, false, false);
		}
		
		public static void SquareWallFrame(int i, int j, bool resetFrame = true)
		{
			WorldMod.WallFrame(i - 1, j - 1, false);
			WorldMod.WallFrame(i - 1, j, false);
			WorldMod.WallFrame(i - 1, j + 1, false);
			WorldMod.WallFrame(i, j - 1, false);
			WorldMod.WallFrame(i, j, resetFrame);
			WorldMod.WallFrame(i, j + 1, false);
			WorldMod.WallFrame(i + 1, j - 1, false);
			WorldMod.WallFrame(i + 1, j, false);
			WorldMod.WallFrame(i + 1, j + 1, false);
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
					WorldMod.TileFrame(i, j, true, true);
					WorldMod.WallFrame(i, j, true);
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
					WorldMod.TileFrame(i, j, false, false);
					WorldMod.WallFrame(i, j, false);
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
					else if (Main.tile.At(i, j).Liquid > 0)
					{
						if (Main.tile.At(i, j).Active)
						{
							if (Main.tileWaterDeath[(int)Main.tile.At(i, j).Type])
							{
								WorldMod.KillTile(i, j, false, false, false);
							}
							if (Main.tile.At(i, j).Lava && Main.tileLavaDeath[(int)Main.tile.At(i, j).Type])
							{
								WorldMod.KillTile(i, j, false, false, false);
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
			}
		}
		
		public static void EveryTileFrame()
		{
			using (var prog = new ProgressLogger (Main.maxTilesX, "Finding tile frames"))
			{
				WorldMod.noLiquidCheck = true;
				WorldMod.noTileActions = true;
				for (int i = 0; i < Main.maxTilesX; i++)
				{
					prog.Value = i;
					
					for (int j = 0; j < Main.maxTilesY; j++)
					{
						WorldMod.TileFrame(i, j, true, false);
						WorldMod.WallFrame(i, j, true);
					}
				}
				WorldMod.noLiquidCheck = false;
				WorldMod.noTileActions = false;
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
				WorldMod.KillTile(i, j, false, false, false);
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
					else if (j >= (int)Main.worldSurface)
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
				int num9 = 0;
				if (resetFrame)
				{
					num9 = WorldMod.genRand.Next(0, 3);
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
						else if (num6 != wall && num8 != wall)
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
						else if (num != wall && num6 != wall)
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
						else if (num3 != wall && num8 != wall)
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
					else if (num2 != wall && num7 == wall && (num4 == wall & num5 == wall))
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
					else if (num2 == wall && num7 != wall && (num4 == wall & num5 == wall))
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
					else if (num2 == wall && num7 == wall && (num4 != wall & num5 == wall))
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
					else if (num2 == wall && num7 == wall && (num4 == wall & num5 != wall))
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
					else if (num2 != wall && num7 == wall && (num4 != wall & num5 == wall))
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
					else if (num2 != wall && num7 == wall && (num4 == wall & num5 != wall))
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
					else if (num2 == wall && num7 != wall && (num4 != wall & num5 == wall))
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
					else if (num2 == wall && num7 != wall && (num4 == wall & num5 != wall))
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
					else if (num2 == wall && num7 == wall && (num4 != wall & num5 != wall))
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
					else if (num2 != wall && num7 != wall && (num4 == wall & num5 == wall))
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
					else if (num2 != wall && num7 == wall && (num4 != wall & num5 != wall))
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
					else if (num2 == wall && num7 != wall && (num4 != wall & num5 != wall))
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
					else if (num2 != wall && num7 != wall && (num4 != wall & num5 == wall))
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
					else if (num2 != wall && num7 != wall && (num4 == wall & num5 != wall))
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
					else if (num2 != wall && num7 != wall && (num4 != wall & num5 != wall))
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
				if (Main.tile.At(i, j).Liquid > 0 && !WorldMod.noLiquidCheck)
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
						WorldMod.PlantCheck(i, j);
						return;
					}
					WorldMod.mergeUp = false;
					WorldMod.mergeDown = false;
					WorldMod.mergeLeft = false;
					WorldMod.mergeRight = false;
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
						WorldMod.KillTile(i, j, false, false, false);
						return;
					}
					else
					{
						if (num9 == 80)
						{
							WorldMod.CactusFrame(i, j);
							return;
						}
						if (num9 == 12 || num9 == 31)
						{
							if (!WorldMod.destroyObject)
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
									WorldMod.destroyObject = true;
									if ((int)Main.tile.At(num10, num11).Type == num9)
									{
										WorldMod.KillTile(num10, num11, false, false, false);
									}
									if ((int)Main.tile.At(num10 + 1, num11).Type == num9)
									{
										WorldMod.KillTile(num10 + 1, num11, false, false, false);
									}
									if ((int)Main.tile.At(num10, num11 + 1).Type == num9)
									{
										WorldMod.KillTile(num10, num11 + 1, false, false, false);
									}
									if ((int)Main.tile.At(num10 + 1, num11 + 1).Type == num9)
									{
										WorldMod.KillTile(num10 + 1, num11 + 1, false, false, false);
									}
									if (num9 == 12)
									{
										Item.NewItem(num10 * 16, num11 * 16, 32, 32, 29, 1, false);
									}
									else if (num9 == 31)
									{
										if (WorldMod.genRand.Next(2) == 0)
										{
											WorldMod.spawnMeteor = true;
										}
										int num12 = Main.rand.Next(5);
										if (!WorldMod.shadowOrbSmashed)
										{
											num12 = 0;
										}
										if (num12 == 0)
										{
											Item.NewItem(num10 * 16, num11 * 16, 32, 32, 96, 1, false);
											int stack = WorldMod.genRand.Next(25, 51);
											Item.NewItem(num10 * 16, num11 * 16, 32, 32, 97, stack, false);
										}
										else if (num12 == 1)
										{
											Item.NewItem(num10 * 16, num11 * 16, 32, 32, 64, 1, false);
										}
										else if (num12 == 2)
										{
											Item.NewItem(num10 * 16, num11 * 16, 32, 32, 162, 1, false);
										}
										else if (num12 == 3)
										{
											Item.NewItem(num10 * 16, num11 * 16, 32, 32, 115, 1, false);
										}
										else if (num12 == 4)
										{
											Item.NewItem(num10 * 16, num11 * 16, 32, 32, 111, 1, false);
										}
										WorldMod.shadowOrbSmashed = true;
										WorldMod.shadowOrbCount++;
										if (WorldMod.shadowOrbCount >= 3)
										{
											WorldMod.shadowOrbCount = 0;
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
											if (WorldMod.shadowOrbCount == 2)
											{
												text = "Screams echo around you...";
											}
											NetMessage.SendData(25, -1, -1, text, 255, 50f, 255f, 130f);
										}
									}
									WorldMod.destroyObject = false;
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
							else if (num4 == num9 && num5 == -1)
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
							else if (num4 == -1 && num5 == num9)
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
							else if (num4 != num9 && num5 == num9)
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
							else if (num4 == num9 && num5 != num9)
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
							else if (num4 != num9 && num4 != -1 && num5 == -1)
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
							else if (num4 == -1 && num5 != num9 && num5 != -1)
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
						else
						{
							if (num9 == 10)
							{
								if (!WorldMod.destroyObject)
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
										WorldMod.destroyObject = true;
										WorldMod.KillTile(i, num17, false, false, false);
										WorldMod.KillTile(i, num17 + 1, false, false, false);
										WorldMod.KillTile(i, num17 + 2, false, false, false);
										Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
									}
									WorldMod.destroyObject = false;
								}
								return;
							}
							if (num9 == 11)
							{
								if (!WorldMod.destroyObject)
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
									else if (frameX2 == 18)
									{
										num19 = i - 1;
										num18 = 1;
									}
									else if (frameX2 == 36)
									{
										num19 = i + 1;
										num18 = -1;
									}
									else if (frameX2 == 54)
									{
										num19 = i;
										num18 = -1;
									}
									if (frameY3 == 0)
									{
										num20 = j;
									}
									else if (frameY3 == 18)
								    {
									    num20 = j - 1;
								    }
								    else if (frameY3 == 36)
								    {
									    num20 = j - 2;
								    }
									if (!Main.tile.At(num19, num20 - 1).Active || !Main.tileSolid[(int)Main.tile.At(num19, num20 - 1).Type] || !Main.tile.At(num19, num20 + 3).Active || !Main.tileSolid[(int)Main.tile.At(num19, num20 + 3).Type])
									{
										flag2 = true;
										WorldMod.destroyObject = true;
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
												WorldMod.destroyObject = true;
												Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
												flag2 = true;
												l = num21;
												m = num20;
											}
											if (flag2)
											{
												WorldMod.KillTile(l, m, false, false, false);
											}
										}
									}
									WorldMod.destroyObject = false;
								}
								return;
							}
							if (num9 == 34 || num9 == 35 || num9 == 36)
							{
								WorldMod.Check3x3(i, j, (int)((byte)num9));
								return;
							}
							if (num9 == 15 || num9 == 20)
							{
								WorldMod.Check1x2(i, j, (byte)num9);
								return;
							}
							if (num9 == 14 || num9 == 17 || num9 == 26 || num9 == 77)
							{
								WorldMod.Check3x2(i, j, (int)((byte)num9));
								return;
							}
							if (num9 == 16 || num9 == 18 || num9 == 29)
							{
								WorldMod.Check2x1(i, j, (byte)num9);
								return;
							}
							if (num9 == 13 || num9 == 33 || num9 == 49 || num9 == 50 || num9 == 78)
							{
								WorldMod.CheckOnTable1x1(i, j, (int)((byte)num9));
								return;
							}
							if (num9 == 21)
							{
								WorldMod.CheckChest(i, j, (int)((byte)num9));
								return;
							}
							if (num9 == 27)
							{
								WorldMod.CheckSunflower(i, j, 27);
								return;
							}
							if (num9 == 28)
							{
								WorldMod.CheckPot(i, j, 28);
								return;
							}
							if (num9 == 42)
							{
								WorldMod.Check1x2Top(i, j, (byte)num9);
								return;
							}
							if (num9 == 55 || num9 == 85)
							{
								WorldMod.CheckSign(i, j, num9);
								return;
							}
							if (num9 == 79)
							{
								WorldMod.Check4x2(i, j, num9);
								return;
							}
							if (num9 == 85)
							{
								WorldMod.Check2x2(i, j, num9);
								return;
							}
							if (num9 == 81)
							{
								if (num4 != -1 || num2 != -1 || num5 != -1)
								{
									WorldMod.KillTile(i, j, false, false, false);
									return;
								}
								if (num7 < 0 || !Main.tileSolid[num7])
								{
									WorldMod.KillTile(i, j, false, false, false);
								}
								return;
							}
							else if (Main.tileAlch[num9])
							{
								WorldMod.CheckAlch(i, j);
								return;
							}
						}
						if (num9 == 72)
						{
							if (num7 != num9 && num7 != 70)
							{
								WorldMod.KillTile(i, j, false, false, false);
							}
							else if (num2 != num9 && Main.tile.At(i, j).FrameX == 0)
							{
								Main.tile.At(i, j).SetFrameNumber ((byte)WorldMod.genRand.Next(3));
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
									WorldMod.KillTile(i, j, false, false, false);
								}
							}
							else if ((Main.tile.At(i, j).FrameX == 88 && Main.tile.At(i, j).FrameY >= 0 && Main.tile.At(i, j).FrameY <= 44) || (Main.tile.At(i, j).FrameX == 66 && Main.tile.At(i, j).FrameY >= 66 && Main.tile.At(i, j).FrameY <= 130) || (Main.tile.At(i, j).FrameX == 110 && Main.tile.At(i, j).FrameY >= 66 && Main.tile.At(i, j).FrameY <= 110) || (Main.tile.At(i, j).FrameX == 132 && Main.tile.At(i, j).FrameY >= 0 && Main.tile.At(i, j).FrameY <= 176))
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
								else if (num4 == num9)
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
								else if (num5 == num9)
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
							if (Main.tile.At(i, j).FrameY >= 132 && Main.tile.At(i, j).FrameY <= 176 && (Main.tile.At(i, j).FrameX == 0 || Main.tile.At(i, j).FrameX == 66 || Main.tile.At(i, j).FrameX == 88))
							{
								if (num7 != 2)
								{
									WorldMod.KillTile(i, j, false, false, false);
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
								else if (num4 != num9)
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
								else if (num5 != num9)
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
							if ((Main.tile.At(i, j).FrameX == 66 && (Main.tile.At(i, j).FrameY == 0 || Main.tile.At(i, j).FrameY == 22 || Main.tile.At(i, j).FrameY == 44)) || (Main.tile.At(i, j).FrameX == 88 && (Main.tile.At(i, j).FrameY == 66 || Main.tile.At(i, j).FrameY == 88 || Main.tile.At(i, j).FrameY == 110)) || (Main.tile.At(i, j).FrameX == 44 && (Main.tile.At(i, j).FrameY == 198 || Main.tile.At(i, j).FrameY == 220 || Main.tile.At(i, j).FrameY == 242)) || (Main.tile.At(i, j).FrameX == 66 && (Main.tile.At(i, j).FrameY == 198 || Main.tile.At(i, j).FrameY == 220 || Main.tile.At(i, j).FrameY == 242)))
							{
								if (num4 != num9 && num5 != num9)
								{
									WorldMod.KillTile(i, j, false, false, false);
								}
							}
							else if (num7 == -1 || num7 == 23)
							{
								WorldMod.KillTile(i, j, false, false, false);
							}
							else if (num2 != num9 && Main.tile.At(i, j).FrameY < 198 && ((Main.tile.At(i, j).FrameX != 22 && Main.tile.At(i, j).FrameX != 44) || Main.tile.At(i, j).FrameY < 132))
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
										else if (num4 == num9)
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
										else if (num5 == num9)
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
									else if (num4 == num9 && num5 == num9)
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
									else if (num4 == num9)
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
									else if (num5 == num9)
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
							num22 = WorldMod.genRand.Next(0, 3);
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
										WorldMod.TileFrame(i, j - 1, false, false);
										if (WorldMod.mergeDown)
										{
											num2 = num9;
										}
									}
									if (num7 == n)
									{
										WorldMod.TileFrame(i, j + 1, false, false);
										if (WorldMod.mergeUp)
										{
											num7 = num9;
										}
									}
									if (num4 == n)
									{
										WorldMod.TileFrame(i - 1, j, false, false);
										if (WorldMod.mergeRight)
										{
											num4 = num9;
										}
									}
									if (num5 == n)
									{
										WorldMod.TileFrame(i + 1, j, false, false);
										if (WorldMod.mergeLeft)
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
						else if (num9 == 57)
						{
							if (num2 == 58)
							{
								WorldMod.TileFrame(i, j - 1, false, false);
								if (WorldMod.mergeDown)
								{
									num2 = num9;
								}
							}
							if (num7 == 58)
							{
								WorldMod.TileFrame(i, j + 1, false, false);
								if (WorldMod.mergeUp)
								{
									num7 = num9;
								}
							}
							if (num4 == 58)
							{
								WorldMod.TileFrame(i - 1, j, false, false);
								if (WorldMod.mergeRight)
								{
									num4 = num9;
								}
							}
							if (num5 == 58)
							{
								WorldMod.TileFrame(i + 1, j, false, false);
								if (WorldMod.mergeLeft)
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
						else if (num9 == 59)
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
						else if (num9 == 58)
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
						else if (num9 == 59)
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
							else if (num9 == 2)
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
							else if (num9 == 23)
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
								else if (num4 == num9 && num5 == num24)
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
							else if (num7 != num9 && num7 != num24 && (num2 == num9 || num2 == num24))
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
								else if (num4 == num9 && num5 == num24)
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
							else if (num4 != num9 && num4 != num24 && (num5 == num9 || num5 == num24))
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
								else if (num7 == num9 && num5 == num2)
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
							else if (num5 != num9 && num5 != num24 && (num4 == num9 || num4 == num24))
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
								else if (num7 == num9 && num5 == num2)
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
							else if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num9)
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
									else if (num3 == num24)
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
									else if (num6 == num24)
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
									else if (num == num24)
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
								else if (num != num9 && num8 != num9)
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
								else if (num3 != num9 && num6 != num9)
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
								else if (num != num9 && num3 == num9 && num6 == num9 && num8 == num9)
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
								else if (num == num9 && num3 != num9 && num6 == num9 && num8 == num9)
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
								else if (num == num9 && num3 == num9 && num6 != num9 && num8 == num9)
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
								else if (num == num9 && num3 == num9 && num6 == num9 && num8 != num9)
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
							else if (num2 == num9 && num7 == num24 && num4 == num9 && num5 == num9 && num == -1 && num3 == -1)
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
							else if (num2 == num24 && num7 == num9 && num4 == num9 && num5 == num9 && num6 == -1 && num8 == -1)
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
							else if (num2 == num9 && num7 == num9 && num4 == num24 && num5 == num9 && num3 == -1 && num8 == -1)
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
							else if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num24 && num == -1 && num6 == -1)
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
							else if (num2 == num9 && num7 == num24 && num4 == num9 && num5 == num9)
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
								else if (num != -1)
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
							else if (num2 == num24 && num7 == num9 && num4 == num9 && num5 == num9)
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
								else if (num6 != -1)
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
							else if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num24)
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
								else if (num6 != -1)
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
							else if (num2 == num9 && num7 == num9 && num4 == num24 && num5 == num9)
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
								else if (num8 != -1)
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
							else if ((num2 == num24 && num7 == num9 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num24 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num24 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num24))
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
								else if (num3 != num9 && num3 != num24 && (num == num9 || num == num24) && (num6 == num9 || num6 == num24) && (num8 == num9 || num8 == num24))
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
								else if (num6 != num9 && num6 != num24 && (num == num9 || num == num24) && (num3 == num9 || num3 == num24) && (num8 == num9 || num8 == num24))
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
								else if (num8 != num9 && num8 != num24 && (num == num9 || num == num24) && (num6 == num9 || num6 == num24) && (num3 == num9 || num3 == num24))
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
							else if (num2 != num24 && num2 != num9 && num7 == num9 && num4 == num9 && num5 != num24 && num5 != num9 && num6 != num24 && num6 != num9)
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
							else if (num7 != num24 && num7 != num9 && num2 == num9 && num4 != num24 && num4 != num9 && num5 == num9 && num3 != num24 && num3 != num9)
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
							else if (num7 != num24 && num7 != num9 && num2 == num9 && num4 == num9 && num5 != num24 && num5 != num9 && num != num24 && num != num9)
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
							else if (num2 != num9 && num2 != num24 && num7 == num9 && num4 == num9 && num5 == num9 && num6 != num9 && num6 != num24 && num8 != num9 && num8 != num24)
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
							else if (num7 != num9 && num7 != num24 && num2 == num9 && num4 == num9 && num5 == num9 && num != num9 && num != num24 && num3 != num9 && num3 != num24)
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
							else if (num4 != num9 && num4 != num24 && num7 == num9 && num2 == num9 && num5 == num9 && num3 != num9 && num3 != num24 && num8 != num9 && num8 != num24)
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
							else if (num5 != num9 && num5 != num24 && num7 == num9 && num2 == num9 && num4 == num9 && num != num9 && num != num24 && num6 != num9 && num6 != num24)
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
							else if (num2 != num24 && num2 != num9 && (num7 == num24 || num7 == num9) && num4 == num24 && num5 == num24)
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
							else if (num7 != num24 && num7 != num9 && (num2 == num24 || num2 == num9) && num4 == num24 && num5 == num24)
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
							else if (num4 != num24 && num4 != num9 && (num5 == num24 || num5 == num9) && num2 == num24 && num7 == num24)
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
							else if (num5 != num24 && num5 != num9 && (num4 == num24 || num4 == num9) && num2 == num24 && num7 == num24)
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
							else if (num2 == num9 && num7 == num24 && num4 == num24 && num5 == num24)
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
							else if (num2 == num24 && num7 == num9 && num4 == num24 && num5 == num24)
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
							else if (num2 == num24 && num7 == num24 && num4 == num9 && num5 == num24)
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
							else if (num2 == num24 && num7 == num24 && num4 == num24 && num5 == num9)
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
								else if ((num8 == num24 || num8 == num9) && num6 != num24 && num6 != num9)
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
							else if (num7 != num9 && num7 != num24 && num2 == num9 && num4 == num9 && num5 == num9)
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
								else if ((num3 == num24 || num3 == num9) && num != num24 && num != num9)
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
							else if (num4 != num9 && num4 != num24 && num2 == num9 && num7 == num9 && num5 == num9)
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
								else if ((num8 == num24 || num8 == num9) && num3 != num24 && num3 != num9)
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
							else if (num5 != num9 && num5 != num24 && num2 == num9 && num7 == num9 && num4 == num9)
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
								else if ((num6 == num24 || num6 == num9) && num != num24 && num != num9)
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
									WorldMod.mergeUp = true;
								}
								else if (num2 == num9 && num7 == -2 && num4 == num9 && num5 == num9)
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
									WorldMod.mergeDown = true;
								}
								else if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == num9)
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
									WorldMod.mergeLeft = true;
								}
								else if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == -2)
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
									WorldMod.mergeRight = true;
								}
								else if (num2 == -2 && num7 == num9 && num4 == -2 && num5 == num9)
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
									WorldMod.mergeUp = true;
									WorldMod.mergeLeft = true;
								}
								else if (num2 == -2 && num7 == num9 && num4 == num9 && num5 == -2)
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
									WorldMod.mergeUp = true;
									WorldMod.mergeRight = true;
								}
								else if (num2 == num9 && num7 == -2 && num4 == -2 && num5 == num9)
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
									WorldMod.mergeDown = true;
									WorldMod.mergeLeft = true;
								}
								else if (num2 == num9 && num7 == -2 && num4 == num9 && num5 == -2)
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
									WorldMod.mergeDown = true;
									WorldMod.mergeRight = true;
								}
								else if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == -2)
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
									WorldMod.mergeLeft = true;
									WorldMod.mergeRight = true;
								}
								else if (num2 == -2 && num7 == -2 && num4 == num9 && num5 == num9)
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
									WorldMod.mergeUp = true;
									WorldMod.mergeDown = true;
								}
								else if (num2 == -2 && num7 == num9 && num4 == -2 && num5 == -2)
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
									WorldMod.mergeUp = true;
									WorldMod.mergeLeft = true;
									WorldMod.mergeRight = true;
								}
								else if (num2 == num9 && num7 == -2 && num4 == -2 && num5 == -2)
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
									WorldMod.mergeDown = true;
									WorldMod.mergeLeft = true;
									WorldMod.mergeRight = true;
								}
								else if (num2 == -2 && num7 == -2 && num4 == num9 && num5 == -2)
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
									WorldMod.mergeUp = true;
									WorldMod.mergeDown = true;
									WorldMod.mergeRight = true;
								}
								else if (num2 == -2 && num7 == -2 && num4 == -2 && num5 == num9)
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
									WorldMod.mergeUp = true;
									WorldMod.mergeDown = true;
									WorldMod.mergeLeft = true;
								}
								else if (num2 == -2 && num7 == -2 && num4 == -2 && num5 == -2)
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
									WorldMod.mergeUp = true;
									WorldMod.mergeDown = true;
									WorldMod.mergeLeft = true;
									WorldMod.mergeRight = true;
								}
								else if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num9)
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
										WorldMod.mergeDown = true;
									}
									else if (num2 == -2 && num7 == -1 && num4 == num9 && num5 == num9)
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
										WorldMod.mergeUp = true;
									}
									else if (num2 == num9 && num7 == num9 && num4 == -1 && num5 == -2)
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
										WorldMod.mergeRight = true;
									}
									else if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == -1)
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
										WorldMod.mergeLeft = true;
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
										WorldMod.mergeUp = true;
									}
									else if (num7 == -2 && num2 == num9)
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
										WorldMod.mergeDown = true;
									}
								}
								else if (num2 != -1 && num7 != -1 && num4 == num9 && num5 == -1)
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
										WorldMod.mergeUp = true;
									}
									else if (num7 == -2 && num2 == num9)
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
										WorldMod.mergeDown = true;
									}
								}
								else if (num2 == -1 && num7 == num9 && num4 != -1 && num5 != -1)
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
									    WorldMod.mergeLeft = true;
								    }
								    else if (num5 == -2 && num4 == num9)
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
										WorldMod.mergeRight = true;
									}
							    }
							    else if (num2 == num9 && num7 == -1 && num4 != -1 && num5 != -1)
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
										WorldMod.mergeLeft = true;
									}
									else if (num5 == -2 && num4 == num9)
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
										WorldMod.mergeRight = true;
									}
								}
								else if (num2 != -1 && num7 != -1 && num4 == -1 && num5 == -1)
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
										WorldMod.mergeUp = true;
										WorldMod.mergeDown = true;
									}
									else if (num2 == -2)
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
										WorldMod.mergeUp = true;
									}
									else if (num7 == -2)
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
										WorldMod.mergeDown = true;
									}
								}
								else if (num2 == -1 && num7 == -1 && num4 != -1 && num5 != -1)
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
										WorldMod.mergeLeft = true;
										WorldMod.mergeRight = true;
									}
									else if (num4 == -2)
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
										WorldMod.mergeLeft = true;
									}
									else if (num5 == -2)
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
										WorldMod.mergeRight = true;
									}
								}
								else if (num2 == -2 && num7 == -1 && num4 == -1 && num5 == -1)
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
									WorldMod.mergeUp = true;
								}
								else if (num2 == -1 && num7 == -2 && num4 == -1 && num5 == -1)
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
									WorldMod.mergeDown = true;
								}
								else if (num2 == -1 && num7 == -1 && num4 == -2 && num5 == -1)
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
									WorldMod.mergeLeft = true;
								}
								else if (num2 == -1 && num7 == -1 && num4 == -1 && num5 == -2)
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
									WorldMod.mergeRight = true;
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
								else if (num6 != num9 && num8 != num9)
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
							    else if (num != num9 && num6 != num9)
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
								else if (num3 != num9 && num8 != num9)
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
							else if (num2 != num9 && num7 == num9 && (num4 == num9 & num5 == num9))
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
							else if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 == num9))
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
							else if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 == num9))
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
							else if (num2 == num9 && num7 == num9 && (num4 == num9 & num5 != num9))
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
							else if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 == num9))
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
							else if (num2 != num9 && num7 == num9 && (num4 == num9 & num5 != num9))
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
							else if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 == num9))
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
							else if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 != num9))
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
							else if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 != num9))
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
							else if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 == num9))
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
							else if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 != num9))
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
							else if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 != num9))
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
							else if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 == num9))
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
							else if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 != num9))
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
							else if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 != num9))
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
								WorldMod.KillTile(i, j, false, false, false);
							}
						}
						if (!WorldMod.noTileActions && (num9 == 53 || ((num9 == 59 || num9 == 57) && WorldMod.genRand.Next(5) == 0)))
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
									} else if (num9 == 57)
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
									WorldMod.SquareTileFrame(i, j, true);
								}
							}
						}
						if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
						{
							bool flag5 = WorldMod.mergeUp;
							bool flag6 = WorldMod.mergeDown;
							bool flag7 = WorldMod.mergeLeft;
							bool flag8 = WorldMod.mergeRight;
							WorldMod.TileFrame(i - 1, j, false, false);
							WorldMod.TileFrame(i + 1, j, false, false);
							WorldMod.TileFrame(i, j - 1, false, false);
							WorldMod.TileFrame(i, j + 1, false, false);
							WorldMod.mergeUp = flag5;
							WorldMod.mergeDown = flag6;
							WorldMod.mergeLeft = flag7;
							WorldMod.mergeRight = flag8;
						}
					}
				}
			}
		}
	}
}
