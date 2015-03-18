using System;
using System.Diagnostics;
using System.Threading;

namespace Terraria_Server.Logging
{
	// TODO: refactor and add a throbber too
	public class ProgressLogger : IDisposable
	{
		public int Max { get; private set; }
		public string Message { get; private set; }
		public Thread Thread { get; private set; }
		
		protected Stopwatch timer;
		
		public ProgressLogger (int max, string message)
		{
			Max = max;
			Message = message;
			current = 0;
			done = false;
			Thread = System.Threading.Thread.CurrentThread;
			timer = new Stopwatch ();
			timer.Start ();
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
				timer.Stop ();
				ProgramLog.RemoveProgressLogger (this);
			}
		}
		
		public TimeSpan RunningTime
		{
			get { return timer.Elapsed; }
		}
		
		public string PrettyTimeSpan (TimeSpan ts)
		{
			var ms = ts.TotalMilliseconds;
			
			if (ms < 1000)
			{
				return String.Format ("{0:0}ms", ms);
			}
			else if (ms < 61000)
			{
				return String.Format ("{0:0.000}s", ms / 1000.0);
			}
			else if (ms < 3600 * 1000)
			{
				int x = (int)ms;
				return String.Format ("{0}m {1}s", x / 60000, (x % 60000) / 1000);
			}
			else
				return ts.ToString();
		}
		
		public string Format (bool verbose = false, int value = -1)
		{
			if (done)
			{
				if (verbose)
					return String.Format ("{0}: done in {1}.", Message, PrettyTimeSpan (RunningTime));
				else
					return String.Format ("{0}: done.", Message);
			}
			
			if (value == -1) value = current;
			
			double percent = value * 100.0 / Max;
			if (verbose)
				return String.Format ("{0}: {1:0.0}% ({2}/{3})", Message, percent, value, Max);
			else
				return String.Format ("{0}: {1:0.0}%", Message, percent);
		}
		
		public void Dispose ()
		{
			Done = true;
		}
	}
}

