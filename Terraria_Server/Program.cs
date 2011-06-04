

namespace Terraria_Server
{
    using System;
    using System.Threading;
    using System.IO;

    internal static class Program
    {

        public static Thread updateThread = null;


        void setupPaths()
        {
            FileInfo file = new FileInfo(Statics.WorldPath);

            if (!file.Exists)
            {
                try
                {
                    file.Directory.Create();

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    Console.WriteLine("Press any key to continue . . . ");
                    Console.ReadKey(true);
                    return;
                }
            }
            file = new FileInfo(Statics.PlayerPath);
            if (!file.Exists)
            {
                try
                {
                    file.Directory.Create();

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

        static void Main(string[] args)
        {
            //[port] [pass] [player cap] [seed]
            //config for world [name] [size] (and/or above)

            //Generate World
            //try
            //{
            string worldFile = Statics.WorldPath + "\\World1.wld";
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

                Console.WriteLine("Preparing...");
                World world = new World(8400, 2400);
                Server server = new Server(40, world);
                server.Initialize();

                server.setWorld(WorldGen.loadWorld(worldFile, server));

                server.StartServer();

                //server.Update();
                updateThread = new Thread(Program.Updater);

                Statics.IsActive = true;
                while (!Statics.serverStarted) { }

                Console.WriteLine("You can now insert Commands.");
                while (Statics.IsActive)
                {
                    string line = Console.ReadLine().Trim().ToLower();
                    if (line.Equals("stop") || line.Equals("exit"))
                    {
                        break;
                    }
                    Console.WriteLine(line);
                }
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
            while (true)
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
        }
    }
}
