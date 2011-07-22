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
		
		public void Log (string text)
		{
			ProgramLog.Log (this, text);
		}
		
		public void Log (string fmt, params object[] args)
		{
			ProgramLog.Log (this, fmt, args);
		}
	}
}

