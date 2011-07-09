using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    /// <summary>
    /// Fired when player uses arrows, knives, tnt, bombs, grenades, guns, boomerangs. Possibly firelash, orb of light, ball/chain, etc.
    /// Be careful when using this to block projectiles.  You could block something you don't want to.
    /// </summary>
    public class PlayerProjectileEvent : Event
    {
        /// <summary>
        /// Projectile instance for the event
        /// </summary>
        public Projectile Projectile { get; set; }
    }
}
