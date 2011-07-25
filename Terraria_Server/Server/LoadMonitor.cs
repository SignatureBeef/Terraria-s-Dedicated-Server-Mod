using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Terraria_Server
{
	public static class LoadMonitor
	{
		static Thread monitorThread;
		
		public static void Start ()
		{
			if (monitorThread != null) return;
			
			monitorThread = new Thread (MonitorLoop);
			monitorThread.IsBackground = true;
			monitorThread.Name = "LMon";
			monitorThread.Start ();
			
			LoadLastMinute = -1;
			LoadLastSecond = -1;
		}
		
		struct Entry
		{
			public TimeSpan realTime;
			public TimeSpan cpuTime;
		}
		
		static readonly Queue<Entry> loads = new Queue<Entry> (60);
		
		public static double LoadLastSecond { get; private set; }
		public static double LoadLastMinute { get; private set; }
		
		static void MonitorLoop ()
		{
			var timer = new Stopwatch ();
			timer.Start ();
			
			var process = Process.GetCurrentProcess();
			
			Entry last = new Entry { realTime = timer.Elapsed, cpuTime = process.TotalProcessorTime };
			
			while (true)
			{
				Thread.Sleep (1000);
				
				var now = new Entry { realTime = timer.Elapsed, cpuTime = process.TotalProcessorTime };
				
				LoadLastSecond = (now.cpuTime - last.cpuTime).TotalMilliseconds * 100.0 / (now.realTime - last.realTime).TotalMilliseconds;
				
				if (loads.Count == 60)
				{
					var minuteAgo = loads.Dequeue ();
					LoadLastMinute = (now.cpuTime - minuteAgo.cpuTime).TotalMilliseconds * 100.0 / (now.realTime - minuteAgo.realTime).TotalMilliseconds;
				}
				
				loads.Enqueue (now);
			}
		}
	}
}

