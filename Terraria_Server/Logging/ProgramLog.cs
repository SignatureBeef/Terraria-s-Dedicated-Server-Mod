using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

using Terraria_Server.Misc;

namespace Terraria_Server.Logging
{
	public static class ProgramLog
	{
		static readonly Queue<LogEntry> entries = new Queue<LogEntry> (1024);
		static ProgramThread dispatchThread = new ProgramThread ("LogD", LogDispatchThread);
		static ProducerConsumerSignal logSignal = new ProducerConsumerSignal (false);
		static LogTarget console = new StandardOutputTarget ();
		static LogTarget logFile = null;
		
		static volatile bool exit = false;
		
		static ProgramLog ()
		{
			dispatchThread.IsBackground = false;
			dispatchThread.Start ();
			
			lock (logTargets)
				logTargets.Add (console);
		}
		
		public static readonly LogChannel Users = new LogChannel ("USR", ConsoleColor.Magenta);
		public static readonly LogChannel Chat  = new LogChannel ("CHT", ConsoleColor.DarkMagenta);
		public static readonly LogChannel Death = new LogChannel ("DTH", ConsoleColor.Green);
		public static readonly LogChannel Admin = new LogChannel ("ADM", ConsoleColor.Yellow);
        public static readonly LogChannel Error = new LogChannel("ERR", ConsoleColor.Red);
        public static readonly LogChannel Debug = new LogChannel("DBG", ConsoleColor.DarkGray);
        public static readonly LogChannel Plugin = new LogChannel("PGN", ConsoleColor.Blue);
		
		struct LogEntry
		{
			public Thread     thread;
			public DateTime   time;
			public object     message;
			public object     args;
			public LogTarget  target;
			public LogChannel channel;
			public SendingLogger logger;
			
			public LogEntry (object message, object args, SendingLogger logger = SendingLogger.CONSOLE)
			{
				this.target = null;
				this.thread = Thread.CurrentThread;
				this.time = DateTime.Now;
				this.message = message;
				this.args = args;
				this.channel = null;
				this.logger = logger;
			}
		}

		static List<LogTarget> logTargets = new List<LogTarget> ();
		
		public static void OpenLogFile (string path)
		{
			var newpath = path;
			
			if (Program.properties.LogRotation)
			{
				var absolute = Path.GetFullPath (path);
				var dir = Path.GetDirectoryName (absolute);
				var name = Path.GetFileNameWithoutExtension (path);
				var ext = Path.GetExtension (path);
				newpath = Path.Combine (dir, String.Format ("{0}_{1:yyyyMMdd_HHmm}{2}", name, DateTime.Now, ext));
			}
			
			logFile = new FileOutputTarget (newpath);
			
			lock (logTargets)
				logTargets.Add (logFile);
			
			Log ("Logging started to file \"{0}\".", newpath);
		}
		
		public static void AddTarget (LogTarget target)
		{
			lock (logTargets) logTargets.Add (target);
		}
		
		public static void RemoveTarget (LogTarget target)
		{
			lock (logTargets) logTargets.Remove (target);
		}
		
		public static void Close ()
		{
			exit = true;
			logSignal.Signal ();
		}
		
		static void Write (LogEntry entry)
		{
			lock (entries)
			{
				entries.Enqueue (entry);
			}
			logSignal.Signal ();
		}
		
		public static void BareLog (string text)
		{
			Write (new LogEntry { message = text, thread = Thread.CurrentThread });
		}

		public static void BareLog (string format, params object[] args)
		{
			Write (new LogEntry { message = format, args = args, thread = Thread.CurrentThread });
		}

		public static void BareLog (LogChannel channel, string text)
		{
			Write (new LogEntry { message = text, thread = Thread.CurrentThread, channel = channel });
		}

		public static void BareLog (LogChannel channel, string format, params object[] args)
		{
			Write (new LogEntry { message = format, args = args, thread = Thread.CurrentThread, channel = channel });
		}

		public static void Log(string text)
		{
			Write(new LogEntry(text, null));
		}

		public static void Log(string text, SendingLogger logger)
		{
			Write(new LogEntry(text, null) { logger = logger });
		}
		
		public static void Log (string format, params object[] args)
		{
			Write (new LogEntry (format, args));
		}
		
		public static void Log (LogChannel channel, string text, bool multi = false)
		{
			if (!multi)
				Write(new LogEntry(text, null) { channel = channel });
			else
			{
				var split = text.Split('\n');

				foreach (var line in split)
					Write(new LogEntry(line, null) { channel = channel });
			}
		}

		public static void Log(LogChannel channel, string format, params object[] args)
		{
			Write(new LogEntry(format, args) { channel = channel });
		}

		public static void Log(LogChannel channel, string format, SendingLogger logger, params object[] args)
		{
			Write(new LogEntry(format, args) { channel = channel, logger = logger });
		}
		
