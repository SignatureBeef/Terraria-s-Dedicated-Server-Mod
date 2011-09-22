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

		public static bool IsPermitted(string node, Player player)
		{
			return isPermittedImpl(node, player);
		}

		// this is only for the permissions plugin to set
		public static Func<string, Player, bool> isPermittedImpl;

        public static void AddNodes(string[] nodes)
		{
			foreach (string node in nodes)
				ActiveNodes.Add(node);
		}

		public static void RemoveNode(string[] nodes)
		{
			foreach (string node in nodes)
				if (ActiveNodes.Contains(node))
					ActiveNodes.Remove(node);
		}

        public static List<String> ActiveNodes = new List<String>();
	}
}
