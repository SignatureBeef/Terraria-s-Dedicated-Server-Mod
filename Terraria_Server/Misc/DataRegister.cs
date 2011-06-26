using System.IO;
using System.Collections;

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

        public void addException(string exception)
        {

            if (!WhiteList.Contains(exception.Trim().ToLower()))
            {
                WhiteList.Add(exception.Trim().ToLower());
            }
        }

        public bool removeException(string exception)
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

            File.WriteAllLines(FilePath, WhiteList.ToArray(typeof(string)) as string[]);

            return true;
        }
    }
}
