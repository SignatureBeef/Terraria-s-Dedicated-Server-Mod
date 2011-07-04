using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class NPCDeathEvent : Event
    {
        public NPC Npc { get; set; }
        public int Damage { get; set; }
        public float KnockBack { get; set; }
        public int HitDirection { get; set; }
    }
}
