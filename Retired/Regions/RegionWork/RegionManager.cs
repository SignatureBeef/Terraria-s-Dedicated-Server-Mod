using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Logging;
using System.IO;
using Terraria_Server.Misc;

namespace Regions.RegionWork
{
    public class RegionManager
    {
        public List<Region> Regions { get; set; }
        private string SaveFolder { get; set; }

        public RegionManager(string saveFolder)
        {
            SaveFolder = saveFolder;
			Regions = new List<Region>();

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);
        }

        public void LoadRegions()
        {
            ProgramLog.Plugin.Log("Loading Regions.");
            Regions = LoadRegions(SaveFolder);
            ProgramLog.Plugin.Log("Loaded {0} Regions.", Regions.Count);
        }

        public bool SaveRegion(Region region)
        {
            FileStream fs = null;
            try
            {
                if (region != null && region.IsValidRegion())
                {
                    string file = SaveFolder + Path.DirectorySeparatorChar + region.Name + ".rgn";

                    if (File.Exists(file))
                        File.Delete(file);

                    fs = File.Open(file, FileMode.CreateNew);
                    string toWrite = region.ToString();
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

        public Region LoadRegion(string location)
        {
            Region region = new Region();

            string Name = "";
            string Description = "";
            Vector2 Point1 = default(Vector2);
            Vector2 Point2 = default(Vector2);
            List<String> Users = new List<String>();
            List<String> Projectiles = new List<String>();
            bool Restricted = false;
            bool RestrictedNPCs = false;

            foreach (string line in File.ReadAllLines(location))
            {
                if (line.Contains(":"))
                {
                    string key = line.Split(':')[0];
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
                                string[] xy = line.Remove(0, line.IndexOf(":") + 1).Trim().Split(',');
                                float x, y;
                                if (!(float.TryParse(xy[0], out x) && float.TryParse(xy[1], out y)))
                                    Point1 = default(Vector2);
                                else
                                    Point1 = new Vector2(x, y);
                                break;
                            }
                        case "point2":
                            {
                                string[] xy = line.Remove(0, line.IndexOf(":") + 1).Trim().Split(',');
                                float x, y;
                                if (!(float.TryParse(xy[0], out x) && float.TryParse(xy[1], out y)))
                                    Point2 = default(Vector2);
                                else
                                    Point2 = new Vector2(x, y);
                                break;
                            }
                        case "users":
                            {
                                string userlist = line.Remove(0, line.IndexOf(":") + 1).Trim();
                                if(userlist.Length > 0)
                                    Users = userlist.Split(' ').ToList<String>();
                                break;
                            }
                        case "projectiles":
                            {
                                string userlist = line.Remove(0, line.IndexOf(":") + 1).Trim();
                                if(userlist.Length > 0)
                                    Projectiles = userlist.Split(' ').ToList<String>();
                                break;
                            }
                        case "restricted":
                            {
                                string restricted = line.Remove(0, line.IndexOf(":") + 1).Trim();
                                bool restrict;
                                if (Boolean.TryParse(restricted, out restrict))
                                    Restricted = restrict;
                                break;
                            }
                        case "npcrestrict":
                            {
                                string restricted = line.Remove(0, line.IndexOf(":") + 1).Trim();
                                bool restrict;
                                if (Boolean.TryParse(restricted, out restrict))
                                    RestrictedNPCs = restrict;
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
            region.ProjectileList = Projectiles;
            region.Restricted = Restricted;
            region.RestrictedNPCs = RestrictedNPCs;

            return region.IsValidRegion() ? region : null;
        }

        public List<Region> LoadRegions(string folder)
        {
            List<Region> rgns = new List<Region>();
            foreach (string file in Directory.GetFiles(folder))
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

        public bool ContainsRegion(string name)
        {
            foreach (Region rgn in Regions)
            {
                if (rgn.Name.ToLower().Equals(name.ToLower()))
                    return true;
            }

            return false;
        }

        public Region GetRegion(string name)
        {
            foreach (Region rgn in Regions)
            {
                if (rgn.Name.ToLower().Equals(name.ToLower()))
                    return rgn;
            }

            return null;
        }
    }
}
