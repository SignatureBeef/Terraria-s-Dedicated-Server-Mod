using System;
using System.Net.Sockets;

using Terraria_Server.Logging;

namespace Terraria_Server.Networking
{
	public class ClientConnection : Connection
	{
		int assignedSlot = -1;
		int messageLength = 0;
		
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
		
		public ClientConnection (Socket socket) : base(socket)
		{
			//var buf = NetMessage.buffer[id];
			
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
	}
}

