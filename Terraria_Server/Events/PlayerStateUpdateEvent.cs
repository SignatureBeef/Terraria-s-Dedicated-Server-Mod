using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerStateUpdateEvent : Event
    {
        byte state;

        public Player getPlayer()
        {
            return (Player)getSender();
        }

        public byte getState()
        {
            return state;
        }

        public void setState(byte State)
        {
            state = State;
        }

    }
}
