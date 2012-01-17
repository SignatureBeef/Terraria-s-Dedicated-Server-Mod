using System;
using System.IO;

namespace Terraria_Server
{
    public static class Statics
    {
		public const String TDCM_QUEST_GIVER = "Quest Giver";

		public const String VERSION_NUMBER = "v1.1.2";

		public const Int32 PRE_RELEASE_BUILD = 1;
		public const Int32 BUILD = 37;
		public const Int32 CURRENT_TERRARIA_RELEASE = 39;
        public static string CURRENT_TERRARIA_RELEASE_STR = CURRENT_TERRARIA_RELEASE.ToString();

        private const String WORLDS = "Worlds";
		private const String PLUGINS = "Plugins";
		private const String DATA = "Data";
		private const String LIBRARIES = "Libs";
        
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
