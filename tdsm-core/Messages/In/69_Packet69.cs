using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class Packet69 : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PACKET_69;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {

            int num153 = (int)ReadInt16(readBuffer);
            int num154 = (int)ReadInt16(readBuffer);
            int num155 = (int)ReadInt16(readBuffer);
            if (Main.netMode == 1)
            {
                if (num153 < 0 || num153 >= 1000)
                {
                    return;
                }
                Chest chest3 = Main.chest[num153];
                if (chest3 == null)
                {
                    chest3 = new Chest(false);
                    chest3.x = num154;
                    chest3.y = num155;
                    Main.chest[num153] = chest3;
                }
                else
                {
                    if (chest3.x != num154 || chest3.y != num155)
                    {
                        return;
                    }
                }
                chest3.name = ReadString(readBuffer);
                return;
            }
            else
            {
                if (num153 < -1 || num153 >= 1000)
                {
                    return;
                }
                if (num153 == -1)
                {
                    num153 = Chest.FindChest(num154, num155);
                    if (num153 == -1)
                    {
                        return;
                    }
                }
                Chest chest4 = Main.chest[num153];
                if (chest4.x != num154 || chest4.y != num155)
                {
                    return;
                }
                NewNetMessage.SendData(69, whoAmI, -1, chest4.name, num153, (float)num154, (float)num155, 0f, 0);
                return;
            }
        }
    }
}
