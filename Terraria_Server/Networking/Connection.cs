using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Terraria_Server.Logging;
using Terraria_Server.Misc;

namespace Terraria_Server.Networking
{
	public abstract class Connection
	{
		
		static readonly int ARRAY_OBJECT_OVERHEAD = 4 + 3 * IntPtr.Size; // value for mono
		
		protected struct Message
		{
			public int kind;
			public int param;
			public object content;
			
			public const int BYTES = 0;
			public const int KICK = 1;
			public const int SEGMENT = 2;
		}
		
		static readonly int MESSAGE_SIZE = 4 + 4 + IntPtr.Size;
		
		protected class SocketAsyncEventArgsExt : SocketAsyncEventArgs
		{
			public volatile Connection conn;
			
			protected override void OnCompleted (SocketAsyncEventArgs args)
			{
			}
		}
		
		protected sealed class SendArgs : SocketAsyncEventArgsExt
		{
			protected override void OnCompleted (SocketAsyncEventArgs args)
			{
				var c = conn;
				if (c != null) c.SendCompleted (this);
			}
		}
		
		protected sealed class RecvArgs : SocketAsyncEventArgsExt
		{
			protected override void OnCompleted (SocketAsyncEventArgs args)
			{
				var c = conn;
				if (c != null) c.ReceiveCompleted (this);
			}
		}
		
		protected sealed class KickArgs : SocketAsyncEventArgsExt
		{
			protected override void OnCompleted (SocketAsyncEventArgs args)
			{
				var c = conn;
				if (c != null) c.KickCompleted (this);
			}
		}
		
		Socket socket;
		ArrayDeque<Message> sendQueue = new ArrayDeque<Message> ();
		
		protected byte[] recvBuffer;
		protected int    recvBytes;
		protected Timer timeout;
		protected volatile int closed = 0;
		protected volatile int error = 0;
		
		// TODO: maybe move these to ServerSlot
		protected byte[] txBuffer;    // buffer for copying messages into
		protected int    txHead;      // first byte used in buffer
		protected int    txCount;     // number of bytes used in buffer
		protected int    txPrepared;  // number of bytes from buffer put into txList
		protected int    txListBytes; // number of bytes in txList elements
		// list of array segments to send at once
		protected List<ArraySegment<byte>> txList = new List<ArraySegment<byte>> ();
		
		internal volatile bool kicking = false;
		internal volatile bool sending = false;
		internal volatile bool receiving = false;
		
		public int QueueLength
		{
			get { return sendQueue.Count; }
		}
		
		public Connection (Socket sock)
		{
			socket = sock;
			RemoteEndPoint = socket.RemoteEndPoint;
            RemoteAddress = RemoteEndPoint.ToString();
		}
		
		public bool Active
		{
			get { return closed == 0 && error == (int)SocketError.Success && socket.Connected; }
		}
		
		public SocketError Error
		{
			get { return closed == 1 ? SocketError.ConnectionAborted : (SocketError) error; }
		}
		
		public EndPoint RemoteEndPoint { get; protected set; }
		public string   RemoteAddress  { get; protected set; }
		public int BytesSent     { get; private set; }
		public int BytesReceived { get; private set; }
		
		protected int queueSize;
		
		// amount of memory used for the send queue of this connection
		// including the messages and the queue itself
		// excluding custom messages like sections and txbuffer
		public int QueueSize
		{
			get
			{
				return queueSize + MESSAGE_SIZE * sendQueue.Capacity;
			}
		}
		
//		public static long TotalOutgoingBytes { get { return totalBytesBuffered; } }
//		public static long TotalOutgoingBytesUnbuffered { get { return totalBytesUnbuffered; } }
		
		//static long totalBytesBuffered;
		//static long totalBytesUnbuffered;
		
		public virtual void Send (byte[] bytes)
		{
			Send (new Message { content = bytes });
		}
		
		public virtual void CopyAndSend (ArraySegment<byte> segment)
		{
			if (CheckQuota () == false)
				return;

			lock (sendQueue)
			{
				if (kicking) return;
				
				if (txBuffer == null || segment.Count > txBuffer.Length - txCount)
				{
					var bytes = new byte [segment.Count];
					Array.Copy (segment.Array, segment.Offset, bytes, 0, segment.Count);
					sendQueue.PushBack (new Message { content = bytes });
					queueSize += segment.Count + ARRAY_OBJECT_OVERHEAD;
				}
				else
				{
					int offset = (txHead + txCount) % txBuffer.Length;
					int first = Math.Min (segment.Count, txBuffer.Length - offset);
					
					Array.Copy (segment.Array, segment.Offset, txBuffer, offset, first);
					if (first < segment.Count) //wraparound
					{
						Array.Copy (segment.Array, segment.Offset + first, txBuffer, 0, segment.Count - first);
					}
					//ProgramLog.Debug.Log ("Using {0}, {1}", offset, segment.Count);
					
					if (sendQueue.Count > 0 && sendQueue.PeekBack().kind == Message.SEGMENT)
					{
						// coalesce
						var old = sendQueue.PeekBack();
						sendQueue.ReplaceBack (new Message { kind = Message.SEGMENT, param = old.param + segment.Count });
					}
					else
					{
						sendQueue.PushBack (new Message { kind = Message.SEGMENT, param = segment.Count });
					}
					
					txCount += segment.Count;
					queueSize += segment.Count;
				}
				
				if (sending == false)
				{
					sending = SendMore (null);
				}
			}
		}
		
