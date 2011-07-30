using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;

using Terraria_Server.Logging;
using Terraria_Server.Networking;

namespace Terraria_Server
{
	[Flags]
	public enum SlotState : int
	{
		SHUTDOWN = 1,        // the client's socket is being shut down unconditionally
		KICK = 2,            // the client is being kicked, disconnect him after sending him all remaining data

		VACANT = 4,          //                this socket has no client and is available
		CONNECTED = 8,       // previously 0,  the client socket has been accepted
		SERVER_AUTH = 16,    //           -1,  the client has been asked for a server password
		ACCEPTED = 32,       //            1,  the client has successfully authenticated
		PLAYER_AUTH = 64,    //                the client has been asked for a character password
		SENDING_WORLD = 128, //            2,  the client requested world info
		SENDING_TILES = 256, //            3,  the client requested tiles
		PLAYING = 512,       //            10
		
		// composites
		DISCONNECTING = 3,   // SHUTDOWN or KICK
	}

	public class ServerSlot
	{
		//public volatile Socket socket;
		//public volatile SlotState state;
		public volatile ClientConnection conn;
		
		public SlotState state
		{
			get { return conn == null ? SlotState.VACANT : conn.State; }
			set
			{
				if (value == SlotState.VACANT)
					conn = null;
				else
					conn.State = value;
			}
		}
		
		public int whoAmI;
		public String statusText2;
		public int statusCount;
		public int statusMax;
		public volatile string remoteAddress;
		public bool[,] tileSection = new bool[Main.maxTilesX / 200, Main.maxTilesY / 150];
		public String statusText = "";
		public int timeOut;
		public bool announced;
		public String name = "Anonymous";
		public String oldName = "";
		public float spamProjectile;
		public float spamAddBlock;
		public float spamDelBlock;
		public float spamWater;
		public float spamProjectileMax = 100f;
		public float spamAddBlockMax = 100f;
		public float spamDelBlockMax = 500f;
		public float spamWaterMax = 50f;
		
		public byte[] readBuffer;
		
		private volatile Queue<byte[]> writeQueue;
		private ProgramThread  writeThread;
		private AutoResetEvent writeSignal;
		
		public bool Connected
		{
			get
			{
				try
				{
					return (conn != null && conn.Active); //(socket != null && socket.Connected);
				}
				catch (SocketException)
				{
					return false;
				}
				catch (ObjectDisposedException)
				{
					return false;
				}
			}
		}
		
		public ServerSlot ()
		{
			writeQueue = new Queue<byte[]> ();
			writeSignal = new AutoResetEvent (false);
			state = SlotState.VACANT;
		}
		
		public void SpamUpdate()
		{
			if (!Netplay.spamCheck)
			{
				this.spamProjectile = 0f;
				this.spamDelBlock = 0f;
				this.spamAddBlock = 0f;
				this.spamWater = 0f;
				return;
			}
			if (this.spamProjectile > this.spamProjectileMax)
			{
				NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Projectile spam");
			}
			if (this.spamAddBlock > this.spamAddBlockMax)
			{
				NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Add tile spam");
			}
			if (this.spamDelBlock > this.spamDelBlockMax)
			{
				NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Remove tile spam");
			}
			if (this.spamWater > this.spamWaterMax)
			{
				NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Liquid spam");
			}
			this.spamProjectile -= 0.4f;
			if (this.spamProjectile < 0f)
			{
				this.spamProjectile = 0f;
			}
			this.spamAddBlock -= 0.3f;
			if (this.spamAddBlock < 0f)
			{
				this.spamAddBlock = 0f;
			}
			this.spamDelBlock -= 5f;
			if (this.spamDelBlock < 0f)
			{
				this.spamDelBlock = 0f;
			}
			this.spamWater -= 0.2f;
			if (this.spamWater < 0f)
			{
				this.spamWater = 0f;
			}
		}
		
		public void SpamClear()
		{
			this.spamProjectile = 0f;
			this.spamAddBlock = 0f;
			this.spamDelBlock = 0f;
			this.spamWater = 0f;
		}
		
		public void Reset()
		{
            tileSection = new bool[Main.maxTilesX / 200, Main.maxTilesY / 150];
			
			if (tileSection.GetLength(0) >= Main.maxSectionsX && tileSection.GetLength(1) >= Main.maxSectionsY)
			{
				for (int i = 0; i < Main.maxSectionsX; i++)
					for (int j = 0; j < Main.maxSectionsY; j++)
						tileSection[i, j] = false;
			}
			else
			{
				tileSection = new bool [Main.maxSectionsX, Main.maxSectionsY];
			}
			
			var oldPlayer = Main.players[this.whoAmI];
			if (oldPlayer != null && state != SlotState.VACANT)
			{
				NetMessage.OnPlayerLeft (oldPlayer, this, announced);
			}
			announced = false;
			this.remoteAddress = "<unknown>";
			
			if (this.whoAmI < 255)
			{
				Main.players[this.whoAmI] = new Player();
			}
			
			this.timeOut = 0; // TODO: move to connection
			this.statusCount = 0;
			this.statusMax = 0;
			this.statusText2 = "";
			this.statusText = "";
			this.name = "Anonymous";
			this.conn = null;
			
			this.SpamClear();
			NetMessage.buffer[this.whoAmI].Reset();
			
			conn = null;
			//socket.SafeClose ();
		}
		
