using System;
using System.Collections.Generic;
using System.IO;
using Terraria_Server;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace Terraria_Utilities.Serialize
{
    public class SerializeNPC
    {
        public static void Start()
        {
            //StreamWriter writer = new StreamWriter("NPC.xml");
            String[] ignoreFields = new String[] { "immune", "ai" };
            DiffSerializer serializer = new DiffSerializer(typeof(NPC), ignoreFields);
            FileStream fs = new FileStream("NPC.xml", FileMode.Create);
            //XmlWriterSettings settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, NewLineHandling = NewLineHandling.Entitize };
            XmlDictionaryWriter.Create(fs);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
            writer.WriteStartElement("NPCs");
            //NPCs npcs = new NPCs();
            for (int i = 0; i < 70; i++)
            {
                if (i == 69)
                {
                    Console.WriteLine("hello");
                }
                NPC npc = new NPC();
                npc.SetDefaults(i);
                if (!String.IsNullOrWhiteSpace(npc.Name))
                {
                    serializer.WriteObject(writer, npc);
                    //npcs.Add(npc);
                }
            }
            writer.WriteEndElement();
            writer.Close();
            fs.Close();
            //XmlSerializer serializer = new XmlSerializer(typeof(NPCs));
            //serializer.Serialize(writer, npcs);
            //Console.WriteLine(npcs.Count);
        }
    }
}
