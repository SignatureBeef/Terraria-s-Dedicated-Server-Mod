using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Collections
{
    public class NPCRegistry : Registry<NPC>
    {
        private NPCRegistry() : base(Statics.NPCXmlPath, new NPC()) { }

        private static NPCRegistry instance = new NPCRegistry();

        public static NPC Create(int type)
        {
            return CloneAndInit(instance[type]);
        }

        public static NPC Create(String type)
        {
            return CloneAndInit(instance[type]);
        }

        private static NPC CloneAndInit(NPC npc)
        {
            NPC cloned = (NPC) npc.Clone();
            NPC.npcSlots = cloned.slots;
            
            cloned.frame = default(Rectangle);

            cloned.width = (int)((float)cloned.width * cloned.scale);
            cloned.height = (int)((float)cloned.height * cloned.scale);
            cloned.life = cloned.lifeMax;
            if (cloned.Type != instance.Default.Type)
            {
                cloned.Active = true;
            }
            return cloned;
        }
    }
}
