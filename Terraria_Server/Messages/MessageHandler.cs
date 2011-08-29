using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Logging;
using Terraria_Server.Networking;

namespace Terraria_Server.Messages
{
	public abstract class MessageHandler
	{
		public MessageHandler ()
		{
			ValidStates = SlotState.PLAYING;
		}
		
		public SlotState ValidStates { get; protected set; }
		public SlotState IgnoredStates { get; protected set; }
	
		public abstract Packet GetPacket();
		
		public abstract void Process (ClientConnection conn, byte[] readBuffer, int length, int pos);
	}
	
	public abstract class SlotMessageHandler : MessageHandler
	{
		public abstract void Process (int whoAmI, byte[] readBuffer, int length, int num);
	
		public override void Process (ClientConnection conn, byte[] readBuffer, int length, int pos)
		{
			var slot = conn.SlotIndex;
			if (slot >= 0)
				Process (slot, readBuffer, length, pos);
			else
				ProgramLog.Error.Log ("Attempt to process packet {0} before slot assignment.", GetPacket());
		}
	}
}
