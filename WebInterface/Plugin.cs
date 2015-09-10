using System;
using OTA.Plugin;
using System.Web.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

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
    [Authorize(Roles = "system")]
    public class SystemController : ApiController
    {
        public Tuple<Double, Double> Get()
        {

            return null;
        }
    }

    /// <summary>
    /// A OWIN controller for non TDSM API's (Most will be in TDSM anyway)
    /// </summary>
    public class ConsoleController : ApiController
    {
        [Authorize(Roles = "console")]
        public async Task<HttpResponseMessage> Get()
        {
            var items = await FetchConsole();
            return this.Request.CreateResponse(HttpStatusCode.OK, items);
        }

//        [Route("")]
        [Authorize(Roles = "console")]
        public async Task<HttpResponseMessage> Put()
        {
            var items = await FetchConsole();
            return this.Request.CreateResponse(HttpStatusCode.OK, items);
        }

        public static async Task<object> FetchConsole()
        {
            return null;
        }
    }
}

