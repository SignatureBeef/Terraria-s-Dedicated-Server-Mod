using tdsm.api;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class TravelShop : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.TRAVEL_SHOP;
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
