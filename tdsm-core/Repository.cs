#define RESPONSE_TYPE_JSON

using System;
using System.Linq;
using System.Net;
using tdsm.api;
using tdsm.core.Logging;
namespace tdsm.core
{
    public static class Repository
    {
        //No command implementation to live here
        //See Command.PluginRepo.cs

        public static PackageInfo[] GetAvailableUpdates()
        {
            return GetAvailableUpdate();
        }

        public static PackageInfo[] GetAvailableUpdate(string packageName = null)
        {
            if (String.IsNullOrEmpty(packageName))
            {
                var info = new PackageInfo[PluginManager.PluginCount];

                var pending = 0;
                foreach (var plg in PluginManager.EnumeratePlugins)
                {
                    var res = GetUpdateInfo(plg.Name, plg.Version);
                    if (res != null) info[pending++] = res;
                }

                if (pending != info.Length)
                {
                    System.Array.Resize(ref info, pending);
                }

                return info;
            }
            else
            {
                var res = GetUpdateInfo(packageName);
                if (res != null) return new PackageInfo[] { res };
            }

            return null;
        }

        class GitHubRelease
        {
            [Newtonsoft.Json.JsonProperty("assets")]
            public GitHubAssets[] Assets { get; set; }

            [Newtonsoft.Json.JsonProperty("published_at")]
            public DateTime PublishedAt { get; set; }
        }

        class GitHubAssets
        {
            [Newtonsoft.Json.JsonProperty("browser_download_url")]
            public string DownloadUrl { get; set; }

            [Newtonsoft.Json.JsonProperty("name")]
            public string FileName { get; set; }
        }

        static GitHubRelease GetGitHubRelease(string url)
        {
            url = "https://api.github.com/repos/DeathCradle/Terraria-s-Dedicated-Server-Mod/releases";
            try
            {
                var wr = WebRequest.Create(url) as HttpWebRequest;
                wr.UserAgent = "TDSM";
                wr.Method = "GET";

                string json = null;
                using (var sr = new System.IO.StreamReader(wr.GetResponse().GetResponseStream()))
                {
                    json = sr.ReadToEnd();
                }

                if (!String.IsNullOrEmpty(json))
                {
                    var releases = Newtonsoft.Json.JsonConvert.DeserializeObject<GitHubRelease[]>(json).OrderByDescending(x => x.PublishedAt);
                    if (releases.Count() > 0) return releases.First();
                }
            }
            catch
            {
                //Perhaps we need a [CurrentCommand].AddError
            }
            return null;
        }

        public static string FetchUpdate(PackageInfo info)
        {
            var tmp = System.IO.Path.Combine(Environment.CurrentDirectory, ".repo");
            var di = new System.IO.DirectoryInfo(tmp);
            if (!di.Exists)
            {
                di.Create();
                di.Attributes = System.IO.FileAttributes.Hidden;
            }

            var isGitHub = System.Text.RegularExpressions.Regex.IsMatch(info.DownloadUrl, "https://api.github.com/repos/.*/.*/releases", System.Text.RegularExpressions.RegexOptions.Singleline);
            if (isGitHub)
            {
                var release = GetGitHubRelease(info.DownloadUrl);
                if (release.Assets != null && release.Assets.Length > 0)
                {
                    var saveAs = System.IO.Path.Combine(tmp, release.Assets[0].FileName);
                    //if (System.IO.File.Exists(saveAs)) System.IO.File.Delete(saveAs);
                    //using (var wc = new ProgressWebClient("Downloading"))
                    //{
                    //    var wait = new System.Threading.AutoResetEvent(false);
                    //    wc.DownloadFileCompleted += (s, a) =>
                    //    {
                    //        wait.Set();
                    //    };
                    //    wc.DownloadFileAsync(new Uri(release.Assets[0].DownloadUrl), saveAs);

                    //    wait.WaitOne();
                    return saveAs;
                    //}
                }
            }
            else
            {
                var fileName = info.DownloadUrl;
                var last = fileName.LastIndexOf("/");
                if (last > -1)
                {
                    fileName = fileName.Remove(0, last);
                }
            }

            return null;
        }

