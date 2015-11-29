using System;
using System.Diagnostics;
using OTA.Logging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Terraria;

namespace TDSM.Core.Data.Management
{
    /// <summary>
    /// Backup result.
    /// </summary>
    public enum BackupResult : int
    {
        SUCCESS = 0,
        LOAD_FAIL = 1,
        SAVE_FAIL = 2,
        SAVE_LOCK = 3
    }

    public static class BackupManager
    {
        const String FolderName = "Backups";

        /// <summary>
        /// Gets the backup folder.
        /// </summary>
        /// <value>The backup folder.</value>
        public static string BackupFolder
        {
            get { return Path.Combine(OTA.Globals.WorldPath, FolderName); }
        }

        /// <summary>
        /// Gets or sets whether to compress backups
        /// </summary>
        public static bool CompressBackups { get; set; }

        /// <summary>
        /// The maximum age in minutes for backups
        /// </summary>
        public static int BackupExpiryMinutes { get; set; }

        /// <summary>
        /// How often backups are created, in minutes
        /// </summary>
        /// <value>The backup interval minutes.</value>
        public static int BackupIntervalMinutes { get; set; }

        /// <summary>
        /// Gets whether backups are enabled
        /// </summary>
        public static bool BackupsEnabled
        {
            get { return BackupIntervalMinutes > 0; }
        }

        /// <summary>
        /// Gets whether backup expirations are enabled
        /// </summary>
        public static bool ExpirationsEnabled
        {
            get { return BackupExpiryMinutes > 0; }
        }

        /// <summary>
        /// Determines upon backing-up if the world should be saved or only duplicated from the existing world file.
        /// </summary>
        public static bool CopyBackups { get; set; }

        [TDSMComponent(ComponentEvent.Initialise)]
        internal static void Initialise(Entry plugin)
        {
            if (!Directory.Exists(BackupFolder)) Directory.CreateDirectory(BackupFolder);

            BackupExpiryMinutes = plugin.Config.Maintenance_BackupExpiryMinutes;
            BackupIntervalMinutes = plugin.Config.Maintenance_BackupIntervalMinutes;
            CompressBackups = plugin.Config.Maintenance_CompressBackups;
            CopyBackups = plugin.Config.Maintenance_CopyBackups;
        }

        static DateTime _lastBackup = DateTime.Now;

        internal static void OnUpdate()
        {
            if (BackupsEnabled && (DateTime.Now - _lastBackup).TotalMinutes >= BackupIntervalMinutes)
            {
                _lastBackup = DateTime.Now;

                try
                {
                    BackupManager.AutoPurge();
                    BackupManager.PerformBackup();
                }
                catch (Exception e)
                {
                    ProgramLog.Log(e, "Error during the backup process");
                }
            }
        }

        /*public static BackupResult LoadWorld(string Name)
        {
            WorldIO.LoadWorld(null, null, Statics.WorldPath + Path.DirectorySeparatorChar + Name);
            if (WorldModify.loadFailed && !WorldModify.loadSuccess)
            {
                return BackupResult.LOAD_FAIL;
            }
            return BackupResult.SUCCESS;
        }*/

        public static BackupResult Compress(string worldPath)
        {
//            Stopwatch stopwatch = new Stopwatch();
//            stopwatch.Start();
//
//            ProgramLog.Log("Compressing backup...");

            if (!File.Exists(worldPath))
            {
                ProgramLog.Error.Log("File not Found: " + worldPath);
                return BackupResult.LOAD_FAIL;
            }

            using (var pg = new ProgressLogger(1, "Compressing backup"))
            {
                FileInfo world = new FileInfo(worldPath);
                String archivePath = String.Concat(worldPath, ".zip");

                using (FileStream inStream = world.OpenRead())
                {
                    using (FileStream outStream = File.Create(archivePath))
                    {
                        using (GZipStream alg = new GZipStream(outStream, CompressionMode.Compress))
                        {
                            // copy the input file into the compression stream
                            inStream.CopyTo(alg);
                        }
                    }
                }

                if (File.Exists(archivePath))
                {
                    if (File.Exists(worldPath))
                        File.Delete(worldPath);
                    //                    stopwatch.Stop();

                    pg.Value = 1;
//                    ProgramLog.Log("Compression duration: " + stopwatch.Elapsed.Seconds + " Second(s)");
                    return BackupResult.SUCCESS;
                }
            }
//                else
//                {
//                    stopwatch.Stop();
            ProgramLog.Error.Log("Compression Failed!");
            return BackupResult.SAVE_FAIL;
//                }
        }

        public static BackupResult Decompress(string archivePath)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ProgramLog.Log("Decompressing backup...");

            if (!File.Exists(archivePath))
            {
                ProgramLog.Error.Log("File not Found: " + archivePath);
                return BackupResult.LOAD_FAIL;
            }

