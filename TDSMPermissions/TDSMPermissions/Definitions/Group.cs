using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Misc;

namespace TDSMPermissions.Definitions
{
	class Group
	{
		public struct _GroupInfo
		{
			public bool Default;
			public String Prefix;
			public String Suffix;
			public Color color;
		}

		public List<String> Inherits;
		public _GroupInfo GroupInfo;
		public String Name;
		public Dictionary<String, bool> permissions;

		public Group(String name)
		{
			Name = name;
			permissions = new Dictionary<String, bool>();
		}

		public void SetGroupInfo(bool Default, String prefix, String suffix, Color color)
		{
			GroupInfo.Default = Default;
			GroupInfo.Prefix = prefix;
			GroupInfo.Suffix = suffix;
			GroupInfo.color = color;
		}
	}
}
