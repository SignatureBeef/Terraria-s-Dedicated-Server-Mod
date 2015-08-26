using System;
using OTA.Plugin;
using System.Web.Http;

namespace TDSM.Web
{
    public class Plugin : BasePlugin
    {
        public Plugin()
        {
            base.Author = "TDSM";
            base.Description = "A web management interface for TDSM";
            base.Name = "Web Interface";
            base.TDSMBuild = 5;
        }

        protected override void Enabled()
        {
            base.Enabled();


        }
    }

    /// <summary>
    /// A OWIN controller for non TDSM API's (Most will be in TDSM anyway)
    /// </summary>
    public class SystemController : ApiController
    {
        [Authorize(Roles = "tdsm.web.getsystemstats")]
        public Tuple<Double, Double> Get()
        {

            return null;
        }
    }
}

