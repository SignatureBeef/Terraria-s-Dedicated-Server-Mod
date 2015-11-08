using System;
using OTA.Logging;

namespace TDSM.Core.Net.Web
{
    public static class ServerManager
    {
        public static int RequestLockoutDuration = 10;
        public static int MaxRequestsPerLapse = 15;

        [TDSMComponent(ComponentEvent.ServerStopping)]
        public static void OnShuttingDown(Entry core)
        {
            if (!String.IsNullOrEmpty(core._webServerAddress))
            {
                ProgramLog.Log("Stopping web server");
                core.WebServer.Stop();
            }
        }
    }
}

