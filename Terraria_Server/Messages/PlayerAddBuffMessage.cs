using System;

namespace Terraria_Server.Messages
{
	public class PlayerAddBuffMessage : IMessage
	{
		public Packet GetPacket()
		{
			return Packet.PLAYER_ADD_BUFF;
		}
	
		public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
		{
			var playerId = readBuffer[num++];
			
			if (playerId != whoAmI)
			{
				Netplay.slots[whoAmI].Kick ("Cheating detected (PLAYER_ADD_BUFF forgery).");
				return;
			}
			
			var type = readBuffer[num++];
			var time = BitConverter.ToInt16 (readBuffer, num);
			
			//FIXME Main.players[whoAmI].AddBuff (type, time, true);
			
			NetMessage.SendData (55, whoAmI, -1, "", whoAmI, type, time, 0f, 0);
		}
	}
}
