using System;

namespace Terraria_Server.Events
{
    public class PlayerDeathEvent : BasePlayerEvent
    {
        public string DeathMessage { get; set; }
    }
}
