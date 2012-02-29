using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Terraria_Server.Commands;
using Terraria_Server.Misc;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.Logging;
using Terraria_Server.Plugins;
using System.Text;
using Terraria_Server.Language;

namespace Terraria_Server.WorldMod
{
	public static class WorldGen
	{
		public const Int32 MAX_TRAPS = 1000;

		public static bool mudWall { get; set; }
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
		public static double rockLayer;
		public static double terrarainMaxY;
		public static double worldSurface;

		private static void resetGen()
		{
			mudWall = false;
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

		//Works to what is needed.
		public static int AsciiToInt32(string input)
		{
			var encodedBytes = Encoding.ASCII.GetBytes(input);

			int ret = 0;
			foreach (byte b in encodedBytes)
				ret += (int)b;

			return ret;
		}

		public static void GenerateWorld(Func<Int32, Int32, ITile> TileRefs, string Seed)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int seed = -1;
			try
			{
				seed = AsciiToInt32(Seed);
			}
			catch (Exception) { }
			GenerateWorld(TileRefs, seed);
		}

		public static void GenerateWorld(Func<Int32, Int32, ITile> TileRefs, int seed = -1)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			Main.checkXmas();

			NPC.ClearNames();
			NPC.SetNames();

			WorldModify.gen = true;
			resetGen();
			if (seed > 0)
				WorldModify.threadRand = new Random(seed);
			else
				WorldModify.genRand = new Random((int)DateTime.Now.Ticks);

			Main.worldID = WorldModify.genRand.Next(Int32.MaxValue);
			int num = 0;
			int num2 = 0;
			double TerrainY = (double)Main.maxTilesY * 0.3;
			TerrainY *= (double)WorldModify.genRand.Next(90, 110) * 0.005;
			double num4 = TerrainY + (double)Main.maxTilesY * 0.2;
			num4 *= (double)WorldModify.genRand.Next(90, 110) * 0.01;
			terrarainMaxY = TerrainY;
			worldSurface = TerrainY;
			double num7 = num4;
			rockLayer = num4;
			int Direction = 0;

			if (WorldModify.genRand.Next(2) == 0)
				Direction = -1;
			else
				Direction = 1;

