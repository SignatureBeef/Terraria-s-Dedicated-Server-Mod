#define ENABLED

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using tdsm.api.Command;
using tdsm.core.Logging;
using tdsm.core.ServerCore;
using tdsm.core.WebInterface.Auth;

namespace tdsm.core.WebInterface
{
    public class RequestEndException : Exception { }

    public enum ResourceType : byte
    {
        Javascript = 1,
        Stylesheet
    }
    public struct ResourceDependancy
    {
        public string Url { get; set; }
        public ResourceType Type { get; set; }
    }

    public abstract class WebPage
    {
        public abstract void ProcessRequest(WebRequest request);

        //public string[] Nodes { get; set; }
        public WebModule ModuleInfo { get; set; }

        public bool CanProcess(WebRequest request)
        {
            if (ModuleInfo == null || ModuleInfo.Nodes == null) return true;
            var has = false;
            foreach (var nd in ModuleInfo.Nodes)
            {
                if (nd == "*") return true;
                var perm = WebPermissions.IsPermitted(nd, request);
                if (perm == api.Permissions.Permission.Denied) return false;
                if (perm == api.Permissions.Permission.Permitted) has = true;
            }
            return has;
        }

        public virtual ResourceDependancy[] GetDependencies()
        {
            return null;
        }
    }

    public class WebModule : Attribute
    {
        public string Url { get; set; }

        public string[] Nodes { get; set; }

        public bool InterfaceModule { get; set; }
    }

    public static class WebServer
    {
        static IHttpAuth Authentication;

        public static Action<ISender, ArgumentList> WebAuthCommand
        {
            get
            {
                return (Authentication as HttpDigestAuth).WebAuthCommand;
            }
        }

        /// <summary>
        /// The name of the host to show to the web user
        /// </summary>
        public static string ProviderName { get; set; }

        /// <summary>
        /// Allows a user to disable serving web files from the server, but rather from another application such as nginx or apache.
        /// </summary>
        public static bool ServeWebFiles { get; set; }

        //private static System.Collections.Generic.Queue<String> _additionalModules = new System.Collections.Generic.Queue<String>();
        private static System.Collections.Generic.Dictionary<String, WebPage> _pages = new System.Collections.Generic.Dictionary<String, WebPage>();

        //        public static readonly string HtmlDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "WebInterface", "Files");
        //		public static readonly string HtmlDirectory = @"C:\Development\Sync\Terraria-s-Dedicated-Server-Mod\tdsm-core\WebInterface\Files";
        public static readonly string HtmlDirectory = Path.GetFullPath(Environment.CurrentDirectory + @"/../../../../tdsm-core/WebInterface/Files");
        //        public static readonly string HtmlDirectory = Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\..\tdsm-core\WebInterface\Files");

        //public static bool RegisterModule(string path)
        //{
        //    if (_additionalModules.Contains(path)) return false;
        //    lock (_additionalModules) _additionalModules.Enqueue(path);
        //    return true;
        //}

        static bool _exit;
        static TcpListener _server;

        //TODO: use reflection in plugins
        public static bool RegisterPage(string request, WebPage page)
        {
            if (_pages.ContainsKey(request)) return false;
            lock (_pages) _pages.Add(request, page);
            return true;
        }

        public delegate void ForEachTileFunc(WebPage page);

        public static void ForEachPage(ForEachTileFunc fnc)
        {
            lock (_pages)
            {
                foreach (var item in _pages)
                    fnc.Invoke(item.Value);
            }
        }

