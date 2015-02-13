using Microsoft.Xna.Framework;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class Teleport : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.TELEPORT;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            BitsByte bitsByte12 = ReadByte(readBuffer);
            int num148 = (int)ReadInt16(readBuffer);
            if (Main.netMode == 2)
            {
                num148 = whoAmI;
            }
            Vector2 newPos = ReadVector2(readBuffer);
            int num149 = 0;
            int num150 = 0;
            if (bitsByte12[0])
            {
                num149++;
            }
            if (bitsByte12[1])
            {
                num149 += 2;
            }
            if (bitsByte12[2])
            {
                num150++;
            }
            if (bitsByte12[3])
            {
                num150++;
            }
            if (num149 == 0)
            {
                Main.player[num148].Teleport(newPos, num150);
            }
            else
            {
                if (num149 == 1)
                {
                    Main.npc[num148].Teleport(newPos, num150);
                }
            }
            if (Main.netMode == 2 && num149 == 0)
            {
                NewNetMessage.SendData(65, -1, whoAmI, "", 0, (float)num148, newPos.X, newPos.Y, num150);
                return;
            }
            return;
        }
    }
}
