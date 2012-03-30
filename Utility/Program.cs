using System;
using Terraria_Utilities.Serialize;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

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

        static Type Item        = typeof(Terraria.Item);
        static Type NPC         = typeof(Terraria.NPC);
        static Type Projectile  = typeof(Terraria.Projectile);

        static void Main(string[] args)
        {
            Console.WriteLine(WELCOME_MESSAGE);

#region tests
			//var err = 0;
			//var res = ConsistencyCheck.CheckTileSets(out err);
			//if (!res)
			//{
			//    Console.WriteLine("TDSM tile set incorrect");
			//}
			//else { return; }

			// [START] Test NPC Serializer
			/*NPCSerializer.Serialize();
			Console.ReadKey(true);
			return;*/
			//[END] Test NPC Serializer */
#endregion

			//var location = "C:\\TerrariaServer.exe";
            var typeSet = GetSet();
            var upperCase = GetUppercaseFields();

            Console.Write("Updating Assemblies...");

            /* Update Reference */
            /*var i = 0;
            foreach (var name_space in new string[] { Item.FullName, NPC.FullName, Projectile.FullName })
            {
                /* Add extra fields from ignored lists incase we forgot some * /
                foreach (var field in typeSet[i++].IgnoreFields)
                {
                    char letter = field.ToCharArray()[0];
                    if (Char.IsUpper(letter))
                    {
                        /* Add both; Lower & Upper - Lazy IDC * /
                        upperCase[name_space].Add(field);
                        upperCase[name_space].Add(Serializer.ReplaceFirst(field, false));
                    }
                }
                Serializer.UpdateAssembly(location, name_space, upperCase[name_space]);
            }*/

			Console.Write("Ok\nDumping Affix's...");
			DumpAffixes();

            Console.Write("Ok\nSerializing...");
            Terraria.Main.dedServ = true; //Set this to true, We don't need the GUI shit.

            /* Serialize */
            foreach (var set in typeSet)
            {
                var data = Serializer.Serialize(   
                                        set.TypeReference, set.IgnoreFields, 
                                        set.TypeReference.GetMethod("SetDefaults", set.SetDefaults), set.InvokeType, 
                                        set.EntityObjNames, (set.EntityObjNames != null) ? set.EntityObjNames.Length : 1000
                );

                var ClassName = set.TypeReference.Name + "Type" + set.InvokeType.ToString();
                var writer = new StreamWriter(ClassName + ".cs");

                Console.Write("Saving {0}...", ClassName);
                var l = String.Format("public enum {0} : int", ClassName);
                writer.WriteLine(l);
                writer.WriteLine("{");
                foreach (var pair in data)
                {
                    var val = pair.Key;
                    var name = pair.Value.ToUpper().Replace(" ", "_").Replace("'", "");
					var line = String.Format("\tN{0}_{1} = {2},", pair.Key, name, pair.Key);
                    var attribute = String.Format("\t[XmlEnum(Name = \"{0}\")]", val);

                    writer.WriteLine(attribute);
                    writer.WriteLine(line);
                }
                writer.WriteLine("}");

                writer.Flush();
                writer.Close();
                writer.Dispose();

                Console.WriteLine("Ok");
            }

            Console.ReadLine();
        }

        public static TypeSets[] GetSet()
        {
            return new TypeSets[]
            {
                /* Items - Int32 */
                new TypeSets()
                {
                    IgnoreFields = new string[]
                    {
                        "Active", "Stack", "UseSound", "Owner", 
                        "NoUseGraphic", "Alpha", "Color", "Accessory", 
                        "Material", "Vanity", "ManaIncrease"
                    },
                    TypeReference = typeof(Terraria.Item),
                    SetDefaults = new Type[] { typeof(Int32), typeof(Boolean) },
                    InvokeType = InvokeType.ITEM_NPC
                },

                /* NPC - Int32 */
                new TypeSets()
                {
                    IgnoreFields = new string[]
                    {
                        "Immune", "Ai", "Active", "Direction", 
                        "Oldtarget", "Target", "Life", "OldPos",
                        "buffType", "buffTime", "buffImmune", "color",
                        "localAI", "position"
                    },
                    TypeReference = typeof(Terraria.NPC),
                    SetDefaults = new Type[] { typeof(Int32), typeof(float) },
                    InvokeType = InvokeType.ITEM_NPC
                },
                
                /* Projectile */
                new TypeSets()
                {
                    IgnoreFields = new string[]
                    {
                        "Ai", "PlayerImmune", "Active"
                    },
                    TypeReference = typeof(Terraria.Projectile),
                    SetDefaults = new Type[] { typeof(Int32) },
                    InvokeType = InvokeType.PROJECTILE
                },

                /* NPC - String */
                new TypeSets()
                {
                    IgnoreFields = new string[]
                    {
                        "Immune", "Ai", "Active", "Direction", 
                        "Oldtarget", "Target", "Life", "OldPos",
                        "buffType", "buffTime", "buffImmune", "color",
                        "localAI", "position"
                    },
                    TypeReference = typeof(Terraria.NPC),
                    SetDefaults = new Type[] { typeof(String) },
                    EntityObjNames = new string[]
                    {
                        "Green Slime", "Pinky", "Baby Slime", "Black Slime",
                        "Purple Slime", "Red Slime", "Yellow Slime", "Jungle Slime",
                        "Little Eater", "Big Eater", "Short Bones", "Big Boned",
                        "Little Stinger", "Big Stinger", "Slimeling", "Slimer2",
                        "Heavy Skeleton"
                    },
                    InvokeType = InvokeType.ITEM_NPC_BY_NAME
                },

                /* Items - String */
                new TypeSets()
                {
                    IgnoreFields = new string[]
                    {
                        "Active", "Stack", "UseSound", "Owner", 
                        "NoUseGraphic", "Alpha", "Color", "Accessory", 
                        "Material", "Vanity", "ManaIncrease"
                    },
                    TypeReference = typeof(Terraria.Item),
                    SetDefaults = new Type[] { typeof(String) },
                    EntityObjNames = new string[]
                    {
                        "Gold Pickaxe", "Gold Broadsword", "Gold Shortsword", "Gold Axe",
                        "Gold Hammer", "Gold Bow", "Silver Pickaxe", "Silver Broadsword",
                        "Silver Shortsword", "Silver Axe", "Silver Hammer", "Silver Bow",
                        "Copper Pickaxe", "Copper Broadsword", "Copper Shortsword", "Copper Axe",
                        "Copper Hammer", "Copper Bow", "Blue Phasesaber", "Red Phasesaber",
                        "Green Phasesaber", "Purple Phasesaber", "White Phasesaber", "Yellow Phasesaber"
                    },
                    InvokeType = InvokeType.ITEM_NPC_BY_NAME
                }
            };
        }

        public static Dictionary<String, List<String>> GetUppercaseFields()
        {
            return new Dictionary<String, List<String>>
            {
                {
                    Item.FullName, new List<String>() { "*" }
                },
                {   NPC.FullName, 
                    new List<String>()
                    {
                        "type", "name", "width", "height", 
                        "inherits", "poisonImmunity", "burningImmunity", 
                        "scaleDamage", "scaleDefense", "scaleLifeMax", "scaleValue", 
                        "scaleKnockBackResist", "scaleSlots", "netID"
                    }
                },
                {   Projectile.FullName, 
                    new List<String>()
                    {
                        "type", "name", "width", "height"
                    }
                }
            };
        }

        public struct TypeSets
        {
            public String[]     IgnoreFields    { get; set; }
            public Type         TypeReference   { get; set; }
            public Type[]       SetDefaults     { get; set; }
            public String[]     EntityObjNames  { get; set; }
            public InvokeType   InvokeType      { get; set; }
        }

		public static void DumpAffixes()
		{
			var ClassName = "Affix";
			var writer = new StreamWriter(ClassName + ".cs");

			Console.Write("Saving {0}...", ClassName);
			var l = String.Format("public enum {0} : int", ClassName);
			writer.WriteLine(l);
			writer.WriteLine("{");

			var item = new Terraria.Item();
			for (byte i = 1; i < Terraria_Server.Item.MAX_AFFIXS + 1; i++)
			{
				item.prefix = i;
				var affix = item.AffixName().Trim();

				var line = "\t{0} = {1}";
				if (i != Terraria_Server.Item.MAX_AFFIXS)
					line += ',';

				var toWrite = String.Format(line, affix, i);
				writer.WriteLine(toWrite);
			}

			writer.WriteLine("}");

			writer.Flush();
			writer.Close();
			writer.Dispose();

			Console.WriteLine("Ok");
		}
    }
}
