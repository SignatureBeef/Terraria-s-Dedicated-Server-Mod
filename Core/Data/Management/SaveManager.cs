using System;
using OTA.Logging;

namespace TDSM.Core.Data.Management
{
    public static class SaveManager
    {
        /// <summary>
        /// How often world saves are performed, in minutes
        /// </summary>
        /// <value>The backup interval minutes.</value>
        public static int SaveIntervalMinutes { get; set; }

        static SaveManager()
        {
            SaveIntervalMinutes = 10;
        }

        static DateTime _lastSave = DateTime.Now;

        internal static void OnUpdate()
        {
            if ((DateTime.Now - _lastSave).TotalMinutes >= SaveIntervalMinutes)
            {
                _lastSave = DateTime.Now;

                try
                {
                    lock (OTA.Callbacks.WorldFileCallback.SavePathLock)
                    {
                        using (var pg = new ProgressLogger(1, "Saving world"))
                        {
                            OTA.Callbacks.WorldFileCallback.SavePath = null; //Clear, and use the default
                            Terraria.IO.WorldFile.saveWorld();
                            pg.Value = 1;
                        }
                    }
                }
                catch (Exception e)
                {
                    ProgramLog.Log(e, "Error during the world save process.");
                }
            }
        }
    }
}

