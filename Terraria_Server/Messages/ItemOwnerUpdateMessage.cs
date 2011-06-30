using System;


namespace Terraria_Server.Messages
{
    public class ItemOwnerUpdateMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.ITEM_OWNER_UPDATE;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short itemIndex = BitConverter.ToInt16(readBuffer, num);
            Main.item[(int)itemIndex].Owner = 255;
            NetMessage.SendData(22, -1, -1, "", (int)itemIndex);
        }
    }
}
