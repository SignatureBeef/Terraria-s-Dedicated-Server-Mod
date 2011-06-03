

namespace Terraria_Server
{
    using System;
    using System.Threading;

    internal static class Program
    {

        public static Thread updateThread = null;

        static void Main(string[] args)
        {
            //[port] [pass] [player cap] [seed]
            //config for world [name] [size] (and/or above)

            //Generate World
            //try
            //{
                Console.WriteLine("Preparing...");
                World world = new World(8400, 2400);
                Server server = new Server(40, world);
                Statics.maxTilesX = 8400;
                Statics.maxTilesY = 2400;
                server.Initialize();
                string path = "C:\\Users\\Luke\\Documents\\My Games\\Terraria\\Worlds\\world1.wld";
                WorldGen.loadWorld(path, server);
                /*if (args.Length <= 0)
                {
                    Console.WriteLine("Cannot find directory!");
                    Console.WriteLine("use: *.exe <worldpath>");
                    Console.WriteLine("Press any key to continue . . . ");
                    Console.ReadKey(true);
                    return;
                }*/
                //WorldGen.loadWorld(args[0], server);
                //world.setServer(server);
                server.StartServer();

                //server.Update();
                updateThread = new Thread(Program.doUpdate);

                //Statics.maxTilesX = 6300;
                //Statics.maxTilesY = 1800;
                //Statics.maxTilesX = 8400;
                //Statics.maxTilesY = 2400;

                //Statics.isRunning = true;

                Statics.IsActive = true;
                while (Statics.IsActive)
                {

                }
                //while (Statics.isRunning)
                //{

                //}
            //}
            //catch (Exception exception)
            //{
            //    Console.WriteLine(exception.ToString());
            //}

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        public static void doUpdate(object dataObject)
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
