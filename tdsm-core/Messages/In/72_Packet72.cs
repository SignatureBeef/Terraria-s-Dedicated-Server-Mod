
using Terraria;
namespace tdsm.core.Messages.In
{
    public class Packet72 : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PACKET_72;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            if (Main.netMode != 1)
            {
                return;
            }
            for (int num156 = 0; num156 < Chest.maxItems; num156++)
            {
                Main.travelShop[num156] = (int)ReadInt16(readBuffer);
            }
        }
    }
}
