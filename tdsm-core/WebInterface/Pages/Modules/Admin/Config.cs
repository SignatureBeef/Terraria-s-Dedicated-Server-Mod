
namespace tdsm.core.WebInterface.Pages.Admin
{
    /*
     * Note, Modules are not to send HTML, rather they are to send API information
     */
    [WebModule(Url = "/api/config", Nodes = new string[]
    {
        "tdsm.web.config"
    })]
    public class Config : WebPage
    {
        //TODO simple JSON writer
        //struct Info
        //{
        //    public string Provider { get; set; }
        //}

        static readonly ResourceDependancy[] _dependencies = new ResourceDependancy[]
        {
            new ResourceDependancy()
            {
                Type = ResourceType.Javascript,
                Url = "modules/config.js"
            }
        };
        public override ResourceDependancy[] GetDependencies()
        {
            return _dependencies;
        }

        public override void ProcessRequest(WebRequest request)
        {
            request.StatusCode = 200;

            //TODO buffer responses in order to fill the Content-Length header
            //var length = (WebServer.ProviderName.Length + 4) + 4 + 4;
            //request.RepsondHeader(200, "OK", "application/octet-stream", length);

            //request.Send(WebServer.ProviderName);
            //request.Send(Globals.Build);
            //request.Send(Entry.CoreBuild);

            //request.End();
        }
    }
}
