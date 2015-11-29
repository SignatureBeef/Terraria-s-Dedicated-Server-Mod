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

        [TDSMComponent(ComponentEvent.Initialise)]
        internal static void Initialise(Entry plugin)
        {
            SaveIntervalMinutes = plugin.Config.Maintenance_SaveIntervalMinutes;

            const Int32 MinSaveInterval = 1;
            if (SaveIntervalMinutes < MinSaveInterval)
            {
                SaveIntervalMinutes = MinSaveInterval;
                ProgramLog.Admin.Log("The save interval cannot be disabled and is now set to {0} minute", MinSaveInterval);
            }
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
                    ProgramLog.Log(e, "Error during the world save process");
                }
            }
        }
    }
}

