#define RESPONSE_TYPE_JSON

using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using System.Net;
using OTA;
using OTA.Logging;
using OTA.Misc;


namespace TDSM.Core
{
    public class RepositoryError : Exception
    {
        public RepositoryError(string message, params object[] args) : base(String.Format(message, args)) { }
    }

    public static class Repository
    {
        #region "Non public"
        //No command implementation to live here
        //See Command.PluginRepo.cs

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

        private static GitHubRelease GetGitHubRelease(string url)
        {
            try
            {
                var wr = WebRequest.Create(url) as HttpWebRequest;
                wr.UserAgent = "TDSM";
                wr.Method = "GET";

                string json = null;
                using (var sr = new StreamReader(wr.GetResponse().GetResponseStream()))
                {
                    json = sr.ReadToEnd();
                }

                if (!String.IsNullOrEmpty(json))
                {
                    var releases = Newtonsoft.Json.JsonConvert.DeserializeObject<GitHubRelease[]>(json).OrderByDescending(x => x.PublishedAt);
                    if (releases.Count() > 0) return releases.First();
                }
            }
            catch { }
            return null;
        }

        private static string ExtractZip(FileInfo info)
        {
            var extractDir = Path.Combine(info.Directory.FullName, info.Name.Substring(0, info.Name.Length - info.Extension.Length));
            if (!Directory.Exists(extractDir)) Directory.CreateDirectory(extractDir);

            var zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(info.FullName);
            foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry entry in zip)
            {
                var dst = Path.Combine(extractDir, entry.Name.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(dst)) File.Delete(dst);

                var inf = new FileInfo(dst);
                if (!inf.Directory.Exists) inf.Directory.Create();

                using (var zipStream = zip.GetInputStream(entry))
                {
                    using (var output = File.OpenWrite(dst))
                    {
                        var buffer = new byte[4096];

                        while (zipStream.Position < entry.Size)
                        {
                            var read = zipStream.Read(buffer, 0, buffer.Length);
                            if (read > 0) output.Write(buffer, 0, read);
                            else break;
                        }
                    }
                }
            }
            zip.Close();

            return extractDir;
        }

        private static string IsChild(string path)
        {
            if (Path.GetFullPath(path).StartsWith(Environment.CurrentDirectory))
                return path;

            return null;
        }