		public void ServerWriteCallBack(IAsyncResult ar)
		{
			NetMessage.buffer[this.whoAmI].spamCount--;
			if (this.statusMax > 0)
			{
				this.statusCount++;
			}
		}
		
		public void Kick (string reason)
		{
			if (state == SlotState.VACANT) return;
			
			conn.Kick (reason);
		}
		
		public void Send (byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentException ("Data to send cannot be null");
			}
			
			if (conn == null) return;
			
			//ProgramLog.Log ("ServerSlot.Send");
			conn.Send (data);
	
//			lock (writeQueue)
//			{
//				if (writeThread == null)
//				{
//					writeThread = new ProgramThread (string.Format ("W{0:000}", whoAmI), this.WriteThread);
//					writeThread.Start ();
//				}
//
//				writeQueue.Enqueue (data);
//			}
//			writeSignal.Set ();
		}
		
		public void Signal ()
		{
			writeSignal.Set ();
		}

		public void Send (byte[] data, int offset, int length)
		{
			if (data == null)
			{
				throw new ArgumentException ("Data to send cannot be null");
			}
			
			if (conn == null) return;
			
			var copy = new byte [length];
			Array.Copy (data, offset, copy, 0, length);
			
			//ProgramLog.Log ("ServerSlot.Send");
			conn.Send (copy);
	
//			lock (writeQueue)
//			{
//				if (writeThread == null)
//				{
//					writeThread = new ProgramThread (string.Format ("W{0:000}", whoAmI), this.WriteThread);
//					writeThread.Start ();
//				}
//
//				writeQueue.Enqueue (copy);
//			}
//			writeSignal.Set ();
		}

//		const int WRITE_THREAD_BATCH_SIZE = 32;
//		internal void WriteThread ()
//		{
//			//Thread.CurrentThread.Name = string.Format ("W{0:000}", whoAmI);
//			
//			byte[][] list = new byte[WRITE_THREAD_BATCH_SIZE][];
//			while (true)
//			{
//				bool kill = false;
//				var queue = writeQueue;
//				var socket = this.socket;
//				var remoteAddress = this.remoteAddress;
//				int items = 0;
//				
//				try
//				{
//					bool kick = false;
//					
//					lock (queue)
//					{
//						while (queue.Count > 0)
//						{
//							list[items++] = queue.Dequeue();
//							if (items == WRITE_THREAD_BATCH_SIZE) break;
//						}
//						
//						kick = (state == SlotState.KICK) && (writeQueue.Count == 0);
//					}
//					
//					if (state == SlotState.SHUTDOWN)
//					{
//						kill = true;
//						writeSignal.WaitOne (250);
//					}
//					else if (items == 0)
//					{
//						writeSignal.WaitOne (1000);
//						continue;
//					}
//					
//					SocketError error = SocketError.Success;
//					
//					try
//					{
//						for (int i = 0; i < items; i++)
//						{
//							int count = 0;
//							int size = list[i].Length;
//							while (size - count > 0)
//								count += socket.Send (list[i], count, size - count, 0, out error);
//							
//							if (error != SocketError.Success) break;
//							
//							NetMessage.buffer[this.whoAmI].spamCount--;
//							if (this.statusMax > 0)
//							{
//								this.statusCount++;
//							}
//						}
//					}
//					finally
//					{
//						for (int i = 0; i < items; i++)
//							list[i] = null;
//						
//						if (error != SocketError.Success)
//						{
//							ProgramLog.Debug.Log ("{0}: error while sending ({1})", remoteAddress, error);
//							kill = true;
//						}
//						else if (kick)
//						{
//							Thread.Sleep (250);
//							kill = true;
//						}
//					}
//				}
//				catch (SocketException e)
//				{
//					ProgramLog.Debug.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
//					kill = true;
//				}
//				catch (ObjectDisposedException e)
//				{
//					ProgramLog.Debug.Log ("{0}: exception while sending ({1})", remoteAddress, e.Message);
//					kill = true;
//				}
//				catch (Exception e)
//				{
//					ProgramLog.Log (e, "Exception within WriteThread of slot " + whoAmI);
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
//					//socket.SafeShutdown ();
//					socket.SafeClose();
//					
//					var deadClients = Netplay.deadClients;
//					lock (deadClients) deadClients.Enqueue (socket);
//				}
//			}
//		}
	}
}
