using System;
using System.IO;

namespace TDSM.Data.MySQL
{
    public static class PluginContent
    {
        public static string GetResource(string name)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

