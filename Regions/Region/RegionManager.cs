using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Logging;
using System.IO;
using Terraria_Server.Misc;

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

        public Region LoadRegion(String location)
        {
            Region region = null;

            String Name = "";
            String Description = "";
            Vector2 Point1 = default(Vector2);
            Vector2 Point2 = default(Vector2);
            List<String> Users = new List<String>();

            foreach (String line in File.ReadAllLines(location))
            {
                if(line.Contains(":"))
                {
                    String key = line.Split(':')[0];
                    switch (key)
                    {
                        case "name":
                            {
                                Name = line.Remove(0, line.IndexOf(":")).Trim();
                                break;
                            }
                        case "decription":
                            {
                                Description = line.Remove(0, line.IndexOf(":")).Trim();
                                break;
                            }
                        case "point1":
                            {
                                String[] xy = line.Remove(0, line.IndexOf(":")).Trim().Split(',');
                                float x, y;
                                if (!(float.TryParse(xy[0], out x) && float.TryParse(xy[1], out y)))
                                    Point1 = default(Vector2);
                                else
                                    Point2 = new Vector2(x, y);
                                break;
                            }
                        case "point2":
                            {
                                String[] xy = line.Remove(0, line.IndexOf(":")).Trim().Split(',');
                                float x, y;
                                if (!(float.TryParse(xy[0], out x) && float.TryParse(xy[1], out y)))
                                    Point2 = default(Vector2);
                                else
                                    Point2 = new Vector2(x, y);
                                break;
                            }
                        case "users":
                            {
                                String userlist = line.Remove(0, line.IndexOf(":")).Trim();
                                Users = userlist.Split(' ').ToList<String>();
                                break;
                            }
                        default: continue;
                    }
                }
            }

            region = new Region();
            region.Name = Name;
            region.Description = Description;
            region.Point1 = Point1;
            region.Point2 = Point2;
            region.UserList = Users;
            
            return region.IsValidRegion() ? region : null;
        }
    }
}
