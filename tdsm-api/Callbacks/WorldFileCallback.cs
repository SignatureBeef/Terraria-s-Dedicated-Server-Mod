//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//#if Full_API
//using Terraria;
//#endif

//namespace tdsm.api.Callbacks
//{
//    public static class WorldFileCallback
//    {
//        public static void clearWorld ()
//        {
//#if Full_API
//            Main.checkXMas ();
//            Main.checkHalloween ();
//            if (Main.mapReady)
//            {
//                for (int i = 0; i < WorldGen.lastMaxTilesX; i++)
//                {
//                    float num = (float)i / (float)WorldGen.lastMaxTilesX;
//                    Main.statusText = string.Concat (new object[]
//                                                     {
//                        Lang.gen [65],
//                        " ",
//                        (int)(num * 100f + 1f),
//                        "%"
//                    });
//                    for (int j = 0; j < WorldGen.lastMaxTilesY; j++)
//                    {
//                        if (Main.Map [i, j] != null)
//                        {
//                            Main.Map[i, j] = null;
//                        }
//                    }
//                }
//            }
//            Main.pumpkinMoon = false;
//            Main.clearMap = true;
//            Main.mapTime = 0;
//            Main.updateMap = false;
//            Main.mapReady = false;
//            Main.refreshMap = false;
//            Main.eclipse = false;
//            NPC.waveKills = 0f;
//            WorldGen.spawnHardBoss = 0;
//            WorldGen.totalSolid2 = 0;
//            WorldGen.totalGood2 = 0;
//            WorldGen.totalEvil2 = 0;
//            WorldGen.totalBlood2 = 0;
//            WorldGen.totalSolid = 0;
//            WorldGen.totalGood = 0;
//            WorldGen.totalEvil = 0;
//            WorldGen.totalBlood = 0;
//            WorldFile.ResetTemps ();
//            Main.maxRaining = 0f;
//            WorldGen.totalX = 0;
//            WorldGen.totalD = 0;
//            WorldGen.tEvil = 0;
//            WorldGen.tBlood = 0;
//            WorldGen.tGood = 0;
//            Main.trashItem = new Item ();
//            WorldGen.spawnEye = false;
//            WorldGen.spawnNPC = 0;
//            WorldGen.shadowOrbCount = 0;
//            WorldGen.altarCount = 0;
//            WorldGen.oreTier1 = -1;
//            WorldGen.oreTier2 = -1;
//            WorldGen.oreTier3 = -1;
//            Main.cloudBGActive = 0f;
//            Main.raining = false;
//            Main.hardMode = false;
//            Main.helpText = 0;
//            Main.dungeonX = 0;
//            Main.dungeonY = 0;
//            NPC.downedBoss1 = false;
//            NPC.downedBoss2 = false;
//            NPC.downedBoss3 = false;
//            NPC.downedQueenBee = false;
//            NPC.downedMechBossAny = false;
//            NPC.downedMechBoss1 = false;
//            NPC.downedMechBoss2 = false;
//            NPC.downedMechBoss3 = false;
//            NPC.downedPlantBoss = false;
//            NPC.savedStylist = false;
//            NPC.savedGoblin = false;
//            NPC.savedWizard = false;
//            NPC.savedMech = false;
//            NPC.downedGoblins = false;
//            NPC.downedClown = false;
//            NPC.downedFrost = false;
//            NPC.downedPirates = false;
//            NPC.savedAngler = false;
//            WorldGen.shadowOrbSmashed = false;
//            WorldGen.spawnMeteor = false;
//            WorldGen.stopDrops = false;
//            Main.invasionDelay = 0;
//            Main.invasionType = 0;
//            Main.invasionSize = 0;
//            Main.invasionWarn = 0;
//            Main.invasionX = 0.0;
//            Main.treeX [0] = Main.maxTilesX;
//            Main.treeX [1] = Main.maxTilesX;
//            Main.treeX [2] = Main.maxTilesX;
//            Main.treeStyle [0] = 0;
//            Main.treeStyle [1] = 0;
//            Main.treeStyle [2] = 0;
//            Main.treeStyle [3] = 0;
//            WorldGen.noLiquidCheck = false;
//            Liquid.numLiquid = 0;
//            LiquidBuffer.numLiquidBuffer = 0;
//            if (Main.netMode == 1 || WorldGen.lastMaxTilesX > Main.maxTilesX || WorldGen.lastMaxTilesY > Main.maxTilesY)
//            {
//                for (int k = 0; k < WorldGen.lastMaxTilesX; k++)
//                {
//                    float num2 = (float)k / (float)WorldGen.lastMaxTilesX;
//                    Main.statusText = string.Concat (new object[]
//                                                     {
//                        Lang.gen [46],
//                        " ",
//                        (int)(num2 * 100f + 1f),
//                        "%"
//                    });
//                    for (int l = 0; l < WorldGen.lastMaxTilesY; l++)
//                    {
//                        Main.tile [k, l] = UserInput.DefaultTile;
//                    }
//                }
//            }
//            WorldGen.lastMaxTilesX = Main.maxTilesX;
//            WorldGen.lastMaxTilesY = Main.maxTilesY;
//            if (Main.netMode != 2)
//            {
//                Main.sectionManager = new WorldSections (Main.maxTilesX / 200, Main.maxTilesY / 150);
//            }
//            if (Main.netMode != 1)
//            {
//                for (int m = 0; m < Main.maxTilesX; m++)
//                {
//                    float num3 = (float)m / (float)Main.maxTilesX;
//                    Main.statusText = string.Concat (new object[]
//                                                     {
//                        Lang.gen [47],
//                        " ",
//                        (int)(num3 * 100f + 1f),
//                        "%"
//                    });
//                    for (int n = 0; n < Main.maxTilesY; n++)
//                    {
//                        if (Main.tile [m, n] != UserInput.DefaultTile)
//                        {
//                            Main.tile [m, n] = new Terraria.Tile ();
//                        }
//                        else
//                        {
//                            Main.tile [m, n].Clear ();
//                        }
//                    }
//                }
//            }
//            for (int num4 = 0; num4 < 6000; num4++)
//            {
//                Main.dust [num4] = new Dust ();
//            }
//            for (int num5 = 0; num5 < 500; num5++)
//            {
//                Main.gore [num5] = new Gore ();
//            }
//            for (int num6 = 0; num6 < 400; num6++)
//            {
//                Main.item [num6] = new Item ();
//            }
//            for (int num7 = 0; num7 < 200; num7++)
//            {
//                Main.npc [num7] = new NPC ();
//            }
//            for (int num8 = 0; num8 < 1000; num8++)
//            {
//                Main.projectile [num8] = new Projectile ();
//            }
//            for (int num9 = 0; num9 < 1000; num9++)
//            {
//                Main.chest [num9] = null;
//            }
//            for (int num10 = 0; num10 < 1000; num10++)
//            {
//                Main.sign [num10] = null;
//            }
//            for (int num11 = 0; num11 < Liquid.resLiquid; num11++)
//            {
//                Main.liquid [num11] = new Liquid ();
//            }
//            for (int num12 = 0; num12 < 10000; num12++)
//            {
//                Main.liquidBuffer [num12] = new LiquidBuffer ();
//            }
//            WorldGen.setWorldSize ();
//            WorldGen.worldCleared = true;
//#endif
//        }

