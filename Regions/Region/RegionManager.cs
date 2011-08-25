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
        public List<Region> Regions { get; set; }
        private String SaveFolder { get; set; }

        public RegionManager(String saveFolder)
        {
            SaveFolder = saveFolder;

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            ProgramLog.Log("Loading Regions.");
            Regions = LoadRegions(saveFolder);
            ProgramLog.Log("Loaded {0} Regions.", Regions.Count);
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
                if (fs != null)
                    fs.Close();
            }
            return false;
        }

        public Region LoadRegion(String location)
        {
            Region region = new Region();

            String Name = "";
            String Description = "";
            Vector2 Point1 = default(Vector2);
            Vector2 Point2 = default(Vector2);
            List<String> Users = new List<String>();
            Boolean Restricted = false;

            foreach (String line in File.ReadAllLines(location))
            {
                if (line.Contains(":"))
                {
                    String key = line.Split(':')[0];
                    switch (key)
                    {
                        case "name":
                            {
                                Name = line.Remove(0, line.IndexOf(":") + 1).Trim();
                                break;
                            }
                        case "description":
                            {
                                Description = line.Remove(0, line.IndexOf(":") + 1).Trim();
                                break;
                            }
                        case "point1":
                            {
                                String[] xy = line.Remove(0, line.IndexOf(":") + 1).Trim().Split(',');
                                float x, y;
                                if (!(float.TryParse(xy[0], out x) && float.TryParse(xy[1], out y)))
                                    Point1 = default(Vector2);
                                else
                                    Point1 = new Vector2(x, y);
                                break;
                            }
                        case "point2":
                            {
                                String[] xy = line.Remove(0, line.IndexOf(":") + 1).Trim().Split(',');
                                float x, y;
                                if (!(float.TryParse(xy[0], out x) && float.TryParse(xy[1], out y)))
                                    Point2 = default(Vector2);
                                else
                                    Point2 = new Vector2(x, y);
                                break;
                            }
                        case "users":
                            {
                                String userlist = line.Remove(0, line.IndexOf(":") + 1).Trim();
                                if(userlist.Length > 0)
                                    Users = userlist.Split(' ').ToList<String>();
                                break;
                            }
                        case "restricted":
                            {
                                String restricted = line.Remove(0, line.IndexOf(":") + 1).Trim();
                                Boolean restrict;
                                if (Boolean.TryParse(restricted, out restrict))
                                    Restricted = restrict;
                                break;
                            }
                        default: continue;
                    }
                }
            }

            region.Name = Name;
            region.Description = Description;
            region.Point1 = Point1;
            region.Point2 = Point2;
            region.UserList = Users;
            region.Restricted = Restricted;

            return region.IsValidRegion() ? region : null;
        }

        public List<Region> LoadRegions(String folder)
        {
            List<Region> rgns = new List<Region>();
            foreach (String file in Directory.GetFiles(folder))
            {
                if (file.ToLower().EndsWith(".rgn"))
                {
                    Region rgn = LoadRegion(file);
                    if (rgn != null)
                        rgns.Add(rgn);
                }
            }
            return rgns;
        }
    }
}
