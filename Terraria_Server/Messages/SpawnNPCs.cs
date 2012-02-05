using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Definitions;

namespace Terraria_Server.Messages
{
	public class SpawnNPCs : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.SPAWN_NPCS;
		}

		public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
		{
			int plr = BitConverter.ToInt32(readBuffer, num);
			num += 4;
			int typeOrInvasion = BitConverter.ToInt32(readBuffer, num);
			num += 4;

			if (typeOrInvasion == (int)NPCType.N04_EYE_OF_CTHULHU ||
				typeOrInvasion == (int)NPCType.N13_EATER_OF_WORLDS_HEAD ||
				typeOrInvasion == (int)NPCType.N50_KING_SLIME ||
				typeOrInvasion == (int)NPCType.N125_RETINAZER ||
				typeOrInvasion == (int)NPCType.N126_SPAZMATISM ||
				typeOrInvasion == (int)NPCType.N134_THE_DESTROYER ||
				typeOrInvasion == (int)NPCType.N127_SKELETRON_PRIME ||
				typeOrInvasion == (int)NPCType.N128_PRIME_CANNON)
			{
				//bool flag8 = true;
				//for (int num123 = 0; num123 < NPC.MAX_NPCS; num123++)
				//{
				//    if (Main.npcs[num123].Active && Main.npcs[num123].Type == typeOrInvasion)
				//        flag8 = false;
				//}

				if (!NPC.IsNPCSummoned(typeOrInvasion))
					NPC.SpawnOnPlayer(plr, typeOrInvasion);
			}
			else
			{
				if (typeOrInvasion >= 0)
					return;

				int num124 = -1;

				if (typeOrInvasion == -1 || typeOrInvasion == -2)
					num124 *= 1;

				if (num124 > 0 && Main.invasionType == 0)
				{
					Main.invasionDelay = 0;
					Main.StartInvasion(typeOrInvasion);
				}
			}
		}
	}
}
