using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerDestroySignEvent : Event
    {
        //Currently not in use
        private Sign sign;
        private bool player = true;

        public Sign getSign()
        {
            return sign;
        }

        public void setSign(Sign Sign)
        {
            sign = Sign;
        }

        public bool isPlayer()
        {
            return player;
        }

        public void setIsPlayer(bool Player)
        {
            player = Player;
        }
    }
}
