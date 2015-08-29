using System;
using System.Linq;
using OTA.Patcher;
using System.IO;

namespace TDSM.Patcher
{
    public static class InjectorExtensions
    {
        public static void ExampleExtension(this OTA.Patcher.Injector injector)
        {
            //Modify IL or whatever
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            var resourceLib = "Vestris.ResourceLib.dll";
            if (!System.IO.File.Exists(resourceLib))
            {
                var bin = System.IO.Path.Combine(Environment.CurrentDirectory, "bin", "x86", "Debug", resourceLib);
                if (System.IO.File.Exists(bin))
                {
                    System.IO.File.Copy(bin, resourceLib);
                }
            }

            //Add additional patches at will
            OTAPatcher.PerformPatch += (object sender, OTAPatcher.InjectorEventArgs e) =>
            {
                e.Injector.ExampleExtension();

//                e.Injector.SwapOTAReferences();
            };

            //Attach to the dependencies event so we can copy our own files
            OTAPatcher.CopyDependencies += (object sender, OTAPatcher.CopyDependenciesEventArgs e) =>
            {
                OTAPatcher.Copy(e.RootDirectory, "Core", Path.Combine(Environment.CurrentDirectory, "Plugins"), "TDSM-Core", true);
                //            Copy(root, "Binaries", Path.Combine(Environment.CurrentDirectory), "TDSM.API");
                //Copy (root, "Restrict", Path.Combine (Environment.CurrentDirectory, "Plugins"), "RestrictPlugin");
//                OTAPatcher.Copy(e.RootDirectory, "tdsm-core", Path.Combine(Environment.CurrentDirectory, "Libraries"), "Newtonsoft.Json", true);
//                OTAPatcher.Copy(e.RootDirectory, "WebInterface", Path.Combine(Environment.CurrentDirectory, "Plugins"), "tdsm-web", true);
//                OTAPatcher.Copy(e.RootDirectory, "tdsm-mysql-connector", Path.Combine(Environment.CurrentDirectory, "Plugins"), "tdsm-mysql-connector", true);
//                OTAPatcher.Copy(e.RootDirectory, "tdsm-sqlite-connector", Path.Combine(Environment.CurrentDirectory, "Plugins"), "tdsm-sqlite-connector", true);

            };

            //By default we will patch a server
            OTAPatcher.PatchMode = OTA.Patcher.PatchMode.Server;

            //Specifiy the official file name
            OTAPatcher.InputFileName = "TerrariaServer.exe";

            //Specify the output assembly[name]
            OTAPatcher.OutputName = "TerrariaServer";

            //Specify the output assembly[name]
            OTAPatcher.OTAProjectDirectory = "Open-Terraria-API";

            OTAPatcher.CopyProjectFiles = true;

            OTAPatcher.PromptToRun = false;

            OTAPatcher.DefaultProcess(args);

            //Post process, we can apply our additional configuration lines
            Console.Write("Generating server.config...");
            OTAPatcher.GenerateConfig();

            Console.Write("Ok\nUpdating binaries...");
            OTAPatcher.BinariesFiles.Add(Path.Combine("Plugins", "TDSM.Core.dll"));
            OTAPatcher.BinariesFiles.Add(Path.Combine("Plugins", "TDSM.Core.dll.mdb"));
            OTAPatcher.BinariesFiles.Add(Path.Combine("Plugins", "TDSM.Core.pdb"));
            Console.Write("Ok\n");

            //Update dev files
            OTAPatcher.UpdateBinaries();

            OTAPatcher.PromptUserToRun(OTAPatcher.OutputName + ".exe", Type.GetType("Mono.Runtime") != null);

//            var isMono = Type.GetType("Mono.Runtime") != null;
//            if (!isMono)
//            {
//                Console.Write("Ok\nUpdating icons...");
//                var res = new Vestris.ResourceLib.IconDirectoryResource(new Vestris.ResourceLib.IconFile("tdsm.ico"));
//                foreach (var fl in new string[] { OTAPatcher.OutputName + ".exe" }) //outFileMS, outFileMN })
//                {
//                    try
//                    {
//                        System.Threading.Thread.Sleep(1000);
//                        res.SaveTo(fl);
//                    }
//                    catch
//                    {
//                        Console.Write("Failed to write icon for: " + fl);
//                    }
//                }
//            
//                Console.Write("Ok\nUpdating headers...");
//                foreach (var fl in new string[] { OTAPatcher.OutputName + ".exe" })
//                {
//                    try
//                    {
//                        using (var ri = new Vestris.ResourceLib.ResourceInfo())
//                        {
//                            System.Threading.Thread.Sleep(1000);
//                            ri.Load(fl);
//            
//                            var ver = (Vestris.ResourceLib.VersionResource)ri[Vestris.ResourceLib.Kernel32.ResourceTypes.RT_VERSION].First();
//            
//                            var inf = (Vestris.ResourceLib.StringFileInfo)ver["StringFileInfo"];
//                            inf["OriginalFilename"] = OTAPatcher.OutputName + ".exe" + '\0';
//                            inf["InternalName"] = OTAPatcher.OutputName + ".exe" + '\0';
//                            inf["ProductName"] = OTAPatcher.OutputName + '\0';
//                            inf["FileDescription"] = OTAPatcher.OutputName + '\0';
//            
//                            ri.Unload();
//                            ver.SaveTo(fl);
//                        }
//                    }
//                    catch
//                    {
//                        Console.WriteLine("Failed to write header for: " + fl);
//                    }
//                }
//            }
        }
    }
}
