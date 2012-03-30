using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Threading;
using Mono.Cecil;
using System.Collections.Generic;

namespace Terraria_Utilities.Serialize
{
    public enum InvokeType
    {
        ITEM_NPC,
        PROJECTILE,
        ITEM_NPC_BY_NAME
    }

    public class Serializer
    {        
        /// <summary>
        /// Supports Serialization of NPC, Items & Projectiles.
        /// It also supports multiple formats by specifing the Method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ignoreFields"></param>
        /// <param name="SetDefaults"></param>
        /// <param name="invokeType"></param>
        /// <param name="input"></param>
        public static Dictionary<Int32, String> Serialize(
			Type type, string[] ignoreFields, MethodInfo SetDefaults, 
			InvokeType invokeType = InvokeType.ITEM_NPC, 
			string[] inputs = null, int MaxObjects = 1000,
			bool NPCOverride = false)
        {
            var FilePath = (invokeType == InvokeType.ITEM_NPC_BY_NAME) ? type.Name + "sByName.xml" : type.Name + "s.xml";
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            DiffSerializer serializer = new DiffSerializer(type, ignoreFields);
            FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
			XmlWriterSettings ws = new XmlWriterSettings();
			ws.Indent = true;
            XmlDictionaryWriter.Create(fs, ws);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
            writer.WriteStartElement("ArrayOf" + type.Name);

            var returnData = new Dictionary<Int32, String>();
            int count = 0;
            for (int i = 0; i < MaxObjects; i++)
            {
                object obj = Activator.CreateInstance(type);

                try
                {
                    if(invokeType == InvokeType.ITEM_NPC)
                        SetDefaults.Invoke(obj, new object[] { i, null });
                    else if (invokeType == InvokeType.ITEM_NPC_BY_NAME)
                        SetDefaults.Invoke(obj, new object[] { inputs[i] });
                    else
                        SetDefaults.Invoke(obj, new object[] { i });
                }
                catch //(Exception e) //Usually catches player data which is not set
                {
                    //Console.WriteLine("[Error] {0}", e.Message);
                }

                FieldInfo info = type.GetField("Name");

                string value = String.Empty;
                try
                {
                    value = (String)info.GetValue(obj);
                }
                catch (NullReferenceException)
                {
                    //Close
                    writer.WriteString("\n");
                    writer.WriteEndElement();
                    writer.Close();
                    fs.Close();

                    Console.WriteLine("Please restart this application, The Assemblies need refreshing.");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }

                if (!String.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine("Processing `{0}`...", value);
                    serializer.WriteObject(writer, obj);
                    count++;

					if (obj is Terraria.NPC)
					{
						var npc = obj as Terraria.NPC;
						if(!returnData.ContainsKey(npc.type))
							returnData.Add(npc.type, value);
					}
					else
						returnData.Add(count, value);
                }
                Thread.Sleep(5);
            }
			writer.WriteString("\n");
            writer.WriteEndElement();
            writer.Close();
            fs.Close();

            Console.WriteLine("Found {0} {1}s.", count, type.Name);

            return returnData;
        }

        public static void UpdateAssembly(string ModulePath, string Type, List<String> caseFeilds)
        {            
            AssemblyDefinition module = AssemblyFactory.GetAssembly(ModulePath);
            TypeDefinition def = module.MainModule.Types[Type];
            string TypeName = def.FullName;

            /* This method renames the Variables to how we have them */
            foreach (FieldDefinition field in def.Fields)
            {
                if (caseFeilds.Contains(field.Name) || caseFeilds.Contains("*"))
                    field.Name = ReplaceFirst(field.Name);
            }

            //module.MainModule.Types["WorldGen"].IsPublic = true;

            AssemblyFactory.SaveAssembly(module, ModulePath);
        }

        public static string ReplaceFirst(string val, bool upper = true)
        {
            var first = String.Empty;

            if(upper)
                first = val.Substring(0, 1).ToUpper();
            else
                first = val.Substring(0, 1).ToLower();

            return first + val.Remove(0, 1);
        }
    }
}
