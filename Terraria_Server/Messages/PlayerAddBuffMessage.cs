using System;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
	public class PlayerAddBuffMessage : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.PLAYER_ADD_BUFF;
		}
	
		public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
		{
			var playerId = readBuffer[num++];
			var type = readBuffer[num++];
			var time = BitConverter.ToInt16 (readBuffer, num);
			
			if (type > 26 || time > 900 /*max buff time*/
				|| (playerId != whoAmI && ((type != 20 && type != 24) || time > 600 /*max debuff time*/)))
			{
				ProgramLog.Debug.Log ("PLAYER_ADD_BUFF: from={0}, for={1}, type={2}, time={3}", whoAmI, playerId, type, time);
				NetPlay.slots[whoAmI].Kick ("Cheating detected (PLAYER_ADD_BUFF forgery).");
				return;
			}
			
			Main.players[whoAmI].AddBuff (type, time, true);
			
			NetMessage.SendData (55, whoAmI, -1, "", whoAmI, type, time, 0f, 0);
		}
	}
}
