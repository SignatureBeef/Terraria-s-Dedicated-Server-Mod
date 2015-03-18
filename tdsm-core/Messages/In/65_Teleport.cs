using tdsm.api;
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
            var flags = (BitsByte)ReadByte(readBuffer);
            ReadInt16(readBuffer);
            var newPos = ReadVector2(readBuffer);

            int type = 0;
            int style = 0;
            if (flags[0]) type++;
            if (flags[1]) type += 2;
            if (flags[2]) style++;
            if (flags[3]) style++;

            if (type == 0)
                Main.player[whoAmI].Teleport(newPos, style);
            else if (type == 1)
                Main.npc[whoAmI].Teleport(newPos, style);

            if (Main.netMode == 2 && type == 0)
            {
                NewNetMessage.SendData(65, -1, whoAmI, "", 0, (float)whoAmI, newPos.X, newPos.Y, style);
            }
        }
    }
}
