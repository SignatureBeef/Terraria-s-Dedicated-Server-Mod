using System.IO;
using System.Collections;
using System;

namespace Terraria_Server.Misc
{
    public class DataRegister
    {
        public DataRegister(string Location)
        {
            FilePath = Location;
        }

        public ArrayList WhiteList { get; set; }

        public string FilePath { get; set; }

        public bool containsException(string exception)
        {
            return WhiteList.Contains(exception.Trim().ToLower());
        }

        public void addException(string exception, bool filter = false, int filterLength = -1)
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

        public bool removeException(string exception)
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
                string exception = WhiteList[i].ToString();

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
                string[] list = File.ReadAllLines(FilePath);
                if (list != null)
                {
                    foreach (string listee in list)
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
