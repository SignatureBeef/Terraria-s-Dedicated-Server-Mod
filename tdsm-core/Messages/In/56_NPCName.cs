using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class NPCName : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.NPC_NAME;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num132 = (int)ReadInt16(readBuffer);
            if (num132 < 0 || num132 >= 200)
            {
                return;
            }
            if (Main.netMode == 1)
            {
                Main.npc[num132].displayName = ReadString(readBuffer);
                return;
            }
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(56, whoAmI, -1, Main.npc[num132].displayName, num132, 0f, 0f, 0f, 0);
                return;
            }
        }
    }
}
