using System;
using System.Web.Http;
using System.Linq;

namespace TDSM.Core
{
    public struct IdentityUser
    {

    }

    public class PlayersController : ApiController
    {

        //TODO implement permissions

        [Authorize(Roles = "tdsm.api.getplayers")]
        public string[] Get()
        {
//            if (this.CheckAccess())
//            {
                return Terraria.Main.player
                    .Where(x => x != null && x.active)
                    .Select(x => x.Name)
                    .ToArray();
//            }
//
//            return null;
        }
    }

    //    public class AccessAttribute : Attribute
    //    {
    //        public AccessAttribute()
    //        {
    //        }
    //
    //        public string Node { get; set; }
    //    }

//    public static class ApiControllerExtensions
//    {
//        public static bool CheckAccess(this ApiController controller)
//        {
//            return false;
//        }
//    }
}