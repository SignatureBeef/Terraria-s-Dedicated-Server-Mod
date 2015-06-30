using tdsm.api;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class CatchNPC : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.CATCH_NPC;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            if (Main.netMode != 2)
            {
                return;
            }
            int i3 = (int)ReadInt16(readBuffer);
            int who = (int)ReadByte(readBuffer);
            if (Main.netMode == 2)
            {
                who = whoAmI;
            }
            NPC.CatchNPC(i3, who);
        }
    }
}
