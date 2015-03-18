using System;

namespace Terraria_Server.Messages
{
	public class NPCAddBuffMessage : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.NPC_ADD_BUFF;
		}
	
		public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
		{
			var npcId = BitConverter.ToInt16 (readBuffer, num); num += 2;
			var type = readBuffer[num++];
			var time = BitConverter.ToInt16 (readBuffer, num);
			
			Main.npcs[npcId].AddBuff (type, time, true);
			
			NetMessage.SendData (54, -1, -1, "", npcId);
		}
	}
}

