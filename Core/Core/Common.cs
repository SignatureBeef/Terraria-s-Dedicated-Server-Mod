using System;

namespace TDSM.Core
{
    public partial class Entry
    {
        /// <summary>
        /// The instance of the OWIN based web server
        /// </summary>
        /// <value>The web server.</value>
        public OTA.Web.WebServer WebServer { get; set; }
    }
}

