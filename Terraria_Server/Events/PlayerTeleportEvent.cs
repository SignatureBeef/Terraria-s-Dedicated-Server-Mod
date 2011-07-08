using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace Terraria_Server.Events
{
    public class PlayerTeleportEvent : BasePlayerEvent
    {
        public Vector2 ToLocation { get; set; }
        public Vector2 FromLocation { get; set; }
    }
}
