using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Logging;
using System.IO;

namespace Regions.Region
{
    public class RegionManager
    {
        List<Region> regions { get; set; }
        private String SaveFolder { get; set; }

        public RegionManager(String saveFolder)
        {
            SaveFolder = saveFolder;

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);
        }

        public Boolean SaveRegion(Region region)
        {
            FileStream fs = null;
            try
            {
                if (region != null && region.IsValidRegion())
                {
                    String file = SaveFolder + Path.DirectorySeparatorChar + region.Name + ".rgn";

                    if (File.Exists(file))
                        File.Delete(file);

                    fs = File.Open(file, FileMode.CreateNew);
                    String toWrite = region.ToString();
                    fs.Write(ASCIIEncoding.ASCII.GetBytes(toWrite), 0, toWrite.Length);
                    fs.Flush();
                    fs.Close();
                    return true;
                }
                else
                    ProgramLog.Error.Log("Region '{0}' was either null or has an issue.",
                        (region != null && region.Name != null) ? region.Name : "??");
            }
            catch (Exception e)
            {
                ProgramLog.Error.Log("Error saving Region {0}\n{1}", region.Name, e.Message);
            }
            finally
            {
                if(fs != null)
                    fs.Close();
            }
            return false;
        }

    }
}
