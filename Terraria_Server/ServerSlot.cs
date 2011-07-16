using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;
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
		public volatile Socket socket;
		public volatile SlotState state;
		
		public int whoAmI;
		public String statusText2;
		public int statusCount;
		public int statusMax;
		public string remoteAddress;
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
		private Thread         writeThread;
		private AutoResetEvent writeSignal;
		
		public bool Connected
		{
			get
			{
				try
				{
					return (socket != null && socket.Connected);
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
			this.writeQueue = new Queue<byte[]> ();
			this.remoteAddress = "<unknown>";

			for (int i = 0; i < Main.maxSectionsX; i++)
			{
				for (int j = 0; j < Main.maxSectionsY; j++)
				{
					this.tileSection[i, j] = false;
				}
			}
			
			var oldPlayer = Main.players[this.whoAmI];
			if (oldPlayer != null && state != SlotState.VACANT)
			{
				NetMessage.OnPlayerLeft (oldPlayer, announced);
			}
			announced = false;
			
			if (this.whoAmI < 255)
			{
				Main.players[this.whoAmI] = new Player();
			}
			
			this.timeOut = 0;
			this.statusCount = 0;
			this.statusMax = 0;
			this.statusText2 = "";
			this.statusText = "";
			this.name = "Anonymous";
			this.state = SlotState.VACANT;
			
			this.SpamClear();
			NetMessage.buffer[this.whoAmI].Reset();
			
			socket.SafeClose ();
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
			
			Program.tConsole.WriteLine ("{0} @ {1}: disconnecting for: {2}", remoteAddress, whoAmI, reason);
			if (state != SlotState.SHUTDOWN)
			{
				NetMessage.SendData (2, whoAmI, -1, reason);
				state = SlotState.KICK;
			}
		}
		
		public void Send (byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentException ("Data to send cannot be null");
			}
	
			lock (writeQueue)
			{
				if (writeThread == null)
				{
					writeThread = new Thread (this.WriteThread);
					writeThread.IsBackground = true;
					writeThread.Start ();
				}

				writeQueue.Enqueue (data);
			}
			writeSignal.Set ();
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
			
			var copy = new byte [length];
			Array.Copy (data, offset, copy, 0, length);
	
			lock (writeQueue)
			{
				if (writeThread == null)
				{
					writeThread = new Thread (this.WriteThread);
					writeThread.IsBackground = true;
					writeThread.Start ();
				}

				writeQueue.Enqueue (copy);
			}
			writeSignal.Set ();
		}

		const int WRITE_THREAD_BATCH_SIZE = 32;
		internal void WriteThread ()
		{
			byte[][] list = new byte[WRITE_THREAD_BATCH_SIZE][];
			while (true)
			{
				try
				{
					int items = 0;
					bool kick = false;

					lock (writeQueue)
					{
						while (writeQueue.Count > 0)
						{
							list[items++] = writeQueue.Dequeue();
							if (items == WRITE_THREAD_BATCH_SIZE) break;
						}
						
						kick = (state == SlotState.KICK) && (writeQueue.Count == 0);
					}
					
					if (state == SlotState.SHUTDOWN)
					{
						writeQueue = new Queue<byte[]> ();
						socket.Close();
					}
					else if (items == 0)
					{
						writeSignal.WaitOne();
						continue;
					}

					try
					{
						for (int i = 0; i < items; i++)
						{
							int count = 0;
							int size = list[i].Length;
							while (size - count > 0)
								count += socket.Send (list[i], count, size - count, 0);
								
							list[i] = null;
							NetMessage.buffer[this.whoAmI].spamCount--;
							if (this.statusMax > 0)
							{
								this.statusCount++;
							}
						}
					}
					finally
					{
						if (kick)
							try
							{
								Thread.Sleep (250);
								socket.Shutdown (SocketShutdown.Both);
								socket.Close ();
							}
							catch {}
					}
					
				}
				catch (SocketException e)
				{
					Program.tConsole.WriteLine("{0}: exception while sending ({1})", remoteAddress, e.Message);
				}
				catch (ObjectDisposedException e)
				{
					Program.tConsole.WriteLine("{0}: exception while sending ({1})", remoteAddress, e.Message);
				}
				catch (Exception e)
				{
					Program.tConsole.WriteLine("Exception within WriteThread:");
					Program.tConsole.WriteLine(e.Message);
				}
			}
		}
	}
}
