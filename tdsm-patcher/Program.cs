#define SERVER
#define DEV
//#define CLIENT

using System;
using System.IO;
using System.Linq;

//using System.Net.Sockets;
//using System.Text;
//using System.Net;

namespace tdsm.patcher
{
    public class Program
    {
        public const String TDSMGuid = "9f7bca2e-4d2e-4244-aaae-fa56ca7797ec";
        public const Int32 Build = 2;
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
            //if (!tdsm.api.Command.WorldTime.Test())
            //{
            //    Console.WriteLine("Time test failed");
            //    Console.ReadKey(true);
            //}

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

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Console.Title = String.Format("TDSM patcher build {0}", Build));
            Console.ForegroundColor = ConsoleColor.White;
            var isMono = Type.GetType("Mono.Runtime") != null;

#if SERVER
            var inFile = "TerrariaServer.exe";
            var fileName = "tdsm";
            //            var outFileMS = fileName + ".microsoft.exe";
            //            var outFileMN = fileName + ".mono.exe";
            var output = fileName + ".exe";
            var patchFile = "tdsm.api.dll";

            if (!File.Exists(inFile))
            {
                var bin = Path.Combine(Environment.CurrentDirectory, "bin", "x86", "Debug", inFile);
                if (File.Exists(bin)) inFile = bin;
            }
            if (!File.Exists(patchFile))
            {
                var bin = Path.Combine(Environment.CurrentDirectory, "bin", "x86", "Debug", patchFile);
                if (File.Exists(bin)) patchFile = bin;
            }

            var resourceLib = "Vestris.ResourceLib.dll";
            if (!System.IO.File.Exists(resourceLib))
            {
                var bin = System.IO.Path.Combine(Environment.CurrentDirectory, "bin", "x86", "Debug", resourceLib);
                if (System.IO.File.Exists(bin))
                {
                    System.IO.File.Copy(bin, resourceLib);
                }
            }

#if DEV
            //            if (File.Exists(outFileMS)) File.Delete(outFileMS);
            //            if (File.Exists(outFileMN)) File.Delete(outFileMN);
            if (File.Exists(output)) File.Delete(output);

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

            Copy(root, "Binaries", Path.Combine(Environment.CurrentDirectory), "tdsm.api");
            Copy(root, "tdsm-api", Environment.CurrentDirectory);
            Copy(root, "tdsm-core", Path.Combine(Environment.CurrentDirectory, "Plugins"));
            //Copy (root, "Restrict", Path.Combine (Environment.CurrentDirectory, "Plugins"), "RestrictPlugin");
            Copy(root, "External", Path.Combine(Environment.CurrentDirectory, "Libraries"), "KopiLua", false);
            Copy(root, "External", Path.Combine(Environment.CurrentDirectory, "Libraries"), "NLua", false);
            Copy(root, "External", Path.Combine(Environment.CurrentDirectory, "Libraries"), "ICSharpCode.SharpZipLib", false);
            Copy(root, "External", Path.Combine(Environment.CurrentDirectory, "Libraries"), "Mono.Nat", false);
            Copy(root, "tdsm-core", Path.Combine(Environment.CurrentDirectory, "Libraries"), "Newtonsoft.Json", true);

#endif
#elif CLIENT
            var inFile = "Terraria.exe";
            var fileName = "tcsm";
            var outFileMS = fileName + ".microsoft.exe";
            var outFileMN = fileName + ".mono.exe";
            var patchFile = "MonoGame.Framework.dll";
#endif
            if (!File.Exists(inFile))
            {
                //Download the supported vanilla software from our GitHub repo
                Console.WriteLine("The original Re-Logic TerrariaServer.exe is missing, download?");
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    //TODO add throbber
                    const String Url = "https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/raw/master/Official/TerrariaServer.exe";
                    using (var wc = new System.Net.WebClient())
                    {
                        var started = DateTime.Now;
                        try
                        {
                            wc.DownloadFile(Url, inFile);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            Console.WriteLine("Press any key to exit...");
                            Console.ReadKey(true);
                            return;
                        }
                        var duration = DateTime.Now - started;
                        Console.WriteLine("Download completed in {0:c}", duration);
                    }
                }
                else return;
            }

            var patcher = new Injector(inFile, patchFile);

            var noVersionCheck = args != null && args.Where(x => x.ToLower() == "-nover").Count() > 0;
            if (noVersionCheck != true)
            {
                var vers = patcher.GetAssemblyVersion();
                if (vers != APIWrapper.TerrariaVersion)
                {
                    Console.WriteLine("This patcher only supports Terraria {0}, but we have detected something else {1}.", APIWrapper.TerrariaVersion, vers);
                    Console.Write("There's a high chance this will fail, continue? (y/n)");
                    if (Console.ReadKey(true).Key != ConsoleKey.Y) return;
                    Console.WriteLine();
                }
            }
