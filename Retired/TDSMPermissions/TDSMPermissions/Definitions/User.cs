using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Misc;

namespace TDSMPermissions.Definitions
{
	public class User
	{
		public List<String> hasPerm;
        public List<String> notHasPerm;
        public string prefix;
        public string suffix;
        public string seperator;
        public bool CanBuild;
		public Color chatColor;
        public List<String> group;

		public User()
		{
            hasPerm = new List<String>();
            notHasPerm = new List<String>();
            group = new List<String>();
			prefix = null;
			suffix = null;
			chatColor = ChatColor.AntiqueWhite;
		}

        public void SetUserInfo(string Prefix, string Suffix, string Seperator, bool canBuild, Color Color)
        {
            prefix = Prefix;
            suffix = Suffix;
            chatColor = Color;
            seperator = Seperator;
            CanBuild = canBuild;
        }
	}
}
