
using System;
namespace tdsm.core.WebInterface.Pages.Admin
{
    /*
     * Note, Modules are not to send HTML, rather they are to send API information
     */
    [WebModule(Url = "/api/admin/info", Nodes = new string[]
    {
        "tdsm.web.info",
        "*"
    })]
    public class AdminInfo : WebPage
    {
        public override void ProcessRequest(WebRequest request)
        {
            System.IO.FileInfo info = null;
            if (!String.IsNullOrEmpty(Terraria.Main.worldPathName))
            {
                info = new System.IO.FileInfo(Terraria.Main.worldPathName);
            }

            request.Writer.Buffer(Terraria.Main.worldName ?? String.Empty);
            request.Writer.Buffer(Terraria.Main.maxTilesX);
            request.Writer.Buffer(Terraria.Main.maxTilesY);

            if (info != null && info.Exists)
            {
                request.Writer.Buffer(info.Length);
            }
            else
            {
                request.Writer.Buffer(0L);
            }

            request.Writer.Buffer(Heartbeat.Enabled);
            request.Writer.Buffer(Heartbeat.ServerName);
            request.Writer.Buffer(Heartbeat.ServerDescription);
            request.Writer.Buffer(Heartbeat.ServerDomain);
            request.Writer.Buffer(Heartbeat.PublishToList);

            request.WriteOut();
        }
    }
}
