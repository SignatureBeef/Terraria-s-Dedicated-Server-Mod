using System.Threading;
using Terraria_Server.Commands;
using System;
using System.IO;
using System.Diagnostics;

namespace Terraria_Server
{
    public class Program
    {
        public const String VERSION_NUMBER = "v1.0.5";

        public static Thread updateThread = null;
        public static ServerProperties properties = null;
        public static CommandParser commandParser = null;
        public static TConsole tConsole = null;
        private static int preserve = 0;

        public static Server server;

        public static void Main(String[] args)
        {
            try
            {
                String MODInfo = "Terraria's Dedicated Server Mod. (" + VERSION_NUMBER + " {" + Statics.CURRENT_TERRARIA_RELEASE + "}) #"
                    + Statics.BUILD;
                Console.Title = MODInfo;

                Console.WriteLine("Initializing " + MODInfo);

                Console.WriteLine("Until this notice is gone, Please make sure the 3.5 framework is supported on this System.");

                if (args != null && args.Length > 0)
                {
                    String commandMessage = args[0].ToLower().Trim();
                    // 0 for Ops
                    if (commandMessage.Equals("-ignoremessages:0"))
                    {
                        Statics.cmdMessages = false;
                    }
                }

                Console.WriteLine("Setting up Paths.");
                if (!SetupPaths())
                {
                    return;
                }

                Platform.InitPlatform();
                tConsole = new TConsole(Statics.DataPath + Path.DirectorySeparatorChar + "server.log", Platform.Type);

                Program.tConsole.WriteLine("Setting up Properties.");
                bool propertiesExist = File.Exists("server.properties");
                SetupProperties();
                
                if(!propertiesExist)
                {
                    Console.Write("New properties file created. Would you like to exit for editing? [Y/n]: ");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        Console.WriteLine("Complete, Press any Key to Exit...");
                        Console.ReadKey(true);
                        return;
                    }
                }


//#if (DEBUG == false)
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
//#endif

                Statics.debugMode = properties.DebugMode;
                if (Statics.debugMode)
                {
                    Program.tConsole.WriteLine("CAUTION: Running Debug Mode! Unexpected errors may occur!");
                }

                Program.tConsole.WriteLine("Preparing Server Data...");

                String worldFile = properties.InitialWorldPath;
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

                    int seed = properties.Seed;
                    if (seed == -1)
                    {
                        Console.Write("Generating Seed...");
                        seed = new Random().Next(100);
                        Console.Write(seed.ToString() + "\n");
                    }

                    int worldX = properties.getMapSizes()[0];
                    int worldY = properties.getMapSizes()[1];
                    if (properties.UseCustomTiles)
                    {
                        int X = properties.MaxTilesX;
                        int Y = properties.MaxTilesY;
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
                    if (properties.UseCustomGenOpts)
                    {
                        WorldGen.numDungeons = properties.DungeonAmount;
                        WorldGen.ficount = properties.FloatingIslandAmount;
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
                if (properties.UseCustomTiles)
                {
                    int X = properties.MaxTilesX;
                    int Y = properties.MaxTilesY;
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
                world.SavePath = worldFile;

                server = new Server(world, properties.MaxPlayers,
                    Statics.DataPath + Path.DirectorySeparatorChar + "whitelist.txt",
                    Statics.DataPath + Path.DirectorySeparatorChar + "banlist.txt",
                    Statics.DataPath + Path.DirectorySeparatorChar + "oplist.txt");
                server.setOpPassword(properties.Password);
                server.setPort(properties.Port);
                server.setIP(properties.ServerIP);
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
                    try
                    {
                        String line = Console.ReadLine().Trim().ToLower();
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
                Program.tConsole.Close();
            }
            catch (Exception e)
            {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(Statics.DataPath + Path.DirectorySeparatorChar + "crashlog.txt", true))
                    {
                        streamWriter.WriteLine(DateTime.Now);
                        streamWriter.WriteLine("Crash Log Generated by TDSM #" + Statics.BUILD + " for " + //+ " r" + Statics.revision + " for " +
                            VERSION_NUMBER + " {" + Statics.CURRENT_TERRARIA_RELEASE + "}");
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
                }
            }
            if (Program.tConsole != null)
            {
                Program.tConsole.Close();
            }
        }

        private static bool SetupPaths()
        {
            try
            {
                CreateDirectory(Statics.WorldPath);
                CreateDirectory(Statics.PlayerPath);
                CreateDirectory(Statics.PluginPath);
                CreateDirectory(Statics.DataPath);
            }
            catch (Exception exception)
            {
                Program.tConsole.WriteLine(exception.ToString());
                Program.tConsole.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return false;
            }

            CreateFile(Statics.DataPath + Path.DirectorySeparatorChar + "whitelist.txt");
            CreateFile(Statics.DataPath + Path.DirectorySeparatorChar + "banlist.txt");
            CreateFile(Statics.DataPath + Path.DirectorySeparatorChar + "oplist.txt");
            CreateFile(Statics.DataPath + Path.DirectorySeparatorChar + "server.log");
            return true;
        }

        private static void CreateDirectory(String dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        private static bool CreateFile(String filePath)
        {
            if (!File.Exists(filePath))
            {
                try
                {
                    File.Create(filePath).Close();
                }
                catch (Exception exception)
                {
                    Program.tConsole.WriteLine(exception.ToString());
                    Program.tConsole.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    return false;
                }
            }
            return true;
        }

        private static void SetupProperties()
        {
            properties = new ServerProperties("server.properties");
            properties.Load();
            properties.pushData();
            properties.Save();
        }

        public static void printData(String dataText, bool console = false)
        {
            if (Platform.Type != Platform.PlatformType.WINDOWS)
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
