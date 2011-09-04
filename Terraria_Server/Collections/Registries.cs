using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Terraria_Server.Collections
{
    public class Registries
    {
        public const string NPC_FILE = "NPCs.xml";
        public const string PROJECTILE_FILE = "Projectiles.xml";

        public static ItemRegistry Item = new ItemRegistry();
        public static Registry<NPC> NPC = new Registry<NPC> ();
        public static Registry<Projectile> Projectile = new Registry<Projectile> ();
    }
}
