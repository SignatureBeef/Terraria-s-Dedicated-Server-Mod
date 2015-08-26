
namespace tdsm.core.WebInterface.Pages
{
    [WebModule(Url = "/api/auth")]
    public class Authenticate : WebPage
    {
        //TODO simple JSON writer
        //struct Info
        //{
        //    public string Provider { get; set; }
        //}

        public override void ProcessRequest(WebRequest request)
        {
            request.StatusCode = 200;
            request.RepsondHeader(200, "OK", "application/octet-stream", 1);

            request.Send(request.AuthenticatedAs != null);

            request.End();
        }
    }
}
