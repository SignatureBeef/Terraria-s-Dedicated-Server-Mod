using System.IO;
using System.Collections;
using System;

namespace Terraria_Server.Misc
{
    public class DataRegister
    {
        public DataRegister(String Location)
        {
            FilePath = Location;
        }

        public ArrayList WhiteList { get; set; }

        public String FilePath { get; set; }

        public bool containsException(String exception)
        {
            return WhiteList.Contains(exception.Trim().ToLower());
        }

        public void addException(String exception)
        {

            if (!WhiteList.Contains(exception.Trim().ToLower()))
            {
                WhiteList.Add(exception.Trim().ToLower());
            }
        }

        public bool removeException(String exception)
        {
            bool pass = false;
            if (WhiteList.Contains(exception.Trim().ToLower()))
            {
                WhiteList.Remove(exception.Trim().ToLower());
            }
            return pass;
        }

        public void Load()
        {
            WhiteList = new ArrayList();

            if (File.Exists(FilePath))
            {
                String[] list = File.ReadAllLines(FilePath);
                if (list != null)
                {
                    foreach (String listee in list)
                    {
                        if (listee != null && listee.Trim().ToLower().Length > 0)
                        {
                            WhiteList.Add(listee.Trim().ToLower());
                        }
                    }
                }
            }
        }

        public bool Save()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                } catch {
                    return false;
                }
            }

            File.WriteAllLines(FilePath, WhiteList.ToArray(typeof(String)) as String[]);

            return true;
        }
    }
}
