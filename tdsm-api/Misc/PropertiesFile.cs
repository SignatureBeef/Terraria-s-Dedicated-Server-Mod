using System;

namespace tdsm.api.Misc
{
    public class PropertiesFile : DataRegister
    {
        //public PropertiesFile(string path)
        //    : base(path)
        //{

        //}
        public PropertiesFile(string path, bool lowerKeys = true, bool autoLoad = true)
            : base(path, lowerKeys, autoLoad)
        {

        }

        public T GetValue<T>(string key, T defaultValue)
        {
            var item = base.Find(key);
            if (item != null)
            {
                Type t = typeof(T);
                if (t.Name == "Int16")
                {
                    short v = 0;
                    if (Int16.TryParse(item, out v)) return (T)(object)v;
                }
                else if (t.Name == "Int32")
                {
                    int v = 0;
                    if (Int32.TryParse(item, out v)) return (T)(object)v;
                }
                else if (t.Name == "Int64")
                {
                    long v = 0;
                    if (Int64.TryParse(item, out v)) return (T)(object)v;
                }
                else if (t.Name == "Double")
                {
                    double v = 0;
                    if (Double.TryParse(item, out v)) return (T)(object)v;
                }
                else if (t.Name == "Single")
                {
                    float v = 0;
                    if (Single.TryParse(item, out v)) return (T)(object)v;
                }
                else if (t.Name == "Boolean")
                {
                    bool v = false;
                    if (Boolean.TryParse(item, out v)) return (T)(object)v;
                }
                else if (t.Name == "String")
                {
                    return (T)(object)item;
                }
            }
            return defaultValue;
        }

        public bool SetValue(string key, object value, bool autoSave = false)
        {
            return base.Update(key, value.ToString(), autoSave);
        }
    }
}
