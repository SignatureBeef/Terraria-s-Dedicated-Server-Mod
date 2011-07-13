using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class NPCSpawnEvent : CancellableEvent
    {
        public NPC NPC { get; set; }
    }
}
