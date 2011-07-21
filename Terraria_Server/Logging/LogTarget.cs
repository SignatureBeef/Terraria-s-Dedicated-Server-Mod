using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

using Terraria_Server.Misc;

namespace Terraria_Server.Logging
{
	public struct OutputEntry
	{
		public string prefix;
		public object message;
		public int    arg;
	}

	public class LogTarget
	{
		protected Queue<OutputEntry> entries = new Queue<OutputEntry> (1024);
		protected ProducerConsumerSignal signal = new ProducerConsumerSignal (false);
		protected bool exit = false;
		
		public void Send (OutputEntry entry)
		{
			lock (entries)
			{
				entries.Enqueue (entry);
			}
			signal.Signal ();
		}
		
		public void Close ()
		{
			exit = true;
			signal.Signal ();
		}
		
		protected int EntryCount ()
		{
			lock (entries)
			{
				return entries.Count;
			}
		}
	}
	
	public class StandardOutputTarget : LogTarget
	{
		Thread thread;
		
		public StandardOutputTarget ()
		{
			thread = new Thread (OutputThread);
			thread.Start ();
		}
		
		void OutputThread ()
		{
			Thread.CurrentThread.Name = "Log1";
			
			try
			{
				var list = new OutputEntry [ProgramLog.LOG_THREAD_BATCH_SIZE];
				var progs = new List<ProgressLogger> ();
				var backspace = 0;
				
				while (exit == false || EntryCount() > 0)
				{
					int items = 0;
					
					lock (entries)
					{
						while (entries.Count > 0)
						{
							list[items++] = entries.Dequeue ();
							if (items == ProgramLog.LOG_THREAD_BATCH_SIZE) break;
						}
					}
					
					if (items == 0)
					{
						if (exit)
							break;
						else if (progs.Count == 0)
							signal.WaitForIt ();
					}
					
					if (backspace > 0)
					{
						Console.Write ("\r");
					}
					
					for (int i = 0; i < items; i++)
					{
						var entry = list[i];
						list[i] = default(OutputEntry);
						
						if (entry.prefix != null)
						{
							Console.Write (entry.prefix);
							backspace -= entry.prefix.Length;
						}
							
						if (entry.message is string)
						{
							var str = (string) entry.message;
							Console.WriteLine (str);
							backspace -= str.Length;
						}
						else if (entry.message is ProgressLogger)
						{
							var prog = (ProgressLogger) entry.message;
							var str = "";
							
							if (entry.arg == -1) // new one
							{
								progs.Add (prog);
								str = string.Format ("{0}: started.", prog.Message);
							}
							else if (entry.arg == -2) // finished one
							{
								progs.Remove (prog);
								str = string.Format ("{0}: done.", prog.Message);
							}
							else // update
							{
								double percent = entry.arg * 100.0 / prog.Max;
								str = string.Format ("{0}: {3:0.0}% ({1}/{2})", prog.Message, entry.arg, prog.Max, percent);
							}
							
							backspace -= str.Length;
							if (backspace <= 0)
								Console.WriteLine (str);
							else
							{
								Console.Write (str);
								for (int j = 0; j < backspace; j++)
									Console.Write (" ");
							}
						}
					}
					
					backspace = 0;
					foreach (var prog in progs)
					{
						double percent = prog.Value * 100.0 / prog.Max;
						var str = string.Format ("[ {0}: {1:0.0}% ] ", prog.Message, percent);
						backspace += str.Length;
						Console.Write (str);
					}
					
					if (backspace > 0 && EntryCount() == 0)
					{
						signal.WaitForIt (100);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine (e.ToString());
			}
		}
	}
	
	public class FileOutputTarget : LogTarget
	{
		StreamWriter file;
		
		public FileOutputTarget (string path)
		{
			file = new StreamWriter (path);
		}
		
		void OutputThread ()
		{
			Thread.CurrentThread.Name = "LogF";
			Thread.CurrentThread.IsBackground = false;
			
			try
			{
				var list = new OutputEntry [ProgramLog.LOG_THREAD_BATCH_SIZE];
				
				while (exit == false || EntryCount() > 0)
				{
					int items = 0;
					
					lock (entries)
					{
						while (entries.Count > 0)
						{
							list[items++] = entries.Dequeue ();
							if (items == ProgramLog.LOG_THREAD_BATCH_SIZE) break;
						}
					}
					
					if (items == 0)
					{
						if (exit)
							break;
						else
							signal.WaitForIt ();
					}
					
					for (int i = 0; i < items; i++)
					{
						var entry = list[i];
						list[i] = default(OutputEntry);
						
						if (entry.prefix != null)
							file.Write (entry.prefix);
							
						if (entry.message is string)
							file.WriteLine ((string) entry.message);
						
						file.Flush ();
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine (e.ToString());
			}
		}

	}
}

