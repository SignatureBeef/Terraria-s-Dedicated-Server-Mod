using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Terraria_Server
{
    public class DataRegister
    {
        private ArrayList dataList = null;
        private string filePath = "";

        public DataRegister(string Location)
        {
            filePath = Location;
        }

        public string getFilePath()
        {
            return filePath;
        }

        public void setFilePath(string Location)
        {
            filePath = Location;
        }

        public bool containsException(string Exception)
        {
            return dataList.Contains(Exception.Trim().ToLower());
        }

        public void addException(string Exception)
        {
            if (!dataList.Contains(Exception.Trim().ToLower()))
            {
                dataList.Add(Exception.Trim().ToLower());
            }
        }

        public bool removeException(string Exception)
        {
            bool pass = false;
            if (dataList.Contains(Exception.Trim().ToLower()))
            {
                dataList.Remove(Exception.Trim().ToLower());
            }
            return pass;
        }

        public void Load()
        {
            dataList = new ArrayList();

            if (System.IO.File.Exists(filePath))
            {
                string[] list = System.IO.File.ReadAllLines(filePath);
                if (list != null)
                {
                    foreach (string listee in list)
                    {
                        if (listee != null && listee.Trim().ToLower().Length > 0)
                        {
                            dataList.Add(listee.Trim().ToLower());
                        }
                    }
                }
            }
        }

        public bool Save()
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                } catch {
                    return false;
                }
            }

            System.IO.File.WriteAllLines(filePath, dataList.ToArray(typeof(string)) as string[]);

            return true;
        }

        public ArrayList getArrayList()
        {
            return dataList;
        }

        public void setArrayList(ArrayList WhiteList)
        {
            dataList = WhiteList;
        }


    }
}
