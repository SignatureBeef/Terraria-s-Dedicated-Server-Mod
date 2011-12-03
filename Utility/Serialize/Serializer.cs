using System;
using System.IO;
using System.Xml;
using Terraria_Server.Collections;
using System.Reflection;
using Terraria_Server;
using Terraria_Server.Definitions;
using System.Threading;
using Mono.Cecil;

namespace Terraria_Utilities.Serialize
{
    public class Serializer
    {
        public static void Serialize(Type type, string[] ignoreFields, MethodInfo SetDefaults, bool Projectile = false)
        {
            DiffSerializer serializer = new DiffSerializer(type, ignoreFields);
            FileStream fs = new FileStream(type.Name + "s.xml", FileMode.OpenOrCreate);
			XmlWriterSettings ws = new XmlWriterSettings();
			ws.Indent = true;
            XmlDictionaryWriter.Create(fs, ws);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
            writer.WriteStartElement("ArrayOf" + type.Name);
            int count = 0;
            for (int i = 0; i < 1000; i++)
            {
                object obj = Activator.CreateInstance(type);
                try
                {
                    if(!Projectile)
                        SetDefaults.Invoke(obj, new object[] { i, null });
                    else
                        SetDefaults.Invoke(obj, new object[] { i });
                }
                catch // (Exception e) //Usually catches player data which is not set
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
                }
                Thread.Sleep(10);
            }
			writer.WriteString("\n");
            writer.WriteEndElement();
            writer.Close();
            fs.Close();
            Console.WriteLine("Found {0} {1}s.", count, type.Name);
        }

        public static void UpdateAssembly(string ModulePath, string Type)
        {            
            AssemblyDefinition module = AssemblyFactory.GetAssembly(ModulePath);
            TypeDefinition def = module.MainModule.Types[Type];
            string TypeName = def.FullName;

            /* This method renames the Variables to how we have them */
            foreach (FieldDefinition field in def.Fields)
                field.Name = ReplaceFirst(field.Name);

            AssemblyFactory.SaveAssembly(module, ModulePath);
        }

        public static string ReplaceFirst(string val)
        {
            var first = val.Substring(0, 1).ToUpper();
            return first + val.Remove(0, 1);
        }
    }
}
