using System;
using System.IO;
using System.Linq;
using OTA.Logging;

namespace TDSM.Core.Data.Management
{
    public static class LogManagement
    {
        /// <summary>
        /// The latest count of log files not to be deleted
        /// </summary>
        public static int LogsToKeep { get; set; }

        /// <summary>
        /// Gets if log purging is enabled
        /// </summary>
        public static bool LogPurgingEnabled
        {
            get { return LogsToKeep > 0; }
        }

        [TDSMComponent(ComponentEvent.Initialise)]
        internal static void Initialise(Entry plugin)
        {
            LogsToKeep = plugin.Config.Maintenance_LogsToKeep;

            if (LogPurgingEnabled)
            {
                var files = Directory.GetFiles(OTA.Globals.LogFolderPath, "*.log")
                    .Select(x => new FileInfo(x))
                    .OrderByDescending(x => x.CreationTime)
                    .Skip(LogsToKeep)
                    .ToList();

                foreach (var file in files)
                    File.Delete(file.FullName);

                if (files.Count > 0)
                    ProgramLog.Admin.Log("Purged {0} log files.", files.Count);
            }
        }
    }
}

