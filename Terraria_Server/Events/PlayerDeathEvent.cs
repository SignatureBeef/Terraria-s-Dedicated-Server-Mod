using System;

namespace Terraria_Server.Events
{
    public class PlayerDeathEvent : BasePlayerEvent
    {
        public String DeathMessage { get; set; }
    }
}
