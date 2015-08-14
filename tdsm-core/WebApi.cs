using System;
using System.Web.Http;

namespace TDSM.Core
{
    public class TDSMController : ApiController
    {
        public System.Collections.Generic.IEnumerable<string> Get()
        { 
            return new string[] { "TDSM CONTROLLER" }; 
        }
    }
}

