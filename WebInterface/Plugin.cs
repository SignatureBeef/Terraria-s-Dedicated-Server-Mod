using System;
using OTA.Plugin;
using System.Web.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Collections;

namespace TDSM.Web
{
    [OTAVersion(1, 0)]
    public class Plugin : BasePlugin
    {
        public Plugin()
        {
            base.Author = "TDSM";
            base.Description = "A web management interface for TDSM";
            base.Name = "Web Interface";
        }

        protected override void Enabled()
        {
            base.Enabled();
        }
    }

    /// <summary>
    /// A OWIN controller for non TDSM API's (Most will be in TDSM anyway)
    /// </summary>
//    [Authorize(Roles = "console")]
    public class ConsoleController : ApiController
    {
        static class Console
        {
            public static ConsoleTarget Listener;

            static Console()
            {
                OTA.Logging.ProgramLog.AddTarget(Listener = new ConsoleTarget());
            }

            public class ConsoleTarget : OTA.Logging.LogTarget
            {
                private readonly object _sync = new object();

                public OTA.Logging.OutputEntry[] Entries
                {
                    get
                    {
                        lock (_sync) return base.entries.ToArray();
                    }
                }
            }
        }

        public static OTA.Logging.OutputEntry[] FetchConsole()
        {
            return Console.Listener.Entries;
        }
    }
}

