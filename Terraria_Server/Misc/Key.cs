using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Misc
{
    /// <summary>
    /// Key class for determining player's keypresses.  Currently only up, down, left, right and jump are detectable
    /// </summary>
    public struct Key
    {
        public bool Up		{ get; set; }
        public bool Down	{ get; set; }
        public bool Left	{ get; set; }
        public bool Right	{ get; set; }
        public bool Jump	{ get; set; }
    }
}
