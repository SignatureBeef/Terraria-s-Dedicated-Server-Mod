using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugin;

namespace Terraria_Server.Events
{
    public class PartyChangeEvent : Event
    {
        private Party partyType = Party.NONE;

        public Player getPlayer()
        {
            return (Player)base.getSender();
        }

        public Party getPartyType()
        {
            return partyType;
        }

        public void setPartyType(Party Type)
        {
            partyType = Type;
        }

    }
}
