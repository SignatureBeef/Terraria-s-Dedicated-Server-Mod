using System;
using System.Text;
using Terraria_Server.Shops;
using Terraria_Server.Collections;

namespace Terraria_Server.Messages
{
    public class ChestItemMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.CHEST_ITEM;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int start = num - 1;
            int chestIndex = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int contentsIndex = (int)readBuffer[num++];
            int stackSize = (int)readBuffer[num++];

            string String8 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            
            if (Main.chest[chestIndex] == null)
            {
                Main.chest[chestIndex] = new Chest();
            }
            Main.chest[chestIndex].contents[contentsIndex] = Registries.Item.Create(String8, stackSize);
        }
    }
}
