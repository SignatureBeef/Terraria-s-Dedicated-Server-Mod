using System;
using System.IO;
using System.Xml;
using Terraria_Server.Collections;
using System.Reflection;

namespace Terraria_Utilities.Serialize
{
    public class Serializer
    {
        public static void Serialize(Type type, String[] ignoreFields)
        {
            DiffSerializer serializer = new DiffSerializer(type, ignoreFields);
            FileStream fs = new FileStream(type.Name + "s.xml", FileMode.OpenOrCreate);
            XmlDictionaryWriter.Create(fs);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
            writer.WriteStartElement("ArrayOf" + type.Name);
            MethodInfo setDefaults = type.GetMethod("SetDefaults", new Type[]{typeof(int), typeof(bool)});
            PropertyInfo name = type.GetProperty("Name");
            int count = 0;
            for (int i = 0; i < 1000; i++)
            {
                object obj = Activator.CreateInstance(type);
                setDefaults.Invoke(obj, new object[]{i, false});
                String value = (String)name.GetValue(obj, null);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    serializer.WriteObject(writer, obj);
                    count++;
                }
            }
            writer.WriteEndElement();
            writer.Close();
            fs.Close();
            Console.WriteLine("Found " + count.ToString() + " items.");
        }
    }
}
