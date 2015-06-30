using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class Packet66 : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PACKET_66;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num151 = (int)ReadByte(readBuffer);
            int num152 = (int)ReadInt16(readBuffer);
            if (num152 <= 0)
            {
                return;
            }
            Player player14 = Main.player[num151];
            player14.statLife += num152;
            if (player14.statLife > player14.statLifeMax2)
            {
                player14.statLife = player14.statLifeMax2;
            }
            player14.HealEffect(num152, false);
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(66, -1, whoAmI, "", num151, (float)num152, 0f, 0f, 0);
                return;
            }
            return;
        }
    }
}
