using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if Full_API
using Terraria;
#endif

namespace tdsm.api.Callbacks
{
    public static class WorldFile
    {
        public static void loadWorld()
		{
			#if Full_API
            Main.WorldLoadBegin();
            Terraria.Main.checkXMas();
            Terraria.Main.checkHalloween();
            if (!File.Exists(Terraria.Main.worldPathName) && Terraria.Main.autoGen)
            {
                for (int i = Terraria.Main.worldPathName.Length - 1; i >= 0; i--)
                {
                    if (Terraria.Main.worldPathName.Substring(i, 1) == string.Concat(Path.DirectorySeparatorChar))
                    {
                        string path = Terraria.Main.worldPathName.Substring(0, i);
                        Directory.CreateDirectory(path);
                        break;
                    }
                }
                WorldGen.clearWorld();
                WorldGen.generateWorld(-1);
                Terraria.WorldFile.saveWorld(false);
            }
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
            }
            var p = Terraria.Main.worldPathName;
            using (FileStream fileStream = new FileStream(p, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    try
                    {
                        WorldGen.loadFailed = false;
                        WorldGen.loadSuccess = false;
                        int num = binaryReader.ReadInt32();
                        Terraria.WorldFile.versionNumber = num;
                        int num2;
                        if (num <= 87)
                        {
                            num2 = Terraria.WorldFile.LoadWorld_Version1(binaryReader);
                        }
                        else
                        {
                            num2 = Terraria.WorldFile.LoadWorld_Version2(binaryReader);
                        }
                        binaryReader.Close();
                        fileStream.Close();
                        if (num2 != 0)
                        {
                            WorldGen.loadFailed = true;
                        }
                        else
                        {
                            WorldGen.loadSuccess = true;
                        }
                        if (!WorldGen.loadFailed && WorldGen.loadSuccess)
                        {
                            WorldGen.gen = true;
                            WorldGen.waterLine = Terraria.Main.maxTilesY;
                            Liquid.QuickWater(2, -1, -1);
                            WorldGen.WaterCheck();
                            int num3 = 0;
                            Liquid.quickSettle = true;
                            int num4 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                            float num5 = 0f;
                            while (Liquid.numLiquid > 0 && num3 < 100000)
                            {
                                num3++;
                                float num6 = (float)(num4 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num4;
                                if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num4)
                                {
                                    num4 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                                }
                                if (num6 > num5)
                                {
                                    num5 = num6;
                                }
                                else
                                {
                                    num6 = num5;
                                }
                                Terraria.Main.statusText = string.Concat(new object[]
						{
							Lang.gen[27],
							" ",
							(int)(num6 * 100f / 2f + 50f),
							"%"
						});
                                Liquid.UpdateLiquid();
                            }
                            Liquid.quickSettle = false;
                            Terraria.Main.weatherCounter = WorldGen.genRand.Next(3600, 18000);
                            Cloud.resetClouds();
                            WorldGen.WaterCheck();
                            WorldGen.gen = false;
                            NPC.setFireFlyChance();
                            Main.WorldLoadEnd();
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
			#endif
        }
	}
}
