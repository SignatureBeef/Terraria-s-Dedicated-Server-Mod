using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Permissions;

namespace Regions.Nodes
{
    public class LiquidFlow : Node
    {
        public const String Node = "regions.liquidflow";

        public Node GetNode()
        {
            return FromPath(Node);
        }
    }
}
