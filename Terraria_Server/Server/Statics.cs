using System;
using System.IO;

namespace Terraria_Server
{
    public static class Statics
    {
        public const string TDCM_QUEST_GIVER = "Quest Giver";

        public const int BUILD = 36;
        public const int CURRENT_TERRARIA_RELEASE = 22;
        public static string CURRENT_TERRARIA_RELEASE_STR = CURRENT_TERRARIA_RELEASE.ToString();

        private const string WORLDS = "Worlds";
        private const string PLUGINS = "Plugins";
        private const string DATA = "Data";
        private const string LIBRARIES = "Libs";
        
		public static bool WorldLoaded
		{
			get;
			internal set;
		}

		public static bool PermissionsEnabled = false;

        public static bool cmdMessages = true;
		
		public static volatile bool Exit = false;

        public static string SavePath = Environment.CurrentDirectory;

        public static string WorldPath
        {
            get
            {
                return SavePath + Path.DirectorySeparatorChar + WORLDS;
            }
        }

        public static string PluginPath
        {
            get
            {
                return SavePath + Path.DirectorySeparatorChar + PLUGINS;
            }
        }

        public static string LibrariesPath
        {
            get
            {
                return SavePath + Path.DirectorySeparatorChar + PLUGINS + Path.DirectorySeparatorChar + LIBRARIES;
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