//        public static void loadWorld()
//        {
//            #if Full_API
//            MainCallback.WorldLoadBegin();
//            Terraria.Main.checkXMas();
//            Terraria.Main.checkHalloween();
//            if (!File.Exists(Terraria.Main.worldPathName) && Terraria.Main.autoGen)
//            {
//                for (int i = Terraria.Main.worldPathName.Length - 1; i >= 0; i--)
//                {
//                    if (Terraria.Main.worldPathName.Substring(i, 1) == string.Concat(Path.DirectorySeparatorChar))
//                    {
//                        string path = Terraria.Main.worldPathName.Substring(0, i);
//                        Directory.CreateDirectory(path);
//                        break;
//                    }
//                }
//                clearWorld();
//                WorldGen.generateWorld(-1);
//                Terraria.WorldFile.saveWorld(false);
//            }
//            if (WorldGen.genRand == null)
//            {
//                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
//            }
//            var p = Terraria.Main.worldPathName;
//            using (FileStream fileStream = new FileStream(p, FileMode.Open))
//            {
//                using (BinaryReader binaryReader = new BinaryReader(fileStream))
//                {
//                    try
//                    {
//                        WorldGen.loadFailed = false;
//                        WorldGen.loadSuccess = false;
//                        int num = binaryReader.ReadInt32();
//                        Terraria.WorldFile.versionNumber = num;
//                        int num2;
//                        if (num <= 87)
//                        {
//                            num2 = Terraria.WorldFile.LoadWorld_Version1(binaryReader);
//                        }
//                        else
//                        {
//                            num2 = Terraria.WorldFile.LoadWorld_Version2(binaryReader);
//                        }
//                        binaryReader.Close();
//                        fileStream.Close();
//                        if (num2 != 0)
//                        {
//                            WorldGen.loadFailed = true;
//                        }
//                        else
//                        {
//                            WorldGen.loadSuccess = true;
//                        }
//                        if (!WorldGen.loadFailed && WorldGen.loadSuccess)
//                        {
//                            WorldGen.gen = true;
//                            WorldGen.waterLine = Terraria.Main.maxTilesY;
//                            Liquid.QuickWater(2, -1, -1);
//                            WorldGen.WaterCheck();
//                            int num3 = 0;
//                            Liquid.quickSettle = true;
//                            int num4 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
//                            float num5 = 0f;
//                            while (Liquid.numLiquid > 0 && num3 < 100000)
//                            {
//                                num3++;
//                                float num6 = (float)(num4 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num4;
//                                if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num4)
//                                {
//                                    num4 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
//                                }
//                                if (num6 > num5)
//                                {
//                                    num5 = num6;
//                                }
//                                else
//                                {
//                                    num6 = num5;
//                                }
//                                Terraria.Main.statusText = string.Concat(new object[]
//                        {
//                            Lang.gen[27],
//                            " ",
//                            (int)(num6 * 100f / 2f + 50f)
//                                        ,
//                            "%"
//                        });
//                                Liquid.UpdateLiquid();
//                            }
//                            Liquid.quickSettle = false;
//                            Terraria.Main.weatherCounter = WorldGen.genRand.Next(3600, 18000);
//                            Cloud.resetClouds();
//                            WorldGen.WaterCheck();
//                            WorldGen.gen = false;
//                            NPC.setFireFlyChance();
//                            MainCallback.WorldLoadEnd();
//                        }
//                    }
//                    catch(Exception e)
//                    {
//                        Console.WriteLine(e);
//                        WorldGen.loadFailed = true;
//                        WorldGen.loadSuccess = false;
//                        try
//                        {
//                            binaryReader.Close();
//                            fileStream.Close();
//                        }
//                        catch
//                        {
//                        }
//                    }
//                }
//            }
//            #endif
//        }
//    }
//}
