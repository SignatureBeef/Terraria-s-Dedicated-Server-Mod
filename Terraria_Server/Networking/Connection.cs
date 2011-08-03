using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Terraria_Server.Logging;

namespace Terraria_Server.Networking
{
	public abstract class Connection
	{
		protected struct Message
		{
			public int kind;
			public int param;
			public object content;
			
			public const int BYTES = 0;
			public const int KICK = 1;
		}
		
		protected class SocketAsyncEventArgsExt : SocketAsyncEventArgs
		{
			public Connection conn;
			
			protected override void OnCompleted (SocketAsyncEventArgs args)
			{
			}
		}
		
		protected sealed class SendArgs : SocketAsyncEventArgsExt
		{
			protected override void OnCompleted (SocketAsyncEventArgs args)
			{
				conn.SendCompleted (this);
			}
		}
		
		protected sealed class RecvArgs : SocketAsyncEventArgsExt
		{
			protected override void OnCompleted (SocketAsyncEventArgs args)
			{
				conn.ReceiveCompleted (this);
			}
		}
		
		protected sealed class KickArgs : SocketAsyncEventArgsExt
		{
			protected override void OnCompleted (SocketAsyncEventArgs args)
			{
				conn.KickCompleted (this);
			}
		}
		
		Socket socket;
		Queue<Message> sendQueue = new Queue<Message> ();
		//SocketAsyncEventArgsExt sendArgs = new SocketAsyncEventArgsExt ();
		//SocketAsyncEventArgsExt recvArgs = new SocketAsyncEventArgsExt ();
		//SocketAsyncEventArgsExt kickArgs = new SocketAsyncEventArgsExt ();
		protected byte[] recvBuffer;
		protected int    recvBytes;
		protected volatile SocketError error = SocketError.Success;
		protected Timer timeout;
		protected volatile int closed = 0;
		
		internal volatile bool kicking = false;
		internal volatile bool sending = false;
		internal volatile bool receiving = false;
		
		public int QueueLength
		{
			get { return sendQueue.Count; }
		}
		
//			lock (sectionUpdatesLock)
//				if (sectionUpdates == null || sectionUpdates.GetLength(0) <= Main.maxTilesX/200 || sectionUpdates.GetLength(1) <= Main.maxTilesY/150)
//				{
//					sectionUpdates = new byte [Main.maxTilesX / 200 + 1, Main.maxTilesY / 150 + 1][];
//				}
//			

		
		public Connection (Socket sock)
		{
			socket = sock;
			RemoteEndPoint = socket.RemoteEndPoint;
			RemoteAddress = RemoteEndPoint.ToString();
			
//			sendArgs.Done += this.SendCompleted;
//			recvArgs.Done += this.ReceiveCompleted;
//			kickArgs.Done += this.KickCompleted;
		}
		
		public bool Active
		{
			get { return error == SocketError.Success && socket.Connected; }
		}
		
		public SocketError Error
		{
			get { return error; }
		}
		
		public EndPoint RemoteEndPoint { get; protected set; }
		public string   RemoteAddress  { get; protected set; }
		public int BytesSent     { get; private set; }
		public int BytesQueued   { get; private set; }
		public int BytesReceived { get; private set; }
		
		public virtual void Send (byte[] bytes)
		{
			Send (new Message { content = bytes });
		}
		
		protected void Send (Message message)
		{
			//Logging.ProgramLog.Log ("Queue {0}.", bytes.Length);
			lock (sendQueue)
			{
				if (kicking) return;
				
				sendQueue.Enqueue (message);
				
				if (sending == false)
				{
					sending = SendMore (null);
				}
			}
			//Logging.ProgramLog.Log ("End queue.", bytes.Length);
		}
		
		public void KickAfter (byte[] bytes)
		{
			lock (sendQueue)
			{
				if (kicking) return;
				
				kicking = true;

				sendQueue.Clear ();
				sendQueue.Enqueue (new Message { content = bytes, kind = Message.KICK });
				
				if (sending == false)
				{
					sending = SendMore (null);
				}
			}
		}
		
