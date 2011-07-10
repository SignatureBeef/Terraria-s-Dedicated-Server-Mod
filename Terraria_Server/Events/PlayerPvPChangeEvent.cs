using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerPvPChangeEvent : Event
    {
        public Player Player() {
            return (Player)Sender;
        }

        public bool PvPState()
        {
            return ((Player)Sender).hostile;
        }
    }
}
