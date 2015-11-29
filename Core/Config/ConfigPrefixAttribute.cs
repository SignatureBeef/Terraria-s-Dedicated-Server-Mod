using System;

namespace TDSM.Core.Config
{
    public class ConfigPrefixAttribute : Attribute
    {
        public string Prefix { get; set; }

        public ConfigPrefixAttribute(string prefix)
        {
            this.Prefix = prefix;
        }
    }
}

