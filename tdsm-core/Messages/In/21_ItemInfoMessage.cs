using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ItemInfoMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.ITEM_INFO;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num48 = (int)ReadInt16(readBuffer);
            var position = ReadVector2(readBuffer);
            var velocity = ReadVector2(readBuffer);
            int stack2 = (int)ReadInt16(readBuffer);
            int pre = (int)ReadByte(readBuffer);
            int num49 = (int)ReadByte(readBuffer);
            int num50 = (int)ReadInt16(readBuffer);

            {
                if (num50 == 0)
                {
                    if (num48 < 400)
                    {
                        Main.item[num48].active = false;
                        NewNetMessage.SendData(21, -1, -1, String.Empty, num48, 0f, 0f, 0f, 0);
                        return;
                    }
                    return;
                }
                else
                {
                    bool flag7 = false;
                    if (num48 == 400)
                    {
                        flag7 = true;
                    }
                    if (flag7)
                    {
                        Item item2 = new Item();
                        item2.netDefaults(num50);
                        num48 = Item.NewItem((int)position.X, (int)position.Y, item2.width, item2.height, item2.type, stack2, true, 0, false);
                    }
                    Item item3 = Main.item[num48];
                    item3.netDefaults(num50);
                    item3.Prefix(pre);
                    item3.stack = stack2;
                    item3.position = position;
                    item3.velocity = velocity;
                    item3.active = true;
                    item3.owner = Main.myPlayer;
                    if (flag7)
                    {
                        NewNetMessage.SendData(21, -1, -1, String.Empty, num48, 0f, 0f, 0f, 0);
                        if (num49 == 0)
                        {
                            Main.item[num48].ownIgnore = whoAmI;
                            Main.item[num48].ownTime = 100;
                        }
                        Main.item[num48].FindOwner(num48);
                        return;
                    }
                    NewNetMessage.SendData(21, -1, whoAmI, String.Empty, num48, 0f, 0f, 0f, 0);
                    return;
                }
            }
        }
    }
}
