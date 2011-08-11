using System;
using System.IO;
using System.Xml;
using Terraria_Server.Collections;
using System.Reflection;
using Terraria_Server;
using Terraria_Server.Definitions;

namespace Terraria_Utilities.Serialize
{
    public class Serializer
    {
        public static readonly String[] ITEM_IGNORE_FIELDS = new String[] { "Active", "Stack", "useAnimation", "useSound", "Owner" };
        public static readonly MethodInfo ITEM_SET_DEFAULTS = typeof(Item).GetMethod("SetDefaults", new Type[] { typeof(int), typeof(bool) });

        public static readonly String[] NPC_IGNORE_FIELDS = new String[] { "immune", "ai", "Active", "direction", "oldtarget", "target", "life" };
        
        public static readonly String[] PROJECTILE_IGNORE_FIELDS = new String[] { "ai", "playerImmune", "type", "Active" };
        public static readonly MethodInfo PROJECTILE_SET_DEFAULTS = typeof(Projectile).GetMethod("SetDefaults", new Type[] { typeof(ProjectileType) });

        public static void Serialize(Type type, String[] ignoreFields)
        {
            DiffSerializer serializer = new DiffSerializer(type, ignoreFields);
            FileStream fs = new FileStream(type.Name + "s.xml", FileMode.OpenOrCreate);
			XmlWriterSettings ws = new XmlWriterSettings();
			ws.Indent = true;
            XmlDictionaryWriter.Create(fs, ws);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
            writer.WriteStartElement("ArrayOf" + type.Name);
            PropertyInfo name = type.GetProperty("Name");
            int count = 0;
            for (int i = 0; i < 1000; i++)
            {
                object obj = Activator.CreateInstance(type);
                PROJECTILE_SET_DEFAULTS.Invoke(obj, new object[] { (ProjectileType)i });
                String value = (String)name.GetValue(obj, null);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    serializer.WriteObject(writer, obj);
                    count++;
                }
            }
			writer.WriteString("\n");
            writer.WriteEndElement();
            writer.Close();
            fs.Close();
            Console.WriteLine("Found " + count.ToString() + " items.");
        }
    }
}
