using System;
using System.Web.Http;
using Owin;
using System.Threading;
using Microsoft.Owin.Hosting;
using TDSM.API.Logging;
using System.IO;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using System.Web.Http.Dispatcher;
using System.Collections.Generic;
using System.Reflection;

namespace TDSM.API
{
    public static class WebServer
    {
        public static HttpConfiguration Config { get; private set; }

        public static string StaticFileDirectory = "Web";

        internal static readonly AutoResetEvent Switch = new AutoResetEvent(false);

        static WebServer()
        {
            Config = new HttpConfiguration();

            Config.Routes.MapHttpRoute(
                name: "DefaultApi", 
                routeTemplate: "api/{controller}/{id}", 
                defaults: new { id = RouteParameter.Optional } 
            );

//            Config.DependencyResolver = new AssembliesResolver();
            Config.Services.Replace(typeof(IAssembliesResolver), new PluginServiceResolver());
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

    public class PluginServiceResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }

    class OWINServer
    {
        public void Configuration(IAppBuilder app)
        {
//            #if DEBUG
//            app.UseWelcomePage();
//            #endif
            app.UseErrorPage();
            app.UseWebApi(WebServer.Config); 

            app.UseFileServer(new FileServerOptions()
                {
                    RequestPath = new PathString("/web"),
                    FileSystem = new PhysicalFileSystem(WebServer.StaticFileDirectory),
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
                using (WebApp.Start<OWINServer>(url: baseAddress as String))
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

