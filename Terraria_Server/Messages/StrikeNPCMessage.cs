using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class StrikeNPCMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.STRIKE_NPC;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short npcIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte playerIndex = readBuffer[num];

            if (Main.netMode == 2)
            {
                playerIndex = (byte)whoAmI;
            }

            Player player = Main.players[(int)playerIndex];
            Main.npcs[(int)npcIndex].StrikeNPC(player.inventory[player.selectedItemIndex].Damage, player.inventory[player.selectedItemIndex].KnockBack, player.direction);
            
            if (Main.netMode == 2)
            {
                NetMessage.SendData(24, -1, whoAmI, "", (int)npcIndex, (float)playerIndex);
                NetMessage.SendData(23, -1, -1, "", (int)npcIndex);
            }
        }
    }
}
