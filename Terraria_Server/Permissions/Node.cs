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

        public static Node FromPath(string node)
		{
			return new Node(node);
		}

		public Node(string node)
		{
			stringNode = node;
		}
	}
}
