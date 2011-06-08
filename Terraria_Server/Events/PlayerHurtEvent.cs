using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerHurtEvent : Event
    {

        int damage;

        public Player getPlayer()
        {
            return (Player)getSender();
        }

        public int getDamage() 
        {
            return damage;
        }

        public void setDamage(int Damage)
        {
            damage = Damage;
        }

    }
}
