using System;

namespace Terraria_Server.Messages
{
	public class NPCAddBuffMessage : IMessage
	{
		public Packet GetPacket()
		{
			return Packet.NPC_ADD_BUFF;
		}
	
		public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
		{
			var npcId = BitConverter.ToInt16 (readBuffer, num); num += 2;
			var type = readBuffer[num++];
			var time = BitConverter.ToInt16 (readBuffer, num);
			
			//FIXME Main.npcs[npcId].AddBuff (type, time, true);
			
			NetMessage.SendData (54, -1, -1, "", npcId);
		}
	}
}

