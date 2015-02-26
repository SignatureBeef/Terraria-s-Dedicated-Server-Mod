#define SERVER
#define DEV
//#define CLIENT
using System;
using System.IO;
using System.Linq;

namespace tdsm.patcher
{
    public class Program
    {
        public const String TDSMGuid = "9f7bca2e-4d2e-4244-aaae-fa56ca7797ec";
        public const Int32 Build = 1;
        //        public static bool IsPatching { get; private set; }

        //        static Program()
        //        {
        //            //Resolves external plugin hook assemblies. So there is no need to place the DLL beside tdsm.exe
        //            AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
        //            {
        //                Console.WriteLine("Looking for: {0}", a.Name);
        //
        //                return null;
        //            };
        //        }

#if DEV
        static void Copy(DirectoryInfo root, string project, string to, string pluginName = null, bool debugFolder = true)
        {
            var projectBinary = pluginName ?? project.Replace("-", ".");
            var p = debugFolder ? Path.Combine(root.FullName, project, "bin", "x86", "Debug") : Path.Combine(root.FullName, project);

            var dllF = Path.Combine(p, projectBinary + ".dll");
            //			var mdbF = Path.Combine (p, projectBinary + ".mdb");
            var ddbF = Path.Combine(p, projectBinary + ".dll.mdb");
            var pdbF = Path.Combine(p, projectBinary + ".pdb");

            var dllT = Path.Combine(to, projectBinary + ".dll");
            //			var mdbT = Path.Combine (to, projectBinary + ".mdb");
            var ddbT = Path.Combine(to, projectBinary + ".dll.mdb");
            var pdbT = Path.Combine(to, projectBinary + ".pdb");

            if (File.Exists(dllT)) File.Delete(dllT);
            //			if (File.Exists (mdbT)) File.Delete (mdbT);
            if (File.Exists(ddbT)) File.Delete(ddbT);
            if (File.Exists(pdbT)) File.Delete(pdbT);

            if (!Directory.Exists(to)) Directory.CreateDirectory(to);

            if (File.Exists(dllF)) File.Copy(dllF, dllT);
            //			if (File.Exists (mdbF)) File.Copy (mdbF, mdbT);
            if (File.Exists(ddbF)) File.Copy(ddbF, ddbT);
            if (File.Exists(pdbF)) File.Copy(pdbF, pdbT);

        }
#endif

