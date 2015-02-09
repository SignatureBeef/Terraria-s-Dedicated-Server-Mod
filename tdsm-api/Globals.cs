using System;
using System.IO;

namespace tdsm.api
{
    public static class Globals
    {
        public const Double Build = 1.0;

        public const Int32 TerrariaRelease = 102;
        public const String TerrariaVersion = "1.2.4.1";

        private const String WorldDirectory = "Worlds";
        private const String PluginDirectory = "Plugins";
        private const String DataDirectory = "Data";
        private const String LibrariesDirectory = "Libraries";
        private const String BackupDirectory = "BackupDirectory";

        public static volatile bool Exit = false;

        public static string SavePath = Environment.CurrentDirectory;

        public static string WorldPath
        {
            get
            { return Path.Combine(SavePath, WorldDirectory); }
        }

        public static string WorldBackupPath
        {
            get
            { return Path.Combine(SavePath, WorldDirectory, BackupDirectory); }
        }

        public static string PluginPath
        {
            get
            { return Path.Combine(SavePath, PluginDirectory); }
        }

        public static string LibrariesPath
        {
            get
            { return Path.Combine(SavePath, PluginDirectory, LibrariesDirectory); }
        }

        public static string DataPath
        {
            get
            { return Path.Combine(SavePath, DataDirectory); }
        }

        public static void Touch()
        {
            if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);
            if (!Directory.Exists(WorldPath)) Directory.CreateDirectory(WorldPath);
            if (!Directory.Exists(WorldBackupPath)) Directory.CreateDirectory(WorldBackupPath);
            if (!Directory.Exists(PluginPath)) Directory.CreateDirectory(PluginPath);
            if (!Directory.Exists(LibrariesPath)) Directory.CreateDirectory(LibrariesPath);
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
        }
    }
}
