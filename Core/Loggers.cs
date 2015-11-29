using System;

namespace TDSM.Core
{
    public static class Loggers
    {
        public static readonly OTA.Logging.LogChannel Chat = new OTA.Logging.LogChannel("Chat", ConsoleColor.DarkMagenta, System.Diagnostics.TraceLevel.Info);
        public static readonly OTA.Logging.LogChannel Death = new OTA.Logging.LogChannel("Death", ConsoleColor.Green, System.Diagnostics.TraceLevel.Info);
    }
}

