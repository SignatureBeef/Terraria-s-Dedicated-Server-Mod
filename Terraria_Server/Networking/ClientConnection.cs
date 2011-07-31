using System;
using System.Net.Sockets;
using System.Collections.Generic;

using Terraria_Server.Logging;

namespace Terraria_Server.Networking
{
	public class ClientConnection : Connection
	{
		int assignedSlot = -1;
		int messageLength = 0;
		int indexInAll = -1;
		
		byte[] sideBuffer;
		int    sideBytes;
		int    sideLength;
		
		volatile SlotState state = SlotState.VACANT;
		
		public SlotState State
		{
			get { return state; }
			set { state = value; }
		}
		
		public int SlotIndex
		{
			get { return assignedSlot; }
			internal set { assignedSlot = value; }
		}
		
		public static List<ClientConnection> All { get; private set; }
		
		static ClientConnection ()
		{
			All = new List<ClientConnection> ();
		}
		
		public ClientConnection (Socket socket) : base(socket)
		{
			//var buf = NetMessage.buffer[id];
			//socket.SendBufferSize = 128000;
			lock (All)
			{
				indexInAll = All.Count;
				All.Add (this);
			}
			StartReceiving (new byte [4192]);
		}
		
		public void ProcessSideBuffer ()
		{
			DecodeMessages (sideBuffer, ref sideBytes, ref sideLength);
			sideBuffer = null;
			sideBytes = 0;
			sideLength = 0;
		}
		
		protected override void ProcessRead ()
		{
			//ProgramLog.Log ("Read (total={0}).", recvBytes);
			DecodeMessages (recvBuffer, ref recvBytes, ref messageLength);
			//ProgramLog.Log ("After read (total={0}).", recvBytes);
		}
		
		protected override void HandleClosure (SocketError err)
		{
			if (assignedSlot >= 0)
			{
				ProgramLog.Users.Log ("{0} @ {1}: connection closed ({2}).", RemoteAddress, assignedSlot, err);
				Netplay.slots[assignedSlot].Reset ();
				assignedSlot = -1;
			}
			else
				ProgramLog.Users.Log ("{0}: connection closed ({2}).", RemoteAddress, err);
			
			FreeSectionBuffer ();
			
			lock (All)
			{
				if (indexInAll == All.Count - 1)
				{
					All.RemoveAt (All.Count - 1);
				}
				else
				{
					var other = All[All.Count - 1];
					other.indexInAll = indexInAll;
					All[indexInAll] = other;
				}
			}
		}
		
		NetMessage sectionBuffer;
		
		protected void FreeSectionBuffer ()
		{
			if (sectionBuffer != null)
			{
				var buf = sectionBuffer;
				sectionBuffer = null;
				FreeSectionBuffer (buf);
			}
		}
		
		protected override ArraySegment<byte> SerializeMessage (Message msg)
		{
			switch (msg.kind)
			{
				case 3:
				{
					// TODO: optimize further
					var buf = TakeSectionBuffer ();
					var sX = (msg.param >> 16) * 200;
					var sY = (msg.param & 0xffff) * 150;
					
					for (int y = sY; y < sY + 150; y++)
					{
						buf.SendTileRow (200, sX, y);
					}
					
					sectionBuffer = buf;
					ProgramLog.Debug.Log ("{0} @ {1}: Sending section ({2}, {3}) of {4} bytes.", RemoteAddress, assignedSlot, sX, sY, buf.Segment.Count);
					//System.Threading.Thread.Sleep (100);
					
					return buf.Segment;
				}
			}
			return new ArraySegment<byte> ();
		}
		
		protected override void MessageSendCompleted ()
		{
			FreeSectionBuffer ();
		}
		
		public void DecodeMessages (byte[] readBuffer, ref int totalData, ref int msgLen)
		{
			int processed = 0;
			
			if (totalData >= 4)
			{
				if (msgLen == 0)
				{
					msgLen = BitConverter.ToInt32 (readBuffer, 0) + 4;
					
					if (msgLen <= 4 || msgLen > 4096)
					{
						Kick ("Client sent invalid network message (" + msgLen + ")");
						msgLen = 0;
					}
				}
				while (totalData >= msgLen + processed && msgLen > 0)
				{
					if (state == SlotState.PLAYER_AUTH && msgLen > 4
						&& (Packet) readBuffer[processed + 4] != Packet.PASSWORD_RESPONSE)
					{
						// put player packets aside until password response
						
						if (sideBytes + msgLen > 4096)
						{
							Kick ("Player data too big.");
							return;
						}
						
						if (sideBuffer == null) sideBuffer = new byte [4096];
						
						Buffer.BlockCopy (readBuffer, processed, sideBuffer, sideBytes, msgLen);
						
						sideBytes += msgLen;
					}
					else
					{
						NetMessage.buffer[assignedSlot].GetData (readBuffer, processed + 4, msgLen - 4);
						// GET DATA GOES HERE
						// msgBuf.GetData (readBuffer, processed + 4, msgLen - 4);
					}
						

					processed += msgLen;
					if (totalData - processed >= 4)
					{
						msgLen = BitConverter.ToInt32 (readBuffer, processed) + 4;
						
						if (msgLen <= 4 || msgLen > 4096)
						{
							Kick ("Client sent invalid network message (" + msgLen + ")");
							msgLen = 0;
						}
					}
					else
					{
						msgLen = 0;
					}
				}
				if (processed == totalData)
				{
					totalData = 0;
				}
				else
				{
					if (processed > 0)
					{
						Buffer.BlockCopy (readBuffer, processed, readBuffer, 0, totalData - processed);
						totalData -= processed;
					}
				}
			}
		}

		public void Kick (string reason, bool announce = true)
		{
			if (announce)
			{
				if (assignedSlot >= 0)
					ProgramLog.Admin.Log ("{0} @ {1}: disconnecting for: {2}", RemoteAddress, assignedSlot, reason);
				else
					ProgramLog.Admin.Log ("{0}: disconnecting for: {2}", RemoteAddress, reason);
			}
			
			if (! kicking)
			{
				var msg = NetMessage.PrepareThreadInstance ();
				msg.Disconnect (reason);
				KickAfter (msg.Output);

				state = SlotState.KICK;
			}
		}
		
		public void SendSection (int x, int y)
		{
			Send (new Message { kind = 3, param = (x << 16) | (y & 0xffff) });
		}
		
		static Stack<NetMessage> sectionPool = new Stack<NetMessage> ();
		static int sectionPoolCount = 0;
		
		static NetMessage TakeSectionBuffer ()
		{
			lock (sectionPool)
			{
				if (sectionPool.Count > 0)
					return sectionPool.Pop ();
				sectionPoolCount += 1;
			}
			
			ProgramLog.Debug.Log ("Section pool capacity: {0}", sectionPoolCount);
			return new NetMessage (272250);
		}
		
		static void FreeSectionBuffer (NetMessage buf)
		{
			buf.Clear();
			lock (sectionPool)
				sectionPool.Push (buf);
		}
	}
}

