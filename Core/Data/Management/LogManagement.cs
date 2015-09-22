using System;
using System.IO;
using System.Linq;
using OTA.Logging;

namespace TDSM.Core.Data.Management
{
    public static class LogManagement
    {
        public static int LogsToLeave { get; set; }

        public static bool LogPurgingEnabled
        {
            get { return LogsToLeave > 0; }
        }

        static LogManagement()
        {
            LogsToLeave = 5;
        }

        internal static void Initialise()
        {
            if (LogPurgingEnabled)
            {
                var files = Directory.GetFiles(OTA.Globals.LogFolderPath, "*.log")
                    .Select(x => new FileInfo(x))
                    .OrderByDescending(x => x.CreationTime)
                    .Skip(LogsToLeave)
                    .ToList();

                foreach (var file in files)
                    File.Delete(file.FullName);

                if (files.Count > 0)
                    ProgramLog.Admin.Log("Purged {0} log files.", files.Count);
            }
        }
    }
}

