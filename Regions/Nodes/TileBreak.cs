using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Permissions;

namespace Regions.Nodes
{
    public class TileBreak : Node
    {
        public const string Node = "regions.tilebreak";

        public Node GetNode()
        {
            return FromPath(Node);
        }
    }
}
