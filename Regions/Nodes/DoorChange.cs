using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Permissions;

namespace Regions.Nodes
{
    public class DoorChange : Node
    {
        public const String Node = "regions.doorchange";

        public Node GetNode()
        {
            return FromPath(Node);
        }
    }
}
