using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Terraria_Server.Commands;
using Terraria_Server.Plugins;
using Terraria_Server.Misc;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.Logging;
using Terraria_Server.Networking;

namespace Terraria_Server.WorldMod
{
	public static class WorldIO
	{
		private static object padlock = new object();
		public static bool worldCleared = false;
		private static int tempMoonPhase = Main.moonPhase;
		private static bool tempDayTime = Main.dayTime;
		private static bool tempBloodMoon = Main.bloodMoon;
		private static double tempTime = Main.time;

		public static void setWorldSize()
		{
			Main.bottomWorld = (float)(Main.maxTilesY * 16);
			Main.rightWorld = (float)(Main.maxTilesX * 16);
			Main.maxSectionsX = Main.maxTilesX / 200;
			Main.maxSectionsY = Main.maxTilesY / 150;
		}

        public static void SaveWorldCallback(object threadContext)
        {
            saveWorld(Server.World.SavePath, false);
        }

        public static void SaveWorldThreaded()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveWorldCallback), 1);
        }

		public static void clearWorld()
		{
			Statics.WorldLoaded = false;
			
			Main.trashItem = new Item();
			WorldModify.spawnEye = false;
			WorldModify.spawnNPC = 0;
			WorldModify.shadowOrbCount = 0;
			Main.helpText = 0;
			Main.dungeonX = 0;
			Main.dungeonY = 0;
			NPC.downedBoss1 = false;
			NPC.downedBoss2 = false;
			NPC.downedBoss3 = false;
			WorldModify.shadowOrbSmashed = false;
			WorldModify.spawnMeteor = false;
			WorldModify.stopDrops = false;
			Main.invasionDelay = 0;
			Main.invasionType = 0;
			Main.invasionSize = 0;
			Main.invasionWarn = 0;
			Main.invasionX = 0.0;
			WorldModify.noLiquidCheck = false;
			Liquid.numLiquid = 0;
			LiquidBuffer.numLiquidBuffer = 0;

//			if (WorldModify.lastMaxTilesX > Main.maxTilesX || WorldModify.lastMaxTilesY > Main.maxTilesY)
//			{
//				using (var prog = new ProgressLogger(WorldModify.lastMaxTilesX - 1, "Freeing unused resources"))
//					for (int i = 0; i < WorldModify.lastMaxTilesX; i++)
//					{
//						prog.Value = i;
//						for (int j = 0; j < WorldModify.lastMaxTilesY; j++)
//						{
//							Main.tile.CreateTileAt(i, j);
//						}
//					}
//			}
			WorldModify.lastMaxTilesX = Main.maxTilesX;
			WorldModify.lastMaxTilesY = Main.maxTilesY;

            if (Main.tile == null || Main.tile.SizeX != Main.maxTilesX || Main.tile.SizeY != Main.maxTilesY)
			{
                Main.tile = null;
				GC.Collect ();
                Main.tile = new TileCollection(Main.maxTilesX, Main.maxTilesY);
			}
			else
			{
				using (var prog = new ProgressLogger(Main.maxTilesX - 1, "Clearing world tiles"))
					for (int k = 0; k < Main.maxTilesX; k++)
					{
						prog.Value = k;
						for (int l = 0; l < Main.maxTilesY; l++)
						{
							Main.tile.CreateTileAt(k, l);
// for testing
//							if ((l == 25 || l == 26) && k > 4000 && k < 5000)
//							{
//								Main.tile.At(k, l).SetLiquid (250);
//								Liquid.AddWater (k, l);
//							}
						}
					}
			}
			
			using (var prog = new ProgressLogger(12212 + Liquid.resLiquid - 1, "Resetting game objects"))
			{
				for (int num3 = 0; num3 < 201; num3++)
				{
					Main.item[num3] = null;
					Main.npcs[num3] = null;
				}
				prog.Value++;
				
				for (int num4 = 0; num4 < 1000; num4++)
				{
					Main.projectile[num4] = null;
					Main.chest[num4] = null;
					Main.sign[num4] = null;
				}
				prog.Value++;
				
//				for (int num8 = 0; num8 < Liquid.resLiquid; num8++)
//				{
//					Main.liquid[num8] = null;
//				}
//				prog.Value++;
				
//				for (int num9 = 0; num9 < 10000; num9++)
//				{
//					Main.liquidBuffer[num9] = null;
//				}
//				prog.Value += 10;
				
				GC.Collect ();
				prog.Value += 10;

				for (int num3 = 0; num3 < 201; num3++)
				{
					Main.item[num3] = new Item();
					Main.npcs[num3] = new NPC();
					prog.Value++;
				}
				for (int num4 = 0; num4 < 1001; num4++)
				{
					
					prog.Value++;
				}
//				for (int num5 = 0; num5 < 1000; num5++)
//				{
//					Main.projectile[num5] = new Projectile();
//					prog.Value++;
//				}
				Projectile.ResetProjectiles ();
				prog.Value += 1000;
				for (int num8 = 0; num8 < Liquid.resLiquid; num8++)
				{
					Main.liquid[num8] = new Liquid();
					prog.Value++;
				}
				for (int num9 = 0; num9 < 10000; num9++)
				{
					Main.liquidBuffer[num9] = new LiquidBuffer();
					prog.Value++;
				}
			}
			setWorldSize();
			
			LiquidUpdateBuffer.Initialize (Main.maxSectionsX, Main.maxSectionsY);
			
			worldCleared = true;
		}

        public static bool saveWorld(string savePath, bool resetTime = false)
		{
            bool success = true;

			if (savePath == null)
			{
				return false;
			}

			if (WorldModify.saveLock)
			{
				return false;
			}

            try
            {
				WorldModify.saveLock = true;
				lock (padlock)
				{
					bool value = Main.dayTime;
					tempTime = Main.time;
					tempMoonPhase = Main.moonPhase;
					tempBloodMoon = Main.bloodMoon;

					if (resetTime)
					{
						value = true;
						tempTime = 13500.0;
						tempMoonPhase = 0;
						tempBloodMoon = false;
					}

					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
                    string tempPath = savePath + ".sav";
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
							binaryWriter.Write(tempTime);
							binaryWriter.Write(value);
							binaryWriter.Write(tempMoonPhase);
							binaryWriter.Write(tempBloodMoon);
							binaryWriter.Write(Main.dungeonX);
							binaryWriter.Write(Main.dungeonY);
							binaryWriter.Write(NPC.downedBoss1);
							binaryWriter.Write(NPC.downedBoss2);
							binaryWriter.Write(NPC.downedBoss3);
							binaryWriter.Write(WorldModify.shadowOrbSmashed);
							binaryWriter.Write(WorldModify.spawnMeteor);
							binaryWriter.Write((byte)WorldModify.shadowOrbCount);
							binaryWriter.Write(Main.invasionDelay);
							binaryWriter.Write(Main.invasionSize);
							binaryWriter.Write(Main.invasionType);
							binaryWriter.Write(Main.invasionX);

							using (var prog = new ProgressLogger(Main.maxTilesX - 1, "Saving world data"))
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
										if (chest.contents[l].Type == 0)
										{
											chest.contents[l].Stack = 0;
										}
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
							for (int i = 0; i < Main.npcs.Length; i++)
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
								ProgramLog.Log("Backing up world file...");
                                string destFileName = savePath + ".bak";
								File.Copy(savePath, destFileName, true);
								try
								{
									File.Delete(destFileName);
								}
								catch (Exception e)
								{
									ProgramLog.Log(e, "Exception removing " + destFileName);
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
							ProgramLog.Log(e, "Exception moving " + tempPath);
						}

						try
						{
							File.Delete(tempPath);
						}
						catch (Exception e)
						{
							ProgramLog.Log(e, "Exception removing " + tempPath);
						}
					}
					stopwatch.Stop();
					ProgramLog.Log("Save duration: " + stopwatch.Elapsed.Seconds + " Second(s)");
				}
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Exception saving the world");
                success = false;
            }
            finally
            {
                WorldModify.saveLock = false;
            }

			return success;
		}

		public static void LoadWorld (string LoadPath)
		{
			using (FileStream fileStream = new FileStream(LoadPath, FileMode.Open))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					try
					{
						WorldModify.loadFailed = false;
						WorldModify.loadSuccess = false;
						int Terraria_Release = binaryReader.ReadInt32();
						if (Terraria_Release > Statics.CURRENT_TERRARIA_RELEASE)
						{
							WorldModify.loadFailed = true;
							WorldModify.loadSuccess = false;
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
							clearWorld();
							Main.spawnTileX = binaryReader.ReadInt32();
							Main.spawnTileY = binaryReader.ReadInt32();
							Main.worldSurface = binaryReader.ReadDouble();
							Main.rockLayer = binaryReader.ReadDouble();
							tempTime = binaryReader.ReadDouble();
							tempDayTime = binaryReader.ReadBoolean();
							tempMoonPhase = binaryReader.ReadInt32();
							tempBloodMoon = binaryReader.ReadBoolean();
							Main.dungeonX = binaryReader.ReadInt32();
							Main.dungeonY = binaryReader.ReadInt32();
							NPC.downedBoss1 = binaryReader.ReadBoolean();
							NPC.downedBoss2 = binaryReader.ReadBoolean();
							NPC.downedBoss3 = binaryReader.ReadBoolean();
							WorldModify.shadowOrbSmashed = binaryReader.ReadBoolean();
							WorldModify.spawnMeteor = binaryReader.ReadBoolean();
							WorldModify.shadowOrbCount = (int)binaryReader.ReadByte();
							Main.invasionDelay = binaryReader.ReadInt32();
							Main.invasionSize = binaryReader.ReadInt32();
							Main.invasionType = binaryReader.ReadInt32();
							Main.invasionX = binaryReader.ReadDouble();

							using (var prog = new ProgressLogger(Main.maxTilesX - 1, "Loading world tiles"))
							{
								for (int j = 0; j < Main.maxTilesX; j++)
								{
									prog.Value = j;

									for (int k = 0; k < Main.maxTilesY; k++)
									{
										Main.tile.At(j, k).SetActive(binaryReader.ReadBoolean());
										if (Main.tile.At(j, k).Active)
										{
											Main.tile.At(j, k).SetType(binaryReader.ReadByte());
											if (Main.tileFrameImportant[(int)Main.tile.At(j, k).Type])
											{
												Main.tile.At(j, k).SetFrameX(binaryReader.ReadInt16());
												Main.tile.At(j, k).SetFrameY(binaryReader.ReadInt16());
											}
											else
											{
												Main.tile.At(j, k).SetFrameX(-1);
												Main.tile.At(j, k).SetFrameY(-1);
											}
										}
										Main.tile.At(j, k).SetLighted(binaryReader.ReadBoolean());
										if (binaryReader.ReadBoolean())
										{
											Main.tile.At(j, k).SetWall(binaryReader.ReadByte());
//											if (Main.tile.At(j, k).Wall == 7)
//												Main.tile.At(j, k).SetWall (17);
										}
										if (binaryReader.ReadBoolean())
										{
											Main.tile.At(j, k).SetLiquid(binaryReader.ReadByte());
											Main.tile.At(j, k).SetLava(binaryReader.ReadBoolean());
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
                                            string defaults = Item.VersionName(binaryReader.ReadString(), Terraria_Release);
											Main.chest[l].contents[m] = Registries.Item.Create(defaults, stack);
										}
									}
								}
							}
							for (int n = 0; n < 1000; n++)
							{
								if (binaryReader.ReadBoolean())
								{
                                    string text = binaryReader.ReadString();
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
                                string NPCName = binaryReader.ReadString();
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
                                string a = binaryReader.ReadString();
								int num6 = binaryReader.ReadInt32();
								if (!flag2 || !(a == Main.worldName) || num6 != Main.worldID)
								{
									WorldModify.loadSuccess = false;
									WorldModify.loadFailed = true;
									binaryReader.Close();
									fileStream.Close();
									return;
								}
								WorldModify.loadSuccess = true;
							}
							else
							{
								WorldModify.loadSuccess = true;
							}
							binaryReader.Close();
							fileStream.Close();

							if (!WorldModify.loadFailed && WorldModify.loadSuccess)
							{
								WorldModify.gen = true;
								WorldModify.waterLine = Main.maxTilesY;
								Liquid.QuickWater(2, -1, -1);
								WorldModify.WaterCheck();
								int num7 = 0;
								Liquid.quickSettle = true;
								int num8 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
								float num9 = 0f;

								using (var prog = new ProgressLogger(100, "Settling liquids"))
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

										prog.Value = (int)(num10 * 50f + 50f);

										Liquid.UpdateLiquid();
									}
								Liquid.quickSettle = false;

								ProgramLog.Log("Performing Water Check");
								WorldModify.WaterCheck();
								WorldModify.gen = false;
							}
						}
					}
					catch
					{
						WorldModify.loadFailed = true;
						WorldModify.loadSuccess = false;
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
				}
			}
			
			if (Main.worldName == null || Main.worldName == "")
			{
				Main.worldName = System.IO.Path.GetFileNameWithoutExtension (LoadPath);
			}
			
			Statics.WorldLoaded = true;
			
			PluginManager.NotifyWorldLoaded ();
			
			var ctx = new HookContext
			{
			};
			
			var args = new HookArgs.WorldLoaded
			{
				Height = Main.maxTilesY,
				Width  = Main.maxTilesX,
			};
			
			HookPoints.WorldLoaded.Invoke (ref ctx, ref args);
		}
	}
}
