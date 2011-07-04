using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    //Currently not in use
    public class PlayerDestroySignEvent : Event
    {
        private bool isPlayer { get; set; }

        public Sign Sign { get; set; }
    }
}
