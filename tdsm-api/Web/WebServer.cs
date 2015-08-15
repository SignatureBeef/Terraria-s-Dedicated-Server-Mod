using System;
using System.Threading;
using System.IO;
using TDSM.API.Logging;
using Owin;
using System.Web.Http;

namespace TDSM.API.Web
{
    public static class WebServer
    {
        public static System.Web.Http.HttpConfiguration Config { get; private set; }

        public static string StaticFileDirectory = "Web";

        internal static readonly AutoResetEvent Switch = new AutoResetEvent(false);

        static WebServer()
        {
            Config = new System.Web.Http.HttpConfiguration();

            Config.Routes.MapHttpRoute(
                name: "DefaultApi", 
                routeTemplate: "api/{controller}/{id}", 
                defaults: new { id = System.Web.Http.RouteParameter.Optional } 
            );

//            Config.DependencyResolver = new AssembliesResolver();
            Config.Services.Replace(typeof(System.Web.Http.Dispatcher.IAssembliesResolver), new PluginServiceResolver());
        }

        public static void Start(string baseAddress)
        {
            Switch.Reset();
            (new Thread(OWINServer.StartServer)).Start(baseAddress);
        }

        public static void Stop()
        {
            Switch.Set();
        }
    }

    class PluginServiceResolver : System.Web.Http.Dispatcher.DefaultAssembliesResolver
    {
        public override System.Collections.Generic.ICollection<System.Reflection.Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }

    class OWINServer
    {
        public void Configuration(Owin.IAppBuilder app)
        {
            app.UseErrorPage();
            app.UseWebApi(WebServer.Config); 

            app.UseFileServer(new Microsoft.Owin.StaticFiles.FileServerOptions()
                {
                    RequestPath = new Microsoft.Owin.PathString("/web"),
                    FileSystem = new Microsoft.Owin.FileSystems.PhysicalFileSystem(WebServer.StaticFileDirectory),
                    EnableDirectoryBrowsing = false
                });
        }


        internal static void StartServer(object baseAddress)
        {
            System.Threading.Thread.CurrentThread.Name = "Web";

            if (!Directory.Exists(WebServer.StaticFileDirectory))
            {
                Directory.CreateDirectory(WebServer.StaticFileDirectory);
            }

            try
            {
                using (Microsoft.Owin.Hosting.WebApp.Start<OWINServer>(url: baseAddress as String))
                {
                    ProgramLog.Web.Log("Web server started listening on {0}", baseAddress);
                    WebServer.Switch.WaitOne();
                } 
            }
            catch (Exception e)
            {
                ProgramLog.Log(e);
            }
        }
    }
}

