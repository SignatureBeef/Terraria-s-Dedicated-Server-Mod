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
        private const String PROJECTILE_FILE = "Projectiles.xml";

        public static ItemRegistry Item = new ItemRegistry();
        public static NPCRegistry NPC = new NPCRegistry(NPC_FILE);
        public static Registry<Projectile> Projectile = new Registry<Projectile>(PROJECTILE_FILE);
    }
}
