using OTA.Extensions;
using System;
using System.Linq;

namespace TDSM.Utility
{
    class Program
    {
        static void Main(string[] args)
        {
            Terraria.Main.dedServ = true;
            Console.WriteLine("TDSM Extractor.\nReferenced Terraria: {0}", Terraria.Main.versionNumber);
            
            var type = typeof(Extractor);
            foreach (var cls in type.Assembly
                .GetTypesLoaded()
                .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract)
            )
            {
                Console.Write("Running extractor: {0}...", cls.Name);
                var ext = (Extractor)Activator.CreateInstance(cls);
                ext.Extract();
                Console.WriteLine();
            }

            Console.WriteLine("Done, press any key to exit.");
            Console.ReadKey();
        }
    }
}
