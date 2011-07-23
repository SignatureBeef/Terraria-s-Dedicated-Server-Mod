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

namespace Terraria_Server.WorldMod
{
	public class WorldGen
	{
		private static int maxDRooms = 100;
		private const int RECTANGLE_OFFSET = 25;
		private const int TILE_OFFSET = 15;
		private const int TILES_OFFSET_2 = 10;
		private const int TILE_OFFSET_3 = 16;
		private const int TILE_OFFSET_4 = 23;
		private const int TILE_SCALE = 16;
		private const int TREE_RADIUS = 2;
		private static int hellChest = 0;
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
		private static bool[] dRoomTreasure = new bool[maxDRooms];
		private static int[] dRoomL = new int[maxDRooms];
		private static int[] dRoomR = new int[maxDRooms];
		private static int[] dRoomT = new int[maxDRooms];
		private static int[] dRoomB = new int[maxDRooms];
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
		private static int numDRooms = 0;
		public static int[] dRoomX = new int[maxDRooms];
		public static int[] dRoomY = new int[maxDRooms];
		public static int[] dRoomSize = new int[maxDRooms];
		public static int dEntranceX = 0;
		public static bool dSurface = false;
		public static int dungeonX;
		public static int dungeonY;
		public static Vector2 lastDungeonHall = default(Vector2);
		public static int numDungeons;

		private static void resetGen()
		{
			hellChest = 0;
			JungleX = 0;
			numMCaves = 0;
			numIslandHouses = 0;
			houseCount = 0;
			dEntranceX = 0;
			numDRooms = 0;
			numDDoors = 0;
			numDPlats = 0;
			numJChests = 0;
		}

		public static void generateWorld(int seed = -1)
		{
			WorldModify.gen = true;
			resetGen();
			if (seed > 0)
			{
				WorldModify.genRand = new Random(seed);
			}
			else
			{
				WorldModify.genRand = new Random((int)DateTime.Now.Ticks);
			}
			Main.worldID = WorldModify.genRand.Next(2147483647);
			int num = 0;
			int num2 = 0;
			double num3 = (double)Main.maxTilesY * 0.3;
			num3 *= (double)WorldModify.genRand.Next(90, 110) * 0.005;
			double num4 = num3 + (double)Main.maxTilesY * 0.2;
			num4 *= (double)WorldModify.genRand.Next(90, 110) * 0.01;
			double num5 = num3;
			double num6 = num3;
			double num7 = num4;
			double num8 = num4;
			int num9 = 0;
			if (WorldModify.genRand.Next(2) == 0)
			{
				num9 = -1;
			}
			else
			{
				num9 = 1;
			}

			using (var prog = new ProgressLogger(Main.maxTilesX - 1, "Generating world terrain"))
			{
				for (int i = 0; i < Main.maxTilesX; i++)
				{
					prog.Value = i;

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
						num = WorldModify.genRand.Next(0, 5);
						num2 = WorldModify.genRand.Next(5, 40);
						if (num == 0)
						{
							num2 *= (int)((double)WorldModify.genRand.Next(5, 30) * 0.2);
						}
					}
					num2--;
					if (num == 0)
					{
						while (WorldModify.genRand.Next(0, 7) == 0)
						{
							num3 += (double)WorldModify.genRand.Next(-1, 2);
						}
					}
					else if (num == 1)
					{
						while (WorldModify.genRand.Next(0, 4) == 0)
						{
							num3 -= 1.0;
						}
						while (WorldModify.genRand.Next(0, 10) == 0)
						{
							num3 += 1.0;
						}
					}
					else if (num == 2)
					{
						while (WorldModify.genRand.Next(0, 4) == 0)
						{
							num3 += 1.0;
						}
						while (WorldModify.genRand.Next(0, 10) == 0)
						{
							num3 -= 1.0;
						}
					}
					else if (num == 3)
					{
						while (WorldModify.genRand.Next(0, 2) == 0)
						{
							num3 -= 1.0;
						}
						while (WorldModify.genRand.Next(0, 6) == 0)
						{
							num3 += 1.0;
						}
					}
					else if (num == 4)
					{
						while (WorldModify.genRand.Next(0, 2) == 0)
						{
							num3 += 1.0;
						}
						while (WorldModify.genRand.Next(0, 5) == 0)
						{
							num3 -= 1.0;
						}
					}
					if (num3 < (double)Main.maxTilesY * 0.15)
					{
						num3 = (double)Main.maxTilesY * 0.15;
						num2 = 0;
					}
					else if (num3 > (double)Main.maxTilesY * 0.3)
					{
						num3 = (double)Main.maxTilesY * 0.3;
						num2 = 0;
					}
					if ((i < 275 || i > Main.maxTilesX - 275) && num3 > (double)Main.maxTilesY * 0.25)
					{
						num3 = (double)Main.maxTilesY * 0.25;
						num2 = 1;
					}
					while (WorldModify.genRand.Next(0, 3) == 0)
					{
						num4 += (double)WorldModify.genRand.Next(-2, 3);
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
						Main.tile.At(i, num11).SetActive(false);
						Main.tile.At(i, num11).SetLighted(true);
						Main.tile.At(i, num11).SetFrameX(-1);
						Main.tile.At(i, num11).SetFrameY(-1);
						num11++;
					}
					for (int j = (int)num3; j < Main.maxTilesY; j++)
					{
						if ((double)j < num4)
						{
							Main.tile.At(i, j).SetActive(true);
							Main.tile.At(i, j).SetType(0);
							Main.tile.At(i, j).SetFrameX(-1);
							Main.tile.At(i, j).SetFrameY(-1);
						}
						else
						{
							Main.tile.At(i, j).SetActive(true);
							Main.tile.At(i, j).SetType(1);
							Main.tile.At(i, j).SetFrameX(-1);
							Main.tile.At(i, j).SetFrameY(-1);
						}
					}
				}
			}
			Main.worldSurface = num6 + 25.0;
			Main.rockLayer = num8;
			double num12 = (double)((int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6);
			Main.rockLayer = Main.worldSurface + num12;
			WorldModify.waterLine = (int)(Main.rockLayer + (double)Main.maxTilesY) / 2;
			WorldModify.waterLine += WorldModify.genRand.Next(-100, 20);
			WorldModify.lavaLine = WorldModify.waterLine + WorldModify.genRand.Next(50, 80);
			int num13 = 0;

			ProgramLog.Log("Adding sand...");
			int num14 = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.0007), (int)((double)Main.maxTilesX * 0.002));
			num14 += 2;
			for (int k = 0; k < num14; k++)
			{
				int num15 = WorldModify.genRand.Next(Main.maxTilesX);
				while ((float)num15 > (float)Main.maxTilesX * 0.45f && (float)num15 < (float)Main.maxTilesX * 0.55f)
				{
					num15 = WorldModify.genRand.Next(Main.maxTilesX);
				}
				int num16 = WorldModify.genRand.Next(35, 90);
				if (k == 1)
				{
					float num17 = (float)(Main.maxTilesX / 4200);
					num16 += (int)((float)WorldModify.genRand.Next(20, 40) * num17);
				}
				if (WorldModify.genRand.Next(3) == 0)
				{
					num16 *= 2;
				}
				if (k == 1)
				{
					num16 *= 3;
				}
				int num18 = num15 - num16;
				num16 = WorldModify.genRand.Next(35, 90);
				if (WorldModify.genRand.Next(3) == 0)
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
					num19 = WorldModify.genRand.Next(260, 300);
					if (num9 == 1)
					{
						num19 += 40;
					}
				}
				else if (k == 2)
				{
					num18 = Main.maxTilesX - WorldModify.genRand.Next(260, 300);
					num19 = Main.maxTilesX;
					if (num9 == -1)
					{
						num18 -= 40;
					}
				}
				int num20 = WorldModify.genRand.Next(50, 100);
				for (int l = num18; l < num19; l++)
				{
					if (WorldModify.genRand.Next(2) == 0)
					{
						num20 += WorldModify.genRand.Next(-1, 2);
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
							num22 += WorldModify.genRand.Next(5);
							for (int m = num21; m < num21 + num22; m++)
							{
								if (l > num18 + WorldModify.genRand.Next(5) && l < num19 - WorldModify.genRand.Next(5))
								{
									Main.tile.At(l, m).SetType(53);
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
				TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), (double)WorldModify.genRand.Next(15, 70), WorldModify.genRand.Next(20, 130), 53, false, 0f, 0f, false, true);
			}
			numMCaves = 0;

			ProgramLog.Log("Generating hills...");
			for (int num23 = 0; num23 < (int)((double)Main.maxTilesX * 0.0008); num23++)
			{
				int num24 = 0;
				bool flag = false;
				bool flag2 = false;
				int num25 = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
				while (!flag2)
				{
					flag2 = true;
					while (num25 > Main.maxTilesX / 2 - 100 && num25 < Main.maxTilesX / 2 + 100)
					{
						num25 = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
					}
					for (int num26 = 0; num26 < numMCaves; num26++)
					{
						if (num25 > mCaveX[num26] - 50 && num25 < mCaveX[num26] + 50)
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
							Mountinater(num25, num27);
							mCaveX[numMCaves] = num25;
							mCaveY[numMCaves] = num27;
							numMCaves++;
							break;
						}
						num27++;
					}
				}
			}

