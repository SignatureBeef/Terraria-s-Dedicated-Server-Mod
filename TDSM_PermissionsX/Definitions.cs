using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TDSM_PermissionsX
{
	public interface IPermission
	{
		string Name { get; set; }

		void WriteElement(XmlTextWriter writer);
	}

	public struct Group : IPermission
	{
		public string Name { get; set; }

		public void WriteElement(XmlTextWriter writer)
		{
			writer.WriteStartElement("Group");

			writer.WriteAttributeString("Name", Name);

			writer.WriteEndElement();
		}
	}

	public struct User : IPermission
	{
		public string Name { get; set; }
		
		public void WriteElement(XmlTextWriter writer)
		{
			writer.WriteStartElement("User");

			writer.WriteAttributeString("Name", Name);

			writer.WriteEndElement();
		}
	}
}
