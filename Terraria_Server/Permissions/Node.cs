using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Permissions
{
	public class Node
	{
		private String stringNode;

		// these are stable APIs
		public string Path { get { return stringNode; } }

		public Node FromPath(String node)
		{
			return new Node { stringNode = node };
		}

		public bool IsPermitted(Player player)
		{
			return isPermittedImpl(this, player);
		}

		// this is only for the permissions plugin to set
		public static Func<Node, Player, bool> isPermittedImpl;
	}
}
