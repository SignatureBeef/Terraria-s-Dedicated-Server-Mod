using System;
using Terraria_Server.Plugins;
using Terraria_Server.Misc;
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
		private const int MAX_TILE_SETS = 150;

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
		private static int grassSpread = 0;

		public static object playerEditLock = new object();

		[ThreadStatic]
		internal static Random threadRand;

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

		public static int StorePlayerItem(ISandbox sandbox, int X, int Y, int Width, int Height, int type, int stack = 1, bool noBroadcast = false, int pfix = 0, int NetID = 255)
		{
			if (sandbox != null)
				sandbox.NewItem(X, Y, Width, Height, type, stack, noBroadcast, pfix, NetID);
			else
				return Item.NewItem(X, Y, Width, Height, type, stack, noBroadcast, pfix, NetID);

			return 0;
		}

		public static void UpdateWorld_Hardmode(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (Main.hardMode)
			{
				int type = (int)TileRefs(i, j).Type;
				if (type == 117 && (double)j > Main.rockLayer && Main.rand.Next(110) == 0)
				{
					int num = genRand.Next(4);
					int num2 = 0;
					int num3 = 0;

					if (num == 0)
						num2 = -1;
					else if (num == 1)
						num2 = 1;
					else
						num3 = (num == 0) ? -1 : 1;

					if (!TileRefs(i + num2, j + num3).Active)
					{
						int num4 = 0;
						int num5 = 6;
						for (int k = i - num5; k <= i + num5; k++)
						{
							for (int l = j - num5; l <= j + num5; l++)
							{
								if (TileRefs(k, l).Active && TileRefs(k, l).Type == 129)
									num4++;
							}
						}
						if (num4 < 2)
						{
							PlaceTile(TileRefs, sandbox, i + num2, j + num3, 129, true, false, -1, 0);
							NetMessage.SendTileSquare(-1, i + num2, j + num3, 1);
						}
					}
				}
				if (type == 23 || type == 25 || type == 32 || type == 112)
				{
					bool flag = true;
					while (flag)
					{
						flag = false;
						int num6 = i + genRand.Next(-3, 4);
						int num7 = j + genRand.Next(-3, 4);
						if (TileRefs(num6, num7).Type == 2)
						{
							if (genRand.Next(2) == 0)
								flag = true;

							TileRefs(num6, num7).SetType(23);
							SquareTileFrame(TileRefs, sandbox, num6, num7, true);
							NetMessage.SendTileSquare(-1, num6, num7, 1);
						}
						else if (TileRefs(num6, num7).Type == 1)
						{
							if (genRand.Next(2) == 0)
								flag = true;

							TileRefs(num6, num7).SetType(25);
							SquareTileFrame(TileRefs, sandbox, num6, num7, true);
							NetMessage.SendTileSquare(-1, num6, num7, 1);
						}
						else if (TileRefs(num6, num7).Type == 53)
						{
							if (genRand.Next(2) == 0)
								flag = true;

							TileRefs(num6, num7).SetType(112);
							SquareTileFrame(TileRefs, sandbox, num6, num7, true);
							NetMessage.SendTileSquare(-1, num6, num7, 1);
						}
						else if (TileRefs(num6, num7).Type == 59)
						{
							if (genRand.Next(2) == 0)
								flag = true;

							TileRefs(num6, num7).SetType(0);
							SquareTileFrame(TileRefs, sandbox, num6, num7, true);
							NetMessage.SendTileSquare(-1, num6, num7, 1);
						}
						else if (TileRefs(num6, num7).Type == 60)
						{
							if (genRand.Next(2) == 0)
								flag = true;

							TileRefs(num6, num7).SetType(23);
							SquareTileFrame(TileRefs, sandbox, num6, num7, true);
							NetMessage.SendTileSquare(-1, num6, num7, 1);
						}
						else if (TileRefs(num6, num7).Type == 69)
						{
							if (genRand.Next(2) == 0)
								flag = true;

							TileRefs(num6, num7).SetType(32);
							SquareTileFrame(TileRefs, sandbox, num6, num7, true);
							NetMessage.SendTileSquare(-1, num6, num7, 1);
						}
					}
				}
				if (type == 109 || type == 110 || type == 113 || type == 115 || type == 116 || type == 117 || type == 118)
				{
					bool flag2 = true;
					while (flag2)
					{
						flag2 = false;
						int num8 = i + genRand.Next(-3, 4);
						int num9 = j + genRand.Next(-3, 4);
						if (TileRefs(num8, num9).Type == 2)
						{
							if (genRand.Next(2) == 0)
								flag2 = true;

							TileRefs(num8, num9).SetType(109);
							SquareTileFrame(TileRefs, sandbox, num8, num9, true);
							NetMessage.SendTileSquare(-1, num8, num9, 1);
						}
						else if (TileRefs(num8, num9).Type == 1)
						{
							if (genRand.Next(2) == 0)
								flag2 = true;

							TileRefs(num8, num9).SetType(117);
							SquareTileFrame(TileRefs, sandbox, num8, num9, true);
							NetMessage.SendTileSquare(-1, num8, num9, 1);
						}
						else if (TileRefs(num8, num9).Type == 53)
						{
							if (genRand.Next(2) == 0)
								flag2 = true;

							TileRefs(num8, num9).SetType(116);
							SquareTileFrame(TileRefs, sandbox, num8, num9, true);
							NetMessage.SendTileSquare(-1, num8, num9, 1);
						}
					}
				}
			}
		}

		public static void Check1x1(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if ((!TileRefs(x, y + 1).Active || !Main.tileSolid[(int)TileRefs(x, y + 1).Type]))
				KillTile(TileRefs, sandbox, x, y);
		}

		public static void CheckMan(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			int num = j - (int)(TileRefs(i, j).FrameY / 18);
			int k;
			for (k = (int)TileRefs(i, j).FrameX; k >= 100; k -= 100)
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
					for (n = (int)TileRefs(num3, num4).FrameX; n >= 100; n -= 100)
					{
					}
					if (n >= 36)
						n -= 36;

					if (!TileRefs(num3, num4).Active || TileRefs(num3, num4).Type != 128 ||
						(int)TileRefs(num3, num4).FrameY != m * 18 || n != l * 18)
					{
						flag = true;
					}
				}
			}
			if (!SolidTile(TileRefs, num2, num + 3) || !SolidTile(TileRefs, num2 + 1, num + 3))
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 498, 1, false, 0);
				for (int num5 = 0; num5 <= 1; num5++)
				{
					for (int num6 = 0; num6 <= 2; num6++)
					{
						int num7 = num2 + num5;
						int num8 = num + num6;
						if (TileRefs(num7, num8).Active && TileRefs(num7, num8).Type == 128)
						{
							KillTile(TileRefs, sandbox, num7, num8);
						}
					}
				}
				destroyObject = false;
			}
		}

		public static void CheckMB(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = false;
			int num = 0;
			int k;

			for (k = (int)(TileRefs(i, j).FrameY / 18); k >= 2; k -= 2)
				num++;

			int num2 = (int)(TileRefs(i, j).FrameX / 18);
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
					if (!TileRefs(l, m).Active || (int)TileRefs(l, m).Type != type ||
						(int)TileRefs(l, m).FrameX != (l - num4) * 18 + num3 * 36 ||
						(int)TileRefs(l, m).FrameY != (m - num5) * 18 + num * 36)
					{
						flag = true;
					}
				}
				if (!SolidTile(TileRefs, l, num5 + 2))
					flag = true;
			}

			if (flag)
			{
				destroyObject = true;
				for (int n = num4; n < num4 + 2; n++)
				{
					for (int num6 = num5; num6 < num5 + 3; num6++)
					{
						if ((int)TileRefs(n, num6).Type == type && TileRefs(n, num6).Active)
							KillTile(TileRefs, sandbox, n, num6);
					}
				}
				StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 562 + num, 1, false, 0);
				for (int num7 = num4 - 1; num7 < num4 + 3; num7++)
				{
					for (int num8 = num5 - 1; num8 < num5 + 3; num8++)
						TileFrame(TileRefs, sandbox, num7, num8);
				}
				destroyObject = false;
			}
		}

		public static void CheckOrb(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (!destroyObject)
			{
				int num = i;
				int num2 = j;

				if (TileRefs(i, j).FrameX == 0)
					num = i;
				else
					num = i - 1;

				if (TileRefs(i, j).FrameY == 0)
					num2 = j;
				else
					num2 = j - 1;

				if ((!TileRefs(num, num2).Active || (int)TileRefs(num, num2).Type != type || !TileRefs(num + 1, num2).Active ||
					(int)TileRefs(num + 1, num2).Type != type || !TileRefs(num, num2 + 1).Active ||
					(int)TileRefs(num, num2 + 1).Type != type || !TileRefs(num + 1, num2 + 1).Active ||
					(int)TileRefs(num + 1, num2 + 1).Type != type))
				{
					destroyObject = true;

					if ((int)TileRefs(num, num2).Type == type)
						KillTile(TileRefs, sandbox, num, num2);

					if ((int)TileRefs(num + 1, num2).Type == type)
						KillTile(TileRefs, sandbox, num + 1, num2);

					if ((int)TileRefs(num, num2 + 1).Type == type)
						KillTile(TileRefs, sandbox, num, num2 + 1);

					if ((int)TileRefs(num + 1, num2 + 1).Type == type)
						KillTile(TileRefs, sandbox, num + 1, num2 + 1);

					if (!noTileActions)
					{
						if (type == 12)
						{
							StorePlayerItem(sandbox, num * 16, num2 * 16, 32, 32, 29, 1, false, 0);
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
								StorePlayerItem(sandbox, num * 16, num2 * 16, 32, 32, 96, 1, false, -1);
								int stack = genRand.Next(25, 51);
								StorePlayerItem(sandbox, num * 16, num2 * 16, 32, 32, 97, stack, false, 0);
							}
							else
							{
								if (num3 == 1)
									StorePlayerItem(sandbox, num * 16, num2 * 16, 32, 32, 64, 1, false, -1);
								else if (num3 == 2)
									StorePlayerItem(sandbox, num * 16, num2 * 16, 32, 32, 162, 1, false, -1);
								else if (num3 == 3)
									StorePlayerItem(sandbox, num * 16, num2 * 16, 32, 32, 115, 1, false, -1);
								else if (num3 == 4)
									StorePlayerItem(sandbox, num * 16, num2 * 16, 32, 32, 111, 1, false, -1);
							}
							shadowOrbSmashed = true;
							shadowOrbCount++;
							if (shadowOrbCount >= 3)
							{
								shadowOrbCount = 0;
								float num4 = (float)(num * 16);
								float num5 = (float)(num2 * 16);
								float num6 = -1f;
								int playerId = 0;
								for (int k = 0; k < 255; k++)
								{
									float num8 = Math.Abs(Main.players[k].Position.X - num4) + Math.Abs(Main.players[k].Position.Y - num5);
									if (num8 < num6 || num6 == -1f)
									{
										playerId = 0;
										num6 = num8;
									}
								}
								var player = Main.players[playerId];
								if (player.zoneEvil)
									NPC.SpawnOnPlayer(playerId, (int)NPCType.N13_EATER_OF_WORLDS_HEAD);
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

		public static void CheckTree(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
			int type = (int)TileRefs(i, j).Type;
			int frameX = (int)TileRefs(i, j).FrameX;
			int frameY = (int)TileRefs(i, j).FrameY;
			if (TileRefs(i - 1, j).Active)
			{
				num4 = (int)TileRefs(i - 1, j).Type;
			}
			if (TileRefs(i + 1, j).Active)
			{
				num5 = (int)TileRefs(i + 1, j).Type;
			}
			if (TileRefs(i, j - 1).Active)
			{
				num2 = (int)TileRefs(i, j - 1).Type;
			}
			if (TileRefs(i, j + 1).Active)
			{
				num7 = (int)TileRefs(i, j + 1).Type;
			}
			if (TileRefs(i - 1, j - 1).Active)
			{
				num = (int)TileRefs(i - 1, j - 1).Type;
			}
			if (TileRefs(i + 1, j - 1).Active)
			{
				num3 = (int)TileRefs(i + 1, j - 1).Type;
			}
			if (TileRefs(i - 1, j + 1).Active)
			{
				num6 = (int)TileRefs(i - 1, j + 1).Type;
			}
			if (TileRefs(i + 1, j + 1).Active)
			{
				num8 = (int)TileRefs(i + 1, j + 1).Type;
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
			if (num7 == 147)
			{
				num7 = 2;
			}
			if (TileRefs(i, j).FrameX >= 22 && TileRefs(i, j).FrameX <= 44 && TileRefs(i, j).FrameY >= 132 && TileRefs(i, j).FrameY <= 176)
			{
				if (num7 != 2)
				{
					KillTile(TileRefs, sandbox, i, j, false, false, false);
				}
				else if ((TileRefs(i, j).FrameX != 22 || num4 != type) && (TileRefs(i, j).FrameX != 44 || num5 != type))
				{
					KillTile(TileRefs, sandbox, i, j, false, false, false);
				}
			}
			else
			{
				if ((TileRefs(i, j).FrameX == 88 && TileRefs(i, j).FrameY >= 0 && TileRefs(i, j).FrameY <= 44) || (TileRefs(i, j).FrameX == 66 && TileRefs(i, j).FrameY >= 66 && TileRefs(i, j).FrameY <= 130) || (TileRefs(i, j).FrameX == 110 && TileRefs(i, j).FrameY >= 66 && TileRefs(i, j).FrameY <= 110) || (TileRefs(i, j).FrameX == 132 && TileRefs(i, j).FrameY >= 0 && TileRefs(i, j).FrameY <= 176))
				{
					if (num4 == type && num5 == type)
					{
						if (TileRefs(i, j).FrameNumber == 0)
						{
							TileRefs(i, j).SetFrameX(110);
							TileRefs(i, j).SetFrameY(66);
						}
						if (TileRefs(i, j).FrameNumber == 1)
						{
							TileRefs(i, j).SetFrameX(110);
							TileRefs(i, j).SetFrameY(88);
						}
						if (TileRefs(i, j).FrameNumber == 2)
						{
							TileRefs(i, j).SetFrameX(110);
							TileRefs(i, j).SetFrameY(110);
						}
					}
					else
					{
						if (num4 == type)
						{
							if (TileRefs(i, j).FrameNumber == 0)
							{
								TileRefs(i, j).SetFrameX(88);
								TileRefs(i, j).SetFrameY(0);
							}
							if (TileRefs(i, j).FrameNumber == 1)
							{
								TileRefs(i, j).SetFrameX(88);
								TileRefs(i, j).SetFrameY(22);
							}
							if (TileRefs(i, j).FrameNumber == 2)
							{
								TileRefs(i, j).SetFrameX(88);
								TileRefs(i, j).SetFrameY(44);
							}
						}
						else
						{
							if (num5 == type)
							{
								if (TileRefs(i, j).FrameNumber == 0)
								{
									TileRefs(i, j).SetFrameX(66);
									TileRefs(i, j).SetFrameY(66);
								}
								if (TileRefs(i, j).FrameNumber == 1)
								{
									TileRefs(i, j).SetFrameX(66);
									TileRefs(i, j).SetFrameY(88);
								}
								if (TileRefs(i, j).FrameNumber == 2)
								{
									TileRefs(i, j).SetFrameX(66);
									TileRefs(i, j).SetFrameY(110);
								}
							}
							else
							{
								if (TileRefs(i, j).FrameNumber == 0)
								{
									TileRefs(i, j).SetFrameX(0);
									TileRefs(i, j).SetFrameY(0);
								}
								if (TileRefs(i, j).FrameNumber == 1)
								{
									TileRefs(i, j).SetFrameX(0);
									TileRefs(i, j).SetFrameY(22);
								}
								if (TileRefs(i, j).FrameNumber == 2)
								{
									TileRefs(i, j).SetFrameX(0);
									TileRefs(i, j).SetFrameY(44);
								}
							}
						}
					}
				}
			}
			if (TileRefs(i, j).FrameY >= 132 && TileRefs(i, j).FrameY <= 176 && (TileRefs(i, j).FrameX == 0 || TileRefs(i, j).FrameX == 66 || TileRefs(i, j).FrameX == 88))
			{
				if (num7 != 2)
				{
					KillTile(TileRefs, sandbox, i, j, false, false, false);
				}
				if (num4 != type && num5 != type)
				{
					if (TileRefs(i, j).FrameNumber == 0)
					{
						TileRefs(i, j).SetFrameX(0);
						TileRefs(i, j).SetFrameY(0);
					}
					if (TileRefs(i, j).FrameNumber == 1)
					{
						TileRefs(i, j).SetFrameX(0);
						TileRefs(i, j).SetFrameY(22);
					}
					if (TileRefs(i, j).FrameNumber == 2)
					{
						TileRefs(i, j).SetFrameX(0);
						TileRefs(i, j).SetFrameY(44);
					}
				}
				else
				{
					if (num4 != type)
					{
						if (TileRefs(i, j).FrameNumber == 0)
						{
							TileRefs(i, j).SetFrameX(0);
							TileRefs(i, j).SetFrameY(132);
						}
						if (TileRefs(i, j).FrameNumber == 1)
						{
							TileRefs(i, j).SetFrameX(0);
							TileRefs(i, j).SetFrameY(154);
						}
						if (TileRefs(i, j).FrameNumber == 2)
						{
							TileRefs(i, j).SetFrameX(0);
							TileRefs(i, j).SetFrameY(176);
						}
					}
					else
					{
						if (num5 != type)
						{
							if (TileRefs(i, j).FrameNumber == 0)
							{
								TileRefs(i, j).SetFrameX(66);
								TileRefs(i, j).SetFrameY(132);
							}
							if (TileRefs(i, j).FrameNumber == 1)
							{
								TileRefs(i, j).SetFrameX(66);
								TileRefs(i, j).SetFrameY(154);
							}
							if (TileRefs(i, j).FrameNumber == 2)
							{
								TileRefs(i, j).SetFrameX(66);
								TileRefs(i, j).SetFrameY(176);
							}
						}
						else
						{
							if (TileRefs(i, j).FrameNumber == 0)
							{
								TileRefs(i, j).SetFrameX(88);
								TileRefs(i, j).SetFrameY(132);
							}
							if (TileRefs(i, j).FrameNumber == 1)
							{
								TileRefs(i, j).SetFrameX(88);
								TileRefs(i, j).SetFrameY(154);
							}
							if (TileRefs(i, j).FrameNumber == 2)
							{
								TileRefs(i, j).SetFrameX(88);
								TileRefs(i, j).SetFrameY(176);
							}
						}
					}
				}
			}
			if ((TileRefs(i, j).FrameX == 66 && (TileRefs(i, j).FrameY == 0 || TileRefs(i, j).FrameY == 22 || TileRefs(i, j).FrameY == 44)) || (TileRefs(i, j).FrameX == 44 && (TileRefs(i, j).FrameY == 198 || TileRefs(i, j).FrameY == 220 || TileRefs(i, j).FrameY == 242)))
			{
				if (num5 != type)
				{
					KillTile(TileRefs, sandbox, i, j, false, false, false);
				}
			}
			else
			{
				if ((TileRefs(i, j).FrameX == 88 && (TileRefs(i, j).FrameY == 66 || TileRefs(i, j).FrameY == 88 || TileRefs(i, j).FrameY == 110)) || (TileRefs(i, j).FrameX == 66 && (TileRefs(i, j).FrameY == 198 || TileRefs(i, j).FrameY == 220 || TileRefs(i, j).FrameY == 242)))
				{
					if (num4 != type)
					{
						KillTile(TileRefs, sandbox, i, j, false, false, false);
					}
				}
				else
				{
					if (num7 == -1 || num7 == 23)
					{
						KillTile(TileRefs, sandbox, i, j, false, false, false);
					}
					else
					{
						if (num2 != type && TileRefs(i, j).FrameY < 198 && ((TileRefs(i, j).FrameX != 22 && TileRefs(i, j).FrameX != 44) || TileRefs(i, j).FrameY < 132))
						{
							if (num4 == type || num5 == type)
							{
								if (num7 == type)
								{
									if (num4 == type && num5 == type)
									{
										if (TileRefs(i, j).FrameNumber == 0)
										{
											TileRefs(i, j).SetFrameX(132);
											TileRefs(i, j).SetFrameY(132);
										}
										if (TileRefs(i, j).FrameNumber == 1)
										{
											TileRefs(i, j).SetFrameX(132);
											TileRefs(i, j).SetFrameY(154);
										}
										if (TileRefs(i, j).FrameNumber == 2)
										{
											TileRefs(i, j).SetFrameX(132);
											TileRefs(i, j).SetFrameY(176);
										}
									}
									else
									{
										if (num4 == type)
										{
											if (TileRefs(i, j).FrameNumber == 0)
											{
												TileRefs(i, j).SetFrameX(132);
												TileRefs(i, j).SetFrameY(0);
											}
											if (TileRefs(i, j).FrameNumber == 1)
											{
												TileRefs(i, j).SetFrameX(132);
												TileRefs(i, j).SetFrameY(22);
											}
											if (TileRefs(i, j).FrameNumber == 2)
											{
												TileRefs(i, j).SetFrameX(132);
												TileRefs(i, j).SetFrameY(44);
											}
										}
										else
										{
											if (num5 == type)
											{
												if (TileRefs(i, j).FrameNumber == 0)
												{
													TileRefs(i, j).SetFrameX(132);
													TileRefs(i, j).SetFrameY(66);
												}
												if (TileRefs(i, j).FrameNumber == 1)
												{
													TileRefs(i, j).SetFrameX(132);
													TileRefs(i, j).SetFrameY(88);
												}
												if (TileRefs(i, j).FrameNumber == 2)
												{
													TileRefs(i, j).SetFrameX(132);
													TileRefs(i, j).SetFrameY(110);
												}
											}
										}
									}
								}
								else
								{
									if (num4 == type && num5 == type)
									{
										if (TileRefs(i, j).FrameNumber == 0)
										{
											TileRefs(i, j).SetFrameX(154);
											TileRefs(i, j).SetFrameY(132);
										}
										if (TileRefs(i, j).FrameNumber == 1)
										{
											TileRefs(i, j).SetFrameX(154);
											TileRefs(i, j).SetFrameY(154);
										}
										if (TileRefs(i, j).FrameNumber == 2)
										{
											TileRefs(i, j).SetFrameX(154);
											TileRefs(i, j).SetFrameY(176);
										}
									}
									else
									{
										if (num4 == type)
										{
											if (TileRefs(i, j).FrameNumber == 0)
											{
												TileRefs(i, j).SetFrameX(154);
												TileRefs(i, j).SetFrameY(0);
											}
											if (TileRefs(i, j).FrameNumber == 1)
											{
												TileRefs(i, j).SetFrameX(154);
												TileRefs(i, j).SetFrameY(22);
											}
											if (TileRefs(i, j).FrameNumber == 2)
											{
												TileRefs(i, j).SetFrameX(154);
												TileRefs(i, j).SetFrameY(44);
											}
										}
										else
										{
											if (num5 == type)
											{
												if (TileRefs(i, j).FrameNumber == 0)
												{
													TileRefs(i, j).SetFrameX(154);
													TileRefs(i, j).SetFrameY(66);
												}
												if (TileRefs(i, j).FrameNumber == 1)
												{
													TileRefs(i, j).SetFrameX(154);
													TileRefs(i, j).SetFrameY(88);
												}
												if (TileRefs(i, j).FrameNumber == 2)
												{
													TileRefs(i, j).SetFrameX(154);
													TileRefs(i, j).SetFrameY(110);
												}
											}
										}
									}
								}
							}
							else
							{
								if (TileRefs(i, j).FrameNumber == 0)
								{
									TileRefs(i, j).SetFrameX(110);
									TileRefs(i, j).SetFrameY(0);
								}
								if (TileRefs(i, j).FrameNumber == 1)
								{
									TileRefs(i, j).SetFrameX(110);
									TileRefs(i, j).SetFrameY(22);
								}
								if (TileRefs(i, j).FrameNumber == 2)
								{
									TileRefs(i, j).SetFrameX(110);
									TileRefs(i, j).SetFrameY(44);
								}
							}
						}
					}
				}
			}
			if ((int)TileRefs(i, j).FrameX != frameX && (int)TileRefs(i, j).FrameY != frameY && frameX >= 0 && frameY >= 0)
			{
				TileFrame(TileRefs, sandbox, i - 1, j, false, false);
				TileFrame(TileRefs, sandbox, i + 1, j, false, false);
				TileFrame(TileRefs, sandbox, i, j - 1, false, false);
				TileFrame(TileRefs, sandbox, i, j + 1, false, false);
			}
		}

		public static void Place1x1(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type, int style = 0)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (SolidTile(TileRefs, x, y + 1) && !TileRefs(x, y).Active)
			{
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetType((byte)type);
				if (type == 144)
				{
					TileRefs(x, y).SetFrameX((short)(style * 18));
					TileRefs(x, y).SetFrameY(0);
					return;
				}
				TileRefs(x, y).SetFrameY((short)(style * 18));
			}
		}

		public static void PlaceMan(Func<Int32, Int32, ITile> TileRefs, int i, int j, int dir)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			for (int k = i; k <= i + 1; k++)
			{
				for (int l = j - 2; l <= j; l++)
				{
					if (TileRefs(k, l).Active)
						return;
				}
			}
			if (!SolidTile(TileRefs, i, j + 1) || !SolidTile(TileRefs, i + 1, j + 1))
				return;

			byte b = 0;
			if (dir == 1)
				b = 36;

			TileRefs(i, j - 2).SetActive(true);
			TileRefs(i, j - 2).SetFrameY(0);
			TileRefs(i, j - 2).SetFrameX((short)b);
			TileRefs(i, j - 2).SetType(128);
			TileRefs(i, j - 1).SetActive(true);
			TileRefs(i, j - 1).SetFrameY(18);
			TileRefs(i, j - 1).SetFrameX((short)b);
			TileRefs(i, j - 1).SetType(128);
			TileRefs(i, j).SetActive(true);
			TileRefs(i, j).SetFrameY(36);
			TileRefs(i, j).SetFrameX((short)b);
			TileRefs(i, j).SetType(128);
			TileRefs(i + 1, j - 2).SetActive(true);
			TileRefs(i + 1, j - 2).SetFrameY(0);
			TileRefs(i + 1, j - 2).SetFrameX((short)(18 + b));
			TileRefs(i + 1, j - 2).SetType(128);
			TileRefs(i + 1, j - 1).SetActive(true);
			TileRefs(i + 1, j - 1).SetFrameY(18);
			TileRefs(i + 1, j - 1).SetFrameX((short)(18 + b));
			TileRefs(i + 1, j - 1).SetType(128);
			TileRefs(i + 1, j).SetActive(true);
			TileRefs(i + 1, j).SetFrameY(36);
			TileRefs(i + 1, j).SetFrameX((short)(18 + b));
			TileRefs(i + 1, j).SetType(128);
		}

		public static void PlaceMB(Func<Int32, Int32, ITile> TileRefs, int X, int y, int type, int style)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
					if (TileRefs(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileRefs(i, y + 1).Active || (!Main.tileSolid[(int)TileRefs(i, y + 1).Type] &&
					!Main.tileTable[(int)TileRefs(i, y + 1).Type]))
				{
					flag = false;
				}
			}
			if (flag)
			{
				TileRefs(num - 1, y - 1).SetActive(true);
				TileRefs(num - 1, y - 1).SetFrameY((short)(style * 36));
				TileRefs(num - 1, y - 1).SetFrameX(0);
				TileRefs(num - 1, y - 1).SetType((byte)type);
				TileRefs(num, y - 1).SetActive(true);
				TileRefs(num, y - 1).SetFrameY((short)(style * 36));
				TileRefs(num, y - 1).SetFrameX(18);
				TileRefs(num, y - 1).SetType((byte)type);
				TileRefs(num - 1, y).SetActive(true);
				TileRefs(num - 1, y).SetFrameY((short)(style * 36 + 18));
				TileRefs(num - 1, y).SetFrameX(0);
				TileRefs(num - 1, y).SetType((byte)type);
				TileRefs(num, y).SetActive(true);
				TileRefs(num, y).SetFrameY((short)(style * 36 + 18));
				TileRefs(num, y).SetFrameX(18);
				TileRefs(num, y).SetType((byte)type);
			}
		}

		public static bool placeTrap(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x2, int y2, int type = -1)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = y2;
			while (!SolidTile(TileRefs, x2, num))
			{
				num++;
				if (num >= Main.maxTilesY - 300)
				{
					return false;
				}
			}
			num--;
			if (TileRefs(x2, num).Liquid > 0 && TileRefs(x2, num).Lava)
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
			if (TileRefs(x2, num).Active || TileRefs(x2 - 1, num).Active || TileRefs(x2 + 1, num).Active || TileRefs(x2, num - 1).Active || TileRefs(x2 - 1, num - 1).Active || TileRefs(x2 + 1, num - 1).Active || TileRefs(x2, num - 2).Active || TileRefs(x2 - 1, num - 2).Active || TileRefs(x2 + 1, num - 2).Active)
			{
				return false;
			}
			if (TileRefs(x2, num + 1).Type == 48)
			{
				return false;
			}
			if (type == 0)
			{
				int num2 = x2;
				int num3 = num;
				num3 -= genRand.Next(3);
				while (!SolidTile(TileRefs, num2, num3))
				{
					num2--;
				}
				int num4 = num2;
				num2 = x2;
				while (!SolidTile(TileRefs, num2, num3))
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
				if (flag && !SolidTile(TileRefs, num4, num3 + 1))
				{
					flag = false;
				}
				if (flag2 && !SolidTile(TileRefs, num5, num3 + 1))
				{
					flag2 = false;
				}
				if (flag && (TileRefs(num4, num3).Type == 10 || TileRefs(num4, num3).Type == 48 || TileRefs(num4, num3 + 1).Type == 10 || TileRefs(num4, num3 + 1).Type == 48))
				{
					flag = false;
				}
				if (flag2 && (TileRefs(num5, num3).Type == 10 || TileRefs(num5, num3).Type == 48 || TileRefs(num5, num3 + 1).Type == 10 || TileRefs(num5, num3 + 1).Type == 48))
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
				if (TileRefs(x2, num).Wall > 0)
				{
					PlaceTile(TileRefs, sandbox, x2, num, 135, true, true, -1, 2);
				}
				else
				{
					PlaceTile(TileRefs, sandbox, x2, num, 135, true, true, -1, genRand.Next(2, 4));
				}
				KillTile(TileRefs, sandbox, num2, num3);
				PlaceTile(TileRefs, sandbox, num2, num3, 137, true, true, -1, style);
				int num8 = x2;
				int num9 = num;
				while (num8 != num2 || num9 != num3)
				{
					TileRefs(num8, num9).SetWire(true);
					if (num8 > num2)
					{
						num8--;
					}
					if (num8 < num2)
					{
						num8++;
					}
					TileRefs(num8, num9).SetWire(true);
					if (num9 > num3)
					{
						num9--;
					}
					if (num9 < num3)
					{
						num9++;
					}
					TileRefs(num8, num9).SetWire(true);
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
						if (!SolidTile(TileRefs, num11, num12))
						{
							return false;
						}
					}
					for (int j = num11 - 2; j <= num11 + 2; j++)
					{
						for (int k = num12 - 2; k <= num12 + 2; k++)
						{
							if (!SolidTile(TileRefs, j, k))
							{
								return false;
							}
						}
					}
					KillTile(TileRefs, sandbox, num11, num12);
					TileRefs(num11, num12).SetActive(true);
					TileRefs(num11, num12).SetType(141);
					TileRefs(num11, num12).SetFrameX(0);
					TileRefs(num11, num12).SetFrameY((short)(18 * Main.rand.Next(2)));
					PlaceTile(TileRefs, sandbox, x2, num, 135, true, true, -1, genRand.Next(2, 4));
					int num13 = x2;
					int num14 = num;
					while (num13 != num11 || num14 != num12)
					{
						TileRefs(num13, num14).SetWire(true);
						if (num13 > num11)
						{
							num13--;
						}
						if (num13 < num11)
						{
							num13++;
						}
						TileRefs(num13, num14).SetWire(true);
						if (num14 > num12)
						{
							num14--;
						}
						if (num14 < num12)
						{
							num14++;
						}
						TileRefs(num13, num14).SetWire(true);
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
						if (!SolidTile(TileRefs, l, m))
						{
							flag4 = false;
						}
						if (TileRefs(l, m).Active && (TileRefs(l, m).Type == 0 || TileRefs(l, m).Type == 1 || TileRefs(l, m).Type == 59))
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
					if (SolidTile(TileRefs, n, num18))
					{
						KillTile(TileRefs, sandbox, n, num18);
					}
				}
			}
			for (int num19 = num16 - 2; num19 <= num16 + 3; num19++)
			{
				for (int num20 = num15 - 2; num20 <= num15 + 3; num20++)
				{
					if (SolidTile(TileRefs, num19, num20))
					{
						TileRefs(num19, num20).SetType(1);
					}
				}
			}
			PlaceTile(TileRefs, sandbox, x2, num, 135, true, true, -1, genRand.Next(2, 4));
			PlaceTile(TileRefs, sandbox, num16, num15 + 2, 130, true, false, -1, 0);
			PlaceTile(TileRefs, sandbox, num16 + 1, num15 + 2, 130, true, false, -1, 0);
			PlaceTile(TileRefs, sandbox, num16 + 1, num15 + 1, 138, true, false, -1, 0);
			num15 += 2;
			TileRefs(num16, num15).SetWire(true);
			TileRefs(num16 + 1, num15).SetWire(true);
			num15++;
			PlaceTile(TileRefs, sandbox, num16, num15, 130, true, false, -1, 0);
			PlaceTile(TileRefs, sandbox, num16 + 1, num15, 130, true, false, -1, 0);
			TileRefs(num16, num15).SetWire(true);
			TileRefs(num16 + 1, num15).SetWire(true);
			PlaceTile(TileRefs, sandbox, num16, num15 + 1, 130, true, false, -1, 0);
			PlaceTile(TileRefs, sandbox, num16 + 1, num15 + 1, 130, true, false, -1, 0);
			TileRefs(num16, num15 + 1).SetWire(true);
			TileRefs(num16 + 1, num15 + 1).SetWire(true);
			int num21 = x2;
			int num22 = num;
			while (num21 != num16 || num22 != num15)
			{
				TileRefs(num21, num22).SetWire(true);
				if (num21 > num16)
				{
					num21--;
				}
				if (num21 < num16)
				{
					num21++;
				}
				TileRefs(num21, num22).SetWire(true);
				if (num22 > num15)
				{
					num22--;
				}
				if (num22 < num15)
				{
					num22++;
				}
				TileRefs(num21, num22).SetWire(true);
			}
			return true;
		}

		public static void SwitchMB(Func<Int32, Int32, ITile> TileRefs, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int k;
			for (k = (int)(TileRefs(i, j).FrameY / 18); k >= 2; k -= 2)
			{
			}
			int num = (int)(TileRefs(i, j).FrameX / 18);
			if (num >= 2)
				num -= 2;

			int num2 = i - num;
			int num3 = j - k;
			for (int l = num2; l < num2 + 2; l++)
			{
				for (int m = num3; m < num3 + 2; m++)
				{
					if (TileRefs(l, m).Active && TileRefs(l, m).Type == 139)
					{
						if (TileRefs(l, m).FrameX < 36)
						{
							short frameX = (short)(TileRefs(l, m).FrameX + 36);
							TileRefs(l, m).SetFrameX(frameX);
						}
						else
						{
							short frameX = (short)(TileRefs(l, m).FrameX - 36);
							TileRefs(l, m).SetFrameX(frameX);
						}
						noWireX[numNoWire] = l;
						noWireY[numNoWire] = m;
						numNoWire++;
					}
				}
			}
			NetMessage.SendTileSquare(-1, num2, num3, 3);
		}

		public static void UpdateMech(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, ISender Sender)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			for (int i = numMechs - 1; i >= 0; i--)
			{
				mechTime[i]--;
				if (TileRefs(mechX[i], mechY[i]).Active && TileRefs(mechX[i], mechY[i]).Type == 144)
				{
					if (TileRefs(mechX[i], mechY[i]).FrameY == 0)
					{
						mechTime[i] = 0;
					}
					else
					{
						int num = (int)(TileRefs(mechX[i], mechY[i]).FrameX / 18);
						if (num == 0)
							num = 60;
						else if (num == 1)
							num = 180;
						else if (num == 2)
							num = 300;

						if (Math.IEEERemainder((double)mechTime[i], (double)num) == 0.0)
						{
							mechTime[i] = 18000;
							TripWire(TileRefs, sandbox, mechX[i], mechY[i], Sender);
						}
					}
				}
				if (mechTime[i] <= 0)
				{
					if (TileRefs(mechX[i], mechY[i]).Active && TileRefs(mechX[i], mechY[i]).Type == 144)
					{
						TileRefs(mechX[i], mechY[i]).SetFrameY(0);
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

		public static void hitSwitch(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, ISender Sender)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (TileRefs(i, j).Type == 135)
			{
				TripWire(TileRefs, sandbox, i, j, Sender);
				return;
			}
			if (TileRefs(i, j).Type == 136)
			{
				if (TileRefs(i, j).FrameY == 0)
					TileRefs(i, j).SetFrameY(8);
				else
					TileRefs(i, j).SetFrameY(0);
				TripWire(TileRefs, sandbox, i, j, Sender);
				return;
			}
			if (TileRefs(i, j).Type == 144)
			{
				if (TileRefs(i, j).FrameY == 0)
				{
					TileRefs(i, j).SetFrameY(18);
					checkMech(i, j, 18000);
				}
				else
					TileRefs(i, j).SetFrameY(0);

				return;
			}
			if (TileRefs(i, j).Type == 132)
			{
				int num = i;
				short num2 = 36;
				num = (int)(TileRefs(i, j).FrameX / 18 * -1);
				int num3 = (int)(TileRefs(i, j).FrameY / 18 * -1);
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
						if (TileRefs(k, l).Type == 132)
						{
							short frameX = (short)(TileRefs(k, l).FrameX + num2);
							TileRefs(k, l).SetFrameX(frameX);
						}
					}
				}
				TileFrame(TileRefs, sandbox, num, num3, false, false);
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num3; n < num3 + 2; n++)
					{
						var tile = TileRefs(m, n);
						if (tile.Type == 132 && tile.Active && tile.Wire)
						{
							TripWire(TileRefs, sandbox, m, n, Sender);
							return;
						}
					}
				}
			}
		}

		public static void TripWire(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, ISender Sender)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			numWire = 0;
			numNoWire = 0;
			numInPump = 0;
			numOutPump = 0;
			noWire(i, j);
			hitWire(TileRefs, sandbox, i, j, Sender);

			if (numInPump > 0 && numOutPump > 0)
				xferWater(TileRefs, sandbox);
		}

		public static void noWire(int i, int j)
		{
			if (numNoWire >= MAX_WIRE - 1)
				return;

			noWireX[numNoWire] = i;
			noWireY[numNoWire] = j;
			numNoWire++;
		}

		public static void xferWater(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			for (int i = 0; i < numInPump; i++)
			{
				int num = inPumpX[i];
				int num2 = inPumpY[i];
				int liquid = (int)TileRefs(num, num2).Liquid;

				if (liquid > 0)
				{
					bool lava = TileRefs(num, num2).Lava;
					for (int j = 0; j < numOutPump; j++)
					{
						int num3 = outPumpX[j];
						int num4 = outPumpY[j];
						int liquid2 = (int)TileRefs(num3, num4).Liquid;
						if (liquid2 < 255)
						{
							bool flag = TileRefs(num3, num4).Lava;
							if (liquid2 == 0)
							{
								flag = lava;
							}
							if (lava == flag)
							{
								int num5 = liquid;
								if (num5 + liquid2 > 255)
								{
									num5 = 255 - liquid2;
								}

								TileRefs(num3, num4).AddLiquid((byte)num5);
								TileRefs(num, num2).AddLiquid(-(byte)num5);

								liquid = (int)TileRefs(num, num2).Liquid;
								TileRefs(num3, num4).SetLava(lava);
								SquareTileFrame(TileRefs, sandbox, num3, num4, true);

								if (TileRefs(num, num2).Liquid == 0)
								{
									TileRefs(num, num2).SetLava(false);
									SquareTileFrame(TileRefs, sandbox, num, num2, true);
									break;
								}
							}
						}
					}
					SquareTileFrame(TileRefs, sandbox, num, num2, true);
				}
			}
		}

		public static void hitWire(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, ISender Sender)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (numWire >= MAX_WIRE - 1)
			{
				return;
			}
			if (!TileRefs(i, j).Wire)
			{
				return;
			}
			for (int k = 0; k < numWire; k++)
			{
				if (wireX[k] == i && wireY[k] == j)
				{
					return;
				}
			}
			wireX[numWire] = i;
			wireY[numWire] = j;
			numWire++;
			int type = (int)TileRefs(i, j).Type;
			bool flag = true;
			for (int l = 0; l < numNoWire; l++)
			{
				if (noWireX[l] == i && noWireY[l] == j)
				{
					flag = false;
				}
			}
			if (flag && TileRefs(i, j).Active)
			{
				if (type == 144)
				{
					hitSwitch(TileRefs, sandbox, i, j, Sender);
					SquareTileFrame(TileRefs, sandbox, i, j, true);
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
				else if (type == 130)
				{
					TileRefs(i, j).SetType(131);
					SquareTileFrame(TileRefs, sandbox, i, j, true);
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
				else if (type == 131)
				{
					TileRefs(i, j).SetType(130);
					SquareTileFrame(TileRefs, sandbox, i, j, true);
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
				else if (type == 11)
				{
					if (CloseDoor(TileRefs, sandbox, i, j, true, Sender))
					{
						NetMessage.SendData(19, -1, -1, "", 1, (float)i, (float)j, 0f, 0);
					}
				}
				else if (type == 10)
				{
					int num = 1;
					if (Main.rand.Next(2) == 0)
					{
						num = -1;
					}
					if (!OpenDoor(TileRefs, sandbox, i, j, num, Sender))
					{
						if (OpenDoor(TileRefs, sandbox, i, j, -num, Sender))
						{
							NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)(-(float)num), 0);
						}
					}
					else
					{
						NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)num, 0);
					}
				}
				else if (type == 4)
				{
					if (TileRefs(i, j).FrameX < 66)
					{
						TileRefs(i, j).AddFrameX(66);
					}
					else
					{
						TileRefs(i, j).AddFrameX(-66);
					}
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
				else if (type == 149)
				{
					if (TileRefs(i, j).FrameX < 54)
					{
						TileRefs(i, j).AddFrameX(54);
					}
					else
					{
						TileRefs(i, j).AddFrameX(-54);
					}
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
				else if (type == 42)
				{
					int num2 = j - (int)(TileRefs(i, j).FrameY / 18);
					short num3 = 18;
					if (TileRefs(i, j).FrameX > 0)
					{
						num3 = -18;
					}

					TileRefs(i, num2).AddFrameX(num3);
					TileRefs(i, num2 + 1).AddFrameX(num3);

					noWire(i, num2);
					noWire(i, num2 + 1);
					NetMessage.SendTileSquare(-1, i, j, 2);
				}
				else if (type == 93)
				{
					int num4 = j - (int)(TileRefs(i, j).FrameY / 18);
					short num5 = 18;
					if (TileRefs(i, j).FrameX > 0)
					{
						num5 = -18;
					}

					TileRefs(i, num4).AddFrameX(num5);
					TileRefs(i, num4 + 1).AddFrameX(num5);
					TileRefs(i, num4 + 2).AddFrameX(num5);

					noWire(i, num4);
					noWire(i, num4 + 1);
					noWire(i, num4 + 2);
					NetMessage.SendTileSquare(-1, i, num4 + 1, 3);
				}
				else if (type == 126 || type == 100 || type == 95)
				{
					int num6 = j - (int)(TileRefs(i, j).FrameY / 18);
					int num7 = (int)(TileRefs(i, j).FrameX / 18);
					if (num7 > 1)
					{
						num7 -= 2;
					}
					num7 = i - num7;
					short num8 = 36;
					if (TileRefs(num7, num6).FrameX > 0)
					{
						num8 = -36;
					}

					TileRefs(num7, num6).AddFrameX(num8);
					TileRefs(num7, num6 + 1).AddFrameX(num8);
					TileRefs(num7 + 1, num6).AddFrameX(num8);
					TileRefs(num7 + 1, num6 + 1).AddFrameX(num8);

					noWire(num7, num6);
					noWire(num7, num6 + 1);
					noWire(num7 + 1, num6);
					noWire(num7 + 1, num6 + 1);
					NetMessage.SendTileSquare(-1, num7, num6, 3);
				}
				else if (type == 34 || type == 35 || type == 36)
				{
					int num9 = j - (int)(TileRefs(i, j).FrameY / 18);
					int num10 = (int)(TileRefs(i, j).FrameX / 18);
					if (num10 > 2)
					{
						num10 -= 3;
					}
					num10 = i - num10;
					short num11 = 54;
					if (TileRefs(num10, num9).FrameX > 0)
					{
						num11 = -54;
					}
					for (int m = num10; m < num10 + 3; m++)
					{
						for (int n = num9; n < num9 + 3; n++)
						{
							TileRefs(m, n).AddFrameX(num11);
							noWire(m, n);
						}
					}
					NetMessage.SendTileSquare(-1, num10 + 1, num9 + 1, 3);
				}
				else if (type == 33)
				{
					short num12 = 18;
					if (TileRefs(i, j).FrameX > 0)
					{
						num12 = -18;
					}
					TileRefs(i, j).AddFrameX(num12);
					NetMessage.SendTileSquare(-1, i, j, 3);
				}
				else if (type == 92)
				{
					int num13 = j - (int)(TileRefs(i, j).FrameY / 18);
					short num14 = 18;
					if (TileRefs(i, j).FrameX > 0)
					{
						num14 = -18;
					}

					TileRefs(i, num13).AddFrameX(num14);
					TileRefs(i, num13 + 1).AddFrameX(num14);
					TileRefs(i, num13 + 2).AddFrameX(num14);
					TileRefs(i, num13 + 3).AddFrameX(num14);
					TileRefs(i, num13 + 4).AddFrameX(num14);
					TileRefs(i, num13 + 5).AddFrameX(num14);

					noWire(i, num13);
					noWire(i, num13 + 1);
					noWire(i, num13 + 2);
					noWire(i, num13 + 3);
					noWire(i, num13 + 4);
					noWire(i, num13 + 5);
					NetMessage.SendTileSquare(-1, i, num13 + 3, 7);
				}
				else if (type == 137)
				{
					if (checkMech(i, j, 180))
					{
						int num15 = -1;
						if (TileRefs(i, j).FrameX != 0)
						{
							num15 = 1;
						}
						float speedX = (float)(12 * num15);
						int damage = 20;
						int type2 = 98;
						Vector2 vector = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 7));
						vector.X += (float)(10 * num15);
						vector.Y += 2f;
						Projectile.NewProjectile((float)((int)vector.X), (float)((int)vector.Y), speedX, 0f, type2, damage, 2f, Main.myPlayer);
					}
				}
				else if (type == 139)
				{
					SwitchMB(TileRefs, i, j);
				}
				else if (type == 141)
				{
					KillTile(TileRefs, sandbox, i, j, false, false, true);
					NetMessage.SendTileSquare(-1, i, j, 1);
					Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0f, 108, 250, 10f, Main.myPlayer);
				}
				else if (type == 142 || type == 143)
				{
					int num16 = j - (int)(TileRefs(i, j).FrameY / 18);
					int num17 = (int)(TileRefs(i, j).FrameX / 18);
					if (num17 > 1)
					{
						num17 -= 2;
					}
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
							{
								break;
							}
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
				else if (type == 105)
				{
					int num24 = j - (int)(TileRefs(i, j).FrameY / 18);
					int num25 = (int)(TileRefs(i, j).FrameX / 18);
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
					if (num26 == 4)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 1))
						{
							num29 = NPC.NewNPC(num27, num28 - 12, 1, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 7)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 49))
						{
							num29 = NPC.NewNPC(num27 - 4, num28 - 6, 49, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 8)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 55))
						{
							num29 = NPC.NewNPC(num27, num28 - 12, 55, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 9)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 46))
						{
							num29 = NPC.NewNPC(num27, num28 - 12, 46, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 10)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 21))
						{
							num29 = NPC.NewNPC(num27, num28, 21, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 18)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 67))
						{
							num29 = NPC.NewNPC(num27, num28 - 12, 67, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 23)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 63))
						{
							num29 = NPC.NewNPC(num27, num28 - 12, 63, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 27)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 85))
						{
							num29 = NPC.NewNPC(num27 - 9, num28, 85, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 28)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 74))
						{
							num29 = NPC.NewNPC(num27, num28 - 12, 74, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 42)
					{
						if (checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 58))
						{
							num29 = NPC.NewNPC(num27, num28 - 12, 58, 0, Main.SpawnsOverride);
						}
					}
					else if (num26 == 37)
					{
						if (checkMech(i, j, 600) && Item.MechSpawn((float)num27, (float)num28, 58))
						{
							StorePlayerItem(sandbox, num27, num28 - 16, 0, 0, 58, 1, false, 0);
						}
					}
					else if (num26 == 2)
					{
						if (checkMech(i, j, 600) && Item.MechSpawn((float)num27, (float)num28, 184))
						{
							StorePlayerItem(sandbox, num27, num28 - 16, 0, 0, 184, 1, false, 0);
						}
					}
					else if (num26 == 17)
					{
						if (checkMech(i, j, 600) && Item.MechSpawn((float)num27, (float)num28, 166))
						{
							StorePlayerItem(sandbox, num27, num28 - 20, 0, 0, 166, 1, false, 0);
						}
					}
					else if (num26 == 40)
					{
						if (checkMech(i, j, 300))
						{
							int[] array = new int[10];
							int num30 = 0;
							for (int num31 = 0; num31 < 200; num31++)
							{
								if (Main.npcs[num31].Active && (Main.npcs[num31].Type == 17 || Main.npcs[num31].Type == 19 ||
									Main.npcs[num31].Type == 22 || Main.npcs[num31].Type == 38 || Main.npcs[num31].Type == 54 ||
										Main.npcs[num31].Type == 107 || Main.npcs[num31].Type == 108))
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
					}
					else if (num26 == 41 && checkMech(i, j, 300))
					{
						int[] array2 = new int[10];
						int num33 = 0;
						for (int num34 = 0; num34 < 200; num34++)
						{
							if (Main.npcs[num34].Active && (Main.npcs[num34].Type == 18 || Main.npcs[num34].Type == 20 ||
								Main.npcs[num34].Type == 124))
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
					if (num29 >= 0)
					{
						Main.npcs[num29].value = 0f;
						Main.npcs[num29].slots = 0f;
					}
				}
			}
			hitWire(TileRefs, sandbox, i - 1, j, Sender);
			hitWire(TileRefs, sandbox, i + 1, j, Sender);
			hitWire(TileRefs, sandbox, i, j - 1, Sender);
			hitWire(TileRefs, sandbox, i, j + 1, Sender);
		}

		//Client code i think, But might still work if a Plugin uses it.
		public static NPCMove_Result MoveNPC(Func<Int32, Int32, ITile> TileRefs, int x, int y, int n)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (!StartRoomCheck(TileRefs, x, y))
				return NPCMove_Result.NOT_VALID;

			if (!RoomNeeds())
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

			ScoreRoom(TileRefs, -1);

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

		public static void MoveRoom(Func<Int32, Int32, ITile> TileRefs, int x, int y, int NPCIndex)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			spawnNPC = Main.npcs[NPCIndex].Type;
			Main.npcs[NPCIndex].homeless = true;
			SpawnNPC(TileRefs, x, y);
		}

		public static bool RoomNeeds()
		{
			roomChair = houseTile[15] || houseTile[79] || houseTile[89] || houseTile[102];
			roomDoor = houseTile[10] || houseTile[11] || houseTile[19];
			roomTable = houseTile[14] || houseTile[18] || houseTile[87] || houseTile[88] || houseTile[90] || houseTile[101];
			roomTorch = houseTile[4] || houseTile[33] || houseTile[34] || houseTile[35] || houseTile[36] || houseTile[42] || houseTile[49] || houseTile[93] || houseTile[95] || houseTile[98] || houseTile[100] || houseTile[149];
			canSpawn = roomChair && roomTable && roomDoor && roomTorch;

			return canSpawn;
		}

		public static bool SolidTile(Func<Int32, Int32, ITile> TileRefs, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Tile = TileRefs(x, y);
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

		public static void SpawnNPC(Func<Int32, Int32, ITile> TileRefs, int x, int y)
		{
			if (Main.stopSpawns)
				return;

			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (Main.wallHouse[(int)TileRefs(x, y).Wall])
				canSpawn = true;

			if (!canSpawn)
				return;

			if (!StartRoomCheck(TileRefs, x, y))
				return;

			if (!RoomNeeds())
				return;

			ScoreRoom(TileRefs, -1);
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
										if (TileRefs(posX, posY).Active && Main.tileSolid[(int)TileRefs(posX, posY).Type])
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
					if (townNPCIndex < NPC.MAX_NPCS)
					{
						Main.npcs[townNPCIndex].homeTileX = bestX;
						Main.npcs[townNPCIndex].homeTileY = bestY;
						if (posX < bestX)
							Main.npcs[townNPCIndex].direction = 1;
						else if (posX > bestX)
							Main.npcs[townNPCIndex].direction = -1;

						Main.npcs[townNPCIndex].netUpdate = true;

						var name = Main.npcs[townNPCIndex].Name;
						if (Main.chrName[Main.npcs[townNPCIndex].Type] != String.Empty)
							name = Main.chrName[Main.npcs[townNPCIndex].Type] + " the " + Main.npcs[townNPCIndex].Name;

						var text = name + " has arrived!"; //String.Format("{0} has arrived!", name);
						NetMessage.SendData(25, -1, -1, text, 255, 50f, 125f, 255f);
					}
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

		public static void QuickFindHome(Func<Int32, Int32, ITile> TileRefs, int npc)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (Main.npcs[npc].homeTileX > 10 && Main.npcs[npc].homeTileY > 10 && Main.npcs[npc].homeTileX < Main.maxTilesX - 10 && Main.npcs[npc].homeTileY < Main.maxTilesY)
			{
				canSpawn = false;
				StartRoomCheck(TileRefs, Main.npcs[npc].homeTileX, Main.npcs[npc].homeTileY - 1);
				if (!canSpawn)
				{
					for (int x = Main.npcs[npc].homeTileX - 1; x < Main.npcs[npc].homeTileX + 2; x++)
					{
						int y = Main.npcs[npc].homeTileY - 1;
						while (y < Main.npcs[npc].homeTileY + 2 && !StartRoomCheck(TileRefs, x, y))
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
						while (y <= Main.npcs[npc].homeTileY + offset && !StartRoomCheck(TileRefs, x, y))
						{
							y += 2;
						}
					}
				}
				if (canSpawn)
				{
					RoomNeeds();
					if (canSpawn)
						ScoreRoom(TileRefs, npc);

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

		public static void ScoreRoom(Func<Int32, Int32, ITile> TileRefs, int ignoreNPC = -1)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
					if (TileRefs(l, m).Active)
					{
						if (TileRefs(l, m).Type == 23 || TileRefs(l, m).Type == 24 || TileRefs(l, m).Type == 25 || TileRefs(l, m).Type == 32)
							Main.evilTiles++;
						else if (TileRefs(l, m).Type == 27)
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
					if (TileRefs(n, num8).Active)
					{
						num = num7;
						if (Main.tileSolid[(int)TileRefs(n, num8).Type] && !Main.tileSolidTop[(int)TileRefs(n, num8).Type] && !Collision.SolidTiles(n - 1, n + 1, num8 - 3, num8 - 1) && TileRefs(n - 1, num8).Active && Main.tileSolid[(int)TileRefs(n - 1, num8).Type] && TileRefs(n + 1, num8).Active && Main.tileSolid[(int)TileRefs(n + 1, num8).Type])
						{
							for (int num9 = n - 2; num9 < n + 3; num9++)
							{
								for (int num10 = num8 - 4; num10 < num8; num10++)
								{
									if (TileRefs(num9, num10).Active)
									{
										if (num9 == n)
											num -= 15;
										else if (TileRefs(num9, num10).Type == 10 || TileRefs(num9, num10).Type == 11)
											num -= 20;
										else if (Main.tileSolid[(int)TileRefs(num9, num10).Type])
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

		public static bool StartRoomCheck(Func<Int32, Int32, ITile> TileRefs, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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

			if (TileRefs(x, y).Active && Main.tileSolid[(int)TileRefs(x, y).Type])
				canSpawn = false;

			CheckRoom(TileRefs, x, y);

			if (numRoomTiles < 60)
				canSpawn = false;

			return canSpawn;
		}

		public static void CheckRoom(Func<Int32, Int32, ITile> TileRefs, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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

			if (TileRefs(x, y).Active)
			{
				houseTile[(int)TileRefs(x, y).Type] = true;
				if (Main.tileSolid[(int)TileRefs(x, y).Type] || TileRefs(x, y).Type == 11)
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
				if (Main.wallHouse[(int)TileRefs(x + j, y).Wall])
					flag = true;

				if (TileRefs(x + j, y).Active && (Main.tileSolid[(int)TileRefs(x + j, y).Type] || TileRefs(x + j, y).Type == 11))
					flag = true;

				if (Main.wallHouse[(int)TileRefs(x, y + j).Wall])
					flag2 = true;

				if (TileRefs(x, y + j).Active && (Main.tileSolid[(int)TileRefs(x, y + j).Type] || TileRefs(x, y + j).Type == 11))
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
						CheckRoom(TileRefs, k, l);
				}
			}
		}

		public static void dropMeteor(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
					if (TileRefs(j, num5).Active && TileRefs(j, num5).Type == 37)
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
					if (TileRefs(x, y).Active && Main.tileSolid[(int)TileRefs(x, y).Type])
					{
						flag = meteor(TileRefs, sandbox, x, y);
						break;
					}
				}
				num++;
				if (num >= 100)
					return;

				Thread.Sleep(10);
			}
		}

		public static bool meteor(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
					if (TileRefs(modX, modY).Active && TileRefs(modX, modY).Type == 21)
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
						if (!Main.tileSolid[(int)TileRefs(num2, num3).Type])
							TileRefs(num2, num3).SetActive(false);

						TileRefs(num2, num3).SetType(37);
					}
				}
			}

			for (int num4 = x - TILES_OFFSET_2; num4 < x + TILES_OFFSET_2; num4++)
			{
				for (int num5 = y - TILES_OFFSET_2; num5 < y + TILES_OFFSET_2; num5++)
				{
					if (num5 > y + Main.rand.Next(-2, 3) - 5 && Math.Abs(x - num4) + Math.Abs(y - num5) < TILES_OFFSET_2 + Main.rand.Next(-3, 4))
						TileRefs(num4, num5).SetActive(false);
				}
			}

			for (int num6 = x - TILE_OFFSET_3; num6 < x + TILE_OFFSET_3; num6++)
			{
				for (int num7 = y - TILE_OFFSET_3; num7 < y + TILE_OFFSET_3; num7++)
				{
					if (TileRefs(num6, num7).Type == 5 || TileRefs(num6, num7).Type == 32)
						KillTile(TileRefs, sandbox, num6, num7);

					SquareTileFrame(TileRefs, sandbox, num6, num7, true);
					SquareWallFrame(TileRefs, num6, num7, true);
				}
			}

			for (int num8 = x - TILE_OFFSET_4; num8 < x + TILE_OFFSET_4; num8++)
			{
				for (int num9 = y - TILE_OFFSET_4; num9 < y + TILE_OFFSET_4; num9++)
				{
					if (TileRefs(num8, num9).Active && Main.rand.Next(10) == 0 && (double)(Math.Abs(x - num8) + Math.Abs(y - num9)) < (double)TILE_OFFSET_4 * 1.3)
					{
						if (TileRefs(num8, num9).Type == 5 || TileRefs(num8, num9).Type == 32)
							KillTile(TileRefs, sandbox, num8, num9);

						TileRefs(num8, num9).SetType(37);
						SquareTileFrame(TileRefs, sandbox, num8, num9, true);
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

		public static void GrowShroom(Func<Int32, Int32, ITile> TileRefs, int i, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (TileRefs(i - 1, y - 1).Lava ||
				TileRefs(i - 1, y - 1).Lava ||
				TileRefs(i + 1, y - 1).Lava)
				return;

			if (TileRefs(i, y).Active && TileRefs(i, y).Type == 70 && TileRefs(i, y - 1).Wall == 0 && TileRefs(i - 1, y).Active && TileRefs(i - 1, y).Type == 70 && TileRefs(i + 1, y).Active && TileRefs(i + 1, y).Type == 70 && EmptyTileCheck(TileRefs, i - 2, i + 2, y - 13, y - 1, 71))
			{
				int num = genRand.Next(4, 11);
				int num2;
				for (int j = y - num; j < y; j++)
				{
					TileRefs(i, j).SetFrameNumber((byte)genRand.Next(3));
					TileRefs(i, j).SetActive(true);
					TileRefs(i, j).SetType(72);
					num2 = genRand.Next(3);
					if (num2 == 0)
					{
						TileRefs(i, j).SetFrameX(0);
						TileRefs(i, j).SetFrameY(0);
					}
					if (num2 == 1)
					{
						TileRefs(i, j).SetFrameX(0);
						TileRefs(i, j).SetFrameY(18);
					}
					if (num2 == 2)
					{
						TileRefs(i, j).SetFrameX(0);
						TileRefs(i, j).SetFrameY(36);
					}
				}
				num2 = genRand.Next(3);
				if (num2 == 0)
				{
					TileRefs(i, y - num).SetFrameX(36);
					TileRefs(i, y - num).SetFrameY(0);
				}
				if (num2 == 1)
				{
					TileRefs(i, y - num).SetFrameX(36);
					TileRefs(i, y - num).SetFrameY(18);
				}
				if (num2 == 2)
				{
					TileRefs(i, y - num).SetFrameX(36);
					TileRefs(i, y - num).SetFrameY(36);
				}
				RangeFrame(TileRefs, null, i - 2, y - num - 1, i + 2, y + 1);
				NetMessage.SendTileSquare(-1, i, (int)((double)y - (double)num * 0.5), num + 1);
			}
		}

		public static bool EmptyTileCheck(Func<Int32, Int32, ITile> TileRefs, int startX, int endX, int startY, int endY, int ignoreStyle = -1)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
					if (TileRefs(i, j).Active)
					{
						if (ignoreStyle == -1)
							return false;

						if (ignoreStyle == 11 && TileRefs(i, j).Type != 11)
							return false;

						if (ignoreStyle == 20 && TileRefs(i, j).Type != 20 &&
							TileRefs(i, j).Type != 3 &&
							TileRefs(i, j).Type != 24 &&
							TileRefs(i, j).Type != 61 &&
							TileRefs(i, j).Type != 32 &&
							TileRefs(i, j).Type != 69 &&
							TileRefs(i, j).Type != 73 &&
							TileRefs(i, j).Type != 74 &&
							TileRefs(i, j).Type != 110 &&
							TileRefs(i, j).Type != 113)
							return false;

						if (ignoreStyle == 71 && TileRefs(i, j).Type != 71)
							return false;

					}
				}
			}
			return true;
		}

		public static bool PlaceDoor(Func<Int32, Int32, ITile> TileRefs, int i, int j, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			try
			{
				if (TileRefs(i, j - 2).Active && Main.tileSolid[(int)TileRefs(i, j - 2).Type] && TileRefs(i, j + 2).Active && Main.tileSolid[(int)TileRefs(i, j + 2).Type])
				{
					TileRefs(i, j - 1).SetActive(true);
					TileRefs(i, j - 1).SetType(10);
					TileRefs(i, j - 1).SetFrameY(0);
					TileRefs(i, j - 1).SetFrameX((short)(genRand.Next(3) * 18));
					TileRefs(i, j).SetActive(true);
					TileRefs(i, j).SetType(10);
					TileRefs(i, j).SetFrameY(18);
					TileRefs(i, j).SetFrameX((short)(genRand.Next(3) * 18));
					TileRefs(i, j + 1).SetActive(true);
					TileRefs(i, j + 1).SetType(10);
					TileRefs(i, j + 1).SetFrameY(36);
					TileRefs(i, j + 1).SetFrameX((short)(genRand.Next(3) * 18));
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

		public static bool CloseDoor(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y, bool forced, ISender sender)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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

			int frameX = (int)TileRefs(x, y).FrameX;
			int frameY = (int)TileRefs(x, y).FrameY;
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
						TileRefs(l, m).SetType(10);
						TileRefs(l, m).SetFrameX((short)(genRand.Next(3) * 18));
					}
					else
					{
						TileRefs(l, m).SetActive(false);
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
					TileFrame(TileRefs, sandbox, n, num5, false, false);
				}
			}
			return true;
		}

		public static bool OpenDoor(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y, int direction, ISender sender)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
			if (TileRefs(x, y - 1).FrameY == 0 && TileRefs(x, y - 1).Type == TileRefs(x, y).Type)
			{
				num = y - 1;
			}
			else if (TileRefs(x, y - 2).FrameY == 0 && TileRefs(x, y - 2).Type == TileRefs(x, y).Type)
			{
				num = y - 2;
			}
			else if (TileRefs(x, y + 1).FrameY == 0 && TileRefs(x, y + 1).Type == TileRefs(x, y).Type)
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
				if (TileRefs(num4, k).Active)
				{
					if (TileRefs(num4, k).Type != 3 && TileRefs(num4, k).Type != 24 && TileRefs(num4, k).Type != 52 && TileRefs(num4, k).Type != 61 && TileRefs(num4, k).Type != 62 && TileRefs(num4, k).Type != 69 && TileRefs(num4, k).Type != 71 && TileRefs(num4, k).Type != 73 && TileRefs(num4, k).Type != 74)
					{
						flag = false;
						break;
					}
					KillTile(TileRefs, sandbox, num4, k);
				}
			}
			if (flag)
			{
				TileRefs(num2, num).SetActive(true);
				TileRefs(num2, num).SetType(11);
				TileRefs(num2, num).SetFrameY(0);
				TileRefs(num2, num).SetFrameX(num3);
				TileRefs(num2 + 1, num).SetActive(true);
				TileRefs(num2 + 1, num).SetType(11);
				TileRefs(num2 + 1, num).SetFrameY(0);
				TileRefs(num2 + 1, num).SetFrameX((short)(num3 + 18));
				TileRefs(num2, num + 1).SetActive(true);
				TileRefs(num2, num + 1).SetType(11);
				TileRefs(num2, num + 1).SetFrameY(18);
				TileRefs(num2, num + 1).SetFrameX(num3);
				TileRefs(num2 + 1, num + 1).SetActive(true);
				TileRefs(num2 + 1, num + 1).SetType(11);
				TileRefs(num2 + 1, num + 1).SetFrameY(18);
				TileRefs(num2 + 1, num + 1).SetFrameX((short)(num3 + 18));
				TileRefs(num2, num + 2).SetActive(true);
				TileRefs(num2, num + 2).SetType(11);
				TileRefs(num2, num + 2).SetFrameY(36);
				TileRefs(num2, num + 2).SetFrameX(num3);
				TileRefs(num2 + 1, num + 2).SetActive(true);
				TileRefs(num2 + 1, num + 2).SetType(11);
				TileRefs(num2 + 1, num + 2).SetFrameY(36);
				TileRefs(num2 + 1, num + 2).SetFrameX((short)(num3 + 18));
				for (int l = num2 - 1; l <= num2 + 2; l++)
				{
					for (int m = num - 1; m <= num + 2; m++)
					{
						TileFrame(TileRefs, sandbox, l, m, false, false);
					}
				}
			}
			return flag;
		}

		public static void Check1x2(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int j, byte type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			int num = j;
			bool flag = true;

			int FrameY = (int)TileRefs(x, num).FrameY;

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
			if ((int)TileRefs(x, num).FrameY == 40 * num2 &&
				(int)TileRefs(x, num + 1).FrameY == 40 * num2 + 18 &&
				TileRefs(x, num).Type == type && TileRefs(x, num + 1).Type == type)
			{
				flag = false;
			}
			if (!TileRefs(x, num + 2).Active || !Main.tileSolid[(int)TileRefs(x, num + 2).Type])
			{
				flag = true;
			}
			if (TileRefs(x, num + 2).Type != 2 && TileRefs(x, num + 2).Type != 109 && TileRefs(x, num + 2).Type != 147 && TileRefs(x, num).Type == 20)
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				if (TileRefs(x, num).Type == type)
				{
					KillTile(TileRefs, sandbox, x, num);
				}
				if (TileRefs(x, num + 1).Type == type)
				{
					KillTile(TileRefs, sandbox, x, num + 1);
				}
				if (type == 15)
				{
					if (num2 == 1)
						StorePlayerItem(sandbox, x * 16, num * 16, 32, 32, 358);
					else
						StorePlayerItem(sandbox, x * 16, num * 16, 32, 32, 34);
				}
				else if (type == 134)
					StorePlayerItem(sandbox, x * 16, num * 16, 32, 32, 525);

				destroyObject = false;
			}
		}

		public static void CheckOnTable1x1(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if ((!TileRefs(x, y + 1).Active || !Main.tileTable[(int)TileRefs(x, y + 1).Type]))
			{
				if (type == 78)
				{
					if (!TileRefs(x, y + 1).Active || !Main.tileSolid[(int)TileRefs(x, y + 1).Type])
					{
						KillTile(TileRefs, sandbox, x, y);
						return;
					}
				}
				else
				{
					KillTile(TileRefs, sandbox, x, y);
				}
			}
		}

		public static void CheckSign(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
			int k = (int)(TileRefs(x, y).FrameX / 18);
			int num5 = (int)(TileRefs(x, y).FrameY / 18);

			while (k > 1)
				k -= 2;

			int num6 = x - k;
			int num7 = y - num5;
			int num8 = (int)(TileRefs(num6, num7).FrameX / 18 / 2);
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
					if (!TileRefs(l, m).Active || (int)TileRefs(l, m).Type != type)
					{
						flag = true;
						break;
					}
					if ((int)(TileRefs(l, m).FrameX / 18) != k + num8 * 2 || (int)(TileRefs(l, m).FrameY / 18) != num5)
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
					if (TileRefs(num6, num7 + 2).Active && Main.tileSolid[(int)TileRefs(num6, num7 + 2).Type] && TileRefs(num6 + 1, num7 + 2).Active &&
						Main.tileSolid[(int)TileRefs(num6 + 1, num7 + 2).Type])
					{
						num8 = 0;
					}
					else
					{
						flag = true;
					}
				}
				else if (TileRefs(num6, num7 + 2).Active && Main.tileSolid[(int)TileRefs(num6, num7 + 2).Type] &&
					TileRefs(num6 + 1, num7 + 2).Active && Main.tileSolid[(int)TileRefs(num6 + 1, num7 + 2).Type])
				{
					num8 = 0;
				}
				else if (TileRefs(num6, num7 - 1).Active && Main.tileSolid[(int)TileRefs(num6, num7 - 1).Type] &&
					!Main.tileSolidTop[(int)TileRefs(num6, num7 - 1).Type] && TileRefs(num6 + 1, num7 - 1).Active &&
					Main.tileSolid[(int)TileRefs(num6 + 1, num7 - 1).Type] && !Main.tileSolidTop[(int)TileRefs(num6 + 1, num7 - 1).Type])
				{
					num8 = 1;
				}
				else if (TileRefs(num6 - 1, num7).Active && Main.tileSolid[(int)TileRefs(num6 - 1, num7).Type] &&
					!Main.tileSolidTop[(int)TileRefs(num6 - 1, num7).Type] && TileRefs(num6 - 1, num7 + 1).Active &&
					Main.tileSolid[(int)TileRefs(num6 - 1, num7 + 1).Type] && !Main.tileSolidTop[(int)TileRefs(num6 - 1, num7 + 1).Type])
				{
					num8 = 2;
				}
				else if (TileRefs(num6 + 2, num7).Active && Main.tileSolid[(int)TileRefs(num6 + 2, num7).Type] &&
					!Main.tileSolidTop[(int)TileRefs(num6 + 2, num7).Type] && TileRefs(num6 + 2, num7 + 1).Active &&
					Main.tileSolid[(int)TileRefs(num6 + 2, num7 + 1).Type] && !Main.tileSolidTop[(int)TileRefs(num6 + 2, num7 + 1).Type])
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
						if ((int)TileRefs(n, num9).Type == type)
							KillTile(TileRefs, sandbox, n, num9);
					}
				}

				Sign.KillSign(num6, num7);

				if (type == 85)
					StorePlayerItem(sandbox, x * 16, y * 16, 32, 32, 321);
				else
					StorePlayerItem(sandbox, x * 16, y * 16, 32, 32, 171);

				destroyObject = false;
				return;
			}
			int num10 = 36 * num8;
			for (int num11 = 0; num11 < 2; num11++)
			{
				for (int num12 = 0; num12 < 2; num12++)
				{
					TileRefs(num6 + num11, num7 + num12).SetActive(true);
					TileRefs(num6 + num11, num7 + num12).SetType((byte)type);
					TileRefs(num6 + num11, num7 + num12).SetFrameX((short)(num10 + 18 * num11));
					TileRefs(num6 + num11, num7 + num12).SetFrameY((short)(18 * num12));
				}
			}
		}

		public static bool PlaceSign(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
				if (TileRefs(x, y + 1).Active && Main.tileSolid[(int)TileRefs(x, y + 1).Type] && TileRefs(x + 1, y + 1).Active && Main.tileSolid[(int)TileRefs(x + 1, y + 1).Type])
				{
					num6--;
					num7 = 0;
				}
				else if (TileRefs(x, y - 1).Active && Main.tileSolid[(int)TileRefs(x, y - 1).Type] && !Main.tileSolidTop[(int)TileRefs(x, y - 1).Type] && TileRefs(x + 1, y - 1).Active && Main.tileSolid[(int)TileRefs(x + 1, y - 1).Type] && !Main.tileSolidTop[(int)TileRefs(x + 1, y - 1).Type])
				{
					num7 = 1;
				}
				else if (TileRefs(x - 1, y).Active && Main.tileSolid[(int)TileRefs(x - 1, y).Type] && !Main.tileSolidTop[(int)TileRefs(x - 1, y).Type] && TileRefs(x - 1, y + 1).Active && Main.tileSolid[(int)TileRefs(x - 1, y + 1).Type] && !Main.tileSolidTop[(int)TileRefs(x - 1, y + 1).Type])
				{
					num7 = 2;
				}
				else
				{
					if (!TileRefs(x + 1, y).Active || !Main.tileSolid[(int)TileRefs(x + 1, y).Type] || Main.tileSolidTop[(int)TileRefs(x + 1, y).Type] || !TileRefs(x + 1, y + 1).Active || !Main.tileSolid[(int)TileRefs(x + 1, y + 1).Type] || Main.tileSolidTop[(int)TileRefs(x + 1, y + 1).Type])
					{
						return false;
					}
					num5--;
					num7 = 3;
				}
			}
			else if (type == 85)
			{
				if (!TileRefs(x, y + 1).Active || !Main.tileSolid[(int)TileRefs(x, y + 1).Type] || !TileRefs(x + 1, y + 1).Active || !Main.tileSolid[(int)TileRefs(x + 1, y + 1).Type])
				{
					return false;
				}
				num6--;
				num7 = 0;
			}
			if (TileRefs(num5, num6).Active || TileRefs(num5 + 1, num6).Active || TileRefs(num5, num6 + 1).Active || TileRefs(num5 + 1, num6 + 1).Active)
			{
				return false;
			}
			int num8 = 36 * num7;
			for (int k = 0; k < 2; k++)
			{
				for (int l = 0; l < 2; l++)
				{
					TileRefs(num5 + k, num6 + l).SetActive(true);
					TileRefs(num5 + k, num6 + l).SetType((byte)type);
					TileRefs(num5 + k, num6 + l).SetFrameX((short)(num8 + 18 * k));
					TileRefs(num5 + k, num6 + l).SetFrameY((short)(18 * l));
				}
			}
			return true;
		}

		public static void PlaceOnTable1x1(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type, int style = 0)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			bool flag = false;
			if (!TileRefs(x, y).Active && TileRefs(x, y + 1).Active && Main.tileTable[(int)TileRefs(x, y + 1).Type])
			{
				flag = true;
			}
			if (type == 78 && !TileRefs(x, y).Active && TileRefs(x, y + 1).Active && Main.tileSolid[(int)TileRefs(x, y + 1).Type])
			{
				flag = true;
			}
			if (flag)
			{
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetFrameX((short)(style * 18));
				TileRefs(x, y).SetFrameY(0);
				TileRefs(x, y).SetType((byte)type);
				if (type == 50)
				{
					TileRefs(x, y).SetFrameX((short)(18 * genRand.Next(5)));
				}
			}
		}

		public static bool PlaceAlch(Func<Int32, Int32, ITile> TileRefs, int x, int y, int style)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (!TileRefs(x, y).Active && TileRefs(x, y + 1).Active)
			{
				bool flag = false;
				if (style == 0)
				{
					if (TileRefs(x, y + 1).Type != 2 && TileRefs(x, y + 1).Type != 78 && TileRefs(x, y + 1).Type != 109)
					{
						flag = true;
					}
					if (TileRefs(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 1)
				{
					if (TileRefs(x, y + 1).Type != 60 && TileRefs(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileRefs(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 2)
				{
					if (TileRefs(x, y + 1).Type != 0 && TileRefs(x, y + 1).Type != 59 && TileRefs(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileRefs(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 3)
				{
					if (TileRefs(x, y + 1).Type != 23 && TileRefs(x, y + 1).Type != 25 && TileRefs(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileRefs(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 4)
				{
					if (TileRefs(x, y + 1).Type != 53 && TileRefs(x, y + 1).Type != 78 && TileRefs(x, y + 1).Type != 116)
					{
						flag = true;
					}
					if (TileRefs(x, y).Liquid > 0 && TileRefs(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (style == 5)
				{
					if (TileRefs(x, y + 1).Type != 57 && TileRefs(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileRefs(x, y).Liquid > 0 && !TileRefs(x, y).Lava)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					TileRefs(x, y).SetActive(true);
					TileRefs(x, y).SetType(82);
					TileRefs(x, y).SetFrameX((short)(18 * style));
					TileRefs(x, y).SetFrameY(0);
					return true;
				}
			}
			return false;
		}

		public static void GrowAlch(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (TileRefs(x, y).Active)
			{
				if (TileRefs(x, y).Type == 82 && genRand.Next(50) == 0)
				{
					TileRefs(x, y).SetType(83);

					NetMessage.SendTileSquare(-1, x, y, 1);
					SquareTileFrame(TileRefs, sandbox, x, y, true);
					return;
				}
				if (TileRefs(x, y).FrameX == 36)
				{
					if (TileRefs(x, y).Type == 83)
						TileRefs(x, y).SetType(84);
					else
						TileRefs(x, y).SetType(83);
					NetMessage.SendTileSquare(-1, x, y, 1);
				}
			}
		}

		public static void PlantAlch(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
			while (y < Main.maxTilesY - 20 && !TileRefs(x, y).Active)
			{
				y++;
			}
			if (TileRefs(x, y).Active && !TileRefs(x, y - 1).Active && TileRefs(x, y - 1).Liquid == 0)
			{
				var tile = TileRefs(x, y);

				if (tile.Type == 2 || tile.Type == 109)
					PlaceAlch(TileRefs, x, y - 1, 0);

				if (tile.Type == 60)
					PlaceAlch(TileRefs, x, y - 1, 1);

				if (tile.Type == 0 || tile.Type == 59)
					PlaceAlch(TileRefs, x, y - 1, 2);

				if (tile.Type == 23 || tile.Type == 25)
					PlaceAlch(TileRefs, x, y - 1, 3);

				if (tile.Type == 53 || tile.Type == 116)
					PlaceAlch(TileRefs, x, y - 1, 4);

				if (tile.Type == 57)
					PlaceAlch(TileRefs, x, y - 1, 5);

				if (TileRefs(x, y - 1).Active)
					NetMessage.SendTileSquare(-1, x, y - 1, 1);
			}
		}

		public static void CheckAlch(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			bool active = !TileRefs(x, y + 1).Active;

			int num = (int)(TileRefs(x, y).FrameX / 18);
			TileRefs(x, y).SetFrameY(0);
			if (!active)
			{
				if (num == 0)
				{
					if (TileRefs(x, y + 1).Type != 2 && TileRefs(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (TileRefs(x, y).Liquid > 0 && TileRefs(x, y).Lava)
					{
						active = true;
					}
				}
				else if (num == 1)
				{
					if (TileRefs(x, y + 1).Type != 60 && TileRefs(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (TileRefs(x, y).Liquid > 0 && TileRefs(x, y).Lava)
					{
						active = true;
					}
				}
				else if (num == 2)
				{
					if (TileRefs(x, y + 1).Type != 0 && TileRefs(x, y + 1).Type != 59 && TileRefs(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (TileRefs(x, y).Liquid > 0 && TileRefs(x, y).Lava)
					{
						active = true;
					}
				}
				else if (num == 3)
				{
					if (TileRefs(x, y + 1).Type != 23 && TileRefs(x, y + 1).Type != 25 && TileRefs(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (TileRefs(x, y).Liquid > 0 && TileRefs(x, y).Lava)
					{
						active = true;
					}
				}
				else if (num == 4)
				{
					if (TileRefs(x, y + 1).Type != 53 && TileRefs(x, y + 1).Type != 78 && TileRefs(x, y + 1).Type != 116)
					{
						active = true;
					}
					if (TileRefs(x, y).Liquid > 0 && TileRefs(x, y).Lava)
					{
						active = true;
					}
					if (TileRefs(x, y).Type != 82 && !TileRefs(x, y).Lava)
					{
						if (TileRefs(x, y).Liquid > 16)
						{
							if (TileRefs(x, y).Type == 83)
							{
								TileRefs(x, y).SetType(84);

								NetMessage.SendTileSquare(-1, x, y, 1);
							}
						}
						else if (TileRefs(x, y).Type == 84)
						{
							TileRefs(x, y).SetType(83);

							NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
				else if (num == 5)
				{
					if (TileRefs(x, y + 1).Type != 57 && TileRefs(x, y + 1).Type != 78)
					{
						active = true;
					}
					if (TileRefs(x, y).Liquid > 0 && !TileRefs(x, y).Lava)
					{
						active = true;
					}
					if (TileRefs(x, y).Type != 82 && TileRefs(x, y).Lava && TileRefs(x, y).Type != 82 && TileRefs(x, y).Lava)
					{
						if (TileRefs(x, y).Liquid > 16)
						{
							if (TileRefs(x, y).Type == 83)
							{
								TileRefs(x, y).SetType(84);
								NetMessage.SendTileSquare(-1, x, y, 1);
							}
						}
						else if (TileRefs(x, y).Type == 84)
						{
							TileRefs(x, y).SetType(83);
							NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
			}
			if (active)
			{
				KillTile(TileRefs, sandbox, x, y);
			}
		}

		public static void Place1x2(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type, int style)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			short frameX = 0;
			if (type == 20)
			{
				frameX = (short)(genRand.Next(3) * 18);
			}
			if (TileRefs(x, y + 1).Active && Main.tileSolid[(int)TileRefs(x, y + 1).Type] && !TileRefs(x, y - 1).Active)
			{
				short frameHeight = (short)(style * 40);
				TileRefs(x, y - 1).SetActive(true);
				TileRefs(x, y - 1).SetFrameY(frameHeight);
				TileRefs(x, y - 1).SetFrameX(frameX);
				TileRefs(x, y - 1).SetType((byte)type);
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetFrameY((short)(frameHeight + 18));
				TileRefs(x, y).SetFrameX(frameX);
				TileRefs(x, y).SetType((byte)type);
			}
		}

		public static void PlaceBanner(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type, int style = 0)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int FrameLength = style * 18;
			if (TileRefs(x, y - 1).Active && Main.tileSolid[(int)TileRefs(x, y - 1).Type] && !Main.tileSolidTop[(int)TileRefs(x, y - 1).Type] && !TileRefs(x, y).Active && !TileRefs(x, y + 1).Active && !TileRefs(x, y + 2).Active)
			{
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetFrameY(0);
				TileRefs(x, y).SetFrameX((short)FrameLength);
				TileRefs(x, y).SetType((byte)type);
				TileRefs(x, y + 1).SetActive(true);
				TileRefs(x, y + 1).SetFrameY(18);
				TileRefs(x, y + 1).SetFrameX((short)FrameLength);
				TileRefs(x, y + 1).SetType((byte)type);
				TileRefs(x, y + 2).SetActive(true);
				TileRefs(x, y + 2).SetFrameY(36);
				TileRefs(x, y + 2).SetFrameX((short)FrameLength);
				TileRefs(x, y + 2).SetType((byte)type);
			}
		}

		public static void CheckBanner(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int j, byte type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			int num = j - (int)(TileRefs(x, j).FrameY / 18);
			int frameX = (int)TileRefs(x, j).FrameX;
			bool flag = false;

			for (int i = 0; i < 3; i++)
			{
				if (!TileRefs(x, num + i).Active)
					flag = true;
				else if (TileRefs(x, num + i).Type != type)
					flag = true;
				else if ((int)TileRefs(x, num + i).FrameY != i * 18)
					flag = true;
				else if ((int)TileRefs(x, num + i).FrameX != frameX)
					flag = true;
			}
			if (!TileRefs(x, num - 1).Active)
				flag = true;

			if (!Main.tileSolid[(int)TileRefs(x, num - 1).Type])
				flag = true;

			if (Main.tileSolidTop[(int)TileRefs(x, num - 1).Type])
				flag = true;

			if (flag)
			{
				destroyObject = true;
				for (int k = 0; k < 3; k++)
				{
					if (TileRefs(x, num + k).Type == type)
						KillTile(TileRefs, sandbox, x, num + k);
				}
				if (type == 91)
				{
					int num2 = frameX / 18;
					StorePlayerItem(sandbox, x * 16, (num + 1) * 16, 32, 32, 337 + num2);
				}
				destroyObject = false;
			}
		}

		public static void Place1x2Top(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			short frameX = 0;
			if (TileRefs(x, y - 1).Active && Main.tileSolid[(int)TileRefs(x, y - 1).Type] &&
				!Main.tileSolidTop[(int)TileRefs(x, y - 1).Type] && !TileRefs(x, y + 1).Active)
			{
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetFrameY(0);
				TileRefs(x, y).SetFrameX(frameX);
				TileRefs(x, y).SetType((byte)type);
				TileRefs(x, y + 1).SetActive(true);
				TileRefs(x, y + 1).SetFrameY(18);
				TileRefs(x, y + 1).SetFrameX(frameX);
				TileRefs(x, y + 1).SetType((byte)type);
			}
		}

		public static void Check1x2Top(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y, byte type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = true;

			if (TileRefs(x, y).FrameY == 18)
				y--;

			if (TileRefs(x, y).FrameY == 0
				&& TileRefs(x, y + 1).FrameY == 18
				&& TileRefs(x, y).Type == type
				&& TileRefs(x, y + 1).Type == type)
			{
				flag = false;
			}

			if (!TileRefs(x, y - 1).Active
				|| !Main.tileSolid[(int)TileRefs(x, y - 1).Type]
				|| Main.tileSolidTop[(int)TileRefs(x, y - 1).Type])
			{
				flag = true;
			}

			if (flag)
			{
				destroyObject = true;
				if (TileRefs(x, y).Type == type)
					KillTile(TileRefs, sandbox, x, y);

				if (TileRefs(x, y + 1).Type == type)
					KillTile(TileRefs, sandbox, x, y + 1);

				if (type == 42)
					StorePlayerItem(sandbox, x * 16, y * 16, 32, 32, 136);

				destroyObject = false;
			}
		}

		public static void Check2x1(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y, byte type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = true;

			if (TileRefs(x, y).FrameX == 18)
				x--;

			if (TileRefs(x, y).FrameX == 0
				&& TileRefs(x + 1, y).FrameX == 18
				&& TileRefs(x, y).Type == type
				&& TileRefs(x + 1, y).Type == type)
			{
				flag = false;
			}

			if (type == 29 || type == 103)
			{
				if (!TileRefs(x, y + 1).Active || !Main.tileTable[(int)TileRefs(x, y + 1).Type])
				{
					flag = true;
				}
				if (!TileRefs(x + 1, y + 1).Active || !Main.tileTable[(int)TileRefs(x + 1, y + 1).Type])
				{
					flag = true;
				}
			}
			else
			{
				if (!TileRefs(x, y + 1).Active || !Main.tileSolid[(int)TileRefs(x, y + 1).Type])
				{
					flag = true;
				}
				if (!TileRefs(x + 1, y + 1).Active || !Main.tileSolid[(int)TileRefs(x + 1, y + 1).Type])
				{
					flag = true;
				}
			}

			if (flag)
			{
				destroyObject = true;
				if (TileRefs(x, y).Type == type)
				{
					KillTile(TileRefs, sandbox, x, y);
				}
				if (TileRefs(x + 1, y).Type == type)
				{
					KillTile(TileRefs, sandbox, x + 1, y);
				}
				if (type == 16)
				{
					StorePlayerItem(sandbox, x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 35, 1, false);
				}
				if (type == 18)
				{
					StorePlayerItem(sandbox, x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 36, 1, false);
				}
				if (type == 29)
				{
					StorePlayerItem(sandbox, x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 87, 1, false);
				}
				if (type == 103)
				{
					StorePlayerItem(sandbox, x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 356, 1, false);
				}
				destroyObject = false;
				SquareTileFrame(TileRefs, sandbox, x, y, true);
				SquareTileFrame(TileRefs, sandbox, x + 1, y, true);
			}
		}

		public static void Place2x1(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			bool flag = false;
			if (type != 29 && type != 103
				&& TileRefs(x, y + 1).Active
				&& TileRefs(x + 1, y + 1).Active
				&& Main.tileSolid[(int)TileRefs(x, y + 1).Type]
				&& Main.tileSolid[(int)TileRefs(x + 1, y + 1).Type]
				&& !TileRefs(x, y).Active
				&& !TileRefs(x + 1, y).Active)
			{
				flag = true;
			}
			else
			{
				if ((type == 29 || type == 103)
					&& TileRefs(x, y + 1).Active
					&& TileRefs(x + 1, y + 1).Active
					&& Main.tileTable[(int)TileRefs(x, y + 1).Type]
					&& Main.tileTable[(int)TileRefs(x + 1, y + 1).Type]
					&& !TileRefs(x, y).Active
					&& !TileRefs(x + 1, y).Active)
				{
					flag = true;
				}
			}

			if (flag)
			{
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetFrameY(0);
				TileRefs(x, y).SetFrameX(0);
				TileRefs(x, y).SetType((byte)type);
				TileRefs(x + 1, y).SetActive(true);
				TileRefs(x + 1, y).SetFrameY(0);
				TileRefs(x + 1, y).SetFrameX(18);
				TileRefs(x + 1, y).SetType((byte)type);
			}
		}

		public static void Check4x2(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = false;
			int num = i;
			num += (int)(TileRefs(i, j).FrameX / 18 * -1);

			if ((type == 79 || type == 90) && TileRefs(i, j).FrameX >= 72)
				num += 4;

			int num2 = j + (int)(TileRefs(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 4; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					int num3 = (k - num) * 18;
					if ((type == 79 || type == 90) && TileRefs(i, j).FrameX >= 72)
					{
						num3 = (k - num + 4) * 18;
					}
					if (!TileRefs(k, l).Active || (int)TileRefs(k, l).Type != type || (int)TileRefs(k, l).FrameX != num3 || (int)TileRefs(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!TileRefs(k, num2 + 2).Active || !Main.tileSolid[(int)TileRefs(k, num2 + 2).Type])
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
						if ((int)TileRefs(m, n).Type == type && TileRefs(m, n).Active)
						{
							KillTile(TileRefs, sandbox, m, n);
						}
					}
				}
				if (type == 79)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 224);
				}
				if (type == 90)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 336);
				}
				destroyObject = false;
				for (int num4 = num - 1; num4 < num + 4; num4++)
				{
					for (int num5 = num2 - 1; num5 < num2 + 4; num5++)
					{
						TileFrame(TileRefs, sandbox, num4, num5);
					}
				}
			}
		}

		public static void Check2x2(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = false;
			int num = i;
			int num2 = 0;
			num = (int)(TileRefs(i, j).FrameX / 18 * -1);
			int num3 = (int)(TileRefs(i, j).FrameY / 18 * -1);
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
					if (!TileRefs(k, l).Active || (int)TileRefs(k, l).Type != type ||
						(int)TileRefs(k, l).FrameX != (k - num) * 18 + num2 || (int)TileRefs(k, l).FrameY != (l - num3) * 18)
					{
						flag = true;
					}
				}
				if (type == 95 || type == 126)
				{
					if (!TileRefs(k, num3 - 1).Active || !Main.tileSolid[(int)TileRefs(k, num3 - 1).Type] || Main.tileSolidTop[(int)TileRefs(k, num3 - 1).Type])
					{
						flag = true;
					}
				}
				else if (type != 138)
				{
					if (!TileRefs(k, num3 + 2).Active || (!Main.tileSolid[(int)TileRefs(k, num3 + 2).Type] && !Main.tileTable[(int)TileRefs(k, num3 + 2).Type]))
					{
						flag = true;
					}
				}
			}
			if (type == 138 && !SolidTile(TileRefs, num, num3 + 2) && !SolidTile(TileRefs, num + 1, num3 + 2))
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
						if ((int)TileRefs(m, n).Type == type && TileRefs(m, n).Active)
							KillTile(TileRefs, sandbox, m, n);
					}
				}
				if (type == 85)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 321);

				if (type == 94)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 352);

				if (type == 95)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 344);

				if (type == 96)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 345);

				if (type == 97)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 346);

				if (type == 98)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 347);

				if (type == 99)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 348);

				if (type == 100)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 349);

				if (type == 125)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 487);

				if (type == 126)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 488);

				if (type == 132)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 513);

				if (type == 142)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 581);

				if (type == 143)
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 582);

				if (type == 138 && !gen)
					Projectile.NewProjectile((float)(num * 16) + 15.5f, (float)(num3 * 16 + 16), 0f, 0f, 99, 70, 10f, Main.myPlayer);

				destroyObject = false;
				for (int num4 = num - 1; num4 < num + 3; num4++)
				{
					for (int num5 = num3 - 1; num5 < num3 + 3; num5++)
					{
						TileFrame(TileRefs, sandbox, num4, num5, false, false);
					}
				}
			}
		}

		public static void Check3x2(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type)
		{
			if (destroyObject)
				return;

			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			bool flag = false;
			int num = i + (int)(TileRefs(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileRefs(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					if (!TileRefs(k, l).Active || (int)TileRefs(k, l).Type != type || (int)TileRefs(k, l).FrameX != (k - num) * 18 ||
						(int)TileRefs(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!TileRefs(k, num2 + 2).Active || !Main.tileSolid[(int)TileRefs(k, num2 + 2).Type])
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
						if ((int)TileRefs(m, n).Type == type && TileRefs(m, n).Active)
							KillTile(TileRefs, sandbox, m, n);
					}
				}
				if (type == 14)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 32, 1, false, 0);
				}
				else if (type == 114)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 398, 1, false, 0);
				}
				else if (type == 26)
				{
					if (!noTileActions)
					{
						SmashAltar(TileRefs, sandbox, i, j);
					}
				}
				else if (type == 17)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 33, 1, false, 0);
				}
				else if (type == 77)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 221, 1, false, 0);
				}
				else if (type == 86)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 332, 1, false, 0);
				}
				else if (type == 87)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 333, 1, false, 0);
				}
				else if (type == 88)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 334, 1, false, 0);
				}
				else if (type == 89)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 335, 1, false, 0);
				}
				else if (type == 133)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 524, 1, false, 0);
				}

				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
						TileFrame(TileRefs, sandbox, num3, num4);
				}
			}
		}

		public static void Place4x2(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type, int direction = -1)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
				return;

			bool flag = true;
			for (int i = x - 1; i < x + 3; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileRefs(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileRefs(i, y + 1).Active || !Main.tileSolid[(int)TileRefs(i, y + 1).Type])
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
				TileRefs(x - 1, y - 1).SetActive(true);
				TileRefs(x - 1, y - 1).SetFrameY(0);
				TileRefs(x - 1, y - 1).SetFrameX(num);
				TileRefs(x - 1, y - 1).SetType((byte)type);
				TileRefs(x, y - 1).SetActive(true);
				TileRefs(x, y - 1).SetFrameY(0);
				TileRefs(x, y - 1).SetFrameX((short)(18 + num));
				TileRefs(x, y - 1).SetType((byte)type);
				TileRefs(x + 1, y - 1).SetActive(true);
				TileRefs(x + 1, y - 1).SetFrameY(0);
				TileRefs(x + 1, y - 1).SetFrameX((short)(36 + num));
				TileRefs(x + 1, y - 1).SetType((byte)type);
				TileRefs(x + 2, y - 1).SetActive(true);
				TileRefs(x + 2, y - 1).SetFrameY(0);
				TileRefs(x + 2, y - 1).SetFrameX((short)(54 + num));
				TileRefs(x + 2, y - 1).SetType((byte)type);
				TileRefs(x - 1, y).SetActive(true);
				TileRefs(x - 1, y).SetFrameY(18);
				TileRefs(x - 1, y).SetFrameX(num);
				TileRefs(x - 1, y).SetType((byte)type);
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetFrameY(18);
				TileRefs(x, y).SetFrameX((short)(18 + num));
				TileRefs(x, y).SetType((byte)type);
				TileRefs(x + 1, y).SetActive(true);
				TileRefs(x + 1, y).SetFrameY(18);
				TileRefs(x + 1, y).SetFrameX((short)(36 + num));
				TileRefs(x + 1, y).SetType((byte)type);
				TileRefs(x + 2, y).SetActive(true);
				TileRefs(x + 2, y).SetFrameY(18);
				TileRefs(x + 2, y).SetFrameX((short)(54 + num));
				TileRefs(x + 2, y).SetType((byte)type);
			}
		}

		public static void Place2x2(Func<Int32, Int32, ITile> TileRefs, int x, int superY, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
					if (TileRefs(i, j).Active)
					{
						flag = false;
					}
					if (type == 98 && TileRefs(i, j).Liquid > 0)
					{
						flag = false;
					}
				}
				if (type == 95 || type == 126)
				{
					if (!TileRefs(i, y - 2).Active ||
						!Main.tileSolid[(int)TileRefs(i, y - 2).Type] ||
						Main.tileSolidTop[(int)TileRefs(i, y - 2).Type])
					{
						flag = false;
					}
				}
				else
				{
					if (!TileRefs(i, y + 1).Active || (
						!Main.tileSolid[(int)TileRefs(i, y + 1).Type] &&
						!Main.tileTable[(int)TileRefs(i, y + 1).Type]))
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				TileRefs(x - 1, y - 1).SetActive(true);
				TileRefs(x - 1, y - 1).SetFrameY(0);
				TileRefs(x - 1, y - 1).SetFrameX(0);
				TileRefs(x - 1, y - 1).SetType((byte)type);
				TileRefs(x, y - 1).SetActive(true);
				TileRefs(x, y - 1).SetFrameY(0);
				TileRefs(x, y - 1).SetFrameX(18);
				TileRefs(x, y - 1).SetType((byte)type);
				TileRefs(x - 1, y).SetActive(true);
				TileRefs(x - 1, y).SetFrameY(18);
				TileRefs(x - 1, y).SetFrameX(0);
				TileRefs(x - 1, y).SetType((byte)type);
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetFrameY(18);
				TileRefs(x, y).SetFrameX(18);
				TileRefs(x, y).SetType((byte)type);
			}
		}

		public static void Place3x2(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
				return;

			bool flag = true;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileRefs(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileRefs(i, y + 1).Active || !Main.tileSolid[(int)TileRefs(i, y + 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				TileRefs(x - 1, y - 1).SetActive(true);
				TileRefs(x - 1, y - 1).SetFrameY(0);
				TileRefs(x - 1, y - 1).SetFrameX(0);
				TileRefs(x - 1, y - 1).SetType((byte)type);
				TileRefs(x, y - 1).SetActive(true);
				TileRefs(x, y - 1).SetFrameY(0);
				TileRefs(x, y - 1).SetFrameX(18);
				TileRefs(x, y - 1).SetType((byte)type);
				TileRefs(x + 1, y - 1).SetActive(true);
				TileRefs(x + 1, y - 1).SetFrameY(0);
				TileRefs(x + 1, y - 1).SetFrameX(36);
				TileRefs(x + 1, y - 1).SetType((byte)type);
				TileRefs(x - 1, y).SetActive(true);
				TileRefs(x - 1, y).SetFrameY(18);
				TileRefs(x - 1, y).SetFrameX(0);
				TileRefs(x - 1, y).SetType((byte)type);
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetFrameY(18);
				TileRefs(x, y).SetFrameX(18);
				TileRefs(x, y).SetType((byte)type);
				TileRefs(x + 1, y).SetActive(true);
				TileRefs(x + 1, y).SetFrameY(18);
				TileRefs(x + 1, y).SetFrameX(36);
				TileRefs(x + 1, y).SetType((byte)type);
			}
		}

		public static void Check3x3(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = false;
			int num = i;
			num = (int)(TileRefs(i, j).FrameX / 18);
			int num2 = i - num;

			if (num >= 3)
				num -= 3;

			num = i - num;
			int num3 = j + (int)(TileRefs(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num3; l < num3 + 3; l++)
				{
					if (!TileRefs(k, l).Active || (int)TileRefs(k, l).Type != type ||
						(int)TileRefs(k, l).FrameX != (k - num2) * 18 || (int)TileRefs(k, l).FrameY != (l - num3) * 18)
					{
						flag = true;
					}
				}
			}
			if (type == 106)
			{
				for (int m = num; m < num + 3; m++)
				{
					if (!TileRefs(m, num3 + 3).Active || !Main.tileSolid[(int)TileRefs(m, num3 + 3).Type])
					{
						flag = true;
						break;
					}
				}
			}
			else if (!TileRefs(num + 1, num3 - 1).Active || !Main.tileSolid[(int)TileRefs(num + 1, num3 - 1).Type] || Main.tileSolidTop[(int)TileRefs(num + 1, num3 - 1).Type])
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
						if ((int)TileRefs(n, num4).Type == type && TileRefs(n, num4).Active)
							KillTile(TileRefs, sandbox, n, num4);
					}
				}
				if (type == 34)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 106);
				}
				else if (type == 35)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 107);
				}
				else if (type == 36)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 108);
				}
				else if (type == 106)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 363);
				}

				destroyObject = false;

				for (int num5 = num - 1; num5 < num + 4; num5++)
				{
					for (int num6 = num3 - 1; num6 < num3 + 4; num6++)
					{
						TileFrame(TileRefs, sandbox, num5, num6);
					}
				}
			}
		}

		public static void Place3x3(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			bool flag = true;
			int num = 0;
			if (type == 106)
			{
				num = -2;
				for (int i = x - 1; i < x + 2; i++)
				{
					for (int j = y - 2; j < y + 1; j++)
					{
						if (TileRefs(i, j).Active)
						{
							flag = false;
						}
					}
				}
				for (int k = x - 1; k < x + 2; k++)
				{
					if (!TileRefs(k, y + 1).Active || !Main.tileSolid[(int)TileRefs(k, y + 1).Type])
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
						if (TileRefs(l, m).Active)
						{
							flag = false;
						}
					}
				}
				if (!TileRefs(x, y - 1).Active || !Main.tileSolid[(int)TileRefs(x, y - 1).Type] || Main.tileSolidTop[(int)TileRefs(x, y - 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				TileRefs(x - 1, y + num).SetActive(true);
				TileRefs(x - 1, y + num).SetFrameY(0);
				TileRefs(x - 1, y + num).SetFrameX(0);
				TileRefs(x - 1, y + num).SetType((byte)type);
				TileRefs(x, y + num).SetActive(true);
				TileRefs(x, y + num).SetFrameY(0);
				TileRefs(x, y + num).SetFrameX(18);
				TileRefs(x, y + num).SetType((byte)type);
				TileRefs(x + 1, y + num).SetActive(true);
				TileRefs(x + 1, y + num).SetFrameY(0);
				TileRefs(x + 1, y + num).SetFrameX(36);
				TileRefs(x + 1, y + num).SetType((byte)type);
				TileRefs(x - 1, y + 1 + num).SetActive(true);
				TileRefs(x - 1, y + 1 + num).SetFrameY(18);
				TileRefs(x - 1, y + 1 + num).SetFrameX(0);
				TileRefs(x - 1, y + 1 + num).SetType((byte)type);
				TileRefs(x, y + 1 + num).SetActive(true);
				TileRefs(x, y + 1 + num).SetFrameY(18);
				TileRefs(x, y + 1 + num).SetFrameX(18);
				TileRefs(x, y + 1 + num).SetType((byte)type);
				TileRefs(x + 1, y + 1 + num).SetActive(true);
				TileRefs(x + 1, y + 1 + num).SetFrameY(18);
				TileRefs(x + 1, y + 1 + num).SetFrameX(36);
				TileRefs(x + 1, y + 1 + num).SetType((byte)type);
				TileRefs(x - 1, y + 2 + num).SetActive(true);
				TileRefs(x - 1, y + 2 + num).SetFrameY(36);
				TileRefs(x - 1, y + 2 + num).SetFrameX(0);
				TileRefs(x - 1, y + 2 + num).SetType((byte)type);
				TileRefs(x, y + 2 + num).SetActive(true);
				TileRefs(x, y + 2 + num).SetFrameY(36);
				TileRefs(x, y + 2 + num).SetFrameX(18);
				TileRefs(x, y + 2 + num).SetType((byte)type);
				TileRefs(x + 1, y + 2 + num).SetActive(true);
				TileRefs(x + 1, y + 2 + num).SetFrameY(36);
				TileRefs(x + 1, y + 2 + num).SetFrameX(36);
				TileRefs(x + 1, y + 2 + num).SetType((byte)type);
			}
		}

		public static void Check3x4(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = false;
			int num = i + (int)(TileRefs(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileRefs(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 4; l++)
				{
					if (!TileRefs(k, l).Active || (int)TileRefs(k, l).Type != type ||
						(int)TileRefs(k, l).FrameX != (k - num) * 18 || (int)TileRefs(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!TileRefs(k, num2 + 4).Active || !Main.tileSolid[(int)TileRefs(k, num2 + 4).Type])
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
						if ((int)TileRefs(m, n).Type == type && TileRefs(m, n).Active)
						{
							KillTile(TileRefs, sandbox, m, n);
						}
					}
				}
				if (type == 101)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 354);
				}
				else if (type == 102)
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 355);
				}

				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						TileFrame(TileRefs, sandbox, num3, num4);
					}
				}
			}
		}

		public static void Check1xX(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int j, byte type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			int num = j - (int)(TileRefs(x, j).FrameY / 18);
			int frameX = (int)TileRefs(x, j).FrameX;
			int num2 = 3;

			if (type == 92)
				num2 = 6;

			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				if (!TileRefs(x, num + i).Active)
				{
					flag = true;
				}
				else if (TileRefs(x, num + i).Type != type)
				{
					flag = true;
				}
				else if ((int)TileRefs(x, num + i).FrameY != i * 18)
				{
					flag = true;
				}
				else if ((int)TileRefs(x, num + i).FrameX != frameX)
				{
					flag = true;
				}
			}
			if (!TileRefs(x, num + num2).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileRefs(x, num + num2).Type])
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				for (int k = 0; k < num2; k++)
				{
					if (TileRefs(x, num + k).Type == type)
					{
						KillTile(TileRefs, sandbox, x, num + k);
					}
				}
				if (type == 92)
					StorePlayerItem(sandbox, x * 16, j * 16, 32, 32, 341);
				else if (type == 93)
					StorePlayerItem(sandbox, x * 16, j * 16, 32, 32, 342);
				destroyObject = false;
			}
		}

		public static void Check2xX(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y, byte type)
		{
			if (destroyObject)
				return;

			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var frx = (int)TileRefs(x, y).FrameX;
			int num = x, k;
			for (k = frx; k >= 36; k -= 36) { }

			if (k == 18)
				num--;

			int num2 = y - (int)(TileRefs(num, y).FrameY / 18);

			int frameX = (int)TileRefs(num, y).FrameX;
			int num3 = 3;

			if (type == 104)
				num3 = 5;

			bool flag = false;
			for (int l = 0; l < num3; l++)
			{
				if (!TileRefs(num, num2 + l).Active)
				{
					flag = true;
				}
				else if (TileRefs(num, num2 + l).Type != type)
				{
					flag = true;
				}
				else if ((int)TileRefs(num, num2 + l).FrameY != l * 18)
				{
					flag = true;
				}
				else if ((int)TileRefs(num, num2 + l).FrameX != frameX)
				{
					flag = true;
				}

				if (!TileRefs(num + 1, num2 + l).Active)
				{
					flag = true;
				}
				else if (TileRefs(num + 1, num2 + l).Type != type)
				{
					flag = true;
				}
				else if ((int)TileRefs(num + 1, num2 + l).FrameY != l * 18)
				{
					flag = true;
				}
				else if ((int)TileRefs(num + 1, num2 + l).FrameX != frameX + 18)
				{
					flag = true;
				}
			}

			if (!TileRefs(num, num2 + num3).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileRefs(num, num2 + num3).Type])
			{
				flag = true;
			}
			if (!TileRefs(num + 1, num2 + num3).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileRefs(num + 1, num2 + num3).Type])
			{
				flag = true;
			}

			if (flag)
			{
				destroyObject = true;
				for (int m = 0; m < num3; m++)
				{
					if (TileRefs(num, num2 + m).Type == type)
						KillTile(TileRefs, sandbox, num, num2 + m);

					if (TileRefs(num + 1, num2 + m).Type == type)
						KillTile(TileRefs, sandbox, num + 1, num2 + m);
				}

				if (type == 104)
					StorePlayerItem(sandbox, num * 16, y * 16, 32, 32, 359, 1, false, 0);

				if (type == 105)
				{
					int num4 = frameX / 36;

					if (num4 == 0)
						num4 = 360;
					else if (num4 == 1)
						num4 = 52;
					else
						num4 = 438 + num4 - 2;

					StorePlayerItem(sandbox, num * 16, y * 16, 32, 32, num4, 1, false, 0);
				}

				destroyObject = false;
			}
		}

		public static void PlaceSunflower(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type = 27)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if ((double)y > Main.worldSurface - 1.0)
				return;

			bool flag = true;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 3; j < y + 1; j++)
				{
					if (TileRefs(i, j).Active || TileRefs(i, j).Wall > 0)
					{
						flag = false;
					}
				}
				if (!TileRefs(i, y + 1).Active || TileRefs(i, y + 1).Type != 2)
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
						TileRefs(x + k, y + l).SetActive(true);
						TileRefs(x + k, y + l).SetFrameX((short)num);
						TileRefs(x + k, y + l).SetFrameY((short)num2);
						TileRefs(x + k, y + l).SetType((byte)type);
					}
				}
			}
		}

		public static void CheckSunflower(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type = 27)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = false;
			int k = 0;
			k += (int)(TileRefs(i, j).FrameX / 18);
			int num = j + (int)(TileRefs(i, j).FrameY / 18 * -1);

			while (k > 1)
				k -= 2;

			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 4; m++)
				{
					int n;
					for (n = (int)(TileRefs(l, m).FrameX / 18); n > 1; n -= 2)
					{
					}

					if (!TileRefs(l, m).Active || (int)TileRefs(l, m).Type != type || n != l - k || (int)TileRefs(l, m).FrameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!TileRefs(l, num + 4).Active || TileRefs(l, num + 4).Type != 2)
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
						if ((int)TileRefs(num2, num3).Type == type && TileRefs(num2, num3).Active)
						{
							KillTile(TileRefs, sandbox, num2, num3);
						}
					}
				}
				StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, 63);
				destroyObject = false;
			}
		}

		public static bool PlacePot(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type = 28)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			bool flag = true;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileRefs(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileRefs(i, y + 1).Active || !Main.tileSolid[(int)TileRefs(i, y + 1).Type])
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
						TileRefs(x + k, y + l).SetActive(true);
						TileRefs(x + k, y + l).SetFrameX((short)num);
						TileRefs(x + k, y + l).SetFrameY((short)num2);
						TileRefs(x + k, y + l).SetType((byte)type);
					}
				}
				return true;
			}
			return false;
		}

		public static bool CheckCactus(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = j;
			int num2 = i;
			while (TileRefs(num2, num).Active && TileRefs(num2, num).Type == 80)
			{
				num++;
				if (!TileRefs(num2, num).Active || TileRefs(num2, num).Type != 80)
				{
					if (TileRefs(num2 - 1, num).Active && TileRefs(num2 - 1, num).Type == 80 && TileRefs(num2 - 1, num - 1).Active && TileRefs(num2 - 1, num - 1).Type == 80 && num2 >= i)
					{
						num2--;
					}
					if (TileRefs(num2 + 1, num).Active && TileRefs(num2 + 1, num).Type == 80 && TileRefs(num2 + 1, num - 1).Active && TileRefs(num2 + 1, num - 1).Type == 80 && num2 <= i)
					{
						num2++;
					}
				}
			}
			if (!TileRefs(num2, num).Active || TileRefs(num2, num).Type != 53)
			{
				KillTile(TileRefs, sandbox, i, j);
				return true;
			}
			if (i != num2)
			{
				if ((!TileRefs(i, j + 1).Active || TileRefs(i, j + 1).Type != 80) && (!TileRefs(i - 1, j).Active || TileRefs(i - 1, j).Type != 80) && (!TileRefs(i + 1, j).Active || TileRefs(i + 1, j).Type != 80))
				{
					KillTile(TileRefs, sandbox, i, j);
					return true;
				}
			}
			else if (i == num2 && (!TileRefs(i, j + 1).Active || (TileRefs(i, j + 1).Type != 80 && TileRefs(i, j + 1).Type != 53)))
			{
				KillTile(TileRefs, sandbox, i, j);
				return true;
			}
			return false;
		}

		public static void PlantCactus(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			GrowCactus(TileRefs, sandbox, i, j);
			for (int k = 0; k < 150; k++)
			{
				int i2 = genRand.Next(i - 1, i + 2);
				int j2 = genRand.Next(j - 10, j + 2);
				GrowCactus(TileRefs, sandbox, i2, j2);
			}
		}

		public static void CactusFrame(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			try
			{
				int num = j;
				int num2 = i;
				if (!CheckCactus(TileRefs, sandbox, i, j))
				{
					while (TileRefs(num2, num).Active && TileRefs(num2, num).Type == 80)
					{
						num++;
						if (!TileRefs(num2, num).Active || TileRefs(num2, num).Type != 80)
						{
							if (TileRefs(num2 - 1, num).Active && TileRefs(num2 - 1, num).Type == 80 && TileRefs(num2 - 1, num - 1).Active && TileRefs(num2 - 1, num - 1).Type == 80 && num2 >= i)
							{
								num2--;
							}
							if (TileRefs(num2 + 1, num).Active && TileRefs(num2 + 1, num).Type == 80 && TileRefs(num2 + 1, num - 1).Active && TileRefs(num2 + 1, num - 1).Type == 80 && num2 <= i)
							{
								num2++;
							}
						}
					}
					num--;
					int num3 = i - num2;
					num2 = i;
					num = j;
					int type = (int)TileRefs(i - 2, j).Type;
					int num4 = (int)TileRefs(i - 1, j).Type;
					int num5 = (int)TileRefs(i + 1, j).Type;
					int num6 = (int)TileRefs(i, j - 1).Type;
					int num7 = (int)TileRefs(i, j + 1).Type;
					int num8 = (int)TileRefs(i - 1, j + 1).Type;
					int num9 = (int)TileRefs(i + 1, j + 1).Type;

					if (!TileRefs(i - 1, j).Active)
						num4 = -1;

					if (!TileRefs(i + 1, j).Active)
						num5 = -1;

					if (!TileRefs(i, j - 1).Active)
						num6 = -1;

					if (!TileRefs(i, j + 1).Active)
						num7 = -1;

					if (!TileRefs(i - 1, j + 1).Active)
						num8 = -1;

					if (!TileRefs(i + 1, j + 1).Active)
						num9 = -1;

					short num10 = TileRefs(i, j).FrameX;
					short num11 = TileRefs(i, j).FrameY;
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
					if (num10 != TileRefs(i, j).FrameX || num11 != TileRefs(i, j).FrameY)
					{
						TileRefs(i, j).SetFrameX(num10);
						TileRefs(i, j).SetFrameY(num11);
						SquareTileFrame(TileRefs, sandbox, i, j, true);
					}
				}
			}
			catch
			{
				TileRefs(i, j).SetFrameX(0);
				TileRefs(i, j).SetFrameY(0);
			}
		}

		public static void GrowCactus(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = j;
			int num2 = i;
			if (!TileRefs(i, j).Active)
				return;

			if (TileRefs(i, j - 1).Liquid > 0)
				return;

			if (TileRefs(i, j).Type != 53 && TileRefs(i, j).Type != 80)
				return;

			if (TileRefs(i, j).Type == 53)
			{
				if (TileRefs(i, j - 1).Active || TileRefs(i - 1, j - 1).Active ||
					TileRefs(i + 1, j - 1).Active)
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
							if (TileRefs(k, l).Active)
							{
								if (TileRefs(k, l).Type == 80)
								{
									num3++;

									if (num3 >= 4)
										return;
								}

								if (TileRefs(k, l).Type == 53 ||
									TileRefs(k, l).Type == 112 ||
									TileRefs(k, l).Type == 116)
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
					TileRefs(i, j - 1).SetActive(true);
					TileRefs(i, j - 1).SetType(80);

					NetMessage.SendTileSquare(-1, i, j - 1, 1);
					SquareTileFrame(TileRefs, sandbox, num2, num - 1, true);
					return;
				}
				return;
			}
			else
			{
				if (TileRefs(i, j).Type != 80)
					return;

				while (TileRefs(num2, num).Active && TileRefs(num2, num).Type == 80)
				{
					num++;
					if (!TileRefs(num2, num).Active || TileRefs(num2, num).Type != 80)
					{
						if (TileRefs(num2 - 1, num).Active && TileRefs(num2 - 1, num).Type == 80 && TileRefs(num2 - 1, num - 1).Active && TileRefs(num2 - 1, num - 1).Type == 80 && num2 >= i)
						{
							num2--;
						}
						if (TileRefs(num2 + 1, num).Active && TileRefs(num2 + 1, num).Type == 80 && TileRefs(num2 + 1, num - 1).Active && TileRefs(num2 + 1, num - 1).Type == 80 && num2 <= i)
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
						if (TileRefs(m, n).Active && TileRefs(m, n).Type == 80)
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
							if (TileRefs(num2, num - 1).Active)
								return;

							TileRefs(num2, num - 1).SetActive(true);
							TileRefs(num2, num - 1).SetType(80);
							SquareTileFrame(TileRefs, sandbox, num2, num - 1, true);

							NetMessage.SendTileSquare(-1, num2, num - 1, 1);
							return;
						}
						else
						{
							bool flag = false;
							bool flag2 = false;
							if (TileRefs(num2, num - 1).Active && TileRefs(num2, num - 1).Type == 80)
							{
								if (!TileRefs(num2 - 1, num).Active && !TileRefs(num2 - 2, num + 1).Active && !TileRefs(num2 - 1, num - 1).Active && !TileRefs(num2 - 1, num + 1).Active && !TileRefs(num2 - 2, num).Active)
								{
									flag = true;
								}
								if (!TileRefs(num2 + 1, num).Active && !TileRefs(num2 + 2, num + 1).Active && !TileRefs(num2 + 1, num - 1).Active && !TileRefs(num2 + 1, num + 1).Active && !TileRefs(num2 + 2, num).Active)
								{
									flag2 = true;
								}
							}
							int num9 = genRand.Next(3);
							if (num9 == 0 && flag)
							{
								TileRefs(num2 - 1, num).SetActive(true);
								TileRefs(num2 - 1, num).SetType(80);
								SquareTileFrame(TileRefs, sandbox, num2 - 1, num, true);
								NetMessage.SendTileSquare(-1, num2 - 1, num, 1);
								return;
							}
							else if (num9 == 1 && flag2)
							{
								TileRefs(num2 + 1, num).SetActive(true);
								TileRefs(num2 + 1, num).SetType(80);
								SquareTileFrame(TileRefs, sandbox, num2 + 1, num, true);
								NetMessage.SendTileSquare(-1, num2 + 1, num, 1);

								return;
							}
							else
							{
								if (num5 >= genRand.Next(2, 8))
								{
									return;
								}
								//if (TileRefs(num2 - 1, num - 1).Active)
								//{
								//    byte arg_5E0_0 = TileRefs(num2 - 1, num - 1).Type;
								//}
								if (TileRefs(num2 + 1, num - 1).Active && TileRefs(num2 + 1, num - 1).Type == 80)
								{
									return;
								}
								TileRefs(num2, num - 1).SetActive(true);
								TileRefs(num2, num - 1).SetType(80);
								SquareTileFrame(TileRefs, sandbox, num2, num - 1, true);
								NetMessage.SendTileSquare(-1, num2, num - 1, 1);
								return;
							}
						}
					}
					else
					{
						if (TileRefs(num2, num - 1).Active || TileRefs(num2, num - 2).Active ||
							TileRefs(num2 + num6, num - 1).Active ||
							!TileRefs(num2 - num6, num - 1).Active || TileRefs(num2 - num6, num - 1).Type != 80)
							return;

						TileRefs(num2, num - 1).SetActive(true);
						TileRefs(num2, num - 1).SetType(80);
						SquareTileFrame(TileRefs, sandbox, num2, num - 1, true);

						NetMessage.SendTileSquare(-1, num2, num - 1, 1);
						return;
					}
				}
			}
		}

		public static void CheckPot(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type = 28)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = false;
			int k = 0;
			k += (int)(TileRefs(i, j).FrameX / 18);
			int num = j + (int)(TileRefs(i, j).FrameY / 18 * -1);

			while (k > 1)
				k -= 2;

			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 2; m++)
				{
					int n;
					for (n = (int)(TileRefs(l, m).FrameX / 18); n > 1; n -= 2)
					{
					}
					if (!TileRefs(l, m).Active || (int)TileRefs(l, m).Type != type || n != l - k || (int)TileRefs(l, m).FrameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!TileRefs(l, num + 2).Active || !Main.tileSolid[(int)TileRefs(l, num + 2).Type])
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
						if ((int)TileRefs(num2, num3).Type == type && TileRefs(num2, num3).Active)
						{
							KillTile(TileRefs, sandbox, num2, num3);
						}
					}
				}
				if (genRand.Next(40) == 0 && (TileRefs(k, num).Wall == 7 || TileRefs(k, num).Wall == 8 || TileRefs(k, num).Wall == 9))
				{
					StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 327);
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
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 292);
							}
							if (num4 == 1)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 298);
							}
							if (num4 == 2)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 299);
							}
							if (num4 == 3)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 290);
							}
						}
						else if ((double)j < Main.rockLayer)
						{
							int num5 = genRand.Next(7);
							if (num5 == 0)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 289);
							}
							if (num5 == 1)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 298);
							}
							if (num5 == 2)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 299);
							}
							if (num5 == 3)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 290);
							}
							if (num5 == 4)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 303);
							}
							if (num5 == 5)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 291);
							}
							if (num5 == 6)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 304);
							}
						}
						else if (j < Main.maxTilesY - 200)
						{
							int num6 = genRand.Next(10);
							if (num6 == 0)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 296);
							}
							if (num6 == 1)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 295);
							}
							if (num6 == 2)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 299);
							}
							if (num6 == 3)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 302);
							}
							if (num6 == 4)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 303);
							}
							if (num6 == 5)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 305);
							}
							if (num6 == 6)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 301);
							}
							if (num6 == 7)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 302);
							}
							if (num6 == 8)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 297);
							}
							if (num6 == 9)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 304);
							}
						}
						else
						{
							int num7 = genRand.Next(12);
							if (num7 == 0)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 296);
							}
							if (num7 == 1)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 295);
							}
							if (num7 == 2)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 293);
							}
							if (num7 == 3)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 288);
							}
							if (num7 == 4)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 294);
							}
							if (num7 == 5)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 297);
							}
							if (num7 == 6)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 304);
							}
							if (num7 == 7)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 305);
							}
							if (num7 == 8)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 301);
							}
							if (num7 == 9)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 302);
							}
							if (num7 == 10)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 288);
							}
							if (num7 == 11)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 300);
							}
						}
					}
					else
					{
						int num8 = Main.rand.Next(8);
						if (num8 == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLife < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLifeMax)
						{
							StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 58);
						}
						else if (num8 == 1 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statMana < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statManaMax)
						{
							StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 184);
						}
						else if (num8 == 2)
						{
							int stack = Main.rand.Next(1, 6);
							if (TileRefs(i, j).Liquid > 0)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 282, stack);
							}
							else
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 8, stack);
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
							StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, type2, stack2);
						}
						else if (num8 == 4)
						{
							int type3 = 28;
							if (j > Main.maxTilesY - 200 || Main.hardMode)
							{
								type3 = 188;
							}
							StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, type3);
						}
						else if (num8 == 5 && (double)j > Main.rockLayer)
						{
							int stack3 = Main.rand.Next(4) + 1;
							StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 166, stack3);
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
									StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 74, num10);
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
									StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 73, num11);
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
									StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 72, num12);
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
									StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 71, num13);
								}
							}
						}
					}
				}
				destroyObject = false;
			}
		}

		public static void Place1xX(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type, int style = 0)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = style * 18;
			int num2 = 3;

			if (type == 92)
				num2 = 6;

			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (TileRefs(x, i).Active)
					flag = false;

				if (type == 93 && TileRefs(x, i).Liquid > 0)
					flag = false;
			}
			if (flag && TileRefs(x, y + 1).Active && Main.tileSolid[(int)TileRefs(x, y + 1).Type])
			{
				for (int j = 0; j < num2; j++)
				{
					TileRefs(x, y - num2 + 1 + j).SetActive(true);
					TileRefs(x, y - num2 + 1 + j).SetFrameY((short)(j * 18));
					TileRefs(x, y - num2 + 1 + j).SetFrameX((short)num);
					TileRefs(x, y - num2 + 1 + j).SetType((byte)type);
				}
			}
		}

		public static void Place2xX(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type, int style = 0)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = style * 36;
			int num2 = 3;
			if (type == 104)
			{
				num2 = 5;
			}
			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (TileRefs(x, i).Active)
				{
					flag = false;
				}
				if (TileRefs(x + 1, i).Active)
				{
					flag = false;
				}
			}
			if (flag && TileRefs(x, y + 1).Active && Main.tileSolid[(int)TileRefs(x, y + 1).Type] && TileRefs(x + 1, y + 1).Active && Main.tileSolid[(int)TileRefs(x + 1, y + 1).Type])
			{
				for (int j = 0; j < num2; j++)
				{
					TileRefs(x, y - num2 + 1 + j).SetActive(true);
					TileRefs(x, y - num2 + 1 + j).SetFrameY((short)(j * 18));
					TileRefs(x, y - num2 + 1 + j).SetFrameX((short)num);
					TileRefs(x, y - num2 + 1 + j).SetType((byte)type);
					TileRefs(x + 1, y - num2 + 1 + j).SetActive(true);
					TileRefs(x + 1, y - num2 + 1 + j).SetFrameY((short)(j * 18));
					TileRefs(x + 1, y - num2 + 1 + j).SetFrameX((short)(num + 18));
					TileRefs(x + 1, y - num2 + 1 + j).SetType((byte)type);
				}
			}
		}

		public static void Place3x4(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
				return;

			bool flag = true;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 3; j < y + 1; j++)
				{
					if (TileRefs(i, j).Active)
						flag = false;
				}

				if (!TileRefs(i, y + 1).Active || !Main.tileSolid[(int)TileRefs(i, y + 1).Type])
					flag = false;
			}
			if (flag)
			{
				for (int k = -3; k <= 0; k++)
				{
					short frameY = (short)((3 + k) * 18);
					TileRefs(x - 1, y + k).SetActive(true);
					TileRefs(x - 1, y + k).SetFrameY(frameY);
					TileRefs(x - 1, y + k).SetFrameX(0);
					TileRefs(x - 1, y + k).SetType((byte)type);
					TileRefs(x, y + k).SetActive(true);
					TileRefs(x, y + k).SetFrameY(frameY);
					TileRefs(x, y + k).SetFrameX(18);
					TileRefs(x, y + k).SetType((byte)type);
					TileRefs(x + 1, y + k).SetActive(true);
					TileRefs(x + 1, y + k).SetFrameY(frameY);
					TileRefs(x + 1, y + k).SetFrameX(36);
					TileRefs(x + 1, y + k).SetType((byte)type);
				}
			}
		}

		public static bool PlaceTile(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type, bool mute = false, bool forced = false, int plr = -1, int style = 0)
		{
			if (type >= MAX_TILE_SETS)
				return false;

			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			bool result = false;
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (forced || Collision.EmptyTile(i, j, false) || !Main.tileSolid[type] || (type == 23 && TileRefs(i, j).Type == 0 && TileRefs(i, j).Active) || (type == 2 && TileRefs(i, j).Type == 0 && TileRefs(i, j).Active) || (type == 109 && TileRefs(i, j).Type == 0 && TileRefs(i, j).Active) || (type == 60 && TileRefs(i, j).Type == 59 && TileRefs(i, j).Active) || (type == 70 && TileRefs(i, j).Type == 59 && TileRefs(i, j).Active))
				{
					var failed =
						(type == 23 && (TileRefs(i, j).Type != 0 || !TileRefs(i, j).Active))
						||
						(type == 2 && (TileRefs(i, j).Type != 0 || !TileRefs(i, j).Active))
						||
						(type == 109 && (TileRefs(i, j).Type != 0 || !TileRefs(i, j).Active))
						||
						(type == 60 && (TileRefs(i, j).Type != 59 || !TileRefs(i, j).Active));

					if (failed)
						return false;

					if (type == 81)
					{
						if (TileRefs(i - 1, j).Active || TileRefs(i + 1, j).Active || TileRefs(i, j - 1).Active)
							return false;
						if (!TileRefs(i, j + 1).Active || !Main.tileSolid[(int)TileRefs(i, j + 1).Type])
							return false;
					}
					if (TileRefs(i, j).Liquid > 0)
					{
						if (type == 4)
						{
							if (style != 8)
								return false;
						}
						else if (type == 3 || type == 4 || type == 20 || type == 24 || type == 27 || type == 32 || type == 51 || type == 69 || type == 72)
							return false;
					}
					TileRefs(i, j).SetFrameY(0);
					TileRefs(i, j).SetFrameX(0);
					if (type == 3 || type == 24 || type == 110)
					{
						if (j + 1 < Main.maxTilesY && TileRefs(i, j + 1).Active && ((TileRefs(i, j + 1).Type == 2 && type == 3) || (TileRefs(i, j + 1).Type == 23 && type == 24) || (TileRefs(i, j + 1).Type == 78 && type == 3) || (TileRefs(i, j + 1).Type == 109 && type == 110)))
						{
							if (type == 24 && genRand.Next(13) == 0)
							{
								TileRefs(i, j).SetActive(true);
								TileRefs(i, j).SetType(32);
								SquareTileFrame(TileRefs, sandbox, i, j);
							}
							else if (TileRefs(i, j + 1).Type == 78)
							{
								TileRefs(i, j).SetActive(true);
								TileRefs(i, j).SetType((byte)type);
								TileRefs(i, j).SetFrameX((short)(genRand.Next(2) * 18 + 108));
							}
							else if (TileRefs(i, j).Wall == 0 && TileRefs(i, j + 1).Wall == 0)
							{
								if (genRand.Next(50) == 0 || (type == 24 && genRand.Next(40) == 0))
								{
									TileRefs(i, j).SetActive(true);
									TileRefs(i, j).SetType((byte)type);
									TileRefs(i, j).SetFrameX(144);
								}
								else if (genRand.Next(35) == 0)
								{
									TileRefs(i, j).SetActive(true);
									TileRefs(i, j).SetType((byte)type);
									TileRefs(i, j).SetFrameX((short)(genRand.Next(2) * 18 + 108));
								}
								else
								{
									TileRefs(i, j).SetActive(true);
									TileRefs(i, j).SetType((byte)type);
									TileRefs(i, j).SetFrameX((short)(genRand.Next(6) * 18));
								}
							}
						}
					}
					else if (type == 61)
					{
						if (j + 1 < Main.maxTilesY && TileRefs(i, j + 1).Active && TileRefs(i, j + 1).Type == 60)
						{
							if (genRand.Next(16) == 0 && (double)j > Main.worldSurface)
							{
								TileRefs(i, j).SetActive(true);
								TileRefs(i, j).SetType(69);
								SquareTileFrame(TileRefs, sandbox, i, j);
							}
							else if (genRand.Next(60) == 0 && (double)j > Main.rockLayer)
							{
								TileRefs(i, j).SetActive(true);
								TileRefs(i, j).SetType((byte)type);
								TileRefs(i, j).SetFrameX(144);
							}
							else if (genRand.Next(1000) == 0 && (double)j > Main.rockLayer)
							{
								TileRefs(i, j).SetActive(true);
								TileRefs(i, j).SetType((byte)type);
								TileRefs(i, j).SetFrameX(162);
							}
							else if (genRand.Next(15) == 0)
							{
								TileRefs(i, j).SetActive(true);
								TileRefs(i, j).SetType((byte)type);
								TileRefs(i, j).SetFrameX((short)(genRand.Next(2) * 18 + 108));
							}
							else
							{
								TileRefs(i, j).SetActive(true);
								TileRefs(i, j).SetType((byte)type);
								TileRefs(i, j).SetFrameX((short)(genRand.Next(6) * 18));
							}
						}
					}
					else if (type == 71)
					{
						if (j + 1 < Main.maxTilesY && TileRefs(i, j + 1).Active && TileRefs(i, j + 1).Type == 70)
						{
							TileRefs(i, j).SetActive(true);
							TileRefs(i, j).SetType((byte)type);
							TileRefs(i, j).SetFrameX((short)(genRand.Next(5) * 18));
						}
					}
					else if (type == 129)
					{
						if (SolidTile(TileRefs, i - 1, j) || SolidTile(TileRefs, i + 1, j) || SolidTile(TileRefs, i, j - 1) || SolidTile(TileRefs, i, j + 1))
						{
							TileRefs(i, j).SetActive(true);
							TileRefs(i, j).SetType((byte)type);
							TileRefs(i, j).SetFrameX((short)(genRand.Next(8) * 18));
							SquareTileFrame(TileRefs, sandbox, i, j);
						}
					}
					else if (type == 132 || type == 138 || type == 142 || type == 143)
					{
						Place2x2(TileRefs, i, j, type);
					}
					else if (type == 137)
					{
						TileRefs(i, j).SetActive(true);
						TileRefs(i, j).SetType((byte)type);

						if (style == 1)
							TileRefs(i, j).SetFrameX(18);
					}
					else if (type == 136)
					{
						if ((TileRefs(i - 1, j).Active && (Main.tileSolid[(int)TileRefs(i - 1, j).Type] || TileRefs(i - 1, j).Type == 124 || (TileRefs(i - 1, j).Type == 5 && TileRefs(i - 1, j - 1).Type == 5 && TileRefs(i - 1, j + 1).Type == 5))) || (TileRefs(i + 1, j).Active && (Main.tileSolid[(int)TileRefs(i + 1, j).Type] || TileRefs(i + 1, j).Type == 124 || (TileRefs(i + 1, j).Type == 5 && TileRefs(i + 1, j - 1).Type == 5 && TileRefs(i + 1, j + 1).Type == 5))) || (TileRefs(i, j + 1).Active && Main.tileSolid[(int)TileRefs(i, j + 1).Type]))
						{
							TileRefs(i, j).SetActive(true);
							TileRefs(i, j).SetType((byte)type);
							SquareTileFrame(TileRefs, sandbox, i, j);
						}
					}
					else if (type == 4)
					{
						if ((TileRefs(i - 1, j).Active && (Main.tileSolid[(int)TileRefs(i - 1, j).Type] || TileRefs(i - 1, j).Type == 124 || (TileRefs(i - 1, j).Type == 5 && TileRefs(i - 1, j - 1).Type == 5 && TileRefs(i - 1, j + 1).Type == 5))) || (TileRefs(i + 1, j).Active && (Main.tileSolid[(int)TileRefs(i + 1, j).Type] || TileRefs(i + 1, j).Type == 124 || (TileRefs(i + 1, j).Type == 5 && TileRefs(i + 1, j - 1).Type == 5 && TileRefs(i + 1, j + 1).Type == 5))) || (TileRefs(i, j + 1).Active && Main.tileSolid[(int)TileRefs(i, j + 1).Type]))
						{
							TileRefs(i, j).SetActive(true);
							TileRefs(i, j).SetType((byte)type);
							TileRefs(i, j).SetFrameY((short)(22 * style));
							SquareTileFrame(TileRefs, sandbox, i, j);
						}
					}
					else if (type == 10)
					{
						if (!TileRefs(i, j - 1).Active && !TileRefs(i, j - 2).Active && TileRefs(i, j - 3).Active && Main.tileSolid[(int)TileRefs(i, j - 3).Type])
						{
							PlaceDoor(TileRefs, i, j - 1, type);
							SquareTileFrame(TileRefs, sandbox, i, j);
						}
						else
						{
							if (TileRefs(i, j + 1).Active || TileRefs(i, j + 2).Active || !TileRefs(i, j + 3).Active || !Main.tileSolid[(int)TileRefs(i, j + 3).Type])
								return false;

							PlaceDoor(TileRefs, i, j + 1, type);
							SquareTileFrame(TileRefs, sandbox, i, j);
						}
					}
					else if (type == 128)
					{
						PlaceMan(TileRefs, i, j, style);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 149)
					{
						if (SolidTile(TileRefs, i - 1, j) || SolidTile(TileRefs, i + 1, j) ||
							SolidTile(TileRefs, i, j - 1) || SolidTile(TileRefs, i, j + 1))
						{
							TileRefs(i, j).SetFrameX((short)(18 * style));
							TileRefs(i, j).SetActive(true);
							TileRefs(i, j).SetType((byte)type);
							SquareTileFrame(TileRefs, sandbox, i, j);
						}
					}
					else if (type == 139)
					{
						PlaceMB(TileRefs, i, j, type, style);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 34 || type == 35 || type == 36 || type == 106)
					{
						Place3x3(TileRefs, i, j, type);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 13 || type == 33 || type == 49 || type == 50 || type == 78)
					{
						PlaceOnTable1x1(TileRefs, i, j, type, style);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 14 || type == 26 || type == 86 || type == 87 || type == 88 || type == 89 || type == 114)
					{
						Place3x2(TileRefs, i, j, type);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 20)
					{
						if (TileRefs(i, j + 1).Active && (TileRefs(i, j + 1).Type == 2 || TileRefs(i, j + 1).Type == 109 || TileRefs(i, j + 1).Type == 147))
						{
							Place1x2(TileRefs, i, j, type, style);
							SquareTileFrame(TileRefs, sandbox, i, j);
						}
					}
					else if (type == 15)
					{
						Place1x2(TileRefs, i, j, type, style);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 16 || type == 18 || type == 29 || type == 103 || type == 134)
					{
						Place2x1(TileRefs, i, j, type);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 92 || type == 93)
					{
						Place1xX(TileRefs, i, j, type, 0);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 104 || type == 105)
					{
						Place2xX(TileRefs, i, j, type, style);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 17 || type == 77 || type == 133)
					{
						Place3x2(TileRefs, i, j, type);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 21)
					{
						PlaceChest(TileRefs, i, j, type, false, style);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 91)
					{
						PlaceBanner(TileRefs, i, j, type, style);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 135 || type == 141 || type == 144)
					{
						Place1x1(TileRefs, i, j, type, style);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 101 || type == 102)
					{
						Place3x4(TileRefs, i, j, type);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 27)
					{
						PlaceSunflower(TileRefs, i, j, 27);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 28)
					{
						PlacePot(TileRefs, i, j, 28);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 42)
					{
						Place1x2Top(TileRefs, i, j, type);
						SquareTileFrame(TileRefs, sandbox, i, j);
					}
					else if (type == 55 || type == 85)
					{
						PlaceSign(TileRefs, i, j, type);
					}
					else if (Main.tileAlch[type])
					{
						PlaceAlch(TileRefs, i, j, style);
					}
					else if (type == 94 || type == 95 || type == 96 || type == 97 || type == 98 || type == 99 || type == 100 || type == 125 || type == 126)
					{
						Place2x2(TileRefs, i, j, type);
					}
					else if (type == 79 || type == 90)
					{
						int direction = 1;
						if (plr > -1)
							direction = Main.players[plr].direction;

						Place4x2(TileRefs, i, j, type, direction);
					}
					else if (type == 81)
					{
						TileRefs(i, j).SetFrameX((short)(26 * genRand.Next(6)));
						TileRefs(i, j).SetActive(true);
						TileRefs(i, j).SetType((byte)type);
					}
					else
					{
						TileRefs(i, j).SetActive(true);
						TileRefs(i, j).SetType((byte)type);
					}

					if (TileRefs(i, j).Active && !mute)
					{
						SquareTileFrame(TileRefs, sandbox, i, j);
						return true;
					}
				}
			}
			return result;
		}

		public static void KillWall(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, bool fail = false)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (TileRefs(i, j).Wall > 0)
				{
					if (fail)
					{
						SquareWallFrame(TileRefs, i, j, true);
						return;
					}

					int num2 = 0;
					if (TileRefs(i, j).Wall == 1)
					{
						num2 = 26;
					}
					if (TileRefs(i, j).Wall == 4)
					{
						num2 = 93;
					}
					if (TileRefs(i, j).Wall == 5)
					{
						num2 = 130;
					}
					if (TileRefs(i, j).Wall == 6)
					{
						num2 = 132;
					}
					if (TileRefs(i, j).Wall == 7)
					{
						num2 = 135;
					}
					if (TileRefs(i, j).Wall == 8)
					{
						num2 = 138;
					}
					if (TileRefs(i, j).Wall == 9)
					{
						num2 = 140;
					}
					if (TileRefs(i, j).Wall == 10)
					{
						num2 = 142;
					}
					if (TileRefs(i, j).Wall == 11)
					{
						num2 = 144;
					}
					if (TileRefs(i, j).Wall == 12)
					{
						num2 = 146;
					}
					if (TileRefs(i, j).Wall == 14)
					{
						num2 = 330;
					}
					if (TileRefs(i, j).Wall == 16)
					{
						num2 = 30;
					}
					if (TileRefs(i, j).Wall == 17)
					{
						num2 = 135;
					}
					if (TileRefs(i, j).Wall == 18)
					{
						num2 = 138;
					}
					if (TileRefs(i, j).Wall == 19)
					{
						num2 = 140;
					}
					if (TileRefs(i, j).Wall == 20)
					{
						num2 = 330;
					}
					if (TileRefs(i, j).Wall == 21)
					{
						num2 = 392;
					}
					if (TileRefs(i, j).Wall == 22)
					{
						num2 = 417;
					}
					if (TileRefs(i, j).Wall == 23)
					{
						num2 = 418;
					}
					if (TileRefs(i, j).Wall == 24)
					{
						num2 = 419;
					}
					if (TileRefs(i, j).Wall == 25)
					{
						num2 = 420;
					}
					if (TileRefs(i, j).Wall == 26)
					{
						num2 = 421;
					}
					if (TileRefs(i, j).Wall == 29)
					{
						num2 = 587;
					}
					if (TileRefs(i, j).Wall == 30)
					{
						num2 = 592;
					}
					if (TileRefs(i, j).Wall == 31)
					{
						num2 = 595;
					}
					if (TileRefs(i, j).Wall == 27)
					{
						num2 = 479;
					}
					if (num2 > 0)
					{
						StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, num2, 1, false, 0);
					}
					TileRefs(i, j).SetWall(0);
					SquareWallFrame(TileRefs, i, j, true);
				}
			}
		}

		public static void KillTile(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, bool fail = false, bool effectOnly = false, bool noItem = false)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (TileRefs(i, j).Active)
				{
					if (j >= 1 && TileRefs(i, j - 1).Active && ((TileRefs(i, j - 1).Type == 5 && TileRefs(i, j).Type != 5) || (TileRefs(i, j - 1).Type == 21 && TileRefs(i, j).Type != 21) || (TileRefs(i, j - 1).Type == 26 && TileRefs(i, j).Type != 26) || (TileRefs(i, j - 1).Type == 72 && TileRefs(i, j).Type != 72) || (TileRefs(i, j - 1).Type == 12 && TileRefs(i, j).Type != 12)) && (TileRefs(i, j - 1).Type != 5 || ((TileRefs(i, j - 1).FrameX != 66 || TileRefs(i, j - 1).FrameY < 0 || TileRefs(i, j - 1).FrameY > 44) && (TileRefs(i, j - 1).FrameX != 88 || TileRefs(i, j - 1).FrameY < 66 || TileRefs(i, j - 1).FrameY > 110) && TileRefs(i, j - 1).FrameY < 198)))
					{
						return;
					}
					if (!effectOnly && !stopDrops)
					{
						if (TileRefs(i, j).Type == 3 || TileRefs(i, j).Type == 110)
						{
							if (TileRefs(i, j).FrameX == 144)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 5, 1, false, 0);
							}
						}
						else if (TileRefs(i, j).Type == 24)
						{
							if (TileRefs(i, j).FrameX == 144)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 60, 1, false, 0);
							}
						}
					}
					int num = 10;
					if (TileRefs(i, j).Type == 128)
					{
						int num2 = i;
						int k = (int)TileRefs(i, j).FrameX;
						int l;
						for (l = (int)TileRefs(i, j).FrameX; l >= 100; l -= 100) { }

						while (l >= 36)
						{
							l -= 36;
						}
						if (l == 18)
						{
							k = (int)TileRefs(i - 1, j).FrameX;
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
							int num4 = (int)(TileRefs(num2, j).FrameY / 18);
							if (num4 == 0)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, Item.headType[num3], 1, false, 0);
							}
							if (num4 == 1)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, Item.bodyType[num3], 1, false, 0);
							}
							if (num4 == 2)
							{
								StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, Item.legType[num3], 1, false, 0);
							}
							for (k = (int)TileRefs(num2, j).FrameX; k >= 100; k -= 100)
							{
							}
							TileRefs(num2, j).SetFrameX((short)k);
						}
					}
					if (fail)
					{
						num = 3;
					}
					if (TileRefs(i, j).Type == 138)
					{
						num = 0;
					}
					for (int m = 0; m < num; m++)
					{
						int num5 = 0;
						if (TileRefs(i, j).Type == 0)
						{
							num5 = 0;
						}
						if (TileRefs(i, j).Type == 1 || TileRefs(i, j).Type == 16 || TileRefs(i, j).Type == 17 || TileRefs(i, j).Type == 38 || TileRefs(i, j).Type == 39 || TileRefs(i, j).Type == 41 || TileRefs(i, j).Type == 43 || TileRefs(i, j).Type == 44 || TileRefs(i, j).Type == 48 || Main.tileStone[(int)TileRefs(i, j).Type] || TileRefs(i, j).Type == 85 || TileRefs(i, j).Type == 90 || TileRefs(i, j).Type == 92 || TileRefs(i, j).Type == 96 || TileRefs(i, j).Type == 97 || TileRefs(i, j).Type == 99 || TileRefs(i, j).Type == 105 || TileRefs(i, j).Type == 117 || TileRefs(i, j).Type == 130 || TileRefs(i, j).Type == 131 || TileRefs(i, j).Type == 132 || TileRefs(i, j).Type == 135 || TileRefs(i, j).Type == 135 || TileRefs(i, j).Type == 137 || TileRefs(i, j).Type == 142 || TileRefs(i, j).Type == 143 || TileRefs(i, j).Type == 144)
						{
							num5 = 1;
						}
						if (TileRefs(i, j).Type == 33 || TileRefs(i, j).Type == 95 || TileRefs(i, j).Type == 98 || TileRefs(i, j).Type == 100)
						{
							num5 = 6;
						}
						if (TileRefs(i, j).Type == 5 || TileRefs(i, j).Type == 10 || TileRefs(i, j).Type == 11 || TileRefs(i, j).Type == 14 || TileRefs(i, j).Type == 15 || TileRefs(i, j).Type == 19 || TileRefs(i, j).Type == 30 || TileRefs(i, j).Type == 86 || TileRefs(i, j).Type == 87 || TileRefs(i, j).Type == 88 || TileRefs(i, j).Type == 89 || TileRefs(i, j).Type == 93 || TileRefs(i, j).Type == 94 || TileRefs(i, j).Type == 104 || TileRefs(i, j).Type == 106 || TileRefs(i, j).Type == 114 || TileRefs(i, j).Type == 124 || TileRefs(i, j).Type == 128 || TileRefs(i, j).Type == 139)
						{
							num5 = 7;
						}
						if (TileRefs(i, j).Type == 21)
						{
							if (TileRefs(i, j).FrameX >= 108)
							{
								num5 = 37;
							}
							else
							{
								if (TileRefs(i, j).FrameX >= 36)
								{
									num5 = 10;
								}
								else
								{
									num5 = 7;
								}
							}
						}
						if (TileRefs(i, j).Type == 2)
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
						if (TileRefs(i, j).Type == 127)
						{
							num5 = 67;
						}
						if (TileRefs(i, j).Type == 91)
						{
							num5 = -1;
						}
						if (TileRefs(i, j).Type == 6 || TileRefs(i, j).Type == 26)
						{
							num5 = 8;
						}
						if (TileRefs(i, j).Type == 7 || TileRefs(i, j).Type == 34 || TileRefs(i, j).Type == 47)
						{
							num5 = 9;
						}
						if (TileRefs(i, j).Type == 8 || TileRefs(i, j).Type == 36 || TileRefs(i, j).Type == 45 || TileRefs(i, j).Type == 102)
						{
							num5 = 10;
						}
						if (TileRefs(i, j).Type == 9 || TileRefs(i, j).Type == 35 || TileRefs(i, j).Type == 42 || TileRefs(i, j).Type == 46 || TileRefs(i, j).Type == 126 || TileRefs(i, j).Type == 136)
						{
							num5 = 11;
						}
						if (TileRefs(i, j).Type == 12)
						{
							num5 = 12;
						}
						if (TileRefs(i, j).Type == 3 || TileRefs(i, j).Type == 73)
						{
							num5 = 3;
						}
						if (TileRefs(i, j).Type == 13 || TileRefs(i, j).Type == 54)
						{
							num5 = 13;
						}
						if (TileRefs(i, j).Type == 22 || TileRefs(i, j).Type == 140)
						{
							num5 = 14;
						}
						if (TileRefs(i, j).Type == 28 || TileRefs(i, j).Type == 78)
						{
							num5 = 22;
						}
						if (TileRefs(i, j).Type == 29)
						{
							num5 = 23;
						}
						if (TileRefs(i, j).Type == 40 || TileRefs(i, j).Type == 103)
						{
							num5 = 28;
						}
						if (TileRefs(i, j).Type == 49)
						{
							num5 = 29;
						}
						if (TileRefs(i, j).Type == 50)
						{
							num5 = 22;
						}
						if (TileRefs(i, j).Type == 51)
						{
							num5 = 30;
						}
						if (TileRefs(i, j).Type == 52)
						{
							num5 = 3;
						}
						if (TileRefs(i, j).Type == 53 || TileRefs(i, j).Type == 81)
						{
							num5 = 32;
						}
						if (TileRefs(i, j).Type == 56 || TileRefs(i, j).Type == 75)
						{
							num5 = 37;
						}
						if (TileRefs(i, j).Type == 57 || TileRefs(i, j).Type == 119 || TileRefs(i, j).Type == 141)
						{
							num5 = 36;
						}
						if (TileRefs(i, j).Type == 59 || TileRefs(i, j).Type == 120)
						{
							num5 = 38;
						}
						if (TileRefs(i, j).Type == 61 || TileRefs(i, j).Type == 62 || TileRefs(i, j).Type == 74 || TileRefs(i, j).Type == 80)
						{
							num5 = 40;
						}
						if (TileRefs(i, j).Type == 69)
						{
							num5 = 7;
						}
						if (TileRefs(i, j).Type == 71 || TileRefs(i, j).Type == 72)
						{
							num5 = 26;
						}
						if (TileRefs(i, j).Type == 70)
						{
							num5 = 17;
						}
						if (TileRefs(i, j).Type == 112)
						{
							num5 = 14;
						}
						if (TileRefs(i, j).Type == 123)
						{
							num5 = 53;
						}
						if (TileRefs(i, j).Type == 116 || TileRefs(i, j).Type == 118 || TileRefs(i, j).Type == 147 || TileRefs(i, j).Type == 148)
						{
							num5 = 51;
						}
						if (TileRefs(i, j).Type == 109)
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
						if (TileRefs(i, j).Type == 110 || TileRefs(i, j).Type == 113 || TileRefs(i, j).Type == 115)
						{
							num5 = 47;
						}
						if (TileRefs(i, j).Type == 107 || TileRefs(i, j).Type == 121)
						{
							num5 = 48;
						}
						if (TileRefs(i, j).Type == 108 || TileRefs(i, j).Type == 122 || TileRefs(i, j).Type == 134 || TileRefs(i, j).Type == 146)
						{
							num5 = 49;
						}
						if (TileRefs(i, j).Type == 111 || TileRefs(i, j).Type == 133 || TileRefs(i, j).Type == 145)
						{
							num5 = 50;
						}
						if (TileRefs(i, j).Type == 149)
						{
							num5 = 49;
						}
						if (Main.tileAlch[(int)TileRefs(i, j).Type])
						{
							int num6 = (int)(TileRefs(i, j).FrameX / 18);
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
						if (TileRefs(i, j).Type == 61)
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
						if (TileRefs(i, j).Type == 58 || TileRefs(i, j).Type == 76 || TileRefs(i, j).Type == 77)
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
						if (TileRefs(i, j).Type == 37)
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
						if (TileRefs(i, j).Type == 32)
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
						if (TileRefs(i, j).Type == 23 || TileRefs(i, j).Type == 24)
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
						if (TileRefs(i, j).Type == 25 || TileRefs(i, j).Type == 31)
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
						if (TileRefs(i, j).Type == 20)
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
						if (TileRefs(i, j).Type == 27)
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
						if (TileRefs(i, j).Type == 129)
						{
							if (TileRefs(i, j).FrameX == 0 || TileRefs(i, j).FrameX == 54 || TileRefs(i, j).FrameX == 108)
							{
								num5 = 68;
							}
							else
							{
								if (TileRefs(i, j).FrameX == 18 || TileRefs(i, j).FrameX == 72 || TileRefs(i, j).FrameX == 126)
								{
									num5 = 69;
								}
								else
								{
									num5 = 70;
								}
							}
						}
						if (TileRefs(i, j).Type == 4)
						{
							int num7 = (int)(TileRefs(i, j).FrameY / 22);
							if (num7 == 0)
							{
								num5 = 6;
							}
							else
							{
								if (num7 == 8)
								{
									num5 = 75;
								}
								else
								{
									num5 = 58 + num7;
								}
							}
						}
						if ((TileRefs(i, j).Type == 34 || TileRefs(i, j).Type == 35 || TileRefs(i, j).Type == 36 || TileRefs(i, j).Type == 42) && Main.rand.Next(2) == 0)
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
						if (TileRefs(i, j).Type == 2 || TileRefs(i, j).Type == 23 || TileRefs(i, j).Type == 109)
						{
							TileRefs(i, j).SetType(0);
						}
						if (TileRefs(i, j).Type == 60 || TileRefs(i, j).Type == 70)
						{
							TileRefs(i, j).SetType(59);
						}
						SquareTileFrame(TileRefs, sandbox, i, j, true);
						return;
					}
					if (TileRefs(i, j).Type == 21)
					{
						int n = (int)(TileRefs(i, j).FrameX / 18);
						int y = j - (int)(TileRefs(i, j).FrameY / 18);
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
						if (TileRefs(i, j).Type == 0 || TileRefs(i, j).Type == 2 || TileRefs(i, j).Type == 109)
						{
							num8 = 2;
						}
						else
						{
							if (TileRefs(i, j).Type == 1)
							{
								num8 = 3;
							}
							else
							{
								if (TileRefs(i, j).Type == 3 || TileRefs(i, j).Type == 73)
								{
									if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].HasItem(281))
									{
										num8 = 283;
									}
								}
								else
								{
									if (TileRefs(i, j).Type == 4)
									{
										int num9 = (int)(TileRefs(i, j).FrameY / 22);
										if (num9 == 0)
										{
											num8 = 8;
										}
										else
										{
											if (num9 == 8)
											{
												num8 = 523;
											}
											else
											{
												num8 = 426 + num9;
											}
										}
									}
									else
									{
										if (TileRefs(i, j).Type == 5)
										{
											if (TileRefs(i, j).FrameX >= 22 && TileRefs(i, j).FrameY >= 198)
											{
												//if (Main.netMode != 1)
												{
													if (genRand.Next(2) == 0)
													{
														int num10 = j;
														while ((!TileRefs(i, num10).Active || !Main.tileSolid[(int)TileRefs(i, num10).Type] || Main.tileSolidTop[(int)TileRefs(i, num10).Type]))
														{
															num10++;
														}
														//if (TileRefs(i, num10] != null)
														{
															if (TileRefs(i, num10).Type == 2 || TileRefs(i, num10).Type == 109)
															{
																num8 = 27;
															}
															else
															{
																num8 = 9;
															}
														}
													}
													else
													{
														num8 = 9;
													}
												}
											}
											else
											{
												num8 = 9;
											}
										}
										else
										{
											if (TileRefs(i, j).Type == 6)
											{
												num8 = 11;
											}
											else
											{
												if (TileRefs(i, j).Type == 7)
												{
													num8 = 12;
												}
												else
												{
													if (TileRefs(i, j).Type == 8)
													{
														num8 = 13;
													}
													else
													{
														if (TileRefs(i, j).Type == 9)
														{
															num8 = 14;
														}
														else
														{
															if (TileRefs(i, j).Type == 123)
															{
																num8 = 424;
															}
															else
															{
																if (TileRefs(i, j).Type == 124)
																{
																	num8 = 480;
																}
																else
																{
																	if (TileRefs(i, j).Type == 149)
																	{
																		if (TileRefs(i, j).FrameX == 0 || TileRefs(i, j).FrameX == 54)
																		{
																			num8 = 596;
																		}
																		else
																		{
																			if (TileRefs(i, j).FrameX == 18 || TileRefs(i, j).FrameX == 72)
																			{
																				num8 = 597;
																			}
																			else
																			{
																				if (TileRefs(i, j).FrameX == 36 || TileRefs(i, j).FrameX == 90)
																				{
																					num8 = 598;
																				}
																			}
																		}
																	}
																	else
																	{
																		if (TileRefs(i, j).Type == 13)
																		{
																			if (TileRefs(i, j).FrameX == 18)
																			{
																				num8 = 28;
																			}
																			else
																			{
																				if (TileRefs(i, j).FrameX == 36)
																				{
																					num8 = 110;
																				}
																				else
																				{
																					if (TileRefs(i, j).FrameX == 54)
																					{
																						num8 = 350;
																					}
																					else
																					{
																						if (TileRefs(i, j).FrameX == 72)
																						{
																							num8 = 351;
																						}
																						else
																						{
																							num8 = 31;
																						}
																					}
																				}
																			}
																		}
																		else
																		{
																			if (TileRefs(i, j).Type == 19)
																			{
																				num8 = 94;
																			}
																			else
																			{
																				if (TileRefs(i, j).Type == 22)
																				{
																					num8 = 56;
																				}
																				else
																				{
																					if (TileRefs(i, j).Type == 140)
																					{
																						num8 = 577;
																					}
																					else
																					{
																						if (TileRefs(i, j).Type == 23)
																						{
																							num8 = 2;
																						}
																						else
																						{
																							if (TileRefs(i, j).Type == 25)
																							{
																								num8 = 61;
																							}
																							else
																							{
																								if (TileRefs(i, j).Type == 30)
																								{
																									num8 = 9;
																								}
																								else
																								{
																									if (TileRefs(i, j).Type == 33)
																									{
																										num8 = 105;
																									}
																									else
																									{
																										if (TileRefs(i, j).Type == 37)
																										{
																											num8 = 116;
																										}
																										else
																										{
																											if (TileRefs(i, j).Type == 38)
																											{
																												num8 = 129;
																											}
																											else
																											{
																												if (TileRefs(i, j).Type == 39)
																												{
																													num8 = 131;
																												}
																												else
																												{
																													if (TileRefs(i, j).Type == 40)
																													{
																														num8 = 133;
																													}
																													else
																													{
																														if (TileRefs(i, j).Type == 41)
																														{
																															num8 = 134;
																														}
																														else
																														{
																															if (TileRefs(i, j).Type == 43)
																															{
																																num8 = 137;
																															}
																															else
																															{
																																if (TileRefs(i, j).Type == 44)
																																{
																																	num8 = 139;
																																}
																																else
																																{
																																	if (TileRefs(i, j).Type == 45)
																																	{
																																		num8 = 141;
																																	}
																																	else
																																	{
																																		if (TileRefs(i, j).Type == 46)
																																		{
																																			num8 = 143;
																																		}
																																		else
																																		{
																																			if (TileRefs(i, j).Type == 47)
																																			{
																																				num8 = 145;
																																			}
																																			else
																																			{
																																				if (TileRefs(i, j).Type == 48)
																																				{
																																					num8 = 147;
																																				}
																																				else
																																				{
																																					if (TileRefs(i, j).Type == 49)
																																					{
																																						num8 = 148;
																																					}
																																					else
																																					{
																																						if (TileRefs(i, j).Type == 51)
																																						{
																																							num8 = 150;
																																						}
																																						else
																																						{
																																							if (TileRefs(i, j).Type == 53)
																																							{
																																								num8 = 169;
																																							}
																																							else
																																							{
																																								if (TileRefs(i, j).Type == 54)
																																								{
																																									num8 = 170;
																																								}
																																								else
																																								{
																																									if (TileRefs(i, j).Type == 56)
																																									{
																																										num8 = 173;
																																									}
																																									else
																																									{
																																										if (TileRefs(i, j).Type == 57)
																																										{
																																											num8 = 172;
																																										}
																																										else
																																										{
																																											if (TileRefs(i, j).Type == 58)
																																											{
																																												num8 = 174;
																																											}
																																											else
																																											{
																																												if (TileRefs(i, j).Type == 60)
																																												{
																																													num8 = 176;
																																												}
																																												else
																																												{
																																													if (TileRefs(i, j).Type == 70)
																																													{
																																														num8 = 176;
																																													}
																																													else
																																													{
																																														if (TileRefs(i, j).Type == 75)
																																														{
																																															num8 = 192;
																																														}
																																														else
																																														{
																																															if (TileRefs(i, j).Type == 76)
																																															{
																																																num8 = 214;
																																															}
																																															else
																																															{
																																																if (TileRefs(i, j).Type == 78)
																																																{
																																																	num8 = 222;
																																																}
																																																else
																																																{
																																																	if (TileRefs(i, j).Type == 81)
																																																	{
																																																		num8 = 275;
																																																	}
																																																	else
																																																	{
																																																		if (TileRefs(i, j).Type == 80)
																																																		{
																																																			num8 = 276;
																																																		}
																																																		else
																																																		{
																																																			if (TileRefs(i, j).Type == 107)
																																																			{
																																																				num8 = 364;
																																																			}
																																																			else
																																																			{
																																																				if (TileRefs(i, j).Type == 108)
																																																				{
																																																					num8 = 365;
																																																				}
																																																				else
																																																				{
																																																					if (TileRefs(i, j).Type == 111)
																																																					{
																																																						num8 = 366;
																																																					}
																																																					else
																																																					{
																																																						if (TileRefs(i, j).Type == 112)
																																																						{
																																																							num8 = 370;
																																																						}
																																																						else
																																																						{
																																																							if (TileRefs(i, j).Type == 116)
																																																							{
																																																								num8 = 408;
																																																							}
																																																							else
																																																							{
																																																								if (TileRefs(i, j).Type == 117)
																																																								{
																																																									num8 = 409;
																																																								}
																																																								else
																																																								{
																																																									if (TileRefs(i, j).Type == 129)
																																																									{
																																																										num8 = 502;
																																																									}
																																																									else
																																																									{
																																																										if (TileRefs(i, j).Type == 118)
																																																										{
																																																											num8 = 412;
																																																										}
																																																										else
																																																										{
																																																											if (TileRefs(i, j).Type == 119)
																																																											{
																																																												num8 = 413;
																																																											}
																																																											else
																																																											{
																																																												if (TileRefs(i, j).Type == 120)
																																																												{
																																																													num8 = 414;
																																																												}
																																																												else
																																																												{
																																																													if (TileRefs(i, j).Type == 121)
																																																													{
																																																														num8 = 415;
																																																													}
																																																													else
																																																													{
																																																														if (TileRefs(i, j).Type == 122)
																																																														{
																																																															num8 = 416;
																																																														}
																																																														else
																																																														{
																																																															if (TileRefs(i, j).Type == 136)
																																																															{
																																																																num8 = 538;
																																																															}
																																																															else
																																																															{
																																																																if (TileRefs(i, j).Type == 137)
																																																																{
																																																																	num8 = 539;
																																																																}
																																																																else
																																																																{
																																																																	if (TileRefs(i, j).Type == 141)
																																																																	{
																																																																		num8 = 580;
																																																																	}
																																																																	else
																																																																	{
																																																																		if (TileRefs(i, j).Type == 145)
																																																																		{
																																																																			num8 = 586;
																																																																		}
																																																																		else
																																																																		{
																																																																			if (TileRefs(i, j).Type == 146)
																																																																			{
																																																																				num8 = 591;
																																																																			}
																																																																			else
																																																																			{
																																																																				if (TileRefs(i, j).Type == 147)
																																																																				{
																																																																					num8 = 593;
																																																																				}
																																																																				else
																																																																				{
																																																																					if (TileRefs(i, j).Type == 148)
																																																																					{
																																																																						num8 = 594;
																																																																					}
																																																																					else
																																																																					{
																																																																						if (TileRefs(i, j).Type == 135)
																																																																						{
																																																																							if (TileRefs(i, j).FrameY == 0)
																																																																							{
																																																																								num8 = 529;
																																																																							}
																																																																							if (TileRefs(i, j).FrameY == 18)
																																																																							{
																																																																								num8 = 541;
																																																																							}
																																																																							if (TileRefs(i, j).FrameY == 36)
																																																																							{
																																																																								num8 = 542;
																																																																							}
																																																																							if (TileRefs(i, j).FrameY == 54)
																																																																							{
																																																																								num8 = 543;
																																																																							}
																																																																						}
																																																																						else
																																																																						{
																																																																							if (TileRefs(i, j).Type == 144)
																																																																							{
																																																																								if (TileRefs(i, j).FrameX == 0)
																																																																								{
																																																																									num8 = 583;
																																																																								}
																																																																								if (TileRefs(i, j).FrameX == 18)
																																																																								{
																																																																									num8 = 584;
																																																																								}
																																																																								if (TileRefs(i, j).FrameX == 36)
																																																																								{
																																																																									num8 = 585;
																																																																								}
																																																																							}
																																																																							else
																																																																							{
																																																																								if (TileRefs(i, j).Type == 130)
																																																																								{
																																																																									num8 = 511;
																																																																								}
																																																																								else
																																																																								{
																																																																									if (TileRefs(i, j).Type == 131)
																																																																									{
																																																																										num8 = 512;
																																																																									}
																																																																									else
																																																																									{
																																																																										if (TileRefs(i, j).Type == 61 || TileRefs(i, j).Type == 74)
																																																																										{
																																																																											if (TileRefs(i, j).FrameX == 144)
																																																																											{
																																																																												StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 331, genRand.Next(2, 4), false, 0);
																																																																											}
																																																																											else
																																																																											{
																																																																												if (TileRefs(i, j).FrameX == 162)
																																																																												{
																																																																													num8 = 223;
																																																																												}
																																																																												else
																																																																												{
																																																																													if (TileRefs(i, j).FrameX >= 108 && TileRefs(i, j).FrameX <= 126 && genRand.Next(100) == 0)
																																																																													{
																																																																														num8 = 208;
																																																																													}
																																																																													else
																																																																													{
																																																																														if (genRand.Next(100) == 0)
																																																																														{
																																																																															num8 = 195;
																																																																														}
																																																																													}
																																																																												}
																																																																											}
																																																																										}
																																																																										else
																																																																										{
																																																																											if (TileRefs(i, j).Type == 59 || TileRefs(i, j).Type == 60)
																																																																											{
																																																																												num8 = 176;
																																																																											}
																																																																											else
																																																																											{
																																																																												if (TileRefs(i, j).Type == 71 || TileRefs(i, j).Type == 72)
																																																																												{
																																																																													if (genRand.Next(50) == 0)
																																																																													{
																																																																														num8 = 194;
																																																																													}
																																																																													else
																																																																													{
																																																																														if (genRand.Next(2) == 0)
																																																																														{
																																																																															num8 = 183;
																																																																														}
																																																																													}
																																																																												}
																																																																												else
																																																																												{
																																																																													if (TileRefs(i, j).Type >= 63 && TileRefs(i, j).Type <= 68)
																																																																													{
																																																																														num8 = (int)(TileRefs(i, j).Type - 63 + 177);
																																																																													}
																																																																													else
																																																																													{
																																																																														if (TileRefs(i, j).Type == 50)
																																																																														{
																																																																															if (TileRefs(i, j).FrameX == 90)
																																																																															{
																																																																																num8 = 165;
																																																																															}
																																																																															else
																																																																															{
																																																																																num8 = 149;
																																																																															}
																																																																														}
																																																																														else
																																																																														{
																																																																															if (Main.tileAlch[(int)TileRefs(i, j).Type] && TileRefs(i, j).Type > 82)
																																																																															{
																																																																																int num11 = (int)(TileRefs(i, j).FrameX / 18);
																																																																																bool flag = false;
																																																																																if (TileRefs(i, j).Type == 84)
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
																																																																																	StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 307 + num11, genRand.Next(1, 4), false, 0);
																																																																																}
																																																																															}
																																																																														}
																																																																													}
																																																																												}
																																																																											}
																																																																										}
																																																																									}
																																																																								}
																																																																							}
																																																																						}
																																																																					}
																																																																				}
																																																																			}
																																																																		}
																																																																	}
																																																																}
																																																															}
																																																														}
																																																													}
																																																												}
																																																											}
																																																										}
																																																									}
																																																								}
																																																							}
																																																						}
																																																					}
																																																				}
																																																			}
																																																		}
																																																	}
																																																}
																																															}
																																														}
																																													}
																																												}
																																											}
																																										}
																																									}
																																								}
																																							}
																																						}
																																					}
																																				}
																																			}
																																		}
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
						if (num8 > 0)
						{
							StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, num8, 1, false, -1);
						}
					}
					TileRefs(i, j).SetActive(false);
					TileRefs(i, j).SetFrameX(-1);
					TileRefs(i, j).SetFrameY(-1);
					TileRefs(i, j).SetFrameNumber(0);
					if (TileRefs(i, j).Type == 58 && j > Main.maxTilesY - 200)
					{
						TileRefs(i, j).SetLava(true);
						TileRefs(i, j).SetLiquid(128);
					}
					TileRefs(i, j).SetType(0);
					SquareTileFrame(TileRefs, sandbox, i, j, true);
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

		public static int totalD, totalX;
		public static void UpdateWorld(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, ISender Sender)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			UpdateMech(TileRefs, sandbox, Sender);
			totalD++;
			if (totalD >= 10)
			{
				totalD = 0;
				CountTiles(TileRefs, totalX);
				totalX++;
				if (totalX >= Main.maxTilesX)
				{
					totalX = 0;
				}
			}

			Liquid.skipCount++;
			if (Liquid.skipCount > 1)
			{
				bool buffer = false;
				if (Program.properties.BufferLiquidUpdates)
					NetMessage.UseLiquidUpdateBuffer = buffer = true;

				try
				{
					Liquid.UpdateLiquid(TileRefs, sandbox);
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

			ITile Tile;
			ITile Tile2;
			while ((float)num3 < (float)(Main.maxTilesX * Main.maxTilesY) * num)
			{
				int TileX = genRand.Next(10, Main.maxTilesX - 10);
				int TileY = genRand.Next(10, (int)Main.worldSurface - 1);
				Tile = TileRefs(TileX, TileY);
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

				Tile2 = TileRefs(TileX, rTileY);

				if (true)
				{
					if (Main.tileAlch[(int)Tile.Type])
					{
						GrowAlch(TileRefs, sandbox, TileX, TileY);
					}
					if (Tile.Liquid > 32)
					{
						if (Tile.Active && (Tile.Type == 3 || Tile.Type == 20 || Tile.Type == 24 || Tile.Type == 27 || Tile.Type == 73))
						{
							KillTile(TileRefs, sandbox, TileX, TileY);
							NetMessage.SendData(17, -1, -1, "", 0, (float)TileX, (float)TileY);
						}
					}
					else if (Tile.Active)
					{
						UpdateWorld_Hardmode(TileRefs, sandbox, TileX, TileY);
						if (Tile.Type == 80)
						{
							if (genRand.Next(15) == 0)
							{
								GrowCactus(TileRefs, sandbox, TileX, TileY);
							}
						}
						else if (Tile.Type == 53)
						{
							if (!Tile2.Active)
							{
								if (TileX < 250 || TileX > Main.maxTilesX - 250)
								{
									if (genRand.Next(500) == 0 && Tile2.Liquid == 255 && TileRefs(TileX, rTileY - 1).Liquid == 255 && TileRefs(TileX, rTileY - 2).Liquid == 255 && TileRefs(TileX, rTileY - 3).Liquid == 255 && TileRefs(TileX, rTileY - 4).Liquid == 255)
									{
										PlaceTile(TileRefs, sandbox, TileX, rTileY, 81, true, false, -1, 0);
										if (Tile2.Active)
										{
											NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
										}
									}
								}
								else if (TileX > 400 && TileX < Main.maxTilesX - 400 && genRand.Next(300) == 0)
								{
									GrowCactus(TileRefs, sandbox, TileX, TileY);
								}
							}
						}
						else if (Tile.Type == 78)
						{
							if (!Tile2.Active)
							{
								PlaceTile(TileRefs, sandbox, TileX, rTileY, 3, true, false, -1, 0);
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
								PlaceTile(TileRefs, sandbox, TileX, rTileY, 3, true, false, -1, 0);
								if (Tile2.Active)
								{
									NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
								}
							}
							if (!Tile2.Active && genRand.Next(10) == 0 && num10 == 23)
							{
								PlaceTile(TileRefs, sandbox, TileX, rTileY, 24, true, false, -1, 0);
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
									if ((TileX != j || TileY != k) && TileRefs(j, k).Active)
									{
										if (num10 == 32)
										{
											num10 = 23;
										}
										if (TileRefs(j, k).Type == 0 || (num10 == 23 && TileRefs(j, k).Type == 2) || (num10 == 23 && TileRefs(j, k).Type == 109))
										{
											SpreadGrass(TileRefs, j, k, 0, num10, false);
											if (num10 == 23)
											{
												SpreadGrass(TileRefs, j, k, 2, num10, false);
												SpreadGrass(TileRefs, j, k, 109, num10, false);
											}
											if ((int)TileRefs(j, k).Type == num10)
											{
												SquareTileFrame(TileRefs, sandbox, j, k, true);
												flag2 = true;
											}
										}
									}
									if (TileRefs(j, k).Type == 0 || (num10 == 109 && TileRefs(j, k).Type == 2) || (num10 == 109 && TileRefs(j, k).Type == 23))
									{
										SpreadGrass(TileRefs, j, k, 0, num10, false);
										if (num10 == 109)
										{
											SpreadGrass(TileRefs, j, k, 2, num10, false);
											SpreadGrass(TileRefs, j, k, 23, num10, false);
										}
										if ((int)TileRefs(j, k).Type == num10)
										{
											SquareTileFrame(TileRefs, sandbox, j, k, true);
											flag2 = true;
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
							WorldGen.GrowTree(TileRefs, sandbox, TileX, TileY);
						}
						if (Tile.Type == 3 && genRand.Next(20) == 0 && Tile.FrameX < 144)
						{
							Tile.SetType(73);
							NetMessage.SendTileSquare(-1, TileX, TileY, 3);
						}
						if (Tile.Type == 110 && genRand.Next(20) == 0 && Tile.FrameX < 144)
						{
							Tile.SetType(113);
							NetMessage.SendTileSquare(-1, TileX, TileY, 3);
						}
						if (Tile.Type == 32 && genRand.Next(3) == 0)
						{
							int num11 = TileX;
							int num12 = TileY;
							int num13 = 0;
							if (TileRefs(num11 + 1, num12).Active && TileRefs(num11 + 1, num12).Type == 32)
							{
								num13++;
							}
							if (TileRefs(num11 - 1, num12).Active && TileRefs(num11 - 1, num12).Type == 32)
							{
								num13++;
							}
							if (TileRefs(num11, num12 + 1).Active && TileRefs(num11, num12 + 1).Type == 32)
							{
								num13++;
							}
							if (TileRefs(num11, num12 - 1).Active && TileRefs(num11, num12 - 1).Type == 32)
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
								if (!TileRefs(num11, num12).Active)
								{
									num13 = 0;
									if (TileRefs(num11 + 1, num12).Active && TileRefs(num11 + 1, num12).Type == 32)
									{
										num13++;
									}
									if (TileRefs(num11 - 1, num12).Active && TileRefs(num11 - 1, num12).Type == 32)
									{
										num13++;
									}
									if (TileRefs(num11, num12 + 1).Active && TileRefs(num11, num12 + 1).Type == 32)
									{
										num13++;
									}
									if (TileRefs(num11, num12 - 1).Active && TileRefs(num11, num12 - 1).Type == 32)
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
												if (Math.Abs(l - num11) * 2 + Math.Abs(m - num12) < 9 && TileRefs(l, m).Active && TileRefs(l, m).Type == 23 && TileRefs(l, m - 1).Active && TileRefs(l, m - 1).Type == 32 && TileRefs(l, m - 1).Liquid == 0)
												{
													flag3 = true;
													break;
												}
											}
										}
										if (flag3)
										{
											TileRefs(num11, num12).SetType(32);
											TileRefs(num11, num12).SetActive(true);
											SquareTileFrame(TileRefs, sandbox, num11, num12, true);

											NetMessage.SendTileSquare(-1, num11, num12, 3);
										}
									}
								}
							}
						}
					}
					else if (flag && spawnNPC > 0)
					{
						SpawnNPC(TileRefs, TileX, TileY);
					}
					if (Tile.Active)
					{
						if ((Tile.Type == 2 || Tile.Type == 52) && genRand.Next(40) == 0 && !TileRefs(TileX, TileY + 1).Active && !TileRefs(TileX, TileY + 1).Lava)
						{
							bool flag4 = false;
							for (int n = TileY; n > TileY - 10; n--)
							{
								if (TileRefs(TileX, n).Active && TileRefs(TileX, n).Type == 2)
								{
									flag4 = true;
									break;
								}
							}
							if (flag4)
							{
								int num20 = TileX;
								int num21 = TileY + 1;
								TileRefs(num20, num21).SetType(52);
								TileRefs(num20, num21).SetActive(true);
								SquareTileFrame(TileRefs, sandbox, num20, num21, true);
								NetMessage.SendTileSquare(-1, num20, num21, 3);
							}
						}
						if (Tile.Type == 60)
						{
							int type = (int)Tile.Type;
							if (!Tile2.Active && genRand.Next(7) == 0)
							{
								PlaceTile(TileRefs, sandbox, TileX, rTileY, 61, true, false, -1, 0);
								if (Tile2.Active)
								{
									NetMessage.SendTileSquare(-1, TileX, rTileY, 1);
								}
							}
							else if (genRand.Next(500) == 0 && (!Tile2.Active || Tile2.Type == 61 || Tile2.Type == 74 || Tile2.Type == 69) && !PlayerLOS(TileX, TileY))
							{
								WorldGen.GrowTree(TileRefs, sandbox, TileX, TileY);
							}
							bool flag5 = false;
							for (int num22 = num6; num22 < num7; num22++)
							{
								for (int num23 = rTileY; num23 < num9; num23++)
								{
									if ((TileX != num22 || TileY != num23) && TileRefs(num22, num23).Active && TileRefs(num22, num23).Type == 59)
									{
										SpreadGrass(TileRefs, num22, num23, 59, type, false);
										if ((int)TileRefs(num22, num23).Type == type)
										{
											SquareTileFrame(TileRefs, sandbox, num22, num23, true);
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
						if ((Tile.Type == 60 || Tile.Type == 62) && genRand.Next(15) == 0 && !TileRefs(TileX, TileY + 1).Active && !TileRefs(TileX, TileY + 1).Lava)
						{
							bool flag6 = false;
							for (int num24 = TileY; num24 > TileY - 10; num24--)
							{
								if (TileRefs(TileX, num24).Active && TileRefs(TileX, num24).Type == 60)
								{
									flag6 = true;
									break;
								}
							}
							if (flag6)
							{
								int num25 = TileX;
								int num26 = TileY + 1;
								TileRefs(num25, num26).SetType(62);
								TileRefs(num25, num26).SetActive(true);
								SquareTileFrame(TileRefs, sandbox, num25, num26, true);
								NetMessage.SendTileSquare(-1, num25, num26, 3);
							}
						}
						if ((Tile.Type == 109 || Tile.Type == 115) && genRand.Next(15) == 0 && !TileRefs(TileX, TileY + 1).Active && !TileRefs(TileX, TileY + 1).Lava)
						{
							bool flag7 = false;
							for (int y = TileY; y > TileY - 10; y--)
							{
								if (TileRefs(TileX, y).Active && TileRefs(TileX, y).Type == 109)
								{
									flag7 = true;
									break;
								}
							}
							if (flag7)
							{
								int setY = TileY + 1;
								TileRefs(TileX, setY).SetType(115);
								TileRefs(TileX, setY).SetActive(true);
								SquareTileFrame(TileRefs, sandbox, TileX, setY, true);
								NetMessage.SendTileSquare(-1, TileX, setY, 3);
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
					if (Main.tileAlch[(int)TileRefs(num28, num29).Type])
					{
						GrowAlch(TileRefs, sandbox, num28, num29);
					}
					if (TileRefs(num28, num29).Liquid <= 32)
					{
						if (TileRefs(num28, num29).Active)
						{
							UpdateWorld_Hardmode(TileRefs, sandbox, num31, num32);
							if (TileRefs(num28, num29).Type == 23 && !TileRefs(num28, num29).Active && genRand.Next(1) == 0)
							{
								PlaceTile(TileRefs, sandbox, num28, num32, 24, true, false, -1, 0);
								if (TileRefs(num28, num29).Active)
								{
									NetMessage.SendTileSquare(-1, num28, num32, 1);
								}
							}
							if (TileRefs(num28, num32).Type == 32 && genRand.Next(3) == 0)
							{
								int num37 = num28;
								int num38 = num32;
								int num39 = 0;
								if (TileRefs(num37 + 1, num38).Active && TileRefs(num37 + 1, num38).Type == 32)
								{
									num39++;
								}
								if (TileRefs(num37 - 1, num38).Active && TileRefs(num37 - 1, num38).Type == 32)
								{
									num39++;
								}
								if (TileRefs(num37, num38 + 1).Active && TileRefs(num37, num38 + 1).Type == 32)
								{
									num39++;
								}
								if (TileRefs(num37, num38 - 1).Active && TileRefs(num37, num38 - 1).Type == 32)
								{
									num39++;
								}
								if (num39 < 3 || TileRefs(num28, num32).Type == 23)
								{
									int num40 = genRand.Next(4);
									if (num40 == 0)
									{
										num38--;
									}
									else if (num40 == 1)
									{
										num38++;
									}
									else if (num40 == 2)
									{
										num37--;
									}
									else if (num40 == 3)
									{
										num37++;
									}
									if (!TileRefs(num37, num38).Active)
									{
										num39 = 0;
										if (TileRefs(num37 + 1, num38).Active && TileRefs(num37 + 1, num38).Type == 32)
										{
											num39++;
										}
										if (TileRefs(num37 - 1, num38).Active && TileRefs(num37 - 1, num38).Type == 32)
										{
											num39++;
										}
										if (TileRefs(num37, num38 + 1).Active && TileRefs(num37, num38 + 1).Type == 32)
										{
											num39++;
										}
										if (TileRefs(num37, num38 - 1).Active && TileRefs(num37, num38 - 1).Type == 32)
										{
											num39++;
										}
										if (num39 < 2)
										{
											int num41 = 7;
											int num42 = num37 - num41;
											int num43 = num37 + num41;
											int num44 = num38 - num41;
											int num45 = num38 + num41;
											bool flag8 = false;
											for (int num46 = num42; num46 < num43; num46++)
											{
												for (int num47 = num44; num47 < num45; num47++)
												{
													if (Math.Abs(num46 - num37) * 2 + Math.Abs(num47 - num38) < 9 && TileRefs(num46, num47).Active &&
														TileRefs(num46, num47).Type == 23 && TileRefs(num46, num47 - 1).Active &&
														TileRefs(num46, num47 - 1).Type == 32 && TileRefs(num46, num47 - 1).Liquid == 0)
													{
														flag8 = true;
														break;
													}
												}
											}
											if (flag8)
											{
												TileRefs(num37, num38).SetType(32);
												TileRefs(num37, num38).SetActive(true);
												SquareTileFrame(TileRefs, sandbox, num37, num38, true);
												NetMessage.SendTileSquare(-1, num37, num38, 3);
											}
										}
									}
								}
							}
							if (TileRefs(num28, num29).Type == 60)
							{
								int type2 = (int)TileRefs(num28, num29).Type;
								if (!TileRefs(num28, num32).Active && genRand.Next(10) == 0)
								{
									PlaceTile(TileRefs, sandbox, num28, num32, 61, true, false, -1, 0);
									if (TileRefs(num28, num32).Active)
									{
										NetMessage.SendTileSquare(-1, num28, num32, 1);
									}
								}
								bool flag7 = false;
								for (int num34 = num30; num34 < num31; num34++)
								{
									for (int num35 = num32; num35 < num33; num35++)
									{
										if ((num28 != num34 || num29 != num35) && TileRefs(num34, num35).Active && TileRefs(num34, num35).Type == 59)
										{
											SpreadGrass(TileRefs, num34, num35, 59, type2, false);
											if ((int)TileRefs(num34, num35).Type == type2)
											{
												SquareTileFrame(TileRefs, sandbox, num34, num35, true);
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
							if (TileRefs(num28, num29).Type == 61 && genRand.Next(3) == 0 && TileRefs(num28, num29).FrameX < 144)
							{
								TileRefs(num28, num29).SetType(74);
								NetMessage.SendTileSquare(-1, num28, num29, 3);
							}
							if ((TileRefs(num28, num29).Type == 60 || TileRefs(num28, num29).Type == 62) && genRand.Next(5) == 0 && !TileRefs(num28, num29 + 1).Active && !TileRefs(num28, num29 + 1).Lava)
							{
								bool flag8 = false;
								for (int num36 = num29; num36 > num29 - 10; num36--)
								{
									if (TileRefs(num28, num36).Active && TileRefs(num28, num36).Type == 60)
									{
										flag8 = true;
										break;
									}
								}
								if (flag8)
								{
									int num37 = num28;
									int num38 = num29 + 1;
									TileRefs(num37, num38).SetType(62);
									TileRefs(num37, num38).SetActive(true);
									SquareTileFrame(TileRefs, sandbox, num37, num38, true);
									NetMessage.SendTileSquare(-1, num37, num38, 3);
								}
							}
							if (TileRefs(num28, num29).Type == 69 && genRand.Next(3) == 0)
							{
								int num39 = num28;
								int num40 = num29;
								int num41 = 0;
								if (TileRefs(num39 + 1, num40).Active && TileRefs(num39 + 1, num40).Type == 69)
								{
									num41++;
								}
								if (TileRefs(num39 - 1, num40).Active && TileRefs(num39 - 1, num40).Type == 69)
								{
									num41++;
								}
								if (TileRefs(num39, num40 + 1).Active && TileRefs(num39, num40 + 1).Type == 69)
								{
									num41++;
								}
								if (TileRefs(num39, num40 - 1).Active && TileRefs(num39, num40 - 1).Type == 69)
								{
									num41++;
								}
								if (num41 < 3 || TileRefs(num28, num29).Type == 60)
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
									if (!TileRefs(num39, num40).Active)
									{
										num41 = 0;
										if (TileRefs(num39 + 1, num40).Active && TileRefs(num39 + 1, num40).Type == 69)
										{
											num41++;
										}
										if (TileRefs(num39 - 1, num40).Active && TileRefs(num39 - 1, num40).Type == 69)
										{
											num41++;
										}
										if (TileRefs(num39, num40 + 1).Active && TileRefs(num39, num40 + 1).Type == 69)
										{
											num41++;
										}
										if (TileRefs(num39, num40 - 1).Active && TileRefs(num39, num40 - 1).Type == 69)
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
													if (Math.Abs(num48 - num39) * 2 + Math.Abs(num49 - num40) < 9 && TileRefs(num48, num49).Active && TileRefs(num48, num49).Type == 60 && TileRefs(num48, num49 - 1).Active && TileRefs(num48, num49 - 1).Type == 69 && TileRefs(num48, num49 - 1).Liquid == 0)
													{
														flag9 = true;
														break;
													}
												}
											}
											if (flag9)
											{
												TileRefs(num39, num40).SetType(69);
												TileRefs(num39, num40).SetActive(true);
												SquareTileFrame(TileRefs, sandbox, num39, num40, true);
												NetMessage.SendTileSquare(-1, num39, num40, 3);
											}
										}
									}
								}
							}
							if (TileRefs(num28, num29).Type == 70)
							{
								int type3 = (int)TileRefs(num28, num29).Type;
								if (!TileRefs(num28, num32).Active && genRand.Next(10) == 0)
								{
									PlaceTile(TileRefs, sandbox, num28, num32, 71, true, false, -1, 0);
									if (TileRefs(num28, num32).Active)
									{
										NetMessage.SendTileSquare(-1, num28, num32, 1);
									}
								}
								if (genRand.Next(200) == 0 && !PlayerLOS(num28, num29))
								{
									GrowShroom(TileRefs, num28, num29);
								}
								bool flag10 = false;
								for (int num50 = num30; num50 < num31; num50++)
								{
									for (int num51 = num32; num51 < num33; num51++)
									{
										if ((num28 != num50 || num29 != num51) && TileRefs(num50, num51).Active && TileRefs(num50, num51).Type == 59)
										{
											SpreadGrass(TileRefs, num50, num51, 59, type3, false);
											if ((int)TileRefs(num50, num51).Type == type3)
											{
												SquareTileFrame(TileRefs, sandbox, num50, num51, true);
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
							SpawnNPC(TileRefs, num28, num29);
						}
					}
				}
				num27++;
			}
			if (Main.rand.Next(100) == 0)
			{
				PlantAlch(TileRefs);
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

		public static void PlaceWall(Func<Int32, Int32, ITile> TileRefs, int i, int j, int type, bool mute = false)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (i <= 1 || j <= 1 || i >= Main.maxTilesX - 2 || j >= Main.maxTilesY - 2)
			{
				return;
			}

			if (TileRefs(i, j).Wall == 0)
			{
				TileRefs(i, j).SetWall ((byte)type);
				SquareWallFrame(TileRefs, i, j, true);
			}
		}

		public static void SpreadGrass(Func<Int32, Int32, ITile> TileRefs, int i, int j, int dirt = 0, int grass = 2, bool repeat = true)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			try
			{
				if ((int)TileRefs(i, j).Type == dirt && TileRefs(i, j).Active && ((double)j >= Main.worldSurface || grass != 70) && ((double)j < Main.worldSurface || dirt != 0))
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
							if (!TileRefs(k, l).Active || !Main.tileSolid[(int)TileRefs(k, l).Type])
							{
								flag = false;
							}
							if (TileRefs(k, l).Lava && TileRefs(k, l).Liquid > 0)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						if (grass != 23 || TileRefs(i, j - 1).Type != 27)
						{
							TileRefs(i, j).SetType((byte)grass);
							for (int m = num; m < num2; m++)
							{
								for (int n = num3; n < num4; n++)
								{
									if (TileRefs(m, n).Active && (int)TileRefs(m, n).Type == dirt)
									{
										try
										{
											if (repeat && grassSpread < 1000)
											{
												grassSpread++;
												SpreadGrass(TileRefs, m, n, dirt, grass, true);
												grassSpread--;
											}
										}
										catch
										{
										}
									}
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

		public static void SquareTileFrame(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, bool resetFrame = true)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			TileFrame(TileRefs, sandbox, i - 1, j - 1, false, false);
			TileFrame(TileRefs, sandbox, i - 1, j, false, false);
			TileFrame(TileRefs, sandbox, i - 1, j + 1, false, false);
			TileFrame(TileRefs, sandbox, i, j - 1, false, false);
			TileFrame(TileRefs, sandbox, i, j, resetFrame, false);
			TileFrame(TileRefs, sandbox, i, j + 1, false, false);
			TileFrame(TileRefs, sandbox, i + 1, j - 1, false, false);
			TileFrame(TileRefs, sandbox, i + 1, j, false, false);
			TileFrame(TileRefs, sandbox, i + 1, j + 1, false, false);
		}

		public static void SquareWallFrame(Func<Int32, Int32, ITile> TileRefs, int i, int j, bool resetFrame = true)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			WallFrame(TileRefs, i - 1, j - 1, false);
			WallFrame(TileRefs, i - 1, j, false);
			WallFrame(TileRefs, i - 1, j + 1, false);
			WallFrame(TileRefs, i, j - 1, false);
			WallFrame(TileRefs, i, j, resetFrame);
			WallFrame(TileRefs, i, j + 1, false);
			WallFrame(TileRefs, i + 1, j - 1, false);
			WallFrame(TileRefs, i + 1, j, false);
			WallFrame(TileRefs, i + 1, j + 1, false);
		}

		public static void SectionTileFrame(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int startX, int startY, int endX, int endY)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
					TileFrame(TileRefs, sandbox, i, j, true, true);
				}
			}
		}

		public static void RangeFrame(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int startX, int startY, int endX, int endY)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = endX + 1;
			int num2 = endY + 1;
			for (int i = startX - 1; i < num + 1; i++)
			{
				for (int j = startY - 1; j < num2 + 1; j++)
				{
					TileFrame(TileRefs, sandbox, i, j);
					WallFrame(TileRefs, i, j);
				}
			}
		}

		public static void WaterCheck(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, ProgressLogger prog = null)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			Liquid.numLiquid = 0;
			LiquidBuffer.numLiquidBuffer = 0;
			for (int i = 1; i < Main.maxTilesX - 1; i++)
			{
				for (int j = Main.maxTilesY - 2; j > 0; j--)
				{
					TileRefs(i, j).SetCheckingLiquid(false);
					if (TileRefs(i, j).Liquid > 0 && TileRefs(i, j).Active && Main.tileSolid[(int)TileRefs(i, j).Type] && !Main.tileSolidTop[(int)TileRefs(i, j).Type])
					{
						TileRefs(i, j).SetLiquid(0);
					}
					else if (TileRefs(i, j).Liquid > 0)
					{
						if (TileRefs(i, j).Active)
						{
							if (Main.tileWaterDeath[(int)TileRefs(i, j).Type] && (TileRefs(i, j).Type != 4 || TileRefs(i, j).FrameY != 176))
							{
								KillTile(TileRefs, sandbox, i, j, false, false, false);
							}
							if (TileRefs(i, j).Lava && Main.tileLavaDeath[(int)TileRefs(i, j).Type])
							{
								KillTile(TileRefs, sandbox, i, j, false, false, false);
							}
						}
						if ((!TileRefs(i, j + 1).Active || !Main.tileSolid[(int)TileRefs(i, j + 1).Type] || Main.tileSolidTop[(int)TileRefs(i, j + 1).Type]) && TileRefs(i, j + 1).Liquid < 255)
						{
							if (TileRefs(i, j + 1).Liquid > 250)
							{
								TileRefs(i, j + 1).SetLiquid(255);
							}
							else
							{
								Liquid.AddWater(TileRefs, sandbox, i, j);
							}
						}
						if ((!TileRefs(i - 1, j).Active || !Main.tileSolid[(int)TileRefs(i - 1, j).Type] || Main.tileSolidTop[(int)TileRefs(i - 1, j).Type]) && TileRefs(i - 1, j).Liquid != TileRefs(i, j).Liquid)
						{
							Liquid.AddWater(TileRefs, sandbox, i, j);
						}
						else if ((!TileRefs(i + 1, j).Active || !Main.tileSolid[(int)TileRefs(i + 1, j).Type] || Main.tileSolidTop[(int)TileRefs(i + 1, j).Type]) && TileRefs(i + 1, j).Liquid != TileRefs(i, j).Liquid)
						{
							Liquid.AddWater(TileRefs, sandbox, i, j);
						}

						if (TileRefs(i, j).Lava)
						{
							if (TileRefs(i - 1, j).Liquid > 0 && !TileRefs(i - 1, j).Lava)
							{
								Liquid.AddWater(TileRefs, sandbox, i, j);
							}
							else if (TileRefs(i + 1, j).Liquid > 0 && !TileRefs(i + 1, j).Lava)
							{
								Liquid.AddWater(TileRefs, sandbox, i, j);
							}
							else if (TileRefs(i, j - 1).Liquid > 0 && !TileRefs(i, j - 1).Lava)
							{
								Liquid.AddWater(TileRefs, sandbox, i, j);
							}
							else if (TileRefs(i, j + 1).Liquid > 0 && !TileRefs(i, j + 1).Lava)
							{
								Liquid.AddWater(TileRefs, sandbox, i, j);
							}
						}
					}
				}
			}

			if (prog != null)
				prog.Value++;
		}

		public static void EveryTileFrame(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			using (var prog = new ProgressLogger(Main.maxTilesX, "Finding tile frames"))
			{
				noLiquidCheck = true;
				noTileActions = true;
				for (int i = 0; i < Main.maxTilesX; i++)
				{
					prog.Value = i;

					for (int j = 0; j < Main.maxTilesY; j++)
					{
						if (TileRefs(i, j).Active)
						{
							TileFrame(TileRefs, sandbox, i, j, true, false);
						}
						if (TileRefs(i, j).Wall > 0)
						{
							WallFrame(TileRefs, i, j, true);
						}
					}
				}
				noLiquidCheck = false;
				noTileActions = false;
			}
		}

		public static void TileFrame(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, bool resetFrame = false, bool noBreak = false)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			try
			{
				if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5)
				{
					if (TileRefs(i, j).Liquid > 0 && !noLiquidCheck)
					{
						Liquid.AddWater(TileRefs, sandbox, i, j);
					}
					if (TileRefs(i, j).Active)
					{
						if (!noBreak || !Main.tileFrameImportant[(int)TileRefs(i, j).Type] || TileRefs(i, j).Type == 4)
						{
							int num = (int)TileRefs(i, j).Type;
							if (Main.tileStone[num])
							{
								num = 1;
							}
							int frameX = (int)TileRefs(i, j).FrameX;
							int frameY = (int)TileRefs(i, j).FrameY;
							Rectangle rectangle = new Rectangle(-1, -1, 0, 0);
							if (Main.tileFrameImportant[(int)TileRefs(i, j).Type])
							{
								if (num == 4)
								{
									short num2 = 0;
									if (TileRefs(i, j).FrameX >= 66)
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

									if (TileRefs(i, j + 1).Active)
									{
										num3 = (int)TileRefs(i, j + 1).Type;
									}
									if (TileRefs(i - 1, j).Active)
									{
										num4 = (int)TileRefs(i - 1, j).Type;
									}
									if (TileRefs(i + 1, j).Active)
									{
										num5 = (int)TileRefs(i + 1, j).Type;
									}
									if (TileRefs(i - 1, j + 1).Active)
									{
										num6 = (int)TileRefs(i - 1, j + 1).Type;
									}
									if (TileRefs(i + 1, j + 1).Active)
									{
										num7 = (int)TileRefs(i + 1, j + 1).Type;
									}
									if (TileRefs(i - 1, j - 1).Active)
									{
										num8 = (int)TileRefs(i - 1, j - 1).Type;
									}
									if (TileRefs(i + 1, j - 1).Active)
									{
										num9 = (int)TileRefs(i + 1, j - 1).Type;
									}
									if (num3 >= 0 && Main.tileSolid[num3] && !Main.tileNoAttach[num3])
									{
										TileRefs(i, j).SetFrameX(num2);
									}
									else if ((num4 >= 0 && Main.tileSolid[num4] && !Main.tileNoAttach[num4]) || num4 == 124 || (num4 == 5 && num8 == 5 && num6 == 5))
									{
										TileRefs(i, j).SetFrameX((short)(22 + num2));
									}
									else if ((num5 >= 0 && Main.tileSolid[num5] && !Main.tileNoAttach[num5]) || num5 == 124 || (num5 == 5 && num9 == 5 && num7 == 5))
									{
										TileRefs(i, j).SetFrameX((short)(44 + num2));
									}
									else
									{
										KillTile(TileRefs, sandbox, i, j);
									}
								}
								else
								{
									if (num == 136)
									{
										int num10 = -1;
										int num11 = -1;
										int num12 = -1;

										if (TileRefs(i, j + 1).Active)
										{
											num10 = (int)TileRefs(i, j + 1).Type;
										}
										if (TileRefs(i - 1, j).Active)
										{
											num11 = (int)TileRefs(i - 1, j).Type;
										}
										if (TileRefs(i + 1, j).Active)
										{
											num12 = (int)TileRefs(i + 1, j).Type;
										}
										if (num10 >= 0 && Main.tileSolid[num10] && !Main.tileNoAttach[num10])
										{
											TileRefs(i, j).SetFrameX(0);
										}
										else if ((num11 >= 0 && Main.tileSolid[num11] && !Main.tileNoAttach[num11]) || num11 == 124 || num11 == 5)
										{
											TileRefs(i, j).SetFrameX(18);
										}
										else if ((num12 >= 0 && Main.tileSolid[num12] && !Main.tileNoAttach[num12]) || num12 == 124 || num12 == 5)
										{
											TileRefs(i, j).SetFrameX(36);
										}
										else
										{
											KillTile(TileRefs, sandbox, i, j);
										}
									}
									else
									{
										if (num == 129 || num == 149)
										{
											int num13 = -1;
											int num14 = -1;
											int num15 = -1;
											int num16 = -1;
											if (TileRefs(i, j - 1).Active)
											{
												num14 = (int)TileRefs(i, j - 1).Type;
											}
											if (TileRefs(i, j + 1).Active)
											{
												num13 = (int)TileRefs(i, j + 1).Type;
											}
											if (TileRefs(i - 1, j).Active)
											{
												num15 = (int)TileRefs(i - 1, j).Type;
											}
											if (TileRefs(i + 1, j).Active)
											{
												num16 = (int)TileRefs(i + 1, j).Type;
											}
											if (num13 >= 0 && Main.tileSolid[num13] && !Main.tileSolidTop[num13])
											{
												TileRefs(i, j).SetFrameY(0);
											}
											else if (num15 >= 0 && Main.tileSolid[num15] && !Main.tileSolidTop[num15])
											{
												TileRefs(i, j).SetFrameY(54);
											}
											else if (num16 >= 0 && Main.tileSolid[num16] && !Main.tileSolidTop[num16])
											{
												TileRefs(i, j).SetFrameY(36);
											}
											else if (num14 >= 0 && Main.tileSolid[num14] && !Main.tileSolidTop[num14])
											{
												TileRefs(i, j).SetFrameY(18);
											}
											else
											{
												KillTile(TileRefs, sandbox, i, j);
											}
										}
										else
										{
											if (num == 3 || num == 24 || num == 61 || num == 71 || num == 73 || num == 74 || num == 110 || num == 113)
											{
												PlantCheck(TileRefs, sandbox, i, j);
											}
											else
											{
												if (num == 12 || num == 31)
												{
													CheckOrb(TileRefs, sandbox, i, j, num);
												}
												else
												{
													if (num == 10)
													{
														if (!destroyObject)
														{
															int frameY2 = (int)TileRefs(i, j).FrameY;
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

															if (!TileRefs(i, num17 - 1).Active || !Main.tileSolid[(int)TileRefs(i, num17 - 1).Type])
															{
																flag = true;
															}
															if (!TileRefs(i, num17 + 3).Active || !Main.tileSolid[(int)TileRefs(i, num17 + 3).Type])
															{
																flag = true;
															}
															if (!TileRefs(i, num17).Active || (int)TileRefs(i, num17).Type != num)
															{
																flag = true;
															}
															if (!TileRefs(i, num17 + 1).Active || (int)TileRefs(i, num17 + 1).Type != num)
															{
																flag = true;
															}
															if (!TileRefs(i, num17 + 2).Active || (int)TileRefs(i, num17 + 2).Type != num)
															{
																flag = true;
															}
															if (flag)
															{
																destroyObject = true;
																KillTile(TileRefs, sandbox, i, num17);
																KillTile(TileRefs, sandbox, i, num17 + 1);
																KillTile(TileRefs, sandbox, i, num17 + 2);
																StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 25, 1);
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
																int frameX2 = (int)TileRefs(i, j).FrameX;
																int frameY3 = (int)TileRefs(i, j).FrameY;
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

																if (!TileRefs(num19, num20 - 1).Active || !Main.tileSolid[(int)TileRefs(num19, num20 - 1).Type] || !TileRefs(num19, num20 + 3).Active || !Main.tileSolid[(int)TileRefs(num19, num20 + 3).Type])
																{
																	flag2 = true;
																	destroyObject = true;
																	StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 25, 1, false, 0);
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
																		if (!flag2 && (TileRefs(k, l).Type != 11 || !TileRefs(k, l).Active))
																		{
																			destroyObject = true;
																			StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 25, 1, false, 0);
																			flag2 = true;
																			k = num21;
																			l = num20;
																		}
																		if (flag2)
																		{
																			KillTile(TileRefs, sandbox, k, l);
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
																Check3x3(TileRefs, sandbox, i, j, (int)((byte)num));
															}
															else
															{
																if (num == 15 || num == 20)
																{
																	Check1x2(TileRefs, sandbox, i, j, (byte)num);
																}
																else
																{
																	if (num == 14 || num == 17 || num == 26 || num == 77 || num == 86 || num == 87 || num == 88 || num == 89 || num == 114 || num == 133)
																	{
																		Check3x2(TileRefs, sandbox, i, j, (int)((byte)num));
																	}
																	else
																	{
																		if (num == 135 || num == 144 || num == 141)
																		{
																			Check1x1(TileRefs, sandbox, i, j, num);
																		}
																		else
																		{
																			if (num == 16 || num == 18 || num == 29 || num == 103 || num == 134)
																			{
																				Check2x1(TileRefs, sandbox, i, j, (byte)num);
																			}
																			else
																			{
																				if (num == 13 || num == 33 || num == 50 || num == 78)
																				{
																					CheckOnTable1x1(TileRefs, sandbox, i, j, (int)((byte)num));
																				}
																				else
																				{
																					if (num == 21)
																					{
																						CheckChest(TileRefs, sandbox, i, j, (int)((byte)num));
																					}
																					else
																					{
																						if (num == 128)
																						{
																							CheckMan(TileRefs, sandbox, i, j);
																						}
																						else
																						{
																							if (num == 27)
																							{
																								CheckSunflower(TileRefs, sandbox, i, j, 27);
																							}
																							else
																							{
																								if (num == 28)
																								{
																									CheckPot(TileRefs, sandbox, i, j, 28);
																								}
																								else
																								{
																									if (num == 132 || num == 138 || num == 142 || num == 143)
																									{
																										Check2x2(TileRefs, sandbox, i, j, num);
																									}
																									else
																									{
																										if (num == 91)
																										{
																											CheckBanner(TileRefs, sandbox, i, j, (byte)num);
																										}
																										else
																										{
																											if (num == 139)
																											{
																												CheckMB(TileRefs, sandbox, i, j, (int)((byte)num));
																											}
																											else
																											{
																												if (num == 92 || num == 93)
																												{
																													Check1xX(TileRefs, sandbox, i, j, (byte)num);
																												}
																												else
																												{
																													if (num == 104 || num == 105)
																													{
																														Check2xX(TileRefs, sandbox, i, j, (byte)num);
																													}
																													else
																													{
																														if (num == 101 || num == 102)
																														{
																															Check3x4(TileRefs, sandbox, i, j, (int)((byte)num));
																														}
																														else
																														{
																															if (num == 42)
																															{
																																Check1x2Top(TileRefs, sandbox, i, j, (byte)num);
																															}
																															else
																															{
																																if (num == 55 || num == 85)
																																{
																																	CheckSign(TileRefs, sandbox, i, j, num);
																																}
																																else
																																{
																																	if (num == 79 || num == 90)
																																	{
																																		Check4x2(TileRefs, sandbox, i, j, num);
																																	}
																																	else
																																	{
																																		if (num == 85 || num == 94 || num == 95 || num == 96 || num == 97 || num == 98 || num == 99 || num == 100 || num == 125 || num == 126)
																																		{
																																			Check2x2(TileRefs, sandbox, i, j, num);
																																		}
																																		else
																																		{
																																			if (num == 81)
																																			{
																																				int num22 = -1;
																																				int num23 = -1;
																																				int num24 = -1;
																																				int num25 = -1;
																																				if (TileRefs(i, j - 1).Active)
																																				{
																																					num23 = (int)TileRefs(i, j - 1).Type;
																																				}
																																				if (TileRefs(i, j + 1).Active)
																																				{
																																					num22 = (int)TileRefs(i, j + 1).Type;
																																				}
																																				if (TileRefs(i - 1, j).Active)
																																				{
																																					num24 = (int)TileRefs(i - 1, j).Type;
																																				}
																																				if (TileRefs(i + 1, j).Active)
																																				{
																																					num25 = (int)TileRefs(i + 1, j).Type;
																																				}
																																				if (num24 != -1 || num23 != -1 || num25 != -1)
																																				{
																																					KillTile(TileRefs, sandbox, i, j);
																																				}
																																				else
																																				{
																																					if (num22 < 0 || !Main.tileSolid[num22])
																																					{
																																						KillTile(TileRefs, sandbox, i, j);
																																					}
																																				}
																																			}
																																			else
																																			{
																																				if (Main.tileAlch[num])
																																				{
																																					CheckAlch(TileRefs, sandbox, i, j);
																																				}
																																				else
																																				{
																																					if (num == 72)
																																					{
																																						int num26 = -1;
																																						int num27 = -1;
																																						if (TileRefs(i, j - 1).Active)
																																						{
																																							num27 = (int)TileRefs(i, j - 1).Type;
																																						}
																																						if (TileRefs(i, j + 1).Active)
																																						{
																																							num26 = (int)TileRefs(i, j + 1).Type;
																																						}
																																						if (num26 != num && num26 != 70)
																																						{
																																							KillTile(TileRefs, sandbox, i, j);
																																						}
																																						else
																																						{
																																							if (num27 != num && TileRefs(i, j).FrameX == 0)
																																							{
																																								TileRefs(i, j).SetFrameNumber((byte)genRand.Next(3));
																																								if (TileRefs(i, j).FrameNumber == 0)
																																								{
																																									TileRefs(i, j).SetFrameX(18);
																																									TileRefs(i, j).SetFrameY(0);
																																								}
																																								if (TileRefs(i, j).FrameNumber == 1)
																																								{
																																									TileRefs(i, j).SetFrameX(18);
																																									TileRefs(i, j).SetFrameY(18);
																																								}
																																								if (TileRefs(i, j).FrameNumber == 2)
																																								{
																																									TileRefs(i, j).SetFrameX(18);
																																									TileRefs(i, j).SetFrameY(36);
																																								}
																																							}
																																						}
																																					}
																																					else
																																					{
																																						if (num == 5)
																																						{
																																							CheckTree(TileRefs, sandbox, i, j);
																																						}
																																					}
																																				}
																																			}
																																		}
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
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
								if (TileRefs(i - 1, j).Active)
								{
									if (Main.tileStone[(int)TileRefs(i - 1, j).Type])
									{
										num31 = 1;
									}
									else
									{
										num31 = (int)TileRefs(i - 1, j).Type;
									}
								}
								if (TileRefs(i + 1, j).Active)
								{
									if (Main.tileStone[(int)TileRefs(i + 1, j).Type])
									{
										num32 = 1;
									}
									else
									{
										num32 = (int)TileRefs(i + 1, j).Type;
									}
								}
								if (TileRefs(i, j - 1).Active)
								{
									if (Main.tileStone[(int)TileRefs(i, j - 1).Type])
									{
										num29 = 1;
									}
									else
									{
										num29 = (int)TileRefs(i, j - 1).Type;
									}
								}
								if (TileRefs(i, j + 1).Active)
								{
									if (Main.tileStone[(int)TileRefs(i, j + 1).Type])
									{
										num34 = 1;
									}
									else
									{
										num34 = (int)TileRefs(i, j + 1).Type;
									}
								}
								if (TileRefs(i - 1, j - 1).Active)
								{
									if (Main.tileStone[(int)TileRefs(i - 1, j - 1).Type])
									{
										num28 = 1;
									}
									else
									{
										num28 = (int)TileRefs(i - 1, j - 1).Type;
									}
								}
								if (TileRefs(i + 1, j - 1).Active)
								{
									if (Main.tileStone[(int)TileRefs(i + 1, j - 1).Type])
									{
										num30 = 1;
									}
									else
									{
										num30 = (int)TileRefs(i + 1, j - 1).Type;
									}
								}
								if (TileRefs(i - 1, j + 1).Active)
								{
									if (Main.tileStone[(int)TileRefs(i - 1, j + 1).Type])
									{
										num33 = 1;
									}
									else
									{
										num33 = (int)TileRefs(i - 1, j + 1).Type;
									}
								}
								if (TileRefs(i + 1, j + 1).Active)
								{
									if (Main.tileStone[(int)TileRefs(i + 1, j + 1).Type])
									{
										num35 = 1;
									}
									else
									{
										num35 = (int)TileRefs(i + 1, j + 1).Type;
									}
								}
								if (!Main.tileSolid[num])
								{
									if (num == 49)
									{
										CheckOnTable1x1(TileRefs, sandbox, i, j, (int)((byte)num));
										return;
									}
									if (num == 80)
									{
										CactusFrame(TileRefs, sandbox, i, j);
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
											if (TileRefs(i, j).FrameNumber == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 0;
											}
											else
											{
												if (TileRefs(i, j).FrameNumber == 1)
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
												if (TileRefs(i, j).FrameNumber == 0)
												{
													rectangle.X = 18;
													rectangle.Y = 0;
												}
												else
												{
													if (TileRefs(i, j).FrameNumber == 1)
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
													if (TileRefs(i, j).FrameNumber == 0)
													{
														rectangle.X = 36;
														rectangle.Y = 0;
													}
													else
													{
														if (TileRefs(i, j).FrameNumber == 1)
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
														if (TileRefs(i, j).FrameNumber == 0)
														{
															rectangle.X = 54;
															rectangle.Y = 0;
														}
														else
														{
															if (TileRefs(i, j).FrameNumber == 1)
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
															if (TileRefs(i, j).FrameNumber == 0)
															{
																rectangle.X = 72;
																rectangle.Y = 0;
															}
															else
															{
																if (TileRefs(i, j).FrameNumber == 1)
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
																if (TileRefs(i, j).FrameNumber == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 0;
																}
																else
																{
																	if (TileRefs(i, j).FrameNumber == 1)
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
																	if (TileRefs(i, j).FrameNumber == 0)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 0;
																	}
																	else
																	{
																		if (TileRefs(i, j).FrameNumber == 1)
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
																	if (TileRefs(i, j).FrameNumber == 0)
																	{
																		rectangle.X = 90;
																		rectangle.Y = 0;
																	}
																	else
																	{
																		if (TileRefs(i, j).FrameNumber == 1)
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
									TileRefs(i, j).SetFrameNumber((byte)num36);
								}
								else
								{
									num36 = (int)TileRefs(i, j).FrameNumber;
								}
								if (num == 0)
								{
									if (num29 >= 0 && Main.tileMergeDirt[num29])
									{
										TileFrame(TileRefs, sandbox, i, j - 1, false, false);
										if (mergeDown)
										{
											num29 = num;
										}
									}
									if (num34 >= 0 && Main.tileMergeDirt[num34])
									{
										TileFrame(TileRefs, sandbox, i, j + 1, false, false);
										if (mergeUp)
										{
											num34 = num;
										}
									}
									if (num31 >= 0 && Main.tileMergeDirt[num31])
									{
										TileFrame(TileRefs, sandbox, i - 1, j, false, false);
										if (mergeRight)
										{
											num31 = num;
										}
									}
									if (num32 >= 0 && Main.tileMergeDirt[num32])
									{
										TileFrame(TileRefs, sandbox, i + 1, j, false, false);
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
													TileFrame(TileRefs, sandbox, i, j - 1, false, false);
													if (mergeDown)
													{
														num29 = num;
													}
												}
												if (num34 == 59)
												{
													TileFrame(TileRefs, sandbox, i, j + 1, false, false);
													if (mergeUp)
													{
														num34 = num;
													}
												}
												if (num31 == 59)
												{
													TileFrame(TileRefs, sandbox, i - 1, j, false, false);
													if (mergeRight)
													{
														num31 = num;
													}
												}
												if (num32 == 59)
												{
													TileFrame(TileRefs, sandbox, i + 1, j, false, false);
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
												TileFrame(TileRefs, sandbox, i, j - 1, false, false);
												if (mergeDown)
												{
													num29 = num;
												}
											}
											if (num34 == 57)
											{
												TileFrame(TileRefs, sandbox, i, j + 1, false, false);
												if (mergeUp)
												{
													num34 = num;
												}
											}
											if (num31 == 57)
											{
												TileFrame(TileRefs, sandbox, i - 1, j, false, false);
												if (mergeRight)
												{
													num31 = num;
												}
											}
											if (num32 == 57)
											{
												TileFrame(TileRefs, sandbox, i + 1, j, false, false);
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
														TileFrame(TileRefs, sandbox, i, j - 1, false, false);
														if (mergeDown)
														{
															num29 = num;
														}
													}
													if (num34 == 0)
													{
														TileFrame(TileRefs, sandbox, i, j + 1, false, false);
														if (mergeUp)
														{
															num34 = num;
														}
													}
													if (num31 == 0)
													{
														TileFrame(TileRefs, sandbox, i - 1, j, false, false);
														if (mergeRight)
														{
															num31 = num;
														}
													}
													if (num32 == 0)
													{
														TileFrame(TileRefs, sandbox, i + 1, j, false, false);
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
														TileFrame(TileRefs, sandbox, i, j - 1, false, false);
														if (mergeDown)
														{
															num29 = num;
														}
													}
													if (num34 == 58 || num34 == 76 || num34 == 75)
													{
														TileFrame(TileRefs, sandbox, i, j + 1, false, false);
														if (mergeUp)
														{
															num34 = num;
														}
													}
													if (num31 == 58 || num31 == 76 || num31 == 75)
													{
														TileFrame(TileRefs, sandbox, i - 1, j, false, false);
														if (mergeRight)
														{
															num31 = num;
														}
													}
													if (num32 == 58 || num32 == 76 || num32 == 75)
													{
														TileFrame(TileRefs, sandbox, i + 1, j, false, false);
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
								TileRefs(i, j).SetFrameX((short)rectangle.X);
								TileRefs(i, j).SetFrameY((short)rectangle.Y);
								if (num == 52 || num == 62 || num == 115)
								{
									//if (TileRefs(i, j - 1] != null)
									{
										if (!TileRefs(i, j - 1).Active)
										{
											num29 = -1;
										}
										else
										{
											num29 = (int)TileRefs(i, j - 1).Type;
										}
									}
									//else
									//{
									//    num29 = num;
									//}
									if (num == 52 && (num29 == 109 || num29 == 115))
									{
										TileRefs(i, j).SetType(115);
										SquareTileFrame(TileRefs, sandbox, i, j, true);
										return;
									}
									if (num == 115 && (num29 == 2 || num29 == 52))
									{
										TileRefs(i, j).SetType(52);
										SquareTileFrame(TileRefs, sandbox, i, j, true);
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
											KillTile(TileRefs, sandbox, i, j);
										}
									}
								}
								if (!noTileActions && (num == 53 || num == 112 || num == 116 || num == 123))
								{
									if (!TileRefs(i, j + 1).Active)
									{
										bool flag6 = true;
										if (TileRefs(i, j - 1).Active && TileRefs(i, j - 1).Type == 21)
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

											TileRefs(i, j).SetActive(false);

											/*int num39 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 2.5f, type2, 10, 0f, Main.myPlayer);
											Main.projectile[num39].Velocity.Y = 0.5f;
											Main.projectile[num39].Position.Y += 2f;

											Main.projectile[num39].netUpdate = true;
											NetMessage.SendTileSquare(-1, i, j, 1);*/

											Terraria_Server.Messages.TileBreakMessage.staticEditor.Sandbox.FallingBlockProjectile(i, j, type2);

											SquareTileFrame(TileRefs, sandbox, i, j, true);
										}
									}
								}
								if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
								{
									bool flag7 = mergeUp;
									bool flag8 = mergeDown;
									bool flag9 = mergeLeft;
									bool flag10 = mergeRight;
									TileFrame(TileRefs, sandbox, i - 1, j, false, false);
									TileFrame(TileRefs, sandbox, i + 1, j, false, false);
									TileFrame(TileRefs, sandbox, i, j - 1, false, false);
									TileFrame(TileRefs, sandbox, i, j + 1, false, false);
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
			{ }
		}

		public static void PlantCheck(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = -1;
			int num2 = (int)TileRefs(i, j).Type;

			if (j + 1 >= Main.maxTilesY)
			{
				num = num2;
			}

			if (j + 1 < Main.maxTilesY && TileRefs(i, j + 1).Active)
			{
				num = (int)TileRefs(i, j + 1).Type;
			}

			if ((num2 == 3 && num != 2 && num != 78) || (num2 == 24 && num != 23) || (num2 == 61 && num != 60) || (num2 == 71 && num != 70) ||
				(num2 == 73 && num != 2 && num != 78) || (num2 == 74 && num != 60) || (num2 == 110 && num != 109) || (num2 == 113 && num != 109))
			{
				if (num == 23)
				{
					num2 = 24;
					if (TileRefs(i, j).FrameX >= 162)
					{
						TileRefs(i, j).SetFrameX(126);
					}
				}
				else if (num == 2)
				{
					if (num2 == 113)
					{
						num2 = 73;
					}
					else
					{
						num2 = 3;
					}
				}
				else if (num == 109)
				{
					if (num2 == 73)
					{
						num2 = 113;
					}
					else
					{
						num2 = 110;
					}
				}

				if (num2 != (int)TileRefs(i, j).Type)
				{
					TileRefs(i, j).SetType((byte)num2);
					return;
				}
				KillTile(TileRefs, sandbox, i, j);
			}
		}
		
		public static void WallFrame(Func<Int32, Int32, ITile> TileRefs, int i, int j, bool resetFrame = false)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY && TileRefs(i, j).Wall > 0)
			{
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				int num5 = -1;
				int num6 = -1;
				int num7 = -1;
				int num8 = -1;
				int wall = (int)TileRefs(i, j).Wall;
				if (wall == 0)
				{
					return;
				}

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
					num4 = (int)TileRefs(i - 1, j).Wall;
				}
				if (i + 1 < Main.maxTilesX)
				{
					num5 = (int)TileRefs(i + 1, j).Wall;
				}
				if (j - 1 >= 0)
				{
					num2 = (int)TileRefs(i, j - 1).Wall;
				}
				if (j + 1 < Main.maxTilesY)
				{
					num7 = (int)TileRefs(i, j + 1).Wall;
				}
				if (i - 1 >= 0 && j - 1 >= 0)
				{
					num = (int)TileRefs(i - 1, j - 1).Wall;
				}
				if (i + 1 < Main.maxTilesX && j - 1 >= 0)
				{
					num3 = (int)TileRefs(i + 1, j - 1).Wall;
				}
				if (i - 1 >= 0 && j + 1 < Main.maxTilesY)
				{
					num6 = (int)TileRefs(i - 1, j + 1).Wall;
				}
				if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY)
				{
					num8 = (int)TileRefs(i + 1, j + 1).Wall;
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
				if (num7 > 0)
				{
					num7 = wall;
				}
				if (num6 > 0)
				{
					num6 = wall;
				}
				if (num8 > 0)
				{
					num8 = wall;
				}
				if (num2 > 0)
				{
					num2 = wall;
				}
				if (num > 0)
				{
					num = wall;
				}
				if (num3 > 0)
				{
					num3 = wall;
				}
				if (num4 > 0)
				{
					num4 = wall;
				}
				if (num5 > 0)
				{
					num5 = wall;
				}
				int num9 = 0;
				if (resetFrame)
				{
					num9 = genRand.Next(0, 3);
					//TileRefs(i, j).SetWallFrameNumber ( (byte)num9);
				}
				else
				{
					//num9 = (int)TileRefs(i, j).WallFrameNumber;
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
				TileRefs(i, j).SetWallFrameX((byte)rectangle.X);
				TileRefs(i, j).SetWallFrameY((byte)rectangle.Y);
			}
		}
		

		public static void StartHardMode()
		{
			ThreadPool.QueueUserWorkItem(smCallBack);
		}

		public static void smCallBack(object state)
		{
			if (Main.hardMode)
				return;

			hardLock = true;
			Main.hardMode = true;
			if (genRand == null)
			{
				DateTime now = DateTime.Now;
				genRand = new Random((int)now.Ticks);
			}

			float num = (float)genRand.Next(300, 400) * 0.001f;
			int i = (int)((float)Main.maxTilesX * num);
			int i2 = (int)((float)Main.maxTilesX * (1f - num));
			int num2 = 1;
			if (genRand.Next(2) == 0)
			{
				i2 = (int)((float)Main.maxTilesX * num);
				i = (int)((float)Main.maxTilesX * (1f - num));
				num2 = -1;
			}

			GERunner(null, null, i, 0, (float)(3 * num2), 5f, true);
			GERunner(null, null, i2, 0, (float)(3 * -(float)num2), 5f, false);

			ProgramLog.Log("Hardmode has been applied");
			NetMessage.SendData(25, -1, -1, "The ancient spirits of light and dark have been released.", 255, 50f, 255f, 130f, 0);
			NetPlay.ResetSections();

			hardLock = false;
		}

		// [TODO] 1.1 - here down
		public static int altarCount { get; set; }
		public static bool hardLock { get; set; }

		public static int totalEvil = 0;
		public static int totalGood = 0;
		public static int totalSolid = 0;
		public static int totalEvil2 = 0;
		public static int totalGood2 = 0;
		public static int totalSolid2 = 0;
		public static byte tEvil = 0;
		public static byte tGood = 0;

		public static void CountTiles(Func<Int32, Int32, ITile> TileRefs, int X)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
				NetMessage.SendData(57);

				totalEvil2 = 0;
				totalSolid2 = 0;
				totalGood2 = 0;
			}
			for (int i = 0; i < Main.maxTilesY; i++)
			{
				int num3 = 1;

				if ((double)i <= Main.worldSurface)
					num3 *= 5;

				if (SolidTile(TileRefs, X, i))
				{
					if (TileRefs(X, i).Type == 109 || TileRefs(X, i).Type == 116 || TileRefs(X, i).Type == 117)
						totalGood2 += num3;
					else if (TileRefs(X, i).Type == 23 || TileRefs(X, i).Type == 25 || TileRefs(X, i).Type == 112)
						totalEvil2 += num3;

					totalSolid2 += num3;
				}
			}
		}

		public static bool PlaceWire(Func<Int32, Int32, ITile> TileRefs, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (!TileRefs(i, j).Wire)
			{
				TileRefs(i, j).SetWire(true);
				return true;
			}
			return false;
		}

		public static bool KillWire(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (TileRefs(i, j).Wire)
			{
				TileRefs(i, j).SetWire(false);
				StorePlayerItem(sandbox, i * 16, j * 16, 16, 16, 530, 1, false, 0);
				return true;
			}
			return false;
		}

		public static void GERunner(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, float speedX, float speedY, bool good)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = genRand.Next(200, 250);
			float num2 = (float)(Main.maxTilesX / 4200);
			num = (int)((float)num * num2);
			double num3 = (double)num;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)genRand.Next(-10, 11) * 0.1f;
			if (speedX != 0f || speedY != 0f)
			{
				value2.X = speedX;
				value2.Y = speedY;
			}
			bool flag = true;
			while (flag)
			{
				int num4 = (int)((double)value.X - num3 * 0.5);
				int num5 = (int)((double)value.X + num3 * 0.5);
				int num6 = (int)((double)value.Y - num3 * 0.5);
				int num7 = (int)((double)value.Y + num3 * 0.5);

				if (num4 < 0)
					num4 = 0;
				if (num5 > Main.maxTilesX)
					num5 = Main.maxTilesX;
				if (num6 < 0)
					num6 = 0;
				if (num7 > Main.maxTilesY)
					num7 = Main.maxTilesY;

				for (int k = num4; k < num5; k++)
				{
					for (int l = num6; l < num7; l++)
					{
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < (double)num * 0.5 * (1.0 + (double)genRand.Next(-10, 11) * 0.015))
						{
							if (good)
							{
								if (TileRefs(k, l).Wall == 3)
								{
									TileRefs(k, l).SetWall(28);
								}
								if (TileRefs(k, l).Type == 2)
								{
									TileRefs(k, l).SetType(109);
									SquareTileFrame(TileRefs, sandbox, k, l, true);
								}
								else
								{
									if (TileRefs(k, l).Type == 1)
									{
										TileRefs(k, l).SetType(117);
										SquareTileFrame(TileRefs, sandbox, k, l, true);
									}
									else
									{
										if (TileRefs(k, l).Type == 53 || TileRefs(k, l).Type == 123)
										{
											TileRefs(k, l).SetType(116);
											SquareTileFrame(TileRefs, sandbox, k, l, true);
										}
										else
										{
											if (TileRefs(k, l).Type == 23)
											{
												TileRefs(k, l).SetType(109);
												SquareTileFrame(TileRefs, sandbox, k, l, true);
											}
											else
											{
												if (TileRefs(k, l).Type == 25)
												{
													TileRefs(k, l).SetType(117);
													SquareTileFrame(TileRefs, sandbox, k, l, true);
												}
												else
												{
													if (TileRefs(k, l).Type == 112)
													{
														TileRefs(k, l).SetType(116);
														SquareTileFrame(TileRefs, sandbox, k, l, true);
													}
												}
											}
										}
									}
								}
							}
							else
							{
								if (TileRefs(k, l).Type == 2)
								{
									TileRefs(k, l).SetType(23);
									SquareTileFrame(TileRefs, sandbox, k, l, true);
								}
								else
								{
									if (TileRefs(k, l).Type == 1)
									{
										TileRefs(k, l).SetType(25);
										SquareTileFrame(TileRefs, sandbox, k, l, true);
									}
									else
									{
										if (TileRefs(k, l).Type == 53 || TileRefs(k, l).Type == 123)
										{
											TileRefs(k, l).SetType(112);
											SquareTileFrame(TileRefs, sandbox, k, l, true);
										}
										else
										{
											if (TileRefs(k, l).Type == 109)
											{
												TileRefs(k, l).SetType(23);
												SquareTileFrame(TileRefs, sandbox, k, l, true);
											}
											else
											{
												if (TileRefs(k, l).Type == 117)
												{
													TileRefs(k, l).SetType(25);
													SquareTileFrame(TileRefs, sandbox, k, l, true);
												}
												else
												{
													if (TileRefs(k, l).Type == 116)
													{
														TileRefs(k, l).SetType(112);
														SquareTileFrame(TileRefs, sandbox, k, l, true);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				value += value2;
				value2.X += (float)genRand.Next(-10, 11) * 0.05f;
				if (value2.X > speedX + 1f)
				{
					value2.X = speedX + 1f;
				}
				if (value2.X < speedX - 1f)
				{
					value2.X = speedX - 1f;
				}
				if (value.X < (float)(-(float)num) || value.Y < (float)(-(float)num) || value.X > (float)(Main.maxTilesX + num) || value.Y > (float)(Main.maxTilesX + num))
				{
					flag = false;
				}
			}
		}

		public static void SmashAltar(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (!Main.hardMode || noTileActions || gen)
				return;

			int num = altarCount % 3;
			int num2 = altarCount / 3 + 1;
			float num3 = (float)(Main.maxTilesX / 4200);
			int num4 = 1 - num;
			num3 = num3 * 310f - (float)(85 * num);
			num3 *= 0.85f;
			num3 /= (float)num2;
			if (num == 0)
			{

				NetMessage.SendData(25, -1, -1, "Your world has been blessed with Cobalt!", 255, 50f, 255f, 130f, 0);
				num = 107;
				num3 *= 1.05f;
			}
			else if (num == 1)
			{
				NetMessage.SendData(25, -1, -1, "Your world has been blessed with Mythril!", 255, 50f, 255f, 130f, 0);
				num = 108;
			}
			else
			{
				NetMessage.SendData(25, -1, -1, "Your world has been blessed with Adamantite!", 255, 50f, 255f, 130f, 0);
				num = 111;
			}

			int num5 = 0;
			while ((float)num5 < num3)
			{
				int i2 = genRand.Next(100, Main.maxTilesX - 100);
				double num6 = Main.worldSurface;
				if (num == 108)
				{
					num6 = Main.rockLayer;
				}
				if (num == 111)
				{
					num6 = (Main.rockLayer + Main.rockLayer + (double)Main.maxTilesY) / 3.0;
				}
				int j2 = genRand.Next((int)num6, Main.maxTilesY - 150);
				OreRunner(TileRefs, sandbox, i2, j2, (double)genRand.Next(5, 9 + num4), genRand.Next(5, 9 + num4), num);
				num5++;
			}
			int num7 = genRand.Next(3);
			while (num7 != 2)
			{
				int num8 = genRand.Next(100, Main.maxTilesX - 100);
				int num9 = genRand.Next((int)Main.rockLayer + 50, Main.maxTilesY - 300);
				if (TileRefs(num8, num9).Active && TileRefs(num8, num9).Type == 1)
				{
					if (num7 == 0)
					{
						TileRefs(num8, num9).SetType(25);
					}
					else
					{
						TileRefs(num8, num9).SetType(117);
					}

					NetMessage.SendTileSquare(-1, num8, num9, 1);
					break;

				}
			}

			int num10 = Main.rand.Next(2) + 1;
			for (int k = 0; k < num10; k++)
			{
				var playerId = (int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16);
				var player = Main.players[playerId];
				NPC.SpawnOnPlayer(playerId, (int)NPCType.N82_WRAITH);
			}

			altarCount++;
		}

		public static void OreRunner(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, double strength, int steps, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			double num = strength;
			float num2 = (float)steps;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)genRand.Next(-10, 11) * 0.1f;
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
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < strength * 0.5 * (1.0 + (double)genRand.Next(-10, 11) * 0.015) &&
							TileRefs(k, l).Active && (TileRefs(k, l).Type == 0 || TileRefs(k, l).Type == 1 || TileRefs(k, l).Type == 23 || TileRefs(k, l).Type == 25 ||
								TileRefs(k, l).Type == 40 || TileRefs(k, l).Type == 53 || TileRefs(k, l).Type == 57 || TileRefs(k, l).Type == 59 ||
									TileRefs(k, l).Type == 60 || TileRefs(k, l).Type == 70 || TileRefs(k, l).Type == 109 || TileRefs(k, l).Type == 112 ||
										TileRefs(k, l).Type == 116 || TileRefs(k, l).Type == 117))
						{
							TileRefs(k, l).SetType((byte)type);
							SquareTileFrame(TileRefs, sandbox, k, l, true);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}
					}
				}
				value += value2;
				value2.X += (float)genRand.Next(-10, 11) * 0.05f;
				if (value2.X > 1f)
				{
					value2.X = 1f;
				}
				if (value2.X < -1f)
				{
					value2.X = -1f;
				}
			}
		}

		public static int PlaceChest(Func<Int32, Int32, ITile> TileRefs, int x, int y, int type = 21, bool notNearOtherChests = false, int style = 0)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			bool flag = true;
			int num = -1;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileRefs(i, j).Active)
					{
						flag = false;
					}
					if (TileRefs(i, j).Lava)
					{
						flag = false;
					}
				}

				if (!TileRefs(i, y + 1).Active || !Main.tileSolid[(int)TileRefs(i, y + 1).Type])
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
							if (TileRefs(k, l).Active && TileRefs(k, l).Type == 21)
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
				TileRefs(x, y - 1).SetActive(true);
				TileRefs(x, y - 1).SetFrameY(0);
				TileRefs(x, y - 1).SetFrameX((short)(36 * style));
				TileRefs(x, y - 1).SetType((byte)type);
				TileRefs(x + 1, y - 1).SetActive(true);
				TileRefs(x + 1, y - 1).SetFrameY(0);
				TileRefs(x + 1, y - 1).SetFrameX((short)(18 + 36 * style));
				TileRefs(x + 1, y - 1).SetType((byte)type);
				TileRefs(x, y).SetActive(true);
				TileRefs(x, y).SetFrameY(18);
				TileRefs(x, y).SetFrameX((short)(36 * style));
				TileRefs(x, y).SetType((byte)type);
				TileRefs(x + 1, y).SetActive(true);
				TileRefs(x + 1, y).SetFrameY(18);
				TileRefs(x + 1, y).SetFrameX((short)(18 + 36 * style));
				TileRefs(x + 1, y).SetType((byte)type);
			}
			return num;
		}

		public static void CheckChest(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int type)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (destroyObject)
				return;

			bool flag = false;
			int k = 0;
			k += (int)(TileRefs(i, j).FrameX / 18);
			int num = j + (int)(TileRefs(i, j).FrameY / 18 * -1);

			while (k > 1)
				k -= 2;

			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 2; m++)
				{
					int n;
					for (n = (int)(TileRefs(l, m).FrameX / 18); n > 1; n -= 2) { }

					if (!TileRefs(l, m).Active || (int)TileRefs(l, m).Type != type || n != l - k || (int)TileRefs(l, m).FrameY != (m - num) * 18)
					{
						flag = true;
					}
				}

				if (!TileRefs(l, num + 2).Active || !Main.tileSolid[(int)TileRefs(l, num + 2).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				int type2 = 48;
				if (TileRefs(i, j).FrameX >= 216)
				{
					type2 = 348;
				}
				else if (TileRefs(i, j).FrameX >= 180)
				{
					type2 = 343;
				}
				else if (TileRefs(i, j).FrameX >= 108)
				{
					type2 = 328;
				}
				else if (TileRefs(i, j).FrameX >= 36)
				{
					type2 = 306;
				}

				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 3; num3++)
					{
						if ((int)TileRefs(num2, num3).Type == type && TileRefs(num2, num3).Active)
						{
							Chest.DestroyChest(num2, num3);
							KillTile(TileRefs, sandbox, num2, num3, false, false, false);
						}
					}
				}

				StorePlayerItem(sandbox, i * 16, j * 16, 32, 32, type2, 1, false, 0);
				destroyObject = false;
			}
		}
	}
}
