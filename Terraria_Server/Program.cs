

namespace Terraria_Server
{
    using System;
    using System.Threading;
    using System.IO;
    using Terraria_Server.Commands;

    internal class Program
    {

        public static Thread updateThread = null;
        public static Properties properties = null;
        public static CommandParser commandParser = null;

        static void setupPaths()
        {
            if (!System.IO.Directory.Exists(Statics.getWorldPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(Statics.getWorldPath);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    Console.WriteLine("Press any key to continue . . . ");
                    Console.ReadKey(true);
                    return;
                }
            }
            if (!System.IO.Directory.Exists(Statics.getPlayerPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(Statics.getPlayerPath);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    Console.WriteLine("Press any key to continue . . . ");
                    Console.ReadKey(true);
                    return;
                }
            }
            if (!System.IO.Directory.Exists(Statics.getPluginPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(Statics.getPluginPath);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    Console.WriteLine("Press any key to continue . . . ");
                    Console.ReadKey(true);
                    return;
                }
            }
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
            for (int i_ = 0; i_ < preserve; i_++)
            {
                Console.Write("\b");
            }
            Console.Write(dataText);
            preserve = dataText.Length;
        }

        static void Main(string[] args)
        {

            if(Statics.isLinux)
            {
                Console.WriteLine("Detected Linux OS.");
                Statics.systemSeperator = "/";
            } //if mac...erm i've never used it, Goolge later?

            Console.WriteLine("Setting up Paths.");
            setupPaths();
            Console.WriteLine("Setting up Properties.");
            setupProperties();


            //[port] [pass] [player cap] [seed]
            //config for world [name] [size] (and/or above)

            //Generate World
            //try
            //{
            string worldFile = properties.getInitialWorldPath(); //Statics.WorldPath + "World1.wld";
            FileInfo file = new FileInfo(worldFile);

            if (!file.Exists)
            {
                try
                {
                    file.Directory.Create();

                }
                catch (Exception exception) {
                    Console.WriteLine(exception.ToString());
                    Console.WriteLine("Press any key to continue . . . ");
                    Console.ReadKey(true);
                    return;
                }
                Console.WriteLine("Generating World '" + worldFile + "'");
                int seed = new Random().Next(100);
                World wor2 = new World((int)World.MAP_SIZE.SMALL_X, (int)World.MAP_SIZE.SMALL_Y);
                Server server2 = new Server(40, wor2);
                WorldGen.clearWorld(wor2);
                server2.Initialize();
                wor2 = WorldGen.generateWorld((int)World.MAP_SIZE.SMALL_X, (int)World.MAP_SIZE.SMALL_Y, seed, wor2);
                wor2.setSavePath(worldFile);
                WorldGen.saveWorld(wor2, true);
            }
            
            Console.WriteLine("Preparing Server Data...");
            World world = new World(8400, 2400);
            Server server = new Server(40, world);
            
            server.Initialize();
            server.setWorld(WorldGen.loadWorld(worldFile, server));
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
            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        public static void Updater(object dataObject)
        {
            Server server = null;
            if (dataObject is Server)
            {
                server = (Server)dataObject;
            }
            if (server == null)
            {
                Console.WriteLine("Issue in updater thread!");
                return;
            }

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(16.666666666666668);
            DateTime d = DateTime.Now;
            while (Statics.IsActive)
            {
                DateTime now = DateTime.Now;
                TimeSpan timeSpan2 = DateTime.Now - d;
                if (timeSpan2 > timeSpan)
                {
                    server.Update();
                    d = now;
                }
                else
                {
                    Thread.Sleep(timeSpan - timeSpan2);
                }
            }
            Console.WriteLine("Exited updater thread!");
        }
    }
}
