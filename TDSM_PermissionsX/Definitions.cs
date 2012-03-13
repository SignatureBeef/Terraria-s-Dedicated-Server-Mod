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
	}

	public struct Group : IPermission
	{
		public string Name { get; set; }

		public List<String> Permissions { get; set; }
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
			Color = ChatColor.White;
			Suffix = String.Empty;
			Prefix = String.Empty;
			ChatSeperator = " ";
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

			writer.WriteEndElement();
		}
	}

	public struct User : IPermission
	{
		public string Name { get; set; }

		public List<Group> Groups { get; set; }
		public List<String> Permissions { get; set; }
		public Color Color { get; set; }
		public bool CanBuild { get; set; }
		public string Prefix { get; set; }
		public string Suffix { get; set; }
		public string ChatSeperator { get; set; }

		public void Initialize()
		{
			Groups = new List<Group>();
			Permissions = new List<String>();
			Color = ChatColor.White;
			Suffix = String.Empty;
			Prefix = String.Empty;
			ChatSeperator = String.Empty;
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
						writer.WriteElementAndValue("Name", Name);
				writer.WriteEndElement();

				writer.WriteStartElement("Permissions");
					foreach (var permission in Permissions)
						writer.WriteElementAndValue("Permission", permission);
				writer.WriteEndElement();

			writer.WriteEndElement();
		}

		public bool HasGroup(string name)
		{
			return (from x in Groups where x.Name == name select x).Count() > 0;
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