        private static List<KeyValuePair<String, WebPage>> GatherWebPages()
        {
            var pages = new List<KeyValuePair<String, WebPage>>();

            Type type = typeof(WebPage), moduleType = typeof(WebModule);
            foreach (Type pageType in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(clazz => clazz.GetTypes())
                .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
            {
                var custom = pageType.GetCustomAttributes(false);
                var module = custom.Where(x => moduleType.IsAssignableFrom(x.GetType())).FirstOrDefault();
                if (module != null)
                {
                    var wp = (WebPage)Activator.CreateInstance(pageType);
                    //wp.Nodes = ((WebModule)module).Nodes;
                    wp.ModuleInfo = module as WebModule;
                    pages.Add(new KeyValuePair<String, WebPage>(((WebModule)module).Url, wp));
                }
            }

            return pages;
        }

        public static void Begin(string bindAddress, string provider)
        {
#if ENABLED
            ProviderName = provider;
            Authentication = new HttpDigestAuth(Path.Combine(tdsm.api.Globals.DataPath, "web.logins"));

            Authentication.CreateLogin("DeathCradle", "testing", ProviderName);

            //Console.WriteLine("Html directory: " + HtmlDirectory);

            var split = bindAddress.Split(':');
            IPAddress addr;
            ushort port;

            if (split.Length != 2 || !IPAddress.TryParse(split[0], out addr) || !ushort.TryParse(split[1], out port) || port < 1)
            {
                ProgramLog.Error.Log("{0} is not a valid bind address, web server disabled.", bindAddress);
                return;
            }
            _server = new TcpListener(addr, port);

            //if (RegisterModule("tdsm.admin.js")) 
            //if (!RegisterPage("/api/tiles.json", new JSPacket()))
            //{
            //    ProgramLog.Error.Log("Failed to register web api module.");
            //    return;
            //}

            foreach (var page in GatherWebPages())
            {
                if (!RegisterPage(page.Key, page.Value))
                {
                    ProgramLog.Error.Log("Failed to register web api module: " + page.Key);
                    return;
                }
            }

            _server.Start();

            (new System.Threading.Thread(() =>
            {
                System.Threading.Thread.CurrentThread.Name = "Web";
                ProgramLog.Admin.Log("Web server started on {0}.", bindAddress);

                _server.Server.Poll(500000, SelectMode.SelectRead);

                while (!_exit)
                {
                    //var client = = server.AcceptSocket();
                    //AcceptClient(client);
                    var handle = _server.BeginAcceptSocket(AcceptClient, _server);
                    handle.AsyncWaitHandle.WaitOne();
                }

                ProgramLog.Admin.Log("Web server exited.", bindAddress);
            })).Start();
#endif
        }

        public static void End()
        {
#if ENABLED
            _exit = true;
            _server.Stop();
#endif
        }

        static void AcceptClient(IAsyncResult result)
        {
            if (!_exit)
            {
                var server = result.AsyncState as TcpListener;
                var client = server.EndAcceptSocket(result);
                client.NoDelay = true;
                try
                {

                    string addr;
                    var rep = client.RemoteEndPoint;
                    if (rep != null)
                        addr = rep.ToString();
                    else
                    {
                        //return false;
                    }

                    var req = new WebRequest(client);
                    req.StartReceiving(new byte[4192]);
                }
                catch (RequestEndException) { return; }
                catch (SocketException)
                {
                    //client.SafeClose();
                }
                catch (Exception e)
                {
                    //return false;
                    Console.WriteLine(e);
                }

                client.SafeClose();
            }
        }

        static string HandleSocketException(Exception e)
        {
            if (e is SocketException)
                return e.Message;
            if (e is System.IO.IOException)
                return e.Message;
            else if (e is ObjectDisposedException)
                return "Socket already disposed";
            else
                throw new Exception("Unexpected exception in socket handling code", e);
        }

        private static FileInfo GetEncapsulated(WebRequest request)
        {
            if (request.Path != null)
            {
                var url = request.Path;
                if (url == "/") url = "index.html";
                url = url.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                while (url.StartsWith("\\") || url.StartsWith("/")) url = url.Remove(0, 1);
                var local = Path.Combine(WebServer.HtmlDirectory, url);

                if (Path.GetFullPath(local).StartsWith(WebServer.HtmlDirectory))
                {
                    var inf = new FileInfo(local);

                    //Allow minification alternates
                    var ex = inf.Extension.ToLower();
                    if (ex == ".js" || ex == ".css")
                    {
                        var min = new FileInfo(inf.FullName.Insert(inf.FullName.Length - inf.Extension.Length, "-min"));
                        if (min.Exists) inf = min;
                    }

                    return inf;
                }
            }
            return null;
        }

        private static readonly Dictionary<String, String> _contentTypes = new Dictionary<String, String>()
        {
            {".js", "application/javascript"},
            {".css", "text/css"}
        };

        internal static string Authenticate(WebRequest request)
        {
            return Authentication.Authenticate(request);
        }

        internal static void HandleRequest(WebRequest request, string content)
        {
            //var url = this.RequestUrl;
            //if (url == "/") url = "index.html";
            //var local = Path.Combine(/*WebServer.HtmlDirectory*/ @"C:\Development\Sync\Terraria-s-Dedicated-Server-Mod\tdsm-core\WebInterface\Files", EncapsulatePath(url));
            var local = GetEncapsulated(request);

            if (local != null && local.Exists)
            {
                //TODO implement cache

                using (var fs = local.OpenRead())
                {
                    var ext = local.Extension.ToLower();
                    var contentType = "text/html";
                    if (_contentTypes.ContainsKey(ext)) contentType = _contentTypes[ext];

                    request.RepsondHeader(200, "OK", contentType, fs.Length);

                    var buf = new byte[512];
                    while (fs.Position < fs.Length)
                    {
                        var read = fs.Read(buf, 0, buf.Length);
                        if (read > 0)
                        {
                            request.Send(buf, read, SocketFlags.None);
                        }
                        else break;
                    }
                }

                request.End();
            }
            else
            {
                //var segments = request.RequestUrl.Split('/');
                if (request.Path != null)
                    if (_pages.ContainsKey(request.Path))
                    {
                        if (_pages[request.Path].CanProcess(request))
                        {
                            _pages[request.Path].ProcessRequest(request);
                            request.End();
                        }
                        else
                        {
                            request.RepsondHeader(404, "Not Found", "text/html", 0);
                            request.End();
                        }
                    }
            }
        }
    }
}