			using (var prog = new ProgressLogger(Main.maxTilesX - 1, Languages.Generation_Terrain))
			{
				for (int TerrainX = 0; TerrainX < Main.maxTilesX; TerrainX++)
				{
					prog.Value = TerrainX;

					if (TerrainY < terrarainMaxY)
						terrarainMaxY = TerrainY;

					if (TerrainY > worldSurface)
						worldSurface = TerrainY;

					if (num4 < num7)
						num7 = num4;

					if (num4 > rockLayer)
						rockLayer = num4;

					if (num2 <= 0)
					{
						num = WorldModify.genRand.Next(0, 5);
						num2 = WorldModify.genRand.Next(5, 40);
						if (num == 0)
							num2 *= (int)((double)WorldModify.genRand.Next(5, 30) * 0.2);
					}
					num2--;
					if (num == 0)
					{
						while (WorldModify.genRand.Next(0, 7) == 0)
							TerrainY += (double)WorldModify.genRand.Next(-1, 2);
					}
					else if (num == 1)
					{
						while (WorldModify.genRand.Next(0, 4) == 0)
							TerrainY -= 1.0;

						while (WorldModify.genRand.Next(0, 10) == 0)
							TerrainY += 1.0;
					}
					else if (num == 2)
					{
						while (WorldModify.genRand.Next(0, 4) == 0)
							TerrainY += 1.0;

						while (WorldModify.genRand.Next(0, 10) == 0)
							TerrainY -= 1.0;
					}
					else if (num == 3)
					{
						while (WorldModify.genRand.Next(0, 2) == 0)
							TerrainY -= 1.0;

						while (WorldModify.genRand.Next(0, 6) == 0)
							TerrainY += 1.0;

					}
					else if (num == 4)
					{
						while (WorldModify.genRand.Next(0, 2) == 0)
							TerrainY += 1.0;

						while (WorldModify.genRand.Next(0, 5) == 0)
							TerrainY -= 1.0;

					}
					if (TerrainY < (double)Main.maxTilesY * 0.15)
					{
						TerrainY = (double)Main.maxTilesY * 0.15;
						num2 = 0;
					}
					else if (TerrainY > (double)Main.maxTilesY * 0.3)
					{
						TerrainY = (double)Main.maxTilesY * 0.3;
						num2 = 0;
					}
					if ((TerrainX < 275 || TerrainX > Main.maxTilesX - 275) && TerrainY > (double)Main.maxTilesY * 0.25)
					{
						TerrainY = (double)Main.maxTilesY * 0.25;
						num2 = 1;
					}

					while (WorldModify.genRand.Next(0, 3) == 0)
						num4 += (double)WorldModify.genRand.Next(-2, 3);

					if (num4 < TerrainY + (double)Main.maxTilesY * 0.05)
						num4 += 1.0;

					if (num4 > TerrainY + (double)Main.maxTilesY * 0.35)
						num4 -= 1.0;

					int terrainY = 0;
					while ((double)terrainY < TerrainY)
					{
						TileRefs(TerrainX, terrainY).SetActive(false);
						//TileRefs(TerrainX, terrainY).SetLighted(true);
						TileRefs(TerrainX, terrainY).SetFrameX(-1);
						TileRefs(TerrainX, terrainY).SetFrameY(-1);
						terrainY++;
					}
					for (int j = (int)TerrainY; j < Main.maxTilesY; j++)
					{
						if ((double)j < num4)
						{
							TileRefs(TerrainX, j).SetActive(true);
							TileRefs(TerrainX, j).SetType(0);
							TileRefs(TerrainX, j).SetFrameX(-1);
							TileRefs(TerrainX, j).SetFrameY(-1);
						}
						else
						{
							TileRefs(TerrainX, j).SetActive(true);
							TileRefs(TerrainX, j).SetType(1);
							TileRefs(TerrainX, j).SetFrameX(-1);
							TileRefs(TerrainX, j).SetFrameY(-1);
						}
					}
				}
			}
			Main.worldSurface = worldSurface + 25.0;
			Main.rockLayer = rockLayer;
			double RealRockLayer = (double)((int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6);
			Main.rockLayer = Main.worldSurface + RealRockLayer;
			WorldModify.waterLine = (int)(Main.rockLayer + (double)Main.maxTilesY) / 2;
			WorldModify.waterLine += WorldModify.genRand.Next(-100, 20);
			WorldModify.lavaLine = WorldModify.waterLine + WorldModify.genRand.Next(50, 80);

			for (int k = 0; k < (int)((double)Main.maxTilesX * 0.0015); k++)
			{
				int[] array = new int[10];
				int[] array2 = new int[10];
				int num14 = WorldModify.genRand.Next(450, Main.maxTilesX - 450);
				int num15 = 0;
				for (int l = 0; l < 10; l++)
				{
					while (!TileRefs(num14, num15).Active)
						num15++;

					array[l] = num14;
					array2[l] = num15 - WorldModify.genRand.Next(11, 16);
					num14 += WorldModify.genRand.Next(5, 11);
				}
				for (int m = 0; m < 10; m++)
				{
					TileRunner(TileRefs, array[m], array2[m], (double)WorldModify.genRand.Next(5, 8),
						WorldModify.genRand.Next(6, 9), 0, true, -2f, -0.3f, false, true);
					TileRunner(TileRefs, array[m], array2[m], (double)WorldModify.genRand.Next(5, 8),
						WorldModify.genRand.Next(6, 9), 0, true, 2f, -0.3f, false, true);
				}
			}

			AddSand(TileRefs, Direction);

			GenerateHills(TileRefs);

			if (((Main.Xmas || WorldModify.genRand.Next(3) == 0) || Program.properties.AlwaysGenerateSnow) && Program.properties.GenerateSnow)
				GenerateSnow(TileRefs);

			PutDirtBehindDirt(TileRefs);

			PlaceRocksWithinDirt(TileRefs);

			PlaceDirtWithinRocks(TileRefs, num7);

			AddClay(TileRefs);

			MakeRandomHoles(TileRefs);

			GenerateSmallCaves(TileRefs);
			GenerateLargeCaves(TileRefs);
			GenerateSurfaceCaves(TileRefs);

			if (Program.properties.GenerateJungle)
				GenerateJungle(TileRefs, Direction);

			GenerateFloatingIslands(TileRefs);

			AddMushroomPatches(TileRefs);

			PlaceMudInDirt(TileRefs, num7);

			AddShinies(TileRefs, num7);

			AddWebs(TileRefs);

			CreateUnderworld(TileRefs);
			AddHellHouses(TileRefs);

			AddWaterBodies(TileRefs);


			int x = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.05), (int)((double)Main.maxTilesX * 0.2)); //Left?
			int x2 = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.8), (int)((double)Main.maxTilesX * 0.95)); //Right?
			int y = 0;

			if (numDungeons >= 2) //Custom Dungeons (0 allowed?)
				numDungeons = 2; //Limt dungeons at 2, May not fit?
			else //1 dungeon
			{
				if (Direction != -1)
					x = x2; //X axis = right
			}

			for (int dnum = 0; dnum < numDungeons; dnum++)
			{
				y = (int)((Main.rockLayer + (double)Main.maxTilesY) / 2.0) + WorldModify.genRand.Next(-200, 200); //Generate a custom Y each time
				if (dnum == 1)
				{
					//num9 = direction
					x = ((x == x2) ? x : x2); //we want the opposite of the original
				}
				MakeDungeon(TileRefs, x, y, 41, 7);
			}

			if (Direction != -1)
				Direction = 1;

			MakeWorldEvil(TileRefs);

			GenerateMountainCaves(TileRefs);

			CreateBeaches(TileRefs, Direction);

			AddGems(TileRefs);
			GravitateSand(TileRefs);

			CleanUpDirtBackgrounds(TileRefs);

			PlaceAltars(TileRefs);

			SettleLiquids(TileRefs, null);

			PlaceLifeCrystals(TileRefs);
			HideTreasure(TileRefs);
			HideMoreTreasure(TileRefs);
			HideJungleTreasure(TileRefs);
			HideWaterTreasure(TileRefs);

			AddIslandHouses(TileRefs);
			PlaceTraps(TileRefs);

			PlaceBreakables(TileRefs);

			PlaceHellforges(TileRefs);

			SpreadGrass(TileRefs);
			GrowCacti(TileRefs);


			int num256 = 5;
			bool flag21 = true;
			while (flag21)
			{
				int num257 = Main.maxTilesX / 2 + WorldModify.genRand.Next(-num256, num256 + 1);
				for (int num258 = 0; num258 < Main.maxTilesY; num258++)
				{
					if (TileRefs(num257, num258).Active)
					{
						Main.spawnTileX = num257;
						Main.spawnTileY = num258;
						TileRefs(num257, num258 - 1).SetLighted(true);
						break;
					}
				}
				flag21 = false;
				num256++;
				if ((double)Main.spawnTileY > Main.worldSurface)
					flag21 = true;

				if (TileRefs(Main.spawnTileX, Main.spawnTileY - 1).Liquid > 0)
					flag21 = true;

			}
			int num259 = 10;
			while ((double)Main.spawnTileY > Main.worldSurface)
			{
				int num260 = WorldModify.genRand.Next(Main.maxTilesX / 2 - num259, Main.maxTilesX / 2 + num259);
				for (int num261 = 0; num261 < Main.maxTilesY; num261++)
				{
					if (TileRefs(num260, num261).Active)
					{
						Main.spawnTileX = num260;
						Main.spawnTileY = num261;
						TileRefs(num260, num261 - 1).SetLighted(true);
						break;
					}
				}
				num259++;
			}

			ProgramLog.Log(Languages.Generation_AddingNPCs);
			NPC.SpawnGuide();
			/*NPC.SpawnTDCMQuestGiver(); */

			PlantSunflowers(TileRefs);
			PlantTrees(TileRefs);
			PlantHerbs(TileRefs);
			PlantWeeds(TileRefs);
			GrowVines(TileRefs);
			PlantFlowers(TileRefs);
			PlantMushrooms(TileRefs);

			var ctx = new HookContext() { };
			var args = new HookArgs.WorldGeneration() { };

			HookPoints.WorldGeneration.Invoke(ref ctx, ref args);

			WorldModify.gen = false;
		}

		public static void AddSand(Func<Int32, Int32, ITile> TileRefs, int direction)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int sandLines = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.0008), (int)((double)Main.maxTilesX * 0.0025)) + 2;
			//var someotherCaveGen = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-06);
			sandLines += 2;
			var extra = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-06);
			using (var sandProg = new ProgressLogger(sandLines + extra, Languages.Generation_AddingSand))
			{
				for (int k = 0; k < sandLines; k++)
				{
					int sandStartX = WorldModify.genRand.Next(Main.maxTilesX);
					while ((float)sandStartX > (float)Main.maxTilesX * 0.4f && (float)sandStartX < (float)Main.maxTilesX * 0.6f)
					{
						sandStartX = WorldModify.genRand.Next(Main.maxTilesX);
					}

					int sandStartEnd = WorldModify.genRand.Next(35, 90);
					if (k == 1)
					{
						sandStartEnd += (int)((float)WorldModify.genRand.Next(20, 40) * (float)(Main.maxTilesX / 4200));
					}
					if (WorldModify.genRand.Next(3) == 0)
					{
						sandStartEnd *= 2;
					}
					if (k == 1)
					{
						sandStartEnd *= 2;
					}
					int sandBoundaries = sandStartX - sandStartEnd;
					sandStartEnd = WorldModify.genRand.Next(35, 90);
					if (WorldModify.genRand.Next(3) == 0)
					{
						sandStartEnd *= 2;
					}
					if (k == 1)
					{
						sandStartEnd *= 2;
					}
					int SandLength = sandStartX + sandStartEnd;
					if (sandBoundaries < 0)
					{
						sandBoundaries = 0;
					}
					if (SandLength > Main.maxTilesX)
					{
						SandLength = Main.maxTilesX;
					}
					if (k == 0)
					{
						sandBoundaries = 0;
						SandLength = WorldModify.genRand.Next(260, 300);
						if (direction == 1)
						{
							SandLength += 40;
						}
					}
					else if (k == 2)
					{
						sandBoundaries = Main.maxTilesX - WorldModify.genRand.Next(260, 300);
						SandLength = Main.maxTilesX;
						if (direction == -1)
						{
							sandBoundaries -= 40;
						}
					}
					int num20 = WorldModify.genRand.Next(50, 100);
					for (int posX = sandBoundaries; posX < SandLength; posX++)
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
						int posY = 0;
						while ((double)posY < Main.worldSurface)
						{
							if (TileRefs(posX, posY).Active)
							{
								int num22 = num20;
								if (posX - sandBoundaries < num22)
								{
									num22 = posX - sandBoundaries;
								}
								if (SandLength - posX < num22)
								{
									num22 = SandLength - posX;
								}
								num22 += WorldModify.genRand.Next(5);
								for (int m = posY; m < posY + num22; m++)
								{
									if (posX > sandBoundaries + WorldModify.genRand.Next(5) && posX < SandLength - WorldModify.genRand.Next(5))
									{
										TileRefs(posX, m).SetType(53);
									}
								}
								break;
							}
							posY++;
						}
					}
					sandProg.Value++;
				}

				for (int n = 0; n < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-06); n++)
				{
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), (double)WorldModify.genRand.Next(15, 70), WorldModify.genRand.Next(20, 130), 53, false, 0f, 0f, false, true);
					sandProg.Value++;
				}
				numMCaves = 0;
			}
		}

		public static void GenerateHills(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var maxHills = (int)((double)Main.maxTilesX * 0.0008);
			using (var hillProg = new ProgressLogger(maxHills, Languages.Generation_GeneratingHills))
			{
				for (int i = 0; i < maxHills; i++)
				{
					int maxCaves = 0;
					bool flag = false;
					bool flag2 = false;
					int HillX = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
					while (!flag2)
					{
						flag2 = true;
						while (HillX > Main.maxTilesX / 2 - 100 && HillX < Main.maxTilesX / 2 + 100)
						{
							HillX = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
						}
						for (int i2 = 0; i2 < numMCaves; i2++)
						{
							if (HillX > mCaveX[i2] - 50 && HillX < mCaveX[i2] + 50)
							{
								maxCaves++;
								flag2 = false;
								break;
							}
						}
						if (maxCaves >= 200)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						int HillY = 0;
						while ((double)HillY < Main.worldSurface)
						{
							if (TileRefs(HillX, HillY).Active)
							{
								Mountinater(TileRefs, HillX, HillY);
								mCaveX[numMCaves] = HillX;
								mCaveY[numMCaves] = HillY;
								numMCaves++;
								break;
							}
							HillY++;
						}
					}
				}
				hillProg.Value++;
			}
		}

		public static void GenerateSnow(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int snowX = WorldModify.genRand.Next(Main.maxTilesX);
			while ((float)snowX < (float)Main.maxTilesX * 0.35f || (float)snowX > (float)Main.maxTilesX * 0.65f)
				snowX = WorldModify.genRand.Next(Main.maxTilesX);

			int snowLenX = WorldModify.genRand.Next(35, 90);
			float modifier = (float)(Main.maxTilesX / 4200);

			snowLenX += (int)((float)WorldModify.genRand.Next(20, 40) * modifier);
			snowLenX += (int)((float)WorldModify.genRand.Next(20, 40) * modifier);

			int snowStartX = snowX - snowLenX;
			snowLenX = WorldModify.genRand.Next(35, 90);
			snowLenX += (int)((float)WorldModify.genRand.Next(20, 40) * modifier);
			snowLenX += (int)((float)WorldModify.genRand.Next(20, 40) * modifier);

			int snowPosXEnd = snowX + snowLenX;
			if (snowStartX < 0)
				snowStartX = 0;
			if (snowPosXEnd > Main.maxTilesX)
				snowPosXEnd = Main.maxTilesX;

			int startX = WorldModify.genRand.Next(50, 100);

			using (var snowProg = new ProgressLogger(snowPosXEnd - snowStartX, Languages.Generation_AddingSnow))
			{
				for (int posX = snowStartX; posX < snowPosXEnd; posX++)
				{
					if (WorldModify.genRand.Next(2) == 0)
					{
						startX += WorldModify.genRand.Next(-1, 2);

						if (startX < 50)
							startX = 50;
						if (startX > 100)
							startX = 100;
					}
					int posY = 0;
					while ((double)posY < Main.worldSurface)
					{
						if (Main.tile.At(posX, posY).Active)
						{
							int lenY = startX;

							if (posX - snowStartX < lenY)
								lenY = posX - snowStartX;

							if (snowPosXEnd - posX < lenY)
								lenY = snowPosXEnd - posX;

							lenY += WorldModify.genRand.Next(5);
							for (int tileY = posY; tileY < posY + lenY; tileY++)
							{
								if (posX > snowStartX + WorldModify.genRand.Next(5) && posX < snowPosXEnd - WorldModify.genRand.Next(5))
									Main.tile.At(posX, tileY).SetType(147);
							}
							break;
						}
						posY++;
					}
					snowProg.Value++;
				}
			}
		}

		public static void PutDirtBehindDirt(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int outlier = 0;

			using (var prog = new ProgressLogger(Main.maxTilesX - 3, Languages.Generation_PuttingDirtBehindDirt))
			{
				for (int posX = 1; posX < Main.maxTilesX - 1; posX++)
				{
					prog.Value = posX - 1;

					bool flag3 = false;
					outlier += WorldModify.genRand.Next(-1, 2);
					if (outlier < 0)
					{
						outlier = 0;
					}
					if (outlier > 10)
					{
						outlier = 10;
					}
					int posY = 0;
					while ((double)posY < Main.worldSurface + 10.0 && (double)posY <= Main.worldSurface + (double)outlier)
					{
						if (flag3)
						{
							TileRefs(posX, posY).SetWall(2);
						}
						if (TileRefs(posX, posY).Active && TileRefs(posX - 1, posY).Active && TileRefs(posX + 1, posY).Active && TileRefs(posX, posY + 1).Active && TileRefs(posX - 1, posY + 1).Active && TileRefs(posX + 1, posY + 1).Active)
						{
							flag3 = true;
						}
						posY++;
					}
				}
			}
		}

		public static void PlaceRocksWithinDirt(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002);
			var Max2 = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0045);
			using (var rockProg = new ProgressLogger((Max * 2) + Max2, Languages.Generation_PlacingRocks))
			{
				for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); i++)
				{
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(0, (int)terrarainMaxY + 1), (double)WorldModify.genRand.Next(4, 15), WorldModify.genRand.Next(5, 40), 1, false, 0f, 0f, false, true);
					rockProg.Value++;
				}
				for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); i++)
				{
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)terrarainMaxY, (int)worldSurface + 1), (double)WorldModify.genRand.Next(4, 10), WorldModify.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
					rockProg.Value++;
				}
				for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0045); i++)
				{
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)worldSurface, (int)rockLayer + 1), (double)WorldModify.genRand.Next(2, 7), WorldModify.genRand.Next(2, 23), 1, false, 0f, 0f, false, true);
					rockProg.Value++;
				}
			}
		}

		public static void PlaceDirtWithinRocks(Func<Int32, Int32, ITile> TileRefs, double num7)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.005);
			using (var dirtProg = new ProgressLogger(Max, Languages.Generation_PlaceDirtWithinRocks))
			{
				for (int i = 0; i < Max; i++)
				{
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(2, 6), WorldModify.genRand.Next(2, 40), 0, false, 0f, 0f, false, true);
					dirtProg.Value++;
				}
			}
		}

		public static void AddClay(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05);
			var Max2 = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 5E-05);
			var Max3 = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05);
			var Max4 = Main.maxTilesX - 5;
			using (var clayProg = new ProgressLogger(Max + Max2 + Max3 + Max4, Languages.Generation_AddingClay))
			{
				for (int i = 0; i < Max; i++)
				{
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(0, (int)terrarainMaxY), (double)WorldModify.genRand.Next(4, 14), WorldModify.genRand.Next(10, 50), 40, false, 0f, 0f, false, true);
					clayProg.Value++;
				}
				for (int i = 0; i < Max2; i++)
				{
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)terrarainMaxY, (int)worldSurface + 1), (double)WorldModify.genRand.Next(8, 14), WorldModify.genRand.Next(15, 45), 40, false, 0f, 0f, false, true);
					clayProg.Value++;
				}
				for (int i = 0; i < Max3; i++)
				{
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)worldSurface, (int)rockLayer + 1), (double)WorldModify.genRand.Next(8, 15), WorldModify.genRand.Next(5, 50), 40, false, 0f, 0f, false, true);
					clayProg.Value++;
				}
				for (int posX = 5; posX < Max4; posX++)
				{
					int startY = 1;
					while ((double)startY < Main.worldSurface - 1.0)
					{
						if (TileRefs(posX, startY).Active)
						{
							for (int posY = startY; posY < startY + 5; posY++)
							{
								if (TileRefs(posX, posY).Type == 40)
								{
									TileRefs(posX, posY).SetType(0);
								}
							}
							break;
						}
						startY++;
					}
					clayProg.Value++;
				}
			}
		}

		public static void MakeRandomHoles(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var MaxRandomHoles = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015);

			using (var prog = new ProgressLogger(MaxRandomHoles - 1, Languages.Generation_MakingRandomHoles))
			{
				for (int i = 0; i < MaxRandomHoles; i++)
				{
					prog.Value = i;

					int type = -1;
					if (WorldModify.genRand.Next(5) == 0)
					{
						type = -2;
					}
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)worldSurface, Main.maxTilesY), (double)WorldModify.genRand.Next(2, 5), WorldModify.genRand.Next(2, 20), type, false, 0f, 0f, false, true);
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)worldSurface, Main.maxTilesY), (double)WorldModify.genRand.Next(8, 15), WorldModify.genRand.Next(7, 30), type, false, 0f, 0f, false, true);
				}
			}
		}

		public static void GenerateSmallCaves(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var MaxSmallHoles = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05);
			using (var prog = new ProgressLogger(MaxSmallHoles - 1, Languages.Generation_GeneratingSmallCaves))
			{
				for (int i = 0; i < MaxSmallHoles; i++)
				{
					prog.Value = i;

					if (rockLayer <= (double)Main.maxTilesY)
					{
						int type2 = -1;
						if (WorldModify.genRand.Next(6) == 0)
						{
							type2 = -2;
						}
						TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)terrarainMaxY, (int)rockLayer + 1), (double)WorldModify.genRand.Next(5, 15), WorldModify.genRand.Next(30, 200), type2, false, 0f, 0f, false, true);
					}
				}
			}
		}

		public static void GenerateLargeCaves(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var MaxLargeCaves = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015);
			using (var prog = new ProgressLogger(MaxLargeCaves - 1, Languages.Generation_GeneratingLargeCaves))
			{
				for (int i = 0; i < MaxLargeCaves; i++)
				{
					prog.Value = i;

					if (rockLayer <= (double)Main.maxTilesY)
					{
						//Potential cave type definition
						int type3 = -1;
						if (WorldModify.genRand.Next(10) == 0)
						{
							type3 = -2;
						}
						TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)rockLayer, Main.maxTilesY), (double)WorldModify.genRand.Next(6, 20), WorldModify.genRand.Next(50, 300), type3, false, 0f, 0f, false, true);
					}
				}
			}
		}

		public static void GenerateSurfaceCaves(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Max = (int)((double)Main.maxTilesX * 0.0025);
			var Max2 = (int)((double)Main.maxTilesX * 0.0007);
			var Max3 = (int)((double)Main.maxTilesX * 0.0003);
			var Max4 = (int)((double)Main.maxTilesX * 0.0004);
			var Max5 = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002);
			using (var surfaceCaveProg = new ProgressLogger(Max + Max2 + Max3 + Max4 + Max5, Languages.Generation_GeneratingSurfaceCaves))
			{
				for (int num47 = 0; num47 < Max; num47++)
				{
					int posX = WorldModify.genRand.Next(0, Main.maxTilesX);
					int posY = 0;
					while ((double)posY < worldSurface)
					{
						if (TileRefs(posX, posY).Active)
						{
							TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next(3, 6), WorldModify.genRand.Next(5, 50), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 1f, false, true);
							break;
						}
						posY++;
					}
					surfaceCaveProg.Value++;
				}
				for (int i = 0; i < Max2; i++)
				{
					int posX = WorldModify.genRand.Next(0, Main.maxTilesX);
					int posY = 0;
					while ((double)posY < worldSurface)
					{
						if (TileRefs(posX, posY).Active)
						{
							TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next(10, 15), WorldModify.genRand.Next(50, 130), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
							break;
						}
						posY++;
					}
					surfaceCaveProg.Value++;
				}
				for (int i = 0; i < Max3; i++)
				{
					int posX = WorldModify.genRand.Next(0, Main.maxTilesX);
					int posY = 0;
					while ((double)posY < worldSurface)
					{
						if (TileRefs(posX, posY).Active)
						{
							TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next(12, 25), WorldModify.genRand.Next(150, 500), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 4f, false, true);
							TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next(8, 17), WorldModify.genRand.Next(60, 200), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
							TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next(5, 13), WorldModify.genRand.Next(40, 170), -1, false, (float)WorldModify.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
							break;
						}
						posY++;
					}
					surfaceCaveProg.Value++;
				}
				for (int i = 0; i < Max4; i++)
				{
					int posX = WorldModify.genRand.Next(0, Main.maxTilesX);
					int posY = 0;
					while ((double)posY < worldSurface)
					{
						if (TileRefs(posX, posY).Active)
						{
							TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next(7, 12), WorldModify.genRand.Next(150, 250), -1, false, 0f, 1f, true, true);
							break;
						}
						posY++;
					}
					surfaceCaveProg.Value++;
				}
				for (int i = 0; i < Max5; i++)
				{
					int posX = WorldModify.genRand.Next(1, Main.maxTilesX - 1);
					int posY = WorldModify.genRand.Next((int)terrarainMaxY, (int)worldSurface);
					if (posY >= Main.maxTilesY)
					{
						posY = Main.maxTilesY - 2;
					}
					if (TileRefs(posX - 1, posY).Active && TileRefs(posX - 1, posY).Type == 0 && TileRefs(posX + 1, posY).Active && TileRefs(posX + 1, posY).Type == 0 && TileRefs(posX, posY - 1).Active && TileRefs(posX, posY - 1).Type == 0 && TileRefs(posX, posY + 1).Active && TileRefs(posX, posY + 1).Type == 0)
					{
						TileRefs(posX, posY).SetActive(true);
						TileRefs(posX, posY).SetType(2);
					}
					posX = WorldModify.genRand.Next(1, Main.maxTilesX - 1);
					posY = WorldModify.genRand.Next(0, (int)terrarainMaxY);
					if (posY >= Main.maxTilesY)
					{
						posY = Main.maxTilesY - 2;
					}
					if (TileRefs(posX - 1, posY).Active && TileRefs(posX - 1, posY).Type == 0 && TileRefs(posX + 1, posY).Active && TileRefs(posX + 1, posY).Type == 0 && TileRefs(posX, posY - 1).Active && TileRefs(posX, posY - 1).Type == 0 && TileRefs(posX, posY + 1).Active && TileRefs(posX, posY + 1).Type == 0)
					{
						TileRefs(posX, posY).SetActive(true);
						TileRefs(posX, posY).SetType(2);
					}
				}
				surfaceCaveProg.Value++;
			}
		}

		public static void GenerateJungle(Func<Int32, Int32, ITile> TileRefs, int direction)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			using (var jungleprog = new ProgressLogger(100, Languages.Generation_GeneratingJungle))
			{
				float jungleSize = (float)(Main.maxTilesX / 4200);
				jungleSize *= 1.5f;
				int posX = 0;
				float sizeModifier = (float)WorldModify.genRand.Next(15, 30) * 0.01f;
				if (direction == -1)
				{
					sizeModifier = 1f - sizeModifier;
					posX = (int)((float)Main.maxTilesX * sizeModifier);
				}
				else
				{
					posX = (int)((float)Main.maxTilesX * sizeModifier);
				}
				int posY = (int)((double)Main.maxTilesY + Main.rockLayer) / 2;
				posX += WorldModify.genRand.Next((int)(-100f * jungleSize), (int)(101f * jungleSize));
				posY += WorldModify.genRand.Next((int)(-100f * jungleSize), (int)(101f * jungleSize));
				int section1X = posX;
				int section1Y = posY;
				TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next((int)(250f * jungleSize), (int)(500f * jungleSize)), WorldModify.genRand.Next(50, 150), 59, false, (float)(direction * 3), 0f, false, true);

				int num82 = 0;
				while ((float)num82 < 6f * jungleSize)
				{
					TileRunner(TileRefs, posX + WorldModify.genRand.Next(-(int)(125f * jungleSize), (int)(125f * jungleSize)), posY + WorldModify.genRand.Next(-(int)(125f * jungleSize), (int)(125f * jungleSize)), (double)WorldModify.genRand.Next(3, 7), WorldModify.genRand.Next(3, 8), WorldModify.genRand.Next(63, 65), false, 0f, 0f, false, true);
					num82++;
				}
				mudWall = true;

				jungleprog.Value = 15;

				posX += WorldModify.genRand.Next((int)(-250f * jungleSize), (int)(251f * jungleSize));
				posY += WorldModify.genRand.Next((int)(-150f * jungleSize), (int)(151f * jungleSize));
				int section2X = posX;
				int section2Y = posY;
				int keepX = posX;
				int keepY = posY;
				TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next((int)(250f * jungleSize), (int)(500f * jungleSize)), WorldModify.genRand.Next(50, 150), 59, false, 0f, 0f, false, true);

				mudWall = false;
				int num87 = 0;
				while ((float)num87 < 6f * jungleSize)
				{
					TileRunner(TileRefs, posX + WorldModify.genRand.Next(-(int)(125f * jungleSize), (int)(125f * jungleSize)), posY + WorldModify.genRand.Next(-(int)(125f * jungleSize), (int)(125f * jungleSize)), (double)WorldModify.genRand.Next(3, 7), WorldModify.genRand.Next(3, 8), WorldModify.genRand.Next(65, 67), false, 0f, 0f, false, true);
					num87++;
				}

				mudWall = true;
				jungleprog.Value = 30;

				posX += WorldModify.genRand.Next((int)(-400f * jungleSize), (int)(401f * jungleSize));
				posY += WorldModify.genRand.Next((int)(-150f * jungleSize), (int)(151f * jungleSize));
				int section3X = posX;
				int section3Y = posY;
				TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next((int)(250f * jungleSize), (int)(500f * jungleSize)), WorldModify.genRand.Next(50, 150), 59, false, (float)(direction * -3), 0f, false, true);

				mudWall = false;
				int num90 = 0;
				while ((float)num90 < 6f * jungleSize)
				{
					TileRunner(TileRefs, posX + WorldModify.genRand.Next(-(int)(125f * jungleSize), (int)(125f * jungleSize)), posY + WorldModify.genRand.Next(-(int)(125f * jungleSize), (int)(125f * jungleSize)), (double)WorldModify.genRand.Next(3, 7), WorldModify.genRand.Next(3, 8), WorldModify.genRand.Next(67, 69), false, 0f, 0f, false, true);
					num90++;
				}
				mudWall = true;

				jungleprog.Value = 45;

				posX = (section1X + section2X + section3X) / 3;
				posY = (section1Y + section2Y + section3Y) / 3;
				TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next((int)(400f * jungleSize), (int)(600f * jungleSize)), 10000, 59, false, 0f, -20f, true, true);
				JungleRunner(TileRefs, null, posX, posY);

				jungleprog.Value = 60;
				mudWall = false;

				for (int num91 = 0; num91 < Main.maxTilesX / 10; num91++)
				{
					posX = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
					posY = WorldModify.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 200);
					while (TileRefs(posX, posY).Wall != 15)
					{
						posX = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
						posY = WorldModify.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 200);
					}
					MudWallRunner(TileRefs, posX, posY);
				}

				posX = keepX;
				posY = keepY;
				int i = 0;
				while ((float)i <= 20f * jungleSize)
				{
					jungleprog.Value = (int)(60f + (float)i / jungleSize);

					posX += WorldModify.genRand.Next((int)(-5f * jungleSize), (int)(6f * jungleSize));
					posY += WorldModify.genRand.Next((int)(-5f * jungleSize), (int)(6f * jungleSize));
					TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next(40, 100), WorldModify.genRand.Next(300, 500), 59, false, 0f, 0f, false, true);
					i++;
				}
				i = 0;
				while ((float)i <= 10f * jungleSize)
				{
					jungleprog.Value = (int)(80f + (float)i / jungleSize * 2f);

					posX = keepX + WorldModify.genRand.Next((int)(-600f * jungleSize), (int)(600f * jungleSize));
					posY = keepY + WorldModify.genRand.Next((int)(-200f * jungleSize), (int)(200f * jungleSize));
					while (posX < 1 || posX >= Main.maxTilesX - 1 || posY < 1 || posY >= Main.maxTilesY - 1 || TileRefs(posX, posY).Type != 59)
					{
						posX = keepX + WorldModify.genRand.Next((int)(-600f * jungleSize), (int)(600f * jungleSize));
						posY = keepY + WorldModify.genRand.Next((int)(-200f * jungleSize), (int)(200f * jungleSize));
					}
					int i2 = 0;
					while ((float)i2 < 8f * jungleSize)
					{
						posX += WorldModify.genRand.Next(-30, 31);
						posY += WorldModify.genRand.Next(-30, 31);
						int type4 = -1;
						if (WorldModify.genRand.Next(7) == 0)
						{
							type4 = -2;
						}
						TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next(10, 20), WorldModify.genRand.Next(30, 70), type4, false, 0f, 0f, false, true);
						i2++;
					}
					i++;
				}
				i = 0;
				while ((float)i <= 300f * jungleSize)
				{
					posX = keepX + WorldModify.genRand.Next((int)(-600f * jungleSize), (int)(600f * jungleSize));
					posY = keepY + WorldModify.genRand.Next((int)(-200f * jungleSize), (int)(200f * jungleSize));
					while (posX < 1 || posX >= Main.maxTilesX - 1 || posY < 1 || posY >= Main.maxTilesY - 1 || TileRefs(posX, posY).Type != 59)
					{
						posX = keepX + WorldModify.genRand.Next((int)(-600f * jungleSize), (int)(600f * jungleSize));
						posY = keepY + WorldModify.genRand.Next((int)(-200f * jungleSize), (int)(200f * jungleSize));
					}
					TileRunner(TileRefs, posX, posY, (double)WorldModify.genRand.Next(4, 10), WorldModify.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
					if (WorldModify.genRand.Next(4) == 0)
					{
						int type5 = WorldModify.genRand.Next(63, 69);
						TileRunner(TileRefs, posX + WorldModify.genRand.Next(-1, 2), posY + WorldModify.genRand.Next(-1, 2), (double)WorldModify.genRand.Next(3, 7), WorldModify.genRand.Next(4, 8), type5, false, 0f, 0f, false, true);
					}
					i++;
				}
				posX = keepX;
				posY = keepY;
				float num75 = (float)WorldModify.genRand.Next(6, 10);
				float num76 = (float)(Main.maxTilesX / 4200);
				num75 *= num76;
				i = 0;
				while ((float)i < num75)
				{
					bool flag4 = true;
					while (flag4)
					{
						posX = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
						posY = WorldModify.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 300);
						if (TileRefs(posX, posY).Type == 59)
						{
							flag4 = false;
							int additionX = WorldModify.genRand.Next(2, 4);
							int additionY = WorldModify.genRand.Next(2, 4);
							for (int tileX = posX - additionX - 1; tileX <= posX + additionX + 1; tileX++)
							{
								for (int tileY = posY - additionY - 1; tileY <= posY + additionY + 1; tileY++)
								{
									TileRefs(tileX, tileY).SetActive(true);
									TileRefs(tileX, tileY).SetType(45);
									TileRefs(tileX, tileY).SetLiquid(0);
									TileRefs(tileX, tileY).SetLava(false);
								}
							}
							for (int tileX = posX - additionX; tileX <= posX + additionX; tileX++)
							{
								for (int tileY = posY - additionY; tileY <= posY + additionY; tileY++)
								{
									TileRefs(tileX, tileY).SetActive(false);
									TileRefs(tileX, tileY).SetWall(10);
								}
							}
							bool flag5 = false;
							int i2 = 0;
							while (!flag5 && i2 < 100)
							{
								i2++;
								int tileX = WorldModify.genRand.Next(posX - additionX, posX + additionX + 1);
								int tileY = WorldModify.genRand.Next(posY - additionY, posY + additionY - 2);
								WorldModify.PlaceTile(TileRefs, null, tileX, tileY, 4, true, false, -1, 0);
								if (TileRefs(tileX, tileY).Type == 4)
								{
									flag5 = true;
								}
							}
							for (int tileX = posX - additionX - 1; tileX <= posX + additionX + 1; tileX++)
							{
								for (int tileY = posY + additionY - 2; tileY <= posY + additionY; tileY++)
								{
									TileRefs(tileX, tileY).SetActive(false);
								}
							}
							for (int tileX = posX - additionX - 1; tileX <= posX + additionX + 1; tileX++)
							{
								for (int tileY = posY + additionY - 2; tileY <= posY + additionY - 1; tileY++)
								{
									TileRefs(tileX, tileY).SetActive(false);
								}
							}
							for (int tileX = posX - additionX - 1; tileX <= posX + additionX + 1; tileX++)
							{
								int amount = 4;
								int tileY = posY + additionY + 2;
								while (!TileRefs(tileX, tileY).Active && tileY < Main.maxTilesY && amount > 0)
								{
									TileRefs(tileX, tileY).SetActive(true);
									TileRefs(tileX, tileY).SetType(59);
									tileY++;
									amount--;
								}
							}
							additionX -= WorldModify.genRand.Next(1, 3);
							int _tileY = posY - additionY - 2;
							while (additionX > -1)
							{
								for (int tileX = posX - additionX - 1; tileX <= posX + additionX + 1; tileX++)
								{
									TileRefs(tileX, _tileY).SetActive(true);
									TileRefs(tileX, _tileY).SetType(45);
								}
								additionX -= WorldModify.genRand.Next(1, 3);
								_tileY--;
							}
							JChestX[numJChests] = posX;
							JChestY[numJChests] = posY;
							numJChests++;
						}
					}
					i++;
				}
				for (int tileX = 0; tileX < Main.maxTilesX; tileX++)
				{
					for (int tileY = 0; tileY < Main.maxTilesY; tileY++)
					{
						if (TileRefs(tileX, tileY).Active)
						{
							WorldModify.SpreadGrass(TileRefs, tileX, tileY, 59, 60, true);
						}
					}
				}
			}
		}

		public static void GenerateFloatingIslands(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			numIslandHouses = 0;
			houseCount = 0;

			using (var fiProg = new ProgressLogger(WorldModify.ficount, Languages.Generation_GeneratingFloatingIslands))
			{
				//Might be limited due to us not modifying fihX[] etc
				for (int islandInt = 0; islandInt < WorldModify.ficount; islandInt++)
				{
					int fihCount = 0;
					bool flag6 = false;
					int randomX = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
					bool flag7 = false;
					while (!flag7)
					{
						flag7 = true;
						while (randomX > Main.maxTilesX / 2 - 80 && randomX < Main.maxTilesX / 2 + 80)
						{
							randomX = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
						}
						for (int fiHouseInt = 0; fiHouseInt < numIslandHouses; fiHouseInt++)
						{
							if (randomX > fihX[fiHouseInt] - 80 && randomX < fihX[fiHouseInt] + 80)
							{
								fihCount++;
								flag7 = false;
								break;
							}
						}
						if (fihCount >= 200)
						{
							flag6 = true;
							break;
						}
					}
					if (!flag6)
					{
						int height = 200;
						while ((double)height < Main.worldSurface)
						{
							if (TileRefs(randomX, height).Active)
							{
								int islandX = randomX;
								int islandY = WorldModify.genRand.Next(90, height - 100);
								while ((double)islandY > terrarainMaxY - 50.0)
								{
									islandY--;
								}
								FloatingIsland(TileRefs, islandX, islandY);
								fihX[numIslandHouses] = islandX;
								fihY[numIslandHouses] = islandY;
								numIslandHouses++;
								break;
							}
							height++;
						}
					}
					fiProg.Value++;
				}
			}
		}

		public static void AddMushroomPatches(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Max = Main.maxTilesX / 300;
			var Max2 = Main.maxTilesX;
			using (var mushProg = new ProgressLogger(Max + Max2, Languages.Generation_AddingMushroomPatches))
			{
				for (int i = 0; i < Max; i++)
				{
					int i2 = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.3), (int)((double)Main.maxTilesX * 0.7));
					int j2 = WorldModify.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);
					ShroomPatch(TileRefs, i2, j2);
					mushProg.Value++;
				}
				for (int tileX = 0; tileX < Max2; tileX++)
				{
					for (int tileY = (int)Main.worldSurface; tileY < Main.maxTilesY; tileY++)
					{
						if (TileRefs(tileX, tileY).Active)
						{
							WorldModify.SpreadGrass(TileRefs, tileX, tileY, 59, 70, false);
						}
					}
					mushProg.Value++;
				}
			}


		}

		public static void PlaceMudInDirt(Func<Int32, Int32, ITile> TileRefs, double num7)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001);
			using (var mudProg = new ProgressLogger(Max, Languages.Generation_PlacingMudInDirt))
			{
				for (int i = 0; i < Max; i++)
				{
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(2, 6), WorldModify.genRand.Next(2, 40), 59, false, 0f, 0f, false, true);
					mudProg.Value++;
				}
			}
		}

		public static void AddShinies(Func<Int32, Int32, ITile> TileRefs, double num7)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;
			// shinies
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
				+ num114max + num115max + num116max + num117max + num118max + num119max + num120max - 12, Languages.Generation_AddingShinies))
			{
				int total = 0;

				for (int i = 0; i < num109max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)terrarainMaxY, (int)worldSurface), (double)WorldModify.genRand.Next(3, 6), WorldModify.genRand.Next(2, 6), 7, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num110max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)worldSurface, (int)rockLayer), (double)WorldModify.genRand.Next(3, 7), WorldModify.genRand.Next(3, 7), 7, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num111max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(4, 9), WorldModify.genRand.Next(4, 8), 7, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num112max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)terrarainMaxY, (int)worldSurface), (double)WorldModify.genRand.Next(3, 7), WorldModify.genRand.Next(2, 5), 6, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num113max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)worldSurface, (int)rockLayer), (double)WorldModify.genRand.Next(3, 6), WorldModify.genRand.Next(3, 6), 6, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num114max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(4, 9), WorldModify.genRand.Next(4, 8), 6, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num115max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)worldSurface, (int)rockLayer), (double)WorldModify.genRand.Next(3, 6), WorldModify.genRand.Next(3, 6), 9, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num116max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(4, 9), WorldModify.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num117max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(0, (int)terrarainMaxY), (double)WorldModify.genRand.Next(4, 9), WorldModify.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num118max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(4, 8), WorldModify.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num119max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(0, (int)terrarainMaxY - 20), (double)WorldModify.genRand.Next(4, 8), WorldModify.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
				}
				for (int i = 0; i < num120max; i++)
				{
					prog.Value = total++;
					TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next((int)num7, Main.maxTilesY), (double)WorldModify.genRand.Next(2, 4), WorldModify.genRand.Next(3, 6), 22, false, 0f, 0f, false, true);
				}
			}
		}

		public static void AddWebs(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001);
			using (var webProg = new ProgressLogger(Max, Languages.Generation_AddingWebs))
			{
				for (int caveInt = 0; caveInt < Max; caveInt++)
				{
					int tileX = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
					int tileY = WorldModify.genRand.Next((int)terrarainMaxY, Main.maxTilesY - 20);
					if (caveInt < numMCaves)
					{
						tileX = mCaveX[caveInt];
						tileY = mCaveY[caveInt];
					}
					if (!TileRefs(tileX, tileY).Active)
					{
						if ((double)tileY <= Main.worldSurface)
						{
							if (TileRefs(tileX, tileY).Wall <= 0)
							{
								continue;
							}
						}
						while (!TileRefs(tileX, tileY).Active && tileY > (int)terrarainMaxY)
						{
							tileY--;
						}
						tileY++;
						int num124 = 1;
						if (WorldModify.genRand.Next(2) == 0)
						{
							num124 = -1;
						}
						while (!TileRefs(tileX, tileY).Active && tileX > 10 && tileX < Main.maxTilesX - 10)
						{
							tileX += num124;
						}
						tileX -= num124;
						if ((double)tileY > Main.worldSurface || TileRefs(tileX, tileY).Wall > 0)
						{
							TileRunner(TileRefs, tileX, tileY, (double)WorldModify.genRand.Next(4, 13), WorldModify.genRand.Next(2, 5), 51, true, (float)num124, -1f, false, false);
						}
					}
					webProg.Value++;
				}
			}
		}

		public static void CreateUnderworld(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Max = (Main.maxTilesX * 5) + (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008) + (Main.maxTilesX - 20);

			using (var uwprog = new ProgressLogger(Max, Languages.Generation_CreatingTheUnderworld))
			{
				int num148 = Main.maxTilesY - WorldModify.genRand.Next(150, 190);
				for (int num149 = 0; num149 < Main.maxTilesX; num149++)
				{
					uwprog.Value++;

					num148 += WorldModify.genRand.Next(-3, 4);
					if (num148 < Main.maxTilesY - 190)
					{
						num148 = Main.maxTilesY - 190;
					}
					if (num148 > Main.maxTilesY - 160)
					{
						num148 = Main.maxTilesY - 160;
					}
					for (int num150 = num148 - 20 - WorldModify.genRand.Next(3); num150 < Main.maxTilesY; num150++)
					{
						if (num150 >= num148)
						{
							TileRefs(num149, num150).SetActive(false);
							TileRefs(num149, num150).SetLava(false);
							TileRefs(num149, num150).SetLiquid(0);
						}
						else
						{
							TileRefs(num149, num150).SetType(57);
						}
					}
				}
				int num151 = Main.maxTilesY - WorldModify.genRand.Next(40, 70);
				for (int num152 = 10; num152 < Main.maxTilesX - 10; num152++)
				{
					uwprog.Value++;

					num151 += WorldModify.genRand.Next(-10, 11);
					if (num151 > Main.maxTilesY - 60)
					{
						num151 = Main.maxTilesY - 60;
					}
					if (num151 < Main.maxTilesY - 100)
					{
						num151 = Main.maxTilesY - 120;
					}
					for (int num153 = num151; num153 < Main.maxTilesY - 10; num153++)
					{
						if (!TileRefs(num152, num153).Active)
						{
							TileRefs(num152, num153).SetLava(true);
							TileRefs(num152, num153).SetLiquid(255);
						}
					}
				}
				for (int num154 = 0; num154 < Main.maxTilesX; num154++)
				{
					uwprog.Value++;

					if (WorldModify.genRand.Next(50) == 0)
					{
						int num155 = Main.maxTilesY - 65;
						while (!TileRefs(num154, num155).Active && num155 > Main.maxTilesY - 135)
						{
							num155--;
						}
						WorldGen.TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), num155 + WorldModify.genRand.Next(20, 50),
							(double)WorldModify.genRand.Next(15, 20), 1000, 57, true, 0f, (float)WorldModify.genRand.Next(1, 3), true, true);
					}
				}
				Liquid.QuickWater(TileRefs, null, -2, -1, -1 /*, uwprog */);
				for (int num156 = 0; num156 < Main.maxTilesX; num156++)
				{
					uwprog.Value++;

					float num157 = (float)num156 / (float)(Main.maxTilesX - 1);

					//uwprog.Value = (int)(num157 * 100f / 2f + 50f);

					if (WorldModify.genRand.Next(13) == 0)
					{
						int num158 = Main.maxTilesY - 65;
						while ((TileRefs(num156, num158).Liquid > 0 || TileRefs(num156, num158).Active) && num158 > Main.maxTilesY - 140)
						{
							num158--;
						}
						WorldGen.TileRunner(TileRefs, num156, num158 - WorldModify.genRand.Next(2, 5), (double)WorldModify.genRand.Next(5, 30), 1000, 57, true, 0f,
							(float)WorldModify.genRand.Next(1, 3), true, true);
						float num159 = (float)WorldModify.genRand.Next(1, 3);
						if (WorldModify.genRand.Next(3) == 0)
						{
							num159 *= 0.5f;
						}
						if (WorldModify.genRand.Next(2) == 0)
						{
							WorldGen.TileRunner(TileRefs, num156, num158 - WorldModify.genRand.Next(2, 5), (double)((int)((float)WorldModify.genRand.Next(5, 15) * num159)),
								(int)((float)WorldModify.genRand.Next(10, 15) * num159), 57, true, 1f, 0.3f, false, true);
						}
						if (WorldModify.genRand.Next(2) == 0)
						{
							num159 = (float)WorldModify.genRand.Next(1, 3);
							WorldGen.TileRunner(TileRefs, num156, num158 - WorldModify.genRand.Next(2, 5), (double)((int)((float)WorldModify.genRand.Next(5, 15) * num159)),
								(int)((float)WorldModify.genRand.Next(10, 15) * num159), 57, true, -1f, 0.3f, false, true);
						}
						WorldGen.TileRunner(TileRefs, num156 + WorldModify.genRand.Next(-10, 10), num158 + WorldModify.genRand.Next(-10, 10),
							(double)WorldModify.genRand.Next(5, 15), WorldModify.genRand.Next(5, 10), -2, false, (float)WorldModify.genRand.Next(-1, 3), (float)WorldModify.genRand.Next(-1, 3), false, true);
						if (WorldModify.genRand.Next(3) == 0)
						{
							WorldGen.TileRunner(TileRefs, num156 + WorldModify.genRand.Next(-10, 10), num158 + WorldModify.genRand.Next(-10, 10),
								(double)WorldModify.genRand.Next(10, 30), WorldModify.genRand.Next(10, 20), -2, false, (float)WorldModify.genRand.Next(-1, 3), (float)WorldModify.genRand.Next(-1, 3), false, true);
						}
						if (WorldModify.genRand.Next(5) == 0)
						{
							WorldGen.TileRunner(TileRefs, num156 + WorldModify.genRand.Next(-15, 15), num158 + WorldModify.genRand.Next(-15, 10),
								(double)WorldModify.genRand.Next(15, 30), WorldModify.genRand.Next(5, 20), -2, false, (float)WorldModify.genRand.Next(-1, 3), (float)WorldModify.genRand.Next(-1, 3), false, true);
						}
					}
				}
				
				for (int num160 = 0; num160 < Main.maxTilesX; num160++)
				{
					uwprog.Value++;

					WorldGen.TileRunner(TileRefs, WorldModify.genRand.Next(20, Main.maxTilesX - 20), WorldModify.genRand.Next(Main.maxTilesY - 180, Main.maxTilesY - 10),
						(double)WorldModify.genRand.Next(2, 7), WorldModify.genRand.Next(2, 7), -2, false, 0f, 0f, false, true);
				}

				for (int num161 = 0; num161 < Main.maxTilesX; num161++)
				{
					uwprog.Value++;

					if (!TileRefs(num161, Main.maxTilesY - 145).Active)
					{
						TileRefs(num161, Main.maxTilesY - 145).SetLiquid(255);
						TileRefs(num161, Main.maxTilesY - 145).SetLava(true);
					}
					if (!TileRefs(num161, Main.maxTilesY - 144).Active)
					{
						TileRefs(num161, Main.maxTilesY - 144).SetLiquid(255);
						TileRefs(num161, Main.maxTilesY - 144).SetLava(true);
					}
				}

				for (int num162 = 0; num162 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); num162++)
				{
					uwprog.Value++;

					WorldGen.TileRunner(TileRefs, WorldModify.genRand.Next(0, Main.maxTilesX), WorldModify.genRand.Next(Main.maxTilesY - 140, Main.maxTilesY),
						(double)WorldModify.genRand.Next(2, 7), WorldModify.genRand.Next(3, 7), 58, false, 0f, 0f, false, true);
				}
			}
		}

		public static void AddWaterBodies(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int waterBodyMax = WorldModify.genRand.Next(2, (int)((double)Main.maxTilesX * 0.005));

			using (var prog = new ProgressLogger(waterBodyMax - 1, Languages.Generation_AddingWaterBodies))
			{
				for (int num140 = 0; num140 < waterBodyMax; num140++)
				{
					prog.Value = num140;

					int tileX = WorldModify.genRand.Next(300, Main.maxTilesX - 300);
					while (tileX > Main.maxTilesX / 2 - 50 && tileX < Main.maxTilesX / 2 + 50)
					{
						tileX = WorldModify.genRand.Next(300, Main.maxTilesX - 300);
					}
					int tileY = (int)terrarainMaxY - 20;
					while (!TileRefs(tileX, tileY).Active)
					{
						tileY++;
					}
					Lakinater(TileRefs, tileX, tileY);
				}
			}
		}

		public static void MakeWorldEvil(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int evilSegment = 0;

			using (var prog = new ProgressLogger(100, Languages.Generation_MakingTheWorldEvil))
			{
				while ((double)evilSegment < (double)Main.maxTilesX * 0.00045)
				{
					float num145 = (float)((double)evilSegment / ((double)Main.maxTilesX * 0.00045));

					prog.Value = (int)(num145 * 100f);

					bool flag8 = false;
					int chasmX = 0;
					int chasmStartX = 0;
					int chasmEndX = 0;
					while (!flag8)
					{
						int jungleLength = 0;
						flag8 = true;
						int MaxTileX_d2 = Main.maxTilesX / 2;
						int cOutlier = 200;
						chasmX = WorldModify.genRand.Next(320, Main.maxTilesX - 320);
						chasmStartX = chasmX - WorldModify.genRand.Next(200) - 100;
						chasmEndX = chasmX + WorldModify.genRand.Next(200) + 100;
						if (chasmStartX < 285)
						{
							chasmStartX = 285;
						}
						if (chasmEndX > Main.maxTilesX - 285)
						{
							chasmEndX = Main.maxTilesX - 285;
						}
						if (chasmX > MaxTileX_d2 - cOutlier && chasmX < MaxTileX_d2 + cOutlier)
						{
							flag8 = false;
						}
						if (chasmStartX > MaxTileX_d2 - cOutlier && chasmStartX < MaxTileX_d2 + cOutlier)
						{
							flag8 = false;
						}
						if (chasmEndX > MaxTileX_d2 - cOutlier && chasmEndX < MaxTileX_d2 + cOutlier)
						{
							flag8 = false;
						}
						for (int tileX = chasmStartX; tileX < chasmEndX; tileX++)
						{
							for (int tileY = 0; tileY < (int)Main.worldSurface; tileY += 5)
							{
								if (TileRefs(tileX, tileY).Active && Main.tileDungeon[(int)TileRefs(tileX, tileY).Type])
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
						if (jungleLength < 200 && JungleX > chasmStartX && JungleX < chasmEndX)
						{
							jungleLength++;
							flag8 = false;
						}
					}
					int tilePass = 0;
					for (int tileX = chasmStartX; tileX < chasmEndX; tileX++)
					{
						if (tilePass > 0)
						{
							tilePass--;
						}
						if (tileX == chasmX || tilePass == 0)
						{
							int tileY = (int)terrarainMaxY;
							while ((double)tileY < Main.worldSurface - 1.0)
							{
								if (TileRefs(tileX, tileY).Active || TileRefs(tileX, tileY).Wall > 0)
								{
									if (tileX == chasmX)
									{
										tilePass = 20;
										ChasmRunner(TileRefs, tileX, tileY, WorldModify.genRand.Next(150) + 150, true);
									}
									if (WorldModify.genRand.Next(35) == 0 && tilePass == 0)
									{
										tilePass = 30;
										ChasmRunner(TileRefs, tileX, tileY, WorldModify.genRand.Next(50) + 50, true);
									}
									break;
								}
								else
								{
									tileY++;
								}
							}
						}
						int tileYCheck = (int)terrarainMaxY;
						while ((double)tileYCheck < Main.worldSurface - 1.0)
						{
							if (TileRefs(tileX, tileYCheck).Active)
							{
								int resetY = tileYCheck + WorldModify.genRand.Next(10, 14);
								for (int tileY = tileYCheck; tileY < resetY; tileY++)
								{
									if ((TileRefs(tileX, tileY).Type == 59 || TileRefs(tileX, tileY).Type == 60) && tileX >= chasmStartX + WorldModify.genRand.Next(5) && tileX < chasmEndX - WorldModify.genRand.Next(5))
									{
										TileRefs(tileX, tileY).SetType(0);
									}
								}
								break;
							}
							tileYCheck++;
						}
					}
					double chasmTop = Main.worldSurface + 40.0;
					for (int chasmCycle = chasmStartX; chasmCycle < chasmEndX; chasmCycle++)
					{
						chasmTop += (double)WorldModify.genRand.Next(-2, 3);
						if (chasmTop < Main.worldSurface + 30.0)
						{
							chasmTop = Main.worldSurface + 30.0;
						}
						if (chasmTop > Main.worldSurface + 50.0)
						{
							chasmTop = Main.worldSurface + 50.0;
						}
						int tileX = chasmCycle;
						bool flag9 = false;
						int tileY = (int)terrarainMaxY;
						while ((double)tileY < chasmTop)
						{
							if (TileRefs(tileX, tileY).Active)
							{
								if (TileRefs(tileX, tileY).Type == 53 && tileX >= chasmStartX + WorldModify.genRand.Next(5) && tileX <= chasmEndX - WorldModify.genRand.Next(5))
								{
									TileRefs(tileX, tileY).SetType(0);
								}
								if (TileRefs(tileX, tileY).Type == 0 && (double)tileY < Main.worldSurface - 1.0 && !flag9)
								{
									WorldModify.SpreadGrass(TileRefs, tileX, tileY, 0, 23, true);
								}
								flag9 = true;
								if (TileRefs(tileX, tileY).Type == 1 && tileX >= chasmStartX + WorldModify.genRand.Next(5) && tileX <= chasmEndX - WorldModify.genRand.Next(5))
								{
									TileRefs(tileX, tileY).SetType(25);
								}
								if (TileRefs(tileX, tileY).Type == 2)
								{
									TileRefs(tileX, tileY).SetType(23);
								}
							}
							tileY++;
						}
					}
					for (int secondChasmCycle = chasmStartX; secondChasmCycle < chasmEndX; secondChasmCycle++)
					{
						for (int cycleBeforeWater = 0; cycleBeforeWater < Main.maxTilesY - 50; cycleBeforeWater++)
						{
							if (TileRefs(secondChasmCycle, cycleBeforeWater).Active && TileRefs(secondChasmCycle, cycleBeforeWater).Type == 31)
							{
								int sectionStartX = secondChasmCycle - 13;
								int sectionEndX = secondChasmCycle + 13;
								//Terrain minus water?
								int tMwStartX = cycleBeforeWater - 13;
								int tMwEndX = cycleBeforeWater + 13;
								for (int tileX = sectionStartX; tileX < sectionEndX; tileX++)
								{
									if (tileX > 10 && tileX < Main.maxTilesX - 10)
									{
										for (int tileY = tMwStartX; tileY < tMwEndX; tileY++)
										{
											if (Math.Abs(tileX - secondChasmCycle) + Math.Abs(tileY - cycleBeforeWater) < 9 + WorldModify.genRand.Next(11) && WorldModify.genRand.Next(3) != 0 && TileRefs(tileX, tileY).Type != 31)
											{
												TileRefs(tileX, tileY).SetActive(true);
												TileRefs(tileX, tileY).SetType(25);
												if (Math.Abs(tileX - secondChasmCycle) <= 1 && Math.Abs(tileY - cycleBeforeWater) <= 1)
												{
													TileRefs(tileX, tileY).SetActive(false);
												}
											}
											if (TileRefs(tileX, tileY).Type != 31 && Math.Abs(tileX - secondChasmCycle) <= 2 + WorldModify.genRand.Next(3) && Math.Abs(tileY - cycleBeforeWater) <= 2 + WorldModify.genRand.Next(3))
											{
												TileRefs(tileX, tileY).SetActive(false);
											}
										}
									}
								}
							}
						}
					}
					evilSegment++;
				}
			}
		}

		public static void GenerateMountainCaves(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			using (var mountainProg = new ProgressLogger(numMCaves, Languages.Generation_GeneratingMoutainCaves))
			{
				for (int cave = 0; cave < numMCaves; cave++)
				{
					int i3 = mCaveX[cave];
					int j3 = mCaveY[cave];
					CaveOpenater(TileRefs, i3, j3);
					Cavinator(TileRefs, i3, j3, WorldModify.genRand.Next(40, 50));
					mountainProg.Value++;
				}
			}
		}

		public static void CreateBeaches(Func<Int32, Int32, ITile> TileRefs, int direction)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int startTilePreserve = 0;
			int inactiveTileY = 0;
			int inactiveTX = 20;
			int inactiveTleX = Main.maxTilesX - 20;

			using (var beachProg = new ProgressLogger(2, Languages.Generation_CreatingBeachesOnEitherSide))
			{
				for (int beachNumSide = 0; beachNumSide < 2; beachNumSide++)
				{
					int beachMinimumX = 0;
					int startTileX = 0;
					if (beachNumSide == 0)
					{
						beachMinimumX = 0;
						startTileX = WorldModify.genRand.Next(125, 200) + 50;
						if (direction == 1) //Right
						{
							startTileX = 275;
						}
						int heightModifier = 0;
						float beachHeight = 1f;
						int startTileY = 0;
						while (!TileRefs(startTileX - 1, startTileY).Active)
						{
							startTileY++;
						}
						startTilePreserve = startTileY;
						startTileY += WorldModify.genRand.Next(1, 5);
						for (int tileX = startTileX - 1; tileX >= beachMinimumX; tileX--)
						{
							heightModifier++;
							if (heightModifier < 3)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.2f;
							}
							else if (heightModifier < 6)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.15f;
							}
							else if (heightModifier < 9)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.1f;
							}
							else if (heightModifier < 15)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.07f;
							}
							else if (heightModifier < 50)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
							}
							else if (heightModifier < 75)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.04f;
							}
							else if (heightModifier < 100)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.03f;
							}
							else if (heightModifier < 125)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.02f;
							}
							else if (heightModifier < 150)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
							}
							else if (heightModifier < 175)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.005f;
							}
							else if (heightModifier < 200)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.001f;
							}
							else if (heightModifier < 230)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
							}
							else if (heightModifier < 235)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
							}
							else if (heightModifier < 240)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.1f;
							}
							else if (heightModifier < 245)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
							}
							else if (heightModifier < 255)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
							}

							if (heightModifier == 235)
							{
								inactiveTleX = tileX;
							}
							if (heightModifier == 235)
							{
								inactiveTX = tileX;
							}
							int extendHeight = WorldModify.genRand.Next(15, 20);
							int tileY = 0;
							while ((float)tileY < (float)startTileY + beachHeight + (float)extendHeight)
							{
								if ((float)tileY < (float)startTileY + beachHeight * 0.75f - 3f)
								{
									TileRefs(tileX, tileY).SetActive(false);
									if (tileY > startTileY)
									{
										TileRefs(tileX, tileY).SetLiquid(255);
									}
									else if (tileY == startTileY)
									{
										TileRefs(tileX, tileY).SetLiquid(127);
									}
								}
								else if (tileY > startTileY)
								{
									TileRefs(tileX, tileY).SetType(53);
									TileRefs(tileX, tileY).SetActive(true);
								}
								TileRefs(tileX, tileY).SetWall(0);
								tileY++;
							}
						}
					}
					else
					{
						beachMinimumX = Main.maxTilesX - WorldModify.genRand.Next(125, 200) - 50;
						startTileX = Main.maxTilesX;
						if (direction == -1)
						{
							beachMinimumX = Main.maxTilesX - 275;
						}
						float beachHeight = 1f;
						int heightModifier = 0;
						int findY = 0;
						while (!TileRefs(beachMinimumX, findY).Active)
						{
							findY++;
						}
						inactiveTileY = findY;
						findY += WorldModify.genRand.Next(1, 5);
						for (int tileX = beachMinimumX; tileX < startTileX; tileX++)
						{
							heightModifier++;
							if (heightModifier < 3)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.2f;
							}
							else if (heightModifier < 6)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.15f;
							}
							else if (heightModifier < 9)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.1f;
							}
							else if (heightModifier < 15)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.07f;
							}
							else if (heightModifier < 50)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
							}
							else if (heightModifier < 75)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.04f;
							}
							else if (heightModifier < 100)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.03f;
							}
							else if (heightModifier < 125)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.02f;
							}
							else if (heightModifier < 150)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
							}
							else if (heightModifier < 175)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.005f;
							}
							else if (heightModifier < 200)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.001f;
							}
							else if (heightModifier < 230)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
							}
							else if (heightModifier < 235)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
							}
							else if (heightModifier < 240)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.1f;
							}
							else if (heightModifier < 245)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.05f;
							}
							else if (heightModifier < 255)
							{
								beachHeight += (float)WorldModify.genRand.Next(10, 20) * 0.01f;
							}

							if (heightModifier == 235)
							{
								inactiveTleX = tileX;
							}
							int beachExtension = WorldModify.genRand.Next(15, 20);
							int tileY = 0;
							while ((float)tileY < (float)findY + beachHeight + (float)beachExtension)
							{
								if ((float)tileY < (float)findY + beachHeight * 0.75f - 3f && (double)tileY < Main.worldSurface - 2.0)
								{
									TileRefs(tileX, tileY).SetActive(false);
									if (tileY > findY)
									{
										TileRefs(tileX, tileY).SetLiquid(255);
									}
									else if (tileY == findY)
									{
										TileRefs(tileX, tileY).SetLiquid(127);
									}
								}
								else if (tileY > findY)
								{
									TileRefs(tileX, tileY).SetType(53);
									TileRefs(tileX, tileY).SetActive(true);
								}
								TileRefs(tileX, tileY).SetWall(0);
								tileY++;
							}
						}
					}
				} // end beaches

				while (!TileRefs(inactiveTX, startTilePreserve).Active)
				{
					startTilePreserve++;
				}
				startTilePreserve++;
				while (!TileRefs(inactiveTleX, inactiveTileY).Active)
				{
					inactiveTileY++;
				}
				inactiveTileY++;
			}
		}

		public static void AddGems(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			using (var genProg = new ProgressLogger(68, Languages.Generation_AddingGems))
			{
				for (int Type = 63; Type <= 68; Type++)
				{
					float gemAmount = 0f;
					if (Type == 67)
					{
						gemAmount = (float)Main.maxTilesX * 0.5f;
					}
					else if (Type == 66)
					{
						gemAmount = (float)Main.maxTilesX * 0.45f;
					}
					else if (Type == 63)
					{
						gemAmount = (float)Main.maxTilesX * 0.3f;
					}
					else if (Type == 65)
					{
						gemAmount = (float)Main.maxTilesX * 0.25f;
					}
					else if (Type == 64)
					{
						gemAmount = (float)Main.maxTilesX * 0.1f;
					}
					else if (Type == 68)
					{
						gemAmount = (float)Main.maxTilesX * 0.05f;
					}
					gemAmount *= 0.2f;
					int num193 = 0;
					while ((float)num193 < gemAmount)
					{
						int tileX = WorldModify.genRand.Next(0, Main.maxTilesX);
						int tileY = WorldModify.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
						while (TileRefs(tileX, tileY).Type != 1)
						{
							tileX = WorldModify.genRand.Next(0, Main.maxTilesX);
							tileY = WorldModify.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
						}
						TileRunner(TileRefs, tileX, tileY, (double)WorldModify.genRand.Next(2, 6), WorldModify.genRand.Next(3, 7),
							Type, false, 0f, 0f, false, true);
						num193++;
					}
				} // end gems
			}

		}

		public static void GravitateSand(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			using (var prog = new ProgressLogger(Main.maxTilesX - 1, Languages.Generation_GravitatingSand))
			{
				for (int tileX = 0; tileX < Main.maxTilesX; tileX++)
				{
					prog.Value = tileX;

					for (int sTileY = Main.maxTilesY - 5; sTileY > 0; sTileY--)
					{
						if (TileRefs(tileX, sTileY).Active && TileRefs(tileX, sTileY).Type == 53)
						{
							int tileY = sTileY;
							while (!TileRefs(tileX, tileY + 1).Active && tileY < Main.maxTilesY - 5)
							{
								TileRefs(tileX, tileY + 1).SetActive(true);
								TileRefs(tileX, tileY + 1).SetType(53);
								tileY++;
							}
						}
					}
				}
			}
		}

		public static void CleanUpDirtBackgrounds(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			using (var prog = new ProgressLogger(Main.maxTilesX - 7, Languages.Generation_CleaningUpDirtBackgrounds))
			{
				for (int tileX = 3; tileX < Main.maxTilesX - 3; tileX++)
				{
					prog.Value = tileX - 3;

					bool flag10 = true;
					int tileY = 0;
					while ((double)tileY < Main.worldSurface)
					{
						if (flag10)
						{
							if (TileRefs(tileX, tileY).Wall == 2)
							{
								TileRefs(tileX, tileY).SetWall(0);
							}
							if (TileRefs(tileX, tileY).Type != 53)
							{
								if (TileRefs(tileX - 1, tileY).Wall == 2)
								{
									TileRefs(tileX - 1, tileY).SetWall(0);
								}
								if (TileRefs(tileX - 2, tileY).Wall == 2 && WorldModify.genRand.Next(2) == 0)
								{
									TileRefs(tileX - 2, tileY).SetWall(0);
								}
								if (TileRefs(tileX - 3, tileY).Wall == 2 && WorldModify.genRand.Next(2) == 0)
								{
									TileRefs(tileX - 3, tileY).SetWall(0);
								}
								if (TileRefs(tileX + 1, tileY).Wall == 2)
								{
									TileRefs(tileX + 1, tileY).SetWall(0);
								}
								if (TileRefs(tileX + 2, tileY).Wall == 2 && WorldModify.genRand.Next(2) == 0)
								{
									TileRefs(tileX + 2, tileY).SetWall(0);
								}
								if (TileRefs(tileX + 3, tileY).Wall == 2 && WorldModify.genRand.Next(2) == 0)
								{
									TileRefs(tileX + 3, tileY).SetWall(0);
								}
								if (TileRefs(tileX, tileY).Active)
								{
									flag10 = false;
								}
							}
						}
						else if (TileRefs(tileX, tileY).Wall == 0 && TileRefs(tileX, tileY + 1).Wall == 0 && TileRefs(tileX, tileY + 2).Wall == 0 && TileRefs(tileX, tileY + 3).Wall == 0 && TileRefs(tileX, tileY + 4).Wall == 0 && TileRefs(tileX - 1, tileY).Wall == 0 && TileRefs(tileX + 1, tileY).Wall == 0 && TileRefs(tileX - 2, tileY).Wall == 0 && TileRefs(tileX + 2, tileY).Wall == 0 && !TileRefs(tileX, tileY).Active && !TileRefs(tileX, tileY + 1).Active && !TileRefs(tileX, tileY + 2).Active && !TileRefs(tileX, tileY + 3).Active)
						{
							flag10 = true;
						}
						tileY++;
					}
				}
			} // end cleaning up
		}

		public static void PlaceAltars(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var MaxAltar = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05);
			using (var prog = new ProgressLogger(MaxAltar - 1, Languages.Generation_PlacingAltars))
			{
				for (int i = 0; i < MaxAltar; i++)
				{
					prog.Value = i;

					//bool flag11 = false;
					int maxAltar = 0;
					while (true)
					{
						int tileX = WorldModify.genRand.Next(1, Main.maxTilesX);
						int tileY = (int)(worldSurface + 20.0);
						WorldModify.Place3x2(TileRefs, tileX, tileY, 26);
						if (TileRefs(tileX, tileY).Type == 26)
						{
							//flag11 = true;
							break;
						}
						else
						{
							maxAltar++;
							if (maxAltar >= 10000)
							{
								//flag11 = true;
								break;
							}
						}
					}
				}
			}
		}

		public static void SettleLiquids(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var Max = Main.maxTilesX - 400;
			var Max2 = (Main.maxTilesX - 1) * 2;
			var Max3 = (int)((100 + Main.maxTilesX + Max + Max2 + (int)(Main.maxTilesX * 2.85)) * 1.65);
			var Max4 = (int)(Max3 * 0.573);
			using (var prog = new ProgressLogger(Max4 + Max3, Languages.Generation_SettlingLiquids))
			{
				for (int tilePosX = 0; tilePosX < Main.maxTilesX; tilePosX++)
				{
					int tileX = tilePosX;
					int tileY = (int)terrarainMaxY;
					while ((double)tileY < Main.worldSurface - 1.0)
					{
						if (TileRefs(tileX, tileY).Active)
						{
							if (TileRefs(tileX, tileY).Type == 60)
							{
								TileRefs(tileX, tileY - 1).SetLiquid(255);
								TileRefs(tileX, tileY - 2).SetLiquid(255);
								break;
							}
							break;
						}
						else
						{
							tileY++;
						}
					}
					prog.Value++;
				}
				for (int tilePosX = 400; tilePosX < Max; tilePosX++)
				{
					int tileX = tilePosX;
					int tileY = (int)terrarainMaxY;
					while ((double)tileY < Main.worldSurface - 1.0)
					{
						if (TileRefs(tileX, tileY).Active)
						{
							if (TileRefs(tileX, tileY).Type == 53)
							{
								int fTileY = tileY;
								while ((double)fTileY > terrarainMaxY)
								{
									fTileY--;
									TileRefs(tileX, fTileY).SetLiquid(0);
								}
								break;
							}
							break;
						}
						else
						{
							tileY++;
						}
					}
					prog.Value++;
				}

				Liquid.QuickWater(TileRefs, sandbox, 3, -1, -1, prog);
				WorldModify.WaterCheck(TileRefs, sandbox, prog);

				int liquidLeft = 0;
				Liquid.quickSettle = true;
				while (liquidLeft < 10)
				{
					int liquidAmount = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
					liquidLeft++;
					float preserver = 0f;
					while (Liquid.numLiquid > 0)
					{
						float liquidLeftO = (float)(liquidAmount - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)liquidAmount;
						if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > liquidAmount)
						{
							liquidAmount = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
						}
						if (liquidLeftO > preserver)
						{
							preserver = liquidLeftO;
						}
						else
						{
							liquidLeftO = preserver;
						}
						if (liquidLeft == 1)
						{
							prog.Value = (int)(liquidLeftO * 100f / 3f + 33f);
						}
						if (liquidLeft <= 10)
						{
							Liquid.UpdateLiquid(TileRefs, sandbox);
						}
					}
					WorldModify.WaterCheck(TileRefs, sandbox, prog);

					prog.Value = (int)((float)liquidLeft * 10f / 3f + 66f);
				}
				Liquid.quickSettle = false;
			} // end settling
		}

		public static void PlaceLifeCrystals(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var num218max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.2E-05);
			using (var prog = new ProgressLogger(num218max - 1, Languages.Generation_PlacingLifeCrystals))
			{
				for (int num218 = 0; num218 < num218max; num218++)
				{
					prog.Value = num218;
					bool flag12 = false;
					int num220 = 0;
					while (!flag12)
					{
						if (AddLifeCrystal(TileRefs, WorldModify.genRand.Next(1, Main.maxTilesX), WorldModify.genRand.Next((int)(worldSurface + 20.0), Main.maxTilesY)))
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
			}
		}

		public static void HideTreasure(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var num221max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 1.7E-05);
			using (var prog = new ProgressLogger(num221max - 1, Languages.Generation_HidingTreasure))
			{
				for (int num221 = 0; num221 < num221max; num221++)
				{
					prog.Value = num221;
					bool flag13 = false;
					int num223 = 0;
					while (!flag13)
					{
						int num224 = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
						int num225 = WorldModify.genRand.Next((int)(worldSurface + 20.0), Main.maxTilesY - 20);
						while (TileRefs(num224, num225).Wall > 0)
						{
							num224 = WorldModify.genRand.Next(1, Main.maxTilesX);
							num225 = WorldModify.genRand.Next((int)(worldSurface + 20.0), Main.maxTilesY - 20);
						}
						if (AddBuriedChest(TileRefs, num224, num225, 0, false))
						{
							flag13 = true;
							var num266 = num225;

							while (TileRefs(num224, num266).Type != 21 && num266 < Main.maxTilesY - 300)
							{
								num266++;
							}
							if (num225 < Main.maxTilesY - 300)
							{
								MineHouse(TileRefs, num224, num266);
							}
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
			}
		}

		public static void HideMoreTreasure(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var num226max = (int)((double)Main.maxTilesX * 0.005);
			using (var prog = new ProgressLogger(num226max - 1, Languages.Generation_HidingMoreTreasure))
			{
				for (int num226 = 0; num226 < num226max; num226++)
				{
					prog.Value = num226;
					bool flag14 = false;
					int num228 = 0;
					while (!flag14)
					{
						int num229 = WorldModify.genRand.Next(300, Main.maxTilesX - 300);
						int num230 = WorldModify.genRand.Next((int)terrarainMaxY, (int)Main.worldSurface);
						bool flag15 = false;
						if (TileRefs(num229, num230).Wall == 2 && !TileRefs(num229, num230).Active)
						{
							flag15 = true;
						}
						if (flag15 && AddBuriedChest(TileRefs, num229, num230, 0, true))
						{
							flag14 = true;
						}
						else
						{
							num228++;
							if (num228 >= 2000)
							{
								flag14 = true;
							}
						}
					}
				}
			}
		}

		public static void HideJungleTreasure(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var num238max = numJChests;
			using (var prog = new ProgressLogger(num238max - 1, Languages.Generation_HidingJungleTreasure))
			{
				int num231 = 0;
				for (int num232 = 0; num232 < num238max; num232++)
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
					if (!AddBuriedChest(TileRefs, JChestX[num232] + WorldModify.genRand.Next(2), JChestY[num232], contain, false))
					{
						for (int num233 = JChestX[num232]; num233 <= JChestX[num232] + 1; num233++)
						{
							for (int num234 = JChestY[num232]; num234 <= JChestY[num232] + 1; num234++)
							{
								WorldModify.KillTile(TileRefs, null, num233, num234);
							}
						}
						AddBuriedChest(TileRefs, JChestX[num232], JChestY[num232], contain, false);
					}
				}
			}
		}

		public static void HideWaterTreasure(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			//Water Treasure
			float num235 = (float)(Main.maxTilesX / 4200);
			int num236 = 0;
			int num237 = 0;
			using (var prog = new ProgressLogger(100, Languages.Generation_HidingWaterTreasure))
			{
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
						while (TileRefs(num238, num239).Liquid < 200 || TileRefs(num238, num239).Lava)
						{
							num238 = WorldModify.genRand.Next(1, Main.maxTilesX);
							num239 = WorldModify.genRand.Next(1, Main.maxTilesY - 200);
						}
						flag16 = AddBuriedChest(TileRefs, num238, num239, contain2, true);
					}
					num237++;
				}
			}
		}

		public static void AddIslandHouses(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (numIslandHouses > 0)
			{
				ProgramLog.Log(Languages.Generation_AddingIslandHouses);
				for (int num240 = 0; num240 < numIslandHouses; num240++)
				{
					IslandHouse(TileRefs, null, fihX[num240], fihY[num240]);
				}
			}
		}

		public static void PlaceBreakables(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var num241max = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008);
			using (var prog = new ProgressLogger(num241max - 1, Languages.Generation_PlacingBreakables))
			{
				for (int num241 = 0; num241 < num241max; num241++)
				{
					float num242 = (float)((double)num241 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008));

					prog.Value = num241;

					bool flag17 = false;
					int num243 = 0;
					while (!flag17)
					{
						int num244 = WorldModify.genRand.Next((int)worldSurface, Main.maxTilesY - 10);
						if ((double)num242 > 0.93)
						{
							num244 = Main.maxTilesY - 150;
						}
						else if ((double)num242 > 0.75)
						{
							num244 = (int)terrarainMaxY;
						}
						int num245 = WorldModify.genRand.Next(1, Main.maxTilesX);
						bool flag18 = false;
						for (int num246 = num244; num246 < Main.maxTilesY; num246++)
						{
							if (!flag18)
							{
								if (TileRefs(num245, num246).Active && Main.tileSolid[(int)TileRefs(num245, num246).Type] && !TileRefs(num245, num246 - 1).Lava)
								{
									flag18 = true;
								}
							}
							else
							{
								if (WorldModify.PlacePot(TileRefs, num245, num246, 28))
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
		}

		public static void PlaceHellforges(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var max = Main.maxTilesX / 200;
			using (var prog = new ProgressLogger(max - 1, Languages.Generation_PlacingHellForges))
			{
				for (int num294 = 0; num294 < Main.maxTilesX / 200; num294++)
				{
					float num295 = (float)(num294 / (Main.maxTilesX / 200));
					bool flag21 = false;
					int num296 = 0;
					while (!flag21)
					{
						int num297 = WorldModify.genRand.Next(1, Main.maxTilesX);
						int num298 = WorldModify.genRand.Next(Main.maxTilesY - 250, Main.maxTilesY - 5);
						try
						{
							if (TileRefs(num297, num298).Wall != 13 && TileRefs(num297, num298).Wall != 14)
								continue;

							while (!TileRefs(num297, num298).Active)
								num298++;

							num298--;
							WorldModify.PlaceTile(TileRefs, null, num297, num298, 77, false, false, -1, 0);
							if (TileRefs(num297, num298).Type == 77)
								flag21 = true;
							else
							{
								num296++;
								if (num296 >= 10000)
									flag21 = true;
							}
						}
						catch
						{ }
					}
				}
				prog.Value++;
			}
		}

		public static void SpreadGrass(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			ProgramLog.Log(Languages.Generation_SpreadingGrass);
			for (int num252 = 0; num252 < Main.maxTilesX; num252++)
			{
				int num48 = num252;
				bool flag20 = true;
				int num253 = 0;
				while ((double)num253 < Main.worldSurface - 1.0)
				{
					if (TileRefs(num48, num253).Active)
					{
						if (flag20 && TileRefs(num48, num253).Type == 0)
						{
							WorldModify.SpreadGrass(TileRefs, num48, num253, 0, 2, true);
						}
						if ((double)num253 > worldSurface)
						{
							break;
						}
						flag20 = false;
					}
					else if (TileRefs(num48, num253).Wall == 0)
					{
						flag20 = true;
					}
					num253++;
				}
			}
		}

		public static void GrowCacti(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			ProgramLog.Log(Languages.Generation_GrowingCacti);
			for (int num254 = 5; num254 < Main.maxTilesX - 5; num254++)
			{
				if (WorldModify.genRand.Next(8) == 0)
				{
					int num255 = 0;
					while ((double)num255 < Main.worldSurface - 1.0)
					{
						if (TileRefs(num254, num255).Active && TileRefs(num254, num255).Type == 53 && !TileRefs(num254, num255 - 1).Active && TileRefs(num254, num255 - 1).Wall == 0)
						{
							if (num254 < 250 || num254 > Main.maxTilesX - 250)
							{
								if (TileRefs(num254, num255 - 2).Liquid == 255 && TileRefs(num254, num255 - 3).Liquid == 255 && TileRefs(num254, num255 - 4).Liquid == 255)
								{
									WorldModify.PlaceTile(TileRefs, null, num254, num255 - 1, 81, true, false, -1, 0);
								}
							}
							else if (num254 > 400 && num254 < Main.maxTilesX - 400)
							{
								WorldModify.PlantCactus(TileRefs, null, num254, num255);
							}
						}
						num255++;
					}
				}
			} // end cacti
		}

		public static void PlantSunflowers(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			ProgramLog.Log(Languages.Generation_PlantingSunflowers);
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
						if (TileRefs(num267, num268).Type == 2 && TileRefs(num267, num268).Active && !TileRefs(num267, num268 - 1).Active)
						{
							WorldModify.PlaceTile(TileRefs, null, num267, num268 - 1, 27, true, false, -1, 0);
						}
						if (TileRefs(num267, num268).Active)
						{
							break;
						}
						num268++;
					}
				}
				num263++;
			} // end sunflowers
		}

		public static void PlantTrees(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			ProgramLog.Log(Languages.Generation_PlantingTrees);
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
						GrowEpicTree(TileRefs, null, num272, num273);
						num273++;
					}
				}
				num269++;
			}

			AddTrees(TileRefs);
		}

		public static void PlantHerbs(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			ProgramLog.Log(Languages.Generation_PlantingHerbs);
			for (int num274 = 0; num274 < Main.maxTilesX * 2; num274++)
			{
				WorldModify.PlantAlch(TileRefs);
			}
		}

		public static void PlantWeeds(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			ProgramLog.Log(Languages.Generation_PlantingWeeds);
			AddPlants(TileRefs);
			for (int num275 = 0; num275 < Main.maxTilesX; num275++)
			{
				for (int num276 = 0; num276 < Main.maxTilesY; num276++)
				{
					if (TileRefs(num275, num276).Active)
					{
						if (num276 >= (int)Main.worldSurface && TileRefs(num275, num276).Type == 70 && !TileRefs(num275, num276 - 1).Active)
						{
							WorldModify.GrowShroom(TileRefs, num275, num276);
							if (!TileRefs(num275, num276 - 1).Active)
							{
								WorldModify.PlaceTile(TileRefs, null, num275, num276 - 1, 71, true, false, -1, 0);
							}
						}
						if (TileRefs(num275, num276).Type == 60 && !TileRefs(num275, num276 - 1).Active)
						{
							WorldModify.PlaceTile(TileRefs, null, num275, num276 - 1, 61, true, false, -1, 0);
						}
					}
				}
			}
		}

		public static void GrowVines(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			ProgramLog.Log(Languages.Generation_GrowingVines);
			for (int num277 = 0; num277 < Main.maxTilesX; num277++)
			{
				int num278 = 0;
				int num279 = 0;
				while ((double)num279 < Main.worldSurface)
				{
					if (num278 > 0 && !TileRefs(num277, num279).Active)
					{
						TileRefs(num277, num279).SetActive(true);
						TileRefs(num277, num279).SetType(52);
						num278--;
					}
					else
					{
						num278 = 0;
					}
					if (TileRefs(num277, num279).Active && TileRefs(num277, num279).Type == 2 && WorldModify.genRand.Next(5) < 3)
					{
						num278 = WorldModify.genRand.Next(1, 10);
					}
					num279++;
				}
				num278 = 0;
				for (int num280 = 0; num280 < Main.maxTilesY; num280++)
				{
					if (num278 > 0 && !TileRefs(num277, num280).Active)
					{
						TileRefs(num277, num280).SetActive(true);
						TileRefs(num277, num280).SetType(62);
						num278--;
					}
					else
					{
						num278 = 0;
					}
					if (TileRefs(num277, num280).Active && TileRefs(num277, num280).Type == 60 && WorldModify.genRand.Next(5) < 3)
					{
						num278 = WorldModify.genRand.Next(1, 10);
					}
				}
			}
		}

		public static void PlantFlowers(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			ProgramLog.Log(Languages.Generation_PlantingFlowers);
			int num281 = 0;
			while ((double)num281 < (double)Main.maxTilesX * 0.005)
			{
				int num282 = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
				int num283 = WorldModify.genRand.Next(5, 15);
				int num284 = WorldModify.genRand.Next(15, 30);
				int num285 = 1;
				while ((double)num285 < Main.worldSurface - 1.0)
				{
					if (TileRefs(num282, num285).Active)
					{
						for (int num286 = num282 - num283; num286 < num282 + num283; num286++)
						{
							for (int num287 = num285 - num284; num287 < num285 + num284; num287++)
							{
								if (TileRefs(num286, num287).Type == 3 || TileRefs(num286, num287).Type == 24)
								{
									TileRefs(num286, num287).SetFrameX((short)(WorldModify.genRand.Next(6, 8) * 18));
								}
							}
						}
						break;
					}
					num285++;
				}
				num281++;
			}
		}

		public static void PlantMushrooms(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			ProgramLog.Log(Languages.Generation_PlantingMushrooms);
			int num288 = 0;
			while ((double)num288 < (double)Main.maxTilesX * 0.002)
			{
				int num289 = WorldModify.genRand.Next(20, Main.maxTilesX - 20);
				int num290 = WorldModify.genRand.Next(4, 10);
				int num291 = WorldModify.genRand.Next(15, 30);
				int num292 = 1;
				while ((double)num292 < Main.worldSurface - 1.0)
				{
					if (TileRefs(num289, num292).Active)
					{
						for (int num293 = num289 - num290; num293 < num289 + num290; num293++)
						{
							for (int num294 = num292 - num291; num294 < num292 + num291; num294++)
							{
								if (TileRefs(num293, num294).Type == 3 || TileRefs(num293, num294).Type == 24)
								{
									TileRefs(num293, num294).SetFrameX(144);
								}
							}
						}
						break;
					}
					num292++;
				}
				num288++;
			}
		}

		public static bool AddBuriedChest(Func<Int32, Int32, ITile> TileRefs, int i, int j, int contain = 0, bool notNearOtherChests = false, int Style = -1)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (WorldModify.genRand == null)
			{
				WorldModify.genRand = new Random((int)DateTime.Now.Ticks);
			}
			int k = j;
			while (k < Main.maxTilesY)
			{
				if (TileRefs(i, k).Active && Main.tileSolid[(int)TileRefs(i, k).Type])
				{
					bool flag = false;
					int num = k;
					int style = 0;
					if ((double)num >= Main.worldSurface + 25.0 || contain > 0)
					{
						style = 1;
					}
					if (Style >= 0)
					{
						style = Style;
					}
					if (num > Main.maxTilesY - 205 && contain == 0)
					{
						if (hellChest == 0)
						{
							contain = 274;
							style = 4;
							flag = true;
						}
						else
						{
							if (hellChest == 1)
							{
								contain = 220;
								style = 4;
								flag = true;
							}
							else
							{
								if (hellChest == 2)
								{
									contain = 112;
									style = 4;
									flag = true;
								}
								else
								{
									if (hellChest == 3)
									{
										contain = 218;
										style = 4;
										flag = true;
										hellChest = 0;
									}
								}
							}
						}
					}
					int num2 = WorldModify.PlaceChest(TileRefs, i - 1, num - 1, 21, notNearOtherChests, style);
					if (num2 >= 0)
					{
						if (flag)
						{
							hellChest++;
						}
						int num3 = 0;
						while (num3 == 0)
						{
							if ((double)num < Main.worldSurface + 25.0)
							{
								if (contain > 0)
								{
									Main.chest[num2].contents[num3] = Registries.Item.Create(contain);
									Main.chest[num2].contents[num3].SetPrefix(-1);
									num3++;
								}
								else
								{
									int num4 = WorldModify.genRand.Next(6);
									if (num4 == 0)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(280);
										Main.chest[num2].contents[num3].SetPrefix(-1);
									}
									if (num4 == 1)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(281);
										Main.chest[num2].contents[num3].SetPrefix(-1);
									}
									if (num4 == 2)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(284);
										Main.chest[num2].contents[num3].SetPrefix(-1);
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
										Main.chest[num2].contents[num3].SetPrefix(-1);
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
							else
							{
								if ((double)num < Main.rockLayer)
								{
									if (contain > 0)
									{
										Main.chest[num2].contents[num3] = Registries.Item.Create(contain);
										Main.chest[num2].contents[num3].SetPrefix(-1);
										num3++;
									}
									else
									{
										int num10 = WorldModify.genRand.Next(7);
										if (num10 == 0)
										{
											Main.chest[num2].contents[num3] = Registries.Item.Create(49);
											Main.chest[num2].contents[num3].SetPrefix(-1);
										}
										if (num10 == 1)
										{
											Main.chest[num2].contents[num3] = Registries.Item.Create(50);
											Main.chest[num2].contents[num3].SetPrefix(-1);
										}
										if (num10 == 2)
										{
											Main.chest[num2].contents[num3] = Registries.Item.Create(52);
										}
										if (num10 == 3)
										{
											Main.chest[num2].contents[num3] = Registries.Item.Create(53);
											Main.chest[num2].contents[num3].SetPrefix(-1);
										}
										if (num10 == 4)
										{
											Main.chest[num2].contents[num3] = Registries.Item.Create(54);
											Main.chest[num2].contents[num3].SetPrefix(-1);
										}
										if (num10 == 5)
										{
											Main.chest[num2].contents[num3] = Registries.Item.Create(55);
											Main.chest[num2].contents[num3].SetPrefix(-1);
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
								else
								{
									if (num < Main.maxTilesY - 250)
									{
										if (contain > 0)
										{
											Main.chest[num2].contents[num3] = Registries.Item.Create(contain);
											Main.chest[num2].contents[num3].SetPrefix(-1);
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
												Main.chest[num2].contents[num3].SetPrefix(-1);
											}
											if (num15 == 1)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(50);
												Main.chest[num2].contents[num3].SetPrefix(-1);
											}
											if (num15 == 2)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(52);
												Main.chest[num2].contents[num3].SetPrefix(-1);
											}
											if (num15 == 3)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(53);
												Main.chest[num2].contents[num3].SetPrefix(-1);
											}
											if (num15 == 4)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(54);
												Main.chest[num2].contents[num3].SetPrefix(-1);
											}
											if (num15 == 5)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(55);
												Main.chest[num2].contents[num3].SetPrefix(-1);
											}
											if (num15 == 6)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(51);
												Main.chest[num2].contents[num3].Stack = WorldModify.genRand.Next(26) + 25;
											}
											num3++;
										}
										if (WorldModify.genRand.Next(5) == 0)
										{
											Main.chest[num2].contents[num3] = Registries.Item.Create(43);
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
											Main.chest[num2].contents[num3].SetPrefix(-1);
											num3++;
										}
										else
										{
											int num22 = WorldModify.genRand.Next(4);
											if (num22 == 0)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(49);
												Main.chest[num2].contents[num3].SetPrefix(-1);
											}
											if (num22 == 1)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(50);
												Main.chest[num2].contents[num3].SetPrefix(-1);
											}
											if (num22 == 2)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(53);
												Main.chest[num2].contents[num3].SetPrefix(-1);
											}
											if (num22 == 3)
											{
												Main.chest[num2].contents[num3] = Registries.Item.Create(54);
												Main.chest[num2].contents[num3].SetPrefix(-1);
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

		public static void MakeDungeon(Func<Int32, Int32, ITile> TileRefs, int x, int y, int tileType = 41, int wallType = 7)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			using (var prog = new ProgressLogger(100, Languages.Generation_CreatingDungeons))
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
				DungeonRoom(TileRefs, dungeonX, dungeonY, tileType, wallType);
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
							DungeonHalls(TileRefs, dungeonX, dungeonY, tileType, wallType, false);
							if (WorldModify.genRand.Next(2) == 0)
							{
								DungeonHalls(TileRefs, dungeonX, dungeonY, tileType, wallType, false);
							}
							DungeonRoom(TileRefs, dungeonX, dungeonY, tileType, wallType);
							dungeonX = num6;
							dungeonY = num7;
						}
						else
						{
							DungeonRoom(TileRefs, dungeonX, dungeonY, tileType, wallType);
						}
					}
					else
					{
						DungeonHalls(TileRefs, dungeonX, dungeonY, tileType, wallType, false);
					}
				}
				DungeonRoom(TileRefs, dungeonX, dungeonY, tileType, wallType);
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
						DungeonHalls(TileRefs, dungeonX, dungeonY, tileType, wallType, true);
						DungeonRoom(TileRefs, dungeonX, dungeonY, tileType, wallType);
						dungeonX = num10;
						dungeonY = num11;
					}
					DungeonStairs(TileRefs, dungeonX, dungeonY, tileType, wallType);
				}
				DungeonEnt(TileRefs, null, dungeonX, dungeonY, tileType, wallType);

				prog.Value = 65;

				for (int j = 0; j < numDRooms; j++)
				{
					for (int k = dRoomL[j]; k <= dRoomR[j]; k++)
					{
						if (!TileRefs(k, dRoomT[j] - 1).Active)
						{
							DPlatX[numDPlats] = k;
							DPlatY[numDPlats] = dRoomT[j] - 1;
							numDPlats++;
							break;
						}
					}
					for (int l = dRoomL[j]; l <= dRoomR[j]; l++)
					{
						if (!TileRefs(l, dRoomB[j] + 1).Active)
						{
							DPlatX[numDPlats] = l;
							DPlatY[numDPlats] = dRoomB[j] + 1;
							numDPlats++;
							break;
						}
					}
					for (int m = dRoomT[j]; m <= dRoomB[j]; m++)
					{
						if (!TileRefs(dRoomL[j] - 1, m).Active)
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
						if (!TileRefs(dRoomR[j] + 1, n).Active)
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
					if ((int)TileRefs(num15, num16).Wall == wallType && !TileRefs(num15, num16).Active)
					{
						int num18 = 1;
						if (WorldModify.genRand.Next(2) == 0)
						{
							num18 = -1;
						}
						while (!TileRefs(num15, num16).Active)
						{
							num16 += num18;
						}
						if (TileRefs(num15 - 1, num16).Active && TileRefs(num15 + 1, num16).Active && !TileRefs(num15 - 1, num16 - num18).Active && !TileRefs(num15 + 1, num16 - num18).Active)
						{
							num14++;
							int num19 = WorldModify.genRand.Next(5, 13);
							while (TileRefs(num15 - 1, num16).Active && TileRefs(num15, num16 + num18).Active && TileRefs(num15, num16).Active && !TileRefs(num15, num16 - num18).Active && num19 > 0)
							{
								TileRefs(num15, num16).SetType(48);
								if (!TileRefs(num15 - 1, num16 - num18).Active && !TileRefs(num15 + 1, num16 - num18).Active)
								{
									TileRefs(num15, num16 - num18).SetType(48);
									TileRefs(num15, num16 - num18).SetActive(true);
								}
								num15--;
								num19--;
							}
							num19 = WorldModify.genRand.Next(5, 13);
							num15 = num17 + 1;
							while (TileRefs(num15 + 1, num16).Active && TileRefs(num15, num16 + num18).Active && TileRefs(num15, num16).Active && !TileRefs(num15, num16 - num18).Active && num19 > 0)
							{
								TileRefs(num15, num16).SetType(48);
								if (!TileRefs(num15 - 1, num16 - num18).Active && !TileRefs(num15 + 1, num16 - num18).Active)
								{
									TileRefs(num15, num16 - num18).SetType(48);
									TileRefs(num15, num16 - num18).SetActive(true);
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
					if ((int)TileRefs(num20, num21).Wall == wallType && !TileRefs(num20, num21).Active)
					{
						int num23 = 1;
						if (WorldModify.genRand.Next(2) == 0)
						{
							num23 = -1;
						}
						while (num20 > 5 && num20 < Main.maxTilesX - 5 && !TileRefs(num20, num21).Active)
						{
							num20 += num23;
						}
						if (TileRefs(num20, num21 - 1).Active && TileRefs(num20, num21 + 1).Active && !TileRefs(num20 - num23, num21 - 1).Active && !TileRefs(num20 - num23, num21 + 1).Active)
						{
							num14++;
							int num24 = WorldModify.genRand.Next(5, 13);
							while (TileRefs(num20, num21 - 1).Active && TileRefs(num20 + num23, num21).Active && TileRefs(num20, num21).Active && !TileRefs(num20 - num23, num21).Active && num24 > 0)
							{
								TileRefs(num20, num21).SetType(48);
								if (!TileRefs(num20 - num23, num21 - 1).Active && !TileRefs(num20 - num23, num21 + 1).Active)
								{
									TileRefs(num20 - num23, num21).SetType(48);
									TileRefs(num20 - num23, num21).SetActive(true);
								}
								num21--;
								num24--;
							}
							num24 = WorldModify.genRand.Next(5, 13);
							num21 = num22 + 1;
							while (TileRefs(num20, num21 + 1).Active && TileRefs(num20 + num23, num21).Active && TileRefs(num20, num21).Active && !TileRefs(num20 - num23, num21).Active && num24 > 0)
							{
								TileRefs(num20, num21).SetType(48);
								if (!TileRefs(num20 - num23, num21 - 1).Active && !TileRefs(num20 - num23, num21 + 1).Active)
								{
									TileRefs(num20 - num23, num21).SetType(48);
									TileRefs(num20 - num23, num21).SetActive(true);
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
						while (!TileRefs(num30, num31).Active)
						{
							num31--;
						}
						if (!Main.tileDungeon[(int)TileRefs(num30, num31).Type])
						{
							flag = false;
						}
						int num32 = num31;
						num31 = DDoorY[num25];
						while (!TileRefs(num30, num31).Active)
						{
							num31++;
						}
						if (!Main.tileDungeon[(int)TileRefs(num30, num31).Type])
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
									if (TileRefs(num38, num39).Active && TileRefs(num38, num39).Type == 10)
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
										if (TileRefs(num41, num40).Active)
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
						while (!TileRefs(num42, num43).Active)
						{
							TileRefs(num42, num43).SetActive(false);
							num43++;
						}
						while (!TileRefs(num42, num44).Active)
						{
							num44--;
						}
						num43--;
						num44++;
						for (int num45 = num44; num45 < num43 - 2; num45++)
						{
							TileRefs(num42, num45).SetActive(true);
							TileRefs(num42, num45).SetType((byte)tileType);
						}
						WorldModify.PlaceTile(TileRefs, null, num42, num43, 10, true, false, -1, 0);
						num42--;
						int num46 = num43 - 3;
						while (!TileRefs(num42, num46).Active)
						{
							num46--;
						}
						if (num43 - num46 < num43 - num44 + 5 && Main.tileDungeon[(int)TileRefs(num42, num46).Type])
						{
							for (int num47 = num43 - 4 - WorldModify.genRand.Next(3); num47 > num46; num47--)
							{
								TileRefs(num42, num47).SetActive(true);
								TileRefs(num42, num47).SetType((byte)tileType);
							}
						}
						num42 += 2;
						num46 = num43 - 3;
						while (!TileRefs(num42, num46).Active)
						{
							num46--;
						}
						if (num43 - num46 < num43 - num44 + 5 && Main.tileDungeon[(int)TileRefs(num42, num46).Type])
						{
							for (int num48 = num43 - 4 - WorldModify.genRand.Next(3); num48 > num46; num48--)
							{
								TileRefs(num42, num48).SetActive(true);
								TileRefs(num42, num48).SetType((byte)tileType);
							}
						}
						num43++;
						num42--;
						TileRefs(num42 - 1, num43).SetActive(true);
						TileRefs(num42 - 1, num43).SetType((byte)tileType);
						TileRefs(num42 + 1, num43).SetActive(true);
						TileRefs(num42 + 1, num43).SetType((byte)tileType);
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
						if (TileRefs(num55, num54).Active)
						{
							flag3 = true;
						}
						else
						{
							while (!TileRefs(num55, num54).Active)
							{
								num55--;
								if (!Main.tileDungeon[(int)TileRefs(num55, num54).Type])
								{
									flag3 = true;
								}
							}
							while (!TileRefs(num56, num54).Active)
							{
								num56++;
								if (!Main.tileDungeon[(int)TileRefs(num56, num54).Type])
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
									if (TileRefs(num61, num62).Active && TileRefs(num61, num62).Type == 19)
									{
										flag4 = false;
										break;
									}
								}
							}
							for (int num63 = num54 + 3; num63 >= num54 - 5; num63--)
							{
								if (TileRefs(num50, num63).Active)
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
						while (!TileRefs(num64, num65).Active)
						{
							TileRefs(num64, num65).SetActive(true);
							TileRefs(num64, num65).SetType(19);
							num64--;
						}
						while (!TileRefs(num66, num65).Active)
						{
							TileRefs(num66, num65).SetActive(true);
							TileRefs(num66, num65).SetType(19);
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
					if ((int)TileRefs(num67, num68).Wall == wallType && !TileRefs(num67, num68).Active)
					{
						int num69 = 1;
						if (WorldModify.genRand.Next(2) == 0)
						{
							num69 = -1;
						}
						while (flag5 && !TileRefs(num67, num68).Active)
						{
							num67 -= num69;
							if (num67 < 5 || num67 > Main.maxTilesX - 5)
							{
								flag5 = false;
							}
							else if (TileRefs(num67, num68).Active && !Main.tileDungeon[(int)TileRefs(num67, num68).Type])
							{
								flag5 = false;
							}
						}
						if (flag5 && TileRefs(num67, num68).Active && Main.tileDungeon[(int)TileRefs(num67, num68).Type] && TileRefs(num67, num68 - 1).Active && Main.tileDungeon[(int)TileRefs(num67, num68 - 1).Type] && TileRefs(num67, num68 + 1).Active && Main.tileDungeon[(int)TileRefs(num67, num68 + 1).Type])
						{
							num67 += num69;
							for (int num70 = num67 - 3; num70 <= num67 + 3; num70++)
							{
								for (int num71 = num68 - 3; num71 <= num68 + 3; num71++)
								{
									if (TileRefs(num70, num71).Active && TileRefs(num70, num71).Type == 19)
									{
										flag5 = false;
										break;
									}
								}
							}
							if (flag5 && (!TileRefs(num67, num68 - 1).Active & !TileRefs(num67, num68 - 2).Active & !TileRefs(num67, num68 - 3).Active))
							{
								int num72 = num67;
								int num73 = num67;
								while (num72 > dMinX && num72 < dMaxX && !TileRefs(num72, num68).Active && !TileRefs(num72, num68 - 1).Active && !TileRefs(num72, num68 + 1).Active)
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
										TileRefs(num67, num68).SetActive(true);
										TileRefs(num67, num68).SetType(19);
										if (flag6)
										{
											WorldModify.PlaceTile(TileRefs, null, num67, num68 - 1, 50, true, false, -1, 0);
											if (WorldModify.genRand.Next(50) == 0 && TileRefs(num67, num68 - 1).Type == 50)
											{
												TileRefs(num67, num68 - 1).SetFrameX(90);
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
										WorldModify.PlaceTile(TileRefs, null, num67, num68, num75, true, false, -1, 0);
										if (TileRefs(num67, num68).Type == 13)
										{
											if (WorldModify.genRand.Next(2) == 0)
											{
												TileRefs(num67, num68).SetFrameX(18);
											}
											else
											{
												TileRefs(num67, num68).SetFrameX(36);
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

				int num76 = 0;
				for (int num77 = 0; num77 < numDRooms; num77++)
				{
					int num78 = 0;
					while (num78 < 1000)
					{
						int num79 = (int)((double)dRoomSize[num77] * 0.4);
						int i2 = dRoomX[num77] + WorldModify.genRand.Next(-num79, num79 + 1);
						int num80 = dRoomY[num77] + WorldModify.genRand.Next(-num79, num79 + 1);
						int num81 = 0;
						num76++;
						int style = 2;
						switch (num76)
						{
							case 1:
								num81 = 329;
								break;
							case 2:
								num81 = 155;
								break;
							case 3:
								num81 = 156;
								break;
							case 4:
								num81 = 157;
								break;
							case 5:
								num81 = 163;
								break;
							case 6:
								num81 = 113;
								break;
							case 7:
								num81 = 327;
								style = 0;
								break;
							default:
								num81 = 164;
								num76 = 0;
								break;
						}
						if ((double)num80 < Main.worldSurface + 50.0)
						{
							num81 = 327;
							style = 0;
						}
						if (num81 == 0 && WorldModify.genRand.Next(2) == 0)
						{
							num78 = 1000;
						}
						else
						{
							if (AddBuriedChest(TileRefs, i2, num80, num81, false, style))
							{
								num78 += 1000;
							}
							num78++;
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
				while (num14 < Main.maxTilesX / 150)
				{
					num12++;
					int num81 = WorldModify.genRand.Next(dMinX, dMaxX);
					int num82 = WorldModify.genRand.Next(dMinY, dMaxY);
					if ((int)TileRefs(num81, num82).Wall == wallType)
					{
						int num83 = num82;
						while (num83 > dMinY)
						{
							if (TileRefs(num81, num83 - 1).Active && (int)TileRefs(num81, num83 - 1).Type == tileType)
							{
								bool flag7 = false;
								for (int num84 = num81 - 15; num84 < num81 + 15; num84++)
								{
									for (int num85 = num83 - 15; num85 < num83 + 15; num85++)
									{
										if (num84 > 0 && num84 < Main.maxTilesX && num85 > 0 && num85 < Main.maxTilesY && TileRefs(num84, num85).Type == 42)
										{
											flag7 = true;
											break;
										}
									}
								}
								if (TileRefs(num81 - 1, num83).Active || TileRefs(num81 + 1, num83).Active || TileRefs(num81 - 1, num83 + 1).Active || TileRefs(num81 + 1, num83 + 1).Active || TileRefs(num81, num83 + 2).Active)
								{
									flag7 = true;
								}
								if (flag7)
								{
									break;
								}
								WorldModify.Place1x2Top(TileRefs, num81, num83, 42);
								if (TileRefs(num81, num83).Type == 42)
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
				num12 = 0;
				num13 = 1000;
				num14 = 0;
				while (num14 < Main.maxTilesX / 500)
				{
					num12++;
					int num90 = WorldModify.genRand.Next(dMinX, dMaxX);
					int num91 = WorldModify.genRand.Next(dMinY, dMaxY);

					if ((int)TileRefs(num90, num91).Wall == wallType && WorldModify.placeTrap(TileRefs, null, num90, num91, 0))
						num12 = num13;

					if (num12 > num13)
					{
						num14++;
						num12 = 0;
					}
				}
			}
		} // end MakeDungeon

		public static void DungeonStairs(Func<Int32, Int32, ITile> TileRefs, int i, int j, int tileType, int wallType)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
			if (i > Main.maxTilesX - 400)
			{
				num2 = -1;
			}
			else
			{
				if (i < 400)
				{
					num2 = 1;
				}
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
				if ((double)value2.Y < Main.worldSurface - 5.0 && TileRefs(num8, (int)((double)value2.Y - num - 6.0 + (double)num9)).Wall == 0 && TileRefs(num8, (int)((double)value2.Y - num - 7.0 + (double)num9)).Wall == 0 && TileRefs(num8, (int)((double)value2.Y - num - 8.0 + (double)num9)).Wall == 0)
				{
					dSurface = true;
					TileRunner(TileRefs, num8, (int)((double)value2.Y - num - 6.0 + (double)num9), (double)WorldModify.genRand.Next(25, 35), WorldModify.genRand.Next(10, 20), -1, false, 0f, -1f, false, true);
				}
				for (int l = num3; l < num4; l++)
				{
					for (int m = num5; m < num6; m++)
					{
						TileRefs(l, m).SetLiquid(0);
						if ((int)TileRefs(l, m).Wall != wallType)
						{
							TileRefs(l, m).SetWall(0);
							TileRefs(l, m).SetActive(true);
							TileRefs(l, m).SetType((byte)tileType);
						}
					}
				}
				for (int n = num3 + 1; n < num4 - 1; n++)
				{
					for (int num10 = num5 + 1; num10 < num6 - 1; num10++)
					{
						WorldModify.PlaceWall(TileRefs, n, num10, wallType, true);
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
						TileRefs(num12, num13).SetActive(false);
						WorldModify.PlaceWall(TileRefs, num12, num13, wallType, true);
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

		public static void DungeonHalls(Func<Int32, Int32, ITile> TileRefs, int i, int j, int tileType, int wallType, bool forceX = false)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
						TileRefs(l, m).SetLiquid(0);
						if (TileRefs(l, m).Wall == 0)
						{
							TileRefs(l, m).SetActive(true);
							TileRefs(l, m).SetType((byte)tileType);
						}
					}
				}
				for (int n = num3 + 1; n < num4 - 1; n++)
				{
					for (int num7 = num5 + 1; num7 < num6 - 1; num7++)
					{
						WorldModify.PlaceWall(TileRefs, n, num7, wallType, true);
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
						TileRefs(num9, num10).SetActive(false);
						TileRefs(num9, num10).SetWall((byte)wallType);
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

		public static void DungeonRoom(Func<Int32, Int32, ITile> TileRefs, int i, int j, int tileType, int wallType)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
						TileRefs(l, m).SetLiquid(0);
						if (TileRefs(l, m).Wall == 0)
						{
							TileRefs(l, m).SetActive(true);
							TileRefs(l, m).SetType((byte)tileType);
						}
					}
				}
				for (int n = num6 + 1; n < num7 - 1; n++)
				{
					for (int num10 = num8 + 1; num10 < num9 - 1; num10++)
					{
						WorldModify.PlaceWall(TileRefs, n, num10, wallType, true);
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
						TileRefs(num11, num12).SetActive(false);
						TileRefs(num11, num12).SetWall((byte)wallType);
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

		public static void DungeonEnt(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j, int tileType, int wallType)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int lMax = 60;
			for (int k = i - lMax; k < i + lMax; k++)
			{
				for (int l = j - lMax; l < j + lMax; l++)
				{
					TileRefs(k, l).SetLiquid(0);
					TileRefs(k, l).SetLava(false);
				}
			}
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
					TileRefs(k, l).SetLiquid(0);
					if ((int)TileRefs(k, l).Wall != wallType)
					{
						TileRefs(k, l).SetWall(0);
						if (k > num4 + 1 && k < num5 - 2 && l > num6 + 1 && l < num7 - 2)
						{
							WorldModify.PlaceWall(TileRefs, k, l, wallType, true);
						}
						TileRefs(k, l).SetActive(true);
						TileRefs(k, l).SetType((byte)tileType);
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
					if ((int)TileRefs(m, n).Wall != wallType)
					{
						TileRefs(m, n).SetActive(true);
						TileRefs(m, n).SetType((byte)tileType);
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
					if ((int)TileRefs(num12, num13).Wall != wallType)
					{
						TileRefs(num12, num13).SetActive(true);
						TileRefs(num12, num13).SetType((byte)tileType);
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
					if ((int)TileRefs(num17, num18).Wall != wallType)
					{
						TileRefs(num17, num18).SetActive(true);
						TileRefs(num17, num18).SetType((byte)tileType);
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
					WorldModify.PlaceWall(TileRefs, num19, num20, 2, true);
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
					WorldModify.PlaceWall(TileRefs, num21, num22, wallType, true);
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
					TileRefs(num23, num24).SetWall((byte)wallType);
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
					TileRefs(num25, num26).SetActive(false);
					TileRefs(num25, num26).SetWall((byte)wallType);
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
					if ((int)TileRefs(num27, num28).Wall != wallType)
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
							TileRefs(num27, num28).SetWall(0);
							TileRefs(num27, num28).SetActive(true);
							TileRefs(num27, num28).SetType((byte)tileType);
						}
					}
				}
			}
			for (int num29 = num4; num29 < num5; num29++)
			{
				for (int num30 = num7; num30 < num7 + 100; num30++)
				{
					WorldModify.PlaceWall(TileRefs, num29, num30, 2, true);
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
					if ((int)TileRefs(num31, num32).Wall != wallType)
					{
						TileRefs(num31, num32).SetActive(true);
						TileRefs(num31, num32).SetType((byte)tileType);
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
					if ((int)TileRefs(num33, num34).Wall != wallType)
					{
						TileRefs(num33, num34).SetActive(true);
						TileRefs(num33, num34).SetType((byte)tileType);
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
					if ((int)TileRefs(num35, num36).Wall != wallType)
					{
						TileRefs(num35, num36).SetActive(true);
						TileRefs(num35, num36).SetType((byte)tileType);
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
					TileRefs(num37, num38).SetWall(0);
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
					TileRefs(num39, num40).SetActive(false);
					TileRefs(num39, num40).SetWall(0);
				}
			}
			for (int num41 = num4; num41 < num5; num41++)
			{
				if (!TileRefs(num41, num7).Active)
				{
					TileRefs(num41, num7).SetActive(true);
					TileRefs(num41, num7).SetType(19);
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
						TileRefs(num44, num45).SetActive(true);
						TileRefs(num44, num45).SetType((byte)tileType);
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
						TileRefs(num47, num48).SetActive(true);
						TileRefs(num47, num48).SetType((byte)tileType);
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
					WorldModify.PlaceWall(TileRefs, num49, num50, wallType, true);
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
					TileRefs(num51, num52).SetActive(false);
				}
			}
			if (num3 < 0)
			{
				vector.X -= 1f;
			}
			WorldModify.PlaceTile(TileRefs, sandbox, (int)vector.X, (int)vector.Y + 1, 10, false, false, -1, 0);
		}

		public static void ChasmRunnerSideways(Func<Int32, Int32, ITile> TileRefs, int i, int j, int direction, int steps)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num2 * 0.5 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015) && TileRefs(k, l).Type != 31 && TileRefs(k, l).Type != 22)
						{
							TileRefs(k, l).SetActive(false);
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
						if ((double)(Math.Abs((float)m - value.X) + Math.Abs((float)n - value.Y)) < num2 * 1.1 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015) && TileRefs(m, n).Wall != 3)
						{
							if (TileRefs(m, n).Type != 25 && n > j + WorldModify.genRand.Next(3, 20))
							{
								TileRefs(m, n).SetActive(true);
							}
							TileRefs(m, n).SetActive(true);
							if (TileRefs(m, n).Type != 31 && TileRefs(m, n).Type != 22)
							{
								TileRefs(m, n).SetType(25);
							}
							if (TileRefs(m, n).Wall == 2)
							{
								TileRefs(m, n).SetWall(0);
							}
						}
					}
				}
				for (int num7 = num3; num7 < num4; num7++)
				{
					for (int num8 = num5; num8 < num6; num8++)
					{
						if ((double)(Math.Abs((float)num7 - value.X) + Math.Abs((float)num8 - value.Y)) < num2 * 1.1 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015) && TileRefs(num7, num8).Wall != 3)
						{
							if (TileRefs(num7, num8).Type != 31 && TileRefs(num7, num8).Type != 22)
							{
								TileRefs(num7, num8).SetType(25);
							}
							TileRefs(num7, num8).SetActive(true);
							WorldModify.PlaceWall(TileRefs, num7, num8, 3, true);
						}
					}
				}
			}
			if (WorldModify.genRand.Next(3) == 0)
			{
				int num9 = (int)value.X;
				int num10 = (int)value.Y;
				while (!TileRefs(num9, num10).Active)
				{
					num10++;
				}
				TileRunner(TileRefs, num9, num10, (double)WorldModify.genRand.Next(2, 6), WorldModify.genRand.Next(3, 7), 22, false, 0f, 0f, false, true);
			}
		}

		public static void ChasmRunner(Func<Int32, Int32, ITile> TileRefs, int i, int j, int steps, bool makeOrb = false)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
					ChasmRunnerSideways(TileRefs, (int)value.X, (int)value.Y, -1, WorldModify.genRand.Next(20, 40));
					ChasmRunnerSideways(TileRefs, (int)value.X, (int)value.Y, 1, WorldModify.genRand.Next(20, 40));
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
							if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num3 * 0.5 * (1.0 + (double)WorldModify.genRand.Next(-10, 11) * 0.015) && TileRefs(k, l).Type != 31 && TileRefs(k, l).Type != 22)
							{
								TileRefs(k, l).SetActive(false);
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
						AddShadowOrb(TileRefs, (int)value.X, (int)value.Y);
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
								WorldModify.Place3x2(TileRefs, num9, num10, 26);
								if (TileRefs(num9, num10).Type == 26)
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
							if (TileRefs(m, n).Type != 25 && n > j + WorldModify.genRand.Next(3, 20))
							{
								TileRefs(m, n).SetActive(true);
							}
							if (steps <= num2)
							{
								TileRefs(m, n).SetActive(true);
							}
							if (TileRefs(m, n).Type != 31)
							{
								TileRefs(m, n).SetType(25);
							}
							if (TileRefs(m, n).Wall == 2)
							{
								TileRefs(m, n).SetWall(0);
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
							if (TileRefs(num11, num12).Type != 31)
							{
								TileRefs(num11, num12).SetType(25);
							}
							if (steps <= num2)
							{
								TileRefs(num11, num12).SetActive(true);
							}
							if (num12 > j + WorldModify.genRand.Next(3, 20))
							{
								WorldModify.PlaceWall(TileRefs, num11, num12, 3, true);
							}
						}
					}
				}
			}
		}

		public static void JungleRunner(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
					if (TileRefs(num3, num4).Wall == 0 && !TileRefs(num3, num4).Active && TileRefs(num3, num4 - 3).Wall == 0 && !TileRefs(num3, num4 - 3).Active && TileRefs(num3, num4 - 1).Wall == 0 && !TileRefs(num3, num4 - 1).Active && TileRefs(num3, num4 - 4).Wall == 0 && !TileRefs(num3, num4 - 4).Active && TileRefs(num3, num4 - 2).Wall == 0 && !TileRefs(num3, num4 - 2).Active && TileRefs(num3, num4 - 5).Wall == 0 && !TileRefs(num3, num4 - 5).Active)
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
							WorldModify.KillTile(TileRefs, sandbox, k, l);
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
					TileRunner(TileRefs, (int)value.X, (int)value.Y, (double)WorldModify.genRand.Next(3, 20), WorldModify.genRand.Next(10, 100), -1, false, (float)num9, 0f, false, true);
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

		public static void TileRunner(Func<Int32, Int32, ITile> TileRefs, int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool noYChange = false, bool overRide = true)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
							if (mudWall && (double)l > Main.worldSurface && l < Main.maxTilesY - 210 - WorldModify.genRand.Next(3))
							{
								WorldModify.PlaceWall(TileRefs, k, l, 15, true);
							}
							if (type < 0)
							{
								if (type == -2 && TileRefs(k, l).Active && (l < WorldModify.waterLine || l > WorldModify.lavaLine))
								{
									TileRefs(k, l).SetLiquid(255);
									if (l > WorldModify.lavaLine)
									{
										TileRefs(k, l).SetLava(true);
									}
								}
								TileRefs(k, l).SetActive(false);
							}
							else
							{
								if ((overRide || !TileRefs(k, l).Active) && (type != 40 || TileRefs(k, l).Type != 53) && (!Main.tileStone[type] || TileRefs(k, l).Type == 1) && TileRefs(k, l).Type != 45 && TileRefs(k, l).Type != 147 && (TileRefs(k, l).Type != 1 || type != 59 || (double)l >= Main.worldSurface + (double)WorldModify.genRand.Next(-50, 50)))
								{
									if (TileRefs(k, l).Type != 53 || (double)l >= Main.worldSurface)
									{
										TileRefs(k, l).SetType((byte)type);
									}
									else if (type == 59)
									{
										TileRefs(k, l).SetType((byte)type);
									}
								}
								if (addTile)
								{
									TileRefs(k, l).SetActive(true);
									TileRefs(k, l).SetLiquid(0);
									TileRefs(k, l).SetLava(false);
								}
								if (noYChange && (double)l < Main.worldSurface && type != 59)
								{
									TileRefs(k, l).SetWall(2);
								}
								if (type == 59 && l > WorldModify.waterLine && TileRefs(k, l).Liquid > 0)
								{
									TileRefs(k, l).SetLava(false);
									TileRefs(k, l).SetLiquid(0);
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

		public static void MudWallRunner(Func<Int32, Int32, ITile> TileRefs, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			double num = (double)WorldModify.genRand.Next(5, 15);
			float num2 = (float)WorldModify.genRand.Next(5, 20);
			float num3 = num2;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldModify.genRand.Next(-10, 11) * 0.1f;
			while (num > 0.0 && num3 > 0f)
			{
				double num4 = num * (double)(num3 / num2);
				num3 -= 1f;
				int num5 = (int)((double)value.X - num4 * 0.5);
				int num6 = (int)((double)value.X + num4 * 0.5);
				int num7 = (int)((double)value.Y - num4 * 0.5);
				int num8 = (int)((double)value.Y + num4 * 0.5);
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
							TileRefs(k, l).SetWall(0);
						}
					}
				}
				value += value2;
				value2.X += (float)WorldModify.genRand.Next(-10, 11) * 0.05f;
				if (value2.X > 1f)
				{
					value2.X = 1f;
				}
				if (value2.X < -1f)
				{
					value2.X = -1f;
				}
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
		}

		public static void FloatingIsland(Func<Int32, Int32, ITile> TileRefs, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
								TileRefs(k, l).SetActive(true);
								if (TileRefs(k, l).Type == 59)
								{
									TileRefs(k, l).SetType(0);
								}
							}
						}
					}
				}
				TileRunner(TileRefs, WorldModify.genRand.Next(num3 + 10, num4 - 10), (int)((double)value.Y + num7 * 0.1 + 5.0), (double)WorldModify.genRand.Next(5, 10), WorldModify.genRand.Next(10, 15), 0, true, 0f, 2f, true, true);
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
								TileRefs(m, n).SetWall(2);
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

		public static void IslandHouse(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
				if (TileRefs((int)vector.X, k).Active)
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
					TileRefs(l, m).SetActive(true);
					TileRefs(l, m).SetType(type);
					TileRefs(l, m).SetWall(0);
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
					if (TileRefs(n, num8).Wall == 0)
					{
						TileRefs(n, num8).SetActive(false);
						TileRefs(n, num8).SetWall(wall);
					}
				}
			}
			int num9 = i + (num2 + 1) * num;
			int num10 = (int)vector.Y;
			for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
			{
				TileRefs(num11, num10).SetActive(false);
				TileRefs(num11, num10 - 1).SetActive(false);
				TileRefs(num11, num10 - 2).SetActive(false);
			}
			WorldModify.PlaceTile(TileRefs, sandbox, num9, num10, 10, true, false, -1, 0);
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
			AddBuriedChest(TileRefs, i, num10 - 3, contain, false, 2);
			houseCount++;
		}

		public static void Mountinater(Func<Int32, Int32, ITile> TileRefs, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
						if (num10 < num7 * 0.4 && !TileRefs(k, l).Active)
						{
							TileRefs(k, l).SetActive(true);
							TileRefs(k, l).SetType(0);
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

		public static void Lakinater(Func<Int32, Int32, ITile> TileRefs, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
							if (TileRefs(k, l).Active)
							{
								TileRefs(k, l).SetLiquid(255);
							}
							TileRefs(k, l).SetActive(false);
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

		public static void ShroomPatch(Func<Int32, Int32, ITile> TileRefs, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
								if (TileRefs(k, l).Type != 59)
								{
									TileRefs(k, l).SetActive(false);
								}
							}
							else
							{
								TileRefs(k, l).SetType(59);
							}
							TileRefs(k, l).SetLiquid(0);
							TileRefs(k, l).SetLava(false);
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

		public static void Cavinator(Func<Int32, Int32, ITile> TileRefs, int i, int j, int steps)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
							TileRefs(l, m).SetActive(false);
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
				Cavinator(TileRefs, (int)value.X, (int)value.Y, steps - 1);
			}
		}

		public static void CaveOpenater(Func<Int32, Int32, ITile> TileRefs, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

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
				if (TileRefs((int)value.X, (int)value.Y).Wall == 0)
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
							TileRefs(l, m).SetActive(false);
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

		public static void AddPlants(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 1; j < Main.maxTilesY; j++)
				{
					if (TileRefs(i, j).Type == 2 && TileRefs(i, j).Active)
					{
						if (!TileRefs(i, j - 1).Active)
						{
							WorldModify.PlaceTile(TileRefs, null, i, j - 1, 3, true, false, -1, 0);
						}
					}
					else if (TileRefs(i, j).Type == 23 && TileRefs(i, j).Active && !TileRefs(i, j - 1).Active)
					{
						WorldModify.PlaceTile(TileRefs, null, i, j - 1, 24, true, false, -1, 0);
					}
				}
			}
		}

		public static bool AddLifeCrystal(Func<Int32, Int32, ITile> TileRefs, int i, int j)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int k = j;
			while (k < Main.maxTilesY)
			{
				if (TileRefs(i, k).Active && Main.tileSolid[(int)TileRefs(i, k).Type])
				{
					int num = k - 1;
					if (TileRefs(i, num - 1).Lava || TileRefs(i - 1, num - 1).Lava)
					{
						return false;
					}
					if (!WorldModify.EmptyTileCheck(TileRefs, i - 1, i, num - 1, num, -1))
					{
						return false;
					}
					TileRefs(i - 1, num - 1).SetActive(true);
					TileRefs(i - 1, num - 1).SetType(12);
					TileRefs(i - 1, num - 1).SetFrameX(0);
					TileRefs(i - 1, num - 1).SetFrameY(0);
					TileRefs(i, num - 1).SetActive(true);
					TileRefs(i, num - 1).SetType(12);
					TileRefs(i, num - 1).SetFrameX(18);
					TileRefs(i, num - 1).SetFrameY(0);
					TileRefs(i - 1, num).SetActive(true);
					TileRefs(i - 1, num).SetType(12);
					TileRefs(i - 1, num).SetFrameX(0);
					TileRefs(i - 1, num).SetFrameY(18);
					TileRefs(i, num).SetActive(true);
					TileRefs(i, num).SetType(12);
					TileRefs(i, num).SetFrameX(18);
					TileRefs(i, num).SetFrameY(18);
					return true;
				}
				else
				{
					k++;
				}
			}
			return false;
		}

		public static void AddShadowOrb(Func<Int32, Int32, ITile> TileRefs, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (x < 10 || x > Main.maxTilesX - 10)
			{
				return;
			}
			if (y < 10 || y > Main.maxTilesY - 10)
			{
				return;
			}
			for (int i = x - 1; i < x + 1; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileRefs(i, j).Active && TileRefs(i, j).Type == 31)
					{
						return;
					}
				}
			}
			TileRefs(x - 1, y - 1).SetActive(true);
			TileRefs(x - 1, y - 1).SetType(31);
			TileRefs(x - 1, y - 1).SetFrameX(0);
			TileRefs(x - 1, y - 1).SetFrameY(0);
			TileRefs(x, y - 1).SetActive(true);
			TileRefs(x, y - 1).SetType(31);
			TileRefs(x, y - 1).SetFrameX(18);
			TileRefs(x, y - 1).SetFrameY(0);
			TileRefs(x - 1, y).SetActive(true);
			TileRefs(x - 1, y).SetType(31);
			TileRefs(x - 1, y).SetFrameX(0);
			TileRefs(x - 1, y).SetFrameY(18);
			TileRefs(x, y).SetActive(true);
			TileRefs(x, y).SetType(31);
			TileRefs(x, y).SetFrameX(18);
			TileRefs(x, y).SetFrameY(18);
		}

		public static void AddHellHouses(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = (int)((double)Main.maxTilesX * 0.25);
			for (int i = num; i < Main.maxTilesX - num; i++)
			{
				int num2 = Main.maxTilesY - 40;
				while (TileRefs(i, num2).Active || TileRefs(i, num2).Liquid > 0)
				{
					num2--;
				}
				if (TileRefs(i, num2 + 1).Active)
				{
					byte b = (byte)WorldModify.genRand.Next(75, 77);
					byte wall = 13;
					if (WorldModify.genRand.Next(5) > 0)
					{
						b = 75;
					}
					if (b == 75)
					{
						wall = 14;
					}
					HellHouse(TileRefs, i, num2, b, wall);
					i += WorldModify.genRand.Next(15, 80);
				}
			}
			float section = (float)(Main.maxTilesX / 4200);
			int tiles = 0;
			while ((float)tiles < 200f * section)
			{
				int attempts = 0;
				bool tilePlaced = false;
				while (!tilePlaced)
				{
					attempts++;
					int x = WorldModify.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.8));
					int y = WorldModify.genRand.Next(Main.maxTilesY - 300, Main.maxTilesY - 20);
					if (TileRefs(x, y).Active && (TileRefs(x, y).Type == 75 || TileRefs(x, y).Type == 76))
					{
						int direction = 0;
						if (TileRefs(x - 1, y).Wall > 0)
							direction = -1;
						else if (TileRefs(x + 1, y).Wall > 0)
							direction = 1;

						if (!TileRefs(x + direction, y).Active && !TileRefs(x + direction, y + 1).Active)
						{
							bool flag2 = false;
							for (int j = x - 8; j < x + 8; j++)
							{
								for (int k = y - 8; k < y + 8; k++)
								{
									if (TileRefs(j, k).Active && TileRefs(j, k).Type == 4)
									{
										flag2 = true;
										break;
									}
								}
							}
							if (!flag2)
							{
								WorldModify.PlaceTile(TileRefs, null, x + direction, y, 4, true, true, -1, 7);
								tilePlaced = true;
							}
						}
					}
					if (attempts > 1000)
						tilePlaced = true;
				}
				tiles++;
			}
		}

		public static void HellHouse(Func<Int32, Int32, ITile> TileRefs, int i, int j, byte type = 76, byte wall = 13)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = WorldModify.genRand.Next(8, 20);
			int num2 = WorldModify.genRand.Next(1, 3);
			int num3 = WorldModify.genRand.Next(4, 13);
			int num4 = j;
			for (int k = 0; k < num2; k++)
			{
				int num5 = WorldModify.genRand.Next(5, 9);
				HellRoom(TileRefs, i, num4, num, num5, type, wall);
				num4 -= num5;
			}
			num4 = j;
			for (int l = 0; l < num3; l++)
			{
				int num6 = WorldModify.genRand.Next(5, 9);
				num4 += num6;
				HellRoom(TileRefs, i, num4, num, num6, type, wall);
			}
			for (int m = i - num / 2; m <= i + num / 2; m++)
			{
				num4 = j;
				while (num4 < Main.maxTilesY && ((TileRefs(m, num4).Active &&
					(TileRefs(m, num4).Type == 76 || TileRefs(m, num4).Type == 75)) ||
					TileRefs(i, num4).Wall == 13 || TileRefs(i, num4).Wall == 14))
				{
					num4++;
				}
				int num7 = 6 + WorldModify.genRand.Next(3);
				while (num4 < Main.maxTilesY && !TileRefs(m, num4).Active)
				{
					num7--;
					TileRefs(m, num4).SetActive(true);
					TileRefs(m, num4).SetType(57);
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
			while (num4 < Main.maxTilesY && ((TileRefs(i, num4).Active && (TileRefs(i, num4).Type == 76 || TileRefs(i, num4).Type == 75)) || TileRefs(i, num4).Wall == 13 || TileRefs(i, num4).Wall == 14))
			{
				num4++;
			}
			num4--;
			num9 = num4;
			while ((TileRefs(i, num4).Active && (TileRefs(i, num4).Type == 76 || TileRefs(i, num4).Type == 75)) || TileRefs(i, num4).Wall == 13 || TileRefs(i, num4).Wall == 14)
			{
				num4--;
				if (TileRefs(i, num4).Active && (TileRefs(i, num4).Type == 76 || TileRefs(i, num4).Type == 75))
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
						if (TileRefs(n, num4 - 1).Wall == 13)
						{
							TileRefs(n, num4).SetWall(13);
						}
						if (TileRefs(n, num4 - 1).Wall == 14)
						{
							TileRefs(n, num4).SetWall(14);
						}
						TileRefs(n, num4).SetType(19);
						TileRefs(n, num4).SetActive(true);
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
								if (TileRefs(num19, num20).Type == 76 || TileRefs(num19, num20).Type == 19)
								{
									TileRefs(num19, num20).SetActive(false);
								}
								TileRefs(num19, num20).SetWall(0);
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

		public static void HellRoom(Func<Int32, Int32, ITile> TileRefs, int i, int j, int width, int height, byte type = 76, byte wall = 13)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (j > Main.maxTilesY - 40)
			{
				return;
			}
			for (int k = i - width / 2; k <= i + width / 2; k++)
			{
				for (int l = j - height; l <= j; l++)
				{
					try
					{
						TileRefs(k, l).SetActive(true);
						TileRefs(k, l).SetType(type);
						TileRefs(k, l).SetLiquid(0);
						TileRefs(k, l).SetLava(false);
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
						TileRefs(m, n).SetActive(false);
						TileRefs(m, n).SetWall(wall);
						TileRefs(m, n).SetLiquid(0);
						TileRefs(m, n).SetLava(false);
					}
					catch
					{
					}
				}
			}
		}

		public static void PlaceTraps(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			var maxTraps = (int)((double)Main.maxTilesX * 0.05);

			using (var logger = new ProgressLogger(maxTraps, Languages.Generation_PlacingTraps))
			{
				for (int num273 = 0; num273 < maxTraps; num273++)
				{
					float num274 = (float)((double)num273 / ((double)Main.maxTilesX * 0.05));
					//Main.statusText = "Placing traps: " + (int)(num274 * 100f + 1f) + "%";
					for (int num275 = 0; num275 < MAX_TRAPS; num275++)
					{
						int X = Main.rand.Next(200, Main.maxTilesX - 200);
						int Y = Main.rand.Next((int)Main.worldSurface, Main.maxTilesY - 300);
						if (TileRefs(X, Y).Wall == 0 && WorldModify.placeTrap(TileRefs, null, X, Y, -1))
						{
							break;
						}
					}
				}
			}
		}

		public static bool GrowEpicTree(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = y;
			while (TileRefs(i, num).Type == 20)
			{
				num++;
			}
			if (TileRefs(i, num).Active && TileRefs(i, num).Type == 2 && TileRefs(i, num - 1).Wall == 0 &&
				TileRefs(i, num - 1).Liquid == 0 && ((TileRefs(i - 1, num).Active && (TileRefs(i - 1, num).Type == 2 ||
				TileRefs(i - 1, num).Type == 23 || TileRefs(i - 1, num).Type == 60 || TileRefs(i - 1, num).Type == 109 || TileRefs(i - 1, num).Type == 147)) ||
				(TileRefs(i + 1, num).Active && (TileRefs(i + 1, num).Type == 2 || TileRefs(i + 1, num).Type == 23 ||
				TileRefs(i + 1, num).Type == 60 || TileRefs(i + 1, num).Type == 109 || TileRefs(i + 1, num).Type == 147))))
			{
				int num2 = 1;
				if (WorldModify.EmptyTileCheck(TileRefs, i - num2, i + num2, num - 55, num - 1, 20))
				{
					bool flag = false;
					bool flag2 = false;
					int num3 = WorldModify.genRand.Next(20, 30);
					int num4;
					for (int j = num - num3; j < num; j++)
					{
						TileRefs(i, j).SetFrameNumber((byte)WorldModify.genRand.Next(3));
						TileRefs(i, j).SetActive(true);
						TileRefs(i, j).SetType(5);
						num4 = WorldModify.genRand.Next(3);
						int num5 = WorldModify.genRand.Next(10);
						if (j == num - 1 || j == num - num3)
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
						if (num5 == 1)
						{
							if (num4 == 0)
							{
								TileRefs(i, j).SetFrameX(0);
								TileRefs(i, j).SetFrameY(66);
							}
							if (num4 == 1)
							{
								TileRefs(i, j).SetFrameX(0);
								TileRefs(i, j).SetFrameY(88);
							}
							if (num4 == 2)
							{
								TileRefs(i, j).SetFrameX(0);
								TileRefs(i, j).SetFrameY(110);
							}
						}
						else
						{
							if (num5 == 2)
							{
								if (num4 == 0)
								{
									TileRefs(i, j).SetFrameX(22);
									TileRefs(i, j).SetFrameY(0);
								}
								if (num4 == 1)
								{
									TileRefs(i, j).SetFrameX(22);
									TileRefs(i, j).SetFrameY(22);
								}
								if (num4 == 2)
								{
									TileRefs(i, j).SetFrameX(22);
									TileRefs(i, j).SetFrameY(44);
								}
							}
							else
							{
								if (num5 == 3)
								{
									if (num4 == 0)
									{
										TileRefs(i, j).SetFrameX(44);
										TileRefs(i, j).SetFrameY(66);
									}
									if (num4 == 1)
									{
										TileRefs(i, j).SetFrameX(44);
										TileRefs(i, j).SetFrameY(88);
									}
									if (num4 == 2)
									{
										TileRefs(i, j).SetFrameX(44);
										TileRefs(i, j).SetFrameY(110);
									}
								}
								else
								{
									if (num5 == 4)
									{
										if (num4 == 0)
										{
											TileRefs(i, j).SetFrameX(22);
											TileRefs(i, j).SetFrameY(66);
										}
										if (num4 == 1)
										{
											TileRefs(i, j).SetFrameX(22);
											TileRefs(i, j).SetFrameY(88);
										}
										if (num4 == 2)
										{
											TileRefs(i, j).SetFrameX(22);
											TileRefs(i, j).SetFrameY(110);
										}
									}
									else
									{
										if (num5 == 5)
										{
											if (num4 == 0)
											{
												TileRefs(i, j).SetFrameX(88);
												TileRefs(i, j).SetFrameY(0);
											}
											if (num4 == 1)
											{
												TileRefs(i, j).SetFrameX(88);
												TileRefs(i, j).SetFrameY(22);
											}
											if (num4 == 2)
											{
												TileRefs(i, j).SetFrameX(88);
												TileRefs(i, j).SetFrameY(44);
											}
										}
										else
										{
											if (num5 == 6)
											{
												if (num4 == 0)
												{
													TileRefs(i, j).SetFrameX(66);
													TileRefs(i, j).SetFrameY(66);
												}
												if (num4 == 1)
												{
													TileRefs(i, j).SetFrameX(66);
													TileRefs(i, j).SetFrameY(88);
												}
												if (num4 == 2)
												{
													TileRefs(i, j).SetFrameX(66);
													TileRefs(i, j).SetFrameY(110);
												}
											}
											else
											{
												if (num5 == 7)
												{
													if (num4 == 0)
													{
														TileRefs(i, j).SetFrameX(110);
														TileRefs(i, j).SetFrameY(66);
													}
													if (num4 == 1)
													{
														TileRefs(i, j).SetFrameX(110);
														TileRefs(i, j).SetFrameY(88);
													}
													if (num4 == 2)
													{
														TileRefs(i, j).SetFrameX(110);
														TileRefs(i, j).SetFrameY(110);
													}
												}
												else
												{
													if (num4 == 0)
													{
														TileRefs(i, j).SetFrameX(0);
														TileRefs(i, j).SetFrameY(0);
													}
													if (num4 == 1)
													{
														TileRefs(i, j).SetFrameX(0);
														TileRefs(i, j).SetFrameY(22);
													}
													if (num4 == 2)
													{
														TileRefs(i, j).SetFrameX(0);
														TileRefs(i, j).SetFrameY(44);
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
							TileRefs(i - 1, j).SetActive(true);
							TileRefs(i - 1, j).SetType(5);
							num4 = WorldModify.genRand.Next(3);
							if (WorldModify.genRand.Next(3) < 2)
							{
								if (num4 == 0)
								{
									TileRefs(i - 1, j).SetFrameX(44);
									TileRefs(i - 1, j).SetFrameY(198);
								}
								if (num4 == 1)
								{
									TileRefs(i - 1, j).SetFrameX(44);
									TileRefs(i - 1, j).SetFrameY(220);
								}
								if (num4 == 2)
								{
									TileRefs(i - 1, j).SetFrameX(44);
									TileRefs(i - 1, j).SetFrameY(242);
								}
							}
							else
							{
								if (num4 == 0)
								{
									TileRefs(i - 1, j).SetFrameX(66);
									TileRefs(i - 1, j).SetFrameY(0);
								}
								if (num4 == 1)
								{
									TileRefs(i - 1, j).SetFrameX(66);
									TileRefs(i - 1, j).SetFrameY(22);
								}
								if (num4 == 2)
								{
									TileRefs(i - 1, j).SetFrameX(66);
									TileRefs(i - 1, j).SetFrameY(44);
								}
							}
						}
						if (num5 == 6 || num5 == 7)
						{
							TileRefs(i + 1, j).SetActive(true);
							TileRefs(i + 1, j).SetType(5);
							num4 = WorldModify.genRand.Next(3);
							if (WorldModify.genRand.Next(3) < 2)
							{
								if (num4 == 0)
								{
									TileRefs(i + 1, j).SetFrameX(66);
									TileRefs(i + 1, j).SetFrameY(198);
								}
								if (num4 == 1)
								{
									TileRefs(i + 1, j).SetFrameX(66);
									TileRefs(i + 1, j).SetFrameY(220);
								}
								if (num4 == 2)
								{
									TileRefs(i + 1, j).SetFrameX(66);
									TileRefs(i + 1, j).SetFrameY(242);
								}
							}
							else
							{
								if (num4 == 0)
								{
									TileRefs(i + 1, j).SetFrameX(88);
									TileRefs(i + 1, j).SetFrameY(66);
								}
								if (num4 == 1)
								{
									TileRefs(i + 1, j).SetFrameX(88);
									TileRefs(i + 1, j).SetFrameY(88);
								}
								if (num4 == 2)
								{
									TileRefs(i + 1, j).SetFrameX(88);
									TileRefs(i + 1, j).SetFrameY(110);
								}
							}
						}
					}
					int num6 = WorldModify.genRand.Next(3);
					bool flag3 = false;
					bool flag4 = false;
					if (TileRefs(i - 1, num).Active && (TileRefs(i - 1, num).Type == 2 || TileRefs(i - 1, num).Type == 23 || TileRefs(i - 1, num).Type == 60 || TileRefs(i - 1, num).Type == 109 || TileRefs(i - 1, num).Type == 147))
					{
						flag3 = true;
					}
					if (TileRefs(i + 1, num).Active && (TileRefs(i + 1, num).Type == 2 || TileRefs(i + 1, num).Type == 23 || TileRefs(i + 1, num).Type == 60 || TileRefs(i + 1, num).Type == 109 || TileRefs(i + 1, num).Type == 147))
					{
						flag4 = true;
					}
					if (!flag3)
					{
						if (num6 == 0)
						{
							num6 = 2;
						}
						if (num6 == 1)
						{
							num6 = 3;
						}
					}
					if (!flag4)
					{
						if (num6 == 0)
						{
							num6 = 1;
						}
						if (num6 == 2)
						{
							num6 = 3;
						}
					}
					if (flag3 && !flag4)
					{
						num6 = 1;
					}
					if (flag4 && !flag3)
					{
						num6 = 2;
					}
					if (num6 == 0 || num6 == 1)
					{
						TileRefs(i + 1, num - 1).SetActive(true);
						TileRefs(i + 1, num - 1).SetType(5);
						num4 = WorldModify.genRand.Next(3);
						if (num4 == 0)
						{
							TileRefs(i + 1, num - 1).SetFrameX(22);
							TileRefs(i + 1, num - 1).SetFrameY(132);
						}
						if (num4 == 1)
						{
							TileRefs(i + 1, num - 1).SetFrameX(22);
							TileRefs(i + 1, num - 1).SetFrameY(154);
						}
						if (num4 == 2)
						{
							TileRefs(i + 1, num - 1).SetFrameX(22);
							TileRefs(i + 1, num - 1).SetFrameY(176);
						}
					}
					if (num6 == 0 || num6 == 2)
					{
						TileRefs(i - 1, num - 1).SetActive(true);
						TileRefs(i - 1, num - 1).SetType(5);
						num4 = WorldModify.genRand.Next(3);
						if (num4 == 0)
						{
							TileRefs(i - 1, num - 1).SetFrameX(44);
							TileRefs(i - 1, num - 1).SetFrameY(132);
						}
						if (num4 == 1)
						{
							TileRefs(i - 1, num - 1).SetFrameX(44);
							TileRefs(i - 1, num - 1).SetFrameY(154);
						}
						if (num4 == 2)
						{
							TileRefs(i - 1, num - 1).SetFrameX(44);
							TileRefs(i - 1, num - 1).SetFrameY(176);
						}
					}
					num4 = WorldModify.genRand.Next(3);
					if (num6 == 0)
					{
						if (num4 == 0)
						{
							TileRefs(i, num - 1).SetFrameX(88);
							TileRefs(i, num - 1).SetFrameY(132);
						}
						if (num4 == 1)
						{
							TileRefs(i, num - 1).SetFrameX(88);
							TileRefs(i, num - 1).SetFrameY(154);
						}
						if (num4 == 2)
						{
							TileRefs(i, num - 1).SetFrameX(88);
							TileRefs(i, num - 1).SetFrameY(176);
						}
					}
					else
					{
						if (num6 == 1)
						{
							if (num4 == 0)
							{
								TileRefs(i, num - 1).SetFrameX(0);
								TileRefs(i, num - 1).SetFrameY(132);
							}
							if (num4 == 1)
							{
								TileRefs(i, num - 1).SetFrameX(0);
								TileRefs(i, num - 1).SetFrameY(154);
							}
							if (num4 == 2)
							{
								TileRefs(i, num - 1).SetFrameX(0);
								TileRefs(i, num - 1).SetFrameY(176);
							}
						}
						else
						{
							if (num6 == 2)
							{
								if (num4 == 0)
								{
									TileRefs(i, num - 1).SetFrameX(66);
									TileRefs(i, num - 1).SetFrameY(132);
								}
								if (num4 == 1)
								{
									TileRefs(i, num - 1).SetFrameX(66);
									TileRefs(i, num - 1).SetFrameY(154);
								}
								if (num4 == 2)
								{
									TileRefs(i, num - 1).SetFrameX(66);
									TileRefs(i, num - 1).SetFrameY(176);
								}
							}
						}
					}
					if (WorldModify.genRand.Next(3) < 2)
					{
						num4 = WorldModify.genRand.Next(3);
						if (num4 == 0)
						{
							TileRefs(i, num - num3).SetFrameX(22);
							TileRefs(i, num - num3).SetFrameY(198);
						}
						if (num4 == 1)
						{
							TileRefs(i, num - num3).SetFrameX(22);
							TileRefs(i, num - num3).SetFrameY(220);
						}
						if (num4 == 2)
						{
							TileRefs(i, num - num3).SetFrameX(22);
							TileRefs(i, num - num3).SetFrameY(242);
						}
					}
					else
					{
						num4 = WorldModify.genRand.Next(3);
						if (num4 == 0)
						{
							TileRefs(i, num - num3).SetFrameX(0);
							TileRefs(i, num - num3).SetFrameY(198);
						}
						if (num4 == 1)
						{
							TileRefs(i, num - num3).SetFrameX(0);
							TileRefs(i, num - num3).SetFrameY(220);
						}
						if (num4 == 2)
						{
							TileRefs(i, num - num3).SetFrameX(0);
							TileRefs(i, num - num3).SetFrameY(242);
						}
					}
					WorldModify.RangeFrame(TileRefs, sandbox, i - 2, num - num3 - 1, i + 2, num + 1);
					NetMessage.SendTileSquare(-1, i, (int)((double)num - (double)num3 * 0.5), num3 + 1);
					return true;
				}
			}
			return false;
		}

		public static void GrowTree(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = y;
			while (TileRefs(i, num).Type == 20)
			{
				num++;
			}
			if ((TileRefs(i - 1, num - 1).Liquid != 0 || TileRefs(i - 1, num - 1).Liquid != 0 ||
				TileRefs(i + 1, num - 1).Liquid != 0) && TileRefs(i, num).Type != 60)
			{
				return;
			}
			if (TileRefs(i, num).Active && (TileRefs(i, num).Type == 2 || TileRefs(i, num).Type == 23 ||
				TileRefs(i, num).Type == 60 || TileRefs(i, num).Type == 109 || TileRefs(i, num).Type == 147) && TileRefs(i, num - 1).Wall == 0 &&
					((TileRefs(i - 1, num).Active && (TileRefs(i - 1, num).Type == 2 ||
					TileRefs(i - 1, num).Type == 23 || TileRefs(i - 1, num).Type == 60 ||
					TileRefs(i - 1, num).Type == 109 || TileRefs(i - 1, num).Type == 147)) || (TileRefs(i + 1, num).Active &&
					(TileRefs(i + 1, num).Type == 2 || TileRefs(i + 1, num).Type == 23 ||
					TileRefs(i + 1, num).Type == 60 || TileRefs(i + 1, num).Type == 109 || TileRefs(i + 1, num).Type == 147))))
			{
				int num2 = 1;
				int num3 = 16;
				if (TileRefs(i, num).Type == 60)
				{
					num3 += 5;
				}
				if (WorldModify.EmptyTileCheck(TileRefs, i - num2, i + num2, num - num3, num - 1, 20))
				{
					bool flag = false;
					bool flag2 = false;
					int num4 = WorldModify.genRand.Next(5, num3 + 1);
					int num5;
					for (int j = num - num4; j < num; j++)
					{
						TileRefs(i, j).SetFrameNumber((byte)WorldModify.genRand.Next(3));
						TileRefs(i, j).SetActive(true);
						TileRefs(i, j).SetType(5);
						num5 = WorldModify.genRand.Next(3);
						int num6 = WorldModify.genRand.Next(10);
						if (j == num - 1 || j == num - num4)
						{
							num6 = 0;
						}
						while (((num6 == 5 || num6 == 7) && flag) || ((num6 == 6 || num6 == 7) && flag2))
						{
							num6 = WorldModify.genRand.Next(10);
						}
						flag = false;
						flag2 = false;
						if (num6 == 5 || num6 == 7)
						{
							flag = true;
						}
						if (num6 == 6 || num6 == 7)
						{
							flag2 = true;
						}
						if (num6 == 1)
						{
							if (num5 == 0)
							{
								TileRefs(i, j).SetFrameX(0);
								TileRefs(i, j).SetFrameY(66);
							}
							if (num5 == 1)
							{
								TileRefs(i, j).SetFrameX(0);
								TileRefs(i, j).SetFrameY(88);
							}
							if (num5 == 2)
							{
								TileRefs(i, j).SetFrameX(0);
								TileRefs(i, j).SetFrameY(110);
							}
						}
						else
						{
							if (num6 == 2)
							{
								if (num5 == 0)
								{
									TileRefs(i, j).SetFrameX(22);
									TileRefs(i, j).SetFrameY(0);
								}
								if (num5 == 1)
								{
									TileRefs(i, j).SetFrameX(22);
									TileRefs(i, j).SetFrameY(22);
								}
								if (num5 == 2)
								{
									TileRefs(i, j).SetFrameX(22);
									TileRefs(i, j).SetFrameY(44);
								}
							}
							else
							{
								if (num6 == 3)
								{
									if (num5 == 0)
									{
										TileRefs(i, j).SetFrameX(44);
										TileRefs(i, j).SetFrameY(66);
									}
									if (num5 == 1)
									{
										TileRefs(i, j).SetFrameX(44);
										TileRefs(i, j).SetFrameY(88);
									}
									if (num5 == 2)
									{
										TileRefs(i, j).SetFrameX(44);
										TileRefs(i, j).SetFrameY(110);
									}
								}
								else
								{
									if (num6 == 4)
									{
										if (num5 == 0)
										{
											TileRefs(i, j).SetFrameX(22);
											TileRefs(i, j).SetFrameY(66);
										}
										if (num5 == 1)
										{
											TileRefs(i, j).SetFrameX(22);
											TileRefs(i, j).SetFrameY(88);
										}
										if (num5 == 2)
										{
											TileRefs(i, j).SetFrameX(22);
											TileRefs(i, j).SetFrameY(110);
										}
									}
									else
									{
										if (num6 == 5)
										{
											if (num5 == 0)
											{
												TileRefs(i, j).SetFrameX(88);
												TileRefs(i, j).SetFrameY(0);
											}
											if (num5 == 1)
											{
												TileRefs(i, j).SetFrameX(88);
												TileRefs(i, j).SetFrameY(22);
											}
											if (num5 == 2)
											{
												TileRefs(i, j).SetFrameX(88);
												TileRefs(i, j).SetFrameY(44);
											}
										}
										else
										{
											if (num6 == 6)
											{
												if (num5 == 0)
												{
													TileRefs(i, j).SetFrameX(66);
													TileRefs(i, j).SetFrameY(66);
												}
												if (num5 == 1)
												{
													TileRefs(i, j).SetFrameX(66);
													TileRefs(i, j).SetFrameY(88);
												}
												if (num5 == 2)
												{
													TileRefs(i, j).SetFrameX(66);
													TileRefs(i, j).SetFrameY(110);
												}
											}
											else
											{
												if (num6 == 7)
												{
													if (num5 == 0)
													{
														TileRefs(i, j).SetFrameX(110);
														TileRefs(i, j).SetFrameY(66);
													}
													if (num5 == 1)
													{
														TileRefs(i, j).SetFrameX(110);
														TileRefs(i, j).SetFrameY(88);
													}
													if (num5 == 2)
													{
														TileRefs(i, j).SetFrameX(110);
														TileRefs(i, j).SetFrameY(110);
													}
												}
												else
												{
													if (num5 == 0)
													{
														TileRefs(i, j).SetFrameX(0);
														TileRefs(i, j).SetFrameY(0);
													}
													if (num5 == 1)
													{
														TileRefs(i, j).SetFrameX(0);
														TileRefs(i, j).SetFrameY(22);
													}
													if (num5 == 2)
													{
														TileRefs(i, j).SetFrameX(0);
														TileRefs(i, j).SetFrameY(44);
													}
												}
											}
										}
									}
								}
							}
						}
						if (num6 == 5 || num6 == 7)
						{
							TileRefs(i - 1, j).SetActive(true);
							TileRefs(i - 1, j).SetType(5);
							num5 = WorldModify.genRand.Next(3);
							if (WorldModify.genRand.Next(3) < 2)
							{
								if (num5 == 0)
								{
									TileRefs(i - 1, j).SetFrameX(44);
									TileRefs(i - 1, j).SetFrameY(198);
								}
								if (num5 == 1)
								{
									TileRefs(i - 1, j).SetFrameX(44);
									TileRefs(i - 1, j).SetFrameY(220);
								}
								if (num5 == 2)
								{
									TileRefs(i - 1, j).SetFrameX(44);
									TileRefs(i - 1, j).SetFrameY(242);
								}
							}
							else
							{
								if (num5 == 0)
								{
									TileRefs(i - 1, j).SetFrameX(66);
									TileRefs(i - 1, j).SetFrameY(0);
								}
								if (num5 == 1)
								{
									TileRefs(i - 1, j).SetFrameX(66);
									TileRefs(i - 1, j).SetFrameY(22);
								}
								if (num5 == 2)
								{
									TileRefs(i - 1, j).SetFrameX(66);
									TileRefs(i - 1, j).SetFrameY(44);
								}
							}
						}
						if (num6 == 6 || num6 == 7)
						{
							TileRefs(i + 1, j).SetActive(true);
							TileRefs(i + 1, j).SetType(5);
							num5 = WorldModify.genRand.Next(3);
							if (WorldModify.genRand.Next(3) < 2)
							{
								if (num5 == 0)
								{
									TileRefs(i + 1, j).SetFrameX(66);
									TileRefs(i + 1, j).SetFrameY(198);
								}
								if (num5 == 1)
								{
									TileRefs(i + 1, j).SetFrameX(66);
									TileRefs(i + 1, j).SetFrameY(220);
								}
								if (num5 == 2)
								{
									TileRefs(i + 1, j).SetFrameX(66);
									TileRefs(i + 1, j).SetFrameY(242);
								}
							}
							else
							{
								if (num5 == 0)
								{
									TileRefs(i + 1, j).SetFrameX(88);
									TileRefs(i + 1, j).SetFrameY(66);
								}
								if (num5 == 1)
								{
									TileRefs(i + 1, j).SetFrameX(88);
									TileRefs(i + 1, j).SetFrameY(88);
								}
								if (num5 == 2)
								{
									TileRefs(i + 1, j).SetFrameX(88);
									TileRefs(i + 1, j).SetFrameY(110);
								}
							}
						}
					}
					int num7 = WorldModify.genRand.Next(3);
					bool flag3 = false;
					bool flag4 = false;
					if (TileRefs(i - 1, num).Active && (TileRefs(i - 1, num).Type == 2 || TileRefs(i - 1, num).Type == 23 || TileRefs(i - 1, num).Type == 60 || TileRefs(i - 1, num).Type == 109 || TileRefs(i - 1, num).Type == 147))
					{
						flag3 = true;
					}
					if (TileRefs(i + 1, num).Active && (TileRefs(i + 1, num).Type == 2 || TileRefs(i + 1, num).Type == 23 || TileRefs(i + 1, num).Type == 60 || TileRefs(i + 1, num).Type == 109 || TileRefs(i + 1, num).Type == 147))
					{
						flag4 = true;
					}
					if (!flag3)
					{
						if (num7 == 0)
						{
							num7 = 2;
						}
						if (num7 == 1)
						{
							num7 = 3;
						}
					}
					if (!flag4)
					{
						if (num7 == 0)
						{
							num7 = 1;
						}
						if (num7 == 2)
						{
							num7 = 3;
						}
					}
					if (flag3 && !flag4)
					{
						num7 = 1;
					}
					if (flag4 && !flag3)
					{
						num7 = 2;
					}
					if (num7 == 0 || num7 == 1)
					{
						TileRefs(i + 1, num - 1).SetActive(true);
						TileRefs(i + 1, num - 1).SetType(5);
						num5 = WorldModify.genRand.Next(3);
						if (num5 == 0)
						{
							TileRefs(i + 1, num - 1).SetFrameX(22);
							TileRefs(i + 1, num - 1).SetFrameY(132);
						}
						if (num5 == 1)
						{
							TileRefs(i + 1, num - 1).SetFrameX(22);
							TileRefs(i + 1, num - 1).SetFrameY(154);
						}
						if (num5 == 2)
						{
							TileRefs(i + 1, num - 1).SetFrameX(22);
							TileRefs(i + 1, num - 1).SetFrameY(176);
						}
					}
					if (num7 == 0 || num7 == 2)
					{
						TileRefs(i - 1, num - 1).SetActive(true);
						TileRefs(i - 1, num - 1).SetType(5);
						num5 = WorldModify.genRand.Next(3);
						if (num5 == 0)
						{
							TileRefs(i - 1, num - 1).SetFrameX(44);
							TileRefs(i - 1, num - 1).SetFrameY(132);
						}
						if (num5 == 1)
						{
							TileRefs(i - 1, num - 1).SetFrameX(44);
							TileRefs(i - 1, num - 1).SetFrameY(154);
						}
						if (num5 == 2)
						{
							TileRefs(i - 1, num - 1).SetFrameX(44);
							TileRefs(i - 1, num - 1).SetFrameY(176);
						}
					}
					num5 = WorldModify.genRand.Next(3);
					if (num7 == 0)
					{
						if (num5 == 0)
						{
							TileRefs(i, num - 1).SetFrameX(88);
							TileRefs(i, num - 1).SetFrameY(132);
						}
						if (num5 == 1)
						{
							TileRefs(i, num - 1).SetFrameX(88);
							TileRefs(i, num - 1).SetFrameY(154);
						}
						if (num5 == 2)
						{
							TileRefs(i, num - 1).SetFrameX(88);
							TileRefs(i, num - 1).SetFrameY(176);
						}
					}
					else
					{
						if (num7 == 1)
						{
							if (num5 == 0)
							{
								TileRefs(i, num - 1).SetFrameX(0);
								TileRefs(i, num - 1).SetFrameY(132);
							}
							if (num5 == 1)
							{
								TileRefs(i, num - 1).SetFrameX(0);
								TileRefs(i, num - 1).SetFrameY(154);
							}
							if (num5 == 2)
							{
								TileRefs(i, num - 1).SetFrameX(0);
								TileRefs(i, num - 1).SetFrameY(176);
							}
						}
						else
						{
							if (num7 == 2)
							{
								if (num5 == 0)
								{
									TileRefs(i, num - 1).SetFrameX(66);
									TileRefs(i, num - 1).SetFrameY(132);
								}
								if (num5 == 1)
								{
									TileRefs(i, num - 1).SetFrameX(66);
									TileRefs(i, num - 1).SetFrameY(154);
								}
								if (num5 == 2)
								{
									TileRefs(i, num - 1).SetFrameX(66);
									TileRefs(i, num - 1).SetFrameY(176);
								}
							}
						}
					}
					if (WorldModify.genRand.Next(4) < 3)
					{
						num5 = WorldModify.genRand.Next(3);
						if (num5 == 0)
						{
							TileRefs(i, num - num4).SetFrameX(22);
							TileRefs(i, num - num4).SetFrameY(198);
						}
						if (num5 == 1)
						{
							TileRefs(i, num - num4).SetFrameX(22);
							TileRefs(i, num - num4).SetFrameY(220);
						}
						if (num5 == 2)
						{
							TileRefs(i, num - num4).SetFrameX(22);
							TileRefs(i, num - num4).SetFrameY(242);
						}
					}
					else
					{
						num5 = WorldModify.genRand.Next(3);
						if (num5 == 0)
						{
							TileRefs(i, num - num4).SetFrameX(0);
							TileRefs(i, num - num4).SetFrameY(198);
						}
						if (num5 == 1)
						{
							TileRefs(i, num - num4).SetFrameX(0);
							TileRefs(i, num - num4).SetFrameY(220);
						}
						if (num5 == 2)
						{
							TileRefs(i, num - num4).SetFrameX(0);
							TileRefs(i, num - num4).SetFrameY(242);
						}
					}
					WorldModify.RangeFrame(TileRefs, sandbox, i - 2, num - num4 - 1, i + 2, num + 1);
					NetMessage.SendTileSquare(-1, i, (int)((double)num - (double)num4 * 0.5), num4 + 1);
				}
			}
		}

		public static void AddTrees(Func<Int32, Int32, ITile> TileRefs)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			for (int x = 1; x < Main.maxTilesX - 1; x++)
			{
				int y = 20;
				while ((double)y < Main.worldSurface)
				{
					GrowTree(TileRefs, null, x, y);
					y++;
				}

				if (WorldModify.genRand.Next(3) == 0 || WorldModify.genRand.Next(4) == 0)
					x++;
			}
		}

		public static void MineHouse(Func<Int32, Int32, ITile> TileRefs, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (x < 50 || x > Main.maxTilesX - 50 || y < 50 || y > Main.maxTilesY - 50 || WorldModify.SolidTile(TileRefs, x, y) || TileRefs(x, y).Wall > 0)
				return;

			int num = WorldModify.genRand.Next(6, 12);
			int num2 = WorldModify.genRand.Next(3, 6);
			int num3 = WorldModify.genRand.Next(15, 30);
			int num4 = WorldModify.genRand.Next(15, 30);

			int num5 = y - num;
			int num6 = y + num2;
			for (int k = 0; k < 2; k++)
			{
				bool flag = true;
				int num7 = x;
				int num8 = y;
				int num9 = -1;
				int num10 = num3;
				if (k == 1)
				{
					num9 = 1;
					num10 = num4;
					num7++;
				}
				while (flag)
				{
					if (num8 - num < num5)
					{
						num5 = num8 - num;
					}
					if (num8 + num2 > num6)
					{
						num6 = num8 + num2;
					}
					for (int l = 0; l < 2; l++)
					{
						int num11 = num8;
						bool flag2 = true;
						int num12 = num;
						int num13 = -1;
						if (l == 1)
						{
							num11++;
							num12 = num2;
							num13 = 1;
						}
						while (flag2)
						{
							if (num7 != x && TileRefs(num7 - num9, num11).Wall != 27 && (WorldModify.SolidTile(TileRefs, num7 - num9, num11) ||
								!TileRefs(num7 - num9, num11).Active))
							{
								TileRefs(num7 - num9, num11).SetActive(true);
								TileRefs(num7 - num9, num11).SetType(30);
							}
							if (WorldModify.SolidTile(TileRefs, num7 - 1, num11))
							{
								TileRefs(num7 - 1, num11).SetType(30);
							}
							if (WorldModify.SolidTile(TileRefs, num7 + 1, num11))
							{
								TileRefs(num7 + 1, num11).SetType(30);
							}
							if (WorldModify.SolidTile(TileRefs, num7, num11))
							{
								int num14 = 0;
								if (WorldModify.SolidTile(TileRefs, num7 - 1, num11)) num14++;
								if (WorldModify.SolidTile(TileRefs, num7 + 1, num11)) num14++;
								if (WorldModify.SolidTile(TileRefs, num7, num11 - 1)) num14++;
								if (WorldModify.SolidTile(TileRefs, num7, num11 + 1)) num14++;

								if (num14 < 2)
								{
									TileRefs(num7, num11).SetActive(false);
								}
								else
								{
									flag2 = false;
									TileRefs(num7, num11).SetType(30);
								}
							}
							else
							{
								TileRefs(num7, num11).SetWall(27);
								TileRefs(num7, num11).SetLiquid(0);
								TileRefs(num7, num11).SetLava(false);
							}
							num11 += num13;
							num12--;
							if (num12 <= 0)
							{
								if (!TileRefs(num7, num11).Active)
								{
									TileRefs(num7, num11).SetActive(true);
									TileRefs(num7, num11).SetType(30);
								}
								flag2 = false;
							}
						}
					}
					num10--;
					num7 += num9;
					if (WorldModify.SolidTile(TileRefs, num7, num8))
					{
						int num15 = 0;
						int num16 = 0;
						int num17 = num8;
						bool flag3 = true;
						while (flag3)
						{
							num17--;
							num15++;
							if (WorldModify.SolidTile(TileRefs, num7 - num9, num17))
							{
								num15 = 999;
								flag3 = false;
							}
							else if (!WorldModify.SolidTile(TileRefs, num7, num17))
							{
								flag3 = false;
							}
						}
						num17 = num8;
						flag3 = true;
						while (flag3)
						{
							num17++;
							num16++;
							if (WorldModify.SolidTile(TileRefs, num7 - num9, num17))
							{
								num16 = 999;
								flag3 = false;
							}
							else if (!WorldModify.SolidTile(TileRefs, num7, num17))
							{
								flag3 = false;
							}
						}
						if (num16 <= num15)
						{
							if (num16 > num2)
							{
								num10 = 0;
							}
							else
							{
								num8 += num16 + 1;
							}
						}
						else if (num15 > num)
						{
							num10 = 0;
						}
						else
						{
							num8 -= num15 + 1;
						}
					}
					if (num10 <= 0)
					{
						flag = false;
					}
				}
			}
			int num18 = x - num3 - 1;
			int num19 = x + num4 + 2;
			int num20 = num5 - 1;
			int num21 = num6 + 2;
			for (int m = num18; m < num19; m++)
			{
				for (int n = num20; n < num21; n++)
				{
					if (TileRefs(m, n).Wall == 27 && !TileRefs(m, n).Active)
					{
						if (TileRefs(m - 1, n).Wall != 27 && m < x && !WorldModify.SolidTile(TileRefs, m - 1, n))
						{
							WorldModify.PlaceTile(TileRefs, null, m, n, 30, true, false, -1, 0);
							TileRefs(m, n).SetWall(0);
						}
						if (TileRefs(m + 1, n).Wall != 27 && m > x && !WorldModify.SolidTile(TileRefs, m + 1, n))
						{
							WorldModify.PlaceTile(TileRefs, null, m, n, 30, true, false, -1, 0);
							TileRefs(m, n).SetWall(0);
						}
						for (int num22 = m - 1; num22 <= m + 1; num22++)
						{
							for (int num23 = n - 1; num23 <= n + 1; num23++)
							{
								if (WorldModify.SolidTile(TileRefs, num22, num23))
								{
									TileRefs(num22, num23).SetType(30);
								}
							}
						}
					}
					if (TileRefs(m, n).Type == 30 && TileRefs(m - 1, n).Wall == 27 && TileRefs(m + 1, n).Wall == 27 && (TileRefs(m, n - 1).Wall == 27 || TileRefs(m, n - 1).Active) && (TileRefs(m, n + 1).Wall == 27 || TileRefs(m, n + 1).Active))
					{
						TileRefs(m, n).SetActive(false);
						TileRefs(m, n).SetWall(27);
					}
				}
			}
			for (int num24 = num18; num24 < num19; num24++)
			{
				for (int num25 = num20; num25 < num21; num25++)
				{
					if (TileRefs(num24, num25).Type == 30)
					{
						if (TileRefs(num24 - 1, num25).Wall == 27 && TileRefs(num24 + 1, num25).Wall == 27 && !TileRefs(num24 - 1, num25).Active && !TileRefs(num24 + 1, num25).Active)
						{
							TileRefs(num24, num25).SetActive(false);
							TileRefs(num24, num25).SetWall(27);
						}
						if (TileRefs(num24, num25 - 1).Type != 21 && TileRefs(num24 - 1, num25).Wall == 27 && TileRefs(num24 + 1, num25).Type == 30 && TileRefs(num24 + 2, num25).Wall == 27 && !TileRefs(num24 - 1, num25).Active && !TileRefs(num24 + 2, num25).Active)
						{
							TileRefs(num24, num25).SetActive(false);
							TileRefs(num24, num25).SetWall(27);
							TileRefs(num24 + 1, num25).SetActive(false);
							TileRefs(num24 + 1, num25).SetWall(27);
						}
						if (TileRefs(num24, num25 - 1).Wall == 27 && TileRefs(num24, num25 + 1).Wall == 27 && !TileRefs(num24, num25 - 1).Active && !TileRefs(num24, num25 + 1).Active)
						{
							TileRefs(num24, num25).SetActive(false);
							TileRefs(num24, num25).SetWall(27);
						}
					}
				}
			}
			for (int num26 = num18; num26 < num19; num26++)
			{
				for (int num27 = num21; num27 > num20; num27--)
				{
					bool flag4 = false;
					if (TileRefs(num26, num27).Active && TileRefs(num26, num27).Type == 30)
					{
						int num28 = -1;
						for (int num29 = 0; num29 < 2; num29++)
						{
							if (!WorldModify.SolidTile(TileRefs, num26 + num28, num27) && TileRefs(num26 + num28, num27).Wall == 0)
							{
								int num30 = 0;
								int num31 = num27;
								int num32 = num27;

								while (TileRefs(num26, num31).Active && TileRefs(num26, num31).Type == 30 && !WorldModify.SolidTile(TileRefs, num26 + num28, num31) &&
									TileRefs(num26 + num28, num31).Wall == 0)
								{
									num31--;
									num30++;
								}

								num31++;
								int num33 = num31 + 1;
								if (num30 > 4)
								{
									if (WorldModify.genRand.Next(2) == 0)
									{
										num31 = num32 - 1;
										bool flag5 = true;
										for (int num34 = num26 - 2; num34 <= num26 + 2; num34++)
										{
											for (int num35 = num31 - 2; num35 <= num31; num35++)
											{
												if (num34 != num26 && TileRefs(num34, num35).Active)
													flag5 = false;
											}
										}
										if (flag5)
										{
											TileRefs(num26, num31).SetActive(false);
											TileRefs(num26, num31 - 1).SetActive(false);
											TileRefs(num26, num31 - 2).SetActive(false);
											WorldModify.PlaceTile(TileRefs, null, num26, num31, 10, true);
											flag4 = true;
										}
									}

									if (!flag4)
									{
										for (int num36 = num33; num36 < num32; num36++)
											TileRefs(num26, num36).SetType(124);
									}
								}
							}
							num28 = 1;
						}
					}
					if (flag4)
					{
						break;
					}
				}
			}

			for (int num37 = num18; num37 < num19; num37++)
			{
				bool flag6 = true;
				for (int num38 = num20; num38 < num21; num38++)
				{
					for (int num39 = num37 - 2; num39 <= num37 + 2; num39++)
					{
						if (TileRefs(num39, num38).Active && (!WorldModify.SolidTile(TileRefs, num39, num38) || TileRefs(num39, num38).Type == 10))
							flag6 = false;
					}
				}
				if (flag6)
				{
					for (int num40 = num20; num40 < num21; num40++)
					{
						if (TileRefs(num37, num40).Wall == 27 && !TileRefs(num37, num40).Active)
							WorldModify.PlaceTile(TileRefs, null, num37, num40, 124, true);
					}
				}
				num37 += WorldModify.genRand.Next(3);
			}

			for (int num41 = 0; num41 < 4; num41++)
			{
				int num42 = WorldModify.genRand.Next(num18 + 2, num19 - 1);
				int num43 = WorldModify.genRand.Next(num20 + 2, num21 - 1);

				while (TileRefs(num42, num43).Wall != 27)
				{
					num42 = WorldModify.genRand.Next(num18 + 2, num19 - 1);
					num43 = WorldModify.genRand.Next(num20 + 2, num21 - 1);
				}

				while (TileRefs(num42, num43).Active)
					num43--;

				while (!TileRefs(num42, num43).Active)
					num43++;

				num43--;
				if (TileRefs(num42, num43).Wall == 27)
				{
					if (WorldModify.genRand.Next(3) == 0)
					{
						int type = WorldModify.genRand.Next(9);

						switch (type)
						{
							case 0:
								type = 14;
								break;
							case 1:
								type = 16;
								break;
							case 2:
								type = 18;
								break;
							case 3:
								type = 86;
								break;
							case 4:
								type = 87;
								break;
							case 5:
								type = 94;
								break;
							case 6:
								type = 101;
								break;
							case 7:
								type = 104;
								break;
							case 8:
								type = 106;
								break;
						}

						WorldModify.PlaceTile(TileRefs, null, num42, num43, type, true);
					}
					else
					{
						int style = WorldModify.genRand.Next(2, 43);
						WorldModify.PlaceTile(TileRefs, null, num42, num43, 105, true, true, -1, style);
					}
				}
			}
		}
	}
}
