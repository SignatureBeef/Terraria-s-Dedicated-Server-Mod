using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Misc;

namespace TDSMPermissions.Definitions
{
    public class Group
    {
        public struct _GroupInfo
        {
            public bool Default;
            public bool CanBuild;
            public string Prefix;
            public string Suffix;
            public string Seperator;
            public Color color;
			public int rank;
        }

        public List<String> Inherits;
        public _GroupInfo GroupInfo;
        public string Name;
        public Dictionary<String, Boolean> permissions;

        public Group(string name)
        {
            Name = name;
            Inherits = new List<String>();
            permissions = new Dictionary<String, Boolean>();
            GroupInfo.Prefix = null;
            GroupInfo.Suffix = null;
            GroupInfo.color = ChatColor.Aqua;
            GroupInfo.Default = false;
            GroupInfo.Seperator = " : ";
        }

        public void SetGroupInfo(bool Default, bool canBuild, string prefix, string suffix, string seperator, Color color, int rank)
        {
            GroupInfo.Default = Default;
            GroupInfo.Prefix = prefix;
            GroupInfo.Suffix = suffix;
            GroupInfo.Seperator = seperator;
            GroupInfo.color = color;
            GroupInfo.CanBuild = canBuild;
			GroupInfo.rank = rank;
        }
    }
}