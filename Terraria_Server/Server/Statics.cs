using System;
using System.IO;

namespace Terraria_Server
{
    public static class Statics
    {
        public const int BUILD = 17;
        public const int CURRENT_TERRARIA_RELEASE = 12;

        private const String WORLDS = "Worlds";
        private const String PLAYERS = "Players";
        private const String PLUGINS = "Plugins";
        private const String DATA = "Data";

        public static bool cmdMessages = true;
        public static bool debugMode = false;
        public static bool keepRunning = false;

        public static bool IsActive = false;
        public static bool serverStarted = false;

        public static string SavePath = Environment.CurrentDirectory;

        public static string WorldPath
        {
            get
            {
                return SavePath + Path.DirectorySeparatorChar + WORLDS;
            }
        }

        public static string PlayerPath
        {
            get
            {
                return SavePath + Path.DirectorySeparatorChar + PLAYERS;
            }
        }

        public static string PluginPath
        {
            get
            {
                return SavePath + Path.DirectorySeparatorChar + PLUGINS;
            }
        }

        public static string DataPath
        {
            get
            {
                return SavePath + Path.DirectorySeparatorChar + DATA;
            }
        }
    }
}
