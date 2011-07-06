using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class NPCSpawnEvent : Event
    {
        public NPC NPC { get; set; }
    }
}
