using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Terraria_Server.Misc;

namespace TDSM_PermissionsX
{
	public interface IPermission
	{
		string Name { get; set; }
		string Suffix { get; set; }
		string Prefix { get; set; }
		string ChatSeperator { get; set; }
		bool CanBuild { get; set; }
		Color Color { get; set; }

		void Initialize();
		void WriteElement(XmlTextWriter writer);

		void SetSuffix(string suffix);
		void SetPrefix(string prefix);
		void SetChatSeperator(string chatSeperator);
		void SetColor(Color color);
		void SetCanBuild(bool canBuild);
	}

	public struct Group : IPermission
	{
		public string Name { get; set; }

		public List<String> Permissions { get; set; }
		public List<String> DenyPermissions { get; set; }
		public Color Color { get; set; }
		public bool CanBuild { get; set; }
		public bool Default { get; set; }
		public string Prefix { get; set; }
		public string Suffix { get; set; }
		public string ChatSeperator { get; set; }
		public int Rank { get; set; }

		public void Initialize()
		{
			Permissions = new List<String>();
			DenyPermissions = new List<String>();
			Color = ChatColor.White;
			Suffix = String.Empty;
			Prefix = String.Empty;
			ChatSeperator = ": ";
		}

		public void WriteElement(XmlTextWriter writer)
		{
			writer.WriteStartElement("Group");

			writer.WriteAttributeString("Name", Name);
			writer.WriteAttributeString("Color", Color.ToParsableString());
			writer.WriteAttributeString("Default", Default.ToString());
			writer.WriteAttributeString("CanBuild", CanBuild.ToString());
			writer.WriteAttributeString("Prefix", Prefix ?? String.Empty);
			writer.WriteAttributeString("Suffix", Suffix ?? String.Empty);
			writer.WriteAttributeString("ChatSeperator", ChatSeperator ?? " ");
			writer.WriteAttributeString("Rank", Rank.ToString());

			writer.WriteStartElement("Permissions");
			foreach (var permission in Permissions)
				writer.WriteElementAndValue("Permission", permission);
			writer.WriteEndElement();

			writer.WriteStartElement("DenyPermissions");
			foreach (var permission in DenyPermissions)
				writer.WriteElementAndValue("Permission", permission);
			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		public void SetSuffix(string suffix)
		{
			Suffix = suffix;
		}

		public void SetPrefix(string prefix)
		{
			Prefix = prefix;
		}

		public void SetChatSeperator(string chatSeperator)
		{
			ChatSeperator = chatSeperator;
		}

		public void SetColor(Color color)
		{
			Color = color;
		}

		public void SetCanBuild(bool canBuild)
		{
			CanBuild = canBuild;
		}

		public Group Merge(IPermission def)
		{
			SetCanBuild(def.CanBuild);
			SetChatSeperator(def.ChatSeperator);
			SetColor(def.Color);
			SetPrefix(def.Prefix);
			SetSuffix(def.Suffix);
			return this;
		}
	}

	public struct User : IPermission
	{
		public string Name { get; set; }

		public List<Group> Groups { get; set; }
		public List<String> Permissions { get; set; }
		public List<String> DenyPermissions { get; set; }
		public Color Color { get; set; }
		public bool CanBuild { get; set; }
		public string Prefix { get; set; }
		public string Suffix { get; set; }
		public string ChatSeperator { get; set; }

		public void Initialize()
		{
			Groups = new List<Group>();
			Permissions = new List<String>();
			DenyPermissions = new List<String>();
			Color = ChatColor.White;
			Suffix = String.Empty;
			Prefix = String.Empty;
			ChatSeperator = ": ";
		}

		public void WriteElement(XmlTextWriter writer)
		{
			writer.WriteStartElement("User");

			writer.WriteAttributeString("Name", Name);
			writer.WriteAttributeString("Color", Color.ToParsableString());
			writer.WriteAttributeString("CanBuild", CanBuild.ToString());
			writer.WriteAttributeString("Prefix", Prefix ?? String.Empty);
			writer.WriteAttributeString("Suffix", Suffix ?? String.Empty);
			writer.WriteAttributeString("ChatSeperator", ChatSeperator ?? String.Empty);

			writer.WriteStartElement("UserGroups");
			foreach (var group in Groups)
				writer.WriteElementAndValue("Name", group.Name);
			writer.WriteEndElement();

			writer.WriteStartElement("Permissions");
			foreach (var permission in Permissions)
				writer.WriteElementAndValue("Permission", permission);
			writer.WriteEndElement();

			writer.WriteStartElement("DenyPermissions");
			foreach (var permission in DenyPermissions)
				writer.WriteElementAndValue("Permission", permission);
			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		public bool HasGroup(string name)
		{
			return (from x in Groups where x.Name == name select x).Count() > 0;
		}

		public void UpdateGroup(Group group)
		{
			for (var i = 0; i < Groups.Count; i++)
			{
				if (Groups[i].Name == group.Name) Groups[i] = group;
			}
		}

		public void SetSuffix(string suffix)
		{
			Suffix = suffix;
		}

		public void SetPrefix(string prefix)
		{
			Prefix = prefix;
		}

		public void SetChatSeperator(string chatSeperator)
		{
			ChatSeperator = chatSeperator;
		}

		public void SetColor(Color color)
		{
			Color = color;
		}

		public void SetCanBuild(bool canBuild)
		{
			CanBuild = canBuild;
		}

		public User Merge(IPermission def)
		{
			SetCanBuild(def.CanBuild);
			SetChatSeperator(def.ChatSeperator);
			SetColor(def.Color);
			SetPrefix(def.Prefix);
			SetSuffix(def.Suffix);
			return this;
		}
	}

	public static class XmlTextWriterExtensions
	{
		public static void WriteElementAndValue(this XmlTextWriter writer, string element, string value)
		{
			writer.WriteStartElement(element);
			writer.WriteString(value);
			writer.WriteEndElement();
		}
	}
}
