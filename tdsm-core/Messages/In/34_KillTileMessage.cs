using System;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class KillTileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.KILL_TILE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //TODO [ChestBreakReceived]


            byte b6 = ReadByte(readBuffer);
            int num89 = (int)ReadInt16(readBuffer);
            int num90 = (int)ReadInt16(readBuffer);
            int num91 = (int)ReadInt16(readBuffer);

            {
                if (b6 == 0)
                {
                    int num92 = WorldGen.PlaceChest(num89, num90, 21, false, num91);
                    if (num92 == -1)
                    {
                        NewNetMessage.SendData(34, whoAmI, -1, String.Empty, (int)b6, (float)num89, (float)num90, (float)num91, num92);
                        Item.NewItem(num89 * 16, num90 * 16, 32, 32, Chest.itemSpawn[num91], 1, true, 0, false);
                        return;
                    }
                    NewNetMessage.SendData(34, -1, -1, String.Empty, (int)b6, (float)num89, (float)num90, (float)num91, num92);
                    return;
                }
                else
                {
                    Tile tile2 = Main.tile[num89, num90];
                    if (tile2.type != 21)
                    {
                        return;
                    }
                    if (tile2.frameX % 36 != 0)
                    {
                        num89--;
                    }
                    if (tile2.frameY % 36 != 0)
                    {
                        num90--;
                    }
                    int number = Chest.FindChest(num89, num90);
                    WorldGen.KillTile(num89, num90, false, false, false);
                    if (!tile2.active())
                    {
                        NewNetMessage.SendData(34, -1, -1, String.Empty, (int)b6, (float)num89, (float)num90, 0f, number);
                        return;
                    }
                    return;
                }
            }
        }
    }
}
