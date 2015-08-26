using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PaintTile : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PAINT_TILE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num144 = (int)ReadInt16(readBuffer);
            int num145 = (int)ReadInt16(readBuffer);
            byte b11 = ReadByte(readBuffer);
            WorldGen.paintTile(num144, num145, b11, false);
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(63, -1, whoAmI, "", num144, (float)num145, (float)b11, 0f, 0);
                return;
            }
            return;
        }
    }
}
