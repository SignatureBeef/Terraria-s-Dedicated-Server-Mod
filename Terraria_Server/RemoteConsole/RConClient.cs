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
		public enum State
		{
			GREETING,
			LOGIN_PROMPT,
			PASSWORD_PROMPT,
			ONLINE,
		}
		
		public string Name { get; private set; }
		
		internal byte[] readBuffer;
		internal int    bytesRead;
		internal int    bytesChecked;
		internal string remoteAddress;
		internal Socket socket;
		internal RConSender sender;
		internal NetworkStream stream;
		internal volatile State state;
		internal int retries;
		
		public RConClient (Socket socket, string addr) : base ("RCWT", new StreamWriter (new NetworkStream (socket)))
		{
			remoteAddress = addr;
			Id = addr;
			this.socket = socket;
			stream = (NetworkStream) ((StreamWriter) writer).BaseStream;
			passExceptions = true;
			((StreamWriter) writer).AutoFlush = true;
			((StreamWriter) writer).NewLine = "\r\n";
			readBuffer = new byte [1024];
			this.sender = new RConSender (this);
			state = State.GREETING;
			retries = 0;
			Name = "";
			ProgramLog.AddTarget (this);
		}
		
		public string Id { get; private set; }
		
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
			
			int timeout = 20;
			while (state != State.ONLINE)
			{
				Thread.Sleep (1000);
				timeout -= 1;
				if (timeout == 0)
				{
					writer.WriteLine ("");
					writer.WriteLine ("\x1b[1;37mTimed out.");
					socket.SafeShutdown ();
					Close ();
				}
			}
			
			try
			{
				base.OutputThread();
			}
			catch (IOException e)
			{
				ProgramLog.Debug.Log ("{0}: exception while sending ({1})", Id, e.Message);
			}
			catch (SocketException e)
			{
				ProgramLog.Debug.Log ("{0}: exception while sending ({1})", Id, e.Message);
			}
			catch (ObjectDisposedException e)
			{
				ProgramLog.Debug.Log ("{0}: exception while sending ({1})", Id, e.Message);
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception within WriteThread of remote console " + Id);
			}
			
			ProgramLog.RemoveTarget (this);
			
			socket.SafeShutdown ();
			socket.SafeClose();
		}
		
		internal bool ProcessRead ()
		{
			int start = 0;
			
			//Console.Write ("(start={0}, bytesChecked={1}) ", start, bytesChecked);
			
			int i;
			for (i = bytesChecked; i < bytesRead; i++)
			{
				//Console.Write ("{0},", readBuffer[i]);
				switch (readBuffer[i])
				{
					case 10:
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
						break;
					}
					case 251:
					case 252:
					case 253:
					case 254:
					case 255:
					{
						start = i + 1;
						break;
					}
				}
			}
			
			if (start > 0)
			{
				if (start < bytesRead)
					Buffer.BlockCopy (readBuffer, i, readBuffer, 0, bytesRead - start);
				
				bytesRead -= start;
			}
			else if (bytesRead == readBuffer.Length)
			{
				return false;
			}
			
			bytesChecked = bytesRead;
			
			return true;
			
			//Console.WriteLine ("(processed={0}, bytesChecked={1}) ", start, bytesChecked);
		}
		
		internal void Greet ()
		{
			state = State.LOGIN_PROMPT;
			SendHello ();
			PromptLogin ();
		}
		
		internal void SendHello ()
		{
			writer.WriteLine (ASCIIArt);
			writer.WriteLine ("");
			writer.WriteLine ("\x1b[1;37mTerraria {0} dedicated server remote console, running TDSM #{1}.\x1b[0m", Program.VERSION_NUMBER, Statics.BUILD);
			writer.WriteLine ("\x1b[1;37mYou have 20 seconds to log in.\x1b[0m");
			writer.Flush ();
		}
		
		internal void PromptLogin ()
		{
			writer.Write ("\x1b[1;36mLogin:\x1b[0m ");
			stream.WriteByte (255); // telnet go-ahead
			stream.WriteByte (249);
			writer.Flush ();
		}
		
		internal void PromptPassword ()
		{
			writer.Write ("\x1b[1;36mPassword:\x1b[0m ");
			stream.WriteByte (255); // telnet go-ahead
			stream.WriteByte (249);
			writer.Flush ();
		}
		
		internal void ProcessLine (string line)
		{
			if (line == null || line.Trim() == "")
				return;
			
			switch (state)
			{
				case State.GREETING:
				{
					state = State.LOGIN_PROMPT;
					SendHello ();
					PromptLogin ();
					break;
				}
					
				case State.LOGIN_PROMPT:
				{
					Name = line.Trim();
					state = State.PASSWORD_PROMPT;
					PromptPassword ();
					break;
				}
				
				case State.PASSWORD_PROMPT:
				{
					var pass = line.Trim();
					if (pass.Length == 0)
					{
						PromptPassword ();
						break;
					}
					
					if (RConServer.LoginDatabase.getValue (Name) != RConServer.Hash (Name, pass))
					{
						ProgramLog.Admin.Log ("Remote console auth failure for user \"{0}\" from {1}", Name, remoteAddress);
						writer.WriteLine ("\x1b[1;31m... no!\x1b[0m");
						writer.WriteLine ("");
						retries += 1;
						if (retries == 3)
						{
							Close ();
							break;
						}
						state = State.LOGIN_PROMPT;
						PromptLogin ();
					}
					else
					{
						Id = Name + "@" + remoteAddress;
						ProgramLog.Admin.Log ("User \"{0}\" has logged in to a remote console from {1}", Name, remoteAddress);
						writer.WriteLine ("");
						writer.WriteLine ("\x1b[1;34mWelcome, you can now enter commands.\x1b[0m");
						writer.WriteLine ("");
						state = State.ONLINE;
					}
					break;
				}
				
				case State.ONLINE:
				{
					ProgramLog.Admin.Log ("Remote command from {0} ({1}): \"{2}\"", Name, remoteAddress, line);
					
					try
					{
						Program.commandParser.ParseConsoleCommand (line, Program.server, this.sender);
					}
					catch (Exception e)
					{
						ProgramLog.Log (e, "Issue parsing remote command from " + Id);
					}
					break;
				}
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
		
		public override void Close ()
		{
			socket.SafeClose ();
			base.Close ();
		}
		
		static readonly string ASCIIArt = ("\x1b[1;31m" +
			@"           (     (      *     " + "\r\n" +
			@"      *   ))\ )  )\ ) (  `    " + "\r\n" +
			@"    ` )  /(()/( (()/( )\))(   " + "\r\n" + //"\x1b[1;33m" +
			@"     ( )(_))(_)) /(_)|(_)()\  " + "\r\n" +
			@"    (_(_()|_))_ (_)) (_()((_) " + "\r\n" +"\x1b[0;32m" +
			@"    |_   _||   \/ __||  \/  | " + "\r\n" +
			@"      | |  | |) \__ \| |\/| | " + "\r\n" +
			@"      |_|  |___/|___/|_|  |_| " + "\r\n" +
			 "  Terraria Dedicated Server Mod").Replace("(", "\x1b[1;33m(\x1b[1;31m");

	}

}

