#define RESPONSE_TYPE_JSON

using System;
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

        public static void PerformUpdate()
        {
            //using (var fs = System.IO.File.OpenRead("package.zip"))
            //{
            //    using (var pk = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Decompress))
            //    {

            //    }
            //}

            //var zip = System.IO.Packaging.ZipPackage.Open("package.zip");

            //foreach (var part in zip.GetParts())
            //{

            //    //if (part.ContentType.ToLowerInvariant().StartsWith("image/"))
            //    //{
            //    //    string target = Path.Combine(
            //    //        dir.FullName, CreateFilenameFromUri(part.Uri));
            //    //    using (Stream source = part.GetStream(
            //    //        FileMode.Open, FileAccess.Read))
            //    //    using (Stream destination = File.OpenWrite(target))
            //    //    {
            //    //        byte[] buffer = new byte[0x1000];
            //    //        int read;
            //    //        while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            //    //        {
            //    //            destination.Write(buffer, 0, read);
            //    //        }
            //    //    }
            //    //    Console.WriteLine("Extracted {0}", target);
            //    //}
            //}
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
        /// <summary>
        /// The source file in the ZIP package
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string PackageFileName { get; set; }

        /// <summary>
        /// The directory
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string[] PackageFileDirectories { get; set; }

        /// <summary>
        /// The destination to be installed to relative to the TDSM installation directory
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string DestinationFileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string[] DestinationDirectories { get; set; }
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
