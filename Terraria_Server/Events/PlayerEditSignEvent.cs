using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerEditSignEvent : PlayerDestroySignEvent
    {
        public bool isPlayer { get; set; }
        public String Text { get; set; }
    }
}
