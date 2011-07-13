using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerEditSignEvent : CancellableEvent
    {
        public String Text { get; set; }
        public Sign Sign { get; set; }
        public Player Player
        {
            get { return (Player)base.Sender; }
        }
    }
}
