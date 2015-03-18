
namespace tdsm.core.WebInterface.Pages.Admin
{
    /*
     * Note, Modules are not to send HTML, rather they are to send API information
     */
    [WebModule(Url = "/api/modules")]
    public class WebModules : WebPage
    {
        //TODO simple JSON writer
        //struct Info
        //{
        //    public string Provider { get; set; }
        //}

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
