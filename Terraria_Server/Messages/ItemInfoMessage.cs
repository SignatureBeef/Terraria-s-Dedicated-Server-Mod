using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Collections;

namespace Terraria_Server.Messages
{
    public class ItemInfoMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.ITEM_INFO;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int start = num - 1;
            short itemIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            float num39 = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float num40 = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float x3 = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float y2 = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            byte stackSize = readBuffer[num++];
            byte Prefix = readBuffer[num++];

            short itemId = BitConverter.ToInt16(readBuffer, num);

            //string String4 = Networking.StringCache.FindOrMake (new ArraySegment<byte> (readBuffer, num, length - num + start));

            Item item = Main.item[(int)itemIndex];

            if (itemId == 0)
            {
                if (itemIndex < 200)
                {
                    item.Active = false;
                    NetMessage.SendData(21, -1, -1, "", (int)itemIndex);
                }
            }
            else
            {
                bool isNewItem = (itemIndex == 200);
                if(isNewItem)
                {
                    Item newItem = Item.netDefaults(itemId);
                    itemIndex = (short)Item.NewItem((int)num39, (int)num40, newItem.Width, newItem.Height, newItem.Type, (int)stackSize, true);
                }

                Main.item[(int)itemIndex] = Item.netDefaults(itemId);
                item = Main.item[(int)itemIndex];
                //item.Prefix = Prefix;
				item.SetPrefix(Prefix);
                item.Stack = (int)stackSize;
                item.Position.X = num39;
                item.Position.Y = num40;
                item.Velocity.X = x3;
                item.Velocity.Y = y2;
                item.Active = true;
                item.Owner = Main.myPlayer;

                if (isNewItem)
                {
                    NetMessage.SendData(21, -1, -1, "", (int)itemIndex);
                    item.OwnIgnore = whoAmI;
                    item.OwnTime = 100;
                    item.FindOwner((int)itemIndex);
                    return;
                }
                NetMessage.SendData(21, -1, whoAmI, "", (int)itemIndex);
            }
        }
    }
}
