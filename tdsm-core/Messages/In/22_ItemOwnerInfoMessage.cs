using System;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ItemOwnerInfoMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.ITEM_OWNER_INFO;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num51 = (int)ReadInt16(readBuffer);
            int num52 = (int)ReadByte(readBuffer);
            if (Main.item[num51].owner != whoAmI)
            {
                return;
            }
            Main.item[num51].owner = num52;
            if (num52 == Main.myPlayer)
            {
                Main.item[num51].keepTime = 15;
            }
            else
            {
                Main.item[num51].keepTime = 0;
            }
            if (Main.netMode == 2)
            {
                Main.item[num51].owner = 255;
                Main.item[num51].keepTime = 15;
                NewNetMessage.SendData(22, -1, -1, String.Empty, num51, 0f, 0f, 0f, 0);
                return;
            }
            return;
        }
    }
}
