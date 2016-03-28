using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace TDSM.Core.Config
{
    public abstract class ComponentConfiguration<T>
    {
        public virtual bool LoadFromFile(string filePath)
        {
            const string Part = "=";
            const string Comment = "#";

            using (var sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    //Ignore comments
                    if (!line.StartsWith(Comment))
                    {
                        //Find the key/values
                        var ix = line.IndexOf(Part);
                        if (ix > -1)
                        {
                            var key = line.Substring(0, ix);
                            var value = line.Remove(0, ix + Part.Length);

                            SetPropertiesByPrefix(key, value);
                        }
                    }
                }
            }
            return false;
        }

        public virtual bool LoadFromArguments()
        {
            var args = Environment.GetCommandLineArgs();
            return LoadFromArguments(args);
        }

        public virtual bool LoadFromArguments(string[] args)
        {
            var line = String.Join(" ", args);
            var al = OTA.Commands.CommandParser.Tokenize(line);

            var t = typeof(ConfigPrefixAttribute);
            var props = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(ConfigPrefixAttribute)));
            foreach (var prop in props)
            {
                var configKey = '-' + ((ConfigPrefixAttribute)Attribute.GetCustomAttribute(prop, t)).Prefix;

                var ix = al.IndexOf(configKey);
                if (ix > -1)
                {
                    if (ix + 1 < al.Count)
                        SetPropertyValue(prop, al[ix + 1]);
                }
            }

            return false;
        }

        public bool SetPropertiesByPrefix(string prefix, string value)
        {
            var t = typeof(ConfigPrefixAttribute);
            var props = typeof(T).GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(ConfigPrefixAttribute)))
                .Select(x => new { Prop = x, Prefix = ((ConfigPrefixAttribute)Attribute.GetCustomAttribute(x, t)).Prefix })
                .Where(x => x.Prefix == prefix);

            var populated = false;
            foreach (var rec in props)
            {
                SetPropertyValue(rec.Prop, value);
                populated = true;
            }
            return populated;
        }

        private void SetPropertyValue(System.Reflection.PropertyInfo property, string value)
        {
            if (property.PropertyType.IsAssignableFrom(typeof(String)))
                property.SetValue(this, value);
            else if (property.PropertyType.IsAssignableFrom(typeof(Int64)))
                property.SetValue(this, Int64.Parse(value));
            else if (property.PropertyType.IsAssignableFrom(typeof(Int32)))
                property.SetValue(this, Int32.Parse(value));
            else if (property.PropertyType.IsAssignableFrom(typeof(Int16)))
                property.SetValue(this, Int16.Parse(value));
            else if (property.PropertyType.IsAssignableFrom(typeof(Boolean)))
                property.SetValue(this, Boolean.Parse(value));
        }
    }
}

