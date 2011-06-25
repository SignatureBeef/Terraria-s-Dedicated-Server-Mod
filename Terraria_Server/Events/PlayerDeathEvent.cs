using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerDeathEvent : Event
    {
        string deathMessage = "";

        public string getDeathMessage()
        {
            return deathMessage;
        }

        public void setDeathMessage(string DeathMessage)
        {
            deathMessage = DeathMessage;
        }

        public Player getPlayer()
        {
            return (Player)base.getSender();
        }
    }
}
