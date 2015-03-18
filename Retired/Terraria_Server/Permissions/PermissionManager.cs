using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Permissions
{
	/// <summary>
	/// Permission manager 
	/// </summary>
	public class PermissionManager
	{
		string[] TDSMnodes = {
                            "tdsm.admin", "tdsm.plugin", "tdsm.who",
                            "tdsm.me", "tdsm.say", "tdsm.slots", "tdsm.kick", "tdsm.ban",
                            "tdsm.unban", "tdsm.whitelist", "tdsm.rcon", "tdsm.status", "tdsm.time", "tdsm.help", "tdsm.give",
                            "tdsm.spawnnpc", "tdsm.tp", "tdsm.tphere", "tdsm.settle", "tdsm.op", "tdsm.deop", "tdsm.oplogin",
                            "tdsm.oplogout", "tdsm.npcspanws", "tdsm.restart", "tdsm.purge", "tdsm.plugins",
                            "tdsm.spawnboss", "tdsm.itemrej", "tdsm.explostions", "tdsm.maxplayers", "tdsm.q", "tdsm.refresh"
        };

        // these are stable APIs
        /// <summary>
        /// Permission check function.  Use this to check player permissions.
        /// </summary>
        /// <param name="node">Permission node to check</param>
        /// <param name="player">Player to check for permission node</param>
        /// <returns>True if player is permitted.  False if not.</returns>
        public bool IsPermitted(string node, Player player)
        {
            return IsPermittedImpl(node, player);
        }

        /// <summary>
        /// Permission check function.  Use this to check player permissions.
        /// </summary>
        /// <param name="node">Permission node to check</param>
        /// <param name="player">Player to check for permission node</param>
        /// <returns>True if player is permitted.  False if not.</returns>
        public bool IsPermitted(Node node, Player player)
        {
            return IsPermittedImpl(node.Path, player);
        }
		
		// this is only for the permissions plugin to set
		/// <summary>
		/// Permission check delegate.  Set only by permissions plugin.  Do no use this to check.
		/// </summary>
		public Func<String, Player, Boolean> IsPermittedImpl;

		/// <summary>
		/// Add permission nodes to the server's list of active nodes.  Global permission purposes.
		/// </summary>
		/// <param name="nodes">String array containing nodes to add.</param>
		public void AddNodes(string[] nodes)
		{
			foreach (string node in nodes)
				ActiveNodes.Add(node);
		}

		/// <summary>
		/// Remove nodes from the server's list of active nodes.  Global permission purposes, may not be necessary.
		/// </summary>
		/// <param name="nodes">String array of nodes to remove.</param>
		public void RemoveNode(string[] nodes)
		{
			foreach (string node in nodes)
				if (ActiveNodes.Contains(node))
					ActiveNodes.Remove(node);
		}

		/// <summary>
		/// List of active permission nodes.  Global permission purposes.
		/// </summary>
		public List<String> ActiveNodes;

		/// <summary>
		/// Permission manager constructor.  //Also adds internal TDSM nodes to ActiveNodes.
		/// </summary>
		public PermissionManager()
		{
			ActiveNodes = new List<String>();
            AddNodes(TDSMnodes); //Not sure if this should be needed as the TDSM commands set their permission nodes themselves.
		}
	}
}
