using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public static class Statics
    {
        public static bool debugMode = false;
        public static int platform = 0;
        public static bool isLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static string getWorldPath
        {
            get
            {
                return Statics.SavePath + systemSeperator + "Worlds";
            }
        }

        public static string getPlayerPath
        {
            get
            {
                return Statics.SavePath + systemSeperator + "Players";
            }
        }

        public static string getPluginPath
        {
            get
            {
                return Statics.SavePath + systemSeperator + "Plugins";
            }
        }

        public static string getDataPath
        {
            get
            {
                return Statics.SavePath + systemSeperator + "Data";
            }
        }

        public static string systemSeperator = "\\";
        public static string SavePath = Environment.CurrentDirectory;

        public static int currentRelease = 9;
        public static string versionNumber = "v1.0.4";

        public static bool IsActive = false;
        public static bool IsFixedTimeStep = false;
        public static bool serverStarted = false;

        public static int fidefault = (int)((double)Main.maxTilesX * 0.0008);
    }
}