		protected void Send (Message message)
		{
			//Logging.ProgramLog.Log ("Queue {0}.", bytes.Length);
			if (CheckQuota () == false)
				return;
			
			lock (sendQueue)
			{
				if (kicking) return;
				
				sendQueue.PushBack (message);
				
				if (message.kind == Message.BYTES)
				{
					queueSize += ((byte[]) message.content).Length + ARRAY_OBJECT_OVERHEAD;
				}
				
				if (sending == false)
				{
					sending = SendMore (null);
				}
			}
			//Logging.ProgramLog.Log ("End queue.", bytes.Length);
		}
		
		bool CheckQuota ()
		{
			if (QueueSize >= Program.properties.SendQueueQuota * 1024)
			{
				// this is an awful hack but I was in a hurry
				var cc = (ClientConnection) this;
				cc.Kick ("Not enough bandwidth or timed out.");
				return false;
			}
			
			return true;
		}
		
		public void KickAfter (byte[] bytes)
		{
			lock (sendQueue)
			{
				if (kicking) return;
				
				kicking = true;

				sendQueue.Clear ();
				sendQueue.PushBack (new Message { content = bytes, kind = Message.KICK });
				queueSize = 0;
				
				timeout = new Timer (Timeout, null, 15000, 0);
				Close (SocketError.ConnectionAborted);
				
				if (sending == false)
				{
					sending = SendMore (null);
				}
			}
		}
		
//		protected int flushCounter = 0;
		
		public void Flush () // TODO: add different priorities to messages indicating how fast the should be flushed
		{
//			flushCounter += 1;
//			if (flushCounter < 5) return;
//			flushCounter = 0;
			
			lock (sendQueue)
			{
				if (! sending && txListBytes > 0)
					sending = SendMore (null, true);
			}
		}
		
//#if BANDWIDTH_ANALYSIS
//		public static int[] packetsPerMessage = new int [255];
//		public static long[] bytesPerMessage = new long [255];
//#endif
		
