using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Events;

namespace Terraria_Server
{
    public class PlayerCommand : ConsoleCommand
    {
        Player player = null;

        public Player getPlayer()
        {
            return player;
        }

        public void setPlayer(Player Player)
        {
            player = Player;
        }

    }
}
