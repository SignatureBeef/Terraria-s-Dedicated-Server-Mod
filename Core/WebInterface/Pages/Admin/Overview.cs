
namespace tdsm.core.WebInterface.Pages.Admin
{
    [WebModule(Url = "/api/module/overview", Nodes = new string[]
    {
        "*"
    })]
    public class Overview : WebPage
    {
        public override void ProcessRequest(WebRequest request)
        {
            //Return server information as json.
            //e.g. Uptime, players connected
        }
    }
}
