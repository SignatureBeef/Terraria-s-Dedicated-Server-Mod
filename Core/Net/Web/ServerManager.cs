using System;
using OTA.Logging;

namespace TDSM.Core.Net.Web
{
    public static class ServerManager
    {
        [TDSMComponent(ComponentEvent.ServerStopping)]
        public static void OnShuttingDown(Entry core)
        {
            if (!String.IsNullOrEmpty(core._webServerAddress))
            {
                ProgramLog.Log("Stopping web server");
                OTA.Web.WebServer.Stop();
            }
        }
    }
}

