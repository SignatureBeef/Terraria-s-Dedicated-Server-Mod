using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Collections
{
    public class Registries
    {
        public static Registry<NPC> NPC = new Registry<NPC>("NPCs.xml", new NPC());
        public static ItemRegistry Item = new ItemRegistry();
    }
}