#if SERVER
            Console.Write("Opening up classes for API usage...");
            patcher.MakeTypesPublic(true);
            Console.Write("Ok\nHooking command line...");
            patcher.PatchCommandLine();
            Console.Write("Ok\nHooking players...");
            patcher.PatchPlayer();
            Console.Write("Ok\nRemoving console handlers...");
            patcher.RemoveConsoleHandler();
//            Console.Write("Ok\nRemoving mono incompatible code...");
//            patcher.RemoveProcess();
            ////patcher.HookConsoleTitle();
            Console.Write("Ok\nSkipping sysmenus functions...");
            patcher.SkipMenu();
            //Console.Write("Ok\nFixing code entry...");
            //patcher.FixEntryPoint();
            Console.Write("Ok\nPatching save paths...");
            patcher.FixSavePath();
            Console.Write("Ok\nHooking receive buffer...");
            patcher.HookMessageBuffer();
            //Console.Write("Ok\nAdding the slot manager...");
            //patcher.PatchServer();
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
            //Console.Write("Ok\nHooking NetMessage...");
            //patcher.HookNetMessage();
            ////Console.Write("Ok\nRemoving client code...");
            ////patcher.RemoveClientCode();
            Console.Write("Ok\nHooking Server events...");
            patcher.HookUpdateServer();
            patcher.HookDedServEnd();
            Console.Write("Ok\nHooking NPC Spawning...");
            patcher.HookNPCSpawning();
            Console.Write("Ok\nHooking config...");
            patcher.HookConfig();
            ////Console.Write("Ok\nRouting socket implementations...");
            ////patcher.HookSockets();
            Console.Write("Ok\nFixing statusText...");
            patcher.FixStatusTexts();
            Console.Write("Ok\nHooking invasions...");
            patcher.HookInvasions();
            Console.Write("Ok\nEnabling rain...");
            patcher.EnableRaining();
            //Console.Write("Ok\nHooking eclipse...");
            //patcher.HookEclipse();
            //Console.Write("Ok\nHooking blood moon...");
            //patcher.HookBloodMoon();

            Console.Write("Ok\nFixing world removal...");
            patcher.PathFileIO();

            //We only need one TDSM.exe if this works...
            Console.Write("Ok\nRemoving port forwarding functionality...");
            patcher.FixNetplay();
