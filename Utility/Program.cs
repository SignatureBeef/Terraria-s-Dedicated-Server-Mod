using System;
using Terraria_Server;
using Terraria_Server.Collections;
using Terraria_Utilities.Serialize;

namespace Terraria_Utilities
{
    /**
     * I needed a place to keep my utility functions that I was using to deconstruct the server. I am placing them within
     * this project so they can be documented, should the need to reuse them ever occur. Please be aware that much of the
     * documentation for included functionality will likely be poor as these functions may survive a single commit before
     * needing to be removed as much of this will lead to dramatic refactorings in the code base.
     */
    class Program
    {
        private const string WELCOME_MESSAGE = "This application exists to provide utility functionality for breaking "
            + "down the\nTerraria Server. Please be aware that many of the facilities existing within\nthis application "
            + "will quickly become outdated and are not intended for long\nterm usage.\n";

        static void Main(string[] args)
        {
            Console.WriteLine(WELCOME_MESSAGE);

            var location ="E:\\TerrariaServer.exe";   

            Console.Write("Updating Assemblies...");

            /* Update Reference */
            foreach (var set in new string[] { "Item", "NPC", "Projectile" })
            {
                Serializer.UpdateAssembly(location, "Terraria." + set);
            }

            Console.Write("Ok\nSerializing...");

            var typeSet = GetSet();

            /* Serialize */
            foreach (var set in typeSet)
                Serializer.Serialize(set.TypeReference, set.IgnoreFields, set.TypeReference.GetMethod("SetDefaults", set.SetDefaults), set.TypeReference == typeof(Terraria.Projectile));

            Console.ReadLine();
        }

        public static TypeSets[] GetSet()
        {
            return new TypeSets[]
            {
                /* Items */
                new TypeSets()
                {
                    IgnoreFields = new string[]
                    {
                        "Active", "Stack", "UseSound", "Owner", 
                        "NoUseGraphic", "Alpha", "Color", "Accessory", 
                        "Material", "Vanity", "ManaIncrease"
                    },
                    TypeReference = typeof(Terraria.Item),
                    SetDefaults = new Type[] { typeof(Int32), typeof(Boolean) }
                },

                /* NPC */
                new TypeSets()
                {
                    IgnoreFields = new string[]
                    {
                        "Immune", "Ai", "Active", "Direction", "Oldtarget", "Target", "Life", "OldPos"
                    },
                    TypeReference = typeof(Terraria.NPC),
                    SetDefaults = new Type[] { typeof(Int32), typeof(float) }
                },
                
                /* Projectile */
                new TypeSets()
                {
                    IgnoreFields = new string[]
                    {
                        "Ai", "PlayerImmune", "Type", "Active"
                    },
                    TypeReference = typeof(Terraria.Projectile),
                    SetDefaults = new Type[] { typeof(Int32) }
                }
            };
        }

        public struct TypeSets
        {
            public String[] IgnoreFields    { get; set; }
            public Type     TypeReference   { get; set; }
            public Type[]   SetDefaults     { get; set; }
        }
    }
}
