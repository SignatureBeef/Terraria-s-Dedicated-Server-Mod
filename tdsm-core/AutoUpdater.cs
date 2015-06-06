using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tdsm.api.Command;

namespace tdsm.core
{
    public static class AutoUpdater
    {
        public static void ShowPackagesForUpdate(ISender sender, ArgumentList args)
        {
            //using (var fs = System.IO.File.OpenRead("package.zip"))
            //{
            //    using (var pk = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Decompress))
            //    {

            //    }
            //}

            var zip = System.IO.Packaging.ZipPackage.Open("package.zip");

            foreach (var part in zip.GetParts())
            {

                //if (part.ContentType.ToLowerInvariant().StartsWith("image/"))
                //{
                //    string target = Path.Combine(
                //        dir.FullName, CreateFilenameFromUri(part.Uri));
                //    using (Stream source = part.GetStream(
                //        FileMode.Open, FileAccess.Read))
                //    using (Stream destination = File.OpenWrite(target))
                //    {
                //        byte[] buffer = new byte[0x1000];
                //        int read;
                //        while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
                //        {
                //            destination.Write(buffer, 0, read);
                //        }
                //    }
                //    Console.WriteLine("Extracted {0}", target);
                //}
            }
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
}
