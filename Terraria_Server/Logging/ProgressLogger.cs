using System;
using System.Threading;

namespace Terraria_Server.Logging
{
	// TODO: refactor and add a throbber too
	public class ProgressLogger : IDisposable
	{
		public int Max { get; private set; }
		public string Message { get; private set; }
		public Thread Thread { get; private set; }
		
		public ProgressLogger (int max, string message)
		{
			Max = max;
			Message = message;
			current = 0;
			done = false;
			Thread = System.Threading.Thread.CurrentThread;
			ProgramLog.AddProgressLogger (this);
		}
		
		volatile int current;
		
		public int Value
		{
			get { return current; }
			set
			{
				if (value < current) return;
				current = value;
//				Log.UpdateProgressLogger (this);
			}
		}
		
		volatile bool done;
		
		public bool Done
		{
			get { return done; }
			set
			{
				if (value == false) throw new InvalidOperationException ("Cannot undo a progress logger.");
				done = true;
				current = Max;
				ProgramLog.RemoveProgressLogger (this);
			}
		}
		
		public void Dispose ()
		{
			Done = true;
		}
	}
}

