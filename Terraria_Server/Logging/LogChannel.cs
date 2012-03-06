using System;

namespace Terraria_Server.Logging
{
	public class LogChannel
	{
		public ConsoleColor Color { get; private set; }
		public string Name { get; private set; }
		
		public LogChannel (string name, ConsoleColor color)
		{
			Color = color;
			Name = name;
		}

		public void Log(string text, bool multi = false)
		{
			ProgramLog.Log(this, text, multi);
		}

		public void Log(string text)
		{
			ProgramLog.Log(this, text, SendingLogger.CONSOLE);
		}

		public void Log(string text, SendingLogger logger)
		{
			ProgramLog.Log(this, text, logger);
		}

		public void Log(string fmt, params object[] args)
		{
			ProgramLog.Log(this, fmt, args);
		}

		public void Log(string fmt, SendingLogger logger, params object[] args)
		{
			ProgramLog.Log(this, fmt, args);
		}
	}
}

