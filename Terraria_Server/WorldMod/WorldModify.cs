using System;
using System.IO;
using System.Diagnostics;
using Terraria_Server.Commands;
using Terraria_Server.Plugins;
using Terraria_Server.Misc;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.Logging;
using Terraria_Server.Networking;
using System.Threading;

namespace Terraria_Server.WorldMod
{
	public enum NPCMove_Result
	{
		NOT_VALID,
		REQUIRED_LIGHT,
		REQUIRED_DOOR,
		REQUIRED_CHAIR,
		REQUIRED_TABLE,
		REQUIRED_MULTIPLE, //Use the variables roomTorch etc
		NOT_SUITABLE,
		OCCUPIED,
		CORRUPTED,
		SUITABLE
	}

	public static class WorldModify
	{
		private const int RECTANGLE_OFFSET = 25;
		private const int TILE_OFFSET = 15;
		private const int TILES_OFFSET_2 = 10;
		private const int TILE_OFFSET_3 = 16;
		private const int TILE_OFFSET_4 = 23;
		private const int TILE_SCALE = 16;
		private const int TREE_RADIUS = 2;
		private const int MAX_TILE_SETS = 145;

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
				if (threadRand == null) threadRand = new Random((int)DateTime.Now.Ticks);
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

		//NPC Moving Checks
		public static bool roomChair { get; set; }
		public static bool roomDoor { get; set; }
		public static bool roomTable { get; set; }
		public static bool roomTorch { get; set; }

		public static bool roomOccupied { get; set; }
		public static bool roomEvil { get; set; }

		/* Pump/Wires */

		public const Int32 MAX_MECH = 1000;
		public const Int32 MAX_PUMP = 20;
		public const Int32 MAX_WIRE = 1000;

		public static int numMechs = 0;
		public static int numOutPump = 0;
		public static int numInPump = 0;
		public static int numWire = 0;
		public static int numNoWire = 0;

		public static int[] mechX = new int[MAX_MECH];
		public static int[] mechY = new int[MAX_MECH];
		public static int[] mechTime = new int[MAX_MECH];

		public static int[] inPumpY = new int[MAX_PUMP];
		public static int[] inPumpX = new int[MAX_PUMP];
		public static int[] outPumpX = new int[MAX_PUMP];
		public static int[] outPumpY = new int[MAX_PUMP];

		public static int[] wireX = new int[MAX_WIRE];
		public static int[] wireY = new int[MAX_WIRE];
		public static int[] noWireX = new int[MAX_WIRE];
		public static int[] noWireY = new int[MAX_WIRE];

		public static void Check1x1(int x, int y, int type)
		{
			if ((!Main.tile.At(x, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(x, y + 1).Type]))
				KillTile(x, y);
		}

		public static void CheckMan(int i, int j)
		{
			if (destroyObject)
				return;

			int num = j - (int)(Main.tile.At(i, j).FrameY / 18);
			int k;
			for (k = (int)Main.tile.At(i, j).FrameX; k >= 100; k -= 100)
			{
			}

			while (k >= 36)
				k -= 36;

			int num2 = i - k / 18;
			bool flag = false;
			for (int l = 0; l <= 1; l++)
			{
				for (int m = 0; m <= 2; m++)
				{
					int num3 = num2 + l;
					int num4 = num + m;
					int n;
					for (n = (int)Main.tile.At(num3, num4).FrameX; n >= 100; n -= 100)
					{
					}
					if (n >= 36)
						n -= 36;

					if (!Main.tile.At(num3, num4).Active || Main.tile.At(num3, num4).Type != 128 || 
						(int)Main.tile.At(num3, num4).FrameY != m * 18 || n != l * 18)
					{
						flag = true;
					}
				}
			}
			if (!SolidTile(num2, num + 3) || !SolidTile(num2 + 1, num + 3))
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				Item.NewItem(i * 16, j * 16, 32, 32, 498, 1, false, 0);
				for (int num5 = 0; num5 <= 1; num5++)
				{
					for (int num6 = 0; num6 <= 2; num6++)
					{
						int num7 = num2 + num5;
						int num8 = num + num6;
						if (Main.tile.At(num7, num8).Active && Main.tile.At(num7, num8).Type == 128)
						{
							KillTile(num7, num8, false, false, false);
						}
					}
				}
				destroyObject = false;
			}
		}

