using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace Terraria_Server
{
	public static class Log
	{
		static readonly Queue<LogEntry> entries = new Queue<LogEntry> (1024);
		static volatile StreamWriter logFile;
		static Thread logThread = new Thread (LogThread);
		static AutoResetEvent logSignal = new AutoResetEvent (false);
		
		static volatile bool exit = false;
		
		static Log ()
		{
			logThread.IsBackground = false;
			logThread.Name = "LT";
			logThread.Start ();
		}
		
		struct LogEntry
		{
			public Thread   thread;
			public DateTime time;
			public string   text;
			public object[] args;
			
			public LogEntry (string text, object[] args)
			{
				this.thread = Thread.CurrentThread;
				this.time = DateTime.Now;
				this.text = text;
				this.args = args;
			}
		}
		
		public static void OpenLogFile (string path)
		{
			try
			{
				logFile = new StreamWriter (path);
			}
			catch (SystemException)
			{
				Console.WriteLine ("Couldn't open logfile \"{0}\" for writing, logging to the console only.", path);
				logFile = null;
			}
		}
		
		public static void Close ()
		{
			exit = true;
			logSignal.Set ();
		}
		
		static void Write (LogEntry entry)
		{
			lock (entries)
			{
				entries.Enqueue (entry);
			}
			logSignal.Set ();
		}
		
		public static void WriteBareLine (string text)
		{
			Write (new LogEntry { text = text });
		}

		public static void WriteBareLine (string format, params object[] args)
		{
			Write (new LogEntry { text = format, args = args });
		}

		public static void WriteLine (string text)
		{
			Write (new LogEntry (text, null));
		}
		
		public static void WriteLine (string format, params object[] args)
		{
			Write (new LogEntry (format, args));
		}
		
		static int EntryCount ()
		{
			lock (entries)
			{
				return entries.Count;
			}
		}
		
		static void WriteOut (LogEntry entry, TextWriter to)
		{
			Exception error = null;
			
			try
			{
				if (entry.args != null)
				{
					try
					{
						entry.text = string.Format (entry.text, entry.args);
					}
					catch (Exception)
					{
						entry.text = string.Format ("<Incorrect log message format string or argument list: message=\"{0}\", args=({1})>",
							entry.text, string.Join (", ", entry.args));
					}
				}
				
				var thread = "?";
				if (entry.thread != null)
				{
					if (entry.thread.Name != null)
						thread = entry.thread.Name;
					else if (entry.thread.IsThreadPoolThread)
						thread = "TPool";
				}
				
				if (entry.thread != null && entry.time != default(DateTime))
					to.WriteLine ("{0} {1}> {2}", entry.time, thread, entry.text);
				else
					to.WriteLine (entry.text);
			}
			catch (Exception e)
			{
				error = e;
			}
			
			if (error != null)
			{
				try
				{
					Console.WriteLine ("Error writing log entry:\n" + error.ToString());
				}
				catch (Exception) {}
			}
		}
		
		const int LOG_THREAD_BATCH_SIZE = 64;
		static void LogThread ()
		{
			try
			{
				var list = new LogEntry [LOG_THREAD_BATCH_SIZE];
				
				while (exit == false || EntryCount() > 0)
				{
					int items = 0;
					
					lock (entries)
					{
						while (entries.Count > 0)
						{
							list[items++] = entries.Dequeue ();
							if (items == LOG_THREAD_BATCH_SIZE) break;
						}
					}
					
					if (items == 0)
					{
						if (exit)
							break;
						else
							logSignal.WaitOne ();
					}
					
					for (int i = 0; i < items; i++)
					{
						var entry = list[i];
						list[i] = default(LogEntry);
						
						WriteOut (entry, Console.Out);
						if (logFile != null)
						{
							WriteOut (entry, logFile);
							logFile.Flush ();
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine (e.ToString());
			}
			
			Statics.IsActive = false;
			Netplay.disconnect = true;
		}
	}
}

