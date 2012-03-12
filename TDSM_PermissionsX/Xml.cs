using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TDSM_PermissionsX
{
	public class Xml
	{
		private string _fileLocation { get; set; }

		public List<Group> Groups { get; set; }
		public List<User> Users { get; set; }

		public Xml(string fileLocation)
		{
			_fileLocation = fileLocation;
		}

		public bool Load()
		{
			try
			{
				using (var reader = new XmlTextReader(_fileLocation))
				{
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
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

						writer.WriteStartElement("Groups");
							foreach (var group in Groups) group.WriteElement(writer);
						writer.WriteEndElement();

						writer.WriteStartElement("Users");
							foreach (var user in Users) user.WriteElement(writer);
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
	}
}
