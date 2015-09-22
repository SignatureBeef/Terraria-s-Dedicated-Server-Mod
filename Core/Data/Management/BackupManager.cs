using System;
using System.Diagnostics;
using OTA.Logging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Terraria;

namespace TDSM.Core.Data.Management
{
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

        public static string BackupFolder
        {
            get { return Path.Combine(OTA.Globals.WorldPath, FolderName); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TDSM.Core.Data.Management.BackupManager"/> compress backups.
        /// </summary>
        /// <value><c>true</c> if compress backups; otherwise, <c>false</c>.</value>
        public static bool CompressBackups { get; set; }

        public static int BackupExpiryMinutes { get; set; }

        public static int BackupIntervalMinutes { get; set; }

        public static bool BackupsEnabled
        {
            get { return BackupIntervalMinutes > 0; }
        }

        static BackupManager()
        {
            BackupExpiryMinutes = 2;
            BackupIntervalMinutes = 1;//60;
            CompressBackups = true;
        }

        internal static void Initialise()
        {
            if (!Directory.Exists(BackupFolder)) Directory.CreateDirectory(BackupFolder);
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
                    ProgramLog.Log(e, "Error during the backup process.");
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ProgramLog.Log("Compressing backup...");

            if (!File.Exists(worldPath))
            {
                ProgramLog.Error.Log("File not Found: " + worldPath);
                return BackupResult.LOAD_FAIL;
            }

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
                stopwatch.Stop();
                ProgramLog.Log("Compression duration: " + stopwatch.Elapsed.Seconds + " Second(s)");
                return BackupResult.SUCCESS;
            }
            else
            {
                stopwatch.Stop();
                ProgramLog.Error.Log("Compression Failed!");
                return BackupResult.SAVE_FAIL;
            }
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
                lock (OTA.Callbacks.WorldFileCallback.SavePathLock)
                {
                    OTA.Callbacks.WorldFileCallback.SavePath = path;
                    Terraria.IO.WorldFile.saveWorld();
                    OTA.Callbacks.WorldFileCallback.SavePath = null; //Reset
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
                ProgramLog.Log("Performing backup...");

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
            return Directory.GetFiles(BackupFolder).Where(x => Path.GetFileName(x).StartsWith(worldName + "_")).ToArray();
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
            if (BackupExpiryMinutes == 0)
                return;

            ProgramLog.Log("Performing backup purge...");
            var backups = GetBackups(worldName);

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

        public static void AutoPurge()
        {
            AutoPurge(Main.worldName);
        }
    }
}

