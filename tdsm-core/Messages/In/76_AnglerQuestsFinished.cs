using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class AnglerQuestsFinished : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.ANGLER_QUESTS_FINISHED;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num157 = (int)ReadByte(readBuffer);
            if (num157 == Main.myPlayer && !Main.ServerSideCharacter)
            {
                return;
            }
            if (Main.netMode == 2)
            {
                num157 = whoAmI;
            }
            Player player15 = Main.player[num157];
            player15.anglerQuestsFinished = ReadInt32(readBuffer);
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(76, -1, whoAmI, "", num157, 0f, 0f, 0f, 0);
                return;
            }
            return;
        }
    }
}
