using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Permissions
{
	public class PermissionManager
	{
		string[] TDSMnodes = {
							"tdsm.exit",	"tdsm.stop", "tdsm.saveall", "tdsm.reload", "tdsm.list", "tdsm.who", "tdsm.players",
							"tdsm.playing", "tdsm.online", "tdsm.me", "tdsm.say", "tdsm.slots", "tdsm.kick", "tdsm.ban",
							"tdsm.unban", "tdsm.whitelist", "tdsm.rcon", "tdsm.status", "tdsm.time", "tdsm.help", "tdsm.give",
							"tdsm.spawnnpc", "tdsm.tp", "tdsm.tphere", "tdsm.settle", "tdsm.op", "tdsm.deop", "tdsm.oplogin",
							"tdsm.oplogout", "tdsm.npcspanws", "tdsm.restart", "tdsm.purge", "tdsm.plugins", "tdsm.plugin",
							"tdsm.spawnboss", "tdsm.itemrej", "tdsm.explostions", "tdsm.maxplayers", "tdsm.q", "tdsm.refresh"
		};

		// these are stable APIs
		public bool IsPermitted(string node, Player player)
		{
			return isPermittedImpl(node, player);
		}

		// this is only for the permissions plugin to set
		public Func<String, Player, Boolean> isPermittedImpl;

		public void AddNodes(string[] nodes)
		{
			foreach (string node in nodes)
				ActiveNodes.Add(node);
		}

		public void RemoveNode(string[] nodes)
		{
			foreach (string node in nodes)
				if (ActiveNodes.Contains(node))
					ActiveNodes.Remove(node);
		}

		public List<String> ActiveNodes;

		public PermissionManager()
		{
			ActiveNodes = new List<String>();
			AddNodes(TDSMnodes);
		}
	}
}
