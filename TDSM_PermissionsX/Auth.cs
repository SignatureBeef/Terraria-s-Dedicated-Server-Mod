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

		public void InitSystem(Languages languages)
		{
			if (!IsRestrictRunning())
			{
				ProgramLog.Plugin.Log(languages.NoAuth);

				string restrict = Statics.PluginPath + Path.DirectorySeparatorChar + "RestrictPlugin";
				string dll = restrict + ".dll";

				if (!UpdateManager.performUpdate(RestrictLink, restrict + ".upt", restrict + ".bak", dll, 1, 1, "Restrict"))
				{
					ProgramLog.Error.Log(languages.RestrictDlFailed);
					return;
				}

				PluginLoadStatus loadStatus = PluginManager.LoadAndInitPlugin(dll);
				if (loadStatus != PluginLoadStatus.SUCCESS)
					ProgramLog.Error.Log(
						"{0}\n{1} {2}", languages.RestrictLoadFail, languages.RestrictLoadResult, loadStatus);
			}
		}

		public bool IsRestrictRunning()
		{
			//return (from x in PluginManager.plugins.Values where x != null && x.Author == Author select x).Count() > 0;
			return PluginManager.plugins.Where(x => x.Value.Author == Author).Count() > 0;
		}
	}
}
