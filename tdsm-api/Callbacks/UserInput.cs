using System;
using tdsm.api.Command;
using tdsm.api.Plugin;

namespace tdsm.api.Callbacks
{
    public static class Patches
    {
        public static string GetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }
    }

    public static class UserInput
    {
        //        static readonly List<String> _officialCommands = new List<String>() 
        //        {
        //            "help",
        //            "playing",
        //            "clear",
        //            "exit",
        //            "exit-nosave",
        //            "save",
        //            "kick",
        //            "ban",
        //            "password",
        //            "version",
        //            "time",
        //            "port",
        //            "maxplayers",
        //            "say",
        //            "motd",
        //            "dawn",
        //            "noon",
        //            "dusk",
        //            "midnight",
        //            "settle"
        //        };


        public static void ListenForCommands()
        {
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.StartCommandProcessing();
            HookPoints.StartCommandProcessing.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
                System.Threading.ThreadPool.QueueUserWorkItem(startDedInputCallBack);
        }

        static readonly CommandParser _cmdParser = new CommandParser();

        public static CommandParser CommandParser
        {
            get
            { return _cmdParser; }
        }

        public static void startDedInputCallBack(object threadContext)
        {
            System.Threading.Thread.CurrentThread.Name = "APC";

			Tools.WriteLine("Ready for commands.");
#if Full_API
            while (!Terraria.Netplay.disconnect)
            {
                try
                {
                    Console.Write(": ");
                    var input = Console.ReadLine();
                    _cmdParser.ParseConsoleCommand(input);
                }
                catch (ExitException) { }
                catch (Exception e)
                {
                    Tools.WriteLine("Exception from command");
                    Tools.WriteLine(e);
                }
            }
#endif
        }

        //public static int X = 0;
        //public static Tile[,] Tiles;

        public static string ProcessInput(string value)
        {
            return value;
            //            var t = String.Empty;
            //            //int x = 0, y = 0;
            //            //if (Tiles[x, y] == DefaultTile)
            //            //{
            //            //    Tiles[x, y] = DefaultTile;
            //            //}
            //
            //            //Main.tile[0, 0] = default(Tile);
            //            //if (Main.tile[0, 0] == DefaultTile)
            //            //{
            //            //    return "";
            //            //}
            //
            //            //if (Main.netMode == 1 || X > Main.maxTilesX || X > Main.maxTilesY)
            //            //{
            //            //    for (int k = 0; k < X; k++)
            //            //    {
            //            //        float num2 = (float)k / (float)X;
            //            //        for (int l = 0; l < X; l++)
            //            //        {
            //            //            Main.tile[k, l] = default(Tile);
            //            //        }
            //            //    }
            //            //}
            //
            //            if (!String.IsNullOrEmpty(value))
            //            {
            //                var command = value.Split(' ').First().ToLower();
            //                if (_officialCommands.Contains(command) && command != "help") return value;
            //
            //                if (command == "spawnnpc")
            //                {
            //
            //                }
            //                else if (command == "test")
            //                {
            //                    //Tools.WriteLine("Active player count: " + Terraria.Main.player.Where(x => x != null && x.active).Count());
            //                }
            //                else if (command == "help")
            //                {
            //                    Tools.WriteLine("spawnnpc " + '\t' + " Spawn an npc by name or id");
            //                    return command; //Allow printout of defaults
            //                }
            //                else
            //                {
            //                    Tools.WriteLine("Server command does not exist.");
            //                }
            //            }
            //            return t;
        }

        public static readonly Terraria.Tile DefaultTile = default(Terraria.Tile);
        public static Terraria.Tile GetTile()
        {
            return DefaultTile;
        }
        
        public static bool Tile_Equality(Terraria.Tile t1, Terraria.Tile t2)
        {
            return t1.isTheSameAs(t2);
        }

        public static bool Tile_Inequality(Terraria.Tile t1, Terraria.Tile t2)
        {
            return !t1.isTheSameAs(t2);
        }
        
        public static bool TileEquals2(TileData t1, TileData t2)
        {
            for (var x = 0; x < 1; x++)
            {
                for (var y = 0; y < 1; y++)
                {
                    TestAA[x, y] = new TestA();
                }
            }
            for (var x = 0; x < 1; x++)
            {
                for (var y = 0; y < 1; y++)
                {
                    TestBB[x, y] = new TestB();
                }
            }

            return false;
        }

        class TestA {}
        struct TestB {}

        static TestA[,] TestAA;
        static TestB[,] TestBB;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    public struct TileData
    {
        public static bool operator !=(TileData t1, TileData t2)
        {

            return false;
        }

        public static bool operator ==(TileData t1, TileData t2)
        {
            return UserInput.TileEquals2(t1, t2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
