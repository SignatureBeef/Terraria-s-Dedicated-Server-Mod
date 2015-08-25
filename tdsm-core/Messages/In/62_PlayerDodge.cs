using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerDodge : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_DODGE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num142 = (int)ReadByte(readBuffer);
            int num143 = (int)ReadByte(readBuffer);
            if (Main.netMode == 2)
            {
                num142 = whoAmI;
            }
            if (num143 == 1)
            {
                Main.player[num142].NinjaDodge();
            }
            if (num143 == 2)
            {
                Main.player[num142].ShadowDodge();
            }
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(62, -1, whoAmI, "", num142, (float)num143, 0f, 0f, 0);
                return;
            }
            return;
        }
    }
}
