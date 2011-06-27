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
        public static ServerProperties properties = null;
        public static CommandParser commandParser = null;
        public static TConsole tConsole = null;

        public static bool createDirectory(string dirPath, bool Exit = false)
        {
            if (!System.IO.Directory.Exists(dirPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                catch (Exception exception)
                {
                    if (!Exit)
                    {
                        Program.tConsole.WriteLine(exception.ToString());
                        Program.tConsole.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool createFile(string filePath, bool Exit = false)
        {
            if (!System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Create(filePath).Close();
                }
                catch (Exception exception)
                {
                    if (!Exit)
                    {
                        Program.tConsole.WriteLine(exception.ToString());
                        Program.tConsole.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        return false;
                    }
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
            createFile(Statics.getDataPath + Statics.systemSeperator + "banlist.txt");
            createFile(Statics.getDataPath + Statics.systemSeperator + "oplist.txt");
            createFile(Statics.getDataPath + Statics.systemSeperator + "server.log");
            return true;
        }

        static void setupProperties()
        {
            properties = new ServerProperties("server.properties");
            properties.Load();
            properties.pushData();
            properties.Save();
        }

        static int preserve = 0;
        public static void printData(string dataText, bool console = false)
        {
            if (Statics.platform > 0)
            {
                if (console == false)
                {
                    tConsole.WriteLine(dataText);
                }
                else
                {
                    Console.WriteLine(dataText);
                }
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
            try
            {
                string MODInfo = "Terraria's Dedicated Server Mod. (" + Statics.versionNumber + " {" + Statics.currentRelease + "}) #"
                    + Statics.build; //+ " r" + Statics.revision;
                Console.Title = MODInfo;

                Console.WriteLine("Initializing " + MODInfo);
                
                Console.WriteLine("Setting up Paths.");
                if (!setupPaths())
                {
                    return;
                }

                if (Statics.isLinux)
                {
                    Console.WriteLine("Detected Linux OS.");
                    Statics.platform = 1;
                }
                else if (Statics.isMac)
                {
                    Console.WriteLine("Detected Mac OS.");
                    Statics.platform = 2;
                }
                else if (Statics.isWindows == false)
                {
                    Console.WriteLine("Unknown OS.");
                    Statics.platform = 3;
                }

                tConsole = new TConsole(Statics.getDataPath + Statics.systemSeperator + "server.log", Statics.platform);

                if (args != null && args.Length > 0)
                {
                    string CmdMessage = args[0].Trim();
                    if (CmdMessage.Length > 0)
                    {
                        // 0 for Ops
                        if (CmdMessage.ToLower().Equals("-ignoremessages:0"))
                        {
                            Statics.cmdMessages = false;
                        }
                    }
                }

                Program.tConsole.WriteLine("Setting up Properties.");
                if (!System.IO.File.Exists("server.properties"))
                {
                    Console.Write("Properties not found, Create and exit? [Y/n]: ");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        setupProperties();
                        Console.WriteLine("Complete, Press any Key to Exit...");
                        Console.ReadKey(true);
                        return;
                    }
                }
                setupProperties();

                try
                {
                    if (UpdateManager.performProcess())
                    {
                        Program.tConsole.WriteLine("Restarting into new update!");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Program.tConsole.WriteLine("Error updating!");
                    Program.tConsole.WriteLine(e.Message);
                }

                Statics.debugMode = properties.debugMode();
                if (Statics.debugMode)
                {
                    Program.tConsole.WriteLine("CAUTION: Running Debug Mode! Unexpected errors may occur!");
                }

                Program.tConsole.WriteLine("Preparing Server Data...");

                string worldFile = properties.getInitialWorldPath();
                FileInfo file = new FileInfo(worldFile);

                if (!file.Exists)
                {
                    try
                    {
                        file.Directory.Create();

                    }
                    catch (Exception exception)
                    {
                        Program.tConsole.WriteLine(exception.ToString());
                        Program.tConsole.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        return;
                    }
                    Program.tConsole.WriteLine("Generating World '" + worldFile + "'");

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

                        if (worldX < (int)World.MAP_SIZE.SMALL_X || worldY < (int)World.MAP_SIZE.SMALL_Y)
                        {
                            Program.tConsole.WriteLine("World dimensions need to be equal to or larger than " + (int)World.MAP_SIZE.SMALL_X + " by " + (int)World.MAP_SIZE.SMALL_Y + "; using built-in 'small'");
                            worldX = (int)((int)World.MAP_SIZE.SMALL_Y * 3.5);
                            worldY = (int)World.MAP_SIZE.SMALL_Y;
                        }

                        Program.tConsole.WriteLine("Generating World with Custom Map Size { " + worldX.ToString() +
                            ", " + worldY.ToString() + " }");
                    }

                    Server.maxTilesX = worldX;
                    Server.maxTilesY = worldY;
                    Server.tile = new Tile[Server.maxTilesX + 1, Server.maxTilesY + 1];

                    WorldGen.clearWorld();
                    (new Server()).Initialize();
                    if (properties.getUsingCustomGenOpts())
                    {
                        WorldGen.numDungeons = properties.getDungeonAmount();
                        WorldGen.ficount = properties.getFloatingIslandAmount();
                    }
                    else
                    {
                        WorldGen.numDungeons = 1;
                        WorldGen.ficount = (int)((double)Server.maxTilesX * 0.0008); //The Statics one was generating with default values, We want it to use the actual tileX for the world
                    }
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

                    if (worldXtiles < (int)World.MAP_SIZE.SMALL_X || worldYtiles < (int)World.MAP_SIZE.SMALL_Y)
                    {
                        Program.tConsole.WriteLine("World dimensions need to be equal to or larger than " + (int)World.MAP_SIZE.SMALL_X + " by " + (int)World.MAP_SIZE.SMALL_Y + "; using built-in 'small'");
                        worldXtiles = (int)((int)World.MAP_SIZE.SMALL_Y * 3.5);
                        worldYtiles = (int)World.MAP_SIZE.SMALL_Y;
                    }

                    Program.tConsole.WriteLine("Using World with Custom Map Size { " + worldXtiles.ToString() +
                        ", " + worldYtiles.ToString() + " }");
                }


                World world = new World(worldXtiles, worldYtiles);
                world.setSavePath(worldFile);

                server = new Server(world, properties.getMaxPlayers(),
                    Statics.getDataPath + Statics.systemSeperator + "whitelist.txt",
                    Statics.getDataPath + Statics.systemSeperator + "banlist.txt",
                    Statics.getDataPath + Statics.systemSeperator + "oplist.txt");
                server.setOpPassword(properties.getServerPassword());
                server.setPort(properties.getPort());
                server.setIP(properties.getServerIP());
                server.Initialize();

                WorldGen.loadWorld();

                tConsole.WriteLine("Starting the Server");
                server.StartServer();

                updateThread = new Thread(Program.Updater);

                Statics.IsActive = true;
                while (!Statics.serverStarted) { }

                commandParser = new CommandParser(server);
                Program.tConsole.WriteLine("You can now insert Commands.");
                while (Statics.IsActive)
                {
                    try {
						string line = Console.ReadLine().Trim().ToLower();
	                    if (line.Length > 0)
	                    {
	                        commandParser.parseConsoleCommand(line, server);
	                    }
                    }
                    catch (Exception)
                    {
                        Program.tConsole.WriteLine("Issue parsing Console Command");
					}

                }
                while (Statics.serverStarted) { }
                Program.tConsole.WriteLine("Exiting...");
            }
            catch (Exception e)
            {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(Statics.getDataPath + Statics.systemSeperator + "crashlog.txt", true))
                    {
                        streamWriter.WriteLine(DateTime.Now);
                        streamWriter.WriteLine("Crash Log Generated by TDSM #" + Statics.build + " for " + //+ " r" + Statics.revision + " for " +
                            Statics.versionNumber + " {" + Statics.currentRelease + "}");
                        streamWriter.WriteLine(e);
                        streamWriter.WriteLine("");
                    }
                    Program.tConsole.WriteLine("Server crash: " + DateTime.Now);
                    Program.tConsole.WriteLine(e.Message);
                    Program.tConsole.WriteLine(e.StackTrace);
                    Program.tConsole.WriteLine(e.InnerException.Message);
                    Program.tConsole.WriteLine("");
                    Program.tConsole.WriteLine("Please send crashlog.txt to http://tdsm.org/");
                }
                catch
                {
                    //Program.tConsole.WriteLine("Lol You crashed your crash log, Good work.");
                }
            }
            if (Program.tConsole != null)
            {
                Program.tConsole.Close();
            }
        }

        //public static void Updater()
        //{
        //    if (server == null)
        //    {
        //        Program.tConsole.WriteLine("Issue in updater thread!");
        //        return;
        //    }

        //    Stopwatch stopwatch = new Stopwatch();
        //    double num6 = 16.666666666666668;
        //    stopwatch.Start();
        //    double num7 = 0.0;

        //    if (Server.rand == null)
        //    {
        //        Server.rand = new Random((int)DateTime.Now.Ticks);
        //    }

        //    while (Statics.IsActive)
        //    {
        //        double num8 = (double)stopwatch.ElapsedMilliseconds + num7;
        //        if (num8 >= num6)
        //        {
        //            num7 = num8 - num6;
        //            stopwatch.Reset();
        //            stopwatch.Start();

        //            server.Update();

        //            float num9 = (float)stopwatch.ElapsedMilliseconds;
        //            if ((double)num9 < num6)
        //            {
        //                int num10 = (int)(num6 - (double)num9) - 1;
        //                if (num10 > 1)
        //                {
        //                    Thread.Sleep(num10);
        //                }
        //            }
        //        }
        //    }
        //}

        public static void Updater()
        {
            if (server == null)
            {
                Program.tConsole.WriteLine("Issue in updater thread!");
                return;
            }

            if (Server.rand == null)
            {
               Server.rand = new Random((int)DateTime.Now.Ticks);
            }

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            double num6 = 16.666666666666668;
            double num7 = 0.0;
            while (!Netplay.disconnect)
            {
                double num8 = (double)stopwatch.ElapsedMilliseconds;
                if (num8 + num7 >= num6)
                {
                    num7 += num8 - num6;
                    stopwatch.Reset();
                    stopwatch.Start();

                    if (num7 > 1000.0)
                    {
                        num7 = 1000.0;
                    }
                    if (Netplay.anyClients)
                    {
                        server.Update();
                    }
                    double num9 = (double)stopwatch.ElapsedMilliseconds + num7;
                    if (num9 < num6)
                    {
                        int num10 = (int)(num6 - num9) - 1;
                        if (num10 > 1)
                        {
                            Thread.Sleep(num10);
                            if (!Netplay.anyClients)
                            {
                                num7 = 0.0;
                                Thread.Sleep(10);
                            }
                        }
                    }
                }
                Thread.Sleep(0);
            }
        }
    
    }
}
