using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Logging;
using Terraria_Server;
using System.IO;
using Terraria_Server.Plugins;

namespace TDSM_PermissionsX
{
	public class Auth
	{
		public const String RestrictLink = "http://update.tdsm.org/RestrictPlugin.dll";
		public const String Author = "UndeadMiner";

		public void InitSystem()
		{
			if (!IsRestrictRunning())
			{
				ProgramLog.Plugin.Log("No login system found! Restrict will be downloaded.");

				string restrict = Statics.PluginPath + Path.DirectorySeparatorChar + "RestrictPlugin";
				string dll = restrict + ".dll";

				if (!UpdateManager.performUpdate(RestrictLink, restrict + ".upt", restrict + ".bak", dll, 1, 1, "Restrict"))
				{
					ProgramLog.Error.Log("Restrict failed to download!");
					return;
				}

				PluginLoadStatus loadStatus = PluginManager.LoadAndInitPlugin(dll);
				if (loadStatus != PluginLoadStatus.SUCCESS)
					ProgramLog.Error.Log("Restrict failed to install!\nLoad result: {0}", loadStatus);
			}
		}

		public bool IsRestrictRunning()
		{
			return (from x in PluginManager.plugins.Values where x != null && x.Author == Author select x).Count() > 0;
		}
	}
}
