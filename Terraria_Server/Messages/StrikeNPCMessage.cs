using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class StrikeNPCMessage : SlotMessageHandler
    {
		public StrikeNPCMessage ()
		{
			ValidStates = SlotState.NONE; //this packet isn't actually used anymore
		}
		
        public override Packet GetPacket()
        {
            return Packet.STRIKE_NPC;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
//            short npcIndex = BitConverter.ToInt16(readBuffer, num);
//            num += 2;
//            byte playerIndex = (byte)whoAmI;
//
//            Player player = Main.players[(int)playerIndex];
//            Main.npcs[(int)npcIndex].StrikeNPC(player.inventory[player.selectedItemIndex].Damage, player.inventory[player.selectedItemIndex].KnockBack, player.direction);
//            
//            
//            NetMessage.SendData(24, -1, whoAmI, "", (int)npcIndex, (float)playerIndex);
			//            NetMessage.SendData(23, -1, -1, "", (int)npcIndex);
			short npcIndex = BitConverter.ToInt16(readBuffer, num);
			num += 2;
			byte playerIndex = (byte)whoAmI;
			Player player = Main.players[(int)playerIndex];

			Main.npcs[(int)npcIndex].StrikeNPC(player, player.inventory[player.selectedItemIndex].damage, player.inventory[player.selectedItemIndex].KnockBack, player.direction);

			NetMessage.SendData(24, -1, whoAmI, "", (int)npcIndex, (float)playerIndex, 0f, 0f, 0);
			NetMessage.SendData(23, -1, -1, "", (int)npcIndex, 0f, 0f, 0f, 0);
        }
    }
}