            FileInfo archive = new FileInfo(archivePath);
            string worldPath = archivePath.Remove(archivePath.Length - archive.Extension.Length);

            using (FileStream inStream = archive.OpenRead())
            {
                using (FileStream outStream = File.Create(worldPath))
                {
                    using (GZipStream alg = new GZipStream(inStream, CompressionMode.Decompress))
                    {
                        alg.CopyTo(outStream);
                    }
                }
            }

            if (File.Exists(worldPath))
            {
                if (File.Exists(archivePath))
                    File.Delete(archivePath);
                stopwatch.Stop();
                ProgramLog.Log("Decompression duration: " + stopwatch.Elapsed.Seconds + " Second(s)");
                return BackupResult.SUCCESS;
            }
            else
            {
                stopwatch.Stop();
                ProgramLog.Error.Log("Decompression Failed!");
                return BackupResult.SAVE_FAIL;
            }
        }

        public static BackupResult SaveWorld(string path)
        {
            if (Terraria.WorldGen.saveLock) return BackupResult.SAVE_LOCK; //Please wait for the current operation to finish.

            try
            {
                using (var pg = new ProgressLogger(1, "Backing up world"))
                {
                    if (CopyBackups)
                    {
                        var copyFrom = Terraria.Main.ActiveWorldFileData.Path;
                        if (File.Exists(copyFrom))
                        {
                            File.Copy(copyFrom, path);
                        }
                    }
                    else
                    {
                        lock (OTA.Callbacks.WorldFileCallback.SavePathLock)
                        {
                            OTA.Callbacks.WorldFileCallback.SavePath = path;
                            Terraria.IO.WorldFile.saveWorld();
                            OTA.Callbacks.WorldFileCallback.SavePath = null; //Reset
                        }
                    }
                    pg.Value = 1;
                }

                if (CompressBackups)
                    Compress(path); // it just adds ".zip" to the timestamp+".wld"
                    
                return BackupResult.SUCCESS;
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Error during the save process.");
            }

            return BackupResult.SAVE_FAIL;
        }

        public static BackupResult PerformBackup(string worldName)
        {
            try
            {
//                ProgramLog.Log("Performing backup...");

                var file = GetStamptedFilePath(worldName);
                while (File.Exists(file))
                    file = GetStamptedFilePath(worldName);

                return SaveWorld(file + ".wld");
            }
            catch (Exception e)
            {
                ProgramLog.Log(e);
            }

            return BackupResult.SAVE_FAIL;
        }

        public static BackupResult PerformBackup()
        {
            if (String.IsNullOrEmpty(Main.worldName))
                throw new Exception("Main.worldName must be initialized.");

            return PerformBackup(Main.worldName);
        }

        public static string GetTimeStamp(string worldName)
        {
            return String.Format("{0}_{1:yyyyMMddHHmmss}", worldName, DateTime.Now);
        }

        public static string GetStamptedFilePath(string worldName)
        {
            var name = GetTimeStamp(worldName);
            return Path.Combine(BackupFolder, name);
        }

        public static string[] GetBackups(string worldName)
        {
            var filter = CompressBackups ? "*.zip" : "*.wld";
            return Directory.GetFiles(BackupFolder, filter).Where(x => Path.GetFileName(x).StartsWith(worldName + "_")).ToArray();
        }

        public static string[] GetBackupsBefore(string worldName, DateTime date)
        {
            var backups = GetBackups(worldName);
            var oldBackups = from x in backups
                                      where File.GetCreationTime(x) < date
                                      select x;
            return oldBackups.ToArray();
        }

        public static string[] GetExpiredBackup(string worldName, int max)
        {
            var backups = GetBackups(worldName);

            var oldBackups = (from x in backups
                                       orderby File.GetCreationTime(x)
                                       select x).ToList();
            var amount = oldBackups.Count;

            if (amount <= max)
                return null;

            oldBackups.RemoveRange(0, max);
            return oldBackups.ToArray();
        }

        public static void AutoPurge(string worldName)
        {
            if (!ExpirationsEnabled)
                return;

//            ProgramLog.Log("Performing backup purge...");
            var backups = GetBackups(worldName);

            if (backups != null && backups.Length > 0)
                using (var pg = new ProgressLogger(backups.Length, "Purging old backups"))
                {

                    var expired = (from x in backups
                                                  where (DateTime.Now - File.GetCreationTime(x)).TotalMinutes >= BackupExpiryMinutes
                                                  select x).ToArray();
                    //var deleted = 0;
                    foreach (var file in expired)
                    {
                        try
                        {
                            File.Delete(file);
                            //deleted++;
                        }
                        catch
                        {
                        }
                    }
                }
            else ProgramLog.Log("No backups to be purged.");
        }

        public static void AutoPurge()
        {
            AutoPurge(Main.worldName);
        }
    }
}

