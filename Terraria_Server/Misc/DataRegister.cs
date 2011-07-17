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

        public void addException(String exception, bool filter = false, int filterLength = -1)
        {
            if (!WhiteList.Contains(exception.Trim().ToLower()))
            {
                WhiteList.Add(exception.Trim().ToLower());
            }
            if (filter)
            {
                if (filterLength == -1)
                {
                    filterLength = exception.Length;
                }
                filterExceptions(filterLength);
            }
        }

        public bool removeException(String exception)
        {
            bool removed = false;
            if (WhiteList.Contains(exception.Trim().ToLower()))
            {
                WhiteList.Remove(exception.Trim().ToLower());
                removed = true;
            }
            return removed;
        }

        public void filterExceptions(int filterLength)
        {
            ArrayList cleanedWhiteList = new ArrayList();
            for (int i = 0; i < WhiteList.Count; i++)
            {
                bool contained = false;
                String exception = WhiteList[i].ToString();

                if (exception.Length > filterLength)
                {
                    exception = exception.Substring(0, filterLength);
                }

                for (int x = 0; x < cleanedWhiteList.Count; x++)
                {
                    if (cleanedWhiteList[x].ToString().StartsWith(exception))
                    {
                        contained = true;
                    }
                }
                if (!contained)
                {
                    cleanedWhiteList.Add(WhiteList[i]);
                }
            }
            WhiteList = (ArrayList)cleanedWhiteList.Clone();
            cleanedWhiteList.Clear();
            cleanedWhiteList = null;
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
