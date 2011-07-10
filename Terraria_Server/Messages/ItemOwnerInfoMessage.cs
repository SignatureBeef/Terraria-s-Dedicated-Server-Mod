using System;

namespace Terraria_Server.Messages
{
    public class ItemOwnerInfoMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.ITEM_OWNER_INFO;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
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
