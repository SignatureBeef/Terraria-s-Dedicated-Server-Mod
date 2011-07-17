using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace Terraria_Server.Events
{
    public class PlayerChestBreakEvent : BasePlayerEvent
    {
        public Vector2 Location { get; set; }
    }
}
