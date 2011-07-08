using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugin;

namespace Terraria_Server.Events
{
    public class PlayerKeyPressEvent : BasePlayerEvent
    {
        public Key KeysPressed { get; set; }
        public bool MouseClicked { get; set;}
        public int FacingDirection { get; set; } // -1 = Left, 1 = Right
    }
}
