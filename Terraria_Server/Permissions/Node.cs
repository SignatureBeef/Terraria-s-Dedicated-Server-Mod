using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Permissions
{
	public class Node
	{
        private string stringNode;

		// these are stable APIs
		public string Path { get { return stringNode; } }

        public Node FromPath(string node)
		{
			return new Node { stringNode = node };
		}

		public bool IsPermitted(Player player)
		{
			return isPermittedImpl(this, player);
		}

		// this is only for the permissions plugin to set
		public static Func<Node, Player, bool> isPermittedImpl;

        public void AddNode(string node)
		{
			ActiveNodes.Add(node);
		}

		public static List<String> ActiveNodes;
	}
}
