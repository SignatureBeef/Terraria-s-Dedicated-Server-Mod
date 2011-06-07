using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Events;

namespace Terraria_Server
{
    public class PlayerCommandEvent : ConsoleCommandEvent
    {
        public Player getPlayer()
        {
            return (Player)base.getSender();
        }

        public void setPlayer(Player Player)
        {
            base.setSender(Player);
        }

    }
}
