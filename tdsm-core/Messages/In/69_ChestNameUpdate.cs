using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ChestNameUpdate : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.CHEST_NAME_UPDATE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            var chestId = (int)ReadInt16(readBuffer);
            var x = (int)ReadInt16(readBuffer);
            var y = (int)ReadInt16(readBuffer);

            if (Main.netMode == 1)
            {
                if (chestId < 0 || chestId >= 1000) return;

                var chest = Main.chest[chestId];
                if (chest == null)
                {
                    chest = new Chest(false);
                    chest.x = x;
                    chest.y = y;
                    Main.chest[chestId] = chest;
                }
                else if (chest.x != x || chest.y != y) return;

                chest.name = ReadString(readBuffer);
            }
            else
            {
                if (chestId < -1 || chestId >= 1000) return;

                if (chestId == -1)
                {
                    chestId = Chest.FindChest(x, y);
                    if (chestId == -1) return;
                }

                var chest = Main.chest[chestId];
                if (chest.x != x || chest.y != y) return;

                NewNetMessage.SendData(Packet.CHEST_NAME_UPDATE, whoAmI, -1, chest.name, chestId, (float)x, (float)y, 0f, 0);
            }
        }
    }
}
