using System.IO;
using System.Threading;
using System.Collections.Generic;
using System;

namespace Terraria_Server.Misc
{
    public class PropertiesFile
    {
        private const char EQUALS = '=';

        private Dictionary<String, String> propertiesMap;
        
        private string propertiesPath = String.Empty;

        public PropertiesFile(String propertiesPath)
        {
            propertiesMap = new Dictionary<string, string>();
            this.propertiesPath = propertiesPath;
        }

        public void Load() {
            //Verify that the properties file exists and we can create it if it doesn't.
            if (!File.Exists(propertiesPath))
            {
                File.WriteAllText(propertiesPath, String.Empty);
            }

            propertiesMap.Clear();
            StreamReader reader = new StreamReader(propertiesPath);
            try
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    int setterIndex = line.IndexOf(EQUALS);
                    if (setterIndex > 0 && setterIndex < line.Length)
                    {
                        propertiesMap.Add(line.Substring(0, setterIndex), line.Substring(setterIndex + 1));
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }

        public void Save()
        {
            StreamWriter writer = new StreamWriter(propertiesPath);
            try
            {
                foreach (KeyValuePair<String, String> pair in propertiesMap)
                {
                    writer.WriteLine(pair.Key + EQUALS + pair.Value);
                }
            }
            finally
            {
                writer.Close();
            }
        }

        public string getValue(string key)
        {
            if (propertiesMap.ContainsKey(key))
            {
                return propertiesMap[key];
            }
            return null;
        }

        public string getValue(string key, string defaultValue)
        {
            String value = getValue(key);
            if (value == null || value.Trim().Length < 0)
            {
                setValue(key, defaultValue);
                return defaultValue;
            }
            return value;
        }

        public int getValue(string key, int defaultValue)
        {
            int result;
            if (int.TryParse(getValue(key), out result))
            {
                return result;
            }

            setValue(key, defaultValue);
            return defaultValue;
        }

        public bool getValue(string key, bool defaultValue)
        {
            bool result;
            if (bool.TryParse(getValue(key), out result))
            {
                return result;
            }

            setValue(key, defaultValue);
            return defaultValue;
        }

        private void setValue(string key, string value)
        {
            propertiesMap[key] = value;
        }

        protected void setValue(string key, int value)
        {
            setValue(key, value.ToString());
        }

        private void setValue(string key, bool value)
        {
            setValue(key, value.ToString());
        }
    }
}
