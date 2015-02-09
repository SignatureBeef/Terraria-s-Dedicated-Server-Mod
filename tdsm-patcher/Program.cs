#define SERVER
//#define CLIENT
using System;
using tdsm.api;
using tdsm.api.Plugin;

namespace tdsm.patcher
{
    public class Program
    {
        public const Double Build = 1;
        public static bool IsPatching { get; private set; }

        static void Main(string[] args)
        {
            IsPatching = true;
            Console.WriteLine("TDSM patcher build {0}", Build);
            //var isMono = Type.GetType("Mono.Runtime") != null;

#if SERVER
            var inFile = "TerrariaServer.exe";
            var outFile = "tdsm.exe";
            var patchFile = "tdsm.api.dll";

#elif CLIENT
            var inFile = "Terraria.exe";
            var outFile = "tdcm.exe";
            var patchFile = "Microsoft.Xna.Framework.dll";
#endif
            //            if (!File.Exists(inFile))
            //            {
            //                //Download the official from out github
            //                //using (var wc = new System.Net.WebClient())
            //                //{
            //
            //                //}
            //            }

            var patcher = new Injector(inFile, patchFile);

            var vers = patcher.GetAssemblyVersion();
            if (vers != Globals.TerrariaVersion)
            {
                Console.Write("This patcher only supports Terraria {0}, but we have detected something else {1}. Continue? (y/n)",
                    Globals.TerrariaVersion,
                    vers);
                if (Console.ReadKey(true).Key != ConsoleKey.Y) return;
                Console.WriteLine();
            }
#if SERVER
            Console.Write("Opening up classes for API usage...");
            patcher.MakeTypesPublic(true);
            Console.Write("Ok\nHooking command line...");
            patcher.PatchCommandLine();
            if (Tools.RuntimePlatform == RuntimePlatform.Mono)
            {
                Console.Write("Ok\nRemoving port forwarding functionality...");
                patcher.FixNetplay();
            }
            Console.Write("Ok\nSkipping sysmenus functions...");
            patcher.SkipMenu();
            Console.Write("Ok\nFixing code entry...");
            patcher.FixEntryPoint();
            Console.Write("Ok\nPatching save paths...");
            patcher.FixSavePath();
            //Console.Write("Ok\nPutting Terraria on a diet...");
            //patcher.ChangeTileToStruct();
            Console.Write("Ok\nHooking receive buffer...");
            patcher.HookMessageBuffer();
            Console.Write("Ok\nAdding the slot manager...");
            patcher.PatchServer();
            Console.Write("Ok\nPatching XNA...");
            patcher.PatchXNA(true);
            Console.Write("Ok\nHooking start...");
            patcher.HookProgramStart();
            Console.Write("Ok\nHooking initialise...");
            patcher.HookInitialise();
            Console.Write("Ok\nHooking into world events");
            patcher.HookWorldEvents();
            Console.Write("Ok\nHooking statusText...");
            patcher.HookStatusText();
            Console.Write("Ok\nHooking NetMessage...");
            patcher.HookNetMessage();
            //Console.Write("Ok\nRemoving client code...");
            //patcher.RemoveClientCode();
            Console.Write("Ok\nHooking Server events...");
            patcher.HookUpdateServer();
            Console.Write("Ok\nHooking config...");
            patcher.HookConfig();
            Console.Write("Ok\n");

            //TODO repace Terraria's Console.SetTitles
#elif CLIENT
            Console.Write("Opening up classes for API usage...");
            patcher.MakeTypesPublic(false);
            Console.Write("Ok\nPatching XNA...");
            patcher.PatchXNA(false);
            Console.Write("Ok\n");
#endif

#if Release
            try
            {
#endif
            Globals.Touch();

            PluginManager.SetHookSource(typeof(HookPoints));
            PluginManager.Initialize(Globals.PluginPath, Globals.LibrariesPath);
            PluginManager.LoadPlugins();

            var ctx = new HookContext
            {
                Sender = HookContext.ConsoleSender
            };

            var hookArgs = new HookArgs.PatchServer
            {
                Default = patcher,
#if SERVER
                IsClient = false,
                IsServer = true
#elif CLIENT
                IsClient = true,
                IsServer = false
#endif
            };
            HookPoints.PatchServer.Invoke(ref ctx, ref hookArgs);
#if Release
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception ocurred during PluginManager.LoadPlugins: {0}", e);
            }
#endif

            Console.Write("Saving to {0}...", outFile);
            patcher.Save(outFile);
            Console.WriteLine("Ok\nYou may now run {0} as you would normally.", outFile);

#if Release || true
            Console.WriteLine("Press [y] to run {0}, any other key will exit . . .", outFile);
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
                System.Diagnostics.Process.Start(outFile, "-config serverconfig.txt");
#endif
        }
    }
}
