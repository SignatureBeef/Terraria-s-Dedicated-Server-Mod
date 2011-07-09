using System;
using System.Collections.Generic;
using System.IO;
using Terraria_Server;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Collections;

namespace Terraria_Utilities.Serialize
{
    public class SerializeNPC
    {
        public static void Start()
        {
            /*
            String[] ignoreFields = new String[] { "immune", "ai", "Active", "direction", "oldtarget", "target", "life" };
            DiffSerializer serializer = new DiffSerializer(typeof(NPC), ignoreFields);
            FileStream fs = new FileStream("NPCs.xml", FileMode.OpenOrCreate);
            XmlDictionaryWriter.Create(fs);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
            writer.WriteStartElement("ArrayOf" + typeof(NPC).Name);
            for (int i = 0; i < 70; i++)
            {
                NPC npc = new NPC();
                npc.SetDefaults(i);
                if (!String.IsNullOrWhiteSpace(npc.Name))
                {
                    serializer.WriteObject(writer, npc);
                }
            }
            writer.WriteEndElement();
            writer.Close();
            fs.Close();
             */
        }
    }
}
