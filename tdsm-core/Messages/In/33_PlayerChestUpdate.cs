using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerChestUpdate : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_CHEST_UPDATE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //TODO implement user in tange

            int num87 = (int)ReadInt16(readBuffer);
            int chestX = (int)ReadInt16(readBuffer);
            int chestY = (int)ReadInt16(readBuffer);
            int num88 = (int)ReadByte(readBuffer);
            string text5 = string.Empty;
            if (num88 != 0)
            {
                if (num88 <= 20)
                {
                    text5 = ReadString(readBuffer);
                }
                else
                {
                    if (num88 != 255)
                    {
                        num88 = 0;
                    }
                }
            }
            if (num88 != 0)
            {
                int chest = Main.player[whoAmI].chest;
                Chest chest2 = Main.chest[chest];
                chest2.name = text5;
                NewNetMessage.SendData(69, -1, whoAmI, text5, chest, (float)chest2.x, (float)chest2.y, 0f, 0);
            }
            Main.player[whoAmI].chest = num87;
            return;
        }
    }
}