		public static void Log (Exception e)
		{
			Write (new LogEntry (e, null) { channel = Error });
		}
		
		public static void Log (Exception e, string text)
		{
			Write (new LogEntry (e, text) { channel = Error });
		}
		
		public static void AddProgressLogger (ProgressLogger prog)
		{
			Write (new LogEntry (prog, null));
		}
		
		public static void RemoveProgressLogger (ProgressLogger prog)
		{
			Write (new LogEntry (prog, null));
		}
		
		public static class Console
		{
			public static void Print (string text)
			{
				ProgramLog.Write (new LogEntry { target = console, message = text, thread = Thread.CurrentThread });
			}
		}
		
		static int EntryCount ()
		{
			lock (entries)
			{
				return entries.Count;
			}
		}
		
		static Dictionary<Thread, string> poolNames = new Dictionary<Thread, string> ();
        //static int nextPoolIndex = 0;
		
		static void Build (LogEntry entry, out OutputEntry output)
		{
			Exception error = null;
			output = default (OutputEntry);
			
			if (entry.channel != null)
				output.color = entry.channel.Color;

			output.logger = entry.logger;
			
			try
			{
				if (entry.message is string)
				{
					var text = (string) entry.message;
					
					if (entry.args != null)
					{
						var args = (object[]) entry.args;
						try
						{
							text = String.Format (text, args);
						}
						catch (Exception)
						{
							text = String.Format ("<Incorrect log message format string or argument list: message=\"{0}\", args=({1})>",
								text, String.Join (", ", args));
						}
					}
					
					output.message = text;
				}
				else if (entry.message is Exception)
				{
					var e = (Exception) entry.message;
					
					if (entry.args is string)
						output.message = String.Format ("{0}:{1}{2}", entry.args, Environment.NewLine, e.ToString());
					else
						output.message = e.ToString();
				}
				else
					output.message = entry.message;
				
				var thread = "?";
				if (entry.thread != null)
				{
					if (entry.thread.IsThreadPoolThread)
					{
						thread = "Pool";
//						string name;
//						if (poolNames.TryGetValue (entry.thread, out name))
//						{
//							thread = name;
//						}
//						else
//						{
//							thread = String.Format ("P{0:000}", nextPoolIndex++);
//							poolNames[entry.thread] = thread;
//						}
					}
					else if (entry.thread.Name != null)
						thread = entry.thread.Name;
				}
				
				if (entry.thread != null && entry.time != default(DateTime))
					output.prefix = String.Format ("{0} {1}> ", entry.time, thread);
			}
			catch (Exception e)
			{
				error = e;
			}
			
			if (error != null)
			{
				try
				{
					System.Console.WriteLine ("Error writing log entry:");
					System.Console.WriteLine (error.ToString());
				}
				catch (Exception) {}
			}
		}
		
		static void Send (LogTarget target, OutputEntry output)
		{
			if (target == null)
			{
				lock (logTargets)
					foreach (var tar in logTargets)
					{
						tar.Send (output);
					}
			}
			else
				target.Send (output);
		}
		
		public const int LOG_THREAD_BATCH_SIZE = 64;
		
		static void LogDispatchThread ()
		{
			try
			{
				var list = new LogEntry [LOG_THREAD_BATCH_SIZE];
				var progs = new List<ProgressLogger> ();
				var last = default(OutputEntry);
				var run = 0;
				
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
							logSignal.WaitForIt ();
					}
					
					for (int i = 0; i < items; i++)
					{
						var entry = list[i];
						list[i] = default(LogEntry);
						OutputEntry output;
						
						Build (entry, out output);
						
						if (entry.message is ProgressLogger)
						{
							var prog = (ProgressLogger) entry.message;
							
							if (progs.Remove (prog))
							{
								// it's done
								output.arg = -2;
							}
							else
							{
								// new one
								progs.Add (prog);
								output.arg = -1;
							}
						}
						else
						{
							// force updates of progress loggers in the same thread
							foreach (var prog in progs)
							{
								if (prog.Thread == entry.thread)
								{
									var upd = new OutputEntry { prefix = output.prefix, message = prog, arg = prog.Value };
									Send (entry.target, upd);
									last = upd;
									run = 0;
								}
							}
						}
						
						if (output.message.Equals (last.message) && output.prefix == last.prefix && output.arg == last.arg)
						{
							run += 1;
							//System.Console.WriteLine (run);
						}
						else if (run > 0)
						{
							//System.Console.WriteLine ("sending");
							last.message = String.Format ("Log message repeated {0} times", run);
							Send (entry.target, last);
							last = output;
							run = 0;
							Send (entry.target, output);
						}
						else
						{
							last = output;
							Send (entry.target, output);
						}
					}
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine (e.ToString());
			}
			
			lock (logTargets)
				foreach (var tar in logTargets)
				{
					tar.Close ();
				}
			
			//Statics.IsActive = false;
			NetPlay.disconnect = true;
		}
	}
}

