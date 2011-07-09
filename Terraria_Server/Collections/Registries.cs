using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Terraria_Server.Collections
{
    public class Registries
    {
        private const String NPC_FILE = "NPCs.xml";

        public static Registry<NPC> NPC = new Registry<NPC>(NPC_FILE, new NPC());
        public static ItemRegistry Item = new ItemRegistry();
    }
}
