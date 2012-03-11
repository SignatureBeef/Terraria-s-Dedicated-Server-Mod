using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.WorldMod;
using System.IO;
using Terraria_Server.Logging;

namespace Terraria_Server.Misc
{
	public enum BackupResult : int
	{
		SUCCESS = 0,
		LOAD_FAIL = 1,
		SAVE_FAIL = 2,
		SAVE_LOCK = 3
	}

	public class BackupManager
	{
		/*public static BackupResult LoadWorld(string Name)
		{
			WorldIO.LoadWorld(null, null, Statics.WorldPath + Path.DirectorySeparatorChar + Name);

			if (WorldModify.loadFailed && !WorldModify.loadSuccess)
			{
				return BackupResult.LOAD_FAIL;
			}

			return BackupResult.SUCCESS;
		}*/

		public static BackupResult SaveWorld(string Path)
		{
			if (WorldModify.saveLock) return BackupResult.SAVE_LOCK;
			//Please wait for the current operation to finish.

			if (WorldIO.SaveWorld(Path))
				return BackupResult.SUCCESS;
			else
				return BackupResult.SAVE_FAIL;
		}

		public static BackupResult PerformBackup(string WorldName)
		{
			try
			{
				ProgramLog.Log("Performing backup...");

				var file = GetStamptedFilePath(WorldName);
				while (File.Exists(file))
					file = GetStamptedFilePath(WorldName);

				return SaveWorld(file + ".wld");				
			}
			catch (Exception e) { ProgramLog.Log(e); }

			return BackupResult.SAVE_FAIL;
		}

		public static BackupResult PerformBackup()
		{
			if (String.IsNullOrEmpty(Main.worldName))
				throw new Exception("Main.worldName must be initialized.");

			return PerformBackup(Main.worldName);
		}

		public static string GetTimeStamp(string WorldName)
		{
			return String.Format("{0}_{1:yyyyMMddHHmmss}", WorldName, DateTime.Now); 
		}

		public static string GetStamptedFilePath(string WorldName)
		{
			var name = GetTimeStamp(WorldName);
			return Path.Combine(Statics.WorldBackupPath, name);
		}

		public static string[] GetBackups(string WorldName)
		{
			return Directory.GetFiles(Statics.WorldBackupPath).Where(x => Path.GetFileName(x).StartsWith(WorldName + "_")).ToArray();
		}

		public static string[] GetBackupsBefore(string WorldName, DateTime date)
		{
			var backups = GetBackups(WorldName);
			var oldBackups = from x in backups where File.GetCreationTime(x) < date select x;
			return oldBackups.ToArray();
		}

		public static string[] GetExpiredBackup(string WorldName, int max)
		{
			var backups = GetBackups(WorldName);

			var oldBackups = (from x in backups orderby File.GetCreationTime(x) select x).ToList();
			var amount = oldBackups.Count;

			if (amount <= max)
				return null;

			oldBackups.RemoveRange(0, max);
			return oldBackups.ToArray();
		}

		public static void AutoPurge(string WorldName)
		{
			var maxTime = Program.properties.PurgeBackupsMinutes;
			if (maxTime == 0)
				return;

			ProgramLog.Log("Performing backup purge...");
			var backups = GetBackups(WorldName);

			var expired = (from x in backups where (DateTime.Now - File.GetCreationTime(x)).TotalMinutes >= maxTime select x).ToArray();
			//var deleted = 0;
			foreach (var file in expired)
			{
				try
				{
					File.Delete(file);
					//deleted++;
				}
				catch { }
			}
		}

		public static void AutoPurge()
		{
			AutoPurge(Main.worldName);
		}
	}
}
