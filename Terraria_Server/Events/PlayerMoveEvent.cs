using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace Terraria_Server.Events
{
    public class PlayerMoveEvent : BasePlayerEvent
    {
        public Vector2 Location { get; set; }
        public Vector2 Velocity { get; set; }
        public int FallStart { get; set; }
    }
}
