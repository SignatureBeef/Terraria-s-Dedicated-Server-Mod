using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class ItemInfoMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.ITEM_INFO;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
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

            string string4 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);

            Item item = Main.item[(int)itemIndex];
            if (Main.netMode == 1)
            {
                if (string4 == "0")
                {
                    item.Active = false;
                    return;
                }
                item.SetDefaults(string4);
                item.Stack = (int)stackSize;
                item.Position.X = num39;
                item.Position.Y = num40;
                item.Velocity.X = x3;
                item.Velocity.Y = y2;
                item.Active = true;
                item.Wet = Collision.WetCollision(item.Position, item.Width, item.Height);
            }
            else
            {
                if (string4 == "0")
                {
                    if (itemIndex < 200)
                    {
                        item.Active = false;
                        NetMessage.SendData(21, -1, -1, "", (int)itemIndex);
                    }
                }
                else
                {
                    bool isNewItem = false;
                    if (itemIndex == 200)
                    {
                        isNewItem = true;
                        Item newItem = new Item();
                        newItem.SetDefaults(string4);
                        itemIndex = (short)Item.NewItem((int)num39, (int)num40, newItem.Width, newItem.Height, newItem.Type, (int)stackSize, true);
                        item = Main.item[(int)itemIndex];
                    }

                    item.SetDefaults(string4);
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
}
