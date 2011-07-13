using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugin;
using Terraria_Server.Misc;
using Terraria_Server.Definitions;

namespace Terraria_Server.Events
{
    public class DoorStateChangeEvent : CancellableEvent
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Direction { get; set; }

        public bool isOpened { get; set; }

        public DoorOpener Opener { get; set; }
    }
}
