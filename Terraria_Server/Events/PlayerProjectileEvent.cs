using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerProjectileEvent : Event
    {
        public Projectile Projectile { get; set; }
    }
}