		public static void CheckMB(int i, int j, int type)
		{
			if (destroyObject)
				return;

			bool flag = false;
			int num = 0;
			int k;

			for (k = (int)(Main.tile.At(i, j).FrameY / 18); k >= 2; k -= 2)
				num++;

			int num2 = (int)(Main.tile.At(i, j).FrameX / 18);
			int num3 = 0;
			if (num2 >= 2)
			{
				num2 -= 2;
				num3++;
			}

			int num4 = i - num2;
			int num5 = j - k;
			for (int l = num4; l < num4 + 2; l++)
			{
				for (int m = num5; m < num5 + 2; m++)
				{
					if (!Main.tile.At(l, m).Active || (int)Main.tile.At(l, m).Type != type || 
						(int)Main.tile.At(l, m).FrameX != (l - num4) * 18 + num3 * 36 || 
						(int)Main.tile.At(l, m).FrameY != (m - num5) * 18 + num * 36)
					{
						flag = true;
					}
				}
				if (!SolidTile(l, num5 + 2))
					flag = true;
			}

			if (flag)
			{
				destroyObject = true;
				for (int n = num4; n < num4 + 2; n++)
				{
					for (int num6 = num5; num6 < num5 + 3; num6++)
					{
						if ((int)Main.tile.At(n, num6).Type == type && Main.tile.At(n, num6).Active)
							KillTile(n, num6);
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, 562 + num, 1, false, 0);
				for (int num7 = num4 - 1; num7 < num4 + 3; num7++)
				{
					for (int num8 = num5 - 1; num8 < num5 + 3; num8++)
						TileFrame(num7, num8);
				}
				destroyObject = false;
			}
		}

		public static void CheckOrb(int i, int j, int type)
		{
			if (!destroyObject)
			{
				int num = i;
				int num2 = j;

				if (Main.tile.At(i, j).FrameX == 0)
					num = i;
				else
					num = i - 1;

				if (Main.tile.At(i, j).FrameY == 0)
					num2 = j;
				else
					num2 = j - 1;

				if ((!Main.tile.At(num, num2).Active || (int)Main.tile.At(num, num2).Type != type || !Main.tile.At(num + 1, num2).Active ||
					(int)Main.tile.At(num + 1, num2).Type != type || !Main.tile.At(num, num2 + 1).Active ||
					(int)Main.tile.At(num, num2 + 1).Type != type || !Main.tile.At(num + 1, num2 + 1).Active ||
					(int)Main.tile.At(num + 1, num2 + 1).Type != type))
				{
					destroyObject = true;

					if ((int)Main.tile.At(num, num2).Type == type)
						KillTile(num, num2);

					if ((int)Main.tile.At(num + 1, num2).Type == type)
						KillTile(num + 1, num2);

					if ((int)Main.tile.At(num, num2 + 1).Type == type)
						KillTile(num, num2 + 1);

					if ((int)Main.tile.At(num + 1, num2 + 1).Type == type)
						KillTile(num + 1, num2 + 1);

					if (!noTileActions)
					{
						if (type == 12)
						{
							Item.NewItem(num * 16, num2 * 16, 32, 32, 29, 1, false, 0);
						}
						else if (type == 31)
						{
							if (genRand.Next(2) == 0)
								spawnMeteor = true;

							int num3 = Main.rand.Next(5);

							if (!shadowOrbSmashed)
								num3 = 0;

							if (num3 == 0)
							{
								Item.NewItem(num * 16, num2 * 16, 32, 32, 96, 1, false, -1);
								int stack = genRand.Next(25, 51);
								Item.NewItem(num * 16, num2 * 16, 32, 32, 97, stack, false, 0);
							}
							else
							{
								if (num3 == 1)
									Item.NewItem(num * 16, num2 * 16, 32, 32, 64, 1, false, -1);
								else if (num3 == 2)
									Item.NewItem(num * 16, num2 * 16, 32, 32, 162, 1, false, -1);
								else if (num3 == 3)
									Item.NewItem(num * 16, num2 * 16, 32, 32, 115, 1, false, -1);
								else if (num3 == 4)
									Item.NewItem(num * 16, num2 * 16, 32, 32, 111, 1, false, -1);
							}
							shadowOrbSmashed = true;
							shadowOrbCount++;
							if (shadowOrbCount >= 3)
							{
								shadowOrbCount = 0;
								float num4 = (float)(num * 16);
								float num5 = (float)(num2 * 16);
								float num6 = -1f;
								int num7 = 0;
								for (int k = 0; k < 255; k++)
								{
									float num8 = Math.Abs(Main.players[k].Position.X - num4) + Math.Abs(Main.players[k].Position.Y - num5);
									if (num8 < num6 || num6 == -1f)
									{
										num7 = 0;
										num6 = num8;
									}
								}
								if (Main.players[num7].zoneEvil)
									NPC.SpawnOnPlayer(Main.players[num7], num7, 13);
							}
							else
							{
								string text = "A horrible chill goes down your spine...";
								if (shadowOrbCount == 2)
									text = "Screams echo around you...";

								NetMessage.SendData(25, -1, -1, text, 255, 50f, 255f, 130f, 0);
							}
						}
					}
					destroyObject = false;
				}
			}
		}

		public static void CheckTree(int i, int j)
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
			int type = (int)Main.tile.At(i, j).Type;
			int frameX = (int)Main.tile.At(i, j).FrameX;
			int frameY = (int)Main.tile.At(i, j).FrameY;

			if (Main.tile.At(i - 1, j).Active)
			{
				num4 = (int)Main.tile.At(i - 1, j).Type;
			}
			if (Main.tile.At(i + 1, j).Active)
			{
				num5 = (int)Main.tile.At(i + 1, j).Type;
			}
			if (Main.tile.At(i, j - 1).Active)
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
			if (num7 == 23)
			{
				num7 = 2;
			}
			if (num7 == 60)
			{
				num7 = 2;
			}
			if (num7 == 109)
			{
				num7 = 2;
			}
			if (Main.tile.At(i, j).FrameX >= 22 && Main.tile.At(i, j).FrameX <= 44 && Main.tile.At(i, j).FrameY >= 132 && Main.tile.At(i, j).FrameY <= 176)
			{
				if (num7 != 2)
				{
					KillTile(i, j);
				}
				else if ((Main.tile.At(i, j).FrameX != 22 || num4 != type) && (Main.tile.At(i, j).FrameX != 44 || num5 != type))
				{
					KillTile(i, j);
				}
			}
			else
			{
				if ((Main.tile.At(i, j).FrameX == 88 && Main.tile.At(i, j).FrameY >= 0 && Main.tile.At(i, j).FrameY <= 44) || (Main.tile.At(i, j).FrameX == 66 && Main.tile.At(i, j).FrameY >= 66 && Main.tile.At(i, j).FrameY <= 130) || (Main.tile.At(i, j).FrameX == 110 && Main.tile.At(i, j).FrameY >= 66 && Main.tile.At(i, j).FrameY <= 110) || (Main.tile.At(i, j).FrameX == 132 && Main.tile.At(i, j).FrameY >= 0 && Main.tile.At(i, j).FrameY <= 176))
				{
					if (num4 == type && num5 == type)
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
						if (num4 == type)
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
							if (num5 == type)
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
					KillTile(i, j);
				}
				if (num4 != type && num5 != type)
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
					if (num4 != type)
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
						if (num5 != type)
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
			if ((Main.tile.At(i, j).FrameX == 66 && (Main.tile.At(i, j).FrameY == 0 || Main.tile.At(i, j).FrameY == 22 || Main.tile.At(i, j).FrameY == 44)) || (Main.tile.At(i, j).FrameX == 44 && (Main.tile.At(i, j).FrameY == 198 || Main.tile.At(i, j).FrameY == 220 || Main.tile.At(i, j).FrameY == 242)))
			{
				if (num5 != type)
				{
					KillTile(i, j);
				}
			}
			else
			{
				if ((Main.tile.At(i, j).FrameX == 88 && (Main.tile.At(i, j).FrameY == 66 || Main.tile.At(i, j).FrameY == 88 || Main.tile.At(i, j).FrameY == 110)) || (Main.tile.At(i, j).FrameX == 66 && (Main.tile.At(i, j).FrameY == 198 || Main.tile.At(i, j).FrameY == 220 || Main.tile.At(i, j).FrameY == 242)))
				{
					if (num4 != type)
					{
						KillTile(i, j);
					}
				}
				else
				{
					if (num7 == -1 || num7 == 23)
					{
						KillTile(i, j);
					}
					else
					{
						if (num2 != type && Main.tile.At(i, j).FrameY < 198 && ((Main.tile.At(i, j).FrameX != 22 && Main.tile.At(i, j).FrameX != 44) || Main.tile.At(i, j).FrameY < 132))
						{
							if (num4 == type || num5 == type)
							{
								if (num7 == type)
								{
									if (num4 == type && num5 == type)
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
										if (num4 == type)
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
											if (num5 == type)
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
									if (num4 == type && num5 == type)
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
										if (num4 == type)
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
											if (num5 == type)
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
			}
			if ((int)Main.tile.At(i, j).FrameX != frameX && (int)Main.tile.At(i, j).FrameY != frameY && frameX >= 0 && frameY >= 0)
			{
				TileFrame(i - 1, j, false, false);
				TileFrame(i + 1, j, false, false);
				TileFrame(i, j - 1, false, false);
				TileFrame(i, j + 1, false, false);
			}
		}

		public static void Place1x1(int x, int y, int type, int style = 0)
		{
			if (SolidTile(x, y + 1) && !Main.tile.At(x, y).Active)
			{
				Main.tile.At(x, y).SetActive(true);
				Main.tile.At(x, y).SetType((byte)type);
				if (type == 144)
				{
					Main.tile.At(x, y).SetFrameX((short)(style * 18));
					Main.tile.At(x, y).SetFrameY(0);
					return;
				}
				Main.tile.At(x, y).SetFrameY((short)(style * 18));
			}
		}

		public static void PlaceMan(int i, int j, int dir)
		{
			for (int k = i; k <= i + 1; k++)
			{
				for (int l = j - 2; l <= j; l++)
				{
					if (Main.tile.At(k, l).Active)
						return;
				}
			}
			if (!SolidTile(i, j + 1) || !SolidTile(i + 1, j + 1))
				return;

			byte b = 0;
			if (dir == 1)
				b = 36;

			Main.tile.At(i, j - 2).SetActive(true);
			Main.tile.At(i, j - 2).SetFrameY(0);
			Main.tile.At(i, j - 2).SetFrameX((short)b);
			Main.tile.At(i, j - 2).SetType(128);
			Main.tile.At(i, j - 1).SetActive(true);
			Main.tile.At(i, j - 1).SetFrameY(18);
			Main.tile.At(i, j - 1).SetFrameX((short)b);
			Main.tile.At(i, j - 1).SetType(128);
			Main.tile.At(i, j).SetActive(true);
			Main.tile.At(i, j).SetFrameY(36);
			Main.tile.At(i, j).SetFrameX((short)b);
			Main.tile.At(i, j).SetType(128);
			Main.tile.At(i + 1, j - 2).SetActive(true);
			Main.tile.At(i + 1, j - 2).SetFrameY(0);
			Main.tile.At(i + 1, j - 2).SetFrameX((short)(18 + b));
			Main.tile.At(i + 1, j - 2).SetType(128);
			Main.tile.At(i + 1, j - 1).SetActive(true);
			Main.tile.At(i + 1, j - 1).SetFrameY(18);
			Main.tile.At(i + 1, j - 1).SetFrameX((short)(18 + b));
			Main.tile.At(i + 1, j - 1).SetType(128);
			Main.tile.At(i + 1, j).SetActive(true);
			Main.tile.At(i + 1, j).SetFrameY(36);
			Main.tile.At(i + 1, j).SetFrameX((short)(18 + b));
			Main.tile.At(i + 1, j).SetType(128);
		}

		public static void PlaceMB(int X, int y, int type, int style)
		{
			int num = X + 1;
			if (num < 5 || num > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = num - 1; i < num + 1; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (Main.tile.At(i, j).Active)
					{
						flag = false;
					}
				}
				if (!Main.tile.At(i, y + 1).Active || (!Main.tileSolid[(int)Main.tile.At(i, y + 1).Type] &&
					!Main.tileTable[(int)Main.tile.At(i, y + 1).Type]))
				{
					flag = false;
				}
			}
			if (flag)
			{
				Main.tile.At(num - 1, y - 1).SetActive(true);
				Main.tile.At(num - 1, y - 1).SetFrameY((short)(style * 36));
				Main.tile.At(num - 1, y - 1).SetFrameX(0);
				Main.tile.At(num - 1, y - 1).SetType((byte)type);
				Main.tile.At(num, y - 1).SetActive(true);
				Main.tile.At(num, y - 1).SetFrameY((short)(style * 36));
				Main.tile.At(num, y - 1).SetFrameX(18);
				Main.tile.At(num, y - 1).SetType((byte)type);
				Main.tile.At(num - 1, y).SetActive(true);
				Main.tile.At(num - 1, y).SetFrameY((short)(style * 36 + 18));
				Main.tile.At(num - 1, y).SetFrameX(0);
				Main.tile.At(num - 1, y).SetType((byte)type);
				Main.tile.At(num, y).SetActive(true);
				Main.tile.At(num, y).SetFrameY((short)(style * 36 + 18));
				Main.tile.At(num, y).SetFrameX(18);
				Main.tile.At(num, y).SetType((byte)type);
			}
		}

		public static bool placeTrap(int x2, int y2, int type = -1)
		{
			int num = y2;
			while (!SolidTile(x2, num))
			{
				num++;
				if (num >= Main.maxTilesY - 300)
				{
					return false;
				}
			}
			num--;
			if (Main.tile.At(x2, num).Liquid > 0 && Main.tile.At(x2, num).Lava)
			{
				return false;
			}
			if (type == -1 && Main.rand.Next(20) == 0)
			{
				type = 2;
			}
			else
			{
				if (type == -1)
				{
					type = Main.rand.Next(2);
				}
			}
			if (Main.tile.At(x2, num).Active || Main.tile.At(x2 - 1, num).Active || Main.tile.At(x2 + 1, num).Active || Main.tile.At(x2, num - 1).Active || Main.tile.At(x2 - 1, num - 1).Active || Main.tile.At(x2 + 1, num - 1).Active || Main.tile.At(x2, num - 2).Active || Main.tile.At(x2 - 1, num - 2).Active || Main.tile.At(x2 + 1, num - 2).Active)
			{
				return false;
			}
			if (Main.tile.At(x2, num + 1).Type == 48)
			{
				return false;
			}
			if (type == 0)
			{
				int num2 = x2;
				int num3 = num;
				num3 -= genRand.Next(3);
				while (!SolidTile(num2, num3))
				{
					num2--;
				}
				int num4 = num2;
				num2 = x2;
				while (!SolidTile(num2, num3))
				{
					num2++;
				}
				int num5 = num2;
				int num6 = x2 - num4;
				int num7 = num5 - x2;
				bool flag = false;
				bool flag2 = false;
				if (num6 > 5 && num6 < 50)
				{
					flag = true;
				}
				if (num7 > 5 && num7 < 50)
				{
					flag2 = true;
				}
				if (flag && !SolidTile(num4, num3 + 1))
				{
					flag = false;
				}
				if (flag2 && !SolidTile(num5, num3 + 1))
				{
					flag2 = false;
				}
				if (flag && (Main.tile.At(num4, num3).Type == 10 || Main.tile.At(num4, num3).Type == 48 || Main.tile.At(num4, num3 + 1).Type == 10 || Main.tile.At(num4, num3 + 1).Type == 48))
				{
					flag = false;
				}
				if (flag2 && (Main.tile.At(num5, num3).Type == 10 || Main.tile.At(num5, num3).Type == 48 || Main.tile.At(num5, num3 + 1).Type == 10 || Main.tile.At(num5, num3 + 1).Type == 48))
				{
					flag2 = false;
				}
				int style = 0;
				if (flag && flag2)
				{
					style = 1;
					num2 = num4;
					if (genRand.Next(2) == 0)
					{
						num2 = num5;
						style = -1;
					}
				}
				else
				{
					if (flag2)
					{
						num2 = num5;
						style = -1;
					}
					else
					{
						if (!flag)
						{
							return false;
						}
						num2 = num4;
						style = 1;
					}
				}
				if (Main.tile.At(x2, num).Wall > 0)
				{
					PlaceTile(x2, num, 135, true, true, -1, 2);
				}
				else
				{
					PlaceTile(x2, num, 135, true, true, -1, genRand.Next(2, 4));
				}
				KillTile(num2, num3, false, false, false);
				PlaceTile(num2, num3, 137, true, true, -1, style);
				int num8 = x2;
				int num9 = num;
				while (num8 != num2 || num9 != num3)
				{
					Main.tile.At(num8, num9).SetWire(true);
					if (num8 > num2)
					{
						num8--;
					}
					if (num8 < num2)
					{
						num8++;
					}
					Main.tile.At(num8, num9).SetWire(true);
					if (num9 > num3)
					{
						num9--;
					}
					if (num9 < num3)
					{
						num9++;
					}
					Main.tile.At(num8, num9).SetWire(true);
				}
				return true;
			}
			if (type != 1)
			{
				if (type == 2)
				{
					int num10 = Main.rand.Next(4, 7);
					int num11 = x2 + Main.rand.Next(-1, 2);
					int num12 = num;
					for (int i = 0; i < num10; i++)
					{
						num12++;
						if (!SolidTile(num11, num12))
						{
							return false;
						}
					}
					for (int j = num11 - 2; j <= num11 + 2; j++)
					{
						for (int k = num12 - 2; k <= num12 + 2; k++)
						{
							if (!SolidTile(j, k))
							{
								return false;
							}
						}
					}
					KillTile(num11, num12, false, false, false);
					Main.tile.At(num11, num12).SetActive(true);
					Main.tile.At(num11, num12).SetType(141);
					Main.tile.At(num11, num12).SetFrameX(0);
					Main.tile.At(num11, num12).SetFrameY((short)(18 * Main.rand.Next(2)));
					PlaceTile(x2, num, 135, true, true, -1, genRand.Next(2, 4));
					int num13 = x2;
					int num14 = num;
					while (num13 != num11 || num14 != num12)
					{
						Main.tile.At(num13, num14).SetWire(true);
						if (num13 > num11)
						{
							num13--;
						}
						if (num13 < num11)
						{
							num13++;
						}
						Main.tile.At(num13, num14).SetWire(true);
						if (num14 > num12)
						{
							num14--;
						}
						if (num14 < num12)
						{
							num14++;
						}
						Main.tile.At(num13, num14).SetWire(true);
					}
				}
				return false;
			}
			int num15 = num - 8;
			int num16 = x2 + genRand.Next(-1, 2);
			bool flag3 = true;
			while (flag3)
			{
				bool flag4 = true;
				int num17 = 0;
				for (int l = num16 - 2; l <= num16 + 3; l++)
				{
					for (int m = num15; m <= num15 + 3; m++)
					{
						if (!SolidTile(l, m))
						{
							flag4 = false;
						}
						if (Main.tile.At(l, m).Active && (Main.tile.At(l, m).Type == 0 || Main.tile.At(l, m).Type == 1 || Main.tile.At(l, m).Type == 59))
						{
							num17++;
						}
					}
				}
				num15--;
				if ((double)num15 < Main.worldSurface)
				{
					return false;
				}
				if (flag4 && num17 > 2)
				{
					flag3 = false;
				}
			}
			if (num - num15 <= 5 || num - num15 >= 40)
			{
				return false;
			}
			for (int n = num16; n <= num16 + 1; n++)
			{
				for (int num18 = num15; num18 <= num; num18++)
				{
					if (SolidTile(n, num18))
					{
						KillTile(n, num18, false, false, false);
					}
				}
			}
			for (int num19 = num16 - 2; num19 <= num16 + 3; num19++)
			{
				for (int num20 = num15 - 2; num20 <= num15 + 3; num20++)
				{
					if (SolidTile(num19, num20))
					{
						Main.tile.At(num19, num20).SetType(1);
					}
				}
			}
			PlaceTile(x2, num, 135, true, true, -1, genRand.Next(2, 4));
			PlaceTile(num16, num15 + 2, 130, true, false, -1, 0);
			PlaceTile(num16 + 1, num15 + 2, 130, true, false, -1, 0);
			PlaceTile(num16 + 1, num15 + 1, 138, true, false, -1, 0);
			num15 += 2;
			Main.tile.At(num16, num15).SetWire(true);
			Main.tile.At(num16 + 1, num15).SetWire(true);
			num15++;
			PlaceTile(num16, num15, 130, true, false, -1, 0);
			PlaceTile(num16 + 1, num15, 130, true, false, -1, 0);
			Main.tile.At(num16, num15).SetWire(true);
			Main.tile.At(num16 + 1, num15).SetWire(true);
			PlaceTile(num16, num15 + 1, 130, true, false, -1, 0);
			PlaceTile(num16 + 1, num15 + 1, 130, true, false, -1, 0);
			Main.tile.At(num16, num15 + 1).SetWire(true);
			Main.tile.At(num16 + 1, num15 + 1).SetWire(true);
			int num21 = x2;
			int num22 = num;
			while (num21 != num16 || num22 != num15)
			{
				Main.tile.At(num21, num22).SetWire(true);
				if (num21 > num16)
				{
					num21--;
				}
				if (num21 < num16)
				{
					num21++;
				}
				Main.tile.At(num21, num22).SetWire(true);
				if (num22 > num15)
				{
					num22--;
				}
				if (num22 < num15)
				{
					num22++;
				}
				Main.tile.At(num21, num22).SetWire(true);
			}
			return true;
		}

		public static void SwitchMB(int i, int j)
		{
			int k;
			for (k = (int)(Main.tile.At(i, j).FrameY / 18); k >= 2; k -= 2)
			{
			}
			int num = (int)(Main.tile.At(i, j).FrameX / 18);
			if (num >= 2)
				num -= 2;

			int num2 = i - num;
			int num3 = j - k;
			for (int l = num2; l < num2 + 2; l++)
			{
				for (int m = num3; m < num3 + 2; m++)
				{
					if (Main.tile.At(l, m).Active && Main.tile.At(l, m).Type == 139)
					{
						if (Main.tile.At(l, m).FrameX < 36)
						{
							short frameX = (short)(Main.tile.At(l, m).FrameX + 36);
							Main.tile.At(l, m).SetFrameX(frameX);
						}
						else
						{
							short frameX = (short)(Main.tile.At(l, m).FrameX - 36);
							Main.tile.At(l, m).SetFrameX(frameX);
						}
						noWireX[numNoWire] = l;
						noWireY[numNoWire] = m;
						numNoWire++;
					}
				}
			}
			NetMessage.SendTileSquare(-1, num2, num3, 3);
		}

		public static void UpdateMech(ISender Sender)
		{
			for (int i = numMechs - 1; i >= 0; i--)
			{
				mechTime[i]--;
				if (Main.tile.At(mechX[i], mechY[i]).Active && Main.tile.At(mechX[i], mechY[i]).Type == 144)
				{
					if (Main.tile.At(mechX[i], mechY[i]).FrameY == 0)
					{
						mechTime[i] = 0;
					}
					else
					{
						int num = (int)(Main.tile.At(mechX[i], mechY[i]).FrameX / 18);
						if (num == 0)
							num = 60;
						else if (num == 1)
							num = 180;
						else if (num == 2)
							num = 300;

						if (Math.IEEERemainder((double)mechTime[i], (double)num) == 0.0)
						{
							mechTime[i] = 18000;
							TripWire(mechX[i], mechY[i], Sender);
						}
					}
				}
				if (mechTime[i] <= 0)
				{
					if (Main.tile.At(mechX[i], mechY[i]).Active && Main.tile.At(mechX[i], mechY[i]).Type == 144)
					{
						Main.tile.At(mechX[i], mechY[i]).SetFrameY(0);
						NetMessage.SendTileSquare(-1, mechX[i], mechY[i], 1);
					}
					for (int j = i; j < numMechs; j++)
					{
						mechX[j] = mechX[j + 1];
						mechY[j] = mechY[j + 1];
						mechTime[j] = mechTime[j + 1];
					}
					numMechs--;
				}
			}
		}

		public static bool checkMech(int i, int j, int time)
		{
			for (int k = 0; k < numMechs; k++)
			{
				if (mechX[k] == i && mechY[k] == j)
				{
					return false;
				}
			}
			if (numMechs < MAX_MECH - 1)
			{
				mechX[numMechs] = i;
				mechY[numMechs] = j;
				mechTime[numMechs] = time;
				numMechs++;
				return true;
			}
			return false;
		}

		public static void hitSwitch(int i, int j, ISender Sender)
		{
			if (Main.tile.At(i, j).Type == 135)
			{
				TripWire(i, j, Sender);
				return;
			}
			if (Main.tile.At(i, j).Type == 136)
			{
				if (Main.tile.At(i, j).FrameY == 0)
					Main.tile.At(i, j).SetFrameY(8);
				else
					Main.tile.At(i, j).SetFrameY(0);
				TripWire(i, j, Sender);
				return;
			}
			if (Main.tile.At(i, j).Type == 144)
			{
				if (Main.tile.At(i, j).FrameY == 0)
				{
					Main.tile.At(i, j).SetFrameY(18);
					checkMech(i, j, 18000);
				}
				else
					Main.tile.At(i, j).SetFrameY(0);

				return;
			}
			if (Main.tile.At(i, j).Type == 132)
			{
				int num = i;
				short num2 = 36;
				num = (int)(Main.tile.At(i, j).FrameX / 18 * -1);
				int num3 = (int)(Main.tile.At(i, j).FrameY / 18 * -1);
				if (num < -1)
				{
					num += 2;
					num2 = -36;
				}
				num += i;
				num3 += j;
				for (int k = num; k < num + 2; k++)
				{
					for (int l = num3; l < num3 + 2; l++)
					{
						if (Main.tile.At(k, l).Type == 132)
						{
							short frameX = (short)(Main.tile.At(k, l).FrameX + num2);
							Main.tile.At(k, l).SetFrameX(frameX);
						}
					}
				}
				TileFrame(num, num3, false, false);
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num3; n < num3 + 2; n++)
					{
						if (Main.tile.At(m, n).Type == 132 && Main.tile.At(m, n).Active && Main.tile.At(m, n).Wire)
						{
							TripWire(m, n, Sender);
							return;
						}
					}
				}
			}
		}

		public static void TripWire(int i, int j, ISender Sender)
		{
			numWire = 0;
			numNoWire = 0;
			numInPump = 0;
			numOutPump = 0;
			noWire(i, j);
			hitWire(i, j, Sender);

			if (numInPump > 0 && numOutPump > 0)
				xferWater();
		}

		public static void xferWater()
		{
			for (int i = 0; i < numInPump; i++)
			{
				int num = inPumpX[i];
				int num2 = inPumpY[i];
				int liquid = (int)Main.tile.At(num, num2).Liquid;
				if (liquid > 0)
				{
					bool lava = Main.tile.At(num, num2).Lava;
					for (int j = 0; j < numOutPump; j++)
					{
						int num3 = outPumpX[j];
						int num4 = outPumpY[j];
						int liquid2 = (int)Main.tile.At(num3, num4).Liquid;
						if (liquid2 < 255)
						{
							bool flag = Main.tile.At(num3, num4).Lava;
							if (liquid2 == 0)
								flag = lava;

							if (lava == flag)
							{
								int num5 = liquid;
								if (num5 + liquid2 > 255)
									num5 = 255 - liquid2;

								short frameX = (short)(Main.tile.At(num3, num4).FrameX + (byte)num5);
								Main.tile.At(num3, num4).SetFrameX(frameX);

								byte lqd = (byte)(Main.tile.At(num, num2).Liquid - (byte)num5);
								Main.tile.At(num, num2).SetLiquid(lqd);


								liquid = (int)Main.tile.At(num, num2).Liquid;
								Main.tile.At(num3, num4).SetLava(lava);
								SquareTileFrame(num3, num4, true);
								if (Main.tile.At(num, num2).Liquid == 0)
								{
									Main.tile.At(num, num2).SetLava(false);
									SquareTileFrame(num, num2, true);
									break;
								}
							}
						}
					}
					SquareTileFrame(num, num2, true);
				}
			}
		}

		public static void noWire(int i, int j)
		{
			if (numNoWire >= MAX_WIRE - 1)
				return;

			noWireX[numNoWire] = i;
			noWireY[numNoWire] = j;
			numNoWire++;
		}

		public static void hitWire(int i, int j, ISender Sender)
		{
			if (numWire >= MAX_WIRE - 1)
				return;

			if (!Main.tile.At(i, j).Wire)
				return;

			for (int k = 0; k < numWire; k++)
			{
				if (wireX[k] == i && wireY[k] == j)
					return;
			}

			wireX[numWire] = i;
			wireY[numWire] = j;
			numWire++;
			int type = (int)Main.tile.At(i, j).Type;
			bool flag = true;
			for (int l = 0; l < numNoWire; l++)
			{
				if (noWireX[l] == i && noWireY[l] == j)
					flag = false;
			}

			if (flag && Main.tile.At(i, j).Active)
			{
				short frameX;
				switch (type)
				{
					case 144:
						hitSwitch(i, j, Sender);
						SquareTileFrame(i, j, true);
						NetMessage.SendTileSquare(-1, i, j, 1);
						break;
					case 130:
						Main.tile.At(i, j).SetType(131);
						SquareTileFrame(i, j, true);
						NetMessage.SendTileSquare(-1, i, j, 1);
						break;
					case 131:
						Main.tile.At(i, j).SetType(130);
						SquareTileFrame(i, j, true);
						NetMessage.SendTileSquare(-1, i, j, 1);
						break;
					case 11:
						if (CloseDoor(i, j, true, Sender))
							NetMessage.SendData(19, -1, -1, "", 1, (float)i, (float)j, 0f, 0);
						break;
					case 10:
						int direction = 1;
						if (Main.rand.Next(2) == 0)
							direction = -1;
						if (!OpenDoor(i, j, direction, Sender))
						{
							if (OpenDoor(i, j, -direction, Sender))
								NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)(-(float)direction), 0);
						}
						else
							NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)direction, 0);
						break;
					case 4:
						var tile = Main.tile.At(i, j);
						if (tile.FrameX < 66)
						{
							frameX = (short)(tile.FrameX + 66);
							Main.tile.At(i, j).SetFrameX(frameX);
						}
						else
						{
							frameX = (short)(tile.FrameX - 66);
							Main.tile.At(i, j).SetFrameX(frameX);
						}
						NetMessage.SendTileSquare(-1, i, j, 1);
						break;
					case 42:
						int num2 = j - (int)(Main.tile.At(i, j).FrameY / 18);
						short num3 = 18;

						if (Main.tile.At(i, j).FrameX > 0)
							num3 = -18;

						frameX = (short)(Main.tile.At(i, num2).FrameX + num3);
						Main.tile.At(i, num2).SetFrameX(frameX);

						frameX = (short)(Main.tile.At(i, num2 + 1).FrameX + num3);
						Main.tile.At(i, num2 + 1).SetFrameX(frameX);

						noWire(i, num2);
						noWire(i, num2 + 1);
						NetMessage.SendTileSquare(-1, i, j, 2);
						break;
					case 93:
						int num4 = j - (int)(Main.tile.At(i, j).FrameX / 18);
						short num5 = 18;

						if (Main.tile.At(i, j).FrameX > 0)
							num5 = -18;

						frameX = (short)(Main.tile.At(i, num4).FrameX + num5);
						Main.tile.At(i, num4).SetFrameX(frameX);

						frameX = (short)(Main.tile.At(i, num4 + 1).FrameX + num5);
						Main.tile.At(i, num4 + 1).SetFrameX(frameX);

						frameX = (short)(Main.tile.At(i, num4 + 2).FrameX + num5);
						Main.tile.At(i, num4 + 2).SetFrameX(frameX);

						noWire(i, num4);
						noWire(i, num4 + 1);
						noWire(i, num4 + 2);
						NetMessage.SendTileSquare(-1, i, num4 + 1, 3);
						break;
					case 33:
						short num12 = 18;
						if (Main.tile.At(i, j).FrameX > 0)
							num12 = -18;

						frameX = (short)(Main.tile.At(i, j).FrameX + num12);
						Main.tile.At(i, j).SetFrameX(frameX);
						NetMessage.SendTileSquare(-1, i, j, 3);
						break;
					case 92:
						int num13 = j - (int)(Main.tile.At(i, j).FrameY / 18);
						short num14 = 18;
						if (Main.tile.At(i, j).FrameX > 0)
							num14 = -18;

						frameX = (short)(Main.tile.At(i, num13).FrameX + num14);
						Main.tile.At(i, num13).SetFrameX(frameX);

						frameX = (short)(Main.tile.At(i, num13 + 1).FrameX + num14);
						Main.tile.At(i, num13 + 1).SetFrameX(frameX);

						frameX = (short)(Main.tile.At(i, num13 + 2).FrameX + num14);
						Main.tile.At(i, num13 + 2).SetFrameX(frameX);

						frameX = (short)(Main.tile.At(i, num13 + 3).FrameX + num14);
						Main.tile.At(i, num13 + 3).SetFrameX(frameX);

						frameX = (short)(Main.tile.At(i, num13 + 4).FrameX + num14);
						Main.tile.At(i, num13 + 4).SetFrameX(frameX);

						frameX = (short)(Main.tile.At(i, num13 + 5).FrameX + num14);
						Main.tile.At(i, num13 + 5).SetFrameX(frameX);

						noWire(i, num13);
						noWire(i, num13 + 1);
						noWire(i, num13 + 2);
						noWire(i, num13 + 3);
						noWire(i, num13 + 4);
						noWire(i, num13 + 5);
						NetMessage.SendTileSquare(-1, i, num13 + 3, 7);
						break;
					case 137:
						if (checkMech(i, j, 180))
						{
							int num15 = -1;
							if (Main.tile.At(i, j).FrameY != 0)
								num15 = 1;

							float speedX = (float)(12 * num15);
							int damage = 20;
							int type2 = 98;
							Vector2 vector = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 7));
							vector.X += (float)(10 * num15);
							vector.Y += 2f;
							Projectile.NewProjectile((float)((int)vector.X), (float)((int)vector.Y), speedX, 0f, type2, damage, 2f, Main.myPlayer);
						}
						break;
					case 139:
						SwitchMB(i, j);
						break;
					case 141:
						KillTile(i, j, false, false, true);
						NetMessage.SendTileSquare(-1, i, j, 1);
						Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0f, 108, 250, 10f, Main.myPlayer);
						break;
					case 105:
						{
							int num24 = j - (int)(Main.tile.At(i, j).FrameY / 18);
							int num25 = (int)(Main.tile.At(i, j).FrameY / 18);
							int num26 = 0;
							while (num25 >= 2)
							{
								num25 -= 2;
								num26++;
							}
							num25 = i - num25;
							noWire(num25, num24);
							noWire(num25, num24 + 1);
							noWire(num25, num24 + 2);
							noWire(num25 + 1, num24);
							noWire(num25 + 1, num24 + 1);
							noWire(num25 + 1, num24 + 2);
							int num27 = num25 * 16 + 16;
							int num28 = (num24 + 3) * 16;
							int num29 = -1;
							switch (num26)
							{
								case 4:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 1))
										num29 = NPC.NewNPC(num27, num28 - 12, 1);
									break;
								case 7:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 49))
										num29 = NPC.NewNPC(num27 - 4, num28 - 6, 49);
									break;
								case 8:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 55))
										num29 = NPC.NewNPC(num27, num28 - 12, 55);
									break;
								case 9:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 46))
										num29 = NPC.NewNPC(num27, num28 - 12, 46);
									break;
								case 10:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 21))
										num29 = NPC.NewNPC(num27, num28, 21);
									break;
								case 18:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 67))
										num29 = NPC.NewNPC(num27, num28 - 12, 67);
									break;
								case 23:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 63))
										num29 = NPC.NewNPC(num27, num28 - 12, 63);
									break;
								case 27:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 85))
										num29 = NPC.NewNPC(num27 - 9, num28, 85);
									break;
								case 28:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 74))
										num29 = NPC.NewNPC(num27, num28 - 12, 74);
									break;
								case 42:
									if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 58))
										num29 = NPC.NewNPC(num27, num28 - 12, 58);
									break;
								case 37:
									if (checkMech(i, j, 600) && Item.MechSpawn((float)num27, (float)num28, 58))
										Item.NewItem(num27, num28 - 16, 0, 0, 58);
									break;
								case 2:
									if (checkMech(i, j, 600) && Item.MechSpawn((float)num27, (float)num28, 184))
										Item.NewItem(num27, num28 - 16, 0, 0, 184);
									break;
								case 17:
									if (checkMech(i, j, 600) && Item.MechSpawn((float)num27, (float)num28, 166))
										Item.NewItem(num27, num28 - 20, 0, 0, 166);
									break;
								case 40:
									if (checkMech(i, j, 300))
									{
										int[] array = new int[10];
										int num30 = 0;
										for (int num31 = 0; num31 < 200; num31++)
										{
											if (Main.npcs[num31].Active &&
												(Main.npcs[num31].Type == 17 || Main.npcs[num31].Type == 19 ||
												Main.npcs[num31].Type == 22 || Main.npcs[num31].Type == 38 ||
												Main.npcs[num31].Type == 54 || Main.npcs[num31].Type == 107 ||
												Main.npcs[num31].Type == 108))
											{
												array[num30] = num31;
												num30++;
												if (num30 >= 9)
												{
													break;
												}
											}
										}
										if (num30 > 0)
										{
											int num32 = array[Main.rand.Next(num30)];
											Main.npcs[num32].Position.X = (float)(num27 - Main.npcs[num32].Width / 2);
											Main.npcs[num32].Position.Y = (float)(num28 - Main.npcs[num32].Height - 1);
											NetMessage.SendData(23, -1, -1, "", num32, 0f, 0f, 0f, 0);
										}
									}
									break;
								case 41:
									if (checkMech(i, j, 300))
									{
										int[] array2 = new int[10];
										int num33 = 0;
										for (int num34 = 0; num34 < 200; num34++)
										{
											if (Main.npcs[num34].Active && (Main.npcs[num34].Type == 18 || Main.npcs[num34].Type == 20 || Main.npcs[num34].Type == 124))
											{
												array2[num33] = num34;
												num33++;
												if (num33 >= 9)
												{
													break;
												}
											}
										}
										if (num33 > 0)
										{
											int num35 = array2[Main.rand.Next(num33)];
											Main.npcs[num35].Position.X = (float)(num27 - Main.npcs[num35].Width / 2);
											Main.npcs[num35].Position.Y = (float)(num28 - Main.npcs[num35].Height - 1);
											NetMessage.SendData(23, -1, -1, "", num35, 0f, 0f, 0f, 0);
										}
									}
									break;
							}

							if (num29 >= 0)
							{
								Main.npcs[num29].value = 0f;
								//Main.npcs[num29].npcSlots = 0f;
							}


							if (type == 126 || type == 100 || type == 95)
							{
								int num6 = j - (int)(Main.tile.At(i, j).FrameY / 18);
								int num7 = (int)(Main.tile.At(i, j).FrameX / 18);
								if (num7 > 1)
								{
									num7 -= 2;
								}
								num7 = i - num7;
								short num8 = 36;
								if (Main.tile.At(num7, num6).FrameX > 0)
								{
									num8 = -36;
								}

								frameX = (short)(Main.tile.At(num7, num6).FrameX + num8);
								Main.tile.At(num7, num6).SetFrameX(frameX);

								frameX = (short)(Main.tile.At(num7, num6 + 1).FrameX + num8);
								Main.tile.At(num7, num6 + 1).SetFrameX(frameX);

								frameX = (short)(Main.tile.At(num7 + 1, num6).FrameX + num8);
								Main.tile.At(num7 + 1, num6).SetFrameX(frameX);

								frameX = (short)(Main.tile.At(num7 + 1, num6 + 1).FrameX + num8);
								Main.tile.At(num7 + 1, num6 + 1).SetFrameX(frameX);


								noWire(num7, num6);
								noWire(num7, num6 + 1);
								noWire(num7 + 1, num6);
								noWire(num7 + 1, num6 + 1);
								NetMessage.SendTileSquare(-1, num7, num6, 3);
							}
							else if (type == 34 || type == 35 || type == 36)
							{
								int num9 = j - (int)(Main.tile.At(i, j).FrameY / 18);
								int num10 = (int)(Main.tile.At(i, j).FrameX / 18);

								if (num10 > 2)
									num10 -= 3;

								num10 = i - num10;
								short num11 = 54;
								if (Main.tile.At(num10, num9).FrameX > 0)
									num11 = -54;

								for (int m = num10; m < num10 + 3; m++)
								{
									for (int n = num9; n < num9 + 3; n++)
									{
										frameX = (short)(Main.tile.At(m, n).FrameX + num11);
										Main.tile.At(m, n).SetFrameX(frameX);
										noWire(m, n);
									}
								}
								NetMessage.SendTileSquare(-1, num10 + 1, num9 + 1, 3);
							}
							else if (type == 142 || type == 143)
							{
								int num16 = j - (int)(Main.tile.At(i, j).FrameY / 18);
								int num17 = (int)(Main.tile.At(i, j).FrameX / 18);
								if (num17 > 1)
									num17 -= 2;

								num17 = i - num17;
								noWire(num17, num16);
								noWire(num17, num16 + 1);
								noWire(num17 + 1, num16);
								noWire(num17 + 1, num16 + 1);
								if (type == 142)
								{
									int num18 = num17;
									int num19 = num16;
									for (int num20 = 0; num20 < 4; num20++)
									{
										if (numInPump >= MAX_PUMP - 1)
											break;

										if (num20 == 0)
										{
											num18 = num17;
											num19 = num16 + 1;
										}
										else if (num20 == 1)
										{
											num18 = num17 + 1;
											num19 = num16 + 1;
										}
										else if (num20 == 2)
										{
											num18 = num17;
											num19 = num16;
										}
										else
										{
											num18 = num17 + 1;
											num19 = num16;
										}
										inPumpX[numInPump] = num18;
										inPumpY[numInPump] = num19;
										numInPump++;
									}
								}
								else
								{
									int num21 = num17;
									int num22 = num16;
									for (int num23 = 0; num23 < 4; num23++)
									{
										if (numOutPump >= MAX_PUMP - 1)
										{
											break;
										}
										if (num23 == 0)
										{
											num21 = num17;
											num22 = num16 + 1;
										}
										else if (num23 == 1)
										{
											num21 = num17 + 1;
											num22 = num16 + 1;
										}
										else if (num23 == 2)
										{
											num21 = num17;
											num22 = num16;
										}
										else
										{
											num21 = num17 + 1;
											num22 = num16;
										}
										outPumpX[numOutPump] = num21;
										outPumpY[numOutPump] = num22;
										numOutPump++;
									}
								}
							}
							break;
						}
				}
			}

			hitWire(i - 1, j, Sender);
			hitWire(i + 1, j, Sender);
			hitWire(i, j - 1, Sender);
			hitWire(i, j + 1, Sender);
		}

		//Client code i think, But might still work if a Plugin uses it.
		public static NPCMove_Result MoveNPC(int x, int y, int n)
		{
			if (!StartRoomCheck(x, y))
				return NPCMove_Result.NOT_VALID;

			if (!RoomNeeds(spawnNPC))
			{
				var moveResult = NPCMove_Result.REQUIRED_MULTIPLE;
				var amount = 0;

				if (!roomTorch)
				{
					amount++;
					moveResult = NPCMove_Result.REQUIRED_LIGHT;
				}
				if (!roomDoor)
				{
					amount++;
					moveResult = NPCMove_Result.REQUIRED_DOOR;
				}
				if (!roomTable)
				{
					amount++;
					moveResult = NPCMove_Result.REQUIRED_TABLE;
				}
				if (!roomChair)
				{
					amount++;
					moveResult = NPCMove_Result.REQUIRED_CHAIR;
				}
				if (amount > 1)
					return NPCMove_Result.REQUIRED_MULTIPLE;

				return moveResult;
			}

			ScoreRoom(-1);

			if (hiScore <= 0)
			{
				if (roomOccupied)
					return NPCMove_Result.OCCUPIED;
				else if (roomEvil)
					return NPCMove_Result.CORRUPTED;
				else
					return NPCMove_Result.NOT_SUITABLE;
			}

			return NPCMove_Result.SUITABLE;
		}

		public static void MoveRoom(int x, int y, int NPCIndex)
		{
			spawnNPC = Main.npcs[NPCIndex].Type;
			Main.npcs[NPCIndex].homeless = true;
			SpawnNPC(x, y);
		}

		public static bool RoomNeeds(int npcType)
		{
			roomChair = houseTile[15] || houseTile[79] || houseTile[89] || houseTile[102];
			roomDoor = houseTile[10] || houseTile[11] || houseTile[19];
			roomTable = houseTile[14] || houseTile[18] || houseTile[87] || houseTile[88] || houseTile[90] || houseTile[101];
			roomTorch = houseTile[4] || houseTile[33] || houseTile[34] || houseTile[35] || houseTile[36] || houseTile[42] || houseTile[49] || houseTile[93] || houseTile[95] || houseTile[98] || houseTile[100];

			canSpawn = roomChair && roomTable && roomDoor && roomTorch;

			return canSpawn;
		}

		public static bool SolidTile(int x, int y)
		{
			var Tile = Main.tile.At(x, y);
			return Tile.Active && Main.tileSolid[(int)Tile.Type] && !Main.tileSolidTop[(int)Tile.Type];
		}

		public static bool InvokeAlterationHook(ISender sender, Player player, int x, int y, byte action, byte type = 0, byte style = 0)
		{
			var ctx = new HookContext
			{
				Sender = sender,
				Player = player,
			};

			var args = new HookArgs.PlayerWorldAlteration
			{
				X = x,
				Y = y,
				Action = action,
				Type = type,
				Style = style,
			};

			HookPoints.PlayerWorldAlteration.Invoke(ref ctx, ref args);

			if (ctx.CheckForKick())
				return false;

			if (ctx.Result == HookResult.IGNORE)
				return false;

			if (ctx.Result == HookResult.RECTIFY)
			{
				if (player.whoAmi >= 0)
					NetMessage.SendTileSquare(player.whoAmi, x, y, 1); // FIXME
				return false;
			}

			return true;
		}

		public static void SpawnNPC(int x, int y)
		{
			if (Main.wallHouse[(int)Main.tile.At(x, y).Wall])
				canSpawn = true;

			if (!canSpawn)
				return;

			if (!StartRoomCheck(x, y))
				return;

			if (!RoomNeeds(spawnNPC))
				return;

			ScoreRoom(-1);
			if (hiScore > 0)
			{
				int npcIndex = -1;
				for (int i = 0; i < Main.npcs.Length; i++)
				{
					if (Main.npcs[i].Active && Main.npcs[i].homeless && Main.npcs[i].Type == spawnNPC)
					{
						npcIndex = i;
						break;
					}
				}
				if (npcIndex == -1)
				{
					int posX = bestX;
					int posY = bestY;
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
									posX = bestX + k;
								}
								else
								{
									posX = bestX - k;
								}
								if (posX > 10 && posX < Main.maxTilesX - 10)
								{
									int num4 = bestY - k;
									double num5 = (double)(bestY + k);

									if (num4 < 10)
										num4 = 10;

									if (num5 > Main.worldSurface)
										num5 = Main.worldSurface;

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
											relativeX++;
									}
								}
								if (flag)
									break;
							}
							if (flag)
								break;
						}
					}
					int townNPCIndex = NPC.NewNPC(posX * 16, posY * 16, spawnNPC, 1);
					Main.npcs[townNPCIndex].homeTileX = bestX;
					Main.npcs[townNPCIndex].homeTileY = bestY;
					if (posX < bestX)
						Main.npcs[townNPCIndex].direction = 1;
					else if (posX > bestX)
						Main.npcs[townNPCIndex].direction = -1;

					Main.npcs[townNPCIndex].netUpdate = true;
					NetMessage.SendData(25, -1, -1, Main.npcs[townNPCIndex].Name + " has arrived!", 255, 50f, 125f, 255f);
				}
				else
				{
					Main.npcs[npcIndex].homeTileX = bestX;
					Main.npcs[npcIndex].homeTileY = bestY;
					Main.npcs[npcIndex].homeless = false;
				}
				spawnNPC = 0;
			}
		}

		public static void QuickFindHome(int npc)
		{
			if (Main.npcs[npc].homeTileX > 10 && Main.npcs[npc].homeTileY > 10 && Main.npcs[npc].homeTileX < Main.maxTilesX - 10 && Main.npcs[npc].homeTileY < Main.maxTilesY)
			{
				canSpawn = false;
				StartRoomCheck(Main.npcs[npc].homeTileX, Main.npcs[npc].homeTileY - 1);
				if (!canSpawn)
				{
					for (int x = Main.npcs[npc].homeTileX - 1; x < Main.npcs[npc].homeTileX + 2; x++)
					{
						int y = Main.npcs[npc].homeTileY - 1;
						while (y < Main.npcs[npc].homeTileY + 2 && !StartRoomCheck(x, y))
						{
							y++;
						}
					}
				}
				if (!canSpawn)
				{
					int offset = 10;
					for (int x = Main.npcs[npc].homeTileX - offset; x <= Main.npcs[npc].homeTileX + offset; x += 2)
					{
						int y = Main.npcs[npc].homeTileY - offset;
						while (y <= Main.npcs[npc].homeTileY + offset && !StartRoomCheck(x, y))
						{
							y += 2;
						}
					}
				}
				if (canSpawn)
				{
					RoomNeeds(Main.npcs[npc].Type);
					if (canSpawn)
						ScoreRoom(npc);

					if (canSpawn && hiScore > 0)
					{
						Main.npcs[npc].homeTileX = bestX;
						Main.npcs[npc].homeTileY = bestY;
						Main.npcs[npc].homeless = false;
						canSpawn = false;
						return;
					}
					Main.npcs[npc].homeless = true;
					return;
				}
				else
					Main.npcs[npc].homeless = true;
			}
		}

		public static void ScoreRoom(int ignoreNPC = -1)
		{
			roomOccupied = false;
			roomEvil = false;
			for (int i = 0; i < Main.npcs.Length; i++)
			{
				if (Main.npcs[i].Active && Main.npcs[i].townNPC && ignoreNPC != i && !Main.npcs[i].homeless)
				{
					for (int j = 0; j < numRoomTiles; j++)
					{
						if (Main.npcs[i].homeTileX == roomX[j] && Main.npcs[i].homeTileY == roomY[j])
						{
							for (int k = 0; k < numRoomTiles; k++)
							{
								if (Main.npcs[i].homeTileX == roomX[k] && Main.npcs[i].homeTileY - 1 == roomY[k])
								{
									roomOccupied = true;
									hiScore = -1;
									return;
								}
							}
						}
					}
				}
			}

			hiScore = 0;

			int num = 0;
			int num2 = 0;
			int num3 = roomX1 - Main.zoneX / 2 / 16 - 1 - 45;
			int num4 = roomX2 + Main.zoneX / 2 / 16 + 1 + 45;
			int num5 = roomY1 - Main.zoneY / 2 / 16 - 1 - 45;
			int num6 = roomY2 + Main.zoneY / 2 / 16 + 1 + 45;

			if (num3 < 0)
				num3 = 0;

			if (num4 >= Main.maxTilesX)
				num4 = Main.maxTilesX - 1;

			if (num5 < 0)
				num5 = 0;

			if (num6 > Main.maxTilesX)
				num6 = Main.maxTilesX;

			for (int l = num3 + 1; l < num4; l++)
			{
				for (int m = num5 + 2; m < num6 + 2; m++)
				{
					if (Main.tile.At(l, m).Active)
					{
						if (Main.tile.At(l, m).Type == 23 || Main.tile.At(l, m).Type == 24 || Main.tile.At(l, m).Type == 25 || Main.tile.At(l, m).Type == 32)
							Main.evilTiles++;
						else if (Main.tile.At(l, m).Type == 27)
							Main.evilTiles -= 5;
					}
				}
			}
			if (num2 < 50)
				num2 = 0;

			int num7 = -num2;
			if (num7 <= -250)
			{
				hiScore = num7;
				roomEvil = true;
				return;
			}

			num3 = roomX1;
			num4 = roomX2;
			num5 = roomY1;
			num6 = roomY2;

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
											num -= 15;
										else if (Main.tile.At(num9, num10).Type == 10 || Main.tile.At(num9, num10).Type == 11)
											num -= 20;
										else if (Main.tileSolid[(int)Main.tile.At(num9, num10).Type])
											num -= 5;
										else
											num += 5;
									}
								}
							}
							if (num > hiScore)
							{
								for (int num11 = 0; num11 < numRoomTiles; num11++)
								{
									if (roomX[num11] == n && roomY[num11] == num8)
									{
										hiScore = num;
										bestX = n;
										bestY = num8;
										break;
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
			roomX1 = x;
			roomX2 = x;
			roomY1 = y;
			roomY2 = y;
			numRoomTiles = 0;
			for (int i = 0; i < MAX_TILE_SETS; i++)
			{
				houseTile[i] = false;
			}
			canSpawn = true;

			if (Main.tile.At(x, y).Active && Main.tileSolid[(int)Main.tile.At(x, y).Type])
				canSpawn = false;

			CheckRoom(x, y);

			if (numRoomTiles < 60)
				canSpawn = false;

			return canSpawn;
		}

		public static void CheckRoom(int x, int y)
		{
			if (!canSpawn)
				return;

			if (x < 10 || y < 10 || x >= Main.maxTilesX - 10 || y >= lastMaxTilesY - 10)
			{
				canSpawn = false;
				return;
			}

			for (int i = 0; i < numRoomTiles; i++)
			{
				if (roomX[i] == x && roomY[i] == y)
					return;
			}

			roomX[numRoomTiles] = x;
			roomY[numRoomTiles] = y;
			numRoomTiles++;

			if (numRoomTiles >= maxRoomTiles)
			{
				canSpawn = false;
				return;
			}

			if (Main.tile.At(x, y).Active)
			{
				houseTile[(int)Main.tile.At(x, y).Type] = true;
				if (Main.tileSolid[(int)Main.tile.At(x, y).Type] || Main.tile.At(x, y).Type == 11)
					return;
			}

			if (x < roomX1)
				roomX1 = x;

			if (x > roomX2)
				roomX2 = x;

			if (y < roomY1)
				roomY1 = y;

			if (y > roomY2)
				roomY2 = y;

			bool flag = false;
			bool flag2 = false;
			for (int j = -2; j < 3; j++)
			{
				if (Main.wallHouse[(int)Main.tile.At(x + j, y).Wall])
					flag = true;

				if (Main.tile.At(x + j, y).Active && (Main.tileSolid[(int)Main.tile.At(x + j, y).Type] || Main.tile.At(x + j, y).Type == 11))
					flag = true;

				if (Main.wallHouse[(int)Main.tile.At(x, y + j).Wall])
					flag2 = true;

				if (Main.tile.At(x, y + j).Active && (Main.tileSolid[(int)Main.tile.At(x, y + j).Type] || Main.tile.At(x, y + j).Type == 11))
					flag2 = true;
			}

			if (!flag || !flag2)
			{
				canSpawn = false;
				return;
			}

			for (int k = x - 1; k < x + 2; k++)
			{
				for (int l = y - 1; l < y + 2; l++)
				{
					if ((k != x || l != y) && canSpawn)
						CheckRoom(k, l);
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
							return;
					}
					num5++;
				}
			}
			while (!flag)
			{
				float num6 = (float)Main.maxTilesX * 0.08f;
				int x = Main.rand.Next(50, Main.maxTilesX - 50);
				while ((float)x > (float)Main.spawnTileX - num6 && (float)x < (float)Main.spawnTileX + num6)
				{
					x = Main.rand.Next(50, Main.maxTilesX - 50);
				}
				for (int y = Main.rand.Next(100); y < Main.maxTilesY; y++)
				{
					if (Main.tile.At(x, y).Active && Main.tileSolid[(int)Main.tile.At(x, y).Type])
					{
						flag = meteor(x, y);
						break;
					}
				}
				num++;
				if (num >= 100)
					return;

				Thread.Sleep(10);
			}
		}

		public static bool meteor(int x, int y)
		{
			if (x < 50 || x > Main.maxTilesX - 50)
				return false;

			if (y < 50 || y > Main.maxTilesY - 50)
				return false;

			Rectangle rectangle = new Rectangle((x - RECTANGLE_OFFSET) * TILE_SCALE, (y - RECTANGLE_OFFSET) * TILE_SCALE,
				RECTANGLE_OFFSET * 2 * TILE_SCALE, RECTANGLE_OFFSET * 2 * TILE_SCALE);

			BaseEntity entity;
			for (int i = 0; i < Main.MAX_PLAYERS; i++)
			{
				entity = Main.players[i];
				if (entity.Active && entity.Intersects(rectangle))
					return false;
			}

			for (int i = 0; i < NPC.MAX_NPCS; i++)
			{
				entity = Main.npcs[i];
				if (entity.Active && entity.Intersects(rectangle))
					return false;
			}

			for (int modX = x - RECTANGLE_OFFSET; modX < x + RECTANGLE_OFFSET; modX++)
			{
				for (int modY = y - RECTANGLE_OFFSET; modY < y + RECTANGLE_OFFSET; modY++)
				{
					if (Main.tile.At(modX, modY).Active && Main.tile.At(modX, modY).Type == 21)
						return false;
				}
			}

			stopDrops = true;
			for (int num2 = x - TILE_OFFSET; num2 < x + TILE_OFFSET; num2++)
			{
				for (int num3 = y - TILE_OFFSET; num3 < y + TILE_OFFSET; num3++)
				{
					if (num3 > y + Main.rand.Next(-2, 3) - 5 && (double)(Math.Abs(x - num2) + Math.Abs(y - num3)) < (double)TILE_OFFSET * 1.5 + (double)Main.rand.Next(-5, 5))
					{
						if (!Main.tileSolid[(int)Main.tile.At(num2, num3).Type])
							Main.tile.At(num2, num3).SetActive(false);

						Main.tile.At(num2, num3).SetType(37);
					}
				}
			}

			for (int num4 = x - TILES_OFFSET_2; num4 < x + TILES_OFFSET_2; num4++)
			{
				for (int num5 = y - TILES_OFFSET_2; num5 < y + TILES_OFFSET_2; num5++)
				{
					if (num5 > y + Main.rand.Next(-2, 3) - 5 && Math.Abs(x - num4) + Math.Abs(y - num5) < TILES_OFFSET_2 + Main.rand.Next(-3, 4))
						Main.tile.At(num4, num5).SetActive(false);
				}
			}

			for (int num6 = x - TILE_OFFSET_3; num6 < x + TILE_OFFSET_3; num6++)
			{
				for (int num7 = y - TILE_OFFSET_3; num7 < y + TILE_OFFSET_3; num7++)
				{
					if (Main.tile.At(num6, num7).Type == 5 || Main.tile.At(num6, num7).Type == 32)
						KillTile(num6, num7, false, false, false);

					SquareTileFrame(num6, num7, true);
					SquareWallFrame(num6, num7, true);
				}
			}

			for (int num8 = x - TILE_OFFSET_4; num8 < x + TILE_OFFSET_4; num8++)
			{
				for (int num9 = y - TILE_OFFSET_4; num9 < y + TILE_OFFSET_4; num9++)
				{
					if (Main.tile.At(num8, num9).Active && Main.rand.Next(10) == 0 && (double)(Math.Abs(x - num8) + Math.Abs(y - num9)) < (double)TILE_OFFSET_4 * 1.3)
					{
						if (Main.tile.At(num8, num9).Type == 5 || Main.tile.At(num8, num9).Type == 32)
							KillTile(num8, num9, false, false, false);

						Main.tile.At(num8, num9).SetType(37);
						SquareTileFrame(num8, num9, true);
					}
				}
			}
			stopDrops = false;

			NetMessage.SendData(25, -1, -1, "A meteorite has landed!", 255, 50f, 255f, 130f);
			NetMessage.SendTileSquare(-1, x, y, 30);
			return true;
		}
		
		public static bool IsValidTreeRootTile(TileRef tile)
		{
			return (tile.Active && (tile.Type == 2 || tile.Type == 23 || tile.Type == 60));
		}

		public static void GrowShroom(int i, int y)
		{
			if (Main.tile.At(i - 1, y - 1).Lava ||
				Main.tile.At(i - 1, y - 1).Lava ||
				Main.tile.At(i + 1, y - 1).Lava)
				return;

			if (Main.tile.At(i, y).Active && Main.tile.At(i, y).Type == 70 && Main.tile.At(i, y - 1).Wall == 0 && Main.tile.At(i - 1, y).Active && Main.tile.At(i - 1, y).Type == 70 && Main.tile.At(i + 1, y).Active && Main.tile.At(i + 1, y).Type == 70 && EmptyTileCheck(i - 2, i + 2, y - 13, y - 1, 71))
			{
				int num = genRand.Next(4, 11);
				int num2;
				for (int j = y - num; j < y; j++)
				{
					Main.tile.At(i, j).SetFrameNumber((byte)genRand.Next(3));
					Main.tile.At(i, j).SetActive(true);
					Main.tile.At(i, j).SetType(72);
					num2 = genRand.Next(3);
					if (num2 == 0)
					{
						Main.tile.At(i, j).SetFrameX(0);
						Main.tile.At(i, j).SetFrameY(0);
					}
					if (num2 == 1)
					{
						Main.tile.At(i, j).SetFrameX(0);
						Main.tile.At(i, j).SetFrameY(18);
					}
					if (num2 == 2)
					{
						Main.tile.At(i, j).SetFrameX(0);
						Main.tile.At(i, j).SetFrameY(36);
					}
				}
				num2 = genRand.Next(3);
				if (num2 == 0)
				{
					Main.tile.At(i, y - num).SetFrameX(36);
					Main.tile.At(i, y - num).SetFrameY(0);
				}
				if (num2 == 1)
				{
					Main.tile.At(i, y - num).SetFrameX(36);
					Main.tile.At(i, y - num).SetFrameY(18);
				}
				if (num2 == 2)
				{
					Main.tile.At(i, y - num).SetFrameX(36);
					Main.tile.At(i, y - num).SetFrameY(36);
				}
				RangeFrame(i - 2, y - num - 1, i + 2, y + 1);
				NetMessage.SendTileSquare(-1, i, (int)((double)y - (double)num * 0.5), num + 1);
			}
		}

		public static bool EmptyTileCheck(int startX, int endX, int startY, int endY, int ignoreStyle = -1)
		{
			if (startX < 0)
				return false;

			if (endX >= Main.maxTilesX)
				return false;

			if (startY < 0)
				return false;

			if (endY >= Main.maxTilesY)
				return false;

			for (int i = startX; i < endX + 1; i++)
			{
				for (int j = startY; j < endY + 1; j++)
				{
					if (Main.tile.At(i, j).Active)
					{
						if (ignoreStyle == -1)
							return false;

						if (ignoreStyle == 11 && Main.tile.At(i, j).Type != 11)
							return false;

						if (ignoreStyle == 20 && Main.tile.At(i, j).Type != 20 &&
							Main.tile.At(i, j).Type != 3 &&
							Main.tile.At(i, j).Type != 24 &&
							Main.tile.At(i, j).Type != 61 &&
							Main.tile.At(i, j).Type != 32 &&
							Main.tile.At(i, j).Type != 69 &&
							Main.tile.At(i, j).Type != 73 &&
							Main.tile.At(i, j).Type != 74 &&
							Main.tile.At(i, j).Type != 110 &&
							Main.tile.At(i, j).Type != 113)
							return false;

						if (ignoreStyle == 71 && Main.tile.At(i, j).Type != 71)
							return false;

					}
				}
			}
			return true;
		}

		public static bool PlaceDoor(int i, int j, int type)
		{
			try
			{
				if (Main.tile.At(i, j - 2).Active && Main.tileSolid[(int)Main.tile.At(i, j - 2).Type] && Main.tile.At(i, j + 2).Active && Main.tileSolid[(int)Main.tile.At(i, j + 2).Type])
				{
					Main.tile.At(i, j - 1).SetActive(true);
					Main.tile.At(i, j - 1).SetType(10);
					Main.tile.At(i, j - 1).SetFrameY(0);
					Main.tile.At(i, j - 1).SetFrameX((short)(genRand.Next(3) * 18));
					Main.tile.At(i, j).SetActive(true);
					Main.tile.At(i, j).SetType(10);
					Main.tile.At(i, j).SetFrameY(18);
					Main.tile.At(i, j).SetFrameX((short)(genRand.Next(3) * 18));
					Main.tile.At(i, j + 1).SetActive(true);
					Main.tile.At(i, j + 1).SetType(10);
					Main.tile.At(i, j + 1).SetFrameY(36);
					Main.tile.At(i, j + 1).SetFrameX((short)(genRand.Next(3) * 18));
					return true;
				}
				else
					return false;
			}
			catch
			{
				return false;
			}
		}

		public static bool CloseDoor(int x, int y, bool forced, ISender sender)
		{
			if (sender == null)
				sender = new ConsoleSender();

			if (Program.properties.NPCDoorOpenCancel && sender is NPC)
				return false;

			var ctx = new HookContext
			{
				Sender = sender,
			};

			var args = new HookArgs.DoorStateChanged
			{
				X = x,
				Y = y,
				Direction = 1,
				Open = false,
			};

			HookPoints.DoorStateChanged.Invoke(ref ctx, ref args);

			if (ctx.CheckForKick())
				return false;

			if (ctx.Result == HookResult.IGNORE)
				return false;

			if (ctx.Result == HookResult.RECTIFY)
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
						return false;
				}
			}
			for (int l = num4; l < num4 + 2; l++)
			{
				for (int m = num3; m < num3 + 3; m++)
				{
					if (l == num2)
					{
						Main.tile.At(l, m).SetType(10);
						Main.tile.At(l, m).SetFrameX((short)(genRand.Next(3) * 18));
					}
					else
					{
						Main.tile.At(l, m).SetActive(false);
					}
				}
			}

			/* 1.1 */
			int num5 = num2;
			for (int n = num3; n <= num3 + 2; n++)
			{
				if (numNoWire < MAX_WIRE - 1)
				{
					noWireX[numNoWire] = num5;
					noWireY[numNoWire] = n;
					numNoWire++;
				}
			}

			for (int n = num2 - 1; n <= num2 + 1; n++)
			{
				for (int num6 = num3 - 1; num5 <= num3 + 2; num5++)
				{
					TileFrame(n, num5, false, false);
				}
			}
			return true;
		}

		public static bool OpenDoor(int x, int y, int direction, ISender sender)
		{
			if (sender == null)
			{
				sender = new ConsoleSender();
			}

			if (Program.properties.NPCDoorOpenCancel && sender is NPC)
				return false;

			var ctx = new HookContext
			{
				Sender = sender,
			};

			var args = new HookArgs.DoorStateChanged
			{
				X = x,
				Y = y,
				Direction = direction,
				Open = true,
			};

			HookPoints.DoorStateChanged.Invoke(ref ctx, ref args);

			if (ctx.CheckForKick())
				return false;

			if (ctx.Result == HookResult.IGNORE)
				return false;

			if (ctx.Result == HookResult.RECTIFY)
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
					KillTile(num4, k, false, false, false);
				}
			}
			if (flag)
			{
				Main.tile.At(num2, num).SetActive(true);
				Main.tile.At(num2, num).SetType(11);
				Main.tile.At(num2, num).SetFrameY(0);
				Main.tile.At(num2, num).SetFrameX(num3);
				Main.tile.At(num2 + 1, num).SetActive(true);
				Main.tile.At(num2 + 1, num).SetType(11);
				Main.tile.At(num2 + 1, num).SetFrameY(0);
				Main.tile.At(num2 + 1, num).SetFrameX((short)(num3 + 18));
				Main.tile.At(num2, num + 1).SetActive(true);
				Main.tile.At(num2, num + 1).SetType(11);
				Main.tile.At(num2, num + 1).SetFrameY(18);
				Main.tile.At(num2, num + 1).SetFrameX(num3);
				Main.tile.At(num2 + 1, num + 1).SetActive(true);
				Main.tile.At(num2 + 1, num + 1).SetType(11);
				Main.tile.At(num2 + 1, num + 1).SetFrameY(18);
				Main.tile.At(num2 + 1, num + 1).SetFrameX((short)(num3 + 18));
				Main.tile.At(num2, num + 2).SetActive(true);
				Main.tile.At(num2, num + 2).SetType(11);
				Main.tile.At(num2, num + 2).SetFrameY(36);
				Main.tile.At(num2, num + 2).SetFrameX(num3);
				Main.tile.At(num2 + 1, num + 2).SetActive(true);
				Main.tile.At(num2 + 1, num + 2).SetType(11);
				Main.tile.At(num2 + 1, num + 2).SetFrameY(36);
				Main.tile.At(num2 + 1, num + 2).SetFrameX((short)(num3 + 18));
				for (int l = num2 - 1; l <= num2 + 2; l++)
				{
					for (int m = num - 1; m <= num + 2; m++)
					{
						TileFrame(l, m, false, false);
					}
				}
			}
			return flag;
		}

		public static void Check1x2(int x, int j, byte type)
		{
			if (destroyObject)
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
			if ((int)Main.tile.At(x, num).FrameY == 40 * num2 &&
				(int)Main.tile.At(x, num + 1).FrameY == 40 * num2 + 18 &&
				Main.tile.At(x, num).Type == type && Main.tile.At(x, num + 1).Type == type)
			{
				flag = false;
			}
			if (!Main.tile.At(x, num + 2).Active || !Main.tileSolid[(int)Main.tile.At(x, num + 2).Type])
			{
				flag = true;
			}
			if (Main.tile.At(x, num + 2).Type != 2 && Main.tile.At(x, num + 2).Type != 109 && Main.tile.At(x, num).Type == 20)
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				if (Main.tile.At(x, num).Type == type)
				{
					KillTile(x, num, false, false, false);
				}
				if (Main.tile.At(x, num + 1).Type == type)
				{
					KillTile(x, num + 1, false, false, false);
				}
				if (type == 15)
				{
					if (num2 == 1)
						Item.NewItem(x * 16, num * 16, 32, 32, 358);
					else
						Item.NewItem(x * 16, num * 16, 32, 32, 34);
				}
				else if (type == 134)
					Item.NewItem(x * 16, num * 16, 32, 32, 525);

				destroyObject = false;
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
						KillTile(x, y, false, false, false);
						return;
					}
				}
				else
				{
					KillTile(x, y, false, false, false);
				}
			}
		}

		public static void CheckSign(int x, int y, int type)
		{
			if (destroyObject)
				return;

			int leftX = x - 2;
			int rightX = x + 3;
			int topY = y - 2;
			int bottomY = y + 3;

			if (leftX < 0)
				return;

			if (rightX > Main.maxTilesX)
				return;

			if (topY < 0)
				return;

			if (bottomY > Main.maxTilesY)
				return;

			bool flag = false;
			int k = (int)(Main.tile.At(x, y).FrameX / 18);
			int num5 = (int)(Main.tile.At(x, y).FrameY / 18);

			while (k > 1)
				k -= 2;

			int num6 = x - k;
			int num7 = y - num5;
			int num8 = (int)(Main.tile.At(num6, num7).FrameX / 18 / 2);
			leftX = num6;
			rightX = num6 + 2;
			topY = num7;
			bottomY = num7 + 2;
			k = 0;

			for (int l = leftX; l < rightX; l++)
			{
				num5 = 0;
				for (int m = topY; m < bottomY; m++)
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
					if (Main.tile.At(num6, num7 + 2).Active && Main.tileSolid[(int)Main.tile.At(num6, num7 + 2).Type] && Main.tile.At(num6 + 1, num7 + 2).Active &&
						Main.tileSolid[(int)Main.tile.At(num6 + 1, num7 + 2).Type])
					{
						num8 = 0;
					}
					else
					{
						flag = true;
					}
				}
				else if (Main.tile.At(num6, num7 + 2).Active && Main.tileSolid[(int)Main.tile.At(num6, num7 + 2).Type] &&
					Main.tile.At(num6 + 1, num7 + 2).Active && Main.tileSolid[(int)Main.tile.At(num6 + 1, num7 + 2).Type])
				{
					num8 = 0;
				}
				else if (Main.tile.At(num6, num7 - 1).Active && Main.tileSolid[(int)Main.tile.At(num6, num7 - 1).Type] &&
					!Main.tileSolidTop[(int)Main.tile.At(num6, num7 - 1).Type] && Main.tile.At(num6 + 1, num7 - 1).Active &&
					Main.tileSolid[(int)Main.tile.At(num6 + 1, num7 - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 + 1, num7 - 1).Type])
				{
					num8 = 1;
				}
				else if (Main.tile.At(num6 - 1, num7).Active && Main.tileSolid[(int)Main.tile.At(num6 - 1, num7).Type] &&
					!Main.tileSolidTop[(int)Main.tile.At(num6 - 1, num7).Type] && Main.tile.At(num6 - 1, num7 + 1).Active &&
					Main.tileSolid[(int)Main.tile.At(num6 - 1, num7 + 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 - 1, num7 + 1).Type])
				{
					num8 = 2;
				}
				else if (Main.tile.At(num6 + 2, num7).Active && Main.tileSolid[(int)Main.tile.At(num6 + 2, num7).Type] &&
					!Main.tileSolidTop[(int)Main.tile.At(num6 + 2, num7).Type] && Main.tile.At(num6 + 2, num7 + 1).Active &&
					Main.tileSolid[(int)Main.tile.At(num6 + 2, num7 + 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num6 + 2, num7 + 1).Type])
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
				destroyObject = true;
				for (int n = leftX; n < rightX; n++)
				{
					for (int num9 = topY; num9 < bottomY; num9++)
					{
						if ((int)Main.tile.At(n, num9).Type == type)
							KillTile(n, num9);
					}
				}

				Sign.KillSign(num6, num7);

				if (type == 85)
					Item.NewItem(x * 16, y * 16, 32, 32, 321);
				else
					Item.NewItem(x * 16, y * 16, 32, 32, 171);

				destroyObject = false;
				return;
			}
			int num10 = 36 * num8;
			for (int num11 = 0; num11 < 2; num11++)
			{
				for (int num12 = 0; num12 < 2; num12++)
				{
					Main.tile.At(num6 + num11, num7 + num12).SetActive(true);
					Main.tile.At(num6 + num11, num7 + num12).SetType((byte)type);
					Main.tile.At(num6 + num11, num7 + num12).SetFrameX((short)(num10 + 18 * num11));
					Main.tile.At(num6 + num11, num7 + num12).SetFrameY((short)(18 * num12));
				}
			}
		}

		public static bool PlaceSign(int x, int y, int type)
		{
			int leftX = x - 2;
			int rightX = x + 3;
			int topY = y - 2;
			int bottomY = y + 3;

			if (leftX < 0)
				return false;

			if (rightX > Main.maxTilesX)
				return false;

			if (topY < 0)
				return false;

			if (bottomY > Main.maxTilesY)
				return false;

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
					Main.tile.At(num5 + k, num6 + l).SetActive(true);
					Main.tile.At(num5 + k, num6 + l).SetType((byte)type);
					Main.tile.At(num5 + k, num6 + l).SetFrameX((short)(num8 + 18 * k));
					Main.tile.At(num5 + k, num6 + l).SetFrameY((short)(18 * l));
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
				Main.tile.At(x, y).SetActive(true);
				Main.tile.At(x, y).SetFrameX((short)(style * 18));
				Main.tile.At(x, y).SetFrameY(0);
				Main.tile.At(x, y).SetType((byte)type);
				if (type == 50)
				{
					Main.tile.At(x, y).SetFrameX((short)(18 * genRand.Next(5)));
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
					if (Main.tile.At(x, y + 1).Type != 2 && Main.tile.At(x, y + 1).Type != 78 && Main.tile.At(x, y + 1).Type != 109)
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
					if (Main.tile.At(x, y + 1).Type != 53 && Main.tile.At(x, y + 1).Type != 78 && Main.tile.At(x, y + 1).Type != 116)
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
					Main.tile.At(x, y).SetActive(true);
					Main.tile.At(x, y).SetType(82);
					Main.tile.At(x, y).SetFrameX((short)(18 * style));
					Main.tile.At(x, y).SetFrameY(0);
					return true;
				}
			}
			return false;
		}

		public static void GrowAlch(int x, int y)
		{
			if (Main.tile.At(x, y).Active)
			{
				if (Main.tile.At(x, y).Type == 82 && genRand.Next(50) == 0)
				{
					Main.tile.At(x, y).SetType(83);

					NetMessage.SendTileSquare(-1, x, y, 1);
					SquareTileFrame(x, y, true);
					return;
				}
				if (Main.tile.At(x, y).FrameX == 36)
				{
					if (Main.tile.At(x, y).Type == 83)
						Main.tile.At(x, y).SetType(84);
					else
						Main.tile.At(x, y).SetType(83);
					NetMessage.SendTileSquare(-1, x, y, 1);
				}
			}
		}

		public static void PlantAlch()
		{
			int x = genRand.Next(20, Main.maxTilesX - 20);
			int y = 0;
			if (genRand.Next(40) == 0)
			{
				var start = (int)(Main.rockLayer + (double)Main.maxTilesY) / 2;
				var end = Main.maxTilesY - 20;
				if (end >= start)
					y = genRand.Next(start, end);
			}
			else if (genRand.Next(10) == 0)
			{
				y = genRand.Next(0, Main.maxTilesY - 20);
			}
			else
			{
				y = genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
			}
			while (y < Main.maxTilesY - 20 && !Main.tile.At(x, y).Active)
			{
				y++;
			}
			if (Main.tile.At(x, y).Active && !Main.tile.At(x, y - 1).Active && Main.tile.At(x, y - 1).Liquid == 0)
			{
				var tile = Main.tile.At(x, y);

				if (tile.Type == 2 || tile.Type == 109)
					PlaceAlch(x, y - 1, 0);

				if (tile.Type == 60)
					PlaceAlch(x, y - 1, 1);

				if (tile.Type == 0 || tile.Type == 59)
					PlaceAlch(x, y - 1, 2);

				if (tile.Type == 23 || tile.Type == 25)
					PlaceAlch(x, y - 1, 3);

				if (tile.Type == 53 || tile.Type == 116)
					PlaceAlch(x, y - 1, 4);

				if (tile.Type == 57)
					PlaceAlch(x, y - 1, 5);

				if (Main.tile.At(x, y - 1).Active)
					NetMessage.SendTileSquare(-1, x, y - 1, 1);
			}
		}

		public static void CheckAlch(int x, int y)
		{
			bool active = !Main.tile.At(x, y + 1).Active;

			int num = (int)(Main.tile.At(x, y).FrameX / 18);
			Main.tile.At(x, y).SetFrameY(0);
			if (!active)
			{
				if (num == 0)
				{
					if (Main.tile.At(x, y + 1).Type != 2 && Main.tile.At(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
					{
						active = true;
					}
				}
				else if (num == 1)
				{
					if (Main.tile.At(x, y + 1).Type != 60 && Main.tile.At(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
					{
						active = true;
					}
				}
				else if (num == 2)
				{
					if (Main.tile.At(x, y + 1).Type != 0 && Main.tile.At(x, y + 1).Type != 59 && Main.tile.At(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
					{
						active = true;
					}
				}
				else if (num == 3)
				{
					if (Main.tile.At(x, y + 1).Type != 23 && Main.tile.At(x, y + 1).Type != 25 && Main.tile.At(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
					{
						active = true;
					}
				}
				else if (num == 4)
				{
					if (Main.tile.At(x, y + 1).Type != 53 && Main.tile.At(x, y + 1).Type != 78 && Main.tile.At(x, y + 1).Type != 116)
					{
						active = true;
					}
					if (Main.tile.At(x, y).Liquid > 0 && Main.tile.At(x, y).Lava)
					{
						active = true;
					}
					if (Main.tile.At(x, y).Type != 82 && !Main.tile.At(x, y).Lava)
					{
						if (Main.tile.At(x, y).Liquid > 16)
						{
							if (Main.tile.At(x, y).Type == 83)
							{
								Main.tile.At(x, y).SetType(84);

								NetMessage.SendTileSquare(-1, x, y, 1);
							}
						}
						else if (Main.tile.At(x, y).Type == 84)
						{
							Main.tile.At(x, y).SetType(83);

							NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
				else if (num == 5)
				{
					if (Main.tile.At(x, y + 1).Type != 57 && Main.tile.At(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (Main.tile.At(x, y).Liquid > 0 && !Main.tile.At(x, y).Lava)
					{
						active = true;
					}
					if (Main.tile.At(x, y).Type != 82 && Main.tile.At(x, y).Lava && Main.tile.At(x, y).Type != 82 && Main.tile.At(x, y).Lava)
					{
						if (Main.tile.At(x, y).Liquid > 16)
						{
							if (Main.tile.At(x, y).Type == 83)
							{
								Main.tile.At(x, y).SetType(84);
								NetMessage.SendTileSquare(-1, x, y, 1);
							}
						}
						else if (Main.tile.At(x, y).Type == 84)
						{
							Main.tile.At(x, y).SetType(83);
							NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
			}
			if (active)
			{
				KillTile(x, y, false, false, false);
			}
		}

		public static void Place1x2(int x, int y, int type, int style)
		{
			short frameX = 0;
			if (type == 20)
			{
				frameX = (short)(genRand.Next(3) * 18);
			}
			if (Main.tile.At(x, y + 1).Active && Main.tileSolid[(int)Main.tile.At(x, y + 1).Type] && !Main.tile.At(x, y - 1).Active)
			{
				short frameHeight = (short)(style * 40);
				Main.tile.At(x, y - 1).SetActive(true);
				Main.tile.At(x, y - 1).SetFrameY(frameHeight);
				Main.tile.At(x, y - 1).SetFrameX(frameX);
				Main.tile.At(x, y - 1).SetType((byte)type);
				Main.tile.At(x, y).SetActive(true);
				Main.tile.At(x, y).SetFrameY((short)(frameHeight + 18));
				Main.tile.At(x, y).SetFrameX(frameX);
				Main.tile.At(x, y).SetType((byte)type);
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
			if (destroyObject)
				return;

			int num = j - (int)(Main.tile.At(x, j).FrameY / 18);
			int frameX = (int)Main.tile.At(x, j).FrameX;
			bool flag = false;

			for (int i = 0; i < 3; i++)
			{
				if (!Main.tile.At(x, num + i).Active)
					flag = true;
				else if (Main.tile.At(x, num + i).Type != type)
					flag = true;
				else if ((int)Main.tile.At(x, num + i).FrameY != i * 18)
					flag = true;
				else if ((int)Main.tile.At(x, num + i).FrameX != frameX)
					flag = true;
			}
			if (!Main.tile.At(x, num - 1).Active)
				flag = true;

			if (!Main.tileSolid[(int)Main.tile.At(x, num - 1).Type])
				flag = true;

			if (Main.tileSolidTop[(int)Main.tile.At(x, num - 1).Type])
				flag = true;

			if (flag)
			{
				destroyObject = true;
				for (int k = 0; k < 3; k++)
				{
					if (Main.tile.At(x, num + k).Type == type)
						KillTile(x, num + k);
				}
				if (type == 91)
				{
					int num2 = frameX / 18;
					Item.NewItem(x * 16, (num + 1) * 16, 32, 32, 337 + num2);
				}
				destroyObject = false;
			}
		}

		public static void Place1x2Top(int x, int y, int type)
		{
			short frameX = 0;
			if (Main.tile.At(x, y - 1).Active && Main.tileSolid[(int)Main.tile.At(x, y - 1).Type] &&
				!Main.tileSolidTop[(int)Main.tile.At(x, y - 1).Type] && !Main.tile.At(x, y + 1).Active)
			{
				Main.tile.At(x, y).SetActive(true);
				Main.tile.At(x, y).SetFrameY(0);
				Main.tile.At(x, y).SetFrameX(frameX);
				Main.tile.At(x, y).SetType((byte)type);
				Main.tile.At(x, y + 1).SetActive(true);
				Main.tile.At(x, y + 1).SetFrameY(18);
				Main.tile.At(x, y + 1).SetFrameX(frameX);
				Main.tile.At(x, y + 1).SetType((byte)type);
			}
		}

		public static void Check1x2Top(int x, int y, byte type)
		{
			if (destroyObject)
				return;

			bool flag = true;

			if (Main.tile.At(x, y).FrameY == 18)
				y--;

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
				destroyObject = true;
				if (Main.tile.At(x, y).Type == type)
					KillTile(x, y);

				if (Main.tile.At(x, y + 1).Type == type)
					KillTile(x, y + 1);

				if (type == 42)
					Item.NewItem(x * 16, y * 16, 32, 32, 136);

				destroyObject = false;
			}
		}

		public static void Check2x1(int x, int y, byte type)
		{
			if (destroyObject)
				return;

			bool flag = true;

			if (Main.tile.At(x, y).FrameX == 18)
				x--;

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
				destroyObject = true;
				if (Main.tile.At(x, y).Type == type)
				{
					KillTile(x, y, false, false, false);
				}
				if (Main.tile.At(x + 1, y).Type == type)
				{
					KillTile(x + 1, y, false, false, false);
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
				destroyObject = false;
				SquareTileFrame(x, y, true);
				SquareTileFrame(x + 1, y, true);
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
				Main.tile.At(x, y).SetActive(true);
				Main.tile.At(x, y).SetFrameY(0);
				Main.tile.At(x, y).SetFrameX(0);
				Main.tile.At(x, y).SetType((byte)type);
				Main.tile.At(x + 1, y).SetActive(true);
				Main.tile.At(x + 1, y).SetFrameY(0);
				Main.tile.At(x + 1, y).SetFrameX(18);
				Main.tile.At(x + 1, y).SetType((byte)type);
			}
		}

		public static void Check4x2(int i, int j, int type)
		{
			if (destroyObject)
				return;

			bool flag = false;
			int num = i;
			num += (int)(Main.tile.At(i, j).FrameX / 18 * -1);

			if ((type == 79 || type == 90) && Main.tile.At(i, j).FrameX >= 72)
				num += 4;

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
				destroyObject = true;
				for (int m = num; m < num + 4; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							KillTile(m, n);
						}
					}
				}
				if (type == 79)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 224);
				}
				if (type == 90)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 336);
				}
				destroyObject = false;
				for (int num4 = num - 1; num4 < num + 4; num4++)
				{
					for (int num5 = num2 - 1; num5 < num2 + 4; num5++)
					{
						TileFrame(num4, num5);
					}
				}
			}
		}

		public static void Check2x2(int i, int j, int type)
		{
			if (destroyObject)
				return;

			bool flag = false;
			int num = i;
			int num2 = 0;
			num = (int)(Main.tile.At(i, j).FrameX / 18 * -1);
			int num3 = (int)(Main.tile.At(i, j).FrameY / 18 * -1);
			if (num < -1)
			{
				num += 2;
				num2 = 36;
			}
			num += i;
			num3 += j;
			for (int k = num; k < num + 2; k++)
			{
				for (int l = num3; l < num3 + 2; l++)
				{
					if (!Main.tile.At(k, l).Active || (int)Main.tile.At(k, l).Type != type ||
						(int)Main.tile.At(k, l).FrameX != (k - num) * 18 + num2 || (int)Main.tile.At(k, l).FrameY != (l - num3) * 18)
					{
						flag = true;
					}
				}
				if (type == 95 || type == 126)
				{
					if (!Main.tile.At(k, num3 - 1).Active || !Main.tileSolid[(int)Main.tile.At(k, num3 - 1).Type] || Main.tileSolidTop[(int)Main.tile.At(k, num3 - 1).Type])
					{
						flag = true;
					}
				}
				else if (type != 138)
				{
					if (!Main.tile.At(k, num3 + 2).Active || (!Main.tileSolid[(int)Main.tile.At(k, num3 + 2).Type] && !Main.tileTable[(int)Main.tile.At(k, num3 + 2).Type]))
					{
						flag = true;
					}
				}
			}
			if (type == 138 && !SolidTile(num, num3 + 2) && !SolidTile(num + 1, num3 + 2))
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num3; n < num3 + 2; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
							KillTile(m, n);
					}
				}
				if (type == 85)
					Item.NewItem(i * 16, j * 16, 32, 32, 321);

				if (type == 94)
					Item.NewItem(i * 16, j * 16, 32, 32, 352);

				if (type == 95)
					Item.NewItem(i * 16, j * 16, 32, 32, 344);

				if (type == 96)
					Item.NewItem(i * 16, j * 16, 32, 32, 345);

				if (type == 97)
					Item.NewItem(i * 16, j * 16, 32, 32, 346);

				if (type == 98)
					Item.NewItem(i * 16, j * 16, 32, 32, 347);

				if (type == 99)
					Item.NewItem(i * 16, j * 16, 32, 32, 348);

				if (type == 100)
					Item.NewItem(i * 16, j * 16, 32, 32, 349);

				if (type == 125)
					Item.NewItem(i * 16, j * 16, 32, 32, 487);

				if (type == 126)
					Item.NewItem(i * 16, j * 16, 32, 32, 488);

				if (type == 132)
					Item.NewItem(i * 16, j * 16, 32, 32, 513);

				if (type == 142)
					Item.NewItem(i * 16, j * 16, 32, 32, 581);

				if (type == 143)
					Item.NewItem(i * 16, j * 16, 32, 32, 582);

				if (type == 138 && !gen)
					Projectile.NewProjectile((float)(num * 16) + 15.5f, (float)(num3 * 16 + 16), 0f, 0f, 99, 70, 10f, Main.myPlayer);

				destroyObject = false;
				for (int num4 = num - 1; num4 < num + 3; num4++)
				{
					for (int num5 = num3 - 1; num5 < num3 + 3; num5++)
					{
						TileFrame(num4, num5, false, false);
					}
				}
			}
		}

		public static void Check3x2(int i, int j, int type)
		{
			if (destroyObject)
				return;

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
				destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							KillTile(m, n);
						}
					}
				}

				if (type == 14)
					Item.NewItem(i * 16, j * 16, 32, 32, 32);
				else if (type == 17)
					Item.NewItem(i * 16, j * 16, 32, 32, 33);
				else if (type == 77)
					Item.NewItem(i * 16, j * 16, 32, 32, 221);
				else if (type == 86)
					Item.NewItem(i * 16, j * 16, 32, 32, 332);
				else if (type == 87)
					Item.NewItem(i * 16, j * 16, 32, 32, 333);
				else if (type == 88)
					Item.NewItem(i * 16, j * 16, 32, 32, 334);
				else if (type == 89)
					Item.NewItem(i * 16, j * 16, 32, 32, 335);
				else if (type == 133)
					Item.NewItem(i * 16, j * 16, 32, 32, 524);

				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						TileFrame(num3, num4, false, false);
					}
				}
			}
		}

		public static void Place4x2(int x, int y, int type, int direction = -1)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
				return;

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
				Main.tile.At(x - 1, y - 1).SetActive(true);
				Main.tile.At(x - 1, y - 1).SetFrameY(0);
				Main.tile.At(x - 1, y - 1).SetFrameX(num);
				Main.tile.At(x - 1, y - 1).SetType((byte)type);
				Main.tile.At(x, y - 1).SetActive(true);
				Main.tile.At(x, y - 1).SetFrameY(0);
				Main.tile.At(x, y - 1).SetFrameX((short)(18 + num));
				Main.tile.At(x, y - 1).SetType((byte)type);
				Main.tile.At(x + 1, y - 1).SetActive(true);
				Main.tile.At(x + 1, y - 1).SetFrameY(0);
				Main.tile.At(x + 1, y - 1).SetFrameX((short)(36 + num));
				Main.tile.At(x + 1, y - 1).SetType((byte)type);
				Main.tile.At(x + 2, y - 1).SetActive(true);
				Main.tile.At(x + 2, y - 1).SetFrameY(0);
				Main.tile.At(x + 2, y - 1).SetFrameX((short)(54 + num));
				Main.tile.At(x + 2, y - 1).SetType((byte)type);
				Main.tile.At(x - 1, y).SetActive(true);
				Main.tile.At(x - 1, y).SetFrameY(18);
				Main.tile.At(x - 1, y).SetFrameX(num);
				Main.tile.At(x - 1, y).SetType((byte)type);
				Main.tile.At(x, y).SetActive(true);
				Main.tile.At(x, y).SetFrameY(18);
				Main.tile.At(x, y).SetFrameX((short)(18 + num));
				Main.tile.At(x, y).SetType((byte)type);
				Main.tile.At(x + 1, y).SetActive(true);
				Main.tile.At(x + 1, y).SetFrameY(18);
				Main.tile.At(x + 1, y).SetFrameX((short)(36 + num));
				Main.tile.At(x + 1, y).SetType((byte)type);
				Main.tile.At(x + 2, y).SetActive(true);
				Main.tile.At(x + 2, y).SetFrameY(18);
				Main.tile.At(x + 2, y).SetFrameX((short)(54 + num));
				Main.tile.At(x + 2, y).SetType((byte)type);
			}
		}

		public static void Place2x2(int x, int superY, int type)
		{
			int y = superY;
			if (type == 95 || type == 126)
				y++;

			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
				return;

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
				if (type == 95 || type == 126)
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
				Main.tile.At(x - 1, y - 1).SetActive(true);
				Main.tile.At(x - 1, y - 1).SetFrameY(0);
				Main.tile.At(x - 1, y - 1).SetFrameX(0);
				Main.tile.At(x - 1, y - 1).SetType((byte)type);
				Main.tile.At(x, y - 1).SetActive(true);
				Main.tile.At(x, y - 1).SetFrameY(0);
				Main.tile.At(x, y - 1).SetFrameX(18);
				Main.tile.At(x, y - 1).SetType((byte)type);
				Main.tile.At(x - 1, y).SetActive(true);
				Main.tile.At(x - 1, y).SetFrameY(18);
				Main.tile.At(x - 1, y).SetFrameX(0);
				Main.tile.At(x - 1, y).SetType((byte)type);
				Main.tile.At(x, y).SetActive(true);
				Main.tile.At(x, y).SetFrameY(18);
				Main.tile.At(x, y).SetFrameX(18);
				Main.tile.At(x, y).SetType((byte)type);
			}
		}

		public static void Place3x2(int x, int y, int type)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
				return;

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
				Main.tile.At(x - 1, y - 1).SetActive(true);
				Main.tile.At(x - 1, y - 1).SetFrameY(0);
				Main.tile.At(x - 1, y - 1).SetFrameX(0);
				Main.tile.At(x - 1, y - 1).SetType((byte)type);
				Main.tile.At(x, y - 1).SetActive(true);
				Main.tile.At(x, y - 1).SetFrameY(0);
				Main.tile.At(x, y - 1).SetFrameX(18);
				Main.tile.At(x, y - 1).SetType((byte)type);
				Main.tile.At(x + 1, y - 1).SetActive(true);
				Main.tile.At(x + 1, y - 1).SetFrameY(0);
				Main.tile.At(x + 1, y - 1).SetFrameX(36);
				Main.tile.At(x + 1, y - 1).SetType((byte)type);
				Main.tile.At(x - 1, y).SetActive(true);
				Main.tile.At(x - 1, y).SetFrameY(18);
				Main.tile.At(x - 1, y).SetFrameX(0);
				Main.tile.At(x - 1, y).SetType((byte)type);
				Main.tile.At(x, y).SetActive(true);
				Main.tile.At(x, y).SetFrameY(18);
				Main.tile.At(x, y).SetFrameX(18);
				Main.tile.At(x, y).SetType((byte)type);
				Main.tile.At(x + 1, y).SetActive(true);
				Main.tile.At(x + 1, y).SetFrameY(18);
				Main.tile.At(x + 1, y).SetFrameX(36);
				Main.tile.At(x + 1, y).SetType((byte)type);
			}
		}

		public static void Check3x3(int i, int j, int type)
		{
			if (destroyObject)
				return;

			bool flag = false;
			int num = i;
			num = (int)(Main.tile.At(i, j).FrameX / 18);
			int num2 = i - num;

			if (num >= 3)
				num -= 3;

			num = i - num;
			int num3 = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num3; l < num3 + 3; l++)
				{
					if (!Main.tile.At(k, l).Active || (int)Main.tile.At(k, l).Type != type ||
						(int)Main.tile.At(k, l).FrameX != (k - num2) * 18 || (int)Main.tile.At(k, l).FrameY != (l - num3) * 18)
					{
						flag = true;
					}
				}
			}
			if (type == 106)
			{
				for (int m = num; m < num + 3; m++)
				{
					if (!Main.tile.At(m, num3 + 3).Active || !Main.tileSolid[(int)Main.tile.At(m, num3 + 3).Type])
					{
						flag = true;
						break;
					}
				}
			}
			else if (!Main.tile.At(num + 1, num3 - 1).Active || !Main.tileSolid[(int)Main.tile.At(num + 1, num3 - 1).Type] || Main.tileSolidTop[(int)Main.tile.At(num + 1, num3 - 1).Type])
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				for (int n = num; n < num + 3; n++)
				{
					for (int num4 = num3; num4 < num3 + 3; num4++)
					{
						if ((int)Main.tile.At(n, num4).Type == type && Main.tile.At(n, num4).Active)
							KillTile(n, num4);
					}
				}
				if (type == 34)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 106);
				}
				else if (type == 35)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 107);
				}
				else if (type == 36)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 108);
				}
				else if (type == 106)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 363);
				}

				destroyObject = false;

				for (int num5 = num - 1; num5 < num + 4; num5++)
				{
					for (int num6 = num3 - 1; num6 < num3 + 4; num6++)
					{
						TileFrame(num5, num6);
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
				Main.tile.At(x - 1, y + num).SetActive(true);
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
			if (destroyObject)
				return;

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
				destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 4; n++)
					{
						if ((int)Main.tile.At(m, n).Type == type && Main.tile.At(m, n).Active)
						{
							KillTile(m, n);
						}
					}
				}
				if (type == 101)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 354);
				}
				else if (type == 102)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 355);
				}

				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						TileFrame(num3, num4);
					}
				}
			}
		}

		public static void Check1xX(int x, int j, byte type)
		{
			if (destroyObject)
				return;

			int num = j - (int)(Main.tile.At(x, j).FrameY / 18);
			int frameX = (int)Main.tile.At(x, j).FrameX;
			int num2 = 3;

			if (type == 92)
				num2 = 6;

			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				if (!Main.tile.At(x, num + i).Active)
				{
					flag = true;
				}
				else if (Main.tile.At(x, num + i).Type != type)
				{
					flag = true;
				}
				else if ((int)Main.tile.At(x, num + i).FrameY != i * 18)
				{
					flag = true;
				}
				else if ((int)Main.tile.At(x, num + i).FrameX != frameX)
				{
					flag = true;
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
				destroyObject = true;
				for (int k = 0; k < num2; k++)
				{
					if (Main.tile.At(x, num + k).Type == type)
					{
						KillTile(x, num + k);
					}
				}
				if (type == 92)
					Item.NewItem(x * 16, j * 16, 32, 32, 341);
				else if (type == 93)
					Item.NewItem(x * 16, j * 16, 32, 32, 342);
				destroyObject = false;
			}
		}

		public static void Check2xX(int i, int j, byte type)
		{
			if (destroyObject)
				return;

			int num = i;
			int k;
			for (k = (int)Main.tile.At(i, j).FrameX; k >= 36; k -= 36)
			{
			}

			if (k == 18)
				num--;

			int num2 = j - (int)(Main.tile.At(num, j).FrameY / 18);

			int frameX = (int)Main.tile.At(num, num2).FrameX;
			int num3 = 3;

			if (type == 104)
				num3 = 5;

			bool flag = false;
			for (int l = 0; l < num3; l++)
			{
				if (!Main.tile.At(num, num2 + l).Active)
				{
					flag = true;
				}
				else if (Main.tile.At(num, num2 + l).Type != type)
				{
					flag = true;
				}
				else if ((int)Main.tile.At(num, num2 + l).FrameY != l * 18)
				{
					flag = true;
				}
				else if ((int)Main.tile.At(num, num2 + l).FrameX != frameX)
				{
					flag = true;
				}

				if (!Main.tile.At(num + 1, num2 + l).Active)
				{
					flag = true;
				}
				else if (Main.tile.At(num + 1, num2 + l).Type != type)
				{
					flag = true;
				}
				else if ((int)Main.tile.At(num + 1, num2 + l).FrameY != l * 18)
				{
					flag = true;
				}
				else if ((int)Main.tile.At(num + 1, num2 + l).FrameX != frameX + 18)
				{
					flag = true;
				}
			}

			if (!Main.tile.At(num, num2 + num3).Active)
				flag = true;

			if (!Main.tileSolid[(int)Main.tile.At(num, num2 + num3).Type])
				flag = true;

			if (!Main.tile.At(num + 1, num2 + num3).Active)
				flag = true;

			if (!Main.tileSolid[(int)Main.tile.At(num + 1, num2 + num3).Type])
				flag = true;

			if (flag)
			{
				destroyObject = true;
				for (int m = 0; m < num3; m++)
				{
					if (Main.tile.At(num, num2 + m).Type == type)
						KillTile(num, num2 + m);

					if (Main.tile.At(num + 1, num2 + m).Type == type)
						KillTile(num + 1, num2 + m);
				}
				if (type == 104)
					Item.NewItem(num * 16, j * 16, 32, 32, 359);

				if (type == 105)
				{
					int num4 = frameX / 36;
					if (num4 == 0)
						num4 = 360;
					else if (num4 == 1)
						num4 = 52;
					else
						num4 = 438 + num4 - 2;

					Item.NewItem(num * 16, j * 16, 32, 32, num4);
				}
				destroyObject = false;
			}
		}

		public static void PlaceSunflower(int x, int y, int type = 27)
		{
			if ((double)y > Main.worldSurface - 1.0)
				return;

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
						int num = k * 18 + genRand.Next(3) * 36;
						int num2 = (l + 3) * 18;
						Main.tile.At(x + k, y + l).SetActive(true);
						Main.tile.At(x + k, y + l).SetFrameX((short)num);
						Main.tile.At(x + k, y + l).SetFrameY((short)num2);
						Main.tile.At(x + k, y + l).SetType((byte)type);
					}
				}
			}
		}

		public static void CheckSunflower(int i, int j, int type = 27)
		{
			if (destroyObject)
				return;

			bool flag = false;
			int k = 0;
			k += (int)(Main.tile.At(i, j).FrameX / 18);
			int num = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);

			while (k > 1)
				k -= 2;

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
				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 4; num3++)
					{
						if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
						{
							KillTile(num2, num3);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, 63);
				destroyObject = false;
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
						int num = k * 18 + genRand.Next(3) * 36;
						int num2 = (l + 1) * 18;
						Main.tile.At(x + k, y + l).SetActive(true);
						Main.tile.At(x + k, y + l).SetFrameX((short)num);
						Main.tile.At(x + k, y + l).SetFrameY((short)num2);
						Main.tile.At(x + k, y + l).SetType((byte)type);
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
				KillTile(i, j);
				return true;
			}
			if (i != num2)
			{
				if ((!Main.tile.At(i, j + 1).Active || Main.tile.At(i, j + 1).Type != 80) && (!Main.tile.At(i - 1, j).Active || Main.tile.At(i - 1, j).Type != 80) && (!Main.tile.At(i + 1, j).Active || Main.tile.At(i + 1, j).Type != 80))
				{
					KillTile(i, j);
					return true;
				}
			}
			else if (i == num2 && (!Main.tile.At(i, j + 1).Active || (Main.tile.At(i, j + 1).Type != 80 && Main.tile.At(i, j + 1).Type != 53)))
			{
				KillTile(i, j);
				return true;
			}
			return false;
		}

		public static void PlantCactus(int i, int j)
		{
			GrowCactus(i, j);
			for (int k = 0; k < 150; k++)
			{
				int i2 = genRand.Next(i - 1, i + 2);
				int j2 = genRand.Next(j - 10, j + 2);
				GrowCactus(i2, j2);
			}
		}

		public static void CactusFrame(int i, int j)
		{
			try
			{
				int num = j;
				int num2 = i;
				if (!CheckCactus(i, j))
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
						num4 = -1;

					if (!Main.tile.At(i + 1, j).Active)
						num5 = -1;

					if (!Main.tile.At(i, j - 1).Active)
						num6 = -1;

					if (!Main.tile.At(i, j + 1).Active)
						num7 = -1;

					if (!Main.tile.At(i - 1, j + 1).Active)
						num8 = -1;

					if (!Main.tile.At(i + 1, j + 1).Active)
						num9 = -1;

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
						Main.tile.At(i, j).SetFrameX(num10);
						Main.tile.At(i, j).SetFrameY(num11);
						SquareTileFrame(i, j, true);
					}
				}
			}
			catch
			{
				Main.tile.At(i, j).SetFrameX(0);
				Main.tile.At(i, j).SetFrameY(0);
			}
		}

		public static void GrowCactus(int i, int j)
		{
			int num = j;
			int num2 = i;
			if (!Main.tile.At(i, j).Active)
				return;

			if (Main.tile.At(i, j - 1).Liquid > 0)
				return;

			if (Main.tile.At(i, j).Type != 53 && Main.tile.At(i, j).Type != 80)
				return;

			if (Main.tile.At(i, j).Type == 53)
			{
				if (Main.tile.At(i, j - 1).Active || Main.tile.At(i - 1, j - 1).Active ||
					Main.tile.At(i + 1, j - 1).Active)
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
										return;
								}

								if (Main.tile.At(k, l).Type == 53 ||
									Main.tile.At(k, l).Type == 112 ||
									Main.tile.At(k, l).Type == 116)
									num4++;
							}
						}
						catch
						{
						}
					}
				}
				if (num4 > 10)
				{
					Main.tile.At(i, j - 1).SetActive(true);
					Main.tile.At(i, j - 1).SetType(80);

					NetMessage.SendTileSquare(-1, i, j - 1, 1);
					SquareTileFrame(num2, num - 1, true);
					return;
				}
				return;
			}
			else
			{
				if (Main.tile.At(i, j).Type != 80)
					return;

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
				if (num8 < genRand.Next(11, 13))
				{
					num2 = i;
					num = j;
					if (num6 == 0)
					{
						if (num5 == 0)
						{
							if (Main.tile.At(num2, num - 1).Active)
								return;

							Main.tile.At(num2, num - 1).SetActive(true);
							Main.tile.At(num2, num - 1).SetType(80);
							SquareTileFrame(num2, num - 1, true);

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
							int num9 = genRand.Next(3);
							if (num9 == 0 && flag)
							{
								Main.tile.At(num2 - 1, num).SetActive(true);
								Main.tile.At(num2 - 1, num).SetType(80);
								SquareTileFrame(num2 - 1, num, true);
								NetMessage.SendTileSquare(-1, num2 - 1, num, 1);
								return;
							}
							else if (num9 == 1 && flag2)
							{
								Main.tile.At(num2 + 1, num).SetActive(true);
								Main.tile.At(num2 + 1, num).SetType(80);
								SquareTileFrame(num2 + 1, num, true);
								NetMessage.SendTileSquare(-1, num2 + 1, num, 1);

								return;
							}
							else
							{
								if (num5 >= genRand.Next(2, 8))
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
								Main.tile.At(num2, num - 1).SetActive(true);
								Main.tile.At(num2, num - 1).SetType(80);
								SquareTileFrame(num2, num - 1, true);
								NetMessage.SendTileSquare(-1, num2, num - 1, 1);
								return;
							}
						}
					}
					else
					{
						if (Main.tile.At(num2, num - 1).Active || Main.tile.At(num2, num - 2).Active ||
							Main.tile.At(num2 + num6, num - 1).Active ||
							!Main.tile.At(num2 - num6, num - 1).Active || Main.tile.At(num2 - num6, num - 1).Type != 80)
							return;

						Main.tile.At(num2, num - 1).SetActive(true);
						Main.tile.At(num2, num - 1).SetType(80);
						SquareTileFrame(num2, num - 1, true);

						NetMessage.SendTileSquare(-1, num2, num - 1, 1);
						return;
					}
				}
			}
		}

		public static void CheckPot(int i, int j, int type = 28)
		{
			if (destroyObject)
				return;

			bool flag = false;
			int k = 0;
			k += (int)(Main.tile.At(i, j).FrameX / 18);
			int num = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);

			while (k > 1)
				k -= 2;

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
				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 2; num3++)
					{
						if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
						{
							KillTile(num2, num3);
						}
					}
				}
				if (genRand.Next(40) == 0 && (Main.tile.At(k, num).Wall == 7 || Main.tile.At(k, num).Wall == 8 || Main.tile.At(k, num).Wall == 9))
				{
					Item.NewItem(i * 16, j * 16, 16, 16, 327);
				}
				else
				{
					if (genRand.Next(45) == 0)
					{
						if ((double)j < Main.worldSurface)
						{
							int num4 = genRand.Next(4);
							if (num4 == 0)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 292);
							}
							if (num4 == 1)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 298);
							}
							if (num4 == 2)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 299);
							}
							if (num4 == 3)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 290);
							}
						}
						else if ((double)j < Main.rockLayer)
						{
							int num5 = genRand.Next(7);
							if (num5 == 0)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 289);
							}
							if (num5 == 1)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 298);
							}
							if (num5 == 2)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 299);
							}
							if (num5 == 3)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 290);
							}
							if (num5 == 4)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 303);
							}
							if (num5 == 5)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 291);
							}
							if (num5 == 6)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 304);
							}
						}
						else if (j < Main.maxTilesY - 200)
						{
							int num6 = genRand.Next(10);
							if (num6 == 0)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 296);
							}
							if (num6 == 1)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 295);
							}
							if (num6 == 2)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 299);
							}
							if (num6 == 3)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 302);
							}
							if (num6 == 4)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 303);
							}
							if (num6 == 5)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 305);
							}
							if (num6 == 6)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 301);
							}
							if (num6 == 7)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 302);
							}
							if (num6 == 8)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 297);
							}
							if (num6 == 9)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 304);
							}
						}
						else
						{
							int num7 = genRand.Next(12);
							if (num7 == 0)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 296);
							}
							if (num7 == 1)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 295);
							}
							if (num7 == 2)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 293);
							}
							if (num7 == 3)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 288);
							}
							if (num7 == 4)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 294);
							}
							if (num7 == 5)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 297);
							}
							if (num7 == 6)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 304);
							}
							if (num7 == 7)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 305);
							}
							if (num7 == 8)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 301);
							}
							if (num7 == 9)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 302);
							}
							if (num7 == 10)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 288);
							}
							if (num7 == 11)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 300);
							}
						}
					}
					else
					{
						int num8 = Main.rand.Next(8);
						if (num8 == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLife < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLifeMax)
						{
							Item.NewItem(i * 16, j * 16, 16, 16, 58);
						}
						else if (num8 == 1 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statMana < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statManaMax)
						{
							Item.NewItem(i * 16, j * 16, 16, 16, 184);
						}
						else if (num8 == 2)
						{
							int stack = Main.rand.Next(1, 6);
							if (Main.tile.At(i, j).Liquid > 0)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 282, stack);
							}
							else
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 8, stack);
							}
						}
						else if (num8 == 3)
						{
							int stack2 = Main.rand.Next(8) + 3;
							int type2 = 40;
							if ((double)j < Main.rockLayer && genRand.Next(2) == 0)
							{
								if (Main.hardMode)
								{
									type2 = 168;
								}
								else
								{
									type2 = 42;
								}
							}
							if (j > Main.maxTilesY - 200)
							{
								type2 = 265;
							}
							else if (Main.hardMode)
							{
								if (Main.rand.Next(2) == 0)
								{
									type2 = 278;
								}
								else
								{
									type2 = 47;
								}
							}
							Item.NewItem(i * 16, j * 16, 16, 16, type2, stack2);
						}
						else if (num8 == 4)
						{
							int type3 = 28;
							if (j > Main.maxTilesY - 200 || Main.hardMode)
							{
								type3 = 188;
							}
							Item.NewItem(i * 16, j * 16, 16, 16, type3);
						}
						else if (num8 == 5 && (double)j > Main.rockLayer)
						{
							int stack3 = Main.rand.Next(4) + 1;
							Item.NewItem(i * 16, j * 16, 16, 16, 166, stack3);
						}
						else
						{
							float num9 = (float)(200 + genRand.Next(-100, 101));
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
									Item.NewItem(i * 16, j * 16, 16, 16, 74, num10);
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
									Item.NewItem(i * 16, j * 16, 16, 16, 73, num11);
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
									Item.NewItem(i * 16, j * 16, 16, 16, 72, num12);
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
									Item.NewItem(i * 16, j * 16, 16, 16, 71, num13);
								}
							}
						}
					}
				}
				destroyObject = false;
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
					TileRef tile = Main.tile.At(modX, modY);
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
				for (int k = x - 25; k < x + 25; k++)
				{
					for (int l = y - 8; l < y + 8; l++)
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
					flag = false;
			}
			if (flag)
			{
				Main.tile.At(x, y - 1).SetActive(true);
				Main.tile.At(x, y - 1).SetFrameY(0);
				Main.tile.At(x, y - 1).SetFrameX((short)(36 * style));
				Main.tile.At(x, y - 1).SetType((byte)type);
				Main.tile.At(x + 1, y - 1).SetActive(true);
				Main.tile.At(x + 1, y - 1).SetFrameY(0);
				Main.tile.At(x + 1, y - 1).SetFrameX((short)(18 + 36 * style));
				Main.tile.At(x + 1, y - 1).SetType((byte)type);
				Main.tile.At(x, y).SetActive(true);
				Main.tile.At(x, y).SetFrameY(18);
				Main.tile.At(x, y).SetFrameX((short)(36 * style));
				Main.tile.At(x, y).SetType((byte)type);
				Main.tile.At(x + 1, y).SetActive(true);
				Main.tile.At(x + 1, y).SetFrameY(18);
				Main.tile.At(x + 1, y).SetFrameX((short)(18 + 36 * style));
				Main.tile.At(x + 1, y).SetType((byte)type);
			}
			return num;
		}

		public static void CheckChest(int i, int j, int type)
		{
			if (destroyObject)
				return;

			bool flag = false;
			int k = 0;
			k += (int)(Main.tile.At(i, j).FrameX / 18);
			int num = j + (int)(Main.tile.At(i, j).FrameY / 18 * -1);

			while (k > 1)
				k -= 2;

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
				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 3; num3++)
					{
						if ((int)Main.tile.At(num2, num3).Type == type && Main.tile.At(num2, num3).Active)
						{
							Chest.DestroyChest(num2, num3);
							KillTile(num2, num3);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, type2);
				destroyObject = false;
			}
		}

		public static void Place1xX(int x, int y, int type, int style = 0)
		{
			int num = style * 18;
			int num2 = 3;
			if (type == 92)
				num2 = 6;

			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (Main.tile.At(x, i).Active)
					flag = false;

				if (type == 93 && Main.tile.At(x, i).Liquid > 0)
					flag = false;
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
			int num = style * 36;
			int num2 = 3;
			if (type == 104)
				num2 = 5;

			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (Main.tile.At(x, i).Active)
					flag = false;

				if (Main.tile.At(x + 1, i).Active)
					flag = false;
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
				return;

			bool flag = true;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 3; j < y + 1; j++)
				{
					if (Main.tile.At(i, j).Active)
						flag = false;
				}

				if (!Main.tile.At(i, y + 1).Active || !Main.tileSolid[(int)Main.tile.At(i, y + 1).Type])
					flag = false;
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
					Main.tile.At(i, j).SetFrameY(0);
					Main.tile.At(i, j).SetFrameX(0);
					if (type == 3 || type == 24)
					{
						if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active && ((Main.tile.At(i, j + 1).Type == 2 && type == 3) || (Main.tile.At(i, j + 1).Type == 23 && type == 24) || (Main.tile.At(i, j + 1).Type == 78 && type == 3) || (Main.tile.At(i, j + 1).Type == 109 && type == 110)))
						{
							if (type == 24 && genRand.Next(13) == 0)
							{
								Main.tile.At(i, j).SetActive(true);
								Main.tile.At(i, j).SetType(32);
								SquareTileFrame(i, j);
							}
							else if (Main.tile.At(i, j + 1).Type == 78)
							{
								Main.tile.At(i, j).SetActive(true);
								Main.tile.At(i, j).SetType((byte)type);
								Main.tile.At(i, j).SetFrameX((short)(genRand.Next(2) * 18 + 108));
							}
							else if (Main.tile.At(i, j).Wall == 0 && Main.tile.At(i, j + 1).Wall == 0)
							{
								if (genRand.Next(50) == 0 || (type == 24 && genRand.Next(40) == 0))
								{
									Main.tile.At(i, j).SetActive(true);
									Main.tile.At(i, j).SetType((byte)type);
									Main.tile.At(i, j).SetFrameX(144);
								}
								else if (genRand.Next(35) == 0)
								{
									Main.tile.At(i, j).SetActive(true);
									Main.tile.At(i, j).SetType((byte)type);
									Main.tile.At(i, j).SetFrameX((short)(genRand.Next(2) * 18 + 108));
								}
								else
								{
									Main.tile.At(i, j).SetActive(true);
									Main.tile.At(i, j).SetType((byte)type);
									Main.tile.At(i, j).SetFrameX((short)(genRand.Next(6) * 18));
								}
							}
						}
					}
					else if (type == 61)
					{
						if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 60)
						{
							if (genRand.Next(10) == 0 && (double)j > Main.worldSurface)
							{
								Main.tile.At(i, j).SetActive(true);
								Main.tile.At(i, j).SetType(69);
								SquareTileFrame(i, j);
							}
							else if (genRand.Next(15) == 0 && (double)j > Main.worldSurface)
							{
								Main.tile.At(i, j).SetActive(true);
								Main.tile.At(i, j).SetType((byte)type);
								Main.tile.At(i, j).SetFrameX(144);
							}
							else if (genRand.Next(1000) == 0 && (double)j > Main.rockLayer)
							{
								Main.tile.At(i, j).SetActive(true);
								Main.tile.At(i, j).SetType((byte)type);
								Main.tile.At(i, j).SetFrameX(162);
							}
							else
							{
								if (genRand.Next(15) == 0)
								{
									Main.tile.At(i, j).SetActive(true);
									Main.tile.At(i, j).SetType((byte)type);
									Main.tile.At(i, j).SetFrameX((short)(genRand.Next(2) * 18 + 108));
								}
								else
								{
									Main.tile.At(i, j).SetActive(true);
									Main.tile.At(i, j).SetType((byte)type);
									Main.tile.At(i, j).SetFrameX((short)(genRand.Next(6) * 18));
								}
							}
						}
					}
					else if (type == 71)
					{
						if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active && Main.tile.At(i, j + 1).Type == 70)
						{
							Main.tile.At(i, j).SetActive(true);
							Main.tile.At(i, j).SetType((byte)type);
							Main.tile.At(i, j).SetFrameX((short)(genRand.Next(5) * 18));
						}
					}
					else if (type == 129)
					{
						if (SolidTile(i - 1, j) || SolidTile(i + 1, j) || SolidTile(i, j - 1) || SolidTile(i, j + 1))
						{
							Main.tile.At(i, j).SetActive(true);
							Main.tile.At(i, j).SetType((byte)type);
							Main.tile.At(i, j).SetFrameX((short)(genRand.Next(8) * 18));
							SquareTileFrame(i, j);
						}
					}
					else if (type == 132 || type == 138 || type == 142 || type == 143)
					{
						Place2x2(i, j, type);
					}
					else if (type == 137)
					{
						Main.tile.At(i, j).SetActive(true);
						Main.tile.At(i, j).SetType((byte)type);
						if (style == 1)
						{
							Main.tile.At(i, j).SetFrameX(18);
						}
					}
					else if (type == 136)
					{
						if ((Main.tile.At(i - 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i - 1, j).Type] ||
							Main.tile.At(i - 1, j).Type == 124 || (Main.tile.At(i - 1, j).Type == 5 && Main.tile.At(i - 1, j - 1).Type == 5 && Main.tile.At(i - 1, j + 1).Type == 5))) ||
							(Main.tile.At(i + 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i + 1, j).Type] || Main.tile.At(i + 1, j).Type == 124 || (
							Main.tile.At(i + 1, j).Type == 5 && Main.tile.At(i + 1, j - 1).Type == 5 && Main.tile.At(i + 1, j + 1).Type == 5))) ||
							(Main.tile.At(i, j + 1).Active && Main.tileSolid[(int)Main.tile.At(i, j + 1).Type]))
						{
							Main.tile.At(i, j).SetActive(true);
							Main.tile.At(i, j).SetType((byte)type);
							SquareTileFrame(i, j);
						}
					}
					else if (type == 4)
					{
						if ((Main.tile.At(i - 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i - 1, j).Type] || (Main.tile.At(i - 1, j).Type == 5 && Main.tile.At(i - 1, j - 1).Type == 5 && Main.tile.At(i - 1, j + 1).Type == 5))) || (Main.tile.At(i + 1, j).Active && (Main.tileSolid[(int)Main.tile.At(i + 1, j).Type] || (Main.tile.At(i + 1, j).Type == 5 && Main.tile.At(i + 1, j - 1).Type == 5 && Main.tile.At(i + 1, j + 1).Type == 5))) || (Main.tile.At(i, j + 1).Active && Main.tileSolid[(int)Main.tile.At(i, j + 1).Type]))
						{
							Main.tile.At(i, j).SetActive(true);
							Main.tile.At(i, j).SetType((byte)type);
							SquareTileFrame(i, j);
						}
					}
					else if (type == 10)
					{
						if (!Main.tile.At(i, j - 1).Active && !Main.tile.At(i, j - 2).Active && Main.tile.At(i, j - 3).Active && Main.tileSolid[(int)Main.tile.At(i, j - 3).Type])
						{
							PlaceDoor(i, j - 1, type);
							SquareTileFrame(i, j);
						}
						else
						{
							if (Main.tile.At(i, j + 1).Active || Main.tile.At(i, j + 2).Active || !Main.tile.At(i, j + 3).Active || !Main.tileSolid[(int)Main.tile.At(i, j + 3).Type])
							{
								return false;
							}
							PlaceDoor(i, j + 1, type);
							SquareTileFrame(i, j);
						}
					}
					else if (type == 128)
					{
						PlaceMan(i, j, style);
						SquareTileFrame(i, j);
					}
					else if (type == 139)
					{
						PlaceMB(i, j, type, style);
						SquareTileFrame(i, j);
					}
					else if (type == 34 || type == 35 || type == 36 || type == 106)
					{
						Place3x3(i, j, type);
						SquareTileFrame(i, j);
					}
					else if (type == 13 || type == 33 || type == 49 || type == 50 || type == 78)
					{
						PlaceOnTable1x1(i, j, type, style);
						SquareTileFrame(i, j);
					}
					else if (type == 14 || type == 26 || type == 86 || type == 87 || type == 88 || type == 89 || type == 114)
					{
						Place3x2(i, j, type);
						SquareTileFrame(i, j);
					}
					else if (type == 20)
					{
						if (Main.tile.At(i, j + 1).Active && (Main.tile.At(i, j + 1).Type == 2 || Main.tile.At(i, j + 1).Type == 109))
						{
							Place1x2(i, j, type, style);
							SquareTileFrame(i, j);
						}
					}
					else if (type == 15)
					{
						Place1x2(i, j, type, style);
						SquareTileFrame(i, j);
					}
					else if (type == 16 || type == 18 || type == 29 || type == 103 || type == 134)
					{
						Place2x1(i, j, type);
						SquareTileFrame(i, j);
					}
					else if (type == 92 || type == 93)
					{
						Place1xX(i, j, type, 0);
						SquareTileFrame(i, j);
					}
					else if (type == 104 || type == 105)
					{
						Place2xX(i, j, type, 0);
						SquareTileFrame(i, j);
					}
					else if (type == 17 || type == 77 || type == 133)
					{
						Place3x2(i, j, type);
						SquareTileFrame(i, j);
					}
					else if (type == 21)
					{
						PlaceChest(i, j, type, false, style);
						SquareTileFrame(i, j);
					}
					else if (type == 91)
					{
						PlaceBanner(i, j, type, style);
						SquareTileFrame(i, j);
					}
					else if (type == 135 || type == 141 || type == 144)
					{
						Place1x1(i, j, type, style);
						SquareTileFrame(i, j);
					}
					else if (type == 101 || type == 102)
					{
						Place3x4(i, j, type);
						SquareTileFrame(i, j);
					}
					else if (type == 27)
					{
						PlaceSunflower(i, j, 27);
						SquareTileFrame(i, j);
					}
					else if (type == 28)
					{
						PlacePot(i, j, 28);
						SquareTileFrame(i, j);
					}
					else if (type == 42)
					{
						Place1x2Top(i, j, type);
						SquareTileFrame(i, j);
					}
					else if (type == 55 || type == 85)
					{
						PlaceSign(i, j, type);
					}
					else if (Main.tileAlch[type])
					{
						PlaceAlch(i, j, style);
					}
					else if (type == 94 || type == 95 || type == 96 || type == 97 ||
						type == 98 || type == 99 || type == 100 || type == 125 || type == 126)
					{
						Place2x2(i, j, type);
					}
					else if (type == 79 || type == 90)
					{
						int direction = 1;
						if (plr > -1)
						{
							direction = Main.players[plr].direction;
						}
						Place4x2(i, j, type, direction);
					}
					else if (type == 81)
					{
						Main.tile.At(i, j).SetFrameX((short)(26 * genRand.Next(6)));
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
						SquareTileFrame(i, j);
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
						SquareWallFrame(i, j);
						return;
					}

					int num2 = 0;
					if (Main.tile.At(i, j).Wall == 1)
						num2 = 26;

					if (Main.tile.At(i, j).Wall == 4)
						num2 = 93;

					if (Main.tile.At(i, j).Wall == 5)
						num2 = 130;
					if (Main.tile.At(i, j).Wall == 6)
						num2 = 132;

					if (Main.tile.At(i, j).Wall == 7)
						num2 = 135;

					if (Main.tile.At(i, j).Wall == 8)
						num2 = 138;

					if (Main.tile.At(i, j).Wall == 9)
						num2 = 140;

					if (Main.tile.At(i, j).Wall == 10)
						num2 = 142;

					if (Main.tile.At(i, j).Wall == 11)
						num2 = 144;

					if (Main.tile.At(i, j).Wall == 12)
						num2 = 146;

					if (Main.tile.At(i, j).Wall == 14)
						num2 = 330;

					if (Main.tile.At(i, j).Wall == 16)
						num2 = 30;

					if (Main.tile.At(i, j).Wall == 17)
						num2 = 135;

					if (Main.tile.At(i, j).Wall == 18)
						num2 = 138;

					if (Main.tile.At(i, j).Wall == 19)
						num2 = 140;

					if (Main.tile.At(i, j).Wall == 20)
						num2 = 330;

					if (Main.tile.At(i, j).Wall == 21)
						num2 = 392;

					if (Main.tile.At(i, j).Wall == 22)
						num2 = 417;

					if (Main.tile.At(i, j).Wall == 23)
						num2 = 418;

					if (Main.tile.At(i, j).Wall == 24)
						num2 = 419;

					if (Main.tile.At(i, j).Wall == 25)
						num2 = 420;

					if (Main.tile.At(i, j).Wall == 26)
						num2 = 421;

					if (Main.tile.At(i, j).Wall == 27)
						num2 = 479;

					if (num2 > 0)
						Item.NewItem(i * 16, j * 16, 16, 16, num2);

					Main.tile.At(i, j).SetWall(0);
					SquareWallFrame(i, j);
				}
			}
		}

		public static void KillTile(int i, int j, bool fail = false, bool effectOnly = false, bool noItem = false)
		{
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (Main.tile.At(i, j).Active)
				{
					if (j >= 1 && Main.tile.At(i, j - 1).Active && ((Main.tile.At(i, j - 1).Type == 5 && Main.tile.At(i, j).Type != 5) ||
						(Main.tile.At(i, j - 1).Type == 21 && Main.tile.At(i, j).Type != 21) || (Main.tile.At(i, j - 1).Type == 26 &&
						Main.tile.At(i, j).Type != 26) || (Main.tile.At(i, j - 1).Type == 72 && Main.tile.At(i, j).Type != 72) ||
						(Main.tile.At(i, j - 1).Type == 12 && Main.tile.At(i, j).Type != 12)) && (Main.tile.At(i, j - 1).Type != 5 ||
						((Main.tile.At(i, j - 1).FrameX != 66 || Main.tile.At(i, j - 1).FrameY < 0 || Main.tile.At(i, j - 1).FrameY > 44) &&
							(Main.tile.At(i, j - 1).FrameX != 88 || Main.tile.At(i, j - 1).FrameY < 66 || Main.tile.At(i, j - 1).FrameY > 110) &&
								Main.tile.At(i, j - 1).FrameY < 198)))
					{
						return;
					}
					if (!effectOnly && !stopDrops)
					{
						if (Main.tile.At(i, j).Type == 3 || Main.tile.At(i, j).Type == 110)
						{
							if (Main.tile.At(i, j).FrameX == 144)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 5);
							}
						}
						else
						{
							if (Main.tile.At(i, j).Type == 24)
							{
								if (Main.tile.At(i, j).FrameX == 144)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 60);
								}
							}
						}
					}
					int num = 10;
					if (Main.tile.At(i, j).Type == 128)
					{
						int num2 = i;
						int k = (int)Main.tile.At(i, j).FrameX;
						int l;
						for (l = (int)Main.tile.At(i, j).FrameX; l >= 100; l -= 100)
						{
						}
						while (l >= 36)
						{
							l -= 36;
						}
						if (l == 18)
						{
							k = (int)Main.tile.At(i - 1, j).FrameX;
							num2--;
						}
						if (k >= 100)
						{
							int num3 = 0;
							while (k >= 100)
							{
								k -= 100;
								num3++;
							}
							int num4 = (int)(Main.tile.At(num2, j).FrameY / 18);
							if (num4 == 0)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, Item.headType[num3], 1, false, 0);
							}
							if (num4 == 1)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, Item.bodyType[num3], 1, false, 0);
							}
							if (num4 == 2)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, Item.legType[num3], 1, false, 0);
							}
							for (k = (int)Main.tile.At(num2, j).FrameX; k >= 100; k -= 100)
							{
							}
							Main.tile.At(num2, j).SetFrameX((short)k);
						}
					}
					if (fail)
					{
						num = 3;
					}
					if (Main.tile.At(i, j).Type == 138)
					{
						num = 0;
					}
					for (int m = 0; m < num; m++)
					{
						int num5 = 0;
						if (Main.tile.At(i, j).Type == 0)
						{
							num5 = 0;
						}
						if (Main.tile.At(i, j).Type == 1 || Main.tile.At(i, j).Type == 16 || Main.tile.At(i, j).Type == 17 || Main.tile.At(i, j).Type == 38 || Main.tile.At(i, j).Type == 39 || Main.tile.At(i, j).Type == 41 || Main.tile.At(i, j).Type == 43 || Main.tile.At(i, j).Type == 44 || Main.tile.At(i, j).Type == 48 || Main.tileStone[(int)Main.tile.At(i, j).Type] || Main.tile.At(i, j).Type == 85 || Main.tile.At(i, j).Type == 90 || Main.tile.At(i, j).Type == 92 || Main.tile.At(i, j).Type == 96 || Main.tile.At(i, j).Type == 97 || Main.tile.At(i, j).Type == 99 || Main.tile.At(i, j).Type == 105 || Main.tile.At(i, j).Type == 117 || Main.tile.At(i, j).Type == 130 || Main.tile.At(i, j).Type == 131 || Main.tile.At(i, j).Type == 132 || Main.tile.At(i, j).Type == 135 || Main.tile.At(i, j).Type == 135 || Main.tile.At(i, j).Type == 137 || Main.tile.At(i, j).Type == 142 || Main.tile.At(i, j).Type == 143 || Main.tile.At(i, j).Type == 144)
						{
							num5 = 1;
						}
						if (Main.tile.At(i, j).Type == 33 || Main.tile.At(i, j).Type == 95 || Main.tile.At(i, j).Type == 98 || Main.tile.At(i, j).Type == 100)
						{
							num5 = 6;
						}
						if (Main.tile.At(i, j).Type == 5 || Main.tile.At(i, j).Type == 10 || Main.tile.At(i, j).Type == 11 || Main.tile.At(i, j).Type == 14 || Main.tile.At(i, j).Type == 15 || Main.tile.At(i, j).Type == 19 || Main.tile.At(i, j).Type == 30 || Main.tile.At(i, j).Type == 86 || Main.tile.At(i, j).Type == 87 || Main.tile.At(i, j).Type == 88 || Main.tile.At(i, j).Type == 89 || Main.tile.At(i, j).Type == 93 || Main.tile.At(i, j).Type == 94 || Main.tile.At(i, j).Type == 104 || Main.tile.At(i, j).Type == 106 || Main.tile.At(i, j).Type == 114 || Main.tile.At(i, j).Type == 124 || Main.tile.At(i, j).Type == 128 || Main.tile.At(i, j).Type == 139)
						{
							num5 = 7;
						}
						if (Main.tile.At(i, j).Type == 21)
						{
							if (Main.tile.At(i, j).FrameX >= 108)
							{
								num5 = 37;
							}
							else if (Main.tile.At(i, j).FrameX >= 36)
							{
								num5 = 10;
							}
							else
							{
								num5 = 7;
							}
						}
						if (Main.tile.At(i, j).Type == 2)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 0;
							}
							else
							{
								num5 = 2;
							}
						}
						if (Main.tile.At(i, j).Type == 127)
						{
							num5 = 67;
						}
						if (Main.tile.At(i, j).Type == 91)
						{
							num5 = -1;
						}
						if (Main.tile.At(i, j).Type == 6 || Main.tile.At(i, j).Type == 26)
						{
							num5 = 8;
						}
						if (Main.tile.At(i, j).Type == 7 || Main.tile.At(i, j).Type == 34 || Main.tile.At(i, j).Type == 47)
						{
							num5 = 9;
						}
						if (Main.tile.At(i, j).Type == 8 || Main.tile.At(i, j).Type == 36 || Main.tile.At(i, j).Type == 45 || Main.tile.At(i, j).Type == 102)
						{
							num5 = 10;
						}
						if (Main.tile.At(i, j).Type == 9 || Main.tile.At(i, j).Type == 35 || Main.tile.At(i, j).Type == 42 || Main.tile.At(i, j).Type == 46 || Main.tile.At(i, j).Type == 126 || Main.tile.At(i, j).Type == 136)
						{
							num5 = 11;
						}
						if (Main.tile.At(i, j).Type == 12)
						{
							num5 = 12;
						}
						if (Main.tile.At(i, j).Type == 3 || Main.tile.At(i, j).Type == 73)
						{
							num5 = 3;
						}
						if (Main.tile.At(i, j).Type == 13 || Main.tile.At(i, j).Type == 54)
						{
							num5 = 13;
						}
						if (Main.tile.At(i, j).Type == 22 || Main.tile.At(i, j).Type == 140)
						{
							num5 = 14;
						}
						if (Main.tile.At(i, j).Type == 28 || Main.tile.At(i, j).Type == 78)
						{
							num5 = 22;
						}
						if (Main.tile.At(i, j).Type == 29)
						{
							num5 = 23;
						}
						if (Main.tile.At(i, j).Type == 40 || Main.tile.At(i, j).Type == 103)
						{
							num5 = 28;
						}
						if (Main.tile.At(i, j).Type == 49)
						{
							num5 = 29;
						}
						if (Main.tile.At(i, j).Type == 50)
						{
							num5 = 22;
						}
						if (Main.tile.At(i, j).Type == 51)
						{
							num5 = 30;
						}
						if (Main.tile.At(i, j).Type == 52)
						{
							num5 = 3;
						}
						if (Main.tile.At(i, j).Type == 53 || Main.tile.At(i, j).Type == 81)
						{
							num5 = 32;
						}
						if (Main.tile.At(i, j).Type == 56 || Main.tile.At(i, j).Type == 75)
						{
							num5 = 37;
						}
						if (Main.tile.At(i, j).Type == 57 || Main.tile.At(i, j).Type == 119 || Main.tile.At(i, j).Type == 141)
						{
							num5 = 36;
						}
						if (Main.tile.At(i, j).Type == 59 || Main.tile.At(i, j).Type == 120)
						{
							num5 = 38;
						}
						if (Main.tile.At(i, j).Type == 61 || Main.tile.At(i, j).Type == 62 || Main.tile.At(i, j).Type == 74 || Main.tile.At(i, j).Type == 80)
						{
							num5 = 40;
						}
						if (Main.tile.At(i, j).Type == 69)
						{
							num5 = 7;
						}
						if (Main.tile.At(i, j).Type == 71 || Main.tile.At(i, j).Type == 72)
						{
							num5 = 26;
						}
						if (Main.tile.At(i, j).Type == 70)
						{
							num5 = 17;
						}
						if (Main.tile.At(i, j).Type == 112)
						{
							num5 = 14;
						}
						if (Main.tile.At(i, j).Type == 123)
						{
							num5 = 53;
						}
						if (Main.tile.At(i, j).Type == 116 || Main.tile.At(i, j).Type == 118)
						{
							num5 = 51;
						}
						if (Main.tile.At(i, j).Type == 109)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 0;
							}
							else
							{
								num5 = 47;
							}
						}
						if (Main.tile.At(i, j).Type == 110 || Main.tile.At(i, j).Type == 113 || Main.tile.At(i, j).Type == 115)
						{
							num5 = 47;
						}
						if (Main.tile.At(i, j).Type == 107 || Main.tile.At(i, j).Type == 121)
						{
							num5 = 48;
						}
						if (Main.tile.At(i, j).Type == 108 || Main.tile.At(i, j).Type == 122 || Main.tile.At(i, j).Type == 134)
						{
							num5 = 49;
						}
						if (Main.tile.At(i, j).Type == 111 || Main.tile.At(i, j).Type == 133)
						{
							num5 = 50;
						}
						if (Main.tileAlch[(int)Main.tile.At(i, j).Type])
						{
							int num6 = (int)(Main.tile.At(i, j).FrameX / 18);
							if (num6 == 0)
							{
								num5 = 3;
							}
							if (num6 == 1)
							{
								num5 = 3;
							}
							if (num6 == 2)
							{
								num5 = 7;
							}
							if (num6 == 3)
							{
								num5 = 17;
							}
							if (num6 == 4)
							{
								num5 = 3;
							}
							if (num6 == 5)
							{
								num5 = 6;
							}
						}
						if (Main.tile.At(i, j).Type == 61)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 38;
							}
							else
							{
								num5 = 39;
							}
						}
						if (Main.tile.At(i, j).Type == 58 || Main.tile.At(i, j).Type == 76 || Main.tile.At(i, j).Type == 77)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 6;
							}
							else
							{
								num5 = 25;
							}
						}
						if (Main.tile.At(i, j).Type == 37)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 6;
							}
							else
							{
								num5 = 23;
							}
						}
						if (Main.tile.At(i, j).Type == 32)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 14;
							}
							else
							{
								num5 = 24;
							}
						}
						if (Main.tile.At(i, j).Type == 23 || Main.tile.At(i, j).Type == 24)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 14;
							}
							else
							{
								num5 = 17;
							}
						}
						if (Main.tile.At(i, j).Type == 25 || Main.tile.At(i, j).Type == 31)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 14;
							}
							else
							{
								num5 = 1;
							}
						}
						if (Main.tile.At(i, j).Type == 20)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 7;
							}
							else
							{
								num5 = 2;
							}
						}
						if (Main.tile.At(i, j).Type == 27)
						{
							if (genRand.Next(2) == 0)
							{
								num5 = 3;
							}
							else
							{
								num5 = 19;
							}
						}
						if (Main.tile.At(i, j).Type == 129)
						{
							if (Main.tile.At(i, j).FrameX == 0 || Main.tile.At(i, j).FrameX == 54 || Main.tile.At(i, j).FrameX == 108)
							{
								num5 = 68;
							}
							else if (Main.tile.At(i, j).FrameX == 18 || Main.tile.At(i, j).FrameX == 72 || Main.tile.At(i, j).FrameX == 126)
							{
								num5 = 69;
							}
							else
							{
								num5 = 70;
							}
						}
						if (Main.tile.At(i, j).Type == 4)
						{
							int num7 = (int)(Main.tile.At(i, j).FrameY / 22);
							if (num7 == 0)
							{
								num5 = 6;
							}
							else if (num7 == 8)
							{
								num5 = 75;
							}
							else
							{
								num5 = 58 + num7;
							}
						}
						if ((Main.tile.At(i, j).Type == 34 || Main.tile.At(i, j).Type == 35 || Main.tile.At(i, j).Type == 36 || Main.tile.At(i, j).Type == 42) && Main.rand.Next(2) == 0)
						{
							num5 = 6;
						}
					}
					if (effectOnly)
					{
						return;
					}
					if (fail)
					{
						if (Main.tile.At(i, j).Type == 2 || Main.tile.At(i, j).Type == 23 || Main.tile.At(i, j).Type == 109)
						{
							Main.tile.At(i, j).SetType(0);
						}
						if (Main.tile.At(i, j).Type == 60 || Main.tile.At(i, j).Type == 70)
						{
							Main.tile.At(i, j).SetType(59);
						}
						SquareTileFrame(i, j, true);
						return;
					}
					if (Main.tile.At(i, j).Type == 21)
					{
						int n = (int)(Main.tile.At(i, j).FrameX / 18);
						int y = j - (int)(Main.tile.At(i, j).FrameY / 18);
						while (n > 1)
						{
							n -= 2;
						}
						n = i - n;
						if (!Chest.DestroyChest(n, y))
						{
							return;
						}
					}
					if (!noItem && !stopDrops)
					{
						int num8 = 0;
						if (Main.tile.At(i, j).Type == 0 || Main.tile.At(i, j).Type == 2 || Main.tile.At(i, j).Type == 109)
						{
							num8 = 2;
						}
						else if (Main.tile.At(i, j).Type == 1)
						{
							num8 = 3;
						}
						else if (Main.tile.At(i, j).Type == 3 || Main.tile.At(i, j).Type == 73)
						{
							if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].HasItem(281))
							{
								num8 = 283;
							}
						}
						else if (Main.tile.At(i, j).Type == 4)
						{
							int num9 = (int)(Main.tile.At(i, j).FrameY / 22);
							if (num9 == 0)
							{
								num8 = 8;
							}
							else if (num9 == 8)
							{
								num8 = 523;
							}
							else
							{
								num8 = 426 + num9;
							}
						}
						else if (Main.tile.At(i, j).Type == 5)
						{
							if (Main.tile.At(i, j).FrameX >= 22 && Main.tile.At(i, j).FrameY >= 198)
							{
								if (genRand.Next(2) == 0)
								{
									int num10 = j;
									while ((!Main.tile.At(i, num10).Active || !Main.tileSolid[(int)Main.tile.At(i, num10).Type] ||
										Main.tileSolidTop[(int)Main.tile.At(i, num10).Type]))
									{
										num10++;
									}
									if (Main.tile.At(i, num10).Type == 2 || Main.tile.At(i, num10).Type == 109)
									{
										num8 = 27;
									}
									else
									{
										num8 = 9;
									}
								}
								else
								{
									num8 = 9;
								}
							}
							else
							{
								num8 = 9;
							}
						}
						else if (Main.tile.At(i, j).Type == 6)
						{
							num8 = 11;
						}
						else if (Main.tile.At(i, j).Type == 7)
						{
							num8 = 12;
						}
						else if (Main.tile.At(i, j).Type == 8)
						{
							num8 = 13;
						}
						else if (Main.tile.At(i, j).Type == 9)
						{
							num8 = 14;
						}
						else if (Main.tile.At(i, j).Type == 123)
						{
							num8 = 424;
						}
						else if (Main.tile.At(i, j).Type == 124)
						{
							num8 = 480;
						}
						else if (Main.tile.At(i, j).Type == 13)
						{
							if (Main.tile.At(i, j).FrameX == 18)
							{
								num8 = 28;
							}
							else if (Main.tile.At(i, j).FrameX == 36)
							{
								num8 = 110;
							}
							else if (Main.tile.At(i, j).FrameX == 54)
							{
								num8 = 350;
							}
							else if (Main.tile.At(i, j).FrameX == 72)
							{
								num8 = 351;
							}
							else
							{
								num8 = 31;
							}
						}
						else if (Main.tile.At(i, j).Type == 19)
						{
							num8 = 94;
						}
						else if (Main.tile.At(i, j).Type == 22)
						{
							num8 = 56;
						}
						else if (Main.tile.At(i, j).Type == 140)
						{
							num8 = 577;
						}
						else if (Main.tile.At(i, j).Type == 23)
						{
							num8 = 2;
						}
						else if (Main.tile.At(i, j).Type == 25)
						{
							num8 = 61;
						}
						else if (Main.tile.At(i, j).Type == 30)
						{
							num8 = 9;
						}
						else if (Main.tile.At(i, j).Type == 33)
						{
							num8 = 105;
						}
						else if (Main.tile.At(i, j).Type == 37)
						{
							num8 = 116;
						}
						else if (Main.tile.At(i, j).Type == 38)
						{
							num8 = 129;
						}
						else if (Main.tile.At(i, j).Type == 39)
						{
							num8 = 131;
						}
						else if (Main.tile.At(i, j).Type == 40)
						{
							num8 = 133;
						}
						else if (Main.tile.At(i, j).Type == 41)
						{
							num8 = 134;
						}
						else if (Main.tile.At(i, j).Type == 43)
						{
							num8 = 137;
						}
						else if (Main.tile.At(i, j).Type == 44)
						{
							num8 = 139;
						}
						else if (Main.tile.At(i, j).Type == 45)
						{
							num8 = 141;
						}
						else if (Main.tile.At(i, j).Type == 46)
						{
							num8 = 143;
						}
						else if (Main.tile.At(i, j).Type == 47)
						{
							num8 = 145;
						}
						else if (Main.tile.At(i, j).Type == 48)
						{
							num8 = 147;
						}
						else if (Main.tile.At(i, j).Type == 49)
						{
							num8 = 148;
						}
						else if (Main.tile.At(i, j).Type == 51)
						{
							num8 = 150;
						}
						else if (Main.tile.At(i, j).Type == 53)
						{
							num8 = 169;
						}
						else if (Main.tile.At(i, j).Type == 54)
						{
							num8 = 170;
						}
						else if (Main.tile.At(i, j).Type == 56)
						{
							num8 = 173;
						}
						else if (Main.tile.At(i, j).Type == 57)
						{
							num8 = 172;
						}
						else if (Main.tile.At(i, j).Type == 58)
						{
							num8 = 174;
						}
						else if (Main.tile.At(i, j).Type == 60)
						{
							num8 = 176;
						}
						else if (Main.tile.At(i, j).Type == 70)
						{
							num8 = 176;
						}
						else if (Main.tile.At(i, j).Type == 75)
						{
							num8 = 192;
						}
						else if (Main.tile.At(i, j).Type == 76)
						{
							num8 = 214;
						}
						else if (Main.tile.At(i, j).Type == 78)
						{
							num8 = 222;
						}
						else if (Main.tile.At(i, j).Type == 81)
						{
							num8 = 275;
						}
						else if (Main.tile.At(i, j).Type == 80)
						{
							num8 = 276;
						}
						else if (Main.tile.At(i, j).Type == 107)
						{
							num8 = 364;
						}
						else if (Main.tile.At(i, j).Type == 108)
						{
							num8 = 365;
						}
						else if (Main.tile.At(i, j).Type == 111)
						{
							num8 = 366;
						}
						else if (Main.tile.At(i, j).Type == 112)
						{
							num8 = 370;
						}
						else if (Main.tile.At(i, j).Type == 116)
						{
							num8 = 408;
						}
						else if (Main.tile.At(i, j).Type == 117)
						{
							num8 = 409;
						}
						else if (Main.tile.At(i, j).Type == 129)
						{
							num8 = 502;
						}
						else if (Main.tile.At(i, j).Type == 118)
						{
							num8 = 412;
						}
						else if (Main.tile.At(i, j).Type == 119)
						{
							num8 = 413;
						}
						else if (Main.tile.At(i, j).Type == 120)
						{
							num8 = 414;
						}
						else if (Main.tile.At(i, j).Type == 121)
						{
							num8 = 415;
						}
						else if (Main.tile.At(i, j).Type == 122)
						{
							num8 = 416;
						}
						else if (Main.tile.At(i, j).Type == 136)
						{
							num8 = 538;
						}
						else if (Main.tile.At(i, j).Type == 137)
						{
							num8 = 539;
						}
						else if (Main.tile.At(i, j).Type == 141)
						{
							num8 = 580;
						}
						else if (Main.tile.At(i, j).Type == 135)
						{
							if (Main.tile.At(i, j).FrameY == 0)
							{
								num8 = 529;
							}
							if (Main.tile.At(i, j).FrameY == 18)
							{
								num8 = 541;
							}
							if (Main.tile.At(i, j).FrameY == 36)
							{
								num8 = 542;
							}
							if (Main.tile.At(i, j).FrameY == 54)
							{
								num8 = 543;
							}
						}
						else if (Main.tile.At(i, j).Type == 144)
						{
							if (Main.tile.At(i, j).FrameX == 0)
							{
								num8 = 583;
							}
							if (Main.tile.At(i, j).FrameX == 18)
							{
								num8 = 584;
							}
							if (Main.tile.At(i, j).FrameX == 36)
							{
								num8 = 585;
							}
						}
						else if (Main.tile.At(i, j).Type == 130)
						{
							num8 = 511;
						}
						else if (Main.tile.At(i, j).Type == 131)
						{
							num8 = 512;
						}
						else if (Main.tile.At(i, j).Type == 61 || Main.tile.At(i, j).Type == 74)
						{
							if (Main.tile.At(i, j).FrameX == 144)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 331, genRand.Next(2, 4), false, 0);
							}
							else
							{
								if (Main.tile.At(i, j).FrameX == 162)
								{
									num8 = 223;
								}
								else if (Main.tile.At(i, j).FrameX >= 108 && Main.tile.At(i, j).FrameX <= 126 && genRand.Next(100) == 0)
								{
									num8 = 208;
								}
								else if (genRand.Next(100) == 0)
								{
									num8 = 195;
								}
							}
						}
						else if (Main.tile.At(i, j).Type == 59 || Main.tile.At(i, j).Type == 60)
						{
							num8 = 176;
						}
						else if (Main.tile.At(i, j).Type == 71 || Main.tile.At(i, j).Type == 72)
						{
							if (genRand.Next(50) == 0)
							{
								num8 = 194;
							}
							else if (genRand.Next(2) == 0)
							{
								num8 = 183;
							}
						}
						else if (Main.tile.At(i, j).Type >= 63 && Main.tile.At(i, j).Type <= 68)
						{
							num8 = (int)(Main.tile.At(i, j).Type - 63 + 177);
						}
						else if (Main.tile.At(i, j).Type == 50)
						{
							if (Main.tile.At(i, j).FrameX == 90)
							{
								num8 = 165;
							}
							else
							{
								num8 = 149;
							}
						}
						else if (Main.tileAlch[(int)Main.tile.At(i, j).Type] && Main.tile.At(i, j).Type > 82)
						{
							int num11 = (int)(Main.tile.At(i, j).FrameX / 18);
							bool flag = false;
							if (Main.tile.At(i, j).Type == 84)
							{
								flag = true;
							}
							if (num11 == 0 && Main.dayTime)
							{
								flag = true;
							}
							if (num11 == 1 && !Main.dayTime)
							{
								flag = true;
							}
							if (num11 == 3 && Main.bloodMoon)
							{
								flag = true;
							}
							num8 = 313 + num11;
							if (flag)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 307 + num11, genRand.Next(1, 4), false, 0);
							}
						}
						if (num8 > 0)
						{
							Item.NewItem(i * 16, j * 16, 16, 16, num8, 1, false, -1);
						}
					}
					Main.tile.At(i, j).SetActive(false);
					Main.tile.At(i, j).SetFrameX(-1);
					Main.tile.At(i, j).SetFrameY(-1);
					Main.tile.At(i, j).SetFrameNumber(0);
					if (Main.tile.At(i, j).Type == 58 && j > Main.maxTilesY - 200)
					{
						Main.tile.At(i, j).SetLava(true);
						Main.tile.At(i, j).SetLiquid(128);
					}
					Main.tile.At(i, j).SetType(0);
					SquareTileFrame(i, j, true);
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
					Rectangle value = new Rectangle((int)((double)Main.players[i].Position.X + (double)Main.players[i].Width * 0.5 - (double)NPC.sWidth * 0.6),
						(int)((double)Main.players[i].Position.Y + (double)Main.players[i].Height * 0.5 - (double)NPC.sHeight * 0.6), (int)((double)NPC.sWidth * 1.2),
						(int)((double)NPC.sHeight * 1.2));
					if (rectangle.Intersects(value))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void UpdateWorld(ISender Sender)
		{
			UpdateMech(Sender);
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
						LiquidUpdateBuffer.FlushQueue();
					}
				}
				Liquid.skipCount = 0;
			}
			float num = 3E-05f;
			float num2 = 1.5E-05f;
			bool flag = false;
			spawnDelay++;
			if (Main.invasionType > 0)
			{
				spawnDelay = 0;
			}
			if (spawnDelay >= 20)
			{
				flag = true;
				spawnDelay = 0;
				if (spawnNPC != 37)
				{
					for (int i = 0; i < NPC.MAX_NPCS; i++)
					{
						if (Main.npcs[i].Active && Main.npcs[i].homeless && Main.npcs[i].townNPC)
						{
							spawnNPC = Main.npcs[i].Type;
							break;
						}
					}
				}
			}
			int num3 = 0;

			if (genRand == null)
			{
				genRand = new Random();
			}

			TileRef Tile;
			TileRef Tile2;
			while ((float)num3 < (float)(Main.maxTilesX * Main.maxTilesY) * num)
			{
				int TileX = genRand.Next(10, Main.maxTilesX - 10);
				int TileY = genRand.Next(10, (int)Main.worldSurface - 1);
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
						GrowAlch(TileX, TileY);
					}
					if (Tile.Liquid > 32)
					{
						if (Tile.Active && (Tile.Type == 3 || Tile.Type == 20 || Tile.Type == 24 || Tile.Type == 27 || Tile.Type == 73))
						{
							KillTile(TileX, TileY, false, false, false);
							NetMessage.SendData(17, -1, -1, "", 0, (float)TileX, (float)TileY);
						}
					}
					else if (Tile.Active)
					{
						if (Tile.Type == 80)
						{
							if (genRand.Next(15) == 0)
							{
								GrowCactus(TileX, TileY);
							}
						}
						else if (Tile.Type == 53)
						{
							if (!Tile2.Active)
							{
								if (TileX < 250 || TileX > Main.maxTilesX - 250)
								{
									if (genRand.Next(500) == 0 && Tile2.Liquid == 255 && Main.tile.At(TileX, rTileY - 1).Liquid == 255 && Main.tile.At(TileX, rTileY - 2).Liquid == 255 && Main.tile.At(TileX, rTileY - 3).Liquid == 255 && Main.tile.At(TileX, rTileY - 4).Liquid == 255)
									{
										PlaceTile(TileX, rTileY, 81, true, false, -1, 0);
										if (Tile2.Active)
										{
											NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
										}
									}
								}
								else if (TileX > 400 && TileX < Main.maxTilesX - 400 && genRand.Next(300) == 0)
								{
									GrowCactus(TileX, TileY);
								}
							}
						}
						else if (Tile.Type == 78)
						{
							if (!Tile2.Active)
							{
								PlaceTile(TileX, rTileY, 3, true, false, -1, 0);
								if (Tile2.Active)
								{
									NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
								}
							}
						}
						else if (Tile.Type == 2 || Tile.Type == 23 || Tile.Type == 32)
						{
							int num10 = (int)Tile.Type;
							if (!Tile2.Active && genRand.Next(12) == 0 && num10 == 2)
							{
								PlaceTile(TileX, rTileY, 3, true, false, -1, 0);
								if (Tile2.Active)
								{
									NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
								}
							}
							if (!Tile2.Active && genRand.Next(10) == 0 && num10 == 23)
							{
								PlaceTile(TileX, rTileY, 24, true, false, -1, 0);
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
											SpreadGrass(j, k, 0, num10, false);
											if (num10 == 23)
											{
												SpreadGrass(j, k, 2, num10, false);
											}
											if ((int)Main.tile.At(j, k).Type == num10)
											{
												SquareTileFrame(j, k, true);
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
						else if (Tile.Type == 20 && genRand.Next(20) == 0 && !PlayerLOS(TileX, TileY))
						{
							WorldGen.GrowTree(TileX, TileY);
						}
						if (Tile.Type == 3 && genRand.Next(20) == 0 && Tile.FrameX < 144)
						{
							Tile.SetType(73);
							NetMessage.SendTileSquare(-1, TileX, TileY, 3);
						}
						if (Tile.Type == 32 && genRand.Next(3) == 0)
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
								int num14 = genRand.Next(4);
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
											Main.tile.At(num11, num12).SetType(32);
											Main.tile.At(num11, num12).SetActive(true);
											SquareTileFrame(num11, num12, true);

											NetMessage.SendTileSquare(-1, num11, num12, 3);
										}
									}
								}
							}
						}
					}
					else if (flag && spawnNPC > 0)
					{
						SpawnNPC(TileX, TileY);
					}
					if (Tile.Active)
					{
						if ((Tile.Type == 2 || Tile.Type == 52) && genRand.Next(40) == 0 && !Main.tile.At(TileX, TileY + 1).Active && !Main.tile.At(TileX, TileY + 1).Lava)
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
								Main.tile.At(num20, num21).SetType(52);
								Main.tile.At(num20, num21).SetActive(true);
								SquareTileFrame(num20, num21, true);
								NetMessage.SendTileSquare(-1, num20, num21, 3);
							}
						}
						if (Tile.Type == 60)
						{
							int type = (int)Tile.Type;
							if (!Tile2.Active && genRand.Next(7) == 0)
							{
								PlaceTile(TileX, rTileY, 61, true, false, -1, 0);
								if (Tile2.Active)
								{
									NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
								}
							}
							else if (genRand.Next(500) == 0 && (!Tile2.Active || Tile2.Type == 61 || Tile2.Type == 74 || Tile2.Type == 69) && !PlayerLOS(TileX, TileY))
							{
								WorldGen.GrowTree(TileX, TileY);
							}
							bool flag5 = false;
							for (int num22 = num6; num22 < num7; num22++)
							{
								for (int num23 = rTileY; num23 < num9; num23++)
								{
									if ((TileX != num22 || TileY != num23) && Main.tile.At(num22, num23).Active && Main.tile.At(num22, num23).Type == 59)
									{
										SpreadGrass(num22, num23, 59, type, false);
										if ((int)Main.tile.At(num22, num23).Type == type)
										{
											SquareTileFrame(num22, num23, true);
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
						if (Tile.Type == 61 && genRand.Next(3) == 0 && Tile.FrameX < 144)
						{
							Tile.SetType(74);
							NetMessage.SendTileSquare(-1, TileX, TileY, 3);
						}
						if ((Tile.Type == 60 || Tile.Type == 62) && genRand.Next(15) == 0 && !Main.tile.At(TileX, TileY + 1).Active && !Main.tile.At(TileX, TileY + 1).Lava)
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
								Main.tile.At(num25, num26).SetType(62);
								Main.tile.At(num25, num26).SetActive(true);
								SquareTileFrame(num25, num26, true);
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
				int num28 = genRand.Next(10, Main.maxTilesX - 10);
				int num29 = genRand.Next((int)Main.worldSurface + 2, Main.maxTilesY - 20);
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
						GrowAlch(num28, num29);
					}
					if (Main.tile.At(num28, num29).Liquid <= 32)
					{
						if (Main.tile.At(num28, num29).Active)
						{
							if (Main.tile.At(num28, num29).Type == 60)
							{
								int type2 = (int)Main.tile.At(num28, num29).Type;
								if (!Main.tile.At(num28, num32).Active && genRand.Next(10) == 0)
								{
									PlaceTile(num28, num32, 61, true, false, -1, 0);
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
											SpreadGrass(num34, num35, 59, type2, false);
											if ((int)Main.tile.At(num34, num35).Type == type2)
											{
												SquareTileFrame(num34, num35, true);
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
							if (Main.tile.At(num28, num29).Type == 61 && genRand.Next(3) == 0 && Main.tile.At(num28, num29).FrameX < 144)
							{
								Main.tile.At(num28, num29).SetType(74);
								NetMessage.SendTileSquare(-1, num28, num29, 3);
							}
							if ((Main.tile.At(num28, num29).Type == 60 || Main.tile.At(num28, num29).Type == 62) && genRand.Next(5) == 0 && !Main.tile.At(num28, num29 + 1).Active && !Main.tile.At(num28, num29 + 1).Lava)
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
									Main.tile.At(num37, num38).SetType(62);
									Main.tile.At(num37, num38).SetActive(true);
									SquareTileFrame(num37, num38, true);
									NetMessage.SendTileSquare(-1, num37, num38, 3);
								}
							}
							if (Main.tile.At(num28, num29).Type == 69 && genRand.Next(3) == 0)
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
									int num42 = genRand.Next(4);
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
												Main.tile.At(num39, num40).SetType(69);
												Main.tile.At(num39, num40).SetActive(true);
												SquareTileFrame(num39, num40, true);
												NetMessage.SendTileSquare(-1, num39, num40, 3);
											}
										}
									}
								}
							}
							if (Main.tile.At(num28, num29).Type == 70)
							{
								int type3 = (int)Main.tile.At(num28, num29).Type;
								if (!Main.tile.At(num28, num32).Active && genRand.Next(10) == 0)
								{
									PlaceTile(num28, num32, 71, true, false, -1, 0);
									if (Main.tile.At(num28, num32).Active)
									{
										NetMessage.SendTileSquare(-1, num28, num32, 1);
									}
								}
								if (genRand.Next(200) == 0 && !PlayerLOS(num28, num29))
								{
									GrowShroom(num28, num29);
								}
								bool flag10 = false;
								for (int num50 = num30; num50 < num31; num50++)
								{
									for (int num51 = num32; num51 < num33; num51++)
									{
										if ((num28 != num50 || num29 != num51) && Main.tile.At(num50, num51).Active && Main.tile.At(num50, num51).Type == 59)
										{
											SpreadGrass(num50, num51, 59, type3, false);
											if ((int)Main.tile.At(num50, num51).Type == type3)
											{
												SquareTileFrame(num50, num51, true);
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
						else if (flag && spawnNPC > 0)
						{
							SpawnNPC(num28, num29);
						}
					}
				}
				num27++;
			}
			if (Main.rand.Next(100) == 0)
			{
				PlantAlch();
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
					Projectile.NewProjectile(vector.X, vector.Y, num56, num57, ProjectileType.N12_FALLING_STAR, 1000, 10f, Main.myPlayer);
				}
			}
		}

		public static void PlaceWall(int i, int j, int type, bool mute = false)
		{
			if (i <= 1 || j <= 1 || i >= Main.maxTilesX - 2 || j >= Main.maxTilesY - 2)
			{
				return;
			}
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
				Main.tile.At(i, j).SetWall((byte)type);
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
				Main.tile.At(i, j).SetType((byte)grass);
				for (int m = num; m < num2; m++)
				{
					for (int n = num3; n < num4; n++)
					{
						if (Main.tile.At(m, n).Active && (int)Main.tile.At(m, n).Type == dirt && repeat)
						{
							SpreadGrass(m, n, dirt, grass);
						}
					}
				}
			}
		}

		public static void SquareTileFrame(int i, int j, bool resetFrame = true)
		{
			TileFrame(i - 1, j - 1, false, false);
			TileFrame(i - 1, j, false, false);
			TileFrame(i - 1, j + 1, false, false);
			TileFrame(i, j - 1, false, false);
			TileFrame(i, j, resetFrame, false);
			TileFrame(i, j + 1, false, false);
			TileFrame(i + 1, j - 1, false, false);
			TileFrame(i + 1, j, false, false);
			TileFrame(i + 1, j + 1, false, false);
		}

		public static void SquareWallFrame(int i, int j, bool resetFrame = true)
		{
			WallFrame(i - 1, j - 1, false);
			WallFrame(i - 1, j, false);
			WallFrame(i - 1, j + 1, false);
			WallFrame(i, j - 1, false);
			WallFrame(i, j, resetFrame);
			WallFrame(i, j + 1, false);
			WallFrame(i + 1, j - 1, false);
			WallFrame(i + 1, j, false);
			WallFrame(i + 1, j + 1, false);
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
					TileFrame(i, j, true, true);
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
					TileFrame(i, j);
					WallFrame(i, j);
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
					Main.tile.At(i, j).SetCheckingLiquid(false);
					if (Main.tile.At(i, j).Liquid > 0 && Main.tile.At(i, j).Active && Main.tileSolid[(int)Main.tile.At(i, j).Type] && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
					{
						Main.tile.At(i, j).SetLiquid(0);
					}
					else if (Main.tile.At(i, j).Liquid > 0)
					{
						if (Main.tile.At(i, j).Active)
						{
							if (Main.tileWaterDeath[(int)Main.tile.At(i, j).Type])
							{
								KillTile(i, j);
							}
							if (Main.tile.At(i, j).Lava && Main.tileLavaDeath[(int)Main.tile.At(i, j).Type])
							{
								KillTile(i, j);
							}
						}
						if ((!Main.tile.At(i, j + 1).Active || !Main.tileSolid[(int)Main.tile.At(i, j + 1).Type] || Main.tileSolidTop[(int)Main.tile.At(i, j + 1).Type]) && Main.tile.At(i, j + 1).Liquid < 255)
						{
							if (Main.tile.At(i, j + 1).Liquid > 250)
							{
								Main.tile.At(i, j + 1).SetLiquid(255);
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
			using (var prog = new ProgressLogger(Main.maxTilesX, "Finding tile frames"))
			{
				noLiquidCheck = true;
				noTileActions = true;
				for (int i = 0; i < Main.maxTilesX; i++)
				{
					prog.Value = i;

					for (int j = 0; j < Main.maxTilesY; j++)
					{
						if (Main.tile.At(i, j).Active)
						{
							TileFrame(i, j, true, false);
						}
						if (Main.tile.At(i, j).Wall > 0)
						{
							WallFrame(i, j, true);
						}
					}
				}
				noLiquidCheck = false;
				noTileActions = false;
			}
		}

		public static void PlantCheck(int i, int j)
		{
			int num = -1;
			int type = (int)Main.tile.At(i, j).Type;
			//int arg_19_0 = i - 1;
			//int arg_23_0 = i + 1;
			//int arg_22_0 = Main.maxTilesX;
			//int arg_29_0 = j - 1;
			if (j + 1 >= Main.maxTilesY)
			{
				num = type;
			}
			//if (i - 1 >= 0 && Main.tile.At(i - 1, j).Active)
			//{
			//    byte arg_74_0 = Main.tile.At(i - 1, j).Type;
			//}
			//if (i + 1 < Main.maxTilesX && Main.tile.At(i + 1, j).Active)
			//{
			//    byte arg_B7_0 = Main.tile.At(i + 1, j).Type;
			//}
			//if (j - 1 >= 0 && Main.tile.At(i, j - 1).Active)
			//{
			//    byte arg_F6_0 = Main.tile.At(i, j - 1).Type;
			//}
			if (j + 1 < Main.maxTilesY && Main.tile.At(i, j + 1).Active)
			{
				num = (int)Main.tile.At(i, j + 1).Type;
			}
			//if (i - 1 >= 0 && j - 1 >= 0 && Main.tile.At(i - 1, j - 1).Active)
			//{
			//    byte arg_184_0 = Main.tile.At(i - 1, j - 1).Type;
			//}
			//if (i + 1 < Main.maxTilesX && j - 1 >= 0 && Main.tile.At(i + 1, j - 1).Active)
			//{
			//    byte arg_1D3_0 = Main.tile.At(i + 1, j - 1).Type;
			//}
			//if (i - 1 >= 0 && j + 1 < Main.maxTilesY && Main.tile.At(i - 1, j + 1).Active)
			//{
			//    byte arg_222_0 = Main.tile.At(i - 1, j + 1).Type;
			//}
			//if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY && Main.tile.At(i + 1, j + 1).Active)
			//{
			//    byte arg_275_0 = Main.tile.At(i + 1, j + 1).Type;
			//}
			if ((type == 3 && num != 2 && num != 78) || (type == 24 && num != 23) || (type == 61 && num != 60) || (type == 71 && num != 70) || (type == 73 && num != 2 && num != 78) || (type == 74 && num != 60) || (type == 110 && type != 109) || (type == 113 && type != 109))
			{
				if (num == 23)
				{
					type = 24;
					if (Main.tile.At(i, j).FrameX >= 162)
					{
						Main.tile.At(i, j).SetFrameX(126);
					}
				}
				else if (num == 2)
				{
					if (type == 113)
						type = 73;
					else
						type = 3;
				}
				else if (num == 109)
				{
					if (type == 73)
						type = 113;
					else
						type = 110;
				}
				if (type != (int)Main.tile.At(i, j).Type)
				{
					Main.tile.At(i, j).SetType((byte)type);
					return;
				}
				KillTile(i, j, false, false, false);
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
					return;

				//byte arg_89_0 = Main.tile.At(i, j).FrameX;
				//byte arg_9B_0 = Main.tile.At(i, j).FrameY;
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
					num4 = (int)Main.tile.At(i - 1, j).Wall;

				if (i + 1 < Main.maxTilesX)
					num5 = (int)Main.tile.At(i + 1, j).Wall;

				if (j - 1 >= 0)
					num2 = (int)Main.tile.At(i, j - 1).Wall;

				if (j + 1 < Main.maxTilesY)
					num7 = (int)Main.tile.At(i, j + 1).Wall;

				if (i - 1 >= 0 && j - 1 >= 0)
					num = (int)Main.tile.At(i - 1, j - 1).Wall;

				if (i + 1 < Main.maxTilesX && j - 1 >= 0)
					num3 = (int)Main.tile.At(i + 1, j - 1).Wall;

				if (i - 1 >= 0 && j + 1 < Main.maxTilesY)
					num6 = (int)Main.tile.At(i - 1, j + 1).Wall;

				if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY)
					num8 = (int)Main.tile.At(i + 1, j + 1).Wall;

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

				if (num7 > 0)
					num7 = wall;

				if (num6 > 0)
					num6 = wall;

				if (num8 > 0)
					num8 = wall;

				if (num2 > 0)
					num2 = wall;

				if (num > 0)
					num = wall;

				if (num3 > 0)
					num3 = wall;

				if (num4 > 0)
					num4 = wall;

				if (num5 > 0)
					num5 = wall;

				int num9 = 0;
				if (resetFrame)
				{
					num9 = genRand.Next(0, 3);
					Main.tile.At(i, j).SetFrameNumber((byte)num9);
				}
				else
					num9 = (int)Main.tile.At(i, j).FrameNumber;

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
						else if (num9 == 0)
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
					} if (num2 == wall && num7 == wall && (num4 != wall & num5 == wall))
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
				Main.tile.At(i, j).SetFrameX((byte)rectangle.X);
				Main.tile.At(i, j).SetFrameY((byte)rectangle.Y);
			}
		}

		public static void TileFrame(int i, int j, bool resetFrame = false, bool noBreak = false)
		{
			try
			{
				if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5)
				{
					if (Main.tile.At(i, j).Liquid > 0 && !noLiquidCheck)
					{
						Liquid.AddWater(i, j);
					}
					if (Main.tile.At(i, j).Active)
					{
						if (!noBreak || !Main.tileFrameImportant[(int)Main.tile.At(i, j).Type] || Main.tile.At(i, j).Type == 4)
						{
							int num = (int)Main.tile.At(i, j).Type;
							if (Main.tileStone[num])
							{
								num = 1;
							}
							int frameX = (int)Main.tile.At(i, j).FrameX;
							int frameY = (int)Main.tile.At(i, j).FrameY;
							Rectangle rectangle = new Rectangle(-1, -1, 0, 0);
							if (Main.tileFrameImportant[(int)Main.tile.At(i, j).Type])
							{
								if (num == 4)
								{
									short num2 = 0;
									if (Main.tile.At(i, j).FrameX >= 66)
									{
										num2 = 66;
									}
									int num3 = -1;
									int num4 = -1;
									int num5 = -1;
									int num6 = -1;
									int num7 = -1;
									int num8 = -1;
									int num9 = -1;
									if (Main.tile.At(i, j - 1).Active)
									{
										byte arg_186_0 = Main.tile.At(i, j - 1).Type;
									}
									if (Main.tile.At(i, j + 1).Active)
									{
										num3 = (int)Main.tile.At(i, j + 1).Type;
									}
									if (Main.tile.At(i - 1, j).Active)
									{
										num4 = (int)Main.tile.At(i - 1, j).Type;
									}
									if (Main.tile.At(i + 1, j).Active)
									{
										num5 = (int)Main.tile.At(i + 1, j).Type;
									}
									if (Main.tile.At(i - 1, j + 1).Active)
									{
										num6 = (int)Main.tile.At(i - 1, j + 1).Type;
									}
									if (Main.tile.At(i + 1, j + 1).Active)
									{
										num7 = (int)Main.tile.At(i + 1, j + 1).Type;
									}
									if (Main.tile.At(i - 1, j - 1).Active)
									{
										num8 = (int)Main.tile.At(i - 1, j - 1).Type;
									}
									if (Main.tile.At(i + 1, j - 1).Active)
									{
										num9 = (int)Main.tile.At(i + 1, j - 1).Type;
									}
									if (num3 >= 0 && Main.tileSolid[num3] && !Main.tileNoAttach[num3])
									{
										Main.tile.At(i, j).SetFrameX(num2);
									}
									else
									{
										if ((num4 >= 0 && Main.tileSolid[num4] && !Main.tileNoAttach[num4]) || num4 == 124 || (num4 == 5 && num8 == 5 && num6 == 5))
										{
											Main.tile.At(i, j).SetFrameX((short)(22 + num2));
										}
										else
										{
											if ((num5 >= 0 && Main.tileSolid[num5] && !Main.tileNoAttach[num5]) || num5 == 124 || (num5 == 5 && num9 == 5 && num7 == 5))
											{
												Main.tile.At(i, j).SetFrameX((short)(44 + num2));
											}
											else
											{
												KillTile(i, j);
											}
										}
									}
								}
								else
								{
									if (num == 136)
									{
										int num10 = -1;
										int num11 = -1;
										int num12 = -1;
										if (Main.tile.At(i, j + 1).Active)
										{
											num10 = (int)Main.tile.At(i, j + 1).Type;
										}
										if (Main.tile.At(i - 1, j).Active)
										{
											num11 = (int)Main.tile.At(i - 1, j).Type;
										}
										if (Main.tile.At(i + 1, j).Active)
										{
											num12 = (int)Main.tile.At(i + 1, j).Type;
										}
										if (num10 >= 0 && Main.tileSolid[num10] && !Main.tileNoAttach[num10])
										{
											Main.tile.At(i, j).SetFrameX(0);
										}
										else
										{
											if ((num11 >= 0 && Main.tileSolid[num11] && !Main.tileNoAttach[num11]) || num11 == 124 || num11 == 5)
											{
												Main.tile.At(i, j).SetFrameX(18);
											}
											else
											{
												if ((num12 >= 0 && Main.tileSolid[num12] && !Main.tileNoAttach[num12]) || num12 == 124 || num12 == 5)
												{
													Main.tile.At(i, j).SetFrameX(36);
												}
												else
												{
													KillTile(i, j);
												}
											}
										}
									}
									else
									{
										if (num == 129)
										{
											int num13 = -1;
											int num14 = -1;
											int num15 = -1;
											int num16 = -1;
											if (Main.tile.At(i, j - 1).Active)
											{
												num14 = (int)Main.tile.At(i, j - 1).Type;
											}
											if (Main.tile.At(i, j + 1).Active)
											{
												num13 = (int)Main.tile.At(i, j + 1).Type;
											}
											if (Main.tile.At(i - 1, j).Active)
											{
												num15 = (int)Main.tile.At(i - 1, j).Type;
											}
											if (Main.tile.At(i + 1, j).Active)
											{
												num16 = (int)Main.tile.At(i + 1, j).Type;
											}
											if (num13 >= 0 && Main.tileSolid[num13] && !Main.tileSolidTop[num13])
											{
												Main.tile.At(i, j).SetFrameY(0);
											}
											else
											{
												if (num15 >= 0 && Main.tileSolid[num15] && !Main.tileSolidTop[num15])
												{
													Main.tile.At(i, j).SetFrameY(54);
												}
												else
												{
													if (num16 >= 0 && Main.tileSolid[num16] && !Main.tileSolidTop[num16])
													{
														Main.tile.At(i, j).SetFrameY(36);
													}
													else
													{
														if (num14 >= 0 && Main.tileSolid[num14] && !Main.tileSolidTop[num14])
														{
															Main.tile.At(i, j).SetFrameY(18);
														}
														else
														{
															KillTile(i, j);
														}
													}
												}
											}
										}
										else
										{
											if (num == 3 || num == 24 || num == 61 || num == 71 || num == 73 || num == 74 || num == 110 || num == 113)
											{
												PlantCheck(i, j);
											}
											else
											{
												if (num == 12 || num == 31)
												{
													CheckOrb(i, j, num);
												}
												else
												{
													if (num == 10)
													{
														if (!destroyObject)
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
															if (!Main.tile.At(i, num17).Active || (int)Main.tile.At(i, num17).Type != num)
															{
																flag = true;
															}
															if (!Main.tile.At(i, num17 + 1).Active || (int)Main.tile.At(i, num17 + 1).Type != num)
															{
																flag = true;
															}
															if (!Main.tile.At(i, num17 + 2).Active || (int)Main.tile.At(i, num17 + 2).Type != num)
															{
																flag = true;
															}
															if (flag)
															{
																destroyObject = true;
																KillTile(i, num17, false, false, false);
																KillTile(i, num17 + 1, false, false, false);
																KillTile(i, num17 + 2, false, false, false);
																Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false, 0);
															}
															destroyObject = false;
														}
													}
													else
													{
														if (num == 11)
														{
															if (!destroyObject)
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
																	destroyObject = true;
																	Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false, 0);
																}
																int num21 = num19;
																if (num18 == -1)
																{
																	num21 = num19 - 1;
																}
																for (int k = num21; k < num21 + 2; k++)
																{
																	for (int l = num20; l < num20 + 3; l++)
																	{
																		if (!flag2 && (Main.tile.At(k, l).Type != 11 || !Main.tile.At(k, l).Active))
																		{
																			destroyObject = true;
																			Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false, 0);
																			flag2 = true;
																			k = num21;
																			l = num20;
																		}
																		if (flag2)
																		{
																			KillTile(k, l, false, false, false);
																		}
																	}
																}
																destroyObject = false;
															}
														}
														else
														{
															if (num == 34 || num == 35 || num == 36 || num == 106)
															{
																Check3x3(i, j, (int)((byte)num));
															}
															else
															{
																if (num == 15 || num == 20)
																{
																	Check1x2(i, j, (byte)num);
																}
																else
																{
																	if (num == 14 || num == 17 || num == 26 || num == 77 || num == 86 || num == 87 || num == 88 || num == 89 || num == 114 || num == 133)
																	{
																		Check3x2(i, j, (int)((byte)num));
																	}
																	else
																	{
																		if (num == 135 || num == 144 || num == 141)
																		{
																			Check1x1(i, j, num);
																		}
																		else
																		{
																			if (num == 16 || num == 18 || num == 29 || num == 103 || num == 134)
																			{
																				Check2x1(i, j, (byte)num);
																			}
																			else
																			{
																				if (num == 13 || num == 33 || num == 50 || num == 78)
																				{
																					CheckOnTable1x1(i, j, (int)((byte)num));
																				}
																				else
																				{
																					if (num == 21)
																					{
																						CheckChest(i, j, (int)((byte)num));
																					}
																					else
																					{
																						if (num == 128)
																						{
																							CheckMan(i, j);
																						}
																						else
																						{
																							if (num == 27)
																							{
																								CheckSunflower(i, j, 27);
																							}
																							else
																							{
																								if (num == 28)
																								{
																									CheckPot(i, j, 28);
																								}
																								else
																								{
																									if (num == 132 || num == 138 || num == 142 || num == 143)
																									{
																										Check2x2(i, j, num);
																									}
																									else
																									{
																										if (num == 91)
																										{
																											CheckBanner(i, j, (byte)num);
																										}
																										else
																										{
																											if (num == 139)
																											{
																												CheckMB(i, j, (int)((byte)num));
																											}
																											else
																											{
																												if (num == 92 || num == 93)
																												{
																													Check1xX(i, j, (byte)num);
																												}
																												else
																												{
																													if (num == 104 || num == 105)
																													{
																														Check2xX(i, j, (byte)num);
																													}
																													else
																													{
																														if (num == 101 || num == 102)
																														{
																															Check3x4(i, j, (int)((byte)num));
																														}
																														else
																														{
																															if (num == 42)
																															{
																																Check1x2Top(i, j, (byte)num);
																															}
																															else
																															{
																																if (num == 55 || num == 85)
																																{
																																	CheckSign(i, j, num);
																																}
																																else
																																{
																																	if (num == 79 || num == 90)
																																	{
																																		Check4x2(i, j, num);
																																	}
																																	else
																																	{
																																		if (num == 85 || num == 94 || num == 95 || num == 96 || num == 97 || num == 98 || num == 99 || num == 100 || num == 125 || num == 126)
																																		{
																																			Check2x2(i, j, num);
																																		}
																																		else
																																		{
																																			if (num == 81)
																																			{
																																				int num22 = -1;
																																				int num23 = -1;
																																				int num24 = -1;
																																				int num25 = -1;
																																				if (Main.tile.At(i, j - 1).Active)
																																				{
																																					num23 = (int)Main.tile.At(i, j - 1).Type;
																																				}
																																				if (Main.tile.At(i, j + 1).Active)
																																				{
																																					num22 = (int)Main.tile.At(i, j + 1).Type;
																																				}
																																				if (Main.tile.At(i - 1, j).Active)
																																				{
																																					num24 = (int)Main.tile.At(i - 1, j).Type;
																																				}
																																				if (Main.tile.At(i + 1, j).Active)
																																				{
																																					num25 = (int)Main.tile.At(i + 1, j).Type;
																																				}
																																				if (num24 != -1 || num23 != -1 || num25 != -1)
																																				{
																																					KillTile(i, j);
																																				}
																																				else
																																				{
																																					if (num22 < 0 || !Main.tileSolid[num22])
																																					{
																																						KillTile(i, j);
																																					}
																																				}
																																			}
																																			else
																																			{
																																				if (Main.tileAlch[num])
																																				{
																																					CheckAlch(i, j);
																																				}
																																				else
																																				{
																																					if (num == 72)
																																					{
																																						int num26 = -1;
																																						int num27 = -1;
																																						if (Main.tile.At(i, j - 1).Active)
																																						{
																																							num27 = (int)Main.tile.At(i, j - 1).Type;
																																						}
																																						if (Main.tile.At(i, j + 1).Active)
																																						{
																																							num26 = (int)Main.tile.At(i, j + 1).Type;
																																						}
																																						if (num26 != num && num26 != 70)
																																						{
																																							KillTile(i, j);
																																						}
																																						else
																																						{
																																							if (num27 != num && Main.tile.At(i, j).FrameX == 0)
																																							{
																																								Main.tile.At(i, j).SetFrameNumber((byte)genRand.Next(3));
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
																																						if (num == 5)
																																						{
																																							CheckTree(i, j);
																																						}
																																					}
																																				}
																																			}
																																		}
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
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
								int num28 = -1;
								int num29 = -1;
								int num30 = -1;
								int num31 = -1;
								int num32 = -1;
								int num33 = -1;
								int num34 = -1;
								int num35 = -1;
								if (Main.tile.At(i - 1, j).Active)
								{
									if (Main.tileStone[(int)Main.tile.At(i - 1, j).Type])
									{
										num31 = 1;
									}
									else
									{
										num31 = (int)Main.tile.At(i - 1, j).Type;
									}
								}
								if (Main.tile.At(i + 1, j).Active)
								{
									if (Main.tileStone[(int)Main.tile.At(i + 1, j).Type])
									{
										num32 = 1;
									}
									else
									{
										num32 = (int)Main.tile.At(i + 1, j).Type;
									}
								}
								if (Main.tile.At(i, j - 1).Active)
								{
									if (Main.tileStone[(int)Main.tile.At(i, j - 1).Type])
									{
										num29 = 1;
									}
									else
									{
										num29 = (int)Main.tile.At(i, j - 1).Type;
									}
								}
								if (Main.tile.At(i, j + 1).Active)
								{
									if (Main.tileStone[(int)Main.tile.At(i, j + 1).Type])
									{
										num34 = 1;
									}
									else
									{
										num34 = (int)Main.tile.At(i, j + 1).Type;
									}
								}
								if (Main.tile.At(i - 1, j - 1).Active)
								{
									if (Main.tileStone[(int)Main.tile.At(i - 1, j - 1).Type])
									{
										num28 = 1;
									}
									else
									{
										num28 = (int)Main.tile.At(i - 1, j - 1).Type;
									}
								}
								if (Main.tile.At(i + 1, j - 1).Active)
								{
									if (Main.tileStone[(int)Main.tile.At(i + 1, j - 1).Type])
									{
										num30 = 1;
									}
									else
									{
										num30 = (int)Main.tile.At(i + 1, j - 1).Type;
									}
								}
								if (Main.tile.At(i - 1, j + 1).Active)
								{
									if (Main.tileStone[(int)Main.tile.At(i - 1, j + 1).Type])
									{
										num33 = 1;
									}
									else
									{
										num33 = (int)Main.tile.At(i - 1, j + 1).Type;
									}
								}
								if (Main.tile.At(i + 1, j + 1).Active)
								{
									if (Main.tileStone[(int)Main.tile.At(i + 1, j + 1).Type])
									{
										num35 = 1;
									}
									else
									{
										num35 = (int)Main.tile.At(i + 1, j + 1).Type;
									}
								}
								if (!Main.tileSolid[num])
								{
									if (num == 49)
									{
										CheckOnTable1x1(i, j, (int)((byte)num));
										return;
									}
									if (num == 80)
									{
										CactusFrame(i, j);
										return;
									}
								}
								else
								{
									if (num == 19)
									{
										if (num32 >= 0 && !Main.tileSolid[num32])
										{
											num32 = -1;
										}
										if (num31 >= 0 && !Main.tileSolid[num31])
										{
											num31 = -1;
										}
										if (num31 == num && num32 == num)
										{
											if (Main.tile.At(i, j).FrameNumber == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 0;
											}
											else
											{
												if (Main.tile.At(i, j).FrameNumber == 1)
												{
													rectangle.X = 0;
													rectangle.Y = 18;
												}
												else
												{
													rectangle.X = 0;
													rectangle.Y = 36;
												}
											}
										}
										else
										{
											if (num31 == num && num32 == -1)
											{
												if (Main.tile.At(i, j).FrameNumber == 0)
												{
													rectangle.X = 18;
													rectangle.Y = 0;
												}
												else
												{
													if (Main.tile.At(i, j).FrameNumber == 1)
													{
														rectangle.X = 18;
														rectangle.Y = 18;
													}
													else
													{
														rectangle.X = 18;
														rectangle.Y = 36;
													}
												}
											}
											else
											{
												if (num31 == -1 && num32 == num)
												{
													if (Main.tile.At(i, j).FrameNumber == 0)
													{
														rectangle.X = 36;
														rectangle.Y = 0;
													}
													else
													{
														if (Main.tile.At(i, j).FrameNumber == 1)
														{
															rectangle.X = 36;
															rectangle.Y = 18;
														}
														else
														{
															rectangle.X = 36;
															rectangle.Y = 36;
														}
													}
												}
												else
												{
													if (num31 != num && num32 == num)
													{
														if (Main.tile.At(i, j).FrameNumber == 0)
														{
															rectangle.X = 54;
															rectangle.Y = 0;
														}
														else
														{
															if (Main.tile.At(i, j).FrameNumber == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 18;
															}
															else
															{
																rectangle.X = 54;
																rectangle.Y = 36;
															}
														}
													}
													else
													{
														if (num31 == num && num32 != num)
														{
															if (Main.tile.At(i, j).FrameNumber == 0)
															{
																rectangle.X = 72;
																rectangle.Y = 0;
															}
															else
															{
																if (Main.tile.At(i, j).FrameNumber == 1)
																{
																	rectangle.X = 72;
																	rectangle.Y = 18;
																}
																else
																{
																	rectangle.X = 72;
																	rectangle.Y = 36;
																}
															}
														}
														else
														{
															if (num31 != num && num31 != -1 && num32 == -1)
															{
																if (Main.tile.At(i, j).FrameNumber == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 0;
																}
																else
																{
																	if (Main.tile.At(i, j).FrameNumber == 1)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 18;
																	}
																	else
																	{
																		rectangle.X = 108;
																		rectangle.Y = 36;
																	}
																}
															}
															else
															{
																if (num31 == -1 && num32 != num && num32 != -1)
																{
																	if (Main.tile.At(i, j).FrameNumber == 0)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 0;
																	}
																	else
																	{
																		if (Main.tile.At(i, j).FrameNumber == 1)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 18;
																		}
																		else
																		{
																			rectangle.X = 126;
																			rectangle.Y = 36;
																		}
																	}
																}
																else
																{
																	if (Main.tile.At(i, j).FrameNumber == 0)
																	{
																		rectangle.X = 90;
																		rectangle.Y = 0;
																	}
																	else
																	{
																		if (Main.tile.At(i, j).FrameNumber == 1)
																		{
																			rectangle.X = 90;
																			rectangle.Y = 18;
																		}
																		else
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
									}
								}
								mergeUp = false;
								mergeDown = false;
								mergeLeft = false;
								mergeRight = false;
								int num36 = 0;
								if (resetFrame)
								{
									num36 = genRand.Next(0, 3);
									Main.tile.At(i, j).SetFrameNumber((byte)num36);
								}
								else
								{
									num36 = (int)Main.tile.At(i, j).FrameNumber;
								}
								if (num == 0)
								{
									if (num29 >= 0 && Main.tileMergeDirt[num29])
									{
										TileFrame(i, j - 1, false, false);
										if (mergeDown)
										{
											num29 = num;
										}
									}
									if (num34 >= 0 && Main.tileMergeDirt[num34])
									{
										TileFrame(i, j + 1, false, false);
										if (mergeUp)
										{
											num34 = num;
										}
									}
									if (num31 >= 0 && Main.tileMergeDirt[num31])
									{
										TileFrame(i - 1, j, false, false);
										if (mergeRight)
										{
											num31 = num;
										}
									}
									if (num32 >= 0 && Main.tileMergeDirt[num32])
									{
										TileFrame(i + 1, j, false, false);
										if (mergeLeft)
										{
											num32 = num;
										}
									}
									if (num29 == 2 || num29 == 23 || num29 == 109)
									{
										num29 = num;
									}
									if (num34 == 2 || num34 == 23 || num34 == 109)
									{
										num34 = num;
									}
									if (num31 == 2 || num31 == 23 || num31 == 109)
									{
										num31 = num;
									}
									if (num32 == 2 || num32 == 23 || num32 == 109)
									{
										num32 = num;
									}
									if (num28 >= 0 && Main.tileMergeDirt[num28])
									{
										num28 = num;
									}
									else
									{
										if (num28 == 2 || num28 == 23 || num28 == 109)
										{
											num28 = num;
										}
									}
									if (num30 >= 0 && Main.tileMergeDirt[num30])
									{
										num30 = num;
									}
									else
									{
										if (num30 == 2 || num30 == 23 || num30 == 109)
										{
											num30 = num;
										}
									}
									if (num33 >= 0 && Main.tileMergeDirt[num33])
									{
										num33 = num;
									}
									else
									{
										if (num33 == 2 || num33 == 23 || num30 == 109)
										{
											num33 = num;
										}
									}
									if (num35 >= 0 && Main.tileMergeDirt[num35])
									{
										num35 = num;
									}
									else
									{
										if (num35 == 2 || num35 == 23 || num35 == 109)
										{
											num35 = num;
										}
									}
									if ((double)j < Main.rockLayer)
									{
										if (num29 == 59)
										{
											num29 = -2;
										}
										if (num34 == 59)
										{
											num34 = -2;
										}
										if (num31 == 59)
										{
											num31 = -2;
										}
										if (num32 == 59)
										{
											num32 = -2;
										}
										if (num28 == 59)
										{
											num28 = -2;
										}
										if (num30 == 59)
										{
											num30 = -2;
										}
										if (num33 == 59)
										{
											num33 = -2;
										}
										if (num35 == 59)
										{
											num35 = -2;
										}
									}
								}
								else
								{
									if (Main.tileMergeDirt[num])
									{
										if (num29 == 0)
										{
											num29 = -2;
										}
										if (num34 == 0)
										{
											num34 = -2;
										}
										if (num31 == 0)
										{
											num31 = -2;
										}
										if (num32 == 0)
										{
											num32 = -2;
										}
										if (num28 == 0)
										{
											num28 = -2;
										}
										if (num30 == 0)
										{
											num30 = -2;
										}
										if (num33 == 0)
										{
											num33 = -2;
										}
										if (num35 == 0)
										{
											num35 = -2;
										}
										if (num == 1)
										{
											if ((double)j > Main.rockLayer)
											{
												if (num29 == 59)
												{
													TileFrame(i, j - 1, false, false);
													if (mergeDown)
													{
														num29 = num;
													}
												}
												if (num34 == 59)
												{
													TileFrame(i, j + 1, false, false);
													if (mergeUp)
													{
														num34 = num;
													}
												}
												if (num31 == 59)
												{
													TileFrame(i - 1, j, false, false);
													if (mergeRight)
													{
														num31 = num;
													}
												}
												if (num32 == 59)
												{
													TileFrame(i + 1, j, false, false);
													if (mergeLeft)
													{
														num32 = num;
													}
												}
												if (num28 == 59)
												{
													num28 = num;
												}
												if (num30 == 59)
												{
													num30 = num;
												}
												if (num33 == 59)
												{
													num33 = num;
												}
												if (num35 == 59)
												{
													num35 = num;
												}
											}
											if (num29 == 57)
											{
												TileFrame(i, j - 1, false, false);
												if (mergeDown)
												{
													num29 = num;
												}
											}
											if (num34 == 57)
											{
												TileFrame(i, j + 1, false, false);
												if (mergeUp)
												{
													num34 = num;
												}
											}
											if (num31 == 57)
											{
												TileFrame(i - 1, j, false, false);
												if (mergeRight)
												{
													num31 = num;
												}
											}
											if (num32 == 57)
											{
												TileFrame(i + 1, j, false, false);
												if (mergeLeft)
												{
													num32 = num;
												}
											}
											if (num28 == 57)
											{
												num28 = num;
											}
											if (num30 == 57)
											{
												num30 = num;
											}
											if (num33 == 57)
											{
												num33 = num;
											}
											if (num35 == 57)
											{
												num35 = num;
											}
										}
									}
									else
									{
										if (num == 58 || num == 76 || num == 75)
										{
											if (num29 == 57)
											{
												num29 = -2;
											}
											if (num34 == 57)
											{
												num34 = -2;
											}
											if (num31 == 57)
											{
												num31 = -2;
											}
											if (num32 == 57)
											{
												num32 = -2;
											}
											if (num28 == 57)
											{
												num28 = -2;
											}
											if (num30 == 57)
											{
												num30 = -2;
											}
											if (num33 == 57)
											{
												num33 = -2;
											}
											if (num35 == 57)
											{
												num35 = -2;
											}
										}
										else
										{
											if (num == 59)
											{
												if ((double)j > Main.rockLayer)
												{
													if (num29 == 1)
													{
														num29 = -2;
													}
													if (num34 == 1)
													{
														num34 = -2;
													}
													if (num31 == 1)
													{
														num31 = -2;
													}
													if (num32 == 1)
													{
														num32 = -2;
													}
													if (num28 == 1)
													{
														num28 = -2;
													}
													if (num30 == 1)
													{
														num30 = -2;
													}
													if (num33 == 1)
													{
														num33 = -2;
													}
													if (num35 == 1)
													{
														num35 = -2;
													}
												}
												if (num29 == 60)
												{
													num29 = num;
												}
												if (num34 == 60)
												{
													num34 = num;
												}
												if (num31 == 60)
												{
													num31 = num;
												}
												if (num32 == 60)
												{
													num32 = num;
												}
												if (num28 == 60)
												{
													num28 = num;
												}
												if (num30 == 60)
												{
													num30 = num;
												}
												if (num33 == 60)
												{
													num33 = num;
												}
												if (num35 == 60)
												{
													num35 = num;
												}
												if (num29 == 70)
												{
													num29 = num;
												}
												if (num34 == 70)
												{
													num34 = num;
												}
												if (num31 == 70)
												{
													num31 = num;
												}
												if (num32 == 70)
												{
													num32 = num;
												}
												if (num28 == 70)
												{
													num28 = num;
												}
												if (num30 == 70)
												{
													num30 = num;
												}
												if (num33 == 70)
												{
													num33 = num;
												}
												if (num35 == 70)
												{
													num35 = num;
												}
												if ((double)j < Main.rockLayer)
												{
													if (num29 == 0)
													{
														TileFrame(i, j - 1, false, false);
														if (mergeDown)
														{
															num29 = num;
														}
													}
													if (num34 == 0)
													{
														TileFrame(i, j + 1, false, false);
														if (mergeUp)
														{
															num34 = num;
														}
													}
													if (num31 == 0)
													{
														TileFrame(i - 1, j, false, false);
														if (mergeRight)
														{
															num31 = num;
														}
													}
													if (num32 == 0)
													{
														TileFrame(i + 1, j, false, false);
														if (mergeLeft)
														{
															num32 = num;
														}
													}
													if (num28 == 0)
													{
														num28 = num;
													}
													if (num30 == 0)
													{
														num30 = num;
													}
													if (num33 == 0)
													{
														num33 = num;
													}
													if (num35 == 0)
													{
														num35 = num;
													}
												}
											}
											else
											{
												if (num == 57)
												{
													if (num29 == 1)
													{
														num29 = -2;
													}
													if (num34 == 1)
													{
														num34 = -2;
													}
													if (num31 == 1)
													{
														num31 = -2;
													}
													if (num32 == 1)
													{
														num32 = -2;
													}
													if (num28 == 1)
													{
														num28 = -2;
													}
													if (num30 == 1)
													{
														num30 = -2;
													}
													if (num33 == 1)
													{
														num33 = -2;
													}
													if (num35 == 1)
													{
														num35 = -2;
													}
													if (num29 == 58 || num29 == 76 || num29 == 75)
													{
														TileFrame(i, j - 1, false, false);
														if (mergeDown)
														{
															num29 = num;
														}
													}
													if (num34 == 58 || num34 == 76 || num34 == 75)
													{
														TileFrame(i, j + 1, false, false);
														if (mergeUp)
														{
															num34 = num;
														}
													}
													if (num31 == 58 || num31 == 76 || num31 == 75)
													{
														TileFrame(i - 1, j, false, false);
														if (mergeRight)
														{
															num31 = num;
														}
													}
													if (num32 == 58 || num32 == 76 || num32 == 75)
													{
														TileFrame(i + 1, j, false, false);
														if (mergeLeft)
														{
															num32 = num;
														}
													}
													if (num28 == 58 || num28 == 76 || num28 == 75)
													{
														num28 = num;
													}
													if (num30 == 58 || num30 == 76 || num30 == 75)
													{
														num30 = num;
													}
													if (num33 == 58 || num33 == 76 || num33 == 75)
													{
														num33 = num;
													}
													if (num35 == 58 || num35 == 76 || num35 == 75)
													{
														num35 = num;
													}
												}
												else
												{
													if (num == 32)
													{
														if (num34 == 23)
														{
															num34 = num;
														}
													}
													else
													{
														if (num == 69)
														{
															if (num34 == 60)
															{
																num34 = num;
															}
														}
														else
														{
															if (num == 51)
															{
																if (num29 > -1 && !Main.tileNoAttach[num29])
																{
																	num29 = num;
																}
																if (num34 > -1 && !Main.tileNoAttach[num34])
																{
																	num34 = num;
																}
																if (num31 > -1 && !Main.tileNoAttach[num31])
																{
																	num31 = num;
																}
																if (num32 > -1 && !Main.tileNoAttach[num32])
																{
																	num32 = num;
																}
																if (num28 > -1 && !Main.tileNoAttach[num28])
																{
																	num28 = num;
																}
																if (num30 > -1 && !Main.tileNoAttach[num30])
																{
																	num30 = num;
																}
																if (num33 > -1 && !Main.tileNoAttach[num33])
																{
																	num33 = num;
																}
																if (num35 > -1 && !Main.tileNoAttach[num35])
																{
																	num35 = num;
																}
															}
														}
													}
												}
											}
										}
									}
								}
								bool flag3 = false;
								if (num == 2 || num == 23 || num == 60 || num == 70 || num == 109)
								{
									flag3 = true;
									if (num29 > -1 && !Main.tileSolid[num29] && num29 != num)
									{
										num29 = -1;
									}
									if (num34 > -1 && !Main.tileSolid[num34] && num34 != num)
									{
										num34 = -1;
									}
									if (num31 > -1 && !Main.tileSolid[num31] && num31 != num)
									{
										num31 = -1;
									}
									if (num32 > -1 && !Main.tileSolid[num32] && num32 != num)
									{
										num32 = -1;
									}
									if (num28 > -1 && !Main.tileSolid[num28] && num28 != num)
									{
										num28 = -1;
									}
									if (num30 > -1 && !Main.tileSolid[num30] && num30 != num)
									{
										num30 = -1;
									}
									if (num33 > -1 && !Main.tileSolid[num33] && num33 != num)
									{
										num33 = -1;
									}
									if (num35 > -1 && !Main.tileSolid[num35] && num35 != num)
									{
										num35 = -1;
									}
									int num37 = 0;
									if (num == 60 || num == 70)
									{
										num37 = 59;
									}
									else
									{
										if (num == 2)
										{
											if (num29 == 23)
											{
												num29 = num37;
											}
											if (num34 == 23)
											{
												num34 = num37;
											}
											if (num31 == 23)
											{
												num31 = num37;
											}
											if (num32 == 23)
											{
												num32 = num37;
											}
											if (num28 == 23)
											{
												num28 = num37;
											}
											if (num30 == 23)
											{
												num30 = num37;
											}
											if (num33 == 23)
											{
												num33 = num37;
											}
											if (num35 == 23)
											{
												num35 = num37;
											}
										}
										else
										{
											if (num == 23)
											{
												if (num29 == 2)
												{
													num29 = num37;
												}
												if (num34 == 2)
												{
													num34 = num37;
												}
												if (num31 == 2)
												{
													num31 = num37;
												}
												if (num32 == 2)
												{
													num32 = num37;
												}
												if (num28 == 2)
												{
													num28 = num37;
												}
												if (num30 == 2)
												{
													num30 = num37;
												}
												if (num33 == 2)
												{
													num33 = num37;
												}
												if (num35 == 2)
												{
													num35 = num37;
												}
											}
										}
									}
									if (num29 != num && num29 != num37 && (num34 == num || num34 == num37))
									{
										if (num31 == num37 && num32 == num)
										{
											if (num36 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 198;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 18;
													rectangle.Y = 198;
												}
												else
												{
													rectangle.X = 36;
													rectangle.Y = 198;
												}
											}
										}
										else
										{
											if (num31 == num && num32 == num37)
											{
												if (num36 == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 198;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 198;
													}
													else
													{
														rectangle.X = 90;
														rectangle.Y = 198;
													}
												}
											}
										}
									}
									else
									{
										if (num34 != num && num34 != num37 && (num29 == num || num29 == num37))
										{
											if (num31 == num37 && num32 == num)
											{
												if (num36 == 0)
												{
													rectangle.X = 0;
													rectangle.Y = 216;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 18;
														rectangle.Y = 216;
													}
													else
													{
														rectangle.X = 36;
														rectangle.Y = 216;
													}
												}
											}
											else
											{
												if (num31 == num && num32 == num37)
												{
													if (num36 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 216;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 216;
														}
														else
														{
															rectangle.X = 90;
															rectangle.Y = 216;
														}
													}
												}
											}
										}
										else
										{
											if (num31 != num && num31 != num37 && (num32 == num || num32 == num37))
											{
												if (num29 == num37 && num34 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 72;
														rectangle.Y = 144;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 162;
														}
														else
														{
															rectangle.X = 72;
															rectangle.Y = 180;
														}
													}
												}
												else
												{
													if (num34 == num && num32 == num29)
													{
														if (num36 == 0)
														{
															rectangle.X = 72;
															rectangle.Y = 90;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 72;
																rectangle.Y = 108;
															}
															else
															{
																rectangle.X = 72;
																rectangle.Y = 126;
															}
														}
													}
												}
											}
											else
											{
												if (num32 != num && num32 != num37 && (num31 == num || num31 == num37))
												{
													if (num29 == num37 && num34 == num)
													{
														if (num36 == 0)
														{
															rectangle.X = 90;
															rectangle.Y = 144;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 90;
																rectangle.Y = 162;
															}
															else
															{
																rectangle.X = 90;
																rectangle.Y = 180;
															}
														}
													}
													else
													{
														if (num34 == num && num32 == num29)
														{
															if (num36 == 0)
															{
																rectangle.X = 90;
																rectangle.Y = 90;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 90;
																	rectangle.Y = 108;
																}
																else
																{
																	rectangle.X = 90;
																	rectangle.Y = 126;
																}
															}
														}
													}
												}
												else
												{
													if (num29 == num && num34 == num && num31 == num && num32 == num)
													{
														if (num28 != num && num30 != num && num33 != num && num35 != num)
														{
															if (num35 == num37)
															{
																if (num36 == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 324;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 324;
																	}
																	else
																	{
																		rectangle.X = 144;
																		rectangle.Y = 324;
																	}
																}
															}
															else
															{
																if (num30 == num37)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 342;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 342;
																		}
																		else
																		{
																			rectangle.X = 144;
																			rectangle.Y = 342;
																		}
																	}
																}
																else
																{
																	if (num33 == num37)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 360;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 126;
																				rectangle.Y = 360;
																			}
																			else
																			{
																				rectangle.X = 144;
																				rectangle.Y = 360;
																			}
																		}
																	}
																	else
																	{
																		if (num28 == num37)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 108;
																				rectangle.Y = 378;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 126;
																					rectangle.Y = 378;
																				}
																				else
																				{
																					rectangle.X = 144;
																					rectangle.Y = 378;
																				}
																			}
																		}
																		else
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 144;
																				rectangle.Y = 234;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 198;
																					rectangle.Y = 234;
																				}
																				else
																				{
																					rectangle.X = 252;
																					rectangle.Y = 234;
																				}
																			}
																		}
																	}
																}
															}
														}
														else
														{
															if (num28 != num && num35 != num)
															{
																if (num36 == 0)
																{
																	rectangle.X = 36;
																	rectangle.Y = 306;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 306;
																	}
																	else
																	{
																		rectangle.X = 72;
																		rectangle.Y = 306;
																	}
																}
															}
															else
															{
																if (num30 != num && num33 != num)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 90;
																		rectangle.Y = 306;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 306;
																		}
																		else
																		{
																			rectangle.X = 126;
																			rectangle.Y = 306;
																		}
																	}
																}
																else
																{
																	if (num28 != num && num30 == num && num33 == num && num35 == num)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 108;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 144;
																			}
																			else
																			{
																				rectangle.X = 54;
																				rectangle.Y = 180;
																			}
																		}
																	}
																	else
																	{
																		if (num28 == num && num30 != num && num33 == num && num35 == num)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 108;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 144;
																				}
																				else
																				{
																					rectangle.X = 36;
																					rectangle.Y = 180;
																				}
																			}
																		}
																		else
																		{
																			if (num28 == num && num30 == num && num33 != num && num35 == num)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 90;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 54;
																						rectangle.Y = 126;
																					}
																					else
																					{
																						rectangle.X = 54;
																						rectangle.Y = 162;
																					}
																				}
																			}
																			else
																			{
																				if (num28 == num && num30 == num && num33 == num && num35 != num)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 90;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 36;
																							rectangle.Y = 126;
																						}
																						else
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
													}
													else
													{
														if (num29 == num && num34 == num37 && num31 == num && num32 == num && num28 == -1 && num30 == -1)
														{
															if (num36 == 0)
															{
																rectangle.X = 108;
																rectangle.Y = 18;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 126;
																	rectangle.Y = 18;
																}
																else
																{
																	rectangle.X = 144;
																	rectangle.Y = 18;
																}
															}
														}
														else
														{
															if (num29 == num37 && num34 == num && num31 == num && num32 == num && num33 == -1 && num35 == -1)
															{
																if (num36 == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 36;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 36;
																	}
																	else
																	{
																		rectangle.X = 144;
																		rectangle.Y = 36;
																	}
																}
															}
															else
															{
																if (num29 == num && num34 == num && num31 == num37 && num32 == num && num30 == -1 && num35 == -1)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 198;
																		rectangle.Y = 0;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 198;
																			rectangle.Y = 18;
																		}
																		else
																		{
																			rectangle.X = 198;
																			rectangle.Y = 36;
																		}
																	}
																}
																else
																{
																	if (num29 == num && num34 == num && num31 == num && num32 == num37 && num28 == -1 && num33 == -1)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 180;
																			rectangle.Y = 0;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 180;
																				rectangle.Y = 18;
																			}
																			else
																			{
																				rectangle.X = 180;
																				rectangle.Y = 36;
																			}
																		}
																	}
																	else
																	{
																		if (num29 == num && num34 == num37 && num31 == num && num32 == num)
																		{
																			if (num30 != -1)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 108;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 54;
																						rectangle.Y = 144;
																					}
																					else
																					{
																						rectangle.X = 54;
																						rectangle.Y = 180;
																					}
																				}
																			}
																			else
																			{
																				if (num28 != -1)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 108;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 36;
																							rectangle.Y = 144;
																						}
																						else
																						{
																							rectangle.X = 36;
																							rectangle.Y = 180;
																						}
																					}
																				}
																			}
																		}
																		else
																		{
																			if (num29 == num37 && num34 == num && num31 == num && num32 == num)
																			{
																				if (num35 != -1)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 54;
																						rectangle.Y = 90;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 54;
																							rectangle.Y = 126;
																						}
																						else
																						{
																							rectangle.X = 54;
																							rectangle.Y = 162;
																						}
																					}
																				}
																				else
																				{
																					if (num33 != -1)
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 36;
																							rectangle.Y = 90;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 36;
																								rectangle.Y = 126;
																							}
																							else
																							{
																								rectangle.X = 36;
																								rectangle.Y = 162;
																							}
																						}
																					}
																				}
																			}
																			else
																			{
																				if (num29 == num && num34 == num && num31 == num && num32 == num37)
																				{
																					if (num28 != -1)
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 54;
																							rectangle.Y = 90;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 54;
																								rectangle.Y = 126;
																							}
																							else
																							{
																								rectangle.X = 54;
																								rectangle.Y = 162;
																							}
																						}
																					}
																					else
																					{
																						if (num33 != -1)
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 54;
																								rectangle.Y = 108;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 54;
																									rectangle.Y = 144;
																								}
																								else
																								{
																									rectangle.X = 54;
																									rectangle.Y = 180;
																								}
																							}
																						}
																					}
																				}
																				else
																				{
																					if (num29 == num && num34 == num && num31 == num37 && num32 == num)
																					{
																						if (num30 != -1)
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 36;
																								rectangle.Y = 90;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 36;
																									rectangle.Y = 126;
																								}
																								else
																								{
																									rectangle.X = 36;
																									rectangle.Y = 162;
																								}
																							}
																						}
																						else
																						{
																							if (num35 != -1)
																							{
																								if (num36 == 0)
																								{
																									rectangle.X = 36;
																									rectangle.Y = 108;
																								}
																								else
																								{
																									if (num36 == 1)
																									{
																										rectangle.X = 36;
																										rectangle.Y = 144;
																									}
																									else
																									{
																										rectangle.X = 36;
																										rectangle.Y = 180;
																									}
																								}
																							}
																						}
																					}
																					else
																					{
																						if ((num29 == num37 && num34 == num && num31 == num && num32 == num) || (num29 == num && num34 == num37 && num31 == num && num32 == num) || (num29 == num && num34 == num && num31 == num37 && num32 == num) || (num29 == num && num34 == num && num31 == num && num32 == num37))
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 18;
																								rectangle.Y = 18;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 36;
																									rectangle.Y = 18;
																								}
																								else
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
									}
									if ((num29 == num || num29 == num37) && (num34 == num || num34 == num37) && (num31 == num || num31 == num37) && (num32 == num || num32 == num37))
									{
										if (num28 != num && num28 != num37 && (num30 == num || num30 == num37) && (num33 == num || num33 == num37) && (num35 == num || num35 == num37))
										{
											if (num36 == 0)
											{
												rectangle.X = 54;
												rectangle.Y = 108;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 54;
													rectangle.Y = 144;
												}
												else
												{
													rectangle.X = 54;
													rectangle.Y = 180;
												}
											}
										}
										else
										{
											if (num30 != num && num30 != num37 && (num28 == num || num28 == num37) && (num33 == num || num33 == num37) && (num35 == num || num35 == num37))
											{
												if (num36 == 0)
												{
													rectangle.X = 36;
													rectangle.Y = 108;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 36;
														rectangle.Y = 144;
													}
													else
													{
														rectangle.X = 36;
														rectangle.Y = 180;
													}
												}
											}
											else
											{
												if (num33 != num && num33 != num37 && (num28 == num || num28 == num37) && (num30 == num || num30 == num37) && (num35 == num || num35 == num37))
												{
													if (num36 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 90;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 54;
															rectangle.Y = 126;
														}
														else
														{
															rectangle.X = 54;
															rectangle.Y = 162;
														}
													}
												}
												else
												{
													if (num35 != num && num35 != num37 && (num28 == num || num28 == num37) && (num33 == num || num33 == num37) && (num30 == num || num30 == num37))
													{
														if (num36 == 0)
														{
															rectangle.X = 36;
															rectangle.Y = 90;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 36;
																rectangle.Y = 126;
															}
															else
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
									if (num29 != num37 && num29 != num && num34 == num && num31 != num37 && num31 != num && num32 == num && num35 != num37 && num35 != num)
									{
										if (num36 == 0)
										{
											rectangle.X = 90;
											rectangle.Y = 270;
										}
										else
										{
											if (num36 == 1)
											{
												rectangle.X = 108;
												rectangle.Y = 270;
											}
											else
											{
												rectangle.X = 126;
												rectangle.Y = 270;
											}
										}
									}
									else
									{
										if (num29 != num37 && num29 != num && num34 == num && num31 == num && num32 != num37 && num32 != num && num33 != num37 && num33 != num)
										{
											if (num36 == 0)
											{
												rectangle.X = 144;
												rectangle.Y = 270;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 162;
													rectangle.Y = 270;
												}
												else
												{
													rectangle.X = 180;
													rectangle.Y = 270;
												}
											}
										}
										else
										{
											if (num34 != num37 && num34 != num && num29 == num && num31 != num37 && num31 != num && num32 == num && num30 != num37 && num30 != num)
											{
												if (num36 == 0)
												{
													rectangle.X = 90;
													rectangle.Y = 288;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 108;
														rectangle.Y = 288;
													}
													else
													{
														rectangle.X = 126;
														rectangle.Y = 288;
													}
												}
											}
											else
											{
												if (num34 != num37 && num34 != num && num29 == num && num31 == num && num32 != num37 && num32 != num && num28 != num37 && num28 != num)
												{
													if (num36 == 0)
													{
														rectangle.X = 144;
														rectangle.Y = 288;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 162;
															rectangle.Y = 288;
														}
														else
														{
															rectangle.X = 180;
															rectangle.Y = 288;
														}
													}
												}
												else
												{
													if (num29 != num && num29 != num37 && num34 == num && num31 == num && num32 == num && num33 != num && num33 != num37 && num35 != num && num35 != num37)
													{
														if (num36 == 0)
														{
															rectangle.X = 144;
															rectangle.Y = 216;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 198;
																rectangle.Y = 216;
															}
															else
															{
																rectangle.X = 252;
																rectangle.Y = 216;
															}
														}
													}
													else
													{
														if (num34 != num && num34 != num37 && num29 == num && num31 == num && num32 == num && num28 != num && num28 != num37 && num30 != num && num30 != num37)
														{
															if (num36 == 0)
															{
																rectangle.X = 144;
																rectangle.Y = 252;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 198;
																	rectangle.Y = 252;
																}
																else
																{
																	rectangle.X = 252;
																	rectangle.Y = 252;
																}
															}
														}
														else
														{
															if (num31 != num && num31 != num37 && num34 == num && num29 == num && num32 == num && num30 != num && num30 != num37 && num35 != num && num35 != num37)
															{
																if (num36 == 0)
																{
																	rectangle.X = 126;
																	rectangle.Y = 234;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 180;
																		rectangle.Y = 234;
																	}
																	else
																	{
																		rectangle.X = 234;
																		rectangle.Y = 234;
																	}
																}
															}
															else
															{
																if (num32 != num && num32 != num37 && num34 == num && num29 == num && num31 == num && num28 != num && num28 != num37 && num33 != num && num33 != num37)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 162;
																		rectangle.Y = 234;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 216;
																			rectangle.Y = 234;
																		}
																		else
																		{
																			rectangle.X = 270;
																			rectangle.Y = 234;
																		}
																	}
																}
																else
																{
																	if (num29 != num37 && num29 != num && (num34 == num37 || num34 == num) && num31 == num37 && num32 == num37)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 270;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 270;
																			}
																			else
																			{
																				rectangle.X = 72;
																				rectangle.Y = 270;
																			}
																		}
																	}
																	else
																	{
																		if (num34 != num37 && num34 != num && (num29 == num37 || num29 == num) && num31 == num37 && num32 == num37)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 288;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 288;
																				}
																				else
																				{
																					rectangle.X = 72;
																					rectangle.Y = 288;
																				}
																			}
																		}
																		else
																		{
																			if (num31 != num37 && num31 != num && (num32 == num37 || num32 == num) && num29 == num37 && num34 == num37)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 0;
																					rectangle.Y = 270;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 0;
																						rectangle.Y = 288;
																					}
																					else
																					{
																						rectangle.X = 0;
																						rectangle.Y = 306;
																					}
																				}
																			}
																			else
																			{
																				if (num32 != num37 && num32 != num && (num31 == num37 || num31 == num) && num29 == num37 && num34 == num37)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 18;
																						rectangle.Y = 270;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 18;
																							rectangle.Y = 288;
																						}
																						else
																						{
																							rectangle.X = 18;
																							rectangle.Y = 306;
																						}
																					}
																				}
																				else
																				{
																					if (num29 == num && num34 == num37 && num31 == num37 && num32 == num37)
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 198;
																							rectangle.Y = 288;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 216;
																								rectangle.Y = 288;
																							}
																							else
																							{
																								rectangle.X = 234;
																								rectangle.Y = 288;
																							}
																						}
																					}
																					else
																					{
																						if (num29 == num37 && num34 == num && num31 == num37 && num32 == num37)
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 198;
																								rectangle.Y = 270;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 216;
																									rectangle.Y = 270;
																								}
																								else
																								{
																									rectangle.X = 234;
																									rectangle.Y = 270;
																								}
																							}
																						}
																						else
																						{
																							if (num29 == num37 && num34 == num37 && num31 == num && num32 == num37)
																							{
																								if (num36 == 0)
																								{
																									rectangle.X = 198;
																									rectangle.Y = 306;
																								}
																								else
																								{
																									if (num36 == 1)
																									{
																										rectangle.X = 216;
																										rectangle.Y = 306;
																									}
																									else
																									{
																										rectangle.X = 234;
																										rectangle.Y = 306;
																									}
																								}
																							}
																							else
																							{
																								if (num29 == num37 && num34 == num37 && num31 == num37 && num32 == num)
																								{
																									if (num36 == 0)
																									{
																										rectangle.X = 144;
																										rectangle.Y = 306;
																									}
																									else
																									{
																										if (num36 == 1)
																										{
																											rectangle.X = 162;
																											rectangle.Y = 306;
																										}
																										else
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
									}
									if (num29 != num && num29 != num37 && num34 == num && num31 == num && num32 == num)
									{
										if ((num33 == num37 || num33 == num) && num35 != num37 && num35 != num)
										{
											if (num36 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 324;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 18;
													rectangle.Y = 324;
												}
												else
												{
													rectangle.X = 36;
													rectangle.Y = 324;
												}
											}
										}
										else
										{
											if ((num35 == num37 || num35 == num) && num33 != num37 && num33 != num)
											{
												if (num36 == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 324;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 324;
													}
													else
													{
														rectangle.X = 90;
														rectangle.Y = 324;
													}
												}
											}
										}
									}
									else
									{
										if (num34 != num && num34 != num37 && num29 == num && num31 == num && num32 == num)
										{
											if ((num28 == num37 || num28 == num) && num30 != num37 && num30 != num)
											{
												if (num36 == 0)
												{
													rectangle.X = 0;
													rectangle.Y = 342;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 18;
														rectangle.Y = 342;
													}
													else
													{
														rectangle.X = 36;
														rectangle.Y = 342;
													}
												}
											}
											else
											{
												if ((num30 == num37 || num30 == num) && num28 != num37 && num28 != num)
												{
													if (num36 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 342;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 342;
														}
														else
														{
															rectangle.X = 90;
															rectangle.Y = 342;
														}
													}
												}
											}
										}
										else
										{
											if (num31 != num && num31 != num37 && num29 == num && num34 == num && num32 == num)
											{
												if ((num30 == num37 || num30 == num) && num35 != num37 && num35 != num)
												{
													if (num36 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 360;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 360;
														}
														else
														{
															rectangle.X = 90;
															rectangle.Y = 360;
														}
													}
												}
												else
												{
													if ((num35 == num37 || num35 == num) && num30 != num37 && num30 != num)
													{
														if (num36 == 0)
														{
															rectangle.X = 0;
															rectangle.Y = 360;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 18;
																rectangle.Y = 360;
															}
															else
															{
																rectangle.X = 36;
																rectangle.Y = 360;
															}
														}
													}
												}
											}
											else
											{
												if (num32 != num && num32 != num37 && num29 == num && num34 == num && num31 == num)
												{
													if ((num28 == num37 || num28 == num) && num33 != num37 && num33 != num)
													{
														if (num36 == 0)
														{
															rectangle.X = 0;
															rectangle.Y = 378;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 18;
																rectangle.Y = 378;
															}
															else
															{
																rectangle.X = 36;
																rectangle.Y = 378;
															}
														}
													}
													else
													{
														if ((num33 == num37 || num33 == num) && num28 != num37 && num28 != num)
														{
															if (num36 == 0)
															{
																rectangle.X = 54;
																rectangle.Y = 378;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 72;
																	rectangle.Y = 378;
																}
																else
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
									}
									if ((num29 == num || num29 == num37) && (num34 == num || num34 == num37) && (num31 == num || num31 == num37) && (num32 == num || num32 == num37) && num28 != -1 && num30 != -1 && num33 != -1 && num35 != -1)
									{
										if (num36 == 0)
										{
											rectangle.X = 18;
											rectangle.Y = 18;
										}
										else
										{
											if (num36 == 1)
											{
												rectangle.X = 36;
												rectangle.Y = 18;
											}
											else
											{
												rectangle.X = 54;
												rectangle.Y = 18;
											}
										}
									}
									if (num29 == num37)
									{
										num29 = -2;
									}
									if (num34 == num37)
									{
										num34 = -2;
									}
									if (num31 == num37)
									{
										num31 = -2;
									}
									if (num32 == num37)
									{
										num32 = -2;
									}
									if (num28 == num37)
									{
										num28 = -2;
									}
									if (num30 == num37)
									{
										num30 = -2;
									}
									if (num33 == num37)
									{
										num33 = -2;
									}
									if (num35 == num37)
									{
										num35 = -2;
									}
								}
								if (rectangle.X == -1 && rectangle.Y == -1 && (Main.tileMergeDirt[num] || num == 0 || num == 2 || num == 57 || num == 58 || num == 59 || num == 60 || num == 70 || num == 109 || num == 76 || num == 75))
								{
									if (!flag3)
									{
										flag3 = true;
										if (num29 > -1 && !Main.tileSolid[num29] && num29 != num)
										{
											num29 = -1;
										}
										if (num34 > -1 && !Main.tileSolid[num34] && num34 != num)
										{
											num34 = -1;
										}
										if (num31 > -1 && !Main.tileSolid[num31] && num31 != num)
										{
											num31 = -1;
										}
										if (num32 > -1 && !Main.tileSolid[num32] && num32 != num)
										{
											num32 = -1;
										}
										if (num28 > -1 && !Main.tileSolid[num28] && num28 != num)
										{
											num28 = -1;
										}
										if (num30 > -1 && !Main.tileSolid[num30] && num30 != num)
										{
											num30 = -1;
										}
										if (num33 > -1 && !Main.tileSolid[num33] && num33 != num)
										{
											num33 = -1;
										}
										if (num35 > -1 && !Main.tileSolid[num35] && num35 != num)
										{
											num35 = -1;
										}
									}
									if (num29 >= 0 && num29 != num)
									{
										num29 = -1;
									}
									if (num34 >= 0 && num34 != num)
									{
										num34 = -1;
									}
									if (num31 >= 0 && num31 != num)
									{
										num31 = -1;
									}
									if (num32 >= 0 && num32 != num)
									{
										num32 = -1;
									}
									if (num29 != -1 && num34 != -1 && num31 != -1 && num32 != -1)
									{
										if (num29 == -2 && num34 == num && num31 == num && num32 == num)
										{
											if (num36 == 0)
											{
												rectangle.X = 144;
												rectangle.Y = 108;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 162;
													rectangle.Y = 108;
												}
												else
												{
													rectangle.X = 180;
													rectangle.Y = 108;
												}
											}
											mergeUp = true;
										}
										else
										{
											if (num29 == num && num34 == -2 && num31 == num && num32 == num)
											{
												if (num36 == 0)
												{
													rectangle.X = 144;
													rectangle.Y = 90;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 162;
														rectangle.Y = 90;
													}
													else
													{
														rectangle.X = 180;
														rectangle.Y = 90;
													}
												}
												mergeDown = true;
											}
											else
											{
												if (num29 == num && num34 == num && num31 == -2 && num32 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 162;
														rectangle.Y = 126;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 162;
															rectangle.Y = 144;
														}
														else
														{
															rectangle.X = 162;
															rectangle.Y = 162;
														}
													}
													mergeLeft = true;
												}
												else
												{
													if (num29 == num && num34 == num && num31 == num && num32 == -2)
													{
														if (num36 == 0)
														{
															rectangle.X = 144;
															rectangle.Y = 126;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 144;
																rectangle.Y = 144;
															}
															else
															{
																rectangle.X = 144;
																rectangle.Y = 162;
															}
														}
														mergeRight = true;
													}
													else
													{
														if (num29 == -2 && num34 == num && num31 == -2 && num32 == num)
														{
															if (num36 == 0)
															{
																rectangle.X = 36;
																rectangle.Y = 90;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 36;
																	rectangle.Y = 126;
																}
																else
																{
																	rectangle.X = 36;
																	rectangle.Y = 162;
																}
															}
															mergeUp = true;
															mergeLeft = true;
														}
														else
														{
															if (num29 == -2 && num34 == num && num31 == num && num32 == -2)
															{
																if (num36 == 0)
																{
																	rectangle.X = 54;
																	rectangle.Y = 90;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 126;
																	}
																	else
																	{
																		rectangle.X = 54;
																		rectangle.Y = 162;
																	}
																}
																mergeUp = true;
																mergeRight = true;
															}
															else
															{
																if (num29 == num && num34 == -2 && num31 == -2 && num32 == num)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 36;
																		rectangle.Y = 108;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 144;
																		}
																		else
																		{
																			rectangle.X = 36;
																			rectangle.Y = 180;
																		}
																	}
																	mergeDown = true;
																	mergeLeft = true;
																}
																else
																{
																	if (num29 == num && num34 == -2 && num31 == num && num32 == -2)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 108;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 144;
																			}
																			else
																			{
																				rectangle.X = 54;
																				rectangle.Y = 180;
																			}
																		}
																		mergeDown = true;
																		mergeRight = true;
																	}
																	else
																	{
																		if (num29 == num && num34 == num && num31 == -2 && num32 == -2)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 180;
																				rectangle.Y = 126;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 180;
																					rectangle.Y = 144;
																				}
																				else
																				{
																					rectangle.X = 180;
																					rectangle.Y = 162;
																				}
																			}
																			mergeLeft = true;
																			mergeRight = true;
																		}
																		else
																		{
																			if (num29 == -2 && num34 == -2 && num31 == num && num32 == num)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 144;
																					rectangle.Y = 180;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 162;
																						rectangle.Y = 180;
																					}
																					else
																					{
																						rectangle.X = 180;
																						rectangle.Y = 180;
																					}
																				}
																				mergeUp = true;
																				mergeDown = true;
																			}
																			else
																			{
																				if (num29 == -2 && num34 == num && num31 == -2 && num32 == -2)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 198;
																						rectangle.Y = 90;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 198;
																							rectangle.Y = 108;
																						}
																						else
																						{
																							rectangle.X = 198;
																							rectangle.Y = 126;
																						}
																					}
																					mergeUp = true;
																					mergeLeft = true;
																					mergeRight = true;
																				}
																				else
																				{
																					if (num29 == num && num34 == -2 && num31 == -2 && num32 == -2)
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 198;
																							rectangle.Y = 144;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 198;
																								rectangle.Y = 162;
																							}
																							else
																							{
																								rectangle.X = 198;
																								rectangle.Y = 180;
																							}
																						}
																						mergeDown = true;
																						mergeLeft = true;
																						mergeRight = true;
																					}
																					else
																					{
																						if (num29 == -2 && num34 == -2 && num31 == num && num32 == -2)
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 216;
																								rectangle.Y = 144;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 216;
																									rectangle.Y = 162;
																								}
																								else
																								{
																									rectangle.X = 216;
																									rectangle.Y = 180;
																								}
																							}
																							mergeUp = true;
																							mergeDown = true;
																							mergeRight = true;
																						}
																						else
																						{
																							if (num29 == -2 && num34 == -2 && num31 == -2 && num32 == num)
																							{
																								if (num36 == 0)
																								{
																									rectangle.X = 216;
																									rectangle.Y = 90;
																								}
																								else
																								{
																									if (num36 == 1)
																									{
																										rectangle.X = 216;
																										rectangle.Y = 108;
																									}
																									else
																									{
																										rectangle.X = 216;
																										rectangle.Y = 126;
																									}
																								}
																								mergeUp = true;
																								mergeDown = true;
																								mergeLeft = true;
																							}
																							else
																							{
																								if (num29 == -2 && num34 == -2 && num31 == -2 && num32 == -2)
																								{
																									if (num36 == 0)
																									{
																										rectangle.X = 108;
																										rectangle.Y = 198;
																									}
																									else
																									{
																										if (num36 == 1)
																										{
																											rectangle.X = 126;
																											rectangle.Y = 198;
																										}
																										else
																										{
																											rectangle.X = 144;
																											rectangle.Y = 198;
																										}
																									}
																									mergeUp = true;
																									mergeDown = true;
																									mergeLeft = true;
																									mergeRight = true;
																								}
																								else
																								{
																									if (num29 == num && num34 == num && num31 == num && num32 == num)
																									{
																										if (num28 == -2)
																										{
																											if (num36 == 0)
																											{
																												rectangle.X = 18;
																												rectangle.Y = 108;
																											}
																											else
																											{
																												if (num36 == 1)
																												{
																													rectangle.X = 18;
																													rectangle.Y = 144;
																												}
																												else
																												{
																													rectangle.X = 18;
																													rectangle.Y = 180;
																												}
																											}
																										}
																										if (num30 == -2)
																										{
																											if (num36 == 0)
																											{
																												rectangle.X = 0;
																												rectangle.Y = 108;
																											}
																											else
																											{
																												if (num36 == 1)
																												{
																													rectangle.X = 0;
																													rectangle.Y = 144;
																												}
																												else
																												{
																													rectangle.X = 0;
																													rectangle.Y = 180;
																												}
																											}
																										}
																										if (num33 == -2)
																										{
																											if (num36 == 0)
																											{
																												rectangle.X = 18;
																												rectangle.Y = 90;
																											}
																											else
																											{
																												if (num36 == 1)
																												{
																													rectangle.X = 18;
																													rectangle.Y = 126;
																												}
																												else
																												{
																													rectangle.X = 18;
																													rectangle.Y = 162;
																												}
																											}
																										}
																										if (num35 == -2)
																										{
																											if (num36 == 0)
																											{
																												rectangle.X = 0;
																												rectangle.Y = 90;
																											}
																											else
																											{
																												if (num36 == 1)
																												{
																													rectangle.X = 0;
																													rectangle.Y = 126;
																												}
																												else
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
									}
									else
									{
										if (num != 2 && num != 23 && num != 60 && num != 70 && num != 109)
										{
											if (num29 == -1 && num34 == -2 && num31 == num && num32 == num)
											{
												if (num36 == 0)
												{
													rectangle.X = 234;
													rectangle.Y = 0;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 252;
														rectangle.Y = 0;
													}
													else
													{
														rectangle.X = 270;
														rectangle.Y = 0;
													}
												}
												mergeDown = true;
											}
											else
											{
												if (num29 == -2 && num34 == -1 && num31 == num && num32 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 234;
														rectangle.Y = 18;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 252;
															rectangle.Y = 18;
														}
														else
														{
															rectangle.X = 270;
															rectangle.Y = 18;
														}
													}
													mergeUp = true;
												}
												else
												{
													if (num29 == num && num34 == num && num31 == -1 && num32 == -2)
													{
														if (num36 == 0)
														{
															rectangle.X = 234;
															rectangle.Y = 36;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 252;
																rectangle.Y = 36;
															}
															else
															{
																rectangle.X = 270;
																rectangle.Y = 36;
															}
														}
														mergeRight = true;
													}
													else
													{
														if (num29 == num && num34 == num && num31 == -2 && num32 == -1)
														{
															if (num36 == 0)
															{
																rectangle.X = 234;
																rectangle.Y = 54;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 252;
																	rectangle.Y = 54;
																}
																else
																{
																	rectangle.X = 270;
																	rectangle.Y = 54;
																}
															}
															mergeLeft = true;
														}
													}
												}
											}
										}
										if (num29 != -1 && num34 != -1 && num31 == -1 && num32 == num)
										{
											if (num29 == -2 && num34 == num)
											{
												if (num36 == 0)
												{
													rectangle.X = 72;
													rectangle.Y = 144;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 162;
													}
													else
													{
														rectangle.X = 72;
														rectangle.Y = 180;
													}
												}
												mergeUp = true;
											}
											else
											{
												if (num34 == -2 && num29 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 72;
														rectangle.Y = 90;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 108;
														}
														else
														{
															rectangle.X = 72;
															rectangle.Y = 126;
														}
													}
													mergeDown = true;
												}
											}
										}
										else
										{
											if (num29 != -1 && num34 != -1 && num31 == num && num32 == -1)
											{
												if (num29 == -2 && num34 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 90;
														rectangle.Y = 144;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 90;
															rectangle.Y = 162;
														}
														else
														{
															rectangle.X = 90;
															rectangle.Y = 180;
														}
													}
													mergeUp = true;
												}
												else
												{
													if (num34 == -2 && num29 == num)
													{
														if (num36 == 0)
														{
															rectangle.X = 90;
															rectangle.Y = 90;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 90;
																rectangle.Y = 108;
															}
															else
															{
																rectangle.X = 90;
																rectangle.Y = 126;
															}
														}
														mergeDown = true;
													}
												}
											}
											else
											{
												if (num29 == -1 && num34 == num && num31 != -1 && num32 != -1)
												{
													if (num31 == -2 && num32 == num)
													{
														if (num36 == 0)
														{
															rectangle.X = 0;
															rectangle.Y = 198;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 18;
																rectangle.Y = 198;
															}
															else
															{
																rectangle.X = 36;
																rectangle.Y = 198;
															}
														}
														mergeLeft = true;
													}
													else
													{
														if (num32 == -2 && num31 == num)
														{
															if (num36 == 0)
															{
																rectangle.X = 54;
																rectangle.Y = 198;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 72;
																	rectangle.Y = 198;
																}
																else
																{
																	rectangle.X = 90;
																	rectangle.Y = 198;
																}
															}
															mergeRight = true;
														}
													}
												}
												else
												{
													if (num29 == num && num34 == -1 && num31 != -1 && num32 != -1)
													{
														if (num31 == -2 && num32 == num)
														{
															if (num36 == 0)
															{
																rectangle.X = 0;
																rectangle.Y = 216;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 18;
																	rectangle.Y = 216;
																}
																else
																{
																	rectangle.X = 36;
																	rectangle.Y = 216;
																}
															}
															mergeLeft = true;
														}
														else
														{
															if (num32 == -2 && num31 == num)
															{
																if (num36 == 0)
																{
																	rectangle.X = 54;
																	rectangle.Y = 216;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 72;
																		rectangle.Y = 216;
																	}
																	else
																	{
																		rectangle.X = 90;
																		rectangle.Y = 216;
																	}
																}
																mergeRight = true;
															}
														}
													}
													else
													{
														if (num29 != -1 && num34 != -1 && num31 == -1 && num32 == -1)
														{
															if (num29 == -2 && num34 == -2)
															{
																if (num36 == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 216;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 234;
																	}
																	else
																	{
																		rectangle.X = 108;
																		rectangle.Y = 252;
																	}
																}
																mergeUp = true;
																mergeDown = true;
															}
															else
															{
																if (num29 == -2)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 144;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 162;
																		}
																		else
																		{
																			rectangle.X = 126;
																			rectangle.Y = 180;
																		}
																	}
																	mergeUp = true;
																}
																else
																{
																	if (num34 == -2)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 90;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 126;
																				rectangle.Y = 108;
																			}
																			else
																			{
																				rectangle.X = 126;
																				rectangle.Y = 126;
																			}
																		}
																		mergeDown = true;
																	}
																}
															}
														}
														else
														{
															if (num29 == -1 && num34 == -1 && num31 != -1 && num32 != -1)
															{
																if (num31 == -2 && num32 == -2)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 162;
																		rectangle.Y = 198;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 180;
																			rectangle.Y = 198;
																		}
																		else
																		{
																			rectangle.X = 198;
																			rectangle.Y = 198;
																		}
																	}
																	mergeLeft = true;
																	mergeRight = true;
																}
																else
																{
																	if (num31 == -2)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 0;
																			rectangle.Y = 252;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 18;
																				rectangle.Y = 252;
																			}
																			else
																			{
																				rectangle.X = 36;
																				rectangle.Y = 252;
																			}
																		}
																		mergeLeft = true;
																	}
																	else
																	{
																		if (num32 == -2)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 252;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 72;
																					rectangle.Y = 252;
																				}
																				else
																				{
																					rectangle.X = 90;
																					rectangle.Y = 252;
																				}
																			}
																			mergeRight = true;
																		}
																	}
																}
															}
															else
															{
																if (num29 == -2 && num34 == -1 && num31 == -1 && num32 == -1)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 144;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 162;
																		}
																		else
																		{
																			rectangle.X = 108;
																			rectangle.Y = 180;
																		}
																	}
																	mergeUp = true;
																}
																else
																{
																	if (num29 == -1 && num34 == -2 && num31 == -1 && num32 == -1)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 90;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 108;
																				rectangle.Y = 108;
																			}
																			else
																			{
																				rectangle.X = 108;
																				rectangle.Y = 126;
																			}
																		}
																		mergeDown = true;
																	}
																	else
																	{
																		if (num29 == -1 && num34 == -1 && num31 == -2 && num32 == -1)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 0;
																				rectangle.Y = 234;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 18;
																					rectangle.Y = 234;
																				}
																				else
																				{
																					rectangle.X = 36;
																					rectangle.Y = 234;
																				}
																			}
																			mergeLeft = true;
																		}
																		else
																		{
																			if (num29 == -1 && num34 == -1 && num31 == -1 && num32 == -2)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 234;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 72;
																						rectangle.Y = 234;
																					}
																					else
																					{
																						rectangle.X = 90;
																						rectangle.Y = 234;
																					}
																				}
																				mergeRight = true;
																			}
																		}
																	}
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
									if (!flag3)
									{
										flag3 = true;
										if (num29 > -1 && !Main.tileSolid[num29] && num29 != num)
										{
											num29 = -1;
										}
										if (num34 > -1 && !Main.tileSolid[num34] && num34 != num)
										{
											num34 = -1;
										}
										if (num31 > -1 && !Main.tileSolid[num31] && num31 != num)
										{
											num31 = -1;
										}
										if (num32 > -1 && !Main.tileSolid[num32] && num32 != num)
										{
											num32 = -1;
										}
										if (num28 > -1 && !Main.tileSolid[num28] && num28 != num)
										{
											num28 = -1;
										}
										if (num30 > -1 && !Main.tileSolid[num30] && num30 != num)
										{
											num30 = -1;
										}
										if (num33 > -1 && !Main.tileSolid[num33] && num33 != num)
										{
											num33 = -1;
										}
										if (num35 > -1 && !Main.tileSolid[num35] && num35 != num)
										{
											num35 = -1;
										}
									}
									if (num == 2 || num == 23 || num == 60 || num == 70 || num == 109)
									{
										if (num29 == -2)
										{
											num29 = num;
										}
										if (num34 == -2)
										{
											num34 = num;
										}
										if (num31 == -2)
										{
											num31 = num;
										}
										if (num32 == -2)
										{
											num32 = num;
										}
										if (num28 == -2)
										{
											num28 = num;
										}
										if (num30 == -2)
										{
											num30 = num;
										}
										if (num33 == -2)
										{
											num33 = num;
										}
										if (num35 == -2)
										{
											num35 = num;
										}
									}
									if (num29 == num && num34 == num && (num31 == num & num32 == num))
									{
										if (num28 != num && num30 != num)
										{
											if (num36 == 0)
											{
												rectangle.X = 108;
												rectangle.Y = 18;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 126;
													rectangle.Y = 18;
												}
												else
												{
													rectangle.X = 144;
													rectangle.Y = 18;
												}
											}
										}
										else
										{
											if (num33 != num && num35 != num)
											{
												if (num36 == 0)
												{
													rectangle.X = 108;
													rectangle.Y = 36;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 126;
														rectangle.Y = 36;
													}
													else
													{
														rectangle.X = 144;
														rectangle.Y = 36;
													}
												}
											}
											else
											{
												if (num28 != num && num33 != num)
												{
													if (num36 == 0)
													{
														rectangle.X = 180;
														rectangle.Y = 0;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 180;
															rectangle.Y = 18;
														}
														else
														{
															rectangle.X = 180;
															rectangle.Y = 36;
														}
													}
												}
												else
												{
													if (num30 != num && num35 != num)
													{
														if (num36 == 0)
														{
															rectangle.X = 198;
															rectangle.Y = 0;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 198;
																rectangle.Y = 18;
															}
															else
															{
																rectangle.X = 198;
																rectangle.Y = 36;
															}
														}
													}
													else
													{
														if (num36 == 0)
														{
															rectangle.X = 18;
															rectangle.Y = 18;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 36;
																rectangle.Y = 18;
															}
															else
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
									else
									{
										if (num29 != num && num34 == num && (num31 == num & num32 == num))
										{
											if (num36 == 0)
											{
												rectangle.X = 18;
												rectangle.Y = 0;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 36;
													rectangle.Y = 0;
												}
												else
												{
													rectangle.X = 54;
													rectangle.Y = 0;
												}
											}
										}
										else
										{
											if (num29 == num && num34 != num && (num31 == num & num32 == num))
											{
												if (num36 == 0)
												{
													rectangle.X = 18;
													rectangle.Y = 36;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 36;
														rectangle.Y = 36;
													}
													else
													{
														rectangle.X = 54;
														rectangle.Y = 36;
													}
												}
											}
											else
											{
												if (num29 == num && num34 == num && (num31 != num & num32 == num))
												{
													if (num36 == 0)
													{
														rectangle.X = 0;
														rectangle.Y = 0;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 0;
															rectangle.Y = 18;
														}
														else
														{
															rectangle.X = 0;
															rectangle.Y = 36;
														}
													}
												}
												else
												{
													if (num29 == num && num34 == num && (num31 == num & num32 != num))
													{
														if (num36 == 0)
														{
															rectangle.X = 72;
															rectangle.Y = 0;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 72;
																rectangle.Y = 18;
															}
															else
															{
																rectangle.X = 72;
																rectangle.Y = 36;
															}
														}
													}
													else
													{
														if (num29 != num && num34 == num && (num31 != num & num32 == num))
														{
															if (num36 == 0)
															{
																rectangle.X = 0;
																rectangle.Y = 54;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 36;
																	rectangle.Y = 54;
																}
																else
																{
																	rectangle.X = 72;
																	rectangle.Y = 54;
																}
															}
														}
														else
														{
															if (num29 != num && num34 == num && (num31 == num & num32 != num))
															{
																if (num36 == 0)
																{
																	rectangle.X = 18;
																	rectangle.Y = 54;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 54;
																	}
																	else
																	{
																		rectangle.X = 90;
																		rectangle.Y = 54;
																	}
																}
															}
															else
															{
																if (num29 == num && num34 != num && (num31 != num & num32 == num))
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 0;
																		rectangle.Y = 72;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 72;
																		}
																		else
																		{
																			rectangle.X = 72;
																			rectangle.Y = 72;
																		}
																	}
																}
																else
																{
																	if (num29 == num && num34 != num && (num31 == num & num32 != num))
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 18;
																			rectangle.Y = 72;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 72;
																			}
																			else
																			{
																				rectangle.X = 90;
																				rectangle.Y = 72;
																			}
																		}
																	}
																	else
																	{
																		if (num29 == num && num34 == num && (num31 != num & num32 != num))
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 90;
																				rectangle.Y = 0;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 90;
																					rectangle.Y = 18;
																				}
																				else
																				{
																					rectangle.X = 90;
																					rectangle.Y = 36;
																				}
																			}
																		}
																		else
																		{
																			if (num29 != num && num34 != num && (num31 == num & num32 == num))
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 108;
																					rectangle.Y = 72;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 126;
																						rectangle.Y = 72;
																					}
																					else
																					{
																						rectangle.X = 144;
																						rectangle.Y = 72;
																					}
																				}
																			}
																			else
																			{
																				if (num29 != num && num34 == num && (num31 != num & num32 != num))
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 108;
																						rectangle.Y = 0;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 126;
																							rectangle.Y = 0;
																						}
																						else
																						{
																							rectangle.X = 144;
																							rectangle.Y = 0;
																						}
																					}
																				}
																				else
																				{
																					if (num29 == num && num34 != num && (num31 != num & num32 != num))
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 108;
																							rectangle.Y = 54;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 126;
																								rectangle.Y = 54;
																							}
																							else
																							{
																								rectangle.X = 144;
																								rectangle.Y = 54;
																							}
																						}
																					}
																					else
																					{
																						if (num29 != num && num34 != num && (num31 != num & num32 == num))
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 162;
																								rectangle.Y = 0;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 162;
																									rectangle.Y = 18;
																								}
																								else
																								{
																									rectangle.X = 162;
																									rectangle.Y = 36;
																								}
																							}
																						}
																						else
																						{
																							if (num29 != num && num34 != num && (num31 == num & num32 != num))
																							{
																								if (num36 == 0)
																								{
																									rectangle.X = 216;
																									rectangle.Y = 0;
																								}
																								else
																								{
																									if (num36 == 1)
																									{
																										rectangle.X = 216;
																										rectangle.Y = 18;
																									}
																									else
																									{
																										rectangle.X = 216;
																										rectangle.Y = 36;
																									}
																								}
																							}
																							else
																							{
																								if (num29 != num && num34 != num && (num31 != num & num32 != num))
																								{
																									if (num36 == 0)
																									{
																										rectangle.X = 162;
																										rectangle.Y = 54;
																									}
																									else
																									{
																										if (num36 == 1)
																										{
																											rectangle.X = 180;
																											rectangle.Y = 54;
																										}
																										else
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
								}
								if (rectangle.X <= -1 || rectangle.Y <= -1)
								{
									if (num36 <= 0)
									{
										rectangle.X = 18;
										rectangle.Y = 18;
									}
									else
									{
										if (num36 == 1)
										{
											rectangle.X = 36;
											rectangle.Y = 18;
										}
									}
									if (num36 >= 2)
									{
										rectangle.X = 54;
										rectangle.Y = 18;
									}
								}
								Main.tile.At(i, j).SetFrameX((short)rectangle.X);
								Main.tile.At(i, j).SetFrameY((short)rectangle.Y);
								if (num == 52 || num == 62 || num == 115)
								{
									//if (Main.tile.At(i, j - 1] != null)
									{
										if (!Main.tile.At(i, j - 1).Active)
										{
											num29 = -1;
										}
										else
										{
											num29 = (int)Main.tile.At(i, j - 1).Type;
										}
									}
									//else
									//{
									//    num29 = num;
									//}
									if (num == 52 && (num29 == 109 || num29 == 115))
									{
										Main.tile.At(i, j).SetType(115);
										SquareTileFrame(i, j, true);
										return;
									}
									if (num == 115 && (num29 == 2 || num29 == 52))
									{
										Main.tile.At(i, j).SetType(52);
										SquareTileFrame(i, j, true);
										return;
									}
									if (num29 != num)
									{
										bool flag4 = false;
										if (num29 == -1)
										{
											flag4 = true;
										}
										if (num == 52 && num29 != 2)
										{
											flag4 = true;
										}
										if (num == 62 && num29 != 60)
										{
											flag4 = true;
										}
										if (num == 115 && num29 != 109)
										{
											flag4 = true;
										}
										if (flag4)
										{
											KillTile(i, j);
										}
									}
								}
								if (!noTileActions && (num == 53 || num == 112 || num == 116 || num == 123))
								{
									if (!Main.tile.At(i, j + 1).Active)
									{
										bool flag6 = true;
										if (Main.tile.At(i, j - 1).Active && Main.tile.At(i, j - 1).Type == 21)
										{
											flag6 = false;
										}
										if (flag6)
										{
											int type2 = 31;
											if (num == 59)
											{
												type2 = 39;
											}
											if (num == 57)
											{
												type2 = 40;
											}
											if (num == 112)
											{
												type2 = 56;
											}
											if (num == 116)
											{
												type2 = 67;
											}
											if (num == 123)
											{
												type2 = 71;
											}
											Main.tile.At(i, j).SetActive(false);
											int num39 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 2.5f, type2, 10, 0f, Main.myPlayer);
											Main.projectile[num39].Velocity.Y = 0.5f;
											Main.projectile[num39].Position.Y = Main.projectile[num39].Position.Y + 2f;

											Main.projectile[num39].netUpdate = true;
											NetMessage.SendTileSquare(-1, i, j, 1);
											SquareTileFrame(i, j, true);
										}
									}
								}
								if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
								{
									bool flag7 = mergeUp;
									bool flag8 = mergeDown;
									bool flag9 = mergeLeft;
									bool flag10 = mergeRight;
									TileFrame(i - 1, j, false, false);
									TileFrame(i + 1, j, false, false);
									TileFrame(i, j - 1, false, false);
									TileFrame(i, j + 1, false, false);
									mergeUp = flag7;
									mergeDown = flag8;
									mergeLeft = flag9;
									mergeRight = flag10;
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		// [TODO] 1.1 - here down
		public static int altarCount { get; set; }

		public static int totalEvil = 0;
		public static int totalGood = 0;
		public static int totalSolid = 0;
		public static int totalEvil2 = 0;
		public static int totalGood2 = 0;
		public static int totalSolid2 = 0;
		public static byte tEvil = 0;
		public static byte tGood = 0;

		public static void CountTiles(int X)
		{
			if (X == 0)
			{
				totalEvil = totalEvil2;
				totalSolid = totalSolid2;
				totalGood = totalGood2;
				float num = (float)totalGood / (float)totalSolid;
				num = (float)Math.Round((double)(num * 100f));
				float num2 = (float)totalEvil / (float)totalSolid;
				num2 = (float)Math.Round((double)(num2 * 100f));
				tGood = (byte)num;
				tEvil = (byte)num2;
				NetMessage.SendData(57, -1, -1, "", 0, 0f, 0f, 0f, 0);
			}
			for (int i = 0; i < Main.maxTilesY; i++)
			{
				int num3 = 1;

				if ((double)i <= Main.worldSurface)
					num3 *= 5;

				if (SolidTile(X, i))
				{
					if (Main.tile.At(X, i).Type == 109 || Main.tile.At(X, i).Type == 116 || Main.tile.At(X, i).Type == 117)
						totalGood2 += num3;
					else if (Main.tile.At(X, i).Type == 23 || Main.tile.At(X, i).Type == 25 || Main.tile.At(X, i).Type == 112)
						totalEvil2 += num3;

					totalSolid2 += num3;
				}
			}
		}
	}
}