//            patcher.DetectMissingXNA();

            Console.Write("Ok\n");
            patcher.InjectHooks();

            //            Console.Write("Ok\nPutting Terraria on a diet...");
            //            patcher.ChangeTileToStruct();
            //Console.Write("Ok\nHooking DEBUG...");
            //patcher.HookWorldFile_DEBUG();

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

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            //            //if (isMono || (args != null && args.Where(x => x.ToLower() == "-removeupnp").Count() > 0))
            //            //{
            //            //    Console.Write("Ok\nRemoving port forwarding functionality...");
            //            //    patcher.FixNetplay();
            //            //}
            //            Console.Write("Ok\nSaving to {0}...", outFileMN);
            //            patcher.Save(outFileMN, Build, TDSMGuid, fileName);
            Console.Write("Saving to {0}...", output);
            patcher.Save(output, Build, TDSMGuid, fileName);


            //var t = patcher.Terraria.Netplay.Fields.Single(x => x.Name == "serverSock");


            patcher.Dispose();
            //            Console.WriteLine("Ok");

            Console.ForegroundColor = ConsoleColor.White;
            if (!isMono)
            {
                Console.Write("Ok\nUpdating icons...");
                var res = new Vestris.ResourceLib.IconDirectoryResource(new Vestris.ResourceLib.IconFile("tdsm.ico"));
                foreach (var fl in new string[] { output }) //outFileMS, outFileMN })
                {
                    try
                    {
                        System.Threading.Thread.Sleep(1000);
                        res.SaveTo(fl);
                    }
                    catch
                    {
                        Console.Write("Failed to write icon for: " + fl);
                    }
                }

                Console.Write("Ok\nUpdating headers...");
                foreach (var fl in new string[] { output }) //outFileMS, outFileMN })
                {
                    try
                    {
                        using (var ri = new Vestris.ResourceLib.ResourceInfo())
                        {
                            System.Threading.Thread.Sleep(1000);
                            ri.Load(fl);

                            var ver = (Vestris.ResourceLib.VersionResource)ri[Vestris.ResourceLib.Kernel32.ResourceTypes.RT_VERSION].First();

                            var inf = (Vestris.ResourceLib.StringFileInfo)ver["StringFileInfo"];
                            inf["OriginalFilename"] = fileName + ".exe" + '\0';
                            inf["InternalName"] = fileName + ".exe" + '\0';
                            inf["ProductName"] = fileName + '\0';
                            inf["FileDescription"] = fileName + '\0';

                            ri.Unload();
                            ver.SaveTo(fl);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Failed to write header for: " + fl);
                    }
                }
            }

#if DEBUG
            Console.Write("Ok\nUpdating Binaries folder...");
            UpdateBinaries();
#endif

#if Release || true
            var noRun = args != null && args.Where(x => x.ToLower() == "-norun").Count() > 0;
            if (!noRun)
            {
                //var current = isMono ? outFileMN : outFileMS;
                //                if (File.Exists("tdsm.exe")) File.Delete("tdsm.exe");
                //                if (isMono)
                //                {
                //                    File.Copy(outFileMN, "tdsm.exe");
                //                }
                //                else
                //                {
                //                    File.Copy(outFileMS, "tdsm.exe");
                //                }
                //                var current = "tdsm.exe";
                Console.WriteLine("Ok");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("You may now run {0} as you would normally.", output);
                Console.WriteLine("Press [y] to run {0}, any other key will exit . . .", output);
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    if (!isMono)
                    {
                        if (File.Exists("serverconfig.txt"))
                            System.Diagnostics.Process.Start(output, "-config serverconfig.txt");
                        else
                            System.Diagnostics.Process.Start(output);
                    }
                    else
                    {
                        Console.Clear();

                        using (var ms = new MemoryStream())
                        {
                            using (var fs = File.OpenRead(output))
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
                            try
                            {
                                if (File.Exists("serverconfig.txt"))
                                    asm.EntryPoint.Invoke(null, new object[] {
                                        new string[] { "-config", "serverconfig.txt", "-noupnp" }
							});
                                else
                                    asm.EntryPoint.Invoke(null, null);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Ok\n");
            }
#endif
        }

        static void UpdateBinaries()
        {
            var pathToBinaries = new DirectoryInfo(Environment.CurrentDirectory);
            while (!Directory.Exists(Path.Combine(pathToBinaries.FullName, "Binaries")))
            {
                pathToBinaries = pathToBinaries.Parent;
            }
            pathToBinaries = new DirectoryInfo(Path.Combine(pathToBinaries.FullName, "Binaries"));

            if (!pathToBinaries.Exists)
            {
                Console.WriteLine("Failed to copy to binaries.");
                return;
            }

            foreach (var rel in new string[]
            { 
                "tdsm.api.dll",
                "tdsm.api.pdb",
                "Libraries" + Path.DirectorySeparatorChar + "Newtonsoft.Json.dll",
                "Libraries" + Path.DirectorySeparatorChar + "Newtonsoft.Json.pdb",
                "Libraries" + Path.DirectorySeparatorChar + "NLua.dll",
                "Plugins" + Path.DirectorySeparatorChar + "tdsm.core.dll",
                "Plugins" + Path.DirectorySeparatorChar + "tdsm.core.pdb",
                "Plugins" + Path.DirectorySeparatorChar + "RestrictPlugin.dll",
                "Plugins" + Path.DirectorySeparatorChar + "RestrictPlugin.pdb",
                "tdsm-patcher.exe",
                "tdsm-patcher.pdb",
                "Vestris.ResourceLib.dll",
                "Libraries" + Path.DirectorySeparatorChar + "KopiLua.dll",
                "Libraries" + Path.DirectorySeparatorChar + "ICSharpCode.SharpZipLib.dll",
                "Libraries" + Path.DirectorySeparatorChar + "Mono.Nat.dll",
                "Libraries" + Path.DirectorySeparatorChar + "Mono.Nat.pdb",
                "tdsm.exe"
//                "tdsm.microsoft.exe",
//                "tdsm.mono.exe"
            })
            {
                if (File.Exists(rel))
                {
                    var pth = Path.Combine(pathToBinaries.FullName, rel);

                    var inf = new FileInfo(pth);
                    if (!inf.Directory.Exists) inf.Directory.Create();
                    if (inf.Exists) inf.Delete();

                    File.Copy(rel, pth);
                }
            }
        }
    }
}