        private static string ExtractZip(System.IO.FileInfo info)
        {
            var extractDir = System.IO.Path.Combine(info.Directory.FullName, info.Name.Substring(0, info.Name.Length - info.Extension.Length));
            if (!System.IO.Directory.Exists(extractDir)) System.IO.Directory.CreateDirectory(extractDir);

            var zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(info.FullName);
            foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry entry in zip)
            {
                var dst = System.IO.Path.Combine(extractDir, entry.Name.Replace('/', System.IO.Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(dst)) System.IO.File.Delete(dst);

                var inf = new System.IO.FileInfo(dst);
                if (!inf.Directory.Exists) inf.Directory.Create();

                using (var zipStream = zip.GetInputStream(entry))
                {
                    using (var output = System.IO.File.OpenWrite(dst))
                    {
                        var buffer = new byte[4096];

                        while (zipStream.Position < zipStream.Length)
                        {
                            var read = zipStream.Read(buffer, 0, buffer.Length);
                            output.Write(buffer, 0, read);
                        }
                    }
                }
            }

            return null;
        }

        public static bool InstallUpdate(string path)
        {
            //Extract ZIP's into a seperate folder
            //Install everything else as a plugin

            var info = new System.IO.FileInfo(path);

            var ext = info.Extension.ToLower();
            if (ext == ".zip")
            {
                //Extract into a temporary directory incase of zip exceptions that could potentially corrupt our installation
                var tmp = ExtractZip(info);
                if (!String.IsNullOrEmpty(tmp))
                {
                    //Read package
                    foreach (var fl in System.IO.Directory.GetFiles(tmp, "*/*", System.IO.SearchOption.AllDirectories))
                    {
                        //if fl in package then
                        //  Move and replace targets
                        //end if
                    }
                }
            }
            else if (ext == ".lua" || ext == ".dll")
            {
                //Move and replace targets
            }

            return false;
        }

        public static bool PerformUpdate(PackageInfo info)
        {
            var upd = FetchUpdate(info);
            if (!String.IsNullOrEmpty(upd))
            {
                return InstallUpdate(upd);
            }

            return false;
        }

        private static PackageInfo GetUpdateInfo(string packageName, string currentVersion = null)
        {
            string data = null;
            try
            {
                using (var wc = new WebClient())
                {
                    var url = GetUrl_PackageInfoUrl(Globals.Build, (short)Globals.BuildPhase, packageName, currentVersion);
                    data = wc.DownloadString(url);
                }
            }
            catch (Exception e)
            {
                ProgramLog.Error.Log("Failed to fetch package", e);
            }

            //Console.WriteLine(packageName);
            //Console.WriteLine(data ?? "null data");

#if RESPONSE_TYPE_JSON
            var pkg = Newtonsoft.Json.JsonConvert.DeserializeObject<PackageInfo[]>(data);
            if (pkg != null && pkg.Length == 1) return pkg[0];
#else
#endif

            return null;
        }

        private static string GetUrl_PackageInfoUrl(int apiBuild, short apiPhase, string packageName, string currentVersion = null)
        {
#if RESPONSE_TYPE_JSON
            const String Fmt = "http://heartbeat.tdsm.org/query.php?code=5&APIBuild={0}&APIPhase={1}&Package={2}{3}{4}";
#else
            const String Fmt = "http://heartbeat.tdsm.org/query.php?code=5&APIBuild={0}&APIPhase={1}&Package={2}{3}{4}&output=xml";
#endif
            const String cv = "&CurrentVersion=";
            return String.Format(Fmt, apiBuild, apiPhase, packageName, currentVersion == null ? String.Empty : cv, currentVersion);
        }
    }

    public class UpdatePackage //package.xml
    {
        [System.Xml.Serialization.XmlArray]
        public UpdateInstruction[] Instructions { get; set; }
    }

    public class UpdateInstruction
    {
        public string DirectorySeperator = "\\";

        /// <summary>
        /// The source file in the ZIP package
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string PackageFileName { get; set; }

        /// <summary>
        /// The destination to be installed to relative to the TDSM installation directory
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string DestinationFileName { get; set; } //If blank use PackageFileName
    }

    public class PackageInfo
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Version { get; set; }
        public string DownloadUrl { get; set; }
    }

    public static class RPC
    {
        public class methodResponse
        {
            [System.Xml.Serialization.XmlElement("params")]
            public param[] Parameters;
        }

        public class param
        {

        }
    }
}