		protected bool SendMore (SocketAsyncEventArgsExt args)
		{
			try
			{
				var queued = false;
				
				while (sendQueue.Count > 0 && !queued)
				{
					var msg = sendQueue.Dequeue();
					
					switch (msg.kind)
					{
						case Message.BYTES:
						{
							if (args == null) args = sendPool.Take(this);
							
							var data = (byte[]) msg.content;
							args.SetBuffer (data, 0, data.Length);
							BytesSent += data.Length;
							queued = socket.SendAsync (args);
							break;
						}
						
						default:
						{
							if (args == null) args = sendPool.Take(this);
							
							var data = SerializeMessage (msg);
							args.SetBuffer (data.Array, data.Offset, data.Count);
							BytesSent += data.Count;
							try
							{
								queued = socket.SendAsync (args);
							}
							finally
							{
								if (! queued) MessageSendCompleted ();
							}
							break;
						}
						
						case Message.KICK:
						{
							if (! queued && args != null && args.conn != null) sendPool.Put (args);
							
							var kickArgs = kickPool.Take (this);
							
							Close (SocketError.ConnectionAborted);
							
							if (timeout == null) timeout = new Timer (Timeout, null, 10000, 0);
							
							var data = (byte[]) msg.content;
							kickArgs.SetBuffer (data, 0, data.Length);
							if (! socket.SendAsync (kickArgs))
							{
								if (! socket.DisconnectAsync (kickArgs))
								{
									KickCompleted (kickArgs);
								}
							}
							queued = false;
							break;
						}
					}
				}
						
				return queued;
			}
			catch (SocketException e)
			{
				HandleError (e.SocketErrorCode);
			}
			catch (ObjectDisposedException e)
			{
				HandleError (SocketError.OperationAborted);
			}
			
			return false;
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
				//ProgramLog.Debug.Log ("SendCompleted {");
				if (argz.SocketError != SocketError.Success)
				{
					HandleError (argz.SocketError);
					sendPool.Put (argz);
				}
				else
				{
					if (argz.BytesTransferred < argz.Count) throw new Exception ("ugh!");
					
					lock (sendQueue)
					{
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
						receiving = socket.ReceiveAsync (argz);
						
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
						kickPool.Put (argz);
						HandleError (SocketError.Disconnecting);
					}
					else
					{
						if (timeout == null) timeout = new Timer (Timeout, null, 10000, 0);
						if (! socket.DisconnectAsync (argz))
						{
							kickPool.Put (argz);
							HandleError (SocketError.Disconnecting);
						}
					}
				}
				else
				{
					HandleError (argz.SocketError);
					kickPool.Put (argz);
				}
				
				kicking = false;
			}
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
				try
				{
					timeout.Dispose ();
				} catch {}
				
				timeout = null;
			}
			
			if (error != SocketError.Success) return;
			
			error = err;
			
			try
			{
				socket.Close ();
			}
			catch (SocketException) {}
			catch (ObjectDisposedException) {}
			
//			if (! sending)
//				try
//				{
//					sendArgs.Dispose();
//					sendArgs = null;
//				} catch {}
//			
//			if (! receiving)
//				try
//				{
//					recvArgs.Dispose();
//					recvArgs = null;
//				} catch {}
//			
//			if (! kicking)
//				try
//				{
//					kickArgs.Dispose();
//					kickArgs = null;
//				} catch {}
//			
			
			Close (error);
		}
		
		void Close (SocketError error)
		{
			kicking = true;
			var closed = Interlocked.Exchange (ref this.closed, 1);
			if (closed > 0) return;
			
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
				if (!(args is T)) ProgramLog.Error.Log ("ArgsPool type mismatch.");
				if (args.conn == null) ProgramLog.Error.Log ("SocketAsyncEventArgsExt freed twice.");
				args.conn = null;
				lock (this) Push ((T) args);
			}
		}
		
		static ArgsPool<SendArgs> sendPool = new ArgsPool<SendArgs> ();
		static ArgsPool<KickArgs> kickPool = new ArgsPool<KickArgs> ();
		static ArgsPool<RecvArgs> recvPool = new ArgsPool<RecvArgs> ();
	}
}

