using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace TDSM_PermissionsX
{
	public class XProperties : PropertiesFile
	{
		public XProperties(string propertiesPath) : base(propertiesPath) { }

		public void pushData()
		{
			object temp = null;
			temp = AllowRestrictAutoDownload;
		}

		public bool AllowRestrictAutoDownload
		{
			get { return getValue("allow-restrict-autodl", true); }
			set { setValue("allow-restrict-autodl", value); }
		}
	}
}
