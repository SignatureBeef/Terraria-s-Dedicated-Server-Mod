using System;
using System.Web.Http;
using Owin;
using System.Threading;
using Microsoft.Owin.Hosting;
using TDSM.API.Logging;

namespace TDSM.API
{
    public static class WebServer
    {
        public static HttpConfiguration Config { get; private set; }

        private static readonly AutoResetEvent _switch = new AutoResetEvent(false);

        static WebServer()
        {
            Config = new HttpConfiguration();
        }

        public static void Start(string baseAddress)
        {
            _switch.Reset();
            (new Thread(StartServer)).Start(baseAddress);
        }

        public static void Stop()
        {
            _switch.Set();
        }

        private static void StartServer(object baseAddress)
        {
            System.Threading.Thread.CurrentThread.Name = "Web";
            try
            {
                using (WebApp.Start<OWINServer>(url: baseAddress as String))
                {
                    ProgramLog.Web.Log("Web server started listening on {0}", baseAddress);
                    _switch.WaitOne();
                } 
            }
            catch (Exception e)
            {
                ProgramLog.Log(e);
            }
        }
    }

    class OWINServer
    {
        public void Configuration(IAppBuilder app)
        {
            #if DEBUG
            app.UseWelcomePage();
            #endif
            app.UseErrorPage();
            app.UseWebApi(WebServer.Config); 
        }
    }
}

