using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Permissions;

namespace Regions.Nodes
{
    public class ProjectileUse : Node
    {
        public const string Node = "regions.projectileuse";

        public Node GetNode()
        {
            return FromPath(Node);
        }
    }
}
