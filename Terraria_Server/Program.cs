using System;
using System.Threading;
using System.IO;
using System.Diagnostics;

using Terraria_Server.Commands;
using System.Collections;

namespace Terraria_Server
{
    public class Program
    {
        public static Thread updateThread = null;
        public static Properties properties = null;
        public static CommandParser commandParser = null;

        static bool createDirectory(string dirPath)
        {
            if (!System.IO.Directory.Exists(dirPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    return false;
                }
            }
            return true;
        }

        static bool createFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Create(filePath).Close();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    return false;
                }
            }
            return true;
        }

        static bool setupPaths()
        {
            if (!createDirectory(Statics.getWorldPath))
            {
                return false;
            }
            if (!createDirectory(Statics.getPlayerPath))
            {
                return false;
            }
            if (!createDirectory(Statics.getPluginPath))
            {
                return false;
            }
            if (!createDirectory(Statics.getDataPath))
            {
                return false;
            }
            createFile(Statics.getDataPath + Statics.systemSeperator + "whitelist.txt");
            //createFile(Statics.getDataPath + Statics.systemSeperator + "joinedplayers.txt");
            createFile(Statics.getDataPath + Statics.systemSeperator + "banlist.txt");
            return true;
        }

        static void setupProperties()
        {
            properties = new Properties("server.properties");
            properties.Load();
            properties.pushData();
            properties.Save();
        }

        static int preserve = 0;
        public static void printData(string dataText)
        {
            if (Statics.platform > 0)
            {
                Console.WriteLine(dataText);
            }
            else
            {
                for (int i_ = 0; i_ < preserve; i_++)
                {
                    Console.Write("\b");
                }
                Console.Write(dataText);
                preserve = dataText.Length;
            }
        }

        public static string mergeStrArray(string[] Array)
        {
            string ReT = "";
            if (Array != null && Array.Length > 0)
            {
                for (int i = 0; i < Array.Length; i++)
                {
                    if (Array[i] != null)
                    {
                        ReT += " " + Array[i];
                    }
                }
            }
            return ReT.Trim();
        }

        public static Server server;

        static void Main(string[] args)
        {
            Console.Title = "Terraria's Dedicated Server Mod. (" + Statics.versionNumber + " {" + Statics.currentRelease + "})";

            if(Statics.isLinux)
            {
                Console.WriteLine("Detected Linux OS.");
                Statics.systemSeperator = "/";
                Statics.platform = 1;
            } //if mac...erm i've never used it, Google later?

            Console.WriteLine("Setting up Paths.");
            if (!setupPaths())
            {
                return;
            }
            Console.WriteLine("Setting up Properties.");
            setupProperties();
             
            Console.WriteLine("Preparing Server Data...");

            string worldFile = properties.getInitialWorldPath();
            FileInfo file = new FileInfo(worldFile);

            if (!file.Exists)
            {
                try
                {
                    file.Directory.Create();

                }
                catch (Exception exception) {
                    Console.WriteLine(exception.ToString());
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    return;
                }
                Console.WriteLine("Generating World '" + worldFile + "'");

                int seed = properties.getSeed();
                if (seed == -1)
                {
                    Console.Write("Generating Seed...");
                    seed = new Random().Next(100);
                    Console.Write(seed.ToString() + "\n");
                }

                int worldX = properties.getMapSizes()[0];
                int worldY = properties.getMapSizes()[1];
                if (properties.isUsingCutomTiles())
                {
                    int X = properties.getMaxTilesX();
                    int Y = properties.getMaxTilesY();
                    if (X > 0 && Y > 0)
                    {
                        worldX = X;
                        worldY = Y;
                    }

                    Console.WriteLine("Generating World with Custom Map Size { " + worldX.ToString() +
                        ", " + worldY.ToString() + " }");
                }

                Server.maxTilesX = worldX;
                Server.maxTilesY = worldY;
                Server.tile = new Tile[Server.maxTilesX+1, Server.maxTilesY+1];

                WorldGen.clearWorld();
                (new Server()).Initialize();
                WorldGen.generateWorld(seed);
                WorldGen.saveWorld(worldFile, true);
            }
            
            int worldXtiles = properties.getMapSizes()[0];
            int worldYtiles = properties.getMapSizes()[1];
            if (properties.isUsingCutomTiles())
            {
                int X = properties.getMaxTilesX();
                int Y = properties.getMaxTilesY();
                if (X > 0 && Y > 0)
                {
                    worldXtiles = X;
                    worldYtiles = Y;
                }
                Console.WriteLine("Using World with Custom Map Size { " + worldXtiles.ToString() +
                    ", " + worldYtiles.ToString() + " }");
            }


            World world = new World(worldXtiles, worldYtiles);
            world.setSavePath(worldFile);

            server = new Server(world, properties.getMaxPlayers(), 
                Statics.getDataPath + Statics.systemSeperator + "whitelist.txt",
                Statics.getDataPath + Statics.systemSeperator + "banlist.txt");
            server.setOpPassword(properties.getOpPassword());
            server.setPort(properties.getPort());
            server.setIP(properties.getServerIP());
            server.Initialize();

            WorldGen.loadWorld();
            server.StartServer();
            
            updateThread = new Thread(Program.Updater);
            
            Statics.IsActive = true;
            while (!Statics.serverStarted) { }

            commandParser = new CommandParser(server);
            Console.WriteLine("You can now insert Commands.");
            while (Statics.IsActive)
            {
                string line = Console.ReadLine().Trim().ToLower();
                if (line.Length > 0)
                {
                    commandParser.parseConsoleCommand(line, server);
                }

            }
            while (Statics.serverStarted) { }
            Console.WriteLine("Exiting...");
            //Console.ReadKey(true);
        }

        public static void Updater()
        {
            if (server == null)
            {
                Console.WriteLine("Issue in updater thread!");
                return;
            }

	        Stopwatch stopwatch = new Stopwatch();
            double num6 = 16.666666666666668;
	        stopwatch.Start();
	        double num7 = 0.0;

	        while (Statics.IsActive)
	        {
		        double num8 = (double)stopwatch.ElapsedMilliseconds + num7;
		        if (num8 >= num6)
		        {
			        num7 = num8 - num6;
			        stopwatch.Reset();
			        stopwatch.Start();

			        server.Update();

			        float num9 = (float)stopwatch.ElapsedMilliseconds;
			        if ((double)num9 < num6)
			        {
				        int num10 = (int)(num6 - (double)num9) - 1;
				        if (num10 > 1)
				        {
					        Thread.Sleep(num10);
				        }
			        }
		        }
	        }
        }
    }
}
