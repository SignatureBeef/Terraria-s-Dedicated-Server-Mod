using tdsm.api;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class AnglerQuest : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.ANGLER_QUEST;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            if (Main.netMode != 1)
            {
                return;
            }
            Main.anglerQuest = (int)ReadByte(readBuffer);
            Main.anglerQuestFinished = ReadBoolean(readBuffer);
        }
    }
}
