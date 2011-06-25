using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Terraria_Server
{
    public static class Statics
    {
        public static int build = 9;
        //public static double revision = 3;   

        public static bool cmdMessages = true;
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
        public static bool isWindows
        {
            get
            {
                PlatformID p = Environment.OSVersion.Platform;
                return (p == PlatformID.Win32NT) || (p == PlatformID.Win32S) || (p == PlatformID.Win32Windows) || (p == PlatformID.WinCE);
            }
        }
        public static bool isMac
        {
            get
            {
                PlatformID p = Environment.OSVersion.Platform;
                return (p == PlatformID.MacOSX);
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

        public static string systemSeperator = Path.DirectorySeparatorChar.ToString();
        public static string SavePath = Environment.CurrentDirectory;

        public static int currentRelease = 12;
        public static string versionNumber = "v1.0.5";

        public static bool IsActive = false;
        public static bool IsFixedTimeStep = false;
        public static bool serverStarted = false;

    }
}
