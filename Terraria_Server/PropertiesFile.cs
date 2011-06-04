using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class PropertiesFile
    {
        string pPath = "";
        public string[] data = null;
        private bool createIfNeeded = false;
        
        public void setFile(string propertiesPath, bool createIN = true)
        {
            createIfNeeded = createIN;
            pPath = propertiesPath;
        }

        public void Load() {
            if (createIfNeeded)
            {
                //System.IO.FileInfo fI = new System.IO.FileInfo(pPath);
                if (!System.IO.File.Exists(pPath))
                {
                    //fI.Directory.Create();
                    System.IO.File.WriteAllText(pPath, "");
                }
                System.Threading.Thread.Sleep(1000);
            }
            data = System.IO.File.ReadAllLines(pPath);
        }

        public void Save()
        {
            int lCount = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Trim().Length > 0)
                {
                    lCount++;
                }
            }
            string[] Data = new string[lCount];
            lCount = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Trim().Length > 0)
                {
                    Data[lCount] = data[i];
                    lCount++;
                }
            }
            System.IO.File.WriteAllLines(pPath, Data);
        }

        public string getValue(string Key)
        {
            for (int i = 0; i < data.Length; i++)
            {
                string line = data[i].Trim();
                if (line.ToLower().StartsWith(Key.ToLower().Trim() + "="))
                {
                    return line.Remove(0, Key.Trim().Length + 1);
                }
            }
            return null;
        }

        public bool setValue(string Key, string Value)
        {
            for (int i = 0; i < data.Length; i++)
            {
                string line = data[i].Trim();
                if (line.ToLower().StartsWith(Key.ToLower().Trim() + "="))
                {
                    data[i] = Key.Trim().ToLower() + "=" + Value.Trim();
                    return true;
                }
            }
            addValue(Key, Value);
            return false;
        }

        public void addValue(string Key, string Value)
        {
            string[] Data = new string[data.Length + 1];
            for (int i = 0; i < data.Length; i++)
            {
                Data[i] = data[i];
            }
            Data[data.Length] = Key.Trim().ToLower() + "=" + Value.Trim();
            data = Data;
        }

        public bool deleteKey(string Key)
        {
            if (containsKey(Key))
            {
                string[] Data = new string[data.Length-1];
                int count = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    string line = data[i].Trim();
                    if (!line.ToLower().StartsWith(Key.ToLower().Trim() + "="))
                    {
                        Data[i] = data[i];
                        count++;
                    }
                }
                data = Data;
            }
            return false;
        }

        public bool containsKey(string Key)
        {
            for (int i = 0; i < data.Length; i++)
            {
                string line = data[i].Trim();
                if (line.ToLower().StartsWith(Key.ToLower().Trim() + "="))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
