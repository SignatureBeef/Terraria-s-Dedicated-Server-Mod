
namespace tdsm.core.WebInterface.Pages.Admin
{
    /*
     * Note, Modules are not to send HTML, rather they are to send API information
     */
    [WebModule(Url = "/api/modules", InterfaceModule = true, Nodes = new string[] { "*" })]
    public class WebModules : WebPage
    {
        //TODO simple JSON writer
        //struct Info
        //{
        //    public string Provider { get; set; }
        //}

        public override void ProcessRequest(WebRequest request)
        {
            //We don't need a list here, but I rather get away from the lock quicker.
            var dependencies = new System.Collections.Generic.List<ResourceDependancy>();
            WebServer.ForEachPage((page) =>
            {
                var dpd = page.GetDependencies();
                if (dpd != null)
                {
                    dependencies.AddRange(dpd);
                }
            });

            //Since we are using a list we can make it a bit easier
            request.Writer.Buffer(dependencies.Count);

            foreach (var item in dependencies)
            {
                request.Writer.Buffer((byte)item.Type);
                request.Writer.Buffer(item.Url);
            }

            dependencies.Clear();
            dependencies = null;

            request.WriteOut("application/octet-stream");
        }
    }
}