		protected bool SendMore (SocketAsyncEventArgsExt args, bool flush = false)
		{
			flush |= txBuffer == null;
			
			try
			{
				var queued = false;
				var escape = false;
				
				while (sendQueue.Count > 0 && txListBytes < 2880 && !escape)
				{
					var msg = sendQueue.PopFront();
					
					switch (msg.kind)
					{
						case Message.BYTES:
						{
							var data = (byte[]) msg.content;
							txList.Add (new ArraySegment<byte> (data));
							txListBytes += data.Length;
							queueSize -= data.Length + ARRAY_OBJECT_OVERHEAD;
							//ProgramLog.Debug.Log ("{1}: Adding bytes {0}", data.Length, Thread.CurrentThread.IsThreadPoolThread);
							break;
						}
						
						case Message.SEGMENT:
						{
							var len = msg.param;
							var txc = txList.Count;
							int wraparound = 0;
							
							if (txc > 0 && txList[txc - 1].Array == txBuffer)
							{
								var seg = txList[txc - 1];
								var nlen = seg.Count + len;
								var nseg = new ArraySegment<byte> (txBuffer, seg.Offset, Math.Min (nlen, txBuffer.Length - seg.Offset));
								
								//ProgramLog.Debug.Log ("{5}: Coalescing {0}, {1} and {2}, {3} [{4}]", seg.Offset, seg.Count, (txHead + txPrepared) % txBuffer.Length, len, nseg.Count, Thread.CurrentThread.IsThreadPoolThread);
								
								txList[txc - 1] = nseg;
								
								wraparound = nlen - nseg.Count;
							}
							else
							{
								var offset = (txHead + txPrepared) % txBuffer.Length;
								
								//ProgramLog.Debug.Log ("{2}: Adding segment {0}, {1}", offset, len, Thread.CurrentThread.IsThreadPoolThread);
								
								var nseg = new ArraySegment<byte> (txBuffer, offset, Math.Min (len, txBuffer.Length - offset));
								txList.Add (nseg);
								
								wraparound = len - nseg.Count;
							}
							
							if (wraparound > 0)
							{
								txList.Add (new ArraySegment<byte> (txBuffer, 0, wraparound));
							}
							
							txPrepared += len;
							txListBytes += len;
							queueSize -= len;

							break;
						}
						
						default:
						{
							var data = SerializeMessage (msg);
							txList.Add (data);
							txListBytes += data.Count;
							//ProgramLog.Debug.Log ("{1}: Adding custom {0}", data.Count, Thread.CurrentThread.IsThreadPoolThread);
							escape = true;
							break;
						}
						
						case Message.KICK:
						{
							if ((! queued) /*&& (! sending)*/ && args != null && args.conn != null) sendPool.Put (args);
							
							//txList.Clear ();
							//txListBytes = 0;
							
							var kickArgs = kickPool.Take (this);
							
							var data = (byte[]) msg.content;
							kickArgs.SetBuffer (data, 0, data.Length);
							if (! socket.SendAsync (kickArgs))
							{
								if (! socket.DisconnectAsync (kickArgs))
								{
									KickCompleted (kickArgs);
								}
							}
							
							return false;
						}
					}
				}
				
				if (escape) flush = true;
				
				if (txListBytes >= 1450 || (txListBytes > 0 && flush))
				{
					if (args == null) args = sendPool.Take(this);
					
					if (txList.Count > 1)
					{
						args.SetBuffer (null, 0, 0);
						args.BufferList = txList;
					}
					else
					{
						var seg = txList[0];
						args.BufferList = null;
						args.SetBuffer (seg.Array, seg.Offset, seg.Count);
					}
					
//					var o = 0;
//					for (int i = 0; i < txList.Count; i++)
//						o += 40 * (1 + txList[i].Count / 1460) + txList[i].Count;
//					
					var n = 40 * (1 + txListBytes / 1460) + txListBytes;
//					Interlocked.Add (ref totalBytesBuffered, (long)n);
//					Interlocked.Add (ref totalBytesUnbuffered, (long)o);
					
					BytesSent += n;
					
					try
					{
						queued = socket.SendAsync (args);
					}
					finally
					{
						if (! queued)
						{
							TxListClear ();
							if (escape) MessageSendCompleted ();
						}
					}
				}
				
				return queued;
			}
			catch (SocketException e)
			{
				HandleError (e.SocketErrorCode);
			}
			catch (ObjectDisposedException)
			{
				HandleError (SocketError.OperationAborted);
			}
			
			return false;
		}
		
		protected void TxListClear ()
		{
			//ProgramLog.Debug.Log ("Cleaning up txList of {0} ({1} bytes)", txList.Count, txListBytes);
			for (int i = 0; i < txList.Count; i++)
			{
				var seg = txList[i];
				if (seg.Array == txBuffer)
				{
					var count = seg.Count;
					//ProgramLog.Debug.Log ("Freeing {0} ({2}), {1}", txHead, count, txList[i].Offset);
					txHead = (txHead + count) % txBuffer.Length;
					txCount -= count;
					txPrepared -= count;
					if (txPrepared < 0 || txCount < 0) ProgramLog.Error.Log ("{0} {1}", txCount, txPrepared);
				}
			}
			if (txCount == 0) txHead = 0;
			txList.Clear ();
			txListBytes = 0;
		}
		
		// a place where subclasses may cleanup after sending a custom message,
		// for instance, return memory to a pool or release locks
		protected virtual void MessageSendCompleted ()
		{
		}
		
		protected virtual ArraySegment<byte> SerializeMessage (Message msg)
		{
			return new ArraySegment<byte> ();
		}
		
		protected void SendCompleted (SocketAsyncEventArgsExt argz)
		{
			try
			{
				MessageSendCompleted ();
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception in connection send callback");
			}

			try
			{
				//ProgramLog.Debug.Log ("SendCompleted [");
				if (argz.SocketError != SocketError.Success)
				{
					HandleError (argz.SocketError);
					sendPool.Put (argz);
				}
				else
				{
					lock (sendQueue)
					{
//						if (kicking)
//						{
//							sendPool.Put (argz);
//							sending = false;
//							return;
//						}
						
						TxListClear ();
						
						sending = SendMore (argz);
						if (! sending && argz.conn != null) sendPool.Put (argz);
					}
				}
				//ProgramLog.Debug.Log ("} SendCompleted " + sending);
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception in connection send callback");
			}
		}
		
		public void StartReceiving (byte[] buffer)
		{
			recvBuffer = buffer;
			var args = recvPool.Take (this);
			args.SetBuffer (buffer, 0, buffer.Length);
			
			receiving = true;
			if (! socket.ReceiveAsync (args))
				ReceiveCompleted (args);
		}
		
