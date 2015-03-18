using System;

namespace Terraria_Server.Messages
{
    public class ItemOwnerInfoMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.ITEM_OWNER_INFO;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            short itemIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte owner = readBuffer[num];
            Item item = Main.item[(int)itemIndex];

            if (item.Owner != whoAmI)
            {
                return;
            }

            item.Owner = 255;
            item.KeepTime = 15;
            NetMessage.SendData(22, -1, -1, "", (int)itemIndex);
        }
    }
}
