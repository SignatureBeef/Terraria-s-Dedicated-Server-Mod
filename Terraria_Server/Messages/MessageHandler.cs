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
            if (utf8Encoding == null)
            {
                utf8Encoding = Encoding.GetEncoding("utf-8", new EncoderExceptionFallback(), new DecoderExceptionFallback());
            }
		}
		
		public SlotState ValidStates { get; protected set; }
		public SlotState IgnoredStates { get; protected set; }

		private static Encoding utf8Encoding;

		protected bool ParseString(byte[] strBuffer, int offset, int count, out string str)
		{
			try
			{
				str = utf8Encoding.GetString(strBuffer, offset, count).Trim();
			}
			catch (DecoderFallbackException)
			{
				str = null;
				return false;
			}
			return true;
		}

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
