using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;

using Terraria_Server.Logging;
using Terraria_Server.Misc;

namespace Terraria_Server.RemoteConsole
{
	public class RConClient : InteractiveLogTarget
	{
		internal byte[] readBuffer;
		internal int    bytesRead;
		internal int    bytesChecked;
		internal string remoteAddress;
		internal Socket socket;
		internal RConSender sender;
		
		public RConClient (Socket socket, string addr) : base ("RCWT", new StreamWriter (new NetworkStream (socket)))
		{
			remoteAddress = addr;
			this.socket = socket;
			passExceptions = true;
			((StreamWriter) writer).AutoFlush = true;
			((StreamWriter) writer).NewLine = "\r\n";
			readBuffer = new byte [1024];
			this.sender = new RConSender (this);
			
			ProgramLog.AddTarget (this);
		}
		
		public void Send (byte[] data)
		{
			Send (new OutputEntry { message = data });
		}
		
		public void WriteLine (string data)
		{
			Send (new OutputEntry { message = data });
		}
		
		protected override void OutputThread ()
		{
			thread.IsBackground = true;
			
			try
			{
				base.OutputThread();
			}
			catch (IOException e)
			{
				ProgramLog.Debug.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
			}
			catch (SocketException e)
			{
				ProgramLog.Debug.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
			}
			catch (ObjectDisposedException e)
			{
				ProgramLog.Debug.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception within WriteThread of a remote console");
			}
			
			ProgramLog.RemoveTarget (this);
			
			socket.SafeShutdown ();
			socket.SafeClose();
		}
		
		internal void ProcessRead ()
		{
			int start = 0;
			
			//Console.Write ("(start={0}, bytesChecked={1}) ", start, bytesChecked);
			
			int i;
			for (i = bytesChecked; i < bytesRead; i++)
			{
				//Console.Write ("{0},", readBuffer[i]);
				if (readBuffer[i] == 10)
				{
					if (readBuffer[start] == 13) start += 1;
					
					if (i > 0 && readBuffer[i - 1] == 13)
					{
						if (i - start > 1)
							ProcessLine (Encoding.UTF8.GetString (readBuffer, start, i - start - 1));
					}
					else if (i - start > 0)
						ProcessLine (Encoding.UTF8.GetString (readBuffer, start, i - start));
					
					start = i + 1;
					
					if (bytesRead > i + 2 && readBuffer[start + 2] == 13)
					{
						start += 1;
					}
				}
			}
			
			if (start > 0)
			{
				if (start < bytesRead)
					Buffer.BlockCopy (readBuffer, i, readBuffer, 0, bytesRead - start);
				
				bytesRead -= start;
			}
			
			bytesChecked = bytesRead;
			
			//Console.WriteLine ("(processed={0}, bytesChecked={1}) ", start, bytesChecked);
		}
		
		internal void ProcessLine (string line)
		{
			if (line == null || line.Trim() == "")
				return;
			
			ProgramLog.Admin.Log ("Remote command from {0}: \"{1}\"", remoteAddress, line);
			
			try
			{
				Program.commandParser.ParseConsoleCommand (line, Program.server, this.sender);
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Issue parsing remote command");
			}
		}
		
		static string[] colors =
		{
			"\x1b[0;30m",
			"\x1b[0;34m",
			"\x1b[0;32m",
			"\x1b[0;36m",
			"\x1b[0;31m",
			"\x1b[0;35m",
			"\x1b[0;33m",
			"\x1b[0;37m",
			
			"\x1b[1;30m",
			"\x1b[1;34m",
			"\x1b[1;32m",
			"\x1b[1;36m",
			"\x1b[1;31m",
			"\x1b[1;35m",
			"\x1b[1;33m",
			"\x1b[1;37m",
		};
		
		protected override void SetColor (ConsoleColor color)
		{
			writer.Write (colors [(int)color]);
		}
		
		protected override void ResetColor ()
		{
			writer.Write ("\x1b[0m");
		}
	}

}