        private static PackageInfo GetUpdateInfo(string packageName, string currentVersion = null)
        {
            string data = null;
            //TODO uncommented until the new DB is in
//            try
//            {
//                using (var wc = new WebClient())
//                {
//                    var url = GetUrl_PackageInfoUrl(Globals.Build, (short)Globals.BuildPhase, packageName, currentVersion);
//                    data = wc.DownloadString(url);
//                }
//            }
//            catch (Exception e)
//            {
//                ProgramLog.Error.Log("Failed to fetch package", e);
//                return null;
//            }

#if RESPONSE_TYPE_JSON
            try
            {
                var pkg = Newtonsoft.Json.JsonConvert.DeserializeObject<PackageInfo[]>(data);
                if (pkg != null && pkg.Length == 1) return pkg[0];
            }
            catch (Exception e)
            {
                ProgramLog.Error.Log("Failed to parse package info", e);
            }
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

        private static string FetchUpdate(PackageInfo info)
        {
            var tmp = Path.Combine(Environment.CurrentDirectory, ".repo");
            var di = new DirectoryInfo(tmp);
            if (!di.Exists)
            {
                di.Create();
                di.Attributes = FileAttributes.Hidden;
            }

            var isGitHub = System.Text.RegularExpressions.Regex.IsMatch(info.DownloadUrl, "https://api.github.com/repos/.*/.*/releases", System.Text.RegularExpressions.RegexOptions.Singleline);
            if (isGitHub)
            {
                var release = GetGitHubRelease(info.DownloadUrl);
                if (release.Assets != null && release.Assets.Length > 0)
                {
                    var saveAs = Path.Combine(tmp, release.Assets[0].FileName);
                    if (File.Exists(saveAs)) File.Delete(saveAs);
                    using (var wc = new ProgressWebClient("Downloading"))
                    {
                        var wait = new System.Threading.AutoResetEvent(false);
                        wc.DownloadFileCompleted += (s, a) =>
                        {
                            wait.Set();
                        };
                        wc.DownloadFileAsync(new Uri(release.Assets[0].DownloadUrl), saveAs);

                        wait.WaitOne();
                        return saveAs;
                    }
                }
                else throw new RepositoryError("Failed to fetch release from GitHub");
            }
            else
            {
                var fileName = info.DownloadUrl;
                var last = fileName.LastIndexOf("/");
                if (last > -1)
                {
                    fileName = fileName.Remove(0, last);
                }

                var saveAs = Path.Combine(tmp, fileName);
                if (File.Exists(saveAs)) File.Delete(saveAs);
                using (var wc = new ProgressWebClient("Downloading"))
                {
                    var wait = new System.Threading.AutoResetEvent(false);
                    wc.DownloadFileCompleted += (s, a) =>
                    {
                        wait.Set();
                    };
                    wc.DownloadFileAsync(new Uri(info.DownloadUrl), saveAs);

                    wait.WaitOne();
                    return saveAs;
                }
            }
        }

        private static string[] InstallUpdate(string path)
        {
            //Extract ZIP's into a seperate folder
            //Install everything else as a plugin

            var info = new FileInfo(path);

            var ext = info.Extension.ToLower();
            if (ext == ".zip")
            {
                //Extract into a temporary directory incase of zip exceptions that could potentially corrupt our installation
                var tmp = ExtractZip(info);
                if (!String.IsNullOrEmpty(tmp))
                {
                    //Read package
                    var pkgFile = Path.Combine(tmp, "package.json");
                    if (File.Exists(pkgFile))
                    {
                        var pkg = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdatePackage>(File.ReadAllText(pkgFile));
                        if (pkg != null && pkg.Instructions != null && pkg.Instructions.Length > 0)
                        {
                            //Verify first then install
                            using (var logger = new ProgressLogger(pkg.Instructions.Length, "Verifying package"))
                            {
                                foreach (var ins in pkg.Instructions)
                                {
                                    if (String.IsNullOrEmpty(ins.PackageFileName)) throw new RepositoryError("Invalid source file");
                                    if (String.IsNullOrEmpty(ins.DestinationFileName)) ins.DestinationFileName = ins.PackageFileName; //If not provided then it means it's the same

                                    var relPath = ins.PackageFileName.Split(ins.DirectorySeperator);
                                    relPath = new string[] { tmp }.Concat(relPath).ToArray();
                                    var from = Path.Combine(relPath);

                                    if (!File.Exists(from)) throw new RepositoryError("Source file {0} does not exist", ins.PackageFileName);

                                    var toPath = ins.PackageFileName.Split(ins.DirectorySeperator);
                                    toPath = new string[] { Environment.CurrentDirectory }.Concat(toPath).ToArray();
                                    var to = Path.Combine(toPath);

                                    if (IsChild(to) == null) throw new RepositoryError("Destination file {0} is not within the TDSM install directory", ins.DestinationFileName);
                                }
                            }

                            using (var logger = new ProgressLogger(pkg.Instructions.Length, "Installing package"))
                            {
                                foreach (var ins in pkg.Instructions)
                                {
                                    var relPath = ins.PackageFileName.Split(ins.DirectorySeperator);
                                    relPath = new string[] { tmp }.Concat(relPath).ToArray();
                                    var from = Path.Combine(relPath);

                                    var toPath = ins.PackageFileName.Split(ins.DirectorySeperator);
                                    toPath = new string[] { Environment.CurrentDirectory }.Concat(toPath).ToArray();
                                    var to = IsChild(Path.Combine(toPath));

                                    if (File.Exists(to)) File.Delete(to);
                                    File.Move(from, to);
                                }
                            }

                            ProgramLog.Admin.Log("Cleaning up");
                            Directory.Delete(tmp, true);
                            File.Delete(path);

                            return pkg.PluginsToLoad;
                        }
                        else
                        {
                            throw new RepositoryError("No install instructions");
                        }
                    }
                    else
                    {
                        throw new RepositoryError("package.json is missing");
                    }
                }
                else
                {
                    throw new RepositoryError("Failed to extract package");
                }
            }
            else if (ext == ".lua" || ext == ".dll")
            {
                //Move and replace targets
                var dest = Path.Combine(Globals.PluginPath, info.Name);
                if (File.Exists(dest)) File.Delete(dest);
                File.Move(info.FullName, dest);

                return new string[] { dest };
            }
            else throw new RepositoryError("No package support for {0}", ext ?? "<unknown>");
        }
        #endregion

        /// <summary>
        /// Checks if all installed plugins are compatible with the latest recorded TDSM version
        /// </summary>
        public static void CheckForLatest()
        {

        }

        /// <summary>
        /// Perform and update
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool PerformUpdate(PackageInfo info)
        {
            var upd = FetchUpdate(info);
            if (!String.IsNullOrEmpty(upd))
            {
                var plugins = InstallUpdate(upd);

                foreach (var plg in plugins)
                {
                    var newPlugin = PluginManager.LoadPluginFromPath(plg);
                    if (newPlugin != null)
                    {
                        //Determine if we must replace an existing or enable a new plugin
                        var oldPlugin = PluginManager.GetPlugin(newPlugin.Name);
                        if (oldPlugin != null)
                        {
                            if (PluginManager.ReplacePlugin(oldPlugin, newPlugin))
                            {
                                ProgramLog.Admin.Log("Plugin {0} has been replaced.", oldPlugin.Name, Color.DodgerBlue);
                            }
                            else if (oldPlugin.IsDisposed)
                            {
                                ProgramLog.Admin.Log("Replacement of plugin {0} failed, it has been unloaded.", oldPlugin.Name, Color.DodgerBlue);
                            }
                            else
                            {
                                ProgramLog.Admin.Log("Replacement of plugin {0} failed, old instance kept.", oldPlugin.Name, Color.DodgerBlue);
                            }

                            break;
                        }

                        PluginManager.RegisterPlugin(newPlugin);
                    }
                }

                return plugins != null && plugins.Length > 0;
            }

            return false;
        }

        /// <summary>
        /// Get all available updates
        /// </summary>
        /// <returns></returns>
        public static PackageInfo[] GetAvailableUpdates()
        {
            return GetAvailableUpdate();
        }

        /// <summary>
        /// Get all available updates or check for a specific package update.
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public static PackageInfo[] GetAvailableUpdate(string packageName = null, bool updateOnly = true)
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
                string current = null, lowered = packageName.ToLower();
                foreach (var plg in PluginManager.EnumeratePlugins)
                {
                    if (plg.Name.ToLower() == lowered)
                    {
                        current = plg.Version;
                        break;
                    }
                }

                if (updateOnly && String.IsNullOrEmpty(current))
                {
                    throw new RepositoryError("No installed package by name {0}", packageName);
                }

                if (!updateOnly && !String.IsNullOrEmpty(current))
                {
                    throw new RepositoryError("{0} is already installed", packageName);
                }

                var res = GetUpdateInfo(packageName, current);
                if (res != null) return new PackageInfo[] { res };
            }

            return null;
        }
    }

    public class UpdatePackage //package.xml
    {
        [System.Xml.Serialization.XmlArray]
        public UpdateInstruction[] Instructions { get; set; }

        public string[] PluginsToLoad { get; set; }
    }

    public class UpdateInstruction
    {
        public char DirectorySeperator = '\\';

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

    //public static class RPC
    //{
    //    public class methodResponse
    //    {
    //        [System.Xml.Serialization.XmlElement("params")]
    //        public param[] Parameters;
    //    }

    //    public class param
    //    {

    //    }
    //}
}
