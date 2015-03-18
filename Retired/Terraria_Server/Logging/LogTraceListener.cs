using System;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Terraria_Server.Logging
{
	public class LogTraceListener : TraceListener
	{
		[ThreadStatic]
		static StringBuilder cache;
		
		public LogTraceListener () : base ("LogTraceListener") { }
		
		public override bool IsThreadSafe
		{
			get { return true; }
		}
		
		public override void Write (string text)
		{
			if (cache == null)
				cache = new StringBuilder ();
			cache.Append (text);
		}
		
		public override void WriteLine (string text)
		{
			if (cache != null && cache.Length > 0)
			{
				cache.Append (text);
				ProgramLog.Log (cache.ToString());
				cache.Clear ();
			}
			else
				ProgramLog.Log (text);
		}
	}
}

