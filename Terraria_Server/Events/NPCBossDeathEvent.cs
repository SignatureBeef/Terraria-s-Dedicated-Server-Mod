using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class NPCBossDeathEvent : Event
    {
        public int Boss { get; set; }
    }
}