			using (var prog = new ProgressLogger(Main.maxTilesX - 3, "Putting dirt behind dirt"))
			{
				for (int num28 = 1; num28 < Main.maxTilesX - 1; num28++)
				{
					prog.Value = num28 - 1;

					bool flag3 = false;
					num13 += WorldModify.genRand.Next(-1, 2);
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
							Main.tile.At(num28, num30).SetWall(2);
						}
						if (Main.tile.At(num28, num30).Active && Main.tile.At(num28 - 1, num30).Active && Main.tile.At(num28 + 1, num30).Active && Main.tile.At(num28, num30 + 1).Active && Main.tile.At(num28 - 1, num30 + 1).Active && Main.tile.At(num28 + 1, num30 + 1).Active)
						{
							flag3 = true;
						}
						num30++;
					}
				}
			}

			ProgramLog.Log("Placing rocks in the dirt...");
			for (int num31 = 0; num31 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num31++)
			{
				TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(0, (int)num5 + 1), (double)WorldModify.genRand.Next(4, 15), WorldModify.genRand.Next(5, 40), 1, false, 0f, 0f, false, true);
			}
			for (int num32 = 0; num32 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num32++)
			{
				TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num5, (int)num6 + 1), (double)WorldModify.genRand.Next(4, 10), WorldModify.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
			}
			for (int num33 = 0; num33 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0045); num33++)
			{
				TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num6, (int)num8 + 1), (double)WorldModify.genRand.Next(2, 7), WorldModify.genRand.Next(2, 23), 1, false, 0f, 0f, false, true);
			}

			ProgramLog.Log("Placing dirt in the rocks...");
			for (int num34 = 0; num34 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.005); num34++)
			{
				TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(2, 6), WorldModify.genRand.Next(2, 40), 0, false, 0f, 0f, false, true);
			}

			ProgramLog.Log("Adding clay...");
			for (int num35 = 0; num35 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num35++)
			{
				TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(0, (int)num5), (double)WorldModify.genRand.Next(4, 14), WorldModify.genRand.Next(10, 50), 40, false, 0f, 0f, false, true);
			}
			for (int num36 = 0; num36 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 5E-05); num36++)
			{
				TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num5, (int)num6 + 1), (double)WorldModify.genRand.Next(8, 14), WorldModify.genRand.Next(15, 45), 40, false, 0f, 0f, false, true);
			}
			for (int num37 = 0; num37 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num37++)
			{
				TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num6, (int)num8 + 1), (double)WorldModify.genRand.Next(8, 15), WorldModify.genRand.Next(5, 50), 40, false, 0f, 0f, false, true);
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
								Main.tile.At(num38, num40).SetType(0);
							}
						}
						break;
					}
					num39++;
				}
			}

			var num41max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015);
			using (var prog = new ProgressLogger(num41max - 1, "Making random holes"))
				for (int num41 = 0; num41 < num41max; num41++)
				{
					prog.Value = num41;

					int type = -1;
					if (WorldModify.genRand.Next(5) == 0)
					{
						type = -2;
					}
					TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num6, Main.maxTilesY), (double)WorldModify.genRand.Next(2, 5), WorldModify.genRand.Next(2, 20), type, false, 0f, 0f, false, true);
					TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num6, Main.maxTilesY), (double)WorldModify.genRand.Next(8, 15), WorldModify.genRand.Next(7, 30), type, false, 0f, 0f, false, true);
				}

			var num43max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05);
			using (var prog = new ProgressLogger(num43max - 1, "Generating small caves"))
				for (int num43 = 0; num43 < num43max; num43++)
				{
					prog.Value = num43;

					if (num8 <= (double)Main.maxTilesY)
					{
						int type2 = -1;
						if (WorldModify.genRand.Next(6) == 0)
						{
							type2 = -2;
						}
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num5, (int)num8 + 1), (double)WorldModify.genRand.Next(5, 15), WorldModify.genRand.Next(30, 200), type2, false, 0f, 0f, false, true);
					}
				}

			var num45max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015);
			using (var prog = new ProgressLogger(num45max - 1, "Generating large caves"))
				for (int num45 = 0; num45 < num45max; num45++)
				{
					prog.Value = num45;

					if (num8 <= (double)Main.maxTilesY)
					{
						int type3 = -1;
						if (WorldModify.genRand.Next(10) == 0)
						{
							type3 = -2;
						}
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num8, Main.maxTilesY), (double)WorldModify.genRand.Next(6, 20), WorldModify.genRand.Next(50, 300), type3, false, 0f, 0f, false, true);
					}
				}

			ProgramLog.Log("Generating surface caves...");
			for (int num47 = 0; num47 < (int)((double)Main.maxTilesX * 0.0025); num47++)
			{
				int num48 = WorldModify.genRand.Next(0, Main.maxTilesX);
				int num49 = 0;
				while ((double)num49 < num6)
				{
					if (Main.tile.At(num48, num49).Active)
					{
						TileRunner(num48, num49, (double)WorldModify.genRand.Next(3, 6), WorldModify.genRand.Next(5, 50), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 1f, false, true);
						break;
					}
					num49++;
				}
			}
			for (int num50 = 0; num50 < (int)((double)Main.maxTilesX * 0.0007); num50++)
			{
				int num48 = WorldModify.genRand.Next(0, Main.maxTilesX);
				int num51 = 0;
				while ((double)num51 < num6)
				{
					if (Main.tile.At(num48, num51).Active)
					{
						TileRunner(num48, num51, (double)WorldModify.genRand.Next(10, 15), WorldModify.genRand.Next(50, 130), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
						break;
					}
					num51++;
				}
			}
			for (int num52 = 0; num52 < (int)((double)Main.maxTilesX * 0.0003); num52++)
			{
				int num48 = WorldModify.genRand.Next(0, Main.maxTilesX);
				int num53 = 0;
				while ((double)num53 < num6)
				{
					if (Main.tile.At(num48, num53).Active)
					{
						TileRunner(num48, num53, (double)WorldModify.genRand.Next(12, 25), WorldModify.genRand.Next(150, 500), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 4f, false, true);
						TileRunner(num48, num53, (double)WorldModify.genRand.Next(8, 17), WorldModify.genRand.Next(60, 200), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
						TileRunner(num48, num53, (double)WorldModify.genRand.Next(5, 13), WorldModify.genRand.Next(40, 170), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
						break;
					}
					num53++;
				}
			}
			for (int num54 = 0; num54 < (int)((double)Main.maxTilesX * 0.0004); num54++)
			{
				int num48 = WorldModify.genRand.Next(0, Main.maxTilesX);
				int num55 = 0;
				while ((double)num55 < num6)
				{
					if (Main.tile.At(num48, num55).Active)
					{
						TileRunner(num48, num55, (double)WorldModify.genRand.Next(7, 12), WorldModify.genRand.Next(150, 250), -1, false, 0f, 1f, true, true);
						break;
					}
					num55++;
				}
			}
			for (int num56 = 0; num56 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002); num56++)
			{
				int num57 = WorldModify.genRand.Next(1, Main.maxTilesX - 1);
				int num58 = WorldModify.genRand.Next((int)num5, (int)num6);
				if (num58 >= Main.maxTilesY)
				{
					num58 = Main.maxTilesY - 2;
				}
				if (Main.tile.At(num57 - 1, num58).Active && Main.tile.At(num57 - 1, num58).Type == 0 && Main.tile.At(num57 + 1, num58).Active && Main.tile.At(num57 + 1, num58).Type == 0 && Main.tile.At(num57, num58 - 1).Active && Main.tile.At(num57, num58 - 1).Type == 0 && Main.tile.At(num57, num58 + 1).Active && Main.tile.At(num57, num58 + 1).Type == 0)
				{
					Main.tile.At(num57, num58).SetActive(true);
					Main.tile.At(num57, num58).SetType(2);
				}
				num57 = WorldModify.genRand.Next(1, Main.maxTilesX - 1);
				num58 = WorldModify.genRand.Next(0, (int)num5);
				if (num58 >= Main.maxTilesY)
				{
					num58 = Main.maxTilesY - 2;
				}
				if (Main.tile.At(num57 - 1, num58).Active && Main.tile.At(num57 - 1, num58).Type == 0 && Main.tile.At(num57 + 1, num58).Active && Main.tile.At(num57 + 1, num58).Type == 0 && Main.tile.At(num57, num58 - 1).Active && Main.tile.At(num57, num58 - 1).Type == 0 && Main.tile.At(num57, num58 + 1).Active && Main.tile.At(num57, num58 + 1).Type == 0)
				{
					Main.tile.At(num57, num58).SetActive(true);
					Main.tile.At(num57, num58).SetType(2);
				}
			}

			using (var jungleprog = new ProgressLogger(100, "Generating jungle"))
			{
				float num59 = (float)(Main.maxTilesX / 4200);
				num59 *= 1.5f;
				int num60 = 0;
				float num61 = (float)WorldModify.genRand.Next(15, 30) * 0.01f;
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
				num60 += WorldModify.genRand.Next((int)(-100f * num59), (int)(101f * num59));
				num62 += WorldModify.genRand.Next((int)(-100f * num59), (int)(101f * num59));
				int num63 = num60;
				int num64 = num62;
				TileRunner(num60, num62, (double)WorldModify.genRand.Next((int)(250f * num59), (int)(500f * num59)), WorldModify.genRand.Next(50, 150), 59, false, (float)(num9 * 3), 0f, false, true);

				jungleprog.Value = 15;

				num60 += WorldModify.genRand.Next((int)(-250f * num59), (int)(251f * num59));
				num62 += WorldModify.genRand.Next((int)(-150f * num59), (int)(151f * num59));
				int num65 = num60;
				int num66 = num62;
				int num67 = num60;
				int num68 = num62;
				TileRunner(num60, num62, (double)WorldModify.genRand.Next((int)(250f * num59), (int)(500f * num59)), WorldModify.genRand.Next(50, 150), 59, false, 0f, 0f, false, true);

				jungleprog.Value = 30;

				num60 += WorldModify.genRand.Next((int)(-400f * num59), (int)(401f * num59));
				num62 += WorldModify.genRand.Next((int)(-150f * num59), (int)(151f * num59));
				int num69 = num60;
				int num70 = num62;
				TileRunner(num60, num62, (double)WorldModify.genRand.Next((int)(250f * num59), (int)(500f * num59)), WorldModify.genRand.Next(50, 150), 59, false, (float)(num9 * -3), 0f, false, true);

				jungleprog.Value = 45;

				num60 = (num63 + num65 + num69) / 3;
				num62 = (num64 + num66 + num70) / 3;
				TileRunner(num60, num62, (double)WorldModify.genRand.Next((int)(400f * num59), (int)(600f * num59)), 10000, 59, false, 0f, -20f, true, true);
				JungleRunner(num60, num62);

				jungleprog.Value = 60;

				num60 = num67;
				num62 = num68;
				int num71 = 0;
				while ((float)num71 <= 20f * num59)
				{
					jungleprog.Value = (int)(60f + (float)num71 / num59);

					num60 += WorldModify.genRand.Next((int)(-5f * num59), (int)(6f * num59));
					num62 += WorldModify.genRand.Next((int)(-5f * num59), (int)(6f * num59));
					TileRunner(num60, num62, (double)WorldModify.genRand.Next(40, 100), WorldModify.genRand.Next(300, 500), 59, false, 0f, 0f, false, true);
					num71++;
				}
				int num72 = 0;
				while ((float)num72 <= 10f * num59)
				{
					jungleprog.Value = (int)(80f + (float)num72 / num59 * 2f);

					num60 = num67 + WorldModify.genRand.Next((int)(-600f * num59), (int)(600f * num59));
					num62 = num68 + WorldModify.genRand.Next((int)(-200f * num59), (int)(200f * num59));
					while (num60 < 1 || num60 >= Main.maxTilesX - 1 || num62 < 1 || num62 >= Main.maxTilesY - 1 || Main.tile.At(num60, num62).Type != 59)
					{
						num60 = num67 + WorldModify.genRand.Next((int)(-600f * num59), (int)(600f * num59));
						num62 = num68 + WorldModify.genRand.Next((int)(-200f * num59), (int)(200f * num59));
					}
					int num73 = 0;
					while ((float)num73 < 8f * num59)
					{
						num60 += WorldModify.genRand.Next(-30, 31);
						num62 += WorldModify.genRand.Next(-30, 31);
						int type4 = -1;
						if (WorldModify.genRand.Next(7) == 0)
						{
							type4 = -2;
						}
						TileRunner(num60, num62, (double)WorldModify.genRand.Next(10, 20), WorldModify.genRand.Next(30, 70), type4, false, 0f, 0f, false, true);
						num73++;
					}
					num72++;
				}
				int num74 = 0;
				while ((float)num74 <= 300f * num59)
				{
					num60 = num67 + WorldModify.genRand.Next((int)(-600f * num59), (int)(600f * num59));
					num62 = num68 + WorldModify.genRand.Next((int)(-200f * num59), (int)(200f * num59));
					while (num60 < 1 || num60 >= Main.maxTilesX - 1 || num62 < 1 || num62 >= Main.maxTilesY - 1 || Main.tile.At(num60, num62).Type != 59)
					{
						num60 = num67 + WorldModify.genRand.Next((int)(-600f * num59), (int)(600f * num59));
						num62 = num68 + WorldModify.genRand.Next((int)(-200f * num59), (int)(200f * num59));
					}
					TileRunner(num60, num62, (double)WorldModify.genRand.Next(4, 10), WorldModify.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
					if (WorldModify.genRand.Next(4) == 0)
					{
						int type5 = WorldModify.genRand.Next(63, 69);
						TileRunner(num60 + WorldModify.genRand.Next(-1, 2), num62 + WorldModify.genRand.Next(-1, 2), (double)WorldModify.genRand.Next(3, 7), WorldModify.genRand.Next(4, 8), type5, false, 0f, 0f, false, true);
					}
					num74++;
				}
				num60 = num67;
				num62 = num68;
				float num75 = (float)WorldModify.genRand.Next(6, 10);
				float num76 = (float)(Main.maxTilesX / 4200);
				num75 *= num76;
				int num77 = 0;
				while ((float)num77 < num75)
				{
					bool flag4 = true;
					while (flag4)
					{
						num60 = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
						num62 = WorldModify.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 300);
						if (Main.tile.At(num60, num62).Type == 59)
						{
							flag4 = false;
							int num78 = WorldModify.genRand.Next(2, 4);
							int num79 = WorldModify.genRand.Next(2, 4);
							for (int num80 = num60 - num78 - 1; num80 <= num60 + num78 + 1; num80++)
							{
								for (int num81 = num62 - num79 - 1; num81 <= num62 + num79 + 1; num81++)
								{
									Main.tile.At(num80, num81).SetActive(true);
									Main.tile.At(num80, num81).SetType(45);
									Main.tile.At(num80, num81).SetLiquid(0);
									Main.tile.At(num80, num81).SetLava(false);
								}
							}
							for (int num82 = num60 - num78; num82 <= num60 + num78; num82++)
							{
								for (int num83 = num62 - num79; num83 <= num62 + num79; num83++)
								{
									Main.tile.At(num82, num83).SetActive(false);
									Main.tile.At(num82, num83).SetWall(10);
								}
							}
							bool flag5 = false;
							int num84 = 0;
							while (!flag5 && num84 < 100)
							{
								num84++;
								int num85 = WorldModify.genRand.Next(num60 - num78, num60 + num78 + 1);
								int num86 = WorldModify.genRand.Next(num62 - num79, num62 + num79 - 2);
								WorldModify.PlaceTile(num85, num86, 4, true, false, -1, 0);
								if (Main.tile.At(num85, num86).Type == 4)
								{
									flag5 = true;
								}
							}
							for (int num87 = num60 - num78 - 1; num87 <= num60 + num78 + 1; num87++)
							{
								for (int num88 = num62 + num79 - 2; num88 <= num62 + num79; num88++)
								{
									Main.tile.At(num87, num88).SetActive(false);
								}
							}
							for (int num89 = num60 - num78 - 1; num89 <= num60 + num78 + 1; num89++)
							{
								for (int num90 = num62 + num79 - 2; num90 <= num62 + num79 - 1; num90++)
								{
									Main.tile.At(num89, num90).SetActive(false);
								}
							}
							for (int num91 = num60 - num78 - 1; num91 <= num60 + num78 + 1; num91++)
							{
								int num92 = 4;
								int num93 = num62 + num79 + 2;
								while (!Main.tile.At(num91, num93).Active && num93 < Main.maxTilesY && num92 > 0)
								{
									Main.tile.At(num91, num93).SetActive(true);
									Main.tile.At(num91, num93).SetType(59);
									num93++;
									num92--;
								}
							}
							num78 -= WorldModify.genRand.Next(1, 3);
							int num94 = num62 - num79 - 2;
							while (num78 > -1)
							{
								for (int num95 = num60 - num78 - 1; num95 <= num60 + num78 + 1; num95++)
								{
									Main.tile.At(num95, num94).SetActive(true);
									Main.tile.At(num95, num94).SetType(45);
								}
								num78 -= WorldModify.genRand.Next(1, 3);
								num94--;
							}
							JChestX[numJChests] = num60;
							JChestY[numJChests] = num62;
							numJChests++;
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
							WorldModify.SpreadGrass(num96, num97, 59, 60, true);
						}
					}
				}
			}

			numIslandHouses = 0;
			houseCount = 0;

			ProgramLog.Log("Generating floating islands...");
			for (int num98 = 0; num98 < WorldModify.ficount; num98++)
			{
				int num99 = 0;
				bool flag6 = false;
				int num100 = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
				bool flag7 = false;
				while (!flag7)
				{
					flag7 = true;
					while (num100 > Main.maxTilesX / 2 - 80 && num100 < Main.maxTilesX / 2 + 80)
					{
						num100 = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
					}
					for (int num101 = 0; num101 < numIslandHouses; num101++)
					{
						if (num100 > fihX[num101] - 80 && num100 < fihX[num101] + 80)
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
							int num104 = WorldModify.genRand.Next(90, num102 - 100);
							while ((double)num104 > num5 - 50.0)
							{
								num104--;
							}
							FloatingIsland(num103, num104);
							fihX[numIslandHouses] = num103;
							fihY[numIslandHouses] = num104;
							numIslandHouses++;
							break;
						}
						num102++;
					}
				}
			}

			ProgramLog.Log("Adding mushroom patches...");
			for (int num105 = 0; num105 < Main.maxTilesX / 300; num105++)
			{
				int i2 = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.3), (int)((double)Main.maxTilesX * 0.7));
				int j2 = WorldModify.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);
				ShroomPatch(i2, j2);
			}
			for (int num106 = 0; num106 < Main.maxTilesX; num106++)
			{
				for (int num107 = (int)Main.worldSurface; num107 < Main.maxTilesY; num107++)
				{
					if (Main.tile.At(num106, num107).Active)
					{
						WorldModify.SpreadGrass(num106, num107, 59, 70, false);
					}
				}
			}

			ProgramLog.Log("Placing mud in the dirt...");
			for (int num108 = 0; num108 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001); num108++)
			{
				TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(2, 6), WorldModify.genRand.Next(2, 40), 59, false, 0f, 0f, false, true);
			}

			{ // shinies
				double tiles = (double)(Main.maxTilesX * Main.maxTilesY);
				var num109max = (int)(tiles * 6E-05);
				var num110max = (int)(tiles * 8E-05);
				var num111max = (int)(tiles * 0.0002);
				var num112max = (int)(tiles * 3E-05);
				var num113max = (int)(tiles * 8E-05);
				var num114max = (int)(tiles * 0.0002);
				var num115max = (int)(tiles * 3E-05);
				var num116max = (int)(tiles * 0.00017);
				var num117max = (int)(tiles * 0.00017);
				var num118max = (int)(tiles * 0.00012);
				var num119max = (int)(tiles * 0.00012);
				var num120max = (int)(tiles * 2E-05);
				using (var prog = new ProgressLogger(num109max + num110max + num111max + num112max + num113max
					+ num114max + num115max + num116max + num117max + num118max + num119max + num120max - 12, "Adding shinies"))
				{
					int total = 0;

					for (int num109 = 0; num109 < num109max; num109++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num5, (int)num6), (double)WorldModify.genRand.Next(3, 6), WorldModify.genRand.Next(2, 6), 7, false, 0f, 0f, false, true);
					}
					for (int num110 = 0; num110 < num110max; num110++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num6, (int)num8), (double)WorldModify.genRand.Next(3, 7), WorldModify.genRand.Next(3, 7), 7, false, 0f, 0f, false, true);
					}
					for (int num111 = 0; num111 < num111max; num111++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(4, 9), WorldModify.genRand.Next(4, 8), 7, false, 0f, 0f, false, true);
					}
					for (int num112 = 0; num112 < num112max; num112++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num5, (int)num6), (double)WorldModify.genRand.Next(3, 7), WorldModify.genRand.Next(2, 5), 6, false, 0f, 0f, false, true);
					}
					for (int num113 = 0; num113 < num113max; num113++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num6, (int)num8), (double)WorldModify.genRand.Next(3, 6), WorldModify.genRand.Next(3, 6), 6, false, 0f, 0f, false, true);
					}
					for (int num114 = 0; num114 < num114max; num114++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(4, 9), WorldModify.genRand.Next(4, 8), 6, false, 0f, 0f, false, true);
					}
					for (int num115 = 0; num115 < num115max; num115++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num6, (int)num8), (double)WorldModify.genRand.Next(3, 6), WorldModify.genRand.Next(3, 6), 9, false, 0f, 0f, false, true);
					}
					for (int num116 = 0; num116 < num116max; num116++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(4, 9), WorldModify.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
					}
					for (int num117 = 0; num117 < num117max; num117++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(0, (int)num5), (double)WorldModify.genRand.Next(4, 9), WorldModify.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
					}
					for (int num118 = 0; num118 < num118max; num118++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(4, 8), WorldModify.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
					}
					for (int num119 = 0; num119 < num119max; num119++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(0, (int)num5 - 20), (double)WorldModify.genRand.Next(4, 8), WorldModify.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
					}
					for (int num120 = 0; num120 < num120max; num120++)
					{
						prog.Value = total++;
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(2, 4), WorldModify.genRand.Next(3, 6), 22, false, 0f, 0f, false, true);
					}
				}
			}

			ProgramLog.Log("Adding webs...");
			for (int num121 = 0; num121 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001); num121++)
			{
				int num122 = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
				int num123 = WorldModify.genRand.Next((int)num5, Main.maxTilesY - 20);
				if (num121 < numMCaves)
				{
					num122 = mCaveX[num121];
					num123 = mCaveY[num121];
				}
				if (!Main.tile.At(num122, num123).Active)
				{
					if ((double)num123 <= Main.worldSurface)
					{
						if (Main.tile.At(num122, num123).Wall <= 0)
						{
							continue;
						}
					}
					while (!Main.tile.At(num122, num123).Active && num123 > (int)num5)
					{
						num123--;
					}
					num123++;
					int num124 = 1;
					if (WorldModify.genRand.Next(2) == 0)
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
						TileRunner(num122, num123, (double)WorldModify.genRand.Next(4, 13), WorldModify.genRand.Next(2, 5), 51, true, (float)num124, -1f, false, false);
					}
				}
			}

			var num138max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002);
			using (var uwprog = new ProgressLogger(Main.maxTilesX * 5 - 10 + num138max, "Creating The Underworld"))
			{
				var uwtotal = 0;
				int num125 = Main.maxTilesY - WorldModify.genRand.Next(150, 190);
				for (int num126 = 0; num126 < Main.maxTilesX; num126++)
				{
					uwprog.Value = (uwtotal++);
					num125 += WorldModify.genRand.Next(-3, 4);
					if (num125 < Main.maxTilesY - 190)
					{
						num125 = Main.maxTilesY - 190;
					}
					if (num125 > Main.maxTilesY - 160)
					{
						num125 = Main.maxTilesY - 160;
					}
					for (int num127 = num125 - 20 - WorldModify.genRand.Next(3); num127 < Main.maxTilesY; num127++)
					{
						if (num127 >= num125)
						{
							Main.tile.At(num126, num127).SetActive(false);
							Main.tile.At(num126, num127).SetLava(false);
							Main.tile.At(num126, num127).SetLiquid(0);
						}
						else
						{
							Main.tile.At(num126, num127).SetType(57);
						}
					}
				}
				int num128 = Main.maxTilesY - WorldModify.genRand.Next(40, 70);
				for (int num129 = 10; num129 < Main.maxTilesX - 10; num129++)
				{
					uwprog.Value = (uwtotal++);
					num128 += WorldModify.genRand.Next(-10, 11);
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
							Main.tile.At(num129, num130).SetLava(true);
							Main.tile.At(num129, num130).SetLiquid(255);
						}
					}
				}
				for (int num131 = 0; num131 < Main.maxTilesX; num131++)
				{
					uwprog.Value = (uwtotal++);
					if (WorldModify.genRand.Next(50) == 0)
					{
						int num132 = Main.maxTilesY - 65;
						while (!Main.tile.At(num131, num132).Active && num132 > Main.maxTilesY - 135)
						{
							num132--;
						}
						TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), num132 + WorldModify.genRand.Next(20, 50), (double)WorldModify.genRand.Next(15, 20), 1000, 57, true, 0f, (float)WorldModify.genRand.Next(1, 3), true, true);
					}
				}
				ProgramLog.Log("Settling lava...");
				Liquid.QuickWater(-2, -1, -1);
				for (int num133 = 0; num133 < Main.maxTilesX; num133++)
				{
					uwprog.Value = (uwtotal++);

					if (WorldModify.genRand.Next(13) == 0)
					{
						int num135 = Main.maxTilesY - 65;
						while ((Main.tile.At(num133, num135).Liquid > 0 || Main.tile.At(num133, num135).Active) && num135 > Main.maxTilesY - 140)
						{
							num135--;
						}
						TileRunner(num133, num135 - WorldModify.genRand.Next(2, 5), (double)WorldModify.genRand.Next(5, 30), 1000, 57, true, 0f, (float)WorldModify.genRand.Next(1, 3), true, true);
						float num136 = (float)WorldModify.genRand.Next(1, 3);
						if (WorldModify.genRand.Next(3) == 0)
						{
							num136 *= 0.5f;
						}
						if (WorldModify.genRand.Next(2) == 0)
						{
							TileRunner(num133, num135 - WorldModify.genRand.Next(2, 5), (double)((int)((float)WorldModify.genRand.Next(5, 15) * num136)), (int)((float)WorldModify.genRand.Next(10, 15) * num136), 57, true, 1f, 0.3f, false, true);
						}
						if (WorldModify.genRand.Next(2) == 0)
						{
							num136 = (float)WorldModify.genRand.Next(1, 3);
							TileRunner(num133, num135 - WorldModify.genRand.Next(2, 5), (double)((int)((float)WorldModify.genRand.Next(5, 15) * num136)), (int)((float)WorldModify.genRand.Next(10, 15) * num136), 57, true, -1f, 0.3f, false, true);
						}
						TileRunner(num133 + WorldModify.genRand.Next(-10, 10), num135 + WorldModify.genRand.Next(-10, 10), (double)WorldModify.genRand.Next(5, 15), WorldModify.genRand.Next(5, 10), -2, false, (float)WorldModify.genRand.Next(-1, 3), (float)WorldModify.genRand.Next(-1, 3), false, true);
						if (WorldModify.genRand.Next(3) == 0)
						{
							TileRunner(num133 + WorldModify.genRand.Next(-10, 10), num135 + WorldModify.genRand.Next(-10, 10), (double)WorldModify.genRand.Next(10, 30), WorldModify.genRand.Next(10, 20), -2, false, (float)WorldModify.genRand.Next(-1, 3), (float)WorldModify.genRand.Next(-1, 3), false, true);
						}
						if (WorldModify.genRand.Next(5) == 0)
						{
							TileRunner(num133 + WorldModify.genRand.Next(-15, 15), num135 + WorldModify.genRand.Next(-15, 10), (double)WorldModify.genRand.Next(15, 30), WorldModify.genRand.Next(5, 20), -2, false, (float)WorldModify.genRand.Next(-1, 3), (float)WorldModify.genRand.Next(-1, 3), false, true);
						}
					}
				}
				for (int num137 = 0; num137 < Main.maxTilesX; num137++)
				{
					uwprog.Value = (uwtotal++);
					if (!Main.tile.At(num137, Main.maxTilesY - 145).Active)
					{
						Main.tile.At(num137, Main.maxTilesY - 145).SetLiquid(255);
						Main.tile.At(num137, Main.maxTilesY - 145).SetLava(true);
					}
					if (!Main.tile.At(num137, Main.maxTilesY - 144).Active)
					{
						Main.tile.At(num137, Main.maxTilesY - 144).SetLiquid(255);
						Main.tile.At(num137, Main.maxTilesY - 144).SetLava(true);
					}
				}
				for (int num138 = 0; num138 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002); num138++)
				{
					uwprog.Value = (uwtotal++);
					TileRunner(WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(Main.maxTilesY - 140, Main.maxTilesY), (double)WorldModify.genRand.Next(3, 8), WorldModify.genRand.Next(3, 8), 58, false, 0f, 0f, false, true);
				}
			}

			AddHellHouses();
			int num139 = WorldModify.genRand.Next(2, (int)((double)Main.maxTilesX * 0.005));

			using (var prog = new ProgressLogger(num139 - 1, "Adding water bodies"))
				for (int num140 = 0; num140 < num139; num140++)
				{
					prog.Value = num140;

					int num142 = WorldModify.genRand.Next(300, Main.maxTilesX - 300);
					while (num142 > Main.maxTilesX / 2 - 50 && num142 < Main.maxTilesX / 2 + 50)
					{
						num142 = WorldModify.genRand.Next(300, Main.maxTilesX - 300);
					}
					int num143 = (int)num5 - 20;
					while (!Main.tile.At(num142, num143).Active)
					{
						num143++;
					}
					Lakinater(num142, num143);
				}

			int x = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.05), (int)((double)Main.maxTilesX * 0.2)); //Left?
			int x2 = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.8), (int)((double)Main.maxTilesX * 0.95)); //Right?
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
				y = (int)((Main.rockLayer + (double)Main.maxTilesY) / 2.0) + WorldModify.genRand.Next(-200, 200); //Generate a custon Y each time
				if (dnum == 1)
				{
					//num9 = direction
					x = ((x == x2) ? x : x2); //we want the opposite of the original
				}
				MakeDungeon(x, y, 41, 7);
			}

			if (num9 != -1)
			{
				num9 = 1;
			}

			int num144 = 0;

			using (var prog = new ProgressLogger(100, "Making the world evil"))
			{
				while ((double)num144 < (double)Main.maxTilesX * 0.00045)
				{
					float num145 = (float)((double)num144 / ((double)Main.maxTilesX * 0.00045));

					prog.Value = (int)(num145 * 100f);

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
						num146 = WorldModify.genRand.Next(320, Main.maxTilesX - 320);
						num147 = num146 - WorldModify.genRand.Next(200) - 100;
						num148 = num146 + WorldModify.genRand.Next(200) + 100;
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
						if (num149 < 200 && JungleX > num147 && JungleX < num148)
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
										ChasmRunner(num155, num156, WorldModify.genRand.Next(150) + 150, true);
										break;
									}
									if (WorldModify.genRand.Next(35) == 0 && num154 == 0)
									{
										num154 = 30;
										bool makeOrb = true;
										ChasmRunner(num155, num156, WorldModify.genRand.Next(50) + 50, makeOrb);
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
								int num158 = num157 + WorldModify.genRand.Next(10, 14);
								for (int num159 = num157; num159 < num158; num159++)
								{
									if ((Main.tile.At(num155, num159).Type == 59 || Main.tile.At(num155, num159).Type == 60) && num155 >= num147 + WorldModify.genRand.Next(5) && num155 < num148 - WorldModify.genRand.Next(5))
									{
										Main.tile.At(num155, num159).SetType(0);
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
						num160 += (double)WorldModify.genRand.Next(-2, 3);
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
								if (Main.tile.At(num48, num162).Type == 53 && num48 >= num147 + WorldModify.genRand.Next(5) && num48 <= num148 - WorldModify.genRand.Next(5))
								{
									Main.tile.At(num48, num162).SetType(0);
								}
								if (Main.tile.At(num48, num162).Type == 0 && (double)num162 < Main.worldSurface - 1.0 && !flag9)
								{
									WorldModify.SpreadGrass(num48, num162, 0, 23, true);
								}
								flag9 = true;
								if (Main.tile.At(num48, num162).Type == 1 && num48 >= num147 + WorldModify.genRand.Next(5) && num48 <= num148 - WorldModify.genRand.Next(5))
								{
									Main.tile.At(num48, num162).SetType(25);
								}
								if (Main.tile.At(num48, num162).Type == 2)
								{
									Main.tile.At(num48, num162).SetType(23);
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
											if (Math.Abs(num169 - num163) + Math.Abs(num170 - num164) < 9 + WorldModify.genRand.Next(11) && WorldModify.genRand.Next(3) != 0 && Main.tile.At(num169, num170).Type != 31)
											{
												Main.tile.At(num169, num170).SetActive(true);
												Main.tile.At(num169, num170).SetType(25);
												if (Math.Abs(num169 - num163) <= 1 && Math.Abs(num170 - num164) <= 1)
												{
													Main.tile.At(num169, num170).SetActive(false);
												}
											}
											if (Main.tile.At(num169, num170).Type != 31 && Math.Abs(num169 - num163) <= 2 + WorldModify.genRand.Next(3) && Math.Abs(num170 - num164) <= 2 + WorldModify.genRand.Next(3))
											{
												Main.tile.At(num169, num170).SetActive(false);
											}
										}
									}
								}
							}
						}
					}
					num144++;
				}
			}

			ProgramLog.Log("Generating mountain caves...");
			for (int num171 = 0; num171 < numMCaves; num171++)
			{
				int i3 = mCaveX[num171];
				int j3 = mCaveY[num171];
				CaveOpenater(i3, j3);
				Cavinator(i3, j3, WorldModify.genRand.Next(40, 50));
			}
			int num172 = 0;
			int num173 = 0;
			int num174 = 20;
			int num175 = Main.maxTilesX - 20;

			ProgramLog.Log("Creating beaches...");
			for (int num176 = 0; num176 < 2; num176++)
			{
				int num177 = 0;
				int num178 = 0;
				if (num176 == 0)
				{
					num177 = 0;
					num178 = WorldModify.genRand.Next(125, 200) + 50;
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
					num181 += WorldModify.genRand.Next(1, 5);
					for (int num182 = num178 - 1; num182 >= num177; num182--)
					{
						num179++;
						if (num179 < 3)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.2f;
						}
						else if (num179 < 6)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.15f;
						}
						else if (num179 < 9)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.1f;
						}
						else if (num179 < 15)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.07f;
						}
						else if (num179 < 50)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
						}
						else if (num179 < 75)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.04f;
						}
						else if (num179 < 100)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.03f;
						}
						else if (num179 < 125)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.02f;
						}
						else if (num179 < 150)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
						}
						else if (num179 < 175)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.005f;
						}
						else if (num179 < 200)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.001f;
						}
						else if (num179 < 230)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
						}
						else if (num179 < 235)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
						}
						else if (num179 < 240)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.1f;
						}
						else if (num179 < 245)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
						}
						else if (num179 < 255)
						{
							num180 += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
						}

						if (num179 == 235)
						{
							num175 = num182;
						}
						if (num179 == 235)
						{
							num174 = num182;
						}
						int num183 = WorldModify.genRand.Next(15, 20);
						int num184 = 0;
						while ((float)num184 < (float)num181 + num180 + (float)num183)
						{
							if ((float)num184 < (float)num181 + num180 * 0.75f - 3f)
							{
								Main.tile.At(num182, num184).SetActive(false);
								if (num184 > num181)
								{
									Main.tile.At(num182, num184).SetLiquid(255);
								}
								else if (num184 == num181)
								{
									Main.tile.At(num182, num184).SetLiquid(127);
								}
							}
							else if (num184 > num181)
							{
								Main.tile.At(num182, num184).SetType(53);
								Main.tile.At(num182, num184).SetActive(true);
							}
							Main.tile.At(num182, num184).SetWall(0);
							num184++;
						}
					}
				}
				else
				{
					num177 = Main.maxTilesX - WorldModify.genRand.Next(125, 200) - 50;
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
					num187 += WorldModify.genRand.Next(1, 5);
					for (int num188 = num177; num188 < num178; num188++)
					{
						num186++;
						if (num186 < 3)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.2f;
						}
						else if (num186 < 6)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.15f;
						}
						else if (num186 < 9)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.1f;
						}
						else if (num186 < 15)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.07f;
						}
						else if (num186 < 50)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
						}
						else if (num186 < 75)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.04f;
						}
						else if (num186 < 100)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.03f;
						}
						else if (num186 < 125)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.02f;
						}
						else if (num186 < 150)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
						}
						else if (num186 < 175)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.005f;
						}
						else if (num186 < 200)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.001f;
						}
						else if (num186 < 230)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
						}
						else if (num186 < 235)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
						}
						else if (num186 < 240)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.1f;
						}
						else if (num186 < 245)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
						}
						else if (num186 < 255)
						{
							num185 += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
						}

						if (num186 == 235)
						{
							num175 = num188;
						}
						int num189 = WorldModify.genRand.Next(15, 20);
						int num190 = 0;
						while ((float)num190 < (float)num187 + num185 + (float)num189)
						{
							if ((float)num190 < (float)num187 + num185 * 0.75f - 3f && (double)num190 < Main.worldSurface - 2.0)
							{
								Main.tile.At(num188, num190).SetActive(false);
								if (num190 > num187)
								{
									Main.tile.At(num188, num190).SetLiquid(255);
								}
								else if (num190 == num187)
								{
									Main.tile.At(num188, num190).SetLiquid(127);
								}
							}
							else if (num190 > num187)
							{
								Main.tile.At(num188, num190).SetType(53);
								Main.tile.At(num188, num190).SetActive(true);
							}
							Main.tile.At(num188, num190).SetWall(0);
							num190++;
						}
					}
				}
			} // end beaches

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

			ProgramLog.Log("Adding gems...");
			for (int num191 = 63; num191 <= 68; num191++)
			{
				float num192 = 0f;
				if (num191 == 67)
				{
					num192 = (float)Main.maxTilesX * 0.5f;
				}
				else if (num191 == 66)
				{
					num192 = (float)Main.maxTilesX * 0.45f;
				}
				else if (num191 == 63)
				{
					num192 = (float)Main.maxTilesX * 0.3f;
				}
				else if (num191 == 65)
				{
					num192 = (float)Main.maxTilesX * 0.25f;
				}
				else if (num191 == 64)
				{
					num192 = (float)Main.maxTilesX * 0.1f;
				}
				else if (num191 == 68)
				{
					num192 = (float)Main.maxTilesX * 0.05f;
				}
				num192 *= 0.2f;
				int num193 = 0;
				while ((float)num193 < num192)
				{
					int num194 = WorldModify.genRand.Next(0, Main.maxTilesX);
					int num195 = WorldModify.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
					while (Main.tile.At(num194, num195).Type != 1)
					{
						num194 = WorldModify.genRand.Next(0, Main.maxTilesX);
						num195 = WorldModify.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
					}
					TileRunner(num194, num195, (double)WorldModify.genRand.Next(2, 6), WorldModify.genRand.Next(3, 7), num191, false, 0f, 0f, false, true);
					num193++;
				}
			} // end gems

			using (var prog = new ProgressLogger(Main.maxTilesX - 1, "Gravitating sand"))
				for (int num196 = 0; num196 < Main.maxTilesX; num196++)
				{
					prog.Value = num196;

					for (int num198 = Main.maxTilesY - 5; num198 > 0; num198--)
					{
						if (Main.tile.At(num196, num198).Active && Main.tile.At(num196, num198).Type == 53)
						{
							int num199 = num198;
							while (!Main.tile.At(num196, num199 + 1).Active && num199 < Main.maxTilesY - 5)
							{
								Main.tile.At(num196, num199 + 1).SetActive(true);
								Main.tile.At(num196, num199 + 1).SetType(53);
								num199++;
							}
						}
					}
				}

			using (var prog = new ProgressLogger(Main.maxTilesX - 7, "Cleaning up dirt backgrounds"))
			{
				for (int num200 = 3; num200 < Main.maxTilesX - 3; num200++)
				{
					prog.Value = num200 - 3;

					bool flag10 = true;
					int num202 = 0;
					while ((double)num202 < Main.worldSurface)
					{
						if (flag10)
						{
							if (Main.tile.At(num200, num202).Wall == 2)
							{
								Main.tile.At(num200, num202).SetWall(0);
							}
							if (Main.tile.At(num200, num202).Type != 53)
							{
								if (Main.tile.At(num200 - 1, num202).Wall == 2)
								{
									Main.tile.At(num200 - 1, num202).SetWall(0);
								}
								if (Main.tile.At(num200 - 2, num202).Wall == 2 && WorldModify.genRand.Next(2) == 0)
								{
									Main.tile.At(num200 - 2, num202).SetWall(0);
								}
								if (Main.tile.At(num200 - 3, num202).Wall == 2 && WorldModify.genRand.Next(2) == 0)
								{
									Main.tile.At(num200 - 3, num202).SetWall(0);
								}
								if (Main.tile.At(num200 + 1, num202).Wall == 2)
								{
									Main.tile.At(num200 + 1, num202).SetWall(0);
								}
								if (Main.tile.At(num200 + 2, num202).Wall == 2 && WorldModify.genRand.Next(2) == 0)
								{
									Main.tile.At(num200 + 2, num202).SetWall(0);
								}
								if (Main.tile.At(num200 + 3, num202).Wall == 2 && WorldModify.genRand.Next(2) == 0)
								{
									Main.tile.At(num200 + 3, num202).SetWall(0);
								}
								if (Main.tile.At(num200, num202).Active)
								{
									flag10 = false;
								}
							}
						}
						else if (Main.tile.At(num200, num202).Wall == 0 && Main.tile.At(num200, num202 + 1).Wall == 0 && Main.tile.At(num200, num202 + 2).Wall == 0 && Main.tile.At(num200, num202 + 3).Wall == 0 && Main.tile.At(num200, num202 + 4).Wall == 0 && Main.tile.At(num200 - 1, num202).Wall == 0 && Main.tile.At(num200 + 1, num202).Wall == 0 && Main.tile.At(num200 - 2, num202).Wall == 0 && Main.tile.At(num200 + 2, num202).Wall == 0 && !Main.tile.At(num200, num202).Active && !Main.tile.At(num200, num202 + 1).Active && !Main.tile.At(num200, num202 + 2).Active && !Main.tile.At(num200, num202 + 3).Active)
						{
							flag10 = true;
						}
						num202++;
					}
				}
			} // end cleaning up

			var num203max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05);
			using (var prog = new ProgressLogger(num203max - 1, "Placing altars"))
				for (int num203 = 0; num203 < num203max; num203++)
				{
					prog.Value = num203;

					bool flag11 = false;
					int num205 = 0;
					while (!flag11)
					{
						int num206 = WorldModify.genRand.Next(1, Main.maxTilesX);
						int num207 = (int)(num6 + 20.0);
						WorldModify.Place3x2(num206, num207, 26);
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
							Main.tile.At(num48, num209 - 1).SetLiquid(255);
							Main.tile.At(num48, num209 - 2).SetLiquid(255);
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
								Main.tile.At(num48, num212).SetLiquid(0);
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

			using (var prog = new ProgressLogger(100, "Settling liquids"))
			{
				Liquid.QuickWater(3, -1, -1);
				WorldModify.WaterCheck();
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
							prog.Value = (int)(num216 * 100f / 3f + 33f);
						}
						int num217 = 10;
						if (num213 <= num217)
						{
							goto IL_4CC6;
						}
					IL_4CC6:
						Liquid.UpdateLiquid();
					}
					WorldModify.WaterCheck();

					prog.Value = (int)((float)num213 * 10f / 3f + 66f);
				}
				Liquid.quickSettle = false;
			} // end settling

			var num218max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.2E-05);
			using (var prog = new ProgressLogger(num218max - 1, "Placing life crystals"))
				for (int num218 = 0; num218 < num218max; num218++)
				{
					prog.Value = num218;
					bool flag12 = false;
					int num220 = 0;
					while (!flag12)
					{
						if (AddLifeCrystal(WorldModify.genRand.Next(1, Main.maxTilesX), WorldModify.genRand.Next((int)(num6 + 20.0), Main.maxTilesY)))
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

			var num221max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 1.7E-05);
			using (var prog = new ProgressLogger(num221max - 1, "Hiding treasure"))
				for (int num221 = 0; num221 < num221max; num221++)
				{
					prog.Value = num221;
					bool flag13 = false;
					int num223 = 0;
					while (!flag13)
					{
						int num224 = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
						int num225 = WorldModify.genRand.Next((int)(num6 + 20.0), Main.maxTilesY - 20);
						while (Main.tile.At(num224, num225).Wall > 0)
						{
							num224 = WorldModify.genRand.Next(1, Main.maxTilesX);
							num225 = WorldModify.genRand.Next((int)(num6 + 20.0), Main.maxTilesY - 20);
						}
						if (AddBuriedChest(num224, num225, 0, false))
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

			var num226max = (int)((double)Main.maxTilesX * 0.005);
			using (var prog = new ProgressLogger(num226max - 1, "Hiding more treasure"))
				for (int num226 = 0; num226 < num226max; num226++)
				{
					prog.Value = num226;
					bool flag14 = false;
					int num228 = 0;
					while (!flag14)
					{
						int num229 = WorldModify.genRand.Next(300, Main.maxTilesX - 300);
						int num230 = WorldModify.genRand.Next((int)num5, (int)Main.worldSurface);
						bool flag15 = false;
						if (Main.tile.At(num229, num230).Wall == 2)
						{
							flag15 = true;
						}
						if (flag15 && AddBuriedChest(num229, num230, 0, true))
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
			for (int num232 = 0; num232 < numJChests; num232++)
			{
				num231++;
				int contain = 211;
				if (num231 == 1)
				{
					contain = 211;
				}
				else if (num231 == 2)
				{
					contain = 212;
				}
				else if (num231 == 3)
				{
					contain = 213;
				}
				if (num231 > 3)
				{
					num231 = 0;
				}
				if (!AddBuriedChest(JChestX[num232] + WorldModify.genRand.Next(2), JChestY[num232], contain, false))
				{
					for (int num233 = JChestX[num232]; num233 <= JChestX[num232] + 1; num233++)
					{
						for (int num234 = JChestY[num232]; num234 <= JChestY[num232] + 1; num234++)
						{
							WorldModify.KillTile(num233, num234, false, false, false);
						}
					}
					AddBuriedChest(JChestX[num232], JChestY[num232], contain, false);
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
				else if (num236 == 2)
				{
					contain2 = 277;
				}
				else
				{
					contain2 = 187;
					num236 = 0;
				}
				bool flag16 = false;
				while (!flag16)
				{
					int num238 = WorldModify.genRand.Next(1, Main.maxTilesX);
					int num239 = WorldModify.genRand.Next(1, Main.maxTilesY - 200);
					while (Main.tile.At(num238, num239).Liquid < 200 || Main.tile.At(num238, num239).Lava)
					{
						num238 = WorldModify.genRand.Next(1, Main.maxTilesX);
						num239 = WorldModify.genRand.Next(1, Main.maxTilesY - 200);
					}
					flag16 = AddBuriedChest(num238, num239, contain2, true);
				}
				num237++;
			}
			for (int num240 = 0; num240 < numIslandHouses; num240++)
			{
				IslandHouse(fihX[num240], fihY[num240]);
			}

			var num241max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008);
			using (var prog = new ProgressLogger(num241max - 1, "Placing breakables"))
			{
				for (int num241 = 0; num241 < num241max; num241++)
				{
					float num242 = (float)((double)num241 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008));

					prog.Value = num241;

					bool flag17 = false;
					int num243 = 0;
					while (!flag17)
					{
						int num244 = WorldModify.genRand.Next((int)num6, Main.maxTilesY - 10);
						if ((double)num242 > 0.93)
						{
							num244 = Main.maxTilesY - 150;
						}
						else if ((double)num242 > 0.75)
						{
							num244 = (int)num5;
						}
						int num245 = WorldModify.genRand.Next(1, Main.maxTilesX);
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
								if (WorldModify.PlacePot(num245, num246, 28))
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
			} // end breakables

			var num247max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0001);
			using (var prog = new ProgressLogger(num247max - 1, "Placing hellforges"))
			{
				for (int num247 = 0; num247 < num247max; num247++)
				{
					prog.Value = num247;

					bool flag19 = false;
					int num249 = 0;
					while (!flag19)
					{
						int num250 = WorldModify.genRand.Next(1, Main.maxTilesX);
						int num251 = WorldModify.genRand.Next(Main.maxTilesY - 250, Main.maxTilesY - 5);
						try
						{
							if (Main.tile.At(num250, num251).Wall == 13)
							{
								while (!Main.tile.At(num250, num251).Active)
								{
									num251++;
								}
								num251--;
								WorldModify.PlaceTile(num250, num251, 77, false, false, -1, 0);
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
			} // end hellforges

			ProgramLog.Log("Spreading grass...");
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
							WorldModify.SpreadGrass(num48, num253, 0, 2, true);
						}
						if ((double)num253 > num6)
						{
							break;
						}
						flag20 = false;
					}
					else if (Main.tile.At(num48, num253).Wall == 0)
					{
						flag20 = true;
					}
					num253++;
				}
			}

			ProgramLog.Log("Growing cacti...");
			for (int num254 = 5; num254 < Main.maxTilesX - 5; num254++)
			{
				if (WorldModify.genRand.Next(8) == 0)
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
									WorldModify.PlaceTile(num254, num255 - 1, 81, true, false, -1, 0);
								}
							}
							else if (num254 > 400 && num254 < Main.maxTilesX - 400)
							{
								WorldModify.PlantCactus(num254, num255);
							}
						}
						num255++;
					}
				}
			} // end cacti
			int num256 = 5;
			bool flag21 = true;
			while (flag21)
			{
				int num257 = Main.maxTilesX / 2 + WorldModify.genRand.Next(-num256, num256 + 1);
				for (int num258 = 0; num258 < Main.maxTilesY; num258++)
				{
					if (Main.tile.At(num257, num258).Active)
					{
						Main.spawnTileX = num257;
						Main.spawnTileY = num258;
						Main.tile.At(num257, num258 - 1).SetLighted(true);
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
				int num260 = WorldModify.genRand.Next(Main.maxTilesX / 2 - num259, Main.maxTilesX / 2 + num259);
				for (int num261 = 0; num261 < Main.maxTilesY; num261++)
				{
					if (Main.tile.At(num260, num261).Active)
					{
						Main.spawnTileX = num260;
						Main.spawnTileY = num261;
						Main.tile.At(num260, num261 - 1).SetLighted(true);
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

			ProgramLog.Log("Planting sunflowers...");
			int num263 = 0;
			while ((double)num263 < (double)Main.maxTilesX * 0.002)
			{
				int num264 = 0;
				int num265 = 0;
				int arg_5A6F_0 = Main.maxTilesX / 2;
				int num266 = WorldModify.genRand.Next(Main.maxTilesX);
				num264 = num266 - WorldModify.genRand.Next(10) - 7;
				num265 = num266 + WorldModify.genRand.Next(10) + 7;
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
							WorldModify.PlaceTile(num267, num268 - 1, 27, true, false, -1, 0);
						}
						if (Main.tile.At(num267, num268).Active)
						{
							break;
						}
						num268++;
					}
				}
				num263++;
			} // end sunflowers

			ProgramLog.Log("Planting trees...");
			int num269 = 0;
			while ((double)num269 < (double)Main.maxTilesX * 0.003)
			{
				int num270 = WorldModify.genRand.Next(50, Main.maxTilesX - 50);
				int num271 = WorldModify.genRand.Next(25, 50);
				for (int num272 = num270 - num271; num272 < num270 + num271; num272++)
				{
					int num273 = 20;
					while ((double)num273 < Main.worldSurface)
					{
						GrowEpicTree(num272, num273);
						num273++;
					}
				}
				num269++;
			}
			AddTrees();

			ProgramLog.Log("Planting herbs...");
			for (int num274 = 0; num274 < Main.maxTilesX * 2; num274++)
			{
				WorldModify.PlantAlch();
			}

			ProgramLog.Log("Planting weeds...");
			AddPlants();
			for (int num275 = 0; num275 < Main.maxTilesX; num275++)
			{
				for (int num276 = 0; num276 < Main.maxTilesY; num276++)
				{
					if (Main.tile.At(num275, num276).Active)
					{
						if (num276 >= (int)Main.worldSurface && Main.tile.At(num275, num276).Type == 70 && !Main.tile.At(num275, num276 - 1).Active)
						{
							WorldModify.GrowShroom(num275, num276);
							if (!Main.tile.At(num275, num276 - 1).Active)
							{
								WorldModify.PlaceTile(num275, num276 - 1, 71, true, false, -1, 0);
							}
						}
						if (Main.tile.At(num275, num276).Type == 60 && !Main.tile.At(num275, num276 - 1).Active)
						{
							WorldModify.PlaceTile(num275, num276 - 1, 61, true, false, -1, 0);
						}
					}
				}
			}

			ProgramLog.Log("Growing vines...");
			for (int num277 = 0; num277 < Main.maxTilesX; num277++)
			{
				int num278 = 0;
				int num279 = 0;
				while ((double)num279 < Main.worldSurface)
				{
					if (num278 > 0 && !Main.tile.At(num277, num279).Active)
					{
						Main.tile.At(num277, num279).SetActive(true);
						Main.tile.At(num277, num279).SetType(52);
						num278--;
					}
					else
					{
						num278 = 0;
					}
					if (Main.tile.At(num277, num279).Active && Main.tile.At(num277, num279).Type == 2 && WorldModify.genRand.Next(5) < 3)
					{
						num278 = WorldModify.genRand.Next(1, 10);
					}
					num279++;
				}
				num278 = 0;
				for (int num280 = 0; num280 < Main.maxTilesY; num280++)
				{
					if (num278 > 0 && !Main.tile.At(num277, num280).Active)
					{
						Main.tile.At(num277, num280).SetActive(true);
						Main.tile.At(num277, num280).SetType(62);
						num278--;
					}
					else
					{
						num278 = 0;
					}
					if (Main.tile.At(num277, num280).Active && Main.tile.At(num277, num280).Type == 60 && WorldModify.genRand.Next(5) < 3)
					{
						num278 = WorldModify.genRand.Next(1, 10);
					}
				}
			}

			ProgramLog.Log("Planting flowers...");
			int num281 = 0;
			while ((double)num281 < (double)Main.maxTilesX * 0.005)
			{
				int num282 = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
				int num283 = WorldModify.genRand.Next(5, 15);
				int num284 = WorldModify.genRand.Next(15, 30);
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
									Main.tile.At(num286, num287).SetFrameX((short)(WorldModify.genRand.Next(6, 8) * 18));
								}
							}
						}
						break;
					}
					num285++;
				}
				num281++;
			}

			ProgramLog.Log("Planting mushrooms...");
			int num288 = 0;
			while ((double)num288 < (double)Main.maxTilesX * 0.002)
			{
				int num289 = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
				int num290 = WorldModify.genRand.Next(4, 10);
				int num291 = WorldModify.genRand.Next(15, 30);
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
									Main.tile.At(num293, num294).SetFrameX(144);
								}
							}
						}
						break;
					}
					num292++;
				}
				num288++;
			}
			WorldModify.gen = false;
		}

		public static bool AddBuriedChest(int i, int j, int contain = 0, bool notNearOtherChests = false)
		{
			if (WorldModify.genRand == null)
			{
				WorldModify.genRand = new Random((int)DateTime.Now.Ticks);
			}
			int k = j;
			while (k < Main.maxTilesY)
			{
				if (Main.tile.At(i, k).Active && Main.tileSolid[(int)Main.tile.At(i, k).Type])
				{
					int num = k;
					int style = 0;
					if ((double)num >= Main.worldSurface + 25.0 || contain > 0)
					{
						style = 1;
					}
					int num2 = WorldModify.PlaceChest(i - 1, num - 1, 21, notNearOtherChests, style);
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
									int num4 = WorldModify.genRand.Next(6);
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
										Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(50, 75);
									}
									if (num4 == 4)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(279);
										Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(25, 50);
									}
									if (num4 == 5)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(285);
									}
									num3++;
								}
								if (WorldModify.genRand.Next(3) == 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(168);
									Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(3, 6);
									num3++;
								}
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num5 = WorldModify.genRand.Next(2);
									int stack = WorldModify.genRand.Next(8) + 3;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num6 = WorldModify.genRand.Next(2);
									int stack2 = WorldModify.genRand.Next(26) + 25;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num7 = WorldModify.genRand.Next(1);
									int stack3 = WorldModify.genRand.Next(3) + 3;
									if (num7 == 0)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(28);
									}
									Main.chest[num2].contents[num3].Stack = stack3;
									num3++;
								}
								if (WorldModify.genRand.Next(3) > 0)
								{
									int num8 = WorldModify.genRand.Next(4);
									int stack4 = WorldModify.genRand.Next(1, 3);
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num9 = WorldModify.genRand.Next(2);
									int stack5 = WorldModify.genRand.Next(11) + 10;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(72);
									Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(10, 30);
									num3++;
								}
							}
							else if ((double)num < Main.rockLayer)
							{
								if (contain > 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(contain);
									num3++;
								}
								else
								{
									int num10 = WorldModify.genRand.Next(7);
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
										Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(26) + 25;
									}
									num3++;
								}
								if (WorldModify.genRand.Next(3) == 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(166);
									Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(10, 20);
									num3++;
								}
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num11 = WorldModify.genRand.Next(2);
									int stack6 = WorldModify.genRand.Next(10) + 5;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num12 = WorldModify.genRand.Next(2);
									int stack7 = WorldModify.genRand.Next(25) + 25;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num13 = WorldModify.genRand.Next(1);
									int stack8 = WorldModify.genRand.Next(3) + 3;
									if (num13 == 0)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(28);
									}
									Main.chest[num2].contents[num3].Stack = stack8;
									num3++;
								}
								if (WorldModify.genRand.Next(3) > 0)
								{
									int num14 = WorldModify.genRand.Next(7);
									int stack9 = WorldModify.genRand.Next(1, 3);
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int stack10 = WorldModify.genRand.Next(11) + 10;
									Main.chest[num2].contents[num3] = Registries.Item.Create(8);
									Main.chest[num2].contents[num3].Stack = stack10;
									num3++;
								}
								if (WorldModify.genRand.Next(2) == 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(72);
									Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(50, 90);
									num3++;
								}
							}
							else if (num < Main.maxTilesY - 250)
							{
								if (contain > 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(contain);
									num3++;
								}
								else
								{
									int num15 = WorldModify.genRand.Next(7);
									if (num15 == 2 && WorldModify.genRand.Next(2) == 0)
									{
										num15 = WorldModify.genRand.Next(7);
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
										Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(26) + 25;
									}
									num3++;
								}
								if (WorldModify.genRand.Next(3) == 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(167);
									num3++;
								}
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num16 = WorldModify.genRand.Next(2);
									int stack11 = WorldModify.genRand.Next(8) + 3;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num17 = WorldModify.genRand.Next(2);
									int stack12 = WorldModify.genRand.Next(26) + 25;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num18 = WorldModify.genRand.Next(1);
									int stack13 = WorldModify.genRand.Next(3) + 3;
									if (num18 == 0)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(188);
									}
									Main.chest[num2].contents[num3].Stack = stack13;
									num3++;
								}
								if (WorldModify.genRand.Next(3) > 0)
								{
									int num19 = WorldModify.genRand.Next(6);
									int stack14 = WorldModify.genRand.Next(1, 3);
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
								if (WorldModify.genRand.Next(3) > 1)
								{
									int num20 = WorldModify.genRand.Next(4);
									int stack15 = WorldModify.genRand.Next(1, 3);
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num21 = WorldModify.genRand.Next(2);
									int stack16 = WorldModify.genRand.Next(15) + 15;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(73);
									Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(1, 3);
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
									if (hellChest == 0)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(274);
									}
									else
									{
										int num22 = WorldModify.genRand.Next(4);
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
									hellChest++;
								}
								if (WorldModify.genRand.Next(3) == 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(167);
									num3++;
								}
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num23 = WorldModify.genRand.Next(2);
									int stack17 = WorldModify.genRand.Next(15) + 15;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num24 = WorldModify.genRand.Next(2);
									int stack18 = WorldModify.genRand.Next(25) + 50;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num25 = WorldModify.genRand.Next(2);
									int stack19 = WorldModify.genRand.Next(15) + 15;
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
								if (WorldModify.genRand.Next(4) > 0)
								{
									int num26 = WorldModify.genRand.Next(7);
									int stack20 = WorldModify.genRand.Next(1, 3);
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
								if (WorldModify.genRand.Next(3) > 0)
								{
									int num27 = WorldModify.genRand.Next(5);
									int stack21 = WorldModify.genRand.Next(1, 3);
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									int num28 = WorldModify.genRand.Next(2);
									int stack22 = WorldModify.genRand.Next(15) + 15;
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
								if (WorldModify.genRand.Next(2) == 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(73);
									Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(2, 5);
									num3++;
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

		public static void MakeDungeon(int x, int y, int tileType = 41, int wallType = 7)
		{
			using (var prog = new ProgressLogger(100, "Creating dungeon"))
			{
				int num = WorldModify.genRand.Next(3);
				int num2 = WorldModify.genRand.Next(3);
				if (num == 1)
				{
					tileType = 43;
				}
				else if (num == 2)
				{
					tileType = 44;
				}
				if (num2 == 1)
				{
					wallType = 8;
				}
				else if (num2 == 2)
				{
					wallType = 9;
				}
				numDDoors = 0;
				numDPlats = 0;
				numDRooms = 0;
				dungeonX = x;
				dungeonY = y;
				dMinX = x;
				dMaxX = x;
				dMinY = y;
				dMaxY = y;
				dxStrength1 = (double)WorldModify.genRand.Next(25, 30);
				dyStrength1 = (double)WorldModify.genRand.Next(20, 25);
				dxStrength2 = (double)WorldModify.genRand.Next(35, 50);
				dyStrength2 = (double)WorldModify.genRand.Next(10, 15);
				float num3 = (float)(Main.maxTilesX / 60);
				num3 += (float)WorldModify.genRand.Next(0, (int)(num3 / 3f));
				float num4 = num3;
				int num5 = 5;
				DungeonRoom(dungeonX, dungeonY, tileType, wallType);
				while (num3 > 0f)
				{
					if (dungeonX < dMinX)
					{
						dMinX = dungeonX;
					}
					if (dungeonX > dMaxX)
					{
						dMaxX = dungeonX;
					}
					if (dungeonY > dMaxY)
					{
						dMaxY = dungeonY;
					}
					num3 -= 1f;

					prog.Value = (int)((num4 - num3) / num4 * 60f);

					if (num5 > 0)
					{
						num5--;
					}
					if (num5 == 0 & WorldModify.genRand.Next(3) == 0)
					{
						num5 = 5;
						if (WorldModify.genRand.Next(2) == 0)
						{
							int num6 = dungeonX;
							int num7 = dungeonY;
							DungeonHalls(dungeonX, dungeonY, tileType, wallType, false);
							if (WorldModify.genRand.Next(2) == 0)
							{
								DungeonHalls(dungeonX, dungeonY, tileType, wallType, false);
							}
							DungeonRoom(dungeonX, dungeonY, tileType, wallType);
							dungeonX = num6;
							dungeonY = num7;
						}
						else
						{
							DungeonRoom(dungeonX, dungeonY, tileType, wallType);
						}
					}
					else
					{
						DungeonHalls(dungeonX, dungeonY, tileType, wallType, false);
					}
				}
				DungeonRoom(dungeonX, dungeonY, tileType, wallType);
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
				dungeonX = num8;
				dungeonY = num9;
				dEntranceX = num8;
				dSurface = false;
				num5 = 5;
				while (!dSurface)
				{
					if (num5 > 0)
					{
						num5--;
					}
					if ((num5 == 0 & WorldModify.genRand.Next(5) == 0) && (double)dungeonY > Main.worldSurface + 50.0)
					{
						num5 = 10;
						int num10 = dungeonX;
						int num11 = dungeonY;
						DungeonHalls(dungeonX, dungeonY, tileType, wallType, true);
						DungeonRoom(dungeonX, dungeonY, tileType, wallType);
						dungeonX = num10;
						dungeonY = num11;
					}
					DungeonStairs(dungeonX, dungeonY, tileType, wallType);
				}
				DungeonEnt(dungeonX, dungeonY, tileType, wallType);

				prog.Value = 65;

				for (int j = 0; j < numDRooms; j++)
				{
					for (int k = dRoomL[j]; k <= dRoomR[j]; k++)
					{
						if (!Main.tile.At(k, dRoomT[j] - 1).Active)
						{
							DPlatX[numDPlats] = k;
							DPlatY[numDPlats] = dRoomT[j] - 1;
							numDPlats++;
							break;
						}
					}
					for (int l = dRoomL[j]; l <= dRoomR[j]; l++)
					{
						if (!Main.tile.At(l, dRoomB[j] + 1).Active)
						{
							DPlatX[numDPlats] = l;
							DPlatY[numDPlats] = dRoomB[j] + 1;
							numDPlats++;
							break;
						}
					}
					for (int m = dRoomT[j]; m <= dRoomB[j]; m++)
					{
						if (!Main.tile.At(dRoomL[j] - 1, m).Active)
						{
							DDoorX[numDDoors] = dRoomL[j] - 1;
							DDoorY[numDDoors] = m;
							DDoorPos[numDDoors] = -1;
							numDDoors++;
							break;
						}
					}
					for (int n = dRoomT[j]; n <= dRoomB[j]; n++)
					{
						if (!Main.tile.At(dRoomR[j] + 1, n).Active)
						{
                            if (numDDoors <= DDoorX.Length)
                            {
                                DDoorX[numDDoors] = dRoomR[j] + 1;
                                DDoorY[numDDoors] = n;
                                DDoorPos[numDDoors] = 1;
                                numDDoors++;
                            }
							break;
						}
					}
				}

				prog.Value = 70;

				int num12 = 0;
				int num13 = 1000;
				int num14 = 0;
				while (num14 < Main.maxTilesX / 100)
				{
					num12++;
					int num15 = WorldModify.genRand.Next(dMinX, dMaxX);
					int num16 = WorldModify.genRand.Next((int)Main.worldSurface + 25, dMaxY);
					int num17 = num15;
					if ((int)Main.tile.At(num15, num16).Wall == wallType && !Main.tile.At(num15, num16).Active)
					{
						int num18 = 1;
						if (WorldModify.genRand.Next(2) == 0)
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
							int num19 = WorldModify.genRand.Next(5, 13);
							while (Main.tile.At(num15 - 1, num16).Active && Main.tile.At(num15, num16 + num18).Active && Main.tile.At(num15, num16).Active && !Main.tile.At(num15, num16 - num18).Active && num19 > 0)
							{
								Main.tile.At(num15, num16).SetType(48);
								if (!Main.tile.At(num15 - 1, num16 - num18).Active && !Main.tile.At(num15 + 1, num16 - num18).Active)
								{
									Main.tile.At(num15, num16 - num18).SetType(48);
									Main.tile.At(num15, num16 - num18).SetActive(true);
								}
								num15--;
								num19--;
							}
							num19 = WorldModify.genRand.Next(5, 13);
							num15 = num17 + 1;
							while (Main.tile.At(num15 + 1, num16).Active && Main.tile.At(num15, num16 + num18).Active && Main.tile.At(num15, num16).Active && !Main.tile.At(num15, num16 - num18).Active && num19 > 0)
							{
								Main.tile.At(num15, num16).SetType(48);
								if (!Main.tile.At(num15 - 1, num16 - num18).Active && !Main.tile.At(num15 + 1, num16 - num18).Active)
								{
									Main.tile.At(num15, num16 - num18).SetType(48);
									Main.tile.At(num15, num16 - num18).SetActive(true);
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

				prog.Value = 75;

				while (num14 < Main.maxTilesX / 100)
				{
					num12++;
					int num20 = WorldModify.genRand.Next(dMinX, dMaxX);
					int num21 = WorldModify.genRand.Next((int)Main.worldSurface + 25, dMaxY);
					int num22 = num21;
					if ((int)Main.tile.At(num20, num21).Wall == wallType && !Main.tile.At(num20, num21).Active)
					{
						int num23 = 1;
						if (WorldModify.genRand.Next(2) == 0)
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
							int num24 = WorldModify.genRand.Next(5, 13);
							while (Main.tile.At(num20, num21 - 1).Active && Main.tile.At(num20 + num23, num21).Active && Main.tile.At(num20, num21).Active && !Main.tile.At(num20 - num23, num21).Active && num24 > 0)
							{
								Main.tile.At(num20, num21).SetType(48);
								if (!Main.tile.At(num20 - num23, num21 - 1).Active && !Main.tile.At(num20 - num23, num21 + 1).Active)
								{
									Main.tile.At(num20 - num23, num21).SetType(48);
									Main.tile.At(num20 - num23, num21).SetActive(true);
								}
								num21--;
								num24--;
							}
							num24 = WorldModify.genRand.Next(5, 13);
							num21 = num22 + 1;
							while (Main.tile.At(num20, num21 + 1).Active && Main.tile.At(num20 + num23, num21).Active && Main.tile.At(num20, num21).Active && !Main.tile.At(num20 - num23, num21).Active && num24 > 0)
							{
								Main.tile.At(num20, num21).SetType(48);
								if (!Main.tile.At(num20 - num23, num21 - 1).Active && !Main.tile.At(num20 - num23, num21 + 1).Active)
								{
									Main.tile.At(num20 - num23, num21).SetType(48);
									Main.tile.At(num20 - num23, num21).SetActive(true);
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

				prog.Value = 80;

				for (int num25 = 0; num25 < numDDoors; num25++)
				{
					int num26 = DDoorX[num25] - 10;
					int num27 = DDoorX[num25] + 10;
					int num28 = 100;
					int num29 = 0;
					for (int num30 = num26; num30 < num27; num30++)
					{
						bool flag = true;
						int num31 = DDoorY[num25];
						while (!Main.tile.At(num30, num31).Active)
						{
							num31--;
						}
						if (!Main.tileDungeon[(int)Main.tile.At(num30, num31).Type])
						{
							flag = false;
						}
						int num32 = num31;
						num31 = DDoorY[num25];
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
								if (DDoorPos[num25] == 0 && num33 - num32 < num28)
								{
									flag2 = true;
								}
								if (DDoorPos[num25] == -1 && num30 > num29)
								{
									flag2 = true;
								}
								if (DDoorPos[num25] == 1 && (num30 < num29 || num29 == 0))
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
						int num43 = DDoorY[num25];
						int num44 = num43;
						while (!Main.tile.At(num42, num43).Active)
						{
							Main.tile.At(num42, num43).SetActive(false);
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
							Main.tile.At(num42, num45).SetActive(true);
							Main.tile.At(num42, num45).SetType((byte)tileType);
						}
						WorldModify.PlaceTile(num42, num43, 10, true, false, -1, 0);
						num42--;
						int num46 = num43 - 3;
						while (!Main.tile.At(num42, num46).Active)
						{
							num46--;
						}
						if (num43 - num46 < num43 - num44 + 5 && Main.tileDungeon[(int)Main.tile.At(num42, num46).Type])
						{
							for (int num47 = num43 - 4 - WorldModify.genRand.Next(3); num47 > num46; num47--)
							{
								Main.tile.At(num42, num47).SetActive(true);
								Main.tile.At(num42, num47).SetType((byte)tileType);
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
							for (int num48 = num43 - 4 - WorldModify.genRand.Next(3); num48 > num46; num48--)
							{
								Main.tile.At(num42, num48).SetActive(true);
								Main.tile.At(num42, num48).SetType((byte)tileType);
							}
						}
						num43++;
						num42--;
						Main.tile.At(num42 - 1, num43).SetActive(true);
						Main.tile.At(num42 - 1, num43).SetType((byte)tileType);
						Main.tile.At(num42 + 1, num43).SetActive(true);
						Main.tile.At(num42 + 1, num43).SetType((byte)tileType);
					}
				}

				prog.Value = 85;

				for (int num49 = 0; num49 < numDPlats; num49++)
				{
					int num50 = DPlatX[num49];
					int num51 = DPlatY[num49];
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
							Main.tile.At(num64, num65).SetActive(true);
							Main.tile.At(num64, num65).SetType(19);
							num64--;
						}
						while (!Main.tile.At(num66, num65).Active)
						{
							Main.tile.At(num66, num65).SetActive(true);
							Main.tile.At(num66, num65).SetType(19);
							num66++;
						}
					}
				}

				prog.Value = 90;

				num12 = 0;
				num13 = 1000;
				num14 = 0;
				while (num14 < Main.maxTilesX / 20)
				{
					num12++;
					int num67 = WorldModify.genRand.Next(dMinX, dMaxX);
					int num68 = WorldModify.genRand.Next(dMinY, dMaxY);
					bool flag5 = true;
					if ((int)Main.tile.At(num67, num68).Wall == wallType && !Main.tile.At(num67, num68).Active)
					{
						int num69 = 1;
						if (WorldModify.genRand.Next(2) == 0)
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
							else if (Main.tile.At(num67, num68).Active && !Main.tileDungeon[(int)Main.tile.At(num67, num68).Type])
							{
								flag5 = false;
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
								while (num72 > dMinX && num72 < dMaxX && !Main.tile.At(num72, num68).Active && !Main.tile.At(num72, num68 - 1).Active && !Main.tile.At(num72, num68 + 1).Active)
								{
									num72 += num69;
								}
								num72 = Math.Abs(num67 - num72);
								bool flag6 = false;
								if (WorldModify.genRand.Next(2) == 0)
								{
									flag6 = true;
								}
								if (num72 > 5)
								{
									for (int num74 = WorldModify.genRand.Next(1, 4); num74 > 0; num74--)
									{
										Main.tile.At(num67, num68).SetActive(true);
										Main.tile.At(num67, num68).SetType(19);
										if (flag6)
										{
											WorldModify.PlaceTile(num67, num68 - 1, 50, true, false, -1, 0);
											if (WorldModify.genRand.Next(50) == 0 && Main.tile.At(num67, num68 - 1).Type == 50)
											{
												Main.tile.At(num67, num68 - 1).SetFrameX(90);
											}
										}
										num67 += num69;
									}
									num12 = 0;
									num14++;
									if (!flag6 && WorldModify.genRand.Next(2) == 0)
									{
										num67 = num73;
										num68--;
										int num75 = WorldModify.genRand.Next(2);
										if (num75 == 0)
										{
											num75 = 13;
										}
										else if (num75 == 1)
										{
											num75 = 49;
										}
										WorldModify.PlaceTile(num67, num68, num75, true, false, -1, 0);
										if (Main.tile.At(num67, num68).Type == 13)
										{
											if (WorldModify.genRand.Next(2) == 0)
											{
												Main.tile.At(num67, num68).SetFrameX(18);
											}
											else
											{
												Main.tile.At(num67, num68).SetFrameX(36);
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

				prog.Value = 95;

				for (int num76 = 0; num76 < numDRooms; num76++)
				{
					int num77 = 0;
					while (num77 < 1000)
					{
						int num78 = (int)((double)dRoomSize[num76] * 0.4);
						int i2 = dRoomX[num76] + WorldModify.genRand.Next(-num78, num78 + 1);
						int j2 = dRoomY[num76] + WorldModify.genRand.Next(-num78, num78 + 1);
						int num79 = 0;
						int num80 = num76;
						switch (num80)
						{
							case 0:
								num79 = 113;
								break;
							case 1:
								num79 = 155;
								break;
							case 2:
								num79 = 156;
								break;
							case 3:
								num79 = 157;
								break;
							case 4:
								num79 = 163;
								break;
							case 5:
								num79 = 164;
								break;
							default:
								break;
						}
						if (num79 == 0 && WorldModify.genRand.Next(2) == 0)
						{
							num77 = 1000;
						}
						else
						{
							if (AddBuriedChest(i2, j2, num79, false))
							{
								num77 += 1000;
							}
							num77++;
						}
					}
				}
				dMinX -= 25;
				dMaxX += 25;
				dMinY -= 25;
				dMaxY += 25;
				if (dMinX < 0)
				{
					dMinX = 0;
				}
				if (dMaxX > Main.maxTilesX)
				{
					dMaxX = Main.maxTilesX;
				}
				if (dMinY < 0)
				{
					dMinY = 0;
				}
				if (dMaxY > Main.maxTilesY)
				{
					dMaxY = Main.maxTilesY;
				}
				num12 = 0;
				num13 = 1000;
				num14 = 0;
				while (num14 < Main.maxTilesX / 40)
				{
					num12++;
					int num81 = WorldModify.genRand.Next(dMinX, dMaxX);
					int num82 = WorldModify.genRand.Next(dMinY, dMaxY);
					if ((int)Main.tile.At(num81, num82).Wall == wallType)
					{
						int num83 = num82;
						while (num83 > dMinY)
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
								WorldModify.Place1x2Top(num81, num83, 42);
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
			}
		} // end MakeDungeon

		public static void DungeonStairs(int i, int j, int tileType, int wallType)
		{
			Vector2 value = default(Vector2);
			double num = (double)WorldModify.genRand.Next(5, 9);
			int num2 = 1;
			Vector2 value2;
			value2.X = (float)i;
			value2.Y = (float)j;
			int k = WorldModify.genRand.Next(10, 30);
			if (i > dEntranceX)
			{
				num2 = -1;
			}
			else
			{
				num2 = 1;
			}
			value.Y = -1f;
			value.X = (float)num2;
			if (WorldModify.genRand.Next(3) == 0)
			{
				value.X *= 0.5f;
			}
			else if (WorldModify.genRand.Next(3) == 0)
			{
				value.Y *= 2f;
			}
			while (k > 0)
			{
				k--;
				int num3 = (int)((double)value2.X - num - 4.0 - (double)WorldModify.genRand.Next(6));
				int num4 = (int)((double)value2.X + num + 4.0 + (double)WorldModify.genRand.Next(6));
				int num5 = (int)((double)value2.Y - num - 4.0);
				int num6 = (int)((double)value2.Y + num + 4.0 + (double)WorldModify.genRand.Next(6));
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
				int num8 = (int)(value2.X + (float)dxStrength1 * 0.6f * (float)num7 + (float)dxStrength2 * (float)num7);
				int num9 = (int)(dyStrength2 * 0.5);
				if ((double)value2.Y < Main.worldSurface - 5.0 && Main.tile.At(num8, (int)((double)value2.Y - num - 6.0 + (double)num9)).Wall == 0 && Main.tile.At(num8, (int)((double)value2.Y - num - 7.0 + (double)num9)).Wall == 0 && Main.tile.At(num8, (int)((double)value2.Y - num - 8.0 + (double)num9)).Wall == 0)
				{
					dSurface = true;
					TileRunner(num8, (int)((double)value2.Y - num - 6.0 + (double)num9), (double)WorldModify.genRand.Next(25, 35), WorldModify.genRand.Next(10, 20), -1, false, 0f, -1f, false, true);
				}
				for (int l = num3; l < num4; l++)
				{
					for (int m = num5; m < num6; m++)
					{
						Main.tile.At(l, m).SetLiquid(0);
						if ((int)Main.tile.At(l, m).Wall != wallType)
						{
							Main.tile.At(l, m).SetWall(0);
							Main.tile.At(l, m).SetActive(true);
							Main.tile.At(l, m).SetType((byte)tileType);
						}
					}
				}
				for (int n = num3 + 1; n < num4 - 1; n++)
				{
					for (int num10 = num5 + 1; num10 < num6 - 1; num10++)
					{
						WorldModify.PlaceWall(n, num10, wallType, true);
					}
				}
				int num11 = 0;
				if (WorldModify.genRand.Next((int)num) == 0)
				{
					num11 = WorldModify.genRand.Next(1, 3);
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
						Main.tile.At(num12, num13).SetActive(false);
						WorldModify.PlaceWall(num12, num13, wallType, true);
					}
				}
				if (dSurface)
				{
					k = 0;
				}
				value2 += value;
			}
			dungeonX = (int)value2.X;
			dungeonY = (int)value2.Y;
		}

		public static void DungeonHalls(int i, int j, int tileType, int wallType, bool forceX = false)
		{
			Vector2 value = default(Vector2);
			double num = (double)WorldModify.genRand.Next(4, 6);
			Vector2 vector = default(Vector2);
			Vector2 value2 = default(Vector2);
			int num2 = 1;
			Vector2 value3;
			value3.X = (float)i;
			value3.Y = (float)j;
			int k = WorldModify.genRand.Next(35, 80);
			if (forceX)
			{
				k += 20;
				lastDungeonHall = default(Vector2);
			}
			else if (WorldModify.genRand.Next(5) == 0)
			{
				num *= 2.0;
				k /= 2;
			}
			bool flag = false;
			while (!flag)
			{
				if (WorldModify.genRand.Next(2) == 0)
				{
					num2 = -1;
				}
				else
				{
					num2 = 1;
				}
				bool flag2 = false;
				if (WorldModify.genRand.Next(2) == 0)
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
					if (WorldModify.genRand.Next(3) == 0)
					{
						if (WorldModify.genRand.Next(2) == 0)
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
					if (WorldModify.genRand.Next(2) == 0)
					{
						if (WorldModify.genRand.Next(2) == 0)
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
				if (lastDungeonHall != value2)
				{
					flag = true;
				}
			}
			if (!forceX)
			{
				if (value3.X > (float)(WorldModify.lastMaxTilesX - 200))
				{
					num2 = -1;
					vector.Y = 0f;
					vector.X = (float)num2;
					value.Y = 0f;
					value.X = (float)num2;
					if (WorldModify.genRand.Next(3) == 0)
					{
						if (WorldModify.genRand.Next(2) == 0)
						{
							value.Y = -0.2f;
						}
						else
						{
							value.Y = 0.2f;
						}
					}
				}
				else if (value3.X < 200f)
				{
					num2 = 1;
					vector.Y = 0f;
					vector.X = (float)num2;
					value.Y = 0f;
					value.X = (float)num2;
					if (WorldModify.genRand.Next(3) == 0)
					{
						if (WorldModify.genRand.Next(2) == 0)
						{
							value.Y = -0.2f;
						}
						else
						{
							value.Y = 0.2f;
						}
					}
				}
				else if (value3.Y > (float)(WorldModify.lastMaxTilesY - 300))
				{
					num2 = -1;
					num += 1.0;
					value.Y = (float)num2;
					value.X = 0f;
					vector.X = 0f;
					vector.Y = (float)num2;
					if (WorldModify.genRand.Next(2) == 0)
					{
						if (WorldModify.genRand.Next(2) == 0)
						{
							value.X = 0.3f;
						}
						else
						{
							value.X = -0.3f;
						}
					}
				}
				else if ((double)value3.Y < Main.rockLayer)
				{
					num2 = 1;
					num += 1.0;
					value.Y = (float)num2;
					value.X = 0f;
					vector.X = 0f;
					vector.Y = (float)num2;
					if (WorldModify.genRand.Next(2) == 0)
					{
						if (WorldModify.genRand.Next(2) == 0)
						{
							value.X = 0.3f;
						}
						else
						{
							value.X = -0.3f;
						}
					}
				}
				else if (value3.X < (float)(Main.maxTilesX / 2) && (double)value3.X > (double)Main.maxTilesX * 0.25)
				{
					num2 = -1;
					vector.Y = 0f;
					vector.X = (float)num2;
					value.Y = 0f;
					value.X = (float)num2;
					if (WorldModify.genRand.Next(3) == 0)
					{
						if (WorldModify.genRand.Next(2) == 0)
						{
							value.Y = -0.2f;
						}
						else
						{
							value.Y = 0.2f;
						}
					}
				}
				else if (value3.X > (float)(Main.maxTilesX / 2) && (double)value3.X < (double)Main.maxTilesX * 0.75)
				{
					num2 = 1;
					vector.Y = 0f;
					vector.X = (float)num2;
					value.Y = 0f;
					value.X = (float)num2;
					if (WorldModify.genRand.Next(3) == 0)
					{
						if (WorldModify.genRand.Next(2) == 0)
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
			if (vector.Y == 0f)
			{
				DDoorX[numDDoors] = (int)value3.X;
				DDoorY[numDDoors] = (int)value3.Y;
				DDoorPos[numDDoors] = 0;
				numDDoors++;
			}
			else
			{
				DPlatX[numDPlats] = (int)value3.X;
				DPlatY[numDPlats] = (int)value3.Y;
				numDPlats++;
			}
			lastDungeonHall = vector;
			while (k > 0)
			{
				if (vector.X > 0f && value3.X > (float)(Main.maxTilesX - 100))
				{
					k = 0;
				}
				else if (vector.X < 0f && value3.X < 100f)
				{
					k = 0;
				}
				else if (vector.Y > 0f && value3.Y > (float)(Main.maxTilesY - 100))
				{
					k = 0;
				}
				else if (vector.Y < 0f && (double)value3.Y < Main.rockLayer + 50.0)
				{
					k = 0;
				}
				k--;
				int num3 = (int)((double)value3.X - num - 4.0 - (double)WorldModify.genRand.Next(6));
				int num4 = (int)((double)value3.X + num + 4.0 + (double)WorldModify.genRand.Next(6));
				int num5 = (int)((double)value3.Y - num - 4.0 - (double)WorldModify.genRand.Next(6));
				int num6 = (int)((double)value3.Y + num + 4.0 + (double)WorldModify.genRand.Next(6));
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
						Main.tile.At(l, m).SetLiquid(0);
						if (Main.tile.At(l, m).Wall == 0)
						{
							Main.tile.At(l, m).SetActive(true);
							Main.tile.At(l, m).SetType((byte)tileType);
						}
					}
				}
				for (int n = num3 + 1; n < num4 - 1; n++)
				{
					for (int num7 = num5 + 1; num7 < num6 - 1; num7++)
					{
						WorldModify.PlaceWall(n, num7, wallType, true);
					}
				}
				int num8 = 0;
				if (value.Y == 0f && WorldModify.genRand.Next((int)num + 1) == 0)
				{
					num8 = WorldModify.genRand.Next(1, 3);
				}
				else if (value.X == 0f && WorldModify.genRand.Next((int)num - 1) == 0)
				{
					num8 = WorldModify.genRand.Next(1, 3);
				}
				else if (WorldModify.genRand.Next((int)num * 3) == 0)
				{
					num8 = WorldModify.genRand.Next(1, 3);
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
						Main.tile.At(num9, num10).SetActive(false);
						Main.tile.At(num9, num10).SetWall((byte)wallType);
					}
				}
				value3 += value;
			}
			dungeonX = (int)value3.X;
			dungeonY = (int)value3.Y;
			if (vector.Y == 0f)
			{
				DDoorX[numDDoors] = (int)value3.X;
				DDoorY[numDDoors] = (int)value3.Y;
				DDoorPos[numDDoors] = 0;
				numDDoors++;
				return;
			}
			DPlatX[numDPlats] = (int)value3.X;
			DPlatY[numDPlats] = (int)value3.Y;
			numDPlats++;
		}

		public static void DungeonRoom(int i, int j, int tileType, int wallType)
		{
			double num = (double)WorldModify.genRand.Next(15, 30);
			Vector2 value;
			value.X = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			value.Y = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			Vector2 value2;
			value2.X = (float)i;
			value2.Y = (float)j - (float)num / 2f;
			int k = WorldModify.genRand.Next(10, 20);
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
						Main.tile.At(l, m).SetLiquid(0);
						if (Main.tile.At(l, m).Wall == 0)
						{
							Main.tile.At(l, m).SetActive(true);
							Main.tile.At(l, m).SetType((byte)tileType);
						}
					}
				}
				for (int n = num6 + 1; n < num7 - 1; n++)
				{
					for (int num10 = num8 + 1; num10 < num9 - 1; num10++)
					{
						WorldModify.PlaceWall(n, num10, wallType, true);
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
						Main.tile.At(num11, num12).SetActive(false);
						Main.tile.At(num11, num12).SetWall((byte)wallType);
					}
				}
				value2 += value;
				value.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
				value.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
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
			dRoomX[numDRooms] = (int)value2.X;
			dRoomY[numDRooms] = (int)value2.Y;
			dRoomSize[numDRooms] = (int)num;
			dRoomL[numDRooms] = (int)num2;
			dRoomR[numDRooms] = (int)num3;
			dRoomT[numDRooms] = (int)num4;
			dRoomB[numDRooms] = (int)num5;
			dRoomTreasure[numDRooms] = false;
			numDRooms++;
		}

		public static void DungeonEnt(int i, int j, int tileType, int wallType)
		{
			double num = dxStrength1;
			double num2 = dyStrength1;
			Vector2 vector;
			vector.X = (float)i;
			vector.Y = (float)j - (float)num2 / 2f;
			dMinY = (int)vector.Y;
			int num3 = 1;
			if (i > Main.maxTilesX / 2)
			{
				num3 = -1;
			}
			int num4 = (int)((double)vector.X - num * 0.60000002384185791 - (double)WorldModify.genRand.Next(2, 5));
			int num5 = (int)((double)vector.X + num * 0.60000002384185791 + (double)WorldModify.genRand.Next(2, 5));
			int num6 = (int)((double)vector.Y - num2 * 0.60000002384185791 - (double)WorldModify.genRand.Next(2, 5));
			int num7 = (int)((double)vector.Y + num2 * 0.60000002384185791 + (double)WorldModify.genRand.Next(8, 16));
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
					Main.tile.At(k, l).SetLiquid(0);
					if ((int)Main.tile.At(k, l).Wall != wallType)
					{
						Main.tile.At(k, l).SetWall(0);
						if (k > num4 + 1 && k < num5 - 2 && l > num6 + 1 && l < num7 - 2)
						{
							WorldModify.PlaceWall(k, l, wallType, true);
						}
						Main.tile.At(k, l).SetActive(true);
						Main.tile.At(k, l).SetType((byte)tileType);
					}
				}
			}
			int num8 = num4;
			int num9 = num4 + 5 + WorldModify.genRand.Next(4);
			int num10 = num6 - 3 - WorldModify.genRand.Next(3);
			int num11 = num6;
			for (int m = num8; m < num9; m++)
			{
				for (int n = num10; n < num11; n++)
				{
					if ((int)Main.tile.At(m, n).Wall != wallType)
					{
						Main.tile.At(m, n).SetActive(true);
						Main.tile.At(m, n).SetType((byte)tileType);
					}
				}
			}
			num8 = num5 - 5 - WorldModify.genRand.Next(4);
			num9 = num5;
			num10 = num6 - 3 - WorldModify.genRand.Next(3);
			num11 = num6;
			for (int num12 = num8; num12 < num9; num12++)
			{
				for (int num13 = num10; num13 < num11; num13++)
				{
					if ((int)Main.tile.At(num12, num13).Wall != wallType)
					{
						Main.tile.At(num12, num13).SetActive(true);
						Main.tile.At(num12, num13).SetType((byte)tileType);
					}
				}
			}
			int num14 = 1 + WorldModify.genRand.Next(2);
			int num15 = 2 + WorldModify.genRand.Next(4);
			int num16 = 0;
			for (int num17 = num4; num17 < num5; num17++)
			{
				for (int num18 = num6 - num14; num18 < num6; num18++)
				{
					if ((int)Main.tile.At(num17, num18).Wall != wallType)
					{
						Main.tile.At(num17, num18).SetActive(true);
						Main.tile.At(num17, num18).SetType((byte)tileType);
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
					WorldModify.PlaceWall(num19, num20, 2, true);
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
					WorldModify.PlaceWall(num21, num22, wallType, true);
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
					Main.tile.At(num23, num24).SetWall((byte)wallType);
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
					Main.tile.At(num25, num26).SetActive(false);
					Main.tile.At(num25, num26).SetWall((byte)wallType);
				}
			}
			DPlatX[numDPlats] = (int)vector.X;
			DPlatY[numDPlats] = num7;
			numDPlats++;
			vector.X += (float)num * 0.6f * (float)num3;
			vector.Y += (float)num2 * 0.5f;
			num = dxStrength2;
			num2 = dyStrength2;
			vector.X += (float)num * 0.55f * (float)num3;
			vector.Y -= (float)num2 * 0.5f;
			num4 = (int)((double)vector.X - num * 0.60000002384185791 - (double)WorldModify.genRand.Next(1, 3));
			num5 = (int)((double)vector.X + num * 0.60000002384185791 + (double)WorldModify.genRand.Next(1, 3));
			num6 = (int)((double)vector.Y - num2 * 0.60000002384185791 - (double)WorldModify.genRand.Next(1, 3));
			num7 = (int)((double)vector.Y + num2 * 0.60000002384185791 + (double)WorldModify.genRand.Next(6, 16));
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
						else if ((double)num27 > (double)vector.X + num * 0.5 - 1.0)
						{
							flag = false;
						}
						if (flag)
						{
							Main.tile.At(num27, num28).SetWall(0);
							Main.tile.At(num27, num28).SetActive(true);
							Main.tile.At(num27, num28).SetType((byte)tileType);
						}
					}
				}
			}
			for (int num29 = num4; num29 < num5; num29++)
			{
				for (int num30 = num7; num30 < num7 + 100; num30++)
				{
					WorldModify.PlaceWall(num29, num30, 2, true);
				}
			}
			num4 = (int)((double)vector.X - num * 0.5);
			num5 = (int)((double)vector.X + num * 0.5);
			num8 = num4;
			if (num3 < 0)
			{
				num8++;
			}
			num9 = num8 + 5 + WorldModify.genRand.Next(4);
			num10 = num6 - 3 - WorldModify.genRand.Next(3);
			num11 = num6;
			for (int num31 = num8; num31 < num9; num31++)
			{
				for (int num32 = num10; num32 < num11; num32++)
				{
					if ((int)Main.tile.At(num31, num32).Wall != wallType)
					{
						Main.tile.At(num31, num32).SetActive(true);
						Main.tile.At(num31, num32).SetType((byte)tileType);
					}
				}
			}
			num8 = num5 - 5 - WorldModify.genRand.Next(4);
			num9 = num5;
			num10 = num6 - 3 - WorldModify.genRand.Next(3);
			num11 = num6;
			for (int num33 = num8; num33 < num9; num33++)
			{
				for (int num34 = num10; num34 < num11; num34++)
				{
					if ((int)Main.tile.At(num33, num34).Wall != wallType)
					{
						Main.tile.At(num33, num34).SetActive(true);
						Main.tile.At(num33, num34).SetType((byte)tileType);
					}
				}
			}
			num14 = 1 + WorldModify.genRand.Next(2);
			num15 = 2 + WorldModify.genRand.Next(4);
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
						Main.tile.At(num35, num36).SetActive(true);
						Main.tile.At(num35, num36).SetType((byte)tileType);
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
					Main.tile.At(num37, num38).SetWall(0);
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
					Main.tile.At(num39, num40).SetActive(false);
					Main.tile.At(num39, num40).SetWall(0);
				}
			}
			for (int num41 = num4; num41 < num5; num41++)
			{
				if (!Main.tile.At(num41, num7).Active)
				{
					Main.tile.At(num41, num7).SetActive(true);
					Main.tile.At(num41, num7).SetType(19);
				}
			}
			Main.dungeonX = (int)vector.X;
			Main.dungeonY = num7;
			int num42 = NPC.NewNPC(dungeonX * 16 + 8, dungeonY * 16, 37, 0);
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
						Main.tile.At(num44, num45).SetActive(true);
						Main.tile.At(num44, num45).SetType((byte)tileType);
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
						Main.tile.At(num47, num48).SetActive(true);
						Main.tile.At(num47, num48).SetType((byte)tileType);
					}
				}
			}
			num14 = 1 + WorldModify.genRand.Next(2);
			num15 = 2 + WorldModify.genRand.Next(4);
			num16 = 0;
			num4 = (int)((double)vector.X - num * 0.5);
			num5 = (int)((double)vector.X + num * 0.5);
			num4 += 2;
			num5 -= 2;
			for (int num49 = num4; num49 < num5; num49++)
			{
				for (int num50 = num6; num50 < num7; num50++)
				{
					WorldModify.PlaceWall(num49, num50, wallType, true);
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
					Main.tile.At(num51, num52).SetActive(false);
				}
			}
			if (num3 < 0)
			{
				vector.X -= 1f;
			}
			WorldModify.PlaceTile((int)vector.X, (int)vector.Y + 1, 10, false, false, -1, 0);
		}

		public static void ChasmRunnerSideways(int i, int j, int direction, int steps)
		{
			float num = (float)steps;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldModify.genRand.Next(10, 21) * 0.1f * (float)direction;
			value2.Y = (float)WorldModify.genRand.Next(-10, 10) * 0.01f;
			double num2 = (double)(WorldModify.genRand.Next(5) + 7);
			while (num2 > 0.0)
			{
				if (num > 0f)
				{
					num2 += (double)WorldModify.genRand.Next(3);
					num2 -= (double)WorldModify.genRand.Next(3);
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
					num2 -= (double)WorldModify.genRand.Next(4);
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
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num2 * 0.5 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015) && Main.tile.At(k, l).Type != 31 && Main.tile.At(k, l).Type != 22)
						{
							Main.tile.At(k, l).SetActive(false);
						}
					}
				}
				value += value2;
				value2.Y += (float)WorldModify.genRand.Next(-10, 10) * 0.1f;
				if (value.Y < (float)(j - 20))
				{
					value2.Y += (float)WorldModify.genRand.Next(20) * 0.01f;
				}
				if (value.Y > (float)(j + 20))
				{
					value2.Y -= (float)WorldModify.genRand.Next(20) * 0.01f;
				}
				if ((double)value2.Y < -0.5)
				{
					value2.Y = -0.5f;
				}
				if ((double)value2.Y > 0.5)
				{
					value2.Y = 0.5f;
				}
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.01f;
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
				else if (direction == 1)
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
						if ((double)(Math.Abs((float)m - value.X) + Math.Abs((float)n - value.Y)) < num2 * 1.1 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015) && Main.tile.At(m, n).Wall != 3)
						{
							if (Main.tile.At(m, n).Type != 25 && n > j + WorldModify.genRand.Next(3, 20))
							{
								Main.tile.At(m, n).SetActive(true);
							}
							Main.tile.At(m, n).SetActive(true);
							if (Main.tile.At(m, n).Type != 31 && Main.tile.At(m, n).Type != 22)
							{
								Main.tile.At(m, n).SetType(25);
							}
							if (Main.tile.At(m, n).Wall == 2)
							{
								Main.tile.At(m, n).SetWall(0);
							}
						}
					}
				}
				for (int num7 = num3; num7 < num4; num7++)
				{
					for (int num8 = num5; num8 < num6; num8++)
					{
						if ((double)(Math.Abs((float)num7 - value.X) + Math.Abs((float)num8 - value.Y)) < num2 * 1.1 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015) && Main.tile.At(num7, num8).Wall != 3)
						{
							if (Main.tile.At(num7, num8).Type != 31 && Main.tile.At(num7, num8).Type != 22)
							{
								Main.tile.At(num7, num8).SetType(25);
							}
							Main.tile.At(num7, num8).SetActive(true);
							WorldModify.PlaceWall(num7, num8, 3, true);
						}
					}
				}
			}
			if (WorldModify.genRand.Next(3) == 0)
			{
				int num9 = (int)value.X;
				int num10 = (int)value.Y;
				while (!Main.tile.At(num9, num10).Active)
				{
					num10++;
				}
				TileRunner(num9, num10, (double)WorldModify.genRand.Next(2, 6), WorldModify.genRand.Next(3, 7), 22, false, 0f, 0f, false, true);
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
			value2.X = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldModify.genRand.Next(11) * 0.2f + 0.5f;
			int num2 = 5;
			double num3 = (double)(WorldModify.genRand.Next(5) + 7);
			while (num3 > 0.0)
			{
				if (num > 0f)
				{
					num3 += (double)WorldModify.genRand.Next(3);
					num3 -= (double)WorldModify.genRand.Next(3);
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
				else if ((double)value.Y > Main.worldSurface + 45.0)
				{
					num3 -= (double)WorldModify.genRand.Next(4);
				}
				if ((double)value.Y > Main.rockLayer && num > 0f)
				{
					num = 0f;
				}
				num -= 1f;
				if (!flag && (double)value.Y > Main.worldSurface + 20.0)
				{
					flag = true;
					ChasmRunnerSideways((int)value.X, (int)value.Y, -1, WorldModify.genRand.Next(20, 40));
					ChasmRunnerSideways((int)value.X, (int)value.Y, 1, WorldModify.genRand.Next(20, 40));
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
							if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num3 * 0.5 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015) && Main.tile.At(k, l).Type != 31 && Main.tile.At(k, l).Type != 22)
							{
								Main.tile.At(k, l).SetActive(false);
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
						AddShadowOrb((int)value.X, (int)value.Y);
					}
					else if (!flag3)
					{
						flag3 = false;
						bool flag4 = false;
						int num8 = 0;
						while (!flag4)
						{
							int num9 = WorldModify.genRand.Next((int)value.X - 25, (int)value.X + 25);
							int num10 = WorldModify.genRand.Next((int)value.Y - 50, (int)value.Y);
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
								WorldModify.Place3x2(num9, num10, 26);
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
				value += value2;
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.01f;
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
						if ((double)(Math.Abs((float)m - value.X) + Math.Abs((float)n - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015))
						{
							if (Main.tile.At(m, n).Type != 25 && n > j + WorldModify.genRand.Next(3, 20))
							{
								Main.tile.At(m, n).SetActive(true);
							}
							if (steps <= num2)
							{
								Main.tile.At(m, n).SetActive(true);
							}
							if (Main.tile.At(m, n).Type != 31)
							{
								Main.tile.At(m, n).SetType(25);
							}
							if (Main.tile.At(m, n).Wall == 2)
							{
								Main.tile.At(m, n).SetWall(0);
							}
						}
					}
				}
				for (int num11 = num4; num11 < num5; num11++)
				{
					for (int num12 = num6; num12 < num7; num12++)
					{
						if ((double)(Math.Abs((float)num11 - value.X) + Math.Abs((float)num12 - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015))
						{
							if (Main.tile.At(num11, num12).Type != 31)
							{
								Main.tile.At(num11, num12).SetType(25);
							}
							if (steps <= num2)
							{
								Main.tile.At(num11, num12).SetActive(true);
							}
							if (num12 > j + WorldModify.genRand.Next(3, 20))
							{
								WorldModify.PlaceWall(num11, num12, 3, true);
							}
						}
					}
				}
			}
		}

		public static void JungleRunner(int i, int j)
		{
			double num = (double)WorldModify.genRand.Next(5, 11);
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldModify.genRand.Next(10, 20) * 0.1f;
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
				JungleX = (int)value.X;
				num += (double)((float)WorldModify.genRand.Next(-20, 21) * 0.1f);
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
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num * 0.5 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015))
						{
							WorldModify.KillTile(k, l, false, false, false);
						}
					}
				}
				num2++;
				if (num2 > 10 && WorldModify.genRand.Next(50) < num2)
				{
					num2 = 0;
					int num9 = -2;
					if (WorldModify.genRand.Next(2) == 0)
					{
						num9 = 2;
					}
					TileRunner((int)value.X, (int)value.Y, (double)WorldModify.genRand.Next(3, 20), WorldModify.genRand.Next(10, 100), -1, false, (float)num9, 0f, false, true);
				}
				value += value2;
				value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.01f;
				if (value2.Y > 0f)
				{
					value2.Y = 0f;
				}
				if (value2.Y < -2f)
				{
					value2.Y = -2f;
				}
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
				if (value.X < (float)(i - 200))
				{
					value2.X += (float)WorldModify.genRand.Next(5, 21) * 0.1f;
				}
				if (value.X > (float)(i + 200))
				{
					value2.X -= (float)WorldModify.genRand.Next(5, 21) * 0.1f;
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
			value2.X = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
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
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < strength * 0.5 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015))
						{
							if (type < 0)
							{
								if (type == -2 && Main.tile.At(k, l).Active && (l < WorldModify.waterLine || l > WorldModify.lavaLine))
								{
									Main.tile.At(k, l).SetLiquid(255);
									if (l > WorldModify.lavaLine)
									{
										Main.tile.At(k, l).SetLava(true);
									}
								}
								Main.tile.At(k, l).SetActive(false);
							}
							else
							{
								if ((overRide || !Main.tile.At(k, l).Active) && (type != 40 || Main.tile.At(k, l).Type != 53) && (!Main.tileStone[type] || Main.tile.At(k, l).Type == 1) && Main.tile.At(k, l).Type != 45 && (Main.tile.At(k, l).Type != 1 || type != 59 || (double)l >= Main.worldSurface + (double)WorldModify.genRand.Next(-50, 50)))
								{
									if (Main.tile.At(k, l).Type != 53 || (double)l >= Main.worldSurface)
									{
										Main.tile.At(k, l).SetType((byte)type);
									}
									else
									{
										if (type == 59)
										{
											Main.tile.At(k, l).SetType((byte)type);
										}
									}
								}
								if (addTile)
								{
									Main.tile.At(k, l).SetActive(true);
									Main.tile.At(k, l).SetLiquid(0);
									Main.tile.At(k, l).SetLava(false);
								}
								if (noYChange && (double)l < Main.worldSurface && type != 59)
								{
									Main.tile.At(k, l).SetWall(2);
								}
								if (type == 59 && l > WorldModify.waterLine && Main.tile.At(k, l).Liquid > 0)
								{
									Main.tile.At(k, l).SetLava(false);
									Main.tile.At(k, l).SetLiquid(0);
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
					value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
					value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
					if (num > 100.0)
					{
						value += value2;
						num2 -= 1f;
						value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
						value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
						if (num > 150.0)
						{
							value += value2;
							num2 -= 1f;
							value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
							value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
							if (num > 200.0)
							{
								value += value2;
								num2 -= 1f;
								value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
								value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
								if (num > 250.0)
								{
									value += value2;
									num2 -= 1f;
									value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
									value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
									if (num > 300.0)
									{
										value += value2;
										num2 -= 1f;
										value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
										value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
										if (num > 400.0)
										{
											value += value2;
											num2 -= 1f;
											value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
											value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
											if (num > 500.0)
											{
												value += value2;
												num2 -= 1f;
												value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
												value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
												if (num > 600.0)
												{
													value += value2;
													num2 -= 1f;
													value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
													value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
													if (num > 700.0)
													{
														value += value2;
														num2 -= 1f;
														value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
														value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
														if (num > 800.0)
														{
															value += value2;
															num2 -= 1f;
															value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
															value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
															if (num > 900.0)
															{
																value += value2;
																num2 -= 1f;
																value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
																value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
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
					value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
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
			double num = (double)WorldModify.genRand.Next(80, 120);
			float num2 = (float)WorldModify.genRand.Next(20, 25);
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldModify.genRand.Next(-20, 21) * 0.2f;
			while (value2.X > -2f && value2.X < 2f)
			{
				value2.X = (float)WorldModify.genRand.Next(-20, 21) * 0.2f;
			}
			value2.Y = (float)WorldModify.genRand.Next(-20, -10) * 0.02f;
			while (num > 0.0 && num2 > 0f)
			{
				num -= (double)WorldModify.genRand.Next(4);
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
				double num7 = num * (double)WorldModify.genRand.Next(80, 120) * 0.01;
				float num8 = value.Y + 1f;
				for (int k = num3; k < num4; k++)
				{
					if (WorldModify.genRand.Next(2) == 0)
					{
						num8 += (float)WorldModify.genRand.Next(-1, 2);
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
								Main.tile.At(k, l).SetActive(true);
								if (Main.tile.At(k, l).Type == 59)
								{
									Main.tile.At(k, l).SetType(0);
								}
							}
						}
					}
				}
				TileRunner(WorldModify.genRand.Next(num3 + 10, num4 - 10), (int)((double)value.Y + num7 * 0.1 + 5.0), (double)WorldModify.genRand.Next(5, 10), WorldModify.genRand.Next(10, 15), 0, true, 0f, 2f, true, true);
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
				num7 = num * (double)WorldModify.genRand.Next(80, 120) * 0.01;
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
								Main.tile.At(m, n).SetWall(2);
							}
						}
					}
				}
				value += value2;
				value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
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
			byte type = (byte)WorldModify.genRand.Next(45, 48);
			byte wall = (byte)WorldModify.genRand.Next(10, 13);
			Vector2 vector = new Vector2((float)i, (float)j);
			int num = 1;
			if (WorldModify.genRand.Next(2) == 0)
			{
				num = -1;
			}
			int num2 = WorldModify.genRand.Next(7, 12);
			int num3 = WorldModify.genRand.Next(5, 7);
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
			int num7 = (int)(vector.Y + 2f + (float)WorldModify.genRand.Next(3, 5));
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
					Main.tile.At(l, m).SetActive(true);
					Main.tile.At(l, m).SetType(type);
					Main.tile.At(l, m).SetWall(0);
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
						Main.tile.At(n, num8).SetActive(false);
						Main.tile.At(n, num8).SetWall(wall);
					}
				}
			}
			int num9 = i + (num2 + 1) * num;
			int num10 = (int)vector.Y;
			for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
			{
				Main.tile.At(num11, num10).SetActive(false);
				Main.tile.At(num11, num10 - 1).SetActive(false);
				Main.tile.At(num11, num10 - 2).SetActive(false);
			}
			WorldModify.PlaceTile(num9, num10, 10, true, false, -1, 0);
			int contain = 0;
			int num12 = houseCount;
			if (num12 > 2)
			{
				num12 = WorldModify.genRand.Next(3);
			}
			if (num12 == 0)
			{
				contain = 159;
			}
			else if (num12 == 1)
			{
				contain = 65;
			}
			else if (num12 == 2)
			{
				contain = 158;
			}
			AddBuriedChest(i, num10 - 3, contain, false);
			houseCount++;
		}

		public static void Mountinater(int i, int j)
		{
			double num = (double)WorldModify.genRand.Next(80, 120);
			float num2 = (float)WorldModify.genRand.Next(40, 55);
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j + num2 / 2f;
			Vector2 value2;
			value2.X = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldModify.genRand.Next(-20, -10) * 0.1f;
			while (num > 0.0 && num2 > 0f)
			{
				num -= (double)WorldModify.genRand.Next(4);
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
				double num7 = num * (double)WorldModify.genRand.Next(80, 120) * 0.01;
				for (int k = num3; k < num4; k++)
				{
					for (int l = num5; l < num6; l++)
					{
						float num8 = Math.Abs((float)k - value.X);
						float num9 = Math.Abs((float)l - value.Y);
						double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
						if (num10 < num7 * 0.4 && !Main.tile.At(k, l).Active)
						{
							Main.tile.At(k, l).SetActive(true);
							Main.tile.At(k, l).SetType(0);
						}
					}
				}
				value += value2;
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
				value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
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
			double num = (double)WorldModify.genRand.Next(25, 50);
			double num2 = num;
			float num3 = (float)WorldModify.genRand.Next(30, 80);
			if (WorldModify.genRand.Next(5) == 0)
			{
				num *= 1.5;
				num2 *= 1.5;
				num3 *= 1.2f;
			}
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j - num3 * 0.3f;
			Vector2 value2;
			value2.X = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldModify.genRand.Next(-20, -10) * 0.1f;
			while (num > 0.0 && num3 > 0f)
			{
				if ((double)value.Y + num2 * 0.5 > Main.worldSurface)
				{
					num3 = 0f;
				}
				num -= (double)WorldModify.genRand.Next(3);
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
				num2 = num * (double)WorldModify.genRand.Next(80, 120) * 0.01;
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
								Main.tile.At(k, l).SetLiquid(255);
							}
							Main.tile.At(k, l).SetActive(false);
						}
					}
				}
				value += value2;
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
				value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
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
			double num = (double)WorldModify.genRand.Next(40, 70);
			double num2 = num;
			float num3 = (float)WorldModify.genRand.Next(10, 20);
			if (WorldModify.genRand.Next(5) == 0)
			{
				num *= 1.5;
				num2 *= 1.5;
				num3 *= 1.2f;
			}
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j - num3 * 0.3f;
			Vector2 value2;
			value2.X = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldModify.genRand.Next(-20, -10) * 0.1f;
			while (num > 0.0 && num3 > 0f)
			{
				num -= (double)WorldModify.genRand.Next(3);
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
				num2 = num * (double)WorldModify.genRand.Next(80, 120) * 0.01;
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
									Main.tile.At(k, l).SetActive(false);
								}
							}
							else
							{
								Main.tile.At(k, l).SetType(59);
							}
							Main.tile.At(k, l).SetLiquid(0);
							Main.tile.At(k, l).SetLava(false);
						}
					}
				}
				value += value2;
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
				value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
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
			double num = (double)WorldModify.genRand.Next(7, 15);
			int num2 = 1;
			if (WorldModify.genRand.Next(2) == 0)
			{
				num2 = -1;
			}
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			int k = WorldModify.genRand.Next(20, 40);
			Vector2 value2;
			value2.Y = (float)WorldModify.genRand.Next(10, 20) * 0.01f;
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
				double num7 = num * (double)WorldModify.genRand.Next(80, 120) * 0.01;
				for (int l = num3; l < num4; l++)
				{
					for (int m = num5; m < num6; m++)
					{
						float num8 = Math.Abs((float)l - value.X);
						float num9 = Math.Abs((float)m - value.Y);
						double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
						if (num10 < num7 * 0.4)
						{
							Main.tile.At(l, m).SetActive(false);
						}
					}
				}
				value += value2;
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
				value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
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
				Cavinator((int)value.X, (int)value.Y, steps - 1);
			}
		}

		public static void CaveOpenater(int i, int j)
		{
			double num = (double)WorldModify.genRand.Next(7, 12);
			int num2 = 1;
			if (WorldModify.genRand.Next(2) == 0)
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
				double num7 = num * (double)WorldModify.genRand.Next(80, 120) * 0.01;
				for (int l = num3; l < num4; l++)
				{
					for (int m = num5; m < num6; m++)
					{
						float num8 = Math.Abs((float)l - value.X);
						float num9 = Math.Abs((float)m - value.Y);
						double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
						if (num10 < num7 * 0.4)
						{
							Main.tile.At(l, m).SetActive(false);
						}
					}
				}
				value += value2;
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
				value2.Y += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
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
							WorldModify.PlaceTile(i, j - 1, 3, true, false, -1, 0);
						}
					}
					else if (Main.tile.At(i, j).Type == 23 && Main.tile.At(i, j).Active && !Main.tile.At(i, j - 1).Active)
					{
						WorldModify.PlaceTile(i, j - 1, 24, true, false, -1, 0);
					}
				}
			}
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
					if (!WorldModify.EmptyTileCheck(i - 1, i, num - 1, num, -1))
					{
						return false;
					}
					Main.tile.At(i - 1, num - 1).SetActive(true);
					Main.tile.At(i - 1, num - 1).SetType(12);
					Main.tile.At(i - 1, num - 1).SetFrameX(0);
					Main.tile.At(i - 1, num - 1).SetFrameY(0);
					Main.tile.At(i, num - 1).SetActive(true);
					Main.tile.At(i, num - 1).SetType(12);
					Main.tile.At(i, num - 1).SetFrameX(18);
					Main.tile.At(i, num - 1).SetFrameY(0);
					Main.tile.At(i - 1, num).SetActive(true);
					Main.tile.At(i - 1, num).SetType(12);
					Main.tile.At(i - 1, num).SetFrameX(0);
					Main.tile.At(i - 1, num).SetFrameY(18);
					Main.tile.At(i, num).SetActive(true);
					Main.tile.At(i, num).SetType(12);
					Main.tile.At(i, num).SetFrameX(18);
					Main.tile.At(i, num).SetFrameY(18);
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
			Main.tile.At(x - 1, y - 1).SetActive(true);
			Main.tile.At(x - 1, y - 1).SetType(31);
			Main.tile.At(x - 1, y - 1).SetFrameX(0);
			Main.tile.At(x - 1, y - 1).SetFrameY(0);
			Main.tile.At(x, y - 1).SetActive(true);
			Main.tile.At(x, y - 1).SetType(31);
			Main.tile.At(x, y - 1).SetFrameX(18);
			Main.tile.At(x, y - 1).SetFrameY(0);
			Main.tile.At(x - 1, y).SetActive(true);
			Main.tile.At(x - 1, y).SetType(31);
			Main.tile.At(x - 1, y).SetFrameX(0);
			Main.tile.At(x - 1, y).SetFrameY(18);
			Main.tile.At(x, y).SetActive(true);
			Main.tile.At(x, y).SetType(31);
			Main.tile.At(x, y).SetFrameX(18);
			Main.tile.At(x, y).SetFrameY(18);
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
					HellHouse(i, num2);
					i += WorldModify.genRand.Next(15, 80);
				}
			}
		}

		public static void HellHouse(int i, int j)
		{
			int num = WorldModify.genRand.Next(8, 20);
			int num2 = WorldModify.genRand.Next(3);
			int num3 = WorldModify.genRand.Next(7);
			int num4 = j;
			for (int k = 0; k < num2; k++)
			{
				int num5 = WorldModify.genRand.Next(5, 9);
				HellRoom(i, num4, num, num5);
				num4 -= num5;
			}
			num4 = j;
			for (int l = 0; l < num3; l++)
			{
				int num6 = WorldModify.genRand.Next(5, 9);
				num4 += num6;
				HellRoom(i, num4, num, num6);
			}
			for (int m = i - num / 2; m <= i + num / 2; m++)
			{
				num4 = j;
				while (num4 < Main.maxTilesY && ((Main.tile.At(m, num4).Active && Main.tile.At(m, num4).Type == 76) || Main.tile.At(m, num4).Wall == 13))
				{
					num4++;
				}
				int num7 = 6 + WorldModify.genRand.Next(3);
				while (num4 < Main.maxTilesY && !Main.tile.At(m, num4).Active)
				{
					num7--;
					Main.tile.At(m, num4).SetActive(true);
					Main.tile.At(m, num4).SetType(57);
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
					int num10 = WorldModify.genRand.Next(i - num / 2 + 1, i + num / 2 - 1);
					int num11 = WorldModify.genRand.Next(i - num / 2 + 1, i + num / 2 - 1);
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
							Main.tile.At(n, num4).SetWall(13);
						}
						Main.tile.At(n, num4).SetType(19);
						Main.tile.At(n, num4).SetActive(true);
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
				int num16 = WorldModify.genRand.Next(i - num / 2, i + num / 2 + 1);
				int num17 = WorldModify.genRand.Next(num8, num9);
				int num18 = WorldModify.genRand.Next(3, 8);
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
									Main.tile.At(num19, num20).SetActive(false);
								}
								Main.tile.At(num19, num20).SetWall(0);
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
						Main.tile.At(k, l).SetActive(true);
						Main.tile.At(k, l).SetType(76);
						Main.tile.At(k, l).SetLiquid(0);
						Main.tile.At(k, l).SetLava(false);
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
						Main.tile.At(m, n).SetActive(false);
						Main.tile.At(m, n).SetWall(13);
						Main.tile.At(m, n).SetLiquid(0);
						Main.tile.At(m, n).SetLava(false);
					}
					catch
					{
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
					WorldModify.GrowTree(i, num);
					num++;
				}
			}
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
				if (WorldModify.EmptyTileCheck(x - TREE_RADIUS, x + TREE_RADIUS, freeTilesAbove - 55, freeTilesAbove - 1, 20))
				{
					WorldModify.GrowTreeShared(x, y, freeTilesAbove, 20, 30);
					return true;
				}
			}
			return false;
		}

	}
}
