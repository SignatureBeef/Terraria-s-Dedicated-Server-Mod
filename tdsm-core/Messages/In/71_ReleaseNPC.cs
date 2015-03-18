using tdsm.api;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ReleaseNPC : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.RELEASE_NPC;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            if (Main.netMode != 2)
            {
                return;
            }
            int x3 = ReadInt32(readBuffer);
            int y3 = ReadInt32(readBuffer);
            int type5 = (int)ReadInt16(readBuffer);
            byte style2 = ReadByte(readBuffer);
            NPC.ReleaseNPC(x3, y3, type5, (int)style2, whoAmI);
        }
    }
}
