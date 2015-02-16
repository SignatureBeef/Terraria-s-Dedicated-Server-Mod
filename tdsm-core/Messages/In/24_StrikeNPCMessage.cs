using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class StrikeNPCMessage : SlotMessageHandler
    {
        public StrikeNPCMessage()
        {
            ValidStates = SlotState.NONE; //this packet isn't actually used anymore
        }

        public override Packet GetPacket()
        {
            return Packet.STRIKE_NPC;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num59 = (int)ReadInt16(readBuffer);
            int num60 = (int)ReadByte(readBuffer);
            if (Main.netMode == 2)
            {
                num60 = whoAmI;
            }
            Player player7 = Main.player[num60];
            Main.npc[num59].StrikeNPC(player7.inventory[player7.selectedItem].damage, player7.inventory[player7.selectedItem].knockBack, player7.direction, false, false);
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(24, -1, whoAmI, String.Empty, num59, (float)num60, 0f, 0f, 0);
                NewNetMessage.SendData(23, -1, -1, String.Empty, num59, 0f, 0f, 0f, 0);
                return;
            }
        }
    }
}
