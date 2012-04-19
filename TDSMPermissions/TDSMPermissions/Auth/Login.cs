using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Terraria_Server.Plugins;
using Terraria_Server.Logging;
using Terraria_Server;
using System.IO;

namespace TDSMPermissions.Auth
{
    public static class Login
    {
        public const String LINK_RESTRICT = "http://update.tdsm.org/RestrictPlugin.dll";

        public static void InitSystem()
        {
            if (!IsRestrictRunning())
            {
                ProgramLog.Plugin.Log("No login system found! Restrict will be downloaded.");

                string restrict = Statics.PluginPath + Path.DirectorySeparatorChar + "RestrictPlugin";
                string dll = restrict + ".dll";

                if (!UpdateManager.PerformUpdate(LINK_RESTRICT, restrict + ".upt", restrict + ".bak", dll, 1, 1, "Restrict")) {
                    ProgramLog.Error.Log("Restrict failed to download!"); 
                    return;
                }

                PluginLoadStatus loadStatus = PluginManager.LoadAndInitPlugin(dll);
                if (loadStatus != PluginLoadStatus.SUCCESS)
                    ProgramLog.Error.Log("Restrict failed to install!\nLoad result: {0}", loadStatus); 
            }
        }

        public static bool IsRestrictRunning()
        {
            BasePlugin plg = PluginManager.GetPlugin("Restrict");

            return (plg != null && plg.Author == "UndeadMiner");
        }
    }
}
