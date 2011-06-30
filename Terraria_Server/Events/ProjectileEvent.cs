using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class ProjectileEvent : Event
    {
        private Projectile projectile;

        public Projectile getProjectile()
        {
            return projectile;
        }

        public void setProjectile(Projectile Projectile)
        {
            projectile = Projectile;
        }
    }
}
