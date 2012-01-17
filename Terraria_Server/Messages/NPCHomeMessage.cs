using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.WorldMod;

namespace Terraria_Server.Messages
{
	public class NPCHomeMessage : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.NPC_HOME;
		}

		public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
		{
			short npcId = BitConverter.ToInt16(readBuffer, num);
			num += 2;
			short homeTileX = BitConverter.ToInt16(readBuffer, num);
			num += 2;
			short homeTileY = BitConverter.ToInt16(readBuffer, num);
			num += 2;
			var homed = readBuffer[num++] == 0;

			if (homed) //Kick out if they are
			{
				Main.npcs[(int)npcId].homeless = true;
				return;
			}

			WorldModify.MoveRoom(null, (int)homeTileX, (int)homeTileY, (int)npcId);
		}
	}
}
