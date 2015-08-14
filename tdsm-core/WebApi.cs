using System;
using System.Web.Http;

namespace TDSM.Core
{
    public struct IdentityUser
    {

    }

    public class TDSMController : ApiController
    {
        public System.Collections.Generic.IEnumerable<string> GetCurrentUser()
        {
            return new string[] { "TDSM CONTROLLER" }; 
        }
    }
}

