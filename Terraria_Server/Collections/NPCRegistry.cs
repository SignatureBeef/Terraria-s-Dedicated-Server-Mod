using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace Terraria_Server.Collections
{
    public class NPCRegistry : Registry<NPC>
    {
        public NPCRegistry(String FILE) : base(FILE) 
        {
        }

        public NPC Create(String Name, NPC NPC)
        {
            //replace name, colour, life, Active
            NPC newNPCClass = base.Create(Name);

            NPC.Name = Name;
            NPC.life = newNPCClass.life;
            NPC.lifeMax = newNPCClass.lifeMax;
            NPC.defense = newNPCClass.defense;
            NPC.damage = newNPCClass.damage;

            newNPCClass = null;

            return NPC;
        }

        public NPC Create(String Name, Int32 NPCID)
        {
            return Create(Name, Main.npcs[NPCID]);
        }
    }
}
