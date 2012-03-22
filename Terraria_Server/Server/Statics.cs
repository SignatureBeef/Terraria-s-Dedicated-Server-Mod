using System;
using System.IO;

namespace Terraria_Server
{
	public static class Statics
	{
		/* Terraria */
		public const String VERSION_NUMBER = "v1.1.2";
		public const Int32 CURRENT_TERRARIA_RELEASE = 39;
		public static string CURRENT_TERRARIA_RELEASE_STR = CURRENT_TERRARIA_RELEASE.ToString();

		/* TDSM */
		public const Int32 BUILD = 38;
		public const String CODENAME = "Classified";

		private const String WORLDS = "Worlds";
		private const String PLUGINS = "Plugins";
		private const String DATA = "Data";
		private const String LIBRARIES = "Libs";
		private const String BACKUPS = "Backups";

		public const String TDCM_QUEST_GIVER = "Quest Giver";

		public static bool WorldLoaded
		{
			get;
			internal set;
		}

		public static bool PermissionsEnabled = false;

		public static bool cmdMessages = true;

		public static volatile bool Exit = false;

		public static string SavePath = Environment.CurrentDirectory;

		public static string WorldPath
		{
			get
			{
				return Path.Combine(SavePath, WORLDS);
			}
		}

		public static string WorldBackupPath
		{
			get
			{
				return Path.Combine(SavePath, WORLDS, BACKUPS);
			}
		}

		public static string PluginPath
		{
			get
			{
				return Path.Combine(SavePath, PLUGINS);
			}
		}

		public static string LibrariesPath
		{
			get
			{
				return Path.Combine(SavePath, PLUGINS, LIBRARIES);
			}
		}

		public static string DataPath
		{
			get
			{
				return Path.Combine(SavePath, DATA);
			}
		}
	}
}
