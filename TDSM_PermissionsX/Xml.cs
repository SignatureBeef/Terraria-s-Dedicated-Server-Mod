using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace TDSM_PermissionsX
{
	public class Xml
	{
		private string _fileLocation { get; set; }

		public List<Group> Groups { get; set; }
		public List<User> Users { get; set; }

		public Xml(string fileLocation, bool load = true)
		{
			_fileLocation = fileLocation;

			Groups = new List<Group>();
			Users = new List<User>();

			if (load) Load();
		}

		public bool Load()
		{
			try
			{
				if (!File.Exists(_fileLocation)) return false; // File.Create(_fileLocation).Close();

				using (var reader = new XmlTextReader(_fileLocation))
				{
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								var elementName = reader.Name;

								switch (elementName)
								{
									case "Groups":
										ParseKnownElement(reader, false);
										break;
									case "Users":
										ParseKnownElement(reader);
										break;
								}
								break;
						}
					}
				}
			}
			catch (Exception e)
			{
				PermissionsX.XLog("Exception loading in {0}\n{1}", GetType().FullName, e);
			}

			return false;
		}

		public bool Save()
		{
			try
			{
				using (var writer = new XmlTextWriter(_fileLocation, Encoding.ASCII))
				{
					writer.WriteStartDocument();
					writer.WriteStartElement("XPermissions");

					writer.WriteStartElement("Groups");
					foreach (var group in Groups) group.WriteElement(writer);
					writer.WriteEndElement();

					writer.WriteStartElement("Users");
					foreach (var user in Users) user.WriteElement(writer);
					writer.WriteEndElement();

					writer.WriteEndElement();
					writer.WriteEndDocument();

					return true;
				}
			}
			catch (Exception e)
			{
				PermissionsX.XLog("Exception saving in {0}\n{1}", GetType().FullName, e);
			}

			return false;
		}

		public void ParseKnownElement(XmlTextReader reader, bool User = true)
		{
			var doc = new XmlDocument();
			doc.Load(reader);

			var users = doc.ChildNodes[0];

			if (users.HasChildNodes)
			{
				foreach (XmlNode node in users.ChildNodes)
				{
					if (node.Attributes.Count > 0)
					{
						var attribute = node.Attributes["Name"];
						if (attribute != null)
						{
							var username = attribute.Value;

							if (User)
								Users.Add(new User()
								{
									Name = username
								});
							else
								Groups.Add(new Group()
								{
									Name = username
								});
						}
					}
				}
			}
		}
	}
}
