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
//	public class RemoteConsole
//	{
//		internal byte[] readBuffer;
//		internal int    bytesRead;
//		internal string remoteAddress;
//		internal Socket socket;
//		
//		internal Queue<object> writeQueue;
//		internal Thread writeThread;
//		internal ProducerConsumerSignal writeSignal;
//		
//		NetworkStream stream;
//		StreamWriter  writer;
//		
//		public RemoteConsole (Socket socket, string addr)
//		{
//			remoteAddress = addr;
//			this.socket = socket;
//			stream = new NetworkStream (socket);
//			writer = new StreamWriter (stream);
//			writeQueue = new Queue<object> ();
//			writeSignal = new ProducerConsumerSignal (false);
//			writeThread = new Thread (WriteLoop);
//			writeThread.Start ();
//		}
//		
//		public void Send (byte[] data)
//		{
//			if (data == null)
//			{
//				throw new ArgumentException ("Data to send cannot be null");
//			}
//			
//			lock (writeQueue)
//			{
//				writeQueue.Enqueue (data);
//			}
//			writeSignal.Signal ();
//		}
//		
//		public void WriteLine (string data)
//		{
//			if (data == null)
//			{
//				throw new ArgumentException ("Data to send cannot be null");
//			}
//			
//			lock (writeQueue)
//			{
//				writeQueue.Enqueue (data);
//			}
//			writeSignal.Signal ();
//		}
//		
//		const int WRITE_THREAD_BATCH_SIZE = 32;
//		internal void WriteLoop ()
//		{
//			Thread.CurrentThread.Name = "RCWT";
//			Thread.CurrentThread.IsBackground = true;
//
//			object[] list = new object [WRITE_THREAD_BATCH_SIZE][];
//			while (true)
//			{
//				var queue = writeQueue;
//				var socket = this.socket;
//				var remoteAddress = this.remoteAddress;
//				int items = 0;
//				bool kill = false;
//				
//				try
//				{
//					lock (queue)
//					{
//						while (queue.Count > 0)
//						{
//							list[items++] = queue.Dequeue();
//							if (items == WRITE_THREAD_BATCH_SIZE) break;
//						}
//					}
//					
//					if (items == 0)
//					{
//						writeSignal.WaitForIt ();
//						continue;
//					}
//					
//					SocketError error = SocketError.Success;
//					
//					try
//					{
//						for (int i = 0; i < items; i++)
//						{
//							if (list[i] is byte[])
//							{
//								var bytes = (byte[]) list[i];
//								int count = 0;
//								int size = bytes.Length;
//								while (size - count > 0)
//									count += socket.Send (bytes, count, size - count, 0, out error);
//							}
//							else if (list[i] is string)
//							{
//								writer.WriteLine ((string) list[i]);
//							}
//							
//							if (error != SocketError.Success) break;
//						}
//					}
//					finally
//					{
//						for (int i = 0; i < items; i++)
//							list[i] = null;
//						
//						if (error != SocketError.Success)
//						{
//							ProgramLog.Log ("{0}: error while sending ({1})", remoteAddress, error);
//							kill = true;
//						}
//					}
//				}
//				catch (SocketException e)
//				{
//					ProgramLog.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
//					kill = true;
//				}
//				catch (ObjectDisposedException e)
//				{
//					ProgramLog.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
//					kill = true;
//				}
//				catch (Exception e)
//				{
//					ProgramLog.Log (e, "Exception within WriteThread of a remote console");
//					kill = true;
//				}
//				
//				if (kill)
//				{
//					lock (queue)
//					{
//						if (queue.Count > 0)
//							queue.Clear ();
//					}
//
//					for (int i = 0; i < items; i++)
//						list[i] = null;
//					
//					socket.SafeShutdown ();
//					socket.SafeClose();
//					
//					return;
//				}
//			}
//		}
//		
//		internal void ProcessRead ()
//		{
//		}
//	}

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
				ProgramLog.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
			}
			catch (SocketException e)
			{
				ProgramLog.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
			}
			catch (ObjectDisposedException e)
			{
				ProgramLog.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
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
			
			if (bytesRead > 0 && readBuffer[0] == 13)
				start = 1;
			
			int i;
			for (i = bytesChecked; i < bytesRead; i++)
			{
				if (readBuffer[i] == 10)
				{
					ProcessLine (Encoding.UTF8.GetString (readBuffer, start, i - start));
					
					start = i + 1;
					
					if (bytesRead > i + 2 && readBuffer[start + 2] == 13)
					{
						start += 1;
					}
				}
			}
			
			if (i < bytesRead)
				Buffer.BlockCopy (readBuffer, i, readBuffer, 0, bytesRead - i);
			
			bytesRead -= i;
			bytesChecked = bytesRead;
		}
		
		internal void ProcessLine (string line)
		{
			ProgramLog.Log ("Remote command from {0}: \"{1}\"", remoteAddress, line);
			
			try
			{
				Program.commandParser.ParseConsoleCommand (line, Program.server, this.sender);
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Issue parsing remote command");
			}
		}
	}

}

