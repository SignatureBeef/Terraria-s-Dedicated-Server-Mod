using System.IO;
using System;

using Terraria_Server.Definitions;
using Terraria_Server.Logging;

namespace Terraria_Server
{
	public class TConsole
	// left for compatibility
	{
//		private StreamWriter streamWriter = null;
//		private Platform.PlatformType platformType;

        public TConsole(string serverLog, Platform.PlatformType platformType)
		{
//			streamWriter = new StreamWriter(serverLog);
//			this.platformType = platformType;
		}

        public void WriteLine(string Line)
		{
			ProgramLog.Log (Line);
		}
		
		public void WriteLine (string fmt, params object[] args)
		{
			ProgramLog.Log (fmt, args);
		}
		
		public void WriteLine()
		{
			//if (platformType == Platform.PlatformType.WINDOWS)
			//{
			//	streamWriter.WriteLine();
			//	streamWriter.Flush();
			//}
		}

		public void Close()
		{
		}

	}
}
