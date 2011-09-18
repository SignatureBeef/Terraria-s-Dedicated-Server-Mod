using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Misc;

namespace TDSMPermissions.Definitions
{
	class User
	{
		public List<string> hasPerm;
		public List<string> notHasPerm;
		public string prefix;
		public string suffix;
		public Color chatColor;
		public List<string> group;

		public User()
		{
			hasPerm = new List<string>();
			notHasPerm = new List<string>();
			group = new List<string>();
			prefix = null;
			suffix = null;
			chatColor = ChatColor.AntiqueWhite;
		}
	}
}
