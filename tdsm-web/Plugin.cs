using System;
using TDSM.API.Plugin;
using System.Web.Http;
using TDSM.Core;

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
        [AccessAttribute(Node = "tdsm.web.GetSystemStats")]
        public Tuple<Double, Double> Get()
        {
            if (this.CheckAccess())
            {
                
            }

            return null;
        }
    }
}

