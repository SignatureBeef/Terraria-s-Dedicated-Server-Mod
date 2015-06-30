using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PaintWall : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PAINT_WALL;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num146 = (int)ReadInt16(readBuffer);
            int num147 = (int)ReadInt16(readBuffer);
            byte b12 = ReadByte(readBuffer);
            WorldGen.paintWall(num146, num147, b12, false);
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(64, -1, whoAmI, "", num146, (float)num147, (float)b12, 0f, 0);
                return;
            }
            return;
        }
    }
}