        static void Main(string[] args)
        {
            //if (!tdsm.api.Permissions.PermissionsManager.IsSet)
            //{
            //    var file = System.IO.Path.Combine(tdsm.api.Globals.DataPath, "permissions.xml");
            //    //if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
            //    if (System.IO.File.Exists(file))
            //    {
            //        var handler = new tdsm.api.Permissions.XmlSupplier(file);
            //        if (handler.Load())
            //            tdsm.api.Permissions.PermissionsManager.SetHandler(handler);
            //    }
            //}

            //return;
            //            IsPatching = true;
            Console.WriteLine(Console.Title = String.Format("TDSM patcher build {0}", Build));
            var isMono = Type.GetType("Mono.Runtime") != null;

#if SERVER
            var inFile = "TerrariaServer.exe";
            var fileName = "tdsm";
            var outFileMS = fileName + ".microsoft.exe";
            var outFileMN = fileName + ".mono.exe";
            var patchFile = "tdsm.api.dll";

#if DEV
            if (File.Exists(outFileMS)) File.Delete(outFileMS);
            if (File.Exists(outFileMN)) File.Delete(outFileMN);

            var root = new DirectoryInfo(Environment.CurrentDirectory);
            while (root.GetDirectories().Where(x => x.Name == "tdsm-patcher").Count() == 0)
            {
                if (root.Parent == null)
                {
                    Console.WriteLine("Failed to find root TDSM project directory");
                    break;
                }
                root = root.Parent;
            }

            Copy(root, "tdsm-api", Environment.CurrentDirectory);
            Copy(root, "tdsm-core", Path.Combine(Environment.CurrentDirectory, "Plugins"));
            Copy(root, "Restrict", Path.Combine(Environment.CurrentDirectory, "Plugins"), "RestrictPlugin");
            Copy(root, "External", Environment.CurrentDirectory, "KopiLua", false);
            Copy(root, "External", Environment.CurrentDirectory, "NLua", false);

#endif
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
            if (vers != APIWrapper.TerrariaVersion)
            {
                Console.Write("This patcher only supports Terraria {0}, but we have detected something else {1}. Continue? (y/n)",
                    APIWrapper.TerrariaVersion,
                    vers);
                if (Console.ReadKey(true).Key != ConsoleKey.Y) return;
                Console.WriteLine();
            }
#if SERVER
            Console.Write("Opening up classes for API usage...");
            patcher.MakeTypesPublic(true);
            Console.Write("Ok\nHooking command line...");
            patcher.PatchCommandLine();
            patcher.HookConsoleTitle();
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
            Console.Write("Ok\nHooking into world events...");
            patcher.HookWorldEvents();
            Console.Write("Ok\nHooking statusText...");
            patcher.HookStatusText();
            Console.Write("Ok\nHooking NetMessage...");
            patcher.HookNetMessage();
            //Console.Write("Ok\nRemoving client code...");
            //patcher.RemoveClientCode();
            Console.Write("Ok\nHooking Server events...");
            patcher.HookUpdateServer();
            patcher.HookDedServEnd();
            Console.Write("Ok\nHooking NPC Spawning...");
            patcher.HookNPCSpawning();
            Console.Write("Ok\nHooking config...");
            patcher.HookConfig();
            Console.Write("Ok\nRouting socket implementations...");
            patcher.HookSockets();
            Console.Write("Ok\nHooking DEBUG...");
            patcher.HookWorldFile_DEBUG();
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
            //            APIWrapper.Initialise();
            //            byte[] data;
            //            using (var ms = new MemoryStream())
            //            {
            //                patcher.Terraria.Write(ms);
            //                ms.Seek(0, SeekOrigin.Begin);
            //                data = ms.ToArray();
            //            }
            //            data = APIWrapper.InvokeEvent(data, true);
            ////            tdsm.api.Globals.Touch();
            ////
            ////            tdsm.api.PluginManager.SetHookSource(typeof(HookPoints));
            ////            tdsm.api.PluginManager.Initialize(tdsm.api.Globals.PluginPath, tdsm.api.Globals.LibrariesPath);
            ////            tdsm.api.PluginManager.LoadPlugins();
            ////
            ////            var ctx = new tdsm.api.Plugin.HookContext
            ////            {
            ////                Sender = tdsm.api.Plugin.HookContext.ConsoleSender
            ////            };
            ////
            ////            var hookArgs = new HookArgs.PatchServer
            ////            {
            ////                Default = patcher,
            ////#if SERVER
            ////                IsClient = false,
            ////                IsServer = true
            ////#elif CLIENT
            ////                IsClient = true,
            ////                IsServer = false
            ////#endif
            ////            };
            ////            HookPoints.PatchServer.Invoke(ref ctx, ref hookArgs);
            //            //var apiBuild = APIWrapper.Build;
            //            APIWrapper.Finish();
#if Release
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception ocurred during PluginManager.LoadPlugins: {0}", e);
            }
#endif

            Console.Write("Saving to {0}...", outFileMS);
            patcher.Save(outFileMS, Build, TDSMGuid, fileName);
            //if (isMono || (args != null && args.Where(x => x.ToLower() == "-removeupnp").Count() > 0))
            {
                Console.Write("Ok\nRemoving port forwarding functionality...");
                patcher.FixNetplay();
            }
            Console.Write("Ok\nSaving to {0}...", outFileMN);
            patcher.Save(outFileMN, Build, TDSMGuid, fileName);


#if Release || true
            var current = isMono ? outFileMN : outFileMS;
            Console.WriteLine("Ok\nYou may now run {0} as you would normally.", current);
            Console.WriteLine("Press [y] to run {0}, any other key will exit . . .", current);
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                if (!isMono)
                {
                    if (File.Exists("serverconfig.txt"))
                        System.Diagnostics.Process.Start(current, "-config serverconfig.txt");
                    else
                        System.Diagnostics.Process.Start(current);
                }
                else
                {
                    Console.Clear();

                    using (var ms = new MemoryStream())
                    {
                        using (var fs = File.OpenRead(current))
                        {
                            var buff = new byte[256];
                            while (fs.Position < fs.Length)
                            {
                                var task = fs.Read(buff, 0, buff.Length);
                                ms.Write(buff, 0, task);
                            }
                        }

                        ms.Seek(0L, SeekOrigin.Begin);
                        var asm = System.Reflection.Assembly.Load(ms.ToArray());
                        if (File.Exists("serverconfig.txt"))
                            asm.EntryPoint.Invoke(null, new object[]
                            {
                                new string[] {"-config serverconfig.txt"}
                            });
                        else
                            asm.EntryPoint.Invoke(null, null);
                    }
                }
            }
#endif
        }
    }
}
