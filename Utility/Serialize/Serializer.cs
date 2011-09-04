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
        public static readonly string[] ITEM_IGNORE_FIELDS = new string[] { "Active", "Stack", "UseSound", "Owner", "NoUseGraphic", "Alpha", "Color", "Accessory", "Material", "Vanity", "ManaIncrease" };
        public static readonly MethodInfo ITEM_SET_DEFAULTS = typeof(Item).GetMethod("SetDefaults", new Type[] { typeof(int), typeof(bool) });

        public static readonly string[] NPC_IGNORE_FIELDS = new string[] { "immune", "ai", "Active", "direction", "oldtarget", "target", "life" };

        public static readonly string[] PROJECTILE_IGNORE_FIELDS = new string[] { "ai", "playerImmune", "type", "Active" };
        public static readonly MethodInfo PROJECTILE_SET_DEFAULTS = typeof(Projectile).GetMethod("SetDefaults", new Type[] { typeof(ProjectileType) });

        public static void Serialize(Type type, string[] ignoreFields)
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
                ITEM_SET_DEFAULTS.Invoke(obj, new object[] { i, false });
                string value = (String)name.GetValue(obj, null);
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
