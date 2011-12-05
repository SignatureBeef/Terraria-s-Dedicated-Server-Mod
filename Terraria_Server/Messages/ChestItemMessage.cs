using System;
using System.Text;
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
            int chestIndex = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int contentsIndex = (int)readBuffer[num++];
            int stackSize = (int)readBuffer[num++];
            int prefix = readBuffer[num++];
            int netID = BitConverter.ToInt16 (readBuffer, num);

            if (Main.chest[chestIndex] == null)
            {
                Main.chest[chestIndex] = new Chest();
            }
			
            var item = Item.netDefaults(netID);
            item.SetPrefix (prefix);
            item.Stack = stackSize;
            Main.chest[chestIndex].contents[contentsIndex] = item;
        }
    }
}
