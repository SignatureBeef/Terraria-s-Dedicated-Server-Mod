using System;
using OTA.Logging;
using TDSM.Core.Data;

namespace TDSM.Core.Net.Web
{
    public static class ServerManager
    {
        public static int RequestLockoutDuration = 10;
        public static int MaxRequestsPerLapse = 15;

        [TDSMComponent(ComponentEvent.Initialise)]
        public static void OnInitialise(Entry core)
        {
            if (!String.IsNullOrEmpty(core.Config.Web_BindAddress))
            {
                ProgramLog.Web.Log("Starting web server");
                CreateAndStartWebServer(core, core.Config.Web_BindAddress);
            }
        }

        [TDSMComponent(ComponentEvent.ServerStopping)]
        public static void OnShuttingDown(Entry core)
        {
            if (!String.IsNullOrEmpty(core.Config.Web_BindAddress))
            {
                ProgramLog.Web.Log("Stopping web server");
                core.WebServer.Stop();
            }
        }

        /// <summary>
        /// Creates and starts the OWIN based web server.
        /// </summary>
        /// <param name="core">Core.</param>
        /// <param name="address">Address.</param>
        internal static void CreateAndStartWebServer(this Entry core, string address)
        {
            core.WebServer = new OTA.Web.WebServer();

            core.WebServer.Started += (sender, e) =>
            {
                ProgramLog.Web.Log("Web server started listening on {0}", address);
            };

            //Attach the TDSM OAuth system
            core.WebServer.ServerOptions.Provider = new PermissionsOAuthProvider();

            core.WebServer.Start(address);
        }
    }
}

