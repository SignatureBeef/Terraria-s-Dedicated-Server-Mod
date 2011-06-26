
using System.IO;
using System.Collections;
namespace Terraria_Server
{
    public class DataRegister
    {
        private string filePath = "";

        public DataRegister(string Location)
        {
            FilePath = Location;
        }


        public ArrayList WhiteList { get; set; }

        public string FilePath { get; set; }

        public bool containsException(string Exception)
        {
            return WhiteList.Contains(Exception.Trim().ToLower());
        }

        public void addException(string Exception)
        {
            if (!WhiteList.Contains(Exception.Trim().ToLower()))
            {
                WhiteList.Add(Exception.Trim().ToLower());
            }
        }

        public bool removeException(string Exception)
        {
            bool pass = false;
            if (WhiteList.Contains(Exception.Trim().ToLower()))
            {
                WhiteList.Remove(Exception.Trim().ToLower());
            }
            return pass;
        }

        public void Load()
        {
            WhiteList = new ArrayList();

            if (System.IO.File.Exists(filePath))
            {
                string[] list = System.IO.File.ReadAllLines(filePath);
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
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                } catch {
                    return false;
                }
            }

            File.WriteAllLines(filePath, WhiteList.ToArray(typeof(string)) as string[]);

            return true;
        }
    }
}
