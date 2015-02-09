using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class StrikePlayerMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.STRIKE_PLAYER;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num64 = (int)ReadByte(readBuffer);
            if (Main.netMode == 2 && whoAmI != num64 && (!Main.player[num64].hostile || !Main.player[whoAmI].hostile))
            {
                return;
            }
            int num65 = (int)(ReadByte(readBuffer) - 1);
            int num66 = (int)ReadInt16(readBuffer);
            string text4 = ReadString(readBuffer);
            BitsByte bitsByte9 = ReadByte(readBuffer);
            bool flag8 = bitsByte9[0];
            bool flag9 = bitsByte9[1];
            Main.player[num64].Hurt(num66, num65, flag8, true, text4, flag9);
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(26, -1, whoAmI, text4, num64, (float)num65, (float)num66, (float)(flag8 ? 1 : 0), flag9 ? 1 : 0);
                return;
            }
        }
    }
}
