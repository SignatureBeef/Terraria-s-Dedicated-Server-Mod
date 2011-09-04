using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Permissions;

namespace Regions.Nodes
{
    public class TilePlace : Node
    {
        public const string Node = "regions.tileplace";

        public Node GetNode()
        {
            return FromPath(Node);
        }
    }
}