		protected void ReceiveCompleted (SocketAsyncEventArgsExt argz)
		{
			try
			{
				if (argz.SocketError != SocketError.Success)
				{
					//ProgramLog.Log ("Bytes received: {0}", argz.BytesTransferred);
					var err = argz.SocketError;
					receiving = false;
					recvPool.Put (argz);
					HandleError (err);
				}
				else if (argz.BytesTransferred == 0)
				{
					//ProgramLog.Log ("Clean connection shutdown. {0}", socket.Connected);
					receiving = false;
					recvPool.Put (argz);
					HandleError (SocketError.Disconnecting);
				}
				else
				{
					var bytes = argz.BytesTransferred;
					receiving = false;
					
					if (kicking)
					{
						recvPool.Put (argz);
						return;
					}
					
					while (! receiving)
					{
						recvBytes += bytes;
						BytesReceived += bytes;
						
						ProcessRead ();
						
						if (kicking)
						{
							receiving = false;
							break;
						}
						
						var left = recvBuffer.Length - recvBytes;
						
						if (left <= 0) return;
						
						argz.SetBuffer (recvBuffer, recvBytes, left);
						try
						{
							receiving = socket.ReceiveAsync (argz);
						}
						catch (ObjectDisposedException)
						{
							receiving = false;
						}
						
						if (receiving) bytes = argz.BytesTransferred;
					}
					
					if (! receiving) recvPool.Put (argz);
				}
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception in connection receive callback");
			}
		}
		
		protected void KickCompleted (SocketAsyncEventArgsExt argz)
		{
			try
			{
				if (argz.SocketError == SocketError.Success)
				{
					if (argz.LastOperation == SocketAsyncOperation.Disconnect)
					{
						kickPool.Put(argz);
						HandleError(SocketError.Disconnecting);
					}
					else
					{
						if (!socket.DisconnectAsync(argz))
						{
							kickPool.Put(argz);
							HandleError(SocketError.Disconnecting);
						}
					}
				}
				else
				{
					HandleError(argz.SocketError);
					kickPool.Put(argz);
				}

				//				kicking = false;
			}
			catch (ObjectDisposedException) { }
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception in connection disconnect callback");
			}
		}
		
		protected virtual void Timeout (object dummy)
		{
			HandleError (SocketError.TimedOut);
		}
		
		protected abstract void ProcessRead ();
		
		protected void HandleError (SocketError err)
		{
//			ProgramLog.Debug.Log ("HandleError {0}", err);
			
			if (timeout != null)
			{
				var timer = Interlocked.CompareExchange (ref this.timeout, null, this.timeout);
				
				if (timer != null)
				{
					try
					{
						timer.Dispose ();
					} catch {}
				}
			}
			
#pragma warning disable 420
			if (Interlocked.CompareExchange (ref this.error, (int)err, (int)SocketError.Success) != (int)SocketError.Success)
				return;
#pragma warning restore 420
			
			Close (err);
			
			try
			{
				socket.Close ();
			}
			catch (SocketException) {}
			catch (ObjectDisposedException) {}
			
			//TxListClear ();
			//txBuffer = null;
		}
		
		void Close (SocketError error)
		{
			kicking = true;
			
#pragma warning disable 420
			if (Interlocked.CompareExchange (ref this.closed, 1, 0) != 0)
				return;
#pragma warning restore 420
			
			try
			{
				HandleClosure (error);
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception while handling connection closure");
			}
		}
		
		protected abstract void HandleClosure (SocketError error);
		
		class ArgsPool<T> : Stack<T> where T : SocketAsyncEventArgsExt, new()
		{
			int total;
			
			public T Take (Connection conn)
			{
//				ProgramLog.Debug.Log ("Take");
				T args;
				lock (this)
				{
					if (Count > 0)
						args = Pop();
					else
					{
						total += 1;
						ProgramLog.Debug.Log ("ArgsPool<{0}> capacity is now: {1}.", typeof(T).Name, total);
						args = new T();
					}
				}
				args.conn = conn;
				return args;
			}
			
			public void Put (SocketAsyncEventArgsExt args)
			{
//				ProgramLog.Debug.Log ("Put");
				if (!(args is T))
				{
					ProgramLog.Error.Log ("ArgsPool type mismatch.");
					return;
				}
				
				lock (this)
				{
					if (args.conn == null)
					{
						ProgramLog.Error.Log ("{0} freed twice.", typeof(T).Name);
						return;
					}
					
					args.BufferList = null;
					
					args.conn = null;
					
					Push ((T) args);
				}
			}
		}
		
		static ArgsPool<SendArgs> sendPool = new ArgsPool<SendArgs> ();
		static ArgsPool<KickArgs> kickPool = new ArgsPool<KickArgs> ();
		static ArgsPool<RecvArgs> recvPool = new ArgsPool<RecvArgs> ();
	}
}

