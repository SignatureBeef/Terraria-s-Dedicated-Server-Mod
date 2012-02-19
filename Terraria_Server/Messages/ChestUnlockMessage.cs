using System;

namespace Terraria_Server.Messages
{
	public class ChestUnlockMessage : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.CHEST_UNLOCK;
		}
	
		public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
		{
			byte playerId = readBuffer[num++];
			
			if (playerId != whoAmI)
			{
				NetPlay.slots[whoAmI].Kick ("Cheating detected (CHEST_UNLOCK forgery).");
				return;
			}
			
			byte action = readBuffer[num++];
			
			if (action == 1)
			{
				int x = BitConverter.ToInt32 (readBuffer, num); num += 4;
				int y = BitConverter.ToInt32 (readBuffer, num);
				
				Chest.Unlock (null, x, y);
				
				NetMessage.SendData (52, -1, whoAmI, "", playerId, action, x, y, 0);
				NetMessage.SendTileSquare (-1, x, y, 2);
			}
		}
	}
}
