using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;
using Terraria_Server.Logging;
using System.IO;
using Terraria_Server;

namespace TDSM_PermissionsX
{
	public class PermissionsX : Commands
	{
		public XProperties Properties { get; set; }
		
		public string GetPath
		{
			get 
			{ return Path.Combine(Statics.PluginPath, "XPermissions"); }
		}
		
		public string GetPropertiesPath
		{
			get
			{ return Path.Combine(GetPath, "XPermissions.properties"); }
		}

		public PermissionsX()
		{
			Name = "Permissions X";
			Description = "XML based permissions system.";
			TDSMBuild = 37;
			Author = "TDSM Dev Team";
			Version = "1.0.0.0";
		}

		protected override void Initialized(object state)
		{
			XLog("Initializing...");

			Properties = new XProperties(GetPropertiesPath);
			Properties.Load();
			Properties.pushData();
			Properties.Save(false);
		}

		protected override void Enabled()
		{
			XLog("Enabled");
		}

		protected override void Disabled()
		{
			XLog("Disabled");
		}

		public static void XLog(string format, params object[] args)
		{
			ProgramLog.Plugin.Log("[XPermission] " + format, args);
		}
	}
}
